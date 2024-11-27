import { Component, Inject, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import { ApplicationUserContext } from "../../../globals/globals";

@Component({
    selector: "component-relative",
    templateUrl: "./component-relative.component.html",
    standalone: false
})

export class ComponentRelativeComponent {
    constructor(public dialogRef: MatDialogRef<ComponentRelativeComponent>
        , @Inject(MAT_DIALOG_DATA) public data: any
        , public applicationUserContext: ApplicationUserContext
    ) {
    }
    //-----------------------------------------------------------------------------------
    //Init
    ngOnInit() {
    }
    //Get child ask for close
    closeAsked(e: any): void {
        this.dialogRef.close({ type: this.data.type, ref: e.ref });
    }
}
