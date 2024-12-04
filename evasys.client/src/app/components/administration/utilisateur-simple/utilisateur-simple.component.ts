import { Component, Inject, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import { ErrorStateMatcher } from "@angular/material/core";
import { AuthService } from "../../../services/auth.service";
import { DomSanitizer } from "@angular/platform-browser";
import { UtilsService } from "../../../services/utils.service";
import { BaseFormComponent } from "../../_ancestors/base-form.component";

//Passwords must be identicals
const pwdIdenticalValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const pwdFC = control.get("Pwd");
  const pwdCheckFC = control.get("PwdCheck");
  return (pwdFC.value || pwdCheckFC.value) && pwdFC.value!=pwdCheckFC.value ? { pwdIdentical: true } : null;
}

@Component({
    selector: "utilisateur-simple",
    templateUrl: "./utilisateur-simple.component.html",
    standalone: false
})

export class UtilisateurSimpleComponent extends BaseFormComponent<dataModelsInterfaces.Utilisateur> {
  @Input() refUtilisateur: number;
  @Input() type: string;
  @Output() askClose = new EventEmitter<any>();
  form: UntypedFormGroup;
  utilisateur: dataModelsInterfaces.Utilisateur = {} as dataModelsInterfaces.Utilisateur;
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  nomFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  loginFC: UntypedFormControl = new UntypedFormControl(null);
  pwdFC: UntypedFormControl = new UntypedFormControl(null, [Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z0-9$@$!%*?&].{11,}')]);
  pwdCheckFC: UntypedFormControl = new UntypedFormControl(null, [Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z0-9$@$!%*?&].{11,}')]);
  eMailFC: UntypedFormControl = new UntypedFormControl(null, [Validators.email]);
  telFC: UntypedFormControl = new UntypedFormControl(null);
  telMobileFC: UntypedFormControl = new UntypedFormControl(null);
  //Required
  pwdRequired: boolean = false;
  emailRequired: boolean = false;
  loginRequired: boolean = false;
  //Visibility
  nomVisible: boolean = false;
  loginVisible: boolean = false;
  emailVisible: boolean = false;
  telVisible: boolean = false;
  telMobileVisible: boolean = false;
  hide: boolean = true;
  //Constructor
  constructor(protected activatedRoute: ActivatedRoute
    , protected router: Router
    , private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected utilsService: UtilsService
    , protected authService: AuthService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
    , protected sanitizer: DomSanitizer) {
    super("AideComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    //Creates the form
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: this.actifFC,
      Nom: this.nomFC,
      Login: this.loginFC,
      Pwd: this.pwdFC,
      PwdCheck: this.pwdCheckFC,
      EMail: this.eMailFC,
      Tel: this.telFC,
      TelMobile: this.telMobileFC
    },
      {
        validator: Validators.compose([pwdIdenticalValidator])
      });
    if (1 != 1) {
      this.lockScreen();
    }
    else {
      this.locked = false;
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //If password reset
    if (this.applicationUserContext.ckiePasswordLost) {
      this.utilisateur.RefUtilisateur = 0;
      this.loginFC.setValue(this.applicationUserContext.ckiePasswordLost);
      this.manageScreen();
    }
    else {
      //Get Utilisateur
      this.dataModelService.getDataModel<dataModelsInterfaces.Utilisateur>(this.refUtilisateur, this.componentName).subscribe(result => {
        //Get data
        this.utilisateur = result;
        this.sourceObj = result;
        //Update form
        this.updateForm();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.nomFC.disable();
    this.loginFC.disable();
    this.pwdFC.disable();
    this.eMailFC.disable();
    this.telFC.disable();
    this.telMobileFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.nomFC.setValue(this.utilisateur.Nom);
    this.loginFC.setValue(this.utilisateur.Login);
    this.eMailFC.setValue(this.utilisateur.EMail);
    this.telFC.setValue(this.utilisateur.Tel);
    this.telMobileFC.setValue(this.utilisateur.TelMobile);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Display
  manageScreen() {
    console.log("managescreen");
    this.loginFC.disable();
    this.loginVisible = true;
    this.nomFC.disable();
    this.nomVisible = true;
    this.eMailFC.disable();
    this.emailRequired = false;
    this.emailVisible = true;
    this.telFC.disable();
    this.telVisible = false;
    this.telMobileFC.disable();
    this.telMobileVisible = false;
    //Email first
    if (!this.utilisateur.EMail) {
      //this.eMailFC.clearValidators();
      //this.eMailFC.setValidators([Validators.email]);
      //this.emailRequired = false;
      //this.eMailFC.enable();
      //this.eMailFC.updateValueAndValidity();
    }
    else {
      this.loginVisible = false;
      //this.eMailFC.clearValidators();
      //this.eMailFC.setValidators([Validators.required, Validators.email]);
      //this.emailRequired = true;
      //this.eMailFC.enable();
      //this.eMailFC.updateValueAndValidity();
    }
    //If password reset
    if (this.applicationUserContext.ckiePasswordLost) {
      this.nomVisible = false
      this.nomFC.disable();
      this.emailVisible = false;
      //this.eMailFC.disable();
      this.loginVisible = false;
    }
    //Validators
    //if (!this.utilisateur.PwdExists || this.applicationUserContext.ckiePasswordLost) {
      this.pwdFC.clearValidators();
      this.pwdFC.setValidators([Validators.required, Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z0-9$@$!%*?&].{11,}')]);
      this.pwdRequired = true;
      this.pwdFC.updateValueAndValidity();
      this.pwdCheckFC.clearValidators();
      this.pwdCheckFC.setValidators([Validators.required, Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z0-9$@$!%*?&].{11,}')]);
      this.pwdCheckFC.updateValueAndValidity();
    //}
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.utilisateur.Nom = this.nomFC.value;
    this.utilisateur.Login = this.loginFC.value;
    this.utilisateur.Pwd = this.pwdFC.value;
    this.utilisateur.EMail = this.eMailFC.value;
    this.utilisateur.Tel = this.telFC.value;
    this.utilisateur.TelMobile = this.telMobileFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.applyChanges();
    //If password reset
    if (this.applicationUserContext.ckiePasswordLost) {
      this.authService.resetPassword(this.utilisateur).subscribe(result => {
        this.utilisateur = result;
        if (this.utilisateur.RefUtilisateur > 0) {
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1097), duration: 4000 } as appInterfaces.SnackbarMsg);
          //delete cookie
          this.applicationUserContext.ckiePasswordLost = "";
        }
        else {
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1098), duration: 6000 } as appInterfaces.SnackbarMsg);
        }
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: this.refUtilisateur });
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext)
      });
    }
    else {
      //Send data
      this.dataModelService.postDataModel<dataModelsInterfaces.Utilisateur>(this.utilisateur, this.componentName)
        .subscribe(result => {
          this.utilisateur = result;
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
          //Emit event for dialog closing
          this.askClose.emit({ type: this.type, ref: this.refUtilisateur });
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
}
