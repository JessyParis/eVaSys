import { Component, Inject, OnInit, Renderer2 } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, ValidationErrors, ValidatorFn, AbstractControl, FormGroupDirective, NgForm, UntypedFormArray } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { InformationComponent } from "../../dialogs/information/information.component";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import { Observable, of } from "rxjs";
import { debounceTime, switchMap } from "rxjs/operators";import moment from "moment";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { showErrorToUser, setOriginalMenu, getCreationModificationTooltipText } from "../../../globals/utils";
import { HabilitationLogistique } from "../../../globals/enums";
import { EventEmitterService } from "../../../services/event-emitter.service";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";

class MyErrorStateMatcher implements ErrorStateMatcher {
    //Constructor
    constructor() { }
    isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
        let result: boolean = false;
        result = !!((control && control.invalid));
        return result;
    }
}

class MyValidators {
    //Constructor
    constructor(public repartition: dataModelsInterfaces.Repartition) { }
    /** Total */
    poidsTotalRepartitionCollectiviteValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        let p: number = 0;
      const fARepartitionCollectivite: UntypedFormArray = control as UntypedFormArray;
        let poidsReparti: number = this.repartition.PoidsReparti ? this.repartition.PoidsReparti : 0;
        for (let i in fARepartitionCollectivite.controls) {
          let fG: UntypedFormGroup = fARepartitionCollectivite.controls[i] as UntypedFormGroup;
            p += fG.get("Poids").value;
        }
        return p !== poidsReparti ? { poidsTotalRepartitionCollectivite: true } : null;
    }
    poidsTotalRepartitionProduitValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        let p: number = 0;
      const fARepartitionProduit: UntypedFormArray = control as UntypedFormArray;
        let poids: number = this.repartition.PoidsReparti ? (this.repartition.PoidsChargement - this.repartition.PoidsReparti) : this.repartition.PoidsChargement;
        for (let i in fARepartitionProduit.controls) {
          let fG: UntypedFormGroup = fARepartitionProduit.controls[i] as UntypedFormGroup;
            p += fG.get("Poids").value;
        }
        return p !== poids ? { poidsTotalRepartitionProduit: true } : null;
    }
}

@Component({
    selector: "repartition",
    templateUrl: "./repartition.component.html",
    styleUrls: ["./repartition.component.scss"],
    standalone: false
})

