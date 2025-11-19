/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/07/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for NonConformiteEtape
    /// </summary>
    public class NonConformiteEtape
    {
        #region Constructor
        public NonConformiteEtape()
        {
        }
        private NonConformiteEtape(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefNonConformiteEtape { get; set; }
        public int RefNonConformite { get; set; }
        public int? RefNonConformiteEtapeType { get; set; }
        private NonConformiteEtapeType _nonConformiteEtapeType;
        public NonConformiteEtapeType NonConformiteEtapeType
        {
            get => LazyLoader.Load(this, ref _nonConformiteEtapeType);
            set => _nonConformiteEtapeType = value;
        }
        public int Ordre { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        [NotMapped]
        public string Libelle
        {
            get { return (currentCulture.Name == "fr-FR" ? LibelleFRFR : LibelleENGB); }
            set { }
        }
        public string Cmt { get; set; }
        public DateTime? DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public DateTime? DControle { get; set; }
        public DateTime? DValide { get; set; }
        [NotMapped]
        public int RefUtilisateurCourant { get; set; }
        public int? RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        public int? RefUtilisateurModif { get; set; }
        public Utilisateur UtilisateurModif { get; set; }
        public int? RefUtilisateurControle { get; set; }
        private Utilisateur _utilisateurControle;
        public Utilisateur UtilisateurControle
        {
            get => LazyLoader.Load(this, ref _utilisateurControle);
            set => _utilisateurControle = value;
        }
        public int? RefUtilisateurValide { get; set; }
        private Utilisateur _utilisateurValide;
        public Utilisateur UtilisateurValide
        {
            get => LazyLoader.Load(this, ref _utilisateurValide);
            set => _utilisateurValide = value;
        }
        public string CreationText
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (RefUtilisateurCreation <= 0) { s = cR.GetTextRessource(9); }
                if (UtilisateurCreation != null && DCreation != null)
                {
                    s = cR.GetTextRessource(388) + " " + ((DateTime)DCreation).ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurCreation.Nom;
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
        public string ValidationText
        {
            get
            {
                string s = "";
                if (UtilisateurValide != null && DValide != null)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    s = cR.GetTextRessource(852) + " " + ((DateTime)DValide).ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurValide.Nom;
                }
                return s;
            }
        }
        public string ControleText
        {
            get
            {
                string s = "";
                if (UtilisateurControle != null && DControle != null)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    s = cR.GetTextRessource(853) + " " + ((DateTime)DControle).ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurControle.Nom;
                }
                return s;
            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Mark modifications
        /// </summary>
        public void MarkModification()
        {
            if (DCreation == null)
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
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if linked data exist.
        /// </summary>
        public string IsDeletable()
        {
            string r = "";
            int nbLinkedData = 0;
            //int nbLinkedData = DbContext.Entry(this).Collection(b => b.NonConformiteEtapeCollectivites).Query().Count();
            //nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteEtapeProduits).Query().Count();
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