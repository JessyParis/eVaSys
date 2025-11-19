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

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class NonConformiteReponseClientTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public NonConformiteReponseClientTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/nonconformitereponseclienttype/{id}
        /// Retrieves the NonConformiteReponseClientType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NonConformiteReponseClientType</param>
        /// <returns>The NonConformiteReponseClientType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var nonConformiteReponseClientType = DbContext.NonConformiteReponseClientTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefNonConformiteReponseClientType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (nonConformiteReponseClientType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<NonConformiteReponseClientType, NonConformiteReponseClientTypeViewModel>(nonConformiteReponseClientType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the NonConformiteReponseClientType with the given {id}
        /// </summary>
        /// <param name="model">The NonConformiteReponseClientTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] NonConformiteReponseClientTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            NonConformiteReponseClientType nonConformiteReponseClientType;
            //Retrieve or create the entity to edit
            if (model.RefNonConformiteReponseClientType > 0)
            {
                nonConformiteReponseClientType = DbContext.NonConformiteReponseClientTypes
                        .Where(q => q.RefNonConformiteReponseClientType == model.RefNonConformiteReponseClientType)
                        .FirstOrDefault();
            }
            else
            {
                nonConformiteReponseClientType = new NonConformiteReponseClientType();
                DbContext.NonConformiteReponseClientTypes.Add(nonConformiteReponseClientType);
            }
            // handle requests asking for non-existing nonConformiteReponseClientTypezes
            if (nonConformiteReponseClientType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            nonConformiteReponseClientType.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(model.LibelleFRFR);
            nonConformiteReponseClientType.LibelleENGB = Utils.Utils.SetEmptyStringToNull(model.LibelleENGB);
            nonConformiteReponseClientType.Actif = model.Actif;
            //Register session user
            nonConformiteReponseClientType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = nonConformiteReponseClientType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated NonConformiteReponseClientType to the client.
                return new JsonResult(
                    _mapper.Map<NonConformiteReponseClientType, NonConformiteReponseClientTypeViewModel>(nonConformiteReponseClientType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the NonConformiteReponseClientType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the nonConformiteReponseClientType from the Database
            var nonConformiteReponseClientType = DbContext.NonConformiteReponseClientTypes.Where(i => i.RefNonConformiteReponseClientType == id).FirstOrDefault();
            // handle requests asking for non-existing nonConformiteReponseClientTypezes
            if (nonConformiteReponseClientType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = nonConformiteReponseClientType.IsDeletable();
            if (del == "")
            {
                DbContext.NonConformiteReponseClientTypes.Remove(nonConformiteReponseClientType);
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
        /// GET: api/nonConformiteReponseClientType/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true" ? true : false);
            string refNonConformiteReponseClientType = Request.Headers["refNonConformiteReponseClientType"].ToString();
            int r = 0;
            //Query
            System.Linq.IQueryable<eVaSys.Data.NonConformiteReponseClientType> req = DbContext.NonConformiteReponseClientTypes;
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (int.TryParse(refNonConformiteReponseClientType, out r)) { req = req.Where(el => el.Actif == true || el.RefNonConformiteReponseClientType == r); }
                else { req = req.Where(el => el.Actif == true); }
            }
            //Get data
            NonConformiteReponseClientTypeViewModel[] all;
            if (CurrentContext.CurrentCulture.Name == "fr-FR")
            {
                all = req.OrderBy(el => el.LibelleFRFR).Select(p => new NonConformiteReponseClientTypeViewModel()
                {
                    RefNonConformiteReponseClientType = p.RefNonConformiteReponseClientType,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleFRFR,
                    Actif = p.Actif
                }).ToArray();
            }
            else
            {
                all = req.OrderBy(el => el.LibelleENGB).Select(p => new NonConformiteReponseClientTypeViewModel()
                {
                    RefNonConformiteReponseClientType = p.RefNonConformiteReponseClientType,
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
