/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 19/10/2023
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for types de camion
    /// </summary>
    public class Ressource 
    {
        #region Constructor
        public Ressource()
        {
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public static string TextFilter(string filter, ApplicationDbContext dbContext)
        {
            StringBuilder r = new();
            if (filter != "")
            {
                foreach (string f in filter.Split(","))
                {
                    var res = dbContext.Ressources.Where(i => i.RefRessource == System.Convert.ToInt32(f)).FirstOrDefault();
                    r.Append(res.LibelleFRFR + ", ");
                }
            }
            if (r.Length != 0) { r = r.Remove(r.Length - 2, 2); }
            return r.ToString();
        }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefRessource { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            //int c = DbContext.Ressources.Where(q => EF.Functions.Collate(q.LibelleFRFR, "SQL_Latin1_General_CP1_CS_AS") == LibelleFRFR && q.RefRessource != RefRessource ).Count();
            //if (c > 0)
            //{
            //    CulturedRessources cR = new(currentCulture, DbContext);
            //    if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
            //}
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