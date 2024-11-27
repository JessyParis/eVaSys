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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class ExportSAGEController : BaseApiController
    {
        #region Constructor
        public ExportSAGEController(ApplicationDbContext context, IConfiguration configuration)
            : base(context, configuration) { }
        #endregion Constructor
        #region Attribute-based Routing
        /// <summary>
        /// GET: api/exportsage/setachatventefilter
        /// ROUTING TYPE: attribute-based
        /// create text file for SAGE : achats/ventes
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("setachatventefilter")]
        public IActionResult SetAchatVenteFilter()
        {
            string month = Request.Headers["month"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int m = 0, y = 0;
            int.TryParse(month, out m);
            int.TryParse(year, out y);
            if (m != 0 && y != 0)
            {
                //Generate filter
                var numAffs = DbContext.CommandeFournisseurs.Where(el =>
                  el.DDechargement != null && ((DateTime)el.DDechargement).Month == m && ((DateTime)el.DDechargement).Year == y && el.ExportSAGE != true)
                    .Select(p => p.NumeroAffretement)
                    .Distinct()
                    .ToList();
                if (numAffs.Count > 0)
                {
                    Context ctx = CurrentContext;
                    ctx.achatVenteFilter = string.Join(",", numAffs);
                    HttpContext.Session.SetObjectAsJson("CurrentContext", ctx);
                    return Ok();
                }
                return Conflict(new ConflictError(CurrentContext.CulturedRessources.GetTextRessource(318)));
            }
            else
                return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(617)));
        }
        
        /// <summary>
        /// GET: api/exportsage/exportachat
        /// ROUTING TYPE: attribute-based
        /// create text file for SAGE : achat
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("achat")]
        public IActionResult ExportAchat()
        {
            string month = Request.Headers["month"].ToString();
            string quarter = Request.Headers["quarter"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int m = 0, q = 0, y = 0;
            int.TryParse(month, out m);
            int.TryParse(quarter, out q);
            int.TryParse(year, out y);
            if (m != 0 && y != 0 && !string.IsNullOrEmpty(CurrentContext.achatVenteFilter))
            {
                //Generate text file
                MemoryStream mS = new();
                Encoding sageEnc = Encoding.GetEncoding("ibm850");
                StreamWriter sw = new(mS, sageEnc);
                //Initialisations
                sw.WriteLine("#FLG 000");
                sw.WriteLine("#VER 8");
                //Traitements
                //Chargement des données
                string sqlStr = "select tblCommandeFournisseur.NumeroAffretement, DDechargement, transporteur.SAGECodeComptable, isnull(fournisseur.RefExt,fournisseur.RefEntite), client.RefSAGEConditionLivraison, tbrSAGECategorieAchat.RefSAGECategorieAchat"
                    + "     , tbrSAGECodeTransport.Code, tbrSAGECodeTransport.Libelle, adresseOrigine.Ville, adresseDestination.Ville, prix"
                    + "     , poids*1000, isnull(TVATaux,0), transporteur.refSAGEModeReglement"
                    + " from tblCommandeFournisseur"
                    + " inner join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                    + " inner join tblEntite as transporteur on transporteur.RefEntite=tblCommandeFournisseur.RefTransporteur"
                    + " inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + " left join tbrSAGECodeTransport on tblProduit.RefSAGECodeTransport=tbrSAGECodeTransport.RefSAGECodeTransport"
                    + " inner join tblAdresse as AdresseOrigine on tblCommandeFournisseur.RefAdresse=adresseOrigine.RefAdresse"
                    + " inner join tblAdresse as AdresseDestination on tblCommandeFournisseur.RefAdresseClient=adresseDestination.RefAdresse"
                    + " inner join tblEntite as client on client.RefEntite=adresseDestination.RefEntite"
                    + " left join tbrSAGECategorieAchat on transporteur.RefSAGECategorieAchat=tbrSAGECategorieAchat.RefSAGECategorieAchat"
                    + " inner join (select NumeroAffretement, sum(PrixTransportHT+PrixTransportSupplementHT+SurcoutCarburantHT) as prix, sum(PoidsDechargement) as poids from tblCommandeFournisseur group by NumeroAffretement) as prix"
                    + "     on tblCommandeFournisseur.NumeroAffretement=prix.NumeroAffretement"
                    + " where OrdreAffretement=1 and tblCommandeFournisseur.NumeroAffretement in(" + CurrentContext.achatVenteFilter + ")";
                using (SqlConnection sqlConn = (SqlConnection)DbContext.Database.GetDbConnection())
                {
                    SqlCommand cmd = new();
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string numeroAffretement = dr.GetSqlInt32(0).ToString();
                        if (numeroAffretement?.Length > 8) { numeroAffretement = numeroAffretement.Substring(numeroAffretement.Length - 8, 8); }
                        string dDechargement = ((DateTime)dr.GetSqlDateTime(1)).ToString("ddMMyy");
                        string transporteurSAGECodeComptable = dr.GetSqlString(2).ToString();
                        string refFournisseur = dr.GetSqlString(3).ToString();
                        string clientSAGEConditionLivraison = dr.GetSqlInt32(4).ToString();
                        string transporteurRefSAGECategorieAchat = dr.GetSqlInt32(5).ToString();
                        string SAGECodeTransport = dr.GetSqlString(6).ToString();
                        string SAGELibelleTransport = dr.GetSqlString(7).ToString();
                        string villeOrigine = dr.GetSqlString(8).ToString();
                        string villeDestination = dr.GetSqlString(9).ToString();
                        string prixTransport = ((decimal)dr.GetSqlDecimal(10)).ToString("0.00");
                        string poidsDechargement = dr.GetSqlInt32(11).ToString();
                        string tvaTaux = ((decimal)dr.GetDecimal(12)).ToString("0.0000");
                        string transporteurRefSAGEModeReglement = dr.GetSqlInt32(13).ToString();
                        //Traitement d'un affretement
                        sw.WriteLine("#CHEN");
                        sw.WriteLine("2");
                        sw.WriteLine("4");
                        sw.WriteLine("1");
                        sw.WriteLine(numeroAffretement);
                        sw.WriteLine(dDechargement);
                        sw.WriteLine(numeroAffretement);
                        sw.WriteLine(dDechargement);
                        sw.WriteLine(transporteurSAGECodeComptable);
                        sw.WriteLine(refFournisseur);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0,000000");
                        sw.WriteLine(transporteurSAGECodeComptable);
                        sw.WriteLine("1");
                        sw.WriteLine(clientSAGEConditionLivraison);
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("21");
                        sw.WriteLine("11");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0,0000");
                        sw.WriteLine(transporteurRefSAGECategorieAchat);
                        sw.WriteLine("0");
                        sw.WriteLine("2");
                        sw.WriteLine("4011000000");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine("#CHLI");
                        sw.WriteLine(numeroAffretement);
                        sw.WriteLine(SAGECodeTransport);
                        sw.WriteLine(SAGELibelleTransport);
                        sw.WriteLine("De " + villeOrigine + " vers " + villeDestination);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine(prixTransport);
                        sw.WriteLine("0");
                        sw.WriteLine("1");
                        sw.WriteLine("1");
                        sw.WriteLine();
                        sw.WriteLine(poidsDechargement);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(dDechargement);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("1");
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine(tvaTaux);
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine(transporteurSAGECodeComptable);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("#CHRE");
                        sw.WriteLine("1");
                        sw.WriteLine(dDechargement);
                        sw.WriteLine();
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine(transporteurRefSAGEModeReglement);
                    }
                    //Fin
                    sw.WriteLine("#FIN");
                    sw.Flush();
                    //Fermeture de la source de données
                    dr.Close();
                    //Mise à jour des commandes
                    cmd.CommandText = "update tblCommandeFournisseur set ExportSAGE=1 where NumeroAffretement in(" + CurrentContext.achatVenteFilter + ")";
                    cmd.ExecuteNonQuery();
                }
                //return file
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = "Achats" + DateTime.Now.ToString("yyMM") + ".txt",
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
        /// GET: api/exportsage/exportvente
        /// ROUTING TYPE: attribute-based
        /// create text file for SAGE : vente
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("vente")]
        public IActionResult ExportVente()
        {
            string month = Request.Headers["month"].ToString();
            string quarter = Request.Headers["quarter"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int m = 0, q = 0, y = 0;
            int.TryParse(month, out m);
            int.TryParse(quarter, out q);
            int.TryParse(year, out y);
            if (m != 0 && y != 0 && !string.IsNullOrEmpty(CurrentContext.achatVenteFilter))
            {
                //Generate text file
                MemoryStream mS = new();
                Encoding sageEnc = Encoding.GetEncoding("ibm850");
                StreamWriter sw = new(mS, sageEnc);
                //Initialisations
                sw.WriteLine("#FLG 000");
                sw.WriteLine("#VER 8");
                //Traitements
                //Chargement des données
                string sqlStr = "select tblCommandeFournisseur.NumeroCommande, DDechargement, client.SAGECodeComptable, isnull(fournisseur.RefExt,fournisseur.RefEntite), adresseDestination.Libelle, client.RefSAGEPeriodicite"
                    + "     , client.RefSAGEConditionLivraison, client.RefSAGECategorieVente, client.SAGECompteTiers, isnull(tblProduit._PCodeProd, tblProduit.RefProduit), tblProduit.Libelle"
                    + "     , tblCommandeFournisseur.NumeroAffretement, PrixTonneHT, cast(PoidsDechargement as decimal(10,3))/1000, PoidsDechargement*1000, isnull(TVATaux,0), client.refSAGEModeReglement"
                    + "     , case when tblCommandeFournisseur.RefPAys = 1 then isnull(tblCommandeFournisseur.CodePostal,'') else '' end as adresseOrigineCodePostal"
                    + " from tblCommandeFournisseur"
                    + " inner join tblEntite as fournisseur on fournisseur.RefEntite=tblCommandeFournisseur.RefEntite"
                    + " inner join tblEntite as transporteur on transporteur.RefEntite=tblCommandeFournisseur.RefTransporteur"
                    + " inner join tblProduit on tblProduit.RefProduit=tblCommandeFournisseur.RefProduit"
                    + " inner join tblAdresse as AdresseOrigine on tblCommandeFournisseur.RefAdresseClient=adresseOrigine.RefAdresse"
                    + " inner join tblAdresse as AdresseDestination on tblCommandeFournisseur.RefAdresseClient=adresseDestination.RefAdresse"
                    + " inner join tblEntite as client on client.RefEntite=adresseDestination.RefEntite"
                    + " left join tbrSAGECategorieVente on client.RefSAGECategorieVente=tbrSAGECategorieVente.RefSAGECategorieVente"
                    + " where tblCommandeFournisseur.NumeroAffretement in(" + CurrentContext.achatVenteFilter + ")";
                using (SqlConnection sqlConn = (SqlConnection)DbContext.Database.GetDbConnection())
                {
                    SqlCommand cmd = new();
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string numeroCommande = dr.GetSqlInt32(0).ToString();
                        if (numeroCommande?.Length > 8) { numeroCommande = numeroCommande.Substring(numeroCommande.Length - 8, 8); }
                        string dDechargement = ((DateTime)dr.GetSqlDateTime(1)).ToString("ddMMyy");
                        string clientSAGECodeComptable = dr.GetSqlString(2).ToString();
                        string refFournisseur = dr.GetSqlString(3).ToString();
                        string adresseDestinationLibelle = dr.GetSqlString(4).ToString();
                        string clientRefSAGEPeriodicite = dr.GetSqlInt32(5).ToString();
                        string clientRefSAGEConditionLivraison = dr.GetSqlInt32(6).ToString();
                        string clientRefSAGECategorieVente = dr.GetSqlInt32(7).ToString();
                        string clientSAGECompteTiers = dr.GetSqlString(8).ToString();
                        string refProduit = dr.GetSqlString(9).ToString();
                        string produitLibelle = dr.GetSqlString(10).ToString();
                        string numeroAffretement = dr.GetSqlInt32(11).ToString();
                        if (numeroAffretement?.Length > 8) { numeroAffretement = numeroAffretement.Substring(numeroAffretement.Length - 8, 8); }
                        string prixVente = ((decimal)dr.GetSqlDecimal(12)).ToString("0.00");
                        string poidsDechargementTonne = ((decimal)dr.GetSqlDecimal(13)).ToString("0.000");
                        string poidsDechargementGramme = dr.GetSqlInt32(14).ToString();
                        string tvaTaux = ((decimal)dr.GetSqlDecimal(15)).ToString("0.0000");
                        string clientRefSAGEModeReglement = dr.GetSqlInt32(16).ToString();
                        string adresseOrigineCodePostal = dr.GetSqlString(17).ToString();
                        if (adresseOrigineCodePostal.Length == 5)
                        {
                            adresseOrigineCodePostal = adresseOrigineCodePostal.Substring(0, 2);
                        }
                        else
                        {
                            adresseOrigineCodePostal = "";
                        }
                        //Traitement d'un affretement
                        sw.WriteLine("#CHEN");
                        sw.WriteLine("1");
                        sw.WriteLine("3");
                        sw.WriteLine("1");
                        sw.WriteLine(numeroCommande);
                        sw.WriteLine(dDechargement);
                        sw.WriteLine(numeroCommande);
                        sw.WriteLine(dDechargement);
                        sw.WriteLine(clientSAGECodeComptable);
                        sw.WriteLine(refFournisseur);
                        sw.WriteLine(adresseDestinationLibelle);
                        sw.WriteLine(clientRefSAGEPeriodicite);
                        sw.WriteLine("0");
                        sw.WriteLine("0,000000");
                        sw.WriteLine(clientSAGECodeComptable);
                        sw.WriteLine("1");
                        sw.WriteLine(clientRefSAGEConditionLivraison);
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("1");
                        sw.WriteLine("21");
                        sw.WriteLine("11");
                        sw.WriteLine();
                        sw.WriteLine("1");
                        sw.WriteLine("1");
                        sw.WriteLine("0");
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0,0000");
                        sw.WriteLine(clientRefSAGECategorieVente);
                        sw.WriteLine("0");
                        sw.WriteLine("2");
                        sw.WriteLine(clientSAGECompteTiers);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine("#CHLI");
                        sw.WriteLine(numeroCommande);
                        sw.WriteLine(refProduit);
                        sw.WriteLine(produitLibelle);
                        sw.WriteLine(numeroAffretement);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine(prixVente);
                        sw.WriteLine("0");
                        sw.WriteLine(poidsDechargementTonne);
                        sw.WriteLine("1");
                        sw.WriteLine("Balles");
                        sw.WriteLine(poidsDechargementGramme);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(dDechargement);
                        sw.WriteLine(refFournisseur);
                        sw.WriteLine();
                        sw.WriteLine("1");
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine(tvaTaux);
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine(clientSAGECodeComptable);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("#CIVA");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(adresseOrigineCodePostal);
                        sw.WriteLine("#CHRE");
                        sw.WriteLine("1");
                        sw.WriteLine(dDechargement);
                        sw.WriteLine();
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine(clientRefSAGEModeReglement);
                    }
                    //Fin
                    sw.WriteLine("#FIN");
                    sw.Flush();
                    //Fermeture de la source de données
                    dr.Close();
                    //Mise à jour des commandes
                    cmd.CommandText = "update tblCommandeFournisseur set ExportSAGE=1 where NumeroAffretement in(" + CurrentContext.achatVenteFilter + ")";
                    cmd.ExecuteNonQuery();
                }
                //return file
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = "Ventes" + DateTime.Now.ToString("yyMM") + ".txt",
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
        /// GET: api/exportsage/notecredit
        /// ROUTING TYPE: attribute-based
        /// create text file for SAGE : note de credit
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("notecredit")]
        public IActionResult ExportNoteCredit()
        {
            string month = Request.Headers["month"].ToString();
            string quarter = Request.Headers["quarter"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int m = 0, q = 0, y = 0;
            int.TryParse(month, out m);
            int.TryParse(quarter, out q);
            int.TryParse(year, out y);
            int i = 1;
            if (q != 0 && y != 0)
            {
                Quarter t = new(q, y, CurrentContext.CurrentCulture);
                //Generate text file
                MemoryStream mS = new();
                Encoding sageEnc = Encoding.GetEncoding("ibm850");
                StreamWriter sw = new(mS, sageEnc);
                //Initialisations
                sw.WriteLine("#FLG 000");
                sw.WriteLine("#VER 10");
                //Traitements
                //Chargement des données
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    SqlCommand cmd = new();
                    string sqlStr = "select dbo.AdresseEnAd(tblEntite.RefEntite), tblEntite.SAGECodeComptable, sum(cast(Poids as decimal))/1000, sum(cast(Poids as decimal(10,2))*PUHT)/sum(cast(Poids as decimal)), max(rep.D)"
                        + " from"
                        + "     ("
                        + "         select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D "
                        + "         from tblRepartition "
                        + " 	        inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        + "         union all"
                        + "         select RefRepartition, RefFournisseur, RefProduit, D "
                        + "         from tblRepartition"
                        + "         where RefCommandeFournisseur is null"
                        + "     ) as rep"
                        + "     inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition"
                        + "     inner join tblEntite on tblRepartitionCollectivite.RefCollectivite=tblEntite.RefEntite"
                        + " where rep.D between @debut and @fin"
                        + " group by TblEntite.refEntite, TblEntite.SAGECodeComptable"
                        + " order by TblEntite.refEntite";
                    cmd.Parameters.Add("@debut", SqlDbType.DateTime).Value = t.Begin; 
                    cmd.Parameters.Add("@fin", SqlDbType.DateTime).Value = t.End; 
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        //Instanciation de l'adresse
                        Adresse a = DbContext.Adresses.Where(el => el.RefAdresse == (int)dr.GetSqlInt32(0)).FirstOrDefault();
                        Entite e = DbContext.Entites.Where(el => el.RefEntite == a.RefEntite).FirstOrDefault();
                        string r = dr.GetSqlString(1).ToString();
                        DateTime d = (DateTime)dr.GetSqlDateTime(4);
                        //Traitement d'un affretement
                        sw.WriteLine("#CHEN");
                        sw.WriteLine("2");
                        sw.WriteLine("4");
                        sw.WriteLine("1");
                        sw.WriteLine("5");
                        sw.WriteLine(t.Begin.ToString("yy") + "T" + t.Nb + i.ToString("0000"));
                        sw.WriteLine(t.End.ToString("ddMMyy"));
                        sw.WriteLine(t.NameShort);
                        sw.WriteLine(t.End.ToString("ddMMyy"));
                        sw.WriteLine(r);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0,000000");
                        sw.WriteLine(r);
                        sw.WriteLine("1");
                        sw.WriteLine("2");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(a.Libelle?.Length > 25 ? Utils.Utils.convertirChaineSansAccent(a.Libelle.Substring(0, 25)) : Utils.Utils.convertirChaineSansAccent(a.Libelle));
                        sw.WriteLine(a.Adr1?.Length > 25 ? Utils.Utils.convertirChaineSansAccent(a.Adr1.Substring(0, 25)) : Utils.Utils.convertirChaineSansAccent(a.Adr1));
                        sw.WriteLine(a.Adr2?.Length > 25 ? Utils.Utils.convertirChaineSansAccent(a.Adr2.Substring(0, 25)) : Utils.Utils.convertirChaineSansAccent(a.Adr2));
                        sw.WriteLine((a.CodePostal + " " + a.Ville)?.Length > 25 ? Utils.Utils.convertirChaineSansAccent((a.CodePostal + " " + a.Ville).Substring(0, 25)) : Utils.Utils.convertirChaineSansAccent(a.CodePostal + " " + a.Ville));
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("21");
                        sw.WriteLine("11");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0,0000");
                        sw.WriteLine(e.AssujettiTVA == true ? 6 : 7);
                        sw.WriteLine("0");
                        sw.WriteLine("2");
                        sw.WriteLine("4011000000");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine("#CIVA");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(a.Libelle?.Length > 69 ? Utils.Utils.convertirChaineSansAccent(a.Libelle.Substring(0, 69)) : Utils.Utils.convertirChaineSansAccent(a.Libelle));
                        sw.WriteLine(a.Adr1?.Length > 69 ? Utils.Utils.convertirChaineSansAccent(a.Adr1.Substring(0, 69)) : Utils.Utils.convertirChaineSansAccent(a.Adr1));
                        sw.WriteLine(a.Adr2?.Length > 69 ? Utils.Utils.convertirChaineSansAccent(a.Adr2.Substring(0, 69)) : Utils.Utils.convertirChaineSansAccent(a.Adr2));
                        sw.WriteLine((a.CodePostal + " " + a.Ville)?.Length > 69 ? Utils.Utils.convertirChaineSansAccent((a.CodePostal + " " + a.Ville).Substring(0, 69)) : Utils.Utils.convertirChaineSansAccent(a.CodePostal + " " + a.Ville));
                        sw.WriteLine("#CHLI");
                        sw.WriteLine();
                        sw.WriteLine("COL" + t.Begin.ToString("yy") + d.ToString("MM"));
                        sw.WriteLine("REPRISE DES EMBALLAGES PLASTIQUES " + t.NameShort);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine(((decimal)dr.GetSqlDecimal(3)).ToString("0.00"));
                        sw.WriteLine("0");
                        sw.WriteLine(((decimal)dr.GetSqlDecimal(2)).ToString("0.000"));
                        sw.WriteLine(((decimal)dr.GetSqlDecimal(2)).ToString("0.000"));
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("1");
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine(e.AssujettiTVA == true ? e.SAGECategorieAchat.TVATaux.ToString("0.0000") : "0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("1");
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine(r);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("#CHRE");
                        sw.WriteLine("1");
                        sw.WriteLine(t.End.AddYears(5).ToString("ddMMyy"));
                        sw.WriteLine();
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("1");
                        sw.WriteLine("0");
                        //Suivant
                        i++;
                    }
                    //Fin
                    sw.WriteLine("#FIN");
                    sw.Flush();
                    //Fermeture de la source de données
                    dr.Close();
                }
                //return file
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = "NotesCredit" + DateTime.Now.ToString("yyMM") + ".txt",
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
        /// GET: api/exportsage/notecredit
        /// ROUTING TYPE: attribute-based
        /// create text file for SAGE : note de credit incitation qualité
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("notecreditincitationqualite")]
        public IActionResult ExportNoteCreditIncitationQualite()
        {
            string month = Request.Headers["month"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int m = 0, y = 0;
            int.TryParse(month, out m);
            int.TryParse(year, out y);
            int i = 1;
            if (y != 0)
            {
                //Generate text file
                MemoryStream mS = new();
                Encoding sageEnc = Encoding.GetEncoding("ibm850");
                StreamWriter sw = new(mS, sageEnc);
                DateTime dDebut = new(y, 1, 1);
                DateTime dFin = new(y, 12, 31);
                //Initialisations
                sw.WriteLine("#FLG 000");
                sw.WriteLine("#VER 10");
                //Traitements
                //Chargement des données
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    SqlCommand cmd = new();
                    cmd.Parameters.Add("@annee", SqlDbType.Int).Value = y;
                    //Chargement des données
                    string sqlStr = "select dbo.AdresseEnAd(tblEntite.RefEntite) as RefAdresse, tblEntite.SAGECodeComptable"
                        + " 	, sum(case when rec.RefCommandeFournisseur is null then cast(PoidsChargement as decimal)/1000 else cast(0 as decimal) end) as PoidsChargement"
                        + " 	, case when count(rec.RefReclamation)*50 <= count(tblCommandeFournisseur.RefCommandeFournisseur) then Mnt.Montant else 0 end as Montant"
                        + " from tblCommandeFournisseur"
                        + "     inner join tblEntite on tblCommandeFournisseur.RefEntite = tblEntite.RefEntite"
                        + "     inner join tblProduit on tblCommandeFournisseur.RefProduit = tblProduit.RefProduit"
                        + " left join"
                        + " (select distinct RefReclamation, RefCommandeFournisseur"
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
                        + " , (select Montant from tbrMontantIncitationQualite where Annee=@annee) as mnt"
                        + " where case when tblCommandeFournisseur.RefusCamion=1 then year(tblCommandeFournisseur.DMoisDechargementPrevu) else year(tblCommandeFournisseur.DChargement) end = @annee"
                        + "     and tblEntite.RefEntite in (select RefEntite from tblContratIncitationQualite where year(DDebut) = @annee)"
                        + "     and tblEntite.AutoControle = 1"
                        + "     and tblProduit.IncitationQualite = 1"
                        + " group by TblEntite.refEntite, TblEntite.SAGECodeComptable, Mnt.Montant"
                        + " having case when count(rec.RefReclamation)*50 <= count(tblCommandeFournisseur.RefCommandeFournisseur) then Mnt.Montant else 0 end > 0"
                        + " order by TblEntite.refEntite";
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        //Instanciation de l'adresse
                        Adresse a = DbContext.Adresses.Where(el => el.RefAdresse == (int)dr.GetSqlInt32(0)).FirstOrDefault();
                        Entite e = DbContext.Entites.Where(el => el.RefEntite == a.RefEntite).FirstOrDefault();
                        string r = dr.GetSqlString(1).ToString();
                        //Traitement d'un affretement
                        sw.WriteLine("#CHEN");
                        sw.WriteLine("2");
                        sw.WriteLine("4");
                        sw.WriteLine("1");
                        sw.WriteLine("4"); //Type de facture : facture CDT
                        sw.WriteLine(dDebut.ToString("yyyy") + i.ToString("0000"));
                        sw.WriteLine(dFin.ToString("ddMMyy"));
                        sw.WriteLine("IQUA" + dDebut.ToString("yyyy"));
                        sw.WriteLine(dFin.ToString("ddMMyy"));
                        sw.WriteLine(r);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0,000000");
                        sw.WriteLine(r);
                        sw.WriteLine("1");
                        sw.WriteLine("2");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(a.Libelle?.Length > 25 ? Utils.Utils.convertirChaineSansAccent(a.Libelle.Substring(0, 25)) : Utils.Utils.convertirChaineSansAccent(a.Libelle));
                        sw.WriteLine(a.Adr1?.Length > 25 ? Utils.Utils.convertirChaineSansAccent(a.Adr1.Substring(0, 25)) : Utils.Utils.convertirChaineSansAccent(a.Adr1));
                        sw.WriteLine(a.Adr2?.Length > 25 ? Utils.Utils.convertirChaineSansAccent(a.Adr2.Substring(0, 25)) : Utils.Utils.convertirChaineSansAccent(a.Adr2));
                        sw.WriteLine((a.CodePostal + " " + a.Ville)?.Length > 25 ? Utils.Utils.convertirChaineSansAccent((a.CodePostal + " " + a.Ville).Substring(0, 25)) : Utils.Utils.convertirChaineSansAccent(a.CodePostal + " " + a.Ville));
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("21");
                        sw.WriteLine("11");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0,0000");
                        sw.WriteLine(e.AssujettiTVA == true ? 6 : 7);
                        sw.WriteLine("0");
                        sw.WriteLine("2");
                        sw.WriteLine("4011000000");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine("#CIVA");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(a.Libelle?.Length > 69 ? Utils.Utils.convertirChaineSansAccent(a.Libelle.Substring(0, 69)) : Utils.Utils.convertirChaineSansAccent(a.Libelle));
                        sw.WriteLine(a.Adr1?.Length > 69 ? Utils.Utils.convertirChaineSansAccent(a.Adr1.Substring(0, 69)) : Utils.Utils.convertirChaineSansAccent(a.Adr1));
                        sw.WriteLine(a.Adr2?.Length > 69 ? Utils.Utils.convertirChaineSansAccent(a.Adr2.Substring(0, 69)) : Utils.Utils.convertirChaineSansAccent(a.Adr2));
                        sw.WriteLine((a.CodePostal + " " + a.Ville)?.Length > 69 ? Utils.Utils.convertirChaineSansAccent((a.CodePostal + " " + a.Ville).Substring(0, 69)) : Utils.Utils.convertirChaineSansAccent(a.CodePostal + " " + a.Ville));
                        sw.WriteLine("#CHLI");
                        sw.WriteLine();
                        sw.WriteLine("IQUA" + dDebut.ToString("yyyy"));
                        sw.WriteLine("INTERESSEMENT INCITATION AUTOCONTROLE " + dDebut.ToString("yyyy"));
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine(((decimal)dr.GetSqlDecimal(3)).ToString("0.00"));
                        sw.WriteLine("0");
                        sw.WriteLine(((decimal)dr.GetSqlDecimal(2)).ToString("0.000"));
                        sw.WriteLine(((decimal)dr.GetSqlDecimal(2)).ToString("0.000"));
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("1");
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine(e.AssujettiTVA == true ? e.SAGECategorieAchat.TVATaux.ToString("0.0000") : "0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("1");
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0,0000");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine(r);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("#CHRE");
                        sw.WriteLine("1");
                        sw.WriteLine(dFin.AddYears(5).ToString("ddMMyy"));
                        sw.WriteLine();
                        sw.WriteLine("0,00");
                        sw.WriteLine("0,00");
                        sw.WriteLine("1");
                        sw.WriteLine("0");
                        //Suivant
                        i++;
                    }
                    //Fin
                    sw.WriteLine("#FIN");
                    sw.Flush();
                    //Fermeture de la source de données
                    dr.Close();
                }
                //return file
                var cD = new System.Net.Mime.ContentDisposition
                {
                    FileName = "NotesCreditIncitationQualite" + DateTime.Now.ToString("yyMM") + ".txt",
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
        /// GET: api/exportsage/notecredit
        /// ROUTING TYPE: attribute-based
        /// create text file for SAGE : collectivités
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("collectivite")]
        public IActionResult ExportCollectivite()
        {
            string month = Request.Headers["month"].ToString();
            string quarter = Request.Headers["quarter"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int m = 0, q = 0, y = 0;
            int.TryParse(month, out m);
            int.TryParse(quarter, out q);
            int.TryParse(year, out y);
            if (q != 0 && y != 0)
            {
                Quarter t = new(q, y, CurrentContext.CurrentCulture);
                //Generate text file
                MemoryStream mS = new();
                Encoding sageEnc = Encoding.GetEncoding("ibm850");
                StreamWriter sw = new(mS, sageEnc);
                string fileName = "Collectivites" + DateTime.Now.ToString("yyMM") + ".txt";
                //Initialisations
                sw.WriteLine("#FLG 000");
                sw.WriteLine("#VER 10");
                //Chargement des données
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    SqlCommand cmd = new();
                    cmd.Parameters.Add("@debut", SqlDbType.DateTime).Value = t.Begin;
                    cmd.Parameters.Add("@fin", SqlDbType.DateTime).Value = t.End;
                    //Chargement des données
                    string sqlStr = "select distinct tblAdresse.RefAdresse, administrative.Libelle, tblEntite.SAGECodeComptable"
                        + " from tblAdresse"
                        + "     inner join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                        + "     inner join (select RefEntite, min(RefAdresse) as RefAdresse from tblAdresse where RefAdresseType=1 group by RefEntite) as ad on tblEntite.RefEntite=ad.RefEntite"
                        + "     inner join tblAdresse as administrative on ad.RefAdresse=administrative.RefAdresse"
                        + " where tblAdresse.RefAdresse in("
                        + "     select distinct dbo.AdresseEnBeAd(tblEntite.RefEntite)"
                        + "     from tblEntite"
                        + "     inner join tblContratCollectivite on tblEntite.RefEntite=tblContratCollectivite.RefEntite"
                        + "     where ExportSAGE=0 and SAGECodeComptable is not null and RefEntiteType=1 and (@debut between DDebut and DFin or @fin between DDebut and DFin))";
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (!dr.HasRows) { fileName = "nothing"; }
                    while (dr.Read())
                    {
                        //Instanciation de l'adresse
                        Adresse a = DbContext.Adresses.Where(el => el.RefAdresse == (int)dr.GetSqlInt32(0)).FirstOrDefault();
                        Entite e = DbContext.Entites.Where(el => el.RefEntite == a.RefEntite).FirstOrDefault();
                        e.ExportSAGE = true;
                        //Traitement d'un affretement
                        sw.WriteLine("#CFOU");
                        sw.WriteLine(dr.GetSqlString(2).ToString());
                        sw.WriteLine("4011000000");
                        sw.WriteLine();
                        sw.WriteLine(a.Libelle?.Length > 35 ? Utils.Utils.convertirChaineSansAccent(a.Libelle.Substring(0, 35)) : Utils.Utils.convertirChaineSansAccent(a.Libelle));
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(a.Adr1?.Length > 35 ? Utils.Utils.convertirChaineSansAccent(a.Adr1.Substring(0, 35)) : Utils.Utils.convertirChaineSansAccent(a.Adr1));
                        sw.WriteLine(a.Adr2?.Length > 35 ? Utils.Utils.convertirChaineSansAccent(a.Adr2.Substring(0, 35)) : Utils.Utils.convertirChaineSansAccent(a.Adr2));
                        sw.WriteLine(Utils.Utils.convertirChaineSansAccent(a.CodePostal));
                        sw.WriteLine(a.Ville?.Length > 35 ? Utils.Utils.convertirChaineSansAccent(a.Ville.Substring(0, 35)) : Utils.Utils.convertirChaineSansAccent(a.Ville));
                        for (int i = 0; i < 18; i++)
                        {
                            sw.WriteLine();
                        }
                        sw.WriteLine(e.AssujettiTVA == true ? 6 : 7);
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(e.CodeTVA);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(dr.GetSqlString(1).ToString()?.Length > 35 ? Utils.Utils.convertirChaineSansAccent(dr.GetSqlString(1).ToString().Substring(0, 35)) : Utils.Utils.convertirChaineSansAccent(dr.GetSqlString(1).ToString()));
                        sw.WriteLine("1");
                        sw.WriteLine("2");
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("1");
                        sw.WriteLine();
                        sw.WriteLine(DateTime.Now.ToString("ddMMyy"));
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("4011000000");
                        sw.WriteLine("#CRLT");
                        sw.WriteLine("5");
                        sw.WriteLine("0");
                        sw.WriteLine("999");
                        sw.WriteLine("28");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("1");
                        sw.WriteLine();
                    }
                    //Fin
                    sw.WriteLine("#FIN");
                    sw.Flush();
                    //Fermeture de la source de données
                    dr.Close();
                    //Mise à jour des codes comptables
                    cmd.CommandText = "update tblEntite set SAGECodeComptable='FC' + isnull(tblEntite.RefExt, cast(tblEntite.RefEntite as nvarchar(50)))"
                        + " where RefEntiteType=1 and CodeEE is not null and SAGECodeComptable is null";
                    cmd.ExecuteNonQuery();
                }
                //Save entities
                DbContext.SaveChanges();
                //return file
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
        /// GET: api/exportsage/notecredit
        /// ROUTING TYPE: attribute-based
        /// create text file for SAGE : centres de tri
        /// </summary>
        /// <returns>Text file</returns>
        [HttpGet("centredetri")]
        public IActionResult ExportCentreDeTri()
        {
            string month = Request.Headers["month"].ToString();
            string year = Request.Headers["year"].ToString();
            //Check parameters
            int m = 0, y = 0;
            int.TryParse(month, out m);
            int.TryParse(year, out y);
            if (y != 0)
            {
                //Generate text file
                MemoryStream mS = new();
                Encoding sageEnc = Encoding.GetEncoding("ibm850");
                StreamWriter sw = new(mS, sageEnc);
                string fileName ="CentresDeTri" + DateTime.Now.ToString("yyMM") + ".txt";
                DateTime dDebut = new(y, 1, 1);
                DateTime dFin = new(y, 12, 31);
                //Initialisations
                sw.WriteLine("#FLG 000");
                sw.WriteLine("#VER 10");
                //Traitements
                //Chargement des données
                using (SqlConnection sqlConn = new(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    SqlCommand cmd = new();
                    cmd.Parameters.Add("@debut", SqlDbType.DateTime).Value = dDebut;
                    cmd.Parameters.Add("@fin", SqlDbType.DateTime).Value = dFin;
                    //Chargement des données
                    string sqlStr = "select tblAdresse.RefAdresse, administrative.Libelle, tblEntite.SAGECodeComptable"
                        + " from tblAdresse"
                        + "     inner join tblEntite on tblAdresse.RefEntite=tblEntite.RefEntite"
                        + "     inner join (select RefEntite, min(RefAdresse) as RefAdresse from tblAdresse where RefAdresseType=1 group by RefEntite) as ad on tblEntite.RefEntite=ad.RefEntite"
                        + "     inner join tblAdresse as administrative on ad.RefAdresse=administrative.RefAdresse"
                        + " where tblAdresse.RefAdresse in("
                        + "     select distinct dbo.AdresseEnBeAd(tblEntite.RefEntite)"
                        + "     from tblEntite"
                        + "     inner join tblContratIncitationQualite on tblEntite.RefEntite=tblContratIncitationQualite.RefEntite"
                        + "     where ExportSAGE=0 and SAGECodeComptable is not null and RefEntiteType=3 and (@debut between DDebut and DFin or @fin between DDebut and DFin))";
                    sqlConn.Open();
                    cmd.Connection = sqlConn;
                    cmd.CommandText = sqlStr;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (!dr.HasRows) { fileName = "nothing"; }
                    while (dr.Read())
                    {
                        //Instanciation de l'adresse
                        Adresse a = DbContext.Adresses.Where(el => el.RefAdresse == (int)dr.GetSqlInt32(0)).FirstOrDefault();
                        Entite e = DbContext.Entites.Where(el => el.RefEntite == a.RefEntite).FirstOrDefault();
                        e.ExportSAGE = true;
                        sw.WriteLine("#CFOU");
                        sw.WriteLine(dr.GetSqlString(2).ToString());
                        sw.WriteLine("4011000000");
                        sw.WriteLine();
                        sw.WriteLine(a.Libelle?.Length > 35 ? Utils.Utils.convertirChaineSansAccent(a.Libelle.Substring(0, 35)) : Utils.Utils.convertirChaineSansAccent(a.Libelle));
                        sw.WriteLine(e.CodeEE);
                        sw.WriteLine();
                        sw.WriteLine(a.Adr1?.Length > 35 ? Utils.Utils.convertirChaineSansAccent(a.Adr1.Substring(0, 35)) : Utils.Utils.convertirChaineSansAccent(a.Adr1));
                        sw.WriteLine(a.Adr2?.Length > 35 ? Utils.Utils.convertirChaineSansAccent(a.Adr2.Substring(0, 35)) : Utils.Utils.convertirChaineSansAccent(a.Adr2));
                        sw.WriteLine(Utils.Utils.convertirChaineSansAccent(a.CodePostal));
                        sw.WriteLine(a.Ville?.Length > 35 ? Utils.Utils.convertirChaineSansAccent(a.Ville.Substring(0, 35)) : Utils.Utils.convertirChaineSansAccent(a.Ville));
                        for (int i = 0; i < 18; i++)
                        {
                            sw.WriteLine();
                        }
                        sw.WriteLine(e.AssujettiTVA == true ? 6 : 7);
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine(e.CodeTVA);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("1");
                        sw.WriteLine("2");
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("1");
                        sw.WriteLine();
                        sw.WriteLine(DateTime.Now.ToString("ddMMyy"));
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine("0");
                        sw.WriteLine("4011000000");
                        sw.WriteLine("#CRLT");
                        sw.WriteLine("5");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("0");
                        sw.WriteLine("1");
                        sw.WriteLine();
                    }
                    //Fin
                    sw.WriteLine("#FIN");
                    sw.Flush();
                    //Fermeture de la source de données
                    dr.Close();
                    //Mise à jour des codes comptables
                    cmd.CommandText = "update tblEntite set SAGECodeComptable='F' + isnull(tblEntite.RefExt, cast(tblEntite.RefEntite as nvarchar(50)))"
                        + " where RefEntiteType=3 and RefEntite in (select distinct RefEntite from tblContratOptimisationTransport) and SAGECodeComptable is null";
                    cmd.ExecuteNonQuery();
                }
                //Save entities
                DbContext.SaveChanges();
                //return file
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
        #endregion
    }
}
    
