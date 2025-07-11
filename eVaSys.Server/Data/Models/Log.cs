/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/09/2021
/// ----------------------------------------------------------------------------------------------------- 
using System;
using System.Globalization;
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    public class Log
    {
        #region Constructor
        public Log()
        {
        }
        private Log(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        #endregion
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefLog { get; set; }
        public string Commentaire { get; set; }
        public int? RefUtilisateur { get; set; }
        private Utilisateur _utilisateur;
        public Utilisateur Utilisateur
        {
            get => LazyLoader.Load(this, ref _utilisateur);
            set => _utilisateur = value;
        }
        public int? RefUtilisateurMaitre { get; set; }
        public DateTime DLogin { get; set; }
        public DateTime? DLogout { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            if (Utilisateur == null)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Utilisateur == null) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(495); }
            }
            return r;
        }
    }
}