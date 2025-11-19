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
    public class DescriptionControleController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public DescriptionControleController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/descriptioncontrole/{id}
        /// Retrieves the DescriptionControle with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing DescriptionControle</param>
        /// <returns>the DescriptionControle with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            DescriptionControle descriptionControle = null;
            //Get or create descriptionControle
            if (id == 0)
            {
                descriptionControle = new DescriptionControle();
                DbContext.DescriptionControles.Add(descriptionControle);
            }
            else
            {
                descriptionControle = DbContext.DescriptionControles
                    .Include(r => r.DescriptionControleProduits)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefDescriptionControle == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (descriptionControle == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            descriptionControle.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<DescriptionControle, DescriptionControleViewModel>(descriptionControle),
                JsonSettings);
        }

        /// <summary>
        /// Edit the DescriptionControle with the given {id}
        /// </summary>
        /// <param name="model">The DescriptionControleViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] DescriptionControleViewModel model)
        {
            string valid = null;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            DescriptionControle descriptionControle;
            //Retrieve or create the entity to edit
            if (model.RefDescriptionControle > 0)
            {
                descriptionControle = DbContext.DescriptionControles
                    .Include(r => r.DescriptionControleProduits)
                    .Where(q => q.RefDescriptionControle == model.RefDescriptionControle)
                    .FirstOrDefault();
            }
            else
            {
                descriptionControle = new DescriptionControle();
                DbContext.DescriptionControles.Add(descriptionControle);
            }
            // handle requests asking for non-existing descriptionControlezes
            if (descriptionControle == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref descriptionControle, model);

            //Register session user
            descriptionControle.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            valid = descriptionControle.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // detach the entity to get fresh data after trigger execution
                DbContext.Entry(descriptionControle).Reload();
                // return the updated DescriptionControle to the client.
                return new JsonResult(
                    _mapper.Map<DescriptionControle, DescriptionControleViewModel>(descriptionControle),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the DescriptionControle with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the descriptionControle from the Database
            var descriptionControle = DbContext.DescriptionControles
                .Include(r => r.DescriptionControleProduits)
                .Where(i => i.RefDescriptionControle == id)
                .FirstOrDefault();

            // handle requests asking for non-existing descriptionControlezes
            if (descriptionControle == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = descriptionControle.IsDeletable();
            if (del == "")
            {
                DbContext.DescriptionControles.Remove(descriptionControle);
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
        /// GET: evapi/descriptioncontrole/getdescriptioncontroles
        /// Retrieves the DescriptionControles according to RefProduit
        /// </summary>
        /// <returns>the correponding DescriptionControles</returns>
        [HttpGet("getdescriptioncontroles")]
        public IActionResult GetDescriptionControles()
        {
            //Get headers
            string refProduit = Request.Headers["refProduit"].ToString();
            int refP = 0;
            List<DescriptionControle> dCs = new();
            //Get DescriptionControles
            if (int.TryParse(refProduit, out refP))
            {
                dCs = DbContext.DescriptionControles
                    .Where(el => el.DescriptionControleProduits.Any(rel => rel.RefProduit == refP))
                    .OrderBy(o => o.Ordre)
                    .ToList();
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
            // handle requests asking for non-existing object
            if (dCs.Count == 0)
            {
                return new EmptyResult();
            }
            //Return Json
            return new JsonResult(
                _mapper.Map<DescriptionControle[], DescriptionControleViewModel[]>(dCs.ToArray()),
                JsonSettings);
        }
        /// <summary>
        /// GET: evapi/descriptioncontrole/copy
        /// Copy all DescriptionControle based on RefProduit
        /// </summary>
        /// <returns>Empty result or error message</returns>
        [HttpGet("copy")]
        public IActionResult Copy()
        {
            //Get headers
            string refProduit = Request.Headers["refProduit"].ToString();
            string refProduitDest = Request.Headers["refProduitDest"].ToString();
            int refP = 0;
            int refPDest = 0;
            int.TryParse(refProduit, out refP);
            int.TryParse(refProduitDest, out refPDest);
            //Process
            if (refP != 0 && refPDest != 0)
            {
                //Check if product does not already have DescriptionControls
                if (DbContext.DescriptionControles
                    .Where(el => el.DescriptionControleProduits
                    .Any(rel => rel.RefProduit == refPDest)).Count() > 0)
                {
                    return Conflict(new ConflictError(CurrentContext.CulturedRessources.GetTextRessource(761)));
                }
                else
                {
                    //Get elements to copy
                    var dCs = DbContext.DescriptionControles
                        .Include(i => i.DescriptionControleProduits)
                        .Where(el => el.DescriptionControleProduits
                        .Any(rel => rel.RefProduit == refP))
                        .ToList();
                    var dCPs = dCs.SelectMany(e => e.DescriptionControleProduits).ToList();
                    foreach (var dC in dCs)
                    {
                        DbContext.Entry(dC).State = EntityState.Detached;
                        dC.RefDescriptionControle = 0;
                        dC.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                        dC.RefUtilisateurCreation = CurrentContext.RefUtilisateur;
                        dC.DCreation = DateTime.Now;
                        dC.RefUtilisateurModif = null;
                        dC.DModif = null;
                    }
                    foreach (var dCP in dCPs)
                    {
                        DbContext.Entry(dCP).State = EntityState.Detached;
                        dCP.RefDescriptionControleProduit = 0;
                        dCP.RefProduit = refPDest;
                    }
                    DbContext.DescriptionControles.AddRange(dCs);
                    DbContext.DescriptionControleProduits.AddRange(dCPs);
                    DbContext.SaveChanges();
                    return new EmptyResult();
                }
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
        private void UpdateData(ref DescriptionControle dataModel, DescriptionControleViewModel viewModel)
        {
            dataModel.Ordre = viewModel.Ordre;
            dataModel.LibelleFRFR = Utils.Utils.SetEmptyStringToNull(viewModel.LibelleFRFR);
            dataModel.LibelleENGB = Utils.Utils.SetEmptyStringToNull(viewModel.LibelleENGB);
            dataModel.Actif = viewModel.Actif ?? false;
            dataModel.CalculLimiteConformite = viewModel.CalculLimiteConformite ?? false;
            //Related product
            //Remove related data DescriptionControleProduit
            if (dataModel.DescriptionControleProduits != null)
            {
                foreach (DescriptionControleProduit dCP in dataModel.DescriptionControleProduits)
                {
                    if (viewModel.DescriptionControleProduits == null)
                    {
                        DbContext.DescriptionControleProduits.Remove(dCP);
                    }
                    else if (viewModel.DescriptionControleProduits.Where(el => el.RefDescriptionControleProduit == dCP.RefDescriptionControleProduit).FirstOrDefault() == null)
                    {
                        DbContext.DescriptionControleProduits.Remove(dCP);
                    }
                }
            }
            //Add or update related data DescriptionControleProduits
            foreach (DescriptionControleProduitViewModel dCPVM in viewModel.DescriptionControleProduits)
            {
                DescriptionControleProduit dCP = null;
                if (dataModel.DescriptionControleProduits != null && dCPVM.RefDescriptionControleProduit != 0)
                {
                    dCP = dataModel.DescriptionControleProduits.Where(el => el.RefDescriptionControleProduit == dCPVM.RefDescriptionControleProduit).FirstOrDefault();
                }
                if (dCP == null)
                {
                    dCP = new DescriptionControleProduit();
                    DbContext.DescriptionControleProduits.Add(dCP);
                    if (dataModel.DescriptionControleProduits == null) { dataModel.DescriptionControleProduits = new HashSet<DescriptionControleProduit>(); }
                    dataModel.DescriptionControleProduits.Add(dCP);
                }
                dCP.RefProduit = dCPVM.Produit.RefProduit;
            }
        }
        #endregion
    }
}

