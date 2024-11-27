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
    public class ContactAdresseDocumentType
    {
        #region Constructor
        public ContactAdresseDocumentType()
        {
        }
        private ContactAdresseDocumentType(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefContactAdresseDocumentType { get; set; }
        public int RefDocumentType { get; set; }
        private DocumentType _documentType;
        public DocumentType DocumentType
        {
            get => LazyLoader.Load(this, ref _documentType);
            set => _documentType = value;
        }
        public int RefContactAdresse { get; set; }
        public int Nb { get; set; }
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