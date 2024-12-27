import { Component, OnInit, AfterViewInit, ViewChild, Inject, OnDestroy } from "@angular/core";
import { Router, NavigationEnd } from "@angular/router";
import { HttpClient, HttpResponse, HttpHeaders } from "@angular/common/http";
import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { Observable, of, BehaviorSubject, ReplaySubject, Subject } from "rxjs";
import { catchError, finalize, tap, takeUntil } from "rxjs/operators";
import { GridService } from "../../../services/grid.service";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { ApplicationUserContext } from "../../../globals/globals";
import { UntypedFormGroup, UntypedFormControl, Validators } from "@angular/forms";
import { SelectionModel } from "@angular/cdk/collections";
import { DataColumnName, MenuName, HabilitationLogistique, ActionName, EnvCommandeFournisseurStatutName, ModuleName, HabilitationQualite, SearchElementType } from "../../../globals/enums";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import moment from "moment";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import * as appClasses from "../../../classes/appClasses";
import { showErrorToUser, createMatTableColumn, removeAccents, shortenLongText, GetUtilisateurTypeLocalizedLabel } from "../../../globals/utils";
import { MatTableColumn } from '../../../interfaces/appInterfaces';
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import { UtilisateurList } from "../../../interfaces/dataModelsInterfaces";
import { EnvComponent } from "../../../classes/appClasses";

