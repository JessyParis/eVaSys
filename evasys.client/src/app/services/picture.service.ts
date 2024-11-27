/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 16/10/2019
/// ----------------------------------------------------------------------------------------------------- 
///
import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";
import moment from "moment";
import * as dataModelsInterfaces from "../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../interfaces/appInterfaces";

//-----------------------------------------------------------------------------------
//Picture services
@Injectable()
export class PictureService {
    //-----------------------------------------------------------------------------------
    //Cobstructor
    constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string) { }
  //-----------------------------------------------------------------------------------
  //Get existing CommandeFournisseurFichier pictures
  getCommandeFournisseurFichierPictures(refCommandeFournisseur: number): Observable<dataModelsInterfaces.CommandeFournisseurFichier[]> {
    return this.http.get<dataModelsInterfaces.CommandeFournisseurFichier[]>(this.baseUrl + "evapi/commandefournisseur/getpictures", {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", refCommandeFournisseur == null ? "" : refCommandeFournisseur.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing NonConformite pictures
  getNonConformitePictures(refNonConformite: number, refNonConformiteFichierType: number): Observable<dataModelsInterfaces.NonConformiterFichier[]> {
    return this.http.get<dataModelsInterfaces.NonConformiterFichier[]>(this.baseUrl + "evapi/nonconformite/getpictures", {
      headers: new HttpHeaders()
        .set("refNonConformite", refNonConformite == null ? "" : refNonConformite.toString())
        .set("refNonConformiteFichierType", refNonConformiteFichierType == null ? "" : refNonConformiteFichierType.toString()),
      responseType: "json"
    });
  }
}
