/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/09/2019
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
    public class ProcessController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ProcessController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/process/{id}
        /// Retrieves the Process with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Process</param>
        /// <returns>the Process with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var process = DbContext.Processs
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefProcess == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (process == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Process, ProcessViewModel>(process),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Process with the given {id}
        /// </summary>
        /// <param name="model">The ProcessViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]ProcessViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Process process;
            //Retrieve or create the entity to edit
            if (model.RefProcess > 0)
            {
                process = DbContext.Processs
                        .Where(q => q.RefProcess == model.RefProcess)
                        .FirstOrDefault();
            }
            else
            {
                process = new Process();
                DbContext.Processs.Add(process);
            }
            // handle requests asking for non-existing processzes
            if (process == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            process.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            process.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = process.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Process to the client.
                return new JsonResult(
                    _mapper.Map<Process, ProcessViewModel>(process),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Process with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the process from the Database
            var process = DbContext.Processs.Where(i => i.RefProcess == id)
                .FirstOrDefault();

            // handle requests asking for non-existing processzes
            if (process == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = process.IsDeletable();
            if (del == "")
            {
                DbContext.Processs.Remove(process);
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
        /// GET: api/items/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            int? refEntite = (string.IsNullOrEmpty(Request.Headers["refEntite"]) ? null : (int?)System.Convert.ToInt32(Request.Headers["refEntite"]));
            System.Linq.IQueryable<eVaSys.Data.Process> req = DbContext.Processs;
            if (refEntite != null)
            {
                req = req.Where(el => el.EntiteProcesss.Any(rel => rel.RefEntite == refEntite));
            }
            //Get data
            var all = req.Distinct().OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Process[], ProcessViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
