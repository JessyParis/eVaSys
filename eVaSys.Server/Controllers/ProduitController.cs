/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/03/2019
/// -----------------------------------------------------------------------------------------------------
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class ProduitController : BaseApiController
    {
        private readonly IMapper _mapper;

        #region Constructor

        public ProduitController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }

        #endregion Constructor

        #region RESTful Conventions

        /// <summary>
        /// GET: evapi/produit/{id}
        /// Retrieves the Produit with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Produit</param>
        /// <returns>the Produit with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var produit = DbContext.Produits
                .Include(i => i.ProduitComposants)
                .Include(i => i.ProduitStandards)
                .Include(r => r.UtilisateurCreation)
                .Include(r => r.UtilisateurModif)
                .Where(i => i.RefProduit == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (produit == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Produit, ProduitViewModel>(produit),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Produit with the given {id}
        /// </summary>
        /// <param name="model">The ProduitViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] ProduitViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Produit produit;
            //Retrieve or create the entity to edit
            if (model.RefProduit > 0)
            {
                produit = DbContext.Produits
                    .Include(i => i.ProduitComposants)
                    .Include(i => i.ProduitStandards)
                    .Where(q => q.RefProduit == model.RefProduit)
                    .FirstOrDefault();
            }
            else
            {
                produit = new Produit();
                DbContext.Produits.Add(produit);
            }
            // handle requests asking for non-existing produitzes
            if (produit == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            Utils.DataUtils.UpdateDataProduit(ref produit, model, CurrentContext.RefUtilisateur);

            //Register session user
            produit.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = produit.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Produit to the client.
                return new JsonResult(
                    _mapper.Map<Produit, ProduitViewModel>(produit),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Produit with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the produit from the Database
            var produit = DbContext.Produits
                .Include(r => r.ProduitComposants)
                .Include(r => r.ProduitStandards)
                .Where(i => i.RefProduit == id)
                .FirstOrDefault();

            // handle requests asking for non-existing produitzes
            if (produit == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = produit.IsDeletable();
            if (del == "")
            {
                DbContext.Produits.Remove(produit);
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

        #endregion RESTful Conventions

        #region Attribute-based Routing

        /// <summary>
        /// GET: api/items/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            bool actif = (Request.Headers["actif"] == "true" ? true : false);
            bool collecte = (Request.Headers["collecte"] == "true" ? true : false);
            int? refEntite = (string.IsNullOrEmpty(Request.Headers["refEntite"]) ? null : (int?)System.Convert.ToInt32(Request.Headers["refEntite"]));
            int? refCollectivite = (string.IsNullOrEmpty(Request.Headers["refCollectivite"]) ? null : (int?)System.Convert.ToInt32(Request.Headers["refCollectivite"]));
            int? refProduitCompose = (string.IsNullOrEmpty(Request.Headers["refProduitCompose"]) ? null : (int?)System.Convert.ToInt32(Request.Headers["refProduitCompose"]));
            int? refProduitARepartir = (string.IsNullOrEmpty(Request.Headers["refProduitARepartir"]) ? null : (int?)System.Convert.ToInt32(Request.Headers["refProduitARepartir"]));
            int? refProcess = (string.IsNullOrEmpty(Request.Headers["refProcess"]) ? null : (int?)System.Convert.ToInt32(Request.Headers["refProcess"]));
            int? refProduit = (string.IsNullOrEmpty(Request.Headers["refProduit"]) ? null : (int?)System.Convert.ToInt32(Request.Headers["refProduit"]));
            DateTime? dRepartition = (string.IsNullOrEmpty(Request.Headers["dRepartition"]) ? null : (DateTime?)System.Convert.ToDateTime(Request.Headers["dRepartition"]));
            bool composant = (Request.Headers["composant"] == "true" ? true : false);
            System.Linq.IQueryable<eVaSys.Data.Produit> req = DbContext.Produits;
            if (actif || CurrentContext.filterGlobalActif)
            {
                if (refProduit != null) { req = req.Where(el => el.Actif == true || el.RefProduit == refProduit); }
                else { req = req.Where(el => el.Actif == true); }
            }
            if (collecte)
            {
                req = req.Where(el => el.Collecte == true);
            }
            if (refEntite != null)
            {
                if (refProduit == null)
                {
                    req = req.Where(el => el.EntiteProduits.Any(rel => rel.RefEntite == refEntite && !rel.Interdit));
                }
                else
                {
                    req = req.Where(el => (el.EntiteProduits.Any(rel => rel.RefEntite == refEntite && !rel.Interdit) || el.RefProduit == refProduit));
                } 
            }
            //Composants
            if (refProduitCompose != null)
            {
                int[] refProduits;
                if (refProduitCompose == 0)
                {
                    refProduits = DbContext.ProduitComposants
                        .Select(p => p.RefComposant).Distinct().ToArray();
                    req = req.Where(el => refProduits.Contains(el.RefProduit));
                }
                else
                {
                    refProduits = DbContext.ProduitComposants
                        .Where(e => e.RefProduit == refProduitCompose)
                        .Select(p => p.RefComposant).ToArray();
                    req = req.Where(el => refProduits.Contains(el.RefProduit));
                }
            }
            if (composant)
            {
                req = req.Where(el => el.Composant == true);
            }
            if (refProduitARepartir != null)
            {
                int[] refProduits = new[] { 0 };
                if (dRepartition != null && refProcess != null)
                {
                    refProduits = DbContext.PrixReprises
                        .Where(e => e.RefProduit == refProduitARepartir && e.RefProcess == refProcess && e.D.Month == ((DateTime)dRepartition).Month && e.D.Year == ((DateTime)dRepartition).Year)
                        .Select(p => p.RefComposant).ToArray();
                }
                req = req.Where(el => refProduits.Contains(el.RefProduit));
            }
            if (refCollectivite != null)
            {
                int?[] refProduits = new int?[] { 0 };
                refProduits = DbContext.RepartitionDetails
                    .Where(e => e.RefCollectivite == refCollectivite)
                    .Select(p => p.RefProduit).Distinct().ToArray();
                req = req.Where(el => refProduits.Contains(el.RefProduit));
            }
            //Get data
            var all = req.Distinct().OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Produit[], ProduitViewModel[]>(all),
                JsonSettings);
        }
        #endregion Attribute-based Routing
    }
}