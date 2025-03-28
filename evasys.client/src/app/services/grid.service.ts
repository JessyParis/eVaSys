import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";
import { removeAccents } from "../globals/utils";

const url = "evapi/grid/getitems";
//-----------------------------------------------------------------------------------
//Data service for standard data grid
@Injectable()
export class GridService {
  //-----------------------------------------------------------------------------------
  //Cobstructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string) { }
  //-----------------------------------------------------------------------------------
  //Get full response (data + headers) to get total row number for paginator
  getItems(module = "", menu = "", action = "", sortExpression = "", sortOrder = "asc", pageNumber = 0, pageSize = 3
    , filterBegin = "", filterEnd = "", filterText = ""
    , filterClients = "", filterTransporteurs = "", filterPrestataires = "", filterCentreDeTris = "", filterIndustriels = "", filterDptDeparts = "", filterDptArrivees = ""
    , filterVilleDeparts = "", filterVilleArrivees = "", filterPayss = "", filterProcesss = "", filterRegionReportings = "", filterProduits = "", filterComposants = "", filterCamionTypes = ""
    , filterMonths = "", filterYears = "", filterNonConformiteEtapeTypes = "", filterNonConformiteNatures = ""
    , filterIFFournisseur = false, filterIFClient = false, filterIFFournisseurRetourLot = false, filterIFFournisseurFacture = false, filterIFFournisseurAttenteBonCommande = false
    , filterIFFournisseurTransmissionFacturation = false, filterNonConformitesATraiter = false, filterNonConformitesATransmettre = false, filterNonConformitesPlanActionAValider = false
    , filterNonConformitesPlanActionNR = false
    , filterEnvCommandeFournisseurStatuts = "", filterDChargementModif = false, filterCommandesFournisseurNonChargees = false, filterValideDPrevues = false, filterDatesTransporteur = false
    , filterAnomalieCommandeFournisseur = false, filterControle = false, filterDRs = "", filterCommandesFournisseurAttribuees = false
    , augmentation = "", selectedItem = "", moisDechargementPrevuItem = ""
    , yearFrom = "", monthFrom = "", yearTo = "", monthTo = "", filterEcoOrganismes = "", filterEntiteTypes = "", filterMessageTypes = "", filterApplicationProduitOrigines = "", filterActif = false, filterNonRepartissable = null
    , filterCollecte = "", filterDonneeEntiteSage = false, filterUtilisateurMaitres = "", filterRepartitionAFinaliser = false
    ): Observable<HttpResponse<any[]>> {
    return this.http.get<any[]>(this.baseUrl + url,
      {
        observe: "response",
        headers: new HttpHeaders()
          .set("action", action)
          .set("filterBegin", filterBegin)
          .set("filterEnd", filterEnd)
          .set("filterClients", filterClients)
          .set("filterTransporteurs", filterTransporteurs)
          .set("filterPrestataires", filterPrestataires)
          .set("filterCentreDeTris", filterCentreDeTris)
          .set("filterIndustriels", filterIndustriels)
          .set("filterAnomalieCommandeFournisseur", filterAnomalieCommandeFournisseur ? "true" : "false")
          .set("filterDatesTransporteur", filterDatesTransporteur ? "true" : "false")
          .set("filterDChargementModif", filterDChargementModif ? "true" : "false")
          .set("filterDptDeparts", filterDptDeparts)
          .set("filterDptArrivees", filterDptArrivees)
          .set("filterVilleDeparts", removeAccents(filterVilleDeparts))
          .set("filterVilleArrivees", removeAccents(filterVilleArrivees))
          .set("filterPayss", filterPayss)
          .set("filterProcesss", filterProcesss)
          .set("filterRegionReportings", filterRegionReportings)
          .set("filterProduits", filterProduits)
          .set("filterComposants", filterComposants)
          .set("filterCamionTypes", filterCamionTypes)
          .set("filterMonths", filterMonths)
          .set("filterYears", filterYears)
          .set("filterEnvCommandeFournisseurStatuts", filterEnvCommandeFournisseurStatuts)
          .set("filterCommandesFournisseurNonChargees", filterCommandesFournisseurNonChargees ? "true" : "false")
          .set("filterValideDPrevues", filterValideDPrevues ? "true" : "false")
          .set("augmentation", augmentation)
          .set("exportTransport", "false")
          .set("selectedItem", selectedItem)
          .set("moisDechargementPrevuItem", moisDechargementPrevuItem)
          .set("yearFrom", yearFrom)
          .set("monthFrom", monthFrom)
          .set("yearTo", yearTo)
          .set("monthTo", monthTo)
          .set("filterEntiteTypes", filterEntiteTypes)
          .set("filterDonneeEntiteSage", filterDonneeEntiteSage ? "true" : "false")
          .set("filterEcoOrganismes", filterEcoOrganismes)
          .set("filterMessageTypes", filterMessageTypes)
          .set("filterApplicationProduitOrigines", filterApplicationProduitOrigines)
          .set("filterControle", filterControle ? "true" : "false")
          .set("filterDRs", filterDRs)
          .set("filterUtilisateurMaitres", filterUtilisateurMaitres)
          .set("filterCommandesFournisseurAttribuees", filterCommandesFournisseurAttribuees ? "true" : "false")
          .set("filterNonConformiteEtapeTypes", filterNonConformiteEtapeTypes)
          .set("filterNonConformiteNatures", filterNonConformiteNatures)
          .set("filterIFFournisseur", filterIFFournisseur ? "true" : "false")
          .set("filterIFClient", filterIFClient ? "true" : "false")
          .set("filterIFFournisseurRetourLot", filterIFFournisseurRetourLot ? "true" : "false")
          .set("filterIFFournisseurFacture", filterIFFournisseurFacture ? "true" : "false")
          .set("filterIFFournisseurAttenteBonCommande", filterIFFournisseurAttenteBonCommande ? "true" : "false")
          .set("filterIFFournisseurTransmissionFacturation", filterIFFournisseurTransmissionFacturation ? "true" : "false")
          .set("filterNonConformitesATraiter", filterNonConformitesATraiter ? "true" : "false")
          .set("filterNonConformitesATransmettre", filterNonConformitesATransmettre ? "true" : "false")
          .set("filterNonConformitesPlanActionAValider", filterNonConformitesPlanActionAValider ? "true" : "false")
          .set("filterNonConformitesPlanActionNR", filterNonConformitesPlanActionNR ? "true" : "false")
          .set("filterActif", filterActif ? "true" : "false")
          .set("filterNonRepartissable", filterNonRepartissable == null ? "" : (filterNonRepartissable == false ? "false" : "true"))
          .set("filterCollecte", filterCollecte)
          .set("filterRepartitionAFinaliser", filterRepartitionAFinaliser ? "true" : "false")
        ,
        params: new HttpParams()
          .set("filterText", removeAccents(filterText))
          .set("module", module)
          .set("menu", menu)
          .set("sortExpression", sortExpression)
          .set("sortOrder", sortOrder)
          .set("pageNumber", pageNumber.toString())
          .set("pageSize", pageSize.toString())
        ,
        responseType: "json"
      });
  }
  //-----------------------------------------------------------------------------------
  //Get the Excel file of transports
  exportTransport(module = "", menu = "", sortExpression = "", sortOrder = "asc", pageNumber = 0, pageSize = 3
    , filterBegin = "", filterEnd = "", filterText = ""
    , filterTransporteurs = "", filterCentreDeTris = "", filterDptDeparts = "", filterDptArrivees = "", filterVilleDeparts = "", filterVilleArrivees = "", filterPayss = "", filterProduits = "", filterCamionTypes = "", filterMonths = ""
    , augmentation = "0"): any {
    return this.http.get(this.baseUrl + url, {
      headers: new HttpHeaders()
        .set("filterEnd", filterEnd)
        .set("filterTransporteurs", filterTransporteurs)
        .set("filterCentreDeTris", filterCentreDeTris)
        .set("filterDptDeparts", filterDptDeparts)
        .set("filterDptArrivees", filterDptArrivees)
        .set("filterVilleDeparts", filterVilleDeparts)
        .set("filterVilleArrivees", filterVilleArrivees)
        .set("filterPayss", filterPayss)
        .set("filterProduits", filterProduits)
        .set("filterCamionTypes", filterCamionTypes)
        .set("filterMonths", filterMonths)
        .set("augmentation", augmentation)
        .set("exportTransport", "true"),
      params: new HttpParams()
        .set("filterText", filterText)
        .set("module", module)
        .set("menu", menu)
        .set("sortExpression", sortExpression)
        .set("sortOrder", sortOrder)
        .set("pageNumber", pageNumber.toString())
        .set("pageSize", pageSize.toString()),
      responseType: "blob"
    });
  }
}


