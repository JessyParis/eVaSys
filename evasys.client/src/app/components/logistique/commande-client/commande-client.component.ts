import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, UntypedFormArray } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { InformationComponent } from "../../dialogs/information/information.component";
import { ContratType, HabilitationLogistique } from "../../../globals/enums";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import moment from "moment";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getContratLabelMultiLine, getContratEntitesLabel, showErrorToUser, getCertficationTooltipText } from "../../../globals/utils";
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
  selector: "commande-client",
  templateUrl: "./commande-client.component.html",
  styleUrls: ["./commande-client.component.scss"],
  standalone: false
})

export class CommandeClientComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Variables
  commandeClientMensuelles: dataModelsInterfaces.CommandeClientMensuelleForm[] = [];
  //Form
  form: UntypedFormGroup;
  cmdMs: UntypedFormArray = new UntypedFormArray([]);
  entiteList: dataModelsInterfaces.EntiteList[];
  adresseList: dataModelsInterfaces.Adresse[];
  contratList: dataModelsInterfaces.Contrat[];
  yearList: appInterfaces.Year[];
  monthList: appInterfaces.Month[];
  entiteListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  adresseListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  contratListFC: UntypedFormControl = new UntypedFormControl(null);
  yearListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  monthListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = true;
  //Functions
  getContratLabelMultiLine = getContratLabelMultiLine;
  getContratEntiteLabel = getContratEntitesLabel;
  //Misc
  contratEntitesLabel: string = "";
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      EntiteList: this.entiteListFC,
      ContratList: this.contratListFC,
      AdresseList: this.adresseListFC,
      YearList: this.yearListFC,
      MonthList: this.monthListFC,
      CmdMs: this.cmdMs,
    });
    this.lockScreen();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.entiteListFC.disable();
    this.adresseListFC.disable();
    this.contratListFC.disable();
    this.yearListFC.disable();
    this.monthListFC.disable();
    this.cmdMs.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.entiteListFC.enable();
    this.adresseListFC.enable();
    this.contratListFC.enable();
    this.yearListFC.enable();
    this.monthListFC.enable();
    this.cmdMs.enable();
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
    let refEntite = Number.parseInt(this.activatedRoute.snapshot.params["refEntite"], 10);
    let refAdresse = Number.parseInt(this.activatedRoute.snapshot.params["refAdresse"], 10);
    let refContrat = Number.parseInt(this.activatedRoute.snapshot.params["refContrat"], 10);
    let d = this.activatedRoute.snapshot.params["d"];
    if (isNaN(refContrat)) { refContrat = null; }
    //Check parameters
    if (moment(d).isValid() && !isNaN(refEntite) && !isNaN(refAdresse)) {
      this.applicationUserContext.commandeClientFormRefEntite = refEntite;
      this.applicationUserContext.commandeClientFormRefAdresse = refAdresse;
      this.applicationUserContext.commandeClientFormRefContrat = refContrat;
      this.applicationUserContext.commandeClientFormD = moment(d);
      var url = this.baseUrl + "evapi/commandeclient/getcommandeclientmensuelles";
      this.http.get<dataModelsInterfaces.CommandeClientMensuelleForm[]>(url,
        {
          headers: new HttpHeaders()
            .set("refEntite", this.applicationUserContext.commandeClientFormRefEntite.toString())
            .set("refAdresse", this.applicationUserContext.commandeClientFormRefAdresse.toString())
            .set("refContrat", (this.applicationUserContext.commandeClientFormRefContrat ? this.applicationUserContext.commandeClientFormRefContrat.toString() : ""))
            .set("d", this.applicationUserContext.commandeClientFormD.format("YYYY-MM-DD 00:00:00"))
        }).subscribe(result => {
          //Get data
          this.commandeClientMensuelles = result;
          //Update form
          this.updateForm();
          //Get existing Entite
          this.listService.getListClient(this.applicationUserContext.commandeClientFormRefEntite
            , null, null, null, null, true)
            .subscribe(result => {
              this.entiteList = result;
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Get existing Adresse
          this.listService.getListAdresse(this.applicationUserContext.commandeClientFormRefAdresse, this.applicationUserContext.commandeClientFormRefEntite
            , 1, null, null, null, true)
            .subscribe(result => {
              this.adresseList = result;
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Get existing Contrat
          this.getContrat();
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
      //New CommandeClient
      this.applicationUserContext.commandeClientFormRefEntite = null;
      this.applicationUserContext.commandeClientFormRefAdresse = null;
      this.applicationUserContext.commandeClientFormRefContrat = null;
      this.applicationUserContext.commandeClientFormD = null;
      this.commandeClientMensuelles = [];
      //Update form
      this.updateForm();
      //Get existing Entite
      this.listService.getListClient(null, null, null, null, null, true)
        .subscribe(result => {
          this.entiteList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      //Get existing Adresse
      this.listService.getListAdresse(null, null, null, null, null, null, true)
        .subscribe(result => {
          this.adresseList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      //Get existing Contrat
      this.getContrat();
      //Get existing month
      this.listService.getListMonth().subscribe(result => {
        this.monthList = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      //Get existing year
      this.listService.getListYear().subscribe(result => {
        this.yearList = result;
        this.yearListFC.setValue(new Date().getFullYear());
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  getData() {
    //Init
    this.cmdMs.clear();
    //Process
    if (moment(this.applicationUserContext.commandeClientFormD).isValid()
      && !isNaN(this.applicationUserContext.commandeClientFormRefEntite)
      && !isNaN(this.applicationUserContext.commandeClientFormRefAdresse)) {
      var url = this.baseUrl + "evapi/commandeclient/getcommandeclientmensuelles";
      this.http.get<dataModelsInterfaces.CommandeClientMensuelleForm[]>(url,
        {
          headers: new HttpHeaders()
            .set("refEntite", this.applicationUserContext.commandeClientFormRefEntite.toString())
            .set("refAdresse", this.applicationUserContext.commandeClientFormRefAdresse.toString())
            .set("refContrat", (this.applicationUserContext.commandeClientFormRefContrat ? this.applicationUserContext.commandeClientFormRefContrat.toString() : ""))
            .set("d", this.applicationUserContext.commandeClientFormD.format("YYYY-MM-DD 00:00:00"))
        }).subscribe(result => {
          //Get data
          this.commandeClientMensuelles = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      this.commandeClientMensuelles = [];
      //Update form
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //Get existing Contrat
  getContrat() {
    //Get data
    this.listService.getListContrats(this.applicationUserContext.commandeClientFormRefEntite
      , this.applicationUserContext.commandeClientFormD)
      .subscribe(result => {
        this.contratList = result;
        //Entites
        this.createContratEntitesLabel();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.entiteListFC.setValue((this.applicationUserContext.commandeClientFormRefEntite ? this.applicationUserContext.commandeClientFormRefEntite : null));
    this.adresseListFC.setValue((this.applicationUserContext.commandeClientFormRefAdresse ? this.applicationUserContext.commandeClientFormRefAdresse : null));
    this.contratListFC.setValue((this.applicationUserContext.commandeClientFormRefContrat ? this.applicationUserContext.commandeClientFormRefContrat : null));
    if (this.applicationUserContext.commandeClientFormD !== null) {
      this.yearListFC.setValue((this.applicationUserContext.commandeClientFormD ? this.applicationUserContext.commandeClientFormD.year() : null));
      this.monthListFC.setValue((this.applicationUserContext.commandeClientFormD ? moment(new Date(1901, this.applicationUserContext.commandeClientFormD.month(), 1, 0, 0, 0)).format("YYYY-MM-DDT00:00:00") : null));
    }
    for (const cmd of this.commandeClientMensuelles) {
      const grp = this.fb.group({
        Cmt: [cmd.Cmt],
        PrixTonneHT: [cmd.PrixTonneHT, Validators.max(2000)],
        Poids: [cmd.Poids, Validators.max(2000)],
        IdExt: [cmd.IdExt],
      });
      this.cmdMs.push(grp);
    }

    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    for (let i in this.cmdMs.controls) {
      let fG: UntypedFormGroup = this.cmdMs.controls[i] as UntypedFormGroup;
      this.commandeClientMensuelles[i].Poids = fG.get("Poids").value;
      this.commandeClientMensuelles[i].PrixTonneHT = fG.get("PrixTonneHT").value;
      this.commandeClientMensuelles[i].IdExt = fG.get("IdExt").value;
      this.commandeClientMensuelles[i].Cmt = fG.get("Cmt").value;
      this.commandeClientMensuelles[i].RefContrat = this.contratListFC.value;
    }
  }
  //-----------------------------------------------------------------------------------
  //Get data for IHM
  get getFormData(): UntypedFormArray {
    return this.cmdMs as UntypedFormArray;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Process
    var url = this.baseUrl + "evapi/commandeclient/postcommandeclientMensuelles";
    //Update if existing models
    this.http
      .post<dataModelsInterfaces.CommandeClientMensuelleForm[]>(url, this.commandeClientMensuelles)
      .subscribe(result => {
        //Redirect to grid and inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Show Adresse details
  showDetails(type: string, ref: number) {
    let data: any;
    //Init
    this.saveData();
    switch (type) {
      case "CommandeClientAdresse":
        data = { title: this.applicationUserContext.getCulturedRessourceText(534), type: type, ref: ref };
        break;
    }
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "900px",
      data,
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      //If data in the dialog have been saved
      if (result) {
        switch (result.type) {
          case "CommandeClientAdresse":
            //Get existing adresse
            this.listService.getListAdresse(this.applicationUserContext.commandeClientFormRefAdresse, this.applicationUserContext.commandeClientFormRefEntite
              , null, null, null, null, true)
              .subscribe(result => {
                this.adresseList = result;
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
        }
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Client selected
  onEntiteSelected() {
    this.applicationUserContext.commandeClientFormRefEntite = this.entiteListFC.value;
    this.applicationUserContext.commandeClientFormRefAdresse = null;
    this.adresseListFC.setValue(null);
    //Get existing Adresse
    this.listService.getListAdresse(this.applicationUserContext.commandeClientFormRefAdresse, this.applicationUserContext.commandeClientFormRefEntite
      , 1, null, null, null, true)
      .subscribe(result => {
        this.adresseList = result;
        //Autoselect
        if (result && result.length === 1) {
          //Select firt adresse
          this.applicationUserContext.commandeClientFormRefAdresse = this.adresseList[0].RefAdresse;
          this.adresseListFC.setValue(this.applicationUserContext.commandeClientFormRefAdresse);
        }
        //Get CommandeClients
        this.getData();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Contrat
    this.getContrat();
  }
  //-----------------------------------------------------------------------------------
  //Adresse selected
  onAdresseSelected() {
    this.applicationUserContext.commandeClientFormRefAdresse = this.adresseListFC.value;
    //Get CommandeClients
    this.getData();
  }
  //-----------------------------------------------------------------------------------
  //Contrat selected
  onContratSelected(raz: boolean) {
    if (raz) {
      this.contratListFC.setValue(null);
    }
    this.applicationUserContext.commandeClientFormRefContrat = this.contratListFC.value;
    //Get CommandeClients
    this.getData();
    //Show ContratEntites if applicable
    this.createContratEntitesLabel();
  }
  //-----------------------------------------------------------------------------------
  //Year selected
  onYearSelected() {
    if (this.monthListFC.value !== null && this.yearListFC.value !== null) {
      this.applicationUserContext.commandeClientFormD = moment(new Date(this.yearListFC.value as number, moment(this.monthListFC.value).toDate().getMonth(), 1, 0, 0, 0, 0));
    }
    else {
      this.applicationUserContext.commandeClientFormD = null;
    }
    //Get existing Contrat
    this.getContrat();
    //Get CommandeClients
    this.getData();
  }
  //-----------------------------------------------------------------------------------
  //Month selected
  onMonthSelected() {
    if (this.monthListFC.value !== null && this.yearListFC.value !== null) {
      this.applicationUserContext.commandeClientFormD = moment(new Date(this.yearListFC.value as number, moment(this.monthListFC.value).toDate().getMonth(), 1, 0, 0, 0, 0));
    }
    else {
      this.applicationUserContext.commandeClientFormD = null;
    }
    //Get existing Contrat
    this.getContrat();
    //Get CommandeClients
    this.getData();
  }
  //-----------------------------------------------------------------------------------
  //Get label roe ContratEntite
  createContratEntitesLabel() {
    this.contratEntitesLabel = "";
    if (this.contratList.length > 0 && this.applicationUserContext.commandeClientFormRefContrat) {
      let contrat = this.contratList.find(x => x.RefContrat === this.applicationUserContext.commandeClientFormRefContrat);
      if (contrat) {
        this.contratEntitesLabel = getContratEntitesLabel(contrat);
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for certification
  getCertificationTooltipText(i: number): string {
    return getCertficationTooltipText(this.commandeClientMensuelles[i]);
  }
}
