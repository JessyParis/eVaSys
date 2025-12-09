import { Component } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, ValidationErrors, ValidatorFn, AbstractControl, UntypedFormArray, FormControl } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { InformationComponent } from "../../dialogs/information/information.component";
import { ListService } from "../../../services/list.service";
import { UtilsService } from "../../../services/utils.service";
import { DataModelService } from "../../../services/data-model.service";
import { Observable, of } from "rxjs";
import { debounceTime, switchMap } from "rxjs/operators";import moment from "moment";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { showErrorToUser, setOriginalMenu, getCreationModificationTooltipText, toFixed, getFormattedNumeroCommande, cmp } from "../../../globals/utils";
import { HabilitationLogistique, SnackbarMsgType } from "../../../globals/enums";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { BaseFormComponent } from "../../_ancestors/base-form.component";
import { DomSanitizer } from "@angular/platform-browser";
import { EventEmitterService } from "../../../services/event-emitter.service";

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

export class RepartitionComponent extends BaseFormComponent<dataModelsInterfaces.Repartition> {
  //Variablesf
  repartition: dataModelsInterfaces.Repartition = {} as dataModelsInterfaces.Repartition;
  //Form
  form: UntypedFormGroup;
  formRepartitionCollectivite: UntypedFormGroup;
  formRepartitionProduit: UntypedFormGroup;
  exportSAGEFC: UntypedFormControl = new UntypedFormControl(null);
  enterTypeFC: UntypedFormControl = new UntypedFormControl(null);
  enterModeFC = new FormControl();
  infoTextFC: UntypedFormControl = new UntypedFormControl(null);
  collectiviteListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  collectiviteNAListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  poidsFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(400000), Validators.min(1)]);
  percentFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(100), Validators.min(0.0001)]);
  pUHTFC: UntypedFormControl = new UntypedFormControl(null, [Validators.max(2500), Validators.min(-1500)]);
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
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = true;
  //Display
  requiredCollectiviteList: boolean = false;
  requiredCollectiviteNAList: boolean = false;
  requiredPrixReprise: boolean = false;
  visibleRepartitionCollectivite: boolean = false;
  visibleRepartitionProduit: boolean = false;
  //Functions
  toFixed = toFixed;
  getFormattedNumeroCommande = getFormattedNumeroCommande;
  constructor(protected activatedRoute: ActivatedRoute
    , protected router: Router
    , private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected utilsService: UtilsService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
    , protected sanitizer: DomSanitizer
    , protected eventEmitterService: EventEmitterService
  ) {

    super("RepartitionComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    // create an empty object from the interface
    this.repartition = {} as dataModelsInterfaces.Repartition;
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      ExportSAGE: this.exportSAGEFC,
      EnterType: this.enterTypeFC,
      InfoText: this.infoTextFC,
      CollectiviteList: this.collectiviteListFC,
      CollectiviteNAList: this.collectiviteNAListFC,
      Poids: this.poidsFC,
      Percent: this.percentFC,
      PUHT: this.pUHTFC
    });
    this.formRepartitionCollectivite = this.fb.group({
      RepartitionCollectivite: this.repartitionCollectivite
    });
    this.formRepartitionProduit = this.fb.group({
      RepartitionProduit: this.repartitionProduit
    });
    this.enterTypeFC.setValue("Collectivite");
    this.enterModeFC.setValue("kg");
    this.lockScreen();
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = +this.activatedRoute.snapshot.params["id"];
    let refCommandeFournisseur = this.activatedRoute.snapshot.params["refCommandeFournisseur"];
    //Register service to get CollectiviteNA
    this.collectiviteNAList = this.collectiviteNAListFC.valueChanges
      .pipe(
        debounceTime(300),
        switchMap(value => this.getCollectiviteNAList(value))
      );
    // get existing object if id exists
    if (id) {
      this.dataModelService.getRepartition(id, 0).subscribe(result => {
        //Get data
        this.repartition = result;
        this.repartition.RepartitionCollectivites.sort(function (a, b) { return cmp(a.RefRepartitionCollectivite, b.RefRepartitionCollectivite); });
        this.repartition.RepartitionProduits.sort(function (a, b) { return cmp(a.RefRepartitionProduit, b.RefRepartitionProduit); });
        this.repartitionCollectivite.setValidators(new MyValidators(this.repartition).poidsTotalRepartitionCollectiviteValidator);
        this.repartitionProduit.setValidators(new MyValidators(this.repartition).poidsTotalRepartitionProduitValidator);
        //Set EnterType if needed
        if (this.repartition.PoidsReparti === 0) {
          this.enterTypeFC.setValue("Produit");
          this.rAZEnterForm();
        }
        //Update form
        this.updateForm();
        if (this.enterTypeFC.value === "Collectivite") {
          //Enter type = Collectivite
          this.listService.getListCollectivite(null, this.repartition.CommandeFournisseur.Entite.RefEntite
            , true, true, moment(this.repartition.CommandeFournisseur.DDechargement)
            , "", true)
            .subscribe(result => {
              this.collectiviteList = result;
              //Auto select
              if (this.collectiviteList.length === 1) {
                this.collectiviteListFC.setValue(this.collectiviteList[0].RefEntite);
                this.refCollectiviteSelected = this.collectiviteList[0].RefEntite;
              }
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        else {
          this.listService.getListCollectivite(null, this.repartition.CommandeFournisseur.Entite.RefEntite
            , true, true, moment(this.repartition.CommandeFournisseur.DDechargement)
            , "", true)
            .subscribe(result => {
              this.collectiviteList = result;
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        //GetPrixReprise
        this.getPrixReprise();
        //Get missing PrixReprise if applicable
        this.getMissingPrixReprises()
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      this.dataModelService.getRepartition(id, refCommandeFournisseur).subscribe(result => {
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
        //GetPrixReprise
        this.getPrixReprise();
        //Check similar
        this.checkSimilar();
        //Get other data
        if (this.enterTypeFC.value === "Collectivite") {
          //Enter type = Collectivite
          this.listService.getListCollectivite(null, this.repartition.CommandeFournisseur.Entite.RefEntite
            , true, true, moment(this.repartition.CommandeFournisseur.DDechargement)
            , "", false)
            .subscribe(result => {
              this.collectiviteList = result;
              //Auto select
              if (this.collectiviteList.length === 1) {
                this.collectiviteListFC.setValue(this.collectiviteList[0].RefEntite);
                this.refCollectiviteSelected = this.collectiviteList[0].RefEntite;
              }
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        else {
          this.listService.getListCollectivite(null, this.repartition.CommandeFournisseur.Entite.RefEntite
            , true, true, moment(this.repartition.CommandeFournisseur.DDechargement)
            , "", false)
            .subscribe(result => {
              this.collectiviteList = result;
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.exportSAGEFC.setValue(this.repartition.ExportSAGE);
    this.infoTextFC.setValue(this.repartition.InfoText);
    this.repartitionCollectivite.clear();
    for (const repColl of this.repartition.RepartitionCollectivites) {
      const grp = this.fb.group({
        Poids: [repColl.Poids, [Validators.required, Validators.max(400000), Validators.min(1)]],
        PUHT: [repColl.PUHT, [Validators.max(2500), Validators.min(-1500)]],
      });
      this.repartitionCollectivite.push(grp);
    }
    this.repartitionProduit.clear();
    for (const repColl of this.repartition.RepartitionProduits) {
      const grp = this.fb.group({
        Poids: [repColl.Poids, [Validators.required, Validators.max(400000), Validators.min(1)]],
        PUHT: [repColl.PUHT, [Validators.max(2500), Validators.min(-1500)]],
      });
      this.repartitionProduit.push(grp);
    }
    //Calculate percentages
    this.calculatePercentages();
    //Message if 100% and Poids incorrect
    if (this.enterModeFC.value == "percent") {
      let totalPercentCollectivite: number = this.repartition.RepartitionCollectivites.reduce((a, b) => a + (b["Percentage"] || 0), 0);
      let totalPercentProduit: number = this.repartition.RepartitionProduits.reduce((a, b) => a + (b["Percentage"] || 0), 0);
      let totalpoidsCollectivite: number = this.repartition.RepartitionCollectivites.reduce((a, b) => a + (b["Poids"] || 0), 0);
      let totalpoidsProduit: number = this.repartition.RepartitionProduits.reduce((a, b) => a + (b["Poids"] || 0), 0);
      if (totalPercentCollectivite === 100 && totalpoidsCollectivite !== this.repartition.PoidsReparti
        || totalPercentProduit === 100 && totalpoidsProduit !== (this.repartition.PoidsChargement - this.repartition.PoidsReparti)) {
        const dialogRef = this.dialog.open(InformationComponent, {
          width: "350px",
          data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(1562) },
          restoreFocus: false
        });
      }
    }
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Calculate percentages
  calculatePercentages() {
    for (const repColl of this.repartition.RepartitionCollectivites) {
      repColl.Percentage = Math.round((repColl.Poids / this.repartition.PoidsReparti) * 10000) / 100;
    }
    for (const repColl of this.repartition.RepartitionProduits) {
      repColl.Percentage = Math.round((repColl.Poids / (this.repartition.PoidsChargement - this.repartition.PoidsReparti)) * 10000) / 100;
    }
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.exportSAGEFC.disable();
    this.collectiviteListFC.disable();
    this.collectiviteNAListFC.disable();
    this.poidsFC.disable();
    this.percentFC.disable();
    this.pUHTFC.disable();
    this.formRepartitionCollectivite.disable();
    this.formRepartitionProduit.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.saveLocked = false;
    this.exportSAGEFC.enable();
    this.collectiviteListFC.enable();
    this.collectiviteNAListFC.enable();
    this.poidsFC.enable();
    this.percentFC.enable();
    this.pUHTFC.enable();
    this.formRepartitionCollectivite.enable();
    this.formRepartitionProduit.enable();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Global lock
    if (
        (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Administrateur
          && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.CentreDeTri)
        || (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.CentreDeTri
          && !this.repartition.CommandeFournisseur.Produit.Collecte)
        || this.repartition.ExportSAGE
      )
    {
      this.lockScreen();
      //Unlock ExportSAGE for admins
      if( this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur) {
        this.exportSAGEFC.enable();
        this.saveLocked = false;
      }
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
      //If CDT, no HCS
      if (this.applicationUserContext.connectedUtilisateur.CentreDeTri) {
        this.exportSAGEFC.disable();
        this.formRepartitionProduit.disable();
        this.enterTypeFC.value == "Collectivite"
      }
      //Process
      if (this.enterTypeFC.value === "Collectivite") {
        this.requiredCollectiviteNAList = true;
        this.requiredCollectiviteList = true;
        this.collectiviteListFC.setValidators(Validators.required);
        this.collectiviteListFC.updateValueAndValidity();
        this.collectiviteNAListFC.setValidators(Validators.required);
        this.collectiviteNAListFC.updateValueAndValidity();
      }
      if (this.enterModeFC.value === "kg") {
        this.percentFC.disable();
        this.poidsFC.enable();
      }
      else {
        this.percentFC.enable();
        this.poidsFC.disable();
      }
    }
    //Visibility
    this.visibleRepartitionCollectivite = (this.repartition.RepartitionCollectivites.length > 0
      || this.repartition.CommandeFournisseur.PoidsReparti > 0);
    this.visibleRepartitionProduit = (this.repartition.RepartitionProduits.length > 0
      || this.repartition.CommandeFournisseur.PoidsReparti < this.repartition.CommandeFournisseur.PoidsChargement)
      && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur;
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
        , null, true, moment(this.repartition.CommandeFournisseur.DDechargement)
        , value ? value : "", true);
    }
    else {
      let r: dataModelsInterfaces.EntiteList[] = [];
      return of(r);
    }
  }
  //-----------------------------------------------------------------------------------
  //Get PrixReprise
  getPrixReprise() {
    if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
      && this.repartition.CommandeFournisseur?.DDechargement) {
      //Get existing PrixReprise
      this.dataModelService.getPrixReprise(0, null
        , this.repartition.CommandeFournisseur.Produit.RefProduit
        , null
        , this.collectiviteListFC.value ? this.collectiviteListFC.value : this.collectiviteNAListFC.value
        , moment(this.repartition.CommandeFournisseur.DDechargement)
      )
        .subscribe(result => {
          this.pUHTFC.setValue(result.PUHT - result.PUHTSurtri - result.PUHTTransport);
        }, error =>
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1550), duration: 4000 } as appInterfaces.SnackbarMsg));
    }
  }
  //-----------------------------------------------------------------------------------
  //Collectivite selected
  onCollectiviteSelected(selectedRefEntite: number) {
    if (selectedRefEntite == null) {
      this.collectiviteListFC.setValue(null);
      this.collectiviteNAListFC.setValue(null);
      this.refCollectiviteSelected = null;
    }
    else {
      this.refCollectiviteSelected = selectedRefEntite;
      //Get PrixReprise
      this.getPrixReprise();
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
  //Update the data model
  saveData() {
    this.repartition.ExportSAGE = this.exportSAGEFC.value;
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
      && ((this.poidsFC.value && this.enterModeFC.value === "kg")
        || (this.percentFC.value && this.enterModeFC.value === "percent"))
    ) {
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
          //Poids
          if (this.enterModeFC.value === "kg") { rC.Poids = this.poidsFC.value; }
          else {
            rC.Poids = Math.round(this.repartition.PoidsReparti * this.percentFC.value / 100);
          }
          rC.PUHT = this.pUHTFC.value;
          this.repartition.RepartitionCollectivites.push(rC);
          //Update form
          this.updateForm();
          //RAZ enter form
          this.rAZEnterForm();
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
    if ((this.poidsFC.value && this.enterModeFC.value === "kg")
      || (this.percentFC.value && this.enterModeFC.value === "percent")
    ) {
      //Save data
      this.saveData();
      //Process
      let rC = {} as dataModelsInterfaces.RepartitionProduit;
      rC.RefRepartitionProduit = 0;
      rC.RefRepartition = this.repartition.RefRepartition;
      if (this.enterModeFC.value === "kg") { rC.Poids = this.poidsFC.value; }
      else {
        rC.Poids = Math.round((this.repartition.PoidsChargement - this.repartition.PoidsReparti) * this.percentFC.value / 100);
      }
      rC.PUHT = this.pUHTFC.value;
      //Get collectivite
      if (this.refCollectiviteSelected) {
        this.dataModelService.getEntite(this.refCollectiviteSelected, null, null)
          .subscribe(result => {
            rC.Fournisseur = result;
            this.repartition.RepartitionProduits.push(rC);
            //Update form
            this.updateForm();
            //RAZ enter form
            this.rAZEnterForm();
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
      else {
        this.repartition.RepartitionProduits.push(rC);
        //Update form
        this.updateForm();
        //RAZ enter form
        this.rAZEnterForm();
      }
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
    this.percentFC.setValue(null);
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
    this.dataModelService.deleteRepartition(this.repartition.RefRepartition)
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
        e.Collectivite.RefEntite === this.refCollectiviteSelected).length === 0);
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
        ok = (this.repartition.RepartitionProduits.filter(e => e.Fournisseur == null).length === 0);
      }
      else {
        ok = (this.repartition.RepartitionProduits.filter(e => (e.Fournisseur ? e.Fournisseur.RefEntite : null) === this.refCollectiviteSelected).length === 0);
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
    this.postRepartition("save");
  }
  //-----------------------------------------------------------------------------------
  //Go to CommandeFournisseur
  onCommandeFournisseur() {
    if (!this.locked) {
      this.saveData();
      //Check validity
      if ((this.formRepartitionCollectivite.get('RepartitionCollectivite').invalid && this.formRepartitionCollectivite.get('RepartitionCollectivite').touched)
        || (this.formRepartitionProduit.get('RepartitionProduit').invalid && this.formRepartitionProduit.get('RepartitionProduit').touched)) {
        const dialogRef = this.dialog.open(ConfirmComponent, {
          width: "350px",
          data: { title: this.applicationUserContext.getCulturedRessourceText(1570), message: this.applicationUserContext.getCulturedRessourceText(1571) },
          autoFocus: false,
          restoreFocus: false
        });
        //After closed
        dialogRef.afterClosed().subscribe(result => {
          if (result === "yes") {
            this.backToCommandeFournisseur();
          }
        });
      }
      else {
        //Save repartition if valid and untouched
        if (!this.formRepartitionCollectivite.get('RepartitionCollectivite').touched
          && !this.formRepartitionProduit.get('RepartitionProduit').touched) {
          this.backToCommandeFournisseur();
        }
        else {
          this.postRepartition("gotoCommandeFournisseur");
        }
      }
    }
    else {
      this.backToCommandeFournisseur();
    }
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  postRepartition(action: string) {
    //Process
    this.dataModelService.postRepartition(this.repartition)
      .subscribe(result => {
        this.repartition = result;
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
        //Next action
        switch (action) {
          case "save":
            //Set original Menu
            setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
            //Route back to grid
            this.router.navigate(["grid"]);
            break;
          case "gotoCommandeFournisseur":
            this.backToCommandeFournisseur()
            break;
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Back to CommandeFournisseur
  backToCommandeFournisseur() {
    //Save menu
    if (this.applicationUserContext.fromMenu == null) {
      this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
    }
    //Set original Menu
    setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
    //Route
    this.router.navigate(["commande-fournisseur", this.repartition.CommandeFournisseur.RefCommandeFournisseur.toString()]);
  }
  //-----------------------------------------------------------------------------------
  //Autocomplete display
  autoCollectiviteNADisplayFn(coll?: dataModelsInterfaces.EntiteList): string | undefined {
    return coll ? coll.Libelle : undefined;
  }
  //-----------------------------------------------------------------------------------
  //Check Repartition for same product in the last year
  checkSimilar() {
    //If new Repartition
    if (!this.repartition.RefRepartition && this.repartition.CommandeFournisseur?.RefCommandeFournisseur && !this.locked) {
      this.dataModelService.isSimilarRepartition(this.repartition.CommandeFournisseur.RefCommandeFournisseur, moment(this.repartition.CommandeFournisseur.DChargement))
        .subscribe((result => {
          if (result) {
            let repartitionSimilar = result;
            repartitionSimilar.RepartitionCollectivites.sort(function (a, b) { return cmp(a.RefRepartitionCollectivite, b.RefRepartitionCollectivite); });
            repartitionSimilar.RepartitionProduits.sort(function (a, b) { return cmp(a.RefRepartitionProduit, b.RefRepartitionProduit); });
            let msg: string;
            msg = this.applicationUserContext.getCulturedRessourceText(1560);
            const dialogRef = this.dialog.open(ConfirmComponent, {
              width: "350px",
              data: {
                title: this.applicationUserContext.getCulturedRessourceText(337), message: "", htmlMessage: msg
              },
              restoreFocus: false
            });
            dialogRef.afterClosed().subscribe(result => {
              if (result === "yes") {
                //Apply similar to RepartitionCollectivites
                if (repartitionSimilar.RepartitionCollectivites.length > 0) {
                  repartitionSimilar.RepartitionCollectivites.forEach(e => {
                    let f: dataModelsInterfaces.RepartitionCollectivite = {
                      RefRepartitionCollectivite: 0,
                      RefRepartition: this.repartition.RefRepartition,
                      Collectivite: e.Collectivite,
                      Poids: Math.round(this.repartition.CommandeFournisseur.PoidsReparti * (e.Poids / repartitionSimilar.CommandeFournisseur.PoidsReparti)),
                      PUHT: null,
                      Percentage: null,
                      Process: null,
                      Produit: null,
                    };
                    this.repartition.RepartitionCollectivites.push(f);
                  });
                  //Adjust to total
                  let total: number = this.repartition.RepartitionCollectivites.reduce((a, b) => a + (b["Poids"] || 0), 0);
                  let diff: number = this.repartition.PoidsReparti - total;
                  if (diff !== 0) {
                    this.repartition.RepartitionCollectivites[0].Poids += diff;
                  }
                }
                //Apply similar to RepartitionProduits
                if (repartitionSimilar.RepartitionProduits.length > 0) {
                  repartitionSimilar.RepartitionProduits.forEach(e => {
                    let f: dataModelsInterfaces.RepartitionProduit = {
                      RefRepartitionProduit: 0,
                      RefRepartition: this.repartition.RefRepartition,
                      Fournisseur: e.Fournisseur,
                      Poids: Math.round((this.repartition.CommandeFournisseur.PoidsChargement - this.repartition.CommandeFournisseur.PoidsReparti) * (e.Poids / (repartitionSimilar.CommandeFournisseur.PoidsReparti - repartitionSimilar.CommandeFournisseur.PoidsChargement))),
                      PUHT: null,
                      Percentage: null,
                      Process: null,
                      Produit: null,
                    };
                    this.repartition.RepartitionProduits.push(f);
                  });
                  //Adjust to total
                  let total: number = this.repartition.RepartitionProduits.reduce((a, b) => a + (b["Poids"] || 0), 0);
                  let diff: number = (this.repartition.PoidsChargement - this.repartition.PoidsReparti) - total;
                  if (diff !== 0) {
                    this.repartition.RepartitionProduits[0].Poids += diff;
                  }
                }
                //Update form
                this.updateForm();
                //get missing PrixReprise if applicable
                this.getMissingPrixReprises();
              }
            });
          }
        }));
    }
  }
  //-----------------------------------------------------------------------------------
  //Get missing PrixReprise if applicable
  getMissingPrixReprises() {
    if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
      && this.repartition.CommandeFournisseur?.DDechargement) {
      let missingPrixReprise: boolean = false;
      let missingPrixRepriseMsgCreated: boolean = false;
      let foundPrixReprise: boolean = false;
      let foundPrixRepriseMsgCreated: boolean = false;
      if (this.repartition.RepartitionCollectivites.some(e => e.PUHT == null)) {
        this.repartition.RepartitionCollectivites.forEach(e => {
          if (e.PUHT == null) {
            this.dataModelService.getPrixReprise(0, null
              , this.repartition.CommandeFournisseur.Produit.RefProduit
              , null
              , e.Collectivite.RefEntite
              , moment(this.repartition.CommandeFournisseur.DDechargement)
            )
              .subscribe(result => {
                e.PUHT = result.PUHT - result.PUHTSurtri - result.PUHTTransport;
                foundPrixReprise = true;
                //Update form
                this.updateForm();
                if (foundPrixReprise && !foundPrixRepriseMsgCreated) {
                  this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1556), duration: 4000 } as appInterfaces.SnackbarMsg);
                  foundPrixRepriseMsgCreated = true;
                }
              }, error => {
                missingPrixReprise = true
                if (missingPrixReprise && !missingPrixRepriseMsgCreated) {
                  this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1555), duration: 4000 } as appInterfaces.SnackbarMsg);
                  missingPrixRepriseMsgCreated = true;
                }
              });
          }
        });
      }
      if (this.repartition.RepartitionProduits.some(e => e.PUHT == null)) {
        this.repartition.RepartitionProduits.forEach(e => {
          if (e.PUHT == null) {
            this.dataModelService.getPrixReprise(0, null
              , this.repartition.CommandeFournisseur.Produit.RefProduit
              , null
              , e.Fournisseur?.RefEntite
              , moment(this.repartition.CommandeFournisseur.DDechargement)
            )
              .subscribe(result => {
                e.PUHT = result.PUHT - result.PUHTSurtri - result.PUHTTransport;
                foundPrixReprise = true;
                //Update form
                this.updateForm();
                if (foundPrixReprise && !foundPrixRepriseMsgCreated) {
                  this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1556), duration: 4000 } as appInterfaces.SnackbarMsg);
                  foundPrixRepriseMsgCreated = true;
                }
              }, error => {
                missingPrixReprise = true
                if (missingPrixReprise && !missingPrixRepriseMsgCreated) {
                  this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1555), duration: 4000 } as appInterfaces.SnackbarMsg);
                  missingPrixRepriseMsgCreated = true;
                }
              });
          }
        });
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //ExportSAGE change
  onExportSAGEChange() {
    //Set values
    this.repartition.ExportSAGE = this.exportSAGEFC.value;
    this.manageScreen();
    //Message to user
    if (this.exportSAGEFC.value === false) {
      const dialogRef = this.dialog.open(InformationComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(1592) },
        restoreFocus: false
      });
    }
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    let s: string = getCreationModificationTooltipText(this.repartition);
    return s;
  }
}
