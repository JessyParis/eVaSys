/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/09/2021
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
    public class RepriseTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public RepriseTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/reprisetype/{id}
        /// Retrieves the RepriseType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing RepriseType</param>
        /// <returns>The RepriseType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var repriseType = DbContext.RepriseTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefRepriseType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (repriseType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<RepriseType, RepriseTypeViewModel>(repriseType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the RepriseType with the given {id}
        /// </summary>
        /// <param name="model">The RepriseTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]RepriseTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            RepriseType repriseType;
            //Retrieve or create the entity to edit
            if (model.RefRepriseType > 0)
            {
                repriseType = DbContext.RepriseTypes
                        .Where(q => q.RefRepriseType == model.RefRepriseType)
                        .FirstOrDefault();
            }
            else
            {
                repriseType = new RepriseType();
                DbContext.RepriseTypes.Add(repriseType);
            }
            // handle requests asking for non-existing repriseTypezes
            if (repriseType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            repriseType.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            repriseType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = repriseType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated RepriseType to the client.
                return new JsonResult(
                    _mapper.Map<RepriseType, RepriseTypeViewModel>(repriseType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the RepriseType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the repriseType from the Database
            var repriseType = DbContext.RepriseTypes.Where(i => i.RefRepriseType == id).FirstOrDefault();
            // handle requests asking for non-existing repriseTypezes
            if (repriseType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = repriseType.IsDeletable();
            if (del == "")
            {
                DbContext.RepriseTypes.Remove(repriseType);
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
        /// GET: api/repriseType/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.RepriseType> req = DbContext.RepriseTypes;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<RepriseType[], RepriseTypeViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
