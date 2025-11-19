/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/03/2020
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for ControleTypes
    /// </summary>
    public class ControleType
    {
        #region Constructor
        public ControleType()
        {
        }
        private ControleType(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefControleType { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public ICollection<Controle> Controles { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.ControleTypes.Where(q => (q.LibelleFRFR == LibelleFRFR || q.LibelleFRFR == LibelleFRFR) && q.RefControleType != RefControleType).Count();
            if (c > 0 || (LibelleFRFR?.Length > 50 || LibelleENGB?.Length > 50))
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.Controles).Query().Count();
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