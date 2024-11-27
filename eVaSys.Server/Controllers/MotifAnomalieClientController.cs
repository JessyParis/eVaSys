/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 09/07/2019
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
    public class MotifAnomalieClientController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public MotifAnomalieClientController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/MotifAnomalieClient/{id}
        /// Retrieves the MotifAnomalieClient with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing MotifAnomalieClient</param>
        /// <returns>The MotifAnomalieClient with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var motifAnomalieClient = DbContext.MotifAnomalieClients
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefMotifAnomalieClient == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (motifAnomalieClient == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<MotifAnomalieClient, MotifAnomalieClientViewModel>(motifAnomalieClient),
                JsonSettings);
        }

        /// <summary>
        /// Edit the MotifAnomalieClient with the given {id}
        /// </summary>
        /// <param name="model">The MotifAnomalieClientViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]MotifAnomalieClientViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            MotifAnomalieClient motifAnomalieClient;
            //Retrieve or create the entity to edit
            if (model.RefMotifAnomalieClient > 0)
            {
                motifAnomalieClient = DbContext.MotifAnomalieClients
                        .Where(q => q.RefMotifAnomalieClient == model.RefMotifAnomalieClient)
                        .FirstOrDefault();
            }
            else
            {
                motifAnomalieClient = new MotifAnomalieClient();
                DbContext.MotifAnomalieClients.Add(motifAnomalieClient);
            }
            // handle requests asking for non-existing MotifAnomalieClientzes
            if (motifAnomalieClient == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            motifAnomalieClient.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            motifAnomalieClient.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = motifAnomalieClient.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated MotifAnomalieClient to the client.
                return new JsonResult(
                    _mapper.Map<MotifAnomalieClient, MotifAnomalieClientViewModel>(motifAnomalieClient),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the MotifAnomalieClient with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the MotifAnomalieClient from the Database
            var motifAnomalieClient = DbContext.MotifAnomalieClients.Where(i => i.RefMotifAnomalieClient == id)
                .FirstOrDefault();

            // handle requests asking for non-existing MotifAnomalieClientzes
            if (motifAnomalieClient == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = motifAnomalieClient.IsDeletable();
            if (del == "")
            {
                DbContext.MotifAnomalieClients.Remove(motifAnomalieClient);
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
        /// GET: api/MotifAnomalieClient/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Where
            System.Linq.IQueryable<eVaSys.Data.MotifAnomalieClient> req = DbContext.MotifAnomalieClients;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<MotifAnomalieClient[], MotifAnomalieClientViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
