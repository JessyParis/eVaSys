/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 01/07/2020
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using eVaSys.APIUtils;
using System.Collections.Generic;
using System;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class NonConformiteController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public NonConformiteController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/nonconformite/{id}
        /// Retrieves the NonConformite with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NonConformite</param>
        /// <returns>the NonConformite with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            NonConformite nonConformite = null;
            //Get or create nonConformite
            if (id == 0)
            {
                int refCommandeFournisseur = System.Convert.ToInt32(Request.Headers["refCommandeFournisseur"]);
                if (refCommandeFournisseur != 0)
                {
                    nonConformite = DbContext.NonConformites
                        .Include(r => r.NonConformiteEtapes)
                        .ThenInclude(r => r.UtilisateurCreation)
                        .Include(r => r.NonConformiteEtapes)
                        .ThenInclude(r => r.UtilisateurModif)
                        .Include(r => r.NonConformiteEtapes)
                        .ThenInclude(r => r.UtilisateurValide)
                        .Include(r => r.NonConformiteEtapes)
                        .ThenInclude(r => r.UtilisateurControle)
                        .Include(r => r.NonConformiteFichiers.OrderBy(o=>o.RefNonConformiteFichier))
                        .Include(r => r.NonConformiteNonConformiteFamilles)
                        .AsSplitQuery()
                        .Where(i => i.RefCommandeFournisseur == refCommandeFournisseur).FirstOrDefault();
                    if (nonConformite == null)
                    {
                        nonConformite = new NonConformite();
                        DbContext.NonConformites.Add(nonConformite);
                        nonConformite.RefCommandeFournisseur = refCommandeFournisseur;
                        nonConformite.NonConformiteEtapes = new HashSet<NonConformiteEtape>();
                        var nCETs = DbContext.NonConformiteEtapeTypes.OrderBy(o => o.Ordre).ToList();
                        foreach (var nCET in nCETs)
                        {
                            if (nCET.RefNonConformiteEtapeType == 6 || nCET.RefNonConformiteEtapeType == 7)
                            {
                                nonConformite.NonConformiteEtapes.Add(
                                    new NonConformiteEtape()
                                    {
                                        NonConformiteEtapeType = nCET,
                                        LibelleFRFR = nCET.LibelleFRFR,
                                        LibelleENGB = nCET.LibelleENGB,
                                        Ordre = nCET.Ordre,
                                        UtilisateurValide = DbContext.Utilisateurs.Where(e => e.RefUtilisateur == 1).FirstOrDefault(),
                                        RefUtilisateurValide = 1,
                                        DValide = DateTime.Now
                                    }) ;
                            }
                            else
                                nonConformite.NonConformiteEtapes.Add(
                                    new NonConformiteEtape()
                                    {
                                        NonConformiteEtapeType = nCET,
                                        LibelleFRFR = nCET.LibelleFRFR,
                                        LibelleENGB = nCET.LibelleENGB,
                                        Ordre = nCET.Ordre
                                    });
                        }
                    }
                }
            }
            else
            {
                nonConformite = DbContext.NonConformites
                    .Include(r => r.NonConformiteEtapes)
                    .ThenInclude(r => r.UtilisateurCreation)
                    .Include(r => r.NonConformiteEtapes)
                    .ThenInclude(r => r.UtilisateurModif)
                    .Include(r => r.NonConformiteEtapes)
                    .ThenInclude(r => r.UtilisateurValide)
                    .Include(r => r.NonConformiteEtapes)
                    .ThenInclude(r => r.UtilisateurControle)
                    .Include(r => r.NonConformiteFichiers.OrderBy(o => o.RefNonConformiteFichier))
                    .Include(r => r.NonConformiteNonConformiteFamilles)
                    .AsSplitQuery()
                    .Where(i => i.RefNonConformite == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (nonConformite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            nonConformite.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<NonConformite, NonConformiteViewModel>(nonConformite),
                JsonSettings);
        }

        /// <summary>
        /// Save the NonConformite with the given {id}
        /// </summary>
        /// <param name="model">The NonConformiteViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]NonConformiteViewModel model)
        {
            string valid = null;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            NonConformite nonConformite;
            //Retrieve or create the entity to edit
            if (model.RefNonConformite > 0)
            {
                nonConformite = DbContext.NonConformites
                    .Include(r => r.NonConformiteEtapes)
                    .ThenInclude(r => r.UtilisateurCreation)
                    .Include(r => r.NonConformiteEtapes)
                    .ThenInclude(r => r.UtilisateurModif)
                    .Include(r => r.NonConformiteEtapes)
                    .ThenInclude(r => r.UtilisateurValide)
                    .Include(r => r.NonConformiteEtapes)
                    .ThenInclude(r => r.UtilisateurControle)
                    .Include(r => r.NonConformiteNonConformiteFamilles)
                    .Include(r => r.NonConformiteFichiers)
                    .Where(q => q.RefNonConformite == model.RefNonConformite)
                    .AsSplitQuery()
                    .FirstOrDefault();
            }
            else
            {
                nonConformite = new NonConformite();
                DbContext.NonConformites.Add(nonConformite);
            }
            // handle requests asking for non-existing nonConformitezes
            if (nonConformite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref nonConformite, model);

            //Auto validate Etapes
            nonConformite.AutoValidateEtapes(CurrentContext.RefUtilisateur);

            //Check validation
            valid = nonConformite.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                DbContext.Entry(nonConformite).Reload();
                DbContext.Entry(nonConformite).Collection(r => r.NonConformiteEtapes).Load();
                // return the updated NonConformite to the client.
                return new JsonResult(
                    _mapper.Map<NonConformite, NonConformiteViewModel>(nonConformite),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the NonConformite with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the nonConformite from the Database
            var nonConformite = DbContext.NonConformites
                .Include(r => r.NonConformiteEtapes)
                .Include(r => r.NonConformiteFichiers)
                .Include(r => r.NonConformiteNonConformiteFamilles)
                .AsSplitQuery()
                .Where(i => i.RefNonConformite == id)
                .FirstOrDefault();
            // handle requests asking for non-existing nonConformitezes
            if (nonConformite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = nonConformite.IsDeletable();
            if (del == "")
            {
                DbContext.NonConformites.Remove(nonConformite);
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
        /// GET: api/nonconformite/getpictures
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of NonConformiteFichier.</returns>
        [HttpGet("getpictures")]
        public IActionResult GetPictures()
        {
            string refNonConformite = Request.Headers["refNonConformite"].ToString();
            string refNonConformiteFichierType = Request.Headers["refNonConformiteFichierType"].ToString();
            int refNC = 0, refNCFT = 0;
            //Check mandatory parameters
            if (int.TryParse(refNonConformite, out refNC) && int.TryParse(refNonConformiteFichierType, out refNCFT))
            {
                var pictures = DbContext.NonConformiteFichiers.Where(el => el.RefNonConformite == refNC && el.RefNonConformiteFichierType == refNCFT && el.Miniature != null)
                    .OrderBy(o => o.RefNonConformiteFichier)
                    .ToArray();
                //Return Json
                return new JsonResult(
                    _mapper.Map<NonConformiteFichier[], NonConformiteFichierMediumViewModel[]>(pictures),
                    JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref NonConformite dataModel, NonConformiteViewModel viewModel)
        {
            //Dirty marker
            bool dirty = false;
            //Remove related data NonConformiteEtape
            if (dataModel.NonConformiteEtapes != null)
            {
                foreach (NonConformiteEtape nCE in dataModel.NonConformiteEtapes)
                {
                    if (viewModel.NonConformiteEtapes == null)
                    {
                        DbContext.NonConformiteEtapes.Remove(nCE);
                        dirty = true;
                    }
                    else if (viewModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtape == nCE.RefNonConformiteEtape).FirstOrDefault() == null)
                    {
                        DbContext.NonConformiteEtapes.Remove(nCE);
                        dirty = true;
                    }
                }
            }
            //Add or update related data NonConformiteEtapes
            foreach (NonConformiteEtapeViewModel nCEVM in viewModel.NonConformiteEtapes)
            {
                NonConformiteEtape nCE = null;
                if (dataModel.NonConformiteEtapes != null && nCEVM.RefNonConformiteEtape != 0)
                {
                    nCE = dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtape == nCEVM.RefNonConformiteEtape).FirstOrDefault();
                }
                if (nCE == null)
                {
                    nCE = new NonConformiteEtape();
                    nCE.RefNonConformiteEtapeType = nCEVM.NonConformiteEtapeType.RefNonConformiteEtapeType;
                    nCE.Ordre = nCEVM.Ordre;
                    nCE.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(nCEVM.LibelleFRFR);
                    nCE.LibelleENGB = Utils.Utils.SetEmptyStringToNull(nCEVM.LibelleENGB);
                    DbContext.NonConformiteEtapes.Add(nCE);
                    if (dataModel.NonConformiteEtapes == null) { dataModel.NonConformiteEtapes = new HashSet<NonConformiteEtape>(); }
                    dataModel.NonConformiteEtapes.Add(nCE);
                }
            }
            //Manage Etapes creations/modifications
            if (Utils.Utils.SetEmptyStringToNull(viewModel.DescrClient) != dataModel.DescrClient
                || viewModel.NonConformiteDemandeClientType?.RefNonConformiteDemandeClientType != dataModel.RefNonConformiteDemandeClientType
                || viewModel.NonConformiteEtapes.Where(el=>el.NonConformiteEtapeType.RefNonConformiteEtapeType==1).FirstOrDefault().Cmt != dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtapeType == 1).FirstOrDefault().Cmt
                )
            {
                manageEtapeModification(ref dataModel, 1);
            }
            if (Utils.Utils.SetEmptyStringToNull(viewModel.DescrValorplast) != dataModel.DescrValorplast
                || viewModel.NonConformiteNature?.RefNonConformiteNature != dataModel.RefNonConformiteNature
                || viewModel.IFFournisseurRetourLot != dataModel.IFFournisseurRetourLot
                || Utils.Utils.SetEmptyStringToNull(viewModel.IFFournisseurDescr) != dataModel.IFFournisseurDescr
                || viewModel.IFFournisseurFactureMontant != dataModel.IFFournisseurFactureMontant
                || viewModel.IFFournisseurDeductionTonnage != dataModel.IFFournisseurDeductionTonnage
                || Utils.Utils.SetEmptyStringToNull(viewModel.IFClientDescr) != dataModel.IFClientDescr
                || viewModel.IFClientFactureMontant != dataModel.IFClientFactureMontant
                || modifiedNonConformiteFamille(dataModel, viewModel)
                || viewModel.NonConformiteEtapes.Where(el=>el.NonConformiteEtapeType.RefNonConformiteEtapeType==2).FirstOrDefault().Cmt != dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtapeType == 2).FirstOrDefault().Cmt
                )
            {
                manageEtapeModification(ref dataModel, 2);
            }
            if (viewModel.IFFournisseurTransmissionFacturation != dataModel.IFFournisseurTransmissionFacturation
                || viewModel.DTransmissionFournisseur != dataModel.DTransmissionFournisseur
                || Utils.Utils.SetEmptyStringToNull(viewModel.ActionDR) != dataModel.ActionDR
                || viewModel.NonConformiteReponseFournisseurType?.RefNonConformiteReponseFournisseurType != dataModel.RefNonConformiteReponseFournisseurType
                || Utils.Utils.SetEmptyStringToNull(viewModel.CmtReponseFournisseur) != dataModel.CmtReponseFournisseur
                || viewModel.NonConformiteEtapes.Where(el => el.NonConformiteEtapeType.RefNonConformiteEtapeType == 3).FirstOrDefault().Cmt != dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtapeType == 3).FirstOrDefault().Cmt
                )
            {
                manageEtapeModification(ref dataModel, 3);
            }
            if (viewModel.NonConformiteReponseClientType?.RefNonConformiteReponseClientType != dataModel.RefNonConformiteReponseClientType
                || Utils.Utils.SetEmptyStringToNull(viewModel.CmtReponseClient) != dataModel.CmtReponseClient
                || viewModel.NonConformiteEtapes.Where(el => el.NonConformiteEtapeType.RefNonConformiteEtapeType == 4).FirstOrDefault().Cmt != dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtapeType == 4).FirstOrDefault().Cmt
                )
            {
                manageEtapeModification(ref dataModel, 4);
            }
            if (viewModel.PlanAction != dataModel.PlanAction
                || Utils.Utils.SetEmptyStringToNull(viewModel.CmtOrigineAction) != dataModel.CmtOrigineAction
                || viewModel.NonConformiteEtapes.Where(el => el.NonConformiteEtapeType.RefNonConformiteEtapeType == 5).FirstOrDefault().Cmt != dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtapeType == 5).FirstOrDefault().Cmt
                )
            {
                manageEtapeModification(ref dataModel, 5);
            }
            if (Utils.Utils.SetEmptyStringToNull(viewModel.IFFournisseurBonCommandeNro) != dataModel.IFFournisseurBonCommandeNro
                || viewModel.IFFournisseurAttenteBonCommande != dataModel.IFFournisseurAttenteBonCommande
                || viewModel.IFFournisseurTransmissionFacturation != dataModel.IFFournisseurTransmissionFacturation
                || viewModel.IFFournisseurFactureNro != dataModel.IFFournisseurFactureNro
                || Utils.Utils.SetEmptyStringToNull(viewModel.IFFournisseurCmtFacturation) != dataModel.IFFournisseurCmtFacturation
                || viewModel.NonConformiteEtapes.Where(el => el.NonConformiteEtapeType.RefNonConformiteEtapeType == 6).FirstOrDefault().Cmt != dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtapeType == 6).FirstOrDefault().Cmt
                )
            {
                manageEtapeModification(ref dataModel, 6);
            }
            if (Utils.Utils.SetEmptyStringToNull(viewModel.IFClientFactureNro) != dataModel.IFClientFactureNro
                || viewModel.IFClientDFacture != dataModel.IFClientDFacture
                || Utils.Utils.SetEmptyStringToNull(viewModel.IFClientCmtFacturation) != dataModel.IFClientCmtFacturation
                || viewModel.NonConformiteEtapes.Where(el => el.NonConformiteEtapeType.RefNonConformiteEtapeType == 7).FirstOrDefault().Cmt != dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtapeType == 7).FirstOrDefault().Cmt
                )
            {
                manageEtapeModification(ref dataModel, 7);
            }
            if (viewModel.PriseEnCharge != dataModel.PriseEnCharge
                || viewModel.MontantPriseEnCharge != dataModel.MontantPriseEnCharge
                || Utils.Utils.SetEmptyStringToNull(viewModel.CmtPriseEnCharge) != dataModel.CmtPriseEnCharge
                || viewModel.NonConformiteEtapes.Where(el => el.NonConformiteEtapeType.RefNonConformiteEtapeType == 8).FirstOrDefault().Cmt != dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtapeType == 8).FirstOrDefault().Cmt
                )
            {
                manageEtapeModification(ref dataModel, 8);
            }
            //Save data
            dataModel.RefCommandeFournisseur = viewModel.CommandeFournisseur?.RefCommandeFournisseur;
            dataModel.DescrClient = Utils.Utils.SetEmptyStringToNull(viewModel.DescrClient);
            dataModel.RefNonConformiteDemandeClientType = viewModel.NonConformiteDemandeClientType?.RefNonConformiteDemandeClientType;
            dataModel.DescrValorplast = Utils.Utils.SetEmptyStringToNull(viewModel.DescrValorplast);
            dataModel.RefNonConformiteNature = viewModel.NonConformiteNature?.RefNonConformiteNature;
            dataModel.DTransmissionFournisseur = viewModel.DTransmissionFournisseur;
            dataModel.ActionDR = Utils.Utils.SetEmptyStringToNull(viewModel.ActionDR);
            dataModel.RefNonConformiteReponseFournisseurType = viewModel.NonConformiteReponseFournisseurType?.RefNonConformiteReponseFournisseurType;
            dataModel.CmtReponseFournisseur = Utils.Utils.SetEmptyStringToNull(viewModel.CmtReponseFournisseur);
            dataModel.RefNonConformiteReponseClientType = viewModel.NonConformiteReponseClientType?.RefNonConformiteReponseClientType;
            dataModel.CmtReponseClient = Utils.Utils.SetEmptyStringToNull(viewModel.CmtReponseClient);
            dataModel.PlanAction = viewModel.PlanAction ?? false;
            dataModel.CmtOrigineAction = Utils.Utils.SetEmptyStringToNull(viewModel.CmtOrigineAction);
            dataModel.IFFournisseurDescr = Utils.Utils.SetEmptyStringToNull(viewModel.IFFournisseurDescr);
            dataModel.IFFournisseurDeductionTonnage = viewModel.IFFournisseurDeductionTonnage;
            dataModel.IFFournisseurRetourLot = viewModel.IFFournisseurRetourLot ?? false;
            dataModel.RefNonConformiteAccordFournisseurType = viewModel.NonConformiteAccordFournisseurType?.RefNonConformiteAccordFournisseurType;
            dataModel.IFFournisseurAttenteBonCommande = viewModel.IFFournisseurAttenteBonCommande ?? false;
            dataModel.IFFournisseurBonCommandeNro = Utils.Utils.SetEmptyStringToNull(viewModel.IFFournisseurBonCommandeNro);
            dataModel.IFFournisseurFacture = viewModel.IFFournisseurFacture ?? false;
            dataModel.IFFournisseurFactureMontant = viewModel.IFFournisseurFactureMontant;
            dataModel.IFFournisseurFactureNro = Utils.Utils.SetEmptyStringToNull(viewModel.IFFournisseurFactureNro);
            dataModel.IFFournisseurTransmissionFacturation = viewModel.IFFournisseurTransmissionFacturation ?? false;
            dataModel.IFFournisseurCmtFacturation = Utils.Utils.SetEmptyStringToNull(viewModel.IFFournisseurCmtFacturation);
            dataModel.IFClientDescr = Utils.Utils.SetEmptyStringToNull(viewModel.IFClientDescr);
            dataModel.IFClientFactureMontant = viewModel.IFClientFactureMontant;
            dataModel.IFClientFactureNro = Utils.Utils.SetEmptyStringToNull(viewModel.IFClientFactureNro);
            dataModel.IFClientCmtFacturation = Utils.Utils.SetEmptyStringToNull(viewModel.IFClientCmtFacturation);
            dataModel.IFClientDFacture = viewModel.IFClientDFacture;
            dataModel.IFClientCommandeAFaire = viewModel.IFClientCommandeAFaire;
            dataModel.IFClientFactureEnAttente = viewModel.IFClientFactureEnAttente;
            dataModel.PriseEnCharge = viewModel.PriseEnCharge ?? false;
            dataModel.MontantPriseEnCharge = viewModel.MontantPriseEnCharge;
            dataModel.CmtPriseEnCharge = Utils.Utils.SetEmptyStringToNull(viewModel.CmtPriseEnCharge);
            //Remove related data NonConformiteNonConformiteFamille
            if (dataModel.NonConformiteNonConformiteFamilles != null)
            {
                foreach (NonConformiteNonConformiteFamille nCNCF in dataModel.NonConformiteNonConformiteFamilles)
                {
                    if (viewModel.NonConformiteNonConformiteFamilles == null)
                    {
                        DbContext.NonConformiteNonConformiteFamilles.Remove(nCNCF);
                        dirty = true;
                    }
                    else if (viewModel.NonConformiteNonConformiteFamilles.Where(el => el.NonConformiteFamille.RefNonConformiteFamille == nCNCF.NonConformiteFamille.RefNonConformiteFamille).FirstOrDefault() == null)
                    {
                        DbContext.NonConformiteNonConformiteFamilles.Remove(nCNCF);
                        dirty = true;
                    }
                }
            }
            //Add related data NonConformiteNonConformiteFamilles
            foreach (NonConformiteNonConformiteFamilleViewModel nCNCFVM in viewModel.NonConformiteNonConformiteFamilles)
            {
                if (dataModel.NonConformiteNonConformiteFamilles == null) { dataModel.NonConformiteNonConformiteFamilles = new HashSet<NonConformiteNonConformiteFamille>(); }
                if (dataModel.NonConformiteNonConformiteFamilles.Where(el => el.NonConformiteFamille.RefNonConformiteFamille == nCNCFVM.NonConformiteFamille.RefNonConformiteFamille).FirstOrDefault() == null)
                {
                    var nCNCF = new NonConformiteNonConformiteFamille();
                    nCNCF.RefNonConformiteFamille = (int)nCNCFVM.NonConformiteFamille.RefNonConformiteFamille;
                    DbContext.NonConformiteNonConformiteFamilles.Add(nCNCF);
                    if (dataModel.NonConformiteNonConformiteFamilles == null) { dataModel.NonConformiteNonConformiteFamilles = new HashSet<NonConformiteNonConformiteFamille>(); }
                    dataModel.NonConformiteNonConformiteFamilles.Add(nCNCF);
                    //Mark as dirty if applicable
                    dirty = true;
                }
            }
            //Remove duplicates
            if (dataModel.NonConformiteNonConformiteFamilles != null)
            {
                var resultNCF = dataModel.NonConformiteNonConformiteFamilles
                    .AsEnumerable()
                    .GroupBy(s => s.RefNonConformiteFamille)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultNCF.Count > 0)
                {
                    foreach (var e in resultNCF)
                    {
                        dataModel.NonConformiteNonConformiteFamilles.Remove(e);
                    }
                    dirty = true;
                }
            }
            //Files
            //delete files
            if (dataModel.NonConformiteFichiers != null)
            {
                foreach (NonConformiteFichier nCF in dataModel.NonConformiteFichiers)
                {
                    if (viewModel.NonConformiteFichiers.Where(el => el.RefNonConformiteFichier == nCF.RefNonConformiteFichier).FirstOrDefault() == null)
                    {
                        DbContext.NonConformiteFichiers.Remove(nCF);
                    }
                }
            }
            //Update related data NonConformiteEtapes
            foreach (NonConformiteEtapeViewModel nCEVM in viewModel.NonConformiteEtapes)
            {
                var nCE = dataModel.NonConformiteEtapes.Where(el => el.RefNonConformiteEtapeType == nCEVM.NonConformiteEtapeType.RefNonConformiteEtapeType).FirstOrDefault();
                //Mark as dirty if applicable
                if (nCE.RefNonConformiteEtapeType != nCEVM.NonConformiteEtapeType?.RefNonConformiteEtapeType
                || nCE.Ordre != nCEVM.Ordre
                || nCE.LibelleFRFR != Utils.Utils.SetEmptyStringToNull(nCEVM.LibelleFRFR)
                || nCE.LibelleENGB != Utils.Utils.SetEmptyStringToNull(nCEVM.LibelleENGB)
                || nCE.Cmt != Utils.Utils.SetEmptyStringToNull(nCEVM.Cmt)
                || nCE.DCreation != nCEVM.DCreation
                || nCE.RefUtilisateurCreation != nCEVM.UtilisateurCreation?.RefUtilisateur
                || nCE.DModif != nCEVM.DModif
                || nCE.RefUtilisateurModif != nCEVM.UtilisateurModif?.RefUtilisateur
                || nCE.DControle != nCEVM.DControle
                || nCE.RefUtilisateurControle != nCEVM.UtilisateurControle?.RefUtilisateur
                || nCE.DValide != nCEVM.DValide
                || nCE.RefUtilisateurValide != nCEVM.UtilisateurControle?.RefUtilisateur
                ) { dirty = true; }
                nCE.RefNonConformiteEtapeType = nCEVM.NonConformiteEtapeType.RefNonConformiteEtapeType;
                nCE.Ordre = nCEVM.Ordre;
                nCE.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(nCEVM.LibelleFRFR);
                nCE.LibelleENGB = Utils.Utils.SetEmptyStringToNull(nCEVM.LibelleENGB);
                nCE.Cmt = Utils.Utils.SetEmptyStringToNull(nCEVM.Cmt);
                nCE.DControle = nCEVM.DControle;
                nCE.RefUtilisateurControle = nCEVM.UtilisateurControle?.RefUtilisateur;
                nCE.DValide = (nCEVM.DValide == null ? null : (nCEVM.DValide != nCE.DValide ? (DateTime?)DateTime.Now : nCE.DValide));
                nCE.RefUtilisateurValide = nCEVM.UtilisateurValide?.RefUtilisateur;
            }
        }
        private void manageEtapeModification(ref NonConformite nC, int refNonConformiteEtapeType)
        {
            var etape = nC.NonConformiteEtapes.FirstOrDefault(el => el.RefNonConformiteEtapeType == refNonConformiteEtapeType);
            etape.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            etape.MarkModification();
        }
        private bool modifiedNonConformiteFamille(NonConformite nC, NonConformiteViewModel nCVM)
        {
            bool r = false;
            if (nC.NonConformiteNonConformiteFamilles == null) { nC.NonConformiteNonConformiteFamilles = new HashSet<NonConformiteNonConformiteFamille>(); }
            if (nC.NonConformiteNonConformiteFamilles.Count != nCVM.NonConformiteNonConformiteFamilles.Count)
            { r = true; }
            else
            {
                foreach (var nCF in nC.NonConformiteNonConformiteFamilles)
                {
                    if (nCVM.NonConformiteNonConformiteFamilles.Where(el => el.NonConformiteFamille.RefNonConformiteFamille == nCF.RefNonConformiteFamille).Count() != 1)
                    {
                        r = true;
                    }
                }
            }
            return r;
        }
        #endregion
    }
}

