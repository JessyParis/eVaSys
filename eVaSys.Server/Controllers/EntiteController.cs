/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/10/2018
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eValorplast.BLL;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System.Web;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class EntiteController : BaseApiController
    {
        private readonly IMapper _mapper;
        protected IWebHostEnvironment _env;

        #region Constructor
        public EntiteController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper, IWebHostEnvironment env)
            : base(context, configuration)
        {
            _mapper = mapper;
            _env = env;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/entite/{id}
        /// Retrieves the Entite with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Entite</param>
        /// <returns>the Entite with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string refContactAdresse = (Request.Headers.ContainsKey("refContactAdresse") ? Request.Headers["refContactAdresse"].ToString() : "");
            string refAction = (Request.Headers.ContainsKey("refAction") ? Request.Headers["refAction"].ToString() : "");
            int refCA = 0;
            int refA = 0;
            Entite entite;
            var req = DbContext.Entites
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseContactAdresseProcesss)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseServiceFonctions)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseDocumentTypes)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.Utilisateurs)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.UtilisateurModif)
                .Include(r => r.Adresses)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.Adresses)
                .ThenInclude(r => r.UtilisateurModif)
                .Include(r => r.EntiteCamionTypes)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.ContratIncitationQualites)
                .Include(r => r.ContratCollectivites)
                .Include(r => r.EntiteDRs)
                .Include(r => r.EntiteProcesss)
                .Include(r => r.EntiteProduits)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.EntiteStandards)
                .Include(r => r.Actions)
                .ThenInclude(r => r.ActionActionTypes)
                .Include(r => r.Actions)
                .ThenInclude(r => r.FormeContact)
                .Include(r => r.Actions)
                .ThenInclude(r => r.ContactAdresse)
                .Include(r => r.Actions)
                .ThenInclude(r => r.ActionFichierNoFiles)
                .Include(r => r.Actions)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.Actions)
                .ThenInclude(r => r.UtilisateurModif)
                .Include(r => r.UtilisateurCreation)
                .Include(r => r.UtilisateurModif)
                .AsSplitQuery()
                .AsQueryable();
            int.TryParse(refContactAdresse, out refCA);
            int.TryParse(refAction, out refA);
            if (refCA > 0)
            {
                req = req.Where(i => i.ContactAdresses.Any(e => e.RefContactAdresse == refCA));
            }
            else if (refA > 0)
            {
                req = req.Where(i => i.Actions.Any(e => e.RefAction == refA));
            }
            else
            {
                req = req.Where(i => i.RefEntite == id);
            }
            entite = req.FirstOrDefault();
            //Load SAGEDocuments if applicable
            var sAGEDocuments = DbContext.SAGEDocuments.Where(e => e.CodeComptable == entite.SAGECodeComptable && e.DocumentType == 17)
                .Include(i => i.SAGEDocumentReglements)
                .ToList();
            entite.SAGEDocuments = sAGEDocuments;
            // handle requests asking for non-existing object
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Entite, EntiteViewModel>(entite),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Entite with the given {id}
        /// </summary>
        /// <param name="model">The EntiteViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] EntiteViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Entite entite;
            if (model.RefEntite > 0)
            {
                entite = DbContext.Entites
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseContactAdresseProcesss)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseServiceFonctions)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseDocumentTypes)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.Utilisateurs)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.UtilisateurModif)
                .Include(r => r.Adresses)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.Adresses)
                .ThenInclude(r => r.UtilisateurModif)
                .Include(r => r.ContratIncitationQualites)
                .Include(r => r.ContratCollectivites)
                .Include(r => r.EntiteCamionTypes)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.EntiteDRs)
                .Include(r => r.EntiteProcesss)
                .Include(r => r.EntiteProduits)
                .Include(r => r.DocumentEntites)
                .ThenInclude(r => r.DocumentNoFile)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.EntiteStandards)
                .Include(r => r.Actions)
                .ThenInclude(r => r.ActionActionTypes)
                .Include(r => r.Actions)
                .ThenInclude(r => r.FormeContact)
                .Include(r => r.Actions)
                .ThenInclude(r => r.ContactAdresse)
                .Include(r => r.Actions)
                .ThenInclude(r => r.ActionFichierNoFiles)
                .Include(r => r.Actions)
                .ThenInclude(r => r.UtilisateurCreation)
                .Include(r => r.Actions)
                .ThenInclude(r => r.UtilisateurModif)
                .Include(r => r.UtilisateurCreation)
                .Include(r => r.UtilisateurModif)
                .AsSplitQuery()
                .Where(q => q.RefEntite == model.RefEntite)
                .FirstOrDefault();
            }
            else
            {
                entite = new Entite();
                DbContext.Entites.Add(entite);
            }

            // handle requests asking for non-existing entitezes
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Check if linked users should be deactivated
            bool deactivateLinkedUsers = model.RefEntite > 0 &&
                (entite.Actif == true && model.Actif != true);

            //Set values
            UpdateData(ref entite, model);
            //Register session user
            entite.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Deactivate linked users if applicable
            if (deactivateLinkedUsers)
            {
                var us = DbContext.Utilisateurs.Where(e =>
                e.RefCentreDeTri == model.RefEntite
                || e.RefClient == model.RefEntite
                || e.RefCollectivite == model.RefEntite
                || e.RefTransporteur == model.RefEntite)
                    .ToList();
                foreach (Utilisateur u in us)
                {
                    u.Actif = false;
                }
            }

            //Check validation
            string valid = entite.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Entite to the client.
                return new JsonResult(
                    _mapper.Map<Entite, EntiteViewModel>(entite),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Entite with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the entite from the Database
            var entite = DbContext.Entites.Where(i => i.RefEntite == id)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseContactAdresseProcesss)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseServiceFonctions)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseDocumentTypes)
                .Include(r => r.Adresses)
                .Include(r => r.ContratIncitationQualites)
                .Include(r => r.ContratCollectivites)
                .Include(r => r.EntiteCamionTypes)
                .Include(r => r.EntiteDRs)
                .Include(r => r.EntiteEntiteInits)
                .Include(r => r.EntiteEntiteRttInits)
                .Include(r => r.EntiteProcesss)
                .Include(r => r.EntiteProduits)
                .Include(r => r.DocumentEntites)
                .ThenInclude(r => r.DocumentNoFile)
                .Include(r => r.EntiteStandards)
                .Include(r => r.Actions)
                .ThenInclude(r => r.ActionActionTypes)
                .Include(r => r.Actions)
                .ThenInclude(r => r.ActionFichierNoFiles)
                .AsSplitQuery()
                .FirstOrDefault();

            // handle requests asking for non-existing entitezes
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = entite.IsDeletable();
            if (del == "")
            {
                //Delete related data 
                //DeleteEntiteEntite(entite);
                //Delete entity
                DbContext.Entites.Remove(entite);
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
            int? refEntiteType = (Request.Query.ContainsKey("refEntiteType") ? (int?)System.Convert.ToInt32(Request.Query["refEntiteType"]) : null);
            string textFilter = (Request.Query.ContainsKey("textFilter") ? Request.Query["textFilter"].ToString() : "");
            string refEntiteFournisseur = Request.Headers["refEntiteFournisseur"].ToString();
            string refEntiteRtt = Request.Headers["refEntiteRtt"].ToString();
            string refEntiteRttForbidden = Request.Headers["refEntiteRttForbidden"].ToString();
            bool actif = (Request.Headers["actif"] == "true");
            string refEntite = Request.Headers["refEntite"].ToString();
            string refEntiteContrat = Request.Headers["refEntiteContrat"].ToString();
            string refProduit = Request.Headers["refProduit"].ToString();
            string d = Request.Headers["d"].ToString();
            bool contratActif = (Request.Headers.ContainsKey("contratActif") ? (Request.Headers["contratActif"] == "true") : false);
            bool lienActif = (Request.Headers.ContainsKey("lienActif") ? (Request.Headers["lienActif"] == "true") : false);
            bool surcoutCarburantHT = (Request.Headers.ContainsKey("surcoutCarburantHT") ? (Request.Headers["surcoutCarburantHT"] == "true" ? true : false) : false);
            string refContrat = Request.Headers["refContrat"].ToString();
            bool hasContrat = Request.Headers.ContainsKey("refContratType");
            string refContratType = Request.Headers["refContratType"].ToString();
            int refP = 0;
            int refE = 0;
            int refEC = 0;
            int refEF = 0;
            int refERtt = 0;
            int refERttF = 0;
            int refCT = 0;
            int refCt = 0;
            DateTime dRef = DateTime.MinValue;
            var req = DbContext.Entites.AsQueryable();
            //RefEntiteType
            if (refEntiteType != null && refEntiteType != 0)
            {
                if (int.TryParse(refEntite, out refE))
                { req = req.Where(el => el.RefEntiteType == refEntiteType || el.RefEntite == refE); }
                else { req = req.Where(el => el.RefEntiteType == refEntiteType); }
                if ((refEntiteType == 1 || refEntiteType == 3) && CurrentContext.filterDR)
                {
                    if (int.TryParse(refEntite, out refE)) { req = req.Where(el => el.RefEntiteType == refEntiteType || el.RefEntite == refE); }
                    else { req = req.Where(el => el.EntiteDRs.Any(p => p.RefDR == CurrentContext.RefUtilisateur) || el.RefEntite == refE); }
                }
            }
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (int.TryParse(refEntite, out refE)) { req = req.Where(el => el.Actif == true || el.RefEntite == refE); }
                else { req = req.Where(el => el.Actif == true); }
            }
            if (surcoutCarburantHT)
            {
                req = req.Where(el => el.SurcoutCarburant == true);
            }
            if (refEntiteRtt != "")
            {
                if (int.TryParse(refEntiteRtt, out refERtt))
                {
                    //Get the list of linked entities
                    var rq = DbContext.EntiteEntites
                        .Where(e => e.RefEntite == refERtt);
                    if (lienActif) { rq = rq.Where(e => e.Actif == true); }
                    List<int> refEntiteRtts = rq.Select(p => p.RefEntiteRtt).ToList();
                    rq = DbContext.EntiteEntites
                        .Where(e => e.RefEntiteRtt == refERtt);
                    if (lienActif) { rq = rq.Where(e => e.Actif == true); }
                    refEntiteRtts.AddRange(rq.Select(p => p.RefEntite).ToList());
                    req = req.Where(el => refEntiteRtts.ToArray().Contains(el.RefEntite));
                }
            }
            //Exclude forbidden linked Entites
            if (refEntiteRttForbidden != "")
            {
                if (int.TryParse(refEntiteRttForbidden, out refERttF))
                {
                    //Get the list of linked entities
                    var rq = DbContext.EntiteEntites
                        .Where(e => e.RefEntite == refERttF && e.Actif == true);
                    List<int> refEntiteRtts = rq.Select(p => p.RefEntiteRtt).ToList();
                    rq = DbContext.EntiteEntites
                        .Where(e => e.RefEntiteRtt == refERttF && e.Actif == true);
                    refEntiteRtts.AddRange(rq.Select(p => p.RefEntite).ToList());
                    if (int.TryParse(refEntite, out refE))
                    {
                        req = req.Where(el => (!(refEntiteRtts.ToArray().Contains(el.RefEntite)) || el.RefEntite == refE));
                    }
                    else
                    {
                        req = req.Where(el => !(refEntiteRtts.ToArray().Contains(el.RefEntite)));
                    }
                }
            }
            //For EntiteType exept when ask for existing contrat collectivite
            if (refEntiteType == 1 && !hasContrat)
            {
                if (contratActif)
                {
                    if (DateTime.TryParse(d, out dRef))
                    {
                        req = req.Where(el => el.ContratCollectivites.Any(p => dRef >= p.DDebut && dRef <= p.DFin));
                    }
                    else
                    {
                        req = req.Where(el => el.ContratCollectivites.Count > 0);
                    }
                }
                else
                {
                    req = req.Where(el => el.ContratCollectivites.Count == 0);
                }
            }
            //Check CommandeClient
            if (int.TryParse(refProduit, out refP) && DateTime.TryParse(d, out dRef))
            {
                int? refContratRI = null;
                int.TryParse(refEntiteFournisseur, out refEF);
                if (refEF != 0)
                {
                    //Get Collectivite linked to the Fournisseur
                    List<int> refEntiteRtts = DbContext.EntiteEntites
                        .Where(e => e.RefEntite == refEF && e.EntiteRtt.Actif == true && e.Actif == true && e.EntiteRtt.RefEntiteType == 1).Select(p => p.RefEntiteRtt).ToList();
                    refEntiteRtts.AddRange(DbContext.EntiteEntites
                        .Where(e => e.RefEntiteRtt == refEF && e.Entite.Actif == true && e.Actif == true && e.Entite.RefEntiteType == 1).Select(p => p.RefEntite).ToList());
                    //Search for Contrat RI
                    refContratRI = DbContext.Contrats
                        .Where(e => e.RefContratType == 1 && e.DDebut <= DateOnly.FromDateTime(dRef) && e.DFin >= DateOnly.FromDateTime(dRef.AddMonths(1).AddDays(-1))
                         && e.ContratEntites.Any(i => refEntiteRtts.Contains(i.RefEntite)))
                        .Select(p => p.RefContrat).FirstOrDefault();
                }
                if (refContratRI > 0)
                {
                    //Check contrat RI if applicable
                    req = req.Where(el => (el.CommandeClients.Any(
                    rel => rel.RefProduit == refP && rel.RefContrat == refContratRI && rel.CommandeClientMensuelles.Any(
                        r => (r.D.Month == dRef.Month && r.D.Year == dRef.Year) && r.Poids > 0)))
                         || el.RefEntite == refE);
                }
                else
                {
                    req = req.Where(el => (el.CommandeClients.Any(
                    rel => rel.RefProduit == refP && rel.CommandeClientMensuelles.Any(
                        r => (r.D.Month == dRef.Month && r.D.Year == dRef.Year) && r.Poids > 0)))
                         || el.RefEntite == refE);
                }
            }
            //Has an active Contrat
            if (hasContrat)
            {
                //Check date for Contrat
                DateTime dT = DateTime.MinValue;
                DateTime.TryParse(d, out dT);
                if (dT == DateTime.MinValue)
                {
                    req = req.Where(el => 1 != 1);
                }
                else
                {
                    //init
                    DateOnly dContrat = DateOnly.FromDateTime(DateTime.Now);
                    int.TryParse(refEntiteContrat, out refEC);
                    int.TryParse(refContratType, out refCT);
                    int.TryParse(refContrat, out refCt);
                    if (dT != DateTime.MinValue) { dContrat = DateOnly.FromDateTime(dT); }
                    //Get idContrats for the Entite
                    List<int> refContrats = new List<int>();
                    if (refCt > 0) { refContrats.Add(refCt); }
                    else
                    {
                        refContrats = DbContext.Contrats
                        .Where(e => e.RefContratType == refCT && e.DDebut <= dContrat && e.DFin >= dContrat && e.ContratEntites.Any(i => i.RefEntite == refEC))
                        .Select(p => p.RefContrat).ToList();
                    }
                    //Process
                    if (int.TryParse(refEntite, out refE))
                    {
                        req = req.Where(el => (el.ContratEntites.Any(p => refContrats.Contains(p.RefContrat))
                        && el.RefEntiteType == refEntiteType)
                        || el.RefEntite == refE);
                    }
                    else
                    {
                        req = req.Where(el => el.ContratEntites.Any(p => refContrats.Contains(p.RefContrat))
                        && el.RefEntiteType == refEntiteType);
                    }
                }
            }
            if (textFilter != "")
            {
                req = req.Where(el => EF.Functions.Like(el.Libelle, "%" + textFilter + "%") || el.CodeEE == textFilter);
            }
            if (CurrentContext.ConnectedUtilisateur.HabilitationModulePrestataire == Enumerations.HabilitationModulePrestataire.Fournisseur.ToString())
            {
                req = req.Where(el => el.EntiteType.RefEntiteType != 3 || !string.IsNullOrWhiteSpace(el.CodeEE));
            }
            //Get formatted data
            var all = req.Select(p => new
            {
                p.RefEntite,
                p.RefEntiteType,
                p.Actif,
                Libelle = (p.CodeEE == null ? p.Libelle : (p.CodeEE + " - " + p.Libelle)),
            }).Select(p => new Entite
            {
                RefEntite = p.RefEntite,
                RefEntiteType = p.RefEntiteType,
                Actif = p.Actif,
                Libelle = p.Libelle,
            }).OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Entite[], EntiteListViewModel[]>(all),
                JsonSettings);
        }

        /// <summary>
        /// GET: api/items/getcontrats
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>The Entite that contains contrats</returns>
        [HttpGet("getcontrats")]
        public IActionResult GetContrats()
        {
            string refEntite = Request.Headers["refEntite"].ToString();
            string d = Request.Headers["d"].ToString();
            int refE = 0;
            int.TryParse(refEntite, out refE);
            DateOnly dRef = DateOnly.MinValue;
            DateOnly.TryParse(d, out dRef);
            var req = DbContext.Contrats.AsQueryable();
            //Process
            req = req.Include(i => i.ContratEntites)
                        .ThenInclude(i => i.Entite)
                        .Where(e => e.ContratEntites.Any(a => a.RefEntite == refE));
            if (dRef != DateOnly.MinValue)
            {
                req = req.Where(e => e.DDebut <= dRef && e.DFin >= dRef);
            }
            var contrats = req.ToArray();
            //End
            return new JsonResult(
                _mapper.Map<Contrat[], ContratViewModel[]>(contrats),
                JsonSettings);
        }

        /// <summary>
        /// GET: api/items/getcontacts
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>The Entite that contains contacts</returns>
        [HttpGet("getcontacts")]
        public IActionResult GetContacts()
        {
            int? refEntite = (Request.Headers.ContainsKey("refEntite") ? (int?)System.Convert.ToInt32(Request.Headers["refEntite"]) : null);
            //var entite = DbContext.Entites.Where(i => i.RefEntite == refEntite).FirstOrDefault();
            var entite = DbContext.Entites
                .Include(p => p.Adresses)
                .Include(p => p.ContactAdresses)
                    .ThenInclude(p => p.ContactAdresseContactAdresseProcesss)
                    .ThenInclude(p => p.ContactAdresseProcess)
                .Include(p => p.ContactAdresses)
                    .ThenInclude(p => p.Adresse)
                .Include(p => p.ContactAdresses)
                    .ThenInclude(p => p.ContactAdresseServiceFonctions)
                    .ThenInclude(p => p.Service)
                .Include(p => p.ContactAdresses)
                    .ThenInclude(p => p.ContactAdresseServiceFonctions)
                    .ThenInclude(p => p.Fonction)
                .Include(r => r.ContactAdresses)
                .ThenInclude(r => r.ContactAdresseDocumentTypes)
                .Include(r => r.ContactAdresses)
                    .ThenInclude(r => r.Utilisateurs)
                .Include(r => r.ContratCollectivites)
                .Include(r => r.EntiteStandards)
                    .ThenInclude(r => r.Standard)
                .Include(r => r.EntiteProduits)
                    .ThenInclude(r => r.Produit)
                .Include(r => r.EntiteCamionTypes)
                    .ThenInclude(r => r.CamionType)
                .AsSplitQuery()
                .Where(i => i.RefEntite == refEntite)
                .FirstOrDefault();
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Entite, EntiteViewModel>(entite),
                JsonSettings);
        }

        /// <summary>
        /// GET: api/items/getdocumententites
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getdocumententites")]
        public IActionResult GetDocumentEntites()
        {
            int? refEntite = (Request.Query.ContainsKey("refEntite") ? (int?)System.Convert.ToInt32(Request.Query["refEntite"]) : null);
            var entite = DbContext.Entites.Where(i => i.RefEntite == refEntite).FirstOrDefault();
            // handle requests asking for non-existing object
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Add general documents
            List<DocumentEntite> dEs = new();
            List<DocumentEntite> docRefs = new();
            string sqlStr = "select distinct tblDocument.RefDocument, tbmDocumentEntite.RefDocumentEntite"
                + "     from tblDocument"
                + "         left join tbmDocumentEntite on tblDocument.RefDocument = tbmDocumentEntite.RefDocument"
                + "         left join tbmDocumentEntiteType on tblDocument.RefDocument = tbmDocumentEntiteType.RefDocument"
                + "     where ((RefEntiteType = '" + entite.EntiteType.RefEntiteType.ToString() + "' and tblDocument.RefDocument not in (select RefDocument from tbmDocumentEntite))"
                + "             or tblDocument.RefDocument in (select RefDocument from tbmDocumentEntite where RefEntite=" + refEntite + "))"
                + "         and isnull(DDEbut, cast(getdate() as date))<= cast(getdate() as date) and isnull(DFin, cast(getdate() as date))>= cast(getdate() as date)"
                + "         and Actif=1"
                + "     order by tblDocument.RefDocument desc";

            using (SqlConnection sqlConn = (SqlConnection)DbContext.Database.GetDbConnection())
            {
                SqlCommand cmd = new();
                sqlConn.Open();
                cmd.Connection = sqlConn;
                cmd.CommandText = sqlStr;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    docRefs.Add(
                        new DocumentEntite()
                        {
                            RefDocument = (int)dr[0],
                            RefDocumentEntite = (dr[1] == DBNull.Value ? 0 : (int)dr[1])
                        });
                }
                //Fermeture de la source de données
                dr.Close();
                foreach (var dR in docRefs)
                {
                    DocumentEntite dE = new DocumentEntite()
                    {
                        RefEntite = (int)refEntite,
                        RefDocument = dR.RefDocument,
                        RefDocumentEntite = dR.RefDocumentEntite
                    };
                    dE.DocumentNoFile = DbContext.DocumentNoFiles.FirstOrDefault(e => e.RefDocument == dR.RefDocument);
                    dEs.Add(dE);
                }
            }
            //End
            return new JsonResult(
                _mapper.Map<DocumentEntite[], DocumentEntiteViewModel[]>(dEs.ToArray()),
                JsonSettings);
        }

        /// <summary>
        /// GET: api/items/getnotecredits
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getnotecredits")]
        public IActionResult GetNoteCredits()
        {
            int? refEntite = (Request.Query.ContainsKey("refEntite") ? (int?)System.Convert.ToInt32(Request.Query["refEntite"]) : null);
            //var entite = DbContext.Entites.Where(i => i.RefEntite == refEntite).FirstOrDefault();
            var documentEntites = DbContext.DocumentEntites
                .Include(p => p.DocumentNoFile)
                .Where(i => i.RefEntite == refEntite).ToArray();
            // handle requests asking for non-existing object
            if (documentEntites == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<DocumentEntite[], DocumentEntiteViewModel[]>(documentEntites),
                JsonSettings);
        }

        /// <summary>
        /// GET: api/items/getetattrimestrielcss
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getetattrimestrielcss")]
        public IActionResult GetEtatTrimestrielCSs()
        {
            int? refEntite = (Request.Query.ContainsKey("refEntite") ? (int?)System.Convert.ToInt32(Request.Query["refEntite"]) : null);
            var entite = DbContext.Entites.Where(i => i.RefEntite == refEntite).FirstOrDefault();
            // handle requests asking for non-existing object
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Process
            List<Quarter> docs = new();
            //Recherche des derniers états complets sur les 5 dernières années
            DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //Recalage de la date de référence pour laisser du délai avant la présentation des états
            d = d.AddDays(-19);
            Quarter t = new Quarter(d, CurrentContext.CurrentCulture);
            d = t.Begin;
            string sqlStr = "";
            //Chargement des documents à télécharger
            for (int i = 1; i < 20; i++)
            {
                d = d.AddMonths(-3);
                t = new Quarter(d, CurrentContext.CurrentCulture);
                //Seulement pour les Quarters concernant la collectivité
                sqlStr = "select count(*) "
                    + " from  tblRepartitionCollectivite"
                    + " 	inner join tblRepartition on tblRepartition.RefRepartition=tblRepartitionCollectivite.RefRepartition"
                    + " 	left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + " where tblCommandeFournisseur.DDechargement between convert(datetime, '" + t.Begin.ToString("dd/MM/yyyy") + "' ,103) and convert(datetime, '" + t.End.ToString("dd/MM/yyyy") + "' , 103) "
                    + " 	and RefCollectivite=" + refEntite.ToString();
                if (Utils.Utils.DbScalar(sqlStr, DbContext.Database.GetDbConnection()) != "0")
                {
                    sqlStr = "select count(*)"
                        + " from tblCommandeFournisseur"
                        + " 	inner join "
                        + "         (select tbmEntiteEntite.RefEntiteEntite, tbmEntiteEntite.RefEntite, RefEntiteRtt, tblEntite.Libelle"
                        + "         from tbmEntiteEntite"
                        + "			    inner join tblEntite on tbmEntiteEntite.RefEntiteRtt=tblEntite.RefEntite"
                        + "			where tblEntite.RefEntiteType=3 and tbmEntiteEntite.RefEntite= " + refEntite.ToString()
                        + "			union all"
                        + "			select tbmEntiteEntite.RefEntiteEntite, tbmEntiteEntite.RefEntiteRtt as RefEntite, tbmEntiteEntite.RefEntite as RefEntiteRtt, tblEntite.Libelle"
                        + "			from tbmEntiteEntite"
                        + "				inner join tblEntite on tbmEntiteEntite.RefEntite=tblEntite.RefEntite"
                        + "			where tblEntite.RefEntiteType=3 and tbmEntiteEntite.RefEntiteRtt= " + refEntite.ToString() + " ) as rtt on rtt.RefEntiteRtt=tblcommandeFournisseur.RefEntite "
                        + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                        + "     left join "
                        + "     	(select RefCommandeFournisseur, sum(Poids) as Poids"
                        + "     	from tblRepartition inner join tblRepartitionCollectivite on tblRepartition.RefRepartition=tblRepartitionCollectivite.RefRepartition"
                        + "     	group by RefCommandeFournisseur) as unitaire on tblCommandeFournisseur.RefCommandeFournisseur=unitaire.RefCommandeFournisseur      "
                        + " where RefusCamion=0 and (unitaire.RefCommandeFournisseur is null or PoidsReparti<>unitaire.Poids) and tblProduit.Collecte=1"
                        + "     and (RefTransporteur is not null and RefAdresseClient is not null and DChargementPrevue is not null and DDechargementPrevue is not null and DChargement is not null and DDechargement is not null and PoidsDechargement<>0 and NbBalleDechargement<>0 and PoidsReparti<>0 and NbBalleChargement<>0)"
                        + "     and DDechargement between convert(datetime, '" + t.Begin.ToString("dd/MM/yyyy") + "' ,103) and convert(datetime, '" + t.End.ToString("dd/MM/yyyy") + "' , 103)";
                    if (Utils.Utils.DbScalar(sqlStr, DbContext.Database.GetDbConnection()) == "0")
                    {
                        docs.Add(t);
                    }
                }
            }
            //End
            return new JsonResult(docs, JsonSettings);
        }

        /// <summary>
        /// GET: api/items/getetatmensuelhcss
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getetatmensuelhcss")]
        public IActionResult GetEtatMensuelHCSs()
        {
            int? refEntite = (Request.Query.ContainsKey("refEntite") ? (int?)System.Convert.ToInt32(Request.Query["refEntite"]) : null);
            var entite = DbContext.Entites.Where(i => i.RefEntite == refEntite).FirstOrDefault();
            // handle requests asking for non-existing object
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Process
            List<Month> docs = new();
            //Recherche des derniers états complets sur les 5 dernières années
            DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //Recalage de la date de référence pour laisser du délai avant la présentation des états
            d = d.AddDays(-8);
            Month m = new Month(d, CurrentContext.CurrentCulture);
            d = m.Begin;
            string sqlStr = "";
            //Chargement des documents à télécharger
            for (int i = 1; i < 60; i++)
            {
                d = d.AddMonths(-1);
                m = new Month(d, CurrentContext.CurrentCulture);
                //Seulement pour les mois concernant la collectivité
                sqlStr = "select count(*) "
                    + " from  tblRepartitionProduit"
                    + " 	inner join tblRepartition on tblRepartition.RefRepartition=tblRepartitionProduit.RefRepartition"
                    + " 	left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + " where year(tblCommandeFournisseur.DDechargement)=" + m.Year.ToString() + " and month(tblCommandeFournisseur.DDechargement)=" + m.Nb.ToString()
                    + " 	and tblRepartitionProduit.RefFournisseur=" + refEntite.ToString();
                if (Utils.Utils.DbScalar(sqlStr, DbContext.Database.GetDbConnection()) != "0")
                {
                    docs.Add(m);
                }
            }
            //End
            return new JsonResult(docs, JsonSettings);
        }

        /// <summary>
        /// GET: api/items/getcertificatrecyclagecss
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getcertificatrecyclagecss")]
        public IActionResult GetCertificatRecyclageCSs()
        {
            int? refEntite = (Request.Query.ContainsKey("refEntite") ? (int?)System.Convert.ToInt32(Request.Query["refEntite"]) : null);
            var entite = DbContext.Entites.Where(i => i.RefEntite == refEntite).FirstOrDefault();
            // handle requests asking for non-existing object
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Process
            List<Quarter> docs = new();
            //Recherche des derniers états complets sur les 5 dernières années
            DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //Recalage de la date de référence pour laisser du délai avant la présentation des états
            d = d.AddDays(-19);
            Quarter t = new Quarter(d, CurrentContext.CurrentCulture);
            d = t.Begin;
            string sqlStr = "";
            string s = "";
            int n = 0;
            //Chargement des documents à télécharger
            for (int i = 1; i < 20; i++)
            {
                d = d.AddMonths(-3);
                t = new Quarter(d, CurrentContext.CurrentCulture);
                //Firt possible date is 01/01/2023
                //if (t.Begin < new DateTime(2023, 7, 1)) { break; }
                //Process for possible dates
                //Seulement pour les Quarters concernant la collectivité
                sqlStr = "select count(*) "
                    + " from  tblRepartitionCollectivite"
                    + " 	inner join tblRepartition on tblRepartition.RefRepartition=tblRepartitionCollectivite.RefRepartition"
                    + " 	left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + " where tblCommandeFournisseur.DDechargement between convert(datetime, '" + t.Begin.ToString("dd/MM/yyyy") + "' ,103) and convert(datetime, '" + t.End.ToString("dd/MM/yyyy") + "' , 103) "
                    + " 	and RefCollectivite=" + refEntite.ToString();
                if (Utils.Utils.DbScalar(sqlStr, DbContext.Database.GetDbConnection()) != "0")
                {
                    docs.Add(t);
                }
            }
            //End
            return new JsonResult(docs, JsonSettings);
        }

        /// <summary>
        /// GET: api/items/getcertificatrecyclagehcss
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getcertificatrecyclagehcss")]
        public IActionResult GetCertificatRecyclageHCSs()
        {
            int? refEntite = (Request.Query.ContainsKey("refEntite") ? (int?)System.Convert.ToInt32(Request.Query["refEntite"]) : null);
            var entite = DbContext.Entites.Where(i => i.RefEntite == refEntite).FirstOrDefault();
            // handle requests asking for non-existing object
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Process
            List<Quarter> docs = new();
            //Recherche des derniers états complets sur les 5 dernières années
            DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //Recalage de la date de référence pour laisser du délai avant la présentation des états
            d = d.AddDays(-19);
            Quarter t = new Quarter(d, CurrentContext.CurrentCulture);
            d = t.Begin;
            string sqlStr = "";
            string s = "";
            int n = 0;
            //Chargement des documents à télécharger
            for (int i = 1; i < 20; i++)
            {
                d = d.AddMonths(-3);
                t = new Quarter(d, CurrentContext.CurrentCulture);
                //Firt possible date is 01/01/2023
                //if (t.Begin < new DateTime(2023, 1, 1)) { break; }
                //Process for possible dates
                //Seulement pour les Quarters concernant la collectivité
                sqlStr = "select count(*) "
                    + " from  tblRepartitionProduit"
                    + " 	inner join tblRepartition on tblRepartition.RefRepartition=tblRepartitionProduit.RefRepartition"
                    + " 	left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + " where tblCommandeFournisseur.DDechargement between convert(datetime, '" + t.Begin.ToString("dd/MM/yyyy") + "' ,103) and convert(datetime, '" + t.End.ToString("dd/MM/yyyy") + "' , 103) "
                    + " 	and tblRepartitionProduit.RefFournisseur=" + refEntite.ToString();
                if (Utils.Utils.DbScalar(sqlStr, DbContext.Database.GetDbConnection()) != "0")
                {
                    docs.Add(t);
                }
            }
            //End
            return new JsonResult(docs, JsonSettings);
        }
        /// <summary>
        /// GET: api/entite/getetatmensuelhcss
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getetatmensuelreceptions")]
        public IActionResult GetEtatMensuelReceptions()
        {
            int? refEntite = (Request.Query.ContainsKey("refEntite") ? (int?)System.Convert.ToInt32(Request.Query["refEntite"]) : null);
            var entite = DbContext.Entites.Where(i => i.RefEntite == refEntite).FirstOrDefault();
            // handle requests asking for non-existing object
            if (entite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Process
            List<Month> docs = new();
            //Recherche des derniers états complets sur les 5 dernières années
            DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //Recalage de la date de référence pour laisser du délai avant la présentation des états
            d = d.AddDays(-8);
            Month m = new Month(d, CurrentContext.CurrentCulture);
            d = m.Begin;
            string sqlStr = "";
            //Chargement des documents à télécharger
            for (int i = 1; i < 60; i++)
            {
                d = d.AddMonths(-1);
                m = new Month(d, CurrentContext.CurrentCulture);
                //Seulement pour les mois concernant la collectivité
                sqlStr = "select count(*)"
                    + " from tblCommandeFournisseur"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + "     inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.refProduit"
                    + " where DDechargement between convert(datetime, '" + m.Begin.ToString("dd/MM/yyyy") + "' ,103) and convert(datetime, '" + m.End.ToString("dd/MM/yyyy") + "' ,103) and tblCommandeFournisseur.RefEntite=" + refEntite.ToString();
                if (Utils.Utils.DbScalar(sqlStr, DbContext.Database.GetDbConnection()) != "0")
                {
                    docs.Add(m);
                }
            }
            //End
            return new JsonResult(docs, JsonSettings);
        }

        /// <summary>
        /// Get existing file
        /// </summary>
        [HttpGet("getdocumenttype")]
        public IActionResult GetDocumentType()
        {
            string refEntite = Request.Headers["refEntite"].ToString();
            string refDocumentType = Request.Headers["refDocumentType"].ToString();
            string year = Request.Headers["year"].ToString();
            MemoryStream mS = null;
            string fileName = "tmp";
            //Get the file
            //Process
            mS = DocumentFileManagement.CreateDocumentType1(System.Convert.ToInt32(refEntite)
                , System.Convert.ToInt32(year), DbContext, _env.ContentRootPath);
            fileName = "DocElu.pdf";
            //Send file
            if (mS != null)
            {
                //Headers
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = fileName,
                    //FileName = fileName.Replace(" ", "_"),
                    // always prompt the user for downloading
                    Inline = false,
                };
                string s = string.Format("attachment; filename=\"{0}\"", HttpUtility.UrlEncode(fileName));
                Response.Headers.Remove("Content-Disposition");
                Response.Headers.Append("Content-Disposition", s);
                //End
                return new FileContentResult(mS.ToArray(), "application/octet-stream");
            }

            return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
        }

        #endregion

        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref Entite dataModel, EntiteViewModel viewModel)
        {
            //Deactivate evry EntiteEntite link if Entite is deactivated
            if ((dataModel.Actif ?? false) && !(viewModel.Actif ?? false))
            {
                foreach (var eE in viewModel.EntiteEntites)
                {
                    eE.Actif = false;
                }
            }
            //Entite data
            dataModel.RefEntiteType = (int)viewModel.EntiteType?.RefEntiteType;
            dataModel.Libelle = Utils.Utils.SetEmptyStringToNull(viewModel.Libelle);
            dataModel.CodeEE = Utils.Utils.SetEmptyStringToNull(viewModel.CodeEE);
            dataModel.RefEcoOrganisme = viewModel.EcoOrganisme?.RefEcoOrganisme;
            dataModel.Adelphe = viewModel.Adelphe ?? false;
            dataModel.Cmt = Utils.Utils.SetEmptyStringToNull(viewModel.Cmt);
            dataModel.AssujettiTVA = viewModel.AssujettiTVA ?? false;
            dataModel.CodeTVA = Utils.Utils.SetEmptyStringToNull(viewModel.CodeTVA);
            dataModel.CodeValorisation = Utils.Utils.SetEmptyStringToNull(viewModel.CodeValorisation);
            dataModel.SAGECodeComptable = Utils.Utils.SetEmptyStringToNull(viewModel.SAGECodeComptable);
            dataModel.SAGECompteTiers = Utils.Utils.SetEmptyStringToNull(viewModel.SAGECompteTiers);
            dataModel.RefSAGEConditionLivraison = viewModel.SAGEConditionLivraison?.RefSAGEConditionLivraison;
            dataModel.RefSAGEPeriodicite = viewModel.SAGEPeriodicite?.RefSAGEPeriodicite;
            dataModel.RefSAGEModeReglement = viewModel.SAGEModeReglement?.RefSAGEModeReglement;
            dataModel.RefSAGECategorieAchat = viewModel.SAGECategorieAchat?.RefSAGECategorieAchat;
            dataModel.RefSAGECategorieVente = viewModel.SAGECategorieVente?.RefSAGECategorieVente;
            dataModel.ActionnaireProprietaire = Utils.Utils.SetEmptyStringToNull(viewModel.ActionnaireProprietaire);
            dataModel.Exploitant = Utils.Utils.SetEmptyStringToNull(viewModel.Exploitant);
            dataModel.Capacite = viewModel.Capacite;
            dataModel.PopulationContratN = viewModel.PopulationContratN;
            dataModel.Horaires = Utils.Utils.SetEmptyStringToNull(viewModel.Horaires);
            dataModel.RefRepreneur = viewModel.Repreneur?.RefRepreneur;
            dataModel.RefRepriseType = viewModel.RepriseType?.RefRepriseType;
            dataModel.ExportSAGE = viewModel.ExportSAGE ?? false;
            dataModel.VisibiliteAffretementCommun = viewModel.VisibiliteAffretementCommun ?? false;
            dataModel.AutoControle = viewModel.AutoControle ?? false;
            dataModel.Actif = viewModel.Actif ?? false;
            dataModel.SousContrat = viewModel.SousContrat ?? false;
            dataModel.ReconductionIncitationQualite = viewModel.ReconductionIncitationQualite ?? false;
            dataModel.SurcoutCarburant = viewModel.SurcoutCarburant ?? false;
            dataModel.RefEquipementier = viewModel.Equipementier?.RefEquipementier;
            dataModel.RefFournisseurTO = viewModel.FournisseurTO?.RefFournisseurTO;
            dataModel.DimensionBalle = Utils.Utils.SetEmptyStringToNull(viewModel.DimensionBalle);
            dataModel.IdNational = Utils.Utils.SetEmptyStringToNull(viewModel.IdNational);
            //Adresses
            //Dirty marker
            bool dirty = false;
            //-----------------------------------------------------------------------------------
            //Remove related data Adresse is deletable
            if (dataModel.Adresses != null)
            {
                foreach (Adresse adr in dataModel.Adresses)
                {
                    if (adr.IsDeletable(false) == "")
                    {
                        if (viewModel.Adresses == null)
                        {
                            DbContext.Adresses.Remove(adr);
                            dirty = true;
                        }
                        else if (viewModel.Adresses.Where(el => el.RefAdresse == adr.RefAdresse).FirstOrDefault() == null)
                        {
                            DbContext.Adresses.Remove(adr);
                            dirty = true;
                        }
                    }
                }
            }
            //Add or update related data Adresses
            foreach (AdresseViewModel adrVM in viewModel.Adresses)
            {
                Adresse adr = null;
                if (dataModel.Adresses != null && adrVM.RefAdresse > 0)
                {
                    adr = dataModel.Adresses.Where(el => el.RefAdresse == adrVM.RefAdresse).FirstOrDefault();
                }
                if (adr == null)
                {
                    adr = new Adresse();
                    DbContext.Adresses.Add(adr);
                    if (dataModel.Adresses == null) { dataModel.Adresses = new HashSet<Adresse>(); }
                    dataModel.Adresses.Add(adr);
                }
                //Mark as dirty if applicable and update data
                if (Utils.DataUtils.UpdateDataAdresse(ref adr, adrVM, CurrentContext.RefUtilisateur))
                {
                    dirty = true;
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data ContactAdresse is deletable
            if (dataModel.ContactAdresses != null)
            {
                foreach (ContactAdresse cA in dataModel.ContactAdresses)
                {
                    if (cA.IsDeletable(true) == "")
                    {
                        if (viewModel.ContactAdresses == null)
                        {
                            DbContext.ContactAdresses.Remove(cA);
                            dirty = true;
                        }
                        else if (viewModel.ContactAdresses.Where(el => el.RefContactAdresse == cA.RefContactAdresse).FirstOrDefault() == null)
                        {
                            DbContext.ContactAdresses.Remove(cA);
                            dirty = true;
                        }
                    }
                }
            }
            //Add or update related data ContactAdresses
            foreach (ContactAdresseViewModel cAVM in viewModel.ContactAdresses)
            {
                ContactAdresse cA = null;
                if (dataModel.ContactAdresses != null && cAVM.RefContactAdresse > 0)
                {
                    cA = dataModel.ContactAdresses.Where(el => el.RefContactAdresse == cAVM.RefContactAdresse).FirstOrDefault();
                }
                if (cA == null)
                {
                    cA = new ContactAdresse();
                    cA.Contact = new Contact();
                    DbContext.ContactAdresses.Add(cA);
                    if (dataModel.ContactAdresses == null) { dataModel.ContactAdresses = new HashSet<ContactAdresse>(); }
                    dataModel.ContactAdresses.Add(cA);
                }
                else
                {
                    //Create Message for administrators if disabled and user associated
                    if (cA.Actif == true && cAVM.Actif == false && cA.Utilisateurs?.Count > 0)
                    {
                        //Create message
                        string corps = "", corpsHTML = " ";
                        corps = cA.Contact.Prenom + " " + cA.Contact.Nom
                            + Environment.NewLine
                            + cA.Entite.Libelle
                            + Environment.NewLine
                            + cA.Entite.CodeEE
                            ;
                        corpsHTML = System.Net.WebUtility.HtmlEncode(cA.Contact.Prenom + " " + cA.Contact.Nom)
                            + "<br/>"
                            + System.Net.WebUtility.HtmlEncode(cA.Entite.Libelle)
                            + "<br/>"
                            + System.Net.WebUtility.HtmlEncode(cA.Entite.CodeEE)
                            + "<br/>"
                            + "<a href=\"/*appLink=entite/" + cA.RefContactAdresse.ToString() + ";elementType=ContactAdresse\">Afficher le contact</a>"
                            + "<br/>"
                            + "<a href=\"/*appLink=utilisateur/" + cA.Utilisateurs.First().RefUtilisateur.ToString() + "\">Afficher l'utilisateur</a>"
                              ;
                        corpsHTML = "<span style=\"font-family:Roboto; font-size:11pt;\">" + corpsHTML + "</span>";
                        var msg = new Message()
                        {
                            RefUtilisateurCourant = CurrentContext.RefUtilisateur,
                            RefMessageType = 3,
                            Libelle = "Accès contact à désactiver",
                            Titre = "Accès contact à désactiver",
                            Corps = corps,
                            CorpsHTML = corpsHTML,
                            DiffusionUnique = true,
                            VisualisationConfirmeUnique = true,
                            DDebut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                            DFin = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).AddDays(7),
                            Actif = true,
                            Important = true
                        };
                        DbContext.Messages.Add(msg);
                        //Manage Diffusion
                        msg.MessageDiffusions = new HashSet<MessageDiffusion>();
                        msg.MessageDiffusions.Add(new MessageDiffusion()
                        {
                            RefModule = Enumerations.Module.Administration.ToString(),
                            RefHabilitation = Enumerations.HabilitationAdministration.Administrateur.ToString()
                        });
                    }
                }
                //Link to new Adresse if applicable
                if (cAVM.RefAdresse < 0)
                {
                    //Find new adresse
                    var newAdr = dataModel.Adresses.SingleOrDefault(e => e.RefAdresse == cAVM.RefAdresse);
                    if (newAdr != null) { cA.Adresse = newAdr; }
                }
                //Mark as dirty if applicable and update data
                if (Utils.DataUtils.UpdateDataContactAdresse(ref cA, cAVM, CurrentContext.RefUtilisateur))
                {
                    dirty = true;
                }
            }
            //Reset all Adresse temporary primary keys for new Adresses
            if (dataModel.Adresses != null)
            {
                foreach (Adresse adr in dataModel.Adresses)
                {
                    if (adr.RefAdresse < 0) { adr.RefAdresse = 0; }
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data Action if deletable
            if (dataModel.Actions != null)
            {
                foreach (Data.Action a in dataModel.Actions)
                {
                    if (viewModel.Actions == null)
                    {
                        DbContext.Actions.Remove(a);
                        dirty = true;
                    }
                    else if (viewModel.Actions.Where(el => el.RefAction == a.RefAction).FirstOrDefault() == null)
                    {
                        DbContext.Actions.Remove(a);
                        dirty = true;
                    }
                }
            }
            //Add or update related data Actions
            foreach (ActionViewModel aVM in viewModel.Actions)
            {
                Data.Action a = null;
                if (dataModel.Actions != null && aVM.RefAction > 0)
                {
                    a = dataModel.Actions.Where(el => el.RefAction == aVM.RefAction).FirstOrDefault();
                }
                if (a == null)
                {
                    a = new Data.Action();
                    DbContext.Actions.Add(a);
                    if (dataModel.Actions == null) { dataModel.Actions = new HashSet<Data.Action>(); }
                    //Attach all newly uploaded files
                    foreach (var aF in aVM.ActionFichierNoFiles)
                    {
                        if (aF.RefAction <= 0)
                        {
                            var dbFile = DbContext.ActionFichierNoFiles.Where(e => e.RefActionFichier == aF.RefActionFichier).FirstOrDefault();
                            if (dbFile != null)
                            {
                                if (a.ActionFichierNoFiles == null) { a.ActionFichierNoFiles = new HashSet<ActionFichierNoFile>(); }
                                a.ActionFichierNoFiles.Add(dbFile);
                            }
                        }
                    }
                    //Add to dataModel
                    dataModel.Actions.Add(a);
                }
                //Link to new ContactAdresse if applicable
                if (aVM.ContactAdresse?.RefContactAdresse < 0)
                {
                    //Find new adresse
                    var newCA = dataModel.ContactAdresses.SingleOrDefault(e => e.RefContactAdresse == aVM.ContactAdresse.RefContactAdresse);
                    if (newCA != null) { a.RefContactAdresse = null; a.ContactAdresse = newCA; }
                }
                //Mark as dirty if applicable and update data
                if (Utils.DataUtils.UpdateDataAction(ref a, aVM, CurrentContext.RefUtilisateur))
                {
                    dirty = true;
                }
            }
            //Reset all ContactAdresse temporary primary keys for new Adresses
            if (dataModel.ContactAdresses != null)
            {
                foreach (ContactAdresse cA in dataModel.ContactAdresses)
                {
                    if (cA.RefContactAdresse < 0) { cA.RefContactAdresse = 0; }
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data EntiteEntite is deletable
            if (dataModel.EntiteEntites != null)
            {
                foreach (EntiteEntite eE in dataModel.EntiteEntites)
                {
                    if (viewModel.EntiteEntites == null)
                    {
                        DbContext.EntiteEntites.Remove(eE);
                        dirty = true;
                    }
                    else if (viewModel.EntiteEntites.Where(el => el.RefEntiteEntite == eE.RefEntiteEntite).FirstOrDefault() == null)
                    {
                        DbContext.EntiteEntites.Remove(eE);
                        dirty = true;
                    }
                }
            }
            //Add or update related data EntiteEntites
            if (dataModel.EntiteEntiteInits == null) { dataModel.EntiteEntiteInits = new HashSet<EntiteEntite>(); }
            foreach (EntiteEntiteViewModel eEVM in viewModel.EntiteEntites)
            {
                EntiteEntite eE = null;
                if (dataModel.EntiteEntites != null && eEVM.RefEntiteEntite > 0)
                {
                    eE = DbContext.EntiteEntites.Where(el => el.RefEntiteEntite == eEVM.RefEntiteEntite).FirstOrDefault();
                    if (eE.RefEntite == viewModel.RefEntite) { dataModel.EntiteEntiteInits.Add(eE); }
                    else { dataModel.EntiteEntiteRttInits.Add(eE); }
                }
                if (eE == null)
                {
                    //Check for duplicates
                    if (viewModel.EntiteEntites.Where(el => el.RefEntiteEntite != eEVM.RefEntiteEntite
                      && ((el.RefEntite == eEVM.RefEntite && el.RefEntiteRtt == eEVM.RefEntiteRtt) || (el.RefEntite == eEVM.RefEntiteRtt && el.RefEntiteRtt == eEVM.RefEntite))
                     )
                        .FirstOrDefault() == null)
                    {
                        //Create entity
                        eE = new EntiteEntite();
                        DbContext.EntiteEntites.Add(eE);
                        if (dataModel.EntiteEntites == null) { dataModel.EntiteEntites = new HashSet<EntiteEntite>(); }
                        dataModel.EntiteEntiteInits.Add(eE);
                    }
                }
                //Mark as dirty if applicable and update data
                if (eE != null)
                {
                    if (Utils.DataUtils.UpdateDataEntiteEntite(ref eE, eEVM, CurrentContext.RefUtilisateur))
                    {
                        dirty = true;
                    }
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data EntiteCamionType is deletable
            if (dataModel.EntiteCamionTypes != null)
            {
                foreach (EntiteCamionType eCT in dataModel.EntiteCamionTypes)
                {
                    if (viewModel.EntiteCamionTypes == null)
                    {
                        DbContext.EntiteCamionTypes.Remove(eCT);
                        dirty = true;
                    }
                    else if (viewModel.EntiteCamionTypes.Where(el => el.RefEntiteCamionType == eCT.RefEntiteCamionType).FirstOrDefault() == null)
                    {
                        DbContext.EntiteCamionTypes.Remove(eCT);
                        dirty = true;
                    }
                }
            }
            //Add or update related data EntiteCamionTypes
            if (dataModel.EntiteCamionTypes == null) { dataModel.EntiteCamionTypes = new HashSet<EntiteCamionType>(); }
            foreach (EntiteCamionTypeViewModel eCTVM in viewModel.EntiteCamionTypes)
            {
                EntiteCamionType eCT = null;
                if (dataModel.EntiteCamionTypes != null && eCTVM.RefEntiteCamionType > 0)
                {
                    eCT = dataModel.EntiteCamionTypes.Where(el => el.RefEntiteCamionType == eCTVM.RefEntiteCamionType).FirstOrDefault();
                }
                if (eCT == null)
                {
                    //Check for duplicates
                    if (viewModel.EntiteCamionTypes.Where(el => el.RefEntiteCamionType != eCTVM.RefEntiteCamionType && el.CamionType.RefCamionType == eCTVM.CamionType.RefCamionType)
                        .FirstOrDefault() == null)
                    {
                        //Create entity
                        eCT = new EntiteCamionType();
                        DbContext.EntiteCamionTypes.Add(eCT);
                        if (dataModel.EntiteCamionTypes == null) { dataModel.EntiteCamionTypes = new HashSet<EntiteCamionType>(); }
                        dataModel.EntiteCamionTypes.Add(eCT);
                    }
                }
                //Mark as dirty if applicable and update data
                if (eCT != null)
                {
                    if (Utils.DataUtils.UpdateDataEntiteCamionType(ref eCT, eCTVM, CurrentContext.RefUtilisateur))
                    {
                        dirty = true;
                    }
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data ContratIncitationQualite is deletable
            if (dataModel.ContratIncitationQualites != null)
            {
                foreach (ContratIncitationQualite cC in dataModel.ContratIncitationQualites)
                {
                    if (viewModel.ContratIncitationQualites == null)
                    {
                        DbContext.ContratIncitationQualites.Remove(cC);
                        dirty = true;
                    }
                    else if (viewModel.ContratIncitationQualites.Where(el => el.RefContratIncitationQualite == cC.RefContratIncitationQualite).FirstOrDefault() == null)
                    {
                        DbContext.ContratIncitationQualites.Remove(cC);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ContratIncitationQualites
            foreach (ContratIncitationQualiteViewModel cCVM in viewModel.ContratIncitationQualites)
            {
                ContratIncitationQualite cC = null;
                if (dataModel.ContratIncitationQualites != null && cCVM.RefContratIncitationQualite > 0)
                {
                    cC = dataModel.ContratIncitationQualites.Where(el => el.RefContratIncitationQualite == cCVM.RefContratIncitationQualite).FirstOrDefault();
                }
                if (cC == null)
                {
                    cC = new ContratIncitationQualite();
                    DbContext.ContratIncitationQualites.Add(cC);
                    if (dataModel.ContratIncitationQualites == null) { dataModel.ContratIncitationQualites = new HashSet<ContratIncitationQualite>(); }
                    dataModel.ContratIncitationQualites.Add(cC);
                }
                //Mark as dirty if applicable and update data
                if (Utils.DataUtils.UpdateDataContratIncitationQualite(ref cC, cCVM, CurrentContext.RefUtilisateur))
                {
                    dirty = true;
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data Contrat if deletable
            if (dataModel.Contrats != null)
            {
                foreach (Contrat cC in dataModel.Contrats)
                {
                    if (viewModel.Contrats == null)
                    {
                        DbContext.Contrats.Remove(cC);
                        dirty = true;
                    }
                    else if (viewModel.Contrats.Where(el => el.RefContrat == cC.RefContrat).Count() == 0)
                    {
                        DbContext.Contrats.Remove(cC);
                        dirty = true;
                    }
                }
            }
            //Add or update related data Contrats
            foreach (ContratViewModel cCVM in viewModel.Contrats)
            {
                Contrat cC = null;
                if (dataModel.Contrats != null && cCVM.RefContrat > 0)
                {
                    cC = dataModel.Contrats.Where(el => el.RefContrat == cCVM.RefContrat).FirstOrDefault();
                }
                if (cC == null)
                {
                    cC = new Contrat();
                    DbContext.Contrats.Add(cC);
                    if (dataModel.Contrats == null) { dataModel.Contrats = new HashSet<Contrat>(); }
                    dataModel.Contrats.Add(cC);
                }
                //Mark as dirty if applicable and update data
                if (Utils.DataUtils.UpdateDataContrat(ref cC, cCVM, CurrentContext.RefUtilisateur))
                {
                    //Remove Contrat if no more ContratEntites
                    if (cC.ContratEntites.Count == 0)
                    {
                        DbContext.Contrats.Remove(cC);
                    }
                    //Mark dirty
                    dirty = true;
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data ContratCollectivite is deletable
            if (dataModel.ContratCollectivites != null)
            {
                foreach (ContratCollectivite cC in dataModel.ContratCollectivites)
                {
                    if (viewModel.ContratCollectivites == null)
                    {
                        DbContext.ContratCollectivites.Remove(cC);
                        dirty = true;
                    }
                    else if (viewModel.ContratCollectivites.Where(el => el.RefContratCollectivite == cC.RefContratCollectivite).FirstOrDefault() == null)
                    {
                        DbContext.ContratCollectivites.Remove(cC);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ContratCollectivites
            foreach (ContratCollectiviteViewModel cCVM in viewModel.ContratCollectivites)
            {
                ContratCollectivite cC = null;
                if (dataModel.ContratCollectivites != null && cCVM.RefContratCollectivite > 0)
                {
                    cC = dataModel.ContratCollectivites.Where(el => el.RefContratCollectivite == cCVM.RefContratCollectivite).FirstOrDefault();
                }
                if (cC == null)
                {
                    cC = new ContratCollectivite();
                    DbContext.ContratCollectivites.Add(cC);
                    if (dataModel.ContratCollectivites == null) { dataModel.ContratCollectivites = new HashSet<ContratCollectivite>(); }
                    dataModel.ContratCollectivites.Add(cC);
                }
                //Mark as dirty if applicable and update data
                if (Utils.DataUtils.UpdateDataContratCollectivite(ref cC, cCVM, CurrentContext.RefUtilisateur))
                {
                    dirty = true;
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data EntiteDR is deletable
            if (dataModel.EntiteDRs != null)
            {
                foreach (EntiteDR eD in dataModel.EntiteDRs)
                {
                    if (viewModel.EntiteDRs == null)
                    {
                        DbContext.EntiteDRs.Remove(eD);
                        dirty = true;
                    }
                    else if (viewModel.EntiteDRs.Where(el => el.DR.RefUtilisateur == eD.RefDR).FirstOrDefault() == null)
                    {
                        DbContext.EntiteDRs.Remove(eD);
                        dirty = true;
                    }
                }
            }
            //Add or update related data EntiteDRs
            foreach (EntiteDRViewModel eDVM in viewModel.EntiteDRs)
            {
                EntiteDR eD = null;
                if (dataModel.EntiteDRs != null)
                {
                    eD = dataModel.EntiteDRs.Where(el => el.RefDR == eDVM.DR.RefUtilisateur).FirstOrDefault();
                }
                if (eD == null)
                {
                    //Create entity
                    eD = new EntiteDR() { RefDR = eDVM.DR.RefUtilisateur, };
                    if (dataModel.EntiteDRs == null) { dataModel.EntiteDRs = new HashSet<EntiteDR>(); }
                    dataModel.EntiteDRs.Add(eD);
                    dirty = true;
                }
            }
            //Remove duplicates
            if (dataModel.EntiteDRs != null)
            {
                var resultEDR = dataModel.EntiteDRs
                    .AsEnumerable()
                    .GroupBy(s => s.RefDR)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultEDR.Count > 0)
                {
                    foreach (var e in resultEDR)
                    {
                        dataModel.EntiteDRs.Remove(e);
                    }
                    dirty = true;
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data EntiteProcess is deletable
            if (dataModel.EntiteProcesss != null)
            {
                foreach (EntiteProcess eP in dataModel.EntiteProcesss)
                {
                    if (viewModel.EntiteProcesss == null)
                    {
                        DbContext.EntiteProcesss.Remove(eP);
                        dirty = true;
                    }
                    else if (viewModel.EntiteProcesss.Where(el => el.Process.RefProcess == eP.RefProcess).FirstOrDefault() == null)
                    {
                        DbContext.EntiteProcesss.Remove(eP);
                        dirty = true;
                    }
                }
            }
            //Add or update related data EntiteProcesss
            foreach (EntiteProcessViewModel eDVM in viewModel.EntiteProcesss)
            {
                EntiteProcess eP = null;
                if (dataModel.EntiteProcesss != null)
                {
                    eP = dataModel.EntiteProcesss.Where(el => el.RefProcess == eDVM.Process.RefProcess).FirstOrDefault();
                }
                if (eP == null)
                {
                    //Create entity
                    eP = new EntiteProcess() { RefProcess = eDVM.Process.RefProcess, };
                    if (dataModel.EntiteProcesss == null) { dataModel.EntiteProcesss = new HashSet<EntiteProcess>(); }
                    dataModel.EntiteProcesss.Add(eP);
                    dirty = true;
                }
            }
            //Remove duplicates
            if (dataModel.EntiteProcesss != null)
            {
                var resultP = dataModel.EntiteProcesss
                    .AsEnumerable()
                    .GroupBy(s => s.RefProcess)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultP.Count > 0)
                {
                    foreach (var e in resultP)
                    {
                        dataModel.EntiteProcesss.Remove(e);
                    }
                    dirty = true;
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data EntiteProduit is deletable
            if (dataModel.EntiteProduits != null)
            {
                foreach (EntiteProduit eP in dataModel.EntiteProduits)
                {
                    if (viewModel.EntiteProduits == null)
                    {
                        DbContext.EntiteProduits.Remove(eP);
                        dirty = true;
                    }
                    else if (viewModel.EntiteProduits.Where(el => el.RefEntiteProduit == eP.RefEntiteProduit).FirstOrDefault() == null)
                    {
                        DbContext.EntiteProduits.Remove(eP);
                        dirty = true;
                    }
                }
            }
            //Add or update related data EntiteProduits
            if (dataModel.EntiteProduits == null) { dataModel.EntiteProduits = new HashSet<EntiteProduit>(); }
            foreach (EntiteProduitViewModel eCTVM in viewModel.EntiteProduits)
            {
                EntiteProduit eP = null;
                if (dataModel.EntiteProduits != null && eCTVM.RefEntiteProduit > 0)
                {
                    eP = dataModel.EntiteProduits.Where(el => el.RefEntiteProduit == eCTVM.RefEntiteProduit).FirstOrDefault();
                }
                if (eP == null)
                {
                    //Check for duplicates
                    if (viewModel.EntiteProduits.Where(el => el.RefEntiteProduit != eCTVM.RefEntiteProduit && el.Produit.RefProduit == eCTVM.Produit.RefProduit)
                        .FirstOrDefault() == null)
                    {
                        //Create entity
                        eP = new EntiteProduit();
                        DbContext.EntiteProduits.Add(eP);
                        if (dataModel.EntiteProduits == null) { dataModel.EntiteProduits = new HashSet<EntiteProduit>(); }
                        dataModel.EntiteProduits.Add(eP);
                    }
                }
                //Mark as dirty if applicable and update data
                if (eP != null)
                {
                    if (Utils.DataUtils.UpdateDataEntiteProduit(ref eP, eCTVM, CurrentContext.RefUtilisateur))
                    {
                        dirty = true;
                    }
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data EntiteStandard is deletable
            if (dataModel.EntiteStandards != null)
            {
                foreach (EntiteStandard eD in dataModel.EntiteStandards)
                {
                    if (viewModel.EntiteStandards == null)
                    {
                        DbContext.EntiteStandards.Remove(eD);
                        dirty = true;
                    }
                    else if (viewModel.EntiteStandards.Where(el => el.Standard.RefStandard == eD.RefStandard).FirstOrDefault() == null)
                    {
                        DbContext.EntiteStandards.Remove(eD);
                        dirty = true;
                    }
                }
            }
            //Add or update related data EntiteStandards
            foreach (EntiteStandardViewModel eDVM in viewModel.EntiteStandards)
            {
                EntiteStandard eD = null;
                if (dataModel.EntiteStandards != null)
                {
                    eD = dataModel.EntiteStandards.Where(el => el.RefStandard == eDVM.Standard.RefStandard).FirstOrDefault();
                }
                if (eD == null)
                {
                    //Create entity
                    eD = new EntiteStandard() { RefStandard = eDVM.Standard.RefStandard, };
                    if (dataModel.EntiteStandards == null) { dataModel.EntiteStandards = new HashSet<EntiteStandard>(); }
                    dataModel.EntiteStandards.Add(eD);
                    dirty = true;
                }
            }
            //Remove duplicates
            if (dataModel.EntiteStandards != null)
            {
                var resultS = dataModel.EntiteStandards
                    .AsEnumerable()
                    .GroupBy(s => s.RefStandard)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultS.Count > 0)
                {
                    foreach (var e in resultS)
                    {
                        dataModel.EntiteStandards.Remove(e);
                    }
                    dirty = true;
                }
            }
            //Files
            //delete files
            if (dataModel.DocumentEntites != null)
            {
                foreach (DocumentEntite dE in dataModel.DocumentEntites)
                {
                    if (viewModel.DocumentEntites.Where(el => el.RefDocumentEntite == dE.RefDocumentEntite).FirstOrDefault() == null)
                    {
                        DbContext.DocumentEntites.Remove(dE);
                    }
                }
            }
            //Attach all newly uploaded files
            foreach (var dE in viewModel.DocumentEntites)
            {
                if (dE.RefEntite <= 0)
                {
                    var dbFile = DbContext.DocumentEntites.Where(e => e.RefDocumentEntite == dE.RefDocumentEntite).FirstOrDefault();
                    if (dbFile != null)
                    {
                        if (dataModel.DocumentEntites == null) { dataModel.DocumentEntites = new HashSet<DocumentEntite>(); }
                        dataModel.DocumentEntites.Add(dbFile);
                    }
                }
            }
            //Remove duplicates
            if (dataModel.DocumentEntites != null)
            {
                var resultDE = dataModel.DocumentEntites
                    .AsEnumerable()
                    .GroupBy(s => s.RefDocument)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultDE.Count > 0)
                {
                    foreach (var e in resultDE)
                    {
                        dataModel.DocumentEntites.Remove(e);
                    }
                    dirty = true;
                }
            }
            //Mark for modification if applicable
            if (dirty) { dataModel.DModif = DateTime.Now; }
        }
        /// <summary>
        /// Delete related data EntiteEntite
        /// </summary>
        private void DeleteEntiteEntite(Entite dataModel)
        {
            int refEntite = dataModel.RefEntite;
            var l = DbContext.EntiteEntites
                .Where(e => e.RefEntite == refEntite || e.RefEntiteRtt == refEntite)
                .ToList();
            DbContext.EntiteEntites.RemoveRange(l);
            DbContext.SaveChanges();
        }
        #endregion
    }
}
