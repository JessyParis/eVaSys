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
    public class NonConformiteDemandeClientTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public NonConformiteDemandeClientTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/nonconformitedemandeclienttype/{id}
        /// Retrieves the NonConformiteDemandeClientType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NonConformiteDemandeClientType</param>
        /// <returns>The NonConformiteDemandeClientType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var nonConformiteDemandeClientType = DbContext.NonConformiteDemandeClientTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefNonConformiteDemandeClientType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (nonConformiteDemandeClientType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<NonConformiteDemandeClientType, NonConformiteDemandeClientTypeViewModel>(nonConformiteDemandeClientType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the NonConformiteDemandeClientType with the given {id}
        /// </summary>
        /// <param name="model">The NonConformiteDemandeClientTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]NonConformiteDemandeClientTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            NonConformiteDemandeClientType nonConformiteDemandeClientType;
            //Retrieve or create the entity to edit
            if (model.RefNonConformiteDemandeClientType > 0)
            {
                nonConformiteDemandeClientType = DbContext.NonConformiteDemandeClientTypes
                        .Where(q => q.RefNonConformiteDemandeClientType == model.RefNonConformiteDemandeClientType)
                        .FirstOrDefault();
            }
            else
            {
                nonConformiteDemandeClientType = new NonConformiteDemandeClientType();
                DbContext.NonConformiteDemandeClientTypes.Add(nonConformiteDemandeClientType);
            }
            // handle requests asking for non-existing nonConformiteDemandeClientTypezes
            if (nonConformiteDemandeClientType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            nonConformiteDemandeClientType.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(model.LibelleFRFR);
            nonConformiteDemandeClientType.LibelleENGB = Utils.Utils.SetEmptyStringToNull(model.LibelleENGB);
            nonConformiteDemandeClientType.Actif = model.Actif;
            //Register session user
            nonConformiteDemandeClientType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = nonConformiteDemandeClientType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated NonConformiteDemandeClientType to the client.
                return new JsonResult(
                    _mapper.Map<NonConformiteDemandeClientType, NonConformiteDemandeClientTypeViewModel>(nonConformiteDemandeClientType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the NonConformiteDemandeClientType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the nonConformiteDemandeClientType from the Database
            var nonConformiteDemandeClientType = DbContext.NonConformiteDemandeClientTypes.Where(i => i.RefNonConformiteDemandeClientType == id).FirstOrDefault();
            // handle requests asking for non-existing nonConformiteDemandeClientTypezes
            if (nonConformiteDemandeClientType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = nonConformiteDemandeClientType.IsDeletable();
            if (del == "")
            {
                DbContext.NonConformiteDemandeClientTypes.Remove(nonConformiteDemandeClientType);
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
        /// GET: api/nonConformiteDemandeClientType/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true" ? true : false);
            string refNonConformiteDemandeClientType = Request.Headers["refNonConformiteDemandeClientType"];
            int r = 0;
            int.TryParse(refNonConformiteDemandeClientType, out r);
            //Query
            System.Linq.IQueryable<eVaSys.Data.NonConformiteDemandeClientType> req = DbContext.NonConformiteDemandeClientTypes;
            if (actif)
            {
                if (r == 0) { req = req.Where(el => el.Actif == true); }
                else { req = req.Where(el => el.Actif == true || el.RefNonConformiteDemandeClientType == r); }
            }
            //Get data
            NonConformiteDemandeClientTypeViewModel[] all;
            if (CurrentContext.CurrentCulture.Name == "fr-FR")
            {
                all = req.OrderBy(el => el.LibelleFRFR).Select(p => new NonConformiteDemandeClientTypeViewModel()
                {
                    RefNonConformiteDemandeClientType = p.RefNonConformiteDemandeClientType,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleFRFR,
                    Actif = p.Actif
                }).ToArray();
            }
            else
            {
                all = req.OrderBy(el => el.LibelleENGB).Select(p => new NonConformiteDemandeClientTypeViewModel()
                {
                    RefNonConformiteDemandeClientType = p.RefNonConformiteDemandeClientType,
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
