/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 31/07/2018
/// ----------------------------------------------------------------------------------------------------- 
///

import { Injectable } from "@angular/core";
import { ModuleName, HabilitationLogistique, EnvCommandeFournisseurStatutName, CommandeFournisseurStatut, MenuName, DataColumnName } from "./enums";
import { UtilsService } from "../services/utils.service";
import { EventEmitterService } from "../services/event-emitter.service";
import moment from "moment";
import * as dataModelsInterfaces from "../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../interfaces/appInterfaces";
import * as appClasses from "../classes/appClasses";
import { Aide } from "../interfaces/dataModelsInterfaces";
import { Router } from "@angular/router";

@Injectable({
  providedIn: "root"
})
export class ApplicationUserContext {
  _connectedUtilisateur: dataModelsInterfaces.Utilisateur = {} as dataModelsInterfaces.Utilisateur;
  //Cookies
  public ckiePasswordLost: string = "";
  //Filters
  public filterText: string = "";
  public filterAnomalieCommandeFournisseur: boolean = false;
  public filterBegin: moment.Moment = moment([1, 0, 1, 0, 0, 0, 0]);
  public filterEnd: moment.Moment = moment([1, 0, 1, 0, 0, 0, 0]);
  public filterApplicationProduitOrigines: dataModelsInterfaces.ApplicationProduitOrigine[];
  public filterActionTypes: dataModelsInterfaces.ActionType[];
  public filterAdresseTypes: dataModelsInterfaces.AdresseType[];
  public filterContactAdresseProcesss: dataModelsInterfaces.ContactAdresseProcess[];
  public filterEntites: dataModelsInterfaces.EntiteList[];
  public filterFonctions: dataModelsInterfaces.Fonction[];
  public filterServices: dataModelsInterfaces.Service[];
  public filterCamionTypes: dataModelsInterfaces.CamionType[];
  public filterClients: dataModelsInterfaces.EntiteList[];
  public filterCentreDeTris: dataModelsInterfaces.EntiteList[];
  public filterCollectivites: dataModelsInterfaces.EntiteList[];
  public filterComposants: dataModelsInterfaces.ProduitList[];
  public filterDRs: dataModelsInterfaces.UtilisateurList[];
  public filterDatesTransporteur: boolean = false;
  public filterDChargementModif: boolean = false;
  public filterDptArrivees: dataModelsInterfaces.Dpt[];
  public filterDptDeparts: dataModelsInterfaces.Dpt[];
  public filterEntiteTypes: dataModelsInterfaces.EntiteType[];
  public filterDonneeEntiteSage: boolean = false;
  public filterEcoOrganismes: dataModelsInterfaces.EcoOrganisme[];
  public filterEnvCommandeFournisseurStatuts: appClasses.EnvCommandeFournisseurStatut[];
  public filterIndustriels: dataModelsInterfaces.EntiteList[];
  public filterMessageTypes: dataModelsInterfaces.MessageType[];
  public filterNonConformiteEtapeTypes: dataModelsInterfaces.NonConformiteEtapeType[];
  public filterNonConformiteNatures: dataModelsInterfaces.NonConformiteNature[];
  public filterProduitGroupeReportings: dataModelsInterfaces.ProduitGroupeReportingList[];
  public filterRegionReportings: dataModelsInterfaces.RegionReporting[];
  public filterIFClientCommandeAFaire: boolean = false;
  public filterIFClientFactureEnAttente: boolean = false;
  public filterIFFournisseurRetourLot: boolean = false;
  public filterIFFournisseurFacture: boolean = false;
  public filterIFFournisseurAttenteBonCommande: boolean = false;
  public filterIFFournisseurTransmissionFacturation: boolean = false;
  public filterNonConformitesATraiter: boolean = false;
  public filterNonConformitesATransmettre: boolean = false;
  public filterNonConformitesPlanActionAValider: boolean = false;
  public filterNonConformitesPlanActionNR: boolean = false;
  public filterPayss: dataModelsInterfaces.Pays[];
  public filterProcesss: dataModelsInterfaces.ProduitList[];
  public filterProduits: dataModelsInterfaces.ProduitList[];
  public filterControle: boolean = false;
  public filterTransporteurs: dataModelsInterfaces.EntiteList[];
  public filterPrestataires: dataModelsInterfaces.EntiteList[];
  public filterUtilisateurs: dataModelsInterfaces.UtilisateurList[];
  public filterUtilisateurMaitres: dataModelsInterfaces.UtilisateurList[];
  public filterValideDPrevues: boolean = false;
  public filterCommandesFournisseurNonChargees: boolean = false;
  public filterCommandesFournisseurAttribuees: boolean = false;
  public filterVilleArrivees: any[];
  public filterVilleDeparts: any[];
  public filterAdresseDestinations: any[];
  public filterMonths: appInterfaces.Month[];
  public filterQuarters: appInterfaces.Quarter[];
  public filterYears: appInterfaces.Year[];
  public filterMonthEnd: appInterfaces.Month;
  public filterYearEnd: appInterfaces.Year;
  public filterDR: boolean = false;
  public filterMoins10Euros: boolean = false;
  public filterCollecte: string = "SousOuHorsCollecte";
  public filterContrat: string = "SousOuHorsContrat";
  public filterDayWeekMonth: string = "Month";
  public filterFirstLogin: boolean = true;
  public filterEE: number = null;
  public visibleFilterDR: boolean = false;
  public filterActif: boolean = false;
  public filterGlobalActif: boolean = true;
  public filterNonRepartissable: boolean = null;
  public filterRepartitionAFinaliser: boolean = false;
  public filterEmailType: any = null;
  public filterContactSelectedColumns: appClasses.EnvDataColumn[] = [];
  //Navigation
  public fromMenu: appClasses.EnvMenu = null;
  //Selections
  public selectedItem: number[];
  //Session objects
  commandeClientFormRefEntite: number;
  commandeClientFormRefContrat: number;
  commandeClientFormRefAdresse: number;
  commandeClientFormD: moment.Moment;
  prixRepriseRefProcess: number;
  prixRepriseD: moment.Moment;
  //Misc
  refUtilisateurMaitre: number = 0;   //First login
  profilSelected: boolean = false;
  hasMessage: number = 0;
  currentCultureName: string = "fr-FR";
  currentModule: appClasses.EnvModule = new appClasses.EnvModule();
  currentMenu: appClasses.EnvMenu = new appClasses.EnvMenu();
  statType: string = "";
  currentColorIndex: number = 0;
  visibleHeaderCollectivite: boolean = false;
  visibleHelp: boolean = false;
  //Animation
  animMessage: boolean = false;
  //Local data
  aides: Aide[] = [];
  //-----------------------------------------------------------------------------------
  //Constructor

