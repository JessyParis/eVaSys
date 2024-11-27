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
    public class FonctionController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public FonctionController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/fonction/{id}
        /// Retrieves the Fonction with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Fonction</param>
        /// <returns>The Fonction with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var fonction = DbContext.Fonctions
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefFonction == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (fonction == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Fonction, FonctionViewModel>(fonction),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Fonction with the given {id}
        /// </summary>
        /// <param name="model">The FonctionViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]FonctionViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            Fonction fonction;
            //Retrieve or create the entity to edit
            if (model.RefFonction > 0)
            {
                fonction = DbContext.Fonctions
                        .Where(q => q.RefFonction == model.RefFonction)
                        .FirstOrDefault();
            }
            else
            {
                fonction = new Fonction();
                DbContext.Fonctions.Add(fonction);
            }
            // handle requests asking for non-existing fonctions
            if (fonction == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            fonction.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            fonction.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = fonction.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Fonction to the client.
                return new JsonResult(
                    _mapper.Map<Fonction, FonctionViewModel>(fonction),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Fonction with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Fonction</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the Fonction from the Database
            var fonction = DbContext.Fonctions.Where(i => i.RefFonction == id).FirstOrDefault();
            // handle requests asking for non-existing Fonctions
            if (fonction == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = fonction.IsDeletable();
            if (del == "")
            {
                DbContext.Fonctions.Remove(fonction);
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
        /// GET: api/fonction/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.Fonction> req = DbContext.Fonctions;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Fonction[], FonctionViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
