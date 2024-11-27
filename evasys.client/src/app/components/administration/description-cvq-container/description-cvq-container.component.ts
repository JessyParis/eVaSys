import { Component, Inject, OnInit, OnDestroy } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { showErrorToUser } from "../../../globals/utils";
import { takeUntil } from "rxjs/operators";
import { Subject, ReplaySubject } from "rxjs";
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

@Component({
    selector: "description-cvq-container",
    templateUrl: "./description-cvq-container.component.html",
    standalone: false
})

export class DescriptionCVQContainerComponent implements OnInit, OnDestroy {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  descriptionCVQs: dataModelsInterfaces.DescriptionCVQ[] = [];
  produitListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  produitListFilterFC: UntypedFormControl = new UntypedFormControl(null);
  produitList: dataModelsInterfaces.ProduitList[];
  filteredProduitList: ReplaySubject<dataModelsInterfaces.ProduitList[]> = new ReplaySubject<dataModelsInterfaces.ProduitList[]>(1);
  produitDestListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  produitDestListFilterFC: UntypedFormControl = new UntypedFormControl(null);
  produitDestList: dataModelsInterfaces.ProduitList[];
  filteredProduitDestList: ReplaySubject<dataModelsInterfaces.ProduitList[]> = new ReplaySubject<dataModelsInterfaces.ProduitList[]>(1);
  // Subject that emits when the component has been destroyed.
  protected _onDestroy = new Subject<void>();
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    //Get existing Produit
    listService.getListProduit(null, null, null, null, null, null, null, false, false).subscribe(result => {
      this.produitList = result;
      this.filteredProduitList.next(this.produitList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get Produit available for copy
    listService.getListProduit(null, null, null, null, null, null, null, false, true).subscribe(result => {
      this.produitDestList = result;
      this.filteredProduitDestList.next(this.produitDestList.slice());
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Create form
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["refProduit"], 10);
    //Load initial data
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.produitListFC.setValue(id);
        //Get data
        this.dataModelService.getDescriptionCVQs(id).subscribe(result => {
          if (result) {
            this.descriptionCVQs = result;
          }
          else {
            this.descriptionCVQs = [];
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
      else {
        this.descriptionCVQs = [];
      }
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
    // listen for search field value changes
    this.produitListFilterFC.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterProduitsMulti();
      });
    this.produitDestListFilterFC.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterProduitsDestMulti();
      });
  }
  //-----------------------------------------------------------------------------------
  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      ProduitList: this.produitListFC,
      ProduitListFilter: this.produitListFilterFC,
      ProduitDestList: this.produitDestListFC,
      ProduitDestListFilter: this.produitDestListFilterFC,
    });
  }
  //-----------------------------------------------------------------------------------
  //Produit selected
  onProduitSelected() {
    //Get data
    this.dataModelService.getDescriptionCVQs(this.produitListFC.value).subscribe(result => {
      if (result) {
        this.descriptionCVQs = result;
      }
      else {
        this.descriptionCVQs = [];
        this.descriptionCVQs = [<dataModelsInterfaces.DescriptionCVQ>{ RefDescriptionCVQ: 0 }];
      }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Add new element
  onAdd() {
    //Create new element
    let newElement = <dataModelsInterfaces.DescriptionCVQ>{
      RefDescriptionCVQ: 0
      , DescriptionCVQProduits: [<dataModelsInterfaces.DescriptionCVQProduit>{ Produit: { RefProduit: this.produitListFC.value } }]
    }
    //Add element
    this.descriptionCVQs.push(newElement);
  }
  //-----------------------------------------------------------------------------------
  //Copy elements
  onCopy() {
    //Process
    var url = this.baseUrl + "evapi/descriptioncvq/copy";
    //Update 
    this.http
      .get<dataModelsInterfaces.DescriptionCVQ>(url, {
        headers: new HttpHeaders()
          .set("refProduit", this.produitListFC.value == null ? "" : this.produitListFC.value.toString())
          .set("refProduitDest", this.produitDestListFC.value == null ? "" : this.produitDestListFC.value.toString())
      })
      .subscribe(() => {
        this.produitListFC.setValue(this.produitDestListFC.value);
        //Show copied elements
        this.dataModelService.getDescriptionCVQs(this.produitListFC.value).subscribe(result => {
          if (result) {
            this.descriptionCVQs = result;
          }
          else {
            this.descriptionCVQs = [];
            this.descriptionCVQs = [<dataModelsInterfaces.DescriptionCVQ>{ RefDescriptionCVQ: 0 }];
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Inform user
        this.snackBarQueueService.addMessage(<appInterfaces.SnackbarMsg>{ text: this.applicationUserContext.getCulturedRessourceText(760), duration: 4000 });
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
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
  //Apply filter
  protected filterProduitsDestMulti() {
    if (!this.produitDestList) {
      return;
    }
    // get the search keyword
    let search = this.produitDestListFilterFC.value;
    if (!search) {
      this.filteredProduitDestList.next(this.produitDestList.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter 
    this.filteredProduitDestList.next(
      this.produitDestList.filter(e => e.Libelle.toLowerCase().indexOf(search) > -1)
    );
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
}
