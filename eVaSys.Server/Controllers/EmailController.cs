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
using eValorplast.BLL;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class EmailController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public EmailController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/email/{id}
        /// Retrieves the Email with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Email</param>
        /// <returns>the Email with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Email email = null;
            //Get or create email
            if (id == 0)
            {
                email = new Email();
                DbContext.Emails.Add(email);
            }
            else
            {
                email = DbContext.Emails
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefEmail == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (email == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            email.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<Email, EmailViewModel>(email),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Email with the given {id}
        /// </summary>
        /// <param name="model">The EmailViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] EmailViewModel model)
        {
            string send = Request.Headers["send"].ToString();
            string emailing = Request.Headers["emailing"].ToString();
            string d = Request.Headers["d"].ToString();
            string year = Request.Headers["year"].ToString();
            string valid = null;
            var email = new Email();
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            // retrieve the email to edit if not new
            if (model.RefEmail != 0)
            {
                email = DbContext.Emails
                        .Where(q => q.RefEmail == model.RefEmail)
                        .FirstOrDefault();
                // handle requests asking for non-existing email
                if (email == null)
                {
                    return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
                }
            }
            else { DbContext.Emails.Add(email); }
            //Set values
            if (!(model.Entrant ?? false) && model.DEnvoi == null)
            {
                UpdateData(ref email, model);
                //Register session user
                email.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                //Check validation
                valid = email.IsValid();
                //End
                if (string.IsNullOrEmpty(valid))
                {
                    string r = CurrentContext.CulturedRessources.GetTextRessource(677);
                    int y = 0;
                    int nb = -1;
                    //Persist to database or send emailing
                    switch (emailing)
                    {
                        case "IncitationQualite":
                            int.TryParse(year, out y);
                            //End if incorrect parameter
                            if (y == 0) { return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711))); }
                            //Process if everything ok
                            r = EmailUtils.SendEmailing(email, emailing, y, DateTime.MinValue, DbContext, CurrentContext, Configuration);
                            int.TryParse(r, out nb);
                            if (nb >= 0) { return new JsonResult(nb); }
                            else
                            {
                                //End if incorrect parameter
                                return BadRequest(new BadRequestError(r));
                            }
                            break;
                        case "EmailNoteCreditCollectivite":
                            DateTime dT = DateTime.MinValue;
                            DateTime.TryParse(d, out dT);
                            //End if incorrect parameter
                            if (dT == DateTime.MinValue) { return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711))); }
                            //Process if everything ok
                            r = EmailUtils.SendEmailing(email, emailing, y, dT, DbContext, CurrentContext, Configuration);
                            int.TryParse(r, out nb);
                            if (nb >= 0) { return new JsonResult(nb); }
                            else
                            {
                                //End if incorrect parameter
                                return BadRequest(new BadRequestError(r));
                            }
                            break;
                        default:
                            // persist the changes into the Database.
                            DbContext.SaveChanges();
                            //Send e-mail if applicable
                            if (send == "true")
                            {
                                r = EmailUtils.Send(email, CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                                if (r == "ok")
                                {
                                    var action = DbContext.Actions.Where(el => el.RefAction == email.RefAction).FirstOrDefault();
                                    if (action != null)
                                    {
                                        action.Libelle = action.Libelle.Replace("(non envoyé)", "(envoyé le " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ")");
                                        DbContext.SaveChanges();
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            if (string.IsNullOrEmpty(valid))
            {
                //Return the updated Email to the client
                return new JsonResult(
                    _mapper.Map<Email, EmailViewModel>(email),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Email with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string deleteAction = Request.Headers["deleteAction"].ToString();
            // retrieve the email from the Database
            var email = DbContext.Emails.Where(i => i.RefEmail == id).FirstOrDefault();

            // handle requests asking for non-existing emailzes
            if (email == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the email from the DbContext.
            DbContext.Emails.Remove(email);
            // remove the Action and Email from the DbContext if applicable 
            if (deleteAction == "true")
            {
                var action = DbContext.Actions
                    .Include(r => r.ActionActionTypes)
                    .Include(r => r.ActionDocumentTypes)
                    .Include(r => r.ActionFichierNoFiles)
                    .Include(r => r.Emails)
                    .Where(i => i.RefAction == email.RefAction)
                    .FirstOrDefault();
                if (action != null) { DbContext.Actions.Remove(action); }
            }
            // persist the changes into the Database.
            DbContext.SaveChanges();

            // return an HTTP Status 200 (OK).
            return Ok();
        }
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// Create new e-mail based on a model
        /// </summary>
        [HttpGet("createemail")]
        public IActionResult CreateEmail()
        {
            //Init error message
            string err = "";
            string emailType = Request.Headers["emailType"].ToString();
            string d = Request.Headers["d"].ToString();
            string year = Request.Headers["year"].ToString();
            string body = "";
            Email email = new();
            DbContext.Emails.Add(email);
            IMail msg;
            MailBuilder mBuilder;
            //Init
            msg = email.Message;
            //Create e-mail
            switch (emailType)
            {
                case "IncitationQualite":
                    //Create e-mail
                    body = "";
                    //Création du corps du message
                    body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">"
                        + WebUtility.HtmlEncode("Bonjour,") + "<br/><br/>"
                        + WebUtility.HtmlEncode("Nous avons le plaisir de vous informer que la note de crédit (ou le courrier de non versement) de l’année " + year.ToString() + " relative à la mise en œuvre de la procédure d’autocontrôle est en ligne.") + "<br/><br/>"
                        + WebUtility.HtmlEncode("Vous pouvez télécharger votre document en vous connectant sur votre espace ") + "<a href='https://app.e-valorplast.com/valorplast' target='_blank'>" + WebUtility.HtmlEncode("e-Valorplast") + "</a>" + WebUtility.HtmlEncode("/Mes documents.") + "<br/><br/>"
                        + WebUtility.HtmlEncode("N’hésitez pas à consulter la statistique « Etat des enlèvements et incitation autocontrôle » afin d’avoir plus de détail.") + "<br/><br/>"
                        + WebUtility.HtmlEncode("Vous en souhaitant bonne réception,") + "<br/><br/>"
                        + WebUtility.HtmlEncode("L’équipe Valorplast") + "<br/><br/>"
                        + "<strong>" + WebUtility.HtmlEncode("VALORPLAST") + "</strong><br/>"
                        + WebUtility.HtmlEncode("21, rue d’Artois") + "<br/>"
                        + WebUtility.HtmlEncode("75008 Paris") + "<br/>"
                        + WebUtility.HtmlEncode("Tél. :  +33 (0)1 88 46 10 08") + "<br/>"
                        + "<a href='http://www.valorplast.com' target='_self'>" + WebUtility.HtmlEncode("www.valorplast.com") + "</a><br/><br/><br/>"
                        + "</span>"
                        + "<div style='font-family:Arial; font-size:8pt; color:green;'>"
                        + WebUtility.HtmlEncode("Avant d'imprimer ce mail, demandez-vous si vous avez vraiment besoin d'une copie papier ! ") + "<br/><br/>"
                        + "</div>";
                    mBuilder = msg.ToBuilder();
                    mBuilder.Html = body;
                    msg = mBuilder.Create();
                    email.Message = msg;
                    return new JsonResult(
                        _mapper.Map<Email, EmailViewModel>(email),
                        JsonSettings);
                case "EmailNoteCreditCollectivite":
                    DateTime dT = DateTime.Now.AddDays(-DateTime.Now.Day);
                    Quarter q = new(dT, CurrentContext.CurrentCulture);
                    //Create e-mail
                    body = "";
                    //Création du corps du message
                    body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">"
                        + WebUtility.HtmlEncode("Bonjour,") + "<br/><br/>"
                        + WebUtility.HtmlEncode("Nous avons le plaisir de vous informer que les documents relatifs à la reprise des balles de plastiques pour le ") + q.NameHTML + WebUtility.HtmlEncode(" sont en ligne.") + "<br/><br/>"
                        + WebUtility.HtmlEncode("Vous pouvez les télécharger en vous connectant sur la page d’accueil de notre site internet ") + "<a href='https://www.valorplast.com' target='_blank'>" + WebUtility.HtmlEncode("www.valorplast.com") + "</a>" + WebUtility.HtmlEncode(" / espace e-Valorplast")
                        + WebUtility.HtmlEncode(", ou directement via l'application ") + "<a href='https://app.e-valorplast.com/valorplast' target='_blank'>" + WebUtility.HtmlEncode("e-Valorplast") + "</a>"
                        + "<br/><br/>"
                        + WebUtility.HtmlEncode("Pensez à vous munir de votre identifiant et de votre mot de passe.") + "<br/><br/>"
                        + WebUtility.HtmlEncode("Vous en souhaitant bonne réception,") + "<br/><br/>"
                        + WebUtility.HtmlEncode("L’équipe Valorplast") + "<br/><br/>"
                        + "<strong>" + WebUtility.HtmlEncode("VALORPLAST") + "</strong><br/>"
                        + WebUtility.HtmlEncode("21, rue d’Artois") + "<br/>"
                        + WebUtility.HtmlEncode("75008 Paris") + "<br/>"
                        + WebUtility.HtmlEncode("Tél. : +33 (0) 1 88 46 10 07") + "<br/>"
                        + "<a href='http://www.valorplast.com' target='_self'>" + WebUtility.HtmlEncode("www.valorplast.com") + "</a><br/><br/><br/>"
                        + "</span>"
                        + "<div style='font-family:Arial; font-size:8pt; color:green;'>"
                        + WebUtility.HtmlEncode("Avant d'imprimer ce mail, demandez-vous si vous avez vraiment besoin d'une copie papier ! ") + "<br/><br/>"
                        + "</div>";
                    mBuilder = msg.ToBuilder();
                    mBuilder.Html = body;
                    msg = mBuilder.Create();
                    email.Message = msg;
                    return new JsonResult(
                        _mapper.Map<Email, EmailViewModel>(email),
                        JsonSettings);
            }
            //Error
            return Conflict(new ConflictError(err));
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref Email dataModel, EmailViewModel viewModel)
        {
            if (!(dataModel.Entrant ?? false) && dataModel.DEnvoi == null)
            {
                dataModel.RefParamEmail = viewModel.ParamEmail?.RefParamEmail;
                dataModel.Entrant = viewModel.Entrant ?? false;
                dataModel.Libelle = Utils.Utils.SetEmptyStringToNull(viewModel.Libelle);
                dataModel.POPId = viewModel.POPId;
                dataModel.EmailFrom = Utils.Utils.SetEmptyStringToNull(viewModel.EmailFrom);
                dataModel.EmailTo = Utils.Utils.SetEmptyStringToNull(viewModel.EmailTo);
                dataModel.EmailSubject = Utils.Utils.SetEmptyStringToNull(viewModel.EmailSubject);
                dataModel.DReception = viewModel.DReception;
                dataModel.DEnvoi = viewModel.DEnvoi;
                dataModel.EmailTextBody = Utils.Utils.SetEmptyStringToNull(viewModel.EmailTextBody);
                dataModel.RefAction = viewModel.Action?.RefAction;
                //Mail message
                var msg = dataModel.Message;
                MailBuilder mBuilder = msg.ToBuilder();
                mBuilder.Html = viewModel.EmailHTMLBody;
                //delete attached files
                foreach (var eF in dataModel.EmailFichiers.OrderByDescending(o => o.Index))
                {
                    if (viewModel.EmailFichiers.Where(el => el.VisualType == eF.VisualType && el.Index == eF.Index).FirstOrDefault() == null)
                    {
                        if (eF.VisualType == "nonvisual")
                            mBuilder.NonVisuals.RemoveAt(eF.Index);
                    }
                }
                msg = mBuilder.Create();
                msg.From.Clear();
                msg.From.Add(new Limilabs.Mail.Headers.MailBox(viewModel.ParamEmail.EmailExpediteur, viewModel.ParamEmail.LibelleExpediteur));
                msg.To.Clear();
                MailAddressParser mParser = new();
                foreach (MailAddress mA in new MailAddressParser().Parse(viewModel.EmailTo))
                {
                    msg.To.Add(mA);
                }
                //Objet
                msg.Subject = Utils.Utils.SetEmptyStringToNull(viewModel.EmailSubject);
                dataModel.Message = msg;
            }
        }
        #endregion
    }
}

