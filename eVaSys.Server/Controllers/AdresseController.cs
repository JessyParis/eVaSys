/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/12/2018
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class AdresseController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public AdresseController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/adresse/{id}
        /// Retrieves the Adresse with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Adresse</param>
        /// <returns>the Adresse with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            bool withEntite = (Request.Headers.ContainsKey("withEntite") ? (Request.Headers["withEntite"] == "true" ? true : false) : false);
            Adresse adresse;
            if (withEntite)
            {
                adresse = DbContext.Adresses
                       .Include(r => r.Entite)
                       .Include(r => r.UtilisateurCreation)
                       .Include(r => r.UtilisateurModif)
                       .Where(i => i.RefAdresse == id).FirstOrDefault();
            }
            else
            {
                adresse = DbContext.Adresses
                       .Include(r => r.UtilisateurCreation)
                       .Include(r => r.UtilisateurModif)
                       .Where(i => i.RefAdresse == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (adresse == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Adresse, AdresseViewModel>(adresse),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Adresse with the given {id}
        /// </summary>
        /// <param name="model">The AdresseViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]AdresseViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            // retrieve the adresse to edit
            Adresse adresse;
            if (model.RefAdresse > 0)
            {
                adresse = DbContext.Adresses.Where(q => q.RefAdresse ==
                            model.RefAdresse).FirstOrDefault();
            }
            else
            {
                adresse = new Adresse();
                DbContext.Adresses.Add(adresse);
            }

            // handle requests asking for non-existing adressezes
            if (adresse == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            Utils.DataUtils.UpdateDataAdresse(ref adresse, model, CurrentContext.RefUtilisateur);
            //Register session user
            adresse.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = adresse.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Adresse to the client.
                return new JsonResult(
                    _mapper.Map<Adresse, AdresseViewModel>(adresse),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Adresse with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the adresse from the Database
            var adresse = DbContext.Adresses.Where(i => i.RefAdresse == id)
                .FirstOrDefault();

            // handle requests asking for non-existing adressezes
            if (adresse == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = adresse.IsDeletable(false);
            if (del == "")
            {
                DbContext.Adresses.Remove(adresse);
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
        /// GET: api/adresse/isdeletable
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>true if Adresse can be deleted, false if not</returns>
        [HttpGet("isdeletable")]
        public IActionResult IsDeletable()
        {
            string refAdresse = Request.Headers["refAdresse"].ToString();
            bool excludeProcess = (Request.Headers["excludeProcess"] == "true");
            bool r = false;
            int refA = 0;
            int.TryParse(refAdresse, out refA);
            //Get adresse from database
            try
            {
                var adr = DbContext.Adresses.Find(refA);
                r = (adr.IsDeletable(excludeProcess) == "");
            }
            catch
            {
                //If adresse does not exists, then it is deletable
                r = true;
            }
            //Return Json
            return new JsonResult(r,
                JsonSettings);
        }
        /// <summary>
        /// GET: api/items/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            string refAdresse = Request.Headers["refAdresse"].ToString();
            string refEntite = Request.Headers["refEntite"].ToString();
            string refContactAdresseProcess = Request.Headers["refContactAdresseProcess"].ToString();
            string refAdresseType = Request.Headers["refAdresseType"].ToString();
            bool actif = (Request.Headers.ContainsKey("actif") ? (Request.Headers["actif"] == "true" ? true : false) : false);
            string refProduit = Request.Headers["refProduit"].ToString();
            string d = Request.Headers["d"].ToString();
            string refEntiteType = Request.Headers["refEntiteType"].ToString();
            int refA = 0;
            int refE = 0;
            int refCAP = 0;
            int refAT = 0;
            int refP = 0;
            DateTime dRef = DateTime.MinValue;
            int refET = 0;
            int.TryParse(refAdresse, out refA);
            int.TryParse(refEntiteType, out refET);
            //Where
            System.Linq.IQueryable<eVaSys.Data.Adresse> req = DbContext.Adresses;
            if (int.TryParse(refEntite, out refE)) { if (refE != 0) { req = req.Where(el => el.RefEntite == refE); } }
            else if (refET == 0) { req = req.Where(el => el.RefAdresse == 0); }
            if (int.TryParse(refAdresseType, out refAT)) { if (refAT != 0) { req = req.Where(el => el.RefAdresseType == refAT); } }
            if (int.TryParse(refContactAdresseProcess, out refCAP))
            {
                if (refCAP != 0)
                {
                    req = req.Where(el => el.ContactAdresses.Any(rel => rel.Actif == true && rel.ContactAdresseContactAdresseProcesss.Any(related => related.RefContactAdresseProcess == refCAP)));
                }
            }
            if (int.TryParse(refProduit, out refP) && DateTime.TryParse(d, out dRef))
            {
                req = req.Where(el => (el.CommandeClients.Any(
                    rel => rel.RefProduit == refP && rel.CommandeClientMensuelles.Any(
                        r => (r.D.Month == dRef.Month && r.D.Year == dRef.Year)))));
            }
            if (refET != 0) { req = req.Where(el => el.Entite.RefEntiteType == 4); }
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (int.TryParse(refAdresse, out refA))
                {
                    req = req.Where(el => (el.Entite.Actif == true && el.Actif == true) || el.RefAdresse == refA);
                }
                else { req = req.Where(el => el.Entite.Actif == true && el.Actif == true); }
            }
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Adresse[], AdresseViewModel[]>(all),
                JsonSettings);
        }
        /// <summary>
        /// GET: api/items/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetListVille")]
        public IActionResult GetListVille()
        {
            string villeType = Request.Headers["villeCategory"].ToString();
            SqlCommand cmd = new();
            DataSet dS = new();
            string sqlStr = "";
            switch (villeType)
            {
                case "TransportDepart":
                    sqlStr = "Select distinct Ville from tblAdresse"
                        + "     inner join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                        + "     inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + "     inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse=tbmContactAdresse.RefContactAdresse "
                        + " where RefContactAdresseProcess=1 and tblEntite.RefEntiteType=3";
                    if (CurrentContext.filterGlobalActif)
                    {
                        sqlStr += " and TblAdresse.Actif=1 and tblEntite.Actif=1";
                    }
                    sqlStr += " order by Ville";
                    break;
                case "TransportArrivee":
                    sqlStr = "Select distinct Ville from tblAdresse"
                        + "     inner join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                        + "     inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + "     inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse=tbmContactAdresse.RefContactAdresse "
                        + " where RefContactAdresseProcess=1 and tblEntite.RefEntiteType in(4,5)";
                    if (CurrentContext.filterGlobalActif)
                    {
                        sqlStr += " and TblAdresse.Actif=1 and tblEntite.Actif=1";
                    }
                    sqlStr += " order by Ville";
                    break;
            }
            //Chargement des données si elles existent
            if (sqlStr != "")
            {
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    SqlDataAdapter dA = new(cmd);
                    if (dS.Tables.Count > 0) { dS.Tables.Clear(); }
                    dA.Fill(dS);
                }
                //Return Json
                return new JsonResult(dS.Tables[0], JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        #endregion
        #region Services
        #endregion
    }
}
