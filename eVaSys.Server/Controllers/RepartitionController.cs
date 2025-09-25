/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 01/10/2019
/// -----------------------------------------------------------------------------------------------------
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class RepartitionController : BaseApiController
    {
        private readonly IMapper _mapper;

        #region Constructor

        public RepartitionController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }

        #endregion Constructor

        #region RESTful Conventions

        /// <summary>
        /// GET: evapi/repartition/{id}
        /// Retrieves the Repartition with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Repartition</param>
        /// <returns>the Repartition with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Repartition repartition = null;
            //Get or create repartition
            if (id != 0)
            {
                repartition = DbContext.Repartitions
                        .Include(r => r.UtilisateurCreation)
                        .Include(r => r.UtilisateurModif)
                        .Include(r => r.RepartitionCollectivites)
                        .Include(r => r.RepartitionProduits)
                        .AsSplitQuery()
                    .Where(i => i.RefRepartition == id).FirstOrDefault();
            }
            else
            {
                int refCommandeFournisseur = System.Convert.ToInt32(Request.Headers["refCommandeFournisseur"]);
                CommandeFournisseur cmdF = DbContext.CommandeFournisseurs.Find(refCommandeFournisseur);
                if (cmdF != null)
                {
                    //Looking for exiting element
                    repartition = DbContext.Repartitions
                        .Include(r => r.UtilisateurCreation)
                        .Include(r => r.UtilisateurModif)
                        .Include(r => r.RepartitionCollectivites)
                        .Include(r => r.RepartitionProduits)
                        .AsSplitQuery()
                        .Where(q => q.RefCommandeFournisseur == refCommandeFournisseur)
                        .FirstOrDefault();
                    //Open existing or create new one
                    if (repartition == null)
                    {
                        //Create new element
                        repartition = new Repartition();
                        repartition.CommandeFournisseur = cmdF;
                        //Unit
                        //Add to context
                        DbContext.Repartitions.Add(repartition);
                    }
                }
            }
            // handle requests asking for non-existing object
            if (repartition == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            repartition.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<Repartition, RepartitionViewModel>(repartition),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Repartition with the given {id}
        /// </summary>
        /// <param name="model">The RepartitionViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]RepartitionViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Repartition repartition;
            //Retrieve or create the entity to edit
            if (model.RefRepartition > 0)
            {
                repartition = DbContext.Repartitions
                    .Where(q => q.RefRepartition == model.RefRepartition)
                    .Include(r => r.RepartitionCollectivites)
                    .Include(r => r.RepartitionProduits)
                    .AsSplitQuery()
                    .FirstOrDefault();
            }
            else
            {
                repartition = new Repartition();
                DbContext.Repartitions.Add(repartition);
            }

            // handle requests asking for non-existing repartitionzes
            if (repartition == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            DataUtils.UpdateDataRepartition(ref repartition, model, CurrentContext.ConnectedUtilisateur.RefUtilisateur);

            //Register session user
            repartition.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = repartition.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                return new JsonResult(
                    _mapper.Map<Repartition, RepartitionViewModel>(repartition),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Repartition with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the repartition from the Database
            var repartition = DbContext.Repartitions
                .Include(r => r.RepartitionCollectivites)
                .Include(r => r.RepartitionProduits)
                .Where(i => i.RefRepartition == id)
                .FirstOrDefault();

            // handle requests asking for non-existing repartitionzes
            if (repartition == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = repartition.IsDeletable();
            if (del == "")
            {
                DbContext.Repartitions.Remove(repartition);
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
        /// GET: api/repatition/getcommandefournisseurs
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of CommandeFournisseur extracts items.</returns>
        [HttpGet("getcommandefournisseurs")]
        public IActionResult getcommandefournisseurs()
        {
            string dRef = Request.Headers["d"].ToString();
            string refProduit = Request.Headers["refProduit"].ToString();
            string refFournisseur = Request.Headers["refFournisseur"].ToString();
            DateTime d = DateTime.MinValue;
            int refP = 0;
            int refF = 0;
            SqlCommand cmd = new();
            DataSet dS = new();
            //Check mandatory parameters
            if (int.TryParse(refFournisseur, out refF) && int.TryParse(refProduit, out refP) && DateTime.TryParse(dRef, out d))
            {
                cmd.Parameters.Add("@refF", SqlDbType.Int).Value = refF;
                cmd.Parameters.Add("@refP", SqlDbType.Int).Value = refP;
                cmd.Parameters.Add("@d", SqlDbType.DateTime).Value = d;
                //Chaine SQL
                string sqlStr = "select NumeroCommande, PoidsReparti, PoidsChargement"
                    + " from tblCommandeFournisseur"
                    + " where tblCommandeFournisseur.RefEntite=@refF and year(tblCommandeFournisseur.DDechargement)=year(@d) and month(tblCommandeFournisseur.DDechargement)=month(@d) and tblCommandeFournisseur.RefProduit=@refP"
                    + " order by tblCommandeFournisseur.DDechargement desc";
                cmd.CommandText = sqlStr;
                SqlDataAdapter dA = new(cmd);
                if (dS.Tables.Count > 0) { dS.Tables.Clear(); }
                using (DbConnection sqlConn = DbContext.Database.GetDbConnection())
                {
                    sqlConn.Open();
                    cmd.Connection = (SqlConnection)sqlConn;
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

        /// <summary>
        /// GET: api/repartition/issimilar
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Check Repartition for same product in the last year</returns>
        [HttpGet("issimilar")]
        public IActionResult IsSimilar()
        {
            string refCommandeFournisseur = Request.Headers["refCommandeFournisseur"].ToString();
            string d = Request.Headers["d"].ToString();
            int refCF = 0;
            DateTime D = DateTime.MinValue;
            //Check mandatory parameters
            if (int.TryParse(refCommandeFournisseur, out refCF) && DateTime.TryParse(d, out D))
            {
                CommandeFournisseur cF = null; //Original Repartition
                cF = DbContext.CommandeFournisseurs.Where(e => e.RefCommandeFournisseur == refCF).FirstOrDefault();
                //Exit if no result
                if (cF == null) { return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711))); }
                //Process if ok
                var q = DbContext.Repartitions
                    .Include(i => i.CommandeFournisseur)
                    .Include(i => i.RepartitionCollectivites)
                    .Include(i => i.RepartitionProduits)
                    .Where(i =>
                        i.CommandeFournisseur.RefCommandeFournisseur != cF.RefCommandeFournisseur
                        && i.CommandeFournisseur.RefEntite == cF.RefEntite
                        && i.CommandeFournisseur.RefProduit == cF.RefProduit
                        && i.CommandeFournisseur.DChargement <= D && i.CommandeFournisseur.DChargement > D.AddDays(-365));
                //Same type (collect, hors collecte or both)
                if (cF.PoidsReparti == 0)
                {
                    q = q.Where(e => e.CommandeFournisseur.PoidsReparti == 0);
                }
                else if (cF.PoidsReparti == cF.PoidsChargement)
                {
                    q = q.Where(e => e.CommandeFournisseur.PoidsReparti == e.CommandeFournisseur.PoidsChargement);
                }
                else
                {
                    q = q.Where(e => e.CommandeFournisseur.PoidsReparti != e.CommandeFournisseur.PoidsChargement);
                }
                q = q.OrderByDescending(e => e.CommandeFournisseur.DChargement);
                var rep = q.FirstOrDefault();
                //Exit if no result
                if (rep == null) { return BadRequest(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460))); }
                //Return Json
                return new JsonResult(
                    _mapper.Map<Repartition, RepartitionViewModel>(rep),
                    JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }

        #endregion Attribute-based Routing

        #region Services

        #endregion Services
    }
}