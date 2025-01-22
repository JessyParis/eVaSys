import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, FormArray, ValidationErrors } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import {
  showConfirmToUser, showErrorToUser, showInformationToUser, cmp, cmpDesc, getAttachmentFilename, chartSeriesColors
  , getCreationModificationTooltipText, getContactAdresseServiceFonctions, getContratCollectiviteLabel, getContratLabel
} from "../../../globals/utils";
import { getAdresseListText, getContactAdresseListText } from "../../../globals/data-model-utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataColumnName, HabilitationAnnuaire, HabilitationQualite, MenuName, ModuleName, SearchElementType } from "../../../globals/enums";
import moment from "moment";
import { initialConfig } from "ngx-mask";
import { UploadComponent } from "../../dialogs/upload/upload.component";
import { DownloadService } from "../../../services/download.service";
import { StatistiqueService } from "../../../services/statistique.service";
import { EventEmitterService } from "../../../services/event-emitter.service";
import { fadeInOnEnterAnimation } from "angular-animations";

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
    selector: "entite",
    templateUrl: "./entite.component.html",
    styleUrls: ["./entite.component.scss"],
    animations: [
        fadeInOnEnterAnimation(),
    ],
    standalone: false
})

export class EntiteComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  entite: dataModelsInterfaces.Entite = { Adresses: [], ContactAdresses: [] } as dataModelsInterfaces.Entite;
  entiteTypeList: dataModelsInterfaces.EntiteType[] = [];
  ecoOrganismeList: dataModelsInterfaces.EcoOrganisme[] = [];
  sAGECategorieAchatList: dataModelsInterfaces.SAGECategorieAchat[] = [];
  sAGEConditionLivraisonList: dataModelsInterfaces.SAGEConditionLivraison[] = [];
  sAGEPeriodiciteList: dataModelsInterfaces.SAGEPeriodicite[] = [];
  sAGEModeReglementList: dataModelsInterfaces.SAGEModeReglement[] = [];
  sAGECategorieVenteList: dataModelsInterfaces.SAGECategorieVente[] = [];
  repriseTypeList: dataModelsInterfaces.RepriseType[] = [];
  repreneurList: dataModelsInterfaces.Repreneur[] = [];
  equipementierList: dataModelsInterfaces.Equipementier[] = [];
  fournisseurTOList: dataModelsInterfaces.FournisseurTO[] = [];
  entiteDRList: dataModelsInterfaces.EntiteDR[] = [];
  entiteProcessList: dataModelsInterfaces.EntiteProcess[] = [];
  entiteStandardList: dataModelsInterfaces.EntiteStandard[] = [];
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  sousContratFC: UntypedFormControl = new UntypedFormControl(null);
  entiteTypeListFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  libelleFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  visibiliteAffretementCommunFC: UntypedFormControl = new UntypedFormControl(null);
  surcoutCarburantFC: UntypedFormControl = new UntypedFormControl(null);
  codeEEFC: UntypedFormControl = new UntypedFormControl(null);
  actionnaireProprietaireFC: UntypedFormControl = new UntypedFormControl(null);
  exploitantFC: UntypedFormControl = new UntypedFormControl(null);
  capaciteFC: UntypedFormControl = new UntypedFormControl(null, [Validators.max(70000), Validators.pattern('^[0-9]+$')]);
  populationContratNFC: UntypedFormControl = new UntypedFormControl(null, [Validators.max(10000000), Validators.pattern('^[0-9]+$')]);
  codeValorisationFC: UntypedFormControl = new UntypedFormControl(null);
  ecoOrganismeListFC: UntypedFormControl = new UntypedFormControl(null);
  repartitionMensuelleFC: UntypedFormControl = new UntypedFormControl(null);
  assujettiTVAFC: UntypedFormControl = new UntypedFormControl(null);
  codeTVAFC: UntypedFormControl = new UntypedFormControl(null);
  sAGECodeComptableFC: UntypedFormControl = new UntypedFormControl(null);
  sAGECompteTiersFC: UntypedFormControl = new UntypedFormControl(null);
  sAGECategorieAchatListFC: UntypedFormControl = new UntypedFormControl(null);
  sAGEConditionLivraisonListFC: UntypedFormControl = new UntypedFormControl(null);
  sAGEPeriodiciteListFC: UntypedFormControl = new UntypedFormControl(null);
  sAGEModeReglementListFC: UntypedFormControl = new UntypedFormControl(null);
  sAGECategorieVenteListFC: UntypedFormControl = new UntypedFormControl(null);
  exportSAGEFC: UntypedFormControl = new UntypedFormControl(null);
  autoControleFC: UntypedFormControl = new UntypedFormControl(null);
  reconductionIncitationQualiteFC: UntypedFormControl = new UntypedFormControl(null);
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  horairesFC: UntypedFormControl = new UntypedFormControl(null);
  repriseTypeListFC: UntypedFormControl = new UntypedFormControl(null);
  repreneurListFC: UntypedFormControl = new UntypedFormControl(null);
  dimensionBalleFC: UntypedFormControl = new UntypedFormControl(null);
  equipementierListFC: UntypedFormControl = new UntypedFormControl(null);
  fournisseurTOListFC: UntypedFormControl = new UntypedFormControl(null);
  dRListFC: UntypedFormControl = new UntypedFormControl(null);
  processListFC: UntypedFormControl = new UntypedFormControl(null);
  standardListFC: UntypedFormControl = new UntypedFormControl(null);
  codeTVAFCRequired: boolean = false;
  idNationalFC: UntypedFormControl = new UntypedFormControl(null);
  //Statistics
  statisticMenuName: string = "";
  statisticResult: any[] = [];
  statisticField: string = "";
  statisticCategoryField: string = "";
  statisticTitle: string = "";
  statisticAxisTitle: any = {
    text: ""
  };
  statisticCategoryAxis: any = {};
  public seriesColors: string[] = chartSeriesColors;
  //Functions
  getAdresseListText = getAdresseListText;
  getContactAdresseServiceFonctions = getContactAdresseServiceFonctions;
  getContratCollectiviteLabel = getContratCollectiviteLabel;
  getContratLabel = getContratLabel;
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  //Pipes
  filterAdresses = (adr: dataModelsInterfaces.Adresse, filterGlobalActif: boolean) => {
    return (filterGlobalActif ? adr.Actif && adr.Pays?.Actif : true);
  }
  filterEntiteProduits = (eP: dataModelsInterfaces.EntiteProduit, filterGlobalActif: boolean) => {
    return (filterGlobalActif ? eP.Produit.Actif : true);
  }
  filterContactAdresses = (cA: dataModelsInterfaces.ContactAdresse, filterGlobalActif: boolean) => {
    return (filterGlobalActif ? ((cA.RefAdresse == null || this.entite.Adresses.some(e => e.RefAdresse == cA.RefAdresse && e.Actif))
      && cA.Actif) : true);
  }
  filterEntiteEntites = (eE: dataModelsInterfaces.EntiteEntite, filterGlobalActif: boolean, refEntiteType: number) => {
    return (eE.RefEntiteRttType == refEntiteType && (filterGlobalActif ? (eE.Actif && eE.ActifEntiteRtt) : true));
  }
  filterEntiteDocuments = (dE: dataModelsInterfaces.DocumentEntite, filterGlobalActif: boolean, refEntiteType: number, visibiliteTotale: boolean) => {
    return (dE.DocumentNoFile.VisibiliteTotale == visibiliteTotale);
  }
  filterContrats = (c: dataModelsInterfaces.Contrat) => {
    return (c.ContratEntites.some(e => e.RefEntite == this.entite.RefEntite));
  }
  //Dynamic labels
  centreDeTriLieLabel: string = this.applicationUserContext.getCulturedRessourceText(1150);
  camionTypeLieLabel: string = this.applicationUserContext.getCulturedRessourceText(1169);
  //Visibility
  showAll: boolean = false;
  showAdresses: boolean = false;
  showContactAdresses: boolean = false;
  showContratIncitationQualites: boolean = false;
  showContrats: boolean = false;
  showContratCollectivites: boolean = false;
  showEntiteEntites1: boolean = false;
  showEntiteEntites2: boolean = false;
  showEntiteEntites3: boolean = false;
  showEntiteEntites4: boolean = false;
  showEntiteEntites5: boolean = false;
  showEntiteCamionTypes: boolean = false;
  showEntiteProduits: boolean = false;
  showActions: boolean = false;
  showPrivateDocuments: boolean = false;
  showPublicDocuments: boolean = false;
  showStatistics: boolean = true;
  //Misc
  lastUploadResut: appInterfaces.UploadResponseBody;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private statistiqueService: StatistiqueService
    , private dataModelService: DataModelService
    , private listService: ListService
    , private downloadService: DownloadService
    , private snackBarQueueService: SnackBarQueueService
    , private eventEmitterService: EventEmitterService
    , public dialog: MatDialog) {
    //Init data
    this.entite.RefEntite = 0;
    if (this.entite.Adresses == null) { this.entite.Adresses = []; }
    if (this.entite.ContactAdresses == null) { this.entite.ContactAdresses = []; }
    if (this.entite.Contrats == null) { this.entite.Contrats = []; }
    if (this.entite.ContratCollectivites == null) { this.entite.ContratCollectivites = []; }
    if (this.entite.ContratIncitationQualites == null) { this.entite.ContratIncitationQualites = []; }
    if (this.entite.EntiteEntites == null) { this.entite.EntiteEntites = []; }
    if (this.entite.EntiteDRs == null) { this.entite.EntiteDRs = []; }
    if (this.entite.EntiteProcesss == null) { this.entite.EntiteProcesss = []; }
    if (this.entite.EntiteStandards == null) { this.entite.EntiteStandards = []; }
    if (this.entite.EntiteCamionTypes == null) { this.entite.EntiteCamionTypes = []; }
    if (this.entite.EntiteProduits == null) { this.entite.EntiteProduits = []; }
    if (this.entite.Actions == null) { this.entite.Actions = []; }
    if (this.entite.DocumentEntites == null) { this.entite.DocumentEntites = []; }
    this.entite.Actif = true;
    //Create form
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: this.actifFC,
      SousContrat: this.sousContratFC,
      EntiteTypeList: this.entiteTypeListFC,
      VisibiliteAffretementCommun: this.visibiliteAffretementCommunFC,
      SurcoutCarburant: this.surcoutCarburantFC,
      Libelle: this.libelleFC,
      CodeEE: this.codeEEFC,
      ActionnaireProprietaire: this.actionnaireProprietaireFC,
      Exploitant: this.exploitantFC,
      Capacite: this.capaciteFC,
      PopulationContratN: this.populationContratNFC,
      CodeValorisation: this.codeValorisationFC,
      EcoOrganismeList: this.ecoOrganismeListFC,
      RepartitionMensuelle: this.repartitionMensuelleFC,
      AssujettiTVA: this.assujettiTVAFC,
      CodeTVA: this.codeTVAFC,
      SAGECompteTiers: this.sAGECompteTiersFC,
      SAGECodeComptable: this.sAGECodeComptableFC,
      SAGECategorieAchatList: this.sAGECategorieAchatListFC,
      SAGEConditionLivraisonList: this.sAGEConditionLivraisonListFC,
      SAGEPeriodiciteList: this.sAGEPeriodiciteListFC,
      SAGEModeReglementList: this.sAGEModeReglementListFC,
      SAGECategorieVenteList: this.sAGECategorieVenteListFC,
      ExportSAGE: this.exportSAGEFC,
      AutoControle: this.autoControleFC,
      ReconductionIncitationQualite: this.reconductionIncitationQualiteFC,
      Cmt: this.cmtFC,
      Horaires: this.horairesFC,
      RepriseTypeList: this.repriseTypeListFC,
      RepreneurList: this.repreneurListFC,
      DimensionBalle: this.dimensionBalleFC,
      EquipementierList: this.equipementierListFC,
      FournisseurTOList: this.fournisseurTOListFC,
      DRList: this.dRListFC,
      ProcessList: this.processListFC,
      StandardList: this.standardListFC,
      IdNational: this.idNationalFC,
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
      if (this.entite.RefEntite) { this.entiteTypeListFC.disable(); }
      else { this.entiteTypeListFC.enable(); }
      if (this.entite?.EntiteType?.CodeEE == true) { this.codeEEFC.enable(); }
      else { this.codeEEFC.disable(); }
      if (this.entite?.EntiteType?.PopulationContratN == true) { this.populationContratNFC.enable(); }
      else { this.populationContratNFC.disable(); }
      if (this.entite?.EntiteType?.SAGECodeComptable == true) { this.sAGECodeComptableFC.enable(); }
      else { this.sAGECodeComptableFC.disable(); }
      if (this.entite?.EntiteType?.SAGECompteTiers == true) { this.sAGECompteTiersFC.enable(); }
      else { this.sAGECompteTiersFC.disable(); }
      if (this.entite?.EntiteType?.RefSAGECategorieAchat == true) { this.sAGECategorieAchatListFC.enable(); }
      else { this.sAGECategorieAchatListFC.disable(); }
      if (this.entite?.EntiteType?.RefSAGEConditionLivraison == true) { this.sAGEConditionLivraisonListFC.enable(); }
      else { this.sAGEConditionLivraisonListFC.disable(); }
      if (this.entite?.EntiteType?.RefSAGEPeriodicite == true) { this.sAGEPeriodiciteListFC.enable(); }
      else { this.sAGEPeriodiciteListFC.disable(); }
      if (this.entite?.EntiteType?.RefSAGEModeReglement == true) { this.sAGEModeReglementListFC.enable(); }
      else { this.sAGEModeReglementListFC.disable(); }
      if (this.entite?.EntiteType?.RefSAGECategorieVente == true) { this.sAGECategorieVenteListFC.enable(); }
      else { this.sAGECategorieVenteListFC.disable(); }
      if (this.entite?.EntiteType?.Horaires == true) { this.horairesFC.enable(); }
      else { this.horairesFC.disable(); }
      if (this.entite?.EntiteType?.RefRepriseType == true) { this.repriseTypeListFC.enable(); }
      else { this.repriseTypeListFC.disable(); }
      if (this.entite?.EntiteType?.RefRepreneur == true) { this.repreneurListFC.enable(); }
      else { this.repreneurListFC.disable(); }
      if (this.entite?.EntiteType?.RefEquipementier == true) { this.equipementierListFC.enable(); }
      else { this.equipementierListFC.disable(); }
      if (this.entite?.EntiteType?.RefFournisseurTO == true) { this.fournisseurTOListFC.enable(); }
      else { this.fournisseurTOListFC.disable(); }
      if (this.entite?.EntiteType?.Process == true) { this.processListFC.enable(); }
      else { this.processListFC.disable(); }
      //Rights
      if (this.applicationUserContext.connectedUtilisateur.HabilitationAnnuaire != HabilitationAnnuaire.Administrateur) {
        this.ecoOrganismeListFC.disable();
        this.codeValorisationFC.disable();
        this.assujettiTVAFC.disable();
        this.codeTVAFC.disable();
        this.sAGECompteTiersFC.disable();
        this.sAGEConditionLivraisonListFC.disable();
        this.sAGEModeReglementListFC.disable();
        this.sAGECategorieVenteListFC.disable();
        this.sAGECodeComptableFC.disable();
        this.sAGECategorieAchatListFC.disable();
        this.sAGEPeriodiciteListFC.disable();
        this.exportSAGEFC.disable();
        this.repartitionMensuelleFC.disable();
        this.surcoutCarburantFC.disable();
        this.processListFC.disable();
      }
      //Validators
      this.codeTVAFCRequired = false;
      this.codeTVAFC.clearValidators();
      if (this.entite.AssujettiTVA) {
        this.codeTVAFC.setValidators(Validators.required);
        this.codeTVAFCRequired = true;
      }
      this.codeTVAFC.updateValueAndValidity();
      this.form.updateValueAndValidity();
      //Dynamic labels
      if (this.entite.EntiteType?.RefEntiteType == 1) {
        this.centreDeTriLieLabel = this.applicationUserContext.getCulturedRessourceText(1167);
      }
      if (this.entite.EntiteType?.RefEntiteType == 3) {
        this.camionTypeLieLabel = this.applicationUserContext.getCulturedRessourceText(1170);
      }
      //Show/hide
      this.showAdresses = (this.showAll);
      this.showContactAdresses = (this.entite.ContactAdresses?.length > 0 || this.showAll);
      this.showContratIncitationQualites = (this.entite?.EntiteType?.RefEntiteType == 3
        && (this.entite.ContratIncitationQualites?.length > 0 || this.showAll));
      this.showContrats = ((this.entite?.EntiteType?.RefEntiteType == 1 || this.entite?.EntiteType?.RefEntiteType == 4)
        && (this.entite.Contrats?.length > 0 || this.showAll));
      this.showContratCollectivites = (this.entite?.EntiteType?.RefEntiteType == 1
        && (this.entite.ContratCollectivites?.length > 0 || this.showAll));
      this.showEntiteEntites1 = (this.entite?.EntiteType?.RefEntiteType == 3
        && (this.entite.EntiteEntites.some(item => item.RefEntiteRttType == 1 && (this.applicationUserContext.filterGlobalActif ? (item.Actif && item.ActifEntiteRtt) : true)) || this.showAll));
      this.showEntiteEntites2 = ((this.entite?.EntiteType?.RefEntiteType == 3 || this.entite?.EntiteType?.RefEntiteType == 4 || this.entite?.EntiteType?.RefEntiteType == 5)
        && (this.entite.EntiteEntites.some(item => item.RefEntiteRttType == 2 && (this.applicationUserContext.filterGlobalActif ? (item.Actif && item.ActifEntiteRtt) : true)) || this.showAll));
      this.showEntiteEntites3 = ((this.entite?.EntiteType?.RefEntiteType == 1 || this.entite?.EntiteType?.RefEntiteType == 2 || this.entite?.EntiteType?.RefEntiteType == 4)
        && (this.entite.EntiteEntites.some(item => item.RefEntiteRttType == 3 && (this.applicationUserContext.filterGlobalActif ? (item.Actif && item.ActifEntiteRtt) : true)) || this.showAll));
      this.showEntiteEntites4 = ((this.entite?.EntiteType?.RefEntiteType == 3 || this.entite?.EntiteType?.RefEntiteType == 2 || this.entite?.EntiteType?.RefEntiteType == 5)
        && (this.entite.EntiteEntites.some(item => item.RefEntiteRttType == 4 && (this.applicationUserContext.filterGlobalActif ? (item.Actif && item.ActifEntiteRtt) : true)) || this.showAll));
      this.showEntiteEntites5 = ((this.entite?.EntiteType?.RefEntiteType == 2 || this.entite?.EntiteType?.RefEntiteType == 4)
        && (this.entite.EntiteEntites.some(item => item.RefEntiteRttType == 5 && (this.applicationUserContext.filterGlobalActif ? (item.Actif && item.ActifEntiteRtt) : true)) || this.showAll));
      this.showEntiteCamionTypes = ((this.entite?.EntiteType?.RefEntiteType == 2 || this.entite?.EntiteType?.RefEntiteType == 3)
        && (this.entite.EntiteCamionTypes?.length > 0 || this.showAll));
      this.showActions = (this.entite.Actions?.length > 0 || this.showAll);
      this.showPrivateDocuments = (this.entite.DocumentEntites?.length > 0 || this.showAll);
      this.showPublicDocuments = (this.entite.DocumentEntites?.length > 0 || this.showAll);
      this.showEntiteProduits = ((this.entite?.EntiteType?.RefEntiteType == 4 || this.entite?.EntiteType?.RefEntiteType == 3)
        && (this.entite.EntiteProduits?.some(item => this.applicationUserContext.filterGlobalActif ? (item.Produit.Actif) : true) || this.showAll));
      this.showStatistics = (this.entite?.EntiteType?.RefEntiteType == 1 || this.entite?.EntiteType?.RefEntiteType == 2);
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let elementType = this.activatedRoute.snapshot.params["elementType"];
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    //Get reference data
    this.listService.getListEntiteType().subscribe(result => {
      this.entiteTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListEcoOrganisme().subscribe(result => {
      this.ecoOrganismeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListSAGECategorieAchat().subscribe(result => {
      this.sAGECategorieAchatList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListSAGEConditionLivraison().subscribe(result => {
      this.sAGEConditionLivraisonList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListSAGEPeriodicite().subscribe(result => {
      this.sAGEPeriodiciteList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListSAGEModeReglement().subscribe(result => {
      this.sAGEModeReglementList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListSAGECategorieVente().subscribe(result => {
      this.sAGECategorieVenteList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListRepriseType().subscribe(result => {
      this.repriseTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListRepreneur().subscribe(result => {
      this.repreneurList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListEquipementier().subscribe(result => {
      this.equipementierList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListFournisseurTO().subscribe(result => {
      this.fournisseurTOList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListUtilisateur(null, ModuleName.Qualite, HabilitationQualite.Utilisateur, null, false, false, false, false).subscribe(result => {
      this.entiteDRList = result.map(item => ({ DR: item } as dataModelsInterfaces.EntiteDR));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListProcess(null, null).subscribe(result => {
      this.entiteProcessList = result.map(item => ({ Process: item } as dataModelsInterfaces.EntiteProcess));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListStandard(null, null).subscribe(result => {
      this.entiteStandardList = result.map(item => ({ Standard: item } as dataModelsInterfaces.EntiteStandard));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Update formif new
    this.updateForm();
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getEntite(elementType == SearchElementType.ContactAdresse ? null : id
          , elementType == SearchElementType.ContactAdresse ? id : null
          , elementType == "Action" ? id : null
        )
          .subscribe(result => {
            //Get data
            this.entite = result;
            this.dataModelService.setMomentFromTextEntite(this.entite);
            //Init arrays
            if (this.entite.Adresses == null) { this.entite.Adresses = []; }
            if (this.entite.ContactAdresses == null) { this.entite.ContactAdresses = []; }
            if (this.entite.EntiteEntites == null) { this.entite.EntiteEntites = []; }
            //Sort
            this.entite.Actions.sort(function (a, b) { return cmpDesc(a.DAction, b.DAction); });
            this.entite.ContactAdresses.sort(function (a, b) { return cmp(a.Contact?.Nom, b.Contact?.Nom); });
            this.entite.ContratIncitationQualites.sort(function (a, b) { return cmpDesc(a.DDebut, b.DDebut); });
            this.entite.Contrats.sort(function (a, b) { return cmpDesc(a.DDebut, b.DDebut); });
            this.entite.ContratCollectivites.sort(function (a, b) { return cmpDesc(a.DDebut, b.DDebut); });
            this.entite.EntiteEntites.sort(function (a, b) { return cmp(a.LibelleEntiteRtt, b.LibelleEntiteRtt); });
            this.entite.EntiteCamionTypes.sort(function (a, b) { return cmp(a.CamionType.Libelle, b.CamionType.Libelle); });
            this.entite.EntiteProduits.sort(function (a, b) { return cmp(a.Produit.Libelle, b.Produit.Libelle); });
            //Get DocumentEntites
            this.dataModelService.getDocumentEntites(this.entite.RefEntite).subscribe(result => {
              //Get data
              this.entite.DocumentEntites = result;
              //Sort
              this.entite.DocumentEntites.sort(function (a, b) { return cmpDesc(a.DocumentNoFile.DCreation, b.DocumentNoFile.DCreation); });
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            //Update form
            this.updateForm();
            //Calculate statistics
            switch (this.entite.EntiteType?.RefEntiteType) {
              case 1:
                this.getStatistic(MenuName.AnnuaireMenuEvolutionTonnage);
                break;
              case 2:
                this.getStatistic(MenuName.AnnuaireMenuNbAffretementTransporteur);
                break;
            }
            //Show ContactAdresse if applicable
            let elementType = this.activatedRoute.snapshot.params["elementType"];
            let i: number;
            switch (elementType) {
              case SearchElementType.ContactAdresse:
                i = this.entite.ContactAdresses.findIndex(e => e.RefContactAdresse == id);
                this.showDetails("AnnuaireContactAdresse", id, null, "")
                break;
              case "Action":
                i = this.entite.Actions.findIndex(e => e.RefAction == id);
                this.showDetails("AnnuaireAction", id, null, "")
                break;
            }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
      else {
        this.showAll = true;
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
    this.actifFC.setValue(this.entite.Actif);
    this.sousContratFC.setValue(this.entite.SousContrat);
    this.entiteTypeListFC.setValue(this.entite.EntiteType?.RefEntiteType);
    this.visibiliteAffretementCommunFC.setValue(this.entite.VisibiliteAffretementCommun);
    this.surcoutCarburantFC.setValue(this.entite.SurcoutCarburant);
    this.libelleFC.setValue(this.entite.Libelle);
    this.codeEEFC.setValue(this.entite.CodeEE);
    this.actionnaireProprietaireFC.setValue(this.entite.ActionnaireProprietaire);
    this.exploitantFC.setValue(this.entite.Exploitant);
    this.capaciteFC.setValue(this.entite.Capacite);
    this.populationContratNFC.setValue(this.entite.PopulationContratN);
    this.codeValorisationFC.setValue(this.entite.CodeValorisation);
    this.ecoOrganismeListFC.setValue(this.entite.EcoOrganisme?.RefEcoOrganisme);
    this.repartitionMensuelleFC.setValue(this.entite.RepartitionMensuelle);
    this.assujettiTVAFC.setValue(this.entite.AssujettiTVA);
    this.codeTVAFC.setValue(this.entite.CodeTVA);
    this.sAGECodeComptableFC.setValue(this.entite.SAGECodeComptable);
    this.sAGECompteTiersFC.setValue(this.entite.SAGECompteTiers);
    this.sAGECategorieAchatListFC.setValue(this.entite.SAGECategorieAchat?.RefSAGECategorieAchat);
    this.sAGEConditionLivraisonListFC.setValue(this.entite.SAGEConditionLivraison?.RefSAGEConditionLivraison);
    this.sAGEPeriodiciteListFC.setValue(this.entite.SAGEPeriodicite?.RefSAGEPeriodicite);
    this.sAGEModeReglementListFC.setValue(this.entite.SAGEModeReglement?.RefSAGEModeReglement);
    this.sAGECategorieVenteListFC.setValue(this.entite.SAGECategorieVente?.RefSAGECategorieVente);
    this.exportSAGEFC.setValue(this.entite.ExportSAGE);
    this.autoControleFC.setValue(this.entite.AutoControle);
    this.reconductionIncitationQualiteFC.setValue(this.entite.ReconductionIncitationQualite);
    this.cmtFC.setValue(this.entite.Cmt);
    this.horairesFC.setValue(this.entite.Horaires);
    this.repriseTypeListFC.setValue(this.entite.RepriseType?.RefRepriseType);
    this.repreneurListFC.setValue(this.entite.Repreneur?.RefRepreneur);
    this.dimensionBalleFC.setValue(this.entite.DimensionBalle);
    this.equipementierListFC.setValue(this.entite.Equipementier?.RefEquipementier);
    this.fournisseurTOListFC.setValue(this.entite.FournisseurTO?.RefFournisseurTO);
    this.dRListFC.setValue(this.entite.EntiteDRs);
    this.processListFC.setValue(this.entite.EntiteProcesss);
    this.standardListFC.setValue(this.entite.EntiteStandards);
    this.idNationalFC.setValue(this.entite.IdNational);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.entite.Actif = this.actifFC.value ? true : false;
    this.entite.SousContrat = this.sousContratFC.value ? true : false;
    this.entite.EntiteType = this.entiteTypeList.find(x => x.RefEntiteType === this.entiteTypeListFC.value);
    this.entite.VisibiliteAffretementCommun = this.visibiliteAffretementCommunFC.value;
    this.entite.SurcoutCarburant = this.surcoutCarburantFC.value;
    this.entite.Libelle = this.libelleFC.value;
    this.entite.CodeEE = this.codeEEFC.value;
    this.entite.ActionnaireProprietaire = this.actionnaireProprietaireFC.value;
    this.entite.Exploitant = this.exploitantFC.value;
    this.entite.Capacite = this.capaciteFC.value;
    this.entite.PopulationContratN = this.populationContratNFC.value;
    this.entite.CodeValorisation = this.codeValorisationFC.value;
    this.entite.EcoOrganisme = this.ecoOrganismeList.find(x => x.RefEcoOrganisme === this.ecoOrganismeListFC.value);
    this.entite.RepartitionMensuelle = this.repartitionMensuelleFC.value;
    this.entite.AssujettiTVA = this.assujettiTVAFC.value;
    this.entite.CodeTVA = this.codeTVAFC.value;
    this.entite.SAGECodeComptable = this.sAGECodeComptableFC.value;
    this.entite.SAGECompteTiers = this.sAGECompteTiersFC.value;
    this.entite.SAGECategorieAchat = this.sAGECategorieAchatList.find(x => x.RefSAGECategorieAchat === this.sAGECategorieAchatListFC.value);
    this.entite.SAGEConditionLivraison = this.sAGEConditionLivraisonList.find(x => x.RefSAGEConditionLivraison === this.sAGEConditionLivraisonListFC.value);
    this.entite.SAGEPeriodicite = this.sAGEPeriodiciteList.find(x => x.RefSAGEPeriodicite === this.sAGEPeriodiciteListFC.value);
    this.entite.SAGEModeReglement = this.sAGEModeReglementList.find(x => x.RefSAGEModeReglement === this.sAGEModeReglementListFC.value);
    this.entite.SAGECategorieVente = this.sAGECategorieVenteList.find(x => x.RefSAGECategorieVente === this.sAGECategorieVenteListFC.value);
    this.entite.ExportSAGE = this.exportSAGEFC.value;
    this.entite.AutoControle = this.autoControleFC.value;
    this.entite.ReconductionIncitationQualite = this.reconductionIncitationQualiteFC.value;
    this.entite.Cmt = this.cmtFC.value;
    this.entite.Horaires = this.horairesFC.value;
    this.entite.RepriseType = this.repriseTypeList.find(x => x.RefRepriseType === this.repriseTypeListFC.value);
    this.entite.Repreneur = this.repreneurList.find(x => x.RefRepreneur === this.repreneurListFC.value);
    this.entite.DimensionBalle = this.dimensionBalleFC.value;
    this.entite.Equipementier = this.equipementierList.find(x => x.RefEquipementier === this.equipementierListFC.value);
    this.entite.FournisseurTO = this.fournisseurTOList.find(x => x.RefFournisseurTO === this.fournisseurTOListFC.value);
    this.entite.EntiteDRs = this.dRListFC.value;
    this.entite.EntiteProcesss = this.processListFC.value;
    this.entite.EntiteStandards = this.standardListFC.value;
    this.entite.IdNational = this.idNationalFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    this.dataModelService.setTextFromMomentEntite(this.entite);
    //Attach Documents
    //this.entite.DocumentEntites = this.entite.DocumentEntites;
    //Update
    this.dataModelService.postEntite(this.entite)
      .subscribe(result => {
        //Redirect to grid and inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Select EntiteType 
  onEntiteTypeSelected() {
    this.saveData();
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //AssujettiTVA
  onAssujettiTVAChange() {
    this.saveData();
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Delete a Action 
  onDeleteAction(refAction: number) {
    if (refAction !== null) {
      let i: number = this.entite.Actions.map(e => e.RefAction).indexOf(refAction);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1197), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Save data
            this.saveData();
            //Remove Action
            this.entite.Actions.splice(i, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a ContratIncitationQualite 
  onDeleteContratIncitationQualite(refContratIncitationQualite: number) {
    if (refContratIncitationQualite !== null) {
      let i: number = this.entite.ContratIncitationQualites.map(e => e.RefContratIncitationQualite).indexOf(refContratIncitationQualite);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1334), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Save data
            this.saveData();
            //Remove ContratIncitationQualite
            this.entite.ContratIncitationQualites.splice(i, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a Contrat 
  onDeleteContrat(refContrat: number) {
    if (refContrat !== null) {
      let i: number = this.entite.Contrats.map(e => e.RefContrat).indexOf(refContrat);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1179), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Save data
            this.saveData();
            //Remove corresponding ContratEntite
            let j: number = this.entite.Contrats[i].ContratEntites.map(e => e.RefEntite).indexOf(this.entite.RefEntite);
            this.entite.Contrats[i].ContratEntites.splice(j, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a ContratCollectivite 
  onDeleteContratCollectivite(refContratCollectivite: number) {
    if (refContratCollectivite !== null) {
      let i: number = this.entite.ContratCollectivites.map(e => e.RefContratCollectivite).indexOf(refContratCollectivite);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1179), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Save data
            this.saveData();
            //Remove ContratCollectivite
            this.entite.ContratCollectivites.splice(i, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a ContactAdress 
  onDeleteContactAdresse(refContactAdresse: number) {
    if (refContactAdresse !== null) {
      let i: number = this.entite.ContactAdresses.map(e => e.RefContactAdresse).indexOf(refContactAdresse);
      //Process
      this.dataModelService.isDeletableContactAdresse(this.entite.ContactAdresses[i].RefContactAdresse, true)
        .subscribe(result => {
          let cAWithAction: boolean = false;
          this.entite.Actions.forEach(e => {
            if (e.ContactAdresse?.RefContactAdresse == refContactAdresse
            ) { cAWithAction = true; }
          });
          if (result && !cAWithAction
            && (!this.entite.ContactAdresses[i].ContactAdresseContactAdresseProcesss
              || this.entite.ContactAdresses[i].ContactAdresseContactAdresseProcesss?.length == 0)
          ) {
            showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1148), this.applicationUserContext)
              .subscribe(result => {
                if (result === "yes") {
                  //Save data
                  this.saveData();
                  //Remove ContactAdresse
                  this.entite.ContactAdresses.splice(i, 1);
                  //Update form
                  this.updateForm();
                }
              });
          }
          else {
            showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(666), this.applicationUserContext.getCulturedRessourceText(393), this.applicationUserContext);
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a Adresse
  onDeleteAdresse(refAdresse: number) {
    if (refAdresse !== null) {
      //Process
      this.dataModelService.isDeletableAdresse(refAdresse, true)
        .subscribe(result => {
          let cAWithProcess: boolean = false;
          this.entite.ContactAdresses.forEach(e => {
            if (e.RefAdresse == refAdresse
              && (!e.ContactAdresseContactAdresseProcesss
                || e.ContactAdresseContactAdresseProcesss?.length > 0)
            ) { cAWithProcess = true; }
          });
          if (result && !cAWithProcess) {
            showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1147), this.applicationUserContext)
              .subscribe(result => {
                if (result === "yes") {
                  //Save data
                  this.saveData();
                  //Detach related ContactAdresse
                  this.entite.ContactAdresses.forEach(e => {
                    if (e.RefAdresse == this.entite.Adresses.find(x => x.RefAdresse === refAdresse)?.RefAdresse) { e.RefAdresse = null; }
                  });
                  //Remove adresse
                  let i: number = this.entite.Adresses.map(e => e.RefAdresse).indexOf(refAdresse);
                  this.entite.Adresses.splice(i, 1);
                  //Update form
                  this.updateForm();
                }
              });
          }
          else {
            showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(666), this.applicationUserContext.getCulturedRessourceText(393), this.applicationUserContext);
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a EntiteEntite 
  onDeleteEntiteEntite(refEntiteEntite: number) {
    if (refEntiteEntite !== null) {
      let i: number = this.entite.EntiteEntites.map(e => e.RefEntiteEntite).indexOf(refEntiteEntite);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1162), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Save data
            this.saveData();
            //Remove EntiteEntite
            this.entite.EntiteEntites.splice(i, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a EntiteCamionType
  onDeleteEntiteCamionType(refEntiteCamionType: number) {
    if (refEntiteCamionType !== null) {
      let i: number = this.entite.EntiteCamionTypes.map(e => e.RefEntiteCamionType).indexOf(refEntiteCamionType);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1171), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Save data
            this.saveData();
            //Remove EntiteCamionType
            this.entite.EntiteCamionTypes.splice(i, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a EntiteProduit
  onDeleteEntiteProduit(refEntiteProduit: number) {
    if (refEntiteProduit !== null) {
      let i: number = this.entite.EntiteProduits.map(e => e.RefEntiteProduit).indexOf(refEntiteProduit);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1172), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Save data
            this.saveData();
            //Remove EntiteProduit
            this.entite.EntiteProduits.splice(i, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(1126) },
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
    this.dataModelService.deleteEntite(this.entite)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1125), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Get formatted DAction for Action
  getActionDAction(a: dataModelsInterfaces.Action): string {
    let s: string = moment(a.DAction).format("L");
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Get ActionTypes for Action
  getActionActionTypes(a: dataModelsInterfaces.Action): string {
    let s: string = "";
    if (a.ActionActionTypes) {
      s = Array.prototype.map.call(a.ActionActionTypes
        , function (item: dataModelsInterfaces.ActionActionType) { return item?.ActionType.Libelle }).join(", ")
    }
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Get Contact name for Action
  getActionContact(a: dataModelsInterfaces.Action): string {
    let s: string = "";
    if (a.ContactAdresse?.Contact) {
      if (a.ContactAdresse.Contact.Prenom) { s = a.ContactAdresse.Contact.Prenom + " "; }
      if (a.ContactAdresse.Contact.Prenom && a.ContactAdresse.Contact.Nom) { s += " "; }
      if (a.ContactAdresse.Contact.Nom) { s += a.ContactAdresse.Contact.Nom; }
    }
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Get ContactAdresseProcesss for ContactAdresse
  getContactAdresseProcesss(cA: dataModelsInterfaces.ContactAdresse): string {
    let s: string = "";
    if (cA.ContactAdresseContactAdresseProcesss) {
      s = Array.prototype.map.call(cA.ContactAdresseContactAdresseProcesss
        , function (item: dataModelsInterfaces.ContactAdresseContactAdresseProcess) { return item?.ContactAdresseProcess?.Libelle }).join(", ")
    }
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Get AdresseType for ContactAdresse
  getAdresseType(cA: dataModelsInterfaces.ContactAdresse): string {
    let s: string = "";
    s = this.entite.Adresses.find(x => x.RefAdresse === cA.RefAdresse)?.AdresseType?.Libelle;
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Show elements details
  showDetails(type: string, ref: number, refEntiteType: number, mode: string) {
    let data: any;
    let title: string = "";
    let dialogWidth: number = 350;
    let dataItems: any;
    let disableClose: boolean = true;
    //Init
    this.saveData();
    switch (type) {
      case "AnnuaireAction":
        //Set width
        dialogWidth = 900;
        //Calculate id
        let minRefAction: number = -1;
        this.entite.Actions?.forEach(item => {
          if (item.RefAction <= minRefAction) minRefAction = item.RefAction - 1;
        });
        //Get Action
        let a = {
          RefAction: minRefAction
          , RefEntite: this.entite.RefEntite
          , ActionFichierNoFiles: []
        } as dataModelsInterfaces.Action;
        if (ref != null) { a = this.entite.Actions.find(x => x.RefAction === ref) }
        data = {
          title: this.applicationUserContext.getCulturedRessourceText(1183)
          , type: type, ref: ref
          , entite: this.entite, action: a
        };
        break;
      case "AnnuaireAdresse":
        title = this.applicationUserContext.getCulturedRessourceText(476);
        //Set width
        dialogWidth = 900;
        //Calculate id
        let minRefAdresse: number = -1;
        this.entite.Adresses?.forEach(item => {
          if (item.RefAdresse <= minRefAdresse) minRefAdresse = item.RefAdresse - 1;
        });
        //Get adresse
        let adr = {
          RefAdresse: minRefAdresse
          , RefEntite: this.entite.RefEntite
          , Actif: true
          , CreationText: this.applicationUserContext.getCulturedRessourceText(9)
          , ModificationText: ""
        } as dataModelsInterfaces.Adresse;
        if (ref != null) { adr = this.entite.Adresses.find(x => x.RefAdresse === ref); }
        dataItems = <dataModelsInterfaces.Adresse[]>this.entite.Adresses.filter(f => !f.Actif);
        if (dataItems.length > 0 && mode == "" && ref == null) {
          type = "AnnuaireAdresseInactive";
          title = this.applicationUserContext.getCulturedRessourceText(1160);
          disableClose = false;
        }
        data = {
          title: title, type: type, ref: ref
          , entite: this.entite, adresse: adr, dataItems: dataItems
        };
        break;
      case "AnnuaireContactAdresse":
        title = this.applicationUserContext.getCulturedRessourceText(487);
        //Set width
        dialogWidth = 900;
        //Calculate id
        let minRefContactAdresse: number = -1;
        this.entite.ContactAdresses?.forEach(item => {
          if (item.RefContactAdresse <= minRefContactAdresse) minRefContactAdresse = item.RefContactAdresse - 1;
        });
        //Get contactAdresse
        let cA = {
          RefContactAdresse: minRefContactAdresse
          , RefEntite: this.entite.RefEntite
          , Actif: true
          , Contact: { RefContact: 0, } as dataModelsInterfaces.Contact
          , ContactAdresseContactAdresseProcesss: []
          , ContactAdresseServiceFonctions: []
          , CreationText: this.applicationUserContext.getCulturedRessourceText(9)
          , ModificationText: ""
        } as dataModelsInterfaces.ContactAdresse;
        if (ref != null) { cA = this.entite.ContactAdresses.find(x => x.RefContactAdresse === ref) }
        dataItems = this.entite.ContactAdresses.filter(f => !f.Actif) as dataModelsInterfaces.ContactAdresse[];
        if (dataItems.length > 0 && mode == "" && ref == null) {
          type = "AnnuaireContactAdresseInactive";
          title = this.applicationUserContext.getCulturedRessourceText(1192);
          disableClose = false;
        }
        data = {
          title: title, type: type, ref: ref
          , entite: this.entite, contactAdresse: cA
          , adresseList: this.entite.Adresses.filter(f => (this.applicationUserContext.filterGlobalActif ? f.Actif : true)), dataItems: dataItems
        };
        break;
      case "AnnuaireEntiteCamionType":
        //Title
        if (this.entite.EntiteType.RefEntiteType == 3) {
          title = this.camionTypeLieLabel + " " + this.applicationUserContext.getCulturedRessourceText(1163);
        }
        else {
          title = this.camionTypeLieLabel;
        }
        //Calculate id
        let minRefEntiteCamionType: number = -1;
        this.entite.EntiteCamionTypes?.forEach(item => {
          if (item.RefEntiteCamionType <= minRefEntiteCamionType) minRefEntiteCamionType = item.RefEntiteCamionType - 1;
        });
        //Get EntiteCamionType
        let eCT = {
          RefEntiteCamionType: minRefEntiteCamionType
          , RefEntite: this.entite.RefEntite
        } as dataModelsInterfaces.EntiteCamionType;
        if (ref != null) { eCT = this.entite.EntiteCamionTypes.find(x => x.RefEntiteCamionType === ref) }
        data = {
          title: title, type: type, ref: ref
          , entite: this.entite, entiteCamionType: eCT, refEntiteType: refEntiteType
        };
        break;
      case "ContratIncitationQualite":
        //Calculate id
        let minRefContratIncitationQualite: number = -1;
        this.entite.ContratIncitationQualites?.forEach(item => {
          if (item.RefContratIncitationQualite <= minRefContratIncitationQualite) minRefContratIncitationQualite = item.RefContratIncitationQualite - 1;
        });
        //Get contratIncitationQualite
        let cIQ = {
          RefContratIncitationQualite: minRefContratIncitationQualite
          , RefEntite: this.entite.RefEntite
        } as dataModelsInterfaces.ContratIncitationQualite;
        if (ref != null) { cIQ = this.entite.ContratIncitationQualites.find(x => x.RefContratIncitationQualite === ref) }
        data = {
          title: this.applicationUserContext.getCulturedRessourceText(1180), type: type, ref: ref
          , entite: this.entite, contratIncitationQualite: cIQ
        };
        break;
      case "Contrat":
        //Set width
        dialogWidth = 900;
        //Calculate id
        let minRefContrat: number = -1;
        this.entite.Contrats?.forEach(item => {
          if (item.RefContrat <= minRefContrat) minRefContrat = item.RefContrat - 1;
        });
        //Get contrat
        let c = {
          RefContrat: minRefContrat
          , Avenant: false
        } as dataModelsInterfaces.Contrat;
        if (ref != null) {
          c = this.entite.Contrats.find(x => x.RefContrat === ref);
        }
        else {
          //Add current Entite to ContratEntite
          let cE = {
            RefEntite: this.entite.RefEntite
            , RefContrat: c.RefContrat
            , Entite: { RefEntite: this.entite.RefEntite, Libelle:this.entite.Libelle, Actif: this.entite.Actif} as dataModelsInterfaces.EntiteList
          } as dataModelsInterfaces.ContratEntite;
          if (c.ContratEntites == null) { c.ContratEntites = []; }
          c.ContratEntites.push(cE);  
        }
        data = {
          title: this.applicationUserContext.getCulturedRessourceText(1176), type: type, ref: ref
          , entite: this.entite, contrat: c
        };
        break;
      case "ContratCollectivite":
        //Calculate id
        let minRefContratCollectivite: number = -1;
        this.entite.ContratCollectivites?.forEach(item => {
          if (item.RefContratCollectivite <= minRefContratCollectivite) minRefContratCollectivite = item.RefContratCollectivite - 1;
        });
        //Get contratCollectivite
        let cC = {
          RefContratCollectivite: minRefContratCollectivite
          , RefEntite: this.entite.RefEntite
          , Avenant: false
        } as dataModelsInterfaces.ContratCollectivite;
        if (ref != null) { cC = this.entite.ContratCollectivites.find(x => x.RefContratCollectivite === ref) }
        data = {
          title: this.applicationUserContext.getCulturedRessourceText(1176), type: type, ref: ref
          , entite: this.entite, contratCollectivite: cC
        };
        break;
      case "AnnuaireEntiteProduit":
        //Calculate id
        let minRefEntiteProduit: number = -1;
        this.entite.EntiteProduits?.forEach(item => {
          if (item.RefEntiteProduit <= minRefEntiteProduit) minRefEntiteProduit = item.RefEntiteProduit - 1;
        });
        //Get EntiteProduit
        let eP = {
          RefEntiteProduit: minRefEntiteProduit
          , RefEntite: this.entite.RefEntite
        } as dataModelsInterfaces.EntiteProduit;
        if (ref != null) { eP = this.entite.EntiteProduits.find(x => x.RefEntiteProduit === ref) }
        data = {
          title: this.applicationUserContext.getCulturedRessourceText(317) + " " + this.applicationUserContext.getCulturedRessourceText(1163)
          , type: type, ref: ref
          , entite: this.entite, entiteProduit: eP, refEntiteType: refEntiteType
        };
        break;
      case "AnnuaireEntiteEntite":
        //Title
        switch (refEntiteType) {
          case 1:
            title = this.applicationUserContext.getCulturedRessourceText(1159);
            break;
          case 2:
            title = this.applicationUserContext.getCulturedRessourceText(1165);
            break;
          case 3:
            title = this.centreDeTriLieLabel;
            break;
          case 4:
            title = this.applicationUserContext.getCulturedRessourceText(1164);
            break;
          case 5:
            title = this.applicationUserContext.getCulturedRessourceText(1166);
            break;
        }
        //Calculate id
        let minRefEntiteEntite: number = -1;
        this.entite.EntiteEntites?.forEach(item => {
          if (item.RefEntiteEntite <= minRefEntiteEntite) minRefEntiteEntite = item.RefEntiteEntite - 1;
        });
        //Get EntiteEntite
        let eE = {
          RefEntiteEntite: minRefEntiteEntite
          , RefEntite: this.entite.RefEntite
          , Actif: true
        } as dataModelsInterfaces.EntiteEntite
        if (ref != null) { eE = this.entite.EntiteEntites.find(x => x.RefEntiteEntite === ref) }
        data = {
          title: title + " " + this.applicationUserContext.getCulturedRessourceText(1163), type: type, ref: ref
          , entite: this.entite, entiteEntite: eE, refEntiteType: refEntiteType
        };
        break;
    }
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: dialogWidth.toString() + "px",
      maxHeight: "calc(100vh - 80px)",
      data,
      autoFocus: false,
      restoreFocus: false,
      disableClose: disableClose,
      closeOnNavigation: false
    });
    dialogRef.afterClosed().subscribe(result => {
      //If data in the dialog have been saved
      if (result) {
        let exists: boolean = false;
        switch (result.type) {
          case "AnnuaireAction":
            //Add or update Action
            let a: dataModelsInterfaces.Action = result.ref as dataModelsInterfaces.Action;
            exists = this.entite.Actions.some(item => item.RefAction == a.RefAction);
            if (!exists) {
              this.entite.Actions.push(a);
              this.entite.Actions.sort(function (a, b) { return cmpDesc(a.DAction, b.DAction); });
            }
            else {
              let ind: number = this.entite.Actions.map(e => e.RefAction).indexOf(a.RefAction);
              this.entite.Actions[ind] = a;
            }
            break;
          case "AnnuaireAdresse":
            //Add or update adresse
            let adr: dataModelsInterfaces.Adresse = result.ref as dataModelsInterfaces.Adresse;
            adr.TexteAdresseList = getAdresseListText(adr);
            exists = this.entite.Adresses.some(item => item.RefAdresse == adr.RefAdresse);
            if (!exists) {
              this.entite.Adresses.push(adr);
            }
            else {
              let ind: number = this.entite.Adresses.map(e => e.RefAdresse).indexOf(adr.RefAdresse);
              this.entite.Adresses[ind] = adr;
            }
            break;
          case "AnnuaireAdresseInactive":
            //Add or update adresse
            this.showDetails("AnnuaireAdresse", result.ref ? result.ref : null, null, "force");
            break;
          case "AnnuaireContactAdresse":
            //Add or update ContactAdresse
            let contactAdresse: dataModelsInterfaces.ContactAdresse = result.ref as dataModelsInterfaces.ContactAdresse;
            contactAdresse.ListText = getContactAdresseListText(contactAdresse);
            exists = this.entite.ContactAdresses.some(item => item.RefContactAdresse == contactAdresse.RefContactAdresse);
            if (!exists) {
              this.entite.ContactAdresses.push(contactAdresse);
              this.entite.ContactAdresses.sort(function (a, b) { return cmp(a.Contact?.Nom, b.Contact?.Nom); });
            }
            else {
              let ind: number = this.entite.ContactAdresses.map(e => e.RefContactAdresse).indexOf(contactAdresse.RefContactAdresse);
              this.entite.ContactAdresses[ind] = contactAdresse;
            }
            break;
          case "AnnuaireContactAdresseInactive":
            //Add or update adresse
            this.showDetails("AnnuaireContactAdresse", result.ref ? result.ref : null, null, "force");
            break;
          case "AnnuaireEntiteCamionType":
            //Add or update EntiteCamionType
            let eCT: dataModelsInterfaces.EntiteCamionType = result.ref as dataModelsInterfaces.EntiteCamionType;
            exists = this.entite.EntiteCamionTypes?.some(item => item.RefEntiteCamionType == eCT.RefEntiteCamionType);
            if (!exists) {
              this.entite.EntiteCamionTypes.push(eCT);
              this.entite.EntiteCamionTypes.sort(function (a, b) { return cmp(a.CamionType.Libelle, b.CamionType.Libelle); });
            }
            else {
              let ind: number = this.entite.EntiteCamionTypes.map(e => e.RefEntiteCamionType).indexOf(eCT.RefEntiteCamionType);
              this.entite.EntiteCamionTypes[ind] = eCT;
            }
            break;
          case "ContratIncitationQualite":
            //Add or update ContratIncitationQualite
            let contratIncitationQualite: dataModelsInterfaces.ContratIncitationQualite = result.ref as dataModelsInterfaces.ContratIncitationQualite;
            exists = this.entite.ContratIncitationQualites.some(item => item.RefContratIncitationQualite == contratIncitationQualite.RefContratIncitationQualite);
            if (!exists) {
              this.entite.ContratIncitationQualites.push(contratIncitationQualite);
              this.entite.ContratIncitationQualites.sort(function (a, b) { return cmpDesc(a.DDebut, b.DDebut); });
            }
            else {
              let ind: number = this.entite.ContratIncitationQualites.map(e => e.RefContratIncitationQualite).indexOf(contratIncitationQualite.RefContratIncitationQualite);
              this.entite.ContratIncitationQualites[ind] = contratIncitationQualite;
            }
            break;
          case "Contrat":
            //Add or update Contrat
            let contrat: dataModelsInterfaces.Contrat = result.ref as dataModelsInterfaces.Contrat;
            exists = this.entite.Contrats.some(item => item.RefContrat == contrat.RefContrat);
            if (!exists) {
              this.entite.Contrats.push(contrat);
              this.entite.Contrats.sort(function (a, b) { return cmpDesc(a.DDebut, b.DDebut); });
            }
            else {
              let ind: number = this.entite.Contrats.map(e => e.RefContrat).indexOf(contrat.RefContrat);
              this.entite.Contrats[ind] = contrat;
            }
            break;
          case "ContratCollectivite":
            //Add or update ContratCollectivite
            let contratCollectivite: dataModelsInterfaces.ContratCollectivite = result.ref as dataModelsInterfaces.ContratCollectivite;
            exists = this.entite.ContratCollectivites.some(item => item.RefContratCollectivite == contratCollectivite.RefContratCollectivite);
            if (!exists) {
              this.entite.ContratCollectivites.push(contratCollectivite);
              this.entite.ContratCollectivites.sort(function (a, b) { return cmpDesc(a.DDebut, b.DDebut); });
            }
            else {
              let ind: number = this.entite.ContratCollectivites.map(e => e.RefContratCollectivite).indexOf(contratCollectivite.RefContratCollectivite);
              this.entite.ContratCollectivites[ind] = contratCollectivite;
            }
            break;
          case "AnnuaireEntiteProduit":
            //Add or update EntiteProduit
            let eP: dataModelsInterfaces.EntiteProduit = result.ref as dataModelsInterfaces.EntiteProduit;
            exists = this.entite.EntiteProduits?.some(item => item.RefEntiteProduit == eP.RefEntiteProduit);
            if (!exists) {
              this.entite.EntiteProduits.push(eP);
              this.entite.EntiteProduits.sort(function (a, b) { return cmp(a.Produit.Libelle, b.Produit.Libelle); });
            }
            else {
              let ind: number = this.entite.EntiteProduits.map(e => e.RefEntiteProduit).indexOf(eP.RefEntiteProduit);
              this.entite.EntiteProduits[ind] = eP;
            }
            break;
          case "AnnuaireEntiteEntite":
            //Add or update EntiteEntite
            let eE: dataModelsInterfaces.EntiteEntite = result.ref as dataModelsInterfaces.EntiteEntite;
            exists = this.entite.EntiteEntites?.some(item => item.RefEntiteEntite == eE.RefEntiteEntite);
            if (!exists) {
              this.entite.EntiteEntites.push(eE);
              this.entite.EntiteEntites.sort(function (a, b) { return cmp(a.LibelleEntiteRtt, b.LibelleEntiteRtt); });
            }
            else {
              let ind: number = this.entite.EntiteEntites.map(e => e.RefEntiteEntite).indexOf(eE.RefEntiteEntite);
              this.entite.EntiteEntites[ind] = eE;
            }
            break;
        }
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  getFormValidationErrors() {
    Object.keys(this.form.controls).forEach(key => {
      const controlErrors: ValidationErrors = this.form.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach(keyError => {
          console.log("Key control: " + key + ", keyError: " + keyError + ", err value: ", controlErrors[keyError]);
        });
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.entite);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for EntiteEntite creation
  getEntiteEntiteCreationTooltipText(r: number) {
    let eE: dataModelsInterfaces.EntiteEntite = this.entite.EntiteEntites.find(x => x.RefEntiteEntite === r)
    if (!!eE.DCreation) {
      if (!!eE.Cmt) {
        return `${this.applicationUserContext.getCulturedRessourceText(388) + " " + moment(eE.DCreation).format("L") + " " + this.applicationUserContext.getCulturedRessourceText(390) + " " + eE.LibelleUtilisateurCreation}
${eE.Cmt}`;
      } else {
        return `${this.applicationUserContext.getCulturedRessourceText(388) + " " + moment(eE.DCreation).format("L") + " " + this.applicationUserContext.getCulturedRessourceText(390) + " " + eE.LibelleUtilisateurCreation}`;
      }
    }
    else if (!!eE.Cmt) {
      return `${eE.Cmt}`;
    }
    else {
      return "";
    }
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for EntiteCamionType creation
  getEntiteCamionTypeCreationTooltipText(r: number) {
    let eE: dataModelsInterfaces.EntiteCamionType = this.entite.EntiteCamionTypes.find(x => x.RefEntiteCamionType === r)
    if (!!eE.DCreation) {
      if (!!eE.Cmt) {
        return `${this.applicationUserContext.getCulturedRessourceText(388) + " " + moment(eE.DCreation).format("L") + " " + this.applicationUserContext.getCulturedRessourceText(390) + " " + eE.UtilisateurCreation.Nom}
${eE.Cmt}`;
      } else {
        return `${this.applicationUserContext.getCulturedRessourceText(388) + " " + moment(eE.DCreation).format("L") + " " + this.applicationUserContext.getCulturedRessourceText(390) + " " + eE.UtilisateurCreation.Nom}`;
      }
    }
    else if (!!eE.Cmt) {
      return `${eE.Cmt}`;
    }
    else {
      return "";
    }
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for EntiteProduit creation
  getEntiteProduitCreationTooltipText(r: number) {
    let eE: dataModelsInterfaces.EntiteProduit = this.entite.EntiteProduits.find(x => x.RefEntiteProduit === r)
    if (!!eE.DCreation) {
      if (!!eE.Cmt) {
        return `${this.applicationUserContext.getCulturedRessourceText(388) + " " + moment(eE.DCreation).format("L") + " " + this.applicationUserContext.getCulturedRessourceText(390) + " " + eE.UtilisateurCreation.Nom}
${eE.Cmt}`;
      } else {
        return `${this.applicationUserContext.getCulturedRessourceText(388) + " " + moment(eE.DCreation).format("L") + " " + this.applicationUserContext.getCulturedRessourceText(390) + " " + eE.UtilisateurCreation.Nom}`;
      }
    }
    else if (!!eE.Cmt) {
      return `${eE.Cmt}`;
    }
    else {
      return "";
    }
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for Contrat creation
  getContratTooltipText(cC: dataModelsInterfaces.Contrat) {
    if (!!cC.Avenant && !!cC.Cmt) {
      return `${this.applicationUserContext.getCulturedRessourceText(1177)}
${cC.Cmt}`;
    }
    else if (!cC.Avenant && !!cC.Cmt) {
      return `${cC.Cmt}`;
    }
    else {
      return "";
    }
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for ContratCollectivite creation
  getcontratCollectiviteTooltipText(cC: dataModelsInterfaces.ContratCollectivite) {
    if (!!cC.Avenant && !!cC.Cmt) {
      return `${this.applicationUserContext.getCulturedRessourceText(1177)}
${cC.Cmt}`;
    }
    else if (!cC.Avenant && !!cC.Cmt) {
      return `${cC.Cmt}`;
    }
    else {
      return "";
    }
  }
  //-----------------------------------------------------------------------------------
  //Show all data
  onShowAll() {
    this.showAll = true;
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Multi select
  compareFnDR(item: dataModelsInterfaces.EntiteDR, selectedItem: dataModelsInterfaces.EntiteDR) {
    return item && selectedItem ? item.DR.RefUtilisateur === selectedItem.DR.RefUtilisateur : item === selectedItem;
  }
  public onDRListReset() {
    this.entite.EntiteDRs = [];
    this.dRListFC.setValue(this.entite.EntiteDRs);
  }
  compareFnProcess(item: dataModelsInterfaces.EntiteProcess, selectedItem: dataModelsInterfaces.EntiteProcess) {
    return item && selectedItem ? item.Process.RefProcess === selectedItem.Process.RefProcess : item === selectedItem;
  }
  public onProcessListReset() {
    this.entite.EntiteProcesss = [];
    this.processListFC.setValue(this.entite.EntiteProcesss);
  }
  compareFnStandard(item: dataModelsInterfaces.EntiteStandard, selectedItem: dataModelsInterfaces.EntiteStandard) {
    return item && selectedItem ? item.Standard.RefStandard === selectedItem.Standard.RefStandard : item === selectedItem;
  }
  public onStandardListReset() {
    this.entite.EntiteStandards = [];
    this.standardListFC.setValue(this.entite.EntiteStandards);
  }
  //-----------------------------------------------------------------------------------
  //Create Process label
  processsLabel(): string {
    let s = this.processListFC.value?.sort(function (a, b) { return cmp(a.Process.Libelle, b.Process.Libelle); })
      .map(item => item.Process.Libelle).join("\n");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create Standards label
  standardsLabel(): string {
    let s = this.standardListFC.value?.sort(function (a, b) { return cmp(a.Standard.Libelle, b.Standard.Libelle); })
      .map(item => item.Standard.Libelle).join("\n");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create ContratIncitationQualite label
  contratIncitationQualiteLabel(contratIncitationQualite: dataModelsInterfaces.ContratIncitationQualite): string {
    let s = moment(contratIncitationQualite.DDebut).format("L") + ' -> ' + moment(contratIncitationQualite.DFin).format("L");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Get tooltip text for Utilisateur Login/EMail
  getLoginTooltipText(cA: dataModelsInterfaces.ContactAdresse): string {
    let l: string = "";
    cA.Utilisateurs?.forEach(e => {
      if (e.EMail) { l += e.EMail; }
      else { l += e.Login; }
      l += "\n";
    });
    return l;
  }
  //-----------------------------------------------------------------------------------
  //Get ContactAdresse active account type
  contactAdresseAccountType(cA: dataModelsInterfaces.ContactAdresse): string {
    let s: string = "";
    let m: boolean = false; //Account active with Maitre
    let a: boolean = false; //Account active
    let p: boolean = false; //Account with profils active
    //Process
    cA.Utilisateurs?.forEach(e => {
      if (e.Actif && e.RefUtilisateurMaitre) { m = true; }
      if (e.Actif && e.HasProfils) { p = true; }
      if (e.Actif && !e.HasProfils) { a = true; }
    })
    if (a) { s = "Utilisateur"; }
    if (m) { s = "UtilisateurEsclave"; }
    if (p) { s = "UtilisateurMaitre"; }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  onUploadFile(visibiliteTotale: boolean) {
    //Save data
    this.saveData();
    //open pop-up
    this.startUploadFile(visibiliteTotale);
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  startUploadFile(visibiliteTotale: boolean) {
    //open pop-up
    const dialogRef = this.dialog.open(UploadComponent, {
      width: "50%", height: "50%",
      data: {
        title: this.applicationUserContext.getCulturedRessourceText(626), message: this.applicationUserContext.getCulturedRessourceText(627)
        , multiple: true, type: "documententite", fileType: (visibiliteTotale ? 1 : 0), ref: this.entite.RefEntite
      },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      this.lastUploadResut = result;
      if (!this.lastUploadResut) {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(341), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
      else if (this.lastUploadResut.error != "") {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(631), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
      else {
        //Reload data
        //Reload data, from database or from dialog depending on existence of Entite in db or not
        if (this.entite.RefEntite > 0) {
          this.dataModelService.getDocumentEntites(this.entite.RefEntite).subscribe(dEs => {
            //Get data
            this.entite.DocumentEntites = dEs;
            this.entite.DocumentEntites.sort(function (a, b) { return cmpDesc(a.DocumentNoFile.DCreation, b.DocumentNoFile.DCreation); });
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        else {
          this.entite.DocumentEntites = this.entite.DocumentEntites.concat(this.lastUploadResut.uploadedFiles);
          this.entite.DocumentEntites.sort(function (a, b) { return cmpDesc(a.DocumentNoFile.DCreation, b.DocumentNoFile.DCreation); });
        }
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(628), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Download a file
  getFile(fileType: string, refDocument: number) {
    this.downloadService.download(fileType, refDocument.toString(), "", this.entite.RefEntite, "")
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        console.log(error);
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  downloadFile(data: any, fileName: string) {
    const blob = new Blob([data]);
    {
      const url = window.URL.createObjectURL(blob);
      const a: HTMLAnchorElement = document.createElement("a") as HTMLAnchorElement;

      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a DocumentEntite
  onDeleteDocumentEntite(refDocumentEntite: number) {
    if (refDocumentEntite !== null) {
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1198), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            this.entite.DocumentEntites = this.entite.DocumentEntites.filter(e => e.RefDocumentEntite != refDocumentEntite);
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Chart labels
  labelContent = (e: any) => {
    let f: string = "";
    switch (this.statisticMenuName) {
      case MenuName.AnnuaireMenuEvolutionTonnage:
        f = e.value.toLocaleString(this.applicationUserContext.currentCultureName,
          { minimumFractionDigits: 3 }) + " t";
        break;
      case MenuName.AnnuaireMenuNbAffretementTransporteur:
        f = e.value.toString();
        break;
    }
    return f;
  }
  //-----------------------------------------------------------------------------------
  //Create statistic 
  getStatistic(menuName: string) {
    switch (menuName) {
      case MenuName.AnnuaireMenuEvolutionTonnage:
        //Init parameters
        this.statisticMenuName = menuName;
        this.statisticTitle = this.applicationUserContext.envMenus.find(x => x.name === menuName).culturedCaption;
        this.statisticAxisTitle = {
          text: this.applicationUserContext.envDataColumns.find(x => x.name === DataColumnName.PoidsTonne).culturedCaption
        };
        this.statisticField = DataColumnName.PoidsTonne;
        this.statisticCategoryField = DataColumnName.MoisAnnee;
        //Load data
        this.statistiqueService.getStatistique(ModuleName.ModuleCollectivite, MenuName.AnnuaireMenuEvolutionTonnage, 0, 10000
          , moment(new Date(new Date().getFullYear() - 5, 0, 1, 0, 0, 0, 0)).format("YYYY-MM-DD HH:mm:ss.SSS"), moment(new Date(new Date().getFullYear(), 11, 31, 0, 0, 0, 0)).format("YYYY-MM-DD HH:mm:ss.SSS"), false
          , "", "", this.entite.RefEntite.toString(), "", "", "", ""
          , "", "", "", "", ""
          , "", "", "", "", "", "", "", ""
          , false, "DansOuHorsCollecte", "", "", "", "", "", ""
          , "", "", "", "", "", true, "Month", "")
          .subscribe((resp: HttpResponse<any[]>) => {
            this.statisticResult = resp.body;
            //Set category axis
            this.statisticCategoryAxis = {
              categories: DataColumnName.MoisAnnee,
              min: (this.statisticResult?.length > 12 ? this.statisticResult?.length - 12 : 0),
              max: this.statisticResult?.length,
              labels: { rotation: "auto" }
            };
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        break;
      case MenuName.AnnuaireMenuNbAffretementTransporteur:
        this.statisticMenuName = menuName;
        this.statisticTitle = this.applicationUserContext.envMenus.find(x => x.name === menuName).culturedCaption;
        //Init parameters
        this.statisticAxisTitle = {
          text: this.applicationUserContext.envDataColumns.find(x => x.name === DataColumnName.NbAffretement).culturedCaption
        };
        this.statisticField = DataColumnName.NbAffretement;
        this.statisticCategoryField = DataColumnName.MoisAnnee;
        //Load data
        this.statistiqueService.getStatistique(ModuleName.Annuaire, MenuName.AnnuaireMenuNbAffretementTransporteur, 0, 10000
          , moment(new Date(new Date().getFullYear() - 5, 0, 1, 0, 0, 0, 0)).format("YYYY-MM-DD HH:mm:ss.SSS"), moment(new Date(new Date().getFullYear(), 11, 31, 0, 0, 0, 0)).format("YYYY-MM-DD HH:mm:ss.SSS"), false
          , "", "", "", "", "", ""
          , "", "", this.entite.RefEntite.toString()
          , "", "", ""
          , "", "", "", "", ""
          , "", "", ""
          , false, "", "", "", "", "", "", ""
          , "", "", "", "", "", true, "Month", "")
          .subscribe((resp: HttpResponse<any[]>) => {
            this.statisticResult = resp.body;
            //Set category axis
            this.statisticCategoryAxis = {
              categories: DataColumnName.MoisAnnee,
              min: (this.statisticResult?.length > 12 ? this.statisticResult?.length - 12 : 0),
              max: this.statisticResult?.length,
              labels: { rotation: "auto" }
            };
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        break;
    }
  }
  //------------------------------------------------------------------------------
  //Show statistic
  onShowStatistic() {
    this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === this.statisticMenuName);
    switch (this.statisticMenuName) {
      case MenuName.AnnuaireMenuEvolutionTonnage:
        //RAZ filters
        this.applicationUserContext.razFilters();
        //Set filter
        this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear() - 5, 0, 1, 0, 0, 0, 0));
        this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear(), 11, 31, 0, 0, 0, 0));
        this.applicationUserContext.filterCollectivites = [];
        this.applicationUserContext.filterCollectivites.push(this.entite);
        break;
      case MenuName.AnnuaireMenuNbAffretementTransporteur:
        //RAZ filters
        this.applicationUserContext.razFilters();
        //Set filter
        this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear() - 5, 0, 1, 0, 0, 0, 0));
        this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear(), 11, 31, 0, 0, 0, 0));
        this.applicationUserContext.filterTransporteurs = [];
        this.applicationUserContext.filterTransporteurs.push(this.entite);
        this.applicationUserContext.filterPayss = [];
        break;
    }
    //Navigate
    this.router.navigate(["statistique-grid"]);
    //Emit event for app compoment refresh (active menus and modules)
    this.eventEmitterService.onChangeMenu();
  }
  //-----------------------------------------------------------------------------------
  //Show Module Collectivité
  onModuleCollectivite() {
    this.saveData();
    this.dataModelService.setTextFromMomentEntite(this.entite);
    //Attach Documents
    this.entite.DocumentEntites = this.entite.DocumentEntites;
    //Update
    this.dataModelService.postEntite(this.entite)
      .subscribe(result => {
        //Redirect to grid and inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Set the corresponding Entite filter
        this.applicationUserContext.filterCollectivites = [];
        this.applicationUserContext.filterCollectivites.push({ RefEntite: this.entite.RefEntite } as dataModelsInterfaces.Entite)
        //Navigate
        this.router.navigate(["accueil-collectivite"]);
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.ModuleCollectivite);
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.ModuleCollectiviteMenuAccueil);
        this.eventEmitterService.onChangeMenu();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
}
