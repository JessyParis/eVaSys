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
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for NonConformiteFichierType
    /// </summary>
    public class NonConformiteFichierType
    {
        #region Constructor
        public NonConformiteFichierType()
        {
        }
        private NonConformiteFichierType(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefNonConformiteFichierType { get; set; }
        public string Libelle{ get; set; }
        public ICollection<NonConformiteFichier> NonConformiteFichiers { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.NonConformiteFichierTypes.Where(q => (q.Libelle == Libelle) && q.RefNonConformiteFichierType != RefNonConformiteFichierType).Count();
            if (c > 0 || (Libelle?.Length > 50 ))
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
                if (Libelle?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(399); }
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.NonConformiteFichiers).Query().Count();
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