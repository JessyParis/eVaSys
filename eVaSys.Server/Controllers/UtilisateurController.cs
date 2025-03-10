/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 04/07/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using eVaSys.Utils;
using eVaSys.APIUtils;
using System.Globalization;
using Microsoft.Data.SqlClient;
using System.Data;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class UtilisateurController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public UtilisateurController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/Utilisateur/{id}
        /// Retrieves the Utilisateur with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Utilisateur</param>
        /// <returns>the Utilisateur with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var utilisateur = DbContext.Utilisateurs
                    .Include(r => r.ContactAdresse)
                    .ThenInclude(r => r.ContactAdresseContactAdresseProcesss)
                    .Include(r => r.UtilisateurMaitre)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Include(r => r.UtilisateurAffectationMaitre)
                    .Where(i => i.RefUtilisateur == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (utilisateur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Don't send password
            var utilisateurVM = _mapper.Map<Utilisateur, UtilisateurViewModel>(utilisateur);
            utilisateurVM.Pwd = null;
            utilisateurVM.PwdExists = !string.IsNullOrEmpty(utilisateur.Pwd);
            //End
            return new JsonResult(
                utilisateurVM,
                JsonSettings);
        }

        /// <summary>
        /// Edit the Utilisateur with the given {id}
        /// </summary>
        /// <param name="model">The UtilisateurViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] UtilisateurViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Utilisateur utilisateur;
            //Retrieve or create the entity to edit
            if (model.RefUtilisateur > 0)
            {
                utilisateur = DbContext.Utilisateurs
                        .Where(q => q.RefUtilisateur == model.RefUtilisateur)
                        .FirstOrDefault();
            }
            else
            {
                utilisateur = new Utilisateur();
                DbContext.Utilisateurs.Add(utilisateur);
            }
            // handle requests asking for non-existing Utilisateur
            if (utilisateur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Store Maitre
            bool assignmentMaitre = false;

            //Set values
            UpdateData(ref utilisateur, model, ref assignmentMaitre);

            //Register session user
            utilisateur.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = utilisateur.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                DbContext.Entry(utilisateur).Reload();
                //Check if creation or reset
                string operationType = "CreationConfirmation";
                if (utilisateur.PwdHash != null) { operationType = "ResetConfirmation"; }
                //Check new password, exit if same as existing
                if (!string.IsNullOrWhiteSpace(model.Pwd))
                {
                    var refUtilisateur = new SqlParameter("@refUtilisateur", utilisateur.RefUtilisateur);
                    var pwd = new SqlParameter("@pwd", model.Pwd);
                    var msg = new SqlParameter("@msg", SqlDbType.NVarChar, 50);
                    msg.Direction = ParameterDirection.Output;
                    DbContext.Database.ExecuteSqlRaw(
                            "dbo.CreateEncryptedPwd @refUtilisateur,@pwd, @msg out",
                            refUtilisateur,
                            pwd,
                            msg);
                    if (msg.Value.ToString() == "same")
                    {
                        return Conflict(new ConflictError(CurrentContext.CulturedRessources.GetTextRessource(1103)));
                    }
                    else
                    {
                        DbContext.Entry(utilisateur).Reload();
                    }
                }
                //Send notification e-mail if applicable
                string resetPassword = Request.Headers["resetPassword"].ToString();
                if (resetPassword == "true")
                {
                    if (operationType == "CreationConfirmation")
                    {
                        EmailUtils.SendEmailPasswordManagement("Creation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                    }
                    else
                    {
                        EmailUtils.SendEmailPasswordManagement("Reset", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                    }
                }
                //Send notification e-mail if applicable
                if (assignmentMaitre)
                {
                    EmailUtils.SendEmailAssignmentMaitre("AssignmentMaitre", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                }
                // return the updated MotifAnomalieTransporteur to the client.
                return new JsonResult(
                    _mapper.Map<Utilisateur, UtilisateurViewModel>(utilisateur),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Utilisateur with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the Utilisateur from the Database
            var Utilisateur = DbContext.Utilisateurs.Where(i => i.RefUtilisateur == id)
                .FirstOrDefault();

            // handle requests asking for non-existing Utilisateurzes
            if (Utilisateur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = Utilisateur.IsDeletable();
            if (del == "")
            {
                //Remove link to slaves if applicable
                foreach (var u in DbContext.Utilisateurs.Where(e => e.RefUtilisateurMaitre == Utilisateur.RefUtilisateur).ToList())
                {
                    u.RefUtilisateurMaitre = null;
                }
                //Remove Utilisateur
                DbContext.Utilisateurs.Remove(Utilisateur);
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
        /// GET: api/items/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlist")]
        public IActionResult GetList()
        {
            string module = Request.Headers["module"].ToString();
            string habilitation = Request.Headers["habilitation"].ToString();
            bool actif = (Request.Headers.ContainsKey("actif") ? (Request.Headers["actif"] == "true" ? true : false) : false);
            string refClient = Request.Headers["refClient"].ToString();
            string refUtilisateur = Request.Headers["refUtilisateur"].ToString();
            bool getProfils = (Request.Headers.ContainsKey("getProfils") ? (Request.Headers["getProfils"] == "true" ? true : false) : false);
            bool getAvailableEsclaves = (Request.Headers.ContainsKey("getAvailableEsclaves") ? (Request.Headers["getAvailableEsclaves"] == "true" ? true : false) : false);
            bool getMaitres = (Request.Headers.ContainsKey("getMaitres") ? (Request.Headers["getMaitres"] == "true" ? true : false) : false);
            int refC = 0;
            int refU = 0;
            int.TryParse(refClient, out refC);
            var req = DbContext.Utilisateurs.AsQueryable();
            if (module != "" && habilitation != "")
            {
                if (module == Enumerations.ModuleName.Administration.ToString()) { req = req.Where(el => el.HabilitationAdministration == habilitation); }
                else if (module == Enumerations.ModuleName.Annuaire.ToString()) { req = req.Where(el => el.HabilitationAnnuaire == habilitation); }
                else if (module == Enumerations.ModuleName.Logistique.ToString()) { req = req.Where(el => el.HabilitationLogistique == habilitation); }
                else if (module == Enumerations.ModuleName.Messagerie.ToString()) { req = req.Where(el => el.HabilitationMessagerie == habilitation); }
                else if (module == Enumerations.ModuleName.Qualite.ToString())
                {
                    if (habilitation == Enumerations.HabilitationQualite.Utilisateur.ToString())
                    {
                        req = req.Include(i => i.EntiteDRs);
                        req = req.Where(el => el.HabilitationQualite == habilitation && el.EntiteDRs.Count > 0);
                    }
                    else
                    {
                        req = req.Where(el => el.HabilitationQualite == habilitation);
                    }
                }
                else if (module == Enumerations.ModuleName.ModuleCollectivite.ToString()) { req = req.Where(el => el.HabilitationModuleCollectivite == habilitation); }
                else if (module == Enumerations.ModuleName.Statistique.ToString()) { req = req.Where(el => el.HabilitationStatistique == habilitation); }
                else if (module == Enumerations.ModuleName.ModulePrestataire.ToString()) { req = req.Where(el => el.HabilitationModulePrestataire == habilitation); }
            }
            //Add current user even if inactive
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (int.TryParse(refUtilisateur, out refU)) { req = req.Where(el => el.Actif == true || el.RefUtilisateur == refU); }
                else { req = req.Where(el => el.Actif == true); }
            }
            //Return all profils
            if (getProfils)
            {
                req = req.Where(el => el.RefUtilisateurMaitre == refU || el.RefUtilisateur == refU);
            }
            //Return available slaves
            if (getAvailableEsclaves)
            {
                req = req.Where(el => el.RefUtilisateurMaitre == null && el.RefUtilisateurMaitre != refU);
            }
            //Return only Maitre
            if (getMaitres)
            {
                req = req.Where(el => el.UtilisateurMaitres.Count()>0);
            }
            var all = req.OrderBy(el => el.Nom).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Utilisateur[], UtilisateurListViewModel[]>(all),
                JsonSettings);
        }

        /// <summary>
        /// GET: evapi/Utilisateur/getconnected
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>The connected user (the one that just logged in).</returns>
        [HttpGet("getconnected")]
        public IActionResult GetConnected()
        {
            bool r = false;
            Utilisateur u = new();
            if (HttpContext.Request.Headers["eValorplastSecurity"] != "")
            {
                JwtSecurityToken jwt = new(HttpContext.Request.Headers["eValorplastSecurity"]);
                var tokenExpirationMins = Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");
                //Check token
                if (jwt.ValidTo.AddMinutes(tokenExpirationMins) > DateTime.UtcNow)
                {
                    //Get user from token
                    if (jwt.Subject != "")
                    {
                        u = DbContext.Utilisateurs
                            .Include(el => el.CentreDeTri.EntiteDRs)
                            .Where(i => (i.RefUtilisateur == System.Convert.ToInt32(jwt.Subject))).FirstOrDefault();
                        // handle requests asking for non-existing object
                        if (u != null)
                        {
                            r = true;
                        }
                    }
                }
            }
            //End
            if (r)
            {
                u.Pwd = null;
                return new JsonResult(
                    _mapper.Map<Utilisateur, UtilisateurViewModel>(u),
                    JsonSettings);
            }
            else
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
        }

        /// <summary>
        /// GET: evapi/Utilisateur/isdr
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Wether the connected user has DR link to entities.</returns>
        [HttpGet("isDR")]
        public IActionResult IsDR()
        {
            bool r = false;
            Utilisateur u = CurrentContext.ConnectedUtilisateur;
            r = (DbContext.EntiteDRs.Where(i => i.RefDR == u.RefUtilisateur).Count() > 0);
            //End
            return new JsonResult(r);
        }

        /// <summary>
        /// GET: evapi/Utilisateur/hasemail
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Wether the connected user has an e-mail address</returns>
        [HttpGet("hasEmail")]
        public IActionResult hasEmail()
        {
            bool r = false;
            r = (!string.IsNullOrWhiteSpace(CurrentContext.ConnectedUtilisateur.EMail));
            //End
            return new JsonResult(r);
        }

        /// <summary>
        /// GET: evapi/Utilisateur/hasemail
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Wether the connected user must change his password</returns>
        [HttpGet("mustmodifypassword")]
        public IActionResult MustModifyPassword()
        {
            var securite = DbContext.Securites.FirstOrDefault();
            int r = 0;
            if (securite != null)
            {
                if (securite.DelaiAvantChangementMotDePasse > 0)
                {
                    if ((CurrentContext.ConnectedUtilisateur.PwdDModif ?? DateTime.Now).AddDays(securite.DelaiAvantChangementMotDePasse) < DateTime.Now)
                    {
                        r = CurrentContext.ConnectedUtilisateur.RefUtilisateur;
                    }
                }
            }
            //End
            return new JsonResult(r);
        }

        /// <summary>
        /// GET: evapi/utilisateur/setfilterdr
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Set FilterDR value.</returns>
        [HttpGet("setFilterDR")]
        public IActionResult SetFilterDR()
        {
            bool val = (Request.Headers.ContainsKey("val") ? (Request.Headers["val"] == "true" ? true : false) : false);
            Context ctx = CurrentContext;
            ctx.filterDR = val;
            HttpContext.Session.SetObjectAsJson("CurrentContext", ctx);
            //End
            return new JsonResult(val);
        }

        /// <summary>
        /// GET: evapi/utilisateur/setfilterglobalactif
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Set FilterGlobalActif value.</returns>
        [HttpGet("setFilterGlobalActif")]
        public IActionResult SetFilterGlobalActif()
        {
            bool val = (Request.Headers.ContainsKey("val") ? (Request.Headers["val"] == "true" ? true : false) : false);
            Context ctx = CurrentContext;
            ctx.filterGlobalActif = val;
            HttpContext.Session.SetObjectAsJson("CurrentContext", ctx);
            //End
            return new JsonResult(val);
        }

        /// <summary>
        /// Check if password reset is authorized
        /// </summary>
        /// <param name="model">The string value of the link</param>
        [HttpPost("checkresetpassword")]
        public IActionResult CheckResetPassword([FromBody] StringViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            ResetPasswordViewModel rPVM = new() { cultureName = "fr-FR", error = true };
            Lien l = new();
            string valeur = CryptoUtils.DecryptAesHexa(model.val, Configuration["AppParameters:k"], Configuration["AppParameters:v"]);
            string[] dec = valeur.Split('-');
            if (dec.Length == 4)
            {

                l = DbContext.Liens.Where(e => e.RefLien == System.Convert.ToInt32(dec[0]) && e.RefUtilisateur == System.Convert.ToInt32(dec[1])).FirstOrDefault();
            }
            //Retrieve the entity to edit. Send error if applicable
            if (!(l.RefLien > 0 && l.DCreation.AddHours((double)Utils.Utils.GetParametre(6, DbContext).ValeurNumerique) > DateTime.Now && l.DUtilisation == null))
            {
                Context ctx = new(DbContext);
                ctx.ChangeCulture(new CultureInfo("fr-FR"));
                if (dec[2] + "-" + dec[3] == "en-GB")
                {
                    ctx.ChangeCulture(new CultureInfo("en-GB"));
                }
                ctx.CulturedRessources = new CulturedRessources(ctx.CurrentCulture, DbContext);
                if (l.RefLien > 0)
                {
                    //rPVM = new ResetPasswordViewModel() { cultureName = dec[2] + "-" + dec[3], error = true };
                    return Conflict(new ConflictError(ctx.CulturedRessources.GetTextRessource(1099), ctx.CurrentCulture.Name));
                }
            }
            else
            {
                rPVM = new ResetPasswordViewModel() { cultureName = dec[2] + "-" + dec[3], error = false };
            }
            return new JsonResult(rPVM);
        }

        /// <summary>
        /// Send e-mail for password création/initialisation
        /// </summary>
        /// <param name="model">The UtilisateurViewModel containing the data to update</param>
        [HttpPost("sendemailpassword")]
        public IActionResult SendEmailPassword([FromBody] UtilisateurViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Utilisateur utilisateur = null;
            //Retrieve or create the entity to edit
            if (model.RefUtilisateur > 0)
            {
                utilisateur = DbContext.Utilisateurs
                        .Where(q => q.RefUtilisateur == model.RefUtilisateur && q.Actif == true)
                        .FirstOrDefault();
            }
            // handle requests asking for non-existing Utilisateur
            if (utilisateur == null || string.IsNullOrWhiteSpace(utilisateur.EMail))
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            Context ctx = new(DbContext);
            if (Request.Headers["cultureName"] == "en-GB")
            {
                ctx.ChangeCulture(new CultureInfo("en-GB"));
            }
            else
            {
                ctx.ChangeCulture(new CultureInfo("fr-FR"));
            }
            ctx.CulturedRessources = new CulturedRessources(ctx.CurrentCulture, DbContext);
            if (utilisateur.PwdHash == null)
            {
                EmailUtils.SendEmailPasswordManagement("Creation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, ctx, Configuration);
            }
            else
            {
                EmailUtils.SendEmailPasswordManagement("Reset", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, ctx, Configuration);
            }
            // return the updated MotifAnomalieTransporteur to the client.
            return new JsonResult(
                _mapper.Map<Utilisateur, UtilisateurViewModel>(utilisateur),
                JsonSettings);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="model">The UtilisateurViewModel containing the data to update</param>
        [HttpPost("resetpassword")]
        public IActionResult ResetPassword([FromBody] UtilisateurViewModel model)
        {
            Context ctx = new(DbContext);
            ctx.ChangeCulture(new CultureInfo("fr-FR"));
            ctx.CulturedRessources = new CulturedRessources(ctx.CurrentCulture, DbContext);
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(ctx.CulturedRessources.GetTextRessource(711)));

            Utilisateur utilisateur = new();
            Lien l = new();
            string valeur = CryptoUtils.DecryptAesHexa(model.Login, Configuration["AppParameters:k"], Configuration["AppParameters:v"]);
            string[] dec = valeur.Split('-');
            if (dec.Length == 4)
            {
                if (dec[2] + "-" + dec[3] == "en-GB")
                {
                    ctx.ChangeCulture(new CultureInfo("en-GB"));
                    ctx.CulturedRessources = new CulturedRessources(ctx.CurrentCulture, DbContext);
                }
                try
                {
                    l = DbContext.Liens.Find(System.Convert.ToInt32(dec[0]));
                    utilisateur = DbContext.Utilisateurs.Find(System.Convert.ToInt32(dec[1]));
                }
                catch { }
            }
            //Retrieve the entity to edit
            if (!(utilisateur.RefUtilisateur > 0 && l.RefLien > 0
                && l.DCreation.AddHours(1) > DateTime.Now && l.DUtilisation == null))
            {
                return BadRequest(new BadRequestError(ctx.CulturedRessources.GetTextRessource(711)));
            }
            //Check if creation or reset
            string operationType = "CreationConfirmation";
            if (utilisateur.PwdHash != null) { operationType = "ResetConfirmation"; }
            //Check new password, exit if same as existing
            if (!string.IsNullOrWhiteSpace(model.Pwd))
            {
                var refUtilisateur = new SqlParameter("@refUtilisateur", utilisateur.RefUtilisateur);
                var pwd = new SqlParameter("@pwd", model.Pwd);
                var msg = new SqlParameter("@msg", SqlDbType.NVarChar, 50);
                msg.Direction = ParameterDirection.Output;
                DbContext.Database.ExecuteSqlRaw(
                        "dbo.CreateEncryptedPwd @refUtilisateur,@pwd, @msg out",
                        refUtilisateur,
                        pwd,
                        msg);
                if (msg.Value.ToString() == "same")
                {
                    return Conflict(new ConflictError(ctx.CulturedRessources.GetTextRessource(1103)));
                }
                else
                {
                    DbContext.Entry(utilisateur).Reload();
                }
            }
            //Update link
            l.DUtilisation = DateTime.Now;
            //End
            // persist the changes into the Database.
            DbContext.SaveChanges();
            //Send notification e-mail
            EmailUtils.SendEmailPasswordManagement(operationType, utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, ctx, Configuration);
            // return the updated Utilisateur to the client.
            return new JsonResult(
                _mapper.Map<Utilisateur, UtilisateurViewModel>(utilisateur),
                JsonSettings);
        }
        #endregion
        #region Services

        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref Utilisateur dataModel, UtilisateurViewModel viewModel, ref bool assignmentMaitre)
        {
            int refUtilisateur = dataModel.RefUtilisateur;
            //Manage relations to UtilisateurMaitre
            //On deactivation
            if (viewModel.Actif == false && dataModel.Actif)
            {
                //Deactivate related contactAdresse if applicable
                if (dataModel.ContactAdresse != null)
                {
                    dataModel.ContactAdresse.Actif = false;
                    dataModel.ContactAdresse.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                }
                //Remove link to UtilisateurMaitre if applicable
                if (dataModel.RefUtilisateurMaitre != null) { dataModel.RefUtilisateurMaitre = null; }
                //Remove link to slaves if applicable
                foreach (var u in DbContext.Utilisateurs.Where(e => e.RefUtilisateurMaitre == refUtilisateur).ToList())
                {
                    u.RefUtilisateurMaitre = null;
                }
            }
            //If new Maitre, record who changed it
            else if (viewModel.RefUtilisateurMaitre != dataModel.RefUtilisateurMaitre
                && viewModel.RefUtilisateurMaitre != dataModel.RefUtilisateur
                && viewModel.RefUtilisateurMaitre != null)
            {
                dataModel.RefUtilisateurMaitre = viewModel.RefUtilisateurMaitre;
                dataModel.RefUtilisateurAffectationMaitre = CurrentContext.RefUtilisateur;
                dataModel.DAffectationMaitre = DateTime.Now;
                assignmentMaitre = true;
            }
            //Remove maitre if applicable
            if (viewModel.RefUtilisateurMaitre == null) { dataModel.RefUtilisateurMaitre = null; }
            //Update principal data
            dataModel.Actif = viewModel.Actif ?? false;
            dataModel.Nom = Utils.Utils.SetEmptyStringToNull(viewModel.Nom);
            dataModel.Login = (string.IsNullOrWhiteSpace(viewModel.EMail) ? Utils.Utils.SetEmptyStringToNull(viewModel.Login) : null);
            dataModel.EMail = Utils.Utils.SetEmptyStringToNull(viewModel.EMail);
            dataModel.Tel = Utils.Utils.SetEmptyStringToNull(viewModel.Tel);
            dataModel.TelMobile = Utils.Utils.SetEmptyStringToNull(viewModel.TelMobile);
            dataModel.Fax = Utils.Utils.SetEmptyStringToNull(viewModel.Fax);
            dataModel.HabilitationAdministration = Utils.Utils.SetEmptyStringToNull(viewModel.HabilitationAdministration);
            dataModel.HabilitationAnnuaire = Utils.Utils.SetEmptyStringToNull(viewModel.HabilitationAnnuaire);
            dataModel.HabilitationLogistique = Utils.Utils.SetEmptyStringToNull(viewModel.HabilitationLogistique);
            dataModel.HabilitationMessagerie = Utils.Utils.SetEmptyStringToNull(viewModel.HabilitationMessagerie);
            dataModel.HabilitationModuleCentreDeTri = Utils.Utils.SetEmptyStringToNull(viewModel.HabilitationModuleCentreDeTri);
            dataModel.HabilitationModuleCollectivite = Utils.Utils.SetEmptyStringToNull(viewModel.HabilitationModuleCollectivite);
            dataModel.HabilitationQualite = Utils.Utils.SetEmptyStringToNull(viewModel.HabilitationQualite);
            dataModel.HabilitationModulePrestataire = Utils.Utils.SetEmptyStringToNull(viewModel.HabilitationModulePrestataire);
            dataModel.RefCentreDeTri = viewModel.CentreDeTri?.RefEntite;
            dataModel.RefClient = viewModel.Client?.RefEntite;
            dataModel.RefCollectivite = viewModel.Collectivite?.RefEntite;
            dataModel.RefTransporteur = viewModel.Transporteur?.RefEntite;
            dataModel.RefPrestataire = viewModel.Prestataire?.RefEntite;
            dataModel.PiedEmail = Utils.Utils.SetEmptyStringToNull(viewModel.PiedEmail);
            dataModel.RefContactAdresse = viewModel.ContactAdresse?.RefContactAdresse;
        }

        #endregion Services
    }
}
