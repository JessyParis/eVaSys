import { Component, Inject, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { getCreationModificationTooltipText, showConfirmToUser, showErrorToUser, GetContactAdresseProcessTooltipRessourceId, showInformationToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import { ErrorStateMatcher } from "@angular/material/core";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";

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
    selector: "contact-adresse",
    templateUrl: "./contact-adresse.component.html",
    styleUrls: ["./contact-adresse.component.scss"],
    standalone: false
})

export class ContactAdresseComponent implements OnInit {
  @Input() refContactAdresse: number;
  @Input() type: string;
  @Input() cA: dataModelsInterfaces.ContactAdresse;
  @Input() entite: dataModelsInterfaces.Entite;
  @Input() adresseList: dataModelsInterfaces.Adresse[];
  @Output() askClose = new EventEmitter<any>();
  matcher = new MyErrorStateMatcher();
  form: UntypedFormGroup;
  contactAdresse: dataModelsInterfaces.ContactAdresse;
  civiliteList: dataModelsInterfaces.Civilite[];
  titreList: dataModelsInterfaces.Titre[];
  contactAdresseContactAdresseProcessList: dataModelsInterfaces.ContactAdresseContactAdresseProcess[] = [];
  //Validation
  adresseRequired: boolean = true;
  //Global lock
  locked: boolean = true;
  //Functions
  GetContactAdresseProcessTooltipRessourceId = GetContactAdresseProcessTooltipRessourceId;
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    // create an empty object from the ContactAdresse interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: [true],
      AdresseList: [null, Validators.required],
      CiviliteList: [null],
      TitreList: [null],
      Prenom: [null],
      Nom: [null, Validators.required],
      Email: [null, Validators.email],
      Tel: [null],
      TelMobile: [null],
      Cmt: [null],
      CmtServiceFonction: [null],
      ContactAdresseProcessList: [null],
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
    this.contactAdresse = { ...this.cA };
    //Get lists
    this.listService.getListCivilite().subscribe(result => {
      this.civiliteList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListTitre().subscribe(result => {
      this.titreList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.listService.getListContactAdresseProcess().subscribe(result => {
      this.contactAdresseContactAdresseProcessList = result.map(item => ({ ContactAdresseProcess: item } as dataModelsInterfaces.ContactAdresseContactAdresseProcess));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Update form
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("Actif").disable();
    this.form.get("AdresseList").disable();
    this.form.get("CiviliteList").disable();
    this.form.get("TitreList").disable();
    this.form.get("Prenom").disable();
    this.form.get("Nom").disable();
    this.form.get("Email").disable();
    this.form.get("Tel").disable();
    this.form.get("TelMobile").disable();
    this.form.get("Cmt").disable();
    this.form.get("CmtServiceFonction").disable();
    this.form.get("ContactAdresseProcessList").disable();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  ManageScreen() {
    this.adresseRequired = false;
    this.form.get("AdresseList").clearValidators();
    this.form.get("CmtServiceFonction").enable();
    if (this.form.get("ContactAdresseProcessList").value.length > 0 ) {
      this.adresseRequired = true;
      this.form.get("AdresseList").addValidators(Validators.required);
    }
    this.form.get("AdresseList").updateValueAndValidity();
    if (this.contactAdresse.ContactAdresseServiceFonctions.length == 0) {
      this.form.get("CmtServiceFonction").disable();
      this.form.get("CmtServiceFonction").setValue(null);
    }
    this.form.get("CmtServiceFonction").updateValueAndValidity();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("Actif").setValue(this.contactAdresse.Actif);
    this.form.get("AdresseList").setValue(this.contactAdresse.RefAdresse);
    this.form.get("CiviliteList").setValue(this.contactAdresse.Contact.Civilite?.RefCivilite);
    this.form.get("TitreList").setValue(this.contactAdresse.Titre?.RefTitre);
    this.form.get("Prenom").setValue(this.contactAdresse.Contact.Prenom);
    this.form.get("Nom").setValue(this.contactAdresse.Contact.Nom, Validators.required);
    this.form.get("Email").setValue(this.contactAdresse.Email);
    this.form.get("Tel").setValue(this.contactAdresse.Tel);
    this.form.get("TelMobile").setValue(this.contactAdresse.TelMobile);
    this.form.get("Cmt").setValue(this.contactAdresse.Cmt);
    this.form.get("CmtServiceFonction").setValue(this.contactAdresse.CmtServiceFonction);
    this.form.get("ContactAdresseProcessList").setValue(this.contactAdresse.ContactAdresseContactAdresseProcesss);
    //Manage screen
    this.ManageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.contactAdresse.Actif = this.form.get("Actif").value;
    this.contactAdresse.RefAdresse = this.form.get("AdresseList").value;
    this.contactAdresse.Contact.Civilite = this.civiliteList.find(x => x.RefCivilite === this.form.get("CiviliteList").value);
    this.contactAdresse.Titre = this.titreList.find(x => x.RefTitre === this.form.get("TitreList").value);
    this.contactAdresse.Contact.Prenom = this.form.get("Prenom").value;
    this.contactAdresse.Contact.Nom = this.form.get("Nom").value;
    this.contactAdresse.Email = this.form.get("Email").value;
    this.contactAdresse.Tel = this.form.get("Tel").value;
    this.contactAdresse.TelMobile = this.form.get("TelMobile").value;
    this.contactAdresse.Cmt = this.form.get("Cmt").value;
    this.contactAdresse.CmtServiceFonction = this.form.get("CmtServiceFonction").value;
    this.contactAdresse.ContactAdresseContactAdresseProcesss = this.form.get("ContactAdresseProcessList").value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.applyChanges();
    //Save data to current object or save to database
    switch (this.type) {
      case "AnnuaireContactAdresse":
        //Check data validity
        if (!this.contactAdresse.RefAdresse && this.contactAdresse.ContactAdresseContactAdresseProcesss?.length > 0) {
          showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(115), this.applicationUserContext.getCulturedRessourceText(1478), this.applicationUserContext);
        }
        else {
          //Save data
          this.cA = { ...this.contactAdresse };
          //Emit event for dialog closing
          this.askClose.emit({ type: this.type, ref: this.cA });
        }
        break;
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a ContactAdresseServiceFonction
  onDeleteContactAdresseServiceFonction(refContactAdresseServiceFonction: number) {
    this.applyChanges();
    if (refContactAdresseServiceFonction !== null) {
      let i: number = this.contactAdresse.ContactAdresseServiceFonctions.map(e => e.RefContactAdresseServiceFonction).indexOf(refContactAdresseServiceFonction);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1187), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Remove ContactAdresseServiceFonction
            this.contactAdresse.ContactAdresseServiceFonctions.splice(i, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Show elements details
  showDetails(type: string, ref: number, refEntiteType: number, mode: string) {
    let data: any;
    let title: string = this.applicationUserContext.getCulturedRessourceText(349);
    this.applyChanges();
    //Init
    switch (type) {
      case "AnnuaireContactAdresseServiceFonction":
        //Calculate id
        let minRefContactAdresseServiceFonction: number = -1;
        this.contactAdresse.ContactAdresseServiceFonctions?.forEach(item => {
          if (item.RefContactAdresseServiceFonction <= minRefContactAdresseServiceFonction) minRefContactAdresseServiceFonction = item.RefContactAdresseServiceFonction - 1;
        });
        //Get ContactAdresseServiceFonction
        let cASF = {
          RefContactAdresseServiceFonction: minRefContactAdresseServiceFonction
          , RefContactAdresse: this.contactAdresse.RefContactAdresse
        } as dataModelsInterfaces.ContactAdresseServiceFonction
        if (ref != null) { cASF = this.contactAdresse.ContactAdresseServiceFonctions.find(x => x.RefContactAdresseServiceFonction === ref) }
        data = {
          title: this.applicationUserContext.getCulturedRessourceText(1146)
          , type: type, ref: ref
          , contactAdresse: this.contactAdresse, cASF: cASF
        };
        break;
    }
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "350px",
      data,
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      //If data in the dialog have been saved
      if (result) {
        let exists: boolean = false;
        switch (result.type) {
          case "AnnuaireContactAdresseServiceFonction":
            //Add or update ContactAdresseServiceFonction
            let cASF: dataModelsInterfaces.ContactAdresseServiceFonction = result.ref as dataModelsInterfaces.ContactAdresseServiceFonction;
            exists = this.contactAdresse.ContactAdresseServiceFonctions?.some(item => item.RefContactAdresseServiceFonction == cASF.RefContactAdresseServiceFonction);
            if (!exists) { this.contactAdresse.ContactAdresseServiceFonctions.push(cASF); }
            else {
              let ind: number = this.contactAdresse.ContactAdresseServiceFonctions.map(e => e.RefContactAdresseServiceFonction).indexOf(cASF.RefContactAdresseServiceFonction);
              this.contactAdresse.ContactAdresseServiceFonctions[ind] = cASF;
            }
            //Manage screen
            this.ManageScreen()
            break;
        }
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Multi select
  compareFnContactAdresseProcess(item: dataModelsInterfaces.ContactAdresseContactAdresseProcess, selectedItem: dataModelsInterfaces.ContactAdresseContactAdresseProcess) {
    return item && selectedItem ? item.ContactAdresseProcess.RefContactAdresseProcess === selectedItem.ContactAdresseProcess.RefContactAdresseProcess : item === selectedItem;
  }
  public onContactAdresseProcessListReset() {
    this.contactAdresse.ContactAdresseContactAdresseProcesss = [];
    this.form.get("ContactAdresseProcessList").setValue(this.contactAdresse.ContactAdresseContactAdresseProcesss);
    this.ManageScreen();
  }
  onContactAdresseProcessSelected() {
    this.ManageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Create ContratCollectivite label
  contactAdresseServiceFonctionLabel(cASF: dataModelsInterfaces.ContactAdresseServiceFonction): string {
    let s = "";
    if (cASF.Fonction) {
      s += cASF.Fonction.Libelle;
    }
    if (cASF.Service && cASF.Fonction) {
      s += " - " ;
    }
    if (cASF.Service) {
      s += cASF.Service.Libelle;
    }
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.contactAdresse);
  }
}
