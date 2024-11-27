import { Component, OnInit, AfterViewInit, ViewChild, Inject, Input, Output, EventEmitter } from "@angular/core";
import { Router, NavigationEnd } from "@angular/router";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { Observable, of ,  BehaviorSubject } from "rxjs";
import { catchError, finalize, tap } from "rxjs/operators";
import { GridSimpleService } from "../../../services/grid-simple.service";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { ApplicationUserContext } from "../../../globals/globals";
import { UntypedFormGroup, UntypedFormControl, Validators, FormGroupDirective, NgForm } from "@angular/forms";
import { SelectionModel } from "@angular/cdk/collections";
import { DataColumnName, MenuName, ModuleName } from "../../../globals/enums";
import moment from "moment";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { createMatTableColumn } from "../../../globals/utils";
import { MatTableColumn } from '../../../interfaces/appInterfaces';
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { EventEmitterService } from "../../../services/event-emitter.service";
import { ErrorStateMatcher } from "@angular/material/core";

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
    selector: "grid-simple",
    templateUrl: "./grid-simple.component.html",
    standalone: false
})
//-----------------------------------------------------------------------------------
//Component class
export class GridSimpleComponent implements AfterViewInit, OnInit {
  matcher = new MyErrorStateMatcher();
  @Input() type: string;
  @Input() dataItems: any[];
  @Input() exclude: string;
  @Output() askClose = new EventEmitter<any>();
  visibleUtilisateurType: boolean = false;
  //Form
  form: UntypedFormGroup;
  //Filters
  filterText: string = "";
  filterUtilisateurType: string = "";
  public filterYear: string = "";
  public filterD: moment.Moment = moment([1, 0, 1, 0, 0, 0, 0]);
  //Data
  selectedItem: any;
  item: any;
  dataSource: GridDataSource;
  selection = new SelectionModel<any>(true, []);
  competitors: any[];
  matSortDisabled: boolean = false;
  pageSize: number = 10;
  @ViewChild(MatPaginator, { static: true }) pagin: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string
    , private router: Router
    , private gridSimpleService: GridSimpleService
    , private eventEmitterService: EventEmitterService
    , public applicationUserContext: ApplicationUserContext
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.http = http;
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
    //Set moment culture
    moment.locale(this.applicationUserContext.currentCultureName.substr(0, 2));
    //Init competitors
    this.competitors = [];
    this.competitors.push({ RefTransport: null, TransporteurLibelle: this.applicationUserContext.getCulturedRessourceText(299), TransportPUHT: null, Nb: null })
  }
  //-----------------------------------------------------------------------------------
  //On init, creation of columns
  ngOnInit() {
    this.dataSource = new GridDataSource(this.gridSimpleService, this.snackBarQueueService, this.applicationUserContext, this.type);
    this.form = new UntypedFormGroup({
      FilterText: new UntypedFormControl(""),
      FilterUtilisateurType: new UntypedFormControl("", Validators.required)
    });
    switch (this.type) {
      case "ContactModificationRequest":
        this.sort.active = "tblAction.DCreation";
        this.sort.direction = "desc";
        this.loadPage("");
        break;
      case "AnnuaireAdresseInactive":
        this.sort.active = DataColumnName.TexteAdresseList;
        this.sort.direction = "asc";
        this.pagin.pageSize = 1000;
        this.pagin.hidePageSize = true;
        this.loadPage("");
        break;
      case "AnnuaireContactAdresseInactive":
        this.sort.active = DataColumnName.TexteContactAdresseList;
        this.sort.direction = "asc";
        this.pagin.pageSize = 1000;
        this.pagin.hidePageSize = true;
        this.matSortDisabled = true;
        this.pageSize = 1000;
        this.loadPage("");
        break;
      case "IncitationQualite":
        this.sort.active = DataColumnName.EntiteCodeCITEO;
        this.sort.direction = "asc";
        if (this.filterYear != "") { this.loadPage(""); }
        break;
      case "EmailNoteCreditCollectivite":
        this.sort.active = DataColumnName.SAGENumeroPiece;
        this.sort.direction = "asc";
        if (this.filterD.toISOString() != moment([1, 0, 1, 0, 0, 0, 0]).toISOString()) { this.loadPage(""); }
        break;
      case "Mixte":
        this.sort.active = DataColumnName.CommandeFournisseurNumeroCommande;
        this.sort.direction = "desc";
        this.loadPage("");
        break;
      case "TransporteurForUtilisateur":
      case "CentreDeTriForUtilisateur":
      case "CollectiviteForUtilisateur":
      case "ClientForUtilisateur":
      case "PrestataireForUtilisateur":
      case "EntiteForDocument":
        this.sort.active = DataColumnName.EntiteLibelle;
        this.sort.direction = "asc";
        this.loadPage("");
        break;
      case "UtilisateurMaitreForUtilisateur":
        this.sort.active = DataColumnName.UtilisateurNom;
        this.sort.direction = "asc";
        this.loadPage("");
        break;
    }
    //Localization
    this.pagin._intl.firstPageLabel = this.applicationUserContext.getCulturedRessourceText(294);
    this.pagin._intl.lastPageLabel = this.applicationUserContext.getCulturedRessourceText(295);
    this.pagin._intl.itemsPerPageLabel = this.applicationUserContext.getCulturedRessourceText(296);
    this.pagin._intl.previousPageLabel = this.applicationUserContext.getCulturedRessourceText(297);
    this.pagin._intl.nextPageLabel = this.applicationUserContext.getCulturedRessourceText(298);
    this.pagin._intl.getRangeLabel = (page: number, pageSize: number, length: number) => { if (length === 0 || pageSize === 0) { return ("0 de " + length); } length = Math.max(length, 0); const startIndex = page * pageSize; const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize; return ((startIndex + 1) + " - " + endIndex + " de " + length); }
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Row clicked
  manageScreen() {
    this.form.get("FilterUtilisateurType").disable();
    if (this.type == "UtilisateurMaitreForUtilisateur") {
      this.visibleUtilisateurType = true;
      this.form.get("FilterUtilisateurType").enable();
    }
  }
  //-----------------------------------------------------------------------------------
  //Row clicked
  onRowClicked(item: any) {
    this.selectedItem = item;
    switch (this.type) {
      case "ContactModificationRequest":
        //Close and show related Action
        this.askClose.emit({ type: this.type });
        if (item.RefAction >= 0) {
          this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Annuaire);
          this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.AnnuaireMenuEntite);
          //Emit event for app compoment refresh (active menus and modules)
          this.eventEmitterService.onDashboardItemClick();
          //Navigate
          this.router.navigate(["entite", item.RefAction, { elementType: "Action" }])
        }
        break;
      case "AnnuaireAdresseInactive":
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: item.RefAdresse });
        break;
      case "AnnuaireContactAdresseInactive":
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: item.RefContactAdresse });
        break;
      case "Mixte":
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: item.RefCommandeFournisseur });
        break;
      case "TransporteurForUtilisateur":
      case "CentreDeTriForUtilisateur":
      case "CollectiviteForUtilisateur":
      case "ClientForUtilisateur":
      case "PrestataireForUtilisateur":
      case "EntiteForDocument":
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: item.RefEntite });
        break;
      case "UtilisateurMaitreForUtilisateur":
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: item.Id });
        break;
    }
  }

  //-----------------------------------------------------------------------------------
  //Ask for new Adresse
  disguardAdresseInactive() {
    switch (this.type) {
      case "AnnuaireAdresseInactive":
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: 0 });
        break;
    }
  }

  //-----------------------------------------------------------------------------------
  //Ask for new ContactAdresse
  disguardContactAdresseInactive() {
    switch (this.type) {
      case "AnnuaireContactAdresseInactive":
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: 0 });
        break;
    }
  }

  // All rows are selected, or not
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.items.length;
    return numSelected === numRows;
  }

  // Selects all rows if they are not all selected; otherwise clear selection. 
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.items.forEach(row => this.selection.select(row));
  }
  //-----------------------------------------------------------------------------------
  //View init
  ngAfterViewInit() {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.pagin.pageIndex = 0);

    this.pagin.page
      .pipe(
        tap(() => { this.razSelection(); this.loadPage("") })
      )
      .subscribe();

    this.sort.sortChange
      .pipe(
        tap(() => { this.razSelection(); this.loadPage("") })
      )
      .subscribe();
  }
  //-----------------------------------------------------------------------------------
  //RAZ selection
  razSelection() {
    this.applicationUserContext.selectedItem = [];
  }
  //-----------------------------------------------------------------------------------
  //Save and apply filters
  applyFilter() {
    //Set filters
    this.setFilters();
    //RAZ selection
    this.razSelection();
    //Reset pager
    this.pagin.pageIndex = 0
    //Apply filters
    this.loadPage("");
  }
  //-----------------------------------------------------------------------------------
  //Set filters
  setFilters() {
    this.filterText = this.form.get("FilterText").value;
    this.filterUtilisateurType = this.form.get("FilterUtilisateurType").value;
  }
  //-----------------------------------------------------------------------------------
  //Load data
  loadPage(action: string): boolean {
    this.setFilters();
    this.dataSource.loadItems(this.type, this.sort.active, this.sort.direction, this.pagin.pageIndex, this.pagin.pageSize
      , this.filterText, this.filterYear
      , this.filterD.toISOString() === moment([1, 0, 1, 0, 0, 0, 0]).toISOString() ? "" : this.filterD.format("YYYY-MM-DD 00:00:00.000")
      , this.filterUtilisateurType, this.exclude 
      , this.selectedItem, this.dataItems)
    return true;
  }
}

