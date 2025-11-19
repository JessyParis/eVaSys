/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/11/2022
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
    public class ActionTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ActionTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/actiontype/{id}
        /// Retrieves the ActionType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing ActionType</param>
        /// <returns>the ActionType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            ActionType actionType = null;
            //Get or create actionType
            if (id == 0)
            {
                actionType = new ActionType();
                DbContext.ActionTypes.Add(actionType);
            }
            else
            {
                actionType = DbContext.ActionTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Include(r => r.ActionTypeEntiteTypes)
                    .Where(i => i.RefActionType == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (actionType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            actionType.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<ActionType, ActionTypeViewModel>(actionType),
                JsonSettings);
        }

        /// <summary>
        /// Modify ActionType
        /// </summary>
        /// <param name="model">The ActionTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] ActionTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            ActionType actionType;
            //Retrieve or create the entity to edit
            if (model.RefActionType > 0)
            {
                actionType = DbContext.ActionTypes
                        .Where(q => q.RefActionType == model.RefActionType)
                        .Include(r => r.ActionTypeEntiteTypes)
                        .FirstOrDefault();
            }
            else
            {
                actionType = new ActionType();
                DbContext.ActionTypes.Add(actionType);
            }

            // handle requests asking for non-existing actionTypezes
            if (actionType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref actionType, model);

            //Register session user
            actionType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = actionType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Return the updated ActionType to the client, or return the next Mixte if applicable
                return new JsonResult(
                    _mapper.Map<ActionType, ActionTypeViewModel>(actionType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the ActionType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the actionType from the Database
            var actionType = DbContext.ActionTypes.Where(i => i.RefActionType == id)
                .Include(r => r.ActionTypeEntiteTypes)
                .FirstOrDefault();

            // handle requests asking for non-existing actionTypezes
            if (actionType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = actionType.IsDeletable();
            if (del == "")
            {
                DbContext.ActionTypes.Remove(actionType);
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
        /// GET: api/items/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            int? refEntiteType = (string.IsNullOrEmpty(Request.Query["refEntiteType"]) ? null : (int?)System.Convert.ToInt32(Request.Query["refEntiteType"]));
            System.Linq.IQueryable<eVaSys.Data.ActionType> req = DbContext.ActionTypes;
            if (refEntiteType != null)
            {
                req = req.Where(el => el.ActionTypeEntiteTypes.Any(rel => rel.RefEntiteType == refEntiteType));
            }
            //Get data
            var all = req.Distinct().OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<ActionType[], ActionTypeViewModel[]>(all),
                JsonSettings);
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref ActionType dataModel, ActionTypeViewModel viewModel)
        {
            dataModel.Libelle = Utils.Utils.SetEmptyStringToNull(viewModel.Libelle);
            dataModel.DocumentType = viewModel.DocumentType;
            if (dataModel.ActionTypeEntiteTypes == null) { dataModel.ActionTypeEntiteTypes = new HashSet<ActionTypeEntiteType>(); }
            //Dirty marker
            bool dirty = false;
            //Remove related data ActionTypeEntiteType
            if (dataModel.ActionTypeEntiteTypes != null)
            {
                foreach (ActionTypeEntiteType aTET in dataModel.ActionTypeEntiteTypes)
                {
                    if (viewModel.ActionTypeEntiteTypes == null)
                    {
                        DbContext.ActionTypeEntiteTypes.Remove(aTET);
                        dirty = true;
                    }
                    else if (viewModel.ActionTypeEntiteTypes.Where(el => el.RefActionTypeEntiteType == aTET.RefActionTypeEntiteType).FirstOrDefault() == null)
                    {
                        DbContext.ActionTypeEntiteTypes.Remove(aTET);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ActionTypeEntiteTypes
            foreach (ActionTypeEntiteTypeViewModel aTETVM in viewModel.ActionTypeEntiteTypes)
            {
                ActionTypeEntiteType aTET = null;
                if (dataModel.ActionTypeEntiteTypes != null && aTETVM.RefActionTypeEntiteType != 0)
                {
                    aTET = dataModel.ActionTypeEntiteTypes.Where(el => el.RefActionTypeEntiteType == aTETVM.RefActionTypeEntiteType).FirstOrDefault();
                }
                if (aTET == null)
                {
                    aTET = new ActionTypeEntiteType();
                    DbContext.ActionTypeEntiteTypes.Add(aTET);
                    if (dataModel.ActionTypeEntiteTypes == null) { dataModel.ActionTypeEntiteTypes = new HashSet<ActionTypeEntiteType>(); }
                    dataModel.ActionTypeEntiteTypes.Add(aTET);
                }
                //Mark as dirty if applicable
                if (aTET.RefEntiteType != aTETVM.EntiteType.RefEntiteType) { dirty = true; }
                //Update data
                aTET.RefEntiteType = aTETVM.EntiteType.RefEntiteType;
            }
            //Mark for modification if applicable
            if (dirty) { dataModel.DModif = DateTime.Now; }
        }
        #endregion
    }
}

