/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création :24/09/2018
/// ----------------------------------------------------------------------------------------------------- 

using eValorplast.BLL;
using eVaSys.Data;
using GemBox.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using static eVaSys.Utils.Enumerations;

namespace eVaSys.Utils
{
    /// <summary>
    /// Utilities 
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private Utils()
        {
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Colors IN html #rrggbb format
        /// </summary>
        public static string[] hTMLColors = new string[] {
          "#4451cb",
          "#55b040",
          "#ffc82a",
          "#fe4201",
          "#ac00b5",
          "#05a5fb",
          "#ff9601",
          "#9e9e9e",
          "#009b8a",
          "#c71050",
          "#5a7d8d",
          "#68d34f",
          "#fff000",
          "#7e5444"
        };
        #region Database utilities
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Execute a raw SQL query and returns the number of affected rows
        /// </summary>
        public static int DbExecute(string sqlStr, SqlConnection sqlConn)
        {
            int i;
            using (sqlConn)
            {
                sqlConn.Open();
                SqlCommand cmd = new(); //commande Sql courante
                cmd.Connection = sqlConn;
                cmd.CommandText = sqlStr;
                i = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            return i;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the scalar value
        /// </summary>
        public static string DbScalar(string sqlScalar, DbConnection sqlConn)
        {
            string scalar;
            object objScalar;
            if (sqlConn.State == ConnectionState.Closed) { sqlConn.Open(); }
            SqlCommand cmd = new(); //commande Sql courante
            cmd.Connection = (SqlConnection)sqlConn;
            cmd.CommandText = sqlScalar;
            objScalar = cmd.ExecuteScalar();
            if (objScalar != null)
            {
                scalar = objScalar.ToString();
            }
            else
            {
                scalar = "";
            }
            cmd.Dispose();
            return scalar;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the RefUtilisateur corresponding to credentials
        /// </summary>
        public static int GetRefUtilisateurByCredentials(string login, string pwd, DbConnection sqlConn)
        {
            int scalar = 0;
            object objScalar;
            if (sqlConn.State == ConnectionState.Closed) { sqlConn.Open(); }
            SqlCommand cmd = new(); //commande Sql courante
            cmd.Connection = (SqlConnection)sqlConn;
            cmd.Parameters.Add("@login", SqlDbType.NVarChar, 50).Value = login;
            cmd.Parameters.Add("@pwd", SqlDbType.NVarChar, 50).Value = pwd;
            cmd.CommandText = "select dbo.GetUtilisateurByCredentials(@login, @pwd)";
            objScalar = cmd.ExecuteScalar();
            if (objScalar != DBNull.Value)
            {
                scalar = (int)objScalar;
            }
            cmd.Dispose();
            return scalar;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates a new record in a table and returns the newly created primary key value
        /// </summary>
        public static string DbId(string sqlInsert, SqlConnection sqlConn)
        {
            string scalar;
            using (sqlConn)
            {
                sqlConn.Open();
                SqlCommand cmd = new(); //commande Sql courante
                cmd.Connection = sqlConn;
                cmd.CommandText = sqlInsert + " SELECT SCOPE_IDENTITY()";
                scalar = cmd.ExecuteScalar().ToString();
                cmd.Dispose();
            }
            return scalar;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates and returns a DataReader
        /// </summary>
        public static SqlDataReader DbReader(string sqlSelect, SqlConnection sqlConn)
        {
            SqlDataReader rd;
            SqlCommand cmd = new(); //commande Sql courante
            cmd.Connection = sqlConn;
            cmd.CommandText = sqlSelect;
            rd = cmd.ExecuteReader();
            cmd.Dispose();
            return rd;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a new line to debug table
        /// </summary>
        public static void DebugPrint(string debug, string connString)
        {
            string sqlStr; //Chaine  SQL courante
            using (SqlConnection sqlConn = new(connString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new();
                sqlCmd.Connection = sqlConn;
                sqlStr = "insert into tbsDebug (text) values (@value)";
                sqlCmd.Parameters.Add("@value", SqlDbType.NVarChar).Value = debug;
                sqlCmd.CommandText = sqlStr;
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
            }
        }
        #endregion
        #region Data locks
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Try to lock a data
        /// </summary>
        public static Utilisateur LockData(string donnee, int? refUtilisateur, int? refDonnee, ApplicationDbContext dbContext)
        {
            Utilisateur curUser = null;
            if (refDonnee != null && refUtilisateur != null)
            {
                //check if data is locked
                curUser = IsLockedData(donnee, refDonnee, dbContext);
                if (curUser == null || curUser.RefUtilisateur == refUtilisateur)
                {
                    if (curUser == null || curUser.RefUtilisateur == refUtilisateur)
                    {
                        //Unlock data for update
                        UnlockData(donnee, refDonnee, refUtilisateur, dbContext);
                    }
                    //Lock data
                    Verrouillage ver = new();
                    dbContext.Verrouillages.Add(ver);
                    ver.RefUtilisateur = refUtilisateur;
                    ver.Donnee = donnee;
                    ver.RefDonnee = (int)refDonnee;
                    dbContext.SaveChanges();
                }
            }
            return curUser;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Check if a data is locked 
        /// Returns '' or Utilisateur name if locked
        /// </summary>
        public static Utilisateur IsLockedData(string donnee, int? refDonnee, ApplicationDbContext dbContext)
        {
            //Init
            Utilisateur curUser = null;
            if (refDonnee != null && !string.IsNullOrEmpty(donnee))
            {
                int delay = 60;
                //Remove old locks
                List<Verrouillage> vers = dbContext.Verrouillages.Where(e => e.RefDonnee == refDonnee && e.Donnee == donnee
                && EF.Functions.DateDiffMinute(e.DVerrouillage, DateTime.Now) >= (delay - 1)).ToList();
                if (vers.Count > 0)
                {
                    dbContext.Verrouillages.RemoveRange(vers);
                    dbContext.SaveChanges();
                }
                //Check if data is locked
                Verrouillage v = dbContext.Verrouillages.Where(e => e.RefDonnee == refDonnee && e.Donnee == donnee
                && EF.Functions.DateDiffMinute(e.DVerrouillage, DateTime.Now) < delay).FirstOrDefault();
                if (v != null) { curUser = v.Utilisateur; }
            }
            return curUser;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Unlock one data type for one user
        /// </summary>
        public static void UnlockDataType(string donnee, int? refUtilisateur, ApplicationDbContext dbContext)
        {
            if (!string.IsNullOrEmpty(donnee) && refUtilisateur != null)
            {
                //Remove user locks for same data type
                List<Verrouillage> vers = dbContext.Verrouillages.Where(e => e.RefUtilisateur == refUtilisateur && e.Donnee == donnee).ToList();
                dbContext.Verrouillages.RemoveRange(vers);
                dbContext.SaveChanges();
            }
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Unlock all data for one user
        /// </summary>
        public static void UnlockAllData(int? refUtilisateur, ApplicationDbContext dbContext)
        {
            if (refUtilisateur != null)
            {
                //Remove user locks for all data
                List<Verrouillage> vers = dbContext.Verrouillages.Where(e => e.RefUtilisateur == refUtilisateur).ToList();
                dbContext.Verrouillages.RemoveRange(vers);
                dbContext.SaveChanges();
            }
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Unlock one data for one user
        /// </summary>
        public static void UnlockData(string donnee, int? refDonnee, int? refUtilisateur, ApplicationDbContext dbContext)
        {
            if (refUtilisateur != null)
            {
                //Remove user locks for all data
                List<Verrouillage> vers = dbContext.Verrouillages.Where(e => e.RefUtilisateur == refUtilisateur && e.RefDonnee == refDonnee && e.Donnee == donnee).ToList();
                dbContext.Verrouillages.RemoveRange(vers);
                dbContext.SaveChanges();
            }
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Unlock all data
        /// </summary>
        public static void UnlockAll(ApplicationDbContext dbContext)
        {
            //Remove all user locks for all data
            List<Verrouillage> vers = dbContext.Verrouillages.ToList();
            dbContext.Verrouillages.RemoveRange(vers);
            dbContext.SaveChanges();
        }
        #endregion
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Add Actif fields for contact extraction if needed
        /// </summary>
        public static string AddActifFields(string fields, Context CurrentContext)
        {
            string s = "";
            if (fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RefEntite.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeCITEO.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSousContrat.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTypeLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCapacite.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteActionnaireProprietaire.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteExploitant.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteDimensionBalle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EquipementierLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.FournisseurTOLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntitePopulationContratN.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteVisibiliteAffretementCommun.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSurcoutCarburant.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeProcesss.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeStandards.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCmt.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeValorisation.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EcoOrganismeLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteRepartitionMensuelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteAssujettiTVA.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteIdNational.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteCodeTVA.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSAGECodeComptable.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteSAGECompteTiers.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGECategorieAchatLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEConditionLivraisonLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEPeriodiciteLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGEModeReglementLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SAGECategorieVenteLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteExportSAGE.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.MiseEnPlaceAutocontrole.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteReconductionIncitationQualite.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RepriseTypeLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.RepreneurLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratCollectiviteDebut.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratCollectiviteFin.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteContratCollectiviteActif.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeContratCollectivites.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratIncitationQualiteDebut.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContratIncitationQualiteFin.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.SousContratIncitationQualite.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeProduits.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeProduitInterdits.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCollectiviteActifs.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCollectiviteInactifs.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CentreDeTriListeCamionTypes.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TransporteurListeCamionTypes.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCentreDeTriActifs.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCentreDeTriInactifs.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeCentreDeTriActifInterdits.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteTransporteurActifInterdits.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeClientActifInterdits.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeIndustrielActifs.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeDocumentPublic.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteListeDocumentValorplast.ToString()].Name)
                )
            {
                s += ("," + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.EntiteActif.ToString()].Name);
            }
            if (fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseTypeLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseAdr1.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseAdr2.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseCodePostal.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseVille.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.PaysLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseHoraires.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseTel.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseEmail.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseSiteWeb.ToString()].Name)
                )
            {
                s += ("," + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.AdresseActif.ToString()].Name);
            }
            if (fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.TitreLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.CiviliteLibelle.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactPrenom.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactNom.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseCmt.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseListeFonctionServices.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseListeDocumentTypes.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseCmtServiceFonction.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseTel.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseTelMobile.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseEmail.ToString()].Name)
                || fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseListeProcesss.ToString()].Name)
                )
            {
                s += ("," + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ContactAdresseActif.ToString()].Name);
            }
            if (
                fields.Contains(CurrentContext.EnvDataColumns[Enumerations.DataColumnName.ListeDR.ToString()].Name)
                )
            {
                s += ("," + CurrentContext.EnvDataColumns[Enumerations.DataColumnName.DRActif.ToString()].Name);
            }
            //End
            return s;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Serialize a array of int into a sql filter string like this '[field] in (comma separated [values])'
        /// </summary>
        public static string CreateSQLFilter(string field, int[] values)
        {
            StringBuilder s = new();
            s.Append("1=1");
            if (values.Length > 0)
            {
                s.Append("in(");
                foreach (int value in values)
                {
                    s.Append(value + ",");
                }
                s.Remove(s.Length - 1, 1);
                s.Append(")");
            }
            return s.ToString();
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create and add parameters from a comma separated string
        /// </summary>
        public static string CreateSQLParametersFromString(string name, string filters, ref SqlCommand cmd, string envColumnDataType)
        {
            StringBuilder s = new();
            string[] crits = filters.Split(',');
            int i = 0;
            foreach (string cr in crits)
            {
                if (envColumnDataType == Enumerations.EnvDataColumnDataType.intNumber.ToString()) { cmd.Parameters.Add("@" + name + i, SqlDbType.Int).Value = cr; }
                else if (envColumnDataType == Enumerations.EnvDataColumnDataType.numeroCommande.ToString()) { cmd.Parameters.Add("@" + name + i, SqlDbType.Int).Value = cr; }
                else if (envColumnDataType == Enumerations.EnvDataColumnDataType.text.ToString()) { cmd.Parameters.Add("@" + name + i, SqlDbType.NVarChar).Value = cr.Trim(); }
                s.Append("@" + name + i + ",");
                i++;
            }
            s.Remove(s.Length - 1, 1);
            return s.ToString();
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Enrich the WHERE clause to comply to the searchStr, given the cols to search in
        /// </summary>
        public static string CreateSQLTextFilter(Context ctxt, SqlCommand cmd, string sqlStr, string searchStr, List<Enumerations.DataColumnName> colsLike, List<Enumerations.DataColumnName> colsLEquals = null)
        {
            //If a search string is provided, build the related SQL
            if (!string.IsNullOrEmpty(searchStr))
            {
                // Initializes a list of criteria
                List<string> criteria = new();
                // Add the "like" criteria
                if (colsLike != null)
                    criteria.AddRange(colsLike
                        .Select(x => ctxt.EnvDataColumns[x.ToString()])
                        .Select(x => (x.DataType == EnvDataColumnDataType.text.ToString() ?
                            x.FullField + " COLLATE Latin1_general_CI_AI like '%'+@filterText+'%' COLLATE Latin1_general_CI_AI"
                            : "CAST (" + x.FullField + " as nvarchar) " + " like '%'+REPLACE(@filterText,' ','')+'%'")));
                // Add the "Equals" criteria
                if (colsLEquals != null)
                    criteria.AddRange(colsLEquals
                        .Select(x => ctxt.EnvDataColumns[x.ToString()])
                        .Select(x => (x.DataType == EnvDataColumnDataType.text.ToString() ?
                            x.FullField + " COLLATE Latin1_general_CI_AI = @filterText COLLATE Latin1_general_CI_AI"
                            : "CAST (" + x.FullField + " as nvarchar) " + " = REPLACE(@filterText,' ','')")));
                if (criteria.Count > 0)
                {
                    // Build the resulting WHERE clause, combining the data with OR keyword
                    sqlStr += " AND ("
                        + String.Join(" OR ", criteria)
                        + ")";
                    // Add the associated query parameter if it does not exists
                    if (!cmd.Parameters.Contains("@filterText"))
                    {
                        cmd.Parameters.Add("@filterText", SqlDbType.NVarChar).Value = searchStr;
                    }
                }
            }
            return sqlStr;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Enrich the WHERE clause with a year filter
        /// </summary>
        public static string CreateSQLYearFilter(Context ctxt, SqlCommand cmd, string sqlStr, string filterYears, Enumerations.DataColumnName col)
        {
            if (!string.IsNullOrEmpty(filterYears))
            {

                string current = ctxt.EnvDataColumns[col.ToString()].DataType;
                if (current == EnvDataColumnDataType.date.ToString() || current == EnvDataColumnDataType.dateTime.ToString())
                    sqlStr += " and year(" + ctxt.EnvDataColumns[col.ToString()].FullField + ") in (";
                else
                    sqlStr += " and " + ctxt.EnvDataColumns[col.ToString()].FullField + " in (";
                sqlStr += CreateSQLParametersFromString("refAnnee", filterYears, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                sqlStr += ")";
            }
            return sqlStr;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Build a SQL string for reading required fields in the database
        /// Use only with a single source table
        /// </summary>
        public static string CreateSQLSelectColumns(Context ctxt, List<Enumerations.DataColumnName> cols, bool includeFromWhere = true)
        {
            // List of fields with aliases
            string sqlStr = "SELECT " + string.Join(",", cols.Select(x => ctxt.EnvDataColumns[x.ToString()]).Select(x => x.FullField + " as [" + x.Name + "]"));
            // Generate FROM & WHERE ?
            if (includeFromWhere)
            {
                // From clause with single table detection
                sqlStr += " FROM " + ctxt.EnvDataColumns[cols.First().ToString()].FullField.Split(".")[0];
                // WHERE clause, always true
                sqlStr += " WHERE 1=1";
            }
            return sqlStr;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Format and write data to a Excel file cell
        /// </summary>
        public static void WriteAndFormatCellData(DataRow dR, int i, int j, EnvDataColumn envDC, ref ExcelWorksheet ws, string menuName
            , CulturedRessources culturedRessources)
        {
            if (envDC != null
                && menuName != Enumerations.MenuName.LogistiqueMenuEtatReceptionEmballagePlastique.ToString()
                && menuName != Enumerations.MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur.ToString()
                && menuName != Enumerations.MenuName.LogistiqueMenuEtatDesFluxDev.ToString()
                && (menuName != Enumerations.StatType.ExtractionRSE.ToString())
                )
            {
                if (envDC.DataType == Enumerations.EnvDataColumnDataType.percent.ToString())
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    st.NumberFormat = NumberFormatBuilder.Percentage(2);
                    ws.Cells[i, j].Value = dR[j];
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.date.ToString())
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    st.NumberFormat = "dd/MM/yyyy";
                    ws.Cells[i, j].Value = dR[j];
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.dateTime.ToString())
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    st.NumberFormat = "dd/MM/yyyy HH:mm";
                    ws.Cells[i, j].Value = dR[j];
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.month.ToString())
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    st.NumberFormat = "MMMM";
                    ws.Cells[i, j].Value = dR[j];
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.monthYear.ToString())
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    st.NumberFormat = "MMMM yyyy";
                    ws.Cells[i, j].Value = dR[j];
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.monthNumber.ToString())
                {
                    string m = "";
                    int n = 0;
                    int.TryParse(dR[j].ToString(), out n);
                    if (n >= 1 && n <= 12)
                    {
                        m = culturedRessources.GetTextRessource(446 + n);
                    }
                    ws.Cells[i, j].Value = m;
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.intNumber.ToString())
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    decimal d = 0;
                    decimal.TryParse(dR[j].ToString(), out d);
                    if (((int)d) == d) { st.NumberFormat = NumberFormatBuilder.Number(0, 1, true); }
                    else { st.NumberFormat = NumberFormatBuilder.Number(2, 1, true); }
                    ws.Cells[i, j].Value = dR[j];
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.decimalNumber2.ToString())
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    st.NumberFormat = NumberFormatBuilder.Number(2, 1, true);
                    ws.Cells[i, j].Value = dR[j];
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.decimalNumber3.ToString())
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    st.NumberFormat = NumberFormatBuilder.Number(3, 1, true);
                    ws.Cells[i, j].Value = dR[j];
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.bit.ToString())
                {
                    if (dR[j] != DBNull.Value)
                    {
                        ws.Cells[i, j].Value = (Boolean)dR[j] ? culturedRessources.GetTextRessource(205) : culturedRessources.GetTextRessource(206);
                    }
                }
                else if (envDC.DataType == Enumerations.EnvDataColumnDataType.numeroCommande.ToString())
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    st.NumberFormat = NumberFormatBuilder.Text();
                    if (dR[j] != DBNull.Value)
                    {
                        ws.Cells[i, j].Value = Utils.FormatNumeroCommande(dR[j].ToString());
                    }
                }
                else
                {
                    ws.Cells[i, j].Value = dR[j];
                }
            }
            else
            {
                //Value
                ws.Cells[i, j].Value = dR[j];
                //Automatic style
                if ((menuName == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteProduit.ToString() && j >= 1)
                    || (menuName == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString() && j >= 2)
                    || (menuName == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString() && j >= 10)
                    || (menuName == Enumerations.StatType.ExtractionRSE.ToString() && j >= 4)
                    )
                {
                    CellStyle st = ws.Cells[i, j].Style;
                    st.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create multiple filter caption from a comma separated id strings
        /// </summary>
        public static string CreateFilterCaptionFromString(string filters, string entity, ApplicationDbContext dbContext, CultureInfo culture)
        {
            StringBuilder s = new();
            string[] crits = filters.Split(',');
            foreach (string cr in crits)
            {
                if (entity == "Month")
                {
                    try
                    {
                        var e = new Month(System.Convert.ToInt32(cr), null, culture);
                        s.Append(e.Name + ", ");
                    }
                    catch { }
                }
                else if (entity == "Quarter")
                {
                    try
                    {
                        var e = new Quarter(System.Convert.ToInt32(cr), 1, culture);
                        s.Append(e.Name + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.Entite.ToString())
                {
                    try
                    {
                        var e = dbContext.Entites.Find(Convert.ToInt32(cr));
                        if (!string.IsNullOrEmpty(e.CodeEE)) { s.Append(e.CodeEE + " - "); }
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.EcoOrganisme.ToString())
                {
                    try
                    {
                        var e = dbContext.EcoOrganismes.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.Adresse.ToString())
                {
                    try
                    {
                        var e = dbContext.Adresses.Find(Convert.ToInt32(cr));
                        s.Append(e.TexteAdresseList + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.ContactAdresse.ToString())
                {
                    try
                    {
                        var e = dbContext.ContactAdresses.Find(Convert.ToInt32(cr));
                        s.Append(e.Contact.Prenom + " " + e.Contact.Nom + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.Pays.ToString())
                {
                    try
                    {
                        var e = dbContext.Payss.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.Process.ToString())
                {
                    try
                    {
                        var e = dbContext.Processs.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.Produit.ToString())
                {
                    try
                    {
                        var e = dbContext.Produits.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == "ProduitNomCommun")
                {
                    try
                    {
                        var e = dbContext.Produits.Find(Convert.ToInt32(cr));
                        s.Append(e.NomCommun + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.ProduitGroupeReporting.ToString())
                {
                    try
                    {
                        var e = dbContext.ProduitGroupeReportings.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.Utilisateur.ToString())
                {
                    try
                    {
                        var e = dbContext.Utilisateurs.Find(Convert.ToInt32(cr));
                        s.Append(e.Nom + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.NonConformiteEtapeType.ToString())
                {
                    try
                    {
                        var e = dbContext.NonConformiteEtapeTypes.Find(Convert.ToInt32(cr));
                        e.currentCulture = culture;
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.NonConformiteNature.ToString())
                {
                    try
                    {
                        var e = dbContext.NonConformiteNatures.Find(Convert.ToInt32(cr));
                        e.currentCulture = culture;
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.CamionType.ToString())
                {
                    try
                    {
                        var e = dbContext.CamionTypes.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.ActionType.ToString())
                {
                    try
                    {
                        var e = dbContext.ActionTypes.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.EntiteType.ToString())
                {
                    try
                    {
                        var e = dbContext.EntiteTypes.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.Service.ToString())
                {
                    try
                    {
                        var e = dbContext.Services.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.Fonction.ToString())
                {
                    try
                    {
                        var e = dbContext.Fonctions.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.AdresseType.ToString())
                {
                    try
                    {
                        var e = dbContext.AdresseTypes.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else if (entity == Enumerations.ObjectName.ContactAdresseProcess.ToString())
                {
                    try
                    {
                        var e = dbContext.ContactAdresseProcesss.Find(Convert.ToInt32(cr));
                        s.Append(e.Libelle + ", ");
                    }
                    catch { }
                }
                else
                {
                    s.Append(cr + ", ");
                }
            }
            if (!string.IsNullOrEmpty(s.ToString()))
            {
                s.Remove(s.Length - 2, 2);
            }
            return s.ToString();
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Format NumeroCommande as 0000 00 0000 if parameter composed of 10 digits
        /// </summary>
        public static string FormatNumeroCommande(string n)
        {
            string r = n;
            if (!string.IsNullOrEmpty(n))
            {
                //Remove spaces
                n = n.Replace(" ", "");
                if (n.Length == 10 && int.TryParse(n, out _))
                {
                    r = n.Substring(0, 4) + " " + n.Substring(4, 2) + " " + n.Substring(6, 4);
                }
            }
            return r;
        }
        #region Divers
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fonction de conversion de chaîne accentué en chaîne sans accent
        /// </summary>
        /// <param name="chaine">La chaine à convertir</param>
        /// <returns>string</returns>
        public static string convertirChaineSansAccent(string chaine)
        {
            if (!string.IsNullOrEmpty(chaine))
            {
                // Déclaration de variables
                string accent = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
                string sansAccent = "AAAAAAaaaaaaOOOOOOooooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";
                // Conversion des chaines en tableaux de caractères
                char[] tableauSansAccent = sansAccent.ToCharArray();
                char[] tableauAccent = accent.ToCharArray();
                // Pour chaque accent
                for (int i = 0; i < accent.Length; i++)
                {
                    // Remplacement de l'accent par son équivalent sans accent dans la chaîne de caractères
                    chaine = chaine.Replace(tableauAccent[i].ToString(), tableauSansAccent[i].ToString());
                }
            }
            // Retour du résultat
            return chaine;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Converts empty or space string to null, remove spaces 
        /// </summary>
        /// <param name="s">String to convert</param>
        /// <returns>string</returns>
        public static string SetEmptyStringToNull(string s)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
            {
                s = null;
            }
            else
            {
                s = s.Trim();
            }
            // End
            return s;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Return the SearcResultType depanding on EntiteType
        /// </summary>
        /// <param name="refEntiteType">Entite type id</param>
        /// <returns>string</returns>
        public static string SearchEntiteType(int r)
        {
            string resultType = "";
            switch (r.ToString())
            {
                case "1":
                    resultType = Enumerations.SearchResultType.Collectivite.ToString();
                    break;
                case "2":
                    resultType = Enumerations.SearchResultType.Transporteur.ToString();
                    break;
                case "3":
                    resultType = Enumerations.SearchResultType.CDT.ToString();
                    break;
                case "4":
                    resultType = Enumerations.SearchResultType.Client.ToString();
                    break;
                default:
                    resultType = "unknown";
                    break;
            }
            // End
            return resultType;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Return the HTML code of a search result, bolding the search term
        /// </summary>
        /// <param name="s">Text to process</param>
        /// <param name="sc">Search criteria</param>
        /// <returns>string</returns>
        public static string FormatSearchResult(string s, string sc)
        {
            string formattedText = "";
            //Format 
            formattedText = HttpUtility.HtmlEncode(s);
            //formattedText = formattedText.Replace(HttpUtility.HtmlEncode(sc), "<strong>"+HttpUtility.HtmlEncode(sc)+"</strong>");
            formattedText = Regex.Replace(formattedText, HttpUtility.HtmlEncode(sc), "<strong>" + HttpUtility.HtmlEncode(sc) + "</strong>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            // End
            return formattedText;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Encode new lines in HTML
        /// </summary>
        /// <param name="s">Text to process</param>
        /// <returns>string</returns>
        public static string FormatHTMLNewLines(string s)
        {
            string formattedText = "";
            //Format new lines
            formattedText = s.Replace(Environment.NewLine, "<br/>");
            // End
            return formattedText;
        }
        #endregion
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Create a encrypted link to reset user password
        /// </summary>
        public static int GenerateLien(int refUtilisateur, int refUtilisateurCreation, string cultureName, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            int r = 0;
            var lien = new Lien() { RefUtilisateur = refUtilisateur, RefUtilisateurCreation = refUtilisateurCreation, Valeur = "NotSet" };
            dbContext.Liens.Add(lien);
            dbContext.SaveChanges();
            dbContext.Entry(lien).Reload();
            if (lien.RefLien > 0 && lien.RefUtilisateur != null)
            {
                //Create valeur
                lien.Valeur = CryptoUtils.EncryptAesHexa(lien.RefLien.ToString() + "-" + lien.RefUtilisateur.ToString() + "-" + cultureName, configuration["AppParameters:k"], configuration["AppParameters:v"]);
                //Update
                dbContext.SaveChanges();
                dbContext.Entry(lien).Reload();
                r = lien.RefLien;
            }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Get an app Parametre or default value
        /// </summary>
        public static Parametre GetParametre(int refParametre, ApplicationDbContext dbContext)
        {
            //Init parameters
            if (refParametre <= 0) { refParametre = 0; }
            //Try to get Parametre
            var p = dbContext.Parametres.Find(refParametre);
            //Set default if no parametre
            if (p == null)
            {
                p = new Parametre() { Libelle = "N/A", Serveur = true, ValeurNumerique = 0, ValeurTexte = "N/A", ValeurHTML = "N/A", ValeurBinaire = null };
                switch (refParametre)
                {
                    case 6: //Durée de validité des liens en heures
                        p.ValeurNumerique = 1;
                        break;
                    case 7: //E-mail suuport e-Valorplast
                        p.ValeurTexte = "systemesinformation@valorplast.com";
                        break;
                }
            }
            return p;
        }
    }
}

