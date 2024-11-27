import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { cmp, getAttachmentFilename, getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { UploadComponent } from "../../dialogs/upload/upload.component";
import { DownloadService } from "../../../services/download.service";
import { DocumentType } from "../../../globals/enums";
import { kendoFontData } from "../../../globals/kendo-utils";

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
    selector: "parametre",
    templateUrl: "./parametre.component.html",
    standalone: false
})

export class ParametreComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  parametre: dataModelsInterfaces.Parametre = {} as dataModelsInterfaces.Parametre;
  serveurFC: UntypedFormControl = new UntypedFormControl(null);
  libelleFC: UntypedFormControl = new UntypedFormControl(null);
  valeurNumeriqueFC: UntypedFormControl = new UntypedFormControl(null);
  valeurTexteFC: UntypedFormControl = new UntypedFormControl(null);
  valeurHTMLFC: UntypedFormControl = new UntypedFormControl(null);
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  //Misc
  public kendoFontData = kendoFontData;
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  //Misc
  lastUploadResut: appInterfaces.UploadResponseBody;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
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
      Serveur: this.serveurFC,
      Libelle: this.libelleFC,
      ValeurNumerique: this.valeurNumeriqueFC,
      ValeurTexte: this.valeurTexteFC,
      ValeurHTML: this.valeurHTMLFC,
      Cmt: this.cmtFC,
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
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getDataModel<dataModelsInterfaces.Parametre>(id, "ParametreComponent").subscribe(result => {
          //Get data
          this.parametre = result;
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
    this.serveurFC.setValue(this.parametre.Serveur);
    this.libelleFC.setValue(this.parametre.Libelle);
    this.valeurNumeriqueFC.setValue(this.parametre.ValeurNumerique);
    this.valeurTexteFC.setValue(this.parametre.ValeurTexte);
    this.valeurHTMLFC.setValue(this.parametre.ValeurHTML);
    this.cmtFC.setValue(this.parametre.Cmt);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.parametre.Serveur = this.serveurFC.value ? true : false;
    this.parametre.Libelle = this.libelleFC.value;
    this.parametre.ValeurNumerique = this.valeurNumeriqueFC.value;
    this.parametre.ValeurTexte = this.valeurTexteFC.value;
    this.parametre.ValeurHTML = this.valeurHTMLFC.value;
    this.parametre.Cmt = this.cmtFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(next: string) {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<dataModelsInterfaces.Parametre>(this.parametre, "ParametreComponent")
      .subscribe(result => {
        //Reload data
        this.parametre = result;
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
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.parametre);
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
        , multiple: false, type: "parametre", fileType: 0, ref: this.parametre.RefParametre.toString()
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
        this.dataModelService.getDataModel<dataModelsInterfaces.Parametre>(this.parametre.RefParametre, "ParametreComponent").subscribe(result => {
          //Get data
          this.parametre = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(628), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Dowmload recyclers map
  onGetParametreFile() {
    this.downloadService.download(DocumentType.Parametre, this.parametre.RefParametre.toString(), "", null, "")
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        console.log(error);
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  downloadFile(data: any, fileName: string) {
    const blob = new Blob([data]);
    {
      const url = window.URL.createObjectURL(blob);
      const a: HTMLAnchorElement = document.createElement("a") as HTMLAnchorElement;

      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    }
  }
}
