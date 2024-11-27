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
    /// Class for 
    /// </summary>
    public class ActionActionType
    {
        #region Constructor
        public ActionActionType()
        {
        }
        private ActionActionType(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefActionActionType { get; set; }
        public int RefAction { get; set; }
        public int RefActionType { get; set; }
        private ActionType _actionType;
        public ActionType ActionType
        {
            get => LazyLoader.Load(this, ref _actionType);
            set => _actionType = value;
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