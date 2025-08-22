/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/03/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using eVaSys.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using eVaSys.APIUtils;
using Microsoft.EntityFrameworkCore;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class StatistiqueController : BaseApiController
    {
        #region Constructor
        public StatistiqueController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment env)
            : base(context, configuration)
        {
            _dbContext = context;
            _env = env;
        }
        protected IWebHostEnvironment _env;
        #endregion Constructor
        private readonly ApplicationDbContext _dbContext;
        /// <summary>
        /// GET: evapi/getstatistiques/get
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all items.</returns>
        [HttpGet("GetStatistique")]
        public IActionResult GetStatistique()
        {
            string module = Request.Query["module"].ToString();
            string menu = Request.Query["menu"].ToString();
            int pageNumber = System.Convert.ToInt32(Request.Query["pageNumber"]);
            int pageSize = System.Convert.ToInt32(Request.Query["pageSize"]);
            EnvStatisticsFilters eSF = new(_dbContext, CurrentContext.CurrentCulture);
            string exportType = Request.Headers["exportType"].ToString();
            eSF.FilterText = Request.Query["filterText"].ToString();
            eSF.FilterMonths = Request.Headers["filterMonths"].ToString();
            eSF.FilterQuarters = Request.Headers["filterQuarters"].ToString();
            eSF.FilterYears = Request.Headers["filterYears"].ToString();
            eSF.FilterDayWeekMonth = Request.Headers["filterDayWeekMonth"].ToString();
            eSF.FilterBegin = Request.Headers["filterBegin"].ToString();
            eSF.FilterEnd = Request.Headers["filterEnd"].ToString();
            eSF.FilterActif = (Request.Headers["filterActif"] == "true" ? true : false);
            eSF.FilterClients = Request.Headers["filterClients"].ToString();
            eSF.FilterCentreDeTris = Request.Headers["filterCentreDeTris"].ToString();
            eSF.FilterCollectivites = Request.Headers["filterCollectivites"].ToString();
            eSF.FilterDptDeparts = Request.Headers["filterDptDeparts"].ToString();
            eSF.FilterDptArrivees = Request.Headers["filterDptArrivees"].ToString();
            eSF.FilterDRs = Request.Headers["filterDRs"].ToString();
            eSF.FilterEntites = Request.Headers["filterEntites"].ToString();
            eSF.FilterFirstLogin = (Request.Headers["filterFirstLogin"] == "true" ? true : false);
            eSF.FilterPayss = Request.Headers["filterPayss"].ToString();
            eSF.FilterEcoOrganismes = Request.Headers["filterEcoOrganismes"].ToString();
            eSF.FilterEntiteTypes = Request.Headers["filterEntiteTypes"].ToString();
            eSF.FilterProcesss = Request.Headers["filterProcesss"].ToString();
            eSF.FilterProduits = Request.Headers["filterProduits"].ToString();
            eSF.FilterProduitGroupeReportings = Request.Headers["filterProduitGroupeReportings"].ToString();
            eSF.FilterTransporteurs = Request.Headers["filterTransporteurs"].ToString();
            eSF.FilterVilleDeparts = Request.Headers["filterVilleDeparts"].ToString();
            eSF.FilterAdresseDestinations = Request.Headers["filterAdresseDestinations"].ToString();
            eSF.FilterVilleArrivees = Request.Headers["filterVilleArrivees"].ToString();
            eSF.FilterMoins10Euros = (Request.Headers["filterMoins10Euros"] == "true" ? true : false);
            eSF.FilterCollecte = Request.Headers["filterCollecte"].ToString();
            eSF.FilterContrat = Request.Headers["filterContrat"].ToString();
            eSF.FilterNonConformiteEtapeTypes = Request.Headers["filterNonConformiteEtapeTypes"].ToString();
            eSF.FilterNonConformiteNatures = Request.Headers["filterNonConformiteNatures"].ToString();
            eSF.FilterCamionTypes = Request.Headers["filterCamionTypes"].ToString();
            eSF.FilterActionTypes = Request.Headers["filterActionTypes"].ToString();
            eSF.FilterAdresseTypes = Request.Headers["filterAdresseTypes"].ToString();
            eSF.FilterContactAdresseProcesss = Request.Headers["filterContactAdresseProcesss"].ToString();
            eSF.FilterFonctions = Request.Headers["filterFonctions"].ToString();
            eSF.FilterServices = Request.Headers["filterServices"].ToString();
            eSF.FilterEmailType = Request.Headers["filterEmailType"].ToString();
            eSF.FilterUtilisateurs = Request.Headers["filterUtilisateurs"].ToString();
            eSF.StatType = Request.Query["statType"].ToString();
            string filterContactSelectedColumns = Request.Headers["filterContactSelectedColumns"].ToString();
            SqlCommand cmd = new();
            DataSet dS = new();
            string sqlStr = "", sqlStrCount = "", sqlStrFinal = "", s = "", s1 = "", s2 = "";
            //Chech authorization
            if (!Rights.AuthorizedMenu(menu, CurrentContext, Configuration))
            {
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString())
            {
                //Base SQL statement
                sqlStr = "select cast(NumeroAffretement as nvarchar(20)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                    + "     , DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefTransporteur == null) { sqlStr += "     , fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"; }
                sqlStr += "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefTransporteur == null) { sqlStr += "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"; }
                sqlStr += "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                    + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefTransporteur == null) { sqlStr += "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"; }
                sqlStr += "     , PrixTransportHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransportPrix.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefTransporteur == null || CurrentContext.ConnectedUtilisateur?.Transporteur?.SurcoutCarburant == true) { sqlStr += "     , isnull(Ratio,0) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SurcoutCarburantRatio.ToString()].Name + "]"; }
                if (CurrentContext.ConnectedUtilisateur.RefTransporteur == null || CurrentContext.ConnectedUtilisateur?.Transporteur?.SurcoutCarburant == true) { sqlStr += "     , SurcoutCarburantHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurSurcoutCarburantHT.ToString()].Name + "]"; }
                sqlStr += "     , PrixTransportSupplementHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPrixTransportSupplementHT.ToString()].Name + "]"
                    + "     , PrixTransportHT+PrixTransportSupplementHT+SurcoutCarburantHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransportTotalPrix.ToString()].Name + "]"
                    + "     , Km as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ParcoursKm.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + " 	inner join tblEntite as transporteur on tblCommandeFournisseur.RefTransporteur=transporteur.RefEntite"
                    + " 	inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 	left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                    + " 	left join tblSurcoutCarburant on transporteur.RefEntite=tblSurcoutCarburant.RefTransporteur and month(isnull(DDechargement, DMoisDechargementPrevu))=tblSurcoutCarburant.Mois and year(isnull(DDechargement, DMoisDechargementPrevu))=tblSurcoutCarburant.Annee and tblAdresse.RefPays=tblSurcoutCarburant.RefPays"
                    + " 	inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                    + " 	inner join tblEntite as fournisseur on tblCommandeFournisseur.RefEntite=fournisseur.RefEntite"
                    + " where 1=1 ";
                //Filters
                if (CurrentContext.ConnectedUtilisateur.RefTransporteur != null)
                {
                    cmd.Parameters.Add("@refTransporteur", SqlDbType.Int).Value = CurrentContext.ConnectedUtilisateur.RefTransporteur;
                    sqlStr += " and transporteur.RefEntite = @refTransporteur";
                }
                else if (eSF.FilterTransporteurs != "")
                {
                    sqlStr += " and transporteur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", eSF.FilterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterBegin) || !string.IsNullOrEmpty(eSF.FilterEnd))
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (!string.IsNullOrEmpty(eSF.FilterBegin)) { DateTime.TryParse(eSF.FilterBegin, out begin); }
                    if (!string.IsNullOrEmpty(eSF.FilterEnd)) { DateTime.TryParse(eSF.FilterEnd, out end); }
                    if (begin != DateTime.MinValue) { cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin; }
                    if (end != DateTime.MinValue) { cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end; }
                    if (begin != DateTime.MinValue && end != DateTime.MinValue)
                    {
                        sqlStr += " and (case when RefusCamion=0 then DDechargement else DDechargementPrevue end between @begin and @end)";
                    }
                    else if (begin != DateTime.MinValue)
                    {
                        sqlStr += " and  (case when RefusCamion=0 then DDechargement else DDechargementPrevue end >= @begin)";
                    }
                    else if (end != DateTime.MinValue)
                    {
                        sqlStr += " and  (case when RefusCamion=0 then DDechargement else DDechargementPrevue end between <= @end)";
                    }
                }
                if (eSF.FilterVilleDeparts != "")
                {
                    sqlStr += " and tblCommandeFournisseur.Ville COLLATE Latin1_general_CI_AI in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("adresseOrigineVille", eSF.FilterVilleDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterVilleArrivees != "")
                {
                    sqlStr += " and tblAdresse.Ville COLLATE Latin1_general_CI_AI in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("adresseDestinationVille", eSF.FilterVilleArrivees, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterPayss != "")
                {
                    sqlStr += " and tbrPays.RefPays in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refPays", eSF.FilterPayss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuLotDisponible.ToString())
            {
                //Base SQL statement
                sqlStr = "select case when tblEntite.CodeEE is not null then tblEntite.CodeEE + ' - ' else '' end + tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]"
                    + " 	, dbo.ListeDR(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + " 	, tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , sum(case when month(D) = 1 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Janvier.ToString()].Name + "]"
                    + "     , sum(case when month(D) = 2 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Fevrier.ToString()].Name + "]"
                    + "     , sum(case when month(D) = 3 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Mars.ToString()].Name + "]"
                    + "     , sum(case when month(D) = 4 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Avril.ToString()].Name + "]"
                    + "     , sum(case when month(D) = 5 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Mai.ToString()].Name + "]"
                    + " 	, sum(case when month(D) = 6 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Juin.ToString()].Name + "]"
                    + " 	, sum(case when month(D) = 7 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Juillet.ToString()].Name + "]"
                    + " 	, sum(case when month(D) = 8 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Aout.ToString()].Name + "]"
                    + " 	, sum(case when month(D) = 9 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Septembre.ToString()].Name + "]"
                    + " 	, sum(case when month(D) = 10 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Octobre.ToString()].Name + "]"
                    + " 	, sum(case when month(D) = 11 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Novembre.ToString()].Name + "]"
                    + " 	, sum(case when month(D) = 12 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Decembre.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + "     inner join tblEntite on tblCommandeFournisseur.RefEntite = tblEntite.RefEntite"
                    + "     inner join tblProduit on tblCommandeFournisseur.RefProduit = tblProduit.RefProduit"
                    + " where D between @begin and @end and tblCommandeFournisseur.RefCommandeFournisseurStatut in (1, 2)"
                    + "     and dbo.CommandeMixte(tblCommandeFournisseur.RefCommandeFournisseur) = 0";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                sqlStr += " group by case when tblEntite.CodeEE is not null then tblEntite.CodeEE + ' - ' else '' end + tblEntite.Libelle, dbo.ListeDR(tblEntite.RefEntite), tblProduit.Libelle";
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatTarifTransporteurClient.ToString())
            {
                //First command for fields
                sqlStr = "select distinct RefTransporteur, tblEntite.Libelle"
                    + " from tblTransport"
                    + " 	inner join tblParcours on tblTransport.RefParcours=tblParcours.RefParcours"
                    + "     inner join tblEntite on tblTransport.RefTransporteur=tblEntite.RefEntite"
                    + "     inner join tblAdresse on tblParcours.RefAdresseOrigine=tblAdresse.RefAdresse"
                    + " where 1=1";
                //Filters
                if (eSF.FilterAdresseDestinations != "")
                {
                    sqlStr += " and RefAdresseDestination in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refAdresseDestination", eSF.FilterAdresseDestinations, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                else
                {
                    sqlStr += " and RefAdresseDestination = 0";
                }
                if (eSF.FilterDptDeparts != "")
                {
                    sqlStr += " and left(tblAdresse.CodePostal+'00',2) in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("codePostal", eSF.FilterDptDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                    sqlStr += ")";
                }
                //Tri
                sqlStr += " order by tblEntite.Libelle";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        s += "[" + dr.GetSqlInt32(0).ToString() + "],";
                        s1 += "[" + dr.GetSqlInt32(0).ToString() + "] as [" + dr.GetSqlString(1).ToString() + "],";
                    }
                    dr.Close();
                    if (s != "") { s = s.Substring(0, s.Length - 1); } else { s = " [0]"; }
                    if (s1 != "") { s1 = s1.Substring(0, s1.Length - 1); } else { s1 = "[0] as [Nothing]"; }
                }
                cmd = new SqlCommand();
                //Base SQL statement
                sqlStr = "select distinct"
                    + "     tblAdresse.CodePostal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineCP.ToString()].Name + "]"
                    + "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]"
                    + "     , tblParcours.Km as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ParcoursKm.ToString()].Name + "]"
                    + "     ," + s1
                    + " from tblAdresse"
                    + "     inner join tblParcours on tblAdresse.RefAdresse=tblParcours.RefAdresseOrigine"
                    + "     inner join tblTransport on tblParcours.RefParcours=tblTransport.RefParcours"
                    + "     inner join "
                    + "     ("
                    + "         select RefParcours, " + s
                    + "         from (select RefParcours, PUHT, RefTransporteur from tblTransport) as SourceTable"
                    + "         pivot"
                    + "         (max(PUHT)"
                    + "         for RefTransporteur in (" + s + ")"
                    + "         ) as PivotTable"
                    + "     )as prixTransport"
                    + "     on tblParcours.RefParcours=prixTransport.RefParcours";
                sqlStr += " where 1=1";
                //Filters
                if (eSF.FilterAdresseDestinations != "")
                {
                    sqlStr += " and RefAdresseDestination in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refAdresseDestination", eSF.FilterAdresseDestinations, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                else
                {
                    sqlStr += " and RefAdresseDestination = 0";
                }
                if (eSF.FilterDptDeparts != "")
                {
                    sqlStr += " and left(tblAdresse.CodePostal+'00',2) in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("codePostal", eSF.FilterDptDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                    sqlStr += ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuPartMarcheTransporteur.ToString())
            {
                string filterYears = "", filterClients = "", filterMonths = "", filterPayss = "";
                sqlStr = "select 0 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Ordre.ToString()].Name + "]"
                    + "     , [Transporteur] as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                    + "     , [Nb. d'affrètements] as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NbAffretement.ToString()].Name + "]"
                    + "     , cast([Nb. d'affrètements]*100/[NbAffretementTotal] as decimal(10,2)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NbAffretementPourcentage.ToString()].Name + "]"
                    + " 	, [CA] as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CA.ToString()].Name + "]"
                    + "     , cast([CA]*100/[CATotal] as decimal(10,2)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CAPourcentage.ToString()].Name + "]"
                    + " from"
                    + " 	("
                    + " 		select RefTransporteur as [Id transporteur], tblEntite.Libelle as [Transporteur], count(distinct NumeroAffretement) as [Nb. d'affrètements]"
                    + " 			, sum(PrixTransportHT+PrixTransportSupplementHT) as [CA]"
                    + " 		from tblCommandeFournisseur"
                    + " 			inner join tblEntite on tblCommandeFournisseur.RefTransporteur=tblEntite.RefEntite"
                    + " 			left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 		where RefusCamion=0 and RefCommandeFournisseurStatut in(1,2)";
                //Critères
                if (eSF.FilterYears != "")
                {
                    sqlStr += " and year(DMoisDechargementPrevu) in (";
                    filterYears = Utils.Utils.CreateSQLParametersFromString("anneeDechargementPrevuA", eSF.FilterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += filterYears;
                    sqlStr += ")";
                }
                if (eSF.FilterMonths != "")
                {
                    sqlStr += " and month(DMoisDechargementPrevu) in (";
                    filterMonths = Utils.Utils.CreateSQLParametersFromString("moisDechargementPrevuA", eSF.FilterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += filterMonths;
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and tblAdresse.RefEntite in (";
                    filterClients = Utils.Utils.CreateSQLParametersFromString("clientA", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += filterClients;
                    sqlStr += ")";
                }
                if (eSF.FilterPayss != "")
                {
                    sqlStr += " and tblAdresse.RefPays in (";
                    filterPayss = Utils.Utils.CreateSQLParametersFromString("paysA", eSF.FilterPayss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += filterPayss;
                    sqlStr += ")";
                }
                sqlStr += " 		group by RefTransporteur, tblEntite.Libelle"
                    + " 	) as detail"
                    + " 	, ("
                    + " 		select cast(count(distinct NumeroAffretement) as decimal(10,2)) as NbAffretementTotal, sum(PrixTransportHT+PrixTransportSupplementHT) as [CATotal]"
                    + " 		from tblCommandeFournisseur"
                    + " 			inner join tblEntite on tblCommandeFournisseur.RefTransporteur=tblEntite.RefEntite"
                    + " 			left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 		where RefusCamion=0 and RefCommandeFournisseurStatut in(1,2)";
                //Critères
                if (eSF.FilterYears != "")
                {
                    sqlStr += " and year(DMoisDechargementPrevu) in (";
                    sqlStr += filterYears;
                    sqlStr += ")";
                }
                if (eSF.FilterMonths != "")
                {
                    sqlStr += " and month(DMoisDechargementPrevu) in (";
                    sqlStr += filterMonths;
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and tblAdresse.RefEntite in (";
                    sqlStr += filterClients;
                    sqlStr += ")";
                }
                if (eSF.FilterPayss != "")
                {
                    sqlStr += " and tblAdresse.RefPays in (";
                    sqlStr += filterPayss;
                    sqlStr += ")";
                }
                sqlStr += " 	) as total"
                    + " union all"
                    + " 	select 1 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Ordre.ToString()].Name + "], 'Total', count(distinct NumeroAffretement) as NbAffretementTotal, 100, sum(PrixTransportHT+PrixTransportSupplementHT) as [CATotal], 100"
                    + " 	from tblCommandeFournisseur"
                    + " 		inner join tblEntite on tblCommandeFournisseur.RefTransporteur=tblEntite.RefEntite"
                    + " 		left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 	where RefusCamion=0  and RefCommandeFournisseurStatut in(1,2)";
                //Critères
                if (eSF.FilterYears != "")
                {
                    sqlStr += " and year(DMoisDechargementPrevu) in (";
                    sqlStr += filterYears;
                    sqlStr += ")";
                }
                if (eSF.FilterMonths != "")
                {
                    sqlStr += " and month(DMoisDechargementPrevu) in (";
                    sqlStr += filterMonths;
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and tblAdresse.RefEntite in (";
                    sqlStr += filterClients;
                    sqlStr += ")";
                }
                if (eSF.FilterPayss != "")
                {
                    sqlStr += " and tblAdresse.RefPays in (";
                    sqlStr += filterPayss;
                    sqlStr += ")";
                }
                sqlStrCount = sqlStr;
                sqlStr += "order by [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Ordre.ToString()].Name + "], [Transporteur]";
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatKmMoyen.ToString())
            {
                sqlStr = "select min(tblProduit.Libelle) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , count(distinct NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NbAffretement.ToString()].Name + "]"
                    + "     , sum(PoidsDechargement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDechargementTotal.ToString()].Name + "]"
                    + " 	, sum(PoidsDechargement)/count(distinct NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDechargementAffretement.ToString()].Name + "]"
                    + "     , sum(Km) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.KmTotal.ToString()].Name + "]"
                    + "    , sum(Km)/count(distinct NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.KmAffretement.ToString()].Name + "]"
                    + " 	, sum(PrixTransportHT+PrixTransportSupplementHT) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixTotalTransport.ToString()].Name + "]"
                    + "     , cast(sum(PrixTransportHT+PrixTransportSupplementHT)/count(distinct NumeroAffretement) as decimal(10,2)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixTransportAffretement.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + " 	inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " where DDechargement between @begin and @end and dbo.CommandeMixte(NumeroAffretement)=0";
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                sqlStr += " group by tblCommandeFournisseur.RefProduit";
                //Critères
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatReceptionFournisseurChargement.ToString())
            {
                sqlStr = "select tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + " 	, DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]"
                    + " 	, DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                    + "     , PoidsChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsChargement.ToString()].Name + "]"
                    + "     , NbBalleChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleChargement.ToString()].Name + "]"
                    + "     , PoidsReparti as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsReparti.ToString()].Name + "]"
                    + " 	, NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + " from tblCommandeFournisseur "
                    + " 	inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 	inner join tblEntite as transporteur on tblCommandeFournisseur.RefTransporteur=transporteur.RefEntite"
                    + " 	inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 	inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                    + " 	inner join tblEntite as fournisseur on tblCommandeFournisseur.RefEntite=fournisseur.RefEntite"
                    + " where DDechargement between  @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and fournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refFournisseur", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatReception.ToString())
            {
                sqlStr = "select tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + " 	, DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                    + "     , fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]"
                    + "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                    + "     , PoidsDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsDechargement.ToString()].Name + "]"
                    + "     , NbBalleDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleDechargement.ToString()].Name + "]"
                    + "     , PrixTonneHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixEuroHTTonne.ToString()].Name + "]"
                    + " 	, NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + " from tblCommandeFournisseur "
                    + " 	inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 	inner join tblEntite as transporteur on tblCommandeFournisseur.RefTransporteur=transporteur.RefEntite"
                    + " 	inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 	inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                    + " 	inner join tblEntite as fournisseur on tblCommandeFournisseur.RefEntite=fournisseur.RefEntite"
                    + " where DDechargement between  @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString())
            {
                string filterProduits = "", filterClients = "", filterVilleDestination = "";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterVilleArrivees != "")
                {
                    filterVilleDestination = Utils.Utils.CreateSQLParametersFromString("villeDestination", eSF.FilterVilleArrivees, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                }
                if (eSF.FilterClients != "")
                {
                    filterClients = Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterProduits != "")
                {
                    filterProduits = Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                sqlStr = "select tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                    + "     , tblProduit.Libelle + case when reliquat.RefContrat is null then '' else ' ('+reliquat.IdContrat+')' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , commandeClient.Poids as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsCommande.ToString()].Name + "]"
                    + "     , isnull(reliquat.Poids,0)/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsChargementAttribue.ToString()].Name + "]"
                    + "     , (isnull(reliquat.Poids,0))/(commandeClient.Poids*10) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsAttribuePourcentage.ToString()].Name + "]"
                    + "     , ((commandeClient.Poids*1000) - isnull(reliquat.Poids,0))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsRestantALivrer.ToString()].Name + "]"
                    + "     , isnull(reliquat.PoidsDate,0)/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsChargementDate.ToString()].Name + "]"
                    + "     , (isnull(reliquat.PoidsDate,0))/(commandeClient.Poids*10) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsChargementDatePourcentage.ToString()].Name + "]"
                    + "     , isnull(reliquat.PoidsDecharge,0)/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDechargementReceptionne.ToString()].Name + "]"
                    + "     , (isnull(reliquat.PoidsDecharge,0))/(commandeClient.Poids*10) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDechargementReceptionnePourcentage.ToString()].Name + "]"
                    + " from tblEntite"
                    + " inner join "
                    + " 	(select RefEntite, RefAdresse, RefProduit, RefContrat, sum(tblCommandeClientMensuelle.Poids) as Poids"
                    + "         from tblCommandeClient inner join tblCommandeClientMensuelle on tblCommandeClient.RefCommandeClient=tblCommandeClientMensuelle.RefCommandeClient"
                    + " 	where tblCommandeClientMensuelle.D between @begin and @end and tblCommandeClientMensuelle.Poids>0"
                    + "     group by RefEntite, RefAdresse, RefProduit, RefContrat"
                    + "     ) as commandeClient on tblEntite.RefEntite=commandeClient.RefEntite"
                    + " inner join tblProduit on commandeClient.RefProduit=tblProduit.RefProduit"
                    + " inner join tblAdresse on commandeClient.RefAdresse=tblAdresse.RefAdresse"
                    + " left join "
                    + " 	("
                    + " 	select tblAdresse.RefEntite, tblAdresse.RefAdresse, tblCommandeFournisseur.RefProduit, VueCommandeFournisseurContrat.RefContrat, tblContrat.IdContrat"
                    + "         , sum(PoidsChargement) as Poids, sum(case when DDechargementPrevue is not null then PoidsChargement else 0 end) as PoidsDate, sum (PoidsDechargement) as PoidsDecharge"
                    + " 	from tblCommandeFournisseur"
                    + "         left join VueCommandeFournisseurContrat on tblCommandeFournisseur.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                    + "         left join tblContrat on tblContrat.RefContrat=VueCommandeFournisseurContrat.RefContrat"
                    + " 	    inner join tblAdresse on tblAdresse.RefAdresse=tblCommandeFournisseur.RefAdresseClient"
                    + "         inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 	where RefusCamion=0 and DMoisDechargementPrevu between @begin and @end";
                if (eSF.FilterVilleArrivees != "") { sqlStr += " and tblAdresse.Ville COLLATE Latin1_general_CI_AI in (" + filterVilleDestination + ")"; }
                if (eSF.FilterClients != "") { sqlStr += " and tblAdresse.RefEntite in (" + filterClients + ")"; }
                if (eSF.FilterProduits != "") { sqlStr += " and tblCommandeFournisseur.RefProduit in (" + filterProduits + ")"; }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and tblProduit.Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                }
                sqlStr += " 	group by tblAdresse.RefEntite, tblAdresse.RefAdresse, tblCommandeFournisseur.RefProduit, VueCommandeFournisseurContrat.RefContrat, tblContrat.IdContrat"
                    + " 	) as reliquat"
                    + " 	on tblEntite.RefEntite=reliquat.RefEntite and commandeClient.RefProduit=reliquat.RefProduit and commandeClient.RefAdresse=reliquat.RefAdresse and isnull(commandeClient.RefContrat,0)=isnull(reliquat.RefContrat,0)"
                    + " where 1=1";
                if (eSF.FilterVilleArrivees != "") { sqlStr += " and tblAdresse.Ville COLLATE Latin1_general_CI_AI in (" + filterVilleDestination + ")"; }
                if (eSF.FilterClients != "") { sqlStr += " and tblEntite.RefEntite in (" + filterClients + ")"; }
                if (eSF.FilterProduits != "") { sqlStr += " and commandeClient.RefProduit in (" + filterProduits + ")"; }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and tblProduit.Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClientProduit.ToString())
            {
                string filterProduits = "", filterCollecte = "";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                    }
                }
                if (eSF.FilterProduits != "")
                {
                    filterProduits = Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    filterCollecte += " and tblProduit.Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    filterCollecte += " and isnull(tblProduit.Collecte,0)=0";
                }
                sqlStr = "select isnull(tblProduit.Libelle,produitFournisseur.Libelle) + case when reliquat.RefContrat is null then '' else ' ('+reliquat.IdContrat+')' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , isnull(commandeClient.Poids,0) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsCommande.ToString()].Name + "]"
                    + "     , isnull(reliquat.Poids,0)/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsStockCentreDeTri.ToString()].Name + "]"
                    + "     , 100-((commandeClient.Poids-isnull(reliquat.Poids,0)/1000)*100/commandeClient.Poids) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsStockCentreDeTriPourcentage.ToString()].Name + "]"
                    + "     , isnull(reliquat.PoidsDate,0)/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsChargementDate.ToString()].Name + "]"
                    + "     , isnull(reliquat.PoidsDecharge,0)/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDechargementReceptionne.ToString()].Name + "]"
                    + "     , isnull(PoidsM,0)/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDontDispoMoisPrecedent.ToString()].Name + "]"
                    + " from "
                    + " 	(select RefProduit, RefContrat, sum(tblCommandeClientMensuelle.Poids) as Poids  from tblCommandeClient inner join tblCommandeClientMensuelle on tblCommandeClient.RefCommandeClient=tblCommandeClientMensuelle.RefCommandeClient "
                    + "     where year(tblCommandeClient.D)=year(@begin) and month(tblCommandeClientMensuelle.D)=month(@begin) and tblCommandeClientMensuelle.Poids>0"
                    + "     group by RefProduit, RefContrat) as commandeClient"
                    + "     left join tblProduit on commandeClient.RefProduit=tblProduit.RefProduit"
                    + "     full outer join "
                    + "         ("
                    + "         select tblCommandeFournisseur.RefProduit, VueCommandeFournisseurContrat.RefContrat, tblContrat.IdContrat"
                    + "             , sum(PoidsChargement) as Poids, sum(case when year(D)!=year(@begin) or month(D)!=month(@begin) then PoidsChargement else 0 end) as PoidsM"
                    + "             , sum(case when DDechargementPrevue is not null then PoidsChargement else 0 end) as PoidsDate, sum (PoidsDechargement) as PoidsDecharge"
                    + "     	from tblCommandeFournisseur"
                    + "             left join VueCommandeFournisseurContrat on tblCommandeFournisseur.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                    + "             left join tblContrat on tblContrat.RefContrat=VueCommandeFournisseurContrat.RefContrat"
                    + "             left join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + "     	where RefusCamion=0 and year(isnull(DMoisDechargementPrevu,D))=year(@begin) and month(isnull(DMoisDechargementPrevu,D))=month(@begin)";
                if (eSF.FilterProduits != "") { sqlStr += " and tblCommandeFournisseur.RefProduit in (" + filterProduits + ")"; }
                if (filterCollecte != "") { sqlStr += filterCollecte; }
                sqlStr += "     	group by tblCommandeFournisseur.RefProduit, VueCommandeFournisseurContrat.RefContrat, tblContrat.IdContrat"
                    + "     	) as reliquat"
                    + "     	on commandeClient.RefProduit=reliquat.RefProduit and isnull(commandeClient.RefContrat,0)=isnull(reliquat.RefContrat,0)"
                    + "     left join tblProduit as produitFournisseur on reliquat.RefProduit=produitFournisseur.RefProduit"
                    + " where 1=1";
                if (eSF.FilterProduits != "") { sqlStr += " and commandeClient.RefProduit in (" + filterProduits + ")"; }
                if (filterCollecte != "") { sqlStr += filterCollecte; }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduit.ToString())
            {
                string filterProduits = "", filterCollecte = "", filterProduitGroupeReportingss = "";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterProduitGroupeReportings != "")
                {
                    filterProduitGroupeReportingss = Utils.Utils.CreateSQLParametersFromString("refProduitGroupeReporting", eSF.FilterProduitGroupeReportings, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterProduits != "")
                {
                    filterProduits = Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    filterCollecte += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    filterCollecte += " and isnull(Collecte,0)=0";
                }
                sqlStr = "select Client as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + "     , Produit as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , ProduitGroupeReporting as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingLibelle.ToString()].Name + "]"
                    + "     , [Poids déchargement (kg)] as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDechargementTonne.ToString()].Name + "]"
                    + "     , [Nb. de balles déchargement] as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleDechargement.ToString()].Name + "]"
                    + "     , [N° d'affrètement] as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NbAffretement.ToString()].Name + "]"
                    + "     , [N° de commande] as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NbCommandeFournisseur.ToString()].Name + "]"
                    + " from"
                    + " ("
                    + " 	select top 1000000000 client.Libelle as Client, tblProduit.Libelle as Produit, tbrProduitGroupeReporting.Libelle as ProduitGroupeReporting"
                    + " 		, cast(sum(PoidsDechargement) as decimal(20,3))/1000 as [Poids déchargement (kg)], sum(NbBalleDechargement) as [Nb. de balles déchargement]"
                    + " 		, count(distinct NumeroAffretement) as [N° d'affrètement], count(NumeroCommande) as [N° de commande]"
                    + " 	from tblCommandeFournisseur "
                    + " 		inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 		left join tbrProduitGroupeReporting on tbrProduitGroupeReporting.RefProduitGroupeReporting=tblProduit.RefProduitGroupeReporting"
                    + " 		inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 		inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                    + " 	where DDechargement between  @begin and @end";
                if (eSF.FilterProduitGroupeReportings != "") { sqlStr += " and tblProduit.RefProduitGroupeReporting in (" + filterProduitGroupeReportingss + ")"; }
                if (eSF.FilterProduits != "") { sqlStr += " and tblCommandeFournisseur.RefProduit in (" + filterProduits + ")"; }
                if (filterCollecte != "") { sqlStr += filterCollecte; }
                sqlStr += " 	group by Client.Libelle, tblProduit.Libelle, tbrProduitGroupeReporting.Libelle"
                    + " 	order by tblProduit.Libelle, Client.Libelle"
                    + " ) as u"
                    + " union all"
                    + " select Client, Produit, ProduitGroupeReporting, [Poids déchargement (kg)], [Nb. de balles déchargement], [N° d'affrètement], [N° de commande]"
                    + " from"
                    + " ("
                    + " 	select 'Total' as Client, 'Général' as Produit, '' as [ProduitGroupeReporting]"
                    + " 		, cast(sum(PoidsDechargement) as decimal(20,3))/1000 as [Poids déchargement (kg)], sum(NbBalleDechargement) as [Nb. de balles déchargement]"
                    + " 		, count(distinct NumeroAffretement) as [N° d'affrètement], count(NumeroCommande) as [N° de commande]"
                    + " 	from tblCommandeFournisseur "
                    + " 		inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 		left join tbrProduitGroupeReporting on tbrProduitGroupeReporting.RefProduitGroupeReporting=tblProduit.RefProduitGroupeReporting"
                    + " 		inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 		inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                    + " 	where DDechargement between  @begin and @end";
                if (eSF.FilterProduitGroupeReportings != "") { sqlStr += " and tblProduit.RefProduitGroupeReporting in (" + filterProduitGroupeReportingss + ")"; }
                if (eSF.FilterProduits != "") { sqlStr += " and tblCommandeFournisseur.RefProduit in (" + filterProduits + ")"; }
                if (filterCollecte != "") { sqlStr += filterCollecte; }
                sqlStr += " ) as u2";
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduitDR.ToString())
            {
                string filterDRs = "", filterProduits = "", filterCollecte = "", filterYears = "";
                //Filters
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    filterDRs = Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterYears != "")
                {
                    filterYears = Utils.Utils.CreateSQLParametersFromString("year", eSF.FilterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterProduits != "")
                {
                    filterProduits = Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    filterCollecte += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    filterCollecte += " and isnull(Collecte,0)=0";
                }
                sqlStr = "select [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JanvierKg.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FevrierKg.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MarsKg.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AvrilKg.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MaiKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuinKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuilletKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AoutKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SeptembreKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.OctobreKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NovembreKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DecembreKg.ToString()].Name + "]"
                    + " from"
                    + " ("
                    + " 	select top 1000000000 dbo.ListeDR(tblCommandeFournisseur.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=1 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JanvierKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=2 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FevrierKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=3 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MarsKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=4 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AvrilKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=5 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MaiKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=6 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuinKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=7 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuilletKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=8 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AoutKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=9 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SeptembreKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=10 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.OctobreKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=11 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NovembreKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=12 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DecembreKg.ToString()].Name + "]"
                    + " 	from tblCommandeFournisseur";
                sqlStr += " 		inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 	where year(DDechargement) in (" + filterYears + ")";
                if (eSF.FilterProduits != "") { sqlStr += " and tblCommandeFournisseur.RefProduit in (" + filterProduits + ")"; }
                if (filterCollecte != "") { sqlStr += filterCollecte; }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += filterDRs;
                    sqlStr += "))";
                }
                sqlStr += " 	group by dbo.ListeDR(tblCommandeFournisseur.RefEntite)"
                    + " 	order by dbo.ListeDR(tblCommandeFournisseur.RefEntite)"
                    + " ) as u"
                    + " union all"
                    + " select [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JanvierKg.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FevrierKg.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MarsKg.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AvrilKg.ToString()].Name + "]"
                    + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MaiKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuinKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuilletKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AoutKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SeptembreKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.OctobreKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NovembreKg.ToString()].Name + "]"
                    + " 	, [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DecembreKg.ToString()].Name + "]"
                    + " from"
                    + " ("
                    + " 	select 'Total' as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=1 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JanvierKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=2 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FevrierKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=3 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MarsKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=4 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AvrilKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=5 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MaiKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=6 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuinKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=7 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuilletKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=8 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AoutKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=9 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SeptembreKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=10 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.OctobreKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=11 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NovembreKg.ToString()].Name + "]"
                    + " 	, sum(case when month(DDechargement)=12 then PoidsChargement else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DecembreKg.ToString()].Name + "]"
                    + " 	from tblCommandeFournisseur";
                sqlStr += "         inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 	where year(DDechargement) in (" + filterYears + ")";
                if (eSF.FilterProduits != "") { sqlStr += " and tblCommandeFournisseur.RefProduit in (" + filterProduits + ")"; }
                if (filterCollecte != "") { sqlStr += filterCollecte; }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += filterDRs;
                    sqlStr += "))";
                }
                sqlStr += " ) as u2";
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString())
            {
                //Base SQL statement
                sqlStr = "select tblCommandeFournisseur.RefCommandeFournisseur [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , dbo.ListeDR(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]";
                }
                sqlStr += "     , case when tblEntite.CodeEE is not null then tblEntite.CodeEE + ' - ' else '' end + tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , tblCommandeFournisseur.CodePostal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineCP.ToString()].Name + "]";
                }
                sqlStr += "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                        + "     , tblAdresse.CodePostal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationCP.ToString()].Name + "]";
                }
                sqlStr += "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]";
                }
                sqlStr += "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , tbrCamionType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CamionTypeLibelle.ToString()].Name + "]";
                }
                sqlStr += "     , tblProduit.Libelle + case when tblContrat.RefContrat is null then '' else ' (' + tblContrat.IdContrat + ')' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , tblCommandeFournisseur.DCreation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDateCreation.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.DChargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargementPrevue.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.DDechargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargementPrevue.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.NbBalleChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleChargement.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.PoidsChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsChargement.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.PoidsReparti as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsReparti.ToString()].Name + "]"
                    + "     , isnull(repCITEO.Poids,0) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsRepartiCITEO.ToString()].Name + "]"
                    + "     , isnull(repAdelphe.Poids,0) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsRepartiAdelphe.ToString()].Name + "]"
                    + "     , isnull(repLeko.Poids,0) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsRepartiLeko.ToString()].Name + "]";
                }
                sqlStr += "     , DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                    + "     , NbBalleDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleDechargement.ToString()].Name + "]"
                    + "     , PoidsDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsDechargement.ToString()].Name + "]"
                    + "     , PrixTonneHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPrixTonneHT.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                    + "     , case when (tblCommandeFournisseur.DDechargement < convert(datetime,'2012-01-01 00:00:00',120) or unitaire.RefCommandeFournisseur is not null) then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(545) + "'"
                    + "         else case when DDechargement is not null then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(546) + "'"
                    + "         else case when RefusCamion = 1 then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(551) + "'"
                    + "         else case when tblCommandeFournisseur.RefCommandeFournisseurStatut is not null then (case when tbsCommandeFournisseurStatut.RefCommandeFournisseurStatut=1 then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(547) + "'"
                    + "         else case when tbsCommandeFournisseurStatut.RefCommandeFournisseurStatut=2 then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(548) + "' else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(549) + "' end end)"
                    + "         else '" + CurrentContext.CulturedRessources.GetTextSQLRessource(550) + "' end end end end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurStatut.ToString()].Name + "]"
                    + "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , PrixTransportHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPrixTransportHT.ToString()].Name + "]"
                        + "     , PrixTransportSupplementHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPrixTransportSupplementHT.ToString()].Name + "]"
                        + "     , SurcoutCarburantHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurSurcoutCarburantHT.ToString()].Name + "]"
                        + "     , PrixTransportHT + PrixTransportSupplementHT + SurcoutCarburantHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransportTotalPrix.ToString()].Name + "]"
                        + "     , km as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurKm.ToString()].Name + "]"
                        + "     , DAnomalieChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDAnomalieChargement.ToString()].Name + "]"
                        + "     , DTraitementAnomalieChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDTraitementAnomalieChargement.ToString()].Name + "]"
                        + "     , tbrMotifAnomalieChargement.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MotifAnomalieChargementLibelle.ToString()].Name + "]"
                        + "     , CmtAnomalieChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurCmtAnomalieChargement.ToString()].Name + "]"
                        + "     , DAnomalieTransporteur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDAnomalieTransporteur.ToString()].Name + "]"
                        + "     , DTraitementAnomalieTransporteur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDTraitementAnomalieTransporteur.ToString()].Name + "]"
                        + "     , tbrMotifAnomalieTransporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MotifAnomalieTransporteurLibelle.ToString()].Name + "]"
                        + "     , CmtAnomalieTransporteur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurCmtAnomalieTransporteur.ToString()].Name + "]"
                        + "     , DAnomalieClient as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDAnomalieClient.ToString()].Name + "]"
                        + "     , DTraitementAnomalieClient as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDTraitementAnomalieClient.ToString()].Name + "]"
                        + "     , tbrMotifAnomalieClient.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MotifAnomalieClientLibelle.ToString()].Name + "]"
                        + "     , CmtAnomalieClient as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurCmtAnomalieClient.ToString()].Name + "]"
                        + "     , VueRepartitionUnitaireDetail.PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHT.ToString()].Name + "]"
                        + "     , VueRepartitionUnitaireDetail.PUHTUnique as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PUHTUnique.ToString()].Name + "]"
                        ;
                }
                sqlStr += " from tblCommandeFournisseur"
                    + " 	left join (select RefCommandeFournisseur, Sum(Poids) as Poids from VueRepartitionUnitaireDetail"
                    + " 	    inner join tblEntite as fournisseur on VueRepartitionUnitaireDetail.RefFournisseur=fournisseur.RefEntite"
                    + "         where fournisseur.RefEcoOrganisme in(2) and Collecte=1 group by RefCommandeFournisseur) as repCITEO on tblCommandeFournisseur.RefCommandeFournisseur=repCITEO.RefCommandeFournisseur"
                    + " 	left join (select RefCommandeFournisseur, Sum(Poids) as Poids from VueRepartitionUnitaireDetail"
                    + " 	    inner join tblEntite as fournisseur on VueRepartitionUnitaireDetail.RefFournisseur=fournisseur.RefEntite"
                    + "         where fournisseur.RefEcoOrganisme in(1) and Collecte=1 group by RefCommandeFournisseur) as repAdelphe on tblCommandeFournisseur.RefCommandeFournisseur=repAdelphe.RefCommandeFournisseur"
                    + " 	left join (select RefCommandeFournisseur, Sum(Poids) as Poids from VueRepartitionUnitaireDetail"
                    + " 	    inner join tblEntite as fournisseur on VueRepartitionUnitaireDetail.RefFournisseur=fournisseur.RefEntite"
                    + "         where fournisseur.RefEcoOrganisme in(3) and Collecte=1 group by RefCommandeFournisseur) as repLeko on tblCommandeFournisseur.RefCommandeFournisseur=repLeko.RefCommandeFournisseur"
                    + "     inner join tblEntite on tblEntite.RefEntite=tblCommandeFournisseur.RefEntite"
                    + "     inner join tblEntite as transporteur on transporteur.RefEntite=tblCommandeFournisseur.RefTransporteur"
                    + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                    + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + "     left join tbrCamionType on tblCommandeFournisseur.RefCamionType=tbrCamionType.RefCamionType"
                    + "     left join tbrMotifAnomalieChargement on tblCommandeFournisseur.RefMotifAnomalieChargement=tbrMotifAnomalieChargement.RefMotifAnomalieChargement"
                    + "     left join tbrMotifAnomalieClient on tblCommandeFournisseur.RefMotifAnomalieClient=tbrMotifAnomalieClient.RefMotifAnomalieClient"
                    + "     left join tbrMotifAnomalieTransporteur on tblCommandeFournisseur.RefMotifAnomalieTransporteur=tbrMotifAnomalieTransporteur.RefMotifAnomalieTransporteur"
                    + "     left join (select distinct RefCommandeFournisseur from tblRepartition) as unitaire on tblCommandeFournisseur.RefCommandeFournisseur=unitaire.RefCommandeFournisseur "
                    + "     left join tbsCommandeFournisseurStatut on tbsCommandeFournisseurStatut.RefCommandeFournisseurStatut=tblCommandeFournisseur.RefCommandeFournisseurStatut"
                    + "     left join (select RefCommandeFournisseur, case when count(distinct PUHT)>1 then null else max(PUHT) end as PUHT, cast(case when count(distinct PUHT)>1 then 0 else 1 end as bit) as PUHTUnique"
                    + "         from VueRepartitionUnitaireDetail"
                    + "         group by RefCommandeFournisseur) as VueRepartitionUnitaireDetail on tblCommandeFournisseur.RefCommandeFournisseur=VueRepartitionUnitaireDetail.RefCommandeFournisseur"
                    + "     left join VueCommandeFournisseurContrat on tblCommandeFournisseur.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                    + "     left join tblContrat on tblContrat.RefContrat=VueCommandeFournisseurContrat.RefContrat"
                    + " where DDechargement between @begin and @end ";
                //Filtre DR
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterTransporteurs != "")
                {
                    sqlStr += " and transporteur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", eSF.FilterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCamionTypes != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefCamionType in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCamionType", eSF.FilterCamionTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and tblProduit.Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString())
            {
                //Base SQL statement
                sqlStr = "select RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                    + "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                    + "     , case when tblEntite.CodeEE is not null then tblEntite.CodeEE + ' - ' else '' end + tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                    + "     , left(tblCommandeFournisseur.CodePostal+'  ',2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineDpt.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]"
                    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                    + "     , D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                    + "     , DMoisDechargementPrevu as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMoisDechargementPrevu.ToString()].Name + "]"
                    + "     , DChargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargementPrevue.ToString()].Name + "]"
                    + "     , DDechargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargementPrevue.ToString()].Name + "]"
                    + "     , PoidsChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsChargement.ToString()].Name + "]"
                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                    + "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]"
                    + "     , DAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDAffretement.ToString()].Name + "]"
                    + "     , dbo.ProchainJourTravaille(DAffretement,2) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurLimiteExclusivite.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + " inner join tblEntite on tblEntite.RefEntite=tblCommandeFournisseur.RefEntite"
                    + " left join tblEntite as transporteur on transporteur.RefEntite=tblCommandeFournisseur.RefTransporteur"
                    + " inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + " left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                    + " left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + " where D between @begin and @end and DDechargement is null and RefCommandeFournisseurStatut in(1,2) and RefusCamion=0";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterTransporteurs != "")
                {
                    sqlStr += " and transporteur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", eSF.FilterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterDptDeparts != "")
                {
                    sqlStr += " and left(tblCommandeFournisseur.CodePostal+'00',2) in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("codePostal", eSF.FilterDptDeparts, ref cmd, Enumerations.EnvDataColumnDataType.text.ToString());
                    sqlStr += ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterMonths))
                {
                    sqlStr += " and month(DMoisDechargementPrevu) in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refMois", eSF.FilterMonths, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //Filtre DR
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuChargementAnnule.ToString())
            {
                //Base SQL statement
                sqlStr = "select RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                    + " 	, dbo.ListeDR(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + "     , case when tblEntite.CodeEE is not null then tblEntite.CodeEE + ' - ' else '' end + tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.CodePostal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineCP.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]"
                    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + "     , tblAdresse.CodePostal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationCP.ToString()].Name + "]"
                    + "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                    + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]"
                    + "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                    + "     , tbrCamionType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CamionTypeLibelle.ToString()].Name + "]"
                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.DCreation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDateCreation.ToString()].Name + "]"
                    + "     , D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                    + "     , DChargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargementPrevue.ToString()].Name + "]"
                    + "     , DDechargementPrevue as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargementPrevue.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                    + "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.PrixTransportHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPrixTransportHT.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.PrixTransportSupplementHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPrixTransportSupplementHT.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.SurcoutCarburantHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurSurcoutCarburantHT.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.PrixTransportHT + tblCommandeFournisseur.PrixTransportSupplementHT + tblCommandeFournisseur.SurcoutCarburantHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransportTotalPrix.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.Km as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurKm.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.DChargementAnnule as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargementAnnule.ToString()].Name + "]"
                    + "     , tblUtilisateur.Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurChargementAnnulePar.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.CmtChargementAnnule as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurCmtChargementAnnule.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + " inner join tblEntite on tblEntite.RefEntite=tblCommandeFournisseur.RefEntite"
                    + " left join tblEntite as transporteur on transporteur.RefEntite=tblCommandeFournisseur.RefTransporteur"
                    + " left join tbrCamionType on tbrCamionType.RefCamionType=tblCommandeFournisseur.RefCamionType"
                    + " inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + " left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                    + " left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + " left join tblUtilisateur on tblUtilisateur.RefUtilisateur=tblCommandeFournisseur.RefUtilisateurChargementAnnule"
                    + " where D between @begin and @end and ChargementAnnule=1";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterTransporteurs != "")
                {
                    sqlStr += " and transporteur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", eSF.FilterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //Filtre DR
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString())
            {
                //Base SQL statement
                if (CurrentContext.ConnectedUtilisateur?.CentreDeTri?.RefEntite != null)
                {
                    sqlStr = "select RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                        + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                        + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                        + "     , tblCommandeFournisseur.DCreation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDateCreation.ToString()].Name + "]"
                        + "     , D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                        + "     , cast(dbo.JoursTravailles(tblCommandeFournisseur.DCreation, D) as decimal) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDelaiCommande.ToString()].Name + "]"
                        + "     , DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]"
                        + "     , cast(dbo.JoursTravailles(D, DChargement) as decimal) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDelaiChargement.ToString()].Name + "]"
                        + "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]"
                        + " from tblCommandeFournisseur"
                        + " inner join tblEntite on tblEntite.RefEntite=tblCommandeFournisseur.RefEntite"
                        + " 	inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                        + " where DChargement between @begin and @end";
                }
                else
                {
                    sqlStr = "select RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                        + " 	, dbo.ListeDR(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                        + "     , case when tblEntite.CodeEE is not null then tblEntite.CodeEE + ' - ' else '' end + tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                        + "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]"
                        + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                        + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                        + "     , tblCommandeFournisseur.DCreation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDateCreation.ToString()].Name + "]"
                        + "     , D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                        + "     , cast(dbo.JoursTravailles(tblCommandeFournisseur.DCreation, D) as decimal) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDelaiCommande.ToString()].Name + "]"
                        + "     , DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]"
                        + "     , cast(dbo.JoursTravailles(D, DChargement) as decimal) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDelaiChargement.ToString()].Name + "]"
                        + " from tblCommandeFournisseur"
                        + " inner join tblEntite on tblEntite.RefEntite=tblCommandeFournisseur.RefEntite"
                        + " 	inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                        + " where DChargement between @begin and @end";
                }
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //Filtre DR
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                //CentreDeTri
                if (CurrentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite != null)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite =" + CurrentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite.ToString();
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatDesEnlevements.ToString())
            {
                //Base SQL statement
                sqlStr = "select RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.DCreation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDateCreation.ToString()].Name + "]"
                    + "     , D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                    + "     , DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]"
                    + "     , adrC.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]"
                    + "     , NbBalleChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleChargement.ToString()].Name + "]"
                    + "     , PoidsChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsChargement.ToString()].Name + "]"
                    + "     , PoidsReparti as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsReparti.ToString()].Name + "]"
                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                    + "     , DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur?.CentreDeTri?.AutoControle ==  true)
                {
                    sqlStr += ", LotControle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurLotControle.ToString()].Name + "]";
                }
                sqlStr += " from tblCommandeFournisseur"
                    + " 	left join tblAdresse as adrC on tblCommandeFournisseur.RefAdresse=adrC.RefAdresse"
                    + " 	inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 	left join tblEntite as transporteur on tblCommandeFournisseur.RefTransporteur=transporteur.RefEntite"
                    + " 	left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 	left join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                    + " where case when tblCommandeFournisseur.RefusCamion=1 then tblCommandeFournisseur.DMoisDechargementPrevu else tblCommandeFournisseur.DChargement end between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //CentreDeTri
                if (CurrentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite != null)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite =" + CurrentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite.ToString();
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString())
            {
                //Base SQL statement
                sqlStr = "select RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]"
                    + "     , NbBalleChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleChargement.ToString()].Name + "]"
                    + "     , PoidsChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsChargement.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + "     , adrC.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + " 	left join tblAdresse as adrC on tblCommandeFournisseur.RefAdresse=adrC.RefAdresse"
                    + " 	inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " where DChargement between @begin and @end and dbo.CommandeMixte(NumeroAffretement)=0"
                    + "     and (RefTransporteur is not null and RefAdresseClient is not null and DChargementPrevue is not null and DDechargementPrevue is not null and DChargement is not null and DDechargement is not null and PoidsDechargement<>0 and NbBalleDechargement<>0 and PoidsChargement<>0 and NbBalleChargement<>0)";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //CentreDeTri
                if (CurrentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite != null)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite =" + CurrentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite.ToString();
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatDesPoids.ToString())
            {
                string filterCollecte = "", filterEcoOrganismes = "";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    filterCollecte += " and tblProduit.Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    filterCollecte += " and isnull(tblProduit.Collecte,0)=0";
                }
                if (eSF.FilterEcoOrganismes != "")
                {
                    filterEcoOrganismes = " and collectivite.RefEcoOrganisme in (";
                    filterEcoOrganismes += Utils.Utils.CreateSQLParametersFromString("refEcoOrganisme", eSF.FilterEcoOrganismes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    filterEcoOrganismes += ")";
                }
                if (eSF.FilterEcoOrganismes == "")
                {
                    sqlStr = "select Produit as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                        + "     , cast(isnull(PoidsChargement,0) as decimal(20,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsChargementTonne.ToString()].Name + "]"
                        + "     , cast(isnull(PoidsReparti,0) as decimal(20,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDeclareTonne.ToString()].Name + "]"
                        + "     , cast(isnull(PoidsDechargement,0) as decimal(20,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDechargementTonne.ToString()].Name + "]"
                        + " from"
                        + " ("
                        + " 	select top 1000000000 tblProduit.Libelle as Produit"
                        + " 		, sum(PoidsChargement) as PoidsChargement, sum(PoidsReparti) as PoidsReparti, sum(PoidsDechargement) as PoidsDechargement"
                        + " 	from tblCommandeFournisseur "
                        + " 		inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                        + " 	where DDechargement between  @begin and @end";
                    if (eSF.FilterCollecte == "Collecte" || eSF.FilterCollecte == "HorsCollecte")
                    {
                        sqlStr += filterCollecte;
                    }
                    sqlStr += " 	group by tblProduit.Libelle"
                        + " 	order by tblProduit.Libelle"
                        + " ) as u"
                        + " union all"
                        + " select Produit as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                        + "     , cast(isnull(PoidsChargement,0) as decimal(20,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsChargementTonne.ToString()].Name + "]"
                        + "     , cast(isnull(PoidsReparti,0) as decimal(20,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDeclareTonne.ToString()].Name + "]"
                        + "     , cast(isnull(PoidsDechargement,0) as decimal(20,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDechargementTonne.ToString()].Name + "]"
                        + " from"
                        + " ("
                        + " 	select 'Total' as Produit"
                        + " 		, sum(PoidsChargement) as PoidsChargement, sum(PoidsReparti) as PoidsReparti, sum(PoidsDechargement) as PoidsDechargement"
                        + " 	from tblCommandeFournisseur "
                        + " 		inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                        + " 	where DDechargement between  @begin and @end";
                    if (eSF.FilterCollecte == "Collecte" || eSF.FilterCollecte == "HorsCollecte")
                    {
                        sqlStr += filterCollecte;
                    }
                    sqlStr += " ) as u2";
                }
                else
                {
                    sqlStr = "select Produit as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                        + "     , cast(isnull(Poids,0) as decimal(20,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDeclareTonne.ToString()].Name + "]"
                        + " from"
                        + " ("
                        + " 	select top 1000000000 tblProduit.Libelle as Produit, sum(Poids) as Poids"
                        + " 	from tblProduit"
                        + " 		inner join"
                        + " 			("
                        + " 			select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D "
                        + " 			from tblRepartition "
                        + " 				inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        + " 			union all"
                        + " 			select RefRepartition, RefFournisseur, RefProduit, D "
                        + " 			from tblRepartition"
                        + " 			where RefCommandeFournisseur is null"
                        + " 			) as rep on rep.RefProduit=tblProduit.RefProduit"
                        + " 			inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition"
                        + " 			inner join tblEntite as collectivite on tblRepartitionCollectivite.RefCollectivite=collectivite.RefEntite"
                        + " 	where D between  @begin and @end";
                    if (eSF.FilterCollecte == "Collecte" || eSF.FilterCollecte == "HorsCollecte")
                    {
                        sqlStr += filterCollecte;
                    }
                    if (eSF.FilterEcoOrganismes != "")
                    {
                        sqlStr += filterEcoOrganismes;
                    }
                    sqlStr += " 	group by tblProduit.Libelle"
                        + " 	order by tblProduit.Libelle"
                        + " ) as u"
                        + " union all"
                        + " select Produit as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                        + "     , cast(isnull(Poids,0) as decimal(20,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDeclareTonne.ToString()].Name + "]"
                        + " from"
                        + " ("
                        + " 	select 'Total' as Produit, sum(Poids) as Poids"
                        + " 	from tblProduit"
                        + " 		inner join"
                        + " 			("
                        + " 			select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D "
                        + " 			from tblRepartition "
                        + " 				inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        + " 			union all"
                        + " 			select RefRepartition, RefFournisseur, RefProduit, D "
                        + " 			from tblRepartition"
                        + " 			where RefCommandeFournisseur is null"
                        + " 			) as rep on rep.RefProduit=tblProduit.RefProduit"
                        + " 			inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition"
                        + " 			inner join tblEntite as collectivite on tblRepartitionCollectivite.RefCollectivite=collectivite.RefEntite"
                        + " 	where D between  @begin and @end";
                    if (eSF.FilterCollecte == "Collecte" || eSF.FilterCollecte == "HorsCollecte")
                    {
                        sqlStr += filterCollecte;
                    }
                    if (eSF.FilterEcoOrganismes != "")
                    {
                        sqlStr += filterEcoOrganismes;
                    }
                    sqlStr += " ) as u2";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatTonnageParProcess.ToString())
            {
                //Base SQL statement
                sqlStr = "select tbrProcess.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProcessLibelle.ToString()].Name + "]"
                    + "     , tblProduit.NomCommun + case when univers.RefContrat is null then '' else ' ('+tblContrat.IdContrat+')' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , composant.NomCommun as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ComposantLibelle.ToString()].Name + "]"
                    + "     , cast(Poids as decimal(15,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsTonne.ToString()].Name + "]"
                    + "     , PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixEuroHTTonne.ToString()].Name + "]"
                    + "     , univers.PUHTSurtri as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHTSurtri.ToString()].Name + "]"
                    + "     , univers.PUHTTransport as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHTTransport.ToString()].Name + "]"
                    + " 	, univers.PUHTNet as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixNetEuroHTTonne.ToString()].Name + "]"
                    + "     , cast(univers.PU/1000 as decimal(10,2)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MontantEuroHT.ToString()].Name + "]"
                    + "  from"
                    + "      (select RefProduit, RefProcess, RefComposant, sum(Poids) as Poids"
                    + "          , PUHT + PUHTSurtri + PUHTTransport as PUHT, PUHTSurtri, PUHTTransport"
                    + "          , PUHT as PUHTNet, PUHT * sum(Poids) as [PU]"
                    + "          , RefContrat"
                    + "          from"
                    + "         ("
                    + "         select VueRepartitionUnitaireDetail.RefProduit, VueRepartitionUnitaireDetail.RefProcess, VueRepartitionUnitaireDetail.RefComposant, Poids"
                    + "              , isnull(tbrPrixReprise.PUHTSurtri,0) as PUHTSurtri, isnull(tbrPrixReprise.PUHTTransport,0) as PUHTTransport, VueRepartitionUnitaireDetail.PUHT as PUHTNet, VueRepartitionUnitaireDetail.PUHT"
                    + "              , VueCommandeFournisseurContrat.RefContrat"
                    + "         from"
                    + "             VueRepartitionUnitaireDetail"
                    + "         	left join VueCommandeFournisseurContrat on VueRepartitionUnitaireDetail.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                    + "         	left join tbrPrixReprise on isnull(tbrPrixReprise.RefProcess,0)=isnull(VueRepartitionUnitaireDetail.RefProcess,0) and tbrPrixReprise.RefProduit=VueRepartitionUnitaireDetail.RefProduit "
                    + "         		and tbrPrixReprise.RefProduit=VueRepartitionUnitaireDetail.RefProduit"
                    + "         		and month(VueRepartitionUnitaireDetail.D)=month(tbrPrixReprise.D) and year(VueRepartitionUnitaireDetail.D)=year(tbrPrixReprise.D)"
                    + "         		and isnull(tbrPrixReprise.RefContrat,0)=isnull(VueCommandeFournisseurContrat.RefContrat,0)"
                    + "             where VueRepartitionUnitaireDetail.D between @begin and @end and VueRepartitionUnitaireDetail.Collecte=1";
                if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and 1!=1";
                }
                sqlStr += "         union all"
                    + "         select VueRepartitionUnitaireDetail.RefProduit, VueRepartitionUnitaireDetail.RefProcess, VueRepartitionUnitaireDetail.RefComposant, Poids"
                    + "              , 0 as PUHTSurtri, 0 as PUHTTransport, VueRepartitionUnitaireDetail.PUHT as PUHTNet, VueRepartitionUnitaireDetail.PUHT"
                    + "              , null as RefContrat"
                    + "         from"
                    + "             VueRepartitionUnitaireDetail"
                    + "             where VueRepartitionUnitaireDetail.D between @begin and @end and VueRepartitionUnitaireDetail.Collecte=0";
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and 1!=1";
                }
                sqlStr += "         ) as u"
                    + "          group by RefProduit, RefProcess, RefComposant, PUHT + PUHTSurtri + PUHTTransport, PUHTSurtri, PUHTTransport, PUHT, RefContrat"
                    + "           ) as univers"
                    + "      inner join tblProduit on univers.RefProduit = tblProduit.refProduit"
                    + "      left join tblProduit as composant on composant.RefProduit = univers.RefComposant"
                    + "      left join tbrProcess on tbrProcess.RefProcess = univers.RefProcess"
                    + "      left join tblContrat on tblContrat.RefContrat=univers.RefContrat"
                    + "     where 1=1";
                //Détail par collectivité
                //sqlStr = "select tbrProcess.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProcessLibelle.ToString()].Name + "]"
                //    + "     , tblProduit.NomCommun as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                //    + "     , composant.NomCommun as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ComposantLibelle.ToString()].Name + "]"
                //    + "     , cast(Poids as decimal(15,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsTonne.ToString()].Name + "]"
                //    + "     , PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixEuroHTTonne.ToString()].Name + "]"
                //    + "     , univers.PUHTSurtri as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHTSurtri.ToString()].Name + "]"
                //    + "     , univers.PUHTTransport as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHTTransport.ToString()].Name + "]"
                //    + " 	, univers.PUHTNet as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixNetEuroHTTonne.ToString()].Name + "]"
                //    + "     , cast(univers.PU/1000 as decimal(10,2)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MontantEuroHT.ToString()].Name + "]"
                //    + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CollectiviteLibelle.ToString()].Name + "]"
                //    + "  from"
                //    + "      (select RefProduit, RefProcess, RefComposant, sum(Poids) as Poids"
                //    + "          , PUHT + PUHTSurtri + PUHTTransport as PUHT, PUHTSurtri, PUHTTransport"
                //    + "          , PUHT as PUHTNet, PUHT * sum(Poids) as [PU], RefFournisseur"
                //    + "          from"
                //    + "         ("
                //    + "         select VueRepartitionUnitaireDetail.RefProduit, VueRepartitionUnitaireDetail.RefProcess, VueRepartitionUnitaireDetail.RefComposant, Poids"
                //    + "              , tbrPrixReprise.PUHTSurtri, tbrPrixReprise.PUHTTransport, VueRepartitionUnitaireDetail.PUHT as PUHTNet, VueRepartitionUnitaireDetail.PUHT"
                //    + "              , VueCommandeFournisseurContrat.RefContrat, VueRepartitionUnitaireDetail.RefFournisseur"
                //    + "         from"
                //    + "             VueRepartitionUnitaireDetail"
                //    + "         	left join VueCommandeFournisseurContrat on VueRepartitionUnitaireDetail.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                //    + "         	inner join tbrPrixReprise on tbrPrixReprise.RefProcess=VueRepartitionUnitaireDetail.RefProcess and tbrPrixReprise.RefProduit=VueRepartitionUnitaireDetail.RefProduit "
                //    + "         		and tbrPrixReprise.RefProduit=VueRepartitionUnitaireDetail.RefProduit"
                //    + "         		and month(VueRepartitionUnitaireDetail.D)=month(tbrPrixReprise.D) and year(VueRepartitionUnitaireDetail.D)=year(tbrPrixReprise.D)"
                //    + "         		and isnull(tbrPrixReprise.RefContrat,0)=isnull(VueCommandeFournisseurContrat.RefContrat,0)"
                //    + "             where VueRepartitionUnitaireDetail.D between @begin and @end and VueRepartitionUnitaireDetail.Collecte=1";
                //if (eSF.FilterCollecte == "HorsCollecte")
                //{
                //    sqlStr += " and 1!=1";
                //}
                //sqlStr += "         union all"
                //    + "         select VueRepartitionUnitaireDetail.RefProduit, VueRepartitionUnitaireDetail.RefProcess, VueRepartitionUnitaireDetail.RefComposant, Poids"
                //    + "              , 0 as PUHTSurtri, 0 as PUHTTransport, VueRepartitionUnitaireDetail.PUHT as PUHTNet, VueRepartitionUnitaireDetail.PUHT"
                //    + "              , null as RefContrat, VueRepartitionUnitaireDetail.RefFournisseur"
                //    + "         from"
                //    + "             VueRepartitionUnitaireDetail"
                //    + "             where VueRepartitionUnitaireDetail.D between @begin and @end and VueRepartitionUnitaireDetail.Collecte=0";
                //if (eSF.FilterCollecte == "Collecte")
                //{
                //    sqlStr += " and 1!=1";
                //}
                //sqlStr += "         ) as u"
                //    + "          group by RefProduit, RefProcess, RefComposant, PUHT + PUHTSurtri + PUHTTransport, PUHTSurtri, PUHTTransport, PUHT, RefFournisseur"
                //    + "           ) as univers"
                //    + "      inner join tblProduit on univers.RefProduit = tblProduit.refProduit"
                //    + "      inner join tblProduit as composant on composant.RefProduit = univers.RefComposant"
                //    + "      inner join tbrProcess on tbrProcess.RefProcess = univers.RefProcess"
                //    + "      inner join tblEntite on tblEntite.RefEntite = univers.RefFournisseur"
                //    + "     where 1=1";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProcesss != "")
                {
                    sqlStr += " and tbrProcess.RefProcess in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProcess", eSF.FilterProcesss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString())
            {
                //Base SQL statement
                sqlStr = "select isnull(GroupeType,'[NR]') as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingTypeLibelle.ToString()].Name + "]"
                    + " 	, isnull(Groupe,'[NR]') as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingLibelle.ToString()].Name + "]"
                    + " 	, isnull(Client,'[NR]') as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + " 	, isnull(Pays,'[NR]') as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]"
                    + " 	, PoidsAnneeRef as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageAnnuelPrecedent.ToString()].Name + "]"
                    + "     , PoidsJanvier as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Janvier.ToString()].Name + "]"
                    + "     , PoidsFevrier as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Fevrier.ToString()].Name + "]"
                    + "     , PoidsMars as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Mars.ToString()].Name + "]"
                    + "     , PoidsAvril as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Avril.ToString()].Name + "]"
                    + "     , PoidsMai as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Mai.ToString()].Name + "]"
                    + "     , PoidsJuin as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Juin.ToString()].Name + "]"
                    + "     , PoidsJuillet as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Juillet.ToString()].Name + "]"
                    + "     , PoidsAout as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Aout.ToString()].Name + "]"
                    + "     , PoidsSeptembre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Septembre.ToString()].Name + "]"
                    + "     , PoidsOctobre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Octobre.ToString()].Name + "]"
                    + "     , PoidsNovembre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Novembre.ToString()].Name + "]"
                    + "     , PoidsDecembre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Decembre.ToString()].Name + "]"
                    + "     , PoidsAnnee as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageAnnuel.ToString()].Name + "]"
                    + "     , Couleur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name + "]"
                    + " from ("
                    + " 	select tbrProduitGroupeReportingType.Libelle as GroupeType, tbrProduitGroupeReporting.Libelle as Groupe, tblEntite.Libelle as Client, tbrPays.Libelle as Pays"
                    + " 		, cast(sum(case when year(DDechargement)=(@year - 1) then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsAnneeRef"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=1 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsJanvier"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=2 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsFevrier"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=3 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsMars"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=4 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsAvril"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=5 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsMai"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=6 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsJuin"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=7 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsJuillet"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=8 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsAout"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=9 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsSeptembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=10 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsOctobre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=11 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsNovembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=12 then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsDecembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year then PoidsDechargement else 0 end) as decimal(15,3))/1000 as PoidsAnnee"
                    + "         , tbrProduitGroupeReporting.Couleur"
                    + " 	from tblCommandeFournisseur"
                    + " 		left join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 		left join tbrProduitGroupeReporting on tblProduit.RefProduitGroupeReporting=tbrProduitGroupeReporting.RefProduitGroupeReporting"
                    + " 		left join tbrProduitGroupeReportingType on tbrProduitGroupeReporting.RefProduitGroupeReportingType=tbrProduitGroupeReportingType.RefProduitGroupeReportingType"
                    + " 		left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 		left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                    + " 		left join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                    + " 	where (year(DDechargement) = (@year - 1) or (year(DDechargement) = @year and month(DDechargement) <= @month))"
                    + " 	group by tbrProduitGroupeReportingType.Libelle, tbrProduitGroupeReporting.Libelle, tblEntite.Libelle, tbrPays.Libelle, tbrProduitGroupeReporting.Couleur"
                    + " 	union all"
                    + " 	select tbrProduitGroupeReportingType.Libelle as GroupeType, tbrProduitGroupeReporting.Libelle as Groupe, 'TSR/Ecart de pesée' as Client, tbrPays.Libelle as Pays"
                    + " 		, cast(sum(case when year(DDechargement)=(@year - 1) then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsAnneeRef"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=1 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsJanvier"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=2 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsFevrier"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=3 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsMars"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=4 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsAvril"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=5 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsMai"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=6 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsJuin"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=7 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsJuillet"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=8 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsAout"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=9 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsSeptembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=10 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsOctobre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=11 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsNovembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=12 then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsDecembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year then PoidsReparti-PoidsDeChargement else 0 end) as decimal(15,3))/1000 as PoidsAnnee"
                    + "         , tbrProduitGroupeReporting.Couleur"
                    + " 	from tblCommandeFournisseur"
                    + " 		left join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 		left join tbrProduitGroupeReporting on tblProduit.RefProduitGroupeReporting=tbrProduitGroupeReporting.RefProduitGroupeReporting"
                    + " 		left join tbrProduitGroupeReportingType on tbrProduitGroupeReporting.RefProduitGroupeReportingType=tbrProduitGroupeReportingType.RefProduitGroupeReportingType"
                    + " 		left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 		left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                    + " 		left join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                    + " 	where (year(DDechargement) = (@year - 1) or (year(DDechargement) = @year and month(DDechargement) <= @month)) and Collecte=1"
                    + " 	group by tbrProduitGroupeReportingType.Libelle, tbrProduitGroupeReporting.Libelle, tbrPays.Libelle, tbrProduitGroupeReporting.Couleur"
                    + " 	) as univers";
                //Filters
                if (eSF.FilterYears != "")
                {
                    string[] crits = eSF.FilterYears.Split(',');
                    eSF.FilterYears = crits[0];
                    cmd.Parameters.Add("@year", SqlDbType.Int).Value = eSF.FilterYears;
                }
                if (eSF.FilterMonths != "")
                {
                    string[] crits = eSF.FilterMonths.Split(',');
                    eSF.FilterMonths = crits[0];
                    cmd.Parameters.Add("@month", SqlDbType.Int).Value = eSF.FilterMonths;
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
            {
                //Base SQL statement
                sqlStr = "select univers.GroupeType as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingTypeLibelle.ToString()].Name + "]"
                    + " 	, univers.Groupe as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingLibelle.ToString()].Name + "]"
                    + " 	, isnull(Region,'[NR]') as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RegionReportingLibelle.ToString()].Name + "]"
                    + " 	, isnull(Pays,'[NR]') as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].Name + "]"
                    + " 	, case when PoidsTotAnneeRef!=0 then PoidsAnneeRef/PoidsTotAnneeRef else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnnuelPrecedentRegionPercent.ToString()].Name + "]"
                    + " 	, PoidsAnneeRef as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageAnnuelPrecedent.ToString()].Name + "]"
                    + "     , case when PoidsTotJanvier!=0 then PoidsJanvier/PoidsTotJanvier else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JanvierRegionPercent.ToString()].Name + "]"
                    + "     , PoidsJanvier as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Janvier.ToString()].Name + "]"
                    + "     , case when PoidsTotFevrier!=0 then PoidsFevrier/PoidsTotFevrier else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FevrierRegionPercent.ToString()].Name + "]"
                    + "     , PoidsFevrier as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Fevrier.ToString()].Name + "]"
                    + "     , case when PoidsTotMars!=0 then PoidsMars/PoidsTotMars else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MarsRegionPercent.ToString()].Name + "]"
                    + "     , PoidsMars as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Mars.ToString()].Name + "]"
                    + " 	, case when PoidsTotAvril!=0 then PoidsAvril/PoidsTotAvril else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AvrilRegionPercent.ToString()].Name + "]"
                    + "     , PoidsAvril as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Avril.ToString()].Name + "]"
                    + "     , case when PoidsTotMai!=0 then PoidsMai/PoidsTotMai else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MaiRegionPercent.ToString()].Name + "]"
                    + "     , PoidsMai as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Mai.ToString()].Name + "]"
                    + "     , case when PoidsTotJuin!=0 then PoidsJuin/PoidsTotJuin else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuinRegionPercent.ToString()].Name + "]"
                    + "     , PoidsJuin as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Juin.ToString()].Name + "]"
                    + " 	, case when PoidsTotJuillet!=0 then PoidsJuillet/PoidsTotJuillet else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.JuilletRegionPercent.ToString()].Name + "]"
                    + "     , PoidsJuillet as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Juillet.ToString()].Name + "]"
                    + "     , case when PoidsTotAout!=0 then PoidsAout/PoidsTotAout else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AoutRegionPercent.ToString()].Name + "]"
                    + "     , PoidsAout as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Aout.ToString()].Name + "]"
                    + "     , case when PoidsTotSeptembre!=0 then PoidsSeptembre/PoidsTotSeptembre else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SeptembreRegionPercent.ToString()].Name + "]"
                    + "     , PoidsSeptembre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Septembre.ToString()].Name + "]"
                    + " 	, case when PoidsTotOctobre!=0 then PoidsOctobre/PoidsTotOctobre else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.OctobreRegionPercent.ToString()].Name + "]"
                    + "     , PoidsOctobre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Octobre.ToString()].Name + "]"
                    + "     , case when PoidsTotNovembre!=0 then PoidsNovembre/PoidsTotNovembre else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NovembreRegionPercent.ToString()].Name + "]"
                    + "     , PoidsNovembre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Novembre.ToString()].Name + "]"
                    + "     , case when PoidsTotDecembre!=0 then PoidsDecembre/PoidsTotDecembre else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DecembreRegionPercent.ToString()].Name + "]"
                    + "     , PoidsDecembre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Decembre.ToString()].Name + "]"
                    + "     , case when PoidsTotAnnee!=0 then PoidsAnnee/PoidsTotAnnee else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnnuelRegionPercent.ToString()].Name + "]"
                    + "     , PoidsAnnee as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageAnnuel.ToString()].Name + "]"
                    + " 	, Couleur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name + "]"
                    + " from ("
                    + " 	select isnull(tbrProduitGroupeReportingType.Libelle,'[NR]') as GroupeType, isnull(tbrProduitGroupeReporting.Libelle,'[NR]') as Groupe, tbrRegionReporting.Libelle as Region, tbrPays.Libelle as Pays"
                    + " 		, cast(sum(case when year(DDechargement)=(@year -1) then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsAnneeRef"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=1 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsJanvier"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=2 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsFevrier"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=3 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsMars"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=4 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsAvril"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=5 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsMai"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=6 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsJuin"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=7 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsJuillet"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=8 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsAout"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=9 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsSeptembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=10 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsOctobre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=11 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsNovembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=12 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsDecembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsAnnee"
                    + " 		, tbrProduitGroupeReporting.Couleur"
                    + " 	from tblCommandeFournisseur"
                    + " 		left join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 		left join tbrProduitGroupeReporting on tblProduit.RefProduitGroupeReporting=tbrProduitGroupeReporting.RefProduitGroupeReporting"
                    + " 		left join tbrProduitGroupeReportingType on tbrProduitGroupeReporting.RefProduitGroupeReportingType=tbrProduitGroupeReportingType.RefProduitGroupeReportingType"
                    + " 		left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 		left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                    + " 		left join tbrRegionReporting on tbrRegionReporting.RefRegionReporting=tbrPays.RefRegionReporting"
                    + " 		left join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                    + " 	where (year(DDechargement) = (@year -1) or (year(DDechargement) = @year and month(DDechargement) <= @month))"
                    + " 	group by isnull(tbrProduitGroupeReportingType.Libelle,'[NR]'), isnull(tbrProduitGroupeReporting.Libelle,'[NR]'), tbrRegionReporting.Libelle, tbrPays.Libelle, tbrProduitGroupeReporting.Couleur"
                    + " 	) as univers"
                    + " 	left join "
                    + " 	("
                    + " 	select isnull(tbrProduitGroupeReportingType.Libelle,'[NR]') as GroupeType, isnull(tbrProduitGroupeReporting.Libelle,'[NR]') as Groupe"
                    + " 		, cast(sum(case when year(DDechargement)=(@year -1) then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotAnneeRef"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=1 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotJanvier"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=2 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotFevrier"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=3 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotMars"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=4 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotAvril"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=5 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotMai"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=6 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotJuin"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=7 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotJuillet"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=8 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotAout"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=9 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotSeptembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=10 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotOctobre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=11 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotNovembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year and month(DDechargement)=12 then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotDecembre"
                    + " 		, cast(sum(case when year(DDechargement)=@year then case when collecte=1 then PoidsReparti else PoidsDechargement end else 0 end) as decimal(15,3))/1000 as PoidsTotAnnee"
                    + " 	from tblCommandeFournisseur"
                    + " 		left join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 		left join tbrProduitGroupeReporting on tblProduit.RefProduitGroupeReporting=tbrProduitGroupeReporting.RefProduitGroupeReporting"
                    + " 		left join tbrProduitGroupeReportingType on tbrProduitGroupeReporting.RefProduitGroupeReportingType=tbrProduitGroupeReportingType.RefProduitGroupeReportingType"
                    + " 		left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 		left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                    + " 		left join tbrRegionReporting on tbrRegionReporting.RefRegionReporting=tbrPays.RefRegionReporting"
                    + " 		left join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                    + " 	where (year(DDechargement) = (@year -1) or (year(DDechargement) = @year and month(DDechargement) <= @month))"
                    + " 	group by isnull(tbrProduitGroupeReportingType.Libelle,'[NR]'), isnull(tbrProduitGroupeReporting.Libelle,'[NR]')"
                    + " 	) as tot on univers.GroupeType=tot.GroupeType and univers.Groupe=tot.Groupe";
                //Filters
                if (eSF.FilterYears != "")
                {
                    string[] crits = eSF.FilterYears.Split(',');
                    eSF.FilterYears = crits[0];
                    cmd.Parameters.Add("@year", SqlDbType.Int).Value = eSF.FilterYears;
                }
                if (eSF.FilterMonths != "")
                {
                    string[] crits = eSF.FilterMonths.Split(',');
                    eSF.FilterMonths = crits[0];
                    cmd.Parameters.Add("@month", SqlDbType.Int).Value = eSF.FilterMonths;
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuSuiviFacturationHC.ToString())
            {
                //Base SQL statement
                sqlStr = "select tblRepartition.RefRepartition as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefRepartition.ToString()].Name + "]"
                    + "     , case when c.RefEntite is not null then dbo.ListeDR(c.RefEntite) else case when f.RefEntite is not null then dbo.ListeDR(f.RefEntite) else dbo.ListeDR(fournisseur.RefEntite) end end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + " 	, case when c.RefEntite is not null then c.CodeEE else case when f.RefEntite is not null then f.CodeEE else fournisseur.CodeEE end end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                    + " 	, isnull(c.Libelle, isnull(f.Libelle, fournisseur.Libelle)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                    + "     , cli.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + "     , isnull(p.Libelle, produit.Libelle) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + " 	, DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                    + " 	, tblRepartitionProduit.Poids as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsChargement.ToString()].Name + "]"
                    + " 	, tblCommandeFournisseur.PoidsDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsDechargement.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                    + "     , tblRepartitionProduit.PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixAchatEuroHT.ToString()].Name + "]"
                    + " from tblRepartition"
                    + "     left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur = tblCommandeFournisseur.RefCommandeFournisseur"
                    + "     left join tblEntite as f on tblCommandeFournisseur.RefEntite = f.RefEntite"
                    + "     left join tblProduit as p on tblCommandeFournisseur.RefProduit = p.RefProduit"
                    + "     left join tblEntite as fournisseur on tblRepartition.RefFournisseur = fournisseur.RefEntite"
                    + "     left join tblProduit as produit on tblRepartition.RefProduit = produit.RefProduit"
                    + "     inner join tblRepartitionProduit on tblRepartition.RefRepartition = tblRepartitionProduit.RefRepartition"
                    + "     left join tblEntite as c on tblRepartitionProduit.RefFournisseur = c.RefEntite"
                    + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient = tblAdresse.RefAdresse"
                    + "     left join tblEntite as cli on tblAdresse.RefEntite = cli.RefEntite"
                    + "     where isnull(tblCommandeFournisseur.DDechargement, tblRepartition.D) between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and (c.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")"
                        + " or f.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + "))";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteProduit.ToString())
            {
                string filterCDTs = "";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    filterCDTs = Utils.Utils.CreateSQLParametersFromString("refCDT", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                //First command for fields
                sqlStr = "select distinct tblProduit.Libelle as Produit"
                            + " from tblProduit"
                            + "     inner join"
                            + "         ("
                            + "         select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                            + "         from tblRepartition"
                            + "             inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur = tblRepartition.RefCommandeFournisseur"
                            + "         union all"
                            + "         select RefRepartition, RefFournisseur, RefProduit, D from tblRepartition where RefCommandeFournisseur is null) as rep on rep.RefProduit = tblProduit.RefProduit"
                            + " where D between @begin and @end";
                //Filters
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and rep.RefFournisseur in (";
                    sqlStr += filterCDTs;
                    sqlStr += ")";
                }
                //Tri
                sqlStr += " order by tblProduit.Libelle";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        s += "[" + dr.GetSqlString(0).ToString() + " (kg)],";
                    }
                    dr.Close();
                    if (s != "") { s = s.Substring(0, s.Length - 1); } else { s = " [0]"; }
                }
                cmd = new SqlCommand();
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    filterCDTs = Utils.Utils.CreateSQLParametersFromString("refCDT", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                //Base SQL statement
                sqlStr = "select distinct"
                    + "      Fournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CollectiviteLibelle.ToString()].Name + "]"
                    + "     ," + s
                    + " from"
                    + "     ("
                    + "     select tblEntite.Libelle+' ('+isnull(tblEntite.CodeEE,'[NR]')+')' as [Fournisseur], tblProduit.Libelle + ' (kg)' as [Produit], tblRepartitionCollectivite.Poids"
                    + "     from"
                    + "         ("
                    + "         select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                    + "         from tblRepartition inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur = tblRepartition.RefCommandeFournisseur"
                    + "         union all"
                    + "         select RefRepartition, RefFournisseur, RefProduit, D from tblRepartition where RefCommandeFournisseur is null"
                    + "         ) as rep inner join tblProduit on rep.RefProduit = tblProduit.RefProduit"
                    + "         inner join tblRepartitionCollectivite on rep.RefRepartition = tblRepartitionCollectivite.RefRepartition"
                    + "         inner join tblEntite on tblRepartitionCollectivite.RefCollectivite = tblEntite.RefEntite"
                    + " where D between @begin and @end";
                //Filters
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and rep.RefFournisseur in (";
                    sqlStr += filterCDTs;
                    sqlStr += ")";
                }
                sqlStr += "     union all"
                    + "     select isnull(tblEntite.Libelle, '[NR]')+' ('+isnull(tblEntite.CodeEE,'[NR]')+')' as [Fournisseur], tblProduit.Libelle + ' (kg)' as [Produit], tblRepartitionProduit.Poids"
                    + "     from"
                    + "         ("
                    + "         select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                    + "         from tblRepartition inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur = tblRepartition.RefCommandeFournisseur"
                    + "         union all"
                    + "         select RefRepartition, RefFournisseur, RefProduit, D from tblRepartition where RefCommandeFournisseur is null"
                    + "         ) as rep inner join tblProduit on rep.RefProduit = tblProduit.RefProduit"
                    + "         inner join tblRepartitionProduit on rep.RefRepartition = tblRepartitionProduit.RefRepartition"
                    + "         left join tblEntite on tblRepartitionProduit.RefFournisseur = tblEntite.RefEntite"
                    + " where D between @begin and @end";
                //Filters
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and rep.RefFournisseur in (";
                    sqlStr += filterCDTs;
                    sqlStr += ")";
                }
                sqlStr += "     ) as SourceTable"
                    + " pivot(sum(Poids)"
                    + " for Produit in(" + s + ")) as PivotTable";
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit.ToString())
            {
                sqlStr = "select dbo.ListeDR(VueRepartitionUnitaireDetail.RefFournisseur) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + " 	, tblProduit.Collecte as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Collecte.ToString()].Name + "]"
                    + " 	, coll.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCollectivite.ToString()].Name + "]"
                    + " 	, coll.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CollectiviteLibelle.ToString()].Name + "]"
                    + " 	, ee.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EcoOrganismeLibelle.ToString()].Name + "]"
                    + " 	, tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                    + " 	, tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]"
                    + " 	, tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + " 	, cast(sum(VueRepartitionUnitaireDetail.Poids) as decimal(20,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsDeclareTonne.ToString()].Name + "]"
                    + " from VueRepartitionUnitaireDetail"
                    + "     inner join tblCommandeFournisseur on VueRepartitionUnitaireDetail.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + "     inner join tblProduit on tblProduit.RefProduit = tblCommandeFournisseur.RefProduit"
                    + " 	inner join tblEntite on tblCommandeFournisseur.RefEntite=tblEntite.RefEntite"
                    + " 	inner join tblEntite as coll on VueRepartitionUnitaireDetail.RefFournisseur=coll.RefEntite"
                    + " 	left join tbrEcoOrganisme as ee on coll.RefEcoOrganisme=ee.RefEcoOrganisme"
                    + " where tblCommandeFournisseur.DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and tblProduit.Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                }
                if (eSF.FilterCollectivites != "")
                {
                    sqlStr += " and VueRepartitionUnitaireDetail.RefFournisseur in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCollectivite", eSF.FilterCollectivites, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and coll.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterEcoOrganismes != "")
                {
                    sqlStr += " and coll.RefEcoOrganisme in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refEcoOrganisme", eSF.FilterEcoOrganismes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //Finalize
                sqlStr += " group by dbo.ListeDR(VueRepartitionUnitaireDetail.RefFournisseur), tblProduit.Collecte, coll.CodeEE, coll.Libelle, ee.Libelle, tblEntite.CodeEE, tblEntite.Libelle, tblProduit.Libelle";
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString())
            {
                string filterCollectivites = "", filterYears = "", filterQuarters = "";
                //Filters
                if (eSF.FilterYears != "")
                {
                    filterYears = Utils.Utils.CreateSQLParametersFromString("y", eSF.FilterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterQuarters != "")
                {
                    filterQuarters = Utils.Utils.CreateSQLParametersFromString("q", eSF.FilterQuarters, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterCollectivites != "")
                {
                    filterCollectivites = Utils.Utils.CreateSQLParametersFromString("refCollectivite", eSF.FilterCollectivites, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                //First command for fields
                sqlStr = "select distinct composant"
                    + " from"
                    + "     ("
                    + "     select tblProduit.Libelle + '#/#' + composant.Libelle as [Composant]"
                    + "     from"
                    + "         ("
                    + "         select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                    + "         from tblRepartition inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur = tblRepartition.RefCommandeFournisseur"
                    + "         union all"
                    + "         select RefRepartition, RefFournisseur, RefProduit, D from tblRepartition where RefCommandeFournisseur is null"
                    + "         ) as rep inner join tblProduit on rep.RefProduit = tblProduit.RefProduit"
                    + "         inner join tblRepartitionCollectivite on rep.RefRepartition = tblRepartitionCollectivite.RefRepartition"
                    + "         inner join tblProduit as composant on tblRepartitionCollectivite.RefProduit = composant.RefProduit"
                    + "     where year(D) in (" + filterYears + ")";
                //Filters
                if (eSF.FilterQuarters != "")
                {
                    sqlStr += " and datepart(quarter,D) in (" + filterQuarters + ")";
                }
                if (eSF.FilterCollectivites != "")
                {
                    sqlStr += " and tblRepartitionCollectivite.RefCollectivite in (" + filterCollectivites + ")";
                }
                sqlStr += "     union all"
                            + "     select tblProduit.Libelle + '#/#' + composant.Libelle as [Composant]"
                            + "     from"
                            + "         ("
                            + "         select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                            + "         from tblRepartition inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur = tblRepartition.RefCommandeFournisseur"
                            + "         union all"
                            + "         select RefRepartition, RefFournisseur, RefProduit, D from tblRepartition where RefCommandeFournisseur is null"
                            + "         ) as rep inner join tblProduit on rep.RefProduit = tblProduit.RefProduit"
                            + "         inner join tblRepartitionProduit on rep.RefRepartition = tblRepartitionProduit.RefRepartition"
                            + "         inner join tblProduit as composant on tblRepartitionProduit.RefProduit = composant.RefProduit"
                            + "     where year(D) in (" + filterYears + ")";
                //Filters
                if (eSF.FilterQuarters != "")
                {
                    sqlStr += " and datepart(quarter,D) in (" + filterQuarters + ")";
                }
                if (eSF.FilterCollectivites != "")
                {
                    sqlStr += " and tblRepartitionProduit.RefFournisseur in (" + filterCollectivites + ")";
                }
                sqlStr += "     ) as univers order by composant";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        s += "[" + dr.GetSqlString(0).ToString() + " (kg)],";
                    }
                    dr.Close();
                    if (s != "") { s = s.Substring(0, s.Length - 1); } else { s = " [0]"; }
                }
                cmd = new SqlCommand();
                //Filters
                if (eSF.FilterYears != "")
                {
                    filterYears = Utils.Utils.CreateSQLParametersFromString("y", eSF.FilterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterQuarters != "")
                {
                    filterQuarters = Utils.Utils.CreateSQLParametersFromString("q", eSF.FilterQuarters, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterCollectivites != "")
                {
                    filterCollectivites = Utils.Utils.CreateSQLParametersFromString("refCollectivite", eSF.FilterCollectivites, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                //Base SQL statement
                sqlStr = "select distinct"
                    + "      Fournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]"
                    + "      , Trimestre as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TrimestreNb.ToString()].Name + "]"
                    + "     , " + s
                    + " from"
                    + "     ("
                    + "     select tblEntite.Libelle +' ('+isnull(tblEntite.CodeEE,'[NR]')+')' as [Fournisseur], datepart(quarter, rep.D) as Trimestre, tblProduit.Libelle + '#/#' + composant.Libelle + ' (kg)' as [Composant], tblRepartitionCollectivite.Poids"
                    + "     from"
                    + "         ("
                    + "         select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                    + "         from tblRepartition inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur = tblRepartition.RefCommandeFournisseur"
                    + "         union all"
                    + "         select RefRepartition, RefFournisseur, RefProduit, D from tblRepartition where RefCommandeFournisseur is null"
                    + "         ) as rep inner join tblProduit on rep.RefProduit = tblProduit.RefProduit"
                    + "         inner join tblRepartitionCollectivite on rep.RefRepartition = tblRepartitionCollectivite.RefRepartition"
                    + "         inner join tblProduit as composant on tblRepartitionCollectivite.RefProduit = composant.RefProduit"
                    + "         inner join tblEntite on rep.RefFournisseur = tblEntite.RefEntite"
                    + "     where year(D) in (" + filterYears + ")";
                //Filters
                if (eSF.FilterQuarters != "")
                {
                    sqlStr += " and datepart(quarter,D) in (" + filterQuarters + ")";
                }
                if (eSF.FilterCollectivites != "")
                {
                    sqlStr += " and tblRepartitionCollectivite.RefCollectivite in (" + filterCollectivites + ")";
                }
                sqlStr += "     union all"
                    + "     select tblEntite.Libelle +' ('+isnull(tblEntite.CodeEE,'[NR]')+')' as [Fournisseur], datepart(quarter, rep.D) as Trimestre, tblProduit.Libelle + '#/#' + composant.Libelle + ' (kg)' as [Composant], tblRepartitionProduit.Poids"
                    + "     from"
                    + "         ("
                    + "         select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                    + "         from tblRepartition inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur = tblRepartition.RefCommandeFournisseur"
                    + "         union all"
                    + "         select RefRepartition, RefFournisseur, RefProduit, D from tblRepartition where RefCommandeFournisseur is null"
                    + "         ) as rep inner join tblProduit on rep.RefProduit = tblProduit.RefProduit"
                    + "         inner join tblRepartitionProduit on rep.RefRepartition = tblRepartitionProduit.RefRepartition"
                    + "         inner join tblProduit as composant on tblRepartitionProduit.RefProduit = composant.RefProduit"
                    + "         left join tblEntite on rep.RefFournisseur = tblEntite.RefEntite"
                    + "     where year(D) in (" + filterYears + ")";
                //Filters
                if (eSF.FilterQuarters != "")
                {
                    sqlStr += " and datepart(quarter,D) in (" + filterQuarters + ")";
                }
                if (eSF.FilterCollectivites != "")
                {
                    sqlStr += " and tblRepartitionProduit.RefFournisseur in (" + filterCollectivites + ")";
                }
                sqlStr += "     ) as SourceTable"
                    + " pivot(sum(Poids)"
                    + " for Composant in(" + s + ")) as PivotTable";
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuTonnageCLSousContrat.ToString())
            {
                //Base SQL statement
                sqlStr = "select dbo.ListeDR(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                    + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CollectiviteLibelle.ToString()].Name + "]"
                    + "     , tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , cast(cast(isnull(Poids,0) as decimal)/1000 as decimal(10,3)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageAnnuel.ToString()].Name + "]"
                    + " from tblEntite"
                    + "     inner join (select distinct RefEntite from tblContratCollectivite where @year between year(DDebut) and year(DFin)) as tblContratCollectivite on tblEntite.RefEntite = tblContratCollectivite.RefEntite"
                    + "  left join"
                    + " ( select tblRepartitionCollectivite.RefCollectivite, rep.RefProduit, sum(Poids) as Poids"
                    + "          from"
                    + "              ("
                    + "                  select RefRepartition, tblCommandeFournisseur.RefProduit"
                    + "                  from tblRepartition"
                    + "                      inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur = tblRepartition.RefCommandeFournisseur"
                    + "                  where year(tblCommandeFournisseur.DDechargement) = @year"
                    + "                  union all"
                    + "                  select RefRepartition, RefProduit"
                    + "                  from tblRepartition"
                    + "                  where RefCommandeFournisseur is null and year(D) = @year) as rep"
                    + "              inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition = rep.RefRepartition"
                    + "          group by tblRepartitionCollectivite.RefCollectivite, rep.RefProduit"
                    + "  	) as univers on tblEntite.RefEntite = univers.RefCollectivite"
                    + "     left join tblProduit on univers.RefProduit = tblProduit.refProduit"
                    + " where 1=1";
                //Filters
                if (eSF.FilterYears != "")
                {
                    string[] crits = eSF.FilterYears.Split(',');
                    eSF.FilterYears = crits[0];
                    cmd.Parameters.Add("@year", SqlDbType.Int).Value = eSF.FilterYears;
                }
                //Filtre DR
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuListeProduit.ToString())
            {
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
                    + "     , Cmt as[" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitCmt.ToString()].Name + "]"
                    + "     , CmtFournisseur as[" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitCmtFournisseur.ToString()].Name + "]"
                    + "     , CmtTransporteur as[" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitCmtTransporteur.ToString()].Name + "]"
                    + "     , CmtClient as[" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitCmtClient.ToString()].Name + "]"
                    + " from tblProduit"
                    + "     left join tbrSAGECodeTransport on tblProduit.RefSAGECodeTransport = tbrSAGECodeTransport.RefSAGECodeTransport"
                    + "     left join tbrProduitGroupeReporting on tblProduit.RefProduitGroupeReporting = tbrProduitGroupeReporting.RefProduitGroupeReporting"
                    + "     left join tblApplicationProduitOrigine on tblProduit.RefApplicationProduitOrigine = tblApplicationProduitOrigine.RefApplicationProduitOrigine"
                    + "     left join (select distinct RefProduit from tbmDescriptionControleProduit) as cB on tblProduit.RefProduit=cB.RefProduit"
                    + "     left join(select distinct RefProduit from tbmDescriptionCVQProduit) as cVQ on tblProduit.RefProduit = cVQ.RefProduit"
                    + " where 1=1";
                //Filters
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and tblProduit.Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                }
                if (eSF.FilterActif)
                {
                    sqlStr += " and tblProduit.Actif=1";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString())
            {
                //Base SQL statement
                sqlStr = "select dbo.ListeDR(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + " 	, tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                    + " 	, tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                    + " 	, cast(round((cast(sum(PoidsChargement) as decimal)) / 1000, 3) as decimal(10, 3)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageTotalChargement.ToString()].Name + "]"
                    + " 	, cast(round((cast(sum(PoidsReparti) as decimal)) / 1000, 3) as decimal(10, 3)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageTotalDeclare.ToString()].Name + "]"
                    + " 	, cast(round((cast(sum(PoidsChargement) / Count(*) as decimal)) / 1000, 3) as decimal(10, 3)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageMoyen.ToString()].Name + "]"
                    + " 	, count(tblCommandeFournisseur.RefCommandeFournisseur) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NbTotalCamion.ToString()].Name + "]"
                    //                            + " 	, cast(round((cast(sum(case when tblProduit.IncitationQualite=1 then PoidsChargement else 0 end) as decimal))/ 1000,3) as decimal(10, 3)) as [Tonnage sous contrat]"
                    + " 	, cast(round((cast(sum(case when tblProduit.IncitationQualite=1 and rec.RefCommandeFournisseur is null then PoidsChargement else 0 end) as decimal))/ 1000,3) as decimal(10, 3)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageSoumisInteressement.ToString()].Name + "]"
                    + " 	, sum(case when tblProduit.IncitationQualite = 1 then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NbLotSoumisInteressement.ToString()].Name + "]"
                    + " 	, count(rec.RefReclamation) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NbLotNonConforme.ToString()].Name + "]"
                    + " 	, case when sum(case when tblProduit.IncitationQualite = 1 then 1 else 0 end) != 0 then"
                    + "         cast(round(cast(count(rec.RefReclamation) as decimal)/sum(case when tblProduit.IncitationQualite=1 then 1 else 0 end),4) as decimal(10,4))"
                    + " 		else null end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TauxNonConformite.ToString()].Name + "]"
                    + " 	, case when count(rec.RefReclamation) * 50 <= sum(case when tblProduit.IncitationQualite = 1 then 1 else 0 end) then cast(round(cast(sum(case when tblProduit.IncitationQualite=1 and rec.RefCommandeFournisseur is null then PoidsChargement else 0 end) as decimal)/ 1000 * Mnt.Montant,2) as decimal(10, 2)) else 0 end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MontantInteressement.ToString()].Name + "]"
                    + "     , cast(case when sousContrat.RefEntite is not null then 1 else 0 end as bit) as[" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SousContratIncitationQualite.ToString()].Name + "]"
                    + " 	, tblEntite.AutoControle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MiseEnPlaceAutocontrole.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + "     inner join tblEntite on tblCommandeFournisseur.RefEntite = tblEntite.RefEntite"
                    + "     inner join tblProduit on tblCommandeFournisseur.RefProduit = tblProduit.RefProduit"
                    + "     left join"
                    + "     (select distinct RefReclamation, RefCommandeFournisseur"
                    + "         from tblReclamation"
                    + "             inner join tblFicheControle on tblReclamation.RefFicheControle = tblFicheControle.RefFicheControle"
                    + "             left join tbrReclamationFamille on tblReclamation.RefReclamationFamille = tbrReclamationFamille.RefReclamationFamille"
                    + "             left join tbrReclamationType on tblReclamation.RefReclamationType = tbrReclamationType.RefReclamationType"
                    + "         where tbrReclamationFamille.IncitationQualite = 1 and tbrReclamationType.IncitationQualite = 1 and tblReclamation.Valide = 1"
                    + "         union"
                    + "         select distinct tblNonConformite.RefNonConformite as RefReclamation, RefCommandeFournisseur"
                    + "         from tblNonConformite"
                    + "             inner join tbrNonConformiteNature on tblNonConformite.RefNonConformiteNature = tbrNonConformiteNature.RefNonConformiteNature"
                    + "             inner join tbmNonConformiteNonConformiteFamille on tblNonConformite.RefNonConformite = tbmNonConformiteNonConformiteFamille.RefNonConformite"
                    + "             inner join tbrNonConformiteFamille on tbmNonConformiteNonConformiteFamille.RefNonConformiteFamille = tbrNonConformiteFamille.RefNonConformiteFamille"
                    + "             left join tblNonConformiteEtape on tblNonConformite.RefNonConformite = tblNonConformiteEtape.RefNonConformite"
                    + "         where tbrNonConformiteNature.IncitationQualite = 1 and tbrNonConformiteFamille.IncitationQualite = 1 and tblNonConformiteEtape.RefNonConformiteEtapeType = 4 and tblNonConformiteEtape.DValide is not null"
                    + " ) as rec on tblCommandeFournisseur.RefCommandeFournisseur = rec.RefCommandeFournisseur"
                    + "     left join(select distinct RefEntite from tblContratIncitationQualite where year(DDebut) = @year) as sousContrat on tblEntite.RefEntite = sousContrat.RefEntite"
                    + "     , (select Montant from tbrMontantIncitationQualite where Annee=@year) as mnt"
                    + " where case when tblCommandeFournisseur.RefusCamion=1 then year(tblCommandeFournisseur.DMoisDechargementPrevu) else year(tblCommandeFournisseur.DChargement) end = @year"
                    + "     and tblProduit.IncitationQualite = 1";
                //Filters
                if (eSF.FilterYears != "")
                {
                    string[] crits = eSF.FilterYears.Split(',');
                    eSF.FilterYears = crits[0];
                    cmd.Parameters.Add("@year", SqlDbType.Int).Value = eSF.FilterYears;
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                if (!string.IsNullOrEmpty(eSF.FilterContrat))
                {
                    switch (eSF.FilterContrat)
                    {
                        case "SousContrat":
                            sqlStr += " and sousContrat.RefEntite is not null";
                            break;
                        case "HorsContrat":
                            sqlStr += " and sousContrat.RefEntite is null";
                            break;
                    }
                }
                sqlStr += " group by tblEntite.RefEntite, tblEntite.CodeEE, tblEntite.Libelle, cast(case when sousContrat.RefEntite is not null then 1 else 0 end as bit), tblEntite.AutoControle, Mnt.Montant";
                if (eSF.FilterMoins10Euros)
                {
                    sqlStr += " having ((case when count(rec.RefReclamation) * 50 <= count(tblCommandeFournisseur.RefCommandeFournisseur) then cast(round(cast(sum(case when tblProduit.IncitationQualite=1 and rec.RefCommandeFournisseur is null then PoidsChargement else 0 end) as decimal)/ 1000 * Mnt.Montant,2) as decimal(10, 2)) else 0 end) <10)";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString())
            {
                //Base SQL statement
                sqlStr = "select client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + "     , month(DDechargement) AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MoisNb.ToString()].Name + "]"
                    + "     , year(DDechargement) AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnneeNb.ToString()].Name + "]"
                    + " 	, count(tblCommandeFournisseur.RefCommandeFournisseur) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbTotalLivraison.ToString()].Name + "]"
                    + "     , sum(case when _controleActif.RefProduit is not null or _cVQActif.RefProduit is not null then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbTotalLivraisonAControler.ToString()].Name + "]"
                    + "     , sum(case when controles.RefFicheControle is not null or cVQs.RefFicheControle is not null then 1 else 0 end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbTotalLivraisonControlees.ToString()].Name + "]"
                    + "     , case when sum(case when _controleActif.RefProduit is not null or _cVQActif.RefProduit is not null then 1 else 0 end)!=0 then cast((cast(sum(case when controles.RefFicheControle is not null or cVQs.RefFicheControle is not null then 1 else 0 end) as decimal(10,2))/sum(case when _controleActif.RefProduit is not null or _cVQActif.RefProduit is not null then 1 else 0 end)*100) as decimal(10,2)) else null end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurTauxControle.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     left join tblFicheControle on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                    + "     left join (select RefFicheControle, count(*) as Nb from tblControle group by RefFicheControle) as controles on tblFicheControle.RefFicheControle=controles.RefFicheControle"
                    + "     left join (select RefFicheControle, count(*) as Nb from tblCVQ group by RefFicheControle) as cVQs on tblFicheControle.RefFicheControle=cVQs.RefFicheControle"
                    + "     left join"
                    + "         (select distinct RefProduit from tbmDescriptionControleProduit"
                    + "         inner join tbrDescriptionControle on tbmDescriptionControleProduit.RefDescriptionControle=tbrDescriptionControle.RefDescriptionControle"
                    + "         where RefProduit is not null and isnull(tbrDescriptionControle.Actif,0)=1) as _controleActif on tblCommandeFournisseur.RefProduit=_controleActif.RefProduit"
                    + "     left join"
                    + "         (select distinct RefProduit from tbrDescriptionCVQ inner join tbmDescriptionCVQProduit on tbrDescriptionCVQ.RefDescriptionCVQ=tbmDescriptionCVQProduit.RefDescriptionCVQ where Actif=1) as _cVQActif on _cVQActif.RefProduit=tblCommandeFournisseur.RefProduit"
                    + " where DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                if (!string.IsNullOrEmpty(eSF.FilterContrat))
                {
                    switch (eSF.FilterContrat)
                    {
                        case "SousContrat":
                            sqlStr += " and Client.SousContrat=1";
                            break;
                        case "HorsContrat":
                            sqlStr += " and Client.SousContrat=0";
                            break;
                    }
                }
                sqlStr += " group by client.Libelle, month(DDechargement), year(DDechargement)";
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString())
            {
                //Eléments de description de la réception
                //First command for fields
                sqlStr = "select distinct tblFicheControleDescriptionReception.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + ", tblFicheControleDescriptionReception.RefDescriptionReception"
                    + " from tblFicheControleDescriptionReception inner join tblFicheControle on tblFicheControleDescriptionReception.RefFicheControle=tblFicheControle.RefFicheControle"
                    + "     inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                    + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + " where tblCommandeFournisseur.DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterContrat))
                {
                    switch (eSF.FilterContrat)
                    {
                        case "SousContrat":
                            sqlStr += " and Client.SousContrat=1";
                            break;
                        case "HorsContrat":
                            sqlStr += " and Client.SousContrat=0";
                            break;
                    }
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                //Tri
                sqlStr += " order by tblFicheControleDescriptionReception.RefDescriptionReception";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        s += "[" + dr.GetSqlString(0).ToString() + "],";
                    }
                    dr.Close();
                }
                if (s != "")
                {
                    s = s.Substring(0, s.Length - 1);
                }
                cmd = new SqlCommand();
                //Base SQL statement
                sqlStr = "select tblFicheControle.RefFicheControle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefFicheControle.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + "     , dbo.ListeDR(fournisseur.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null) { sqlStr += "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"; }
                sqlStr += "     , NumeroLotUsine as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleNumeroLotUsine.ToString()].Name + "]"
                    + "     , DDechargement AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                    + "     , case when fournisseur.CodeEE is not null then fournisseur.CodeEE + ' - ' else '' end + fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                    + "     , tblProduit.Libelle AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , PoidsDechargement AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsDechargement.ToString()].Name + "]"
                    + "     , NbBalleDechargement AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleDechargement.ToString()].Name + "]"
                    + "     , isnull(tblContact.Prenom,'') + ' ' + tblContact.Nom AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleControleur.ToString()].Name + "]"
                    + "     , Reserve AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleReserve.ToString()].Name + "]"
                    + "     , tblFicheControle.Cmt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleCmt.ToString()].Name + "]"
                    + "     " + (string.IsNullOrEmpty(s) ? "" : ", " + s)
                    + "     , cast(case when tblNonConformite.RefCommandeFournisseur is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNonConformite.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + "     left join tblNonConformite on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                    + "     inner join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     inner join tblFicheControle on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                    + "     left join tbmContactAdresse on tblFicheControle.RefControleur=tbmContactAdresse.RefContactAdresse"
                    + "     left join tblContact on tbmContactAdresse.RefContact = tblContact.RefContact";
                if (!string.IsNullOrEmpty(s))
                {
                    sqlStr += "     inner join "
                    + "     ("
                    + "         select RefFicheControle, " + s
                    + "         from (select RefFicheControle, case when Positif=1 then tblFicheControleDescriptionReception.Oui" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " else tblFicheControleDescriptionReception.Non" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " end as Descr, tblFicheControleDescriptionReception.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " from tblFicheControleDescriptionReception) as SourceTable"
                    + "         pivot"
                    + "         (max(Descr)"
                    + "         for Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " in (" + s + ")"
                    + "         ) as PivotTable"
                    + "     )as descriptionReception"
                    + "     on tblFicheControle.RefFicheControle=descriptionReception.RefFicheControle";
                }
                sqlStr += " where tblCommandeFournisseur.DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterContrat))
                {
                    switch (eSF.FilterContrat)
                    {
                        case "SousContrat":
                            sqlStr += " and Client.SousContrat=1";
                            break;
                        case "HorsContrat":
                            sqlStr += " and Client.SousContrat=0";
                            break;
                    }
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.QualiteMenuExtractionControle.ToString())
            {
                //Eléments de description des CB
                //First command for fields
                sqlStr = "select tblControleDescriptionControle.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + ", max(tblControleDescriptionControle.Ordre)"
                    + " from tblControle"
                    + "     inner join tblFicheControle on tblFicheControle.RefFicheControle=tblControle.RefFicheControle"
                    + "     inner join tblControleDescriptionControle on tblControleDescriptionControle.RefControle=tblControle.RefControle"
                    + "     inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                    + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + " where tblCommandeFournisseur.DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                //Regroupement
                sqlStr += " group by tblControleDescriptionControle.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR");
                //Tri
                sqlStr += " order by max(tblControleDescriptionControle.Ordre)";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        s += "[" + dr.GetSqlString(0).ToString() + "],";
                    }
                    dr.Close();
                }
                if (s != "")
                {
                    s = s.Substring(0, s.Length - 1);
                }
                cmd = new SqlCommand();
                //Base SQL statement
                sqlStr = "select tblFicheControle.RefFicheControle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefFicheControle.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + "     , dbo.ListeDR(fournisseur.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null) { sqlStr += "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"; }
                sqlStr += "     , NumeroLotUsine as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleNumeroLotUsine.ToString()].Name + "]"
                    + "     , case when fournisseur.CodeEE is not null then fournisseur.CodeEE + ' - ' else '' end + fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                    + "     , tblProduit.Libelle AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , DDechargement AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                    + "     , DControle AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ControleDControle.ToString()].Name + "]"
                    + "     , isnull(tblContact.Prenom,'') + ' ' + tblContact.Nom AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleControleur.ToString()].Name + "]"
                    + "     , tblControle.Poids as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ControlePoids.ToString()].Name + "]"
                    + "     , tblControle.Cmt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ControleCmt.ToString()].Name + "]"
                    + "     " + (string.IsNullOrEmpty(s) ? "" : ", " + s)
                    + "     , tot.PoidsTotal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ControlePoidsTotal.ToString()].Name + "]"
                    + "     , tot.PoidsTotalIndesirable as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ControlePoidsTotalIndesirable.ToString()].Name + "]"
                    + "     , tot.PourcentageIndesirable as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ControlePourcentageIndesirable.ToString()].Name + "]"
                    + "     , cast(case when tblNonConformite.RefCommandeFournisseur is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNonConformite.ToString()].Name + "]"
                    + " from tblControle"
                    + "     inner join tblFicheControle on tblFicheControle.RefFicheControle=tblControle.RefFicheControle"
                    + "     inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                    + "     left join tblNonConformite on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                    + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     inner join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + "     left join tbmContactAdresse on tblControle.RefControleur=tbmContactAdresse.RefContactAdresse"
                    + "     left join tblContact on tbmContactAdresse.RefContact = tblContact.RefContact"
                    + "     left join ("
                    + "         select tblControleDescriptionControle.RefControle, cast(sum(tblControleDescriptionControle.Poids) as decimal(10, 3)) as [PoidsTotal]"
                    + "         , cast(sum(case when CalculLimiteConformite = 1 then tblControleDescriptionControle.Poids else 0 end) as decimal(10, 3)) as [PoidsTotalIndesirable]"
                    + "         , case when sum(tblControleDescriptionControle.Poids)= 0 then null"
                    + "             else cast(sum(round(cast(case when CalculLimiteConformite = 1 then tblControleDescriptionControle.Poids else 0 end as decimal(10, 3)) / tblControle.Poids, 4)) as decimal(10, 4)) end as PourcentageIndesirable"
                    + "         from tblControleDescriptionControle inner join tblControle on tblControleDescriptionControle.RefControle=tblControle.RefControle"
                    + "         group by tblControleDescriptionControle.RefControle, tblControle.Poids) as tot on tot.RefControle=tblControle.refControle";
                if (!string.IsNullOrEmpty(s))
                {
                    sqlStr += "     inner join "
                    + "     ("
                    + "         select RefControle, " + s
                    + "         from (select tblControle.RefControle, replace(cast(cast((tblControleDescriptionControle.Poids/tblControle.Poids)*100 as decimal(10,2)) as nvarchar)+'%','.',',') as poids, tblControleDescriptionControle.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " from tblControleDescriptionControle inner join tblControle on tblControleDescriptionControle.RefControle=tblControle.RefControle) as SourceTable"
                    + "         pivot"
                    + "         (max(Poids)"
                    + "         for Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " in (" + s + ")"
                    + "         ) as PivotTable"
                    + "     )as descriptionControle"
                    + "     on tblControle.RefControle=descriptionControle.RefControle";
                }
                sqlStr += " where tblCommandeFournisseur.DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterContrat))
                {
                    switch (eSF.FilterContrat)
                    {
                        case "SousContrat":
                            sqlStr += " and Client.SousContrat=1";
                            break;
                        case "HorsContrat":
                            sqlStr += " and Client.SousContrat=0";
                            break;
                    }
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString())
            {
                //Base SQL statement
                sqlStr = "select tblNonConformite.RefNonConformite [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefNonConformite.ToString()].Name + "]"
                    + "     , NumeroCommande [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                    + "     , case when Etape8Valide.RefNonConformite is null then tblNonConformiteEtape.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " else '" + CurrentContext.CulturedRessources.GetTextRessource(165) + "' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteEtapeLibelle.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , dbo.ListeDR(fournisseur.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                        + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]";
                }
                sqlStr += "     , case when fournisseur.CodeEE is not null then fournisseur.CodeEE + ' - ' else '' end + fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                    + "     , tblProduit.Libelle AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                    + "     , nonConformiteCreation.DCreation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteDateCreation.ToString()].Name + "]"
                    + "     , tblNonConformite.DescrClient as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteDescrClient.ToString()].Name + "]"
                    + "     , tbrNonConformiteDemandeClientType.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteDemandeClientTypeLibelle.ToString()].Name + "]"
                    + "     , tblNonConformite.DescrValorplast as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteDescrValorplast.ToString()].Name + "]"
                    + "     , tbrNonConformiteNature.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteNatureLibelle.ToString()].Name + "]"
