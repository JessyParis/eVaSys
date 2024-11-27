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
import { HttpHandler, HttpEvent, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { AuthService } from "./auth.service";
import { Observable } from "rxjs";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private injector: Injector) { }

    intercept(
        request: HttpRequest<any>,
        next: HttpHandler): Observable<HttpEvent<any>> {

        var auth = this.injector.get(AuthService);
        var token = (auth.isLoggedIn()) ? auth.getAuth()!.token : null;
        if (token) {
            request = request.clone({
                setHeaders: {
                eValorplastSecurity: token,
                "Cache-Control": "no-cache",
                "Pragma": "no-cache",
                "Expires": "Sat, 01 Jan 2000 00:00:00 GMT"
                }
            });
        }
        return next.handle(request);
    }
}
