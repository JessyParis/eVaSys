/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/05/2020
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
    public class CVQController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public CVQController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/cVQ/{id}
        /// Retrieves the CVQ with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing CVQ</param>
        /// <returns>the CVQ with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            CVQ cVQ = null;
            //Get or create cVQ
            if (id == 0)
            {
                int refProduit = System.Convert.ToInt32(Request.Headers["refProduit"]);
                if (refProduit != 0)
                {
                    cVQ = new CVQ();
                    DbContext.CVQs.Add(cVQ);
                    cVQ.CVQDescriptionCVQs = new HashSet<CVQDescriptionCVQ>();
                    var dCs = DbContext.DescriptionCVQs
                        .Where(w => w.DescriptionCVQProduits.Any(rel => rel.RefProduit == refProduit) && w.Actif == true).ToList();
                    foreach (var dC in dCs)
                    {
                        cVQ.CVQDescriptionCVQs.Add(
                            new CVQDescriptionCVQ()
                            {
                                DescriptionCVQ = dC,
                                LibelleFRFR = dC.LibelleFRFR,
                                LibelleENGB = dC.LibelleENGB,
                                Ordre = dC.Ordre,
                                LimiteBasse = dC.LimiteBasse,
                                LimiteHaute = dC.LimiteHaute
                                
                            });
                    }
                }
            }
            else
            {
                cVQ = DbContext.CVQs
                    .Include(r => r.CVQDescriptionCVQs)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefCVQ == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (cVQ == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            var c = _mapper.Map<CVQ, CVQViewModel>(cVQ);
            //End
            cVQ.currentCulture = CurrentContext.CurrentCulture;
            foreach (var el in cVQ.CVQDescriptionCVQs) { el.currentCulture = CurrentContext.CurrentCulture; }
            return new JsonResult(
                _mapper.Map<CVQ, CVQViewModel>(cVQ),
                JsonSettings);
        }

        /// <summary>
        /// Edit the CVQ with the given {id}
        /// </summary>
        /// <param name="model">The CVQViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]CVQViewModel model)
        {
            string valid = null;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            CVQ cVQ;
            //Retrieve or create the entity to edit
            if (model.RefCVQ > 0)
            {
                cVQ = DbContext.CVQs
                    .Include(r => r.CVQDescriptionCVQs)
                    .Where(q => q.RefCVQ == model.RefCVQ)
                    .FirstOrDefault();
            }
            else
            {
                cVQ = new CVQ();
                DbContext.CVQs.Add(cVQ);
            }
            // handle requests asking for non-existing cVQzes
            if (cVQ == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref cVQ, model);

            //Register session user
            cVQ.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            valid = cVQ.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated CVQ to the client.
                return new JsonResult(
                    _mapper.Map<CVQ, CVQViewModel>(cVQ),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the CVQ with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the cVQ from the Database
            var cVQ = DbContext.CVQs
                .Include(r=>r.CVQDescriptionCVQs)
                .Where(i => i.RefCVQ == id)
                .FirstOrDefault();

            // handle requests asking for non-existing cVQzes
            if (cVQ == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = cVQ.IsDeletable();
            if (del == "")
            {
                DbContext.CVQs.Remove(cVQ);
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
        private void UpdateData(ref CVQ dataModel, CVQViewModel viewModel)
        {
            dataModel.RefFicheControle = viewModel.RefFicheControle;
            dataModel.RefControleur = viewModel.Controleur?.RefContactAdresse;
            dataModel.DCVQ = viewModel.DCVQ;
            dataModel.Etiquette = viewModel.Etiquette;
            dataModel.Cmt = viewModel.Cmt;
            //Dirty marker
            bool dirty = false;
            //Remove related data CVQDescriptionCVQ
            if (dataModel.CVQDescriptionCVQs != null)
            {
                foreach (CVQDescriptionCVQ fCDR in dataModel.CVQDescriptionCVQs)
                {
                    if (viewModel.CVQDescriptionCVQs == null)
                    {
                        DbContext.CVQDescriptionCVQs.Remove(fCDR);
                        dirty = true;
                    }
                    else if (viewModel.CVQDescriptionCVQs.Where(el => el.RefCVQDescriptionCVQ == fCDR.RefCVQDescriptionCVQ).FirstOrDefault() == null)
                    {
                        DbContext.CVQDescriptionCVQs.Remove(fCDR);
                        dirty = true;
                    }
                }
            }
            //Add or update related data CVQDescriptionCVQs
            foreach (CVQDescriptionCVQViewModel fCDRVM in viewModel.CVQDescriptionCVQs)
            {
                CVQDescriptionCVQ fCDR = null;
                if (dataModel.CVQDescriptionCVQs != null && fCDRVM.RefCVQDescriptionCVQ != 0)
                {
                    fCDR = dataModel.CVQDescriptionCVQs.Where(el => el.RefCVQDescriptionCVQ == fCDRVM.RefCVQDescriptionCVQ).FirstOrDefault();
                }
                if (fCDR == null)
                {
                    fCDR = new CVQDescriptionCVQ();
                    DbContext.CVQDescriptionCVQs.Add(fCDR);
                    if (dataModel.CVQDescriptionCVQs == null) { dataModel.CVQDescriptionCVQs = new HashSet<CVQDescriptionCVQ>(); }
                    dataModel.CVQDescriptionCVQs.Add(fCDR);
                }
                //Mark as dirty if applicable
                if (fCDR.Ordre != fCDRVM.Ordre
                || fCDR.LibelleFRFR != Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleFRFR)
                || fCDR.LibelleENGB != Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleENGB)
                || fCDR.Nb != fCDRVM.Nb) { dirty = true; }
                fCDR.Ordre = fCDRVM.Ordre;
                fCDR.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleFRFR);
                fCDR.LibelleENGB = Utils.Utils.SetEmptyStringToNull(fCDRVM.LibelleENGB);
                fCDR.Nb = fCDRVM.Nb;
                fCDR.LimiteBasse = fCDRVM.LimiteBasse;
                fCDR.LimiteHaute = fCDRVM.LimiteHaute;
            }
            //Mark for modification if applicable
            if (dirty) { dataModel.DModif = DateTime.Now; }
        }
        #endregion
    }
}

