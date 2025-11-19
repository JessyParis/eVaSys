/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/03/2020
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for ControleDescriptionControles
    /// </summary>
    public class ControleDescriptionControle
    {
        #region Constructor
        public ControleDescriptionControle()
        {
        }
        private ControleDescriptionControle(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefControleDescriptionControle { get; set; }
        public int RefControle { get; set; }
        public int RefDescriptionControle { get; set; }
        private DescriptionControle _descriptionControle;
        public DescriptionControle DescriptionControle
        {
            get => LazyLoader.Load(this, ref _descriptionControle);
            set => _descriptionControle = value;
        }
        public int Ordre { get; set; }
        public double Poids { get; set; }
        public bool CalculLimiteConformite { get; set; }
        public string Cmt { get; set; }
        public bool CmtObligatoire { get; set; }
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
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.ControleDescriptionControles
                .Where(q => q.RefControle == RefControle && q.RefDescriptionControle == RefDescriptionControle && q.RefControleDescriptionControle != RefControleDescriptionControle).Count();
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