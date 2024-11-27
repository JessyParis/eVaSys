import { Component, Inject, OnInit, Input, Output, EventEmitter, OnDestroy } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, FormGroupDirective, NgForm, ValidatorFn, AbstractControl, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import moment from "moment";
import { ErrorStateMatcher } from "@angular/material/core";
import { showErrorToUser, showInformationToUser } from "../../../globals/utils";
import { ReplaySubject, Subject, takeUntil } from "rxjs";
import { HabilitationAnnuaire } from "../../../globals/enums";

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
    selector: "entite-produit",
    templateUrl: "./entite-produit.component.html",
    standalone: false
})

export class EntiteProduitComponent implements OnInit, OnDestroy {
  @Input() refEntiteProduit: number;
  @Input() type: string;
  @Input() eP: dataModelsInterfaces.EntiteProduit;
  @Input() entite: dataModelsInterfaces.Entite;
  @Output() askClose = new EventEmitter<any>();
  @Output() title : string;
  matcher = new MyErrorStateMatcher();
  entiteProduit: dataModelsInterfaces.EntiteProduit;
  form: UntypedFormGroup;
  produitListFilterFC: UntypedFormControl = new UntypedFormControl(null);
  produitList: dataModelsInterfaces.Produit[];
  filteredProduitList: ReplaySubject<dataModelsInterfaces.ProduitList[]> = new ReplaySubject<dataModelsInterfaces.ProduitList[]>(1);
  //Global lock
  locked: boolean = true;
  // Subject that emits when the component has been destroyed.
  protected _onDestroy = new Subject<void>();
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    // create an empty object from the EntiteProduit interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: [true],
      ProduitList: [null, Validators.required],
      ProduitListFilter: this.produitListFilterFC,
      Interdit: [null],
      Cmt: [null],
    });
    if (1 != 1) {
      this.lockScreen();
    }
    else {
      this.locked = false;
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    this.entiteProduit = { ...this.eP };
    //Get existing Produit
    this.listService.getListProduit(this.eP.Produit?.RefProduit, null, null, null, null, null, null, false, true).subscribe(result => {
      this.produitList = result;
      this.filteredProduitList.next(this.produitList.slice());
      //Auto select
      if (this.produitList.length === 1) { this.form.get("ProduitList").setValue(this.produitList[0].RefProduit); }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.updateForm();
    // listen for search field value changes
    this.produitListFilterFC.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterProduitsMulti();
      });
  }
  //-----------------------------------------------------------------------------------
  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("ProduitList").disable();
    this.form.get("Interdit").disable();
    this.form.get("Cmt").disable();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    if (this.applicationUserContext.connectedUtilisateur.HabilitationAnnuaire != HabilitationAnnuaire.Administrateur) {
      this.form.get("Interdit").disable();
      if (this.entiteProduit.Interdit) {
        this.form.get("Cmt").disable();
        this.form.get("ProduitList").disable();
        this.locked = true;
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("ProduitList").setValue(this.entiteProduit?.Produit?.RefProduit);
    this.form.get("Interdit").setValue(this.entiteProduit.Interdit);
    this.form.get("Cmt").setValue(this.entiteProduit.Cmt);
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.entiteProduit.Cmt = this.form.get("Cmt").value;
    this.entiteProduit.Interdit = this.form.get("Interdit").value;
    this.entiteProduit.Produit = this.produitList.find(x => x.RefProduit === this.form.get("ProduitList").value);
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    //Save data to current object 
    this.applyChanges();
    //Check duplicates
    if (this.entite.EntiteProduits?.some(item =>
      item.RefEntiteProduit != this.entiteProduit.RefEntiteProduit && item.Produit.RefProduit == this.entiteProduit.Produit.RefProduit
    )) {
      showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(115), this.applicationUserContext.getCulturedRessourceText(410), this.applicationUserContext);
    }
    else {
      //Save data
      this.eP = { ...this.entiteProduit };
      //Emit event for dialog closing
      this.askClose.emit({ type: this.type, ref: this.eP });
    }
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationTooltipText() {
    let eP = this.entiteProduit;
    if (!!eP.DCreation) {
      return `${this.applicationUserContext.getCulturedRessourceText(388) + " " + moment(eP.DCreation).format("L") + " " + this.applicationUserContext.getCulturedRessourceText(390) + " " + eP.UtilisateurCreation.Nom}`;
    }
    else {
      return "";
    }
  }
  //-----------------------------------------------------------------------------------
  //Apply filter
  protected filterProduitsMulti() {
    if (!this.produitList) {
      return;
    }
    // get the search keyword
    let search = this.produitListFilterFC.value;
    if (!search) {
      this.filteredProduitList.next(this.produitList.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter 
    this.filteredProduitList.next(
      this.produitList.filter(e => e.Libelle.toLowerCase().indexOf(search) > -1)
    );
  }
  //-----------------------------------------------------------------------------------
  //Produit selected
  onProduitDeselected() {
    this.entiteProduit.Produit = null;
  }
}
