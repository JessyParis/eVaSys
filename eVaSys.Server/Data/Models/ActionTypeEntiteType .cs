/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/04/2019
/// ----------------------------------------------------------------------------------------------------- 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for ActionTypeEntiteType
    /// </summary>
    public class ActionTypeEntiteType
    {
        #region Constructor
        public ActionTypeEntiteType()
        {
        }
        private ActionTypeEntiteType(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefActionTypeEntiteType { get; set; }
        public int RefActionType { get; set; }
        public int RefEntiteType { get; set; }
        private EntiteType _entiteType;
        public EntiteType EntiteType
        {
            get => LazyLoader.Load(this, ref _entiteType);
            set => _entiteType = value;
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