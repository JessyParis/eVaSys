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
    public class ContactAdresseServiceFonction
    {
        #region Constructor
        public ContactAdresseServiceFonction()
        {
        }
        private ContactAdresseServiceFonction(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefContactAdresseServiceFonction { get; set; }
        public int RefContactAdresse { get; set; }
        public int? RefService { get; set; }
        private Service _service;
        public Service Service
        {
            get => LazyLoader.Load(this, ref _service);
            set => _service = value;
        }
        public int? RefFonction { get; set; }
        private Fonction _fonction;
        public Fonction Fonction
        {
            get => LazyLoader.Load(this, ref _fonction);
            set => _fonction = value;
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