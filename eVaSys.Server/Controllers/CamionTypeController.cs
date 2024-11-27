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
    public class CamionTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public CamionTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/camionType/{id}
        /// Retrieves the CamionType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing CamionType</param>
        /// <returns>The CamionType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var camionType = DbContext.CamionTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefCamionType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (camionType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<CamionType, CamionTypeViewModel>(camionType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the CamionType with the given {id}
        /// </summary>
        /// <param name="model">The CamionTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]CamionTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            CamionType camionType;
            //Retrieve or create the entity to edit
            if (model.RefCamionType > 0)
            {
                camionType = DbContext.CamionTypes
                        .Where(q => q.RefCamionType == model.RefCamionType)
                        .FirstOrDefault();
            }
            else
            {
                camionType = new CamionType();
                DbContext.CamionTypes.Add(camionType);
            }
            // handle requests asking for non-existing camionTypezes
            if (camionType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            camionType.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            camionType.RefModeTransportEE = model.ModeTransportEE?.RefModeTransportEE;
            //Register session user
            camionType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = camionType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated CamionType to the client.
                return new JsonResult(
                    _mapper.Map<CamionType, CamionTypeViewModel>(camionType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the CamionType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the camionType from the Database
            var camionType = DbContext.CamionTypes.Where(i => i.RefCamionType == id).FirstOrDefault();
            // handle requests asking for non-existing camionTypezes
            if (camionType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = camionType.IsDeletable();
            if (del == "")
            {
                DbContext.CamionTypes.Remove(camionType);
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
        /// GET: api/camionType/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.CamionType> req = DbContext.CamionTypes;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<CamionType[], CamionTypeViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
