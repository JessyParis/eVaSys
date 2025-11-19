/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class FicheControleController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public FicheControleController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/fichecontrole/{id}
        /// Retrieves the FicheControle with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing FicheControle</param>
        /// <returns>the FicheControle with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            FicheControle ficheControle = null;
            //Get or create ficheControle
            if (id == 0)
            {
                int refCommandeFournisseur = System.Convert.ToInt32(Request.Headers["refCommandeFournisseur"]);
                if (refCommandeFournisseur != 0)
                {
                    ficheControle = DbContext.FicheControles
                        .Include(r => r.FicheControleDescriptionReceptions)
                        .Include(r => r.Controles)
                        .Include(r => r.CVQs)
                        .Include(r => r.UtilisateurCreation)
                        .Include(r => r.UtilisateurModif)
                        .AsSplitQuery()
                        .Where(i => i.RefCommandeFournisseur == refCommandeFournisseur).FirstOrDefault();
                    if (ficheControle == null)
                    {
                        ficheControle = new FicheControle();
                        DbContext.FicheControles.Add(ficheControle);
                        ficheControle.RefCommandeFournisseur = refCommandeFournisseur;
                        ficheControle.FicheControleDescriptionReceptions = new HashSet<FicheControleDescriptionReception>();
                        var dRs = DbContext.DescriptionReceptions.ToList();
                        foreach (var dR in dRs)
                        {
                            ficheControle.FicheControleDescriptionReceptions.Add(
                                new FicheControleDescriptionReception()
                                {
                                    DescriptionReception = dR,
                                    LibelleFRFR = dR.LibelleFRFR,
                                    LibelleENGB = dR.LibelleENGB,
                                    Ordre = dR.Ordre,
                                    OuiFRFR = dR.OuiFRFR,
                                    OuiENGB = dR.OuiENGB,
                                    NonFRFR = dR.NonFRFR,
                                    NonENGB = dR.NonENGB,
                                    Positif = dR.ReponseParDefaut,
                                });
                        }
                    }
                }
            }
            else
            {
                ficheControle = DbContext.FicheControles
                    .Include(r => r.FicheControleDescriptionReceptions)
                    .Include(r => r.Controles)
                    .Include(r => r.CVQs)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .AsSplitQuery()
                    .Where(i => i.RefFicheControle == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (ficheControle == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            ficheControle.currentCulture = CurrentContext.CurrentCulture;
            foreach (var el in ficheControle.FicheControleDescriptionReceptions) { el.currentCulture = CurrentContext.CurrentCulture; }
            return new JsonResult(
                _mapper.Map<FicheControle, FicheControleViewModel>(ficheControle),
                JsonSettings);
        }

        /// <summary>
        /// Edit the FicheControle with the given {id}
        /// </summary>
        /// <param name="model">The FicheControleViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] FicheControleViewModel model)
        {
            string valid = null;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            System.Threading.Thread.Sleep(1000);

            FicheControle ficheControle;
            //Retrieve or create the entity to edit
            if (model.RefFicheControle > 0)
            {
                ficheControle = DbContext.FicheControles
                    .Include(r => r.FicheControleDescriptionReceptions)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(q => q.RefFicheControle == model.RefFicheControle)
                    .FirstOrDefault();
            }
            else
            {
                ficheControle = new FicheControle();
                DbContext.FicheControles.Add(ficheControle);
            }
            // handle requests asking for non-existing ficheControlezes
            if (ficheControle == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref ficheControle, model);

            //Register session user
            ficheControle.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            valid = ficheControle.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                DbContext.Entry(ficheControle).Reload();
                DbContext.Entry(ficheControle).Collection(r => r.CVQs).Load();
                DbContext.Entry(ficheControle).Collection(r => r.Controles).Load();
                // return the updated FicheControle to the client.
                return new JsonResult(
                    _mapper.Map<FicheControle, FicheControleViewModel>(ficheControle),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the FicheControle with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the ficheControle from the Database
            var ficheControle = DbContext.FicheControles
                .Include(r => r.FicheControleDescriptionReceptions)
                .Include(r => r.Controles).ThenInclude(r => r.ControleDescriptionControles)
                .Include(r => r.CVQs).ThenInclude(r => r.CVQDescriptionCVQs)
                .Where(i => i.RefFicheControle == id)
                .FirstOrDefault();
            // handle requests asking for non-existing ficheControlezes
            if (ficheControle == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = ficheControle.IsDeletable();
            if (del == "")
            {
                DbContext.FicheControles.Remove(ficheControle);
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
        /// GET: api/fichecontrole/getrelatedreffichecontrole
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Get de related RefFicheControle</returns>
        [HttpGet("getrelatedreffichecontrole")]
        public IActionResult GetRelatedRefFichecontrole()
        {
            string refCommandeFournisseur = Request.Headers["refCommandeFournisseur"].ToString();
            int r = 0;
            int refC = 0;
            //Check mandatory parameters
            if (int.TryParse(refCommandeFournisseur, out refC))
            {
                var fC = DbContext.FicheControles.Where(i => i.RefCommandeFournisseur == refC).FirstOrDefault();
                if (fC != null) { r = fC.RefFicheControle; }
                //Return Json
                return new JsonResult(r, JsonSettings);
            }
            else
            {
                return new JsonResult(0, JsonSettings);
            }
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref FicheControle dataModel, FicheControleViewModel viewModel)
        {
            dataModel.RefCommandeFournisseur = viewModel.CommandeFournisseur?.RefCommandeFournisseur;
            dataModel.NumeroLotUsine = viewModel.NumeroLotUsine;
            dataModel.Cmt = viewModel.Cmt;
            dataModel.RefControleur = viewModel.Controleur?.RefContactAdresse;
            dataModel.Reserve = viewModel.Reserve ?? false;
            //Dirty marker
            bool dirty = false;
            //Remove related data FicheControleDescriptionReception
            if (dataModel.FicheControleDescriptionReceptions != null)
            {
                foreach (FicheControleDescriptionReception fCDR in dataModel.FicheControleDescriptionReceptions)
                {
                    if (viewModel.FicheControleDescriptionReceptions == null)
                    {
                        DbContext.FicheControleDescriptionReceptions.Remove(fCDR);
                        dirty = true;
                    }
                    else if (viewModel.FicheControleDescriptionReceptions.Where(el => el.RefFicheControleDescriptionReception == fCDR.RefFicheControleDescriptionReception).FirstOrDefault() == null)
                    {
                        DbContext.FicheControleDescriptionReceptions.Remove(fCDR);
                        dirty = true;
                    }
                }
            }
            //Add or update related data FicheControleDescriptionReceptions
            foreach (FicheControleDescriptionReceptionViewModel fCDRVM in viewModel.FicheControleDescriptionReceptions)
            {
                FicheControleDescriptionReception fCDR = null;
                if (dataModel.FicheControleDescriptionReceptions != null && fCDRVM.RefFicheControleDescriptionReception != 0)
                {
                    fCDR = dataModel.FicheControleDescriptionReceptions.Where(el => el.RefFicheControleDescriptionReception == fCDRVM.RefFicheControleDescriptionReception).FirstOrDefault();
                }
                if (fCDR == null)
                {
                    fCDR = new FicheControleDescriptionReception();
                    DbContext.FicheControleDescriptionReceptions.Add(fCDR);
                    if (dataModel.FicheControleDescriptionReceptions == null) { dataModel.FicheControleDescriptionReceptions = new HashSet<FicheControleDescriptionReception>(); }
                    dataModel.FicheControleDescriptionReceptions.Add(fCDR);
                }
                //Mark as dirty if applicable
                if (fCDR.RefDescriptionReception != fCDRVM.DescriptionReception.RefDescriptionReception
                || fCDR.Cmt != fCDRVM.Cmt
                || fCDR.Positif != (fCDRVM.Positif ?? false)
                || fCDR.Ordre != fCDRVM.Ordre
                || fCDR.LibelleFRFR != Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleFRFR)
                || fCDR.OuiFRFR != Utils.Utils.SetEmptyStringToNull(fCDRVM.OuiFRFR)
                || fCDR.NonFRFR != Utils.Utils.SetEmptyStringToNull(fCDRVM.NonFRFR)
                || fCDR.LibelleENGB != Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleENGB)
                || fCDR.OuiENGB != Utils.Utils.SetEmptyStringToNull(fCDRVM.OuiENGB)
                || fCDR.NonENGB != Utils.Utils.SetEmptyStringToNull(fCDRVM.NonENGB)) { dirty = true; }
                fCDR.RefDescriptionReception = fCDRVM.DescriptionReception.RefDescriptionReception;
                fCDR.Cmt = Utils.Utils.SetEmptyStringToNull(fCDRVM.Cmt);
                fCDR.Positif = fCDRVM.Positif ?? false;
                fCDR.Ordre = fCDRVM.Ordre;
                fCDR.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleFRFR);
                fCDR.OuiFRFR = Utils.Utils.SetEmptyStringToNull(fCDRVM.OuiFRFR);
                fCDR.NonFRFR = Utils.Utils.SetEmptyStringToNull(fCDRVM.NonFRFR);
                fCDR.LibelleENGB = Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleENGB);
                fCDR.OuiENGB = Utils.Utils.SetEmptyStringToNull(fCDRVM.OuiENGB);
                fCDR.NonENGB = Utils.Utils.SetEmptyStringToNull(fCDRVM.NonENGB);
            }
            //Mark for modification if applicable
            if (dirty) { dataModel.DModif = DateTime.Now; }
        }
        #endregion
    }
}

