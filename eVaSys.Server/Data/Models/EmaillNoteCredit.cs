/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/06/21
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
    public class EmailNoteCredit
    {
        #region Constructor
        public EmailNoteCredit()
        {
        }
        private EmailNoteCredit(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefEmailNoteCredit { get; set; }
        public string RefSAGEDocument { get; set; }
        public string EmailTo { get; set; }
        public DateTime DCreation { get; set; }
        public int RefUtilisateurCreation { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.EmailNoteCredits.Where(q => q.RefSAGEDocument == RefSAGEDocument
                    && q.RefEmailNoteCredit != RefEmailNoteCredit)
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