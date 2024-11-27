/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 29/11/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for Email
    /// </summary>
    public class EmailIncitationQualite
    {
        #region Constructor
        public EmailIncitationQualite()
        {
        }
        private EmailIncitationQualite(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefEmailIncitationQualite { get; set; }
        public int RefEntite { get; set; }
        public string EmailTo { get; set; }
        public int Annee { get; set; }
        public DateTime DCreation { get; set; }
        public int RefUtilisateurCreation { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.EmailIncitationQualites.Where(q => q.Annee == Annee && q.RefEntite == RefEntite
                    && q.RefEmailIncitationQualite != RefEmailIncitationQualite)
                .Count();
            if (c > 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
            }
            return r;
        }
    }
}