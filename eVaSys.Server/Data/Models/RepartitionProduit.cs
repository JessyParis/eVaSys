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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for RepartitionProduit
    /// </summary>
    public class RepartitionProduit
    {
        #region Constructor
        public RepartitionProduit()
        {
        }
        private RepartitionProduit(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefRepartitionProduit { get; set; }
        public int RefRepartition { get; set; }
        public int? RefProcess { get; set; }
        private Process _process;
        public Process Process
        {
            get => LazyLoader.Load(this, ref _process);
            set => _process = value;
        }
        public int? RefFournisseur { get; set; }
        private Entite _fournisseur;
        public Entite Fournisseur
        {
            get => LazyLoader.Load(this, ref _fournisseur);
            set => _fournisseur = value;
        }
        public int? RefProduit { get; set; }
        private Produit _produit;
        public Produit Produit
        {
            get => LazyLoader.Load(this, ref _produit);
            set => _produit = value;
        }
        public int Poids { get; set; }
        public decimal? PUHT { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tblRefRepartitionProduit where RefRepartitionProduit=" + RefRepartitionProduit, DbContext.Database.GetDbConnection()) != "0");
            //XXXXXXXXXXXXXXXXXXXXXXXXXX
            int c = DbContext.RepartitionProduits.Where(q => (q.RefRepartition == RefRepartition && q.RefProduit == RefProduit && q.RefProcess == RefProcess && q.RefFournisseur == RefFournisseur)).Count();
            if ((!existsInDB && c > 0) || (existsInDB && c > 1))
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "") { r += Environment.NewLine; }
                r += cR.GetTextRessource(410);
            }
            return r;
        }
    }
}