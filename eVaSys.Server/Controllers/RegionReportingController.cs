/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/06/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class RegionReportingController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public RegionReportingController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/regionreporting/{id}
        /// Retrieves the RegionReporting with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing RegionReporting</param>
        /// <returns>The RegionReporting with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var region = DbContext.RegionReportings
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefRegionReporting == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (region == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<RegionReporting, RegionReportingViewModel>(region),
                JsonSettings);
        }

        /// <summary>
        /// Edit the RegionReporting with the given {id}
        /// </summary>
        /// <param name="model">The RegionReportingViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]RegionReportingViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            RegionReporting region;
            //Retrieve or create the entity to edit
            if (model.RefRegionReporting > 0)
            {
                region = DbContext.RegionReportings
                        .Where(q => q.RefRegionReporting == model.RefRegionReporting)
                        .FirstOrDefault();
            }
            else
            {
                region = new RegionReporting();
                DbContext.RegionReportings.Add(region);
            }
            // handle requests asking for non-existing RegionReporting
            if (region == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            region.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            region.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = region.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated RegionReporting to the client.
                return new JsonResult(
                    _mapper.Map<RegionReporting, RegionReportingViewModel>(region),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the RegionReporting with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing RegionReporting</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the RegionReporting from the Database
            var region = DbContext.RegionReportings.Where(i => i.RefRegionReporting == id).FirstOrDefault();
            // handle requests asking for non-existing RegionReporting
            if (region == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the RegionReporting from the DbContext if applicable
            string del = region.IsDeletable();
            if (del == "")
            {
                DbContext.RegionReportings.Remove(region);
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return an HTTP Status 200 (OK).
                return Ok();
            }
            else
            {
                return Conflict(new ConflictError(del));
            }
        }
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/regionreporting/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.RegionReporting> req = DbContext.RegionReportings;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<RegionReporting[], RegionReportingViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
