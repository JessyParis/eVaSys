import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, UntypedFormArray } from "@angular/forms";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ContactAdresseProcess, HabilitationQualite } from "../../../globals/enums";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { showErrorToUser, cmp, getCreationModificationTooltipText } from "../../../globals/utils";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";

class MyErrorStateMatcher implements ErrorStateMatcher {
  //Constructor
  constructor(public applicationUserContext: ApplicationUserContext) { }
  isErrorState(control: UntypedFormControl | null): boolean {
    let result: boolean = false;
    result = !!((control && control.invalid));
    return result;
  }
}

@Component({
    selector: "cvq",
    templateUrl: "./cvq.component.html",
    styleUrls: ["./cvq.component.scss"],
    standalone: false
})

export class CVQComponent implements OnInit {
  matcher = new MyErrorStateMatcher(this.applicationUserContext);
  //Variables
  cVQ: dataModelsInterfaces.CVQ = {} as dataModelsInterfaces.CVQ;
  //Form
  form: UntypedFormGroup;
  cVQDescriptionCVQs: UntypedFormArray = new UntypedFormArray([]);
  cVQControleurList: dataModelsInterfaces.ContactAdresse[];
  cVQControleurListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  dCVQFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  //Entries
  @Input() refCVQ: string;
  @Input() refClient: string;
  @Input() refProduit: string;
  @Input() refFicheControle: string;
  @Input() gUID: string;
  @Output() saveResult: EventEmitter<string> = new EventEmitter();  
  @Output() deleteResult: EventEmitter<string> = new EventEmitter();  
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = true;
  //Constructor
  constructor(private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      CVQControleurList: this.cVQControleurListFC,
      DCVQ: this.dCVQFC,
      Cmt: this.cmtFC,
      CVQDescriptionCVQs: this.cVQDescriptionCVQs,
    });
    this.lockScreen();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.cVQControleurListFC.disable();
    this.dCVQFC.disable();
    this.cmtFC.disable();
    this.cVQDescriptionCVQs.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.cVQControleurListFC.enable();
    this.dCVQFC.enable();
    this.cmtFC.enable();
    this.cVQDescriptionCVQs.enable();
  }
  //-----------------------------------------------------------------------------------
  //Manage screen
  manageScreen() {
    //Global lock
    if (this.applicationUserContext.connectedUtilisateur.HabilitationQualite !== HabilitationQualite.Administrateur
      && this.applicationUserContext.connectedUtilisateur.HabilitationQualite !== HabilitationQualite.Client) {
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
    let id = Number.parseInt(this.refCVQ, 10);
    let refClient = Number.parseInt(this.refClient, 10);
    let refProduit = Number.parseInt(this.refProduit, 10);
    let refFicheControle = Number.parseInt(this.refFicheControle, 10);
    //Check parameters
    if (!isNaN(id) && !isNaN(refClient) && !isNaN(refProduit)) {
      this.dataModelService.getCVQ(id, refProduit).subscribe(result => {
        //Get data
        this.cVQ = result;
        //Attach to FicheControle if needed
        if (this.cVQ.RefFicheControle == null) { this.cVQ.RefFicheControle = refFicheControle; }
        //Sort cVQ.CVQDescriptionCVQs
        var sortedArray: dataModelsInterfaces.CVQDescriptionCVQ[] = this.cVQ.CVQDescriptionCVQs.sort(function (a, b) {
          return cmp(a.Ordre, b.Ordre);
        });
        this.cVQ.CVQDescriptionCVQs = sortedArray;
        //Update form
        this.updateForm();
        //Get existing Controleur
        this.listService.getListContactAdresse(this.cVQ.Controleur?.RefContactAdresse, refClient, null, ContactAdresseProcess.ControleurQualite, true)
          .subscribe(result => {
            this.cVQControleurList = result;
            //Auto select
            if (this.cVQControleurList.length === 1) { this.cVQControleurListFC.setValue(this.cVQControleurList[0].RefContactAdresse); }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      showErrorToUser(this.dialog, { error: { error: { message: this.applicationUserContext.getCulturedRessourceText(712) } } }, this.applicationUserContext);
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.cVQControleurListFC.setValue(this.cVQ.Controleur ? this.cVQ.Controleur.RefContactAdresse : null);
    this.dCVQFC.setValue(this.cVQ.DCVQ);
    this.cmtFC.setValue(this.cVQ.Cmt);
    for (const dC of this.cVQ.CVQDescriptionCVQs) {
      const grp = this.fb.group({
        Nb: [(!dC.Nb ? null : dC.Nb.toString()), Validators.max(100)],
      });
      this.cVQDescriptionCVQs.push(grp);
    }
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.cVQ.Controleur = (this.cVQControleurList ? this.cVQControleurList.find(x => x.RefContactAdresse === this.cVQControleurListFC.value) : null);
    this.cVQ.DCVQ = this.dCVQFC.value;
    this.cVQ.Cmt = this.cmtFC.value;
    //CVQDescriptionCVQs
    for (let i in this.cVQDescriptionCVQs.controls) {
      let fG: UntypedFormGroup = this.cVQDescriptionCVQs.controls[i] as UntypedFormGroup;
      this.cVQ.CVQDescriptionCVQs[i].Nb = (fG.get("Nb").value == null ? 0 : fG.get("Nb").value);
    }
  }
  //-----------------------------------------------------------------------------------
  //Calculate Quality
  getQuality(nb: number, limiteBasse: number, limiteHaute: number): number {
    let r: number = null;
    if (nb == null) { nb = 0; }
    if (nb <= limiteBasse) { r = 1; }
    else if (nb > limiteHaute) { r = 3; }
    else { r = 2; }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Get Quality css
  getQualityCss(nb: number, limiteBasse: number, limiteHaute: number): string {
    let r: string = "";
    if (nb == null) { nb = 0; }
    if (nb <= limiteBasse) { r = "ev-color-green"; }
    else if (nb > limiteHaute) { r = "ev-color-red"; }
    else { r = "ev-color-orange"; }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Get data for IHM
  get getFormData(): UntypedFormArray {
    return this.cVQDescriptionCVQs as UntypedFormArray;
  }
  //-----------------------------------------------------------------------------------
  //Show ContactAdresse details
  showDetails(type: string, ref: number) {
    let refClient = Number.parseInt(this.refClient, 10);
    let data: any;
    //Init
    this.saveData();
    switch (type) {
      case "CVQControleur":
        data = { title: this.applicationUserContext.getCulturedRessourceText(487), type: type, ref: ref };
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
        switch (result.type) {
          case "CVQControleur":
            //Get existing Controleur
            this.listService.getListContactAdresse(this.cVQ.Controleur?.RefContactAdresse, refClient, null, ContactAdresseProcess.ControleurQualite, true)
              .subscribe(result => {
                this.cVQControleurList = result;
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
        }
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onDistantSave() {
    this.saveData();
    //Process
    this.dataModelService.postCVQ(this.cVQ)
      .subscribe(result => {
        //back to pristine
        this.form.markAsPristine();
        this.saveResult.emit(this.gUID);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(116) },
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    if (this.cVQ.RefCVQ > 0) {
      this.dataModelService.deleteCVQ(this.cVQ)
        .subscribe(result => {
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(752), duration: 4000 } as appInterfaces.SnackbarMsg);
          //Inform parent
          this.deleteResult.emit(this.gUID);
        }, error => {
          showErrorToUser(this.dialog, error, this.applicationUserContext);
        });
    }
    else {
      this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(752), duration: 4000 } as appInterfaces.SnackbarMsg);
      //Inform parent
      this.deleteResult.emit(this.gUID);
    }
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.cVQ);
  }
}
