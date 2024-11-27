/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 19/07/2020
/// ----------------------------------------------------------------------------------------------------- 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for contact à l'adresse
    /// </summary>
    public class NonConformiteNonConformiteFamille
    {
        #region Constructor
        public NonConformiteNonConformiteFamille()
        {
        }
        private NonConformiteNonConformiteFamille(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefNonConformiteNonConformiteFamille { get; set; }
        public int RefNonConformite { get; set; }
        public int RefNonConformiteFamille { get; set; }
        private NonConformiteFamille _NonConformiteFamille;
        public NonConformiteFamille NonConformiteFamille
        {
            get => LazyLoader.Load(this, ref _NonConformiteFamille);
            set => _NonConformiteFamille = value;
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