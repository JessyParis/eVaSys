/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 04/05/2022
/// ----------------------------------------------------------------------------------------------------- 
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for VueRepartitionUnitaireDetail
    /// </summary>
    public class RepartitionDetail
    {
        #region Constructor
        public RepartitionDetail()
        {
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefRepartition { get; set; }
        public int? RefFournisseur { get; set; }
        public int? RefProduit { get; set; }
        public int? RefProcess { get; set; }
        public int? RefComposant { get; set; }
        public int? Poids { get; set; }
        public decimal? PUHT { get; set; }
        public DateTime? D { get; set; }
    }
}