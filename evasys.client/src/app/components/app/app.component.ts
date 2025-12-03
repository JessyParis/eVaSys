/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/06/2018
/// ----------------------------------------------------------------------------------------------------- 
import { Component, OnInit, inject, signal } from "@angular/core";
import { Router, Event, NavigationEnd, RouterOutlet, ActivatedRoute } from "@angular/router";
import { AuthService } from "../../services/auth.service";
import { ApplicationUserContext } from "../../globals/globals"
import { slideInAnimation, modulesAnimation } from "../../animations/animations";
import { EventEmitterService } from "../../services/event-emitter.service";
import { MenuName, DocumentType, UtilisateurType } from "../../globals/enums";
import * as appClasses from "../../classes/appClasses";
import { DataModelService } from "../../services/data-model.service";
import { changeCulture, deleteCookie, getAttachmentFilename, getCookie, showErrorToUser } from "../../globals/utils";
import { MatDialog } from "@angular/material/dialog";
import { MatCheckboxChange } from "@angular/material/checkbox";
import { ComponentRelativeComponent } from "../dialogs/component-relative/component-relative.component";
import { SnackBarQueueService } from "../../services/snackbar-queue.service";
import { DownloadService } from "../../services/download.service";
import { MessageVisuComponent } from "../dialogs/message-visu/message-visu.component";
import { SnackbarMsg } from "../../interfaces/appInterfaces";
import { fadeInOnEnterAnimation, fadeOutOnLeaveAnimation, flashAnimation, slideInLeftAnimation } from "angular-animations";
import * as dataModelsInterfaces from "../../interfaces/dataModelsInterfaces";
import { HttpResponse } from "@angular/common/http";
import { DateAdapter, MAT_DATE_LOCALE } from '@angular/material/core';
//import { UpdateService } from "../../services/update.service";

