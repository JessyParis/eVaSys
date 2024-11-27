/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 05/11/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for verrouillages
    /// </summary>
    public class Verrouillage
    {
        #region Constructor
        public Verrouillage()
        {
        }
        private Verrouillage(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefVerrouillage { get; set; }
        public int? RefUtilisateur { get; set; }
        private Utilisateur _utilisateur;
        public Utilisateur Utilisateur
        {
            get => LazyLoader.Load(this, ref _utilisateur);
            set => _utilisateur = value;
        }
        public string Donnee { get; set; }
        public int RefDonnee { get; set; }
        public DateTime DVerrouillage { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.Verrouillages.Where(q => q.Donnee == Donnee && q.RefDonnee == RefDonnee && q.RefVerrouillage != RefVerrouillage).Count();
            if (c > 0 || Donnee?.Length > 50)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
                if (Donnee?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
            }
            return r;
        }
    }
}