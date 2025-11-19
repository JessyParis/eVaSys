using eVaSys.APIUtils;
/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/06/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class TokenController : BaseApiController
    {
        #region Private Members
        #endregion Private Members

        #region Constructor
        public TokenController(
            ApplicationDbContext context,
            IConfiguration configuration
            )
            : base(context, configuration) { }
        #endregion

        [HttpPost("auth")]
        public IActionResult Auth([FromBody] TokenRequestViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext?.CulturedRessources.GetTextRessource(711) ?? "Bad request. Requête mal formattée"));
            switch (model.grantType)
            {
                case "password":
                    //Reset session
                    return GetToken(model, false);
                case "profil":
                    //Reset session
                    return GetToken(model, true);
                case "refresh_token":
                    return RefreshToken(model, HttpContext);
                default:
                    // not supported - return a HTTP 401 (Unauthorized)
                    return BadRequest(new BadRequestError(CurrentContext?.CulturedRessources.GetTextRessource(711) ?? "Bad request.Requête mal formattée"));
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return new AcceptedResult();
        }

        private IActionResult GetToken(TokenRequestViewModel model, bool changeProfil)
        {
            //            try
            {
                var response = new TokenResponseViewModel();

                // retrieve the relevant FORM data
                string identifiant = model.login;
                string motdepasse = model.pwd;
                string cultureName = model.cultureName;
                Utilisateur utilisateur = null;
                int refUtilisateur = 0;
                int refUtilisateurMaitreConnected = 0;
                int refNewProfilToConnect = 0; //New profil
                //Connect user with login and password
                if (!changeProfil)
                {
                    // check if there's an user with the given credentials
                    //Prefer e-mail to login
                    refUtilisateur = Utils.Utils.GetRefUtilisateurByCredentials(identifiant, motdepasse, DbContext.Database.GetDbConnection());
                    utilisateur = DbContext.Utilisateurs.Where(i => i.RefUtilisateur == refUtilisateur).FirstOrDefault();
                    if (utilisateur == null
                        || (utilisateur != null && !string.IsNullOrWhiteSpace(utilisateur.EMail) && identifiant == utilisateur.Login)
                        || (utilisateur != null && utilisateur.RefUtilisateurMaitre != null)
                        )
                    {
                        response.token = "";
                        response.expiration = 0;
                        // if user has an e-mail but try to connect with login, then return an error
                        if (utilisateur != null && !string.IsNullOrWhiteSpace(utilisateur.EMail) && identifiant == utilisateur.Login)
                        {
                            response.error = "UseEmail";
                        }
                        // if user has a Maitre, then return an error
                        if (utilisateur != null && utilisateur.RefUtilisateurMaitre != null)
                        {
                            response.error = "UseMaitre";
                        }
                    }
                }
                //Change profil
                else
                {
                    //Prepare error in case processing of new profil fails
                    response.token = "";
                    response.expiration = 0;
                    //Try to get profil id
                    int.TryParse(identifiant, out refNewProfilToConnect);
                    //Get and check id of the originaly Maitre connected user
                    if (CurrentContext?.RefUtilisateurMaitre > 0 && CurrentContext?.ConnectedUtilisateur?.RefUtilisateur > 0)
                    {
                        if (CurrentContext?.RefUtilisateurMaitre != CurrentContext?.ConnectedUtilisateur?.RefUtilisateur)
                        {
                            //Check the relation between users
                            var nb = DbContext.Utilisateurs
                                .Where(e => e.RefUtilisateur == CurrentContext.ConnectedUtilisateur.RefUtilisateur
                                    && e.RefUtilisateurMaitre == CurrentContext.RefUtilisateurMaitre
                                    && e.Actif).Count();
                            if (nb > 0)
                            {
                                refUtilisateurMaitreConnected = CurrentContext.RefUtilisateurMaitre;
                            }
                        }
                        else
                        {
                            refUtilisateurMaitreConnected = CurrentContext.RefUtilisateurMaitre;
                        }
                    }
                    if (refUtilisateurMaitreConnected > 0)
                    {
                        //Get user to connect
                        int.TryParse(identifiant, out refNewProfilToConnect);
                        if (refNewProfilToConnect != 0)
                        {
                            //Check if there's an user with id that is link to actual connected user
                            //If Maitre Profil
                            if (refNewProfilToConnect == refUtilisateurMaitreConnected)
                            {
                                utilisateur = DbContext.Utilisateurs
                                    .Where(i => i.RefUtilisateur == refNewProfilToConnect && i.RefUtilisateurMaitre == null && i.Actif).FirstOrDefault();
                            }
                            //Else
                            else
                            {
                                utilisateur = DbContext.Utilisateurs
                                    .Where(i => i.RefUtilisateur == refNewProfilToConnect && i.RefUtilisateurMaitre == refUtilisateurMaitreConnected && i.Actif).FirstOrDefault();
                            }
                            if (utilisateur != null) { refUtilisateur = utilisateur.RefUtilisateur; }
                        }
                    }
                }
                //Process connexion
                if (utilisateur != null && refUtilisateur != 0)
                {
                    // username & password matches, or new profil validated: create and return the Jwt token.

                    DateTime now = DateTime.UtcNow;

                    // add the registered claims for JWT (RFC7519).
                    // For more info, see https://tools.ietf.org/html/rfc7519#section-4.1
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, utilisateur.RefUtilisateur.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
                        // TODO: add additional claims here
                    };

                    var tokenExpirationMins =
                        Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");
                    var issuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]));

                    var token = new JwtSecurityToken(
                        issuer: Configuration["Auth:Jwt:Issuer"],
                        audience: Configuration["Auth:Jwt:Audience"],
                        claims: claims,
                        notBefore: now,
                        expires: now.Add(TimeSpan.FromMinutes(tokenExpirationMins)),
                        signingCredentials: new SigningCredentials(
                            issuerSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                    var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
                    //Record the new login or update the last login of firt UtilisateurMaitre connection
                    var log = new Log();
                    if (!changeProfil)
                    {
                        log = new Log()
                        {
                            RefUtilisateur = utilisateur.RefUtilisateur,
                            DLogin = DateTime.Now
                        };
                        DbContext.Logs.Add(log);
                    }
                    else
                    {
                        //If first Profil change, we modify the original login
                        log = DbContext.Logs.Where(e => e.RefUtilisateur == CurrentContext.RefUtilisateurMaitre && e.RefUtilisateurMaitre == null)
                            .OrderByDescending(o => o.RefLog)
                            .FirstOrDefault();
                        if (log != null)
                        {
                            log.RefUtilisateur = utilisateur.RefUtilisateur;
                            log.RefUtilisateurMaitre = CurrentContext.RefUtilisateurMaitre;
                        }
                        else
                        {
                            //If new profil change, we record a new log
                            log = new Log()
                            {
                                RefUtilisateur = utilisateur.RefUtilisateur,
                                RefUtilisateurMaitre = CurrentContext.RefUtilisateurMaitre,
                                DLogin = DateTime.Now
                            };
                            DbContext.Logs.Add(log);
                        }
                    }
                    DbContext.SaveChanges();
                    //Initialize session
                    HttpContext.Session.Clear();
                    Context ctx = new(DbContext);
                    ctx.RefUtilisateur = utilisateur.RefUtilisateur;
                    ctx.ChangeCulture(new CultureInfo(cultureName));
                    if (!changeProfil)
                    {
                        ctx.RefUtilisateurMaitre = utilisateur.RefUtilisateur;
                    }
                    else
                    {
                        ctx.RefUtilisateurMaitre = refUtilisateurMaitreConnected;
                    }
                    HttpContext.Session.SetObjectAsJson("CurrentContext", ctx);
                    // build & return the response
                    response.token = encodedToken;
                    response.expiration = tokenExpirationMins;

                    //************************TESTS*****************************
                    if (Configuration["AppParameters:EmailDev"] == "true")
                    {
                        ////E-mails création/récupération mot de passe
                        //CurrentContext.ChangeCulture(new CultureInfo("fr-FR"));
                        //EmailUtils.SendEmailPasswordManagement("Creation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //EmailUtils.SendEmailPasswordManagement("Reset", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //EmailUtils.SendEmailPasswordManagement("CreationConfirmation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //EmailUtils.SendEmailPasswordManagement("ResetConfirmation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //CurrentContext.ChangeCulture(new CultureInfo("en-GB"));
                        //EmailUtils.SendEmailPasswordManagement("Creation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //EmailUtils.SendEmailPasswordManagement("Reset", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //EmailUtils.SendEmailPasswordManagement("CreationConfirmation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //EmailUtils.SendEmailPasswordManagement("ResetConfirmation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //utilisateur.CentreDeTri = DbContext.Entites.Find(40);
                        //CurrentContext.ChangeCulture(new CultureInfo("fr-FR"));
                        //EmailUtils.SendEmailPasswordManagement("Creation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //EmailUtils.SendEmailPasswordManagement("Reset", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //CurrentContext.ChangeCulture(new CultureInfo("en-GB"));
                        //EmailUtils.SendEmailPasswordManagement("Creation", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        //EmailUtils.SendEmailPasswordManagement("Reset", utilisateur, CurrentContext == null ? 1 : CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                    }
                    //************************TESTS*****************************

                }
                return Json(response);
            }
            //catch (Exception ex)
            //{
            //    return new StatusCodeResult(500);
            //}
        }
        private IActionResult RefreshToken(TokenRequestViewModel model, HttpContext httpContext)
        {
            bool ok = false;
            var response = new TokenResponseViewModel();
            try
            {
                if (HttpContext.Request.Headers["eValorplastSecurity"] != "")
                {
                    JwtSecurityToken jwt = new(HttpContext.Request.Headers["eValorplastSecurity"]);
                    var tokenExpirationMins = Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");
                    //Check user
                    if (jwt.Subject != "")
                    {
                        var u = DbContext.Utilisateurs.Where(i => i.RefUtilisateur == System.Convert.ToInt32(jwt.Subject) && i.Actif).FirstOrDefault();
                        if (u != null)
                        {
                            // user exists and ASP NET session is still valid for him: create and return the Jwt token.
                            if (CurrentContext?.RefUtilisateur == System.Convert.ToInt32(jwt.Subject))
                            {
                                DateTime now = DateTime.UtcNow;

                                // add the registered claims for JWT (RFC7519).
                                // For more info, see https://tools.ietf.org/html/rfc7519#section-4.1
                                var claims = new[] {
                                    new Claim(JwtRegisteredClaimNames.Sub, u.RefUtilisateur.ToString()),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
                                };

                                var issuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]));

                                var token = new JwtSecurityToken(
                                    issuer: Configuration["Auth:Jwt:Issuer"],
                                    audience: Configuration["Auth:Jwt:Audience"],
                                    claims: claims,
                                    notBefore: now,
                                    expires: now.Add(TimeSpan.FromMinutes(tokenExpirationMins)),
                                    signingCredentials: new SigningCredentials(
                                        issuerSigningKey, SecurityAlgorithms.HmacSha256)
                                );
                                var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

                                // build & return the response
                                response = new TokenResponseViewModel()
                                {
                                    token = encodedToken,
                                    expiration = tokenExpirationMins
                                };
                                //Validate
                                ok = true;
                            }
                        }
                    }
                }
                if (ok) { return Json(response); }
                else { return StatusCode((int)HttpStatusCode.Forbidden, new ForbiddenError(CurrentContext?.CulturedRessources.GetTextRessource(723) ?? "Vous avez été déconnecté suite à une longue période d'inactivité. Veuillez vous reconnecter. \n You have been disconnected, please login again.")); }
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new ForbiddenError(CurrentContext?.CulturedRessources.GetTextRessource(345) ?? "Erreur inconnue. Unknown error."));
            }
        }
    }
}
