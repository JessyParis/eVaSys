import { Component } from "@angular/core";
import { ApplicationUserContext } from "../../globals/globals";

@Component({
    selector: "forbidden",
    templateUrl: "./forbidden.component.html",
    standalone: false
})

export class ForbiddenComponent {
    constructor(public applicationUserContext: ApplicationUserContext) {}
}
