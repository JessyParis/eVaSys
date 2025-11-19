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
    public class SAGECodeTransportController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public SAGECodeTransportController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/sagecodetransport/{id}
        /// Retrieves the SAGECodeTransport with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing SAGECodeTransport</param>
        /// <returns>The SAGECodeTransport with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var code = DbContext.SAGECodeTransports
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefSAGECodeTransport == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (code == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<SAGECodeTransport, SAGECodeTransportViewModel>(code),
                JsonSettings);
        }

        /// <summary>
        /// Edit the SAGECodeTransport with the given {id}
        /// </summary>
        /// <param name="model">The SAGECodeTransportViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] SAGECodeTransportViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            SAGECodeTransport code;
            //Retrieve or create the entity to edit
            if (model.RefSAGECodeTransport > 0)
            {
                code = DbContext.SAGECodeTransports
                        .Where(q => q.RefSAGECodeTransport == model.RefSAGECodeTransport)
                        .FirstOrDefault();
            }
            else
            {
                code = new SAGECodeTransport();
                DbContext.SAGECodeTransports.Add(code);
            }
            // handle requests asking for non-existing SAGECodeTransport
            if (code == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            code.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            code.Code = Utils.Utils.SetEmptyStringToNull(model.Code);
            //Register session user
            code.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = code.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Fonction to the client.
                return new JsonResult(
                    _mapper.Map<SAGECodeTransport, SAGECodeTransportViewModel>(code),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the SAGECodeTransport with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing SAGECodeTransport</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the SAGECodeTransport from the Database
            var code = DbContext.SAGECodeTransports.Where(i => i.RefSAGECodeTransport == id).FirstOrDefault();
            // handle requests asking for non-existing Fonctions
            if (code == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = code.IsDeletable();
            if (del == "")
            {
                DbContext.SAGECodeTransports.Remove(code);
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
        /// GET: api/sagecodetransport/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.SAGECodeTransport> req = DbContext.SAGECodeTransports;
            //Get data
            var all = req.OrderBy(el => System.Convert.ToInt32(el.Code.Replace("TP", ""))).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<SAGECodeTransport[], SAGECodeTransportViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
