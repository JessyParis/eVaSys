/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 17/12/2020
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
    public class ProduitGroupeReportingController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ProduitGroupeReportingController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/produitgroupereporting/{id}
        /// Retrieves the ProduitGroupeReporting with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing ProduitGroupeReporting</param>
        /// <returns>The ProduitGroupeReporting with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var produitGroupeReporting = DbContext.ProduitGroupeReportings
                    .Include(r => r.ProduitGroupeReportingType)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefProduitGroupeReporting == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (produitGroupeReporting == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<ProduitGroupeReporting, ProduitGroupeReportingViewModel>(produitGroupeReporting),
                JsonSettings);
        }

        /// <summary>
        /// Edit the ProduitGroupeReporting with the given {id}
        /// </summary>
        /// <param name="model">The ProduitGroupeReportingViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] ProduitGroupeReportingViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            ProduitGroupeReporting produitGroupeReporting;
            //Retrieve or create the entity to edit
            if (model.RefProduitGroupeReporting > 0)
            {
                produitGroupeReporting = DbContext.ProduitGroupeReportings
                        .Where(q => q.RefProduitGroupeReporting == model.RefProduitGroupeReporting)
                        .FirstOrDefault();
            }
            else
            {
                produitGroupeReporting = new ProduitGroupeReporting();
                DbContext.ProduitGroupeReportings.Add(produitGroupeReporting);
            }
            // handle requests asking for non-existing produitGroupeReportingzes
            if (produitGroupeReporting == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            produitGroupeReporting.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            produitGroupeReporting.Couleur = Utils.Utils.SetEmptyStringToNull(model.Couleur);
            produitGroupeReporting.RefProduitGroupeReportingType = model.RefProduitGroupeReportingType;
            //Register session user
            produitGroupeReporting.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = produitGroupeReporting.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated ProduitGroupeReporting to the client.
                return new JsonResult(
                    _mapper.Map<ProduitGroupeReporting, ProduitGroupeReportingViewModel>(produitGroupeReporting),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the ProduitGroupeReporting with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the produitGroupeReporting from the Database
            var produitGroupeReporting = DbContext.ProduitGroupeReportings.Where(i => i.RefProduitGroupeReporting == id).FirstOrDefault();
            // handle requests asking for non-existing produitGroupeReportingzes
            if (produitGroupeReporting == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = produitGroupeReporting.IsDeletable();
            if (del == "")
            {
                DbContext.ProduitGroupeReportings.Remove(produitGroupeReporting);
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
        /// GET: api/produitgroupereporting/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.ProduitGroupeReporting> req = DbContext.ProduitGroupeReportings;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<ProduitGroupeReporting[], ProduitGroupeReportingViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
