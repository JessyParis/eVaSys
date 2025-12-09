/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/06/2022
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using static eVaSys.Utils.Enumerations;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class APICommandeFournisseurController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public APICommandeFournisseurController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper, IWebHostEnvironment env)
            : base(context, configuration)
        {
            _mapper = mapper;
            _env = env;
        }
        protected IWebHostEnvironment _env;
        #endregion Constructor

        #region RESTful Conventions

        /// <summary>
        /// Get the Laser transaction (CommandeFournisseur) with the given {id}
        /// </summary>
        /// <param name="model">The CommandeFournisseurViewModel containing the data to update</param>
        [HttpPost("PostRefExt")]
        public IActionResult PostRefExt([FromBody] APICommandeFournisseurRefExtViewModel model)
        {
            string path = HttpContext.Request.Path;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(path, null, (int?)HttpStatusCode.BadRequest, CurrentContext.CulturedRessources.GetTextRessource(711), "Contenu non reconnu", CurrentContext.RefUtilisateur, DbContext);
                //End
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }

            //---------------------------------------------------------------------------------------------------
            //Check validity of payload
            string jsonBody = JsonConvert.SerializeObject(model);
            //Check validity
            string err = "";
            if (string.IsNullOrWhiteSpace(model.RefExt) || model.RefExt?.Length > 50)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " RefExt";
            }
            if (string.IsNullOrWhiteSpace(model.ActionCode) || model.ActionCode?.Length > 50
                || !Enum.TryParse<Enumerations.APICommandeFournisseurActionCode>(model.ActionCode, out var _enum))
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " ActionCode";
            }
            //---------------------------------------------------------------------------------------------------
            //Exit on error
            if (err != "")
            {
                err = CurrentContext.CulturedRessources.GetTextRessource(319) + err;
                //Log
                APIUtils.ApiUtils.LogAPICall(path, jsonBody, (int?)HttpStatusCode.BadRequest, null, err, CurrentContext.RefUtilisateur, DbContext);
                //End
                return BadRequest(new BadRequestError(err));
            }

            //---------------------------------------------------------------------------------------------------
            //Get Laser transaction information
            LaserTransaction laserTransaction = new LaserTransaction();
            bool r = LaserAPIUtils.CommandeFournisseurGetData(model.RefExt, ref laserTransaction, CurrentContext.RefUtilisateur, DbContext, Configuration);
            //---------------------------------------------------------------------------------------------------
            //Exit on error
            if (!r)
            {
                err = Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1292);
                APIUtils.ApiUtils.LogAPICall(path, jsonBody, (int?)HttpStatusCode.BadRequest, null, err, CurrentContext.RefUtilisateur, DbContext);
                //End
                return BadRequest(new BadRequestError(err));
            }

            //---------------------------------------------------------------------------------------------------
            //Process if no error
            CommandeFournisseur commandeFournisseur = null;
            //Retrieve CommandeFournisseur
            //Check if the commandeFournisseur already exists
            commandeFournisseur = DbContext.CommandeFournisseurs
                .Where(q => q.RefExt == model.RefExt)
                .FirstOrDefault();
            if (commandeFournisseur != null)
            {
                //Return error if rejected or canceled
                if (commandeFournisseur.ChargementAnnule == true
                    || commandeFournisseur.RefusCamion == true)
                {
                    err = Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1259);
                }
                //Return error if already exists and "Creation" asked
                else if (model.ActionCode == Enumerations.APICommandeFournisseurActionCode.Creation.ToString())
                {
                    err = Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1290);
                }
                //Return error if already processed, except for documentation rejection
                else if (model.ActionCode != Enumerations.APICommandeFournisseurActionCode.RefusDocumentationRecycleur.ToString()
                    && model.ActionCode != Enumerations.APICommandeFournisseurActionCode.RefusDocumentationTransporteur.ToString()
                    && (commandeFournisseur.DDechargement != null
                        || commandeFournisseur.PoidsDechargement > 0
                        || commandeFournisseur.NbBalleDechargement > 0
                        || commandeFournisseur.ChargementEffectue == true
                        || commandeFournisseur.ExportSAGE == true))
                {
                    err = Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1258);
                }
            }
            else
            {
                //Get CentreDeTri
                Entite cDT = DbContext.Entites
                    .Include(e => e.EntiteProduits)
                    .Where(e => e.CodeEE == laserTransaction.sourceActor.code).FirstOrDefault();
                //Get Produit: quality or flow
                int? refP = null;
                if (!string.IsNullOrWhiteSpace(laserTransaction.quality?.code) && !string.IsNullOrWhiteSpace(laserTransaction.quality?.name))
                {
                    refP = DbContext.Produits
                        .Where(e => e.Libelle == (laserTransaction.quality.code + " - " + laserTransaction.quality.name)
                        && e.LaserType == LaserType.Quality.ToString()
                    ).FirstOrDefault()?.RefProduit;
                }
                if (!string.IsNullOrWhiteSpace(laserTransaction.flow?.type) && !string.IsNullOrWhiteSpace(laserTransaction.flow?.name))
                {
                    refP = DbContext.Produits
                        .Where(e => e.Libelle == (laserTransaction.flow.type + " - " + laserTransaction.flow.name)
                        && e.LaserType == LaserType.Flow.ToString()
                        ).FirstOrDefault()?.RefProduit;
                }
                //Return error if no exists and other than "Creation" asked
                if (model.ActionCode != Enumerations.APICommandeFournisseurActionCode.Creation.ToString())
                {
                    err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1291);
                }
                //Return error if CDT not found
                if (cDT == null)
                {
                    err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1597)
                        + Environment.NewLine + " - sourceActor.code=" + laserTransaction.sourceActor.code;
                }
                //Return error if Produit not found
                else if (refP == null)
                {
                    err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(358)
                        + Environment.NewLine + " - quality.code=" + laserTransaction.quality?.code
                        + Environment.NewLine + " - quality.name=" + laserTransaction.quality?.name
                        + Environment.NewLine + " - flow.type=" + laserTransaction.flow?.type
                        + Environment.NewLine + " - flow.name=" + laserTransaction.flow?.name;
                }
                //Return error if businessId not found
                else if (string.IsNullOrWhiteSpace(laserTransaction.businessId))
                {
                    err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1601);
                }
                //Check if Produit is allowed for CentreDeTri
                else if (cDT != null && refP != null)
                {
                    if (cDT.EntiteProduits.Where(e => e.RefProduit == refP && e.Interdit == true).Any())
                    {
                        err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1228);
                    }
                }
            }
            //---------------------------------------------------------------------------------------------------
            //Exit on error
            if (err != "")
            {
                //Create message if rejcted for forbidden Produit
                //Create message
                string corps = "", corpsHTML = " ", titre = "Demande d'enlèvement rejetée";
                if (err == Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1228))
                {
                    titre = "Demande d'enlèvement rejetée pour produit interdit";
                    corps = "La demande d'enlèvement " + laserTransaction.businessId + " (" + laserTransaction.id.ToString() + ") a été rejetée car elle concerne un produit interdit pour le Centre de Tri.";
                    corps += Environment.NewLine + "Centre de Tri : " + laserTransaction.sourceActor?.name;
                    corps += Environment.NewLine + "Produit (quality) : " + laserTransaction.quality?.code + " - " + laserTransaction.quality?.name;
                    corps += Environment.NewLine + "Produit (flow) : " + laserTransaction.flow?.type + " - " + laserTransaction.flow?.name;
                    corps += Environment.NewLine + "Quantité (kg) : " + laserTransaction.forecastedQuantity.ToString();
                    corps += Environment.NewLine + "Date de disponibilité : " + laserTransaction.sourceAvailabilityDate;
                }
                else
                {
                    titre = "Demande d'enlèvement rejetée";
                    corps = "La demande d'enlèvement " + laserTransaction.businessId + " (" + laserTransaction.id.ToString() + ") a été rejetée pour la raison suivante.";
                    corps += Environment.NewLine + err;
                }
                corps += Environment.NewLine + Environment.NewLine + "Le " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");
                corpsHTML = Utils.Utils.FormatHTMLNewLines(System.Net.WebUtility.HtmlEncode(corps));
                corpsHTML = "<span style=\"font-family:Roboto; font-size:11pt;\">" + corpsHTML + "</span>";
                var msg = new Message()
                {
                    RefUtilisateurCourant = CurrentContext.RefUtilisateur,
                    RefMessageType = 4,
                    Libelle = titre,
                    Titre = titre,
                    Corps = corps,
                    CorpsHTML = corpsHTML,
                    DiffusionUnique = true,
                    VisualisationConfirmeUnique = true,
                    DDebut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    DFin = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).AddDays(7),
                    Actif = true,
                    Important = true
                };
                DbContext.Messages.Add(msg);
                //Manage Diffusion
                msg.MessageDiffusions = new HashSet<MessageDiffusion>();
                msg.MessageDiffusions.Add(new MessageDiffusion()
                {
                    RefModule = Enumerations.Module.Logistique.ToString(),
                    RefHabilitation = Enumerations.HabilitationLogistique.Administrateur.ToString()
                });
                DbContext.SaveChanges();
                //Log error
                err = CurrentContext.CulturedRessources.GetTextRessource(319) + err;
                //Log
                APIUtils.ApiUtils.LogAPICall(path, jsonBody, (int?)HttpStatusCode.BadRequest, null, err, CurrentContext.RefUtilisateur, DbContext);
                //End
                return BadRequest(new BadRequestError(err));
            }

            //---------------------------------------------------------------------------------------------------
            //Save data if no error
            if (commandeFournisseur == null)
            {
                commandeFournisseur = new CommandeFournisseur();
                DbContext.CommandeFournisseurs.Add(commandeFournisseur);
            }
            commandeFournisseur.RefExt = model.RefExt;
            //Creating viewModel for update
            var viewModel = new APICommandeFournisseurViewModel();
            viewModel.RefExt = laserTransaction.id.ToString();
            viewModel.LibExt = laserTransaction.businessId;
            viewModel.RefPrestataire = CurrentContext.ConnectedUtilisateur.Prestataire?.RefEntite;
            viewModel.DDisponibilite = laserTransaction.sourceAvailabilityDate;
            viewModel.FournisseurLibelle = laserTransaction.sourceActor.name;
            viewModel.FournisseurCodeCITEO = laserTransaction.sourceActor.code;
            //Laser flow or quality
            if (!string.IsNullOrWhiteSpace(laserTransaction.quality?.code) && !string.IsNullOrWhiteSpace(laserTransaction.quality?.name))
            { viewModel.ProduitLibelle = laserTransaction.quality.code + " - " + laserTransaction.quality.name; }
            if (!string.IsNullOrWhiteSpace(laserTransaction.flow?.type) && !string.IsNullOrWhiteSpace(laserTransaction.flow?.name))
            { viewModel.ProduitLibelle = laserTransaction.flow.type + " - " + laserTransaction.flow.name; }
            viewModel.PoidsChargement = laserTransaction.forecastedQuantity;
            viewModel.NbBalleChargement = laserTransaction.forecastedBalesAmount;
            viewModel.ActionCode = model.ActionCode;
            viewModel.Adr1 = laserTransaction.sourceActor.addressLine;
            viewModel.CodePostal = laserTransaction.sourceActor.postalCode;
            viewModel.Ville = laserTransaction.sourceActor.city;
            viewModel.Prenom = laserTransaction.sourceActor.operationalContact?.firstname;
            viewModel.Nom = laserTransaction.sourceActor.operationalContact?.lastname;
            viewModel.Tel = laserTransaction.sourceActor.operationalContact?.phone;
            viewModel.Email = laserTransaction.sourceActor.operationalContact?.email;
            viewModel.carrierRejectedLastComment = laserTransaction.carrierRejectedLastComment;
            viewModel.receiptRejectedLastComment = laserTransaction.receiptRejectedLastComment;
            //Update data
            UpdateData(ref commandeFournisseur, viewModel);

            //Register session user
            commandeFournisseur.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = commandeFournisseur.IsValid();
            //string valid = "";
            //End
            if (string.IsNullOrEmpty(valid))
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Log
                APIUtils.ApiUtils.LogAPICall(path, jsonBody, (int?)HttpStatusCode.OK, null, "", CurrentContext.RefUtilisateur, DbContext);
                //End
                return new OkResult();
            }
            else
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(path, jsonBody, (int?)HttpStatusCode.Conflict, null, valid, CurrentContext.RefUtilisateur, DbContext);
                //End
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Edit the CommandeFournisseur with the given {id}
        /// </summary>
        /// <param name="model">The CommandeFournisseurViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] APICommandeFournisseurViewModel model)
        {
            string path = HttpContext.Request.Path;
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(path, null, (int?)HttpStatusCode.BadRequest, CurrentContext.CulturedRessources.GetTextRessource(711), "Contenu non reconnu", CurrentContext.RefUtilisateur, DbContext);
                //End
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
            string jsonBody = JsonConvert.SerializeObject(model);
            //Check validity
            string err = "";
            if (string.IsNullOrWhiteSpace(model.RefExt) || model.RefExt?.Length > 50)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " RefExt";
            }
            if (string.IsNullOrWhiteSpace(model.LibExt) || model.LibExt?.Length > 50)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " LibExt";
            }
            if (string.IsNullOrWhiteSpace(model.FournisseurCodeCITEO) || model.FournisseurCodeCITEO.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " FournisseurCodeCITEO";
            }
            if (model.FournisseurLibelle.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " FournisseurLibelle";
            }
            if (string.IsNullOrWhiteSpace(model.ProduitLibelle) || model.ProduitLibelle.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " ProduitLibelle";
            }
            if (model.PoidsChargement <= 0 || model.PoidsChargement > 200000)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " PoidsChargement";
            }
            if (model.NbBalleChargement <= 0 || model.NbBalleChargement > 600)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " NbBalleChargement";
            }
            if (!string.IsNullOrWhiteSpace(model.Adr1) && model.Adr1?.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " Adr1";
            }
            if (!string.IsNullOrWhiteSpace(model.Adr2) && model.Adr2?.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " Adr2";
            }
            if (!string.IsNullOrWhiteSpace(model.CodePostal) && model.CodePostal?.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " CodePostal";
            }
            if (!string.IsNullOrWhiteSpace(model.Ville) && model.Ville?.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " Ville";
            }
            if (!string.IsNullOrWhiteSpace(model.Civilite) && model.Civilite?.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " Civilite";
            }
            if (!string.IsNullOrWhiteSpace(model.Prenom) && model.Prenom?.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " Prenom";
            }
            if (!string.IsNullOrWhiteSpace(model.Nom) && model.Nom?.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " Nom";
            }
            if (!string.IsNullOrWhiteSpace(model.Tel) && model.Tel?.Length > 100)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " Tel";
            }
            if (!string.IsNullOrWhiteSpace(model.Email) && model.Email?.Length > 200)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " Email";
            }
            if (!string.IsNullOrWhiteSpace(model.Cmt) && model.Cmt?.Length > 5000)
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " Cmt";
            }
            if (!string.IsNullOrWhiteSpace(model.ActionCode)
                && (
                    model.ActionCode != Enumerations.APICommandeFournisseurActionCode.RefusDocumentationRecycleur.ToString()
                    && model.ActionCode != Enumerations.APICommandeFournisseurActionCode.RefusDocumentationTransporteur.ToString()
                    )
                )
            {
                err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " ActionCode";
            }

            //Retrieve the commandeFournisseur to edit and check dates
            var commandeFournisseur = DbContext.CommandeFournisseurs
                .Include(r => r.CommandeFournisseurFichiers)
                .Where(q => q.RefExt == model.RefExt)
                .FirstOrDefault();
            if (commandeFournisseur == null)
            {
                //Date check
                if (model.DDisponibilite == null)
                {
                    err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1257) + " DDisponibilite";
                }
                //Documentation reject check
                else if (!string.IsNullOrWhiteSpace(model.ActionCode))
                {
                    err += Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1273);
                }
                else
                {
                    commandeFournisseur = new CommandeFournisseur();
                    DbContext.CommandeFournisseurs.Add(commandeFournisseur);
                }
            }
            else
            {
                //Return error if already processed, except for documentation rejection
                if (string.IsNullOrWhiteSpace(model.ActionCode))
                {
                    if (commandeFournisseur.ChargementAnnule == true
                        || commandeFournisseur.RefusCamion == true)
                    {
                        err = Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1259);
                    }
                    else if (commandeFournisseur.DDechargement != null
                        || commandeFournisseur.PoidsDechargement > 0
                        || commandeFournisseur.NbBalleDechargement > 0
                        || commandeFournisseur.ChargementEffectue == true
                        || commandeFournisseur.ExportSAGE == true)
                    {
                        err = Environment.NewLine + CurrentContext.CulturedRessources.GetTextRessource(1258);
                    }
                }
            }

            if (err != "")
            {
                err = CurrentContext.CulturedRessources.GetTextRessource(711) + err;
                //Log
                APIUtils.ApiUtils.LogAPICall(path, jsonBody, (int?)HttpStatusCode.BadRequest, null, err, CurrentContext.RefUtilisateur, DbContext);
                //End
                return BadRequest(new BadRequestError(err));
            }

            //Set values
            UpdateData(ref commandeFournisseur, model);

            //Register session user
            commandeFournisseur.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = commandeFournisseur.IsValid();
            //End
            if (string.IsNullOrEmpty(valid))
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Log
                APIUtils.ApiUtils.LogAPICall(path, jsonBody, (int?)HttpStatusCode.OK, null, "", CurrentContext.RefUtilisateur, DbContext);
                //End
                return new OkResult();
            }
            else
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(path, jsonBody, (int?)HttpStatusCode.Conflict, null, valid, CurrentContext.RefUtilisateur, DbContext);
                //End
                return Conflict(new ConflictError(valid));
            }
        }
        #endregion

        #region Services
        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref CommandeFournisseur dataModel, APICommandeFournisseurViewModel viewModel)
        {
            //Create new
            if (dataModel.RefCommandeFournisseur <= 0)
            {
                //Process
                dataModel.NumeroCommande = 0;
                dataModel.NumeroAffretement = 0;
                dataModel.RefExt = viewModel.RefExt;
                dataModel.LibExt = viewModel.LibExt;
                dataModel.RefPrestataire = viewModel.RefPrestataire;
                //Search for Entite
                int? refEntite = DbContext.Entites.Where(e => e.CodeEE == viewModel.FournisseurCodeCITEO).FirstOrDefault()?.RefEntite;
                if (refEntite == null && !string.IsNullOrWhiteSpace(viewModel.FournisseurLibelle))
                {
                    refEntite = DbContext.Entites.Where(e => e.Libelle == viewModel.FournisseurLibelle).FirstOrDefault()?.RefEntite;
                }
                dataModel.RefEntite = refEntite ?? 0;
                //Get adresse if applicable
                if (refEntite != null)
                {
                    var cA = DbContext.ContactAdresses
                        .Include(r => r.ContactAdresseContactAdresseProcesss)
                        .Include(r => r.Adresse)
                        .Where(el => el.RefEntite == refEntite && el.Actif == true
                            && el.ContactAdresseContactAdresseProcesss.Any(related => related.RefContactAdresseProcess == (int)Enumerations.ContactAdresseProcess.BonDeLivraison)).FirstOrDefault();
                    if (cA != null)
                    {
                        dataModel.RefContactAdresse = cA.RefContactAdresse;
                        dataModel.RefCivilite = cA.Contact.Civilite?.RefCivilite;
                        dataModel.Prenom = cA.Contact.Prenom;
                        dataModel.Nom = cA.Contact.Nom;
                        dataModel.Tel = cA.Tel;
                        dataModel.TelMobile = cA.TelMobile;
                        dataModel.Fax = cA.Fax;
                        dataModel.Email = cA.Email;
                        dataModel.RefAdresse = cA.RefAdresse;
                        dataModel.Libelle = cA.Adresse.Libelle;
                        dataModel.Adr1 = cA.Adresse.Adr1;
                        dataModel.Adr2 = cA.Adresse.Adr2;
                        dataModel.CodePostal = cA.Adresse.CodePostal;
                        dataModel.Ville = cA.Adresse.Ville;
                        dataModel.RefPays = cA.Adresse?.RefPays;
                        dataModel.Horaires = cA.Adresse?.Horaires;
                    }
                }
                //Get produit if applicable
                int? refProduit = DbContext.Produits.Where(e => e.Libelle == viewModel.ProduitLibelle
                && e.EntiteProduits.Any(r => r.RefEntite == (int)refEntite)).FirstOrDefault()?.RefProduit;
                dataModel.RefProduit = refProduit ?? 0;
                dataModel.D = viewModel.DDisponibilite;
                dataModel.PoidsChargement = viewModel.PoidsChargement;
                dataModel.NbBalleChargement = viewModel.NbBalleChargement;
                dataModel.DMoisDechargementPrevu = new DateTime(((DateTime)viewModel.DDisponibilite).Year, ((DateTime)viewModel.DDisponibilite).Month, 1, 0, 0, 0);
                //Create Cmt
                dataModel.CmtFournisseur += Environment.NewLine
                    + "*** Demande d'enlèvement " + CurrentContext.ConnectedUtilisateur.Nom + " ***"
                    + Environment.NewLine + "Réf. ext. : " + viewModel.LibExt
                    + Environment.NewLine + "Id ext : " + viewModel.RefExt;
                dataModel.CmtFournisseur += Environment.NewLine
                    + viewModel.FournisseurCodeCITEO.Trim() + " - " + viewModel.FournisseurLibelle.Trim();
                dataModel.CmtFournisseur += Environment.NewLine + "Dispo. : ";
                if (viewModel.DDisponibilite != null)
                {
                    dataModel.CmtFournisseur += ((DateTime)viewModel.DDisponibilite).ToString("dd/MM/yyyy");
                }
                dataModel.CmtFournisseur += Environment.NewLine + "Produit : " + viewModel.ProduitLibelle.Trim();
                dataModel.CmtFournisseur += Environment.NewLine + "Poids (kg) : " + viewModel.PoidsChargement;
                dataModel.CmtFournisseur += Environment.NewLine + "Nb. balles : " + viewModel.NbBalleChargement;
                if (!string.IsNullOrWhiteSpace(viewModel.Adr1) || !string.IsNullOrWhiteSpace(viewModel.Adr2)
                    || !string.IsNullOrWhiteSpace(viewModel.CodePostal) || !string.IsNullOrWhiteSpace(viewModel.Ville)
                    || !string.IsNullOrWhiteSpace(viewModel.Civilite) || !string.IsNullOrWhiteSpace(viewModel.Nom)
                    || !string.IsNullOrWhiteSpace(viewModel.Prenom) || !string.IsNullOrWhiteSpace(viewModel.Tel)
                    || !string.IsNullOrWhiteSpace(viewModel.Email))
                {
                    if (!string.IsNullOrWhiteSpace(viewModel.Adr1))
                    {
                        dataModel.CmtFournisseur += Environment.NewLine + viewModel.Adr1.Trim();
                    }
                    if (!string.IsNullOrWhiteSpace(viewModel.Adr2))
                    {
                        dataModel.CmtFournisseur += Environment.NewLine + viewModel.Adr2.Trim();
                    }
                    if (!string.IsNullOrWhiteSpace(viewModel.CodePostal) || !string.IsNullOrWhiteSpace(viewModel.Ville))
                    {
                        dataModel.CmtFournisseur += Environment.NewLine + (viewModel.CodePostal.Trim() ?? "-") + " " + (viewModel.Ville.Trim() ?? "-");
                    }
                    if (!string.IsNullOrWhiteSpace(viewModel.Prenom) || !string.IsNullOrWhiteSpace(viewModel.Nom))
                    {
                        dataModel.CmtFournisseur += Environment.NewLine + (viewModel.Prenom.Trim() ?? "-") + " " + (viewModel.Nom.Trim() ?? "-");
                    }
                    if (!string.IsNullOrWhiteSpace(viewModel.Tel))
                    {
                        dataModel.CmtFournisseur += Environment.NewLine + viewModel.Tel.Trim();
                    }
                    if (!string.IsNullOrWhiteSpace(viewModel.Email))
                    {
                        dataModel.CmtFournisseur += Environment.NewLine + viewModel.Email.Trim();
                    }
                }
                if (!string.IsNullOrWhiteSpace(viewModel.Cmt))
                {
                    dataModel.CmtFournisseur += Environment.NewLine
                        + " Commentaires : " + viewModel.Cmt.Trim();
                }
                dataModel.CmtFournisseur += Environment.NewLine
                    + "*** Fin des données externes ***";
            }
            else
            //Modify or cancel
            {
                //Add Cmt
                dataModel.CmtFournisseur += Environment.NewLine
                    + "*** Nouvelles données externes le " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " ***";
                dataModel.CmtFournisseur += Environment.NewLine + "Dispo. : ";
                if (viewModel.DDisponibilite != null)
                {
                    dataModel.CmtFournisseur += ((DateTime)viewModel.DDisponibilite).ToString("dd/MM/yyyy");
                }
                dataModel.CmtFournisseur += Environment.NewLine + "Poids : " + viewModel.PoidsChargement.ToString();
                dataModel.CmtFournisseur += Environment.NewLine + "Nb balle(s) : " + viewModel.NbBalleChargement.ToString();
                if (!string.IsNullOrWhiteSpace(viewModel.Cmt))
                {
                    dataModel.CmtFournisseur += Environment.NewLine
                        + "Commentaires : " + viewModel.Cmt.Trim();
                }
                //LASER action
                if (viewModel.ActionCode == Enumerations.APICommandeFournisseurActionCode.RefusDocumentationTransporteur.ToString())
                {
                    dataModel.CmtFournisseur += Environment.NewLine + "Action Laser : " + "refus de documentation transporteur";
                    if (!string.IsNullOrWhiteSpace(viewModel.carrierRejectedLastComment))
                    {
                        dataModel.CmtFournisseur += Environment.NewLine + "Commentaires Laser : " + viewModel.carrierRejectedLastComment.Trim();
                    }
                    dataModel.ValideDPrevues = false;
                }
                else if (viewModel.ActionCode == Enumerations.APICommandeFournisseurActionCode.RefusDocumentationRecycleur.ToString())
                {
                    dataModel.CmtFournisseur += Environment.NewLine + "Action Laser : " + "refus de documentation recycleur";
                    if (!string.IsNullOrWhiteSpace(viewModel.receiptRejectedLastComment))
                    {
                        dataModel.CmtFournisseur += Environment.NewLine + "Commentaires Laser : " + viewModel.receiptRejectedLastComment.Trim();
                    }
                }
                else if (viewModel.ActionCode == Enumerations.APICommandeFournisseurActionCode.Annulation.ToString())
                {
                    dataModel.CmtFournisseur += Environment.NewLine + "Action Laser : " + "annulation de demande d'enlèvement";
                    if (!string.IsNullOrWhiteSpace(viewModel.receiptRejectedLastComment))
                    {
                        dataModel.CmtFournisseur += Environment.NewLine + "Commentaires Laser : " + viewModel.receiptRejectedLastComment.Trim();
                    }
                }
                dataModel.CmtFournisseur += Environment.NewLine
                        + "*** Fin des données externes ***";
                //Create message
                string corps = "", corpsHTML = " ", titre = "";
                //Depending on modification type
                if (viewModel.ActionCode == Enumerations.APICommandeFournisseurActionCode.Annulation.ToString())
                {
                    if (dataModel.ValideDPrevues)
                    {
                        corps = "La demande d'enlèvement " + (Utils.Utils.FormatNumeroCommande(dataModel?.NumeroCommande.ToString()) ?? "NA") + " (Réf. ext. " + (dataModel?.LibExt ?? "NA") + ", Id ext. " + (dataModel?.RefExt ?? "NA") + ") a été annulée."
                            + Environment.NewLine
                            + "Veuillez consulter le commentaire dans la commande pour plus de détails.";
                        corpsHTML = System.Net.WebUtility.HtmlEncode("La demande d'enlèvement ")
                            + "<a href=\"/*appLink=commande-fournisseur/" + dataModel.RefCommandeFournisseur.ToString() + "\">" + (Utils.Utils.FormatNumeroCommande(dataModel?.NumeroCommande.ToString()) ?? "NA") + " (Réf. ext. " + (dataModel?.LibExt ?? "NA") + ", Id ext. " + (dataModel?.RefExt ?? "NA") + ")" + " </a>"
                            + System.Net.WebUtility.HtmlEncode(" a été annulée.")
                            + "<br/>"
                            + System.Net.WebUtility.HtmlEncode("Veuillez consulter le commentaire dans la commande pour plus de détails.")
                              ;
                        corpsHTML = "<span style=\"font-family:Roboto; font-size:11pt;\">" + corpsHTML + "</span>";
                    }
                    else
                    {
                        corps = "La demande d'enlèvement " + (Utils.Utils.FormatNumeroCommande(dataModel?.NumeroCommande.ToString()) ?? "NA") + " (Réf. ext. " + (dataModel?.LibExt ?? "NA") + ", Id ext. " + (dataModel?.RefExt ?? "NA") + ") a été annulée et supprimée.";
                        corps += Environment.NewLine + dataModel.Entite.Libelle;
                        corps += Environment.NewLine + "Date de disponibilité : " + ((DateTime)dataModel.D).ToString("dd/MM/yyyy");
                        corps += Environment.NewLine;
                        corps += dataModel.Produit.Libelle + ", ";
                        corps += dataModel.PoidsChargement + " kg, " + dataModel.NbBalleChargement + "balle(s)";
                        if (!string.IsNullOrWhiteSpace(dataModel.Transporteur?.Libelle))
                        {
                            corps += Environment.NewLine + "Transporteur : " + dataModel.Transporteur.Libelle;
                        }
                        if (!string.IsNullOrWhiteSpace(dataModel.AdresseClient?.Entite?.Libelle))
                        {
                            corps += Environment.NewLine + "Client : " + dataModel.AdresseClient?.Entite.Libelle;
                        }
                        corpsHTML = Utils.Utils.FormatHTMLNewLines(System.Net.WebUtility.HtmlEncode(corps));
                        corpsHTML = "<span style=\"font-family:Roboto; font-size:11pt;\">" + corpsHTML + "</span>";
                        //Save data
                        DbContext.CommandeFournisseurs.Remove(dataModel);
                    }
                }
                else if (viewModel.ActionCode == Enumerations.APICommandeFournisseurActionCode.RefusDocumentationTransporteur.ToString()
                    || viewModel.ActionCode == Enumerations.APICommandeFournisseurActionCode.RefusDocumentationRecycleur.ToString())
                {
                    //Create message and save data
                    corps = "La demande d'enlèvement " + (Utils.Utils.FormatNumeroCommande(dataModel?.NumeroCommande.ToString()) ?? "NA") + " (Réf. ext. " + (dataModel?.LibExt ?? "NA") + ", Id ext. " + (dataModel?.RefExt ?? "NA") + ") a fait l'objet d'un refus de documentation."
                    + Environment.NewLine
                    + "Veuillez consulter le commentaire dans la commande pour plus de détails.";
                    corpsHTML = System.Net.WebUtility.HtmlEncode("La demande d'enlèvement ")
                        + "<a href=\"/*appLink=commande-fournisseur/" + dataModel.RefCommandeFournisseur.ToString() + "\">" + (Utils.Utils.FormatNumeroCommande(dataModel?.NumeroCommande.ToString()) ?? "NA") + " (Réf. ext. " + (dataModel?.LibExt ?? "NA") + ", Id ext. " + (dataModel?.RefExt ?? "NA") + ")" + " </a>"
                        + System.Net.WebUtility.HtmlEncode(" a fait l'objet d'un refus de documentation.")
                        + "<br/>"
                        + System.Net.WebUtility.HtmlEncode("Veuillez consulter le commentaire dans la commande pour plus de détails.")
                          ;
                    corpsHTML = "<span style=\"font-family:Roboto; font-size:11pt;\">" + corpsHTML + "</span>";
                }
                else
                {
                    //Create message and save data
                    corps = "La demande d'enlèvement " + (Utils.Utils.FormatNumeroCommande(dataModel?.NumeroCommande.ToString()) ?? "NA") + " (Réf. ext. " + (dataModel?.LibExt ?? "NA") + ", Id ext. " + (dataModel?.RefExt ?? "NA") + ") a été modifiée."
                    + Environment.NewLine
                    + "Veuillez consulter le commentaire dans la commande pour plus de détails.";
                    corpsHTML = System.Net.WebUtility.HtmlEncode("La demande d'enlèvement ")
                        + "<a href=\"/*appLink=commande-fournisseur/" + dataModel.RefCommandeFournisseur.ToString() + "\">" + (Utils.Utils.FormatNumeroCommande(dataModel?.NumeroCommande.ToString()) ?? "NA") + " (Réf. ext. " + (dataModel?.LibExt ?? "NA") + ", Id ext. " + (dataModel?.RefExt ?? "NA") + ")" + " </a>"
                        + System.Net.WebUtility.HtmlEncode(" a été modifiée.")
                        + "<br/>"
                        + System.Net.WebUtility.HtmlEncode("Veuillez consulter le commentaire dans la commande pour plus de détails.")
                          ;
                    corpsHTML = "<span style=\"font-family:Roboto; font-size:11pt;\">" + corpsHTML + "</span>";
                }
                //Save data
                if (!dataModel.ValideDPrevues)
                {
                    dataModel.PoidsChargement = viewModel.PoidsChargement;
                    dataModel.NbBalleChargement = viewModel.NbBalleChargement;
                    dataModel.DMoisDechargementPrevu = new DateTime(((DateTime)viewModel.DDisponibilite).Year, ((DateTime)viewModel.DDisponibilite).Month, 1, 0, 0, 0);
                }
                if (viewModel.ActionCode == Enumerations.APICommandeFournisseurActionCode.Annulation.ToString())
                {
                    titre = "Annulation de demande d'enlèvement prestataire";
                }
                else
                {
                    titre = "Modification de demande d'enlèvement prestataire";
                }
                //Add message if applicable
                if (!string.IsNullOrWhiteSpace(corps) || !string.IsNullOrWhiteSpace(corpsHTML))
                {
                    var msg = new Message()
                    {
                        RefUtilisateurCourant = CurrentContext.RefUtilisateur,
                        RefMessageType = 4,
                        Libelle = titre,
                        Titre = titre,
                        Corps = corps,
                        CorpsHTML = corpsHTML,
                        DiffusionUnique = true,
                        VisualisationConfirmeUnique = true,
                        DDebut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        DFin = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).AddDays(7),
                        Actif = true,
                        Important = true
                    };
                    DbContext.Messages.Add(msg);
                    //Manage Diffusion
                    msg.MessageDiffusions = new HashSet<MessageDiffusion>();
                    msg.MessageDiffusions.Add(new MessageDiffusion()
                    {
                        RefModule = Enumerations.Module.Logistique.ToString(),
                        RefHabilitation = Enumerations.HabilitationLogistique.Administrateur.ToString()
                    });
                }
            }
        }
        #endregion
    }
}

