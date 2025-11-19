/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/12/2018
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
    public class MotifCamionIncompletController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public MotifCamionIncompletController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/MotifCamionIncomplet/{id}
        /// Retrieves the MotifCamionIncomplet with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing MotifCamionIncomplet</param>
        /// <returns>The MotifCamionIncomplet with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var motifCamionIncomplet = DbContext.MotifCamionIncomplets
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefMotifCamionIncomplet == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (motifCamionIncomplet == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<MotifCamionIncomplet, MotifCamionIncompletViewModel>(motifCamionIncomplet),
                JsonSettings);
        }

        /// <summary>
        /// Edit the MotifCamionIncomplet with the given {id}
        /// </summary>
        /// <param name="model">The MotifCamionIncompletViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] MotifCamionIncompletViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            MotifCamionIncomplet motifCamionIncomplet;
            //Retrieve or create the entity to edit
            if (model.RefMotifCamionIncomplet > 0)
            {
                motifCamionIncomplet = DbContext.MotifCamionIncomplets
                        .Where(q => q.RefMotifCamionIncomplet == model.RefMotifCamionIncomplet)
                        .FirstOrDefault();
            }
            else
            {
                motifCamionIncomplet = new MotifCamionIncomplet();
                DbContext.MotifCamionIncomplets.Add(motifCamionIncomplet);
            }
            // handle requests asking for non-existing MotifCamionIncompletzes
            if (motifCamionIncomplet == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            motifCamionIncomplet.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            motifCamionIncomplet.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = motifCamionIncomplet.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated MotifCamionIncomplet to the client.
                return new JsonResult(
                    _mapper.Map<MotifCamionIncomplet, MotifCamionIncompletViewModel>(motifCamionIncomplet),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the MotifCamionIncomplet with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the MotifCamionIncomplet from the Database
            var motifCamionIncomplet = DbContext.MotifCamionIncomplets.Where(i => i.RefMotifCamionIncomplet == id)
                .FirstOrDefault();

            // handle requests asking for non-existing MotifCamionIncompletzes
            if (motifCamionIncomplet == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = motifCamionIncomplet.IsDeletable();
            if (del == "")
            {
                DbContext.MotifCamionIncomplets.Remove(motifCamionIncomplet);
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
        /// GET: api/MotifCamionIncomplet/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Where
            System.Linq.IQueryable<eVaSys.Data.MotifCamionIncomplet> req = DbContext.MotifCamionIncomplets;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<MotifCamionIncomplet[], MotifCamionIncompletViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
