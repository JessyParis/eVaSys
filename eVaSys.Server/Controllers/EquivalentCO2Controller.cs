/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/10/2023
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
    public class EquivalentCO2Controller : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public EquivalentCO2Controller(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/equivalentco2/{id}
        /// Retrieves the EquivalentCO2 with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing EquivalentCO2</param>
        /// <returns>The EquivalentCO2 with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var equivalentCO2 = DbContext.EquivalentCO2s
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefEquivalentCO2 == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (equivalentCO2 == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<EquivalentCO2, EquivalentCO2ViewModel>(equivalentCO2),
                JsonSettings);
        }

        /// <summary>
        /// Edit the EquivalentCO2 with the given {id}
        /// </summary>
        /// <param name="model">The EquivalentCO2ViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] EquivalentCO2ViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            EquivalentCO2 equivalentCO2;
            //Retrieve or create the entity to edit
            if (model.RefEquivalentCO2 > 0)
            {
                equivalentCO2 = DbContext.EquivalentCO2s
                        .Where(q => q.RefEquivalentCO2 == model.RefEquivalentCO2)
                        .FirstOrDefault();
            }
            else
            {
                equivalentCO2 = new EquivalentCO2();
                DbContext.EquivalentCO2s.Add(equivalentCO2);
            }
            // handle requests asking for non-existing equivalentCO2zes
            if (equivalentCO2 == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            equivalentCO2.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            equivalentCO2.Ordre = model.Ordre;
            equivalentCO2.Ratio = model.Ratio;
            equivalentCO2.Actif = model.Actif;
            //Register session user
            equivalentCO2.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = equivalentCO2.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated EquivalentCO2 to the client.
                return new JsonResult(
                    _mapper.Map<EquivalentCO2, EquivalentCO2ViewModel>(equivalentCO2),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the EquivalentCO2 with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the equivalentCO2 from the Database
            var equivalentCO2 = DbContext.EquivalentCO2s.Where(i => i.RefEquivalentCO2 == id).FirstOrDefault();
            // handle requests asking for non-existing equivalentCO2zes
            if (equivalentCO2 == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext
            DbContext.EquivalentCO2s.Remove(equivalentCO2);
            // persist the changes into the Database.
            DbContext.SaveChanges();
            // return an HTTP Status 200 (OK).
            return Ok();
        }
        #endregion

        #region Attribute-based Routing

        /// <summary>
        /// GET: evapi/equivalentco2/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlist")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers.ContainsKey("actif") ? (Request.Headers["actif"] == "true" ? true : false) : false);
            //Get data
            System.Linq.IQueryable<eVaSys.Data.EquivalentCO2> req = DbContext.EquivalentCO2s;
            if (actif)
            {
                req = req.Where(el => el.Actif == true && el.Actif == true); 
            }
            var all = req.OrderBy(el => el.Ordre).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<EquivalentCO2[], EquivalentCO2ViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
