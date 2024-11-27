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
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using eVaSys.APIUtils;
using System;
using System.Collections.Generic;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class GridSimpleController : BaseApiController
    {
        #region Constructor
        public GridSimpleController(ApplicationDbContext context, IConfiguration configuration)
            : base(context, configuration)
        {
            _dbContext = context;
        }
        #endregion Constructor
        private readonly ApplicationDbContext _dbContext;
        /// <summary>
        /// GET: evapi/gridsimple/get
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of all items.</returns>
        [HttpGet("GetItems")]
        public IActionResult GetItems()
        {
            string type = Request.Query["type"].ToString();
            int pageNumber = System.Convert.ToInt32(Request.Query["pageNumber"]);
            int pageSize = System.Convert.ToInt32(Request.Query["pageSize"]);
            string sortOrder = Request.Query["sortOrder"].ToString();
            string sortExpression = Request.Query["sortExpression"].ToString();
            string filterText = Request.Headers["filterText"].ToString();
            string filterUtilisateurType = Request.Headers["filterUtilisateurType"].ToString();
            string filterD = Request.Headers["filterD"].ToString();
            string filterYear = Request.Headers["filterYear"].ToString();
            string selectedItem = Request.Headers["selectedItem"].ToString();
            string exclude = Request.Headers["exclude"].ToString();
            SqlCommand cmd = new();
            DataSet dS = new();
            string sqlStr = "";
            //Process
            DateTime d = DateTime.MinValue;
            DateTime.TryParse(filterD, out d);
            //Chech authorization
            if (!string.IsNullOrEmpty(type) && !Rights.AuthorizedAction(type, CurrentContext, Configuration))
            {
                Utils.Utils.DebugPrint("Grid-simple controller - no action", Configuration["Data:DefaultConnection:ConnectionString"]);
                return Unauthorized(new UnauthorizedError(CurrentContext.CulturedRessources.GetTextRessource(345)));
            }
            //Process if Authorized
            switch (type)
            {
                case "Mixte":
                    sqlStr = "select distinct RefCommandeFournisseur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefCommandeFournisseur.ToString()].Name + "]"
                        + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "], tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriLibelle.ToString()].Name + "]"
                        + "     , adresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurAdresseOrigineVille.ToString()].Name + "], tblProduit.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitLibelle.ToString()].Name + "]"
                        + "     , D as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDDisponibilite.ToString()].Name + "], NumeroCommande as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].Name + "]"
                        + " from tblCommandeFournisseur"
                        + " inner join tblEntite on tblEntite.RefEntite=tblCommandeFournisseur.RefEntite"
                        + " inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                        + " left join (select tblAdresse.RefEntite, CodePostal, Ville from tblAdresse"
                        + "     inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + "     inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse=tbmContactAdresse.RefContactAdresse where RefContactAdresseProcess=1) as adresse on tblEntite.RefEntite=adresse.RefEntite"
                        + " where tblCommandeFournisseur.RefAdresseClient is null and tblCommandeFournisseur.RefTransporteur is null";
                    //Filters
                    if (filterText != "")
                    {
                        sqlStr += " and (cast(RefCommandeFournisseur as nvarchar)=@cherche or tblEntite.Libelle like '%'+@cherche+'%' or tblEntite.CodeEE = @cherche or adresse.Ville like '%'+@cherche+'%' or tblProduit.Libelle like '%'+@cherche+'%' or cast(NumeroCommande as nvarchar) =@cherche)";
                        cmd.Parameters.Add("@cherche", SqlDbType.NVarChar).Value = filterText;
                    }
                    break;
                case "TransporteurForUtilisateur":
                    sqlStr = "select tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefEntite.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + " from tblEntite"
                        + " where tblEntite.RefEntiteType=2";
                    //Filters
                    sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                        , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteLibelle }
                        );
                    if (CurrentContext.filterGlobalActif)
                    {
                        sqlStr += " and tblEntite.Actif=1";
                    }
                    break;
                case "CentreDeTriForUtilisateur":
                    sqlStr = "select distinct tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefEntite.ToString()].Name + "]"
                        + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseOrigineVille.ToString()].Name + "]"
                        + " from tblEntite"
                        + " left join (select tblAdresse.RefEntite, CodePostal, Ville from tblAdresse"
                        + "     inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + "     inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresseContactAdresseProcess.RefContactAdresse=tbmContactAdresse.RefContactAdresse where RefContactAdresseProcess=1) as tblAdresse on tblEntite.RefEntite=tblAdresse.RefEntite"
                        + " where tblEntite.RefEntiteType=3";
                    //Filters
                    sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                        , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteLibelle, Enumerations.DataColumnName.AdresseVille }
                        , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteCodeCITEO }
                        );
                    if (CurrentContext.filterGlobalActif)
                    {
                        sqlStr += " and tblEntite.Actif=1";
                    }
                    break;
                case "CollectiviteForUtilisateur":
                    sqlStr = "select tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefEntite.ToString()].Name + "]"
                        + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + " from tblEntite"
                        + " where tblEntite.RefEntiteType=1";
                    //Filters
                    sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                        , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteLibelle }
                        , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteCodeCITEO }
                        );
                    if (CurrentContext.filterGlobalActif)
                    {
                        sqlStr += " and tblEntite.Actif=1";
                    }
                    break;
                case "ClientForUtilisateur":
                    sqlStr = "select distinct tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefEntite.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + "     , tblAdresse.Ville as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                        + " from tblEntite"
                        + " left join tblAdresse on tblEntite.RefEntite=tblAdresse.RefEntite"
                        + " where tblEntite.RefEntiteType=4";
                    //Filters
                    sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                        , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteLibelle, Enumerations.DataColumnName.AdresseVille }
                        );
                    if (CurrentContext.filterGlobalActif)
                    {
                        sqlStr += " and tblEntite.Actif=1";
                    }
                    break;
                case "PrestataireForUtilisateur":
                    sqlStr = "select distinct tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefEntite.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + "     , null as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseDestinationVille.ToString()].Name + "]"
                        + " from tblEntite"
                        + " where tblEntite.RefEntiteType=8";
                    //Filters
                    sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                        , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteLibelle }
                        );
                    if (CurrentContext.filterGlobalActif)
                    {
                        sqlStr += " and tblEntite.Actif=1";
                    }
                    break;
                case "UtilisateurMaitreForUtilisateur":
                    sqlStr = "select tblUtilisateur.RefUtilisateur as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.Id.ToString()].Name + "]"
                        + "     , tblUtilisateur.Nom as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurNom.ToString()].Name + "]"
                        + "     , case when collectivite.RefEntite is not null then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(594) + "'"
                        + "         else case when transporteur.RefEntite is not null then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(255) + "'"
                        + "         else case when centreDeTri.RefEntite is not null then '" + CurrentContext.CulturedRessources.GetTextSQLRessource(17) + "'"
                        + "         else case when client.RefEntite is not null then  '" + CurrentContext.CulturedRessources.GetTextSQLRessource(16) + "'"
                        + "         else case when prestataire.RefEntite is not null then  '" + CurrentContext.CulturedRessources.GetTextSQLRessource(1251) + "'"
                        + "         else  '" + CurrentContext.CulturedRessources.GetTextSQLRessource(1447) + "' end end end end end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.UtilisateurType.ToString()].Name + "]"
                        + "     , case when collectivite.RefEntite is not null then (case when collectivite.CodeEE is not null then collectivite.CodeEE + ' - ' else '' end + collectivite.Libelle)"
                        + "         else case when transporteur.RefEntite is not null then Transporteur.Libelle"
                        + "         else case when centreDeTri.RefEntite is not null then (case when centredetri.CodeEE is not null then centredetri.CodeEE + ' - ' else '' end + centredetri.Libelle)"
                        + "         else case when client.RefEntite is not null then client.Libelle"
                        + "         else case when prestataire.RefEntite is not null then prestataire.Libelle"
                        + "         else 'Valorplast' end end end end end as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + " from tblUtilisateur"
                        + "     left join tblEntite as client on tblUtilisateur.RefClient=client.RefEntite"
                        + "     left join tblEntite as collectivite on tblUtilisateur.RefCollectivite=collectivite.RefEntite"
                        + "     left join tblEntite as centredetri on tblUtilisateur.RefCentreDeTri=centredetri.RefEntite"
                        + "     left join tblEntite as transporteur on tblUtilisateur.RefTransporteur=transporteur.RefEntite"
                        + "     left join tblEntite as prestataire on tblUtilisateur.RefPrestataire=prestataire.RefEntite"
                        + " where tblUtilisateur.RefUtilisateur<>1 and tblUtilisateur.Actif=1 and tblUtilisateur.RefUtilisateurMaitre is null";
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
                    if (!string.IsNullOrWhiteSpace(exclude))
                    {
                        int e = 0;
                        int.TryParse(exclude, out e);
                        cmd.Parameters.Add("@exclude", SqlDbType.Int).Value = exclude;
                        sqlStr += " and tblUtilisateur.RefUtilisateur != @exclude";
                    }
                    //UtilisateurType
                    sqlStr += " and ("
                        + " (case when @filterUtilisateurType='" + Enumerations.UtilisateurType.Valorplast + "' and collectivite.RefEntite is null and transporteur.RefEntite is null"
                        + "         and centreDeTri.RefEntite is null and client.RefEntite is null and prestataire.RefEntite is  null then 1 else 0 end = 1)"
                        + " or (case when @filterUtilisateurType='" + Enumerations.UtilisateurType.Collectivite + "' and collectivite.RefEntite is not null then 1 else 0 end = 1)"
                        + " or (case when @filterUtilisateurType='" + Enumerations.UtilisateurType.CentreDeTri + "' and centreDeTri.RefEntite is not null then 1 else 0 end = 1)"
                        + " or (case when @filterUtilisateurType='" + Enumerations.UtilisateurType.Transporteur + "' and transporteur.RefEntite is not null then 1 else 0 end = 1)"
                        + " or (case when @filterUtilisateurType='" + Enumerations.UtilisateurType.Client + "' and client.RefEntite is not null then 1 else 0 end = 1)"
                        + " or (case when @filterUtilisateurType='" + Enumerations.UtilisateurType.Prestataire + "' and prestataire.RefEntite is not null then 1 else 0 end = 1)"
                        + " or (0 = 1)"
                        + " )";
                    cmd.Parameters.Add("@filterUtilisateurType", SqlDbType.NVarChar).Value = filterUtilisateurType;
                    break;
                case "IncitationQualite":
                    sqlStr = "select distinct tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + "     , tbmContactAdresse.Email as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseEmail.ToString()].Name + "]"
                        + " from tblEntite"
                        + " 	inner join tblAdresse on tblAdresse.RefEntite=tblEntite.refEntite"
                        + " 	inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + " 	inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresse.RefContactAdresse = tbmContactAdresseContactAdresseProcess.RefContactAdresse"
                        + "     left join (select distinct RefEntite from  tblEmailIncitationQualite where Annee=@year) as tblEmailIncitationQualite on tblEntite.RefEntite=tblEmailIncitationQualite.RefEntite"
                        + " where tbmContactAdresseContactAdresseProcess.RefContactAdresseProcess = 8"
                        + "     and tblEmailIncitationQualite.RefEntite is null"
                        + " 	and tblEntite.RefEntite in (select RefEntite from tblContratIncitationQualite where year(DDebut) = @year)";
                    //Filters
                    cmd.Parameters.Add("@year", SqlDbType.NVarChar).Value = (string.IsNullOrEmpty(filterYear) ? "0" : filterYear);
                    break;
                case "EmailNoteCreditCollectivite":
                    sqlStr = "select distinct DO_PIECE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGENumeroPiece.ToString()].Name + "]"
                        + "     , DO_DATE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEDatePiece.ToString()].Name + "]"
                        + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + "     , tbmContactAdresse.Email as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseEmail.ToString()].Name + "]"
                        + " from F_DOCENTETE"
                        + " 	inner join tblEntite on F_DOCENTETE.DO_TIERS=tblEntite.SAGECodeComptable"
                        + " 	inner join tblAdresse on tblAdresse.RefEntite=tblEntite.refEntite"
                        + " 	inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                        + " 	inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresse.RefContactAdresse = tbmContactAdresseContactAdresseProcess.RefContactAdresse"
                        + "     left join tblEmailNoteCredit on F_DOCENTETE.DO_PIECE=tblEmailNoteCredit.RefSAGEDocument"
                        + " where tbmContactAdresseContactAdresseProcess.RefContactAdresseProcess = 3 and tbmContactAdresse.Email is not null"
                        + " 	and DO_TYPE = 17 and DO_DATE >= @d"
                        + "     and tblEmailNoteCredit.RefSAGEDocument is null";
                    //Filters
                    cmd.Parameters.Add("@d", SqlDbType.DateTime).Value = d;
                    break;
                case "ContactModificationRequest":
                    sqlStr = "select tblAction.RefAction, tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                        + "     , tblAction.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MessageLibelle.ToString()].Name + "]"
                        + " from tblAction"
                        + "     inner join tblEntite on tblAction.RefEntite=tblEntite.RefEntite"
                        + "     left join tbmContactAdresse on tblAction.RefContactAdresse=tbmContactAdresse.RefContactAdresse"
                        + "     left join tblContact on tbmContactAdresse.RefContact=tblContact.RefContact"
                        + " where DAction is null and tblAction.Libelle like 'Demande de modification d%' and RefFormeContact=6";
                    if (CurrentContext.filterDR)
                    {
                        sqlStr += " and tblEntite.RefEntite in (select RefEntite from tbmEntiteDR where RefDR =" + CurrentContext.RefUtilisateur + ")";
                    }
                    break;
                case "EntiteForDocument":
                    sqlStr = "select tblEntite.RefEntite as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefEntite.ToString()].Name + "]"
                        + "     , tblEntite.CodeEE as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name + "]"
                        + "     , tblEntite.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name + "]"
                        + "     , tbrEntiteType.Libelle as [" + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTypeLibelle.ToString()].Name + "]"
                        + " from tblEntite"
                        + "     left join tbrEntiteType on tblEntite.RefEntiteType=tbrEntiteType.RefEntiteType"
                        + " where 1=1";
                    //Filters
                    sqlStr = Utils.Utils.CreateSQLTextFilter(CurrentContext, cmd, sqlStr, filterText
                        , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteLibelle }
                        , new List<Enumerations.DataColumnName> { Enumerations.DataColumnName.EntiteCodeCITEO }
                        );
                    if (CurrentContext.filterGlobalActif)
                    {
                        sqlStr += " and tblEntite.Actif=1";
                    }
                    break;
            }
            //Chargement des données si elles existent
            if (sqlStr != "")
            {
                var nbRow = "0";
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    //Get the total row count
                    cmd.CommandText = "select count(*) from (" + sqlStr + ") as univers";
                    nbRow = cmd.ExecuteScalar().ToString();
                    //Get formatted data page
                    sqlStr = " set language French; " + sqlStr + " ORDER BY " + sortExpression + " " + sortOrder + " OFFSET (@pageNumber  * @pageSize) ROWS FETCH NEXT @pageSize ROWS ONLY";
                    cmd.CommandText = sqlStr;
                    cmd.Parameters.Add("@pageNumber", SqlDbType.Int).Value = pageNumber;
                    cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                    SqlDataAdapter dA = new(cmd);
                    if (dS.Tables.Count > 0) { dS.Tables.Clear(); }
                    dA.Fill(dS);
                }
                //Return nb of row in header
                Response.Headers.Add("nbRow", nbRow);
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

