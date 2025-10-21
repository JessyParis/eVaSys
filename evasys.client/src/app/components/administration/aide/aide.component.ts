import { Component } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { cmp, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { kendoFontData } from "../../../globals/kendo-utils";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { EnvComponent } from "../../../classes/appClasses";
import { DomSanitizer } from "@angular/platform-browser";
import { UtilsService } from "../../../services/utils.service";
import { BaseFormComponent } from "../../_ancestors/base-form.component";
import { SnackbarMsgType } from "../../../globals/enums";

@Component({
    selector: "aide",
    templateUrl: "./aide.component.html",
    standalone: false
})

export class AideComponent extends BaseFormComponent<dataModelsInterfaces.Aide> {
  //Form
  form: UntypedFormGroup;
  aideObj: dataModelsInterfaces.Aide = {} as dataModelsInterfaces.Aide;
  envComponents: EnvComponent[] = [];
  envComponentListFC: UntypedFormControl = new UntypedFormControl(null);
  valeurHTMLFC: UntypedFormControl = new UntypedFormControl(null);
  //Misc
  public kendoFontData = kendoFontData;
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
    super("AideComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      EnvComponentList: this.envComponentListFC,
      ValeurHTML: this.valeurHTMLFC,
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    this.envComponents = this.applicationUserContext.envComponents.filter(e => e.hasHelp);
    this.envComponents.sort(function (a, b) { return cmp(a.libelle, b.libelle); })
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getDataModel<dataModelsInterfaces.Aide>(id, this.componentName).subscribe(result => {
          //Get data
          this.aideObj = result;
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
    this.envComponentListFC.setValue(this.aideObj.Composant);
    this.valeurHTMLFC.setValue(this.aideObj.ValeurHTML);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.aideObj.Composant = this.envComponentListFC.value;
    this.aideObj.ValeurHTML = this.valeurHTMLFC.value;
    let cmp = this.applicationUserContext.envComponents.find(e => e.name == this.aideObj.Composant);
    this.aideObj.RefRessource = cmp.ressLibel;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<dataModelsInterfaces.Aide>(this.aideObj, this.componentName)
      .subscribe(result => {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
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
    this.dataModelService.deleteDataModel<dataModelsInterfaces.Aide>(id, this.componentName)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(this.ressAfterDel), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
}
