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
    public class MotifAnomalieChargementController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public MotifAnomalieChargementController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

            #region RESTful Conventions
            /// <summary>
            /// GET: evapi/MotifAnomalieChargement/{id}
            /// Retrieves the MotifAnomalieChargement with the given {id}
            /// </summary>
            /// <param name="id">The ID of an existing MotifAnomalieChargement</param>
            /// <returns>The MotifAnomalieChargement with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var motifAnomalieChargement = DbContext.MotifAnomalieChargements
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefMotifAnomalieChargement == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (motifAnomalieChargement == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<MotifAnomalieChargement, MotifAnomalieChargementViewModel>(motifAnomalieChargement),
                JsonSettings);
        }

        /// <summary>
        /// Edit the MotifAnomalieChargement with the given {id}
        /// </summary>
        /// <param name="model">The MotifAnomalieChargementViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]MotifAnomalieChargementViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            MotifAnomalieChargement motifAnomalieChargement;
            //Retrieve or create the entity to edit
            if (model.RefMotifAnomalieChargement > 0)
            {
                motifAnomalieChargement = DbContext.MotifAnomalieChargements
                        .Where(q => q.RefMotifAnomalieChargement == model.RefMotifAnomalieChargement)
                        .FirstOrDefault();
            }
            else
            {
                motifAnomalieChargement = new MotifAnomalieChargement();
                DbContext.MotifAnomalieChargements.Add(motifAnomalieChargement);
            }

            // handle requests asking for non-existing MotifAnomalieChargementzes
            if (motifAnomalieChargement == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            motifAnomalieChargement.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            motifAnomalieChargement.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = motifAnomalieChargement.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated MotifAnomalieChargement to the client.
                return new JsonResult(
                    _mapper.Map<MotifAnomalieChargement, MotifAnomalieChargementViewModel>(motifAnomalieChargement),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the MotifAnomalieChargement with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the MotifAnomalieChargement from the Database
            var motifAnomalieChargement = DbContext.MotifAnomalieChargements.Where(i => i.RefMotifAnomalieChargement == id)
                .FirstOrDefault();

            // handle requests asking for non-existing MotifAnomalieChargementzes
            if (motifAnomalieChargement == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = motifAnomalieChargement.IsDeletable();
            if (del == "")
            {
                DbContext.MotifAnomalieChargements.Remove(motifAnomalieChargement);
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
        /// GET: api/MotifAnomalieChargement/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Where
            System.Linq.IQueryable<eVaSys.Data.MotifAnomalieChargement> req = DbContext.MotifAnomalieChargements;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<MotifAnomalieChargement[], MotifAnomalieChargementViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
