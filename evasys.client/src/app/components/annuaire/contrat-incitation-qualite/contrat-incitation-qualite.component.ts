import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
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
    selector: "contrat-incitation-qualite",
    templateUrl: "./contrat-incitation-qualite.component.html",
    standalone: false
})

export class ContratIncitationQualiteComponent implements OnInit {
  @Input() type: string;
  @Input() contratIncitationQualite: dataModelsInterfaces.ContratIncitationQualite;
  @Input() entite: dataModelsInterfaces.Entite;
  @Output() askClose = new EventEmitter<any>();
  matcher = new MyErrorStateMatcher();
  form: UntypedFormGroup;
  //Global lock
  locked: boolean = true;
  constructor(private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , public dialog: MatDialog) {
    // create an empty object from the ContratIncitationQualite interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      DDebut: [null, Validators.required],
      DFin: [null, Validators.required],
      Avenant: [true],
      DDebutAvenant: [null],
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
    //Update form
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.form.get("DDebut").disable();
    this.form.get("DFin").disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.form.get("DDebut").setValue(this.contratIncitationQualite.DDebut);
    this.form.get("DFin").setValue(this.contratIncitationQualite.DFin);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.contratIncitationQualite.DDebut = this.form.get("DDebut").value;
    this.contratIncitationQualite.DFin = this.form.get("DFin").value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.applyChanges();
    //Save data to current object or save to database
    switch (this.type) {
      case "ContratIncitationQualite":
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: this.contratIncitationQualite });
        break;
    }
  }
}
