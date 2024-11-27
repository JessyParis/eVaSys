/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/12/2018
/// ----------------------------------------------------------------------------------------------------- 
import { Component, OnInit, ViewChild, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { forkJoin, Subject } from "rxjs";
import { UploadService } from "../../../services/upload.service";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ApplicationUserContext } from "../../../globals/globals";
import { ActionFichier, ActionFichierNoFile, DocumentEntite, DocumentNoFile } from "../../../interfaces/dataModelsInterfaces";

@Component({
    selector: "upload",
    templateUrl: "./upload.component.html",
    standalone: false
})
export class UploadComponent implements OnInit {
  @ViewChild("file") file: any;
  public files: Set<File> = new Set();
  //Progress bar
  color = "warn";
  mode = "indeterminate";
  value = 50;
  displayProgressBar = false;
  /** Subject that emits when the component has been destroyed. */
  protected _onDestroy = new Subject<void>();
  //Constructor
  constructor(public dialogRef: MatDialogRef<UploadComponent>, @Inject(MAT_DIALOG_DATA) public data: any
    , public uploadService: UploadService
    , public applicationUserContext: ApplicationUserContext) { }

  ngOnInit() {
  }

  progress: any;
  uploadBodyResult: any;
  uploadedFiles: any = [];
  canBeClosed = true;
  primaryButtonText = this.applicationUserContext.getCulturedRessourceText(332);
  addFilesButtonText = (this.data.multiple ? this.applicationUserContext.getCulturedRessourceText(633) : this.applicationUserContext.getCulturedRessourceText(797));
  showCancelButton = true;
  uploading = false;
  uploadSuccessful = false;
  lastUploadResut: appInterfaces.UploadResponseBody;

  onFilesAdded() {
    const files: { [key: string]: File } = this.file.nativeElement.files;
    for (let key in files) {
      if (!isNaN(parseInt(key))) {
        this.files.add(files[key]);
      }
    }
  }

  addFiles() {
    this.file.nativeElement.click();
  }

  closeDialog() {
    // if everything was uploaded already, just close the dialog
    // , and send back the last message and all uploaded files
    if (this.uploadSuccessful) {
      this.lastUploadResut.uploadedFiles = this.uploadedFiles;
      return this.dialogRef.close(this.lastUploadResut);
    }

    // set the component state to "uploading"
    this.uploading = true;
    // Display progress bar
    this.displayProgressBar = true;
    // start the upload and save the progress map
    let res: any = this.uploadService.upload(this.files, this.data.type, this.data.fileType, this.data.ref);
    this.progress = res.status;
    this.uploadBodyResult = res.res;

    //Store the upload result for the messageBox return data
    for (const key in this.uploadBodyResult) {
      this.uploadBodyResult[key].uploadBodyResult.subscribe((val: appInterfaces.UploadResponseBody) => {
        //Store last result
        this.lastUploadResut = val
        //Store each file to be sent back to the data model
        if (this.data.type == "actionfichier") {
          (this.uploadedFiles as ActionFichierNoFile[]).push({
            RefActionFichier: val.fileDbId,
            RefAction: this.data.ref,
            Nom: val.fileName
          } as ActionFichierNoFile)
        }
        if (this.data.type == "documententite") {
          (this.uploadedFiles as DocumentEntite[]).push({
            RefDocumentEntite: val.fileDbId,
            RefEntite: this.data.ref,
            DocumentNoFile: {
              Libelle: val.fileName,
              Nom: val.fileName,
              Actif: true,
              VisibiliteTotale: this.data.fileType
            } as DocumentNoFile
          } as DocumentEntite)
        }
      });
    }

    // convert the progress map into an array
    let allProgressObservables = [];
    for (let key in this.progress) {
      allProgressObservables.push(this.progress[key].progress);
    }

    // The OK-button should have the text "Finish" now
    this.primaryButtonText = this.applicationUserContext.getCulturedRessourceText(634);

    // The dialog should not be closed while uploading
    this.canBeClosed = false;
    this.dialogRef.disableClose = true;

    // Hide the cancel-button
    this.showCancelButton = false;

    // When all progress-observables are completed...
    forkJoin(allProgressObservables).subscribe(end => {
      //Stop progress bar
      this.displayProgressBar = false;
      //Close the dialog and send the result
      //return this.dialogRef.close("yes");

      //// ... the dialog can be closed again...
      this.canBeClosed = true;
      this.dialogRef.disableClose = false;

      //// ... the upload was successful...
      this.uploadSuccessful = true;

      //// ... and the component is no longer uploading
      this.uploading = false;
    });
  }
  //-----------------------------------------------------------------------------------
  ngOnDestroy() {
    //Stop progress bar
    this.displayProgressBar = false;
    //Search fields
    this._onDestroy.next();
    this._onDestroy.complete();
  }
}
