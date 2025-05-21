import { Component, Inject, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router, NavigationEnd } from "@angular/router";
import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher, ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import moment from "moment";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getAttachmentFilename } from "../../../globals/utils";
import { showErrorToUser } from "../../../globals/utils";
import { EntiteList } from "../../../interfaces/dataModelsInterfaces";
import { ReplaySubject, Subject } from "rxjs";
import { finalize, takeUntil } from "rxjs/operators";
import { ProgressBarComponent } from "../../global/progress-bar/progress-bar.component";
import { ProgressBarMode } from "@angular/material/progress-bar";

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
    selector: "document-edit",
    templateUrl: "./document-edit.component.html",
    standalone: false
})

export class DocumentEditComponent implements OnInit, OnDestroy {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  documentEditTypeListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  collectiviteListFC: UntypedFormControl = new UntypedFormControl(null);
  collectiviteListFilterFC: UntypedFormControl = new UntypedFormControl(null);
  centreDeTriListFC: UntypedFormControl = new UntypedFormControl(null);
  centreDeTriListFilterFC: UntypedFormControl = new UntypedFormControl(null);
  monthListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  quarterListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  yearListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  collectiviteList: EntiteList[];
  filteredCollectiviteList: ReplaySubject<EntiteList[]> = new ReplaySubject<EntiteList[]>(1);
  centreDeTriList: EntiteList[];
  filteredCentreDeTriList: ReplaySubject<EntiteList[]> = new ReplaySubject<EntiteList[]>(1);
  monthList: appInterfaces.Month[];
  quarterList: appInterfaces.Quarter[];
  yearList: appInterfaces.Year[];
  documentEditTypeList: any[] = [
    { t: "EtatTrimerstrielCollectiviteCS", name: this.applicationUserContext.getCulturedRessourceText(1361) }
    , { t: "EtatMensuelCollectiviteHCS", name: this.applicationUserContext.getCulturedRessourceText(1385) }
    , { t: "EtatMensuelReception", name: this.applicationUserContext.getCulturedRessourceText(1431) }
    , { t: "CertificatRecyclageCS", name: this.applicationUserContext.getCulturedRessourceText(1386) }
    , { t: "CertificatRecyclageHCS", name: this.applicationUserContext.getCulturedRessourceText(1387) }
  ]
  //Loading indicator
  loading = false;
  color = "warn" as ThemePalette;
  mode = "indeterminate" as ProgressBarMode;
  value = 50;
  displayProgressBar = false;
  /** Subject that emits when the component has been destroyed. */
  protected _onDestroy = new Subject<void>();
  @ViewChild(ProgressBarComponent) progressBar: ProgressBarComponent;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , public dialog: MatDialog) {
    // override the route reuse strategy
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    }
    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationEnd) {
        // trick the Router into believing it"s last link wasn"t previously loaded
        this.router.navigated = false;
      }
    });
    //Create form
    this.createForm();
    //this.form.updateValueAndValidity();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      DocumentEditTypeList: this.documentEditTypeListFC,
      CollectiviteList: this.collectiviteListFC,
      CollectiviteListFilter: this.collectiviteListFilterFC,
      CentreDeTriList: this.centreDeTriListFC,
      CentreDeTriListFilter: this.centreDeTriListFilterFC,
      MonthList: this.monthListFC,
      QuarterList: this.quarterListFC,
      YearList: this.yearListFC
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    //Load initial data
    //Get existing Collectivite
    this.listService.getListCollectivite(null, null, false, true, null, "", false).subscribe(result => {
      this.collectiviteList = result;
      this.filteredCollectiviteList.next(this.collectiviteList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing CentreDeTri
    this.listService.getListCentreDeTri(null, null, false).subscribe(result => {
      this.centreDeTriList = result;
      this.filteredCentreDeTriList.next(this.centreDeTriList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
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
    this.manageScreen();
    this.form.updateValueAndValidity();
    // listen for search field value changes
    this.collectiviteListFilterFC.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCollectivitesMulti();
      });
    // listen for search field value changes
    this.centreDeTriListFilterFC.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCentreDeTrisMulti();
      });
  }
  //-----------------------------------------------------------------------------------
  ngOnDestroy() {
    //Stop progress bar
    this.displayProgressBar = false;
    this.progressBar.stop();
    //Search fields
    this._onDestroy.next();
    this._onDestroy.complete();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    switch (this.documentEditTypeListFC.value) {
      case "EtatMensuelReception":
        this.collectiviteListFC.disable();
        this.quarterListFC.disable();
        this.centreDeTriListFC.enable()
        this.monthListFC.enable();
        break;
      case "EtatTrimerstrielCollectiviteCS":
      case "CertificatRecyclageCS":
      case "CertificatRecyclageHCS":
        this.collectiviteListFC.enable();
        this.quarterListFC.enable();
        this.centreDeTriListFC.disable()
        this.monthListFC.disable();
        break;
      case "EtatMensuelCollectiviteHCS":
        this.collectiviteListFC.enable();
        this.quarterListFC.disable();
        this.centreDeTriListFC.disable()
        this.monthListFC.enable();
        break;
    }
  }
  //-----------------------------------------------------------------------------------
  //DocumentEditType selected
  onDocumentEditTypeSelected() {
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Ask for export file
  onSave() {
    let filterCentreDeTris: string = "";
    //Save data
    if (this.centreDeTriListFC.value && this.centreDeTriListFC.value.length > 0) {
      filterCentreDeTris = Array.prototype.map.call(this.centreDeTriListFC.value, function (item: EntiteList) { return item.RefEntite; }).join(",");
    }
    let filterCollectivites: string = "";
    if (this.collectiviteListFC.value && this.collectiviteListFC.value.length > 0) {
      filterCollectivites = Array.prototype.map.call(this.collectiviteListFC.value, function (item: EntiteList) { return item.RefEntite; }).join(",");
    }
    // Display progress bar
    this.progressBar.start();
    //Process
    switch (this.documentEditTypeListFC.value) {
      case "EtatMensuelReception":
        var url = this.baseUrl + "evapi/documentedit/etatmensuelreception";
        this.http
          .get(url, {
            headers: new HttpHeaders()
              .set("filterCentreDeTris", filterCentreDeTris)
              .set("month", this.monthListFC.value == null ? "0" : (moment(this.monthListFC.value).month() + 1).toString())
              .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
            observe: "response",
            responseType: "blob"
          })
          .pipe(
            finalize(() => { this.progressBar.stop(); })
          )
          .subscribe((data: HttpResponse<Blob>) => {
            let contentDispositionHeader = data.headers.get("Content-Disposition");
            let fileName = getAttachmentFilename(contentDispositionHeader);
            this.downloadFile(data.body, fileName);
          }, error => {
            this.progressBar.stop();
            this.displayProgressBar = false;
            showErrorToUser(this.dialog, error, this.applicationUserContext);
          });
        break;
      case "EtatTrimerstrielCollectiviteCS":
        var url = this.baseUrl + "evapi/documentedit/etattrimerstrielcollectivitecs";
        this.http
          .get(url, {
            headers: new HttpHeaders()
              .set("filterCollectivites", filterCollectivites)
              .set("quarter", this.quarterListFC.value == null ? "0" : this.quarterListFC.value.toString())
              .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
            observe: "response",
            responseType: "blob"
          })
          .pipe(
            finalize(() => { this.progressBar.stop(); })
          )
          .subscribe((data: HttpResponse<Blob>) => {
            let contentDispositionHeader = data.headers.get("Content-Disposition");
            let fileName = getAttachmentFilename(contentDispositionHeader);
            this.downloadFile(data.body, fileName);
          }, error => {
            showErrorToUser(this.dialog, error, this.applicationUserContext);
          });
        break;
      case "EtatMensuelCollectiviteHCS":
        var url = this.baseUrl + "evapi/documentedit/etatmensuelcollectivitehcs";
        this.http
          .get(url, {
            headers: new HttpHeaders()
              .set("filterCollectivites", filterCollectivites)
              .set("month", this.monthListFC.value == null ? "0" : (moment(this.monthListFC.value).month() + 1).toString())
              .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString()),
            observe: "response",
            responseType: "blob"
          })
          .pipe(
            finalize(() => { this.progressBar.stop(); })
          )
          .subscribe((data: HttpResponse<Blob>) => {
            let contentDispositionHeader = data.headers.get("Content-Disposition");
            let fileName = getAttachmentFilename(contentDispositionHeader);
            this.downloadFile(data.body, fileName);
          }, error => {
            showErrorToUser(this.dialog, error, this.applicationUserContext);
          });
        break;
      case "CertificatRecyclageCS":
      case "CertificatRecyclageHCS":
        var url = this.baseUrl + "evapi/documentedit/certificatrecyclage";
        this.http
          .get(url, {
            headers: new HttpHeaders()
              .set("filterCollectivites", filterCollectivites)
              .set("quarter", this.quarterListFC.value == null ? "0" : this.quarterListFC.value.toString())
              .set("year", this.yearListFC.value == null ? "0" : this.yearListFC.value.toString())
              .set("cs", this.documentEditTypeListFC.value == "CertificatRecyclageCS" ? "true" : "false"),
            observe: "response",
            responseType: "blob"
          })
          .pipe(
            finalize(() => { this.progressBar.stop(); })
          )
          .subscribe((data: HttpResponse<Blob>) => {
            let contentDispositionHeader = data.headers.get("Content-Disposition");
            let fileName = getAttachmentFilename(contentDispositionHeader);
            this.downloadFile(data.body, fileName);
          }, error => {
            showErrorToUser(this.dialog, error, this.applicationUserContext);
          });
        break;
    }
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
  compareFnCollectivite(item: EntiteList, selectedItem: EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  compareFnCentreDeTri(item: EntiteList, selectedItem: EntiteList) {
    return item && selectedItem ? item.RefEntite === selectedItem.RefEntite : item === selectedItem;
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["home"]);
  }
  //-----------------------------------------------------------------------------------
  //Apply filter
  protected filterCollectivitesMulti() {
    if (!this.collectiviteList) {
      return;
    }
    // get the search keyword
    let search = this.collectiviteListFilterFC.value;
    if (!search) {
      this.filteredCollectiviteList.next(this.collectiviteList.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter 
    this.filteredCollectiviteList.next(
      this.collectiviteList.filter(e => e.Libelle.toLowerCase().indexOf(search) > -1)
    );
  }
  protected filterCentreDeTrisMulti() {
    if (!this.centreDeTriList) {
      return;
    }
    // get the search keyword
    let search = this.centreDeTriListFilterFC.value;
    if (!search) {
      this.filteredCentreDeTriList.next(this.centreDeTriList.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter 
    this.filteredCentreDeTriList.next(
      this.centreDeTriList.filter(e => e.Libelle.toLowerCase().indexOf(search) > -1)
    );
  }
}
