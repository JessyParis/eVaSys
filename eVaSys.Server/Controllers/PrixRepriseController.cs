/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/10/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using eVaSys.APIUtils;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class PrixRepriseController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public PrixRepriseController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/prixreprise/{id}
        /// Retrieves the PrixReprise with the given {id}, or that corresponds to parameters in header
        /// </summary>
        /// <param name="id">The ID of an existing PrixReprise</param>
        /// <returns>the PrixReprise with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            PrixReprise prixReprise = null;
            //Get headers
            string d = Request.Headers["d"].ToString();
            string refProcess = Request.Headers["refProcess"].ToString();
            string refProduit = Request.Headers["refProduit"].ToString();
            string refComposant = Request.Headers["refComposant"].ToString();
            string refEntite = Request.Headers["refEntite"].ToString();
            int refPt = 0;
            int refE = 0;
            DateTime dRef = DateTime.MinValue;
            //Get or create prixReprise
            if (id == 0)
            {
                if (int.TryParse(refProduit, out refPt) && DateTime.TryParse(d, out dRef))
                {
                    //If Entite, try to find a PrixReprise with Contrat
                    if (int.TryParse(refEntite, out refE))
                    {
                        prixReprise = DbContext.PrixReprises
                            .Include(r => r.UtilisateurCreation)
                            .Include(r => r.UtilisateurModif)
                            .Where(el => (el.RefProduit == refPt
                                && el.Contrat.ContratEntites.Any(e => e.RefEntite == refE)
                                && el.D.Month == dRef.Month && el.D.Year == dRef.Year))
                            .FirstOrDefault();
                    }
                    if (prixReprise == null)
                    {
                        {
                            prixReprise = DbContext.PrixReprises
                                .Include(r => r.UtilisateurCreation)
                                .Include(r => r.UtilisateurModif)
                                .Where(el => el.RefProduit == refPt
                                    && el.D.Month == dRef.Month && el.D.Year == dRef.Year
                                    && el.RefContrat == null)
                                .FirstOrDefault();
                        }
                    }
                }
            }
            else
            {
                prixReprise = DbContext.PrixReprises
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefPrixReprise == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (prixReprise == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            prixReprise.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<PrixReprise, PrixRepriseViewModel>(prixReprise),
                JsonSettings);
        }

        /// <summary>
        /// Create/Modify/Delete PrixReprise
        /// </summary>
        /// <param name="model">The PrixRepriseViewModels containing the data to update</param>
        [HttpPost("PostPrixReprises")]
        public IActionResult PostPrixReprises([FromBody] PrixRepriseViewModel[] model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            //Init
            string errs = "";
            //For each PrixReprise
            foreach (PrixRepriseViewModel prxR in model)
            {
                PrixReprise prx = null;
                //Delete PrixReprise if applicable
                if (prxR.RefPrixReprise != null && prxR.PUHT == null)
                {
                    prx = DbContext.PrixReprises.Where(el => el.RefPrixReprise == prxR.RefPrixReprise).FirstOrDefault();
                    if (prx != null) { DbContext.Remove(prx); }
                }
                else if (prxR.PUHT != null)
                {
                    //Get the corresponding PrixReprise
                    if (prxR.RefPrixReprise > 0)
                    {
                        prx = DbContext.PrixReprises.Where(i => i.RefPrixReprise == prxR.RefPrixReprise).FirstOrDefault();
                    }
                    else
                    {
                        prx = new PrixReprise();
                        prx.RefProcess = prxR.Process?.RefProcess;
                        prx.RefProduit = prxR.Produit.RefProduit;
                        prx.RefComposant = prxR.Composant?.RefProduit;
                        prx.RefContrat = prxR.Contrat?.RefContrat;
                        prx.D = prxR.D;
                        DbContext.PrixReprises.Add(prx);
                    }
                    if (prx != null)
                    {
                        prx.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                        prx.PUHT = prxR.PUHT ?? 0;
                        prx.PUHTSurtri = prxR.PUHTSurtri ?? 0;
                        prx.PUHTTransport = prxR.PUHTTransport ?? 0;
                        string validError = prx.IsValid();
                        if (validError == " ")
                        {
                            DbContext.SaveChanges();
                        }
                        else
                        {
                            errs += validError;
                        }
                        //Certif
                        if (prxR.Certif && prx.RefUtilisateurCreation != CurrentContext.RefUtilisateur)
                        {
                            prx.RefUtilisateurCertif = CurrentContext.RefUtilisateur;
                            prx.DCertif = DateTime.Now;
                        }
                    }
                }
            }
            //End
            if (errs == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Return the updated PrixReprises to the client
                return Ok();
            }
            else
            {
                return Conflict(new ConflictError(errs));
            }
        }

        /// <summary>
        /// Modify PrixReprise
        /// </summary>
        /// <param name="model">The PrixRepriseViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] PrixRepriseViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            PrixReprise prixReprise;
            //Retrieve or create the entity to edit
            if (model.RefPrixReprise > 0)
            {
                prixReprise = DbContext.PrixReprises
                        .Where(q => q.RefPrixReprise == model.RefPrixReprise)
                        .FirstOrDefault();
            }
            else
            {
                prixReprise = new PrixReprise();
                DbContext.PrixReprises.Add(prixReprise);
            }
            // handle requests asking for non-existing prixReprisezes
            if (prixReprise == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref prixReprise, model);

            //Register session user
            prixReprise.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = prixReprise.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Return the updated PrixReprise to the client, or return the next Mixte if applicable
                return new JsonResult(
                    _mapper.Map<PrixReprise, PrixRepriseViewModel>(prixReprise),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the PrixReprise with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the prixReprise from the Database
            var prixReprise = DbContext.PrixReprises.Where(i => i.RefPrixReprise == id)
                .FirstOrDefault();

            // handle requests asking for non-existing prixReprisezes
            if (prixReprise == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = prixReprise.IsDeletable();
            if (del == "")
            {
                DbContext.PrixReprises.Remove(prixReprise);
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
        /// GET: evapi/getcollectiviteprixreprises
        /// Retrieves the PrixReprise list for the given date and process
        /// </summary>
        [HttpGet("GetCollectivitePrixReprises")]
        public IActionResult GetCollectivitePrixReprises()
        {
            //Get headers
            string refEntite = Request.Headers["refEntite"].ToString();
            string d = Request.Headers["d"].ToString();
            string filterCollecte = Request.Headers["filterCollecte"].ToString();
            int refE = 0;
            DateTime dRef = DateTime.MinValue;
            List<PrixReprise> prxList = new();
            //Get PrixReprises
            if (int.TryParse(refEntite, out refE) && DateTime.TryParse(d, out dRef))
            {
                var q = (from ent in DbContext.Entites
                         join eS in DbContext.EntiteStandards on ent.RefEntite equals eS.RefEntite
                         join sP in DbContext.ProduitStandards on eS.RefStandard equals sP.RefStandard
                         join s in DbContext.Standards on eS.RefStandard equals s.RefStandard
                         join pR in DbContext.PrixReprises on sP.RefProduit equals pR.RefProduit
                         join p in DbContext.Produits on pR.RefProduit equals p.RefProduit
                         where eS.RefEntite == refE && pR.D.Month == dRef.Month && pR.D.Year == dRef.Year
                         orderby s.Libelle, p.NomCommun
                         select new
                         {
                             StandardLibelle = s.Libelle,
                             StandardCmt = s.Cmt,
                             ProduitNomCommun = p.NomCommun,
                             pR.PUHT,
                             p.Collecte,
                             pR.RefContrat
                         }
                       );
                if (filterCollecte == "Collecte") { q = q.Where(w => w.Collecte == true); }
                else if (filterCollecte == "HorsCollecte") { q = q.Where(w => w.Collecte == false); }
                else if (filterCollecte != "DansOuHorsCollecte") { q = q.Where(w => 1 != 1); }
                var res = (q.ToArray());
                //If Entite has on Contract RI the show only price in Contract
                var ct = DbContext.Contrats.Where(e => e.RefContratType == 1 && e.ContratEntites.Any(a => a.RefEntite == refE)).Count();
                if (ct > 0)
                {
                    res = res.Where(e => e.RefContrat != null).ToArray();
                }
                else
                {
                    res = res.Where(e => e.RefContrat == null).ToArray();
                }
                //Return Json
                return new JsonResult(res, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// GET: evapi/getprixreprises
        /// Retrieves the PrixReprise list for the given date and process
        /// </summary>
        [HttpGet("GetPrixReprises")]
        public IActionResult GetPrixReprises()
        {
            //Get headers
            string refProcess = Request.Headers["refProcess"].ToString();
            string d = Request.Headers["d"].ToString();
            string filterProduits = Request.Headers["filterProduits"].ToString();
            string filterComposants = Request.Headers["filterComposants"].ToString();
            int? refP = null;
            DateTime dRef = DateTime.MinValue;
            List<PrixReprise> prxList = new();
            //Init
            if (int.TryParse(refProcess, out int refPTmp)) { refP = refPTmp; }
            //Get PrixReprises
            if (DateTime.TryParse(d, out dRef))
            {
                prxList = SelectPrixReprises(refP, dRef, filterProduits, filterComposants);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
            // handle requests asking for non-existing object
            if (prxList.Count == 0)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            //Return Json
            return new JsonResult(
                _mapper.Map<List<PrixReprise>, List<PrixRepriseViewModel>>(prxList),
                JsonSettings);
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref PrixReprise dataModel, PrixRepriseViewModel viewModel)
        {
            dataModel.RefProcess = viewModel.Process.RefProcess;
            dataModel.RefProduit = viewModel.Produit.RefProduit;
            dataModel.RefComposant = viewModel.Composant.RefProduit;
            dataModel.RefContrat = viewModel.Contrat?.RefContrat;
            dataModel.D = viewModel.D;
            dataModel.PUHT = viewModel.PUHT ?? 0;
            dataModel.PUHTSurtri = viewModel.PUHTSurtri ?? 0;
            dataModel.PUHTTransport = viewModel.PUHTTransport ?? 0;
        }
        /// <summary>
        /// find all possible/existing PrixReprise
        /// </summary>
        private List<PrixReprise> SelectPrixReprises(int? refP, DateTime dRef, string filterProduits, string filterComposants)
        {
            List<PrixReprise> prxList = new();
            PrixReprise prx;
            SqlCommand cmd = new();
            SqlDataAdapter dA = new(cmd);
            DataSet dS = new();
            string sqlStr = "";
            if (refP != null)
            {
                sqlStr = "select tbrPrixReprise.RefPrixReprise, @d as D, @refProcess as RefProcess, tblProduit.RefProduit, tblProduit.Libelle as Produit, composant.RefProduit as RefComposant, composant.Libelle as Composant"
                    + "     , tbrPrixReprise.RefContrat"
                    + "     , tbrPrixReprise.PUHT, tblProduit.PUHTSurtri as Surtri, tbrPrixReprise.PUHTSurtri, tblProduit.PUHTTransport as Transport, tbrPrixReprise.PUHTTransport"
                    + " from tblProduit"
                    + " 	inner join tbmProduitComposant on tblProduit.refProduit=tbmProduitComposant.RefProduit"
                    + " 	inner join tblProduit as composant on composant.RefProduit=tbmProduitComposant.RefComposant"
                    + " 	left join (select * from tbrPrixReprise where D=@d and RefProcess=@refProcess and tbrPrixReprise.RefContrat is null) as tbrPrixReprise on tbrPrixReprise.RefProduit=tbmProduitComposant.RefProduit and tbrPrixReprise.RefComposant=tbmProduitComposant.RefComposant"
                    + " where ((composant.Actif=1 and tblProduit.Actif=1) or tbrPrixReprise.RefPrixReprise is not null)"
                    + " union all"
                    + " select tbrPrixReprise.RefPrixReprise, @d as D, @refProcess as RefProcess, tblProduit.RefProduit, tblProduit.Libelle as Produit, composant.RefProduit as RefComposant, composant.Libelle as Composant"
                    + " 	, tblContrat.RefContrat"
                    + "     , tbrPrixReprise.PUHT, tblProduit.PUHTSurtri as Surtri, tbrPrixReprise.PUHTSurtri, tblProduit.PUHTTransport as Transport, tbrPrixReprise.PUHTTransport"
                    + " from tblCommandeClient"
                    + " 	inner join tblCommandeClientMensuelle on tblCommandeClient.RefCommandeClient=tblCommandeClientMensuelle.RefCommandeClient"
                    + " 	inner join tblProduit on tblCommandeClient.RefProduit = tblProduit.RefProduit"
                    + " 	inner join tbmProduitComposant on tblProduit.refProduit=tbmProduitComposant.RefProduit"
                    + " 	inner join tblProduit as composant on composant.RefProduit=tbmProduitComposant.RefComposant	"
                    + " 	inner join tblContrat on tblCommandeClient.RefContrat=tblContrat.RefContrat"
                    + " 	left join (select * from tbrPrixReprise where D=@d and RefProcess=@refProcess) as tbrPrixReprise "
                    + " 		on tbrPrixReprise.RefProduit=tbmProduitComposant.RefProduit and tbrPrixReprise.RefComposant=tbmProduitComposant.RefComposant and tbrPrixReprise.RefContrat=tblCommandeClient.RefContrat"
                    + " where tblCommandeClient.RefContrat is not null and tblCommandeClientMensuelle.D=@d";
            }
            else
            {
                sqlStr = "select tbrPrixReprise.RefPrixReprise, @d as D, null as RefProcess, tblProduit.RefProduit, tblProduit.Libelle as Produit, null as RefComposant, null as Composant"
                    + "     , tbrPrixReprise.RefContrat"
                    + "     , tbrPrixReprise.PUHT, tblProduit.PUHTSurtri as Surtri, tbrPrixReprise.PUHTSurtri, tblProduit.PUHTTransport as Transport, tbrPrixReprise.PUHTTransport"
                    + " from tblProduit"
                    + " 	left join (select * from tbrPrixReprise where D=@d and tbrPrixReprise.RefContrat is null) as tbrPrixReprise on tbrPrixReprise.RefProduit=tblProduit.RefProduit"
                    + " where (tblProduit.Actif=1 or tbrPrixReprise.RefPrixReprise is not null)"
                    + " union all"
                    + " select tbrPrixReprise.RefPrixReprise, @d as D, null as RefProcess, tblProduit.RefProduit, tblProduit.Libelle as Produit, null as RefComposant, null as Composant"
                    + " 	, tblContrat.RefContrat"
                    + "     , tbrPrixReprise.PUHT, tblProduit.PUHTSurtri as Surtri, tbrPrixReprise.PUHTSurtri, tblProduit.PUHTTransport as Transport, tbrPrixReprise.PUHTTransport"
                    + " from tblCommandeClient"
                    + " 	inner join tblCommandeClientMensuelle on tblCommandeClient.RefCommandeClient=tblCommandeClientMensuelle.RefCommandeClient"
                    + " 	inner join tblProduit on tblCommandeClient.RefProduit = tblProduit.RefProduit"
                    + " 	inner join tblContrat on tblCommandeClient.RefContrat=tblContrat.RefContrat"
                    + " 	left join (select * from tbrPrixReprise where D=@d) as tbrPrixReprise "
                    + " 		on tbrPrixReprise.RefProduit=tblProduit.RefProduit and tbrPrixReprise.RefContrat=tblCommandeClient.RefContrat"
                    + " where tblCommandeClient.RefContrat is not null and tblCommandeClientMensuelle.D=@d";
            }
            cmd.Parameters.Add("@d", SqlDbType.DateTime).Value = dRef;
            if (refP != null) { cmd.Parameters.Add("@refProcess", SqlDbType.Int).Value = refP; }
            if (filterProduits != "")
            {
                sqlStr += " and tblProduit.Refproduit in (";
                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                sqlStr += ")";
            }
            if (filterComposants != "" && refP != null)
            {
                sqlStr += " and composant.RefProduit in (";
                sqlStr += Utils.Utils.CreateSQLParametersFromString("refComposant", filterComposants, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                sqlStr += ")";
            }
            if (refP != null) { sqlStr += " 	order by tblProduit.Libelle, composant.Libelle"; }
            else { sqlStr += " 	order by tblProduit.Libelle"; }
            cmd.CommandText = sqlStr;
            cmd.Connection = (SqlConnection)DbContext.Database.GetDbConnection();
            dA.Fill(dS);
            foreach (DataRow dRow in dS.Tables[0].Rows)
            {
                if (dRow["RefPrixReprise"] != DBNull.Value)
                {
                    prx = DbContext.PrixReprises
                        .Include(r => r.UtilisateurCreation)
                        .Include(r => r.UtilisateurModif)
                        .FirstOrDefault(el => el.RefPrixReprise == (int)dRow["RefPrixReprise"]);
                    if (prx != null) { prxList.Add(prx); }
                }
                else
                {
                    prx = new PrixReprise();
                    DbContext.PrixReprises.Add(prx);
                    prx.Produit = DbContext.Produits.FirstOrDefault(el => el.RefProduit == (int)dRow["RefProduit"]);
                    if (prx.Produit != null)
                    {
                        prx.D = dRef;
                        prxList.Add(prx);
                    }
                    if (dRow["RefContrat"] != DBNull.Value)
                    {
                        prx.Contrat = DbContext.Contrats.FirstOrDefault(el => el.RefContrat == (int)dRow["RefContrat"]);
                    }
                }
            }
            return prxList;
        }
        #endregion
    }
}

