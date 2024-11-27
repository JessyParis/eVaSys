/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 17/10/2022
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
    public class ProduitGroupeReportingTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ProduitGroupeReportingTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/produitGroupeReportingType/{id}
        /// Retrieves the ProduitGroupeReportingType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing ProduitGroupeReportingType</param>
        /// <returns>The ProduitGroupeReportingType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var produitGroupeReportingType = DbContext.ProduitGroupeReportingTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefProduitGroupeReportingType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (produitGroupeReportingType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<ProduitGroupeReportingType, ProduitGroupeReportingTypeViewModel>(produitGroupeReportingType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the ProduitGroupeReportingType with the given {id}
        /// </summary>
        /// <param name="model">The ProduitGroupeReportingTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]ProduitGroupeReportingTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            ProduitGroupeReportingType produitGroupeReportingType;
            //Retrieve or create the entity to edit
            if (model.RefProduitGroupeReportingType > 0)
            {
                produitGroupeReportingType = DbContext.ProduitGroupeReportingTypes
                        .Where(q => q.RefProduitGroupeReportingType == model.RefProduitGroupeReportingType)
                        .FirstOrDefault();
            }
            else
            {
                produitGroupeReportingType = new ProduitGroupeReportingType();
                DbContext.ProduitGroupeReportingTypes.Add(produitGroupeReportingType);
            }
            // handle requests asking for non-existing produitGroupeReportingTypezes
            if (produitGroupeReportingType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            produitGroupeReportingType.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            produitGroupeReportingType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = produitGroupeReportingType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated ProduitGroupeReportingType to the client.
                return new JsonResult(
                    _mapper.Map<ProduitGroupeReportingType, ProduitGroupeReportingTypeViewModel>(produitGroupeReportingType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the ProduitGroupeReportingType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the produitGroupeReportingType from the Database
            var produitGroupeReportingType = DbContext.ProduitGroupeReportingTypes.Where(i => i.RefProduitGroupeReportingType == id).FirstOrDefault();
            // handle requests asking for non-existing produitGroupeReportingTypezes
            if (produitGroupeReportingType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = produitGroupeReportingType.IsDeletable();
            if (del == "")
            {
                DbContext.ProduitGroupeReportingTypes.Remove(produitGroupeReportingType);
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
        /// GET: api/produitGroupeReportingType/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.ProduitGroupeReportingType> req = DbContext.ProduitGroupeReportingTypes;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<ProduitGroupeReportingType[], ProduitGroupeReportingTypeViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
