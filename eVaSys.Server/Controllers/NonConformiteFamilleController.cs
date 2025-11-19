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
    public class NonConformiteFamilleController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public NonConformiteFamilleController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/nonconformitefamille/{id}
        /// Retrieves the NonConformiteFamille with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NonConformiteFamille</param>
        /// <returns>The NonConformiteFamille with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var nonConformiteFamille = DbContext.NonConformiteFamilles
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefNonConformiteFamille == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (nonConformiteFamille == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<NonConformiteFamille, NonConformiteFamilleViewModel>(nonConformiteFamille),
                JsonSettings);
        }

        /// <summary>
        /// Edit the NonConformiteFamille with the given {id}
        /// </summary>
        /// <param name="model">The NonConformiteFamilleViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] NonConformiteFamilleViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            NonConformiteFamille nonConformiteFamille;
            //Retrieve or create the entity to edit
            if (model.RefNonConformiteFamille > 0)
            {
                nonConformiteFamille = DbContext.NonConformiteFamilles
                        .Where(q => q.RefNonConformiteFamille == model.RefNonConformiteFamille)
                        .FirstOrDefault();
            }
            else
            {
                nonConformiteFamille = new NonConformiteFamille();
                DbContext.NonConformiteFamilles.Add(nonConformiteFamille);
            }
            // handle requests asking for non-existing nonConformiteFamillezes
            if (nonConformiteFamille == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            nonConformiteFamille.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(model.LibelleFRFR);
            nonConformiteFamille.LibelleENGB = Utils.Utils.SetEmptyStringToNull(model.LibelleENGB);
            nonConformiteFamille.Actif = model.Actif;
            nonConformiteFamille.IncitationQualite = model.IncitationQualite;
            //Register session user
            nonConformiteFamille.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = nonConformiteFamille.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated NonConformiteFamille to the client.
                return new JsonResult(
                    _mapper.Map<NonConformiteFamille, NonConformiteFamilleViewModel>(nonConformiteFamille),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the NonConformiteFamille with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the nonConformiteFamille from the Database
            var nonConformiteFamille = DbContext.NonConformiteFamilles.Where(i => i.RefNonConformiteFamille == id).FirstOrDefault();
            // handle requests asking for non-existing nonConformiteFamillezes
            if (nonConformiteFamille == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = nonConformiteFamille.IsDeletable();
            if (del == "")
            {
                DbContext.NonConformiteFamilles.Remove(nonConformiteFamille);
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
        /// GET: api/nonconformitefamille/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true" ? true : false);
            string refNonConformiteFamilles = Request.Headers["refNonConformiteFamilles"].ToString();
            //Query
            System.Linq.IQueryable<eVaSys.Data.NonConformiteFamille> req = DbContext.NonConformiteFamilles;
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (refNonConformiteFamilles != "")
                {
                    List<int> ids = refNonConformiteFamilles.Split(',').Select(int.Parse).ToList();
                    req = req.Where(el => el.Actif == true || ids.Contains(el.RefNonConformiteFamille));
                }
                else { req = req.Where(el => el.Actif == true); }
            }
            //Get data
            NonConformiteFamilleViewModel[] all;
            if (CurrentContext.CurrentCulture.Name == "fr-FR")
            {
                all = req.OrderBy(el => el.LibelleFRFR).Select(p => new NonConformiteFamilleViewModel()
                {
                    RefNonConformiteFamille = p.RefNonConformiteFamille,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleFRFR,
                    Actif = p.Actif
                }).ToArray();
            }
            else
            {
                all = req.OrderBy(el => el.LibelleENGB).Select(p => new NonConformiteFamilleViewModel()
                {
                    RefNonConformiteFamille = p.RefNonConformiteFamille,
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
