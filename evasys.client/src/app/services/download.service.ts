/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 15/01/2018
/// ----------------------------------------------------------------------------------------------------- 
import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";

const url = "evapi/download/getfile";

@Injectable()
//-----------------------------------------------------------------------------------
//Download service for file download
export class DownloadService {
  //-----------------------------------------------------------------------------------
  //Cobstructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string) { }

  public download(fileType: string, fileRef: string, fileFormat: string, containerRef: number, docType: string): any {
    //Return the result of the import
    return this.http.get(this.baseUrl + url, {
      headers: new HttpHeaders()
        .set("fileType", fileType == null ? "" : fileType)
        .set("fileRef", fileRef == null ? "-1" : fileRef)
        .set("fileFormat", fileFormat == null ? "" : fileFormat)
        .set("containerRef", containerRef == null ? "0" : containerRef.toString())
        .set("docType", docType == null ? "" : docType),
      observe: "response",
      responseType: "blob"
    });
  }
}

