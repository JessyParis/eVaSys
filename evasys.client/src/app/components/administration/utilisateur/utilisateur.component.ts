import { Component, Inject, OnInit, OnDestroy } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ReplaySubject, Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { UtilsService } from "../../../services/utils.service";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { getCreationModificationTooltipText, showErrorToUser, cmp, GetUtilisateurTypeLocalizedLabel, removeAccents } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { ListService } from "../../../services/list.service";
import { HabilitationAnnuaire, HabilitationLogistique, HabilitationModuleCentreDeTri, HabilitationModuleCollectivite, HabilitationModulePrestataire, HabilitationQualite, UtilisateurType } from "../../../globals/enums";
import { EnvComponent } from "../../../classes/appClasses";
import { kendoFontData } from "../../../globals/kendo-utils";
import { UtilisateurList } from "../../../interfaces/dataModelsInterfaces";
import { environment } from "../../../../environments/environment";
import { DomSanitizer } from "@angular/platform-browser";
import { BaseFormComponent } from "../../_ancestors/base-form.component";

@Component({
    selector: "utilisateur",
    templateUrl: "./utilisateur.component.html",
    standalone: false
})

export class UtilisateurComponent extends BaseFormComponent<dataModelsInterfaces.Utilisateur> {
  //Data
  habilitationAdministrationList: string[] = [];
  habilitationAnnuaireList: string[] = [];
  habilitationLogistiqueList: string[] = [];
  habilitationMessagerieList: string[] = [];
  habilitationModuleCentreDeTriList: string[] = [];
  habilitationModuleCollectiviteList: string[] = [];
  habilitationQualiteList: string[] = [];
  habilitationModulePrestataireList: string[] = [];
  contactAdresseList: dataModelsInterfaces.ContactAdresse[] = [];
  utilisateurMaitreList: dataModelsInterfaces.UtilisateurList[] = [];
  originalActif: boolean = false;

