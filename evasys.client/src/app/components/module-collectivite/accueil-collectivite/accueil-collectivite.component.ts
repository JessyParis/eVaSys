import { Component, Inject, OnInit, ViewChild } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, Validators, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { AuthService } from "../../../services/auth.service";
import { MatDialog } from "@angular/material/dialog";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { chartSeriesColors, cmpDesc, getAttachmentFilename, getContactAdresseServiceFonctions, getCulturedMonthName, showErrorToUser, toFixed, toSpaceThousandSepatator } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import { DataColumnName, DocumentType, MenuName, StatType } from "../../../globals/enums";
import { DownloadService } from "../../../services/download.service";
import moment from "moment";
import { ErrorStateMatcher, ThemePalette } from "@angular/material/core";
import { ListService } from "../../../services/list.service";
import { MessageVisuComponent } from "../../dialogs/message-visu/message-visu.component";
import { SnackbarMsg } from "../../../interfaces/appInterfaces";
import { StatistiqueService } from "../../../services/statistique.service";
import { finalize, map, take, timer } from "rxjs";
import { ProgressBarComponent } from "../../global/progress-bar/progress-bar.component";
import { ProgressBarMode } from "@angular/material/progress-bar";
import { EventEmitterService } from "../../../services/event-emitter.service";
import { fadeInOnEnterAnimation, fadeOutOnLeaveAnimation, flashAnimation, tadaAnimation } from "angular-animations";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";

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
    selector: "accueil-collectivite",
    templateUrl: "./accueil-collectivite.component.html",
    animations: [
        fadeInOnEnterAnimation(),
        fadeOutOnLeaveAnimation(),
        flashAnimation(),
        tadaAnimation()
    ],
    standalone: false
})

