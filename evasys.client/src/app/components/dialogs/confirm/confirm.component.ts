import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import { ApplicationUserContext } from "../../../globals/globals";

@Component({
    selector: "confirm",
    templateUrl: "./confirm.component.html",
    standalone: false
})

export class ConfirmComponent {
    constructor(dialogRef: MatDialogRef<ConfirmComponent>
        , @Inject(MAT_DIALOG_DATA) public data: any
        , public applicationUserContext: ApplicationUserContext
    ) { }
}
