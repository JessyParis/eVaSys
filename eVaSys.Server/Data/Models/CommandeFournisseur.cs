/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/05/2019
/// -----------------------------------------------------------------------------------------------------
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using static eVaSys.Utils.Enumerations;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for CommandeFournisseur
    /// </summary>
    public class CommandeFournisseur : IMarkModification
    {
        #region Constructor

        public CommandeFournisseur()
        {
        }

        private CommandeFournisseur(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        private ILazyLoader LazyLoader { get; set; }

        #endregion Constructor

        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefCommandeFournisseur { get; set; }
        public int NumeroCommande { get; set; }
        public int NumeroAffretement { get; set; }
        public int? OrdreAffretement { get; set; } = 1;
        public int RefEntite { get; set; }
        private Entite _entite;

        public Entite Entite
        {
            get => LazyLoader.Load(this, ref _entite);
            set => _entite = value;
        }

        public string Horaires { get; set; }
        public int? RefAdresse { get; set; }
        private Adresse _adresse;

        public Adresse Adresse
        {
            get => LazyLoader.Load(this, ref _adresse);
            set => _adresse = value;
        }

        public string Libelle { get; set; }
        public string Adr1 { get; set; }
        public string Adr2 { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public int? RefPays { get; set; }
        private Pays _pays;
        public Pays Pays
        {
            get => LazyLoader.Load(this, ref _pays);
            set => _pays = value;
        }
        public int? RefContactAdresse { get; set; }
        private ContactAdresse _contactAdresse;

        public ContactAdresse ContactAdresse
        {
            get => LazyLoader.Load(this, ref _contactAdresse);
            set => _contactAdresse = value;
        }

        public int? RefCivilite { get; set; }
        private Civilite _civilite;

        public Civilite Civilite
        {
            get => LazyLoader.Load(this, ref _civilite);
            set => _civilite = value;
        }

        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Tel { get; set; }
        public string TelMobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public int RefProduit { get; set; }
        private Produit _produit;

        public Produit Produit
        {
            get => LazyLoader.Load(this, ref _produit);
            set => _produit = value;
        }

        public DateTime? D { get; set; }
        public DateTime? DChargementPrevue { get; set; }
        public string HoraireChargementPrevu { get; set; }
        public bool ChargementAnnule { get; set; }
        public DateTime? DChargementAnnule { get; set; }
        public int? RefUtilisateurChargementAnnule { get; set; }
        private Utilisateur _utilisateurChargementAnnule;

        public Utilisateur UtilisateurChargementAnnule
        {
            get => LazyLoader.Load(this, ref _utilisateurChargementAnnule);
            set => _utilisateurChargementAnnule = value;
        }

        public string CmtChargementAnnule { get; set; }
        public DateTime? DChargement { get; set; }
        public DateTime? DDechargementPrevue { get; set; }
        public string HoraireDechargementPrevu { get; set; }
        public DateTime? DDechargement { get; set; }
        public int PoidsChargement { get; set; }
        public int NbBalleChargement { get; set; }
        public bool TicketPeseeChargement { get; set; }
        public int PoidsDechargement { get; set; }
        public int NbBalleDechargement { get; set; }
        public bool TicketPeseeDechargement { get; set; }
        public int? RefCamionType { get; set; } = 1;
        private CamionType _camionType;

        public CamionType CamionType
        {
            get => LazyLoader.Load(this, ref _camionType);
            set => _camionType = value;
        }

        public int? RefTransporteur { get; set; }
        private Entite _transporteur;

        public Entite Transporteur
        {
            get => LazyLoader.Load(this, ref _transporteur);
            set => _transporteur = value;
        }

        public int? RefTransporteurContactAdresse { get; set; }
        private ContactAdresse _transporteurContactAdresse;

        public ContactAdresse TransporteurContactAdresse
        {
            get => LazyLoader.Load(this, ref _transporteurContactAdresse);
            set => _transporteurContactAdresse = value;
        }

        public decimal PrixTransportHT { get; set; }
        public decimal SurcoutCarburantHT { get; set; }
        public decimal PrixTransportSupplementHT { get; set; }
        public int Km { get; set; }
        public int? RefAdresseClient { get; set; }
        private Adresse _adresseClient;

        public Adresse AdresseClient
        {
            get => LazyLoader.Load(this, ref _adresseClient);
            set => _adresseClient = value;
        }

        public string CmtFournisseur { get; set; }
        public string CmtClient { get; set; }
        public string CmtTransporteur { get; set; }
        public decimal PrixTonneHT { get; set; }
        public DateTime? DMoisDechargementPrevu { get; set; }
        public bool Imprime { get; set; }
        public bool ExportSAGE { get; set; }
        private bool _valideDPrevues;

        public bool ValideDPrevues
        {
            get { return _valideDPrevues; }
            set
            {
                if (value == true) { DChargementModif = null; }
                else if (value == false && _valideDPrevues == true) { DChargementModif = DateTime.Now; }
                _valideDPrevues = value;
            }
        }

        public bool RefusCamion { get; set; }
        public int PoidsReparti { get; set; }
        public DateTime? DBlocage { get; set; }
        public int? RefUtilisateurBlocage { get; set; }
        private Utilisateur _utilisateurBlocage;

        public Utilisateur UtilisateurBlocage
        {
            get => LazyLoader.Load(this, ref _utilisateurBlocage);
            set => _utilisateurBlocage = value;
        }

        public string CmtBlocage { get; set; }
        public DateTime? DAffretement { get; set; }
        public int? RefCommandeFournisseurStatut { get; set; }
        private CommandeFournisseurStatut _commandeFournisseurStatut;

        public CommandeFournisseurStatut CommandeFournisseurStatut
        {
            get => LazyLoader.Load(this, ref _commandeFournisseurStatut);
            set => _commandeFournisseurStatut = value;
        }

        public bool? CamionComplet { get; set; }
        public int? RefMotifCamionIncomplet { get; set; }
        private MotifCamionIncomplet _motifCamionIncomplet;

        public MotifCamionIncomplet MotifCamionIncomplet
        {
            get => LazyLoader.Load(this, ref _motifCamionIncomplet);
            set => _motifCamionIncomplet = value;
        }

        public DateTime? DChargementModif { get; set; }
        public bool ChargementEffectue { get; set; }
        public DateTime? DAnomalieChargement { get; set; }
        public int? RefMotifAnomalieChargement { get; set; }
        private MotifAnomalieChargement _motifAnomalieChargement;

        public MotifAnomalieChargement MotifAnomalieChargement
        {
            get => LazyLoader.Load(this, ref _motifAnomalieChargement);
            set => _motifAnomalieChargement = value;
        }

        public string CmtAnomalieChargement { get; set; }
        public DateTime? DTraitementAnomalieChargement { get; set; }
        public DateTime? DAnomalieClient { get; set; }
        public int? RefMotifAnomalieClient { get; set; }
        private MotifAnomalieClient _motifAnomalieClient;

        public MotifAnomalieClient MotifAnomalieClient
        {
            get => LazyLoader.Load(this, ref _motifAnomalieClient);
            set => _motifAnomalieClient = value;
        }

        public string CmtAnomalieClient { get; set; }
        public DateTime? DTraitementAnomalieClient { get; set; }
        public DateTime? DAnomalieTransporteur { get; set; }
        public int? RefMotifAnomalieTransporteur { get; set; }
        private MotifAnomalieTransporteur _motifAnomalieTransporteur;

        public MotifAnomalieTransporteur MotifAnomalieTransporteur
        {
            get => LazyLoader.Load(this, ref _motifAnomalieTransporteur);
            set => _motifAnomalieTransporteur = value;
        }

        public string CmtAnomalieTransporteur { get; set; }
        public bool LotControle { get; set; }
        public DateTime? DTraitementAnomalieTransporteur { get; set; }
        public DateTime? DAnomalieOk { get; set; }
        public int? RefUtilisateurAnomalieOk { get; set; }
        private Utilisateur _utilisateurAnomalieOk;
        public Utilisateur UtilisateurAnomalieOk
        {
            get => LazyLoader.Load(this, ref _utilisateurAnomalieOk);
            set => _utilisateurAnomalieOk = value;
        }
        public bool NonRepartissable { get; set; } = false;
        public int? RefPrestataire { get; set; }
        public string RefExt { get; set; }
        private Entite _prestataire;
        public Entite Prestataire
        {
            get => LazyLoader.Load(this, ref _prestataire);
            set => _prestataire = value;
        }
        public CommandeFournisseurContrat CommandeFournisseurContrat { get; set; }
        [NotMapped]
        public bool Mixte
        {
            get
            {
                return DbContext.CommandeFournisseurs
                    .Where(i => (i.NumeroAffretement == NumeroAffretement))
                    .Count() > 1;
            }
        }
        [NotMapped]
        public bool MixteAndPrintable
        {
            get
            {
                return DbContext.CommandeFournisseurs
                    .Where(i => (i.NumeroAffretement == NumeroAffretement) && i.NumeroCommande!= NumeroCommande && (i.DChargementPrevue == null || i.DDechargementPrevue == null))
                    .Count() == 0;
            }
        }

        public ICollection<Repartition> Repartitions { get; set; }
        public ICollection<CommandeFournisseurFichier> CommandeFournisseurFichiers { get; set; }
        public ICollection<FicheControle> FicheControles { get; set; }
        public ICollection<NonConformite> NonConformites { get; set; }

        [NotMapped]
        public Utilisateur LockedBy
        {
            get
            {
                return Utils.Utils.IsLockedData(LockableData.RefCommandeFournisseur.ToString(), RefCommandeFournisseur, DbContext);
            }
        }

        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }

        [NotMapped]
        public int RefUtilisateurCourant { get; set; }

        public int RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        public int? RefUtilisateurModif { get; set; }
        public Utilisateur UtilisateurModif { get; set; }

        public string GetTelsOrEmail
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (!string.IsNullOrWhiteSpace(Tel)) { s = cR.GetTextRessource(417) + " : " + Tel; }
                if (!string.IsNullOrWhiteSpace(TelMobile))
                {
                    if (!string.IsNullOrWhiteSpace(Tel)) { s += " / "; }
                    s += cR.GetTextRessource(437) + " : " + TelMobile;
                }
                if ((string.IsNullOrWhiteSpace(Tel) || string.IsNullOrWhiteSpace(TelMobile)) && !string.IsNullOrWhiteSpace(Email))
                {
                    {
                        if (!string.IsNullOrWhiteSpace(Tel) || !string.IsNullOrWhiteSpace(TelMobile)) { s += " / "; }
                        s += cR.GetTextRessource(419) + " : " + Email;
                    }
                }
                return s;
            }
        }
        public string GetFirstTelOrEmail
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (!string.IsNullOrWhiteSpace(Tel)) { s = cR.GetTextRessource(417) + " : " + Tel; }
                else if (!string.IsNullOrWhiteSpace(TelMobile)) { s += cR.GetTextRessource(437) + " : " + TelMobile; }
                else if (!string.IsNullOrWhiteSpace(Email)) { s += cR.GetTextRessource(419) + " : " + Email; }
                return s;
            }
        }
        public string GetSecondTelOrEmail
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (!string.IsNullOrWhiteSpace(Tel))
                {
                    if (!string.IsNullOrWhiteSpace(TelMobile))
                    {
                        s = cR.GetTextRessource(437) + " : " + TelMobile;
                    }
                    else if (!string.IsNullOrWhiteSpace(Email)) { s += cR.GetTextRessource(419) + " : " + Email; }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(TelMobile))
                    {
                        if (!string.IsNullOrWhiteSpace(Email)) { s += cR.GetTextRessource(419) + " : " + Email; }
                    }
                }
                return s;
            }
        }
        public string CreationText
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (UtilisateurCreation == null || DCreation == DateTime.MinValue) { s = cR.GetTextRessource(9); }
                else
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

        public string AnomalieOkText
        {
            get
            {
                string s = "";
                if (UtilisateurAnomalieOk != null && DAnomalieOk != null)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    s = cR.GetTextRessource(577) + " " + ((DateTime)DAnomalieOk).ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurAnomalieOk.Nom;
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

        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tblCommandeFournisseur where RefCommandeFournisseur=" + RefCommandeFournisseur, DbContext.Database.GetDbConnection()) != "0");
            int c = DbContext.CommandeFournisseurs.Where(q => q.NumeroCommande == NumeroCommande).Count();
            if ((!existsInDB && c > 0) || (existsInDB && c > 1) || Libelle?.Length > 50)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if ((!existsInDB && c > 0) || (existsInDB && c > 1)) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
                if (Libelle?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.Repartitions).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurFichiers).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FicheControles).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformites).Query().Count();
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