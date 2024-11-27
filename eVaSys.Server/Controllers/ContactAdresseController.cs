/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/06/2019
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
    public class ContactAdresseController : BaseApiController
    {
        private readonly IMapper _mapper;

        #region Constructor

        public ContactAdresseController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }

        #endregion Constructor

        #region RESTful Conventions

        /// <summary>
        /// GET: evapi/contactAdresse/{id}
        /// Retrieves the ContactAdresse with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing ContactAdresse</param>
        /// <returns>The ContactAdresse with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var contactAdresse = DbContext.ContactAdresses
                .Include(r => r.ContactAdresseContactAdresseProcesss)
                .Include(r => r.ContactAdresseDocumentTypes)
                .Include(r => r.ContactAdresseServiceFonctions)
                .Include(r => r.UtilisateurCreation)
                .Include(r => r.UtilisateurModif)
                .AsSplitQuery()
                .Where(i => i.RefContactAdresse == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (contactAdresse == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<ContactAdresse, ContactAdresseViewModel>(contactAdresse),
                JsonSettings);
        }

        /// <summary>
        /// Edit the ContactAdresse with the given {id}
        /// </summary>
        /// <param name="model">The ContactAdresseViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] ContactAdresseViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            ContactAdresse contactAdresse;
            //Retrieve or create the entity to edit
            if (model.RefContactAdresse > 0)
            {
                contactAdresse = DbContext.ContactAdresses
                .Include(r => r.ContactAdresseContactAdresseProcesss)
                .Include(r => r.ContactAdresseDocumentTypes)
                .Include(r => r.ContactAdresseServiceFonctions)
                .Include(r => r.UtilisateurCreation)
                .Include(r => r.UtilisateurModif)
                .AsSplitQuery()
                .Where(q => q.RefContactAdresse == model.RefContactAdresse)
                .FirstOrDefault();
            }
            else
            {
                contactAdresse = new ContactAdresse();
                DbContext.ContactAdresses.Add(contactAdresse);
            }

            // handle requests asking for non-existing contactAdressezes
            if (contactAdresse == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            Utils.DataUtils.UpdateDataContactAdresse(ref contactAdresse, model, CurrentContext.RefUtilisateur);
            //Check validation
            string valid = contactAdresse.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated ContactAdresse to the client.
                return new JsonResult(
                    _mapper.Map<ContactAdresse, ContactAdresseViewModel>(contactAdresse),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the ContactAdresse with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the contactAdresse from the Database
            var contactAdresse = DbContext.ContactAdresses
                .Include(r => r.ContactAdresseContactAdresseProcesss)
                .Include(r => r.ContactAdresseDocumentTypes)
                .Include(r => r.ContactAdresseServiceFonctions)
                .Where(i => i.RefContactAdresse == id)
                .FirstOrDefault();
            // handle requests asking for non-existing contactAdressezes
            if (contactAdresse == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = contactAdresse.IsDeletable(false);
            if (del == "")
            {
                DbContext.ContactAdresses.Remove(contactAdresse);
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

        #endregion RESTful Conventions

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/contactadresse/isdeletable
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>true if ContactAdresse can be deleted, false if not</returns>
        [HttpGet("isdeletable")]
        public IActionResult IsDeletable()
        {
            string refContactAdresse = Request.Headers["refContactAdresse"].ToString();
            bool excludeProcess = (Request.Headers["excludeProcess"] == "true");
            bool r = false;
            int refCA = 0;
            int.TryParse(refContactAdresse, out refCA);
            //Get ContactAdresse from database
            try
            {
                var cA = DbContext.ContactAdresses.Find(refCA);
                r = (cA.IsDeletable(excludeProcess) == "");
            }
            catch
            {
                //If ContactAdresse does not exists, then it is deletable
                r = true;
            }
            //Return Json
            return new JsonResult(r,
                JsonSettings);
        }

        /// <summary>
        /// GET: api/contactAdresse/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true");
            string refAdresse = Request.Headers["refAdresse"].ToString();
            string refEntite = Request.Headers["refEntite"].ToString();
            string refContactAdresse = Request.Headers["refContactAdresse"].ToString();
            string refContactAdresseProcess = Request.Headers["refContactAdresseProcess"].ToString();
            int refA = 0;
            int refE = 0;
            int refCA = 0;
            int refCAP = 0;
            //Where
            System.Linq.IQueryable<eVaSys.Data.ContactAdresse> req = DbContext.ContactAdresses
                .Include(r => r.ContactAdresseContactAdresseProcesss)
                .Include(r => r.ContactAdresseDocumentTypes)
                .Include(r => r.ContactAdresseServiceFonctions)
                .ThenInclude(r => r.Service)
                .Include(r => r.ContactAdresseServiceFonctions)
                .ThenInclude(r => r.Fonction)
                .AsSplitQuery()
                ;
            if (int.TryParse(refAdresse, out refA)) { if (refA != 0) { req = req.Where(el => el.RefAdresse == refA); } }
            else if (int.TryParse(refEntite, out refE)) { if (refE != 0) { req = req.Where(el => el.RefEntite == refE); } }
            else { req = req.Where(el => el.RefContactAdresse == 0); }
            if (int.TryParse(refContactAdresseProcess, out refCAP))
            {
                if (refCAP != 0) { req = req.Where(el => el.ContactAdresseContactAdresseProcesss.Any(related => related.RefContactAdresseProcess == refCAP)); }
            }
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (int.TryParse(refContactAdresse, out refCA))
                {
                    req = req.Where(el => (el.Actif == true && (el.Adresse == null || el.Adresse.Actif == true) && el.Entite.Actif == true) || el.RefContactAdresse == refCA);
                }
                else
                {
                    req = req.Where(el => (el.Actif == true && (el.Adresse == null || el.Adresse.Actif == true) && el.Entite.Actif == true));
                }
            }
            //Get data
            var all = req.OrderBy(el => el.Contact.Nom).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<ContactAdresse[], ContactAdresseViewModel[]>(all),
                JsonSettings);
        }

        #endregion Attribute-based Routing

        #region Services
        #endregion Services
    }
}