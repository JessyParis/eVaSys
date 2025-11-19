/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 19/10/2023
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class RessourceController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public RessourceController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/ressource/{id}
        /// Retrieves the Ressource with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Ressource</param>
        /// <returns>The Ressource with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var ressource = DbContext.Ressources
                    .Where(i => i.RefRessource == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (ressource == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Ressource, RessourceViewModel>(ressource),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Ressource with the given {id}
        /// </summary>
        /// <param name="model">The RessourceViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] RessourceViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            Ressource ressource;
            //Retrieve or create the entity to edit
            if (model.RefRessource > 0)
            {
                ressource = DbContext.Ressources
                        .Where(q => q.RefRessource == model.RefRessource)
                        .FirstOrDefault();
            }
            else
            {
                ressource = new Ressource();
                DbContext.Ressources.Add(ressource);
            }
            // handle requests asking for non-existing ressourcezes
            if (ressource == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            ressource.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(model.LibelleFRFR);
            ressource.LibelleENGB = Utils.Utils.SetEmptyStringToNull(model.LibelleENGB);
            //Check validation
            string valid = ressource.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Ressource to the client.
                return new JsonResult(
                    _mapper.Map<Ressource, RessourceViewModel>(ressource),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Ressource with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the ressource from the Database
            var ressource = DbContext.Ressources.Where(i => i.RefRessource == id).FirstOrDefault();
            // handle requests asking for non-existing ressourcezes
            if (ressource == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = ressource.IsDeletable();
            if (del == "")
            {
                DbContext.Ressources.Remove(ressource);
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
        #endregion
    }
}