@Component({
    selector: "grid",
    templateUrl: "./grid.component.html",
    standalone: false
})
//-----------------------------------------------------------------------------------
//Component class
export class GridComponent implements AfterViewInit, OnInit, OnDestroy {
  //Form
  form: UntypedFormGroup;
  formAugmentation: UntypedFormGroup;
  formPrixRepriseCopy: UntypedFormGroup;
  //Filters
  filterText: string = "";
  filterBegin: string = "";
  filterApplicationProduitOrigines: string = "";
  filterCentreDeTris: string = "";
  filterClients: string = "";
  filterCamionTypes: string = "";
  filterComposants: string = "";
  filterDRs: string = "";
  filterUtilisateurMaitres: string = "";
  filterDptDeparts: string = "";
  filterDptArrivees: string = "";
  filterEntiteTypes: string = "";
  filterDonneeEntiteSage: boolean = false;
  filterEcoOrganismes: string = "";
  filterEnd: string = "";
  filterEnvCommandeFournisseurStatuts: string = "";
  filterIndustriels: string = "";
  filterMessageTypes: string = "";
  filterMonths: string = "";
  filterNonConformiteEtapeTypes: string = "";
  filterNonConformiteNatures: string = "";
  filterPayss: string = "";
  filterProcesss: string = "";
  filterRegionReportings: string = "";
  filterProduits: string = "";
  filterTransporteurs: string = "";
  filterPrestataires: string = "";
  filterVilleArrivees: string = "";
  filterVilleDeparts: string = "";
  filterYears: string = "";
  //Data
  selectedItem: string;
  moisDechargementPrevuItem: string;
  item: any;
  applicationProduitOrigineList: dataModelsInterfaces.ApplicationProduitOrigine[];
  centreDeTriList: dataModelsInterfaces.EntiteList[];
  filteredCentreDeTriList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  camionTypeList: dataModelsInterfaces.CamionType[];
  clientList: dataModelsInterfaces.EntiteList[];
  filteredClientList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  composantList: dataModelsInterfaces.ProduitList[];
  dRList: dataModelsInterfaces.UtilisateurList[];
  utilisateurMaitreList: dataModelsInterfaces.UtilisateurList[];
  filteredUtilisateurMaitreList: ReplaySubject<dataModelsInterfaces.UtilisateurList[]> = new ReplaySubject<dataModelsInterfaces.UtilisateurList[]>(1);
  dptDepartList: dataModelsInterfaces.Dpt[];
  dptArriveeList: dataModelsInterfaces.Dpt[];
  entiteTypeList: dataModelsInterfaces.EntiteType[];
  ecoOrganismeList: dataModelsInterfaces.EcoOrganisme[];
  industrielList: dataModelsInterfaces.EntiteList[];
  messageTypeList: dataModelsInterfaces.MessageType[];
  monthList: appInterfaces.Month[];
  monthListFrom: appInterfaces.Month[];
  monthListTo: appInterfaces.Month[];
  nonConformiteEtapeTypeList: dataModelsInterfaces.NonConformiteEtapeType[];
  nonConformiteNatureList: dataModelsInterfaces.NonConformiteNature[];
  paysList: dataModelsInterfaces.Pays[];
  processList: dataModelsInterfaces.Process[];
  regionReportingList: dataModelsInterfaces.RegionReporting[];
  produitList: dataModelsInterfaces.ProduitList[];
  filteredProduitList: ReplaySubject<dataModelsInterfaces.ProduitList[]> = new ReplaySubject<dataModelsInterfaces.ProduitList[]>(1);
  transporteurList: dataModelsInterfaces.EntiteList[];
  filteredTransporteurList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  prestataireList: dataModelsInterfaces.EntiteList[];
  filteredPrestataireList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  villeDepartList: any[];
  filteredVilleDepartList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
  villeArriveeList: any[];
  filteredVilleArriveeList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
  yearList: appInterfaces.Year[];
  yearListFrom: appInterfaces.Year[];
  yearListTo: appInterfaces.Year[];Fetap
  dataSource: GridDataSource;
  selection = new SelectionModel<any>(true, []);
  moisDechargementPrevuItems: any[] = [];
  @ViewChild(MatPaginator, { static: true }) pagin: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  //Dynamic labels
  labelMonthListFC: string = this.applicationUserContext.getCulturedRessourceText(571);
  labelYearListFC: string = this.applicationUserContext.getCulturedRessourceText(569);
  labelDBeginFC: string = this.applicationUserContext.getCulturedRessourceText(212);
  labelDFinFC: string = this.applicationUserContext.getCulturedRessourceText(213);
  //Visibiliry marks
  moisDechargementPrevuVisible: boolean = false;
  enAttenteVisible: boolean = false;
  deactivateUtilisateurVisible: boolean = false;
  //Filters visibility marks
  visibleFilterActif: boolean = false;
  visibleFilterNonRepartissable: boolean = false;
  visibleFilterAnomalieCommandeFournisseur: boolean = false;
  visibleFilterCollecte: boolean = false;
  visibleFilterDBegin: boolean = false;
  visibleFilterDatesTransporteur: boolean = false;
  visibleFilterDChargementModif: boolean = false;
  visibleFilterDEnd: boolean = false;
  visibleFilterCamionTypeList: boolean = false;
  visibleFilterApplicationProduitOrigineList = false;
  visibleFilterCentreDeTriList: boolean = false;
  visibleFilterClientList: boolean = false;
  visibleFilterComposantList: boolean = false;
  visibleFilterDRList: boolean = false;
  visibleFilterUtilisateurMaitreList: boolean = false;
  visibleFilterDptArriveeList: boolean = false;
  visibleFilterDptDepartList: boolean = false;
  visibleFilterEntiteTypeList: boolean = false;
  visibleFilterDonneeEntiteSage: boolean = false;
  visibleFilterEcoOrganismeList: boolean = false;
  visibleFilterEnvCommandeFournisseurStatutList: boolean = false;
  visibleFilterFilterText: boolean = false;
  visibleFilterIndustrielList: boolean = false;
  visibleFilterMonthList: boolean = false;
  visibleFilterNonConformiteEtapeTypeList: boolean = false;
  visibleFilterNonConformiteNatureList: boolean = false;
  visibleFilterIFFournisseur: boolean = false;
  visibleFilterIFClient: boolean = false;
  visibleFilterIFFournisseurRetourLot: boolean = false;
  visibleFilterIFFournisseurFacture: boolean = false;
  visibleFilterIFFournisseurAttenteBonCommande: boolean = false;
  visibleFilterIFFournisseurTransmissionFacturation: boolean = false;
  visibleFilterNonConformitesATraiter: boolean = false;
  visibleFilterNonConformitesATransmettre: boolean = false;
  visibleFilterNonConformitesPlanActionAValider: boolean = false;
  visibleFilterNonConformitesPlanActionNR: boolean = false;
  visibleFilterMessageTypeList: boolean = false;
  visibleFilterPaysList: boolean = false;
  visibleFilterProcessList: boolean = false;
  visibleFilterRegionReportingList: boolean = false;
  visibleFilterProduitList: boolean = false;
  visibleFilterControle: boolean = false;
  visibleFilterTransporteurList: boolean = false;
  visibleFilterPrestataireList: boolean = false;
  visibleFilterVilleDepartList: boolean = false;
  visibleFilterVilleArriveeList: boolean = false;
  visibleFilterCommandesFournisseurNonChargees: boolean = false;
  visibleFilterCommandesFournisseurAttribuees: boolean = false;
  visibleFilterValideDPrevues: boolean = false;
  visibleFilterYearList: boolean = false;
  //Misc
  refMessageVisualisations: number = 0;
  refTransportConcurrence: number = 0;
  //Functions
  shortenLongText = shortenLongText;
  // Subject that emits when the component has been destroyed.
  protected _onDestroy = new Subject<void>();
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string
    , private router: Router
    , private gridService: GridService
    , private listService: ListService
    , public applicationUserContext: ApplicationUserContext
    , private snackBarQueueService: SnackBarQueueService
    , private dataModelService: DataModelService
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
    //Set moment culture
    moment.locale(this.applicationUserContext.currentCultureName.substring(0, 2));
    //Set dynamic labels
    this.setDynamicLabels();
    //Creates forms
    this.form = new UntypedFormGroup({
      FilterText: new UntypedFormControl(this.applicationUserContext.filterText),
      DBegin: new UntypedFormControl(this.applicationUserContext.filterBegin.toISOString() === moment([1, 0, 1, 0, 0, 0, 0]).toISOString() ? "" : this.applicationUserContext.filterBegin),
      DEnd: new UntypedFormControl(this.applicationUserContext.filterEnd.toISOString() === moment([1, 0, 1, 0, 0, 0, 0]).toISOString() ? "" : this.applicationUserContext.filterEnd),
      ApplicationProduitOrigineList: new UntypedFormControl(this.applicationUserContext.filterApplicationProduitOrigines),
      ClientList: new UntypedFormControl(this.applicationUserContext.filterClients),
      ClientListFilter: new UntypedFormControl(),
      TransporteurList: new UntypedFormControl(this.applicationUserContext.filterTransporteurs),
      TransporteurListFilter: new UntypedFormControl(),
      PrestataireList: new UntypedFormControl(this.applicationUserContext.filterPrestataires),
      PrestataireListFilter: new UntypedFormControl(),
      CentreDeTriList: new UntypedFormControl(this.applicationUserContext.filterCentreDeTris),
      CentreDeTriListFilter: new UntypedFormControl(),
      IndustrielList: new UntypedFormControl(this.applicationUserContext.filterIndustriels),
      DptDepartList: new UntypedFormControl(this.applicationUserContext.filterDptDeparts),
      DptArriveeList: new UntypedFormControl(this.applicationUserContext.filterDptArrivees),
      EntiteTypeList: new UntypedFormControl(this.applicationUserContext.filterEntiteTypes),
      DonneeEntiteSage: new UntypedFormControl(this.applicationUserContext.filterDonneeEntiteSage),
      EcoOrganismeList: new UntypedFormControl(this.applicationUserContext.filterEcoOrganismes),
      VilleDepartList: new UntypedFormControl(this.applicationUserContext.filterVilleDeparts),
      VilleDepartListFilter: new UntypedFormControl(),
      VilleArriveeList: new UntypedFormControl(this.applicationUserContext.filterVilleArrivees),
      VilleArriveeListFilter: new UntypedFormControl(),
      PaysList: new UntypedFormControl(this.applicationUserContext.filterPayss),
      ProcessList: new UntypedFormControl(this.applicationUserContext.filterProcesss),
      RegionReportingList: new UntypedFormControl(this.applicationUserContext.filterRegionReportings),
      ProduitList: new UntypedFormControl(this.applicationUserContext.filterProduits),
      ProduitListFilter: new UntypedFormControl(),
      Controle: new UntypedFormControl(this.applicationUserContext.filterControle),
      ComposantList: new UntypedFormControl(this.applicationUserContext.filterComposants),
      DRList: new UntypedFormControl(this.applicationUserContext.filterDRs),
      UtilisateurMaitreList: new UntypedFormControl(this.applicationUserContext.filterUtilisateurMaitres),
      UtilisateurMaitreListFilter: new UntypedFormControl(this.applicationUserContext.filterUtilisateurMaitres),
      MessageTypeList: new UntypedFormControl(this.applicationUserContext.filterMessageTypes),
      MonthList: new UntypedFormControl(this.applicationUserContext.filterMonths),
      NonConformiteEtapeTypeList: new UntypedFormControl(this.applicationUserContext.filterNonConformiteEtapeTypes),
      NonConformiteNatureList: new UntypedFormControl(this.applicationUserContext.filterNonConformiteNatures),
      IFFournisseur: new UntypedFormControl(this.applicationUserContext.filterIFFournisseur),
      IFClient: new UntypedFormControl(this.applicationUserContext.filterIFClient),
      IFFournisseurRetourLot: new UntypedFormControl(this.applicationUserContext.filterIFFournisseurRetourLot),
      IFFournisseurAttenteBonCommande: new UntypedFormControl(this.applicationUserContext.filterIFFournisseurAttenteBonCommande),
      IFFournisseurFacture: new UntypedFormControl(this.applicationUserContext.filterIFFournisseurFacture),
      IFFournisseurTransmissionFacturation: new UntypedFormControl(this.applicationUserContext.filterIFFournisseurTransmissionFacturation),
      NonConformitesATraiter: new UntypedFormControl(this.applicationUserContext.filterNonConformitesATraiter),
      NonConformitesATransmettre: new UntypedFormControl(this.applicationUserContext.filterNonConformitesATransmettre),
      NonConformitesPlanActionAValider: new UntypedFormControl(this.applicationUserContext.filterNonConformitesPlanActionAValider),
      NonConformitesPlanActionNR: new UntypedFormControl(this.applicationUserContext.filterNonConformitesPlanActionNR),
      YearList: new UntypedFormControl(this.applicationUserContext.filterYears),
      CamionTypeList: new UntypedFormControl(this.applicationUserContext.filterCamionTypes),
      EnvCommandeFournisseurStatutList: new UntypedFormControl(this.applicationUserContext.filterEnvCommandeFournisseurStatuts),
      CommandesFournisseurNonChargees: new UntypedFormControl(this.applicationUserContext.filterCommandesFournisseurNonChargees),
      CommandesFournisseurAttribuees: new UntypedFormControl(this.applicationUserContext.filterCommandesFournisseurAttribuees),
      ValideDPrevues: new UntypedFormControl(this.applicationUserContext.filterValideDPrevues),
      DChargementModif: new UntypedFormControl(this.applicationUserContext.filterDChargementModif),
      DatesTransporteur: new UntypedFormControl(this.applicationUserContext.filterDatesTransporteur),
      AnomalieCommandeFournisseur: new UntypedFormControl(this.applicationUserContext.filterAnomalieCommandeFournisseur),
      Actif: new UntypedFormControl(this.applicationUserContext.filterActif),
      Collecte: new UntypedFormControl(this.applicationUserContext.filterCollecte),
      NonRepartissable: new UntypedFormControl(
        this.applicationUserContext.filterNonRepartissable ? "1" : (this.applicationUserContext.filterNonRepartissable == false ? "0" : ""))
    });
    this.formAugmentation = new UntypedFormGroup({
      Augmentation: new UntypedFormControl("", Validators.required)
    });
    this.formPrixRepriseCopy = new UntypedFormGroup({
      YearListFrom: new UntypedFormControl(null, Validators.required),
      MonthListFrom: new UntypedFormControl(null, Validators.required),
      YearListTo: new UntypedFormControl(null, Validators.required),
      MonthListTo: new UntypedFormControl(null, Validators.required)
    });
    //Get existing client
    listService.getListClient(null, null, null, null, false).subscribe(result => {
      this.clientList = result;
      this.filteredClientList.next(this.clientList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing transporteur
    listService.getListTransporteur(null, false, false).subscribe(result => {
      this.transporteurList = result;
      this.filteredTransporteurList.next(this.transporteurList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing prestataire
    listService.getListPrestataire(null, false).subscribe(result => {
      this.prestataireList = result;
      this.filteredPrestataireList.next(this.prestataireList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing centre de tri
    listService.getListCentreDeTri(null, null, false).subscribe(result => {
      this.centreDeTriList = result;
      this.filteredCentreDeTriList.next(this.centreDeTriList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Industriel
    listService.getListIndustriel(null, false).subscribe(result => {
      this.industrielList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Dpt départ
    listService.getListDpt().subscribe(result => {
      this.dptDepartList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Dpt arrivée
    listService.getListDpt().subscribe(result => {
      this.dptArriveeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ville départ
    listService.getListVilleDepart().subscribe(result => {
      this.villeDepartList = result;
      this.filteredVilleDepartList.next(this.villeDepartList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ville arrivée
    listService.getListVilleArrivee().subscribe(result => {
      this.villeArriveeList = result;
      this.filteredVilleArriveeList.next(this.villeArriveeList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Pays
    listService.getListPays(null, false).subscribe(result => {
      this.paysList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Process
    listService.getListProcess(null, null).subscribe(result => {
      this.processList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing RegionReporting
    listService.getListRegionReporting().subscribe(result => {
      this.regionReportingList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Produit
    this.getListProduit();
    //Get existing Composant
    listService.getListProduit(null, null, 0, null, null, null, null, false, false).subscribe(result => {
      this.composantList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing DR
    listService.getListUtilisateur(null, ModuleName.Qualite, HabilitationQualite.Utilisateur, null, false, false, false, false).subscribe(result => {
      this.dRList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Maitre
    listService.getListUtilisateur(null, null, null, null, false, false, true, false).subscribe(result => {
      this.utilisateurMaitreList = result;
      this.filteredUtilisateurMaitreList.next(this.utilisateurMaitreList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing month
    listService.getListMonth().subscribe(result => {
      this.monthList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing year
    listService.getListYear().subscribe(result => {
      this.yearList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing month from
    listService.getListMonth().subscribe(result => {
      this.monthListFrom = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing year from
    listService.getListYear().subscribe(result => {
      this.yearListFrom = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing month to
    listService.getListMonth().subscribe(result => {
      this.monthListTo = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing year to
    listService.getListYear().subscribe(result => {
      this.yearListTo = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing CamionType
    listService.getListNonConformiteNature(false, null).subscribe(result => {
      this.nonConformiteNatureList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing CamionType
    listService.getListNonConformiteEtapeType(false, null).subscribe(result => {
      this.nonConformiteEtapeTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing CamionType
    listService.getListCamionType().subscribe(result => {
      this.camionTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing MessageType
    listService.getListMessageType().subscribe(result => {
      this.messageTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing EntiteType
    listService.getListEntiteType().subscribe(result => {
      this.entiteTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing EcoOrganisme
    listService.getListEcoOrganisme().subscribe(result => {
      this.ecoOrganismeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ApplicationProduitOrigine
    listService.getListApplicationProduitOrigine().subscribe(result => {
      this.applicationProduitOrigineList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
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
  //Properties
  columns = [
    { columnDef: "Error", sticky: false, stickyEnd: false, header: this.applicationUserContext.getCulturedRessourceText(319), cell: (item: any) => item.Erreur },
  ];
  //Init columns
  displayedColumns = this.columns.map(c => c.columnDef);
  //-----------------------------------------------------------------------------------
  //On init, creation of columns
  ngOnInit() {
    //this.createColumns();
    if (this.applicationUserContext.currentMenu.gridField) {
      this.sort.active = this.applicationUserContext.currentMenu.gridField;
      this.sort.direction = this.applicationUserContext.currentMenu.gridSort ? "asc" : "desc";
    }
    else {
      switch (this.applicationUserContext.currentMenu.name) {
        case MenuName.AnnuaireMenuEntite:
          this.sort.active = DataColumnName.EntiteLibelle;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuDescriptionControle:
          this.sort.active = DataColumnName.DescriptionControleOrdre;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuDescriptionCVQ:
          this.sort.active = DataColumnName.DescriptionCVQOrdre;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuDescriptionReception:
          this.sort.active = DataColumnName.DescriptionReceptionOrdre;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuClientApplication:
          this.sort.active = DataColumnName.RefClientApplication;
          this.sort.direction = "desc";
          break;
        case MenuName.AdministrationMenuEquipementier:
          this.sort.active = DataColumnName.EquipementierLibelle;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuFonction:
          this.sort.active = DataColumnName.FonctionLibelle;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuFournisseurTO:
          this.sort.active = DataColumnName.FournisseurTOLibelle;
          this.sort.direction = "asc";
          break;
        case MenuName.LogistiqueMenuModifierTousPrixTransport:
        case MenuName.LogistiqueMenuModifierTransport:
        case MenuName.LogistiqueMenuSupprimerTransportEnMasse:
        case MenuName.LogistiqueMenuTransportNonValide:
        case MenuName.LogistiqueMenuValiderNouveauPrixTransport:
          this.sort.active = DataColumnName.RefTransport;
          this.sort.direction = "asc";
          break;
        case MenuName.LogistiqueMenuSurcoutCarburant:
          this.sort.active = DataColumnName.RefSurcoutCarburant;
          this.sort.direction = "desc";
          break;
        case MenuName.LogistiqueMenuTransportDemandeEnlevement:
          this.sort.active = DataColumnName.CommandeFournisseurDDisponibilite;
          this.sort.direction = "asc";
          break;
        case MenuName.LogistiqueMenuCommandeClient:
          this.sort.active = DataColumnName.RefCommandeClient;
          this.sort.direction = "desc";
          break;
        case MenuName.LogistiqueMenuCommandeFournisseur:
          if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client) {
            this.sort.active = DataColumnName.CommandeFournisseurDDechargementPrevue;
            this.sort.direction = "desc";
          }
          else {
            if (this.applicationUserContext.filterValideDPrevues) {
              this.sort.active = DataColumnName.CommandeFournisseurDDechargementPrevue;
              this.sort.direction = "asc";
            }
            else if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.CentreDeTri) {
              this.sort.active = DataColumnName.CommandeFournisseurDDisponibilite;
              this.sort.direction = "desc";
            }
          }
          break;
        case MenuName.LogistiqueMenuTransportCommande:
        case MenuName.LogistiqueMenuTransportCommandeEnCours:
        case MenuName.LogistiqueMenuTransportCommandeModifiee:
          this.sort.active = DataColumnName.CommandeFournisseurDDisponibilite;
          this.sort.direction = "asc";
          break;
        case MenuName.LogistiqueMenuRepartition:
          this.sort.active = DataColumnName.CommandeFournisseurNumeroCommande;
          this.sort.direction = "desc";
          break;
        case MenuName.LogistiqueMenuPrixReprise:
          this.sort.active = DataColumnName.RefPrixReprise;
          this.sort.direction = "desc";
          break;
        case MenuName.AdministrationMenuUtilisateur:
        case MenuName.AdministrationMenuUtilisateurInactif:
          this.sort.active = DataColumnName.UtilisateurNom;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuMontantIncitationQualite:
          this.sort.active = DataColumnName.RefMontantIncitationQualite;
          this.sort.direction = "desc";
          break;
        case MenuName.AdministrationMenuNonConformiteDemandeClientType:
          this.sort.active = DataColumnName.NonConformiteDemandeClientTypeLibelleFRFR;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuNonConformiteFamille:
          this.sort.active = DataColumnName.NonConformiteFamilleLibelleFRFR;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuNonConformiteNature:
          this.sort.active = DataColumnName.NonConformiteNatureLibelleFRFR;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuNonConformiteReponseClientType:
          this.sort.active = DataColumnName.NonConformiteReponseClientTypeLibelleFRFR;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuNonConformiteReponseFournisseurType:
          this.sort.active = DataColumnName.NonConformiteReponseFournisseurTypeLibelleFRFR;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuService:
          this.sort.active = DataColumnName.ServiceLibelle;
          this.sort.direction = "asc";
          break;
        case MenuName.AdministrationMenuTitre:
          this.sort.active = DataColumnName.TitreLibelle;
          this.sort.direction = "asc";
          break;
        case MenuName.MessagerieMenuMessage:
          this.sort.active = DataColumnName.RefMessage;
          this.sort.direction = "desc";
          break;
        case MenuName.MessagerieMenuVisualisation:
          this.sort.active = DataColumnName.MessageVisualisationD;
          this.sort.direction = "desc";
          break;
        case MenuName.QualiteMenuControle:
          this.sort.active = DataColumnName.CommandeFournisseurDDechargement;
          this.sort.direction = "desc";
          break;
        case MenuName.QualiteMenuNonConformite:
          this.sort.active = DataColumnName.CommandeFournisseurDDechargement;
          this.sort.direction = "desc";
          break;
      }
    }
    this.dataSource = new GridDataSource(this.gridService, this.snackBarQueueService, this.applicationUserContext);
    //Init filters
    switch (this.applicationUserContext.currentMenu.name) {
      case MenuName.LogistiqueMenuTransportDemandeEnlevement:
      case MenuName.LogistiqueMenuTransportCommande:
      case MenuName.LogistiqueMenuTransportCommandeEnCours:
      case MenuName.LogistiqueMenuTransportCommandeModifiee:
      case MenuName.AdministrationMenuUtilisateurInactif:
        this.applicationUserContext.filterBegin = moment([1, 0, 1, 0, 0, 0, 0]);
        break;
    }
    this.loadPage(""); 
    //Localization
    this.pagin._intl.firstPageLabel = this.applicationUserContext.getCulturedRessourceText(294);
    this.pagin._intl.lastPageLabel = this.applicationUserContext.getCulturedRessourceText(295);
    this.pagin._intl.itemsPerPageLabel = this.applicationUserContext.getCulturedRessourceText(296);
    this.pagin._intl.previousPageLabel = this.applicationUserContext.getCulturedRessourceText(297);
    this.pagin._intl.nextPageLabel = this.applicationUserContext.getCulturedRessourceText(298);
    this.pagin._intl.getRangeLabel = (page: number, pageSize: number, length: number) => { if (length === 0 || pageSize === 0) { return ("0 de " + length); } length = Math.max(length, 0); const startIndex = page * pageSize; const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize; return ((startIndex + 1) + " - " + endIndex + " de " + length); }
    //Filters visibility
    this.setFilterVisibility();
    // listen for search field value changes
    this.form.get("TransporteurListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterTransporteursMulti();
      });
    this.form.get("PrestataireListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterPrestatairesMulti();
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
    this.form.get("VilleArriveeListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterVilleArriveesMulti();
      });
    this.form.get("UtilisateurMaitreListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterUtilisateurMaitresMulti();
      });
  }
  //-----------------------------------------------------------------------------------
  ngAfterViewInit() {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.pagin.pageIndex = 0);

    this.pagin.page
      .pipe(
        tap(() => { this.razSelection(); this.loadPage("") })
      )
      .subscribe();

    this.sort.sortChange
      .pipe(
        tap(() => { this.razSelection(); this.loadPage("") })
      )
      .subscribe();

    //this.setInitialValue();
  }
  //-----------------------------------------------------------------------------------
  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }
  //-----------------------------------------------------------------------------------
  //Dynamic labels
  setDynamicLabels() {
    if (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeClient
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
    ) {
      this.labelMonthListFC = this.applicationUserContext.getCulturedRessourceText(459);
      this.labelYearListFC = this.applicationUserContext.getCulturedRessourceText(1071);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
    ) {
      this.labelMonthListFC = this.applicationUserContext.getCulturedRessourceText(860);
      this.labelYearListFC = this.applicationUserContext.getCulturedRessourceText(861);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuControle
    ) {
      this.labelMonthListFC = this.applicationUserContext.getCulturedRessourceText(1010);
      this.labelYearListFC = this.applicationUserContext.getCulturedRessourceText(1011);
    }
    if (
      this.applicationUserContext.connectedUtilisateur.HabilitationLogistique == HabilitationLogistique.Client
    ) {
      this.labelDBeginFC = this.applicationUserContext.getCulturedRessourceText(587);
      this.labelDFinFC = this.applicationUserContext.getCulturedRessourceText(587);
    }
    if (
      this.applicationUserContext.connectedUtilisateur.HabilitationLogistique == HabilitationLogistique.CentreDeTri
    ) {
      this.labelDBeginFC = this.applicationUserContext.getCulturedRessourceText(1425);
      this.labelDFinFC = this.applicationUserContext.getCulturedRessourceText(1425);
    }
    else if (this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuVisualisation) {
      this.labelDBeginFC = this.applicationUserContext.getCulturedRessourceText(647);
      this.labelDFinFC = this.applicationUserContext.getCulturedRessourceText(647);
    }
    else if (this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuMessage) {
      this.labelDBeginFC = this.applicationUserContext.getCulturedRessourceText(644);
      this.labelDFinFC = this.applicationUserContext.getCulturedRessourceText(644);
    }
    else {
      this.labelDBeginFC = this.applicationUserContext.getCulturedRessourceText(1511);
      this.labelDFinFC = this.applicationUserContext.getCulturedRessourceText(1511);
    }
    if (
      this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuUtilisateurInactif
    ) {
      this.labelDBeginFC = this.applicationUserContext.getCulturedRessourceText(1116);
    }
  }
  //-----------------------------------------------------------------------------------
  //Filters visibility
  setFilterVisibility() {
    this.visibleFilterActif = (
      this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuUtilisateur
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuProduit
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuDescriptionControle
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuDescriptionCVQ
    );
    this.visibleFilterNonRepartissable = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur
        || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterAnomalieCommandeFournisseur = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterEnvCommandeFournisseurStatutList = (
      (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
        && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Client))
      || this.applicationUserContext.currentMenu.name === MenuName.ModulePrestataireMenuCommandeFournisseur
    );
    this.visibleFilterFilterText = (
      this.applicationUserContext.currentMenu.name === MenuName.AnnuaireMenuEntite
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuActionType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuAdresseType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuAide
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuApplication
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuCamionType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuCivilite
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuDescriptionControle
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuDescriptionCVQ
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuDescriptionReception
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuDocument
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuRepreneur
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuEcoOrganisme
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuEquivalentCO2
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuFonction
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuFormeContact
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuFournisseurTO
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuProduitGroupeReporting
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuProduitGroupeReportingType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuMessageType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuMessageType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuModeTransportEE
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuMotifAnomalieChargement
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuMotifAnomalieClient
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuMotifAnomalieTransporteur
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuMotifCamionIncomplet
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuNonConformiteDemandeClientType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuNonConformiteFamille
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuNonConformiteNature
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuNonConformiteReponseClientType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuNonConformiteReponseFournisseurType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuParametre
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuParamEmail
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuPays
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuProcess
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuProduit
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuApplicationProduitOrigine
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuRegionEE
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuRegionReporting
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuRepreneur
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuRepriseType
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuRessource
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuSAGECodeTransport
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuService
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuStandard
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuTicket
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuTitre
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuUtilisateur
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuUtilisateurInactif
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeClient
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuRepartition
      || this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuMessage
      || this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuVisualisation
      || this.applicationUserContext.currentMenu.name === MenuName.ModulePrestataireMenuCommandeFournisseur
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuControle
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
    );
    this.visibleFilterApplicationProduitOrigineList = (
      this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuClientApplication
    );
    this.visibleFilterCamionTypeList = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTransport
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSupprimerTransportEnMasse
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuValiderNouveauPrixTransport
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTousPrixTransport
    );
    this.visibleFilterCentreDeTriList = (
      !this.applicationUserContext.connectedUtilisateur.CentreDeTri
      && (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuRepartition
        || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuControle
        || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
        || this.applicationUserContext.currentMenu.name === MenuName.ModulePrestataireMenuCommandeFournisseur
        )
    );
    this.visibleFilterClientList = (
      !this.applicationUserContext.connectedUtilisateur.Client
      &&!this.applicationUserContext.connectedUtilisateur.CentreDeTri
      && (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeClient
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
        || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuClientApplication
        || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuControle
        || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
        )
    );
    this.visibleFilterComposantList = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuPrixReprise
    );
    this.visibleFilterDRList = (
      this.applicationUserContext.currentMenu.name === MenuName.AnnuaireMenuEntite
      || (this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuControle && this.applicationUserContext.connectedUtilisateur.Client == null)
      || (this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite && this.applicationUserContext.connectedUtilisateur.Client == null)
    );
    this.visibleFilterUtilisateurMaitreList = (
      this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuUtilisateur
    );
    this.visibleFilterDatesTransporteur = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterDBegin = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
      || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
      || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client)
      || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.CentreDeTri)
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuUtilisateurInactif
      || this.applicationUserContext.currentMenu.name === MenuName.ModulePrestataireMenuCommandeFournisseur
      || this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuVisualisation
      || this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuMessage
    );
    this.visibleFilterDEnd = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
      || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
      || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client)
      || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.CentreDeTri)
      || this.applicationUserContext.currentMenu.name === MenuName.ModulePrestataireMenuCommandeFournisseur
      || this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuVisualisation
      || this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuMessage
    );
    this.visibleFilterDChargementModif = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterDptArriveeList = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTransport
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSupprimerTransportEnMasse
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuValiderNouveauPrixTransport
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTousPrixTransport
    );
    this.visibleFilterDptDepartList = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTransport
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSupprimerTransportEnMasse
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuValiderNouveauPrixTransport
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTousPrixTransport
    );
    this.visibleFilterEntiteTypeList = (
      this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuVisualisation
      || this.applicationUserContext.currentMenu.name === MenuName.AnnuaireMenuEntite
    );
    this.visibleFilterDonneeEntiteSage = (
      this.applicationUserContext.currentMenu.name === MenuName.AnnuaireMenuEntite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationAnnuaire === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterEcoOrganismeList = (
      this.applicationUserContext.currentMenu.name === MenuName.AnnuaireMenuEntite
    );
    this.visibleFilterIndustrielList = (
      false
    );
    this.visibleFilterMessageTypeList = (
      this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuMessage
      || this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuVisualisation
    );
    this.visibleFilterMonthList = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeClient
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuRepartition
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuPrixReprise
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSurcoutCarburant
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuControle
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
    );
    this.visibleFilterNonConformiteEtapeTypeList = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
    );
    this.visibleFilterNonConformiteNatureList = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
    );
    this.visibleFilterIFFournisseur = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterIFClient = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationLogistique.Administrateur
        || this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationLogistique.Client)
    );
    this.visibleFilterIFFournisseurRetourLot = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
    );
    this.visibleFilterIFFournisseurFacture = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterIFFournisseurAttenteBonCommande = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      && (this.applicationUserContext.connectedUtilisateur.Client == null)
    );
    this.visibleFilterIFFournisseurTransmissionFacturation = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterNonConformitesATraiter = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationLogistique.Utilisateur)
    );
    this.visibleFilterNonConformitesATransmettre = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationLogistique.Utilisateur)
    );
    this.visibleFilterNonConformitesPlanActionAValider = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterNonConformitesPlanActionNR = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationLogistique.Utilisateur)
    );
    this.visibleFilterPaysList = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTransport
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSupprimerTransportEnMasse
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuValiderNouveauPrixTransport
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTousPrixTransport
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSurcoutCarburant
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuPays
    );
    this.visibleFilterProcessList = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuPrixReprise
    );
    this.visibleFilterRegionReportingList = (
      this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuPays
    );
    this.visibleFilterProduitList = (
      this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuDescriptionControle
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuDescriptionCVQ
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeClient
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuRepartition
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuPrixReprise
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuControle
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      || this.applicationUserContext.currentMenu.name === MenuName.ModulePrestataireMenuCommandeFournisseur
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuProduit
    );
    this.visibleFilterControle = (
      this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuControle
    );
    this.visibleFilterTransporteurList = (
      (!this.applicationUserContext.connectedUtilisateur.Transporteur
        && (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide
          || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTransport
          || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSupprimerTransportEnMasse
          || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuValiderNouveauPrixTransport
          || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTousPrixTransport
          || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
          || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSurcoutCarburant
          || this.applicationUserContext.currentMenu.name === MenuName.ModulePrestataireMenuCommandeFournisseur
        )
      )
    );
    this.visibleFilterPrestataireList = (
      (!this.applicationUserContext.connectedUtilisateur.Prestataire
        && !this.applicationUserContext.connectedUtilisateur.Transporteur
        && !this.applicationUserContext.connectedUtilisateur.CentreDeTri
        && !this.applicationUserContext.connectedUtilisateur.Client
        && (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
          || this.applicationUserContext.currentMenu.name === MenuName.ModulePrestataireMenuCommandeFournisseur
        )
      )
    );
    this.visibleFilterValideDPrevues = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterCommandesFournisseurNonChargees = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
    );
    this.visibleFilterCommandesFournisseurAttribuees = (
      (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client))
      ||(this.applicationUserContext.currentMenu.name === MenuName.ModulePrestataireMenuCommandeFournisseur)
    );
    this.visibleFilterVilleArriveeList = (
      !this.applicationUserContext.connectedUtilisateur.CentreDeTri
      && (
        this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
        || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur && this.applicationUserContext.connectedUtilisateur.Client == null)
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTransport
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSupprimerTransportEnMasse
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuValiderNouveauPrixTransport
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTousPrixTransport
      )
    );
    this.visibleFilterVilleDepartList = (
      !this.applicationUserContext.connectedUtilisateur.CentreDeTri
      && (
        this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportDemandeEnlevement
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommande
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeEnCours
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportCommandeModifiee
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTransport
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSupprimerTransportEnMasse
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuValiderNouveauPrixTransport
        || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTousPrixTransport
      )
    );
    this.visibleFilterYearList = (
      this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeClient
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuRepartition
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuPrixReprise
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuClientApplication
      || this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSurcoutCarburant
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuControle
      || this.applicationUserContext.currentMenu.name === MenuName.QualiteMenuNonConformite
      || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuJourFerie
    );
    this.visibleFilterCollecte = (
      this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuProduit
    );
  }
  //-----------------------------------------------------------------------------------
  //Styling
  setStyle(item: any, column: any): string {
    let classes: string = "ev-cell-pointer";
    //Allow click
    if (this.applicationUserContext.currentMenu.name == MenuName.LogistiqueMenuTransportDemandeEnlevement) {
      classes = "ev-cell";
    }
    //General styling
    switch (this.applicationUserContext.currentMenu.name) {
      case MenuName.AnnuaireMenuEntite:
          if (!item[DataColumnName.EntiteActif] == true) { classes += " color-inactif"; }
        break;
      case MenuName.LogistiqueMenuCommandeFournisseur:
      case MenuName.ModulePrestataireMenuCommandeFournisseur:
        if (column.columnDef === DataColumnName.EntiteCodeCITEO
          || column.columnDef === DataColumnName.CommandeFournisseurAdresseOrigineDpt
          || column.columnDef === DataColumnName.CommandeFournisseurAdresseOrigineVille
          || column.columnDef === DataColumnName.CommandeFournisseurDDisponibilite) {
          if (!item.CommandeFournisseurCamionCompletInvisible) { classes += " background-color-camion-incomplet"; }
        }
        if (column.columnDef === DataColumnName.CommandeFournisseurNumeroCommande) {
          if (item.RefCommandeFournisseurStatut == 1) { classes += " background-color-statut-1"; }
          else if (item.RefCommandeFournisseurStatut == 2) { classes += " background-color-statut-2"; }
          else if (item.RefCommandeFournisseurStatut == 3) { classes += " background-color-statut-3"; }
        }
        if (column.columnDef === DataColumnName.CentreDeTriLibelle) {
          if (item.RefMotifAnomalieChargement && item.CommandeFournisseurDTraitementAnomalieChargementInvisible == null) { classes += " background-color-anomalie"; }
        }
        if (column.columnDef === DataColumnName.ClientLibelle) {
          if (item.RefMotifAnomalieClient && item.CommandeFournisseurDTraitementAnomalieClientInvisible == null) { classes += " background-color-anomalie"; }
        }
        if (column.columnDef === DataColumnName.TransporteurLibelle) {
          if (item.RefMotifAnomalieTransporteur && item.CommandeFournisseurDTraitementAnomalieTransporteurInvisible == null) { classes += " background-color-anomalie"; }
        }
        if (column.columnDef === DataColumnName.Controles) {
          if (item.CommandeFournisseurNonConformiteInvisible) {
            classes += " background-color-accent";
          }
          else if (item.Controles != "") { classes += " background-color-primary"; classes += " color-white"; }
        }
        if (column.columnDef === DataColumnName.CommandeFournisseurRefExt) {
          if (item.CommandeFournisseurRefExt) {
            classes += " background-icon-api";
          }
        }
        break;
      case MenuName.LogistiqueMenuTransportCommande:
      case MenuName.LogistiqueMenuTransportCommandeEnCours:
      case MenuName.LogistiqueMenuTransportCommandeModifiee:
        if (column.columnDef === DataColumnName.CommandeFournisseurDChargementPrevue
          || column.columnDef === DataColumnName.CommandeFournisseurDDechargementPrevue
        ) {
          if (item.CommandeFournisseurDChargementPrevue == null || item.CommandeFournisseurDDechargementPrevue == null) { classes += " background-color-donnee-manquante"; }
        }
        break;
      case MenuName.QualiteMenuControle:
        if (column.columnDef === DataColumnName.FicheControleControle
        ) {
          if (item.FicheControleControle) { classes += " background-color-primary"; classes += " color-white"; }
        }
        if (column.columnDef === DataColumnName.CommandeFournisseurNonConformite
        ) {
          if (item.CommandeFournisseurNonConformite) { classes += " background-color-accent"; }
        }
        else if (column.columnDef === DataColumnName.FicheControleReserve
        ) {
          if (item.FicheControleReserve) { classes += " background-color-accent"; }
        }
        break;
      case MenuName.QualiteMenuNonConformite:
        if (column.columnDef === DataColumnName.NonConformiteEtapeLibelle
        ) {
          if (item.NonConformiteClotureeInvisible) { classes += " background-color-grey"; }
        }
        if (column.columnDef === DataColumnName.NonConformiteIFFournisseurRetourLot
        ) {
          if (item.NonConformiteIFFournisseurRetourLot) { classes += " background-color-grey"; }
        }
        break;
      case MenuName.MessagerieMenuMessage:
      case MenuName.LogistiqueMenuModifierTransport:
        if (column == "infoTransportConcurrence") {
          if (item.RefTransport && item.RefTransport == this.refTransportConcurrence) { classes += " background-color-accent"; }
        }
        if (column == "infoMessageVisualisations") {
          if (item.RefMessage == this.refMessageVisualisations) { classes += " background-color-accent"; }
        }
        break;
    }
    return classes;
  }
  //-----------------------------------------------------------------------------------
  //Row clicked
  onRowClicked(item: any) {
    this.selectedItem = item;
    if (this.applicationUserContext.currentMenu.gridGoto) {
      this.router.navigate([this.applicationUserContext.currentMenu.gridGoto, item[this.applicationUserContext.currentMenu.gridRef]]);
    }
    else {
      switch (this.applicationUserContext.currentMenu.name) {
        case MenuName.AnnuaireMenuEntite:
          this.router.navigate(["entite", item.Id, { elementType: SearchElementType.Entite }])
          break;
        case MenuName.AdministrationMenuUtilisateur:
        case MenuName.AdministrationMenuUtilisateurInactif:
          this.router.navigate(["utilisateur", item.Id])
          break;
        case MenuName.AdministrationMenuDescriptionControle:
          this.router.navigate(["description-controle-container", item.RefProduit])
          break;
        case MenuName.AdministrationMenuDescriptionCVQ:
          this.router.navigate(["description-cvq-container", item.RefProduit])
          break;
        case MenuName.AdministrationMenuDescriptionReception:
          this.router.navigate(["description-reception-container"])
          break;
        case MenuName.AdministrationMenuClientApplication:
          this.router.navigate(["client-application", item.RefClientApplication])
          break;
        case MenuName.AdministrationMenuEquipementier:
          this.router.navigate(["equipementier", item.RefEquipementier])
          break;
        case MenuName.AdministrationMenuFonction:
          this.router.navigate(["fonction", item.RefFonction])
          break;
        case MenuName.AdministrationMenuFournisseurTO:
          this.router.navigate(["fournisseurto", item.RefFournisseurTO])
          break;
        case MenuName.AdministrationMenuMontantIncitationQualite:
          this.router.navigate(["montant-incitation-qualite", item.RefMontantIncitationQualite])
          break;
        case MenuName.AdministrationMenuNonConformiteDemandeClientType:
          this.router.navigate(["non-conformite-demande-client-type", item.RefNonConformiteDemandeClientType])
          break;
        case MenuName.AdministrationMenuNonConformiteFamille:
          this.router.navigate(["non-conformite-famille", item.RefNonConformiteFamille])
          break;
        case MenuName.AdministrationMenuNonConformiteNature:
          this.router.navigate(["non-conformite-nature", item.RefNonConformiteNature])
          break;
        case MenuName.AdministrationMenuNonConformiteReponseClientType:
          this.router.navigate(["non-conformite-reponse-client-type", item.RefNonConformiteReponseClientType])
          break;
        case MenuName.AdministrationMenuNonConformiteReponseFournisseurType:
          this.router.navigate(["non-conformite-reponse-fournisseur-type", item.RefNonConformiteReponseFournisseurType])
          break;
        case MenuName.AdministrationMenuTitre:
          this.router.navigate(["titre", item.RefTitre])
          break;
        case MenuName.AdministrationMenuService:
          this.router.navigate(["service", item.RefService])
          break;
        case MenuName.LogistiqueMenuModifierTousPrixTransport:
        case MenuName.LogistiqueMenuModifierTransport:
        case MenuName.LogistiqueMenuSupprimerTransportEnMasse:
        case MenuName.LogistiqueMenuTransportNonValide:
        case MenuName.LogistiqueMenuValiderNouveauPrixTransport:
          this.router.navigate(["transport", item.RefTransport == null ? "0" : item.RefTransport, {
            refAdresseOrigine: item.RefAdresseOrigine
            , refAdresseDestination: item.RefAdresseDestination
            , refTransporteur: item.RefTransporteur
            , refCamionType: item.RefCamionType
          }])
          break;
        case MenuName.LogistiqueMenuSurcoutCarburant:
          this.router.navigate(["surcout-carburant", item.RefSurcoutCarburant])
          break;
        case MenuName.LogistiqueMenuTransportCommande:
        case MenuName.LogistiqueMenuTransportCommandeEnCours:
        case MenuName.LogistiqueMenuTransportCommandeModifiee:
        case MenuName.LogistiqueMenuCommandeFournisseur:
          this.router.navigate(["commande-fournisseur", item.RefCommandeFournisseur])
          break;
        case MenuName.LogistiqueMenuCommandeClient:
          this.router.navigate(["commande-client", "0", {
            refEntite: item.RefEntite,
            refAdresse: item.RefAdresse,
            refEntiteFournisseur: item.RefEntiteFournisseur,
            d: item.CommandeClientMensuelleD
          }])
          break;
        case MenuName.LogistiqueMenuRepartition:
          this.router.navigate(["repartition", item.RefRepartition, {
            refCommandeFournisseur: item.RefCommandeFournisseur
          }])
          break;
        case MenuName.LogistiqueMenuPrixReprise:
          this.router.navigate(["prix-reprise", "0", {
            refProcess: item.RefProcess,
            d: item.PrixRepriseD
          }])
          break;
        case MenuName.MessagerieMenuMessage:
          this.router.navigate(["message", item.RefMessage])
          break;
        case MenuName.QualiteMenuControle:
          this.router.navigate(["fiche-controle", item.RefFicheControle, {
            refCommandeFournisseur: "0"
          }])
          break;
        case MenuName.QualiteMenuNonConformite:
          this.router.navigate(["non-conformite", item.RefNonConformite, {
            refCommandeFournisseur: "0"
          }])
          break;
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Menu info clicked
  onTransportConcurrenceClicked(item: any) {
    let data: any;
    data = { title: this.applicationUserContext.getCulturedRessourceText(726), type: "TransportConcurrence", ref: item.RefTransport };
    this.refTransportConcurrence = (item.RefTransport ? item.RefTransport : 0);
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      maxHeight: "350px",
      data,
      autoFocus: false,
      restoreFocus: false,
      backdropClass: "cdk-overlay-transparent-backdrop",
      position: { right: "50px", top: "70px" }
    });
    //RAZ Style
    dialogRef.afterClosed().subscribe(result => { this.refTransportConcurrence = 0 });
  }

  //-----------------------------------------------------------------------------------
  //Menu infoMessageVisualisations clicked
  onMessageVisualisationsClicked(item: any) {
    let data: any;
    data = { title: this.applicationUserContext.getCulturedRessourceText(641), type: "MessageVisualisations", ref: item.RefMessage };
    this.refMessageVisualisations = item.RefMessage;
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      maxHeight: "350px",
      data,
      autoFocus: false,
      restoreFocus: false,
      backdropClass: "cdk-overlay-transparent-backdrop",
      position: { right: "50px", top: "70px" }
    });
    //RAZ Style
    dialogRef.afterClosed().subscribe(result => { this.refMessageVisualisations = 0});
  }

  // All rows are selected, or not
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.items.length;
    return numSelected === numRows;
  }

  // Selects all rows if they are not all selected; otherwise clear selection. 
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.items.forEach(row => this.selection.select(row));
  }
  //-----------------------------------------------------------------------------------
  //Save filters
  saveFilter() {
    //Save filters
    this.applicationUserContext.filterText = this.form.get("FilterText").value;
    this.applicationUserContext.filterBegin = (this.form.get("DBegin").value ? moment(this.form.get("DBegin").value) : moment([1, 0, 1, 0, 0, 0, 0]));
    this.applicationUserContext.filterEnd = (this.form.get("DEnd").value ? moment(this.form.get("DEnd").value) : moment([1, 0, 1, 0, 0, 0, 0]));
    this.applicationUserContext.filterClients = this.form.get("ClientList").value;
    this.applicationUserContext.filterTransporteurs = this.form.get("TransporteurList").value;
    this.applicationUserContext.filterPrestataires = this.form.get("PrestataireList").value;
    this.applicationUserContext.filterApplicationProduitOrigines = this.form.get("ApplicationProduitOrigineList").value;
    this.applicationUserContext.filterCentreDeTris = this.form.get("CentreDeTriList").value;
    this.applicationUserContext.filterEntiteTypes = this.form.get("EntiteTypeList").value;
    this.applicationUserContext.filterDonneeEntiteSage = this.form.get("DonneeEntiteSage").value;
    this.applicationUserContext.filterEcoOrganismes = this.form.get("EcoOrganismeList").value;
    this.applicationUserContext.filterIndustriels = this.form.get("IndustrielList").value;
    this.applicationUserContext.filterDptDeparts = this.form.get("DptDepartList").value;
    this.applicationUserContext.filterDptArrivees = this.form.get("DptArriveeList").value;
    this.applicationUserContext.filterVilleDeparts = this.form.get("VilleDepartList").value;
    this.applicationUserContext.filterVilleArrivees = this.form.get("VilleArriveeList").value;
    this.applicationUserContext.filterMessageTypes = this.form.get("MessageTypeList").value;
    this.applicationUserContext.filterMonths = this.form.get("MonthList").value;
    this.applicationUserContext.filterNonConformiteEtapeTypes = this.form.get("NonConformiteEtapeTypeList").value;
    this.applicationUserContext.filterNonConformiteNatures = this.form.get("NonConformiteNatureList").value;
    this.applicationUserContext.filterIFFournisseur = this.form.get("IFFournisseur").value;
    this.applicationUserContext.filterIFClient = this.form.get("IFClient").value;
    this.applicationUserContext.filterIFFournisseurRetourLot = this.form.get("IFFournisseurRetourLot").value;
    this.applicationUserContext.filterIFFournisseurFacture = this.form.get("IFFournisseurFacture").value;
    this.applicationUserContext.filterIFFournisseurTransmissionFacturation = this.form.get("IFFournisseurTransmissionFacturation").value;
    this.applicationUserContext.filterNonConformitesATraiter = this.form.get("NonConformitesATraiter").value;
    this.applicationUserContext.filterNonConformitesATransmettre = this.form.get("NonConformitesATransmettre").value;
    this.applicationUserContext.filterNonConformitesPlanActionAValider = this.form.get("NonConformitesPlanActionAValider").value;
    this.applicationUserContext.filterNonConformitesPlanActionNR = this.form.get("NonConformitesPlanActionNR").value;
    this.applicationUserContext.filterDatesTransporteur = this.form.get("DatesTransporteur").value;
    this.applicationUserContext.filterYears = this.form.get("YearList").value;
    this.applicationUserContext.filterProcesss = this.form.get("ProcessList").value;
    this.applicationUserContext.filterRegionReportings = this.form.get("RegionReportingList").value;
    this.applicationUserContext.filterProduits = this.form.get("ProduitList").value;
    this.applicationUserContext.filterControle = this.form.get("Controle").value;
    this.applicationUserContext.filterComposants = this.form.get("ComposantList").value;
    this.applicationUserContext.filterDRs = this.form.get("DRList").value;
    this.applicationUserContext.filterUtilisateurMaitres = this.form.get("UtilisateurMaitreList").value;
    this.applicationUserContext.filterPayss = this.form.get("PaysList").value;
    this.applicationUserContext.filterCamionTypes = this.form.get("CamionTypeList").value;
    this.applicationUserContext.filterEnvCommandeFournisseurStatuts = this.form.get("EnvCommandeFournisseurStatutList").value;
    this.applicationUserContext.filterCommandesFournisseurNonChargees = this.form.get("CommandesFournisseurNonChargees").value;
    this.applicationUserContext.filterCommandesFournisseurAttribuees = this.form.get("CommandesFournisseurAttribuees").value;
    this.applicationUserContext.filterValideDPrevues = this.form.get("ValideDPrevues").value;
    this.applicationUserContext.filterDChargementModif = this.form.get("DChargementModif").value;
    this.applicationUserContext.filterIFFournisseurAttenteBonCommande = this.form.get("IFFournisseurAttenteBonCommande").value;
    this.applicationUserContext.filterAnomalieCommandeFournisseur = this.form.get("AnomalieCommandeFournisseur").value;
    this.applicationUserContext.filterActif = this.form.get("Actif").value;
    this.applicationUserContext.filterNonRepartissable = (this.form.get("NonRepartissable").value == "1" ? true : (this.form.get("NonRepartissable").value == "0" ? false : null));
    this.applicationUserContext.filterCollecte = this.form.get("Collecte").value;
  }
  //-----------------------------------------------------------------------------------
  //Save filters
  razSelection() {
    //Save filters
    this.applicationUserContext.selectedItem = [];
  }
  //-----------------------------------------------------------------------------------
  //Set DMoisDechargementPrevu
  setDMoisDechargementPrevu(d: any, refCommandeFournisseur: number) {
    //Add or remove item
    if (refCommandeFournisseur) {
      let i: number;
      //remove if applicable
      if (this.moisDechargementPrevuItems) {
        i = this.moisDechargementPrevuItems.findIndex(x => x.refCommandeFournisseur === refCommandeFournisseur)
        if (i >= 0) {
          this.moisDechargementPrevuItems.splice(i, 1);
        }
      }
      //Add
      this.moisDechargementPrevuItems.push({ refCommandeFournisseur: refCommandeFournisseur, d: d.d });
    }
  }
  //-----------------------------------------------------------------------------------
  //Save and apply filters
  applyFilter() {
    //Save filters
    this.saveFilter();
    //RAZ selection
    this.razSelection();
    //Reset pager
    this.pagin.pageIndex = 0
    //Apply filters
    this.loadPage("");
  }
  //-----------------------------------------------------------------------------------
  //Copy PrixReprise
  copyPrixReprise() {
    //Reset pager
    this.pagin.pageIndex = 0
    //Calculate and get rows back
    this.loadPage(ActionName.CopyPrixReprise);
    //Reset formPrixRepriseCopy
    //this.formPrixRepriseCopy.reset();
  }
  //-----------------------------------------------------------------------------------
  //Calculate new prices
  calculatePrices() {
    //Reset pager
    this.pagin.pageIndex = 0
    //Calculate and get rows back
    this.loadPage("");
    //Reset augmentation
    this.formAugmentation.reset();
  }
  //-----------------------------------------------------------------------------------
  //Delete new prices
  deletePrices() {
    this.applicationUserContext.selectedItem = Array.from(this.selection.selected, item => item.RefTransport);
    //Get all transports to delete
    if (this.applicationUserContext.selectedItem && this.applicationUserContext.selectedItem.length > 0) {
      this.selectedItem = Array.prototype.map.call(this.applicationUserContext.selectedItem, function (item: number) { return item.toString(); }).join(",");
    }
    else {
      this.selectedItem = "";
    }
    //Reset pager
    this.pagin.pageIndex = 0;
    //Calculate and get rows back
    this.loadPage(ActionName.DeleteTransport);
    //Reset selection
    this.selection.clear();
  }
  //-----------------------------------------------------------------------------------
  //Delete new prices
  deleteAllPrices() {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(301) },
      autoFocus: false,
      restoreFocus: false
    });
    //Confirm
    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.applicationUserContext.selectedItem = [];
        //Get all transports to delete
        this.selectedItem = "all";
        //Reset pager
        this.pagin.pageIndex = 0;
        //Calculate and get rows back
        this.loadPage(ActionName.DeleteTransport);
        //Reset selection
        this.selection.clear();
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Validate new prices
  validatePrices() {
    this.applicationUserContext.selectedItem = Array.from(this.selection.selected, item => item.RefTransport);
    //Get all transports to validate
    if (this.applicationUserContext.selectedItem && this.applicationUserContext.selectedItem.length > 0) {
      this.selectedItem = Array.prototype.map.call(this.applicationUserContext.selectedItem, function (item: number) { return item.toString(); }).join(",");
    }
    else {
      this.selectedItem = "";
    }
    //Reset pager
    this.pagin.pageIndex = 0;
    //Calculate and get rows back
    this.loadPage(ActionName.ValidateTransportPrice);
    //Reset selection
    this.selection.clear();
  }
  //-----------------------------------------------------------------------------------
  //Validate new prices
  validateAllPrices() {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(302) },
      autoFocus: false,
      restoreFocus: false
    });
    //Confirm
    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.applicationUserContext.selectedItem = [];
        //Get all transports to validate
        this.selectedItem = "all";
        //Reset pager
        this.pagin.pageIndex = 0;
        //Calculate and get rows back
        this.loadPage(ActionName.ValidateTransportPrice);
        //Reset selection
        this.selection.clear();
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Reject new prices
  rejectPrices() {
    this.applicationUserContext.selectedItem = Array.from(this.selection.selected, item => item.RefTransport);
    //Get all transports to reject
    if (this.applicationUserContext.selectedItem && this.applicationUserContext.selectedItem.length > 0) {
      this.selectedItem = Array.prototype.map.call(this.applicationUserContext.selectedItem, function (item: number) { return item.toString(); }).join(",");
    }
    else {
      this.selectedItem = "";
    }
    //Reset pager
    this.pagin.pageIndex = 0;
    //Calculate and get rows back
    this.loadPage(ActionName.RejectTransportPrice);
    //Reset selection
    this.selection.clear();
  }
  //-----------------------------------------------------------------------------------
  //Reject new prices
  rejectAllPrices() {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(303) },
      autoFocus: false,
      restoreFocus: false
    });
    //Confirm
    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.applicationUserContext.selectedItem = [];
        //Get all transports to validate
        this.selectedItem = "all";
        //Reset pager
        this.pagin.pageIndex = 0;
        //Calculate and get rows back
        this.loadPage(ActionName.RejectTransportPrice);
        //Reset selection
        this.selection.clear();
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Deactivate users
  deactivateUtilisateur() {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(1113) },
      autoFocus: false,
      restoreFocus: false
    });
    //Confirm
    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.applicationUserContext.selectedItem = Array.from(this.selection.selected, item => item.Id);
        //Get all users to deactivate
        if (this.applicationUserContext.selectedItem && this.applicationUserContext.selectedItem.length > 0) {
          this.selectedItem = Array.prototype.map.call(this.applicationUserContext.selectedItem, function (item: number) { return item.toString(); }).join(",");
        }
        else {
          this.selectedItem = "";
        }
        //Reset pager
        this.pagin.pageIndex = 0;
        //Calculate and get rows back
        this.loadPage(ActionName.DeactivateUtilisateur);
        //Reset selection
        this.selection.clear();
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Set EnAttente
  setEnAttente() {
    this.applicationUserContext.selectedItem = Array.from(this.selection.selected, item => item.RefCommandeFournisseur);
    //Get all CommandeFournisseur to process
    if (this.applicationUserContext.selectedItem && this.applicationUserContext.selectedItem.length > 0) {
      this.selectedItem = Array.prototype.map.call(this.applicationUserContext.selectedItem, function (item: number) { return item.toString(); }).join(",");
    }
    else {
      this.selectedItem = "";
    }
    //Reset pager
    this.pagin.pageIndex = 0;
    //Calculate and get rows back
    this.loadPage(ActionName.SetEnAttente);
    //Reset selection
    this.selection.clear();
  }
  //-----------------------------------------------------------------------------------
  //Change MoisDechargementPrevu
  changeMoisDechargementPrevu() {
    //Get all CommandeFournisseur to process
    if (this.moisDechargementPrevuItems && this.moisDechargementPrevuItems.length > 0) {
      this.moisDechargementPrevuItem = Array.prototype.map.call(this.moisDechargementPrevuItems, function (item: any) { return (item.refCommandeFournisseur.toString() + ";" + moment(item.d).format("YYYY-MM-DD 00:00:00.000")); }).join(",");
    }
    else {
      this.moisDechargementPrevuItem = "";
    }
    //Reset pager
    this.pagin.pageIndex = 0;
    //Calculate and get rows back
    this.loadPage(ActionName.ChangeMoisDechargementPrevu);
    //Reset changes
    this.moisDechargementPrevuItems = [];
    //Reset selection
    this.selection.clear();
  }
  //-----------------------------------------------------------------------------------
  //Set filters
  setFilters() {
    this.filterText = this.applicationUserContext.filterText;
    this.filterBegin = this.applicationUserContext.filterBegin.format("YYYY-MM-DD 00:00:00.000");
    this.filterEnd = this.applicationUserContext.filterEnd.format("YYYY-MM-DD 00:00:00.000");
    if (this.applicationUserContext.filterClients && this.applicationUserContext.filterClients.length > 0) {
      this.filterClients = Array.prototype.map.call(this.applicationUserContext.filterClients, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterClients = "";
    }
    if (this.applicationUserContext.filterTransporteurs && this.applicationUserContext.filterTransporteurs.length > 0) {
      this.filterTransporteurs = Array.prototype.map.call(this.applicationUserContext.filterTransporteurs, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterTransporteurs = "";
    }
    if (this.applicationUserContext.filterPrestataires && this.applicationUserContext.filterPrestataires.length > 0) {
      this.filterPrestataires = Array.prototype.map.call(this.applicationUserContext.filterPrestataires, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterPrestataires = "";
    }
    if (this.applicationUserContext.filterApplicationProduitOrigines && this.applicationUserContext.filterApplicationProduitOrigines.length > 0) {
      this.filterApplicationProduitOrigines = Array.prototype.map.call(this.applicationUserContext.filterApplicationProduitOrigines, function (item: dataModelsInterfaces.ApplicationProduitOrigine) { return item.RefApplicationProduitOrigine; }).join(",");
    }
    else {
      this.filterApplicationProduitOrigines = "";
    }
    if (this.applicationUserContext.filterCentreDeTris && this.applicationUserContext.filterCentreDeTris.length > 0) {
      this.filterCentreDeTris = Array.prototype.map.call(this.applicationUserContext.filterCentreDeTris, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterCentreDeTris = "";
    }
    if (this.applicationUserContext.filterIndustriels && this.applicationUserContext.filterIndustriels.length > 0) {
      this.filterIndustriels = Array.prototype.map.call(this.applicationUserContext.filterIndustriels, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    else {
      this.filterIndustriels = "";
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
    if (this.applicationUserContext.filterVilleDeparts && this.applicationUserContext.filterVilleDeparts.length > 0) {
      this.filterVilleDeparts = Array.prototype.map.call(this.applicationUserContext.filterVilleDeparts, function (item: any) { return item.Ville; }).join(",");
    }
    else {
      this.filterVilleDeparts = "";
    }
    if (this.applicationUserContext.filterVilleArrivees && this.applicationUserContext.filterVilleArrivees.length > 0) {
      this.filterVilleArrivees = Array.prototype.map.call(this.applicationUserContext.filterVilleArrivees, function (item: any) { return item.Ville; }).join(",");
    }
    else {
      this.filterVilleArrivees = "";
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
    if (this.applicationUserContext.filterRegionReportings && this.applicationUserContext.filterRegionReportings.length > 0) {
      this.filterRegionReportings = Array.prototype.map.call(this.applicationUserContext.filterRegionReportings, function (item: dataModelsInterfaces.RegionReporting) { return item.RefRegionReporting; }).join(",");
    }
    else {
      this.filterRegionReportings = "";
    }
    if (this.applicationUserContext.filterProduits && this.applicationUserContext.filterProduits.length > 0) {
      this.filterProduits = Array.prototype.map.call(this.applicationUserContext.filterProduits, function (item: dataModelsInterfaces.ProduitList) { return item.RefProduit; }).join(",");
    }
    else {
      this.filterProduits = "";
    }
    if (this.applicationUserContext.filterComposants && this.applicationUserContext.filterComposants.length > 0) {
      this.filterComposants = Array.prototype.map.call(this.applicationUserContext.filterComposants, function (item: dataModelsInterfaces.Produit) { return item.RefProduit; }).join(",");
    }
    else {
      this.filterComposants = "";
    }
    if (this.applicationUserContext.filterDRs && this.applicationUserContext.filterDRs.length > 0) {
      this.filterDRs = Array.prototype.map.call(this.applicationUserContext.filterDRs, function (item: dataModelsInterfaces.UtilisateurList) { return item.RefUtilisateur; }).join(",");
    }
    else {
      this.filterDRs = "";
    }
    if (this.applicationUserContext.filterUtilisateurMaitres && this.applicationUserContext.filterUtilisateurMaitres.length > 0) {
      this.filterUtilisateurMaitres = Array.prototype.map.call(this.applicationUserContext.filterUtilisateurMaitres, function (item: dataModelsInterfaces.UtilisateurList) { return item.RefUtilisateur; }).join(",");
    }
    else {
      this.filterUtilisateurMaitres = "";
    }
    if (this.applicationUserContext.filterMonths && this.applicationUserContext.filterMonths.length > 0) {
      this.filterMonths = Array.prototype.map.call(this.applicationUserContext.filterMonths, function (item: appInterfaces.Month) { return moment(item.d).format("MM"); }).join(",");
    }
    else {
      this.filterMonths = "";
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
    if (this.applicationUserContext.filterCamionTypes && this.applicationUserContext.filterCamionTypes.length > 0) {
      this.filterCamionTypes = Array.prototype.map.call(this.applicationUserContext.filterCamionTypes, function (item: dataModelsInterfaces.CamionType) { return item.RefCamionType; }).join(",");
    }
    else {
      this.filterCamionTypes = "";
    }
    if (this.applicationUserContext.filterEnvCommandeFournisseurStatuts && this.applicationUserContext.filterEnvCommandeFournisseurStatuts.length > 0) {
      this.filterEnvCommandeFournisseurStatuts = Array.prototype.map.call(this.applicationUserContext.filterEnvCommandeFournisseurStatuts, function (item: appClasses.EnvCommandeFournisseurStatut) { return item.name; }).join(",");
    }
    else {
      this.filterEnvCommandeFournisseurStatuts = "";
    }
    if (this.applicationUserContext.filterEntiteTypes && this.applicationUserContext.filterEntiteTypes.length > 0) {
      this.filterEntiteTypes = Array.prototype.map.call(this.applicationUserContext.filterEntiteTypes, function (item: dataModelsInterfaces.EntiteType) { return item.RefEntiteType; }).join(",");
    }
    if (this.applicationUserContext.filterEcoOrganismes && this.applicationUserContext.filterEcoOrganismes.length > 0) {
      this.filterEcoOrganismes = Array.prototype.map.call(this.applicationUserContext.filterEcoOrganismes, function (item: dataModelsInterfaces.EcoOrganisme) { return item.RefEcoOrganisme; }).join(",");
    }
    else {
      this.filterEcoOrganismes = "";
    }
    if (this.applicationUserContext.filterMessageTypes && this.applicationUserContext.filterMessageTypes.length > 0) {
      this.filterMessageTypes = Array.prototype.map.call(this.applicationUserContext.filterMessageTypes, function (item: dataModelsInterfaces.MessageType) { return item.RefMessageType; }).join(",");
    }
    else {
      this.filterMessageTypes = "";
    }
  }
  //-----------------------------------------------------------------------------------
  //Export Excel file
  exportTransport() {
    this.setFilters();
    this.gridService.exportTransport(this.applicationUserContext.currentModule.name, this.applicationUserContext.currentMenu.name, this.sort.active, this.sort.direction, this.pagin.pageIndex, this.pagin.pageSize
      , this.filterBegin, this.filterEnd, this.filterText
      , this.filterTransporteurs, this.filterCentreDeTris, this.filterDptDeparts, this.filterDptArrivees, this.filterVilleDeparts, this.filterVilleArrivees
      , this.filterPayss, this.filterProduits, this.filterCamionTypes, this.filterMonths, this.formAugmentation.get("Augmentation").value)
      .subscribe((data: Response) => this.downloadFile(data), error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  downloadFile(data: any) {
    const blob = new Blob([data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
    //if (window.navigator.msSaveOrOpenBlob) //IE & Edge
    //{
    //  //msSaveBlob only available for IE & Edge
    //  window.navigator.msSaveBlob(blob, "TransportsValorplast.xlsx");
    //}
    //else //Chrome & FF
    {
      const url = window.URL.createObjectURL(blob);
      let fileName = "TransportsValorplast.xlsx";
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
    //Visibility
    if (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      && (this.applicationUserContext.filterEnvCommandeFournisseurStatuts.findIndex(x => x.name === EnvCommandeFournisseurStatutName.DemandeEnlevement) >= 0
        || this.applicationUserContext.filterEnvCommandeFournisseurStatuts.findIndex(x => x.name === EnvCommandeFournisseurStatutName.EnAttente) >= 0
        || this.applicationUserContext.filterEnvCommandeFournisseurStatuts.findIndex(x => x.name === EnvCommandeFournisseurStatutName.Ouverte) >= 0
        || this.applicationUserContext.filterEnvCommandeFournisseurStatuts.findIndex(x => x.name === EnvCommandeFournisseurStatutName.Bloquee) >= 0
      )
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
    ) {
      this.moisDechargementPrevuVisible = true;
    }
    else {
      this.moisDechargementPrevuVisible = false;
    }
    if (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
      && (this.applicationUserContext.filterEnvCommandeFournisseurStatuts && this.applicationUserContext.filterEnvCommandeFournisseurStatuts.length === 1 && this.applicationUserContext.filterEnvCommandeFournisseurStatuts[0].name === EnvCommandeFournisseurStatutName.DemandeEnlevement)
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
    ) {
      this.enAttenteVisible = true;
    }
    else {
      this.enAttenteVisible = false;
    }
    //Columns
    //this.createColumns();
    //Filters
    this.setFilters();
    //this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
    //Get data
    this.dataSource.loadItems(this.applicationUserContext.currentModule.name, this.applicationUserContext.currentMenu.name, action, this.sort.active, this.sort.direction, this.pagin.pageIndex, this.pagin.pageSize
      , this.filterBegin, this.filterEnd, this.filterText
      , this.filterClients, this.filterTransporteurs, this.filterPrestataires, this.filterCentreDeTris, this.filterIndustriels, this.filterDptDeparts, this.filterDptArrivees, this.filterVilleDeparts, this.filterVilleArrivees
      , this.filterPayss, this.filterProcesss, this.filterRegionReportings, this.filterProduits, this.filterComposants, this.filterCamionTypes
      , this.filterMonths, this.filterYears, this.filterNonConformiteEtapeTypes, this.filterNonConformiteNatures
      , this.applicationUserContext.filterIFFournisseur, this.applicationUserContext.filterIFClient, this.applicationUserContext.filterIFFournisseurRetourLot, this.applicationUserContext.filterIFFournisseurFacture, this.applicationUserContext.filterIFFournisseurAttenteBonCommande
      , this.applicationUserContext.filterIFFournisseurTransmissionFacturation, this.applicationUserContext.filterNonConformitesATraiter, this.applicationUserContext.filterNonConformitesATransmettre, this.applicationUserContext.filterNonConformitesPlanActionAValider
      , this.applicationUserContext.filterNonConformitesPlanActionNR
      , this.filterEnvCommandeFournisseurStatuts, this.applicationUserContext.filterDChargementModif, this.applicationUserContext.filterCommandesFournisseurNonChargees, this.applicationUserContext.filterValideDPrevues, this.applicationUserContext.filterDatesTransporteur
      , this.applicationUserContext.filterAnomalieCommandeFournisseur, this.applicationUserContext.filterControle, this.filterDRs, this.applicationUserContext.filterCommandesFournisseurAttribuees
      , this.formAugmentation.get("Augmentation").value.toString(), this.selectedItem, this.moisDechargementPrevuItem
      , this.formPrixRepriseCopy.get("YearListFrom").value ? this.formPrixRepriseCopy.get("YearListFrom").value.toString() : "", this.formPrixRepriseCopy.get("MonthListFrom").value ? this.formPrixRepriseCopy.get("MonthListFrom").value : ""
      , this.formPrixRepriseCopy.get("YearListTo").value ? this.formPrixRepriseCopy.get("YearListTo").value.toString() : "", this.formPrixRepriseCopy.get("MonthListTo").value ? this.formPrixRepriseCopy.get("MonthListTo").value : ""
      , this.filterEcoOrganismes, this.filterEntiteTypes, this.filterMessageTypes, this.filterApplicationProduitOrigines, this.applicationUserContext.filterActif, this.applicationUserContext.filterNonRepartissable, this.applicationUserContext.filterCollecte
      , this.applicationUserContext.filterDonneeEntiteSage, this.filterUtilisateurMaitres
    )
  }
  //-----------------------------------------------------------------------------------
  //Functions for selecting selected items
  compareFnClient(item: dataModelsInterfaces.EntiteList, selectedItem: dataModelsInterfaces.EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnTransporteur(item: dataModelsInterfaces.EntiteList, selectedItem: dataModelsInterfaces.EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnApplicationProduitOrigine(item: dataModelsInterfaces.ApplicationProduitOrigine, selectedItem: dataModelsInterfaces.ApplicationProduitOrigine) {
    return item && selectedItem ? item.RefApplicationProduitOrigine === selectedItem.RefApplicationProduitOrigine : item === selectedItem;
  }
  compareFnCentreDeTri(item: dataModelsInterfaces.EntiteList, selectedItem: dataModelsInterfaces.EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnIndustriel(item: dataModelsInterfaces.EntiteList, selectedItem: dataModelsInterfaces.EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnPrestataire(item: dataModelsInterfaces.EntiteList, selectedItem: dataModelsInterfaces.EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnDptDepart(item: dataModelsInterfaces.Dpt, selectedItem: dataModelsInterfaces.Dpt) {
    return item && selectedItem ? item.RefDpt === selectedItem.RefDpt : item === selectedItem;
  }
  compareFnDptArrivee(item: dataModelsInterfaces.Dpt, selectedItem: dataModelsInterfaces.Dpt) {
    return item && selectedItem ? item.RefDpt === selectedItem.RefDpt : item === selectedItem;
  }
  compareFnVilleDepart(item: any, selectedItem: any) {
    return item && selectedItem ? item.Ville === selectedItem.Ville : item === selectedItem;
  }
  compareFnVilleArrivee(item: any, selectedItem: any) {
    return item && selectedItem ? item.Ville === selectedItem.Ville : item === selectedItem;
  }
  compareFnPays(item: dataModelsInterfaces.Pays, selectedItem: dataModelsInterfaces.Pays) {
    return item && selectedItem ? item.RefPays === selectedItem.RefPays : item === selectedItem;
  }
  compareFnProcess(item: dataModelsInterfaces.Process, selectedItem: dataModelsInterfaces.Process) {
    return item && selectedItem ? item.RefProcess === selectedItem.RefProcess : item === selectedItem;
  }
  compareFnRegionReporting(item: dataModelsInterfaces.RegionReporting, selectedItem: dataModelsInterfaces.RegionReporting) {
    return item && selectedItem ? item.RefRegionReporting === selectedItem.RefRegionReporting : item === selectedItem;
  }
  compareFnProduit(item: dataModelsInterfaces.ProduitList, selectedItem: dataModelsInterfaces.ProduitList) {
    return item && selectedItem ? item.RefProduit === selectedItem.RefProduit : item === selectedItem;
  }
  compareFnComposant(item: dataModelsInterfaces.Produit, selectedItem: dataModelsInterfaces.Produit) {
    return item && selectedItem ? item.RefProduit === selectedItem.RefProduit : item === selectedItem;
  }
  compareFnDR(item: dataModelsInterfaces.UtilisateurList, selectedItem: dataModelsInterfaces.UtilisateurList) {
    return item && selectedItem ? item.RefUtilisateur === selectedItem.RefUtilisateur : item === selectedItem;
  }
  compareFnUtilisateurMaitre(item: dataModelsInterfaces.UtilisateurList, selectedItem: dataModelsInterfaces.UtilisateurList) {
    return item && selectedItem ? item.RefUtilisateur === selectedItem.RefUtilisateur : item === selectedItem;
  }
  compareFnMonth(item: appInterfaces.Month, selectedItem: appInterfaces.Month) {
    return item && selectedItem ? item.d === selectedItem.d : item === selectedItem;
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
  compareFnCamionType(item: dataModelsInterfaces.CamionType, selectedItem: dataModelsInterfaces.CamionType) {
    return item && selectedItem ? item.RefCamionType === selectedItem.RefCamionType : item === selectedItem;
  }
  compareFnEnvCommandeFournisseurStatut(item: appClasses.EnvCommandeFournisseurStatut, selectedItem: appClasses.EnvCommandeFournisseurStatut) {
    return item && selectedItem ? item.name === selectedItem.name : item === selectedItem;
  }
  compareFnEntiteType(item: dataModelsInterfaces.EntiteType, selectedItem: dataModelsInterfaces.EntiteType) {
    return item && selectedItem ? item.RefEntiteType === selectedItem.RefEntiteType : item === selectedItem;
  }
  compareFnEcoOrganisme(item: dataModelsInterfaces.EcoOrganisme, selectedItem: dataModelsInterfaces.EcoOrganisme) {
    return item && selectedItem ? item.RefEcoOrganisme === selectedItem.RefEcoOrganisme : item === selectedItem;
  }
  compareFnMessageType(item: dataModelsInterfaces.MessageType, selectedItem: dataModelsInterfaces.MessageType) {
    return item && selectedItem ? item.RefMessageType === selectedItem.RefMessageType : item === selectedItem;
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
  protected filterPrestatairesMulti() {
    if (!this.prestataireList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("PrestataireListFilter").value;
    if (!search) {
      this.filteredPrestataireList.next(this.prestataireList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredPrestataireList.next(
      this.prestataireList.filter(e => removeAccents(e.Libelle.toLowerCase()).indexOf(search) > -1)
    );
  }
  protected filterUtilisateurMaitresMulti() {
    if (!this.utilisateurMaitreList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("UtilisateurMaitreListFilter").value;
    if (!search) {
      this.filteredUtilisateurMaitreList.next(this.utilisateurMaitreList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredUtilisateurMaitreList.next(
      this.utilisateurMaitreList.filter(e => removeAccents(e.Nom.toLowerCase() + e.EntiteLibelle.toLowerCase()).indexOf(search) > -1)
    );
  }
  public onResetFilter() {
    this.applicationUserContext.razFilters();
    this.form.get("FilterText").setValue(this.applicationUserContext.filterText);
    this.form.get("DBegin").setValue(this.applicationUserContext.filterBegin.toISOString() === moment([1, 0, 1, 0, 0, 0, 0]).toISOString() ? "" : this.applicationUserContext.filterBegin);
    this.form.get("DEnd").setValue(this.applicationUserContext.filterEnd.toISOString() === moment([1, 0, 1, 0, 0, 0, 0]).toISOString() ? "" : this.applicationUserContext.filterEnd);
    this.form.get("ApplicationProduitOrigineList").setValue(this.applicationUserContext.filterApplicationProduitOrigines);
    this.form.get("ClientList").setValue(this.applicationUserContext.filterClients);
    this.form.get("ClientListFilter").setValue(null);
    this.form.get("TransporteurList").setValue(this.applicationUserContext.filterTransporteurs);
    this.form.get("TransporteurListFilter").setValue(null);
    this.form.get("PrestataireList").setValue(this.applicationUserContext.filterPrestataires);
    this.form.get("PrestataireListFilter").setValue(null);
    this.form.get("CentreDeTriList").setValue(this.applicationUserContext.filterCentreDeTris);
    this.form.get("CentreDeTriListFilter").setValue(null);
    this.form.get("IndustrielList").setValue(this.applicationUserContext.filterIndustriels);
    this.form.get("DptDepartList").setValue(this.applicationUserContext.filterDptDeparts);
    this.form.get("DptArriveeList").setValue(this.applicationUserContext.filterDptArrivees);
    this.form.get("EntiteTypeList").setValue(this.applicationUserContext.filterEntiteTypes);
    this.form.get("DonneeEntiteSage").setValue(this.applicationUserContext.filterDonneeEntiteSage);
    this.form.get("EcoOrganismeList").setValue(this.applicationUserContext.filterEcoOrganismes);
    this.form.get("VilleDepartList").setValue(this.applicationUserContext.filterVilleDeparts);
    this.form.get("VilleDepartListFilter").setValue(null);
    this.form.get("VilleArriveeList").setValue(this.applicationUserContext.filterVilleArrivees);
    this.form.get("VilleArriveeListFilter").setValue(null);
    this.form.get("PaysList").setValue(this.applicationUserContext.filterPayss);
    this.form.get("ProcessList").setValue(this.applicationUserContext.filterProcesss);
    this.form.get("RegionReportingList").setValue(this.applicationUserContext.filterRegionReportings);
    this.form.get("ProduitList").setValue(this.applicationUserContext.filterProduits);
    this.form.get("ProduitListFilter").setValue(null);
    this.form.get("Controle").setValue(this.applicationUserContext.filterControle);
    this.form.get("ComposantList").setValue(this.applicationUserContext.filterComposants);
    this.form.get("DRList").setValue(this.applicationUserContext.filterDRs);
    this.form.get("UtilisateurMaitreList").setValue(this.applicationUserContext.filterUtilisateurMaitres);
    this.form.get("MessageTypeList").setValue(this.applicationUserContext.filterMessageTypes);
    this.form.get("MonthList").setValue(this.applicationUserContext.filterMonths);
    this.form.get("YearList").setValue(this.applicationUserContext.filterYears);
    this.form.get("NonConformiteEtapeTypeList").setValue(this.applicationUserContext.filterNonConformiteEtapeTypes);
    this.form.get("NonConformiteNatureList").setValue(this.applicationUserContext.filterNonConformiteNatures);
    this.form.get("IFFournisseur").setValue(this.applicationUserContext.filterIFFournisseur);
    this.form.get("IFClient").setValue(this.applicationUserContext.filterIFClient);
    this.form.get("IFFournisseurRetourLot").setValue(this.applicationUserContext.filterIFFournisseurRetourLot);
    this.form.get("IFFournisseurFacture").setValue(this.applicationUserContext.filterIFFournisseurFacture);
    this.form.get("IFFournisseurAttenteBonCommande").setValue(this.applicationUserContext.filterIFFournisseurAttenteBonCommande);
    this.form.get("IFFournisseurTransmissionFacturation").setValue(this.applicationUserContext.filterIFFournisseurTransmissionFacturation);
    this.form.get("IFFournisseurFacture").setValue(this.applicationUserContext.filterIFFournisseurFacture);
    this.form.get("IFFournisseurTransmissionFacturation").setValue(this.applicationUserContext.filterIFFournisseurTransmissionFacturation);
    this.form.get("NonConformitesATraiter").setValue(this.applicationUserContext.filterNonConformitesATraiter);
    this.form.get("NonConformitesATransmettre").setValue(this.applicationUserContext.filterNonConformitesATransmettre);
    this.form.get("NonConformitesPlanActionAValider").setValue(this.applicationUserContext.filterNonConformitesPlanActionAValider);
    this.form.get("NonConformitesPlanActionNR").setValue(this.applicationUserContext.filterNonConformitesPlanActionNR);
    this.form.get("CamionTypeList").setValue(this.applicationUserContext.filterCamionTypes);
    this.form.get("EnvCommandeFournisseurStatutList").setValue(this.applicationUserContext.filterEnvCommandeFournisseurStatuts);
    this.form.get("CommandesFournisseurNonChargees").setValue(this.applicationUserContext.filterCommandesFournisseurNonChargees);
    this.form.get("CommandesFournisseurAttribuees").setValue(this.applicationUserContext.filterCommandesFournisseurAttribuees);
    this.form.get("ValideDPrevues").setValue(this.applicationUserContext.filterValideDPrevues);
    this.form.get("DChargementModif").setValue(this.applicationUserContext.filterDChargementModif);
    this.form.get("DatesTransporteur").setValue(this.applicationUserContext.filterDatesTransporteur);
    this.form.get("AnomalieCommandeFournisseur").setValue(this.applicationUserContext.filterAnomalieCommandeFournisseur)
    this.form.get("EnvCommandeFournisseurStatutList").setValue(this.applicationUserContext.filterEnvCommandeFournisseurStatuts);
    this.form.get("Actif").setValue(this.applicationUserContext.filterActif);
    this.form.get("NonRepartissable").setValue(this.applicationUserContext.filterNonRepartissable);
    this.form.get("Collecte").setValue(this.applicationUserContext.filterCollecte);
  }
  public onFilterTextReset() {
    this.applicationUserContext.filterText = "";
    this.form.get("FilterText").setValue(this.applicationUserContext.filterText);
  }
  public onEnvCommandeFournisseurStatutListReset() {
    this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
    this.form.get("EnvCommandeFournisseurStatutList").setValue(this.applicationUserContext.filterEnvCommandeFournisseurStatuts);
  }
  public onDBeginReset() {
    this.applicationUserContext.filterBegin = moment([1, 0, 1, 0, 0, 0, 0]);
    this.form.get("DBegin").setValue(this.applicationUserContext.filterBegin.toISOString() === moment([1, 0, 1, 0, 0, 0, 0]).toISOString() ? "" : this.applicationUserContext.filterBegin);
  }
  public onDEndReset() {
    this.applicationUserContext.filterEnd = moment([1, 0, 1, 0, 0, 0, 0]);
    this.form.get("DEnd").setValue(this.applicationUserContext.filterEnd.toISOString() === moment([1, 0, 1, 0, 0, 0, 0]).toISOString() ? "" : this.applicationUserContext.filterEnd);
  }
  public onTransporteurListReset() {
    this.applicationUserContext.filterTransporteurs = [];
    this.form.get("TransporteurList").setValue(this.applicationUserContext.filterTransporteurs);
  }
  public onPrestataireListReset() {
    this.applicationUserContext.filterPrestataires = [];
    this.form.get("PrestataireList").setValue(this.applicationUserContext.filterPrestataires);
  }
  public onClientListReset() {
    this.applicationUserContext.filterClients = [];
    this.form.get("ClientList").setValue(this.applicationUserContext.filterClients);
  }
  public onCentreDeTriListReset() {
    this.applicationUserContext.filterCentreDeTris = [];
    this.form.get("CentreDeTriList").setValue(this.applicationUserContext.filterCentreDeTris);
  }
  public onIndustrielListReset() {
    this.applicationUserContext.filterIndustriels = [];
    this.form.get("IndustrielList").setValue(this.applicationUserContext.filterIndustriels);
  }
  public onProcessListReset() {
    this.applicationUserContext.filterProcesss = [];
    this.form.get("ProcessList").setValue(this.applicationUserContext.filterProcesss);
  }
  public onRegionReportingListReset() {
    this.applicationUserContext.filterRegionReportings = [];
    this.form.get("RegionReportingList").setValue(this.applicationUserContext.filterRegionReportings);
  }
  public onProduitListReset() {
    this.applicationUserContext.filterProduits = [];
    this.form.get("ProduitList").setValue(this.applicationUserContext.filterProduits);
  }
  public onComposantListReset() {
    this.applicationUserContext.filterComposants = [];
    this.form.get("ComposantList").setValue(this.applicationUserContext.filterComposants);
  }
  public onDptDepartListReset() {
    this.applicationUserContext.filterDptDeparts = [];
    this.form.get("DptDepartList").setValue(this.applicationUserContext.filterDptDeparts);
  }
  public onVilleDepartListReset() {
    this.applicationUserContext.filterVilleDeparts = [];
    this.form.get("VilleDepartList").setValue(this.applicationUserContext.filterVilleDeparts);
  }
  public onDptArriveeListReset() {
    this.applicationUserContext.filterDptArrivees = [];
    this.form.get("DptArriveeList").setValue(this.applicationUserContext.filterDptArrivees);
  }
  public onVilleArriveeListReset() {
    this.applicationUserContext.filterVilleArrivees = [];
    this.form.get("VilleArriveeList").setValue(this.applicationUserContext.filterVilleArrivees);
  }
  public onPaysListReset() {
    this.applicationUserContext.filterPayss = [];
    this.form.get("PaysList").setValue(this.applicationUserContext.filterPayss);
  }
  public onMonthListReset() {
    this.applicationUserContext.filterMonths = [];
    this.form.get("MonthList").setValue(this.applicationUserContext.filterMonths);
  }
  public onYearListReset() {
    this.applicationUserContext.filterYears = [];
    this.form.get("YearList").setValue(this.applicationUserContext.filterYears);
  }
  public onNonConformiteEtapeTypeListReset() {
    this.applicationUserContext.filterNonConformiteEtapeTypes = [];
    this.form.get("NonConformiteEtapeTypeList").setValue(this.applicationUserContext.filterNonConformiteEtapeTypes);
  }
  public onNonConformiteNatureListReset() {
    this.applicationUserContext.filterNonConformiteNatures = [];
    this.form.get("NonConformiteNatureList").setValue(this.applicationUserContext.filterNonConformiteNatures);
  }
  public onCamionTypeListReset() {
    this.applicationUserContext.filterCamionTypes = [];
    this.form.get("CamionTypeList").setValue(this.applicationUserContext.filterCamionTypes);
  }
  public onMessageTypeListReset() {
    this.applicationUserContext.filterMessageTypes = [];
    this.form.get("MessageTypeList").setValue(this.applicationUserContext.filterMessageTypes);
  }
  public onEntiteTypeListReset() {
    this.applicationUserContext.filterEntiteTypes = [];
    this.form.get("EntiteTypeList").setValue(this.applicationUserContext.filterEntiteTypes);
  }
  public onEcoOrganismeListReset() {
    this.applicationUserContext.filterEcoOrganismes = [];
    this.form.get("EcoOrganismeList").setValue(this.applicationUserContext.filterEcoOrganismes);
  }
  public onApplicationProduitOrigineListReset() {
    this.applicationUserContext.filterApplicationProduitOrigines = [];
    this.form.get("ApplicationProduitOrigineList").setValue(this.applicationUserContext.filterApplicationProduitOrigines);
  }
  public onDRListReset() {
    this.applicationUserContext.filterDRs = [];
    this.form.get("DRList").setValue(this.applicationUserContext.filterDRs);
  }
  public onUtilisateurMaitreListReset() {
    this.applicationUserContext.filterUtilisateurMaitres = [];
    this.form.get("UtilisateurMaitreList").setValue(this.applicationUserContext.filterUtilisateurMaitres);
  }
  //-----------------------------------------------------------------------------------
  //Create profils info
  getProfilInfos(profil: UtilisateurList): string {
    let s = profil.Nom;
    if (GetUtilisateurTypeLocalizedLabel(profil.UtilisateurType, this.applicationUserContext)) {
      s += " - " + GetUtilisateurTypeLocalizedLabel(profil.UtilisateurType, this.applicationUserContext);
    }
    if (profil.EntiteLibelle) {
      s += " - " + profil.EntiteLibelle;
    }
    //End
    return s;
  }
}

//-----------------------------------------------------------------------------------
//Datasource class
export class GridDataSource implements DataSource<any> {
  //Subjects
  private itemsSubject = new BehaviorSubject<any[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();
  public nbRow: number = 0; //Pager length
  public nbAugmentation: string = "";
  public nbValidatedPrices: string = "";
  public nbRejectedPrices: string = "";
  public nbDeletedPrices: string = "";
  public nbSetEnAttente: string = "";
  public nbChangeMoisDechargementPrevu: string = "";
  public nbCopyPrixReprise: string = "";
  public nbDeactivateUtilisateur: string = "";
  public items: any[];
  public dMoisDechargementPrevuChangeList: { ref: number, m: appInterfaces.Month[] }[] = [];
  public columns: MatTableColumn[] = [];
  public displayedColumns: string[] = [];

  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private gridService: GridService
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
  //Load data
  loadItems(module = "", menu = "", action = "", sortExpression = "", sortDirection = "asc", pageIndex = 0, pageSize = 25
    , filterBegin = "", filterEnd = "", filterText = ""
    , filterClients = "", filterTransporteurs = "", filterPrestataires = "", filterCentreDeTris = "", filterIndustriels = "", filterDptDeparts = "", filterDptArrivees = ""
    , filterVilleDeparts = "", filterVilleArrivees = "", filterPayss = "", filterProcesss = "", filterRegionReportings = "", filterProduits = "", filterComposants = "", filterCamionTypes = ""
    , filterMonths = "", filterYears = "", filterNonConformiteEtapeTypes = "", filterNonConformiteNatures = ""
    , filterIFFournisseur = false, filterIFClient = false, filterIFFournisseurRetourLot = false, filterIFFournisseurFacture = false, filterIFFournisseurAttenteBonCommande = false
    , filterIFFournisseurTransmissionFacturation = false, filterNonConformitesATraiter = false, filterNonConformitesATransmettre = false, filterNonConformitesPlanActionAValider = false
    , filterNonConformitesPlanActionNR = false
    , filterEnvCommandeFournisseurStatuts = "", filterDChargementModif = false, filterCommandesFournisseurNonChargees = false, filterValideDPrevues = false, filterDatesTransporteur = false
    , filterAnomalieCommandeFournisseur = false, filterControle = false, filterDRs = "", filterCommandesFournisseurAttribuees = false
    , augmentation = "0", selectedItem = "", moisDechargementPrevuItem = ""
    , yearFrom = "", monthFrom = "", yearTo = "", monthTo = ""
    , filterEcoOrganismes = "", filterEntiteTypes = "", filterMessageTypes = "", filterApplicationProduitOrigines = "", filterActif = false, filterNonRepartissable = true, filterCollecte = ""
    , filterDonneeEntiteSage = false, filterUtilisateurMaitres = ""
    ) {
    //Show loading indicator
    this.loadingSubject.next(true);
    //Get data and hide loading indicator
    this.gridService.getItems(module, menu, action, sortExpression, sortDirection, pageIndex, pageSize
      , filterBegin, filterEnd, filterText
      , filterClients, filterTransporteurs, filterPrestataires, filterCentreDeTris, filterIndustriels, filterDptDeparts, filterDptArrivees, filterVilleDeparts, filterVilleArrivees
      , filterPayss, filterProcesss, filterRegionReportings, filterProduits, filterComposants, filterCamionTypes
      , filterMonths, filterYears, filterNonConformiteEtapeTypes, filterNonConformiteNatures
      , filterIFFournisseur, filterIFClient, filterIFFournisseurRetourLot, filterIFFournisseurFacture, filterIFFournisseurAttenteBonCommande
      , filterIFFournisseurTransmissionFacturation, filterNonConformitesATraiter, filterNonConformitesATransmettre, filterNonConformitesPlanActionAValider
      , filterNonConformitesPlanActionNR
      , filterEnvCommandeFournisseurStatuts, filterDChargementModif, filterCommandesFournisseurNonChargees, filterValideDPrevues, filterDatesTransporteur
      , filterAnomalieCommandeFournisseur, filterControle, filterDRs, filterCommandesFournisseurAttribuees
      , augmentation, selectedItem, moisDechargementPrevuItem
      , yearFrom, monthFrom, yearTo, monthTo
      , filterEcoOrganismes, filterEntiteTypes, filterMessageTypes, filterApplicationProduitOrigines, filterActif, filterNonRepartissable
      , filterCollecte, filterDonneeEntiteSage, filterUtilisateurMaitres).pipe<any, HttpResponse<any[]>>(
        catchError(() => of([])),
        finalize(() => { this.loadingSubject.next(false); })
      )
      .subscribe((resp: HttpResponse<any[]>) => {
        let n: number = parseInt(resp.headers.get("nbRow"), 10);
        if (!isNaN(n)) { this.nbRow = n };
        this.nbAugmentation = resp.headers.get("nbAugmentation");
        this.nbValidatedPrices = resp.headers.get("nbValidatedPrices");
        this.nbRejectedPrices = resp.headers.get("nbRejectedPrices");
        this.nbDeletedPrices = resp.headers.get("nbDeletedPrices");
        this.nbSetEnAttente = resp.headers.get("nbSetEnAttente");
        this.nbChangeMoisDechargementPrevu = resp.headers.get("nbChangeMoisDechargementPrevu");
        this.nbCopyPrixReprise = resp.headers.get("nbCopyPrixReprise");
        this.nbDeactivateUtilisateur = resp.headers.get("nbDeactivateUtilisateur");
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
          //Menu columns
          if ((this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur))
            || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuModifierTransport && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur))) {
            if (!this.displayedColumns.includes("infoTransportConcurrence"))
              this.displayedColumns.unshift("infoTransportConcurrence");
          }
          if (this.applicationUserContext.currentMenu.name === MenuName.MessagerieMenuMessage) {
            if (!this.displayedColumns.includes("infoMessageVisualisations"))
              this.displayedColumns.unshift("infoMessageVisualisations");
          }
          //Select column
          if ((this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuTransportNonValide && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur))
            || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuSupprimerTransportEnMasse && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur))
            || (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
              && (this.applicationUserContext.filterEnvCommandeFournisseurStatuts && this.applicationUserContext.filterEnvCommandeFournisseurStatuts.length === 1 && this.applicationUserContext.filterEnvCommandeFournisseurStatuts[0].name === EnvCommandeFournisseurStatutName.DemandeEnlevement)
              && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur))
            || this.applicationUserContext.currentMenu.name === MenuName.AdministrationMenuUtilisateurInactif
          ) {
            if (!this.displayedColumns.includes("select"))
            this.displayedColumns.unshift("select");
          }
          //moisDechargementPrevu column
          if (this.applicationUserContext.currentMenu.name === MenuName.LogistiqueMenuCommandeFournisseur
            && (this.applicationUserContext.filterEnvCommandeFournisseurStatuts.findIndex(x => x.name === EnvCommandeFournisseurStatutName.DemandeEnlevement) >= 0
              || this.applicationUserContext.filterEnvCommandeFournisseurStatuts.findIndex(x => x.name === EnvCommandeFournisseurStatutName.EnAttente) >= 0
              || this.applicationUserContext.filterEnvCommandeFournisseurStatuts.findIndex(x => x.name === EnvCommandeFournisseurStatutName.Ouverte) >= 0
              || this.applicationUserContext.filterEnvCommandeFournisseurStatuts.findIndex(x => x.name === EnvCommandeFournisseurStatutName.Bloquee) >= 0
            )
            && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)
          ) {
            if (!this.displayedColumns.includes("moisDechargementPrevu"))
              this.displayedColumns.push("moisDechargementPrevu");
          }
          else {
            const index = this.displayedColumns.indexOf("moisDechargementPrevu", 0);
            if (index > -1) {
              this.displayedColumns.splice(index, 1);
            }
          }
        }
        else {
          //clear existing columns and inform user
          this.columns = [];
          this.displayedColumns = this.columns.map(c => c.columnDef);
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(318), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
        //Load data 
        this.itemsSubject.next(resp.body);
        this.items = resp.body;
        ////Modify data
        //if (this.displayedColumns.includes(DataColumnName.AideComposant)){
        //  this.items.forEach((item) => {
        //    let composant: EnvComponent = this.applicationUserContext.envComponents.find(e => e.name === item.AideComposant);
        //    item.AideComposant = this.applicationUserContext.getCulturedRessourceText(composant.ressLibel);
        //  })
        //}
        //Info
        if (this.nbAugmentation) {
          this.snackBarQueueService.addMessage({ text: this.nbAugmentation + " " + this.applicationUserContext.getCulturedRessourceText(304), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
        if (this.nbValidatedPrices) {
          this.snackBarQueueService.addMessage({ text: this.nbValidatedPrices + " " + this.applicationUserContext.getCulturedRessourceText(305), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
        if (this.nbRejectedPrices) {
          this.snackBarQueueService.addMessage({ text: this.nbRejectedPrices + " " + this.applicationUserContext.getCulturedRessourceText(306), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
        if (this.nbDeletedPrices) {
          this.snackBarQueueService.addMessage({ text: this.nbDeletedPrices + " " + this.applicationUserContext.getCulturedRessourceText(307), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
        if (this.nbSetEnAttente) {
          this.snackBarQueueService.addMessage({ text: this.nbSetEnAttente + " " + this.applicationUserContext.getCulturedRessourceText(557), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
        if (this.nbChangeMoisDechargementPrevu) {
          this.snackBarQueueService.addMessage({ text: this.nbChangeMoisDechargementPrevu + " " + this.applicationUserContext.getCulturedRessourceText(558), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
        if (this.nbDeactivateUtilisateur) {
          this.snackBarQueueService.addMessage({ text: this.nbDeactivateUtilisateur + " " + this.applicationUserContext.getCulturedRessourceText(1114), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
        if (action == ActionName.CopyPrixReprise) {
          if (resp.headers.has("nbCopyPrixReprise")) {
            if (this.nbCopyPrixReprise == "-1") {
              this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(620), duration: 4000 } as appInterfaces.SnackbarMsg);
            }
            else if (this.nbCopyPrixReprise == "0") {
              this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(618), duration: 4000 } as appInterfaces.SnackbarMsg);
            }
            else {
              this.snackBarQueueService.addMessage({ text: this.nbCopyPrixReprise + " " + this.applicationUserContext.getCulturedRessourceText(619), duration: 4000 } as appInterfaces.SnackbarMsg);
            }
          }
          else {
            this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(617), duration: 4000 } as appInterfaces.SnackbarMsg);
          }
        }
      });
  }
}


