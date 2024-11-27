import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";

const url = "evapi/gridsimple/getitems";
//-----------------------------------------------------------------------------------
//Data service for simple data grid
@Injectable()
export class GridSimpleService {
  //-----------------------------------------------------------------------------------
  //Cobstructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string) { }
  //-----------------------------------------------------------------------------------
  //Get full response (data + headers) to get total row number for paginator
  getItems(type = "", sortExpression = "", sortOrder = "asc", pageNumber = 0, pageSize = 3
    , filterText = "", filterYear = "", filterD = "", filterUtilisateurType = "", exclude = ""
    , selectedItem = ""): Observable<HttpResponse<any[]>> {
    return this.http.get<any[]>(this.baseUrl + url,
      {
        observe: "response",
        headers: new HttpHeaders()
          .set("filterText", filterText)
          .set("filterUtilisateurType", filterUtilisateurType)
          .set("filterYear", filterYear)
          .set("filterD", filterD)
          .set("exclude", exclude)
          .set("selectedItem", selectedItem),
        params: new HttpParams()
          .set("type", type)
          .set("sortExpression", sortExpression)
          .set("sortOrder", sortOrder)
          .set("pageNumber", pageNumber.toString())
          .set("pageSize", pageSize.toString()),
        responseType: "json"
      });
  }
}


