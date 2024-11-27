import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { HabilitationLogistique } from "../../../globals/enums";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";

class MyErrorStateMatcher implements ErrorStateMatcher {
  //Constructor
  constructor() { }
  isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    let result: boolean = false;
    result = !!((control && control.invalid));
    return result;
  }
}

@Component({
    selector: "transport",
    templateUrl: "./transport.component.html",
    standalone: false
})

export class TransportComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  transport: dataModelsInterfaces.Transport;
  competitors: any[];
  form: UntypedFormGroup;
  //Global lock
  locked: boolean = true;
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    // create an empty object from the Transport interface
    this.transport = {} as dataModelsInterfaces.Transport;
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      AdresseOrigine: [""],
      AdresseDestination: [""],
      CamionType: [""],
      Transporteur: [""],
      Km: ["", [Validators.required, Validators.max(3000)]],
      PUHTDemande: ["", Validators.max(5000)],
      PUHT: ["", [Validators.required, Validators.max(5000)]],
    });
    this.locked = false;
    if (this.applicationUserContext.connectedUtilisateur.Transporteur) {
      this.form.get("Km").disable();
      this.form.get("PUHT").disable();
    }
    else if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Administrateur) {
      this.form.get("Km").disable();
      this.form.get("PUHT").disable();
      this.form.get("PUHTDemande").disable();
      this.locked = true;
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    let id = +this.activatedRoute.snapshot.params["id"];
    let refAdresseOrigine = this.activatedRoute.snapshot.params["refAdresseOrigine"];
    let refAdresseDestination = this.activatedRoute.snapshot.params["refAdresseDestination"];
    let refTransporteur = this.activatedRoute.snapshot.params["refTransporteur"];
    let RefCamionType = this.activatedRoute.snapshot.params["refCamionType"];
    // get existing object if id exists
    if (id) {
      var url = this.baseUrl + "evapi/transport/" + id;
      this.http.get<dataModelsInterfaces.Transport>(url).subscribe(result => {
        this.transport = result;
        this.updateForm();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      //Get competitors
      url = this.baseUrl + "evapi/transport/getcompetitors";
      this.http.get<any[]>(url, {
        headers: new HttpHeaders()
          .set("RefTransport", id.toString()),
        responseType: "json"
      }).subscribe(result => {
        this.competitors = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    // get existing object if id exists
    else {
      var url = this.baseUrl + "evapi/transport/" + id;
      this.http.get<dataModelsInterfaces.Transport>(url,
        {
          headers: new HttpHeaders()
            .set("refAdresseOrigine", refAdresseOrigine)
            .set("refAdresseDestination", refAdresseDestination)
            .set("refTransporteur", refTransporteur)
            .set("refCamionType", RefCamionType)
        }).subscribe(result => {
          this.transport = result;
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("AdresseOrigine").setValue(this.transport.Parcours.AdresseOrigine.TexteAdresse);
    this.form.get("AdresseDestination").setValue(this.transport.Parcours.AdresseDestination.TexteAdresse);
    this.form.get("CamionType").setValue(this.transport.CamionType.Libelle);
    this.form.get("Transporteur").setValue(this.transport.Transporteur.Libelle);
    this.form.get("Km").setValue(this.transport.Parcours.Km);
    this.form.get("PUHTDemande").setValue(this.transport.PUHTDemande);
    this.form.get("PUHT").setValue(this.transport.PUHT);
    this.form.updateValueAndValidity();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.transport.Parcours.Km = this.form.get("Km").value;
    this.transport.PUHTDemande = this.form.get("PUHTDemande").value;
    this.transport.PUHT = this.form.get("PUHT").value;
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(330) },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {

    var url = this.baseUrl + "evapi/transport/" + this.transport.RefTransport;
    this.http
      .delete(url)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(331), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.applyChanges();
    var url = this.baseUrl + "evapi/transport";
    //Update
    this.http
      .post<dataModelsInterfaces.Transport>(url, this.transport)
      .subscribe(result => {
        this.transport = result;
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.transport);
  }
}
