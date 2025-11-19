/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/12/2018
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class DptController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public DptController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/dpt/{id}
        /// Retrieves the dpt with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Dpt</param>
        /// <returns>the Dpt with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var dpt = DbContext.Dpts.Where(i => i.RefDpt == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (dpt == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Dpt, DptViewModel>(dpt),
                JsonSettings);
        }
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/items/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Where
            System.Linq.IQueryable<eVaSys.Data.Dpt> req = DbContext.Dpts;
            //Get data
            var all = req.OrderBy(el => el.RefDpt).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<Dpt[], DptViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
