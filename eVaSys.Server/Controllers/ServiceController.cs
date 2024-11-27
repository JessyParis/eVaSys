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
    public class ServiceController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ServiceController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/service/{id}
        /// Retrieves the Service with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing service</param>
        /// <returns>The Service with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var service = DbContext.Services
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefService == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (service == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Service, ServiceViewModel>(service),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Service with the given {id}
        /// </summary>
        /// <param name="model">The ServiceViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]ServiceViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            Service service;
            //Retrieve or create the entity to edit
            if (model.RefService > 0)
            {
                service = DbContext.Services
                        .Where(q => q.RefService == model.RefService)
                        .FirstOrDefault();
            }
            else
            {
                service = new Service();
                DbContext.Services.Add(service);
            }
            // handle requests asking for non-existing services
            if (service == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            service.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            service.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = service.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Service to the client.
                return new JsonResult(
                    _mapper.Map<Service, ServiceViewModel>(service),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Service with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Service</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the Service from the Database
            var service = DbContext.Services.Where(i => i.RefService == id).FirstOrDefault();
            // handle requests asking for non-existing Services
            if (service == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = service.IsDeletable();
            if (del == "")
            {
                DbContext.Services.Remove(service);
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
        /// GET: api/service/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.Service> req = DbContext.Services;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Service[], ServiceViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