  constructor(private utilsService: UtilsService
    , private router: Router
    , private eventEmitterService: EventEmitterService,) {
    if (this.parametres?.length ?? 0 == 0) { this.initParametres(); }
    if (this.culturedRessources?.length ?? 0 == 0) { this.initCulturedRessources(); }
    if (this.resources?.length ?? 0 == 0) { this.initResources(); }
    if (this.envModules?.length ?? 0 == 0) { this.initEnvModules(); }
    if (this.envMenus?.length ?? 0 == 0) { this.initEnvMenus(); }
    if (this.envDataColumns?.length ?? 0 == 0) { this.initEnvDataColumns(); }
    if (this.envComponents?.length ?? 0 == 0) { this.initEnvComponents(); }
  }
  //Curently connected user
  get connectedUtilisateur(): dataModelsInterfaces.Utilisateur {
    return this._connectedUtilisateur;
  }
  set connectedUtilisateur(value: dataModelsInterfaces.Utilisateur) {
    this._connectedUtilisateur = value;
    //RAZ filters
    this.razFilters();
    //Setting personnal parameters
    this.setDefaultModule();
    this.initFilters();
    //Getting specific data
    this.initEnvCommandeFournisseurStatuts();
    //Emit event for app compoment refresh (active menus and modules)
    this.eventEmitterService.onRedirectToHome();
  }
  //-----------------------------------------------------------------------------------------------------
  //Autologin
  //devLogin = ""; devPwd = "";
  devLogin = "jean-christian.nugues@enviromatic.fr"; devPwd = "evalo1rplast$";
  //devLogin = "jean-christian.nugues@wanadoo.fr"; devPwd = "1Toto$aaaaaa";
  //devLogin = "j.decamas@valorplast.com"; devPwd = "1Toto$aaaaaa";
  //devLogin = "chantal.guerrero@suez.com"; devPwd = "1Toto$aaaaaa";      //CDT TRIVALOIRE
  //devLogin = "azimmer@schroll.fr"; devPwd = "1Toto$aaaaaa";      //CDT ALTEM
  //devLogin = "64AK"; devPwd = "1Toto$aaaaaa";      //CDT VALORBEARN (NDC qualité)
  //devLogin = "azimmer@schroll.fr"; devPwd = "1Toto$aaaaaa";      //CDT ALTEM
  //devLogin = "nicolas.stasiak@sicovad.fr"; devPwd = "1Toto$aaaaaa";      //CDT SICOVAD
  //devLogin = "dechetterie_fmg@terrestouloises.com"; devPwd = "1Toto$aaaaaa";      //CDT CC DES TERRES TOULOISES
  //devLogin = "v.vauzelle@transport-ltr.com"; devPwd = "1Toto$aaaaaa";      //Transporteur
  //devLogin = "laurent.roucoulet@wanadoo.fr"; devPwd = "AZF47LMJD";      //Transporteur
  //devLogin = "n.cadue@valorplast.com"; devPwd = "1Toto$aaaaaa";      //DR
  //devLogin = "lefebvre"; devPwd = "ELghd782LEF!";      //Client
  //devLogin = "dla"; devPwd = "dla";      //Client Freudenberg
  //devLogin = "gregory.dilhuidy@nantesmetropole.fr"; devPwd = "1Toto$aaaaaa";      //Collectivité Nantes CS
  //devLogin = "AROUN@syctom-paris.fr"; devPwd = "1Toto$aaaaaa";      //Collectivité Paris HCS
  //devLogin = "tommy.nombalay@agglo2b.fr"; devPwd = "1Toto$aaaaaa";      //Collectivité Bocage Bressuirais CS+HCS
  //devLogin = "CDST"; devPwd = "@15TyKUP9j$8";      //CITEO
  //devLogin = "lydie.malioche@veolia.com"; devPwd = "1Toto$aaaaaa";      //CITEO

