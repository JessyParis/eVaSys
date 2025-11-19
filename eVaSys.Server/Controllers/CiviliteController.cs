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
    public class CiviliteController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public CiviliteController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/civilite/{id}
        /// Retrieves the Civilite with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Civilite</param>
        /// <returns>The Civilite with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var civilite = DbContext.Civilites
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefCivilite == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (civilite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Civilite, CiviliteViewModel>(civilite),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Civilite with the given {id}
        /// </summary>
        /// <param name="model">The CiviliteViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] CiviliteViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            Civilite civilite;
            //Retrieve or create the entity to edit
            if (model.RefCivilite > 0)
            {
                civilite = DbContext.Civilites
                        .Where(q => q.RefCivilite == model.RefCivilite)
                        .FirstOrDefault();
            }
            else
            {
                civilite = new Civilite();
                DbContext.Civilites.Add(civilite);
            }
            // handle requests asking for non-existing civilitezes
            if (civilite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            civilite.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            civilite.RefPays = (int)model.Pays?.RefPays;
            //Register session user
            civilite.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = civilite.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Civilite to the client.
                return new JsonResult(
                    _mapper.Map<Civilite, CiviliteViewModel>(civilite),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Civilite with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the civilite from the Database
            var civilite = DbContext.Civilites.Where(i => i.RefCivilite == id).FirstOrDefault();
            // handle requests asking for non-existing civilitezes
            if (civilite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = civilite.IsDeletable();
            if (del == "")
            {
                DbContext.Civilites.Remove(civilite);
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
        /// GET: api/civilite/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.Civilite> req = DbContext.Civilites;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Civilite[], CiviliteViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
