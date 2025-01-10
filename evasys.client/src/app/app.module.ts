import { NgModule, APP_INITIALIZER, LOCALE_ID, isDevMode } from "@angular/core";
import { CommonModule, APP_BASE_HREF } from "@angular/common";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { SwUpdate } from '@angular/service-worker';
import { MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS } from "@angular/material/core";
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatBadgeModule } from "@angular/material/badge";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatDialogModule } from "@angular/material/dialog";
import { MatFormFieldModule, MAT_FORM_FIELD_DEFAULT_OPTIONS } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatMenuModule } from "@angular/material/menu";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatRadioModule } from "@angular/material/radio";
import { MatSelectModule } from "@angular/material/select";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { MatButtonToggleModule } from "@angular/material/button-toggle";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatTableModule } from "@angular/material/table";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatDividerModule } from "@angular/material/divider";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatIconModule } from "@angular/material/icon";
import { MatRippleModule } from '@angular/material/core';
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSortModule } from "@angular/material/sort";
import { MatStepperModule } from "@angular/material/stepper";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatChipsModule } from '@angular/material/chips';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { OverlayModule } from '@angular/cdk/overlay';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from "ngx-mask"
import { NgxMatSelectSearchModule } from "ngx-mat-select-search";
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import { AccueilCollectiviteComponent } from "./components/module-collectivite/accueil-collectivite/accueil-collectivite.component";
import { ActionComponent } from "./components/annuaire/action/action.component";
import { ActionTypeComponent } from "./components/administration/action-type/action-type.component";
import { AdresseSimpleComponent } from "./components/annuaire/adresse-simple/adresse-simple.component";
import { AdresseTypeComponent } from "./components/administration/administration-components";
import { AdresseComponent } from "./components/annuaire/adresse/adresse.component";
import { AideComponent } from "./components/administration/aide/aide.component";
import { AideConteneurComponent } from "./components/global/aide/aide-conteneur.component";
import { AideBoutonComponent } from "./components/global/aide/aide-bouton.component";
import { AppComponent } from "./components/app/app.component";
import { ApplicationComponent } from "./components/administration/administration-components";
import { ApplicationProduitOrigineComponent } from "./components/administration/administration-components";
import { CamionTypeComponent } from "./components/administration/administration-components";
import { CibleComponent } from "./components/logistique/cible/cible.component";
import { CiviliteComponent } from "./components/administration/administration-components";
import { ClientApplicationComponent } from "./components/administration/client-application/client-application.component";
import { ComponentRelativeComponent } from "./components/dialogs/component-relative/component-relative.component";
import { CommandeClientComponent } from "./components/logistique/commande-client/commande-client.component";
import { CommandeFournisseurComponent } from "./components/logistique/commande-fournisseur/commande-fournisseur.component";
import { ConfirmComponent } from "./components/dialogs/confirm/confirm.component";
import { ContactAdresseComponent } from "./components/annuaire/contact-adresse/contact-adresse.component";
import { ContactAdresseServiceFonctionComponent } from "./components/annuaire/contact-adresse-service-fonction/contact-adresse-service-fonction.component";
import { ContactAdresseSimpleComponent } from "./components/annuaire/contact-adresse-simple/contact-adresse-simple.component";
import { ContratComponent } from "./components/annuaire/contrat/contrat.component";
import { ContratCollectiviteComponent } from "./components/annuaire/contrat-collectivite/contrat-collectivite.component";
import { ContratIncitationQualiteComponent } from "./components/annuaire/contrat-incitation-qualite/contrat-incitation-qualite.component";
import { ControleComponent } from "./components/qualite/controle/controle.component";
import { CVQComponent } from "./components/qualite/cvq/cvq.component";
import { RessourceComponent } from "./components/administration/administration-components";
import { DescriptionControleContainerComponent } from "./components/administration/description-controle-container/description-controle-container.component";
import { DescriptionControleComponent } from "./components/administration/description-controle/description-controle.component";
import { DescriptionCVQContainerComponent } from "./components/administration/description-cvq-container/description-cvq-container.component";
import { DescriptionCVQComponent } from "./components/administration/description-cvq/description-cvq.component";
import { DescriptionReceptionContainerComponent } from "./components/administration/description-reception-container/description-reception-container.component";
import { DescriptionReceptionComponent } from "./components/administration/description-reception/description-reception.component";
import { DocumentComponent } from "./components/administration/document/document.component";
import { DocumentEditComponent } from "./components/logistique/document-edit/document-edit.component";
import { DocumentEntiteComponent } from "./components/annuaire/document-entite/document-entite.component";
import { DocumentsComponent } from "./components/annuaire/documents/documents.component";
import { EcoOrganismeComponent } from "./components/administration/administration-components";
import { EmailComponent } from "./components/email/email.component";
import { EntiteComponent } from "./components/annuaire/entite/entite.component";
import { EntiteCamionTypeComponent } from "./components/annuaire/entite-camion-type/entite-camion-type.component";
import { EntiteEntiteComponent } from "./components/annuaire/entite-entite/entite-entite.component";
import { EntiteProduitComponent } from "./components/annuaire/entite-produit/entite-produit.component";
import { EntiteTypeComponent } from "./components/administration/administration-components";
import { EnvControlComponent } from "./components/_ancestors/env-control.component";
import { EnvMenuComponent } from "./components/_ancestors/env-menu.component";
import { EnvoiEmailComponent } from "./components/annuaire/envoi-email/envoi-email.component";
import { EquivalentCO2Component } from "./components/administration/equivalentco2/equivalentco2.component";
import { EquipementierComponent } from "./components/administration/administration-components";
import { ErrorComponent } from "./components/error/error.component";
import { ExportSAGEComponent } from "./components/logistique/export-sage/export-sage.component";
import { FicheControleComponent } from "./components/qualite/fiche-controle/fiche-controle.component";
import { FonctionComponent } from "./components/administration/administration-components";
import { ForbiddenComponent } from "./components/forbidden/forbidden.component";
import { FormeContactComponent } from "./components/administration/administration-components";
import { FournisseurTOComponent } from "./components/administration/administration-components";
import { GridComponent } from "./components/global/grid/grid.component";
import { GridSimpleComponent } from "./components/global/grid-simple/grid-simple.component";
import { HomeComponent } from "./components/home/home.component";
import { ImageGalleryComponent } from "./components/global/image-gallery/image-gallery.component";
import { InformationComponent } from "./components/dialogs/information/information.component";
import { InputFormComponent } from "./components/dialogs/input-form/input-form.component";
import { JourFerieComponent } from "./components/administration/administration-components";
import { LoginComponent } from "./components/login/login.component";
import { MyContactsComponent } from "./components/annuaire/my-contacts/my-contacts.component";
import { MessageComponent } from "./components/messagerie/message/message.component";
import { MessageTypeComponent } from "./components/administration/administration-components";
import { MessageVisuComponent } from "./components/dialogs/message-visu/message-visu.component";
import { MessageVisualisationsComponent } from "./components/dialogs/message-visualisations/message-visualisations.component";
import { ModeTransportEEComponent } from "./components/administration/administration-components";
import { MontantIncitationQualiteComponent } from "./components/administration/montant-incitation-qualite/montant-incitation-qualite.component";
import { MotifCamionIncompletComponent } from "./components/administration/administration-components";
import { MotifAnomalieChargementComponent } from "./components/administration/administration-components";
import { MotifAnomalieClientComponent } from "./components/administration/administration-components";
import { MotifAnomalieTransporteurComponent } from "./components/administration/administration-components";
import { NonConformiteComponent } from "./components/qualite/non-conformite/non-conformite.component";
import { NonConformiteDemandeClientTypeComponent } from "./components/administration/non-conformite-demande-client-type/non-conformite-demande-client-type.component";
import { NonConformiteFamilleComponent } from "./components/administration/non-conformite-famille/non-conformite-famille.component";
import { NonConformiteNatureComponent } from "./components/administration/non-conformite-nature/non-conformite-nature.component";
import { NonConformiteReponseClientTypeComponent } from "./components/administration/non-conformite-reponse-client-type/non-conformite-reponse-client-type.component";
import { NonConformiteReponseFournisseurTypeComponent } from "./components/administration/non-conformite-reponse-fournisseur-type/non-conformite-reponse-fournisseur-type.component";
import { ParamEmailComponent } from "./components/administration/param-email/param-email.component";
import { ParametreComponent } from "./components/administration/parametre/parametre.component";
import { PaysComponent } from "./components/administration/administration-components";
import { ProduitComponent } from "./components/administration/produit/produit.component";
import { ProduitGroupeReportingTypeComponent } from "./components/administration/administration-components";
import { ProduitGroupeReportingComponent } from "./components/administration/administration-components";
import { PrintableActionComponent } from "./components/printable/printable-action/printable-action.component";
import { ProfilsComponent } from "./components/login/profils.component";
import { ProgressBarComponent } from "./components/global/progress-bar/progress-bar.component";
import { ProgressSpinnerComponent } from "./components/global/progress-spinner/progress-spinner.component";
import { PrixRepriseComponent } from "./components/logistique/prix-reprise/prix-reprise.component";
import { PrixReprisesComponent } from "./components/module-collectivite/prix-reprises/prix-reprises.component";
import { ProcessComponent } from "./components/administration/administration-components";
import { RegionReportingComponent } from "./components/administration/administration-components";
import { RegionEEComponent } from "./components/administration/region-ee/region-ee.component";
import { RepartitionComponent } from "./components/logistique/repartition/repartition.component";
import { RepreneurComponent } from "./components/administration/administration-components";
import { RepriseTypeComponent } from "./components/administration/administration-components";
import { SecuriteComponent } from "./components/administration/securite/securite.component";
import { SAGECodeTransportComponent } from "./components/administration/administration-components";
import { StatistiqueGridComponent } from "./components/statistiques/statistique-grid/statistique-grid.component";
import { StandardComponent } from "./components/administration/administration-components";
import { SurcoutCarburantComponent } from "./components/logistique/surcout-carburant/surcout-carburant.component";
import { SearchResultComponent } from "./components/search-result/search-result.component";
import { ServiceComponent } from "./components/administration/administration-components";
import { TicketComponent } from "./components/administration/administration-components";
import { TicketsComponent } from "./components/dialogs/ticket/tickets.component";
import { TitreComponent } from "./components/administration/administration-components";
import { TransportComponent } from "./components/logistique/transport/transport.component";
import { TransportConcurrenceComponent } from "./components/dialogs/transport-concurrence/transport-concurrence.component";
import { TransportSimpleComponent } from "./components/logistique/transport-simple/transport-simple.component";
import { UploadTransportComponent } from "./components/logistique/upload-transport/upload-transport.component";
import { UploadComponent } from "./components/dialogs/upload/upload.component";
import { UtilisateurComponent } from "./components/administration/utilisateur/utilisateur.component";
import { UtilisateurSimpleComponent } from "./components/administration/utilisateur-simple/utilisateur-simple.component";
import { AuthInterceptor } from "./services/auth.interceptor.service";
import { AuthResponseInterceptor } from "./services/auth.response.interceptor.service";
import { AuthService } from "./services/auth.service";
import { DataModelService } from "./services/data-model.service";
import { DownloadService } from "./services/download.service";
import { GridService } from "./services/grid.service";
import { GridSimpleService } from "./services/grid-simple.service";
import { ListService } from "./services/list.service";
import { OverlayService } from "./services/overlay.service";
import { PictureService } from "./services/picture.service";
import { StatistiqueService } from "./services/statistique.service";
import { UploadService } from "./services/upload.service";
import { UtilsService } from "./services/utils.service";
import { ApplicationUserContext } from "./globals/globals";
import { MomentDateAdapter, MAT_MOMENT_DATE_FORMATS, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from "@angular/material-moment-adapter";
import { ChartsModule } from '@progress/kendo-angular-charts';
import { EditorModule } from "@progress/kendo-angular-editor";
import { ScrollViewModule } from "@progress/kendo-angular-scrollview";
import { PDFExportModule } from "@progress/kendo-angular-pdf-export";
import { CallbackPipe } from "./pipes/callback.pipe";
import { SafeHtmlPipe } from "./pipes/safehtml.pipe";
import { BlockCopyPasteDirective } from "./directives/app-block-copy-paste.directive";
import "hammerjs";
import { registerLocaleData } from '@angular/common';
import localeFr from '@angular/common/locales/fr';
import { IntlModule } from "@progress/kendo-angular-intl";
import "@progress/kendo-angular-intl/locales/fr/all";
import { BackButtonDisableModule } from 'angular-disable-browser-back-button';
import "moment/locale/fr";
import "moment/locale/en-gb";
import { environment } from "../environments/environment";
import { ServiceWorkerModule } from '@angular/service-worker';
registerLocaleData(localeFr);

export function configurationParametres(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.initParametres();
  };
}
export function configurationCulturedRessources(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.initCulturedRessources();
  };
}
export function configurationResources(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.initResources();
  };
}
export function configurationEnvModules(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.initEnvModules();
  };
}
export function configurationEnvComponents(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.initEnvComponents();
  };
}
export function configurationEnvMenus(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.initEnvMenus();
  };
}
export function configurationEnvDataColumns(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.initEnvDataColumns();
  };
}
export function configurationEnvCommandeFournisseurStatuts(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.initEnvCommandeFournisseurStatuts();
  };
}
export function configurationNonConformiteEtapeTypes(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.initNonConformiteEtapeTypes();
  };
}
export function configurationInitEnvironment(config: ApplicationUserContext) {
  return (): Promise<any> => {
    return config.getEnvironment();
  };
}

