import { Component, Inject, OnInit, Input } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { getCreationModificationTooltipText, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { SnackbarMsgType } from "../../../globals/enums";

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
    selector: "description-cvq",
    templateUrl: "./description-cvq.component.html",
    standalone: false
})

export class DescriptionCVQComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  descriptionCVQ: dataModelsInterfaces.DescriptionCVQ = {} as dataModelsInterfaces.DescriptionCVQ;
  ordreFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(100), Validators.min(0)]);
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  calculLimiteConformiteFC: UntypedFormControl = new UntypedFormControl(null);
  libelleFRFRFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  libelleENGBFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  limiteBasseFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(100), Validators.min(0)]);
  limiteHauteFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(100), Validators.min(0)]);
  //Global lock
  locked: boolean = false;
  saveLocked: boolean = false;
  //Entries
  @Input() refDescriptionCVQ: string;
  @Input() refProduit: string;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Ordre: this.ordreFC,
      Actif: this.actifFC,
      CalculLimiteConformite: this.calculLimiteConformiteFC,
      LibelleFRFR: this.libelleFRFRFC,
      LibelleENGB: this.libelleENGBFC,
      LimiteBasse: this.limiteBasseFC,
      LimiteHaute: this.limiteHauteFC
    });
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.ordreFC.disable();
    this.actifFC.disable();
    this.calculLimiteConformiteFC.disable();
    this.libelleFRFRFC.disable();
    this.libelleENGBFC.disable();
    this.limiteBasseFC.disable();
    this.limiteHauteFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Global lock
    if (1 !== 1) {
      this.lockScreen();
    }
    else {
      //Init
      this.unlockScreen();
    }
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.refDescriptionCVQ, 10);
    //Load initial data
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getDescriptionCVQ(id).subscribe(result => {
          //Get data
          this.descriptionCVQ = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
      else {
        let refP = Number.parseInt(this.refProduit, 10);
        if (!isNaN(refP)) {
          if (refP != 0) {
            //Add new element
            this.descriptionCVQ = {
              RefDescriptionCVQ: 0
              , DescriptionCVQProduits: [{
                Produit: { RefProduit: refP }
              } as dataModelsInterfaces.DescriptionCVQProduit]
            } as dataModelsInterfaces.DescriptionCVQ
          }
        }
      }
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.ordreFC.setValue(this.descriptionCVQ.Ordre);
    this.actifFC.setValue(this.descriptionCVQ.Actif);
    this.libelleFRFRFC.setValue(this.descriptionCVQ.LibelleFRFR);
    this.libelleENGBFC.setValue(this.descriptionCVQ.LibelleENGB);
    this.limiteBasseFC.setValue(this.descriptionCVQ.LimiteBasse);
    this.limiteHauteFC.setValue(this.descriptionCVQ.LimiteHaute);
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.descriptionCVQ.Ordre = this.ordreFC.value;
    this.descriptionCVQ.Actif = this.actifFC.value;
    this.descriptionCVQ.LibelleFRFR = this.libelleFRFRFC.value;
    this.descriptionCVQ.LibelleENGB = this.libelleENGBFC.value;
    this.descriptionCVQ.LimiteBasse = this.limiteBasseFC.value;
    this.descriptionCVQ.LimiteHaute = this.limiteHauteFC.value;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Process
    var url = this.baseUrl + "evapi/descriptioncvq";
    //Update 
    this.http
      .post<dataModelsInterfaces.DescriptionCVQ>(url, this.descriptionCVQ)
      .subscribe(result => {
        //Inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000, type: SnackbarMsgType.Success } as appInterfaces.SnackbarMsg);
        //back to pristine
        this.form.markAsPristine();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(754) },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    var url = this.baseUrl + "evapi/descriptioncvq/" + this.descriptionCVQ.RefDescriptionCVQ;
    this.http
      .delete(url)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(751), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Lock
        this.lockScreen();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.descriptionCVQ);
  }
}
