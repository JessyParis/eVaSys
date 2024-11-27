/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 22/01/2019
/// ----------------------------------------------------------------------------------------------------- 
///
import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable, of } from "rxjs";
import * as dataModelsInterfaces from "../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../interfaces/appInterfaces";
import * as appClasses from "../classes/appClasses";
import { NonConformiteEtapeType } from '../interfaces/dataModelsInterfaces';
import { removeAccents } from "../globals/utils";
import { ApplicationUserContext } from "../globals/globals";
import { DataModelService } from "./data-model.service";

//-----------------------------------------------------------------------------------
//Utils services
@Injectable()
export class UtilsService {
  //-----------------------------------------------------------------------------------
  //Cobstructor
  constructor(private http: HttpClient
    , @Inject("BASE_URL") private baseUrl: string  ) { }
  //-----------------------------------------------------------------------------------
  //Set culture
  setCulture(cultureName: string): Observable<any> {
    return this.http.get<any>(this.baseUrl + "evapi/utils/setculture", {
      headers: new HttpHeaders()
        .set("cultureName", cultureName),
    });
  }
  //-----------------------------------------------------------------------------------
  //Get all parametres
  getParametres(): Observable<dataModelsInterfaces.Parametre[]> {
    return this.http.get<dataModelsInterfaces.Parametre[]>(this.baseUrl + "evapi/utils/getappparametres", {
      headers: new HttpHeaders()
    });
  }
  //-----------------------------------------------------------------------------------
  //Get specific Aide (from local store or from server if not present in local store)
  getAide(componentName: string, applicationUserContext: ApplicationUserContext
    , dataModelService: DataModelService): Observable<dataModelsInterfaces.Aide> {
    let aide: dataModelsInterfaces.Aide;
    aide = applicationUserContext.aides.find(e => e.Composant == componentName);
    if (aide && aide?.ValeurHTML) {
      return of(aide);
    }
    else {
      return dataModelService.getAideByComposant(componentName);
    }
  }
  //-----------------------------------------------------------------------------------
  //Get all cultured ressources
  getCulturedRessources(cultureName: string): Observable<appInterfaces.CulturedRessource[]> {
    return this.http.get<appInterfaces.CulturedRessource[]>(this.baseUrl + "evapi/utils/getculturedressources", {
      headers: new HttpHeaders()
        .set("cultureName", cultureName),
    });
  }
  //-----------------------------------------------------------------------------------
  //Get all app resources
  getResources(): Observable<appInterfaces.Resource[]> {
    return this.http.get<appInterfaces.Resource[]>(this.baseUrl + "evapi/utils/getappresources", {
      headers: new HttpHeaders()
    });
  }
  //-----------------------------------------------------------------------------------
  //Get all modules
  getEnvModules(cultureName: string): Observable<appClasses.EnvModule[]> {
    return this.http.get<appClasses.EnvModule[]>(this.baseUrl + "evapi/utils/getenvmodules", {
      headers: new HttpHeaders()
        .set("cultureName", cultureName),
    });
  }
  //-----------------------------------------------------------------------------------
  //Get all menus
  getEnvMenus(cultureName: string): Observable<appClasses.EnvMenu[]> {
    return this.http.get<appClasses.EnvMenu[]>(this.baseUrl + "evapi/utils/getenvmenus", {
      headers: new HttpHeaders()
        .set("cultureName", cultureName),
    });
  }
  //-----------------------------------------------------------------------------------
  //Get all components
  getEnvComponents(cultureName: string): Observable<appClasses.EnvComponent[]> {
    return this.http.get<appClasses.EnvComponent[]>(this.baseUrl + "evapi/utils/getenvcomponents", {
      headers: new HttpHeaders()
        .set("cultureName", cultureName),
    });
  }
  //-----------------------------------------------------------------------------------
  //Get all data columns
  getEnvDataColumns(cultureName: string): Observable<appClasses.EnvDataColumn[]> {
    return this.http.get<appClasses.EnvDataColumn[]>(this.baseUrl + "evapi/utils/getenvdatacolumns", {
      headers: new HttpHeaders()
        .set("cultureName", cultureName),
    });
  }
  //-----------------------------------------------------------------------------------
  //Get environment type
  getEnvironment(): Observable<string> {
    return this.http.get<string>(this.baseUrl + "evapi/utils/getenvironment");
  }
  //-----------------------------------------------------------------------------------
  //Get all CommandeFournisseur statuts
  getEnvCommandeFournisseurStatuts(cultureName: string): Observable<appClasses.EnvCommandeFournisseurStatut[]> {
    return this.http.get<appClasses.EnvCommandeFournisseurStatut[]>(this.baseUrl + "evapi/utils/getenvcommandefournisseurstatuts", {
      headers: new HttpHeaders()
        .set("cultureName", cultureName),
    });
  }
  //-----------------------------------------------------------------------------------
  //Get all NonConformiteEtapeType
  getNonConformiteEtapeType(cultureName: string): Observable<NonConformiteEtapeType[]> {
    return this.http.get<NonConformiteEtapeType[]>(this.baseUrl + "evapi/utils/getnonconformiteetapetype", {
      headers: new HttpHeaders()
        .set("cultureName", cultureName),
    });
  }
  //-----------------------------------------------------------------------------------
  //Unlock all data for one user
  unlockAllData(): Observable<any> {
    return this.http.get<any>(this.baseUrl + "evapi/utils/unlockalldata");
  }
  //-----------------------------------------------------------------------------------
  //Get all habilitation values of one type
  getHabilitationValues(habilitationType: string): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + "evapi/utils/gethabilitationvalues", {
      headers: new HttpHeaders()
        .set("habilitationType", habilitationType),
    });
  }
  //-----------------------------------------------------------------------------------
  //Get search result
  //Decompose single accented unicode character into 2 character+accent. Then, remove accent character
  getSearchResult(searchText: string, elementType: string, skip:number): Observable<number[]> {
    return this.http.get<number[]>(this.baseUrl + "evapi/search/searchresults", {
      headers: new HttpHeaders()
        .set("skip", skip.toString())
        .set("searchText", removeAccents(searchText))
        .set("elementType", elementType),
    });
  }
  //-----------------------------------------------------------------------------------
  //Get search details
  getSearchResultDetails(elementRef: number, elementType: string, searchText: string): Observable<appInterfaces.SearchResultDetails> {
    return this.http.get<appInterfaces.SearchResultDetails>(this.baseUrl + "evapi/search/searchresultdetails", {
      headers: new HttpHeaders()
        .set("elementRef", !elementRef ? "" : elementRef.toString())
        .set("elementType", elementType)
        .set("searchText", removeAccents(searchText))
    });
  }
}
