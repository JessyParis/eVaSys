/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/07/2019
/// ----------------------------------------------------------------------------------------------------- 
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for NonConformiteEtapeType
    /// </summary>
    public class NonConformiteEtapeType
    {
        #region Constructor
        public NonConformiteEtapeType()
        {
        }
        private NonConformiteEtapeType(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefNonConformiteEtapeType { get; set; }
        public int Ordre { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        [NotMapped]
        public string Libelle
        {
            get { return (currentCulture.Name == "fr-FR" ? LibelleFRFR : LibelleENGB); }
            set { }
        }
        public bool Controle { get; set; }
        public ICollection<NonConformiteEtape> NonConformiteEtapes { get; set; }
    }
}