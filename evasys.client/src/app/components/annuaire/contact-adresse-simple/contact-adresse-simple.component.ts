import { Component, Inject, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { HabilitationLogistique, SnackbarMsgType } from "../../../globals/enums";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
@Component({
    selector: "contact-adresse-simple",
    templateUrl: "./contact-adresse-simple.component.html",
    standalone: false
})

export class ContactAdresseSimpleComponent implements OnInit {
  @Input() refContactAdresse: number;
  @Input() type: string;
  @Output() askClose = new EventEmitter<any>();
  contactAdresse: dataModelsInterfaces.ContactAdresse;
  form: UntypedFormGroup;
  civiliteList: dataModelsInterfaces.Civilite[];
  //Global lock
  locked: boolean = true;
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.contactAdresse = {} as dataModelsInterfaces.ContactAdresse;
    // create an empty object from the ContactAdresse interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Civilite: [null],
      Prenom: [null],
      Nom: [null, Validators.required],
      Email: [null, Validators.email],
      Tel: [null],
      TelMobile: [null],
      Fax: [null],
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
    //Get existing Civilite
    this.listService.getListCivilite().subscribe(result => {
      this.civiliteList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get ContactAdresse
    if (this.refContactAdresse) {
      var url = this.baseUrl + "evapi/contactadresse/" + this.refContactAdresse;
      this.http.get<dataModelsInterfaces.ContactAdresse>(url).subscribe(result => {
        this.contactAdresse = result;
        //Update the form
        this.updateForm();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("Civilite").disable();
    this.form.get("Prenom").disable();
    this.form.get("Nom").disable();
    this.form.get("Email").disable();
    this.form.get("Tel").disable();
    this.form.get("TelMobile").disable();
    this.form.get("Fax").disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("Civilite").setValue(this.contactAdresse.Contact.Civilite?.RefCivilite);
    this.form.get("Prenom").setValue(this.contactAdresse.Contact.Prenom);
    this.form.get("Nom").setValue(this.contactAdresse.Contact.Nom, Validators.required);
    this.form.get("Email").setValue(this.contactAdresse.Email);
    this.form.get("Tel").setValue(this.contactAdresse.Tel);
    this.form.get("TelMobile").setValue(this.contactAdresse.TelMobile);
    this.form.get("Fax").setValue(this.contactAdresse.Fax);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.contactAdresse.Contact.RefCivilite = this.form.get("Civilite").value;
    this.contactAdresse.Contact.Prenom = this.form.get("Prenom").value;
    this.contactAdresse.Contact.Nom = this.form.get("Nom").value;
    this.contactAdresse.Email = this.form.get("Email").value;
    this.contactAdresse.Tel = this.form.get("Tel").value;
    this.contactAdresse.TelMobile = this.form.get("TelMobile").value;
    this.contactAdresse.Fax = this.form.get("Fax").value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.applyChanges();
    var url = this.baseUrl + "evapi/contactadresse";
    //Send data
    this.http
      .post<dataModelsInterfaces.ContactAdresse>(url, this.contactAdresse)
      .subscribe(result => {
        this.contactAdresse = result;
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: this.refContactAdresse });
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.contactAdresse);
  }
}
