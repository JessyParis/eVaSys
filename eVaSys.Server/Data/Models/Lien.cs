/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/08/2021
/// ----------------------------------------------------------------------------------------------------- 
using System;
using System.Globalization;
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    public class Lien
    {
        #region Constructor
        public Lien()
        {
        }
        private Lien(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        #endregion
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefLien { get; set; }
        public string Valeur { get; set; }
        public int? RefUtilisateur { get; set; }
        private Utilisateur _utilisateur;
        public Utilisateur Utilisateur
        {
            get => LazyLoader.Load(this, ref _utilisateur);
            set => _utilisateur = value;
        }
        public DateTime? DUtilisation { get; set; }
        public DateTime DCreation { get; set; }
        public int? RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            if (Valeur?.Length > 200 || Utilisateur == null)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Valeur?.Length > 200) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(495); }
                if (Utilisateur == null) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(495); }
            }
            return r;
        }
    }
}