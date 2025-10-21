import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { ApplicationUserContext } from "../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../services/list.service";
import { DownloadService } from "../../services/download.service";
import * as dataModelsInterfaces from "../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../interfaces/appInterfaces";
import { ConfirmComponent } from "../dialogs/confirm/confirm.component";
import { DataModelService } from "../../services/data-model.service";
import moment from "moment";
import { UploadComponent } from "../dialogs/upload/upload.component";
import { getAttachmentFilename, getCreationModificationTooltipText } from "../../globals/utils";
import { TicketsComponent } from "../dialogs/ticket/tickets.component";
import { showErrorToUser } from "../../globals/utils";
import { SnackBarQueueService } from "../../services/snackbar-queue.service";
import { kendoFontData } from "../../globals/kendo-utils";
import { SnackbarMsgType } from "../../globals/enums";

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
    selector: "email",
    templateUrl: "./email.component.html",
    standalone: false
})

export class EmailComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  email: dataModelsInterfaces.Email;
  paramEmailList: dataModelsInterfaces.ParamEmailList[];
  form: UntypedFormGroup;
  paramEmailListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  emailFromFC: UntypedFormControl = new UntypedFormControl(null);
  emailToFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  emailSubjectFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  emailHTMLBodyFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = false;
  //Misc
  title: string = this.applicationUserContext.getCulturedRessourceText(419);
  lastUploadResut: appInterfaces.UploadResponseBody;
  public kendoFontData = kendoFontData;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private downloadService: DownloadService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog
    , public dialogRef: MatDialogRef<EmailComponent>
    , @Inject(MAT_DIALOG_DATA) public data: any
    ) {
    this.email = {} as dataModelsInterfaces.Email;
    // create an empty object from the Email interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      ParamEmailList: this.paramEmailListFC,
      EmailFrom: this.emailFromFC,
      EmailTo: this.emailToFC,
      EmailSubject: this.emailSubjectFC,
      EmailHTMLBody: this.emailHTMLBodyFC
    });
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //Get existing Pays
    this.listService.getListParamEmail(null, true, true).subscribe(result => {
      this.paramEmailList = result;
      //Auto select
      if (this.paramEmailList.length === 1) { this.paramEmailListFC.setValue(this.paramEmailList[0].RefParamEmail); }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get Email
    if (this.data.refEmail) {
      var url = this.baseUrl + "evapi/email/" + this.data.refEmail.toString();
      this.http.get<dataModelsInterfaces.Email>(url).subscribe(result => {
        this.email = result;
        //Format dates
        this.dataModelService.setMomentFromTextEmail(this.email);
        //Update the form
        this.updateForm();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.paramEmailListFC.setValue(this.email.ParamEmail.RefParamEmail);
    this.emailFromFC.setValue(this.email.EmailFrom);
    this.emailToFC.setValue(this.email.EmailTo);
    this.emailSubjectFC.setValue(this.email.EmailSubject);
    this.emailHTMLBodyFC.setValue(this.email.EmailHTMLBody);
    this.title = this.applicationUserContext.getCulturedRessourceText(419);
    if (this.email.DReception != null) {
      this.title += " " + this.applicationUserContext.getCulturedRessourceText(671) + " " + moment(this.email.DReception).format("L") + " " + moment(this.email.DReception).format("LT");
    }
    else if (this.email.DEnvoi != null) {
      this.title += " " + this.applicationUserContext.getCulturedRessourceText(672) + " " + moment(this.email.DEnvoi).format("L") + " " + moment(this.email.DEnvoi).format("LT");
    }
    else {
      this.title += " " + this.applicationUserContext.getCulturedRessourceText(673);
    }
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Global lock
    if (this.email.DEnvoi || this.email.DReception) {
      this.lockScreen();
    }
    else {
      //Init
      this.unlockScreen();
    }
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.paramEmailListFC.disable();
    this.emailFromFC.disable();
    this.emailToFC.disable();
    this.emailSubjectFC.disable();
    this.emailHTMLBodyFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.paramEmailListFC.enable();
    this.emailFromFC.disable();
    this.emailToFC.enable();
    this.emailSubjectFC.enable();
    this.emailHTMLBodyFC.enable();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.email.ParamEmail = this.paramEmailList.find(x => x.RefParamEmail === this.paramEmailListFC.value);
    this.email.EmailFrom = this.emailFromFC.value;
    this.email.EmailTo = this.emailToFC.value;
    this.email.EmailSubject = this.emailSubjectFC.value;
    this.email.EmailHTMLBody = this.emailHTMLBodyFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(send: boolean) {
    //Lock screen if send
    this.lockScreen();
    this.title = this.applicationUserContext.getCulturedRessourceText(681);
    //Save
    this.saveData();
    var url = this.baseUrl + "evapi/email";
    //Format dates
    this.dataModelService.setTextFromMomentEmail(this.email);
    //Send data
    this.http
      .post<dataModelsInterfaces.Email>(url, this.email, {
        headers: new HttpHeaders()
          .set("send", send ? "true" : "false")
      })
      .subscribe(result => {
        this.email = result;
        this.snackBarQueueService.addMessage({ text: send ? this.applicationUserContext.getCulturedRessourceText(680) : this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Emit event for dialog closing
        return this.dialogRef.close({ type: this.data.type, ref: this.data.refEmail });
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(674) },
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        var url = this.baseUrl + "evapi/email/" + this.email.RefEmail;
        this.http
          .delete(url, {
            headers: new HttpHeaders()
              .set("deleteAction", "true")
          })
          .subscribe(result => {
            //Emit event for dialog closing
            this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(675), duration: 4000 } as appInterfaces.SnackbarMsg);
            return this.dialogRef.close({ refEmail: this.data.refEmail });
          }, error => {
            showErrorToUser(this.dialog, error, this.applicationUserContext);
          });
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Delete a CommandeFournisseurFichier
  onDeleteEmailFichier(i: number) {
    if (i !== null) {
      //Save data
      this.saveData();
      //Process
      this.email.EmailFichiers.splice(i, 1);
      //Update form
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  onUploadFile() {
    //Save data
    this.saveData();
    var url = this.baseUrl + "evapi/email";
    this.http
      .post<dataModelsInterfaces.Email>(url, this.email, {
        headers: new HttpHeaders()
          .set("send", "false")
      })
      .subscribe(result => {
        this.email = result;
        //open pop-up
        const dialogRef = this.dialog.open(UploadComponent, {
          width: "50%", height: "50%",
          data: {
            title: this.applicationUserContext.getCulturedRessourceText(626), message: this.applicationUserContext.getCulturedRessourceText(627)
            , multiple: false, type: "email", fileType: 0, ref: this.email.RefEmail
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
            this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(342), duration: 4000, type: SnackbarMsgType.Error } as appInterfaces.SnackbarMsg);
          }
          else {
            //Reload data
            var url = this.baseUrl + "evapi/email/" + this.email.RefEmail;
            this.http.get<dataModelsInterfaces.Email>(url).subscribe(result => {
              //Get data
              this.email = result;
              //Update form
              this.updateForm();
              //Inform user
              this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(628), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          }
        });
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Get file
  getFile(fileType: string, fileRef: number) {
    this.downloadService.download(fileType, fileRef.toString(), "", this.email.RefEmail, "")
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  downloadFile(data: any, fileName: string) {
    const blob = new Blob([data]);
    const url = window.URL.createObjectURL(blob);
    const a: HTMLAnchorElement = document.createElement("a") as HTMLAnchorElement;

    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();

    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for tickets
  onTickets() {
    //Save data
    this.saveData();
    //open pop-up
    const dialogRef = this.dialog.open(TicketsComponent, {
      width: "50%",
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (!this.email.EmailHTMLBody) { this.email.EmailHTMLBody = ""; }
        let res = "<div style=\"font-family:Gill Sans MT; font-size:11pt;\">" + result.ticketTexte + "</div>";
        this.email.EmailHTMLBody += res;
        //this.email.EmailHTMLBody += result.ticketTexte;
        this.updateForm();
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.email);
  }
}
