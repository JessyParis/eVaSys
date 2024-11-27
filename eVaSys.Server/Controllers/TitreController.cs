/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/10/2021
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
    public class TitreController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public TitreController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/titre/{id}
        /// Retrieves the Titre with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Titre</param>
        /// <returns>The Titre with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var titre = DbContext.Titres
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefTitre == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (titre == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Titre, TitreViewModel>(titre),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Titre with the given {id}
        /// </summary>
        /// <param name="model">The TitreViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]TitreViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            Titre titre;
            //Retrieve or create the entity to edit
            if (model.RefTitre > 0)
            {
                titre = DbContext.Titres
                        .Where(q => q.RefTitre == model.RefTitre)
                        .FirstOrDefault();
            }
            else
            {
                titre = new Titre();
                DbContext.Titres.Add(titre);
            }
            // handle requests asking for non-existing titrezes
            if (titre == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            titre.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            titre.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = titre.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Titre to the client.
                return new JsonResult(
                    _mapper.Map<Titre, TitreViewModel>(titre),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Titre with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the titre from the Database
            var titre = DbContext.Titres.Where(i => i.RefTitre == id).FirstOrDefault();
            // handle requests asking for non-existing titrezes
            if (titre == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = titre.IsDeletable();
            if (del == "")
            {
                DbContext.Titres.Remove(titre);
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
        /// GET: api/titre/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.Titre> req = DbContext.Titres;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Titre[], TitreViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
