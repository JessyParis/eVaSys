import { Component, Inject, OnInit, ViewEncapsulation } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { InformationComponent } from "../../dialogs/information/information.component";
import { HabilitationLogistique, ModuleName, HabilitationAnnuaire, HabilitationAdministration, HabilitationMessagerie, HabilitationQualite, HabilitationModuleCollectivite, HabilitationStatistique, SnackbarMsgType } from "../../../globals/enums";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import moment from "moment";
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
    selector: "message",
    templateUrl: "./message.component.html",
    styleUrls: ["./message.component.scss"],
    encapsulation: ViewEncapsulation.None,
    standalone: false
})

export class MessageComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Variables
  message: dataModelsInterfaces.Message = {} as dataModelsInterfaces.Message;
  //Form
  form: UntypedFormGroup;
  formDiffusion: UntypedFormGroup;
  messageTypeList: dataModelsInterfaces.MessageType[];
  moduleList: string[];
  habilitationList: string[];
  utilisateurList: dataModelsInterfaces.UtilisateurList[];
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  importantFC: UntypedFormControl = new UntypedFormControl(null);
  diffusionUniqueFC: UntypedFormControl = new UntypedFormControl(null);
  visualisationConfirmeUniqueFC: UntypedFormControl = new UntypedFormControl(null);
  messageTypeListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  libelleFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  titreFC: UntypedFormControl = new UntypedFormControl(null);
  dDebutFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  dFinFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  corpsFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  corpsHTMLFC: UntypedFormControl = new UntypedFormControl(null);
  moduleListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  habilitationListFC: UntypedFormControl = new UntypedFormControl(null);
  utilisateurListFC: UntypedFormControl = new UntypedFormControl(null);
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
      Important: this.importantFC,
      DiffusionUnique: this.diffusionUniqueFC,
      VisualisationConfirmeUnique: this.visualisationConfirmeUniqueFC,
      MessageTypeList: this.messageTypeListFC,
      Libelle: this.libelleFC,
      Titre: this.titreFC,
      DDebut: this.dDebutFC,
      DFin: this.dFinFC,
      Corps: this.corpsFC,
      CorpsHTML: this.corpsHTMLFC,
      Cmt: this.cmtFC,
    });
    this.formDiffusion = this.fb.group({
      ModuleList: this.moduleListFC,
      HabilitationList: this.habilitationListFC,
      UtilisateurList: this.utilisateurListFC,
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
    //Get MessageTypes
    this.listService.getListMessageType()
      .subscribe(result => {
        this.messageTypeList = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check parameters
    if (!isNaN(id)) {
      this.dataModelService.getMessage(id, false).subscribe(result => {
        //Get data
        this.message = result;
        //Format dates
        this.dataModelService.setMomentFromTextMessage(this.message);
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
    this.actifFC.setValue(this.message.Actif);
    this.importantFC.setValue(this.message.Important);
    this.diffusionUniqueFC.setValue(this.message.DiffusionUnique);
    this.visualisationConfirmeUniqueFC.setValue(this.message.VisualisationConfirmeUnique);
    this.messageTypeListFC.setValue((this.message.MessageType ? this.message.MessageType.RefMessageType : null));
    this.libelleFC.setValue(this.message.Libelle);
    this.titreFC.setValue(this.message.Titre);
    this.dDebutFC.setValue(this.message.DDebut);
    this.dFinFC.setValue(this.message.DFin);
    this.corpsFC.setValue(this.message.Corps);
    this.corpsHTMLFC.setValue(this.message.CorpsHTML);
    this.cmtFC.setValue(this.message.Cmt);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.message.Actif = this.actifFC.value;
    this.message.Important = this.importantFC.value;
    this.message.DiffusionUnique = this.diffusionUniqueFC.value;
    this.message.VisualisationConfirmeUnique = this.visualisationConfirmeUniqueFC.value;
    this.message.MessageType = this.messageTypeList.find(x => x.RefMessageType == this.messageTypeListFC.value);
    this.message.Libelle = this.libelleFC.value;
    this.message.Titre = this.titreFC.value;
    this.message.DDebut = (this.dDebutFC.value == null ? null : moment(this.dDebutFC.value));
    this.message.DFin = (this.dFinFC.value == null ? null : moment(this.dFinFC.value));
    this.message.Corps = this.corpsFC.value;
    this.message.CorpsHTML = this.corpsHTMLFC.value;
    this.message.Cmt = this.cmtFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Process
    var url = this.baseUrl + "evapi/message";
    //Format dates
    this.dataModelService.setTextFromMomentMessage(this.message);
    //Send data
    this.http
      .post<dataModelsInterfaces.Message>(url, this.message)
      .subscribe(result => {
        //Redirect to grid and inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Module selected
  onModuleSelected() {
    //Manage habilitation list
    this.habilitationList = [];
    switch (this.moduleListFC.value) {
      case ModuleName.Administration.toString():
        for (let value in HabilitationAdministration) {
          this.habilitationList.push(value);
        }
        break;
      case ModuleName.Annuaire.toString():
        for (let value in HabilitationAnnuaire) {
          this.habilitationList.push(value);
        }
        break;
      case ModuleName.Logistique.toString():
        for (let value in HabilitationLogistique) {
          this.habilitationList.push(value);
        }
        break;
      case ModuleName.Messagerie.toString():
        for (let value in HabilitationMessagerie) {
          this.habilitationList.push(value);
        }
        break;
      case ModuleName.Qualite.toString():
        for (let value in HabilitationQualite) {
          this.habilitationList.push(value);
        }
        break;
      case ModuleName.ModuleCollectivite.toString():
        for (let value in HabilitationModuleCollectivite) {
          this.habilitationList.push(value);
        }
        break;
      case ModuleName.Statistique.toString():
        for (let value in HabilitationStatistique) {
          this.habilitationList.push(value);
        }
        break;
    }
  }
  //-----------------------------------------------------------------------------------
  //Habilitation selected
  onHabilitationSelected() {
    //Manage utilisateur list
    this.utilisateurList = [];
    if (this.moduleListFC.value !== null && this.habilitationListFC.value !== null) {
      this.listService.getListUtilisateur(null, this.moduleListFC.value, this.habilitationListFC.value, null, false, false, false, true).subscribe(
        result => {
          this.utilisateurList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Add a MessageDiffusion
  addMessageDiffusion() {
    let ok: boolean = false;
    this.saveData();
    //Add new MesageDiffusion
    ok = (this.message.MessageDiffusions.filter(e => e.RefModule === this.moduleListFC.value && e.RefHabilitation === this.habilitationListFC.value
      && (!e.Utilisateur ? null : e.Utilisateur.RefUtilisateur) === this.utilisateurListFC.value
    ).length === 0);
    if (ok && this.moduleListFC.value !== null) {
      //Process
      let mD = {} as dataModelsInterfaces.MessageDiffusion;
      mD.RefModule = this.moduleListFC.value;
      mD.RefHabilitation = this.habilitationListFC.value;
      if (this.utilisateurListFC.value) {
        //Get Utilisateur
        this.dataModelService.getDataModel<dataModelsInterfaces.Utilisateur>(this.utilisateurListFC.value, "UtilisateurComponent").subscribe(result => {
            mD.Utilisateur = result;
            this.message.MessageDiffusions.push(mD);
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
      else {
        this.message.MessageDiffusions.push(mD);
      }
      //RAZ
      this.moduleListFC.setValue(null);
      this.habilitationListFC.setValue(null);
      this.utilisateurListFC.setValue(null);
    }
    else {
      this.updateForm();
      const dialogRef = this.dialog.open(InformationComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(660) },
        restoreFocus: false
      });
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a MessageDiffusion
  onDeleteMessageDiffusion(i: number) {
    if (i !== null) {
      //Save data
      this.saveData();
      //Process
      this.message.MessageDiffusions.splice(i, 1);
      //Update form
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(690) },
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
    var url = this.baseUrl + "evapi/message/" + this.message.RefMessage;
    this.http
      .delete(url)
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
    return getCreationModificationTooltipText(this.message);
  }
}
