/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 25/05/2021
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class SearchController : BaseApiController
    {
        #region Constructor
        public SearchController(ApplicationDbContext context, IConfiguration configuration)
            : base(context, configuration) { }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// Get search results
        /// </summary>
        [HttpGet("SearchResults")]
        public IActionResult SearchResults()
        {
            string skip = Request.Headers["skip"].ToString();
            string searchText = Request.Headers["searchText"].ToString();
            string elementType = Request.Headers["elementType"].ToString();
            //Process 
            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(elementType) && int.TryParse(skip, out int sk))
            {
                int[] e = new int[0];
                if (elementType == Enumerations.SearchElementType.Entite.ToString())
                {
                    var req = DbContext.Entites
                        .Include(i => i.Adresses)
                        .Where(e => e.Libelle.Contains(EF.Functions.Collate(searchText, "SQL_Latin1_General_CP1_CI_AI"))
                            || e.CodeEE == searchText
                            || e.Adresses.Any(a => a.Ville.Contains(EF.Functions.Collate(searchText, "SQL_Latin1_General_CP1_CI_AI"))));
                    if (CurrentContext.filterGlobalActif)
                    {
                        req = req.Where(e => e.Actif == true);
                    }
                    e = req.Select(e => new { e.RefEntite }).Distinct().OrderBy(e => e.RefEntite).Skip(sk).Take(11)
                        .Select(e => e.RefEntite).ToArray();
                }
                if (elementType == Enumerations.SearchElementType.ContactAdresse.ToString())
                {
                    var req = DbContext.ContactAdresses
                        .Where(e => e.Contact.Nom.Contains(EF.Functions.Collate(searchText, "SQL_Latin1_General_CP1_CI_AI"))
                        || e.Tel.Replace(" ", "") == searchText.Replace(" ", "")
                        || e.TelMobile.Replace(" ", "") == searchText.Replace(" ", ""));
                    if (CurrentContext.filterGlobalActif)
                    {
                        req = req.Where(e => e.Actif == true && (e.RefAdresse == null || e.Adresse.Actif == true) && e.Entite.Actif == true);
                    }
                    e = req.Select(e => new { e.RefContactAdresse }).Distinct().OrderBy(e => e.RefContactAdresse).Skip(sk).Take(11)
                        .Select(e => e.RefContactAdresse).ToArray();
                }
                //End
                return Ok(e);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// Get search result details
        /// </summary>
        [HttpGet("SearchResultDetails")]
        public IActionResult SearchResultDetails()
        {
            string elementRef = Request.Headers["elementRef"].ToString();
            string elementType = Request.Headers["elementType"].ToString();
            string searchText = Request.Headers["searchText"].ToString();
            string resultType = "", description = "";
            bool actif = true;
            //Process 
            if (int.TryParse(elementRef, out int eR))
            {
                if (eR >= 0)
                {
                    if (elementType == Enumerations.SearchElementType.Entite.ToString())
                    {
                        try
                        {
                            var e = DbContext.Entites.Find(eR);
                            resultType = Utils.Utils.SearchEntiteType(e.RefEntiteType);
                            actif = e.Actif ?? false;
                            e.EntiteType.currentCulture = CurrentContext.CurrentCulture;
                            description = HttpUtility.HtmlEncode(e.EntiteType.Libelle);
                            if (!string.IsNullOrEmpty(e.CodeEE)) { description += Utils.Utils.FormatSearchResult(" (" + e.CodeEE + ")", searchText); }
                            description += Environment.NewLine + Utils.Utils.FormatSearchResult(e.Libelle, searchText);
                            //Addresses
                            DbContext.Entry(e).Collection(c => c.Adresses).Load();
                            if (e.Adresses != null)
                            {
                                description += Environment.NewLine + HttpUtility.HtmlEncode(" (");
                                description += Utils.Utils.FormatSearchResult(string.Join(", ", e.Adresses.Select(s => (s.Ville)).Distinct().ToArray()), searchText);
                                description += HttpUtility.HtmlEncode(")");
                            }
                            //Contract if Collectivité
                            if (e.EntiteType.RefEntiteType == 1)
                            {
                                DbContext.Entry(e).Collection(c => c.ContratCollectivites).Load();
                                if (e.ContratCollectivites.Where(p => DateTime.Now >= p.DDebut && DateTime.Now <= p.DFin).Count() > 0)
                                {
                                    description += Environment.NewLine + CurrentContext.CulturedRessources.GetHTMLRessource(1077);
                                }
                            }
                            //Format
                            description = Utils.Utils.FormatHTMLNewLines(description);
                        }
                        catch
                        {
                            return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
                        }
                    }
                    if (elementType == Enumerations.SearchElementType.ContactAdresse.ToString())
                    {
                        try
                        {
                            var e = DbContext.ContactAdresses.Find(eR);
                            DbContext.Entry(e).Reference(c => c.Adresse).Load();
                            DbContext.Entry(e).Reference(c => c.Entite).Load();
                            resultType = Utils.Utils.SearchEntiteType(e.Entite.RefEntiteType);
                            actif = e.Actif ?? false;
                            e.Entite.EntiteType.currentCulture = CurrentContext.CurrentCulture;
                            description = HttpUtility.HtmlEncode(e.Entite.EntiteType.Libelle);
                            if (!string.IsNullOrEmpty(e.Entite.CodeEE)) { description += HttpUtility.HtmlEncode(" (" + e.Entite.CodeEE + ")"); }
                            if (e.Adresse != null)
                            {
                                description += Environment.NewLine + HttpUtility.HtmlEncode(e.Adresse.Entite.Libelle);
                            }
                            description += Environment.NewLine + Utils.Utils.FormatSearchResult(e.Contact.Prenom + " " + e.Contact.Nom, searchText);
                            if (!string.IsNullOrEmpty(e.TelMobile)) { description += Environment.NewLine + Utils.Utils.FormatSearchResult(e.TelMobile, searchText); }
                            else if (!string.IsNullOrEmpty(e.Tel)) { description += Environment.NewLine + Utils.Utils.FormatSearchResult(e.Tel, searchText); }
                            //Format
                            description = Utils.Utils.FormatHTMLNewLines(description);
                        }
                        catch
                        {
                            return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
                        }
                    }
                }
                else
                {
                    resultType = "unknown";
                    description = "<strong>" + HttpUtility.HtmlEncode("...") + "</strong><br/><br/>";
                    description += HttpUtility.HtmlEncode(CurrentContext.CulturedRessources.GetTextRessource(1076));
                }
                return Ok(new { resultType, description, actif });
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        #endregion
    }
}
