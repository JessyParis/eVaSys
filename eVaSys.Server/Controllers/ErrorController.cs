/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.APIUtils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace eVaSys.Controllers
{
    [Route("/error")]
    public class ErrorController : Controller
    {
        #region Constructor
        public ErrorController() { }
        #endregion Constructor

        [Route("{code}")]
        public IActionResult Error(int code)
        {
            HttpStatusCode parsedCode = (HttpStatusCode)code;
            ApiError error = new(code, parsedCode.ToString());

            return new ObjectResult(error);
        }
    }
}
