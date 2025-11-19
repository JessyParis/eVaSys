/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/07/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for EntiteProduit
    /// </summary>
    public class EntiteProduit
    {
        #region Constructor
        public EntiteProduit()
        {
        }
        private EntiteProduit(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefEntiteProduit { get; set; }
        public int RefEntite { get; set; }
        public int RefProduit { get; set; }
        private Produit _produit;
        public Produit Produit
        {
            get => LazyLoader.Load(this, ref _produit);
            set => _produit = value;
        }
        public bool Interdit { get; set; }
        public int? RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        public DateTime? DCreation { get; set; }
        public string Cmt { get; set; }
        [NotMapped]
        public string LibelleUtilisateurCreation { get; set; }
        [NotMapped]
        public string _creationText { get; set; }
        [NotMapped]
        public string CreationText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_creationText))
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    string s = "";
                    if (RefUtilisateurCreation <= 0) { s = cR.GetTextRessource(9); }
                    if (UtilisateurCreation != null)
                    {
                        s = cR.GetTextRessource(388) + " " + ((DateTime)DCreation).ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurCreation.Nom;
                    }
                    _creationText = s;
                }
                return _creationText;
            }
            set { _creationText = value; }
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
    }
}