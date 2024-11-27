import { Component, HostListener, Inject, OnInit, ViewEncapsulation } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from "@angular/material/dialog";
import { ApplicationUserContext } from "../../../globals/globals";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { DataModelService } from "../../../services/data-model.service";
import { showErrorToUser } from "../../../globals/utils";
import moment from "moment";

@Component({
    selector: "message-visualisations",
    templateUrl: "./message-visualisations.component.html",
    encapsulation: ViewEncapsulation.None,
    standalone: false
})

export class MessageVisualisationsComponent implements OnInit {
  message: dataModelsInterfaces.Message;
  constructor(public dialogRef: MatDialogRef<MessageVisualisationsComponent>
    , @Inject(MAT_DIALOG_DATA) public data: any
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , public dialog: MatDialog
  ) {
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    if (this.data.ref) {
      //Get MessageVisualisations
      this.dataModelService.getMessage(this.data.ref, true).subscribe(result => {
        //Format dates
        this.dataModelService.setMomentFromTextMessage(result);
        //Store data
        this.message = result;
        //Format data
        this.message.MessageVisualisations.forEach(e => e.D = moment(e.D).format("DD/MM/YYYY HH:mm"));
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
}
