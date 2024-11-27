/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/08/2020
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class NonConformiteEtapeTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public NonConformiteEtapeTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/nonconformiteEtapeType/{id}
        /// Retrieves the NonConformiteEtapeType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NonConformiteEtapeType</param>
        /// <returns>The NonConformiteEtapeType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var nonConformiteEtapeType = DbContext.NonConformiteEtapeTypes
                    .Where(i => i.RefNonConformiteEtapeType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (nonConformiteEtapeType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<NonConformiteEtapeType, NonConformiteEtapeTypeViewModel>(nonConformiteEtapeType),
                JsonSettings);
        }

        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/nonConformiteEtapeType/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.NonConformiteEtapeType> req = DbContext.NonConformiteEtapeTypes;
            //Get data
            NonConformiteEtapeTypeViewModel[] all;
            if (CurrentContext.CurrentCulture.Name == "fr-FR")
            {
                all = req.OrderBy(el => el.Ordre).Select(p => new NonConformiteEtapeTypeViewModel()
                {
                    RefNonConformiteEtapeType = p.RefNonConformiteEtapeType,
                    Ordre = p.Ordre,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleFRFR,
                    Controle = p.Controle
                }).ToArray();
            }
            else
            {
                all = req.OrderBy(el => el.Ordre).Select(p => new NonConformiteEtapeTypeViewModel()
                {
                    RefNonConformiteEtapeType = p.RefNonConformiteEtapeType,
                    Ordre = p.Ordre,
                    LibelleFRFR = p.LibelleFRFR,
                    LibelleENGB = p.LibelleENGB,
                    Libelle = p.LibelleENGB,
                    Controle = p.Controle
                }).ToArray();
            }
            //Return Json
            return new JsonResult(all,
                JsonSettings);
        }
        #endregion
    }
}
