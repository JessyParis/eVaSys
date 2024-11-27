/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/12/2019
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
    public class ApplicationProduitOrigineController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ApplicationProduitOrigineController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/applicationproduitorigine/{id}
        /// Retrieves the ApplicationProduitOrigine with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing ApplicationProduitOrigine</param>
        /// <returns>The ApplicationProduitOrigine with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var applicationProduitOrigine = DbContext.ApplicationProduitOrigines
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefApplicationProduitOrigine == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (applicationProduitOrigine == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<ApplicationProduitOrigine, ApplicationProduitOrigineViewModel>(applicationProduitOrigine),
                JsonSettings);
        }

        /// <summary>
        /// Edit the ApplicationProduitOrigine with the given {id}
        /// </summary>
        /// <param name="model">The ApplicationProduitOrigineViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]ApplicationProduitOrigineViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            ApplicationProduitOrigine applicationProduitOrigine;
            //Retrieve or create the entity to edit
            if (model.RefApplicationProduitOrigine > 0)
            {
                applicationProduitOrigine = DbContext.ApplicationProduitOrigines
                        .Where(q => q.RefApplicationProduitOrigine == model.RefApplicationProduitOrigine)
                        .FirstOrDefault();
            }
            else
            {
                applicationProduitOrigine = new ApplicationProduitOrigine();
                DbContext.ApplicationProduitOrigines.Add(applicationProduitOrigine);
            }
            // handle requests asking for non-existing applicationProduitOriginezes
            if (applicationProduitOrigine == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            applicationProduitOrigine.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            applicationProduitOrigine.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = applicationProduitOrigine.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated ApplicationProduitOrigine to the client.
                return new JsonResult(
                    _mapper.Map<ApplicationProduitOrigine, ApplicationProduitOrigineViewModel>(applicationProduitOrigine),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the ApplicationProduitOrigine with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the applicationProduitOrigine from the Database
            var applicationProduitOrigine = DbContext.ApplicationProduitOrigines.Where(i => i.RefApplicationProduitOrigine == id).FirstOrDefault();
            // handle requests asking for non-existing applicationProduitOriginezes
            if (applicationProduitOrigine == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = applicationProduitOrigine.IsDeletable();
            if (del == "")
            {
                DbContext.ApplicationProduitOrigines.Remove(applicationProduitOrigine);
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
        /// GET: api/applicationProduitOrigine/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.ApplicationProduitOrigine> req = DbContext.ApplicationProduitOrigines;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<ApplicationProduitOrigine[], ApplicationProduitOrigineViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
