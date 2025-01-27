/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/08/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using eVaSys.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using eVaSys.APIUtils;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class GridController : BaseApiController
    {
        #region Constructor
        public GridController(ApplicationDbContext context, IConfiguration configuration)
            : base(context, configuration)
        {
            _dbContext = context;
        }
        #endregion Constructor
        private readonly ApplicationDbContext _dbContext;
        /// <summary>
        /// GET: evapi/grid/get
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all items.</returns>
        [HttpGet("GetItems")]
        public IActionResult GetAll()
        {
            string module = Request.Query["module"].ToString();
            string menu = Request.Query["menu"].ToString();
            int pageNumber = System.Convert.ToInt32(Request.Query["pageNumber"]);
            int pageSize = System.Convert.ToInt32(Request.Query["pageSize"]);
            string sortOrder = Request.Query["sortOrder"].ToString();
            string sortExpression = Request.Query["sortExpression"].ToString();
            string filterBegin = Request.Headers["filterBegin"].ToString();
            string filterEnd = Request.Headers["filterEnd"].ToString();
            string filterText = Request.Query["filterText"].ToString();
            string filterApplicationProduitOrigines = Request.Headers["filterApplicationProduitOrigines"].ToString();
            string filterClients = Request.Headers["filterClients"].ToString();
            string filterTransporteurs = Request.Headers["filterTransporteurs"].ToString();
            string filterPrestataires = Request.Headers["filterPrestataires"].ToString();
            string filterCentreDeTris = Request.Headers["filterCentreDeTris"].ToString();
            bool filterAnomalieCommandeFournisseur = (Request.Headers["filterAnomalieCommandeFournisseur"] == "true" ? true : false);
            bool filterDatesTransporteur = (Request.Headers["filterDatesTransporteur"] == "true" ? true : false);
            bool filterDChargementModif = (Request.Headers["filterDChargementModif"] == "true" ? true : false);
            string filterDptDeparts = Request.Headers["filterDptDeparts"].ToString();
            string filterDptArrivees = Request.Headers["filterDptArrivees"].ToString();
            string filterVilleDeparts = Request.Headers["filterVilleDeparts"].ToString();
            string filterVilleArrivees = Request.Headers["filterVilleArrivees"].ToString();
            string filterPayss = Request.Headers["filterPayss"].ToString();
            string filterProcesss = Request.Headers["filterProcesss"].ToString();
            string filterRegionReportings = Request.Headers["filterRegionReportings"].ToString();
            string filterProduits = Request.Headers["filterProduits"].ToString();
            string filterComposants = Request.Headers["filterComposants"].ToString();
            string filterCamionTypes = Request.Headers["filterCamionTypes"].ToString();
            string filterMonths = Request.Headers["filterMonths"].ToString();
            string filterYears = Request.Headers["filterYears"].ToString();
            string filterEnvCommandeFournisseurStatuts = Request.Headers["filterEnvCommandeFournisseurStatuts"].ToString();
            bool filterCommandesFournisseurNonChargees = (Request.Headers["filterCommandesFournisseurNonChargees"] == "true" ? true : false);
            bool filterCommandesFournisseurAttribuees = (Request.Headers["filterCommandesFournisseurAttribuees"] == "true" ? true : false);
            bool filterValideDPrevues = (Request.Headers["filterValideDPrevues"] == "true" ? true : false);
            string selectedItem = Request.Headers["selectedItem"].ToString();
            string moisDechargementPrevuItem = Request.Headers["moisDechargementPrevuItem"].ToString();
            string augmentation = Request.Headers["augmentation"].ToString();
            string exportTransport = Request.Headers["exportTransport"].ToString();
            string action = Request.Headers["action"].ToString();
            string yearFrom = Request.Headers["yearFrom"].ToString();
            string monthFrom = Request.Headers["monthFrom"].ToString();
            string yearTo = Request.Headers["yearTo"].ToString();
            string monthTo = Request.Headers["monthTo"].ToString();
            string filterEntiteTypes = Request.Headers["filterEntiteTypes"].ToString();
            bool filterDonneeEntiteSage = (Request.Headers["filterDonneeEntiteSage"] == "true" ? true : false);
            string filterEcoOrganismes = Request.Headers["filterEcoOrganismes"].ToString();
            string filterMessageTypes = Request.Headers["filterMessageTypes"].ToString();
            bool filterControle = (Request.Headers["filterControle"] == "true" ? true : false);
            string filterDRs = Request.Headers["filterDRs"].ToString();
            string filterUtilisateurMaitres = Request.Headers["filterUtilisateurMaitres"].ToString();
            string filterNonConformiteEtapeTypes = Request.Headers["filterNonConformiteEtapeTypes"].ToString();
            string filterNonConformiteNatures = Request.Headers["filterNonConformiteNatures"].ToString();
            bool filterIFFournisseur = (Request.Headers["filterIFFournisseur"] == "true" ? true : false);
            bool filterIFClient = (Request.Headers["filterIFClient"] == "true" ? true : false);
            bool filterIFFournisseurRetourLot = (Request.Headers["filterIFFournisseurRetourLot"] == "true" ? true : false);
            bool filterIFFournisseurFacture = (Request.Headers["filterIFFournisseurFacture"] == "true" ? true : false);
            bool filterIFFournisseurAttenteBonCommande = (Request.Headers["filterIFFournisseurAttenteBonCommande"] == "true" ? true : false);
            bool filterIFFournisseurTransmissionFacturation = (Request.Headers["filterIFFournisseurTransmissionFacturation"] == "true" ? true : false);
            bool filterNonConformitesATraiter = (Request.Headers["filterNonConformitesATraiter"] == "true" ? true : false);
            bool filterNonConformitesATransmettre = (Request.Headers["filterNonConformitesATransmettre"] == "true" ? true : false);
            bool filterNonConformitesPlanActionAValider = (Request.Headers["filterNonConformitesPlanActionAValider"] == "true" ? true : false);
            bool filterNonConformitesPlanActionNR = (Request.Headers["filterNonConformitesPlanActionNR"] == "true" ? true : false);
            bool filterActif = (Request.Headers["filterActif"] == "true" ? true : false);
            bool? filterNonRepartissable = null;
            string filterCollecte = Request.Headers["filterCollecte"].ToString();
            if (Request.Headers["filterNonRepartissable"] == "true") { filterNonRepartissable = true; }
            else if (Request.Headers["filterNonRepartissable"] == "false") { filterNonRepartissable = false; }
            SqlCommand cmd = new();
            DataSet dS = new();
            string sqlStr = "";
            //Unlock data
            Utils.Utils.UnlockAllData(CurrentContext.RefUtilisateur, DbContext);
            //Process
            //Chech authorization
            if (!Rights.AuthorizedMenu(menu, CurrentContext, Configuration))
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            if (!string.IsNullOrEmpty(action) && !Rights.AuthorizedAction(action, CurrentContext, Configuration))
            {
                Utils.Utils.DebugPrint("Grid controller - no action", Configuration["Data:DefaultConnection:ConnectionString"]);
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            switch (module)
            {
                case "Logistique":
                case "ModulePrestataire":
                    //Process if Authorized
                    switch (menu)
                    {
                        case "LogistiqueMenuTransportNonValide":
                        case "LogistiqueMenuModifierTransport":
                        case "LogistiqueMenuModifierTousPrixTransport":
                        case "LogistiqueMenuSupprimerTransportEnMasse":
                            //Chaine SQL globale
                            sqlStr = "select RefTransport as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefTransport.ToString()].Name + "]";
                            if (CurrentContext.ConnectedUtilisateur.RefTransporteur == null)
                            {
                                sqlStr += "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                                + "     , fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]";
                            }
                            sqlStr += "     , adresseOrigine.CodePostal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineCP.ToString()].Name + "]"
                                + "     , adresseOrigine.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]";
                            if (CurrentContext.ConnectedUtilisateur.RefTransporteur == null)
                            {
                                sqlStr += "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]";
                            }
                            sqlStr += "     , adresseDestination.CodePostal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationCP.ToString()].Name + "]"
                                + "     , adresseDestination.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                                + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]"
                                + "     , tbrCamionType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CamionTypeLibelle.ToString()].Name + "]"
                                + "     , Km as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ParcoursKm.ToString()].Name + "]"
                                + "     , PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransportPUHT.ToString()].Name + "]"
                                + "     , PUHTDemande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransportPUHTDemande.ToString()].Name + "]"
                                + "     , univers.RefAdresseOrigine as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefAdresseOrigine.ToString()].Name + "]"
                                + "     , univers.RefAdresseDestination as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefAdresseDestination.ToString()].Name + "]"
                                + "     , univers.RefTransporteur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefTransporteur.ToString()].Name + "]"
                                + "     , tbrCamionType.RefCamionType as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCamionType.ToString()].Name + "]"
                                + " from"
                                + " 	(select transporteur.RefEntite as RefTransporteur, adresseOrigine.RefAdresse as RefAdresseOrigine, adresseDestination.RefAdresse as RefAdresseDestination, RefCamionType"
                                + "   	from "
                                + "    		(select RefEntite from tblentite where RefEntiteType=2) as transporteur"
                                + "    		, (select tblAdresse.RefAdresse from tblAdresse "
                                + "    			inner join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                                + "    			inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                                + "    			inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse=tbmContactAdresse.RefContactAdresse "
                                + "    			where tblEntite.Actif=1 and tblAdresse.Actif=1 and RefContactAdresseProcess=1 and tblEntite.RefEntiteType=3) as adresseOrigine	"
                                + "    		, (select tblAdresse.RefAdresse from tblAdresse "
                                + "    			inner join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                                + "    			inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                                + "    			inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse=tbmContactAdresse.RefContactAdresse "
                                + "    			where tblEntite.Actif=1 and tblAdresse.Actif=1 and RefContactAdresseProcess=1 and tblEntite.RefEntiteType in(4,5)) as adresseDestination";
                            if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null)
                            {
                                sqlStr += "         , (select distinct RefCamionType from tbmEntiteCamionType where RefEntite = " + CurrentContext.ConnectedUtilisateur.RefTransporteur + ") as tbrCamionType";
                            }
                            else
                            {
                                sqlStr += "         , tbrCamionType";

                            }
                            sqlStr += ") as univers"
                                + "    	left join tblParcours on univers.RefAdresseOrigine=tblParcours.RefAdresseOrigine and univers.RefAdresseDestination=tblParcours.RefAdresseDestination"
                                + "    	left join tblTransport on tblTransport.RefParcours=tblParcours.RefParcours and tblTransport.RefTransporteur=univers.RefTransporteur and tblTransport.RefCamionType=univers.RefCamionType"
                                + "     inner join tbrCamionType on univers.RefCamionType= tbrCamionType.RefCamionType"
                                + "    	inner join tblEntite as transporteur on transporteur.RefEntite=univers.RefTransporteur"
                                + "     inner join tbmEntiteCamionType on tbmEntiteCamionType.RefCamionType=univers.RefCamionType and tbmEntiteCamionType.RefEntite=Univers.RefTransporteur"
                                + "    	inner join tblAdresse as adresseOrigine on univers.RefAdresseOrigine=adresseOrigine.RefAdresse"
                                + "    	inner join tblEntite as fournisseur on fournisseur.RefEntite=adresseOrigine.RefEntite"
                                + "    	inner join tblAdresse as adresseDestination on univers.RefAdresseDestination=adresseDestination.RefAdresse"
                                + "     left join tbrPays on adresseDestination.RefPays=tbrPays.RefPays"
                                + "    	inner join tblEntite as client on client.RefEntite=adresseDestination.RefEntite"
                                + " where 1=1 ";
                            if (menu != Enumerations.MenuName.LogistiqueMenuModifierTransport.ToString())
                            {
                                sqlStr += " and tblTransport.RefTransport is not null";
                            }
                            if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null)
                            {
                                sqlStr += " and univers.RefTransporteur = " + CurrentContext.ConnectedUtilisateur.RefTransporteur;
                            }
                            //Menu specific
                            if (menu == Enumerations.MenuName.LogistiqueMenuTransportNonValide.ToString())
                            {
                                sqlStr += " and tblTransport.PUHTDemande is not null";
                            }
                            if (menu == Enumerations.MenuName.LogistiqueMenuModifierTousPrixTransport.ToString())
                            {
                                sqlStr += " and isnull(PUHT,0)<>0";
                            }
                            //General Filters
                            if (!string.IsNullOrEmpty(filterTransporteurs))
                            {
                                sqlStr += " and univers.RefTransporteur in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", filterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterDptDeparts))
                            {
                                sqlStr += " and case when len(adresseOrigine.CodePostal)=5 and adresseOrigine.RefPays=1 then left(adresseOrigine.CodePostal,2) else '' end in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refDptDepart", filterDptDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterVilleDeparts))
                            {
                                sqlStr += " and rtrim(ltrim(adresseOrigine.Ville)) COLLATE Latin1_general_CI_AI in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("adresseOrigineVille", filterVilleDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterDptArrivees))
                            {
                                sqlStr += " and case when len(adresseDestination.CodePostal)=5 and adresseDestination.RefPays=1 then left(adresseDestination.CodePostal,2) else '' end in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refDptArrivee", filterDptArrivees, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterVilleArrivees))
                            {
                                sqlStr += " and rtrim(ltrim(adresseDestination.Ville)) COLLATE Latin1_general_CI_AI in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("adresseDestinationVille", filterVilleArrivees, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterPayss))
                            {
                                sqlStr += " and adresseDestination.RefPays in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refPays", filterPayss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterCamionTypes))
                            {
                                sqlStr += " and tbrCamionType.RefCamionType in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refCamionType", filterCamionTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            break;
                        case "LogistiqueMenuTransportDemandeEnlevement":
                            //Chaine SQL globale
                            sqlStr = "select RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                                + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                                + "     , left(tblCommandeFournisseur.CodePostal+'  ',2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineDpt.ToString()].Name + "]"
                                + "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineVille.ToString()].Name + "]"
                                + "     , D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                                + "     , DMoisDechargementPrevu as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMoisDechargementPrevu.ToString()].Name + "]"
                                + "     , left(adresseDestination.CodePostal+'  ',2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationDpt.ToString()].Name + "]"
                                + "     , adresseDestination.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                                + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]"
                                + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                + " from tblCommandeFournisseur"
                                + "     inner join tblEntite on tblEntite.RefEntite=tblCommandeFournisseur.RefEntite"
                                + "     left join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                                + "     left join tblAdresse as adresseDestination on tblCommandeFournisseur.RefAdresseClient=adresseDestination.RefAdresse"
                                + "     left join tbrPays on adresseDestination.RefPays=tbrPays.RefPays"
                                + "     left join tblEntite as client on client.RefEntite=adresseDestination.RefEntite"
                                + " where (ValideDPrevues=0"
                                + "     and (RefCommandeFournisseurStatut=1)"
                                + "     and (DChargement is null or DDechargement is null)"
                                + "     and DBlocage is null"
                                + "     and RefusCamion=0"
                                + "     and (RefTransporteur is null or (RefTransporteur is not null and dbo.JoursTravailles(DAffretement,getdate())>2))"
                                + "     )";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , null
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement }
                                );
                            if (!string.IsNullOrEmpty(filterBegin) || !string.IsNullOrEmpty(filterEnd))
                            {
                                DateTime begin = DateTime.MinValue;
                                DateTime end = DateTime.MinValue;
                                if (!string.IsNullOrEmpty(filterBegin)) { DateTime.TryParse(filterBegin, out begin); }
                                if (!string.IsNullOrEmpty(filterEnd)) { DateTime.TryParse(filterEnd, out end); }
                                if (begin != DateTime.MinValue) { cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin; }
                                if (end != DateTime.MinValue) { cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end; }
                                if (begin != DateTime.MinValue && end != DateTime.MinValue)
                                {
                                    sqlStr += " and tblCommandeFournisseur.D between @begin and @end";
                                }
                                else if (begin != DateTime.MinValue)
                                {
                                    sqlStr += " and tblCommandeFournisseur.D >= @begin";
                                }
                                else if (end != DateTime.MinValue)
                                {
                                    sqlStr += " and tblCommandeFournisseur.D <= @end";
                                }
                            }
                            if (!string.IsNullOrEmpty(filterTransporteurs) && CurrentContext.ConnectedUtilisateur.RefTransporteur == null)
                            {
                                sqlStr += " and tblCommandeFournisseur.RefTransporteur in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", filterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterDptDeparts))
                            {
                                sqlStr += " and case when len(tblCommandeFournisseur.CodePostal)=5 then left(tblCommandeFournisseur.CodePostal,2) else '' end in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refDptDepart", filterDptDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterVilleDeparts))
                            {
                                sqlStr += " and rtrim(ltrim(tblCommandeFournisseur.Ville)) COLLATE Latin1_general_CI_AI in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("adresseOrigineVille", filterVilleDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterDptArrivees))
                            {
                                sqlStr += " and case when len(adresseDestination.CodePostal)=5 and adresseDestination.RefPays=1 then left(adresseDestination.CodePostal,2) else '' end in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refDptArrivee", filterDptArrivees, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterVilleArrivees))
                            {
                                sqlStr += " and rtrim(ltrim(adresseDestination.Ville)) COLLATE Latin1_general_CI_AI in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("adresseDestinationVille", filterVilleArrivees, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterPayss))
                            {
                                sqlStr += " and tbrPays.RefPays in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refPays", filterPayss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterMonths))
                            {
                                sqlStr += " and month(DMoisDechargementPrevu) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refMonth", filterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            break;
                        case "LogistiqueMenuCommandeClient":
                            //Chaine SQL globale
                            sqlStr = "select tblCommandeClient.RefCommandeClient as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeClient.ToString()].Name + "]"
                                + "     , tblCommandeClientMensuelle.RefCommandeClientMensuelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeClientMensuelle.ToString()].Name + "]"
                                + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                                + "     , case when tblAdresse.Adr1 is null then '' else tblAdresse.Adr1 + ' ' end + tblAdresse.CodePostal + ' ' + tblAdresse.Ville + ' (' + tbrAdresseType.Libelle + ')' as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Adresse.ToString()].Name + "]"
                                + "     , tblContrat.IdContrat as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratLibelle.ToString()].Name + "]"
                                + "     , tblCommandeClientMensuelle.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeClientMensuelleD.ToString()].Name + "]"
                                + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                + "     , tblCommandeClientMensuelle.Poids as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeClientMensuellePoids.ToString()].Name + "]"
                                + "     , (sum(case when DDechargement is null then PoidsChargement else PoidsDechargement end) / 10) / tblCommandeClientMensuelle.Poids as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeClientPositionne.ToString()].Name + "]"
                                + "     , tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefEntite.ToString()].Name + "]"
                                + "     , tblContrat.RefContrat as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefContrat.ToString()].Name + "]"
                                + "     , tblAdresse.RefAdresse as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefAdresse.ToString()].Name + "]"
                                + " from tblCommandeClient"
                                + "     left join tblCommandeClientMensuelle on tblCommandeClientMensuelle.RefCommandeClient = tblCommandeClient.RefCommandeClient"
                                + "     left join tblEntite on tblEntite.RefEntite = tblCommandeClient.RefEntite"
                                + "     left join tblContrat on tblContrat.RefContrat = tblCommandeClient.RefContrat"
                                + "     left join tblProduit on tblProduit.RefProduit = tblCommandeClient.RefProduit"
                                + "     left join tblAdresse on tblCommandeClient.RefAdresse = tblAdresse.RefAdresse"
                                + "     left join tbrAdresseType on tblAdresse.RefAdresseType = tbrAdresseType.RefAdresseType"
                                + "     left join (select tblCommandeFournisseur.RefCommandeFournisseur, DDechargement, PoidsChargement, PoidsDechargement, RefAdresseClient, DMoisDechargementPrevu, RefProduit, RefContrat"
                                + " 		from tblCommandeFournisseur"
                                + " 		left join VueCommandeFournisseurContrat on VueCommandeFournisseurContrat.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                                + " 		) as tblCommandeFournisseur on tblCommandeFournisseur.RefAdresseClient = tblCommandeClient.RefAdresse"
                                + " 		and year(isnull(DDechargement, DMoisDechargementPrevu)) = year(tblCommandeClientMensuelle.D) and month(isnull(DDechargement, DMoisDechargementPrevu))= month(tblCommandeClientMensuelle.D)"
                                + " 		and tblCommandeFournisseur.RefProduit=tblCommandeClient.refProduit"
                                + " 		and isnull(tblCommandeClient.RefContrat,0)=isnull(tblCommandeFournisseur.RefContrat,0)"
                                + " where tblCommandeClientMensuelle.Poids!=0";
                            //General Filters
                            if (!string.IsNullOrEmpty(filterText))
                            {
                                sqlStr += " and ((case when tblAdresse.Adr1 is null then '' else tblAdresse.Adr1 + ' ' end + tblAdresse.CodePostal + ' ' + tblAdresse.Ville + ' (' + tbrAdresseType.Libelle + ')') COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI)";
                                cmd.Parameters.Add("@filterText", SqlDbType.NVarChar).Value = filterText;
                            }
                            if (!string.IsNullOrEmpty(filterClients))
                            {
                                sqlStr += " and tblCommandeClient.RefEntite in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", filterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterProduits))
                            {
                                sqlStr += " and tblProduit.RefProduit in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterYears))
                            {
                                sqlStr += " and year(tblCommandeClientMensuelle.D) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refAnnee", filterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterMonths))
                            {
                                sqlStr += " and month(tblCommandeClientMensuelle.D) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refMonth", filterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            sqlStr += " group by tblCommandeClient.RefCommandeClient, tblCommandeClientMensuelle.RefCommandeClientMensuelle, tblEntite.Libelle, tblCommandeClientMensuelle.D"
                                + "     , case when tblAdresse.Adr1 is null then '' else tblAdresse.Adr1 + ' ' end + tblAdresse.CodePostal + ' ' + tblAdresse.Ville + ' (' + tbrAdresseType.Libelle + ')'"
                                + "     , tblContrat.IdContrat, tblProduit.Libelle, tblCommandeClientMensuelle.Poids"
                                + "     , tblEntite.RefEntite, tblContrat.RefContrat, tblAdresse.RefAdresse";
                            break;
                        case "LogistiqueMenuCommandeFournisseur":
                        case "LogistiqueMenuTransportCommande":
                        case "LogistiqueMenuTransportCommandeEnCours":
                        case "LogistiqueMenuTransportCommandeModifiee":
                        case "ModulePrestataireMenuCommandeFournisseur":
                            //Chaine SQL globale
                            if (CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Utilisateur.ToString()
                                || CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Administrateur.ToString())
                            {
                                sqlStr = "select tblCommandeFournisseur.RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                                    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                                    + "     , case when (tblCommandeFournisseur.DDechargement < convert(datetime,'2012-01-01 00:00:00',120) or unitaire.RefCommandeFournisseur is not null or mensuelle.RefRepartition is not null) then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(545) + "'"
                                    + "         else case when DDechargement is not null then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(546) + "'"
                                    + "         else case when RefusCamion = 1 then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(551) + "'"
                                    + "         else case when tblCommandeFournisseur.RefCommandeFournisseurStatut is not null then (case when tbsCommandeFournisseurStatut.RefCommandeFournisseurStatut=1 then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(547) + "'"
                                    + "         else case when tbsCommandeFournisseurStatut.RefCommandeFournisseurStatut=2 then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(548) + "' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(549) + "' end end)"
                                    + "         else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(550) + "' end end end end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurStatut.ToString()].Name + "]"
                                    + "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                                    + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                                    + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]";
                                if ((menu == Enumerations.MenuName.LogistiqueMenuTransportCommande.ToString()
                                    || menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeEnCours.ToString()
                                    || menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeModifiee.ToString()
                                    || menu == Enumerations.MenuName.ModulePrestataireMenuCommandeFournisseur.ToString()))
                                {
                                    sqlStr += "     , left(tblCommandeFournisseur.CodePostal+'  ',2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineDpt.ToString()].Name + "]";
                                }
                                sqlStr += "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineVille.ToString()].Name + "]"
                                    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                                    + "     , left(adresseDestination.CodePostal+'  ',2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationDpt.ToString()].Name + "]"
                                    + "     , adresseDestination.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                                    + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                                    + "     , DMoisDechargementPrevu as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMoisDechargementPrevu.ToString()].Name + "]"
                                    + "     , DChargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargementPrevue.ToString()].Name + "]"
                                    + "     , DDechargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargementPrevue.ToString()].Name + "]"
                                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                    + "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]"
                                    + "     , dbo.ProchainJourTravaille(DAffretement,2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurLimiteExclusivite.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.RefExt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurRefExt.ToString()].Name + "]";
                                if (menu == Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString() || menu == Enumerations.MenuName.ModulePrestataireMenuCommandeFournisseur.ToString())
                                {
                                    sqlStr += "     , case when tblFicheControle.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(766) + " ' end"
                                        + " + case when tblCVQ.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(767) + " ' end"
                                        + " + case when tblControle.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(768) + " ' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Controles.ToString()].Name + "]";
                                }
                            }
                            if (CurrentContext.ConnectedUtilisateur.HabilitationModulePrestataire == Enumerations.HabilitationModulePrestataire.Fournisseur.ToString())
                            {
                                sqlStr = "select tblCommandeFournisseur.RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                                    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                                    + "     , case when (tblCommandeFournisseur.DDechargement < convert(datetime,'2012-01-01 00:00:00',120) or unitaire.RefCommandeFournisseur is not null or mensuelle.RefRepartition is not null) then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(545) + "'"
                                    + "         else case when DDechargement is not null then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(546) + "'"
                                    + "         else case when RefusCamion = 1 then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(551) + "'"
                                    + "         else case when tblCommandeFournisseur.RefCommandeFournisseurStatut is not null then (case when tbsCommandeFournisseurStatut.RefCommandeFournisseurStatut=1 then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(547) + "'"
                                    + "         else case when tbsCommandeFournisseurStatut.RefCommandeFournisseurStatut=2 then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(548) + "' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(549) + "' end end)"
                                    + "         else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(550) + "' end end end end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurStatut.ToString()].Name + "]"
                                    + "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                                    + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                                    + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]";
                                if ((menu == Enumerations.MenuName.LogistiqueMenuTransportCommande.ToString()
                                    || menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeEnCours.ToString()
                                    || menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeModifiee.ToString()
                                    || menu == Enumerations.MenuName.ModulePrestataireMenuCommandeFournisseur.ToString()))
                                {
                                    sqlStr += "     , left(tblCommandeFournisseur.CodePostal+'  ',2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineDpt.ToString()].Name + "]";
                                }
                                sqlStr += "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineVille.ToString()].Name + "]"
                                    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                                    + "     , left(adresseDestination.CodePostal+'  ',2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationDpt.ToString()].Name + "]"
                                    + "     , adresseDestination.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                                    + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                                    + "     , DMoisDechargementPrevu as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMoisDechargementPrevu.ToString()].Name + "]"
                                    + "     , DChargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargementPrevue.ToString()].Name + "]"
                                    + "     , DDechargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargementPrevue.ToString()].Name + "]"
                                    + "     , HoraireDechargementPrevu as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurHoraireDechargementPrevu.ToString()].Name + "]"
                                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                    + "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]"
                                    + "     , dbo.ProchainJourTravaille(DAffretement,2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurLimiteExclusivite.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.RefExt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurRefExt.ToString()].Name + "]";
                                if (menu == Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString() || menu == Enumerations.MenuName.ModulePrestataireMenuCommandeFournisseur.ToString())
                                {
                                    sqlStr += "     , case when tblFicheControle.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(766) + " ' end"
                                        + " + case when tblCVQ.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(767) + " ' end"
                                        + " + case when tblControle.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(768) + " ' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Controles.ToString()].Name + "]";
                                }
                            }
                            if (CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Transporteur.ToString())
                            {
                                sqlStr = "select tblCommandeFournisseur.RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                                    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
;
                                if ((menu == Enumerations.MenuName.LogistiqueMenuTransportCommande.ToString()
                                    || menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeEnCours.ToString()
                                    || menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeModifiee.ToString()
                                    || menu == Enumerations.MenuName.ModulePrestataireMenuCommandeFournisseur.ToString()))
                                {
                                    sqlStr += "     , left(tblCommandeFournisseur.CodePostal+'  ',2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineDpt.ToString()].Name + "]";
                                }
                                sqlStr += "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineVille.ToString()].Name + "]"
                                    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                                    + "     , left(adresseDestination.CodePostal+'  ',2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationDpt.ToString()].Name + "]"
                                    + "     , adresseDestination.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                                    + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                                    + "     , DMoisDechargementPrevu as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMoisDechargementPrevu.ToString()].Name + "]"
                                    + "     , DChargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargementPrevue.ToString()].Name + "]"
                                    + "     , DDechargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargementPrevue.ToString()].Name + "]"
                                    + "     , HoraireDechargementPrevu as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurHoraireDechargementPrevu.ToString()].Name + "]"
                                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                    + "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]"
                                    + "     , dbo.ProchainJourTravaille(DAffretement,2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurLimiteExclusivite.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.RefExt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurRefExt.ToString()].Name + "]";
                                if (menu == Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString() || menu == Enumerations.MenuName.ModulePrestataireMenuCommandeFournisseur.ToString())
                                {
                                    sqlStr += "     , case when tblFicheControle.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(766) + " ' end"
                                        + " + case when tblCVQ.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(767) + " ' end"
                                        + " + case when tblControle.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(768) + " ' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Controles.ToString()].Name + "]";
                                }
                            }
                            if (CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.Client.ToString())
                            {
                                sqlStr = "select tblCommandeFournisseur.RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                                    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                                    + "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                                    + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                                    + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineVille.ToString()].Name + "]"
                                    + "     , adresseDestination.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                                    + "     , DMoisDechargementPrevu as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMoisDechargementPrevu.ToString()].Name + "]"
                                    + "     , DDechargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargementPrevue.ToString()].Name + "]"
                                    + "     , HoraireDechargementPrevu as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurHoraireDechargementPrevu.ToString()].Name + "]"
                                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                    + "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.RefExt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurRefExt.ToString()].Name + "]";
                                if (menu == Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString() || menu == Enumerations.MenuName.ModulePrestataireMenuCommandeFournisseur.ToString())
                                {
                                    sqlStr += "     , case when tblFicheControle.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(766) + " ' end"
                                        + " + case when tblCVQ.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(767) + " ' end"
                                        + " + case when tblControle.RefFicheControle is null then '' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(768) + " ' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Controles.ToString()].Name + "]";
                                }
                            }
                            if (CurrentContext.ConnectedUtilisateur.HabilitationLogistique == Enumerations.HabilitationLogistique.CentreDeTri.ToString())
                            {
                                sqlStr = "select tblCommandeFournisseur.RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                                    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                                    + "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]"
                                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                                    + "     , DChargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargementPrevue.ToString()].Name + "]"
                                    + "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                                    + "     , tbrCamionType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CamionTypeLibelle.ToString()].Name + "]"
                                    + "     , PoidsChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsChargement.ToString()].Name + "]"
                                    + "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineVille.ToString()].Name + "]"
                                    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                                    + "     , ChargementEffectue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurChargementEffectue.ToString()].Name + "]"
                                    + "     , DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]"
                                    ;
                            }
                            //Common part
                            sqlStr += "     , tblCommandeFournisseur.RefCommandeFournisseurStatut as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseurStatut.ToString()].Name + "]"
                             + "     , DTraitementAnomalieChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDTraitementAnomalieChargementInvisible.ToString()].Name + "]"
                             + "     , RefMotifAnomalieChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefMotifAnomalieChargement.ToString()].Name + "]"
                             + "     , DTraitementAnomalieClient as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDTraitementAnomalieClientInvisible.ToString()].Name + "]"
                             + "     , RefMotifAnomalieClient as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefMotifAnomalieClient.ToString()].Name + "]"
                             + "     , DTraitementAnomalieTransporteur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDTraitementAnomalieTransporteurInvisible.ToString()].Name + "]"
                             + "     , RefMotifAnomalieTransporteur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefMotifAnomalieTransporteur.ToString()].Name + "]"
                             + "     , CamionComplet as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurCamionCompletInvisible.ToString()].Name + "]"
                             + "     , DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargementInvisible.ToString()].Name + "]"
                             + "     , cast(case when tblNonConformite.RefCommandeFournisseur is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNonConformiteInvisible.ToString()].Name + "]"
                             + " from tblCommandeFournisseur"
                             + "     left join tblEntite on tblEntite.RefEntite=tblCommandeFournisseur.RefEntite"
                             + "     left join tblAdresse as adresseDestination on tblCommandeFournisseur.RefAdresseClient=adresseDestination.RefAdresse"
                             + "     left join tbrPays on adresseDestination.RefPays=tbrPays.RefPays"
                             + "     left join tblEntite as client on client.RefEntite=adresseDestination.RefEntite"
                             + "     left join tblEntite as transporteur on transporteur.RefEntite=tblCommandeFournisseur.RefTransporteur"
                             + "     left join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                             + "     left join tbrCamionType on tbrCamionType.RefCamionType=tblCommandeFournisseur.RefCamionType"
                             + "     left join tbsCommandeFournisseurStatut on tbsCommandeFournisseurStatut.RefCommandeFournisseurStatut=tblCommandeFournisseur.RefCommandeFournisseurStatut"
                             + "     left join (select distinct RefCommandeFournisseur from tblRepartition) as unitaire on tblCommandeFournisseur.RefCommandeFournisseur=unitaire.RefCommandeFournisseur "
                             + "     left join tblRepartition as mensuelle on tblCommandeFournisseur.RefEntite=mensuelle.RefFournisseur and year(tblCommandeFournisseur.DDechargement)=year(mensuelle.D) and month(tblCommandeFournisseur.DDechargement)=month(mensuelle.D) and tblCommandeFournisseur.RefProduit=mensuelle.RefProduit"
                             + "     left join tblFicheControle on tblFicheControle.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                             + "     left join (select distinct RefFicheControle from tblControle) as tblControle on tblFicheControle.RefFicheControle=tblControle.RefFicheControle"
                             + "     left join (select distinct RefFicheControle from tblCVQ) as tblCVQ on tblFicheControle.RefFicheControle=tblCVQ.RefFicheControle"
                             + "     left join (select distinct RefCommandeFournisseur from tblNonConformite) as tblNonConformite on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                             + " where 1=1";
                            if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null)
                            {
                                sqlStr += " and tblCommandeFournisseur.RefTransporteur = " + CurrentContext.ConnectedUtilisateur.RefTransporteur;
                            }
                            if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                            {
                                sqlStr += " and client.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                            }
                            if (CurrentContext.ConnectedUtilisateur.RefPrestataire != null)
                            {
                                sqlStr += " and tblCommandeFournisseur.RefPrestataire=" + CurrentContext.ConnectedUtilisateur.RefPrestataire;
                            }
                            if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                            {
                                sqlStr += " and tblCommandeFournisseur.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefCentreDeTri;
                            }
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , null
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.CommandeFournisseurNumeroCommande, Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement, Enumerations.DataColumnName.CommandeFournisseurRefExt }
                                );
                            if (menu == Enumerations.MenuName.LogistiqueMenuTransportCommande.ToString())
                            {
                                sqlStr += " and tblCommandeFournisseur.RefCommandeFournisseurStatut=1 and  ValideDPrevues=1";
                            }
                            if (menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeEnCours.ToString())
                            {
                                sqlStr += " and (tblCommandeFournisseur.RefCommandeFournisseurStatut=1 and DChargementModif is null and ValideDPrevues=0 and DDechargement is null)";
                            }
                            if (menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeModifiee.ToString())
                            {
                                sqlStr += " and ((tblCommandeFournisseur.RefCommandeFournisseurStatut=1 or tblCommandeFournisseur.RefCommandeFournisseurStatut=3) and DChargementModif is not null and ValideDPrevues=0 and DDechargement is null)";
                            }
                            if (menu == Enumerations.MenuName.LogistiqueMenuTransportCommande.ToString()
                                    || menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeEnCours.ToString()
                                    || menu == Enumerations.MenuName.LogistiqueMenuTransportCommandeModifiee.ToString()
                                    || menu == Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString()
                                    )
                            {
                                if (!string.IsNullOrEmpty(filterBegin) || !string.IsNullOrEmpty(filterEnd))
                                {
                                    DateTime begin = DateTime.MinValue;
                                    DateTime end = DateTime.MinValue;
                                    if (!string.IsNullOrEmpty(filterBegin)) { DateTime.TryParse(filterBegin, out begin); }
                                    if (!string.IsNullOrEmpty(filterEnd)) { DateTime.TryParse(filterEnd, out end); }
                                    if (begin != DateTime.MinValue) { cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin; }
                                    if (end != DateTime.MinValue) { cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end; }
                                    if (begin != DateTime.MinValue && end != DateTime.MinValue)
                                    {
                                        if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                                        { sqlStr += " and tblCommandeFournisseur.DDechargementPrevue between @begin and @end"; }
                                        else if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                                        { sqlStr += " and tblCommandeFournisseur.DChargementPrevue between @begin and @end"; }
                                        else
                                        { sqlStr += " and tblCommandeFournisseur.D between @begin and @end"; }
                                    }
                                    else if (begin != DateTime.MinValue)
                                    {
                                        if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                                        { sqlStr += " and tblCommandeFournisseur.DDechargementPrevue >= @begin"; }
                                        else if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                                        { sqlStr += " and tblCommandeFournisseur.DChargementPrevue >= @begin"; }
                                        else
                                        { sqlStr += " and tblCommandeFournisseur.D >= @begin"; }
                                    }
                                    else if (end != DateTime.MinValue)
                                    {
                                        if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                                        { sqlStr += " and tblCommandeFournisseur.DDechargementPrevue <= @end"; }
                                        else if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                                        { sqlStr += " and tblCommandeFournisseur.DChargementPrevue <= @end"; }
                                        else
                                        { sqlStr += " and tblCommandeFournisseur.D <= @end"; }
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(filterClients))
                            {
                                sqlStr += " and adresseDestination.RefEntite in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", filterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterTransporteurs))
                            {
                                sqlStr += " and tblCommandeFournisseur.RefTransporteur in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", filterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterPrestataires))
                            {
                                sqlStr += " and tblCommandeFournisseur.RefPrestataire in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refPrestataire", filterPrestataires, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterCentreDeTris))
                            {
                                sqlStr += " and tblEntite.RefEntite in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refEntite", filterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterDptDeparts))
                            {
                                sqlStr += " and case when len(tblCommandeFournisseur.CodePostal)=5 then left(tblCommandeFournisseur.CodePostal,2) else '' end in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refDptDepart", filterDptDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterVilleDeparts))
                            {
                                sqlStr += " and rtrim(ltrim(tblCommandeFournisseur.Ville)) COLLATE Latin1_general_CI_AI in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("adresseOrigineVille", filterVilleDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (filterDChargementModif == true)
                            {
                                sqlStr += " and tblCommandeFournisseur.DChargementModif is not null";
                            }
                            if (!string.IsNullOrEmpty(filterDptArrivees))
                            {
                                sqlStr += " and case when len(adresseDestination.CodePostal)=5 and adresseDestination.RefPays=1 then left(adresseDestination.CodePostal,2) else '' end in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refDptArrivee", filterDptArrivees, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterVilleArrivees))
                            {
                                sqlStr += " and rtrim(ltrim(adresseDestination.Ville))COLLATE Latin1_general_CI_AI in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("adresseDestinationVille", filterVilleArrivees, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterPayss))
                            {
                                sqlStr += " and tbrPays.RefPays in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refPays", filterPayss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterProduits))
                            {
                                sqlStr += " and tblProduit.RefProduit in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterMonths))
                            {
                                sqlStr += " and month(DMoisDechargementPrevu) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refMonth", filterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterYears))
                            {
                                sqlStr += " and year(DMoisDechargementPrevu) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refAnnee", filterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterEnvCommandeFournisseurStatuts))
                            {
                                sqlStr += " and (1!=1";
                                if (filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.Repartie.ToString()))
                                {
                                    sqlStr += " or (tblCommandeFournisseur.DDechargement < convert(datetime, '2012-01-01 00:00:00', 120) or unitaire.RefCommandeFournisseur is not null or mensuelle.RefRepartition is not null)";
                                }
                                if (filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.Receptionnee.ToString()))
                                {
                                    sqlStr += " or (year(tblCommandeFournisseur.DDechargement) >= 2012 and unitaire.RefCommandeFournisseur is null and mensuelle.RefRepartition is null)";
                                }
                                if (filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.Refusee.ToString()))
                                {
                                    sqlStr += " or ((unitaire.RefCommandeFournisseur is null and mensuelle.RefRepartition is null) and DDechargement is null and RefusCamion = 1)";
                                }
                                if (filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.Ouverte.ToString())
                                    || filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.EnAttente.ToString())
                                    || filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.Bloquee.ToString()))
                                {
                                    sqlStr += " or ((unitaire.RefCommandeFournisseur is null and mensuelle.RefRepartition is null) and DDechargement is null and isnull(RefusCamion,0) = 0 and tblCommandeFournisseur.RefCommandeFournisseurStatut in(";
                                    if (filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.Ouverte.ToString()))
                                    {
                                        sqlStr += " 1,";
                                    }
                                    if (filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.EnAttente.ToString()))
                                    {
                                        sqlStr += " 2,";
                                    }
                                    if (filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.Bloquee.ToString()))
                                    {
                                        sqlStr += " 3,";
                                    }
                                    sqlStr = sqlStr.Substring(0, sqlStr.Length - 1);
                                    sqlStr += ")";
                                    sqlStr += ")";
                                }
                                if (filterEnvCommandeFournisseurStatuts.Contains(Enumerations.CommandeFournisseurStatutName.DemandeEnlevement.ToString()))
                                {
                                    sqlStr += " or ((unitaire.RefCommandeFournisseur is null and mensuelle.RefRepartition is null) and DDechargement is null and isnull(RefusCamion,0) = 0 and tblCommandeFournisseur.RefCommandeFournisseurStatut is null)";
                                }
                                sqlStr += ")";
                            }
                            if (filterAnomalieCommandeFournisseur == true)
                            {
                                sqlStr += " and (RefUtilisateurAnomalieOk is not null and DAnomalieOk is null)";
                            }
                            if (filterDatesTransporteur == true)
                            {
                                sqlStr += " and (tblCommandeFournisseur.RefCommandeFournisseurStatut = 1 and tblCommandeFournisseur.ValideDPrevues=0 and DDechargement is null and tblCommandeFournisseur.DDechargementPrevue is not null)";
                            }
                            if (filterCommandesFournisseurNonChargees == true)
                            {
                                sqlStr += " and (ChargementEffectue=0 and DChargementPrevue is not null and dbo.JoursTravailles(DChargementPrevue,getdate())>1"
                                    + " and ChargementAnnule=0 and RefusCamion=0)";
                            }
                            if (filterCommandesFournisseurAttribuees == true)
                            {
                                sqlStr += " and (DChargementPrevue is null or DDechargementPrevue is null)";
                            }
                            if (filterValideDPrevues == true)
                            {
                                sqlStr += " and tblCommandeFournisseur.ValideDPrevues = 1";
                            }
                            if (filterDChargementModif == true)
                            {
                                sqlStr += " and (tblCommandeFournisseur.RefCommandeFournisseurStatut is not null and tblCommandeFournisseur.DChargementModif is not null and tblCommandeFournisseur.DDechargement is null and tblCommandeFournisseur.ValideDPrevues = 0)";
                            }
                            if (filterNonRepartissable == true)
                            {
                                sqlStr += " and tblCommandeFournisseur.NonRepartissable = 1";
                            }
                            else if (filterNonRepartissable == false)
                            {
                                sqlStr += " and tblCommandeFournisseur.NonRepartissable = 0";
                            }
                            if (CurrentContext.filterDR)
                            {
                                sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                            }
                            break;
                        case "LogistiqueMenuRepartition":
                            //Chaine SQL globale
                            sqlStr = "select RefRepartition as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefRepartition.ToString()].Name + "]"
                                + "     , isnull(f.Libelle,fournisseur.Libelle) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]"
                                + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                                + "     , isnull(p.Libelle, produit.Libelle) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                + "     , isnull(tblCommandeFournisseur.DDechargement, tblRepartition.D) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RepartitionD.ToString()].Name + "]"
                                + " from tblRepartition"
                                + " 	left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                                + " 	left join tblEntite as f on tblCommandeFournisseur.RefEntite=f.RefEntite"
                                + " 	left join tblProduit as p on tblCommandeFournisseur.RefProduit=p.RefProduit"
                                + " 	left join tblEntite as fournisseur on tblRepartition.RefFournisseur=fournisseur.RefEntite"
                                + " 	left join tblProduit as produit on tblRepartition.RefProduit=produit.RefProduit"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , null
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.CommandeFournisseurNumeroCommande }
                                );
                            //Other filters
                            if (!string.IsNullOrEmpty(filterCentreDeTris))
                            {
                                sqlStr += " and isnull(f.RefEntite,fournisseur.RefEntite) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", filterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterProduits))
                            {
                                sqlStr += " and isnull(p.RefProduit, produit.Refproduit) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterYears))
                            {
                                sqlStr += " and isnull(year(tblCommandeFournisseur.DDechargement),year(tblRepartition.D)) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refAnnee", filterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterMonths))
                            {
                                sqlStr += " and isnull(month(tblCommandeFournisseur.DDechargement),month(tblRepartition.D)) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refMonth", filterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (CurrentContext.filterDR)
                            {
                                sqlStr += " and isnull(f.RefEntite,fournisseur.RefEntite) in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                            }
                            break;
                        case "LogistiqueMenuPrixReprise":
                            //Chaine SQL globale
                            sqlStr = "select RefPrixReprise as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefPrixReprise.ToString()].Name + "]"
                                + "     , tbrPrixReprise.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixRepriseD.ToString()].Name + "]"
                                + "     , tbrProcess.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProcessLibelle.ToString()].Name + "]"
                                + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                + "     , composant.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ComposantLibelle.ToString()].Name + "]"
                                + "     , tblContrat.IdContrat as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratLibelle.ToString()].Name + "]"
                                + "     , tbrPrixReprise.PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHT.ToString()].Name + "]"
                                + "     , tbrPrixReprise.PUHTSurtri as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHTSurtri.ToString()].Name + "]"
                                + "     , tbrPrixReprise.PUHTTransport as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHTTransport.ToString()].Name + "]"
                                + "     , tbrPrixReprise.RefProcess as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefProcess.ToString()].Name + "]"
                                + " from tbrPrixReprise"
                                + "     left join tbrProcess on tbrPrixReprise.RefProcess=tbrProcess.RefProcess"
                                + "     left join tblProduit on tbrPrixReprise.RefProduit=tblProduit.RefProduit"
                                + "     left join tblProduit as composant on composant.RefProduit=tbrPrixReprise.RefComposant"
                                + "     left join tblContrat on tbrPrixReprise.RefContrat=tblContrat.RefContrat"
                                + " where 1=1";
                            //General Filters
                            if (!string.IsNullOrEmpty(filterProcesss))
                            {
                                sqlStr += " and tbrPrixReprise.RefProcess in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProcess", filterProcesss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterProduits))
                            {
                                sqlStr += " and tbrPrixReprise.Refproduit in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterComposants))
                            {
                                sqlStr += " and tbrPrixReprise.RefComposant in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refComposant", filterComposants, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterYears))
                            {
                                sqlStr += " and year(D) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refAnnee", filterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterMonths))
                            {
                                sqlStr += " and month(D) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refMonth", filterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            break;
                        case "LogistiqueMenuSurcoutCarburant":
                            //Chaine SQL globale
                            sqlStr = "select RefSurcoutCarburant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefSurcoutCarburant.ToString()].Name + "]"
                                + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                                + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysLibelle.ToString()].Name + "]"
                                + "     , Annee as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnneeNb.ToString()].Name + "]"
                                + "     , Mois as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MoisNb.ToString()].Name + "]"
                                + "     , Ratio as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SurcoutCarburantRatio.ToString()].Name + "]"
                                + " from tblSurcoutCarburant"
                                + "     inner join tblEntite on tblSurcoutCarburant.RefTransporteur=tblEntite.RefEntite"
                                + "     inner join tbrPays on tblSurcoutCarburant.RefPays=tbrPays.RefPays"
                                + " where 1=1";
                            //General Filters
                            if (!string.IsNullOrEmpty(filterTransporteurs))
                            {
                                sqlStr += " and tblSurcoutCarburant.RefTransporteur in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", filterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterPayss))
                            {
                                sqlStr += " and tblSurcoutCarburant.RefPays in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refPays", filterPayss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterYears))
                            {
                                sqlStr += " and annee in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("annee", filterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterMonths))
                            {
                                sqlStr += " and mois in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("mois", filterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            break;
                    }
                    break;
                case "Administration":
                    //Process if Authorized
                    switch (menu)
                    {
                        case "AdministrationMenuProduit":
                            //Base SQL statement
                            sqlStr = "select tblProduit.RefProduit as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Id.ToString()].Name + "]"
                                + "     , tblProduit.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitActif.ToString()].Name + "]"
                                + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                + "     , NomCommercial as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitNomCommercial.ToString()].Name + "]"
                                + "     , NomCommun as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitNomCommun.ToString()].Name + "]"
                                + "     , Collecte as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Collecte.ToString()].Name + "]"
                                + "     , Composant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitComposant.ToString()].Name + "]"
                                + "     , NumeroStatistique as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitNumeroStatistique.ToString()].Name + "]"
                                + "     , tbrSAGECodeTransport.Code as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGECodeTransportCode.ToString()].Name + "]"
                                + "     , tbrSAGECodeTransport.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGECodeTransportLibelle.ToString()].Name + "]"
                                + "     , CodeListeVerte as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitCodeListeVerte.ToString()].Name + "]"
                                + "     , Co2KgParT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitCo2KgParT.ToString()].Name + "]"
                                + "     , tblApplicationProduitOrigine.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ApplicationProduitOrigineLibelle.ToString()].Name + "]"
                                + "     , tbrProduitGroupeReporting.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingLibelle.ToString()].Name + "]"
                                + "     , IncitationQualite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.IncitationQualite.ToString()].Name + "]"
                                + "     , PUHTSurtri as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitPUHTSurtri.ToString()].Name + "]"
                                + "     , PUHTTransport as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitPUHTTransport.ToString()].Name + "]"
                                + "     , dbo.ListeComposant(tblProduit.RefProduit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeComposant.ToString()].Name + "]"
                                + "     , dbo.ListeProduitStandard(tblProduit.RefProduit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeProduitStandard.ToString()].Name + "]"
                                + "     , CodeEE as[" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitCodeEE.ToString()].Name + "]"
                                + "     , cast(case when cVQ.RefProduit is null then 0 else 1 end as bit) as[" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurCVQ.ToString()].Name + "]"
                                + "     , cast(case when cB.RefProduit is null then 0 else 1 end as bit) as[" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurCB.ToString()].Name + "]"
                                + "     , left(Cmt,500) as[" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitCmt.ToString()].Name + "]"
                                + " from tblProduit"
                                + "     left join tbrSAGECodeTransport on tblProduit.RefSAGECodeTransport = tbrSAGECodeTransport.RefSAGECodeTransport"
                                + "     left join tbrProduitGroupeReporting on tblProduit.RefProduitGroupeReporting = tbrProduitGroupeReporting.RefProduitGroupeReporting"
                                + "     left join tblApplicationProduitOrigine on tblProduit.RefApplicationProduitOrigine = tblApplicationProduitOrigine.RefApplicationProduitOrigine"
                                + "     left join (select distinct RefProduit from tbmDescriptionControleProduit) as cB on tblProduit.RefProduit=cB.RefProduit"
                                + "     left join(select distinct RefProduit from tbmDescriptionCVQProduit) as cVQ on tblProduit.RefProduit = cVQ.RefProduit"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ProduitLibelle, Enumerations.DataColumnName.ProduitNomCommercial, Enumerations.DataColumnName.ProduitNomCommun }
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ProduitCodeEE, Enumerations.DataColumnName.ProduitCodeListeVerte }
                                );
                            //Filters
                            if (filterProduits != "")
                            {
                                sqlStr += " and tblProduit.RefProduit in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (filterCollecte == "Collecte")
                            {
                                sqlStr += " and tblProduit.Collecte=1";
                            }
                            else if (filterCollecte == "HorsCollecte")
                            {
                                sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                            }
                            if (filterActif)
                            {
                                sqlStr += " and tblProduit.Actif=1";
                            }
                            break;
                        case "AdministrationMenuUtilisateur":
                            //Chaine SQL globale
                            sqlStr = "select tblUtilisateur.RefUtilisateur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Id.ToString()].Name + "]"
                                + "     , Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurNom.ToString()].Name + "]"
                                + "     , tblUtilisateur.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurActif.ToString()].Name + "]"
                                + "     , HabilitationAdministration as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationAdministration.ToString()].Name + "]"
                                + "     , HabilitationAnnuaire as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationAnnuaire.ToString()].Name + "]"
                                + "     , HabilitationLogistique as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationLogistique.ToString()].Name + "]"
                                + "     , Transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurTransporteur.ToString()].Name + "]"
                                + "     , HabilitationMessagerie as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationMessagerie.ToString()].Name + "]"
                                + "     , case when centredetri.CodeEE is not null then centredetri.CodeEE + ' - ' else '' end + centredetri.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurCentreDeTri.ToString()].Name + "]"
                                + "     , HabilitationModuleCollectivite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationModuleCollectivite.ToString()].Name + "]"
                                + "     , case when collectivite.CodeEE is not null then collectivite.CodeEE + ' - ' else '' end + collectivite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurCollectivite.ToString()].Name + "]"
                                + "     , HabilitationQualite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationQualite.ToString()].Name + "]"
                                + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurClient.ToString()].Name + "]"
                                + "     , HabilitationModulePrestataire as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationModulePrestataire.ToString()].Name + "]"
                                + "     , prestataire.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurPrestataire.ToString()].Name + "]"
                                + " from tblUtilisateur"
                                + "     left join tblEntite as client on tblUtilisateur.RefClient=client.RefEntite"
                                + "     left join tblEntite as collectivite on tblUtilisateur.RefCollectivite=collectivite.RefEntite"
                                + "     left join tblEntite as centredetri on tblUtilisateur.RefCentreDeTri=centredetri.RefEntite"
                                + "     left join tblEntite as transporteur on tblUtilisateur.RefTransporteur=transporteur.RefEntite"
                                + "     left join tblEntite as prestataire on tblUtilisateur.RefPrestataire=prestataire.RefEntite"
                                + " where tblUtilisateur.RefUtilisateur<>1";
                            //General Filters
                            if (!string.IsNullOrEmpty(filterText))
                            {
                                sqlStr += " and (tblUtilisateur.Nom COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or tblUtilisateur.EMail COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or transporteur.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or centredetri.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or centredetri.CodeEE = @filterText"
                                    + " or collectivite.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or collectivite.CodeEE = @filterText"
                                    + " or client.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or prestataire.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI)";
                                cmd.Parameters.Add("@filterText", SqlDbType.NVarChar).Value = filterText;
                            }
                            if (filterActif)
                            {
                                sqlStr += " and tblUtilisateur.Actif=1";
                            }
                            if (!string.IsNullOrEmpty(filterUtilisateurMaitres))
                            {
                                sqlStr += " and TblUtilisateur.RefUtilisateurMaitre in(";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refUtilisateurMaitre", filterUtilisateurMaitres, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            break;
                        case "AdministrationMenuUtilisateurInactif":
                            //Chaine SQL globale
                            sqlStr = "select tblUtilisateur.RefUtilisateur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Id.ToString()].Name + "]"
                                + "     , Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurNom.ToString()].Name + "]"
                                + "     , tblUtilisateur.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurActif.ToString()].Name + "]"
                                + "     , HabilitationAdministration as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationAdministration.ToString()].Name + "]"
                                + "     , HabilitationAnnuaire as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationAnnuaire.ToString()].Name + "]"
                                + "     , HabilitationLogistique as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationLogistique.ToString()].Name + "]"
                                + "     , Transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurTransporteur.ToString()].Name + "]"
                                + "     , HabilitationMessagerie as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationMessagerie.ToString()].Name + "]"
                                + "     , case when centredetri.CodeEE is not null then centredetri.CodeEE + ' - ' else '' end + centredetri.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurCentreDeTri.ToString()].Name + "]"
                                + "     , HabilitationModuleCollectivite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationModuleCollectivite.ToString()].Name + "]"
                                + "     , case when collectivite.CodeEE is not null then collectivite.CodeEE + ' - ' else '' end + collectivite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurCollectivite.ToString()].Name + "]"
                                + "     , HabilitationQualite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationQualite.ToString()].Name + "]"
                                + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurClient.ToString()].Name + "]"
                                + "     , HabilitationModulePrestataire as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurHabilitationModulePrestataire.ToString()].Name + "]"
                                + "     , prestataire.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurPrestataire.ToString()].Name + "]"
                                + " from tblUtilisateur"
                                + "     left join tblEntite as client on tblUtilisateur.RefClient=client.RefEntite"
                                + "     left join tblEntite as collectivite on tblUtilisateur.RefCollectivite=collectivite.RefEntite"
                                + "     left join tblEntite as centredetri on tblUtilisateur.RefCentreDeTri=centredetri.RefEntite"
                                + "     left join tblEntite as transporteur on tblUtilisateur.RefTransporteur=Transporteur.RefEntite"
                                + "     left join tblEntite as prestataire on tblUtilisateur.RefPrestataire=prestataire.RefEntite"
                                + " where tblUtilisateur.RefUtilisateur<>1 and tblUtilisateur.Actif=1";
                            //General Filters
                            if (!string.IsNullOrEmpty(filterText))
                            {
                                sqlStr += " and (tblUtilisateur.Nom COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or transporteur.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or centredetri.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or centredetri.CodeEE = @filterText"
                                    + " or collectivite.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or collectivite.CodeEE = @filterText"
                                    + " or client.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                                    + " or prestataire.Libelle COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI)";
                                cmd.Parameters.Add("@filterText", SqlDbType.NVarChar).Value = filterText;
                            }
                            if (!string.IsNullOrWhiteSpace(filterBegin))
                            {
                                DateTime begin = DateTime.MinValue;
                                DateTime.TryParse(filterBegin, out begin);
                                if (begin != DateTime.MinValue)
                                {
                                    cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                                    sqlStr += " and RefUtilisateur not in (select distinct RefUtilisateur from tblLog where DLogin > @begin)"
                                        + " and tblUtilisateur.DCreation < @begin";
                                }
                                else { sqlStr += " and 1!=1"; }
                            }
                            else { sqlStr += " and 1!=1"; }
                            break;
                        case "AdministrationMenuMontantIncitationQualite":
                            //Chaine SQL globale
                            sqlStr = "select RefMontantIncitationQualite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefMontantIncitationQualite.ToString()].Name + "]"
                                + "     , Annee as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnneeNb.ToString()].Name + "]"
                                + "     , Montant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Montant.ToString()].Name + "]"
                                + " from tbrMontantIncitationQualite";
                            break;
                        case "AdministrationMenuClientApplication":
                            //Chaine SQL globale
                            sqlStr = "select RefClientApplication as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefClientApplication.ToString()].Name + "]"
                                + "     , tblClientApplication.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Annee.ToString()].Name + "]"
                                + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                                + "     , tblApplicationProduitOrigine.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ApplicationProduitOrigineLibelle.ToString()].Name + "]"
                                + " from tblClientApplication"
                                + "     inner join tblEntite on tblClientApplication.RefEntite=tblEntite.RefEntite"
                                + "     left join tblApplicationProduitOrigine on tblClientApplication.RefApplicationProduitOrigine=tblApplicationProduitOrigine.RefApplicationProduitOrigine"
                                + " where 1=1";
                            //General Filters
                            if (!string.IsNullOrEmpty(filterClients))
                            {
                                sqlStr += " and tblClientApplication.RefEntite in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", filterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterApplicationProduitOrigines))
                            {
                                sqlStr += " and tblApplicationProduitOrigine.RefApplicationProduitOrigine in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refApplicationProduitOrigine", filterApplicationProduitOrigines, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterYears))
                            {
                                sqlStr += " and year(tblClientApplication.D) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refAnnee", filterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            break;
                        case "AdministrationMenuDescriptionControle":
                            //Chaine SQL globale
                            sqlStr = "select tbrDescriptionControle.RefDescriptionControle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefDescriptionControle.ToString()].Name + "]"
                                + "     , tblProduit.RefProduit as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefProduit.ToString()].Name + "]"
                                + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                + "     , tbrDescriptionControle.Ordre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionControleOrdre.ToString()].Name + "]"
                                + "     , tbrDescriptionControle.LibelleFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionControleLibelleFRFR.ToString()].Name + "]"
                                + "     , tbrDescriptionControle.LibelleENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionControleLibelleENGB.ToString()].Name + "]"
                                + "     , tbrDescriptionControle.CalculLimiteConformite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionControleCalculLimiteConformite.ToString()].Name + "]"
                                + "     , tbrDescriptionControle.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionControleActif.ToString()].Name + "]"
                                + " from tbrDescriptionControle"
                                + "     left join tbmDescriptionControleProduit on tbrDescriptionControle.RefDescriptionControle=tbmDescriptionControleProduit.RefDescriptionControle"
                                + "     left join tblProduit on tbmDescriptionControleProduit.RefProduit=tblProduit.RefProduit"
                                + " where 1=1";
                            //General Filters
                            if (!string.IsNullOrEmpty(filterText))
                            {
                                sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                    , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.DescriptionControleLibelleFRFR, Enumerations.DataColumnName.DescriptionControleLibelleENGB }
                                    );
                            }
                            if (!string.IsNullOrEmpty(filterProduits))
                            {
                                sqlStr += " and tbmDescriptionControleProduit.Refproduit in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (filterActif)
                            {
                                sqlStr += " and tbrDescriptionControle.Actif=1";
                            }
                            break;
                        case "AdministrationMenuDescriptionCVQ":
                            //Chaine SQL globale
                            sqlStr = "select tbrDescriptionCVQ.RefDescriptionCVQ as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefDescriptionCVQ.ToString()].Name + "]"
                                + "     , tblProduit.RefProduit as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefProduit.ToString()].Name + "]"
                                + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                                + "     , tbrDescriptionCVQ.Ordre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionCVQOrdre.ToString()].Name + "]"
                                + "     , tbrDescriptionCVQ.LibelleFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionCVQLibelleFRFR.ToString()].Name + "]"
                                + "     , tbrDescriptionCVQ.LibelleENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionCVQLibelleENGB.ToString()].Name + "]"
                                + "     , tbrDescriptionCVQ.LimiteBasse as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionCVQLimiteBasse.ToString()].Name + "]"
                                + "     , tbrDescriptionCVQ.LimiteHaute as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionCVQLimiteHaute.ToString()].Name + "]"
                                + "     , tbrDescriptionCVQ.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionCVQActif.ToString()].Name + "]"
                                + " from tbrDescriptionCVQ"
                                + "     left join tbmDescriptionCVQProduit on tbrDescriptionCVQ.RefDescriptionCVQ=tbmDescriptionCVQProduit.RefDescriptionCVQ"
                                + "     left join tblProduit on tbmDescriptionCVQProduit.RefProduit=tblProduit.RefProduit"
                                + " where 1=1";
                            //General Filters
                            if (!string.IsNullOrEmpty(filterText))
                            {
                                sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                    , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.DescriptionCVQLibelleFRFR, Enumerations.DataColumnName.DescriptionCVQLibelleENGB }
                                    );
                            }
                            if (!string.IsNullOrEmpty(filterProduits))
                            {
                                sqlStr += " and tbmDescriptionCVQProduit.Refproduit in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (filterActif)
                            {
                                sqlStr += " and tbrDescriptionCVQ.Actif=1";
                            }
                            break;
                        case "AdministrationMenuDescriptionReception":
                            //Chaine SQL globale
                            sqlStr = "select tbrDescriptionReception.RefDescriptionReception as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefDescriptionReception.ToString()].Name + "]"
                                + "     , tbrDescriptionReception.Ordre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionReceptionOrdre.ToString()].Name + "]"
                                + "     , tbrDescriptionReception.LibelleFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionReceptionLibelleFRFR.ToString()].Name + "]"
                                + "     , tbrDescriptionReception.LibelleENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionReceptionLibelleENGB.ToString()].Name + "]"
                                + "     , tbrDescriptionReception.OuiFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionReceptionOuiFRFR.ToString()].Name + "]"
                                + "     , tbrDescriptionReception.OuiENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionReceptionOuiENGB.ToString()].Name + "]"
                                + "     , tbrDescriptionReception.NonFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionReceptionNonFRFR.ToString()].Name + "]"
                                + "     , tbrDescriptionReception.NonENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionReceptionNonENGB.ToString()].Name + "]"
                                + "     , tbrDescriptionReception.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DescriptionReceptionActif.ToString()].Name + "]"
                                + " from tbrDescriptionReception"
                                + " where 1=1";
                            //General Filters
                            if (!string.IsNullOrEmpty(filterText))
                            {
                                sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                    , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.DescriptionReceptionLibelleFRFR, Enumerations.DataColumnName.DescriptionReceptionLibelleENGB
                                        , Enumerations.DataColumnName.DescriptionReceptionOuiFRFR, Enumerations.DataColumnName.DescriptionReceptionOuiENGB
                                        , Enumerations.DataColumnName.DescriptionReceptionNonFRFR, Enumerations.DataColumnName.DescriptionReceptionNonENGB }
                                    );
                            }
                            break;
                        case "AdministrationMenuEcoOrganisme":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefEcoOrganisme, Enumerations.DataColumnName.EcoOrganismeLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EcoOrganismeLibelle });
                            break;
                        case "AdministrationMenuEntiteType":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefEntiteType, Enumerations.DataColumnName.EntiteTypeLibelleFRFR, Enumerations.DataColumnName.EntiteTypeLibelleENGB });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteTypeLibelleFRFR, Enumerations.DataColumnName.EntiteTypeLibelleENGB });
                            break;
                        case "AdministrationMenuPays":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefPays, Enumerations.DataColumnName.PaysLibelle, Enumerations.DataColumnName.PaysActif, Enumerations.DataColumnName.PaysCodeISO, Enumerations.DataColumnName.RegionReportingLibelle }, false);
                            sqlStr += " from tbrPays"
                                + "    left join tbrRegionReporting on tbrPays.RefRegionReporting = tbrRegionReporting.RefRegionReporting"
                                + " where 1=1";
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.PaysLibelle, Enumerations.DataColumnName.RegionReportingLibelle }
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.PaysCodeISO }
                                );
                            if (!string.IsNullOrEmpty(filterPayss))
                            {
                                sqlStr += " and tbrPays.RefPays in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refPays", filterPayss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterRegionReportings))
                            {
                                sqlStr += " and tbrRegionReporting.RefRegionReporting in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refRegionReporting", filterRegionReportings, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            break;
                        case "AdministrationMenuProduitGroupeReportingType":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefProduitGroupeReportingType, Enumerations.DataColumnName.ProduitGroupeReportingTypeLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ProduitGroupeReportingTypeLibelle });
                            break;
                        case "AdministrationMenuProduitGroupeReporting":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext
                                , new List<Enumerations.DataColumnName> {
                                    Enumerations.DataColumnName.RefProduitGroupeReporting
                                    , Enumerations.DataColumnName.ProduitGroupeReportingLibelle
                                    , Enumerations.DataColumnName.ProduitGroupeReportingTypeLibelle
                                    , Enumerations.DataColumnName.ProduitGroupeReportingCouleur
                                }, false);
                            sqlStr += " from tbrProduitGroupeReporting"
                                + "    left join tbrProduitGroupeReportingType on tbrProduitGroupeReporting.RefProduitGroupeReportingType = tbrProduitGroupeReportingType.RefProduitGroupeReportingType"
                                + " where 1=1";
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> {
                                    Enumerations.DataColumnName.ProduitGroupeReportingLibelle,
                                    Enumerations.DataColumnName.ProduitGroupeReportingTypeLibelle
                                });
                            break;
                        case "AdministrationMenuRegionReporting":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefRegionReporting, Enumerations.DataColumnName.RegionReportingLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RegionReportingLibelle });
                            break;
                        case "AdministrationMenuRepreneur":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefRepreneur, Enumerations.DataColumnName.RepreneurLibelle, Enumerations.DataColumnName.RepreneurContrat });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RepreneurLibelle });
                            break;
                        case "AdministrationMenuRepriseType":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefRepriseType, Enumerations.DataColumnName.RepriseTypeLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RepriseTypeLibelle });
                            break;
                        case "AdministrationMenuRessource":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefRessource, Enumerations.DataColumnName.RessourceLibelleFRFR, Enumerations.DataColumnName.RessourceLibelleENGB });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RessourceLibelleFRFR, Enumerations.DataColumnName.RessourceLibelleENGB });
                            break;
                        case "AdministrationMenuFormeContact":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefFormeContact, Enumerations.DataColumnName.FormeContactLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.FormeContactLibelle });
                            break;
                        case "AdministrationMenuModeTransportEE":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefModeTransportEE, Enumerations.DataColumnName.ModeTransportEELibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ModeTransportEELibelle });
                            break;
                        case "AdministrationMenuMotifAnomalieChargement":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefMotifAnomalieChargement, Enumerations.DataColumnName.MotifAnomalieChargementLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.MotifAnomalieChargementLibelle });
                            break;
                        case "AdministrationMenuMotifAnomalieClient":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefMotifAnomalieClient, Enumerations.DataColumnName.MotifAnomalieClientLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.MotifAnomalieClientLibelle });
                            break;
                        case "AdministrationMenuMotifAnomalieTransporteur":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefMotifAnomalieTransporteur, Enumerations.DataColumnName.MotifAnomalieTransporteurLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.MotifAnomalieTransporteurLibelle });
                            break;
                        case "AdministrationMenuMotifCamionIncomplet":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefMotifCamionIncomplet, Enumerations.DataColumnName.MotifCamionIncompletLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.MotifCamionIncompletLibelle });
                            break;
                        case "AdministrationMenuCamionType":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefCamionType
                                , Enumerations.DataColumnName.CamionTypeLibelle
                                , Enumerations.DataColumnName.ModeTransportEELibelle}
                                , false);
                            sqlStr += " from tbrCamionType"
                                + "    left join tbrModeTransportEE on tbrCamionType.RefModeTransportEE = tbrModeTransportEE.RefModeTransportEE"
                                + " where 1=1";
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.CamionTypeLibelle });
                            break;
                        case "AdministrationMenuJourFerie":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefJourFerie, Enumerations.DataColumnName.JourFerie });
                            sqlStr = Utils.Utils.CreateSQLYearFilter(CurrentContext, cmd, sqlStr, filterYears, Enumerations.DataColumnName.JourFerie);
                            break;
                        case "AdministrationMenuMessageType":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefMessageType, Enumerations.DataColumnName.MessageTypeLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.MessageTypeLibelle });
                            break;
                        case "AdministrationMenuNonConformiteDemandeClientType":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> {
                                Enumerations.DataColumnName.RefNonConformiteDemandeClientType, Enumerations.DataColumnName.NonConformiteDemandeClientTypeActif,
                                Enumerations.DataColumnName.NonConformiteDemandeClientTypeLibelleFRFR, Enumerations.DataColumnName.NonConformiteDemandeClientTypeLibelleENGB});
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText,
                                new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.NonConformiteDemandeClientTypeLibelleFRFR, Enumerations.DataColumnName.NonConformiteDemandeClientTypeLibelleENGB });
                            break;
                        case "AdministrationMenuNonConformiteFamille":
                            //Chaine SQL globale
                            sqlStr = "select tbrNonConformiteFamille.RefNonConformiteFamille as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefNonConformiteFamille.ToString()].Name + "]"
                                + "     , tbrNonConformiteFamille.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteFamilleActif.ToString()].Name + "]"
                                + "     , tbrNonConformiteFamille.IncitationQualite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteFamilleIncitationQualite.ToString()].Name + "]"
                                + "     , tbrNonConformiteFamille.LibelleFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteFamilleLibelleFRFR.ToString()].Name + "]"
                                + "     , tbrNonConformiteFamille.LibelleENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteFamilleLibelleENGB.ToString()].Name + "]"
                                + " from tbrNonConformiteFamille"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.NonConformiteFamilleLibelleFRFR, Enumerations.DataColumnName.NonConformiteFamilleLibelleENGB }
                                );
                            break;
                        case "AdministrationMenuNonConformiteNature":
                            //Chaine SQL globale
                            sqlStr = "select tbrNonConformiteNature.RefNonConformiteNature as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefNonConformiteNature.ToString()].Name + "]"
                                + "     , tbrNonConformiteNature.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteNatureActif.ToString()].Name + "]"
                                + "     , tbrNonConformiteNature.IncitationQualite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteNatureIncitationQualite.ToString()].Name + "]"
                                + "     , tbrNonConformiteNature.LibelleFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteNatureLibelleFRFR.ToString()].Name + "]"
                                + "     , tbrNonConformiteNature.LibelleENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteNatureLibelleENGB.ToString()].Name + "]"
                                + " from tbrNonConformiteNature"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.NonConformiteNatureLibelleFRFR, Enumerations.DataColumnName.NonConformiteNatureLibelleENGB }
                                );
                            break;
                        case "AdministrationMenuNonConformiteReponseClientType":
                            //Chaine SQL globale
                            sqlStr = "select tbrNonConformiteReponseClientType.RefNonConformiteReponseClientType as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefNonConformiteReponseClientType.ToString()].Name + "]"
                                + "     , tbrNonConformiteReponseClientType.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteReponseClientTypeActif.ToString()].Name + "]"
                                + "     , tbrNonConformiteReponseClientType.LibelleFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteReponseClientTypeLibelleFRFR.ToString()].Name + "]"
                                + "     , tbrNonConformiteReponseClientType.LibelleENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteReponseClientTypeLibelleENGB.ToString()].Name + "]"
                                + " from tbrNonConformiteReponseClientType"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.NonConformiteReponseClientTypeLibelleFRFR, Enumerations.DataColumnName.NonConformiteReponseClientTypeLibelleENGB }
                                );
                            break;
                        case "AdministrationMenuNonConformiteReponseFournisseurType":
                            //Chaine SQL globale
                            sqlStr = "select tbrNonConformiteReponseFournisseurType.RefNonConformiteReponseFournisseurType as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefNonConformiteReponseFournisseurType.ToString()].Name + "]"
                                + "     , tbrNonConformiteReponseFournisseurType.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteReponseFournisseurTypeActif.ToString()].Name + "]"
                                + "     , tbrNonConformiteReponseFournisseurType.LibelleFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteReponseFournisseurTypeLibelleFRFR.ToString()].Name + "]"
                                + "     , tbrNonConformiteReponseFournisseurType.LibelleENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteReponseFournisseurTypeLibelleENGB.ToString()].Name + "]"
                                + " from tbrNonConformiteReponseFournisseurType"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.NonConformiteReponseFournisseurTypeLibelleFRFR, Enumerations.DataColumnName.NonConformiteReponseFournisseurTypeLibelleENGB }
                                );
                            break;
                        case "AdministrationMenuCivilite":
                            //Chaine SQL globale
                            sqlStr = "select tbrCivilite.RefCivilite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCivilite.ToString()].Name + "]"
                                + "     , tbrCivilite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CiviliteLibelle.ToString()].Name + "]"
                                + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysLibelle.ToString()].Name + "]"
                                + " from tbrCivilite"
                                + "     left join tbrPays on tbrCivilite.RefPays=tbrPays.RefPays"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText,
                                new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.CiviliteLibelle, Enumerations.DataColumnName.PaysLibelle });
                            break;
                        case "AdministrationMenuActionType":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefActionType, Enumerations.DataColumnName.ActionTypeLibelle, Enumerations.DataColumnName.ActionTypeDocumentType });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ActionTypeLibelle });
                            break;
                        case "AdministrationMenuApplication":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefApplication, Enumerations.DataColumnName.ApplicationLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ApplicationLibelle });
                            break;
                        case "AdministrationMenuApplicationProduitOrigine":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefApplicationProduitOrigine, Enumerations.DataColumnName.ApplicationProduitOrigineLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ApplicationProduitOrigineLibelle });
                            break;
                        case "AdministrationMenuAdresseType":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefAdresseType, Enumerations.DataColumnName.AdresseTypeLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.AdresseTypeLibelle });
                            break;
                        case "AdministrationMenuEquipementier":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefEquipementier, Enumerations.DataColumnName.EquipementierLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EquipementierLibelle });
                            break;
                        case "AdministrationMenuDocument":
                            sqlStr = "select tblDocument.RefDocument as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Id.ToString()].Name + "]"
                                + "     , tblDocument.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DocumentActif.ToString()].Name + "]"
                                + "     , tblDocument.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DocumentLibelle.ToString()].Name + "]"
                                + "     , tblDocument.Description as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DocumentDescription.ToString()].Name + "]"
                                + " from tblDocument"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.DocumentLibelle, Enumerations.DataColumnName.DocumentDescription }
                                );
                            break;
                        case "AdministrationMenuEquivalentCO2":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> {
                                Enumerations.DataColumnName.RefEquivalentCO2
                                , Enumerations.DataColumnName.EquivalentCO2Libelle
                                , Enumerations.DataColumnName.EquivalentCO2Ordre
                                , Enumerations.DataColumnName.EquivalentCO2Ratio
                                , Enumerations.DataColumnName.EquivalentCO2Actif
                            });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EquivalentCO2Libelle });
                            break;
                        case "AdministrationMenuFonction":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefFonction, Enumerations.DataColumnName.FonctionLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.FonctionLibelle });
                            break;
                        case "AdministrationMenuFournisseurTO":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefFournisseurTO, Enumerations.DataColumnName.FournisseurTOLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.FournisseurTOLibelle });
                            break;
                        case "AdministrationMenuParamEmail":
                            //Chaine SQL globale
                            sqlStr = "select tbsParamEmail.RefParamEmail as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefParamEmail.ToString()].Name + "]"
                                + "     , tbsParamEmail.ComptePOP as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ParamEmailComptePOP.ToString()].Name + "]"
                                + "     , tbsParamEmail.EmailExpediteur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ParamEmailEmailExpediteur.ToString()].Name + "]"
                                + "     , tbsParamEmail.LibelleExpediteur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ParamEmailLibelleExpediteur.ToString()].Name + "]"
                                + "     , tbsParamEmail.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ParamEmailActif.ToString()].Name + "]"
                                + " from tbsParamEmail"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ParamEmailComptePOP, Enumerations.DataColumnName.ParamEmailEmailExpediteur, Enumerations.DataColumnName.ParamEmailLibelleExpediteur }
                                );
                            break;
                        case "AdministrationMenuAide":
                            ////Chaine SQL globale
                            sqlStr = "select tblAide.RefAide as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefAide.ToString()].Name + "]";
                            if(CurrentContext.CurrentCulture.Name == "en-GB")
                            {
                                sqlStr += "     , tblAide.LibelleENGB as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AideLibelleENGB.ToString()].Name + "]";
                            }
                            else
                            {
                                sqlStr += "     , tblAide.LibelleFRFR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AideLibelleFRFR.ToString()].Name + "]";
                            }
                            sqlStr+= "     , tblAide.ValeurHTML as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AideValeurHTML.ToString()].Name + "]"
                                + " from tblAide"
                                + " where 1=1";
                            if (CurrentContext.CurrentCulture.Name == "en-GB")
                            {
                                sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.AideLibelleENGB });
                            }
                            else
                            {
                                sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.AideLibelleFRFR });
                            }
                            break;
                        case "AdministrationMenuParametre":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> {
                                Enumerations.DataColumnName.RefParametre
                                , Enumerations.DataColumnName.ParametreLibelle
                                , Enumerations.DataColumnName.ParametreValeurNumerique
                                , Enumerations.DataColumnName.ParametreValeurTexte
                            });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ParametreLibelle });
                            break;
                        case "AdministrationMenuProcess":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefProcess, Enumerations.DataColumnName.ProcessLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ProcessLibelle });
                            break;
                        case "AdministrationMenuRegionEE":
                            //Chaine SQL globale
                            sqlStr = "select tbrRegionEE.RefRegionEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefRegionEE.ToString()].Name + "]"
                                + "     , tbrRegionEE.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RegionEELibelle.ToString()].Name + "]"
                                + " from tbrRegionEE"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RegionEELibelle });
                            break;
                        case "AdministrationMenuSAGECodeTransport":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefSAGECodeTransport, Enumerations.DataColumnName.SAGECodeTransportCode, Enumerations.DataColumnName.SAGECodeTransportLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.SAGECodeTransportCode, Enumerations.DataColumnName.SAGECodeTransportLibelle });
                            break;
                        case "AdministrationMenuService":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefService, Enumerations.DataColumnName.ServiceLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.ServiceLibelle });
                            break;
                        case "AdministrationMenuStandard":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefStandard, Enumerations.DataColumnName.StandardLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.StandardLibelle });
                            break;
                        case "AdministrationMenuTicket":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.PaysLibelle, Enumerations.DataColumnName.RefTicket, Enumerations.DataColumnName.TicketLibelle, Enumerations.DataColumnName.TicketTexte }, false);
                            sqlStr += " from tblTicket"
                                + "    left join tbrPays on tblTicket.RefPays = tbrPays.RefPays"
                                + " where 1=1";
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.TicketLibelle, Enumerations.DataColumnName.TicketTexte });
                            break;
                        case "AdministrationMenuTitre":
                            sqlStr = Utils.Utils.CreateSQLSelectColumns(CurrentContext, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.RefTitre, Enumerations.DataColumnName.TitreLibelle });
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText, new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.TitreLibelle });
                            break;
                    }
                    break;
                case "Annuaire":
                    //Process if Authorized
                    switch (menu)
                    {
                        case "AnnuaireMenuEntite":
                            //Chaine SQL globale
                            sqlStr = "select tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Id.ToString()].Name + "]"
                                + "     , tbrEntiteType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTypeLibelle.ToString()].Name + "]"
                                + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                                + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                                + "     , tbrEcoOrganisme.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EcoOrganismeLibelle.ToString()].Name + "]"
                                + "     , Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteActif.ToString()].Name + "]";
                            if (filterDonneeEntiteSage)
                            {
                                sqlStr += "      , tblEntite.CodeTVA as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeTVA.ToString()].Name + "]"
                                    + "     , F_COMPTET.CT_Identifiant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEEntiteCodeTVA.ToString()].Name + "]"
                                    + "     , tblEntite.IdNational as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteIdNational.ToString()].Name + "]"
                                    + "     , F_COMPTET.CT_Siret as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEEntiteIdNational.ToString()].Name + "]";
                            }
                            sqlStr += " from tblEntite"
                                + "     left join tbrEntiteType on tblEntite.RefEntiteType=tbrEntiteType.RefEntiteType"
                                + "     left join tbrEcoOrganisme on tblEntite.RefEcoOrganisme=tbrEcoOrganisme.RefEcoOrganisme";
                            if (filterDonneeEntiteSage)
                            {
                                sqlStr += " inner join F_COMPTET on tblEntite.SAGECodeComptable=F_COMPTET.CT_Num";
                            }
                            sqlStr += " where 1=1 ";
                            if (filterDonneeEntiteSage)
                            {
                                sqlStr += " and ((rtrim(ltrim(isnull(F_COMPTET.CT_Siret,''))) != rtrim(ltrim(isnull(tblEntite.IdNational,''))))"
                                + "     or (rtrim(ltrim(isnull(F_COMPTET.CT_Identifiant,''))) != rtrim(ltrim(isnull(tblEntite.CodeTVA,'')))))";
                            }
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteLibelle }
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteCodeCITEO, Enumerations.DataColumnName.RefEntite }
                                );
                            if (!string.IsNullOrEmpty(filterEntiteTypes))
                            {
                                sqlStr += " and tblEntite.RefEntiteType in(";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refEntiteType", filterEntiteTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterEcoOrganismes))
                            {
                                sqlStr += " and tblEntite.RefEcoOrganisme in(";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refEcoOrganisme", filterEcoOrganismes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterDRs))
                            {
                                sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", filterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += "))";
                            }
                            if (CurrentContext.filterGlobalActif)
                            {
                                sqlStr += " and tblEntite.Actif=1";
                            }
                            if (CurrentContext.filterDR)
                            {
                                sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                            }
                            break;
                    }
                    break;
                case "Messagerie":
                    //Process if Authorized
                    switch (menu)
                    {
                        case "MessagerieMenuMessage":
                            //Chaine SQL globale
                            sqlStr = "select RefMessage as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefMessage.ToString()].Name + "]"
                                + "    , tbrMessageType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageTypeLibelle.ToString()].Name + "]"
                                + "    , tblMessage.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageLibelle.ToString()].Name + "]"
                                + "    , DDebut as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageDDebut.ToString()].Name + "]"
                                + "    , DFin as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageDFin.ToString()].Name + "]"
                                + " from tblMessage"
                                + "     left join tbrMessageType on tblMessage.RefMessageType=tbrMessageType.RefMessageType"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.MessageLibelle }
                                );
                            if (!string.IsNullOrEmpty(filterMessageTypes))
                            {
                                sqlStr += " and tblMessage.RefMessageType in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refMessageType", filterMessageTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterBegin) || !string.IsNullOrEmpty(filterEnd))
                            {
                                DateTime begin = DateTime.MinValue;
                                DateTime end = DateTime.MinValue;
                                if (!string.IsNullOrEmpty(filterBegin)) { DateTime.TryParse(filterBegin, out begin); }
                                if (!string.IsNullOrEmpty(filterEnd)) { DateTime.TryParse(filterEnd, out end); }
                                if (begin != DateTime.MinValue) { cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin; }
                                if (end != DateTime.MinValue) { cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end; }
                                if (begin != DateTime.MinValue && end != DateTime.MinValue)
                                {
                                    sqlStr += " and DDebut between @begin and @end";
                                }
                                else if (begin != DateTime.MinValue)
                                {
                                    sqlStr += " and DDebut >= @begin";
                                }
                                else if (end != DateTime.MinValue)
                                {
                                    sqlStr += " and DDebut <= @end";
                                }
                            }
                            break;
                        case "MessagerieMenuVisualisation":
                            //Chaine SQL globale
                            sqlStr = "select tblMessage.RefMessage as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefMessage.ToString()].Name + "]"
                                + "     , D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageVisualisationD.ToString()].Name + "]"
                                + "     , Confirme as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageVisualisationConfirme.ToString()].Name + "]"
                                + "     , tblUtilisateur.Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageVisualisationPar.ToString()].Name + "]"
                                + "     , tbrEntiteType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTypeLibelle.ToString()].Name + "]"
                                + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                                + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                                + "     , tbrMessageType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageTypeLibelle.ToString()].Name + "]"
                                + "     , tblMessage.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageLibelle.ToString()].Name + "]"
                                + " from tblMessage"
                                + " 	inner join tbmMessageVisualisation on tbmMessageVisualisation.RefMessage=tblMessage.RefMessage"
                                + " 	left join tbrMessageType on tblMessage.RefMessageType=tbrMessageType.RefMessageType"
                                + " 	left join tblUtilisateur on tblUtilisateur.RefUtilisateur=tbmMessageVisualisation.RefUtilisateur"
                                + " 	left join tblEntite on tblUtilisateur.RefCollectivite=tblEntite.RefEntite or tblUtilisateur.RefClient=tblEntite.RefEntite or tblUtilisateur.RefTransporteur=tblEntite.RefEntite or tblUtilisateur.RefCentreDeTri=tblEntite.RefEntite"
                                + "     left join tbrEntiteType on tblEntite.RefEntiteType=tbrEntiteType.RefEntiteType"
                                + " where 1=1";
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.MessageLibelle, Enumerations.DataColumnName.EntiteLibelle, Enumerations.DataColumnName.UtilisateurNom }
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteCodeCITEO }
                                );
                            if (!string.IsNullOrEmpty(filterBegin) || !string.IsNullOrEmpty(filterEnd))
                            {
                                DateTime begin = DateTime.MinValue;
                                DateTime end = DateTime.MinValue;
                                if (!string.IsNullOrEmpty(filterBegin)) { DateTime.TryParse(filterBegin, out begin); }
                                if (!string.IsNullOrEmpty(filterEnd)) { DateTime.TryParse(filterEnd, out end); }
                                if (begin != DateTime.MinValue) { cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin; }
                                if (end != DateTime.MinValue) { cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end; }
                                if (begin != DateTime.MinValue && end != DateTime.MinValue)
                                {
                                    sqlStr += " and tbmMessageVisualisation.D >= @begin and tbmMessageVisualisation.D < @end";
                                }
                                else if (begin != DateTime.MinValue)
                                {
                                    sqlStr += " and tbmMessageVisualisation.D >= @begin";
                                }
                                else if (end != DateTime.MinValue)
                                {
                                    sqlStr += " and tbmMessageVisualisation.D < @end";
                                }
                            }
                            if (!string.IsNullOrEmpty(filterEntiteTypes))
                            {
                                sqlStr += " and tblEntite.RefEntiteType in(";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refEntiteType", filterEntiteTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterMessageTypes))
                            {
                                sqlStr += " and tblMessage.RefMessageType in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refMessageType", filterMessageTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            break;
                    }
                    break;
                case "Qualite":
                    //Process if Authorized
                    switch (menu)
                    {
                        case "QualiteMenuControle":
                            sqlStr = "select tblFicheControle.RefFicheControle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefFicheControle.ToString()].Name + "]"
                                + "     , NumeroCommande [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]";
                            if (CurrentContext.ConnectedUtilisateur.HabilitationQualite != Enumerations.HabilitationQualite.Client.ToString())
                            {
                                sqlStr += "     , dbo.ListeDR(fournisseur.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                                + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]";
                            }
                            sqlStr += "     , NumeroLotUsine as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleNumeroLotUsine.ToString()].Name + "]"
                             + "     , fournisseur.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                             + "     , fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                             + "     , tblProduit.Libelle AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                             + "     , DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                             + "     , PoidsDechargement AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsDechargement.ToString()].Name + "]"
                             + "     , NbBalle AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleDechargement.ToString()].Name + "]"
                             + "     , cast(1 as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleReception.ToString()].Name + "]"
                             + "     , cast(case when cvq.RefFicheControle is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleCVQ.ToString()].Name + "]"
                             + "     , cast(case when c.RefFicheControle is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleControle.ToString()].Name + "]"
                             + "     , Reserve AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleReserve.ToString()].Name + "]"
                             + "     , cast(case when r.RefCommandeFournisseur is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNonConformite.ToString()].Name + "]"
                             + " from tblCommandeFournisseur"
                             + "     inner join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                             + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                             + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                             + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                             + "     inner join tblFicheControle on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                             + "     left join tblUtilisateur on tblFicheControle.RefControleur=tblUtilisateur.RefUtilisateur"
                             + "     left join (select distinct RefCommandeFournisseur from tblNonConformite) as r on tblCommandeFournisseur.RefCommandeFournisseur=r.RefCommandeFournisseur"
                             + "     left join (select distinct RefFicheControle from tblControle) as c on tblFicheControle.RefFicheControle=c.RefFicheControle"
                             + "     left join (select distinct RefFicheControle from tblCVQ) as cvq on tblFicheControle.RefFicheControle=cvq.RefFicheControle"
                             + "     where 1=1";
                            //Filtre DR
                            if (CurrentContext.filterDR)
                            {
                                sqlStr += " and fournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                            }
                            if (!string.IsNullOrEmpty(filterDRs))
                            {
                                sqlStr += " and fournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", filterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += "))";
                            }
                            //Filter user
                            if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                            {
                                sqlStr += " and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                            }
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , null
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.CommandeFournisseurNumeroCommande }
                                );
                            if (!string.IsNullOrEmpty(filterCentreDeTris))
                            {
                                sqlStr += " and fournisseur.RefEntite in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refFournisseur", filterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterClients))
                            {
                                sqlStr += " and client.RefEntite in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", filterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterYears))
                            {
                                sqlStr += " and year(tblCommandeFournisseur.DDechargement) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refAnnee", filterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterMonths))
                            {
                                sqlStr += " and month(tblCommandeFournisseur.DDechargement) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refMois", filterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (filterControle)
                            {
                                sqlStr += " and c.RefFicheControle is not null";
                            }
                            if (!string.IsNullOrEmpty(filterProduits))
                            {
                                sqlStr += " and tblProduit.RefProduit in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            break;
                        case "QualiteMenuNonConformite":
                            sqlStr = "select tblNonConformite.RefNonConformite [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefNonConformite.ToString()].Name + "]"
                                + "     , NumeroCommande [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                                + "     , tblNonConformiteEtape.RefNonConformiteEtapeType as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefNonConformiteEtapeType.ToString()].Name + "]"
                                + "     , tblNonConformiteEtape.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteEtapeLibelle.ToString()].Name + "]";
                            if (CurrentContext.ConnectedUtilisateur.HabilitationQualite != Enumerations.HabilitationQualite.Client.ToString())
                            {
                                sqlStr += "     , dbo.ListeDR(fournisseur.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                                + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]";
                            }
                            else
                            {
                                sqlStr += "     , tblFicheControle.NumeroLotUsine as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleNumeroLotUsine.ToString()].Name + "]";
                            }
                            sqlStr += "     , fournisseur.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                            + "     , fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                            + "     , tblProduit.Libelle AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                            + "     , DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                            + "     , nonConformiteCreation.DCreation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteDateCreation.ToString()].Name + "]";
                            if (CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Client.ToString())
                            {
                                sqlStr += "     , tblNonConformite.DescrClient as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteDescrClient.ToString()].Name + "]";
                            }
                            sqlStr += "     , tblNonConformite.DescrValorplast as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteDescrValorplast.ToString()].Name + "]"
                            + "     , tbrNonConformiteNature.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteNatureLibelle.ToString()].Name + "]";
                            if (CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Administrateur.ToString())
                            {
                                sqlStr += "     , case when (IFFournisseurDescr is not null or IFFournisseurFactureMontant is not null or IFFournisseurDeductionTonnage is not null) and (IFClientDescr is not null or IFClientFactureMontant is not null) then '" + CurrentContext.CulturedRessources.GetTextRessource(16) + " / " + CurrentContext.CulturedRessources.GetTextRessource(17) + "'"
                                    + "         else case when IFClientDescr is not null or IFClientFactureMontant is not null then '" + CurrentContext.CulturedRessources.GetTextRessource(17) + "'"
                                    + "         else case when IFFournisseurDescr is not null or IFFournisseurFactureMontant is not null or IFFournisseurDeductionTonnage is not null then '" + CurrentContext.CulturedRessources.GetTextRessource(16) + "' else '' end end end"
                                    + "     as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIF.ToString()].Name + "]";
                            }
                            if (CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Administrateur.ToString()
                                || CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Fournisseur.ToString()
                                || CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Utilisateur.ToString())
                            {
                                sqlStr += "     , IFFournisseurFactureMontant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurFactureMontant.ToString()].Name + "]"
                                + "     , IFFournisseurRetourLot as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurRetourLot.ToString()].Name + "]";
                            }
                            if (CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Administrateur.ToString()
                                || CurrentContext.ConnectedUtilisateur.HabilitationQualite == Enumerations.HabilitationQualite.Client.ToString())
                            {
                                sqlStr += "     , IFClientFactureMontant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFClientFactureMontant.ToString()].Name + "]";
                            }
                            sqlStr += "     , cast(case when [CmtOrigineAction] is not null then 1 else 0 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteOriginePlanActionSaisi.ToString()].Name + "]"
                             + "     , cast(case when etape8Valide.RefNonConformite is not null then 1 else 0 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteClotureeInvisible.ToString()].Name + "]"
                             + " from tblCommandeFournisseur"
                             + "     inner join tblNonConformite on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                             + "     left join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                             + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                             + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                             + "     left join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                             + "     left join tblFicheControle on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                             + "     left join tbrNonConformiteNature on tblNonConformite.RefNonConformiteNature=tbrNonConformiteNature.RefNonConformiteNature"
                             + "     left join (select RefNonConformite, min(DCreation) as DCreation from tblNonConformiteEtape where RefNonConformiteEtapeType=1 group by RefNonConformite) as nonConformiteCreation on tblNonConformite.RefNonConformite=nonConformiteCreation.RefNonConformite"
                             + "     left join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                             + "     left join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite=Etape.RefNonConformite and tblNonConformiteEtape.Ordre=etape.Ordre"
                             + "     left join (select RefNonConformite from tblNonConformiteEtape where DValide is null and RefNonConformiteEtapeType= 5) as etape5 on etape5.RefNonConformite = tblNonConformite.RefNonConformite"
                             + "     left join (select RefNonConformite from tblNonConformiteEtape where DValide is not null and RefNonConformiteEtapeType= 8) as etape8Valide on etape8Valide.RefNonConformite = tblNonConformite.RefNonConformite"
                             + "     where 1=1";
                            //Filtre DR
                            if (CurrentContext.filterDR)
                            {
                                sqlStr += " and fournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                            }
                            if (!string.IsNullOrEmpty(filterDRs))
                            {
                                sqlStr += " and fournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", filterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += "))";
                            }
                            //Filter user
                            if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                            {
                                sqlStr += " and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                            }
                            //General Filters
                            sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                                , null
                                , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.CommandeFournisseurNumeroCommande }
                                );
                            if (!string.IsNullOrEmpty(filterCentreDeTris))
                            {
                                sqlStr += " and fournisseur.RefEntite in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refFournisseur", filterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterClients))
                            {
                                sqlStr += " and client.RefEntite in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", filterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterYears))
                            {
                                sqlStr += " and year(nonConformiteCreation.DCreation) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refAnnee", filterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterMonths))
                            {
                                sqlStr += " and month(nonConformiteCreation.DCreation) in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refMois", filterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterProduits))
                            {
                                sqlStr += " and tblProduit.RefProduit in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", filterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterNonConformiteEtapeTypes))
                            {
                                sqlStr += " and tblNonConformiteEtape.RefNonConformiteEtapeType in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refNonConformiteEtapeType", filterNonConformiteEtapeTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (!string.IsNullOrEmpty(filterNonConformiteNatures))
                            {
                                sqlStr += " and tblNonConformite.RefNonConformiteNature in (";
                                sqlStr += Utils.Utils.CreateSQLParametersFromString("refNonConformiteNature", filterNonConformiteNatures, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                sqlStr += ")";
                            }
                            if (filterIFFournisseur == true)
                            {
                                sqlStr += " and (IFFournisseurDescr is not null or IFFournisseurFactureMontant is not null or IFFournisseurDeductionTonnage is not null)";
                            }
                            if (filterIFClient == true)
                            {
                                sqlStr += " and (IFClientDescr is not null or IFClientFactureMontant is not null)";
                            }
                            if (filterIFFournisseurRetourLot == true)
                            {
                                sqlStr += " and (IFFournisseurRetourLot=1)";
                            }
                            if (filterIFFournisseurFacture == true)
                            {
                                sqlStr += " and (IFFournisseurFacture=1 and IFFournisseurFactureNro is null)";
                            }
                            if (filterIFFournisseurAttenteBonCommande == true)
                            {
                                sqlStr += " and (IFFournisseurAttenteBonCommande=1 and IFFournisseurBonCommandeNro is null)";
                            }
                            if (filterIFFournisseurTransmissionFacturation == true)
                            {
                                sqlStr += " and (IFFournisseurTransmissionFacturation=1 and IFFournisseurFactureNro is null)";
                            }
                            if (filterNonConformitesATraiter == true)
                            {
                                sqlStr += " and (tblNonConformiteEtape.RefNonConformiteEtapeType = 3 and RefNonConformiteReponseFournisseurType is null and DTransmissionFournisseur is not null)";
                            }
                            if (filterNonConformitesATransmettre == true)
                            {
                                sqlStr += " and (tblNonConformiteEtape.RefNonConformiteEtapeType = 3 and DTransmissionFournisseur is null)";
                            }
                            if (filterNonConformitesPlanActionAValider == true)
                            {
                                sqlStr += " and (etape5.RefNonConformite is not null and CmtOrigineAction is not null)";
                            }
                            if (filterNonConformitesPlanActionNR == true)
                            {
                                sqlStr += " and (CmtOrigineAction is null)";
                            }
                            break;
                    }
                    break;
            }
            //Chargement des données si elles existent
            if (!string.IsNullOrEmpty(sqlStr))
            {
                var nbRow = "0";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    //Calculate augmentation if needed
                    if (menu == Enumerations.MenuName.LogistiqueMenuModifierTousPrixTransport.ToString() && !string.IsNullOrEmpty(augmentation))
                    {
                        decimal augm = 0;
                        if (decimal.TryParse(augmentation, out augm))
                        {
                            if (augm != 0)
                            {
                                cmd.CommandText = "update tblTransport set PUHTDemande=cast((tblTransport.PUHT * @evolution) as decimal(10,0)), DModif=getdate(), RefUtilisateurModif=" + CurrentContext.RefUtilisateur.ToString()
                                    + " from tblTransport inner join (" + sqlStr + ") as univers on univers." + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefTransport.ToString()].Name + "=tblTransport.RefTransport";
                                cmd.Parameters.Add("@evolution", SqlDbType.Decimal).Value = (1 + augm / 100);
                                int res = cmd.ExecuteNonQuery();
                                //Return nb of row in header
                                Response.Headers.Add("nbAugmentation", res.ToString());
                            }
                        }
                    }
                    //Deactivate users if needed
                    if (menu == Enumerations.MenuName.AdministrationMenuUtilisateurInactif.ToString() && action == Enumerations.ActionName.DeactivateUtilisateur.ToString())
                    {
                        if (!string.IsNullOrEmpty(selectedItem))
                        {
                            cmd.CommandText = "update tblUtilisateur set Actif=0, DModif=getdate(), RefUtilisateurModif=" + CurrentContext.RefUtilisateur.ToString()
                            + " where RefUtilisateur in (";
                            cmd.CommandText += Utils.Utils.CreateSQLParametersFromString("refUtilisateur", selectedItem, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                            cmd.CommandText += ")";
                            int res = cmd.ExecuteNonQuery();
                            //Return nb of affected rows in header
                            Response.Headers.Add("nbDeactivateUtilisateur", res.ToString());
                        }
                    }
                    //Validate prices if needed
                    if (menu == Enumerations.MenuName.LogistiqueMenuTransportNonValide.ToString() && action == Enumerations.ActionName.ValidateTransportPrice.ToString())
                    {
                        if (!string.IsNullOrEmpty(selectedItem))
                        {
                            if (selectedItem == "all")
                            {
                                cmd.CommandText = "update tblTransport set PUHT=PUHTDemande, PUHTDemande=null, DModif=getdate(), RefUtilisateurModif=" + CurrentContext.RefUtilisateur.ToString()
                                + " where PUHTDemande is not null"
                                + " and tblTransport.RefTransport in (select [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefTransport.ToString()].Name + "] from (" + sqlStr + ") as univers)";
                            }
                            else
                            {
                                cmd.CommandText = "update tblTransport set PUHT=PUHTDemande, PUHTDemande=null, DModif=getdate(), RefUtilisateurModif=" + CurrentContext.RefUtilisateur.ToString()
                                + " where PUHTDemande is not null"
                                + " and tblTransport.RefTransport in (";
                                cmd.CommandText += Utils.Utils.CreateSQLParametersFromString("refTransport", selectedItem, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                cmd.CommandText += ")";
                            }
                            int res = cmd.ExecuteNonQuery();
                            //Return nb of affected rows in header
                            Response.Headers.Add("nbValidatedPrices", res.ToString());
                        }
                    }
                    //Reject prices if needed
                    if (menu == Enumerations.MenuName.LogistiqueMenuTransportNonValide.ToString() && action == Enumerations.ActionName.RejectTransportPrice.ToString())
                    {
                        if (!string.IsNullOrEmpty(selectedItem))
                        {
                            if (selectedItem == "all")
                            {
                                cmd.CommandText = "update tblTransport set PUHTDemande=null, DModif=getdate(), RefUtilisateurModif=" + CurrentContext.RefUtilisateur.ToString()
                                + " where PUHTDemande is not null"
                                + " and tblTransport.RefTransport in (select [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefTransport.ToString()].Name + "] from (" + sqlStr + ") as univers)";
                            }
                            else
                            {
                                cmd.CommandText = "update tblTransport set PUHTDemande=null, DModif=getdate(), RefUtilisateurModif=" + CurrentContext.RefUtilisateur.ToString()
                                + " where PUHTDemande is not null"
                                + " and tblTransport.RefTransport in (";
                                cmd.CommandText += Utils.Utils.CreateSQLParametersFromString("refTransport", selectedItem, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                cmd.CommandText += ")";
                            }
                            int res = cmd.ExecuteNonQuery();
                            //Return nb of affected rows in header
                            Response.Headers.Add("nbRejectedPrices", res.ToString());
                        }
                    }
                    //Delete prices if needed
                    if (menu == Enumerations.MenuName.LogistiqueMenuSupprimerTransportEnMasse.ToString() && action == Enumerations.ActionName.DeleteTransport.ToString())
                    {
                        if (!string.IsNullOrEmpty(selectedItem))
                        {
                            if (selectedItem == "all")
                            {
                                cmd.CommandText = "delete from tblTransport"
                                + " where tblTransport.RefTransport in (select [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefTransport.ToString()].Name + "] from (" + sqlStr + ") as univers)";
                            }
                            else
                            {
                                cmd.CommandText = "delete from tblTransport"
                                + " where tblTransport.RefTransport in (";
                                cmd.CommandText += Utils.Utils.CreateSQLParametersFromString("refTransport", selectedItem, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                                cmd.CommandText += ")";
                            }
                            int res = cmd.ExecuteNonQuery();
                            //Delete orphan parcours
                            cmd.CommandText = "delete from tblParcours from tblParcours left join tblTransport on tblParcours.RefParcours = tblTransport.RefParcours where tblTransport.RefParcours is null";
                            cmd.ExecuteNonQuery();
                            //Return nb of affected rows in header
                            Response.Headers.Add("nbDeletedPrices", res.ToString());
                        }
                    }
                    //Set EnAttente if needed
                    if (menu == Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString() && action == Enumerations.ActionName.SetEnAttente.ToString())
                    {
                        if (!string.IsNullOrEmpty(selectedItem))
                        {
                            cmd.CommandText = "Update tblCommandeFournisseur set RefCommandeFournisseurStatut=" + ((int)Enumerations.CommandeFournisseurStatut.EnAttente).ToString() + ", DModif = getdate(), RefUtilisateurModif = " + CurrentContext.RefUtilisateur.ToString()
                              + " where RefCommandeFournisseur in (";
                            cmd.CommandText += Utils.Utils.CreateSQLParametersFromString("RefCommandeFournisseur", selectedItem, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                            cmd.CommandText += ")";
                            int res = cmd.ExecuteNonQuery();
                            //Return nb of affected rows in header
                            Response.Headers.Add("nbSetEnAttente", res.ToString());
                        }
                    }
                    //Change DMoisDechargementPrevu if needed
                    if (menu == Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString() && action == Enumerations.ActionName.ChangeMoisDechargementPrevu.ToString())
                    {
                        if (!string.IsNullOrEmpty(moisDechargementPrevuItem))
                        {
                            string[] els = moisDechargementPrevuItem.Split(',');
                            int i = 0;
                            foreach (string el in els)
                            {
                                //Check data
                                int r = 0;
                                DateTime d = DateTime.MinValue;
                                if (int.TryParse(el.Substring(0, el.IndexOf(";")), out r) && DateTime.TryParse(el.Substring(el.IndexOf(";") + 1, el.Length - el.IndexOf(";") - 1), out d))
                                {
                                    CommandeFournisseur cmdF = DbContext.CommandeFournisseurs.Find(r);
                                    cmdF.RefUtilisateurCourant = CurrentContext.RefUtilisateur;
                                    //Get reference year
                                    int y = (cmdF.DMoisDechargementPrevu != null ? ((DateTime)cmdF.DMoisDechargementPrevu).Year : (cmdF.D != null ? ((DateTime)cmdF.D).Year : DateTime.Now.Year));
                                    //Set reference year
                                    d = new DateTime(y, d.Month, 1);
                                    //Translate reference year if needed
                                    if (d.AddMonths(6) < cmdF.DMoisDechargementPrevu) { d = new DateTime(y + 1, d.Month, 1); }
                                    else if (d.AddMonths(-6) > cmdF.DMoisDechargementPrevu) { d = new DateTime(y - 1, d.Month, 1); }
                                    //Set new DMoisDechargementPrevu
                                    cmdF.DMoisDechargementPrevu = d;
                                    i++;
                                }
                            }
                            DbContext.SaveChanges();
                            //Return nb of affected rows in header
                            Response.Headers.Add("nbChangeMoisDechargementPrevu", i.ToString());
                        }
                    }
                    //Copy PrixReprise
                    if (menu == Enumerations.MenuName.LogistiqueMenuPrixReprise.ToString() && action == Enumerations.ActionName.CopyPrixReprise.ToString())
                    {
                        DateTime dFrom = DateTime.MinValue;
                        DateTime dTo = DateTime.MinValue;
                        int yFrom = 0, yTo = 0;
                        int mFrom = 0, mTo = 0;
                        if (DateTime.TryParse(monthFrom, out dFrom) && DateTime.TryParse(monthTo, out dTo) && int.TryParse(yearFrom, out yFrom) && int.TryParse(yearTo, out yTo))
                        {
                            mFrom = dFrom.Month;
                            mTo = dTo.Month;
                            int i = 0;
                            string sqlStrCopy;
                            //Traitements
                            int r = DbContext.PrixReprises.Where(el => el.D.Year == yTo && el.D.Month == mTo).Count();
                            if (r == 0)
                            {
                                sqlStrCopy = "insert into tbrPrixReprise (D,RefProcess,RefProduit,RefComposant,PUHT, PUHTSurtri, PUHTTransport,RefUtilisateurCreation)"
                                    + " select convert(datetime,'" + "01/" + mTo.ToString() + "/" + yTo.ToString() + "',103),RefProcess,RefProduit,RefComposant,PUHT, PUHTSurtri, PUHTTransport," + CurrentContext.RefUtilisateur.ToString()
                                    + " from tbrPrixReprise"
                                    + " where MONTH(D)=" + mFrom.ToString() + " and YEAR(D)=" + yFrom.ToString();
                                i = Utils.Utils.DbExecute(sqlStrCopy, (SqlConnection)DbContext.Database.GetDbConnection());
                                Response.Headers.Add("nbCopyPrixReprise", i.ToString());
                            }
                            else
                            {
                                Response.Headers.Add("nbCopyPrixReprise", "-1");
                            }
                        }
                        else
                        {
                            Response.Headers.Add("nbCopyPrixReprise", "");
                        }
                    }
                    //Export if needed
                    if (exportTransport == "true")
                    {
                        //Sql command
                        cmd.CommandText = sqlStr;
                        //Headers
                        var cD = new System.Net.Mime.ContentDisposition
                        {
                            FileName = "ValorplastTransport.xlsx",
                            // always prompt the user for downloading
                            Inline = false,
                        };
                        Response.Headers.Add("Content-Disposition", cD.ToString());
                        //Generate Excel file
                        return new FileContentResult(ExcelFileManagement.ExportTransport(cmd, CurrentContext, _dbContext, filterDptDeparts, filterDptArrivees, filterVilleDeparts, filterVilleArrivees, filterPayss, filterCamionTypes).ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    }
                    //Get the total row count
                    cmd.CommandText = "select count(*) from (" + sqlStr + ") as univers";
                    nbRow = cmd.ExecuteScalar().ToString();
                    //sqlConn.Close();
                    //sqlConn.Open();
                    //Get formatted data page
                    if (menu == Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString() && string.IsNullOrEmpty(sortExpression))
                    {
                        sqlStr = " set language French; " + sqlStr + " ORDER BY tblCommandeFournisseur.DDechargementPrevue, tblCommandeFournisseur.D OFFSET (@pageNumber  * @pageSize) ROWS FETCH NEXT @pageSize ROWS ONLY";
                    }
                    else { sqlStr = " set language French; " + sqlStr + " ORDER BY[" + sortExpression + "] " + sortOrder + " OFFSET (@pageNumber  * @pageSize) ROWS FETCH NEXT @pageSize ROWS ONLY"; }
                    cmd.CommandText = sqlStr;
                    cmd.Parameters.Add("@pageNumber", SqlDbType.Int).Value = pageNumber;
                    cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                    SqlDataAdapter dA = new(cmd);
                    if (dS.Tables.Count > 0) { dS.Tables.Clear(); }
                    dA.Fill(dS);
                }
                //Return nb of row in header
                Response.Headers.Append("nbRow", nbRow);
                //Return Json
                return new JsonResult(dS.Tables[0], JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
    }
}
