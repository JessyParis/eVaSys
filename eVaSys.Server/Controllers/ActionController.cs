/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/05/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Action = eVaSys.Data.Action;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class ActionController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ActionController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/Action/{id}
        /// Retrieves the Action with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Action</param>
        /// <returns>the Action with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Data.Action action = null;
            //Get or create Action
            if (id != 0)
            {
                action = DbContext.Actions
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefAction == id).FirstOrDefault();
            }
            else
            {
                int refAction = System.Convert.ToInt32(Request.Headers["refAction"]);
                action = new Data.Action
                {
                    RefUtilisateurCreation = CurrentContext.RefUtilisateur,
                    DCreation = DateTime.Now,
                };
                DbContext.Actions.Add(action);
            }
            // handle requests asking for non-existing object
            if (action == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            action.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<Data.Action, ActionViewModel>(action),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Action with the given {id}
        /// </summary>
        /// <param name="model">The ActionViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] ActionViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Action action;
            //Retrieve or create the Action to edit
            if (model.RefAction > 0)
            {
                // retrieve the Action to edit
                action = DbContext.Actions
                    .Include(r => r.ActionActionTypes)
                    .Include(r => r.ActionDocumentTypes)
                    .Where(q => q.RefAction ==
                        model.RefAction).FirstOrDefault();
            }
            else
            {
                action = new Data.Action();
                DbContext.Actions.Add(action);
            }
            // handle requests asking for non-existing Actionzes
            if (action == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            Utils.DataUtils.UpdateDataAction(ref action, model, CurrentContext.RefUtilisateur);

            //Register session user
            action.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = action.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Action to the client.
                return new JsonResult(
                    _mapper.Map<Data.Action, ActionViewModel>(action),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Action with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the Action from the Database
            var action = DbContext.Actions.Where(i => i.RefAction == id)
                .Include(r => r.ActionActionTypes)
                .Include(r => r.ActionDocumentTypes)
                .Include(r => r.ActionFichierNoFiles)
                .Include(r => r.Emails)
                .AsSplitQuery()
                .FirstOrDefault();

            // handle requests asking for non-existing Actionzes
            if (action == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = action.IsDeletable();
            if (del == "")
            {
                DbContext.Actions.Remove(action);
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
        /// GET: api/items/getactionfichiers
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getactionfichiernofiles")]
        public IActionResult GetActionFichierNoFiles()
        {
            int? refAction = (Request.Query.ContainsKey("refAction") ? (int?)System.Convert.ToInt32(Request.Query["refAction"]) : null);
            var actionFichiers = DbContext.ActionFichierNoFiles
                .Where(i => i.RefAction == refAction).ToArray();
            // handle requests asking for non-existing object
            if (actionFichiers == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<ActionFichierNoFile[], ActionFichierNoFileViewModel[]>(actionFichiers),
                JsonSettings);
        }
        #endregion
        #region Services
        #endregion
    }
}

