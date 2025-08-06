/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 13/01/2020
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
using System.Linq;
using eVaSys.APIUtils;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class SurcoutCarburantController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public SurcoutCarburantController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/surcoutcarburant/{id}
        /// Retrieves the SurcoutCarburant with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing SurcoutCarburant</param>
        /// <returns>the SurcoutCarburant with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            SurcoutCarburant surcoutCarburant = null;
            //Get or create surcoutCarburant
            if (id == 0)
            {
                surcoutCarburant = new SurcoutCarburant();
                DbContext.SurcoutCarburants.Add(surcoutCarburant);
            }
            else
            {
                surcoutCarburant = DbContext.SurcoutCarburants
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefSurcoutCarburant == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (surcoutCarburant == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            surcoutCarburant.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<SurcoutCarburant, SurcoutCarburantViewModel>(surcoutCarburant),
                JsonSettings);
        }

        /// <summary>
        /// Edit the SurcoutCarburant with the given {id}
        /// </summary>
        /// <param name="model">The SurcoutCarburantViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] SurcoutCarburantViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            SurcoutCarburant surcoutCarburant;
            //Retrieve or create the entity to edit
            if (model.RefSurcoutCarburant > 0)
            {
                surcoutCarburant = DbContext.SurcoutCarburants
                        .Where(q => q.RefSurcoutCarburant == model.RefSurcoutCarburant)
                        .FirstOrDefault();
            }
            else
            {
                surcoutCarburant = new SurcoutCarburant();
                DbContext.SurcoutCarburants.Add(surcoutCarburant);
            }
            // handle requests asking for non-existing surcoutCarburantzes
            if (surcoutCarburant == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref surcoutCarburant, model);

            //Register session user
            surcoutCarburant.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = surcoutCarburant.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated SurcoutCarburant to the client.
                return new JsonResult(
                    _mapper.Map<SurcoutCarburant, SurcoutCarburantViewModel>(surcoutCarburant),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the SurcoutCarburant with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the surcoutCarburant from the Database
            var surcoutCarburant = DbContext.SurcoutCarburants
                .Where(i => i.RefSurcoutCarburant == id)
                .FirstOrDefault();

            // handle requests asking for non-existing surcoutCarburantzes
            if (surcoutCarburant == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = surcoutCarburant.IsDeletable();
            if (del == "")
            {
                DbContext.SurcoutCarburants.Remove(surcoutCarburant);
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
        /// GET: evapi/getsurcoutcarburantratio
        /// Retrieves the SurcoutCarburant according to parameters
        /// </summary>
        /// <returns>the SurcoutCarburant ratio</returns>
        [HttpGet("getsurcoutcarburantratio")]
        public IActionResult GetSurcoutCarburantRatio()
        {
            //Get headers
            string refTransporteur = Request.Headers["refTransporteur"].ToString();
            string refPays = Request.Headers["refPays"].ToString();
            string d = Request.Headers["d"].ToString();
            int refT = 0;
            int refP = 0;
            DateTime da = DateTime.MinValue;
            //Get SurcoutCarburants
            if (int.TryParse(refTransporteur, out refT) && int.TryParse(refPays, out refP) && DateTime.TryParse(d, out da))
            {
                var sC = DbContext.SurcoutCarburants.Where(el => el.RefTransporteur == refT && el.RefPays == refP && el.Annee == da.Year && el.Mois == da.Month).FirstOrDefault();
                if (sC == null) { return Ok(0); }
                else { return Ok(sC.Ratio); }
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
        /// <summary>
        /// GET: evapi/getsurcoutcarburants
        /// Retrieves the SurcoutCarburants according to parameters
        /// </summary>
        /// <returns>the correponding SurcoutCarburants</returns>
        [HttpGet("getsurcoutcarburants")]
        public IActionResult GetSurcoutCarburants()
        {
            //Get headers
            string refTransporteur = Request.Headers["refTransporteur"].ToString();
            string refPays = Request.Headers["refPays"].ToString();
            string year = Request.Headers["year"].ToString();
            int refT = 0;
            int refP = 0;
            int y = 0;
            List<SurcoutCarburant> sCs = new();
            //Get SurcoutCarburants
            if (int.TryParse(refTransporteur, out refT) && int.TryParse(refPays, out refP) && int.TryParse(year, out y))
            {
                sCs = DbContext.SurcoutCarburants
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(el => el.RefTransporteur == refT && el.RefPays == refP && el.Annee == y).ToList();
                //Add missing ClientApplicationApplication
                for (int i = 1; i <= 12; i++)
                {
                    if (sCs.Where(e => e.Mois == i).FirstOrDefault() == null)
                    {
                        SurcoutCarburant sC = new()
                        {
                            RefTransporteur = refT,
                            RefPays = refP,
                            Annee = y,
                            Mois = i,
                            CreationText = CurrentContext.CulturedRessources.GetTextRessource(9),
                            ModificationText = ""
                        };
                        DbContext.SurcoutCarburants.Add(sC);
                        sCs.Add(sC);
                    }
                }
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
            // handle requests asking for non-existing object
            if (sCs.Count == 0)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Return Json
            return new JsonResult(
                _mapper.Map<SurcoutCarburant[], SurcoutCarburantViewModel[]>(sCs.ToArray()),
                JsonSettings);
        }
        /// <summary>
        /// Check if Create/Modify/Delete SurcoutCarburants could have side effects on CommandeFournisseur
        /// </summary>
        /// <param name="model">The SurcoutCarburantFromViewModels containing the data to update</param>
        [HttpPost("checkpostsurcoutcarburants")]
        public IActionResult CheckPostSurcoutCarburants([FromBody] SurcoutCarburantViewModel[] model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            //Init
            int c = 0;
            //For each SurcoutCarburant
            foreach (SurcoutCarburantViewModel sCVM in model)
            {
                SurcoutCarburant sC = null;
                //Get the corresponding SurcoutCarburant
                if (sCVM.RefSurcoutCarburant > 0)
                {
                    sC = DbContext.SurcoutCarburants.Where(el => el.RefSurcoutCarburant == sCVM.RefSurcoutCarburant).FirstOrDefault();
                }
                else
                {
                    sC = new SurcoutCarburant();
                    DbContext.SurcoutCarburants.Add(sC);
                }
                if (sC != null)
                {
                    if ((sC.Ratio ?? 0) != (sCVM.Ratio ?? 0))
                    {
                        c += DbContext.CommandeFournisseurs
                            .Where(el => el.RefTransporteur == sCVM.Transporteur.RefEntite && el.AdresseClient.RefPays == sCVM.Pays.RefPays
                                && el.ExportSAGE == true
                                && ((el.DDechargement != null || el.DDechargementPrevue != null)
                                    && ((DateTime)(el.DDechargement ?? el.DDechargementPrevue)).Month == sCVM.Mois
                                    && ((DateTime)(el.DDechargement ?? el.DDechargementPrevue)).Year == sCVM.Annee
                                )
                            ).Count();
                    }
                }
            }
            //End
            if (c == 0)
            {
                //Return the updated SurcoutCarburants to the client
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }
        /// <summary>
        /// Create/Modify/Delete SurcoutCarburants
        /// </summary>
        /// <param name="model">The SurcoutCarburantFromViewModels containing the data to update</param>
        [HttpPost("postsurcoutcarburants")]
        public IActionResult PostSurcoutCarburants([FromBody] SurcoutCarburantViewModel[] model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            //Init
            string errs = "";
            //For each SurcoutCarburant
            foreach (SurcoutCarburantViewModel sCVM in model)
            {
                var payss = new List<int?>();
                //Get or create every concerned SurcoutCarburants if AllPays
                if (sCVM.AllPays)
                {
                    //Get all concerned Pays
                    if (sCVM.Ratio != 0)
                    {
                        payss = DbContext.CommandeFournisseurs
                        .Where(el => el.RefTransporteur == sCVM.Transporteur.RefEntite && el.ExportSAGE == false
                            && ((el.DDechargement != null || el.DDechargementPrevue != null)
                                && ((DateTime)(el.DDechargement ?? el.DDechargementPrevue)).Month == sCVM.Mois
                                && ((DateTime)(el.DDechargement ?? el.DDechargementPrevue)).Year == sCVM.Annee
                            )
                        )
                        .Select(s => s.AdresseClient.RefPays)
                        .Distinct()
                        .ToList();
                    }
                    else
                    {
                        payss = DbContext.SurcoutCarburants
                        .Where(el => el.RefTransporteur == sCVM.Transporteur.RefEntite
                                && el.Mois == sCVM.Mois
                                && el.Annee == sCVM.Annee
                        )
                        .Select(s => (int?)s.Pays.RefPays)
                        .Distinct()
                        .ToList();
                    }
                }
                else { payss = new List<int?> { sCVM.Pays.RefPays }; }
                //Process
                string validError = "";
                //For each pays
                foreach (var refPays in payss)
                {
                    SurcoutCarburant sC = null;
                    sC = DbContext.SurcoutCarburants.Where(el => el.RefTransporteur == sCVM.Transporteur.RefEntite
                        && el.Pays.RefPays == refPays
                        && el.Mois == sCVM.Mois && el.Annee == sCVM.Annee
                        ).FirstOrDefault();
                    //Remove, update or create
                    if (sCVM.Ratio == 0)
                    {
                        if (sC != null)
                        {
                            //Delete SurcoutCarburant if applicable
                            DbContext.Remove(sC);
                        }
                    }
                    else
                    {
                        sCVM.Pays = _mapper.Map<Pays, PaysViewModel>(DbContext.Payss.Find(refPays));
                        if (sC == null)
                        {
                            sC = new SurcoutCarburant();
                            DbContext.SurcoutCarburants.Add(sC);
                        }
                        UpdateData(ref sC, sCVM);
                        //Register session user
                        sC.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                        validError = sC.IsValid();
                        if (validError != "")
                        {
                            errs += " " + validError;
                        }
                    }
                }
            }
            //End
            if (errs == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Return the updated SurcoutCarburants to the client
                return Ok();
            }
            else
            {
                return Conflict(new ConflictError(errs));
            }
        }
        #endregion
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref SurcoutCarburant dataModel, SurcoutCarburantViewModel viewModel)
        {
            //Modify CommandeFournisseur if applicable
            if ((viewModel.Ratio ?? 0) != (dataModel.Ratio ?? 0))
            {
                //Update CommandeFournisseur if applicable
                var cmdFs = DbContext.CommandeFournisseurs
                    .Where(el => el.RefTransporteur == viewModel.Transporteur.RefEntite && el.AdresseClient.RefPays == viewModel.Pays.RefPays
                        && el.ExportSAGE == false
                        && ((el.DDechargement != null || el.DDechargementPrevue != null)
                            && ((DateTime)(el.DDechargement ?? el.DDechargementPrevue)).Month == viewModel.Mois
                            && ((DateTime)(el.DDechargement ?? el.DDechargementPrevue)).Year == viewModel.Annee
                        )
                    ).ToList();
                foreach (var cmdF in cmdFs)
                {
                    cmdF.SurcoutCarburantHT = cmdF.PrixTransportHT * ((decimal)(viewModel.Ratio ?? 0) / 100);
                    cmdF.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                }
            }
            //Update data
            dataModel.RefTransporteur = viewModel.Transporteur.RefEntite;
            dataModel.RefPays = viewModel.Pays.RefPays;
            dataModel.Annee = (int)viewModel.Annee;
            dataModel.Mois = (int)viewModel.Mois;
            dataModel.Ratio = viewModel.Ratio;
        }
        #endregion
    }
}

