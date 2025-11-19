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
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for ActionFichier
    /// </summary>
    public class ActionFichier
    {
        #region Constructor
        public ActionFichier()
        {
        }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefActionFichier { get; set; }
        public int RefAction { get; set; }
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
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            int c = DbContext.ActionFichiers.Where(q => q.Nom == Nom && q.RefAction != RefAction).Count();
            if (c > 0 || Nom?.Length > 250 || Extension?.Length > 50)
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