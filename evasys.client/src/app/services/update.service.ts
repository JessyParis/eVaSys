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

@Injectable({
  providedIn: 'root'
})
export class UpdateService {
  constructor(private swUpdate: SwUpdate, private snackBar: MatSnackBar) {
    this.swUpdate.versionUpdates.subscribe((event) => {
      if (event.type === "VERSION_READY") {
        this.promptUser();
      }
    });
  }

  promptUser(): void {
    const snackBarRef = this.snackBar.open('New version available', 'Reload', {
      duration: 6000,
    });

    snackBarRef.onAction().subscribe(() => {
      this.swUpdate.activateUpdate().then(() => document.location.reload());
    });
  }
}
