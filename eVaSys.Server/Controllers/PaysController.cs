/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/12/2018
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
    public class PaysController : BaseApiController
    {
        private readonly IMapper _mapper;

        #region Constructor

        public PaysController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }

        #endregion Constructor

        #region RESTful Conventions

        /// <summary>
        /// GET: evapi/pays/{id}
        /// Retrieves the Pays with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Pays</param>
        /// <returns>The Pays with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var pays = DbContext.Payss
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefPays == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (pays == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Pays, PaysViewModel>(pays),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Pays with the given {id}
        /// </summary>
        /// <param name="model">The PaysViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]PaysViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Pays pays;
            //Retrieve or create the entity to edit
            if (model.RefPays > 0)
            {
                pays = DbContext.Payss
                        .Where(q => q.RefPays == model.RefPays)
                        .FirstOrDefault();
            }
            else
            {
                pays = new Pays();
                DbContext.Payss.Add(pays);
            }
            // handle requests asking for non-existing pays
            if (pays == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            pays.Actif = model.Actif ?? false;
            pays.Libelle = model.Libelle;
            pays.LibelleCourt = model.LibelleCourt;
            pays.RefRegionReporting = model.RefRegionReporting;

            //Register session user
            pays.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = pays.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Pays to the client.
                return new JsonResult(
                    _mapper.Map<Pays, PaysViewModel>(pays),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Pays with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the pays from the Database
            var pays = DbContext.Payss.Where(i => i.RefPays == id)
                .FirstOrDefault();

            // handle requests asking for non-existing payszes
            if (pays == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = pays.IsDeletable();
            if (del == "")
            {
                DbContext.Payss.Remove(pays);
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

        #endregion RESTful Conventions

        #region Attribute-based Routing

        /// <summary>
        /// GET: api/pays/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true" ? true : false);
            int? refPays = (string.IsNullOrEmpty(Request.Headers["refPays"]) ? null : (int?)System.Convert.ToInt32(Request.Headers["refPays"]));
            //Where
            System.Linq.IQueryable<eVaSys.Data.Pays> req = DbContext.Payss;
            //Filters
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (refPays != null) { req = req.Where(el => el.Actif == true || el.RefPays == refPays); }
                else { req = req.Where(el => el.Actif == true); }
            }
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Pays[], PaysViewModel[]>(all),
                JsonSettings);
        }

        #endregion Attribute-based Routing
    }
}