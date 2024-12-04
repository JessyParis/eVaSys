import { Component, ViewEncapsulation } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { UtilsService } from "../../../services/utils.service";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { kendoFontData } from "../../../globals/kendo-utils";
import { DomSanitizer } from "@angular/platform-browser";
import { BaseFormComponent } from "../../_ancestors/base-form.component";

@Component({
    selector: "param-email",
    templateUrl: "./param-email.component.html",
    encapsulation: ViewEncapsulation.None,
    standalone: false
})

export class ParamEmailComponent extends BaseFormComponent<dataModelsInterfaces.ParamEmail> {
  //Variables
  paramEmail: dataModelsInterfaces.ParamEmail = {} as dataModelsInterfaces.ParamEmail;
  //Form
  form: UntypedFormGroup;
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  rttClientFC: UntypedFormControl = new UntypedFormControl(null);
  pOPFC: UntypedFormControl = new UntypedFormControl(null);
  portPOPFC: UntypedFormControl = new UntypedFormControl(null);
  comptePOPFC: UntypedFormControl = new UntypedFormControl(null);
  pwdPOPFC: UntypedFormControl = new UntypedFormControl(null);
  tLSPOPFC: UntypedFormControl = new UntypedFormControl(null);
  enTeteEmailFC: UntypedFormControl = new UntypedFormControl(null);
  piedEmailFC: UntypedFormControl = new UntypedFormControl(null);
  emailExpediteurFC: UntypedFormControl = new UntypedFormControl(null);
  libelleExpediteurFC: UntypedFormControl = new UntypedFormControl(null);
  sMTPFC: UntypedFormControl = new UntypedFormControl(null);
  portSMTPFC: UntypedFormControl = new UntypedFormControl(null);
  compteSMTPFC: UntypedFormControl = new UntypedFormControl(null);
  pwdSMTPFC: UntypedFormControl = new UntypedFormControl(null);
  allowAuthSMTPFC: UntypedFormControl = new UntypedFormControl(null);
  tLSSMTPFC: UntypedFormControl = new UntypedFormControl(null);
  startTLSSMTPFC: UntypedFormControl = new UntypedFormControl(null);
  aRFC: UntypedFormControl = new UntypedFormControl(null);
  aREmailFC: UntypedFormControl = new UntypedFormControl(null);
  aREmailSujetFC: UntypedFormControl = new UntypedFormControl(null);
  aRExpediteurEmailFC: UntypedFormControl = new UntypedFormControl(null);
  aRExpediteurLibelleFC: UntypedFormControl = new UntypedFormControl(null);
  //Misc
  public kendoFontData = kendoFontData;
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
    super("ParamEmailComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: this.actifFC,
      RttClient: this.rttClientFC,
      POP: this.pOPFC,
      PortPOP: this.portPOPFC,
      ComptePOP: this.comptePOPFC,
      PwdPOP: this.pwdPOPFC,
      TLSPOP: this.tLSPOPFC,
      EnTeteEmail: this.enTeteEmailFC,
      PiedEmail: this.piedEmailFC,
      EmailExpediteur: this.emailExpediteurFC,
      LibelleExpediteur: this.libelleExpediteurFC,
      SMTP: this.sMTPFC,
      PortSMTP: this.portSMTPFC,
      CompteSMTP: this.compteSMTPFC,
      PwdSMTP: this.pwdSMTPFC,
      AllowAuthSMTP: this.allowAuthSMTPFC,
      TLSSMTP: this.tLSSMTPFC,
      StartTLSSMTP: this.startTLSSMTPFC,
      AR: this.aRFC,
      AREmail: this.aREmailFC,
      AREmailSujet: this.aREmailSujetFC,
      ARExpediteurEmail: this.aRExpediteurEmailFC,
      ARExpediteurLibelle: this.aRExpediteurLibelleFC,
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    //Check parameters
    if (!isNaN(id)) {
      this.dataModelService.getDataModel<dataModelsInterfaces.ParamEmail>(id, this.componentName).subscribe(result => {
        //Get data
        this.paramEmail = result;
        this.sourceObj = result;
        //Update form
        this.updateForm();
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
        this.router.navigate(["grid"]);
      });
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.actifFC.setValue(this.paramEmail.Actif);
    this.rttClientFC.setValue(this.paramEmail.RttClient);
    this.pOPFC.setValue(this.paramEmail.POP);
    this.portPOPFC.setValue(this.paramEmail.PortPOP);
    this.comptePOPFC.setValue(this.paramEmail.ComptePOP);
    this.pwdPOPFC.setValue(this.paramEmail.PwdPOP);
    this.tLSPOPFC.setValue(this.paramEmail.TLSPOP);
    this.enTeteEmailFC.setValue(this.paramEmail.EnTeteEmail);
    this.piedEmailFC.setValue(this.paramEmail.PiedEmail);
    this.emailExpediteurFC.setValue(this.paramEmail.EmailExpediteur);
    this.libelleExpediteurFC.setValue(this.paramEmail.LibelleExpediteur);
    this.sMTPFC.setValue(this.paramEmail.SMTP);
    this.portSMTPFC.setValue(this.paramEmail.PortSMTP);
    this.compteSMTPFC.setValue(this.paramEmail.CompteSMTP);
    this.pwdSMTPFC.setValue(this.paramEmail.PwdSMTP);
    this.allowAuthSMTPFC.setValue(this.paramEmail.AllowAuthSMTP);
    this.tLSSMTPFC.setValue(this.paramEmail.TLSSMTP);
    this.startTLSSMTPFC.setValue(this.paramEmail.StartTLSSMTP);
    this.aRFC.setValue(this.paramEmail.AR);
    this.aREmailFC.setValue(this.paramEmail.AREmail);
    this.aREmailSujetFC.setValue(this.paramEmail.AREmailSujet);
    this.aRExpediteurEmailFC.setValue(this.paramEmail.ARExpediteurEmail);
    this.aRExpediteurLibelleFC.setValue(this.paramEmail.ARExpediteurLibelle);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.paramEmail.Actif = this.actifFC.value;
    this.paramEmail.RttClient = this.rttClientFC.value;
    this.paramEmail.POP = this.pOPFC.value;
    this.paramEmail.PortPOP = this.portPOPFC.value;
    this.paramEmail.ComptePOP = this.comptePOPFC.value;
    this.paramEmail.PwdPOP = this.pwdPOPFC.value;
    this.paramEmail.TLSPOP = this.tLSPOPFC.value;
    this.paramEmail.EnTeteEmail = this.enTeteEmailFC.value;
    this.paramEmail.PiedEmail = this.piedEmailFC.value;
    this.paramEmail.EmailExpediteur = this.emailExpediteurFC.value;
    this.paramEmail.LibelleExpediteur = this.libelleExpediteurFC.value;
    this.paramEmail.SMTP = this.sMTPFC.value;
    this.paramEmail.PortSMTP = this.portSMTPFC.value;
    this.paramEmail.CompteSMTP = this.compteSMTPFC.value;
    this.paramEmail.PwdSMTP = this.pwdSMTPFC.value;
    this.paramEmail.AllowAuthSMTP = this.allowAuthSMTPFC.value;
    this.paramEmail.TLSSMTP = this.tLSSMTPFC.value;
    this.paramEmail.StartTLSSMTP = this.startTLSSMTPFC.value;
    this.paramEmail.AR = this.aRFC.value;
    this.paramEmail.AREmail = this.aREmailFC.value;
    this.paramEmail.AREmailSujet = this.aREmailSujetFC.value;
    this.paramEmail.ARExpediteurEmail = this.aRExpediteurEmailFC.value;
    this.paramEmail.ARExpediteurLibelle = this.aRExpediteurLibelleFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<dataModelsInterfaces.ParamEmail>(this.paramEmail, this.componentName)
      .subscribe(result => {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(this.ressBeforeDel) },
      autoFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    this.dataModelService.deleteDataModel<dataModelsInterfaces.ParamEmail>(id, this.componentName)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(this.ressAfterDel), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
}
