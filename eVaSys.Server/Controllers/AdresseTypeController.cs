/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/06/2018
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
    public class AdresseTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public AdresseTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/adressetype/{id}
        /// Retrieves the AdresseType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing AdresseType</param>
        /// <returns>the AdresseType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var adresseType = DbContext.AdresseTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefAdresseType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (adresseType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<AdresseType, AdresseTypeViewModel>(adresseType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the AdresseType with the given {id}
        /// </summary>
        /// <param name="model">The AdresseTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] AdresseTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            AdresseType adresseType;
            //Retrieve or create the entiey to edit
            if (model.RefAdresseType > 0)
            {
                adresseType = DbContext.AdresseTypes
                        .Where(q => q.RefAdresseType == model.RefAdresseType)
                        .FirstOrDefault();
            }
            else
            {
                adresseType = new AdresseType();
                DbContext.AdresseTypes.Add(adresseType);
            }
            // handle requests asking for non-existing adresseTypezes
            if (adresseType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            adresseType.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            adresseType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = adresseType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated AdresseType to the client.
                return new JsonResult(
                    _mapper.Map<AdresseType, AdresseTypeViewModel>(adresseType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the AdresseType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the adresseType from the Database
            var adresseType = DbContext.AdresseTypes.Where(i => i.RefAdresseType == id)
                .FirstOrDefault();

            // handle requests asking for non-existing adresseTypezes
            if (adresseType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = adresseType.IsDeletable();
            if (del == "")
            {
                // remove the adresseType from the DbContext.
                DbContext.AdresseTypes.Remove(adresseType);
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
        /// GET: api/adressetype/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.AdresseType> req = DbContext.AdresseTypes;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<AdresseType[], AdresseTypeViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
