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
    public class NonConformiteAccordFournisseurTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public NonConformiteAccordFournisseurTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/nonconformiteaccordfournisseurtype/{id}
        /// Retrieves the NonConformiteAccordFournisseurType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NonConformiteAccordFournisseurType</param>
        /// <returns>The NonConformiteAccordFournisseurType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var nonConformiteAccordFournisseurType = DbContext.NonConformiteAccordFournisseurTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefNonConformiteAccordFournisseurType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (nonConformiteAccordFournisseurType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<NonConformiteAccordFournisseurType, NonConformiteAccordFournisseurTypeViewModel>(nonConformiteAccordFournisseurType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the NonConformiteAccordFournisseurType with the given {id}
        /// </summary>
        /// <param name="model">The NonConformiteAccordFournisseurTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] NonConformiteAccordFournisseurTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            NonConformiteAccordFournisseurType nonConformiteAccordFournisseurType;
            //Retrieve or create the entity to edit
            if (model.RefNonConformiteAccordFournisseurType > 0)
            {
                nonConformiteAccordFournisseurType = DbContext.NonConformiteAccordFournisseurTypes
                        .Where(q => q.RefNonConformiteAccordFournisseurType == model.RefNonConformiteAccordFournisseurType)
                        .FirstOrDefault();
            }
            else
            {
                nonConformiteAccordFournisseurType = new NonConformiteAccordFournisseurType();
                DbContext.NonConformiteAccordFournisseurTypes.Add(nonConformiteAccordFournisseurType);
            }
            // handle requests asking for non-existing nonConformiteAccordFournisseurTypezes
            if (nonConformiteAccordFournisseurType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            nonConformiteAccordFournisseurType.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(model.LibelleFRFR);
            nonConformiteAccordFournisseurType.LibelleENGB = Utils.Utils.SetEmptyStringToNull(model.LibelleENGB);
            nonConformiteAccordFournisseurType.Actif = model.Actif;
            //Register session user
            nonConformiteAccordFournisseurType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = nonConformiteAccordFournisseurType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated NonConformiteAccordFournisseurType to the client.
                return new JsonResult(
                    _mapper.Map<NonConformiteAccordFournisseurType, NonConformiteAccordFournisseurTypeViewModel>(nonConformiteAccordFournisseurType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the NonConformiteAccordFournisseurType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the nonConformiteAccordFournisseurType from the Database
            var nonConformiteAccordFournisseurType = DbContext.NonConformiteAccordFournisseurTypes.Where(i => i.RefNonConformiteAccordFournisseurType == id).FirstOrDefault();
            // handle requests asking for non-existing nonConformiteAccordFournisseurTypezes
            if (nonConformiteAccordFournisseurType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = nonConformiteAccordFournisseurType.IsDeletable();
            if (del == "")
            {
                DbContext.NonConformiteAccordFournisseurTypes.Remove(nonConformiteAccordFournisseurType);
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
        /// GET: api/nonconformiteaccordfournisseurtype/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true" ? true : false);
            string refAccordFournisseurType = Request.Headers["refAccordFournisseurType"].ToString();
            int r = 0;
            //Query
            System.Linq.IQueryable<eVaSys.Data.NonConformiteAccordFournisseurType> req = DbContext.NonConformiteAccordFournisseurTypes;
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (int.TryParse(refAccordFournisseurType, out r)) { req = req.Where(el => el.Actif == true || el.RefNonConformiteAccordFournisseurType == r); }
                else { req = req.Where(el => el.Actif == true); }
            }
            //Get data
            NonConformiteAccordFournisseurTypeViewModel[] all;
            if (CurrentContext.CurrentCulture.Name == "fr-FR")
            {
                all = req.OrderBy(el => el.RefNonConformiteAccordFournisseurType).Select(p => new NonConformiteAccordFournisseurTypeViewModel()
                {
                    RefNonConformiteAccordFournisseurType = p.RefNonConformiteAccordFournisseurType,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleFRFR,
                    Actif = p.Actif
                }).ToArray();
            }
            else
            {
                all = req.OrderBy(el => el.RefNonConformiteAccordFournisseurType).Select(p => new NonConformiteAccordFournisseurTypeViewModel()
                {
                    RefNonConformiteAccordFournisseurType = p.RefNonConformiteAccordFournisseurType,
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
