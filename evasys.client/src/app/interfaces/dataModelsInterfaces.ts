/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 01/10/2018
/// ----------------------------------------------------------------------------------------------------- 
///
import moment from "moment";
export interface Action {
  RefAction: number;
  RefEntite: number;
  Entite: Entite;
  RefContactAdresse: number;
  ContactAdresse: ContactAdresse;
  RefFormeContact: number;
  FormeContact: FormeContact;
  RefEnvoiDocument: number;
  Libelle: string;
  DAction: string | moment.Moment;
  Email: string;
  Cmt: string;
  ActionActionTypes: ActionActionType[];
  ActionDocumentTypes: ActionDocumentType[]
  ActionFichierNoFiles: ActionFichierNoFile[]
  CreationText: string;
  ModificationText: string;
}
export interface ActionActionType {
  RefAction: number;
  ActionType: ActionType;
}
export interface ActionDocumentType {
  RefAction: number;
  DocumentType: DocumentType;
}
export interface ActionFichier {
  RefActionFichier: number;
  RefAction: number;
  Nom: string;
  Extension: string;
}
export interface ActionFichierNoFile {
  RefActionFichier: number;
  RefAction: number;
  Nom: string;
  Extension: string;
}
export interface ActionType {
  RefActionType: number;
  Libelle: string;
  DocumentType: boolean;
  ActionTypeEntiteTypes: ActionTypeEntiteType[];
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface ActionTypeEntiteType {
  RefActionType: number;
  EntiteType: EntiteType;
}
export interface Adresse {
  RefAdresse: number;
  RefEntite: number;
  Entite: Entite;
  AdresseType: AdresseType;
  Libelle: string;
  Adr1: string;
  Adr2: string;
  CodePostal: string;
  Ville: string;
  RefPays: number;
  Pays: Pays;
  Tel: string;
  Fax: string;
  SiteWeb: string;
  Email: string;
  Horaires: string;
  Cmt: string;
  Actif: boolean;
  RegionEE: RegionEE;
  ContactAdresses: ContactAdresse[];
  TexteAdresseList: string;
  TexteAdresse: string;
  CreationText: string;
  ModificationText: string;
}
export interface AdresseType {
  RefAdresseType: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Aide {
  RefAide: number;
  Composant: string;
  ValeurHTML: string;
  LibelleFRFR: string;
  LibelleENGB: string;
  CreationText: string;
  ModificationText: string;
  RefRessource: number;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface Application {
  RefApplication: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface ApplicationProduitOrigine {
  RefApplicationProduitOrigine: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface CamionType {
  RefCamionType: number;
  Libelle: string;
  ModeTransportEE: ModeTransportEE;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Civilite {
  RefCivilite: number;
  Libelle: string;
  Pays: Pays;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface ClientApplication {
  RefClientApplication: number;
  Entite: Entite | EntiteList;
  ApplicationProduitOrigine: ApplicationProduitOrigine;
  D: string | moment.Moment;
  ClientApplicationApplications: ClientApplicationApplication[];
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface ClientApplicationApplication {
  RefClientApplicationApplication: number;
  RefClientApplication: number;
  Application: Application;
  Ratio: number;
}
export interface CommandeClient {
  RefCommandeClient: number;
  Entite: Entite;
  Adresse: Adresse;
  D: string | moment.Moment;
  Produit: Produit;
  Poids: number;
  Cmt: string;
  CommandeClientMensuelles: CommandeClientMensuelle[];
  CreationText: string;
  ModificationText: string;
}
export interface CommandeClientMensuelle {
  RefCommandeClientMensuelle: number;
  RefCommandeClient: number;
  D: Date;
  Poids: number;
  PrixTonneHT: number;
  IdExt: number;
  RefExt: number;
  CertificationText: string;
  Certif: boolean;
}
export interface CommandeClientMensuelleForm {
  RefCommandeClient: number;
  RefCommandeClientMensuelle: number;
  RefContrat: number;
  RefEntite: number;
  RefAdresse: number;
  RefProduit: number;
  LibelleProduit: string;
  Cmt: string;
  D: Date;
  Poids: number;
  PrixTonneHT: number;
  IdExt: number;
  CertificationText: string;
  Certif: boolean;
}
export interface CommandeFournisseur {
  RefCommandeFournisseur: number;
  NumeroCommande: number;
  NumeroAffretement: number;
  OrdreAffretement: number;
  Entite: Entite;
  Horaires: string;
  Adresse: Adresse;
  Libelle: string;
  Adr1: string;
  Adr2: string;
  CodePostal: string;
  Ville: string;
  Pays: Pays;
  ContactAdresse: ContactAdresse;
  Civilite: Civilite;
  Prenom: string;
  Nom: string;
  Tel: string;
  TelMobile: string;
  Fax: string;
  Email: string;
  Produit: Produit;
  D: string | moment.Moment;
  DChargementPrevue: string | moment.Moment;
  HoraireChargementPrevu: string;
  ChargementAnnule: boolean;
  DChargementAnnule: string | moment.Moment;
  UtilisateurChargementAnnule: Utilisateur;
  CmtChargementAnnule: string;
  DChargement: string | moment.Moment;
  DDechargementPrevue: string | moment.Moment;
  HoraireDechargementPrevu: string;
  DDechargement: string | moment.Moment;
  PoidsChargement: number;
  NbBalleChargement: number;
  TicketPeseeChargement: boolean;
  PoidsDechargement: number;
  NbBalleDechargement: number;
  TicketPeseeDechargement: boolean;
  CamionType: CamionType;
  Transporteur: Entite;
  TransporteurContactAdresse: ContactAdresse;
  PrixTransportHT: number;
  SurcoutCarburantHT: number;
  PrixTransportSupplementHT: number;
  Km: number;
  AdresseClient: Adresse;
  CmtFournisseur: string;
  CmtClient: string;
  CmtTransporteur: string;
  PrixTonneHT: number;
  DMoisDechargementPrevu: string | moment.Moment;
  Imprime: boolean;
  ExportSAGE: boolean;
  ValideDPrevues: boolean;
  RefusCamion: boolean;
  PoidsReparti: number;
  DBlocage: string | moment.Moment;
  UtilisateurBlocage: Utilisateur;
  CmtBlocage: string;
  DAffretement: string | moment.Moment;
  CommandeFournisseurStatut: CommandeFournisseurStatut;
  CamionComplet: boolean;
  MotifCamionIncomplet: MotifCamionIncomplet;
  DChargementModif: string | moment.Moment;
  ChargementEffectue: boolean;
  DAnomalieChargement: string | moment.Moment;
  MotifAnomalieChargement: MotifAnomalieChargement;
  CmtAnomalieChargement: string;
  DTraitementAnomalieChargement: string | moment.Moment;
  DAnomalieClient: string | moment.Moment;
  MotifAnomalieClient: MotifAnomalieClient;
  CmtAnomalieClient: string;
  DTraitementAnomalieClient: string | moment.Moment;
  DAnomalieTransporteur: string | moment.Moment;
  MotifAnomalieTransporteur: MotifAnomalieTransporteur;
  CmtAnomalieTransporteur: string;
  DTraitementAnomalieTransporteur: string | moment.Moment;
  LotControle: boolean;
  DAnomalieOk: string | moment.Moment;
  UtilisateurAnomalieOk: Utilisateur;
  NonRepartissable: boolean;
  RefExt: string;
  LibExt: string;
  Prestataire: Entite;
  CommandeFournisseurFichiers: CommandeFournisseurFichier[];
  Mixte: boolean;
  MixteAndPrintable: boolean;
  LockedBy: Utilisateur;
  CreationText: string;
  ModificationText: string;
  AnomalieOkText: string;
  AllowBaseForm: boolean;
}
export interface CommandeFournisseurFichier {
  RefCommandeFournisseurFichier: number;
  RefCommandeFournisseur: number;
  Corps: BinaryType;
  Nom: string;
  Extension: string;
  CorpsBase64: string;
  VignetteBase64: string;
  MiniatureBase64: string;
}
export interface CommandeFournisseurStatut {
  RefCommandeFournisseurStatut: number;
  Libelle: string;
  Couleur: string;
}
export interface Contact {
  RefContact: number;
  Prenom: string;
  Nom: string;
  RefCivilite: number;
  Civilite: Civilite;
  CreationText: string;
  ModificationText: string;
}
export interface ContactAdresse {
  RefContactAdresse: number;
  Contact: Contact;
  RefEntite: number;
  RefAdresse: number;
  Titre: Titre;
  Tel: string;
  TelMobile: string;
  Fax: string;
  Email: string;
  Cmt: string;
  CmtServiceFonction: string;
  Actif: boolean;
  ContactAdresseContactAdresseProcesss: ContactAdresseContactAdresseProcess[];
  ContactAdresseDocumentTypes: ContactAdresseDocumentType[];
  ContactAdresseServiceFonctions: ContactAdresseServiceFonction[];
  Utilisateurs: UtilisateurList[];
  ListText: string;
  CreationText: string;
  ModificationText: string;
}
export interface ContactAdresseContactAdresseProcess {
  ContactAdresseProcess: ContactAdresseProcess;
  RefContactAdresse: number;
}
export interface ContactAdresseDocumentType {
  DocumentType: DocumentType;
  RefContactAdresse: number;
}
export interface ContactAdresseProcess {
  RefContactAdresseProcess: number;
  Libelle: string;
}
export interface ContactAdresseServiceFonction {
  RefContactAdresseServiceFonction: number;
  RefContactAdresse: number;
  Fonction: Fonction;
  Service: Service;
}
export interface ContratIncitationQualite {
  RefContratIncitationQualite: number;
  RefEntite: number;
  DDebut: string | moment.Moment;
  DFin: string | moment.Moment;
}
export interface Contrat {
  RefContrat: number;
  IdContrat: string;
  ContratType: ContratType;
  DDebut: string | moment.Moment;
  DFin: string | moment.Moment;
  ReconductionTacite : string
  Avenant: boolean;
  Cmt: string;
  ContratEntites: ContratEntite[];
}
export interface ContratEntite {
  RefContratEntite?: number;
  RefContrat: number;
  RefEntite: number;
  Entite: EntiteList;
}
export interface ContratCollectivite {
  RefContratCollectivite: number;
  RefEntite: number;
  DDebut: string | moment.Moment;
  DFin: string | moment.Moment;
  Avenant: boolean;
  Cmt: string;
}
export interface ContratType {
  RefContratType: number;
  Libelle: string;
}
export interface Controle {
  RefControle: number;
  RefFicheControle: number;
  ControleType: ControleType;
  Controleur: ContactAdresse;
  DControle: string | moment.Moment;
  Poids: number;
  Cmt: string;
  ControleDescriptionControles: ControleDescriptionControle[];
  CreationText: string;
  ModificationText: string;
  GUID: string;
  Saved: boolean;
  Deleted: boolean;
}
export interface ControleDescriptionControle {
  RefControleDescriptionControle: number;
  RefControle: number;
  DescriptionControle: DescriptionControle;
  Ordre: number;
  Poids: number;
  CalculLimiteConformite: boolean;
  Cmt: string;
  CmtObligatoire: string;
  LibelleFRFR: string;
  LibelleENGB: string;
  LibelleCultured: string;
}
export interface ControleType {
  RefControleType: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  CreationText: string;
  ModificationText: string;
}
export interface Couleur {
  RefCouleur: string;
  Libelle: string;
}
export interface CulturedRessource {
  RefCulturedRessource: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface CVQ {
  RefCVQ: number;
  RefFicheControle: number;
  Controleur: ContactAdresse;
  DCVQ: string | moment.Moment;
  Etiquette: string;
  Cmt: string;
  CVQDescriptionCVQs: CVQDescriptionCVQ[];
  CreationText: string;
  ModificationText: string;
  GUID: string;
  Saved: boolean;
  Deleted: boolean;
}
export interface CVQDescriptionCVQ {
  RefCVQDescriptionCVQ: number;
  RefCVQ: number;
  DescriptionCVQ: DescriptionCVQ;
  Nb: number;
  Ordre: number;
  LimiteBasse: number;
  LimiteHaute: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  LibelleCultured: string;
}
export interface DescriptionControle {
  RefDescriptionControle: number;
  Ordre: number;
  Actif: boolean;
  CalculLimiteConformite: boolean;
  LibelleFRFR: string;
  LibelleENGB: string;
  DescriptionControleProduits: DescriptionControleProduit[];
  CreationText: string;
  ModificationText: string;
}
export interface DescriptionControleProduit {
  RefDescriptionControleProduit: number;
  RefDescriptionControle: number;
  Produit: Produit;
}
export interface DescriptionCVQ {
  RefDescriptionCVQ: number;
  Ordre: number;
  Actif: boolean;
  LimiteBasse: number;
  LimiteHaute: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  DescriptionCVQProduits: DescriptionCVQProduit[];
  CreationText: string;
  ModificationText: string;
}
export interface DescriptionCVQProduit {
  RefDescriptionCVQProduit: number;
  RefDescriptionCVQ: number;
  Produit: Produit;
}
export interface DescriptionReception {
  RefDescriptionReception: number;
  Ordre: number;
  Actif: boolean;
  LibelleFRFR: string;
  OuiFRFR: string;
  NonFRFR: string;
  LibelleENGB: string;
  OuiENGB: string;
  NonENGB: string;
  LibelleCultured: string;
  OuiCultured: string;
  NonCultured: string;
  ReponseParDefaut: boolean;
  CreationText: string;
  ModificationText: string;
}
export interface Document {
  RefDocument: number;
  Libelle: string;
  Description: string;
  Nom: string;
  Actif: boolean;
  VisibiliteTotale: boolean;
  DDebut: string | moment.Moment;
  DFin: string | moment.Moment;
  DCreation: string | moment.Moment;
  DocumentEntites: DocumentEntite[];
  DocumentEntiteTypes: DocumentEntiteType[];
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface DocumentNoFile {
  RefDocument: number;
  Libelle: string;
  Description: string;
  Nom: string;
  Actif: boolean;
  VisibiliteTotale: boolean;
  DDebut: string | moment.Moment;
  DFin: string | moment.Moment;
  DCreation: string | moment.Moment;
  CreationText: string;
  ModificationText: string;
}
export interface DocumentEntite {
  RefDocumentEntite?: number;
  RefDocument: number;
  RefEntite: number;
  Entite: EntiteList;
  DocumentNoFile?: DocumentNoFile;
}
export interface DocumentEntiteType {
  RefDocumentEntiteType: number;
  RefDocument: number;
  RefEntiteType: number;
  EntiteType: EntiteType;
}
export interface DocumentType {
  RefDocumentType: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
}
export interface Dpt {
  RefDpt: string;
  Libelle: string;
}
export interface EcoOrganisme {
  RefEcoOrganisme: number;
  Libelle: string;
  EcoOrganismeContrat: boolean;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface EquivalentCO2 {
  RefEquivalentCO2: number;
  Libelle: string;
  Ordre: number;
  Ratio: number;
  IconeBase64: string;
  Actif: boolean;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface Email {
  RefEmail: number;
  ParamEmail: ParamEmailList;
  POPId: string;
  Entrant: boolean;
  Action: Action;
  Libelle: string;
  DEnvoi: string | moment.Moment;
  DReception: string | moment.Moment;
  EmailFrom: string;
  EmailTo: string;
  EmailSubject: string;
  EmailTextBody: string;
  EmailHTMLBody: string;
  EmailFichiers: EmailFichier[];
  CreationText: string;
  ModificationText: string;
}
export interface EmailFichier {
  Index: number;
  RefCommandeFournisseur: number;
  Nom: string;
  VisualType: string;
}
export interface Entite {
  RefEntite: number;
  EntiteType: EntiteType;
  Libelle: string;
  CodeEE: string;
  EcoOrganisme: EcoOrganisme;
  Adelphe: boolean;
  CodeValorisation: string;
  Cmt: string;
  AssujettiTVA: boolean;
  CodeTVA: string;
  SAGECodeComptable: string;
  SAGECompteTiers: string;
  SAGEConditionLivraison: SAGEConditionLivraison;
  SAGEPeriodicite: SAGEPeriodicite;
  SAGEModeReglement: SAGEModeReglement;
  SAGECategorieAchat: SAGECategorieAchat;
  SAGECategorieVente: SAGECategorieVente;
  ActionnaireProprietaire: string;
  Exploitant: string;
  Capacite: number;
  PopulationContratN: number;
  Horaires: string;
  Repreneur: Repreneur;
  RepriseType: RepriseType;
  ExportSAGE: boolean;
  VisibiliteAffretementCommun: boolean;
  AutoControle: boolean;
  Actif: boolean;
  SousContrat: boolean;
  ReconductionIncitationQualite: boolean;
  SurcoutCarburant: boolean;
  Equipementier: Equipementier;
  FournisseurTO: FournisseurTO;
  Actions: Action[];
  ContactAdresses: ContactAdresse[];
  ContratIncitationQualites: ContratIncitationQualite[];
  Contrats: Contrat[];
  ContratCollectivites: ContratCollectivite[];
  Adresses: Adresse[];
  EntiteEntites: EntiteEntite[];
  EntiteCamionTypes: EntiteCamionType[];
  EntiteDRs: EntiteDR[];
  EntiteProcesss: EntiteProcess[];
  EntiteProduits: EntiteProduit[];
  EntiteStandards: EntiteStandard[];
  DocumentEntites: DocumentEntite[];
  SAGEDocuments: SAGEDocument[];
  DimensionBalle: string;
  IdNational: string;
  LibelleCode: string
  CreationText: string;
  ModificationText: string;
}
export interface EntiteCamionType {
  RefEntiteCamionType: number;
  RefEntite: number;
  CamionType: CamionType;
  Cmt: string;
  UtilisateurCreation: Utilisateur;
  DCreation: string | moment.Moment;
  CreationText: string;
}
export interface EntiteDR {
  RefEntiteDR: number;
  RefEntite: number;
  DR: UtilisateurList;
}
export interface EntiteEntite {
  RefEntiteEntite: number;
  RefEntite: number;
  RefEntiteRtt: number;
  EntiteRtt: EntiteList;
  Actif: boolean;
  Cmt: string;
  LibelleUtilisateurCreation: string;
  DCreation: string | moment.Moment;
  CreationText: string;
  RefEntiteRttType: number;
  LibelleEntiteRtt: string;
  ActifEntiteRtt: boolean;
}
export interface EntiteList {
  RefEntite: number;
  Libelle: string;
  Actif: boolean;
  LibelleCode: string
}
export interface EntiteProcess {
  RefEntiteProcess: number;
  RefEntite: number;
  Process: Process;
}
export interface EntiteProduit {
  RefEntiteProduit: number;
  RefEntite: number;
  Produit: Produit;
  Interdit: boolean;
  Cmt: string;
  UtilisateurCreation: Utilisateur;
  DCreation: string | moment.Moment;
  CreationText: string;
}
export interface EntiteStandard {
  RefEntiteStandard: number;
  RefStandard: number;
  Standard: Standard;
}
export interface EntiteType {
  RefEntiteType: number;
  Libelle: string;
  LibelleFRFR: string;
  LibelleENGB: string;
  CodeEE: boolean;
  RefEcoOrganisme: boolean;
  Adelphe: boolean;
  CodeValorisation: boolean;
  AssujettiTVA: boolean;
  CodeTVA: boolean;
  SAGECodeComptable: boolean;
  SAGECompteTiers: boolean;
  RefSAGEConditionLivraison: boolean;
  RefSAGEPeriodicite: boolean;
  RefSAGEModeReglement: boolean;
  RefSAGECategorieAchat: boolean;
  RefSAGECategorieVente: boolean;
  ActionnaireProprietaire: boolean;
  Exploitant: boolean;
  Demarrage: boolean;
  Capacite: boolean;
  TonnageGlobal: boolean;
  PopulationContratN: boolean;
  LiaisonEntiteProduit: boolean;
  LiaisonEntiteProduitInterdit: boolean;
  Horaires: boolean;
  ContratOptimisationTransport: boolean;
  ContratCollectivite: boolean;
  RefRepreneur: boolean;
  RefRepriseType: boolean;
  UtilisateurCollectivite: boolean;
  ExportSAGE: boolean;
  Process: boolean;
  VisibiliteAffretementCommun: boolean;
  BLUnique: boolean;
  AutoControle: boolean;
  LiaisonEntiteStandard: boolean;
  SousContrat: boolean;
  LiaisonEntiteCamionType: boolean;
  IncitationQualite: boolean;
  RefEquipementier: boolean;
  RefFournisseurTO: boolean;
  DimensionBalle: boolean;
  LiaisonEntiteDR: boolean;
  SurcoutCarburant: boolean;
  IdNational: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Equipementier {
  RefEquipementier: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface FicheControle {
  RefFicheControle: number;
  CommandeFournisseur: CommandeFournisseur;
  Fournisseur: Entite;
  Produit: Produit;
  Controleur: ContactAdresse;
  NumeroLotUsine: string;
  Reserve: boolean;
  Cmt: string;
  Valide: boolean;
  FicheControleDescriptionReceptions: FicheControleDescriptionReception[];
  Controles: Controle[];
  CVQs: CVQ[];
  FicheControleText: string;
  CreationText: string;
  ModificationText: string;
}
export interface FicheControleDescriptionReception {
  RefFicheControleDescriptionReception: number;
  RefFicheControle: number;
  DescriptionReception: DescriptionReception;
  Cmt: string;
  Positif: boolean;
  Ordre: number;
  LibelleFRFR: string;
  OuiFRFR: string;
  NonFRFR: string;
  LibelleENGB: string;
  OuiENGB: string;
  NonENGB: string;
  LibelleCultured: string;
  OuiCultured: string;
  NonCultured: string;
}
export interface Fonction {
  RefFonction: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface FormeContact {
  RefFormeContact: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface FournisseurTO {
  RefFournisseurTO: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface JourFerie {
  RefJourFerie: number;
  D: string | moment.Moment;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface ModeTransportEE {
  RefModeTransportEE: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface MotifAnomalieChargement {
  RefMotifAnomalieChargement: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface MotifAnomalieClient {
  RefMotifAnomalieClient: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface MotifAnomalieTransporteur {
  RefMotifAnomalieTransporteur: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface MotifCamionIncomplet {
  RefMotifCamionIncomplet: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Message {
  RefMessage: number;
  MessageType: MessageType;
  Libelle: string;
  Titre: string;
  Corps: string;
  CorpsHTML: string;
  CorpsHTMLFromText: string;
  Cmt: string;
  DDebut: string | moment.Moment;
  DFin: string | moment.Moment;
  DiffusionUnique: boolean;
  Actif: boolean;
  Important: boolean;
  VisualisationConfirmeUnique: boolean;
  MessageDiffusions: MessageDiffusion[];
  MessageVisualisations: MessageVisualisation[];
  CreationText: string;
  ModificationText: string;
}
export interface MessageDiffusion {
  RefMessageDiffusion: number;
  RefMessage: string;
  RefModule: string;
  RefHabilitation: string;
  Utilisateur: Utilisateur;
}
export interface MessageVisualisation {
  RefMessageVisualisation: number;
  RefMessage: string;
  D: string | moment.Moment;
  Confirme: boolean;
  Utilisateur: UtilisateurList;
}
export interface MessageType {
  RefMessageType: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface MontantIncitationQualite {
  RefMontantIncitationQualite: number;
  Annee: number;
  Montant: number;
  CreationText: string;
  ModificationText: string;
}
export interface NonConformiteAccordFournisseurType {
  RefNonConformiteAccordFournisseurType: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  Libelle: string;
  Actif: boolean;
  CreationText: string;
  ModificationText: string;
}
export interface NonConformiteDemandeClientType {
  RefNonConformiteDemandeClientType: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  Libelle: string;
  Actif: boolean;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface NonConformiteEtape {
  RefNonConformiteEtape: number;
  RefNonConformite: number;
  NonConformiteEtapeType: NonConformiteEtapeType;
  Ordre: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  Libelle: string;
  Cmt: string;
  DCreation: string | moment.Moment;
  UtilisateurCreation: Utilisateur;
  DModif: string | moment.Moment;
  UtilisateurModif: Utilisateur;
  DControle: string | moment.Moment;
  UtilisateurControle: Utilisateur;
  DValide: string | moment.Moment;
  UtilisateurValide: Utilisateur;
  CreationText: string;
  ModificationText: string;
  ValidationText: string;
  ControleText: string;
}
export interface NonConformiteEtapeType {
  RefNonConformiteEtapeType: number;
  Ordre: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  Libelle: string;
  Controle: boolean;
}
export interface NonConformiteFamille {
  RefNonConformiteFamille: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  Libelle: string;
  Actif: boolean;
  IncitationQualite: boolean;
  CreationText: string;
  ModificationText: string;
}
export interface NonConformiterFichier {
  RefNonConformiteFichier: number;
  RefNonConformite: number;
  NonConformiteFichierType: NonConformiteFichierType;
  Corps: BinaryType;
  Nom: string;
  Extension: string;
  CorpsBase64: string;
  VignetteBase64: string;
  MiniatureBase64: string;
}
export interface NonConformiteFichierType {
  RefNonConformiteFichierType: number;
  Libelle: string;
}
export interface NonConformiteNonConformiteFamille {
  RefNonConformiteNonConformiteFamille: number;
  NonConformiteFamille: NonConformiteFamille;
  RefNonConformite: number;
}
export interface NonConformiteNature {
  RefNonConformiteNature: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  Libelle: string;
  Actif: boolean;
  IncitationQualite: boolean;
  CreationText: string;
  ModificationText: string;
}
export interface NonConformiteReponseClientType {
  RefNonConformiteReponseClientType: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  Libelle: string;
  Actif: boolean;
  CreationText: string;
  ModificationText: string;
}
export interface NonConformiteReponseFournisseurType {
  RefNonConformiteReponseFournisseurType: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  Libelle: string;
  Actif: boolean;
  CreationText: string;
  ModificationText: string;
}
export interface NonConformite {
  RefNonConformite: number;
  CommandeFournisseur: CommandeFournisseur;
  DescrClient: string;
  NonConformiteDemandeClientType: NonConformiteDemandeClientType;
  DescrValorplast: string;
  NonConformiteNature: NonConformiteNature;
  DTransmissionFournisseur: string | moment.Moment;
  ActionDR: string;
  NonConformiteReponseFournisseurType: NonConformiteReponseFournisseurType;
  CmtReponseFournisseur: string;
  NonConformiteReponseClientType: NonConformiteReponseClientType
  CmtReponseClient: string;
  PlanAction: boolean;
  CmtOrigineAction: string;
  PriseEnCharge: boolean;
  MontantPriseEnCharge: number;
  CmtPriseEnCharge: string;
  IFFournisseurDescr: string;
  IFFournisseurFactureMontant: number;
  IFFournisseurDeductionTonnage: number;
  IFFournisseurRetourLot: boolean;
  NonConformiteAccordFournisseurType: NonConformiteAccordFournisseurType
  IFFournisseurAttenteBonCommande: boolean;
  IFFournisseurBonCommandeNro: string;
  IFFournisseurFacture: boolean;
  IFFournisseurTransmissionFacturation: boolean;
  IFFournisseurFactureNro: string;
  IFFournisseurCmtFacturation: string;
  IFClientDescr: string;
  IFClientFactureMontant: number;
  IFClientFactureNro: string;
  IFClientCmtFacturation: string;
  IFClientDFacture: string | moment.Moment;
  IFClientCommandeAFaire: boolean;
  IFClientFactureEnAttente: boolean;
  NonConformiteNonConformiteFamilles: NonConformiteNonConformiteFamille[];
  NonConformiteFichiers: NonConformiterFichier[];
  NonConformiteEtapes: NonConformiteEtape[];
  NonConformiteTextCmd: string;
  NonConformiteTextClient: string;
  NonConformiteTextFournisseur: string;
  AllowBaseForm: boolean;
}
export interface ParamEmail {
  RefParamEmail: number;
  Actif: boolean;
  Defaut: boolean;
  RttClient: boolean;
  POP: string;
  PortPOP: number;
  ComptePOP: string;
  PwdPOP: string;
  TLSPOP: boolean;
  EnTeteEmail: string;
  PiedEmail: string;
  EmailExpediteur: string;
  LibelleExpediteur: string;
  SMTP: string;
  PortSMTP: number;
  CompteSMTP: string;
  PwdSMTP: string;
  AllowAuthSMTP: boolean;
  TLSSMTP: boolean;
  StartTLSSMTP: boolean;
  AR: boolean;
  AREmail: string;
  AREmailSujet: string;
  ARExpediteurEmail: string;
  ARExpediteurLibelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface ParamEmailList {
  RefParamEmail: number;
  EmailExpediteur: string;
  LibelleExpediteur: string;
}
export interface Parametre {
  RefParametre: number;
  Libelle: string;
  Serveur: boolean;
  ValeurNumerique: number;
  ValeurTexte: string;
  ValeurHTML: string;
  ValeurBinaireBase64: string;
  Cmt: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface Parcours {
  RefParcours: number;
  RefAdresseOrigine: number;
  AdresseOrigine: Adresse;
  RefAdresseDestination: number;
  AdresseDestination: Adresse;
  Km: number;
}
export interface Pays {
  RefPays: number;
  Actif: boolean;
  Libelle: string;
  LibelleCourt: string;
  RefRegionReporting: number;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface PrixReprise {
  RefPrixReprise: number;
  Process: Process;
  Produit: Produit;
  Composant: Produit;
  Contrat: Contrat;
  D: Date;
  PUHT: number;
  PUHTSurtri: number;
  PUHTTransport: number;
  CreationText: string;
  ModificationText: string;
  CertificationText: string;
  Certif: boolean;
}
export interface CollectivitePrixReprise {
  StandardLibelle: string;
  StandardCmt: string;
  ProduitNomCommun: string;
  PUHT: number;
}
export interface Produit {
  RefProduit: number;
  _PCodeProd: string;
  Libelle: string;
  NomCommercial: string;
  NomCommun: string;
  Collecte: boolean;
  NumeroStatistique: string;
  SAGECodeTransport: SAGECodeTransport;
  Actif: boolean;
  Repreneur: Entite;
  ApplicationProduitOrigine: ApplicationProduitOrigine;
  Composant: boolean;
  PUHTSurtri: boolean;
  PUHTTransport: boolean;
  ProduitGroupeReporting: ProduitGroupeReporting;
  CodeListeVerte: string;
  CodeEE: string;
  IncitationQualite: boolean;
  Cmt: string;
  Co2KgParT: number;
  CmtFournisseur: string;
  CmtTransporteur: string;
  CmtClient: string;
  LaserType: string;
  ProduitComposants: ProduitComposant[];
  ProduitStandards : ProduitStandard[]
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface ProduitList {
  RefProduit: number;
  Libelle: string;
}
export interface ProduitComposant {
  RefProduitComposant: number;
  RefProduit: number;
  Composant: ProduitList,
}
export interface ProduitStandard {
  RefProduitStandard: number;
  RefProduit: number;
  Standard: Standard,
}
export interface ProduitGroupeReporting {
  RefProduitGroupeReporting: number;
  Libelle: string;
  Couleur: string;
  RefProduitGroupeReportingType: number,
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface ProduitGroupeReportingList {
  RefProduitGroupeReporting: number;
  Libelle: string;
}
export interface ProduitGroupeReportingType {
  RefProduitGroupeReportingType: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Process {
  RefProcess: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface RegionEE {
  RefRegionEE: number;
  Libelle: string;
  RegionEEDpts: RegionEEDpt[];
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface RegionEEDpt {
  RefRegionEEDpt: number;
  RefRegionEE: number;
  Dpt: Dpt;
}
export interface RegionReporting {
  RefRegionReporting: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Repartition {
  RefRepartition: number;
  CommandeFournisseur: CommandeFournisseur;
  ExportSAGE: boolean;
  RepartitionCollectivites: RepartitionCollectivite[];
  RepartitionProduits: RepartitionProduit[];
  PoidsReparti: number;
  PoidsChargement: number;
  UtilisateurCreation: Utilisateur;
  InfoText: string;
  CreationText: string;
  ModificationText: string;
  AllowBaseForm: boolean;
}
export interface RepartitionProduit {
  RefRepartitionProduit: number;
  RefRepartition: number;
  Process: Process;
  Fournisseur: Entite;
  Produit: Produit;
  Poids: number;
  PUHT: number;
  Percentage: number;
}
export interface RepartitionCollectivite {
  RefRepartitionCollectivite: number;
  RefRepartition: number;
  Collectivite: Entite;
  Process: Process;
  Produit: Produit;
  Poids: number;
  PUHT: number;
  Percentage: number;
}
export interface Repreneur {
  RefRepreneur: number;
  Libelle: string;
  RepreneurContrat: boolean;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Ressource {
  RefRessource: number;
  LibelleFRFR: string;
  LibelleENGB: string;
  AllowStandardCRUD: string;
}
export interface RepriseType {
  RefRepriseType: number;
  Libelle: string;
  RepriseTypeContrat: boolean;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface SAGECategorieAchat {
  RefSAGECategorieAchat: number;
  Libelle: string;
  TVATaux: number;
}
export interface SAGECategorieVente {
  RefSAGECategorieVente: number;
  Libelle: string;
  TVATaux: number;
}
export interface SAGECodeTransport {
  RefSAGECodeTransport: number;
  Code: string;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface SAGEConditionLivraison {
  RefSAGEConditionLivraison: number;
  Libelle: string;
}
export interface SAGEDocument {
  ID: number;
  RefSAGEDocument: string;
  D: string | moment.Moment;
  CodeComptable: string;
  ConditionLivraison: string;
  TotalHT: number;
  TotalTTC: number;
  TotalNet: number;
  TotalTVA: number;
  Adr1: string;
  Adr2: string;
  Adr3: string;
  Adr4: string;
  SAGEDocumentCompteTVA: SAGEDocumentCompteTVA;
  SAGEDocumentLignes: SAGEDocumentLigne[];
  RefSAGECategorieAchat: number;
  Libelle: string;
  TVATaux: number;
  Type: string;
  Regle: boolean;
}
export interface SAGEDocumentLigne {
  ID: number;
  RefSAGEDocument: number;
  RefSAGEDocumentLigne: number;
  Quantite: number;
  LibelleDL: string;
  PUHT: number;
  TotalHT: number;
  TotalTTC: number;
  TVAMontant: number;
  TVATaux: number;
  SAGEDocumentArticle: SAGEDocumentArticle;
}
export interface SAGEDocumentArticle {
  ID: number;
  RefSAGEDocumentArticle: number;
  LibelleAR: string;
  Designation1: string;
  Designation2: string;
  SAGEDocumentLignes: SAGEDocumentLigne[];
}
export interface SAGEDocumentCompteTVA {
  RefSAGEDocumentCompteTVA: string;
  CompteTVA: string;
}
export interface SAGEModeReglement {
  RefSAGEModeReglement: number;
  Libelle: string;
  TVATaux: number;
}
export interface SAGEPeriodicite {
  RefSAGEPeriodicite: number;
  Libelle: string;
  TVATaux: number;
}
export interface Service {
  RefService: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Securite {
  RefSecurite: number;
  DelaiAvantDesactivationUtilisateur: number;
  DelaiAvantChangementMotDePasse: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowBaseForm: boolean;
}
export interface Standard {
  RefStandard: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface SurcoutCarburant {
  RefSurcoutCarburant: number;
  Transporteur: EntiteList;
  Pays: Pays;
  Annee: number;
  Mois: number;
  Ratio: number;
  AllPays: boolean;
  CreationText: string;
  ModificationText: string;
}
export interface Titre {
  RefTitre: number;
  Libelle: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Ticket {
  RefTicket: number;
  Pays: Pays;
  Libelle: string;
  TicketTexte: string;
  TicketTexteHTMLFromText: string;
  CreationText: string;
  ModificationText: string;
  AllowStandardCRUD: string;
}
export interface Transport {
  RefTransport: number;
  RefParcours: number;
  Parcours: Parcours;
  RefTransporteur: number;
  Transporteur: Entite;
  RefCamionType: number;
  CamionType: CamionType;
  PUHT: number;
  PUHTDemande: number;
  CreationText: string;
  ModificationText: string;
}
export interface Utilisateur {
  RefUtilisateur: number;
  RefUtilisateurMaitre: number;
  UtilisateurMaitre: UtilisateurList;
  RefUtilisateurAffectationMaitre: number;
  Nom: string;
  Login: string;
  Pwd: string;
  PwdExists: boolean;
  Actif: boolean;
  HabilitationQualite: string;
  Client: Entite;
  HabilitationAnnuaire: string;
  HabilitationLogistique: string;
  Transporteur: Entite;
  HabilitationStatistique: string;
  HabilitationAdministration: string;
  HabilitationModuleCentreDeTri: string;
  HabilitationModuleCollectivite: string;
  Collectivite: Entite;
  CentreDeTri: Entite;
  HabilitationMessagerie: string;
  HabilitationModulePrestataire: string;
  Prestataire: Entite;
  RefPays: number;
  EMail: string;
  PiedEmail: string;
  Tel: string;
  TelMobile: string;
  Fax: string;
  ContactAdresse: ContactAdresse;
  UtilisateurType: string;
  EntiteLibelle: string;
  HasProfils: boolean;
  CreationText: string;
  ModificationText: string;
  PwdModificationText: string;
  AffectationUtilisateurMaitreText: string;
  AllowStandardCRUD: string;
  AllowBaseForm: boolean;
}
export interface UtilisateurList {
  RefUtilisateur: number;
  RefUtilisateurMaitre: number;
  Nom: string;
  Login: string;
  Tel: string;
  TelMobile: string;
  EMail: string;
  Actif: boolean;
  UtilisateurType: string;
  EntiteLibelle: string;
  HasProfils: boolean;
}
