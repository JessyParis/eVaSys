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

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class MotifAnomalieTransporteurController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public MotifAnomalieTransporteurController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/MotifAnomalieTransporteur/{id}
        /// Retrieves the MotifAnomalieTransporteur with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing MotifAnomalieTransporteur</param>
        /// <returns>The MotifAnomalieTransporteur with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var motifAnomalieTransporteur = DbContext.MotifAnomalieTransporteurs
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefMotifAnomalieTransporteur == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (motifAnomalieTransporteur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<MotifAnomalieTransporteur, MotifAnomalieTransporteurViewModel>(motifAnomalieTransporteur),
                JsonSettings);
        }

        /// <summary>
        /// Edit the MotifAnomalieTransporteur with the given {id}
        /// </summary>
        /// <param name="model">The MotifAnomalieTransporteurViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] MotifAnomalieTransporteurViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            MotifAnomalieTransporteur motifAnomalieTransporteur;
            //Retrieve or create the entity to edit
            if (model.RefMotifAnomalieTransporteur > 0)
            {
                motifAnomalieTransporteur = DbContext.MotifAnomalieTransporteurs
                        .Where(q => q.RefMotifAnomalieTransporteur == model.RefMotifAnomalieTransporteur)
                        .FirstOrDefault();
            }
            else
            {
                motifAnomalieTransporteur = new MotifAnomalieTransporteur();
                DbContext.MotifAnomalieTransporteurs.Add(motifAnomalieTransporteur);
            }
            // handle requests asking for non-existing MotifAnomalieTransporteurzes
            if (motifAnomalieTransporteur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            motifAnomalieTransporteur.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            motifAnomalieTransporteur.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = motifAnomalieTransporteur.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated MotifAnomalieTransporteur to the client.
                return new JsonResult(
                    _mapper.Map<MotifAnomalieTransporteur, MotifAnomalieTransporteurViewModel>(motifAnomalieTransporteur),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the MotifAnomalieTransporteur with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the MotifAnomalieTransporteur from the Database
            var motifAnomalieTransporteur = DbContext.MotifAnomalieTransporteurs.Where(i => i.RefMotifAnomalieTransporteur == id)
                .FirstOrDefault();

            // handle requests asking for non-existing MotifAnomalieTransporteurzes
            if (motifAnomalieTransporteur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = motifAnomalieTransporteur.IsDeletable();
            if (del == "")
            {
                DbContext.MotifAnomalieTransporteurs.Remove(motifAnomalieTransporteur);
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
        /// GET: api/MotifAnomalieTransporteur/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Where
            System.Linq.IQueryable<eVaSys.Data.MotifAnomalieTransporteur> req = DbContext.MotifAnomalieTransporteurs;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<MotifAnomalieTransporteur[], MotifAnomalieTransporteurViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
