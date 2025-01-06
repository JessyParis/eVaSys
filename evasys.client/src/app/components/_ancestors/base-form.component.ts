import { Injectable } from "@angular/core";
import { UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationUserContext } from "../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import * as dataModelsInterfaces from "../../interfaces/dataModelsInterfaces";
import { getCreationModificationTooltipText, showErrorToUser } from "../../globals/utils";
import { SnackBarQueueService } from "../../services/snackbar-queue.service";
import { DataModelService } from "../../services/data-model.service";
import { UtilsService } from "../../services/utils.service";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";

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
export abstract class BaseFormComponent<T extends { AllowBaseForm: boolean }> {
  ressLibelSing = 0;
  ressBeforeDel = 0;
  ressAfterDel = 0;
  componentRef = "";
  formClass = "ev-dataform-container";
  getId0 = false;
  matcher = new MyErrorStateMatcher();
  //Create/modi tooltip
  createModifTooltipVisible: boolean = false;
  // Source object
  sourceObj: T = {} as T;
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  //Help
  hasAide: boolean = false;
  visibleAide: boolean = false;
  aide: dataModelsInterfaces.Aide;
  safeValeurHTML: SafeHtml;
  //Constructor
  constructor(protected componentName: string
    , protected activatedRoute: ActivatedRoute
    , protected router: Router
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected utilsService: UtilsService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
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
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
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
            this.safeValeurHTML=this.sanitizer.bypassSecurityTrustHtml(this.aide.ValeurHTML);
          }
        }
        , error: (err) => { }
      });
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
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(
      {
        CreationText: (this.sourceObj["CreationText"] ? this.sourceObj["CreationText"] : "")
        , ModificationText: (this.sourceObj["ModificationText"] ? this.sourceObj["ModificationText"] : "")
      } as any);
  }
}