//                    + "     , dbo.ListeNonConformiteFamille(tblNonConformite.RefNonConformite, '" + CurrentContext.CurrentCulture.Name + "') as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteListeFamille.ToString()].Name + "]"
                    + "     , IFFournisseurRetourLot as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurRetourLot.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , cast(case when IFFournisseurDescr is not null or IFFournisseurFactureMontant is not null or IFFournisseurDeductionTonnage is not null then 1 else 0 end as bit)"
                        + "     as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseur.ToString()].Name + "]"
                        + "     , IFFournisseurFactureMontant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurFactureMontant.ToString()].Name + "]"
                        + "     , IFFournisseurDeductionTonnage as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurDeductionTonnage.ToString()].Name + "]";
                }
                sqlStr += "     , cast(case when IFClientDescr is not null or IFClientFactureMontant is not null then 1 else 0 end as bit)"
                    + "     as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFClient.ToString()].Name + "]"
                    + "     , IFClientFactureMontant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFClientFactureMontant.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , DTransmissionFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteDTransmissionFournisseur.ToString()].Name + "]"
                        + "     , ActionDR as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteActionDR.ToString()].Name + "]"
                        + "     , tbrNonConformiteReponseFournisseurType.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteReponseFournisseurTypeLibelle.ToString()].Name + "]"
                        + "     , CmtReponseFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteCmtReponseFournisseur.ToString()].Name + "]";
                }
                sqlStr += "     , tbrNonConformiteReponseClientType.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteReponseClientTypeLibelle.ToString()].Name + "]"
                    + "     , CmtReponseClient as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteCmtReponseClient.ToString()].Name + "]"
                    + "     , PlanAction as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformitePlanActionNonConcerne.ToString()].Name + "]"
                    + "     , CmtOrigineAction as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformitePlanAction.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , tbrNonConformiteAccordFournisseurType.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteAccordFournisseurTypeLibelle.ToString()].Name + "]"
                        + "     , IFFournisseurBonCommandeNro as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurBonCommandeNro.ToString()].Name + "]"
                        + "     , IFFournisseurAttenteBonCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurAttenteBonCommandeNro.ToString()].Name + "]"
                        + "     , IFFournisseurFacture as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurFacture.ToString()].Name + "]"
                        + "     , IFFournisseurTransmissionFacturation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurTransmissionFacturation.ToString()].Name + "]"
                        + "     , IFFournisseurFactureNro as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurFactureNro.ToString()].Name + "]"
                        + "     , IFFournisseurCmtFacturation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurCmtFacturation.ToString()].Name + "]"
                        + "     , IFClientCommandeAFaire as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFClientCommandeAFaire.ToString()].Name + "]"
                        + "     , IFClientFactureEnAttente as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFClientFactureEnAttente.ToString()].Name + "]"
                        ;
                }
                sqlStr += "     , IFClientFactureNro as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFClientFactureNro.ToString()].Name + "]"
                    + "     , IFClientDFacture as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFClientDFacture.ToString()].Name + "]"
                    + "     , IFClientCmtFacturation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFClientCmtFacturation.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , PriseEnCharge as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformitePriseEnCharge.ToString()].Name + "]"
                        + "     , MontantPriseEnCharge as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteMontantPriseEnCharge.ToString()].Name + "]"
                        + "     , CmtPriseEnCharge as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteCmtPriseEnCharge.ToString()].Name + "]";
                }
                sqlStr += "     , cast(case when etape8Valide.RefNonConformite is not null then 1 else 0 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteCloturee.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + "     inner join tblNonConformite on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                    + "     left join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                    + "     left join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     left join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + "     left join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     left join tblFicheControle on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                    + "     left join tbrNonConformiteDemandeClientType on tblNonConformite.RefNonConformiteDemandeClientType=tbrNonConformiteDemandeClientType.RefNonConformiteDemandeClientType"
                    + "     left join tbrNonConformiteReponseClientType on tblNonConformite.RefNonConformiteReponseClientType=tbrNonConformiteReponseClientType.RefNonConformiteReponseClientType"
                    + "     left join tbrNonConformiteReponseFournisseurType on tblNonConformite.RefNonConformiteReponseFournisseurType=tbrNonConformiteReponseFournisseurType.RefNonConformiteReponseFournisseurType"
                    + "     left join tbrNonConformiteNature on tblNonConformite.RefNonConformiteNature=tbrNonConformiteNature.RefNonConformiteNature"
                    + "     left join tbrNonConformiteAccordFournisseurType on tblNonConformite.RefNonConformiteAccordFournisseurType=tbrNonConformiteAccordFournisseurType.RefNonConformiteAccordFournisseurType"
                    + "     left join (select RefNonConformite, min(DCreation) as DCreation from tblNonConformiteEtape where RefNonConformiteEtapeType=1 group by RefNonConformite) as nonConformiteCreation on tblNonConformite.RefNonConformite=nonConformiteCreation.RefNonConformite"
                    + "     left join (select RefNonConformite, min(Ordre) as ordre from tblNonConformiteEtape where DValide is null group by RefNonConformite) as etape on etape.RefNonConformite = tblNonConformite.RefNonConformite"
                    + "     left join tblNonConformiteEtape on tblNonConformiteEtape.RefNonConformite=Etape.RefNonConformite and tblNonConformiteEtape.Ordre=etape.Ordre"
                    + "     left join (select RefNonConformite from tblNonConformiteEtape where DValide is null and RefNonConformiteEtapeType= 5) as etape5 on etape5.RefNonConformite = tblNonConformite.RefNonConformite"
                    + "     left join (select RefNonConformite from tblNonConformiteEtape where DValide is not null and RefNonConformiteEtapeType= 8) as etape8Valide on etape8Valide.RefNonConformite = tblNonConformite.RefNonConformite";
                sqlStr += "     where nonConformiteCreation.DCreation>=@begin and nonConformiteCreation.DCreation<@end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end.AddDays(1);
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterNonConformiteEtapeTypes != "")
                {
                    sqlStr += " and tblNonConformiteEtape.RefNonConformiteEtapeType in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refNonConformiteEtapeType", eSF.FilterNonConformiteEtapeTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterNonConformiteNatures != "")
                {
                    sqlStr += " and tblNonConformite.RefNonConformiteNature in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refNonConformiteNature", eSF.FilterNonConformiteNatures, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                if (!string.IsNullOrEmpty(eSF.FilterContrat))
                {
                    switch (eSF.FilterContrat)
                    {
                        case "SousContrat":
                            sqlStr += " and Client.SousContrat=1";
                            break;
                        case "HorsContrat":
                            sqlStr += " and Client.SousContrat=0";
                            break;
                    }
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString())
            {
                //Eléments de description des CVQ
                //First command for fields
                sqlStr = "select tblCVQDescriptionCVQ.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + ", max(tblCVQDescriptionCVQ.Ordre)"
                    + " from tblCVQ"
                    + "     inner join tblFicheControle on tblFicheControle.RefFicheControle=tblCVQ.RefFicheControle"
                    + "     inner join tblCVQDescriptionCVQ on tblCVQDescriptionCVQ.RefCVQ=tblCVQ.RefCVQ"
                    + "     inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                    + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + " where tblCommandeFournisseur.DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                //Regroupement
                sqlStr += " group by tblCVQDescriptionCVQ.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR");
                //Tri
                sqlStr += " order by max(tblCVQDescriptionCVQ.Ordre)";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        s += ", [" + dr.GetSqlString(0).ToString() + "], [" + dr.GetSqlString(0).ToString() + " (" + CurrentContext.CulturedRessources.GetTextRessource(29) + ")]";
                        s1 += ", max(case when tblCVQDescriptionCVQ.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + "='" + dr.GetSqlString(0).ToString().Replace("'", "''") + "'"
                            + " then Nb else 0 end)"
                            + " as [" + dr.GetSqlString(0).ToString() + "]";
                        s1 += ", max(case when tblCVQDescriptionCVQ.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + "='" + dr.GetSqlString(0).ToString().Replace("'", "''") + "'"
                            + " then case when Nb<=LimiteBasse then 1 else case when nb>LimiteHaute then 3 else 2 end end else 0 end)"
                            + " as [" + dr.GetSqlString(0).ToString() + " (" + CurrentContext.CulturedRessources.GetTextRessource(29) + ")]";
                    }
                    dr.Close();
                }
                cmd = new SqlCommand();
                //Base SQL statement
                sqlStr = "select tblFicheControle.RefFicheControle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefFicheControle.ToString()].Name + "]"
                    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , dbo.ListeDR(fournisseur.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                        + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]";
                }
                sqlStr += "     , NumeroLotUsine as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleNumeroLotUsine.ToString()].Name + "]"
                    + "     , case when fournisseur.CodeEE is not null then fournisseur.CodeEE + ' - ' else '' end + fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurLibelle.ToString()].Name + "]"
                    + "     , tblProduit.Libelle AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                    + "     , DDechargement AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                    + "     , DCVQ AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CVQDCVQ.ToString()].Name + "]"
                    + "     , isnull(tblContact.Prenom,'') + ' ' + tblContact.Nom AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FicheControleControleur.ToString()].Name + "]"
                    + "     , tblCVQ.Cmt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CVQCmt.ToString()].Name + "]"
                    + s
                    + "     , cast(case when tblNonConformite.RefCommandeFournisseur is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNonConformite.ToString()].Name + "]"
                    + " from tblCVQ"
                    + "     inner join tblFicheControle on tblFicheControle.RefFicheControle=tblCVQ.RefFicheControle"
                    + "     inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblFicheControle.RefCommandeFournisseur"
                    + "     left join tblNonConformite on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                    + "     inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     inner join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                    + "     left join tbmContactAdresse on tblCVQ.RefControleur=tbmContactAdresse.RefContactAdresse"
                    + "     left join tblContact on tbmContactAdresse.RefContact = tblContact.RefContact"
                    + "     inner join ("
                    + "         select RefCVQ " + s1 + " from tblCVQDescriptionCVQ group by RefCVQ) as cvq on cvq.RefCVQ=tblCVQ.RefCVQ"
                    ;
                sqlStr += " where tblCommandeFournisseur.DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterContrat))
                {
                    switch (eSF.FilterContrat)
                    {
                        case "SousContrat":
                            sqlStr += " and Client.SousContrat=1";
                            break;
                        case "HorsContrat":
                            sqlStr += " and Client.SousContrat=0";
                            break;
                    }
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.QualiteMenuEtatControleReception.ToString())
            {
                //Base SQL statement
                sqlStr = "select tblCommandeFournisseur.RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                    + "     , tblCommandeFournisseur.NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                {
                    sqlStr += "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]";
                }
                sqlStr += "     , dbo.CommandeMixte(NumeroAffretement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurMixte.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , dbo.ListeDR(fournisseur.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                    + "     , client.SousContrat as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSousContrat.ToString()].Name + "]";
                }
                sqlStr += "     , case when fournisseur.CodeEE is not null then fournisseur.CodeEE + ' - ' else '' end + fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                {
                    sqlStr += "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineVille.ToString()].Name + "]";
                }
                sqlStr += "     , tblProduit.Libelle + case when tblContrat.RefContrat is null then '' else ' ('+tblContrat.IdContrat+')' end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                {
                    sqlStr += "     , transporteur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurLibelle.ToString()].Name + "]"
                        + "     , adresseDestination.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]";
                }
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]";
                }
                sqlStr += "     , DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                {
                    sqlStr += "     , NbBalleDechargement AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNbBalleDechargement.ToString()].Name + "]";
                }
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , PoidsChargement AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsChargement.ToString()].Name + "]";
                }
                sqlStr += "     , PoidsDechargement AS [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsDechargement.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient != null)
                {
                    sqlStr += "     , PrixTonneHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPrixTonneHT.ToString()].Name + "]";
                }
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , PoidsReparti as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsReparti.ToString()].Name + "]";
                }
                sqlStr += "     , cast(case when tblFicheControle.RefFicheControle is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurFicheControle.ToString()].Name + "]"
                    + "     , cast(case when tblCVQ.RefFicheControle is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurCVQ.ToString()].Name + "]"
                    + "     , cast(case when tblControle.RefFicheControle is null then 0 else 1 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurCB.ToString()].Name + "]"
                    + "     , tbrNonConformiteNature.Libelle" + (CurrentContext.CurrentCulture.Name == "en-GB" ? "ENGB" : "FRFR") + " as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteNatureLibelle.ToString()].Name + "]";
                if (CurrentContext.ConnectedUtilisateur.RefClient == null)
                {
                    sqlStr += "     , IFFournisseurFactureMontant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurFactureMontant.ToString()].Name + "]"
                        + "     , IFClientFactureMontant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFClientFactureMontant.ToString()].Name + "]";
                }
                sqlStr += "     , IFFournisseurRetourLot as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NonConformiteIFFournisseurRetourLot.ToString()].Name + "]"
                    + " from tblCommandeFournisseur"
                    + "     left join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                    + "     left join tblAdresse as adresseDestination on tblCommandeFournisseur.RefAdresseClient=adresseDestination.RefAdresse"
                    + "     left join tbrPays on adresseDestination.RefPays=tbrPays.RefPays"
                    + "     left join tblEntite as client on client.RefEntite=adresseDestination.RefEntite"
                    + "     left join tblEntite as transporteur on transporteur.RefEntite=tblCommandeFournisseur.RefTransporteur"
                    + "     left join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + "     left join (select distinct RefFicheControle, RefCommandeFournisseur from tblFicheControle) as tblFicheControle on tblFicheControle.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + "     left join (select distinct RefFicheControle from tblControle) as tblControle on tblFicheControle.RefFicheControle=tblControle.RefFicheControle"
                    + "     left join (select distinct RefFicheControle from tblCVQ) as tblCVQ on tblFicheControle.RefFicheControle=tblCVQ.RefFicheControle"
                    + "     left join tblNonConformite on tblCommandeFournisseur.RefCommandeFournisseur=tblNonConformite.RefCommandeFournisseur"
                    + "     left join tbrNonConformiteNature on tblNonConformite.RefNonConformiteNature=tbrNonConformiteNature.RefNonConformiteNature"
                    + "     left join VueCommandeFournisseurContrat on tblCommandeFournisseur.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                    + "     left join tblContrat on tblContrat.RefContrat=VueCommandeFournisseurContrat.RefContrat"
                    + " where DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                if (!string.IsNullOrEmpty(eSF.FilterContrat))
                {
                    switch (eSF.FilterContrat)
                    {
                        case "SousContrat":
                            sqlStr += " and Client.SousContrat=1";
                            break;
                        case "HorsContrat":
                            sqlStr += " and Client.SousContrat=0";
                            break;
                    }
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblCommandeFournisseur.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatReceptionEmballagePlastique.ToString())
            {
                //Base SQL statement
                sqlStr = "select year(VueRepartitionUnitaireDetail.D) as [Année], 'T'+cast(datepart(quarter,VueRepartitionUnitaireDetail.D) as nvarchar(1)) as [Trimestre], collectivite.CodeEE as [Code CITEO], tbrProcess.Libelle as [Process]"
                    + " 	, cast(cast(sum(Poids) as decimal(10,3))/1000 as decimal(10,3)) as [Poids total déclaré (tonne)]"
                    + " 	, cast(cast(sum(case when VueRepartitionUnitaireDetail.RefProduit=114 then Poids else 0 end) as decimal(10,3))/1000 as decimal(10,3)) as [Poids EP (tonne)]"
                    + " 	, cast(cast(sum(case when VueRepartitionUnitaireDetail.RefProduit=64 then Poids else 0 end) as decimal(10,3))/1000 as decimal(10,3)) as [Poids BF (tonne)]"
                    + " 	, isnull(tbrRegionEE.Libelle, 'NR') as [Région CITEO]"
                    + "     , cast(year(VueRepartitionUnitaireDetail.D) as nvarchar(4)) + 'T' + cast(datepart(quarter,VueRepartitionUnitaireDetail.D) as nvarchar(1)) + '-' + collectivite.CodeEE + '-' + fournisseur.CodeEE + '-' + cast(month(VueRepartitionUnitaireDetail.D) as nvarchar) + '_' + dbo.ToCiteoStandard(tblProduit.NomCommun) as [Numéro du transport]"
                    + "     , fournisseur.CodeEE as [Code CITEO centre de tri]"
                    + "     , fournisseur.Libelle as [Centre de tri], dbo.ToCiteoStandard(tblProduit.NomCommun) as [Produit]"
                    + "     , VueRepartitionUnitaireDetail.PUHT+isnull(tbrPrixReprise.PUHTSurtri,0)+isnull(tbrPrixReprise.PUHTTransport,0) as [Prix €HT/t], '' as [Sous-catégorie 1]"
                    + "     , isnull(tbrPrixReprise.PUHTTransport,0) as [Coût de transport -€HT/t]"
                    + "     , month(VueRepartitionUnitaireDetail.D) as [Mois], 1 as [Type d'enregistrement], isnull(tblProduit.CodeEE,'ND') as [Matériau], isnull(tblProduit.CodeEE,'ND') as [Standard], 'REP-3310' as [Repreneur]"
                    + "     , convert(nvarchar(10),DATEADD(day,-day(VueRepartitionUnitaireDetail.D)+1,VueRepartitionUnitaireDetail.D),103) as [Date d'enlèvement]"
                    + "     , convert(nvarchar(10),DATEADD(day,-day(VueRepartitionUnitaireDetail.D)+1,VueRepartitionUnitaireDetail.D),103) as [Date de livraison]"
                    + "     , 'ROUTIER' as [Mode de transport]"
                    + "     , 'RDC' as [Destination]"
                    + " from VueRepartitionUnitaireDetail"
                    + "     left join tblCommandeFournisseur on VueRepartitionUnitaireDetail.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + "     left join VueCommandeFournisseurContrat on VueRepartitionUnitaireDetail.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                    + "     left join tbrPrixReprise on tbrPrixReprise.RefProcess=VueRepartitionUnitaireDetail.RefProcess and tbrPrixReprise.RefProduit=VueRepartitionUnitaireDetail.RefProduit "
                    + "     	and tbrPrixReprise.RefProduit=VueRepartitionUnitaireDetail.RefProduit"
                    + "     	and month(VueRepartitionUnitaireDetail.D)=month(tbrPrixReprise.D) and year(VueRepartitionUnitaireDetail.D)=year(tbrPrixReprise.D)"
                    + "     	and isnull(tbrPrixReprise.RefContrat,0)=isnull(VueCommandeFournisseurContrat.RefContrat,0)"
                    + " 	left join tbrProcess on VueRepartitionUnitaireDetail.RefProcess=tbrProcess.RefProcess"
                    + " 	inner join tblEntite as collectivite on VueRepartitionUnitaireDetail.RefFournisseur=collectivite.RefEntite"
                    + " 	inner join tblEntite as fournisseur on tblCommandeFournisseur.RefEntite=fournisseur.RefEntite"
                    + " 	inner join (select RefEntite, max(CodePostal) as CodePostal from tblAdresse where RefAdresseType=1 group by RefEntite) as adr on collectivite.RefEntite=adr.RefEntite"
                    + " 	inner join tblProduit on VueRepartitionUnitaireDetail.RefProduit=tblProduit.refProduit"
                    + " 	left join tbmRegionEEDpt on substring(adr.CodePostal,1,2)=tbmRegionEEDpt.RefDpt"
                    + " 	left join tbrRegionEE on tbmRegionEEDpt.RefRegionEE=tbrRegionEE.RefRegionEE"
                    + " where VueRepartitionUnitaireDetail.D between @begin and @end and VueRepartitionUnitaireDetail.Collecte=1 and collectivite.RefEcoOrganisme in(1,2)";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and fournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //Group by
                sqlStr += " group by year(VueRepartitionUnitaireDetail.D), 'T'+cast(datepart(quarter,VueRepartitionUnitaireDetail.D) as nvarchar(1)), month(VueRepartitionUnitaireDetail.D), isnull(tbrRegionEE.Libelle, 'NR'), collectivite.CodeEE, tbrProcess.Libelle, fournisseur.CodeEE, fournisseur.Libelle, dbo.ToCiteoStandard(tblProduit.NomCommun), isnull(tblProduit.CodeEE,'ND'), VueRepartitionUnitaireDetail.PUHT, tbrPrixReprise.PUHTSurtri, tbrPrixReprise.PUHTTransport"
                    + " , cast(year(VueRepartitionUnitaireDetail.D) as nvarchar(4)) + 'T' + cast(datepart(quarter,VueRepartitionUnitaireDetail.D) as nvarchar(1)) + '-' + collectivite.CodeEE + '-' + fournisseur.CodeEE + '-' + cast(month(VueRepartitionUnitaireDetail.D) as nvarchar) + '_' + dbo.ToCiteoStandard(tblProduit.NomCommun)"
                    + " , convert(nvarchar(10),DATEADD(day,-day(VueRepartitionUnitaireDetail.D)+1,VueRepartitionUnitaireDetail.D),103)";
                //Count
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur.ToString())
            {
                //Base SQL statement
                sqlStr = "select year(tblCommandeFournisseur.DDechargement) as [Année], 'T'+cast(datepart(quarter,tblCommandeFournisseur.DDechargement) as nvarchar(1)) as [Trimestre], isnull(tblProduit.CodeEE,'ND') as [Matériau]"
                            + " 	, tblProduit.NomCommun as [Produit], tblCommandeFournisseur.NumeroCommande as [N° commande], tblCommandeFournisseur.NumeroAffretement as [Affretement]"
                            + " 	, fournisseur.CodeEE as [Code CITEO centre de tri], client.Libelle as [Recycleur], convert(nvarchar(10),DDechargement,103) as [Date de réception]"
                            + " 	, cast(cast(rep.Poids as decimal(10,3))/1000 as decimal(10,3)) as [Poids (tonne)], Km, tbrRegionEE.Libelle as [Région CITEO centre de tri], fournisseur.Libelle as [Centre de tri], tblAdresse.Ville as [Ville], tbrPays.Libelle as [Pays]"
                            + "     , 1 as [Type d'enregistrement], isnull(tblProduit.CodeEE,'ND') as [Standard], month(DDechargement) as [Mois], 'REP-3310' as [Repreneur], convert(nvarchar(10),DChargement,103) as [Date d'enlèvement], tbrModeTransportEE.Libelle as [Mode de transport], 'Balles' as [Conditionnement], cast(cast(PoidsChargement as decimal(10,3))/1000 as decimal(10,3)) as [Poids chargement]"
                            + " from tblCommandeFournisseur"
                            + " 	inner join tblRepartition on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                            + " 	inner join (select tblRepartitionCollectivite.RefRepartition, Sum(Poids) as Poids"
                            + "         from tblRepartitionCollectivite inner join tblEntite as collectivite on tblRepartitionCollectivite.RefCollectivite=collectivite.RefEntite"
                            + "         where collectivite.RefEcoOrganisme in(1,2)"
                            + "         group by tblRepartitionCollectivite.RefRepartition) as rep on rep.RefRepartition=tblRepartition.RefRepartition"
                            + " 	inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.refProduit"
                            + " 	inner join tblAdresse on tblAdresse.RefAdresse=tblCommandeFournisseur.RefAdresseClient"
                            + " 	left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                            + " 	inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                            + " 	inner join tblEntite as fournisseur on tblCommandeFournisseur.RefEntite=fournisseur.RefEntite"
                            + " 	inner join (select RefEntite, max(CodePostal) as CodePostal from tblAdresse where RefAdresseType=1 group by RefEntite) as adr on fournisseur.RefEntite=adr.RefEntite"
                            + " 	left join tbrCamionType on tblCommandeFournisseur.RefCamionType=tbrCamionType.RefCamionType"
                            + " 	left join tbrModeTransportEE on tbrCamionType.RefModeTransportEE=tbrModeTransportEE.RefModeTransportEE"
                            + " 	left join tbmRegionEEDpt on substring(adr.CodePostal,1,2)=tbmRegionEEDpt.RefDpt"
                            + " 	left join tbrRegionEE on tbmRegionEEDpt.RefRegionEE=tbrRegionEE.RefRegionEE"
                            + " 	left join (select RefCommandeFournisseur from tblRepartition) as unitaire on tblCommandeFournisseur.RefCommandeFournisseur=unitaire.RefCommandeFournisseur      "
                            + " 	left join tblRepartition as mensuelle on tblCommandeFournisseur.RefEntite=mensuelle.RefFournisseur and year(tblCommandeFournisseur.DDechargement)=year(mensuelle.D) and month(tblCommandeFournisseur.DDechargement)=month(mensuelle.D) and tblCommandeFournisseur.RefProduit=mensuelle.RefProduit"
                            + " where tblCommandeFournisseur.DDechargement between @begin and @end and tblProduit.Collecte=1";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and fournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterClients != "")
                {
                    sqlStr += " and client.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refClient", eSF.FilterClients, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //Count
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuExtractionOscar.ToString())
            {
                //Base SQL statement
                sqlStr = "select year(VueRepartitionUnitaireDetail.D) as [Année]"
                    + "     , month(VueRepartitionUnitaireDetail.D) as [Mois d'affectation]"
                    + "     , cast(year(VueRepartitionUnitaireDetail.D) as varchar(4))"
                    + "         + '-' + cast(month(VueRepartitionUnitaireDetail.D) as varchar(2))"
                    + "         + '-' + cast(collectivite.RefEntite as varchar(20))"
                    + "         + '-' + cast(fournisseur.RefEntite as varchar(20))"
                    + "         + '-' + cast(tblAdresse.RefAdresse as varchar(20))"
                    + "         + '-' + cast(tblProduit.RefProduit as varchar(20))"
                    + "         + '-' + isnull(cast(tbrModeTransportEE.RefModeTransportEE as varchar(20)),'NR') as [Numéro Référence]"
                    + "     , isnull(tblProduit.CodeEE,'ND') as [Standard]"
                    + "     , fournisseur.CodeEE as [Point d'enlèvement]"
                    + "     , collectivite.CodeEE as [Collectivité]"
                    + " 	, cast(cast(sum(Poids) as decimal(10,3))/1000 as decimal(10,3)) as [Tonnage réparti par CL (en tonnes)]"
                    + "     , 'recycleur_final' as [Déclarant Confidentiel ou Recycleur Final ou Centre de sutri]"
                    + "     , cast(tblAdresse.RefAdresse as varchar(20)) as [Nom Destination]"
                    + "     , tbrModeTransportEE.Libelle as [Mode de transport]"
                    + "     , '' as [Intermédiaire 1]"
                    + "     , '' as [Intermédiaire 2]"
                    + "     , '' as [Intermédiaire 3]"
                    + "     , '' as [Intermédiaire 4]"
                    + "     , '' as [Intermédiaire 5]"
                    + "     , '' as [Intermédiaire 6]"
                    + "     , '' as [Intermédiaire 7]"
                    + "     , '' as [Intermédiaire 8]"
                    + "     , '' as [Intermédiaire 9]"
                    + "     , '' as [Intermédiaire 10]"
                    + " from VueRepartitionUnitaireDetail"
                    + "     left join tblCommandeFournisseur on VueRepartitionUnitaireDetail.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + "     left join VueCommandeFournisseurContrat on VueRepartitionUnitaireDetail.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                    + " 	inner join tblEntite as collectivite on VueRepartitionUnitaireDetail.RefFournisseur=collectivite.RefEntite"
                    + " 	inner join tblEntite as fournisseur on tblCommandeFournisseur.RefEntite=fournisseur.RefEntite"
                    + " 	inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + " 	inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                    + " 	inner join tblProduit on VueRepartitionUnitaireDetail.RefProduit=tblProduit.refProduit"
                    + " 	left join tbrCamionType on tblCommandeFournisseur.RefCamionType=tbrCamionType.RefCamionType"
                    + " 	left join tbrModeTransportEE on tbrCamionType.RefModeTransportEE=tbrModeTransportEE.RefModeTransportEE"
                    + " where VueRepartitionUnitaireDetail.D between @begin and @end and VueRepartitionUnitaireDetail.Collecte=1 and collectivite.RefEcoOrganisme in(1,2)";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterCentreDeTris != "")
                {
                    sqlStr += " and fournisseur.RefEntite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCentreDeTri", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //Group by
                sqlStr += " group by year(VueRepartitionUnitaireDetail.D), month(VueRepartitionUnitaireDetail.D), fournisseur.CodeEE, collectivite.CodeEE, isnull(tblProduit.CodeEE,'ND')"
                    + "     , client.CodeEE, tbrModeTransportEE.Libelle"
                    + "     , collectivite.RefEntite, fournisseur.RefEntite, tblAdresse.RefAdresse, tblProduit.RefProduit, tbrModeTransportEE.RefModeTransportEE";
                //Count
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatDesFluxDev.ToString())
            {
                //Base SQL statement
                sqlStr = "select '' as [Numéro de transaction], '' as [Colonne vide a conserver], tblCommandeFournisseur.NumeroCommande as [Numéro de ticket de pesée]"
                    + " 	, '' as [Statut], convert(nvarchar(10), tblCommandeFournisseur.DCreation, 103) as [Date de création]"
                    + " 	, '' as [Date de réception de la demande]"
                    + " 	, convert(nvarchar(10), tblCommandeFournisseur.DChargement, 103) as [Date de collecte]"
                    + " 	, convert(nvarchar(10), tblCommandeFournisseur.DDechargement, 103) as [Date de livraison]"
                    + " 	, '' as [Date de première validation], '' as [Date de validation finale(recyclé)]"
                    + " 	, fournisseur.Libelle as [CDST], fournisseur.CodeEE as [Code CDST]"
                    + " 	, transporteur.Libelle as [Transporteur], transporteur.RefEntite as [Code Transporteur]"
                    + " 	, client.Libelle as [Recycleur], client.CodeEE as [Code Recycleur]"
                    + " 	, tblProduit.Libelle as [Flux], tblProduit.RefProduit as [Code Flux]"
                    + " 	, tblCommandeFournisseur.NbBalleChargement as [Nombre de balles]"
                    + " 	, tblCommandeFournisseur.PoidsDechargement as [Poids livré]"
                    + " 	, tblCommandeFournisseur.RefExt as [Référence externe]"
                    + " from tblCommandeFournisseur"
                    + "     inner join tblProduit on tblCommandeFournisseur.RefProduit = tblProduit.refProduit"
                    + "     inner join tblAdresse on tblAdresse.RefAdresse = tblCommandeFournisseur.RefAdresseClient"
                    + "     inner join tblEntite as client on tblAdresse.RefEntite = client.RefEntite"
                    + "     inner join tblEntite as transporteur on tblCommandeFournisseur.RefTransporteur = transporteur.RefEntite"
                    + "     inner join tblEntite as fournisseur on tblCommandeFournisseur.RefEntite = fournisseur.RefEntite"
                    + " where tblCommandeFournisseur.DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuEtatDesFluxDevLeko.ToString())
            {
                //Base SQL statement
                sqlStr = "select null as [ID de transport]"
                    + "     , fournisseur.Libelle as [SP]"
                    + "     , fournisseur.CodeEE as [SP code]"
                    + "     , fournisseur.CodeEE as [Référence centre de tri]"
                    + "     , tblProduit.Libelle as [Flux]"
                    + "     , 'Balle' as [Conditionnement]"
                    + "     , CONVERT(char(10),tblCommandeFournisseur.D,103) as [Date programmée]"
                    + " 	, cast(cast(PoidsChargement as decimal(10,3))/1000 as decimal(10,3)) as [Tonnes disponibles]"
                    + "     , CONVERT(char(10),DChargement,103) as [Date de chargement]"
                    + "     , '10:00' as [Date de chargement heure]"
                    + "     , tbrModeTransportEE.Libelle as [Mode de transport]"
                    + "     , 'NA' as [Plaque d'immatriculation]"
                    + " 	, 0 as [Poids entrant]"
                    + "     , cast(cast(PoidsChargement as decimal(10,3))/1000 as decimal(10,3)) as [Poids sortant]"
                    + "     , cast(cast(PoidsChargement as decimal(10,3))/1000 as decimal(10,3)) as [Tonnes chargées (optionnel)]"
                    + "     , 'LEKO' as [Repreneur]"
                    + "     , NumeroCommande as [Référence repreneur]"
                    + "     , null as [Repreneur intermédiaire]"
                    + "     , null as [Repreneur intermédiaire SIRET]"
                    + "     , 0 as [Repreneur intermédiaire confidential]"
                    + "     , null as [Stockage]"
                    + "     , null as [Stockage SIRET]"
                    + "     , client.Libelle as [Destination]"
                    + "     , client.Libelle as [Référence destination]"
                    + "     , client.IdNational as [Destination SIRET]"
                    + "     , CONVERT(char(10),DDechargement,103) as [Date de livraison]"
                    + " 	, cast(cast(PoidsChargement as decimal(10,3))/1000 as decimal(10,3)) as [Tonnes livrées]"
                    + "     , null as [Commentaire]"
                    + "     , null as [Commentaire de commande]"
                    + "     , 'Livré' as [Statut]"
                    + " from tblCommandeFournisseur"
                    + "     inner join tblProduit on tblCommandeFournisseur.RefProduit = tblProduit.refProduit"
                    + "     inner join tblAdresse on tblAdresse.RefAdresse = tblCommandeFournisseur.RefAdresseClient"
                    + "     inner join tblEntite as client on tblAdresse.RefEntite = client.RefEntite"
                    + "     inner join tblEntite as fournisseur on tblCommandeFournisseur.RefEntite = fournisseur.RefEntite"
                    + " 	left join tbrCamionType on tblCommandeFournisseur.RefCamionType=tbrCamionType.RefCamionType"
                    + " 	left join tbrModeTransportEE on tbrCamionType.RefModeTransportEE=tbrModeTransportEE.RefModeTransportEE"
                    + " where tblCommandeFournisseur.DDechargement between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and tblProduit.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.LogistiqueMenuExtractionLeko.ToString())
            {
                //Base SQL statement
                sqlStr = "select null as [ID de transport]"
                    + "     , fournisseur.Libelle as [SP]"
                    + "     , fournisseur.CodeEE as [SP code]"
                    + "     , fournisseur.CodeEE as [Référence centre de tri]"
                    + "     , tblProduit.Libelle as [Flux]"
                    + "     , 'Balle' as [Conditionnement]"
                    + "     , CONVERT(char(10),tblCommandeFournisseur.D,103) as [Date programmée]"
                    + " 	, cast(cast(sum(Poids) as decimal(10,3))/1000 as decimal(10,3)) as [Tonnes disponibles]"
                    + "     , CONVERT(char(10),DChargement,103) as [Date de chargement]"
                    + "     , '10:00' as [Date de chargement heure]"
                    + "     , tbrModeTransportEE.Libelle as [Mode de transport]"
                    + "     , 'NA' as [Plaque d'immatriculation]"
                    + " 	, 0 as [Poids entrant]"
                    + "     , cast(cast(sum(Poids) as decimal(10,3))/1000 as decimal(10,3)) as [Poids sortant]"
                    + "     , cast(cast(sum(Poids) as decimal(10,3))/1000 as decimal(10,3)) as [Tonnes chargées (optionnel)]"
                    + "     , 'VALORPLAST' as [Repreneur]"
                    + "     , NumeroCommande as [Référence repreneur]"
                    + "     , null as [Repreneur intermédiaire]"
                    + "     , null as [Repreneur intermédiaire SIRET]"
                    + "     , 0 as [Repreneur intermédiaire confidential]"
                    + "     , null as [Stockage]"
                    + "     , null as [Stockage SIRET]"
                    + "     , client.Libelle as [Destination]"
                    + "     , client.Libelle as [Référence destination]"
                    + "     , client.IdNational as [Destination SIRET]"
                    + "     , CONVERT(char(10),DDechargement,103) as [Date de livraison]"
                    + " 	, cast(cast(sum(Poids) as decimal(10,3))/1000 as decimal(10,3)) as [Tonnes livrées]"
                    + "     , null as [Commentaire]"
                    + "     , null as [Commentaire de commande]"
                    + "     , 'Livré' as [Statut]"
                    + " from tblCommandeFournisseur"
                    + " 	inner join tblRepartition on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                    + " 	inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=tblRepartition.RefRepartition"
                    + " 	inner join tblEntite as collectivite on tblRepartitionCollectivite.RefCollectivite=collectivite.RefEntite"
                    + " 	left join tblEntite as fournisseur on tblCommandeFournisseur.RefEntite=fournisseur.RefEntite"
                    + " 	left join tblAdresse as adrC on tblCommandeFournisseur.RefAdresseClient=adrC.RefAdresse"
                    + " 	left join tblEntite as client on adrC.RefEntite=client.RefEntite"
                    + " 	inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                    + " 	left join tbrCamionType on tblCommandeFournisseur.RefCamionType=tbrCamionType.RefCamionType"
                    + " 	left join tbrModeTransportEE on tbrCamionType.RefModeTransportEE=tbrModeTransportEE.RefModeTransportEE"
                    + " where DDechargement between @begin and @end and collectivite.RefEcoOrganisme in(3)";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                }
                //Group by
                sqlStr += " group by fournisseur.Libelle"
                    + "     , fournisseur.CodeEE"
                    + "     , CONVERT(char(10),tblCommandeFournisseur.D,103)"
                    + "     , CONVERT(char(10),DChargement,103)"
                    + "     , tbrModeTransportEE.Libelle"
                    + "     , cast(cast(PoidsDechargement as decimal(10,3))/1000 as decimal(10,3))"
                    + "     , NumeroCommande"
                    + "     , tblProduit.Libelle"
                    + "     , client.Libelle"
                    + "     , client.IdNational"
                    + "     , CONVERT(char(10),DDechargement,103)";
                //Count
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.AnnuaireMenuSuiviEnvois.ToString())
            {
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end.AddDays(1);
                    }
                }
                if (eSF.FilterEmailType == "IncitationQualite")
                {
                    sqlStr = "select Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.GenerePar.ToString()].Name + "]"
                        + "     , tblEmailIncitationQualite.Annee as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnneeNb.ToString()].Name + "]"
                        + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + "     , tblEmailIncitationQualite.EmailTo as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseEmail.ToString()].Name + "]"
                        + "     , tblEmailIncitationQualite.DCreation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EmailDEnvoi.ToString()].Name + "]"
                        + " from tblEmailIncitationQualite"
                        + "     left join tblEntite on tblEmailIncitationQualite.RefEntite = tblEntite.RefEntite"
                        + "     left join tblUtilisateur on tblEmailIncitationQualite.RefUtilisateurCreation = tblUtilisateur.RefUtilisateur"
                        + " where tblEmailIncitationQualite.DCreation>=@begin and tblEmailIncitationQualite.DCreation<@end";
                    if (!string.IsNullOrEmpty(eSF.FilterText))
                    {
                        sqlStr += " and (tblEntite.CodeEE=@filterText or tblEntite.Libelle COLLATE Latin1_general_CI_AI like '%' + @filterText + '%' COLLATE Latin1_general_CI_AI or tblEmailIncitationQualite.EmailTo like '%' + @filterText + '%')";
                        cmd.Parameters.Add("@filterText", SqlDbType.NVarChar).Value = eSF.FilterText;
                    }
                }
                if (eSF.FilterEmailType == "NoteCreditCollectivite")
                {
                    sqlStr = "select Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.GenerePar.ToString()].Name + "]"
                        + "     , RefSAGEDocument as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGENumeroPiece.ToString()].Name + "]"
                        + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + "     , tblEmailNoteCredit.EmailTo as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseEmail.ToString()].Name + "]"
                        + "     , tblEmailNoteCredit.DCreation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EmailDEnvoi.ToString()].Name + "]"
                        + " from tblEmailNoteCredit"
                        + "     left join tblUtilisateur on tblEmailNoteCredit.RefUtilisateurCreation = tblUtilisateur.RefUtilisateur"
                        + "     left join F_DOCENTETE on tblEmailNoteCredit.RefSAGEDocument = F_DOCENTETE.DO_PIECE"
                        + "     left join tblEntite on tblEntite.SAGECodeComptable = F_DOCENTETE.DO_TIERS"
                        + " where tblEmailNoteCredit.DCreation>=@begin and tblEmailNoteCredit.DCreation<@end";
                    if (!string.IsNullOrEmpty(eSF.FilterText))
                    {
                        sqlStr += " and (tblEntite.CodeEE=@filterText or tblEntite.Libelle COLLATE Latin1_general_CI_AI like '%' + @filterText + '%' COLLATE Latin1_general_CI_AI or tblEmailNoteCredit.EmailTo like '%' + @filterText + '%' or tblEmailNoteCredit.RefSAGEDocument=@filterText)";
                        cmd.Parameters.Add("@filterText", SqlDbType.NVarChar).Value = eSF.FilterText;
                    }
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.AnnuaireMenuExtractionAction.ToString())
            {
                sqlStr = "select tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                    + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                    + "     , isnull(tblContact.Prenom,'') + ' ' + tblContact.Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TexteContactAdresseList.ToString()].Name + "]"
                    + "     , tbrFormeContact.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FormeContactLibelle.ToString()].Name + "]"
                    + "     , DAction as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ActionDate.ToString()].Name + "]"
                    + "     , dbo.ListeActionActionType(tblAction.RefAction) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ActionTypeLibelle.ToString()].Name + "]"
                    + "     , tblAction.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ActionLibelle.ToString()].Name + "]"
                    + "     , tblAction.Cmt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ActionCmt.ToString()].Name + "]"
                    + "     , dbo.ListeActionFichier(tblAction.RefAction) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DocumentJoint.ToString()].Name + "]"
                    + "     , utilisateurCreation.Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CreationPar.ToString()].Name + "]"
                    + "     , utilisateurModification.Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ModificationPar.ToString()].Name + "]"
                    + " from tblAction"
                    + "     left join tbrFormeContact on tblAction.RefFormeContact=tbrFormeContact.RefFormeContact"
                    + "     left join tblUtilisateur as utilisateurCreation on tblAction.RefUtilisateurCreation=utilisateurCreation.RefUtilisateur"
                    + "     left join tblUtilisateur as utilisateurModification on tblAction.RefUtilisateurModif=utilisateurModification.RefUtilisateur"
                    + "     inner join tblEntite on tblAction.RefEntite=tblEntite.RefEntite"
                    + "     left join tbmContactAdresse on tblAction.RefContactAdresse=tbmContactAdresse.RefContactAdresse"
                    + "     left join tblContact on tbmContactAdresse.RefContact=tblContact.RefContact"
                    + " where DAction between @begin and @end";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end.AddDays(1);
                    }
                }
                sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, eSF.FilterText
                    , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteLibelle }
                    , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteCodeCITEO }
                    );
                if (eSF.FilterActionTypes != "")
                {
                    sqlStr += " and tblAction.RefAction in (select RefAction from tbmActionActionType where RefActionType in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refActionType", eSF.FilterActionTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                if (eSF.FilterEntiteTypes != "")
                {
                    sqlStr += " and tblEntite.RefEntiteType in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refEntiteType", eSF.FilterEntiteTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += "))";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString())
            {

                string filterRefEntiteType = "", filterRefAdresseType = "", filterRefContactAdresseProcess = "", filterRefFonction = "", filterRefService = "", filterRefProcess = "", filterRefDR = "";
                if (eSF.FilterEntiteTypes != "")
                {
                    filterRefEntiteType = Utils.Utils.CreateSQLParametersFromString("refEntiteType", eSF.FilterEntiteTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterAdresseTypes != "")
                {
                    filterRefAdresseType = Utils.Utils.CreateSQLParametersFromString("refAdresseType", eSF.FilterAdresseTypes, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterContactAdresseProcesss != "")
                {
                    filterRefContactAdresseProcess = Utils.Utils.CreateSQLParametersFromString("refContactAdresseProcess", eSF.FilterContactAdresseProcesss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterFonctions != "")
                {
                    filterRefFonction = Utils.Utils.CreateSQLParametersFromString("refFonction", eSF.FilterFonctions, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterServices != "")
                {
                    filterRefService = Utils.Utils.CreateSQLParametersFromString("refService", eSF.FilterServices, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterProcesss != "")
                {
                    filterRefProcess = Utils.Utils.CreateSQLParametersFromString("refProcess", eSF.FilterProcesss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    filterRefDR = Utils.Utils.CreateSQLParametersFromString("refDR", eSF.FilterDRs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }

                sqlStr = "select distinct ";
                if (string.IsNullOrWhiteSpace(filterContactSelectedColumns))
                {
                    sqlStr += "'Aucune colonne sélectionnée' as [Erreur]";
                }
                else
                {
                    sqlStr += filterContactSelectedColumns;
                    if (!CurrentContext.filterGlobalActif)
                    {
                        sqlStr += Utils.Utils.AddActifFields(filterContactSelectedColumns, CurrentContext);
                    }
                }
                sqlStr += " from (";
                sqlStr += " select tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Id.ToString()].Name + "]"
                    + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                    + "     , tblEntite.SousContrat as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSousContrat.ToString()].Name + "]"
                    + "     , tbrEntiteType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTypeLibelle.ToString()].Name + "]"
                    + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                    + "     , tblEntite.Capacite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCapacite.ToString()].Name + "]"
                    + "     , tblEntite.ActionnaireProprietaire as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteActionnaireProprietaire.ToString()].Name + "]"
                    + "     , tblEntite.Exploitant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteExploitant.ToString()].Name + "]"
                    + "     , tblEntite.DimensionBalle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteDimensionBalle.ToString()].Name + "]"
                    + "     , tbrEquipementier.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EquipementierLibelle.ToString()].Name + "]"
                    + "     , tbrFournisseurTO.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurTOLibelle.ToString()].Name + "]"
                    + "     , tblEntite.PopulationContratN as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntitePopulationContratN.ToString()].Name + "]"
                    + "     , tblEntite.VisibiliteAffretementCommun as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteVisibiliteAffretementCommun.ToString()].Name + "]"
                    + "     , tblEntite.SurcoutCarburant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSurcoutCarburant.ToString()].Name + "]"
                    + "     , dbo.ListeDR(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteProcess(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeProcesss.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteStandard(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeStandards.ToString()].Name + "]"
                    + "     , tblEntite.Cmt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCmt.ToString()].Name + "]"
                    + "     , tblEntite.CodeValorisation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeValorisation.ToString()].Name + "]"
                    + "     , tbrEcoOrganisme.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EcoOrganismeLibelle.ToString()].Name + "]"
                    + "     , tblEntite.AssujettiTVA as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteAssujettiTVA.ToString()].Name + "]"
                    + "     , tblEntite.CodeTVA as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeTVA.ToString()].Name + "]"
                    + "     , tblEntite.IdNational as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteIdNational.ToString()].Name + "]"
                    + "     , tblEntite.SAGECodeComptable as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSAGECodeComptable.ToString()].Name + "]"
                    + "     , tblEntite.SAGECompteTiers as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSAGECompteTiers.ToString()].Name + "]"
                    + "     , tbrSAGECategorieAchat.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGECategorieAchatLibelle.ToString()].Name + "]"
                    + "     , tbrSAGEConditionLivraison.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEConditionLivraisonLibelle.ToString()].Name + "]"
                    + "     , tbrSAGEPeriodicite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEPeriodiciteLibelle.ToString()].Name + "]"
                    + "     , tbrSAGEModeReglement.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEModeReglementLibelle.ToString()].Name + "]"
                    + "     , tbrSAGECategorieVente.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGECategorieVenteLibelle.ToString()].Name + "]"
                    + "     , tblEntite.ExportSAGE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteExportSAGE.ToString()].Name + "]"
                    + "     , tblEntite.AutoControle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MiseEnPlaceAutocontrole.ToString()].Name + "]"
                    + "     , tblEntite.ReconductionIncitationQualite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteReconductionIncitationQualite.ToString()].Name + "]"
                    + "     , tbrRepriseType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RepriseTypeLibelle.ToString()].Name + "]"
                    + "     , tbrRepreneur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RepreneurLibelle.ToString()].Name + "]"
                    + "     , tblContratCollectivite.DDebut as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratCollectiviteDebut.ToString()].Name + "]"
                    + "     , tblContratCollectivite.DFin as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratCollectiviteFin.ToString()].Name + "]"
                    + "     , cast(case when tblContratCollectivite.DFin>=getdate() then 1 else 0 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteContratCollectiviteActif.ToString()].Name + "]"
                    + "     , dbo.ListeContratCollectivite(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeContratCollectivites.ToString()].Name + "]"
                    + "     , tblContratIncitationQualite.DDebut as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratIncitationQualiteDebut.ToString()].Name + "]"
                    + "     , tblContratIncitationQualite.DFin as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratIncitationQualiteFin.ToString()].Name + "]"
                    + "     , cast(case when tblContratIncitationQualite.DFin>=getdate() then 1 else 0 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SousContratIncitationQualite.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteProduit(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeProduits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteProduitInterdit(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeProduitInterdits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,1,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCollectiviteActifs.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,1,0) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCollectiviteInactifs.ToString()].Name + "]"
                    + "     , case when tblEntite.RefEntiteType=3 then dbo.ListeEntiteCamionTypes(tblEntite.RefEntite) else null end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriListeCamionTypes.ToString()].Name + "]"
                    + "     , case when tblEntite.RefEntiteType=2 then dbo.ListeEntiteCamionTypes(tblEntite.RefEntite) else null end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurListeCamionTypes.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,3,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCentreDeTriActifs.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,3,0) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCentreDeTriInactifs.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,3,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCentreDeTriActifInterdits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,2,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTransporteurActifInterdits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,4,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeClientActifInterdits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,5,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeIndustrielActifs.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteDocumentPublic(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeDocumentPublic.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteDocumentValorplast(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeDocumentValorplast.ToString()].Name + "]"
                    + "     , tbrAdresseType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseTypeLibelle.ToString()].Name + "]"
                    + "     , tblAdresse.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseLibelle.ToString()].Name + "]"
                    + "     , tblAdresse.Adr1 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseAdr1.ToString()].Name + "]"
                    + "     , tblAdresse.Adr2 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseAdr2.ToString()].Name + "]"
                    + "     , tblAdresse.CodePostal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseCodePostal.ToString()].Name + "]"
                    + "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseVille.ToString()].Name + "]"
                    + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysLibelle.ToString()].Name + "]"
                    + "     , tblAdresse.Horaires as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseHoraires.ToString()].Name + "]"
                    + "     , tblAdresse.Tel as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseTel.ToString()].Name + "]"
                    + "     , tblAdresse.Email as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseEmail.ToString()].Name + "]"
                    + "     , tblAdresse.SiteWeb as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseSiteWeb.ToString()].Name + "]"
                    + "     , tbrTitre.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TitreLibelle.ToString()].Name + "]"
                    + "     , tbrCivilite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CiviliteLibelle.ToString()].Name + "]"
                    + "     , tblContact.Prenom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactPrenom.ToString()].Name + "]"
                    + "     , tblContact.Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactNom.ToString()].Name + "]"
                    + "     , tbmContactAdresse.Cmt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseCmt.ToString()].Name + "]"
                    + "     , dbo.ListeFonctionService(tbmContactAdresse.RefContactAdresse) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseListeFonctionServices.ToString()].Name + "]"
                    + "     , tbmContactAdresse.CmtServiceFonction as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseCmtServiceFonction.ToString()].Name + "]"
                    + "     , tbmContactAdresse.Tel as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseTel.ToString()].Name + "]"
                    + "     , tbmContactAdresse.TelMobile as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseTelMobile.ToString()].Name + "]"
                    + "     , tbmContactAdresse.Email as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseEmail.ToString()].Name + "]"
                    + "     , dbo.ListeProcess(tbmContactAdresse.RefContactAdresse) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseListeProcesss.ToString()].Name + "]"
                    + "     , cast(case when tbmContactAdresse.RefContactAdresse is null then null else case when tblUtilisateur.RefUtilisateur is null then 0 else 1 end end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseIsUser.ToString()].Name + "]"
                    + "     , tblEntite.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteActif.ToString()].Name + "]"
                    + "     , tblAdresse.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseActif.ToString()].Name + "]"
                    + "     , tbmContactAdresse.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseActif.ToString()].Name + "]"
                    + "     , DR.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DRActif.ToString()].Name + "]";
                sqlStr += " from tblEntite";
                if (CurrentContext.filterGlobalActif)
                {
                    sqlStr += "     left join (select * from  tblAdresse where Actif=1 and (RefPays in (select RefPays from tbrPays where Actif=1) or RefPays is null)) as tblAdresse on tblEntite.RefEntite = tblAdresse.RefEntite";
                }
                else
                {
                    sqlStr += "     left join tblAdresse on tblEntite.RefEntite = tblAdresse.RefEntite";
                }
                if (CurrentContext.filterGlobalActif)
                {
                    sqlStr += "     left join (select * from  tbmContactAdresse where Actif=1) as tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse";
                }
                else
                {
                    sqlStr += "     left join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse";
                }
                sqlStr += "     left join tblContact on tbmContactAdresse.RefContact = tblContact.RefContact"
                    + "     left join tbmEntiteDR on tblEntite.RefEntite = tbmEntiteDR.RefEntite";
                if (CurrentContext.filterGlobalActif)
                {
                    sqlStr += "     left join (select * from  tblUtilisateur where Actif=1) as DR on tbmEntiteDR.RefDR = DR.RefUtilisateur";
                }
                else
                {
                    sqlStr += "     left join tblUtilisateur as DR on tbmEntiteDR.RefDR = DR.RefUtilisateur";
                }
                sqlStr += "     left join tbrEntiteType on tblEntite.RefEntiteType = tbrEntiteType.RefEntiteType"
                    + "     left join tbrRepreneur on tblEntite.RefRepreneur=tbrRepreneur.RefRepreneur"
                    + "     left join tbrEcoOrganisme on tblEntite.RefEcoOrganisme=tbrEcoOrganisme.RefEcoOrganisme"
                    + "     left join tbrEquipementier on tblEntite.RefEquipementier=tbrEquipementier.RefEquipementier"
                    + "     left join tbrFournisseurTO on tblEntite.RefFournisseurTO=tbrFournisseurTO.RefFournisseurTO"
                    + "     left join tbrSAGECategorieAchat on tblEntite.RefSAGECategorieAchat=tbrSAGECategorieAchat.RefSAGECategorieAchat"
                    + "     left join tbrSAGEConditionLivraison on tblEntite.RefSAGEConditionLivraison=tbrSAGEConditionLivraison.RefSAGEConditionLivraison"
                    + "     left join tbrSAGEPeriodicite on tblEntite.RefSAGEPeriodicite=tbrSAGEPeriodicite.RefSAGEPeriodicite"
                    + "     left join tbrSAGEModeReglement on tblEntite.RefSAGEModeReglement=tbrSAGEModeReglement.RefSAGEModeReglement"
                    + "     left join tbrSAGECategorieVente on tblEntite.RefSAGECategorieVente=tbrSAGECategorieVente.RefSAGECategorieVente"
                    + "     left join tbrRepriseType on tblEntite.RefRepriseType=tbrRepriseType.RefRepriseType"
                    + "     left join tbrAdresseType on tblAdresse.RefAdresseType = tbrAdresseType.RefAdresseType"
                    + "     left join tbrPays on tblAdresse.RefPays = tbrPays.RefPays"
                    + "     left join (select RefEntite, max(DDebut) as DDebut, Max(DFin) as DFin from tblContratCollectivite group by RefEntite) as tblContratCollectivite on tblEntite.RefEntite=tblContratCollectivite.RefEntite"
                    + "     left join (select RefEntite, max(DDebut) as DDebut, Max(DFin) as DFin from tblContratOptimisationTransport group by RefEntite) as tblContratOptimisationTransport on tblEntite.RefEntite=tblContratOptimisationTransport.RefEntite"
                    + "     left join (select RefEntite, max(DDebut) as DDebut, Max(DFin) as DFin from tblContratIncitationQualite group by RefEntite) as tblContratIncitationQualite on tblEntite.RefEntite=tblContratIncitationQualite.RefEntite"
                    + "     left join tbrCivilite on tblContact.RefCivilite = tbrCivilite.RefCivilite"
                    + "     left join tbrTitre on tbmContactAdresse.RefTitre = tbrTitre.RefTitre"
                    + "     left join tblUtilisateur on tbmContactAdresse.RefContactAdresse = tblUtilisateur.RefContactAdresse"
                    + " where 1=1";
                //Filters
                if (eSF.FilterEntiteTypes != "")
                {
                    sqlStr += " and tblEntite.RefEntiteType in (";
                    sqlStr += filterRefEntiteType;
                    sqlStr += ")";
                }
                if (eSF.FilterAdresseTypes != "")
                {
                    sqlStr += " and tblAdresse.RefAdresseType in (";
                    sqlStr += filterRefAdresseType;
                    sqlStr += ")";
                }
                if (eSF.FilterContactAdresseProcesss != "")
                {
                    sqlStr += " and tbmContactAdresse.RefContactAdresse in (select RefContactAdresse from tbmContactAdresseContactAdresseProcess where RefContactAdresseProcess in(";
                    sqlStr += filterRefContactAdresseProcess;
                    sqlStr += "))";
                }
                if (eSF.FilterFonctions != "")
                {
                    sqlStr += " and tbmContactAdresse.RefContactAdresse in (select RefContactAdresse from tbmContactAdresseServiceFonction where RefFonction in(";
                    sqlStr += filterRefFonction;
                    sqlStr += "))";
                }
                if (eSF.FilterServices != "")
                {
                    sqlStr += " and tbmContactAdresse.RefContactAdresse in (select RefContactAdresse from tbmContactAdresseServiceFonction where RefService in(";
                    sqlStr += filterRefService;
                    sqlStr += "))";
                }
                if (eSF.FilterProcesss != "")
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteProcess where RefProcess in(";
                    sqlStr += filterRefProcess;
                    sqlStr += "))";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += filterRefDR;
                    sqlStr += "))";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                //Filter actif
                if (CurrentContext.filterGlobalActif)
                {
                    sqlStr += " and tblEntite.Actif=1";
                }
                //End
                sqlStr += " union all ";
                sqlStr += " select tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Id.ToString()].Name + "]"
                    + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                    + "     , tblEntite.SousContrat as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSousContrat.ToString()].Name + "]"
                    + "     , tbrEntiteType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTypeLibelle.ToString()].Name + "]"
                    + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                    + "     , tblEntite.Capacite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCapacite.ToString()].Name + "]"
                    + "     , tblEntite.ActionnaireProprietaire as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteActionnaireProprietaire.ToString()].Name + "]"
                    + "     , tblEntite.Exploitant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteExploitant.ToString()].Name + "]"
                    + "     , tblEntite.DimensionBalle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteDimensionBalle.ToString()].Name + "]"
                    + "     , tbrEquipementier.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EquipementierLibelle.ToString()].Name + "]"
                    + "     , tbrFournisseurTO.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurTOLibelle.ToString()].Name + "]"
                    + "     , tblEntite.PopulationContratN as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntitePopulationContratN.ToString()].Name + "]"
                    + "     , tblEntite.VisibiliteAffretementCommun as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteVisibiliteAffretementCommun.ToString()].Name + "]"
                    + "     , tblEntite.SurcoutCarburant as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSurcoutCarburant.ToString()].Name + "]"
                    + "     , dbo.ListeDR(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteProcess(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeProcesss.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteStandard(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeStandards.ToString()].Name + "]"
                    + "     , tblEntite.Cmt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCmt.ToString()].Name + "]"
                    + "     , tblEntite.CodeValorisation as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeValorisation.ToString()].Name + "]"
                    + "     , tbrEcoOrganisme.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EcoOrganismeLibelle.ToString()].Name + "]"
                    + "     , tblEntite.AssujettiTVA as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteAssujettiTVA.ToString()].Name + "]"
                    + "     , tblEntite.CodeTVA as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeTVA.ToString()].Name + "]"
                    + "     , tblEntite.IdNational as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteIdNational.ToString()].Name + "]"
                    + "     , tblEntite.SAGECodeComptable as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSAGECodeComptable.ToString()].Name + "]"
                    + "     , tblEntite.SAGECompteTiers as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSAGECompteTiers.ToString()].Name + "]"
                    + "     , tbrSAGECategorieAchat.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGECategorieAchatLibelle.ToString()].Name + "]"
                    + "     , tbrSAGEConditionLivraison.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEConditionLivraisonLibelle.ToString()].Name + "]"
                    + "     , tbrSAGEPeriodicite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEPeriodiciteLibelle.ToString()].Name + "]"
                    + "     , tbrSAGEModeReglement.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEModeReglementLibelle.ToString()].Name + "]"
                    + "     , tbrSAGECategorieVente.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGECategorieVenteLibelle.ToString()].Name + "]"
                    + "     , tblEntite.ExportSAGE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteExportSAGE.ToString()].Name + "]"
                    + "     , tblEntite.AutoControle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MiseEnPlaceAutocontrole.ToString()].Name + "]"
                    + "     , tblEntite.ReconductionIncitationQualite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteReconductionIncitationQualite.ToString()].Name + "]"
                    + "     , tbrRepriseType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RepriseTypeLibelle.ToString()].Name + "]"
                    + "     , tbrRepreneur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RepreneurLibelle.ToString()].Name + "]"
                    + "     , tblContratCollectivite.DDebut as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratCollectiviteDebut.ToString()].Name + "]"
                    + "     , tblContratCollectivite.DFin as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratCollectiviteFin.ToString()].Name + "]"
                    + "     , cast(case when tblContratCollectivite.DFin>=getdate() then 1 else 0 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteContratCollectiviteActif.ToString()].Name + "]"
                    + "     , dbo.ListeContratCollectivite(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeContratCollectivites.ToString()].Name + "]"
                    + "     , tblContratIncitationQualite.DDebut as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratIncitationQualiteDebut.ToString()].Name + "]"
                    + "     , tblContratIncitationQualite.DFin as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratIncitationQualiteFin.ToString()].Name + "]"
                    + "     , cast(case when tblContratIncitationQualite.DFin>=getdate() then 1 else 0 end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SousContratIncitationQualite.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteProduit(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeProduits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteProduitInterdit(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeProduitInterdits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,1,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCollectiviteActifs.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,1,0) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCollectiviteInactifs.ToString()].Name + "]"
                    + "     , case when tblEntite.RefEntiteType=3 then dbo.ListeEntiteCamionTypes(tblEntite.RefEntite) else null end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriListeCamionTypes.ToString()].Name + "]"
                    + "     , case when tblEntite.RefEntiteType=2 then dbo.ListeEntiteCamionTypes(tblEntite.RefEntite) else null end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurListeCamionTypes.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,3,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCentreDeTriActifs.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,3,0) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCentreDeTriInactifs.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,3,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCentreDeTriActifInterdits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,2,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTransporteurActifInterdits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,4,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeClientActifInterdits.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteEntite(tblEntite.RefEntite,5,1) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeIndustrielActifs.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteDocumentPublic(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeDocumentPublic.ToString()].Name + "]"
                    + "     , dbo.ListeEntiteDocumentValorplast(tblEntite.RefEntite) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeDocumentValorplast.ToString()].Name + "]"
                    + "     , tbrAdresseType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseTypeLibelle.ToString()].Name + "]"
                    + "     , tblAdresse.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseLibelle.ToString()].Name + "]"
                    + "     , tblAdresse.Adr1 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseAdr1.ToString()].Name + "]"
                    + "     , tblAdresse.Adr2 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseAdr2.ToString()].Name + "]"
                    + "     , tblAdresse.CodePostal as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseCodePostal.ToString()].Name + "]"
                    + "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseVille.ToString()].Name + "]"
                    + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysLibelle.ToString()].Name + "]"
                    + "     , tblAdresse.Horaires as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseHoraires.ToString()].Name + "]"
                    + "     , tblAdresse.Tel as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseTel.ToString()].Name + "]"
                    + "     , tblAdresse.Email as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseEmail.ToString()].Name + "]"
                    + "     , tblAdresse.SiteWeb as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseSiteWeb.ToString()].Name + "]"
                    + "     , tbrTitre.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TitreLibelle.ToString()].Name + "]"
                    + "     , tbrCivilite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CiviliteLibelle.ToString()].Name + "]"
                    + "     , tblContact.Prenom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactPrenom.ToString()].Name + "]"
                    + "     , tblContact.Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactNom.ToString()].Name + "]"
                    + "     , tbmContactAdresse.Cmt as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseCmt.ToString()].Name + "]"
                    + "     , dbo.ListeFonctionService(tbmContactAdresse.RefContactAdresse) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseListeFonctionServices.ToString()].Name + "]"
                    + "     , tbmContactAdresse.CmtServiceFonction as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseCmtServiceFonction.ToString()].Name + "]"
                    + "     , tbmContactAdresse.Tel as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseTel.ToString()].Name + "]"
                    + "     , tbmContactAdresse.TelMobile as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseTelMobile.ToString()].Name + "]"
                    + "     , tbmContactAdresse.Email as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseEmail.ToString()].Name + "]"
                    + "     , dbo.ListeProcess(tbmContactAdresse.RefContactAdresse) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseListeProcesss.ToString()].Name + "]"
                    + "     , cast(case when tbmContactAdresse.RefContactAdresse is null then null else case when tblUtilisateur.RefUtilisateur is null then 0 else 1 end end as bit) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseIsUser.ToString()].Name + "]"
                    + "     , tblEntite.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteActif.ToString()].Name + "]"
                    + "     , tblAdresse.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseActif.ToString()].Name + "]"
                    + "     , tbmContactAdresse.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseActif.ToString()].Name + "]"
                    + "     , DR.Actif as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DRActif.ToString()].Name + "]";
                sqlStr += " from tblEntite";
                //Contacts sans adresse
                sqlStr += "     left join (select * from  tblAdresse where 1!=1) as tblAdresse on tblEntite.RefEntite = tblAdresse.RefEntite";
                if (CurrentContext.filterGlobalActif)
                {
                    sqlStr += "     inner join (select tbmContactAdresse.* from tbmContactAdresse left join tblAdresse on tbmContactAdresse.RefAdresse = tblAdresse.RefAdresse where tblAdresse.RefAdresse is null and tbmContactAdresse.Actif=1) as tbmContactAdresse on tbmContactAdresse.RefEntite=tblEntite.RefEntite";
                }
                else
                {
                    sqlStr += "     inner join (select tbmContactAdresse.* from tbmContactAdresse left join tblAdresse on tbmContactAdresse.RefAdresse = tblAdresse.RefAdresse where tblAdresse.RefAdresse is null) as tbmContactAdresse on tbmContactAdresse.RefEntite=tblEntite.RefEntite";
                }
                sqlStr += "     left join tblContact on tbmContactAdresse.RefContact = tblContact.RefContact"
                    + "     left join tbmEntiteDR on tblEntite.RefEntite = tbmEntiteDR.RefEntite";
                if (CurrentContext.filterGlobalActif)
                {
                    sqlStr += "     left join (select * from  tblUtilisateur where Actif=1) as DR on tbmEntiteDR.RefDR = DR.RefUtilisateur";
                }
                else
                {
                    sqlStr += "     left join tblUtilisateur as DR on tbmEntiteDR.RefDR = DR.RefUtilisateur";
                }
                sqlStr += "     left join tbrEntiteType on tblEntite.RefEntiteType = tbrEntiteType.RefEntiteType"
                    + "     left join tbrRepreneur on tblEntite.RefRepreneur=tbrRepreneur.RefRepreneur"
                    + "     left join tbrEcoOrganisme on tblEntite.RefEcoOrganisme=tbrEcoOrganisme.RefEcoOrganisme"
                    + "     left join tbrEquipementier on tblEntite.RefEquipementier=tbrEquipementier.RefEquipementier"
                    + "     left join tbrFournisseurTO on tblEntite.RefFournisseurTO=tbrFournisseurTO.RefFournisseurTO"
                    + "     left join tbrSAGECategorieAchat on tblEntite.RefSAGECategorieAchat=tbrSAGECategorieAchat.RefSAGECategorieAchat"
                    + "     left join tbrSAGEConditionLivraison on tblEntite.RefSAGEConditionLivraison=tbrSAGEConditionLivraison.RefSAGEConditionLivraison"
                    + "     left join tbrSAGEPeriodicite on tblEntite.RefSAGEPeriodicite=tbrSAGEPeriodicite.RefSAGEPeriodicite"
                    + "     left join tbrSAGEModeReglement on tblEntite.RefSAGEModeReglement=tbrSAGEModeReglement.RefSAGEModeReglement"
                    + "     left join tbrSAGECategorieVente on tblEntite.RefSAGECategorieVente=tbrSAGECategorieVente.RefSAGECategorieVente"
                    + "     left join tbrRepriseType on tblEntite.RefRepriseType=tbrRepriseType.RefRepriseType"
                    + "     left join tbrAdresseType on tblAdresse.RefAdresseType = tbrAdresseType.RefAdresseType"
                    + "     left join tbrPays on tblAdresse.RefPays = tbrPays.RefPays"
                    + "     left join (select RefEntite, max(DDebut) as DDebut, Max(DFin) as DFin from tblContratCollectivite group by RefEntite) as tblContratCollectivite on tblEntite.RefEntite=tblContratCollectivite.RefEntite"
                    + "     left join (select RefEntite, max(DDebut) as DDebut, Max(DFin) as DFin from tblContratOptimisationTransport group by RefEntite) as tblContratOptimisationTransport on tblEntite.RefEntite=tblContratOptimisationTransport.RefEntite"
                    + "     left join (select RefEntite, max(DDebut) as DDebut, Max(DFin) as DFin from tblContratIncitationQualite group by RefEntite) as tblContratIncitationQualite on tblEntite.RefEntite=tblContratIncitationQualite.RefEntite"
                    + "     left join tbrCivilite on tblContact.RefCivilite = tbrCivilite.RefCivilite"
                    + "     left join tbrTitre on tbmContactAdresse.RefTitre = tbrTitre.RefTitre"
                    + "     left join tblUtilisateur on tbmContactAdresse.RefContactAdresse = tblUtilisateur.RefContactAdresse"
                    + " where 1=1";
                //Filters
                if (eSF.FilterEntiteTypes != "")
                {
                    sqlStr += " and tblEntite.RefEntiteType in (";
                    sqlStr += filterRefEntiteType;
                    sqlStr += ")";
                }
                if (eSF.FilterAdresseTypes != "")
                {
                    sqlStr += " and tblAdresse.RefAdresseType in (";
                    sqlStr += filterRefAdresseType;
                    sqlStr += ")";
                }
                if (eSF.FilterContactAdresseProcesss != "")
                {
                    sqlStr += " and tbmContactAdresse.RefContactAdresse in (select RefContactAdresse from tbmContactAdresseContactAdresseProcess where RefContactAdresseProcess in(";
                    sqlStr += filterRefContactAdresseProcess;
                    sqlStr += "))";
                }
                if (eSF.FilterFonctions != "")
                {
                    sqlStr += " and tbmContactAdresse.RefContactAdresse in (select RefContactAdresse from tbmContactAdresseServiceFonction where RefFonction in(";
                    sqlStr += filterRefFonction;
                    sqlStr += "))";
                }
                if (eSF.FilterServices != "")
                {
                    sqlStr += " and tbmContactAdresse.RefContactAdresse in (select RefContactAdresse from tbmContactAdresseServiceFonction where RefService in(";
                    sqlStr += filterRefService;
                    sqlStr += "))";
                }
                if (eSF.FilterProcesss != "")
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteProcess where RefProcess in(";
                    sqlStr += filterRefProcess;
                    sqlStr += "))";
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs))
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR in(";
                    sqlStr += filterRefDR;
                    sqlStr += "))";
                }
                if (CurrentContext.filterDR)
                {
                    sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                }
                //Filter actif
                if (CurrentContext.filterGlobalActif)
                {
                    sqlStr += " and tblEntite.Actif=1";
                }
                //End
                sqlStr += " ) as univers";
                //Count
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.AnnuaireMenuNbAffretementTransporteur.ToString())
            {
                //First command for fields
                sqlStr = "select distinct tbrPays.Libelle"
                    + " from tblCommandeFournisseur"
                    + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                    + "     inner join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                    + " where 1=1";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                    sqlStr += " and tblCommandeFournisseur.DDechargement between @begin and @end";
                }
                if (eSF.FilterTransporteurs != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefTransporteur in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", eSF.FilterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterPayss != "")
                {
                    sqlStr += " and tbrPays.RefPays in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refPays", eSF.FilterPayss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                //Order
                sqlStr += " order by tbrPays.Libelle";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        s1 += ", sum(case when tbrPays.Libelle='" + dr.GetSqlString(0).ToString().Replace("'", "''") + "'"
                            + " then 1 else 0 end)"
                            + " as [" + dr.GetSqlString(0).ToString() + "]";
                    }
                    dr.Close();
                }
                cmd = new SqlCommand();
                //Main query
                sqlStr = "select cast(month(tblCommandeFournisseur.DDechargement) as nvarchar) + '/' + cast(year(tblCommandeFournisseur.DDechargement) as nvarchar) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MoisAnnee.ToString()].Name + "]";
                if (eSF.FilterPayss != "")
                {
                    sqlStr += s1;
                }
                else
                {
                    sqlStr += "     , count(*) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.NbAffretement.ToString()].Name + "]";
                }
                sqlStr += " from tblCommandeFournisseur"
                + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                + "     inner join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                + " where 1=1";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                    sqlStr += " and tblCommandeFournisseur.DDechargement between @begin and @end";
                }
                if (eSF.FilterTransporteurs != "")
                {
                    sqlStr += " and tblCommandeFournisseur.RefTransporteur in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refTransporteur", eSF.FilterTransporteurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterPayss != "")
                {
                    sqlStr += " and tbrPays.RefPays in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refPays", eSF.FilterPayss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                sqlStr += " group by year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement)";
                if (eSF.FilterPayss != "")
                {
                    //sqlStr += ", tbrPays.Libelle";
                }
                //Count
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString())
            {
                //First command for fields
                sqlStr = "select distinct tblProduit.Libelle"
                    + " from tblRepartition"
                    + "     left join VueRepartitionDetail on tblRepartition.RefRepartition=VueRepartitionDetail.RefRepartition"
                    + "     left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + "     inner join tblProduit on VueRepartitionDetail.RefProduit=tblProduit.refProduit"
                    + " where 1=1";

                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                    sqlStr += " and isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement) between @begin and @end";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and VueRepartitionDetail.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollectivites != "")
                {
                    sqlStr += " and VueRepartitionDetail.RefCollectivite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCollectivite", eSF.FilterCollectivites, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                //Order
                sqlStr += " order by tblProduit.Libelle";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        s += ", [" + dr.GetSqlString(0).ToString() + "]";
                        s1 += ", cast(sum(case when tblProduit.Libelle='" + dr.GetSqlString(0).ToString().Replace("'", "''") + "'"
                            + " then VueRepartitionDetail.Poids else 0 end) as decimal(15,3))/1000"
                            + " as [" + dr.GetSqlString(0).ToString() + "]";
                    }
                    dr.Close();
                }
                cmd = new SqlCommand();
                //Main query
                sqlStr = "select cast(month(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement)) as nvarchar) + '/' + cast(year(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement)) as nvarchar) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MoisAnnee.ToString()].Name + "]";
                if (eSF.FilterProduits != "")
                {
                    sqlStr += s1;
                }
                else
                {
                    sqlStr += "     , cast(sum(VueRepartitionDetail.Poids) as decimal(15,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsTonne.ToString()].Name + "]";
                }
                sqlStr += "     from tblRepartition"
                    + "     left join VueRepartitionDetail on tblRepartition.RefRepartition=VueRepartitionDetail.RefRepartition"
                    + "     left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                    + "     inner join tblProduit on VueRepartitionDetail.RefProduit=tblProduit.refProduit"
                    + " where 1=1";
                //Filters
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    }
                    sqlStr += " and isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement) between @begin and @end";
                }
                if (eSF.FilterProduits != "")
                {
                    sqlStr += " and VueRepartitionDetail.RefProduit in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollectivites != "")
                {
                    sqlStr += " and VueRepartitionDetail.RefCollectivite in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refCollectivite", eSF.FilterCollectivites, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterCollecte == "Collecte")
                {
                    sqlStr += " and Collecte=1";
                }
                else if (eSF.FilterCollecte == "HorsCollecte")
                {
                    sqlStr += " and isnull(Collecte,0)=0";
                }
                sqlStr += " group by year(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement)), month(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement))";
                //Count
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.AdministrationMenuExtractionLogin.ToString())
            {
                //Logins extraction
                sqlStr = "select LogLogin.[Login] as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DLogin.ToString()].Name + "]"
                    + "     , Logout as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DLogout.ToString()].Name + "]"
                    + "     , tblUtilisateur.Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurNom.ToString()].Name + "]"
                    + "     , dbo.ListeUtilisateurEntite(tblUtilisateur.RefUtilisateur) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListe.ToString()].Name + "]"
                    + "     , dbo.ListeUtilisateurEntiteType(tblUtilisateur.RefUtilisateur) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTypeListe.ToString()].Name + "]"
                    + " from tblUtilisateur"
                    + "     left join tblEntite as client on tblUtilisateur.RefClient=client.RefEntite"
                    + "     left join tblEntite as centreDeTri on tblUtilisateur.RefCentreDeTri=centreDeTri.RefEntite"
                    + "     left join tblEntite as transporteur on tblUtilisateur.RefTransporteur=transporteur.RefEntite"
                    + "     left join tblEntite as prestataire on tblUtilisateur.RefPrestataire=prestataire.RefEntite"
                    + "     left join tblEntite as collectivite on tblUtilisateur.RefCollectivite=collectivite.RefEntite";
                if (eSF.FilterFirstLogin)
                {
                    sqlStr += " inner join LogLogin on tblUtilisateur.RefUtilisateur = LogLogin.RefUtilisateur";
                }
                else
                {
                    sqlStr += " inner join LogLogins as LogLogin on tblUtilisateur.RefUtilisateur = LogLogin.RefUtilisateur";
                }
                sqlStr += " where 1=1";
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    DateTime begin = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end.AddDays(1);
                    }
                    sqlStr += " and LogLogin.[Login] >= @begin and LogLogin.[Login] < @end";
                }
                if (eSF.FilterUtilisateurs != "")
                {
                    sqlStr += " and LogLogin.RefUtilisateur in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refUtilisateur", eSF.FilterUtilisateurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterEntites != "")
                {
                    string f = Utils.Utils.CreateSQLParametersFromString("refEntite", eSF.FilterEntites, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += " and logLogin.RefUtilisateur in ("
                        + "     select Refutilisateur from tblUtilisateur where "
                        + "     RefCentreDeTri in (" + f + ") or RefClient in (" + f + ") or RefCollectivite in (" + f + ") or RefPrestataire in (" + f + ") or RefTransporteur in (" + f + ")"
                    + ")";
                }
                if (eSF.FilterEntiteTypes != "")
                {
                    sqlStr += " and (1!=1 ";
                    string[] crits = eSF.FilterEntiteTypes.Split(',');
                    int i = 0;
                    foreach (string cr in crits)
                    {
                        switch (cr)
                        {
                            case "1":
                                sqlStr += " or logLogin.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefCollectivite is not null)";
                                break;
                            case "2":
                                sqlStr += " or logLogin.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefTransporteur is not null)";
                                break;
                            case "3":
                                sqlStr += " or logLogin.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefCentreDeTri is not null)";
                                break;
                            case "4":
                                sqlStr += " or logLogin.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefClient is not null)";
                                break;
                            case "8":
                                sqlStr += " or logLogin.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefPrestataire is not null)";
                                break;
                        }
                    }
                    sqlStr += ")";
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString())
            {
                //Suivi logins
                DateTime begin = DateTime.MinValue;
                DateTime end = DateTime.MinValue;
                if (eSF.FilterBegin != "" && eSF.FilterEnd != "")
                {
                    if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                    {
                        //Defines first date depending on period units
                        switch (eSF.FilterDayWeekMonth)
                        {
                            case "Week":
                                begin = begin.AddDays(-(int)begin.DayOfWeek).AddDays(1);
                                break;
                            case "Month":
                                begin = new DateTime(begin.Year, begin.Month, 1, 0, 0, 0);
                                break;
                        }

                        cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                        cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end.AddDays(1);
                    }
                }
                //Query
                switch (eSF.FilterDayWeekMonth)
                {
                    case "Day":
                        sqlStr = "select CONVERT(nvarchar, D, 23) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Periode.ToString()].Name + "]"
                            + "     , sum(Nb) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Connexions.ToString()].CulturedCaption + "]"
                            + "     , count(*) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PremieresConnexions.ToString()].CulturedCaption + "]"
                            + " from LogCount";
                        break;
                    case "Week":
                        sqlStr = "select datename(YEAR,D) + '-' + format(datepart(week,D),'00') as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Periode.ToString()].Name + "]"
                            + "     , sum(Nb) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Connexions.ToString()].CulturedCaption + "]"
                            + "     , count(*) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PremieresConnexions.ToString()].CulturedCaption + "]"
                            + " from LogCount";
                        break;
                    case "Month":
                        sqlStr = "select datename(YEAR,D) + '-' + format(datepart(month,D),'00') as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Periode.ToString()].Name + "]"
                            + "     , sum(Nb) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Connexions.ToString()].CulturedCaption + "]"
                            + "     , count(*) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PremieresConnexions.ToString()].CulturedCaption + "]"
                            + " from LogCount";
                        break;
                }
                //Where
                sqlStr += " where D>=@begin and D<@end";
                //Filters
                if (eSF.FilterUtilisateurs != "")
                {
                    sqlStr += " and LogCount.RefUtilisateur in (";
                    sqlStr += Utils.Utils.CreateSQLParametersFromString("refUtilisateur", eSF.FilterUtilisateurs, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += ")";
                }
                if (eSF.FilterEntites != "")
                {
                    string f = Utils.Utils.CreateSQLParametersFromString("refEntite", eSF.FilterEntites, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                    sqlStr += " and LogCount.RefUtilisateur in ("
                        + "     select Refutilisateur from tblUtilisateur where "
                        + "     RefCentreDeTri in (" + f + ") or RefClient in (" + f + ") or RefCollectivite in (" + f + ") or RefPrestataire in (" + f + ") or RefTransporteur in (" + f + ")"
                    + ")";
                }
                if (eSF.FilterEntiteTypes != "")
                {
                    sqlStr += " and (1!=1 ";
                    string[] crits = eSF.FilterEntiteTypes.Split(',');
                    int i = 0;
                    foreach (string cr in crits)
                    {
                        switch (cr)
                        {
                            case "1":
                                sqlStr += " or LogCount.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefCollectivite is not null)";
                                break;
                            case "2":
                                sqlStr += " or LogCount.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefTransporteur is not null)";
                                break;
                            case "3":
                                sqlStr += " or LogCount.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefCentreDeTri is not null)";
                                break;
                            case "4":
                                sqlStr += " or LogCount.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefClient is not null)";
                                break;
                            case "8":
                                sqlStr += " or LogCount.RefUtilisateur in (select Refutilisateur from tblUtilisateur where RefPrestataire is not null)";
                                break;
                        }
                    }
                    sqlStr += ")";
                }
                //End
                switch (eSF.FilterDayWeekMonth)
                {
                    case "Day":
                        sqlStr += " group by CONVERT(nvarchar, D, 23)";
                        break;
                    case "Week":
                        sqlStr += " group by datename(YEAR, D) + '-' + format(datepart(wk, D), '00')";
                        break;
                    case "Month":
                        sqlStr += " group by datename(YEAR, D) + '-' + format(datepart(month, D), '00')";
                        break;
                }
                sqlStrCount = sqlStr;
            }
            if (menu == Enumerations.MenuName.ModuleCollectiviteMenuStatistiques.ToString())
            {
                string filterCollectivites = "", filterProduits = "";
                //Filters
                DateTime begin = DateTime.MinValue;
                DateTime end = DateTime.MinValue;
                if (DateTime.TryParse(eSF.FilterBegin, out begin) && DateTime.TryParse(eSF.FilterEnd, out end))
                {
                    cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = begin;
                    cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                }
                if (DateTime.TryParse(eSF.FilterBegin, out begin))
                {
                    cmd.Parameters.Add("@year", SqlDbType.Int).Value = begin.Year;
                }
                if (eSF.FilterCollectivites != "")
                {
                    filterCollectivites = Utils.Utils.CreateSQLParametersFromString("refCollectivite", eSF.FilterCollectivites, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                if (eSF.FilterProduits != "")
                {
                    filterProduits = Utils.Utils.CreateSQLParametersFromString("refProduit", eSF.FilterProduits, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                }
                switch (eSF.StatType)
                {
                    case "DonneeCDT":
                        //Base SQL statement
                        sqlStr = "select year(tblCommandeFournisseur.DDechargement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnneeNb.ToString()].Name + "]"
                            + "     , month(tblCommandeFournisseur.DDechargement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MoisNb.ToString()].Name + "]"
                            + "     , fournisseur.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                            + "     , fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]"
                            + "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseVille.ToString()].Name + "]"
                            + "     , tblProduit.NomCommun as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                            + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                            + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                            + "     , tblCommandeFournisseur.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                            + "     , tbrCamionType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CamionTypeLibelle.ToString()].Name + "]"
                            + "     , tblCommandeFournisseur.DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]"
                            + "     , tblCommandeFournisseur.DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                            + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                            + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysLibelle.ToString()].Name + "]"
                            + " 	, univers.Poids as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsReparti.ToString()].Name + "]"
                            + "     , univers.PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHT.ToString()].Name + "]"
                            + " from"
                            + " 	( select VueRepartitionUnitaireDetail.RefCommandeFournisseur, VueRepartitionUnitaireDetail.PUHT, sum(VueRepartitionUnitaireDetail.Poids) as Poids"
                            + "         from tblCommandeFournisseur"
                            + "             inner join VueRepartitionUnitaireDetail on tblCommandeFournisseur.RefCommandeFournisseur=VueRepartitionUnitaireDetail.RefCommandeFournisseur"
                            + "         	left join VueCommandeFournisseurContrat on VueRepartitionUnitaireDetail.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                            //+ "         	inner join tbrPrixReprise on tbrPrixReprise.RefProcess=VueRepartitionUnitaireDetail.RefProcess and tbrPrixReprise.RefProduit=VueRepartitionUnitaireDetail.RefProduit "
                            //+ "         		and tbrPrixReprise.RefProduit=VueRepartitionUnitaireDetail.RefProduit"
                            //+ "         		and month(VueRepartitionUnitaireDetail.D)=month(tbrPrixReprise.D) and year(VueRepartitionUnitaireDetail.D)=year(tbrPrixReprise.D)"
                            //+ "         		and isnull(tbrPrixReprise.RefContrat,0)=isnull(VueCommandeFournisseurContrat.RefContrat,0)"
                            + "             where VueRepartitionUnitaireDetail.D between @begin and @end"
                            + "                 and VueRepartitionUnitaireDetail.RefFournisseur in (" + filterCollectivites + ")"
                            + "         group by VueRepartitionUnitaireDetail.RefCommandeFournisseur, VueRepartitionUnitaireDetail.PUHT"
                            + "      )"
                            + " 	 as univers"
                            + "         inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=univers.RefCommandeFournisseur"
                            + " 	    inner join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                            + " 	    inner join tblAdresse on tblAdresse.RefAdresse=tblCommandeFournisseur.RefAdresseClient"
                            + " 	    inner join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                            + " 	    inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                            + " 	    inner join tbrCamionType on tblCommandeFournisseur.RefCamionType=tbrCamionType.refCamionType"
                            + " 	    inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.refProduit";
                        //sqlStr = "select year(tblCommandeFournisseur.DDechargement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnneeNb.ToString()].Name + "]"
                        //    + "     , month(tblCommandeFournisseur.DDechargement) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MoisNb.ToString()].Name + "]"
                        //    + "     , fournisseur.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                        //    + "     , fournisseur.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]"
                        //    + "     , tblCommandeFournisseur.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseVille.ToString()].Name + "]"
                        //    + "     , tblProduit.NomCommun as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                        //    + "     , NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                        //    + "     , NumeroAffretement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroAffretement.ToString()].Name + "]"
                        //    + "     , tblCommandeFournisseur.D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "]"
                        //    + "     , tbrCamionType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CamionTypeLibelle.ToString()].Name + "]"
                        //    + "     , tblCommandeFournisseur.DChargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDChargement.ToString()].Name + "]"
                        //    + "     , tblCommandeFournisseur.DDechargement as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDechargement.ToString()].Name + "]"
                        //    + "     , client.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                        //    + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysLibelle.ToString()].Name + "]"
                        //    + " 	, univers.Poids as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurPoidsReparti.ToString()].Name + "]"
                        //    + "     , univers.PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHT.ToString()].Name + "]"
                        //    + " from"
                        //    + " 	( select u.RefCommandeFournisseur, u.RefProcess, PUHT, u.PUHTSurtri, u.PUHTTransport, sum(u.Poids) as Poids"
                        //    + " 		from"
                        //    + " 		("
                        //    + " 	        ( select rep.RefCommandeFournisseur, rep.RefProduit, tblRepartitionCollectivite.RefProcess, rep.D, Poids"
                        //    + " 		    , tblRepartitionCollectivite.PUHT+tbrPrixReprise.PUHTSurtri+tbrPrixReprise.PUHTTransport as PUHT, tbrPrixReprise.PUHTSurtri, tbrPrixReprise.PUHTTransport"
                        //    + " 		    from"
                        //    + " 	            ("
                        //    + " 		            select RefRepartition, tblCommandeFournisseur.RefCommandeFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D "
                        //    + " 		            from tblRepartition "
                        //    + " 			            inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        //    + " 	            ) as rep"
                        //    + " 	            inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition"
                        //    + " 	            inner join tbrPrixReprise on tbrPrixReprise.RefProcess=tblRepartitionCollectivite.RefProcess and tbrPrixReprise.RefComposant=tblRepartitionCollectivite.RefProduit and tbrPrixReprise.RefProduit=rep.RefProduit"
                        //    + " 		            and month(rep.D)=month(tbrPrixReprise.D) and year(rep.D)=year(tbrPrixReprise.D)"
                        //    + "             where rep.D between @begin and @end"
                        //    + "                 and tblRepartitionCollectivite.RefCollectivite in (" + filterCollectivites + ")"
                        //    + " 		        )"
                        //    + " 		    union all"
                        //    + " 	        ( select rep.RefCommandeFournisseur, rep.RefProduit, tblRepartitionProduit.RefProcess, rep.D, Poids"
                        //    + " 		    , tblRepartitionProduit.PUHT+tbrPrixReprise.PUHTSurtri+tbrPrixReprise.PUHTTransport as PUHT, tbrPrixReprise.PUHTSurtri, tbrPrixReprise.PUHTTransport"
                        //    + " 		    from"
                        //    + " 	            ("
                        //    + " 		            select RefRepartition, tblCommandeFournisseur.RefCommandeFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D "
                        //    + " 		            from tblRepartition "
                        //    + " 			            inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        //    + " 	            ) as rep"
                        //    + " 	            inner join tblRepartitionProduit on tblRepartitionProduit.RefRepartition=rep.RefRepartition"
                        //    + " 	            inner join tbrPrixReprise on tbrPrixReprise.RefProcess=tblRepartitionProduit.RefProcess and tbrPrixReprise.RefComposant=tblRepartitionProduit.RefProduit and tbrPrixReprise.RefProduit=rep.RefProduit"
                        //    + " 		            and month(rep.D)=month(tbrPrixReprise.D) and year(rep.D)=year(tbrPrixReprise.D)"
                        //    + "             where rep.D between @begin and @end"
                        //    + "                 and tblRepartitionProduit.RefFournisseur in (" + filterCollectivites + ")"
                        //    + " 		    )"
                        //    + "         ) as u"
                        //    + "         group by u.RefCommandeFournisseur, u.RefProcess, PUHT, u.PUHTSurtri, u.PUHTTransport"
                        //    + "      )"
                        //    + " 	 as univers"
                        //    + "         inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=univers.RefCommandeFournisseur"
                        //    + " 	    inner join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                        //    + " 	    inner join tblAdresse on tblAdresse.RefAdresse=tblCommandeFournisseur.RefAdresseClient"
                        //    + " 	    inner join tbrPays on tblAdresse.RefPays=tbrPays.RefPays"
                        //    + " 	    inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        //    + " 	    inner join tbrCamionType on tblCommandeFournisseur.RefCamionType=tbrCamionType.refCamionType"
                        //    + " 	    inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.refProduit"
                        //    + " 	    inner join tbrProcess on univers.RefProcess=tbrProcess.RefProcess";
                        //Filters
                        if (eSF.FilterCollecte == "Collecte")
                        {
                            sqlStr += " and tblProduit.Collecte=1";
                        }
                        else if (eSF.FilterCollecte == "HorsCollecte")
                        {
                            sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                        }
                        sqlStrCount = sqlStr;
                        break;
                    case "ExtractionRSE":
                        s = "";
                        //Fields
                        var eqs = DbContext.EquivalentCO2s.Where(e => e.Ratio != null).OrderBy(o => o.Ordre).AsNoTracking().ToList();
                        foreach(var eq in eqs)
                        {
                            s += ", cast(round((sum(Poids * cast(isnull(tblProduit.Co2KgParT,0) as bigint))/1000)*" + eq.Ratio.ToString().Replace(",",".") + ",0) as int) as [" + eq.Libelle + "]";
                        }
                        //Base SQL statement
                        sqlStr = "select cast(YEAR(D) as char(4)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnneeTexte.ToString()].Name + "]"
                            + "     , MONTH(D) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MoisNb.ToString()].Name + "]"
                            + "     , cast(round(cast(SUM(Poids) as decimal(20,3))/1000,0) as int)  as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsTonne.ToString()].Name + "]"
                            + "     , cast(round(sum(Poids * cast(isnull(tblProduit.Co2KgParT,0) as bigint))/1000000,0) as int) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EconomieCO2KgT.ToString()].Name + "]"
                            + s
                            + " from VueRepartitionDetail"
                            + "     inner join tblProduit on tblProduit.RefProduit=VueRepartitionDetail.RefProduit"
                            + " where RefCollectivite in (" + filterCollectivites + ") and tblProduit.Collecte=1 and YEAR(D)>=2023"
                            + "     and D between @begin and @end"
                            + " group by YEAR(D), MONTH(D)";
                        sqlStrCount = sqlStr;
                        break;
                    case "ExtractionPrixReprise":
                        //Base SQL statement
                        sqlStr = "select distinct year(univers.D) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AnneeNb.ToString()].Name + "]"
                            + "     , month(univers.D) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MoisNb.ToString()].Name + "]"
                            + "     , tbrStandard.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.StandardLibelle.ToString()].Name + "]"
                            + "     , tblProduit.NomCommun as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                            + "     , univers.PUHT as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PrixReprisePUHT.ToString()].Name + "]"
                            + " from"
                            + " 	( select isnull(u.RefProduit, tbrPrixReprise.RefProduit) as RefProduit, isnull(u.RefProcess, tbrPrixReprise.RefProcess) as RefProcess"
                            + "         , isnull(u.RefComposant, tbrPrixReprise.RefComposant) as RefComposant, isnull(u.D, tbrPrixReprise.D) as D"
                            + "         , isnull(u.PUHT, tbrPrixReprise.PUHT) as PUHT, isnull(u.PUHTSurtri, tbrPrixReprise.PUHTSurtri) as PUHTSurtri"
                            + "         , isnull(u.PUHTTransport, tbrPrixReprise.PUHTTransport) as PUHTTransport"
                            + "         , tbrPrixReprise.RefStandard"
                            + " 		from"
                            + " 		("
                            + " 	        ( select rep.RefProduit, tblRepartitionCollectivite.RefProcess, tblRepartitionCollectivite.RefProduit as RefComposant, rep.D"
                            + " 		    , tblRepartitionCollectivite.PUHT+isnull(tbrPrixReprise.PUHTSurtri,0)+isnull(tbrPrixReprise.PUHTTransport,0) as PUHT, tbrPrixReprise.PUHTSurtri, tbrPrixReprise.PUHTTransport"
                            + " 		    from"
                            + " 		    ("
                            + " 				select RefRepartition, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                            + " 				from tblRepartition"
                            + " 					inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                            + " 				where tblCommandeFournisseur.DDechargement between @begin and @end"
                            + "             ) as rep"
                            + " 			inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition"
                            + " 			left join tbrPrixReprise on tbrPrixReprise.RefProcess=tblRepartitionCollectivite.RefProcess and tbrPrixReprise.RefComposant=tblRepartitionCollectivite.RefProduit and tbrPrixReprise.RefProduit=rep.RefProduit"
                            + " 					and month(rep.D)=month(tbrPrixReprise.D) and year(rep.D)=year(tbrPrixReprise.D)"
                            + " 		    where tblRepartitionCollectivite.RefCollectivite in (" + filterCollectivites + ")"
                            + " 		    )"
                            + " 		union all"
                            + " 	        ( select rep.RefProduit, tblRepartitionProduit.RefProcess, tblRepartitionProduit.RefProduit as RefComposant, rep.D"
                            + " 		    , tblRepartitionProduit.PUHT+isnull(tbrPrixReprise.PUHTSurtri,0)+isnull(tbrPrixReprise.PUHTTransport,0) as PUHT, tbrPrixReprise.PUHTSurtri, tbrPrixReprise.PUHTTransport"
                            + " 		    from"
                            + " 		    ("
                            + " 				select RefRepartition, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                            + " 				from tblRepartition"
                            + " 					inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                            + " 				where tblCommandeFournisseur.DDechargement between @begin and @end"
                            + "             ) as rep"
                            + " 			inner join tblRepartitionProduit on tblRepartitionProduit.RefRepartition=rep.RefRepartition"
                            + " 			left join tbrPrixReprise on tbrPrixReprise.RefProcess=tblRepartitionProduit.RefProcess and tbrPrixReprise.RefComposant=tblRepartitionProduit.RefProduit and tbrPrixReprise.RefProduit=rep.RefProduit"
                            + " 					and month(rep.D)=month(tbrPrixReprise.D) and year(rep.D)=year(tbrPrixReprise.D)"
                            + " 		    where tblRepartitionProduit.RefFournisseur in (" + filterCollectivites + ")"
                            + " 		    )"
                            + "         ) as u"
                            + "         full outer join (select distinct tbrPrixReprise.*, tbmEntiteStandard.RefStandard from tbmEntiteStandard"
                            + "             inner join tbmProduitStandard on tbmEntiteStandard.RefStandard = tbmProduitStandard.RefStandard"
                            + "             inner join tbrPrixReprise on tbmProduitStandard.RefProduit = tbrPrixReprise.RefProduit"
                            + "             where tbmEntiteStandard.RefEntite in (" + filterCollectivites + ") and D between @begin and @end";
                        //Contrat RI
                        if (filterCollectivites.Split(',').Length == 1)
                        {
                            int fC = System.Convert.ToInt32(eSF.FilterCollectivites.Split(',')[0]);
                            if (_dbContext.ContratEntites.Where(e => e.RefEntite == fC).Count() > 0)
                            {
                                sqlStr += " and tbrPrixReprise.RefContrat is not null";
                            }
                            else
                            {
                                sqlStr += " and tbrPrixReprise.RefContrat is null";
                            }
                        }
                        sqlStr += "             ) as tbrPrixReprise on tbrPrixReprise.RefProcess=u.RefProcess and tbrPrixReprise.RefComposant=u.RefComposant and tbrPrixReprise.RefProduit=u.RefProduit and tbrPrixReprise.D=u.D"
                            + "      ) as univers"
                            + " 	inner join tblProduit on univers.RefProduit=tblProduit.refProduit"
                            + " 	left join tblProduit as composant on composant.RefProduit=univers.RefComposant"
                            + " 	left join tbrProcess on tbrProcess.RefProcess=univers.RefProcess"
                            + "     inner join tbrStandard on univers.RefStandard = tbrStandard.RefStandard"
                            + " where 1=1";

                        //Filters
                        if (eSF.FilterCollecte == "Collecte")
                        {
                            sqlStr += " and tblProduit.Collecte=1";
                        }
                        else if (eSF.FilterCollecte == "HorsCollecte")
                        {
                            sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                        }
                        if (eSF.FilterProduits != "")
                        {
                            sqlStr += " and tblProduit.RefProduit in (" + filterProduits + ")";
                        }
                        if (eSF.FilterProcesss != "")
                        {
                            sqlStr += " and tbrProcess.RefProcess in (";
                            sqlStr += Utils.Utils.CreateSQLParametersFromString("refProcess", eSF.FilterProcesss, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                            sqlStr += ")";
                        }
                        //Group by
                        sqlStrCount = sqlStr;
                        break;
                    case "DestinationTonnage":
                        //Base SQL statement
                        sqlStr = "select (tblEntite.Libelle + case when tbrPays.Libelle is not null then ' - ' + tbrPays.Libelle + case when tbrPays.RefPays=1 then ' ('+LEFT(tblAdresse.CodePostal,2) +')' else '' end else '' end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                            + "     , cast(sum(Poids) as decimal(18,5)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Pourcentage.ToString()].Name + "]"
                            + "     , tbrPays.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysLibelle.ToString()].Name + "]"
                            + " from"
                            + " 	(select VueRepartitionDetail.RefCollectivite as RefCollectivite, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement) as y, month(tblCommandeFournisseur.DDechargement) as m, tblAdresse.RefEntite as RefClient, tblAdresse.RefAdresse, sum(Poids)as Poids"
                            + " 	from tblRepartition "
                            + " 		inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                            + " 		inner join VueRepartitionDetail on VueRepartitionDetail.RefRepartition=tblRepartition.RefRepartition"
                            + " 		inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                            + " 		inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                            + " 	where VueRepartitionDetail.RefCollectivite in (" + filterCollectivites + ") and tblCommandeFournisseur.DDechargement between @begin and @end"
                            + " 	group by VueRepartitionDetail.RefCollectivite, tblCommandeFournisseur.RefEntite, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement), tblAdresse.RefEntite, tblAdresse.RefAdresse"
                            + " 	union all"
                            + " 	select mensuel.RefCollectivite, mensuel.RefFournisseur, mensuel.RefProduit, mensuel.y, mensuel.m, r.RefClient, r.RefAdresse, cast(cast(mensuel.Poids as float)*ratio as int) as Poids"
                            + " 	from"
                            + " 		(select VueRepartitionDetail.RefCollectivite as RefCollectivite, RefFournisseur, tblRepartition.RefProduit, year(VueRepartitionDetail.D) as y, month(VueRepartitionDetail.D) as m, sum(Poids) as Poids  "
                            + " 		from tblRepartition"
                            + " 			inner join VueRepartitionDetail on VueRepartitionDetail.RefRepartition=tblRepartition.RefRepartition"
                            + " 			inner join tblProduit on tblRepartition.RefProduit=tblProduit.RefProduit"
                            + " 		where VueRepartitionDetail.RefCollectivite in (" + filterCollectivites + ") and RefCommandeFournisseur is null and VueRepartitionDetail.D between @begin and @end"
                            + " 		group by VueRepartitionDetail.RefCollectivite, RefFournisseur, tblRepartition.RefProduit, year(VueRepartitionDetail.D), month(VueRepartitionDetail.D)) as mensuel"
                            + " 		inner join"
                            + " 		(select detail.RefFournisseur, detail.RefProduit, detail.y, detail.m, detail.RefClient, detail.RefAdresse, cast(detail.Poids as float)/cast(univers.Poids as float) as ratio"
                            + " 		from"
                            + " 			(select tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement) as y, month(tblCommandeFournisseur.DDechargement) as m, tblAdresse.RefEntite as RefClient, tblAdresse.RefAdresse, sum(PoidsDechargement) as Poids"
                            + " 			from tblCommandeFournisseur "
                            + " 				inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                            + " 				inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                            + " 				inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                            + " 			where tblCommandeFournisseur.DDechargement between @begin and @end"
                            + " 			group by tblCommandeFournisseur.RefEntite, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement), tblAdresse.RefEntite, tblAdresse.RefAdresse) as detail"
                            + " 			inner join "
                            + " 			(select tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement) as y, month(tblCommandeFournisseur.DDechargement) as m, sum(PoidsDechargement) as Poids"
                            + " 			from tblCommandeFournisseur "
                            + " 				inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                            + " 				inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                            + " 				inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                            + " 			where tblCommandeFournisseur.DDechargement between @begin and @end"
                            + " 			group by tblCommandeFournisseur.RefEntite, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement)) as univers"
                            + " 			on detail.RefFournisseur=univers.RefFournisseur and detail.RefProduit=univers.RefProduit and detail.y=univers.y and detail.m=univers.m) as r"
                            + " 			on mensuel.RefFournisseur=r.RefFournisseur and mensuel.RefProduit=r.RefProduit and mensuel.y=r.y and mensuel.m=r.m) as u"
                            + " 	inner join tblEntite on tblEntite.RefEntite=u.RefClient	"
                            + " 	inner join tblAdresse on tblAdresse.RefAdresse=u.RefAdresse	"
                            + " 	left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays	"
                            + "     inner join tblProduit on u.RefProduit=tblProduit.refProduit"
                            + " where RefCollectivite in (" + filterCollectivites + ")";
                        //Filters
                        if (eSF.FilterCollecte == "Collecte")
                        {
                            sqlStr += " and tblProduit.Collecte=1";
                        }
                        else if (eSF.FilterCollecte == "HorsCollecte")
                        {
                            sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                        }
                        if (eSF.FilterCentreDeTris != "")
                        {
                            sqlStr += " and u.RefFournisseur in (";
                            sqlStr += Utils.Utils.CreateSQLParametersFromString("refFournisseur", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                            sqlStr += ")";
                        }
                        //Group by
                        sqlStr += " group by tbrPays.Libelle, tblEntite.Libelle + case when tbrPays.Libelle is not null then ' - ' + tbrPays.Libelle + case when tbrPays.RefPays=1 then ' ('+LEFT(tblAdresse.CodePostal,2) +')' else '' end else '' end"
                            + " having sum(Poids)!=0";
                        sqlStrCount = sqlStr;
                        sqlStr = "select [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                            + "     , [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Pourcentage.ToString()].Name + "]"
                            + " from ("
                            + sqlStr
                            + ") as u"
                            + " ORDER BY [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysLibelle.ToString()].Name + "], [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "]"
                            ;
                        break;
                    case "DestinationTonnageParPays":
                        //Base SQL statement
                        sqlStr = "select (case when tbrPays.Libelle is null then 'NA' else tbrPays.LibelleCourt end) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysCodeISO.ToString()].Name + "]"
                            + "     , cast(sum(Poids) as decimal(18,5)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsTonne.ToString()].Name + "]"
                            + "     , cast(sum(Poids * cast(isnull(tblProduit.Co2KgParT,0) as bigint))/1000 as int) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsKg.ToString()].Name + "]"
                            + " from"
                            + " 	(select VueRepartitionDetail.RefCollectivite as RefCollectivite, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement) as y, month(tblCommandeFournisseur.DDechargement) as m, tblAdresse.RefEntite as RefClient, tblAdresse.RefAdresse, sum(Poids)as Poids"
                            + " 	from tblRepartition "
                            + " 		inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                            + " 		inner join VueRepartitionDetail on VueRepartitionDetail.RefRepartition=tblRepartition.RefRepartition"
                            + " 		inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                            + " 		inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                            + " 	where VueRepartitionDetail.RefCollectivite in (" + filterCollectivites + ") and tblCommandeFournisseur.DDechargement between @begin and @end"
                            + " 	group by VueRepartitionDetail.RefCollectivite, tblCommandeFournisseur.RefEntite, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement), tblAdresse.RefEntite, tblAdresse.RefAdresse"
                            + " 	union all"
                            + " 	select mensuel.RefCollectivite, mensuel.RefFournisseur, mensuel.RefProduit, mensuel.y, mensuel.m, r.RefClient, r.RefAdresse, cast(cast(mensuel.Poids as float)*ratio as int) as Poids"
                            + " 	from"
                            + " 		(select VueRepartitionDetail.RefCollectivite as RefCollectivite, RefFournisseur, tblRepartition.RefProduit, year(VueRepartitionDetail.D) as y, month(VueRepartitionDetail.D) as m, sum(Poids) as Poids  "
                            + " 		from tblRepartition"
                            + " 			inner join VueRepartitionDetail on VueRepartitionDetail.RefRepartition=tblRepartition.RefRepartition"
                            + " 			inner join tblProduit on tblRepartition.RefProduit=tblProduit.RefProduit"
                            + " 		where VueRepartitionDetail.RefCollectivite in (" + filterCollectivites + ") and RefCommandeFournisseur is null and VueRepartitionDetail.D between @begin and @end"
                            + " 		group by VueRepartitionDetail.RefCollectivite, RefFournisseur, tblRepartition.RefProduit, year(VueRepartitionDetail.D), month(VueRepartitionDetail.D)) as mensuel"
                            + " 		inner join"
                            + " 		(select detail.RefFournisseur, detail.RefProduit, detail.y, detail.m, detail.RefClient, detail.RefAdresse, cast(detail.Poids as float)/cast(univers.Poids as float) as ratio"
                            + " 		from"
                            + " 			(select tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement) as y, month(tblCommandeFournisseur.DDechargement) as m, tblAdresse.RefEntite as RefClient, tblAdresse.RefAdresse, sum(PoidsDechargement) as Poids"
                            + " 			from tblCommandeFournisseur "
                            + " 				inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                            + " 				inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                            + " 				inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                            + " 			where tblCommandeFournisseur.DDechargement between @begin and @end"
                            + " 			group by tblCommandeFournisseur.RefEntite, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement), tblAdresse.RefEntite, tblAdresse.RefAdresse) as detail"
                            + " 			inner join "
                            + " 			(select tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement) as y, month(tblCommandeFournisseur.DDechargement) as m, sum(PoidsDechargement) as Poids"
                            + " 			from tblCommandeFournisseur "
                            + " 				inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                            + " 				inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                            + " 				inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                            + " 			where tblCommandeFournisseur.DDechargement between @begin and @end"
                            + " 			group by tblCommandeFournisseur.RefEntite, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement)) as univers"
                            + " 			on detail.RefFournisseur=univers.RefFournisseur and detail.RefProduit=univers.RefProduit and detail.y=univers.y and detail.m=univers.m) as r"
                            + " 			on mensuel.RefFournisseur=r.RefFournisseur and mensuel.RefProduit=r.RefProduit and mensuel.y=r.y and mensuel.m=r.m) as u"
                            + " 	inner join tblEntite on tblEntite.RefEntite=u.RefClient	"
                            + " 	inner join tblAdresse on tblAdresse.RefAdresse=u.RefAdresse	"
                            + " 	left join tbrPays on tblAdresse.RefPays=tbrPays.RefPays	"
                            + "     inner join tblProduit on u.RefProduit=tblProduit.refProduit"
                            + " where RefCollectivite in (" + filterCollectivites + ")";
                        //Filters
                        if (eSF.FilterCollecte == "Collecte")
                        {
                            sqlStr += " and tblProduit.Collecte=1";
                        }
                        else if (eSF.FilterCollecte == "HorsCollecte")
                        {
                            sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                        }
                        //Group by
                        sqlStr += " group by (case when tbrPays.Libelle is null then 'NA' else tbrPays.LibelleCourt end)"
                            + " having sum(Poids)!=0";
                        sqlStrCount = sqlStr;
                        break;
                    case "DeboucheBalle":
                        //Base SQL statement
                        sqlStr = "select min(tblApplication.Libelle) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ApplicationLibelle.ToString()].Name + "]"
                            + "     , cast(sum(cast(Poids as decimal)*Ratio/100) as decimal(18,5)) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Pourcentage.ToString()].Name + "]"
                            + " from"
                            + " 	(select VueRepartitionDetail.RefCollectivite as RefCollectivite, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement) as y, month(tblCommandeFournisseur.DDechargement) as m, tblAdresse.RefEntite as RefClient, sum(Poids)as Poids"
                            + " 	from tblRepartition "
                            + " 		inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                            + " 		inner join VueRepartitionDetail on VueRepartitionDetail.RefRepartition=tblRepartition.RefRepartition"
                            + " 		inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                            + " 		inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                            + " 	where VueRepartitionDetail.RefCollectivite in (" + filterCollectivites + ") and year(tblCommandeFournisseur.DDechargement)=@year"
                            + " 	group by VueRepartitionDetail.RefCollectivite, tblCommandeFournisseur.RefEntite, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement), tblAdresse.RefEntite"
                            + " 	union all"
                            + " 	select mensuel.RefCollectivite, mensuel.RefFournisseur, mensuel.RefProduit, mensuel.y, mensuel.m, r.RefClient, cast(cast(mensuel.Poids as float)*ratio as int) as Poids"
                            + " 	from"
                            + " 		(select VueRepartitionDetail.RefCollectivite as RefCollectivite, RefFournisseur, tblRepartition.RefProduit, year(VueRepartitionDetail.D) as y, month(VueRepartitionDetail.D) as m, sum(Poids) as Poids  "
                            + " 		from tblRepartition"
                            + " 			inner join VueRepartitionDetail on VueRepartitionDetail.RefRepartition=tblRepartition.RefRepartition"
                            + " 			inner join tblProduit on tblRepartition.RefProduit=tblProduit.RefProduit"
                            + " 		where VueRepartitionDetail.RefCollectivite in (" + filterCollectivites + ") and RefCommandeFournisseur is null and year(VueRepartitionDetail.D)=@year"
                            + " 		group by VueRepartitionDetail.RefCollectivite, RefFournisseur, tblRepartition.RefProduit, year(VueRepartitionDetail.D), month(VueRepartitionDetail.D)) as mensuel"
                            + " 		inner join"
                            + " 		(select detail.RefFournisseur, detail.RefProduit, detail.y, detail.m, detail.RefClient, cast(detail.Poids as float)/cast(univers.Poids as float) as ratio"
                            + " 		from"
                            + " 			(select tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit as RefProduit, year(tblCommandeFournisseur.DDechargement) as y, month(tblCommandeFournisseur.DDechargement) as m, tblAdresse.RefEntite as RefClient, sum(PoidsDechargement) as Poids"
                            + " 			from tblCommandeFournisseur "
                            + " 				inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                            + " 				inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                            + " 				inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                            + " 			where year(tblCommandeFournisseur.DDechargement)=@year"
                            + " 			group by tblCommandeFournisseur.RefEntite, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement), tblAdresse.RefEntite) as detail"
                            + " 			inner join "
                            + " 			(select tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit as RefProduit, year(tblCommandeFournisseur.DDechargement) as y, month(tblCommandeFournisseur.DDechargement) as m, sum(PoidsDechargement) as Poids"
                            + " 			from tblCommandeFournisseur "
                            + " 				inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                            + " 				inner join tblEntite as client on tblAdresse.RefEntite=client.RefEntite"
                            + " 				inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.RefProduit"
                            + " 			where year(tblCommandeFournisseur.DDechargement)=@year"
                            + " 			group by tblCommandeFournisseur.RefEntite, tblCommandeFournisseur.RefProduit, year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement)"
                            + "             ) as univers"
                            + " 			on detail.RefFournisseur=univers.RefFournisseur and detail.RefProduit=univers.RefProduit and detail.y=univers.y and detail.m=univers.m) as r"
                            + " 			on mensuel.RefFournisseur=r.RefFournisseur and mensuel.RefProduit=r.RefProduit and mensuel.y=r.y and mensuel.m=r.m"
                            + "         ) as u"
                            + " 	inner join tblEntite on tblEntite.RefEntite=u.RefClient	"
                            + " 	inner join tblProduit on u.RefProduit=tblProduit.RefProduit"
                            + " 	inner join tblApplicationProduitOrigine on tblProduit.RefApplicationProduitOrigine=tblApplicationProduitOrigine.RefApplicationProduitOrigine"
                            + " 	inner join tblClientApplication on year(tblClientApplication.D)=u.y "
                            + " 		and tblClientApplication.RefApplicationProduitOrigine=tblProduit.RefApplicationProduitOrigine"
                            + " 		and tblClientApplication.RefEntite=u.RefClient"
                            + " 	inner join tblClientApplicationApplication on tblClientApplication.RefClientApplication=tblClientApplicationApplication.RefClientApplication"
                            + " 	inner join tblApplication on tblClientApplicationApplication.RefApplication=tblApplication.RefApplication"
                            + " where RefCollectivite in (" + filterCollectivites + ") and y=@year";
                        //Filters
                        if (eSF.FilterCollecte == "Collecte")
                        {
                            sqlStr += " and tblProduit.Collecte=1";
                        }
                        else if (eSF.FilterCollecte == "HorsCollecte")
                        {
                            sqlStr += " and isnull(tblProduit.Collecte,0)=0";
                        }
                        if (eSF.FilterCentreDeTris != "")
                        {
                            sqlStr += " and u.RefFournisseur in (";
                            sqlStr += Utils.Utils.CreateSQLParametersFromString("refFournisseur", eSF.FilterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                            sqlStr += ")";
                        }
                        //Group by
                        sqlStr += " group by tblClientApplicationApplication.RefApplication"
                            + " having cast(sum(cast(Poids as decimal)*Ratio/100) as int)!=0";
                        sqlStrCount = sqlStr;
                        break;
                    case "EvolutionTonnage":
                        //First command for fields
                        sqlStr = "select distinct tblProduit.NomCommun"
                            + " from tblRepartition"
                            + "     left join VueRepartitionDetail on tblRepartition.RefRepartition=VueRepartitionDetail.RefRepartition"
                            + "     left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                            + "     inner join tblProduit on VueRepartitionDetail.RefProduit=tblProduit.refProduit"
                            + " where isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement) between @begin and @end"
                            + "     and  VueRepartitionDetail.RefCollectivite in (" + filterCollectivites + ")";
                        //Filters
                        if (eSF.FilterProduits != "")
                        {
                            sqlStr += " and VueRepartitionDetail.RefProduit in (" + filterProduits + ")";
                        }
                        if (eSF.FilterCollecte == "Collecte")
                        {
                            sqlStr += " and Collecte=1";
                        }
                        else if (eSF.FilterCollecte == "HorsCollecte")
                        {
                            sqlStr += " and isnull(Collecte,0)=0";
                        }
                        //Order
                        sqlStr += " order by tblProduit.NomCommun";
                        using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                        {
                            sqlConn.Open();
                            cmd.Connection = sqlConn;
                            cmd.CommandText = sqlStr;
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                s += ", [" + dr.GetSqlString(0).ToString() + "]";
                                s1 += ", cast(sum(case when tblProduit.NomCommun='" + dr.GetSqlString(0).ToString().Replace("'", "''") + "'"
                                    + " then VueRepartitionDetail.Poids else 0 end) as decimal(15,3))/1000"
                                    + " as [" + dr.GetSqlString(0).ToString() + "]";
                            }
                            dr.Close();
                        }
                        //Main query
                        sqlStr = "select cast(month(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement)) as nvarchar) + '/' + cast(year(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement)) as nvarchar) as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MoisAnnee.ToString()].Name + "]";
                        if (eSF.FilterProduits != "")
                        {
                            sqlStr += s1;
                        }
                        else
                        {
                            sqlStr += "     , cast(sum(VueRepartitionDetail.Poids) as decimal(15,3))/1000 as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsTonne.ToString()].Name + "]";
                        }
                        sqlStr += "     from tblRepartition"
                            + "     left join VueRepartitionDetail on tblRepartition.RefRepartition=VueRepartitionDetail.RefRepartition"
                            + "     left join tblCommandeFournisseur on tblRepartition.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                            + "     inner join tblProduit on VueRepartitionDetail.RefProduit=tblProduit.refProduit"
                            + " where isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement) between @begin and @end"
                            + "     and  VueRepartitionDetail.RefCollectivite in (" + filterCollectivites + ")";
                        //Filters
                        if (eSF.FilterProduits != "")
                        {
                            sqlStr += " and VueRepartitionDetail.RefProduit in (" + filterProduits + ")";
                        }
                        if (eSF.FilterCollecte == "Collecte")
                        {
                            sqlStr += " and Collecte=1";
                        }
                        else if (eSF.FilterCollecte == "HorsCollecte")
                        {
                            sqlStr += " and isnull(Collecte,0)=0";
                        }
                        sqlStr += " group by year(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement)), month(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement))";
                        //Count
                        sqlStrCount = sqlStr;
                        break;
                }
            }
            //Chargement des données si elles existent
            if (sqlStr != "")
            {
                int nbRow = 0;
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    //Set timeout for stats
                    cmd.CommandTimeout = 120;
                    //Get the total row count
                    cmd.CommandText = "select count(*) from (" + sqlStrCount + ") as univers OPTION ( QUERYRULEOFF JoinCommute )";
                    nbRow = (int)cmd.ExecuteScalar();
                    sqlConn.Close();
                    sqlConn.Open();
                    //Get formatted data page
                    sqlStrFinal = " set language French; " + sqlStr;
                    if (menu == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString())
                    {
                        switch (eSF.FilterDayWeekMonth)
                        {
                            case "Day":
                                sqlStrFinal += "order by CONVERT(nvarchar, D, 23)";
                                break;
                            case "Week":
                                sqlStrFinal += "order by datename(YEAR, D) + '-' + format(datepart(wk, D), '00')";
                                break;
                            case "Month":
                                sqlStrFinal += "order by datename(YEAR, D) + '-' + format(datepart(month, D), '00')";
                                break;
                        }
                    }
                    if (menu == Enumerations.MenuName.AdministrationMenuExtractionLogin.ToString())
                    {
                        sqlStrFinal += " ORDER BY LogLogin.Login, tblUtilisateur.Nom";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuChargementAnnule.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblCommandeFournisseur.RefCommandeFournisseur";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString())
                    {
                        sqlStrFinal += " ORDER BY client.Libelle, transporteur.Libelle, fournisseur.Libelle";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuLotDisponible.ToString())
                    {
                        sqlStrFinal += " ORDER BY case when tblEntite.CodeEE is not null then tblEntite.CodeEE + ' - ' else '' end + tblEntite.Libelle, dbo.ListeDR(tblEntite.RefEntite), tblProduit.Libelle";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatKmMoyen.ToString())
                    {
                        sqlStrFinal += " ORDER BY min(tblProduit.Libelle)";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatReceptionFournisseurChargement.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblCommandeFournisseur.RefProduit, DChargement";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatReception.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblCommandeFournisseur.RefProduit, DChargement";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblEntite.Libelle, commandeClient.RefAdresse, tblProduit.Libelle";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClientProduit.ToString())
                    {
                        sqlStrFinal += " ORDER BY isnull(tblProduit.Libelle,produitFournisseur.Libelle)";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatTonnageParProcess.ToString())
                    {
                        sqlStrFinal += " ORDER BY tbrProcess.Libelle, composant.NomCommun, tblProduit.NomCommun";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString())
                    {
                        sqlStrFinal += " ORDER BY dbo.ListeDR(tblEntite.RefEntite), tblEntite.CodeEE, D";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatDesEnlevements.ToString())
                    {
                        sqlStrFinal += " ORDER BY DChargement";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblProduit.Libelle, DChargement";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatReceptionEmballagePlastique.ToString())
                    {
                        sqlStrFinal += " ORDER BY year(VueRepartitionUnitaireDetail.D), 'T'+cast(datepart(quarter,VueRepartitionUnitaireDetail.D) as nvarchar(1)), month(VueRepartitionUnitaireDetail.D), collectivite.CodeEE";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString())
                    {
                        sqlStrFinal += " ORDER BY GroupeType, Groupe, case when Client='TSR/Ecart de pesée' then 'zzzzzz' else Client end, Pays";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
                    {
                        sqlStrFinal += " ORDER BY univers.GroupeType, univers.Groupe, Region, Pays";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuSuiviFacturationHC.ToString())
                    {
                        sqlStrFinal += " ORDER BY isnull(c.Libelle, isnull(f.Libelle, fournisseur.Libelle)), isnull(p.Libelle, produit.Libelle), cli.Libelle, NumeroCommande";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString())
                    {
                        sqlStrFinal += " ORDER BY Trimestre, Fournisseur";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuTonnageCLSousContrat.ToString())
                    {
                        sqlStrFinal += " ORDER BY dbo.ListeDR(tblEntite.RefEntite), tblEntite.CodeEE, univers.RefProduit";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblCommandeFournisseur.DDechargement, tbrRegionEE.Libelle, fournisseur.CodeEE, client.Libelle, tblProduit.NomCommun";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatDesFluxDev.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblCommandeFournisseur.NumeroCommande";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatDesFluxDevLeko.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblCommandeFournisseur.NumeroCommande";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuListeProduit.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblProduit.RefProduit";
                    }
                    if (menu == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString())
                    {
                        sqlStrFinal += " ORDER BY client.Libelle, year(DDechargement), month(DDechargement)";
                    }
                    if (menu == Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString())
                    {
                        sqlStrFinal += " ORDER BY DDechargement desc";
                    }
                    if (menu == Enumerations.MenuName.QualiteMenuExtractionControle.ToString())
                    {
                        sqlStrFinal += " ORDER BY client.Libelle, year(DDechargement), month(DDechargement)";
                    }
                    if (menu == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString())
                    {
                        sqlStrFinal += " ORDER BY client.Libelle, year(DDechargement), month(DDechargement)";
                    }
                    if (menu == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString())
                    {
                        sqlStrFinal += " ORDER BY client.Libelle, year(DDechargement), month(DDechargement)";
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString())
                    {
                        sqlStrFinal += " ORDER BY tblCommandeFournisseur.D";
                    }
                    if (menu == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString())
                    {
                        sqlStrFinal += " ORDER BY cast(case when sousContrat.RefEntite is not null then 1 else 0 end as bit) desc, dbo.ListeDR(tblEntite.RefEntite), tblEntite.Libelle OPTION( QUERYRULEOFF JoinCommute )";
                    }
                    if (menu == Enumerations.MenuName.QualiteMenuEtatControleReception.ToString())
                    {
                        sqlStrFinal += " ORDER BY client.Libelle, year(DDechargement), month(DDechargement) ";
                    }
                    if (menu == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString())
                    {
                        sqlStrFinal += " ORDER BY " + (string.IsNullOrWhiteSpace(filterContactSelectedColumns) ? "Erreur" : filterContactSelectedColumns.Split(",")[0]);
                    }
                    if (menu == Enumerations.MenuName.AnnuaireMenuNbAffretementTransporteur.ToString())
                    {
                        sqlStrFinal += " ORDER BY year(tblCommandeFournisseur.DDechargement), month(tblCommandeFournisseur.DDechargement)";
                        if (eSF.FilterPayss != "")
                        {
                            //sqlStr += ", tbrPays.Libelle";
                        }
                    }
                    if (menu == Enumerations.MenuName.AnnuaireMenuSuiviEnvois.ToString())
                    {
                        if (eSF.FilterEmailType == "IncitationQualite")
                        {
                            sqlStrFinal += " ORDER BY tblEmailIncitationQualite.DCreation ";
                        }
                        if (eSF.FilterEmailType == "NoteCreditCollectivite")
                        {
                            sqlStrFinal += " ORDER BY tblEmailNoteCredit.DCreation ";
                        }
                    }
                    if (menu == Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString())
                    {
                        sqlStrFinal += " ORDER BY year(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement)), month(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement))";
                    }
                    if (menu == Enumerations.MenuName.ModuleCollectiviteMenuStatistiques.ToString())
                    {
                        switch (eSF.StatType)
                        {
                            case "DonneeCDT":
                                sqlStrFinal += " ORDER BY tblCommandeFournisseur.DDechargement";
                                break;
                            case "ExtractionRSE":
                                sqlStrFinal += " ORDER BY YEAR(D), MONTH(D)";
                                break;
                            case "ExtractionPrixReprise":
                                sqlStrFinal += " ORDER BY year(univers.D), month(univers.D)";
                                break;
                            case "DestinationTonnage":
                                //sqlStrFinal += " ORDER BY tbrPays.Libelle, (tblEntite.Libelle + case when tbrPays.Libelle is not null then ' - ' + tbrPays.Libelle + case when tbrPays.RefPays=1 then ' ('+LEFT(tblAdresse.CodePostal,2) +')' else '' end else '' end)";
                                break;
                            case "EvolutionTonnage":
                                sqlStrFinal += " ORDER BY year(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement)), month(isnull(tblRepartition.D,tblCommandeFournisseur.DDechargement))";
                                break;
                        }
                    }
                    //sqlStrFinal += " ) as univ";
                    //sqlStrFinal += " OFFSET (@pageNumber  * @pageSize) ROWS FETCH NEXT @pageSize ROWS ONLY) as univ";
                    //Get whole data for Excel
                    cmd.CommandText = sqlStrFinal;
                    //cmd.Parameters.Add("@pageNumber", SqlDbType.Int).Value = pageNumber;
                    //cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                    SqlDataAdapter dA = new(cmd);
                    if (dS.Tables.Count > 0) { dS.Tables.Clear(); }
                    dA.Fill(dS);
                }
                //Mise en forme spécifique
                int i = 0;
                if (dS.Tables[0].Rows.Count > 0)
                {
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString())
                    {
                        decimal val1 = 0;
                        decimal val2 = 0;
                        decimal val3 = 0;
                        decimal val4 = 0;
                        int val5 = 0;
                        i = 0;
                        DataRow r;
                        //Transporteur
                        if (CurrentContext.ConnectedUtilisateur.RefTransporteur == null)
                        {
                            //Initialisations
                            val1 = (decimal)dS.Tables[0].Rows[0][8];
                            val2 = (decimal)dS.Tables[0].Rows[0][10];
                            val3 = (decimal)dS.Tables[0].Rows[0][11];
                            val4 = (decimal)dS.Tables[0].Rows[0][12];
                            val5 = (int)dS.Tables[0].Rows[0][13];
                            //Parcours du dataset
                            for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                            {
                                //Ajout aux valeurs
                                val1 += (decimal)dS.Tables[0].Rows[i][8];
                                val2 += (decimal)dS.Tables[0].Rows[i][10];
                                val3 += (decimal)dS.Tables[0].Rows[i][11];
                                val4 += (decimal)dS.Tables[0].Rows[i][12];
                                val5 += (int)dS.Tables[0].Rows[i][13];
                            }
                            //Total final
                            r = dS.Tables[0].NewRow();
                            r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                            r[8] = val1;
                            r[10] = val2;
                            r[11] = val3;
                            r[12] = val4;
                            r[13] = val5;
                            dS.Tables[0].Rows.InsertAt(r, i);
                            nbRow++;
                        }
                        else if (CurrentContext.ConnectedUtilisateur?.Transporteur?.SurcoutCarburant == true)
                        {
                            //Initialisations
                            val1 = (decimal)dS.Tables[0].Rows[0][5];
                            val2 = (decimal)dS.Tables[0].Rows[0][7];
                            val3 = (decimal)dS.Tables[0].Rows[0][8];
                            val4 = (decimal)dS.Tables[0].Rows[0][9];
                            val5 = (int)dS.Tables[0].Rows[0][10];
                            //Parcours du dataset
                            for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                            {
                                //Ajout aux valeurs
                                val1 += (decimal)dS.Tables[0].Rows[i][5];
                                val2 += (decimal)dS.Tables[0].Rows[i][7];
                                val3 += (decimal)dS.Tables[0].Rows[i][8];
                                val4 += (decimal)dS.Tables[0].Rows[i][9];
                                val5 += (int)dS.Tables[0].Rows[i][10];
                            }
                            //Total final
                            r = dS.Tables[0].NewRow();
                            r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                            r[5] = val1;
                            r[7] = val2;
                            r[8] = val3;
                            r[9] = val4;
                            r[10] = val5;
                            dS.Tables[0].Rows.InsertAt(r, i);
                            nbRow++;
                        }
                        else
                        {
                            //Initialisations
                            val1 = (decimal)dS.Tables[0].Rows[0][5];
                            val3 = (decimal)dS.Tables[0].Rows[0][6];
                            val4 = (decimal)dS.Tables[0].Rows[0][7];
                            val5 = (int)dS.Tables[0].Rows[0][8];
                            //Parcours du dataset
                            for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                            {
                                //Ajout aux valeurs
                                val1 += (decimal)dS.Tables[0].Rows[i][5];
                                val3 += (decimal)dS.Tables[0].Rows[i][6];
                                val4 += (decimal)dS.Tables[0].Rows[i][7];
                                val5 += (int)dS.Tables[0].Rows[i][8];
                            }
                            //Total final
                            r = dS.Tables[0].NewRow();
                            r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                            r[5] = val1;
                            r[6] = val3;
                            r[7] = val4;
                            r[8] = val5;
                            dS.Tables[0].Rows.InsertAt(r, i);
                            nbRow++;
                        }
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatKmMoyen.ToString())
                    {
                        long val0 = 0, val1 = 0, val2 = 0;
                        decimal val3 = 0;
                        i = 0;
                        DataRow r;
                        //Initialisations
                        val0 = (int)dS.Tables[0].Rows[0][1];
                        val1 = (int)dS.Tables[0].Rows[0][2];
                        val2 = (int)dS.Tables[0].Rows[0][4];
                        val3 = (decimal)dS.Tables[0].Rows[0][6];
                        //Parcours du dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Ajout aux valeurs
                            val0 += (int)dS.Tables[0].Rows[i][1];
                            val1 += (int)dS.Tables[0].Rows[i][2];
                            val2 += (int)dS.Tables[0].Rows[i][4];
                            val3 += (decimal)dS.Tables[0].Rows[i][6];
                        }
                        //Total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[1] = val0;
                        r[2] = val1;
                        r[3] = val1 / val0;
                        r[4] = val2;
                        r[5] = val2 / val0;
                        r[6] = val3;
                        r[7] = (decimal)((int)((val3 / val0) * 10)) / 10;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString())
                    {
                        long val0 = 0, val1 = 0, val2 = 0, val3 = 0, val4 = 0;
                        long totVal0 = 0, totVal1 = 0, totVal2 = 0, totVal3 = 0, totVal4 = 0;
                        string client = "";
                        i = 0;
                        DataRow r;
                        //Initialisations
                        client = dS.Tables[0].Rows[0][0].ToString();
                        val0 = (int)dS.Tables[0].Rows[0][3];
                        val1 = (int)dS.Tables[0].Rows[0][4];
                        val2 = (int)dS.Tables[0].Rows[0][6];
                        val3 = (int)dS.Tables[0].Rows[0][7];
                        val4 = (int)dS.Tables[0].Rows[0][9];
                        totVal0 = (int)dS.Tables[0].Rows[0][3];
                        totVal1 = (int)dS.Tables[0].Rows[0][4];
                        totVal2 = (int)dS.Tables[0].Rows[0][6];
                        totVal3 = (int)dS.Tables[0].Rows[0][7];
                        totVal4 = (int)dS.Tables[0].Rows[0][9];
                        //Parcours de dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Ajout au total général
                            totVal0 += (int)dS.Tables[0].Rows[i][3];
                            totVal1 += (int)dS.Tables[0].Rows[i][4];
                            totVal2 += (int)dS.Tables[0].Rows[i][6];
                            totVal3 += (int)dS.Tables[0].Rows[i][7];
                            totVal4 += (int)dS.Tables[0].Rows[i][9];
                            //Vérification du changement de regroupement
                            if (client != dS.Tables[0].Rows[i][0].ToString())
                            {
                                //inscription du sous-total
                                r = dS.Tables[0].NewRow();
                                r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                r[3] = val0;
                                r[4] = val1;
                                r[5] = (val1 * 100) / val0;
                                r[6] = val0 - val1;
                                r[7] = val3;
                                r[8] = (val3 * 100) / val0;
                                r[9] = val4;
                                r[10] = (val4 * 100) / val0;
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                //Initialisation des valeurs
                                client = dS.Tables[0].Rows[i][0].ToString();
                                val0 = (int)dS.Tables[0].Rows[i][3];
                                val1 = (int)dS.Tables[0].Rows[i][4];
                                val2 = (int)dS.Tables[0].Rows[i][6];
                                val3 = (int)dS.Tables[0].Rows[i][7];
                                val4 = (int)dS.Tables[0].Rows[i][9];
                            }
                            else
                            {
                                //masquage du doublon
                                dS.Tables[0].Rows[i][0] = "";
                                //Ajout aux valeurs
                                val0 += (int)dS.Tables[0].Rows[i][3];
                                val1 += (int)dS.Tables[0].Rows[i][4];
                                val2 += (int)dS.Tables[0].Rows[i][6];
                                val3 += (int)dS.Tables[0].Rows[i][7];
                                val4 += (int)dS.Tables[0].Rows[i][9];
                            }
                        }
                        //Sous-total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[3] = val0;
                        r[4] = val1;
                        r[5] = (val1 * 100) / val0;
                        r[6] = val0 - val1;
                        r[7] = val3;
                        r[8] = (val3 * 100) / val0;
                        r[9] = val4;
                        r[10] = (val4 * 100) / val0;
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        i++;
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(779);
                        r[1] = "";
                        r[3] = totVal0;
                        r[4] = totVal1;
                        r[5] = (totVal1 * 100) / totVal0;
                        r[6] = totVal0 - totVal1;
                        r[7] = totVal3;
                        r[8] = (totVal3 * 100) / totVal0;
                        r[9] = totVal4;
                        r[10] = (totVal4 * 100) / totVal0;
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString())
                    {
                        decimal val0 = 0, val1 = 0;
                        decimal totVal0 = 0, totVal1 = 0;
                        int cpt = 0, cptTotal = 0;
                        string cDT = "";
                        i = 0;
                        int val0Index = 8, val1Index = 10, totLibIndex = 1;
                        DataRow r;
                        //Initialisations
                        if (CurrentContext.ConnectedUtilisateur?.CentreDeTri?.RefEntite != null)
                        {
                            val0Index = 5;
                            val1Index = 7;
                            totLibIndex = 1;
                        }
                        cDT = dS.Tables[0].Rows[0][2].ToString();
                        val0 = (decimal)dS.Tables[0].Rows[0][val0Index];
                        val1 = (decimal)dS.Tables[0].Rows[0][val1Index];
                        totVal0 = (decimal)dS.Tables[0].Rows[0][val0Index];
                        totVal1 = (decimal)dS.Tables[0].Rows[0][val1Index];
                        cpt = 1;
                        cptTotal = 1;
                        //Parcours de dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Ajout à la moyenne
                            totVal0 += (decimal)dS.Tables[0].Rows[i][val0Index];
                            totVal1 += (decimal)dS.Tables[0].Rows[i][val1Index];
                            cptTotal++;
                            //Vérification du changement de regroupement
                            if (cDT != dS.Tables[0].Rows[i][2].ToString())
                            {
                                if (CurrentContext.ConnectedUtilisateur?.CentreDeTri?.RefEntite == null)
                                {
                                    //inscription de la moyenne
                                    r = dS.Tables[0].NewRow();
                                    r[totLibIndex] = CurrentContext.CulturedRessources.GetTextRessource(1271);
                                    r[val0Index] = Math.Round(val0 / cpt, 1, MidpointRounding.AwayFromZero);
                                    r[val1Index] = Math.Round(val1 / cpt, 1, MidpointRounding.AwayFromZero);
                                    dS.Tables[0].Rows.InsertAt(r, i);
                                    nbRow++;
                                    i++;
                                }
                                //Initialisation des valeurs
                                cDT = dS.Tables[0].Rows[i][2].ToString();
                                val0 = (decimal)dS.Tables[0].Rows[i][val0Index];
                                val1 = (decimal)dS.Tables[0].Rows[i][val1Index];
                                cpt = 1;
                            }
                            else
                            {
                                //Ajout aux valeurs
                                val0 += (decimal)dS.Tables[0].Rows[i][val0Index];
                                val1 += (decimal)dS.Tables[0].Rows[i][val1Index];
                                cpt++;
                            }
                        }
                        //Moyenne finale
                        if (CurrentContext.ConnectedUtilisateur?.CentreDeTri?.RefEntite == null)
                        {
                            r = dS.Tables[0].NewRow();
                            r[totLibIndex] = CurrentContext.CulturedRessources.GetTextRessource(1271);
                            r[val0Index] = Math.Round(val0 / cpt, 1, MidpointRounding.AwayFromZero);
                            r[val1Index] = Math.Round(val1 / cpt, 1, MidpointRounding.AwayFromZero);
                            nbRow++;
                            dS.Tables[0].Rows.InsertAt(r, i);
                            i++;
                        }
                        //Moyenne générale
                        r = dS.Tables[0].NewRow();
                        r[totLibIndex] = CurrentContext.CulturedRessources.GetTextRessource(1272);
                        r[val0Index] = Math.Round(totVal0 / cptTotal, 1, MidpointRounding.AwayFromZero);
                        r[val1Index] = Math.Round(totVal1 / cptTotal, 1, MidpointRounding.AwayFromZero);
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString())
                    {
                        int val0 = 0, val1 = 0;
                        int totVal0 = 0, totVal1 = 0;
                        int cpt = 0, cptTotal = 0;
                        string cDT = "";
                        i = 0;
                        DataRow r;
                        //Initialisations
                        cDT = dS.Tables[0].Rows[0][1].ToString();
                        val0 = (int)dS.Tables[0].Rows[0][3];
                        val1 = (int)dS.Tables[0].Rows[0][4];
                        totVal0 = (int)dS.Tables[0].Rows[0][3];
                        totVal1 = (int)dS.Tables[0].Rows[0][4];
                        cpt = 1;
                        cptTotal = 1;
                        //Parcours de dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Ajout à la moyenne
                            totVal0 += (int)dS.Tables[0].Rows[i][3];
                            totVal1 += (int)dS.Tables[0].Rows[i][4];
                            cptTotal++;
                            //Vérification du changement de regroupement
                            if (cDT != dS.Tables[0].Rows[i][1].ToString())
                            {
                                //inscription de la moyenne
                                r = dS.Tables[0].NewRow();
                                r[1] = CurrentContext.CulturedRessources.GetTextRessource(1271);
                                r[3] = Math.Round((decimal)(val0 / cpt), 1, MidpointRounding.AwayFromZero);
                                r[4] = Math.Round((decimal)(val1 / cpt), 1, MidpointRounding.AwayFromZero);
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                //Initialisation des valeurs
                                cDT = dS.Tables[0].Rows[i][1].ToString();
                                val0 = (int)dS.Tables[0].Rows[i][3];
                                val1 = (int)dS.Tables[0].Rows[i][4];
                                cpt = 1;
                            }
                            else
                            {
                                //Ajout aux valeurs
                                val0 += (int)dS.Tables[0].Rows[i][3];
                                val1 += (int)dS.Tables[0].Rows[i][4];
                                cpt++;
                            }
                        }
                        //Moyenne finale
                        r = dS.Tables[0].NewRow();
                        r[1] = CurrentContext.CulturedRessources.GetTextRessource(1271);
                        r[3] = Math.Round((decimal)(val0 / cpt), 1, MidpointRounding.AwayFromZero);
                        r[4] = Math.Round((decimal)(val1 / cpt), 1, MidpointRounding.AwayFromZero);
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        i++;
                        //Moyenne générale
                        r = dS.Tables[0].NewRow();
                        r[1] = CurrentContext.CulturedRessources.GetTextRessource(1272);
                        r[3] = Math.Round((decimal)(totVal0 / cptTotal), 1, MidpointRounding.AwayFromZero);
                        r[4] = Math.Round((decimal)(totVal1 / cptTotal), 1, MidpointRounding.AwayFromZero);
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatReceptionFournisseurChargement.ToString())
                    {
                        long val0 = 0, val1 = 0, val2 = 0, val3 = 0, val4 = 0;
                        long totVal0 = 0, totVal1 = 0, totVal2 = 0, totVal3 = 0, totVal4 = 0;
                        string produit = "";
                        i = 0;
                        DataRow r;
                        //Initialisations
                        produit = dS.Tables[0].Rows[0][0].ToString();
                        val0 = (int)dS.Tables[0].Rows[0][5];
                        val1 = (int)dS.Tables[0].Rows[0][6];
                        val2 = (int)dS.Tables[0].Rows[0][7];
                        val3 = 1;
                        val4 = 1;
                        totVal0 = (int)dS.Tables[0].Rows[0][5];
                        totVal1 = (int)dS.Tables[0].Rows[0][6];
                        totVal2 = (int)dS.Tables[0].Rows[0][7];
                        totVal3 = 1;
                        totVal4 = 1;
                        //Parcours de dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Ajout au total général
                            totVal0 += (int)dS.Tables[0].Rows[i][5];
                            totVal1 += (int)dS.Tables[0].Rows[i][6];
                            totVal2 += (int)dS.Tables[0].Rows[i][7];
                            totVal3++;
                            totVal4++;
                            //Vérification du changement de regroupement
                            if (produit != dS.Tables[0].Rows[i][0].ToString())
                            {
                                //inscription du sous-total
                                r = dS.Tables[0].NewRow();
                                r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                r[5] = val0;
                                r[6] = val1;
                                r[7] = val2;
                                r[8] = val3;
                                r[9] = val4;
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                //Initialisation des valeurs
                                produit = dS.Tables[0].Rows[i][0].ToString();
                                val0 = (int)dS.Tables[0].Rows[i][5];
                                val1 = (int)dS.Tables[0].Rows[i][6];
                                val2 = (int)dS.Tables[0].Rows[i][7];
                                val3 = 1;
                                val4 = 1;
                            }
                            else
                            {
                                //Ajout aux valeurs
                                val0 += (int)dS.Tables[0].Rows[i][5];
                                val1 += (int)dS.Tables[0].Rows[i][6];
                                val2 += (int)dS.Tables[0].Rows[i][7];
                                val3++;
                                val4++;
                            }
                        }
                        //Sous-total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[5] = val0;
                        r[6] = val1;
                        r[7] = val2;
                        r[8] = val3;
                        r[9] = val4;
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        i++;
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(779);
                        r[5] = totVal0;
                        r[6] = totVal1;
                        r[7] = totVal2;
                        r[8] = totVal3;
                        r[9] = totVal4;
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatReception.ToString())
                    {
                        long val0 = 0, val1 = 0, val2 = 0, val3 = 0;
                        long totVal0 = 0, totVal1 = 0, totVal2 = 0, totVal3 = 0;
                        string produit = "";
                        i = 0;
                        DataRow r;
                        //Initialisations
                        produit = dS.Tables[0].Rows[0][0].ToString();
                        val0 = (int)dS.Tables[0].Rows[0][4];
                        val1 = (int)dS.Tables[0].Rows[0][5];
                        val2 = 1;
                        val3 = 1;
                        totVal0 = (int)dS.Tables[0].Rows[0][4];
                        totVal1 = (int)dS.Tables[0].Rows[0][5];
                        totVal2 = 1;
                        totVal3 = 1;
                        //Parcours de dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Ajout au total général
                            totVal0 += (int)dS.Tables[0].Rows[i][4];
                            totVal1 += (int)dS.Tables[0].Rows[i][5];
                            totVal2++;
                            totVal3++;
                            //Vérification du changement de regroupement
                            if (produit != dS.Tables[0].Rows[i][0].ToString())
                            {
                                //inscription du sous-total
                                r = dS.Tables[0].NewRow();
                                r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                r[4] = val0;
                                r[5] = val1;
                                r[7] = val2;
                                r[8] = val3;
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                //Initialisation des valeurs
                                produit = dS.Tables[0].Rows[i][0].ToString();
                                val0 = (int)dS.Tables[0].Rows[i][4];
                                val1 = (int)dS.Tables[0].Rows[i][5];
                                val2 = 1;
                                val3 = 1;
                            }
                            else
                            {
                                //Ajout aux valeurs
                                val0 += (int)dS.Tables[0].Rows[i][4];
                                val1 += (int)dS.Tables[0].Rows[i][5];
                                val2++;
                                val3++;
                            }
                        }
                        //Sous-total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[4] = val0;
                        r[5] = val1;
                        r[7] = val2;
                        r[8] = val3;
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        i++;
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(779);
                        r[4] = totVal0;
                        r[5] = totVal1;
                        r[7] = totVal2;
                        r[8] = totVal3;
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClientProduit.ToString())
                    {
                        long val0 = 0, val1 = 0, val2 = 0, val3 = 0, val4 = 0;
                        i = 0;
                        DataRow r;
                        //Initialisations
                        val0 = (int)dS.Tables[0].Rows[0][1];
                        val1 = (int)dS.Tables[0].Rows[0][2];
                        val2 = (int)dS.Tables[0].Rows[0][4];
                        val3 = (int)dS.Tables[0].Rows[0][5];
                        val4 = (int)dS.Tables[0].Rows[0][6];
                        //Parcours du dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Calcul des valeurs
                            //Ajout aux valeurs
                            val0 += (int)dS.Tables[0].Rows[i][1];
                            val1 += (int)dS.Tables[0].Rows[i][2];
                            val2 += (int)dS.Tables[0].Rows[i][4];
                            val3 += (int)dS.Tables[0].Rows[i][5];
                            val4 += (int)dS.Tables[0].Rows[i][6];
                        }
                        //Total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[1] = val0;
                        r[2] = val1;
                        if (val0 != 0) { r[3] = 100 - ((val0 - val1) * 100 / val0); }
                        r[4] = val2;
                        r[5] = val3;
                        r[6] = val4;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuTonnageCLSousContrat.ToString())
                    {
                        long val0 = 0;
                        decimal val1 = 0;
                        long totVal0 = 0;
                        decimal totVal1 = 0;
                        string dR = "", coll = "";
                        i = 0;
                        DataRow r;
                        //Initialisations
                        dR = dS.Tables[0].Rows[0][0].ToString();
                        coll = dS.Tables[0].Rows[0][2].ToString();
                        val0 = 1;
                        val1 = (decimal)dS.Tables[0].Rows[0][4];
                        totVal0 = 1;
                        totVal1 = (decimal)dS.Tables[0].Rows[0][4];
                        //Parcours de dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Vérification du changement de regroupement
                            if (dR != dS.Tables[0].Rows[i][0].ToString())
                            {
                                //inscription du sous-total
                                r = dS.Tables[0].NewRow();
                                r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                r[2] = val0;
                                r[4] = val1;
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                //Initialisation des valeurs
                                dR = dS.Tables[0].Rows[i][0].ToString();
                                val0 = 1;
                                val1 = (decimal)dS.Tables[0].Rows[i][4];
                            }
                            else
                            {
                                //Ajout aux valeurs
                                if (coll != dS.Tables[0].Rows[i][2].ToString())
                                {
                                    val0++;
                                }
                                val1 += (decimal)dS.Tables[0].Rows[i][4];
                            }
                            //Ajout au total général
                            if (coll != dS.Tables[0].Rows[i][2].ToString())
                            {
                                totVal0++;
                                coll = dS.Tables[0].Rows[i][2].ToString();
                            }
                            totVal1 += (decimal)dS.Tables[0].Rows[i][4];
                        }
                        //Sous-total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[2] = val0;
                        r[4] = val1;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                        i++;
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(779);
                        r[1] = "";
                        r[2] = totVal0;
                        r[4] = totVal1;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduit.ToString())
                    {
                        if (dS.Tables[0].Rows.Count > 1)
                        {
                            decimal val0 = 0;
                            long val1 = 0, val2 = 0, val3 = 0;
                            string produit = "";
                            i = 0;
                            DataRow r;
                            //Initialisations
                            produit = dS.Tables[0].Rows[0][1].ToString();
                            val0 = (decimal)dS.Tables[0].Rows[0][3];
                            val1 = (int)dS.Tables[0].Rows[0][4];
                            val2 = (int)dS.Tables[0].Rows[0][5];
                            val3 = (int)dS.Tables[0].Rows[0][6];
                            for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                            {
                                //Vérification du changement de regroupement
                                if (produit != dS.Tables[0].Rows[i][1].ToString())
                                {
                                    //inscription du total
                                    r = dS.Tables[0].NewRow();
                                    r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                    r[3] = val0;
                                    r[4] = val1;
                                    r[5] = val2;
                                    r[6] = val3;
                                    dS.Tables[0].Rows.InsertAt(r, i);
                                    i++;
                                    nbRow++;
                                    //Initialisation des valeurs
                                    produit = dS.Tables[0].Rows[i][1].ToString();
                                    val0 = (decimal)dS.Tables[0].Rows[i][3];
                                    val1 = (int)dS.Tables[0].Rows[i][4];
                                    val2 = (int)dS.Tables[0].Rows[i][5];
                                    val3 = (int)dS.Tables[0].Rows[i][6];
                                }
                                else
                                {
                                    val0 += (decimal)dS.Tables[0].Rows[i][3];
                                    val1 += (int)dS.Tables[0].Rows[i][4];
                                    val2 += (int)dS.Tables[0].Rows[i][5];
                                    val3 += (int)dS.Tables[0].Rows[i][6];
                                }
                            }
                        }
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuLotDisponible.ToString())
                    {
                        int val1; int val2; int val3; int val4; int val5; int val6; int val7; int val8; int val9; int val10; int val11; int val12; int totVal1;
                        int totVal2; int totVal3; int totVal4; int totVal5; int totVal6; int totVal7; int totVal8; int totVal9; int totVal10; int totVal11; int totVal12;
                        string centreDeTri = "";
                        i = 0;
                        DataRow r;
                        //Initialisations
                        centreDeTri = dS.Tables[0].Rows[0][0].ToString();
                        val1 = (int)dS.Tables[0].Rows[0][3];
                        val2 = (int)dS.Tables[0].Rows[0][4];
                        val3 = (int)dS.Tables[0].Rows[0][5];
                        val4 = (int)dS.Tables[0].Rows[0][6];
                        val5 = (int)dS.Tables[0].Rows[0][7];
                        val6 = (int)dS.Tables[0].Rows[0][8];
                        val7 = (int)dS.Tables[0].Rows[0][9];
                        val8 = (int)dS.Tables[0].Rows[0][10];
                        val9 = (int)dS.Tables[0].Rows[0][11];
                        val10 = (int)dS.Tables[0].Rows[0][12];
                        val11 = (int)dS.Tables[0].Rows[0][13];
                        val12 = (int)dS.Tables[0].Rows[0][14];
                        totVal1 = (int)dS.Tables[0].Rows[0][3];
                        totVal2 = (int)dS.Tables[0].Rows[0][4];
                        totVal3 = (int)dS.Tables[0].Rows[0][5];
                        totVal4 = (int)dS.Tables[0].Rows[0][6];
                        totVal5 = (int)dS.Tables[0].Rows[0][7];
                        totVal6 = (int)dS.Tables[0].Rows[0][8];
                        totVal7 = (int)dS.Tables[0].Rows[0][9];
                        totVal8 = (int)dS.Tables[0].Rows[0][10];
                        totVal9 = (int)dS.Tables[0].Rows[0][11];
                        totVal10 = (int)dS.Tables[0].Rows[0][12];
                        totVal11 = (int)dS.Tables[0].Rows[0][13];
                        totVal12 = (int)dS.Tables[0].Rows[0][14];
                        //Parcours du dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Ajout au total général
                            totVal1 += (int)dS.Tables[0].Rows[i][3];
                            totVal2 += (int)dS.Tables[0].Rows[i][4];
                            totVal3 += (int)dS.Tables[0].Rows[i][5];
                            totVal4 += (int)dS.Tables[0].Rows[i][6];
                            totVal5 += (int)dS.Tables[0].Rows[i][7];
                            totVal6 += (int)dS.Tables[0].Rows[i][8];
                            totVal7 += (int)dS.Tables[0].Rows[i][9];
                            totVal8 += (int)dS.Tables[0].Rows[i][10];
                            totVal9 += (int)dS.Tables[0].Rows[i][11];
                            totVal10 += (int)dS.Tables[0].Rows[i][12];
                            totVal11 += (int)dS.Tables[0].Rows[i][13];
                            totVal12 += (int)dS.Tables[0].Rows[i][14];
                            //Vérification du changement de regroupement
                            if (centreDeTri != dS.Tables[0].Rows[i][0].ToString())
                            {
                                //inscription du sous-total
                                r = dS.Tables[0].NewRow();
                                r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                r[3] = val1;
                                r[4] = val2;
                                r[5] = val3;
                                r[6] = val4;
                                r[7] = val5;
                                r[8] = val6;
                                r[9] = val7;
                                r[10] = val8;
                                r[11] = val9;
                                r[12] = val10;
                                r[13] = val11;
                                r[14] = val12;
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                //Initialisation des valeurs
                                centreDeTri = dS.Tables[0].Rows[i][0].ToString();
                                val1 = (int)dS.Tables[0].Rows[i][3];
                                val2 = (int)dS.Tables[0].Rows[i][4];
                                val3 = (int)dS.Tables[0].Rows[i][5];
                                val4 = (int)dS.Tables[0].Rows[i][6];
                                val5 = (int)dS.Tables[0].Rows[i][7];
                                val6 = (int)dS.Tables[0].Rows[i][8];
                                val7 = (int)dS.Tables[0].Rows[i][9];
                                val8 = (int)dS.Tables[0].Rows[i][10];
                                val9 = (int)dS.Tables[0].Rows[i][11];
                                val10 = (int)dS.Tables[0].Rows[i][12];
                                val11 = (int)dS.Tables[0].Rows[i][13];
                                val12 = (int)dS.Tables[0].Rows[i][14];
                            }
                            else
                            {
                                //Ajout aux valeurs
                                val1 += (int)dS.Tables[0].Rows[i][3];
                                val2 += (int)dS.Tables[0].Rows[i][4];
                                val3 += (int)dS.Tables[0].Rows[i][5];
                                val4 += (int)dS.Tables[0].Rows[i][6];
                                val5 += (int)dS.Tables[0].Rows[i][7];
                                val6 += (int)dS.Tables[0].Rows[i][8];
                                val7 += (int)dS.Tables[0].Rows[i][9];
                                val8 += (int)dS.Tables[0].Rows[i][10];
                                val9 += (int)dS.Tables[0].Rows[i][11];
                                val10 += (int)dS.Tables[0].Rows[i][12];
                                val11 += (int)dS.Tables[0].Rows[i][13];
                                val12 += (int)dS.Tables[0].Rows[i][14];
                            }
                        }
                        //Sous-total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[3] = val1;
                        r[4] = val2;
                        r[5] = val3;
                        r[6] = val4;
                        r[7] = val5;
                        r[8] = val6;
                        r[9] = val7;
                        r[10] = val8;
                        r[11] = val9;
                        r[12] = val10;
                        r[13] = val11;
                        r[14] = val12;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                        i++;
                        //Total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(779);
                        r[3] = totVal1;
                        r[4] = totVal2;
                        r[5] = totVal3;
                        r[6] = totVal4;
                        r[7] = totVal5;
                        r[8] = totVal6;
                        r[9] = totVal7;
                        r[10] = totVal8;
                        r[11] = totVal9;
                        r[12] = totVal10;
                        r[13] = totVal11;
                        r[14] = totVal12;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuEtatTonnageParProcess.ToString())
                    {
                        decimal val0 = 0, val1 = 0, val2 = 0, totVal0 = 0, totVal1 = 0, totVal2 = 0, genVal0 = 0, genVal1 = 0, genVal2 = 0;
                        string composant = "", process = "";
                        i = 0;
                        DataRow r;
                        //Initialisations
                        process = dS.Tables[0].Rows[0][0].ToString();
                        composant = dS.Tables[0].Rows[0][2].ToString();
                        val0 = System.Convert.ToDecimal(dS.Tables[0].Rows[0][3]);
                        val1 = System.Convert.ToDecimal(dS.Tables[0].Rows[0][3]) * (decimal)dS.Tables[0].Rows[0][7];
                        val2 = (decimal)dS.Tables[0].Rows[0][8];
                        totVal0 = System.Convert.ToDecimal(dS.Tables[0].Rows[0][3]);
                        totVal1 = System.Convert.ToDecimal(dS.Tables[0].Rows[0][3]) * (decimal)dS.Tables[0].Rows[i][7];
                        totVal2 = (decimal)dS.Tables[0].Rows[0][8];
                        genVal0 = System.Convert.ToDecimal(dS.Tables[0].Rows[0][3]);
                        genVal1 = System.Convert.ToDecimal(dS.Tables[0].Rows[0][3]) * (decimal)dS.Tables[0].Rows[i][7];
                        genVal2 = (decimal)dS.Tables[0].Rows[0][8];
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Vérification du changement de regroupement composant
                            if (composant != dS.Tables[0].Rows[i][2].ToString() || process != dS.Tables[0].Rows[i][0].ToString())
                            {
                                //inscription du total
                                r = dS.Tables[0].NewRow();
                                r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                r[1] = composant;
                                r[3] = val0;
                                r[7] = Math.Round(val1 / val0, 2, MidpointRounding.AwayFromZero);
                                r[8] = val2;
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                //Initialisation des valeurs
                                composant = dS.Tables[0].Rows[i][2].ToString();
                                val0 = System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]);
                                val1 = System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]) * (decimal)dS.Tables[0].Rows[i][7];
                                val2 = (decimal)dS.Tables[0].Rows[i][8];
                                //Vérification du changement de regroupement process
                                if (process != dS.Tables[0].Rows[i][0].ToString())
                                {
                                    //inscription du total
                                    r = dS.Tables[0].NewRow();
                                    r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                    r[1] = process;
                                    r[3] = totVal0;
                                    r[7] = Math.Round(totVal1 / totVal0, 2, MidpointRounding.AwayFromZero);
                                    r[8] = totVal2;
                                    dS.Tables[0].Rows.InsertAt(r, i);
                                    nbRow++;
                                    i++;
                                    //Initialisation des valeurs
                                    process = dS.Tables[0].Rows[i][0].ToString();
                                    totVal0 = System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]);
                                    totVal1 = System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]) * (decimal)dS.Tables[0].Rows[i][7];
                                    totVal2 = (decimal)dS.Tables[0].Rows[i][8];
                                }
                                else
                                {
                                    totVal0 += System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]);
                                    totVal1 += System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]) * (decimal)dS.Tables[0].Rows[i][7];
                                    totVal2 += (decimal)dS.Tables[0].Rows[i][8];
                                }
                            }
                            else
                            {
                                val0 += System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]);
                                val1 += System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]) * (decimal)dS.Tables[0].Rows[i][7];
                                val2 += (decimal)dS.Tables[0].Rows[i][8];
                                totVal0 += System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]);
                                totVal1 += System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]) * (decimal)dS.Tables[0].Rows[i][7];
                                totVal2 += (decimal)dS.Tables[0].Rows[i][8];
                            }
                            genVal0 += System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]);
                            genVal1 += System.Convert.ToDecimal(dS.Tables[0].Rows[i][3]) * (decimal)dS.Tables[0].Rows[i][7];
                            genVal2 += (decimal)dS.Tables[0].Rows[i][8];
                        }
                        //Total final
                        //inscription du total
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[1] = composant;
                        r[3] = val0;
                        r[7] = Math.Round(val1 / val0, 2, MidpointRounding.AwayFromZero);
                        r[8] = val2;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                        i++;
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[1] = process;
                        r[3] = totVal0;
                        r[7] = Math.Round(totVal1 / totVal0, 2, MidpointRounding.AwayFromZero);
                        r[8] = totVal2;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                        i++;
                        //Total général
                        //inscription du total
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(779);
                        r[1] = "";
                        r[3] = genVal0;
                        r[7] = Math.Round(genVal1 / genVal0, 2, MidpointRounding.AwayFromZero);
                        r[8] = genVal2;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                        i++;
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteProduit.ToString())
                    {
                        long[] val = new long[dS.Tables[0].Columns.Count - 1];
                        i = 0;
                        DataRow r;
                        for (i = 0; i < dS.Tables[0].Rows.Count; i++)
                        {
                            for (int v = 1; v < dS.Tables[0].Columns.Count; v++)
                            {
                                val[v - 1] += (dS.Tables[0].Rows[i][v] == DBNull.Value ? 0 : (int)dS.Tables[0].Rows[i][v]);
                            }
                        }
                        //inscription du total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        for (int v = 0; v < dS.Tables[0].Columns.Count - 1; v++)
                        {
                            if (val[v] != 0) { r[v + 1] = val[v]; }
                        }
                        dS.Tables[0].Rows.InsertAt(r, i);
                        i++;
                        nbRow++;
                    }
                    if (menu == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString())
                    {
                        long[] val = new long[dS.Tables[0].Columns.Count - 2];
                        string groupe = "";
                        i = 0;
                        string p = "";
                        long sTotal = 0;
                        int d = 0;
                        DataRow r;
                        //Initialisations
                        groupe = dS.Tables[0].Rows[0][0].ToString();
                        for (int v = 2; v < dS.Tables[0].Columns.Count; v++)
                        {
                            val[v - 2] = (dS.Tables[0].Rows[0][v] == DBNull.Value ? 0 : (int)dS.Tables[0].Rows[0][v]);
                        }
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Ajout des valeurs
                            for (int v = 2; v < dS.Tables[0].Columns.Count; v++)
                            {
                                val[v - 2] += (dS.Tables[0].Rows[i][v] == DBNull.Value ? 0 : (int)dS.Tables[0].Rows[i][v]);
                            }
                            //    //Vérification du changement de regroupement
                            //    if (groupe != dS.Tables[0].Rows[i][0].ToString())
                            //    {
                            //        //inscription du total par composant
                            //        r = dS.Tables[0].NewRow();
                            //        r[0] = CurrentContext.CulturedRessources.GetTextRessource(1022);
                            //        for (int v = 0; v < dS.Tables[0].Columns.Count - 2; v++)
                            //        {
                            //            if (val[v] != 0) { r[v + 2] = val[v]; }
                            //        }
                            //        dS.Tables[0].Rows.InsertAt(r, i);
                            //        nbRow++;
                            //        i++;
                            //        //inscription du total par produit
                            //        r = dS.Tables[0].NewRow();
                            //        r[0] = CurrentContext.CulturedRessources.GetTextRessource(1021);
                            //        p = dS.Tables[0].Columns[2].Caption.Substring(0, dS.Tables[0].Columns[2].Caption.LastIndexOf("#/#"));
                            //        sTotal = val[0];
                            //        d = 0;
                            //        for (int v = 1; v < dS.Tables[0].Columns.Count - 2; v++)
                            //        {
                            //            //Si changement de produit
                            //            if (p != dS.Tables[0].Columns[v + 2].Caption.Substring(0, dS.Tables[0].Columns[v + 2].Caption.LastIndexOf("#/#")))
                            //            {
                            //                //Inscription des valeurs
                            //                for (int c = d; c < v; c++)
                            //                {
                            //                    if (sTotal != 0) { r[c + 2] = sTotal; }
                            //                }
                            //                //Réinitialisations
                            //                p = dS.Tables[0].Columns[v + 2].Caption.Substring(0, dS.Tables[0].Columns[v + 2].Caption.LastIndexOf("#/#"));
                            //                sTotal = val[v];
                            //                d = v;
                            //            }
                            //            else
                            //            {
                            //                sTotal += val[v];
                            //            }
                            //        }
                            //        //Dernier total par produit
                            //        for (int c = d; c < dS.Tables[0].Columns.Count - 2; c++)
                            //        {
                            //            if (sTotal != 0) { r[c + 2] = sTotal; }
                            //        }
                            //        //Insertion de la ligne
                            //        dS.Tables[0].Rows.InsertAt(r, i);
                            //        nbRow++;
                            //        i++;
                            //        //Initialisation des valeurs
                            //        groupe = dS.Tables[0].Rows[i][0].ToString();
                            //        for (int v = 2; v < dS.Tables[0].Columns.Count; v++)
                            //        {
                            //            val[v - 2] = (dS.Tables[0].Rows[i][v] == DBNull.Value ? 0 : (int)dS.Tables[0].Rows[i][v]);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        for (int v = 2; v < dS.Tables[0].Columns.Count; v++)
                            //        {
                            //            val[v - 2] += (dS.Tables[0].Rows[i][v] == DBNull.Value ? 0 : (int)dS.Tables[0].Rows[i][v]);
                            //        }
                            //    }
                            //}
                            ////inscription du total final
                            //r = dS.Tables[0].NewRow();
                            //r[0] = CurrentContext.CulturedRessources.GetTextRessource(1022);
                            //for (int v = 0; v < dS.Tables[0].Columns.Count - 2; v++)
                            //{
                            //    if (val[v] != 0) { r[v + 2] = val[v]; }
                            //}
                            //dS.Tables[0].Rows.InsertAt(r, i);
                            //nbRow++;
                            //i++;
                            ////inscription du total par produit
                            //r = dS.Tables[0].NewRow();
                            //r[0] = CurrentContext.CulturedRessources.GetTextRessource(1021);
                            //p = dS.Tables[0].Columns[2].Caption.Substring(0, dS.Tables[0].Columns[2].Caption.LastIndexOf("#/#"));
                            //sTotal = val[0];
                            //d = 0;
                            //for (int v = 1; v < dS.Tables[0].Columns.Count - 2; v++)
                            //{
                            //    //Si changement de produit
                            //    if (p != dS.Tables[0].Columns[v + 2].Caption.Substring(0, dS.Tables[0].Columns[v + 2].Caption.LastIndexOf("#/#")))
                            //    {
                            //        //Inscription des valeurs
                            //        for (int c = d; c < v; c++)
                            //        {
                            //            if (sTotal != 0) { r[c + 2] = sTotal; }
                            //        }
                            //        //Réinitialisations
                            //        p = dS.Tables[0].Columns[v + 2].Caption.Substring(0, dS.Tables[0].Columns[v + 2].Caption.LastIndexOf("#/#"));
                            //        sTotal = val[v];
                            //        d = v;
                            //    }
                            //    else
                            //    {
                            //        sTotal += val[v];
                            //    }
                            //}
                            ////Dernier total par produit
                            //for (int c = d; c < dS.Tables[0].Columns.Count - 2; c++)
                            //{
                            //    if (sTotal != 0) { r[c + 2] = sTotal; }
                            //}
                            ////Insertion de la ligne
                            //dS.Tables[0].Rows.InsertAt(r, i);
                            //nbRow++;
                        }
                        //Inscription
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        for (int v = 0; v < dS.Tables[0].Columns.Count - 2; v++)
                        {
                            if (val[v] != 0) { r[v + 2] = val[v]; }
                        }
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                        i++;
                    }
                    if (menu == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString())
                    {
                        long val0 = 0, val1 = 0, val2 = 0;
                        long totVal0 = 0, totVal1 = 0, totVal2 = 0;
                        string client = "";
                        DataRow r;
                        //Initialisations
                        client = dS.Tables[0].Rows[0][0].ToString();
                        val0 = (int)dS.Tables[0].Rows[0][3];
                        val1 = (int)dS.Tables[0].Rows[0][4];
                        val2 = (int)dS.Tables[0].Rows[0][5];
                        totVal0 = (int)dS.Tables[0].Rows[0][3];
                        totVal1 = (int)dS.Tables[0].Rows[0][4];
                        totVal2 = (int)dS.Tables[0].Rows[0][5];
                        //Parcours de dataset
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Ajout au total général
                            totVal0 += (int)dS.Tables[0].Rows[i][3];
                            totVal1 += (int)dS.Tables[0].Rows[i][4];
                            totVal2 += (int)dS.Tables[0].Rows[i][5];
                            //Vérification du changement de regroupement
                            if (client != dS.Tables[0].Rows[i][0].ToString())
                            {
                                //inscription du sous-total
                                r = dS.Tables[0].NewRow();
                                r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                r[3] = val0;
                                r[4] = val1;
                                r[5] = val2;
                                r[6] = (val1 == 0 ? 0 : Math.Round(val2 * 100 / (decimal)val1, 2, MidpointRounding.AwayFromZero)); ;
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                //Initialisation des valeurs
                                client = dS.Tables[0].Rows[i][0].ToString();
                                val0 = (int)dS.Tables[0].Rows[i][3];
                                val1 = (int)dS.Tables[0].Rows[i][4];
                                val2 = (int)dS.Tables[0].Rows[i][5];
                            }
                            else
                            {
                                //masquage du doublon
                                dS.Tables[0].Rows[i][0] = "";
                                //Ajout aux valeurs
                                val0 += (int)dS.Tables[0].Rows[i][3];
                                val1 += (int)dS.Tables[0].Rows[i][4];
                                val2 += (int)dS.Tables[0].Rows[i][5];
                            }
                        }
                        //Sous-total final
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[3] = val0;
                        r[4] = val1;
                        r[5] = val2;
                        r[6] = (val1 == 0 ? 0 : Math.Round(val2 * 100 / (decimal)val1, 2, MidpointRounding.AwayFromZero));
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                        i++;
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[3] = totVal0;
                        r[4] = totVal1;
                        r[5] = totVal2;
                        r[6] = (totVal1 == 0 ? 0 : Math.Round(totVal2 * 100 / (decimal)totVal1, 2, MidpointRounding.AwayFromZero));
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                    }
                    if (menu == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString())
                    {
                        decimal val0 = 0, val1 = 0, val2 = 0, val3 = 0, val4 = 0, val5 = 0, val6 = 0, val7 = 0, val8 = 0, val9 = 0;
                        decimal totVal0 = 0, totVal1 = 0, totVal2 = 0, totVal3 = 0, totVal4 = 0, totVal5 = 0, totVal6 = 0, totVal7 = 0, totVal8 = 0, totVal9 = 0;
                        int nbLigne = dS.Tables[0].Rows.Count;
                        string contrat = "", region = "";
                        i = 0;
                        DataRow r;
                        //Initialisations
                        contrat = dS.Tables[0].Rows[0][8].ToString();
                        region = dS.Tables[0].Rows[0][0].ToString();
                        val0 = 1;
                        val1 = (decimal)dS.Tables[0].Rows[0][3];
                        val2 = (decimal)dS.Tables[0].Rows[0][4];
                        val3 = (int)dS.Tables[0].Rows[0][6];
                        val4 = (decimal)dS.Tables[0].Rows[0][7];
                        val5 = (int)dS.Tables[0].Rows[0][8];
                        val6 = (int)dS.Tables[0].Rows[0][9];
                        val7 = (decimal)dS.Tables[0].Rows[0][11];
                        val8 = (bool)dS.Tables[0].Rows[0][12] ? 1 : 0;
                        val9 = (bool)dS.Tables[0].Rows[0][13] ? 1 : 0;
                        totVal0 = 1;
                        totVal1 = (decimal)dS.Tables[0].Rows[0][3];
                        totVal2 = (decimal)dS.Tables[0].Rows[0][4];
                        totVal3 = (int)dS.Tables[0].Rows[0][6];
                        totVal4 = (decimal)dS.Tables[0].Rows[0][7];
                        totVal5 = (int)dS.Tables[0].Rows[0][8];
                        totVal6 = (int)dS.Tables[0].Rows[0][9];
                        totVal7 = (decimal)dS.Tables[0].Rows[0][11];
                        totVal8 = (bool)dS.Tables[0].Rows[0][12] ? 1 : 0;
                        totVal9 = (bool)dS.Tables[0].Rows[0][13] ? 1 : 0;
                        for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                        {
                            //Vérification du changement de regroupement région
                            if (region != dS.Tables[0].Rows[i][0].ToString())
                            {
                                //inscription du total
                                r = dS.Tables[0].NewRow();
                                r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                r[1] = CurrentContext.CulturedRessources.GetTextRessource(1019);
                                r[2] = val0.ToString();
                                r[3] = val1;
                                r[4] = val2;
                                r[5] = ((val1 / val3)).ToString("0.000");
                                r[6] = val3;
                                r[7] = val4;
                                r[8] = val5;
                                r[9] = val6;
                                if (val5 != 0) { r[10] = (val6 / val5).ToString("0.0000"); }
                                r[11] = val7;
                                //r[12] = val8;
                                //r[13] = val9;
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                //Initialisation des valeurs
                                region = dS.Tables[0].Rows[i][0].ToString();
                                val0 = 1;
                                val1 = (decimal)dS.Tables[0].Rows[i][3];
                                val2 = (decimal)dS.Tables[0].Rows[i][4];
                                val3 = (int)dS.Tables[0].Rows[i][6];
                                val4 = (decimal)dS.Tables[0].Rows[i][7];
                                val5 = (int)dS.Tables[0].Rows[i][8];
                                val6 = (int)dS.Tables[0].Rows[i][9];
                                val7 = (decimal)dS.Tables[0].Rows[i][11];
                                val8 = (bool)dS.Tables[0].Rows[i][12] ? 1 : 0;
                                val9 = (bool)dS.Tables[0].Rows[i][13] ? 1 : 0;
                                //Vérification du changement de regroupement contrat
                                if (contrat != dS.Tables[0].Rows[i][8].ToString())
                                {
                                    //inscription du total
                                    r = dS.Tables[0].NewRow();
                                    r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                    r[1] = CurrentContext.CulturedRessources.GetTextRessource(1020);
                                    r[2] = totVal0.ToString();
                                    r[3] = totVal1;
                                    r[4] = totVal2;
                                    r[5] = ((totVal1 / totVal3)).ToString("0.000");
                                    r[6] = totVal3;
                                    r[7] = totVal4;
                                    r[8] = totVal5;
                                    r[9] = totVal6;
                                    if (totVal5 != 0) { r[10] = (totVal6 / totVal5).ToString("0.0000"); }
                                    r[11] = totVal7;
                                    //r[12] = totVal8;
                                    //r[13] = totVal9;
                                    dS.Tables[0].Rows.InsertAt(r, i);
                                    nbRow++;
                                    i++;
                                    //Initialisation des valeurs
                                    contrat = dS.Tables[0].Rows[i][8].ToString();
                                    totVal0 = 1;
                                    totVal1 = (decimal)dS.Tables[0].Rows[i][3];
                                    totVal2 = (decimal)dS.Tables[0].Rows[i][4];
                                    totVal3 = (int)dS.Tables[0].Rows[i][6];
                                    totVal4 = (decimal)dS.Tables[0].Rows[i][7];
                                    totVal5 = (int)dS.Tables[0].Rows[i][8];
                                    totVal6 = (int)dS.Tables[0].Rows[i][9];
                                    totVal7 = (decimal)dS.Tables[0].Rows[i][11];
                                    totVal8 = (bool)dS.Tables[0].Rows[i][12] ? 1 : 0;
                                    totVal9 = (bool)dS.Tables[0].Rows[i][13] ? 1 : 0;
                                }
                                else
                                {
                                    totVal0 += 1;
                                    totVal1 += (decimal)dS.Tables[0].Rows[i][3];
                                    totVal2 += (decimal)dS.Tables[0].Rows[i][4];
                                    totVal3 += (int)dS.Tables[0].Rows[i][6];
                                    totVal4 += (decimal)dS.Tables[0].Rows[i][7];
                                    totVal5 += (int)dS.Tables[0].Rows[i][8];
                                    totVal6 += (int)dS.Tables[0].Rows[i][9];
                                    totVal7 += (decimal)dS.Tables[0].Rows[i][11];
                                    totVal8 += (bool)dS.Tables[0].Rows[i][12] ? 1 : 0;
                                    totVal9 += (bool)dS.Tables[0].Rows[i][13] ? 1 : 0;
                                }
                            }
                            else
                            {
                                val0 += 1;
                                val1 += (decimal)dS.Tables[0].Rows[i][3];
                                val2 += (decimal)dS.Tables[0].Rows[i][4];
                                val3 += (int)dS.Tables[0].Rows[i][6];
                                val4 += (decimal)dS.Tables[0].Rows[i][7];
                                val5 += (int)dS.Tables[0].Rows[i][8];
                                val6 += (int)dS.Tables[0].Rows[i][9];
                                val7 += (decimal)dS.Tables[0].Rows[i][11];
                                val8 += (bool)dS.Tables[0].Rows[i][12] ? 1 : 0;
                                val9 += (bool)dS.Tables[0].Rows[i][13] ? 1 : 0;
                                totVal0 += 1;
                                totVal1 += (decimal)dS.Tables[0].Rows[i][3];
                                totVal2 += (decimal)dS.Tables[0].Rows[i][4];
                                totVal3 += (int)dS.Tables[0].Rows[i][6];
                                totVal4 += (decimal)dS.Tables[0].Rows[i][7];
                                totVal5 += (int)dS.Tables[0].Rows[i][8];
                                totVal6 += (int)dS.Tables[0].Rows[i][9];
                                totVal7 += (decimal)dS.Tables[0].Rows[i][11];
                                totVal8 += (bool)dS.Tables[0].Rows[i][12] ? 1 : 0;
                                totVal9 += (bool)dS.Tables[0].Rows[i][13] ? 1 : 0;
                            }
                        }
                        //Total final
                        //inscription du total
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[1] = CurrentContext.CulturedRessources.GetTextRessource(1019);
                        r[2] = val0.ToString();
                        r[3] = val1;
                        r[4] = val2;
                        r[5] = ((val1 / val3)).ToString("0.000");
                        r[6] = val3;
                        r[7] = val4;
                        r[8] = val5;
                        r[9] = val6;
                        if (val5 != 0) { r[10] = (val6 / val5).ToString("0.0000"); }
                        r[11] = val7;
                        //r[12] = val8;
                        //r[13] = val9;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                        i++;
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                        r[1] = CurrentContext.CulturedRessources.GetTextRessource(1020);
                        r[2] = totVal0.ToString();
                        r[3] = totVal1;
                        r[4] = totVal2;
                        r[5] = ((totVal1 / totVal3)).ToString("0.000");
                        r[6] = totVal3;
                        r[7] = totVal4;
                        r[8] = totVal5;
                        r[9] = totVal6;
                        if (totVal5 != 0) { r[10] = (totVal6 / totVal5).ToString("0.0000"); }
                        r[11] = totVal7;
                        //r[12] = totVal8;
                        //r[13] = totVal9;
                        dS.Tables[0].Rows.InsertAt(r, i);
                        nbRow++;
                        i++;
                        //Total général
                        totVal0 = 0; totVal1 = 0; totVal2 = 0; totVal3 = 0; totVal4 = 0; totVal5 = 0; totVal6 = 0; totVal7 = 0; totVal8 = 0; totVal9 = 0;
                        for (int j = 0; j < dS.Tables[0].Rows.Count; j++)
                        {
                            if (dS.Tables[0].Rows[j][0].ToString() != CurrentContext.CulturedRessources.GetTextRessource(778))
                            {
                                totVal0 += 1;
                                totVal1 += (decimal)dS.Tables[0].Rows[j][3];
                                totVal2 += (decimal)dS.Tables[0].Rows[j][4];
                                totVal3 += (int)dS.Tables[0].Rows[j][6];
                                totVal4 += (decimal)dS.Tables[0].Rows[j][7];
                                totVal5 += (int)dS.Tables[0].Rows[j][8];
                                totVal6 += (int)dS.Tables[0].Rows[j][9];
                                totVal7 += (decimal)dS.Tables[0].Rows[j][11];
                                totVal8 += (bool)dS.Tables[0].Rows[j][12] ? 1 : 0;
                                totVal9 += (bool)dS.Tables[0].Rows[j][13] ? 1 : 0;
                            }
                        }
                        r = dS.Tables[0].NewRow();
                        r[0] = CurrentContext.CulturedRessources.GetTextRessource(779);
                        r[1] = "";
                        r[2] = totVal0.ToString();
                        r[3] = totVal1;
                        r[4] = totVal2;
                        r[5] = ((totVal1 / totVal3)).ToString("0.000");
                        r[6] = totVal3;
                        r[7] = totVal4;
                        r[8] = totVal5;
                        r[9] = totVal6;
                        if (totVal5 != 0) { r[10] = (totVal6 / totVal5).ToString("0.0000"); }
                        r[11] = totVal7;
                        //r[12] = totVal8;
                        //r[13] = totVal9;
                        nbRow++;
                        dS.Tables[0].Rows.InsertAt(r, i);
                    }
                    if (menu == Enumerations.MenuName.ModuleCollectiviteMenuStatistiques.ToString())
                    {
                        decimal val0 = 0;
                        decimal totVal0 = 0;
                        switch (eSF.StatType)
                        {
                            case "DestinationTonnage":
                                //Parcours de dataset pour calculer le total
                                for (i = 0; i < dS.Tables[0].Rows.Count; i++)
                                {
                                    //Ajout au total général
                                    totVal0 += (decimal)dS.Tables[0].Rows[i][1];
                                }
                                //Parcours de dataset pour modifier les valeurs
                                for (i = 0; i < dS.Tables[0].Rows.Count; i++)
                                {
                                    val0 = Math.Round((((decimal)dS.Tables[0].Rows[i][1]) / totVal0) * 100, 2, MidpointRounding.AwayFromZero);
                                    dS.Tables[0].Rows[i][1] = val0;
                                }
                                break;
                            case "DeboucheBalle":
                                //Parcours de dataset pour calculer le total
                                for (i = 0; i < dS.Tables[0].Rows.Count; i++)
                                {
                                    //Ajout au total général
                                    totVal0 += (decimal)dS.Tables[0].Rows[i][1];
                                }
                                //Parcours de dataset pour modifier les valeurs
                                for (i = 0; i < dS.Tables[0].Rows.Count; i++)
                                {
                                    val0 = Math.Round((((decimal)dS.Tables[0].Rows[i][1]) / totVal0) * 100, 2, MidpointRounding.AwayFromZero);
                                    dS.Tables[0].Rows[i][0] += (" (" + val0.ToString("F2", CurrentContext.CurrentCulture) + "%)");
                                    dS.Tables[0].Rows[i][1] = val0;
                                }
                                break;
                            case "ExtractionRSE":
                                long[] val = new long[dS.Tables[0].Columns.Count - 2];
                                string groupe = "";
                                i = 0;
                                DataRow r;
                                //Initialisations
                                groupe = dS.Tables[0].Rows[0][0].ToString();
                                for (int v = 2; v < dS.Tables[0].Columns.Count; v++)
                                {
                                    val[v - 2] = (dS.Tables[0].Rows[0][v] == DBNull.Value ? 0 : (int)dS.Tables[0].Rows[0][v]);
                                }
                                for (i = 1; i < dS.Tables[0].Rows.Count; i++)
                                {
                                    //Ajout des valeurs
                                    for (int v = 2; v < dS.Tables[0].Columns.Count; v++)
                                    {
                                        val[v - 2] += (dS.Tables[0].Rows[i][v] == DBNull.Value ? 0 : (int)dS.Tables[0].Rows[i][v]);
                                    }
                                }
                                //Inscription
                                r = dS.Tables[0].NewRow();
                                r[0] = CurrentContext.CulturedRessources.GetTextRessource(778);
                                for (int v = 0; v < dS.Tables[0].Columns.Count - 2; v++)
                                {
                                    if (val[v] != 0) { r[v + 2] = val[v]; }
                                }
                                dS.Tables[0].Rows.InsertAt(r, i);
                                nbRow++;
                                i++;
                                break;
                        }
                    }
                }
                //Export if needed
                if (exportType != "")
                {
                    //Headers
                    var cD = new System.Net.Mime.ContentDisposition
                    {
                        FileName = "ValorplastTransport.xlsx",
                        // always prompt the user for downloading
                        Inline = false,
                    };
                    Response.Headers.Append("Content-Disposition", cD.ToString());
                    //Define statistique name
                    string statName = menu;
                    if (menu == Enumerations.MenuName.ModuleCollectiviteMenuStatistiques.ToString()
                        && !string.IsNullOrWhiteSpace(eSF.StatType)) { 
                        statName = eSF.StatType; }
                    //Generate Excel file
                    return new FileContentResult(ExcelFileManagement.CreateStatistique(statName, dS, eSF, CurrentContext, _dbContext, _env.ContentRootPath).ToArray(), "application /octet-stream");
                }
                //Return nb of row in header
                Response.Headers.Append("nbRow", nbRow.ToString());
                //Return Json paged data
                var pagedTable = dS.Tables[0];
                if (pagedTable.Rows.Count > 0)
                {
                    pagedTable = (from r in dS.Tables[0].AsEnumerable()
                                  select r).Skip(pageNumber * pageSize).Take(pageSize).CopyToDataTable();
                }
                return new JsonResult(pagedTable, JsonSettings);
            }
            else
            {
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));
            }
        }
    }
}
