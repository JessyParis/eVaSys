import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpParams } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { EventEmitterService } from "../../../services/event-emitter.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import * as appClasses from "../../../classes/appClasses";
import { cmp, getContratCollectiviteLabel, getContratLabelSingleLine, showErrorToUser } from "../../../globals/utils";
import { HabilitationLogistique, HabilitationModuleCollectivite } from "../../../globals/enums";
import moment from "moment";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import { ErrorStateMatcher } from "@angular/material/core";
import { EntiteEntiteComponent } from "../entite-entite/entite-entite.component";
import { fadeInOnEnterAnimation } from "angular-animations";

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
    selector: "my-contacts",
    templateUrl: "./my-contacts.component.html",
    animations: [
        fadeInOnEnterAnimation(),
    ],
    standalone: false
})

export class MyContactsComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  form: UntypedFormGroup;
  modifying: boolean = false;
  entite: dataModelsInterfaces.Entite;
  lastContratCollectivites: string;
  lastContratType1: string;
  centreDeTriAssocies: dataModelsInterfaces.EntiteEntite[] = [];
  collectiviteAssociees: dataModelsInterfaces.EntiteEntite[] = [];
  produitAssocies: dataModelsInterfaces.EntiteProduit[] = [];
  produitInterdits: dataModelsInterfaces.EntiteProduit[] = [];
  //Global lock
  locked: boolean = true;
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router, private fb: UntypedFormBuilder
    , private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, public applicationUserContext: ApplicationUserContext
    , private eventEmitterService: EventEmitterService
    , private snackBarQueueService: SnackBarQueueService
    , private dataModelService: DataModelService
    , public dialog: MatDialog) {
    //Sow/hide ModuleCollectivite header
    if (applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) {
      applicationUserContext.visibleHeaderCollectivite = true;
    }
    //Create form
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Cmt: ["", Validators.required]
    });
    if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique == HabilitationLogistique.Transporteur
      || this.applicationUserContext.connectedUtilisateur.HabilitationLogistique == HabilitationLogistique.Client
      || this.applicationUserContext.connectedUtilisateur.HabilitationModuleCollectivite == HabilitationModuleCollectivite.Utilisateur
      || this.applicationUserContext.connectedUtilisateur.Prestataire?.RefEntite
      || this.applicationUserContext.connectedUtilisateur.CentreDeTri?.RefEntite
    ) {
      this.locked = false;
    }
    else {
      this.lockScreen();
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //Find Entite
    let refEntite = 0;
    if (this.applicationUserContext.connectedUtilisateur.Transporteur) { refEntite = this.applicationUserContext.connectedUtilisateur.Transporteur.RefEntite; }
    else if (this.applicationUserContext.connectedUtilisateur.Client) { refEntite = this.applicationUserContext.connectedUtilisateur.Client.RefEntite; }
    else if (this.applicationUserContext.connectedUtilisateur.Prestataire) { refEntite = this.applicationUserContext.connectedUtilisateur.Prestataire.RefEntite; }
    else if (this.applicationUserContext.connectedUtilisateur.Collectivite) { refEntite = this.applicationUserContext.connectedUtilisateur.Collectivite.RefEntite; }
    else if (this.applicationUserContext.connectedUtilisateur.CentreDeTri) { refEntite = this.applicationUserContext.connectedUtilisateur.CentreDeTri.RefEntite; }
    else if (this.applicationUserContext.filterCollectivites.length == 1) {
      refEntite = this.applicationUserContext.filterCollectivites[0].RefEntite;
    }
    // get contacts
    this.dataModelService.getEntiteContacts(refEntite).subscribe(result => {
      this.entite = result;
      //Process ContactAdresse without Adresse
      this.entite.ContactAdresses.sort(function (a, b) { return cmp(a.Contact?.Nom, b.Contact?.Nom); });
      this.entite.ContactAdresses = this.entite.ContactAdresses.filter(f => !(this.entite.Adresses.some(s => s.RefAdresse == f.RefAdresse)));
      //Keep only Actif
      this.entite.ContactAdresses = this.entite.ContactAdresses.filter(f => f.Actif == true);
      //Keep only related user Actif
      this.entite.ContactAdresses.forEach(c => c.Utilisateurs = c.Utilisateurs.filter(f => f.Actif == true));
      //Process ContactAdresse with Adresse
      //Keep only Actif Adresse
      this.entite.Adresses = this.entite.Adresses.filter(f => f.Actif == true);
      for (let adr of this.entite.Adresses) {
        adr.ContactAdresses.sort(function (a, b) { return cmp(a.Contact?.Nom, b.Contact?.Nom); });
        //Keep only Actif
        adr.ContactAdresses = adr.ContactAdresses.filter(f => f.Actif == true);
        //Keep only related user Actif
        adr.ContactAdresses.forEach(c => c.Utilisateurs = c.Utilisateurs.filter(f => f.Actif == true));
      }
      //Process ContratCollectivites, keep last
      if (this.entite.ContratCollectivites?.length > 0) {
        const max = this.entite.ContratCollectivites.reduce(function (prev, current) {
          return (prev.DFin > current.DFin) ? prev : current
        })
        this.lastContratCollectivites = this.applicationUserContext.getCulturedRessourceText(1357) + " - "
          + getContratCollectiviteLabel(max);
      }
      //Process Contrat RI, keep last
      if (this.entite.Contrats?.length > 0) {
        const max = this.entite.Contrats
          .filter(f => f.ContratType.RefContratType == 1)
          .reduce(function (prev, current) {
          return (prev.DFin > current.DFin) ? prev : current
        })
        this.lastContratType1 = getContratLabelSingleLine(max);
      }
      //Process EntiteEntite
      this.entite.EntiteEntites.sort(function (a, b) { return cmp(a.LibelleEntiteRtt, b.LibelleEntiteRtt); });
      this.centreDeTriAssocies = this.entite.EntiteEntites.filter(f => f.Actif == true && f.RefEntiteRttType == 3);
      this.collectiviteAssociees = this.entite.EntiteEntites.filter(f => f.Actif == true && f.RefEntiteRttType == 1);
      this.entite.EntiteProduits.sort(function (a, b) { return cmp(a.Produit.Libelle, b.Produit.Libelle); });
      this.produitInterdits = this.entite.EntiteProduits.filter(f => f.Interdit);
      this.produitAssocies = this.entite.EntiteProduits.filter(f => !f.Interdit);
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Open modificvation if requested
    if (this.activatedRoute.snapshot.params["action"] == "mod") {
      this.modifying = true;
    }
  }
  //-----------------------------------------------------------------------------------
  //Open web site
  onSiteWeb(u: string) {
    let url: string = '';
    if (!/^http[s]?:\/\//.test(u)) {
      url += 'https://';
    }
    url += u;
    window.open(url, '_blank');
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("Cmt").disable();
  }
  //-----------------------------------------------------------------------------------
  //Saving modifications
  onSave() {
    let action = {} as dataModelsInterfaces.Action;
    action.RefEntite = this.entite.RefEntite;
    action.FormeContact = { RefFormeContact: 6 } as dataModelsInterfaces.FormeContact;
    action.Libelle = "Demande de modification d'annuaire le " + moment().format("DD/MM/YYYY");
    action.Cmt = this.form.value.Cmt;
    //Insert existing model
    this.dataModelService.postAction(action).subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(421), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.modifying = false;
        this.form.value.Cmt = "";
        this.applicationUserContext.setDefaultModule();
        this.applicationUserContext.currentMenu = new appClasses.EnvMenu();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
}
