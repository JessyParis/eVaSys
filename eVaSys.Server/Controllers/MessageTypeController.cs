/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 21/11/2019
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
    public class MessageTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public MessageTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/messageType/{id}
        /// Retrieves the MessageType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing MessageType</param>
        /// <returns>The MessageType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var messageType = DbContext.MessageTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefMessageType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (messageType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<MessageType, MessageTypeViewModel>(messageType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the MessageType with the given {id}
        /// </summary>
        /// <param name="model">The MessageTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] MessageTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            MessageType messageType;
            //Retrieve or create the entity to edit
            if (model.RefMessageType > 0)
            {
                messageType = DbContext.MessageTypes
                        .Where(q => q.RefMessageType == model.RefMessageType)
                        .FirstOrDefault();
            }
            else
            {
                messageType = new MessageType();
                DbContext.MessageTypes.Add(messageType);
            }
            // handle requests asking for non-existing messageTypezes
            if (messageType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            messageType.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);
            //Register session user
            messageType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = messageType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated MessageType to the client.
                return new JsonResult(
                    _mapper.Map<MessageType, MessageTypeViewModel>(messageType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the MessageType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the messageType from the Database
            var messageType = DbContext.MessageTypes.Where(i => i.RefMessageType == id).FirstOrDefault();
            // handle requests asking for non-existing messageTypezes
            if (messageType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = messageType.IsDeletable();
            if (del == "")
            {
                DbContext.MessageTypes.Remove(messageType);
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
        /// GET: api/messageType/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.MessageType> req = DbContext.MessageTypes;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<MessageType[], MessageTypeViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
