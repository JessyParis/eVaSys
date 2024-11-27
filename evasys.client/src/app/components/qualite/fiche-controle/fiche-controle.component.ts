import { Component, Inject, OnInit, ViewChildren, QueryList, ViewChild, OnDestroy } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, UntypedFormArray } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher, ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ContactAdresseProcess, ModuleName, MenuName, HabilitationQualite } from "../../../globals/enums";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import { ComponentRelativeComponent } from "../../dialogs/component-relative/component-relative.component";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { showErrorToUser, cmp, setOriginalMenu, newGUID, getAttachmentFilename, getCreationModificationTooltipText } from "../../../globals/utils";
import { getFormattedNumeroCommande } from "../../../globals/utils";
import { getEnvCommandeFournisseurStatut } from "../../../globals/utils";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { EventEmitterService } from "../../../services/event-emitter.service";
import { ControleComponent } from "../controle/controle.component";
import { Subject } from "rxjs";
import { CVQComponent } from "../cvq/cvq.component";
import { DownloadService } from "../../../services/download.service";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { ProgressBarComponent } from "../../global/progress-bar/progress-bar.component";
import { ProgressBarMode } from "@angular/material/progress-bar";

class MyErrorStateMatcher implements ErrorStateMatcher {
    //Constructor
    constructor(public applicationUserContext: ApplicationUserContext) {}
    isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
        let result: boolean = false;
        result = !!((control && control.invalid)); 
        return result;
    }
}

@Component({
    selector: "fiche-controle",
    templateUrl: "./fiche-controle.component.html",
    styleUrls: ["./fiche-controle.component.scss"],
    standalone: false
})

