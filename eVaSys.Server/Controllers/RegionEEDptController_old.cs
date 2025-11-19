/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/01/2021
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class RegionEEDptController_old : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public RegionEEDptController_old(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/regioneedpt/{id}
        /// Retrieves the RegionEEDpt with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing RegionEEDpt</param>
        /// <returns>The RegionEEDpt with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var region = DbContext.RegionEEDpts
                    //.Include(r => r.UtilisateurCreation)
                    //.Include(r => r.UtilisateurModif)
                    .Where(i => i.RefRegionEEDpt == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (region == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<RegionEEDpt, RegionEEDptViewModel>(region),
                JsonSettings);
        }

        /// <summary>
        /// Edit the RegionEEDpt with the given {id}
        /// </summary>
        /// <param name="model">The RegionEEDptViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] RegionEEDptViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            RegionEEDpt region;
            //Retrieve or create the entity to edit
            if (model.RefRegionEEDpt > 0)
            {
                region = DbContext.RegionEEDpts
                        .Where(q => q.RefRegionEEDpt == model.RefRegionEEDpt)
                        .FirstOrDefault();
            }
            else
            {
                region = new RegionEEDpt();
                DbContext.RegionEEDpts.Add(region);
            }
            // handle requests asking for non-existing RegionEEDpt
            if (region == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            //region.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            //region.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = region.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated RegionEEDpt to the client.
                return new JsonResult(
                    _mapper.Map<RegionEEDpt, RegionEEDptViewModel>(region),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the RegionEEDpt with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing RegionEEDpt</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the RegionEEDpt from the Database
            var region = DbContext.RegionEEDpts.Where(i => i.RefRegionEEDpt == id).FirstOrDefault();
            // handle requests asking for non-existing RegionEEDpt
            if (region == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the RegionEEDpt from the DbContext if applicable
            string del = region.IsDeletable();
            if (del == "")
            {
                DbContext.RegionEEDpts.Remove(region);
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
        /// GET: api/regioneedpt/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.RegionEEDpt> req = DbContext.RegionEEDpts;
            //Get data
            var all = req.ToArray();//.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<RegionEEDpt[], RegionEEDptViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
