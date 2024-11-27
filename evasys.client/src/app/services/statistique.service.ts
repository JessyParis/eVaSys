import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";
import { removeAccents } from "../globals/utils";

const url = "evapi/statistique/getstatistique";
//-----------------------------------------------------------------------------------
//Data service for module statistiques
@Injectable()
export class StatistiqueService {
  //-----------------------------------------------------------------------------------
  //Cobstructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string) { }
  //-----------------------------------------------------------------------------------
  //Get full response (data + headers) to get total row number for paginator
  getStatistique(module = "", menu = "", pageNumber = 0, pageSize = 25
    , filterBegin = "", filterEnd = "", filterActif = false
    , filterCentreDeTris = "", filterClients = "", filterCollectivites = "", filterDptDeparts = "", filterDptArrivees = "", filterPayss = ""
    , filterProcesss = "", filterProduits = "", filterTransporteurs = ""
    , filterVilleDeparts = "", filterAdresseDestinations = "", filterVilleArrivees = ""
    , filterDRs = "", filterMonths = "", filterQuarters = "", filterYears = "", filterNonConformiteEtapeTypes = ""
    , filterNonConformiteNatures = "", filterCamionTypes = "", filterProduitGroupeReportings = ""
    , filterMoins10Euros = false, filterCollecte = "", filterContrat = "", filterEE = null, filterEcoOrganismes = "", filterEntiteTypes = "", filterEmailType = "", filterActionTypes = ""
    , filterAdresseTypes = "", filterContactAdresseProcesss = "", filterFonctions = "", filterServices = "", filterContactSelectedColumns = ""
    , filterFirstLogin = false, filterDayWeekMonth = "Month", filterEntites = "", filterUtilisateurs = "", filterText = "", statType = ""
  ): Observable<HttpResponse<any[]>> {
    return this.http.get<any[]>(this.baseUrl + url,
      {
        observe: "response",
        headers: new HttpHeaders()
          .set("filterBegin", filterBegin)
          .set("filterEnd", filterEnd)
          .set("filterActif", filterActif ? "true" : "false")
          .set("filterCentreDeTris", filterCentreDeTris)
          .set("filterClients", filterClients)
          .set("filterCollectivites", filterCollectivites)
          .set("filterDptDeparts", filterDptDeparts)
          .set("filterDptArrivees", filterDptArrivees)
          .set("filterDRs", filterDRs)
          .set("filterEcoOrganismes", filterEcoOrganismes)
          .set("filterEntiteTypes", filterEntiteTypes)
          .set("filterPayss", filterPayss)
          .set("filterProcesss", filterProcesss)
          .set("filterProduits", filterProduits)
          .set("filterProduitGroupeReportings", filterProduitGroupeReportings)
          .set("filterTransporteurs", filterTransporteurs)
          .set("filterVilleDeparts", removeAccents(filterVilleDeparts))
          .set("filterAdresseDestinations", filterAdresseDestinations)
          .set("filterVilleArrivees", removeAccents(filterVilleArrivees))
          .set("exportType", "")
          .set("filterMonths", filterMonths)
          .set("filterQuarters", filterQuarters)
          .set("filterYears", filterYears)
          .set("filterNonConformiteEtapeTypes", filterNonConformiteEtapeTypes)
          .set("filterNonConformiteNatures", filterNonConformiteNatures)
          .set("filterCamionTypes", filterCamionTypes)
          .set("filterMoins10Euros", filterMoins10Euros ? "true" : "false")
          .set("filterCollecte", filterCollecte)
          .set("filterContrat", filterContrat)
          .set("filterEE", (filterEE == null ? "" : filterEE.toString()))
          .set("filterEmailType", filterEmailType)
          .set("filterActionTypes", filterActionTypes)
          .set("filterAdresseTypes", filterAdresseTypes)
          .set("filterContactAdresseProcesss", filterContactAdresseProcesss)
          .set("filterFonctions", filterFonctions)
          .set("filterServices", filterServices)
          .set("filterContactSelectedColumns", filterContactSelectedColumns)
          .set("filterFirstLogin", filterFirstLogin ? "true" : "false")
          .set("filterDayWeekMonth", filterDayWeekMonth)
          .set("filterEntites", filterEntites)
          .set("filterUtilisateurs", filterUtilisateurs),
        params: new HttpParams()
          .set("filterText", removeAccents(filterText))
          .set("module", module)
          .set("menu", menu)
          .set("statType", statType)
          .set("pageNumber", pageNumber.toString())
          .set("pageSize", pageSize.toString()),
        responseType: "json"
      });
  }
  //-----------------------------------------------------------------------------------
  //Get the Excel file 
  exportExcel(module = "", menu = "", pageNumber = 0, pageSize = 25
    , filterBegin = "", filterEnd = "", filterActif = false
    , filterCentreDeTris = "", filterClients = "", filterCollectivites = "", filterDptDeparts = "", filterDptArrivees = "", filterPayss = ""
    , filterProcesss = "", filterProduits = "", filterTransporteurs = ""
    , filterVilleDeparts = "", filterAdresseDestinations = "", filterVilleArrivees = ""
    , filterDRs = "", filterMonths = "", filterQuarters = "", filterYears = "", filterNonConformiteEtapeTypes = ""
    , filterNonConformiteNatures = "", filterCamionTypes = "", filterProduitGroupeReportings = ""
    , filterMoins10Euros = false, filterCollecte = "", filterContrat = "", filterEE = null, filterEcoOrganismes = "", filterEntiteTypes = "", filterEmailType = "", filterActionTypes = ""
    , filterAdresseTypes = "", filterContactAdresseProcesss = "", filterFonctions = "", filterServices = "", filterContactSelectedColumns = ""
    , filterFirstLogin = false, filterDayWeekMonth = "Month", filterEntites = "", filterUtilisateurs = "", filterText = "", statType = ""
  ): any {
    return this.http.get(this.baseUrl + url, {
      headers: new HttpHeaders()
        .set("filterBegin", filterBegin)
        .set("filterEnd", filterEnd)
        .set("filterActif", filterActif ? "true" : "false")
        .set("filterCentreDeTris", filterCentreDeTris)
        .set("filterClients", filterClients)
        .set("filterCollectivites", filterCollectivites)
        .set("filterDptDeparts", filterDptDeparts)
        .set("filterDptArrivees", filterDptArrivees)
        .set("filterDRs", filterDRs)
        .set("filterEcoOrganismes", filterEcoOrganismes)
        .set("filterEntiteTypes", filterEntiteTypes)
        .set("filterPayss", filterPayss)
        .set("filterProcesss", filterProcesss)
        .set("filterProduits", filterProduits)
        .set("filterProduitGroupeReportings", filterProduitGroupeReportings)
        .set("filterTransporteurs", filterTransporteurs)
        .set("filterVilleDeparts", removeAccents(filterVilleDeparts))
        .set("filterAdresseDestinations", filterAdresseDestinations)
        .set("filterVilleArrivees", removeAccents(filterVilleArrivees))
        .set("exportType", "Excel")
        .set("filterMonths", filterMonths)
        .set("filterQuarters", filterQuarters)
        .set("filterYears", filterYears)
        .set("filterNonConformiteEtapeTypes", filterNonConformiteEtapeTypes)
        .set("filterNonConformiteNatures", filterNonConformiteNatures)
        .set("filterCamionTypes", filterCamionTypes)
        .set("filterMoins10Euros", filterMoins10Euros ? "true" : "false")
        .set("filterCollecte", filterCollecte)
        .set("filterContrat", filterContrat)
        .set("filterEE", (filterEE == null ? "" : filterEE.toString()))
        .set("filterEmailType", filterEmailType)
        .set("filterActionTypes", filterActionTypes)
        .set("filterAdresseTypes", filterAdresseTypes)
        .set("filterContactAdresseProcesss", filterContactAdresseProcesss)
        .set("filterFonctions", filterFonctions)
        .set("filterServices", filterServices)
        .set("filterContactSelectedColumns", filterContactSelectedColumns)
        .set("filterFirstLogin", filterFirstLogin ? "true" : "false")
        .set("filterDayWeekMonth", filterDayWeekMonth)
        .set("filterEntites", filterEntites)
        .set("filterUtilisateurs", filterUtilisateurs),
      params: new HttpParams()
        .set("filterText", removeAccents(filterText))
        .set("module", module)
        .set("menu", menu)
        .set("statType", statType)
        .set("pageNumber", pageNumber.toString())
        .set("pageSize", pageSize.toString()),
      responseType: "blob"
    });
  }
}


