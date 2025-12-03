import { ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from "@angular/core";
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
import { ComponentName, DocumentType, HabilitationAnnuaire, HabilitationLogistique, ModuleName, RefDocumentType } from "../../../globals/enums";
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
  documentTypeList: dataModelsInterfaces.DocumentType[];
  paramEmailList: dataModelsInterfaces.ParamEmailList[];
  yearList: appInterfaces.Year[];
  email: dataModelsInterfaces.Email = {} as dataModelsInterfaces.Email;
  yearListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  dFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  documentTypeListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  paramEmailListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  emailHTMLBodyFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
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
  RefDocumentType = RefDocumentType;
  refDocumentTypeLabel: string = "";
  showGrid: boolean = false;
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
    , private changeDetectorRef: ChangeDetectorRef
  ) {
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      DocumentTypeList: this.documentTypeListFC,
      D: this.dFC,
      YearList: this.yearListFC,
      ParamEmailList: this.paramEmailListFC,
      EmailHTMLBody: this.emailHTMLBodyFC
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    //Get existing DocumentType
    this.listService.getList<dataModelsInterfaces.DocumentType>(ComponentName.DocumentType).subscribe(result => {
      this.documentTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
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
    this.documentTypeLabel = this.documentTypeListFC.value?.Libelle;
    //Global lock
    if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Administrateur) {
      this.lockScreen();
    }
    else {
      //Init
      this.unlockScreen();
      if (this.documentTypeListFC.value?.RefDocumentType != RefDocumentType.IncitationQualite
        && this.documentTypeListFC.value?.RefDocumentType != RefDocumentType.ReportingCollectiviteElu
        && this.documentTypeListFC.value?.RefDocumentType != RefDocumentType.ReportingCollectiviteGrandPublic
      ) { this.yearListFC.disable(); }
      if (this.documentTypeListFC.value?.RefDocumentType != RefDocumentType.EmailNoteCreditCollectivite) { this.dFC.disable(); }
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
    //Init DocumenType
    let refDocumentType: number = this.documentTypeListFC.value?.RefDocumentType;
    if (!!this.dFC.value && refDocumentType == RefDocumentType.EmailNoteCreditCollectivite) {
      //Get email template
      this.dataModelService.getEmailTemplate(RefDocumentType[refDocumentType], null, this.dFC.value)
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
    //Init DocumenType
    let refDocumentType: number = this.documentTypeListFC.value?.RefDocumentType;
    if (!!this.yearListFC.value
      && (refDocumentType == RefDocumentType.ReportingCollectiviteElu
        || refDocumentType == RefDocumentType.ReportingCollectiviteGrandPublic
        || refDocumentType == RefDocumentType.IncitationQualite)
    ) {
      //Get email template
      this.dataModelService.getEmailTemplate(RefDocumentType[refDocumentType], this.yearListFC.value, null)
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
  //On DocumentType selected
  onDocumentTypeSelected(documentType: dataModelsInterfaces.DocumentType) {
    //Init
    if (!documentType) {
      this.documentTypeListFC.setValue(null);
    }
    this.email = {} as dataModelsInterfaces.Email;
    this.emailHTMLBodyFC.setValue(null);
    this.dFC.setValue(null);
    this.yearListFC.setValue(null);
    //Init child GridSimple
    this.refDocumentTypeLabel = "";
    this.changeDetectorRef.detectChanges();
    //Init DocumenType
    let refDocumentType: number = this.documentTypeListFC.value?.RefDocumentType;
    this.refDocumentTypeLabel = refDocumentType ? RefDocumentType[refDocumentType] : "";
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Get RefDocumentType label from DocumentType
  getRefDocumentTypeLabel(): string {
    let refDocumentType: number = this.documentTypeListFC.value?.RefDocumentType;
    return refDocumentType ? RefDocumentType[refDocumentType] : "";
  }
  //-----------------------------------------------------------------------------------
  //Ask for confirmation
  onSave() {
    this.saveData();
    //Confirm
    if (this.documentTypeListFC.value?.RefDocumentType == RefDocumentType.IncitationQualite) {
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
    if (this.documentTypeListFC.value?.RefDocumentType == RefDocumentType.EmailNoteCreditCollectivite
      || this.documentTypeListFC.value?.RefDocumentType == RefDocumentType.ReportingCollectiviteElu
      || this.documentTypeListFC.value?.RefDocumentType == RefDocumentType.ReportingCollectiviteGrandPublic
    ) {
      //Create message
      let msg = this.applicationUserContext.getCulturedRessourceText(1617) + this.documentTypeListFC.value.Libelle;
      //Open confirm dialog
      const dialogRef = this.dialog.open(ConfirmComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: msg },
        autoFocus: false,
        restoreFocus: false
      });
      //After closed
      dialogRef.afterClosed().subscribe(result => {
        if (result === "yes") {
          this.onEmailing();
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
          .set("emailing", RefDocumentType[RefDocumentType.IncitationQualite])
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
  onEmailing() {
    //Check if there is data to process
    //Create e-mails
    var url = this.baseUrl + "evapi/email";
    this.progressBar.start();
    this.http
      .post<number>(url, this.email, {
        headers: new HttpHeaders()
          .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString())
          .set("d", !this.dFC.value ? "" : moment(this.dFC.value).format("YYYY-MM-DDT00:00:00.000"))
          .set("emailing", RefDocumentType[this.documentTypeListFC.value.RefDocumentType])
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
