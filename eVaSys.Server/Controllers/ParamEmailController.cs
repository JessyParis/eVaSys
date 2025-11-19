/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/12/2019
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
    public class ParamEmailController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ParamEmailController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/actiontype/{id}
        /// Retrieves the ParamEmail with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing ParamEmail</param>
        /// <returns>the ParamEmail with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            ParamEmail paramEmail = null;
            //Get or create paramEmail
            if (id == 0)
            {
                paramEmail = new ParamEmail();
                DbContext.ParamEmails.Add(paramEmail);
            }
            else
            {
                paramEmail = DbContext.ParamEmails
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefParamEmail == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (paramEmail == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            paramEmail.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<ParamEmail, ParamEmailViewModel>(paramEmail),
                JsonSettings);
        }

        /// <summary>
        /// Modify ParamEmail
        /// </summary>
        /// <param name="model">The ParamEmailViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] ParamEmailViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            ParamEmail paramEmail;
            //Retrieve or create the entity to edit
            if (model.RefParamEmail > 0)
            {
                paramEmail = DbContext.ParamEmails
                        .Where(q => q.RefParamEmail == model.RefParamEmail)
                        .FirstOrDefault();
            }
            else
            {
                paramEmail = new ParamEmail();
                DbContext.ParamEmails.Add(paramEmail);
            }

            // handle requests asking for non-existing paramEmailzes
            if (paramEmail == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref paramEmail, model);

            //Register session user
            paramEmail.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = paramEmail.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Return the updated ParamEmail to the client, or return the next Mixte if applicable
                return new JsonResult(
                    _mapper.Map<ParamEmail, ParamEmailViewModel>(paramEmail),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the ParamEmail with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the paramEmail from the Database
            var paramEmail = DbContext.ParamEmails.Where(i => i.RefParamEmail == id)
                .FirstOrDefault();

            // handle requests asking for non-existing paramEmailzes
            if (paramEmail == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = paramEmail.IsDeletable();
            if (del == "")
            {
                DbContext.ParamEmails.Remove(paramEmail);
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
        /// GET: api/paramEmail/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true" ? true : false);
            bool defaut = (Request.Headers["defaut"] == "false" ? false : true);
            string refParamEmail = Request.Headers["refParamEmail"].ToString();
            int r = 0;
            //Query
            System.Linq.IQueryable<eVaSys.Data.ParamEmail> req = DbContext.ParamEmails;
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (int.TryParse(refParamEmail, out r)) { req = req.Where(el => el.Actif == true || el.RefParamEmail == r); }
                else { req = req.Where(el => el.Actif == true); }
            }
            if (defaut)
            {
                req = req.Where(el => el.Defaut);
            }
            //Get data
            var all = req.OrderBy(el => el.EmailExpediteur).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<ParamEmail[], ParamEmailListViewModel[]>(all),
                JsonSettings);
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref ParamEmail dataModel, ParamEmailViewModel viewModel)
        {
            dataModel.Actif = viewModel.Actif ?? false;
            dataModel.RttClient = viewModel.RttClient ?? false;
            dataModel.POP = Utils.Utils.SetEmptyStringToNull(viewModel.POP);
            dataModel.PortPOP = viewModel.PortPOP;
            dataModel.ComptePOP = Utils.Utils.SetEmptyStringToNull(viewModel.ComptePOP);
            dataModel.PwdPOP = Utils.Utils.SetEmptyStringToNull(viewModel.PwdPOP);
            dataModel.TLSPOP = viewModel.TLSPOP ?? false;
            dataModel.EnTeteEmail = Utils.Utils.SetEmptyStringToNull(viewModel.EnTeteEmail);
            dataModel.PiedEmail = Utils.Utils.SetEmptyStringToNull(viewModel.PiedEmail);
            dataModel.EmailExpediteur = Utils.Utils.SetEmptyStringToNull(viewModel.EmailExpediteur);
            dataModel.LibelleExpediteur = Utils.Utils.SetEmptyStringToNull(viewModel.LibelleExpediteur);
            dataModel.SMTP = Utils.Utils.SetEmptyStringToNull(viewModel.SMTP);
            dataModel.PortSMTP = viewModel.PortSMTP;
            dataModel.CompteSMTP = Utils.Utils.SetEmptyStringToNull(viewModel.CompteSMTP);
            dataModel.PwdSMTP = Utils.Utils.SetEmptyStringToNull(viewModel.PwdSMTP);
            dataModel.AllowAuthSMTP = viewModel.AllowAuthSMTP ?? false;
            dataModel.TLSSMTP = viewModel.TLSSMTP ?? false;
            dataModel.StartTLSSMTP = viewModel.StartTLSSMTP ?? false;
            dataModel.AR = viewModel.AR ?? false;
            dataModel.AREmail = Utils.Utils.SetEmptyStringToNull(viewModel.AREmail);
            dataModel.AREmailSujet = Utils.Utils.SetEmptyStringToNull(viewModel.AREmailSujet);
            dataModel.ARExpediteurEmail = Utils.Utils.SetEmptyStringToNull(viewModel.ARExpediteurEmail);
            dataModel.ARExpediteurLibelle = Utils.Utils.SetEmptyStringToNull(viewModel.ARExpediteurLibelle);
        }
        #endregion
    }
}