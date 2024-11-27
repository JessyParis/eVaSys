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
import { showErrorToUser, cmp, toFixed, getCreationModificationTooltipText } from "../../../globals/utils";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";

class MyErrorStateMatcher implements ErrorStateMatcher {
  //Constructor
  constructor(public applicationUserContext: ApplicationUserContext) { }
  isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    let result: boolean = false;
    result = !!((control && control.invalid));
    return result;
  }
}

@Component({
    selector: "controle",
    templateUrl: "./controle.component.html",
    styleUrls: ["./controle.component.scss"],
    standalone: false
})

export class ControleComponent implements OnInit {
  matcher = new MyErrorStateMatcher(this.applicationUserContext);
  //Functions
  toFixed = toFixed;
  //Variables
  controle: dataModelsInterfaces.Controle = {} as dataModelsInterfaces.Controle;
  //Form
  form: UntypedFormGroup;
  controleDescriptionControles: UntypedFormArray = new UntypedFormArray([]);
  controleControleurList: dataModelsInterfaces.ContactAdresse[];
  controleControleurListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  dControleFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  poidsFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.max(1000)]);
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  //Entries
  @Input() refControle: string;
  @Input() refClient: string;
  @Input() refProduit: string;
  @Input() refFicheControle: string;
  @Input() gUID: string;
  @Input() poidsFicheControle: string;
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
      ControleControleurList: this.controleControleurListFC,
      DControle: this.dControleFC,
      Poids: this.poidsFC,
      Cmt: this.cmtFC,
      ControleDescriptionControles: this.controleDescriptionControles,
    });
    this.lockScreen();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.controleControleurListFC.disable();
    this.dControleFC.disable();
    this.poidsFC.disable();
    this.cmtFC.disable();
    this.controleDescriptionControles.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.controleControleurListFC.enable();
    this.poidsFC.enable();
    this.dControleFC.enable();
    this.cmtFC.enable();
    this.controleDescriptionControles.enable();
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
    let id = Number.parseInt(this.refControle, 10);
    let refClient = Number.parseInt(this.refClient, 10);
    let refProduit = Number.parseInt(this.refProduit, 10);
    let refFicheControle = Number.parseInt(this.refFicheControle, 10);
    //Check parameters
    if (!isNaN(id) && !isNaN(refClient) && !isNaN(refProduit)) {
      this.dataModelService.getControle(id, refProduit).subscribe(result => {
        //Get data
        this.controle = result;
        //Attach to FicheControle if needed
        if (this.controle.RefFicheControle == null) { this.controle.RefFicheControle = refFicheControle; }
        //Sort controle.ControleDescriptionControles
        var sortedArray: dataModelsInterfaces.ControleDescriptionControle[] = this.controle.ControleDescriptionControles.sort(function (a, b) {
          return cmp(a.Ordre, b.Ordre);
        });
        this.controle.ControleDescriptionControles = sortedArray;
        //Update form
        this.updateForm();
        //Get existing Controleur
        this.listService.getListContactAdresse(this.controle?.Controleur?.RefContactAdresse, refClient, null, ContactAdresseProcess.ControleurQualite, true)
          .subscribe(result => {
            this.controleControleurList = result;
            //Auto select
            if (this.controleControleurList.length === 1) { this.controleControleurListFC.setValue(this.controleControleurList[0].RefContactAdresse); }
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
    this.controleControleurListFC.setValue(this.controle.Controleur ? this.controle.Controleur.RefContactAdresse : null);
    this.dControleFC.setValue(this.controle.DControle);
    this.poidsFC.setValue(this.controle.Poids);
    let p = Number.parseInt(this.poidsFicheControle, 10)
    if (!isNaN(p)) {
      if (p > 0) { this.poidsFC.setValidators([Validators.required, Validators.max(p)]); }
    }
    this.cmtFC.setValue(this.controle.Cmt);
    for (const dC of this.controle.ControleDescriptionControles) {
      const grp = this.fb.group({
        Poids: (!dC.Poids ? null : dC.Poids.toString()),
      });
      this.controleDescriptionControles.push(grp);
    }
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.controle.Controleur = (this.controleControleurList ? this.controleControleurList.find(x => x.RefContactAdresse === this.controleControleurListFC.value) : null);
    this.controle.DControle = this.dControleFC.value;
    this.controle.Poids = this.poidsFC.value;
    this.controle.Cmt = this.cmtFC.value;
    //ControleDescriptionControles
    for (let i in this.controleDescriptionControles.controls) {
      let fG: UntypedFormGroup = this.controleDescriptionControles.controls[i] as UntypedFormGroup;
      this.controle.ControleDescriptionControles[i].Poids = (fG.get("Poids").value == null ? 0 : fG.get("Poids").value);
    }
  }
  //-----------------------------------------------------------------------------------
  //Get data for IHM
  get getFormData(): UntypedFormArray {
    return this.controleDescriptionControles as UntypedFormArray;
  }
  //-----------------------------------------------------------------------------------
  //Show ContactAdresse details
  showDetails(type: string, ref: number) {
    let refClient = Number.parseInt(this.refClient, 10);
    let data: any;
    //Init
    this.saveData();
    switch (type) {
      case "ControleControleur":
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
          case "ControleControleur":
            //Get existing Controleur
            this.listService.getListContactAdresse(this.controle?.Controleur?.RefContactAdresse, refClient, null, ContactAdresseProcess.ControleurQualite, true)
              .subscribe(result => {
                this.controleControleurList = result;
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
        }
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Get sum of Poids
  getSumPoids(): number {
    let r: number = 0;
    for (let i in this.controleDescriptionControles.controls) {
      let fG: UntypedFormGroup = this.controleDescriptionControles.controls[i] as UntypedFormGroup;
      if (fG.get("Poids").value) {
        r += Number.parseFloat(fG.get("Poids").value);
      }
    }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Get sum of Poids with CalculLimiteConformite
  getSumPoidsCalculLimiteConformite(): number {
    let r: number = 0;
    for (let i in this.controleDescriptionControles.controls) {
      let fG: UntypedFormGroup = this.controleDescriptionControles.controls[i] as UntypedFormGroup;
      if (fG.get("Poids").value && this.controle.ControleDescriptionControles[i].CalculLimiteConformite) {
        r += Number.parseFloat(fG.get("Poids").value);
      }
    }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Get sum of recentages
  getSumPercentage(): number {
    let r: number = 0;
    for (let i in this.controleDescriptionControles.controls) {
      let fG: UntypedFormGroup = this.controleDescriptionControles.controls[i] as UntypedFormGroup;
      if (fG.get("Poids").value && this.poidsFC.value && this.controle.ControleDescriptionControles[i].CalculLimiteConformite) {
        //r += Number.parseFloat(((fG.get("Poids").value / this.poidsFC.value)*100).toFixed(2));
        r += Number.parseFloat(toFixed(((fG.get("Poids").value / this.poidsFC.value)*100), 2));
      }
    }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onDistantSave() {
    this.saveData();
    //Process
    this.dataModelService.postControle(this.controle)
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
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(108) },
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    if (this.controle.RefControle > 0) {
      this.dataModelService.deleteControle(this.controle)
        .subscribe(result => {
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(750), duration: 4000 } as appInterfaces.SnackbarMsg);
          //Inform parent
          this.deleteResult.emit(this.gUID);
        }, error => {
          showErrorToUser(this.dialog, error, this.applicationUserContext);
        });
    }
    else {
      this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(750), duration: 4000 } as appInterfaces.SnackbarMsg);
      //Inform parent
      this.deleteResult.emit(this.gUID);
    }
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.controle);
  }
}
