import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { ApplicationUserContext } from "../../../globals/globals";

@Component({
    selector: "information",
    templateUrl: "./information.component.html",
    styleUrls: ["./information.component.scss"],
    standalone: false
})

export class InformationComponent {
  htmlMessage: string = "";
  constructor(dialogRef: MatDialogRef<InformationComponent>
    , @Inject(MAT_DIALOG_DATA) public data: any
    , public applicationUserContext: ApplicationUserContext
  ) {
  }
}
