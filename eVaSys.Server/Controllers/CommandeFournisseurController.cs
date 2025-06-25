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
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.MIME;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Net;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class CommandeFournisseurController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public CommandeFournisseurController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper, IWebHostEnvironment env)
            : base(context, configuration)
        {
            _mapper = mapper;
            _env = env;
        }
        protected IWebHostEnvironment _env;
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/commandefournisseur/{id}
        /// Retrieves the CommandeFournisseur with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing CommandeFournisseur</param>
        /// <returns>the CommandeFournisseur with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            CommandeFournisseur commandeFournisseur = null;
            bool lockData = Request.Headers["lockData"].ToString() == "true" ? true : false;
            //Get or create commandeFournisseur
            if (id == 0)
            {
                commandeFournisseur = new CommandeFournisseur();
                DbContext.CommandeFournisseurs.Add(commandeFournisseur);
            }
            else
            {
                commandeFournisseur = DbContext.CommandeFournisseurs
                    .Include(r => r.CommandeFournisseurFichiers)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefCommandeFournisseur == id).FirstOrDefault();
                //Check rights
                if (!Rights.AuthorizedEntity(commandeFournisseur.RefCommandeFournisseur, Enumerations.ObjectName.CommandeFournisseur, CurrentContext.ConnectedUtilisateur, DbContext))
                {
                    return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
                }
                //Lock data if applicable
                if (lockData)
                {
                    Utils.Utils.LockData(Enumerations.DataTypeToLock.RefCommandeFournisseur.ToString(), CurrentContext.RefUtilisateur, commandeFournisseur?.RefCommandeFournisseur, DbContext); ;
                }
            }
            // handle requests asking for non-existing object
            if (commandeFournisseur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            commandeFournisseur.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<CommandeFournisseur, CommandeFournisseurViewModel>(commandeFournisseur),
                JsonSettings);
        }

        /// <summary>
        /// Edit the CommandeFournisseur with the given {id}
        /// </summary>
        /// <param name="model">The CommandeFournisseurViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] CommandeFournisseurViewModel model)
        {
            CommandeFournisseur cmd;
            bool unLockData = Request.Headers["unLockData"].ToString() == "true" ? true : false;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            // retrieve the commandeFournisseur to edit
            var commandeFournisseur = new CommandeFournisseur();
            if (model.RefCommandeFournisseur > 0)
            {
                commandeFournisseur = DbContext.CommandeFournisseurs
                        .Where(q => q.RefCommandeFournisseur == model.RefCommandeFournisseur)
                        .Include(r => r.CommandeFournisseurFichiers)
                        .FirstOrDefault();
            }
            else { DbContext.CommandeFournisseurs.Add(commandeFournisseur); }

            // handle requests asking for non-existing commandeFournisseurzes
            if (commandeFournisseur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            bool dirtyAnomalieChargement = false, dirtyAnomalieClient = false, dirtyAnomalieTransporteur = false;
            bool dirtyTransporteur = false, dirtyCDT = false, newDPrevue = false, newReceptionTransporteur = false, newReceptionClient = false;
            UpdateData(ref commandeFournisseur, model, ref dirtyAnomalieChargement, ref dirtyAnomalieClient, ref dirtyAnomalieTransporteur
                , ref dirtyTransporteur, ref dirtyCDT, ref newDPrevue, ref newReceptionTransporteur, ref newReceptionClient);

            //Register session user
            commandeFournisseur.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = commandeFournisseur.IsValid();
            //End
            if (string.IsNullOrEmpty(valid))
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                DbContext.Entry(commandeFournisseur).Reload();
                //Process additionnal actions
                //Inform external API if applicable
                if (!string.IsNullOrEmpty(commandeFournisseur.RefExt))
                {
                    if (newDPrevue)
                    {
                        LaserAPIUtils.CommandeFournisseurSendDestination(commandeFournisseur, CurrentContext.RefUtilisateur, DbContext, Configuration);
                        LaserAPIUtils.CommandeFournisseurModifyCarrier(commandeFournisseur, CurrentContext.RefUtilisateur, DbContext, Configuration);
                        LaserAPIUtils.CommandeFournisseurSendDatesPrevues(commandeFournisseur, CurrentContext.RefUtilisateur, DbContext, Configuration);
                    }
                    if (newReceptionTransporteur)
                    {
                        LaserAPIUtils.CommandeFournisseurSendDatesReelles(commandeFournisseur, CurrentContext.RefUtilisateur, DbContext, Configuration);
                    }
                    if (newReceptionClient)
                    {
                        LaserAPIUtils.CommandeFournisseurSendReception(commandeFournisseur, CurrentContext.RefUtilisateur, DbContext, Configuration);
                    }
                }
                //Send e-mails
                if (dirtyAnomalieChargement) { try { SendEmail(commandeFournisseur, null, "AnomalieChargement"); } catch { } }
                if (dirtyAnomalieClient) { try { SendEmail(commandeFournisseur, null, "AnomalieClient"); } catch { } }
                if (dirtyAnomalieTransporteur) { try { SendEmail(commandeFournisseur, null, "AnomalieTransporteur"); } catch { } }
                if (dirtyTransporteur) { try { SendEmail(commandeFournisseur, null, "NouveauTransport"); } catch { } }
                if (dirtyCDT) { try { SendEmail(commandeFournisseur, null, "NouvelEnlevement"); } catch { } }
                //Unlock data if applicable
                if (unLockData)
                {
                    Utils.Utils.UnlockData(Enumerations.DataTypeToLock.RefCommandeFournisseur.ToString(), commandeFournisseur.RefCommandeFournisseur, CurrentContext.RefUtilisateur, DbContext);
                }
                //Return the updated CommandeFournisseur to the client, or return the next Mixte if applicable
                //Check if Mixte depending on user Habilitation
                if (CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Transporteur.ToString())
                {
                    cmd = DbContext.CommandeFournisseurs
                        .Where(i => i.NumeroAffretement == commandeFournisseur.NumeroAffretement && i.RefCommandeFournisseur != commandeFournisseur.RefCommandeFournisseur
                        && i.RefTransporteur == CurrentContext.ConnectedUtilisateur.RefTransporteur
                        && (i.DChargementPrevue == null || i.DDechargementPrevue == null))
                        .FirstOrDefault();
                    if (cmd != null)
                    {
                        //Lock data
                        Utils.Utils.LockData(Enumerations.DataTypeToLock.RefCommandeFournisseur.ToString(), CurrentContext.RefUtilisateur, commandeFournisseur.RefCommandeFournisseur, DbContext); ;
                        //Return
                        return new JsonResult(
                            _mapper.Map<CommandeFournisseur, CommandeFournisseurViewModel>(cmd),
                            JsonSettings);
                    }
                }
                else if (CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Administrateur.ToString())
                {
                    cmd = DbContext.CommandeFournisseurs
                        .Where(i => i.NumeroAffretement == commandeFournisseur.NumeroAffretement && i.RefCommandeFournisseur != commandeFournisseur.RefCommandeFournisseur
                        && (i.DChargementPrevue != null && i.DDechargementPrevue != null && i.ValideDPrevues != true))
                        .FirstOrDefault();
                    if (cmd != null)
                    {
                        //Lock data
                        Utils.Utils.LockData(Enumerations.DataTypeToLock.RefCommandeFournisseur.ToString(), CurrentContext.RefUtilisateur, commandeFournisseur.RefCommandeFournisseur, DbContext); ;
                        //Return
                        return new JsonResult(
                            _mapper.Map<CommandeFournisseur, CommandeFournisseurViewModel>(cmd),
                            JsonSettings);
                    }
                }
                return new JsonResult(
                    _mapper.Map<CommandeFournisseur, CommandeFournisseurViewModel>(commandeFournisseur),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the CommandeFournisseur with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the commandeFournisseur from the Database
            var commandeFournisseur = DbContext.CommandeFournisseurs
                .Include(r => r.CommandeFournisseurFichiers)
                .Where(i => i.RefCommandeFournisseur == id)
                .FirstOrDefault();

            // handle requests asking for non-existing commandeFournisseurzes
            if (commandeFournisseur == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = commandeFournisseur.IsDeletable();
            if (del == "")
            {
                //Inform external API if applicable
                if (!string.IsNullOrEmpty(commandeFournisseur.RefExt))
                {
                    LaserAPIUtils.CommandeFournisseurRefuseCarriage(commandeFournisseur, CurrentContext.RefUtilisateur, DbContext, Configuration);
                }
                //Delete
                DbContext.CommandeFournisseurs.Remove(commandeFournisseur);
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
        /// GET: api/commandefournisseurs/getallmixte
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of CommandeFournisseur with same Affretement.</returns>
        [HttpGet("getallmixte")]
        public IActionResult GetAllMixte()
        {
            string numeroAffretement = Request.Headers["numeroAffretement"].ToString();
            int nAff = 0;
            //Check mandatory parameters
            if (int.TryParse(numeroAffretement, out nAff))
            {
                var cFs = DbContext.CommandeFournisseurs
                        .Where(el => el.NumeroAffretement == nAff)
                        .Select(i => i.RefCommandeFournisseur)
                        .ToArray();
                //Return Json
                return new JsonResult(cFs, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// GET: api/commandefournisseurs/getcible
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of cible items.</returns>
        [HttpGet("getcible")]
        public IActionResult GetCible()
        {
            string refCommandeFournisseur = Request.Headers["refCommandeFournisseur"].ToString();
            string refFournisseur = Request.Headers["refFournisseur"].ToString();
            string refTransporteur = Request.Headers["refTransporteur"].ToString();
            string refAdresseFournisseur = Request.Headers["refAdresseFournisseur"].ToString();
            string refProduit = Request.Headers["refProduit"].ToString();
            string refClient = Request.Headers["refClient"].ToString();
            string dMoisDechargementPrevu = Request.Headers["dMoisDechargementPrevu"].ToString();
            string filterCamionTypes = Request.Headers["filterCamionTypes"].ToString();
            string filterVilleArrivees = Request.Headers["filterVilleArrivees"].ToString();
            int refCF = 0;
            int refF = 0;
            int refAF = 0;
            int refP = 0;
            int refC = 0;
            int refT = 0;
            int refContrat = 0;
            DateTime d = DateTime.MinValue;
            SqlCommand cmd = new();
            DataSet dS = new();
            //Check mandatory parameters
            if (int.TryParse(refFournisseur, out refF) && int.TryParse(refCommandeFournisseur, out refCF)
                && int.TryParse(refAdresseFournisseur, out refAF) && int.TryParse(refProduit, out refP) && DateTime.TryParse(dMoisDechargementPrevu, out d))
            {
                using (DbConnection sqlConn = DbContext.Database.GetDbConnection())
                {
                    sqlConn.Open();
                    cmd.Connection = (SqlConnection)sqlConn;
                    //Parameters
                    cmd.Parameters.Add("@refCommandeFournisseur", SqlDbType.Int).Value = refCF;
                    cmd.Parameters.Add("@refFournisseur", SqlDbType.Int).Value = refF;
                    cmd.Parameters.Add("@refAdresseFournisseur", SqlDbType.Int).Value = refAF;
                    cmd.Parameters.Add("@refProduit", SqlDbType.Int).Value = refP;
                    cmd.Parameters.Add("@dMoisDechargementPrevu", SqlDbType.DateTime).Value = d;
                    //Vérification d'un contrat RI
                    string sqlStr = "select RefContrat from VueCommandeFournisseurContrat where RefCommandeFournisseur=@refCommandeFournisseur";
                    cmd.CommandText = sqlStr;
                    var res = cmd.ExecuteScalar();
                    if(res != null)
                    {
                        refContrat = (int)res;
                        cmd.Parameters.Add("@refContrat", SqlDbType.Int).Value = refContrat;
                    }
                    //Chaine SQL
                    sqlStr = "select distinct tblTransport.RefTransport, RefAdresseDestination, Client.Libelle as Client, (commandeClient.Poids*1000 - isnull(reliquat.Poids,0))/1000 as [ResteALivrer], (isnull(reliquat.Poids,0)*100)/(commandeClient.Poids*1000) as [Positionne], tblAdresse.Ville, transporteur.RefEntite as RefTransporteur"
                        + "     , transporteur.Libelle as Transporteur, tbrCamionType.Libelle as [CamionType], tblTransport.PUHT, tr.NbTransportIdentique, tblParcours.Km"
                        + " from tblEntite as transporteur"
                        + " 	inner join tblTransport on transporteur.RefEntite=tblTransport.RefTransporteur "
                        + " 	inner join tblParcours on tblTransport.RefParcours=tblParcours.RefParcours"
                        + "     inner join tbrCamionType on tblTransport.RefCamionType=tbrCamionType.RefCamionType"
                        + " 	inner join"
                        + "         (select distinct tblAdresse.RefAdresse, tblAdresse.RefEntite, Ville"
                        + "             from tblAdresse"
                        + "    			inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + "    			inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse=tbmContactAdresse.RefContactAdresse "
                        + "         where RefContactAdresseProcess=1 and tblAdresse.Actif=1) as tblAdresse on tblParcours.RefAdresseDestination=tblAdresse.RefAdresse"
                        + " 	inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                        + " 	inner join tblAdresse as adresseOrigine on tblParcours.RefAdresseOrigine=adresseOrigine.RefAdresse"
                        + " 	inner join "
                        + " 		(select RefEntite, RefAdresse, tblCommandeClientMensuelle.Poids  from tblCommandeClient inner join tblCommandeClientMensuelle on tblCommandeClient.RefCommandeClient=tblCommandeClientMensuelle.RefCommandeClient "
                        + " 		where RefProduit=@refProduit and year(tblCommandeClient.D)=year(@dMoisDechargementPrevu) and month(tblCommandeClientMensuelle.D)=month(@dMoisDechargementPrevu) and tblCommandeClientMensuelle.Poids>0";
                    //Contrat RI or not
                    if (refContrat > 0)
                    {
                        sqlStr += " and tblCommandeClient.RefContrat=@refContrat";
                    }
                    else
                    {
                        sqlStr += " and tblCommandeClient.RefContrat is null";
                    }

                    sqlStr += "         ) as commandeClient"
                        + " 		on client.RefEntite=commandeClient.RefEntite and tblParcours.RefAdresseDestination=CommandeClient.RefAdresse"
                        + " 	left join "
                        + " 		(select RefAdresseClient, sum(case when DDechargement is null then PoidsChargement else PoidsDechargement end) as Poids"
                        + " 		from tblCommandeFournisseur"
                        + " 		where year(isnull(DDechargement, DMoisDechargementPrevu))=year(@dMoisDechargementPrevu) and month(isnull(DDechargement, DMoisDechargementPrevu))=month(@dMoisDechargementPrevu) and RefProduit=@refProduit and RefusCamion=0";
                    //Contrat RI or not
                    if (refContrat > 0)
                    {
                        sqlStr += " and RefCommandeFournisseur in (select RefCommandeFournisseur from VueCommandeFournisseurContrat where RefContrat=@refContrat)";
                    }
                    else
                    {
                        sqlStr += " and RefCommandeFournisseur not in (select RefCommandeFournisseur from VueCommandeFournisseurContrat)";
                    }
                    sqlStr += " 		group by RefAdresseClient) as reliquat"
                        + " 		on tblParcours.RefAdresseDestination=reliquat.RefAdresseClient"
                        + " 	left join (select * from tbmEntiteEntite where Actif=1) as tbmEntiteEntite on "
                        + " 		(client.RefEntite=tbmEntiteEntite.RefEntite and adresseOrigine.RefEntite=tbmEntiteEntite.RefEntiteRtt)"
                        + " 		or (client.RefEntite=tbmEntiteEntite.RefEntiteRtt and adresseOrigine.RefEntite=tbmEntiteEntite.RefEntite)"
                        + " 		or (client.RefEntite=tbmEntiteEntite.RefEntite and transporteur.RefEntite=tbmEntiteEntite.RefEntiteRtt)"
                        + " 		or (client.RefEntite=tbmEntiteEntite.RefEntiteRtt and transporteur.RefEntite=tbmEntiteEntite.RefEntite)"
                        + " 		or (transporteur.RefEntite=tbmEntiteEntite.RefEntite and adresseOrigine.RefEntite=tbmEntiteEntite.RefEntiteRtt)"
                        + " 		or (transporteur.RefEntite=tbmEntiteEntite.RefEntiteRtt and adresseOrigine.RefEntite=tbmEntiteEntite.RefEntite)"
                        + "     left join (select RefCamionType from tbmEntiteCamionType where RefEntite=@refFournisseur) as tbmEntiteCamionType on tblTransport.RefCamionType=tbmEntiteCamionType.RefCamionType"
                        + "     inner join tbmEntiteCamionType as transporteurCamionType on tblTransport.RefCamionType=transporteurCamionType.RefCamionType and transporteurCamionType.RefEntite=transporteur.RefEntite"
                        + "     left join (select RefTransporteur, RefAdresse, RefAdresseClient, RefCamionType, count(*) as NbTransportIdentique from tblCommandeFournisseur where dateadd(day, 180, DDechargement) > getdate() group by RefTransporteur, RefAdresse, RefAdresseClient, RefCamionType) as tr"
                        + "         on transporteur.RefEntite=tr.RefTransporteur and tblParcours.RefAdresseOrigine=tr.RefAdresse and tblParcours.RefAdresseDestination=tr.RefAdresseClient and tblTransport.RefCamionType=tr.RefCamionType"
                        + "     inner join (select distinct RefEntite, Interdit from tbmEntiteProduit where RefProduit=@refProduit) as clientProduit on client.RefEntite=clientProduit.RefEntite "
                        + " where isnull(tblTransport.PUHT,0)!=0 and tblParcours.RefAdresseOrigine=@refAdresseFournisseur"
                        + "     and (tbmEntiteEntite.RefEntite is null or tbmEntiteEntite.RefEntiteRtt is null)"
                        + "     and tblTransport.RefTransporteur in (select distinct RefEntite from VueBL)"
                        + "     and client.Actif=1"
                        + "     and clientProduit.Interdit=0"
                        + "     and transporteur.Actif=1"
                        + "     and tbmEntiteCamionType.RefCamionType is null";
                    //Contrat RI or not
                    if (refContrat > 0)
                    {
                        sqlStr += " and client.RefEntite in (select RefEntite from tbmContratEntite where RefContrat=@RefContrat)";
                    }
                    //Type de camion de la commande
                    if (filterCamionTypes != "")
                    {
                        sqlStr += " and tblTransport.RefCamionType in (";
                        sqlStr += Utils.Utils.CreateSQLParametersFromString("refCamionType", filterCamionTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                        sqlStr += ")";
                    }
                    //Client de la commande
                    if (int.TryParse(refClient, out refC))
                    {
                        if (refC != 0)
                        {
                            cmd.Parameters.Add("@refClient", SqlDbType.Int).Value = refC;
                            sqlStr += "   and client.RefEntite=@refClient";
                        }
                    }
                    //Transporteur de la commande
                    if (int.TryParse(refTransporteur, out refT))
                    {
                        if (refT != 0)
                        {
                            cmd.Parameters.Add("@refTransporteur", SqlDbType.Int).Value = refT;
                            sqlStr += "   and transporteur.RefEntite=@refTransporteur";
                        }
                    }
                    if (filterVilleArrivees != "")
                    {
                        sqlStr += " and tblAdresse.Ville in (";
                        sqlStr += Utils.Utils.CreateSQLParametersFromString("tblAdresse", filterVilleArrivees, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                        sqlStr += ")";
                    }
                    sqlStr += " order by tblTransport.PUHT";
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
        /// <summary>
        /// GET: api/commandefournisseurs/getpictures
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of CommandeFournisseurFichier.</returns>
        [HttpGet("getpictures")]
        public IActionResult GetPictures()
        {
            string refCommandeFournisseur = Request.Headers["refCommandeFournisseur"].ToString();
            int refC = 0;
            //Check mandatory parameters
            if (int.TryParse(refCommandeFournisseur, out refC))
            {
                var pictures = DbContext.CommandeFournisseurFichiers.Where(el => el.RefCommandeFournisseur == refC && el.Miniature != null).ToArray();
                //Return Json
                return new JsonResult(
                    _mapper.Map<CommandeFournisseurFichier[], CommandeFournisseurFichierMediumViewModel[]>(pictures),
                    JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// GET: api/commandefournisseurs/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Calulated values for NbBalle, PoidsChargement and PoidsReparti</returns>
        [HttpGet("getnbballe")]
        public IActionResult GetNbBalle()
        {
            string refProduit = Request.Headers["refProduit"].ToString();
            string refEntite = Request.Headers["refEntite"].ToString();
            int refP = 0;
            int refE = 0;
            SqlCommand cmd = new();
            DataSet dS = new();
            //Check mandatory parameters
            if (int.TryParse(refEntite, out refE) && int.TryParse(refProduit, out refP))
            {

                cmd.Parameters.Add("@refProduit", SqlDbType.Int).Value = refP;
                cmd.Parameters.Add("@refEntite", SqlDbType.Int).Value = refE;
                //Chaine SQL
                string sqlStr = "select isnull(avg(PoidsDechargement),0) as PoidsDechargement, isnull(avg(NbBalleDechargement),0) as NbBalleDechargement"
                    + " from tblCommandeFournisseur"
                    + " where dbo.CommandeMixte(NumeroAffretement)=0 and DDechargement>dateadd(year,-1,getdate()) and RefProduit=@refProduit and RefEntite=@refEntite";
                cmd.CommandText = sqlStr;
                if (dS.Tables.Count > 0) { dS.Tables.Clear(); }
                using (DbConnection sqlConn = DbContext.Database.GetDbConnection())
                {
                    sqlConn.Open();
                    cmd.Connection = (SqlConnection)sqlConn;
                    SqlDataAdapter dA = new(cmd);
                    dA.Fill(dS);
                    dA.Dispose();
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
        /// GET: api/commandefournisseur/hasrepartition
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Check if the CommandeFournisseur is linked to a Repartition</returns>
        [HttpGet("hasrepartition")]
        public IActionResult HasRepartition()
        {
            string refCommandeFournisseur = Request.Headers["refCommandeFournisseur"].ToString();
            bool r = false;
            int refC = 0;
            //Check mandatory parameters
            if (int.TryParse(refCommandeFournisseur, out refC))
            {
                CommandeFournisseur cF = DbContext.CommandeFournisseurs.Where(i => i.RefCommandeFournisseur == refC).FirstOrDefault();
                if (cF != null && cF.DDechargement != null)
                {
                    r = (DbContext.Repartitions
                        .Where(i => (i.RefCommandeFournisseur == cF.RefCommandeFournisseur
                            || (i.RefFournisseur == cF.RefEntite && i.RefProduit == cF.RefProduit && i.D.Value.Month == cF.DDechargement.Value.Month && i.D.Value.Year == cF.DDechargement.Value.Year)))
                        .Any());
                }
                //Return Json
                return new JsonResult(r, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// GET: api/commandefournisseur/hasfichecontrole
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Check if the CommandeFournisseur is linked to a FicheControle</returns>
        [HttpGet("hasfichecontrole")]
        public IActionResult HasFicheControle()
        {
            string refCommandeFournisseur = Request.Headers["refCommandeFournisseur"].ToString();
            bool r = false;
            int refC = 0;
            //Check mandatory parameters
            if (int.TryParse(refCommandeFournisseur, out refC))
            {
                var cF = DbContext.FicheControles.Where(i => i.RefCommandeFournisseur == refC).FirstOrDefault();
                r = (cF != null);
                //Return Json
                return new JsonResult(r, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// GET: api/commandefournisseur/issimilar
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Check if similar CommandeFournisseur are available before</returns>
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
                CommandeFournisseur cF = null;
                try { cF = DbContext.CommandeFournisseurs.Find(refCF); } catch { }
                //Exit if no result
                if (cF == null) { return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711))); }
                //Process if ok
                var cFs = DbContext.CommandeFournisseurs
                    .Include(i => i.CommandeFournisseurStatut)
                    .Where(i =>
                    i.RefCommandeFournisseur != refCF && i.RefEntite == cF.Entite.RefEntite && i.RefProduit == cF.RefProduit
                    && i.D <= D && i.DDechargementPrevue == null)
                    .Select(i => new { i.NumeroCommande, i.D, i.CommandeFournisseurStatut.Libelle })
                    .ToArray();
                //Return Json
                return new JsonResult(cFs, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// GET: api/commandefournisseurs/ismixte
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Check if the CommandeFournisseur is Mixte</returns>
        [HttpGet("ismixte")]
        public IActionResult IsMixte()
        {
            string refCommandeFournisseur = Request.Headers["refCommandeFournisseur"].ToString();
            bool r = false;
            int refC = 0;
            //Check mandatory parameters
            if (int.TryParse(refCommandeFournisseur, out refC))
            {
                CommandeFournisseur cF = DbContext.CommandeFournisseurs.Where(i => i.RefCommandeFournisseur == refC).FirstOrDefault();
                r = (DbContext.CommandeFournisseurs
                    .Where(i => (i.NumeroAffretement == cF.NumeroAffretement))
                    .Count() > 1);
                //Return Json
                return new JsonResult(r, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// GET: api/commandefournisseurs/unmarkmixtes
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Remove link with other Mixte CommandeFournisseur</returns>
        [HttpGet("unmarkmixte")]
        public IActionResult UnmarkMixte()
        {
            string numeroCommande = Request.Headers["numeroCommande"].ToString();
            int nC = 0;
            //Check mandatory parameters
            if (int.TryParse(numeroCommande, out nC))
            {
                var r = DbContext.CommandeFournisseurs.Where(i => i.NumeroAffretement == nC).ToList();
                if (r.Count > 0)
                {
                    foreach (CommandeFournisseur c in r)
                    {
                        c.NumeroAffretement = c.NumeroCommande;
                        c.OrdreAffretement = 1;
                        c.Imprime = false;
                    }
                    DbContext.SaveChanges();
                }
                //Return Json
                return new JsonResult(true, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// GET: api/commandefournisseurs/unlock
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Remove link with other Mixte CommandeFournisseur</returns>
        [HttpGet("unlock")]
        public IActionResult Unlock()
        {
            string refCommandeFournisseur = Request.Headers["refCommandeFournisseur"].ToString();
            int rCF = 0;
            //Check mandatory parameters
            if (int.TryParse(refCommandeFournisseur, out rCF))
            {
                var r = DbContext.Verrouillages.Where(i => i.Donnee== "RefCommandeFournisseur" && i.RefDonnee == rCF).ExecuteDelete();
                //Return Json
                return new JsonResult(true, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// Create new e-mail based on a model
        /// </summary>
        /// <param name="model">The CommandeFournisseurViewModel containing the data to update</param>
        [HttpGet("createemail")]
        public IActionResult CreateEmail()
        {
            //Init error message
            string err = "";
            string refCommandeFournisseur = Request.Headers["refCommandeFournisseur"].ToString();
            string emailType = Request.Headers["emailType"].ToString();
            int refCF = 0;
            int.TryParse(refCommandeFournisseur, out refCF);
            if (refCF != 0)
            {
                // retrieve the commandeFournisseur
                var cF = DbContext.CommandeFournisseurs
                        .Where(q => q.RefCommandeFournisseur == refCF)
                        .Include(r => r.CommandeFournisseurFichiers)
                        .FirstOrDefault();

                // handle requests asking for non-existing commandeFournisseur
                if (cF == null)
                {
                    return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
                }
                ContactAdresse cA;
                string body = "";
                //Search for ParamEmail
                var paramEmail = DbContext.ParamEmails.Where(el => el.Actif && el.Defaut).FirstOrDefault();
                if (paramEmail != null)
                {
                    Email email = new();
                    IMail msg;
                    Data.Action action;
                    MailBuilder mBuilder;
                    //Init
                    email.Entrant = false;
                    email.ParamEmail = paramEmail;
                    email.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                    msg = email.Message;
                    msg.From.Add(new MailBox("e-valorplast@valorplast.com", "Valorplast"));
                    //Create e-mail
                    switch (emailType)
                    {
                        case "AnomalieChargement":
                            //Create e-mail
                            cA = cF.ContactAdresse;
                            body = "";
                            DbContext.Emails.Add(email);
                            //Création du corps du message
                            body = email.ParamEmail.EnTeteEmail + "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                            body += WebUtility.HtmlEncode("Bonjour,") + "<br/>" + "<br/>"
                                + WebUtility.HtmlEncode("Vous nous avez fait part d’une anomalie concernant le transport référencé en objet ci-dessus et ayant pour motif suivant : " + cF.MotifAnomalieChargement.Libelle + ".") + "<br/>" + "<br/>" + "<br/>" + "<br/>"
                                + WebUtility.HtmlEncode("Cordialement,") + "<br/>" + "<br/>"
                                + WebUtility.HtmlEncode("Le Service Logistique") + "<br/>"
                                + WebUtility.HtmlEncode("VALORPLAST") + "<br/>" + "<br/>"
                                + WebUtility.HtmlEncode("Tél. : " + Utils.Utils.GetParametre(9, DbContext).ValeurTexte) + "<br/>";
                            body += "</span>" + (string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.PiedEmail) ? email.ParamEmail.PiedEmail : CurrentContext.ConnectedUtilisateur.PiedEmail);
                            mBuilder = msg.ToBuilder();
                            mBuilder.Html = body;
                            msg = mBuilder.Create();
                            //Destinataire
                            msg.To.Add(new MailBox(cA.Email));
                            //Objet
                            msg.Subject = "Valorplast : anomalie CDT " + cF.NumeroCommande.ToString();
                            email.Message = msg;
                            //Création de l'action liée
                            action = new Data.Action();
                            action.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                            DbContext.Actions.Add(action);
                            email.Action = action;
                            email.Action.RefContactAdresse = cA.RefContactAdresse;
                            email.Action.DAction = DateTime.Now;
                            email.Action.RefEntite = cF.RefEntite;
                            email.Action.RefFormeContact = 2;
                            email.Action.Libelle = "Email envoyé au CDT suite à une remonté d'anomalie " + cF.NumeroCommande.ToString() + " (non envoyé)";
                            email.currentCulture = CurrentContext.CurrentCulture;
                            DbContext.SaveChanges();
                            DbContext.Entry(email).Reload();
                            return new JsonResult(email.RefEmail,
                                JsonSettings);
                        case "AnomalieTransporteur":
                            //Create e-mail
                            cA = cF.TransporteurContactAdresse;
                            body = "";
                            DbContext.Emails.Add(email);
                            //Création du corps du message
                            body = email.ParamEmail.EnTeteEmail + "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                            body += WebUtility.HtmlEncode("Bonjour,") + "<br/>" + "<br/>"
                                + WebUtility.HtmlEncode("Vous nous avez fait part d’une anomalie concernant le transport référencé en objet ci-dessus et ayant pour motif suivant : " + cF.MotifAnomalieTransporteur.Libelle + ".") + "<br/>" + "<br/>" + "<br/>" + "<br/>"
                                + WebUtility.HtmlEncode("Cordialement,") + "<br/>" + "<br/>"
                                + WebUtility.HtmlEncode("Le Service Logistique") + "<br/>"
                                + WebUtility.HtmlEncode("VALORPLAST") + "<br/>" + "<br/>"
                                + WebUtility.HtmlEncode("Tél. : " + Utils.Utils.GetParametre(9, DbContext).ValeurTexte) + "<br/>";
                            body += "</span>" + (string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.PiedEmail) ? email.ParamEmail.PiedEmail : CurrentContext.ConnectedUtilisateur.PiedEmail);
                            mBuilder = msg.ToBuilder();
                            mBuilder.Html = body;
                            msg = mBuilder.Create();
                            //Destinataire
                            msg.To.Add(new MailBox(cA.Email));
                            //Objet
                            msg.Subject = "Valorplast : anomalie transporteur " + cF.NumeroCommande.ToString();
                            email.Message = msg;
                            //Création de l'action liée
                            action = new Data.Action();
                            action.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                            DbContext.Actions.Add(action);
                            email.Action = action;
                            email.Action.RefContactAdresse = cA.RefContactAdresse;
                            email.Action.DAction = DateTime.Now;
                            email.Action.RefEntite = cF.Transporteur.RefEntite;
                            email.Action.RefFormeContact = 2;
                            email.Action.Libelle = "Email envoyé au transporteur suite à une remonté d'anomalie " + cF.NumeroCommande.ToString() + " (non envoyé)";
                            email.currentCulture = CurrentContext.CurrentCulture;
                            DbContext.SaveChanges();
                            DbContext.Entry(email).Reload();
                            return new JsonResult(email.RefEmail,
                                JsonSettings);
                        case "AnomalieClient":
                            //Search for ContactAdresse
                            cA = DbContext.ContactAdresses.Where(el => el.RefAdresse == cF.AdresseClient.RefAdresse
                            && el.ContactAdresseContactAdresseProcesss.Any(related => related.RefContactAdresseProcess == (int)Enumerations.ContactAdresseProcess.BonDeLivraison))
                                .FirstOrDefault();
                            if (cA != null)
                            {
                                //Create e-mail
                                body = "";
                                DbContext.Emails.Add(email);
                                //Création du corps du message
                                body = email.ParamEmail.EnTeteEmail + "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                                body += WebUtility.HtmlEncode("Bonjour,") + "<br/>" + "<br/>"
                                    + WebUtility.HtmlEncode("Vous nous avez fait part d’une anomalie concernant le transport référencé en objet ci-dessus et ayant pour motif suivant : " + cF.MotifAnomalieClient.Libelle + ".") + "<br/>" + "<br/>" + "<br/>" + "<br/>"
                                    + WebUtility.HtmlEncode("Cordialement,") + "<br/>" + "<br/>"
                                    + WebUtility.HtmlEncode("Le Service Logistique") + "<br/>"
                                    + WebUtility.HtmlEncode("VALORPLAST") + "<br/>" + "<br/>"
                                    + WebUtility.HtmlEncode("Tél. : " + Utils.Utils.GetParametre(9, DbContext).ValeurTexte) + "<br/>";
                                body += "</span>" + (string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.PiedEmail) ? email.ParamEmail.PiedEmail : CurrentContext.ConnectedUtilisateur.PiedEmail);
                                mBuilder = msg.ToBuilder();
                                mBuilder.Html = body;
                                msg = mBuilder.Create();
                                //Destinataire
                                msg.To.Add(new MailBox(cA.Email));
                                //Objet
                                msg.Subject = "Valorplast : anomalie client " + cF.NumeroCommande.ToString();
                                email.Message = msg;
                                //Création de l'action liée
                                action = new Data.Action();
                                action.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                                DbContext.Actions.Add(action);
                                email.Action = action;
                                email.Action.RefContactAdresse = cA.RefContactAdresse;
                                email.Action.DAction = DateTime.Now;
                                email.Action.RefEntite = cF.AdresseClient.RefEntite;
                                email.Action.RefFormeContact = 2;
                                email.Action.Libelle = "Email envoyé au client suite à une remonté d'anomalie " + cF.NumeroCommande.ToString() + " (non envoyé)";
                                email.currentCulture = CurrentContext.CurrentCulture;
                                DbContext.SaveChanges();
                                DbContext.Entry(email).Reload();
                                return new JsonResult(email.RefEmail,
                                    JsonSettings);
                            }
                            break;
                        case "BLChargement":
                        case "BLClient":
                            //Search for ContactAdresse
                            if (emailType == "BLChargement") { cA = new ContactAdresse() { Email = cF.Email }; }
                            else
                            {
                                cA = DbContext.ContactAdresses.Where(el => el.RefAdresse == cF.AdresseClient.RefAdresse
                                && el.ContactAdresseContactAdresseProcesss.Any(related => related.RefContactAdresseProcess == (int)Enumerations.ContactAdresseProcess.BonDeLivraison))
                                    .FirstOrDefault();
                            }
                            if (cA != null)
                            {
                                //Create e-mail
                                body = "";
                                DbContext.Emails.Add(email);
                                email.Entrant = false;
                                email.ParamEmail = paramEmail;
                                email.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                                msg = email.Message;
                                msg.Subject = "Valorplast : bordereau de livraison " + cF.NumeroCommande.ToString();
                                //Création du corps du message
                                body = email.ParamEmail.EnTeteEmail;
                                if (CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Transporteur.ToString())
                                {
                                    body += "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">"
                                        + WebUtility.HtmlEncode("Bonjour,") + "<br/>" + "<br/>"
                                        + WebUtility.HtmlEncode("Veuillez trouver ci-joint le bordereau de livraison Valorplast vous confirmant le prochain enlèvement de balles d’emballages plastiques dans votre centre de tri.") + "<br/>" + "<br/>"
                                        + WebUtility.HtmlEncode("Merci de remettre au chauffeur, une copie complétée (nombre de balles, poids, date et signature) lors du chargement.") + "<br/>" + "<br/>"
                                        + WebUtility.HtmlEncode("Nous restons à votre disposition pour toute question complémentaire.") + "<br/>" + "<br/>"
                                        + WebUtility.HtmlEncode("Cordialement,") + "<br/>" + "<br/>"
                                        + WebUtility.HtmlEncode("Le Service Logistique") + "<br/>"
                                        + WebUtility.HtmlEncode("VALORPLAST") + "<br/>" + "<br/>"
                                        + WebUtility.HtmlEncode("Tél. : " + Utils.Utils.GetParametre(9, DbContext).ValeurTexte) + "<br/></span>";
                                }
                                mBuilder = msg.ToBuilder();
                                mBuilder.Html = body;
                                //Attachment
                                MemoryStream mS = ExcelFileManagement.CreateCommandeAffretement(cF.RefCommandeFournisseur, false, cF.RefCommandeFournisseur, "pdf", CurrentContext, DbContext, _env.ContentRootPath);
                                //Ajout du fichier
                                MimeData att1 = mBuilder.AddAttachment(mS.ToArray());
                                att1.FileName = "BL" + cF.NumeroCommande + ".pdf";
                                msg = mBuilder.Create();
                                //Destinataire
                                msg.To.Add(new MailBox(cA.Email));
                                //Objet
                                msg.Subject = "Valorplast : BL " + cF.NumeroCommande.ToString();
                                email.Message = msg;
                                DbContext.SaveChanges();
                                DbContext.Entry(email).Reload();
                                return new JsonResult(email.RefEmail,
                                    JsonSettings);
                            }
                            break;
                    }
                }
            }
            //Error
            return Conflict(new ConflictError(err));
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref CommandeFournisseur dataModel, CommandeFournisseurViewModel viewModel
             , ref bool dirtyAnomalieChargement, ref bool dirtyAnomalieClient, ref bool dirtyAnomalieTransporteur, ref bool dirtyTransporteur, ref bool dirtyCDT
            , ref bool newDPrevue, ref bool newReceptionTransporteur, ref bool newReceptionClient)
        {
            //Record data changes
            if (viewModel.DChargementPrevue != null
                && viewModel.DDechargementPrevue != null
                && viewModel.AdresseClient?.RefAdresse != null
                && viewModel.Transporteur?.RefEntite != null)
            {
                if (
                    (
                        (
                            viewModel.DChargementPrevue != dataModel.DChargementPrevue
                            || viewModel.DDechargementPrevue != dataModel.DDechargementPrevue
                            || viewModel.AdresseClient?.RefAdresse != dataModel.AdresseClient?.RefAdresse
                            || viewModel.Transporteur?.RefEntite != dataModel.Transporteur?.RefEntite
                        )
                        && viewModel.ValideDPrevues == true
                    )
                    || (dataModel.ValideDPrevues == false && viewModel.ValideDPrevues == true)
                    )
                {
                    newDPrevue = true;
                }
            }
            if (viewModel.DChargement != null && viewModel.DDechargement != null && viewModel.NbBalleDechargement != 0 && viewModel.PoidsDechargement != 0)
            {
                if (viewModel.DChargement != dataModel.DChargement
                    || viewModel.DDechargement != dataModel.DDechargement)
                {
                    newReceptionTransporteur = true;
                }
                if (viewModel.DDechargement != dataModel.DDechargement
                    || viewModel.PoidsDechargement != dataModel.PoidsDechargement
                    || viewModel.NbBalleDechargement != dataModel.NbBalleDechargement)
                {
                    newReceptionClient = true;
                }
            }
            //Store original data
            var chargementAnnuleOr = dataModel.ChargementAnnule;
            var chargementEffectueOr = dataModel.ChargementEffectue;
            var refusCamionOr = dataModel.RefusCamion;
            //Process
            dataModel.NumeroCommande = viewModel.NumeroCommande ?? 0;
            dataModel.NumeroAffretement = viewModel.NumeroAffretement ?? 0;
            //Ordre calculated automatically if 0
            if (viewModel.OrdreAffretement == 0)
            {
                dataModel.OrdreAffretement = DbContext.CommandeFournisseurs.Where(i => i.NumeroAffretement == viewModel.NumeroAffretement).Max(i => i.OrdreAffretement) + 1;
            }
            else { dataModel.OrdreAffretement = viewModel.OrdreAffretement; }
            dataModel.RefEntite = viewModel.Entite.RefEntite;
            dataModel.Horaires = Utils.Utils.SetEmptyStringToNull(viewModel.Horaires);
            dataModel.RefAdresse = viewModel.Adresse?.RefAdresse;
            dataModel.Libelle = Utils.Utils.SetEmptyStringToNull(viewModel.Libelle);
            dataModel.Adr1 = Utils.Utils.SetEmptyStringToNull(viewModel.Adr1);
            dataModel.Adr2 = Utils.Utils.SetEmptyStringToNull(viewModel.Adr2);
            dataModel.CodePostal = Utils.Utils.SetEmptyStringToNull(viewModel.CodePostal);
            dataModel.Ville = Utils.Utils.SetEmptyStringToNull(viewModel.Ville);
            dataModel.RefPays = viewModel.Pays?.RefPays;
            dataModel.RefContactAdresse = viewModel.ContactAdresse?.RefContactAdresse;
            dataModel.RefCivilite = viewModel.Civilite?.RefCivilite;
            dataModel.Prenom = Utils.Utils.SetEmptyStringToNull(viewModel.Prenom);
            dataModel.Nom = Utils.Utils.SetEmptyStringToNull(viewModel.Nom);
            dataModel.Tel = Utils.Utils.SetEmptyStringToNull(viewModel.Tel);
            dataModel.TelMobile = Utils.Utils.SetEmptyStringToNull(viewModel.TelMobile);
            dataModel.Fax = Utils.Utils.SetEmptyStringToNull(viewModel.Fax);
            dataModel.Email = Utils.Utils.SetEmptyStringToNull(viewModel.Email);
            dataModel.RefProduit = viewModel.Produit.RefProduit;
            dataModel.D = viewModel.D;
            dataModel.DChargementPrevue = viewModel.DChargementPrevue;
            dataModel.HoraireChargementPrevu = Utils.Utils.SetEmptyStringToNull(viewModel.HoraireChargementPrevu);
            dataModel.DChargement = viewModel.DChargement;
            dataModel.DDechargementPrevue = viewModel.DDechargementPrevue;
            dataModel.HoraireDechargementPrevu = Utils.Utils.SetEmptyStringToNull(viewModel.HoraireDechargementPrevu);
            dataModel.DDechargement = viewModel.DDechargement;
            dataModel.PoidsChargement = viewModel.PoidsChargement ?? 0;
            dataModel.NbBalleChargement = viewModel.NbBalleChargement ?? 0;
            dataModel.TicketPeseeChargement = viewModel.TicketPeseeChargement ?? false;
            dataModel.PoidsDechargement = viewModel.PoidsDechargement ?? 0;
            dataModel.NbBalleDechargement = viewModel.NbBalleDechargement ?? 0;
            dataModel.TicketPeseeDechargement = viewModel.TicketPeseeDechargement ?? false;
            dataModel.RefCamionType = viewModel.CamionType?.RefCamionType;
            if (dataModel.RefTransporteur != viewModel.Transporteur?.RefEntite && viewModel.Transporteur != null) { dirtyTransporteur = true; }
            dataModel.RefTransporteur = viewModel.Transporteur?.RefEntite;
            dataModel.RefTransporteurContactAdresse = viewModel.TransporteurContactAdresse?.RefContactAdresse;
            dataModel.PrixTransportHT = viewModel.PrixTransportHT ?? 0;
            dataModel.SurcoutCarburantHT = viewModel.SurcoutCarburantHT ?? 0;
            dataModel.PrixTransportSupplementHT = viewModel.PrixTransportSupplementHT ?? 0;
            dataModel.Km = viewModel.Km ?? 0;
            dataModel.RefAdresseClient = viewModel.AdresseClient?.RefAdresse;
            dataModel.CmtFournisseur = Utils.Utils.SetEmptyStringToNull(viewModel.CmtFournisseur);
            dataModel.CmtClient = Utils.Utils.SetEmptyStringToNull(viewModel.CmtClient);
            dataModel.CmtTransporteur = Utils.Utils.SetEmptyStringToNull(viewModel.CmtTransporteur);
            dataModel.PrixTonneHT = viewModel.PrixTonneHT;
            dataModel.DMoisDechargementPrevu = viewModel.DMoisDechargementPrevu;
            dataModel.Imprime = viewModel.Imprime ?? false;
            dataModel.ExportSAGE = viewModel.ExportSAGE ?? false;
            if (dataModel.ValideDPrevues != viewModel.ValideDPrevues && viewModel.ValideDPrevues == true
                && viewModel.DChargementPrevue >= DateTime.Now.AddDays(-1)) { dirtyCDT = true; }
            dataModel.ValideDPrevues = viewModel.ValideDPrevues ?? false;
            dataModel.RefusCamion = viewModel.RefusCamion ?? false;
            dataModel.PoidsReparti = viewModel.PoidsReparti ?? 0;
            dataModel.DBlocage = viewModel.DBlocage;
            dataModel.RefUtilisateurBlocage = viewModel.UtilisateurBlocage?.RefUtilisateur;
            dataModel.CmtBlocage = Utils.Utils.SetEmptyStringToNull(viewModel.CmtBlocage);
            DateTime? dAff = null;
            if (viewModel.DAffretement != null)
            {
                if (viewModel.DAffretement != dataModel.DAffretement)
                { dAff = DateTime.Now; }
                else
                { dAff = viewModel.DAffretement; }
            }
            dataModel.DAffretement = dAff;
            dataModel.RefCommandeFournisseurStatut = viewModel.CommandeFournisseurStatut?.RefCommandeFournisseurStatut;
            dataModel.CamionComplet = viewModel.CamionComplet;
            dataModel.RefMotifCamionIncomplet = viewModel.MotifCamionIncomplet?.RefMotifCamionIncomplet;
            dataModel.ChargementEffectue = viewModel.ChargementEffectue ?? false;
            dataModel.ChargementAnnule = viewModel.ChargementAnnule ?? false;
            dataModel.CmtChargementAnnule = viewModel.CmtChargementAnnule;
            //Data consistency
            if ((chargementAnnuleOr) == false && dataModel.ChargementAnnule == true)
            {
                dataModel.RefUtilisateurChargementAnnule = CurrentContext.RefUtilisateur;
                dataModel.DChargementAnnule = DateTime.Now;
            }
            if (dataModel.ChargementEffectue == true)
            {
                dataModel.RefUtilisateurChargementAnnule = null;
                dataModel.DChargementAnnule = null;
                dataModel.CmtChargementAnnule = "";
            }
            if (dataModel.ChargementAnnule == true)
            {
                dataModel.ChargementEffectue = false;
                dataModel.DDechargement = null;
                dataModel.PoidsDechargement = 0;
                dataModel.NbBalleDechargement = 0;
                dataModel.PrixTonneHT = 0;
            }
            //Delete Repartition on RefusCamion
            if(refusCamionOr == false && dataModel.RefusCamion == true)
            {
                DbContext.Repartitions.Where(i => i.RefCommandeFournisseur == (int)viewModel.RefCommandeFournisseur).ExecuteDelete();
            }
            //Anomalies
            if (viewModel.MotifAnomalieChargement == null)
            {
                dataModel.DAnomalieChargement = null;
                dataModel.RefMotifAnomalieChargement = null;
                dataModel.CmtAnomalieChargement = null;
                dataModel.DTraitementAnomalieChargement = null;
            }
            else
            {
                if (dataModel.RefMotifAnomalieChargement != viewModel.MotifAnomalieChargement?.RefMotifAnomalieChargement) { dirtyAnomalieChargement = true; }
                if (dataModel.RefMotifAnomalieChargement == null && CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null) { dataModel.DAnomalieChargement = DateTime.Now; }
                dataModel.RefMotifAnomalieChargement = viewModel.MotifAnomalieChargement?.RefMotifAnomalieChargement;
                dataModel.CmtAnomalieChargement = viewModel.CmtAnomalieChargement;
                dataModel.DTraitementAnomalieChargement = (dataModel.DTraitementAnomalieChargement == null && viewModel.DTraitementAnomalieChargement != null ? (DateTime?)DateTime.Now : (viewModel.DTraitementAnomalieChargement == null ? null : dataModel.DTraitementAnomalieChargement));
            }
            if (viewModel.MotifAnomalieClient == null)
            {
                dataModel.DAnomalieClient = null;
                dataModel.RefMotifAnomalieClient = null;
                dataModel.CmtAnomalieClient = null;
                dataModel.DTraitementAnomalieClient = null;
            }
            else
            {
                if (dataModel.RefMotifAnomalieClient != viewModel.MotifAnomalieClient?.RefMotifAnomalieClient) { dirtyAnomalieClient = true; }
                if (dataModel.RefMotifAnomalieClient == null && CurrentContext.ConnectedUtilisateur.RefClient != null) { dataModel.DAnomalieClient = DateTime.Now; }
                dataModel.RefMotifAnomalieClient = viewModel.MotifAnomalieClient?.RefMotifAnomalieClient;
                dataModel.CmtAnomalieClient = viewModel.CmtAnomalieClient;
                dataModel.DTraitementAnomalieClient = (dataModel.DTraitementAnomalieClient == null && viewModel.DTraitementAnomalieClient != null ? (DateTime?)DateTime.Now : (viewModel.DTraitementAnomalieClient == null ? null : dataModel.DTraitementAnomalieClient));
            }
            if (viewModel.MotifAnomalieTransporteur == null)
            {
                dataModel.DAnomalieTransporteur = null;
                dataModel.RefMotifAnomalieTransporteur = null;
                dataModel.CmtAnomalieTransporteur = null;
                dataModel.DTraitementAnomalieTransporteur = null;
            }
            else
            {
                if (dataModel.RefMotifAnomalieTransporteur != viewModel.MotifAnomalieTransporteur?.RefMotifAnomalieTransporteur) { dirtyAnomalieTransporteur = true; ; }
                if (dataModel.RefMotifAnomalieTransporteur == null && CurrentContext.ConnectedUtilisateur.RefTransporteur != null) { dataModel.DAnomalieTransporteur = DateTime.Now; }
                dataModel.RefMotifAnomalieTransporteur = viewModel.MotifAnomalieTransporteur?.RefMotifAnomalieTransporteur;
                dataModel.CmtAnomalieTransporteur = viewModel.CmtAnomalieTransporteur;
                dataModel.DTraitementAnomalieTransporteur = (dataModel.DTraitementAnomalieTransporteur == null && viewModel.DTraitementAnomalieTransporteur != null ? (DateTime?)DateTime.Now : (viewModel.DTraitementAnomalieTransporteur == null ? null : dataModel.DTraitementAnomalieTransporteur));
            }
            dataModel.LotControle = viewModel.LotControle ?? false;
            dataModel.DAnomalieOk = viewModel.DAnomalieOk;
            dataModel.RefUtilisateurAnomalieOk = viewModel.UtilisateurAnomalieOk?.RefUtilisateur;
            dataModel.NonRepartissable = viewModel.NonRepartissable;
            //Files
            //delete files
            if (dataModel.CommandeFournisseurFichiers != null)
            {
                foreach (CommandeFournisseurFichier cmdFF in dataModel.CommandeFournisseurFichiers)
                {
                    if (viewModel.CommandeFournisseurFichiers.Where(el => el.RefCommandeFournisseurFichier == cmdFF.RefCommandeFournisseurFichier).FirstOrDefault() == null)
                    {
                        DbContext.CommandeFournisseurFichiers.Remove(cmdFF);
                    }
                }
            }
        }
        /// <summary>
        /// Edit the CommandeFournisseur with the given {id}
        /// </summary>
        /// <param name="model">The CommandeFournisseurViewModel containing the data to update</param>
        public string SendEmail(CommandeFournisseur cF, ContactAdresse cA, string emailType)
        {
            //Init error message
            string err = CurrentContext.CulturedRessources.GetTextRessource(677);
            string body = "";
            //Search for ParamEmail
            var paramEmail = DbContext.ParamEmails.Where(el => el.Actif && el.Defaut).FirstOrDefault();
            if (paramEmail != null)
            {
                Email email = new();
                IMail msg;
                MailBuilder mBuilder;
                //Init
                email.Entrant = false;
                email.ParamEmail = paramEmail;
                email.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                msg = email.Message;
                msg.From.Add(new MailBox("e-valorplast@valorplast.com", "Valorplast"));
                //Create e-mail
                switch (emailType)
                {
                    case "AnomalieChargement":
                        //Create e-mail
                        DbContext.Emails.Add(email);
                        msg.Subject = "Valorplast : nouvelle anomalie CDT " + cF.NumeroCommande.ToString();
                        //Création du corps du message
                        body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                        body += "<hr/>"
                            + "<div align=\"center\">"
                            + "<strong>Cet email a &eacute;t&eacute; envoy&eacute; automatiquement, ne pas y r&eacute;pondre.</strong>"
                            + "<hr/>"
                            + "</div>"
                            + "<p>" + WebUtility.HtmlEncode("Une nouvelle anomalie vient d'être saisie pour la commande n°" + cF.NumeroCommande) + "<br/>";
                        body += WebUtility.HtmlEncode(" - CDT : " + cF.Entite.Libelle) + "<br/>";
                        body += WebUtility.HtmlEncode(" - Motif : " + cF.MotifAnomalieChargement.Libelle) + "<br/>";
                        body += WebUtility.HtmlEncode(" - Commentaires : " + cF.CmtAnomalieChargement) + "<br/>";
                        body += "<br/></p>"
                            + "<hr/>"
                            + "<div align=\"center\">"
                            + "<strong>Cet email a &eacute;t&eacute; envoy&eacute; automatiquement par e-Valorplast.</strong>"
                            + "</div></span>";
                        mBuilder = msg.ToBuilder();
                        mBuilder.Html = body;
                        msg = mBuilder.Create();
                        //Destinataire
                        msg.To.Add(new MailBox(Utils.Utils.GetParametre(11, DbContext).ValeurTexte, "Logistique Valorplast"));
                        //Objet
                        email.Message = msg;
                        email.currentCulture = CurrentContext.CurrentCulture;
                        DbContext.SaveChanges();
                        DbContext.Entry(email).Reload();
                        return EmailUtils.Send(email, CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                    case "AnomalieClient":
                        Entite e = DbContext.Entites.Where(el => el.RefEntite == cF.AdresseClient.RefEntite).FirstOrDefault();
                        //Create e-mail
                        DbContext.Emails.Add(email);
                        msg.Subject = "Valorplast : nouvelle anomalie client " + cF.NumeroCommande.ToString();
                        //Création du corps du message
                        body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                        body += "<hr/>"
                            + "<div align=\"center\">"
                            + "<strong>Cet email a &eacute;t&eacute; envoy&eacute; automatiquement, ne pas y r&eacute;pondre.</strong>"
                            + "<hr/>"
                            + "</div>"
                            + "<p>" + WebUtility.HtmlEncode("Une nouvelle anomalie vient d'être saisie pour la commande n°" + cF.NumeroCommande) + "<br/>";
                        body += WebUtility.HtmlEncode(" - Client : " + e?.Libelle) + "<br/>";
                        body += WebUtility.HtmlEncode(" - Motif : " + cF.MotifAnomalieClient.Libelle) + "<br/>";
                        body += WebUtility.HtmlEncode(" - Commentaires : " + cF.CmtAnomalieClient) + "<br/>";
                        body += "<br/></p>"
                            + "<hr/>"
                            + "<div align=\"center\">"
                            + "<strong>Cet email a &eacute;t&eacute; envoy&eacute; automatiquement par e-Valorplast.</strong>"
                            + "</div></span>";
                        mBuilder = msg.ToBuilder();
                        mBuilder.Html = body;
                        msg = mBuilder.Create();
                        //Destinataire
                        msg.To.Add(new MailBox(Utils.Utils.GetParametre(11, DbContext).ValeurTexte, "Logistique Valorplast"));
                        //Objet
                        email.Message = msg;
                        email.currentCulture = CurrentContext.CurrentCulture;
                        DbContext.SaveChanges();
                        DbContext.Entry(email).Reload();
                        return EmailUtils.Send(email, CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                    case "AnomalieTransporteur":
                        //Create e-mail
                        DbContext.Emails.Add(email);
                        msg.Subject = "Valorplast : nouvelle anomalie transporteur " + cF.NumeroCommande.ToString();
                        //Création du corps du message
                        body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                        body += "<hr/>"
                            + "<div align=\"center\">"
                            + "<strong>Cet email a &eacute;t&eacute; envoy&eacute; automatiquement, ne pas y r&eacute;pondre.</strong>"
                            + "<hr/>"
                            + "</div>"
                            + "<p>" + WebUtility.HtmlEncode("Une nouvelle anomalie vient d'être saisie pour la commande n°" + cF.NumeroCommande) + "<br/>";
                        body += WebUtility.HtmlEncode(" - Transporteur : " + cF.Transporteur.Libelle) + "<br/>";
                        body += WebUtility.HtmlEncode(" - Motif : " + cF.MotifAnomalieTransporteur.Libelle) + "<br/>";
                        body += WebUtility.HtmlEncode(" - Commentaires : " + cF.CmtAnomalieTransporteur) + "<br/>";
                        //Suite
                        body += "<br/></p>"
                            + "<hr/>"
                            + "<div align=\"center\">"
                            + "<strong>Cet email a &eacute;t&eacute; envoy&eacute; automatiquement par e-Valorplast.</strong>"
                            + "</div></span>";
                        mBuilder = msg.ToBuilder();
                        mBuilder.Html = body;
                        msg = mBuilder.Create();
                        //Destinataire
                        msg.To.Add(new MailBox(Utils.Utils.GetParametre(11, DbContext).ValeurTexte, "Logistique Valorplast"));
                        //Objet
                        email.Message = msg;
                        email.currentCulture = CurrentContext.CurrentCulture;
                        DbContext.SaveChanges();
                        DbContext.Entry(email).Reload();
                        return EmailUtils.Send(email, CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                    case "NouveauTransport":
                        if (!string.IsNullOrEmpty(cF.TransporteurContactAdresse.Email))
                        {
                            //Create e-mail
                            DbContext.Emails.Add(email);
                            email.Message.Subject = "Valorplast : nouvelle commande d'affrètement " + cF.NumeroAffretement.ToString() + " - new delivery order " + cF.NumeroAffretement.ToString();
                            //Limite d'exclusivité
                            DateTime dLim = Convert.ToDateTime(Utils.Utils.DbScalar("select dbo.ProchainJourTravaille(getdate(),2)", DbContext.Database.GetDbConnection()));
                            //Création du corps du message  
                            body = "<hr/>"
                                + "<div align=\"center\" style=\"font-family:Gill Sans MT; font-size:11pt;\">"
                                + "<strong>" + WebUtility.HtmlEncode("Cet email a été envoyé automatiquement, ne pas y répondre - This email have been sent automatically, do not reply.") + "</strong></div>"
                                + "<div style=\"font-family:Gill Sans MT; font-size:11pt;\">"
                                + "<hr/>";
                            body += WebUtility.HtmlEncode("Madame, Monsieur,") + "<br/><br/>";
                            body += WebUtility.HtmlEncode("Nous vous informons que nous venons de mettre à votre disposition, sur votre espace e-Valorplast, une commande d’affrètement n°" + cF.NumeroAffretement.ToString() + " au départ de " + cF.Adresse.Ville + "  " + cF.Adresse.CodePostal + "  " + cF.Adresse.Pays.LibelleCourt + " et à destination de " + cF.AdresseClient.Ville + "  " + cF.AdresseClient.CodePostal + "  " + cF.AdresseClient.Pays.LibelleCourt + ".") + "<br/><br/>";
                            body += WebUtility.HtmlEncode("Nous vous rappelons que ce transport vous est réservé jusqu’au " + dLim.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("fr-FR")) + " minuit et que passé ce délai, il sera à la vue de tout le monde dans le menu « Bourse d'affrètements Valorplast » et qu’il pourra vous être retiré à tous moments sans avertissement.") + "<br/><br/>";
                            //Ajout du commentaire spécifique produit, le cas échéant
                            if (!string.IsNullOrWhiteSpace(cF.Produit.CmtFournisseur))
                            {
                                body += WebUtility.HtmlEncode("*** INFORMATIONS IMPORTANTES ***") + "<br/>";
                                body += WebUtility.HtmlEncode(cF.Produit.CmtFournisseur) + "<br/><br/>";
                            }
                            body += WebUtility.HtmlEncode("Si vous n’êtes pas intéressé par ce transport, merci de nous le signaler dans les plus brefs délais.") + "<br/><br/>";
                            body += WebUtility.HtmlEncode("Cordialement,") + "<br/><br/><hr/>";
                            body += WebUtility.HtmlEncode("Dear Mrs/M,") + "<br/><br/>";
                            body += WebUtility.HtmlEncode("The new delivery order " + cF.NumeroAffretement.ToString() + " is available on e-Valorplast.") + "<br/>";
                            body += WebUtility.HtmlEncode(cF.Adresse.Ville + "  " + cF.Adresse.CodePostal + "  " + cF.Adresse.Pays.LibelleCourt + " -> " + cF.AdresseClient.Ville + "  " + cF.AdresseClient.CodePostal + "  " + cF.AdresseClient.Pays.LibelleCourt + ".") + "<br/>";
                            body += WebUtility.HtmlEncode("Remember that this order is reserved for you until " + dLim.ToString("dd", CultureInfo.CreateSpecificCulture("en-GB")) + " of " + dLim.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("en-GB")) + " midnight. After that, the delivery order will be accessible for everyone in e-Valorplast, and then, may be assigned to someone else.") + "<br/><br/>";
                            //Ajout du commentaire spécifique produit, le cas échéant
                            if (!string.IsNullOrWhiteSpace(cF.Produit.CmtFournisseur))
                            {
                                body += WebUtility.HtmlEncode("*** IMPORTANT INFORMATION ***") + "<br/>";
                                body += WebUtility.HtmlEncode(cF.Produit.CmtFournisseur) + "<br/><br/>";
                            }
                            body += WebUtility.HtmlEncode("If you are not interested for doing this delivery, please let us know asap.") + "<br/><br/>";
                            body += WebUtility.HtmlEncode("Regards,") + "<br/>";
                            //Suite
                            body += "<br/></p>"
                                + "<hr/>"
                                + "<strong>" + WebUtility.HtmlEncode("Le service Logistique VALORPLAST - VALORPLAST logistics department") + "</strong><br/>"
                                + WebUtility.HtmlEncode("Tél - phone : " + Utils.Utils.GetParametre(9, DbContext).ValeurTexte) + "<br/>"
                                + "</div>";
                            mBuilder = msg.ToBuilder();
                            mBuilder.Html = body;
                            msg = mBuilder.Create();
                            //Destinataire
                            msg.To.Add(new MailBox(cF.TransporteurContactAdresse.Email));
                            //Objet
                            email.Message = msg;
                            email.currentCulture = CurrentContext.CurrentCulture;
                            DbContext.SaveChanges();
                            DbContext.Entry(email).Reload();
                            return EmailUtils.Send(email, CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        }
                        else
                        {
                            err = "Email de notification transporteur : erreur d'envoi car aucun email de destinataire";
                        }
                        break;
                    case "NouvelEnlevement":
                        if (!string.IsNullOrEmpty(cF.Email))
                        {
                            //Create e-mail
                            DbContext.Emails.Add(email);
                            email.Message.Subject = "Valorplast : nouvelle date de chargement " + cF.NumeroAffretement.ToString();
                            //Création du corps du message  
                            body = "<hr/>"
                                + "<div align=\"center\" style=\"font-family:Gill Sans MT; font-size:11pt;\">"
                                + "<strong>" + WebUtility.HtmlEncode("Cet email a été envoyé automatiquement, ne pas y répondre.") + "</strong></div>"
                                + "<div style=\"font-family:Gill Sans MT; font-size:11pt;\">"
                                + "<hr/>";
                            body += WebUtility.HtmlEncode("Madame, Monsieur,") + "<br/><br/>";
                            body += WebUtility.HtmlEncode("Vous avez une nouvelle date de chargement qui a été confirmée et que vous pouvez consulter dans votre espace « Logistique » sur e-Valorplast.") + " <br/><br/>";
                            body += WebUtility.HtmlEncode("N° de commande " + cF.NumeroCommande) + "<br/><br/>";
                            body += WebUtility.HtmlEncode("Produit " + cF.Produit.Libelle) + "<br/><br/>";
                            body += WebUtility.HtmlEncode("Disponible le " + ((DateTime)cF.D).ToString("dd/MM/yyyy")) + "<br/><br/>";
                            body += WebUtility.HtmlEncode("Merci de vérifier que vous aurez le nombre de balles suffisant pour un chargement complet et que le chargement pourra bien être effectué dans les 2 heures maximum qui suivent l’arrivée du camion.") + "<br/><br/>";
                            body += WebUtility.HtmlEncode("Attention : toute annulation ou modification doit se faire 48 heures maximum avant la date de chargement annoncée, à l’adresse e-mail " + Utils.Utils.GetParametre(11, DbContext).ValeurTexte + ".") + "<br/><br/>";
                            //Ajout du commentaire spécifique produit, le cas échéant
                            if(!string.IsNullOrWhiteSpace(cF.Produit.CmtFournisseur))
                            {
                                body += WebUtility.HtmlEncode("*** INFORMATIONS IMPORTANTES ***") + "<br/>";
                                body += WebUtility.HtmlEncode(cF.Produit.CmtFournisseur) + "<br/><br/>";
                            }
                            body += WebUtility.HtmlEncode("Cordialement,") + "<br/><br/>";
                            //Suite
                            body += "<br/></p>"
                                + "<hr/>"
                                + "<strong>" + WebUtility.HtmlEncode("Le service Logistique VALORPLAST") + "</strong><br/>"
                                + WebUtility.HtmlEncode("Tél : " + Utils.Utils.GetParametre(9, DbContext).ValeurTexte) + "<br/>"
                                + "</div>";
                            //+ "<img alt='Valorplast' src=';
                            mBuilder = msg.ToBuilder();
                            mBuilder.Html = body;
                            msg = mBuilder.Create();
                            //Destinataire
                            msg.To.Add(new MailBox(cF.Email));
                            //Objet
                            email.Message = msg;
                            email.currentCulture = CurrentContext.CurrentCulture;
                            DbContext.SaveChanges();
                            DbContext.Entry(email).Reload();
                            return EmailUtils.Send(email, CurrentContext.RefUtilisateur, DbContext, CurrentContext, Configuration);
                        }
                        else
                        {
                            err = "Email de notification transporteur : erreur d'envoi car aucun email de destinataire";
                        }
                        break;
                }
            }
            //Error
            return err;
        }
        #endregion
    }
}

