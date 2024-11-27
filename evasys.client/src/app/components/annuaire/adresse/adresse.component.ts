import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import { HabilitationLogistique } from "../../../globals/enums";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import { ErrorStateMatcher } from "@angular/material/core";

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
    selector: "adresse",
    templateUrl: "./adresse.component.html",
    standalone: false
})

export class AdresseComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  @Input() refAdresse: number;
  @Input() adr: dataModelsInterfaces.Adresse;
  @Input() type: string;
  @Input() ent: dataModelsInterfaces.Entite;
  @Output() askClose = new EventEmitter<any>();
  form: UntypedFormGroup;
  entite: dataModelsInterfaces.Entite;
  adresse: dataModelsInterfaces.Adresse;
  adresseTypeList: dataModelsInterfaces.AdresseType[];
  paysList: dataModelsInterfaces.Pays[];
  //Global lock
  locked: boolean = true;
  constructor(private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    // create an empty object from the Adresse interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: [true],
      AdresseTypeList: [null, Validators.required],
      Libelle: [null],
      Adr1: [null],
      Adr2: [null],
      CodePostal: [null, Validators.required],
      Ville: [null, Validators.required],
      PaysList: [null, Validators.required],
      Tel: [null],
      Email: [null, Validators.email],
      SiteWeb: [null],
      Horaires: [null],
      Cmt: [null]
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
    this.adresse = { ...this.adr };
    this.entite = { ...this.ent };
    //Get existing Pays
    this.listService.getListPays(this.adresse.Pays?.RefPays, false).subscribe(result => {
      this.paysList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing AdresseType
    this.listService.getListAdresseType().subscribe(result => {
      this.adresseTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("Actif").disable();
    this.form.get("AdresseTypeList").disable();
    this.form.get("Libelle").disable();
    this.form.get("Adr1").disable();
    this.form.get("Adr2").disable();
    this.form.get("CodePostal").disable();
    this.form.get("Ville").disable();
    this.form.get("PaysList").disable();
    this.form.get("Tel").disable();
    this.form.get("Email").disable();
    this.form.get("SiteWeb").disable();
    this.form.get("Horaires").disable();
    this.form.get("Cmt").disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("Actif").setValue(this.adresse.Actif);
    this.form.get("AdresseTypeList").setValue(this.adresse?.AdresseType?.RefAdresseType);
    this.form.get("Libelle").setValue(this.adresse.Libelle);
    this.form.get("Adr1").setValue(this.adresse.Adr1);
    this.form.get("Adr2").setValue(this.adresse.Adr2);
    this.form.get("CodePostal").setValue(this.adresse.CodePostal);
    this.form.get("Ville").setValue(this.adresse.Ville);
    this.form.get("PaysList").setValue(this.adresse?.Pays?.RefPays);
    this.form.get("Tel").setValue(this.adresse.Tel);
    this.form.get("Email").setValue(this.adresse.Email);
    this.form.get("SiteWeb").setValue(this.adresse.SiteWeb);
    this.form.get("Horaires").setValue(this.adresse.Horaires);
    this.form.get("Cmt").setValue(this.adresse.Cmt);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.adresse.Actif = this.form.get("Actif").value;
    this.adresse.AdresseType = this.adresseTypeList.find(x => x.RefAdresseType === this.form.get("AdresseTypeList").value);
    this.adresse.Libelle = this.form.get("Libelle").value;
    this.adresse.Adr1 = this.form.get("Adr1").value;
    this.adresse.Adr2 = this.form.get("Adr2").value;
    this.adresse.CodePostal = this.form.get("CodePostal").value;
    this.adresse.Ville = this.form.get("Ville").value;
    this.adresse.Pays = this.paysList.find(x => x.RefPays === this.form.get("PaysList").value);
    this.adresse.Tel = this.form.get("Tel").value;
    this.adresse.Email = this.form.get("Email").value;
    this.adresse.SiteWeb = this.form.get("SiteWeb").value;
    this.adresse.Horaires = this.form.get("Horaires").value;
    this.adresse.Cmt = this.form.get("Cmt").value;
  }
  //-----------------------------------------------------------------------------------
  //Open web site
  onSiteWeb() {
    let url: string = '';
    if (!/^http[s]?:\/\//.test(this.form.get('SiteWeb').value)) {
      url += 'http://';
    }
    url += this.form.get('SiteWeb').value;
    window.open(url, '_blank');
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.adresse);
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.applyChanges();
    //Save data to current object or save to database
    switch (this.type) {
      case "AnnuaireAdresse":
        //Save data
        this.adr = { ...this.adresse };
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: this.adr });
        break;
    }
  }
}
