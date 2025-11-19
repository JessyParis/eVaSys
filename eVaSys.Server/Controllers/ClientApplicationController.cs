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
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class ClientApplicationController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ClientApplicationController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/clientapplication/{id}
        /// Retrieves the ClientApplication with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing ClientApplication</param>
        /// <returns>the ClientApplication with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            ClientApplication clientApplication = null;
            //Get or create clientApplication
            if (id == 0)
            {
                clientApplication = new ClientApplication();
                clientApplication.D = new DateTime(DateTime.Now.Year - 1, 1, 1, 0, 0, 0);
                DbContext.ClientApplications.Add(clientApplication);
            }
            else
            {
                clientApplication = DbContext.ClientApplications
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Include(r => r.ClientApplicationApplications)
                    .Where(i => i.RefClientApplication == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (clientApplication == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //Add missing ClientApplicationApplication
            if (clientApplication.ClientApplicationApplications == null) { clientApplication.ClientApplicationApplications = new HashSet<ClientApplicationApplication>(); }
            foreach (var app in DbContext.Applications.OrderBy(o => o.Libelle).ToList())
            {
                if (clientApplication.ClientApplicationApplications.Where(e => e.RefApplication == app.RefApplication).FirstOrDefault() == null)
                {
                    clientApplication.ClientApplicationApplications.Add(new ClientApplicationApplication() { Application = app });
                }
            }
            //End
            clientApplication.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<ClientApplication, ClientApplicationViewModel>(clientApplication),
                JsonSettings);
        }

        /// <summary>
        /// Modify ClientApplication
        /// </summary>
        /// <param name="model">The ClientApplicationViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] ClientApplicationViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            ClientApplication clientApplication;
            //Retrieve or create the entity to edit
            if (model.RefClientApplication > 0)
            {
                clientApplication = DbContext.ClientApplications
                        .Where(q => q.RefClientApplication == model.RefClientApplication)
                        .Include(r => r.ClientApplicationApplications)
                        .FirstOrDefault();
            }
            else
            {
                clientApplication = new ClientApplication();
                DbContext.ClientApplications.Add(clientApplication);
            }

            // handle requests asking for non-existing clientApplicationzes
            if (clientApplication == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref clientApplication, model);

            //Register session user
            clientApplication.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = clientApplication.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Return the updated ClientApplication to the client, or return the next Mixte if applicable
                return new JsonResult(
                    _mapper.Map<ClientApplication, ClientApplicationViewModel>(clientApplication),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the ClientApplication with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the clientApplication from the Database
            var clientApplication = DbContext.ClientApplications.Where(i => i.RefClientApplication == id)
                .Include(r => r.ClientApplicationApplications)
                .FirstOrDefault();

            // handle requests asking for non-existing clientApplicationzes
            if (clientApplication == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = clientApplication.IsDeletable();
            if (del == "")
            {
                DbContext.ClientApplications.Remove(clientApplication);
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
        private void UpdateData(ref ClientApplication dataModel, ClientApplicationViewModel viewModel)
        {
            dataModel.RefEntite = viewModel.Entite.RefEntite;
            dataModel.D = viewModel.D;
            dataModel.RefApplicationProduitOrigine = viewModel.ApplicationProduitOrigine.RefApplicationProduitOrigine;
            if (dataModel.ClientApplicationApplications == null) { dataModel.ClientApplicationApplications = new HashSet<ClientApplicationApplication>(); }
            //Dirty marker
            bool dirty = false;
            //Remove related data ClientApplicationApplication
            if (dataModel.ClientApplicationApplications != null)
            {
                foreach (ClientApplicationApplication cAA in dataModel.ClientApplicationApplications)
                {
                    if (viewModel.ClientApplicationApplications == null)
                    {
                        DbContext.ClientApplicationApplications.Remove(cAA);
                        dirty = true;
                    }
                    else if (viewModel.ClientApplicationApplications.Where(el => el.RefClientApplicationApplication == cAA.RefClientApplicationApplication).FirstOrDefault() == null)
                    {
                        DbContext.ClientApplicationApplications.Remove(cAA);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ClientApplicationApplications
            foreach (ClientApplicationApplicationViewModel rCVM in viewModel.ClientApplicationApplications)
            {
                ClientApplicationApplication cAA = null;
                if (dataModel.ClientApplicationApplications != null && rCVM.RefClientApplicationApplication != 0)
                {
                    cAA = dataModel.ClientApplicationApplications.Where(el => el.RefClientApplicationApplication == rCVM.RefClientApplicationApplication).FirstOrDefault();
                }
                if (cAA == null)
                {
                    cAA = new ClientApplicationApplication();
                    DbContext.ClientApplicationApplications.Add(cAA);
                    if (dataModel.ClientApplicationApplications == null) { dataModel.ClientApplicationApplications = new HashSet<ClientApplicationApplication>(); }
                    dataModel.ClientApplicationApplications.Add(cAA);
                }
                //Mark as dirty if applicable
                if (cAA.RefApplication != rCVM.Application.RefApplication
                    || cAA.Ratio != rCVM.Ratio) { dirty = true; }
                //Update data
                cAA.RefApplication = rCVM.Application.RefApplication;
                cAA.Ratio = rCVM.Ratio;
            }
            //Mark for modification if applicable
            if (dirty) { dataModel.DModif = DateTime.Now; }
        }
        #endregion
    }
}

