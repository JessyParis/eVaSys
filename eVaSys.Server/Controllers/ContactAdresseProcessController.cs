/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 3/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class ContactAdresseProcessController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public ContactAdresseProcessController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/contactadresseprocess/getlist
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("getlist")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.ContactAdresseProcess> req = DbContext.ContactAdresseProcesss;
            //Get data
            var all = req
                .Where(e=> e.RefContactAdresseProcess != 2 && e.RefContactAdresseProcess != 5 && e.RefContactAdresseProcess != 6 && e.RefContactAdresseProcess != 8)
                .OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<ContactAdresseProcess[], ContactAdresseProcessViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
