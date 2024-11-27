/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/05/2019
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
    public class CommandeFournisseurStatutController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public CommandeFournisseurStatutController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/CommandeFournisseurStatut/{id}
        /// Retrieves the CommandeFournisseurStatut with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing CommandeFournisseurStatut</param>
        /// <returns>The CommandeFournisseurStatut with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var commandeFournisseurStatut = DbContext.CommandeFournisseurStatuts.Where(i => i.RefCommandeFournisseurStatut == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (commandeFournisseurStatut == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<CommandeFournisseurStatut, CommandeFournisseurStatutViewModel>(commandeFournisseurStatut),
                JsonSettings);
        }
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/CommandeFournisseurStatut/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Where
            System.Linq.IQueryable<eVaSys.Data.CommandeFournisseurStatut> req = DbContext.CommandeFournisseurStatuts;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<CommandeFournisseurStatut[], CommandeFournisseurStatutViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
