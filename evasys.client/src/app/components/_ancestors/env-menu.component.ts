import { Component, Input} from "@angular/core";
import { ApplicationUserContext } from "../../globals/globals";
import { EnvMenu } from "../../classes/appClasses";

@Component({
    selector: "env-menu",
    templateUrl: "./env-menu.component.html",
    standalone: false
})
export class EnvMenuComponent {
  constructor(public applicationUserContext: ApplicationUserContext) {
  }
  @Input() menu: EnvMenu;
  style: string = "ev-menu";
  onNew(menuName: string) {
  }
  onMenu(menuName: string) {
  }
}
