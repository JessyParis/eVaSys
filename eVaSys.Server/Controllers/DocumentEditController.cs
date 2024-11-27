/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 11/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using eValorplast.BLL;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class DocumentEditController : BaseApiController
    {
        #region Constructor
        public DocumentEditController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment env)
            : base(context, configuration)
        {
            _env = env;
        }
        protected IWebHostEnvironment _env;
        #endregion Constructor
        #region Attribute-based Routing

        /// <summary>
        /// GET: api/documentedit/certificatrecyclage
        /// ROUTING TYPE: attribute-based
        /// create file CertificatRecyclage
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("certificatrecyclage")]
        public IActionResult CertificatRecyclage()
        {
            string filterCollectivites = Request.Headers["filterCollectivites"].ToString();
            bool cS = (Request.Headers["cs"].ToString() == "true" ? true : false);
            string quarter = Request.Headers["quarter"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int q = 0, y = 0;
            int.TryParse(quarter, out q);
            int.TryParse(year, out y);
            if (q != 0 && y != 0)
            {
                MemoryStream mS = new();
                //Init
                //Create file
                mS = ExcelFileManagement.CreateCertificatRecyclage(new Quarter(q, y, CurrentContext.CurrentCulture), filterCollectivites, cS, "pdf", DbContext, _env.ContentRootPath);
                //return file
                string fileName = "CertificatRecyclageHCS.pdf";
                if (cS == true)
                {
                    fileName = "CertificatRecyclageCS.pdf";
                }
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = fileName,
                    // always prompt the user for downloading
                    Inline = false,
                };
                Response.Headers.Add("Content-Disposition", cD.ToString());
                return new FileContentResult(mS.ToArray(), "application/octet-stream");
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(617)));
            }
        }
        /// <summary>
        /// GET: api/documentedit/etattrimerstrielcollectivitecs
        /// ROUTING TYPE: attribute-based
        /// create file EtatTrimerstrielCollectiviteCS
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("etattrimerstrielcollectivitecs")]
        public IActionResult EtatTrimerstrielCollectiviteCS()
        {
            string filterCollectivites = Request.Headers["filterCollectivites"].ToString();
            string quarter = Request.Headers["quarter"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int q = 0, y = 0;
            int.TryParse(quarter, out q);
            int.TryParse(year, out y);
            if (q != 0 && y != 0)
            {
                MemoryStream mS = new();
                //Init
                //Create file
                mS = ExcelFileManagement.CreateEtatTrimestrielCollectiviteCS(new Quarter(q, y, CurrentContext.CurrentCulture), filterCollectivites, "pdf", DbContext, _env.ContentRootPath);
                //return file
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = "EtatTrimestrielCS.pdf",
                    // always prompt the user for downloading
                    Inline = false,
                };
                Response.Headers.Add("Content-Disposition", cD.ToString());
                return new FileContentResult(mS.ToArray(), "application/octet-stream");
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(617)));
            }
        }
        /// <summary>
        /// GET: api/documentedit/etattrimerstrielcollectivitecs
        /// ROUTING TYPE: attribute-based
        /// create file EtatTrimerstrielCollectivite
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("etatmensuelcollectivitehcs")]
        public IActionResult EtatMensuelCollectiviteHCS()
        {
            string filterCollectivites = Request.Headers["filterCollectivites"].ToString();
            string month = Request.Headers["month"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int m = 0, y = 0;
            int.TryParse(month, out m);
            int.TryParse(year, out y);
            if (m != 0 && y != 0)
            {
                MemoryStream mS = new();
                //Init
                //Create file
                mS = ExcelFileManagement.CreateEtatMensuelCollectiviteHCS(new Month(m, y, CurrentContext.CurrentCulture), filterCollectivites, "pdf", DbContext, _env.ContentRootPath);
                //return file
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = "EtatMensuelHCS.pdf",
                    // always prompt the user for downloading
                    Inline = false,
                };
                Response.Headers.Add("Content-Disposition", cD.ToString());
                return new FileContentResult(mS.ToArray(), "application/octet-stream");
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(617)));
            }
        }
        /// <summary>
        /// GET: api/documentedit/etatmensuelreceptionhcs
        /// ROUTING TYPE: attribute-based
        /// create file EtatMensuelReception
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("etatmensuelreception")]
        public IActionResult EtatMensuelReception()
        {
            string filterCentreDeTris = Request.Headers["filterCentreDeTris"].ToString();
            string month = Request.Headers["month"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int.TryParse(month, out int m);
            int.TryParse(year, out int y);
            if (m != 0 && y != 0)
            {
                MemoryStream mS = new();
                //Init
                //Create file
                mS = ExcelFileManagement.CreateEtatMensuelReception(m, y, filterCentreDeTris, "pdf", DbContext, _env.ContentRootPath);
                //return file
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = "EtatMensuelReception.pdf",
                    // always prompt the user for downloading
                    Inline = false,
                };
                Response.Headers.Add("Content-Disposition", cD.ToString());
                return new FileContentResult(mS.ToArray(), "application/octet-stream");
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(617)));
            }
        }
        /// <summary>
        /// GET: api/documentedit/etatmensuelreceptionhcs
        /// ROUTING TYPE: attribute-based
        /// create file EtatMensuelReception
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("ficherepartition")]
        public IActionResult FicheRepartition()
        {
            string refCentreDeTri = Request.Headers["refCentreDeTri"].ToString();
            //Check parameters
            int.TryParse(refCentreDeTri, out int r);
            if (r != 0)
            {
                MemoryStream mS = new();
                //Init
                //Create file
                mS = ExcelFileManagement.CreateFicheRepartition(r, "xlsx", DbContext, _env.ContentRootPath);
                //return file
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = "FicheDeRepartition.xlsx",
                    // always prompt the user for downloading
                    Inline = false,
                };
                Response.Headers.Append("Content-Disposition", cD.ToString());
                return new FileContentResult(mS.ToArray(), "application/octet-stream");
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(617)));
            }
        }
        /// <summary>
        /// GET: api/documentedit/incitationqualitecourriernonversement
        /// ROUTING TYPE: attribute-based
        /// create file IncitationQualiteCourrierNonVersement
        /// </summary>
        /// <returns>PDF file</returns>
        [HttpGet("incitationqualitecourriernonversement")]
        public IActionResult IncitationQualiteCourrierNonVersement()
        {
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int.TryParse(year, out int y);
            if (y != 0)
            {
                int r;
                //Init
                //Create files
                r = PdfFileManagement.CreateIncitationQualiteCourrierNonVersement(y, DbContext, _env.ContentRootPath, Configuration, CurrentContext);
                //return number of created files
                return new JsonResult(r);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(617)));
            }
        }
        #endregion
    }
}
    
