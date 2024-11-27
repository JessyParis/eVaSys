/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/10/2021
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for région CITEO
    /// </summary>
    public class RegionEEDpt
    {
        #region Constructor
        public RegionEEDpt()
        {
        }
        private RegionEEDpt(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefRegionEEDpt { get; set; }
        public int RefRegionEE { get; set; }
        public string RefDpt { get; set; }
        private Dpt _dpt;
        public Dpt Dpt
        {
            get => LazyLoader.Load(this, ref _dpt);
            set => _dpt = value;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.RegionEEDpts.Where(q => q.RefRegionEE == RefRegionEE && q.RefDpt == RefDpt && q.RefRegionEEDpt != RefRegionEEDpt).Count();
            if (c > 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
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
            return r;
        }
    }
}