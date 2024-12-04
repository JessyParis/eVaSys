import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, FormGroupDirective, NgForm, UntypedFormArray } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../../services/list.service";
import { DataModelService } from "../../../services/data-model.service";
import moment from "moment";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { cmp, showErrorToUser, clearFormArray, getCreationModificationTooltipText } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";

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
    selector: "surcout-carburant",
    templateUrl: "./surcout-carburant.component.html",
    styleUrls: ["./surcout-carburant.component.scss"],
    standalone: false
})

export class SurcoutCarburantComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  surcoutCarburant: dataModelsInterfaces.SurcoutCarburant;
  surcoutCarburants: dataModelsInterfaces.SurcoutCarburant[];
  sCs: UntypedFormArray = new UntypedFormArray([]);
  entiteList: dataModelsInterfaces.EntiteList[];
  paysList: dataModelsInterfaces.Pays[];
  yearList: appInterfaces.Year[];
  entiteListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  paysListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  yearListFC: UntypedFormControl = new UntypedFormControl(null, Validators.required);
  //Global lock
  locked: boolean = false;
  saveLocked: boolean = false;
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
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
      EntiteList: this.entiteListFC,
      PaysList: this.paysListFC,
      YearList: this.yearListFC,
      SCs: this.sCs,
    });
    //this.lockScreen();
  }
  //-----------------------------------------------------------------------------------
  //Lock all controls
  lockScreen() {
    this.locked = true;
    this.entiteListFC.disable();
    this.paysListFC.disable();
    this.yearListFC.disable();
    this.sCs.disable();
  }
  //-----------------------------------------------------------------------------------
  //Lock main controls
  lockMainControls() {
    this.entiteListFC.disable();
    this.paysListFC.disable();
    this.yearListFC.disable();
  }
  //-----------------------------------------------------------------------------------
  //Unlock all controls
  unlockScreen() {
    this.locked = false;
    this.entiteListFC.enable();
    this.paysListFC.enable();
    this.yearListFC.enable();
    this.sCs.enable();
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    //Get Entite
    this.listService.getListTransporteur(null, true, true)
      .subscribe(result => {
        this.entiteList = result;
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing year
    this.listService.getListYear().subscribe(result => {
      this.yearList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check parameters
    if (!isNaN(id)) {
      this.dataModelService.getSurcoutCarburant(id).subscribe(result => {
        //Get original SurcoutCarburant
        this.surcoutCarburant = result;
        //Lock main data
        if (this.surcoutCarburant.RefSurcoutCarburant > 0) {
          this.lockMainControls();
        }
        //Get Pays
        this.listService.getListPays(this.surcoutCarburant.Pays?.RefPays, false)
          .subscribe(result => {
            this.paysList = result;
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        //Get data
        this.getData();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
  //-----------------------------------------------------------------------------------
  //Form creation when data model exists
  updateForm() {
    this.entiteListFC.setValue((this.surcoutCarburant.Transporteur ? this.surcoutCarburant.Transporteur.RefEntite : null));
    this.paysListFC.setValue((this.surcoutCarburant.Pays ? this.surcoutCarburant.Pays.RefPays : null));
    this.yearListFC.setValue(this.surcoutCarburant.Annee);
    //Empty formArray before filling it
    clearFormArray(this.sCs);
    for (const sC of this.surcoutCarburants) {
      const grp = this.fb.group({
        Ratio: [sC.Ratio ? sC.Ratio : null, [Validators.max(100), Validators.min(-100)]],
        AllPays: sC.AllPays,
      });
      this.sCs.push(grp);
    }
  }
  //-----------------------------------------------------------------------------------
  //Seclection change
  selectionChange() {
    this.surcoutCarburant.Transporteur = this.entiteListFC.value == null ? null : this.entiteList.find(x => x.RefEntite === this.entiteListFC.value);
    this.surcoutCarburant.Pays = this.paysListFC.value == null ? null : this.paysList.find(x => x.RefPays === this.paysListFC.value);
    this.surcoutCarburant.Annee = this.yearListFC.value;
    this.getData();
  }
    //-----------------------------------------------------------------------------------
    //Get data
  getData() {
    //Init
    this.surcoutCarburants = [];
    //Process
    if (this.surcoutCarburant.Transporteur
      && this.surcoutCarburant.Pays
      && this.surcoutCarburant.Annee>0
    ) {
      this.dataModelService.getSurcoutCarburants(this.surcoutCarburant.Transporteur.RefEntite, this.surcoutCarburant.Pays.RefPays, this.surcoutCarburant.Annee).subscribe(res => {
        //Get all SCs
        this.surcoutCarburants = res;
        //Sort SurcoutCarburants
        var sortedArray: dataModelsInterfaces.SurcoutCarburant[] = this.surcoutCarburants.sort(function (a, b) {
          return cmp(a.Mois, b.Mois);
        });
        this.surcoutCarburants = sortedArray;
        //Update form
        this.updateForm();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    for (let i in this.sCs.controls) {
      let fG: UntypedFormGroup = this.sCs.controls[i] as UntypedFormGroup;
      this.surcoutCarburants[i].Ratio = (fG.get("Ratio").value ? fG.get("Ratio").value : 0);
      this.surcoutCarburants[i].AllPays = fG.get("AllPays").value;
    }
  }
  //-----------------------------------------------------------------------------------
  //Get data for IHM
  get getFormData(): UntypedFormArray {
    return this.sCs as UntypedFormArray;
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Check if some, already exported to SAGE, CommandeFournisseur may be impacted
    var url = this.baseUrl + "evapi/surcoutcarburant/checkpostsurcoutcarburants";
    //Update 
    this.http
      .post<boolean>(url, this.surcoutCarburants)
      .subscribe(result => {
        if (result) {
          this.submit();
        }
        else {
          //Ask user
          const dialogRef = this.dialog.open(ConfirmComponent, {
            width: "350px",
            data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(718) },
            autoFocus: false,
            restoreFocus: false
          });

          dialogRef.afterClosed().subscribe(result => {
            if (result === "yes") {
              this.submit();
            }
          });
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  submit() {
    var url = this.baseUrl + "evapi/surcoutcarburant/postsurcoutcarburants";
    //Update 
    this.http
      .post<dataModelsInterfaces.SurcoutCarburant[]>(url, this.surcoutCarburants)
      .subscribe(result => {
        //Redirect to grid and inform user
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(120), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Deletes the data model in DB
  //Route back to the list
  onDelete(): void {
    const dialogRef = this.dialog.open(ConfirmComponent, {
      width: "350px",
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(1530) },
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
    var url = this.baseUrl + "evapi/surcoutcarburant/" + this.surcoutCarburant.RefSurcoutCarburant;
    this.http
      .delete(url)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1531), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
  //-----------------------------------------------------------------------------------
  //Get month name
  getMonthName(n: number) {
  return moment(new Date(2000, n, 1)).format("MMMM");
  }
  //-----------------------------------------------------------------------------------
  //Format multiline tooltip text for creation/modification
  getCreationTooltipText(i: number): string {
    return getCreationModificationTooltipText(this.surcoutCarburants[i]);
  }
}
