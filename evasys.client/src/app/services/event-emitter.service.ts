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
import { Injectable, EventEmitter } from "@angular/core";
import { Subscription } from "rxjs/internal/Subscription";

@Injectable({
  providedIn: "root"
})
export class EventEmitterService {

  invokeAppScreenRefresh = new EventEmitter();    //Ask for app component refresh
  subsDashboardClick: Subscription; //subscribe to dashboard item click

  constructor() { }

  onDashboardItemClick() {
    this.invokeAppScreenRefresh.emit();
  }
  onRedirectToHome() {
    this.invokeAppScreenRefresh.emit();
  }
  onChangeMenu() {
    this.invokeAppScreenRefresh.emit();
  }
}   
