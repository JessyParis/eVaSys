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
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for EntiteCamionType
    /// </summary>
    public class EntiteCamionType
    {
        #region Constructor
        public EntiteCamionType()
        {
        }
        private EntiteCamionType(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefEntiteCamionType { get; set; }
        public int RefEntite { get; set; }
        public int RefCamionType { get; set; }
        private CamionType _camionType;
        public CamionType CamionType
        {
            get => LazyLoader.Load(this, ref _camionType);
            set => _camionType = value;
        }
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