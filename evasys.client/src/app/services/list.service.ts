/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 22/05/2019
/// ----------------------------------------------------------------------------------------------------- 
///
import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";
import moment from "moment";
import * as dataModelsInterfaces from "../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../interfaces/appInterfaces";
import { ApplicationUserContext } from '../globals/globals';

//-----------------------------------------------------------------------------------
//Lists services
@Injectable()
export class ListService {
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private http: HttpClient
    , @Inject("BASE_URL") private baseUrl: string
    , public applicationUserContext: ApplicationUserContext
  ) { }
  //Get generic list from component name
  getList<T extends { AllowStandardCRUD: string }>(component: string): Observable<T[]> {
    return this.getGenericList<T>(component, null);
  }
  private getGenericList<T>(component: string, headers: HttpHeaders): Observable<T[]> {
    if (headers) {
      return this.http.get<T[]>(this.baseUrl + this.applicationUserContext.getURLforComponent(component) + "/getlist", {
        headers: headers,
        responseType: "json"
      });
    }
    else {
      return this.http.get<T[]>(this.baseUrl + this.applicationUserContext.getURLforComponent(component) + "/getlist", {
        responseType: "json"
      });
    }
  }
  //-----------------------------------------------------------------------------------
  //Get existing ContratType
  getListContratType(): Observable<dataModelsInterfaces.ContratType[]> {
    return this.http.get<dataModelsInterfaces.ContratType[]>(this.baseUrl + "evapi/contrattype/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing AdresseType
  getListAdresseType(): Observable<dataModelsInterfaces.AdresseType[]> {
    return this.http.get<dataModelsInterfaces.AdresseType[]>(this.baseUrl + "evapi/adressetype/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing EcoOrganisme
  getListEcoOrganisme(): Observable<dataModelsInterfaces.EcoOrganisme[]> {
    return this.http.get<dataModelsInterfaces.EcoOrganisme[]>(this.baseUrl + "evapi/ecoorganisme/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing EquivalentCO2
  getListEquivalentCO2(actif: boolean): Observable<dataModelsInterfaces.EquivalentCO2[]> {
    return this.http.get<dataModelsInterfaces.EquivalentCO2[]>(this.baseUrl + "evapi/equivalentco2/getlist", {
      headers: new HttpHeaders()
        .set("actif", actif === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing RegionReporting
  getListRegionReporting(): Observable<dataModelsInterfaces.RegionReporting[]> {
    return this.getGenericList<dataModelsInterfaces.RegionReporting>("RegionReportingComponent", null);
  }
  //-----------------------------------------------------------------------------------
  //Get existing RegionEEDpt
  getListRegionEEDpt(): Observable<dataModelsInterfaces.RegionEEDpt[]> {
    return this.getGenericList<dataModelsInterfaces.RegionEEDpt>("RegionEEDptComponent", null);
  }
  //-----------------------------------------------------------------------------------
  //Get existing ActionType
  getListActionType(refEntiteType: number): Observable<dataModelsInterfaces.ActionType[]> {
    return this.http.get<dataModelsInterfaces.ActionType[]>(this.baseUrl + "evapi/actiontype/getlist", {
      params: new HttpParams()
        .set("refEntiteType", refEntiteType ? refEntiteType.toString() : "0"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Entite
  getListEntite(refEntite: number, refEntiteType: number, textFilter: string, actif: boolean): Observable<dataModelsInterfaces.EntiteList[]> {
    return this.http.get<dataModelsInterfaces.EntiteList[]>(this.baseUrl + "evapi/entite/getlist", {
      params: new HttpParams()
        .set("refEntiteType", refEntiteType ? refEntiteType.toString() : "0")
        .set("textFilter", textFilter),
      headers: new HttpHeaders()
        .set("refEntite", refEntite == null ? "" : refEntite.toString())
        .set("actif", actif === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing centre de tri
  getListCentreDeTri(refCentreDeTri: number, refEntite: number, actif: boolean): Observable<dataModelsInterfaces.EntiteList[]> {
    return this.http.get<dataModelsInterfaces.EntiteList[]>(this.baseUrl + "evapi/entite/getlist", {
      params: new HttpParams()
        .set("refEntiteType", "3"),
      headers: new HttpHeaders()
        .set("refEntite", refCentreDeTri == null ? "" : refCentreDeTri.toString())
        .set("refEntiteRtt", refEntite == null ? "" : refEntite.toString())
        .set("actif", actif === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing ContactAdresseProcess
  getListContactAdresseProcess(): Observable<dataModelsInterfaces.ContactAdresseProcess[]> {
    return this.http.get<dataModelsInterfaces.ContactAdresseProcess[]>(this.baseUrl + "evapi/contactadresseprocess/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Industriel
  getListIndustriel(refIndustriel: number, actif: boolean): Observable<dataModelsInterfaces.EntiteList[]> {
    return this.http.get<dataModelsInterfaces.EntiteList[]>(this.baseUrl + "evapi/entite/getlist", {
      params: new HttpParams()
        .set("refEntiteType", "5"),
      headers: new HttpHeaders()
        .set("refEntite", refIndustriel == null ? "" : refIndustriel.toString())
        .set("actif", actif === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Equipementier
  getListEquipementier(): Observable<dataModelsInterfaces.Equipementier[]> {
    return this.http.get<dataModelsInterfaces.Equipementier[]>(this.baseUrl + "evapi/equipementier/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing FournisseurTO
  getListFournisseurTO(): Observable<dataModelsInterfaces.FournisseurTO[]> {
    return this.http.get<dataModelsInterfaces.FournisseurTO[]>(this.baseUrl + "evapi/Fournisseurto/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing transporteur
  getListTransporteur(refTransporteur: number, surcoutCarburantHT: boolean, actif: boolean): Observable<dataModelsInterfaces.EntiteList[]> {
    return this.http.get<dataModelsInterfaces.EntiteList[]>(this.baseUrl + "evapi/entite/getlist", {
      params: new HttpParams()
        .set("refEntiteType", "2"),
      headers: new HttpHeaders()
        .set("refEntite", refTransporteur == null ? "" : refTransporteur.toString())
        .set("actif", actif === true ? "true" : "false")
        .set("surcoutCarburantHT", surcoutCarburantHT === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing prestataire
  getListPrestataire(refPrestataire: number, actif: boolean): Observable<dataModelsInterfaces.EntiteList[]> {
    return this.http.get<dataModelsInterfaces.EntiteList[]>(this.baseUrl + "evapi/entite/getlist", {
      params: new HttpParams()
        .set("refEntiteType", "8"),
      headers: new HttpHeaders()
        .set("refEntite", refPrestataire == null ? "" : refPrestataire.toString())
        .set("actif", actif === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Dpt
  getListDpt(): Observable<dataModelsInterfaces.Dpt[]> {
    return this.http.get<dataModelsInterfaces.Dpt[]>(this.baseUrl + "evapi/dpt/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing adresse
  getListAdresse(refAdresse: number, refEntite: number, refContactAdresseProcess: number, refProduit: number, d: moment.Moment
    , refEntiteType: number, actif: boolean): Observable<dataModelsInterfaces.Adresse[]> {
    return this.http.get<dataModelsInterfaces.Adresse[]>(this.baseUrl + "evapi/adresse/getlist", {
      headers: new HttpHeaders()
        .set("refAdresse", refAdresse == null ? "" : refAdresse.toString())
        .set("refEntite", refEntite == null ? "" : refEntite.toString())
        .set("refContactAdresseProcess", refContactAdresseProcess == null ? "" : refContactAdresseProcess.toString())
        .set("refProduit", refProduit == null ? "" : refProduit.toString())
        .set("d", d == null ? "" : moment(d).format("YYYY-MM-DD 00:00:00.000"))
        .set("refEntiteType", refEntiteType == null ? "" : refEntiteType.toString())
        .set("actif", actif == true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing contactadresse
  getListContactAdresse(refContactAdresse: number, refEntite: number, refAdresse: number, refContactAdresseProcess: number, actif: boolean): Observable<dataModelsInterfaces.ContactAdresse[]> {
    return this.http.get<dataModelsInterfaces.ContactAdresse[]>(this.baseUrl + "evapi/contactadresse/getlist", {
      headers: new HttpHeaders()
        .set("refContactAdresse", refContactAdresse == null ? "" : refContactAdresse.toString())
        .set("refEntite", refEntite == null ? "" : refEntite.toString())
        .set("refAdresse", refAdresse == null ? "" : refAdresse.toString())
        .set("refContactAdresseProcess", refContactAdresseProcess == null ? "" : refContactAdresseProcess.toString())
        .set("actif", actif == true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing ville départ
  getListVilleDepart(): Observable<dataModelsInterfaces.EntiteList[]> {
    return this.http.get<dataModelsInterfaces.EntiteList[]>(this.baseUrl + "evapi/adresse/getlistville", {
      headers: new HttpHeaders()
        .set("villeCategory", "TransportDepart"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing ville arrivée
  getListVilleArrivee(): Observable<dataModelsInterfaces.EntiteList[]> {
    return this.http.get<dataModelsInterfaces.EntiteList[]>(this.baseUrl + "evapi/adresse/getlistville", {
      headers: new HttpHeaders()
        .set("villeCategory", "TransportArrivee"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing ParamEmail
  getListParamEmail(refParamEmail: number, actif: boolean, defaut: boolean): Observable<dataModelsInterfaces.ParamEmailList[]> {
    return this.http.get<dataModelsInterfaces.ParamEmailList[]>(this.baseUrl + "evapi/paramemail/getlist", {
      headers: new HttpHeaders()
        .set("refParamEmail", refParamEmail == null ? "" : refParamEmail.toString())
        .set("actif", actif == true ? "true" : "false")
        .set("defaut", defaut == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Pays
  getListPays(refPays: number, actif: boolean): Observable<dataModelsInterfaces.Pays[]> {
    return this.http.get<dataModelsInterfaces.Pays[]>(this.baseUrl + "evapi/pays/getlist", {
      headers: new HttpHeaders()
        .set("refPays", refPays == null ? "" : refPays.toString())
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing process
  getListProcess(refProcess: number, refEntite: number): Observable<dataModelsInterfaces.Process[]> {
    return this.http.get<dataModelsInterfaces.Process[]>(this.baseUrl + "evapi/process/getlist", {
      headers: new HttpHeaders()
        .set("refEntite", refEntite == null ? "" : refEntite.toString())
        .set("refProcess", refProcess == null ? "" : refProcess.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing produit
  getListProduit(refProduit: number, refEntite: number, refProduitCompose: number, refProduitARepartir: number
    , dRepartition: moment.Moment, refProcess: number, refCollectivite: number, composant: boolean, actif: boolean): Observable<dataModelsInterfaces.Produit[]> {
    return this.http.get<dataModelsInterfaces.Produit[]>(this.baseUrl + "evapi/produit/getlist", {
      headers: new HttpHeaders()
        .set("refProduit", refProduit == null ? "" : refProduit.toString())
        .set("refEntite", refEntite == null ? "" : refEntite.toString())
        .set("refCollectivite", refCollectivite == null ? "" : refCollectivite.toString())
        .set("refProduitCompose", refProduitCompose == null ? "" : refProduitCompose.toString())
        .set("refProduitARepartir", refProduitARepartir == null ? "" : refProduitARepartir.toString())
        .set("dRepartition", dRepartition == null ? "" : moment(dRepartition).format("YYYY-MM-DD 00:00:00.000"))
        .set("refProcess", refProcess == null ? "" : refProcess.toString())
        .set("composant", composant == false ? "false" : "true")
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing ProduitGroupeReporting
  getListProduitGroupeReporting(): Observable<dataModelsInterfaces.ProduitGroupeReportingList[]> {
    return this.http.get<dataModelsInterfaces.ProduitGroupeReportingList[]>(this.baseUrl + "evapi/produitgroupereporting/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing ModeTransportEE
  getListModeTransportEE(): Observable<dataModelsInterfaces.ModeTransportEE[]> {
    return this.http.get<dataModelsInterfaces.ModeTransportEE[]>(this.baseUrl + "evapi/modetransportee/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing ProduitGroupeReportingType
  getListProduitGroupeReportingType(): Observable<dataModelsInterfaces.ProduitGroupeReportingType[]> {
    return this.http.get<dataModelsInterfaces.ProduitGroupeReportingType[]>(this.baseUrl + "evapi/produitgroupereportingtype/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Couleur
  getListCouleur(): Observable<dataModelsInterfaces.Couleur[]> {
    return this.http.get<dataModelsInterfaces.Couleur[]>(this.baseUrl + "evapi/couleur/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing standard
  getListStandard(refStandard: number, refEntite: number): Observable<dataModelsInterfaces.Standard[]> {
    return this.http.get<dataModelsInterfaces.Standard[]>(this.baseUrl + "evapi/standard/getlist", {
      headers: new HttpHeaders()
        .set("refEntite", refEntite == null ? "" : refEntite.toString())
        .set("refStandard", refStandard == null ? "" : refStandard.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Utilisateur
  getListUtilisateur(refUtilisateur: number, module: string, habilitation: string, refClient: number
    , getProfils: boolean, getAvailableEsclaves: boolean, getMaitres: boolean, actif: boolean): Observable<dataModelsInterfaces.UtilisateurList[]> {
    return this.http.get<dataModelsInterfaces.UtilisateurList[]>(this.baseUrl + "evapi/utilisateur/getlist", {
      headers: new HttpHeaders()
        .set("refUtilisateur", refUtilisateur == null ? "" : refUtilisateur.toString())
        .set("module", module == null ? "" : module)
        .set("habilitation", habilitation == null ? "" : habilitation)
        .set("refClient", refClient == null ? "" : refClient.toString())
        .set("getProfils", getProfils == false ? "false" : "true")
        .set("getAvailableEsclaves", getAvailableEsclaves == false ? "false" : "true")
        .set("getMaitres", getMaitres == false ? "false" : "true")
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing month
  getListMonth(): Observable<appInterfaces.Month[]> {
    return this.http.get<appInterfaces.Month[]>(this.baseUrl + "evapi/utils/getmonthlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing quarter
  getListQuarter(): Observable<appInterfaces.Quarter[]> {
    return this.http.get<appInterfaces.Quarter[]>(this.baseUrl + "evapi/utils/getquarterlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing year
  getListYear(): Observable<appInterfaces.Year[]> {
    return this.http.get<appInterfaces.Year[]>(this.baseUrl + "evapi/utils/getyearlist", {
      headers: new HttpHeaders()
        .set("menuName", !this.applicationUserContext.currentMenu ? "" : this.applicationUserContext.currentMenu.name)
        .set("statType", !this.applicationUserContext.statType ? "" : this.applicationUserContext.statType),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing month for DdMoisDechargementPrevu
  getListdMoisDechargementPrevuMonth(d: moment.Moment): Observable<appInterfaces.Month[]> {
    return this.http.get<appInterfaces.Month[]>(this.baseUrl + "evapi/utils/getmonthlist", {
      headers: new HttpHeaders()
        .set("d", moment(d).format("YYYY-MM-DD 00:00:00.000")),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get yes/no
  getYesNoList(): Observable<appInterfaces.YesNo[]> {
    return this.http.get<appInterfaces.YesNo[]>(this.baseUrl + "evapi/utils/getyesnolist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing MotifCamionIncomplet
  getListMotifCamionIncomplet(): Observable<dataModelsInterfaces.MotifCamionIncomplet[]> {
    return this.http.get<dataModelsInterfaces.MotifCamionIncomplet[]>(this.baseUrl + "evapi/motifcamionincomplet/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing MotifAnomalieChargement
  getListMotifAnomalieChargement(): Observable<dataModelsInterfaces.MotifAnomalieChargement[]> {
    return this.http.get<dataModelsInterfaces.MotifAnomalieChargement[]>(this.baseUrl + "evapi/motifanomaliechargement/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing MotifAnomalieClient
  getListMotifAnomalieClient(): Observable<dataModelsInterfaces.MotifAnomalieClient[]> {
    return this.http.get<dataModelsInterfaces.MotifAnomalieClient[]>(this.baseUrl + "evapi/motifanomalieclient/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing MotifAnomalieTransporteur
  getListMotifAnomalieTransporteur(): Observable<dataModelsInterfaces.MotifAnomalieTransporteur[]> {
    return this.http.get<dataModelsInterfaces.MotifAnomalieTransporteur[]>(this.baseUrl + "evapi/motifanomalietransporteur/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing CommandeFournisseurStatut
  getListCommandeFournisseurStatut(): Observable<dataModelsInterfaces.CommandeFournisseurStatut[]> {
    return this.http.get<dataModelsInterfaces.CommandeFournisseurStatut[]>(this.baseUrl + "evapi/commandefournisseurstatut/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing ApplicationProduitOrigine
  getListApplicationProduitOrigine(): Observable<dataModelsInterfaces.ApplicationProduitOrigine[]> {
    return this.http.get<dataModelsInterfaces.ApplicationProduitOrigine[]>(this.baseUrl + "evapi/applicationproduitorigine/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing CamionType
  getListCamionType(): Observable<dataModelsInterfaces.CamionType[]> {
    return this.http.get<dataModelsInterfaces.CamionType[]>(this.baseUrl + "evapi/camiontype/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Civilite
  getListCivilite(): Observable<dataModelsInterfaces.Civilite[]> {
    return this.http.get<dataModelsInterfaces.Civilite[]>(this.baseUrl + "evapi/civilite/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Fonction
  getListFonction(): Observable<dataModelsInterfaces.Fonction[]> {
    return this.http.get<dataModelsInterfaces.Fonction[]>(this.baseUrl + "evapi/fonction/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Service
  getListService(): Observable<dataModelsInterfaces.Service[]> {
    return this.http.get<dataModelsInterfaces.Service[]>(this.baseUrl + "evapi/service/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Titre
  getListTitre(): Observable<dataModelsInterfaces.Titre[]> {
    return this.http.get<dataModelsInterfaces.Titre[]>(this.baseUrl + "evapi/titre/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Client
  getListClient(refClient: number, refProduit: number, d: moment.Moment, refEntiteRttForbidden: number, actif: boolean): Observable<dataModelsInterfaces.EntiteList[]> {
    var url = this.baseUrl + "evapi/entite/getlist";
    return this.http.get<dataModelsInterfaces.EntiteList[]>(this.baseUrl + "evapi/entite/getlist", {
      params: new HttpParams()
        .set("refEntiteType", "4"),
      headers: new HttpHeaders()
        .set("actif", actif == false ? "false" : "true")
        .set("refEntite", refClient == null ? "" : refClient.toString())
        .set("refEntiteRttForbidden", refEntiteRttForbidden == null ? "" : refEntiteRttForbidden.toString())
        .set("refProduit", refProduit == null ? "" : refProduit.toString())
        .set("d", d == null ? "" : moment(d).format("YYYY-MM-DD 00:00:00.000")),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Collectivite
  getListCollectivite(refCollectivite: number, refEntiteRtt: number, lienActif: boolean, contratActif: boolean, d: moment.Moment, textFilter: string, actif: boolean): Observable<dataModelsInterfaces.EntiteList[]> {
    return this.http.get<dataModelsInterfaces.EntiteList[]>(this.baseUrl + "evapi/entite/getlist", {
      params: new HttpParams()
        .set("refEntiteType", "1")
        .set("textFilter", textFilter),
      headers: new HttpHeaders()
        .set("refEntite", refCollectivite == null ? "" : refCollectivite.toString())
        .set("actif", actif == false ? "false" : "true")
        .set("refEntiteRtt", refEntiteRtt == null ? "" : refEntiteRtt.toString())
        .set("lienActif", lienActif == true ? "true" : "false")
        .set("contratActif", contratActif == true ? "true" : "false")
        .set("d", d == null ? "" : moment(d).format("YYYY-MM-DD 00:00:00.000")),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing EntiteType
  getListEntiteType(): Observable<dataModelsInterfaces.EntiteType[]> {
    return this.http.get<dataModelsInterfaces.EntiteType[]>(this.baseUrl + "evapi/entitetype/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing MessageType
  getListMessageType(): Observable<dataModelsInterfaces.MessageType[]> {
    return this.http.get<dataModelsInterfaces.MessageType[]>(this.baseUrl + "evapi/messagetype/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing NonConformiteAccordFournisseurType
  getListNonConformiteAccordFournisseurType(actif: boolean, refNonConformiteAccordFournisseurType: number): Observable<dataModelsInterfaces.NonConformiteAccordFournisseurType[]> {
    return this.http.get<dataModelsInterfaces.NonConformiteAccordFournisseurType[]>(this.baseUrl + "evapi/nonconformiteaccordfournisseurtype/getlist", {
      headers: new HttpHeaders()
        .set("refNonConformiteAccordFournisseurType", refNonConformiteAccordFournisseurType ? refNonConformiteAccordFournisseurType.toString() : "")
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing NonConformiteDemandeClientType
  getListNonConformiteDemandeClientType(actif: boolean, refNonConformiteDemandeClientType: number): Observable<dataModelsInterfaces.NonConformiteDemandeClientType[]> {
    return this.http.get<dataModelsInterfaces.NonConformiteDemandeClientType[]>(this.baseUrl + "evapi/nonconformitedemandeclienttype/getlist", {
      headers: new HttpHeaders()
        .set("refNonConformiteDemandeClientType", refNonConformiteDemandeClientType ? refNonConformiteDemandeClientType.toString() : "")
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing NonConformiteFamille
  getListNonConformiteFamille(actif: boolean, refNonConformiteFamilles: string): Observable<dataModelsInterfaces.NonConformiteFamille[]> {
    return this.http.get<dataModelsInterfaces.NonConformiteFamille[]>(this.baseUrl + "evapi/nonconformitefamille/getlist", {
      headers: new HttpHeaders()
        .set("refNonConformiteFamilles", refNonConformiteFamilles ? refNonConformiteFamilles.toString() : "")
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing NonConformiteEtapeType
  getListNonConformiteEtapeType(actif: boolean, refNonConformiteEtapeType: number): Observable<dataModelsInterfaces.NonConformiteEtapeType[]> {
    return this.http.get<dataModelsInterfaces.NonConformiteEtapeType[]>(this.baseUrl + "evapi/nonconformiteEtapeType/getlist", {
      headers: new HttpHeaders()
        .set("refNonConformiteEtapeType", refNonConformiteEtapeType ? refNonConformiteEtapeType.toString() : "")
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing NonConformiteNature
  getListNonConformiteNature(actif: boolean, refNonConformiteNature: number): Observable<dataModelsInterfaces.NonConformiteNature[]> {
    return this.http.get<dataModelsInterfaces.NonConformiteNature[]>(this.baseUrl + "evapi/nonconformitenature/getlist", {
      headers: new HttpHeaders()
        .set("refNonConformiteNature", refNonConformiteNature ? refNonConformiteNature.toString() : "")
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing NonConformiteReponseClientType
  getListNonConformiteReponseClientType(actif: boolean, refNonConformiteReponseClientType: number): Observable<dataModelsInterfaces.NonConformiteReponseClientType[]> {
    return this.http.get<dataModelsInterfaces.NonConformiteReponseClientType[]>(this.baseUrl + "evapi/nonconformitereponseclienttype/getlist", {
      headers: new HttpHeaders()
        .set("refNonConformiteReponseClientType", refNonConformiteReponseClientType ? refNonConformiteReponseClientType.toString() : "")
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing NonConformiteReponseFournisseurType
  getListNonConformiteReponseFournisseurType(actif: boolean, refNonConformiteReponseFournisseurType: number): Observable<dataModelsInterfaces.NonConformiteReponseFournisseurType[]> {
    return this.http.get<dataModelsInterfaces.NonConformiteReponseFournisseurType[]>(this.baseUrl + "evapi/nonconformitereponsefournisseurtype/getlist", {
      headers: new HttpHeaders()
        .set("refNonConformiteReponseFournisseurType", refNonConformiteReponseFournisseurType ? refNonConformiteReponseFournisseurType.toString() : "")
        .set("actif", actif == false ? "false" : "true"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing RepriseType
  getListRepriseType(): Observable<dataModelsInterfaces.RepriseType[]> {
    return this.http.get<dataModelsInterfaces.RepriseType[]>(this.baseUrl + "evapi/reprisetype/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Repreneur
  getListRepreneur(): Observable<dataModelsInterfaces.Repreneur[]> {
    return this.http.get<dataModelsInterfaces.Repreneur[]>(this.baseUrl + "evapi/repreneur/getlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing SAGECategorieAchat
  getListSAGECategorieAchat(): Observable<dataModelsInterfaces.SAGECategorieAchat[]> {
    return this.http.get<dataModelsInterfaces.SAGECategorieAchat[]>(this.baseUrl + "evapi/utils/getlistsagecategorieachatlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing SAGEConditionLivraison
  getListSAGEConditionLivraison(): Observable<dataModelsInterfaces.SAGEConditionLivraison[]> {
    return this.http.get<dataModelsInterfaces.SAGEConditionLivraison[]>(this.baseUrl + "evapi/utils/getlistsageconditionlivraisonlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing SAGEPeriodicite
  getListSAGEPeriodicite(): Observable<dataModelsInterfaces.SAGEPeriodicite[]> {
    return this.http.get<dataModelsInterfaces.SAGEPeriodicite[]>(this.baseUrl + "evapi/utils/getlistsageperiodicitelist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing SAGEModeReglement
  getListSAGEModeReglement(): Observable<dataModelsInterfaces.SAGEModeReglement[]> {
    return this.http.get<dataModelsInterfaces.SAGEModeReglement[]>(this.baseUrl + "evapi/utils/getlistsagemodereglementlist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing SAGECategorieVente
  getListSAGECategorieVente(): Observable<dataModelsInterfaces.SAGECategorieVente[]> {
    return this.http.get<dataModelsInterfaces.SAGECategorieVente[]>(this.baseUrl + "evapi/utils/getlistsagecategorieventelist", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Ticket
  getTickets(filterText: string): Observable<dataModelsInterfaces.Ticket[]> {
    return this.http.get<dataModelsInterfaces.Ticket[]>(this.baseUrl + "evapi/ticket/getall", {
      headers: new HttpHeaders()
        .set("filterText", filterText),
      responseType: "json"
    });
  }
}
