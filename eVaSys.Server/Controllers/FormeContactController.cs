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

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class FormeContactController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public FormeContactController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/formecontact/{id}
        /// Retrieves the FormeContact with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing FormeContact</param>
        /// <returns>The FormeContact with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var forme = DbContext.FormeContacts
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefFormeContact == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (forme == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<FormeContact, FormeContactViewModel>(forme),
                JsonSettings);
        }

        /// <summary>
        /// Edit the FormeContact with the given {id}
        /// </summary>
        /// <param name="model">The FormeContactViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] FormeContactViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            FormeContact forme;
            //Retrieve or create the entity to edit
            if (model.RefFormeContact > 0)
            {
                forme = DbContext.FormeContacts
                        .Where(q => q.RefFormeContact == model.RefFormeContact)
                        .FirstOrDefault();
            }
            else
            {
                forme = new FormeContact();
                DbContext.FormeContacts.Add(forme);
            }
            // handle requests asking for non-existing fonctions
            if (forme == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            forme.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            forme.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = forme.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated FormeContact to the client.
                return new JsonResult(
                    _mapper.Map<FormeContact, FormeContactViewModel>(forme),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the FormeContact with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing FormeContact</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the FormeContact from the Database
            var forme = DbContext.FormeContacts.Where(i => i.RefFormeContact == id).FirstOrDefault();
            // handle requests asking for non-existing Fonctions
            if (forme == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the FormeContact from the DbContext if applicable
            string del = forme.IsDeletable();
            if (del == "")
            {
                DbContext.FormeContacts.Remove(forme);
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
        /// GET: api/formecontact/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.FormeContact> req = DbContext.FormeContacts;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<FormeContact[], FormeContactViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
