import { Component, Inject, OnInit, Input } from "@angular/core";
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
    selector: "description-reception",
    templateUrl: "./description-reception.component.html",
    standalone: false
})

export class DescriptionReceptionComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  descriptionReception: dataModelsInterfaces.DescriptionReception = {} as dataModelsInterfaces.DescriptionReception;
  ordreFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(100), Validators.min(0)]);
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  calculLimiteConformiteFC: UntypedFormControl = new UntypedFormControl(null);
  libelleFRFRFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  libelleENGBFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  ouiFRFRFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  ouiENGBFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  nonFRFRFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  nonENGBFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  reponseParDefautListFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  //Global lock
  locked: boolean = false;
  saveLocked: boolean = false;
  //Entries
  @Input() refDescriptionReception: string;
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
      Ordre: this.ordreFC,
      Actif: this.actifFC,
      LibelleFRFR: this.libelleFRFRFC,
      LibelleENGB: this.libelleENGBFC,
      OuiFRFR: this.ouiFRFRFC,
      OuiENGB: this.ouiENGBFC,
      NonFRFR: this.nonFRFRFC,
      NonENGB: this.nonENGBFC,
      ReponseParDefautList: this.reponseParDefautListFC
    });
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.ordreFC.disable();
    this.actifFC.disable();
    this.calculLimiteConformiteFC.disable();
    this.libelleFRFRFC.disable();
    this.libelleENGBFC.disable();
    this.ouiFRFRFC.disable();
    this.ouiENGBFC.disable();
    this.nonFRFRFC.disable();
    this.nonENGBFC.disable();
    this.reponseParDefautListFC.disable();
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
    let id = Number.parseInt(this.refDescriptionReception, 10);
    //Load initial data
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getDescriptionReception(id).subscribe(result => {
          //Get data
          this.descriptionReception = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
      else {
        //Add new element
        this.descriptionReception = { RefDescriptionReception: 0 } as dataModelsInterfaces.DescriptionReception
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
    this.ordreFC.setValue(this.descriptionReception.Ordre);
    this.actifFC.setValue(this.descriptionReception.Actif);
    this.libelleFRFRFC.setValue(this.descriptionReception.LibelleFRFR);
    this.libelleENGBFC.setValue(this.descriptionReception.LibelleENGB);
    this.ouiFRFRFC.setValue(this.descriptionReception.OuiFRFR);
    this.ouiENGBFC.setValue(this.descriptionReception.OuiENGB);
    this.nonFRFRFC.setValue(this.descriptionReception.NonFRFR);
    this.nonENGBFC.setValue(this.descriptionReception.NonENGB);
    this.reponseParDefautListFC.setValue(this.descriptionReception.ReponseParDefaut === true ? "yes" : (this.descriptionReception.ReponseParDefaut === false ? "no" : "none"));
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.descriptionReception.Ordre = this.ordreFC.value;
    this.descriptionReception.Actif = this.actifFC.value;
    this.descriptionReception.LibelleFRFR = this.libelleFRFRFC.value;
    this.descriptionReception.LibelleENGB = this.libelleENGBFC.value;
    this.descriptionReception.OuiFRFR = this.ouiFRFRFC.value;
    this.descriptionReception.OuiENGB = this.ouiENGBFC.value;
    this.descriptionReception.NonFRFR = this.nonFRFRFC.value;
    this.descriptionReception.NonENGB = this.nonENGBFC.value;
    this.descriptionReception.ReponseParDefaut = (this.reponseParDefautListFC.value === "yes" ? true : (this.reponseParDefautListFC.value === "no" ? false : null));
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Process
    var url = this.baseUrl + "evapi/descriptionreception";
    //Update 
    this.http
      .post<dataModelsInterfaces.DescriptionReception>(url, this.descriptionReception)
      .subscribe(result => {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
        //back to pristine
        this.form.markAsPristine();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(755) },
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
    var url = this.baseUrl + "evapi/descriptionreception/" + this.descriptionReception.RefDescriptionReception;
    this.http
      .delete(url)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(752), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Lock
        this.lockScreen();
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
    return getCreationModificationTooltipText(this.descriptionReception);
  }
}
