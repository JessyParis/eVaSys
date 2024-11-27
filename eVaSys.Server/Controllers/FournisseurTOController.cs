/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/11/2021
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
    public class FournisseurTOController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public FournisseurTOController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/fournisseurTO/{id}
        /// Retrieves the FournisseurTO with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing FournisseurTO</param>
        /// <returns>the FournisseurTO with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var fournisseurTO = DbContext.FournisseurTOs
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefFournisseurTO == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (fournisseurTO == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<FournisseurTO, FournisseurTOViewModel>(fournisseurTO),
                JsonSettings);
        }

        /// <summary>
        /// Edit the FournisseurTO with the given {id}
        /// </summary>
        /// <param name="model">The FournisseurTOViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]FournisseurTOViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            FournisseurTO fournisseurTO;
            //Retrieve or create the entiey to edit
            if (model.RefFournisseurTO > 0)
            {
                fournisseurTO = DbContext.FournisseurTOs
                        .Where(q => q.RefFournisseurTO == model.RefFournisseurTO)
                        .FirstOrDefault();
            }
            else
            {
                fournisseurTO = new FournisseurTO();
                DbContext.FournisseurTOs.Add(fournisseurTO);
            }
            // handle requests asking for non-existing fournisseurTOzes
            if (fournisseurTO == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            fournisseurTO.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            fournisseurTO.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = fournisseurTO.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated FournisseurTO to the client.
                return new JsonResult(
                    _mapper.Map<FournisseurTO, FournisseurTOViewModel>(fournisseurTO),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the FournisseurTO with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the fournisseurTO from the Database
            var fournisseurTO = DbContext.FournisseurTOs.Where(i => i.RefFournisseurTO == id)
                .FirstOrDefault();

            // handle requests asking for non-existing fournisseurTOzes
            if (fournisseurTO == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = fournisseurTO.IsDeletable();
            if (del == "")
            {
                // remove the fournisseurTO from the DbContext.
                DbContext.FournisseurTOs.Remove(fournisseurTO);
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
        /// GET: api/fournisseurto/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.FournisseurTO> req = DbContext.FournisseurTOs;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<FournisseurTO[], FournisseurTOViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
