import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, UntypedFormArray } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import moment from "moment";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { showErrorToUser, cmp, getCreationModificationTooltipText } from "../../../globals/utils";
import { HabilitationLogistique } from "../../../globals/enums";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";

class MyErrorStateMatcher implements ErrorStateMatcher {
    //Constructor
    constructor() {}
    isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
        let result: boolean = false;
        result = !!((control && control.invalid)); 
        return result;
    }
}

@Component({
    selector: "prix-reprise",
    templateUrl: "./prix-reprise.component.html",
    styleUrls: ["./prix-reprise.component.scss"],
    standalone: false
})

export class PrixRepriseComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Variables
  prixReprises: dataModelsInterfaces.PrixReprise[] = [];
  //Form
  form: UntypedFormGroup;
  prxReps: UntypedFormArray = new UntypedFormArray([]);
  processList: dataModelsInterfaces.Process[];
  produitList: dataModelsInterfaces.ProduitList[];
  composantList: dataModelsInterfaces.ProduitList[];
  yearList: appInterfaces.Year[];
  monthList: appInterfaces.Month[];
  processListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  produitListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  composantListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  yearListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  monthListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = true;
  //Misc
  filterProduits: string = "";
  filterComposants: string = "";
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.setFilters();
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      ProcessList: this.processListFC,
      YearList: this.yearListFC,
      MonthList: this.monthListFC,
      PrxReps: this.prxReps,
    });
    this.lockScreen();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.processListFC.disable();
    this.yearListFC.disable();
    this.monthListFC.disable();
    this.prxReps.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    //this.processListFC.enable();
    this.yearListFC.enable();
    this.monthListFC.enable();
    this.prxReps.enable();
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
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let refProcess = Number.parseInt(this.activatedRoute.snapshot.params["refProcess"], 10);
    let d = this.activatedRoute.snapshot.params["d"];
    //Check parameters
    if (d && moment(d).isValid()/* && !isNaN(refProcess)*/) {
      this.applicationUserContext.prixRepriseRefProcess = refProcess;
      this.applicationUserContext.prixRepriseD = moment(d);
      var url = this.baseUrl + "evapi/prixreprise/getprixreprises";
      this.http.get<dataModelsInterfaces.PrixReprise[]>(url,
        {
          headers: new HttpHeaders()
            .set("refProcess", this.applicationUserContext.prixRepriseRefProcess.toString())
            .set("d", this.applicationUserContext.prixRepriseD.format("YYYY-MM-DD 00:00:00"))
            .set("filterProduits", this.filterProduits)
            .set("filterComposants", this.filterComposants)
        }).subscribe(result => {
          //Get data
          result.forEach(p => {
            if (p.RefPrixReprise < 0) {
              p.PUHT = null;
              p.PUHTSurtri = null;
              p.PUHTTransport = null;
            }
          })
          this.prixReprises = result;
          //Sort prixReprises
          let sortedArray: dataModelsInterfaces.PrixReprise[] = this.prixReprises.sort(function (a, b) {
            return cmp(
              [-cmp(a.PUHT == null ? 0 : 1, b.PUHT == null ? 0 : 1), cmp(a.Produit.Libelle, b.Produit.Libelle)],
              [-cmp(b.PUHT == null ? 0 : 1, a.PUHT == null ? 0 : 1), cmp(b.Produit.Libelle, a.Produit.Libelle)]);
          });
          this.prixReprises = sortedArray;
          //Update form
          this.updateForm();
          //Get existing Process
          this.listService.getListProcess(this.applicationUserContext.prixRepriseRefProcess, null)
            .subscribe(result => {
              this.processList = result;
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Get existing month
          this.listService.getListMonth().subscribe(result => {
            this.monthList = result;
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Get existing year
          this.listService.getListYear().subscribe(result => {
            this.yearList = result;
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //New PrixReprise
      this.applicationUserContext.prixRepriseRefProcess = null;
      this.applicationUserContext.prixRepriseD = null;
      this.prixReprises = [];
      //Init date
      this.applicationUserContext.prixRepriseD = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0))
      //Update form
      this.updateForm();
      //Get existing Process
      this.listService.getListProcess(null, null)
        .subscribe(result => {
          this.processList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      //Get existing month
      this.listService.getListMonth().subscribe(result => {
        this.monthList = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      //Get existing year
      this.listService.getListYear().subscribe(result => {
        this.yearList = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      //Get data
      this.getData();
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  getData() {
    //Init
    this.prxReps.clear();
    //Process
    if (moment(this.applicationUserContext.prixRepriseD).isValid()
      /*&& !isNaN(this.applicationUserContext.prixRepriseRefProcess)*/
    ) {
      var url = this.baseUrl + "evapi/prixreprise/getprixreprises";
      this.http.get<dataModelsInterfaces.PrixReprise[]>(url,
        {
          headers: new HttpHeaders()
            .set("refProcess", "")
            .set("d", this.applicationUserContext.prixRepriseD.format("YYYY-MM-DD 00:00:00"))
            .set("filterProduits", this.filterProduits)
            .set("filterComposants", this.filterComposants)
        }).subscribe(result => {
          //Get data
          result.forEach(p => {
            if (p.RefPrixReprise < 0) {
              p.PUHT = null;
              p.PUHTSurtri = null;
              p.PUHTTransport = null;
            }
          })
          this.prixReprises = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Update form
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.processListFC.setValue((this.applicationUserContext.prixRepriseRefProcess ? this.applicationUserContext.prixRepriseRefProcess : null));
    if (this.applicationUserContext.prixRepriseD !== null) {
      this.yearListFC.setValue((this.applicationUserContext.prixRepriseD ? this.applicationUserContext.prixRepriseD.year() : null));
      this.monthListFC.setValue((this.applicationUserContext.prixRepriseD ? moment(new Date(1901, this.applicationUserContext.prixRepriseD.month(), 1, 0, 0, 0)).format("YYYY-MM-DDT00:00:00") : null));
    }
    for (const prxRep of this.prixReprises) {
      const grp = this.fb.group({
        PUHT: [prxRep.PUHT, [Validators.max(2500), Validators.min(-1500)]],
        PUHTSurtri: [prxRep.PUHTSurtri, Validators.max(300)],
        PUHTTransport: [prxRep.PUHTTransport, Validators.max(100)],
      });
      this.prxReps.push(grp);
    }
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    for (let i in this.prxReps.controls) {
      let fG: UntypedFormGroup = this.prxReps.controls[i] as UntypedFormGroup;
      this.prixReprises[i].PUHT = fG.get("PUHT").value;
      this.prixReprises[i].PUHTSurtri = fG.get("PUHTSurtri").value;
      this.prixReprises[i].PUHTTransport = fG.get("PUHTTransport").value;
    }
  }
  //-----------------------------------------------------------------------------------
  //Get data for IHM
  get getFormData(): UntypedFormArray {
    return this.prxReps as UntypedFormArray;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Process
    var url = this.baseUrl + "evapi/prixreprise/postprixreprises";
    //Update if existing models
    this.http
      .post<dataModelsInterfaces.PrixReprise[]>(url, this.prixReprises)
      .subscribe(result => {
        //Redirect to grid and inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Fournisseur selected
  onProcessSelected() {
    this.applicationUserContext.prixRepriseRefProcess = this.processListFC.value;
    //Get PrixReprises
    this.getData();
  }
  //-----------------------------------------------------------------------------------
  //Year selected
  onYearSelected() {
    if (this.monthListFC.value !== null && this.yearListFC.value !== null) {
      this.applicationUserContext.prixRepriseD = moment(new Date(this.yearListFC.value as number, moment(this.monthListFC.value).toDate().getMonth(), 1, 0, 0, 0, 0));
    }
    else {
      this.applicationUserContext.prixRepriseD = null;
    }
    //Get PrixReprises
    this.getData();
  }
  //-----------------------------------------------------------------------------------
  //Month selected
  onMonthSelected() {
    if (this.monthListFC.value !== null && this.yearListFC.value !== null) {
      this.applicationUserContext.prixRepriseD = moment(new Date(this.yearListFC.value as number, moment(this.monthListFC.value).toDate().getMonth(), 1, 0, 0, 0, 0));
    }
    else {
      this.applicationUserContext.prixRepriseD = null;
    }
    //Get PrixReprises
    this.getData();
  }
  //-----------------------------------------------------------------------------------
  //Set filters
  setFilters() {
    if (this.applicationUserContext.filterProduits && this.applicationUserContext.filterProduits.length > 0) {
      this.filterProduits = Array.prototype.map.call(this.applicationUserContext.filterProduits, function (item: dataModelsInterfaces.ProduitList) { return item.RefProduit; }).join(",");
    }
    else {
      this.filterProduits = "";
    }
    if (this.applicationUserContext.filterComposants && this.applicationUserContext.filterComposants.length > 0) {
      this.filterComposants = Array.prototype.map.call(this.applicationUserContext.filterComposants, function (item: dataModelsInterfaces.Produit) { return item.RefProduit; }).join(",");
    }
    else {
      this.filterComposants = "";
    }
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationTooltipText(i: number): string {
    return getCreationModificationTooltipText(this.prixReprises[i]);
  }
}
