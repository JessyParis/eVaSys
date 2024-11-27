import { Component, Inject, OnInit, ViewEncapsulation } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { UtilsService } from "../../../services/utils.service";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { kendoFontData } from "../../../globals/kendo-utils";

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
    selector: "param-email",
    templateUrl: "./param-email.component.html",
    encapsulation: ViewEncapsulation.None,
    standalone: false
})

export class ParamEmailComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
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
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private utilsService: UtilsService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
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
      this.dataModelService.getParamEmail(id).subscribe(result => {
        //Get data
        this.paramEmail = result;
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
    //Send data
    this.dataModelService.postParamEmail(this.paramEmail)
      .subscribe(result => {
        //Redirect to grid and inform user
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
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), paramEmail: this.applicationUserContext.getCulturedRessourceText(690) },
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
    this.dataModelService.deleteParamEmail(this.paramEmail)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(691), duration: 4000 } as appInterfaces.SnackbarMsg);
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
    return getCreationModificationTooltipText(this.paramEmail);
  }
}