export class RepartitionComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Variables
  repartition: dataModelsInterfaces.Repartition = {} as dataModelsInterfaces.Repartition;
  //Form
  form: UntypedFormGroup;
  formRepartitionCollectivite: UntypedFormGroup;
  formRepartitionProduit: UntypedFormGroup;
  enterTypeFC: UntypedFormControl = new UntypedFormControl(null);
  infoTextFC: UntypedFormControl = new UntypedFormControl(null);
  collectiviteListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  collectiviteNAListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  processListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  composantListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  poidsFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(400000)]);
  pUHTFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(2500), Validators.min(-1500)]);
  repartitionCollectivite: UntypedFormArray = new UntypedFormArray([]);
  repartitionProduit: UntypedFormArray = new UntypedFormArray([]);
  //Variables
  commandeFournisseurs: any[] = [];
  sumPoidsReparti: number;
  sumPoidsChargement: number;
  sumRepartitionCollectivite: number;
  sumRepartitionProduit: number;
  collectiviteList: dataModelsInterfaces.EntiteList[];
  collectiviteNAList: Observable<dataModelsInterfaces.EntiteList[]>;
  refCollectiviteSelected: number = null;
  processList: dataModelsInterfaces.Process[];
  composantList: dataModelsInterfaces.ProduitList[];
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = true;
  //Display
  requiredCollectiviteList: boolean = false;
  requiredCollectiviteNAList: boolean = false;
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private eventEmitterService: EventEmitterService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog
    , private renderer: Renderer2) {
    // create an empty object from the interface
    this.repartition = {} as dataModelsInterfaces.Repartition;
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      EnterType: this.enterTypeFC,
      InfoText: this.infoTextFC,
      CollectiviteList: this.collectiviteListFC,
      CollectiviteNAList: this.collectiviteNAListFC,
      ProcessList: this.processListFC,
      ComposantList: this.composantListFC,
      Poids: this.poidsFC,
      PUHT: this.pUHTFC
    });
    this.formRepartitionCollectivite = this.fb.group({
      RepartitionCollectivite: this.repartitionCollectivite
    });
    this.formRepartitionProduit = this.fb.group({
      RepartitionProduit: this.repartitionProduit
    });
    this.enterTypeFC.setValue("Collectivite");
    this.lockScreen();
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = +this.activatedRoute.snapshot.params["id"];
    let refCommandeFournisseur = this.activatedRoute.snapshot.params["refCommandeFournisseur"];
    var url = this.baseUrl + "evapi/repartition/" + id;
    //Register service to get CollectiviteNA
    this.collectiviteNAList = this.collectiviteNAListFC.valueChanges
      .pipe(
        debounceTime(300),
        switchMap(value => this.getCollectiviteNAList(value))
      );
    // get existing object if id exists
    if (id) {
      this.http.get<dataModelsInterfaces.Repartition>(url).subscribe(result => {
        //Get data
        this.repartition = result;
        this.repartitionCollectivite.setValidators(new MyValidators(this.repartition).poidsTotalRepartitionCollectiviteValidator);
        this.repartitionProduit.setValidators(new MyValidators(this.repartition).poidsTotalRepartitionProduitValidator);
        //Set EnterType if needed
        if (this.repartition.PoidsReparti === 0) {
          this.enterTypeFC.setValue("Produit");
          this.rAZEnterForm();
        }
        //Update form
        this.updateForm();
        //Get other data
        if (this.repartition.CommandeFournisseur == null) {
          this.getCommandeFournisseurs();
        }
        if (this.enterTypeFC.value === "Collectivite") {
          //Enter type = Collectivite
          this.listService.getListCollectivite(null, this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.Entite.RefEntite : this.repartition.Fournisseur.RefEntite
            , true, true, moment(this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.DDechargement : this.repartition.D)
            , "", false)
            .subscribe(result => {
              this.collectiviteList = result;
              //Auto select
              if (this.collectiviteList.length === 1) {
                this.collectiviteListFC.setValue(this.collectiviteList[0].RefEntite);
                this.refCollectiviteSelected = this.collectiviteList[0].RefEntite;
                //Get existing process  -> Composant
                this.getProcesss();
              }
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        else {
          this.listService.getListCollectivite(null, this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.Entite.RefEntite : this.repartition.Fournisseur.RefEntite
            , true, true, moment(this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.DDechargement : this.repartition.D)
            , "", false)
            .subscribe(result => {
              this.collectiviteList = result;
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Get existing process  -> Composant
          this.getProcesss();
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      this.http.get<dataModelsInterfaces.Repartition>(url,
        {
          headers: new HttpHeaders()
            .set("refCommandeFournisseur", refCommandeFournisseur)
        }).subscribe(result => {
          //Get data
          this.repartition = result;
          this.repartitionCollectivite.setValidators(new MyValidators(this.repartition).poidsTotalRepartitionCollectiviteValidator);
          this.repartitionProduit.setValidators(new MyValidators(this.repartition).poidsTotalRepartitionProduitValidator);
          //Set EnterType if needed
          if (this.repartition.PoidsReparti === 0) {
            this.enterTypeFC.setValue("Produit");
          }
          //Update form
          this.updateForm();
          //Get other data
          if (this.repartition.CommandeFournisseur == null) {
            this.getCommandeFournisseurs();
          }
          if (this.enterTypeFC.value === "Collectivite") {
            //Enter type = Collectivite
            this.listService.getListCollectivite(null, this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.Entite.RefEntite : this.repartition.Fournisseur.RefEntite
              , true, true, moment(this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.DDechargement : this.repartition.D)
              , "", false)
              .subscribe(result => {
                this.collectiviteList = result;
                //Auto select
                if (this.collectiviteList.length === 1) {
                  this.collectiviteListFC.setValue(this.collectiviteList[0].RefEntite);
                  this.refCollectiviteSelected = this.collectiviteList[0].RefEntite;
                  //Get existing process  -> Composant
                  this.getProcesss();
                }
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          }
          else {
            this.listService.getListCollectivite(null, this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.Entite.RefEntite : this.repartition.Fournisseur.RefEntite
              , true, true, moment(this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.DDechargement : this.repartition.D)
              , "", false)
              .subscribe(result => {
                this.collectiviteList = result;
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            //Get existing process  -> Composant
            this.getProcesss();
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.repartitionCollectivite.clear();
    for (const repColl of this.repartition.RepartitionCollectivites) {
      const grp = this.fb.group({
        Poids: [repColl.Poids, [Validators.required, Validators.max(400000)]],
        PUHT: [repColl.PUHT, [Validators.required, Validators.max(2500), Validators.min(-1500)]],
      });
      this.repartitionCollectivite.push(grp);
    }
    this.repartitionProduit.clear();
    for (const repColl of this.repartition.RepartitionProduits) {
      const grp = this.fb.group({
        Poids: [repColl.Poids, [Validators.required, Validators.max(400000)]],
        PUHT: [repColl.PUHT, [Validators.required, Validators.max(2500), Validators.min(-1500)]],
      });
      this.repartitionProduit.push(grp);
    }
    this.infoTextFC.setValue(this.repartition.InfoText);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.collectiviteListFC.disable();
    this.collectiviteNAListFC.disable();
    this.processListFC.disable();
    this.composantListFC.disable();
    this.poidsFC.disable();
    this.pUHTFC.disable();
    this.formRepartitionCollectivite.disable();
    this.formRepartitionProduit.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.collectiviteListFC.enable();
    this.collectiviteNAListFC.enable();
    this.processListFC.enable();
    this.composantListFC.enable();
    this.poidsFC.enable();
    this.pUHTFC.enable();
    this.formRepartitionCollectivite.enable();
    this.formRepartitionProduit.enable();
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
      this.requiredCollectiviteList = false;
      this.requiredCollectiviteNAList = false;
      this.collectiviteListFC.clearValidators();
      this.collectiviteListFC.updateValueAndValidity();
      this.collectiviteNAListFC.clearValidators();
      this.collectiviteNAListFC.updateValueAndValidity();
      if (this.collectiviteListFC.value) { this.collectiviteNAListFC.disable(); }
      if (this.collectiviteNAListFC.value) { this.collectiviteListFC.disable(); }
      //Process
      if (this.enterTypeFC.value === "Collectivite") {
        this.requiredCollectiviteNAList = true;
        this.requiredCollectiviteList = true;
        this.collectiviteListFC.setValidators(Validators.required);
        this.collectiviteListFC.setValidators(Validators.required);
        this.collectiviteNAListFC.updateValueAndValidity();
        this.collectiviteNAListFC.updateValueAndValidity();
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Get commandeFournisseurs
  getCommandeFournisseurs() {
    this.commandeFournisseurs = [];
    this.commandeFournisseurs.push({ NumeroCommande: null, PoidsReparti: null, PoidsChargement: null })
    if (this.repartition.CommandeFournisseur == null
      && this.repartition.D && this.repartition.Produit && this.repartition.Fournisseur) {
      //Get data
      var url = this.baseUrl + "evapi/repartition/getcommandefournisseurs";
      this.http.get<any[]>(url, {
        headers: new HttpHeaders()
          .set("d", moment(this.repartition.D).format("YYYY-MM-DD 00:00:00.000"))
          .set("refProduit", this.repartition.Produit.RefProduit.toString())
          .set("refFournisseur", this.repartition.Fournisseur.RefEntite.toString()),
        responseType: "json"
      }).subscribe(result => {
        this.commandeFournisseurs = result;
        this.sumPoidsChargement = this.commandeFournisseurs.reduce((sum, c) => sum + c.PoidsChargement, 0)
        this.sumPoidsReparti = this.commandeFournisseurs.reduce((sum, c) => sum + c.PoidsReparti, 0)
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Get sumRepartitionCollectivite
  getSumRepartitionCollectivite(): number {
    let r: number = 0;
    for (let i in this.repartitionCollectivite.controls) {
      let fG: UntypedFormGroup = this.repartitionCollectivite.controls[i] as UntypedFormGroup;
      r += fG.get("Poids").value;
    }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Get sumRepartitionProduit
  getSumRepartitionProduit(): number {
    let r: number = 0;
    for (let i in this.repartitionProduit.controls) {
      let fG: UntypedFormGroup = this.repartitionProduit.controls[i] as UntypedFormGroup;
      r += fG.get("Poids").value;
    }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Get data for IHM
  get getFormRepartitionCollectiviteData(): UntypedFormArray {
    return this.repartitionCollectivite as UntypedFormArray;
  }
  get getFormRepartitionProduitData(): UntypedFormArray {
    return this.repartitionProduit as UntypedFormArray;
  }
  //-----------------------------------------------------------------------------------
  //Get non affected Collectivite
  getCollectiviteNAList(value: string) {
    if (value && value.length > 1) {
      return this.listService.getListCollectivite(null, null
        , null, true, moment(this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.DDechargement : this.repartition.D)
        , value ? value : "", false);
    }
    else {
      let r: dataModelsInterfaces.EntiteList[] = [];
      return of(r);
    }
  }
  //Get Processs
  getProcesss() {
    //Get existing process  
    this.listService.getListProcess(null, this.refCollectiviteSelected)
      .subscribe(result => {
        this.processList = result;
        //Auto select
        if (this.processList.length === 1) {
          this.processListFC.setValue(this.processList[0].RefProcess);
          //Get existing Composant
          this.getComposants();
        }
        else {
          this.composantList = [];
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Get Composants
  getComposants() {
    //Get existing Composant
    this.listService.getListProduit(null, null
      , this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.Produit.RefProduit : this.repartition.Produit.RefProduit
      , this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.Produit.RefProduit : this.repartition.Produit.RefProduit
      , moment(this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.DDechargement : this.repartition.D)
      , this.processListFC.value, null, false, true)
      .subscribe(result => {
        this.composantList = result;
        //Auto select
        if (this.composantList.length === 1) {
          this.composantListFC.setValue(this.composantList[0].RefProduit);
          this.getPrixReprise();
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Get PrixReprise
  getPrixReprise() {
    //Get existing PrixReprise
    this.dataModelService.getPrixReprise(0, this.processListFC.value
      , this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.Produit.RefProduit : this.repartition.Produit.RefProduit
      , this.composantListFC.value
      , null
      , moment(this.repartition.CommandeFournisseur ? this.repartition.CommandeFournisseur.DDechargement : this.repartition.D)
    )
      .subscribe(result => {
        this.pUHTFC.setValue(result.PUHT - result.PUHTSurtri - result.PUHTTransport);
        const element = this.renderer.selectRootElement("#inputPoids");
        setTimeout(() => element.focus(), 0);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Collectivite selected
  onCollectiviteSelected(selectedRefEntite: number) {
    if (selectedRefEntite == null) {
      this.collectiviteListFC.setValue(null);
      this.collectiviteNAListFC.setValue(null);
      this.refCollectiviteSelected = null;
      if (this.enterTypeFC.value === "Collectivite") {
        this.processListFC.setValue(null);
        this.processList = [];
        this.composantListFC.setValue(null);
        this.composantList = [];
        this.getProcesss();
      }
      else {
        //Focus Poids if RepartitionProduit
        const element = this.renderer.selectRootElement("#inputPoids");
        setTimeout(() => element.focus(), 0);
      }
    }
    else {
      this.refCollectiviteSelected = selectedRefEntite;
      if (this.enterTypeFC.value === "Collectivite") {
        this.getProcesss();
      }
      else {
        //Focus Poids if RepartitionProduit
        const element = this.renderer.selectRootElement("#inputPoids");
        setTimeout(() => element.focus(), 0);
      }
    }
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //CollectiviteNA selected 
  onCollectiviteNASelected() {
    this.onCollectiviteSelected(this.collectiviteNAListFC.value ? this.collectiviteNAListFC.value.RefEntite : null);
  }
  //-----------------------------------------------------------------------------------
  //Change enter type
  onEnterTypeChange() {
    this.rAZEnterForm();
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Process selected
  onProcessSelected() {
    //Get Composant
    this.getComposants();
  }
  //-----------------------------------------------------------------------------------
  //Composant selected
  onComposantSelected() {
    //Get price
    this.getPrixReprise();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    for (let i in this.repartitionCollectivite.controls) {
      let fG: UntypedFormGroup = this.repartitionCollectivite.controls[i] as UntypedFormGroup;
      this.repartition.RepartitionCollectivites[i].Poids = fG.get("Poids").value;
      this.repartition.RepartitionCollectivites[i].PUHT = fG.get("PUHT").value;
    }
    for (let i in this.repartitionProduit.controls) {
      let fG: UntypedFormGroup = this.repartitionProduit.controls[i] as UntypedFormGroup;
      this.repartition.RepartitionProduits[i].Poids = fG.get("Poids").value;
      this.repartition.RepartitionProduits[i].PUHT = fG.get("PUHT").value;
    }
  }
  //-----------------------------------------------------------------------------------
  //Add a new RepartitionCollectivite
  addRepartitionCollectivite() {
    if (this.refCollectiviteSelected
      && this.processListFC.value && this.composantListFC.value && this.poidsFC.value && this.pUHTFC.value !== null) {
      //Save data
      this.saveData();
      //Process
      let rC = {} as dataModelsInterfaces.RepartitionCollectivite;
      rC.RefRepartitionCollectivite = 0;
      rC.RefRepartition = this.repartition.RefRepartition;
      //Get collectivite
      this.dataModelService.getEntite(this.refCollectiviteSelected, null, null)
        .subscribe(result => {
          rC.Collectivite = result;
          //Get composant
          this.dataModelService.getProduit(this.composantListFC.value)
            .subscribe(result => {
              rC.Produit = result;
              //Get Process
              this.dataModelService.getProcess(this.processListFC.value)
                .subscribe(result => {
                  rC.Process = result;
                  rC.Poids = this.poidsFC.value;
                  rC.PUHT = this.pUHTFC.value;
                  this.repartition.RepartitionCollectivites.push(rC);
                  //Update form
                  this.updateForm();
                  //RAZ enter form
                  this.rAZEnterForm();
                }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a RepartitionCollectivite
  onDeleteRepartitionCollectivite(i: number) {
    if (i !== null) {
      //Save data
      this.saveData();
      //Process
      this.repartition.RepartitionCollectivites.splice(i, 1);
      //Update form
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //Add a new RepartitionProduit
  addRepartitionProduit() {
    if (this.processListFC.value && this.composantListFC.value && this.poidsFC.value && this.pUHTFC.value !== null) {
      //Save data
      this.saveData();
      //Process
      let rC = {} as dataModelsInterfaces.RepartitionProduit;
      rC.RefRepartitionProduit = 0;
      rC.RefRepartition = this.repartition.RefRepartition;
      //Get composant
      this.dataModelService.getProduit(this.composantListFC.value)
        .subscribe(result => {
          rC.Produit = result;
          //Get Process
          this.dataModelService.getProcess(this.processListFC.value)
            .subscribe(result => {
              rC.Process = result;
              //Get collectivite
              if (this.refCollectiviteSelected) {
                this.dataModelService.getEntite(this.refCollectiviteSelected, null, null)
                  .subscribe(result => {
                    rC.Fournisseur = result;
                    rC.Poids = this.poidsFC.value;
                    rC.PUHT = this.pUHTFC.value;
                    this.repartition.RepartitionProduits.push(rC);
                    //Update form
                    this.updateForm();
                    //RAZ enter form
                    this.rAZEnterForm();
                  }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
              }
              else {
                rC.Poids = this.poidsFC.value;
                rC.PUHT = this.pUHTFC.value;
                this.repartition.RepartitionProduits.push(rC);
                //Update form
                this.updateForm();
                //RAZ enter form
                this.rAZEnterForm();
              }
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a RepartitionProduit
  onDeleteRepartitionProduit(i: number) {
    if (i !== null) {
      //Save data
      this.saveData();
      //Process
      this.repartition.RepartitionProduits.splice(i, 1);
      //Update form
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //RAZ enter form
  rAZEnterForm(): void {
    this.collectiviteListFC.setValue(null);
    this.collectiviteNAListFC.setValue(null);
    this.refCollectiviteSelected = null;
    this.processListFC.setValue(null);
    this.processList = [];
    this.getProcesss();
    if (this.enterTypeFC.value === "Produit") {
      this.processListFC.setValue(5);
      this.getComposants();
    }
    this.composantListFC.setValue(null);
    this.composantList = [];
    this.poidsFC.setValue(null);
    this.pUHTFC.setValue(null);
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(630) },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    var url = this.baseUrl + "evapi/repartition/" + this.repartition.RefRepartition;
    this.http
      .delete(url)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(775), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Set original Menu
        setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
        //Route back to grid
        this.router.navigate(["grid"]);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Saves the line
  onSubmit() {
    let ok: boolean = false;
    this.saveData();
    //Add new RepartitionCollectivite
    if (this.enterTypeFC.value === "Collectivite") {
      ok = (this.repartition.RepartitionCollectivites.filter(e =>
        e.Collectivite.RefEntite === this.refCollectiviteSelected && e.Process.RefProcess === this.processListFC.value && e.Produit.RefProduit === this.composantListFC.value).length === 0);
      if (ok) {
        this.addRepartitionCollectivite();
      }
      else {
        const dialogRef = this.dialog.open(InformationComponent, {
          width: "350px",
          data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(610) },
          restoreFocus: false
        });
      }
    }
    else {
      //Add new RepartitionProduit
      if (this.refCollectiviteSelected == null) {
        ok = (this.repartition.RepartitionProduits.filter(e =>
          e.Fournisseur == null && e.Process.RefProcess === this.processListFC.value && e.Produit.RefProduit === this.composantListFC.value).length === 0);
      }
      else {
        ok = (this.repartition.RepartitionProduits.filter(e =>
          (e.Fournisseur ? e.Fournisseur.RefEntite : null) === this.refCollectiviteSelected && e.Process.RefProcess === this.processListFC.value && e.Produit.RefProduit === this.composantListFC.value).length === 0);
      }
      if (ok) {
        this.addRepartitionProduit();
      }
      else {
        const dialogRef = this.dialog.open(InformationComponent, {
          width: "350px",
          data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(612) },
          restoreFocus: false
        });
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  saveRepartition() {
    this.saveData();
    //Process
    var url = this.baseUrl + "evapi/repartition";
    //Update 
    this.http
      .post<dataModelsInterfaces.Repartition>(url, this.repartition)
      .subscribe(result => {
        this.repartition = result;
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Set original Menu
        setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
        //Route back to grid
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Go to CommandeFournisseur
  onCommandeFournisseur() {
    if (!this.locked) {
      this.saveData();
      //Process
      var url = this.baseUrl + "evapi/repartition";
      //Update 
      this.http
        .post<dataModelsInterfaces.Repartition>(url, this.repartition)
        .subscribe(result => {
          this.repartition = result;
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
          //Save menu
          if (this.applicationUserContext.fromMenu == null) {
            this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
          }
          //Set original Menu
          setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
          //Route
          this.router.navigate(["commande-fournisseur", this.repartition.CommandeFournisseur.RefCommandeFournisseur.toString()]);
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Save menu
      if (this.applicationUserContext.fromMenu == null) {
        this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
      }
      //Set original Menu
      setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
      //Route
      this.router.navigate(["commande-fournisseur", this.repartition.CommandeFournisseur.RefCommandeFournisseur.toString()]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Autocomplete display
  autoCollectiviteNADisplayFn(coll?: dataModelsInterfaces.EntiteList): string | undefined {
    return coll ? coll.Libelle : undefined;
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    //Set original Menu
    setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
    //Route back to grid
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.repartition);
  }
}
