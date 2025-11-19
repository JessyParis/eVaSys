/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/01/2024
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
    public class EcoOrganismeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public EcoOrganismeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/ecoorganisme/{id}
        /// Retrieves the EcoOrganisme with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing EcoOrganisme</param>
        /// <returns>The EcoOrganisme with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var ecoorganisme = DbContext.EcoOrganismes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefEcoOrganisme == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (ecoorganisme == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<EcoOrganisme, EcoOrganismeViewModel>(ecoorganisme),
                JsonSettings);
        }

        /// <summary>
        /// Edit the EcoOrganisme with the given {id}
        /// </summary>
        /// <param name="model">The EcoOrganismeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] EcoOrganismeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            EcoOrganisme ecoorganisme;
            //Retrieve or create the entity to edit
            if (model.RefEcoOrganisme > 0)
            {
                ecoorganisme = DbContext.EcoOrganismes
                        .Where(q => q.RefEcoOrganisme == model.RefEcoOrganisme)
                        .FirstOrDefault();
            }
            else
            {
                ecoorganisme = new EcoOrganisme();
                DbContext.EcoOrganismes.Add(ecoorganisme);
            }
            // handle requests asking for non-existing ecoorganismezes
            if (ecoorganisme == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            ecoorganisme.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            ecoorganisme.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = ecoorganisme.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated EcoOrganisme to the client.
                return new JsonResult(
                    _mapper.Map<EcoOrganisme, EcoOrganismeViewModel>(ecoorganisme),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the EcoOrganisme with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the ecoorganisme from the Database
            var ecoorganisme = DbContext.EcoOrganismes.Where(i => i.RefEcoOrganisme == id).FirstOrDefault();
            // handle requests asking for non-existing ecoorganismezes
            if (ecoorganisme == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = ecoorganisme.IsDeletable();
            if (del == "")
            {
                DbContext.EcoOrganismes.Remove(ecoorganisme);
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
        /// GET: api/ecoorganisme/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlist")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.EcoOrganisme> req = DbContext.EcoOrganismes;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<EcoOrganisme[], EcoOrganismeViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