  parametres: dataModelsInterfaces.Parametre[] = [];
  parametres22: dataModelsInterfaces.Parametre[] = [];
  envModules: appClasses.EnvModule[] = [];
  envMenus: appClasses.EnvMenu[] = [];
  envComponents: appClasses.EnvComponent[] = [];
  envDataColumns: appClasses.EnvDataColumn[] = [];
  contactAvailableColumns: appClasses.EnvDataColumn[] = [];
  environment: string;
  culturedRessources: appInterfaces.CulturedRessource[] = [];
  resources: appInterfaces.Resource[] = [];
  envCommandeFournisseurStatuts: appClasses.EnvCommandeFournisseurStatut[] = [];
  nonConformiteEtapeTypes: dataModelsInterfaces.NonConformiteEtapeType[] = [];
  //-----------------------------------------------------------------------------------------------------
  //Init parameters
  public setDefaultModule() {
    this.currentModule = new appClasses.EnvModule();
    if (this._connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur) {
      this.currentModule = this.envModules.find(x => x.name === ModuleName.Logistique);
    }
    if (this._connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client) {
      this.currentModule = this.envModules.find(x => x.name === ModuleName.Logistique);
    }
    if (this._connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.CentreDeTri) {
      this.currentModule = this.envModules.find(x => x.name === ModuleName.Logistique);
    }
    if (this._connectedUtilisateur.Prestataire?.RefEntite) {
      this.currentModule = this.envModules.find(x => x.name === ModuleName.ModulePrestataire);
    }
    if (this._connectedUtilisateur.Collectivite?.RefEntite) {
      this.currentModule = this.envModules.find(x => x.name === ModuleName.ModuleCollectivite);
    }
  }
  public initFilters() {
    this.filterEnvCommandeFournisseurStatuts = [];
    this.filterTransporteurs = [];
    this.filterClients = [];
    this.filterCentreDeTris = [];
    this.filterCollectivites = [];
    if (this._connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur
      || this._connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
    ) {
      this.filterEnvCommandeFournisseurStatuts = [];
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.EnAttente));
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
    }
    if (this._connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur
    ) {
      this.filterEnvCommandeFournisseurStatuts = [];
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      this.filterBegin = moment([1, 0, 1, 0, 0, 0, 0]);
      this.filterEnd = moment([1, 0, 1, 0, 0, 0, 0]);
      this.filterTransporteurs = [];
      this.filterTransporteurs.push(this.connectedUtilisateur.Transporteur);
    }
    if (this._connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client
    ) {
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Bloquee));
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Receptionnee));
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Refusee));
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Repartie));
      this.filterBegin = moment([1, 0, 1, 0, 0, 0, 0]);
      this.filterEnd = moment([1, 0, 1, 0, 0, 0, 0]);
      this.filterClients = [];
      this.filterClients.push(this.connectedUtilisateur.Client);
    }
  }
  public setDefaultFilters() {
    if (this._connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur
    ) {
      this.filterEnvCommandeFournisseurStatuts = [];
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      this.filterTransporteurs = [];
      this.filterTransporteurs.push(this.connectedUtilisateur.Transporteur);
    }
    if (this._connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client
    ) {
      this.filterEnvCommandeFournisseurStatuts = [];
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Ouverte));
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Bloquee));
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Receptionnee));
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Refusee));
      this.filterEnvCommandeFournisseurStatuts.push(this.envCommandeFournisseurStatuts.find(x => x.name === EnvCommandeFournisseurStatutName.Repartie));
      this.filterClients = [];
      this.filterClients.push(this.connectedUtilisateur.Client);
    }
    if (this._connectedUtilisateur.Prestataire
    ) {
      this.filterPrestataires = [];
      this.filterPrestataires.push(this.connectedUtilisateur.Prestataire);
    }
    if (this._connectedUtilisateur.CentreDeTri
    ) {
      this.filterCentreDeTris = [];
      this.filterCentreDeTris.push(this.connectedUtilisateur.CentreDeTri);
    }
    //Dev
  //  if (this.environment) {
  //    this.filterCollectivites = [];
  //    this.filterCollectivites.push({ RefEntite: 1004 } as dataModelsInterfaces.Entite)
  //  }
  }
  // -----------------------------------------------------------------------------------------------------
  //Init app resources
  public initParametres(): Promise<void> {
    this.parametres = [];
    return this.utilsService.getParametres().toPromise().then(result => {
      //Localized ressources
      this.parametres = result;
    });
  }
  // -----------------------------------------------------------------------------------------------------
  //Init cultured ressources
  public initCulturedRessources(): Promise<void> {
    this.culturedRessources = [];
    return this.utilsService.getCulturedRessources(this.currentCultureName).toPromise().then(result => {
      //Localized ressources
      this.culturedRessources = result;
    });
  }
  // -----------------------------------------------------------------------------------------------------
  //Init app resources
  public initResources(): Promise<void> {
    this.resources = [];
    return this.utilsService.getResources().toPromise().then(result => {
      //Localized ressources
      this.resources = result;
    });
  }
  // -----------------------------------------------------------------------------------------------------
  //Init modules
  public initEnvModules(): Promise<void> {
    this.envModules = [];
    return this.utilsService.getEnvModules(this.currentCultureName).toPromise().then(result => {
      //App modules
      this.envModules = result;
    });
  }
  // -----------------------------------------------------------------------------------------------------
  //Init menus
  public initEnvMenus(): Promise<void> {
    this.envMenus = [];
    return this.utilsService.getEnvMenus(this.currentCultureName).toPromise().then(result => {
      //App Menus
      this.envMenus = result;
    });
  }
  // -----------------------------------------------------------------------------------------------------
  //Init components
  public initEnvComponents(): Promise<void> {
    this.envComponents = [];
    return this.utilsService.getEnvComponents(this.currentCultureName).toPromise().then(result => {
      //App Menus
      this.envComponents = result;
    });
  }
  // -----------------------------------------------------------------------------------------------------
  //Init data columns
  public initEnvDataColumns(): Promise<void> {
    this.envDataColumns = [];
    return this.utilsService.getEnvDataColumns(this.currentCultureName).toPromise().then(result => {
      //App DataColumns
      this.envDataColumns = result;
      //Create data columns arrays
      this.contactAvailableColumns = [];
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.Id));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteCodeCITEO));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteSousContrat));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteTypeLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteCapacite));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteActionnaireProprietaire));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteExploitant));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteDimensionBalle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EquipementierLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.FournisseurTOLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntitePopulationContratN));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteVisibiliteAffretementCommun));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteSurcoutCarburant));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ListeDR));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeProcesss));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeStandards));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteCmt));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteCodeValorisation));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EcoOrganismeLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteRepartitionMensuelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteAssujettiTVA));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteCodeTVA));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteIdNational));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteSAGECodeComptable));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteSAGECompteTiers));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.SAGECategorieAchatLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.SAGEConditionLivraisonLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.SAGEPeriodiciteLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.SAGEModeReglementLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.SAGECategorieVenteLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteExportSAGE));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.MiseEnPlaceAutocontrole));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteReconductionIncitationQualite));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.RepriseTypeLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.RepreneurLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContratCollectiviteDebut));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContratCollectiviteFin));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeContratCollectivites));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.SousContratIncitationQualite));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContratIncitationQualiteDebut));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContratIncitationQualiteFin));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeProduits));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeProduitInterdits));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeCollectiviteActifs));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeCollectiviteInactifs));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.CentreDeTriListeCamionTypes));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.TransporteurListeCamionTypes));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeCentreDeTriActifs));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeCentreDeTriInactifs));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeCentreDeTriActifInterdits));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteTransporteurActifInterdits));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeClientActifInterdits));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeIndustrielActifs));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeDocumentValorplast));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.EntiteListeDocumentPublic));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseTypeLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseAdr1));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseAdr2));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseCodePostal));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseVille));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.PaysLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseHoraires));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseTel));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseEmail));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.AdresseSiteWeb));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.TitreLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.CiviliteLibelle));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactPrenom));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactNom));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactAdresseCmt));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactAdresseListeFonctionServices));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactAdresseCmtServiceFonction));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactAdresseTel));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactAdresseTelMobile));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactAdresseEmail));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactAdresseListeProcesss));
      this.contactAvailableColumns.push(this.envDataColumns.find(i => i.name == DataColumnName.ContactAdresseIsUser));
    });
  }
