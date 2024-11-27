import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import moment from "moment";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { getAttachmentFilename } from "../../../globals/utils";
import { showErrorToUser } from "../../../globals/utils";
import { HabilitationLogistique } from "../../../globals/enums";

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
    selector: "export-sage",
    templateUrl: "./export-sage.component.html",
    standalone: false
})

export class ExportSAGEComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  exportSAGETypeListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  monthListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  quarterListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  yearListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  monthList: appInterfaces.Month[];
  quarterList: appInterfaces.Quarter[];
  yearList: appInterfaces.Year[];
  exportSAGETypeList: any[] = [
    { t: "AchatVente", name: this.applicationUserContext.getCulturedRessourceText(698) }
    , { t: "NoteCredit", name: this.applicationUserContext.getCulturedRessourceText(699) }
    , { t: "IncitationQualite", name: this.applicationUserContext.getCulturedRessourceText(700) }]
  //Global lock
  locked: boolean = true;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , public dialog: MatDialog) {
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      ExportSAGETypeList: this.exportSAGETypeListFC,
      MonthList: this.monthListFC,
      QuarterList: this.quarterListFC,
      YearList: this.yearListFC
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    //Load initial data
    //Get existing month
    this.listService.getListMonth().subscribe(result => {
      this.monthList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing quarter
    this.listService.getListQuarter().subscribe(result => {
      this.quarterList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing year
    this.listService.getListYear().subscribe(result => {
      this.yearList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.exportSAGETypeListFC.setValue("AchatVente");
    this.manageScreen();
    this.form.updateValueAndValidity();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Global lock
    if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Administrateur) {
      this.lockScreen();
    }
    else {
      //Init
      this.unlockScreen();
      this.monthListFC.disable();
      this.quarterListFC.disable();
      switch (this.exportSAGETypeListFC.value) {
        case "AchatVente":
          this.monthListFC.enable();
          break;
        case "NoteCredit":
          this.quarterListFC.enable();
          break;
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.exportSAGETypeListFC.disable();
    this.quarterListFC.disable();
    this.yearListFC.disable();
    this.monthListFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.exportSAGETypeListFC.enable();
    this.quarterListFC.enable();
    this.yearListFC.enable();
    this.monthListFC.enable();
  }
  //-----------------------------------------------------------------------------------
  //ExportSAGEType selected
  onExportSAGETypeSelected() {
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Ask for export file
  onSave() {
    //Achat/Vente
    if (this.exportSAGETypeListFC.value === "AchatVente") {
    //Confirm
      const dialogRef = this.dialog.open(ConfirmComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(893) },
        autoFocus: false,
        restoreFocus: false
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result === "yes") {
          this.onAchatVente();
        }
      });
    }
    else {
      switch (this.exportSAGETypeListFC.value) {
        case "NoteCredit":
          //Collectivités
          var url = this.baseUrl + "evapi/exportsage/collectivite";
          this.http
            .get(url, {
              headers: new HttpHeaders()
                .set("month", this.monthListFC.value == null ? "0" : (moment(this.monthListFC.value).month() + 1).toString())
                .set("quarter", this.quarterListFC.value == null ? "0" : this.quarterListFC.value.toString())
                .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
              observe: "response",
              responseType: "blob"
            })
            .subscribe((data: HttpResponse<Blob>) => {
              let contentDispositionHeader = data.headers.get("Content-Disposition");
              let fileName = getAttachmentFilename(contentDispositionHeader);
              this.downloadFile(data.body, fileName);
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Notes de crédit
          var url = this.baseUrl + "evapi/exportsage/notecredit";
          this.http
            .get(url, {
              headers: new HttpHeaders()
                .set("month", this.monthListFC.value == null ? "0" : (moment(this.monthListFC.value).month() + 1).toString())
                .set("quarter", this.quarterListFC.value == null ? "0" : this.quarterListFC.value.toString())
                .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
              observe: "response",
              responseType: "blob"
            })
            .subscribe((data: HttpResponse<Blob>) => {
              let contentDispositionHeader = data.headers.get("Content-Disposition");
              let fileName = getAttachmentFilename(contentDispositionHeader);
              this.downloadFile(data.body, fileName);
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          break;
        case "IncitationQualite":
          //Centres de tri
          var url = this.baseUrl + "evapi/exportsage/centredetri";
          this.http
            .get(url, {
              headers: new HttpHeaders()
                .set("month", this.monthListFC.value == null ? "0" : (moment(this.monthListFC.value).month() + 1).toString())
                .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
              observe: "response",
              responseType: "blob"
            })
            .subscribe((data: HttpResponse<Blob>) => {
              let contentDispositionHeader = data.headers.get("Content-Disposition");
              let fileName = getAttachmentFilename(contentDispositionHeader);
              this.downloadFile(data.body, fileName);
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Incitation qualité
          var url = this.baseUrl + "evapi/exportsage/notecreditincitationqualite";
          this.http
            .get(url, {
              headers: new HttpHeaders()
                .set("month", this.monthListFC.value == null ? "0" : (moment(this.monthListFC.value).month() + 1).toString())
                .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
              observe: "response",
              responseType: "blob"
            })
            .subscribe((data: HttpResponse<Blob>) => {
              let contentDispositionHeader = data.headers.get("Content-Disposition");
              let fileName = getAttachmentFilename(contentDispositionHeader);
              this.downloadFile(data.body, fileName);
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          break;
      }
    }
  }
  onAchatVente() {
    //Check if there is data to process
    var url = this.baseUrl + "evapi/exportsage/setachatventefilter";
    this.http
      .get(url, {
        headers: new HttpHeaders()
          .set("month", this.monthListFC.value == null ? "0" : (moment(this.monthListFC.value).month() + 1).toString())
          .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
      })
      .subscribe(result => {
        //Achat
        var url = this.baseUrl + "evapi/exportsage/achat";
        this.http
          .get(url, {
            headers: new HttpHeaders()
              .set("month", this.monthListFC.value == null ? "0" : (moment(this.monthListFC.value).month() + 1).toString())
              .set("quarter", this.quarterListFC.value == null ? "0" : this.quarterListFC.value.toString())
              .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
            observe: "response",
            responseType: "blob"
          })
          .subscribe((data: HttpResponse<Blob>) => {
            let contentDispositionHeader = data.headers.get("Content-Disposition");
            let fileName = getAttachmentFilename(contentDispositionHeader);
            this.downloadFile(data.body, fileName);
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Vente
        var url = this.baseUrl + "evapi/exportsage/vente";
        this.http
          .get(url, {
            headers: new HttpHeaders()
              .set("month", this.monthListFC.value == null ? "0" : (moment(this.monthListFC.value).month() + 1).toString())
              .set("quarter", this.quarterListFC.value == null ? "0" : this.quarterListFC.value.toString())
              .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
            observe: "response",
            responseType: "blob"
          })
          .subscribe((data: HttpResponse<Blob>) => {
            let contentDispositionHeader = data.headers.get("Content-Disposition");
            let fileName = getAttachmentFilename(contentDispositionHeader);
            this.downloadFile(data.body, fileName);
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  downloadFile(data: any, fileName: string) {
    //If applicable
    if (fileName !== "nothing") {
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
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["home"]);
  }
}
