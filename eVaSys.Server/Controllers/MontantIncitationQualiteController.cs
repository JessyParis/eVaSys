/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class MontantIncitationQualiteController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public MontantIncitationQualiteController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/montantincitationqualite/{id}
        /// Retrieves the MontantIncitationQualite with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing MontantIncitationQualite</param>
        /// <returns>The MontantIncitationQualite with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            MontantIncitationQualite montantIncitationQualite = null;
            bool lockData = Request.Headers["lockData"].ToString() == "true" ? true : false;
            //Get or create commandeFournisseur
            if (id == 0)
            {
                montantIncitationQualite = new MontantIncitationQualite();
                DbContext.MontantIncitationQualites.Add(montantIncitationQualite);
            }
            else
            {
                montantIncitationQualite = DbContext.MontantIncitationQualites
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefMontantIncitationQualite == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (montantIncitationQualite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Lock data if applicable
            if (id != 0)
            {
                if (lockData)
                {
                    Utils.Utils.LockData(Enumerations.DataTypeToLock.RefMontantIncitationQualite.ToString(), CurrentContext.RefUtilisateur, montantIncitationQualite.RefMontantIncitationQualite, DbContext); ;
                }
            }
            //End
            return new JsonResult(
                _mapper.Map<MontantIncitationQualite, MontantIncitationQualiteViewModel>(montantIncitationQualite),
                JsonSettings);
        }

        /// <summary>
        /// Edit the MontantIncitationQualite with the given {id}
        /// </summary>
        /// <param name="model">The MontantIncitationQualiteViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] MontantIncitationQualiteViewModel model)
        {
            bool unLockData = Request.Headers["unLockData"].ToString() == "true" ? true : false;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            MontantIncitationQualite montantIncitationQualite;
            //Retrieve or create the entity to edit
            if (model.RefMontantIncitationQualite > 0)
            {
                montantIncitationQualite = DbContext.MontantIncitationQualites
                        .Where(q => q.RefMontantIncitationQualite == model.RefMontantIncitationQualite)
                        .FirstOrDefault();
            }
            else
            {
                montantIncitationQualite = new MontantIncitationQualite();
                DbContext.MontantIncitationQualites.Add(montantIncitationQualite);
            }
            // handle requests asking for non-existing montantIncitationQualitezes
            if (montantIncitationQualite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Set values
            UpdateData(ref montantIncitationQualite, model);
            //Register session user
            montantIncitationQualite.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //Check validation
            string valid = montantIncitationQualite.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Unlock data if applicable
                if (unLockData)
                {
                    Utils.Utils.UnlockData(Enumerations.DataTypeToLock.RefMontantIncitationQualite.ToString(), montantIncitationQualite.RefMontantIncitationQualite, CurrentContext.RefUtilisateur, DbContext);
                }
                // return the updated MontantIncitationQualite to the client.
                return new JsonResult(
                    _mapper.Map<MontantIncitationQualite, MontantIncitationQualiteViewModel>(montantIncitationQualite),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the MontantIncitationQualite with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the montantIncitationQualite from the Database
            var montantIncitationQualite = DbContext.MontantIncitationQualites.Where(i => i.RefMontantIncitationQualite == id).FirstOrDefault();
            // handle requests asking for non-existing montantIncitationQualitezes
            if (montantIncitationQualite == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = montantIncitationQualite.IsDeletable();
            if (del == "")
            {
                DbContext.MontantIncitationQualites.Remove(montantIncitationQualite);
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
        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref MontantIncitationQualite dataModel, MontantIncitationQualiteViewModel viewModel)
        {
            dataModel.Annee = viewModel.Annee;
            dataModel.Montant = viewModel.Montant;
        }
        #endregion
    }
}
