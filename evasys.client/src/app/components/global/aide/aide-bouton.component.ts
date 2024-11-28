import { Component, Output, EventEmitter } from "@angular/core";
import { ApplicationUserContext } from "../../../globals/globals";
import { fadeInOnEnterAnimation } from "angular-animations";

@Component({
  selector: "aide-bouton",
  templateUrl: "./aide-bouton.component.html",
  animations: [
    fadeInOnEnterAnimation(),
  ],
  standalone: false
})
//-----------------------------------------------------------------------------------
//Component class
export class AideBoutonComponent {
  @Output() clicked = new EventEmitter<any>();

  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(public applicationUserContext: ApplicationUserContext) { }
  //-----------------------------------------------------------------------------------
  //Click
  onClick() {
    //Emit event for dialog closing
    this.clicked.emit(true);
  }
}




