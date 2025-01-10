/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/12/2018
/// ----------------------------------------------------------------------------------------------------- 
import { Injectable } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApplicationUserContext } from "../globals/globals"
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
//-----------------------------------------------------------------------------------
//Service worker automatic update
export class UpdateService {
  constructor(private swUpdate: SwUpdate
    , public applicationUserContext: ApplicationUserContext
    , private snackBar: MatSnackBar) {
  //  console.log("UpdateService constructor");
  //  console.log("Production : " + environment.production.toString())
  //  //If new version available
  //  this.swUpdate.versionUpdates.subscribe((event) => {
  //    if (event.type === "VERSION_READY") {
  //      console.log("New version available");
  //      //Prompt user
  //      const snackBarRef = this.snackBar.open(this.applicationUserContext.getCulturedRessourceText(1540)
  //        , undefined, {
  //        duration: 6000,
  //      });
  //      //Update
  //      this.swUpdate.activateUpdate().then(() => document.location.reload());
  //    }
  //  });
  }
}
