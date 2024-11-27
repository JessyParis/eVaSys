import { Component, Inject, OnInit, Input, Output, EventEmitter } from "@angular/core";
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
import { debounceTime, switchMap } from "rxjs/operators";
import { of } from "rxjs";
import { Observable } from "rxjs";
import { ErrorStateMatcher } from "@angular/material/core";
import { showInformationToUser } from "../../../globals/utils";

class MyErrorStateMatcher implements ErrorStateMatcher {
  //Constructor
  constructor() { }
  isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    let result: boolean = false;
    result = !!((control && control.invalid));
    return result;
  }
}

function autocompleteObjectValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    if (typeof control.value === 'string') {
      return { 'invalidAutocompleteObject': { value: control.value } }
    }
    return null  /* valid option selected */
  }
}

@Component({
    selector: "entite-entite",
    templateUrl: "./entite-entite.component.html",
    standalone: false
})

export class EntiteEntiteComponent implements OnInit {
  @Input() refEntiteEntite: number;
  @Input() type: string;
  @Input() eE: dataModelsInterfaces.EntiteEntite;
  @Input() entite: dataModelsInterfaces.Entite;
  @Input() refEntiteType: number;
  @Output() askClose = new EventEmitter<any>();
  matcher = new MyErrorStateMatcher();
  entiteEntite: dataModelsInterfaces.EntiteEntite;
  form: UntypedFormGroup;
  entiteList: Observable<dataModelsInterfaces.EntiteList[]>;
  //Global lock
  locked: boolean = true;
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    // create an empty object from the EntiteEntite interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: [true],
      EntiteList: [null, { validators: [autocompleteObjectValidator(), Validators.required] }],
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
    this.entiteEntite = { ...this.eE };
    //Register service to get EntiteNA
    this.entiteList = this.form.get("EntiteList").valueChanges
      .pipe(
        debounceTime(300),
        switchMap(value => this.getEntiteList(value))
      );
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("Actif").disable();
    this.form.get("EntiteList").disable();
    this.form.get("Cmt").disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("Actif").setValue(this.entiteEntite.Actif);
    this.form.get("EntiteList").setValue(this.entiteEntite?.RefEntiteRtt);
    this.form.get("Cmt").setValue(this.entiteEntite.Cmt);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.entiteEntite.Actif = this.form.get("Actif").value;
    //this.entiteEntite.RefEntiteRtt = this.form.get("EntiteList").value;
    this.entiteEntite.Cmt = this.form.get("Cmt").value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    //Save data to current object 
    this.applyChanges();
    //Close
    switch (this.type) {
      case "AnnuaireEntiteEntite":
        //Check duplicates
        if (this.entite.EntiteEntites?.some(item => item.RefEntiteEntite != this.entiteEntite.RefEntiteEntite && item.RefEntiteRtt == this.entiteEntite.RefEntiteRtt)) {
          showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(115), this.applicationUserContext.getCulturedRessourceText(410), this.applicationUserContext);
        }
        else {
          //Save data
          this.eE = { ...this.entiteEntite };
          //Emit event for dialog closing
          this.askClose.emit({ type: this.type, ref: this.eE });
        }
        break;
    }
  }
  //-----------------------------------------------------------------------------------
  //Get Entite
  getEntiteList(value: string) {
    if (value && value.length > 1) {
      switch (this.refEntiteType) {
        case 1:
          return this.listService.getListCollectivite(null, null, false, true, null, value, true);
        default:
          return this.listService.getListEntite(null, this.refEntiteType, value, true);
      }
    }
    else {
      let r: dataModelsInterfaces.EntiteList[] = [];
      return of(r);
    }
  }
  //-----------------------------------------------------------------------------------
  //Autocomplete display
  autoEntiteDisplayFn(coll?: dataModelsInterfaces.EntiteList): string | undefined {
    return coll ? coll.Libelle : undefined;
  }
  //-----------------------------------------------------------------------------------
  //Autocomplete selected value
  onEntiteSelected(opt:any) {
    this.entiteEntite.RefEntiteRtt = opt.option.value.RefEntite;
    this.entiteEntite.ActifEntiteRtt = opt.option.value.Actif;
    this.entiteEntite.RefEntiteRttType = this.refEntiteType;
    this.entiteEntite.LibelleEntiteRtt = opt.option.value.Libelle;
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationTooltipText() {
    let eE = this.entiteEntite;
    if (!!eE.DCreation) {
      return `${this.applicationUserContext.getCulturedRessourceText(388) + " " + moment(eE.DCreation).format("L") + " " + this.applicationUserContext.getCulturedRessourceText(390) + " " + eE.LibelleUtilisateurCreation}`;
    }
    else {
      return "";
    }
  }
}
