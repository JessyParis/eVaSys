import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, UntypedFormArray, ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import moment from "moment";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { cmp, getCreationModificationTooltipText } from "../../../globals/utils";
import { showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { BaseFormComponent } from "../../_ancestors/base-form.component";
import { UtilsService } from "../../../services/utils.service";
import { DomSanitizer } from "@angular/platform-browser";

class MyValidators {
  //Constructor
  constructor() { }
  /** 100% */
  fullPercentValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    let p: number = 0;
    const fCAAS: UntypedFormArray = control as UntypedFormArray;
    for (let i in fCAAS.controls) {
      let fG: UntypedFormGroup = fCAAS.controls[i] as UntypedFormGroup;
      p += (fG.get("Ratio").value ? fG.get("Ratio").value : 0);
    }
    return p !== 100 ? { fullPercent: true } : null;
  }
}

@Component({
    selector: "client-application",
    templateUrl: "./client-application.component.html",
    styleUrls: ["./client-application.component.scss"],
    standalone: false
})

export class ClientApplicationComponent extends BaseFormComponent<dataModelsInterfaces.ClientApplication> {
  //Variables
  clientApplication: dataModelsInterfaces.ClientApplication;
  //Form
  form: UntypedFormGroup;
  cAAs: UntypedFormArray = new UntypedFormArray([], new MyValidators().fullPercentValidator);
  entiteList: dataModelsInterfaces.EntiteList[];
  yearList: appInterfaces.Year[];
  applicationProduitOrigineList: dataModelsInterfaces.ApplicationProduitOrigine[];
  entiteListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  yearListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  applicationProduitOrigineListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  //Constructor
  constructor(protected activatedRoute: ActivatedRoute
    , protected router: Router
    , private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected utilsService: UtilsService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
    , protected sanitizer: DomSanitizer) {
    super("ClientApplicationComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      EntiteList: this.entiteListFC,
      YearList: this.yearListFC,
      ApplicationProduitOrigineList: this.applicationProduitOrigineListFC,
      CAAs: this.cAAs,
    });
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.entiteListFC.disable();
    this.yearListFC.disable();
    this.applicationProduitOrigineListFC.disable();
    this.cAAs.disable();
  }
  //-----------------------------------------------------------------------------------
  //Lock main controls
  lockMainControls() {
    this.entiteListFC.disable();
    this.yearListFC.disable();
    this.applicationProduitOrigineListFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock main controls
  unlockMainControls() {
    this.entiteListFC.enable();
    this.yearListFC.enable();
    this.applicationProduitOrigineListFC.enable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.entiteListFC.enable();
    this.yearListFC.enable();
    this.applicationProduitOrigineListFC.enable();
    this.cAAs.enable();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Lock main controls for existing element
    if (!(this.clientApplication.RefClientApplication > 0)) {
      //Lock details if main controls not valid
      if (!this.entiteListFC.value || !this.yearListFC.value || !this.applicationProduitOrigineListFC.value) {
        this.cAAs.disable();
      }
      else {
        this.cAAs.enable();
        let filled = false;
        for (let i in this.cAAs.controls) {
          let fG: UntypedFormGroup = this.cAAs.controls[i] as UntypedFormGroup;
          if (fG.get("Ratio").value) { filled = true; }
        }
        if (filled) { this.lockMainControls(); } else { this.unlockMainControls(); }
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    //Get Entite
    this.listService.getListClient(null, null, null, null, true)
      .subscribe(result => {
        this.entiteList = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get Entite
    this.listService.getListApplicationProduitOrigine()
      .subscribe(result => {
        this.applicationProduitOrigineList = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing year
    this.listService.getListYear().subscribe(result => {
      this.yearList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check parameters
    if (!isNaN(id)) {
      this.dataModelService.getDataModel<dataModelsInterfaces.ClientApplication>(id, this.componentName).subscribe(result => {
        //Get data
        this.clientApplication = result;
        this.sourceObj = result;
        //Sort ClientApplicationApplication
        var sortedArray: dataModelsInterfaces.ClientApplicationApplication[] = this.clientApplication.ClientApplicationApplications.sort(function (a, b) {
          return cmp(
            [-cmp(a.Ratio == 0 ? 0 : 1, b.Ratio == 0 ? 0 : 1), cmp(a.Application.Libelle, b.Application.Libelle)],
            [-cmp(b.Ratio == 0 ? 0 : 1, a.Ratio == 0 ? 0 : 1), cmp(b.Application.Libelle, a.Application.Libelle)]);
        });
        this.clientApplication.ClientApplicationApplications = sortedArray;
        //Update form
        this.updateForm();
        //Initial lock for existing element
        if (this.clientApplication.RefClientApplication > 0) {
          this.lockMainControls();
        }
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
    this.entiteListFC.setValue((this.clientApplication.Entite ? this.clientApplication.Entite.RefEntite : null));
    this.applicationProduitOrigineListFC.setValue((this.clientApplication.ApplicationProduitOrigine ? this.clientApplication.ApplicationProduitOrigine.RefApplicationProduitOrigine : null));
    this.yearListFC.setValue(this.clientApplication.D ? (moment(this.clientApplication.D).year()!==1 ? moment(this.clientApplication.D).year() : null) : null);
    for (const cAA of this.clientApplication.ClientApplicationApplications) {
      const grp = this.fb.group({
        Ratio: [cAA.Ratio? cAA.Ratio : null, Validators.max(100)],
      });
      this.cAAs.push(grp);
    }
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    if (this.entiteListFC.value == null) { this.clientApplication.Entite = null }
    else {
      this.clientApplication.Entite = this.entiteList.find(x => x.RefEntite === this.entiteListFC.value);
      this.dataModelService.getEntite(this.entiteListFC.value, null, null).subscribe(result => this.clientApplication.Entite = result, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    this.clientApplication.ApplicationProduitOrigine = this.applicationProduitOrigineListFC.value == null ? null : this.applicationProduitOrigineList.find(x => x.RefApplicationProduitOrigine === this.applicationProduitOrigineListFC.value);
    this.clientApplication.D = this.yearListFC.value == null ? null : moment(new Date(this.yearListFC.value, 1, 1));
    for (let i in this.cAAs.controls) {
      let fG: UntypedFormGroup = this.cAAs.controls[i] as UntypedFormGroup;
      this.clientApplication.ClientApplicationApplications[i].Ratio = (fG.get("Ratio").value ? fG.get("Ratio").value : 0);
    }
  }
  //-----------------------------------------------------------------------------------
  //Get data for IHM
  get getFormData(): UntypedFormArray {
    return this.cAAs as UntypedFormArray;
  }
  //-----------------------------------------------------------------------------------
  //Get sumPercent
  getSumPercent(): number {
    let r: number = 0;
    for (let i in this.cAAs.controls) {
      let fG: UntypedFormGroup = this.cAAs.controls[i] as UntypedFormGroup;
      r += (fG.get("Ratio").value ? fG.get("Ratio").value : 0);
    }
    return 100-r;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<dataModelsInterfaces.ClientApplication>(this.clientApplication, this.componentName)
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
    this.dataModelService.deleteDataModel<dataModelsInterfaces.ClientApplication>(id, this.componentName)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(this.ressAfterDel), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
}