// -----------------------------------------------------------------------------------------------------
  //Init CommandeFournisseurStatuts
  public initEnvCommandeFournisseurStatuts(): Promise<void> {
    this.envCommandeFournisseurStatuts = [];
    return this.utilsService.getEnvCommandeFournisseurStatuts(this.currentCultureName).toPromise().then(result => {
      this.envCommandeFournisseurStatuts = result;
    });
  }
  // -----------------------------------------------------------------------------------------------------
  //Init NonConformiteEtapeTypes
  public initNonConformiteEtapeTypes(): Promise<void> {
    this.nonConformiteEtapeTypes = [];
    return this.utilsService.getNonConformiteEtapeType(this.currentCultureName).toPromise().then(result => {
      this.nonConformiteEtapeTypes = result;
    });
  }
  //-----------------------------------------------------------------------------------
  //Get environment type
  getEnvironment(): Promise<void> {
    return this.utilsService.getEnvironment().toPromise().then(result => {
      //Set Server environment
      this.environment = result;
    });
  }
  // -----------------------------------------------------------------------------------------------------
  //Disconnect
  public disconnect(): void {
    console.log("disconnect");
    this.connectedUtilisateur = {} as dataModelsInterfaces.Utilisateur;
    this.currentModule = new appClasses.EnvModule();
    this.currentMenu = new appClasses.EnvMenu();
    this.razFilters()
    this.profilSelected = false;
    //Selections
    this.selectedItem = [];
    //Route to login if necessary
    if (this.router.url != "/login") {
      this.router.navigate(["login"]);
    }
  }
  // -----------------------------------------------------------------------------------------------------
  //RAZ menus/dashboard specific filters
  public razMenuSpecificFilters(): void {
    this.filterAnomalieCommandeFournisseur = false;
    this.filterDatesTransporteur = false;
    this.filterDChargementModif = false;
    this.filterValideDPrevues = false;
    this.filterCommandesFournisseurNonChargees = false;
    this.filterCommandesFournisseurAttribuees = false;
  }
  // -----------------------------------------------------------------------------------------------------
  //RAZ filters
  public razFilters(): void {
    this.filterText = "";
    this.filterBegin = moment([1, 0, 1, 0, 0, 0, 0]);
    this.filterEnd = moment([1, 0, 1, 0, 0, 0, 0]);
    this.filterActionTypes = [];
    this.filterAdresseTypes = [];
    this.filterContactAdresseProcesss = [];
    this.filterEntites = [];
    this.filterFonctions = [];
    this.filterServices = [];
    this.filterCamionTypes = [];
    this.filterAnomalieCommandeFournisseur = false;
    this.filterApplicationProduitOrigines = [];
    this.filterCentreDeTris = [];
    this.filterClients = [];
    this.filterCollectivites = [];
    this.filterComposants = [];
    this.filterDRs = [];
    this.filterDatesTransporteur = false;
    this.filterDChargementModif = false;
    this.filterDptDeparts = [];
    this.filterDptArrivees = [];
    this.filterEntiteTypes = [];
    this.filterDonneeEntiteSage = false;
    this.filterEcoOrganismes = [];
    this.filterEnvCommandeFournisseurStatuts = [];
    this.filterIndustriels = [];
    this.filterMessageTypes = [];
    this.filterNonConformiteEtapeTypes = [];
    this.filterNonConformiteNatures = [];
    this.filterNonConformiteNatures = [];
    this.filterProduitGroupeReportings = [];
    this.filterPayss = [];
    this.filterProcesss = [];
    this.filterProduits = [];
    this.filterRegionReportings = [];
    this.filterControle = false;
    this.filterTransporteurs = [];
    this.filterPrestataires = [];
    this.filterUtilisateurs = [];
    this.filterUtilisateurMaitres = [];
    this.filterValideDPrevues = false;
    this.filterCommandesFournisseurNonChargees = false;
    this.filterCommandesFournisseurAttribuees = false;
    this.filterVilleDeparts = [];
    this.filterAdresseDestinations = [];
    this.filterVilleArrivees = [];
    this.filterMonths = [];
    this.filterQuarters = [];
    this.filterYears = [];
    this.filterMonthEnd = null;
    this.filterYearEnd = null;
    this.filterIFClientCommandeAFaire = false;
    this.filterIFClientFactureEnAttente = false;
    this.filterIFFournisseurRetourLot = false;
    this.filterIFFournisseurFacture = false;
    this.filterIFFournisseurAttenteBonCommande = false;
    this.filterIFFournisseurTransmissionFacturation = false;
    this.filterNonConformitesATraiter = false;
    this.filterNonConformitesATransmettre = false;
    this.filterNonConformitesPlanActionAValider = false;
    this.filterNonConformitesPlanActionNR = false;
    this.filterActif = false;
    //this.filterGlobalActif = true;
    this.filterNonRepartissable = null;
    this.filterCollecte = "DansOuHorsCollecte";
    this.filterContrat = "SousOuHorsContrat";
    this.filterDayWeekMonth = "Month";
    this.filterFirstLogin = true;
    this.filterEE = null;
    this.filterEmailType = null;
    this.filterContactSelectedColumns = [];
    this.filterRepartitionAFinaliser = false;
    //Set default mandatory filters
    this.setDefaultFilters();
  }
  // -----------------------------------------------------------------------------------------------------
  //Get localized caption of data column
  public getEnvDataColulmnCulturedCaption(name: string): string {
    let s: string = "?" + name + "?";
    if (name) {
      let envDataColumn = this.envDataColumns.find(x => x.name === name);
      if (envDataColumn) {
        s = envDataColumn.culturedCaption;
      }
    }
    return s;
  }
  // -----------------------------------------------------------------------------------------------------
  //Get module name for the current language
  public getEnvModuleCulturedCaption(name: string): string {
    let s: string = "?" + name + "?";
    if (name) {
      let envModule = this.envModules.find(x => x.name === name);
      if (envModule) {
        s = envModule.culturedCaption;
      }
    }
    return s;
  }
  // -----------------------------------------------------------------------------------------------------
  //Get menu name for the current language
  public getEnvMenuCulturedCaption(name: string): string {
    let s: string = "?" + name + "?";
    if (name) {
      let envMenu = this.envMenus.find(x => x.name === name);
      if (envMenu) {
        s = envMenu.culturedCaption;
      }
    }
    return s;
  }
  // -----------------------------------------------------------------------------------------------------
  //Get text from cultured ressource
  public getCulturedRessourceText(refRessource: number): string {
    let s: string = "---???---";
    if (refRessource) {
      let cultruredRessource = this.culturedRessources.find(x => x.refRessource === refRessource);
      if (cultruredRessource) {
        s = cultruredRessource.textRessource;
      }
    }
    return s;
  }
  // -----------------------------------------------------------------------------------------------------
  //Get text from cultured ressource
  public getCulturedRessourceTextJavascrip(refRessource: number): string {
    let s: string = "---???---";
    if (refRessource) {
      let cultruredRessource = this.culturedRessources.find(x => x.refRessource === refRessource);
      if (cultruredRessource) {
        s = cultruredRessource.textJavascriptRessource;
      }
    }
    return s;
  }
  // -----------------------------------------------------------------------------------------------------
  //Get text from cultured ressource
  public getCulturedRessourceHTML(refRessource: number): string {
    let s: string = "---???---";
    if (refRessource) {
      let cultruredRessource = this.culturedRessources.find(x => x.refRessource === refRessource);
      if (cultruredRessource) {
        s = cultruredRessource.hTMLRessource;
      }
    }
    return s;
  }
  // -----------------------------------------------------------------------------------------------------
  //Get text from cultured ressource
  public getCulturedRessourceHTMLButton(refRessource: number): string {
    let s: string = "---???---";
    if (refRessource) {
      let cultruredRessource = this.culturedRessources.find(x => x.refRessource === refRessource);
      if (cultruredRessource) {
        s = cultruredRessource.hTMLButtonRessource;
      }
    }
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Get the API URL of a component
  public getURLforComponent(component: string): string {
    // Retrieves the URL from Component settings
    let url = "";
    let cmp = this.envComponents.find(x => x.name === component)
    if (cmp) {
      url = cmp.url;
    }
    return url;
  }
}
