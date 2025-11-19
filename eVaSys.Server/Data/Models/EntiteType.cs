/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 02/10/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for types d'Entite
    /// </summary>
    public class EntiteType : IMarkModification
    {
        #region Constructor
        public EntiteType()
        {
        }
        private EntiteType(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }

        public CultureInfo currentCulture = new("fr-FR");
        public int RefEntiteType { get; set; }
        public string Libelle { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public bool? CodeEE { get; set; }
        public bool Adelphe { get; set; }
        public bool? RefEcoOrganisme { get; set; }
        public bool? AncienCodeAdelphe { get; set; }
        public bool? CodeValorisation { get; set; }
        public bool? AssujettiTVA { get; set; }
        public bool? CodeTVA { get; set; }
        public bool? SAGECodeComptable { get; set; }
        public bool? SAGECompteTiers { get; set; }
        public bool? RefSAGEConditionLivraison { get; set; }
        public bool? RefSAGEPeriodicite { get; set; }
        public bool? RefSAGEModeReglement { get; set; }
        public bool? RefSAGECategorieAchat { get; set; }
        public bool? RefSAGECategorieVente { get; set; }
        public bool ActionnaireProprietaire { get; set; }
        public bool Exploitant { get; set; }
        public bool RefGroupeIndustriel { get; set; }
        public bool RefParticularite { get; set; }
        public bool Demarrage { get; set; }
        public bool Capacite { get; set; }
        public bool TonnageGlobal { get; set; }
        public bool RefEquipementier1 { get; set; }
        public bool RefEquipementier2 { get; set; }
        public bool RefEquipementier3 { get; set; }
        public bool EquipementierCmt { get; set; }
        public bool RefPresse { get; set; }
        public bool PressePuissance { get; set; }
        public bool PresseSection { get; set; }
        public bool PresseCmt { get; set; }
        public bool TriCouleurPellenc { get; set; }
        public bool TriCouleurTitech { get; set; }
        public bool TriMatierePellenc { get; set; }
        public bool TriMatiereTitech { get; set; }
        public bool TriBiTechnologiePellenc { get; set; }
        public bool TriBiTechnologieTitech { get; set; }
        public bool TriCmt { get; set; }
        public bool RefEtoileDisque { get; set; }
        public bool EtoileDisqueNbMachine { get; set; }
        public bool RefBalistique { get; set; }
        public bool BalistiqueNbMachine { get; set; }
        public bool RefTrommel { get; set; }
        public bool TrommelNbMachine { get; set; }
        public bool RefAutre { get; set; }
        public bool CriblageNbMachine { get; set; }
        public bool CriblageCmt { get; set; }
        public bool PopulationContratN { get; set; }
        public bool PopulationContratN1 { get; set; }
        public bool PopulationContratN2 { get; set; }
        public bool PopulationContratN3 { get; set; }
        public bool LiaisonEntiteProduit { get; set; }
        public bool LiaisonEntiteProduitInterdit { get; set; }
        public bool Horaires { get; set; }
        public bool ContratOptimisationTransport { get; set; }
        public bool ContratCollectivite { get; set; }
        public bool RefRepreneur { get; set; }
        public bool RefRepriseType { get; set; }
        public bool UtilisateurCollectivite { get; set; }
        public bool ExportSAGE { get; set; }
        public bool Process { get; set; }
        public bool VisibiliteAffretementCommun { get; set; }
        public bool? BLUnique { get; set; }
        public bool AutoControle { get; set; }
        public bool LiaisonEntiteStandard { get; set; }
        public bool SousContrat { get; set; }
        public bool LiaisonEntiteCamionType { get; set; }
        public bool IncitationQualite { get; set; }
        public bool SurcoutCarburant { get; set; }
        public bool RefEquipementier { get; set; }
        public bool RefFournisseurTO { get; set; }
        public bool DimensionBalle { get; set; }
        public bool IdNational { get; set; }
        public bool LiaisonEntiteDR { get; set; }
        public ICollection<Entite> Entites { get; set; }
        public ICollection<ActionTypeEntiteType> ActionTypeEntiteTypes { get; set; }
        public ICollection<DocumentEntiteType> DocumentEntiteTypes { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
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
            bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tbrEntiteType where RefEntiteType=" + RefEntiteType, DbContext.Database.GetDbConnection()) != "0");
            int c = DbContext.EntiteTypes.Where(q => (q.LibelleFRFR == LibelleFRFR || q.LibelleENGB == LibelleENGB)).Count();
            if (!existsInDB && c > 0 || existsInDB && c > 1 || LibelleFRFR?.Length > 50 || LibelleENGB?.Length > 50)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if ((!existsInDB && c > 0) || (existsInDB && c > 1)) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
                if (LibelleFRFR?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(398); }
                if (LibelleENGB?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(399); }
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.Entites).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ActionTypeEntiteTypes).Query().Count();
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