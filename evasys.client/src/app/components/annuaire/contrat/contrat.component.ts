import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { DataModelService } from "../../../services/data-model.service";
import { ErrorStateMatcher } from "@angular/material/core";
import { ListService } from "../../../services/list.service";
import { showConfirmToUser, showErrorToUser } from "../../../globals/utils";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import { ApplicationComponent } from "../../administration/administration-components";
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
    selector: "contrat",
    templateUrl: "./contrat.component.html",
    styleUrls: ["./contrat.component.scss"],
    standalone: false
})

export class ContratComponent implements OnInit {
  @Input() type: string;
  @Input() contrat: dataModelsInterfaces.Contrat;
  @Input() entite: dataModelsInterfaces.Entite;
  @Output() askClose = new EventEmitter<any>();
  matcher = new MyErrorStateMatcher();
  contratTypeList: dataModelsInterfaces.ContratType[]=[];
  form: UntypedFormGroup;
  contratTypeListFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  idContratFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  dDebutFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  dFinFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  avenantFC: UntypedFormControl = new UntypedFormControl(null);
  reconductionTaciteFC: UntypedFormControl = new UntypedFormControl(null);
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  //Global lock
  locked: boolean = true;
  constructor(private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected listService: ListService
    , public dialog: MatDialog) {
    // create an empty object from the Contrat interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      ContratTypeList: this.contratTypeListFC,
      IdContrat: this.idContratFC,
      DDebut: this.dDebutFC,
      DFin: this.dFinFC,
      Avenant: this.avenantFC,
      ReconductionTacite: this.reconductionTaciteFC,
      Cmt: this.cmtFC,
    });
    if (this.applicationUserContext.connectedUtilisateur.HabilitationAnnuaire != HabilitationAnnuaire.Administrateur) {
      this.lockScreen();
    }
    else {
      this.locked = false;
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //Load initial data
    this.listService.getListContratType().subscribe(result => {
      this.contratTypeList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Update form
    this.updateForm();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.contratTypeListFC.disable();
    this.idContratFC.disable();
    this.dDebutFC.disable();
    this.dFinFC.disable();
    this.reconductionTaciteFC.disable();
    this.avenantFC.disable();
    this.cmtFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.contratTypeListFC.setValue(this.contrat.ContratType?.RefContratType);
    this.idContratFC.setValue(this.contrat.IdContrat);
    this.dDebutFC.setValue(this.contrat.DDebut);
    this.dFinFC.setValue(this.contrat.DFin);
    this.reconductionTaciteFC.setValue(this.contrat.ReconductionTacite);
    this.avenantFC.setValue(this.contrat.Avenant);
    this.cmtFC.setValue(this.contrat.Cmt);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  applyChanges() {
    this.contrat.ContratType = this.contratTypeList.find(x => x.RefContratType === this.contratTypeListFC.value);
    this.contrat.IdContrat = this.idContratFC.value;
    this.contrat.DDebut = this.dDebutFC.value;
    this.contrat.DFin = this.dFinFC.value;
    this.contrat.ReconductionTacite = this.reconductionTaciteFC.value;
    this.contrat.Avenant = this.avenantFC.value;
    this.contrat.Cmt = this.cmtFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.applyChanges();
    //Save data to current object or save to database
    switch (this.type) {
      case "Contrat":
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: this.contrat });
        break;
    }
  }
  //-----------------------------------------------------------------------------------
  //Delete a ContratEntite
  onDeleteContratEntite(refContratEntite: number) {
    this.applyChanges();
    if (refContratEntite !== null) {
      let i: number = this.contrat.ContratEntites.map(e => e.RefContratEntite).indexOf(refContratEntite);
      //Process
      showConfirmToUser(this.dialog, this.applicationUserContext.getCulturedRessourceText(300), this.applicationUserContext.getCulturedRessourceText(1126), this.applicationUserContext)
        .subscribe(result => {
          if (result === "yes") {
            //Remove ContactAdresseServiceFonction
            this.contrat.ContratEntites.splice(i, 1);
            //Update form
            this.updateForm();
          }
        });
    }
  }
  //-----------------------------------------------------------------------------------
  //Show elements details
  showDetails() {
    let data: any;
    this.applyChanges();
    //Init
    data = {
      title: this.applicationUserContext.getCulturedRessourceText(1124)
      , type: "EntiteForContrat", ref: null
      , contrat: this.contrat
    };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "50%",
      maxHeight: "80vh",
      data,
      autoFocus: false,
      restoreFocus: false
    });
    dialogRef.afterClosed().subscribe(result => {
      //If data in the dialog have been saved
      if (result) {
        let exists: boolean = false;
        //Add or update 
        let cE: dataModelsInterfaces.ContratEntite = {
          RefContrat: this.contrat.RefContrat,
          RefEntite: result.ref,
          Entite: {RefEntite:result.ref, Libelle:result.libelle, Actif:true},
        };
        exists = this.contrat.ContratEntites?.some(item => item.RefEntite == cE.RefEntite);
        if (!exists) {
          if (this.contrat.ContratEntites == null) this.contrat.ContratEntites = [];
          this.contrat.ContratEntites.push(cE);
        }
      }
    });
  }
}
