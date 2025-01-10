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
  //Delete a ContactAdresseServiceFonction
  onDeleteContactAdresseServiceFonction(refContratEntite: number) {
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
  showDetails(type: string, ref: number, refEntiteType: number, mode: string) {
    let data: any;
    let title: string = this.applicationUserContext.getCulturedRessourceText(349);
    this.applyChanges();
    //Init
    switch (type) {
      case "AnnuaireContratEntite":
        //Calculate id
        let minRefContratEntite: number = -1;
        this.contrat.ContratEntites?.forEach(item => {
          if (item.RefContratEntite <= minRefContratEntite) minRefContratEntite = item.RefContratEntite - 1;
        });
        //Get ContactAdresseServiceFonction
        let cE = {
          RefContratEntite: minRefContratEntite
          , RefContrat: this.contrat.RefContrat
        } as dataModelsInterfaces.ContratEntite
        if (ref != null) { cE = this.contrat.ContratEntites.find(x => x.RefContratEntite === ref) }
        data = {
          title: this.applicationUserContext.getCulturedRessourceText(1124)
          , type: type, ref: ref
          , contrat: this.contrat
          , cE: cE
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
          case "AnnuaireContratEntite":
            //Add or update ContactAdresseServiceFonction
            let cE: dataModelsInterfaces.ContratEntite = result.ref as dataModelsInterfaces.ContratEntite;
            exists = this.contrat.ContratEntites?.some(item => item.RefContratEntite == cE.RefContratEntite);
            if (!exists) { this.contrat.ContratEntites.push(cE); }
            else {
              let ind: number = this.contrat.ContratEntites.map(e => e.RefContratEntite).indexOf(cE.RefContratEntite);
              this.contrat.ContratEntites[ind] = cE;
            }
            break;
        }
      }
    });
  }
}