export class FicheControleComponent implements OnInit, OnDestroy {
  matcher = new MyErrorStateMatcher(this.applicationUserContext);
  //Variables
  ficheControle: dataModelsInterfaces.FicheControle = {} as dataModelsInterfaces.FicheControle;
  nextAction: string;
  //Form
  form: UntypedFormGroup;
  ficheControleDescriptionReceptions: UntypedFormArray = new UntypedFormArray([]);
  ficheControleControleurList: dataModelsInterfaces.ContactAdresse[];
  ficheControleTextFC: UntypedFormControl = new UntypedFormControl(null);
  numeroLotUsineFC: UntypedFormControl = new UntypedFormControl(null);
  ficheControleControleurListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  reserveFC: UntypedFormControl = new UntypedFormControl(null);
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  protected allChildrenSaved = new Subject<boolean>();
  //Functions
  getFormattedNumeroCommande = getFormattedNumeroCommande;
  getEnvCommandeFournisseurStatut = getEnvCommandeFournisseurStatut;
  //Global lock
  locked: boolean = true;
  saveLocked: boolean = true;
  //Display
  visibleAddControle: boolean = false;
  possibleCVQ: boolean = false;
  visibleAddCVQ: boolean = false;
  //Progress bar
  color = "warn" as ThemePalette;
  mode = "indeterminate" as ProgressBarMode;
  value = 50;
  displayProgressBar = false;
  //Children
  @ViewChildren(ControleComponent) controles: QueryList<ControleComponent>;
  @ViewChildren(CVQComponent) cVQs: QueryList<CVQComponent>;
  @ViewChild(ProgressBarComponent) progressBar: ProgressBarComponent;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , private dataModelService: DataModelService
    , private downloadService: DownloadService
    , private eventEmitterService: EventEmitterService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    this.allChildrenSaved.next(false);
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      FicheControleText: this.ficheControleTextFC,
      NumeroLotUsine: this.numeroLotUsineFC,
      FicheControleControleurList: this.ficheControleControleurListFC,
      Reserve: this.reserveFC,
      Cmt: this.cmtFC,
      FicheControleDescriptionReceptions: this.ficheControleDescriptionReceptions,
    });
    this.lockScreen();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.numeroLotUsineFC.disable();
    this.ficheControleControleurListFC.disable();
    this.reserveFC.disable();
    this.cmtFC.disable();
    this.ficheControleDescriptionReceptions.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.numeroLotUsineFC.enable();
    this.ficheControleControleurListFC.enable();
    this.reserveFC.enable();
    this.cmtFC.enable();
    this.ficheControleDescriptionReceptions.enable();
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
    let refFicheControle = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    let refCommandeFournisseur = Number.parseInt(this.activatedRoute.snapshot.params["refCommandeFournisseur"], 10);
    //Check parameters
    if (!isNaN(refFicheControle) && !isNaN(refCommandeFournisseur)) {
      this.loadData(refFicheControle, refCommandeFournisseur);
    }
    else {
      showErrorToUser(this.dialog, { error: { error: { message: this.applicationUserContext.getCulturedRessourceText(712) } } }, this.applicationUserContext);
    }
    //Wait for all controls to be correcty saved before saving FicheControle
    this.allChildrenSaved
      .subscribe(res => {
        if (res) {
          //Inform user
          this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
          //Next action if applicable
          if (this.nextAction !== "") {
            switch (this.nextAction) {
              case "CommandeFournisseur":
                //Save menu
                if (this.applicationUserContext.fromMenu == null) {
                  this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
                }
                this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
                this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
                this.eventEmitterService.onChangeMenu();
                this.router.navigate(["commande-fournisseur", this.ficheControle.CommandeFournisseur.RefCommandeFournisseur.toString()]);
                break;
              case "NonConformite":
                //Save menu
                if (this.applicationUserContext.fromMenu == null) {
                  this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
                }
                this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
                this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
                this.eventEmitterService.onChangeMenu();
                this.router.navigate(["non-conformite", "0", {
                  refCommandeFournisseur: this.ficheControle.CommandeFournisseur.RefCommandeFournisseur
                }]);
                break;
              case "Print":
                this.print();
                this.loadData(this.ficheControle.RefFicheControle, this.ficheControle.CommandeFournisseur.RefCommandeFournisseur);
                break;
            }
          }
          else {
            //Set original Menu
            setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
            this.router.navigate(["grid"]);
          }
        }
      });
  }
  //-----------------------------------------------------------------------------------
  //Destroy
  ngOnDestroy() {
    //Stop progress bar
    //this.displayProgressBar = false;
    this.progressBar.stop();
  }
  //-----------------------------------------------------------------------------------
  //Load data
  loadData(refFicheControle: number, refCommandeFournisseur: number) {
    this.dataModelService.getFicheControle(refFicheControle, refCommandeFournisseur).subscribe(result => {
      //Get data
      for (const c of result.Controles) { c.GUID = newGUID(); c.Saved = false; c.Deleted = false; }
      for (const c of result.CVQs) { c.GUID = newGUID(); c.Saved = false; c.Deleted = false; }
      this.ficheControle = result;
      //Sort ficheControle.FicheControleDescriptionReceptions
      let sortedArray: dataModelsInterfaces.FicheControleDescriptionReception[] = this.ficheControle.FicheControleDescriptionReceptions.sort(function (a, b) {
        return cmp(a.Ordre, b.Ordre);
      });
      this.ficheControle.FicheControleDescriptionReceptions = sortedArray;
      //Update form
      this.updateForm();
      //Get existing Controleur
      this.listService.getListContactAdresse(this.ficheControle.Controleur?.RefContactAdresse, this.ficheControle.CommandeFournisseur.AdresseClient?.RefEntite, null, ContactAdresseProcess.ControleurQualite, true)
        .subscribe(result => {
          this.ficheControleControleurList = result;
          //Auto select
          if (this.ficheControleControleurList.length === 1) { this.ficheControleControleurListFC.setValue(this.ficheControleControleurList[0].RefContactAdresse); }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      //Display
      this.dataModelService.getCVQ(0, this.ficheControle.CommandeFournisseur.Produit.RefProduit).subscribe(result => {
        this.possibleCVQ = result.CVQDescriptionCVQs.length > 0;
        //Display
        this.setVisibleAddCVQ();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      this.dataModelService.getControle(0, this.ficheControle.CommandeFournisseur.Produit.RefProduit).subscribe(result => {
        this.visibleAddControle = result.ControleDescriptionControles.length > 0;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.ficheControleTextFC.setValue(this.ficheControle.FicheControleText);
    this.numeroLotUsineFC.setValue(this.ficheControle.NumeroLotUsine);
    this.ficheControleControleurListFC.setValue(this.ficheControle.Controleur ? this.ficheControle.Controleur.RefContactAdresse : null);
    this.reserveFC.setValue(this.ficheControle.Reserve);
    this.cmtFC.setValue(this.ficheControle.Cmt);
    this.ficheControleDescriptionReceptions.clear();
    for (const dR of this.ficheControle.FicheControleDescriptionReceptions) {
      const grp = this.fb.group({
        Positif: [dR.Positif == null ? null : dR.Positif.toString(), Validators.required],
      });
      this.ficheControleDescriptionReceptions.push(grp);
    }
    //Manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.ficheControle.NumeroLotUsine = this.numeroLotUsineFC.value;
    this.ficheControle.Controleur = (this.ficheControleControleurList ? this.ficheControleControleurList.find(x => x.RefContactAdresse === this.ficheControleControleurListFC.value) : null);
    this.ficheControle.Reserve = this.reserveFC.value;
    this.ficheControle.Cmt = this.cmtFC.value;
    //FicheControleDescriptionReceptions
    for (let i in this.ficheControleDescriptionReceptions.controls) {
      let fG: UntypedFormGroup = this.ficheControleDescriptionReceptions.controls[i] as UntypedFormGroup;
      this.ficheControle.FicheControleDescriptionReceptions[i].Positif = (fG.get("Positif").value === "true" ? true : false);
    }
  }
  //-----------------------------------------------------------------------------------
  //Get data for IHM
  get getFormData(): UntypedFormArray {
    return this.ficheControleDescriptionReceptions as UntypedFormArray;
  }
  //-----------------------------------------------------------------------------------
  //Show ContactAdresse details
  showDetails(type: string, ref: number) {
    let data: any;
    //Init
    this.saveData();
    switch (type) {
      case "FicheControleControleur":
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
          case "FicheControleControleur":
            //Get existing Controleur
            this.listService.getListContactAdresse(this.ficheControle.Controleur?.RefContactAdresse, this.ficheControle.CommandeFournisseur?.AdresseClient?.RefEntite, null, ContactAdresseProcess.ControleurQualite, true)
              .subscribe(result => {
                this.ficheControleControleurList = result;
              }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
            break;
        }
      }
    });
  }
  //-----------------------------------------------------------------------------------
  //Go to CommandeFournisseur
  onCommandeFournisseur() {
    this.onSave("CommandeFournisseur");
  }
  //-----------------------------------------------------------------------------------
  //Go to NonConformite
  onNonConformite() {
    this.onSave("NonConformite");
  }
  //-----------------------------------------------------------------------------------
  //Save data
  onSave(next: string) {
    this.nextAction = next;
    if (!this.locked) {
      this.saveData();
      this.childrenMarkUnsaved();
      //Save FicheControle, then all children
      var url = this.baseUrl + "evapi/fichecontrole";
      // Display progress bar
      //this.displayProgressBar = true;
      this.progressBar.start();
      //Update if existing models
      this.http
        .post<dataModelsInterfaces.FicheControle>(url, this.ficheControle)
        .subscribe(result => {
          //Get data
          this.ficheControle.RefFicheControle = result.RefFicheControle;
          //Link all controls
          if (this.controles) {
            for (const c of this.controles) {
              c.controle.RefFicheControle = this.ficheControle.RefFicheControle;
            }
          }
          //Link all CVQ
          if (this.cVQs) {
            for (const c of this.cVQs) {
              c.cVQ.RefFicheControle = this.ficheControle.RefFicheControle;
            }
          }
          //Wait for all controls to be correcty saved before saving FicheControle
          //Subscribed in onInit
          //Save all controls
          if (this.controles) {
            for (const c of this.controles) {
              if (!(c.getSumPoids() === 0 || c.getSumPoids() > c.poidsFC.value || !c.form.valid)) { c.onDistantSave(); }
            }
          }
          //Save all CVQ
          if (this.cVQs) {
            for (const c of this.cVQs) {
              if (c.form.valid) { c.onDistantSave(); }
            }
          }
          //Check if all Controles and CVQs are saved (in case there are no Control and no CVQ )
          if (!(this.ficheControle.Controles && this.ficheControle.Controles.length > 0) && !(this.ficheControle.CVQs && this.ficheControle.CVQs.length > 0)) {
            this.checkAllChildrenSaved();
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Next action if applicable
      if (this.nextAction !== "") {
        switch (this.nextAction) {
          case "CommandeFournisseur":
            //Save menu
            if (this.applicationUserContext.fromMenu == null) {
              this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
            }
            this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
            this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
            this.eventEmitterService.onChangeMenu();
            this.router.navigate(["commande-fournisseur", this.ficheControle.CommandeFournisseur.RefCommandeFournisseur.toString()]);
            break;
          case "NonConformite":
            //Save menu
            if (this.applicationUserContext.fromMenu == null) {
              this.applicationUserContext.fromMenu = this.applicationUserContext.currentMenu;
            }
            this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Qualite);
            this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.QualiteMenuNonConformite);
            this.eventEmitterService.onChangeMenu();
            this.router.navigate(["non-conformite", "0", {
              refCommandeFournisseur: this.ficheControle.CommandeFournisseur.RefCommandeFournisseur
            }]);
            break;
          case "Print":
            this.print();
            break;
        }
      }
      else {
        //Set original Menu
        setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
        this.router.navigate(["grid"]);
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Set CVQ display
  setVisibleAddCVQ() {
    this.visibleAddCVQ = this.possibleCVQ;
    for (const c of this.ficheControle.CVQs) {
      if (!c.Deleted) { this.visibleAddCVQ = false; }
    }
  }
  //-----------------------------------------------------------------------------------
  //Children validity
  childrenValid() {
    let r: boolean = true;
    if (this.controles) {
      for (const c of this.controles) {
        if (c.getSumPoids() === 0 || c.getSumPoids() > c.poidsFC.value || !c.form.valid) { r = false; }
      }
    }
    if (this.cVQs) {
      for (const c of this.cVQs) {
        if (!c.form.valid) { r = false; }
      }
    }
    return r;
  }
  //-----------------------------------------------------------------------------------
  //Children mark unsaved
  childrenMarkUnsaved() {
    if (this.ficheControle.Controles) {
      for (const c of this.ficheControle.Controles) {
        c.Saved = false;
      }
    }
    if (this.ficheControle.CVQs) {
      for (const c of this.ficheControle.CVQs) {
        c.Saved = false;
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Children saved
  childrenControleSaved(gUID: string) {
    //Mark as saved
    this.ficheControle.Controles[this.ficheControle.Controles.findIndex(item => item.GUID == gUID)].Saved = true;
    //Check if all Controles and CVQs are saved
    this.checkAllChildrenSaved();
  }
  //-----------------------------------------------------------------------------------
  //Children deleted
  childrenControleDeleted(gUID: string) {
    //Mark as deleted
    this.ficheControle.Controles[this.ficheControle.Controles.findIndex(item => item.GUID == gUID)].Deleted = true;
  }
  //-----------------------------------------------------------------------------------
  //Add new Controle
  onAddControle() {
    //Get new element from server
    this.dataModelService.getControle(0, this.ficheControle.CommandeFournisseur.Produit.RefProduit).subscribe(result => {
      result.GUID = newGUID();
      result.Deleted = false;
      this.ficheControle.Controles.push(result);
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //Children saved
  childrenCVQSaved(gUID: string) {
    //Mark as saved
    this.ficheControle.CVQs[this.ficheControle.CVQs.findIndex(item => item.GUID == gUID)].Saved = true;
    //Check if all Controles and CVQs are saved
    this.checkAllChildrenSaved();
  }
  //-----------------------------------------------------------------------------------
  //Check for all children to be saved
  checkAllChildrenSaved() {
    let r: boolean = true;
    for (const c of this.ficheControle.Controles) {
      if (!c.Saved && !c.Deleted) { r = false; }
    }
    for (const c of this.ficheControle.CVQs) {
      if (!c.Saved && !c.Deleted) { r = false; }
    }
    if (r) {
      //Stop progress bar
      this.progressBar.stop();
      //End
      this.allChildrenSaved.next(true);
    }
  }
  //-----------------------------------------------------------------------------------
  //Children deleted
  childrenCVQDeleted(gUID: string) {
    //Mark as deleted
    this.ficheControle.CVQs[this.ficheControle.CVQs.findIndex(item => item.GUID == gUID)].Deleted = true;
    //Display
    this.setVisibleAddCVQ();
  }
  //-----------------------------------------------------------------------------------
  //Add new Controle
  onAddCVQ() {
    //Get new element from server
    this.dataModelService.getCVQ(0, this.ficheControle.CommandeFournisseur.Produit.RefProduit).subscribe(result => {
      result.GUID = newGUID();
      result.Deleted = false;
      this.ficheControle.CVQs.push(result);
      //Display
      this.setVisibleAddCVQ();
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Print
  print() {
    this.downloadService.download("FicheControle", this.ficheControle.RefFicheControle.toString(), "xlsx", null, "")
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  downloadFile(data: any, fileName: string) {
    const blob = new Blob([data]);
      const url = window.URL.createObjectURL(blob);
      const a: HTMLAnchorElement = document.createElement("a") as HTMLAnchorElement;
      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(107) },
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    var url = this.baseUrl + "evapi/fichecontrole/" + this.ficheControle.RefFicheControle;
    this.http
      .delete(url)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(774), duration: 4000 } as appInterfaces.SnackbarMsg);
        //Set original Menu
        setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
        this.router.navigate(["grid"]);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    //Set original Menu
    setOriginalMenu(this.applicationUserContext, this.eventEmitterService);
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationModificationTooltipText(): string {
    return getCreationModificationTooltipText(this.ficheControle);
  }
}
