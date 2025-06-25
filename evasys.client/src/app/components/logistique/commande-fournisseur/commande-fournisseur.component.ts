import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, ValidationErrors, ValidatorFn, AbstractControl, FormGroupDirective, NgForm, AbstractControlOptions } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { InformationComponent } from "../../dialogs/information/information.component";
import { HabilitationLogistique, ContactAdresseProcess, CommandeFournisseurStatut, ModuleName, MenuName, UtilisateurType } from "../../../globals/enums";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import moment from "moment";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { UploadComponent } from "../../dialogs/upload/upload.component";
import { DownloadService } from "../../../services/download.service";
import { getAttachmentFilename, setOriginalMenu, showHtmlInformationToUser, showInformationToUser } from "../../../globals/utils";
import { EmailComponent } from "../../email/email.component";
import { showErrorToUser } from "../../../globals/utils";
import { EventEmitterService } from "../../../services/event-emitter.service";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { ReplaySubject, Subject, takeUntil } from "rxjs";
import { fadeInOnEnterAnimation } from "angular-animations";
import { encode } from 'html-entities';
import { DomSanitizer } from "@angular/platform-browser";
import { UtilsService } from "../../../services/utils.service";
import { BaseFormComponent } from "../../_ancestors/base-form.component";

class MyErrorStateMatcher implements ErrorStateMatcher {
  //Constructor
  constructor(public applicationUserContext: ApplicationUserContext) { }
  isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    let result: boolean = false;
    if (control.parent.get("PoidsReparti") === control) {
      result = !!((form.errors && form.errors.poidsChargementLessThanPoidsReparti) || (control && control.invalid));
    }
    else if (control.parent.get("DDechargement") === control) {
      result = !!((form.errors && form.errors.dDechargementBeforeDChargement) || (control && control.invalid));
    }
    else if (control.parent.get("DDechargementPrevue") === control) {
      result = !!((form.errors && form.errors.dDechargementPrevueBeforeDChargementPrevue)
        || (form.errors && form.errors.dDechargementPrevueOutOfDMoisDechargementPrevu && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur)
        || (control && control.invalid));
    }
    else if (control.parent.get("D") === control) {
      result = !!((form.errors && form.errors.dBeforeToday) || (control && control.invalid));
    }
    else { result = !!((control && control.invalid)); }
    return result;
  }
}

/** NbBalleDechargement can"t be away from nbBalleChargement */
const nbBalleDechargementValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const nbBalleChargementFC = control.get("NbBalleChargement");
  const nbBalleDechargementFC = control.get("NbBalleDechargement");
  return nbBalleDechargementFC.enabled && nbBalleChargementFC && nbBalleDechargementFC && nbBalleChargementFC.value && nbBalleDechargementFC.value && Math.abs(nbBalleChargementFC.value - nbBalleDechargementFC.value) > 0 ? { nbBalleChargementAwayFrombBalleDechargement: true } : null;
}

/** PoidsDechargement can"t be 500kg away from PoidsChargement */
const poidsDechargementValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const poidsChargementFC = control.get("PoidsChargement");
  const poidsDechargementFC = control.get("PoidsDechargement");
  return poidsDechargementFC.enabled && poidsChargementFC && poidsDechargementFC && poidsChargementFC.value && poidsDechargementFC.value && Math.abs(poidsChargementFC.value - poidsDechargementFC.value) >= 500 ? { poidsChargementAwayFromPoidsDechargement: true } : null;
}

/** PoidsChargement less then PoidsReparti */
const poidsRepartiValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const poidsChargementFC = control.get("PoidsChargement");
  const poidsRepartiFC = control.get("PoidsReparti");
  return poidsRepartiFC.enabled && poidsChargementFC && poidsRepartiFC && poidsChargementFC.value && poidsRepartiFC.value && poidsChargementFC.value < poidsRepartiFC.value ? { poidsChargementLessThanPoidsReparti: true } : null;
}

/** PrixTonneHT = 0 */
const prixTonneHTValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const prixTonneHTFC = control.get("PrixTonneHT");
  let result: boolean = (prixTonneHTFC.enabled && prixTonneHTFC && prixTonneHTFC.value !== null && prixTonneHTFC.value === 0);
  return result ? { prixTonneHTEquals0: true } : null;
}

/** RefusCamion related data DDechargement*/
const refusCamionDDechargementValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const refusCamionFC = control.get("RefusCamion");
  const dDechargementFC = control.get("DDechargement");
  let result: boolean = (dDechargementFC.enabled && dDechargementFC && dDechargementFC.value && refusCamionFC.value === true);
  return result ? { refusCamionDDechargementRelatedData: true } : null;
}
/** RefusCamion related data PoidsDechargement*/
const refusCamionPoidsDechargementValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const refusCamionFC = control.get("RefusCamion");
  const poidsDechargementFC = control.get("PoidsDechargement");
  let result: boolean = (poidsDechargementFC.enabled && poidsDechargementFC && poidsDechargementFC.value && refusCamionFC.value === true);
  return result ? { refusCamionPoidsDechargementRelatedData: true } : null;
}
/** RefusCamion related data NbBalleDechargement*/
const refusCamionNbBalleDechargementValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const refusCamionFC = control.get("RefusCamion");
  const nbBalleDechargementFC = control.get("NbBalleDechargement");
  let result: boolean = (nbBalleDechargementFC.enabled && nbBalleDechargementFC && nbBalleDechargementFC.value && refusCamionFC.value === true);
  return result ? { refusCamionNbBalleDechargementRelatedData: true } : null;
}

/** DDechargement can"t be before DChargement */
const dDechargementValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const dChargementFC = control.get("DChargement");
  const dDechargementFC = control.get("DDechargement");
  let result: boolean = false;
  if (dDechargementFC.enabled && dChargementFC && dDechargementFC && dChargementFC.value && dDechargementFC.value && moment(dChargementFC.value).isValid() && moment(dDechargementFC.value).isValid()) {
    result = (moment(dChargementFC.value).isAfter(moment(dDechargementFC.value)));
  }
  return result ? { dDechargementBeforeDChargement: true } : null;
}

/** D can"t be before today */
const dValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const dFC = control.get("D");
  let result: boolean = false;
  if (dFC.enabled && dFC && dFC.value && moment(dFC.value).isValid()) {
    result = (moment().add(-1, "days").isAfter(moment(dFC.value)));
  }
  return result ? { dBeforeToday: true } : null;
}

/** DDechargementPrevue can"t be before DChargementPrevue */
const dDechargementPrevueValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const dChargementPrevueFC = control.get("DChargementPrevue");
  const dDechargementPrevueFC = control.get("DDechargementPrevue");
  let result: boolean = false;
  if (dDechargementPrevueFC.enabled && dChargementPrevueFC && dDechargementPrevueFC && dChargementPrevueFC.value && dDechargementPrevueFC.value && moment(dChargementPrevueFC.value).isValid() && moment(dDechargementPrevueFC.value).isValid()) {
    result = (moment(dChargementPrevueFC.value).isAfter(moment(dDechargementPrevueFC.value)));
  }
  return result ? { dDechargementPrevueBeforeDChargementPrevue: true } : null;
}

/** DDechargementPrevue can"t be out of DMoisDechargementPrevu */
const dDechargementPrevueMoisValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const dMoisDechargementPrevuFC = control.get("DMoisDechargementPrevuList");
  const dDechargementPrevueFC = control.get("DDechargementPrevue");
  let result: boolean = false;
  if (dDechargementPrevueFC && dDechargementPrevueFC.enabled && dMoisDechargementPrevuFC && dMoisDechargementPrevuFC.value && moment(dMoisDechargementPrevuFC.value).isValid() && moment(dDechargementPrevueFC.value).isValid()) {
    result = (moment(dMoisDechargementPrevuFC.value).format("YYYYMM") !== moment(dDechargementPrevueFC.value).format("YYYYMM"));
  }
  return result ? { dDechargementPrevueOutOfDMoisDechargementPrevu: true } : null;
}

@Component({
    selector: "commande-fournisseur",
    templateUrl: "./commande-fournisseur.component.html",
    animations: [
        fadeInOnEnterAnimation(),
    ],
    standalone: false
})

