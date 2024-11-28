import { Component, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { cmp, getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DownloadService } from "../../../services/download.service";
import { kendoFontData } from "../../../globals/kendo-utils";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { EnvComponent } from "../../../classes/appClasses";

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
    selector: "aide",
    templateUrl: "./aide.component.html",
    standalone: false
})

export class AideComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  aide: dataModelsInterfaces.Aide = {} as dataModelsInterfaces.Aide;
  envComponents: EnvComponent[] = [];
  envComponentListFC: UntypedFormControl = new UntypedFormControl(null);
  valeurHTMLFC: UntypedFormControl = new UntypedFormControl(null);
  //Misc
  public kendoFontData = kendoFontData;
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  //Misc
  lastUploadResut: appInterfaces.UploadResponseBody;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private downloadService: DownloadService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
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
    this.envComponents = this.applicationUserContext.envComponents.filter(e => e.hasHelp);
    this.envComponents.sort(function (a, b) { return cmp(a.libelle, b.libelle); })
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getDataModel<dataModelsInterfaces.Aide>(id, "AideComponent").subscribe(result => {
          //Get data
          this.aide = result;
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
    this.envComponentListFC.setValue(this.aide.Composant);
    this.valeurHTMLFC.setValue(this.aide.ValeurHTML);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.aide.Composant = this.envComponentListFC.value;
    this.aide.ValeurHTML = this.valeurHTMLFC.value;
    let cmp = this.applicationUserContext.envComponents.find(e => e.name == this.aide.Composant);
    this.aide.RefRessource = cmp.ressLibel;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(next: string) {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<dataModelsInterfaces.Aide>(this.aide, "AideComponent")
      .subscribe(result => {
        //Reload data
        this.aide = result;
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
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(1526) },
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
    this.dataModelService.deleteDataModel<dataModelsInterfaces.Aide>(id, "AideComponent")
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1527), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.aide);
  }
}
