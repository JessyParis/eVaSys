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
    public class DescriptionCVQController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public DescriptionCVQController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/descriptioncvq/{id}
        /// Retrieves the DescriptionCVQ with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing DescriptionCVQ</param>
        /// <returns>the DescriptionCVQ with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            DescriptionCVQ descriptionCVQ = null;
            //Get or create descriptionCVQ
            if (id == 0)
            {
                descriptionCVQ = new DescriptionCVQ();
                DbContext.DescriptionCVQs.Add(descriptionCVQ);
            }
            else
            {
                descriptionCVQ = DbContext.DescriptionCVQs
                    .Include(r => r.DescriptionCVQProduits)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefDescriptionCVQ == id).FirstOrDefault();
            }
            // handle requests asking for non-existing object
            if (descriptionCVQ == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            descriptionCVQ.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<DescriptionCVQ, DescriptionCVQViewModel>(descriptionCVQ),
                JsonSettings);
        }

        /// <summary>
        /// Edit the DescriptionCVQ with the given {id}
        /// </summary>
        /// <param name="model">The DescriptionCVQViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] DescriptionCVQViewModel model)
        {
            string valid = null;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            DescriptionCVQ descriptionCVQ;
            //Retrieve or create the entity to edit
            if (model.RefDescriptionCVQ > 0)
            {
                descriptionCVQ = DbContext.DescriptionCVQs
                    .Include(r => r.DescriptionCVQProduits)
                    .Where(q => q.RefDescriptionCVQ == model.RefDescriptionCVQ)
                    .FirstOrDefault();
            }
            else
            {
                descriptionCVQ = new DescriptionCVQ();
                DbContext.DescriptionCVQs.Add(descriptionCVQ);
            }
            // handle requests asking for non-existing descriptionCVQzes
            if (descriptionCVQ == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref descriptionCVQ, model);

            //Register session user
            descriptionCVQ.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            valid = descriptionCVQ.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated DescriptionCVQ to the client.
                return new JsonResult(
                    _mapper.Map<DescriptionCVQ, DescriptionCVQViewModel>(descriptionCVQ),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the DescriptionCVQ with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the descriptionCVQ from the Database
            var descriptionCVQ = DbContext.DescriptionCVQs
                .Include(r => r.DescriptionCVQProduits)
                .Where(i => i.RefDescriptionCVQ == id)
                .FirstOrDefault();

            // handle requests asking for non-existing descriptionCVQzes
            if (descriptionCVQ == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = descriptionCVQ.IsDeletable();
            if (del == "")
            {
                DbContext.DescriptionCVQs.Remove(descriptionCVQ);
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
        /// GET: evapi/getdescriptioncvqs
        /// Retrieves the DescriptionCVQs according to RefProduit
        /// </summary>
        /// <returns>the correponding DescriptionCVQs</returns>
        [HttpGet("getdescriptioncvqs")]
        public IActionResult GetDescriptionCVQs()
        {
            //Get headers
            string refProduit = Request.Headers["refProduit"].ToString();
            int refP = 0;
            List<DescriptionCVQ> dCs = new();
            //Get DescriptionCVQs
            if (int.TryParse(refProduit, out refP))
            {
                dCs = DbContext.DescriptionCVQs
                    .Where(el => el.DescriptionCVQProduits.Any(rel => rel.RefProduit == refP))
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
                _mapper.Map<DescriptionCVQ[], DescriptionCVQViewModel[]>(dCs.ToArray()),
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
                if (DbContext.DescriptionCVQs
                    .Where(el => el.DescriptionCVQProduits
                    .Any(rel => rel.RefProduit == refPDest)).Count() > 0)
                {
                    return Conflict(new ConflictError(CurrentContext.CulturedRessources.GetTextRessource(761)));
                }
                else
                {
                    //Get elements to copy
                    var dCs = DbContext.DescriptionCVQs
                        .Include(i => i.DescriptionCVQProduits)
                        .Where(el => el.DescriptionCVQProduits
                        .Any(rel => rel.RefProduit == refP))
                        .ToList();
                    var dCPs = dCs.SelectMany(e => e.DescriptionCVQProduits).ToList();
                    foreach (var dC in dCs)
                    {
                        DbContext.Entry(dC).State = EntityState.Detached;
                        dC.RefDescriptionCVQ = 0;
                        dC.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                        dC.RefUtilisateurCreation = CurrentContext.RefUtilisateur;
                        dC.DCreation = DateTime.Now;
                        dC.RefUtilisateurModif = null;
                        dC.DModif = null;
                    }
                    foreach (var dCP in dCPs)
                    {
                        DbContext.Entry(dCP).State = EntityState.Detached;
                        dCP.RefDescriptionCVQProduit = 0;
                        dCP.RefProduit = refPDest;
                    }
                    DbContext.DescriptionCVQs.AddRange(dCs);
                    DbContext.DescriptionCVQProduits.AddRange(dCPs);
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
        private void UpdateData(ref DescriptionCVQ dataModel, DescriptionCVQViewModel viewModel)
        {
            dataModel.Ordre = viewModel.Ordre;
            dataModel.LibelleFRFR = viewModel.LibelleFRFR;
            dataModel.LibelleENGB = viewModel.LibelleENGB;
            dataModel.Actif = viewModel.Actif ?? false;
            dataModel.LimiteBasse = viewModel.LimiteBasse;
            dataModel.LimiteHaute = viewModel.LimiteHaute;
            //Related product
            //Remove related data DescriptionCVQProduit
            if (dataModel.DescriptionCVQProduits != null)
            {
                foreach (DescriptionCVQProduit dCP in dataModel.DescriptionCVQProduits)
                {
                    if (viewModel.DescriptionCVQProduits == null)
                    {
                        DbContext.DescriptionCVQProduits.Remove(dCP);
                    }
                    else if (viewModel.DescriptionCVQProduits.Where(el => el.RefDescriptionCVQProduit == dCP.RefDescriptionCVQProduit).FirstOrDefault() == null)
                    {
                        DbContext.DescriptionCVQProduits.Remove(dCP);
                    }
                }
            }
            //Add or update related data DescriptionCVQProduits
            foreach (DescriptionCVQProduitViewModel dCPVM in viewModel.DescriptionCVQProduits)
            {
                DescriptionCVQProduit dCP = null;
                if (dataModel.DescriptionCVQProduits != null && dCPVM.RefDescriptionCVQProduit != 0)
                {
                    dCP = dataModel.DescriptionCVQProduits.Where(el => el.RefDescriptionCVQProduit == dCPVM.RefDescriptionCVQProduit).FirstOrDefault();
                }
                if (dCP == null)
                {
                    dCP = new DescriptionCVQProduit();
                    DbContext.DescriptionCVQProduits.Add(dCP);
                    if (dataModel.DescriptionCVQProduits == null) { dataModel.DescriptionCVQProduits = new HashSet<DescriptionCVQProduit>(); }
                    dataModel.DescriptionCVQProduits.Add(dCP);
                }
                dCP.RefProduit = dCPVM.Produit.RefProduit;
            }
        }
        #endregion
    }
}

