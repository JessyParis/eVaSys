import { Component, Inject, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { HabilitationLogistique, SnackbarMsgType } from "../../../globals/enums";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { ErrorStateMatcher } from "@angular/material/core";

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
    selector: "transport-simple",
    templateUrl: "./transport-simple.component.html",
    standalone: false
})

export class TransportSimpleComponent implements OnInit {
  @Input() refAdresseOrigine: number;
  @Input() refAdresseDestination: number;
  @Input() refTransporteur: number;
  @Input() refCamionType: number;
  @Input() type: string;
  @Output() askClose = new EventEmitter<any>();
  matcher = new MyErrorStateMatcher();
  transport: dataModelsInterfaces.Transport;
  form: UntypedFormGroup;
  //Global lock
  locked: boolean = true;
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.transport = {} as dataModelsInterfaces.Transport;
    // create an empty object from the Transport interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Km: ["", [Validators.required, Validators.max(3000)]],
      PUHT: ["", [Validators.required, Validators.max(5000)]],
    });
    if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Administrateur) {
      this.lockScreen();
    }
    else {
      this.locked = false;
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //Get Transport
    var url = this.baseUrl + "evapi/transport/0";
    this.http.get<dataModelsInterfaces.Transport>(url,
      {
        headers: new HttpHeaders()
          .set("refAdresseOrigine", this.refAdresseOrigine.toString())
          .set("refAdresseDestination", this.refAdresseDestination.toString())
          .set("refTransporteur", this.refTransporteur.toString())
          .set("refCamionType", this.refCamionType.toString())
      }).subscribe(result => {
        this.transport = result;
        //Update the form
        this.updateForm();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("Km").disable();
    this.form.get("PUHT").disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("Km").setValue(this.transport.Parcours.Km);
    this.form.get("PUHT").setValue(this.transport.PUHT);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.transport.Parcours.Km = this.form.get("Km").value;
    this.transport.PUHT = this.form.get("PUHT").value;
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
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: this.transport.RefTransport });
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.transport);
  }
}
