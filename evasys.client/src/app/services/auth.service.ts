/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/12/2018
/// ----------------------------------------------------------------------------------------------------- 
import { EventEmitter, Inject, Injectable, PLATFORM_ID } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map, catchError } from "rxjs/operators";
import { ApplicationUserContext } from "../globals/globals";
import "../globals/utils";
import * as dataModelsInterfaces from "../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../interfaces/appInterfaces";
import { showErrorToUser, showInformationToUser } from "../globals/utils";
import { DataModelService } from "./data-model.service";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { UtilisateurList } from "../interfaces/dataModelsInterfaces";
import { Router } from "@angular/router";

@Injectable()
export class AuthService {
  authKey: string = "auth";
  //clientId: string = "eValorplastWebApp";
  u: dataModelsInterfaces.Utilisateur;
  constructor(private http: HttpClient
    , @Inject("BASE_URL") private baseUrl: string
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private router: Router
    , @Inject(PLATFORM_ID) private platformId: any) {
  }
  // performs the login
  login(login: string, pwd: string, cultureName: string): Observable<appInterfaces.TokenResponse> {
    var url = this.baseUrl + "evapi/token/auth";
    var data = {
      login: login,
      pwd: pwd,
      grantType: "password",
      cultureName: cultureName
    };

    return this.getAuthFromServer(url, data, true);
  }
  // Ask for Profil change
  profil(login: string, pwd: string, cultureName: string): Observable<appInterfaces.TokenResponse> {
    var url = this.baseUrl + "evapi/token/auth";
    var data = {
      login: login,
      pwd: pwd,
      grantType: "profil",
      cultureName: cultureName
    };

    return this.getAuthFromServer(url, data, true);
  }

  // try to refresh token
  refreshToken(): Observable<appInterfaces.TokenResponse> {
    var url = this.baseUrl + "evapi/token/auth";
    var data = {
      // required when signing up with username/password
      login: "",
      pwd: "",
      grantType: "refresh_token",
    };

    return this.getAuthFromServer(url, data, false);
  }

  // retrieve the access & refresh tokens from the server
  getAuthFromServer(url: string, data: any, getConnectedUser: boolean): Observable<appInterfaces.TokenResponse> {
    return this.http.post<appInterfaces.TokenResponse>(url, data)
      .pipe(
        map((res) => {
          if (res.token === "" || res.expiration === 0) {
            // store username and jwt token
            this.logout();
          }
          else {
            // store username and jwt token
            this.setAuth(res);
            //if (getConnectedUser) { this.getUserFromServer(); }
          }
          // return token response
          return res;
        }),
        catchError(error => {
          return new Observable<any>(error);
        })
      );
  }

  // Reset server session
  resetServerSession() {
    var url = this.baseUrl + "evapi/token/logout";
    this.http.post(url, "");
  }

  // performs the logout
  logout() {
    //Delete JWT
    this.setAuth(null);
    //Reset local session data
    this.applicationUserContext.disconnect();
    //Reset server session
    this.resetServerSession();
  }

  // Persist auth into localStorage or removes it if a NULL argument is given
  setAuth(auth: appInterfaces.TokenResponse | null) {
    if (auth) {
      sessionStorage.setItem(
        this.authKey,
        JSON.stringify(auth));
      var i = sessionStorage.getItem(this.authKey);
    }
    else {
      sessionStorage.removeItem(this.authKey);
    }
  }

  // Retrieves the auth JSON object (or NULL if none)
  getAuth(): appInterfaces.TokenResponse | null {
    var i = sessionStorage.getItem(this.authKey);
    if (i) {
      return JSON.parse(i);
    }
    else {
      return null;
    }
  }

  // Returns TRUE if the user is logged in, FALSE otherwise.
  isLoggedIn(): boolean {
    let r: boolean = false;
    r = (sessionStorage.getItem(this.authKey) != null);
    return r;
  }

  // Check if link for password reset is valid
  checkResetPassword(valeur:string): Observable<appInterfaces.ResetPassword> {
    return this.http.post<appInterfaces.ResetPassword>(this.baseUrl + "evapi/utilisateur/checkresetpassword", <appInterfaces.StringBody>{ val: valeur }, {
      responseType: "json"
    });
  }