//-----------------------------------------------------------------------------------
//Datasource class
export class GridDataSource implements DataSource<any> {
  //Subjects
  private itemsSubject = new BehaviorSubject<any[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();
  public nbRow: number = 0; //Pager length
  public items: any[];
  public columns: MatTableColumn[] = [];
  public displayedColumns: string[] = [];
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private gridSimpleService: GridSimpleService
    , private snackBarQueueService: SnackBarQueueService
    , public applicationUserContext: ApplicationUserContext
    , private type: string
  ) { }
  //Connect to table
  connect(collectionViewer: CollectionViewer): Observable<any[]> {
    return this.itemsSubject.asObservable();
  }
  //-----------------------------------------------------------------------------------
  //Disconnect from table
  disconnect(collectionViewer: CollectionViewer): void {
    this.itemsSubject.complete();
    this.loadingSubject.complete();
  }
  //-----------------------------------------------------------------------------------
  //Load data
  loadItems(type = "", sortExpression = "", sortDirection = "asc", pageIndex = 0, pageSize = 10
    , filterText = "", filterYear = "", filterD = "", filterUtilisateurType = "", exclude = ""
    , selectedItem = "", dataItems: any[] = []) {
    //Load items from data or database
    if (this.type == "AnnuaireAdresseInactive") {
      //Create columns
      let column: MatTableColumn;
      let dataColumn = this.applicationUserContext.envDataColumns.find(e => e.name == DataColumnName.TexteAdresseList);
      column = {
        columnDef: DataColumnName.TexteAdresseList,
        header: dataColumn.culturedCaption,
        sticky: false,
        stickyEnd: false,
        cell: (item: any) => item[DataColumnName.TexteAdresseList]
      };
      this.columns = [];
      this.columns.push(column);
      this.displayedColumns = this.columns.map(c => c.columnDef);
      //Load data
      this.nbRow = dataItems.length;
      this.itemsSubject.next(dataItems);
      this.items = dataItems;
    }
    else if (this.type == "AnnuaireContactAdresseInactive") {
      //Create columns
      let column: MatTableColumn;
      let dataColumn = this.applicationUserContext.envDataColumns.find(e => e.name == DataColumnName.TexteContactAdresseList);
      column = {
        columnDef: DataColumnName.TexteContactAdresseList,
        header: dataColumn.culturedCaption,
        sticky: false,
        stickyEnd: false,
        cell: (item: any) => item["ListText"]
      };
      this.columns = [];
      this.columns.push(column);
      this.displayedColumns = this.columns.map(c => c.columnDef);
      //Load data
      this.nbRow = dataItems.length;
      this.itemsSubject.next(dataItems);
      this.items = dataItems;
    }
    else {
      //Show loading indicator
      this.loadingSubject.next(true);
      //Get data and hide loading indicator
      this.gridSimpleService.getItems(type, sortExpression, sortDirection, pageIndex, pageSize
        , filterText, filterYear, filterD, filterUtilisateurType,exclude, selectedItem).pipe<any, HttpResponse<any[]>>(
          catchError(() => of(["Error"])),
          finalize(() => this.loadingSubject.next(false))
        )
        .subscribe((resp: HttpResponse<any[]>) => {
          let n: number = parseInt(resp.headers.get("nbRow"), 10);
          if (!isNaN(n)) { this.nbRow = n };
          //Manage columns if data is present
          if (resp.body.length > 0) {
            let columnsTmp: MatTableColumn[] = [];
            //Get item properties
            let listPropertyNames = Object.keys(resp.body[0]);
            //Create columns if data is present and colums collection changed
            let refreshColumns: boolean = false;
            //Create columns
            for (let prop of listPropertyNames) {
              let col: MatTableColumn = createMatTableColumn(prop, this.applicationUserContext);
              if (col !== null) { columnsTmp.push(col); }
            }
            //Check changes in columns collection
            if (this.columns.length !== columnsTmp.length) { refreshColumns = true; }
            else {
              for (let c in this.columns) {
                if (this.columns[c].columnDef != columnsTmp[c].columnDef) { refreshColumns = true; }
              }
            }
            //Refresh columns if needed
            if (refreshColumns) {
              this.columns = columnsTmp;
              this.displayedColumns = this.columns.map(c => c.columnDef);
            }
          }
          else {
            //clear existing columns and inform user
            this.columns = [];
            this.displayedColumns = this.columns.map(c => c.columnDef);
            this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(318), duration: 4000 } as appInterfaces.SnackbarMsg);
          }
          //Load data
          this.itemsSubject.next(resp.body);
          this.items = resp.body;
        });
    }
  }
}



