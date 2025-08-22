import { Component, Inject, OnInit, AfterViewInit, ViewChild, OnDestroy } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, UntypedFormArray, ValidationErrors, ValidatorFn } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher, ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { MatStepper } from "@angular/material/stepper";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import moment from "moment";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { showErrorToUser, setOriginalMenu, getAttachmentFilename, shortenLongText } from "../../../globals/utils";
import { UploadComponent } from "../../dialogs/upload/upload.component";
import { DownloadService } from "../../../services/download.service";
import { ModuleName, MenuName, HabilitationQualite, InputFormComponentElementType } from "../../../globals/enums";
import { EventEmitterService } from "../../../services/event-emitter.service";
import { InputFormComponent } from "../../dialogs/input-form/input-form.component";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { ProgressBarComponent } from "../../global/progress-bar/progress-bar.component";
import { finalize } from "rxjs/operators";
import { toSpaceThousandSepatator } from "../../../globals/utils";
import { ProgressBarMode } from "@angular/material/progress-bar";
import { BaseFormComponent } from "../../_ancestors/base-form.component";
import { DomSanitizer } from "@angular/platform-browser";
import { UtilsService } from "../../../services/utils.service";

/** At least one NonConformiteFamille must be checked*/
const nonConformiteFamillesValidator: ValidatorFn = (control: UntypedFormArray): ValidationErrors | null => {
  const nonConformiteFamillesFA = control;
  const totalSelected = nonConformiteFamillesFA.controls
    // get a list of checkbox values (boolean)
    .map(control => control.get('Famille').value === true ? 1 : 0)
    // total up the number of checked checkboxes
    .reduce((prev, next) => next ? prev + next : prev, 0);

  return totalSelected === 0 ? { nonConformiteFamilleNotSelected: true } : null;
}

@Component({
    selector: "non-conformite",
    templateUrl: "./non-conformite.component.html",
    standalone: false
})

