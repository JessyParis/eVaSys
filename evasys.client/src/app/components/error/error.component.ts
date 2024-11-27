import { Component } from "@angular/core";
import { ApplicationUserContext } from "../../globals/globals";

@Component({
    selector: "error",
    templateUrl: "./error.component.html",
    standalone: false
})

export class ErrorComponent {
    constructor(public applicationUserContext: ApplicationUserContext) {}
}
