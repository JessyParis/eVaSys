/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 04/04/2019
/// ----------------------------------------------------------------------------------------------------- 
///
import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { SnackbarMsg } from "../interfaces/appInterfaces";

@Injectable({
  providedIn: "root"
})
export class SnackBarQueueService {
  private processingMessage = false;
  private messageNumber = 0;
  private messageQueue: SnackbarMsg[] = [];

  constructor(private snackBar: MatSnackBar) { }


  private displaySnackbar(): void {
    const nextMessage = this.getNextMessage();

    if (!nextMessage) {
      this.processingMessage = false;
      return;
    }

    this.processingMessage = true;

    this.snackBar.open(nextMessage.text, undefined, { duration: nextMessage.duration })
      .afterDismissed()
      .subscribe(() => {
        this.displaySnackbar();
      });
  }

  private getNextMessage(): SnackbarMsg | undefined {
    return this.messageQueue.length ? this.messageQueue.shift() : undefined;
  }

  addMessage(msg: SnackbarMsg): void {
    this.messageQueue.push(msg);
    if (!this.processingMessage) {
      this.displaySnackbar();
    }
  }
}  
