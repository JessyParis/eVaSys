/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 01/10/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for entités
    /// </summary>
    public class Entite : IMarkModification
    {
        #region Constructor
        public Entite()
        {
        }
        private Entite(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        #endregion
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }

        public CultureInfo currentCulture = new("fr-FR");
        public int RefEntite { get; set; }
        public int RefEntiteType { get; set; }
        private EntiteType _entiteType;
        public EntiteType EntiteType
        {
            get => LazyLoader.Load(this, ref _entiteType);
            set => _entiteType = value;
        }
        public string Libelle { get; set; }
        public string CodeEE { get; set; }
        public bool Adelphe { get; set; }
        public string AncienCodeAdelphe { get; set; }
        public int? RefEcoOrganisme { get; set; }
        private EcoOrganisme _ecoOrganisme;
        public EcoOrganisme EcoOrganisme
        {
            get => LazyLoader.Load(this, ref _ecoOrganisme);
            set => _ecoOrganisme = value;
        }
        public string CodeValorisation { get; set; }
        public string Cmt { get; set; }
        public bool? AssujettiTVA { get; set; }
        public string CodeTVA { get; set; }
        public string? SAGECodeComptable { get; set; }
        public string SAGECompteTiers { get; set; }
        public int? RefSAGEConditionLivraison { get; set; }
        private SAGEConditionLivraison _sAGEConditionLivraison;
        public SAGEConditionLivraison SAGEConditionLivraison
        {
            get => LazyLoader.Load(this, ref _sAGEConditionLivraison);
            set => _sAGEConditionLivraison = value;
        }
        public int? RefSAGEPeriodicite { get; set; }
        private SAGEPeriodicite _sAGEPeriodicite;
        public SAGEPeriodicite SAGEPeriodicite
        {
            get => LazyLoader.Load(this, ref _sAGEPeriodicite);
            set => _sAGEPeriodicite = value;
        }
        public int? RefSAGEModeReglement { get; set; }
        private SAGEModeReglement _sAGEModeReglement;
        public SAGEModeReglement SAGEModeReglement
        {
            get => LazyLoader.Load(this, ref _sAGEModeReglement);
            set => _sAGEModeReglement = value;
        }
        public int? RefSAGECategorieAchat { get; set; }
        private SAGECategorieAchat _sAGECategorieAchat;
        public SAGECategorieAchat SAGECategorieAchat
        {
            get => LazyLoader.Load(this, ref _sAGECategorieAchat);
            set => _sAGECategorieAchat = value;
        }
        public int? RefSAGECategorieVente { get; set; }
        private SAGECategorieVente _sAGECategorieVente;
        public SAGECategorieVente SAGECategorieVente
        {
            get => LazyLoader.Load(this, ref _sAGECategorieVente);
            set => _sAGECategorieVente = value;
        }
        public string ActionnaireProprietaire { get; set; }
        public string Exploitant { get; set; }
        public int? RefGroupeIndustriel { get; set; }
        public int? RefParticularite { get; set; }
        public string Demarrage { get; set; }
        public int? Capacite { get; set; }
        public int? TonnageGlobal { get; set; }
        public int? RefEquipementier1 { get; set; }
        public int? RefEquipementier2 { get; set; }
        public int? RefEquipementier3 { get; set; }
        public string EquipementierCmt { get; set; }
        public int? RefPresse { get; set; }
        public int? PressePuissance { get; set; }
        public string PresseSection { get; set; }
        public string PresseCmt { get; set; }
        public int? TriCouleurPellenc { get; set; }
        public int? TriCouleurTitech { get; set; }
        public int? TriMatierePellenc { get; set; }
        public int? TriMatiereTitech { get; set; }
        public int? TriBiTechnologiePellenc { get; set; }
        public int? TriBiTechnologieTitech { get; set; }
        public string TriCmt { get; set; }
        public int? RefEtoileDisque { get; set; }
        public int? EtoileDisqueNbMachine { get; set; }
        public int? RefBalistique { get; set; }
        public int? BalistiqueNbMachine { get; set; }
        public int? RefTrommel { get; set; }
        public int? TrommelNbMachine { get; set; }
        public int? RefAutre { get; set; }
        public int? CriblageNbMachine { get; set; }
        public string CriblageCmt { get; set; }
        public int? PopulationContratN { get; set; }
        public int? PopulationContratN1 { get; set; }
        public int? PopulationContratN2 { get; set; }
        public int? PopulationContratN3 { get; set; }
        public string Horaires { get; set; }
        public int? RefRepreneur { get; set; }
        private Repreneur _repreneur;
        public Repreneur Repreneur
        {
            get => LazyLoader.Load(this, ref _repreneur);
            set => _repreneur = value;
        }
        public int? RefRepriseType { get; set; }
        private RepriseType _repriseType;
        public RepriseType RepriseType
        {
            get => LazyLoader.Load(this, ref _repriseType);
            set => _repriseType = value;
        }
        public bool RepartitionMensuelle { get; set; }
        public bool ExportSAGE { get; set; }
        public bool VisibiliteAffretementCommun { get; set; }
        public bool AutoControle { get; set; }
        public bool? Actif { get; set; }
        public bool SousContrat { get; set; }
        public bool ReconductionIncitationQualite { get; set; }
        public bool SurcoutCarburant { get; set; }
        public int? RefEquipementier { get; set; }
        private Equipementier _equipementier;
        public Equipementier Equipementier
        {
            get => LazyLoader.Load(this, ref _equipementier);
            set => _equipementier = value;
        }
        public int? RefFournisseurTO { get; set; }
        private FournisseurTO _fournisseurTO;
        public FournisseurTO FournisseurTO
        {
            get => LazyLoader.Load(this, ref _fournisseurTO);
            set => _fournisseurTO = value;
        }
        public string DimensionBalle { get; set; }
        public string IdNational { get; set; }
        public string RefExt { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public ICollection<Utilisateur> UtilisateurClients { get; set; }
        public ICollection<Utilisateur> UtilisateurTransporteurs { get; set; }
        public ICollection<Utilisateur> UtilisateurCentreDeTris { get; set; }
        public ICollection<Utilisateur> UtilisateurCollectivites { get; set; }
        public ICollection<Utilisateur> UtilisateuPrestataires { get; set; }
        public ICollection<Transport> Transports { get; set; }
        public ICollection<Action> Actions { get; set; }
        public ICollection<Adresse> Adresses { get; set; }
        public ICollection<ContactAdresse> ContactAdresses { get; set; }
        public ICollection<ClientApplication> ClientApplications { get; set; }
        public ICollection<CommandeClient> CommandeClients { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurs { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurTransporteurs { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurPrestataires { get; set; }
        public ICollection<ContratIncitationQualite> ContratIncitationQualites { get; set; }
        public ICollection<ContratCollectivite> ContratCollectivites { get; set; }
        public ICollection<DocumentEntite> DocumentEntites { get; set; }
        public ICollection<EntiteCamionType> EntiteCamionTypes { get; set; }
        public ICollection<EntiteDR> EntiteDRs { get; set; }
        public ICollection<EntiteProcess> EntiteProcesss { get; set; }
        public ICollection<EntiteProduit> EntiteProduits { get; set; }
        public ICollection<EntiteStandard> EntiteStandards { get; set; }
        public ICollection<FicheControle> FicheControles { get; set; }
        public ICollection<Repartition> Repartitions { get; set; }
        public ICollection<RepartitionCollectivite> RepartitionCollectivites { get; set; }
        public ICollection<RepartitionProduit> RepartitionProduits { get; set; }
        public ICollection<SurcoutCarburant> SurcoutCarburantTransporteurs { get; set; }
        public ICollection<EntiteEntite> EntiteEntiteInits { get; set; }
        public ICollection<EntiteEntite> EntiteEntiteRttInits { get; set; }
        private ICollection<EntiteEntite> _entiteEntites { get; set; }
        [NotMapped]
        public ICollection<SAGEDocument> SAGEDocuments { get; set; }
        [NotMapped]
        public ICollection<EntiteEntite> EntiteEntites
        {
            get
            {
                //Load if applicable
                if (_entiteEntites == null && RefEntite > 0)
                {
                    _entiteEntites = DbContext.EntiteEntites
                        .Include(i => i.EntiteRtt)
                        .Include(i => i.Entite)
                        .Include(i => i.UtilisateurCreation)
                        .Where(e => e.RefEntite == RefEntite)
                        .Select(i => new EntiteEntite
                        {
                            RefEntiteEntite = i.RefEntiteEntite,
                            RefEntite = i.RefEntite,
                            RefEntiteRtt = i.RefEntiteRtt,
                            Actif = i.Actif,
                            Cmt = i.Cmt,
                            LibelleUtilisateurCreation = i.UtilisateurCreation.Nom,
                            RefEntiteRttType = i.EntiteRtt.RefEntiteType,
                            LibelleEntiteRtt = (string.IsNullOrWhiteSpace(i.EntiteRtt.CodeEE) ? i.EntiteRtt.Libelle : i.EntiteRtt.CodeEE + " - " + i.EntiteRtt.Libelle),
                            ActifEntiteRtt = i.EntiteRtt.Actif,
                            DCreation = i.DCreation,
                        })
                        .ToHashSet();
                    var e = DbContext.EntiteEntites
                        .Include(i => i.EntiteRtt)
                        .Include(i => i.Entite)
                        .Include(i => i.UtilisateurCreation)
                        .Where(e => e.RefEntiteRtt == RefEntite)
                        .Select(i => new EntiteEntite
                        {
                            RefEntiteEntite = i.RefEntiteEntite,
                            RefEntite = i.RefEntiteRtt,
                            RefEntiteRtt = i.RefEntite,
                            Actif = i.Actif,
                            Cmt = i.Cmt,
                            LibelleUtilisateurCreation = i.UtilisateurCreation.Nom,
                            RefEntiteRttType = i.Entite.RefEntiteType,
                            LibelleEntiteRtt = (string.IsNullOrWhiteSpace(i.Entite.CodeEE) ? i.Entite.Libelle : i.Entite.CodeEE + " - " + i.Entite.Libelle),
                            ActifEntiteRtt = i.Entite.Actif,
                            DCreation = i.DCreation,
                        })
                        .ToHashSet();
                    _entiteEntites = _entiteEntites.Concat(e).ToHashSet();
                }
                return _entiteEntites;
            }
            set { _entiteEntites = value; }
        }
        //Texte de synthèse
        private string _texteEntite = "";
        public string TexteEntite
        {
            get
            {
                //Texte de synthèse des donnée sprincipales de l'adresse
                string s = Libelle;
                if (EntiteType.CodeEE == true)
                {
                    _texteEntite += " - Code CITEO : " + (string.IsNullOrEmpty(CodeEE) ? "(NR)" : CodeEE);
                }
                if (EntiteType.Adelphe)
                {
                    _texteEntite += " - Région Adelphe : " + (Adelphe ? "oui" : "non");
                }
                return s;
            }
        }
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
            if (Libelle?.Length > 50)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Libelle?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
                if (CodeEE?.Length > 20) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(400); }
                if (AncienCodeAdelphe?.Length > 15) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(401); }
                if (CodeValorisation?.Length > 3) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(402); }
                if (CodeTVA?.Length > 20) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(403); }
                if (SAGECodeComptable?.Length > 17) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(404); }
                if (SAGECompteTiers?.Length > 13) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(405); }
                if (ActionnaireProprietaire?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(406); }
                if (Exploitant?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(407); }
                if (Demarrage?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(408); }
                if (PresseSection?.Length > 20) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(409); }
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.UtilisateurCentreDeTris).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.UtilisateurClients).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.UtilisateurCollectivites).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.UtilisateurTransporteurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.Transports).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ClientApplications).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeClients).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurTransporteurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurPrestataires).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FicheControles).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.Repartitions).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepartitionCollectivites).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepartitionProduits).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.SurcoutCarburantTransporteurs).Query().Count();
            //Check if attached entities can be deleted
            foreach (var adr in Adresses)
            {
                if (adr.IsDeletable(false) != "") { nbLinkedData++; }
            }
            foreach (var cA in ContactAdresses)
            {
                if (cA.IsDeletable(false) != "") { nbLinkedData++; }
            }
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