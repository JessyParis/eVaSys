import { Component } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { UtilsService } from "../../../services/utils.service";
import { ConfirmComponent } from "../../dialogs/confirm/confirm.component";
import { cmp, showErrorToUser } from "../../../globals/utils";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { DomSanitizer } from "@angular/platform-browser";
import { BaseFormComponent } from "../../_ancestors/base-form.component";
import { ListService } from "../../../services/list.service";
import { MatCheckboxChange } from "@angular/material/checkbox";

@Component({
    selector: "produit",
    templateUrl: "./produit.component.html",
    standalone: false
})

export class ProduitComponent extends BaseFormComponent<dataModelsInterfaces.Produit> {
  sAGECodeTransportList: dataModelsInterfaces.SAGECodeTransport[] = [];
  applicationProduitOrigineList: dataModelsInterfaces.ApplicationProduitOrigine[] = [];
  produitGroupeReportingList: dataModelsInterfaces.ProduitGroupeReporting[] = [];
  produitComposantList: dataModelsInterfaces.ProduitComposant[] = [];
  produitStandardList: dataModelsInterfaces.ProduitStandard[] = [];
  //Validators
  requiredCodeListeVerte: boolean = true;
  requiredSAGECodeTransport: boolean = true;
  requiredNumeroStatistique: boolean = true;
  //Form
  form: UntypedFormGroup;
  produit: dataModelsInterfaces.Produit = {Actif: true} as dataModelsInterfaces.Produit;
  actifFC: UntypedFormControl = new UntypedFormControl(null);
  libelleFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  nomCommercialFC: UntypedFormControl = new UntypedFormControl(null);
  nomCommunFC: UntypedFormControl = new UntypedFormControl(null);
  collecteFC: UntypedFormControl = new UntypedFormControl(null);
  composantFC: UntypedFormControl = new UntypedFormControl(null);
  numeroStatistiqueFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  sAGECodeTransportListFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  codeListeVerteFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required]);
  applicationProduitOrigineListFC: UntypedFormControl = new UntypedFormControl(null);
  produitGroupeReportingListFC: UntypedFormControl = new UntypedFormControl(null);
  incitationQualiteFC: UntypedFormControl = new UntypedFormControl(null);
  pUHTSurtriFC: UntypedFormControl = new UntypedFormControl(null);
  pUHTTransportFC: UntypedFormControl = new UntypedFormControl(null);
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  codeEEFC: UntypedFormControl = new UntypedFormControl(null);
  composantListFC: UntypedFormControl = new UntypedFormControl(null);
  standardListFC: UntypedFormControl = new UntypedFormControl(null);
  co2KgParT: UntypedFormControl = new UntypedFormControl(null, [Validators.max(10000)]);
  cmtFournisseurFC: UntypedFormControl = new UntypedFormControl(null);
  cmtTransporteurFC: UntypedFormControl = new UntypedFormControl(null);
  cmtClientFC: UntypedFormControl = new UntypedFormControl(null);
  laserTypeListFC: UntypedFormControl = new UntypedFormControl(null);
  //Constructor
  constructor(protected activatedRoute: ActivatedRoute
    , protected router: Router
    , private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected utilsService: UtilsService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , protected dialog: MatDialog
    , protected sanitizer: DomSanitizer) {
    super("ProduitComponent", activatedRoute, router, applicationUserContext, dataModelService
      , utilsService, snackBarQueueService, dialog, sanitizer);
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Actif: this.actifFC,
      Libelle: this.libelleFC,
      NomCommercial: this.nomCommercialFC,
      NomCommun: this.nomCommunFC,
      Collecte: this.collecteFC,
      Composant: this.composantFC,
      NumeroStatistique: this.numeroStatistiqueFC,
      SAGECodeTransportList: this.sAGECodeTransportListFC,
      CodeListeVerte: this.codeListeVerteFC,
      ApplicationProduitOrigineList: this.applicationProduitOrigineListFC,
      ProduitGroupeReportingList: this.produitGroupeReportingListFC,
      IncitationQualite: this.incitationQualiteFC,
      PUHTSurtri: this.pUHTSurtriFC,
      PUHTTransport: this.pUHTTransportFC,
      Cmt: this.cmtFC,
      CodeEE: this.codeEEFC,
      ComposantList: this.composantListFC,
      StandardList: this.standardListFC,
      Co2KgParT: this.co2KgParT,
      CmtFournisseur: this.cmtFournisseurFC,
      CmtTransporteur: this.cmtTransporteurFC,
      CmtClient: this.cmtClientFC,
      LaserTypeList: this.laserTypeListFC,
    });
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    //Load initial data
    //Get existing SAGECodeTransport
    this.listService.getList<dataModelsInterfaces.SAGECodeTransport>("SAGECodeTransportComponent").subscribe(result => {
      this.sAGECodeTransportList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ApplicationProduitOrigine
    this.listService.getList<dataModelsInterfaces.ApplicationProduitOrigine>("ApplicationProduitOrigineComponent").subscribe(result => {
      this.applicationProduitOrigineList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing ProduitGroupeReporting
    this.listService.getList<dataModelsInterfaces.ProduitGroupeReporting>("ProduitGroupeReportingComponent").subscribe(result => {
      this.produitGroupeReportingList = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Produit as Composant
    this.listService.getListProduit(null, null, null, null, null, null, null, false, false).subscribe(result => {
      this.produitComposantList = result.map(item => ({ Composant: item as dataModelsInterfaces.ProduitList } as dataModelsInterfaces.ProduitComposant));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Get existing Standard
    this.listService.getListStandard(null, null).subscribe(result => {
      this.produitStandardList = result.map(item => ({ Standard: item } as dataModelsInterfaces.ProduitStandard));
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    //Check parameters
    if (!isNaN(id)) {
      if (id != 0) {
        this.dataModelService.getDataModel<dataModelsInterfaces.Produit>(id, this.componentName).subscribe(result => {
          //Get data
          this.produit = result;
          this.sourceObj = result;
          //Update form
          this.updateForm();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
      else {
        //Update form
        this.updateForm();
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
    this.actifFC.setValue(this.produit.Actif);
    this.libelleFC.setValue(this.produit.Libelle);
    this.nomCommercialFC.setValue(this.produit.NomCommercial);
    this.nomCommunFC.setValue(this.produit.NomCommun);
    this.collecteFC.setValue(this.produit.Collecte);
    this.composantFC.setValue(this.produit.Composant);
    this.numeroStatistiqueFC.setValue(this.produit.NumeroStatistique);
    this.sAGECodeTransportListFC.setValue(this.produit.SAGECodeTransport?.RefSAGECodeTransport);
    this.codeListeVerteFC.setValue(this.produit.CodeListeVerte);
    this.applicationProduitOrigineListFC.setValue(this.produit.ApplicationProduitOrigine?.RefApplicationProduitOrigine);
    this.produitGroupeReportingListFC.setValue(this.produit.ProduitGroupeReporting?.RefProduitGroupeReporting);
    this.incitationQualiteFC.setValue(this.produit.IncitationQualite);
    this.pUHTSurtriFC.setValue(this.produit.PUHTSurtri);
    this.pUHTTransportFC.setValue(this.produit.PUHTTransport);
    this.cmtFC.setValue(this.produit.Cmt);
    this.codeEEFC.setValue(this.produit.CodeEE);
    this.composantListFC.setValue(this.produit.ProduitComposants);
    this.standardListFC.setValue(this.produit.ProduitStandards);
    this.co2KgParT.setValue(this.produit.Co2KgParT);
    this.cmtFournisseurFC.setValue(this.produit.CmtFournisseur);
    this.cmtTransporteurFC.setValue(this.produit.CmtTransporteur);
    this.cmtClientFC.setValue(this.produit.CmtClient);
    this.laserTypeListFC.setValue(this.produit.LaserType);
    //Manage screen
    this.manageScreen();
  }
  //Manage screen
  manageScreen() {
    //Global lock
    if (1 !== 1) {
      this.lockScreen();
    }
    else {
      //Init
      this.unlockScreen();
      //validators
      if (this.produit.Composant) {
        this.requiredSAGECodeTransport = false;
        this.sAGECodeTransportListFC.clearValidators();
        this.sAGECodeTransportListFC.updateValueAndValidity();
        this.requiredCodeListeVerte = false;
        this.codeListeVerteFC.clearValidators();
        this.codeListeVerteFC.updateValueAndValidity();
        this.requiredNumeroStatistique = false;
        this.numeroStatistiqueFC.clearValidators();
        this.numeroStatistiqueFC.updateValueAndValidity();
      }
      else {
        this.requiredSAGECodeTransport = true;
        this.sAGECodeTransportListFC.setValidators([Validators.required]);
        this.sAGECodeTransportListFC.updateValueAndValidity();
        this.requiredCodeListeVerte = true;
        this.codeListeVerteFC.setValidators([Validators.required]);
        this.codeListeVerteFC.updateValueAndValidity();
        this.requiredNumeroStatistique = true;
        this.numeroStatistiqueFC.setValidators([Validators.required]);
        this.numeroStatistiqueFC.updateValueAndValidity();
      }
    }
  }
  //-----------------------------------------------------------------------------------
  //Creates a new similar Produit
  onCopy() {
    this.saveData();
    //Process
    this.produit.RefProduit = undefined;
    this.produit.Libelle += " - copie";
    this.produit.ProduitComposants=[];
    //this.produit.ProduitComposants.forEach(pC => pC.RefProduitComposant = undefined);
    this.produit.ProduitStandards.forEach(pS => pS.RefProduitStandard = undefined);
    this.updateForm();
    //Inform user
    this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1439), duration: 4000 } as appInterfaces.SnackbarMsg);
  }
  //-----------------------------------------------------------------------------------
  //Update the data model
  saveData() {
    this.produit.Actif = this.actifFC.value ? true : false;
    this.produit.Libelle = this.libelleFC.value;
    this.produit.NomCommercial = this.nomCommercialFC.value;
    this.produit.NomCommun = this.nomCommunFC.value;
    this.produit.Collecte = this.collecteFC.value;
    this.produit.Composant = this.composantFC.value;
    this.produit.NumeroStatistique = this.numeroStatistiqueFC.value;
    this.produit.SAGECodeTransport = this.sAGECodeTransportList.find(x => x.RefSAGECodeTransport === this.sAGECodeTransportListFC.value);
    this.produit.ApplicationProduitOrigine = this.applicationProduitOrigineList.find(x => x.RefApplicationProduitOrigine === this.applicationProduitOrigineListFC.value);
    this.produit.CodeListeVerte = this.codeListeVerteFC.value;
    this.produit.ProduitGroupeReporting = this.produitGroupeReportingList.find(x => x.RefProduitGroupeReporting === this.produitGroupeReportingListFC.value);
    this.produit.IncitationQualite = this.incitationQualiteFC.value;
    this.produit.PUHTSurtri = this.pUHTSurtriFC.value;
    this.produit.PUHTTransport = this.pUHTTransportFC.value;
    this.produit.Cmt = this.cmtFC.value;
    this.produit.CodeEE = this.codeEEFC.value;
    this.produit.ProduitComposants = this.composantListFC.value;
    this.produit.ProduitStandards = this.standardListFC.value;
    this.produit.Co2KgParT = this.co2KgParT.value;
    this.produit.CmtFournisseur = this.cmtFournisseurFC.value;
    this.produit.CmtTransporteur = this.cmtTransporteurFC.value;
    this.produit.CmtClient = this.cmtClientFC.value;
    this.produit.LaserType = this.laserTypeListFC.value;
    //manage screen
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //If Composant, or not
  onComposantChange(ev: MatCheckboxChange) {
    this.produit.Composant = ev.checked ? true : false;
    this.manageScreen();
  }
  //-----------------------------------------------------------------------------------
  //Saves the data model in DB
  onSave() {
    this.saveData();
    //Update
    this.dataModelService.postDataModel<dataModelsInterfaces.Produit>(this.produit, this.componentName)
      .subscribe(result => {
        //Inform user
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
      data: { title: this.applicationUserContext.getCulturedRessourceText(300), message: this.applicationUserContext.getCulturedRessourceText(this.ressBeforeDel) },
      autoFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === "yes") {
        this.delete();
      }
    });
  }
  delete() {
    let id = Number.parseInt(this.activatedRoute.snapshot.params["id"], 10);
    this.dataModelService.deleteDataModel<dataModelsInterfaces.Produit>(id, this.componentName)
      .subscribe(result => {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(this.ressAfterDel), duration: 4000 } as appInterfaces.SnackbarMsg);
        this.router.navigate(["grid"]);
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Multi select
  compareFnComposant(item: dataModelsInterfaces.ProduitComposant, selectedItem: dataModelsInterfaces.ProduitComposant) {
    return item && selectedItem ? item.Composant.RefProduit === selectedItem.Composant.RefProduit : item === selectedItem;
  }
  public onComposantListReset() {
    this.produit.ProduitComposants = [];
    this.composantListFC.setValue(this.produit.ProduitComposants);
  }
  compareFnStandard(item: dataModelsInterfaces.ProduitStandard, selectedItem: dataModelsInterfaces.ProduitStandard) {
    return item && selectedItem ? item.Standard.RefStandard === selectedItem.Standard.RefStandard : item === selectedItem;
  }
  public onStandardListReset() {
    this.produit.ProduitStandards = [];
    this.standardListFC.setValue(this.produit.ProduitStandards);
  }
  //-----------------------------------------------------------------------------------
  //Create Composant label
  composantsLabel(): string {
    let s = this.composantListFC.value?.sort(function (a, b) { return cmp(a.Composant.Libelle, b.Composant.Libelle); })
      .map(item => item.Composant.Libelle).join("\n");
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create Standards label
  standardsLabel(): string {
    let s = this.standardListFC.value?.sort(function (a, b) { return cmp(a.Standard.Libelle, b.Standard.Libelle); })
      .map(item => item.Standard.Libelle).join("\n");
    //End
    return s;
  }
}
