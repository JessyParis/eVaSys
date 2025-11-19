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
    public class TicketController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public TicketController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/ticket/{id}
        /// Retrieves the Ticket with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Ticket</param>
        /// <returns>the Ticket with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Ticket ticket = null;
            //Get or create ticket
            if (id == 0)
            {
                ticket = new Ticket();
                DbContext.Tickets.Add(ticket);
            }
            else
            {
                ticket = DbContext.Tickets
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefTicket == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (ticket == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            ticket.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<Ticket, TicketViewModel>(ticket),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Ticket with the given {id}
        /// </summary>
        /// <param name="model">The TicketViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] TicketViewModel model)
        {
            string send = Request.Headers["send"].ToString();
            string valid = null;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Ticket ticket;
            //Retrieve or create the entity to edit
            if (model.RefTicket > 0)
            {
                ticket = DbContext.Tickets
                        .Where(q => q.RefTicket == model.RefTicket)
                        .FirstOrDefault();
            }
            else
            {
                ticket = new Ticket();
                DbContext.Tickets.Add(ticket);
            }
            // handle requests asking for non-existing ticketzes
            if (ticket == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref ticket, model);

            //Register session user
            ticket.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            valid = ticket.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Ticket to the client.
                return new JsonResult(
                    _mapper.Map<Ticket, TicketViewModel>(ticket),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Ticket with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string deleteAction = Request.Headers["deleteAction"].ToString();
            // retrieve the ticket from the Database
            var ticket = DbContext.Tickets.Where(i => i.RefTicket == id).FirstOrDefault();

            // handle requests asking for non-existing ticketzes
            if (ticket == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = ticket.IsDeletable();
            if (del == "")
            {
                DbContext.Tickets.Remove(ticket);
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
        /// GET: api/items/getall
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all items.</returns>
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            string filterText = Request.Headers["filterText"].ToString();
            //Where
            var all = DbContext.Tickets
                .Where(el => el.Libelle.Contains(filterText) || el.TicketTexte.Contains(filterText)).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Ticket[], TicketViewModel[]>(all),
                JsonSettings);
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref Ticket dataModel, TicketViewModel viewModel)
        {
            dataModel.RefPays = viewModel.Pays.RefPays;
            dataModel.Libelle = Utils.Utils.SetEmptyStringToNull(viewModel.Libelle);
            dataModel.TicketTexte = Utils.Utils.SetEmptyStringToNull(viewModel.TicketTexte);
        }
        #endregion
    }
}

