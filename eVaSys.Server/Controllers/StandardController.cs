/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 26/11/2021
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
    public class StandardController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public StandardController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/standard/{id}
        /// Retrieves the Standard with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Standard</param>
        /// <returns>the Standard with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var standard = DbContext.Standards
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefStandard == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (standard == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Standard, StandardViewModel>(standard),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Standard with the given {id}
        /// </summary>
        /// <param name="model">The StandardViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] StandardViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Standard standard;
            //Retrieve or create the entity to edit
            if (model.RefStandard > 0)
            {
                standard = DbContext.Standards
                        .Where(q => q.RefStandard == model.RefStandard)
                        .FirstOrDefault();
            }
            else
            {
                standard = new Standard();
                DbContext.Standards.Add(standard);
            }
            // handle requests asking for non-existing standardzes
            if (standard == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            standard.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            standard.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = standard.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Standard to the client.
                return new JsonResult(
                    _mapper.Map<Standard, StandardViewModel>(standard),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Standard with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the standard from the Database
            var standard = DbContext.Standards.Where(i => i.RefStandard == id)
                .FirstOrDefault();

            // handle requests asking for non-existing standardzes
            if (standard == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = standard.IsDeletable();
            if (del == "")
            {
                DbContext.Standards.Remove(standard);
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
            System.Linq.IQueryable<eVaSys.Data.Standard> req = DbContext.Standards;
            if (refEntite != null)
            {
                req = req.Where(el => el.EntiteStandards.Any(rel => rel.RefEntite == refEntite));
            }
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Standard[], StandardViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
