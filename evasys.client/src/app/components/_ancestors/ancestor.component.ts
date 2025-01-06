import { Injectable, OnInit } from "@angular/core";
import { UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationUserContext } from "../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../services/list.service";
import { DataModelService } from "../../services/data-model.service";
import * as dataModelsInterfaces from "../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../interfaces/appInterfaces";
import { ConfirmComponent } from "../dialogs/confirm/confirm.component";
import { getCreationModificationTooltipText, showErrorToUser } from "../../globals/utils";
import { SnackBarQueueService } from "../../services/snackbar-queue.service";
import { EnvComponentControl, EnvBox } from "../../classes/appClasses";
import { FormControlType, ListContentType } from "../../globals/enums";
import { SafeHtml, DomSanitizer } from "@angular/platform-browser";
import { UtilsService } from "../../services/utils.service";

class MyErrorStateMatcher implements ErrorStateMatcher {
  //Constructor
  constructor() { }
  isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    let result: boolean = false;
    result = !!((control && control.invalid));
    return result;
  }
}
@Injectable()
export abstract class AncestorComponent<T extends { AllowStandardCRUD: string }> implements OnInit {
  currentbox = -1;
  ressLibelSing = 0;
  ressBeforeDel = 0;
  ressAfterDel = 0;
  componentRef = "";
  formClass = "ev-dataform-container";
  getId0 = false;
  matcher = new MyErrorStateMatcher();
  form = this.fb.group({});
  //Create/modi tooltip
  createModifTooltipVisible: boolean = false;
  //Boxes
  boxes: EnvBox[] = [];
  // Source object
  sourceObj: T = {} as T;
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  // Dropdown lists
  paysList: dataModelsInterfaces.Pays[];
  //Help
  hasAide: boolean = false;
  visibleAide: boolean = false;
  aide: dataModelsInterfaces.Aide;
  safeValeurHTML: SafeHtml;
  //Constructor
  constructor(protected componentName: string, protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog
    , protected utilsService: UtilsService
    , protected sanitizer: DomSanitizer
  ) {
    let cmp = applicationUserContext.envComponents.find(x => x.name === this.componentName)
    if (cmp) {
      this.ressLibelSing = cmp.ressLibel;
      this.ressBeforeDel = cmp.ressBeforeDel;
      this.ressAfterDel = cmp.ressAfterDel;
      this.componentRef = cmp.ref;
      this.formClass = cmp.formClass;
      this.getId0 = cmp.getId0;
      this.createModifTooltipVisible = cmp.createModifTooltipVisible;
      if (cmp.hasHelp) {
        this.getHelp();
      }
    }
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Initializes form controls
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.boxes[0].Controls.push(new EnvComponentControl({
      field: "Libelle",
      type: FormControlType.Texte,
      ress: this.ressLibelSing,
      required: true,
      FC: new UntypedFormControl(null, [Validators.required])
    }));
  }
  //-----------------------------------------------------------------------------------
  //Forms initial creation
  createForm() {
    this.initControls();
    this.boxes.forEach(box =>
      box.Controls.forEach(ctrl => {
        this.form.addControl(ctrl.field, ctrl.FC)
      }));
  }
  //-----------------------------------------------------------------------------------
  //Boxes creation
  CreateBox(ressName: number): EnvBox {
    let box = new EnvBox;
    box.ressName = ressName;
    this.boxes.push(box);
    this.currentbox = this.boxes.length - 1;
    return box;
  }
  //-----------------------------------------------------------------------------------
  //Defines the active box
  SetCurrentBox(boxIndex: number) {
    this.currentbox = boxIndex;
  }
  //-----------------------------------------------------------------------------------
  //Additionnal control definition
  CreateControl<K extends keyof T>(field: K, type: FormControlType, required: boolean, ress: number): EnvComponentControl {
    let ctrl = new EnvComponentControl();
    ctrl.field = field.toString();
    ctrl.type = type;
    ctrl.required = required;
    ctrl.FC = new UntypedFormControl(null);
    ctrl.ress = ress;
    ctrl.maxlength = type == FormControlType.TexteMulti ? 500 : 50;
    if (required) {
      ctrl.FC.addValidators(Validators.required);
      ctrl.FC.updateValueAndValidity();
    }
    if (type == FormControlType.DatePicker) {
      ctrl.class = "ev-date-picker";
    }
    this.boxes[this.currentbox].Controls.push(ctrl);
    return ctrl;
  }
  CreateControlWithFC<K extends keyof T>(field: K, FC: UntypedFormControl, type: FormControlType, ress: number): EnvComponentControl {
    let ctrl = new EnvComponentControl();
    ctrl.field = field.toString();
    ctrl.FC = FC;
    ctrl.ress = ress;
    ctrl.type = type;
    ctrl.maxlength = type == FormControlType.TexteMulti ? 500 : 50;
    ctrl.required = FC.hasValidator(Validators.required);
    this.boxes[this.currentbox].Controls.push(ctrl);
    return ctrl;
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Global lock
    if (1 !== 1) {
      this.lockScreen();
    }
    else {
      //Init
      this.unlockScreen();
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    var controls: EnvComponentControl[] = [];
    this.boxes.forEach(form => controls = controls.concat(form.Controls));
    // Load the required lists to fill the dropdowns
    this.LoadList(controls);
    //Load initial data
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0 || this.getId0) {
        this.dataModelService.getDataModel<T>(id, this.componentName).subscribe(result => {
          //Get data
          this.sourceObj = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Update the forms using the object properties
  updateForm() {
    this.boxes.forEach(box => {
      box.Controls.forEach(x => {
        // If there is a list of authorized values
        if (x.listDesc) {
          // Sometimes the item is not stored directly in the field and we have to use a specific field to identify that item
          if (x.listStore != ".") {
            x.FC.setValue(x.listItems.find(y => y[x.listStore] === this.sourceObj[x.field]));
          } else {
            // When the item is stored directly in the field, must find the corresponding item in the list, using its reference field
            x.FC.setValue(x.listItems.find(y => y[x.listRef] === this.sourceObj[x.field][x.listRef]));
          }
        } else {
          // Default is always to directly store sourceobj field value in the formcontrol
          x.FC.setValue(this.sourceObj[x.field])
        }
      });
    })
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model using the form controls values
  saveData() {
    this.boxes.forEach(form => {
      form.Controls.forEach(x => {
        if (x.listDesc && x.listStore != ".") {
          if (x.FC.value == null) {
            this.sourceObj[x.field] = null;
          }
          else {
            this.sourceObj[x.field] = x.FC.value[x.listStore];
          }
        } else {
          this.sourceObj[x.field] = x.FC.value
        }
      })
    })
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<T>(this.sourceObj, this.componentName)
      .subscribe(result => {
        //Redirect to grid and inform user
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
    this.dataModelService.deleteDataModel<T>(id, this.componentName)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(this.ressAfterDel), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Get object reference
  GetReference(): number {
    return this.sourceObj[this.componentRef];
  }
  GetCreationText(): string {
    return (this.sourceObj["CreationText"] ? this.sourceObj["CreationText"] : "");
  }
  GetModificationText(): string {
    return (this.sourceObj["ModificationText"] ? this.sourceObj["ModificationText"] : "");
  }
  GetBoxDescription(ress: number): string {
    if (ress == 0)
      return "";
    return this.applicationUserContext.getCulturedRessourceText(ress);
  }
  LoadList(controls: EnvComponentControl[]) {
    // Builds a list containing all distnct FormControlTypes among the form controls
    let contents: ListContentType[] = [];
    controls.forEach(ctrl => contents.push(ctrl.listContent))
    // Keeps unique values
    contents = [...new Set(contents)];
    // Check on each type if there is an associated list of authorized values
    contents.forEach(content => {
      let service = "";
      let desc = "";
      let store = ".";
      let ref = "";
      if (content == ListContentType.ModeTransportEE) {
        service = "getListModeTransportEE";
        desc = "Libelle";
        ref = "RefModeTransportEE";
      }
      if (content == ListContentType.Pays) {
        service = "getListPays";
        desc = "Libelle";
        ref = "RefPays";
      }
      if (content == ListContentType.Couleur) {
        service = "getListCouleur";
        desc = "Libelle";
        ref = "RefCouleur";
        store = "RefCouleur";
      }
      else if (content == ListContentType.ProduitGroupeReportingTypeRef) {
        service = "getListProduitGroupeReportingType";
        desc = "Libelle";
        ref = "RefProduitGroupeReportingType";
        store = "RefProduitGroupeReportingType";
      }
      else if (content == ListContentType.RegionReportingRef) {
        service = "getListRegionReporting";
        desc = "Libelle"
        store = "RefRegionReporting";
      }
      // If formcontroltype requires a list, then service contains the query function
      if (service != "") {
        this.listService[service]().subscribe(result => {
          // Once the list is downloaded, spreads the data on each EnvComponentControl who uses that list
          controls.forEach(ctrl => {
            if (ctrl.listContent == content) {
              ctrl.listItems = result;
              ctrl.listDesc = desc;
              ctrl.listStore = store;
              ctrl.listRef = ref;
            }
          })
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    })
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(
      {
        CreationText: (this.sourceObj["CreationText"] ? this.sourceObj["CreationText"] : "")
        , ModificationText: (this.sourceObj["ModificationText"] ? this.sourceObj["ModificationText"] : "")
      } as any);
  }
  //-----------------------------------------------------------------------------------
  //Get help
  getHelp() {
    //Get help
    this.utilsService.getAide(this.componentName, this.applicationUserContext, this.dataModelService)
      .subscribe({
        next: (result) => {
          this.aide = result;
          if (result && result?.ValeurHTML) {
            this.hasAide = true;
            this.safeValeurHTML = this.sanitizer.bypassSecurityTrustHtml(this.aide.ValeurHTML);
          }
        }
        , error: (err) =>   {}
      });
  }
}
