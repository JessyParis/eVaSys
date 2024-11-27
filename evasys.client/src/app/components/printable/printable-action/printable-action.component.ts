import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getAttachmentFilename, showConfirmToUser, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import { ErrorStateMatcher } from "@angular/material/core";
import { UploadComponent } from "../../dialogs/upload/upload.component";
import { DownloadService } from "../../../services/download.service";
import { HttpResponse } from "@angular/common/http";
import { pdf } from "@progress/kendo-drawing";

class MyErrorStateMatcher implements ErrorStateMatcher {
  //Constructor
  constructor() { }
  isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    let result: boolean = false;
    result = !!((control && control.invalid));
    return result;
  }
}

@Component({
    selector: "printable-action",
    templateUrl: "./printable-action.component.html",
    standalone: false
})

export class PrintableActionComponent implements OnInit {
  @Input() action: dataModelsInterfaces.Action;
  @Input() entite: dataModelsInterfaces.Entite;
  form: UntypedFormGroup;
  //Constructor
  constructor(private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private listService: ListService
    , private downloadService: DownloadService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
  }
  //-----------------------------------------------------------------------------------
  //Create Entite label
  entiteLabel(): string {
    let s = this.entite.Libelle;
    if (this.entite.CodeEE) {
      s += " (" + this.entite.CodeEE + ")";
    }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create ActionType label
  actionActionTypesLabel(): string {
    let s = this.action.ActionActionTypes?.map(item => item.ActionType.Libelle).join("\n");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationTooltipText() {
    return (this.action?.CreationText + " - " + this.action?.ModificationText);
  }
}
