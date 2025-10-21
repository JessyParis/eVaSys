/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 09/01/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using Limilabs.Mail;
using Limilabs.Mail.MIME;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class UploadController : BaseApiController
    {
        #region Constructor
        public UploadController(ApplicationDbContext context, IConfiguration configuration)
            : base(context, configuration) { }
        #endregion Constructor

        #region Services
        /// <summary>
        /// Get uploaded Transport prices file(s)
        /// </summary>
        [HttpPost("Transport")]
        public IActionResult PostTransport(IFormFile file)
        {
            //Chech authorization
            if (!Rights.AuthorizedMenu(Enumerations.MenuName.LogistiqueMenuImporterTransport.ToString(), CurrentContext, Configuration))
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            long size = file.Length;
            //var stream = new FileStream("c:\\enviromatic\\temp\\test.xlsx", FileMode.Create);
            var stream = new MemoryStream();
            file.CopyTo(stream);
            //Process Excel file
            string error = "";
            string message = Utils.ExcelFileManagement.ImportTransport(stream, ref error, CurrentContext, DbContext);
            //End
            return Ok(new { error, message, file.FileName });
        }
        /// <summary>
        /// Get uploaded CommandeFournisseur file(s)
        /// </summary>
        [HttpPost("CommandeFournisseur")]
        public async Task<IActionResult> PostCommandeFournisseur(IFormFile file)
        {
            //Chech authorization
            if (!Rights.AuthorizedMenu(Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString(), CurrentContext, Configuration)
                && !Rights.AuthorizedMenu(Enumerations.MenuName.LogistiqueMenuTransportCommande.ToString(), CurrentContext, Configuration)
                && !Rights.AuthorizedMenu(Enumerations.MenuName.LogistiqueMenuTransportCommandeEnCours.ToString(), CurrentContext, Configuration)
                && !Rights.AuthorizedMenu(Enumerations.MenuName.LogistiqueMenuTransportCommandeModifiee.ToString(), CurrentContext, Configuration)
                )
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            //Check parameters
            string r = Request.Headers["ref"].ToString();
            long size = file.Length;
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            //Process file
            int rCmdF = 0;
            int.TryParse(r, out rCmdF);
            string error = "";
            string message = CurrentContext.CulturedRessources.GetTextRessource(628);
            if (rCmdF == 0)
            {
                error = CurrentContext.CulturedRessources.GetTextRessource(617);
            }
            else
            {
                CommandeFournisseur cmdF = DbContext.CommandeFournisseurs
                    .Include(r => r.CommandeFournisseurFichiers)
                    .Where(el => el.RefCommandeFournisseur == rCmdF).FirstOrDefault();
                if (cmdF == null)
                {
                    error = CurrentContext.CulturedRessources.GetTextRessource(617);
                }
                else
                {
                    //Save data
                    CommandeFournisseurFichier cmdFF = new();
                    //Generate resized images if applicable
                    if (Utils.ImageUtils.IsValidImage(stream))
                    {
                        var destImage = new MemoryStream();
                        using (Image img = Image.FromStream(stream))
                        {
                            //Check image orientation
                            bool portrait = img.Height >= img.Width;
                            //Original
                            if ((img.Width > 1200 && !portrait) || (img.Height > 1200 && portrait))
                            {
                                destImage = new MemoryStream();
                                Utils.ImageUtils.ResizeImage(img, portrait ? 0 : 1200, portrait ? 1200 : 0).Save(destImage, img.RawFormat);
                                cmdFF.Corps = destImage.ToArray();
                            }
                            else
                            {
                                cmdFF.Corps = stream.ToArray();
                            }
                            //Vignette
                            if ((img.Width > 600 && !portrait) || (img.Height > 600 && portrait))
                            {
                                destImage = new MemoryStream();
                                Utils.ImageUtils.ResizeImage(img, portrait ? 0 : 600, portrait ? 600 : 0).Save(destImage, img.RawFormat);
                                cmdFF.Vignette = destImage.ToArray();
                            }
                            else
                            {
                                cmdFF.Vignette = stream.ToArray();
                            }
                            //Miniature
                            if ((img.Width > 100 && !portrait) || (img.Height > 100 && portrait))
                            {
                                destImage = new MemoryStream();
                                Utils.ImageUtils.ResizeImage(img, portrait ? 0 : 100, portrait ? 100 : 0).Save(destImage, img.RawFormat);
                                cmdFF.Miniature = destImage.ToArray();
                            }
                            else
                            {
                                cmdFF.Miniature = stream.ToArray();
                            }
                        }
                    }
                    else
                    {
                        cmdFF.Corps = stream.ToArray();
                    }
                    cmdFF.Nom = file.FileName;
                    DbContext.CommandeFournisseurFichiers.Add(cmdFF);
                    cmdF.CommandeFournisseurFichiers.Add(cmdFF);
                    DbContext.SaveChanges();
                }
            }
            //End
            return Ok(new { error, message, file.FileName });
        }
        /// <summary>
        /// Get uploaded Email file(s)
        /// </summary>
        [HttpPost("Email")]
        public async Task<IActionResult> PostEmail(IFormFile file)
        {
            //Chech authorization
            if (!Rights.AuthorizedMenu(Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString(), CurrentContext, Configuration)
                )
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            //Check parameters
            string r = Request.Headers["ref"].ToString();
            long size = file.Length;
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            //Process file
            int rE = 0;
            int.TryParse(r, out rE);
            string error = "";
            string message = CurrentContext.CulturedRessources.GetTextRessource(628);
            if (rE == 0)
            {
                error = CurrentContext.CulturedRessources.GetTextRessource(617);
            }
            else
            {
                Email e = DbContext.Emails
                    .Where(el => el.RefEmail == rE).FirstOrDefault();
                if (e == null)
                {
                    error = CurrentContext.CulturedRessources.GetTextRessource(617);
                }
                else
                {
                    var msg = e.Message;
                    MailBuilder mB = msg.ToBuilder();
                    MimeData att1 = mB.AddAttachment(stream.ToArray());
                    att1.FileName = file.FileName;
                    msg = mB.Create();
                    e.Message = msg;
                    //Save data
                    DbContext.SaveChanges();
                }
            }
            //End
            return Ok(new { error, message, file.FileName });
        }
        /// <summary>
        /// Get uploaded NonConformite file(s)
        /// </summary>
        [HttpPost("NonConformite")]
        public async Task<IActionResult> PostNonConformite(IFormFile file)
        {
            //Chech authorization
            if (!Rights.AuthorizedMenu(Enumerations.MenuName.QualiteMenuNonConformite.ToString(), CurrentContext, Configuration)
                )
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            //Check parameters
            string r = Request.Headers["ref"].ToString();
            string fileType = Request.Headers["fileType"].ToString();
            long size = file.Length;
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            //Process file
            int rNC = 0;
            int fT = 0;
            int.TryParse(r, out rNC);
            int.TryParse(fileType, out fT);
            string error = "";
            string message = CurrentContext.CulturedRessources.GetTextRessource(628);
            if (rNC == 0)
            {
                error = CurrentContext.CulturedRessources.GetTextRessource(617);
            }
            else
            {
                var nC = DbContext.NonConformites
                    .Include(r => r.NonConformiteEtapes)
                    .Include(r => r.NonConformiteFichiers)
                    .Where(el => el.RefNonConformite == rNC).FirstOrDefault();
                if (nC == null)
                {
                    error = CurrentContext.CulturedRessources.GetTextRessource(617);
                }
                else
                {
                    //Save data
                    var nCF = new NonConformiteFichier();
                    //Generate resized images if applicable
                    if (Utils.ImageUtils.IsValidImage(stream))
                    {
                        var destImage = new MemoryStream();
                        using (Image img = Image.FromStream(stream))
                        {
                            //Check image orientation
                            bool portrait = img.Height >= img.Width;
                            //Original
                            if ((img.Width > 1200 && !portrait) || (img.Height > 1200 && portrait))
                            {
                                destImage = new MemoryStream();
                                Utils.ImageUtils.ResizeImage(img, portrait ? 0 : 1200, portrait ? 1200 : 0).Save(destImage, img.RawFormat);
                                nCF.Corps = destImage.ToArray();
                            }
                            else
                            {
                                nCF.Corps = stream.ToArray();
                            }
                            //Vignette
                            if ((img.Width > 600 && !portrait) || (img.Height > 600 && portrait))
                            {
                                destImage = new MemoryStream();
                                Utils.ImageUtils.ResizeImage(img, portrait ? 0 : 600, portrait ? 600 : 0).Save(destImage, img.RawFormat);
                                nCF.Vignette = destImage.ToArray();
                            }
                            else
                            {
                                nCF.Vignette = stream.ToArray();
                            }
                            //Miniature
                            if ((img.Width > 100 && !portrait) || (img.Height > 100 && portrait))
                            {
                                destImage = new MemoryStream();
                                Utils.ImageUtils.ResizeImage(img, portrait ? 0 : 100, portrait ? 100 : 0).Save(destImage, img.RawFormat);
                                nCF.Miniature = destImage.ToArray();
                            }
                            else
                            {
                                nCF.Miniature = stream.ToArray();
                            }
                        }
                    }
                    else
                    {
                        nCF.Corps = stream.ToArray();
                    }
                    nCF.Nom = file.FileName;
                    nCF.RefNonConformiteFichierType = fT;
                    DbContext.NonConformiteFichiers.Add(nCF);
                    nC.NonConformiteFichiers.Add(nCF);
                    //Etape management
                    NonConformiteEtape etp = null;
                    switch (fT)
                    {
                        case 1:
                            etp = nC.NonConformiteEtapes.Where(e => e.RefNonConformiteEtapeType == 1).FirstOrDefault();
                            break;
                        case 2:
                            etp = nC.NonConformiteEtapes.Where(e => e.RefNonConformiteEtapeType == 3).FirstOrDefault();
                            break;
                        case 3:
                            etp = nC.NonConformiteEtapes.Where(e => e.RefNonConformiteEtapeType == 6).FirstOrDefault();
                            break;
                        case 4:
                            etp = nC.NonConformiteEtapes.Where(e => e.RefNonConformiteEtapeType == 8).FirstOrDefault();
                            break;
                    }
                    etp.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                    etp.MarkModification();
                    //Save
                    DbContext.SaveChanges();
                }
            }
            //End
            return Ok(new { error, message, file.FileName });
        }
        /// <summary>
        /// Get uploaded ActionFichier file(s)
        /// </summary>
        [HttpPost("ActionFichier")]
        public async Task<IActionResult> PostActionFichier(IFormFile file)
        {
            //Chech authorization
            if (!Rights.AuthorizedMenu(Enumerations.MenuName.AnnuaireMenuEntite.ToString(), CurrentContext, Configuration)
                )
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            //Check parameters
            string r = Request.Headers["ref"].ToString();
            string fileType = Request.Headers["fileType"].ToString();
            int.TryParse(fileType, out int fT);
            long size = file.Length;
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            //Process file
            int.TryParse(r, out int refContainer);
            string error = "";
            string message = CurrentContext.CulturedRessources.GetTextRessource(628);
            int fileDbId = 0;
            if (refContainer > 0)
            {
                var container = DbContext.Actions
                    .Include(r => r.ActionFichiers)
                    .Where(el => el.RefAction == refContainer).FirstOrDefault();
                if (container == null)
                {
                    error = CurrentContext.CulturedRessources.GetTextRessource(617);
                }
                else
                {
                    refContainer = container.RefAction;
                }
            }
            //Save data if no error
            if (error == "")
            {
                ActionFichier actionFichier = new()
                {
                    RefAction = refContainer,
                    Corps = stream.ToArray(),
                    Nom = file.FileName,
                };
                DbContext.ActionFichiers.Add(actionFichier);
                DbContext.SaveChanges();
                DbContext.Entry(actionFichier).Reload();
                fileDbId = actionFichier.RefActionFichier;
            }
            //End
            return Ok(new { error, message, file.FileName, fileDbId });
        }
        /// <summary>
        /// Get uploaded DocumentEntite file(s)
        /// </summary>

        [HttpPost("DocumentEntite")]
        public async Task<IActionResult> PostDocumentEntite(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(631)));
            }
            //Chech authorization
            if (!Rights.AuthorizedMenu(Enumerations.MenuName.AnnuaireMenuEntite.ToString(), CurrentContext, Configuration)
                )
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            //Check parameters
            string r = Request.Headers["ref"].ToString();
            string fileType = Request.Headers["fileType"].ToString();
            int.TryParse(fileType, out int fT);
            long size = file.Length;
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            //Process file
            int.TryParse(r, out int refEntite);
            string error = "";
            string message = CurrentContext.CulturedRessources.GetTextRessource(628);
            int fileDbId = 0;
            if (refEntite > 0)
            {
                Entite entite = DbContext.Entites
                    .Where(el => el.RefEntite == refEntite).FirstOrDefault();
                if (entite == null)
                {
                    error = CurrentContext.CulturedRessources.GetTextRessource(617);
                }
                else
                {
                    refEntite = entite.RefEntite;
                }
            }
            //Save data if no error
            if (error == "")
            {
                //Save data
                Document document = new()
                {
                    Corps = stream.ToArray(),
                    Nom = file.FileName,
                    Libelle = file.FileName,
                    Actif = true,
                    VisibiliteTotale = (fT == 1),
                    RefUtilisateurCourant = CurrentContext.RefUtilisateur,
                    DocumentEntites = []
                };
                DbContext.Documents.Add(document);
                DocumentEntite dE = new()
                {
                    RefEntite = refEntite,
                };
                document.DocumentEntites.Add(dE);
                DbContext.DocumentEntites.Add(dE);
                DbContext.SaveChanges();
                DbContext.Entry(dE).Reload();
                fileDbId = dE.RefDocumentEntite;
            }
            //End
            return Ok(new { error, message, fileName = file.FileName, fileDbId });
        }
        /// <summary>
        /// Get uploaded DocumentEntite file(s)
        /// </summary>
        [HttpPost("Document")]
        public async Task<IActionResult> PostDocument(IFormFile file)
        {
            //Chech authorization
            if (!Rights.AuthorizedMenu(Enumerations.MenuName.AdministrationMenuDocument.ToString(), CurrentContext, Configuration)
                )
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            //Check parameters
            string r = Request.Headers["ref"].ToString();
            string fileType = Request.Headers["fileType"].ToString();
            long size = file.Length;
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            //Process file
            int.TryParse(r, out int refDocument);
            string error = "";
            string message = CurrentContext.CulturedRessources.GetTextRessource(628);
            int fileDbId = 0;
            if (refDocument > 0)
            {
                Document doc = DbContext.Documents
                    .Where(el => el.RefDocument == refDocument).FirstOrDefault();
                if (doc == null)
                {
                    error = CurrentContext.CulturedRessources.GetTextRessource(617);
                }
                else
                {
                    doc.Corps = stream.ToArray();
                    doc.Nom = file.FileName;
                    doc.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                    DbContext.SaveChanges();
                    fileDbId = doc.RefDocument;
                }
            }
            else
            {
                error = CurrentContext.CulturedRessources.GetTextRessource(617);
            }
            //End
            return Ok(new { error, message, file.FileName, fileDbId });
        }
        /// <summary>
        /// Get uploaded Document file(s)
        /// </summary>
        [HttpPost("EquivalentCO2")]
        public async Task<IActionResult> PostEquivalentCO2(IFormFile file)
        {
            //Chech authorization
            if (!Rights.AuthorizedMenu(Enumerations.MenuName.AdministrationMenuEquivalentCO2.ToString(), CurrentContext, Configuration)
                )
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            //Check parameters
            string r = Request.Headers["ref"].ToString();
            long size = file.Length;
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            //Process file
            int.TryParse(r, out int refEquivalentCO2);
            string error = "";
            string message = CurrentContext.CulturedRessources.GetTextRessource(628);
            int fileDbId = 0;
            if (refEquivalentCO2 > 0)
            {
                var equivalentCO2 = DbContext.EquivalentCO2s
                    .Where(el => el.RefEquivalentCO2 == refEquivalentCO2).FirstOrDefault();
                if (equivalentCO2 == null)
                {
                    error = CurrentContext.CulturedRessources.GetTextRessource(617);
                }
                else
                {
                    equivalentCO2.Icone = stream.ToArray();
                    DbContext.SaveChanges();
                    DbContext.Entry(equivalentCO2).Reload();
                    fileDbId = equivalentCO2.RefEquivalentCO2;
                }
            }
            //End
            return Ok(new { error, message, file.FileName, fileDbId });
        }
        /// <summary>
        /// Get uploaded Document file(s)
        /// </summary>
        [HttpPost("parametre")]
        public async Task<IActionResult> PostParametre(IFormFile file)
        {
            //Chech authorization
            if (!Rights.AuthorizedMenu(Enumerations.MenuName.AdministrationMenuParametre.ToString(), CurrentContext, Configuration)
                )
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            //Check parameters
            string r = Request.Headers["ref"].ToString();
            long size = file.Length;
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            //Process file
            int.TryParse(r, out int refParametre);
            string error = "";
            string message = CurrentContext.CulturedRessources.GetTextRessource(628);
            int fileDbId = 0;
            if (refParametre > 0)
            {
                var parametre = DbContext.Parametres
                    .Where(el => el.RefParametre == refParametre).FirstOrDefault();
                if (parametre == null)
                {
                    error = CurrentContext.CulturedRessources.GetTextRessource(617);
                }
                else
                {
                    parametre.ValeurBinaire = stream.ToArray();
                    parametre.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                    parametre.MarkModification(false);
                    DbContext.SaveChanges();
                    DbContext.Entry(parametre).Reload();
                    fileDbId = parametre.RefParametre;
                }
            }
            //End
            return Ok(new { error, message, file.FileName, fileDbId });
        }
        #endregion
    }
}
