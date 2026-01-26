import { Component, OnInit, Inject,  } from "@angular/core";
import { ApplicationUserContext } from "../../globals/globals";
import moment from "moment";
import { Router, NavigationEnd } from "@angular/router";
import { MenuName, ModuleName, EnvCommandeFournisseurStatutName, HabilitationLogistique, SearchElementType, HabilitationAnnuaire, RefDocumentType } from "../../globals/enums";
import { EventEmitterService } from "../../services/event-emitter.service";
import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import * as appInterfaces from "../../interfaces/appInterfaces";
import { MatDialog } from "@angular/material/dialog";
import { MessageVisuComponent } from "../dialogs/message-visu/message-visu.component";
import { UtilsService } from "../../services/utils.service";
import { DataModelService } from "../../services/data-model.service";
import { showErrorToUser } from "../../globals/utils";
import { UntypedFormGroup, UntypedFormControl } from "@angular/forms";
import { SnackBarQueueService } from "../../services/snackbar-queue.service";
import { dashboardAnimation, searchResultAnimation } from "../../animations/animations";
import { ComponentRelativeComponent } from "../dialogs/component-relative/component-relative.component";
import { fadeInOnEnterAnimation, pulseAnimation, flashAnimation } from "angular-animations";

@Component({
    selector: "home",
    templateUrl: "./home.component.html",
    animations: [
        searchResultAnimation,
        dashboardAnimation,
        fadeInOnEnterAnimation(),
        pulseAnimation(),
        flashAnimation()
    ],
    standalone: false
})
export class HomeComponent implements OnInit {
  animSearchEntite = false;
  animSearchContactAdresse = false;
  animDashboard = false;
  currentDate: string = "";
  searchMode: boolean = false;  //Indicates if a search is currently running
  //Form
  form: UntypedFormGroup;
  formAugmentation: UntypedFormGroup;
  formPrixRepriseCopy: UntypedFormGroup;
  //Filters
  filterText: string = "";
  visibleFilterFilterText: boolean = false;
  //Search
  searchEntiteResult: number[] = [];
  searchContactAdresseResult: number[] = [];
  searchText: string;
  searchSkipEntite: number = 0;
  searchSkipContactAdresse: number = 0;
  //Dashboards
  AnomalieCommandeFournisseurCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  AnomalieCommandeFournisseurNb: string = "0";
  AnomalieCommandeFournisseurNbUrgent: string = "0";
  CommandesNonReceptionneesCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  CommandesNonReceptionneesNb: string = "0";
  CommandesNonReceptionneesNbUrgent: string = "0";
  CommandesNonChargeesCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  CommandesNonChargeesNb: string = "0";
  CommandesNonChargeesNbUrgent: string = "0";
  DatesTransporteurCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  DatesTransporteurNb: string = "0";
  DatesTransporteurNbUrgent: string = "0";
  CommandesModifieesCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  CommandesModifieesNb: string = "0";
  CommandesModifieesNbUrgent: string = "0";
  CommandesBourseAffrettementCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  CommandesBourseAffrettementNb: string = "0";
  CommandesBourseAffrettementNbUrgent: string = "0";
  RepartitionAFinaliserCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  RepartitionAFinaliserNb: string = "0";
  RepartitionAFinaliserNbUrgent: string = "0";
  MessagePourDiffusionCss: string = "boxDashboard ev-dashboard-off ev-dashboard-on-color-administration";
  MessagePourDiffusionNb: string = "0";
  MessagePourDiffusionNbUrgent: string = "0";
  NouvellesDemandesEnlevementCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  NouvellesDemandesEnlevementNb: string = "0";
  NouvellesDemandesEnlevementNbUrgent: string = "0";
  EvolutionPrixTransportCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  EvolutionPrixTransportNb: string = "0";
  EvolutionPrixTransportNbUrgent: string = "0";
  CommandesEnCoursCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  CommandesEnCoursNb: string = "0";
  CommandesEnCoursNbUrgent: string = "0";
  ContactModificationRequestCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-annuaire";
  ContactModificationRequestNb: string = "0";
  ContactModificationRequestNbUrgent: string = "0";
  DonneeEntiteSageCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-annuaire";
  DonneeEntiteSageNb: string = "0";
  DonneeEntiteSageNbUrgent: string = "0";
  CommandesFournisseurAttribueesCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-logistique";
  CommandesFournisseurAttribueesNb: string = "0";
  CommandesFournisseurAttribueesNbUrgent: string = "0";
  NouvellesNonConformitesCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NouvellesNonConformitesNb: string = "0";
  NouvellesNonConformitesNbUrgent: string = "0";
  NonConformitesEnCoursCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NonConformitesEnCoursNb: string = "0";
  NonConformitesEnCoursNbUrgent: string = "0";
  NonConformitesATraiterCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NonConformitesATraiterNb: string = "0";
  NonConformitesATraiterNbUrgent: string = "0";
  NonConformitesATransmettreCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NonConformitesATransmettreNb: string = "0";
  NonConformitesATransmettreNbUrgent: string = "0";
  NonConformitesAValiderCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NonConformitesAValiderNb: string = "0";
  NonConformitesAValiderNbUrgent: string = "0";
  NonConformitesEnAttentePlanActionCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NonConformitesEnAttentePlanActionNb: string = "0";
  NonConformitesEnAttentePlanActionNbUrgent: string = "0";
  NonConformitesPlanActionAValiderCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NonConformitesPlanActionAValiderNb: string = "0";
  NonConformitesPlanActionAValiderNbUrgent: string = "0";
  NonConformitesAFacturerCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NonConformitesAFacturerNb: string = "0";
  NonConformitesAFacturerNbUrgent: string = "0";
  NonConformitesACloturerCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NonConformitesACloturerNb: string = "0";
  NonConformitesACloturerNbUrgent: string = "0";
  NonConformitesEnAttenteFactureCss: string = "boxDashboard ev-dashboard-off ev-dashboard-off-color-qualite";
  NonConformitesEnAttenteFactureNb: string = "0";
  NonConformitesEnAttenteFactureNbUrgent: string = "0";
  //Labels
  NouvellesDemandesEnlevementCaption: string = "";
    downloadService: any;
    entite: any;
  //Constructor
  constructor(public applicationUserContext: ApplicationUserContext
    , private http: HttpClient, @Inject("BASE_URL") private baseUrl: string
    , private eventEmitterService: EventEmitterService
    , private router: Router
    , public dialog: MatDialog
    , public utilsService: UtilsService
    , public dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
  ) {
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
    this.currentDate = moment().format("dddd D MMMM YYYY");
  }
  //------------------------------------------------------------------------------
  //On init
  ngOnInit() {
    //Unlock all data for current user
    this.utilsService.unlockAllData().subscribe(result => {
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check for messages to show today
    this.dataModelService.existsMessageToDisplay(true)
      .subscribe(result => {
        if (result>0) {
          this.showMessage();
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check for messages
    this.dataModelService.existsMessageToDisplay(false)
      .subscribe(result => {
          this.applicationUserContext.hasMessage = result;
        if (this.applicationUserContext.hasMessage>0) { this.applicationUserContext.animMessage = true; }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Set form
    this.form = new UntypedFormGroup({
      FilterText: new UntypedFormControl(null)
    });
    //ManageScreen
    this.ManageScreen();
    //Fetch data for all dashboard items
    var url = this.baseUrl + "evapi/dashboard/getitem";
    //Warning in CommandeFournisseur
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "AnomalieCommandeFournisseur"),
      responseType: "json"
    }).subscribe(result => {
      this.AnomalieCommandeFournisseurNb = result.nb.toString();
      this.AnomalieCommandeFournisseurNbUrgent = result.nbUrgent.toString();
      this.AnomalieCommandeFournisseurCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.AnomalieCommandeFournisseurCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.AnomalieCommandeFournisseurCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.AnomalieCommandeFournisseurCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Commandes non chargées
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "CommandesNonChargees"),
      responseType: "json"
    }).subscribe(result => {
      this.CommandesNonChargeesNb = result.nb.toString();
      this.CommandesNonChargeesNbUrgent = result.nbUrgent.toString();
      this.CommandesNonChargeesCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.CommandesNonChargeesCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.CommandesNonChargeesCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.CommandesNonChargeesCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Commandes non réceptionnées
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "CommandesNonReceptionnees"),
      responseType: "json"
    }).subscribe(result => {
      this.CommandesNonReceptionneesNb = result.nb.toString();
      this.CommandesNonReceptionneesNbUrgent = result.nbUrgent.toString();
      this.CommandesNonReceptionneesCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.CommandesNonReceptionneesCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.CommandesNonReceptionneesCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.CommandesNonReceptionneesCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Dates transporteur
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "DatesTransporteur"),
      responseType: "json"
    }).subscribe(result => {
      this.DatesTransporteurNb = result.nb.toString();
      this.DatesTransporteurNbUrgent = result.nbUrgent.toString();
      this.DatesTransporteurCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.DatesTransporteurCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.DatesTransporteurCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.DatesTransporteurCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Commandes bourse d'affrètement
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "CommandesBourseAffrettement"),
      responseType: "json"
    }).subscribe(result => {
      this.CommandesBourseAffrettementNb = result.nb.toString();
      this.CommandesBourseAffrettementNbUrgent = result.nbUrgent.toString();
      this.CommandesBourseAffrettementCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.CommandesBourseAffrettementCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.CommandesBourseAffrettementCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.CommandesBourseAffrettementCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Commandes modifiées
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "CommandesModifiees"),
      responseType: "json"
    }).subscribe(result => {
      this.CommandesModifieesNb = result.nb.toString();
      this.CommandesModifieesNbUrgent = result.nbUrgent.toString();
      this.CommandesModifieesCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.CommandesModifieesCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.CommandesModifieesCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.CommandesModifieesCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Commandes en cours
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "CommandesEnCours"),
      responseType: "json"
    }).subscribe(result => {
      this.CommandesEnCoursNb = result.nb.toString();
      this.CommandesEnCoursNbUrgent = result.nbUrgent.toString();
      this.CommandesEnCoursCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.CommandesEnCoursCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.CommandesEnCoursCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.CommandesEnCoursCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Répartition à finaliser
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "RepartitionAFinaliser"),
      responseType: "json"
    }).subscribe(result => {
      this.RepartitionAFinaliserNb = result.nb.toString();
      this.RepartitionAFinaliserNbUrgent = result.nbUrgent.toString();
      this.RepartitionAFinaliserCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.RepartitionAFinaliserCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.RepartitionAFinaliserCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.RepartitionAFinaliserCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Messages
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "MessagePourDiffusion"),
      responseType: "json"
    }).subscribe(result => {
      this.MessagePourDiffusionNb = result.nb.toString();
      this.MessagePourDiffusionNbUrgent = result.nbUrgent.toString();
      this.MessagePourDiffusionCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.MessagePourDiffusionCss += " ev-dashboard-on-warning ev-dashboard-on-color-administration ev-dashboard-click"; }
      else if (result.nb > 0) { this.MessagePourDiffusionCss += " ev-dashboard-on ev-dashboard-on-color-administration ev-dashboard-click"; }
      else { this.MessagePourDiffusionCss += " ev-dashboard-off ev-dashboard-off-color-administration"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //New demandes d'enlèvement
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NouvellesDemandesEnlevement"),
      responseType: "json"
    }).subscribe(result => {
      this.NouvellesDemandesEnlevementNb = result.nb.toString();
      this.NouvellesDemandesEnlevementNbUrgent = result.nbUrgent.toString();
      this.NouvellesDemandesEnlevementCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NouvellesDemandesEnlevementCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.NouvellesDemandesEnlevementCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.NouvellesDemandesEnlevementCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //New carrier prices
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "EvolutionPrixTransport"),
      responseType: "json"
    }).subscribe(result => {
      this.EvolutionPrixTransportNb = result.nb.toString();
      this.EvolutionPrixTransportNbUrgent = result.nbUrgent.toString();
      this.EvolutionPrixTransportCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.EvolutionPrixTransportCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.EvolutionPrixTransportCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.EvolutionPrixTransportCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Contacts modification requests
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "ContactModificationRequest"),
      responseType: "json"
    }).subscribe(result => {
      this.ContactModificationRequestNb = result.nb.toString();
      this.ContactModificationRequestNbUrgent = result.nbUrgent.toString();
      this.ContactModificationRequestCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.ContactModificationRequestCss += " ev-dashboard-on-warning ev-dashboard-on-color-annuaire ev-dashboard-click"; }
      else if (result.nb > 0) { this.ContactModificationRequestCss += " ev-dashboard-on ev-dashboard-on-color-annuaire ev-dashboard-click"; }
      else { this.ContactModificationRequestCss += " ev-dashboard-off ev-dashboard-off-color-annuaire"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Données SAGE divergentes
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "DonneeEntiteSage"),
      responseType: "json"
    }).subscribe(result => {
      this.DonneeEntiteSageNb = result.nb.toString();
      this.DonneeEntiteSageNbUrgent = result.nbUrgent.toString();
      this.DonneeEntiteSageCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.DonneeEntiteSageCss += " ev-dashboard-on-warning ev-dashboard-on-color-annuaire ev-dashboard-click"; }
      else if (result.nb > 0) { this.DonneeEntiteSageCss += " ev-dashboard-on ev-dashboard-on-color-annuaire ev-dashboard-click"; }
      else { this.DonneeEntiteSageCss += " ev-dashboard-off ev-dashboard-off-color-annuaire"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Commandes modifiées
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "CommandesFournisseurAttribuees"),
      responseType: "json"
    }).subscribe(result => {
      this.CommandesFournisseurAttribueesNb = result.nb.toString();
      this.CommandesFournisseurAttribueesNbUrgent = result.nbUrgent.toString();
      this.CommandesFournisseurAttribueesCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.CommandesFournisseurAttribueesCss += " ev-dashboard-on-warning ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else if (result.nb > 0) { this.CommandesFournisseurAttribueesCss += " ev-dashboard-on ev-dashboard-on-color-logistique ev-dashboard-click"; }
      else { this.CommandesFournisseurAttribueesCss += " ev-dashboard-off ev-dashboard-off-color-logistique"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //New non conformités
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NouvellesNonConformites"),
      responseType: "json"
    }).subscribe(result => {
      this.NouvellesNonConformitesNb = result.nb.toString();
      this.NouvellesNonConformitesNbUrgent = result.nbUrgent.toString();
      this.NouvellesNonConformitesCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NouvellesNonConformitesCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NouvellesNonConformitesCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NouvellesNonConformitesCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Non conformités en cours
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NonConformitesEnCours"),
      responseType: "json"
    }).subscribe(result => {
      this.NonConformitesEnCoursNb = result.nb.toString();
      this.NonConformitesEnCoursNbUrgent = result.nbUrgent.toString();
      this.NonConformitesEnCoursCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NonConformitesEnCoursCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NonConformitesEnCoursCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NonConformitesEnCoursCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Non conformité à traiter
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NonConformitesATraiter"),
      responseType: "json"
    }).subscribe(result => {
      this.NonConformitesATraiterNb = result.nb.toString();
      this.NonConformitesATraiterNbUrgent = result.nbUrgent.toString();
      this.NonConformitesATraiterCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NonConformitesATraiterCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NonConformitesATraiterCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NonConformitesATraiterCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Non conformité à Transmettre
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NonConformitesATransmettre"),
      responseType: "json"
    }).subscribe(result => {
      this.NonConformitesATransmettreNb = result.nb.toString();
      this.NonConformitesATransmettreNbUrgent = result.nbUrgent.toString();
      this.NonConformitesATransmettreCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NonConformitesATransmettreCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NonConformitesATransmettreCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NonConformitesATransmettreCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //NC à valider
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NonConformitesAValider"),
      responseType: "json"
    }).subscribe(result => {
      this.NonConformitesAValiderNb = result.nb.toString();
      this.NonConformitesAValiderNbUrgent = result.nbUrgent.toString();
      this.NonConformitesAValiderCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NonConformitesAValiderCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NonConformitesAValiderCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NonConformitesAValiderCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //NC en attente plan action
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NonConformitesEnAttentePlanAction"),
      responseType: "json"
    }).subscribe(result => {
      this.NonConformitesEnAttentePlanActionNb = result.nb.toString();
      this.NonConformitesEnAttentePlanActionNbUrgent = result.nbUrgent.toString();
      this.NonConformitesEnAttentePlanActionCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NonConformitesEnAttentePlanActionCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NonConformitesEnAttentePlanActionCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NonConformitesEnAttentePlanActionCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //NC plan d'action à valider
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NonConformitesPlanActionAValider"),
      responseType: "json"
    }).subscribe(result => {
      this.NonConformitesPlanActionAValiderNb = result.nb.toString();
      this.NonConformitesPlanActionAValiderNbUrgent = result.nbUrgent.toString();
      this.NonConformitesPlanActionAValiderCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NonConformitesPlanActionAValiderCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NonConformitesPlanActionAValiderCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NonConformitesPlanActionAValiderCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //NC à facturer
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NonConformitesAFacturer"),
      responseType: "json"
    }).subscribe(result => {
      this.NonConformitesAFacturerNb = result.nb.toString();
      this.NonConformitesAFacturerNbUrgent = result.nbUrgent.toString();
      this.NonConformitesAFacturerCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NonConformitesAFacturerCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NonConformitesAFacturerCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NonConformitesAFacturerCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //NC à cloturer
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NonConformitesACloturer"),
      responseType: "json"
    }).subscribe(result => {
      this.NonConformitesACloturerNb = result.nb.toString();
      this.NonConformitesACloturerNbUrgent = result.nbUrgent.toString();
      this.NonConformitesACloturerCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NonConformitesACloturerCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NonConformitesACloturerCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NonConformitesACloturerCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //NC en attente facture
    this.http.get<appInterfaces.DashboardItem>(url, {
      headers: new HttpHeaders()
        .set("itemName", "NonConformitesEnAttenteFacture"),
      responseType: "json"
    }).subscribe(result => {
      this.NonConformitesEnAttenteFactureNb = result.nb.toString();
      this.NonConformitesEnAttenteFactureNbUrgent = result.nbUrgent.toString();
      this.NonConformitesEnAttenteFactureCss = "boxDashboard";
      if (result.nbUrgent > 0) { this.NonConformitesEnAttenteFactureCss += " ev-dashboard-on-warning ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else if (result.nb > 0) { this.NonConformitesEnAttenteFactureCss += " ev-dashboard-on ev-dashboard-on-color-qualite ev-dashboard-click"; }
      else { this.NonConformitesEnAttenteFactureCss += " ev-dashboard-off ev-dashboard-off-color-qualite"; }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.animDashboard = true;
    //Test
    //this.getTestDocumentTypeFile();
  }
  //-----------------------------------------------------------------------------------
  //Download a file
  getTestDocumentTypeFile() {
    this.dataModelService.getEntiteDocumentType(2945, RefDocumentType.ReportingCollectiviteElu, 2025)
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = "ReportingCollectiviteElu.pdf";
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
  //------------------------------------------------------------------------------
  //Manage screen
  ManageScreen() {
    this.visibleFilterFilterText = (
      this.applicationUserContext.connectedUtilisateur.Client?.RefEntite == null
      && this.applicationUserContext.connectedUtilisateur.CentreDeTri?.RefEntite == null
      && this.applicationUserContext.connectedUtilisateur.Transporteur?.RefEntite == null
      && this.applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite == null
    );
    if (this.applicationUserContext.connectedUtilisateur.CentreDeTri) {
      this.NouvellesDemandesEnlevementCaption = this.applicationUserContext.getCulturedRessourceText(1433)
    }
    else {
      this.NouvellesDemandesEnlevementCaption = this.applicationUserContext.getCulturedRessourceText(553)
    }
  }
  //-----------------------------------------------------------------------------------
  //Apply filters
  applyFilter(nextEntite: boolean, nextContactAdresse: boolean) {
    //Init
    this.searchMode = true;
    this.searchText = this.form.get("FilterText").value;
    //Search if applicable
    if (nextEntite || (!nextEntite && !nextContactAdresse)) {
      //Reset results if new search
      if (!nextEntite) { this.searchEntiteResult = []; this.searchSkipEntite = 0; }
      this.utilsService.getSearchResult(this.searchText, SearchElementType.Entite, this.searchSkipEntite).subscribe(result => {
        if (result && result.length > 0) {
          //Update skip for next search
          this.searchSkipEntite += (result.length > 10 ? 10 : result.length);
          //Add or replace search result depending on user choice
          if (!nextEntite) { this.searchEntiteResult = result; }
          else { this.searchEntiteResult.pop(); this.searchEntiteResult = this.searchEntiteResult.concat(result); }
          //Set last result if more than allowed results
          if (result.length > 10) { this.searchEntiteResult[this.searchEntiteResult.length - 1] = -1; }
          this.animSearchEntite = !this.animSearchEntite;
        }
        else {
          //Inform user if no result
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1074), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    //Search if applicable
    if (nextContactAdresse || (!nextEntite && !nextContactAdresse)) {
      //Reset results if new search
      if (!nextContactAdresse) { this.searchContactAdresseResult = []; this.searchSkipContactAdresse = 0; }
      this.utilsService.getSearchResult(this.searchText, SearchElementType.ContactAdresse, this.searchSkipContactAdresse).subscribe(result => {
        if (result && result.length > 0) {
          //Update skip for next search
          this.searchSkipContactAdresse += (result.length > 10 ? 10 : result.length);
          //Add or replace search result depending on user choice
          if (!nextContactAdresse) { this.searchContactAdresseResult = result; }
          else { this.searchContactAdresseResult.pop(); this.searchContactAdresseResult = this.searchContactAdresseResult.concat(result); }
          //Set last result if more than allowed results
          if (result.length > 10) { this.searchContactAdresseResult[this.searchContactAdresseResult.length - 1] = -1; }
          this.animSearchContactAdresse = !this.animSearchContactAdresse;
        }
        else {
          //Inform user if no result
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1075), duration: 4000 } as appInterfaces.SnackbarMsg);
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //------------------------------------------------------------------------------
  //Reset filter
  public onResetFilter() {
    this.form.get("FilterText").setValue(null);
    this.searchText = "";
    this.searchMode = false;
   }
  //------------------------------------------------------------------------------
  //Navigation to menu for search results
  OpenSearchResult(elementType: string, elementRef: number) {
    if (elementType && elementRef) {
      //If search for more
      if (elementRef === -1) {
        switch (elementType) {
          case SearchElementType.Entite:
            this.applyFilter(true, false);
            break;
          case SearchElementType.ContactAdresse:
            this.applyFilter(false, true);
            break;
        }
      }
      if (elementRef >= 0) {
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Annuaire);
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.AnnuaireMenuEntite);
        //Emit event for app compoment refresh (active menus and modules)
        this.eventEmitterService.onDashboardItemClick();
        //Navigate
        this.router.navigate(["entite", elementRef, { elementType: elementType }])
      }
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  AnomalieCommandeFournisseurClick() {
    if (this.AnomalieCommandeFournisseurNb !== "0" || this.AnomalieCommandeFournisseurNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
      this.applicationUserContext.filterAnomalieCommandeFournisseur = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  CommandesNonChargeesClick() {
    if (this.CommandesNonChargeesNb !== "0" || this.CommandesNonChargeesNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      this.applicationUserContext.filterCommandesFournisseurNonChargees = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  CommandesNonReceptionneesClick() {
    if (this.CommandesNonReceptionneesNb !== "0" || this.CommandesNonReceptionneesNbUrgent !== "0") {
      if (this.applicationUserContext.connectedUtilisateur.Prestataire) {
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.ModulePrestataire);
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.ModulePrestataireMenuCommandeFournisseur);
      }
      else {
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
      }
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
      //this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      this.applicationUserContext.filterValideDPrevues = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  DatesTransporteurClick() {
    if (this.DatesTransporteurNb !== "0" || this.DatesTransporteurNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      this.applicationUserContext.filterDatesTransporteur = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  CommandesEnCoursClick() {
    if (this.CommandesEnCoursNb !== "0" || this.CommandesEnCoursNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
      //RAZ filters
      this.applicationUserContext.razFilters();
      if (this.applicationUserContext.connectedUtilisateur.Transporteur) {
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuTransportCommandeEnCours);
        //Set filter
        this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
        this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      }
      else {
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
        //Set filter
        this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
        this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.EnAttente));
        this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      }
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  CommandesModifieesClick() {
    if (this.CommandesModifieesNb !== "0" || this.CommandesModifieesNbUrgent !== "0") {
      if (this.applicationUserContext.connectedUtilisateur.Prestataire) {
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.ModulePrestataire);
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.ModulePrestataireMenuCommandeFournisseur);
      }
      else {
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
        if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur) {
          this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuTransportCommandeModifiee);
        }
        else {
          this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
        }
      }
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      this.applicationUserContext.filterDChargementModif = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  CommandesBourseAffrettementClick() {
    if (this.CommandesBourseAffrettementNb !== "0" || this.CommandesBourseAffrettementNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuTransportDemandeEnlevement);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  RepartitionAFinaliserClick() {
    if (this.RepartitionAFinaliserNb !== "0" || this.RepartitionAFinaliserNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuRepartition);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterRepartitionAFinaliser = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  MessagePourDiffusionClick() {
    if (this.MessagePourDiffusionNb !== "0" || this.MessagePourDiffusionNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Messagerie);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.MessagerieMenuMessage);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NouvellesDemandesEnlevementClick() {
    if (this.NouvellesDemandesEnlevementNb !== "0" || this.NouvellesDemandesEnlevementNbUrgent !== "0") {
      if (this.applicationUserContext.connectedUtilisateur.Prestataire) {
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.ModulePrestataire);
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.ModulePrestataireMenuCommandeFournisseur);
      }
      else {
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
      }
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.DemandeEnlevement));
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  EvolutionPrixTransportClick() {
    if (this.EvolutionPrixTransportNb !== "0" || this.EvolutionPrixTransportNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuTransportNonValide);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  CommandesFournisseurAttribueesClick() {
    if (this.CommandesFournisseurAttribueesNb !== "0" || this.CommandesFournisseurAttribueesNbUrgent !== "0") {
      if (this.applicationUserContext.connectedUtilisateur.Prestataire) {
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.ModulePrestataire);
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.ModulePrestataireMenuCommandeFournisseur);
      }
      else {
        this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
        this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
      }
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts = [];
      this.applicationUserContext.filterEnvCommandeFournisseurStatuts.push(this.applicationUserContext.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      this.applicationUserContext.filterCommandesFournisseurAttribuees = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  ContactModificationRequestClick() {
    if ((this.ContactModificationRequestNb !== "0" || this.ContactModificationRequestNbUrgent !== "0")
      && (this.applicationUserContext.connectedUtilisateur.HabilitationAnnuaire == HabilitationAnnuaire.Administrateur
        || this.applicationUserContext.connectedUtilisateur.HabilitationAnnuaire == HabilitationAnnuaire.Utilisateur)
    ) {
      let title = this.applicationUserContext.getCulturedRessourceText(438);
      let data = {
        title: title, type: "ContactModificationRequest"
      };
      //Open dialog
      this.dialog.open(ComponentRelativeComponent, {
        width: "850px",
        data,
        autoFocus: false,
        restoreFocus: false,
      });
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  DonneeEntiteSageClick() {
    if ((this.DonneeEntiteSageNb !== "0" || this.DonneeEntiteSageNbUrgent !== "0")
      && (this.applicationUserContext.connectedUtilisateur.HabilitationAnnuaire == HabilitationAnnuaire.Administrateur)
    ) {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Annuaire);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.AnnuaireMenuEntite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterDonneeEntiteSage = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NouvellesNonConformitesClick() {
    if (this.NouvellesNonConformitesNb !== "0" || this.NouvellesNonConformitesNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformiteEtapeTypes = [];
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 1));
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NonConformitesEnCoursClick() {
    if (this.NonConformitesEnCoursNb !== "0" || this.NonConformitesEnCoursNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformiteEtapeTypes = [];
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 2));
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 3));
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 4));
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 5));
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 6));
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 7));
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 8));
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NonConformitesATraiterClick() {
    if (this.NonConformitesATraiterNb !== "0" || this.NonConformitesATraiterNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformitesATraiter = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NonConformitesATransmettreClick() {
    if (this.NonConformitesATransmettreNb !== "0" || this.NonConformitesATransmettreNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformitesATransmettre = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NonConformitesAValiderClick() {
    if (this.NonConformitesAValiderNb !== "0" || this.NonConformitesAValiderNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformiteEtapeTypes = [];
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 4));
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NonConformitesEnAttentePlanActionClick() {
    if (this.NonConformitesEnAttentePlanActionNb !== "0" || this.NonConformitesEnAttentePlanActionNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformiteEtapeTypes = [];
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 5));
      this.applicationUserContext.filterNonConformitesPlanActionNR = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NonConformitesPlanActionAValiderClick() {
    if (this.NonConformitesPlanActionAValiderNb !== "0" || this.NonConformitesPlanActionAValiderNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformitesPlanActionAValider = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NonConformitesAFacturerClick() {
    if (this.NonConformitesAFacturerNb !== "0" || this.NonConformitesAFacturerNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformiteEtapeTypes = [];
      this.applicationUserContext.filterIFFournisseurFacture = true;
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NonConformitesACloturerClick() {
    if (this.NonConformitesACloturerNb !== "0" || this.NonConformitesACloturerNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformiteEtapeTypes = [];
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 8));
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  NonConformitesEnAttenteFactureClick() {
    if (this.NonConformitesEnAttenteFactureNb !== "0" || this.NonConformitesEnAttenteFactureNbUrgent !== "0") {
      this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
      this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
      //RAZ filters
      this.applicationUserContext.razFilters();
      //Set filter
      this.applicationUserContext.filterNonConformiteEtapeTypes = [];
      this.applicationUserContext.filterNonConformiteEtapeTypes.push(this.applicationUserContext.nonConformiteEtapeTypes.find(x => x.RefNonConformiteEtapeType === 7));
      //Navigate
      this.router.navigate(["grid"]);
      //Emit event for app compoment refresh (active menus and modules)
      this.eventEmitterService.onDashboardItemClick();
    }
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for messages
  showMessage() {
    //open pop-up
    const dialogRef = this.dialog.open(MessageVisuComponent, {
      width: "50%", height: "50%",
      data: { mode: "unred" },
      autoFocus: false,
      closeOnNavigation: false,
      disableClose: true,
      restoreFocus: false
    });
  }
}
