import { Component, HostListener, Inject, OnInit, ViewEncapsulation } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from "@angular/material/dialog";
import { ApplicationUserContext } from "../../../globals/globals";
import { DataModelService } from "../../../services/data-model.service";
import { showErrorToUser } from "../../../globals/utils";

@Component({
    selector: "transport-concurrence",
    templateUrl: "./transport-concurrence.component.html",
    encapsulation: ViewEncapsulation.None,
    standalone: false
})

export class TransportConcurrenceComponent implements OnInit {
  competitors: any[] = [];
  constructor(public dialogRef: MatDialogRef<TransportConcurrenceComponent>
    , @Inject(MAT_DIALOG_DATA) public data: any
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , public dialog: MatDialog
  ) {
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    this.competitors = [];
    if (this.data.ref) {
      //Get transports
      this.dataModelService.getTransportCompetitors(this.data.ref).subscribe(result => {
        //Store data
        this.competitors = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
}
