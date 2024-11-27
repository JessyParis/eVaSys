/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/09/2021
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class SecuriteController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public SecuriteController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/securite
        /// Retrieves the Securite rule
        /// </summary>
        [HttpGet]
        public IActionResult Get(int id)
        {
            var securite = DbContext.Securites
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .FirstOrDefault();
            // handle requests asking for non-existing object
            if (securite == null)
            {
                securite = new Securite();
                DbContext.Securites.Add(securite);
            }
            //End
            return new JsonResult(
                _mapper.Map<Securite, SecuriteViewModel>(securite),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Securite 
        /// </summary>
        /// <param name="model">The SecuriteViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] SecuriteViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            Securite securite = DbContext.Securites.FirstOrDefault();
            // handle requests asking for non-existing securitezes
            if (securite == null)
            {
                securite = new Securite();
                DbContext.Securites.Add(securite);
            }
            //Set values
            securite.DelaiAvantDesactivationUtilisateur = model.DelaiAvantDesactivationUtilisateur;
            securite.DelaiAvantChangementMotDePasse = model.DelaiAvantChangementMotDePasse;
            //Register session user
            securite.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
            //End
            // persist the changes into the Database.
            DbContext.SaveChanges();
            // return the updated Securite to the client.
            return new JsonResult(
                _mapper.Map<Securite, SecuriteViewModel>(securite),
                JsonSettings);
        }
        #endregion

        #region Attribute-based Routing
        #endregion
    }
}
