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
    public class EquipementierController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public EquipementierController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/equipementier/{id}
        /// Retrieves the Equipementier with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Equipementier</param>
        /// <returns>the Equipementier with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var equipementier = DbContext.Equipementiers
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefEquipementier == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (equipementier == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Equipementier, EquipementierViewModel>(equipementier),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Equipementier with the given {id}
        /// </summary>
        /// <param name="model">The EquipementierViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]EquipementierViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Equipementier equipementier;
            //Retrieve or create the entiey to edit
            if (model.RefEquipementier > 0)
            {
                equipementier = DbContext.Equipementiers
                        .Where(q => q.RefEquipementier == model.RefEquipementier)
                        .FirstOrDefault();
            }
            else
            {
                equipementier = new Equipementier();
                DbContext.Equipementiers.Add(equipementier);
            }
            // handle requests asking for non-existing equipementierzes
            if (equipementier == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            equipementier.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            equipementier.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = equipementier.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Equipementier to the client.
                return new JsonResult(
                    _mapper.Map<Equipementier, EquipementierViewModel>(equipementier),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Equipementier with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the equipementier from the Database
            var equipementier = DbContext.Equipementiers.Where(i => i.RefEquipementier == id)
                .FirstOrDefault();

            // handle requests asking for non-existing equipementierzes
            if (equipementier == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = equipementier.IsDeletable();
            if (del == "")
            {
                // remove the equipementier from the DbContext.
                DbContext.Equipementiers.Remove(equipementier);
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
        /// GET: api/equipementier/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.Equipementier> req = DbContext.Equipementiers;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Equipementier[], EquipementierViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
