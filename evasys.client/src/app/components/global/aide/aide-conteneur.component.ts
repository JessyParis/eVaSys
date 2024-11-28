import { Component, Input, Output, EventEmitter } from "@angular/core";
import { fadeInOnEnterAnimation } from "angular-animations";
import { SafeHtml } from "@angular/platform-browser";
import { ApplicationUserContext } from "../../../globals/globals";

@Component({
  selector: "aide-conteneur",
  templateUrl: "./aide-conteneur.component.html",
  animations: [
    fadeInOnEnterAnimation(),
  ],
  standalone: false
})
//-----------------------------------------------------------------------------------
//Component class
export class AideConteneurComponent {
  @Input() safeValeurHTML: SafeHtml;
  @Output() askClose = new EventEmitter<any>();

  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(public applicationUserContext: ApplicationUserContext) { }
  //-----------------------------------------------------------------------------------
  //Ask for close
  onClose() {
    //Emit event for dialog closing
    this.askClose.emit(true);
  }
}




