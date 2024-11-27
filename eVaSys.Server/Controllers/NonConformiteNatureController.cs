/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/06/2019
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
    public class NonConformiteNatureController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public NonConformiteNatureController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/nonconformitenature/{id}
        /// Retrieves the NonConformiteNature with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NonConformiteNature</param>
        /// <returns>The NonConformiteNature with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var nonConformiteNature = DbContext.NonConformiteNatures
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefNonConformiteNature == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (nonConformiteNature == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<NonConformiteNature, NonConformiteNatureViewModel>(nonConformiteNature),
                JsonSettings);
        }

        /// <summary>
        /// Edit the NonConformiteNature with the given {id}
        /// </summary>
        /// <param name="model">The NonConformiteNatureViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] NonConformiteNatureViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            NonConformiteNature nonConformiteNature;
            //Retrieve or create the entity to edit
            if (model.RefNonConformiteNature > 0)
            {
                nonConformiteNature = DbContext.NonConformiteNatures
                        .Where(q => q.RefNonConformiteNature == model.RefNonConformiteNature)
                        .FirstOrDefault();
            }
            else
            {
                nonConformiteNature = new NonConformiteNature();
                DbContext.NonConformiteNatures.Add(nonConformiteNature);
            }
            // handle requests asking for non-existing nonConformiteNaturezes
            if (nonConformiteNature == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            nonConformiteNature.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(model.LibelleFRFR);
            nonConformiteNature.LibelleENGB = Utils.Utils.SetEmptyStringToNull(model.LibelleENGB);
            nonConformiteNature.Actif = model.Actif;
            nonConformiteNature.IncitationQualite = model.IncitationQualite;
            //Register session user
            nonConformiteNature.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = nonConformiteNature.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated NonConformiteNature to the client.
                return new JsonResult(
                    _mapper.Map<NonConformiteNature, NonConformiteNatureViewModel>(nonConformiteNature),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the NonConformiteNature with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the nonConformiteNature from the Database
            var nonConformiteNature = DbContext.NonConformiteNatures.Where(i => i.RefNonConformiteNature == id).FirstOrDefault();
            // handle requests asking for non-existing nonConformiteNaturezes
            if (nonConformiteNature == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = nonConformiteNature.IsDeletable();
            if (del == "")
            {
                DbContext.NonConformiteNatures.Remove(nonConformiteNature);
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
        /// GET: api/nonconformitenature/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true" ? true : false);
            string refNonConformiteNature = Request.Headers["refNonConformiteNature"].ToString();
            int r = 0;
            //Query
            System.Linq.IQueryable<eVaSys.Data.NonConformiteNature> req = DbContext.NonConformiteNatures;
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (int.TryParse(refNonConformiteNature, out r)) { req = req.Where(el => el.Actif == true || el.RefNonConformiteNature == r); }
                else { req = req.Where(el => el.Actif == true); }
            }
            //Get data
            NonConformiteNatureViewModel[] all;
            if (CurrentContext.CurrentCulture.Name == "fr-FR")
            {
                all = req.OrderBy(el => el.LibelleFRFR).Select(p => new NonConformiteNatureViewModel()
                {
                    RefNonConformiteNature = p.RefNonConformiteNature,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleFRFR,
                    Actif = p.Actif
                }).ToArray();
            }
            else
            {
                all = req.OrderBy(el => el.LibelleENGB).Select(p => new NonConformiteNatureViewModel()
                {
                    RefNonConformiteNature = p.RefNonConformiteNature,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleENGB,
                    Actif = p.Actif
                }).ToArray();
            }
            //Return Json
            return new JsonResult(all,
                JsonSettings);
        }
        #endregion
    }
}
