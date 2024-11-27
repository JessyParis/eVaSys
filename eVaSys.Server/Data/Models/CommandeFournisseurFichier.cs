/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/10/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using System;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for CommandeFournisseurFichier
    /// </summary>
    public class CommandeFournisseurFichier
    {
        #region Constructor
        public CommandeFournisseurFichier()
        {
        }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefCommandeFournisseurFichier { get; set; }
        public int RefCommandeFournisseur { get; set; }
        public byte[] Corps { get; set; }
        public string CorpsBase64
        {
            get
            {
                return Convert.ToBase64String(Corps);
            }
        }
        public string Nom { get; set; }
        public string Extension { get; set; }
        public byte[]? Vignette { get; set; }
        public string VignetteBase64 
        {
            get
            {
                string r = "";
                if(Vignette != null) { r = Convert.ToBase64String(Vignette); }
                return r;
            } 
        }
        public byte[]? Miniature { get; set; }
        public string MiniatureBase64
        {
            get
            {
                string r = "";
                if (Miniature != null) { r = Convert.ToBase64String(Miniature); }
                return r;
            }
        }
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.CommandeFournisseurFichiers.Where(q => q.Nom == Nom && q.RefCommandeFournisseur == RefCommandeFournisseur && q.RefCommandeFournisseurFichier != RefCommandeFournisseurFichier).Count();
            if (c > 0 || Nom?.Length > 250 || Extension?.Length>50)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
                if (Nom?.Length > 250) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(621); }
                if (Extension?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(622); }
            }
            return r;
        }
    }
}