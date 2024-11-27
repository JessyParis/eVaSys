import { Component, Inject, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, FormGroupDirective, NgForm, ValidatorFn, AbstractControl, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import moment from "moment";
import { ErrorStateMatcher } from "@angular/material/core";
import { showErrorToUser, showInformationToUser } from "../../../globals/utils";

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
    selector: "entite-camion-type",
    templateUrl: "./entite-camion-type.component.html",
    standalone: false
})

export class EntiteCamionTypeComponent implements OnInit {
  @Input() refEntiteCamionType: number;
  @Input() type: string;
  @Input() eCT: dataModelsInterfaces.EntiteCamionType;
  @Input() entite: dataModelsInterfaces.Entite;
  @Input() refEntiteType: number;
  @Output() askClose = new EventEmitter<any>();
  matcher = new MyErrorStateMatcher();
  entiteCamionType: dataModelsInterfaces.EntiteCamionType;
  form: UntypedFormGroup;
  camionTypeList: dataModelsInterfaces.CamionType[];
  //Global lock
  locked: boolean = true;
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    // create an empty object from the EntiteCamionType interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: [true],
      CamionTypeList: [null, Validators.required],
      Cmt: [null],
    });
    if (1 != 1) {
      this.lockScreen();
    }
    else {
      this.locked = false;
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    this.entiteCamionType = { ...this.eCT };
    //Get existing CamionType
    this.listService.getListCamionType().subscribe(result => {
      this.camionTypeList = result;
      //Auto select
      if (this.camionTypeList.length === 1) { this.form.get("CamionTypeList").setValue(this.camionTypeList[0].RefCamionType); }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("CamionTypeList").disable();
    this.form.get("Cmt").disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("CamionTypeList").setValue(this.entiteCamionType?.CamionType?.RefCamionType);
    this.form.get("Cmt").setValue(this.entiteCamionType.Cmt);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.entiteCamionType.Cmt = this.form.get("Cmt").value;
    this.entiteCamionType.CamionType = this.camionTypeList.find(x => x.RefCamionType === this.form.get("CamionTypeList").value);
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    //Save data to current object 
    this.applyChanges();
    //Check duplicates
    if (this.entite.EntiteCamionTypes?.some(item =>
      item.RefEntiteCamionType != this.entiteCamionType.RefEntiteCamionType && item.CamionType.RefCamionType == this.entiteCamionType.CamionType.RefCamionType
    )) {
      showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(115), this.applicationUserContext.getCulturedRessourceText(410), this.applicationUserContext);
    }
    else {
      //Save data
      this.eCT = { ...this.entiteCamionType };
      //Emit event for dialog closing
      this.askClose.emit({ type: this.type, ref: this.eCT });
    }
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationTooltipText() {
    let eCT = this.entiteCamionType;
    if (!!eCT.DCreation) {
      return `${this.applicationUserContext.getCulturedRessourceText(388) + " " + moment(eCT.DCreation).format("L") + " " + this.applicationUserContext.getCulturedRessourceText(390) + " " + eCT.UtilisateurCreation.Nom}`;
    }
    else {
      return "";
    }
  }
}
