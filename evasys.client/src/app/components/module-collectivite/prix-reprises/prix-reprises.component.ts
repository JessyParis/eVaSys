import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, isFormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpParams } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { EventEmitterService } from "../../../services/event-emitter.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import * as appClasses from "../../../classes/appClasses";
import { cmp, showErrorToUser } from "../../../globals/utils";
import { HabilitationLogistique } from "../../../globals/enums";
import moment from "moment";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DataModelService } from "../../../services/data-model.service";
import { fadeInOnEnterAnimation } from "angular-animations";

@Component({
    selector: "prix-reprises",
    templateUrl: "./prix-reprises.component.html",
    animations: [
        fadeInOnEnterAnimation(),
    ],
    standalone: false
})

export class PrixReprisesComponent implements OnInit {
  form: UntypedFormGroup;
  collectivitePrixReprises: dataModelsInterfaces.CollectivitePrixReprise[] = [];
  collectiviteStandards: dataModelsInterfaces.CollectivitePrixReprise[] = [];
  refEntite: number = 0;
  d: moment.Moment = moment(new Date(new Date().getFullYear(), new Date().getMonth(), 1, 0, 0, 0, 0));

  currentMonth: string = "";
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router, private fb: UntypedFormBuilder
    , private http: HttpClient, @Inject("BASE_URL") private baseUrl: string
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private eventEmitterService: EventEmitterService
    , private snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    //Sow/hide ModuleCollectivite header
    if (applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) {
      applicationUserContext.visibleHeaderCollectivite = true;
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //Set filters
    if (this.applicationUserContext.filterCollectivites.length == 1) {
      this.refEntite = this.applicationUserContext.filterCollectivites[0].RefEntite;
    }
    this.currentMonth = this.d.format("MMMM YYYY").toUpperCase();
    //Get existing CollectivitePrixReprises
    this.dataModelService.getCollectivitePrixReprises(this.refEntite, this.d, this.applicationUserContext.filterCollecte).subscribe(result => {
      this.collectivitePrixReprises = result;
      this.collectiviteStandards = this.collectivitePrixReprises.filter(
        (pR, i, arr) => arr.findIndex(t => t.StandardLibelle === pR.StandardLibelle) === i
      )
      if (result.length == 0) {
        this.d = this.d.add(-1, "months");
        this.currentMonth = this.d.format("MMMM YYYY").toUpperCase();
        this.dataModelService.getCollectivitePrixReprises(this.refEntite, this.d, this.applicationUserContext.filterCollecte).subscribe(result => {
          this.collectivitePrixReprises = result;
          this.collectiviteStandards = this.collectivitePrixReprises.filter(
            (pR, i, arr) => arr.findIndex(t => t.StandardLibelle === pR.StandardLibelle) === i
          )
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  getPRs(standardLibelle): dataModelsInterfaces.CollectivitePrixReprise[] {
    return this.collectivitePrixReprises.filter(
      (pR, i, arr) => (pR.StandardLibelle == standardLibelle));
  }
  //-----------------------------------------------------------------------------------
  //On filter
  onChipChange($event: any) {
    if (!$event.selected) {
      $event.source.selected = true;
    }
    if (this.applicationUserContext.filterCollecte != $event.source.value) {
      this.applicationUserContext.filterCollecte = $event.source.value;
      this.dataModelService.getCollectivitePrixReprises(this.refEntite, this.d, this.applicationUserContext.filterCollecte).subscribe(result => {
        this.collectivitePrixReprises = result;
        this.collectiviteStandards = this.collectivitePrixReprises.filter(
          (pR, i, arr) => arr.findIndex(t => t.StandardLibelle === pR.StandardLibelle) === i
        )
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
}
