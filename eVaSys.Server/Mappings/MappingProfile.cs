/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création :24/09/2018
/// ----------------------------------------------------------------------------------------------------- 

using AutoMapper;
using eVaSys.Data;
using eVaSys.ViewModels;

namespace eVaSys.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ActionActionType, ActionActionTypeViewModel >().ReverseMap();
            CreateMap<ActionDocumentType, ActionDocumentTypeViewModel>().ReverseMap();
            CreateMap<ActionTypeEntiteType, ActionTypeEntiteTypeViewModel>().ReverseMap();
            CreateMap<ActionType, ActionTypeViewModel>().ReverseMap();
            CreateMap<Data.Action, ActionViewModel>().ReverseMap();
            CreateMap<ActionFichier, ActionFichierViewModel>().ReverseMap();
            CreateMap<ActionFichierNoFile, ActionFichierNoFileViewModel>().ReverseMap();
            CreateMap<Adresse, AdresseListViewModel>().ReverseMap();
            CreateMap<Adresse, AdresseViewModel>().ReverseMap();
            CreateMap<AdresseType, AdresseTypeViewModel>().ReverseMap();
            CreateMap<Aide, AideViewModel>().ReverseMap();
            CreateMap<Application, ApplicationViewModel>().ReverseMap();
            CreateMap<ApplicationProduitOrigine, ApplicationProduitOrigineViewModel>().ReverseMap();
            CreateMap<CamionType, CamionTypeViewModel>().ReverseMap();
            CreateMap<Civilite, CiviliteViewModel>().ReverseMap();
            CreateMap<ClientApplication, ClientApplicationViewModel>().ReverseMap();
            CreateMap<ClientApplicationApplication, ClientApplicationApplicationViewModel>().ReverseMap();
            CreateMap<CommandeClient, CommandeClientViewModel>().ReverseMap();
            CreateMap<CommandeClientMensuelle, CommandeClientMensuelleViewModel>().ReverseMap();
            CreateMap<CommandeFournisseur, CommandeFournisseurViewModel>().ReverseMap();
            CreateMap<CommandeFournisseurFichier, CommandeFournisseurFichierLightViewModel>().ReverseMap();
            CreateMap<CommandeFournisseurFichier, CommandeFournisseurFichierMediumViewModel>().ReverseMap();
            CreateMap<CommandeFournisseurFichier, CommandeFournisseurFichierFullViewModel>().ReverseMap();
            CreateMap<CommandeFournisseurStatut, CommandeFournisseurStatutViewModel>().ReverseMap();
            CreateMap<ContactAdresseContactAdresseProcess, ContactAdresseContactAdresseProcessViewModel>().ReverseMap();
            CreateMap<ContactAdresseDocumentType, ContactAdresseDocumentTypeViewModel>().ReverseMap();
            CreateMap<ContactAdresseServiceFonction, ContactAdresseServiceFonctionViewModel>().ReverseMap();
            CreateMap<ContactAdresse, ContactAdresseViewModel>().ReverseMap();
            CreateMap<ContactAdresseProcess, ContactAdresseProcessViewModel>().ReverseMap();
            CreateMap<Contact, ContactViewModel>().ReverseMap();
            CreateMap<Contrat, ContratViewModel>().ReverseMap();
            CreateMap<ContratCollectivite, ContratCollectiviteViewModel>().ReverseMap();
            CreateMap<ContratIncitationQualite, ContratIncitationQualiteViewModel>().ReverseMap();
            CreateMap<ContratType, ContratTypeViewModel>().ReverseMap();
            CreateMap<Controle, ControleViewModel>().ReverseMap();
            CreateMap<ControleDescriptionControle, ControleDescriptionControleViewModel>().ReverseMap();
            CreateMap<ControleType, ControleTypeViewModel>().ReverseMap();
            CreateMap<CVQ, CVQViewModel>().ReverseMap();
            CreateMap<CVQDescriptionCVQ, CVQDescriptionCVQViewModel>().ReverseMap();
            CreateMap<Ressource, RessourceViewModel>().ReverseMap();
            CreateMap<DescriptionControle, DescriptionControleViewModel>().ReverseMap();
            CreateMap<DescriptionControleProduit, DescriptionControleProduitViewModel>().ReverseMap();
            CreateMap<DescriptionCVQ, DescriptionCVQViewModel>().ReverseMap();
            CreateMap<DescriptionCVQProduit, DescriptionCVQProduitViewModel>().ReverseMap();
            CreateMap<DescriptionReception, DescriptionReceptionViewModel>().ReverseMap();
            CreateMap<Document, DocumentViewModel>().ReverseMap();
            CreateMap<DocumentEntite, DocumentEntiteViewModel>().ReverseMap();
            CreateMap<DocumentEntiteType, DocumentEntiteTypeViewModel>().ReverseMap();
            CreateMap<DocumentNoFile, DocumentNoFileViewModel>().ReverseMap();
            CreateMap<DocumentEntite, DocumentEntiteViewModel>().ReverseMap();
            CreateMap<DocumentType, DocumentTypeViewModel>().ReverseMap();
            CreateMap<Dpt, DptViewModel>().ReverseMap();
            CreateMap<EcoOrganisme, EcoOrganismeViewModel>().ReverseMap();
            CreateMap<EquivalentCO2, EquivalentCO2ViewModel>().ReverseMap();
            CreateMap<Email, EmailViewModel>().ReverseMap();
            CreateMap<EmailFichier, EmailFichierViewModel>().ReverseMap();
            CreateMap<EmailFichier, EmailFichierFullViewModel>().ReverseMap();
            CreateMap<Entite, EntiteListViewModel>().ReverseMap();
            CreateMap<Entite, EntiteViewModel>().ReverseMap();
            CreateMap<EntiteCamionType, EntiteCamionTypeViewModel>().ReverseMap();
            CreateMap<EntiteDR, EntiteDRViewModel>().ReverseMap();
            CreateMap<EntiteEntite, EntiteEntiteViewModel>().ReverseMap();
            CreateMap<EntiteProcess, EntiteProcessViewModel>().ReverseMap();
            CreateMap<EntiteProduit, EntiteProduitViewModel>().ReverseMap();
            CreateMap<EntiteStandard, EntiteStandardViewModel>().ReverseMap();
            CreateMap<EntiteType, EntiteTypeViewModel>().ReverseMap();
            CreateMap<Equipementier, EquipementierViewModel>().ReverseMap();
            CreateMap<Fonction, FonctionViewModel>().ReverseMap();
            CreateMap<FicheControle, FicheControleViewModel>().ReverseMap();
            CreateMap<FicheControleDescriptionReception, FicheControleDescriptionReceptionViewModel>().ReverseMap();
            CreateMap<FormeContact, FormeContactViewModel>().ReverseMap();
            CreateMap<FournisseurTO, FournisseurTOViewModel>().ReverseMap();
            CreateMap<JourFerie, JourFerieViewModel>().ReverseMap();
            CreateMap<Message, MessageViewModel>().ReverseMap();
            CreateMap<MessageDiffusion, MessageDiffusionViewModel>().ReverseMap();
            CreateMap<MessageType, MessageTypeViewModel>().ReverseMap();
            CreateMap<MessageVisualisation, MessageVisualisationViewModel>().ReverseMap();
            CreateMap<ModeTransportEE, ModeTransportEEViewModel>().ReverseMap();
            CreateMap<MontantIncitationQualite, MontantIncitationQualiteViewModel>().ReverseMap();
            CreateMap<MotifAnomalieChargement, MotifAnomalieChargementViewModel>().ReverseMap();
            CreateMap<MotifAnomalieClient, MotifAnomalieClientViewModel>().ReverseMap();
            CreateMap<MotifAnomalieTransporteur, MotifAnomalieTransporteurViewModel>().ReverseMap();
            CreateMap<MotifCamionIncomplet, MotifCamionIncompletViewModel>().ReverseMap();
            CreateMap<NonConformite, NonConformiteViewModel>().ReverseMap();
            CreateMap<NonConformiteAccordFournisseurType, NonConformiteAccordFournisseurTypeViewModel>().ReverseMap();
            CreateMap<NonConformiteDemandeClientType, NonConformiteDemandeClientTypeViewModel>().ReverseMap();
            CreateMap<NonConformiteEtape, NonConformiteEtapeViewModel>().ReverseMap();
            CreateMap<NonConformiteEtapeType, NonConformiteEtapeTypeViewModel>().ReverseMap();
            CreateMap<NonConformiteFamille, NonConformiteFamilleViewModel>().ReverseMap();
            CreateMap<NonConformiteFichier, NonConformiteFichierFullViewModel>().ReverseMap();
            CreateMap<NonConformiteFichier, NonConformiteFichierLightViewModel>().ReverseMap();
            CreateMap<NonConformiteFichier, NonConformiteFichierMediumViewModel>().ReverseMap();
            CreateMap<NonConformiteFichierType, NonConformiteFichierTypeViewModel>().ReverseMap();
            CreateMap<NonConformiteNature, NonConformiteNatureViewModel>().ReverseMap();
            CreateMap<NonConformiteNonConformiteFamille, NonConformiteNonConformiteFamilleViewModel>().ReverseMap();
            CreateMap<NonConformiteReponseClientType, NonConformiteReponseClientTypeViewModel>().ReverseMap();
            CreateMap<NonConformiteReponseFournisseurType, NonConformiteReponseFournisseurTypeViewModel> ().ReverseMap();
            CreateMap<ParamEmail, ParamEmailViewModel>().ReverseMap();
            CreateMap<ParamEmail, ParamEmailListViewModel>().ReverseMap();
            CreateMap<Parametre, ParametreViewModel>().ReverseMap();
            CreateMap<Parcours, ParcoursViewModel>().ReverseMap();
            CreateMap<Pays, PaysViewModel>().ReverseMap();
            CreateMap<Process, ProcessViewModel>().ReverseMap();
            CreateMap<PrixReprise, PrixRepriseViewModel>().ReverseMap();
            CreateMap<Produit, ProduitListViewModel>().ReverseMap();
            CreateMap<Produit, ProduitViewModel>().ReverseMap();
            CreateMap<ProduitComposant, ProduitComposantViewModel>().ReverseMap();
            CreateMap<ProduitStandard, ProduitStandardViewModel>().ReverseMap();
            CreateMap<ProduitGroupeReporting, ProduitGroupeReportingViewModel>().ReverseMap();
            CreateMap<ProduitGroupeReportingType, ProduitGroupeReportingTypeViewModel>().ReverseMap();
            CreateMap<RegionEE, RegionEEViewModel>().ReverseMap();
            CreateMap<RegionEEDpt, RegionEEDptViewModel>().ReverseMap();
            CreateMap<RegionReporting, RegionReportingViewModel>().ReverseMap();
            CreateMap<Repreneur, RepreneurViewModel>().ReverseMap();
            CreateMap<RepriseType, RepriseTypeViewModel>().ReverseMap();
            CreateMap<SAGECategorieAchat, SAGECategorieAchatViewModel>().ReverseMap();
            CreateMap<SAGECategorieVente, SAGECategorieVenteViewModel>().ReverseMap();
            CreateMap<SAGECodeTransport, SAGECodeTransportViewModel>().ReverseMap();
            CreateMap<SAGEConditionLivraison, SAGEConditionLivraisonViewModel>().ReverseMap();
            CreateMap<SAGEDocument, SAGEDocumentViewModel>().ReverseMap();
            CreateMap<SAGEDocumentArticle, SAGEDocumentArticleViewModel>().ReverseMap();
            CreateMap<SAGEDocumentCompteTVA, SAGEDocumentCompteTVA>().ReverseMap();
            CreateMap<SAGEDocumentLigne, SAGEDocumentLigneViewModel>().ReverseMap();
            CreateMap<SAGEDocumentLigneText, SAGEDocumentLigneTextViewModel>().ReverseMap();
            CreateMap<SAGEModeReglement, SAGEModeReglementViewModel>().ReverseMap();
            CreateMap<SAGEPeriodicite, SAGEPeriodiciteViewModel>().ReverseMap();
            CreateMap<Securite, SecuriteViewModel>().ReverseMap();
            CreateMap<Service, ServiceViewModel>().ReverseMap();
            CreateMap<Standard, StandardViewModel>().ReverseMap();
            CreateMap<SurcoutCarburant, SurcoutCarburantViewModel>().ReverseMap();
            CreateMap<Repartition, RepartitionViewModel>().ReverseMap();
            CreateMap<RepartitionCollectivite, RepartitionCollectiviteViewModel>().ReverseMap();
            CreateMap<RepartitionProduit, RepartitionProduitViewModel>().ReverseMap();
            CreateMap<Titre, TitreViewModel>().ReverseMap();
            CreateMap<Ticket, TicketViewModel>().ReverseMap();
            CreateMap<Transport, TransportViewModel>().ReverseMap();
            CreateMap<Utilisateur, UtilisateurViewModel>().ReverseMap();
            CreateMap<Utilisateur, UtilisateurListViewModel>().ReverseMap();
        }
    }
}