export class CommandeFournisseurComponent extends BaseFormComponent<dataModelsInterfaces.CommandeFournisseur> {
  matcher = new MyErrorStateMatcher(this.applicationUserContext);
  //Variables
  reparti: boolean = false;
  commandeFournisseur: dataModelsInterfaces.CommandeFournisseur = {} as dataModelsInterfaces.CommandeFournisseur;
  //Form
  camionCompletList: appInterfaces.YesNo[];
  motifAnomalieChargementList: dataModelsInterfaces.MotifAnomalieChargement[];
  motifAnomalieClientList: dataModelsInterfaces.MotifAnomalieClient[];
  motifAnomalieTransporteurList: dataModelsInterfaces.MotifAnomalieTransporteur[];
  motifCamionIncompletList: dataModelsInterfaces.MotifCamionIncomplet[];
  commandeFournisseurStatutList: dataModelsInterfaces.CommandeFournisseurStatut[];
  camionTypeList: dataModelsInterfaces.CamionType[];
  centreDeTriList: dataModelsInterfaces.EntiteList[];
  filteredCentreDeTriList: ReplaySubject<dataModelsInterfaces.EntiteList[]> = new ReplaySubject<dataModelsInterfaces.EntiteList[]>(1);
  adresseList: dataModelsInterfaces.Adresse[];
  contactAdresseList: dataModelsInterfaces.ContactAdresse[];
  transporteurContactAdresseList: dataModelsInterfaces.ContactAdresse[];
  clientContactAdresseList: dataModelsInterfaces.ContactAdresse[];
  clientList: dataModelsInterfaces.EntiteList[];
  clientAdresseList: dataModelsInterfaces.Adresse[];
  produitList: dataModelsInterfaces.ProduitList[];
  dMoisDechargementPrevuList: appInterfaces.Month[];
  civiliteList: dataModelsInterfaces.Civilite[];
  paysList: dataModelsInterfaces.Pays[];
  form: UntypedFormGroup;
  numeroCommandeFC: UntypedFormControl = new UntypedFormControl(null);
  numeroAffretementFC: UntypedFormControl = new UntypedFormControl(null);
  ordreAffretementFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(9)]);
  commandeFournisseurStatutListFC: UntypedFormControl = new UntypedFormControl(null);
  cmtBlocageFC: UntypedFormControl = new UntypedFormControl(null);
  camionCompletListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  motifCamionIncompletListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  prixTransportHTFC: UntypedFormControl = new UntypedFormControl(null, Validators.max(5000));
  surcoutCarburantHTFC: UntypedFormControl = new UntypedFormControl(null, Validators.max(9999));
  prixTransportSupplementHTFC: UntypedFormControl = new UntypedFormControl(null, [Validators.max(5000), Validators.min(-5000)]);
  kmFC: UntypedFormControl = new UntypedFormControl(null, Validators.max(3000));
  camionTypeListFC: UntypedFormControl = new UntypedFormControl(null);
  transporteurContactAdresseListFC: UntypedFormControl = new UntypedFormControl(null);
  cmtTransporteurFC: UntypedFormControl = new UntypedFormControl(null);
  centreDeTriListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  centreDeTriListFilterFC: UntypedFormControl = new UntypedFormControl(null);
  libelleFC: UntypedFormControl = new UntypedFormControl(null);
  adr1FC: UntypedFormControl = new UntypedFormControl(null);
  adr2FC: UntypedFormControl = new UntypedFormControl(null);
  codePostalFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  villeFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  paysListFC: UntypedFormControl = new UntypedFormControl(null);
  horairesFC: UntypedFormControl = new UntypedFormControl(null);
  adresseListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  contactAdresseListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  civiliteListFC: UntypedFormControl = new UntypedFormControl(null);
  prenomFC: UntypedFormControl = new UntypedFormControl(null);
  nomFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  emailFC: UntypedFormControl = new UntypedFormControl(null, Validators.email);
  telFC: UntypedFormControl = new UntypedFormControl(null);
  telMobileFC: UntypedFormControl = new UntypedFormControl(null);
  dFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  dMoisDechargementPrevuFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  produitListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  poidsChargementFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(200000)]);
  poidsRepartiFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(200000)]);
  nbBalleChargementFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(600)]);
  dChargementPrevueFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  horaireChargementPrevuFC: UntypedFormControl = new UntypedFormControl(null);
  dChargementFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  chargementEffectueFC: UntypedFormControl = new UntypedFormControl(null);
  chargementAnnuleFC: UntypedFormControl = new UntypedFormControl(null);
  cmtChargementAnnuleFC: UntypedFormControl = new UntypedFormControl(null);
  lotControleFC: UntypedFormControl = new UntypedFormControl();
  cmtFournisseurFC: UntypedFormControl = new UntypedFormControl(null);
  clientListFC: UntypedFormControl = new UntypedFormControl(null);
  clientAdresseListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  clientContactAdresseListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  horaireDechargementPrevuFC: UntypedFormControl = new UntypedFormControl(null);
  dDechargementFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  dDechargementPrevueFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  poidsDechargementFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(200000)]);
  nbBalleDechargementFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(600)]);
  refusCamionFC: UntypedFormControl = new UntypedFormControl(null);
  prixTonneHTFC: UntypedFormControl = new UntypedFormControl(null, Validators.max(2000));
  cmtClientFC: UntypedFormControl = new UntypedFormControl(null);
  imprimeFC: UntypedFormControl = new UntypedFormControl(null);
  exportSAGEFC: UntypedFormControl = new UntypedFormControl(null);
  nonRepartissableFC: UntypedFormControl = new UntypedFormControl(null);
  valideDPrevuesFC: UntypedFormControl = new UntypedFormControl(null);
  traitementAnomalieChargementFC: UntypedFormControl = new UntypedFormControl(null);
  motifAnomalieChargementListFC: UntypedFormControl = new UntypedFormControl(null);
  cmtAnomalieChargementFC: UntypedFormControl = new UntypedFormControl(null);
  traitementAnomalieClientFC: UntypedFormControl = new UntypedFormControl(null);
  motifAnomalieClientListFC: UntypedFormControl = new UntypedFormControl(null);
  cmtAnomalieClientFC: UntypedFormControl = new UntypedFormControl(null);
  traitementAnomalieTransporteurFC: UntypedFormControl = new UntypedFormControl(null);
  motifAnomalieTransporteurListFC: UntypedFormControl = new UntypedFormControl(null);
  cmtAnomalieTransporteurFC: UntypedFormControl = new UntypedFormControl(null);
  //Bloc visibility
  blocTransporteurVisible: boolean = false;
  blocContactAdresseVisible: boolean = false;
  blocClientVisible: boolean = false;
  blocChargementVisible: boolean = false;
  blocDechargementVisible: boolean = false;
  blocAnomalieChargementVisible: boolean = false;
  blocAnomalieClientVisible: boolean = false;
  blocAnomalieTransporteurVisible: boolean = false;
  blocCommandeFournisseurFichiersVisible: boolean = false;
  //Other visibility
  buttonPrintVisible = false;
  cmtFournisseurVisible: boolean = false;
  //Required
  dChargementPrevueFCRequired: boolean = true;
  dDechargementPrevueFCRequired: boolean = true;
  dChargementFCRequired: boolean = true;
  dDechargementFCRequired: boolean = true;
  dMoisDechargementPrevuFCRequired: boolean = true;
  nbBalleDechargementFCRequired: boolean = true;
  poidsDechargementFCRequired: boolean = true;
  poidsRepartiFCRequired: boolean = true;
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = true;
  deleteLocked: boolean = true;
  //Formatted data
  formattedDTraitementAnomalieChargement: string = "";
  formattedDAnomalieChargement: string = "";
  formattedDTraitementAnomalieClient: string = "";
  formattedDAnomalieClient: string = "";
  formattedDTraitementAnomalieTransporteur: string = "";
  formattedDAnomalieTransporteur: string = "";
  //Misc
  lastUploadResut: appInterfaces.UploadResponseBody;
  dMin = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0)).add(-1, "years");
  originalPoidsChargement: number = 0;
  originalPoidsReparti: number = 0;
  // Subject that emits when the component has been destroyed.
  protected _onDestroy = new Subject<void>();
    http: any;
    baseUrl: string;
    downloadService: any;
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
    , protected eventEmitterService: EventEmitterService
  ) {

    super("CommandeFournisseurComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    // create an empty object from the interface
    this.commandeFournisseur = {} as dataModelsInterfaces.CommandeFournisseur;
    // Create the form
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    let formOptions: AbstractControlOptions = {
      validators: Validators.compose([poidsDechargementValidator
        , dDechargementValidator
        , dDechargementPrevueValidator
        , dDechargementPrevueMoisValidator
        , refusCamionDDechargementValidator
        , refusCamionPoidsDechargementValidator
        , poidsRepartiValidator
        , prixTonneHTValidator
        , refusCamionNbBalleDechargementValidator
        , nbBalleDechargementValidator
        , prixTonneHTValidator])
    };
    if (this.applicationUserContext.connectedUtilisateur.CentreDeTri) {
      formOptions = {
        validators: Validators.compose([poidsDechargementValidator
          , dValidator
          , dDechargementValidator
          , dDechargementPrevueValidator
          , dDechargementPrevueMoisValidator
          , refusCamionDDechargementValidator
          , refusCamionPoidsDechargementValidator
          , poidsRepartiValidator
          , prixTonneHTValidator
          , refusCamionNbBalleDechargementValidator
          , nbBalleDechargementValidator
          , prixTonneHTValidator])
      }
    }
    this.form = this.fb.group({
      NumeroCommande: this.numeroCommandeFC,
      NumeroAffretement: this.numeroAffretementFC,
      OrdreAffretement: this.ordreAffretementFC,
      CommandeFournisseurStatutList: this.commandeFournisseurStatutListFC,
      CmtBlocage: this.cmtBlocageFC,
      CamionCompletList: this.camionCompletListFC,
      MotifCamionIncompletList: this.motifCamionIncompletListFC,
      PrixTransportHT: this.prixTransportHTFC,
      SurcoutCarburantHT: this.surcoutCarburantHTFC,
      PrixTransportSupplementHT: this.prixTransportSupplementHTFC,
      Km: this.kmFC,
      CamionTypeList: this.camionTypeListFC,
      TransporteurContactAdresseList: this.transporteurContactAdresseListFC,
      CmtFournisseur: this.cmtFournisseurFC,
      CentreDeTriList: this.centreDeTriListFC,
      CentreDeTriListFilter: this.centreDeTriListFilterFC,
      Libelle: this.libelleFC,
      Adr1: this.adr1FC,
      Adr2: this.adr2FC,
      CodePostal: this.codePostalFC,
      Ville: this.villeFC,
      PaysList: this.paysListFC,
      Horaires: this.horairesFC,
      AdresseList: this.adresseListFC,
      ContactAdresseList: this.contactAdresseListFC,
      CiviliteList: this.civiliteListFC,
      Prenom: this.prenomFC,
      Nom: this.nomFC,
      Email: this.emailFC,
      Tel: this.telFC,
      TelMobile: this.telMobileFC,
      D: this.dFC,
      DMoisDechargementPrevuList: this.dMoisDechargementPrevuFC,
      ProduitList: this.produitListFC,
      PoidsChargement: this.poidsChargementFC,
      PoidsReparti: this.poidsRepartiFC,
      NbBalleChargement: this.nbBalleChargementFC,
      DChargementPrevue: this.dChargementPrevueFC,
      HoraireChargementPrevu: this.horaireChargementPrevuFC,
      DChargement: this.dChargementFC,
      ChargementEffectue: this.chargementEffectueFC,
      ChargementAnnule: this.chargementAnnuleFC,
      CmtChargementAnnule: this.cmtChargementAnnuleFC,
      LotControle: this.lotControleFC,
      CmtTransporteur: this.cmtTransporteurFC,
      ClientList: this.clientListFC,
      ClientAdresseList: this.clientAdresseListFC,
      ClientContactAdresseList: this.clientContactAdresseListFC,
      HoraireDechargementPrevu: this.horaireDechargementPrevuFC,
      DDechargement: this.dDechargementFC,
      DDechargementPrevue: this.dDechargementPrevueFC,
      PoidsDechargement: this.poidsDechargementFC,
      NbBalleDechargement: this.nbBalleDechargementFC,
      RefusCamion: this.refusCamionFC,
      PrixTonneHT: this.prixTonneHTFC,
      CmtClient: this.cmtClientFC,
      Imprime: this.imprimeFC,
      ExportSAGE: this.exportSAGEFC,
      NonRepartissable: this.nonRepartissableFC,
      ValideDPrevues: this.valideDPrevuesFC,
      TraitementAnomalieChargement: this.traitementAnomalieChargementFC,
      MotifAnomalieChargementList: this.motifAnomalieChargementListFC,
      CmtAnomalieChargement: this.cmtAnomalieChargementFC,
      TraitementAnomalieClient: this.traitementAnomalieClientFC,
      MotifAnomalieClientList: this.motifAnomalieClientListFC,
      CmtAnomalieClient: this.cmtAnomalieClientFC,
      TraitementAnomalieTransporteur: this.traitementAnomalieTransporteurFC,
      MotifAnomalieTransporteurList: this.motifAnomalieTransporteurListFC,
      CmtAnomalieTransporteur: this.cmtAnomalieTransporteurFC,
    },
      formOptions);
    this.lockScreen();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.deleteLocked = true;
    this.saveLocked = true;
    this.commandeFournisseurStatutListFC.disable();
    this.cmtBlocageFC.disable();
    this.camionTypeListFC.disable();
    this.camionCompletListFC.disable();
    this.motifCamionIncompletListFC.disable();
    this.prixTransportHTFC.disable();
    this.surcoutCarburantHTFC.disable();
    this.prixTransportSupplementHTFC.disable();
    this.kmFC.disable();
    this.transporteurContactAdresseListFC.disable();
    this.cmtTransporteurFC.disable()
    this.centreDeTriListFC.disable();
    this.libelleFC.disable();
    this.adr1FC.disable();
    this.adr2FC.disable();
    this.codePostalFC.disable();
    this.villeFC.disable();
    this.paysListFC.disable()
    this.horairesFC.disable();
    this.adresseListFC.disable();
    this.contactAdresseListFC.disable();
    this.civiliteListFC.disable();
    this.prenomFC.disable();
    this.nomFC.disable();
    this.emailFC.disable();
    this.telFC.disable();
    this.telMobileFC.disable();
    this.dFC.disable();
    this.dMoisDechargementPrevuFC.disable();
    this.produitListFC.disable();
    this.poidsChargementFC.disable();
    this.poidsRepartiFC.disable();
    this.nbBalleChargementFC.disable();
    this.dChargementPrevueFC.disable();
    this.horaireChargementPrevuFC.disable();
    this.dChargementFC.disable();
    this.chargementEffectueFC.disable();
    this.chargementAnnuleFC.disable();
    this.cmtChargementAnnuleFC.disable();
    this.lotControleFC.disable();
    this.cmtFournisseurFC.disable();
    this.clientListFC.disable();
    this.clientAdresseListFC.disable();
    this.clientContactAdresseListFC.disable();
    this.horaireDechargementPrevuFC.disable();
    this.dDechargementFC.disable();
    this.dDechargementPrevueFC.disable();
    this.poidsDechargementFC.disable();
    this.nbBalleDechargementFC.disable();
    this.refusCamionFC.disable();
    this.prixTonneHTFC.disable();
    this.cmtClientFC.disable();
    this.imprimeFC.disable();
    this.exportSAGEFC.disable();
    this.nonRepartissableFC.disable();
    this.valideDPrevuesFC.disable();
    this.traitementAnomalieChargementFC.disable();
    this.motifAnomalieChargementListFC.disable();
    this.cmtAnomalieChargementFC.disable();
    this.traitementAnomalieClientFC.disable();
    this.motifAnomalieClientListFC.disable();
    this.cmtAnomalieClientFC.disable();
    this.traitementAnomalieTransporteurFC.disable();
    this.motifAnomalieTransporteurListFC.disable();
    this.cmtAnomalieTransporteurFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.saveLocked = false;
    this.deleteLocked = false;
    this.commandeFournisseurStatutListFC.enable();
    this.cmtBlocageFC.enable();
    this.camionCompletListFC.enable();
    this.camionTypeListFC.enable();
    this.motifCamionIncompletListFC.enable();
    this.prixTransportHTFC.enable();
    this.surcoutCarburantHTFC.enable();
    this.prixTransportSupplementHTFC.enable();
    this.kmFC.enable();
    this.transporteurContactAdresseListFC.enable();
    this.cmtTransporteurFC.enable();
    this.centreDeTriListFC.enable();
    this.libelleFC.enable();
    this.adr1FC.enable();
    this.adr2FC.enable();
    this.codePostalFC.enable();
    this.villeFC.enable();
    this.paysListFC.enable();
    this.horairesFC.enable();
    this.adresseListFC.enable();
    this.contactAdresseListFC.enable();
    this.civiliteListFC.enable();
    this.prenomFC.enable();
    this.nomFC.enable();
    this.emailFC.enable();
    this.telFC.enable();
    this.telMobileFC.enable();
    this.dFC.enable();
    this.dMoisDechargementPrevuFC.enable();
    this.produitListFC.enable();
    this.poidsChargementFC.enable();
    this.poidsRepartiFC.enable();
    this.nbBalleChargementFC.enable();
    this.dChargementPrevueFC.enable();
    this.horaireChargementPrevuFC.enable();
    this.dChargementFC.enable();
    this.chargementEffectueFC.enable();
    this.chargementAnnuleFC.enable();
    this.cmtChargementAnnuleFC.enable();
    this.lotControleFC.enable();
    this.cmtFournisseurFC.enable();
    this.clientListFC.enable();
    this.clientAdresseListFC.enable();
    this.clientContactAdresseListFC.enable();
    this.horaireDechargementPrevuFC.enable();
    this.dDechargementFC.enable();
    this.dDechargementPrevueFC.enable();
    this.poidsDechargementFC.enable();
    this.nbBalleDechargementFC.enable();
    this.refusCamionFC.enable();
    this.prixTonneHTFC.enable();
    this.cmtClientFC.enable();
    this.imprimeFC.enable();
    this.exportSAGEFC.enable();
    this.nonRepartissableFC.enable();
    this.valideDPrevuesFC.enable();
    this.traitementAnomalieChargementFC.enable();
    this.motifAnomalieChargementListFC.enable();
    this.cmtAnomalieChargementFC.enable();
    this.traitementAnomalieClientFC.enable();
    this.motifAnomalieClientListFC.enable();
    this.cmtAnomalieClientFC.enable();
    this.traitementAnomalieTransporteurFC.enable();
    this.motifAnomalieTransporteurListFC.enable();
    this.cmtAnomalieTransporteurFC.enable();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Visibilities
    this.blocTransporteurVisible = (this.commandeFournisseur.Transporteur !== null);
    this.blocContactAdresseVisible = (this.commandeFournisseur.Entite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
        || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur
        || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur
        || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.CentreDeTri
      )
    );
    this.blocClientVisible = (this.commandeFournisseur.Entite
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
        || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur
        || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur
      )
    );
    this.blocChargementVisible = (this.commandeFournisseur.Entite !== null);
    this.blocDechargementVisible = (this.commandeFournisseur.AdresseClient
      && this.commandeFournisseur.Transporteur
      && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
        || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur
        || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur
        || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client
      )
    );
    //Anomalie chargement
    this.blocAnomalieChargementVisible = (
      this.commandeFournisseur.ValideDPrevues === true
      && (
        (this.applicationUserContext.connectedUtilisateur.CentreDeTri !== null && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.CentreDeTri)
        || (
          this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
          || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur
        )
      )
    );
    //Anomalie client
    this.blocAnomalieClientVisible = (
      this.commandeFournisseur.ValideDPrevues === true
      && (
        this.applicationUserContext.connectedUtilisateur.Client !== null
        || (
          this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
          || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur)
      )
    );
    //Anomalie transporteur
    this.blocAnomalieTransporteurVisible = (
      this.commandeFournisseur.ValideDPrevues === true
      && (
        this.applicationUserContext.connectedUtilisateur.Transporteur !== null
        || (
          this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
          || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur)
      )
    );
    //Attached files
    this.blocCommandeFournisseurFichiersVisible = (this.commandeFournisseur.CommandeFournisseurFichiers.length > 0);
    //Buttons
    //buttonPrint
    //Hide specific Transporteur
    if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === "Transporteur"
      && (
        (this.commandeFournisseur.Mixte && !this.commandeFournisseur.MixteAndPrintable)
        || (!this.commandeFournisseur.Mixte && (this.commandeFournisseur.DChargementPrevue == null || this.commandeFournisseur.DDechargementPrevue == null)
        ))) {
      this.buttonPrintVisible = false;
    }
    else {
      //Standard visibility
      this.buttonPrintVisible = this.commandeFournisseur.AdresseClient && this.commandeFournisseur.Transporteur
        && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
          || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur
          || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur
          || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client
          || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.CentreDeTri);
    }
    //Global lock
    if ((this.commandeFournisseur.CommandeFournisseurStatut && this.commandeFournisseur.CommandeFournisseurStatut.RefCommandeFournisseurStatut === CommandeFournisseurStatut.Bloquee)
      || this.commandeFournisseur.ExportSAGE === true
      || (this.commandeFournisseur.LockedBy && (this.commandeFournisseur.LockedBy.RefUtilisateur !== this.applicationUserContext.connectedUtilisateur.RefUtilisateur))
      || (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Administrateur
        && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Utilisateur
        && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Transporteur
        && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Client
        && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.CentreDeTri)
    ) {
      this.lockScreen();
      if (!(this.commandeFournisseur.LockedBy && (this.commandeFournisseur.LockedBy.RefUtilisateur !== this.applicationUserContext.connectedUtilisateur.RefUtilisateur))
        && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
          || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur)) {
        if (!(this.commandeFournisseur.ExportSAGE === true)) {
          this.commandeFournisseurStatutListFC.enable();
          this.cmtBlocageFC.enable();
          this.saveLocked = false;
        }
        if (this.commandeFournisseur.ExportSAGE === true) {
          this.exportSAGEFC.enable();
          this.traitementAnomalieChargementFC.enable();
          this.motifAnomalieChargementListFC.enable();
          this.cmtAnomalieChargementFC.enable();
          this.traitementAnomalieClientFC.enable();
          this.motifAnomalieClientListFC.enable();
          this.cmtAnomalieClientFC.enable();
          this.traitementAnomalieTransporteurFC.enable();
          this.motifAnomalieTransporteurListFC.enable();
          this.cmtAnomalieTransporteurFC.enable();
        }
        if (this.commandeFournisseur.ExportSAGE === true
          && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
            || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur)
        ) {
          this.exportSAGEFC.enable();
          this.poidsChargementFC.enable();
          this.poidsRepartiFC.enable();
          this.saveLocked = false;
        }
      }
    }
    else {
      //Init
      this.unlockScreen();
      //Validators
      this.dChargementPrevueFC.clearValidators();
      this.dChargementPrevueFC.updateValueAndValidity();
      this.dDechargementPrevueFC.clearValidators();
      this.dDechargementPrevueFC.updateValueAndValidity();
      this.dChargementFC.clearValidators();
      this.dChargementFC.updateValueAndValidity();
      this.dDechargementFC.clearValidators();
      this.dDechargementFC.updateValueAndValidity();
      this.dMoisDechargementPrevuFC.clearValidators();
      this.dMoisDechargementPrevuFC.updateValueAndValidity();
      this.poidsRepartiFC.clearValidators();
      this.poidsRepartiFC.setValidators([Validators.max(200000)]);
      this.poidsRepartiFC.updateValueAndValidity();
      this.nbBalleDechargementFC.clearValidators();
      this.nbBalleDechargementFC.setValidators([Validators.max(600)]);
      this.nbBalleDechargementFC.updateValueAndValidity();
      this.poidsDechargementFC.clearValidators();
      this.poidsDechargementFC.setValidators([Validators.max(200000)]);
      this.poidsDechargementFC.updateValueAndValidity;
      this.dChargementPrevueFCRequired = false;
      this.dDechargementPrevueFCRequired = false;
      this.dChargementFCRequired = false;
      this.dDechargementFCRequired = false;
      this.poidsRepartiFCRequired = false;
      this.nbBalleDechargementFCRequired = false;
      this.poidsDechargementFCRequired = false;
      //Statics
      this.numeroCommandeFC.disable();
      this.numeroAffretementFC.disable();
      this.clientContactAdresseListFC.disable();
      //Dynamics
      //Blocs
      if (!this.blocTransporteurVisible) {
        this.prixTransportHTFC.disable();
        this.surcoutCarburantHTFC.disable();
        this.prixTransportSupplementHTFC.disable();
        this.kmFC.disable();
        this.transporteurContactAdresseListFC.disable();
      }
      if (!this.blocContactAdresseVisible) {
        this.contactAdresseListFC.disable();
        this.civiliteListFC.disable();
        this.nomFC.disable();
      }
      if (!this.blocChargementVisible) {
        this.poidsChargementFC.disable();
        this.poidsRepartiFC.disable();
        this.nbBalleChargementFC.disable();
        this.dChargementFC.disable();
        this.horaireChargementPrevuFC.disable();
        this.dChargementPrevueFC.disable();
      }
      if (!this.blocClientVisible) {
        this.clientListFC.disable();
        this.clientAdresseListFC.disable();
        this.clientContactAdresseListFC.disable();
      }
      if (!this.blocDechargementVisible) {
        this.poidsDechargementFC.disable();
        this.nbBalleDechargementFC.disable();
        this.dDechargementFC.disable();
        this.dDechargementPrevueFC.disable();
        this.prixTonneHTFC.disable();
      }
      //Details
      if (this.commandeFournisseur.CamionComplet !== false) { this.motifCamionIncompletListFC.disable() }
      if (!this.commandeFournisseur.Adresse) { this.commandeFournisseurStatutListFC.disable() }
      //Depending on CommandeFournisseurStatut
      if (!this.commandeFournisseur.CommandeFournisseurStatut) {
        this.dChargementPrevueFC.disable();
        this.horaireChargementPrevuFC.disable();
        this.dChargementFC.disable();
        this.chargementEffectueFC.disable();
        this.chargementAnnuleFC.disable();
        this.cmtChargementAnnuleFC.disable();
        this.lotControleFC.disable();
      }
      //Depending on client existing
      if (this.commandeFournisseur.AdresseClient) {
        this.produitListFC.disable();
        this.dFC.disable();
        //this.dMoisDechargementPrevuFC.disable();
      }
      if (!this.clientListFC.value) {
        this.clientAdresseListFC.disable();
      }
      //Depending on entite
      if (!this.commandeFournisseur.Entite) {
        this.contactAdresseListFC.disable();
        this.adresseListFC.disable();
        this.produitListFC.disable();
      }
      //Depending on transporteur
      if (this.commandeFournisseur.Transporteur) {
        this.adresseListFC.disable();
        this.clientListFC.disable();
        this.clientAdresseListFC.disable();
        this.commandeFournisseurStatutListFC.disable();
        this.cmtBlocageFC.disable();
        this.camionCompletListFC.disable();
      }
      //Depending on DDechargement
      if (this.commandeFournisseur.DDechargement) {
        this.refusCamionFC.disable();
        this.dChargementPrevueFC.setValidators(Validators.required);
        this.dChargementPrevueFC.updateValueAndValidity();
        this.dDechargementPrevueFC.setValidators(Validators.required);
        this.dDechargementPrevueFC.updateValueAndValidity();
        this.dChargementFC.setValidators(Validators.required);
        this.dChargementFC.updateValueAndValidity();
        this.dDechargementFC.setValidators(Validators.required);
        this.dDechargementFC.updateValueAndValidity();
        this.dMoisDechargementPrevuFC.setValidators(Validators.required);
        this.dMoisDechargementPrevuFC.updateValueAndValidity();
        this.poidsRepartiFC.setValidators([Validators.required, Validators.max(200000)]);
        this.poidsRepartiFC.updateValueAndValidity();
        this.nbBalleDechargementFC.setValidators([Validators.required, Validators.max(600)]);
        this.nbBalleDechargementFC.updateValueAndValidity();
        this.poidsDechargementFC.setValidators([Validators.required, Validators.max(200000)]);
        this.poidsDechargementFC.updateValueAndValidity();
        this.dChargementPrevueFCRequired = true;
        this.dDechargementPrevueFCRequired = true;
        this.dChargementFCRequired = true;
        this.dDechargementFCRequired = true;
        this.poidsRepartiFCRequired = true;
        this.nbBalleDechargementFCRequired = true;
        this.poidsDechargementFCRequired = true;
      }
      else {
        this.prixTonneHTFC.disable();
      }
      //Miscelanous
      if (this.commandeFournisseur.Imprime !== true) {
        this.imprimeFC.disable();
      }
      if (this.commandeFournisseur.DDechargement == null) {
        this.poidsRepartiFC.disable();
      }
      if (this.commandeFournisseur.D == null) {
        this.dMoisDechargementPrevuFC.disable();
      }
      if (this.commandeFournisseur.DChargementPrevue == null || this.commandeFournisseur.DDechargementPrevue == null) {
        this.valideDPrevuesFC.disable();
      }
      //Rights
      if (!(this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)) {
        this.ordreAffretementFC.disable();
      }
      if (!(this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur)) {
        this.nonRepartissableFC.disable();
      }
      //Transporteur
      if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur) {
        this.lockScreen();
        //Anomalies
        this.blocAnomalieChargementVisible = false;
        this.blocAnomalieClientVisible = false;
        if (this.commandeFournisseur.DTraitementAnomalieTransporteur == null) {
          this.motifAnomalieTransporteurListFC.enable();
          this.cmtAnomalieTransporteurFC.enable();
        }
        //Unlock/hide data
        if (this.commandeFournisseur.CommandeFournisseurStatut
          && this.commandeFournisseur.CommandeFournisseurStatut.RefCommandeFournisseurStatut === CommandeFournisseurStatut.Ouverte
          && this.commandeFournisseur.Transporteur
          && this.commandeFournisseur.Transporteur.RefEntite === this.applicationUserContext.connectedUtilisateur.Transporteur.RefEntite
        ) {
          this.saveLocked = false;
          this.dDechargementPrevueFC.enable();
          this.dDechargementPrevueFC.setValidators(Validators.required);
          this.dDechargementPrevueFC.updateValueAndValidity();
          this.horaireDechargementPrevuFC.enable();
          this.transporteurContactAdresseListFC.enable();
          this.dChargementPrevueFC.enable();
          this.dChargementPrevueFC.setValidators(Validators.required);
          this.dChargementPrevueFC.updateValueAndValidity();
          this.horaireChargementPrevuFC.enable();
          this.transporteurContactAdresseListFC.setValidators(Validators.required);
          this.transporteurContactAdresseListFC.updateValueAndValidity();
          //Depending on ValideDPrevues
          if (this.commandeFournisseur.ValideDPrevues) {
            this.dDechargementPrevueFC.disable();
            this.horaireDechargementPrevuFC.disable();
            this.transporteurContactAdresseListFC.disable();
            this.dChargementPrevueFC.disable();
            this.horaireChargementPrevuFC.disable();
          }
        }
      }
      //Client
      if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Client) {
        this.lockScreen();
        //Anomalies
        this.blocAnomalieChargementVisible = false;
        this.blocAnomalieTransporteurVisible = false;
        if (this.commandeFournisseur.DTraitementAnomalieClient == null) {
          this.motifAnomalieClientListFC.enable();
          this.cmtAnomalieClientFC.enable();
        }
        //Unlock/hide data
        this.saveLocked = false;
        this.cmtClientFC.enable();
      }
      //CentreDeTri
      if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.CentreDeTri) {
        this.lockScreen();
        //Unlock data
        //CmtFournisseur enable if not created via API
        if (!this.commandeFournisseur.RefExt) {
          this.cmtFournisseurFC.enable();
          this.saveLocked = false;
        }
        //Anomalies
        this.blocAnomalieClientVisible = false;
        this.blocAnomalieTransporteurVisible = false;
        if (this.commandeFournisseur.DTraitementAnomalieChargement == null && this.blocAnomalieChargementVisible) {
          this.motifAnomalieChargementListFC.enable();
          this.cmtAnomalieChargementFC.enable();
          this.saveLocked = false;
        }
        //Unlock/hide data if no customer attached
        //Deverrouillage de la case chargée et lot contrôlé si date de chargement prévisionnelle renseignée et statut ouvert et commande non réceptionnée
        if (this.commandeFournisseur.DDechargementPrevue && !this.commandeFournisseur.DDechargement
          && this.commandeFournisseur.CommandeFournisseurStatut.RefCommandeFournisseurStatut == CommandeFournisseurStatut.Ouverte) {
          this.chargementEffectueFC.enable();
          this.lotControleFC.enable();
          this.saveLocked = false;
        }
        //While no customer
        if (!this.commandeFournisseur.AdresseClient) {
          this.deleteLocked = false;
          this.dFC.enable();
          this.dFC.setValidators(Validators.required);
          this.dFC.updateValueAndValidity();
          this.camionCompletListFC.enable();
          this.camionCompletListFC.setValidators(Validators.required);
          this.camionCompletListFC.updateValueAndValidity();
          if (this.camionCompletListFC.value == false) {
            this.motifCamionIncompletListFC.enable();
            this.motifCamionIncompletListFC.setValidators(Validators.required);
            this.motifCamionIncompletListFC.updateValueAndValidity();
          }
          this.produitListFC.enable();
          this.produitListFC.setValidators(Validators.required);
          this.produitListFC.updateValueAndValidity();
          this.poidsChargementFC.enable();
          this.poidsChargementFC.setValidators([Validators.required, Validators.max(200000)]);
          this.poidsChargementFC.updateValueAndValidity();
          this.nbBalleChargementFC.enable();
          this.nbBalleChargementFC.setValidators([Validators.required, Validators.max(600)]);
          this.nbBalleChargementFC.updateValueAndValidity();
          if (!this.commandeFournisseur.RefExt) { this.cmtFournisseurFC.enable(); }
          //Affichage de la liste des adresses d'enlèvement si plusieurs adresses disponibles XXXXXX
          if (this.adresseList?.length > 1) {
            this.adresseListFC.enable();
            this.adresseListFC.setValidators(Validators.required);
            this.adresseListFC.updateValueAndValidity();
          }
          this.saveLocked = false;
        }
        //Chargement
        if (this.chargementEffectueFC.value == true) {
          this.poidsChargementFC.enable();
          this.poidsChargementFC.setValidators([Validators.required, Validators.max(200000)]);
          this.poidsChargementFC.updateValueAndValidity();
          this.nbBalleChargementFC.enable();
          this.nbBalleChargementFC.setValidators([Validators.required, Validators.max(600)]);
          this.nbBalleChargementFC.updateValueAndValidity();
          this.dChargementFC.enable();
          this.dChargementFC.setValidators(Validators.required);
          this.dChargementFC.updateValueAndValidity();
          this.saveLocked = false;
        }
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = +this.activatedRoute.snapshot.params["id"];
    // get existing object if id exists
    if (id || id === 0) {
      //Set date min if CDT
      if (this.applicationUserContext.connectedUtilisateur.CentreDeTri) {
        this.dMin = moment(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0, 0));
      }
      //Create an empty object from the interface
      this.commandeFournisseur = {} as dataModelsInterfaces.CommandeFournisseur;
      //Get the oblect
      this.dataModelService.getCommandeFournisseur(id, true).subscribe(result => {
        //Anomalies management
        result.DAnomalieOk = null;
        result.UtilisateurAnomalieOk = null;
        //Get data
        this.commandeFournisseur = result;
        //Update form
        this.updateForm();
        //Set CentreDeTri
        if (id === 0 && this.commandeFournisseur.Entite == null) {
          if (this.applicationUserContext.connectedUtilisateur.CentreDeTri?.RefEntite) {
            this.onEntiteSelected(this.applicationUserContext.connectedUtilisateur.CentreDeTri?.RefEntite);
          }
        }
        //Show CmtFournisseur if applicable
        if (!this.commandeFournisseur.DChargement && !this.commandeFournisseur.RefExt && !!this.commandeFournisseur.CmtFournisseur
          && (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Administrateur
            || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Utilisateur
          )) {
          this.cmtFournisseurVisible = true;
        }
        //Show product specific information if applicable
        this.showProduitCommentaires();
        //Get existing CommandeFournisseurStatut
        this.listService.getListCommandeFournisseurStatut().subscribe(result => {
          this.commandeFournisseurStatutList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing MotifCamionIncomplet
        this.listService.getListMotifCamionIncomplet().subscribe(result => {
          this.motifCamionIncompletList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing CamionType
        this.listService.getListCamionType().subscribe(result => {
          this.camionTypeList = result;
          //Auto select
          if (this.camionTypeList.length === 1) { this.camionTypeListFC.setValue(this.camionTypeList[0].RefCamionType); }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing Pays
        this.listService.getListPays(this.commandeFournisseur.Pays?.RefPays, false).subscribe(result => {
          this.paysList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing Centre De Tri
        this.listService.getListCentreDeTri(null, null, false).subscribe(result => {
          this.centreDeTriList = result;
          this.filteredCentreDeTriList.next(this.centreDeTriList.slice());
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing clientAdresse if applicable
        this.listService.getListAdresse(this.commandeFournisseur.AdresseClient?.RefAdresse
          , this.commandeFournisseur.AdresseClient?.RefEntite
          , ContactAdresseProcess.BonDeLivraison
          , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
          , (this.commandeFournisseur.DMoisDechargementPrevu == null ? moment(new Date(1, 1, 1, 0, 0, 0)) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
          , null, true)
          .subscribe(result => {
            this.clientAdresseList = result;
            //Get existing Client if applicable
            this.listService.getListClient(this.commandeFournisseur.AdresseClient?.RefEntite
              , this.commandeFournisseur.Produit?.RefProduit
              , (this.commandeFournisseur.DMoisDechargementPrevu == null ? moment(new Date(1, 1, 1, 0, 0, 0)) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
              , this.commandeFournisseur.Entite?.RefEntite
              , this.commandeFournisseur.Entite?.RefEntite, true
            )
              .subscribe(result => {
                this.clientList = result;
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            //Get existing clientContactAdresse if applicable
            if (this.commandeFournisseur.AdresseClient) {
              this.listService.getListContactAdresse(null, this.commandeFournisseur.AdresseClient?.RefEntite, this.commandeFournisseur.AdresseClient?.RefAdresse
                , ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
                  this.clientContactAdresseList = result;
                  //Auto select
                  if (this.clientContactAdresseList.length === 1) { this.clientContactAdresseListFC.setValue(this.clientContactAdresseList[0].RefContactAdresse); }
                }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing Civilite
        this.listService.getListCivilite().subscribe(result => {
          this.civiliteList = result;
          //Auto select
          if (this.civiliteList.length === 1) { this.civiliteListFC.setValue(this.civiliteList[0].RefCivilite); }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing CamionComplet
        this.listService.getYesNoList().subscribe(result => {
          this.camionCompletList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing adresse
        if (this.commandeFournisseur.Entite) {
          this.listService.getListAdresse(this.commandeFournisseur.Adresse?.RefAdresse, this.commandeFournisseur.Entite?.RefEntite
            , ContactAdresseProcess.BonDeLivraison, null, null, null, true).subscribe(result => {
              this.adresseList = result;
              this.manageScreen();
              //Select adresse if exists
              if (this.commandeFournisseur.Adresse) {
                this.adresseListFC.setValue(this.commandeFournisseur.Adresse.RefAdresse);
              }
              //Auto select if no adresse selected
              else if (this.adresseList && this.adresseList.length === 1 && !this.commandeFournisseur.Adresse) {
                this.adresseListFC.setValue(this.adresseList[0].RefAdresse);
              }
              //Get existing contactAdresse
              this.listService.getListContactAdresse(this.commandeFournisseur.ContactAdresse?.RefContactAdresse, this.commandeFournisseur.Entite?.RefEntite, this.commandeFournisseur.Adresse?.RefAdresse, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
                this.contactAdresseList = result;
                //Auto select
                if (this.contactAdresseList.length === 1
                  && (this.commandeFournisseur.ContactAdresse == null || (this.commandeFournisseur.ContactAdresse.RefContactAdresse === this.contactAdresseList[0].RefContactAdresse))
                ) {
                  this.contactAdresseListFC.setValue(this.contactAdresseList[0].RefContactAdresse);
                }
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        //Get existing transporteurContactAdresse
        if (this.commandeFournisseur.Transporteur) {
          this.listService.getListContactAdresse(this.commandeFournisseur.TransporteurContactAdresse?.RefContactAdresse, this.commandeFournisseur.Transporteur?.RefEntite, null, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
            this.transporteurContactAdresseList = result;
            //Auto select
            if (this.transporteurContactAdresseList.length === 1) { this.transporteurContactAdresseListFC.setValue(this.transporteurContactAdresseList[0].RefContactAdresse); }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        //Get existing month
        if (this.commandeFournisseur.D) {
          this.listService.getListdMoisDechargementPrevuMonth(
            (this.commandeFournisseur.DMoisDechargementPrevu ? this.commandeFournisseur.DMoisDechargementPrevu : this.commandeFournisseur.D) as moment.Moment).subscribe(result => {
              this.dMoisDechargementPrevuList = result;
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        else {
          this.dMoisDechargementPrevuList = [];
        }
        //Get existing Produit
        if (this.commandeFournisseur.Entite) {
          this.listService.getListProduit(this.commandeFournisseur.Produit?.RefProduit, this.commandeFournisseur.Entite?.RefEntite
            , null, null, null, null, null, false, true).subscribe(result => {
              this.produitList = result;
              //Auto select
              if (this.produitList.length === 1) { this.produitListFC.setValue(this.produitList[0].RefProduit); }
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        else {
          this.produitList = [];
        }
        //Get existing MotifAnomalieChargement
        this.listService.getListMotifAnomalieChargement().subscribe(result => {
          this.motifAnomalieChargementList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing MotifAnomalieClient
        this.listService.getListMotifAnomalieClient().subscribe(result => {
          this.motifAnomalieClientList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get existing MotifAnomalieTransporteur
        this.listService.getListMotifAnomalieTransporteur().subscribe(result => {
          this.motifAnomalieTransporteurList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Check if linked to a repartition
        this.dataModelService.hasRepartition(this.commandeFournisseur.RefCommandeFournisseur)
          .subscribe((result: boolean) => {
            this.reparti = result;
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Check similar
        this.checkSimilar();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    this.form.get("CentreDeTriListFilter").valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterCentreDeTrisMulti();
      });
  }
  //-----------------------------------------------------------------------------------
  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.numeroCommandeFC.setValue(this.commandeFournisseur.NumeroCommande);
    this.numeroAffretementFC.setValue(this.commandeFournisseur.NumeroAffretement);
    this.ordreAffretementFC.setValue(this.commandeFournisseur.OrdreAffretement);
    this.commandeFournisseurStatutListFC.setValue((this.commandeFournisseur.CommandeFournisseurStatut ? this.commandeFournisseur.CommandeFournisseurStatut.RefCommandeFournisseurStatut : null));
    this.cmtBlocageFC.setValue(this.commandeFournisseur.CmtBlocage);
    this.camionCompletListFC.setValue(this.commandeFournisseur.CamionComplet);
    this.motifCamionIncompletListFC.setValue((this.commandeFournisseur.MotifCamionIncomplet ? this.commandeFournisseur.MotifCamionIncomplet.RefMotifCamionIncomplet : null));
    this.prixTransportHTFC.setValue(this.commandeFournisseur.PrixTransportHT);
    this.surcoutCarburantHTFC.setValue(this.commandeFournisseur.SurcoutCarburantHT);
    this.prixTransportSupplementHTFC.setValue(this.commandeFournisseur.PrixTransportSupplementHT);
    this.kmFC.setValue((this.commandeFournisseur.Km === 0 && !(this.commandeFournisseur.Mixte && this.commandeFournisseur.NumeroAffretement > 1)) ? null : this.commandeFournisseur.Km);
    this.camionTypeListFC.setValue((this.commandeFournisseur.CamionType ? this.commandeFournisseur.CamionType.RefCamionType : null));
    this.transporteurContactAdresseListFC.setValue((this.commandeFournisseur.TransporteurContactAdresse ? this.commandeFournisseur.TransporteurContactAdresse.RefContactAdresse : null));
    this.cmtTransporteurFC.setValue(this.commandeFournisseur.CmtTransporteur);
    this.centreDeTriListFC.setValue((this.commandeFournisseur.Entite ? this.commandeFournisseur.Entite.RefEntite : null));
    this.adresseListFC.setValue((this.commandeFournisseur.Adresse ? this.commandeFournisseur.Adresse.RefAdresse : null));
    this.libelleFC.setValue(this.commandeFournisseur.Libelle);
    this.adr1FC.setValue(this.commandeFournisseur.Adr1);
    this.adr2FC.setValue(this.commandeFournisseur.Adr2);
    this.codePostalFC.setValue(this.commandeFournisseur.CodePostal);
    this.villeFC.setValue(this.commandeFournisseur.Ville);
    this.paysListFC.setValue((this.commandeFournisseur.Pays ? this.commandeFournisseur.Pays.RefPays : null));
    this.horairesFC.setValue(this.commandeFournisseur.Horaires);
    this.contactAdresseListFC.setValue((this.commandeFournisseur.ContactAdresse ? this.commandeFournisseur.ContactAdresse.RefContactAdresse : null));
    this.civiliteListFC.setValue((this.commandeFournisseur.Civilite ? this.commandeFournisseur.Civilite.RefCivilite : null));
    this.prenomFC.setValue(this.commandeFournisseur.Prenom);
    this.nomFC.setValue(this.commandeFournisseur.Nom);
    this.emailFC.setValue(this.commandeFournisseur.Email);
    this.telFC.setValue(this.commandeFournisseur.Tel);
    this.telMobileFC.setValue(this.commandeFournisseur.TelMobile);
    this.dFC.setValue(this.commandeFournisseur.D);
    this.dMoisDechargementPrevuFC.setValue(this.commandeFournisseur.DMoisDechargementPrevu ? (this.commandeFournisseur.DMoisDechargementPrevu as moment.Moment).format("YYYY-MM-DDT00:00:00") : null);
    this.produitListFC.setValue((this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : null));
    this.poidsChargementFC.setValue(this.commandeFournisseur.PoidsChargement === 0 ? null : this.commandeFournisseur.PoidsChargement);
    this.poidsRepartiFC.setValue(this.commandeFournisseur.PoidsReparti);
    this.nbBalleChargementFC.setValue(this.commandeFournisseur.NbBalleChargement === 0 ? null : this.commandeFournisseur.NbBalleChargement);
    this.dChargementPrevueFC.setValue(this.commandeFournisseur.DChargementPrevue);
    this.horaireChargementPrevuFC.setValue(this.commandeFournisseur.HoraireChargementPrevu);
    this.dChargementFC.setValue(this.commandeFournisseur.DChargement);
    this.chargementEffectueFC.setValue(this.commandeFournisseur.ChargementEffectue);
    this.chargementAnnuleFC.setValue(this.commandeFournisseur.ChargementAnnule);
    this.cmtChargementAnnuleFC.setValue(this.commandeFournisseur.CmtChargementAnnule);
    this.lotControleFC.setValue(this.commandeFournisseur.LotControle);
    this.cmtFournisseurFC.setValue(this.commandeFournisseur.CmtFournisseur);
    this.clientListFC.setValue((this.commandeFournisseur.AdresseClient ? this.commandeFournisseur.AdresseClient.RefEntite : null));
    this.clientAdresseListFC.setValue((this.commandeFournisseur.AdresseClient ? this.commandeFournisseur.AdresseClient.RefAdresse : null));
    this.horaireDechargementPrevuFC.setValue(this.commandeFournisseur.HoraireDechargementPrevu);
    this.dDechargementFC.setValue(this.commandeFournisseur.DDechargement);
    this.dDechargementPrevueFC.setValue(this.commandeFournisseur.DDechargementPrevue);
    this.poidsDechargementFC.setValue(this.commandeFournisseur.PoidsDechargement === 0 ? null : this.commandeFournisseur.PoidsDechargement);
    this.nbBalleDechargementFC.setValue(this.commandeFournisseur.NbBalleDechargement === 0 ? null : this.commandeFournisseur.NbBalleDechargement);
    this.refusCamionFC.setValue(this.commandeFournisseur.RefusCamion);
    this.prixTonneHTFC.setValue(this.commandeFournisseur.PrixTonneHT);
    this.cmtClientFC.setValue(this.commandeFournisseur.CmtClient);
    this.valideDPrevuesFC.setValue(this.commandeFournisseur.ValideDPrevues);
    this.exportSAGEFC.setValue(this.commandeFournisseur.ExportSAGE);
    this.nonRepartissableFC.setValue(this.commandeFournisseur.NonRepartissable);
    this.motifAnomalieChargementListFC.setValue((this.commandeFournisseur.MotifAnomalieChargement ? this.commandeFournisseur.MotifAnomalieChargement.RefMotifAnomalieChargement : null));
    this.cmtAnomalieChargementFC.setValue(this.commandeFournisseur.CmtAnomalieChargement);
    this.traitementAnomalieChargementFC.setValue(this.commandeFournisseur.DTraitementAnomalieChargement ? true : false);
    this.motifAnomalieClientListFC.setValue((this.commandeFournisseur.MotifAnomalieClient ? this.commandeFournisseur.MotifAnomalieClient.RefMotifAnomalieClient : null));
    this.cmtAnomalieClientFC.setValue(this.commandeFournisseur.CmtAnomalieClient);
    this.traitementAnomalieClientFC.setValue(this.commandeFournisseur.DTraitementAnomalieClient ? true : false);
    this.motifAnomalieTransporteurListFC.setValue((this.commandeFournisseur.MotifAnomalieTransporteur ? this.commandeFournisseur.MotifAnomalieTransporteur.RefMotifAnomalieTransporteur : null));
    this.cmtAnomalieTransporteurFC.setValue(this.commandeFournisseur.CmtAnomalieTransporteur);
    this.traitementAnomalieTransporteurFC.setValue(this.commandeFournisseur.DTraitementAnomalieTransporteur ? true : false);
    this.formattedDAnomalieChargement = (this.commandeFournisseur.DAnomalieChargement ? moment(this.commandeFournisseur.DAnomalieChargement).format("LLL") : "");
    this.formattedDTraitementAnomalieChargement = (this.commandeFournisseur.DTraitementAnomalieChargement ? moment(this.commandeFournisseur.DTraitementAnomalieChargement).format("LLL") : "");
    this.formattedDAnomalieClient = (this.commandeFournisseur.DAnomalieClient ? moment(this.commandeFournisseur.DAnomalieClient).format("LLL") : "");
    this.formattedDTraitementAnomalieClient = (this.commandeFournisseur.DTraitementAnomalieClient ? moment(this.commandeFournisseur.DTraitementAnomalieClient).format("LLL") : "");
    this.formattedDAnomalieTransporteur = (this.commandeFournisseur.DAnomalieTransporteur ? moment(this.commandeFournisseur.DAnomalieTransporteur).format("LLL") : "");
    this.formattedDTraitementAnomalieTransporteur = (this.commandeFournisseur.DTraitementAnomalieTransporteur ? moment(this.commandeFournisseur.DTraitementAnomalieTransporteur).format("LLL") : "");
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.commandeFournisseur.OrdreAffretement = this.ordreAffretementFC.value;
    this.commandeFournisseur.CmtBlocage = this.cmtBlocageFC.value;
    this.commandeFournisseur.CamionComplet = this.camionCompletListFC.value;
    if (this.motifCamionIncompletList) { this.commandeFournisseur.MotifCamionIncomplet = this.motifCamionIncompletList.find(x => x.RefMotifCamionIncomplet === this.motifCamionIncompletListFC.value); }
    this.commandeFournisseur.PrixTransportHT = (this.prixTransportHTFC.value == null ? 0 : this.prixTransportHTFC.value);
    this.commandeFournisseur.SurcoutCarburantHT = (this.surcoutCarburantHTFC.value == null ? 0 : this.surcoutCarburantHTFC.value);
    this.commandeFournisseur.PrixTransportSupplementHT = (this.prixTransportSupplementHTFC.value == null ? 0 : this.prixTransportSupplementHTFC.value);
    this.commandeFournisseur.Km = (this.kmFC.value == null ? 0 : this.kmFC.value);
    if (this.camionTypeList) { this.commandeFournisseur.CamionType = this.camionTypeList.find(x => x.RefCamionType == this.camionTypeListFC.value); }
    if (this.transporteurContactAdresseList) { this.commandeFournisseur.TransporteurContactAdresse = (this.transporteurContactAdresseList ? this.transporteurContactAdresseList.find(x => x.RefContactAdresse === this.transporteurContactAdresseListFC.value) : null); }
    this.commandeFournisseur.CmtTransporteur = this.cmtTransporteurFC.value;
    this.commandeFournisseur.Libelle = this.libelleFC.value;
    this.commandeFournisseur.Adr1 = this.adr1FC.value;
    this.commandeFournisseur.Adr2 = this.adr2FC.value;
    this.commandeFournisseur.CodePostal = this.codePostalFC.value;
    this.commandeFournisseur.Ville = this.villeFC.value;
    if (this.paysList) { this.commandeFournisseur.Pays = this.paysList.find(x => x.RefPays === this.paysListFC.value); }
    this.commandeFournisseur.Horaires = this.horairesFC.value;
    if (this.civiliteList) { this.commandeFournisseur.Civilite = this.civiliteList.find(x => x.RefCivilite === this.civiliteListFC.value); }
    this.commandeFournisseur.Prenom = this.prenomFC.value;
    this.commandeFournisseur.Nom = this.nomFC.value;
    this.commandeFournisseur.Email = this.emailFC.value;
    this.commandeFournisseur.Tel = this.telFC.value;
    this.commandeFournisseur.TelMobile = this.telMobileFC.value;
    this.commandeFournisseur.D = (this.dFC.value == null ? null : this.dFC.value);
    this.commandeFournisseur.DMoisDechargementPrevu = (this.dMoisDechargementPrevuFC.value == null ? null : moment(this.dMoisDechargementPrevuFC.value));
    this.commandeFournisseur.PoidsChargement = (this.poidsChargementFC.value == null ? 0 : this.poidsChargementFC.value);
    this.commandeFournisseur.NbBalleChargement = (this.nbBalleChargementFC.value == null ? 0 : this.nbBalleChargementFC.value);
    this.commandeFournisseur.DChargementPrevue = (this.dChargementPrevueFC.value == null ? null : this.dChargementPrevueFC.value);
    this.commandeFournisseur.HoraireChargementPrevu = this.horaireChargementPrevuFC.value;
    this.commandeFournisseur.DChargement = (this.dChargementFC.value == null ? null : this.dChargementFC.value);
    this.commandeFournisseur.ChargementEffectue = this.chargementEffectueFC.value;
    this.commandeFournisseur.ChargementAnnule = this.chargementAnnuleFC.value;
    this.commandeFournisseur.CmtChargementAnnule = this.cmtChargementAnnuleFC.value;
    this.commandeFournisseur.LotControle = this.lotControleFC.value;
    if (!(this.applicationUserContext.connectedUtilisateur.CentreDeTri && this.commandeFournisseur.RefExt)) {
      this.commandeFournisseur.CmtFournisseur = this.cmtFournisseurFC.value;
    }
    this.commandeFournisseur.HoraireDechargementPrevu = this.horaireDechargementPrevuFC.value;
    this.commandeFournisseur.DDechargementPrevue = (this.dDechargementPrevueFC.value == null ? null : this.dDechargementPrevueFC.value);
    this.commandeFournisseur.PoidsDechargement = (this.poidsDechargementFC.value == null ? 0 : this.poidsDechargementFC.value);
    this.commandeFournisseur.PoidsReparti = (this.commandeFournisseur.DDechargement == null && this.commandeFournisseur.Produit && this.commandeFournisseur.Produit.Collecte ? 0 : (this.poidsRepartiFC.value == null ? 0 : this.poidsRepartiFC.value));
    this.commandeFournisseur.DDechargement = (this.dDechargementFC.value == null ? null : this.dDechargementFC.value);
    this.commandeFournisseur.NbBalleDechargement = (this.nbBalleDechargementFC.value == null ? 0 : this.nbBalleDechargementFC.value);
    this.commandeFournisseur.RefusCamion = this.refusCamionFC.value;
    this.commandeFournisseur.PrixTonneHT = (this.prixTonneHTFC.value == null ? 0 : this.prixTonneHTFC.value);
    this.commandeFournisseur.CmtClient = this.cmtClientFC.value;
    this.commandeFournisseur.ValideDPrevues = this.valideDPrevuesFC.value;
    this.commandeFournisseur.ExportSAGE = this.exportSAGEFC.value;
    this.commandeFournisseur.NonRepartissable = this.nonRepartissableFC.value;
    if (this.motifAnomalieChargementList) { this.commandeFournisseur.MotifAnomalieChargement = this.motifAnomalieChargementList.find(x => x.RefMotifAnomalieChargement === this.motifAnomalieChargementListFC.value); }
    this.commandeFournisseur.CmtAnomalieChargement = this.cmtAnomalieChargementFC.value;
    this.commandeFournisseur.DTraitementAnomalieChargement = (this.traitementAnomalieChargementFC.value === true ? moment() : null);
    if (this.motifAnomalieClientList) { this.commandeFournisseur.MotifAnomalieClient = this.motifAnomalieClientList.find(x => x.RefMotifAnomalieClient === this.motifAnomalieClientListFC.value); }
    this.commandeFournisseur.CmtAnomalieClient = this.cmtAnomalieClientFC.value;
    this.commandeFournisseur.DTraitementAnomalieClient = (this.traitementAnomalieClientFC.value === true ? moment() : null);
    if (this.motifAnomalieTransporteurList) { this.commandeFournisseur.MotifAnomalieTransporteur = this.motifAnomalieTransporteurList.find(x => x.RefMotifAnomalieTransporteur === this.motifAnomalieTransporteurListFC.value); }
    this.commandeFournisseur.CmtAnomalieTransporteur = this.cmtAnomalieTransporteurFC.value;
    this.commandeFournisseur.DTraitementAnomalieTransporteur = (this.traitementAnomalieTransporteurFC.value === true ? moment() : null);
    //Handle PoidsChargement and PoidsReparti change
    if ((this.commandeFournisseur.PoidsChargement != this.originalPoidsChargement || this.commandeFournisseur.PoidsReparti != this.originalPoidsReparti)
      && this.commandeFournisseur.Reparti && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique == HabilitationLogistique.Administrateur){
      const dialogRef = this.dialog.open(InformationComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(1561) },
        autoFocus: false,
        restoreFocus: false
      });
      this.originalPoidsChargement = this.commandeFournisseur.PoidsChargement;
      this.originalPoidsReparti = this.commandeFournisseur.PoidsReparti;
    }
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(772) },
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    this.dataModelService.deleteCommandeFournisseur(this.commandeFournisseur.RefCommandeFournisseur)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(773), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Set original Menu
        setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
        //Route back to grid
        this.router.navigate(["grid"]);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  getFormValidationErrors() {
    Object.keys(this.form.controls).forEach(key => {
      const controlErrors: ValidationErrors = this.form.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach(keyError => {
          console.log("Key control: " + key + ", keyError: " + keyError + ", err value: ", controlErrors[keyError]);
        });
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  isFormValidForSave(): boolean {
    let r: boolean = false;
    let formErrors: boolean = false;
    let controlErrors: boolean = false;
    let realErrorsCount: number = 0;
    //Check form errors
    if (this.form.errors) {
      //console.log(this.form.errors);
      //Count real errors
      Object.keys(this.form.errors).forEach(key => {
        if ((key !== "nbBalleChargementAwayFrombBalleDechargement"
          && key !== "poidsChargementAwayFromPoidsDechargement"
          && key !== "prixTonneHTEquals0"
          && key !== "refusCamionDDechargementRelatedData"
          && key !== "dDechargementPrevueOutOfDMoisDechargementPrevu"
        )
          || (key === "dDechargementPrevueOutOfDMoisDechargementPrevu" && this.applicationUserContext.connectedUtilisateur.HabilitationLogistique === HabilitationLogistique.Transporteur)
        ) {
          realErrorsCount += 1;
          controlErrors = true;
        }
      });
      formErrors = realErrorsCount > 0;
    }
    //Check controls errors
    Object.keys(this.form.controls).forEach(key => {
      if (this.form.get(key).errors) {
        //console.log(this.form.get(key).errors);
        controlErrors = true;
      }
    });
    //End
    r = !formErrors && !controlErrors;
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(nextAction: string) {
    this.getFormValidationErrors();
    if (this.isFormValidForSave()) {
      this.saveData();
      //Update warnings
      if (this.form.errors
        && (this.form.errors.prixTonneHTEquals0
          || this.form.errors.nbBalleChargementAwayFrombBalleDechargement
          || this.form.errors.poidsChargementAwayFromPoidsDechargement
          || this.form.errors.refusCamionDDechargementRelatedData
          || this.form.errors.refusCamionPoidsDechargementRelatedData
          || this.form.errors.refusCamionNbBalleDechargementRelatedData
          || this.form.errors.dDechargementPrevueOutOfDMoisDechargementPrevu)) {
        this.commandeFournisseur.UtilisateurAnomalieOk = this.applicationUserContext.connectedUtilisateur;
      }
      //Process
      //Update DB
      this.dataModelService.postCommandeFournisseur(this.commandeFournisseur, true).subscribe(result => {
        //Check if another CommandeFournisseur have been sent to be processed
        if (result.RefCommandeFournisseur === this.commandeFournisseur.RefCommandeFournisseur
          || this.commandeFournisseur.RefCommandeFournisseur <= 0) {
          this.commandeFournisseur = result;
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
          //Next action if applicable
          if (nextAction !== "") {
            switch (nextAction) {
              case "EmailAnomalieChargement":
                this.createEmail("AnomalieChargement");
                break;
              case "EmailAnomalieTransporteur":
                this.createEmail("AnomalieTransporteur");
                break;
              case "EmailAnomalieClient":
                this.createEmail("AnomalieClient");
                break;
              case "EmailBLChargement":
                this.createEmail("BLChargement");
                break;
              case "EmailBLClient":
                this.createEmail("BLClient");
                break;
              case "Print":
                this.print();
                break;
            }
          }
          else {
            //Set original Menu
            setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
            //Route back to grid
            this.router.navigate(["grid"]);
          }
        }
        else {
          this.commandeFournisseur = result;
          this.updateForm();
          const dialogRef = this.dialog.open(InformationComponent, {
            width: "350px",
            data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(537) },
            autoFocus: false,
            restoreFocus: false
          });
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Unlock CommandeFournisseur
  //Route back to the list
  onUnlock(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(1475) },
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.unlock();
      }
    });
  }
  unlock() {
    this.dataModelService.unlockCommandeFournisseur(this.commandeFournisseur.RefCommandeFournisseur)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1476), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Set original Menu
        setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
        //Route back to grid
        this.router.navigate(["grid"]);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Show ContactAdresse details
  showDetails(type: string, ref: number) {
    let data: any;
    let dialogWidth: number = 350;
    //Init
    this.saveData();
    switch (type) {
      case "CommandeFournisseurAdresse":
      case "CommandeFournisseurClientAdresse":
        //Set width
        dialogWidth = 900;
        //Data
        data = { title: this.applicationUserContext.getCulturedRessourceText(534), type: type, ref: ref };
        break;
      case "CommandeFournisseurContactAdresse":
      case "CommandeFournisseurTransporteurContactAdresse":
      case "CommandeFournisseurClientContactAdresse":
        data = { title: this.applicationUserContext.getCulturedRessourceText(487), type: type, ref: ref };
        break;
      case "CommandeFournisseurTransport":
        data = {
          title: this.applicationUserContext.getCulturedRessourceText(521), type: type
          , ref: this.commandeFournisseur.Adresse.RefAdresse
          , ref1: this.commandeFournisseur.AdresseClient.RefAdresse
          , ref2: this.commandeFournisseur.Transporteur.RefEntite
          , ref3: this.commandeFournisseur.CamionType.RefCamionType
        };
        break;
    }
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: dialogWidth.toString() + "px",
      data,
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      //If data in the dialog have been saved
      if (result) {
        switch (result.type) {
          case "CommandeFournisseurAdresse":
            //Get existing adresse
            this.listService.getListAdresse(this.commandeFournisseur.Adresse?.RefAdresse, this.commandeFournisseur.Adresse?.RefEntite
              , ContactAdresseProcess.BonDeLivraison, null, null, null, true)
              .subscribe(result => {
                this.adresseList = result;
                //Update BL data
                let a = this.adresseList.find(x => x.RefAdresse == this.adresseListFC.value);
                this.libelleFC.setValue(a.Libelle);
                this.adr1FC.setValue(a.Adr1);
                this.adr2FC.setValue(a.Adr2);
                this.codePostalFC.setValue(a.CodePostal);
                this.villeFC.setValue(a.Ville);
                this.paysListFC.setValue(a.Pays.RefPays);
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "CommandeFournisseurTransport":
            //Get existing contactAdresse
            this.dataModelService.getTransport(result.ref).subscribe(result => {
              //Update data
              this.kmFC.setValue(result.Parcours.Km);
              this.prixTransportHTFC.setValue(result.PUHT);
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "CommandeFournisseurContactAdresse":
            //Get existing contactAdresse
            this.listService.getListContactAdresse(this.commandeFournisseur.ContactAdresse?.RefContactAdresse, this.commandeFournisseur.Entite?.RefEntite, null, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
              this.contactAdresseList = result;
              //Update BL data
              let cA = this.contactAdresseList.find(x => x.RefContactAdresse == this.contactAdresseListFC.value);
              this.civiliteListFC.setValue((cA.Contact.Civilite ? cA.Contact.Civilite.RefCivilite : null));
              this.prenomFC.setValue(cA.Contact.Prenom);
              this.nomFC.setValue(cA.Contact.Nom);
              this.emailFC.setValue(cA.Email);
              this.telFC.setValue(cA.Tel);
              this.telMobileFC.setValue(cA.TelMobile);
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "CommandeFournisseurTransporteurContactAdresse":
            //Get existing transporteurContactAdresse
            this.listService.getListContactAdresse(this.commandeFournisseur.TransporteurContactAdresse?.RefContactAdresse, this.commandeFournisseur.Transporteur?.RefEntite, null, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
              this.transporteurContactAdresseList = result;
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "CommandeFournisseurClientAdresse":
            //Get existing clientAdresse
            this.listService.getListAdresse(this.commandeFournisseur.AdresseClient?.RefAdresse, this.commandeFournisseur.AdresseClient?.RefEntite
              , ContactAdresseProcess.BonDeLivraison
              , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
              , (this.commandeFournisseur.DMoisDechargementPrevu == null ? moment(new Date(1, 1, 1, 0, 0, 0)) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
              , null, true)
              .subscribe(result => {
                this.clientAdresseList = result;
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "CommandeFournisseurClientContactAdresse":
            //Get existing clientContactAdresse
            this.listService.getListContactAdresse(null, this.commandeFournisseur.AdresseClient?.RefEntite, null, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
              this.clientContactAdresseList = result;
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
        }
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Find carriage
  onFindCible() {
    //Update data
    this.saveData();
    let transportOnly: boolean = this.commandeFournisseur.ValideDPrevues;
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "75%",
      maxHeight: "80vh",
      data: {
        title: this.applicationUserContext.getCulturedRessourceText(489), type: "Cible"
        , refCommandeFournisseur: this.commandeFournisseur.RefCommandeFournisseur
        , refFournisseur: this.commandeFournisseur.Entite.RefEntite
        , refAdresseFournisseur: this.commandeFournisseur.Adresse.RefAdresse
        , refTransporteur: (transportOnly ? this.commandeFournisseur.Transporteur.RefEntite : null)
        , refProduit: this.commandeFournisseur.Produit.RefProduit
        , refCamionType: ((this.commandeFournisseur.CamionType && !transportOnly) ? this.commandeFournisseur.CamionType.RefCamionType : null)
        , refClient: (this.commandeFournisseur.AdresseClient ? this.commandeFournisseur.AdresseClient.RefEntite : null)
        , dMoisDechargementPrevu: this.commandeFournisseur.DMoisDechargementPrevu
      },
      autoFocus: false,
      restoreFocus: false
    });
    //Register dialog after close event
    dialogRef.afterClosed().subscribe(result => {
      //If data in the dialog have been saved
      if (result) {
        if (result.type === "Cible") {
          let newTransport: dataModelsInterfaces.Transport;
          //Get chosen transport
          if (result.ref && result.ref != 0) {
            this.dataModelService.getTransport(result.ref).subscribe(result => {
              newTransport = result;
              //Update data
              if (newTransport) {
                //Delete old data 
                if (!transportOnly) {
                  this.commandeFournisseur.DChargement = null;
                  this.commandeFournisseur.DChargementPrevue = null;
                  this.commandeFournisseur.DDechargement = null;
                  this.commandeFournisseur.DDechargementPrevue = null;
                  this.commandeFournisseur.ValideDPrevues = false;
                  this.commandeFournisseur.PoidsDechargement = 0;
                  this.commandeFournisseur.NbBalleDechargement = 0;
                  this.commandeFournisseur.TicketPeseeDechargement = false;
                  this.commandeFournisseur.TicketPeseeChargement = false;
                  this.commandeFournisseur.Transporteur = null;
                  this.commandeFournisseur.TransporteurContactAdresse = null;
                  this.commandeFournisseur.PrixTransportHT = 0;
                  this.commandeFournisseur.SurcoutCarburantHT = 0;
                  this.commandeFournisseur.PrixTransportSupplementHT = 0;
                  this.commandeFournisseur.MotifAnomalieChargement = null;
                  this.commandeFournisseur.DAnomalieChargement = null;
                  this.commandeFournisseur.ChargementEffectue = false;
                  this.commandeFournisseur.ChargementAnnule = false;
                  this.commandeFournisseur.CmtChargementAnnule = "";
                  this.commandeFournisseur.LotControle = false;
                  this.commandeFournisseur.CmtClient = "";
                  this.commandeFournisseur.CmtTransporteur = "";
                  this.commandeFournisseur.Imprime = false;
                }
                this.commandeFournisseur.Transporteur = null;
                this.commandeFournisseur.TransporteurContactAdresse = null;
                this.commandeFournisseur.PrixTransportHT = 0;
                this.commandeFournisseur.SurcoutCarburantHT = 0;
                this.commandeFournisseur.PrixTransportSupplementHT = 0;
                this.commandeFournisseur.NumeroAffretement = this.commandeFournisseur.NumeroCommande;
                this.commandeFournisseur.OrdreAffretement = 1;
                //Update new data
                this.commandeFournisseur.AdresseClient = newTransport.Parcours.AdresseDestination;
                this.commandeFournisseur.Transporteur = newTransport.Transporteur;
                this.commandeFournisseur.PrixTransportHT = newTransport.PUHT;
                this.commandeFournisseur.Km = newTransport.Parcours.Km;
                this.commandeFournisseur.CamionType = newTransport.CamionType;
                this.commandeFournisseur.DAffretement = moment();
                //Update form
                this.updateForm();
                //Get existing transporteur contactAdresse
                this.listService.getListContactAdresse(null, this.commandeFournisseur.Transporteur?.RefEntite
                  , null, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
                    this.transporteurContactAdresseList = result;
                    //Auto select
                    if (this.transporteurContactAdresseList.length === 1) {
                      this.commandeFournisseur.TransporteurContactAdresse = this.transporteurContactAdresseList[0];
                      this.transporteurContactAdresseListFC.setValue((this.commandeFournisseur.TransporteurContactAdresse ? this.commandeFournisseur.TransporteurContactAdresse.RefContactAdresse : null));
                      //Manage screen
                      this.manageScreen();
                    }
                  }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
                //Get existing AdresseClient and contactAdresse if applicable
                this.listService.getListAdresse(this.commandeFournisseur.AdresseClient?.RefAdresse
                  , this.commandeFournisseur.AdresseClient?.RefEntite
                  , ContactAdresseProcess.BonDeLivraison
                  , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
                  , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
                  , null, true)
                  .subscribe(result => {
                    this.clientAdresseList = result;
                    //Select right adresse
                    this.commandeFournisseur.AdresseClient = newTransport.Parcours.AdresseDestination;
                    this.clientAdresseListFC.setValue((this.commandeFournisseur.AdresseClient ? this.commandeFournisseur.AdresseClient.RefAdresse : null));
                    //Manage screen
                    this.manageScreen();
                    //Get Client
                    this.listService.getListClient(this.commandeFournisseur.AdresseClient?.RefEntite
                      , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
                      , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
                      , this.commandeFournisseur.Entite?.RefEntite
                      , this.commandeFournisseur.Entite?.RefEntite, true)
                      .subscribe(result => {
                        this.clientList = result;
                      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
                    //Get existing clientContactAdresse if AdresseClient is not null
                    if (this.commandeFournisseur.AdresseClient) {
                      this.listService.getListContactAdresse(null, this.commandeFournisseur.AdresseClient?.RefEntite, this.commandeFournisseur.AdresseClient?.RefAdresse
                        , ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
                          this.clientContactAdresseList = result;
                          //Auto select
                          if (this.clientContactAdresseList.length === 1) { this.clientContactAdresseListFC.setValue(this.clientContactAdresseList[0].RefContactAdresse); }
                        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
                    }
                    else {
                      this.clientContactAdresseList = [];
                    }
                  }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
              }
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          }
        }
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Accept warnings
  onAcceptAnomalie() {
    this.commandeFournisseur.DAnomalieOk = moment();
    this.commandeFournisseur.UtilisateurAnomalieOk = this.applicationUserContext.connectedUtilisateur;
    this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(575), duration: 4000 } as appInterfaces.SnackbarMsg);
  }
  //-----------------------------------------------------------------------------------
  //Remove carriage
  onRemoveCible() {
    let msg: string = this.applicationUserContext.getCulturedRessourceText(523);
    if (this.commandeFournisseur.DChargementPrevue || this.commandeFournisseur.DDechargementPrevue) {
      msg += " " + this.applicationUserContext.getCulturedRessourceText(727);
    }
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: msg },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.removeCible(null);
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(524), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
    });
  }
  removeCible(refClient: number) {
    //Update data
    this.saveData();
    //Delete old data
    this.commandeFournisseur.NumeroAffretement = this.commandeFournisseur.NumeroCommande;
    this.commandeFournisseur.DAffretement = null;
    this.commandeFournisseur.DChargement = null;
    this.commandeFournisseur.DChargementPrevue = null;
    this.commandeFournisseur.DDechargement = null;
    this.commandeFournisseur.DDechargementPrevue = null;
    this.commandeFournisseur.ValideDPrevues = false;
    this.commandeFournisseur.PoidsDechargement = 0;
    this.commandeFournisseur.NbBalleDechargement = 0;
    this.commandeFournisseur.TicketPeseeDechargement = false;
    this.commandeFournisseur.TicketPeseeChargement = false;
    this.commandeFournisseur.Transporteur = null;
    this.commandeFournisseur.TransporteurContactAdresse = null;
    this.transporteurContactAdresseList = [];
    this.commandeFournisseur.PrixTransportHT = 0;
    this.commandeFournisseur.PrixTransportSupplementHT = 0;
    this.commandeFournisseur.MotifAnomalieChargement = null;
    this.commandeFournisseur.DAnomalieChargement = null;
    this.commandeFournisseur.ChargementEffectue = false;
    this.commandeFournisseur.ChargementAnnule = false;
    this.commandeFournisseur.CmtChargementAnnule = "";
    this.commandeFournisseur.LotControle = false;
    this.commandeFournisseur.AdresseClient = null;
    this.clientAdresseList = [];
    this.clientAdresseListFC.setValue(null);
    this.clientList = [];
    this.clientListFC.setValue(null);
    this.clientContactAdresseList = [];
    this.clientContactAdresseListFC.setValue(null);
    this.commandeFournisseur.CmtClient = "";
    this.commandeFournisseur.CmtTransporteur = "";
    this.commandeFournisseur.NumeroAffretement = this.commandeFournisseur.NumeroCommande;
    this.commandeFournisseur.OrdreAffretement = 1;
    this.commandeFournisseur.Imprime = false;
    //Update form
    this.updateForm();
    //Set client if applicable
    if (refClient !== null) {
      //Set Client if possible
      //Get existing clientAdresse
      this.listService.getListAdresse(null, refClient
        , ContactAdresseProcess.BonDeLivraison
        , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
        , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
        , null, true)
        .subscribe(result => {
          this.clientAdresseList = result;
          if (result && result.length > 0 && this.clientAdresseList.find(x => x.RefEntite == refClient)) {
            this.clientListFC.setValue(refClient);
            this.manageScreen();
            //Auto select
            if (result.length === 1) {
              this.commandeFournisseur.AdresseClient = this.clientAdresseList[0];
              this.clientAdresseListFC.setValue(this.commandeFournisseur.AdresseClient.RefAdresse);
              //Get existing clientContactAdresse
              this.listService.getListContactAdresse(null, null, this.commandeFournisseur.AdresseClient?.RefAdresse, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
                this.clientContactAdresseList = result;
                //Auto select
                if (this.clientContactAdresseList.length === 1) { this.clientContactAdresseListFC.setValue(this.clientContactAdresseList[0].RefContactAdresse); }
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            }
            //Get existing Client if applicable
            this.listService.getListClient(refClient
              , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
              , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
              , this.commandeFournisseur.Entite?.RefEntite
              , this.commandeFournisseur.Entite?.RefEntite, true)
              .subscribe(result => {
                this.clientList = result;
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Get possible Client
      this.listService.getListClient(null
        , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
        , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
        , this.commandeFournisseur.Entite?.RefEntite
        , this.commandeFournisseur.Entite?.RefEntite, true)
        .subscribe(result => {
          this.clientList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    //Remove Mixtes
    if (this.commandeFournisseur.NumeroCommande > 0)
      //Check Mixte
      this.dataModelService.isMixte(this.commandeFournisseur.RefCommandeFournisseur)
        .subscribe((result: boolean) => {
          if (result === true) {
            //Mak Mixte
            this.dataModelService.unmarkMixte(this.commandeFournisseur.NumeroCommande)
              .subscribe((result: boolean) => {
                this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(536), duration: 4000 } as appInterfaces.SnackbarMsg);
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Cancel unloading
  onCancelUnloading() {
    //Set values
    this.saveData();
    //Check if operation is accepted
    this.dataModelService.hasRepartition(this.commandeFournisseur.RefCommandeFournisseur)
      .subscribe((result: boolean) => {
        if (!result) {
          //Accepted
          const dialogRef = this.dialog.open(ConfirmComponent, {
            width: "350px",
            data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(528) },
            autoFocus: false,
            restoreFocus: false
          });

          dialogRef.afterClosed().subscribe(result => {
            if (result === "yes") {
              this.cancelUnloading();
              this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(531), duration: 4000 } as appInterfaces.SnackbarMsg);
            }
          });
        }
        else {
          //Rejected
          const dialogRef = this.dialog.open(InformationComponent, {
            width: "350px",
            data: {
              title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(529)
            },
            restoreFocus: false
          });
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  cancelUnloading() {
    //Update data
    this.saveData();
    //Delete old data
    this.commandeFournisseur.DChargement = null;
    this.commandeFournisseur.DDechargement = null;
    this.commandeFournisseur.PoidsDechargement = 0;
    this.commandeFournisseur.NbBalleDechargement = 0;
    this.commandeFournisseur.PrixTonneHT = 0;
    //Update form
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Mixte
  onMixte() {
    let data: any;
    //Init
    this.saveData();
    data = { title: this.applicationUserContext.getCulturedRessourceText(533), type: "Mixte" };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "50%",
      data,
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        //Get linked CommandeFournisseur
        this.dataModelService.getCommandeFournisseur(result.ref, true).subscribe(result => {
          //Update data
          result.NumeroAffretement = this.commandeFournisseur.NumeroAffretement;
          result.AdresseClient = this.commandeFournisseur.AdresseClient;
          result.Transporteur = this.commandeFournisseur.Transporteur;
          result.Km = 0;
          result.DChargementPrevue = this.commandeFournisseur.DChargementPrevue;
          result.HoraireChargementPrevu = this.commandeFournisseur.HoraireChargementPrevu;
          result.DDechargementPrevue = this.commandeFournisseur.DDechargementPrevue;
          result.DMoisDechargementPrevu = this.commandeFournisseur.DMoisDechargementPrevu;
          result.HoraireDechargementPrevu = this.commandeFournisseur.HoraireDechargementPrevu;
          result.CamionType = this.commandeFournisseur.CamionType;
          result.CmtClient = this.commandeFournisseur.CmtClient;
          result.TransporteurContactAdresse = this.commandeFournisseur.TransporteurContactAdresse;
          result.OrdreAffretement = 0;    //Will ask for calculation of the order
          result.DAffretement = this.commandeFournisseur.DAffretement;
          result.CommandeFournisseurStatut = this.commandeFournisseur.CommandeFournisseurStatut;
          result.CamionComplet = this.commandeFournisseur.CamionComplet;
          //Format dates
          this.dataModelService.setTextFromMomentCommandeFournisseur(result);
          //Update linked CommandeFournisseur
          this.dataModelService.postCommandeFournisseur(this.commandeFournisseur, true)
            .subscribe(r => {
              this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
              //Manage screen
              this.manageScreen();
              //Check Mixte
              this.dataModelService.isMixte(this.commandeFournisseur.RefCommandeFournisseur)
                .subscribe((result: boolean) => {
                  this.commandeFournisseur.Mixte = result;
                }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Copy
  onCopy() {
    if (this.isFormValidForSave()) {
      const dialogRef = this.dialog.open(ConfirmComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(526) },
        autoFocus: false,
        restoreFocus: false
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result === "yes") {
          //Update data
          this.saveData();
          //Save to database
          if (this.commandeFournisseur.RefCommandeFournisseur > 0) {
            this.dataModelService.postCommandeFournisseur(this.commandeFournisseur, true).subscribe(result => {
              this.commandeFournisseur = result;
              this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
              //Copy
              this.copy();
              this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(527), duration: 4000 } as appInterfaces.SnackbarMsg);
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          }
        }
      });
    }
  }
  copy() {
    //Delete old data
    this.commandeFournisseur.RefCommandeFournisseur = 0;
    this.commandeFournisseur.NumeroCommande = null;
    this.commandeFournisseur.NumeroAffretement = null;
    this.commandeFournisseur.OrdreAffretement = 1;
    this.commandeFournisseur.D = null;
    this.commandeFournisseur.DMoisDechargementPrevu = null;
    //Update form
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Fournisseur selected
  onEntiteSelected(selectedRefEntite: number) {
    //Set values
    this.saveData();
    //Init
    this.blocContactAdresseVisible = false;
    this.blocChargementVisible = false;
    if (selectedRefEntite == null) {
      this.removeCible(null);
      this.commandeFournisseur.Entite = null;
    }
    //Adresse
    this.commandeFournisseur.Adresse = null;
    this.commandeFournisseur.Libelle = "";
    this.commandeFournisseur.Adr1 = "";
    this.commandeFournisseur.Adr2 = "";
    this.commandeFournisseur.CodePostal = "";
    this.commandeFournisseur.Ville = "";
    this.commandeFournisseur.Pays = null;
    //Horaires
    this.commandeFournisseur.Horaires = "";
    //Contact
    this.commandeFournisseur.ContactAdresse = null;
    this.commandeFournisseur.Civilite = null;
    this.commandeFournisseur.Prenom = "";
    this.commandeFournisseur.Nom = "";
    this.commandeFournisseur.Tel = "";
    this.commandeFournisseur.TelMobile = "";
    this.commandeFournisseur.Fax = "";
    this.commandeFournisseur.Email = "";
    //Produit
    this.commandeFournisseur.Produit = null;
    this.produitList = [];
    //Update form
    this.updateForm();
    //Set ContactAdresse and Adresse
    if (selectedRefEntite) {
      //Get full object from service
      this.dataModelService.getEntite(selectedRefEntite, null, null).subscribe(result => {
        if (result) {
          //if (result) {
          //Set entite
          this.commandeFournisseur.Entite = result;
          //Confirm existing produit or reset
          this.listService.getListProduit(this.commandeFournisseur.Produit?.RefProduit, this.commandeFournisseur.Entite.RefEntite
            , null, null, null, null, null, false, true).subscribe(result => {
              this.produitList = result;
              if (!(result && result.length > 0 && this.produitList.find(x => x.RefProduit == (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : null)))) {
                //Reset produit
                this.commandeFournisseur.Produit == null;
                this.produitListFC.setValue(null);
              }
              //Confirm existing Client, or reset Client
              this.listService.getListAdresse(null, null
                , ContactAdresseProcess.BonDeLivraison
                , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
                , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
                , null, true)
                .subscribe(result => {
                  this.clientAdresseList = result;
                  //Set Client if possible
                  if (!(result && result.length > 0 && this.clientAdresseList.find(x => x.RefAdresse == this.commandeFournisseur.AdresseClient.RefAdresse))) {
                    //Reset Client
                    this.clientList = [];
                    this.clientAdresseList = [];
                    this.clientContactAdresseList = [];
                    this.commandeFournisseur.AdresseClient = null;
                    this.clientListFC.setValue(null);
                    this.clientAdresseListFC.setValue(null);
                    this.clientContactAdresseListFC.setValue(null);
                    //Get possible Clients
                    this.listService.getListClient(null
                      , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
                      , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
                      , this.commandeFournisseur.Entite?.RefEntite
                      , this.commandeFournisseur.Entite?.RefEntite, true)
                      .subscribe(result => {
                        this.clientList = result;
                      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
                  }
                }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Set Adresse if applicable
          if (this.commandeFournisseur.Entite) {
            this.listService.getListAdresse(null, this.commandeFournisseur.Entite?.RefEntite
              , ContactAdresseProcess.BonDeLivraison, null, null, null, true)
              .subscribe(result => {
                this.adresseList = result;
                if (result && result.length == 1) {
                  //Select firts adresse
                  this.commandeFournisseur.Adresse = this.adresseList[0];
                  //Set Adresse values
                  this.commandeFournisseur.Libelle = this.commandeFournisseur.Adresse.Libelle;
                  this.commandeFournisseur.Adr1 = this.commandeFournisseur.Adresse.Adr1;
                  this.commandeFournisseur.Adr2 = this.commandeFournisseur.Adresse.Adr2;
                  this.commandeFournisseur.CodePostal = this.commandeFournisseur.Adresse.CodePostal;
                  this.commandeFournisseur.Ville = this.commandeFournisseur.Adresse.Ville;
                  this.commandeFournisseur.Pays = this.commandeFournisseur.Adresse.Pays;
                  this.commandeFournisseur.Horaires = this.commandeFournisseur.Adresse.Horaires;
                  //Update form
                  this.updateForm();
                  //Get existing contactAdresse
                  this.listService.getListContactAdresse(null, this.commandeFournisseur.Entite?.RefEntite, this.commandeFournisseur.Adresse?.RefAdresse, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
                    this.contactAdresseList = result;
                    //Auto select
                    if (this.contactAdresseList && this.contactAdresseList.length === 1
                      && (this.commandeFournisseur.ContactAdresse == null || (this.commandeFournisseur.ContactAdresse.RefContactAdresse !== this.contactAdresseList[0].RefContactAdresse))
                    ) {
                      let cA = this.contactAdresseList[0];
                      this.commandeFournisseur.ContactAdresse = cA;
                      //Set ContactAdresse data
                      this.commandeFournisseur.Civilite = cA.Contact.Civilite;
                      this.commandeFournisseur.Prenom = cA.Contact.Prenom;
                      this.commandeFournisseur.Nom = cA.Contact.Nom;
                      this.commandeFournisseur.Tel = cA.Tel;
                      this.commandeFournisseur.TelMobile = cA.TelMobile;
                      this.commandeFournisseur.Fax = cA.Fax;
                      this.commandeFournisseur.Email = cA.Email;
                      //Update form
                      this.updateForm();
                    }
                  }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
                }
                else {
                  //Update form
                  this.updateForm();
                }
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          }
          //  }
          //  else {
          //    this.adresseList = [];
          //    this.contactAdresseList = [];
          //    this.produitList = [];
          //    //Update form
          //    this.updateForm();
          //  }
        }
        else {
          this.removeCible(null);
          this.commandeFournisseur.Entite = null;
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      this.adresseList = [];
      this.contactAdresseList = [];
      this.produitList = [];
      //Update form
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //Adresse selected
  onAdresseSelected(selectedRefAdresse: number) {
    //Save data
    this.saveData();
    //Reset values
    //Adresse
    this.commandeFournisseur.Libelle = "";
    this.commandeFournisseur.Adr1 = "";
    this.commandeFournisseur.Adr2 = "";
    this.commandeFournisseur.CodePostal = "";
    this.commandeFournisseur.Ville = "";
    this.commandeFournisseur.Pays = null;
    //Horaires
    this.commandeFournisseur.Horaires = "";
    //Contact
    this.commandeFournisseur.ContactAdresse = null;
    this.commandeFournisseur.Civilite = null;
    this.commandeFournisseur.Prenom = "";
    this.commandeFournisseur.Nom = "";
    this.commandeFournisseur.Tel = "";
    this.commandeFournisseur.TelMobile = "";
    this.commandeFournisseur.Fax = "";
    this.commandeFournisseur.Email = "";
    //Set values
    if (selectedRefAdresse !== null) {
      this.commandeFournisseur.Adresse = this.adresseList.find(x => x.RefAdresse == selectedRefAdresse);
      //Set Adresse values
      this.commandeFournisseur.Libelle = this.commandeFournisseur.Adresse.Libelle;
      this.commandeFournisseur.Adr1 = this.commandeFournisseur.Adresse.Adr1;
      this.commandeFournisseur.Adr2 = this.commandeFournisseur.Adresse.Adr2;
      this.commandeFournisseur.CodePostal = this.commandeFournisseur.Adresse.CodePostal;
      this.commandeFournisseur.Ville = this.commandeFournisseur.Adresse.Ville;
      this.commandeFournisseur.Pays = this.commandeFournisseur.Adresse.Pays;
      this.commandeFournisseur.Horaires = this.commandeFournisseur.Adresse.Horaires;
      //Get existing contactAdresse
      this.listService.getListContactAdresse(this.commandeFournisseur.ContactAdresse?.RefContactAdresse, this.commandeFournisseur.Entite?.RefEntite, this.commandeFournisseur.Adresse?.RefAdresse, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
        this.contactAdresseList = result;
        //Auto select
        if (this.contactAdresseList.length === 1
          && (this.commandeFournisseur.ContactAdresse == null || (this.commandeFournisseur.ContactAdresse.RefContactAdresse === this.contactAdresseList[0].RefContactAdresse))
        ) {
          this.contactAdresseListFC.setValue(this.contactAdresseList[0].RefContactAdresse);
          let cA = this.contactAdresseList[0];
          this.commandeFournisseur.ContactAdresse = cA;
          //Set ContactAdresse data
          this.commandeFournisseur.Civilite = cA.Contact.Civilite;
          this.commandeFournisseur.Prenom = cA.Contact.Prenom;
          this.commandeFournisseur.Nom = cA.Contact.Nom;
          this.commandeFournisseur.Tel = cA.Tel;
          this.commandeFournisseur.TelMobile = cA.TelMobile;
          this.commandeFournisseur.Fax = cA.Fax;
          this.commandeFournisseur.Email = cA.Email;
          //Update form
          this.updateForm();
          //Update ContactAdresse
          this.onContactAdresseSelected(this.contactAdresseList[0].RefContactAdresse);
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      this.commandeFournisseur.Adresse = null;
      this.contactAdresseList = [];
    }
    //Update form
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Client selected
  onClientSelected(selectedRefEntite: number) {
    //Set values
    this.saveData();
    //Init
    this.removeCible(selectedRefEntite);
  }
  //-----------------------------------------------------------------------------------
  //ClientAdresse selected
  onClientAdresseSelected(selectedRefAdresse: number) {
    //Set values
    this.saveData();
    if (selectedRefAdresse !== null) {
      this.commandeFournisseur.AdresseClient = this.clientAdresseList.find(x => x.RefAdresse == selectedRefAdresse);
      //Get existing clientContactAdresse
      this.listService.getListContactAdresse(null, null, this.commandeFournisseur.AdresseClient?.RefAdresse, ContactAdresseProcess.BonDeLivraison, true).subscribe(result => {
        this.clientContactAdresseList = result;
        //Auto select
        if (this.clientContactAdresseList.length === 1) { this.clientContactAdresseListFC.setValue(this.clientContactAdresseList[0].RefContactAdresse); }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      this.commandeFournisseur.AdresseClient = null;
      this.clientContactAdresseList = [];
    }
  }
  //-----------------------------------------------------------------------------------
  //Produit selected
  onProduitSelected(selectedRefProduit: number) {
    if (selectedRefProduit == null) { this.produitListFC.setValue(null); }
    //Set values
    this.saveData();
    //Set produit
    if (selectedRefProduit) {
      //Get full object from service
      this.dataModelService.getProduit(selectedRefProduit).subscribe(result => {
        //Set produit
        this.commandeFournisseur.Produit = result;
        //Manage screen
        this.manageScreen();
        //Calculate NbBalle
        this.calculateNbBalle();
        //Confirm existing Client, or reset Client
        this.listService.getListAdresse(null, null
          , ContactAdresseProcess.BonDeLivraison
          , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
          , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
          , null, true)
          .subscribe(result => {
            this.clientAdresseList = result;
            //Set Client if possible
            if (!(result && result.length > 0 && this.clientAdresseList.find(x => x.RefAdresse == this.commandeFournisseur.AdresseClient.RefAdresse))) {
              //Reset Client
              this.clientList = [];
              this.clientAdresseList = [];
              this.clientContactAdresseList = [];
              this.commandeFournisseur.AdresseClient = null;
              this.clientListFC.setValue(null);
              this.clientAdresseListFC.setValue(null);
              this.clientContactAdresseListFC.setValue(null);
              //Get possible Clients
              this.listService.getListClient(null
                , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
                , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
                , this.commandeFournisseur.Entite?.RefEntite
                , this.commandeFournisseur.Entite?.RefEntite, true)
                .subscribe(result => {
                  this.clientList = result;
                }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Show product specific information if applicable
        this.showProduitCommentaires();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Manage screen
      this.manageScreen();
    }
  }
  //-----------------------------------------------------------------------------------
  //Show product specific comments
  showProduitCommentaires() {
    //Show product specific information if applicable
    if (this.applicationUserContext.connectedUtilisateur.UtilisateurType == UtilisateurType.CentreDeTri && this.commandeFournisseur.Produit?.CmtFournisseur) {
      let msg: string = encode(this.commandeFournisseur.Produit?.CmtFournisseur);
      if (!this.commandeFournisseur.ChargementEffectue && !this.commandeFournisseur.RefusCamion) {
        showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(1514), msg, this.applicationUserContext);
      }
    }
    if (this.applicationUserContext.connectedUtilisateur.UtilisateurType == UtilisateurType.Transporteur && this.commandeFournisseur.Produit?.CmtTransporteur) {
      let msg: string = encode(this.commandeFournisseur.Produit?.CmtTransporteur);
      if (!this.commandeFournisseur.ChargementEffectue && !this.commandeFournisseur.RefusCamion) {
        showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(1514), msg, this.applicationUserContext);
      }
    }
    if (this.applicationUserContext.connectedUtilisateur.UtilisateurType == UtilisateurType.Client && this.commandeFournisseur.Produit?.CmtClient) {
      let msg: string = encode(this.commandeFournisseur.Produit?.CmtClient);
      if (!this.commandeFournisseur.DDechargement) {
        showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(1514), msg, this.applicationUserContext);
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //ContactAdresse selected
  onContactAdresseSelected(selectedRefContactAdresse: number) {
    //Set values
    this.saveData();
    //RAZ
    this.commandeFournisseur.ContactAdresse = null;
    this.commandeFournisseur.Civilite = null;
    this.commandeFournisseur.Prenom = null;
    this.commandeFournisseur.Nom = null;
    this.commandeFournisseur.Email = null;
    this.commandeFournisseur.Tel = null;
    this.commandeFournisseur.TelMobile = null;
    this.commandeFournisseur.Fax = null;
    //Set ContactAdresse
    if (selectedRefContactAdresse) {
      let cA = this.contactAdresseList.find(x => x.RefContactAdresse === selectedRefContactAdresse);
      //Set ContactAdresse
      this.commandeFournisseur.ContactAdresse = cA;
      this.commandeFournisseur.Civilite = cA.Contact.Civilite;
      this.commandeFournisseur.Prenom = cA.Contact.Prenom;
      this.commandeFournisseur.Nom = cA.Contact.Nom;
      this.commandeFournisseur.Email = cA.Email;
      this.commandeFournisseur.Tel = cA.Tel;
      this.commandeFournisseur.TelMobile = cA.TelMobile;
      this.commandeFournisseur.Fax = cA.Fax;
    }
    //Update form
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //DMoisDechargementPrevuSelected selected
  onDMoisDechargementPrevuSelected(selectedNb: number) {
    //Stores original value
    let dOri: moment.Moment = this.commandeFournisseur.DMoisDechargementPrevu as moment.Moment;
    //Set values
    if (selectedNb == null) { this.dMoisDechargementPrevuFC.setValue(null); }
    this.saveData();
    //Manage screen
    this.manageScreen();
    if (this.commandeFournisseur.DMoisDechargementPrevu != null && this.commandeFournisseur.AdresseClient) {
      //Check CommandeClient, inform user if no exists
      this.dataModelService.getCommandeClient(0, this.commandeFournisseur.Entite?.RefEntite
        , (this.commandeFournisseur.AdresseClient?.RefAdresse ? this.commandeFournisseur.AdresseClient?.RefAdresse : 0)
        , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit?.RefProduit : 0)
        , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
        , this.commandeFournisseur.RefCommandeFournisseur
      )
        .subscribe(result => {
          if (result) {
            let cmd = result.CommandeClientMensuelles.find(i => moment(i.D).format("YYYY-MM") === moment(this.commandeFournisseur.DMoisDechargementPrevu).format("YYYY-MM"));
            if (cmd && cmd.Poids > 0) {
              //Get existing Client if applicable
              this.listService.getListClient(this.commandeFournisseur.AdresseClient?.RefEntite
                , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
                , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
                , this.commandeFournisseur.Entite?.RefEntite
                , this.commandeFournisseur.Entite?.RefEntite, true)
                .subscribe(result => {
                  this.clientList = result;
                }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
              //Check SurcoutCarburant
              this.checkSurcoutCarburant();
            }
            else {
              this.dMoisDechargementPrevuFC.setValue(moment(dOri).format("YYYY-MM-01T00:00:00"));
              this.commandeFournisseur.DMoisDechargementPrevu = dOri;
              //Information to user
              const dialogRef = this.dialog.open(InformationComponent, {
                width: "350px",
                data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(541) },
                restoreFocus: false
              });
            }
          }
          else {
            this.dMoisDechargementPrevuFC.setValue(moment(dOri).format("YYYY-MM-01T00:00:00"));
            this.commandeFournisseur.DMoisDechargementPrevu = dOri;
            if (selectedNb !== null) {
              //Information to user
              const dialogRef = this.dialog.open(InformationComponent, {
                width: "350px",
                data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(541) },
                restoreFocus: false
              });
            }
          }
        }, error => {
          if (error.status === 404) {
            this.dMoisDechargementPrevuFC.setValue(moment(dOri).format("YYYY-MM-01T00:00:00"));
            this.commandeFournisseur.DMoisDechargementPrevu = dOri;
            //Information to user
            const dialogRef = this.dialog.open(InformationComponent, {
              width: "350px",
              data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(541) },
              restoreFocus: false
            });
          }
        });
    }
    else {
      //Check SurcoutCarburant
      this.checkSurcoutCarburant();
    }
  }
  //-----------------------------------------------------------------------------------
  //ChargementEffectue
  onChargementEffectueChange() {
    //Set PoidsReparti if applicable
    if (this.chargementEffectueFC.value == true && this.commandeFournisseur.Produit.Collecte)  {
      this.poidsRepartiFC.setValue(this.commandeFournisseur.PoidsChargement);
    }
    //Set values
    this.saveData();
    //Manage screen
    this.manageScreen();
    //Message to user if applicable
    if (this.chargementEffectueFC.value == true) {
      const dialogRef = this.dialog.open(InformationComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(1430) },
        restoreFocus: false
      });
    }
  }
  //-----------------------------------------------------------------------------------
  //PoidsChargement
  onPoidsChargementChange() {
    //Set PoidsReparti if applicable
    if (this.commandeFournisseur.Produit.Collecte) {
      this.poidsRepartiFC.setValue(this.commandeFournisseur.PoidsChargement);
    }
  }
  //-----------------------------------------------------------------------------------
  //Camion complet selected
  onCamionCompletSelected(yesNo: boolean) {
    //Set values
    this.saveData();
    //Process
    if (yesNo && !this.commandeFournisseur.PoidsChargement && !this.commandeFournisseur.NbBalleChargement) {
      this.calculateNbBalle();
    }
    if (!yesNo) {
      this.motifCamionIncompletListFC.enable();
      this.motifCamionIncompletListFC.setValidators(Validators.required);
      this.motifCamionIncompletListFC.updateValueAndValidity();
    }
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //CommandeFournisseurStatut selected
  onCommandeFournisseurStatutSelected(refCommandeFournisseurStatut: number) {
    //Set values
    this.saveData();
    if (refCommandeFournisseurStatut == null) {
      this.commandeFournisseur.UtilisateurBlocage = null;
      this.commandeFournisseur.DBlocage = null;
      //Manage screen
      this.manageScreen();
    }
    else {
      //Get full object from service
      this.dataModelService.getCommandeFournisseurStatut(refCommandeFournisseurStatut).subscribe(result => {
        //Set statut
        this.commandeFournisseur.CommandeFournisseurStatut = result;
        if (refCommandeFournisseurStatut === CommandeFournisseurStatut.Bloquee) {
          this.commandeFournisseur.UtilisateurBlocage = this.applicationUserContext.connectedUtilisateur;
          this.commandeFournisseur.DBlocage = moment();
        }
        else {
          this.commandeFournisseur.UtilisateurBlocage = null;
          this.commandeFournisseur.DBlocage = null;
        }
        //Manage screen
        this.manageScreen();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //D change
  onDDateChange(d: moment.Moment) {
    //Set values
    if (d == null) {
      this.dFC.setValue(null);
    }
    this.saveData();
    //Manage screen
    this.manageScreen();
    //Process
    //Get existing month
    if (this.commandeFournisseur.D) {
      this.listService.getListdMoisDechargementPrevuMonth(this.commandeFournisseur.D as moment.Moment).subscribe(result => {
        this.dMoisDechargementPrevuList = result;
        //Set month
        this.dMoisDechargementPrevuFC.setValue(moment(this.commandeFournisseur.D).format("YYYY-MM-01T00:00:00"));
        this.commandeFournisseur.DMoisDechargementPrevu = (this.dMoisDechargementPrevuFC.value == null ? null : moment(this.dMoisDechargementPrevuFC.value));
        //Get existing Client if applicable
        this.listService.getListClient(this.commandeFournisseur.AdresseClient?.RefEntite
          , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
          , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
          , this.commandeFournisseur.Entite?.RefEntite
          , this.commandeFournisseur.Entite?.RefEntite, true)
          .subscribe(result => {
            this.clientList = result;
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      this.dMoisDechargementPrevuList = [];
      //Get existing Client if applicable
      this.listService.getListClient(this.commandeFournisseur.AdresseClient?.RefEntite
        , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
        , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
        , this.commandeFournisseur.Entite?.RefEntite
        , this.commandeFournisseur.Entite?.RefEntite, true)
        .subscribe(result => {
          this.clientList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //DDechargement change
  onDDechargementDateChange(d: moment.Moment) {
    //Stores original value
    let dOri: moment.Moment = this.commandeFournisseur.DDechargement as moment.Moment;
    //Set values
    if (d == null) {
      this.dDechargementFC.setValue(null);
      this.poidsRepartiFC.setValue(0);
    }
    else {
      this.poidsRepartiFC.setValue(this.commandeFournisseur.DDechargement == null && this.commandeFournisseur.Produit && this.commandeFournisseur.Produit.Collecte ? this.poidsChargementFC.value : this.poidsRepartiFC.value);
    }
    this.saveData();
    //Manage screen
    this.manageScreen();
    //Process if needed
    if (d == null) {
      this.prixTonneHTFC.setValue(null);
      this.commandeFournisseur.PrixTonneHT = null;
    }
    else {
      //Get CommandeClient, set price, inform user 
      this.dataModelService.getCommandeClient(0, this.commandeFournisseur.Entite?.RefEntite
        , (this.commandeFournisseur.AdresseClient.RefAdresse ? this.commandeFournisseur.AdresseClient.RefAdresse : 0)
        , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
        , (this.commandeFournisseur.DDechargement == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DDechargement) as moment.Moment
        , this.commandeFournisseur.RefCommandeFournisseur 
      )
        .subscribe(result => {
          if (result) {
            let cmd = result.CommandeClientMensuelles.find(i => moment(i.D).format("YYYY-MM") === moment(this.commandeFournisseur.DDechargement).format("YYYY-MM"));
            if (cmd && cmd.Poids > 0) {
              if (moment(this.commandeFournisseur.DMoisDechargementPrevu).format("YYYY-MM") === moment(cmd.D).format("YYYY-MM")) {
                //Modify data according to new CommandeClient
                this.commandeFournisseur.PrixTonneHT = cmd.PrixTonneHT;
                this.prixTonneHTFC.setValue(cmd.PrixTonneHT);
                //Sync DMoisDechargementPrevu
                if (this.commandeFournisseur.DMoisDechargementPrevu &&
                  moment(this.commandeFournisseur.DDechargement).format("YYYY-MM") !== moment(this.commandeFournisseur.DMoisDechargementPrevu).format("YYYY-MM")) {
                  this.listService.getListdMoisDechargementPrevuMonth(this.commandeFournisseur.DDechargement as moment.Moment).subscribe(result => {
                    this.dMoisDechargementPrevuList = result;
                    //Set month
                    this.dMoisDechargementPrevuFC.setValue(moment(this.commandeFournisseur.DDechargement).format("YYYY-MM-01T00:00:00"));
                    this.commandeFournisseur.DMoisDechargementPrevu = (this.dMoisDechargementPrevuFC.value == null ? null : moment(this.dMoisDechargementPrevuFC.value));
                    //Get existing Client if applicable
                    this.listService.getListClient(this.commandeFournisseur.AdresseClient?.RefEntite
                      , (this.commandeFournisseur.Produit ? this.commandeFournisseur.Produit.RefProduit : 0)
                      , (this.commandeFournisseur.DMoisDechargementPrevu == null ? new Date(1, 1, 1, 0, 0, 0) : this.commandeFournisseur.DMoisDechargementPrevu) as moment.Moment
                      , this.commandeFournisseur.Entite?.RefEntite
                      , this.commandeFournisseur.Entite?.RefEntite, true)
                      .subscribe(result => {
                        this.clientList = result;
                      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
                  }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
                  //Information to user
                  this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(542), duration: 6000 } as appInterfaces.SnackbarMsg);
                }
                else {
                  //Information to user
                  this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(543), duration: 6000 } as appInterfaces.SnackbarMsg);
                }
              }
              else {
                //Information to user
                this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(576), duration: 6000 } as appInterfaces.SnackbarMsg);
              }
            }
            else {
              this.dDechargementFC.setValue(dOri);
              this.commandeFournisseur.DDechargement = dOri;
              //Information to user
              const dialogRef = this.dialog.open(InformationComponent, {
                width: "350px",
                data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(541) },
                restoreFocus: false
              });
            }
          }
          else {
            this.dDechargementFC.setValue(dOri);
            this.commandeFournisseur.DDechargement = dOri;
            if (d !== null) {
              //Information to user
              const dialogRef = this.dialog.open(InformationComponent, {
                width: "350px",
                data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(541) },
                restoreFocus: false
              });
            }
          }
        }, error => {
          if (error.status === 404) {
            //Information to user
            const dialogRef = this.dialog.open(InformationComponent, {
              width: "350px",
              data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(541) },
              restoreFocus: false
            });
          }
        });
      //Check SurcoutCarburant
      this.checkSurcoutCarburant();
    }
  }
  //-----------------------------------------------------------------------------------
  //DChargementPrevue change
  onDChargementPrevueDateChange(d: moment.Moment) {
    //Set values
    if (d == null) {
      this.dChargementPrevueFC.setValue(null);
    }
    this.saveData();
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //DDechargementPrevue change
  onDDechargementPrevueDateChange(d: moment.Moment) {
    //Set values
    if (d == null) {
      this.dDechargementPrevueFC.setValue(null);
    }
    this.saveData();
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //ExportSAGE change
  onExportSAGEChange() {
    //Set values
    this.saveData();
    //Manage screen
    this.manageScreen();
    //Message to user
    if (this.exportSAGEFC.value === false) {
      const dialogRef = this.dialog.open(InformationComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(544) },
        restoreFocus: false
      });
    }
  }
  //-----------------------------------------------------------------------------------
  //ValideDPrevues change
  onValideDPrevuesChange() {
    //Set values
    this.saveData();
    //Check similar
    this.checkSimilar();
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Check similar CommandeFournisseur change
  checkSimilar() {
    //On validation, inform user if same elements without dates
    if (this.valideDPrevuesFC.value) {
      this.dataModelService.isSimilarToCommandeFournisseur(this.commandeFournisseur.RefCommandeFournisseur, moment(this.commandeFournisseur.D))
        .subscribe((result: any[]) => {
          if (result && result.length > 0) {
            let msg: string;
            msg = this.applicationUserContext.getCulturedRessourceText(1072);
            msg += "<br/>";
            result.forEach(x =>
              msg += this.getFormattedNumeroCommande(x["NumeroCommande"])
              + " - " + x["Libelle"]
              + " (" + moment(x["D"]).format("L") + ")<br/>");
            const dialogRef = this.dialog.open(InformationComponent, {
              width: "350px",
              data: {
                title: this.applicationUserContext.getCulturedRessourceText(337), message: "", htmlMessage: msg
              },
              restoreFocus: false
            });
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Show FicheControle
  onFicheControle() {
    if (this.isFormValidForSave()) {
      //Update data
      this.saveData();
      //Update if existing model
      if (this.commandeFournisseur.RefCommandeFournisseur > 0) {
        this.dataModelService.postCommandeFournisseur(this.commandeFournisseur, true).subscribe(result => {
          this.commandeFournisseur = result;
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
          //Save menu
          if (this.applicationUserContext.fromMenu == null) {
            this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
          }
          //Open Repartition
          this.router.navigate(["fiche-controle", "0", {
            refCommandeFournisseur: this.commandeFournisseur.RefCommandeFournisseur
          }])
          this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
          this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuControle);
          this.eventEmitterService.onChangeMenu();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    }
    else {
      this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(589), duration: 4000 } as appInterfaces.SnackbarMsg);
    }
  }
  //-----------------------------------------------------------------------------------
  //Show NonConformite
  onNonConformite() {
    if (this.isFormValidForSave()) {
      //Update data
      this.saveData();
      //Update if existing model
      if (this.commandeFournisseur.RefCommandeFournisseur > 0) {
        this.dataModelService.postCommandeFournisseur(this.commandeFournisseur, true).subscribe(result => {
          this.commandeFournisseur = result;
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
          //Save menu
          if (this.applicationUserContext.fromMenu == null) {
            this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
          }
          //Open Repartition
          this.router.navigate(["non-conformite", "0", {
            refCommandeFournisseur: this.commandeFournisseur.RefCommandeFournisseur
          }])
          this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
          this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
          this.eventEmitterService.onChangeMenu();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    }
    else {
      this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(589), duration: 4000 } as appInterfaces.SnackbarMsg);
    }
  }
  //-----------------------------------------------------------------------------------
  //Show repartition
  onRepartition() {
    if (!(this.locked && this.saveLocked)) {
      //Update data
      this.saveData();
      //Check if form is valid for save
      if (!this.isFormValidForSave() && this.form.touched) {
        //Update if existing model
        const dialogRef = this.dialog.open(ConfirmComponent, {
          width: "350px",
          data: { title: this.applicationUserContext.getCulturedRessourceText(1570), message: this.applicationUserContext.getCulturedRessourceText(1571) },
          autoFocus: false,
          restoreFocus: false
        });
        //After closed
        dialogRef.afterClosed().subscribe(result => {
          if (result === "yes") {
            this.backToRepartition();
          }
        });
      }
      else {
        //Save repartition if valide or untouched
        if (!this.form.touched) {
          this.backToRepartition();
        }
        else {
          this.postCommandeFournisseur("gotoRepartition");
        }
      }
    }
    else {
      this.backToRepartition();
    }
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  postCommandeFournisseur(action: string) {
    //Process
    this.dataModelService.postCommandeFournisseur(this.commandeFournisseur, true)
      .subscribe(result => {
        this.commandeFournisseur = result;
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Next action
        switch (action) {
          case "save":
            //Set original Menu
            setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
            //Route back to grid
            this.router.navigate(["grid"]);
            break;
          case "gotoRepartition":
            this.backToRepartition()
            break;
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Back to Repartition
  backToRepartition() {
    //Save menu
    if (this.applicationUserContext.fromMenu == null) {
      this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
    }
    //Set original Menu
    setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
    //Route
    this.router.navigate(["repartition", "0", {
      refCommandeFournisseur: this.commandeFournisseur.RefCommandeFournisseur
    }])
  }
  //------------------------------------------------------------------------------------------------------------------------------------------
  //Calculation of NbBalle and PoidsChargement
  calculateNbBalle() {
    //Set values
    this.saveData();
    //Process
    if (this.commandeFournisseur.Produit && this.commandeFournisseur.Entite && this.commandeFournisseur.CamionComplet === true
      && (!this.commandeFournisseur.PoidsChargement || this.commandeFournisseur.PoidsChargement === 0)
      && (!this.commandeFournisseur.PoidsReparti || this.commandeFournisseur.PoidsReparti === 0)
      && (!this.commandeFournisseur.NbBalleChargement || this.commandeFournisseur.NbBalleChargement === 0)
    ) {
      this.dataModelService.getEstimatedNbBalle(this.commandeFournisseur.Produit.RefProduit, this.commandeFournisseur.Entite.RefEntite)
        .subscribe((result: any) => {
          this.commandeFournisseur.PoidsChargement = result[0].PoidsDechargement;
          this.commandeFournisseur.PoidsReparti = (this.commandeFournisseur.Produit.Collecte === true ? result[0].PoidsDechargement : 0);
          this.commandeFournisseur.NbBalleChargement = result[0].NbBalleDechargement;
          //Update form
          this.poidsChargementFC.setValue(this.commandeFournisseur.PoidsChargement === 0 ? null : this.commandeFournisseur.PoidsChargement);
          this.poidsRepartiFC.setValue(this.commandeFournisseur.PoidsReparti === 0 ? null : this.commandeFournisseur.PoidsReparti);
          this.nbBalleChargementFC.setValue(this.commandeFournisseur.NbBalleChargement === 0 ? null : this.commandeFournisseur.NbBalleChargement);
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    //  else {
    //    this.commandeFournisseur.PoidsChargement = 0;
    //    this.commandeFournisseur.PoidsReparti = 0;
    //    this.commandeFournisseur.NbBalleChargement = 0;
    //    //Update form
    //    this.poidsChargementFC.setValue(this.commandeFournisseur.PoidsChargement === 0 ? null : this.commandeFournisseur.PoidsChargement);
    //    this.poidsRepartiFC.setValue(this.commandeFournisseur.PoidsReparti === 0 ? null : this.commandeFournisseur.PoidsReparti);
    //    this.nbBalleChargementFC.setValue(this.commandeFournisseur.NbBalleChargement === 0 ? null : this.commandeFournisseur.NbBalleChargement);
    //  }
  }
  //------------------------------------------------------------------------------------------------------------------------------------------
  //Get formatted blocage details text
  getBlocageDetails(): string {
    let r: string = "";
    if (this.commandeFournisseur.UtilisateurBlocage && this.commandeFournisseur.DBlocage !== null) {
      r = this.applicationUserContext.getCulturedRessourceText(497) + " " + this.commandeFournisseur.UtilisateurBlocage.Nom + " - " + moment(this.commandeFournisseur.DBlocage).format("DD/MM/YYYY");
    }
    return r;
  }
  //------------------------------------------------------------------------------------------------------------------------------------------
  //Get formatted NumeroCommande
  getFormattedNumeroCommande(s: number): string {
    let r: string = "";
    if (s) {
      r = s.toString();
      if (r.length === 10) {
        r = r.substring(0, 4) + " " + r.substring(4, 6) + " " + r.substring(6, 10);
      }
    }
    return r;
  }
  //------------------------------------------------------------------------------------------------------------------------------------------
  //Get EnvCommandeFournisseurStatut
  getEnvCommandeFournisseurStatut(): string {
    let r: string = "";
    //Process
    if ((this.commandeFournisseur.DDechargement && moment(this.commandeFournisseur.DDechargement).year() < 2012) || this.reparti) {
      r = this.applicationUserContext.getCulturedRessourceText(545);
    }
    else if (this.commandeFournisseur.DDechargement !== null) {
      r = this.applicationUserContext.getCulturedRessourceText(546);
    }
    else if (this.commandeFournisseur.RefusCamion === true) {
      r = this.applicationUserContext.getCulturedRessourceText(551);
    }
    else if (this.commandeFournisseur.CommandeFournisseurStatut !== null) {
      if (this.commandeFournisseur.CommandeFournisseurStatut.RefCommandeFournisseurStatut === 1) {
        r = this.applicationUserContext.getCulturedRessourceText(547);
      }
      else if (this.commandeFournisseur.CommandeFournisseurStatut.RefCommandeFournisseurStatut === 2) {
        r = this.applicationUserContext.getCulturedRessourceText(548);
      }
      else {
        r = this.applicationUserContext.getCulturedRessourceText(549);
      }
    }
    else {
      r = this.applicationUserContext.getCulturedRessourceText(550);
    }
    //End
    return r;
  }
  //------------------------------------------------------------------------------------------------------------------------------------------
  //Get Prestataire info
  getPrestataireInfo(): string {
    let r: string = this.applicationUserContext.getCulturedRessourceText(1251) + " - " + this.commandeFournisseur.Prestataire.Libelle;
    //RefExt
    if (this.commandeFournisseur.RefExt) {
      r += " (" + this.commandeFournisseur.RefExt + ")";
    }
    else if (this.commandeFournisseur.DDechargement !== null) {
      r = this.applicationUserContext.getCulturedRessourceText(546);
    }
    else if (this.commandeFournisseur.RefusCamion === true) {
      r = this.applicationUserContext.getCulturedRessourceText(551);
    }
    else if (this.commandeFournisseur.CommandeFournisseurStatut !== null) {
      if (this.commandeFournisseur.CommandeFournisseurStatut.RefCommandeFournisseurStatut === 1) {
        r = this.applicationUserContext.getCulturedRessourceText(547);
      }
      else if (this.commandeFournisseur.CommandeFournisseurStatut.RefCommandeFournisseurStatut === 2) {
        r = this.applicationUserContext.getCulturedRessourceText(548);
      }
      else {
        r = this.applicationUserContext.getCulturedRessourceText(549);
      }
    }
    else {
      r = this.applicationUserContext.getCulturedRessourceText(550);
    }
    //End
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Show image gallery
  showPictures(elementType: string, elementId: number, pictureId: number) {
    let data: any;
    //Init
    this.saveData();
    data = { title: this.applicationUserContext.getCulturedRessourceText(623), type: "ImageGallery", elementType: elementType, ref: elementId.toString(), pictureId: pictureId };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "650px",
      data,
      autoFocus: false,
      restoreFocus: false
    });
  }
  //-----------------------------------------------------------------------------------
  //Delete a CommandeFournisseurFichier
  onDeleteCommandeFournisseurFichier(i: number) {
    if (i !== null) {
      //Save data
      this.saveData();
      //Process
      this.commandeFournisseur.CommandeFournisseurFichiers.splice(i, 1);
      //Update form
      this.updateForm();
    }
  }
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  onUploadFile() {
    //Save data
    this.saveData();
    //open pop-up
    const dialogRef = this.dialog.open(UploadComponent, {
      width: "50%", height: "50%",
      data: {
        title: this.applicationUserContext.getCulturedRessourceText(626), message: this.applicationUserContext.getCulturedRessourceText(627)
        , multiple: true, type: "commandefournisseur", fileType: 0, ref: this.commandeFournisseur.RefCommandeFournisseur
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
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(342), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
      else {
        //Reload data
        var url = this.baseUrl + "evapi/commandefournisseur/" + this.commandeFournisseur.RefCommandeFournisseur;
        this.dataModelService.getCommandeFournisseur(this.commandeFournisseur.RefCommandeFournisseur, true).subscribe(result => {
          //Get data
          this.commandeFournisseur.CommandeFournisseurFichiers = result.CommandeFournisseurFichiers;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(628), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Download a file
  getFile(fileType: string, RefCommandeFournisseurFichier: number) {
    this.downloadService.download(fileType, RefCommandeFournisseurFichier.toString(), "", this.commandeFournisseur.RefCommandeFournisseur, "")
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
    //If CDT or client, no letter
    let courrierCommandeAffretement: boolean = true;
    if (this.applicationUserContext.connectedUtilisateur.CentreDeTri != null || this.applicationUserContext.connectedUtilisateur.Client != null) { courrierCommandeAffretement = false; }
    this.downloadService.download("CommandeAffretement", this.commandeFournisseur.RefCommandeFournisseur.toString(), "xlsx", null, courrierCommandeAffretement ? "courrierCommandeAffretement" : "")
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Create e-mail
  createEmail(emailType: string) {
    let cmdFs: number[] = [this.commandeFournisseur.RefCommandeFournisseur];
    //For each CommandeFournisseur
    for (let cmdF of cmdFs) {
      this.dataModelService.createEmailCommandeFournisseur(cmdF, emailType).subscribe(result => {
        let data: any;
        data = { refEmail: result };
        //Open dialog
        const dialogRef = this.dialog.open(EmailComponent, {
          width: "950px",
          data,
          disableClose: true,
          closeOnNavigation: false,
          autoFocus: false,
          restoreFocus: false
        });
      }), error => showErrorToUser(this.dialog, error, this.applicationUserContext);
    }
  }
  //-----------------------------------------------------------------------------------
  //Get SurcoutCarburant
  checkSurcoutCarburant() {
    if ((this.commandeFournisseur.DDechargement != null || this.commandeFournisseur.DMoisDechargementPrevu != null)
      && this.commandeFournisseur.Transporteur?.RefEntite
      && this.commandeFournisseur.AdresseClient?.Pays?.RefPays) {
      this.dataModelService.getSurcoutCarburantRatio(moment(this.commandeFournisseur.DDechargement == null ? this.commandeFournisseur.DMoisDechargementPrevu : this.commandeFournisseur.DDechargement)
        , this.commandeFournisseur.Transporteur?.RefEntite
        , this.commandeFournisseur.AdresseClient?.Pays?.RefPays
      )
        .subscribe((result: number) => {
          if ((result / 100 * this.commandeFournisseur.PrixTransportHT).toFixed(2) !== this.commandeFournisseur.SurcoutCarburantHT.toFixed(2)) {
            const dialogRef = this.dialog.open(InformationComponent, {
              width: "350px",
              data: {
                title: this.applicationUserContext.getCulturedRessourceText(337), message: this.applicationUserContext.getCulturedRessourceText(719) + " " + (result / 100 * this.commandeFournisseur.PrixTransportHT).toFixed(2).toString() + this.applicationUserContext.getCulturedRessourceText(233)
              },
              restoreFocus: false
            });
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Apply filter
  protected filterCentreDeTrisMulti() {
    if (!this.centreDeTriList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("CentreDeTriListFilter").value;
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
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationTooltipText() {
    return `${this.commandeFournisseur?.CreationText}
${this.commandeFournisseur?.ModificationText}
${this.commandeFournisseur?.AnomalieOkText}`;
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getLockByTooltipText() {
    return `${this.applicationUserContext.getCulturedRessourceText(632)}
${this.commandeFournisseur?.LockedBy?.Nom}`;
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for ChargementAnnule
  getChargementAnnuleTooltipText(): string {
    if (this.commandeFournisseur.UtilisateurChargementAnnule) {
      return `${this.commandeFournisseur.UtilisateurChargementAnnule.Nom}
${moment(this.commandeFournisseur.DChargementAnnule).format("L") + " " + moment(this.commandeFournisseur.DChargementAnnule).format("LT")}`;
    }
    else {
      return ``;
    }
  }
}
