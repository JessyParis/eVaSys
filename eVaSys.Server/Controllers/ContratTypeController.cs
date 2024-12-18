/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 18/12/2024
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class ContratTypeController : BaseApiController
    {
        #region Constructor
        public ContratTypeController(ApplicationDbContext context, IConfiguration configuration)
            : base(context, configuration) { }
        #endregion Constructor

        #region RESTful Conventions
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/contrattype/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlist")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.ContratType> req = DbContext.ContratTypes.AsNoTracking();
            //Get data
            ContratType[] all;
            if (CurrentContext.CurrentCulture.Name == "fr-FR")
            {
                all = req.OrderBy(el => el.LibelleFRFR).Select(p => new ContratType()
                {
                    RefContratType = p.RefContratType,
                    Libelle = p.LibelleFRFR,
                }).ToArray();
            }
            else
            {
                all = req.OrderBy(el => el.LibelleENGB).Select(p => new ContratType()
                {
                    RefContratType = p.RefContratType,
                    Libelle = p.LibelleENGB,
                }).ToArray();
            }
            //Return Json
            return new JsonResult(all,
                JsonSettings);
        }
        #endregion
    }
}
