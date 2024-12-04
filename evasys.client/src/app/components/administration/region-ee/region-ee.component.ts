import { Component } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { cmp, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { ListService } from "../../../services/list.service";
import { DomSanitizer } from "@angular/platform-browser";
import { UtilsService } from "../../../services/utils.service";
import { BaseFormComponent } from "../../_ancestors/base-form.component";

@Component({
    selector: "region-ee",
    templateUrl: "./region-ee.component.html",
    standalone: false
})

export class RegionEEComponent extends BaseFormComponent<dataModelsInterfaces.RegionEE> {
  //Form
  form: UntypedFormGroup;
  regionEE: dataModelsInterfaces.RegionEE = {} as dataModelsInterfaces.RegionEE;
  regionEEDptList: dataModelsInterfaces.RegionEEDpt[] = [];
  libelleFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  dptListFC: UntypedFormControl = new UntypedFormControl(null);
  //Constructor
  constructor(protected activatedRoute: ActivatedRoute
    , protected router: Router
    , private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected utilsService: UtilsService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
    , protected sanitizer: DomSanitizer) {
    super("RegionEEComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    //Create form
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Libelle: this.libelleFC,
      DptList: this.dptListFC,
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    this.listService.getListDpt().subscribe(result => {
      this.regionEEDptList = result.map(item => ({ Dpt: item } as dataModelsInterfaces.RegionEEDpt));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check parameters
    if (!isNaN(id)) {
      this.dataModelService.getDataModel<dataModelsInterfaces.RegionEE>(id, this.componentName).subscribe(result => {
        //Get data
        this.regionEE = result;
        this.sourceObj = result;
        //Update form
        this.updateForm();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.libelleFC.setValue(this.regionEE.Libelle);
    this.dptListFC.setValue(this.regionEE.RegionEEDpts);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.regionEE.Libelle = this.libelleFC.value;
    this.regionEE.RegionEEDpts = this.dptListFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<dataModelsInterfaces.RegionEE>(this.regionEE, this.componentName)
      .subscribe(result => {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(this.ressBeforeDel) },
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
    this.dataModelService.deleteDataModel<dataModelsInterfaces.RegionEE>(id, this.componentName)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(this.ressAfterDel), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Multi select
  compareFnDpt(item: dataModelsInterfaces.RegionEEDpt, selectedItem: dataModelsInterfaces.RegionEEDpt) {
    return item && selectedItem ? item.Dpt.RefDpt === selectedItem.Dpt.RefDpt : item === selectedItem;
  }
  public onDptListReset() {
    this.regionEE.RegionEEDpts = [];
    this.dptListFC.setValue(this.regionEE.RegionEEDpts);
  }
  //-----------------------------------------------------------------------------------
  //Create Dpt label
  dptsLabel(): string {
    let s = this.dptListFC.value?.sort(function (a, b) { return cmp(a.Dpt.Libelle, b.Dpt.Libelle); })
      .map(item => item.Dpt.Libelle).join("\n");
    //End
    return s;
  }
}
