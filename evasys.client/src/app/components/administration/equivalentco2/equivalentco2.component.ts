import { Component } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { UtilsService } from "../../../services/utils.service";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { UploadComponent } from "../../dialogs/upload/upload.component";
import { BaseFormComponent } from "../../_ancestors/base-form.component";
import { DomSanitizer } from "@angular/platform-browser";

@Component({
    selector: "equivalentco2",
    templateUrl: "./equivalentco2.component.html",
    standalone: false
})

export class EquivalentCO2Component extends BaseFormComponent<dataModelsInterfaces.EquivalentCO2> {
  //Form
  form: UntypedFormGroup;
  equivalentCO2: dataModelsInterfaces.EquivalentCO2 = {Actif: true} as dataModelsInterfaces.EquivalentCO2;
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  libelleFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  ordreFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  ratioFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  //Misc
  lastUploadResut: appInterfaces.UploadResponseBody;
  //Constructor
  constructor(protected activatedRoute: ActivatedRoute
    , protected router: Router
    , private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected utilsService: UtilsService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
    , protected sanitizer: DomSanitizer) {
    super("EquivalentCO2Component", activatedRoute, router, applicationUserContext, dataModelService
      ,utilsService, snackBarQueueService, dialog, sanitizer);
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: this.actifFC,
      Libelle: this.libelleFC,
      Ordre: this.ordreFC,
      Ratio: this.ratioFC,
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getDataModel<dataModelsInterfaces.EquivalentCO2>(id, "EquivalentCO2Component").subscribe(result => {
          //Get data
          this.equivalentCO2 = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
      else {
        //Update form
        this.updateForm();
      }
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.actifFC.setValue(this.equivalentCO2.Actif);
    this.libelleFC.setValue(this.equivalentCO2.Libelle);
    this.ordreFC.setValue(this.equivalentCO2.Ordre);
    this.ratioFC.setValue(this.equivalentCO2.Ratio);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.equivalentCO2.Actif = this.actifFC.value ? true : false;
    this.equivalentCO2.Libelle = this.libelleFC.value;
    this.equivalentCO2.Ordre = this.ordreFC.value;
    this.equivalentCO2.Ratio = this.ratioFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(next: string) {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<dataModelsInterfaces.EquivalentCO2>(this.equivalentCO2, "EquivalentCO2Component")
      .subscribe(result => {
        //Reload data
        this.equivalentCO2 = result;
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Next action if applicable or return to man grid
        if (next == "UploadFile") {
          //open pop-up
          this.startUploadFile();
        }
        else {
          this.router.navigate(["grid"]);
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(1396) },
      autoFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    this.dataModelService.deleteDataModel<dataModelsInterfaces.EquivalentCO2>(id, "EquivalentCO2Component")
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1397), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  onUploadFile() {
    //Save to database
      this.onSave("UploadFile");
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  startUploadFile() {
    //open pop-up
    const dialogRef = this.dialog.open(UploadComponent, {
      width: "50%", height: "50%",
      data: {
        title: this.applicationUserContext.getCulturedRessourceText(626), message: this.applicationUserContext.getCulturedRessourceText(1399)
        , multiple: false, type: "equivalentco2", fileType: 0, ref: this.equivalentCO2.RefEquivalentCO2.toString()
      },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      this.lastUploadResut = result;
      if (!this.lastUploadResut) {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(341), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
      else if (this.lastUploadResut.error != "") {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(631), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
      else {
        //Reload data
        this.dataModelService.getDataModel<dataModelsInterfaces.EquivalentCO2>(this.equivalentCO2.RefEquivalentCO2, "EquivalentCO2Component").subscribe(result => {
          //Get data
          this.equivalentCO2 = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(628), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
    });
  }
}
