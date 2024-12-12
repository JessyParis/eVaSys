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

@Injectable({
  providedIn: 'root'
})
export class UpdateService {
  constructor(private swUpdate: SwUpdate
    , public applicationUserContext: ApplicationUserContext
    , private snackBar: MatSnackBar) {
    this.swUpdate.versionUpdates.subscribe((event) => {
      if (event.type === "VERSION_READY") {
        this.promptUser();
      }
    });
  }

  promptUser(): void {
    const snackBarRef = this.snackBar.open(this.applicationUserContext.getCulturedRessourceText(1540)
      , this.applicationUserContext.getCulturedRessourceText(1364), {
      duration: 6000,
    });

    snackBarRef.onAction().subscribe(() => {
      this.swUpdate.activateUpdate().then(() => document.location.reload());
    });
  }
}
