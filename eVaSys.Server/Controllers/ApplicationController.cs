/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/11/2021
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class ApplicationController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ApplicationController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/application/{id}
        /// Retrieves the application with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing application</param>
        /// <returns>the application with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var application = DbContext.Applications
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefApplication == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (application == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Application, ApplicationViewModel>(application),
                JsonSettings);
        }

        /// <summary>
        /// Edit the application with the given {id}
        /// </summary>
        /// <param name="model">The ApplicationViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] ApplicationViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Application application;
            //Retrieve or create the application to edit
            if (model.RefApplication > 0)
            {
                application = DbContext.Applications
                        .Where(q => q.RefApplication == model.RefApplication)
                        .FirstOrDefault();
            }
            else
            {
                application = new Application();
                DbContext.Applications.Add(application);
            }
            // handle requests asking for non-existing applications
            if (application == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            application.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            application.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = application.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated application to the client.
                return new JsonResult(
                    _mapper.Map<Application, ApplicationViewModel>(application),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Application with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Application</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the application from the Database
            var application = DbContext.Applications.Where(i => i.RefApplication == id)
                .FirstOrDefault();

            // handle requests asking for non-existing applications
            if (application == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the application from the DbContext if applicable
            string del = application.IsDeletable();
            if (del == "")
            {
                // remove the equipementier from the DbContext.
                DbContext.Applications.Remove(application);
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
        /// GET: api/application/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.Application> req = DbContext.Applications;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Application[], ApplicationViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
