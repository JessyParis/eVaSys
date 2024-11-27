/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/10/2019
/// ----------------------------------------------------------------------------------------------------- 
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for ProduitComposant
    /// </summary>
    public class ProduitComposant
    {
        #region Constructor
        public ProduitComposant()
        {
        }
        private ProduitComposant(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefProduitComposant { get; set; }
        public int RefProduit { get; set; }
        public int RefComposant { get; set; }
        private Produit _composant;
        public Produit Composant
        {
            get => LazyLoader.Load(this, ref _composant);
            set => _composant = value;
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