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
import { UtilsService } from "../../../services/utils.service";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { cmp, getAttachmentFilename, getCreationModificationTooltipText, showConfirmToUser, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { ListService } from "../../../services/list.service";
import moment from "moment";
import { UploadComponent } from "../../dialogs/upload/upload.component";
import { DownloadService } from "../../../services/download.service";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import { DomSanitizer } from "@angular/platform-browser";
import { BaseFormComponent } from "../../_ancestors/base-form.component";
import { SnackbarMsgType } from "../../../globals/enums";

@Component({
    selector: "document",
    templateUrl: "./document.component.html",
    styleUrls: ["./document.component.scss"],
    standalone: false
})

export class DocumentComponent extends BaseFormComponent<dataModelsInterfaces.Document> {
  sAGECodeTransportList: dataModelsInterfaces.SAGECodeTransport[] = [];
  documentEntiteList: dataModelsInterfaces.DocumentEntite[] = [];
  documentEntiteTypeList: dataModelsInterfaces.DocumentEntiteType[] = [];
  //Form
  form: UntypedFormGroup;
  document: dataModelsInterfaces.Document = { Actif: true } as dataModelsInterfaces.Document;
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  visibiliteTotaleFC: UntypedFormControl = new UntypedFormControl(null);
  libelleFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  descriptionFC: UntypedFormControl = new UntypedFormControl(null);
  dDebutFC: UntypedFormControl = new UntypedFormControl(null);
  dFinFC: UntypedFormControl = new UntypedFormControl(null);
  entiteTypeListFC: UntypedFormControl = new UntypedFormControl(null);
  //Misc
  lastUploadResut: appInterfaces.UploadResponseBody;
  //Constructor
  constructor(protected activatedRoute: ActivatedRoute
    , protected router: Router
    , private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected downloadService: DownloadService
    , protected utilsService: UtilsService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
    , protected sanitizer: DomSanitizer) {
    super("DocumentComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: this.actifFC,
      VisibiliteTotale: this.visibiliteTotaleFC,
      Libelle: this.libelleFC,
      Description: this.descriptionFC,
      DDebut: this.dDebutFC,
      DFin: this.dFinFC,
      EntiteTypeList: this.entiteTypeListFC,
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    //Get existing Standard
    this.listService.getListEntiteType().subscribe(result => {
      this.documentEntiteTypeList = result.map(item => ({ EntiteType: item } as dataModelsInterfaces.DocumentEntiteType));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getDataModel<dataModelsInterfaces.Document>(id, this.componentName).subscribe(result => {
          //Get data
          this.document = result;
          this.sourceObj = result;
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
    this.actifFC.setValue(this.document.Actif);
    this.visibiliteTotaleFC.setValue(this.document.VisibiliteTotale);
    this.libelleFC.setValue(this.document.Libelle);
    this.descriptionFC.setValue(this.document.Description);
    this.dDebutFC.setValue(this.document.DDebut);
    this.dFinFC.setValue(this.document.DFin);
    this.entiteTypeListFC.setValue(this.document.DocumentEntiteTypes);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.document.Actif = this.actifFC.value ? true : false;
    this.document.VisibiliteTotale = this.visibiliteTotaleFC.value ? true : false;
    this.document.Libelle = this.libelleFC.value;
    this.document.Description = this.descriptionFC.value;
    this.document.DDebut = (this.dDebutFC.value == null ? null : moment(this.dDebutFC.value));
    this.document.DFin = (this.dFinFC.value == null ? null : moment(this.dFinFC.value));
    this.document.DocumentEntiteTypes = this.entiteTypeListFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(next: string) {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<dataModelsInterfaces.Document>(this.document, this.componentName)
      .subscribe(result => {
        //Reload data
        this.document = result;
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
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
    this.dataModelService.deleteDataModel<dataModelsInterfaces.Document>(id, this.componentName)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(this.ressAfterDel), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
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
        , multiple: false, type: "document", fileType: 0, ref: this.document.RefDocument.toString()
      },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      this.lastUploadResut = result;
      if (!this.lastUploadResut) {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(341), duration: 4000, type: SnackbarMsgType.Error } as appInterfaces.SnackbarMsg);
      }
      else if (this.lastUploadResut.error != "") {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(631), duration: 4000, type: SnackbarMsgType.Error } as appInterfaces.SnackbarMsg);
      }
      else {
        //Reload data
        this.dataModelService.getDataModel<dataModelsInterfaces.Document>(this.document.RefDocument, "DocumentComponent").subscribe(result => {
          //Get data
          this.document = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(628), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Multi select
  compareFnEntiteType(item: dataModelsInterfaces.DocumentEntiteType, selectedItem: dataModelsInterfaces.DocumentEntiteType) {
    return item && selectedItem ? item.EntiteType.RefEntiteType === selectedItem.EntiteType.RefEntiteType : item === selectedItem;
  }
  public onEntiteTypeListReset() {
    this.document.DocumentEntiteTypes = [];
    this.entiteTypeListFC.setValue(this.document.DocumentEntiteTypes);
  }
  //-----------------------------------------------------------------------------------
  //Create EntiteTypes label
  entiteTypesLabel(): string {
    let s = this.entiteTypeListFC.value?.sort(function (a, b) { return cmp(a.EntiteType.Libelle, b.EntiteType.Libelle); })
      .map(item => item.EntiteType.Libelle).join("\n");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Download a file
  getFile(fileType: string, refDocument: number) {
    this.downloadService.download(fileType, refDocument.toString(), "", null, "")
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
  //-----------------------------------------------------------------------------------
  //Show elements details
  addDocumentEntite() {
    let data: any;
    let title: string = "";
    //Init
    this.saveData();
    //Title
    title = this.applicationUserContext.getCulturedRessourceText(1124);
    //Calculate id
    data = {
      title: title, type: "EntiteForDocument"
    };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "50%",
      data,
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      //If data in the dialog have been saved
      if (result) {
        let exists: boolean = false;
        exists = this.document.DocumentEntites?.some(item => item.RefEntite == result.ref);
        if (!exists) {
          this.dataModelService.getEntite(result.ref, null, null).subscribe(result => {
            //Update data
            let entiteList: dataModelsInterfaces.EntiteList = result;
            let dE: dataModelsInterfaces.DocumentEntite={
              RefDocument : this.document.RefDocument,
              Entite : entiteList,
              RefEntite : entiteList.RefEntite
            }
            this.document.DocumentEntites.push(dE);
            this.updateForm();
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
      }
    })
  }
  //-----------------------------------------------------------------------------------
  //Delete a DocumentEntite 
  onDeleteDocumentEntite(refEntite: number) {
    if (refEntite !== null) {
      let i: number = this.document.DocumentEntites.map(e => e.RefEntite).indexOf(refEntite);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1162), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Save data
            this.saveData();
            //Remove EntiteEntite
            this.document.DocumentEntites.splice(i, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
}

