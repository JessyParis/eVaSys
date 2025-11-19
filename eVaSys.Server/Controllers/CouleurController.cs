/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/10/2022
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class CouleurController : BaseApiController
    {
        #region Constructor
        public CouleurController(ApplicationDbContext context, IConfiguration configuration)
            : base(context, configuration)
        {
        }
        #endregion Constructor

        #region RESTful Conventions
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/couleur/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            var dT = new DataTable();
            dT.Columns.Add("RefCouleur", typeof(string));
            dT.Columns.Add("Libelle", typeof(string));
            object[] rowVals = new object[2];
            rowVals[0] = "#22529D";
            rowVals[1] = "#22529D";
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = "#6BB135";
            rowVals[1] = "#6BB135";
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = "#F9B62E";
            rowVals[1] = "#F9B62E";
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = "#E53636";
            rowVals[1] = "#E53636";
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = "#37BCE5";
            rowVals[1] = "#37BCE5";
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = "#9F9F9F";
            rowVals[1] = "#9F9F9F";
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = "#632AAA";
            rowVals[1] = "#632AAA";
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = "#9C227A";
            rowVals[1] = "#9C227A";
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = "#9D2222";
            rowVals[1] = "#9D2222";
            dT.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = "#C5C74D";
            rowVals[1] = "#C5C74D";
            dT.Rows.Add(rowVals);
            //End
            return new JsonResult(dT, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        }
        #endregion
    }
}
