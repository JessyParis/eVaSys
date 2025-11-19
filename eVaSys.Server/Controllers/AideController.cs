/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/10/2024
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class AideController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public AideController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/aide/{id}
        /// Retrieves the Aide with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Aide</param>
        /// <returns>The Aide with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var aide = DbContext.Aides
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefAide == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (aide == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Aide, AideViewModel>(aide),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Aide with the given {id}
        /// </summary>
        /// <param name="model">The AideViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] AideViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            Aide aide;
            //Retrieve or create the entity to edit
            if (model.RefAide > 0)
            {
                aide = DbContext.Aides
                        .Where(q => q.RefAide == model.RefAide)
                        .FirstOrDefault();
            }
            else
            {
                aide = new Aide();
                DbContext.Aides.Add(aide);
            }
            // handle requests asking for non-existing aides
            if (aide == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            aide.Composant = Utils.Utils.SetEmptyStringToNull(model.Composant);
            aide.ValeurHTML = Utils.Utils.SetEmptyStringToNull(model.ValeurHTML);
            aide.LibelleFRFR = DbContext.Ressources.Where(e => e.RefRessource == model.RefRessource).FirstOrDefault()?.LibelleFRFR;
            aide.LibelleENGB = DbContext.Ressources.Where(e => e.RefRessource == model.RefRessource).FirstOrDefault()?.LibelleENGB;
            //Register session user
            aide.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = aide.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Aide to the client.
                return new JsonResult(
                    _mapper.Map<Aide, AideViewModel>(aide),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Aide with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Aide</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the Aide from the Database
            var aide = DbContext.Aides.Where(i => i.RefAide == id).FirstOrDefault();
            // handle requests asking for non-existing Aides
            if (aide == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = aide.IsDeletable();
            if (del == "")
            {
                DbContext.Aides.Remove(aide);
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
        /// GET: evapi/aide/getaidebycomposant
        /// Retrieves the Aide with the given component name (Composant)
        /// </summary>
        /// <param name="composant">The name of the component of an existing Aide</param>
        /// <returns>The Aide with the given component name (Composant)</returns>
        [HttpGet("getaidebycomposant")]
        public IActionResult getaidebycomposant()
        {
            string composant = Request.Headers["composant"].ToString();
            var aide = DbContext.Aides
                    .Where(i => i.Composant == composant).FirstOrDefault();
            // handle requests asking for non-existing object
            if (aide == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Aide, AideViewModel>(aide),
                JsonSettings);
        }
        #endregion
    }
}
