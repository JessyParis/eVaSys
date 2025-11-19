/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/04/2020
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for CVQDescriptionCVQs
    /// </summary>
    public class CVQDescriptionCVQ
    {
        #region Constructor
        public CVQDescriptionCVQ()
        {
        }
        private CVQDescriptionCVQ(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefCVQDescriptionCVQ { get; set; }
        public int RefCVQ { get; set; }
        public int RefDescriptionCVQ { get; set; }
        private DescriptionCVQ _descriptionCVQ;
        public DescriptionCVQ DescriptionCVQ
        {
            get => LazyLoader.Load(this, ref _descriptionCVQ);
            set => _descriptionCVQ = value;
        }
        public int Nb { get; set; }
        public int Ordre { get; set; }
        public int LimiteBasse { get; set; }
        public int LimiteHaute { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        [NotMapped]
        public string LibelleCultured
        {
            get
            {
                if (currentCulture.TwoLetterISOLanguageName == "fr") { return LibelleFRFR; }
                else { return LibelleENGB; }
            }
        }
        [NotMapped]
        public int Qualite
        {
            get
            {
                int r = 0;
                if (Nb <= LimiteBasse) { r = 1; }
                else if (Nb > LimiteHaute) { r = 3; }
                else { r = 2; }
                return r;
            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.CVQDescriptionCVQs
                .Where(q => q.RefCVQ == RefCVQ && q.RefDescriptionCVQ == RefDescriptionCVQ && q.RefCVQDescriptionCVQ != RefCVQDescriptionCVQ)
                .Count();
            if (c > 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                r += cR.GetTextRessource(410);
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
            int nbLinkedData = 0;
            if (nbLinkedData != 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "") { r += Environment.NewLine; }
                r += cR.GetTextRessource(393);
            }
            return r;
        }
    }
}