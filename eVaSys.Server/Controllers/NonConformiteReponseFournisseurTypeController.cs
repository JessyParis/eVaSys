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
    public class NonConformiteReponseFournisseurTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public NonConformiteReponseFournisseurTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/nonconformitereponsefournisseurtype/{id}
        /// Retrieves the NonConformiteReponseFournisseurType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NonConformiteReponseFournisseurType</param>
        /// <returns>The NonConformiteReponseFournisseurType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var nonConformiteReponseFournisseurType = DbContext.NonConformiteReponseFournisseurTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefNonConformiteReponseFournisseurType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (nonConformiteReponseFournisseurType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<NonConformiteReponseFournisseurType, NonConformiteReponseFournisseurTypeViewModel>(nonConformiteReponseFournisseurType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the NonConformiteReponseFournisseurType with the given {id}
        /// </summary>
        /// <param name="model">The NonConformiteReponseFournisseurTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]NonConformiteReponseFournisseurTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            NonConformiteReponseFournisseurType nonConformiteReponseFournisseurType;
            //Retrieve or create the entity to edit
            if (model.RefNonConformiteReponseFournisseurType > 0)
            {
                nonConformiteReponseFournisseurType = DbContext.NonConformiteReponseFournisseurTypes
                        .Where(q => q.RefNonConformiteReponseFournisseurType == model.RefNonConformiteReponseFournisseurType)
                        .FirstOrDefault();
            }
            else
            {
                nonConformiteReponseFournisseurType = new NonConformiteReponseFournisseurType();
                DbContext.NonConformiteReponseFournisseurTypes.Add(nonConformiteReponseFournisseurType);
            }
            // handle requests asking for non-existing nonConformiteReponseFournisseurTypezes
            if (nonConformiteReponseFournisseurType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            nonConformiteReponseFournisseurType.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(model.LibelleFRFR);
            nonConformiteReponseFournisseurType.LibelleENGB = Utils.Utils.SetEmptyStringToNull(model.LibelleENGB);
            nonConformiteReponseFournisseurType.Actif = model.Actif;
            //Register session user
            nonConformiteReponseFournisseurType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = nonConformiteReponseFournisseurType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated NonConformiteReponseFournisseurType to the client.
                return new JsonResult(
                    _mapper.Map<NonConformiteReponseFournisseurType, NonConformiteReponseFournisseurTypeViewModel>(nonConformiteReponseFournisseurType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the NonConformiteReponseFournisseurType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the nonConformiteReponseFournisseurType from the Database
            var nonConformiteReponseFournisseurType = DbContext.NonConformiteReponseFournisseurTypes.Where(i => i.RefNonConformiteReponseFournisseurType == id).FirstOrDefault();
            // handle requests asking for non-existing nonConformiteReponseFournisseurTypezes
            if (nonConformiteReponseFournisseurType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = nonConformiteReponseFournisseurType.IsDeletable();
            if (del == "")
            {
                DbContext.NonConformiteReponseFournisseurTypes.Remove(nonConformiteReponseFournisseurType);
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
        /// GET: api/nonconformitereponsefournisseurtype/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true" ? true : false);
            string refNonConformiteReponseFournisseurType = Request.Headers["refNonConformiteReponseFournisseurType"].ToString();
            int r = 0;
            //Query
            System.Linq.IQueryable<eVaSys.Data.NonConformiteReponseFournisseurType> req = DbContext.NonConformiteReponseFournisseurTypes;
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (int.TryParse(refNonConformiteReponseFournisseurType, out r)) { req = req.Where(el => el.Actif == true || el.RefNonConformiteReponseFournisseurType == r); }
                else { req = req.Where(el => el.Actif == true); }
            }
            //Get data
            NonConformiteReponseFournisseurTypeViewModel[] all;
            if (CurrentContext.CurrentCulture.Name == "fr-FR")
            {
                all = req.OrderBy(el => el.LibelleFRFR).Select(p => new NonConformiteReponseFournisseurTypeViewModel()
                {
                    RefNonConformiteReponseFournisseurType = p.RefNonConformiteReponseFournisseurType,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleFRFR,
                    Actif = p.Actif
                }).ToArray();
            }
            else
            {
                all = req.OrderBy(el => el.LibelleENGB).Select(p => new NonConformiteReponseFournisseurTypeViewModel()
                {
                    RefNonConformiteReponseFournisseurType = p.RefNonConformiteReponseFournisseurType,
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