  // Reset password
  resetPassword(u: dataModelsInterfaces.Utilisateur): Observable<dataModelsInterfaces.Utilisateur> {
    return this.http.post<dataModelsInterfaces.Utilisateur>(this.baseUrl + "evapi/utilisateur/resetpassword", u, {
      responseType: "json"
    });
  }

  // Connect user
  connectUser(profil: UtilisateurList, dialog: MatDialog, askClose : EventEmitter<any>, type: string) {
    //Try to connect new profil
    this.profil(profil.RefUtilisateur.toString(), "", this.applicationUserContext.currentCultureName)
      .subscribe(res => {
        if (res.token != "" && res.expiration != 0) {
          // login successful
          this.applicationUserContext.profilSelected = true;
          //Get connected user
          this.dataModelService.getUserFromServer().subscribe(result => {
            this.applicationUserContext.connectedUtilisateur = result;
            //Set help visibility
            let refParametre: number = 0;
            this.applicationUserContext.visibleHelp = false;
            if (this.applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) { refParametre = 14; }
            if (this.applicationUserContext.connectedUtilisateur.CentreDeTri?.RefEntite) { refParametre = 15; }
            if (this.applicationUserContext.connectedUtilisateur.Transporteur?.RefEntite) { refParametre = 16; }
            if (this.applicationUserContext.connectedUtilisateur.Client?.RefEntite) { refParametre = 17; }
            if (this.applicationUserContext.connectedUtilisateur.Prestataire?.RefEntite) { refParametre = 18; }
            if (refParametre > 0) {
              this.dataModelService.existsBinaryData(refParametre).subscribe(result => {
                if (result == true) {
                  this.applicationUserContext.visibleHelp = true;
                }
              }, error => showErrorToUser(dialog, error, this.applicationUserContext));
            }
            //DRFilter visibility
            this.dataModelService.isDR().subscribe(result => {
              this.applicationUserContext.visibleFilterDR = result;
              //If applicable, set the filter to true
              if (this.applicationUserContext.visibleFilterDR) {
                this.dataModelService.setFilterDR(true).subscribe(result => {
                  this.applicationUserContext.filterDR = result;
                  //Redirect to home
                  if (!this.applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) {
                    //Emit event for dialog closing, or route if not opened in a dialog
                    if (askClose != null) { askClose.emit({ type: type, ref: "home" }); }
                    else { this.router.navigate(["home"]); }
                  }
                  else {
                    this.applicationUserContext.filterCollectivites = [];
                    this.applicationUserContext.filterCollectivites.push({ RefEntite: this.applicationUserContext.connectedUtilisateur.Collectivite.RefEntite } as dataModelsInterfaces.Entite)
                    //Emit event for dialog closing, or route if not opened in a dialog
                    if (askClose != null) { askClose.emit({ type: type, ref: "accueil-collectivite" }); }
                    else { this.router.navigate(["accueil-collectivite"]); }
                  }
                }, error => showErrorToUser(dialog, error, this.applicationUserContext));
              }
              else {
                //Redirect to home, set collectivite if applicable
                if (!this.applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) {
                  //Emit event for dialog closing, or route if not opened in a dialog
                  if (askClose != null) { askClose.emit({ type: type, ref: "home" }); }
                  else { this.router.navigate(["home"]); }
                }
                else {
                  this.applicationUserContext.filterCollectivites = [];
                  this.applicationUserContext.filterCollectivites.push({ RefEntite: this.applicationUserContext.connectedUtilisateur.Collectivite.RefEntite } as dataModelsInterfaces.Entite)
                  //Emit event for dialog closing, or route if not opened in a dialog
                  if (askClose != null) { askClose.emit({ type: type, ref: "accueil-collectivite" }); }
                  else { this.router.navigate(["accueil-collectivite"]); }
                }
              }
            }, error => showErrorToUser(dialog, error, this.applicationUserContext));
          }, error => showErrorToUser(dialog, error, this.applicationUserContext));
        }
        else {
          //Connection error
          //Inform user
          showInformationToUser(dialog, this.applicationUserContext.getCulturedRessourceText(319), this.applicationUserContext.getCulturedRessourceText(1504), this.applicationUserContext);
          //Go back to login
          this.logout();
          //Emit event for dialog closing, or route if not opened in a dialog
          if (askClose != null) { askClose.emit({ type: type, ref: "login" }); }
          else { this.router.navigate(["login"]); }
        }
      }, error => showErrorToUser(dialog, error, this.applicationUserContext));
  }
}
