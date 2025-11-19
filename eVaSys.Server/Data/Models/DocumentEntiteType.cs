/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 11/03/2024
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for document à l'module
    /// </summary>
    public class DocumentEntiteType
    {
        #region Constructor
        public DocumentEntiteType()
        {
        }
        private DocumentEntiteType(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefDocumentEntiteType { get; set; }
        public int RefDocument { get; set; }
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
            int c = DbContext.DocumentEntiteTypes.Where(q => q.RefDocument == RefDocument && q.RefEntiteType == RefEntiteType && RefDocumentEntiteType != RefDocumentEntiteType)
                .Count();
            if (c > 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
            }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if linked data exist.
        /// </summary>
        public string IsDeletable()
        {
            string r = "";
            return r;
        }
    }
}