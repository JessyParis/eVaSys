/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 19/03/2020
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for DescriptionCVQProduit
    /// </summary>
    public class DescriptionCVQProduit
    {
        #region Constructor
        public DescriptionCVQProduit()
        {
        }
        private DescriptionCVQProduit(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefDescriptionCVQProduit { get; set; }
        public int RefDescriptionCVQ { get; set; }
        public int RefProduit { get; set; }
        private Produit _produit;
        public Produit Produit
        {
            get => LazyLoader.Load(this, ref _produit);
            set => _produit = value;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.DescriptionCVQProduits.Where(q => (q.RefDescriptionCVQ == RefDescriptionCVQ && q.RefProduit == RefProduit && q.RefDescriptionCVQProduit != RefDescriptionCVQProduit)).Count();
            if (c > 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "") { r += Environment.NewLine; }
                r += cR.GetTextRessource(410);
            }
            return r;
        }
    }
}