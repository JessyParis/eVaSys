import { Component, OnInit, AfterViewInit, ViewChild, Inject, OnDestroy } from "@angular/core";
import { Router, NavigationEnd } from "@angular/router";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { Observable, of, BehaviorSubject, ReplaySubject, Subject } from "rxjs";
import { catchError, finalize, tap, takeUntil } from "rxjs/operators";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { ApplicationUserContext } from "../../../globals/globals";
import { UntypedFormGroup, UntypedFormControl, Validators, FormGroupDirective, NgForm, FormControl } from "@angular/forms";
import { MenuName, ModuleName, HabilitationQualite, DataColumnName, HabilitationModuleCollectivite, HabilitationLogistique, DocumentType } from "../../../globals/enums";
import { StatistiqueService } from "../../../services/statistique.service";
import moment from "moment";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { showErrorToUser, createMatTableColumn, getCulturedMonthName, createContactAvailableColumns, chartSeriesColors, removeAccents, pointColor, shortenLongText, getAttachmentFilename } from "../../../globals/utils";
import { MatTableColumn } from '../../../interfaces/appInterfaces';
import { ErrorStateMatcher, ThemePalette } from '@angular/material/core';
import { MatDatepicker } from '@angular/material/datepicker';
import { Moment } from 'moment';
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import {CdkDragDrop, moveItemInArray, transferArrayItem} from '@angular/cdk/drag-drop';
import { EnvDataColumn } from "../../../classes/appClasses";
import { DataModelService } from "../../../services/data-model.service";
import { ProgressBarComponent } from "../../global/progress-bar/progress-bar.component";
import { ProgressBarMode } from "@angular/material/progress-bar";
import { DownloadService } from "../../../services/download.service";

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
    selector: "statistique-grid",
    templateUrl: "./statistique-grid.component.html",
    styleUrls: ["./statistique-grid.component.scss"],
    standalone: false
})
//-----------------------------------------------------------------------------------
//Component class
export class StatistiqueGridComponent implements AfterViewInit, OnInit, OnDestroy {
  matcher = new MyErrorStateMatcher();
  //-----------------------------------------------------------------------------------
  //Properties
  form: UntypedFormGroup;
  title: string;
  filterText: string = "";
  filterBegin: string = "";
  filterEnd: string = "";
  filterCentreDeTris: string = "";
  filterClients: string = "";
  filterCollectivites: string = "";
  filterDptDeparts: string = "";
  filterDptArrivees: string = "";
  filterDRs: string = "";
  filterEntites: string = "";
  filterEcoOrganismes: string = "";
  filterEntiteTypes: string = "";
  filterPayss: string = "";
  filterProcesss: string = "";
  filterProduits: string = "";
  filterTransporteurs: string = "";
  filterUtilisateurs: string = "";
  filterVilleDeparts: string = "";
  filterAdresseDestinations: string = "";
  filterVilleArrivees: string = "";
  filterMonths: string = "";
  filterQuarters: string = "";
  filterYears: string = "";
  filterNonConformiteEtapeTypes: string = "";
  filterNonConformiteNatures: string = "";
  filterProduitGroupeReportings: string = "";
  filterCamionTypes: string = "";
  filterActionTypes: string = "";
  filterAdresseTypes: string = "";
  filterContactAdresseProcesss: string = "";
  filterFonctions: string = "";
  filterServices: string = "";
  filterEmailType: string = "";
  filterContactSelectedColumns = "";
  entiteList: dataModelsInterfaces.EntiteList[];
  filteredEntiteList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  transporteurList: dataModelsInterfaces.EntiteList[];
  filteredTransporteurList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  clientList: dataModelsInterfaces.EntiteList[];
  filteredClientList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  centreDeTriList: dataModelsInterfaces.EntiteList[];
  filteredCentreDeTriList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  collectiviteList: dataModelsInterfaces.EntiteList[];
  filteredCollectiviteList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  produitList: dataModelsInterfaces.ProduitList[];
  filteredProduitList: ReplaySubject<dataModelsInterfaces.ProduitList[]> = new ReplaySubject<dataModelsInterfaces.ProduitList[]>(1);
  utilisateurList: dataModelsInterfaces.UtilisateurList[];
  filteredUtilisateurList: ReplaySubject<dataModelsInterfaces.UtilisateurList[]> = new ReplaySubject<dataModelsInterfaces.UtilisateurList[]>(1);
  dptDepartList: dataModelsInterfaces.Dpt[];
  dptArriveeList: dataModelsInterfaces.Dpt[];
  dRList: dataModelsInterfaces.UtilisateurList[];
  villeDepartList: any[];
  filteredVilleDepartList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
  adresseDestinationList: any[];
  filteredAdresseDestinationList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
  villeArriveeList: any[];
  filteredVilleArriveeList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
  ecoOrganismeList: dataModelsInterfaces.EcoOrganisme[];
  entiteTypeList: dataModelsInterfaces.EntiteType[];
  paysList: dataModelsInterfaces.Pays[];
  processList: dataModelsInterfaces.Process[];
  monthList: appInterfaces.Month[];
  quarterList: appInterfaces.Quarter[];
  yearList: appInterfaces.Year[];
  nonConformiteEtapeTypeList: dataModelsInterfaces.NonConformiteEtapeType[];
  nonConformiteNatureList: dataModelsInterfaces.NonConformiteNature[];
  produitGroupeReportingList: dataModelsInterfaces.ProduitGroupeReportingList[];
  camionTypeList: dataModelsInterfaces.CamionType[];
  actionTypeList: dataModelsInterfaces.ActionType[];
  adresseTypeList: dataModelsInterfaces.AdresseType[];
  contactAdresseProcessList: dataModelsInterfaces.ContactAdresseProcess[];
  fonctionList: dataModelsInterfaces.Fonction[];
  serviceList: dataModelsInterfaces.Service[];
  emailTypeList: any[] = [
    { Type: "IncitationQualite", Libelle: this.applicationUserContext.getCulturedRessourceText(700) }
    , { Type: "NoteCreditCollectivite", Libelle: this.applicationUserContext.getCulturedRessourceText(699) }]
  dataSource: StatistiqueDataSource;
  visibleFilterFilterText: boolean = false;
  visibleFilterDBegin: boolean = false;
  visibleFilterDEnd: boolean = false;
  visibleFilterCentreDeTriList: boolean = false;
  visibleFilterClientList: boolean = false;
  visibleFilterCollectiviteList: boolean = false;
  visibleFilterDptDepartList: boolean = false;
  visibleFilterDptArriveeList: boolean = false;
  visibleFilterDRList: boolean = false;
  visibleFilterEntiteList: boolean = false;
  visibleFilterEcoOrganismeList: boolean = false;
  visibleFilterEntiteTypeList: boolean = false;
  visibleFilterPaysList: boolean = false;
  visibleFilterProcessList: boolean = false;
  visibleFilterProduitList: boolean = false;
  visibleFilterTransporteurList: boolean = false;
  visibleFilterUtilisateurList: boolean = false;
  visibleFilterVilleDepartList: boolean = false;
  visibleFilterAdresseDestinationList: boolean = false;
  visibleFilterVilleArriveeList: boolean = false;
  visibleFilterMonthList: boolean = false;
  visibleFilterQuarterList: boolean = false;
  visibleFilterYearList: boolean = false;
  visibleFilterMonthEndList: boolean = false;
  visibleFilterYearEndList: boolean = false;
  visibleFilterNonConformiteEtapeTypeList: boolean = false;
  visibleFilterNonConformiteNatureList: boolean = false;
  visibleFilterProduitGroupeReportingList: boolean = false;
  visibleFilterCamionTypeList: boolean = false;
  visibleFilterActionTypeList: boolean = false;
  visibleFilterAdresseTypeList: boolean = false;
  visibleFilterContactAdresseProcessList: boolean = false;
  visibleFilterFonctionList: boolean = false;
  visibleFilterServiceList: boolean = false;
  visibleFilterMoins10Euros: boolean = false;
  visibleFilterCollecte: boolean = false;
  visibleFilterCollecteChip: boolean = false;
  visibleFilterContrat: boolean = false;
  visibleFilterEE: boolean = false;
  visibleFilterActif: boolean = false;
  visibleFilterEmailTypeList: boolean = false;
  visibleFilterFirstLogin: boolean = false;
  visibleFilterDayWeekMonth: boolean = false;
  visibleStatistiqueCollectivite: boolean = false;
  requiredCentreDeTriList: boolean = false;
  requiredMonthList: boolean = false;
  monthListMultiple: boolean = true;
  yearListMultiple: boolean = true;
  entiteTypeListMultiple: boolean = true;
  collectiviteListMultiple: boolean = true;
  pageSize: number = 25;
  pageSizeOptions: number[] = [10, 25, 50];
  @ViewChild(MatPaginator, { static: true }) pagin: MatPaginator;
  @ViewChild(ProgressBarComponent) progressBar: ProgressBarComponent;
  //Dynamic labels
  labelCollectiviteListFC: string = this.applicationUserContext.getCulturedRessourceText(594);
  placeHolderMonthListFC: string = this.applicationUserContext.getCulturedRessourceText(571);
  labelMonthListFC: string = "";
  labelQuarterListFC: string = "";
  placeHolderYearListFC: string = this.applicationUserContext.getCulturedRessourceText(569);
  labelYearListFC: string = "";
  placeholderDBeginFC: string = this.applicationUserContext.getCulturedRessourceText(212);
  placeholderDEndFC: string = this.applicationUserContext.getCulturedRessourceText(213);
  labelDBeginDEndFC: string = "";
  optionSousOuHorsContratFC: string = this.applicationUserContext.getCulturedRessourceText(888);
  optionSousContratFC: string = this.applicationUserContext.getCulturedRessourceText(886);
  optionHorsContratFC: string = this.applicationUserContext.getCulturedRessourceText(887);
  //Ergonomics
  datePickerMonthYear: boolean = false;
  showResultType: boolean = false;
  ResultType = new FormControl();
  //Fields for data extract
  contactAvailableColumns: EnvDataColumn[] = [];
  contactSelectedColumns: EnvDataColumn[] = [];
  // Subject that emits when the component has been destroyed.
  protected _onDestroy = new Subject<void>();
  //Loading indicator
  loading = false;
  color = "warn" as ThemePalette;
  mode = "indeterminate" as ProgressBarMode;
  value = 50;
  displayProgressBar = false;
  public seriesColors: string[] = chartSeriesColors;
  //Functions
  shortenLongText = shortenLongText;
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string
    , private router: Router
    , private statistiqueService: StatistiqueService
    , private listService: ListService
    , private dataModelService: DataModelService
    , private downloadService: DownloadService
    , public applicationUserContext: ApplicationUserContext
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.http = http;
    // override the route reuse strategy
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    }
    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationEnd) {
        // trick the Router into believing it"s last link wasn"t previously loaded
        this.router.navigated = false;
      }
    });
    //Sow/hide ModuleCollectivite header
    if (applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) {
      applicationUserContext.visibleHeaderCollectivite = true;
    }
    //Set unique collectivité if applicable
    let uniqueRefCollectivite: number = null;
    if (applicationUserContext.filterCollectivites.length == 1) {
      uniqueRefCollectivite = applicationUserContext.filterCollectivites[0].RefEntite;
    }
    //Set moment culture
    moment.locale(this.applicationUserContext.currentCultureName.substr(0, 2));
    //Set dynamic labels
    this.setDynamicLabels();
    //Get existing transporteur
    listService.getListTransporteur(null, false, false).subscribe(result => {
      this.transporteurList = result;
      this.filteredTransporteurList.next(this.transporteurList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing entite
    listService.getListEntite(null, null, "", false).subscribe(result => {
      this.entiteList = result;
      this.filteredEntiteList.next(this.entiteList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing centre de tri
    this.getListCentreDeTri();
    //Get existing Client
    listService.getListClient(null, null, null, null, null, false).subscribe(result => {
      this.clientList = result;
      this.filteredClientList.next(this.clientList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Collectivite
    listService.getListCollectivite(null, null, false, true, null, "", false).subscribe(result => {
      this.collectiviteList = result;
      this.filteredCollectiviteList.next(this.collectiviteList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Dpt départ
    listService.getListDpt().subscribe(result => {
      this.dptDepartList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Dpt arrivée
    listService.getListDpt().subscribe(result => {
      this.dptArriveeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing DR
    listService.getListUtilisateur(null, ModuleName.Qualite, HabilitationQualite.Utilisateur, null, false, false, false, false).subscribe(result => {
      this.dRList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ville départ
    listService.getListVilleDepart().subscribe(result => {
      this.villeDepartList = result;
      this.filteredVilleDepartList.next(this.villeDepartList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ville départ
    listService.getListAdresse(null, null, 1, null, null, 4, false).subscribe(result => {
      this.adresseDestinationList = result;
      this.filteredAdresseDestinationList.next(this.adresseDestinationList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ville arrivée
    listService.getListVilleArrivee().subscribe(result => {
      this.villeArriveeList = result;
      this.filteredVilleArriveeList.next(this.villeArriveeList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing EcoOrganisme
    listService.getListEcoOrganisme().subscribe(result => {
      this.ecoOrganismeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing EntiteType
    listService.getListEntiteType().subscribe(result => {
      this.entiteTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Pays
    listService.getListPays(null, false).subscribe(result => {
      this.paysList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Process
    this.getListProcess();
    //Get existing Produit
    this.getListProduit();
    //Get existing month
    listService.getListMonth().subscribe(result => {
      this.monthList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing quarter
    this.listService.getListQuarter().subscribe(result => {
      this.quarterList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing year
    listService.getListYear().subscribe(result => {
      this.yearList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing NonConformiteNature
    listService.getListNonConformiteNature(false, null).subscribe(result => {
      this.nonConformiteNatureList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ProduitGroupeReporting
    listService.getListProduitGroupeReporting().subscribe(result => {
      this.produitGroupeReportingList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing NonConformiteEtapeType
    listService.getListNonConformiteEtapeType(false, null).subscribe(result => {
      this.nonConformiteEtapeTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing CamionType
    listService.getListCamionType().subscribe(result => {
      this.camionTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ActionType
    listService.getListActionType(null).subscribe(result => {
      this.actionTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing AdresseType
    listService.getListAdresseType().subscribe(result => {
      this.adresseTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ContactAdresseProcess
    listService.getListContactAdresseProcess().subscribe(result => {
      this.contactAdresseProcessList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Fonction
    listService.getListFonction().subscribe(result => {
      this.fonctionList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Service
    listService.getListService().subscribe(result => {
      this.serviceList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing utilisateur
    listService.getListUtilisateur(null, "", "", null, false, false, false, false).subscribe(result => {
      this.utilisateurList = result;
      this.filteredUtilisateurList.next(this.utilisateurList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //On init, creation of columns
  ngOnInit() {
    let p: dataModelsInterfaces.ProduitList;
    this.dataSource = new StatistiqueDataSource(this.statistiqueService, this.snackBarQueueService, this.applicationUserContext);
    //Init filters and pager
    this.pageSize = 100;
    this.pagin.pageSize = 100;
    this.pageSizeOptions = [100];
    this.pagin.pageSizeOptions = [100]
    //Page size
    switch (this.applicationUserContext.currentMenu.name) {
      case MenuName.AdministrationMenuSuiviLogin:
      case MenuName.AdministrationMenuExtractionLogin:
      case MenuName.AnnuaireMenuEvolutionTonnage:
      case MenuName.AnnuaireMenuNbAffretementTransporteur:
        this.pageSize = 200;
        this.pagin.pageSize = 200;
        this.pageSizeOptions = [200];
        this.pagin.pageSizeOptions = [200]
        break;
    }
    //init filter default values
    switch (this.applicationUserContext.currentMenu.name) {
      case MenuName.AnnuaireMenuExtractionAnnuaire:
        this.applicationUserContext.filterEntiteTypes = [];
        break;
      case MenuName.AdministrationMenuExtractionLogin:
      case MenuName.AnnuaireMenuExtractionAction:
      case MenuName.LogistiqueMenuEtatCoutTransport:
      case MenuName.QualiteMenuRepartitionControleClient:
      case MenuName.QualiteMenuExtractionFicheControle:
      case MenuName.QualiteMenuExtractionControle:
      case MenuName.QualiteMenuExtractionNonConformite:
      case MenuName.QualiteMenuExtractionCVQ:
      case MenuName.QualiteMenuEtatControleReception:
      case MenuName.LogistiqueMenuChargementAnnule:
      case MenuName.LogistiqueMenuDelaiCommandeEnlevement:
      case MenuName.LogistiqueMenuEtatReceptionEmballagePlastique:
      case MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur:
      case MenuName.LogistiqueMenuExtractionOscar:
      case MenuName.LogistiqueMenuEtatKmMoyen:
      case MenuName.LogistiqueMenuEtatReceptionFournisseurChargement:
      case MenuName.LogistiqueMenuEtatReception:
      case MenuName.LogistiqueMenuSuiviCommandeClient:
      case MenuName.LogistiqueMenuSuiviCommandeClientProduit:
      case MenuName.LogistiqueMenuEtatReceptionProduit:
      case MenuName.LogistiqueMenuExtractionReception:
      case MenuName.LogistiqueMenuExtractionCommande:
      case MenuName.LogistiqueMenuEtatDesPoids:
      case MenuName.LogistiqueMenuEtatTonnageParProcess:
      case MenuName.LogistiqueMenuSuiviFacturationHC:
      case MenuName.LogistiqueMenuTonnageCollectiviteProduit:
      case MenuName.LogistiqueMenuEtatDesEnlevements:
      case MenuName.LogistiqueMenuExtractionLeko:
      case MenuName.LogistiqueMenuPoidsMoyenChargementProduit:
        if (this.applicationUserContext.filterBegin.isSame(moment([1, 0, 1, 0, 0, 0, 0])) || this.applicationUserContext.filterEnd.isSame(moment([1, 0, 1, 0, 0, 0, 0]))) {
          this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));
          this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0)).add(1, "months").add(-1, "days");
        }
        break;
      case MenuName.LogistiqueMenuLotDisponible:
        if (this.applicationUserContext.filterBegin.isSame(moment([1, 0, 1, 0, 0, 0, 0])) || this.applicationUserContext.filterEnd.isSame(moment([1, 0, 1, 0, 0, 0, 0]))) {
          this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear(), 0, 1, 0, 0, 0, 0));
          this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear(), 11, 31, 0, 0, 0, 0));
        }
        break;
      case MenuName.AdministrationMenuSuiviLogin:
      case MenuName.AnnuaireMenuEvolutionTonnage:
      case MenuName.AnnuaireMenuNbAffretementTransporteur:
        this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear(), 0, 1, 0, 0, 0, 0));
        this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear(), 11, 31, 0, 0, 0, 0));
        break;
      case MenuName.LogistiqueMenuEtatReceptionProduitDR:
      case MenuName.LogistiqueMenuTonnageCDTProduitComposant:
        if (this.applicationUserContext.filterYears.length === 0) {
          let y: appInterfaces.Year = { y: new Date().getFullYear() };
          this.applicationUserContext.filterYears.push(y);
        }
        break;
      case MenuName.LogistiqueMenuPartMarcheTransporteur:
        if (this.applicationUserContext.filterYears.length === 0) {
          let y: appInterfaces.Year = { y: new Date().getFullYear() };
          this.applicationUserContext.filterYears.push(y);
        }
        if (this.applicationUserContext.filterMonths.length === 0) {
          let m: appInterfaces.Month = {
            n: new Date().getMonth() + 1, d: moment().format("1901-MM-01T00:00:00")
            , name: getCulturedMonthName(new Date().getMonth() + 1, this.applicationUserContext)
          };
          this.applicationUserContext.filterMonths.push(m);
        }
        break;
      case MenuName.ModuleCollectiviteMenuStatistiques:
        if (this.applicationUserContext.filterBegin.isSame(moment([1, 0, 1, 0, 0, 0, 0])) || this.applicationUserContext.filterEnd.isSame(moment([1, 0, 1, 0, 0, 0, 0]))) {
          this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));
          this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0)).add(1, "months").add(-1, "days");
        }
        this.applicationUserContext.filterMonths = [];
        this.applicationUserContext.filterMonths.push({
          d: this.applicationUserContext.filterBegin.format("1901-MM-01T00:00:00"),
          n: this.applicationUserContext.filterBegin.month() + 1,
          name: getCulturedMonthName(this.applicationUserContext.filterBegin.month() + 1, this.applicationUserContext)
        } as appInterfaces.Month);
        this.applicationUserContext.filterYears = [];
        this.applicationUserContext.filterYears.push({ y: this.applicationUserContext.filterBegin.year() });
        this.applicationUserContext.filterMonthEnd = {
          d: this.applicationUserContext.filterEnd.format("1901-MM-01T00:00:00"),
          n: this.applicationUserContext.filterEnd.month() + 1,
          name: getCulturedMonthName(this.applicationUserContext.filterEnd.month() + 1, this.applicationUserContext)
        } as appInterfaces.Month;
        this.applicationUserContext.filterYearEnd = { y: this.applicationUserContext.filterEnd.year() } as appInterfaces.Year;
        //Current collectivité if applicable
        if (this.applicationUserContext.connectedUtilisateur.HabilitationModuleCollectivite === HabilitationModuleCollectivite.Utilisateur) {
          this.applicationUserContext.filterCollectivites.push(this.applicationUserContext.connectedUtilisateur.Collectivite);
        }
        break;
      case MenuName.LogistiqueMenuEtatDesFluxDev:
        if (this.applicationUserContext.filterBegin.isSame(moment([1, 0, 1, 0, 0, 0, 0])) || this.applicationUserContext.filterEnd.isSame(moment([1, 0, 1, 0, 0, 0, 0]))) {
          this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));
          this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0)).add(1, "months").add(-1, "days");
        }
        this.applicationUserContext.filterProduits = [];
        p = { RefProduit: 126, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 127, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 128, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 129, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 130, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 170, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 160, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 169, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 188, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 173, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 174, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 211, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        break;
      case MenuName.LogistiqueMenuEtatDesFluxDevLeko:
        if (this.applicationUserContext.filterBegin.isSame(moment([1, 0, 1, 0, 0, 0, 0])) || this.applicationUserContext.filterEnd.isSame(moment([1, 0, 1, 0, 0, 0, 0]))) {
          this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));
          this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0)).add(1, "months").add(-1, "days");
        }
        this.applicationUserContext.filterProduits = [];
        p = { RefProduit: 189, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        p = { RefProduit: 190, Libelle: "" };
        this.applicationUserContext.filterProduits.push(p);
        break;
    }
    this.form = new UntypedFormGroup({
      FilterText: new UntypedFormControl(this.applicationUserContext.filterText),
      DBegin: new UntypedFormControl(this.applicationUserContext.filterBegin.isSame(moment([1, 0, 1, 0, 0, 0, 0])) ? "" : this.applicationUserContext.filterBegin, Validators.required),
      DEnd: new UntypedFormControl(this.applicationUserContext.filterEnd.isSame(moment([1, 0, 1, 0, 0, 0, 0])) ? "" : this.applicationUserContext.filterEnd, Validators.required),
      CentreDeTriList: new UntypedFormControl(this.applicationUserContext.filterCentreDeTris),
      CentreDeTriListFilter: new UntypedFormControl(),
      ClientList: new UntypedFormControl(this.applicationUserContext.filterClients),
      ClientListFilter: new UntypedFormControl(),
      CollectiviteList: new UntypedFormControl(this.applicationUserContext.filterCollectivites, Validators.required),
      CollectiviteListFilter: new UntypedFormControl(),
      DptDepartList: new UntypedFormControl(this.applicationUserContext.filterDptDeparts),
      DptArriveeList: new UntypedFormControl(this.applicationUserContext.filterDptArrivees),
      DRList: new UntypedFormControl(this.applicationUserContext.filterDRs),
      EntiteList: new UntypedFormControl(this.applicationUserContext.filterEntites),
      EntiteListFilter: new UntypedFormControl(),
      EcoOrganismeList: new UntypedFormControl(),
      EntiteTypeList: new UntypedFormControl(),
      PaysList: new UntypedFormControl(this.applicationUserContext.filterPayss),
      ProcessList: new UntypedFormControl(this.applicationUserContext.filterProcesss),
      ProduitList: new UntypedFormControl(this.applicationUserContext.filterProduits),
      ProduitListFilter: new UntypedFormControl(),
      TransporteurList: new UntypedFormControl(this.applicationUserContext.filterTransporteurs),
      TransporteurListFilter: new UntypedFormControl(),
      VilleDepartList: new UntypedFormControl(this.applicationUserContext.filterVilleDeparts),
      VilleDepartListFilter: new UntypedFormControl(),
      AdresseDestinationList: new UntypedFormControl(this.applicationUserContext.filterAdresseDestinations),
      AdresseDestinationListFilter: new UntypedFormControl(),
      VilleArriveeList: new UntypedFormControl(this.applicationUserContext.filterVilleArrivees),
      VilleArriveeListFilter: new UntypedFormControl(),
      MonthList: new UntypedFormControl(),
      QuarterList: new UntypedFormControl(this.applicationUserContext.filterQuarters),
      YearList: new UntypedFormControl(null, Validators.required),
      MonthEndList: new UntypedFormControl(this.applicationUserContext.filterMonthEnd?.n, Validators.required),
      YearEndList: new UntypedFormControl(this.applicationUserContext.filterYearEnd?.y, Validators.required),
      NonConformiteEtapeTypeList: new UntypedFormControl(this.applicationUserContext.filterNonConformiteEtapeTypes),
      NonConformiteNatureList: new UntypedFormControl(this.applicationUserContext.filterNonConformiteNatures),
      ProduitGroupeReportingList: new UntypedFormControl(this.applicationUserContext.filterProduitGroupeReportings),
      CamionTypeList: new UntypedFormControl(this.applicationUserContext.filterCamionTypes),
      ActionTypeList: new UntypedFormControl(this.applicationUserContext.filterActionTypes),
      AdresseTypeList: new UntypedFormControl(this.applicationUserContext.filterAdresseTypes),
      ContactAdresseProcessList: new UntypedFormControl(this.applicationUserContext.filterContactAdresseProcesss),
      FonctionList: new UntypedFormControl(this.applicationUserContext.filterFonctions),
      ServiceList: new UntypedFormControl(this.applicationUserContext.filterServices),
      Moins10Euros: new UntypedFormControl(this.applicationUserContext.filterMoins10Euros),
      Collecte: new UntypedFormControl(this.applicationUserContext.filterCollecte),
      Contrat: new UntypedFormControl(this.applicationUserContext.filterContrat),
      EE: new UntypedFormControl(this.applicationUserContext.filterEE == null ? "" : this.applicationUserContext.filterEE.toString()),
      Actif: new UntypedFormControl(this.applicationUserContext.filterActif),
      EmailTypeList: new UntypedFormControl(this.applicationUserContext.filterEmailType, Validators.required),
      UtilisateurList: new UntypedFormControl(this.applicationUserContext.filterUtilisateurs),
      UtilisateurListFilter: new UntypedFormControl(),
      FirstLogin: new UntypedFormControl(this.applicationUserContext.filterFirstLogin),
      DayWeekMonth: new UntypedFormControl(this.applicationUserContext.filterDayWeekMonth),
      StatistiqueCollectivite: new UntypedFormControl("DonneeCDT")
    });
    //Manage screen
    this.manageScreen()
    //Set values depending on Managscrenn (simple/multiple selects)
    if (this.monthListMultiple) { this.form.get("MonthList").setValue(this.applicationUserContext.filterMonths); }
    else { this.form.get("MonthList").setValue(this.applicationUserContext.filterMonths.length > 0 ? this.applicationUserContext.filterMonths[0] : null); }
    if (this.yearListMultiple) { this.form.get("YearList").setValue(this.applicationUserContext.filterYears); }
    else { this.form.get("YearList").setValue(this.applicationUserContext.filterYears.length > 0 ? this.applicationUserContext.filterYears[0] : null); }
    this.form.get("EcoOrganismeList").setValue(this.applicationUserContext.filterEcoOrganismes);
    if (this.entiteTypeListMultiple) { this.form.get("EntiteTypeList").setValue(this.applicationUserContext.filterEntiteTypes); }
    else { this.form.get("EntiteTypeList").setValue(this.applicationUserContext.filterEntiteTypes.length > 0 ? this.applicationUserContext.filterEntiteTypes[0] : null); }
    if (this.collectiviteListMultiple) { this.form.get("CollectiviteList").setValue(this.applicationUserContext.filterCollectivites); }
    else { this.form.get("CollectiviteList").setValue(this.applicationUserContext.filterCollectivites.length > 0 ? this.applicationUserContext.filterCollectivites[0] : null); }
    //Localization
    this.pagin._intl.firstPageLabel = this.applicationUserContext.getCulturedRessourceText(294);
    this.pagin._intl.lastPageLabel = this.applicationUserContext.getCulturedRessourceText(295);
    this.pagin._intl.itemsPerPageLabel = this.applicationUserContext.getCulturedRessourceText(296);
    this.pagin._intl.previousPageLabel = this.applicationUserContext.getCulturedRessourceText(297);
    this.pagin._intl.nextPageLabel = this.applicationUserContext.getCulturedRessourceText(298);
    this.pagin._intl.getRangeLabel = (page: number, pageSize: number, length: number) => { if (length === 0 || pageSize === 0) { return ("0 de " + length); } length = Math.max(length, 0); const startIndex = page * pageSize; const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize; return ((startIndex + 1) + " - " + endIndex + " de " + length); }
    //Load data if form is valid
    this.form.updateValueAndValidity();
    if (!this.form.invalid
      && this.applicationUserContext.currentMenu.name != MenuName.AnnuaireMenuExtractionAnnuaire
      && (this.applicationUserContext.currentMenu.name != MenuName.AnnuaireMenuEvolutionTonnage || this.applicationUserContext.filterCollectivites.length == 1)
    ) {
      this.calculateStatistics();
    }
    // listen for search field value changes
    this.form.get("EntiteListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterEntitesMulti();
      });
    this.form.get("TransporteurListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterTransporteursMulti();
      });
    this.form.get("CollectiviteListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCollectivitesMulti();
      });
    this.form.get("ClientListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterClientsMulti();
      });
    this.form.get("CentreDeTriListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCentreDeTrisMulti();
      });
    this.form.get("ProduitListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterProduitsMulti();
      });
    this.form.get("VilleDepartListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterVilleDepartsMulti();
      });
    this.form.get("AdresseDestinationListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterAdresseDestinationsMulti();
      });
    this.form.get("VilleArriveeListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterVilleArriveesMulti();
      });
    this.form.get("UtilisateurListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterUtilisateursMulti();
      });
    //Init available columns for data extract
    this.contactAvailableColumns = createContactAvailableColumns(this.applicationUserContext.contactAvailableColumns, null, this.dataModelService);
    this.contactSelectedColumns = [];
  }
  //-----------------------------------------------------------------------------------
  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }
  //-----------------------------------------------------------------------------------
  //Get list Produits
  getListProduit() {
    //Get existing Produit
    //Collectivite
    let refCollectivite: number = null;
    if (this.applicationUserContext.connectedUtilisateur.Collectivite) {
      refCollectivite = this.applicationUserContext.connectedUtilisateur.Collectivite.RefEntite;
    }
    //Specific Entite
    let refEntite: number = null;
    if (this.applicationUserContext.connectedUtilisateur.CentreDeTri) {
      refEntite = this.applicationUserContext.connectedUtilisateur.CentreDeTri.RefEntite;
    }
    if (this.applicationUserContext.connectedUtilisateur.Client) {
      refEntite = this.applicationUserContext.connectedUtilisateur.Client.RefEntite;
    }
    this.listService.getListProduit(null, refEntite, null, null, null, null, refCollectivite, false, false).subscribe(result => {
      //Change produit label if applicable
      if (this.applicationUserContext.connectedUtilisateur.Collectivite) {
        result.forEach(p => p.Libelle = p.NomCommun);
      }
      this.produitList = result;
      this.filteredProduitList.next(this.produitList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Get list Process
  getListProcess() {
    //Get existing Process
    let refCollectivite: number = null;
    if (this.applicationUserContext.currentMenu.name == MenuName.AnnuaireMenuEvolutionTonnage
      || this.applicationUserContext.currentMenu.name === MenuName.ModuleCollectiviteMenuStatistiques) {
      if (this.applicationUserContext.filterCollectivites.length == 1) {
        refCollectivite = this.applicationUserContext.filterCollectivites[0].RefEntite;
      }
      else { refCollectivite = 0; }
    }
    this.listService.getListProcess(null, refCollectivite).subscribe(result => {
      this.processList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Get list CentreDeTri
  getListCentreDeTri() {
    //Get existing Centre De Tri
    let refCollectivite: number = null;
    if (this.applicationUserContext.currentMenu.name == MenuName.AnnuaireMenuEvolutionTonnage
      || this.applicationUserContext.currentMenu.name === MenuName.ModuleCollectiviteMenuStatistiques) {
      if (this.applicationUserContext.filterCollectivites.length == 1) {
        refCollectivite = this.applicationUserContext.filterCollectivites[0].RefEntite;
      }
      else { refCollectivite = 0; }
    }
    this.listService.getListCentreDeTri(null, refCollectivite, false).subscribe(result => {
      this.centreDeTriList = result;
      this.filteredCentreDeTriList.next(this.centreDeTriList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Dynamic labels
  setDynamicLabels() {
    this.labelDBeginDEndFC = "";
    if (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuRepartitionControleClient
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuExtractionFicheControle
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuExtractionControle
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuExtractionCVQ
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuEtatControleReception
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuEtatIncitationQualite
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatReceptionEmballagePlastique
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuExtractionOscar
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatDesFluxDev
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatDesFluxDevLeko
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatReceptionProduit
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuExtractionReception
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuExtractionLeko
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatReceptionFournisseurChargement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatReception
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatKmMoyen
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSuiviFacturationHC
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatDesPoids
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTonnageCollectiviteProduit
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatTonnageParProcess
    ) {
      this.labelDBeginDEndFC = this.applicationUserContext.getCulturedRessourceText(894);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuExtractionNonConformite
    ) {
      this.labelDBeginDEndFC = this.applicationUserContext.getCulturedRessourceText(858);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.AnnuaireMenuSuiviEnvois
    ) {
      this.labelDBeginDEndFC = this.applicationUserContext.getCulturedRessourceText(1085);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatDesEnlevements
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuPoidsMoyenChargementProduit
    ) {
      this.labelDBeginDEndFC = this.applicationUserContext.getCulturedRessourceText(894);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSuiviCommandeClient
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSuiviCommandeClientProduit
    ) {
      this.labelDBeginDEndFC = this.applicationUserContext.getCulturedRessourceText(426);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSuiviCommandeClient
    ) {
      this.placeholderDBeginFC = this.applicationUserContext.getCulturedRessourceText(993);
      this.placeholderDEndFC = this.applicationUserContext.getCulturedRessourceText(994);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSuiviCommandeClientProduit
    ) {
      this.placeholderDBeginFC = this.applicationUserContext.getCulturedRessourceText(995);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuChargementAnnule
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuDelaiCommandeEnlevement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuLotDisponible
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuExtractionCommande
    ) {
      this.labelDBeginDEndFC = this.applicationUserContext.getCulturedRessourceText(1511);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuExtractionCommande
    ) {
      this.labelMonthListFC = this.applicationUserContext.getCulturedRessourceText(436);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient
    ) {
      this.placeHolderMonthListFC = this.applicationUserContext.getCulturedRessourceText(1057);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuEtatIncitationQualite
    ) {
      this.optionSousOuHorsContratFC = this.applicationUserContext.getCulturedRessourceText(1002);
      this.optionSousContratFC = this.applicationUserContext.getCulturedRessourceText(1000);
      this.optionHorsContratFC = this.applicationUserContext.getCulturedRessourceText(1001);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTonnageCDTProduitComposant
    ) {
      this.labelCollectiviteListFC = this.applicationUserContext.getCulturedRessourceText(316);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTonnageCDTProduitComposant
    ) {
      this.labelQuarterListFC = this.applicationUserContext.getCulturedRessourceText(894);
      this.labelYearListFC = this.applicationUserContext.getCulturedRessourceText(894);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.ModuleCollectiviteMenuStatistiques
    ) {
      this.placeHolderMonthListFC = this.applicationUserContext.getCulturedRessourceText(993);
      this.placeHolderYearListFC = this.applicationUserContext.getCulturedRessourceText(1343);
    }
  }
  //-----------------------------------------------------------------------------------
  //View init
  ngAfterViewInit() {
    //Pagination
    this.pagin.page
      .pipe(
        tap(() => { this.loadPage("") })
      )
      .subscribe();
  }
  //-----------------------------------------------------------------------------------
  //Manage visibleFilter controls
  manageScreen() {
    //RAZ
    this.form.get("DBegin").disable();
    this.form.get("DEnd").disable();
    this.form.get("CollectiviteList").disable();
    this.form.get("EmailTypeList").disable();
    this.form.get("MonthList").disable();
    this.form.get("YearList").disable();
    this.form.get("MonthEndList").disable();
    this.form.get("YearEndList").disable();
    this.ResultType.setValue("data");
    this.showResultType = false;
    //Set visibleFilter controls
    switch (this.applicationUserContext.currentMenu.name) {
      case MenuName.AdministrationMenuSuiviLogin:
        this.showResultType = true;
        this.ResultType.setValue("chart");
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterEntiteList = true;
        this.visibleFilterUtilisateurList = true;
        this.visibleFilterDayWeekMonth = true;
        this.visibleFilterEntiteTypeList = true;
        break;
      case MenuName.AdministrationMenuExtractionLogin:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterEntiteList = true;
        this.visibleFilterUtilisateurList = true;
        this.visibleFilterFirstLogin = true;
        this.visibleFilterEntiteTypeList = true;
        break;
      case MenuName.AdministrationMenuRepreneur:
      case MenuName.AdministrationMenuParamEmail:
      case MenuName.AdministrationMenuActionType:
        this.visibleFilterFilterText = true;
        break;
      case MenuName.AnnuaireMenuExtractionAction:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterDRList = true;
        this.visibleFilterEntiteTypeList = true;
        this.visibleFilterActionTypeList = true;
        this.visibleFilterFilterText = true;
        break;
      case MenuName.AnnuaireMenuExtractionAnnuaire:
        this.visibleFilterDRList = true;
        this.entiteTypeListMultiple = false;
        this.visibleFilterEntiteTypeList = true;
        this.visibleFilterProcessList = true;
        this.visibleFilterAdresseTypeList = true;
        this.visibleFilterContactAdresseProcessList = true;
        this.visibleFilterFonctionList = true;
        this.visibleFilterServiceList = true;
        break;
      case MenuName.AnnuaireMenuNbAffretementTransporteur:
        this.showResultType = true;
        this.ResultType.setValue("chart");
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterTransporteurList = true;
        this.visibleFilterPaysList = true;
        break;
      case MenuName.LogistiqueMenuDelaiCommandeEnlevement:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique == HabilitationLogistique.Administrateur.toString()
          || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique == HabilitationLogistique.Utilisateur.toString()) {
          this.visibleFilterDRList = true;
          this.visibleFilterProduitList = true;
          this.visibleFilterCentreDeTriList = true;
        }
        break;
      case MenuName.LogistiqueMenuEtatCoutTransport:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterVilleDepartList = true;
        this.visibleFilterVilleArriveeList = true;
        this.visibleFilterTransporteurList = true;
        this.visibleFilterPaysList = true;
        break;
      case MenuName.LogistiqueMenuLotDisponible:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterDRList = true;
        this.visibleFilterProduitList = true;
        this.visibleFilterCollecte = true;
        break;
      case MenuName.LogistiqueMenuEtatTarifTransporteurClient:
        this.visibleFilterDptDepartList = true;
        this.visibleFilterAdresseDestinationList = true;
        break;
      case MenuName.LogistiqueMenuPartMarcheTransporteur:
        this.visibleFilterMonthList = true;
        this.form.get("MonthList").enable();
        this.visibleFilterYearList = true;
        this.form.get("YearList").enable();
        this.visibleFilterPaysList = true;
        this.visibleFilterClientList = true;
        break;
      case MenuName.LogistiqueMenuEtatKmMoyen:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCollecte = true;
        break;
      case MenuName.LogistiqueMenuEtatReceptionFournisseurChargement:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCentreDeTriList = true;
        this.form.get("CentreDeTriList").setValidators(Validators.required);
        this.requiredCentreDeTriList = true;
        this.form.get("CentreDeTriList").updateValueAndValidity();
        break;
      case MenuName.LogistiqueMenuEtatReception:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterClientList = true;
        break;
      case MenuName.LogistiqueMenuSuiviCommandeClient:
        this.datePickerMonthYear = true;
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterVilleArriveeList = true;
        this.visibleFilterClientList = true;
        this.visibleFilterProduitList = true;
        this.visibleFilterCollecte = true;
        break;
      case MenuName.LogistiqueMenuSuiviCommandeClientProduit:
        this.datePickerMonthYear = true;
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterCollecte = true;
        this.visibleFilterProduitList = true;
        break;
      case MenuName.LogistiqueMenuEtatReceptionProduit:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCollecte = true;
        this.visibleFilterProduitList = true;
        this.visibleFilterProduitGroupeReportingList = true;
        break;
      case MenuName.LogistiqueMenuEtatReceptionProduitDR:
        this.visibleFilterYearList = true;
        this.form.get("YearList").enable();
        this.visibleFilterProduitList = true;
        this.visibleFilterCollecte = true;
        this.visibleFilterDRList = true;
        break;
      case MenuName.LogistiqueMenuExtractionReception:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterDRList = true;
        this.visibleFilterClientList = true;
        this.visibleFilterTransporteurList = true;
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterProduitList = true;
        this.visibleFilterCamionTypeList = true;
        this.visibleFilterCollecte = true;
        break;
      case MenuName.LogistiqueMenuExtractionCommande:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterClientList = true;
        this.visibleFilterTransporteurList = true;
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterProduitList = true;
        this.visibleFilterDptDepartList = true;
        this.visibleFilterMonthList = true;
        this.form.get("MonthList").enable();
        break;
      case MenuName.LogistiqueMenuEtatDesPoids:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCollecte = true;
        this.visibleFilterEcoOrganismeList = true;
        break;
      case MenuName.LogistiqueMenuEtatTonnageParProcess:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterProduitList = true;
        this.visibleFilterProcessList = true;
        this.visibleFilterCollecte = true;
        break;
      case MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient:
      case MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient:
        this.yearListMultiple = false;
        this.form.get("YearList").enable();
        this.visibleFilterYearList = true;
        this.form.get("MonthList").setValidators(Validators.required);
        this.requiredMonthList = true;
        this.form.get("MonthList").updateValueAndValidity();
        this.monthListMultiple = false;
        this.form.get("MonthList").enable();
        this.visibleFilterMonthList = true;
        this.form.get("MonthList").enable();
        break;
      case MenuName.LogistiqueMenuSuiviFacturationHC:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        break;
      case MenuName.LogistiqueMenuTonnageCollectiviteProduit:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCentreDeTriList = true;
        this.form.get("CentreDeTriList").setValidators(Validators.required);
        this.requiredCentreDeTriList = true;
        this.form.get("CentreDeTriList").updateValueAndValidity();
        break;
      case MenuName.LogistiqueMenuTonnageCDTProduitComposant:
        this.visibleFilterQuarterList = true;
        this.form.get("YearList").enable();
        this.visibleFilterYearList = true;
        this.form.get("CollectiviteList").enable();
        this.visibleFilterCollectiviteList = true;
        break;
      case MenuName.LogistiqueMenuTonnageCLSousContrat:
        this.form.get("YearList").enable();
        this.visibleFilterYearList = true;
        this.visibleFilterDRList = true;
        break;
      case MenuName.LogistiqueMenuListeProduit:
        this.visibleFilterCollecte = true;
        this.visibleFilterProduitList = true;
        this.visibleFilterActif = true;
        break;
      case MenuName.LogistiqueMenuChargementAnnule:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterDRList = true;
        this.visibleFilterTransporteurList = true;
        break;
      case MenuName.LogistiqueMenuEtatReceptionEmballagePlastique:
      case MenuName.LogistiqueMenuExtractionOscar:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterProduitList = true;
        break;
      case MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterClientList = true;
        this.visibleFilterProduitList = true;
        break;
      case MenuName.LogistiqueMenuEtatDesFluxDev:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterProduitList = true;
        break;
      case MenuName.LogistiqueMenuEtatDesFluxDevLeko:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterProduitList = true;
        break;
      case MenuName.LogistiqueMenuEtatDesEnlevements:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterProduitList = true;
        break;
      case MenuName.LogistiqueMenuExtractionLeko:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        break;
      case MenuName.LogistiqueMenuPoidsMoyenChargementProduit:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterProduitList = true;
        break;
      case MenuName.AnnuaireMenuEvolutionTonnage:
        this.showResultType = true;
        this.ResultType.setValue("chart");
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterProduitList = true;
        this.visibleFilterCollecte = true;
        this.collectiviteListMultiple = false;
        this.form.get("CollectiviteList").enable();
        this.visibleFilterCollectiviteList = true;
        break;
      case MenuName.QualiteMenuRepartitionControleClient:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterProduitList = true;
        this.visibleFilterClientList = !this.applicationUserContext.connectedUtilisateur.Client;
        this.visibleFilterContrat = (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Administrateur || this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Utilisateur);
        break;
      case MenuName.QualiteMenuExtractionFicheControle:
      case MenuName.QualiteMenuExtractionControle:
      case MenuName.QualiteMenuExtractionCVQ:
      case MenuName.QualiteMenuEtatControleReception:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterProduitList = true;
        this.visibleFilterClientList = !this.applicationUserContext.connectedUtilisateur.Client;
        this.visibleFilterDRList = (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Administrateur || this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Utilisateur);
        this.visibleFilterContrat = (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Administrateur || this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Utilisateur);
        this.visibleFilterCollecte = !this.applicationUserContext.connectedUtilisateur.Client;
        break;
      case MenuName.QualiteMenuEtatIncitationQualite:
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterMoins10Euros = true;
        this.visibleFilterDRList = true;
        this.visibleFilterContrat = true;
        this.visibleFilterYearList = true;
        this.form.get("YearList").enable();
        break;
      case MenuName.QualiteMenuExtractionNonConformite:
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterCentreDeTriList = true;
        this.visibleFilterProduitList = true;
        this.visibleFilterClientList = !this.applicationUserContext.connectedUtilisateur.Client;
        this.visibleFilterNonConformiteEtapeTypeList = true;
        this.visibleFilterNonConformiteNatureList = true;
        this.visibleFilterDRList = (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Administrateur || this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Utilisateur);
        this.visibleFilterContrat = (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Administrateur || this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Utilisateur);
        this.visibleFilterCollecte = !this.applicationUserContext.connectedUtilisateur.Client;
        break;
      case MenuName.AnnuaireMenuSuiviEnvois:
        this.visibleFilterFilterText = true;
        this.visibleFilterDBegin = true;
        this.form.get("DBegin").enable();
        this.visibleFilterDEnd = true;
        this.form.get("DEnd").enable();
        this.visibleFilterEmailTypeList = true;
        this.form.get("EmailTypeList").enable();
        break;
      case MenuName.ModuleCollectiviteMenuStatistiques:
        this.visibleStatistiqueCollectivite = true;
        if (this.applicationUserContext.connectedUtilisateur.HabilitationModuleCollectivite === HabilitationModuleCollectivite.Administrateur) {
          this.visibleFilterCollectiviteList = true;
        }
        this.visibleStatistiqueCollectivite = true;
        this.form.get("CollectiviteList").enable();
        this.collectiviteListMultiple = false;
        this.visibleFilterCollecteChip = true;
        this.monthListMultiple = false
        this.yearListMultiple = false;
        this.visibleFilterMonthList = false;
        this.form.get("MonthList").disable();
        this.visibleFilterYearList = false;
        this.form.get("YearList").disable();
        this.visibleFilterMonthEndList = false;
        this.form.get("MonthEndList").disable();
        this.visibleFilterYearEndList = false;
        this.form.get("YearEndList").disable();
        this.visibleFilterProduitList = false;
        this.visibleFilterProcessList = false;
        this.visibleFilterCentreDeTriList = false;
        switch (this.form.get("StatistiqueCollectivite").value) {
          case "DonneeCDT":
            this.requiredMonthList = true;
            this.visibleFilterMonthList = true;
            this.form.get("MonthList").enable();
            this.visibleFilterYearList = true;
            this.form.get("YearList").enable();
            this.visibleFilterMonthEndList = true;
            this.form.get("MonthEndList").enable();
            this.visibleFilterYearEndList = true;
            this.form.get("YearEndList").enable();
            break;
          case "ExtractionRSE":
            this.visibleFilterCollecteChip = false;
            this.requiredMonthList = true;
            this.visibleFilterMonthList = true;
            this.form.get("MonthList").enable();
            this.visibleFilterYearList = true;
            this.form.get("YearList").enable();
            this.visibleFilterMonthEndList = true;
            this.form.get("MonthEndList").enable();
            this.visibleFilterYearEndList = true;
            this.form.get("YearEndList").enable();
            break;
          case "ExtractionPrixReprise":
            this.requiredMonthList = true;
            this.visibleFilterMonthList = true;
            this.form.get("MonthList").enable();
            this.visibleFilterYearList = true;
            this.form.get("YearList").enable();
            this.visibleFilterMonthEndList = true;
            this.form.get("MonthEndList").enable();
            this.visibleFilterYearEndList = true;
            this.form.get("YearEndList").enable();
            this.visibleFilterProduitList = true;
            break;
          case "DestinationTonnage":
            this.showResultType = true;
            this.ResultType.setValue("chart");
            this.requiredMonthList = true;
            this.visibleFilterMonthList = true;
            this.form.get("MonthList").enable();
            this.visibleFilterYearList = true;
            this.form.get("YearList").enable();
            this.visibleFilterMonthEndList = true;
            this.form.get("MonthEndList").enable();
            this.visibleFilterYearEndList = true;
            this.form.get("YearEndList").enable();
            this.visibleFilterCentreDeTriList = true;
            break;
          case "DeboucheBalle":
            this.showResultType = true;
            this.ResultType.setValue("chart");
            this.visibleFilterYearList = true;
            this.form.get("YearList").enable();
            this.visibleFilterCentreDeTriList = true;
            break;
          case "EvolutionTonnage":
            this.showResultType = true;
            this.ResultType.setValue("chart");
            this.requiredMonthList = true;
            this.visibleFilterMonthList = true;
            this.form.get("MonthList").enable();
            this.visibleFilterYearList = true;
            this.form.get("YearList").enable();
            this.visibleFilterMonthEndList = true;
            this.form.get("MonthEndList").enable();
            this.visibleFilterYearEndList = true;
            this.form.get("YearEndList").enable();
            this.visibleFilterProduitList = true;
            break;
        }
        break;
    }
  }
  //-----------------------------------------------------------------------------------
  //Save filters
  saveFilter() {
    //Save filters
    this.applicationUserContext.filterText = this.form.get("FilterText").value;
    if (this.applicationUserContext.currentMenu.name === MenuName.ModuleCollectiviteMenuStatistiques) {
      //this.applicationUserContext.filterBegin = moment([1, 0, 1, 0, 0, 0, 0]); //Moment UTC date
      //this.applicationUserContext.filterEnd = moment([1, 0, 1, 0, 0, 0, 0]); //Moment UTC date
      if (this.form.get("MonthList").value && this.form.get("YearList").value) {
        this.applicationUserContext.filterBegin = moment([(this.form.get("YearList").value as appInterfaces.Year).y
          , (this.form.get("MonthList").value as appInterfaces.Month).n - 1, 1, 0, 0, 0, 0]);
      }
      if (this.form.get("MonthEndList").value && this.form.get("YearEndList").value) {
        this.applicationUserContext.filterEnd = moment([this.form.get("YearEndList").value
          , this.form.get("MonthEndList").value - 1, 1, 0, 0, 0, 0]).add(1, "M").add(-1, "d");
      }
    }
    else {
      this.applicationUserContext.filterBegin = (this.form.get("DBegin").value ? moment(this.form.get("DBegin").value) : moment([1, 0, 1, 0, 0, 0, 0])); //Moment UTC date
      this.applicationUserContext.filterEnd = (this.form.get("DEnd").value ? moment(this.form.get("DEnd").value) : moment([1, 0, 1, 0, 0, 0, 0])); //Moment UTC date
    }
    this.applicationUserContext.filterDptDeparts = this.form.get("DptDepartList").value;
    this.applicationUserContext.filterDptArrivees = this.form.get("DptArriveeList").value;
    this.applicationUserContext.filterDRs = this.form.get("DRList").value;
    this.applicationUserContext.filterClients = this.form.get("ClientList").value;
    this.applicationUserContext.filterCentreDeTris = this.form.get("CentreDeTriList").value;
    if (this.collectiviteListMultiple) { this.applicationUserContext.filterCollectivites = this.form.get("CollectiviteList").value; }
    else { this.applicationUserContext.filterCollectivites = [this.form.get("CollectiviteList").value]; }
    if (this.entiteTypeListMultiple) { this.applicationUserContext.filterEcoOrganismes = this.form.get("EcoOrganismeList").value; }
    else { this.applicationUserContext.filterEcoOrganismes = [this.form.get("EcoOrganismeList").value]; }
    if (this.entiteTypeListMultiple) { this.applicationUserContext.filterEntiteTypes = this.form.get("EntiteTypeList").value; }
    else { this.applicationUserContext.filterEntiteTypes = [this.form.get("EntiteTypeList").value]; }
    this.applicationUserContext.filterPayss = this.form.get("PaysList").value;
    this.applicationUserContext.filterProcesss = this.form.get("ProcessList").value;
    this.applicationUserContext.filterProduits = this.form.get("ProduitList").value;
    this.applicationUserContext.filterEntites = this.form.get("EntiteList").value;
    this.applicationUserContext.filterTransporteurs = this.form.get("TransporteurList").value;
    this.applicationUserContext.filterVilleDeparts = this.form.get("VilleDepartList").value;
    this.applicationUserContext.filterAdresseDestinations = this.form.get("AdresseDestinationList").value;
    this.applicationUserContext.filterVilleArrivees = this.form.get("VilleArriveeList").value;
    if (this.monthListMultiple) { this.applicationUserContext.filterMonths = this.form.get("MonthList").value; }
    else { this.applicationUserContext.filterMonths = [this.form.get("MonthList").value]; }
    this.applicationUserContext.filterQuarters = this.form.get("QuarterList").value;
    if (this.yearListMultiple) { this.applicationUserContext.filterYears = this.form.get("YearList").value; }
    else { this.applicationUserContext.filterYears = [this.form.get("YearList").value]; }
    this.applicationUserContext.filterNonConformiteEtapeTypes = this.form.get("NonConformiteEtapeTypeList").value;
    this.applicationUserContext.filterNonConformiteNatures = this.form.get("NonConformiteNatureList").value;
    this.applicationUserContext.filterProduitGroupeReportings = this.form.get("ProduitGroupeReportingList").value;
    this.applicationUserContext.filterCamionTypes = this.form.get("CamionTypeList").value;
    this.applicationUserContext.filterActionTypes = this.form.get("ActionTypeList").value;
    this.applicationUserContext.filterAdresseTypes = this.form.get("AdresseTypeList").value;
    this.applicationUserContext.filterContactAdresseProcesss = this.form.get("ContactAdresseProcessList").value;
    this.applicationUserContext.filterFonctions = this.form.get("FonctionList").value;
    this.applicationUserContext.filterServices = this.form.get("ServiceList").value;
    this.applicationUserContext.filterMoins10Euros = this.form.get("Moins10Euros").value;
    this.applicationUserContext.filterCollecte = this.form.get("Collecte").value;
    this.applicationUserContext.filterContrat = this.form.get("Contrat").value;
    this.applicationUserContext.filterEE = (this.form.get("EE").value ? this.form.get("EE").value : null);
    this.applicationUserContext.filterActif = this.form.get("Actif").value;
    this.applicationUserContext.filterEmailType = this.form.get("EmailTypeList").value;
    this.applicationUserContext.filterContactSelectedColumns = this.contactSelectedColumns;
    this.applicationUserContext.filterFirstLogin = this.form.get("FirstLogin").value;
    this.applicationUserContext.filterDayWeekMonth = this.form.get("DayWeekMonth").value;
    this.applicationUserContext.filterUtilisateurs = this.form.get("UtilisateurList").value;
    this.applicationUserContext.statType = this.form.get("StatistiqueCollectivite").value;
  }
  //-----------------------------------------------------------------------------------
  //Calculate statistics
  calculateStatistics() {
    //Save filters
    this.saveFilter();
    //Set filters
    this.setFilters();
    //Reset pager
    this.pagin.pageIndex = 0
    //Load data
    this.loadPage("");
  }
  //-----------------------------------------------------------------------------------
  //Set filters
  setFilters() {
    this.filterText = this.applicationUserContext.filterText;
    this.filterBegin = this.applicationUserContext.filterBegin.format("YYYY-MM-DD HH:mm:ss.SSS");
    this.filterEnd = this.applicationUserContext.filterEnd.format("YYYY-MM-DD HH:mm:ss.SSS");
    if (this.applicationUserContext.filterClients && this.applicationUserContext.filterClients.length > 0) {
      this.filterClients = Array.prototype.map.call(this.applicationUserContext.filterClients, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterClients = "";
    }
    if (this.applicationUserContext.filterCentreDeTris && this.applicationUserContext.filterCentreDeTris.length > 0) {
      this.filterCentreDeTris = Array.prototype.map.call(this.applicationUserContext.filterCentreDeTris, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterCentreDeTris = "";
    }
    if (this.applicationUserContext.filterCollectivites && this.applicationUserContext.filterCollectivites.length > 0) {
      this.filterCollectivites = Array.prototype.map.call(this.applicationUserContext.filterCollectivites, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterCollectivites = "";
    }
    if (this.applicationUserContext.filterDptDeparts && this.applicationUserContext.filterDptDeparts.length > 0) {
      this.filterDptDeparts = Array.prototype.map.call(this.applicationUserContext.filterDptDeparts, function (item: dataModelsInterfaces.Dpt) { return item.RefDpt; }).join(",");
    }
    else {
      this.filterDptDeparts = "";
    }
    if (this.applicationUserContext.filterDptArrivees && this.applicationUserContext.filterDptArrivees.length > 0) {
      this.filterDptArrivees = Array.prototype.map.call(this.applicationUserContext.filterDptArrivees, function (item: dataModelsInterfaces.Dpt) { return item.RefDpt; }).join(",");
    }
    else {
      this.filterDptArrivees = "";
    }
    if (this.applicationUserContext.filterDRs && this.applicationUserContext.filterDRs.length > 0) {
      this.filterDRs = Array.prototype.map.call(this.applicationUserContext.filterDRs, function (item: dataModelsInterfaces.UtilisateurList) { return item.RefUtilisateur; }).join(",");
    }
    else {
      this.filterDRs = "";
    }
    if (this.applicationUserContext.filterEcoOrganismes && this.applicationUserContext.filterEcoOrganismes.length > 0) {
      this.filterEcoOrganismes = Array.prototype.map.call(this.applicationUserContext.filterEcoOrganismes, function (item: dataModelsInterfaces.EcoOrganisme) { return item.RefEcoOrganisme; }).join(",");
    }
    else {
      this.filterEcoOrganismes = "";
    }
    if (this.applicationUserContext.filterEntiteTypes && this.applicationUserContext.filterEntiteTypes.length > 0) {
      this.filterEntiteTypes = Array.prototype.map.call(this.applicationUserContext.filterEntiteTypes, function (item: dataModelsInterfaces.EntiteType) { return item.RefEntiteType; }).join(",");
    }
    else {
      this.filterEntiteTypes = "";
    }
    if (this.applicationUserContext.filterPayss && this.applicationUserContext.filterPayss.length > 0) {
      this.filterPayss = Array.prototype.map.call(this.applicationUserContext.filterPayss, function (item: dataModelsInterfaces.Pays) { return item.RefPays; }).join(",");
    }
    else {
      this.filterPayss = "";
    }
    if (this.applicationUserContext.filterProcesss && this.applicationUserContext.filterProcesss.length > 0) {
      this.filterProcesss = Array.prototype.map.call(this.applicationUserContext.filterProcesss, function (item: dataModelsInterfaces.Process) { return item.RefProcess; }).join(",");
    }
    else {
      this.filterProcesss = "";
    }
    if (this.applicationUserContext.filterProduits && this.applicationUserContext.filterProduits.length > 0) {
      this.filterProduits = Array.prototype.map.call(this.applicationUserContext.filterProduits, function (item: dataModelsInterfaces.ProduitList) { return item.RefProduit; }).join(",");
    }
    else {
      this.filterProduits = "";
    }
    if (this.applicationUserContext.filterTransporteurs && this.applicationUserContext.filterTransporteurs.length > 0) {
      this.filterTransporteurs = Array.prototype.map.call(this.applicationUserContext.filterTransporteurs, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterTransporteurs = "";
    }
    if (this.applicationUserContext.filterEntites && this.applicationUserContext.filterEntites.length > 0) {
      this.filterEntites = Array.prototype.map.call(this.applicationUserContext.filterEntites, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterEntites = "";
    }
    if (this.applicationUserContext.filterVilleDeparts && this.applicationUserContext.filterVilleDeparts.length > 0) {
      this.filterVilleDeparts = Array.prototype.map.call(this.applicationUserContext.filterVilleDeparts, function (item: any) { return item.Ville; }).join(",");
    }
    else {
      this.filterVilleDeparts = "";
    }
    if (this.applicationUserContext.filterAdresseDestinations && this.applicationUserContext.filterAdresseDestinations.length > 0) {
      this.filterAdresseDestinations = Array.prototype.map.call(this.applicationUserContext.filterAdresseDestinations, function (item: any) { return item.RefAdresse; }).join(",");
    }
    else {
      this.filterAdresseDestinations = "";
    }
    if (this.applicationUserContext.filterVilleArrivees && this.applicationUserContext.filterVilleArrivees.length > 0) {
      this.filterVilleArrivees = Array.prototype.map.call(this.applicationUserContext.filterVilleArrivees, function (item: any) { return item.Ville; }).join(",");
    }
    else {
      this.filterVilleArrivees = "";
    }
    if (this.applicationUserContext.filterMonths && this.applicationUserContext.filterMonths.length > 0) {
      this.filterMonths = Array.prototype.map.call(this.applicationUserContext.filterMonths, function (item: appInterfaces.Month) { return moment(item.d).format("MM"); }).join(",");
    }
    else {
      this.filterMonths = "";
    }
    if (this.applicationUserContext.filterQuarters && this.applicationUserContext.filterQuarters.length > 0) {
      this.filterQuarters = Array.prototype.map.call(this.applicationUserContext.filterQuarters, function (item: appInterfaces.Quarter) { return item.n.toString(); }).join(",");
    }
    else {
      this.filterQuarters = "";
    }
    if (this.applicationUserContext.filterYears && this.applicationUserContext.filterYears.length > 0) {
      this.filterYears = Array.prototype.map.call(this.applicationUserContext.filterYears, function (item: appInterfaces.Year) { return item.y.toString(); }).join(",");
    }
    else {
      this.filterYears = "";
    }
    if (this.applicationUserContext.filterNonConformiteEtapeTypes && this.applicationUserContext.filterNonConformiteEtapeTypes.length > 0) {
      this.filterNonConformiteEtapeTypes = Array.prototype.map.call(this.applicationUserContext.filterNonConformiteEtapeTypes, function (item: dataModelsInterfaces.NonConformiteEtapeType) { return item.RefNonConformiteEtapeType; }).join(",");
    }
    else {
      this.filterNonConformiteEtapeTypes = "";
    }
    if (this.applicationUserContext.filterNonConformiteNatures && this.applicationUserContext.filterNonConformiteNatures.length > 0) {
      this.filterNonConformiteNatures = Array.prototype.map.call(this.applicationUserContext.filterNonConformiteNatures, function (item: dataModelsInterfaces.NonConformiteNature) { return item.RefNonConformiteNature; }).join(",");
    }
    else {
      this.filterNonConformiteNatures = "";
    }
    if (this.applicationUserContext.filterProduitGroupeReportings && this.applicationUserContext.filterProduitGroupeReportings.length > 0) {
      this.filterProduitGroupeReportings = Array.prototype.map.call(this.applicationUserContext.filterProduitGroupeReportings, function (item: dataModelsInterfaces.ProduitGroupeReporting) { return item.RefProduitGroupeReporting; }).join(",");
    }
    else {
      this.filterProduitGroupeReportings = "";
    }
    if (this.applicationUserContext.filterCamionTypes && this.applicationUserContext.filterCamionTypes.length > 0) {
      this.filterCamionTypes = Array.prototype.map.call(this.applicationUserContext.filterCamionTypes, function (item: dataModelsInterfaces.CamionType) { return item.RefCamionType; }).join(",");
    }
    else {
      this.filterCamionTypes = "";
    }
    if (this.applicationUserContext.filterActionTypes && this.applicationUserContext.filterActionTypes.length > 0) {
      this.filterActionTypes = Array.prototype.map.call(this.applicationUserContext.filterActionTypes, function (item: dataModelsInterfaces.ActionType) { return item.RefActionType; }).join(",");
    }
    else {
      this.filterActionTypes = "";
    }
    if (this.applicationUserContext.filterAdresseTypes && this.applicationUserContext.filterAdresseTypes.length > 0) {
      this.filterAdresseTypes = Array.prototype.map.call(this.applicationUserContext.filterAdresseTypes, function (item: dataModelsInterfaces.AdresseType) { return item.RefAdresseType; }).join(",");
    }
    else {
      this.filterAdresseTypes = "";
    }
    if (this.applicationUserContext.filterContactAdresseProcesss && this.applicationUserContext.filterContactAdresseProcesss.length > 0) {
      this.filterContactAdresseProcesss = Array.prototype.map.call(this.applicationUserContext.filterContactAdresseProcesss, function (item: dataModelsInterfaces.ContactAdresseProcess) { return item.RefContactAdresseProcess; }).join(",");
    }
    else {
      this.filterContactAdresseProcesss = "";
    }
    if (this.applicationUserContext.filterFonctions && this.applicationUserContext.filterFonctions.length > 0) {
      this.filterFonctions = Array.prototype.map.call(this.applicationUserContext.filterFonctions, function (item: dataModelsInterfaces.Fonction) { return item.RefFonction; }).join(",");
    }
    else {
      this.filterFonctions = "";
    }
    if (this.applicationUserContext.filterServices && this.applicationUserContext.filterServices.length > 0) {
      this.filterServices = Array.prototype.map.call(this.applicationUserContext.filterServices, function (item: dataModelsInterfaces.Service) { return item.RefService; }).join(",");
    }
    else {
      this.filterServices = "";
    }
    this.filterEmailType = (this.form.get("EmailTypeList").value ? this.form.get("EmailTypeList").value.Type : "");
    if (this.applicationUserContext.filterContactSelectedColumns && this.applicationUserContext.filterContactSelectedColumns.length > 0) {
      this.filterContactSelectedColumns = Array.prototype.map.call(this.applicationUserContext.filterContactSelectedColumns, function (item: EnvDataColumn) { return item.name; }).join(",");
    }
    else {
      this.filterContactSelectedColumns = "";
    }
    if (this.applicationUserContext.filterUtilisateurs && this.applicationUserContext.filterUtilisateurs.length > 0) {
      this.filterUtilisateurs = Array.prototype.map.call(this.applicationUserContext.filterUtilisateurs, function (item: dataModelsInterfaces.UtilisateurList) { return item.RefUtilisateur; }).join(",");
    }
    else {
      this.filterUtilisateurs = "";
    }
  }
  //-----------------------------------------------------------------------------------
  //Export Excel file
  exportExcel() {
    //Save filters
    this.saveFilter();
    //Set filters
    this.setFilters();
    //Calculate statistique
    if (this.applicationUserContext.currentMenu.name !== MenuName.AnnuaireMenuExtractionAnnuaire) {
      this.calculateStatistics();
    }
    //Show progress bar
    this.progressBar.start();
    //Get Excel file
    this.statistiqueService.exportExcel(this.applicationUserContext.currentModule.name, this.applicationUserContext.currentMenu.name, this.pagin.pageIndex, this.pagin.pageSize
      , this.filterBegin, this.filterEnd, this.applicationUserContext.filterActif
      , this.filterCentreDeTris, this.filterClients, this.filterCollectivites, this.filterDptDeparts, this.filterDptArrivees, this.filterPayss, this.filterProcesss
      , this.filterProduits, this.filterTransporteurs, this.filterVilleDeparts, this.filterAdresseDestinations, this.filterVilleArrivees
      , this.filterDRs, this.filterMonths, this.filterQuarters, this.filterYears, this.filterNonConformiteEtapeTypes, this.filterNonConformiteNatures, this.filterCamionTypes, this.filterProduitGroupeReportings
      , this.applicationUserContext.filterMoins10Euros, this.applicationUserContext.filterCollecte, this.applicationUserContext.filterContrat, this.applicationUserContext.filterEE
      , this.filterEcoOrganismes, this.filterEntiteTypes, this.filterEmailType, this.filterActionTypes, this.filterAdresseTypes, this.filterContactAdresseProcesss, this.filterFonctions, this.filterServices, this.filterContactSelectedColumns
      , this.applicationUserContext.filterFirstLogin, this.applicationUserContext.filterDayWeekMonth, this.filterEntites, this.filterUtilisateurs, this.filterText, this.applicationUserContext.statType
    )
      .pipe(
        finalize(() => this.progressBar.stop())
      )
      .subscribe((data: Response) => {
        this.downloadStatFile(data)
      },
        error => {
          showErrorToUser(this.dialog, error, this.applicationUserContext);
        });
  }
  downloadStatFile(data: any) {
    const blob = new Blob([data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
    {
      const url = window.URL.createObjectURL(blob);
      let fileName = "Stat.xlsx";
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
  //Load data
  loadPage(action: string) {
    this.setFilters();
    this.dataSource.loadItems(this.applicationUserContext.currentModule.name, this.applicationUserContext.currentMenu.name, this.pagin.pageIndex, this.pagin.pageSize
      , this.filterBegin, this.filterEnd, this.applicationUserContext.filterActif
      , this.filterCentreDeTris, this.filterClients, this.filterCollectivites, this.filterDptDeparts, this.filterDptArrivees, this.filterPayss, this.filterProcesss
      , this.filterProduits, this.filterTransporteurs, this.filterVilleDeparts, this.filterAdresseDestinations, this.filterVilleArrivees
      , this.filterDRs, this.filterMonths, this.filterQuarters, this.filterYears, this.filterNonConformiteEtapeTypes, this.filterNonConformiteNatures, this.filterCamionTypes, this.filterProduitGroupeReportings
      , this.applicationUserContext.filterMoins10Euros, this.applicationUserContext.filterCollecte, this.applicationUserContext.filterContrat, this.applicationUserContext.filterEE
      , this.filterEcoOrganismes, this.filterEntiteTypes, this.filterEmailType, this.filterActionTypes, this.filterAdresseTypes, this.filterContactAdresseProcesss, this.filterFonctions, this.filterServices, this.filterContactSelectedColumns
      , this.applicationUserContext.filterFirstLogin, this.applicationUserContext.filterDayWeekMonth, this.filterEntites, this.filterUtilisateurs, this.filterText, this.applicationUserContext.statType
    )
  }
  //-----------------------------------------------------------------------------------
  //Functions for selecting selected items
  compareFnCentreDeTri(item: dataModelsInterfaces.EntiteList, selectedItem: dataModelsInterfaces.EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnClient(item: dataModelsInterfaces.EntiteList, selectedItem: dataModelsInterfaces.EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnCollectivite(item: dataModelsInterfaces.EntiteList, selectedItem: dataModelsInterfaces.EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnDptDepart(item: dataModelsInterfaces.Dpt, selectedItem: dataModelsInterfaces.Dpt) {
    return item && selectedItem ? item.RefDpt === selectedItem.RefDpt : item === selectedItem;
  }
  compareFnDptArrivee(item: dataModelsInterfaces.Dpt, selectedItem: dataModelsInterfaces.Dpt) {
    return item && selectedItem ? item.RefDpt === selectedItem.RefDpt : item === selectedItem;
  }
  compareFnDR(item: dataModelsInterfaces.UtilisateurList, selectedItem: dataModelsInterfaces.UtilisateurList) {
    return item && selectedItem ? item.RefUtilisateur === selectedItem.RefUtilisateur : item === selectedItem;
  }
  compareFnEntite(item: dataModelsInterfaces.Entite, selectedItem: dataModelsInterfaces.Entite) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnEcoOrganisme(item: dataModelsInterfaces.EcoOrganisme, selectedItem: dataModelsInterfaces.EcoOrganisme) {
    return item && selectedItem ? item.RefEcoOrganisme === selectedItem.RefEcoOrganisme : item === selectedItem;
  }
  compareFnEntiteType(item: dataModelsInterfaces.EntiteType, selectedItem: dataModelsInterfaces.EntiteType) {
    return item && selectedItem ? item.RefEntiteType === selectedItem.RefEntiteType : item === selectedItem;
  }
  compareFnPays(item: dataModelsInterfaces.Pays, selectedItem: dataModelsInterfaces.Pays) {
    return item && selectedItem ? item.RefPays === selectedItem.RefPays : item === selectedItem;
  }
  compareFnProcess(item: dataModelsInterfaces.Process, selectedItem: dataModelsInterfaces.Process) {
    return item && selectedItem ? item.RefProcess === selectedItem.RefProcess : item === selectedItem;
  }
  compareFnProduit(item: dataModelsInterfaces.ProduitList, selectedItem: dataModelsInterfaces.ProduitList) {
    return item && selectedItem ? item.RefProduit === selectedItem.RefProduit : item === selectedItem;
  }
  compareFnTransporteur(item: dataModelsInterfaces.EntiteList, selectedItem: dataModelsInterfaces.EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnVilleDepart(item: any, selectedItem: any) {
    return item && selectedItem ? item.Ville === selectedItem.Ville : item === selectedItem;
  }
  compareFnAdresseDestination(item: any, selectedItem: any) {
    return item && selectedItem ? item.Ville === selectedItem.Ville : item === selectedItem;
  }
  compareFnVilleArrivee(item: any, selectedItem: any) {
    return item && selectedItem ? item.Ville === selectedItem.Ville : item === selectedItem;
  }
  compareFnMonth(item: appInterfaces.Month, selectedItem: appInterfaces.Month) {
    return item && selectedItem ? item.d === selectedItem.d : item === selectedItem;
  }
  compareFnQuarter(item: appInterfaces.Quarter, selectedItem: appInterfaces.Quarter) {
    return item && selectedItem ? item.n === selectedItem.n : item === selectedItem;
  }
  compareFnYear(item: appInterfaces.Year, selectedItem: appInterfaces.Year) {
    return item && selectedItem ? item.y === selectedItem.y : item === selectedItem;
  }
  compareFnNonConformiteEtapeType(item: dataModelsInterfaces.NonConformiteEtapeType, selectedItem: dataModelsInterfaces.NonConformiteEtapeType) {
    return item && selectedItem ? item.RefNonConformiteEtapeType === selectedItem.RefNonConformiteEtapeType : item === selectedItem;
  }
  compareFnNonConformiteNature(item: dataModelsInterfaces.NonConformiteNature, selectedItem: dataModelsInterfaces.NonConformiteNature) {
    return item && selectedItem ? item.RefNonConformiteNature === selectedItem.RefNonConformiteNature : item === selectedItem;
  }
  compareFnProduitGroupeReporting(item: dataModelsInterfaces.ProduitGroupeReporting, selectedItem: dataModelsInterfaces.ProduitGroupeReporting) {
    return item && selectedItem ? item.RefProduitGroupeReporting === selectedItem.RefProduitGroupeReporting : item === selectedItem;
  }
  compareFnCamionType(item: dataModelsInterfaces.CamionType, selectedItem: dataModelsInterfaces.CamionType) {
    return item && selectedItem ? item.RefCamionType === selectedItem.RefCamionType : item === selectedItem;
  }
  compareFnActionType(item: dataModelsInterfaces.ActionType, selectedItem: dataModelsInterfaces.ActionType) {
    return item && selectedItem ? item.RefActionType === selectedItem.RefActionType : item === selectedItem;
  }
  compareFnAdresseType(item: dataModelsInterfaces.AdresseType, selectedItem: dataModelsInterfaces.AdresseType) {
    return item && selectedItem ? item.RefAdresseType === selectedItem.RefAdresseType : item === selectedItem;
  }
  compareFnContactAdresseProcess(item: dataModelsInterfaces.ContactAdresseProcess, selectedItem: dataModelsInterfaces.ContactAdresseProcess) {
    return item && selectedItem ? item.RefContactAdresseProcess === selectedItem.RefContactAdresseProcess : item === selectedItem;
  }
  compareFnFonction(item: dataModelsInterfaces.Fonction, selectedItem: dataModelsInterfaces.Fonction) {
    return item && selectedItem ? item.RefFonction === selectedItem.RefFonction : item === selectedItem;
  }
  compareFnService(item: dataModelsInterfaces.Service, selectedItem: dataModelsInterfaces.Service) {
    return item && selectedItem ? item.RefService === selectedItem.RefService : item === selectedItem;
  }
  compareFnUtilisateur(item: dataModelsInterfaces.UtilisateurList, selectedItem: dataModelsInterfaces.UtilisateurList) {
    return item && selectedItem ? item.RefUtilisateur === selectedItem.RefUtilisateur : item === selectedItem;
  }
  //-----------------------------------------------------------------------------------
  //Apply filter
  protected filterCentreDeTrisMulti() {
    if (!this.centreDeTriList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("CentreDeTriListFilter").value;
    if (!search) {
      this.filteredCentreDeTriList.next(this.centreDeTriList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredCentreDeTriList.next(
      this.centreDeTriList.filter(e => removeAccents(e.Libelle.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterCollectivitesMulti() {
    if (!this.collectiviteList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("CollectiviteListFilter").value;
    if (!search) {
      this.filteredCollectiviteList.next(this.collectiviteList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredCollectiviteList.next(
      this.collectiviteList.filter(e => removeAccents(e.Libelle.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterClientsMulti() {
    if (!this.clientList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("ClientListFilter").value;
    if (!search) {
      this.filteredClientList.next(this.clientList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredClientList.next(
      this.clientList.filter(e => removeAccents(e.Libelle.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterProduitsMulti() {
    if (!this.produitList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("ProduitListFilter").value;
    if (!search) {
      this.filteredProduitList.next(this.produitList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredProduitList.next(
      this.produitList.filter(e => removeAccents(e.Libelle.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterVilleDepartsMulti() {
    if (!this.villeDepartList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("VilleDepartListFilter").value;
    if (!search) {
      this.filteredVilleDepartList.next(this.villeDepartList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredVilleDepartList.next(
      this.villeDepartList.filter(e => removeAccents(e.Ville.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterAdresseDestinationsMulti() {
    if (!this.adresseDestinationList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("AdresseDestinationListFilter").value;
    if (!search) {
      this.filteredAdresseDestinationList.next(this.adresseDestinationList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredAdresseDestinationList.next(
      this.adresseDestinationList.filter(e => removeAccents(e.TexteAdresseList.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterVilleArriveesMulti() {
    if (!this.villeArriveeList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("VilleArriveeListFilter").value;
    if (!search) {
      this.filteredVilleArriveeList.next(this.villeArriveeList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredVilleArriveeList.next(
      this.villeArriveeList.filter(e => removeAccents(e.Ville.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterTransporteursMulti() {
    if (!this.transporteurList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("TransporteurListFilter").value;
    if (!search) {
      this.filteredTransporteurList.next(this.transporteurList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredTransporteurList.next(
      this.transporteurList.filter(e => removeAccents(e.Libelle.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterEntitesMulti() {
    if (!this.entiteList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("EntiteListFilter").value;
    if (!search) {
      this.filteredEntiteList.next(this.entiteList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredEntiteList.next(
      this.entiteList.filter(e => removeAccents(e.Libelle.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterUtilisateursMulti() {
    if (!this.utilisateurList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("UtilisateurListFilter").value;
    if (!search) {
      this.filteredUtilisateurList.next(this.utilisateurList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredUtilisateurList.next(
      this.utilisateurList.filter(e => removeAccents(e.Nom.toLowerCase()).indexOf(search) > -1)
    );
  }
  public onResetFilter() {
    this.applicationUserContext.razFilters();
    this.form.get("FilterText").setValue(this.applicationUserContext.filterText);
    this.form.get("DBegin").setValue(this.applicationUserContext.filterBegin.isSame(moment([1, 0, 1, 0, 0, 0, 0])) ? "" : this.applicationUserContext.filterBegin);
    this.form.get("DEnd").setValue(this.applicationUserContext.filterEnd.isSame(moment([1, 0, 1, 0, 0, 0, 0])) ? "" : this.applicationUserContext.filterEnd);
    this.form.get("ClientList").setValue(this.applicationUserContext.filterClients);
    this.form.get("ClientListFilter").setValue(null);
    this.form.get("EntiteList").setValue(this.applicationUserContext.filterEntites);
    this.form.get("EntiteListFilter").setValue(null);
    this.form.get("TransporteurList").setValue(this.applicationUserContext.filterTransporteurs);
    this.form.get("TransporteurListFilter").setValue(null);
    this.form.get("CentreDeTriList").setValue(this.applicationUserContext.filterCentreDeTris);
    this.form.get("CentreDeTriListFilter").setValue(null);
    this.form.get("VilleDepartList").setValue(this.applicationUserContext.filterVilleDeparts);
    this.form.get("VilleDepartListFilter").setValue(null);
    this.form.get("VilleDepartList").setValue(this.applicationUserContext.filterVilleDeparts);
    this.form.get("DptDepartList").setValue(this.applicationUserContext.filterDptDeparts);
    this.form.get("AdresseDestinationList").setValue(this.applicationUserContext.filterAdresseDestinations);
    this.form.get("AdresseDestinationListFilter").setValue(null);
    this.form.get("VilleArriveeList").setValue(this.applicationUserContext.filterVilleArrivees);
    this.form.get("VilleArriveeListFilter").setValue(null);
    this.form.get("DptArriveeList").setValue(this.applicationUserContext.filterDptArrivees);
    this.form.get("EcoOrganismeList").setValue(this.applicationUserContext.filterEcoOrganismes);
    if (this.entiteTypeListMultiple) { this.form.get("EntiteTypeList").setValue(this.applicationUserContext.filterEntiteTypes); }
    else { this.form.get("EntiteTypeList").setValue(this.applicationUserContext.filterEntiteTypes.length > 0 ? this.applicationUserContext.filterEntiteTypes[0] : null); }
    this.form.get("PaysList").setValue(this.applicationUserContext.filterPayss);
    this.form.get("ProcessList").setValue(this.applicationUserContext.filterProcesss);
    this.form.get("ProduitList").setValue(this.applicationUserContext.filterProduits);
    this.form.get("ProduitListFilter").setValue(null);
    this.form.get("DRList").setValue(this.applicationUserContext.filterDRs);
    if (this.collectiviteListMultiple) { this.form.get("CollectiviteList").setValue(this.applicationUserContext.filterCollectivites); }
    else { this.form.get("CollectiviteList").setValue(this.applicationUserContext.filterCollectivites.length > 0 ? this.applicationUserContext.filterCollectivites[0] : null); }
    if (this.monthListMultiple) { this.form.get("MonthList").setValue(this.applicationUserContext.filterMonths); }
    else { this.form.get("MonthList").setValue(this.applicationUserContext.filterMonths.length > 0 ? this.applicationUserContext.filterMonths[0] : null); }
    this.form.get("QuarterList").setValue(this.applicationUserContext.filterQuarters);
    if (this.yearListMultiple) { this.form.get("YearList").setValue(this.applicationUserContext.filterYears); }
    else { this.form.get("YearList").setValue(this.applicationUserContext.filterYears.length > 0 ? this.applicationUserContext.filterYears[0] : null); }
    this.form.get("MonthEndList").setValue(this.applicationUserContext.filterMonthEnd?.n);
    this.form.get("YearEndList").setValue(this.applicationUserContext.filterYearEnd?.y);
    this.form.get("NonConformiteEtapeTypeList").setValue(this.applicationUserContext.filterNonConformiteEtapeTypes);
    this.form.get("NonConformiteNatureList").setValue(this.applicationUserContext.filterNonConformiteNatures);
    this.form.get("ProduitGroupeReportingList").setValue(this.applicationUserContext.filterProduitGroupeReportings);
    this.form.get("CamionTypeList").setValue(this.applicationUserContext.filterCamionTypes);
    this.form.get("ActionTypeList").setValue(this.applicationUserContext.filterActionTypes);
    this.form.get("AdresseTypeList").setValue(this.applicationUserContext.filterAdresseTypes);
    this.form.get("ContactAdresseProcessList").setValue(this.applicationUserContext.filterContactAdresseProcesss);
    this.form.get("FonctionList").setValue(this.applicationUserContext.filterFonctions);
    this.form.get("ServiceList").setValue(this.applicationUserContext.filterServices);
    this.form.get("Moins10Euros").setValue(this.applicationUserContext.filterMoins10Euros);
    this.form.get("Collecte").setValue(this.applicationUserContext.filterCollecte);
    this.form.get("Contrat").setValue(this.applicationUserContext.filterContrat);
    this.form.get("EE").setValue(this.applicationUserContext.filterEE == null ? "" : this.applicationUserContext.filterEE.toString());
    this.form.get("Actif").setValue(this.applicationUserContext.filterActif);
    this.form.get("EmailTypeList").setValue(this.applicationUserContext.filterEmailType);
    this.form.get("UtilisateurList").setValue(this.applicationUserContext.filterUtilisateurs);
    this.form.get("UtilisateurListFilter").setValue(null);
    this.form.get("DayWeekMonth").setValue(this.applicationUserContext.filterDayWeekMonth);
    this.form.get("FirstLogin").setValue(this.applicationUserContext.filterFirstLogin);
    this.contactSelectedColumns = [];
    this.contactAvailableColumns = [];
    this.applicationUserContext.contactAvailableColumns.forEach(val => this.contactAvailableColumns.push(Object.assign({}, val)));
  }
  public onFilterTextReset() {
    this.applicationUserContext.filterText = "";
    this.form.get("FilterText").setValue(this.applicationUserContext.filterText);
  }
  public onDBeginReset() {
    this.applicationUserContext.filterBegin = moment([1, 0, 1, 0, 0, 0, 0]);
    this.form.get("DBegin").setValue(this.applicationUserContext.filterBegin.isSame(moment([1, 0, 1, 0, 0, 0, 0])) ? "" : this.applicationUserContext.filterBegin);
  }
  public onDEndReset() {
    this.applicationUserContext.filterEnd = moment([1, 0, 1, 0, 0, 0, 0]);
    this.form.get("DEnd").setValue(this.applicationUserContext.filterEnd.isSame(moment([1, 0, 1, 0, 0, 0, 0])) ? "" : this.applicationUserContext.filterEnd);
  }
  public onClientListReset() {
    this.applicationUserContext.filterClients = [];
    this.form.get("ClientList").setValue(this.applicationUserContext.filterClients);
  }
  public onCentreDeTriListReset() {
    this.applicationUserContext.filterCentreDeTris = [];
    this.form.get("CentreDeTriList").setValue(this.applicationUserContext.filterCentreDeTris);
  }
  public onCollectiviteListReset() {
    this.applicationUserContext.filterCollectivites = [];
    this.form.get("CollectiviteList").setValue(this.applicationUserContext.filterCollectivites);
  }
  public onEntiteListReset() {
    this.applicationUserContext.filterEntites = [];
    this.form.get("EntiteList").setValue(this.applicationUserContext.filterEntites);
  }
  public onTransporteurListReset() {
    this.applicationUserContext.filterTransporteurs = [];
    this.form.get("TransporteurList").setValue(this.applicationUserContext.filterTransporteurs);
  }
  public onVilleDepartListReset() {
    this.applicationUserContext.filterVilleDeparts = [];
    this.form.get("VilleDepartList").setValue(this.applicationUserContext.filterVilleDeparts);
  }
  public onDptDepartListReset() {
    this.applicationUserContext.filterDptDeparts = [];
    this.form.get("DptDepartList").setValue(this.applicationUserContext.filterDptDeparts);
  }
  public onAdresseDestinationListReset() {
    this.applicationUserContext.filterAdresseDestinations = [];
    this.form.get("AdresseDestinationList").setValue(this.applicationUserContext.filterAdresseDestinations);
  }
  public onVilleArriveeListReset() {
    this.applicationUserContext.filterVilleArrivees = [];
    this.form.get("VilleArriveeList").setValue(this.applicationUserContext.filterVilleArrivees);
  }
  public onDptArriveeListReset() {
    this.applicationUserContext.filterDptArrivees = [];
    this.form.get("DptArriveeList").setValue(this.applicationUserContext.filterDptArrivees);
  }
  public onEcoOrganismeListReset() {
    this.applicationUserContext.filterEcoOrganismes = [];
    this.form.get("EcoOrganismeList").setValue(this.applicationUserContext.filterEcoOrganismes);
    this.contactAvailableColumns = createContactAvailableColumns(this.applicationUserContext.contactAvailableColumns, null, this.dataModelService);
    this.contactSelectedColumns = [];
  }
  public onEntiteTypeListReset() {
    this.applicationUserContext.filterEntiteTypes = [];
    this.form.get("EntiteTypeList").setValue(this.applicationUserContext.filterEntiteTypes);
    this.contactAvailableColumns = createContactAvailableColumns(this.applicationUserContext.contactAvailableColumns, null, this.dataModelService);
    this.contactSelectedColumns = [];
  }
  public onPaysListReset() {
    this.applicationUserContext.filterPayss = [];
    this.form.get("PaysList").setValue(this.applicationUserContext.filterPayss);
  }
  public onProcessListReset() {
    this.applicationUserContext.filterProcesss = [];
    this.form.get("ProcessList").setValue(this.applicationUserContext.filterProcesss);
  }
  public onProduitListReset() {
    this.applicationUserContext.filterProduits = [];
    this.form.get("ProduitList").setValue(this.applicationUserContext.filterProduits);
  }
  public onDRListReset() {
    this.applicationUserContext.filterDRs = [];
    this.form.get("DRList").setValue(this.applicationUserContext.filterDRs);
  }
  public onMonthListReset() {
    this.applicationUserContext.filterMonths = [];
    this.form.get("MonthList").setValue(this.applicationUserContext.filterMonths);
  }
  public onQuarterListReset() {
    this.applicationUserContext.filterQuarters = [];
    this.form.get("QuarterList").setValue(this.applicationUserContext.filterQuarters);
  }
  public onYearListReset() {
    this.applicationUserContext.filterYears = [];
    this.form.get("YearList").setValue(this.applicationUserContext.filterYears);
  }
  public onMonthEndListReset() {
    this.applicationUserContext.filterMonthEnd = null;
    this.form.get("MonthEndList").setValue(this.applicationUserContext.filterMonthEnd);
  }
  public onYearEndListReset() {
    this.applicationUserContext.filterYearEnd = null;
    this.form.get("YearEndList").setValue(this.applicationUserContext.filterYearEnd);
  }
  public onNonConformiteEtapeTypeListReset() {
    this.applicationUserContext.filterNonConformiteEtapeTypes = [];
    this.form.get("NonConformiteEtapeTypeList").setValue(this.applicationUserContext.filterNonConformiteEtapeTypes);
  }
  public onNonConformiteNatureListReset() {
    this.applicationUserContext.filterNonConformiteNatures = [];
    this.form.get("NonConformiteNatureList").setValue(this.applicationUserContext.filterNonConformiteNatures);
  }
  public onProduitGroupeReportingListReset() {
    this.applicationUserContext.filterProduitGroupeReportings = [];
    this.form.get("ProduitGroupeReportingList").setValue(this.applicationUserContext.filterProduitGroupeReportings);
  }
  public onCamionTypeListReset() {
    this.applicationUserContext.filterCamionTypes = [];
    this.form.get("CamionTypeList").setValue(this.applicationUserContext.filterCamionTypes);
  }
  public onActionTypeListReset() {
    this.applicationUserContext.filterActionTypes = [];
    this.form.get("ActionTypeList").setValue(this.applicationUserContext.filterActionTypes);
  }
  public onAdresseTypeListReset() {
    this.applicationUserContext.filterAdresseTypes = [];
    this.form.get("AdresseTypeList").setValue(this.applicationUserContext.filterAdresseTypes);
  }
  public onContactAdresseProcessListReset() {
    this.applicationUserContext.filterContactAdresseProcesss = [];
    this.form.get("ContactAdresseProcessList").setValue(this.applicationUserContext.filterContactAdresseProcesss);
  }
  public onFonctionListReset() {
    this.applicationUserContext.filterFonctions = [];
    this.form.get("FonctionList").setValue(this.applicationUserContext.filterFonctions);
  }
  public onServiceListReset() {
    this.applicationUserContext.filterServices = [];
    this.form.get("ServiceList").setValue(this.applicationUserContext.filterServices);
  }
  public onEmailTypeListReset() {
    this.applicationUserContext.filterEmailType = null;
    this.form.get("EmailTypeList").setValue(this.applicationUserContext.filterEmailType);
  }
  public onUtilisateurListReset() {
    this.applicationUserContext.filterUtilisateurs = [];
    this.form.get("UtilisateurList").setValue(this.applicationUserContext.filterUtilisateurs);
  }
  pickerChosenYearHandler(normalizedYear: Moment, controlName: string) {
    let ctrlValue = this.form.get(controlName).value;
    ctrlValue.year(normalizedYear.year());
    this.form.get(controlName).setValue(ctrlValue);
  }
  pickerChosenMonthHandler(normalizedMonth: Moment, datepicker: MatDatepicker<Moment>, controlName: string) {
    let ctrlValue = this.form.get(controlName).value;
    ctrlValue.month(normalizedMonth.month());
    ctrlValue.date(1);
    if (controlName == "DEnd") {
      ctrlValue = moment(ctrlValue).add(1, "M");
      ctrlValue = moment(ctrlValue).add(-1, "d");
    }
    this.form.get(controlName).setValue(ctrlValue);
    datepicker.close();
  }
  //-----------------------------------------------------------------------------------
  //On drag-drop 
  drop(event: CdkDragDrop<string[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
    }
  }
  //-----------------------------------------------------------------------------------
  //On drag-drop 
  allColumnsToDestination() {
    this.contactAvailableColumns.forEach(item => {
      this.contactSelectedColumns.push(item);
    });
    this.contactAvailableColumns = [];
  }
  //-----------------------------------------------------------------------------------
  //On drag-drop 
  allColumnsToSource() {
    this.contactAvailableColumns = createContactAvailableColumns(this.applicationUserContext.contactAvailableColumns, this.form.get("EntiteTypeList").value, this.dataModelService);
    this.contactSelectedColumns = [];
  }
  //-----------------------------------------------------------------------------------
  //On EntiteType selected
  onStatistiqueCollectiviteSelected() {
    this.calculateStatistics();
    this.manageScreen();
    //Limit Produit list to those associated with Collectivite
    this.getListProduit();
    //Get existing year
    this.listService.getListYear().subscribe(result => {
      this.yearList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //On EntiteType selected
  onEntiteTypeSelected() {
    if (!this.entiteTypeListMultiple) {
      //Init available columns for data extract
      this.contactAvailableColumns = createContactAvailableColumns(this.applicationUserContext.contactAvailableColumns, this.form.get("EntiteTypeList").value, this.dataModelService);
    }
    else {
      //Init available columns for data extract
      this.contactAvailableColumns = createContactAvailableColumns(this.applicationUserContext.contactAvailableColumns, null, this.dataModelService);
    }
    this.contactSelectedColumns = [];
  }
  //-----------------------------------------------------------------------------------
  //On Collectivite selected
  onCollectiviteSelected() {
    //Reset Produit filter

    //Get existing Produit
    let refCollectivite: number = null;
    if (this.applicationUserContext.currentMenu.name == MenuName.AnnuaireMenuEvolutionTonnage) {
      if (this.form.get("CollectiviteList").value) {
        refCollectivite = this.form.get("CollectiviteList").value.RefEntite;
      }
      else { refCollectivite = 0; }
    }
    this.listService.getListProduit(null, null, null, null, null, null, refCollectivite, false, false).subscribe(result => {
      this.produitList = result;
      this.filteredProduitList.next(this.produitList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //On filter Collecte chip
  onChipChange($event: any) {
    //Save filters
    this.saveFilter();
    //Save specific chip filter
    if (!$event.selected) {
      $event.source.selected = true;
    }
    else if (this.applicationUserContext.filterCollecte != $event.source.value) {
      this.applicationUserContext.filterCollecte = $event.source.value;
      this.form.get("Collecte").setValue(this.applicationUserContext.filterCollecte);
    }
    //Set filters
    this.setFilters();
    //Reset pager
    this.pagin.pageIndex = 0
    //Load data
    this.loadPage("");
  }
  //-----------------------------------------------------------------------------------
  //Dowmload recyclers map
  onGetRecyclersMap() {
    this.downloadService.download(DocumentType.Parametre, "19", "", null, "")
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
  //Styling
  setStyle(item: any, column: any): string {
    let classes: string = "ev-cell";
    //General styling
    switch (this.applicationUserContext.currentMenu.name) {
      case MenuName.LogistiqueMenuExtractionReception:
        if (column.columnDef === DataColumnName.PUHTUnique) {
          if (item.PUHTUnique == 0) { classes += " background-color-anomalie"; }
        }
        break;
    }
    return classes;
  }
}
//-----------------------------------------------------------------------------------
//Datasource class
export class StatistiqueDataSource implements DataSource<any> {
  //Subjects
  private itemsSubject = new BehaviorSubject<any[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();
  public nbRow: number = 0; //Pager length
  public items: any[];
  public columns: MatTableColumn[] = [];
  public displayedColumns: string[] = [];
  //Statistics
  categoryField: string = "";
  categoryAxis: any = {};
  chartSeries: appInterfaces.ChartSerie[] = [];
  statisticAxisTitle: any = {
    text: ""
  };
  zoomable: any | boolean = { mousewheel: { lock: 'y' } };
  legend: boolean = false;
  //Functions
  pointColor = pointColor;
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private statistiqueService: StatistiqueService
    , private snackBarQueueService: SnackBarQueueService
    , public applicationUserContext: ApplicationUserContext
  ) { }
  //Connect to table
  connect(collectionViewer: CollectionViewer): Observable<any[]> {
    return this.itemsSubject.asObservable();
  }
  //-----------------------------------------------------------------------------------
  //Disconnect from table
  disconnect(collectionViewer: CollectionViewer): void {
    this.itemsSubject.complete();
    this.loadingSubject.complete();
  }
  //-----------------------------------------------------------------------------------
  //Chart labels
  labelContent = (e: any) => {
    let f: string = "";
    switch (this.applicationUserContext.currentMenu.name) {
      case MenuName.AdministrationMenuSuiviLogin:
        f = e.value.toString();
        break;
      case MenuName.AnnuaireMenuEvolutionTonnage:
        f = e.value.toLocaleString(this.applicationUserContext.currentCultureName,
          { minimumFractionDigits: 3 }) + " t";
        break;
      case MenuName.AnnuaireMenuNbAffretementTransporteur:
        f = e.value.toString();
        break;
      case MenuName.LogistiqueMenuDelaiCommandeEnlevement:
        f = e.value.toString();
        break;
      case MenuName.ModuleCollectiviteMenuStatistiques:
        switch (this.applicationUserContext.statType) {
          case "DestinationTonnage":
            f = e.value.toLocaleString(this.applicationUserContext.currentCultureName,
              { minimumFractionDigits: 2 }) + "%";
            break;
          case "DeboucheBalle":
            f = e.category;
            //f = e.category
            //  + " (" + e.value.toLocaleString(this.applicationUserContext.currentCultureName,
            //    { minimumFractionDigits: 2 }) + "%)";
            break;
          case "EvolutionTonnage":
            f = e.value.toLocaleString(this.applicationUserContext.currentCultureName,
              { minimumFractionDigits: 3 }) + " t";
            break;
        }
        break;
    }
    return f;
  }
  //-----------------------------------------------------------------------------------
  //Load data
  loadItems(module = "", menu = "", pageIndex = 0, pageSize = 25
    , filterBegin = "", filterEnd = "", filterActif = false
    , filterCentreDeTris = "", filterClients = "", filterCollectivites = "", filterDptDeparts = "", filterDptArrivees = "", filterPayss = "", filterProcesss = ""
    , filterProduits = "", filterTransporteurs = "", filterVilleDeparts = "", filterAdresseDestinations = "", filterVilleArrivees = ""
    , filterDRs = "", filterMonths = "", filterQuarters = "", filterYears = "", filterNonConformiteEtapeTypes = "", filterNonConformiteNatures = "", filterCamionTypes = "", filterProduitGroupeReportings = ""
    , filterMoins10Euros = false, filterCollecte = "", filterContrat = "", filterEE = null, filterEcoOrganismes = "", filterEntiteTypes = "", filterEmailType = "", filterActionTypes = ""
    , filterAdresseTypes = "", filterContactAdresseProcesss = "", filterFonctions = "", filterServices = "", filterContactSelectedColumns = ""
    , filterFirstLogin = false, filterDayWeekMonth = "Month", filterEntites = "", filterUtilisateurs = "", filterText = "", statType = ""
  ) {
    //Show loading indicator
    this.loadingSubject.next(true);
    //Get data and hide loading indicator
    this.statistiqueService.getStatistique(module, menu, pageIndex, pageSize
      , filterBegin, filterEnd, filterActif
      , filterCentreDeTris, filterClients, filterCollectivites, filterDptDeparts, filterDptArrivees, filterPayss, filterProcesss
      , filterProduits, filterTransporteurs, filterVilleDeparts, filterAdresseDestinations, filterVilleArrivees
      , filterDRs, filterMonths, filterQuarters, filterYears, filterNonConformiteEtapeTypes, filterNonConformiteNatures, filterCamionTypes, filterProduitGroupeReportings
      , filterMoins10Euros, filterCollecte, filterContrat, filterEE, filterEcoOrganismes, filterEntiteTypes, filterEmailType, filterActionTypes
      , filterAdresseTypes, filterContactAdresseProcesss, filterFonctions, filterServices, filterContactSelectedColumns
      , filterFirstLogin, filterDayWeekMonth, filterEntites, filterUtilisateurs, filterText, statType).pipe<any, HttpResponse<any[]>>(
        catchError(() => of([])),
        finalize(() => this.loadingSubject.next(false))
      )
      .subscribe((resp: HttpResponse<any[]>) => {
        let n: number = parseInt(resp.headers.get("nbRow"), 10);
        if (!isNaN(n)) { this.nbRow = n };
        //Manage columns if data is present
        if (resp.body.length > 0) {
          let columnsTmp: MatTableColumn[] = [];
          //Get item properties
          let listPropertyNames = Object.keys(resp.body[0]);
          //Create columns if data is present and colums collection changed
          let refreshColumns: boolean = false;
          //Create columns
          for (let prop of listPropertyNames) {
            let col: MatTableColumn = createMatTableColumn(prop, this.applicationUserContext);
            if (col !== null) { columnsTmp.push(col); }
          }
          //Check changes in columns collection
          if (this.columns.length !== columnsTmp.length) { refreshColumns = true; }
          else {
            for (let c in this.columns) {
              if (this.columns[c].columnDef != columnsTmp[c].columnDef) { refreshColumns = true; }
            }
          }
          //Refresh columns if needed
          if (refreshColumns) {
            this.columns = columnsTmp;
            this.displayedColumns = this.columns.map(c => c.columnDef);
          }
        }
        else {
          //clear existing columns and inform user
          this.columns = [];
          this.displayedColumns = this.columns.map(c => c.columnDef);
          this.snackBarQueueService.addMessage(<appInterfaces.SnackbarMsg>{ text: this.applicationUserContext.getCulturedRessourceText(318), duration: 4000 });
        }
        //Load data
        this.itemsSubject.next(resp.body);
        this.items = resp.body;
        if (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient
          || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient
        ) {
          this.snackBarQueueService.addMessage(<appInterfaces.SnackbarMsg>{ text: this.applicationUserContext.getCulturedRessourceText(1056), duration: 6000 });
        }
        //Statistics charts
        //Init
        this.chartSeries = [];
        this.categoryField = "";
        this.categoryAxis = {
          categories: "",
          labels: { rotation: "auto" }
        }
        this.statisticAxisTitle = {
          text: ""
        };
        this.legend = false;
        //process
        if (resp.body.length > 0) {
          if (this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuSuiviLogin) {
            //Init parameters
            this.categoryField = DataColumnName.Periode;
            this.categoryAxis = {
              categories: this.categoryField,
              labels: { rotation: "auto" }
            }
            this.statisticAxisTitle = {
              text: this.applicationUserContext.envDataColumns.find(x => x.name === DataColumnName.Nb).culturedCaption
            };
            //Get item properties
            let listPropertyNames = Object.keys(resp.body[0]);
            //Create series
            for (let prop of listPropertyNames) {
              if (prop != this.categoryField) {
                let serie: appInterfaces.ChartSerie = {
                  type: "line",
                  data: resp.body,
                  field: prop,
                  categoryField: this.categoryField,
                  name: prop
                };
                this.chartSeries.push(serie);
              }
            }
            this.legend = this.chartSeries?.length > 1 || (this.chartSeries?.length == 1 && this.chartSeries[0]?.field != DataColumnName.Nb);
          }
          if (this.applicationUserContext.currentMenu.name === MenuName.AnnuaireMenuNbAffretementTransporteur) {
            //Init parameters
            this.categoryField = DataColumnName.MoisAnnee;
            this.categoryAxis = {
              categories: this.categoryField,
              labels: { rotation: "auto" }
            }
            this.statisticAxisTitle = {
              text: this.applicationUserContext.envDataColumns.find(x => x.name === DataColumnName.NbAffretement).culturedCaption
            };
            //Get item properties
            let listPropertyNames = Object.keys(resp.body[0]);
            //Create series
            for (let prop of listPropertyNames) {
              if (prop != this.categoryField) {
                let serie: appInterfaces.ChartSerie = {
                  type: "column",
                  data: resp.body,
                  field: prop,
                  categoryField: this.categoryField,
                  name: prop
                };
                this.chartSeries.push(serie);
              }
            }
            this.legend = this.chartSeries?.length > 1 || (this.chartSeries?.length == 1 && this.chartSeries[0]?.field != DataColumnName.NbAffretement);
          }
          if (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuDelaiCommandeEnlevement) {
            //Init parameters
            this.categoryField = DataColumnName.CommandeFournisseurNumeroCommande;
            this.categoryAxis = {
              categories: this.categoryField,
              labels: { rotation: "auto" }
            }
            this.statisticAxisTitle = {
              text: this.applicationUserContext.getCulturedRessourceText(1429)
            };
            //Create series
            let serie: appInterfaces.ChartSerie = {
              type: "column",
              data: resp.body.filter(e => e.RefCommandeFournisseur != null),
              field: DataColumnName.CommandeFournisseurDelaiCommande,
              categoryField: this.categoryField,
              name: this.applicationUserContext.envDataColumns.find(x => x.name === DataColumnName.CommandeFournisseurDelaiCommande).culturedCaption
            };
            this.chartSeries.push(serie);
            serie = {
              type: "column",
              data: resp.body.filter(e => e.RefCommandeFournisseur != null),
              field: DataColumnName.CommandeFournisseurDelaiChargement,
              categoryField: this.categoryField,
              name: this.applicationUserContext.envDataColumns.find(x => x.name === DataColumnName.CommandeFournisseurDelaiChargement).culturedCaption
            };
            this.chartSeries.push(serie);
            //Legend
            this.legend = true;
          }
          if (this.applicationUserContext.currentMenu.name === MenuName.AnnuaireMenuEvolutionTonnage
            || (this.applicationUserContext.currentMenu.name === MenuName.ModuleCollectiviteMenuStatistiques
              && this.applicationUserContext.statType == "EvolutionTonnage")) {
            //Init parameters
            this.categoryField = DataColumnName.MoisAnnee;
            this.categoryAxis = {
              categories: this.categoryField,
              labels: { rotation: "auto" }
            }
            this.statisticAxisTitle = {
              text: this.applicationUserContext.envDataColumns.find(x => x.name === DataColumnName.PoidsTonne).culturedCaption
            };
            this.zoomable = { mousewheel: { lock: 'y' } };;
            //Get item properties
            let listPropertyNames = Object.keys(resp.body[0]);
            //Create series
            for (let prop of listPropertyNames) {
              if (prop != this.categoryField) {
                let serie: appInterfaces.ChartSerie = {
                  type: "column",
                  data: resp.body,
                  field: prop,
                  categoryField: this.categoryField,
                  name: prop
                };
                this.chartSeries.push(serie);
              }
            }
            this.legend = this.chartSeries?.length > 1 || (this.chartSeries?.length == 1 && this.chartSeries[0]?.field != DataColumnName.PoidsTonne);
          }
          if (this.applicationUserContext.currentMenu.name === MenuName.ModuleCollectiviteMenuStatistiques
            && this.applicationUserContext.statType == "DestinationTonnage") {
            //Init parameters
            this.categoryField = DataColumnName.ClientLibelle;
            this.categoryAxis = {
              categories: this.categoryField,
              //labels: { rotation: -45 }
            }
            this.statisticAxisTitle = {
              text: this.applicationUserContext.envDataColumns.find(x => x.name === DataColumnName.Pourcentage).culturedCaption
            };
            this.zoomable = false;
            //Get item properties
            let listPropertyNames = Object.keys(resp.body[0]);
            //Create series
            for (let prop of listPropertyNames) {
              if (prop != this.categoryField) {
                let serie: appInterfaces.ChartSerie = {
                  type: "bar",
                  data: resp.body,
                  field: prop,
                  categoryField: this.categoryField,
                  name: prop,
                  color: this.pointColor,
                };
                this.chartSeries.push(serie);
              }
            }
            this.legend = false;
          }
          if (this.applicationUserContext.currentMenu.name === MenuName.ModuleCollectiviteMenuStatistiques
            && this.applicationUserContext.statType == "DeboucheBalle") {
            //Init parameters
            this.categoryField = DataColumnName.ApplicationLibelle;
            this.categoryAxis = {
              categories: this.categoryField,
              labels: { rotation: "auto" }
            }
            this.statisticAxisTitle = {
              text: this.applicationUserContext.envDataColumns.find(x => x.name === DataColumnName.Pourcentage).culturedCaption
            };
            this.zoomable = false;
            //Get item properties
            let listPropertyNames = Object.keys(resp.body[0]);
            //Create series
            for (let prop of listPropertyNames) {
              if (prop != this.categoryField) {
                let serie: appInterfaces.ChartSerie = {
                  type: "donut",
                  data: resp.body,
                  field: prop,
                  categoryField: this.categoryField,
                  name: prop, 
                };
                this.chartSeries.push(serie);
              }
            }
            this.legend = false;
          }
        }
      });
  }
}
