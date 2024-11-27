import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getAttachmentFilename, getCreationModificationTooltipText, showConfirmToUser, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import { ErrorStateMatcher } from "@angular/material/core";
import { UploadComponent } from "../../dialogs/upload/upload.component";
import { DownloadService } from "../../../services/download.service";
import { HttpResponse } from "@angular/common/http";
import { pdf } from "@progress/kendo-drawing";

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
    selector: "action",
    templateUrl: "./action.component.html",
    standalone: false
})

export class ActionComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  @Input() refAction: number;
  @Input() a: dataModelsInterfaces.Action;
  @Input() type: string;
  @Input() entite: dataModelsInterfaces.Entite;
  @Output() askClose = new EventEmitter<any>();
  form: UntypedFormGroup;
  action: dataModelsInterfaces.Action;
  formeContactList: dataModelsInterfaces.FormeContact[];
  actionActionTypeList: dataModelsInterfaces.ActionActionType[];
  contactAdresseList: dataModelsInterfaces.ContactAdresse[];
  //FormControls
  formeContactListFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  contactAdresseListFC: UntypedFormControl = new UntypedFormControl(null);
  dActionFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  actionActionTypeListFC: UntypedFormControl = new UntypedFormControl(null);
  libelleFC: UntypedFormControl = new UntypedFormControl(null);
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  //Global lock
  locked: boolean = true;
  //Misc
  lastUploadResut: appInterfaces.UploadResponseBody;
  //Constructor
  constructor(private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private listService: ListService
    , private downloadService: DownloadService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    // create an empty object from the Action interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      FormeContactList: this.formeContactListFC,
      ContactAdresseList: this.contactAdresseListFC,
      ActionActionTypeList: this.actionActionTypeListFC,
      DAction: this.dActionFC,
      Libelle: this.libelleFC,
      Cmt: this.cmtFC,
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
    this.action = { ...this.a };
    //Get existing FormeContact
    this.listService.getList<dataModelsInterfaces.FormeContact>("FormeContactComponent").subscribe(result => {
      this.formeContactList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ActionType
    this.listService.getListActionType(this.entite.EntiteType.RefEntiteType).subscribe(result => {
      this.actionActionTypeList = result.map(item => ({ ActionType: item } as dataModelsInterfaces.ActionActionType));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.updateForm();
    this.contactAdresseList = this.entite.ContactAdresses;
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.formeContactListFC.disable();
    this.contactAdresseListFC.disable();
    this.actionActionTypeListFC.disable();
    this.dActionFC.disable();
    this.libelleFC.disable();
    this.cmtFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.formeContactListFC.setValue(this.action.FormeContact?.RefFormeContact);
    this.contactAdresseListFC.setValue(this.action.ContactAdresse?.RefContactAdresse);
    this.actionActionTypeListFC.setValue(this.action.ActionActionTypes);
    this.dActionFC.setValue(this.action.DAction);
    this.libelleFC.setValue(this.action.Libelle);
    this.cmtFC.setValue(this.action.Cmt);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.action.FormeContact = this.formeContactList.find(x => x.RefFormeContact === this.formeContactListFC.value);
    this.action.ContactAdresse = this.contactAdresseList.find(x => x.RefContactAdresse === this.contactAdresseListFC.value);
    this.action.ActionActionTypes = this.actionActionTypeListFC.value;
    this.action.DAction = this.dActionFC.value;
    this.action.Libelle = this.libelleFC.value;
    this.action.Cmt = this.cmtFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Create ActionType label
  actionActionTypesLabel(): string {
    let s = this.actionActionTypeListFC.value?.map(item => item.ActionType.Libelle).join("\n");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.action);
  }
  //-----------------------------------------------------------------------------------
  //Multi select
  compareFnActionActionType(item: dataModelsInterfaces.ActionActionType, selectedItem: dataModelsInterfaces.ActionActionType) {
    return item && selectedItem ? item.ActionType.RefActionType === selectedItem.ActionType.RefActionType : item === selectedItem;
  }
  public onActionActionTypeListReset() {
    this.action.ActionActionTypes = [];
    this.actionActionTypeListFC.setValue(this.action.ActionActionTypes);
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(close:boolean) {
    this.applyChanges();
    //Save data to parent object and close
    if (close) {
      switch (this.type) {
        case "AnnuaireAction":
          //Save data
          this.a = { ...this.action };
          //Emit event for dialog closing
          this.askClose.emit({ type: this.type, ref: this.a });
          break;
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  onUploadFile() {
    //Save data
    this.applyChanges();
    //open pop-up
    this.startUploadFile();
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  startUploadFile() {
    //open pop-up
    const dialogRef = this.dialog.open(UploadComponent, {
      width: "50%", height: "50%",
      data: {
        title: this.applicationUserContext.getCulturedRessourceText(626), message: this.applicationUserContext.getCulturedRessourceText(627)
        , multiple: true, type: "actionfichier", fileType: "", ref: this.action.RefAction
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
        //Reload data, from database or from dialog depending on existence of Action in db or not
        if (this.action.RefAction > 0) {
          this.dataModelService.getActionFichierNoFiles(this.action.RefAction).subscribe(aFNFs => {
            //Get data
            this.action.ActionFichierNoFiles = aFNFs;
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        else {
          this.action.ActionFichierNoFiles = this.action.ActionFichierNoFiles.concat(this.lastUploadResut.uploadedFiles);
        }
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(628), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Download a file
  getFile(fileType: string, refActionFichier: number) {
    this.downloadService.download(fileType, refActionFichier.toString(), "", this.action.RefAction, "")
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
  //Delete a DocumentEntite
  onDeleteActionFichier(refActionFichier: number) {
    if (refActionFichier !== null) {
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1198), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            this.action.ActionFichierNoFiles = this.action.ActionFichierNoFiles.filter(e => e.RefActionFichier != refActionFichier);
          }
        });
    }
  }
}
