/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 21/11/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class EntiteTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public EntiteTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/entiteType/{id}
        /// Retrieves the EntiteType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing EntiteType</param>
        /// <returns>The EntiteType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            EntiteType entiteType = null;
            if (id == 0)
            {
                entiteType = new EntiteType();
                DbContext.EntiteTypes.Add(entiteType);
            }
            else
            {
                entiteType = DbContext.EntiteTypes
                        .Include(r => r.UtilisateurCreation)
                        .Include(r => r.UtilisateurModif)
                        .Where(i => i.RefEntiteType == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (entiteType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<EntiteType, EntiteTypeViewModel>(entiteType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the EntiteType with the given {id}
        /// </summary>
        /// <param name="model">The EntiteTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] EntiteTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            EntiteType entiteType;
            //Retrieve or create the EntiteType to edit
            if (model.RefEntiteType > 0)
            {
                entiteType = DbContext.EntiteTypes
                        .Where(q => q.RefEntiteType == model.RefEntiteType)
                        .FirstOrDefault();
            }
            else
            {
                entiteType = new EntiteType();
                DbContext.EntiteTypes.Add(entiteType);
            }
            // handle requests asking for non-existing EntiteTypes
            if (entiteType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            UpdateData(entiteType, model);
            //Register session user
            entiteType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = entiteType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated EntiteType to the client.
                return new JsonResult(
                    _mapper.Map<EntiteType, EntiteTypeViewModel>(entiteType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the EntiteType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the entiteType from the Database
            var entiteType = DbContext.EntiteTypes.Where(i => i.RefEntiteType == id).FirstOrDefault();
            // handle requests asking for non-existing entiteTypezes
            if (entiteType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entiteType from the DbContext.
            DbContext.EntiteTypes.Remove(entiteType);
            // persist the changes into the Database.
            DbContext.SaveChanges();
            // return an HTTP Status 200 (OK).
            return Ok();
        }
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/entiteType/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.EntiteType> req = DbContext.EntiteTypes;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<EntiteType[], EntiteTypeViewModel[]>(all),
                JsonSettings);
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(EntiteType dataModel, EntiteTypeViewModel viewModel)
        {
            dataModel.LibelleENGB = viewModel.LibelleENGB;
            dataModel.LibelleFRFR = viewModel.LibelleFRFR;
            dataModel.BLUnique = viewModel.BLUnique ?? false;
            dataModel.CodeEE = viewModel.CodeEE;
            dataModel.RefEcoOrganisme = viewModel.RefEcoOrganisme;
            dataModel.CodeValorisation = viewModel.CodeValorisation;
            dataModel.AssujettiTVA = viewModel.AssujettiTVA;
            dataModel.CodeTVA = viewModel.CodeTVA;
            dataModel.SAGECodeComptable = viewModel.SAGECodeComptable;
            dataModel.SAGECompteTiers = viewModel.SAGECompteTiers;
            dataModel.RefSAGEConditionLivraison = viewModel.RefSAGEConditionLivraison;
            dataModel.RefSAGEPeriodicite = viewModel.RefSAGEPeriodicite;
            dataModel.RefSAGEModeReglement = viewModel.RefSAGEModeReglement;
            dataModel.RefSAGECategorieAchat = viewModel.RefSAGECategorieAchat;
            dataModel.RefSAGECategorieVente = viewModel.RefSAGECategorieVente;
            dataModel.ActionnaireProprietaire = viewModel.ActionnaireProprietaire;
            dataModel.Exploitant = viewModel.Exploitant;
            dataModel.Demarrage = viewModel.Demarrage;
            dataModel.Capacite = viewModel.Capacite;
            dataModel.TonnageGlobal = viewModel.TonnageGlobal;
            dataModel.RefEquipementier = viewModel.RefEquipementier;
            dataModel.PopulationContratN = viewModel.PopulationContratN;
            dataModel.LiaisonEntiteProduit = viewModel.LiaisonEntiteProduit;
            dataModel.LiaisonEntiteProduitInterdit = viewModel.LiaisonEntiteProduitInterdit;
            dataModel.LiaisonEntiteStandard = viewModel.LiaisonEntiteStandard;
            dataModel.LiaisonEntiteDR = viewModel.LiaisonEntiteDR;
            dataModel.Horaires = viewModel.Horaires;
            dataModel.ContratOptimisationTransport = viewModel.ContratOptimisationTransport;
            dataModel.VisibiliteAffretementCommun = viewModel.VisibiliteAffretementCommun;
            dataModel.ContratCollectivite = viewModel.ContratCollectivite;
            dataModel.RefRepreneur = viewModel.RefRepreneur;
            dataModel.RefRepriseType = viewModel.RefRepriseType;
            dataModel.ExportSAGE = viewModel.ExportSAGE;
            dataModel.UtilisateurCollectivite = viewModel.UtilisateurCollectivite;
            dataModel.Process = viewModel.Process;
            dataModel.AutoControle = viewModel.AutoControle;
            dataModel.SousContrat = viewModel.SousContrat;
            dataModel.LiaisonEntiteCamionType = viewModel.LiaisonEntiteCamionType;
            dataModel.IncitationQualite = viewModel.IncitationQualite;
            dataModel.SurcoutCarburant = viewModel.SurcoutCarburant;
            dataModel.RefFournisseurTO = viewModel.RefFournisseurTO;
            dataModel.DimensionBalle = viewModel.DimensionBalle;
            dataModel.IdNational = viewModel.IdNational;
        }
        #endregion
    }
}
