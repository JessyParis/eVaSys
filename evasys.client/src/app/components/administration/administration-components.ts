import { Component } from "@angular/core";
import { AncestorComponent } from "../_ancestors/ancestor.component";
import { UntypedFormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationUserContext } from "../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import { ListService } from "../../services/list.service";
import { DataModelService } from "../../services/data-model.service";
import { SnackBarQueueService } from "../../services/snackbar-queue.service";
import * as dataModelsInterfaces from "../../interfaces/dataModelsInterfaces";
import { FormControlType, ListContentType } from "../../globals/enums";

@Component({
    selector: "adressetype",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class AdresseTypeComponent extends AncestorComponent<dataModelsInterfaces.AdresseType> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("AdresseTypeComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "application",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class ApplicationComponent extends AncestorComponent<dataModelsInterfaces.Application> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("ApplicationComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "application-produit-origine",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class ApplicationProduitOrigineComponent extends AncestorComponent<dataModelsInterfaces.ApplicationProduitOrigine> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("ApplicationProduitOrigineComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "camion-type",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class CamionTypeComponent extends AncestorComponent<dataModelsInterfaces.CamionType> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("CamionTypeComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("Libelle", FormControlType.Texte, true, 474);
    this.CreateControl("ModeTransportEE", FormControlType.List, true, 10058).listContent = ListContentType.ModeTransportEE;
  }
}

@Component({
    selector: "ressource",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class RessourceComponent extends AncestorComponent<dataModelsInterfaces.Ressource> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("RessourceComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("LibelleFRFR", FormControlType.TexteMulti, true, 741).maxlength = 1000;
    this.CreateControl("LibelleENGB", FormControlType.TexteMulti, false, 742).maxlength = 1000;
  }
}

@Component({
    selector: "civilite",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class CiviliteComponent extends AncestorComponent<dataModelsInterfaces.Civilite> {
  constructor(protected override activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("CiviliteComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("Pays", FormControlType.List, true, 10002).listContent = ListContentType.Pays;
    this.CreateControl("Libelle", FormControlType.Texte, true, 469);
  }
}

@Component({
    selector: "code-transport-sage",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class SAGECodeTransportComponent extends AncestorComponent<dataModelsInterfaces.SAGECodeTransport> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("SAGECodeTransportComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("Code", FormControlType.Texte, true, 1028);
    let ctrl = this.CreateControl("Libelle", FormControlType.Texte, true, 1029);
    ctrl.comparer = function (item: dataModelsInterfaces.EntiteStandard, selectedItem: dataModelsInterfaces.EntiteStandard): boolean {
      return item && selectedItem ? item.Standard.RefStandard === selectedItem.Standard.RefStandard : item === selectedItem;
    };
  }
}

@Component({
    selector: "eco-organisme",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class EcoOrganismeComponent extends AncestorComponent<dataModelsInterfaces.EcoOrganisme> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("EcoOrganismeComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("Libelle", FormControlType.Texte, true, 1416);
  }
}

@Component({
    selector: "entite-type",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class EntiteTypeComponent extends AncestorComponent<dataModelsInterfaces.EntiteType> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("EntiteTypeComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("LibelleFRFR", FormControlType.Texte, true, 10070);
    this.CreateControl("LibelleENGB", FormControlType.Texte, true, 10071);
    this.CreateControl("BLUnique", FormControlType.Checkbox, false, 10072);
    this.CreateControl("CodeEE", FormControlType.Checkbox, false, 64);
    this.CreateControl("RefEcoOrganisme", FormControlType.Checkbox, false, 1416);
    this.CreateControl("CodeValorisation", FormControlType.Checkbox, false, 1137);
    this.CreateControl("AssujettiTVA", FormControlType.Checkbox, false, 787);
    this.CreateControl("CodeTVA", FormControlType.Checkbox, false, 788);
    this.CreateControl("SAGECodeComptable", FormControlType.Checkbox, false, 1130);
    this.CreateControl("SAGECompteTiers", FormControlType.Checkbox, false, 1138);
    this.CreateControl("RefSAGEConditionLivraison", FormControlType.Checkbox, false, 1139);
    this.CreateControl("RefSAGEPeriodicite", FormControlType.Checkbox, false, 1140);
    this.CreateControl("RefSAGEModeReglement", FormControlType.Checkbox, false, 1141);
    this.CreateControl("RefSAGECategorieAchat", FormControlType.Checkbox, false, 1152);
    this.CreateControl("RefSAGECategorieVente", FormControlType.Checkbox, false, 1142);
    this.CreateBox(0);
    this.CreateControl("ActionnaireProprietaire", FormControlType.Checkbox, false, 1133);
    this.CreateControl("Exploitant", FormControlType.Checkbox, false, 1134);
    this.CreateControl("Demarrage", FormControlType.Checkbox, false, 10089);
    this.CreateControl("Capacite", FormControlType.Checkbox, false, 1135);
    this.CreateControl("TonnageGlobal", FormControlType.Checkbox, false, 10090);
    this.CreateControl("RefEquipementier", FormControlType.Checkbox, false, 1156);
    this.CreateControl("PopulationContratN", FormControlType.Checkbox, false, 1128);
    this.CreateControl("LiaisonEntiteProduit", FormControlType.Checkbox, false, 10091);
    this.CreateControl("LiaisonEntiteProduitInterdit", FormControlType.Checkbox, false, 10095);
    this.CreateControl("LiaisonEntiteStandard", FormControlType.Checkbox, false, 1030);
    this.CreateControl("LiaisonEntiteDR", FormControlType.Checkbox, false, 764);
    this.CreateControl("Horaires", FormControlType.Checkbox, false, 1186);
    this.CreateControl("ContratOptimisationTransport", FormControlType.Checkbox, false, 10092);
    this.CreateControl("VisibiliteAffretementCommun", FormControlType.Checkbox, false, 1143);
    this.CreateControl("ContratCollectivite", FormControlType.Checkbox, false, 1176);
    this.CreateBox(0);
    this.CreateControl("RefRepreneur", FormControlType.Checkbox, false, 1132);
    this.CreateControl("RefRepriseType", FormControlType.Checkbox, false, 1131);
    this.CreateControl("RepartitionMensuelle", FormControlType.Checkbox, false, 591);
    this.CreateControl("UtilisateurCollectivite", FormControlType.Checkbox, false, 10093);
    this.CreateControl("Process", FormControlType.Checkbox, false, 583);
    this.CreateControl("AutoControle", FormControlType.Checkbox, false, 905);
    this.CreateControl("SousContrat", FormControlType.Checkbox, false, 1155);
    this.CreateControl("LiaisonEntiteCamionType", FormControlType.Checkbox, false, 10094);
    this.CreateControl("IncitationQualite", FormControlType.Checkbox, false, 700);
    this.CreateControl("SurcoutCarburant", FormControlType.Checkbox, false, 714);
    this.CreateControl("RefFournisseurTO", FormControlType.Checkbox, false, 1157);
    this.CreateControl("DimensionBalle", FormControlType.Checkbox, false, 1158);
    this.CreateControl("ExportSAGE", FormControlType.Checkbox, false, 500);
    this.CreateControl("IdNational", FormControlType.Checkbox, false, 1421);
  }
}

@Component({
    selector: "equipementier",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class EquipementierComponent extends AncestorComponent<dataModelsInterfaces.Equipementier> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("EquipementierComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "fonction",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class FonctionComponent extends AncestorComponent<dataModelsInterfaces.Fonction> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("FonctionComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "forme-contact",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class FormeContactComponent extends AncestorComponent<dataModelsInterfaces.FormeContact> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("FormeContactComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "fournisseurto",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class FournisseurTOComponent extends AncestorComponent<dataModelsInterfaces.FournisseurTO> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("FournisseurTOComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "jour-ferie",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class JourFerieComponent extends AncestorComponent<dataModelsInterfaces.JourFerie> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("JourFerieComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("D", FormControlType.DatePicker, true, 10085);
  }
}

@Component({
    selector: "message-type",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class MessageTypeComponent extends AncestorComponent<dataModelsInterfaces.MessageType> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("MessageTypeComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "mode-transport-EE",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class ModeTransportEEComponent extends AncestorComponent<dataModelsInterfaces.ModeTransportEE> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("ModeTransportEEComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "motif-anomalie-chargement",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class MotifAnomalieChargementComponent extends AncestorComponent<dataModelsInterfaces.MotifAnomalieChargement> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("MotifAnomalieChargementComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
    //Specific lax length
    this.boxes[0].Controls[0].maxlength = 100;
  }
}

@Component({
    selector: "motif-anomalie-client",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class MotifAnomalieClientComponent extends AncestorComponent<dataModelsInterfaces.MotifAnomalieClient> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("MotifAnomalieClientComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
    //Specific lax length
    this.boxes[0].Controls[0].maxlength = 100;
  }
}

@Component({
    selector: "motif-anomalie-transporteur",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class MotifAnomalieTransporteurComponent extends AncestorComponent<dataModelsInterfaces.MotifAnomalieTransporteur> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("MotifAnomalieTransporteurComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
    //Specific lax length
    this.boxes[0].Controls[0].maxlength = 100;
  }
}

@Component({
    selector: "motif-camion-incomplet",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class MotifCamionIncompletComponent extends AncestorComponent<dataModelsInterfaces.MotifCamionIncomplet> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("MotifCamionIncompletComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "pays",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class PaysComponent extends AncestorComponent<dataModelsInterfaces.Pays> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("PaysComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("Actif", FormControlType.Checkbox, false, 651);
    this.CreateControl("Libelle", FormControlType.Texte, true, 10002);
    this.CreateControl("LibelleCourt", FormControlType.Texte, true, 10080).maxlength = 2;
    this.CreateControl("RefRegionReporting", FormControlType.List, false, 10073).listContent = ListContentType.RegionReportingRef;
  }
}

@Component({
    selector: "process",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class ProcessComponent extends AncestorComponent<dataModelsInterfaces.Process> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("ProcessComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "produit-groupe-reporting-type",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class ProduitGroupeReportingTypeComponent extends AncestorComponent<dataModelsInterfaces.ProduitGroupeReportingType> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("ProduitGroupeReportingTypeComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "produit-groupe-reporting",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class ProduitGroupeReportingComponent extends AncestorComponent<dataModelsInterfaces.ProduitGroupeReporting> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("ProduitGroupeReportingComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("Libelle", FormControlType.Texte, true, 474);
    this.CreateControl("RefProduitGroupeReportingType", FormControlType.List, false, 1039).listContent = ListContentType.ProduitGroupeReportingTypeRef;
    this.CreateControl("Couleur", FormControlType.List, false, 1284).listContent = ListContentType.Couleur;
  }
}

@Component({
    selector: "region-reporting",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class RegionReportingComponent extends AncestorComponent<dataModelsInterfaces.RegionReporting> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("RegionReportingComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "repreneur",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class RepreneurComponent extends AncestorComponent<dataModelsInterfaces.Repreneur> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("RepreneurComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing);
    this.CreateControl("Libelle", FormControlType.Texte, true, 1132);
    this.CreateControl("RepreneurContrat", FormControlType.Checkbox, false, 1293);
  }
}

@Component({
    selector: "reprise-type",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class RepriseTypeComponent extends AncestorComponent<dataModelsInterfaces.RepriseType> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("RepriseTypeComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "service",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class ServiceComponent extends AncestorComponent<dataModelsInterfaces.Service> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("ServiceComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "standard",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class StandardComponent extends AncestorComponent<dataModelsInterfaces.Standard> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("StandardComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}

@Component({
    selector: "ticket",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class TicketComponent extends AncestorComponent<dataModelsInterfaces.Ticket> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("TicketComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
  initControls() {
    this.CreateBox(this.ressLibelSing).boxClass = "boxDoubleSize";
    this.CreateControl("Pays", FormControlType.List, true, 10002).listContent = ListContentType.Pays;
    this.CreateControl("Libelle", FormControlType.Texte, true, 10051).maxlength = 200;
    let ctrl = this.CreateControl("TicketTexte", FormControlType.TexteMulti, true, 10084);
    ctrl.style = "height: 500px;";
    ctrl.maxlength = 50000;
  }
}

@Component({
    selector: "titre",
    templateUrl: "../_ancestors/ancestor.component.html",
    standalone: false
})
export class TitreComponent extends AncestorComponent<dataModelsInterfaces.Titre> {
  constructor(protected activatedRoute: ActivatedRoute, protected router: Router,
    protected fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , protected dataModelService: DataModelService
    , protected listService: ListService
    , protected snackBarQueueService: SnackBarQueueService
    , public dialog: MatDialog) {
    super("TitreComponent", activatedRoute, router, fb, applicationUserContext, dataModelService, listService, snackBarQueueService, dialog);
  }
}