@Component({
    selector: "app",
    templateUrl: "./app.component.html",
    styleUrls: ["./app.component.scss"],
    animations: [
        fadeInOnEnterAnimation(),
        fadeOutOnLeaveAnimation(),
        slideInLeftAnimation(),
        flashAnimation(), slideInAnimation, modulesAnimation
    ],
    standalone: false
})
export class AppComponent implements OnInit {
  menuModules: boolean = true;
  logistiqueModuleStyle: string = "ev-mod&ule";
  annuaireModuleStyle: string = "ev-module";
  messagerieModuleStyle: string = "ev-module";
  modulePrestataireModuleStyle: string = "ev-module";
  moduleCollectiviteModuleStyle: string = "ev-module";
  administrationModuleStyle: string = "ev-module";
  qualiteModuleStyle: string = "ev-module";
  administrationMenuUtilisateurStyle: string = "ev-menu";
  administrationMenuSecuriteStyle: string = "ev-menu";
  administrationMenuUtilisateurInactifStyle: string = "ev-menu";
  annuaireMenuEmailNoteCreditCollectiviteStyle: string = "ev-menu";
  annuaireMenuSuiviEnvoisStyle: string = "ev-menu";
  annuaireMenuEntiteStyle: string = "ev-menu";
  annuaireMenuExtractionActionStyle: string = "ev-menu";
  annuaireMenuExtractionAnnuaireStyle: string = "ev-menu";
  annuaireMenuNbAffretementTransporteurStyle: string = "ev-menu";
  annuaireMenuEvolutionTonnageStyle: string = "ev-menu";
  administrationMenuActionTypeStyle = "ev-menu";
  administrationMenuAdresseTypeStyle = "ev-menu";
  administrationMenuAideStyle: string = "ev-menu";
  administrationMenuApplicationStyle = "ev-menu";
  administrationMenuApplicationProduitOrigineStyle = "ev-menu";
  administrationMenuCamionTypeStyle = "ev-menu";
  administrationMenuCiviliteStyle: string = "ev-menu";
  administrationMenuClientApplicationStyle: string = "ev-menu";
  administrationMenuDescriptionControleStyle: string = "ev-menu";
  administrationMenuDescriptionCVQStyle: string = "ev-menu";
  administrationMenuDescriptionReceptionStyle: string = "ev-menu";
  administrationMenuDocumentStyle: string = "ev-menu";
  administrationMenuEcoOrganismeStyle: string = "ev-menu";
  administrationMenuEntiteTypeStyle: string = "ev-menu";
  administrationMenuEquipementierStyle: string = "ev-menu";
  administrationMenuEquivalentCO2Style: string = "ev-menu";
  administrationMenuExtractionLoginStyle = "ev-menu";
  administrationMenuFonctionStyle: string = "ev-menu";
  administrationMenuFormeContactStyle: string = "ev-menu";
  administrationMenuFournisseurTOStyle: string = "ev-menu";
  administrationMenuJourFerieStyle: string = "ev-menu";
  administrationMenuMessageTypeStyle: string = "ev-menu";
  administrationMenuModeTransportEEStyle: string = "ev-menu";
  administrationMenuMontantIncitationQualiteStyle: string = "ev-menu";
  administrationMenuMotifAnomalieChargementStyle: string = "ev-menu";
  administrationMenuMotifAnomalieClientStyle: string = "ev-menu";
  administrationMenuMotifAnomalieTransporteurStyle: string = "ev-menu";
  administrationMenuMotifCamionIncompletStyle: string = "ev-menu";
  administrationMenuNonConformiteDemandeClientTypeStyle: string = "ev-menu";
  administrationMenuNonConformiteFamilleStyle: string = "ev-menu";
  administrationMenuNonConformiteNatureStyle: string = "ev-menu";
  administrationMenuNonConformiteReponseClientTypeStyle: string = "ev-menu";
  administrationMenuNonConformiteReponseFournisseurTypeStyle: string = "ev-menu";
  administrationMenuParamEmailStyle = "ev-menu";
  administrationMenuParametreStyle: string = "ev-menu";
  administrationMenuPaysStyle: string = "ev-menu";
  administrationMenuProcessStyle: string = "ev-menu";
  administrationMenuProduitStyle: string = "ev-menu";
  administrationMenuProduitGroupeReportingTypeStyle: string = "ev-menu";
  administrationMenuProduitGroupeReportingStyle: string = "ev-menu";
  administrationMenuRegionEEStyle = "ev-menu";
  administrationMenuRegionReportingStyle: string = "ev-menu";
  administrationMenuRepreneurStyle: string = "ev-menu";
  administrationMenuRepriseTypeStyle: string = "ev-menu";
  administrationMenuRessourceStyle: string = "ev-menu";
  administrationMenuSAGECodeTransportStyle: string = "ev-menu";
  administrationMenuServiceStyle: string = "ev-menu";
  administrationMenuStandardStyle: string = "ev-menu";
  administrationMenuSuiviLoginStyle = "ev-menu";
  administrationMenuTicketStyle: string = "ev-menu";
  administrationMenuTitreStyle: string = "ev-menu";
  logistiqueMenuChargementAnnuleStyle: string = "ev-menu";
  logistiqueMenuCommandeClientStyle: string = "ev-menu";
  logistiqueMenuCommandeFournisseurStyle: string = "ev-menu";
  logistiqueMenuRepartitionStyle: string = "ev-menu";
  logistiqueMenuImpressionStyle: string = "ev-menu";
  logistiqueMenuExportSAGEStyle: string = "ev-menu";
  logistiqueMenuPrixRepriseStyle: string = "ev-menu";
  logistiqueMenuTransportNonValideStyle: string = "ev-menu";
  logistiqueMenuModifierTransportStyle: string = "ev-menu";
  logistiqueMenuImporterTransportStyle: string = "ev-menu";
  logistiqueMenuSupprimerTransportEnMasseStyle: string = "ev-menu";
  logistiqueMenuValiderNouveauPrixTransportStyle: string = "ev-menu";
  logistiqueMenuModifierTousPrixTransportStyle: string = "ev-menu";
  logistiqueMenuSurcoutCarburantStyle: string = "ev-menu";
  logistiqueMenuTransportDemandeEnlevementStyle: string = "ev-menu";
  logistiqueMenuTransportCommandeEnCoursStyle: string = "ev-menu";
  logistiqueMenuTransportCommandeModifieesStyle: string = "ev-menu";
  logistiqueMenuTransportCommandeStyle: string = "ev-menu";
  logistiqueMenuEtatCoutTransportStyle: string = "ev-menu";
  logistiqueMenuLotDisponibleStyle: string = "ev-menu";
  logistiqueMenuEtatReceptionFournisseurChargementStyle: string = "ev-menu";
  logistiqueMenuEtatReceptionStyle: string = "ev-menu";
  logistiqueMenuSuiviCommandeClientStyle: string = "ev-menu";
  logistiqueMenuDelaiCommandeEnlevementStyle: string = "ev-menu";
  logistiqueMenuPoidsMoyenChargementProduitStyle: string = "ev-menu";
  logistiqueMenuEtatDesEnlevementsStyle: string = "ev-menu";
  logistiqueMenuExtractionLekoStyle: string = "ev-menu";
  logistiqueMenuSuiviCommandeClientProduitStyle: string = "ev-menu";
  logistiqueMenuEtatReceptionProduitStyle: string = "ev-menu";
  logistiqueMenuEtatReceptionProduitDRStyle: string = "ev-menu";
  logistiqueMenuExtractionReceptionStyle: string = "ev-menu";
  logistiqueMenuExtractionCommandeStyle: string = "ev-menu";
  logistiqueMenuEtatDesPoidsStyle: string = "ev-menu";
  logistiqueMenuEtatTonnageParProcessStyle: string = "ev-menu";
  logistiqueMenuEtatVenteAnnuelleProduitClientStyle: string = "ev-menu";
  logistiqueMenuEtatDestinationAnnuelleProduitClientStyle: string = "ev-menu";
  logistiqueMenuSuiviFacturationHCStyle: string = "ev-menu";
  logistiqueMenuTonnageCollectiviteProduitStyle: string = "ev-menu";
  logistiqueMenuTonnageCollectiviteCDTProduitStyle: string = "ev-menu";
  logistiqueMenuTonnageCDTProduitComposantStyle: string = "ev-menu";
  logistiqueMenuTonnageCLSousContratStyle: string = "ev-menu";
  logistiqueMenuListeProduitStyle: string = "ev-menu";
  logistiqueMenuEtatTarifTransporteurClientStyle: string = "ev-menu";
  logistiqueMenuPartMarcheTransporteurStyle: string = "ev-menu";
  logistiqueMenuEtatKmMoyenStyle: string = "ev-menu";
  logistiqueMenuEtatReceptionEmballagePlastiqueStyle: string = "ev-menu";
  logistiqueMenuEtatSuiviTonnageRecycleurStyle: string = "ev-menu";
  logistiqueMenuEtatDesFluxDevStyle: string = "ev-menu";
  logistiqueMenuExtractionOscarStyle: string = "ev-menu";
  logistiqueMenuEtatDesFluxDevLekoStyle: string = "ev-menu";
  menuVisualisationAnnuaireStyle: string = "ev-menu";
  menuDocumentsStyle: string = "ev-menu";
  messagerieMenuMessageStyle: string = "ev-menu";
  messagerieMenuVisualisationStyle: string = "ev-menu";
  modulePrestataireMenuCommandeFournisseurStyle: string = "ev-menu";
  moduleCollectiviteMenuAccueilStyle: string = "ev-menu";
  moduleCollectiviteMenuDocumentsStyle: string = "ev-menu";
  moduleCollectiviteMenuPrixRepriseStyle: string = "ev-menu";
  moduleCollectiviteMenuStatistiquesStyle: string = "ev-menu";
  moduleCollectiviteMenuVisualisationAnnuaireStyle: string = "ev-menu";
  qualiteMenuControleStyle: string = "ev-menu";
  qualiteMenuNonConformiteStyle: string = "ev-menu";
  qualiteMenuRepartitionControleClientStyle: string = "ev-menu";
  qualiteMenuExtractionFicheControleStyle: string = "ev-menu";
  qualiteMenuExtractionControleStyle: string = "ev-menu";
  qualiteMenuExtractionNonConformiteStyle: string = "ev-menu";
  qualiteMenuExtractionCVQStyle: string = "ev-menu";
  qualiteMenuEtatIncitationQualiteStyle: string = "ev-menu";
  qualiteMenuEtatControleReceptionStyle: string = "ev-menu";
  ModuleCollectiviteMenuAccueil: string = "ev-menu";
  ModuleCollectiviteMenuDocuments: string = "ev-menu";
  ModuleCollectiviteMenuPrixReprise: string = "ev-menu";
  ModuleCollectiviteMenuStatistiques: string = "ev-menu";
  ModuleCollectiviteMenuVisualisationAnnuaire: string = "ev-menu";
  sideNavOpened: boolean = true;
  panelTransportExpanded: boolean = false;
  panelParametreGeneralExpanded: boolean = false;
  panelParametreAnnuaireExpanded: boolean = false;
  panelGestionEntiteExpanded: boolean = false;
  panelParametreLogistiqueExpanded: boolean = false;
  panelParametreQualiteExpanded: boolean = false;
  panelParametreMessagerieExpanded: boolean = false;
  panelParametreAdminAnnuaireExpanded: boolean = false;
  panelInformationsExpanded: boolean = false;
  panelLogistiqueExpanded: boolean = false;
  panelModulePrestataireExpanded: boolean = false;
  panelMessagerieExpanded: boolean = false;
  panelQualiteExpanded: boolean = false;
  panelStatistiqueExpanded: boolean = false;
  panelStatistiqueCITEOExpanded: boolean = false;
  panelModuleCollectiviteExpanded: boolean = true;
  //Misc
  refEntiteForDocument: number = 0;
  contactAdministratifNom: string;
  contactAdministratifTel: string;
  contactAdministratifEmail: string;
  responsableLogistiqueADVTel: string;
  responsableLogistiqueADVEmail: string;
  contactQualiteTel: string;
  contactQualiteEmail: string;
  //Locale
  private readonly _adapter = inject<DateAdapter<unknown, unknown>>(DateAdapter);
  private readonly _locale = signal(inject<unknown>(MAT_DATE_LOCALE));
  //Constructor
  constructor(public auth: AuthService,
    private activatedRoute: ActivatedRoute
    , private router: Router
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private eventEmitterService: EventEmitterService
    , private authService: AuthService
    , private downloadService: DownloadService
    //, private updateService: UpdateService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.activatedRoute.fragment.subscribe(params => {
    });
    router.events.subscribe((routerEvent: Event) => {
      this.checkRouterEvent(routerEvent);
    });
    //Init
    this.contactAdministratifNom = applicationUserContext.parametres.find(r => r.RefParametre == 3).ValeurTexte;
    this.contactAdministratifTel = applicationUserContext.parametres.find(r => r.RefParametre == 4).ValeurTexte;
    this.contactAdministratifEmail = applicationUserContext.parametres.find(r => r.RefParametre == 2).ValeurTexte;
    this.responsableLogistiqueADVTel = applicationUserContext.parametres.find(r => r.RefParametre == 9).ValeurTexte;
    this.responsableLogistiqueADVEmail = applicationUserContext.parametres.find(r => r.RefParametre == 11).ValeurTexte;
    this.contactQualiteTel = applicationUserContext.parametres.find(r => r.RefParametre == 12).ValeurTexte;
    this.contactQualiteEmail = applicationUserContext.parametres.find(r => r.RefParametre == 13).ValeurTexte;
  }
  ngOnInit() {
    //Register event from dashboard
    if (this.eventEmitterService.subsDashboardClick === undefined) {
      this.eventEmitterService.subsDashboardClick = this.eventEmitterService.
        invokeAppScreenRefresh.subscribe((name: string) => {
          //Manasge screen
          this.moduleStyleManagement();
          this.menuStyleManagement();
        });
    }
    //Password init if requested
    this.applicationUserContext.ckiePasswordLost = getCookie("passwordlost");
    if (this.applicationUserContext.ckiePasswordLost) {
      deleteCookie("passwordlost");
      this.router.navigate(["login"]);
      //Check if password reset is authorized
      this.authService.checkResetPassword(this.applicationUserContext.ckiePasswordLost).subscribe(result => {
        //Set culture
        if (result.cultureName.toString() === "en-GB") {
          changeCulture("en-GB", this.applicationUserContext);
          this._locale.set('en-gb');
        }
        else {
          changeCulture("fr-FR", this.applicationUserContext);
          this._locale.set('fr');
        }
        this._adapter.setLocale(this._locale());
        let data: any;
        //Init
        data = { title: this.applicationUserContext.getCulturedRessourceText(1089), type: "Utilisateur", ref: null, elementType: "ResetPwd" };
        //Open dialog
        const dialogRef = this.dialog.open(ComponentRelativeComponent, {
          width: "350px",
          data,
          autoFocus: false,
          restoreFocus: false
        });
      }, error => {
        //Change culture if applicable
        if (error.error.cultureName && error.error.cultureName == "en-GB") {
          changeCulture("en-GB", this.applicationUserContext);
          this._locale.set('en-gb');
          this._adapter.setLocale(this._locale());
        }
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
    }
    else {
      if (!this.auth.isLoggedIn()) {
        this.router.navigate(["login"]);
      }
    }
    //Set help visibility
  }
  onHome() {
    this.applicationUserContext.setDefaultModule();
    this.applicationUserContext.currentMenu = new appClasses.EnvMenu();
    this.moduleStyleManagement();
    this.menuStyleManagement();
    //Redirect to home
    if (!this.applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) {
      this.router.navigate(["home"]);
    }
    else {
      this.router.navigate(["accueil-collectivite"]);
    }
  }
  onLogout() {
    this.auth.logout();
    this.moduleStyleManagement();
    this.menuStyleManagement();
    this.refEntiteForDocument = 0;
    this.router.navigate(["login"]);
  }
  //------------------------------------------------------------------------------
  //Show user account
  onUserAccount() {
    let data: any;
    //Init
    data = { title: this.applicationUserContext.getCulturedRessourceText(1089), type: "Utilisateur", ref: this.applicationUserContext.connectedUtilisateur.RefUtilisateur };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "400px",
      data,
      autoFocus: false,
      restoreFocus: false
    });
  }
  //------------------------------------------------------------------------------
  //Return src of UtilisateurType icone
  getUtilisateurTypeImage():string {
    let s: string="";
    //Process
    switch (this.applicationUserContext.connectedUtilisateur.UtilisateurType) {
      case UtilisateurType.CentreDeTri:
        s = "assets/images/utilisateur-centre-de-tri.svg"
        break;
      case UtilisateurType.Collectivite:
        s = "assets/images/utilisateur-collectivite.svg"
        break;
      case UtilisateurType.Transporteur:
        s = "assets/images/utilisateur-transporteur.svg"
        break;
      case UtilisateurType.Client:
        s = "assets/images/utilisateur-client.svg"
        break;
      case UtilisateurType.Prestataire:
        s = "assets/images/utilisateur-prestataire.svg"
        break;
      case UtilisateurType.Valorplast:
        s = "assets/images/utilisateur-valorplast.svg"
        break;
    }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Dowmload Mentions Légales
  onMentionsLegales() {
    this.downloadService.download(DocumentType.Parametre, "20", "", null, "")
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        console.log(error);
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Dowmload help
  onHelp() {
    let refParametre: number = 0;
    if (this.applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) { refParametre = 14; }
    if (this.applicationUserContext.connectedUtilisateur.CentreDeTri?.RefEntite) { refParametre = 15; }
    if (this.applicationUserContext.connectedUtilisateur.Transporteur?.RefEntite) { refParametre = 16; }
    if (this.applicationUserContext.connectedUtilisateur.Client?.RefEntite) { refParametre = 17; }
    if (this.applicationUserContext.connectedUtilisateur.Prestataire?.RefEntite) { refParametre = 18; }
    if (refParametre > 0) {
      this.downloadService.download(DocumentType.Parametre, refParametre.toString(), "", null, "")
        .subscribe((data: HttpResponse<Blob>) => {
          let contentDispositionHeader = data.headers.get("Content-Disposition");
          let fileName = getAttachmentFilename(contentDispositionHeader);
          this.downloadFile(data.body, fileName);
        }, error => {
          console.log(error);
          showErrorToUser(this.dialog, error, this.applicationUserContext);
        });
    }
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
  //Show Entite public Document button
  showEntiteDocument(): number {
    if (this.refEntiteForDocument == 0) {
      let refEntite: number = 0;
      if (this.applicationUserContext.connectedUtilisateur.Transporteur?.RefEntite) {
        refEntite = this.applicationUserContext.connectedUtilisateur.Transporteur?.RefEntite;
      }
      else if (this.applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) {
        refEntite = this.applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite;
      }
      if (this.applicationUserContext.connectedUtilisateur.Client?.RefEntite) {
        refEntite = this.applicationUserContext.connectedUtilisateur.Client?.RefEntite;
      }
      if (this.applicationUserContext.connectedUtilisateur.Prestataire?.RefEntite) {
        refEntite = this.applicationUserContext.connectedUtilisateur.Prestataire?.RefEntite;
      }
      if (refEntite > 0) {
        this.refEntiteForDocument = refEntite
      }
    }
    return this.refEntiteForDocument;
  }
  //------------------------------------------------------------------------------
  //Show Entite public Document
  onDocuments() {
    let data: any;
    //Set RefEntite
    let refEntite: number = 0;
    refEntite = this.showEntiteDocument();
    //Init
    data = { title: this.applicationUserContext.getCulturedRessourceText(1201), type: "DocumentEntite", refEntite: this.refEntiteForDocument };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "350px",
      data,
      autoFocus: false,
      restoreFocus: false
    });
  }
  //------------------------------------------------------------------------------
  //Prepare router
  prepareRoute(outlet: RouterOutlet) {
    return outlet && outlet.activatedRouteData && outlet.activatedRouteData["animation"];
  }
  //------------------------------------------------------------------------------
  //Manage router
  checkRouterEvent(routerEvent: Event): void {
    if (routerEvent instanceof NavigationEnd) {
      if (routerEvent.id === 403) {
        this.router.navigate(["login"]);
      }
    }
  }
  onFilterDRChange(ev: MatCheckboxChange) {
    this.dataModelService.setFilterDR(ev.checked).subscribe(result => {
      this.applicationUserContext.filterDR = result;
      //Reset filters
      this.applicationUserContext.razFilters();
      //Redirect to home
      this.router.navigate(["home"]);
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  onFilterGlobalActifChange(ev: MatCheckboxChange) {
    this.dataModelService.setFilterGlobalActif(ev.checked).subscribe(result => {
      //Reset filters
      this.applicationUserContext.razFilters();
      //Set filterGlobalActif
      this.applicationUserContext.filterGlobalActif = result;
      //Redirect to home
      this.router.navigate(["home"]);
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //------------------------------------------------------------------------------
  //Modules style management
  moduleStyleManagement(): void {
    this.menuModules = false;
    this.logistiqueModuleStyle = "ev-module";
    this.annuaireModuleStyle = "ev-module";
    this.messagerieModuleStyle = "ev-module";
    this.qualiteModuleStyle = "ev-module";
    this.modulePrestataireModuleStyle = "ev-module";
    this.moduleCollectiviteModuleStyle = "ev-module";
    this.administrationModuleStyle = "ev-module";
    switch (this.applicationUserContext.currentModule.name) {
      case "Logistique":
        this.logistiqueModuleStyle = "ev-module-selected";
        this.setExpanded("panelLogistiqueExpanded");
        break;
      case "Annuaire":
        this.annuaireModuleStyle = "ev-module-selected";
        break;
      case "Messagerie":
        this.messagerieModuleStyle = "ev-module-selected";
        this.setExpanded("panelMessagerieExpanded");
        break;
      case "Qualite":
        this.qualiteModuleStyle = "ev-module-selected";
        this.setExpanded("panelQualiteExpanded");
        break;
      case "Administration":
        this.administrationModuleStyle = "ev-module-selected";
        break;
      case "ModulePrestataire":
        this.modulePrestataireModuleStyle = "ev-module-selected";
        break;
      case "ModuleCollectivite":
        this.moduleCollectiviteModuleStyle = "ev-module-selected";
        break;
      default:
        this.menuModules = true;
        break;
    }
  }
  //------------------------------------------------------------------------------
  //Menus style management
  menuStyleManagement(): void {
    //RAZ
    this.administrationMenuActionTypeStyle = "ev-menu";
    this.administrationMenuAdresseTypeStyle = "ev-menu";
    this.administrationMenuAideStyle = "ev-menu";
    this.administrationMenuApplicationStyle = "ev-menu";
    this.administrationMenuApplicationProduitOrigineStyle = "ev-menu";
    this.administrationMenuCamionTypeStyle = "ev-menu";
    this.administrationMenuCiviliteStyle = "ev-menu";
    this.administrationMenuClientApplicationStyle = "ev-menu";
    this.administrationMenuDescriptionControleStyle = "ev-menu";
    this.administrationMenuDescriptionCVQStyle = "ev-menu";
    this.administrationMenuDescriptionReceptionStyle = "ev-menu";
    this.administrationMenuDocumentStyle = "ev-menu";
    this.administrationMenuEcoOrganismeStyle = "ev-menu";
    this.administrationMenuExtractionLoginStyle = "ev-menu";
    this.administrationMenuEntiteTypeStyle = "ev-menu";
    this.administrationMenuEquipementierStyle = "ev-menu";
    this.administrationMenuEquivalentCO2Style = "ev-menu";
    this.administrationMenuFonctionStyle = "ev-menu";
    this.administrationMenuFormeContactStyle = "ev-menu";
    this.administrationMenuFournisseurTOStyle = "ev-menu";
    this.administrationMenuJourFerieStyle = "ev-menu";
    this.administrationMenuMessageTypeStyle = "ev-menu";
    this.administrationMenuModeTransportEEStyle = "ev-menu";
    this.administrationMenuMontantIncitationQualiteStyle = "ev-menu";
    this.administrationMenuMotifAnomalieChargementStyle = "ev-menu";
    this.administrationMenuMotifAnomalieClientStyle = "ev-menu";
    this.administrationMenuMotifAnomalieTransporteurStyle = "ev-menu";
    this.administrationMenuMotifCamionIncompletStyle = "ev-menu";
    this.administrationMenuNonConformiteDemandeClientTypeStyle = "ev-menu";
    this.administrationMenuNonConformiteFamilleStyle = "ev-menu";
    this.administrationMenuNonConformiteNatureStyle = "ev-menu";
    this.administrationMenuNonConformiteReponseClientTypeStyle = "ev-menu";
    this.administrationMenuNonConformiteReponseFournisseurTypeStyle = "ev-menu";
    this.administrationMenuParamEmailStyle = "ev-menu";
    this.administrationMenuParametreStyle = "ev-menu";
    this.administrationMenuPaysStyle = "ev-menu";
    this.administrationMenuProcessStyle = "ev-menu";
    this.administrationMenuProduitStyle = "ev-menu";
    this.administrationMenuProduitGroupeReportingTypeStyle = "ev-menu";
    this.administrationMenuProduitGroupeReportingStyle = "ev-menu";
    this.administrationMenuRegionEEStyle = "ev-menu";
    this.administrationMenuRegionReportingStyle = "ev-menu";
    this.administrationMenuRepreneurStyle = "ev-menu";
    this.administrationMenuRepriseTypeStyle = "ev-menu";
    this.administrationMenuRessourceStyle = "ev-menu";
    this.administrationMenuSAGECodeTransportStyle = "ev-menu";
    this.administrationMenuSecuriteStyle = "ev-menu";
    this.administrationMenuServiceStyle = "ev-menu";
    this.administrationMenuStandardStyle = "ev-menu";
    this.administrationMenuSuiviLoginStyle = "ev-menu";
    this.administrationMenuTicketStyle = "ev-menu",
    this.administrationMenuTitreStyle = "ev-menu";
    this.administrationMenuUtilisateurStyle = "ev-menu";
    this.administrationMenuUtilisateurInactifStyle = "ev-menu";
    this.annuaireMenuEmailNoteCreditCollectiviteStyle = "ev-menu";
    this.annuaireMenuSuiviEnvoisStyle = "ev-menu";
    this.annuaireMenuEntiteStyle = "ev-menu";
    this.annuaireMenuExtractionActionStyle = "ev-menu";
    this.annuaireMenuExtractionAnnuaireStyle = "ev-menu";
    this.annuaireMenuNbAffretementTransporteurStyle = "ev-menu";
    this.logistiqueMenuChargementAnnuleStyle = "ev-menu";
    this.logistiqueMenuCommandeClientStyle = "ev-menu";
    this.logistiqueMenuCommandeFournisseurStyle = "ev-menu";
    this.logistiqueMenuRepartitionStyle = "ev-menu";
    this.logistiqueMenuImpressionStyle = "ev-menu";
    this.logistiqueMenuExportSAGEStyle = "ev-menu";
    this.logistiqueMenuPrixRepriseStyle = "ev-menu";
    this.logistiqueMenuTransportNonValideStyle = "ev-menu";
    this.logistiqueMenuModifierTransportStyle = "ev-menu";
    this.logistiqueMenuImporterTransportStyle = "ev-menu";
    this.logistiqueMenuSupprimerTransportEnMasseStyle = "ev-menu";
    this.logistiqueMenuValiderNouveauPrixTransportStyle = "ev-menu";
    this.logistiqueMenuModifierTousPrixTransportStyle = "ev-menu";
    this.logistiqueMenuSurcoutCarburantStyle = "ev-menu";
    this.logistiqueMenuTransportDemandeEnlevementStyle = "ev-menu";
    this.logistiqueMenuTransportCommandeEnCoursStyle = "ev-menu";
    this.logistiqueMenuTransportCommandeModifieesStyle = "ev-menu";
    this.logistiqueMenuTransportCommandeStyle = "ev-menu";
    this.logistiqueMenuEtatCoutTransportStyle = "ev-menu";
    this.logistiqueMenuLotDisponibleStyle = "ev-menu";
    this.logistiqueMenuEtatReceptionFournisseurChargementStyle = "ev-menu";
    this.logistiqueMenuEtatReceptionStyle = "ev-menu";
    this.logistiqueMenuSuiviCommandeClientStyle = "ev-menu";
    this.logistiqueMenuDelaiCommandeEnlevementStyle = "ev-menu";
    this.logistiqueMenuPoidsMoyenChargementProduitStyle = "ev-menu";
    this.logistiqueMenuEtatDesEnlevementsStyle = "ev-menu";
    this.logistiqueMenuExtractionLekoStyle = "ev-menu";
    this.logistiqueMenuSuiviCommandeClientProduitStyle = "ev-menu";
    this.logistiqueMenuEtatReceptionProduitStyle = "ev-menu";
    this.logistiqueMenuEtatReceptionProduitDRStyle = "ev-menu";
    this.logistiqueMenuExtractionReceptionStyle = "ev-menu";
    this.logistiqueMenuExtractionCommandeStyle = "ev-menu";
    this.logistiqueMenuEtatDesPoidsStyle = "ev-menu";
    this.logistiqueMenuEtatTonnageParProcessStyle = "ev-menu";
    this.logistiqueMenuEtatVenteAnnuelleProduitClientStyle = "ev-menu";
    this.logistiqueMenuEtatDestinationAnnuelleProduitClientStyle = "ev-menu";
    this.logistiqueMenuSuiviFacturationHCStyle = "ev-menu";
    this.logistiqueMenuTonnageCollectiviteProduitStyle = "ev-menu";
    this.logistiqueMenuTonnageCollectiviteCDTProduitStyle = "ev-menu";
    this.logistiqueMenuTonnageCDTProduitComposantStyle = "ev-menu";
    this.logistiqueMenuTonnageCLSousContratStyle = "ev-menu";
    this.logistiqueMenuListeProduitStyle = "ev-menu";
    this.logistiqueMenuEtatTarifTransporteurClientStyle = "ev-menu";
    this.logistiqueMenuPartMarcheTransporteurStyle = "ev-menu";
    this.logistiqueMenuEtatKmMoyenStyle = "ev-menu";
    this.logistiqueMenuEtatReceptionEmballagePlastiqueStyle = "ev-menu";
    this.logistiqueMenuEtatSuiviTonnageRecycleurStyle = "ev-menu";
    this.logistiqueMenuEtatDesFluxDevStyle = "ev-menu";
    this.logistiqueMenuExtractionOscarStyle = "ev-menu";
    this.logistiqueMenuEtatDesFluxDevLekoStyle = "ev-menu";
    this.menuVisualisationAnnuaireStyle = "ev-menu";
    this.menuDocumentsStyle = "ev-menu";
    this.messagerieMenuMessageStyle = "ev-menu";
    this.messagerieMenuVisualisationStyle = "ev-menu";
    this.modulePrestataireMenuCommandeFournisseurStyle = "ev-menu";
    this.annuaireMenuEvolutionTonnageStyle = "ev-menu";
    this.qualiteMenuControleStyle = "ev-menu";
    this.qualiteMenuNonConformiteStyle = "ev-menu";
    this.qualiteMenuRepartitionControleClientStyle = "ev-menu";
    this.qualiteMenuExtractionFicheControleStyle = "ev-menu";
    this.qualiteMenuExtractionControleStyle = "ev-menu";
    this.qualiteMenuExtractionNonConformiteStyle = "ev-menu";
    this.qualiteMenuExtractionCVQStyle = "ev-menu";
    this.qualiteMenuEtatIncitationQualiteStyle = "ev-menu";
    this.qualiteMenuEtatControleReceptionStyle = "ev-menu";
    this.moduleCollectiviteMenuAccueilStyle = "ev-menu";
    this.moduleCollectiviteMenuDocumentsStyle = "ev-menu";
    this.moduleCollectiviteMenuPrixRepriseStyle = "ev-menu";
    this.moduleCollectiviteMenuStatistiquesStyle = "ev-menu";
    this.moduleCollectiviteMenuVisualisationAnnuaireStyle = "ev-menu";
    this.panelTransportExpanded = false;
    this.panelParametreAnnuaireExpanded = false;
    this.panelGestionEntiteExpanded = false;
    this.panelParametreGeneralExpanded = false;
    this.panelParametreLogistiqueExpanded = false;
    this.panelParametreQualiteExpanded = false;
    this.panelParametreMessagerieExpanded = false;
    this.panelParametreAdminAnnuaireExpanded = false;
    this.panelInformationsExpanded = false;
    this.panelLogistiqueExpanded = false;
    this.panelModulePrestataireExpanded = false;
    this.panelMessagerieExpanded = false;
    this.panelQualiteExpanded = false;
    this.panelStatistiqueExpanded = false;
    this.panelStatistiqueCITEOExpanded = false;
    //Process
    switch (this.applicationUserContext.currentMenu.name) {
      case "AdministrationMenuActionType":
        this.administrationMenuActionTypeStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuAdresseType":
        this.administrationMenuAdresseTypeStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuAide":
        this.administrationMenuAideStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuApplication":
        this.administrationMenuApplicationStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuApplicationProduitOrigine":
        this.administrationMenuApplicationProduitOrigineStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuExtractionLogin":
        this.administrationMenuExtractionLoginStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "AdministrationMenuSuiviLogin":
        this.administrationMenuSuiviLoginStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "AdministrationMenuEcoOrganisme":
        this.administrationMenuEcoOrganismeStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuEntiteType":
        this.administrationMenuEntiteTypeStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuMessageType":
        this.administrationMenuMessageTypeStyle = "ev-menu-selected";
        this.panelParametreMessagerieExpanded = true;
        break;
      case "AdministrationMenuFormeContact":
        this.administrationMenuFormeContactStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuParamEmail":
        this.administrationMenuParamEmailStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuPays":
        this.administrationMenuPaysStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuRegionEE":
        this.administrationMenuRegionEEStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuRegionReporting":
        this.administrationMenuRegionReportingStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuRepreneur":
        this.administrationMenuRepreneurStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuRepriseType":
        this.administrationMenuRepriseTypeStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuParametre":
        this.administrationMenuParametreStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuRessource":
        this.administrationMenuRessourceStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuProduitGroupeReportingType":
        this.administrationMenuProduitGroupeReportingTypeStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuProduitGroupeReporting":
        this.administrationMenuProduitGroupeReportingStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuSAGECodeTransport":
        this.administrationMenuSAGECodeTransportStyle = "ev-menu-selected";
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "AdministrationMenuTicket":
        this.administrationMenuTicketStyle = "ev-menu-selected";
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "AdministrationMenuJourFerie":
        this.administrationMenuJourFerieStyle = "ev-menu-selected";
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "AnnuaireMenuEnvoiDocumentType":
        this.annuaireMenuEmailNoteCreditCollectiviteStyle = "ev-menu-selected";
        this.panelParametreAnnuaireExpanded = true;
        break;
      case "AnnuaireMenuSuiviEnvois":
        this.annuaireMenuSuiviEnvoisStyle = "ev-menu-selected";
        this.panelParametreAnnuaireExpanded = true;
        break;
      case "AnnuaireMenuEntite":
        this.annuaireMenuEntiteStyle = "ev-menu-selected";
        this.panelGestionEntiteExpanded = true;
        break;
      case "AnnuaireMenuExtractionAction":
        this.annuaireMenuExtractionActionStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "AnnuaireMenuExtractionAnnuaire":
        this.annuaireMenuExtractionAnnuaireStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "AnnuaireMenuNbAffretementTransporteur":
        this.annuaireMenuNbAffretementTransporteurStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "AdministrationMenuUtilisateur":
        this.administrationMenuUtilisateurStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuSecurite":
        this.administrationMenuSecuriteStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuUtilisateurInactif":
        this.administrationMenuUtilisateurInactifStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuCamionType":
        this.administrationMenuCamionTypeStyle = "ev-menu-selected";
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "AdministrationMenuCivilite":
        this.administrationMenuCiviliteStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuClientApplication":
        this.administrationMenuClientApplicationStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuDescriptionControle":
        this.administrationMenuDescriptionControleStyle = "ev-menu-selected";
        this.panelParametreQualiteExpanded = true;
        break;
      case "AdministrationMenuDescriptionCVQ":
        this.administrationMenuDescriptionCVQStyle = "ev-menu-selected";
        this.panelParametreQualiteExpanded = true;
        break;
      case "AdministrationMenuDescriptionReception":
        this.administrationMenuDescriptionReceptionStyle = "ev-menu-selected";
        this.panelParametreQualiteExpanded = true;
        break;
      case "AdministrationMenuDocument":
        this.administrationMenuDocumentStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuEquipementier":
        this.administrationMenuEquipementierStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuEquivalentCO2":
        this.administrationMenuEquivalentCO2Style = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuFonction":
        this.administrationMenuFonctionStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuFournisseurTO":
        this.administrationMenuFournisseurTOStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuModeTransportEE":
        this.administrationMenuModeTransportEEStyle = "ev-menu-selected";
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "AdministrationMenuMontantIncitationQualite":
        this.administrationMenuMontantIncitationQualiteStyle = "ev-menu-selected";
        this.panelParametreQualiteExpanded = true;
        break;
      case "AdministrationMenuMotifAnomalieChargement":
        this.administrationMenuMotifAnomalieChargementStyle = "ev-menu-selected";
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "AdministrationMenuMotifAnomalieClient":
        this.administrationMenuMotifAnomalieClientStyle = "ev-menu-selected";
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "AdministrationMenuMotifAnomalieTransporteur":
        this.administrationMenuMotifAnomalieTransporteurStyle = "ev-menu-selected";
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "AdministrationMenuMotifCamionIncomplet":
        this.administrationMenuMotifCamionIncompletStyle = "ev-menu-selected";
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "AdministrationMenuNonConformiteDemandeClientType":
        this.administrationMenuNonConformiteDemandeClientTypeStyle = "ev-menu-selected";
        this.panelParametreQualiteExpanded = true;
        break;
      case "AdministrationMenuNonConformiteFamille":
        this.administrationMenuNonConformiteFamilleStyle = "ev-menu-selected";
        this.panelParametreQualiteExpanded = true;
        break;
      case "AdministrationMenuNonConformiteNature":
        this.administrationMenuNonConformiteNatureStyle = "ev-menu-selected";
        this.panelParametreQualiteExpanded = true;
        break;
      case "AdministrationMenuNonConformiteReponseClientType":
        this.administrationMenuNonConformiteReponseClientTypeStyle = "ev-menu-selected";
        this.panelParametreQualiteExpanded = true;
        break;
      case "AdministrationMenuNonConformiteReponseFournisseurType":
        this.administrationMenuNonConformiteReponseFournisseurTypeStyle = "ev-menu-selected";
        this.panelParametreQualiteExpanded = true;
        break;
      case "AdministrationMenuProcess":
        this.administrationMenuProcessStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuProduit":
        this.administrationMenuProduitStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuStandard":
        this.administrationMenuStandardStyle = "ev-menu-selected";
        this.panelParametreGeneralExpanded = true;
        break;
      case "AdministrationMenuTitre":
        this.administrationMenuTitreStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "AdministrationMenuService":
        this.administrationMenuServiceStyle = "ev-menu-selected";
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "LogistiqueMenuCommandeClient":
        this.logistiqueMenuCommandeClientStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuCommandeFournisseur":
        this.logistiqueMenuCommandeFournisseurStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuRepartition":
        this.logistiqueMenuRepartitionStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuImpression":
        this.logistiqueMenuImpressionStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuExportSAGE":
        this.logistiqueMenuExportSAGEStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuPrixReprise":
        this.logistiqueMenuPrixRepriseStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuSurcoutCarburant":
        this.logistiqueMenuSurcoutCarburantStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "LogistiqueMenuTransportNonValide":
        this.logistiqueMenuTransportNonValideStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "LogistiqueMenuModifierTransport":
        this.logistiqueMenuModifierTransportStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "LogistiqueMenuImporterTransport":
        this.logistiqueMenuImporterTransportStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "LogistiqueMenuSupprimerTransportEnMasse":
        this.logistiqueMenuSupprimerTransportEnMasseStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "LogistiqueMenuValiderNouveauPrixTransport":
        this.logistiqueMenuValiderNouveauPrixTransportStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "LogistiqueMenuModifierTousPrixTransport":
        this.logistiqueMenuModifierTousPrixTransportStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "MenuVisualisationAnnuaire":
        this.menuVisualisationAnnuaireStyle = "ev-menu-selected";
        this.panelInformationsExpanded = true;
        break;
      case "MenuDocuments":
        this.menuDocumentsStyle = "ev-menu-selected";
        this.panelInformationsExpanded = true;
        break;
      case "LogistiqueMenuTransportDemandeEnlevement":
        this.logistiqueMenuTransportDemandeEnlevementStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuTransportCommandeEnCours":
        this.logistiqueMenuTransportCommandeEnCoursStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuTransportCommandeModifiee":
        this.logistiqueMenuTransportCommandeModifieesStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuTransportCommande":
        this.logistiqueMenuTransportCommandeStyle = "ev-menu-selected";
        this.panelLogistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatCoutTransport":
        this.logistiqueMenuEtatCoutTransportStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "LogistiqueMenuEtatTarifTransporteurClient":
        this.logistiqueMenuEtatTarifTransporteurClientStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "LogistiqueMenuPartMarcheTransporteur":
        this.logistiqueMenuPartMarcheTransporteurStyle = "ev-menu-selected";
        this.panelTransportExpanded = true;
        break;
      case "LogistiqueMenuChargementAnnule":
        this.logistiqueMenuChargementAnnuleStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuLotDisponible":
        this.logistiqueMenuLotDisponibleStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatReceptionFournisseurChargement":
        this.logistiqueMenuEtatReceptionFournisseurChargementStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatReception":
        this.logistiqueMenuEtatReceptionStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuSuiviCommandeClient":
        this.logistiqueMenuSuiviCommandeClientStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuDelaiCommandeEnlevement":
        this.logistiqueMenuDelaiCommandeEnlevementStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuPoidsMoyenChargementProduit":
        this.logistiqueMenuPoidsMoyenChargementProduitStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatDesEnlevements":
        this.logistiqueMenuEtatDesEnlevementsStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuSuiviCommandeClientProduit":
        this.logistiqueMenuSuiviCommandeClientProduitStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatReceptionProduit":
        this.logistiqueMenuEtatReceptionProduitStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatReceptionProduitDR":
        this.logistiqueMenuEtatReceptionProduitDRStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuExtractionReception":
        this.logistiqueMenuExtractionReceptionStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuExtractionCommande":
        this.logistiqueMenuExtractionCommandeStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatDesPoids":
        this.logistiqueMenuEtatDesPoidsStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatKmMoyen":
        this.logistiqueMenuEtatKmMoyenStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatTonnageParProcess":
        this.logistiqueMenuEtatTonnageParProcessStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatVenteAnnuelleProduitClient":
        this.logistiqueMenuEtatVenteAnnuelleProduitClientStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatDestinationAnnuelleProduitClient":
        this.logistiqueMenuEtatDestinationAnnuelleProduitClientStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuSuiviFacturationHCS":
        this.logistiqueMenuSuiviFacturationHCStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuTonnageCollectiviteProduit":
        this.logistiqueMenuTonnageCollectiviteProduitStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuTonnageCollectiviteCDTProduit":
        this.logistiqueMenuTonnageCollectiviteCDTProduitStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuTonnageCDTProduitComposant":
        this.logistiqueMenuTonnageCDTProduitComposantStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuTonnageCLSousContrat":
        this.logistiqueMenuTonnageCLSousContratStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuListeProduit":
        this.logistiqueMenuListeProduitStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "LogistiqueMenuEtatReceptionEmballagePlastique":
        this.logistiqueMenuEtatReceptionEmballagePlastiqueStyle = "ev-menu-selected";
        this.panelStatistiqueCITEOExpanded = true;
        break;
      case "LogistiqueMenuEtatSuiviTonnageRecycleur":
        this.logistiqueMenuEtatSuiviTonnageRecycleurStyle = "ev-menu-selected";
        this.panelStatistiqueCITEOExpanded = true;
        break;
      case "LogistiqueMenuEtatDesFluxDev":
        this.logistiqueMenuEtatDesFluxDevStyle = "ev-menu-selected";
        this.panelStatistiqueCITEOExpanded = true;
        break;
      case "LogistiqueMenuExtractionOscar":
        this.logistiqueMenuExtractionOscarStyle = "ev-menu-selected";
        this.panelStatistiqueCITEOExpanded = true;
        break;
      case "LogistiqueMenuEtatDesFluxDevLeko":
        this.logistiqueMenuEtatDesFluxDevLekoStyle = "ev-menu-selected";
        this.panelStatistiqueCITEOExpanded = true;
        break;
      case "LogistiqueMenuExtractionLeko":
        this.logistiqueMenuExtractionLekoStyle = "ev-menu-selected";
        this.panelStatistiqueCITEOExpanded = true;
        break;
      case "AnnuaireMenuEvolutionTonnage":
        this.annuaireMenuEvolutionTonnageStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "MessagerieMenuMessage":
        this.messagerieMenuMessageStyle = "ev-menu-selected";
        this.panelMessagerieExpanded = true;
        break;
      case "MessagerieMenuVisualisation":
        this.messagerieMenuVisualisationStyle = "ev-menu-selected";
        this.panelMessagerieExpanded = true;
        break;
      case "ModulePrestataireMenuCommandeFournisseur":
        this.modulePrestataireMenuCommandeFournisseurStyle = "ev-menu-selected";
        this.panelModulePrestataireExpanded = true;
        break;
      case "QualiteMenuControle":
        this.qualiteMenuControleStyle = "ev-menu-selected";
        this.panelQualiteExpanded = true;
        break;
      case "QualiteMenuNonConformite":
        this.qualiteMenuNonConformiteStyle = "ev-menu-selected";
        this.panelQualiteExpanded = true;
        break;
      case "QualiteMenuRepartitionControleClient":
        this.qualiteMenuRepartitionControleClientStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "QualiteMenuExtractionFicheControle":
        this.qualiteMenuExtractionFicheControleStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "QualiteMenuExtractionControle":
        this.qualiteMenuExtractionControleStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "QualiteMenuExtractionNonConformite":
        this.qualiteMenuExtractionNonConformiteStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "QualiteMenuExtractionCVQ":
        this.qualiteMenuExtractionCVQStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "QualiteMenuEtatIncitationQualite":
        this.qualiteMenuEtatIncitationQualiteStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "QualiteMenuEtatControleReception":
        this.qualiteMenuEtatControleReceptionStyle = "ev-menu-selected";
        this.panelStatistiqueExpanded = true;
        break;
      case "ModuleCollectiviteMenuAccueil":
        this.moduleCollectiviteMenuAccueilStyle = "ev-menu-selected";
        this.panelModuleCollectiviteExpanded = true;
        break;
      case "ModuleCollectiviteMenuDocuments":
        this.moduleCollectiviteMenuDocumentsStyle = "ev-menu-selected";
        this.panelModuleCollectiviteExpanded = true;
        break;
      case "ModuleCollectiviteMenuPrixReprise":
        this.moduleCollectiviteMenuPrixRepriseStyle = "ev-menu-selected";
        this.panelModuleCollectiviteExpanded = true;
        break;
      case "ModuleCollectiviteMenuStatistiques":
        this.moduleCollectiviteMenuStatistiquesStyle = "ev-menu-selected";
        this.panelModuleCollectiviteExpanded = true;
        break;
    }
  }
  //------------------------------------------------------------------------------
  //Menus groups style management
  setExpanded(menuGroupName: string): void {
    //RAZ
    this.panelTransportExpanded = false;
    this.panelParametreAnnuaireExpanded = false;
    this.panelParametreGeneralExpanded = false;
    this.panelParametreLogistiqueExpanded = false;
    this.panelParametreMessagerieExpanded = false;
    this.panelParametreAdminAnnuaireExpanded = false;
    this.panelInformationsExpanded = false;
    this.panelLogistiqueExpanded = false;
    this.panelMessagerieExpanded = false;
    this.panelQualiteExpanded = false;
    this.panelStatistiqueExpanded = false;
    this.panelStatistiqueCITEOExpanded = false;
    //Process
    switch (menuGroupName) {
      case "panelParametreGeneralExpanded":
        this.panelParametreGeneralExpanded = true;
        break;
      case "panelParametreAnnuaireExpanded":
        this.panelParametreAnnuaireExpanded = true;
        break;
      case "panelParametreAdminAnnuaireExpanded":
        this.panelParametreAdminAnnuaireExpanded = true;
        break;
      case "panelGestionEntiteExpanded":
        this.panelGestionEntiteExpanded = true;
        break;
      case "panelParametreLogistiqueExpanded":
        this.panelParametreLogistiqueExpanded = true;
        break;
      case "panelParametreMessagerieExpanded":
        this.panelParametreMessagerieExpanded = true;
        break;
      case "panelTransportExpanded":
        this.panelTransportExpanded = true;
        break;
      case "panelInformationsExpanded":
        this.panelInformationsExpanded = true;
        break;
      case "panelLogistiqueExpanded":
        this.panelLogistiqueExpanded = true;
        break;
      case "panelMessagerieExpanded":
        this.panelMessagerieExpanded = true;
        break;
      case "panelQualiteExpanded":
        this.panelQualiteExpanded = true;
        break;
      case "panelStatistiqueExpanded":
        this.panelStatistiqueExpanded = true;
        break;
      case "panelStatistiqueCITEOExpanded":
        this.panelStatistiqueCITEOExpanded = true;
        break;
    }
  }
  //------------------------------------------------------------------------------
  //Navigation to menu
  navigateToMenu(addNew: boolean): void {
    //RAZ menus/dashboard specific filters
    this.applicationUserContext.razMenuSpecificFilters();
    //this.applicationUserContext.razFilters();
    this.applicationUserContext.setDefaultFilters();
    //Navigate
    if (this.applicationUserContext.currentMenu.gridGoto) {
      if (addNew) { this.router.navigate([this.applicationUserContext.currentMenu.gridGoto, "0"]); }
      else { this.router.navigate(["grid"]); }
    }
    else {
      switch (this.applicationUserContext.currentMenu.name) {
        case MenuName.AdministrationMenuUtilisateur:
          if (addNew) { this.router.navigate(["utilisateur", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuSecurite:
          this.router.navigate(["securite"]);
          break;
        case MenuName.AnnuaireMenuEnvoiDocumentType:
          this.router.navigate(["envoi-email", "EmailNoteCreditCollectivite"]);
          break;
        case MenuName.MenuVisualisationAnnuaire:
        case MenuName.ModuleCollectiviteMenuVisualisationAnnuaire:
          this.router.navigate(["my-contacts"]);
          break;
        case MenuName.AnnuaireMenuEntite:
          if (addNew) { this.router.navigate(["entite", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.MenuDocuments:
          this.router.navigate(["documents", DocumentType.Documents]);
          break;
        case MenuName.LogistiqueMenuCommandeClient:
          if (addNew) { this.router.navigate(["commande-client", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.LogistiqueMenuCommandeFournisseur:
          if (addNew) { this.router.navigate(["commande-fournisseur", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuDocument:
          if (addNew) { this.router.navigate(["document", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuEquipementier:
          if (addNew) { this.router.navigate(["equipementier", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuEquivalentCO2:
          if (addNew) { this.router.navigate(["equivalentco2", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuFonction:
          if (addNew) { this.router.navigate(["fonction", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuFournisseurTO:
          if (addNew) { this.router.navigate(["fournisseurto", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuService:
          if (addNew) { this.router.navigate(["service", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuTitre:
          if (addNew) { this.router.navigate(["titre", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuParametre:
        case MenuName.AdministrationMenuDescriptionReception:
        case MenuName.AdministrationMenuUtilisateurInactif:
        case MenuName.LogistiqueMenuTransportNonValide:
        case MenuName.LogistiqueMenuModifierTransport:
        case MenuName.LogistiqueMenuSupprimerTransportEnMasse:
        case MenuName.LogistiqueMenuModifierTousPrixTransport:
        case MenuName.LogistiqueMenuTransportDemandeEnlevement:
        case MenuName.LogistiqueMenuTransportCommandeEnCours:
        case MenuName.LogistiqueMenuTransportCommandeModifiee:
        case MenuName.LogistiqueMenuTransportCommande:
        case MenuName.LogistiqueMenuRepartition:
        case MenuName.MessagerieMenuVisualisation:
        case MenuName.ModulePrestataireMenuCommandeFournisseur:
          this.router.navigate(["grid"]);
          break;
        case MenuName.MessagerieMenuMessage:
          if (addNew) { this.router.navigate(["message", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.QualiteMenuControle:
          if (addNew) { this.router.navigate(["controle", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.QualiteMenuNonConformite:
          this.router.navigate(["grid"]);
          break;
        case MenuName.AdministrationMenuSuiviLogin:
        case MenuName.AdministrationMenuExtractionLogin:
        case MenuName.QualiteMenuRepartitionControleClient:
        case MenuName.QualiteMenuExtractionFicheControle:
        case MenuName.QualiteMenuExtractionControle:
        case MenuName.QualiteMenuExtractionNonConformite:
        case MenuName.QualiteMenuExtractionCVQ:
        case MenuName.QualiteMenuEtatIncitationQualite:
        case MenuName.QualiteMenuEtatControleReception:
        case MenuName.ModuleCollectiviteMenuStatistiques:
          this.router.navigate(["statistique-grid"]);
          break;
        case MenuName.AdministrationMenuAide:
          if (addNew) { this.router.navigate(["aide", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuMontantIncitationQualite:
          if (addNew) { this.router.navigate(["montant-incitation-qualite", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuNonConformiteDemandeClientType:
          if (addNew) { this.router.navigate(["non-conformite-demande-client-type", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuNonConformiteFamille:
          if (addNew) { this.router.navigate(["non-conformite-famille", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuNonConformiteNature:
          if (addNew) { this.router.navigate(["non-conformite-nature", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuNonConformiteReponseClientType:
          if (addNew) { this.router.navigate(["non-conformite-reponse-client-type", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuNonConformiteReponseFournisseurType:
          if (addNew) { this.router.navigate(["non-conformite-reponse-fournisseur-type", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuClientApplication:
          if (addNew) { this.router.navigate(["client-application", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuDescriptionControle:
          if (addNew) { this.router.navigate(["description-controle-container", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.AdministrationMenuDescriptionCVQ:
          if (addNew) { this.router.navigate(["description-cvq-container", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.LogistiqueMenuPrixReprise:
          if (addNew) { this.router.navigate(["prix-reprise", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.LogistiqueMenuExportSAGE:
          this.router.navigate(["export-sage"]);
          break;
        case MenuName.LogistiqueMenuImpression:
          this.router.navigate(["document-edit"]);
          break;
        case MenuName.LogistiqueMenuImporterTransport:
          this.router.navigate(["upload-transport"]);
          break;
        case MenuName.LogistiqueMenuSurcoutCarburant:
          if (addNew) { this.router.navigate(["surcout-carburant", "0"]); }
          else { this.router.navigate(["grid"]); }
          break;
        case MenuName.ModuleCollectiviteMenuAccueil:
          this.router.navigate(["accueil-collectivite"]);
          break;
        case MenuName.ModuleCollectiviteMenuDocuments:
          this.router.navigate(["documents", DocumentType.Documents]);
          break;
        case MenuName.ModuleCollectiviteMenuPrixReprise:
          this.router.navigate(["prix-reprises"]);
          break;
        case MenuName.AnnuaireMenuExtractionAnnuaire:
        case MenuName.AnnuaireMenuNbAffretementTransporteur:
        case MenuName.AnnuaireMenuExtractionAction:
        case MenuName.LogistiqueMenuEtatCoutTransport:
        case MenuName.LogistiqueMenuChargementAnnule:
        case MenuName.LogistiqueMenuEtatTarifTransporteurClient:
        case MenuName.LogistiqueMenuPartMarcheTransporteur:
        case MenuName.LogistiqueMenuEtatKmMoyen:
        case MenuName.LogistiqueMenuLotDisponible:
        case MenuName.LogistiqueMenuEtatReceptionFournisseurChargement:
        case MenuName.LogistiqueMenuEtatReception:
        case MenuName.LogistiqueMenuSuiviCommandeClient:
        case MenuName.LogistiqueMenuDelaiCommandeEnlevement:
        case MenuName.LogistiqueMenuEtatDesEnlevements:
        case MenuName.LogistiqueMenuExtractionLeko:
        case MenuName.LogistiqueMenuPoidsMoyenChargementProduit:
        case MenuName.LogistiqueMenuSuiviCommandeClientProduit:
        case MenuName.LogistiqueMenuEtatReceptionProduit:
        case MenuName.LogistiqueMenuEtatReceptionProduitDR:
        case MenuName.LogistiqueMenuExtractionReception:
        case MenuName.LogistiqueMenuExtractionCommande:
        case MenuName.LogistiqueMenuEtatDesPoids:
        case MenuName.LogistiqueMenuEtatTonnageParProcess:
        case MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient:
        case MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient:
        case MenuName.LogistiqueMenuSuiviFacturationHCS:
        case MenuName.LogistiqueMenuTonnageCollectiviteProduit:
        case MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit:
        case MenuName.LogistiqueMenuTonnageCDTProduitComposant:
        case MenuName.LogistiqueMenuTonnageCLSousContrat:
        case MenuName.LogistiqueMenuListeProduit:
        case MenuName.LogistiqueMenuEtatReceptionEmballagePlastique:
        case MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur:
        case MenuName.LogistiqueMenuEtatDesFluxDev:
        case MenuName.LogistiqueMenuExtractionOscar:
        case MenuName.LogistiqueMenuEtatDesFluxDevLeko:
        case MenuName.AnnuaireMenuEvolutionTonnage:
          this.router.navigate(["statistique-grid"]);
          break;
        case MenuName.AnnuaireMenuSuiviEnvois:
          this.router.navigate(["statistique-grid"]);
          break;
      }
    }
  }
  //------------------------------------------------------------------------------
  //Click a module
  onModule(moduleName: string) {
    this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === moduleName);
    this.applicationUserContext.currentMenu = new appClasses.EnvMenu();
    this.moduleStyleManagement();
    //Hide modules and show menus
    this.menuModules = false;
  }
  //------------------------------------------------------------------------------
  //Click a menu
  onMenu(menuName: string) {
    this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === menuName);
    this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === this.applicationUserContext.currentMenu.moduleName);
    this.applicationUserContext.fromMenu = null;
    this.menuStyleManagement();
    this.navigateToMenu(false);
  }
  //------------------------------------------------------------------------------
  //Add item
  onNew(menuName: string) {
    this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === menuName);
    this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === this.applicationUserContext.currentMenu.moduleName);
    this.applicationUserContext.fromMenu = null;
    this.menuStyleManagement();
    this.navigateToMenu(true);
  }
  //------------------------------------------------------------------------------
  //Change profil
  onChangeProfil() {
    let data: any;
    let title: string = "";
    //Title
    title = this.applicationUserContext.getCulturedRessourceText(1500);
    //Calculate id
    data = {
      title: title, type: "Profils", changeProfil: true
    };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      data,
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      //If something have been done
      if (result) {
        if (result.ref) {
          this.router.navigate([result.ref]);
        }
      }
    })
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for messages
  showMessage() {
    //If a message exists
    //Check for messages
    this.dataModelService.existsMessageToDisplay(false)
      .subscribe(result => {
        this.applicationUserContext.hasMessage = result;
        if (result > 0) {
          //open pop-up
          const dialogRef = this.dialog.open(MessageVisuComponent, {
            width: "50%", height: "50%",
            data: { mode: "all" },
            autoFocus: false,
            closeOnNavigation: false,
            disableClose: true,
            restoreFocus: false
          });
        }
        else {
          //Inform user
          this.snackBarQueueService.addMessage({
            text: this.applicationUserContext.getCulturedRessourceText(1243)
            , duration: 4000
          } as SnackbarMsg);
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Tooltip for messages
  getHasMessageTooltipText() {
    let s: string = this.applicationUserContext.getCulturedRessourceText(1255);
    if (this.applicationUserContext.hasMessage > 0) {
      s = this.applicationUserContext.getCulturedRessourceText(1256);
    }
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Tooltip for messages
  getEntiteDR(): dataModelsInterfaces.EntiteDR {
    let entiteDR: dataModelsInterfaces.EntiteDR;
    if (this.applicationUserContext?.connectedUtilisateur?.CentreDeTri?.EntiteDRs?.length > 0) {
      entiteDR = this.applicationUserContext?.connectedUtilisateur?.CentreDeTri?.EntiteDRs[0];
    }
    return entiteDR;
  }
}
