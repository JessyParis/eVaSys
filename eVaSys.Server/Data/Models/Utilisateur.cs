/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 04/07/2018
/// ----------------------------------------------------------------------------------------------------- 
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    public class Utilisateur : IMarkModification
    {
        #region Constructor
        public Utilisateur()
        {
            //AdresseTypeCreation = new HashSet<AdresseType>();
            //AdresseTypeModif = new HashSet<AdresseType>();
            //AdresseCreation = new HashSet<Adresse>();
            //AdresseModif = new HashSet<Adresse>();
            //CamionTypeCreation = new HashSet<CamionType>();
            //CamionTypeModif = new HashSet<CamionType>();
            //EntiteCreation = new HashSet<Entite>();
            //EntiteModif = new HashSet<Entite>();
            //EntiteTypeCreation = new HashSet<EntiteType>();
            //EntiteTypeModif = new HashSet<EntiteType>();
            //ModeTransportEECreation = new HashSet<ModeTransportEE>();
            //ModeTransportEEModif = new HashSet<ModeTransportEE>();
            //PaysCreation = new HashSet<Pays>();
            //PaysModif = new HashSet<Pays>();
            //TransportCreation = new HashSet<Transport>();
            //TransportModif = new HashSet<Transport>();
        }
        private Utilisateur(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        #endregion
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }

        public CultureInfo currentCulture = new("fr-FR");
        public int RefUtilisateur { get; set; }
        public int? RefUtilisateurMaitre { get; set; }
        public Utilisateur UtilisateurMaitre { get; set; }
        public int? RefUtilisateurAffectationMaitre { get; set; }
        public Utilisateur UtilisateurAffectationMaitre { get; set; }
        public DateTime? DAffectationMaitre { get; set; }
        public string Nom { get; set; }
        public string Login { get; set; }
        public string Pwd { get; set; }
        public byte[] PwdHash { get; set; }
        public DateTime? PwdDModif { get; set; }
        public bool Actif { get; set; } = true;
        public string HabilitationQualite { get; set; }
        public int? RefClient { get; set; }
        private Entite _client;
        public Entite Client
        {
            get => LazyLoader.Load(this, ref _client);
            set => _client = value;
        }
        public string HabilitationModuleTransporteur { get; set; }
        public int? RefTransporteur { get; set; }
        private Entite _transporteur;
        public Entite Transporteur
        {
            get => LazyLoader.Load(this, ref _transporteur);
            set => _transporteur = value;
        }
        public string HabilitationAnnuaire { get; set; }
        public string HabilitationLogistique { get; set; }
        public string HabilitationStatistique { get; set; }
        public string HabilitationBDC { get; set; }
        public string HabilitationAdministration { get; set; }
        public string HabilitationModuleCentreDeTri { get; set; }
        public string HabilitationModuleCollectivite { get; set; }
        public int? RefCentreDeTri { get; set; }
        private Entite _centreDeTri;
        public Entite CentreDeTri
        {
            get => LazyLoader.Load(this, ref _centreDeTri);
            set => _centreDeTri = value;
        }
        public int? RefCollectivite { get; set; }
        private Entite _collectivite;
        public Entite Collectivite
        {
            get => LazyLoader.Load(this, ref _collectivite);
            set => _collectivite = value;
        }
        public string HabilitationMessagerie { get; set; }
        public string HabilitationModulePrestataire { get; set; }
        public int? RefPrestataire { get; set; }
        private Entite _prestataire;
        public Entite Prestataire
        {
            get => LazyLoader.Load(this, ref _prestataire);
            set => _prestataire = value;
        }
        public int? RefPays { get; set; }
        private Pays _pays;
        public Pays Pays
        {
            get => LazyLoader.Load(this, ref _pays);
            set => _pays = value;
        }
        public string EMail { get; set; }
        public string PiedEmail { get; set; }
        public string Tel { get; set; }
        public string TelMobile { get; set; }
        public string Fax { get; set; }
        public string UidChars { get; set; }
        public int? RefContactAdresse { get; set; }
        private ContactAdresse _contactAdresse;
        public ContactAdresse ContactAdresse
        {
            get => LazyLoader.Load(this, ref _contactAdresse);
            set => _contactAdresse = value;
        }
        [NotMapped]
        public string UtilisateurType
        {
            get
            {
                string s = "";
                if (RefCollectivite > 0) { s = Enumerations.UtilisateurType.Collectivite.ToString(); }
                else if (RefTransporteur > 0) { s = Enumerations.UtilisateurType.Transporteur.ToString(); }
                else if (RefClient > 0) { s = Enumerations.UtilisateurType.Client.ToString(); }
                else if (RefCentreDeTri > 0) { s = Enumerations.UtilisateurType.CentreDeTri.ToString(); }
                else if (RefPrestataire > 0) { s = Enumerations.UtilisateurType.Prestataire.ToString(); }
                else { s = Enumerations.UtilisateurType.Valorplast.ToString(); }
                return s;
            }
        }
        [NotMapped]
        public string EntiteLibelle
        {
            get
            {
                string s = "";
                if (RefCollectivite > 0)
                {
                    if (Collectivite == null) { DbContext.Entry(this).Reference(e => e.Collectivite).Load(); }
                    s = Collectivite?.Libelle;
                    if (!string.IsNullOrWhiteSpace(Collectivite?.CodeEE)) { s += " (" + Collectivite.CodeEE + ")"; }
                }
                else if (RefTransporteur > 0)
                {
                    if (Transporteur == null) { DbContext.Entry(this).Reference(e => e.Transporteur).Load(); }
                    s = Transporteur?.Libelle;
                }
                else if (RefClient > 0)
                {
                    if (Client == null) { DbContext.Entry(this).Reference(e => e.Client).Load(); }
                    s = Client?.Libelle;
                }
                else if (RefCentreDeTri > 0)
                {
                    if (CentreDeTri == null) { DbContext.Entry(this).Reference(e => e.CentreDeTri).Load(); }
                    s = CentreDeTri?.Libelle;
                    if (!string.IsNullOrWhiteSpace(CentreDeTri?.CodeEE)) { s += " (" + CentreDeTri.CodeEE + ")"; }
                }
                else if (RefPrestataire > 0)
                {
                    if (Prestataire == null) { DbContext.Entry(this).Reference(e => e.Prestataire).Load(); }
                    s = Prestataire?.Libelle;
                }
                else { s = Enumerations.UtilisateurType.Valorplast.ToString(); }
                return s;
            }
        }
        [NotMapped]
        public bool HasProfils
        {
            get
            {
                var r = DbContext.Utilisateurs
                    .Where(e => e.RefUtilisateur!=RefUtilisateur && e.RefUtilisateurMaitre == RefUtilisateur && Actif)
                    .Count() > 0;
                return r;
            }
        }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public ICollection<EntiteDR> EntiteDRs { get; set; }
        public ICollection<Action> ActionCreations { get; set; }
        public ICollection<Action> ActionModifs { get; set; }
        public ICollection<ActionType> ActionTypeCreations { get; set; }
        public ICollection<ActionType> ActionTypeModifs { get; set; }
        public ICollection<AdresseType> AdresseTypeCreations { get; set; }
        public ICollection<AdresseType> AdresseTypeModifs { get; set; }
        public ICollection<Adresse> AdresseCreations { get; set; }
        public ICollection<Adresse> AdresseModifs { get; set; }
        public ICollection<Aide> AideCreations { get; set; }
        public ICollection<Aide> AideModifs { get; set; }
        public ICollection<APILog> APILogs { get; set; }
        public ICollection<Application> ApplicationCreations { get; set; }
        public ICollection<Application> ApplicationModifs { get; set; }
        public ICollection<ApplicationProduitOrigine> ApplicationProduitOrigineCreations { get; set; }
        public ICollection<ApplicationProduitOrigine> ApplicationProduitOrigineModifs { get; set; }
        public ICollection<CamionType> CamionTypeCreations { get; set; }
        public ICollection<CamionType> CamionTypeModifs { get; set; }
        public ICollection<ClientApplication> ClientApplicationCreations { get; set; }
        public ICollection<ClientApplication> ClientApplicationModifs { get; set; }
        public ICollection<CommandeClient> CommandeClientCreations { get; set; }
        public ICollection<CommandeClient> CommandeClientModifs { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurUtilisateurChargementAnnules { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurUtilisateurBlocages { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurUtilisateurAnomalieOks { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurCreations { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurModifs { get; set; }
        public ICollection<Contact> ContactCreations { get; set; }
        public ICollection<Contact> ContactModifs { get; set; }
        public ICollection<ContactAdresse> ContactAdresseCreations { get; set; }
        public ICollection<ContactAdresse> ContactAdresseModifs { get; set; }
        public ICollection<Controle> ControleCreations { get; set; }
        public ICollection<Controle> ControleModifs { get; set; }
        public ICollection<Civilite> CiviliteCreations { get; set; }
        public ICollection<Civilite> CiviliteModifs { get; set; }
        public ICollection<CVQ> CVQCreations { get; set; }
        public ICollection<CVQ> CVQModifs { get; set; }
        public ICollection<DescriptionControle> DescriptionControleCreations { get; set; }
        public ICollection<DescriptionControle> DescriptionControleModifs { get; set; }
        public ICollection<DescriptionCVQ> DescriptionCVQCreations { get; set; }
        public ICollection<DescriptionCVQ> DescriptionCVQModifs { get; set; }
        public ICollection<DescriptionReception> DescriptionReceptionCreations { get; set; }
        public ICollection<DescriptionReception> DescriptionReceptionModifs { get; set; }
        public ICollection<Document> DocumentCreations { get; set; }
        public ICollection<Document> DocumentModifs { get; set; }
        public ICollection<DocumentNoFile> DocumentNoFileCreations { get; set; }
        public ICollection<DocumentNoFile> DocumentNoFileModifs { get; set; }
        public ICollection<DocumentType> DocumentTypeCreations { get; set; }
        public ICollection<DocumentType> DocumentTypeModifs { get; set; }
        public ICollection<EcoOrganisme> EcoOrganismeCreations { get; set; }
        public ICollection<EcoOrganisme> EcoOrganismeModifs { get; set; }
        public ICollection<EquivalentCO2> EquivalentCO2Creations { get; set; }
        public ICollection<EquivalentCO2> EquivalentCO2Modifs { get; set; }
        public ICollection<Email> EmailCreations { get; set; }
        public ICollection<Email> EmailModifs { get; set; }
        public ICollection<Entite> EntiteCreations { get; set; }
        public ICollection<Entite> EntiteModifs { get; set; }
        public ICollection<EntiteCamionType> EntiteCamionTypeCreations { get; set; }
        public ICollection<EntiteEntite> EntiteEntiteCreations { get; set; }
        public ICollection<EntiteProduit> EntiteProduitCreations { get; set; }
        public ICollection<EntiteType> EntiteTypeCreations { get; set; }
        public ICollection<EntiteType> EntiteTypeModifs { get; set; }
        public ICollection<Equipementier> EquipementierCreations { get; set; }
        public ICollection<Equipementier> EquipementierModifs { get; set; }
        public ICollection<FicheControle> FicheControleCreations { get; set; }
        public ICollection<FicheControle> FicheControleModifs { get; set; }
        public ICollection<Fonction> FonctionCreations { get; set; }
        public ICollection<Fonction> FonctionModifs { get; set; }
        public ICollection<FormeContact> FormeContactCreations { get; set; }
        public ICollection<FormeContact> FormeContactModifs { get; set; }
        public ICollection<FournisseurTO> FournisseurTOCreations { get; set; }
        public ICollection<FournisseurTO> FournisseurTOModifs { get; set; }
        public ICollection<JourFerie> JourFerieCreations { get; set; }
        public ICollection<JourFerie> JourFerieModifs { get; set; }
        public ICollection<Lien> Liens { get; set; }
        public ICollection<Lien> LienCreations { get; set; }
        public ICollection<Log> Logs { get; set; }
        public ICollection<Message> MessageCreations { get; set; }
        public ICollection<Message> MessageModifs { get; set; }
        public ICollection<MessageType> MessageTypeCreations { get; set; }
        public ICollection<MessageType> MessageTypeModifs { get; set; }
        public ICollection<MessageDiffusion> MessageDiffusions { get; set; }
        public ICollection<MessageVisualisation> MessageVisualisations { get; set; }
        public ICollection<ModeTransportEE> ModeTransportEECreations { get; set; }
        public ICollection<ModeTransportEE> ModeTransportEEModifs { get; set; }
        public ICollection<MontantIncitationQualite> MontantIncitationQualiteCreations { get; set; }
        public ICollection<MontantIncitationQualite> MontantIncitationQualiteModifs { get; set; }
        public ICollection<MotifAnomalieChargement> MotifAnomalieChargementCreations { get; set; }
        public ICollection<MotifAnomalieChargement> MotifAnomalieChargementModifs { get; set; }
        public ICollection<MotifAnomalieClient> MotifAnomalieClientCreations { get; set; }
        public ICollection<MotifAnomalieClient> MotifAnomalieClientModifs { get; set; }
        public ICollection<MotifAnomalieTransporteur> MotifAnomalieTransporteurCreations { get; set; }
        public ICollection<MotifAnomalieTransporteur> MotifAnomalieTransporteurModifs { get; set; }
        public ICollection<MotifCamionIncomplet> MotifCamionIncompletCreations { get; set; }
        public ICollection<MotifCamionIncomplet> MotifCamionIncompletModifs { get; set; }
        public ICollection<NonConformiteAccordFournisseurType> NonConformiteAccordFournisseurTypeCreations { get; set; }
        public ICollection<NonConformiteAccordFournisseurType> NonConformiteAccordFournisseurTypeModifs { get; set; }
        public ICollection<NonConformiteDemandeClientType> NonConformiteDemandeClientTypeCreations { get; set; }
        public ICollection<NonConformiteDemandeClientType> NonConformiteDemandeClientTypeModifs { get; set; }
        public ICollection<NonConformiteFamille> NonConformiteFamilleCreations { get; set; }
        public ICollection<NonConformiteFamille> NonConformiteFamilleModifs { get; set; }
        public ICollection<NonConformiteNature> NonConformiteNatureCreations { get; set; }
        public ICollection<NonConformiteNature> NonConformiteNatureModifs { get; set; }
        public ICollection<NonConformiteReponseClientType> NonConformiteReponseClientTypeCreations { get; set; }
        public ICollection<NonConformiteReponseClientType> NonConformiteReponseClientTypeModifs { get; set; }
        public ICollection<NonConformiteReponseFournisseurType> NonConformiteReponseFournisseurTypeCreations { get; set; }
        public ICollection<NonConformiteReponseFournisseurType> NonConformiteReponseFournisseurTypeModifs { get; set; }
        public ICollection<NonConformiteEtape> NonConformiteEtapeCreations { get; set; }
        public ICollection<NonConformiteEtape> NonConformiteEtapeModifs { get; set; }
        public ICollection<NonConformiteEtape> NonConformiteEtapeControles { get; set; }
        public ICollection<NonConformiteEtape> NonConformiteEtapeValides { get; set; }
        public ICollection<ParamEmail> ParamEmailCreations { get; set; }
        public ICollection<ParamEmail> ParamEmailModifs { get; set; }
        public ICollection<Parametre> ParametreCreations { get; set; }
        public ICollection<Parametre> ParametreModifs { get; set; }
        public ICollection<Pays> PaysCreations { get; set; }
        public ICollection<Pays> PaysModifs { get; set; }
        public ICollection<PrixReprise> PrixRepriseCreations { get; set; }
        public ICollection<PrixReprise> PrixRepriseModifs { get; set; }
        public ICollection<Produit> ProduitCreations { get; set; }
        public ICollection<Produit> ProduitModifs { get; set; }
        public ICollection<Process> ProcessCreations { get; set; }
        public ICollection<Process> ProcessModifs { get; set; }
        public ICollection<ProduitGroupeReporting> ProduitGroupeReportingCreations { get; set; }
        public ICollection<ProduitGroupeReporting> ProduitGroupeReportingModifs { get; set; }
        public ICollection<ProduitGroupeReportingType> ProduitGroupeReportingTypeCreations { get; set; }
        public ICollection<ProduitGroupeReportingType> ProduitGroupeReportingTypeModifs { get; set; }
        public ICollection<RegionEE> RegionEECreations { get; set; }
        public ICollection<RegionEE> RegionEEModifs { get; set; }
        public ICollection<RegionReporting> RegionReportingCreations { get; set; }
        public ICollection<RegionReporting> RegionReportingModifs { get; set; }
        public ICollection<Repartition> RepartitionCreations { get; set; }
        public ICollection<Repartition> RepartitionModifs { get; set; }
        public ICollection<Repreneur> RepreneurCreations { get; set; }
        public ICollection<Repreneur> RepreneurModifs { get; set; }
        public ICollection<RepriseType> RepriseTypeCreations { get; set; }
        public ICollection<RepriseType> RepriseTypeModifs { get; set; }
        public ICollection<SAGECodeTransport> SAGECodeTransportCreations { get; set; }
        public ICollection<SAGECodeTransport> SAGECodeTransportModifs { get; set; }
        public ICollection<Securite> SecuriteCreations { get; set; }
        public ICollection<Securite> SecuriteModifs { get; set; }
        public ICollection<Service> ServiceCreations { get; set; }
        public ICollection<Service> ServiceModifs { get; set; }
        public ICollection<SurcoutCarburant> SurcoutCarburantCreations { get; set; }
        public ICollection<SurcoutCarburant> SurcoutCarburantModifs { get; set; }
        public ICollection<Standard> StandardCreations { get; set; }
        public ICollection<Standard> StandardModifs { get; set; }
        public ICollection<Titre> TitreCreations { get; set; }
        public ICollection<Titre> TitreModifs { get; set; }
        public ICollection<Ticket> TicketCreations { get; set; }
        public ICollection<Ticket> TicketModifs { get; set; }
        public ICollection<Transport> TransportCreations { get; set; }
        public ICollection<Transport> TransportModifs { get; set; }
        public ICollection<Verrouillage> Verrouillages { get; set; }
        public ICollection<Utilisateur> UtilisateurMaitres { get; set; }
        public ICollection<Utilisateur> UtilisateurAffectationMaitres { get; set; }
        public ICollection<Utilisateur> UtilisateurCreations { get; set; }
        public ICollection<Utilisateur> UtilisateurModifs { get; set; }
        [NotMapped]
        public int RefUtilisateurCourant { get; set; }
        public int RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        public int? RefUtilisateurModif { get; set; }
        public Utilisateur UtilisateurModif { get; set; }
        public string CreationText
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (RefUtilisateurCreation <= 0) { s = cR.GetTextRessource(9); }
                if (UtilisateurCreation != null)
                {
                    s = cR.GetTextRessource(388) + " " + DCreation.ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurCreation.Nom;
                }
                return s;
            }
        }
        public string ModificationText
        {
            get
            {
                string s = "";
                if (UtilisateurModif != null && DModif != null)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    s = cR.GetTextRessource(389) + " " + ((DateTime)DModif).ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurModif.Nom;
                }
                return s;
            }
        }
        public string PwdModificationText
        {
            get
            {
                string s = "";
                if (PwdDModif != null)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    s = cR.GetTextRessource(1111) + " " + ((DateTime)PwdDModif).ToString("G", currentCulture);
                }
                return s;
            }
        }
        public string AffectationUtilisateurMaitreText
        {
            get
            {
                string s = "";
                if (UtilisateurAffectationMaitre != null && RefUtilisateurAffectationMaitre > 0)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    s = cR.GetTextRessource(1509) + " " + ((DateTime)DAffectationMaitre).ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurAffectationMaitre.Nom;
                }
                return s;
            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Mark modifications
        /// </summary>
        public void MarkModification(bool add)
        {
            if (add)
            {
                RefUtilisateurCreation = RefUtilisateurCourant;
                DCreation = DateTime.Now;
            }
            else
            {
                RefUtilisateurModif = RefUtilisateurCourant;
                DModif = DateTime.Now;
            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.Utilisateurs.Where(q =>
            ((!string.IsNullOrWhiteSpace(q.EMail) && q.PwdHash != null && (q.EMail == EMail && q.PwdHash == PwdHash))
            || (!string.IsNullOrWhiteSpace(q.Login) && q.PwdHash != null && (q.Login == Login && q.PwdHash == PwdHash))
            ) && q.RefUtilisateur != RefUtilisateur)
            .Count();
            if (c > 0 || Nom?.Length > 100 || Login?.Length > 50 || EMail?.Length > 200 || Tel?.Length > 20
                 || TelMobile?.Length > 20 || Fax?.Length > 20)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Nom?.Length > 100) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(396); }
                if (Login?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(397); }
                if (EMail?.Length > 200) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(413); }
                if (Tel?.Length > 20) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(414); }
                if (TelMobile?.Length > 20) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(415); }
                if (Fax?.Length > 20) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(416); }
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(1112); }
            }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if linked data exist.
        /// </summary>
        public string IsDeletable()
        {
            string r = "";
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.ActionCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ActionModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ActionTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ActionTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.AdresseCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.AdresseModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.AdresseTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.AdresseTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ApplicationCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ApplicationModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ApplicationProduitOrigineCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ApplicationProduitOrigineModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.AideCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.AideModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CamionTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CamionTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ClientApplicationCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ClientApplicationModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeClientCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeClientModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurUtilisateurChargementAnnules).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurUtilisateurBlocages).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurUtilisateurAnomalieOks).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ContactCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ContactModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ContactAdresseCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ContactAdresseModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ControleCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ControleModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CiviliteCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CiviliteModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CVQCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CVQModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DescriptionControleCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DescriptionControleModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DescriptionCVQCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DescriptionCVQModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DescriptionReceptionCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DescriptionReceptionModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DocumentCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DocumentModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DocumentTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DocumentTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EcoOrganismeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EcoOrganismeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EquivalentCO2Creations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EquivalentCO2Modifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EmailCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EmailModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EntiteCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EntiteModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EntiteCamionTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EntiteEntiteCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EntiteProduitCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EntiteTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EntiteTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EquipementierCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EquipementierModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FicheControleCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FicheControleModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FonctionCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FonctionCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FournisseurTOCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FournisseurTOModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MessageCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MessageModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MessageTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MessageTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ModeTransportEECreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ModeTransportEEModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteAccordFournisseurTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteAccordFournisseurTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteDemandeClientTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteDemandeClientTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteFamilleCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteFamilleModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteNatureCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteNatureModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteReponseFournisseurTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteReponseFournisseurTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteReponseClientTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteReponseClientTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ParamEmailCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ParamEmailModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ParametreCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ParametreModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.PaysCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.PaysModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.PrixRepriseCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.PrixRepriseModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProduitCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProduitModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProcessCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProcessModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProduitGroupeReportingTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProduitGroupeReportingModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProduitGroupeReportingTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProduitGroupeReportingTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RegionEECreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RegionEEModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepartitionCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepartitionModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepreneurCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepreneurModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepriseTypeCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepriseTypeModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.SAGECodeTransportCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.SAGECodeTransportModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.SecuriteCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.SecuriteModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ServiceCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ServiceModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.SurcoutCarburantCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.SurcoutCarburantModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.StandardCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.StandardModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.TicketCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.TicketModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.TitreCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.TitreModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.TransportCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.TransportModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.UtilisateurMaitres).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.UtilisateurAffectationMaitres).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MotifAnomalieChargementCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MotifAnomalieChargementModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MotifAnomalieClientCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MotifAnomalieClientModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MotifAnomalieTransporteurCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MotifAnomalieTransporteurModifs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MotifCamionIncompletCreations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.MotifCamionIncompletModifs).Query().Count();
            if (nbLinkedData != 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "") { r += Environment.NewLine; }
                r += cR.GetTextRessource(393);
            }
            return r;
        }
    }
}