  //Form
  form: UntypedFormGroup;
  utilisateurTypeForm: UntypedFormGroup;
  utilisateur: dataModelsInterfaces.Utilisateur = { Actif: true } as dataModelsInterfaces.Utilisateur;
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  nomFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  loginFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  pwdFC: UntypedFormControl = new UntypedFormControl(null, [Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z0-9$@$!%*?&].{11,}')]);
  eMailFC: UntypedFormControl = new UntypedFormControl(null, [Validators.email]);
  telFC: UntypedFormControl = new UntypedFormControl(null);
  telMobileFC: UntypedFormControl = new UntypedFormControl(null);
  faxFC: UntypedFormControl = new UntypedFormControl(null);
  habilitationAdministrationFC: UntypedFormControl = new UntypedFormControl(null);
  habilitationAnnuaireFC: UntypedFormControl = new UntypedFormControl(null);
  habilitationLogistiqueFC: UntypedFormControl = new UntypedFormControl(null);
  transporteurLibelleFC: UntypedFormControl = new UntypedFormControl(null);
  habilitationMessagerieFC: UntypedFormControl = new UntypedFormControl(null);
  habilitationModuleCentreDeTriFC: UntypedFormControl = new UntypedFormControl(null);
  centreDeTriLibelleFC: UntypedFormControl = new UntypedFormControl(null);
  habilitationModuleCollectiviteFC: UntypedFormControl = new UntypedFormControl(null);
  collectiviteLibelleFC: UntypedFormControl = new UntypedFormControl(null);
  habilitationQualiteFC: UntypedFormControl = new UntypedFormControl(null);
  clientLibelleFC: UntypedFormControl = new UntypedFormControl(null);
  habilitationModulePrestataireFC: UntypedFormControl = new UntypedFormControl(null);
  prestataireLibelleFC: UntypedFormControl = new UntypedFormControl(null);
  piedEmailHTMLFC: UntypedFormControl = new UntypedFormControl(null);
  contactAdresseListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  utilisateurTypeFC: UntypedFormControl = new UntypedFormControl(null);
  utilisateurMaitreLibelleFC: UntypedFormControl = new UntypedFormControl(null);
  //Misc
  filteredUtilisateurMaitreList: ReplaySubject<dataModelsInterfaces.UtilisateurList[]> = new ReplaySubject<dataModelsInterfaces.UtilisateurList[]>(1);
  public kendoFontData = kendoFontData;
  profils: dataModelsInterfaces.UtilisateurList[] = [];
  // Subject that emits when the component has been destroyed.
  protected _onDestroy = new Subject<void>();
  //Constructor
  constructor(protected activatedRoute: ActivatedRoute
    , private http: HttpClient, @Inject("BASE_URL") private baseUrl: string
    , protected router: Router
    , private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected utilsService: UtilsService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
    , protected sanitizer: DomSanitizer) {
    super("UtilisateurComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: this.actifFC,
      Nom: this.nomFC,
      Login: this.loginFC,
      Pwd: this.pwdFC,
      EMail: this.eMailFC,
      Tel: this.telFC,
      TelMobile: this.telMobileFC,
      Fax: this.faxFC,
      HabilitationAdministration: this.habilitationAdministrationFC,
      HabilitationAnnuaire: this.habilitationAnnuaireFC,
      HabilitationLogistique: this.habilitationLogistiqueFC,
      TransporteurLibelle: this.transporteurLibelleFC,
      HabilitationMessagerie: this.habilitationMessagerieFC,
      HabilitationModuleCentreDeTri: this.habilitationModuleCentreDeTriFC,
      CentreDeTriLibelle: this.centreDeTriLibelleFC,
      HabilitationModuleCollectivite: this.habilitationModuleCollectiviteFC,
      CollectiviteLibelle: this.collectiviteLibelleFC,
      HabilitationQualite: this.habilitationQualiteFC,
      ClientLibelle: this.clientLibelleFC,
      HabilitationModulePrestataire: this.habilitationModulePrestataireFC,
      PrestataireLibelle: this.prestataireLibelleFC,
      PiedEmailHTML: this.piedEmailHTMLFC,
      ContactAdresseList: this.contactAdresseListFC,
      UtilisateurMaitreLibelle: this.utilisateurMaitreLibelleFC
    });
    this.utilisateurTypeForm = this.fb.group({
      UtilisateurType: this.utilisateurTypeFC
    });
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Global lock
    if (1 !== 1) {
      this.lockScreen();
    }
    else {
      //Init
      this.unlockScreen();
      if (!this.eMailFC.value) { this.loginFC.enable(); } else { this.loginFC.disable(); }
      if (this.utilisateur.HabilitationQualite == HabilitationQualite.Client
        || this.utilisateur.HabilitationModuleCollectivite == HabilitationModuleCollectivite.Utilisateur
        || this.utilisateur.HabilitationLogistique == HabilitationLogistique.Transporteur
        || this.utilisateur.HabilitationLogistique == HabilitationLogistique.CentreDeTri
        || this.utilisateur.HabilitationModuleCentreDeTri == HabilitationModuleCentreDeTri.Utilisateur
        || (this.utilisateur.HabilitationModulePrestataire && (this.utilisateur.HabilitationModulePrestataire != HabilitationModulePrestataire.Administrateur))
      ) {
        this.contactAdresseListFC.enable();
      }
      else { this.contactAdresseListFC.disable(); }
      //Enable all
      this.habilitationAdministrationFC.enable();
      this.habilitationAnnuaireFC.enable();
      this.habilitationLogistiqueFC.enable();
      this.habilitationMessagerieFC.enable();
      this.habilitationQualiteFC.enable();
      this.habilitationModuleCollectiviteFC.enable();
      this.habilitationModuleCentreDeTriFC.enable();
      this.habilitationModulePrestataireFC.enable();
      //Selective disable
      switch (this.utilisateurTypeFC.value) {
        case UtilisateurType.Valorplast.toString():
          this.habilitationModuleCentreDeTriFC.disable();
          this.habilitationModulePrestataireFC.disable();
          break;
        case UtilisateurType.Collectivite.toString():
          this.habilitationAdministrationFC.disable();
          this.habilitationAnnuaireFC.disable();
          this.habilitationLogistiqueFC.disable();
          this.habilitationMessagerieFC.disable();
          this.habilitationQualiteFC.disable();
          this.habilitationModuleCollectiviteFC.disable();
          this.habilitationModuleCentreDeTriFC.disable();
          this.habilitationModulePrestataireFC.disable();
          break;
        case UtilisateurType.CentreDeTri.toString():
          this.habilitationAdministrationFC.disable();
          this.habilitationAnnuaireFC.disable();
          this.habilitationLogistiqueFC.disable();
          this.habilitationMessagerieFC.disable();
          this.habilitationQualiteFC.disable();
          this.habilitationModuleCollectiviteFC.disable();
          this.habilitationModuleCentreDeTriFC.disable();
          this.habilitationModulePrestataireFC.disable();
          break;
        case UtilisateurType.Transporteur.toString():
          this.habilitationAdministrationFC.disable();
          this.habilitationAnnuaireFC.disable();
          this.habilitationLogistiqueFC.disable();
          this.habilitationMessagerieFC.disable();
          this.habilitationQualiteFC.disable();
          this.habilitationModuleCollectiviteFC.disable();
          this.habilitationModuleCentreDeTriFC.disable();
          this.habilitationModulePrestataireFC.disable();
          break;
        case UtilisateurType.Client.toString():
          this.habilitationAdministrationFC.disable();
          this.habilitationAnnuaireFC.disable();
          this.habilitationLogistiqueFC.disable();
          this.habilitationMessagerieFC.disable();
          this.habilitationQualiteFC.disable();
          this.habilitationModuleCollectiviteFC.disable();
          this.habilitationModuleCentreDeTriFC.disable();
          this.habilitationModulePrestataireFC.disable();
          break;
        case UtilisateurType.Prestataire.toString():
          this.habilitationAdministrationFC.disable();
          this.habilitationAnnuaireFC.disable();
          this.habilitationLogistiqueFC.disable();
          this.habilitationMessagerieFC.disable();
          this.habilitationQualiteFC.disable();
          this.habilitationModuleCollectiviteFC.disable();
          this.habilitationModuleCentreDeTriFC.disable();
          this.habilitationModulePrestataireFC.disable();
          break;
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    //Get Habilitations
    this.utilsService.getHabilitationValues("HabilitationAdministration").subscribe(result => {
      this.habilitationAdministrationList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.utilsService.getHabilitationValues("HabilitationAnnuaire").subscribe(result => {
      this.habilitationAnnuaireList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.utilsService.getHabilitationValues("HabilitationLogistique").subscribe(result => {
      this.habilitationLogistiqueList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.utilsService.getHabilitationValues("HabilitationMessagerie").subscribe(result => {
      this.habilitationMessagerieList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.utilsService.getHabilitationValues("HabilitationModuleCentreDeTri").subscribe(result => {
      this.habilitationModuleCentreDeTriList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.utilsService.getHabilitationValues("HabilitationModuleCollectivite").subscribe(result => {
      this.habilitationModuleCollectiviteList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.utilsService.getHabilitationValues("HabilitationQualite").subscribe(result => {
      this.habilitationQualiteList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.utilsService.getHabilitationValues("HabilitationModulePrestataire").subscribe(result => {
      this.habilitationModulePrestataireList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getDataModel<dataModelsInterfaces.Utilisateur>(id, this.componentName).subscribe(result => {
          //Get data
          this.utilisateur = result;
          this.sourceObj = result;
          this.originalActif = this.utilisateur.Actif;
          this.fillContactAdresse();
          //Get available Maitres
          this.listService.getListUtilisateur(this.utilisateur.RefUtilisateur, null, null, null, false, true, false, true).subscribe(result => {
            this.utilisateurMaitreList = result;
            this.filteredUtilisateurMaitreList.next(this.utilisateurMaitreList.slice());
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Get all available profiles
          this.listService.getListUtilisateur(this.utilisateur.RefUtilisateur, "", "", null, true, false, false, true).subscribe(result => {
            this.profils = result.filter(e => e.RefUtilisateur != this.utilisateur.RefUtilisateur);
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
      else {
        //Get available Maitres
        this.listService.getListUtilisateur(null, null, null, null, false, true, false, true).subscribe(result => {
          this.utilisateurMaitreList = result;
          this.filteredUtilisateurMaitreList.next(this.utilisateurMaitreList.slice());
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Update form
        this.updateForm();
      }
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.actifFC.setValue(this.utilisateur.Actif);
    this.nomFC.setValue(this.utilisateur.Nom);
    this.loginFC.setValue(this.utilisateur.Login);
    this.pwdFC.setValue(this.utilisateur.Pwd);
    this.eMailFC.setValue(this.utilisateur.EMail);
    this.telFC.setValue(this.utilisateur.Tel);
    this.telMobileFC.setValue(this.utilisateur.TelMobile);
    this.faxFC.setValue(this.utilisateur.Fax);
    this.habilitationAdministrationFC.setValue(this.utilisateur.HabilitationAdministration);
    this.habilitationAnnuaireFC.setValue(this.utilisateur.HabilitationAnnuaire);
    this.habilitationLogistiqueFC.setValue(this.utilisateur.HabilitationLogistique);
    this.transporteurLibelleFC.setValue(this.utilisateur?.Transporteur?.Libelle);
    this.habilitationMessagerieFC.setValue(this.utilisateur.HabilitationMessagerie);
    this.habilitationModuleCentreDeTriFC.setValue(this.utilisateur.HabilitationModuleCentreDeTri);
    this.centreDeTriLibelleFC.setValue(this.utilisateur?.CentreDeTri?.Libelle);
    this.habilitationModuleCollectiviteFC.setValue(this.utilisateur.HabilitationModuleCollectivite);
    this.collectiviteLibelleFC.setValue(this.utilisateur?.Collectivite?.Libelle);
    this.habilitationQualiteFC.setValue(this.utilisateur.HabilitationQualite);
    this.clientLibelleFC.setValue(this.utilisateur?.Client?.Libelle);
    this.habilitationModulePrestataireFC.setValue(this.utilisateur.HabilitationModulePrestataire);
    this.prestataireLibelleFC.setValue(this.utilisateur?.Prestataire?.Libelle);
    this.piedEmailHTMLFC.setValue(this.utilisateur.PiedEmail);
    this.contactAdresseListFC.setValue((this.utilisateur.ContactAdresse ? this.utilisateur.ContactAdresse.RefContactAdresse : null));
    let utilisateurMaitreLibelle: string = "";
    if (this.utilisateur.UtilisateurMaitre) {
      utilisateurMaitreLibelle = this.utilisateur.UtilisateurMaitre?.Nom
        + "\n" + this.utilisateur.UtilisateurMaitre?.UtilisateurType
        + "\n" + this.utilisateur.UtilisateurMaitre?.EntiteLibelle;
    }
    this.utilisateurMaitreLibelleFC.setValue(utilisateurMaitreLibelle);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.utilisateur.Actif = this.actifFC.value ? true : false;
    this.utilisateur.Nom = this.nomFC.value;
    this.utilisateur.Login = (this.eMailFC.value ? null : this.loginFC.value);
    this.utilisateur.Pwd = this.pwdFC.value;
    this.utilisateur.EMail = this.eMailFC.value;
    this.utilisateur.Tel = this.telFC.value;
    this.utilisateur.TelMobile = this.telMobileFC.value;
    this.utilisateur.Fax = this.faxFC.value;
    this.utilisateur.HabilitationAdministration = this.habilitationAdministrationFC.value;
    this.utilisateur.HabilitationAnnuaire = this.habilitationAnnuaireFC.value;
    this.utilisateur.HabilitationLogistique = this.habilitationLogistiqueFC.value;
    this.utilisateur.HabilitationMessagerie = this.habilitationMessagerieFC.value;
    this.utilisateur.HabilitationModuleCentreDeTri = this.habilitationModuleCentreDeTriFC.value;
    this.utilisateur.HabilitationModuleCollectivite = this.habilitationModuleCollectiviteFC.value;
    this.utilisateur.HabilitationQualite = this.habilitationQualiteFC.value;
    this.utilisateur.HabilitationModulePrestataire = this.habilitationModulePrestataireFC.value;
    this.utilisateur.PiedEmail = this.piedEmailHTMLFC.value;
    this.utilisateur.ContactAdresse = this.contactAdresseList.find(x => x.RefContactAdresse === this.contactAdresseListFC.value);
  }
  //-----------------------------------------------------------------------------------
  //on change EMail
  onChangeEMail(event: any) {
    if (event.target.value) {
      this.loginFC.disable();
    }
    else {
      this.loginFC.enable();
    }
  }
  //-----------------------------------------------------------------------------------
  //Chose habilitation
  choseHabilitationLogistique(val: string) {
    this.saveData();
    if (val == null || val !== 'Transporteur') {
      this.utilisateur.Transporteur = null;
    }
    else if (val === 'Transporteur') {
      this.utilisateur.CentreDeTri = null;
      this.utilisateur.Collectivite = null;
      this.utilisateur.HabilitationModuleCollectivite = null;
      this.utilisateur.Client = null;
      this.utilisateur.HabilitationQualite = null;
      this.utilisateur.Prestataire = null;
      this.utilisateur.HabilitationModulePrestataire = null;
    }
    this.contactAdresseList = [];
    this.utilisateur.ContactAdresse = null;
    this.updateForm();
  }
  choseHabilitationModulePrestataire(val: string) {
    this.saveData();
    if (val == null || val === HabilitationModulePrestataire.Administrateur) {
      this.utilisateur.Prestataire = null;
    }
    else if (val !== HabilitationModulePrestataire.Administrateur) {
      this.utilisateur.Transporteur = null;
      this.utilisateur.HabilitationLogistique = null;
      this.utilisateur.CentreDeTri = null;
      this.utilisateur.Collectivite = null;
      this.utilisateur.HabilitationModuleCollectivite = null;
      this.utilisateur.Client = null;
      this.utilisateur.HabilitationQualite = null;
    }
    this.contactAdresseList = [];
    this.utilisateur.ContactAdresse = null;
    this.updateForm();
  }
  choseHabilitationModuleCollectivite(val: string) {
    this.saveData();
    if (val == null || val !== 'Utilisateur') {
      this.utilisateur.Collectivite = null;
    }
    else if (val === 'Utilisateur') {
      this.utilisateur.Transporteur = null;
      this.utilisateur.HabilitationLogistique = null;
      this.utilisateur.CentreDeTri = null;
      this.utilisateur.Client = null;
      this.utilisateur.HabilitationQualite = null;
      this.utilisateur.Prestataire = null;
      this.utilisateur.HabilitationModulePrestataire = null;
    }
    this.contactAdresseList = [];
    this.utilisateur.ContactAdresse = null;
    this.updateForm();
  }
  choseHabilitationQualite(val: string) {
    this.saveData();
    if (val == null || val !== 'Client') {
      this.utilisateur.Client = null;
    }
    else if (val === 'Client') {
      this.utilisateur.Transporteur = null;
      this.utilisateur.HabilitationLogistique = null;
      this.utilisateur.CentreDeTri = null;
      this.utilisateur.Collectivite = null;
      this.utilisateur.HabilitationModuleCollectivite = null;
      this.utilisateur.Prestataire = null;
      this.utilisateur.HabilitationModulePrestataire = null;
    }
    this.contactAdresseList = [];
    this.utilisateur.ContactAdresse = null;
    this.updateForm();
  }
  choseHabilitationModuleCentreDeTri(val: string) {
    this.saveData();
    if (val == null || val !== 'Utilisateur') {
      this.utilisateur.CentreDeTri = null;
    }
    else if (val === 'Utilisateur') {
      this.utilisateur.Transporteur = null;
      this.utilisateur.HabilitationLogistique = null;
      this.utilisateur.Collectivite = null;
      this.utilisateur.HabilitationModuleCollectivite = null;
      this.utilisateur.Client = null;
      this.utilisateur.HabilitationQualite = null;
      this.utilisateur.Prestataire = null;
      this.utilisateur.HabilitationModulePrestataire = null;
    }
    this.contactAdresseList = [];
    this.utilisateur.ContactAdresse = null;
    this.updateForm();
  }
  removeUtilisateurMaitre() {
    this.saveData();
    this.utilisateur.RefUtilisateurMaitre = null;
    this.utilisateur.UtilisateurMaitre = null;
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Chose related element
  choseElement(elementType: string) {
    let data: any;
    //Init
    this.saveData();
    switch (elementType) {
      case "TransporteurForUtilisateur":
        data = { title: this.applicationUserContext.getCulturedRessourceText(931), type: elementType };
        break;
      case "CentreDeTriForUtilisateur":
        data = { title: this.applicationUserContext.getCulturedRessourceText(930), type: elementType };
        break;
      case "CollectiviteForUtilisateur":
        data = { title: this.applicationUserContext.getCulturedRessourceText(932), type: elementType };
        break;
      case "ClientForUtilisateur":
        data = { title: this.applicationUserContext.getCulturedRessourceText(933), type: elementType };
        break;
      case "PrestataireForUtilisateur":
        data = { title: this.applicationUserContext.getCulturedRessourceText(1253), type: elementType };
        break;
      case "UtilisateurMaitreForUtilisateur":
        data = { title: this.applicationUserContext.getCulturedRessourceText(1512), type: elementType, exclude: this.utilisateur.RefUtilisateur?.toString() };
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
        //Get linked element
        switch (elementType) {
          case "TransporteurForUtilisateur":
            this.dataModelService.getEntite(result.ref, null, null).subscribe(result => {
              //Update data
              this.utilisateur.Transporteur = result;
              this.updateForm();
              this.fillContactAdresse();
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "CentreDeTriForUtilisateur":
            this.dataModelService.getEntite(result.ref, null, null).subscribe(result => {
              //Update data
              this.utilisateur.CentreDeTri = result;
              this.updateForm();
              this.fillContactAdresse();
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "CollectiviteForUtilisateur":
            this.dataModelService.getEntite(result.ref, null, null).subscribe(result => {
              //Update data
              this.utilisateur.Collectivite = result;
              this.updateForm();
              this.fillContactAdresse();
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "ClientForUtilisateur":
            this.dataModelService.getEntite(result.ref, null, null).subscribe(result => {
              //Update data
              this.utilisateur.Client = result;
              this.updateForm();
              this.fillContactAdresse();
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "PrestataireForUtilisateur":
            this.dataModelService.getEntite(result.ref, null, null).subscribe(result => {
              //Update data
              this.utilisateur.Prestataire = result;
              this.updateForm();
              this.fillContactAdresse();
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "UtilisateurMaitreForUtilisateur":
            this.dataModelService.getDataModel<dataModelsInterfaces.Utilisateur>(result.ref, "UtilisateurComponent").subscribe(result => {
              //Update data
              this.utilisateur.RefUtilisateurMaitre = result.RefUtilisateur;
              this.utilisateur.UtilisateurMaitre = result;
              this.updateForm();
              this.onUtilisateurMaitreSelected(result.RefUtilisateur);
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
        }
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Get all available ContactAdresse
  fillContactAdresse() {
    //Get existing contactAdresse
    //Find correct Entite
    let refEntite = this.utilisateur.Client?.RefEntite;
    if (this.utilisateur.Transporteur) { refEntite = this.utilisateur.Transporteur.RefEntite; }
    if (this.utilisateur.Collectivite) { refEntite = this.utilisateur.Collectivite.RefEntite; }
    if (this.utilisateur.CentreDeTri) { refEntite = this.utilisateur.CentreDeTri.RefEntite; }
    if (this.utilisateur.Prestataire) { refEntite = this.utilisateur.Prestataire.RefEntite; }
    this.listService.getListContactAdresse(this.utilisateur.ContactAdresse?.RefContactAdresse
      , refEntite, null, null, true).subscribe(result => {
        this.contactAdresseList = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Create profils label
  profilsLabel(): string {
    let s = this.applicationUserContext.getCulturedRessourceText(1498).toUpperCase() + "\n";
    s += this.profils.sort(function (a, b) { return cmp(a.Nom, b.Nom); })
      .map(item => "-> " + item.Nom + " - " + item.UtilisateurType + " - " + item.EntiteLibelle)
      .join("\n");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Chose UtilisateurType
  choseUtilisateurType(val: string) {
    this.saveData();
    switch (val) {
      case UtilisateurType.Valorplast.toString():
        this.utilisateur.HabilitationAdministration = null;
        this.utilisateur.HabilitationLogistique = null;
        this.utilisateur.HabilitationAnnuaire = null;
        this.utilisateur.HabilitationMessagerie = null;
        this.utilisateur.HabilitationStatistique = null;
        this.utilisateur.Collectivite = null;
        this.utilisateur.HabilitationModuleCollectivite = null;
        this.utilisateur.CentreDeTri = null;
        this.utilisateur.HabilitationModuleCentreDeTri = null;
        this.utilisateur.Transporteur = null;
        this.utilisateur.Client = null;
        this.utilisateur.HabilitationQualite = null;
        this.utilisateur.Prestataire = null;
        this.utilisateur.HabilitationModulePrestataire = null;
        this.utilisateur.ContactAdresse = null;
        break;
      case UtilisateurType.Collectivite.toString():
        this.utilisateur.HabilitationAdministration = null;
        this.utilisateur.HabilitationLogistique = null;
        this.utilisateur.HabilitationAnnuaire = null;
        this.utilisateur.HabilitationMessagerie = null;
        this.utilisateur.HabilitationStatistique = null;
        this.utilisateur.Collectivite = null;
        this.utilisateur.HabilitationModuleCollectivite = HabilitationModuleCollectivite.Utilisateur;
        this.utilisateur.CentreDeTri = null;
        this.utilisateur.HabilitationModuleCentreDeTri = null;
        this.utilisateur.Transporteur = null;
        this.utilisateur.Client = null;
        this.utilisateur.HabilitationQualite = null;
        this.utilisateur.Prestataire = null;
        this.utilisateur.HabilitationModulePrestataire = null;
        this.utilisateur.ContactAdresse = null;
        break;
      case UtilisateurType.CentreDeTri.toString():
        this.utilisateur.HabilitationAdministration = null;
        this.utilisateur.HabilitationLogistique = HabilitationLogistique.CentreDeTri;
        this.utilisateur.HabilitationAnnuaire = HabilitationAnnuaire.Visualisation;
        this.utilisateur.HabilitationMessagerie = null;
        this.utilisateur.HabilitationStatistique = null;
        this.utilisateur.Collectivite = null;
        this.utilisateur.HabilitationModuleCollectivite = null;
        this.utilisateur.CentreDeTri = null;
        this.utilisateur.HabilitationModuleCentreDeTri = HabilitationModuleCentreDeTri.Utilisateur;
        this.utilisateur.Transporteur = null;
        this.utilisateur.Client = null;
        this.utilisateur.HabilitationQualite = null;
        this.utilisateur.Prestataire = null;
        this.utilisateur.HabilitationModulePrestataire = null;
        this.utilisateur.ContactAdresse = null;
        break;
      case UtilisateurType.Transporteur.toString():
        this.utilisateur.HabilitationAdministration = null;
        this.utilisateur.HabilitationLogistique = HabilitationLogistique.Transporteur;
        this.utilisateur.HabilitationAnnuaire = HabilitationAnnuaire.Visualisation;
        this.utilisateur.HabilitationMessagerie = null;
        this.utilisateur.HabilitationStatistique = null;
        this.utilisateur.Collectivite = null;
        this.utilisateur.HabilitationModuleCollectivite = null;
        this.utilisateur.CentreDeTri = null;
        this.utilisateur.HabilitationModuleCentreDeTri = null;
        this.utilisateur.Transporteur = null;
        this.utilisateur.Client = null;
        this.utilisateur.HabilitationQualite = null;
        this.utilisateur.Prestataire = null;
        this.utilisateur.HabilitationModulePrestataire = null;
        this.utilisateur.ContactAdresse = null;
        break;
      case UtilisateurType.Client.toString():
        this.utilisateur.HabilitationAdministration = null;
        this.utilisateur.HabilitationLogistique = HabilitationLogistique.Client;
        this.utilisateur.HabilitationAnnuaire = HabilitationAnnuaire.Visualisation;
        this.utilisateur.HabilitationMessagerie = null;
        this.utilisateur.HabilitationStatistique = null;
        this.utilisateur.Collectivite = null;
        this.utilisateur.HabilitationModuleCollectivite = null;
        this.utilisateur.CentreDeTri = null;
        this.utilisateur.HabilitationModuleCentreDeTri = null;
        this.utilisateur.Transporteur = null;
        this.utilisateur.Client = null;
        this.utilisateur.HabilitationQualite = HabilitationQualite.Client;
        this.utilisateur.Prestataire = null;
        this.utilisateur.HabilitationModulePrestataire = null;
        this.utilisateur.ContactAdresse = null;
        break;
      case UtilisateurType.Prestataire.toString():
        this.utilisateur.HabilitationAdministration = null;
        this.utilisateur.HabilitationAnnuaire = null;
        this.utilisateur.HabilitationMessagerie = null;
        this.utilisateur.HabilitationStatistique = null;
        this.utilisateur.Collectivite = null;
        this.utilisateur.HabilitationModuleCollectivite = null;
        this.utilisateur.CentreDeTri = null;
        this.utilisateur.HabilitationModuleCentreDeTri = null;
        this.utilisateur.Transporteur = null;
        this.utilisateur.Client = null;
        this.utilisateur.HabilitationQualite = null;
        this.utilisateur.Prestataire = null;
        this.utilisateur.HabilitationModulePrestataire = HabilitationModulePrestataire.Fournisseur;
        this.utilisateur.ContactAdresse = null;
        break;
    }
    //End
    this.updateForm();
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Apply filter
  protected filterUtilisateursMaitre() {
    if (!this.utilisateurMaitreList) {
      return;
    }
    // get the search keyword
    let search = this.form.get("UtilisateurMaitreListFilter").value;
    if (!search) {
      this.filteredUtilisateurMaitreList.next(this.utilisateurMaitreList.slice());
      return;
    } else {
      search = removeAccents(search.toLowerCase());
    }
    // filter 
    this.filteredUtilisateurMaitreList.next(
      this.utilisateurMaitreList.filter(e => removeAccents(e.Nom.toLowerCase() + e.EntiteLibelle.toLowerCase()).indexOf(search) > -1)
    );
  }
  //-----------------------------------------------------------------------------------
  //Ask for user validation if Maitre selected
  onUtilisateurMaitreSelected(refUtilisateurMaitre: number) {
    if (refUtilisateurMaitre) {
      const dialogRef = this.dialog.open(ConfirmComponent, {
        width: "350px",
        data: {
          title: this.applicationUserContext.getCulturedRessourceText(300),
          message: this.applicationUserContext.getCulturedRessourceText(1508),
          htmlMessage: ""
        },
        autoFocus: false,
        restoreFocus: false
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result !== "yes") {
          this.removeUtilisateurMaitre();
        }
      });
    }
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave(nextAction: string) {
    this.saveData();
    if ((this.originalActif && !this.utilisateur.Actif && this.utilisateur.ContactAdresse)
      || (!this.utilisateur.Actif && this.utilisateur.HasProfils)) {
      let msg: string = "";
      if (this.originalActif && !this.utilisateur.Actif && this.utilisateur.ContactAdresse) {
        msg = this.applicationUserContext.getCulturedRessourceHTML(1240);
        if (this.utilisateur.ContactAdresse?.ContactAdresseContactAdresseProcesss?.length > 0) {
          msg += "<br/>" + this.applicationUserContext.getCulturedRessourceHTML(1241);
        }
      }
      if (!this.utilisateur.Actif && this.utilisateur.HasProfils) {
        if (msg) { msg += "<br/><br/>"; }
        msg += this.applicationUserContext.getCulturedRessourceHTML(1506);
      }
      const dialogRef = this.dialog.open(ConfirmComponent, {
        width: "350px",
        data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: "", htmlMessage: msg },
        autoFocus: false,
        restoreFocus: false
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result === "yes") {
          this.save(nextAction);
        }
      });
    }
    else {
      this.save(nextAction);
    }
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  save(nextAction: string) {
    this.saveData();
    //Process
    var url = this.baseUrl + "evapi/utilisateur";
    //Update 
    this.dataModelService.postDataModel<dataModelsInterfaces.Utilisateur>(this.utilisateur, this.componentName)
      .subscribe(result => {
        //Redirect to grid and inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
        //Next action if applicable
        switch (nextAction) {
          case "sendEmailPasswordFR":
            this.http
              .post<dataModelsInterfaces.Utilisateur>(url + "/sendemailpassword", result, {
                headers: new HttpHeaders()
                  .set("cultureName", "fr-FR"),
                responseType: "json"
              })
              .subscribe(result => {
                //Inform user
                this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1105), duration: 4000 } as appInterfaces.SnackbarMsg);
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
          case "sendEmailPasswordEN":
            this.http
              .post<dataModelsInterfaces.Utilisateur>(url + "/sendemailpassword", result, {
                headers: new HttpHeaders()
                  .set("cultureName", "en-GB"),
                responseType: "json"
              })
              .subscribe(result => {
                //Inform user
                this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1105), duration: 4000 } as appInterfaces.SnackbarMsg);
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(this.ressBeforeDel) },
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
    this.dataModelService.deleteDataModel<dataModelsInterfaces.Utilisateur>(this.utilisateur.RefUtilisateur, this.componentName)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(this.ressAfterDel), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    let s: string = getCreationModificationTooltipText(this.utilisateur);
    if (this.utilisateur.AffectationUtilisateurMaitreText) {
      s += "\n" + this.utilisateur.AffectationUtilisateurMaitreText;
    }
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Enable/Disable HabilitationLogistique
  habilitationLogistiqueDisabled(h: string): boolean {
    let r: boolean = false;
    if (this.utilisateurTypeFC.value == UtilisateurType.Valorplast) {
      if (h == HabilitationLogistique.CentreDeTri
        || h == HabilitationLogistique.Client
        || h == HabilitationLogistique.Transporteur
      ) { r = true; }
    }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Enable/Disable HabilitationQualite
  habilitationQualiteDisabled(h: string): boolean {
    let r: boolean = false;
    if (this.utilisateurTypeFC.value == UtilisateurType.Valorplast) {
      if (h == HabilitationQualite.Fournisseur
        || h == HabilitationQualite.Client
      ) { r = true; }
    }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Enable/Disable HabilitationQualite
  habilitationModuleCollectiviteDisabled(h: string): boolean {
    let r: boolean = false;
    if (this.utilisateurTypeFC.value == UtilisateurType.Valorplast) {
      if (h == HabilitationModuleCollectivite.Utilisateur
      ) { r = true; }
    }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Create profils info
  getProfilInfos(profil: UtilisateurList): string {
    let s = profil.Nom;
    if (GetUtilisateurTypeLocalizedLabel(profil.UtilisateurType, this.applicationUserContext)) {
      s += " - " + GetUtilisateurTypeLocalizedLabel(profil.UtilisateurType, this.applicationUserContext);
    }
    if (profil.EntiteLibelle) {
      s += " - " + profil.EntiteLibelle;
    }
    //End
    return s;
  }
}
