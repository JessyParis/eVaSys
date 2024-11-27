/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/12/2018
/// ----------------------------------------------------------------------------------------------------- 
import { Injectable, Injector } from "@angular/core";
import { Router } from "@angular/router";
import { HttpHandler, HttpEvent, HttpInterceptor, HttpRequest, HttpResponse, HttpErrorResponse } from "@angular/common/http";
import { AuthService } from "./auth.service";
import { Observable, of, observable, throwError } from "rxjs";
import { tap, catchError, flatMap } from "rxjs/operators";
import { showErrorToUser } from "../globals/utils";
import { ApplicationUserContext } from "../globals/globals";
import { MatDialog } from "@angular/material/dialog";

@Injectable()
export class AuthResponseInterceptor implements HttpInterceptor {

  auth: AuthService;

  constructor(
    private injector: Injector,
    private router: Router,
    private applicationUserContext: ApplicationUserContext
    , public dialog: MatDialog
  ) { }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler): Observable<HttpEvent<any>> {

    this.auth = this.injector.get(AuthService);
    var token = (this.auth.isLoggedIn()) ? this.auth.getAuth()!.token : null;

    if (token) {
      // save current request
      let currentRequest: HttpRequest<any> = request.clone();

      return next.handle(request).pipe(
        tap((event: HttpEvent<any>) => {
          if (event instanceof HttpResponse) {
            // do nothing
          }
        }),
        catchError(error => {
          return this.handleError(error, next, currentRequest)
        }));
    }
    else {
      return next.handle(request);
    }
  }

  handleError(err: any, next: HttpHandler, request: HttpRequest<any>) {
    if (err instanceof HttpErrorResponse) {
      if (err.status === 403) {
        // JWT token might be expired if the error does not come from a new token request
        if (err.url.endsWith("evapi/token/auth")) {
          //Close all existing dialog
          this.dialog.closeAll();
          //Show error
          showErrorToUser(this.dialog, err, this.applicationUserContext);
          //Logout
          this.auth.logout();
          //Route to login
          this.router.navigate(["login"]);
        }
        else {
          // try to get a new one using refresh token
          let previousRequest = request;
          return this.auth.refreshToken().pipe(
            flatMap((refreshed) => {
              var token = (this.auth.isLoggedIn()) ? this.auth.getAuth()!.token : null;
              if (token) {
                previousRequest = previousRequest.clone({
                  setHeaders: { eValorplastSecurity: token }
                });
              }
              return next.handle(previousRequest);
            }));
        }
      }
      if (err.status === 401) {
        //Forbidden
        this.router.navigate(["forbidden"]);
      }
    }
    return throwError(err);
  }
}
