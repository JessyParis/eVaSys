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

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class RepreneurController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public RepreneurController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/repreneur/{id}
        /// Retrieves the Repreneur with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Repreneur</param>
        /// <returns>The Repreneur with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var repreneur = DbContext.Repreneurs
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefRepreneur == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (repreneur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Repreneur, RepreneurViewModel>(repreneur),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Repreneur with the given {id}
        /// </summary>
        /// <param name="model">The RepreneurViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] RepreneurViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            Repreneur repreneur;
            //Retrieve or create the entity to edit
            if (model.RefRepreneur > 0)
            {
                repreneur = DbContext.Repreneurs
                        .Where(q => q.RefRepreneur == model.RefRepreneur)
                        .FirstOrDefault();
            }
            else
            {
                repreneur = new Repreneur();
                DbContext.Repreneurs.Add(repreneur);
            }
            // handle requests asking for non-existing repreneurzes
            if (repreneur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            repreneur.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            repreneur.RepreneurContrat = model?.RepreneurContrat ?? false;
            //Register session user
            repreneur.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = repreneur.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Repreneur to the client.
                return new JsonResult(
                    _mapper.Map<Repreneur, RepreneurViewModel>(repreneur),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Repreneur with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the repreneur from the Database
            var repreneur = DbContext.Repreneurs.Where(i => i.RefRepreneur == id).FirstOrDefault();
            // handle requests asking for non-existing repreneurzes
            if (repreneur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = repreneur.IsDeletable();
            if (del == "")
            {
                DbContext.Repreneurs.Remove(repreneur);
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
        /// GET: api/repreneur/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.Repreneur> req = DbContext.Repreneurs;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Repreneur[], RepreneurViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