export class AccueilCollectiviteComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  legalMentionEndYear: string;
  form: UntypedFormGroup;
  entite: dataModelsInterfaces.Entite = { Adresses: [], ContactAdresses: [] } as dataModelsInterfaces.Entite;
  refEntite: number = 0;
  documents: { date: string, n: string }[] = [];
  collectivitePrixReprises: dataModelsInterfaces.CollectivitePrixReprise[] = [];
  entiteSAGEDocuments: dataModelsInterfaces.SAGEDocument[] = [];
  dStat: moment.Moment = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));
  dPrixReprise: moment.Moment = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));
  dNoteDeCredit: moment.Moment = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));
  dDeboucheBalle: moment.Moment = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));
  currentMonth: string = "";
  contactAdministratifNom: string;
  contactAdministratifTel: string;
  contactAdministratifEmail: string;
  outilsDeCommunicationFormUrl: string;
  actualitesUrl: string;
  entiteDR: dataModelsInterfaces.EntiteDR;
  tonnage: number = 0;
  tonnagePrevious: number = 0;
  tonnageText: string = "";
  tonnagePreviousText: string = "";
  destinations: any[] = [];
  destinationFrancePercentageText: string = "0%";
  equivalentCO2s: dataModelsInterfaces.EquivalentCO2[] = [];
  currentEquivalentCO2Libelle: string;
  currentEquivalentCO2Icone: string;
  currentEquivalentCO2Ratio: number;
  currentEquivalentCO2Tonnage: number;
  currentEquivalentCO2TonnageText: string;
  accueilCollectiviteImage: string;
  //Form
  monthList: appInterfaces.Month[];
  monthListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  monthEndListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  //Stats
  categoryField: string = "";
  categoryAxis: any = {};
  chartSeries: appInterfaces.ChartSerie[] = [];
  statisticAxisTitle: any = {
    text: ""
  };
  zoomable: any | boolean = { mousewheel: { lock: 'y' } };
  legend: boolean = false;
  visibleChart: boolean = false;
  public seriesColors: string[] = chartSeriesColors;
  //Loading indicator
  loading = false;
  color = "warn" as ThemePalette;
  mode = "indeterminate" as ProgressBarMode;
  value = 50;
  displayProgressBar = false;
  @ViewChild(ProgressBarComponent) progressBar: ProgressBarComponent;
  //Misc
  visibleRSE: boolean = false;
  visibleEquivalentCO2: boolean = false;
  //Animation
  animRSEIcon: boolean = false;
  //Funcions
  getContactAdresseServiceFonctions = getContactAdresseServiceFonctions;
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router, private fb: UntypedFormBuilder
    , private http: HttpClient, @Inject("BASE_URL") private baseUrl: string
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private listService: ListService
    , private downloadService: DownloadService
    , private eventEmitterService: EventEmitterService
    , public auth: AuthService
    , private snackBarQueueService: SnackBarQueueService
    , private statistiqueService: StatistiqueService
    , public dialog: MatDialog) {
    //Init
    this.legalMentionEndYear = moment().format("YYYY");
    this.contactAdministratifNom = applicationUserContext.parametres.find(r => r.RefParametre == 3).ValeurTexte;
    this.contactAdministratifTel = applicationUserContext.parametres.find(r => r.RefParametre == 4).ValeurTexte;
    this.contactAdministratifEmail = applicationUserContext.parametres.find(r => r.RefParametre == 2).ValeurTexte;
    this.outilsDeCommunicationFormUrl = applicationUserContext.parametres.find(r => r.RefParametre == 5).ValeurTexte;
    this.actualitesUrl = applicationUserContext.parametres.find(r => r.RefParametre == 1).ValeurTexte;
    this.accueilCollectiviteImage = applicationUserContext.parametres.find(r => r.RefParametre == 1).ValeurBinaireBase64;
    applicationUserContext.visibleHeaderCollectivite = false;
    //Change stat reference date to stay on past year during january
    if (this.dStat.month() == 0) {
      this.dStat = moment(new Date(new Date().getFullYear() - 1, 11, 1, 0, 0, 0, 0));
    }
    else {
      this.dStat = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //Set filters
    if (this.applicationUserContext.filterCollectivites.length == 1) {
      this.refEntite = this.applicationUserContext.filterCollectivites[0].RefEntite;
    }
    this.dataModelService.getEntite(this.refEntite, null, null)
      .subscribe(result => {
        //Get data
        this.entite = result;
        this.dataModelService.setMomentFromTextEntite(this.entite);
        //Init arrays
        this.entite.DocumentEntites = this.entite.DocumentEntites.filter(f => f.DocumentNoFile.VisibiliteTotale == true);
        this.entiteSAGEDocuments = this.entite.SAGEDocuments.filter(f => moment(f.D) > moment(new Date(new Date().getFullYear() - 4, 0, 1, 0, 0, 0, 0)));
        //Sort
        this.entite.DocumentEntites.sort(function (a, b) { return cmpDesc(a.DocumentNoFile.DCreation, b.DocumentNoFile.DCreation); });
        this.entiteSAGEDocuments.sort(function (a, b) { return cmpDesc(a.D, b.D); });
        //Init component data
        if (this.entite.EntiteDRs?.length > 0) {
          this.entiteDR = this.entite.EntiteDRs[0];
        }
        //Check for messages to show today
        this.dataModelService.existsMessageToDisplay(true)
          .subscribe(result => {
            if (result > 0) {
              this.showMessage();
            }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Check for messages
        this.dataModelService.existsMessageToDisplay(false)
          .subscribe(result => {
            this.applicationUserContext.hasMessage = result;
            if (this.applicationUserContext.hasMessage>0) { this.applicationUserContext.animMessage = true; }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get EquivalentCO2 list
    this.listService.getListEquivalentCO2(true).subscribe(result => {
      this.equivalentCO2s = result;
      if (this.equivalentCO2s.length > 0) {
        this.currentEquivalentCO2Libelle = this.equivalentCO2s[0].Libelle;
        this.currentEquivalentCO2Ratio = this.equivalentCO2s[0].Ratio;
        this.currentEquivalentCO2Icone = this.equivalentCO2s[0].IconeBase64;
        this.visibleEquivalentCO2 = true;
      }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get month list
    this.listService.getListMonth().subscribe(result => {
      this.monthList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //get PrixReprise
    this.currentMonth = this.dPrixReprise.format("MMMM YYYY").toUpperCase();
    //Get existing CollectivitePrixReprises
    //Remove duplicates
    this.dataModelService.getCollectivitePrixReprises(this.refEntite, this.dPrixReprise, this.applicationUserContext.filterCollecte).subscribe(result => {
      this.collectivitePrixReprises = result.filter((value, index, self) =>
        index === self.findIndex((t) => (
          t.ProduitNomCommun === value.ProduitNomCommun && t.PUHT === value.PUHT
        )));
      if (result.length == 0) {
        this.dPrixReprise = this.dPrixReprise.add(-1, "months");
        this.currentMonth = this.dPrixReprise.format("MMMM YYYY").toUpperCase();
        this.dataModelService.getCollectivitePrixReprises(this.refEntite, this.dPrixReprise, this.applicationUserContext.filterCollecte).subscribe(result => {
          this.collectivitePrixReprises = result.filter((value, index, self) =>
            index === self.findIndex((t) => (
              t.ProduitNomCommun === value.ProduitNomCommun && t.PUHT === value.PUHT
            )));
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Create form
    this.createForm();
    //Init filters
    this.initFilters();
    //Get data
    this.getDataDeboucheBalle();
    this.getData();
    this.getPreviousData();
    //Init timer
    const countdown$ = timer(0, 10000).pipe(
      take(300),
      map(n => n)
    );
    //Subscribe to timer
    countdown$.subscribe(n => {
      //Calculate economieCO2 index if more than one
      let i: number = 0;
      if (this.equivalentCO2s.length > 1) {
        i = n % this.equivalentCO2s.length;
        this.currentEquivalentCO2Ratio = this.equivalentCO2s[i].Ratio;
        this.currentEquivalentCO2Icone = this.equivalentCO2s[i].IconeBase64;
        this.currentEquivalentCO2Libelle = this.equivalentCO2s[i].Libelle;
        //Animate
        this.animRSEIcon = true;
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      MonthList: this.monthListFC,
      MonthEndList: this.monthEndListFC,
    });
  }
  //Form initial creation
  initFilters() {
    //RAZ filters if no filter or filter out of current year (or previous year if january)
    if (this.applicationUserContext.filterBegin.isSame(moment([1, 0, 1, 0, 0, 0, 0]))
      || this.applicationUserContext.filterEnd.isSame(moment([1, 0, 1, 0, 0, 0, 0]))
      || (new Date().getMonth() > 0 && this.applicationUserContext.filterBegin.year() != (new Date().getFullYear()))
      || (new Date().getMonth() > 0 && this.applicationUserContext.filterEnd.year() != (new Date().getFullYear()))
      || (new Date().getMonth() == 0 && this.applicationUserContext.filterBegin.year() != (new Date().getFullYear()-1))
      || (new Date().getMonth() == 0 && this.applicationUserContext.filterEnd.year() != (new Date().getFullYear()-1))
    ) {
      if (new Date().getMonth() > 0) {
        this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear(), 0, 1, 0, 0, 0, 0));
        this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0)).add(1, "months").add(-1, "days");
      }
      else {
        this.applicationUserContext.filterBegin = moment(new Date(new Date().getFullYear()-1, 0, 1, 0, 0, 0, 0));
        this.applicationUserContext.filterEnd = moment(new Date(new Date().getFullYear()-1, 11, 1, 0, 0, 0, 0)).add(1, "months").add(-1, "days");
      }
    }
    this.applicationUserContext.filterMonths = [];
    this.applicationUserContext.filterMonths.push({
      d: this.applicationUserContext.filterBegin.format("1901-MM-01T00:00:00"),
      n: this.applicationUserContext.filterBegin.month() + 1,
      name: getCulturedMonthName(this.applicationUserContext.filterBegin.month() + 1, this.applicationUserContext)
    } as appInterfaces.Month);
    this.applicationUserContext.filterMonthEnd = {
      d: this.applicationUserContext.filterEnd.format("1901-MM-01T00:00:00"),
      n: this.applicationUserContext.filterEnd.month() + 1,
      name: getCulturedMonthName(this.applicationUserContext.filterEnd.month() + 1, this.applicationUserContext)
    } as appInterfaces.Month;
    //Update form
    this.monthListFC.setValue(this.applicationUserContext.filterMonths.length > 0 ? this.applicationUserContext.filterMonths[0].n : null);
    this.monthEndListFC.setValue(this.applicationUserContext.filterMonthEnd?.n);
  }
  //-----------------------------------------------------------------------------------
  //Save filters
  saveFilter() {
    //Save filters
    this.applicationUserContext.filterBegin = moment([1, 0, 1, 0, 0, 0, 0]); //Moment UTC date
    this.applicationUserContext.filterEnd = moment([1, 0, 1, 0, 0, 0, 0]); //Moment UTC date
    if (this.monthListFC.value) {
      if (new Date().getMonth() > 0) {
        this.applicationUserContext.filterBegin = moment([(new Date().getFullYear()), this.monthListFC.value - 1, 1, 0, 0, 0, 0]);
      }
      else {
        this.applicationUserContext.filterBegin = moment([(new Date().getFullYear()-1), this.monthListFC.value - 1, 1, 0, 0, 0, 0]);
      }
    }
    if (this.monthEndListFC.value) {
      if (new Date().getMonth() > 0) {
        this.applicationUserContext.filterEnd = moment([(new Date().getFullYear()), this.monthEndListFC.value - 1, 1, 0, 0, 0, 0]).add(1, "M").add(-1, "d");
      }
      else {
        this.applicationUserContext.filterEnd = moment([(new Date().getFullYear()-1), this.monthEndListFC.value - 1, 1, 0, 0, 0, 0]).add(1, "M").add(-1, "d");
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //On filter
  onChipChange($event: any) {
    if (!$event.selected) {
      $event.source.selected = true;
    }
    if (this.applicationUserContext.filterCollecte != $event.source.value) {
      this.applicationUserContext.filterCollecte = $event.source.value;
      this.dataModelService.getCollectivitePrixReprises(this.refEntite, this.dPrixReprise, this.applicationUserContext.filterCollecte).subscribe(result => {
        this.collectivitePrixReprises = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      //Get data
      this.getPreviousData();
      this.getData();
    }
  }
  onFilterChange() {
    this.saveFilter();
    //Get data
    this.getPreviousData();
    this.getData();
  }
  //Form
  public getPeriode(): string {
    return this.applicationUserContext.getCulturedRessourceText(1371) + " " + this.dStat.year().toString();
  }
  public getPreviousPeriode(): string {
    return this.applicationUserContext.getCulturedRessourceText(1373) + " " + (this.dStat.year()-1).toString();
  }
  public getYearDeboucheBalle(): string {
    return (this.dDeboucheBalle.year() - 1).toString();
  }
  public getSAGEDocumentDate(sD: dataModelsInterfaces.SAGEDocument): string {
    return moment(sD.D).format("L");
  }
  public getSAGEDocumentHT(sD: dataModelsInterfaces.SAGEDocument): string {
    return sD.TotalHT.toLocaleString(undefined, { minimumFractionDigits: 2 });
  }
  //------------------------------------------------------------------------------
  //Logout
  onLogout() {
    this.auth.logout();
    this.router.navigate(["login"]);
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for messages
  showMessage() {
    //If a message exists
    if (this.applicationUserContext.hasMessage > 0) {
      //Check for messages
      this.dataModelService.existsMessageToDisplay(false)
        .subscribe(result => {
          this.applicationUserContext.hasMessage = result;
          if (result) {
            //open pop-up
            const dialogRef = this.dialog.open(MessageVisuComponent, {
              width: "50%", height: "50%",
              data: { mode: "all" },
              autoFocus: false,
              closeOnNavigation: false,
              disableClose: true,
              restoreFocus: false
            });
            //Re check for messages
            dialogRef.afterClosed().subscribe(result => {
              this.dataModelService.existsMessageToDisplay(false)
                .subscribe(result => {
                  this.applicationUserContext.hasMessage = result;
                }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            });
          }
          else {
            //Inform user
            this.snackBarQueueService.addMessage({
              text: this.applicationUserContext.getCulturedRessourceText(1243)
              , duration: 4000
            } as SnackbarMsg);
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Inform user
      this.snackBarQueueService.addMessage({
        text: this.applicationUserContext.getCulturedRessourceText(1255)
        , duration: 4000
      } as SnackbarMsg);
    }
  }
  //-----------------------------------------------------------------------------------
  //Open Valorplast communication tools
  onCommunicationTool() {
    window.open(this.outilsDeCommunicationFormUrl, "_blank");
  }
  //-----------------------------------------------------------------------------------
  //Dowmload help
  onHelp() {
    this.downloadService.download(DocumentType.Parametre, "14", "", null, "")
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        console.log(error);
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Open Valorplast communication tools
  onHelpRSE() {
    window.open("assets/docs/Guide-RSE.pdf", "_blank");
  }
  //-----------------------------------------------------------------------------------
  //Tooltip for messages
  getHasMessageTooltipText() {
    let s: string = this.applicationUserContext.getCulturedRessourceText(1255);
    if (this.applicationUserContext.hasMessage > 0) {
      s = this.applicationUserContext.getCulturedRessourceText(1256);
    }
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Get EconomieC02
  getCurrentEconomieCO2TonnageText():string {
    let s: string = "";
    s = toSpaceThousandSepatator(Number.parseInt(toFixed(this.currentEquivalentCO2Tonnage * this.currentEquivalentCO2Ratio * 1000, 0))) 
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Destination exists
  isDestination(paysCodeISO: string): boolean {
    let r: boolean = false;
    r = this.destinations.some(e => e.PaysCodeISO == paysCodeISO);
    return r;
  }
  //------------------------------------------------------------------------------
  //Click a menu
  onMenu(menuName: string, action: string) {
    this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === menuName);
    this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === this.applicationUserContext.currentMenu.moduleName);
    this.applicationUserContext.fromMenu = null;
    //Emit event for app compoment refresh (active menus and modules)
    this.eventEmitterService.onDashboardItemClick();
    //Navigate
    switch (this.applicationUserContext.currentMenu.name) {
      case MenuName.ModuleCollectiviteMenuDocuments:
        this.router.navigate(["documents", action]);
        break;
      case MenuName.ModuleCollectiviteMenuPrixReprise:
        this.router.navigate(["prix-reprises"]);
        break;
      case MenuName.ModuleCollectiviteMenuStatistiques:
        this.router.navigate(["statistique-grid"]);
        break;
      case MenuName.ModuleCollectiviteMenuVisualisationAnnuaire:
        this.router.navigate(["my-contacts"]);
        break;
    }
  }
  //------------------------------------------------------------------------------
  //Get data
  getData() {
    let filterCollectivites: string = "";
    //Set filters
    if (this.applicationUserContext.filterCollectivites && this.applicationUserContext.filterCollectivites.length > 0) {
      filterCollectivites = Array.prototype.map.call(this.applicationUserContext.filterCollectivites, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    //get data
    this.statistiqueService.getStatistique(this.applicationUserContext.currentModule.name, MenuName.ModuleCollectiviteMenuStatistiques, 0, 1000
      , this.applicationUserContext.filterBegin.format("YYYY-MM-DD HH:mm:ss.SSS"), this.applicationUserContext.filterEnd.format("YYYY-MM-DD HH:mm:ss.SSS"), false
      , "", "", filterCollectivites, "", "", "", ""
      , "", "", "", "", ""
      , "", "", "", "", "", "", "", ""
      , false, this.applicationUserContext.filterCollecte, "", null, "", "", "", ""
      , "", "", "", "", ""
      , false, "Month", "", "", "", StatType.DestinationTonnageParPays).subscribe(
        {
          next: (result: HttpResponse<any[]>) => {
            if (result.body.length > 0) {
              this.destinations = result.body;
              this.tonnage = result.body.reduce((sum, el) => sum += el.PoidsTonne, 0) / 1000;
              this.tonnageText = toSpaceThousandSepatator(Number.parseInt(toFixed(this.tonnage, 0)));
              this.currentEquivalentCO2Tonnage = result.body.reduce((sum, el) => sum += el.PoidsKg, 0) / 1000;
              this.currentEquivalentCO2TonnageText = toSpaceThousandSepatator(Number.parseInt(toFixed(this.currentEquivalentCO2Tonnage, 0)));
              this.visibleRSE = true;
              if (result.body.filter(el => el.PaysCodeISO == "FR")[0]) {
                this.destinationFrancePercentageText = toFixed((result.body.filter(el => el.PaysCodeISO == "FR")[0].PoidsTonne / 1000 / this.tonnage) * 100, 0) + "%";
              }
              else {
                this.destinationFrancePercentageText = "0%";
              }
            }
            else {
              this.destinations = [];
              this.destinationFrancePercentageText = "0%";
              this.tonnage = 0;
              this.tonnageText = "0";
              this.currentEquivalentCO2Tonnage = 0;
              this.currentEquivalentCO2TonnageText = "0";
              this.visibleRSE = false;
            }
          }
          , error: e => showErrorToUser(this.dialog, e, this.applicationUserContext)
        });
  }
  //------------------------------------------------------------------------------
  //Get data for DeboucheBalle
  getDataDeboucheBalle() {
    let filterCollectivites: string = "";
    //Set filters
    if (this.applicationUserContext.filterCollectivites && this.applicationUserContext.filterCollectivites.length > 0) {
      filterCollectivites = Array.prototype.map.call(this.applicationUserContext.filterCollectivites, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    //Get data for stat DeboucheBalle
    //Set date for past year
    let begin: moment.Moment = moment(new Date(this.dDeboucheBalle.year() - 1, new Date().getMonth(), 1, 0, 0, 0, 0));
    let end: moment.Moment = moment(new Date(this.dDeboucheBalle.year(), new Date().getMonth(), 1, 0, 0, 0, 0)).add(-1, "days");
    //Get data
    this.statistiqueService.getStatistique(this.applicationUserContext.currentModule.name, MenuName.ModuleCollectiviteMenuStatistiques, 0, 1000
      , begin.format("YYYY-MM-DD HH:mm:ss.SSS"), end.format("YYYY-MM-DD HH:mm:ss.SSS"), false
      , "", "", filterCollectivites, "", "", "", ""
      , "", "", "", "", ""
      , "", "", "", "", "", "", "", ""
      , false, this.applicationUserContext.filterCollecte, "", null, "", "", "", ""
      , "", "", "", "", ""
      , false, "Month", "", "", "", StatType.DeboucheBalle).subscribe(
        {
          next: (result: HttpResponse<any[]>) => {
            //RAZ parameters
            this.chartSeries = [];
            this.categoryField = "";
            this.categoryAxis = {};
            //If data present
            if (result.body.length > 0) {
              //Init parameters
              this.visibleChart = true;
              this.categoryField = DataColumnName.ApplicationLibelle;
              this.categoryAxis = {
                categories: this.categoryField,
                labels: { rotation: "auto" }
              }
              this.zoomable = false;
              //Get item properties
              let listPropertyNames = Object.keys(result.body[0]);
              //Create series
              for (let prop of listPropertyNames) {
                if (prop != this.categoryField) {
                  let serie: appInterfaces.ChartSerie = {
                    type: "donut",
                    data: result.body,
                    field: prop,
                    categoryField: this.categoryField,
                    name: prop,
                  };
                  this.chartSeries.push(serie);
                }
              }
              this.legend = true;
            }
            else {
              this.dDeboucheBalle = this.dDeboucheBalle.add(-1, "years");
              let begin: moment.Moment = moment(new Date(this.dDeboucheBalle.year() - 1, new Date().getMonth(), 1, 0, 0, 0, 0));
              let end: moment.Moment = moment(new Date(this.dDeboucheBalle.year(), new Date().getMonth(), 1, 0, 0, 0, 0)).add(-1, "days");
              //Get data
              this.statistiqueService.getStatistique(this.applicationUserContext.currentModule.name, MenuName.ModuleCollectiviteMenuStatistiques, 0, 1000
                , begin.format("YYYY-MM-DD HH:mm:ss.SSS"), end.format("YYYY-MM-DD HH:mm:ss.SSS"), false
                , "", "", filterCollectivites, "", "", "", ""
                , "", "", "", "", ""
                , "", "", "", "", "", "", "", ""
                , false, this.applicationUserContext.filterCollecte, "", null, "", "", "", ""
                , "", "", "", "", ""
                , false, "Month", "", "", "", StatType.DeboucheBalle).subscribe(
                  {
                    next: (result: HttpResponse<any[]>) => {
                      //If data present
                      if (result.body.length > 0) {
                        //Init parameters
                        this.visibleChart = true;
                        this.categoryField = DataColumnName.ApplicationLibelle;
                        this.categoryAxis = {
                          categories: this.categoryField,
                          labels: { rotation: "auto" }
                        }
                        this.zoomable = false;
                        //Get item properties
                        let listPropertyNames = Object.keys(result.body[0]);
                        //Create series
                        for (let prop of listPropertyNames) {
                          if (prop != this.categoryField) {
                            let serie: appInterfaces.ChartSerie = {
                              type: "donut",
                              data: result.body,
                              field: prop,
                              categoryField: this.categoryField,
                              name: prop,
                            };
                            this.chartSeries.push(serie);
                          }
                        }
                        this.legend = true;
                      }
                      else {
                        this.visibleChart = false;
                      }
                    }
                    , error: e => showErrorToUser(this.dialog, e, this.applicationUserContext)
                  });
            }
          }
          , error: e => showErrorToUser(this.dialog, e, this.applicationUserContext)
        });
  }
  //------------------------------------------------------------------------------
  //Get data
  getPreviousData() {
    let filterCollectivites: string = "";
    let begin = this.applicationUserContext.filterBegin.clone();
    let end = this.applicationUserContext.filterEnd.clone();
    //Set filters
    if (this.applicationUserContext.filterCollectivites && this.applicationUserContext.filterCollectivites.length > 0) {
      filterCollectivites = Array.prototype.map.call(this.applicationUserContext.filterCollectivites, function (item: dataModelsInterfaces.EntiteList) { return item.RefEntite; }).join(",");
    }
    //get data
    this.statistiqueService.getStatistique(this.applicationUserContext.currentModule.name, MenuName.ModuleCollectiviteMenuStatistiques, 0, 1000
      , begin.add(-1, "years").format("YYYY-MM-DD HH:mm:ss.SSS"), end.add(-1, "years").format("YYYY-MM-DD HH:mm:ss.SSS"), false
      , "", "", filterCollectivites, "", "", "", ""
      , "", "", "", "", ""
      , "", "", "", "", "", "", "", ""
      , false, this.applicationUserContext.filterCollecte, "", null, "", "", "", ""
      , "", "", "", "", ""
      , false, "Month", "", "", "", StatType.DestinationTonnageParPays).subscribe((result: HttpResponse<any[]>) => {
        if (result.body.length > 0) {
          this.tonnagePrevious = result.body.reduce((sum, el) => sum += el.PoidsTonne, 0) / 1000;
          this.tonnagePreviousText = toSpaceThousandSepatator(Number.parseInt(toFixed(this.tonnagePrevious, 0)));
        }
        else {
          this.tonnagePrevious = 0;
          this.tonnagePreviousText = "0";
        }
      }
        , error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Download a file
  getFile(fileType: string, refDocumentEntite: number) {
    this.downloadService.download(fileType, refDocumentEntite.toString(), "", this.entite.RefEntite, "")
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
  //Export PDF file
  exportPdf(typeDoc: string, refDoc: string) {
    //Show progress bar
    this.progressBar.start();
    //Get Excel file
    this.downloadService.download(typeDoc, refDoc, "pdf", 0, null)
      .pipe(
        finalize(() => this.progressBar.stop())
      )
      .subscribe((data: Response) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      },
        error => {
          showErrorToUser(this.dialog, error, this.applicationUserContext);
        });
  }
  //------------------------------------------------------------------------------
  //Change profil
  onChangeProfil() {
    let data: any;
    let title: string = "";
    //Title
    title = this.applicationUserContext.getCulturedRessourceText(1500);
    //Calculate id
    data = {
      title: title, type: "Profils", changeProfil: true
    };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      data,
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      //If something have been done
      if (result) {
        if (result.ref) {
          this.router.navigate([result.ref]);
        }
      }
      else {
        this.router.navigate(["login"]);
      }
    })
  }
}
