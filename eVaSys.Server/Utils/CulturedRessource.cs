/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/07/2018
/// ----------------------------------------------------------------------------------------------------- 

using eVaSys.Data;
using System.Globalization;
using System.Web;

namespace eVaSys.Utils
{
    /// <summary>
    /// Multilingual ressources management
    /// </summary>
    public class CulturedRessources
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CulturedRessources(CultureInfo currentCulture, ApplicationDbContext context)
        {
            CurrentCulture = currentCulture;
            _dbContext = context;
        }
        private readonly ApplicationDbContext _dbContext;
        public CultureInfo CurrentCulture { get; private set; } = new CultureInfo("fr-FR");
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Rénvoi d'une ressource au format texte
        public string GetTextRessource(int refRessource)
        {
            string r = "---???---";
            try
            {
                var res = _dbContext.Ressources.Find(refRessource);
                switch (CurrentCulture.Name)
                {
                    case "en-GB":
                        r = res.LibelleENGB ?? "";
                        break;
                    default:
                        r = res.LibelleFRFR ?? "";
                        break;
                }
                if (r.Trim() == "")
                {
                    r = "---???---";
                }
            }
            catch { }
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Rénvoi d'une ressource au format texte pour intégration dans une chaine SQL
        public string GetTextSQLRessource(int refRessource)
        {
            string r = "---???---";
            try
            {
                var res = _dbContext.Ressources.Find(refRessource);
                switch (CurrentCulture.Name)
                {
                    case "en-GB":
                        r = res.LibelleENGB ?? "";
                        break;
                    default:
                        r = res.LibelleFRFR ?? "";
                        break;
                }
                if (r.Trim() == "")
                {
                    r = "---???---";
                }
            }
            catch { }
            return r.Replace("'", "''");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Rénvoi d'une ressource au format texte pour insertion javascript
        public string GetTextJavascriptRessource(int refRessource)
        {
            string r = "---???---";
            try
            {
                var res = _dbContext.Ressources.Find(refRessource);
                switch (CurrentCulture.Name)
                {
                    case "en-GB":
                        r = res.LibelleENGB ?? "";
                        break;
                    default:
                        r = res.LibelleFRFR ?? "";
                        break;
                }
                if (r.Trim() == "")
                {
                    r = "---???---";
                }
            }
            catch { }
            return r.Replace(@"'", @"\'");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Rénvoi d'une ressource au format HTML
        public string GetHTMLRessource(int refRessource)
        {
            string r = "---???---";
            try
            {
                var res = _dbContext.Ressources.Find(refRessource);
                switch (CurrentCulture.Name)
                {
                    case "en-GB":
                        r = res.LibelleENGB ?? "";
                        break;
                    default:
                        r = res.LibelleFRFR ?? "";
                        break;
                }
                if (r.Trim() == "")
                {
                    r = "---???---";
                }
            }
            catch { }
            return HttpUtility.HtmlEncode(r).Replace(Environment.NewLine, "<BR/>");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Rénvoi d'une ressource au format HTML pour les boutons (espaces insecables)
        public string GetHTMLButtonRessource(int refRessource)
        {
            string r = "---???---";
            try
            {
                var res = _dbContext.Ressources.Find(refRessource);
                switch (CurrentCulture.Name)
                {
                    case "en-GB":
                        r = res.LibelleENGB ?? "";
                        break;
                    default:
                        r = res.LibelleFRFR ?? "";
                        break;
                }
                if (r.Trim() == "")
                {
                    r = "---???---";
                }
            }
            catch { }
            return HttpUtility.HtmlEncode(r).Replace(Environment.NewLine, "<BR/>").Replace(" ", "&nbsp;");
        }
    }
}
