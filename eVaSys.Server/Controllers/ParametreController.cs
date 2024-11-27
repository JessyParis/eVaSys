/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/01/2024
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class ParametreController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ParametreController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/parametre/{id}
        /// Retrieves the Parametre with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Parametre</param>
        /// <returns>The Parametre with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var parametre = DbContext.Parametres
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefParametre == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (parametre == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Parametre, ParametreViewModel>(parametre),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Parametre with the given {id}
        /// </summary>
        /// <param name="model">The ParametreViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] ParametreViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            Parametre parametre;
            //Retrieve or create the entity to edit
            if (model.RefParametre > 0)
            {
                parametre = DbContext.Parametres
                        .Where(q => q.RefParametre == model.RefParametre)
                        .FirstOrDefault();
            }
            else
            {
                parametre = new Parametre();
                DbContext.Parametres.Add(parametre);
            }
            // handle requests asking for non-existing parametres
            if (parametre == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            parametre.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            parametre.Serveur = model.Serveur;
            parametre.ValeurNumerique = model.ValeurNumerique;
            if (string.IsNullOrWhiteSpace(model.ValeurBinaireBase64)) { parametre.ValeurBinaire = null; }
            parametre.ValeurTexte = Utils.Utils.SetEmptyStringToNull(model.ValeurTexte);
            parametre.ValeurHTML = Utils.Utils.SetEmptyStringToNull(model.ValeurHTML);
            parametre.Cmt = Utils.Utils.SetEmptyStringToNull(model.Cmt);
            //Register session user
            parametre.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = parametre.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Parametre to the client.
                return new JsonResult(
                    _mapper.Map<Parametre, ParametreViewModel>(parametre),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Parametre with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Parametre</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the Parametre from the Database
            var parametre = DbContext.Parametres.Where(i => i.RefParametre == id).FirstOrDefault();
            // handle requests asking for non-existing Parametres
            if (parametre == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = parametre.IsDeletable();
            if (del == "")
            {
                DbContext.Parametres.Remove(parametre);
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
        /// <summary>
        /// GET: api/parametre/existsbinarydata
        /// ROUTING TYPE: attribute-based
        /// Check if a file exists for the Parametre
        /// </summary>
        /// <returns>True/False</returns>
        [HttpGet("existsbinarydata")]
        public IActionResult ExistsBinaryData()
        {
            bool a = false;
            string refParametre = Request.Headers["refParametre"].ToString();
            //Check parameters
            int.TryParse(refParametre, out int r);
            if (r != 0)
            {
                a = (DbContext.Parametres
                    .Where(i => (i.RefParametre == r && i.ValeurBinaire != null))
                    .Count() > 0);
                //Return Json
                return new JsonResult(a, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(617)));
            }
        }
    }
}
