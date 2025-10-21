import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { SnackbarMsgType } from "../../../globals/enums";

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
    selector: "non-conformite-famille",
    templateUrl: "./non-conformite-famille.component.html",
    standalone: false
})

export class NonConformiteFamilleComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  nonConformiteFamille: dataModelsInterfaces.NonConformiteFamille = {} as dataModelsInterfaces.NonConformiteFamille;
  libelleFRFRFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  libelleENGBFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  incitationQualiteFC: UntypedFormControl = new UntypedFormControl(null);
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      LibelleFRFR: this.libelleFRFRFC,
      LibelleENGB: this.libelleENGBFC,
      Actif: this.actifFC,
      IncitationQualite : this.incitationQualiteFC
    });
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Global lock
    if (1 !== 1) {
      this.lockScreen();
    }
    else {
      //Init
      this.unlockScreen();
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getNonConformiteFamille(id).subscribe(result => {
          //Get data
          this.nonConformiteFamille = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.libelleFRFRFC.setValue(this.nonConformiteFamille.LibelleFRFR);
    this.libelleENGBFC.setValue(this.nonConformiteFamille.LibelleENGB);
    this.actifFC.setValue(this.nonConformiteFamille.Actif);
    this.incitationQualiteFC.setValue(this.nonConformiteFamille.IncitationQualite);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.nonConformiteFamille.LibelleFRFR = this.libelleFRFRFC.value;
    this.nonConformiteFamille.LibelleENGB = this.libelleENGBFC.value;
    this.nonConformiteFamille.Actif = this.actifFC.value ? true : false;
    this.nonConformiteFamille.IncitationQualite = this.incitationQualiteFC.value ? true : false;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Process
    var url = this.baseUrl + "evapi/nonconformitefamille";
    //Update 
    this.http
      .post<dataModelsInterfaces.NonConformiteFamille>(url, this.nonConformiteFamille)
      .subscribe(result => {
        //Redirect to grid and inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(821) },
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
    var url = this.baseUrl + "evapi/nonconformitefamille/" + this.nonConformiteFamille.RefNonConformiteFamille;
    this.http
      .delete(url)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1064), duration: 4000 } as appInterfaces.SnackbarMsg);
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
    return getCreationModificationTooltipText(this.nonConformiteFamille);
  }
}
