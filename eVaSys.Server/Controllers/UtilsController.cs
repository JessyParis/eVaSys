/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/01/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Resources;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class UtilsController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public UtilsController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor
        #region Attribute-based Routing
        /// <summary>
        /// GET: api/utils/setculture
        /// ROUTING TYPE: attribute-based
        /// Set the default culture for localized resources
        /// </summary>
        /// <returns>OK</returns>
        [HttpGet("SetCulture")]
        public IActionResult SetCulture()
        {
            string cultureName = Request.Headers["cultureName"].ToString();

            CultureInfo culture = new(cultureName);
            Context ctx = CurrentContext;
            ctx.ChangeCulture(new CultureInfo(cultureName));
            HttpContext.Session.SetObjectAsJson("CurrentContext", ctx);
            return Ok();
        }
        /// <summary>
        /// GET: api/utils/getressources
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all localized multilingual ressources</returns>
        [HttpGet("GetCulturedRessources")]
        public IActionResult GetCulturedRessources()
        {
            string cultureName = Request.Headers["cultureName"].ToString();
            CulturedRessources cR = new(new CultureInfo(cultureName), DbContext);
            var dT = new DataTable();
            dT.Columns.Add("refRessource", typeof(int));
            dT.Columns.Add("textRessource", typeof(string));
            dT.Columns.Add("textJavascriptRessource", typeof(string));
            dT.Columns.Add("hTMLRessource", typeof(string));
            dT.Columns.Add("hTMLButtonRessource", typeof(string));
            foreach (Ressource rsc in DbContext.Ressources)
            {
                object[] rowVals = new object[5];
                rowVals[0] = rsc.RefRessource;
                rowVals[1] = cR.GetTextRessource(rsc.RefRessource);
                rowVals[2] = cR.GetTextJavascriptRessource(rsc.RefRessource);
                rowVals[3] = cR.GetHTMLRessource(rsc.RefRessource);
                rowVals[4] = cR.GetHTMLButtonRessource(rsc.RefRessource);
                dT.Rows.Add(rowVals);
            }
            return new JsonResult(dT);
        }
        /// <summary>
        /// GET: api/utils/getressources
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all localized multilingual ressources</returns>
        [HttpGet("getappresources")]
        public IActionResult GeAppResources()
        {
            string resxFile = @".\Resource.resx";
            var dT = new DataTable();
            dT.Columns.Add("name", typeof(string));
            dT.Columns.Add("value", typeof(string));
            using (ResXResourceReader resxReader = new ResXResourceReader(resxFile))
            {
                foreach (DictionaryEntry entry in resxReader)
                {
                    if (entry.Value.GetType() == typeof(string))
                    {
                        object[] rowVals = new object[2];
                        rowVals[0] = (string)entry.Key;
                        rowVals[1] = (string)entry.Value;
                        dT.Rows.Add(rowVals);
                    }
                }
            }
            return new JsonResult(dT);
        }
        /// <summary>
        /// GET: api/utils/getenvmodules
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all modules</returns>
        [HttpGet("GetEnvModules")]
        public IActionResult GetEnvModules()
        {
            //Init
            string cultureName = Request.Headers["cultureName"].ToString();
            Context ctx = new(DbContext);
            ctx.ChangeCulture(new CultureInfo(cultureName));
            var dT = new DataTable();
            //Process
            dT.Columns.Add("name", typeof(string));
            dT.Columns.Add("refRessource", typeof(string));
            dT.Columns.Add("culturedCaption", typeof(string));
            dT.Columns.Add("cmt", typeof(string));
            foreach (EnvModule mod in ctx.EnvModules.Values)
            {
                object[] rowVals = new object[4];
                rowVals[0] = mod.Name;
                rowVals[1] = mod.RefRessource.ToString();
                rowVals[2] = mod.CulturedCaption;
                rowVals[3] = mod.Cmt;
                dT.Rows.Add(rowVals);
            }
            return new JsonResult(dT, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getenvmenus
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all menus</returns>
        [HttpGet("GetEnvMenus")]
        public IActionResult GetEnvMenus()
        {
            //Init
            string cultureName = Request.Headers["cultureName"].ToString();
            Context ctx = new(DbContext);
            ctx.ChangeCulture(new CultureInfo(cultureName));
            var dT = new DataTable();
            //Process
            dT.Columns.Add("name", typeof(string));
            dT.Columns.Add("refRessource", typeof(string));
            dT.Columns.Add("culturedCaption", typeof(string));
            dT.Columns.Add("moduleName", typeof(string));
            dT.Columns.Add("cmt", typeof(string));
            dT.Columns.Add("gridField", typeof(string));
            dT.Columns.Add("gridSort", typeof(bool));
            dT.Columns.Add("gridGoto", typeof(string));
            dT.Columns.Add("gridRef", typeof(string));
            foreach (EnvMenu mod in ctx.EnvMenus.Values)
            {
                object[] rowVals = new object[9];
                rowVals[0] = mod.Name;
                rowVals[1] = mod.RefRessource.ToString();
                rowVals[2] = mod.CulturedCaption;
                rowVals[3] = mod.Modules[0]?.Name;
                rowVals[4] = mod.Cmt;
                rowVals[5] = mod.GridField;
                rowVals[6] = mod.GridSort;
                rowVals[7] = mod.GridGoto;
                rowVals[8] = mod.GridRef;
                dT.Rows.Add(rowVals);
            }
            return new JsonResult(dT, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getenvcomponents
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all components</returns>
        [HttpGet("GetEnvComponents")]
        public IActionResult GetEnvComponents()
        {
            //Init
            string cultureName = Request.Headers["cultureName"].ToString();
            Context ctx = new(DbContext);
            ctx.ChangeCulture(new CultureInfo(cultureName));
            var dT = new DataTable();
            //Process
            dT.Columns.Add("name", typeof(string));
            dT.Columns.Add("url", typeof(string));
            dT.Columns.Add("ref", typeof(string));
            dT.Columns.Add("ressLibel", typeof(int));
            dT.Columns.Add("ressBeforeDel", typeof(int));
            dT.Columns.Add("ressAfterDel", typeof(int));
            dT.Columns.Add("moments", typeof(string));
            dT.Columns.Add("formClass", typeof(string));
            dT.Columns.Add("getId0", typeof(bool));
            dT.Columns.Add("createModifTooltipVisible", typeof(bool));
            dT.Columns.Add("hasHelp", typeof(bool));
            dT.Columns.Add("libelle", typeof(string));
            foreach (EnvComponent cmp in ctx.EnvComponents.Values)
            {
                object[] rowVals = new object[12];
                rowVals[0] = cmp.Name;
                rowVals[1] = cmp.URL;
                rowVals[2] = cmp.Ref;
                rowVals[3] = cmp.RessLibel;
                rowVals[4] = cmp.RessBeforeDel;
                rowVals[5] = cmp.RessAfterDel;
                rowVals[6] = cmp.Moments;
                rowVals[7] = cmp.FormClass;
                rowVals[8] = cmp.GetId0;
                rowVals[9] = cmp.CreateModifTooltipVisible;
                rowVals[10] = cmp.HasHelp;
                rowVals[11] = cmp.Libelle;
                dT.Rows.Add(rowVals);
            }
            return new JsonResult(dT, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getenvdatacolumns
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all data columns</returns>
        [HttpGet("GetEnvDataColumns")]
        public IActionResult GetEnvDataColumns()
        {
            //Init
            string cultureName = Request.Headers["cultureName"].ToString();
            Context ctx = new(DbContext);
            ctx.ChangeCulture(new CultureInfo(cultureName));
            var dT = new DataTable();
            //Process
            dT.Columns.Add("name", typeof(string));
            dT.Columns.Add("refRessource", typeof(string));
            dT.Columns.Add("culturedCaption", typeof(string));
            dT.Columns.Add("field", typeof(string));
            dT.Columns.Add("fullField", typeof(string));
            dT.Columns.Add("cmt", typeof(string));
            dT.Columns.Add("dataType", typeof(string));
            foreach (EnvDataColumn mod in ctx.EnvDataColumns.Values)
            {
                object[] rowVals = new object[7];
                rowVals[0] = mod.Name;
                rowVals[1] = mod.RefRessource.ToString();
                rowVals[2] = mod.CulturedCaption;
                rowVals[3] = mod.Field;
                rowVals[4] = mod.FullField;
                rowVals[5] = mod.Cmt;
                rowVals[6] = mod.DataType;
                dT.Rows.Add(rowVals);
            }
            return new JsonResult(dT, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getenvironment
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>The type of environment and database (dev or prod)</returns>
        [HttpGet("GetEnvironment")]
        public IActionResult GetEnvironment()
        {
            //Init
            bool debugMode = Configuration.GetValue<bool>("AppParameters:DebugMode");
            string r = (debugMode ? "Env. dév." : "");
            bool bDDProd = (Configuration.GetValue<string>("Data:DefaultConnection:ConnectionString").StartsWith("data source=EVAPROD;initial catalog=eValorplast;")
                || Configuration.GetValue<string>("Data:DefaultConnection:ConnectionString").StartsWith("data source=EVASECOURS;initial catalog=eValorplast;"));
            bool bDDRestore = Configuration.GetValue<string>("Data:DefaultConnection:ConnectionString").StartsWith("data source=EVASECOURS;initial catalog=eValorplastRestore;");
            bool bDDRepMens = Configuration.GetValue<string>("Data:DefaultConnection:ConnectionString").StartsWith("data source=EVASECOURS;initial catalog=eValorplastRepMens;");
            if (!bDDProd)
            {
                if (bDDRestore) { r += " BDD restauration"; }
                else if (bDDRepMens) { r += " BDD avant éclatement répartitions"; }
                else { r += " BDD dév."; }
            }
            return new JsonResult(r, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getenvcommandefournisseurstatut
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all EnvCommandeFournisseurStatut</returns>
        [HttpGet("GetEnvCommandeFournisseurStatuts")]
        public IActionResult GetEnvCommandeFournisseurStatuts()
        {
            //Init
            string cultureName = Request.Headers["cultureName"].ToString();
            Context ctx = new(DbContext);
            ctx.ChangeCulture(new CultureInfo(cultureName));
            var dT = new DataTable();
            //Process
            dT.Columns.Add("name", typeof(string));
            dT.Columns.Add("culturedCaption", typeof(string));
            foreach (EnvCommandeFournisseurStatut mod in ctx.EnvCommandeFournisseurStatuts.Values)
            {
                object[] rowVals = new object[2];
                rowVals[0] = mod.Name;
                rowVals[1] = mod.CulturedCaption;
                dT.Rows.Add(rowVals);
            }
            return new JsonResult(dT, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getnonconformiteetapetype
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of NonConformiteEtapeType.</returns>
        [HttpGet("GetNonConformiteEtapeType")]
        public IActionResult GetNonConformiteEtapeType()
        {
            string cultureName = Request.Headers["cultureName"].ToString();
            //Query
            System.Linq.IQueryable<eVaSys.Data.NonConformiteEtapeType> req = DbContext.NonConformiteEtapeTypes;
            //Get data
            NonConformiteEtapeType[] all;
            if (cultureName == "fr-FR")
            {
                all = req.OrderBy(el => el.Ordre).Select(p => new NonConformiteEtapeType()
                {
                    RefNonConformiteEtapeType = p.RefNonConformiteEtapeType,
                    Ordre = p.Ordre,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleFRFR,
                    Controle = p.Controle
                }).ToArray();
            }
            else
            {
                all = req.OrderBy(el => el.Ordre).Select(p => new NonConformiteEtapeType()
                {
                    RefNonConformiteEtapeType = p.RefNonConformiteEtapeType,
                    Ordre = p.Ordre,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleENGB,
                    Controle = p.Controle
                }).ToArray();
            }
            //Return Json
            return new JsonResult(
                _mapper.Map<NonConformiteEtapeType[], NonConformiteEtapeTypeViewModel[]>(all),
                JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getmonthlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all months</returns>
        [HttpGet("GetMonthList")]
        public IActionResult GetMonthList()
        {
            string dRef = Request.Headers["d"].ToString();
            DateTime d;
            var dT = new DataTable();
            int nb = 12;
            //Process
            dT.Columns.Add("d", typeof(DateTime));
            dT.Columns.Add("n", typeof(int));
            dT.Columns.Add("name", typeof(string));
            if (DateTime.TryParse(dRef, out d))
            {
                d = d.AddDays(-d.Day + 1);
                //Only months for DMoisDechargementPrevu
                d = d.AddMonths(-1);
                nb = 5;
            }
            else
            {
                d = new DateTime(1901, 1, 1, 0, 0, 0);
            }
            object[] rowVals = new object[3];
            for (int i = 0; i < nb; i++)
            {
                rowVals = new object[3];
                rowVals[0] = d.AddMonths(i);
                rowVals[1] = i + 1;
                rowVals[2] = CurrentContext.CulturedRessources.GetTextRessource(447 + d.AddMonths(i).Month - 1);
                dT.Rows.Add(rowVals);
            }
            return new JsonResult(dT, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getquarterlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all quarters</returns>
        [HttpGet("GetQuarterList")]
        public IActionResult GetQuarterList()
        {
            var dT = new DataTable();
            //Process
            dT.Columns.Add("n", typeof(int));
            dT.Columns.Add("name", typeof(string));
            object[] rowVals = new object[2];
            rowVals[0] = 1;
            rowVals[1] = CurrentContext.CulturedRessources.GetTextRessource(692);
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = 2;
            rowVals[1] = CurrentContext.CulturedRessources.GetTextRessource(693);
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = 3;
            rowVals[1] = CurrentContext.CulturedRessources.GetTextRessource(694);
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = 4;
            rowVals[1] = CurrentContext.CulturedRessources.GetTextRessource(695);
            dT.Rows.Add(rowVals);
            return new JsonResult(dT, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getyearlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all months</returns>
        [HttpGet("GetYearList")]
        public IActionResult GetYearList()
        {
            string menuName = Request.Headers["menuName"].ToString();
            string statType = Request.Headers["statType"].ToString();
            var dT = new DataTable();
            //Process
            dT.Columns.Add("y", typeof(int));
            object[] rowVals = new object[1];
            if (menuName == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString())
            {
                rowVals = new object[1];
                rowVals[0] = 2023;
                dT.Rows.Add(rowVals);
                rowVals[0] = 2022;
                dT.Rows.Add(rowVals);
                rowVals[0] = 2021;
                dT.Rows.Add(rowVals);
                rowVals[0] = 2020;
                dT.Rows.Add(rowVals);
                rowVals[0] = 2019;
                dT.Rows.Add(rowVals);
                rowVals[0] = 2018;
                dT.Rows.Add(rowVals);
            }
            else
            {
                for (int i = DateTime.Now.Year + 1; i > DateTime.Now.Year - 20; i--)
                {
                    if (!(menuName == Enumerations.MenuName.ModuleCollectiviteMenuStatistiques.ToString()
                        && statType == Enumerations.StatType.ExtractionRSE.ToString()
                        && i < 2023)
                        )
                    {
                        rowVals = new object[1];
                        rowVals[0] = i;
                        dT.Rows.Add(rowVals);
                    }
                }
            }
            return new JsonResult(dT, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getyesno
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of yes and no values</returns>
        [HttpGet("GetYesNoList")]
        public IActionResult GetYesNoList()
        {
            var dT = new DataTable();
            //Process
            dT.Columns.Add("value", typeof(bool));
            dT.Columns.Add("text", typeof(string));
            object[] rowVals = new object[2];
            rowVals[0] = true;
            rowVals[1] = CurrentContext.CulturedRessources.GetTextRessource(205);
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = false;
            rowVals[1] = CurrentContext.CulturedRessources.GetTextRessource(206);
            dT.Rows.Add(rowVals);
            return new JsonResult(dT, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/unlockalldata
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of yes and no values</returns>
        [HttpGet("unlockalldata")]
        public IActionResult UnlockAllData()
        {
            //Unlock data
            Utils.Utils.UnlockAllData(CurrentContext.RefUtilisateur, DbContext);
            //End
            return Ok();
        }
        /// <summary>
        /// GET: api/utils/gethabilitationvalues
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of enum values</returns>
        [HttpGet("gethabilitationvalues")]
        public IActionResult GetHabilitationValues()
        {
            string habilitationType = Request.Headers["habilitationType"].ToString();
            string[] h = new string[0];
            //Process
            switch (habilitationType)
            {
                case "HabilitationAdministration":
                    h = Enum.GetNames(typeof(Enumerations.HabilitationAdministration));
                    break;
                case "HabilitationAnnuaire":
                    h = Enum.GetNames(typeof(Enumerations.HabilitationAnnuaire));
                    break;
                case "HabilitationLogistique":
                    h = Enum.GetNames(typeof(Enumerations.HabilitationLogistique));
                    break;
                case "HabilitationMessagerie":
                    h = Enum.GetNames(typeof(Enumerations.HabilitationMessagerie));
                    break;
                case "HabilitationModuleCollectivite":
                    h = Enum.GetNames(typeof(Enumerations.HabilitationModuleCollectivite));
                    break;
                case "HabilitationModuleCentreDeTri":
                    h = Enum.GetNames(typeof(Enumerations.HabilitationModuleCentreDeTri));
                    break;
                case "HabilitationQualite":
                    h = Enum.GetNames(typeof(Enumerations.HabilitationQualite));
                    break;
                case "HabilitationModulePrestataire":
                    h = Enum.GetNames(typeof(Enumerations.HabilitationModulePrestataire));
                    break;
            }
            //End
            return new JsonResult(h, JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/passwordlost
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>true if e-mail has been sent, or an error</returns>
        [HttpGet("passwordlost")]
        public IActionResult PasswordLost()
        {
            string eMail = Request.Query["email"].ToString();
            //Set context
            string cultureName = Request.Headers["cultureName"].ToString();
            Context ctx = new(DbContext);
            ctx.ChangeCulture(new CultureInfo(cultureName));
            ctx.CulturedRessources = new CulturedRessources(ctx.CurrentCulture, DbContext);
            //Search for existing user
            var us = DbContext.Utilisateurs.Where(e => e.EMail == eMail.Trim() && e.Actif == true).ToList();
            string s = ctx.CulturedRessources.GetTextRessource(677);
            //Send email to each corresopnding user
            foreach (var u in us)
            {
                string operationType = "Creation";
                //Operation impossible if has Maitre
                if (u.RefUtilisateurMaitre > 0)
                {
                    operationType = "HasMaitre";
                }
                else
                {
                    if (u.PwdHash != null) { operationType = "Reset"; }
                }
                //Try to send e-mail
                s = EmailUtils.SendEmailPasswordManagement(operationType, u, 1, DbContext, ctx, Configuration);
                //exit on error
                if (s != "ok") { return UnprocessableEntity(new InternalServerError(s)); }
            }
            //End
            return new JsonResult(true);
        }
        /// <summary>
        /// GET: api/utils/getlistsagecategorieachatlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getappparametres")]
        public IActionResult GetAppParametres()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.Parametre> req = DbContext.Parametres;
            //Get data
            var all = req.ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Parametre[], ParametreViewModel[]>(all),
                JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getlistsageconditionlivraisonlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        /// <summary>
        /// GET: api/utils/getlistsagecategorieachatlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlistsagecategorieachatlist")]
        public IActionResult GetListSAGECategorieAchatList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.SAGECategorieAchat> req = DbContext.SAGECategorieAchats;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<SAGECategorieAchat[], SAGECategorieAchatViewModel[]>(all),
                JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getlistsageconditionlivraisonlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlistsageconditionlivraisonlist")]
        public IActionResult GetListSAGEConditionLivraisonList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.SAGEConditionLivraison> req = DbContext.SAGEConditionLivraisons;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<SAGEConditionLivraison[], SAGEConditionLivraisonViewModel[]>(all),
                JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getlistsageperiodicitelist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlistsageperiodicitelist")]
        public IActionResult GetListSAGEPeriodiciteList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.SAGEPeriodicite> req = DbContext.SAGEPeriodicites;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<SAGEPeriodicite[], SAGEPeriodiciteViewModel[]>(all),
                JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getlistsagemodereglementlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlistsagemodereglementlist")]
        public IActionResult GetListSAGEModeReglementList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.SAGEModeReglement> req = DbContext.SAGEModeReglements;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<SAGEModeReglement[], SAGEModeReglementViewModel[]>(all),
                JsonSettings);
        }
        /// <summary>
        /// GET: api/utils/getlistsagecategorieventelist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlistsagecategorieventelist")]
        public IActionResult GetListSAGECategorieVenteList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.SAGECategorieVente> req = DbContext.SAGECategorieVentes;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<SAGECategorieVente[], SAGECategorieVenteViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
