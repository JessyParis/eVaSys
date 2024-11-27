import { Component, Inject, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import { HabilitationLogistique } from "../../../globals/enums";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
@Component({
    selector: "adresse-simple",
    templateUrl: "./adresse-simple.component.html",
    standalone: false
})

export class AdresseSimpleComponent implements OnInit {
  @Input() refAdresse: number;
  @Input() type: string;
  @Output() askClose = new EventEmitter<any>();
  form: UntypedFormGroup;
  adresse: dataModelsInterfaces.Adresse;
  paysList: dataModelsInterfaces.Pays[];
  //Global lock
  locked: boolean = true;
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.adresse = {} as dataModelsInterfaces.Adresse;
    // create an empty object from the Adresse interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Libelle: [null],
      Adr1: [null],
      Adr2: [null],
      CodePostal: [null, Validators.required],
      Ville: [null, Validators.required],
      PaysList: [null],
      Horaires: [null],
      Cmt: [null]
    });
    if (this.applicationUserContext.connectedUtilisateur.HabilitationLogistique !== HabilitationLogistique.Administrateur) {
      this.lockScreen();
    }
    else {
      this.locked = false;
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //Get Adresse
    if (this.refAdresse) {
      this.dataModelService.getAdresse(this.refAdresse, true).subscribe(result => {
        this.adresse = result;
        //Get existing Pays
        this.listService.getListPays(this.adresse.Pays?.RefPays, false).subscribe(result => {
          this.paysList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Update the form
        this.updateForm();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("Libelle").disable();
    this.form.get("Adr1").disable();
    this.form.get("Adr2").disable();
    this.form.get("CodePostal").disable();
    this.form.get("Ville").disable();
    this.form.get("PaysList").disable();
    this.form.get("Horaires").disable();
    this.form.get("Cmt").disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("Libelle").setValue(this.adresse.Libelle);
    this.form.get("Adr1").setValue(this.adresse.Adr1);
    this.form.get("Adr2").setValue(this.adresse.Adr2);
    this.form.get("CodePostal").setValue(this.adresse.CodePostal);
    this.form.get("Ville").setValue(this.adresse.Ville);
    this.form.get("PaysList").setValue(this.adresse.Pays.RefPays);
    this.form.get("Horaires").setValue(this.adresse.Horaires);
    this.form.get("Cmt").setValue(this.adresse.Cmt);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.adresse.Libelle = this.form.get("Libelle").value;
    this.adresse.Adr1 = this.form.get("Adr1").value;
    this.adresse.Adr2 = this.form.get("Adr2").value;
    this.adresse.CodePostal = this.form.get("CodePostal").value;
    this.adresse.Ville = this.form.get("Ville").value;
    this.adresse.Pays = this.paysList.find(x => x.RefPays === this.form.get("PaysList").value);
    this.adresse.Horaires = this.form.get("Horaires").value;
    this.adresse.Cmt = this.form.get("Cmt").value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.applyChanges();
    //Send data
    this.dataModelService.postAdresse(this.adresse)
      .subscribe(result => {
        this.adresse = result;
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: this.refAdresse });
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.adresse);
  }
}
