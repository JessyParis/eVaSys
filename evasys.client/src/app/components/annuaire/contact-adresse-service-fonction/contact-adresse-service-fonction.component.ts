import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { ErrorStateMatcher } from "@angular/material/core";
import { showErrorToUser, showInformationToUser } from "../../../globals/utils";

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
    selector: "contact-adresse-service-fonction",
    templateUrl: "./contact-adresse-service-fonction.component.html",
    standalone: false
})

export class ContactAdresseServiceFonctionComponent implements OnInit {
  @Input() refContactAdresseServiceFonction: number;
  @Input() type: string;
  @Input() cASF: dataModelsInterfaces.ContactAdresseServiceFonction;
  @Input() contactAdresse: dataModelsInterfaces.ContactAdresse;
  @Output() askClose = new EventEmitter<any>();
  @Output() title : string;
  matcher = new MyErrorStateMatcher();
  contactAdresseServiceFonction: dataModelsInterfaces.ContactAdresseServiceFonction;
  form: UntypedFormGroup;
  serviceList: dataModelsInterfaces.Service[];
  fonctionList: dataModelsInterfaces.Fonction[];
  //Global lock
  locked: boolean = true;
  constructor(private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , public dialog: MatDialog) {
    // create an empty object from the ContactAdresseServiceFonction interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: [true],
      ServiceList: [null],
      FonctionList: [null],
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
    this.contactAdresseServiceFonction = { ...this.cASF };
    //Get existing Service
    this.listService.getListService().subscribe(result => {
      this.serviceList = result;
      //Auto select
      if (this.serviceList.length === 1) { this.form.get("ServiceList").setValue(this.serviceList[0].RefService); }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Fonction
    this.listService.getListFonction().subscribe(result => {
      this.fonctionList = result;
      //Auto select
      if (this.fonctionList.length === 1) { this.form.get("FonctionList").setValue(this.fonctionList[0].RefFonction); }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("ServiceList").disable();
    this.form.get("FonctionList").disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("ServiceList").setValue(this.contactAdresseServiceFonction?.Service?.RefService);
    this.form.get("FonctionList").setValue(this.contactAdresseServiceFonction?.Fonction?.RefFonction);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.contactAdresseServiceFonction.Service = this.serviceList.find(x => x.RefService === this.form.get("ServiceList").value);
    this.contactAdresseServiceFonction.Fonction = this.fonctionList.find(x => x.RefFonction === this.form.get("FonctionList").value);
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    //Save data to current object 
    this.applyChanges();
    //Check duplicates
    if (this.contactAdresse.ContactAdresseServiceFonctions?.some(item =>
      item.RefContactAdresseServiceFonction != this.contactAdresseServiceFonction.RefContactAdresseServiceFonction
      && (item.Service?.RefService == this.contactAdresseServiceFonction.Service?.RefService
        && item.Fonction?.RefFonction == this.contactAdresseServiceFonction.Fonction?.RefFonction)
    )) {
      showInformationToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(115), this.applicationUserContext.getCulturedRessourceText(410), this.applicationUserContext);
    }
    else {
      //Save data
      this.cASF = { ...this.contactAdresseServiceFonction };
      //Emit event for dialog closing
      this.askClose.emit({ type: this.type, ref: this.cASF });
    }
  }
}
