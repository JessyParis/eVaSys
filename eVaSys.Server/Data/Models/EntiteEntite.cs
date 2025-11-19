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
    /// Class for EntiteEntite
    /// </summary>
    public class EntiteEntite
    {
        #region Constructor
        public EntiteEntite()
        {
        }
        private EntiteEntite(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefEntiteEntite { get; set; }
        public int RefEntite { get; set; }
        public Entite Entite { get; set; }
        public int RefEntiteRtt { get; set; }
        public Entite EntiteRtt { get; set; }
        public bool Actif { get; set; } = true;
        public int? RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        public DateTime? DCreation { get; set; }
        public string Cmt { get; set; }
        [NotMapped]
        public string LibelleUtilisateurCreation { get; set; }
        [NotMapped]
        public int RefEntiteRttType { get; set; }
        [NotMapped]
        public string LibelleEntiteRtt { get; set; }
        [NotMapped]
        public bool? ActifEntiteRtt { get; set; }
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