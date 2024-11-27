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
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for FicheControleDescriptionReceptions
    /// </summary>
    public class FicheControleDescriptionReception
    {
        #region Constructor
        public FicheControleDescriptionReception()
        {
        }
        private FicheControleDescriptionReception(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefFicheControleDescriptionReception { get; set; }
        public int RefFicheControle { get; set; }
        public int RefDescriptionReception { get; set; }
        private DescriptionReception _descriptionReception;
        public DescriptionReception DescriptionReception
        {
            get => LazyLoader.Load(this, ref _descriptionReception);
            set => _descriptionReception = value;
        }
        public string Cmt { get; set; }
        public bool? Positif { get; set; }
        public int? Ordre { get; set; }
        public string LibelleFRFR { get; set; }
        public string OuiFRFR { get; set; }
        public string NonFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public string OuiENGB { get; set; }
        public string NonENGB { get; set; }
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
        public string OuiCultured
        {
            get
            {
                if (currentCulture.TwoLetterISOLanguageName == "fr") { return OuiFRFR; }
                else { return OuiENGB; }
            }
        }
        [NotMapped]
        public string NonCultured
        {
            get
            {
                if (currentCulture.TwoLetterISOLanguageName == "fr") { return NonFRFR; }
                else { return NonENGB; }
            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.FicheControleDescriptionReceptions.Where(q => q.RefFicheControle == RefFicheControle && q.RefDescriptionReception != RefDescriptionReception && q.RefFicheControleDescriptionReception != RefFicheControleDescriptionReception).Count();
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