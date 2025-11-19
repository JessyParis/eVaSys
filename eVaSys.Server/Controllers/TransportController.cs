/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/06/2018
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class TransportController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public TransportController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/transport/{id}
        /// Retrieves the Transport with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Transport</param>
        /// <returns>the Transport with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Transport transport = null;
            //Get or create transport
            if (id != 0)
            {
                transport = DbContext.Transports
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefTransport == id).FirstOrDefault();
            }
            else
            {
                int refAdresseOrigine = System.Convert.ToInt32(Request.Headers["refAdresseOrigine"]);
                int refAdresseDestination = System.Convert.ToInt32(Request.Headers["refAdresseDestination"]);
                int refTransporteur = System.Convert.ToInt32(Request.Headers["refTransporteur"]);
                int refCamionType = System.Convert.ToInt32(Request.Headers["refCamionType"]);
                //Looking for parcours
                Parcours parcours = DbContext.Parcourss.Where(q => q.RefAdresseOrigine == refAdresseOrigine && q.RefAdresseDestination == refAdresseDestination).FirstOrDefault();
                if (parcours == null)
                {
                    parcours = new Parcours
                    {
                        RefAdresseOrigine = (int)refAdresseOrigine,
                        RefAdresseDestination = (int)refAdresseDestination
                    };
                    DbContext.Parcourss.Add(parcours);
                    transport = new Transport
                    {
                        RefTransporteur = (int)refTransporteur,
                        RefCamionType = (int)refCamionType,
                        Parcours = parcours
                    };
                    DbContext.Transports.Add(transport);
                }
                else
                {
                    //Looking for transport
                    transport = DbContext.Transports
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(q => q.RefParcours == parcours.RefParcours && q.RefTransporteur == refTransporteur && q.RefCamionType == refCamionType).FirstOrDefault();
                    if (transport == null)
                    {
                        transport = new Transport
                        {
                            RefTransporteur = (int)refTransporteur,
                            RefCamionType = (int)refCamionType,
                            Parcours = parcours
                        };
                        DbContext.Transports.Add(transport);
                    }
                }
            }
            // handle requests asking for non-existing object
            if (transport == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Finalizing
            transport.currentCulture = CurrentContext.CurrentCulture;
            //End
            return new JsonResult(
                _mapper.Map<Transport, TransportViewModel>(transport),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Transport with the given {id}
        /// </summary>
        /// <param name="model">The TransportViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] TransportViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Transport transport;
            //Retrieve or create the entity to edit
            if (model.RefTransport > 0)
            {
                transport = DbContext.Transports
               .Where(q => q.RefTransport == model.RefTransport).FirstOrDefault();

                //Set values
                transport.PUHT = model.PUHT;
                transport.PUHTDemande = model.PUHTDemande;
                transport.Parcours.Km = model.Parcours.Km;
            }
            else
            {
                transport = new Transport
                {
                    //write changes
                    RefTransporteur = model.RefTransporteur,
                    RefCamionType = model.RefCamionType
                };
                DbContext.Transports.Add(transport);
                //Looking for parcours
                Parcours parcours = DbContext.Parcourss.Where(q => q.RefAdresseOrigine == model.Parcours.RefAdresseOrigine && q.RefAdresseDestination == model.Parcours.RefAdresseDestination).FirstOrDefault();
                if (parcours == null)
                {
                    parcours = new Parcours
                    {
                        RefAdresseOrigine = model.Parcours.RefAdresseOrigine,
                        RefAdresseDestination = model.Parcours.RefAdresseDestination,
                        Km = model.Parcours.Km
                    };
                    DbContext.Parcourss.Add(parcours);
                }
                transport.Parcours = parcours;
                //Set values
                transport.PUHT = model.PUHT;
                transport.PUHTDemande = model.PUHTDemande;
                transport.Parcours.Km = model.Parcours.Km;
            }

            // handle requests asking for non-existing transportzes
            if (transport == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Register session user
            transport.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = transport.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Transport to the client.
                return new JsonResult(
                    _mapper.Map<Transport, TransportViewModel>(transport),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Transport with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the transport from the Database
            var transport = DbContext.Transports.Where(i => i.RefTransport == id)
                .FirstOrDefault();

            // handle requests asking for non-existing transportzes
            if (transport == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the transport from the DbContext.
            DbContext.Transports.Remove(transport);
            // persist the changes into the Database.
            DbContext.SaveChanges();

            // return an HTTP Status 200 (OK).
            return Ok();
        }
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/items/getcompetitor
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all competitor transports used during the last 365 days.</returns>
        [HttpGet("GetCompetitors")]
        public IActionResult GetCompetitor()
        {
            string refTransport = Request.Headers["refTransport"].ToString();
            int refT = 0;
            if (int.TryParse(refTransport, out refT))
            {
                SqlCommand cmd = new();
                DataSet dS = new();
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = "select RefTransport as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefTransport.ToString()].Name + "], tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "], tblTransport.PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransportPUHT.ToString()].Name + "], count(*) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Nb.ToString()].Name + "]"
                        + " from tblTransport"
                        + "     inner join (select RefTransporteur, RefParcours, RefCamionType from tblTransport where RefTransport = @refTransport) as reference"
                        + "         on tblTransport.RefParcours = reference.RefParcours and tblTransport.RefCamionType = reference.RefCamionType"
                        + "     inner join tblParcours on tblTransport.RefParcours = tblParcours.RefParcours"
                        + "     inner join tblEntite on tblTransport.RefTransporteur = tblEntite.RefEntite"
                        + "     left join(select RefCommandeFournisseur, RefCamionType, RefAdresse, RefAdresseClient, RefTransporteur from tblCommandeFournisseur where DATEADD(day, 365, tblCommandeFournisseur.DChargement)> getdate()) as cmd"
                        + "         on cmd.RefCamionType = tblTransport.RefCamionType"
                        + "             and cmd.RefAdresse = tblParcours.RefAdresseOrigine and cmd.RefAdresseClient = tblParcours.RefAdresseDestination"
                        + "             and cmd.RefTransporteur = tblTransport.RefTransporteur"
                        + " group by tblTransport.RefTransport, tblEntite.Libelle, tblTransport.PUHT"
                        + " order by tblTransport.PUHT";
                    cmd.Parameters.Add("@refTransport", SqlDbType.Int).Value = Convert.ToInt32(refTransport);
                    SqlDataAdapter dA = new(cmd);
                    if (dS.Tables.Count > 0) { dS.Tables.Clear(); }
                    dA.Fill(dS);
                    if (dS.Tables[0].Rows.Count == 0)
                    {
                        DataRow dRow = dS.Tables[0].NewRow();
                        dRow[1] = CurrentContext.CulturedRessources.GetTextRessource(299);
                        dS.Tables[0].Rows.Add(dRow);
                    }
                }
                //Return Json
                return new JsonResult(dS.Tables[0], JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
    }
    #endregion
}

