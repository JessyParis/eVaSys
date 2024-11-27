/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 15/01/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using GemBox.Pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Web;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class DownloadController : BaseApiController
    {
        #region Constructor
        public DownloadController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment env)
            : base(context, configuration)
        {
            _env = env;
        }
        protected IWebHostEnvironment _env;
        #endregion Constructor
        #region Attribute-based Routing
        /// <summary>
        /// Get existing file
        /// </summary>
        [HttpGet("GetFile")]
        public IActionResult GetFile()
        {
            string fileType = Request.Headers["fileType"].ToString();
            string fileRef = Request.Headers["fileRef"].ToString();
            string fileFormat = Request.Headers["fileFormat"].ToString();
            string docType = Request.Headers["docType"].ToString();
            string containerRef = Request.Headers["containerRef"].ToString();
            int fR = -1;
            int cR = 0;
            int.TryParse(fileRef, out fR);
            int.TryParse(containerRef, out cR);
            if (fileType != "")
            {
                MemoryStream mS = null;
                string fileName = "tmp";
                //Get the file
                switch (fileType)
                {
                    case "CommandeFournisseurFichier":
                        CommandeFournisseurFichier cmdFF = DbContext.CommandeFournisseurFichiers.Where(e => e.RefCommandeFournisseurFichier == fR).FirstOrDefault();
                        if (cmdFF != null)
                        {
                            //Get CommandeFournisseur
                            CommandeFournisseur cmdF = DbContext.CommandeFournisseurs.Where(e => e.RefCommandeFournisseur == cmdFF.RefCommandeFournisseur).FirstOrDefault();
                            if (cmdF != null)
                            {
                                //Rights
                                if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationLogistique)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Client.ToString() && cmdF.AdresseClient.RefEntite != CurrentContext.ConnectedUtilisateur.Client.RefEntite)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Transporteur.ToString() && cmdF.Transporteur.RefEntite != CurrentContext.ConnectedUtilisateur.Transporteur.RefEntite)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.CentreDeTri.ToString() && cmdF.RefEntite != CurrentContext.ConnectedUtilisateur.CentreDeTri.RefEntite)
                                    )
                                {
                                    mS = new MemoryStream(cmdFF.Corps);
                                    fileName = cmdFF.Nom;
                                }
                            }
                        }
                        break;
                    case "NonConformiteFichier":
                        var nCF = DbContext.NonConformiteFichiers.Where(e => e.RefNonConformiteFichier == fR).FirstOrDefault();
                        if (nCF != null)
                        {
                            //Get NonConformite
                            var nC = DbContext.NonConformites.Where(e => e.RefNonConformite == nCF.RefNonConformite).FirstOrDefault();
                            if (nC != null)
                            {
                                //Rights
                                if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationQualite)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Client.ToString() && nC.CommandeFournisseur.AdresseClient.RefEntite != CurrentContext.ConnectedUtilisateur.Client.RefEntite)
                                    )
                                {
                                    mS = new MemoryStream(nCF.Corps);
                                    fileName = nCF.Nom;
                                }
                            }
                        }
                        break;
                    case "CommandeAffretement":
                        if (fileFormat == "xlsx" || fileFormat == "pdf")
                        {
                            CommandeFournisseur cmdF = DbContext.CommandeFournisseurs.Where(e => e.RefCommandeFournisseur == fR).FirstOrDefault();
                            if (cmdF != null)
                            {
                                //Rights
                                if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationLogistique)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Client.ToString() && cmdF.AdresseClient.RefEntite != CurrentContext.ConnectedUtilisateur.Client.RefEntite)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Transporteur.ToString() && cmdF.Transporteur.RefEntite != CurrentContext.ConnectedUtilisateur.Transporteur.RefEntite)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.CentreDeTri.ToString() && cmdF.RefEntite != CurrentContext.ConnectedUtilisateur.CentreDeTri.RefEntite)
                                    )
                                {
                                    //File type
                                    if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null
                                        || CurrentContext.ConnectedUtilisateur.RefClient != null
                                        || CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null) { fileFormat = "pdf"; }
                                    //fileFormat = "pdf";
                                    //Process
                                    mS = ExcelFileManagement.CreateCommandeAffretement(fR, docType == "courrierCommandeAffretement" ? true : false, null, fileFormat, CurrentContext, DbContext, _env.ContentRootPath);
                                    fileName = "CommandeAffretement." + fileFormat;
                                }
                            }
                        }
                        break;
                    case "Document":
                        //Check parameters
                        if (fR > 0)
                        {
                            if (Rights.AuthorizedDocument(fR, CurrentContext.ConnectedUtilisateur, DbContext))
                            {
                                var doc = DbContext.Documents
                                    .Where(e => e.RefDocument == fR).FirstOrDefault();
                                if (doc != null)
                                {
                                    mS = new MemoryStream(doc.Corps);
                                    fileName = doc.Nom;
                                    break;
                                }
                            }
                        }
                        break;
                    case "ActionFichier":
                        //Check parameters
                        if (fR > 0)
                        {
                            //Rights
                            if (
                                (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationAnnuaire)
                                && (CurrentContext.ConnectedUtilisateur.HabilitationAnnuaire == Enumerations.HabilitationQualite.Administrateur.ToString()
                                   || CurrentContext.ConnectedUtilisateur.HabilitationAnnuaire == Enumerations.HabilitationQualite.Utilisateur.ToString())
                                ))
                            {
                                var aF = DbContext.ActionFichiers
                                    .Where(e => e.RefActionFichier == fR).FirstOrDefault();
                                if (aF != null)
                                {
                                    mS = new MemoryStream(aF.Corps);
                                    fileName = aF.Nom;
                                    break;
                                }
                            }
                        }
                        break;
                    case "EmailFichier":
                        if (cR != 0)
                        {
                            //Get e-mail
                            Email em = DbContext.Emails.Where(e => e.RefEmail == cR).FirstOrDefault();
                            if (em != null)
                            {
                                //Rights
                                if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationLogistique)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Client.ToString())
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Transporteur.ToString())
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.CentreDeTri.ToString())
                                    )
                                {
                                    var emF = em.EmailFichiers.Where(el => el.Index == fR && el.VisualType == "nonvisual").FirstOrDefault();
                                    if (emF != null)
                                    {
                                        mS = new MemoryStream(emF.Corps);
                                        fileName = emF.Nom;
                                    }
                                }
                            }
                        }
                        break;
                    case "FicheControle":
                        if (fileFormat == "xlsx" || fileFormat == "pdf")
                        {
                            FicheControle fC = DbContext.FicheControles
                                .Where(e => e.RefFicheControle == fR)
                                .FirstOrDefault();
                            if (fC != null)
                            {
                                //Rights
                                if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationQualite)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Client.ToString() && fC.CommandeFournisseur.AdresseClient.RefEntite != CurrentContext.ConnectedUtilisateur.Client.RefEntite)
                                    )
                                {
                                    //Process
                                    mS = ExcelFileManagement.CreateFicheControle(fR, false, null, "pdf", CurrentContext, DbContext, _env.ContentRootPath);
                                    fileName = "FicheControle-" + fC.CommandeFournisseur.NumeroCommande + ".pdf";
                                }
                            }
                        }
                        break;
                    case "NonConformite":
                        if (fileFormat == "xlsx" || fileFormat == "pdf")
                        {
                            var nC = DbContext.NonConformites
                                .Where(e => e.RefNonConformite == fR)
                                .FirstOrDefault();
                            if (nC != null)
                            {
                                //Rights
                                if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationQualite)
                                    && !(CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Client.ToString() && nC.CommandeFournisseur.AdresseClient.RefEntite != CurrentContext.ConnectedUtilisateur.Client.RefEntite)
                                    )
                                {
                                    //Process
                                    //ExcelFileManagement.CreateNonConformiteTest();
                                    mS = ExcelFileManagement.CreateNonConformite(fR, docType, CurrentContext, DbContext, _env.ContentRootPath);
                                    fileName = "NonConformite" + (docType == "Client" ? "Client" : "CDT") + "-" + nC.CommandeFournisseur.NumeroCommande + ".pdf";
                                    //fileName = "NonConformite" + (docType == "Client" ? "Client" : "CDT") + "-" + nC.CommandeFournisseur.NumeroCommande + ".xlsx";
                                }
                            }
                        }
                        break;
                    case "NoteCredit":
                        if (fileFormat == "pdf")
                        {
                            var nC = DbContext.SAGEDocuments
                                .Where(e => e.RefSAGEDocument == fileRef)
                                .FirstOrDefault();
                            if (nC != null)
                            {
                                //Rights
                                if (
                                        (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationModuleCollectivite)
                                        && !(CurrentContext.ConnectedUtilisateur.HabilitationModuleCollectivite == Enumerations.HabilitationQualite.Utilisateur.ToString() && nC.CodeComptable != CurrentContext.ConnectedUtilisateur?.Collectivite?.SAGECodeComptable)
                                        )
                                        || (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationLogistique)
                                        && !(CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.CentreDeTri.ToString() && nC.CodeComptable != CurrentContext.ConnectedUtilisateur?.CentreDeTri?.SAGECodeComptable)
                                        )
                                    )
                                {
                                    //Process
                                    mS = PdfFileManagement.CreateNoteCredit(fileRef, DbContext, _env.ContentRootPath);
                                    fileName = "NoteCredit" + "-" + nC.RefSAGEDocument + ".pdf";
                                }
                            }
                        }
                        break;
                    case "Parametre":
                        mS = new MemoryStream(Utils.Utils.GetParametre(fR, DbContext).ValeurBinaire);
                        fileName = Utils.Utils.GetParametre(fR, DbContext).ValeurTexte;
                        break;
                }
                //Send file
                if (mS != null)
                {
                    //Headers
                    var cD = new System.Net.Mime.ContentDisposition
                    {
                        FileName = fileName,
                        //FileName = fileName.Replace(" ", "_"),
                        // always prompt the user for downloading
                        Inline = false,
                    };
                    string s=string.Format("attachment; filename=\"{0}\"", HttpUtility.UrlEncode(fileName));
                    Response.Headers.Remove("Content-Disposition");
                    Response.Headers.Append("Content-Disposition", s);
                    //End
                    return new FileContentResult(mS.ToArray(), "application/octet-stream");
                }
            }
            return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
        }
        #endregion
    }
}
