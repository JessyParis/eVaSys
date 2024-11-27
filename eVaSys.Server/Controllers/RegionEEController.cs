/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/12/2019
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
    public class RegionEEController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public RegionEEController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/regionee/{id}
        /// Retrieves the RegionEE with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing RegionEE</param>
        /// <returns>the RegionEE with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            RegionEE regionEE = null;
            //Get or create regionEE
            if (id == 0)
            {
                regionEE = new RegionEE();
                DbContext.RegionEEs.Add(regionEE);
            }
            else
            {
                regionEE = DbContext.RegionEEs
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Include(r => r.RegionEEDpts)
                    .Where(i => i.RefRegionEE == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (regionEE == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            regionEE.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<RegionEE, RegionEEViewModel>(regionEE),
                JsonSettings);
        }

        /// <summary>
        /// Modify RegionEE
        /// </summary>
        /// <param name="model">The RegionEEViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]RegionEEViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            RegionEE regionEE;
            //Retrieve or create the entity to edit
            if (model.RefRegionEE > 0)
            {
                regionEE = DbContext.RegionEEs
                        .Where(q => q.RefRegionEE == model.RefRegionEE)
                        .Include(r => r.RegionEEDpts)
                        .FirstOrDefault();
            }
            else
            {
                regionEE = new RegionEE();
                DbContext.RegionEEs.Add(regionEE);
            }

            // handle requests asking for non-existing regionEEzes
            if (regionEE == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref regionEE, model);

            //Register session user
            regionEE.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = regionEE.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Return the updated RegionEE to the client, or return the next Mixte if applicable
                return new JsonResult(
                    _mapper.Map<RegionEE, RegionEEViewModel>(regionEE),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the RegionEE with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the regionEE from the Database
            var regionEE = DbContext.RegionEEs.Where(i => i.RefRegionEE == id)
                .Include(r => r.RegionEEDpts)
                .FirstOrDefault();

            // handle requests asking for non-existing regionEEzes
            if (regionEE == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = regionEE.IsDeletable();
            if (del == "")
            {
                DbContext.RegionEEs.Remove(regionEE);
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
        private void UpdateData(ref RegionEE dataModel, RegionEEViewModel viewModel)
        {
            dataModel.Libelle = Utils.Utils.SetEmptyStringToNull(viewModel.Libelle);
            if (dataModel.RegionEEDpts == null) { dataModel.RegionEEDpts = new HashSet<RegionEEDpt>(); }
            //Dirty marker
            bool dirty = false;
            //Remove related data RegionEEDpt
            if (dataModel.RegionEEDpts != null)
            {
                foreach (RegionEEDpt rEED in dataModel.RegionEEDpts)
                {
                    if (viewModel.RegionEEDpts == null)
                    {
                        DbContext.RegionEEDpts.Remove(rEED);
                        dirty = true;
                    }
                    else if (viewModel.RegionEEDpts.Where(el => el.RefRegionEEDpt == rEED.RefRegionEEDpt).FirstOrDefault() == null)
                    {
                        DbContext.RegionEEDpts.Remove(rEED);
                        dirty = true;
                    }
                }
            }
            //Add or update related data RegionEEDpts
            foreach (RegionEEDptViewModel rEEDVM in viewModel.RegionEEDpts)
            {
                RegionEEDpt rEED = null;
                if (dataModel.RegionEEDpts != null && rEEDVM.RefRegionEEDpt != 0)
                {
                    rEED = dataModel.RegionEEDpts.Where(el => el.RefRegionEEDpt == rEEDVM.RefRegionEEDpt).FirstOrDefault();
                }
                if (rEED == null)
                {
                    rEED = new RegionEEDpt();
                    DbContext.RegionEEDpts.Add(rEED);
                    if (dataModel.RegionEEDpts == null) { dataModel.RegionEEDpts = new HashSet<RegionEEDpt>(); }
                    dataModel.RegionEEDpts.Add(rEED);
                }
                //Mark as dirty if applicable
                if (rEED.RefDpt != rEEDVM.Dpt.RefDpt) { dirty = true; }
                //Update data
                rEED.RefDpt = rEEDVM.Dpt.RefDpt;
            }
            //Mark for modification if applicable
            if (dirty) { dataModel.DModif = DateTime.Now; }
        }
        #endregion
    }
}