export class NonConformiteComponent extends BaseFormComponent<dataModelsInterfaces.NonConformite> implements OnInit, AfterViewInit, OnDestroy {
  //Misc
  lastUploadResut: appInterfaces.UploadResponseBody;
  nextAction: string;
  hasFicheControle: boolean = false;
  //Data
  nonConformiteAccordFournisseurTypeList: dataModelsInterfaces.NonConformiteAccordFournisseurType[] = [];
  nonConformiteDemandeClientTypeList: dataModelsInterfaces.NonConformiteDemandeClientType[] = [];
  nonConformiteNatureList: dataModelsInterfaces.NonConformiteNature[] = [];
  nonConformiteFamilleList: dataModelsInterfaces.NonConformiteFamille[] = [];
  nonConformiteReponseClientTypeList: dataModelsInterfaces.NonConformiteReponseClientType[] = [];
  nonConformiteReponseFournisseurTypeList: dataModelsInterfaces.NonConformiteReponseFournisseurType[] = [];
  nonConformiteFichiersClient: dataModelsInterfaces.NonConformiterFichier[] = [];
  nonConformiteFichiersFournisseur: dataModelsInterfaces.NonConformiterFichier[] = [];
  nonConformiteFichiersFacturationFournisseur: dataModelsInterfaces.NonConformiterFichier[] = [];
  nonConformiteFichiersCloture: dataModelsInterfaces.NonConformiterFichier[] = [];
  //Form
  form: UntypedFormGroup;
  formGroup1: UntypedFormGroup;
  formGroup2: UntypedFormGroup;
  formGroup3: UntypedFormGroup;
  formGroup4: UntypedFormGroup;
  formGroup5: UntypedFormGroup;
  formGroup6: UntypedFormGroup;
  formGroup7: UntypedFormGroup;
  formGroup8: UntypedFormGroup;
  nonConformite: dataModelsInterfaces.NonConformite = {} as dataModelsInterfaces.NonConformite;
  nonConformiteTextCmdFC: UntypedFormControl = new UntypedFormControl(null);
  nonConformiteTextClientFC: UntypedFormControl = new UntypedFormControl(null);
  nonConformiteTextFournisseurFC: UntypedFormControl = new UntypedFormControl(null);
  descrClientFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  nonConformiteDemandeClientTypeListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  descrValorplastFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  nonConformiteNatureListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  nonConformiteFamilles: UntypedFormArray = new UntypedFormArray([]);
  iFFournisseurFC: UntypedFormControl = new UntypedFormControl();
  iFFournisseurDescrFC: UntypedFormControl = new UntypedFormControl(null);
  iFFournisseurFactureMontantFC: UntypedFormControl = new UntypedFormControl(null, [Validators.max(10000)]);
  iFFournisseurDeductionTonnageFC: UntypedFormControl = new UntypedFormControl(null, [Validators.min(1), Validators.max(8000)]);
  iFFournisseurRetourLotFC: UntypedFormControl = new UntypedFormControl();
  iFClientFC: UntypedFormControl = new UntypedFormControl();
  iFClientDescrFC: UntypedFormControl = new UntypedFormControl(null);
  iFClientFactureMontantFC: UntypedFormControl = new UntypedFormControl(null, [Validators.max(10000)]);
  iFClientCommandeAFaireFC: UntypedFormControl = new UntypedFormControl();
  iFClientFactureEnAttenteFC: UntypedFormControl = new UntypedFormControl();
  transmissionFournisseurFC: UntypedFormControl = new UntypedFormControl();
  dTransmissionFournisseurFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  actionDRFC: UntypedFormControl = new UntypedFormControl(null);
  nonConformiteReponseFournisseurTypeListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  cmtReponseFournisseurFC: UntypedFormControl = new UntypedFormControl(null);
  nonConformiteReponseClientTypeListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  cmtReponseClientFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  planActionFC: UntypedFormControl = new UntypedFormControl(null);
  cmtOrigineActionFC: UntypedFormControl = new UntypedFormControl(null);
  bonCommandeFC: UntypedFormControl = new UntypedFormControl(null);
  nonConformiteAccordFournisseurTypeFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  iFFournisseurBonCommandeNroFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  iFFournisseurAttenteBonCommandeFC: UntypedFormControl = new UntypedFormControl(null);
  iFFournisseurFactureFC: UntypedFormControl = new UntypedFormControl(null);
  iFFournisseurTransmissionFacturationFC: UntypedFormControl = new UntypedFormControl(null);
  iFFournisseurFactureNroFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  iFFournisseurCmtFacturationFC: UntypedFormControl = new UntypedFormControl(null);
  iFClientFactureNroFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  iFClientDFactureFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  iFClientCmtFacturationFC: UntypedFormControl = new UntypedFormControl(null);
  priseEnChargeFC: UntypedFormControl = new UntypedFormControl(null);
  montantPriseEnChargeFC: UntypedFormControl = new UntypedFormControl(null, [Validators.max(10000)]);
  cmtPriseEnChargeFC: UntypedFormControl = new UntypedFormControl(null);
  //Lock
  locked: boolean = true;
  saveLocked: boolean = true;
  formGroup1Disabled: boolean = true;
  formGroup2Disabled: boolean = true;
  formGroup3Disabled: boolean = true;
  formGroup5Disabled: boolean = true;
  formGroup6Disabled: boolean = true;
  formGroup8Disabled: boolean = true;
  toSpaceThousandSepatator = toSpaceThousandSepatator;
  //Progress bar
  color = "warn" as ThemePalette;
  mode = "indeterminate" as ProgressBarMode;
  value = 50;
  displayProgressBar = false;
  //Misc
  visibleNonConformiteFamille: boolean = false;
  //DOM
  @ViewChild('stepper') stepper: MatStepper;
  @ViewChild(ProgressBarComponent) progressBar: ProgressBarComponent;
  //Constructor
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
    , private eventEmitterService: EventEmitterService
    , private downloadService: DownloadService
  ) {
    super("NonConformiteComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      NonConformiteTextCmd: this.nonConformiteTextCmdFC,
      NonConformiteTextClient: this.nonConformiteTextClientFC,
      NonConformiteTextFournisseur: this.nonConformiteTextFournisseurFC,
    });
    this.formGroup1 = this.fb.group({
      DescrClient: this.descrClientFC,
      NonConformiteDemandeClientTypeList: this.nonConformiteDemandeClientTypeListFC,
    });
    this.formGroup2 = this.fb.group({
      DescrValorplast: this.descrValorplastFC,
      NonConformiteNatureList: this.nonConformiteNatureListFC,
      IFFournisseur: this.iFFournisseurFC,
      IFFournisseurDescr: this.iFFournisseurDescrFC,
      IFFournisseurFactureMontant: this.iFFournisseurFactureMontantFC,
      IFFournisseurDeductionTonnage: this.iFFournisseurDeductionTonnageFC,
      IFFournisseurRetourLot: this.iFFournisseurRetourLotFC,
      IFClient: this.iFClientFC,
      IFClientDescr: this.iFClientDescrFC,
      IFClientFactureMontant: this.iFClientFactureMontantFC,
      NonConformiteFamilles: this.nonConformiteFamilles,
    });
    this.formGroup3 = this.fb.group({
      TransmissionFournisseur: this.transmissionFournisseurFC,
      DTransmissionFournisseur: this.dTransmissionFournisseurFC,
      ActionDR: this.actionDRFC,
      NonConformiteReponseFournisseurTypeList: this.nonConformiteReponseFournisseurTypeListFC,
      CmtReponseFournisseur: this.cmtReponseFournisseurFC,
    });
    this.formGroup4 = this.fb.group({
      NonConformiteReponseClientTypeList: this.nonConformiteReponseClientTypeListFC,
      CmtReponseClient: this.cmtReponseClientFC,
    });
    this.formGroup5 = this.fb.group({
      PlanAction: this.planActionFC,
      CmtOrigineAction: this.cmtOrigineActionFC,
    });
    this.formGroup6 = this.fb.group({
      BonCommande: this.bonCommandeFC,
      NonConformiteAccordFournisseurType:this.nonConformiteAccordFournisseurTypeFC,
      IFFournisseurBonCommandeNro: this.iFFournisseurBonCommandeNroFC,
      IFFournisseurAttenteBonCommande: this.iFFournisseurAttenteBonCommandeFC,
      IFFournisseurFacture: this.iFFournisseurFactureFC,
      IFFournisseurTransmissionFacturation: this.iFFournisseurTransmissionFacturationFC,
      IFFournisseurFactureNro: this.iFFournisseurFactureNroFC,
      IFFournisseurCmtFacturation: this.iFFournisseurCmtFacturationFC,
    });
    this.formGroup7 = this.fb.group({
      IFClientFactureNro: this.iFClientFactureNroFC,
      IFClientDFacture: this.iFClientDFactureFC,
      IFClientCmtFacturation: this.iFClientCmtFacturationFC,
      IFClientCommandeAFaire: this.iFClientCommandeAFaireFC,
      IFClientFactureEnAttente: this.iFClientFactureEnAttenteFC,
    });
    this.formGroup8 = this.fb.group({
      PriseEnCharge: this.priseEnChargeFC,
      MontantPriseEnCharge: this.montantPriseEnChargeFC,
      CmtPriseEnCharge: this.cmtPriseEnChargeFC,
    });
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.saveLocked = true;                      
    this.formGroup1.disable();
    this.formGroup1Disabled = true;
    this.formGroup2.disable();
    this.formGroup2Disabled = true;
    this.formGroup3.disable();
    this.formGroup3Disabled = true;
    this.formGroup4.disable();
    this.formGroup5.disable();
    this.formGroup5Disabled = true;
    this.formGroup6.disable();
    this.formGroup6Disabled = true;
    this.formGroup7.disable();
    this.formGroup8.disable();
    this.formGroup8Disabled = true;
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.saveLocked = false;
    this.formGroup1.enable();
    this.formGroup1Disabled = false;
    this.formGroup2.enable();
    this.formGroup2Disabled = false;
    this.formGroup3.enable();
    this.formGroup3Disabled = false;
    this.formGroup4.enable();
    this.formGroup5.enable();
    this.formGroup6Disabled = false;
    this.formGroup6.enable();
    this.formGroup6Disabled = false;
    this.formGroup7.enable();
    this.formGroup8.enable();
    this.formGroup8Disabled = false;
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Global lock 
    this.lockScreen();
    //Rights
    if (!(this.nonConformite.NonConformiteEtapes && this.nonConformite.NonConformiteEtapes[7]?.DValide)) {
      if (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Administrateur) {
        this.locked = false;
        this.saveLocked = false;
        if (!this.nonConformite.NonConformiteEtapes[0]?.DValide) { this.formGroup1.enable(); this.formGroup1Disabled = false; }
        if (!this.nonConformite.NonConformiteEtapes[1]?.DValide) { this.formGroup2.enable(); this.formGroup2Disabled = false; }
        if (!this.nonConformite.NonConformiteEtapes[2]?.DValide) {
          this.formGroup3.enable();
          this.formGroup3Disabled = false;
          this.manageScreenFormGroup3();
        }
        if (!this.nonConformite.NonConformiteEtapes[3]?.DValide) { this.formGroup4.enable(); }
        if (!this.nonConformite.NonConformiteEtapes[4]?.DValide) {
          this.formGroup5.enable();
          this.formGroup5Disabled = false;
          this.manageScreenFormGroup5();
        }
        if (!this.nonConformite.NonConformiteEtapes[5]?.DValide) {
          this.formGroup6.enable();
          this.formGroup6Disabled = false;
          this.manageScreenFormGroup6();
        }
        if (!this.nonConformite.NonConformiteEtapes[6]?.DValide) {
          this.formGroup7.enable();
          this.manageScreenFormGroup7();
        }
        if (!this.nonConformite.NonConformiteEtapes[7]?.DValide) { this.formGroup8.enable(); this.formGroup8Disabled = false; }
      }
      if (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Utilisateur) {
        this.locked = false;
        this.saveLocked = false;
        if (!this.nonConformite.NonConformiteEtapes[2]?.DValide) {
          this.formGroup3.enable();
          this.formGroup3Disabled = false;
          this.manageScreenFormGroup3();
        }
        if (!this.nonConformite.NonConformiteEtapes[4]?.DValide) {
          this.formGroup5.enable();
          this.formGroup5Disabled = false;
          this.manageScreenFormGroup5();
        }
      }
      if (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Client) {
        this.locked = false;
        this.saveLocked = false;
        if (!this.nonConformite.NonConformiteEtapes[0]?.DValide) { this.formGroup1.enable(); this.formGroup1Disabled = false; }
      }
      if (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Fournisseur) {

      }
    }
    else if (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Administrateur) {
      this.saveLocked = false;
    }
  }
  manageScreenFormGroup3() {
    //Validation
    this.dTransmissionFournisseurFC.disable();
    if (this.transmissionFournisseurFC.value) {
      this.dTransmissionFournisseurFC.enable();
    }
    this.nonConformiteReponseFournisseurTypeListFC.disable();
    if (this.transmissionFournisseurFC.value) {
      this.nonConformiteReponseFournisseurTypeListFC.enable();
    }
  }
  manageScreenFormGroup5() {
    //Validation
    this.cmtOrigineActionFC.disable();
    if (!this.planActionFC.value) {
      this.cmtOrigineActionFC.enable();
    }
  }
  manageScreenFormGroup6() {
    //Validation
    this.nonConformiteAccordFournisseurTypeFC.disable();
    if (this.bonCommandeFC.value) {
      this.nonConformiteAccordFournisseurTypeFC.enable();
    }
    this.iFFournisseurBonCommandeNroFC.disable();
    if (this.bonCommandeFC.value) {
      this.iFFournisseurBonCommandeNroFC.enable();
    }
    this.iFFournisseurFactureNroFC.disable();
    if (this.iFFournisseurFactureFC.value) {
      this.iFFournisseurFactureNroFC.enable();
    }
  }
  manageScreenFormGroup7() {
    if (this.iFClientFactureEnAttenteFC.value) {
      this.iFClientFactureNroFC.disable();
      this.iFClientDFactureFC.disable();
    }
    else {
      this.iFClientFactureNroFC.enable();
      this.iFClientDFactureFC.enable();
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    let refCommandeFournisseur = Number.parseInt(this.activatedRoute.snapshot.params["refCommandeFournisseur"], 10);
    //Load initial data
    //Check parameters
    if (!isNaN(id) && !isNaN(refCommandeFournisseur)) {
      this.dataModelService.getNonConformite(id, refCommandeFournisseur).subscribe(result => {
        //Get data
        this.nonConformite = result;
        this.initNonConformiteFichierArrays();
        //Set dates
        this.dataModelService.setMomentFromTextNonConformite(this.nonConformite);
        //Update form
        this.updateForm();
        this.initSlideToggles();
        this.manageScreen();
        //Set selected step
        let currentStep = this.getCurrentStepIndex();
        if (currentStep < 8 && currentStep > 0) {
          //this.stepper.selectedIndex = currentStep;
        }
        //Get existing NonConformiteAccordFournisseur
        this.listService.getListNonConformiteAccordFournisseurType(true, this.nonConformite.NonConformiteAccordFournisseurType?.RefNonConformiteAccordFournisseurType).subscribe(result => {
          this.nonConformiteAccordFournisseurTypeList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing NonConformiteDemandeClientType
        this.listService.getListNonConformiteDemandeClientType(true, this.nonConformite.NonConformiteDemandeClientType?.RefNonConformiteDemandeClientType).subscribe(result => {
          this.nonConformiteDemandeClientTypeList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing NonConformiteNature
        this.listService.getListNonConformiteNature(true, this.nonConformite.NonConformiteNature?.RefNonConformiteNature).subscribe(result => {
          this.nonConformiteNatureList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing NonConformiteFamille
        this.listService.getListNonConformiteFamille(false, this.nonConformite.NonConformiteNonConformiteFamilles.map(val => val.NonConformiteFamille.RefNonConformiteFamille).join(", ")).subscribe(result => {
          this.nonConformiteFamilleList = result;
          //Update form
          this.updateFormArrayFamille();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing NonConformiteReponseClient
        this.listService.getListNonConformiteReponseClientType(true, this.nonConformite.NonConformiteReponseClientType?.RefNonConformiteReponseClientType).subscribe(result => {
          this.nonConformiteReponseClientTypeList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing NonConformiteReponseFournisseur
        this.listService.getListNonConformiteReponseFournisseurType(true, this.nonConformite.NonConformiteReponseFournisseurType?.RefNonConformiteReponseFournisseurType).subscribe(result => {
          this.nonConformiteReponseFournisseurTypeList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Check if linked to a FicheControle
        this.dataModelService.hasFicheControle(this.nonConformite.CommandeFournisseur.RefCommandeFournisseur)
          .subscribe((result: boolean) => {
            this.hasFicheControle = result;
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngAfterViewInit() {
    //Set selected step
    let currentStep = this.getCurrentStepIndex();
    if (currentStep < 8 && currentStep > 0) {
      this.stepper.selectedIndex = currentStep;
    }
  }
  //-----------------------------------------------------------------------------------
  //Destroy
  ngOnDestroy() {
    //Stop progress bar
    //this.displayProgressBar = false;
    this.progressBar.stop();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  initNonConformiteFichierArrays() {
    this.nonConformiteFichiersClient = this.nonConformite.NonConformiteFichiers.filter(e => e.NonConformiteFichierType.RefNonConformiteFichierType == 1);
    this.nonConformiteFichiersFournisseur = this.nonConformite.NonConformiteFichiers.filter(e => e.NonConformiteFichierType.RefNonConformiteFichierType == 2);
    this.nonConformiteFichiersFacturationFournisseur = this.nonConformite.NonConformiteFichiers.filter(e => e.NonConformiteFichierType.RefNonConformiteFichierType == 3);
    this.nonConformiteFichiersCloture = this.nonConformite.NonConformiteFichiers.filter(e => e.NonConformiteFichierType.RefNonConformiteFichierType == 4);
  }
  //-----------------------------------------------------------------------------------
  //Get data for IHM
  get getNonConformiteFamilles(): UntypedFormArray {
    return this.nonConformiteFamilles as UntypedFormArray;
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.nonConformiteTextCmdFC.setValue(this.nonConformite.NonConformiteTextCmd);
    this.nonConformiteTextClientFC.setValue(this.nonConformite.NonConformiteTextClient);
    this.nonConformiteTextFournisseurFC.setValue(this.nonConformite.NonConformiteTextFournisseur);
    this.descrClientFC.setValue(this.nonConformite.DescrClient);
    this.nonConformiteDemandeClientTypeListFC.setValue(this.nonConformite.NonConformiteDemandeClientType?.RefNonConformiteDemandeClientType);
    this.descrValorplastFC.setValue(this.nonConformite.DescrValorplast);
    this.nonConformiteNatureListFC.setValue(this.nonConformite.NonConformiteNature?.RefNonConformiteNature);
    this.iFFournisseurDescrFC.setValue(this.nonConformite.IFFournisseurDescr);
    this.iFFournisseurFactureMontantFC.setValue(this.nonConformite.IFFournisseurFactureMontant);
    this.iFFournisseurDeductionTonnageFC.setValue(this.nonConformite.IFFournisseurDeductionTonnage);
    this.iFFournisseurRetourLotFC.setValue(this.nonConformite.IFFournisseurRetourLot);
    this.iFClientDescrFC.setValue(this.nonConformite.IFClientDescr);
    this.iFClientFactureMontantFC.setValue(this.nonConformite.IFClientFactureMontant);
    this.dTransmissionFournisseurFC.setValue(this.nonConformite.DTransmissionFournisseur);
    this.actionDRFC.setValue(this.nonConformite.ActionDR);
    this.nonConformiteReponseFournisseurTypeListFC.setValue(this.nonConformite.NonConformiteReponseFournisseurType?.RefNonConformiteReponseFournisseurType);
    this.cmtReponseFournisseurFC.setValue(this.nonConformite.CmtReponseFournisseur);
    this.nonConformiteReponseClientTypeListFC.setValue(this.nonConformite.NonConformiteReponseClientType?.RefNonConformiteReponseClientType);
    this.cmtReponseClientFC.setValue(this.nonConformite.CmtReponseClient);
    this.planActionFC.setValue(!this.nonConformite.PlanAction);
    this.cmtOrigineActionFC.setValue(this.nonConformite.CmtOrigineAction);
    this.nonConformiteAccordFournisseurTypeFC.setValue(this.nonConformite.NonConformiteAccordFournisseurType?.RefNonConformiteAccordFournisseurType.toString());
    this.iFFournisseurAttenteBonCommandeFC.setValue(this.nonConformite.IFFournisseurAttenteBonCommande);
    this.iFFournisseurBonCommandeNroFC.setValue(this.nonConformite.IFFournisseurBonCommandeNro);
    this.iFFournisseurFactureFC.setValue(this.nonConformite.IFFournisseurFacture);
    this.iFFournisseurTransmissionFacturationFC.setValue(this.nonConformite.IFFournisseurTransmissionFacturation);
    this.iFFournisseurFactureNroFC.setValue(this.nonConformite.IFFournisseurFactureNro);
    this.iFFournisseurCmtFacturationFC.setValue(this.nonConformite.IFFournisseurCmtFacturation);
    this.iFClientFactureNroFC.setValue(this.nonConformite.IFClientFactureNro);
    this.iFClientDFactureFC.setValue(this.nonConformite.IFClientDFacture);
    this.iFClientCmtFacturationFC.setValue(this.nonConformite.IFClientCmtFacturation);
    this.iFClientCommandeAFaireFC.setValue(this.nonConformite.IFClientCommandeAFaire);
    this.iFClientFactureEnAttenteFC.setValue(this.nonConformite.IFClientFactureEnAttente);
    this.priseEnChargeFC.setValue(this.nonConformite.PriseEnCharge);
    this.montantPriseEnChargeFC.setValue(this.nonConformite.MontantPriseEnCharge);
    this.cmtPriseEnChargeFC.setValue(this.nonConformite.CmtPriseEnCharge);
    this.updateFormArrayFamille();
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateFormArrayFamille() {
    this.nonConformiteFamilles.clear();
    for (const nCF of this.nonConformiteFamilleList) {
      const grp = this.fb.group({
        Famille: this.nonConformite.NonConformiteNonConformiteFamilles.some(el => el.NonConformiteFamille.RefNonConformiteFamille === nCF.RefNonConformiteFamille),
      });
      this.nonConformiteFamilles.push(grp);
    }
  }
  //-----------------------------------------------------------------------------------
  //Slide toggles init
  initSlideToggles() {
    this.iFFournisseurFC.setValue(!!this.nonConformite.IFFournisseurDescr || !!this.nonConformite.IFFournisseurFactureMontant || !!this.nonConformite.IFFournisseurDeductionTonnage);
    this.iFClientFC.setValue(!!this.nonConformite.IFClientDescr || !!this.nonConformite.IFClientFactureMontant);
    this.transmissionFournisseurFC.setValue(!!this.nonConformite.DTransmissionFournisseur || !!this.nonConformite.NonConformiteReponseFournisseurType || !!this.nonConformite.CmtReponseFournisseur);
    this.bonCommandeFC.setValue(!!this.nonConformite.IFFournisseurAttenteBonCommande || !!this.nonConformite.IFFournisseurBonCommandeNro || !!this.nonConformite.NonConformiteAccordFournisseurType);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.nonConformite.DescrClient = this.descrClientFC.value;
    this.nonConformite.NonConformiteDemandeClientType = this.nonConformiteDemandeClientTypeList.find(x => x.RefNonConformiteDemandeClientType === this.nonConformiteDemandeClientTypeListFC.value);
    this.nonConformite.DescrValorplast = this.descrValorplastFC.value;
    this.nonConformite.NonConformiteNature = this.nonConformiteNatureList.find(x => x.RefNonConformiteNature === this.nonConformiteNatureListFC.value);
    this.nonConformite.IFFournisseurDescr = this.iFFournisseurDescrFC.value;
    this.nonConformite.IFFournisseurFactureMontant = this.iFFournisseurFactureMontantFC.value;
    this.nonConformite.IFFournisseurDeductionTonnage = this.iFFournisseurDeductionTonnageFC.value;
    this.nonConformite.IFFournisseurRetourLot = this.iFFournisseurRetourLotFC.value;
    this.nonConformite.IFClientDescr = this.iFClientDescrFC.value;
    this.nonConformite.IFClientFactureMontant = this.iFClientFactureMontantFC.value;
    this.nonConformite.DTransmissionFournisseur = (this.dTransmissionFournisseurFC.value == null ? null : this.dTransmissionFournisseurFC.value);
    this.nonConformite.ActionDR = this.actionDRFC.value;
    this.nonConformite.NonConformiteReponseFournisseurType = this.nonConformiteReponseFournisseurTypeList.find(x => x.RefNonConformiteReponseFournisseurType === this.nonConformiteReponseFournisseurTypeListFC.value);
    this.nonConformite.CmtReponseFournisseur = this.cmtReponseFournisseurFC.value;
    this.nonConformite.NonConformiteReponseClientType = this.nonConformiteReponseClientTypeList.find(x => x.RefNonConformiteReponseClientType === this.nonConformiteReponseClientTypeListFC.value);
    this.nonConformite.CmtReponseClient = this.cmtReponseClientFC.value;
    this.nonConformite.PlanAction = !this.planActionFC.value;
    this.nonConformite.CmtOrigineAction = this.cmtOrigineActionFC.value;
    this.nonConformite.NonConformiteAccordFournisseurType = (!!this.nonConformiteAccordFournisseurTypeFC.value ? this.nonConformiteAccordFournisseurTypeList[this.nonConformiteAccordFournisseurTypeFC.value - 1] : null);
    this.nonConformite.IFFournisseurAttenteBonCommande = this.iFFournisseurAttenteBonCommandeFC.value;
    this.nonConformite.IFFournisseurBonCommandeNro = this.iFFournisseurBonCommandeNroFC.value;
    this.nonConformite.IFFournisseurFacture = this.iFFournisseurFactureFC.value;
    this.nonConformite.IFFournisseurTransmissionFacturation = this.iFFournisseurTransmissionFacturationFC.value;
    this.nonConformite.IFFournisseurFactureNro = this.iFFournisseurFactureNroFC.value;
    this.nonConformite.IFFournisseurCmtFacturation = this.iFFournisseurCmtFacturationFC.value;
    this.nonConformite.IFClientFactureNro = this.iFClientFactureNroFC.value;
    this.nonConformite.IFClientDFacture = (this.iFClientDFactureFC.value == null ? null : this.iFClientDFactureFC.value);
    this.nonConformite.IFClientCmtFacturation = this.iFClientCmtFacturationFC.value;
    this.nonConformite.IFClientCommandeAFaire = this.iFClientCommandeAFaireFC.value;
    this.nonConformite.IFClientFactureEnAttente = this.iFClientFactureEnAttenteFC.value;
    this.nonConformite.PriseEnCharge = this.priseEnChargeFC.value;
    this.nonConformite.MontantPriseEnCharge = this.montantPriseEnChargeFC.value;
    this.nonConformite.CmtPriseEnCharge = this.cmtPriseEnChargeFC.value;
    //NonConformiteFamilles
    this.nonConformite.NonConformiteNonConformiteFamilles = [];
    for (let i in this.nonConformiteFamilles.controls) {
      let fG: UntypedFormGroup = this.nonConformiteFamilles.controls[i] as UntypedFormGroup;
      if (fG.get("Famille").value) {
        this.nonConformite.NonConformiteNonConformiteFamilles.push(
          { RefNonConformiteNonConformiteFamille: 0, RefNonConformite: this.nonConformite.RefNonConformite, NonConformiteFamille: this.nonConformiteFamilleList[i] });
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Show image gallery
  showPictures(elementType: string, elementId: number, pictureId: number) {
    let data: any;
    //Init
    this.saveData();
    data = { title: this.applicationUserContext.getCulturedRessourceText(623), type: "ImageGallery", elementType: elementType, ref: elementId.toString(), pictureId: pictureId };
    //Open dialog
    this.dialog.open(ComponentRelativeComponent, {
      width: "650px",
      data,
      autoFocus: false,
      restoreFocus: false
    });
  }
  //-----------------------------------------------------------------------------------
  //Delete a onDeleteNonConformiteFichier
  onDeleteNonConformiteFichier(refNonConformiteFichier: number) {
    if (refNonConformiteFichier !== null) {
      //Save data
      this.saveData();
      //Process
      this.nonConformite.NonConformiteFichiers = this.nonConformite.NonConformiteFichiers.filter(e => e.RefNonConformiteFichier != refNonConformiteFichier);
      //Init NonConformiteFichier arrays
      this.initNonConformiteFichierArrays();
      //Update form
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  onUploadFile(fileType: number) {
    //Save data
    this.saveData();
    //Save to database if needed
    if (this.nonConformite.RefNonConformite <= 0) {
      this.onSave("UploadFile", fileType);
    }
    else {
      //open pop-up
      this.startUploadFile(fileType);
    }
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  startUploadFile(fileType: number) {
      //open pop-up
      const dialogRef = this.dialog.open(UploadComponent, {
        width: "50%", height: "50%",
        data: {
          title: this.applicationUserContext.getCulturedRessourceText(626), message: this.applicationUserContext.getCulturedRessourceText(627)
          , multiple: true, type: "nonconformite", fileType: fileType, ref: this.nonConformite.RefNonConformite
        },
        autoFocus: false,
        restoreFocus: false
      });

    dialogRef.afterClosed().subscribe(result => {
      this.lastUploadResut = result;
      if (!this.lastUploadResut) {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(341), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
      else if (this.lastUploadResut.error != "") {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(631), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
      else {
        //Reload data
        this.dataModelService.getNonConformite(this.nonConformite.RefNonConformite,null).subscribe(result => {
          //Get data
          this.nonConformite.NonConformiteFichiers = result.NonConformiteFichiers;
          this.initNonConformiteFichierArrays();
          ////Set dates
          //this.dataModelService.setMomentFromTextNonConformite(this.nonConformite);
          ////Update form
          //this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(628), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Go to CommandeFournisseur
  onCommandeFournisseur() {
    this.onSave("CommandeFournisseur", 0);
  }
  //-----------------------------------------------------------------------------------
  //Go to FicheControle
  onFicheControle() {
    this.onSave("FicheControle", 0);
  }
  //-----------------------------------------------------------------------------------
  //on change IFFournisseurBonCommandeNro
  onChangeIFFournisseurBonCommandeNro(event: any) {
    if (event.target.value) {
      this.iFFournisseurAttenteBonCommandeFC.setValue(false);
    }
  }
  //-----------------------------------------------------------------------------------
  //on change BonCommande
  onChangeBonCommande() {
    if (!this.bonCommandeFC.value) {
      this.nonConformiteAccordFournisseurTypeFC.setValue("");
      this.iFFournisseurAttenteBonCommandeFC.setValue(false);
      this.iFFournisseurBonCommandeNroFC.setValue(null);
    }
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //on change IFFournisseurFacture
  onChangeIFFournisseurFacture() {
    if (!this.iFFournisseurFactureFC.value) {
      this.iFFournisseurTransmissionFacturationFC.setValue(false);
      this.iFFournisseurFactureNroFC.setValue(null);
      this.iFFournisseurCmtFacturationFC.setValue(null);
    }
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //on change IFClientFactureEnAttente
  onChangeIFClientFactureEnAttente() {
    if (this.iFClientFactureEnAttenteFC.value) {
      this.iFClientFactureNroFC.setValue(null);
      this.iFClientDFactureFC.setValue(null);
    }
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Get etape
  getStep(stepIndex: number): dataModelsInterfaces.NonConformiteEtape {
    let etape = {} as dataModelsInterfaces.NonConformiteEtape;
    if (this.nonConformite?.NonConformiteEtapes) {
      etape = this.nonConformite?.NonConformiteEtapes[stepIndex];
    }
    return etape;
  }
  //-----------------------------------------------------------------------------------
  //Is current step
  getCurrentStepIndex(): number {
    let curIndex = 10;
    if (this.nonConformite?.NonConformiteEtapes) {
      for (let i: number = 0; i < 8; i++) {
        if (this.nonConformite.NonConformiteEtapes[i]?.DValide == null) {
          curIndex = i;
          break;
        }
      }
    }
    return curIndex;
  }
  //-----------------------------------------------------------------------------------
  //Validate step
  validateStep(stepIndex: number) {
    let etape = this.nonConformite.NonConformiteEtapes[stepIndex];
    if (etape.UtilisateurCreation == null) {
      etape.DCreation = moment();
      etape.UtilisateurCreation = this.applicationUserContext.connectedUtilisateur;
    }
    else {
      etape.DModif = moment();
      etape.UtilisateurModif = this.applicationUserContext.connectedUtilisateur;
    }
    etape.DValide = moment();
    etape.UtilisateurValide = this.applicationUserContext.connectedUtilisateur;
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Invalidate step
  invalidateStep(stepIndex: number) {
    let etape = this.nonConformite.NonConformiteEtapes[stepIndex];
    if (etape.UtilisateurCreation == null) {
      etape.DCreation = moment();
      etape.UtilisateurCreation = this.applicationUserContext.connectedUtilisateur;
    }
    else {
      etape.DModif = moment();
      etape.UtilisateurModif = this.applicationUserContext.connectedUtilisateur;
    }
    etape.DValide = null;
    etape.UtilisateurValide = null;
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Comment step
  commentStep(stepIndex: number) {
    //open pop-up
    const dialogRef = this.dialog.open(InputFormComponent, {
      width: "50%",
      data: {
        type: InputFormComponentElementType.NonConformiteCmt,
        title: this.applicationUserContext.getCulturedRessourceText(855),
        cmt: this.nonConformite.NonConformiteEtapes[stepIndex].Cmt
      },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.nonConformite.NonConformiteEtapes[stepIndex].Cmt = result.resp;
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Create texts for Etape
  getStepText(stepIndex: number): string {
    let s = "";
    if (this.nonConformite?.NonConformiteEtapes) {
      let etape = this.nonConformite.NonConformiteEtapes[stepIndex];
      //Process
      if (etape?.UtilisateurCreation !== null) {
        s += this.applicationUserContext.getCulturedRessourceText(388) + " "
          + moment(etape?.DCreation).format("L") + " " + moment(etape?.DCreation).format("LT") + " "
          + this.applicationUserContext.getCulturedRessourceText(390) + " "
          + etape?.UtilisateurCreation.Nom;
      }
      if (etape?.UtilisateurModif !== null) {
        if (s !== "") { s += "\n"; }
        s += this.applicationUserContext.getCulturedRessourceText(389) + " "
          + moment(etape?.DModif).format("L") + " " + moment(etape?.DModif).format("LT") + " "
          + this.applicationUserContext.getCulturedRessourceText(390) + " "
          + etape?.UtilisateurModif.Nom;
      }
      if (etape?.UtilisateurValide !== null) {
        if (s !== "") { s += "\n"; }
        s += this.applicationUserContext.getCulturedRessourceText(852) + " "
          + moment(etape?.DValide).format("L") + " " + moment(etape?.DValide).format("LT") + " "
          + this.applicationUserContext.getCulturedRessourceText(390) + " "
          + etape?.UtilisateurValide.Nom;
      }
      if (etape?.UtilisateurControle !== null) {
        if (s !== "") { s += "\n"; }
        s += this.applicationUserContext.getCulturedRessourceText(853) + " "
          + moment(etape?.DControle).format("L") + " " + moment(etape?.DControle).format("LT") + " "
          + this.applicationUserContext.getCulturedRessourceText(390) + " "
          + etape?.UtilisateurControle.Nom;
      }
    }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create infos for Etape
  getStepInfo(stepIndex: number): string {
    let s = "";
    if (this.nonConformite?.NonConformiteEtapes) {
      //Process
      switch (stepIndex) {
        case 0:
          s = (!this.nonConformiteDemandeClientTypeListFC.value ? "" : this.nonConformiteDemandeClientTypeList.find(x => x.RefNonConformiteDemandeClientType === this.nonConformiteDemandeClientTypeListFC.value)?.Libelle);
          break;
        case 1:
          s = (!this.descrValorplastFC.value ? "" : shortenLongText(this.descrValorplastFC.value,300));
          break;
        case 2:
          s = this.applicationUserContext.getCulturedRessourceText(833) + " - ";
          if (!!this.dTransmissionFournisseurFC.value || !!this.nonConformiteReponseFournisseurTypeListFC.value || !!this.cmtReponseFournisseurFC.value) {
            s += this.applicationUserContext.getCulturedRessourceText(205);
            if (!!this.dTransmissionFournisseurFC.value) {
              s += "\n" + moment(this.dTransmissionFournisseurFC.value).format("L");
            }
            if (!!this.nonConformiteReponseFournisseurTypeListFC.value) {
              s += " \n" + this.nonConformiteReponseFournisseurTypeList.find(x => x.RefNonConformiteReponseFournisseurType === this.nonConformiteReponseFournisseurTypeListFC.value)?.Libelle;
            }
          }
          else {
            s += this.applicationUserContext.getCulturedRessourceText(206);
          }
          break;
        case 3:
          if (!!this.nonConformiteReponseClientTypeListFC.value) {
            s += this.nonConformiteReponseClientTypeList.find(x => x.RefNonConformiteReponseClientType === this.nonConformiteReponseClientTypeListFC.value)?.Libelle;
          }
          if (!!this.cmtReponseClientFC.value) {
            if (s) { s += "\n"; }
            s += shortenLongText(this.cmtReponseClientFC.value, 200);
          }
          break;
        case 4:
          if (this.planActionFC.value) {
            s += this.applicationUserContext.getCulturedRessourceText(837)
          }
          if (this.cmtOrigineActionFC.value) {
            if (s) { s += "\n"; }
            s += (!this.cmtOrigineActionFC.value ? "" : this.applicationUserContext.getCulturedRessourceText(215));
          }
          break;
        case 5:
          if (this.bonCommandeFC.value) {
            s += this.applicationUserContext.getCulturedRessourceText(863) + " - ";
            if (this.nonConformiteAccordFournisseurTypeFC.value) {
              s += this.nonConformiteAccordFournisseurTypeList[this.nonConformiteAccordFournisseurTypeFC.value - 1]?.Libelle;
            }
          }
          if (s) { s += "\n"; }
          if (!!this.iFFournisseurBonCommandeNroFC.value) {
            s += this.applicationUserContext.getCulturedRessourceText(839) + " - " + this.iFFournisseurBonCommandeNroFC.value
          }
          else if (this.iFFournisseurAttenteBonCommandeFC.value) {
            s += this.applicationUserContext.getCulturedRessourceText(848);
          }
          break;
        case 6:
          s = this.applicationUserContext.getCulturedRessourceText(841) + " - " + (!this.iFClientFactureNroFC.value ? "" : this.iFClientFactureNroFC.value);
          if (!!this.iFClientDFactureFC.value) {
            s += "\n" + moment(this.iFClientDFactureFC.value).format("L");
          }
          break;
        case 7:
          if (this.priseEnChargeFC.value) {
            s += this.applicationUserContext.getCulturedRessourceText(842)
          }
          break;
      }
    }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create infos for Etape
  getStepInfo2(stepIndex: number): string {
    let s = "";
    if (this.nonConformite?.NonConformiteEtapes) {
      //Process
      switch (stepIndex) {
        case 1:
          s = (!this.nonConformiteNatureListFC.value ? "" : this.nonConformiteNatureList.find(x => x.RefNonConformiteNature === this.nonConformiteNatureListFC.value)?.Libelle);
          let selectedFamilles = this.nonConformiteFamillesLabel();
          if (selectedFamilles) {
            if (s) { s += "\n"; }
            s += this.nonConformiteFamillesLabel();
          }
          s = shortenLongText(s, 150);
          if (!!this.iFFournisseurRetourLotFC.value) {
            if (s) { s += "\n"; }
            s += this.applicationUserContext.getCulturedRessourceText(827) + " - " + (!this.iFFournisseurRetourLotFC.value ? this.applicationUserContext.getCulturedRessourceText(206) : this.applicationUserContext.getCulturedRessourceText(205));
          }
          break;
        case 5:
          s += this.applicationUserContext.getCulturedRessourceText(840) + " - ";
          if (this.iFFournisseurTransmissionFacturationFC.value) {
            s += this.applicationUserContext.getCulturedRessourceText(205)
          }
          else {
            s += this.applicationUserContext.getCulturedRessourceText(206)
          }
          if (!!this.iFFournisseurFactureNroFC.value) {
            if (s) { s += "\n"; }
            s += this.applicationUserContext.getCulturedRessourceText(841) + " - " + this.iFFournisseurFactureNroFC.value;
          }
          break;
      }
    }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create infos for Etape
  getStepInfo3(stepIndex: number): string {
    let s = "";
    if (this.nonConformite?.NonConformiteEtapes) {
      //Process
      switch (stepIndex) {
        case 1:
          if (!!this.iFFournisseurDescrFC.value || !!this.iFFournisseurFactureMontantFC.value || !!this.iFFournisseurDeductionTonnageFC.value) {
            s = this.applicationUserContext.getCulturedRessourceText(185);
            if (!!this.iFFournisseurFactureMontantFC.value) {
              s += "\n" + this.applicationUserContext.getCulturedRessourceText(826) + " - "
                + (!this.iFFournisseurFactureMontantFC.value ? "" : this.toSpaceThousandSepatator(this.iFFournisseurFactureMontantFC.value));
            }
            if (!!this.iFFournisseurDeductionTonnageFC.value) {
              s += "\n" + this.applicationUserContext.getCulturedRessourceText(236) + " - "
                + (!this.iFFournisseurDeductionTonnageFC.value ? "" : this.toSpaceThousandSepatator(this.iFFournisseurDeductionTonnageFC.value));
            }
            break;
          }
      }
    }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create infos for Etape
  getStepInfo4(stepIndex: number): string {
    let s = "";
    if (this.nonConformite?.NonConformiteEtapes) {
      //Process
      switch (stepIndex) {
        case 1:
          if (!!this.iFClientDescrFC.value || !!this.iFClientFactureMontantFC.value) {
            s = this.applicationUserContext.getCulturedRessourceText(193);
            s += "\n" + this.applicationUserContext.getCulturedRessourceText(826) + " - "
              + (!this.iFClientFactureMontantFC.value ? "" : this.toSpaceThousandSepatator(this.iFClientFactureMontantFC.value));
            break;
          }
      }
    }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create Familles
  nonConformiteFamillesLabel(): string {
    let s = "";
    let selectedFamilles = this.formGroup2.value.NonConformiteFamilles
      .map((val, i) => val.Famille ? this.nonConformiteFamilleList[i].Libelle : null)
      .filter(v => v !== null)
      .join(", ");
    if (!!selectedFamilles) {
      if (s) { s += "\n"; }
      s += selectedFamilles;
    }
    if (this.iFFournisseurRetourLotFC.value) {
      if (s) { s += "\n"; }
    }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //List of Familles sparated by comma
  nonConformiteFamillesIds(): string {
    let s = "";
    s = this.nonConformite.NonConformiteNonConformiteFamilles.map(val => val.NonConformiteFamille.RefNonConformiteFamille).join(", ");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Clear IFFournisseur data
  clearIFFournisseurData() {
    //Clear IFFournisseur
    if (!this.iFFournisseurFC.value) {
      this.iFFournisseurDescrFC.setValue(null);
      this.iFFournisseurFactureMontantFC.setValue(null);
      this.iFFournisseurDeductionTonnageFC.setValue(null);
      //Clear related data
      this.clearIFFournisseurRelatedData();
    }
    else if (!this.iFFournisseurDescrFC.value && !this.iFFournisseurFactureMontantFC.value && !this.iFFournisseurDeductionTonnageFC.value) {
      this.invalidateStep(5);
    }
  }
  //-----------------------------------------------------------------------------------
  //Clear IFFournisseur related data
  clearIFFournisseurRelatedData() {
    //Clear IFFournisseur
    if (!this.iFFournisseurDescrFC.value && !this.iFFournisseurFactureMontantFC.value && !this.iFFournisseurDeductionTonnageFC.value) {
      this.iFFournisseurBonCommandeNroFC.setValue(null);
      this.iFFournisseurAttenteBonCommandeFC.setValue(false);
      this.iFFournisseurTransmissionFacturationFC.setValue(false);
      this.iFFournisseurFactureNroFC.setValue(null);
      this.iFFournisseurCmtFacturationFC.setValue(null);
      this.iFFournisseurFC.setValue(false);
      this.validateStep(5);
    }
  }
  //-----------------------------------------------------------------------------------
  //Clear IFClient data
  clearIFClientData() {
    //Clear IFClient
    if (!this.iFClientFC.value) {
      this.iFClientDescrFC.setValue(null);
      this.iFClientFactureMontantFC.setValue(null);
      //Clear related data
      this.clearIFClientRelatedData();
    }
    else if (!this.iFClientDescrFC.value && !this.iFClientFactureMontantFC.value) {
      this.invalidateStep(6);
    }
  }
  //-----------------------------------------------------------------------------------
  //Clear IFClient related data
  clearIFClientRelatedData() {
    //Clear IFClient
    if (!this.iFClientDescrFC.value && !this.iFClientFactureMontantFC.value) {
      this.iFClientFactureNroFC.setValue(null);
      this.iFClientDFactureFC.setValue(null);
      this.iFClientCmtFacturationFC.setValue(null);
      this.iFClientFC.setValue(false);
      this.validateStep(6);
    }
  }
  //-----------------------------------------------------------------------------------
  //Choose element to save to other NonConformite 
  chooseElement(elementType: string) {
    let data: any;
    //Init
    this.saveData();
    switch (elementType) {
      case "NonConformiteForIFClientFacture":
        data = {
          title: this.applicationUserContext.getCulturedRessourceText(1575)
          , type: elementType
          , ref: this.nonConformite.RefNonConformite.toString()
        };
        break;
    }
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "50%",
      maxHeight: "80vh",
      data,
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.dataModelService.setNonConforimiteIFClientFacture(result.ref, this.iFClientFactureNroFC.value, this.iFClientDFactureFC.value)
          .subscribe(result => {
            //Inform user
            this.snackBarQueueService.addMessage({ text: result + " " + this.applicationUserContext.getCulturedRessourceText(1577), duration: 4000 } as appInterfaces.SnackbarMsg);
            //Validate step
            this.validateStep(6);
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Download a file
  getFile(fileType: string, RefNonConformiteFichier: number) {
    this.downloadService.download(fileType, RefNonConformiteFichier.toString(), "", this.nonConformite.RefNonConformite, "")
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
  //Print
  print() {
    //NonConformite type Client
    if (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Administrateur
      || this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Utilisateur
      || this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Fournisseur
    ) {
      this.downloadService.download("NonConformite", this.nonConformite.RefNonConformite.toString(), "pdf", null, "Fournisseur")
        .subscribe((data: HttpResponse<Blob>) => {
          let contentDispositionHeader = data.headers.get("Content-Disposition");
          let fileName = getAttachmentFilename(contentDispositionHeader);
          this.downloadFile(data.body, fileName);
        }, error => {
          showErrorToUser(this.dialog, error, this.applicationUserContext);
        });
    }
    //NonConformite type Fournisseur
    if (this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Administrateur
      || this.applicationUserContext.connectedUtilisateur.HabilitationQualite === HabilitationQualite.Client
    ) {
      this.downloadService.download("NonConformite", this.nonConformite.RefNonConformite.toString(), "pdf", null, "Client")
        .subscribe((data: HttpResponse<Blob>) => {
          let contentDispositionHeader = data.headers.get("Content-Disposition");
          let fileName = getAttachmentFilename(contentDispositionHeader);
          this.downloadFile(data.body, fileName);
        }, error => {
          showErrorToUser(this.dialog, error, this.applicationUserContext);
        });
    }
    //FicheControle
    //Get related FicheControle
    this.dataModelService.getRelatedRefFicheControle(this.nonConformite.CommandeFournisseur.RefCommandeFournisseur)
      .subscribe(result => {
        if (result != 0) {
          this.downloadService.download("FicheControle", result.toString(), "xlsx", null, "")
            .subscribe((data: HttpResponse<Blob>) => {
              let contentDispositionHeader = data.headers.get("Content-Disposition");
              let fileName = getAttachmentFilename(contentDispositionHeader);
              this.downloadFile(data.body, fileName);
            }, error => {
              showErrorToUser(this.dialog, error, this.applicationUserContext);
            });
        }
      });
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(next: string, fileType: number) {
    this.nextAction = next;
    this.saveData();
    // Display progress bar
    this.progressBar.start();
    //Format dates
    this.dataModelService.setTextFromMomentNonConformite(this.nonConformite);
    //Update 
    this.dataModelService.postNonConformite(this.nonConformite, true)
      .pipe(
        finalize(() => this.progressBar.stop())
      )
      .subscribe(result => {
        //Set primary key
        this.nonConformite = result;
        this.initNonConformiteFichierArrays();
        //Set dates
        this.dataModelService.setMomentFromTextNonConformite(this.nonConformite);
        //Update form
        this.updateForm();
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        if (this.nextAction !== "") {
          switch (this.nextAction) {
            case "CommandeFournisseur":
              //Save menu
              if (this.applicationUserContext.fromMenu == null) {
                this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
              }
              this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
              this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
              this.eventEmitterService.onChangeMenu();
              this.router.navigate(["commande-fournisseur", this.nonConformite.CommandeFournisseur.RefCommandeFournisseur.toString()]);
              break;
            case "FicheControle":
              //Save menu
              if (this.applicationUserContext.fromMenu == null) {
                this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
              }
              this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
              this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
              this.eventEmitterService.onChangeMenu();
              this.router.navigate(["fiche-controle", "0", {
                refCommandeFournisseur: this.nonConformite.CommandeFournisseur.RefCommandeFournisseur
              }]);
              break;
            case "UploadFile":
              //open pop-up
              this.startUploadFile(fileType);
              break;
            case "Print":
              this.print();
              break;
          }
        }
        else {
          //Set original Menu
          setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
          this.router.navigate(["grid"]);
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(850) },
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
    this.dataModelService.deleteNonConformite(this.nonConformite.RefNonConformite)
      .subscribe(() => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(851), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
}
