/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/05/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using eVaSys.APIUtils;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class CommandeClientController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public CommandeClientController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/commandeclient/{id}
        /// Retrieves the CommandeClient with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing CommandeClient</param>
        /// <returns>the CommandeClient with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            CommandeClient commandeClient = null;
            //Get headers
            string refEntite = Request.Headers["refEntite"].ToString();
            string refAdresse = Request.Headers["refAdresse"].ToString();
            string refProduit = Request.Headers["refProduit"].ToString();
            string d = Request.Headers["d"].ToString();
            int refP = 0;
            int refE = 0;
            int refA = 0;
            DateTime dRef = DateTime.MinValue;
            //Get or create commandeClient
            if (id == 0)
            {
                if (int.TryParse(refEntite, out refE) && int.TryParse(refAdresse, out refA) && int.TryParse(refProduit, out refP) && DateTime.TryParse(d, out dRef))
                {
                    commandeClient = DbContext.CommandeClients
                        .Include(r => r.CommandeClientMensuelles)
                        .Include(r => r.UtilisateurCreation)
                        .Include(r => r.UtilisateurModif)
                        .Where(el => (el.RefEntite == refE && el.RefAdresse == refA && el.RefProduit == refP && el.CommandeClientMensuelles.Any(
                        r => (r.D.Month == dRef.Month && r.D.Year == dRef.Year))))
                        .FirstOrDefault();
                }
                else
                {
                    commandeClient = new CommandeClient();
                    DbContext.CommandeClients.Add(commandeClient);
                }
            }
            else
            {
                commandeClient = DbContext.CommandeClients
                    .Include(r => r.CommandeClientMensuelles)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefCommandeClient == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (commandeClient == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            commandeClient.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<CommandeClient, CommandeClientViewModel>(commandeClient),
                JsonSettings);
        }

        /// <summary>
        /// Create/Modify/Delete CommandeClientMensuelle
        /// </summary>
        /// <param name="model">The CommandeClientFromViewModels containing the data to update</param>
        [HttpPost("PostCommandeClientMensuelles")]
        public IActionResult PostCommandeClientMensuelles([FromBody]CommandeClientMensuelleFormViewModel[] model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            //Init
            string errs = "";
            //For each CommandeClientMensuelle
            foreach (CommandeClientMensuelleFormViewModel cmdMF in model)
            {
                CommandeClient cmd = null;
                CommandeClientMensuelle cmdM = null;
                //Delete CommandeClientMensuelle if applicable
                if (cmdMF.RefCommandeClientMensuelle != null && (cmdMF.Poids == 0 || cmdMF.Poids == null))
                {
                    cmdM = DbContext.CommandeClientMensuelles.Find(cmdMF.RefCommandeClientMensuelle);
                    if (cmdM != null) { DbContext.Remove(cmdM); }
                }
                else if(cmdMF.Poids != 0 && cmdMF.Poids != null)
                {
                    //Get the corresponding CommandeClient
                    if (cmdMF.RefCommandeClient != null)
                    {
                        cmd = DbContext.CommandeClients
                            .Include(r => r.CommandeClientMensuelles)
                            .Where(i => i.RefCommandeClient == cmdMF.RefCommandeClient)
                            .FirstOrDefault();
                    }
                    else
                    {
                        cmd = DbContext.CommandeClients
                            .Include(r => r.CommandeClientMensuelles)
                            .Where(i => i.RefEntite == cmdMF.RefEntite && i.RefAdresse == cmdMF.RefAdresse && i.D.Year == cmdMF.D.Year && i.RefProduit == cmdMF.RefProduit)
                            .FirstOrDefault();
                        if (cmd == null)
                        {
                            cmd = new CommandeClient
                            {
                                RefEntite = cmdMF.RefEntite,
                                RefEntiteFournisseur = cmdMF.RefEntiteFournisseur,
                                RefAdresse = cmdMF.RefAdresse,
                                RefProduit = cmdMF.RefProduit,
                                D = new DateTime(cmdMF.D.Year, 1, 1),
                                CommandeClientMensuelles = new HashSet<CommandeClientMensuelle>()
                            };
                            DbContext.CommandeClients.Add(cmd);
                        }
                    }
                    if (cmd != null)
                    {
                        cmd.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                        //Get the CommandeClientMensuelle
                        if (cmdMF.RefCommandeClientMensuelle != null)
                        {
                            cmdM = cmd.CommandeClientMensuelles.FirstOrDefault(e => e.RefCommandeClientMensuelle==cmdMF.RefCommandeClientMensuelle);
                        }
                        else
                        {
                            cmdM = new CommandeClientMensuelle
                            {
                                D = cmdMF.D
                            };
                            DbContext.CommandeClientMensuelles.Add(cmdM);
                            cmd.CommandeClientMensuelles.Add(cmdM);
                        }
                        if (cmdM != null)
                        {
                            //Update data
                            cmd.Cmt = cmdMF.Cmt;
                            cmdM.Poids = (int)cmdMF.Poids;
                            cmdM.PrixTonneHT = (decimal)(cmdMF.PrixTonneHT==null?0:cmdMF.PrixTonneHT);
                            cmdM.IdExt = cmdMF.IdExt;
                        }
                    }
                    string validError = cmd.IsValid() + " " + cmdM.IsValid();
                    if (validError == " ")
                    {
                        DbContext.SaveChanges();
                    }
                    else
                    {
                        errs += validError;
                    }
                }
            }
            //End
            if (errs == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Return the updated CommandeClientMensuelles to the client
                return Ok();
            }
            else
            {
                return Conflict(new ConflictError(errs));
            }
        }

        /// <summary>
        /// Deletes the CommandeClient with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the commandeClient from the Database
            var commandeClient = DbContext.CommandeClients
                .Include(r=>r.CommandeClientMensuelles)
                .Where(i => i.RefCommandeClient == id)
                .FirstOrDefault();

            // handle requests asking for non-existing commandeClientzes
            if (commandeClient == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = commandeClient.IsDeletable();
            if (del == "")
            {
                DbContext.CommandeClients.Remove(commandeClient);
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
        /// GET: evapi/getcommandeclientmensuelles/{id}
        /// Retrieves the CommandeClientMensuelle regarding parameters
        /// </summary>
        /// <param name="id">The ID of an existing CommandeClient</param>
        /// <returns>the CommandeClient with the given {id}</returns>
        [HttpGet("GetCommandeClientMensuelles")]
        public IActionResult GetCommandeClientMensuelles()
        {
            //Get headers
            string refEntite = Request.Headers["refEntite"].ToString();
            string refAdresse = Request.Headers["refAdresse"].ToString();
            string d = Request.Headers["d"].ToString();
            DateTime dRef = DateTime.MinValue;
            DataSet dS = new();
            //Get CommandeClientMensuelles
            if (int.TryParse(refEntite, out int refE) && int.TryParse(refAdresse, out int refA) && DateTime.TryParse(d, out dRef))
            {
                SelectCommandeClientMensuelles(refE, refA, dRef, ref dS);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
            // handle requests asking for non-existing object
            if (dS.Tables[0].Rows.Count == 0)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            //Return Json
            return new JsonResult(dS.Tables[0], JsonSettings);
        }
        #endregion
        #region Services
        private void SelectCommandeClientMensuelles(int refE, int refA, DateTime dRef, ref DataSet dS)
        {
            SqlCommand cmd = new();
            SqlDataAdapter dA = new(cmd);
            string sqlStr = "select tblProduit.Libelle as LibelleProduit, univers.*"
                + " from tblProduit"
                + "     inner join"
                + "     (select isnull(entiteP.RefProduit, cmdM.Refproduit) as RefProduit"
                + "         , @refEntite as RefEntite, @refAdresse as RefAdresse, cmdM.Cmt, cmdM.RefCommandeClient, cmdM.RefCommandeClientMensuelle, @d as D, cmdM.Poids, cmdM.PrixTonneHT, cmdM.IdExt"
                + "     from"
                + "         (select tblCommandeClient.RefEntite, tblCommandeClient.RefAdresse, tblCommandeClient.RefProduit, tblCommandeClient.Cmt, tblCommandeClientMensuelle.*"
                + "         from tblCommandeClient"
                + "             left join tblCommandeClientMensuelle on tblCommandeClientMensuelle.RefCommandeClient = tblCommandeClient.RefCommandeClient"
                + "         where tblCommandeClient.RefEntite = @refEntite and tblCommandeClient.RefAdresse = @refAdresse and year(tblCommandeClientMensuelle.D) = year(@d) and month(tblCommandeClientMensuelle.D) = month(@d)) as cmdM"
                + "         full outer join"
                + "         (select * from tbmEntiteProduit where RefEntite = @refEntite) as entiteP on cmdM.RefProduit = entiteP.RefProduit) as univers"
                + "     on tblProduit.RefProduit = univers.Refproduit"
                + " order by (case when Poids is null or Poids=0 then 0 else 1 end) desc, Libelle";
            cmd.CommandText = sqlStr;
            cmd.Parameters.Add("@refEntite", SqlDbType.Int).Value = refE;
            cmd.Parameters.Add("@refAdresse", SqlDbType.Int).Value = refA;
            cmd.Parameters.Add("@d", SqlDbType.DateTime).Value = dRef;
            cmd.Connection = (SqlConnection)DbContext.Database.GetDbConnection();
            dA.Fill(dS);
        }
        #endregion
    }
}

