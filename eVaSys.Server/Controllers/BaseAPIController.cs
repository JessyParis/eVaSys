using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/06/2018
/// ----------------------------------------------------------------------------------------------------- 
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class BaseApiController : Controller
    {
        #region Constructor
        public BaseApiController(ApplicationDbContext context, IConfiguration configuration)
        {
            // Instantiate the ApplicationDbContext through DI
            DbContext = context;
            Configuration = configuration;

            // Instantiate a single JsonSerializerSettings object
            // that can be reused multiple times.
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
            };

        }
        #endregion
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext ctx)
        {
            bool authenticated = false;
            bool authorized = false;
            //Security check
            if (Request.Path.Value != "/evapi/token/auth"
                && Request.Path.Value != "/evapi/token/logout"
                && Request.Path.Value != "/evapi/utils/getappparametres"
                && Request.Path.Value != "/evapi/utils/getculturedressources"
                && Request.Path.Value != "/evapi/utils/getappresources"
                && Request.Path.Value != "/evapi/utils/getenvmodules"
                && Request.Path.Value != "/evapi/utils/getenvmenus"
                && Request.Path.Value != "/evapi/utils/getenvcomponents"
                && Request.Path.Value != "/evapi/utils/getenvdatacolumns"
                && Request.Path.Value != "/evapi/utils/getenvcommandefournisseurstatuts"
                && Request.Path.Value != "/evapi/utils/getnonconformiteetapetype"
                && Request.Path.Value != "/evapi/utils/setculture"
                && Request.Path.Value != "/evapi/utils/getenvironment"
                && Request.Path.Value != "/evapi/utils/passwordlost"
                && Request.Path.Value != "/evapi/utilisateur/getconnected"
                && Request.Path.Value != "/evapi/utilisateur/checkresetpassword"
                && Request.Path.Value != "/evapi/utilisateur/resetpassword"
                && !Request.Path.Value.StartsWith("/error")
                )
            {
                if (HttpContext.Request.Headers["eValorplastSecurity"] != "")
                {
                    JwtSecurityToken jwt = new(HttpContext.Request.Headers["eValorplastSecurity"]);
                    //Check token expiration
                    if (jwt.ValidTo > DateTime.UtcNow)
                    {
                        //Get token user 
                        if (jwt.Subject != "")
                        {
                            var u = DbContext.Utilisateurs.Where(i => (i.RefUtilisateur == System.Convert.ToInt32(jwt.Subject))).FirstOrDefault();
                            if (u != null)
                            {
                                //Check if session user equals token user
                                if (u.RefUtilisateur == CurrentContext?.RefUtilisateur)
                                {
                                    authenticated = true;
                                    //Rights management
                                    authorized = Rights.Authorized(Request.Path.Value, Request.Method, u, Configuration);
                                }
                                else
                                {
                                    Utils.Utils.DebugPrint("Session user cannot be found: " + CurrentContext?.RefUtilisateur ?? "NC", Configuration["Data:DefaultConnection:ConnectionString"]);
                                }
                            }
                            else
                            {
                                Utils.Utils.DebugPrint("JSon user cannot be found: " + jwt.Subject, Configuration["Data:DefaultConnection:ConnectionString"]);
                            }
                        }
                        else
                        {
                            Utils.Utils.DebugPrint("JSon user undefined: ", Configuration["Data:DefaultConnection:ConnectionString"]);
                        }
                    }
                    else
                    {
                        Utils.Utils.DebugPrint("JSon Token timed out", Configuration["Data:DefaultConnection:ConnectionString"]);
                    }
                }
                else
                {
                    Utils.Utils.DebugPrint("JSon Token not present", Configuration["Data:DefaultConnection:ConnectionString"]);
                }
            }
            else
            { authorized = true; authenticated = true; }
            //If authenticated then go on, else, retruns error 
            if (authenticated && authorized)
            {
                base.OnActionExecuting(ctx);
            }
            else
            {
                if (!authenticated)
                {
                    ctx.Result = StatusCode((int)HttpStatusCode.Forbidden, new ForbiddenError(CurrentContext?.CulturedRessources.GetTextRessource(723) ?? "Vous avez été déconnecté suite à une longue période d'inactivité. Veuillez vous reconnecter. \n You have been disconnected, please login again."));
                }
                else    //Not authorized
                {
                    ctx.Result = Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
                }
            }
        }
        #region Shared Properties
        protected ApplicationDbContext DbContext { get; private set; }
        protected JsonSerializerSettings JsonSettings { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        //Instance of session context
        private Context _currentContext;
        public Context CurrentContext
        {
            get
            {
                if (_currentContext == null)
                {
                    if (HttpContext.Session.GetString("CurrentContext") == null)
                    {
                        ////Create and store a new context
                        //Context ctx = new Context(DbContext);
                        //ctx.CulturedRessources = new CulturedRessources(ctx.CurrentCulture, ctx.DbContext);
                        //HttpContext.Session.SetObjectAsJson("CurrentContext", ctx);
                        //return ctx;
                        return null;
                    }
                    else
                    {
                        _currentContext = HttpContext.Session.GetObjectFromJson<Context>("CurrentContext");
                        _currentContext.DbContext = DbContext;
                        _currentContext.CulturedRessources = new CulturedRessources(_currentContext.CurrentCulture, _currentContext.DbContext);
                        _currentContext.CreateEnvDataColumns();
                        _currentContext.CreateEnvModules();
                        _currentContext.CreateEnvComponents();
                        _currentContext.CreateEnvActions();
                        _currentContext.CreateEnvMenus();
                        _currentContext.CreateEnvCommandeFournisseurStatuts();
                        return _currentContext;
                    }
                }
                else
                {
                    return _currentContext;
                }
            }
            set
            {
                //HttpContext.Session.SetObjectAsJson("CurrentContext", value);
            }
        }
        #endregion
    }
}
