import { Component, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
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

/** 0 or 90 to 365  for delaiAvantDesactivationUtilisateurFC*/
const delaiAvantDesactivationUtilisateurRange: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const delaiAvantDesactivationUtilisateurFC = control;
  return delaiAvantDesactivationUtilisateurFC.value > 0 && delaiAvantDesactivationUtilisateurFC.value < 90 ? { delaiAvantDesactivationUtilisateurRange: true } : null;
}

/** 0 or 90 to 365  for delaiAvantChangementMotDePasseFC*/
const delaiAvantChangementMotDePasseRange: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const delaiAvantChangementMotDePasseFC = control;
  return delaiAvantChangementMotDePasseFC.value > 0 && delaiAvantChangementMotDePasseFC.value < 90 ? { delaiAvantChangementMotDePasseRange: true } : null;
}

@Component({
    selector: "securite",
    templateUrl: "./securite.component.html",
    standalone: false
})

export class SecuriteComponent implements OnInit {
  matcher = new MyErrorStateMatcher();

  //Form
  form: UntypedFormGroup;
  securite: dataModelsInterfaces.Securite = {} as dataModelsInterfaces.Securite;
  delaiAvantDesactivationUtilisateurFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, delaiAvantDesactivationUtilisateurRange, Validators.pattern('^[0-9]+$')]);
  delaiAvantChangementMotDePasseFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, delaiAvantChangementMotDePasseRange, Validators.pattern('^[0-9]+$')]);
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  //Constructor
  constructor(private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      DelaiAvantDesactivationUtilisateur: this.delaiAvantDesactivationUtilisateurFC,
      DelaiAvantChangementMotDePasse: this.delaiAvantChangementMotDePasseFC,
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
    //Load initial data
    this.dataModelService.getSecurite().subscribe(result => {
      //Get data
      this.securite = result;
      //Update form
      this.updateForm();
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.delaiAvantDesactivationUtilisateurFC.setValue(this.securite.DelaiAvantDesactivationUtilisateur);
    this.delaiAvantChangementMotDePasseFC.setValue(this.securite.DelaiAvantChangementMotDePasse);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.securite.DelaiAvantDesactivationUtilisateur = this.delaiAvantDesactivationUtilisateurFC.value;
    this.securite.DelaiAvantChangementMotDePasse = this.delaiAvantChangementMotDePasseFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(nextAction: string) {
    this.saveData();
    //Process
    //Update 
    this.dataModelService.postSecurite(this.securite)
      .subscribe(result => {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.securite);
  }
}
