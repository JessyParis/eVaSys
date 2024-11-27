/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/07/2019
/// ----------------------------------------------------------------------------------------------------- 
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for EntiteStandard
    /// </summary>
    public class EntiteStandard
    {
        #region Constructor
        public EntiteStandard()
        {
        }
        private EntiteStandard(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefEntiteStandard { get; set; }
        public int RefEntite { get; set; }
        public int RefStandard { get; set; }
        private Standard _standard;
        public Standard Standard
        {
            get => LazyLoader.Load(this, ref _standard);
            set => _standard = value;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            return r;
        }
    }
}