const modules = [
  MatFormFieldModule, MatInputModule, MatBadgeModule, MatButtonModule, MatTooltipModule, MatCardModule, MatMenuModule, MatIconModule, MatToolbarModule
  , MatSidenavModule, MatExpansionModule, MatCheckboxModule, MatTableModule, MatPaginatorModule, MatSortModule, MatProgressBarModule, MatProgressSpinnerModule
  , MatDividerModule, MatDialogModule, MatSnackBarModule, MatSelectModule, MatListModule, MatProgressBarModule, MatDatepickerModule, MatRadioModule
  , MatAutocompleteModule, MatRippleModule, MatSlideToggleModule, MatButtonToggleModule, MatStepperModule, MatChipsModule
  , DragDropModule, NgxMatSelectSearchModule, NgxMaskDirective, NgxMaskPipe, OverlayModule, BackButtonDisableModule
  ];
const progressModules = [
  EditorModule, ScrollViewModule, ChartsModule, ScrollingModule
];

@NgModule({
  imports: [...modules],
  exports: [...modules]
}) export class MaterialModule { };

@NgModule({
  imports: [...progressModules],
  exports: [...progressModules]
}) export class KendoModule { };


@NgModule({
  declarations: [
    AccueilCollectiviteComponent,
    ActionComponent,
    ActionTypeComponent,
    AdresseComponent,
    AdresseSimpleComponent,
    AdresseTypeComponent,
    AideComponent,
    AideConteneurComponent,
    AideBoutonComponent,
    AppComponent,
    ApplicationComponent,
    ApplicationProduitOrigineComponent,
    CamionTypeComponent,
    CibleComponent,
    CiviliteComponent,
    ClientApplicationComponent,
    CommandeClientComponent,
    CommandeFournisseurComponent,
    ComponentRelativeComponent,
    ConfirmComponent,
    ContactAdresseComponent,
    ContactAdresseServiceFonctionComponent,
    ContactAdresseSimpleComponent,
    ContratComponent,
    ContratCollectiviteComponent,
    ContratIncitationQualiteComponent,
    ControleComponent,
    RessourceComponent,
    CVQComponent,
    DescriptionControleContainerComponent,
    DescriptionControleComponent,
    DescriptionCVQContainerComponent,
    DescriptionCVQComponent,
    DescriptionReceptionComponent,
    DescriptionReceptionContainerComponent,
    DocumentComponent,
    DocumentEditComponent,
    DocumentEntiteComponent,
    DocumentsComponent,
    EcoOrganismeComponent,
    EmailComponent,
    EntiteComponent,
    EntiteCamionTypeComponent,
    EntiteEntiteComponent,
    EntiteProduitComponent,
    EntiteTypeComponent,
    EnvoiEmailComponent,
    EquipementierComponent,
    EquivalentCO2Component,
    EnvControlComponent,
    EnvMenuComponent,
    ErrorComponent,
    ExportSAGEComponent,
    FicheControleComponent,
    FonctionComponent,
    ForbiddenComponent,
    FormeContactComponent,
    FournisseurTOComponent,
    HomeComponent,
    ImageGalleryComponent,
    InformationComponent,
    InputFormComponent,
    JourFerieComponent,
    LoginComponent,
    GridComponent,
    GridSimpleComponent,
    MessageComponent,
    MessageTypeComponent,
    MessageVisuComponent,
    MessageVisualisationsComponent,
    ModeTransportEEComponent,
    MontantIncitationQualiteComponent,
    MotifAnomalieChargementComponent,
    MotifAnomalieClientComponent,
    MotifAnomalieTransporteurComponent,
    MotifCamionIncompletComponent,
    MyContactsComponent,
    NonConformiteComponent,
    NonConformiteDemandeClientTypeComponent,
    NonConformiteFamilleComponent,
    NonConformiteNatureComponent,
    NonConformiteReponseClientTypeComponent,
    NonConformiteReponseFournisseurTypeComponent,
    ParamEmailComponent,
    ParametreComponent,
    PaysComponent,
    ProduitComponent,
    ProduitGroupeReportingTypeComponent,
    ProduitGroupeReportingComponent,
    PrixRepriseComponent,
    PrixReprisesComponent,
    PrintableActionComponent,
    ProcessComponent,
    ProfilsComponent,
    ProgressBarComponent,
    ProgressSpinnerComponent,
    RegionEEComponent,
    RegionReportingComponent,
    RepartitionComponent,
    RepreneurComponent,
    RepriseTypeComponent,
    SAGECodeTransportComponent,
    SecuriteComponent,
    SearchResultComponent,
    ServiceComponent,
    StandardComponent,
    SurcoutCarburantComponent,
    TicketComponent,
    TicketsComponent,
    TitreComponent,
    TransportComponent,
    TransportConcurrenceComponent,
    TransportSimpleComponent,
    UploadComponent,
    UploadTransportComponent,
    UtilisateurComponent,
    UtilisateurSimpleComponent,
    StatistiqueGridComponent,
    CallbackPipe,
    SafeHtmlPipe,
    BlockCopyPasteDirective,
  ],
  imports: [CommonModule,
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    KendoModule,
    IntlModule,
    PDFExportModule,
    BackButtonDisableModule.forRoot(),
    //ServiceWorkerModule.register('ngsw-worker.js', {
    //  //enabled: true,
    //  //enabled: false,
    //  enabled: environment.production,
    //  // Register the ServiceWorker as soon as the application is stable
    //  // or after 30 seconds (whichever comes first).
    //  registrationStrategy: 'registerImmediately'
    //}),
    RouterModule.forRoot([
      { path: "", redirectTo: "home", pathMatch: "full" },
      { path: "accueil-collectivite", component: AccueilCollectiviteComponent, data: { animation: "FadeIn" } },
      { path: "action-type/:id", component: ActionTypeComponent, data: { animation: "Item" } },
      { path: "adressetype/:id", component: AdresseTypeComponent, data: { animation: "Item" } },
      { path: "application-produit-origine/:id", component: ApplicationProduitOrigineComponent, data: { animation: "Item" } },
      { path: "aide/:id", component: AideComponent, data: { animation: "Item" } },
      { path: "application/:id", component: ApplicationComponent, data: { animation: "Item" } },
      { path: "camion-type/:id", component: CamionTypeComponent, data: { animation: "Item" } },
      { path: "civilite/:id", component: CiviliteComponent, data: { animation: "Item" } },
      { path: "client-application/:id", component: ClientApplicationComponent, data: { animation: "Item" } },
      { path: "commande-client/:id", component: CommandeClientComponent, data: { animation: "Item" } },
      { path: "commande-fournisseur/:id", component: CommandeFournisseurComponent, data: { animation: "Item" } },
      { path: "ressource/:id", component: RessourceComponent, data: { animation: "Item" } },
      { path: "description-controle-container/:refProduit", component: DescriptionControleContainerComponent, data: { animation: "Item" } },
      { path: "description-cvq-container/:refProduit", component: DescriptionCVQContainerComponent, data: { animation: "Item" } },
      { path: "description-reception-container", component: DescriptionReceptionContainerComponent, data: { animation: "Item" } },
      { path: "document/:id", component: DocumentComponent, data: { animation: "FadeIn" } },
      { path: "document-edit", component: DocumentEditComponent, data: { animation: "FadeIn" } },
      { path: "documents/:documentsType", component: DocumentsComponent, data: { animation: "FadeIn" } },
      { path: "eco-organisme/:id", component: EcoOrganismeComponent, data: { animation: "Item" } },
      { path: "entite/:id", component: EntiteComponent, data: { animation: "Item" } },
      { path: "envoi-email/:documentType", component: EnvoiEmailComponent, data: { animation: "FadeIn" } },
      { path: "equipementier/:id", component: EquipementierComponent, data: { animation: "Item" } },
      { path: "equivalentco2/:id", component: EquivalentCO2Component, data: { animation: "Item" } },
      { path: "entite-type/:id", component: EntiteTypeComponent, data: { animation: "Item" } },
      { path: "error", component: ErrorComponent, data: { animation: "FadeIn" } },
      { path: "export-sage", component: ExportSAGEComponent, data: { animation: "Item" } },
      { path: "fiche-controle/:id", component: FicheControleComponent, data: { animation: "Item" } },
      { path: "fonction/:id", component: FonctionComponent, data: { animation: "Item" } },
      { path: "forbidden", component: ForbiddenComponent, data: { animation: "FadeIn" } },
      { path: "forme-contact/:id", component: FormeContactComponent, data: { animation: "Item" } },
      { path: "fournisseurto/:id", component: FournisseurTOComponent, data: { animation: "Item" } },
      { path: "home", component: HomeComponent, data: { animation: "FadeIn" } },
      { path: "jour-ferie/:id", component: JourFerieComponent, data: { animation: "Item" } },
      { path: "login", component: LoginComponent, data: { animation: "FadeIn" } },
      { path: "grid", component: GridComponent, data: { animation: "List" } },
      { path: "image-gallery", component: ImageGalleryComponent, data: { animation: "FadeIn" } },
      { path: "my-contacts", component: MyContactsComponent, data: { animation: "FadeIn" } },
      { path: "message/:id", component: MessageComponent, data: { animation: "Item" } },
      { path: "message-type/:id", component: MessageTypeComponent, data: { animation: "Item" } },
      { path: "mode-transport-EE/:id", component: ModeTransportEEComponent, data: { animation: "Item" } },
      { path: "montant-incitation-qualite/:id", component: MontantIncitationQualiteComponent, data: { animation: "Item" } },
      { path: "motif-anomalie-chargement/:id", component: MotifAnomalieChargementComponent, data: { animation: "Item" } },
      { path: "motif-anomalie-client/:id", component: MotifAnomalieClientComponent, data: { animation: "Item" } },
      { path: "motif-anomalie-transporteur/:id", component: MotifAnomalieTransporteurComponent, data: { animation: "Item" } },
      { path: "motif-camion-incomplet/:id", component: MotifCamionIncompletComponent, data: { animation: "Item" } },
      { path: "non-conformite/:id", component: NonConformiteComponent, data: { animation: "Item" } },
      { path: "non-conformite-demande-client-type/:id", component: NonConformiteDemandeClientTypeComponent, data: { animation: "Item" } },
      { path: "non-conformite-famille/:id", component: NonConformiteFamilleComponent, data: { animation: "Item" } },
      { path: "non-conformite-nature/:id", component: NonConformiteNatureComponent, data: { animation: "Item" } },
      { path: "non-conformite-reponse-client-type/:id", component: NonConformiteReponseClientTypeComponent, data: { animation: "Item" } },
      { path: "non-conformite-reponse-fournisseur-type/:id", component: NonConformiteReponseFournisseurTypeComponent, data: { animation: "Item" } },
      { path: "param-email/:id", component: ParamEmailComponent, data: { animation: "Item" } },
      { path: "parametre/:id", component: ParametreComponent, data: { animation: "Item" } },
      { path: "pays/:id", component: PaysComponent, data: { animation: "Item" } },
      { path: "prix-reprise/:id", component: PrixRepriseComponent, data: { animation: "Item" } },
      { path: "prix-reprises", component: PrixReprisesComponent, data: { animation: "FadeIn" } },
      { path: "process/:id", component: ProcessComponent, data: { animation: "Item" } },
      { path: "produit/:id", component: ProduitComponent, data: { animation: "Item" } },
      { path: "produit-groupe-reporting-type/:id", component: ProduitGroupeReportingTypeComponent, data: { animation: "Item" } },
      { path: "produit-groupe-reporting/:id", component: ProduitGroupeReportingComponent, data: { animation: "Item" } },
      { path: "profils", component: ProfilsComponent, data: { animation: "FadeIn" } },
      { path: "region-ee/:id", component: RegionEEComponent, data: { animation: "Item" } },
      { path: "region-reporting/:id", component: RegionReportingComponent, data: { animation: "Item" } },
      { path: "repartition/:id", component: RepartitionComponent, data: { animation: "Item" } },
      { path: "repreneur/:id", component: RepreneurComponent, data: { animation: "Item" } },
      { path: "reprise-type/:id", component: RepriseTypeComponent, data: { animation: "Item" } },
      { path: "securite", component: SecuriteComponent, data: { animation: "Item" } },
      { path: "service/:id", component: ServiceComponent, data: { animation: "Item" } },
      { path: "sage-code-transport/:id", component: SAGECodeTransportComponent, data: { animation: "Item" } },
      { path: "standard/:id", component: StandardComponent, data: { animation: "Item" } },
      { path: "statistique-grid", component: StatistiqueGridComponent, data: { animation: "FadeIn" } },
      { path: "surcout-carburant/:id", component: SurcoutCarburantComponent, data: { animation: "Item" } },
      { path: "ticket/:id", component: TicketComponent, data: { animation: "Item" } },
      { path: "titre/:id", component: TitreComponent, data: { animation: "Item" } },
      { path: "transport/:id", component: TransportComponent, data: { animation: "Item" } },
      { path: "upload-transport", component: UploadTransportComponent, data: { animation: "FadeIn" } },
      { path: "utilisateur/:id", component: UtilisateurComponent, data: { animation: "Item" } },
      { path: "**", redirectTo: "home", data: { animation: "FadeIn" } }
    ], {})],
  providers: [
    AuthService,
    DataModelService,
    DownloadService,
    GridService,
    GridSimpleService,
    ListService,
    OverlayService,
    PictureService,
    StatistiqueService,
    UploadService,
    UtilsService,
    ApplicationUserContext,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthResponseInterceptor,
      multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationParametres, deps: [ApplicationUserContext], multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationCulturedRessources, deps: [ApplicationUserContext], multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationResources, deps: [ApplicationUserContext], multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationEnvModules, deps: [ApplicationUserContext], multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationEnvComponents, deps: [ApplicationUserContext], multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationEnvMenus, deps: [ApplicationUserContext], multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationEnvDataColumns, deps: [ApplicationUserContext], multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationEnvCommandeFournisseurStatuts, deps: [ApplicationUserContext], multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationNonConformiteEtapeTypes, deps: [ApplicationUserContext], multi: true
    },
    {
      provide: APP_INITIALIZER, useFactory: configurationInitEnvironment, deps: [ApplicationUserContext], multi: true
    },
    { provide: LOCALE_ID, useValue: "fr-FR" },
    { provide: MAT_DATE_LOCALE, useValue: "fr-FR" },
    { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: "BASE_URL", useValue: environment.baseUrl },
    provideNgxMask(),
    provideHttpClient(withInterceptorsFromDi()),
    provideMomentDateAdapter()
  ],
  bootstrap: [AppComponent]
}
)
export class AppModule {
}
