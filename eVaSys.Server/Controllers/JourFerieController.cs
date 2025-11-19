/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 29/12/2021
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
    public class JourFerieController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public JourFerieController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/jourferie/{id}
        /// Retrieves the JourFerie with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing JourFerie</param>
        /// <returns>The JourFerie with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var jf = DbContext.JourFeries
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefJourFerie == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (jf == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<JourFerie, JourFerieViewModel>(jf),
                JsonSettings);
        }

        /// <summary>
        /// Edit the JourFerie with the given {id}
        /// </summary>
        /// <param name="model">The JourFerie containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] JourFerieViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            JourFerie jf;
            //Retrieve or create the entity to edit
            if (model.RefJourFerie > 0)
            {
                jf = DbContext.JourFeries
                        .Where(q => q.RefJourFerie == model.RefJourFerie)
                        .FirstOrDefault();
            }
            else
            {
                jf = new JourFerie();
                DbContext.JourFeries.Add(jf);
            }
            // handle requests asking for non-existing JourFerie
            if (jf == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            jf.D = model.D;
            //Register session user
            jf.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = jf.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated JourFerie to the client.
                return new JsonResult(
                    _mapper.Map<JourFerie, JourFerieViewModel>(jf),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the JourFerie with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing JourFerie</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the JourFerie from the Database
            var jf = DbContext.JourFeries.Where(i => i.RefJourFerie == id).FirstOrDefault();
            // handle requests asking for non-existing JourFerie
            if (jf == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the JourFerie from the DbContext if applicable
            string del = jf.IsDeletable();
            if (del == "")
            {
                DbContext.JourFeries.Remove(jf);
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
        /// GET: api/jourferie/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.JourFerie> req = DbContext.JourFeries;
            //Get data
            var all = req.OrderBy(el => el.D).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<JourFerie[], JourFerieViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
