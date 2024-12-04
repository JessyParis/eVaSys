import { Component } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DomSanitizer } from "@angular/platform-browser";
import { ActivatedRoute, Router } from "@angular/router";
import { UtilsService } from "../../../services/utils.service";
import { BaseFormComponent } from "../../_ancestors/base-form.component";

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

export class SecuriteComponent extends BaseFormComponent<dataModelsInterfaces.Securite> {
  //Form
  form: UntypedFormGroup;
  securite: dataModelsInterfaces.Securite = {} as dataModelsInterfaces.Securite;
  delaiAvantDesactivationUtilisateurFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, delaiAvantDesactivationUtilisateurRange, Validators.pattern('^[0-9]+$')]);
  delaiAvantChangementMotDePasseFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, delaiAvantChangementMotDePasseRange, Validators.pattern('^[0-9]+$')]);
  //Constructor
  constructor(protected activatedRoute: ActivatedRoute
    , protected router: Router
    , private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected utilsService: UtilsService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
    , protected sanitizer: DomSanitizer) {
    super("SecuriteComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
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
  //Init
  ngOnInit() {
    //Load initial data
    this.dataModelService.getSecurite().subscribe(result => {
      //Get data
      this.securite = result;
      this.sourceObj = result;
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
  onSave() {
    this.saveData();
    //Update 
    this.dataModelService.postSecurite(this.securite)
      .subscribe(result => {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
}
