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
    public class ControleController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ControleController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/controle/{id}
        /// Retrieves the Controle with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Controle</param>
        /// <returns>the Controle with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Controle controle = null;
            //Get or create controle
            if (id == 0)
            {
                int refProduit = System.Convert.ToInt32(Request.Headers["refProduit"]);
                if (refProduit != 0)
                {
                    controle = new Controle();
                    DbContext.Controles.Add(controle);
                    controle.ControleDescriptionControles = new HashSet<ControleDescriptionControle>();
                    var dCs = DbContext.DescriptionControles
                        .Where(w => w.DescriptionControleProduits.Any(rel => rel.RefProduit == refProduit) && w.Actif == true).ToList();
                    foreach (var dC in dCs)
                    {
                        controle.ControleDescriptionControles.Add(
                            new ControleDescriptionControle()
                            {
                                DescriptionControle = dC,
                                LibelleFRFR = dC.LibelleFRFR,
                                LibelleENGB = dC.LibelleENGB,
                                Ordre = dC.Ordre,
                                CalculLimiteConformite=dC.CalculLimiteConformite
                            });
                    }
                }
            }
            else
            {
                controle = DbContext.Controles
                    .Include(r => r.ControleDescriptionControles)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefControle == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (controle == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            controle.currentCulture = CurrentContext.CurrentCulture;
            foreach (var el in controle.ControleDescriptionControles) { el.currentCulture = CurrentContext.CurrentCulture; }
            return new JsonResult(
                _mapper.Map<Controle, ControleViewModel>(controle),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Controle with the given {id}
        /// </summary>
        /// <param name="model">The ControleViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]ControleViewModel model)
        {
            string valid = null;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Controle controle;
            //Retrieve or create the entity to edit
            if (model.RefControle > 0)
            {
                controle = DbContext.Controles
                    .Include(r => r.ControleDescriptionControles)
                    .Where(q => q.RefControle == model.RefControle)
                    .FirstOrDefault();
            }
            else
            {
                controle = new Controle();
                DbContext.Controles.Add(controle);
            }
            // handle requests asking for non-existing controlezes
            if (controle == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref controle, model);

            //Register session user
            controle.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            valid = controle.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Controle to the client.
                return new JsonResult(
                    _mapper.Map<Controle, ControleViewModel>(controle),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Controle with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the controle from the Database
            var controle = DbContext.Controles
                .Include(r=>r.ControleDescriptionControles)
                .Where(i => i.RefControle == id)
                .FirstOrDefault();

            // handle requests asking for non-existing controlezes
            if (controle == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = controle.IsDeletable();
            if (del == "")
            {
                DbContext.Controles.Remove(controle);
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
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref Controle dataModel, ControleViewModel viewModel)
        {
            dataModel.RefFicheControle = viewModel.RefFicheControle;
            dataModel.RefControleType = viewModel.ControleType?.RefControleType;
            dataModel.RefControleur = viewModel.Controleur?.RefContactAdresse;
            dataModel.DControle = viewModel.DControle;
            dataModel.Poids = viewModel.Poids;
            dataModel.RefControleur = viewModel.Controleur?.RefContactAdresse;
            dataModel.Etiquette = Utils.Utils.SetEmptyStringToNull(viewModel.Etiquette);
            dataModel.Cmt = Utils.Utils.SetEmptyStringToNull(viewModel.Cmt);
            //Dirty marker
            bool dirty = false;
            //Remove related data ControleDescriptionControle
            if (dataModel.ControleDescriptionControles != null)
            {
                foreach (ControleDescriptionControle fCDR in dataModel.ControleDescriptionControles)
                {
                    if (viewModel.ControleDescriptionControles == null)
                    {
                        DbContext.ControleDescriptionControles.Remove(fCDR);
                        dirty = true;
                    }
                    else if (viewModel.ControleDescriptionControles.Where(el => el.RefControleDescriptionControle == fCDR.RefControleDescriptionControle).FirstOrDefault() == null)
                    {
                        DbContext.ControleDescriptionControles.Remove(fCDR);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ControleDescriptionControles
            foreach (ControleDescriptionControleViewModel fCDRVM in viewModel.ControleDescriptionControles)
            {
                ControleDescriptionControle fCDR = null;
                if (dataModel.ControleDescriptionControles != null && fCDRVM.RefControleDescriptionControle != 0)
                {
                    fCDR = dataModel.ControleDescriptionControles.Where(el => el.RefControleDescriptionControle == fCDRVM.RefControleDescriptionControle).FirstOrDefault();
                }
                if (fCDR == null)
                {
                    fCDR = new ControleDescriptionControle();
                    DbContext.ControleDescriptionControles.Add(fCDR);
                    if (dataModel.ControleDescriptionControles == null) { dataModel.ControleDescriptionControles = new HashSet<ControleDescriptionControle>(); }
                    dataModel.ControleDescriptionControles.Add(fCDR);
                }
                //Mark as dirty if applicable
                if (fCDR.Cmt != Utils.Utils.SetEmptyStringToNull(fCDRVM.Cmt)
                || fCDR.CalculLimiteConformite != fCDRVM.CalculLimiteConformite
                || fCDR.Ordre != fCDRVM.Ordre
                || fCDR.LibelleFRFR != Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleFRFR)
                || fCDR.LibelleENGB != Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleENGB)
                || fCDR.Poids != fCDRVM.Poids) { dirty = true; }
                fCDR.Cmt = Utils.Utils.SetEmptyStringToNull(fCDRVM.Cmt);
                fCDR.CalculLimiteConformite = fCDRVM.CalculLimiteConformite ?? false;
                fCDR.Ordre = fCDRVM.Ordre;
                fCDR.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleFRFR);
                fCDR.LibelleENGB = Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleENGB);
                fCDR.Poids = fCDRVM.Poids;
            }
            //Mark for modification if applicable
            if (dirty) { dataModel.DModif = DateTime.Now; }
        }
        #endregion
    }
}

