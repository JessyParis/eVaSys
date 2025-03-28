/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 04/04/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using eVaSys.APIUtils;
using eVaSys.Utils;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class DashboardController : BaseApiController
    {
        #region Constructor
        public DashboardController(ApplicationDbContext context, IConfiguration configuration)
            : base(context, configuration) { }
        #endregion Constructor
        #region Attribute-based Routing
        /// <summary>
        /// GET: api/dashboard/getitem
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>Data for dashboard item</returns>
        [HttpGet("GetItem")]
        public IActionResult GetItem()
        {
            string itemName = Request.Headers["itemName"].ToString();
            DashboardItemViewModel res = new() { nb = "0", nbUrgent = "0" };
            SqlCommand cmd = new();
            DataSet dS = new();
            string sqlStr = "";
            string sqlStrUrgent = "";
            //Process
            switch (itemName)
            {
                case "AnomalieCommandeFournisseur":
                    sqlStr = "select count(*) from tblCommandeFournisseur"
                        + " where RefUtilisateurAnomalieOk is not null and DAnomalieOk is null";
                    sqlStrUrgent = "select count(*) from tblCommandeFournisseur"
                        + " where RefUtilisateurAnomalieOk is not null and DAnomalieOk is null";
                    break;
                case "CommandesNonChargees":
                    sqlStr = "select count(*) from tblCommandeFournisseur"
                        + " where ChargementEffectue=0 and DChargementPrevue is not null and dbo.JoursTravailles(DChargementPrevue,getdate())>1"
                        + " and ChargementAnnule=0 and RefusCamion=0";
                    if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefEntite=" + CurrentContext.ConnectedUtilisateur.RefCentreDeTri;
                    }
                    sqlStrUrgent = "select count(*) from tblCommandeFournisseur"
                        + " where ChargementEffectue=0 and DChargementPrevue is not null and dbo.JoursTravailles(DChargementPrevue,getdate())>3"
                        + " and ChargementAnnule=0 and RefusCamion=0";
                    if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                    {
                        sqlStrUrgent += " and tblCommandeFournisseur.RefEntite=" + CurrentContext.ConnectedUtilisateur.RefCentreDeTri;
                    }
                    break;
                case "CommandesNonReceptionnees":
                    sqlStr = "select count(*) from tblCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + " where RefCommandeFournisseurStatut is not null and ValideDPrevues=1 and isnull(RefusCamion,0) = 0 and DDechargement is null";
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStr += " and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefPrestataire != null)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefPrestataire=" + CurrentContext.ConnectedUtilisateur.RefPrestataire;
                    }
                    sqlStrUrgent = "select count(*) from tblCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + " where RefCommandeFournisseurStatut is not null and ValideDPrevues=1 and isnull(RefusCamion,0) = 0 and DDechargement is null and datediff(month,getdate(),DMoisDechargementPrevu)<0";
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStrUrgent += " and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefPrestataire != null)
                    {
                        sqlStrUrgent += " and tblCommandeFournisseur.RefPrestataire=" + CurrentContext.ConnectedUtilisateur.RefPrestataire;
                    }
                    break;
                case "DatesTransporteur":
                    sqlStr = "select count(*) from tblCommandeFournisseur"
                        + " where RefCommandeFournisseurStatut = 1 and ValideDPrevues=0 and DDechargement is null and DDechargementPrevue is not null";
                    break;
                case "CommandesModifiees":
                    sqlStr = "select count(*) from tblCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + " where ((tblCommandeFournisseur.RefCommandeFournisseurStatut=1 or tblCommandeFournisseur.RefCommandeFournisseurStatut=3) and DChargementModif is not null and ValideDPrevues=0 and DDechargement is null)";
                    if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null)
                    {
                        sqlStr += " and RefTransporteur = " + CurrentContext.ConnectedUtilisateur.RefTransporteur + " and RefCommandeFournisseurStatut=1";
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStr += " and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient + " and RefCommandeFournisseurStatut=1";
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefPrestataire != null)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefPrestataire=" + CurrentContext.ConnectedUtilisateur.RefPrestataire;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefEntite=" + CurrentContext.ConnectedUtilisateur.RefCentreDeTri;
                    }
                    break;
                case "CommandesBourseAffrettement":
                    sqlStr = "select count(*) from tblCommandeFournisseur"
                        + " where (ValideDPrevues=0"
                        + "     and (RefCommandeFournisseurStatut=1)"
                        + "     and (DChargement is null or DDechargement is null)"
                        + "     and DBlocage is null"
                        + "     and RefusCamion=0"
                        + "     and (RefTransporteur is null or (RefTransporteur is not null and dbo.JoursTravailles(DAffretement,getdate())>2))"
                        + "     )";
                    break;
                case "CommandesEnCours":
                    sqlStr = "select count(*) from tblCommandeFournisseur"
                        + "     where (DChargement is null or DDechargement is null or PoidsDechargement = 0 or NbBalleDechargement = 0 or PoidsChargement = 0 or NbBalleChargement = 0)"
                        + "         and isnull(RefusCamion,0) = 0";
                    if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null)
                    {
                        sqlStr += " and (RefTransporteur=" + CurrentContext.ConnectedUtilisateur.RefTransporteur + " and RefCommandeFournisseurStatut=1 and DChargementModif is null and ValideDPrevues = 0)";
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefEntite=" + CurrentContext.ConnectedUtilisateur.RefCentreDeTri + " and RefCommandeFournisseurStatut in (1,2)";
                    }
                    break;
                case "RepartitionAFinaliser":
                    sqlStr = "select count(distinct RefRepartition) from tblRepartition"
                        + "     inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        + "     inner join tblUtilisateur on tblUtilisateur.RefUtilisateur=tblRepartition.RefUtilisateurCreation"
                        + " where tblCommandeFournisseur.DDechargement is not null"
                        + "     and tblUtilisateur.RefCentreDeTri is null"
                        + "     and (RefRepartition in (select distinct RefRepartition from tblRepartitionCollectivite where PUHT is null)"
                        + "         or RefRepartition in (select distinct RefRepartition from tblRepartitionProduit where PUHT is null)"
                        + "         or RefRepartition in (select distinct RefRepartition from RepartitionIncompletePoids)"
                        + "     )";
                    sqlStrUrgent = "select count(distinct RefRepartition) from tblRepartition"
                        + "     inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        + "     inner join tblUtilisateur on tblUtilisateur.RefUtilisateur=tblRepartition.RefUtilisateurCreation"
                        + " where tblCommandeFournisseur.DDechargement is null"
                        + "     and tblUtilisateur.RefCentreDeTri is not null"
                        + "     and DDechargement>getdate() and month(DDechargement)!=month(getdate())"
                        + "     and (RefRepartition in (select distinct RefRepartition from tblRepartitionCollectivite where PUHT is null)"
                        + "         or RefRepartition in (select distinct RefRepartition from tblRepartitionProduit where PUHT is null)"
                        + "         or RefRepartition in (select distinct RefRepartition from RepartitionIncompletePoids)"
                        + "     )";
                    break;
                case "MessagePourDiffusion":
                    sqlStr = "select count(*)"
                        + " from tblMessage"
                        + "     left join tbrMessageType on tblMessage.RefMessageType=tbrMessageType.RefMessageType"
                        + " where Actif=1 and getdate() <= DFin";
                    sqlStrUrgent = "select count(*)"
                        + " from tblMessage"
                        + "     left join tbrMessageType on tblMessage.RefMessageType=tbrMessageType.RefMessageType"
                        + " where Actif=1 and getdate() >= DDebut and  getdate() <= DFin";
                    break;
                case "NouvellesDemandesEnlevement":
                    sqlStr = "select count(*) from tblCommandeFournisseur"
                        + "     left join (select RefCommandeFournisseur from tblRepartition) as unitaire on tblCommandeFournisseur.RefCommandeFournisseur=unitaire.RefCommandeFournisseur "
                        + "     left join tblRepartition as mensuelle on tblCommandeFournisseur.RefEntite=mensuelle.RefFournisseur and year(tblCommandeFournisseur.DDechargement)=year(mensuelle.D) and month(tblCommandeFournisseur.DDechargement)=month(mensuelle.D) and tblCommandeFournisseur.RefProduit=mensuelle.RefProduit"
                        + " where ((unitaire.RefCommandeFournisseur is null and mensuelle.RefRepartition is null) and DDechargement is null and isnull(RefusCamion,0) = 0 and tblCommandeFournisseur.RefCommandeFournisseurStatut is null)";
                    if (CurrentContext.ConnectedUtilisateur.RefPrestataire != null)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefPrestataire=" + CurrentContext.ConnectedUtilisateur.RefPrestataire;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefEntite=" + CurrentContext.ConnectedUtilisateur.RefCentreDeTri;
                    }
                    break;
                case "EvolutionPrixTransport":
                    sqlStr = "select count(*) from tblTransport"
                        + "     inner join tblParcours on tblTransport.RefParcours = tblParcours.RefParcours"
                        + "     inner join (select RefEntite from tblentite where RefEntiteType=2) as transporteur on transporteur.RefEntite=tblTransport.RefTransporteur"
                        + "    	inner join (select tblAdresse.RefAdresse from tblAdresse"
                        + "                inner join tblEntite on tblAdresse.RefEntite = tblEntite.RefEntite"
                        + "                inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + "                inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse = tbmContactAdresse.RefContactAdresse"
                        + "                where tblEntite.Actif = 1 and tblAdresse.Actif = 1 and RefContactAdresseProcess = 1 and tblEntite.RefEntiteType = 3) as adresseOrigine on tblParcours.RefAdresseOrigine=adresseOrigine.RefAdresse"
                        + "    	inner join (select tblAdresse.RefAdresse from tblAdresse"
                        + "                inner join tblEntite on tblAdresse.RefEntite = tblEntite.RefEntite"
                        + "                inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + "                inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse = tbmContactAdresse.RefContactAdresse"
                        + "                where tblEntite.Actif = 1 and tblAdresse.Actif = 1 and RefContactAdresseProcess = 1 and tblEntite.RefEntiteType in(4, 5)) as adresseDestination on tblParcours.RefAdresseDestination=adresseDestination.RefAdresse"
                        + "     inner join tbrCamionType on tblTransport.RefCamionType=tbrCamionType.RefCamionType"
                        + " where tblTransport.PUHTDemande is not null";
                    if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null)
                    {
                        sqlStr += " and tblTransport.RefTransporteur = " + CurrentContext.ConnectedUtilisateur.RefTransporteur;
                    }
                    break;
                case "ContactModificationRequest":
                    sqlStr = "select count(*)"
                        + " from tblAction"
                        + " where DAction is null and tblAction.Libelle like 'Demande de modification d%' and RefFormeContact=6";
                    if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null)
                    {
                        sqlStr += " and tblAction.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefTransporteur;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStr += " and tblAction.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefCollectivite != null)
                    {
                        sqlStr += " and tblAction.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefCollectivite;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                    {
                        sqlStr += " and tblAction.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefCentreDeTri;
                    }
                    if (CurrentContext.filterDR)
                    {
                        sqlStr += " and tblAction.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                    }
                    sqlStrUrgent = "select count(*)"
                        + " from tblAction"
                        + " where DAction is null and dbo.JoursTravailles(DCreation, getdate())>1  and tblAction.Libelle like 'Demande de modification d%' and RefFormeContact=6";
                    if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null)
                    {
                        sqlStrUrgent += " and tblAction.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefTransporteur;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStrUrgent += " and tblAction.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefCollectivite != null)
                    {
                        sqlStrUrgent += " and tblAction.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefCollectivite;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                    {
                        sqlStrUrgent += " and tblAction.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefCentreDeTri;
                    }
                    if (CurrentContext.filterDR)
                    {
                        sqlStrUrgent += " and tblAction.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                    }
                    break;
                case "DonneeEntiteSage":
                    sqlStr = "select count(distinct tblEntite.RefEntite)"
                        + " from F_COMPTET"
                        + " 	inner join tblEntite on SAGECodeComptable=CT_Num"
                        + " where ((rtrim(ltrim(isnull(CT_Siret,''))) != rtrim(ltrim(isnull(IdNational,''))))"
                        + "     or (rtrim(ltrim(isnull(CT_Identifiant,''))) != rtrim(ltrim(isnull(CodeTVA,'')))))";
                    if (CurrentContext.filterGlobalActif)
                    {
                        sqlStr += " and tblEntite.Actif=1";
                    }
                    sqlStrUrgent = "";
                    break;
                case "TransportArrivee":
                    sqlStr = "Select distinct Ville from tblAdresse"
                        + "     inner join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                        + "     inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + "     inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse=tbmContactAdresse.RefContactAdresse "
                        + " where RefContactAdresseProcess=1 and tblEntite.RefEntiteType in(4,5) and TblAdresse.Actif=1 and tblEntite.Actif=1 order by Ville";
                    break;
                case "CommandesFournisseurAttribuees":
                    sqlStr = "select count(*) from tblCommandeFournisseur"
                        + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + " where tblCommandeFournisseur.RefCommandeFournisseurStatut=1 and isnull(RefusCamion,0)=0 and (DChargementPrevue is null or DDechargementPrevue is null)";
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStr += "     and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefPrestataire != null)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefPrestataire=" + CurrentContext.ConnectedUtilisateur.RefPrestataire;
                    }
                    if (CurrentContext.ConnectedUtilisateur.RefCentreDeTri != null)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefEntite=" + CurrentContext.ConnectedUtilisateur.RefCentreDeTri;
                    }
                    break;
                case "NouvellesNonConformites":
                    sqlStr = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 1";
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStr += "     and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                    }
                    break;
                case "NonConformitesEnCours":
                    sqlStr = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType in (2,3,4,5,6,7,8)";
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStr += "     and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                    }
                    sqlStrUrgent = "select count(*)"
                        + "from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + "     inner join (select RefNonConformite from tblNonConformiteEtape where DValide is not null and RefNonConformiteEtapeType = 4) as etape4 on etape4.RefNonConformite = tblNonConformite.RefNonConformite"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType in (2,3,4,5,6,7,8)";
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStrUrgent += "     and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                    }
                    break;
                case "NonConformitesATraiter":
                    sqlStr = "select count(*)"
                        + "from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 3 and RefNonConformiteReponseFournisseurType is null and DTransmissionFournisseur is not null";
                    if (CurrentContext.filterDR)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                    }
                    sqlStrUrgent = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 3 and RefNonConformiteReponseFournisseurType is null and DTransmissionFournisseur is not null"
                        + "    and  dbo.JoursTravailles(isnull(tblNonConformiteEtape.DModif,tblNonConformiteEtape.DCreation), getdate())>1";
                    if (CurrentContext.filterDR)
                    {
                        sqlStrUrgent += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                    }
                    break;
                case "NonConformitesATransmettre":
                    sqlStr = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 3 and DTransmissionFournisseur is null";
                    if (CurrentContext.filterDR)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                    }
                    sqlStrUrgent = "select count(*)"
                        + "from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + "     inner join (select * from tblNonConformiteEtape where RefNonConformiteEtapeType=2) as etape2 on tblNonConformite.RefNonConformite = etape2.RefNonConformite"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 3 and DTransmissionFournisseur is null"
                        + "     and dbo.JoursTravailles(etape2.DValide, getdate())>1";
                    if (CurrentContext.filterDR)
                    {
                        sqlStrUrgent += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                    }
                    break;
                case "NonConformitesAValider":
                    sqlStr = "select count(*)"
                        + "from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 4";
                    sqlStrUrgent = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + "     inner join (select * from tblNonConformiteEtape where DValide is not null and RefNonConformiteEtapeType= 3) as etape3 on etape3.RefNonConformite = tblNonConformite.RefNonConformite"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 4 and datediff(day, etape3.DValide, getdate())>1";
                    break;
                case "NonConformitesEnAttentePlanAction":
                    sqlStr = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 5 and CmtOrigineAction is null";
                    if (CurrentContext.filterDR)
                    {
                        sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                    }
                    sqlStrUrgent = "select count(*)"
                        + "from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + "     inner join (select * from tblNonConformiteEtape where DValide is not null and RefNonConformiteEtapeType= 4) as etape4 on etape4.RefNonConformite = tblNonConformite.RefNonConformite"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 5 and CmtOrigineAction is null and datediff(day, etape4.DValide, getdate())>2";
                    if (CurrentContext.filterDR)
                    {
                        sqlStrUrgent += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR=" + CurrentContext.RefUtilisateur + ")";
                    }
                    break;
                case "NonConformitesPlanActionAValider":
                    sqlStr = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + "     inner join (select RefNonConformite from tblNonConformiteEtape where DValide is null and RefNonConformiteEtapeType= 5) as etape5 on etape5.RefNonConformite = tblNonConformite.RefNonConformite"
                        + " where CmtOrigineAction is not null";
                    break;
                case "NonConformitesAFacturer":
                    sqlStr = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where (IFFournisseurFacture = 1 and IFFournisseurFactureNro is null)";
                    break;
                case "NonConformitesACloturer":
                    sqlStr = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 8";
                    break;
                case "NonConformitesEnAttenteFacture":
                    sqlStr = "select count(*)"
                        + " from tblNonConformite"
                        + "     left join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                        + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                        + "     inner join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite = Etape.RefNonConformite and tblNonConformiteEtape.Ordre = etape.Ordre"
                        + " where tblNonConformiteEtape.RefNonConformiteEtapeType = 7";
                    if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                    {
                        sqlStr += "     and tblAdresse.RefEntite = " + CurrentContext.ConnectedUtilisateur.RefClient;
                    }
                    break;
            }
            //Chargement des données si elles existent
            if (sqlStr != "")
            {
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    res.nb = cmd.ExecuteScalar().ToString();
                    if (sqlStrUrgent != "")
                    {
                        cmd.CommandText = sqlStrUrgent;
                        res.nbUrgent = cmd.ExecuteScalar().ToString();
                    }
                }
                //Return Json
                return new JsonResult(res, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError());
            }
        }
        #endregion
    }
}
