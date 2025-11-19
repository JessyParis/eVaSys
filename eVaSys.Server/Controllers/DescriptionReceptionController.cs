/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class DescriptionReceptionController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public DescriptionReceptionController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/descriptionReception/{id}
        /// Retrieves the DescriptionReception with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing DescriptionReception</param>
        /// <returns>the DescriptionReception with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            DescriptionReception descriptionReception = null;
            //Get or create descriptionReception
            if (id == 0)
            {
                descriptionReception = new DescriptionReception();
                DbContext.DescriptionReceptions.Add(descriptionReception);
            }
            else
            {
                descriptionReception = DbContext.DescriptionReceptions
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefDescriptionReception == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (descriptionReception == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            descriptionReception.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<DescriptionReception, DescriptionReceptionViewModel>(descriptionReception),
                JsonSettings);
        }

        /// <summary>
        /// Edit the DescriptionReception with the given {id}
        /// </summary>
        /// <param name="model">The DescriptionReceptionViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] DescriptionReceptionViewModel model)
        {
            string valid = null;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            DescriptionReception descriptionReception;
            //Retrieve or create the entity to edit
            if (model.RefDescriptionReception > 0)
            {
                descriptionReception = DbContext.DescriptionReceptions
                    .Where(q => q.RefDescriptionReception == model.RefDescriptionReception)
                    .FirstOrDefault();
            }
            else
            {
                descriptionReception = new DescriptionReception();
                DbContext.DescriptionReceptions.Add(descriptionReception);
            }
            // handle requests asking for non-existing descriptionReceptionzes
            if (descriptionReception == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref descriptionReception, model);

            //Register session user
            descriptionReception.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            valid = descriptionReception.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated DescriptionReception to the client.
                return new JsonResult(
                    _mapper.Map<DescriptionReception, DescriptionReceptionViewModel>(descriptionReception),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the DescriptionReception with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the descriptionReception from the Database
            var descriptionReception = DbContext.DescriptionReceptions
                .Where(i => i.RefDescriptionReception == id)
                .FirstOrDefault();

            // handle requests asking for non-existing descriptionReceptionzes
            if (descriptionReception == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = descriptionReception.IsDeletable();
            if (del == "")
            {
                DbContext.DescriptionReceptions.Remove(descriptionReception);
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
        /// GET: evapi/getdescriptionReceptions
        /// Retrieves the DescriptionReceptions according to RefProduit
        /// </summary>
        /// <returns>the correponding DescriptionReceptions</returns>
        [HttpGet("getdescriptionreceptions")]
        public IActionResult GetDescriptionReceptions()
        {
            List<DescriptionReception> dCs = new();
            //Get DescriptionReceptions
            dCs = DbContext.DescriptionReceptions
                .OrderBy(o => o.Ordre)
                .ToList();
            // handle requests asking for non-existing object
            if (dCs.Count == 0)
            {
                return new EmptyResult();
            }
            //Return Json
            return new JsonResult(
                _mapper.Map<DescriptionReception[], DescriptionReceptionViewModel[]>(dCs.ToArray()),
                JsonSettings);
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref DescriptionReception dataModel, DescriptionReceptionViewModel viewModel)
        {
            dataModel.Ordre = viewModel.Ordre;
            dataModel.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(viewModel.LibelleFRFR);
            dataModel.LibelleENGB = Utils.Utils.SetEmptyStringToNull(viewModel.LibelleENGB);
            dataModel.Actif = viewModel.Actif ?? false;
            dataModel.OuiFRFR = Utils.Utils.SetEmptyStringToNull(viewModel.OuiFRFR);
            dataModel.OuiENGB = Utils.Utils.SetEmptyStringToNull(viewModel.OuiENGB);
            dataModel.NonFRFR = Utils.Utils.SetEmptyStringToNull(viewModel.NonFRFR);
            dataModel.NonENGB = Utils.Utils.SetEmptyStringToNull(viewModel.NonENGB);
            dataModel.ReponseParDefaut = viewModel.ReponseParDefaut;
        }
        #endregion
    }
}

