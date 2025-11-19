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

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class APITransportController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public APITransportController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper, IWebHostEnvironment env)
            : base(context, configuration)
        {
            _mapper = mapper;
            _env = env;
        }
        protected IWebHostEnvironment _env;
        #endregion Constructor

        /// <summary>
        /// Get data depending of body content
        /// </summary>
        /// <param name="model">The APIPostViewModel containing the details of the request</param>
        [HttpPost("getlist")]
        public IActionResult getlist([FromBody] APIPostViewModel model)
        {
            string err = "";
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(HttpContext.Request.Path, null, (int?)HttpStatusCode.BadRequest, CurrentContext.CulturedRessources.GetTextRessource(711), "Contenu non reconnu", CurrentContext.RefUtilisateur, DbContext);
                //End
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }

            //---------------------------------------------------------------------------------------------------
            //Store the raw payload
            string jsonBody = JsonConvert.SerializeObject(model);
            //Check validity
            List<APITransportViewModel> aPITransports = new List<APITransportViewModel>();
            //---------------------------------------------------------------------------------------------------
            //Process if no error
            var cmds = DbContext.CommandeFournisseurs
                .Include(i => i.AdresseClient)
                .ThenInclude(i => i.Entite)
                .Include(i => i.AdresseClient)
                .ThenInclude(i => i.ContactAdresses)
                .ThenInclude(i => i.ContactAdresseContactAdresseProcesss)
                .Where(e => e.AdresseClient.RefEntite == CurrentContext.ConnectedUtilisateur.RefClient
                 && e.ValideDPrevues && e.DDechargement == null).ToList();
            foreach (var c in cmds)
            {
                var v = new APITransportViewModel();
                var ctc = c.AdresseClient.ContactAdresses.Where(e => e.Actif == true && e.ContactAdresseContactAdresseProcesss.Any(f => f.RefContactAdresseProcess == (int)Enumerations.ContactAdresseProcess.BonDeLivraison)).FirstOrDefault();
                v.RefCommandeFournisseur = c.RefCommandeFournisseur;
                v.NumeroCommande = c.NumeroCommande;
                v.CommandeFournisseurStatutLibelle = c.CommandeFournisseurStatut.Libelle;
                v.Mixte = c.Mixte;
                v.NumeroAffretement = c.NumeroAffretement;
                v.OrdreAffretement = c.OrdreAffretement;
                v.CamionComplet = c.CamionComplet;
                v.OrigineEntiteRefEntite = c.RefEntite;
                v.OrigineEntiteSiret = c.Entite.IdNational;
                v.OrigineEntiteLibelle = c.Entite.Libelle;
                v.OrigineHoraires = c.Horaires;
                v.OrigineAdresseRefAdresse = c.RefAdresse;
                v.OrigineAdresseLibelle = c.Libelle;
                v.OrigineAdresseAdr1 = c.Adr1;
                v.OrigineAdresseAdr2 = c.Adr2;
                v.OrigineAdresseCodePostal = c.CodePostal;
                v.OrigineAdresseVille = c.Ville;
                v.OrigineAdressePaysLibelle = c.Pays.Libelle;
                v.OrigineContactPrenom = c.Prenom;
                v.OrigineContactNom = c.Nom;
                v.OrigineContactTel = c.Tel;
                v.OrigineContactTelMobile = c.TelMobile;
                v.OrigineContactEmail = c.Email;
                v.RefProduit = c.RefProduit;
                v.ProduitLibelle = c.Produit.Libelle;
                v.ProduitCmtTransporteur = c.Produit.CmtTransporteur;
                v.D = c.D;
                v.DMoisDechargementPrevu = (c.DMoisDechargementPrevu == null ? null : ((DateTime)c.DMoisDechargementPrevu).Month);
                v.DChargementPrevue = c.DChargementPrevue;
                v.HoraireChargementPrevu = c.HoraireChargementPrevu;
                v.DChargement = c.DChargement;
                v.DDechargementPrevue = c.DDechargementPrevue;
                v.HoraireDechargementPrevu = c.HoraireDechargementPrevu;
                v.DDechargement = c.DDechargement;
                v.PoidsChargement = c.PoidsChargement;
                v.NbBalleChargement = c.NbBalleChargement;
                v.PoidsDechargement = c.PoidsDechargement;
                v.NbBalleDechargement = c.NbBalleDechargement;
                v.CamionTypeLibelle = c.CamionType.Libelle;
                v.TransporteurEntiteRefEntite = c.Transporteur.RefEntite;
                v.TransporteurEntiteSiret = c.Transporteur.IdNational;
                v.TransporteurEntiteLibelle = c.Transporteur.Libelle;
                v.TransporteurContactPrenom = c.TransporteurContactAdresse.Contact.Prenom;
                v.TransporteurContactNom = c.TransporteurContactAdresse.Contact.Nom;
                v.TransporteurContactTel = c.TransporteurContactAdresse.Tel;
                v.TransporteurContactTelMobile = c.TransporteurContactAdresse.TelMobile;
                v.TransporteurContactEmail = c.TransporteurContactAdresse.Email;
                v.Km = c.Km;
                v.DestinationEntiteRefEntite = c.AdresseClient.RefEntite;
                v.DestinationEntiteSiret = c.AdresseClient.Entite.IdNational;
                v.DestinationEntiteLibelle = c.AdresseClient.Entite.Libelle;
                v.DestinationAdresseRefAdresse = c.AdresseClient.RefAdresse;
                v.DestinationAdresseLibelle = c.AdresseClient.Libelle;
                v.DestinationAdresseAdr1 = c.AdresseClient.Adr1;
                v.DestinationAdresseAdr2 = c.AdresseClient.Adr2;
                v.DestinationAdresseCodePostal = c.AdresseClient.CodePostal;
                v.DestinationAdresseVille = c.AdresseClient.Ville;
                v.DestinationAdressePaysLibelle = c.AdresseClient.Pays.Libelle;
                v.DestinationHoraires = c.AdresseClient.Horaires;
                v.DestinationContactPrenom = ctc?.Contact.Prenom;
                v.DestinationContactNom = ctc?.Contact.Nom;
                v.DestinationContactTel = ctc?.Tel;
                v.DestinationContactTelMobile = ctc?.TelMobile;
                v.DestinationContactEmail = ctc?.Email;
                v.CommandeFournisseurCmtTransporteur = c.CmtTransporteur;
                v.RefExt = c.RefExt;
                aPITransports.Add(v);
            }
            //End
            if (string.IsNullOrEmpty(err))
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(HttpContext.Request.Path, jsonBody, (int?)HttpStatusCode.OK, null, "", CurrentContext.RefUtilisateur, DbContext);
                //End
                return new JsonResult(aPITransports, JsonSettings);
            }
            else
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(HttpContext.Request.Path, jsonBody, (int?)HttpStatusCode.Conflict, null, err, CurrentContext.RefUtilisateur, DbContext);
                //End
                return Conflict(new ConflictError(err));
            }
        }
    }
}
