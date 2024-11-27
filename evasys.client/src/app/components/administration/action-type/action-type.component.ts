import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { cmp, getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { ListService } from "../../../services/list.service";

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
    selector: "action-type",
    templateUrl: "./action-type.component.html",
    standalone: false
})

export class ActionTypeComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  actionType: dataModelsInterfaces.ActionType = {} as dataModelsInterfaces.ActionType;
  actionTypeEntiteTypeList: dataModelsInterfaces.ActionTypeEntiteType[] = [];
  libelleFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  documentTypeFC: UntypedFormControl = new UntypedFormControl(null);
  entiteTypeListFC: UntypedFormControl = new UntypedFormControl(null);
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private listService: ListService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    //Init data
    this.actionType.RefActionType = 0;
    if (this.actionType.ActionTypeEntiteTypes == null) { this.actionType.ActionTypeEntiteTypes = []; }
    //Create form
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Libelle: this.libelleFC,
      EntiteTypeList: this.entiteTypeListFC,
      DocumentType: this.documentTypeFC,
    });
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
    //Load initial data
    this.listService.getListEntiteType().subscribe(result => {
      this.actionTypeEntiteTypeList = result.map(item => ({ EntiteType: item } as dataModelsInterfaces.ActionTypeEntiteType));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check parameters
    if (!isNaN(id)) {
      this.dataModelService.getActionType(id).subscribe(result => {
        //Get data
        this.actionType = result;
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
    this.libelleFC.setValue(this.actionType.Libelle);
    this.entiteTypeListFC.setValue(this.actionType.ActionTypeEntiteTypes);
    this.documentTypeFC.setValue(this.actionType.DocumentType);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.actionType.Libelle = this.libelleFC.value;
    this.actionType.ActionTypeEntiteTypes = this.entiteTypeListFC.value;
    this.actionType.DocumentType = this.documentTypeFC.value ? true : false;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //EntiteType
    this.dataModelService.postActionType(this.actionType)
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
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(821) },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    var url = this.baseUrl + "evapi/regionee/" + this.actionType.RefActionType;
    this.dataModelService.deleteActionType(this.actionType)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1064), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Multi select
  compareFnEntiteType(item: dataModelsInterfaces.ActionTypeEntiteType, selectedItem: dataModelsInterfaces.ActionTypeEntiteType) {
    return item && selectedItem ? item.EntiteType.RefEntiteType === selectedItem.EntiteType.RefEntiteType : item === selectedItem;
  }
  public onEntiteTypeListReset() {
    this.actionType.ActionTypeEntiteTypes = [];
    this.entiteTypeListFC.setValue(this.actionType.ActionTypeEntiteTypes);
  }
  //-----------------------------------------------------------------------------------
  //Create EntiteType label
  entiteTypesLabel(): string {
    let s = this.entiteTypeListFC.value?.sort(function (a, b) { return cmp(a.EntiteType.Libelle, b.EntiteType.Libelle); })
      .map(item => item.EntiteType.Libelle).join("\n");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.actionType);
  }
}
