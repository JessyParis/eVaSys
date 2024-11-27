import { Component, Inject, OnInit, ViewChild } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher, ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import moment from "moment";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { showErrorToUser } from "../../../globals/utils";
import { HabilitationAnnuaire, HabilitationLogistique, ModuleName } from "../../../globals/enums";
import { DataModelService } from '../../../services/data-model.service';
import { SnackBarQueueService } from '../../../services/snackbar-queue.service';
import { GridSimpleComponent } from '../../global/grid-simple/grid-simple.component';
import { finalize } from "rxjs/operators";
import { ProgressBarComponent } from "../../global/progress-bar/progress-bar.component";
import { ProgressBarMode } from "@angular/material/progress-bar";
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
    selector: "envoi-email",
    templateUrl: "./envoi-email.component.html",
    standalone: false
})

export class EnvoiEmailComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  paramEmailList: dataModelsInterfaces.ParamEmailList[];
  yearList: appInterfaces.Year[];
  email: dataModelsInterfaces.Email = {} as dataModelsInterfaces.Email;
  yearListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  dFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  paramEmailListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  emailHTMLBodyFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  documentType: string = "";
  dataYear: number;
  //Global lock
  locked: boolean = true;
  //Loading indicator
  loading = false;
  color = "warn" as ThemePalette;
  mode = "indeterminate" as ProgressBarMode;
  value = 50;
  displayProgressBar = false;
  //Misc
  documentTypeLabel: string = "";
  public kendoFontData = kendoFontData;
  //Children
  @ViewChild(GridSimpleComponent) gridSimple: GridSimpleComponent;
  @ViewChild(ProgressBarComponent) progressBar: ProgressBarComponent;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , public dialog: MatDialog
    , private snackBarQueueService: SnackBarQueueService
  ) {
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      D: this.dFC,
      YearList: this.yearListFC,
      ParamEmailList: this.paramEmailListFC,
      EmailHTMLBody: this.emailHTMLBodyFC
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    this.documentType = this.activatedRoute.snapshot.params["documentType"];
    //Get existing ParamEmail
    this.listService.getListParamEmail(null, true, false).subscribe(result => {
      this.paramEmailList = result;
      //Auto select
      if (this.paramEmailList.length === 1) {
        this.paramEmailListFC.setValue(this.paramEmailList[0].RefParamEmail);
      }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing year
    this.listService.getListYear().subscribe(result => {
      this.yearList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    if (this.documentType == "IncitationQualite") { this.documentTypeLabel = this.applicationUserContext.getCulturedRessourceText(700); }
    if (this.documentType == "EmailNoteCreditCollectivite") { this.documentTypeLabel = this.applicationUserContext.getCulturedRessourceText(699); }
    //Global lock
    if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Administrateur) {
      this.lockScreen();
    }
    else {
      //Init
      this.unlockScreen();
      if (this.documentType != "IncitationQualite") { this.yearListFC.disable(); }
      if (this.documentType != "EmailNoteCreditCollectivite") { this.dFC.disable(); }
    }
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.dFC.disable();
    this.yearListFC.disable();
    this.paramEmailListFC.disable();
    this.emailHTMLBodyFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.dFC.enable();
    this.yearListFC.enable();
    this.paramEmailListFC.enable();
    this.emailHTMLBodyFC.enable();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.email.ParamEmail = this.paramEmailList.find(x => x.RefParamEmail === this.paramEmailListFC.value);
    this.email.EmailHTMLBody = this.emailHTMLBodyFC.value;
  }
  //-----------------------------------------------------------------------------------
  //D change
  onDDateChange(d: moment.Moment) {
    this.emailHTMLBodyFC.setValue(null);
    if (!!this.dFC.value) {
      //Get email template
      this.dataModelService.getEmailTemplate(this.documentType, null, this.dFC.value)
        .subscribe(result => {
          this.email = result;
          this.emailHTMLBodyFC.setValue(this.email.EmailHTMLBody);
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      this.email = {} as dataModelsInterfaces.Email;
    }
    //Get data
    this.gridSimple.filterD = this.dFC.value;
    this.gridSimple.loadPage("");
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Year selected
  onYearSelected(year: number) {
    this.emailHTMLBodyFC.setValue(null);
    if (!!this.yearListFC.value) {
      //Get email template
      this.dataModelService.getEmailTemplate(this.documentType, this.yearListFC.value, null)
        .subscribe(result => {
          this.email = result;
          this.emailHTMLBodyFC.setValue(this.email.EmailHTMLBody);
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      this.email = {} as dataModelsInterfaces.Email;
    }
    //Get data
    this.gridSimple.filterYear = this.yearListFC.value.toString();
    this.gridSimple.loadPage("");
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Ask for confirmation
  onSave() {
    this.saveData();
    //Confirm
    if (this.documentType == "IncitationQualite") {
      const dialogRef = this.dialog.open(ConfirmComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(1008) },
        autoFocus: false,
        restoreFocus: false
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result === "yes") {
          this.onIncitationQualite();
        }
      });
    }
    if (this.documentType == "EmailNoteCreditCollectivite") {
      const dialogRef = this.dialog.open(ConfirmComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(1082) },
        autoFocus: false,
        restoreFocus: false
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result === "yes") {
          this.onEmailNoteCreditCollectivite();
        }
      });
    }
  }
  onIncitationQualite() {
    //Check if there is data to process
    //Create e-mails
    var url = this.baseUrl + "evapi/email";
    this.progressBar.start();
    this.http
      .post<number>(url, this.email, {
        headers: new HttpHeaders()
          .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString())
          .set("emailing", this.documentType)
      })
      .pipe(
        finalize(() => this.progressBar.stop())
      )
      .subscribe((result: number) => {
        //Re-load
        this.gridSimple.loadPage("");
        //Inform user
        this.snackBarQueueService.addMessage({ text: result + " " + this.applicationUserContext.getCulturedRessourceText(1061), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Edit courriers de non versement
        this.progressBar.start();
        url = this.baseUrl + "evapi/documentedit/incitationqualitecourriernonversement";
        this.http
          .get(url, {
            headers: new HttpHeaders()
              .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString())
          })
          .pipe(
            finalize(() => this.progressBar.stop())
          )
          .subscribe((result: number) => {
            //Inform user
            this.snackBarQueueService.addMessage({ text: result + " " + this.applicationUserContext.getCulturedRessourceText(1062), duration: 4000 } as appInterfaces.SnackbarMsg);
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  onEmailNoteCreditCollectivite() {
    //Check if there is data to process
    //Create e-mails
    var url = this.baseUrl + "evapi/email";
    this.progressBar.start();
    this.http
      .post<number>(url, this.email, {
        headers: new HttpHeaders()
          .set("d", !this.dFC.value ? "" : moment(this.dFC.value).format("YYYY-MM-DDT00:00:00.000"))
          .set("emailing", this.documentType)
      })
      .pipe(
        finalize(() => this.progressBar.stop())
      )
      .subscribe((result: number) => {
        //Re-load
        this.gridSimple.loadPage("");
        //Inform user
        this.snackBarQueueService.addMessage({ text: result + " " + this.applicationUserContext.getCulturedRessourceText(1061), duration: 4000 } as appInterfaces.SnackbarMsg);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  downloadFile(data: any, fileName: string) {
    //If applicable
    if (fileName !== "nothing") {
      const blob = new Blob([data]);
      //if (window.navigator.msSaveOrOpenBlob) //IE & Edge
      //{
      //  //msSaveBlob only available for IE & Edge
      //  window.navigator.msSaveBlob(blob, fileName);
      //}
      //else //Chrome & FF
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
}
