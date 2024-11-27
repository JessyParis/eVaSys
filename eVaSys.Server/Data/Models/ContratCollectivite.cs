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
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for ContratCollectivite
    /// </summary>
    public class ContratCollectivite
    {
        #region Constructor
        public ContratCollectivite()
        {
        }
        private ContratCollectivite(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefContratCollectivite { get; set; }
        public int RefEntite { get; set; }
        public Entite Entite;
        public DateTime? DDebut { get; set; }
        public DateTime? DFin { get; set; }
        public bool Avenant { get; set; }
        public string Cmt { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            if (DDebut == null || DFin == null)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (DDebut == null) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(1174); }
                if (DFin == null) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(1175); }
            }
            return r;
        }
    }
}