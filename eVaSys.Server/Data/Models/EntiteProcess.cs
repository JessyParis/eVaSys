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
    /// Class for EntiteProcess
    /// </summary>
    public class EntiteProcess
    {
        #region Constructor
        public EntiteProcess()
        {
        }
        private EntiteProcess(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefEntiteProcess { get; set; }
        public int RefEntite { get; set; }
        public int RefProcess { get; set; }
        private Process _process;
        public Process Process
        {
            get => LazyLoader.Load(this, ref _process);
            set => _process = value;
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