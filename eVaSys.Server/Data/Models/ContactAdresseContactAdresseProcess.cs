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
    /// Class for contact à l'adresse
    /// </summary>
    public class ContactAdresseContactAdresseProcess
    {
        #region Constructor
        public ContactAdresseContactAdresseProcess()
        {
        }
        private ContactAdresseContactAdresseProcess(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefContactAdresseContactAdresseProcess { get; set; }
        public int RefContactAdresse { get; set; }
        public int RefContactAdresseProcess { get; set; }
        private ContactAdresseProcess _contactAdresseProcess;
        public ContactAdresseProcess ContactAdresseProcess
        {
            get => LazyLoader.Load(this, ref _contactAdresseProcess);
            set => _contactAdresseProcess = value;
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