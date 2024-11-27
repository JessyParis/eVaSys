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
    public class ModeTransportEEController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ModeTransportEEController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/modetransportee/{id}
        /// Retrieves the ModeTransportEE with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing ModeTransportEE</param>
        /// <returns>The ModeTransportEE with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var mode = DbContext.ModeTransportEEs
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefModeTransportEE == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (mode == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<ModeTransportEE, ModeTransportEEViewModel>(mode),
                JsonSettings);
        }

        /// <summary>
        /// Edit the ModeTransportEE with the given {id}
        /// </summary>
        /// <param name="model">The ModeTransportEEViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]ModeTransportEEViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            ModeTransportEE mode;
            //Retrieve or create the entity to edit
            if (model.RefModeTransportEE > 0)
            {
                mode = DbContext.ModeTransportEEs
                        .Where(q => q.RefModeTransportEE == model.RefModeTransportEE)
                        .FirstOrDefault();
            }
            else
            {
                mode = new ModeTransportEE();
                DbContext.ModeTransportEEs.Add(mode);
            }
            // handle requests asking for non-existing fonctions
            if (mode == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            mode.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            mode.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = mode.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated ModeTransportEE to the client.
                return new JsonResult(
                    _mapper.Map<ModeTransportEE, ModeTransportEEViewModel>(mode),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the ModeTransportEE with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing ModeTransportEE</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the ModeTransportEE from the Database
            var mode = DbContext.ModeTransportEEs.Where(i => i.RefModeTransportEE == id).FirstOrDefault();
            // handle requests asking for non-existing Fonctions
            if (mode == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the ModeTransportEE from the DbContext if applicable
            string del = mode.IsDeletable();
            if (del == "")
            {
                DbContext.ModeTransportEEs.Remove(mode);
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
        /// GET: api/modetransportee/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.ModeTransportEE> req = DbContext.ModeTransportEEs;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<ModeTransportEE[], ModeTransportEEViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
