/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création :08/01/2018
/// ----------------------------------------------------------------------------------------------------- 

using System.Data;
using System.Text;
using eVaSys.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using GemBox.Spreadsheet;
using eValorplast.BLL;
using GemBox.Spreadsheet.Charts;
using GemBox.Spreadsheet.Drawing;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace eVaSys.Utils
{
    /// <summary>
    /// Excel files management 
    /// </summary>
    public static class ExcelFileManagement
    {
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Export transports
        /// </summary>
        public static MemoryStream ExportTransport(SqlCommand cmd, Context currentContext, ApplicationDbContext dbContext, string filterDptDeparts, string filterDptArrivees, string filterVilleDeparts, string filterVilleArrivees, string filterPayss, string filterCamionTypes)
        {
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            MemoryStream mS = new();
            var eP = new GemBox.Spreadsheet.ExcelFile();
            //Init
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            // Add a new worksheet to the empty workbook
            var wS = eP.Worksheets.Add(currentContext.CulturedRessources.GetTextRessource(360));
            //Generate file
            int i = 0;
            int j = 0;

            //Styles
            //Titre
            CellStyle styleTitre = new(eP)
            {
                Font = new ExcelFont()
                {
                    Weight = ExcelFont.BoldWeight,
                    Size = 20 * 20
                }
            };

            //Critères
            CellStyle styleCritere = new(eP)
            {
                Font = new ExcelFont()
                {
                    //Weight = ExcelFont.BoldWeight,
                    Size = 14 * 20
                }
            };

            //Critères (titre)
            CellStyle styleCritereTitre = new(eP)
            {
                Font = new ExcelFont()
                {
                    Name = "Arial",
                    Size = 12 * 20
                },
                HorizontalAlignment = HorizontalAlignmentStyle.Right
            };

            //Critères (liste)
            CellStyle styleCritereListe = new(eP)
            {
                Font = new ExcelFont()
                {
                    Name = "Arial",
                    Size = 12 * 20
                },
            };
            CellStyle styleEnTeteColonne = new(eP);
            styleEnTeteColonne.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
            styleEnTeteColonne.Font.Weight = ExcelFont.BoldWeight;
            styleEnTeteColonne.Font.Name = "Arial";
            styleEnTeteColonne.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            styleEnTeteColonne.WrapText = true;
            CellStyle styleSousTotal = new(eP);
            styleSousTotal.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
            styleSousTotal.Font.Weight = ExcelFont.BoldWeight;
            styleSousTotal.Font.Name = "Arial";
            styleSousTotal.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            CellStyle styleEnTeteGroupe = new(eP)
            {
                Font = new ExcelFont()
                {
                    Name = "Arial",
                    Weight = ExcelFont.BoldWeight,
                    Size = 12 * 20
                },
                HorizontalAlignment = HorizontalAlignmentStyle.Right
            };

            //Load data
            //Initialisations
            DataSet dsStat = new();
            SqlDataAdapter adapterStat = new();
            adapterStat.SelectCommand = cmd;
            adapterStat.Fill(dsStat);
            DataTable dT = dsStat.Tables[0];
            //Rename columns
            foreach (DataColumn dC in dT.Columns)
            {
                if (currentContext.EnvDataColumns.ContainsKey(dC.ColumnName))
                {
                    if (currentContext.EnvDataColumns[dC.ColumnName].CulturedCaption != "")
                    {
                        dC.ColumnName = currentContext.EnvDataColumns[dC.ColumnName].CulturedCaption;
                    }
                }
            }
            j = 0;
            i = 7;
            //Inscription des en-têtes de colonne
            for (j = 0; j < dT.Columns.Count; j++)
            {
                wS.Cells[i, j].Value = (dT.Columns[j].Caption);
                wS.Cells[i, j].Style = styleEnTeteColonne;
            }
            i++;
            //Inscription des valeurs
            foreach (DataRow dR in dT.Rows)
            {
                for (j = 0; j < dT.Columns.Count; j++)
                {
                    //Formattage
                    if (dT.Columns[j].DataType == typeof(bool))
                    {
                        wS.Cells[i, j].Value = dR[j] == DBNull.Value ? null : ((bool)dR[j] ? currentContext.CulturedRessources.GetTextRessource(205) : currentContext.CulturedRessources.GetTextRessource(206));
                    }
                    else if (dT.Columns[j].DataType == typeof(DateTime))
                    {
                        wS.Cells[i, j].Value = dR[j] == DBNull.Value ? null : (DateTime?)dR[j];
                        wS.Cells[i, j].Style.NumberFormat = "dd/MM/yyyy";
                    }
                    else if (dT.Columns[j].DataType == typeof(int))
                    {
                        wS.Cells[i, j].Value = dR[j] == DBNull.Value ? null : (int?)dR[j];
                        wS.Cells[i, j].Style.NumberFormat = "#,##0";
                    }
                    else if (dT.Columns[j].DataType == typeof(decimal))
                    {
                        wS.Cells[i, j].Value = dR[j] == DBNull.Value ? null : (decimal?)dR[j];
                        wS.Cells[i, j].Style.NumberFormat = "#,##0.00";
                    }
                    else
                    {
                        wS.Cells[i, j].Value = dR[j] == DBNull.Value ? null : dR[j];
                    }
                    //Déverrouillage
                    if (j == 9)
                    {
                        wS.Cells[i, j].Style.Locked = false;
                    }
                }
                i++;
            }
            //Mise en forme finale
            int columnCount = wS.CalculateMaxUsedColumns();
            for (int c = 0; c < columnCount; c++)
                wS.Columns[c].AutoFit();
            //Titre du tableau
            wS.Cells[0, 1].Value = currentContext.CulturedRessources.GetTextRessource(361);
            wS.Cells[0, 1].Style = styleTitre;
            //Critères
            wS.Cells[1, 1].Style = styleCritere;
            if (filterDptDeparts != "")
            {
                wS.Cells[1, 1].Value = currentContext.CulturedRessources.GetTextRessource(309) + " - " + Dpt.TextFilter(filterDptDeparts, dbContext);
            }
            else
            {
                wS.Cells[1, 1].Value = currentContext.CulturedRessources.GetTextRessource(363);
            }
            wS.Cells[2, 1].Style = styleCritere;
            if (filterVilleDeparts != "")
            {
                wS.Cells[2, 1].Value = currentContext.CulturedRessources.GetTextRessource(310) + " - " + filterVilleDeparts.Replace(",", ", ");
            }
            else
            {
                wS.Cells[2, 1].Value = currentContext.CulturedRessources.GetTextRessource(364);
            }
            wS.Cells[3, 1].Style = styleCritere;
            if (filterVilleArrivees != "")
            {
                wS.Cells[3, 1].Value = currentContext.CulturedRessources.GetTextRessource(311) + " - " + filterVilleArrivees.Replace(",", ", ");
            }
            else
            {
                wS.Cells[3, 1].Value = currentContext.CulturedRessources.GetTextRessource(365);
            }
            wS.Cells[4, 1].Style = styleCritere;
            if (filterCamionTypes != "")
            {
                wS.Cells[4, 1].Value = currentContext.CulturedRessources.GetTextRessource(313) + " - " + CamionType.TextFilter(filterCamionTypes, dbContext);
            }
            else
            {
                wS.Cells[4, 1].Value = currentContext.CulturedRessources.GetTextRessource(366);
            }
            wS.Cells[5, 1].Style = styleCritere;
            if (filterPayss != "")
            {
                wS.Cells[5, 1].Value = currentContext.CulturedRessources.GetTextRessource(312) + " - " + Pays.TextFilter(filterPayss, dbContext);
            }
            else
            {
                wS.Cells[5, 1].Value = currentContext.CulturedRessources.GetTextRessource(367);
            }
            //Masquage des colonnes inutiles
            wS.Columns[0].Hidden = true;
            wS.Columns[10].Hidden = true;
            wS.Columns[11].Hidden = true;
            wS.Columns[12].Hidden = true;
            wS.Columns[13].Hidden = true;
            //Verrouillage
            wS.Protected = true;
            wS.ProtectionSettings.AllowDeletingRows = false;
            wS.ProtectionSettings.AllowDeletingRows = false;
            wS.ProtectionSettings.AllowEditingObjects = false;
            wS.ProtectionSettings.AllowEditingScenarios= false;
            wS.ProtectionSettings.AllowFormattingCells= false;
            wS.ProtectionSettings.AllowFormattingColumns= false;   
            wS.ProtectionSettings.AllowFormattingRows= false;
            wS.ProtectionSettings.AllowInsertingColumns= false;
            wS.ProtectionSettings.AllowInsertingHyperlinks= false;
            wS.ProtectionSettings.AllowSelectingUnlockedCells = true;
            wS.ProtectionSettings.SetPassword("eValorplastExcelLock");

            // set some document properties
            eP.DocumentProperties.BuiltIn[BuiltInDocumentProperties.Title] = currentContext.CulturedRessources.GetTextRessource(360);
            eP.DocumentProperties.BuiltIn[BuiltInDocumentProperties.Author] = "e-Valorplast";
            eP.DocumentProperties.BuiltIn[BuiltInDocumentProperties.Comments] = currentContext.CulturedRessources.GetTextRessource(362);
            // set some extended property values
            eP.DocumentProperties.BuiltIn[BuiltInDocumentProperties.Company] = "Valorplast";
            eP.DocumentProperties.BuiltIn[BuiltInDocumentProperties.DateContentCreated] = DateTime.Now.ToString("s") + "Z";
            eP.DocumentProperties.BuiltIn[BuiltInDocumentProperties.Application] = "Powered by Enviromatic : e-Valorplast";
            eP.Save(mS, SaveOptions.XlsxDefault);
            //End
            return mS;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Import new transport prices
        /// </summary>
        public static string ImportTransport(Stream fS, ref string error, Context currentContext, ApplicationDbContext dbContext)
        {
            int l = 0; //First line to process
            int i = 0; //Nb of updated prices
            StringBuilder s = new();
            try
            {
                //Open the workbook and worksheet
                SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
                var excelFile = ExcelFile.Load(fS);            //Création de la feuille
                var wS = excelFile.Worksheets[0];
                //Check global format
                if (wS.Cells[7, 0].Value != null && wS.Cells[7, 9].Value != null && wS.Cells[7, 10].Value != null && wS.Cells[7, 11].Value != null && wS.Cells[7, 12].Value != null && wS.Cells[7, 13].Value != null)
                {
                    if (wS.Cells[7, 0].Value.ToString() == currentContext.EnvDataColumns[Enumerations.DataColumnName.RefTransport.ToString()].Name
                        && wS.Cells[7, 10].Value.ToString() == currentContext.EnvDataColumns[Enumerations.DataColumnName.RefAdresseOrigine.ToString()].Name
                        && wS.Cells[7, 11].Value.ToString() == currentContext.EnvDataColumns[Enumerations.DataColumnName.RefAdresseDestination.ToString()].Name
                        && wS.Cells[7, 12].Value.ToString() == currentContext.EnvDataColumns[Enumerations.DataColumnName.RefTransporteur.ToString()].Name
                        && wS.Cells[7, 13].Value.ToString() == currentContext.EnvDataColumns[Enumerations.DataColumnName.RefCamionType.ToString()].Name)
                    {
                        //Process
                        l = 8;
                        while (wS.Cells[l, 1].Value != null && !string.IsNullOrWhiteSpace(wS.Cells[l, 1].Value.ToString()))
                        {
                            double? ancienPrix = null;
                            double? prix = null;
                            if (wS.Cells[l, 8].ValueType == CellValueType.Int) { ancienPrix = wS.Cells[l, 8].IntValue; }
                            if (wS.Cells[l, 8].ValueType == CellValueType.Double) { ancienPrix = wS.Cells[l, 8].DoubleValue; }
                            if (wS.Cells[l, 9].ValueType == CellValueType.Int) { prix = wS.Cells[l, 9].IntValue; }
                            if (wS.Cells[l, 9].ValueType == CellValueType.Double) { prix = wS.Cells[l, 9].DoubleValue; }
                            if (prix != null && ancienPrix != prix)
                            {
                                bool formatError = false;
                                int? refTransport = null;
                                int? refAdresseOrigine = null;
                                int? refAdresseDestination = null;
                                int? refCamionType = null;
                                int? refTransporteur = null;
                                try
                                {
                                    //Get transport data
                                    if (wS.Cells[l, 0].Value != null)
                                    {
                                        refTransport = wS.Cells[l, 0].IntValue;
                                    }
                                    else
                                    {
                                        refAdresseOrigine = wS.Cells[l, 10].IntValue;
                                        refAdresseDestination = wS.Cells[l, 11].IntValue;
                                        refTransporteur = wS.Cells[l, 12].IntValue;
                                        refCamionType = wS.Cells[l, 13].IntValue;
                                    }
                                }
                                catch
                                {
                                    formatError = true;
                                }
                                if (refTransport == null
                                    && (refTransporteur == null || refAdresseOrigine == null || refAdresseDestination == null || refCamionType == null)
                                    )
                                {
                                    formatError = true;
                                }
                                if (!formatError)
                                {
                                    Transport transport = null;
                                    if (refTransport != null)
                                    {
                                        // retrieve the existing transport to edit
                                        transport = dbContext.Transports.Where(q => q.RefTransport == refTransport).FirstOrDefault();
                                    }
                                    else
                                    {
                                        //Check if Transport exists
                                        transport = dbContext.Transports
                                            .Where(q => q.RefTransporteur == refTransporteur
                                            && q.RefCamionType == refCamionType
                                            && q.Parcours.RefAdresseOrigine == refAdresseOrigine
                                            && q.Parcours.RefAdresseDestination == refAdresseDestination)
                                            .FirstOrDefault();
                                        if (transport == null)
                                        {
                                            //Create new transport if needed
                                            transport = new Transport
                                            {
                                                RefUtilisateurCreation = currentContext.RefUtilisateur,
                                                DCreation = DateTime.Now,
                                                RefTransporteur = (int)refTransporteur,
                                                RefCamionType = (int)refCamionType
                                            };
                                            dbContext.Transports.Add(transport);

                                            //Looking for parcours
                                            Parcours parcours = dbContext.Parcourss.Where(q => q.RefAdresseOrigine == refAdresseOrigine && q.RefAdresseDestination == refAdresseDestination).FirstOrDefault();
                                            if (parcours == null)
                                            {
                                                parcours = new Parcours
                                                {
                                                    RefAdresseOrigine = (int)refAdresseOrigine,
                                                    RefAdresseDestination = (int)refAdresseDestination
                                                };
                                                dbContext.Parcourss.Add(parcours);
                                            }
                                            transport.Parcours = parcours;
                                        }
                                    }
                                    if (transport != null)
                                    {
                                        //Set values
                                        transport.PUHTDemande = (decimal?)prix;

                                        //Register session user
                                        transport.RefUtilisateurCourant = currentContext.RefUtilisateur;

                                        //Check validation
                                        string valid = transport.IsValid();
                                        //End
                                        if (valid == "")
                                        {
                                            // persist the changes into the Database.
                                            dbContext.SaveChanges();
                                            //One more trnapost processed
                                            i++;
                                        }
                                    }
                                }
                                else
                                {
                                    //Renvoi du message d'erreur
                                    error = currentContext.CulturedRessources.GetTextRessource(368);
                                    s.Append(currentContext.CulturedRessources.GetTextRessource(369) + " " + l.ToString() + ". " + currentContext.CulturedRessources.GetTextRessource(370));
                                    return s.ToString();
                                }
                            }
                            l++;
                        }
                        //Renvoi de l'information de réussite
                        s.Append(currentContext.CulturedRessources.GetTextRessource(371) + " " + i.ToString() + " " + currentContext.CulturedRessources.GetTextRessource(372));
                    }
                    else
                    {
                        //Renvoi du message d'erreur
                        error = currentContext.CulturedRessources.GetTextRessource(368);
                        s.Append(currentContext.CulturedRessources.GetTextRessource(373));
                    }
                }
                else
                {
                    //Renvoi du message d'erreur
                    error = currentContext.CulturedRessources.GetTextRessource(368);
                    s.Append(currentContext.CulturedRessources.GetTextRessource(373));
                }
            }
            catch
            {
                error = currentContext.CulturedRessources.GetTextRessource(374);
                s.Append(currentContext.CulturedRessources.GetTextRessource(375));
            }
            //End
            return s.ToString();
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create Fiche de répartition
        /// </summary>
        public static MemoryStream CreateFicheRepartition(int refEntite, string fileType, ApplicationDbContext dbContext, string rootPath, Context currentContext)
        {
            MemoryStream ms = new();
            int i = 0; //Compteur de lignes
            int l = 0;      //Compteur de lignes
            string sqlStr = "";
            //Validation
            Entite entite = dbContext.Entites.Where(e => e.RefEntite == refEntite).FirstOrDefault();
            if (entite == null)
            {
                throw new Exception(currentContext.CulturedRessources.GetTextRessource(356));
            }
            //Traitements
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            var excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\FicheRepartition.xlsx");            //Création de la feuille
            GemBox.Spreadsheet.ExcelWorksheet ws = excelFile.Worksheets[0];
            //Initialisations
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            var sqlConn = (SqlConnection)dbContext.Database.GetDbConnection();
            //Traitement de la feuille de répartition
            //Unitaire
            //Initialisations
            ws = excelFile.Worksheets[1];
            //Informations générales
            ws.Cells[8, 0].Value += entite.Libelle + (string.IsNullOrWhiteSpace(entite.CodeEE) ? "" : " (" + entite.CodeEE + ")");
            ws.Cells[17, 0].Value = Utils.GetParametre(3, dbContext).ValeurTexte
                + " - tél. : " + Utils.GetParametre(4, dbContext).ValeurTexte
                + " - e-mail : " + Utils.GetParametre(2, dbContext).ValeurTexte;
            //Gestion des produits
            //Création de la source de données
            sqlStr = "select tblProduit.NomCommun"
                + " from tbmEntiteProduit inner join tblProduit on tbmEntiteProduit.RefProduit=tblProduit.RefProduit"
                + " where RefEntite=" + entite.RefEntite
                + " order by tblProduit.NomCommun";
            //Chargement des données
            SqlCommand cmd = new SqlCommand(); //commande Sql courante
            cmd.CommandText = sqlStr;
            cmd.Connection = sqlConn;
            if (sqlConn.State == ConnectionState.Closed) { sqlConn.Open(); }
            SqlDataReader dr = cmd.ExecuteReader();
            l = 9;
            while (dr.Read())
            {
                if (l > 14)
                {
                    //ws.Rows[l].InsertCopy(1, ws.Rows[14]);
                    ws.Rows.InsertCopy(l, ws.Rows[14]);
                    ws.Cells[l, 3].Value = null;
                    ws.Cells[l, 4].Value = null;
                    //ws.Cells.GetSubrangeAbsolute(l, 3, l, 4).Merged = true;
                }
                ws.Cells[l, 3].Value = dr.GetValue(0);
                l++;
            }
            //Fermeture de la source de données
            dr.Close();
            //Gestion des collectivités
            //Création de la source de données
            sqlStr = "select distinct Libelle"
                + " from ("
                + "     select tblEntite.Libelle + case when tblEntite.CodeEE is null then '' else ' (' + tblEntite.CodeEE + ')' end as Libelle"
                + "     from tbmEntiteEntite inner join tblEntite on tbmEntiteEntite.RefEntiteRtt=tblEntite.RefEntite"
                + "         inner join tblContratCollectivite on tblEntite.RefEntite=tblContratCollectivite.RefEntite"
                + "     where tbmEntiteEntite.Actif=1 and tblEntite.RefEntiteType=1 and tbmEntiteEntite.RefEntite=" + entite.RefEntite
                + "         and getdate() between DDebut and DFin"
                + "     union all"
                + "     select tblEntite.Libelle + case when tblEntite.CodeEE is null then '' else ' (' + tblEntite.CodeEE + ')' end as Libelle"
                + "     from tbmEntiteEntite inner join tblEntite on tbmEntiteEntite.RefEntite=tblEntite.RefEntite"
                + "         inner join tblContratCollectivite on tblEntite.RefEntite=tblContratCollectivite.RefEntite"
                + "     where tbmEntiteEntite.Actif=1 and tblEntite.RefEntiteType=1 and tbmEntiteEntite.RefEntiteRtt=" + entite.RefEntite
                + "         and getdate() between DDebut and DFin"
                + " ) as u order by Libelle";
            //Chargement des données
            cmd = new SqlCommand(); //commande Sql courante
            cmd.CommandText = sqlStr;
            cmd.Connection = sqlConn;
            dr = cmd.ExecuteReader();
            if (l > 14) { l += 6; }
            else { l = 21; }
            while (dr.Read())
            {
                ws.Cells[l, 0].Value = dr.GetValue(0);
                l++;
            }
            //Fermeture de la source de données
            dr.Close();
            //Suppression des lignes inutiles
            for (i = 85; i >= l + 10; i--)
            {
                ws.Rows.Remove(i);
            }
            //Hauteur de ligne
            ws.Rows[15].AutoFit();
            ws.Rows[19].AutoFit();
            //Suppression des feuilles inutiles pour ne garder que la répartition unitaire
            excelFile.Worksheets.Remove(3);
            excelFile.Worksheets.Remove(2);
            excelFile.Worksheets.Remove(0);

            //Sauvegarde du fichier
            if (fileType == "xlsx")
            {
                excelFile.Save(ms, SaveOptions.XlsxDefault);
            }
            else
            {
                PdfConformanceLevel conformanceLevel = PdfConformanceLevel.PdfA2a;
                excelFile.Save(ms, new PdfSaveOptions()
                {
                    SelectionType = SelectionType.EntireFile,
                    ConformanceLevel = conformanceLevel
                });
            }
            return ms;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create CommandeAffretement
        /// </summary>
        public static MemoryStream CreateCommandeAffretement(int refCommandeFournisseur, bool courrier, int? bLUnique, string fileType, Context contexteCourant, ApplicationDbContext dbContext, string rootPath)
        {
            MemoryStream mSOut = new();
            //Instanciation de l'utilisateur connecté
            Utilisateur utilisateurConnecte = contexteCourant.ConnectedUtilisateur;
            int l = 0;      //Compteur de lignes
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            var excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\CommandeAffretement.xlsx");            //Création de la feuille
            //var excelFile = ExcelFile.Load(@"c:\tmp\CommandeAffretement.xlsx");            //Création de la feuille
            GemBox.Spreadsheet.ExcelWorksheet ws = excelFile.Worksheets[0];
            //Initialisations
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            CommandeFournisseur cf = dbContext.CommandeFournisseurs
                .Include(i=>i.CommandeFournisseurContrat)
                .Where(el => el.RefCommandeFournisseur == refCommandeFournisseur).FirstOrDefault();
            Entite client = dbContext.Entites.Where(el => el.RefEntite == cf.AdresseClient.RefEntite).FirstOrDefault();
            Adresse transporteurtAdresse = dbContext.Adresses.Where(el => el.RefAdresse == cf.TransporteurContactAdresse.RefAdresse).FirstOrDefault();
            ws.Cells[4, 3].SetValue(ws.Cells[4, 3].Value + cf.NumeroAffretement.ToString());
            ws.Cells[8, 3].SetValue(cf.Transporteur.Libelle);
            ws.Rows[8].AutoFit();
            ws.Cells[9, 3].SetValue(transporteurtAdresse.Adr1);
            ws.Rows[9].AutoFit();
            l = 10;
            if (!string.IsNullOrEmpty(transporteurtAdresse.Adr2))
            {
                ws.Cells[l, 3].SetValue(transporteurtAdresse.Adr2);
                ws.Rows[l].AutoFit();
                l++;
            }
            ws.Cells[l, 3].SetValue(transporteurtAdresse.CodePostal + " " + transporteurtAdresse.Ville);
            l += 3;
            ws.Cells[13, 3].SetValue(cf.TransporteurContactAdresse.Contact.Prenom + " " + cf.TransporteurContactAdresse.Contact.Nom);
            ws.Rows[13].AutoFit();
            ws.Cells[14, 3].SetValue(ws.Cells[14, 3].Value + cf.TransporteurContactAdresse.Fax);
            ws.Cells[17, 0].SetValue(ws.Cells[17, 0].Value + cf.CamionType.Libelle);
            ws.Cells[27, 1].SetValue(client.Libelle);
            ws.Rows[27].AutoFit();
            ws.Cells[28, 1].SetValue(cf.AdresseClient.Adr1);
            ws.Rows[28].AutoFit();
            l = 29;
            if (!string.IsNullOrEmpty(cf.AdresseClient.Adr2))
            {
                ws.Cells[l, 1].SetValue(cf.AdresseClient.Adr2);
                ws.Rows[l].AutoFit();
                l++;
            }
            ws.Cells[l, 1].SetValue(cf.AdresseClient.CodePostal + " " + cf.AdresseClient.Ville);
            l++;
            ws.Cells[l, 1].SetValue(cf.AdresseClient.Pays.Libelle);
            decimal prix = dbContext.CommandeFournisseurs.Where(el => el.NumeroAffretement == cf.NumeroAffretement).Sum(s => s.PrixTransportHT + s.PrixTransportSupplementHT);
            ws.Cells[33, 1].SetValue(prix + " €");
            //Signature
            ws.Cells[48, 1].SetValue(Utils.GetParametre(8, dbContext).ValeurTexte);
            var mS = new MemoryStream(Utils.GetParametre(10, dbContext).ValeurBinaire);
            ws.Pictures.Add(mS, ExcelPictureFormat.Jpeg, ws.Cells[45, 3].ToString(), 150, 150, LengthUnit.Pixel).Position.Mode = PositioningMode.FreeFloating;
            //Gestion des affrêtements
            //Création de la source de données
            List<CommandeFournisseur> cmdFs;
            if (bLUnique != null)
            {
                cmdFs = dbContext.CommandeFournisseurs.Where(el => el.RefCommandeFournisseur == bLUnique).ToList();
            }
            else
            {
                cmdFs = dbContext.CommandeFournisseurs.Where(el => el.NumeroAffretement == cf.NumeroAffretement).OrderBy(el => el.OrdreAffretement).ToList();
            }
            l = 21;
            int f = 1;
            foreach (CommandeFournisseur bl in cmdFs)
            {
                ws.Cells[l, 1].SetValue(bl.NumeroCommande);
                ws.Cells[l, 2].SetValue(bl.NbBalleChargement);
                ws.Cells[l, 3].SetValue(bl.Libelle);
                ws.Cells[l, 4].SetValue(bl.Ville);
                int? refContrat = bl.CommandeFournisseurContrat?.RefContrat;
                //Récupération de la commande client mensuelle
                CommandeClient cmdC = dbContext.CommandeClients
                    .Include(rel => rel.CommandeClientMensuelles)
                    .Where(el =>
                  el.RefProduit == bl.RefProduit && el.D.Year == ((DateTime)bl.DMoisDechargementPrevu).Year
                  && el.RefEntite == bl.AdresseClient.RefEntite && el.RefAdresse == bl.RefAdresseClient
                  && el.RefContrat==refContrat).FirstOrDefault();
                CommandeClientMensuelle ccm = cmdC.CommandeClientMensuelles.Where(el => el.D.Month == ((DateTime)bl.DMoisDechargementPrevu).Month).FirstOrDefault();
                //Instanciation du contactBL chez le client
                ContactAdresse cbl = dbContext.ContactAdresses
                    .Where(el => el.RefAdresse == bl.AdresseClient.RefAdresse && el.Actif == true
                        && el.ContactAdresseContactAdresseProcesss.Any(related => related.RefContactAdresseProcess == (int)Enumerations.ContactAdresseProcess.BonDeLivraison)).FirstOrDefault();
                Adresse blTransporteurAdresse = dbContext.Adresses.Where(el => el.RefAdresse == bl.TransporteurContactAdresse.RefAdresse).FirstOrDefault();
                //Instanciation du client
                client = dbContext.Entites.Include(rel => rel.Adresses)
                        .Where(el => el.RefEntite == bl.AdresseClient.RefEntite).FirstOrDefault();
                //-------------------------------------------------------------------------------------------------------
                //Choix du type de bordereau de livraison
                //-------------------------------------------------------------------------------------------------------
                //Bordereau de livraison logistique
                //-------------------------------------------------------------------------------------------------------
                if (bl.Pays.RefPays == 1 && bl.AdresseClient.Pays.RefPays == 1)
                {
                    //Copie de la feuille
                    excelFile.Worksheets.AddCopy("BL " + (f).ToString(), excelFile.Worksheets[1]);
                    //Ouverture du BL
                    GemBox.Spreadsheet.ExcelWorksheet sbl = excelFile.Worksheets[f + 2];
                    //Remplissage 
                    sbl.Cells[5, 1].SetValue(bl.Produit.NomCommercial);
                    sbl.Cells[6, 1].SetValue(sbl.Cells[6, 1].Value + ccm.IdExt);
                    sbl.Cells[7, 1].SetValue(sbl.Cells[7, 1].Value + bl.Produit.NumeroStatistique + " (" + bl.Produit.CodeListeVerte + ")");
                    sbl.Cells[8, 0].SetValue(sbl.Cells[8, 0].Value + bl.NumeroCommande.ToString());
                    sbl.Cells[9, 1].SetValue(sbl.Cells[9, 1].Value + bl.NumeroAffretement.ToString());
                    sbl.Cells[10, 0].SetValue(bl.Transporteur.Libelle);
                    sbl.Cells[11, 0].SetValue(bl.TransporteurContactAdresse.Contact.Prenom + " " + bl.TransporteurContactAdresse.Contact.Nom);
                    sbl.Cells[12, 0].SetValue(bl.TransporteurContactAdresse.GetTels);
                    sbl.Cells[10, 1].SetValue(sbl.Cells[10, 1].Value + bl.OrdreAffretement.ToString() + "/");
                    sbl.Cells[11, 1].SetValue(sbl.Cells[11, 1].Value + bl.CamionType.Libelle);
                    sbl.Cells[13, 1].SetValue(sbl.Cells[13, 1].Value + Utils.GetParametre(8, dbContext).ValeurTexte);
                    sbl.Cells[14, 1].SetValue(sbl.Cells[14, 1].Value + Utils.GetParametre(9, dbContext).ValeurTexte);
                    sbl.Cells[16, 0].SetValue(bl.Libelle);
                    sbl.Cells[17, 0].SetValue(sbl.Cells[17, 0].Value + bl.Entite.CodeEE);
                    sbl.Cells[18, 0].SetValue(bl.Adr1);
                    int a = 19;
                    if (!string.IsNullOrEmpty(bl.Adr2))
                    {
                        sbl.Cells[a, 0].SetValue(bl.Adr2);
                        a++;
                    }
                    sbl.Cells[a, 0].SetValue(bl.CodePostal + " " + bl.Ville);
                    a++;
                    sbl.Cells[a, 0].SetValue(bl.Pays?.Libelle);
                    sbl.Cells[22, 0].SetValue(sbl.Cells[22, 0].Value + bl.Prenom + " " + bl.Nom);
                    sbl.Cells[23, 0].SetValue(bl.GetTelsOrEmail);
                    sbl.Cells[24, 0].SetValue(sbl.Cells[24, 0].Value + bl.Horaires);
                    sbl.Cells[25, 0].SetValue(sbl.Cells[25, 0].Value + (!string.IsNullOrWhiteSpace(bl.RefExt) ? ("Ref. CITEO - " + bl.RefExt) : bl.CmtFournisseur));
                    sbl.Cells[26, 0].SetValue(sbl.Cells[26, 0].Value + ((DateTime)bl.D).ToString("dd/MM/yyyy"));
                    sbl.Cells[27, 0].SetValue(sbl.Cells[27, 0].Value + bl.NbBalleChargement.ToString());
                    sbl.Cells[28, 0].SetValue(sbl.Cells[28, 0].Value + (bl.DChargementPrevue == null ? "" : ((DateTime)bl.DChargementPrevue).ToString("dd/MM/yyyy") + " " + bl.HoraireChargementPrevu));
                    sbl.Cells[16, 1].SetValue(bl.AdresseClient.Libelle);
                    sbl.Cells[17, 1].SetValue(bl.AdresseClient.Adr1);
                    a = 18;
                    if (!string.IsNullOrEmpty(bl.AdresseClient.Adr2))
                    {
                        sbl.Cells[a, 1].SetValue(bl.AdresseClient.Adr2);
                        a++;
                    }
                    sbl.Cells[a, 1].SetValue(bl.AdresseClient.CodePostal + " " + bl.AdresseClient.Ville);
                    a++;
                    sbl.Cells[a, 1].SetValue(bl.AdresseClient.Pays.Libelle);
                    sbl.Cells[22, 1].SetValue(sbl.Cells[22, 1].Value + cbl.Contact.Prenom + " " + cbl.Contact.Nom);
                    sbl.Cells[23, 1].SetValue(cbl.GetTelsOrEmail);
                    sbl.Cells[24, 1].SetValue(sbl.Cells[24, 1].Value + bl.AdresseClient.Horaires);
                    sbl.Cells[25, 1].SetValue(sbl.Cells[25, 1].Value + bl.CmtClient);
                    sbl.Cells[26, 1].SetValue(sbl.Cells[26, 1].Value + (bl.DDechargementPrevue == null ? "" : ((DateTime)bl.DDechargementPrevue).ToString("dd/MM/yyyy") + " " + bl.HoraireDechargementPrevu));
                    sbl.Cells[56, 0].SetValue(sbl.Cells[56, 0].Value + client.CodeValorisation + " comme énumérée à l'annexe IIB de la directive 75/442 CEE.");
                    //Options d'impression
                    //sbl.NamedRanges.SetPrintArea(sbl.GetUsedCellRange(true));
                    sbl.PrintOptions.FitWorksheetHeightToPages = 1;
                    sbl.PrintOptions.FitWorksheetWidthToPages = 1;
                    sbl.PrintOptions.BottomMargin = 0.4;
                    sbl.PrintOptions.TopMargin = 0.4;
                    sbl.PrintOptions.LeftMargin = 0.4;
                    sbl.PrintOptions.RightMargin = 0.4;
                    sbl.PrintOptions.FitToPage = true;
                }
                //-------------------------------------------------------------------------------------------------------
                //Bordereau de livraison international
                //-------------------------------------------------------------------------------------------------------
                else
                {
                    //Copie de la feuille
                    excelFile.Worksheets.AddCopy("BL " + (f).ToString(), excelFile.Worksheets[2]);
                    //Ouverture du BL
                    GemBox.Spreadsheet.ExcelWorksheet sbl = excelFile.Worksheets[f + 2];
                    //Remplissage 
                    sbl.Cells[4, 3].SetValue(sbl.Cells[4, 3].Value + ccm.IdExt);
                    sbl.Cells[41, 3].SetValue(sbl.Cells[41, 3].Value + " " + bl.Produit.CodeListeVerte);
                    sbl.Cells[1, 3].SetValue(sbl.Cells[1, 3].Value + bl.NumeroCommande.ToString());
                    sbl.Cells[2, 3].SetValue(sbl.Cells[2, 3].Value + bl.OrdreAffretement.ToString() + "/");
                    sbl.Cells[3, 3].SetValue(sbl.Cells[3, 3].Value + bl.NumeroAffretement.ToString());
                    sbl.Cells[11, 0].SetValue(sbl.Cells[11, 0].Value + Utils.GetParametre(8, dbContext).ValeurTexte);
                    sbl.Cells[12, 0].SetValue(sbl.Cells[12, 0].Value + Utils.GetParametre(9, dbContext).ValeurTexte);
                    //Client administratif
                    Adresse ad = client.Adresses.Where(el => el.RefAdresseType == 1 && el.Actif).FirstOrDefault();
                    sbl.Cells[8, 3].SetValue(sbl.Cells[8, 3].Value + client.Libelle);
                    sbl.Cells[9, 3].SetValue(sbl.Cells[9, 3].Value + ad.Adr1);
                    if (!string.IsNullOrEmpty(ad.Adr2))
                    {
                        sbl.Cells[9, 3].SetValue(sbl.Cells[9, 3].Value + "/" + ad.Adr2);
                    }
                    sbl.Cells[10, 3].SetValue(sbl.Cells[10, 3].Value + ad.CodePostal + " " + ad.Ville + " / " + ad.Pays.Libelle);
                    //Contact à l'adresse administrative
                    ContactAdresse cad = dbContext.ContactAdresses.Where(el => el.RefAdresse == ad.RefAdresse && el.Actif==true).FirstOrDefault();
                    sbl.Cells[11, 3].SetValue(sbl.Cells[11, 3].Value + cad.Contact.Prenom + " " + cad.Contact.Nom);
                    sbl.Cells[12, 3].SetValue(cad.GetFirstTelOrEmail);
                    sbl.Cells[13, 3].SetValue(cad.GetSecondTelOrEmail);
                    //Transporteur
                    sbl.Cells[16, 0].SetValue(sbl.Cells[16, 0].Value + bl.Transporteur.Libelle);
                    sbl.Cells[17, 0].SetValue(sbl.Cells[17, 0].Value + blTransporteurAdresse.Adr1);
                    if (!string.IsNullOrEmpty(blTransporteurAdresse.Adr2))
                    {
                        sbl.Cells[17, 0].SetValue(sbl.Cells[17, 0].Value + "/" + blTransporteurAdresse.Adr2);
                    }
                    sbl.Cells[18, 0].SetValue(sbl.Cells[18, 0].Value + blTransporteurAdresse.CodePostal + " " + blTransporteurAdresse.Ville + " / " + blTransporteurAdresse.Pays.Libelle);
                    sbl.Cells[19, 0].SetValue(sbl.Cells[19, 0].Value + bl.TransporteurContactAdresse.Contact.Prenom + " " + bl.TransporteurContactAdresse.Contact.Nom);
                    sbl.Cells[20, 0].SetValue(bl.TransporteurContactAdresse.GetFirstTelOrEmail);
                    sbl.Cells[21, 0].SetValue(bl.TransporteurContactAdresse.GetSecondTelOrEmail);
                    sbl.Cells[22, 0].SetValue(sbl.Cells[22, 0].Value + bl.CamionType.Libelle);
                    sbl.Cells[23, 0].SetValue(sbl.Cells[23, 0].Value + (bl.DChargementPrevue == null ? "" : ((DateTime)bl.DChargementPrevue).ToString("dd/MM/yyyy") + " " + bl.HoraireChargementPrevu));
                    sbl.Cells[25, 1].SetValue(sbl.Cells[25, 1].Value + bl.Entite.CodeEE);
                    sbl.Cells[26, 0].SetValue(sbl.Cells[26, 0].Value + bl.Libelle);
                    sbl.Cells[27, 0].SetValue(sbl.Cells[27, 0].Value + bl.Adr1);
                    if (!string.IsNullOrEmpty(bl.Adr2))
                    {
                        sbl.Cells[27, 0].SetValue(sbl.Cells[27, 0].Value + "/" + bl.Adr2);
                    }
                    sbl.Cells[28, 0].SetValue(sbl.Cells[28, 0].Value + bl.CodePostal + " " + bl.Ville + " / " + (string.IsNullOrWhiteSpace(bl.Pays?.Libelle) ? "France" : bl.Pays?.Libelle));
                    sbl.Cells[29, 0].SetValue(sbl.Cells[29, 0].Value + bl.Prenom + " " + bl.Nom);
                    sbl.Cells[30, 0].SetValue(bl.GetFirstTelOrEmail);
                    sbl.Cells[31, 0].SetValue(bl.GetSecondTelOrEmail);
                    sbl.Cells[32, 0].SetValue(sbl.Cells[32, 0].Value + bl.Horaires);
                    sbl.Cells[33, 0].SetValue(sbl.Cells[33, 0].Value + (!string.IsNullOrWhiteSpace(bl.RefExt) ? ("Ref. CITEO - " + bl.RefExt) : bl.CmtFournisseur));
                    sbl.Cells[34, 0].SetValue(sbl.Cells[34, 0].Value + ((DateTime)bl.D).ToString("dd/MM/yyyy"));
                    sbl.Cells[34, 1].SetValue(sbl.Cells[34, 1].Value + bl.NbBalleChargement.ToString());
                    sbl.Cells[31, 3].SetValue(bl.Produit.NomCommercial);
                    sbl.Cells[26, 3].SetValue(sbl.Cells[26, 3].Value + client.CodeValorisation);
                    sbl.Cells[36, 0].SetValue(sbl.Cells[36, 0].Value + bl.AdresseClient.Libelle);
                    sbl.Cells[37, 0].SetValue(sbl.Cells[37, 0].Value + bl.AdresseClient.Adr1);
                    if (!string.IsNullOrEmpty(bl.AdresseClient.Adr2))
                    {
                        sbl.Cells[37, 0].SetValue(sbl.Cells[37, 0].Value + "/" + bl.AdresseClient.Adr2);
                    }
                    sbl.Cells[38, 0].SetValue(sbl.Cells[38, 0].Value + bl.AdresseClient.CodePostal + " " + bl.AdresseClient.Ville + " / " + bl.AdresseClient.Pays.Libelle);
                    sbl.Cells[39, 0].SetValue(sbl.Cells[39, 0].Value + cbl.Contact.Prenom + " " + cbl.Contact.Nom);
                    sbl.Cells[40, 0].SetValue(cbl.GetFirstTelOrEmail);
                    sbl.Cells[41, 0].SetValue(cbl.GetSecondTelOrEmail);
                    sbl.Cells[42, 1].SetValue(sbl.Cells[42, 0].Value + bl.AdresseClient.Horaires);
                    sbl.Cells[43, 1].SetValue(sbl.Cells[43, 1].Value + bl.CmtClient);
                    sbl.Cells[44, 0].SetValue(sbl.Cells[44, 0].Value + (bl.DDechargementPrevue == null ? "" : ((DateTime)bl.DDechargementPrevue).ToString("dd/MM/yyyy") + " " + bl.HoraireDechargementPrevu));
                    sbl.Cells[47, 0].SetValue(string.IsNullOrWhiteSpace(bl.Pays?.Libelle) ? "France" : bl.Pays?.Libelle);
                    sbl.Cells[47, 5].SetValue(bl.AdresseClient.Pays.Libelle);
                    sbl.Cells[52, 1].SetValue(sbl.Cells[52, 1].Value + DateTime.Now.ToString("dd/MM/yyyy"));
                    //Signature
                    sbl.Cells[52, 0].SetValue(sbl.Cells[52, 0].Value + Utils.GetParametre(8, dbContext).ValeurTexte);
                    mS = new MemoryStream(Utils.GetParametre(10, dbContext).ValeurBinaire);
                    sbl.Pictures.Add(mS, ExcelPictureFormat.Jpeg, ws.Cells[51, 5].ToString(), 65, 65, LengthUnit.Pixel).Position.Mode = PositioningMode.FreeFloating;
                    sbl.Pictures.Last().TransparentColor = SpreadsheetColor.FromArgb(255, 255, 255);
                    //Options d'impression
                    sbl.PrintOptions.FitWorksheetHeightToPages = 1;
                    sbl.PrintOptions.FitWorksheetWidthToPages = 1;
                    sbl.PrintOptions.BottomMargin = 0.4;
                    sbl.PrintOptions.TopMargin = 0.4;
                    sbl.PrintOptions.LeftMargin = 0.4;
                    sbl.PrintOptions.RightMargin = 0.4;
                    sbl.PrintOptions.FitToPage = true;
                }
                //Au suivant
                l++;
                f++;
            }
            //Suppression des BL modèle
            excelFile.Worksheets.Remove(2);
            excelFile.Worksheets.Remove(1);
            //Ajout aux nombres d'enlèvement
            for (int a = 1; a < f; a++)
            {
                if ((excelFile.Worksheets[a].Cells[0, 3].Value == null ? "" : excelFile.Worksheets[a].Cells[0, 3].Value.ToString()) == "BORDEREAU DE LIVRAISON")
                {
                    //BL international
                    excelFile.Worksheets[a].Cells[2, 3].SetValue(excelFile.Worksheets[a].Cells[2, 3].Value + (l - 21).ToString());
                }
                if ((excelFile.Worksheets[a].Cells[3, 1].Value == null ? "" : excelFile.Worksheets[a].Cells[3, 1].Value.ToString()) == "BORDEREAU DE LIVRAISON")
                {
                    //BL Logistique
                    excelFile.Worksheets[a].Cells[10, 1].SetValue(excelFile.Worksheets[a].Cells[10, 1].Value + (l - 21).ToString());
                }
            }
            ws.Cells[52, 0].SetValue(ws.Cells[52, 0].Value + (l - 21).ToString() + " bordereau(x) de livraison");
            //Suppression du courrier si impression faite par CDT ou 
            if (!courrier)
            {
                excelFile.Worksheets.Remove(0);
            }
            //Sauvegarde du fichier
            if (fileType == "xlsx")
            {
                excelFile.Save(mSOut, SaveOptions.XlsxDefault);
            }
            else
            {
                excelFile.Save(mSOut, new PdfSaveOptions()
                {
                    SelectionType = SelectionType.EntireFile
                });
            }
            return mSOut;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create Etat trimestriel collectivité CS
        /// </summary>
        public static MemoryStream CreateEtatTrimestrielCollectiviteCS(Quarter t, string filterCollectivites, string fileType, ApplicationDbContext dbContext, string rootPath)
        {
            MemoryStream ms = new();
            int i = 0; //Compteur de lignes
            int l = 0;      //Compteur de lignes
            int f = 0;  //compteur de feuilles
            string sqlStr = "";
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            var excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\EtatTrimestrielCollectiviteCS.xlsx");            //Création de la feuille
            GemBox.Spreadsheet.ExcelWorksheet ws = excelFile.Worksheets[0];
            //Initialisations
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            //Préparation au chargement des collectivités
            SqlCommand cmdCollectivite = new(); //commande Sql courante
            cmdCollectivite.Parameters.Add("@debut", SqlDbType.DateTime).Value = t.Begin;
            cmdCollectivite.Parameters.Add("@fin", SqlDbType.DateTime).Value = t.End;
            //Préparation à l'impression en masse
            //Création de la source de données pour les collectivités
            sqlStr = "select distinct tblRepartitionCollectivite.RefCollectivite, tblEntite.CodeEE, tblEntite.Libelle, c.Adr1, c.adr2"
                + "     , c.CodePostal, c.Ville"
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
                + "     left join(select tblAdresse.RefEntite, tblAdresse.Adr1, tblAdresse.adr2, tblAdresse.CodePostal, tblAdresse.Ville"
                + "         from tblAdresse "
                + "         where tblAdresse.RefAdresse in (select min(tblAdresse.RefAdresse) as RefAdresse from tblAdresse where Actif=1 and RefAdresseType=1 group by RefEntite)"
                + "         ) as c on tblEntite.RefEntite=c.RefEntite";
            if (!string.IsNullOrEmpty(filterCollectivites))
            {
                sqlStr += " where tblEntite.RefEntite in (";
                sqlStr += Utils.CreateSQLParametersFromString("refEntite", filterCollectivites, ref cmdCollectivite, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                sqlStr += ")"
                    + "     and rep.D between @debut and @fin"
                    + " order by tblEntite.CodeEE";
            }
            else
            {
                sqlStr += " where rep.D between @debut and @fin"
                    + " order by tblEntite.CodeEE";
            }
            //Chargement des données
            using (SqlConnection sqlConn = (SqlConnection)dbContext.Database.GetDbConnection())
            {
                sqlConn.Open();
                cmdCollectivite.Connection = sqlConn;
                cmdCollectivite.CommandText = sqlStr;
                DataSet ds = new();
                SqlDataAdapter adapter = new();
                adapter.SelectCommand = cmdCollectivite;
                adapter.Fill(ds);
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    //Initialisations
                    string refEntite = dRow[0].ToString();
                    string codeEE = dRow[1].ToString();
                    string libelle = dRow[2].ToString();
                    string adr1 = dRow[3].ToString();
                    string adr2 = dRow[4].ToString();
                    string codePostal = dRow[5].ToString();
                    string ville = dRow[6].ToString();
                    //Copie des feuilles
                    ws = excelFile.Worksheets[f];
                    ws.Name = ws.Name + refEntite;
                    excelFile.Worksheets.AddCopy("Etat", ws);
                    //Traitement de la feuille état
                    //Texte
                    ws = excelFile.Worksheets[f];
                    ws.Cells[5, 6].Value += t.Name;
                    ws.Cells[8, 6].Value = libelle;
                    ws.Cells[9, 6].Value = adr1;
                    l = 10;
                    if (adr2 != "")
                    {
                        ws.Cells[l, 6].Value = adr2;
                        l++;
                    }
                    ws.Cells[l, 6].Value = codePostal + " " + ville;
                    l += 3;
                    ws.Cells[14, 6].Value = "";
                    ws.Cells[15, 6].Value += DateTime.Now.ToString("dd/MM/yyyy");
                    ws.Cells[17, 0].Value += codeEE;
                    ws.Cells[18, 0].Value = "Contact : " + Utils.GetParametre(3, dbContext).ValeurTexte
                        + " - tél. : " + Utils.GetParametre(4, dbContext).ValeurTexte;
                    //Gestion des produits
                    //Création de la source de données
                    sqlStr = "select tblProduit.NomCommun, fournisseur.Libelle + case when fournisseur.CodeEE is not null then ' (' + fournisseur.CodeEE + ')' else '' end"
                        + "     , RefProcess, RefComposant, Poids, PUHT"
                        + " from"
                        + " 	( select VueRepartitionUnitaireDetail.RefProduit, tblCommandeFournisseur.RefEntite as RefFournisseur, null as RefProcess, null as RefComposant, sum(VueRepartitionUnitaireDetail.Poids) as Poids"
                        + "         , VueRepartitionUnitaireDetail.PUHT"
                        + "         from tblCommandeFournisseur"
                        + "             inner join VueRepartitionUnitaireDetail on tblCommandeFournisseur.RefCommandeFournisseur=VueRepartitionUnitaireDetail.RefCommandeFournisseur"
                        + "         	left join VueCommandeFournisseurContrat on VueRepartitionUnitaireDetail.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                        + "             where VueRepartitionUnitaireDetail.Collecte=1 and VueRepartitionUnitaireDetail.D between @debut and @fin"
                        + "                 and VueRepartitionUnitaireDetail.RefFournisseur=" + refEntite
                        + "         group by VueRepartitionUnitaireDetail.RefProduit, tblCommandeFournisseur.RefEntite"
                        + "             , VueRepartitionUnitaireDetail.PUHT"
                        + " 		 ) as univers"
                        + " 	inner join tblProduit on univers.RefProduit=tblProduit.refProduit"
                        + " 	inner join tblEntite as fournisseur on univers.RefFournisseur=fournisseur.RefEntite"
                        + " order by tblProduit.NomCommun, fournisseur.Libelle";
                    //Chargement des données
                    SqlCommand cmd = new(); //commande Sql courante
                    cmd.Connection = sqlConn;
                    cmd.Parameters.Add("@debut", SqlDbType.DateTime).Value = t.Begin;
                    cmd.Parameters.Add("@fin", SqlDbType.DateTime).Value = t.End;
                    cmd.CommandText = sqlStr;
                    {
                        string produit = "";
                        int poids = 0;
                        int total = 0;
                        SqlDataReader dr = cmd.ExecuteReader();
                        l = 0;
                        while (dr.Read())
                        {
                            if (l > 0)  //On ne traite pas le changement de produit pour la première ligne
                            {
                                if (produit != dr.GetValue(0).ToString())
                                {
                                    //Changement de produit
                                    ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                                    ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                                    ws.Cells[l + 21, 5].Style.Font.Weight = 700;
                                    ws.Cells[l + 21, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    ws.Cells[l + 21, 5].Style.Font.Size = 220;
                                    ws.Cells[l + 21, 5].Value = "Total " + produit;
                                    ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    ws.Cells[l + 21, 6].Style.Font.Weight = 700;
                                    ws.Cells[l + 21, 6].Style.Font.Size = 220;
                                    ws.Cells[l + 21, 6].Value = poids;
                                    ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                    produit = dr.GetValue(0).ToString();
                                    total += poids;
                                    poids = 0;
                                    l++;
                                }
                                ws.Cells[l + 21, 0].Value = dr.GetValue(0).ToString();
                                ws.Cells[l + 21, 3].Value = dr.GetValue(1).ToString();
                                ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                ws.Cells[l + 21, 6].Value = dr.GetValue(4);
                                ws.Cells[l + 21, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 7].Style.NumberFormat = NumberFormatBuilder.Number(2, useThousandsSeparator: true);
                                ws.Cells[l + 21, 7].Value = dr.GetValue(5);
                                poids += (int)dr.GetSqlInt32(4);
                                l++;
                            }
                            else
                            {
                                ws.Cells[l + 21, 0].Value = dr.GetValue(0).ToString();
                                ws.Cells[l + 21, 3].Value = dr.GetValue(1).ToString();
                                ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                ws.Cells[l + 21, 6].Value = dr.GetValue(4);
                                ws.Cells[l + 21, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 7].Style.NumberFormat = NumberFormatBuilder.Number(2, useThousandsSeparator: true);
                                ws.Cells[l + 21, 7].Value = dr.GetValue(5);
                                produit = dr.GetValue(0).ToString();
                                poids = (int)dr.GetSqlInt32(4);
                                l++;
                            }
                        }
                        //Dernier sou-total
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells[l + 21, 5].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 5].Style.Font.Size = 220;
                        ws.Cells[l + 21, 5].Value = "Total " + produit;
                        ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 6].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 6].Style.Font.Size = 220;
                        ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                        ws.Cells[l + 21, 6].Value = poids;
                        total += poids;
                        //Total général
                        l++;
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells[l + 21, 0].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        ws.Cells[l + 21, 0].Style.Font.Size = 220;
                        ws.Cells[l + 21, 0].Value = ">>>>>>> FIN DU DOCUMENT <<<<<<<";
                        ws.Cells[l + 21, 3].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 3].Style.Font.Size = 220;
                        ws.Cells[l + 21, 3].Value = "Total";
                        ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 6].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 6].Style.Font.Size = 220;
                        ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                        ws.Cells[l + 21, 6].Value = total;
                        //Fermeture de la source de données
                        dr.Close();
                        //Suppression des lignes en trop
                        for (i = 0; i < 100; i++)
                        {
                            ws.Rows.Remove(l + 22);
                        }
                        //Impression
                        ws.PrintOptions.FitWorksheetHeightToPages = 0;
                        ws.PrintOptions.FitWorksheetWidthToPages = 1;
                        ws.PrintOptions.FitToPage = true;
                    }

                    //Feuille suivante
                    f++;
                }
                //Suppression des dernières feuilles
                excelFile.Worksheets.Remove(f);
                //Sauvegarde du fichier
                if (fileType == "xlsx")
                {
                    excelFile.Save(ms, SaveOptions.XlsxDefault);
                }
                else
                {
                    PdfConformanceLevel conformanceLevel = PdfConformanceLevel.PdfA2a;
                    excelFile.Save(ms, new PdfSaveOptions()
                    {
                        SelectionType = SelectionType.EntireFile,
                        ConformanceLevel = conformanceLevel
                    });
                }
                return ms;
            }
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create Etat mensuel collectivité HCS
        /// </summary>
        public static MemoryStream CreateEtatMensuelCollectiviteHCS(Month m, string filterCollectivites, string fileType, ApplicationDbContext dbContext, string rootPath)
        {
            MemoryStream ms = new();
            int i = 0; //Compteur de lignes
            int l = 0;      //Compteur de lignes
            int f = 0;  //compteur de feuilles
            string sqlStr = "";
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            var excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\EtatMensuelCollectiviteHCS.xlsx");            //Création de la feuille
            GemBox.Spreadsheet.ExcelWorksheet ws = excelFile.Worksheets[0];
            //Initialisations
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            //Préparation au chargement des collectivités
            SqlCommand cmdCollectivite = new(); //commande Sql courante
            cmdCollectivite.Parameters.Add("@debut", SqlDbType.DateTime).Value = m.Begin;
            cmdCollectivite.Parameters.Add("@fin", SqlDbType.DateTime).Value = m.End;
            //Préparation à l'impression en masse
            //Création de la source de données pour les collectivités
            sqlStr = "select distinct tblRepartitionProduit.RefFournisseur, tblEntite.CodeEE, tblEntite.Libelle, c.Adr1, c.adr2"
                + "     , c.CodePostal, c.Ville"
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
                + "     inner join tblRepartitionProduit on tblRepartitionProduit.RefRepartition=rep.RefRepartition"
                + "     inner join tblEntite on tblRepartitionProduit.RefFournisseur=tblEntite.RefEntite"
                + "     left join(select tblAdresse.RefEntite, tblAdresse.Adr1, tblAdresse.adr2, tblAdresse.CodePostal, tblAdresse.Ville"
                + "         from tblAdresse "
                + "         where tblAdresse.RefAdresse in (select min(tblAdresse.RefAdresse) as RefAdresse from tblAdresse where Actif=1 and RefAdresseType=1 group by RefEntite)"
                + "         ) as c on tblEntite.RefEntite=c.RefEntite";
            if (!string.IsNullOrEmpty(filterCollectivites))
            {
                sqlStr += " where tblEntite.RefEntite in (";
                sqlStr += Utils.CreateSQLParametersFromString("refEntite", filterCollectivites, ref cmdCollectivite, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                sqlStr += ")"
                    + " order by tblEntite.CodeEE";
            }
            else
            {
                sqlStr += " where rep.D between @debut and @fin"
                    + " order by tblEntite.CodeEE";
            }
            //Chargement des données
            using (SqlConnection sqlConn = (SqlConnection)dbContext.Database.GetDbConnection())
            {
                sqlConn.Open();
                cmdCollectivite.Connection = sqlConn;
                cmdCollectivite.CommandText = sqlStr;
                DataSet ds = new();
                SqlDataAdapter adapter = new();
                adapter.SelectCommand = cmdCollectivite;
                adapter.Fill(ds);
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    //Initialisations
                    string refEntite = dRow[0].ToString();
                    string codeEE = dRow[1].ToString();
                    string libelle = dRow[2].ToString();
                    string adr1 = dRow[3].ToString();
                    string adr2 = dRow[4].ToString();
                    string codePostal = dRow[5].ToString();
                    string ville = dRow[6].ToString();
                    //Copie des feuilles
                    ws = excelFile.Worksheets[f];
                    ws.Name = ws.Name + refEntite;
                    excelFile.Worksheets.AddCopy("Certificat", ws);
                    //Traitement de la feuille état
                    //Texte
                    ws = excelFile.Worksheets[f];
                    ws.Cells[5, 6].Value += m.Name + " " + m.Year.ToString();
                    ws.Cells[8, 6].Value = libelle;
                    ws.Cells[9, 6].Value = adr1;
                    l = 10;
                    if (adr2 != "")
                    {
                        ws.Cells[l, 6].Value = adr2;
                        l++;
                    }
                    ws.Cells[l, 6].Value = codePostal + " " + ville;
                    l += 3;
                    ws.Cells[14, 6].Value = "";
                    ws.Cells[15, 6].Value += DateTime.Now.ToString("dd/MM/yyyy");
                    ws.Cells[17, 0].Value += codeEE;
                    ws.Cells[18, 0].Value = "Contact : " + Utils.GetParametre(3, dbContext).ValeurTexte
                        + " - tél. : " + Utils.GetParametre(4, dbContext).ValeurTexte;
                    //Gestion des produits
                    //Création de la source de données
                    sqlStr = "select tblProduit.NomCommun, fournisseur.Libelle + case when fournisseur.CodeEE is not null then ' (' + fournisseur.CodeEE + ')' else '' end"
                        + "     , RefProcess, RefComposant, Poids, PUHT"
                        + " from"
                        + " 	( select VueRepartitionUnitaireDetail.RefProduit, tblCommandeFournisseur.RefEntite as RefFournisseur, null as RefProcess, null as RefComposant, sum(VueRepartitionUnitaireDetail.Poids) as Poids"
                        + "         , VueRepartitionUnitaireDetail.PUHT"
                        + "         from tblCommandeFournisseur"
                        + "             inner join VueRepartitionUnitaireDetail on tblCommandeFournisseur.RefCommandeFournisseur=VueRepartitionUnitaireDetail.RefCommandeFournisseur"
                        + "         	left join VueCommandeFournisseurContrat on VueRepartitionUnitaireDetail.RefCommandeFournisseur=VueCommandeFournisseurContrat.RefCommandeFournisseur"
                        + "             where VueRepartitionUnitaireDetail.Collecte=0 and VueRepartitionUnitaireDetail.D between @debut and @fin"
                        + "                 and VueRepartitionUnitaireDetail.RefFournisseur=" + refEntite
                        + "         group by VueRepartitionUnitaireDetail.RefProduit, tblCommandeFournisseur.RefEntite"
                        + "             , VueRepartitionUnitaireDetail.PUHT"
                        + " 		 ) as univers"
                        + " 	inner join tblProduit on univers.RefProduit=tblProduit.refProduit"
                        + " 	inner join tblEntite as fournisseur on univers.RefFournisseur=fournisseur.RefEntite"
                        + " order by tblProduit.NomCommun, fournisseur.Libelle";
                    //Chargement des données
                    SqlCommand cmd = new(); //commande Sql courante
                    cmd.Connection = sqlConn;
                    cmd.Parameters.Add("@debut", SqlDbType.DateTime).Value = m.Begin;
                    cmd.Parameters.Add("@fin", SqlDbType.DateTime).Value = m.End;
                    cmd.CommandText = sqlStr;
                    {
                        string produit = "";
                        int poids = 0, poidsTotal = 0;
                        decimal prix = 0, prixTotal = 0;
                        SqlDataReader dr = cmd.ExecuteReader();
                        l = 0;
                        while (dr.Read())
                        {
                            if (l > 0)  //On ne traite pas le changement de produit pour la première ligne
                            {
                                if (produit != dr.GetValue(0).ToString())
                                {
                                    //Changement de produit
                                    ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                                    ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                                    ws.Cells[l + 21, 5].Style.Font.Weight = 700;
                                    ws.Cells[l + 21, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    ws.Cells[l + 21, 5].Style.Font.Size = 220;
                                    ws.Cells[l + 21, 5].Value = "Total " + produit;
                                    ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    ws.Cells[l + 21, 6].Style.Font.Weight = 700;
                                    ws.Cells[l + 21, 6].Style.Font.Size = 220;
                                    ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                    ws.Cells[l + 21, 6].Value = poids;
                                    produit = dr.GetValue(0).ToString();
                                    poidsTotal += poids;
                                    prixTotal += prix;
                                    poids = 0;
                                    prix = 0;
                                    l++;
                                }
                                ws.Cells[l + 21, 0].Value = dr.GetValue(0).ToString();
                                ws.Cells[l + 21, 3].Value = dr.GetValue(1).ToString();
                                ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                ws.Cells[l + 21, 6].Value = dr.GetValue(4);
                                ws.Cells[l + 21, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 7].Style.NumberFormat = NumberFormatBuilder.Number(2, useThousandsSeparator: true);
                                ws.Cells[l + 21, 7].Value = dr.GetValue(5);
                                poids += (int)dr.GetSqlInt32(4);
                                prix += Math.Round(Convert.ToDecimal((int)dr.GetValue(4)) / 1000 * (decimal)dr.GetValue(5), 2, MidpointRounding.AwayFromZero);
                                l++;
                            }
                            else
                            {
                                ws.Cells[l + 21, 0].Value = dr.GetValue(0).ToString();
                                ws.Cells[l + 21, 3].Value = dr.GetValue(1).ToString();
                                ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                ws.Cells[l + 21, 6].Value = dr.GetValue(4);
                                ws.Cells[l + 21, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 7].Style.NumberFormat = NumberFormatBuilder.Number(2, useThousandsSeparator: true);
                                ws.Cells[l + 21, 7].Value = dr.GetValue(5);
                                produit = dr.GetValue(0).ToString();
                                poids = (int)dr.GetSqlInt32(4);
                                prix = Math.Round(Convert.ToDecimal((int)dr.GetValue(4)) / 1000 * (decimal)dr.GetValue(5), 2, MidpointRounding.AwayFromZero);
                                l++;
                            }
                        }
                        //Dernier sou-total
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells[l + 21, 5].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 5].Style.Font.Size = 220;
                        ws.Cells[l + 21, 5].Value = "Total " + produit;
                        ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 6].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 6].Style.Font.Size = 220;
                        ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                        ws.Cells[l + 21, 6].Value = poids;
                        poidsTotal += poids;
                        prixTotal += prix;
                        //Total général
                        l++;
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 7).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells[l + 21, 3].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        ws.Cells[l + 21, 3].Style.Font.Size = 220;
                        ws.Cells[l + 21, 3].Value = ">>>>>>> FIN DU DOCUMENT <<<<<<<";
                        ws.Cells[l + 21, 5].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 5].Style.Font.Size = 220;
                        ws.Cells[l + 21, 5].Value = "Total";
                        ws.Cells[l + 21, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 6].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 6].Style.Font.Size = 220;
                        ws.Cells[l + 21, 6].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                        ws.Cells[l + 21, 6].Value = poidsTotal;
                        //Fermeture de la source de données
                        dr.Close();
                        //Inscription du bon texte facture
                        if (prixTotal < 0) { ws.Cells[15, 2].Value = "Cet état sert de base à Valorplast pour vous établir une facture."; }
                        //Suppression des lignes en trop
                        for (i = 0; i < 100; i++)
                        {
                            ws.Rows.Remove(l + 22);
                        }
                        //Impression
                        ws.PrintOptions.FitWorksheetHeightToPages = 0;
                        ws.PrintOptions.FitWorksheetWidthToPages = 1;
                        ws.PrintOptions.FitToPage = true;
                    }
                    //Feuille suivante
                    f++;
                }
                //Suppression des dernières feuilles
                excelFile.Worksheets.Remove(f);
                //Sauvegarde du fichier
                if (fileType == "xlsx")
                {
                    excelFile.Save(ms, SaveOptions.XlsxDefault);
                }
                else
                {
                    PdfConformanceLevel conformanceLevel = PdfConformanceLevel.PdfA2a;
                    excelFile.Save(ms, new PdfSaveOptions()
                    {
                        SelectionType = SelectionType.EntireFile,
                        ConformanceLevel = conformanceLevel
                    });
                }
                return ms;
            }
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create certificat recyclage CS
        /// </summary>
        public static MemoryStream CreateCertificatRecyclage(Quarter t, string filterCollectivites, bool cS, string fileType, ApplicationDbContext dbContext, string rootPath)
        {
            MemoryStream ms = new();
            int i = 0; //Compteur de lignes
            int l = 0;      //Compteur de lignes
            int f = 0;  //compteur de feuilles
            string sqlStr = "";
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            var excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\CertificatRecyclage.xlsx");            //Création de la feuille
            GemBox.Spreadsheet.ExcelWorksheet ws = excelFile.Worksheets[0];
            //Initialisations
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            //Préparation au chargement des collectivités
            SqlCommand cmdCollectivite = new(); //commande Sql courante
            cmdCollectivite.Parameters.Add("@begin", SqlDbType.DateTime).Value = t.Begin;
            cmdCollectivite.Parameters.Add("@end", SqlDbType.DateTime).Value = t.End;
            //Préparation à l'impression en masse
            //Création de la source de données pour les collectivités
            sqlStr = "select distinct tblRepartitionCollectivite.RefCollectivite, tblEntite.CodeEE, tblEntite.Libelle"
                + " from"
                + "     ("
                + "         select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D "
                + "         from tblRepartition "
                + " 	        inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                + "         union all"
                + "         select RefRepartition, RefFournisseur, RefProduit, D "
                + "         from tblRepartition"
                + "         where RefCommandeFournisseur is null"
                + "     ) as rep";
            //CS or HCS
            if (cS == true)
            {
                sqlStr += "     inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition";
            }
            else
            {
                sqlStr += "     inner join (select RefRepartition, RefFournisseur as RefCollectivite from tblRepartitionProduit) as tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition";
            }
            //end
            sqlStr += "     inner join tblEntite on tblRepartitionCollectivite.RefCollectivite=tblEntite.RefEntite";
            //Filters
            if (!string.IsNullOrEmpty(filterCollectivites))
            {
                sqlStr += " where tblEntite.RefEntite in (";
                sqlStr += Utils.CreateSQLParametersFromString("refEntite", filterCollectivites, ref cmdCollectivite, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                sqlStr += ")"
                    + "     and rep.D between @begin and @end"
                    + " order by tblEntite.CodeEE";
            }
            else
            {
                sqlStr += " where rep.D between @begin and @end"
                    + " order by tblEntite.CodeEE";
            }
            //Chargement des données
            using (SqlConnection sqlConn = (SqlConnection)dbContext.Database.GetDbConnection())
            {
                sqlConn.Open();
                cmdCollectivite.Connection = sqlConn;
                cmdCollectivite.CommandText = sqlStr;
                DataSet ds = new();
                SqlDataAdapter adapter = new();
                adapter.SelectCommand = cmdCollectivite;
                adapter.Fill(ds);
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    //Initialisations
                    string refEntite = dRow[0].ToString();
                    string codeEE = dRow[1].ToString();
                    string libelle = dRow[2].ToString();
                    //Copie des feuilles
                    ws = excelFile.Worksheets[f];
                    ws.Name = ws.Name + refEntite;
                    excelFile.Worksheets.AddCopy("Certificat", ws);
                    //Traitement de la feuille état
                    //Texte
                    ws = excelFile.Worksheets[f];
                    ws.Cells[8, 4].Value += t.Name;
                    ws.Cells[9, 4].Value = libelle;
                    ws.Cells[10, 4].Value = codeEE;
                    ws.Cells[15, 4].Value = "Paris, le " + DateTime.Now.ToString("dd/MM/yyyy");
                    //Gestion des produits
                    //Création de la source de données
                    sqlStr = "select y, m, tblProduit.NomCommun, fournisseur.Libelle + case when fournisseur.CodeEE is not null then ' (' + fournisseur.CodeEE + ')' else '' end"
                        + " , Poids, tblAdresse.Ville, tbrPays.Libelle"
                        + " from"
                        + "     ( select year(D) as y, month(D) as m, rep.RefProduit, rep.RefFournisseur, rep.RefAdresseClient, sum(Poids) as Poids"
                        + " 		from"
                        + "             ("
                        + " 				select RefRepartition, tblCommandeFournisseur.RefAdresseClient, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                        + " 				from tblRepartition"
                        + " 					inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        + " 				where tblCommandeFournisseur.DDechargement between @begin and @end"
                        + "                 union all"
                        + " 				select RefRepartition, null as RefAdresseClient, RefFournisseur, RefProduit, D"
                        + "                 from tblRepartition"
                        + "                 where RefCommandeFournisseur is null and D between @begin and @end) as rep";
                    //CS or HCS
                    if (cS == true)
                    {
                        sqlStr += " 			inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition";
                    }
                    else
                    {
                        sqlStr += "             inner join (select RefRepartition, RefFournisseur as RefCollectivite, Poids from tblRepartitionProduit) as tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition";
                    }
                    sqlStr += "         where tblRepartitionCollectivite.RefCollectivite=" + refEntite.ToString()
                        + " 		group by year(D), month(D), rep.RefProduit, rep.RefFournisseur, rep.RefAdresseClient"
                        + " 		 ) as univers"
                        + " 	inner join tblProduit on univers.RefProduit=tblProduit.refProduit"
                        + " 	inner join tblEntite as fournisseur on univers.RefFournisseur=fournisseur.RefEntite"
                        + " 	left join tblAdresse on univers.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     left join tbrPays on tbrPays.RefPays=tblAdresse.RefPays"
                        + " order by y, m, tblProduit.NomCommun, fournisseur.Libelle";
                    //Chargement des données
                    SqlCommand cmd = new(); //commande Sql courante
                    cmd.Connection = sqlConn;
                    cmd.Parameters.Add("@begin", SqlDbType.DateTime).Value = t.Begin;
                    cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = t.End;
                    cmd.CommandText = sqlStr;
                    {
                        string mois = "", annee = "";
                        int poids = 0;
                        int total = 0;
                        SqlDataReader dr = cmd.ExecuteReader();
                        l = 0;
                        var m = new Month(DateTime.Now, new System.Globalization.CultureInfo("fr-FR"));
                        while (dr.Read())
                        {
                            //Process
                            if (l > 0)  //On ne traite pas le changement de mois pour la première ligne
                            {
                                if (annee != dr.GetValue(0).ToString() || mois != dr.GetValue(1).ToString())
                                {
                                    //Changement de période
                                    ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 6).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                                    ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 6).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                                    ws.Cells[l + 21, 2].Style.Font.Weight = 700;
                                    ws.Cells[l + 21, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    ws.Cells[l + 21, 2].Style.Font.Size = 220;
                                    ws.Cells[l + 21, 2].Value = "Total " + m.Name + " " + m.Year.ToString();
                                    ws.Cells[l + 21, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    ws.Cells[l + 21, 3].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                    ws.Cells[l + 21, 3].Style.Font.Weight = 700;
                                    ws.Cells[l + 21, 3].Style.Font.Size = 220;
                                    ws.Cells[l + 21, 3].Value = poids;
                                    annee = dr.GetValue(0).ToString();
                                    mois = dr.GetValue(1).ToString();
                                    total += poids;
                                    poids = 0;
                                    l++;
                                }
                                //Month
                                m = new Month((int)dr.GetValue(1), (int)dr.GetValue(0), new System.Globalization.CultureInfo("fr-FR"));
                                ws.Cells[l + 21, 0].Value = m.Name + " " + m.Year.ToString();
                                ws.Cells[l + 21, 1].Value = dr.GetValue(2).ToString();
                                ws.Cells[l + 21, 2].Value = dr.GetValue(3).ToString();
                                ws.Cells[l + 21, 3].Value = (int)dr.GetValue(4);
                                ws.Cells[l + 21, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 3].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                ws.Cells[l + 21, 4].Value = dr.GetValue(5);
                                ws.Cells[l + 21, 5].Value = dr.GetValue(6);
                                ws.Cells[l + 21, 6].Value = "Recyclage matière";
                                poids += (int)dr.GetValue(4);
                                l++;
                            }
                            else
                            {
                                //Month
                                m = new Month((int)dr.GetValue(1), (int)dr.GetValue(0), new System.Globalization.CultureInfo("fr-FR"));
                                ws.Cells[l + 21, 0].Value = m.Name + " " + m.Year.ToString();
                                ws.Cells[l + 21, 1].Value = dr.GetValue(2).ToString();
                                ws.Cells[l + 21, 2].Value = dr.GetValue(3).ToString();
                                ws.Cells[l + 21, 3].Value = (int)dr.GetValue(4); ;
                                ws.Cells[l + 21, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 21, 3].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                ws.Cells[l + 21, 4].Value = dr.GetValue(5);
                                ws.Cells[l + 21, 5].Value = dr.GetValue(6);
                                ws.Cells[l + 21, 6].Value = "Recyclage matière";
                                annee = dr.GetValue(0).ToString();
                                mois = dr.GetValue(1).ToString();
                                poids = (int)dr.GetValue(4);
                                l++;
                            }
                        }
                        //Dernier sou-total
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 6).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells[l + 21, 2].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 2].Style.Font.Size = 220;
                        ws.Cells[l + 21, 2].Value = "Total " + m.Name + " " + m.Year.ToString();
                        ws.Cells[l + 21, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 3].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                        ws.Cells[l + 21, 3].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 3].Style.Font.Size = 220;
                        ws.Cells[l + 21, 3].Value = poids;
                        total += poids;
                        //Total général
                        l++;
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 6).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 6).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells[l + 21, 1].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        ws.Cells[l + 21, 1].Style.Font.Size = 220;
                        ws.Cells[l + 21, 1].Value = ">>> FIN DU DOCUMENT <<<";
                        ws.Cells[l + 21, 2].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 2].Style.Font.Size = 220;
                        ws.Cells[l + 21, 2].Value = "Total";
                        ws.Cells[l + 21, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 3].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                        ws.Cells[l + 21, 3].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 3].Style.Font.Size = 220;
                        ws.Cells[l + 21, 3].Value = total;
                        //Fermeture de la source de données
                        dr.Close();
                        //Suppression des lignes en trop
                        for (i = 0; i < 100; i++)
                        {
                            ws.Rows.Remove(l + 22);
                        }
                        //Impression
                        ws.PrintOptions.FitWorksheetHeightToPages = 0;
                        ws.PrintOptions.FitWorksheetWidthToPages = 1;
                        ws.PrintOptions.FitToPage = true;
                    }

                    //Feuille suivante
                    f++;
                }
                //Suppression des dernières feuilles
                excelFile.Worksheets.Remove(f);
                //Sauvegarde du fichier
                if (fileType == "xlsx")
                {
                    excelFile.Save(ms, SaveOptions.XlsxDefault);
                }
                else
                {
                    PdfConformanceLevel conformanceLevel = PdfConformanceLevel.PdfA2a;
                    excelFile.Save(ms, new PdfSaveOptions()
                    {
                        SelectionType = SelectionType.EntireFile,
                        ConformanceLevel = conformanceLevel
                    });
                }
                return ms;
            }
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create Etat mensuel réception
        /// </summary>
        public static MemoryStream CreateEtatMensuelReception(int m, int y, string filterCentreDeTris, string fileType, ApplicationDbContext dbContext, string rootPath)
        {
            MemoryStream ms = new();
            int i = 0; //Compteur de lignes
            int l = 0;      //Compteur de lignes
            int f = 0;  //compteur de feuilles
            string sqlStr = "";
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            var excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\EtatMensuelReception.xlsx");            //Création de la feuille
            GemBox.Spreadsheet.ExcelWorksheet ws = excelFile.Worksheets[0];
            //Initialisations
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            DateTime dDebut = new(y, m, 1);
            DateTime dFin = new DateTime(y, m, 1).AddMonths(1).AddSeconds(-1);
            //Préparation au chargement des collectivités
            SqlCommand cmd = new(); //commande Sql courante
            cmd.Parameters.Add("@debut", SqlDbType.DateTime).Value = dDebut;
            cmd.Parameters.Add("@fin", SqlDbType.DateTime).Value = dFin;
            //Préparation à l'impression en masse
            //Création de la source de données pour les fournisseur totalement répartis
            if (!string.IsNullOrEmpty(filterCentreDeTris))
            {
                sqlStr = "Select RefEntite from tblEntite where RefEntite in(";
                sqlStr += Utils.CreateSQLParametersFromString("refEntite", filterCentreDeTris, ref cmd, Enumerations.EnvDataColumnDataType.intNumber.ToString());
                sqlStr += ")";

            }
            else
            {
                sqlStr = "select distinct RefFournisseur, Libelle"
                    + " from ("
                    + " 	select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D "
                    + " 	from tblRepartition "
                    + " 		inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                    + " 	union all"
                    + " 	select RefRepartition, RefFournisseur, RefProduit, D "
                    + " 	from tblRepartition"
                    + " 	where RefCommandeFournisseur is null"
                    + " 	) as rep "
                    + " inner join tblEntite on RefFournisseur=RefEntite"
                    + " where D between  @debut and @fin"
                    + " order By CodeEE";
            }
            //Chargement des données
            using (SqlConnection sqlConn = (SqlConnection)dbContext.Database.GetDbConnection())
            {
                sqlConn.Open();
                cmd.Connection = sqlConn;
                cmd.CommandText = sqlStr;
                DataSet dsFournisseur = new();
                SqlDataAdapter adapter = new();
                adapter.SelectCommand = cmd;
                adapter.Fill(dsFournisseur);
                foreach (DataRow dRow in dsFournisseur.Tables[0].Rows)
                {

                    //Initialisations
                    Entite cDT = dbContext.Entites.Where(el => el.RefEntite == (int)dRow[0]).FirstOrDefault();
                    Adresse adr = dbContext.Adresses.Where(el => el.RefEntite == cDT.RefEntite && el.Actif 
                    && (el.RefAdresseType == 1 || el.RefAdresseType == 4)).OrderBy(o=>o.RefAdresseType).FirstOrDefault();
                    //Copie des feuilles
                    ws = excelFile.Worksheets[f];
                    ws.Name = ws.Name + cDT.RefEntite;
                    excelFile.Worksheets.AddCopy("Etat", ws);
                    ws = excelFile.Worksheets[f + 1];
                    ws.Name = ws.Name + cDT.RefEntite;
                    excelFile.Worksheets.AddCopy("Repartition", ws);
                    ws = excelFile.Worksheets[f + 2];
                    ws.Name = ws.Name + cDT.RefEntite;
                    excelFile.Worksheets.AddCopy("Produits", ws);
                    //------------------------------------------------------------------------------
                    //Traitement de la feuille état
                    //Initialisations
                    ws = excelFile.Worksheets[f];
                    ws.Cells[6, 3].Value += dDebut.ToString("MMMM yyyy");
                    ws.Cells[8, 3].Value = cDT.Libelle;
                    ws.Rows[8].AutoFit();
                    ws.Cells[9, 3].Value = adr.Adr1;
                    l = 10;
                    if (adr.Adr2 != "")
                    {
                        ws.Cells[l, 3].Value = adr.Adr2;
                        l++;
                    }
                    ws.Cells[l, 3].Value = adr.CodePostal + " " + adr.Ville;
                    l += 3;
                    ws.Cells[14, 3].Value = "";
                    ws.Rows[14].AutoFit();
                    ws.Cells[15, 3].Value += DateTime.Now.ToString("dd/MM/yyyy");
                    ws.Cells[17, 0].Value += cDT.CodeEE;
                    ws.Cells[18, 0].Value = "Contact : " + Utils.GetParametre(3, dbContext).ValeurTexte
                        + " - tél. : " + Utils.GetParametre(4, dbContext).ValeurTexte;
                    //Gestion des produits
                    //Création de la source de données
                    sqlStr = "select tblProduit.NomCommun, NumeroCommande, DDechargement, client.Libelle, PoidsChargement"
                        + " from tblCommandeFournisseur"
                        + "     inner join tblAdresse on tblCommandeFournisseur.RefAdresseClient=tblAdresse.RefAdresse"
                        + "     inner join tblEntite as client on client.RefEntite=tblAdresse.RefEntite"
                        + "     inner join tblProduit on tblCommandeFournisseur.RefProduit=tblProduit.refProduit"
                        + " where DDechargement between @debut and @fin and tblCommandeFournisseur.RefEntite=" + cDT.RefEntite;
                    sqlStr += " order by tblProduit.NomCommun, DDechargement";
                    //Chargement des données
                    cmd.CommandText = sqlStr;
                    string produit = "";
                    int poids = 0;
                    int total = 0;
                    cmd.Connection = sqlConn;
                    SqlDataReader dr = cmd.ExecuteReader();
                    l = 0;
                    while (dr.Read())
                    {
                        if (l > 0)  //On ne traite pas le changement de produit pour la première ligne
                        {
                            if (produit != dr.GetValue(0).ToString())
                            {
                                //Changement de produit
                                ws.Cells.GetSubrangeAbsolute(l + 23, 0, l + 23, 4).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                                ws.Cells.GetSubrangeAbsolute(l + 23, 0, l + 23, 4).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                                ws.Cells[l + 23, 3].Style.Font.Weight = 700;
                                ws.Cells[l + 23, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                ws.Cells[l + 23, 3].Value = "Total " + produit;
                                ws.Cells[l + 23, 4].Style.Font.Weight = 700;
                                ws.Cells[l + 23, 4].Style.Font.Size = 220;
                                ws.Cells[l + 23, 4].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                ws.Cells[l + 23, 4].Value = poids;
                                produit = dr.GetValue(0).ToString();
                                total += poids;
                                poids = 0;
                                l++;
                            }
                            ws.Cells[l + 23, 0].Value = dr.GetValue(0).ToString();
                            ws.Cells[l + 23, 1].Value = dr.GetValue(1).ToString();
                            ws.Cells[l + 23, 2].Value = ((DateTime)dr.GetSqlDateTime(2)).ToString("dd/MM/yy");
                            ws.Cells[l + 23, 3].Value = dr.GetValue(3).ToString();
                            ws.Cells[l + 23, 4].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                            ws.Cells[l + 23, 4].Value = dr.GetValue(4);
                            poids += (int)dr.GetSqlInt32(4);
                            l++;
                        }
                        else
                        {
                            ws.Cells[l + 23, 0].Value = dr.GetValue(0).ToString();
                            ws.Cells[l + 23, 1].Value = dr.GetValue(1).ToString();
                            ws.Cells[l + 23, 2].Value = ((DateTime)dr.GetSqlDateTime(2)).ToString("dd/MM/yy");
                            ws.Cells[l + 23, 3].Value = dr.GetValue(3).ToString();
                            ws.Cells[l + 23, 4].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                            ws.Cells[l + 23, 4].Value = dr.GetValue(4);
                            poids = (int)dr.GetSqlInt32(4);
                            produit = dr.GetValue(0).ToString();
                            l++;
                        }
                    }
                    dr.Close();
                    //Dernier sous-total
                    ws.Cells.GetSubrangeAbsolute(l + 23, 0, l + 23, 4).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells[l + 23, 3].Style.Font.Weight = 700;
                    ws.Cells[l + 23, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    ws.Cells[l + 23, 3].Value = "Total " + produit;
                    ws.Cells[l + 23, 4].Style.Font.Weight = 700;
                    ws.Cells[l + 23, 4].Style.Font.Size = 220;
                    ws.Cells[l + 23, 4].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                    ws.Cells[l + 23, 4].Value = poids;
                    total += poids;
                    //Total général
                    l++;
                    ws.Cells.GetSubrangeAbsolute(l + 23, 0, l + 23, 4).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells.GetSubrangeAbsolute(l + 23, 0, l + 23, 4).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells[l + 23, 3].Style.Font.Weight = 700;
                    ws.Cells[l + 23, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    ws.Cells[l + 23, 3].Value = "Total";
                    ws.Cells[l + 23, 4].Style.Font.Weight = 700;
                    ws.Cells[l + 23, 4].Style.Font.Size = 220;
                    ws.Cells[l + 23, 4].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                    ws.Cells[l + 23, 4].Value = total;
                    //Fermeture de la source de données
                    dr.Close();
                    ws.PrintOptions.FitWorksheetHeightToPages = 0;
                    ws.PrintOptions.FitWorksheetWidthToPages = 1;
                    ws.PrintOptions.FitToPage = true;
                    //------------------------------------------------------------------------------------------
                    //Traitement de la feuille répartition
                    f++;
                    //Si il a des données à traiter
                    bool exist = false;
                    sqlStr = "select count(*)"
                        + " from "
                        + " 	("
                        + " 		select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                        + " 		from tblRepartition"
                        + " 			inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        + " 		union all"
                        + " 		select RefRepartition, RefFournisseur, RefProduit, D"
                        + " 		from tblRepartition"
                        + " 		where RefCommandeFournisseur is null) as rep	   "
                        + "     inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition 	"
                        + " where RefFournisseur=" + cDT.RefEntite + " and D between  @debut and @fin";
                    cmd.CommandText = sqlStr;
                    exist = (cmd.ExecuteScalar().ToString() != "0");
                    if (!exist)
                    {
                        excelFile.Worksheets.Remove(f);
                        f--;
                    }
                    else
                    {
                        ws = excelFile.Worksheets[f];
                        ws.Cells[6, 3].Value += dDebut.ToString("MMMM yyyy");
                        ws.Cells[8, 3].Value = cDT.Libelle;
                        ws.Rows[8].AutoFit();
                        ws.Cells[9, 3].Value = adr.Adr1;
                        l = 10;
                        if (adr.Adr2 != "")
                        {
                            ws.Cells[l, 3].Value = adr.Adr2;
                            l++;
                        }
                        ws.Cells[l, 3].Value = adr.CodePostal + " " + adr.Ville;
                        l += 3;
                        ws.Cells[14, 3].Value = "";
                        ws.Rows[14].AutoFit();
                        ws.Cells[15, 3].Value += DateTime.Now.ToString("dd/MM/yyyy");
                        ws.Cells[17, 0].Value += cDT.CodeEE;
                        ws.Cells[18, 0].Value = "Contact : " + Utils.GetParametre(3, dbContext).ValeurTexte
                        + " - tél. : " + Utils.GetParametre(4, dbContext).ValeurTexte;
                        //Gestion des produits
                        //Création de la source de données
                        //Chaine SQL de récupération des données
                        sqlStr = "select '('+isnull(collectivite.CodeEE,'NR')+') '+collectivite.Libelle, tblProduit.NomCommun as Produit, sum(Poids) as Poids"
                            + " from tblProduit 	"
                            + "     inner join"
                            + "  		("
                            + "  		select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D"
                            + "   		from tblRepartition"
                            + "   			inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                            + "  		union all"
                            + "  		select RefRepartition, RefFournisseur, RefProduit, D"
                            + "   		from tblRepartition"
                            + "  		where RefCommandeFournisseur is null 		) as rep on rep.RefProduit=tblProduit.RefProduit 	   "
                            + "     inner join tblRepartitionCollectivite on tblRepartitionCollectivite.RefRepartition=rep.RefRepartition 	"
                            + "     inner join tblEntite as collectivite on tblRepartitionCollectivite.RefCollectivite=collectivite.RefEntite "
                            + " where RefFournisseur=" + cDT.RefEntite + " and D between  @debut and @fin"
                            + " group by '('+isnull(collectivite.CodeEE,'NR')+') '+collectivite.Libelle, tblProduit.NomCommun"
                            + " order by '('+isnull(collectivite.CodeEE,'NR')+') '+collectivite.Libelle, tblProduit.NomCommun";
                        //Chargement des données
                        cmd.CommandText = sqlStr;
                        string collectivite = "";
                        dr = cmd.ExecuteReader();
                        l = 0;
                        poids = 0;
                        total = 0;
                        //Inscription des données
                        while (dr.Read())
                        {
                            if (l > 0)  //On ne traite pas le changement de collectivité pour la première ligne
                            {
                                if (collectivite != dr.GetValue(0).ToString())
                                {
                                    //Changement de produit
                                    ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 4).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                                    ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 4).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                                    ws.Cells[l + 21, 1].Style.Font.Weight = 700;
                                    ws.Cells[l + 21, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    ws.Cells[l + 21, 1].Value = "Total " + collectivite;
                                    ws.Cells[l + 21, 4].Value = poids;
                                    total += poids;
                                    poids = 0;
                                    l++;
                                }
                                ws.Cells[l + 21, 0].Value = dr.GetValue(1).ToString();
                                ws.Cells[l + 21, 1].Value = dr.GetValue(0).ToString();
                                ws.Cells[l + 21, 4].Value = (int)dr.GetSqlInt32(2);
                                collectivite = dr.GetValue(0).ToString();
                                poids += (int)dr.GetSqlInt32(2);
                                l++;
                            }
                            else
                            {
                                ws.Cells[l + 21, 0].Value = dr.GetValue(1).ToString();
                                ws.Cells[l + 21, 1].Value = dr.GetValue(0).ToString();
                                ws.Cells[l + 21, 4].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                                ws.Cells[l + 21, 4].Value = (int)dr.GetSqlInt32(2);
                                collectivite = dr.GetValue(0).ToString();
                                poids = (int)dr.GetSqlInt32(2);
                                produit = dr.GetValue(0).ToString();
                                l++;
                            }
                        }
                        //Dernier sous-total
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 4).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells[l + 21, 1].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 1].Value = "Total " + collectivite;
                        ws.Cells[l + 23, 4].Value = poids;
                        total += poids;
                        //Total général
                        l++;
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 4).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells.GetSubrangeAbsolute(l + 21, 0, l + 21, 4).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells[l + 21, 1].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 21, 1].Value = "Total";
                        ws.Cells[l + 21, 4].Style.Font.Weight = 700;
                        ws.Cells[l + 21, 4].Style.Font.Size = 220;
                        ws.Cells[l + 21, 4].Style.NumberFormat = NumberFormatBuilder.Number(0, useThousandsSeparator: true);
                        ws.Cells[l + 21, 4].Value = total;
                        //Fermeture de la source de données
                        dr.Close();
                        //Suppression des lignes en trop
                        for (i = 120; i > l + 21; i--)
                        {
                            ws.Rows.Remove(l + 22);
                        }
                        ws.PrintOptions.FitWorksheetHeightToPages = 0;
                        ws.PrintOptions.FitWorksheetWidthToPages = 1;
                        ws.PrintOptions.FitToPage = true;
                    }
                    f++;
                    //------------------------------------------------------------------------------------------
                    //Traitement de la feuille Hors collecte
                    //Si il a des données à traiter
                    exist = false;
                    sqlStr = "select count(*)"
                        + " from "
                        + "  		("
                        + "  		select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D, tblCommandeFournisseur.RefCommandeFournisseur"
                        + "   		from tblRepartition"
                        + "   			inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                        + "  		union all"
                        + "  		select RefRepartition, RefFournisseur, RefProduit, D, null as RefCommandeFournisseur"
                        + "   		from tblRepartition"
                        + "  		where RefCommandeFournisseur is null 		) as rep"
                        + " 	inner join tblRepartitionProduit on tblRepartitionProduit.RefRepartition=rep.RefRepartition"
                        + "     left join tblEntite on tblRepartitionProduit.RefFournisseur=tblEntite.RefEntite"
                        + " where tblEntite.RefEntite is null and rep.RefFournisseur=" + cDT.RefEntite + " and rep.D between  @debut and @fin";
                    cmd.CommandText = sqlStr;
                    exist = (cmd.ExecuteScalar().ToString() != "0");
                    if (!exist)
                    {
                        excelFile.Worksheets.Remove(f);
                        f--;
                    }
                    else
                    {
                        ws = excelFile.Worksheets[f];
                        ws.Cells[6, 3].Value += dDebut.ToString("MMMM yyyy");
                        ws.Cells[8, 3].Value = cDT.Libelle;
                        ws.Rows[8].AutoFit();
                        ws.Cells[9, 3].Value = adr.Adr1;
                        l = 10;
                        if (adr.Adr2 != "")
                        {
                            ws.Cells[l, 3].Value = adr.Adr2;
                            l++;
                        }
                        ws.Cells[l, 3].Value = adr.CodePostal + " " + adr.Ville;
                        l += 3;
                        ws.Cells[14, 3].Value = "";
                        ws.Rows[14].AutoFit();
                        ws.Cells[15, 3].Value += DateTime.Now.ToString("dd/MM/yyyy");
                        ws.Cells[17, 0].Value += cDT.CodeEE;
                        ws.Cells[18, 0].Value = "Contact : " + Utils.GetParametre(3, dbContext).ValeurTexte
                        + " - tél. : " + Utils.GetParametre(4, dbContext).ValeurTexte;
                        //Gestion des produits
                        //Création de la source de données
                        //Chaine SQL de récupération des données
                        sqlStr = "select tblProduit.NomCommun, tblCommandeFournisseur.NumeroCommande, rep.D, null as NomCommun, Poids, PUHT"
                            + " from tblProduit 	"
                            + " 	inner join"
                            + "  			("
                            + "  			select RefRepartition, tblCommandeFournisseur.RefEntite as RefFournisseur, tblCommandeFournisseur.RefProduit, tblCommandeFournisseur.DDechargement as D, tblCommandeFournisseur.RefCommandeFournisseur"
                            + "   			from tblRepartition"
                            + "   				inner join tblCommandeFournisseur on tblCommandeFournisseur.RefCommandeFournisseur=tblRepartition.RefCommandeFournisseur"
                            + "  			union all"
                            + "  			select RefRepartition, RefFournisseur, RefProduit, D, null as RefCommandeFournisseur"
                            + "   			from tblRepartition"
                            + "  			where RefCommandeFournisseur is null 		) as rep on rep.RefProduit=tblProduit.RefProduit 	   "
                            + " 	inner join tblRepartitionProduit on tblRepartitionProduit.RefRepartition=rep.RefRepartition 	"
                            + " 	left join tblCommandeFournisseur on rep.RefCommandeFournisseur=tblCommandeFournisseur.RefCommandeFournisseur"
                            + "     left join tblEntite on tblRepartitionProduit.RefFournisseur=tblEntite.RefEntite"
                            + " where tblEntite.RefEntite is null and rep.RefFournisseur=" + cDT.RefEntite + " and rep.D between  @debut and @fin"
                            + " order by tblProduit.NomCommun, tblCommandeFournisseur.NumeroCommande, rep.D";
                        //Chargement des données
                        produit = "";
                        poids = 0;
                        int poidsTotal = 0;
                        decimal prix = 0, prixTotal = 0;
                        cmd.CommandText = sqlStr;
                        dr = cmd.ExecuteReader();
                        l = 0;
                        while (dr.Read())
                        {
                            ws.Cells[l + 23, 0].Style.Font.Size = 220;
                            ws.Cells[l + 23, 0].Value = dr.GetValue(0).ToString();
                            ws.Cells[l + 23, 1].Style.Font.Size = 220;
                            ws.Cells[l + 23, 1].Value = dr.GetValue(1).ToString();
                            ws.Cells[l + 23, 2].Style.Font.Size = 220;
                            ws.Cells[l + 23, 2].Value = ((DateTime)dr.GetSqlDateTime(2)).ToString("dd/MM/yy");
                            ws.Cells[l + 23, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            ws.Cells[l + 23, 3].Style.Font.Size = 220;
                            ws.Cells[l + 23, 3].Value = dr.GetValue(4).ToString();
                            ws.Cells[l + 23, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            ws.Cells[l + 23, 4].Style.Font.Size = 220;
                            ws.Cells[l + 23, 4].Value = dr.GetValue(5).ToString();
                            ws.Cells[l + 23, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            ws.Cells[l + 23, 5].Style.NumberFormat = "#,##0.00";
                            if (dr.GetValue(5) != DBNull.Value)
                            {
                                ws.Cells[l + 23, 5].Value = Math.Round((Convert.ToDecimal((int)dr.GetValue(4)) / 1000) * (decimal)dr.GetValue(5), 2, MidpointRounding.AwayFromZero);
                            }
                            poidsTotal += (int)dr.GetSqlInt32(4);
                            //prixTotal += Math.Round((Convert.ToDecimal((int)dr.GetValue(4)) / 1000) * (decimal)dr.GetValue(5), 2, MidpointRounding.AwayFromZero);
                            l++;
                        }
                        dr.Close();
                        //Total général
                        ws.Cells.GetSubrangeAbsolute(l + 23, 0, l + 23, 6).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells.GetSubrangeAbsolute(l + 23, 0, l + 23, 6).Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        ws.Cells[l + 23, 2].Style.Font.Weight = 700;
                        ws.Cells[l + 23, 2].Style.Font.Size = 220;
                        ws.Cells[l + 23, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 23, 2].Value = "Total";
                        ws.Cells[l + 23, 3].Style.Font.Weight = 700;
                        ws.Cells[l + 23, 3].Style.Font.Size = 220;
                        ws.Cells[l + 23, 3].Value = poidsTotal;
                        ws.Cells[l + 23, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        ws.Cells[l + 23, 5].Style.Font.Weight = 700;
                        ws.Cells[l + 23, 5].Style.Font.Size = 220;
                        ws.Cells[l + 23, 5].Style.NumberFormat = "#,##0.00";
                        //ws.Cells[l + 23, 5].Value = prixTotal;
                        //Fermeture de la source de données
                        dr.Close();
                        //Inscription du bon texte facture
                        if (prixTotal < 0) { ws.Cells[20, 0].Value = "Cet état sert de base à Valorplast pour vous établir une facture."; }
                        //Suppression des lignes en trop
                        for (i = 0; i < 100; i++)
                        {
                            ws.Rows.Remove(l + 24);
                        }
                        ws.PrintOptions.FitWorksheetHeightToPages = 0;
                        ws.PrintOptions.FitWorksheetWidthToPages = 1;
                        ws.PrintOptions.FitToPage = true;
                    }
                    //Feuille suivante
                    f++;
                }
                //Suppression des dernières feuilles
                excelFile.Worksheets.Remove(f);
                excelFile.Worksheets.Remove(f);
                excelFile.Worksheets.Remove(f);
            }
            //Sauvegarde du fichier
            if (fileType == "xlsx")
            {
                excelFile.Save(ms, SaveOptions.XlsxDefault);
            }
            else
            {
                PdfConformanceLevel conformanceLevel = PdfConformanceLevel.PdfA2a;
                excelFile.Save(ms, new PdfSaveOptions()
                {
                    SelectionType = SelectionType.EntireFile,
                    ConformanceLevel = conformanceLevel
                });
            }
            return ms;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create Statistique
        /// </summary>
        public static MemoryStream CreateStatistique(string name, DataSet dsStat, EnvStatisticsFilters eSF
            , Context currentContext, ApplicationDbContext dbContext, string rootPath)
        {
            //Init
            MemoryStream ms = new();
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            ExcelFile excelFile;
            //Chargement du document avec logo ou non
            if (name == Enumerations.StatType.DonneeCDT.ToString()
                || name == Enumerations.StatType.ExtractionRSE.ToString()
                || name == Enumerations.StatType.ExtractionPrixReprise.ToString()
                || name == Enumerations.StatType.DestinationTonnage.ToString()
                || name == Enumerations.StatType.DeboucheBalle.ToString()
                || name == Enumerations.StatType.EvolutionTonnage.ToString()
                || (currentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite > 0 && name == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString())
                || (currentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite > 0 && name == Enumerations.MenuName.LogistiqueMenuEtatDesEnlevements.ToString())
                || (currentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite > 0 && name == Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString())
                || name == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString()
                || name == Enumerations.MenuName.AdministrationMenuExtractionLogin.ToString()
                )
            {
                excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\ValorplastAvecLogo.xlsx");            //Création de la feuille
            }
            else
            {
                excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\Valorplast.xlsx");            //Création de la feuille
            }
            GemBox.Spreadsheet.ExcelWorksheet ws = excelFile.Worksheets[0];
            int i = 0;
            int j = 0;
            int lastCriteriaRowIndex = 2;
            int firstDataRowIndex = 3;
            int lastDataRowIndex = 0;
            //Styles
            //Titre
            CellStyle styleTitre = new(excelFile)
            {
                Font = new ExcelFont()
                {
                    Weight = ExcelFont.BoldWeight,
                    Size = 20 * 20
                }
            };

            //Critères
            CellStyle styleCritere = new(excelFile)
            {
                Font = new ExcelFont()
                {
                    //Weight = ExcelFont.BoldWeight,
                    Size = 14 * 20
                }
            };

            //Critères (titre)
            CellStyle styleCritereTitre = new(excelFile)
            {
                Font = new ExcelFont()
                {
                    Name = "Arial",
                    Size = 12 * 20
                },
                HorizontalAlignment = HorizontalAlignmentStyle.Right
            };

            //Critères (liste)
            CellStyle styleCritereListe = new(excelFile)
            {
                Font = new ExcelFont()
                {
                    Name = "Arial",
                    Size = 12 * 20
                },
            };
            CellStyle styleEnTeteColonne = new(excelFile);
            styleEnTeteColonne.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
            styleEnTeteColonne.Font.Weight = ExcelFont.BoldWeight;
            styleEnTeteColonne.Font.Name = "Arial";
            styleEnTeteColonne.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            styleEnTeteColonne.WrapText = true;
            CellStyle styleSousTotal = new(excelFile);
            styleSousTotal.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
            styleSousTotal.Font.Weight = ExcelFont.BoldWeight;
            styleSousTotal.Font.Name = "Arial";
            styleSousTotal.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            CellStyle styleEnTeteGroupe = new(excelFile)
            {
                Font = new ExcelFont()
                {
                    Name = "Arial",
                    Weight = ExcelFont.BoldWeight,
                    Size = 12 * 20
                },
                HorizontalAlignment = HorizontalAlignmentStyle.Right
            };
            DataTable dT = dsStat.Tables[0];
            //Suppression des colonnes inutiles
            if (name != Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString()
                 && name != Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
            {
                for (int c = dT.Columns.Count - 1; c >= 0; c--)
                {
                    if (currentContext.EnvDataColumns.ContainsKey(dT.Columns[c].ColumnName))
                    {
                        if (string.IsNullOrEmpty(currentContext.EnvDataColumns[dT.Columns[c].ColumnName].CulturedCaption)) { dT.Columns.Remove(dT.Columns[c]); }
                    }
                }
            }
            //Création de la feuille
            if (name == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString())
            {
                ws.Name = currentContext.CulturedRessources.GetTextRessource(960);
            }
            else if (name == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
            {
                ws.Name = currentContext.CulturedRessources.GetTextRessource(1055);
            }
            else
            {
                ws.Name = currentContext.CulturedRessources.GetTextRessource(1036);
            }
            j = 0;
            i = 2;
            //Nom des colonnes
            for (int c = 0; c < dT.Columns.Count; c++)
            {
                if (currentContext.EnvDataColumns.ContainsKey(dT.Columns[c].ColumnName))
                {
                    dT.Columns[c].Caption = currentContext.EnvDataColumns[dT.Columns[c].ColumnName].CulturedCaption;
                }
            }
            //Mise en forme des en-têtes de colonnes en cas de headers multiples
            if (name == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString() && dT.Columns.Count > 2)
            {
                int span = 1;
                if (dT.Columns[2].Caption.LastIndexOf("#/#") >= 0)
                {
                    ws.Cells[i, 2].Value = dT.Columns[2].Caption.Substring(0, dT.Columns[2].Caption.LastIndexOf("#/#"));
                    ws.Cells[i, 2].Style = styleEnTeteColonne;
                }
                for (int c = 3; c < dT.Columns.Count; c++)
                {
                    //Changement de produit
                    if (dT.Columns[c - span].Caption.Substring(0, dT.Columns[c - span].Caption.LastIndexOf("#/#")) != dT.Columns[c].Caption.Substring(0, dT.Columns[c].Caption.LastIndexOf("#/#")))
                    {
                        //Si on change de produit, on span la cellule
                        ws.Cells.GetSubrangeRelative(i, c - span, span, 1).Merged = true;
                        ws.Cells[i, c].Value = dT.Columns[c].Caption.Substring(0, dT.Columns[c].Caption.LastIndexOf("#/#"));
                        span = 0;
                    }
                    //Formattage de la cellule
                    ws.Cells[i, c].Style = styleEnTeteColonne;
                    //Span
                    span++;
                }
                //Span dernière cellule
                if (span > 0)
                {
                    ws.Cells.GetSubrangeRelative(i, dT.Columns.Count - span, span, 1).Merged = true;
                }
                i++;
            }
            //Header simple
            if (name != Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString()
                && name != Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
            {
                for (j = 0; j < dT.Columns.Count; j++)
                {
                    if (name == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString() && j >= 2
                        && dT.Columns[j].Caption.LastIndexOf("#/#") >= 0)
                    {
                        ws.Cells[i, j].Value = (dT.Columns[j].Caption.Substring(dT.Columns[j].Caption.LastIndexOf("#/#") + 3, dT.Columns[j].Caption.Length - dT.Columns[j].Caption.LastIndexOf("#/#") - 3));
                    }
                    else
                    {
                        ws.Cells[i, j].Value = dT.Columns[j].Caption;
                    }
                    ws.Cells[i, j].Style = styleEnTeteColonne;
                }
                i++;
            }
            //Inscription des valeurs
            if (name == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString())
            {
                //Initialisations
                int l = 2;
                DataTable dV = new();
                dV.Columns.Add("Libelle", typeof(string));
                object[] rowVals = new object[1];
                rowVals[0] = currentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingTypeLibelle.ToString()].Name;
                dV.Rows.Add(rowVals);
                rowVals[0] = currentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingLibelle.ToString()].Name;
                dV.Rows.Add(rowVals);
                //Tableau des regroupements
                string[] rgs = new string[2];
                rgs[0] = "";
                rgs[1] = "";
                bool premiereLigne = true;
                //Figeage des volets
                ws.Panes = new WorksheetPanes(PanesState.Frozen, 1, l + 4, "B" + (l + 5).ToString(), PanePosition.BottomRight);
                //Inscription des en-têtes de colonne
                ws.Columns[0].Width = 70 * 256;
                //Largeur de toutes les colonnes de données
                for (int a = 1; a <= 15; a++)
                {
                    ws.Columns[a].Width = 15 * 256;
                }
                ////En-têtes de colonnes
                ws.Cells[l, 0].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].CulturedCaption;
                ws.Cells[l, 1].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.PaysDestinbationLibelle.ToString()].CulturedCaption;
                ws.Cells[l, 2].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageAnnuelPrecedent.ToString()].CulturedCaption;
                ws.Cells[l, 3].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Janvier.ToString()].CulturedCaption;
                ws.Cells[l, 4].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Fevrier.ToString()].CulturedCaption;
                ws.Cells[l, 5].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Mars.ToString()].CulturedCaption;
                ws.Cells[l, 6].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Avril.ToString()].CulturedCaption;
                ws.Cells[l, 7].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Mai.ToString()].CulturedCaption;
                ws.Cells[l, 8].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Juin.ToString()].CulturedCaption;
                ws.Cells[l, 9].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Juillet.ToString()].CulturedCaption;
                ws.Cells[l, 10].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Aout.ToString()].CulturedCaption;
                ws.Cells[l, 11].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Septembre.ToString()].CulturedCaption;
                ws.Cells[l, 12].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Octobre.ToString()].CulturedCaption;
                ws.Cells[l, 13].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Novembre.ToString()].CulturedCaption;
                ws.Cells[l, 14].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Decembre.ToString()].CulturedCaption;
                ws.Cells[l, 15].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageAnnuel.ToString()].CulturedCaption;
                for (int k = 0; k <= 15; k++)
                {
                    CellStyle style = ws.Cells[l, k].Style;
                    style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                    style.Font.Weight = ExcelFont.BoldWeight;
                    style.WrapText = true;
                    style.VerticalAlignment = VerticalAlignmentStyle.Center;
                    if (k > 0) { style.HorizontalAlignment = HorizontalAlignmentStyle.Center; }
                }
                l++;
                //Logo
                ws.Pictures.Add(rootPath + @"\Assets\Images\logo-imprimable.jpg", "O1", 128, 72, LengthUnit.Pixel);
                //Inscription des valeurs
                for (int r = 0; r < dT.Rows.Count; r++)
                {
                    DataRow dRow = dT.Rows[r];
                    //En-tête et pieds de grupe de groupe si au moins un regroupement
                    if (dV.Rows.Count > 0)
                    {
                        //Pour chaque niveau de regroupement qui change, on affiche le sous-total, le cas échéant
                        //SAUF PREMIERE LIGNE
                        for (i = dV.Rows.Count - 1; i >= 0; i--)
                        {
                            if (rgs[i] != dRow[i].ToString() && !premiereLigne)
                            {
                                ws.Cells[l, 0].Value = currentContext.CulturedRessources.GetTextRessource(778) + " - " + dT.Rows[r - 1][i];
                                ws.Cells[l, 0].Style = styleSousTotal;
                                //Valeurs
                                string filtre = "1=1 ";
                                for (int c = 0; c <= i; c++)
                                {
                                    filtre += " and " + dV.Rows[c]["Libelle"].ToString()
                                        + (dT.Rows[r - 1][c].ToString() == "" ? " is null" : " = '" + dT.Rows[r - 1][c] + "'");
                                }
                                //Initialisation du tableau de total
                                DataRow tot = dT.NewRow();
                                for (int t = 4; t < 18; t++)
                                {
                                    tot[t] = 0;
                                }
                                //Calcul du total
                                foreach (DataRow dR in dT.Select(filtre))
                                {
                                    for (int t = 4; t < 18; t++)
                                    {
                                        tot[t] = ((tot[t] == System.DBNull.Value ? 0 : (decimal)tot[t]) + (dR[t] == System.DBNull.Value ? 0 : (decimal)dR[t]));
                                    }
                                }
                                //Inscription
                                ws.Cells[l, 2].Value = tot[4];
                                ws.Cells[l, 3].Value = tot[5];
                                ws.Cells[l, 4].Value = tot[6];
                                ws.Cells[l, 5].Value = tot[7];
                                ws.Cells[l, 6].Value = tot[8];
                                ws.Cells[l, 7].Value = tot[9];
                                ws.Cells[l, 8].Value = tot[10];
                                ws.Cells[l, 9].Value = tot[11];
                                ws.Cells[l, 10].Value = tot[12];
                                ws.Cells[l, 11].Value = tot[13];
                                ws.Cells[l, 12].Value = tot[14];
                                ws.Cells[l, 13].Value = tot[15];
                                ws.Cells[l, 14].Value = tot[16];
                                ws.Cells[l, 15].Value = tot[17];
                                //colorisation sauf premier niveau de rg
                                for (int k = 0; k <= 15; k++)
                                {
                                    CellStyle style = ws.Cells[l, k].Style;
                                    if (dT.Rows[r - 1][currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name] != DBNull.Value && i != 0)
                                    {
                                        style.Font.Color = System.Drawing.Color.FromArgb(int.Parse(dT.Rows[r - 1][currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name].ToString().Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier));
                                    }
                                    style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                    style.Font.Weight = ExcelFont.BoldWeight;
                                    style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    if (k >= 2) { style.NumberFormat = NumberFormatBuilder.Number(0, 1, true); }
                                }
                                //Ligne suivante
                                l++;
                            }
                        }
                        //Pour chaque niveau de regroupement qui change, on affiche l'en-tête de groupe
                        for (i = 0; i < dV.Rows.Count; i++)
                        {
                            if (rgs[i] != dRow[i].ToString() || premiereLigne)
                            {
                                ws.Cells[l, 0].Value = dRow[i];
                                CellStyle style = ws.Cells[l, 0].Style;
                                style.Font.Weight = ExcelFont.BoldWeight;
                                style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                //style.Font.Size = 12*20;
                                //colorisation pour le niveau 2 seulement
                                if (dRow[currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name] != DBNull.Value && i != 0)
                                {
                                    for (int k = 0; k <= 15; k++)
                                    {
                                        CellStyle styleC = ws.Cells[l, k].Style;
                                        styleC.Font.Color = System.Drawing.Color.FromArgb(int.Parse(dRow[currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name].ToString().Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier));
                                    }
                                }
                                //Saut de page
                                if (dRow[currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name] != DBNull.Value && i == 0 && !premiereLigne)
                                {
                                    ws.HorizontalPageBreaks.Add(l);
                                }
                                //Ligne suivante
                                l++;
                            }
                        }
                    }
                    //Enregistrement des nouveaux en-têtes de groupe courants
                    for (i = 0; i < dV.Rows.Count; i++)
                    {
                        rgs[i] = dRow[i].ToString();
                    }
                    //Valeurs
                    ws.Cells[l, 0].Value = dRow[2];
                    ws.Cells[l, 1].Value = dRow[3];
                    ws.Cells[l, 2].Value = dRow[4];
                    ws.Cells[l, 3].Value = dRow[5];
                    ws.Cells[l, 4].Value = dRow[6];
                    ws.Cells[l, 5].Value = dRow[7];
                    ws.Cells[l, 6].Value = dRow[8];
                    ws.Cells[l, 7].Value = dRow[9];
                    ws.Cells[l, 8].Value = dRow[10];
                    ws.Cells[l, 9].Value = dRow[11];
                    ws.Cells[l, 10].Value = dRow[12];
                    ws.Cells[l, 11].Value = dRow[13];
                    ws.Cells[l, 12].Value = dRow[14];
                    ws.Cells[l, 13].Value = dRow[15];
                    ws.Cells[l, 14].Value = dRow[16];
                    ws.Cells[l, 15].Value = dRow[17];
                    //colorisation
                    if (dRow[currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name] != DBNull.Value)
                    {
                        for (int k = 0; k <= 15; k++)
                        {
                            CellStyle style = ws.Cells[l, k].Style;
                            style.Font.Color = System.Drawing.Color.FromArgb(int.Parse(dRow[currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name].ToString().Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier));
                            style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                            if (k >= 2) { style.NumberFormat = NumberFormatBuilder.Number(0, 1, true); }
                        }
                    }
                    //Suivant
                    l++;
                    premiereLigne = false;
                }
                //Dernier sous-total
                if (dV.Rows.Count - 1 > 0)
                {
                    //Pour chaque niveau de regroupement qui change, on affiche le sous-total, le cas échéant
                    //SAUF PREMIERE LIGNE
                    for (i = dV.Rows.Count - 1; i >= 0; i--)
                    {
                        if (!premiereLigne)
                        {
                            ws.Cells[l, 0].Value = currentContext.CulturedRessources.GetTextRessource(778) + " - " + dT.Rows[dT.Rows.Count - 1][i];
                            ws.Cells[l, 0].Style = styleSousTotal;
                            //Valeurs
                            string filtre = "1=1 ";
                            for (int c = 0; c <= i; c++)
                            {
                                filtre += " and " + dV.Rows[c]["Libelle"].ToString()
                                    + (dT.Rows[dT.Rows.Count - 1][c].ToString() == "" ? " is null" : " = '" + dT.Rows[dT.Rows.Count - 1][c] + "'");
                            }
                            //Initialisation du tableau de total
                            DataRow tot = dT.NewRow();
                            for (int t = 4; t < 18; t++)
                            {
                                tot[t] = 0;
                            }
                            //Calcul du total
                            foreach (DataRow dR in dT.Select(filtre))
                            {
                                for (int t = 4; t < 18; t++)
                                {
                                    tot[t] = ((tot[t] == System.DBNull.Value ? 0 : (decimal)tot[t]) + (dR[t] == System.DBNull.Value ? 0 : (decimal)dR[t]));
                                }
                            }
                            //Inscription
                            ws.Cells[l, 2].Value = tot[4];
                            ws.Cells[l, 3].Value = tot[5];
                            ws.Cells[l, 4].Value = tot[6];
                            ws.Cells[l, 5].Value = tot[7];
                            ws.Cells[l, 6].Value = tot[8];
                            ws.Cells[l, 7].Value = tot[9];
                            ws.Cells[l, 8].Value = tot[10];
                            ws.Cells[l, 9].Value = tot[11];
                            ws.Cells[l, 10].Value = tot[12];
                            ws.Cells[l, 11].Value = tot[13];
                            ws.Cells[l, 12].Value = tot[14];
                            ws.Cells[l, 13].Value = tot[15];
                            ws.Cells[l, 14].Value = tot[16];
                            ws.Cells[l, 15].Value = tot[17];
                            //colorisation
                            for (int k = 0; k <= 15; k++)
                            {
                                CellStyle style = ws.Cells[l, k].Style;
                                if (dT.Rows[dT.Rows.Count - 1][currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name] != DBNull.Value && i != 0)
                                {
                                    style.Font.Color = System.Drawing.Color.FromArgb(int.Parse(dT.Rows[dT.Rows.Count - 1][currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name].ToString().Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier));
                                }
                                if (k >= 2) { style.NumberFormat = NumberFormatBuilder.Number(0, 1, true); }
                                style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                style.Font.Weight = ExcelFont.BoldWeight;
                                style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            }
                            //Ligne suivante
                            l++;
                        }
                    }
                }
                //Total TSR
                if (dV.Rows.Count > 0)
                {
                    ws.Cells[l, 0].Value = currentContext.CulturedRessources.GetTextRessource(778) + " " + currentContext.CulturedRessources.GetTextRessource(1037);
                    //Valeurs
                    //Initialisation du tableau de total
                    DataRow tot = dT.NewRow();
                    for (int t = 4; t < 18; t++)
                    {
                        tot[t] = 0;
                    }
                    string filtre = currentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "='TSR/Ecart de pesée'";
                    //Calcul du total
                    foreach (DataRow dR in dT.Select(filtre))
                    {
                        for (int t = 4; t < 18; t++)
                        {
                            tot[t] = ((tot[t] == System.DBNull.Value ? 0 : (decimal)tot[t]) + (dR[t] == System.DBNull.Value ? 0 : (decimal)dR[t]));
                        }
                    }
                    //Inscription
                    ws.Cells[l, 2].Value = tot[4];
                    ws.Cells[l, 3].Value = tot[5];
                    ws.Cells[l, 4].Value = tot[6];
                    ws.Cells[l, 5].Value = tot[7];
                    ws.Cells[l, 6].Value = tot[8];
                    ws.Cells[l, 7].Value = tot[9];
                    ws.Cells[l, 8].Value = tot[10];
                    ws.Cells[l, 9].Value = tot[11];
                    ws.Cells[l, 10].Value = tot[12];
                    ws.Cells[l, 11].Value = tot[13];
                    ws.Cells[l, 12].Value = tot[14];
                    ws.Cells[l, 13].Value = tot[15];
                    ws.Cells[l, 14].Value = tot[16];
                    ws.Cells[l, 15].Value = tot[17];
                    //Format
                    for (int k = 0; k <= 15; k++)
                    {
                        CellStyle style = ws.Cells[l, k].Style;
                        if (k >= 2) { style.NumberFormat = NumberFormatBuilder.Number(0, 1, true); }
                        style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                        style.Font.Weight = ExcelFont.BoldWeight;
                        style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    }
                    //Suivant
                    l++;
                }
                //Total général hors TSR
                if (dV.Rows.Count > 0)
                {
                    ws.Cells[l, 0].Value = currentContext.CulturedRessources.GetTextRessource(779);
                    //Valeurs
                    //Initialisation du tableau de total
                    DataRow tot = dT.NewRow();
                    for (int t = 4; t < 18; t++)
                    {
                        tot[t] = 0;
                    }
                    string filtre = currentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + "<>'TSR/Ecart de pesée' or " + currentContext.EnvDataColumns[Enumerations.DataColumnName.ClientLibelle.ToString()].Name + " is null";
                    //Calcul du total
                    foreach (DataRow dR in dT.Select(filtre))
                    {
                        for (int t = 4; t < 18; t++)
                        {
                            tot[t] = ((tot[t] == System.DBNull.Value ? 0 : (decimal)tot[t]) + (dR[t] == System.DBNull.Value ? 0 : (decimal)dR[t]));
                        }
                    }
                    //Inscription
                    ws.Cells[l, 2].Value = tot[4];
                    ws.Cells[l, 3].Value = tot[5];
                    ws.Cells[l, 4].Value = tot[6];
                    ws.Cells[l, 5].Value = tot[7];
                    ws.Cells[l, 6].Value = tot[8];
                    ws.Cells[l, 7].Value = tot[9];
                    ws.Cells[l, 8].Value = tot[10];
                    ws.Cells[l, 9].Value = tot[11];
                    ws.Cells[l, 10].Value = tot[12];
                    ws.Cells[l, 11].Value = tot[13];
                    ws.Cells[l, 12].Value = tot[14];
                    ws.Cells[l, 13].Value = tot[15];
                    ws.Cells[l, 14].Value = tot[16];
                    ws.Cells[l, 15].Value = tot[17];
                    //Format
                    for (int k = 0; k <= 15; k++)
                    {
                        CellStyle style = ws.Cells[l, k].Style;
                        if (k >= 2) { style.NumberFormat = NumberFormatBuilder.Number(0, 1, true); }
                        style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                        style.Font.Weight = ExcelFont.BoldWeight;
                        style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    }
                }
                lastDataRowIndex = l;
            }
            else if (name == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
            {
                //Initialisations
                int l = 2;
                DataTable dV = new();
                dV.Columns.Add("Libelle", typeof(string));
                object[] rowVals = new object[1];
                rowVals[0] = currentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingTypeLibelle.ToString()].Name;
                dV.Rows.Add(rowVals);
                rowVals[0] = currentContext.EnvDataColumns[Enumerations.DataColumnName.ProduitGroupeReportingLibelle.ToString()].Name;
                dV.Rows.Add(rowVals);
                rowVals[0] = currentContext.EnvDataColumns[Enumerations.DataColumnName.RegionReportingLibelle.ToString()].Name;
                dV.Rows.Add(rowVals);
                //Tableau des regroupements
                string[] rgs = new string[3];
                for (i = 0; i < 3; i++)
                {
                    rgs[i] = "";
                }
                bool premiereLigne = true;
                bool changementRg = false;
                //Figeage des volets
                ws.Panes = new WorksheetPanes(PanesState.Frozen, 1, l + 4, "B" + (l + 5).ToString(), PanePosition.BottomRight);
                //Inscription des en-têtes de colonne
                ws.Columns[0].Width = 60 * 256;
                //Largeur de toutes les colonnes de données
                for (int a = 1; a <= 28; a++)
                {
                    if (a % 2 == 0)
                    {
                        ws.Columns[a].Width = 10 * 256;
                    }
                    else
                    {
                        ws.Columns[a].Width = 7 * 256;
                    }
                }
                //En-têtes de colonnes
                ws.Cells[l, 0].Value = "Lieu de régénération";
                ws.Cells[l, 1].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageAnnuelPrecedent.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 1, 2, 1).Merged = true;
                ws.Cells[l, 3].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Janvier.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 3, 2, 1).Merged = true;
                ws.Cells[l, 5].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Fevrier.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 5, 2, 1).Merged = true;
                ws.Cells[l, 7].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Mars.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 7, 2, 1).Merged = true;
                ws.Cells[l, 9].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Avril.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 9, 2, 1).Merged = true;
                ws.Cells[l, 11].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Mai.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 11, 2, 1).Merged = true;
                ws.Cells[l, 13].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Juin.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 13, 2, 1).Merged = true;
                ws.Cells[l, 15].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Juillet.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 15, 2, 1).Merged = true;
                ws.Cells[l, 17].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Aout.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 17, 2, 1).Merged = true;
                ws.Cells[l, 19].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Septembre.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 19, 2, 1).Merged = true;
                ws.Cells[l, 21].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Octobre.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 21, 2, 1).Merged = true;
                ws.Cells[l, 23].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Novembre.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 23, 2, 1).Merged = true;
                ws.Cells[l, 25].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.Decembre.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 25, 2, 1).Merged = true;
                ws.Cells[l, 27].Value = currentContext.EnvDataColumns[Enumerations.DataColumnName.TonnageAnnuel.ToString()].CulturedCaption;
                ws.Cells.GetSubrangeRelative(l, 27, 2, 1).Merged = true;
                for (int k = 0; k <= 28; k++)
                {
                    CellStyle style = ws.Cells[l, k].Style;
                    style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                    style.Font.Weight = ExcelFont.BoldWeight;
                    style.WrapText = true;
                    style.VerticalAlignment = VerticalAlignmentStyle.Center;
                    if (k > 0)
                    {
                        style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    }
                    else
                    {
                        style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    }
                }
                l++;
                //Logo
                ws.Pictures.Add(rootPath + @"\Assets\Images\logo-imprimable.jpg", "Z1", 128, 72, LengthUnit.Pixel);
                //Inscription des valeurs
                SqlConnection sqlConn = (SqlConnection)dbContext.Database.GetDbConnection();
                for (int r = 0; r < dT.Rows.Count; r++)
                {
                    DataRow dRow = dT.Rows[r];
                    //En-tête et pieds de grupe de groupe si au moins un regroupement
                    if (dV.Rows.Count > 0)
                    {
                        //Pour chaque niveau de regroupement qui change, on affiche le sous-total, le cas échéant
                        //SAUF PREMIERE LIGNE
                        for (i = dV.Rows.Count - 1; i >= 0; i--)
                        {
                            //Test du changement de niveau de regroupement
                            changementRg = false;
                            for (int w = i; w >= 0; w--)
                            {
                                if (rgs[w] != dRow[w].ToString()) { changementRg = true; }
                            }
                            if (changementRg && !premiereLigne)
                            {
                                //Ajout tableau de synthèse si niveau de regroupement 0
                                if (i == 0)
                                {
                                    DataSet dsSynthese = new();
                                    sqlConn.Open();
                                    SqlCommand selectStat = new("DestinationProduitClientParTypeDeGroupe", sqlConn);
                                    selectStat.CommandTimeout = 30;
                                    selectStat.CommandType = CommandType.StoredProcedure;
                                    SqlDataAdapter adapterStat = new();
                                    adapterStat.SelectCommand = selectStat;
                                    //Dates 
                                    if (eSF.FilterYears != "") { selectStat.Parameters.Add("@anneeRef", SqlDbType.Int).Value = System.Convert.ToInt32(eSF.FilterYears) - 1; }
                                    else { selectStat.Parameters.Add("@anneeRef", SqlDbType.Int).Value = DBNull.Value; }
                                    if (eSF.FilterYears != "") { selectStat.Parameters.Add("@annee", SqlDbType.Int).Value = eSF.FilterYears; }
                                    else { selectStat.Parameters.Add("@annee", SqlDbType.Int).Value = DBNull.Value; }
                                    if (dT.Rows[r - 1][0] != DBNull.Value) { selectStat.Parameters.Add("@produitGroupeReportingType", SqlDbType.NVarChar).Value = dT.Rows[r - 1][0].ToString(); }
                                    else { selectStat.Parameters.Add("@produitGroupeReportingType", SqlDbType.NVarChar).Value = DBNull.Value; }
                                    if (eSF.FilterMonths != "") { selectStat.Parameters.Add("@moisFin", SqlDbType.Int).Value = eSF.FilterMonths; }
                                    else { selectStat.Parameters.Add("@moisFin", SqlDbType.Int).Value = DBNull.Value; }
                                    //Remplissage dataset
                                    adapterStat.Fill(dsSynthese);
                                    foreach (DataRow dR in dsSynthese.Tables[0].Rows)
                                    {
                                        //Valeurs
                                        ws.Cells[l, 0].Value = dR["Region"];
                                        ws.Cells[l, 1].Value = dR["PoidsTotAnneeRef"];
                                        ws.Cells[l, 2].Value = dR["PoidsAnneeRef"];
                                        ws.Cells[l, 3].Value = dR["PoidsTotJanvier"];
                                        ws.Cells[l, 4].Value = dR["PoidsJanvier"];
                                        ws.Cells[l, 5].Value = dR["PoidsTotFevrier"];
                                        ws.Cells[l, 6].Value = dR["PoidsFevrier"];
                                        ws.Cells[l, 7].Value = dR["PoidsTotMars"];
                                        ws.Cells[l, 8].Value = dR["PoidsMars"];
                                        ws.Cells[l, 9].Value = dR["PoidsTotAvril"];
                                        ws.Cells[l, 10].Value = dR["PoidsAvril"];
                                        ws.Cells[l, 11].Value = dR["PoidsTotMai"];
                                        ws.Cells[l, 12].Value = dR["PoidsMai"];
                                        ws.Cells[l, 13].Value = dR["PoidsTotJuin"];
                                        ws.Cells[l, 14].Value = dR["PoidsJuin"];
                                        ws.Cells[l, 15].Value = dR["PoidsTotJuillet"];
                                        ws.Cells[l, 16].Value = dR["PoidsJuillet"];
                                        ws.Cells[l, 17].Value = dR["PoidsTotAout"];
                                        ws.Cells[l, 18].Value = dR["PoidsAout"];
                                        ws.Cells[l, 19].Value = dR["PoidsTotSeptembre"];
                                        ws.Cells[l, 20].Value = dR["PoidsSeptembre"];
                                        ws.Cells[l, 21].Value = dR["PoidsTotOctobre"];
                                        ws.Cells[l, 22].Value = dR["PoidsOctobre"];
                                        ws.Cells[l, 23].Value = dR["PoidsTotNovembre"];
                                        ws.Cells[l, 24].Value = dR["PoidsNovembre"];
                                        ws.Cells[l, 25].Value = dR["PoidsTotDecembre"];
                                        ws.Cells[l, 26].Value = dR["PoidsDecembre"];
                                        ws.Cells[l, 27].Value = dR["PoidsTotAnnee"];
                                        ws.Cells[l, 28].Value = dR["PoidsAnnee"];
                                        //Format
                                        for (int k = 0; k <= 28; k++)
                                        {
                                            CellStyle style = ws.Cells[l, k].Style;
                                            style.Font.Weight = ExcelFont.BoldWeight;
                                            if (k > 0)
                                            {
                                                if (k % 2 == 0)
                                                {
                                                    style.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                                                    style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                    style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                    style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                    style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                                }
                                                else
                                                {
                                                    style.NumberFormat = NumberFormatBuilder.Percentage(0);
                                                    style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                    style.Borders.SetBorders(MultipleBorders.Left, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                    style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                    style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                                                }
                                            }
                                            else
                                            {
                                                style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                            }
                                        }
                                        //Suivant
                                        l++;
                                    }
                                }
                                ws.Cells[l, 0].Value = currentContext.CulturedRessources.GetTextRessource(778) + " - " + dT.Rows[r - 1][i];
                                //Valeurs
                                string filtre = "1=1 ";
                                for (int c = 0; c <= i; c++)
                                {
                                    filtre += " and " + dV.Rows[c]["Libelle"].ToString()
                                        + (dT.Rows[r - 1][c].ToString() == "" ? " is null" : " = '" + dT.Rows[r - 1][c].ToString().Replace("'", "''") + "'");
                                }
                                //Initialisation du tableau de total
                                DataRow tot = dT.NewRow();
                                for (int t = 4; t < 32; t++)
                                {
                                    tot[t] = 0;
                                }
                                //Calcul du total
                                foreach (DataRow dR in dT.Select(filtre))
                                {
                                    for (int t = 4; t < 32; t++)
                                    {
                                        tot[t] = ((tot[t] == System.DBNull.Value ? 0 : (decimal)tot[t]) + (dR[t] == System.DBNull.Value ? 0 : (decimal)dR[t]));
                                    }
                                }
                                //Inscription
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 1].Value = tot[4]; }
                                ws.Cells[l, 2].Value = tot[5];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 3].Value = tot[6]; }
                                ws.Cells[l, 4].Value = tot[7];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 5].Value = tot[8]; }
                                ws.Cells[l, 6].Value = tot[9];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 7].Value = tot[10]; }
                                ws.Cells[l, 8].Value = tot[11];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 9].Value = tot[12]; }
                                ws.Cells[l, 10].Value = tot[13];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 11].Value = tot[14]; }
                                ws.Cells[l, 12].Value = tot[15];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 13].Value = tot[16]; }
                                ws.Cells[l, 14].Value = tot[17];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 15].Value = tot[18]; }
                                ws.Cells[l, 16].Value = tot[19];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 17].Value = tot[20]; }
                                ws.Cells[l, 18].Value = tot[21];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 19].Value = tot[22]; }
                                ws.Cells[l, 20].Value = tot[23];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 21].Value = tot[24]; }
                                ws.Cells[l, 22].Value = tot[25];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 23].Value = tot[26]; }
                                ws.Cells[l, 24].Value = tot[27];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 25].Value = tot[28]; }
                                ws.Cells[l, 26].Value = tot[29];
                                if (i == dV.Rows.Count - 1) { ws.Cells[l, 27].Value = tot[30]; }
                                ws.Cells[l, 28].Value = tot[31];
                                //Format
                                for (int k = 0; k <= 28; k++)
                                {
                                    CellStyle style = ws.Cells[l, k].Style;
                                    style.Font.Weight = ExcelFont.BoldWeight;
                                    if (k > 0)
                                    {
                                        if (k % 2 == 0)
                                        {
                                            style.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                                            style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                            style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                            style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                            style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                        }
                                        else
                                        {
                                            style.NumberFormat = NumberFormatBuilder.Percentage(0);
                                            style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                            style.Borders.SetBorders(MultipleBorders.Left, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                            style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                            style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                                        }
                                    }
                                    else
                                    {
                                        style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                        style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    }
                                }
                                //colorisation sauf premier niveau de rg
                                if (dT.Rows[r - 1][currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name] != DBNull.Value && i != 0)
                                {
                                    for (int k = 0; k <= 28; k++)
                                    {
                                        if (k % 2 == 0)
                                        {
                                            CellStyle style = ws.Cells[l, k].Style;
                                            style.Font.Color = System.Drawing.Color.FromArgb(int.Parse(dT.Rows[r - 1][currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name].ToString().Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier));
                                        }
                                    }
                                }
                                //Ligne suivante
                                l++;
                            }
                        }
                        //Pour chaque niveau de regroupement qui change, on affiche l'en-tête de groupe
                        for (i = 0; i < dV.Rows.Count; i++)
                        {
                            //Test du changement de niveau de regroupement
                            changementRg = false;
                            for (int w = i; w >= 0; w--)
                            {
                                if (rgs[w] != dRow[w].ToString()) { changementRg = true; }
                            }
                            if (changementRg || premiereLigne)
                            {
                                ws.Cells[l, 0].Value = dRow[i];
                                CellStyle style = ws.Cells[l, 0].Style;
                                style.Font.Weight = ExcelFont.BoldWeight;
                                style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                //colorisation pour le niveau 2 seulement
                                if (dRow[currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name] != DBNull.Value && i != 0)
                                {
                                    style.Font.Color = System.Drawing.Color.FromArgb(int.Parse(dRow[currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name].ToString().Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier));
                                }
                                //Saut de page
                                if (i == 0 && !premiereLigne)
                                {
                                    ws.HorizontalPageBreaks.Add(l);
                                }
                                //Ligne suivante
                                l++;
                            }
                        }
                    }
                    //Enregistrement des nouveaux en-têtes de groupe courants
                    for (i = 0; i < dV.Rows.Count; i++)
                    {
                        rgs[i] = dRow[i].ToString();
                    }
                    //Valeurs
                    ws.Cells[l, 0].Value = dRow[3];
                    ws.Cells[l, 1].Value = dRow[4];
                    ws.Cells[l, 2].Value = dRow[5];
                    ws.Cells[l, 3].Value = dRow[6];
                    ws.Cells[l, 4].Value = dRow[7];
                    ws.Cells[l, 5].Value = dRow[8];
                    ws.Cells[l, 6].Value = dRow[9];
                    ws.Cells[l, 7].Value = dRow[10];
                    ws.Cells[l, 8].Value = dRow[11];
                    ws.Cells[l, 9].Value = dRow[12];
                    ws.Cells[l, 10].Value = dRow[13];
                    ws.Cells[l, 11].Value = dRow[14];
                    ws.Cells[l, 12].Value = dRow[15];
                    ws.Cells[l, 13].Value = dRow[16];
                    ws.Cells[l, 14].Value = dRow[17];
                    ws.Cells[l, 15].Value = dRow[18];
                    ws.Cells[l, 16].Value = dRow[19];
                    ws.Cells[l, 17].Value = dRow[20];
                    ws.Cells[l, 18].Value = dRow[21];
                    ws.Cells[l, 19].Value = dRow[22];
                    ws.Cells[l, 20].Value = dRow[23];
                    ws.Cells[l, 21].Value = dRow[24];
                    ws.Cells[l, 22].Value = dRow[25];
                    ws.Cells[l, 23].Value = dRow[26];
                    ws.Cells[l, 24].Value = dRow[27];
                    ws.Cells[l, 25].Value = dRow[28];
                    ws.Cells[l, 26].Value = dRow[29];
                    ws.Cells[l, 27].Value = dRow[30];
                    ws.Cells[l, 28].Value = dRow[31];
                    //Format
                    for (int k = 0; k <= 28; k++)
                    {
                        CellStyle style = ws.Cells[l, k].Style;
                        if (k > 0)
                        {
                            if (k % 2 == 0)
                            {
                                style.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                                style.Font.Color = System.Drawing.Color.FromArgb(int.Parse(dRow[currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name].ToString().Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier));
                                style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                                style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                                style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                                style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            }
                            else
                            {
                                style.NumberFormat = NumberFormatBuilder.Percentage(0);
                                style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                                style.Borders.SetBorders(MultipleBorders.Left, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                                style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                                style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            }
                        }
                        else
                        {
                            style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
                            style.Font.Color = System.Drawing.Color.FromArgb(int.Parse(dRow[currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name].ToString().Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier));
                            style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                        }

                    }
                    //Suivant
                    l++;
                    premiereLigne = false;
                }
                //Dernier sous-total
                if (dV.Rows.Count - 1 > 0)
                {
                    //SAUF PREMIERE LIGNE
                    for (i = dV.Rows.Count - 1; i >= 0; i--)
                    {
                        if (!premiereLigne)
                        {
                            //Ajout tableau de synthèse si niveau de regroupement 0
                            if (i == 0)
                            {
                                DataSet dsSynthese = new();
                                SqlCommand selectStat = new("DestinationProduitClientParTypeDeGroupe", sqlConn);
                                selectStat.CommandTimeout = 30;
                                selectStat.CommandType = CommandType.StoredProcedure;
                                SqlDataAdapter adapterStat = new();
                                adapterStat.SelectCommand = selectStat;
                                //Dates 
                                if (eSF.FilterYears != "") { selectStat.Parameters.Add("@anneeRef", SqlDbType.Int).Value = System.Convert.ToInt32(eSF.FilterYears) - 1; }
                                else { selectStat.Parameters.Add("@anneeRef", SqlDbType.Int).Value = DBNull.Value; }
                                if (eSF.FilterYears != "") { selectStat.Parameters.Add("@annee", SqlDbType.Int).Value = eSF.FilterYears; }
                                else { selectStat.Parameters.Add("@annee", SqlDbType.Int).Value = DBNull.Value; }
                                if (dT.Rows[dT.Rows.Count - 1][0] != DBNull.Value) { selectStat.Parameters.Add("@produitGroupeReportingType", SqlDbType.NVarChar).Value = dT.Rows[dT.Rows.Count - 1][0].ToString(); }
                                else { selectStat.Parameters.Add("@produitGroupeReportingType", SqlDbType.NVarChar).Value = DBNull.Value; }
                                if (eSF.FilterMonths != "") { selectStat.Parameters.Add("@moisFin", SqlDbType.Int).Value = eSF.FilterMonths; }
                                else { selectStat.Parameters.Add("@moisFin", SqlDbType.Int).Value = DBNull.Value; }
                                //Remplissage dataset
                                adapterStat.Fill(dsSynthese);
                                foreach (DataRow dR in dsSynthese.Tables[0].Rows)
                                {
                                    //Valeurs
                                    ws.Cells[l, 0].Value = dR["Region"];
                                    ws.Cells[l, 1].Value = dR["PoidsTotAnneeRef"];
                                    ws.Cells[l, 2].Value = dR["PoidsAnneeRef"];
                                    ws.Cells[l, 3].Value = dR["PoidsTotJanvier"];
                                    ws.Cells[l, 4].Value = dR["PoidsJanvier"];
                                    ws.Cells[l, 5].Value = dR["PoidsTotFevrier"];
                                    ws.Cells[l, 6].Value = dR["PoidsFevrier"];
                                    ws.Cells[l, 7].Value = dR["PoidsTotMars"];
                                    ws.Cells[l, 8].Value = dR["PoidsMars"];
                                    ws.Cells[l, 9].Value = dR["PoidsTotAvril"];
                                    ws.Cells[l, 10].Value = dR["PoidsAvril"];
                                    ws.Cells[l, 11].Value = dR["PoidsTotMai"];
                                    ws.Cells[l, 12].Value = dR["PoidsMai"];
                                    ws.Cells[l, 13].Value = dR["PoidsTotJuin"];
                                    ws.Cells[l, 14].Value = dR["PoidsJuin"];
                                    ws.Cells[l, 15].Value = dR["PoidsTotJuillet"];
                                    ws.Cells[l, 16].Value = dR["PoidsJuillet"];
                                    ws.Cells[l, 17].Value = dR["PoidsTotAout"];
                                    ws.Cells[l, 18].Value = dR["PoidsAout"];
                                    ws.Cells[l, 19].Value = dR["PoidsTotSeptembre"];
                                    ws.Cells[l, 20].Value = dR["PoidsSeptembre"];
                                    ws.Cells[l, 21].Value = dR["PoidsTotOctobre"];
                                    ws.Cells[l, 22].Value = dR["PoidsOctobre"];
                                    ws.Cells[l, 23].Value = dR["PoidsTotNovembre"];
                                    ws.Cells[l, 24].Value = dR["PoidsNovembre"];
                                    ws.Cells[l, 25].Value = dR["PoidsTotDecembre"];
                                    ws.Cells[l, 26].Value = dR["PoidsDecembre"];
                                    ws.Cells[l, 27].Value = dR["PoidsTotAnnee"];
                                    ws.Cells[l, 28].Value = dR["PoidsAnnee"];
                                    //Format
                                    for (int k = 0; k <= 28; k++)
                                    {
                                        CellStyle style = ws.Cells[l, k].Style;
                                        style.Font.Weight = ExcelFont.BoldWeight;
                                        if (k > 0)
                                        {
                                            if (k % 2 == 0)
                                            {
                                                style.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                                                style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                            }
                                            else
                                            {
                                                style.NumberFormat = NumberFormatBuilder.Percentage(0);
                                                style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                style.Borders.SetBorders(MultipleBorders.Left, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                                style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                                            }
                                        }
                                        else
                                        {
                                            style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                            style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                        }
                                    }
                                    //Suivant
                                    l++;
                                }
                            }
                            ws.Cells[l, 0].Value = currentContext.CulturedRessources.GetTextRessource(778) + " - " + dT.Rows[dT.Rows.Count - 1][i];
                            ws.Cells[l, 0].Style = styleSousTotal;
                            //Valeurs
                            string filtre = "1=1 ";
                            for (int c = 0; c <= i; c++)
                            {
                                filtre += " and " + dV.Rows[c]["Libelle"].ToString()
                                    + (dT.Rows[dT.Rows.Count - 1][c].ToString() == "" ? " is null" : " = '" + dT.Rows[dT.Rows.Count - 1][c].ToString().Replace("'", "''") + "'");
                            }
                            //Initialisation du tableau de total
                            DataRow tot = dT.NewRow();
                            for (int t = 4; t < 32; t++)
                            {
                                tot[t] = 0;
                            }
                            //Calcul du total
                            foreach (DataRow dR in dT.Select(filtre))
                            {
                                for (int t = 4; t < 32; t++)
                                {
                                    tot[t] = ((tot[t] == System.DBNull.Value ? 0 : (decimal)tot[t]) + (dR[t] == System.DBNull.Value ? 0 : (decimal)dR[t]));
                                }
                            }
                            //Inscription
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 1].Value = tot[4]; }
                            ws.Cells[l, 2].Value = tot[5];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 3].Value = tot[6]; }
                            ws.Cells[l, 4].Value = tot[7];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 5].Value = tot[8]; }
                            ws.Cells[l, 6].Value = tot[9];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 7].Value = tot[10]; }
                            ws.Cells[l, 8].Value = tot[11];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 9].Value = tot[12]; }
                            ws.Cells[l, 10].Value = tot[13];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 11].Value = tot[14]; }
                            ws.Cells[l, 12].Value = tot[15];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 13].Value = tot[16]; }
                            ws.Cells[l, 14].Value = tot[17];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 15].Value = tot[18]; }
                            ws.Cells[l, 16].Value = tot[19];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 17].Value = tot[20]; }
                            ws.Cells[l, 18].Value = tot[21];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 19].Value = tot[22]; }
                            ws.Cells[l, 20].Value = tot[23];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 21].Value = tot[24]; }
                            ws.Cells[l, 22].Value = tot[25];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 23].Value = tot[26]; }
                            ws.Cells[l, 24].Value = tot[27];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 25].Value = tot[28]; }
                            ws.Cells[l, 26].Value = tot[29];
                            if (i == dV.Rows.Count - 1) { ws.Cells[l, 27].Value = tot[30]; }
                            ws.Cells[l, 28].Value = tot[31];
                            //Format
                            for (int k = 0; k <= 28; k++)
                            {
                                CellStyle style = ws.Cells[l, k].Style;
                                style.Font.Weight = ExcelFont.BoldWeight;
                                if (k > 0)
                                {
                                    if (k % 2 == 0)
                                    {
                                        style.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                                        style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                        style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                        style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                        style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                    }
                                    else
                                    {
                                        style.NumberFormat = NumberFormatBuilder.Percentage(0);
                                        style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                        style.Borders.SetBorders(MultipleBorders.Left, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                        style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                        style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                                    }
                                }
                                else
                                {
                                    style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                    style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                }
                            }
                            //colorisation sauf premier niveau de rg
                            if (dT.Rows[dT.Rows.Count - 1][currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name] != DBNull.Value && i != 0)
                            {
                                for (int k = 0; k <= 28; k++)
                                {
                                    if (k % 2 == 0)
                                    {
                                        CellStyle style = ws.Cells[l, k].Style;
                                        style.Font.Color = System.Drawing.Color.FromArgb(int.Parse(dT.Rows[dT.Rows.Count - 1][currentContext.EnvDataColumns[Enumerations.DataColumnName.Couleur.ToString()].Name].ToString().Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier));
                                    }
                                }
                            }
                            //Ligne suivante
                            l++;
                        }
                    }
                }
                //Total général
                if (dV.Rows.Count > 0)
                {
                    DataSet dsSynthese = new();
                    SqlCommand selectStat = new("DestinationProduitClient", sqlConn);
                    selectStat.CommandTimeout = 30;
                    selectStat.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapterStat = new();
                    adapterStat.SelectCommand = selectStat;
                    //Dates 
                    if (eSF.FilterYears != "") { selectStat.Parameters.Add("@anneeRef", SqlDbType.Int).Value = System.Convert.ToInt32(eSF.FilterYears) - 1; }
                    else { selectStat.Parameters.Add("@anneeRef", SqlDbType.Int).Value = DBNull.Value; }
                    if (eSF.FilterYears != "") { selectStat.Parameters.Add("@annee", SqlDbType.Int).Value = eSF.FilterYears; }
                    else { selectStat.Parameters.Add("@annee", SqlDbType.Int).Value = DBNull.Value; }
                    if (eSF.FilterMonths != "") { selectStat.Parameters.Add("@moisFin", SqlDbType.Int).Value = eSF.FilterMonths; }
                    else { selectStat.Parameters.Add("@moisFin", SqlDbType.Int).Value = DBNull.Value; }
                    //Remplissage dataset
                    adapterStat.Fill(dsSynthese);
                    foreach (DataRow dR in dsSynthese.Tables[0].Rows)
                    {
                        //Valeurs
                        ws.Cells[l, 0].Value = dR["Region"];
                        ws.Cells[l, 1].Value = dR["PoidsTotAnneeRef"];
                        ws.Cells[l, 2].Value = dR["PoidsAnneeRef"];
                        ws.Cells[l, 3].Value = dR["PoidsTotJanvier"];
                        ws.Cells[l, 4].Value = dR["PoidsJanvier"];
                        ws.Cells[l, 5].Value = dR["PoidsTotFevrier"];
                        ws.Cells[l, 6].Value = dR["PoidsFevrier"];
                        ws.Cells[l, 7].Value = dR["PoidsTotMars"];
                        ws.Cells[l, 8].Value = dR["PoidsMars"];
                        ws.Cells[l, 9].Value = dR["PoidsTotAvril"];
                        ws.Cells[l, 10].Value = dR["PoidsAvril"];
                        ws.Cells[l, 11].Value = dR["PoidsTotMai"];
                        ws.Cells[l, 12].Value = dR["PoidsMai"];
                        ws.Cells[l, 13].Value = dR["PoidsTotJuin"];
                        ws.Cells[l, 14].Value = dR["PoidsJuin"];
                        ws.Cells[l, 15].Value = dR["PoidsTotJuillet"];
                        ws.Cells[l, 16].Value = dR["PoidsJuillet"];
                        ws.Cells[l, 17].Value = dR["PoidsTotAout"];
                        ws.Cells[l, 18].Value = dR["PoidsAout"];
                        ws.Cells[l, 19].Value = dR["PoidsTotSeptembre"];
                        ws.Cells[l, 20].Value = dR["PoidsSeptembre"];
                        ws.Cells[l, 21].Value = dR["PoidsTotOctobre"];
                        ws.Cells[l, 22].Value = dR["PoidsOctobre"];
                        ws.Cells[l, 23].Value = dR["PoidsTotNovembre"];
                        ws.Cells[l, 24].Value = dR["PoidsNovembre"];
                        ws.Cells[l, 25].Value = dR["PoidsTotDecembre"];
                        ws.Cells[l, 26].Value = dR["PoidsDecembre"];
                        ws.Cells[l, 27].Value = dR["PoidsTotAnnee"];
                        ws.Cells[l, 28].Value = dR["PoidsAnnee"];
                        //Format
                        for (int k = 0; k <= 28; k++)
                        {
                            CellStyle style = ws.Cells[l, k].Style;
                            style.Font.Weight = ExcelFont.BoldWeight;
                            if (k > 0)
                            {
                                if (k % 2 == 0)
                                {
                                    style.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                                    style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                    style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                    style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                    style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                }
                                else
                                {
                                    style.NumberFormat = NumberFormatBuilder.Percentage(0);
                                    style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                    style.Borders.SetBorders(MultipleBorders.Left, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                    style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                    style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                                }
                            }
                            else
                            {
                                style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            }
                        }
                        //Suivant
                        l++;
                    }
                    ws.Cells[l, 0].Value = currentContext.CulturedRessources.GetTextRessource(779);
                    ws.Cells[l, 0].Style = styleSousTotal;
                    //Valeurs
                    //Initialisation du tableau de total
                    DataRow tot = dT.NewRow();
                    for (int t = 4; t < 32; t++)
                    {
                        tot[t] = 0;
                    }
                    //Calcul du total
                    foreach (DataRow dR in dT.Rows)
                    {
                        for (int t = 4; t < 32; t++)
                        {
                            tot[t] = ((tot[t] == System.DBNull.Value ? 0 : (decimal)tot[t]) + (dR[t] == System.DBNull.Value ? 0 : (decimal)dR[t]));
                        }
                    }
                    //Inscription
                    ws.Cells[l, 2].Value = tot[5];
                    ws.Cells[l, 4].Value = tot[7];
                    ws.Cells[l, 6].Value = tot[9];
                    ws.Cells[l, 8].Value = tot[11];
                    ws.Cells[l, 10].Value = tot[13];
                    ws.Cells[l, 12].Value = tot[15];
                    ws.Cells[l, 14].Value = tot[17];
                    ws.Cells[l, 16].Value = tot[19];
                    ws.Cells[l, 18].Value = tot[21];
                    ws.Cells[l, 20].Value = tot[23];
                    ws.Cells[l, 22].Value = tot[25];
                    ws.Cells[l, 24].Value = tot[27];
                    ws.Cells[l, 26].Value = tot[29];
                    ws.Cells[l, 28].Value = tot[31];
                    //Format
                    for (int k = 0; k <= 28; k++)
                    {
                        CellStyle style = ws.Cells[l, k].Style;
                        style.Font.Weight = ExcelFont.BoldWeight;
                        if (k > 0)
                        {
                            if (k % 2 == 0)
                            {
                                style.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                                style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            }
                            else
                            {
                                style.NumberFormat = NumberFormatBuilder.Percentage(0);
                                style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                style.Borders.SetBorders(MultipleBorders.Left, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                                style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            }
                        }
                        else
                        {
                            style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);
                            style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        }
                    }
                }
                lastDataRowIndex = l;
            }
            else
            {
                foreach (DataRow dR in dT.Rows)
                {
                    for (j = 0; j < dT.Columns.Count; j++)
                    {
                        EnvDataColumn envDC = null;
                        if (name != Enumerations.MenuName.LogistiqueMenuEtatReceptionEmballagePlastique.ToString()
                            && name != Enumerations.MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur.ToString()
                            && name != Enumerations.MenuName.LogistiqueMenuEtatDesFluxDev.ToString()
                            && name != Enumerations.MenuName.LogistiqueMenuExtractionOscar.ToString())
                        {
                            if (currentContext.EnvDataColumns.ContainsKey(dT.Columns[j].ColumnName))
                            {
                                envDC = currentContext.EnvDataColumns[dT.Columns[j].ColumnName];
                            }
                        }
                        //Formattage and data inscription
                        Utils.WriteAndFormatCellData(dR, i, j, envDC, ref ws, name, currentContext.CulturedRessources);
                        if (name == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString())
                        {
                            if (j >= 2)
                            {
                                CellStyle st = ws.Cells[i, j].Style;
                                st.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                            }
                        }
                        if (name == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString())
                        {
                            if (j >= 10)
                            {
                                CellStyle st = ws.Cells[i, j].Style;
                                st.NumberFormat = NumberFormatBuilder.Number(0, 1, true);
                            }
                        }
                        //Application du style si total
                        if (ws.Cells[i, 0].Value != null
                            && (
                                ws.Cells[i, 0].Value.ToString() == currentContext.CulturedRessources.GetTextRessource(778)
                                || ws.Cells[i, 0].Value.ToString() == currentContext.CulturedRessources.GetTextRessource(779)
                                || ws.Cells[i, 0].Value.ToString() == currentContext.CulturedRessources.GetTextRessource(1021)
                                || ws.Cells[i, 0].Value.ToString() == currentContext.CulturedRessources.GetTextRessource(1022)
                                || ws.Cells[i, 0].Value.ToString() == currentContext.CulturedRessources.GetTextRessource(1271)
                                || ws.Cells[i, 0].Value.ToString() == currentContext.CulturedRessources.GetTextRessource(1272)
                                )
                            )
                        {
                            ws.Cells[i, j].Style.Borders = styleEnTeteColonne.Borders;
                            ws.Cells[i, j].Style.Font.Weight = styleEnTeteColonne.Font.Weight;
                            ws.Cells[i, j].Style.Font.Name = styleEnTeteColonne.Font.Name;
                            ws.Cells[i, j].Style.HorizontalAlignment = styleEnTeteColonne.HorizontalAlignment;
                            ws.Cells[i, j].Style.WrapText = styleEnTeteColonne.WrapText;
                        }
                    }
                    //Formattage spécifique
                    if (name == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString() && dsStat.Tables[0].Columns.Count > 2)
                    {
                        if (ws.Cells[i, 0].StringValue == currentContext.CulturedRessources.GetTextRessource(1021))
                        {
                            string p = dsStat.Tables[0].Columns[2].Caption.Substring(0, dsStat.Tables[0].Columns[2].Caption.LastIndexOf("#/#"));
                            int d = 0;
                            //Span
                            for (int v = 1; v < dsStat.Tables[0].Columns.Count - 2; v++)
                            {
                                //Si changement de produit
                                if (p != dsStat.Tables[0].Columns[v + 2].Caption.Substring(0, dsStat.Tables[0].Columns[v + 2].Caption.LastIndexOf("#/#")))
                                {
                                    //Span si au moins 2 cellules
                                    if (v - d > 1)
                                    {
                                        ws.Cells.GetSubrangeRelative(i, d + 2, v - d, 1).Merged = true;
                                    }
                                    //Réinitialisations
                                    p = dsStat.Tables[0].Columns[v + 2].Caption.Substring(0, dsStat.Tables[0].Columns[v + 2].Caption.LastIndexOf("#/#"));
                                    d = v;
                                }
                            }
                            if (dsStat.Tables[0].Columns.Count - 3 > d)
                            {
                                for (int c = d; c <= dsStat.Tables[0].Columns.Count - 3 - 1; c++)
                                {
                                    ws.Cells.GetSubrangeRelative(i, d + 2, dsStat.Tables[0].Columns.Count - 2 - d, 1).Merged = true;
                                }
                            }
                        }
                    }
                    if (name == Enumerations.MenuName.LogistiqueMenuEtatTarifTransporteurClient.ToString() && dsStat.Tables[0].Columns.Count > 4)
                    {
                        decimal min = 1000000000;
                        int n = 0, k = 0;
                        //Récupération de la valeur maximum
                        for (k = 4; k < dsStat.Tables[0].Columns.Count; k++)
                        {
                            if ((dR[k] == DBNull.Value ? 0 : (decimal)dR[k]) < min && (dR[k] == DBNull.Value ? 0 : (decimal)dR[k]) > 0)
                            {
                                min = (decimal)dR[k];
                                n = k;
                            }
                        }
                        //Mise en gras du min
                        if (n > 0 && min > 0)
                        {
                            ws.Cells[i, n].Style.Font.Weight = ExcelFont.BoldWeight;
                        }
                    }
                    if (name == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString())
                    {
                        //Colorisation si prix de reprise non unique
                        if (dR[currentContext.EnvDataColumns[Enumerations.DataColumnName.PUHTUnique.ToString()].Name] != DBNull.Value)
                        {
                            if ((bool)dR[currentContext.EnvDataColumns[Enumerations.DataColumnName.PUHTUnique.ToString()].Name] == false)
                            {
                                CellStyle style = ws.Cells[i, j-1].Style;
                                style.FillPattern.SetSolid(System.Drawing.Color.FromArgb(int.Parse("C185F8", System.Globalization.NumberStyles.AllowHexSpecifier)));
                            }
                        }
                    }
                    i++;
                }
                lastDataRowIndex = i - 1;
            }
            //Mise en forme générale
            if (name == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString()
                || name == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
            {
            }
            else
            {
                int columnCount = ws.CalculateMaxUsedColumns();
                for (int c = 0; c < columnCount; c++)
                    ws.Columns[c].AutoFit();
            }
            //Titre et critères
            if (name != Enumerations.MenuName.LogistiqueMenuEtatReceptionEmballagePlastique.ToString()
                && name != Enumerations.MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur.ToString()
                && name != Enumerations.MenuName.LogistiqueMenuExtractionOscar.ToString()
                )
            {
                //Titre du tableau
                if (name == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString())
                {
                    ws.Cells[0, 0].Value = currentContext.CulturedRessources.GetTextRessource(1035);
                }
                else if (name == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
                {
                    ws.Cells[0, 0].Value = currentContext.CulturedRessources.GetTextRessource(1054);
                }
                else if (name == Enumerations.StatType.DonneeCDT.ToString())
                {
                    ws.Cells[0, 0].Value = currentContext.CulturedRessources.GetTextRessource(1367);
                }
                else if (name == Enumerations.StatType.ExtractionRSE.ToString())
                {
                    ws.Cells[0, 0].Value = currentContext.CulturedRessources.GetTextRessource(1463);
                }
                else if (name == Enumerations.StatType.ExtractionPrixReprise.ToString())
                {
                    ws.Cells[0, 0].Value = currentContext.CulturedRessources.GetTextRessource(1344);
                }
                else if (name == Enumerations.StatType.DestinationTonnage.ToString())
                {
                    ws.Cells[0, 0].Value = currentContext.CulturedRessources.GetTextRessource(1345);
                }
                else if (name == Enumerations.StatType.DeboucheBalle.ToString())
                {
                    ws.Cells[0, 0].Value = currentContext.CulturedRessources.GetTextRessource(1346);
                }
                else if (name == Enumerations.StatType.EvolutionTonnage.ToString())
                {
                    ws.Cells[0, 0].Value = currentContext.CulturedRessources.GetTextRessource(1347);
                }
                else
                {
                    ws.Cells[0, 0].Value = currentContext.EnvMenus[name].CulturedCaption;
                }
                ws.Cells[0, 0].Style = styleTitre;
                ws.Rows.InsertEmpty(1);
                lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                //Filtres
                if (!string.IsNullOrEmpty(eSF.FilterText)
                    && (
                         name == Enumerations.MenuName.AnnuaireMenuExtractionAction.ToString()
                         || name == Enumerations.MenuName.AnnuaireMenuSuiviEnvois.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1084) + " : " + eSF.FilterText;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterDRs)
                    && (name == Enumerations.MenuName.LogistiqueMenuLotDisponible.ToString()
                        || name == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatControleReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuChargementAnnule.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCLSousContrat.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuExtractionAction.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(764) + " : " + eSF.FilterDRsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterClients)
                    && (name == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatControleReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(314) + " : " + eSF.FilterClientsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterTransporteurs)
                    && (name == Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuChargementAnnule.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuNbAffretementTransporteur.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(308) + " : " + eSF.FilterTransporteursCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterEntites)
                    && (name == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString()
                        || name == Enumerations.MenuName.AdministrationMenuExtractionLogin.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1124) + " : " + eSF.FilterEntitesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterAdresseDestinations)
                    && (name == Enumerations.MenuName.LogistiqueMenuEtatTarifTransporteurClient.ToString())
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(321) + " : " + eSF.FilterAdresseDestinationsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterNonConformiteEtapeTypes)
                    && (name == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString())
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(859) + " : " + eSF.FilterNonConformiteEtapeTypesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterNonConformiteNatures)
                    && (name == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString())
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(817) + " : " + eSF.FilterNonConformiteNaturesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterDptArrivees)
                    && (name == Enumerations.MenuName.LogistiqueMenuEtatTarifTransporteurClient.ToString())
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(434) + " : " + eSF.FilterDptArriveesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterVilleDeparts)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(249) + " : " + eSF.FilterVilleDepartsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterVilleArrivees)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(246) + " : " + eSF.FilterVilleArriveesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterPayss)
                    && (name == Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString())
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(312) + " : " + eSF.FilterPayssCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterProduitGroupeReportings)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduit.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(976) + " : " + eSF.FilterProduitGroupeReportingsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterProcesss)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuEtatTonnageParProcess.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(583) + " : " + eSF.FilterProcesssCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterProduits)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuLotDisponible.ToString()
                        || name == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatControleReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClientProduit.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduit.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduitDR.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatTonnageParProcess.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuListeProduit.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatDesEnlevements.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatDesFluxDev.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatDesFluxDevLeko.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(317) + " : " + eSF.FilterProduitsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterProduits)
                    && (
                        name == Enumerations.StatType.EvolutionTonnage.ToString()
                        || name == Enumerations.StatType.ExtractionPrixReprise.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(317) + " : " + eSF.FilterProduitNomCommunsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterCamionTypes)
                    && (
                         name == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(313) + " : " + eSF.FilterCamionTypesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterActionTypes)
                    && (
                         name == Enumerations.MenuName.AnnuaireMenuExtractionAction.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1193) + " : " + eSF.FilterActionTypesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterFonctions)
                    && (
                         name == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(10015) + " : " + eSF.FilterFonctionsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterServices)
                    && (
                         name == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(10011) + " : " + eSF.FilterServicesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterContactAdresseProcesss)
                    && (
                         name == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(420) + " : " + eSF.FilterContactAdresseProcesssCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterAdresseTypes)
                    && (
                         name == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(10043) + " : " + eSF.FilterAdresseTypesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterEntiteTypes)
                    && (
                         name == Enumerations.MenuName.AnnuaireMenuExtractionAction.ToString()
                         || name == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString()
                         || name == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(10066) + " : " + eSF.FilterEntiteTypesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterUtilisateurs)
                    && (name == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString()
                        || name == Enumerations.MenuName.AdministrationMenuExtractionLogin.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(911) + " : " + eSF.FilterUtilisateursCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (
                        name == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString()
                    )
                {
                    if (eSF.FilterDayWeekMonth == "Day") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1326); }
                    else if (eSF.FilterDayWeekMonth == "Week") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1327); }
                    else { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1328); }
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (
                        name == Enumerations.MenuName.AdministrationMenuExtractionLogin.ToString()
                    )
                {
                    if (eSF.FilterFirstLogin) { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1329); }
                    else { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1330); }
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (
                        name == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatControleReception.ToString()
                    )
                {
                    if (eSF.FilterContrat == "SousContrat") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(886); }
                    else if (eSF.FilterContrat == "HorsContrat") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(887); }
                    else { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(888); }
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (
                        name == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString()
                    )
                {
                    if (eSF.FilterContrat == "SousContrat") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1000); }
                    else if (eSF.FilterContrat == "HorsContrat") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1001); }
                    else { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1002); }
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterEcoOrganismes)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuEtatDesPoids.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(1419) + " : " + eSF.FilterEcoOrganismesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterCentreDeTris)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuLotDisponible.ToString()
                        || name == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatControleReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuChargementAnnule.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatReceptionFournisseurChargement.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteProduit.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit.ToString()
                        || name == Enumerations.StatType.DestinationTonnage.ToString()
                        || name == Enumerations.StatType.DeboucheBalle.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(315) + " : " + eSF.FilterCentreDeTrisCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterCollectivites)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString()
                        || name == Enumerations.StatType.DonneeCDT.ToString()
                        || name == Enumerations.StatType.ExtractionRSE.ToString()
                        || name == Enumerations.StatType.DestinationTonnage.ToString()
                        || name == Enumerations.StatType.EvolutionTonnage.ToString()
                        || name == Enumerations.StatType.ExtractionPrixReprise.ToString()
                        || name == Enumerations.StatType.DeboucheBalle.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(316) + " : " + eSF.FilterCollectivitesCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterYears)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuPartMarcheTransporteur.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduitDR.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCLSousContrat.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString()
                        || name == Enumerations.StatType.DeboucheBalle.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(942) + " : " + eSF.FilterYearsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterMonths)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuPartMarcheTransporteur.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(571) + " : " + eSF.FilterMonthsCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (eSF.FilterActif
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuListeProduit.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(651) + " : " + currentContext.CulturedRessources.GetTextRessource(205);
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterQuarters)
                    && (
                        name == Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(696) + " : " + eSF.FilterQuartersCaption;
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (name == Enumerations.MenuName.LogistiqueMenuEtatKmMoyen.ToString()
                    || name == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString()
                    || name == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClientProduit.ToString()
                    || name == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduitDR.ToString()
                    || name == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduit.ToString()
                    || name == Enumerations.MenuName.LogistiqueMenuEtatDesPoids.ToString()
                    || name == Enumerations.MenuName.LogistiqueMenuEtatTonnageParProcess.ToString()
                    || name == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString()
                    || name == Enumerations.MenuName.LogistiqueMenuListeProduit.ToString()
                    || name == Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString()
                    || name == Enumerations.StatType.DonneeCDT.ToString()
                    || name == Enumerations.StatType.DestinationTonnage.ToString()
                    || name == Enumerations.StatType.EvolutionTonnage.ToString()
                    || name == Enumerations.StatType.ExtractionPrixReprise.ToString()
                    || name == Enumerations.StatType.DeboucheBalle.ToString()
                    || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit.ToString()
                    )
                {
                    if (eSF.FilterCollecte == "Collecte") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(603); }
                    else if (eSF.FilterCollecte == "HorsCollecte") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(599); }
                    else { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(949); }
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterBegin) &&
                        (
                        name == Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString()
                        || name == Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionControle.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString()
                        || name == Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString()
                        || name == Enumerations.MenuName.QualiteMenuEtatControleReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuChargementAnnule.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatReceptionFournisseurChargement.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatReceptionEmballagePlastique.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionOscar.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatDesFluxDev.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatDesFluxDevLeko.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuLotDisponible.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatKmMoyen.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatReceptionProduit.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatDesPoids.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatTonnageParProcess.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuSuiviFacturationHC.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteProduit.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuSuiviEnvois.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuExtractionAction.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString()
                        || name == Enumerations.StatType.DonneeCDT.ToString()
                        || name == Enumerations.StatType.ExtractionRSE.ToString()
                        || name == Enumerations.StatType.DestinationTonnage.ToString()
                        || name == Enumerations.StatType.EvolutionTonnage.ToString()
                        || name == Enumerations.StatType.ExtractionPrixReprise.ToString()
                        || name == Enumerations.MenuName.AnnuaireMenuNbAffretementTransporteur.ToString()
                        || name == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString()
                        || name == Enumerations.MenuName.AdministrationMenuExtractionLogin.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuEtatDesEnlevements.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString()
                        || name == Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteCDTProduit.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = "Du " + Convert.ToDateTime(eSF.FilterBegin).ToString("dd/MM/yyyy") + " au " + Convert.ToDateTime(eSF.FilterEnd).ToString("dd/MM/yyyy");
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (!string.IsNullOrEmpty(eSF.FilterBegin) &&
                        (
                            name == Enumerations.MenuName.LogistiqueMenuSuiviCommandeClientProduit.ToString()
                        )
                    )
                {
                    ws.Cells[1, 0].Value = Convert.ToDateTime(eSF.FilterBegin).ToString("MMMM yyyy");
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
                if (
                        name == Enumerations.MenuName.AnnuaireMenuSuiviEnvois.ToString()
                    )
                {
                    if (eSF.FilterEmailType == "IncitationQualite") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(700); }
                    else if (eSF.FilterEmailType == "NoteCreditCollectivite") { ws.Cells[1, 0].Value = currentContext.CulturedRessources.GetTextRessource(699); }
                    ws.Cells[1, 0].Style = styleCritere;
                    ws.Rows.InsertEmpty(1);
                    lastCriteriaRowIndex++; firstDataRowIndex++; lastDataRowIndex++;
                }
            }
            else
            {
                //Suppression des 2 lignes vides en haut si pas de titre ou critère
                ws.Rows.Remove(0);
                ws.Rows.Remove(0);
            }
            //Mise en forme finale
            int rowCount = ws.GetUsedCellRange(true).LastRowIndex;
            for (int r = 0; r < rowCount; r++)
            {
                ws.Rows[r].AutoFit();
            }
            //Special heihgts
            if (name == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
            {
                ws.Rows[5].Height = 5 * 128;
            }
            if (name == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString())
            {
                ws.Rows[5].Height = 7 * 128;
            }
            if (name == Enumerations.StatType.DonneeCDT.ToString()
                || name == Enumerations.StatType.ExtractionRSE.ToString()
                || name == Enumerations.StatType.ExtractionPrixReprise.ToString()
                || name == Enumerations.StatType.DestinationTonnage.ToString()
                || name == Enumerations.StatType.DeboucheBalle.ToString()
                || name == Enumerations.StatType.EvolutionTonnage.ToString()
                || (currentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite > 0 && name == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString())
                || (currentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite > 0 && name == Enumerations.MenuName.LogistiqueMenuEtatDesEnlevements.ToString())
                || (currentContext.ConnectedUtilisateur.CentreDeTri?.RefEntite > 0 && name == Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString())
                || name == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString()
                || name == Enumerations.MenuName.AdministrationMenuExtractionLogin.ToString()
                )
            {
                ws.Rows[0].Height =18 * 128;
            }
            if (name == Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString()
                || name == Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString())
            {
                ws.NamedRanges.SetPrintTitles(ws.Rows[0], 6);
                ws.PrintOptions.FitWorksheetWidthToPages = 1;
                ws.PrintOptions.FitWorksheetHeightToPages = 0;
                ws.PrintOptions.BottomMargin = 0.4;
                ws.PrintOptions.TopMargin = 0.4;
                ws.PrintOptions.LeftMargin = 0.4;
                ws.PrintOptions.RightMargin = 0.4;
                ws.PrintOptions.FitToPage = true;
                ws.PrintOptions.Portrait = false;
            }
            if (name == Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString())
            {
                //Mise en forme finale
                int colCount = ws.GetUsedCellRange(true).LastColumnIndex;
                for (int r = lastCriteriaRowIndex; r < rowCount; r++)
                {
                    for (int c = 0; c <= colCount; c++)
                    {
                        ws.Rows[r].Cells[c].Style.WrapText = true;
                    }
                }
                //Taille maximum des colonnes
                for (int col = 0; col <= colCount; col++)
                {
                    if (ws.Columns[col].Width > 50 * 256) { ws.Columns[col].Width = 50 * 256; }
                }
            }
            //Charts
            //if (name == Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString())
            //{
            //    // Create chart and select data for it.
            //    var chart = ws.Charts.Add<ColumnChart>(ChartGrouping.Clustered, ws.Cells[lastDataRowIndex + 3, 0].Name, ws.Cells[lastDataRowIndex + 30, 10].Name);
            //    chart.Series.Add(currentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDelaiCommande.ToString()].CulturedCaption, ws.Cells.GetSubrangeAbsolute(firstDataRowIndex, 4, lastDataRowIndex - 1, 4).ToString());
            //    chart.Series.Add(currentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurDelaiChargement.ToString()].CulturedCaption, ws.Cells.GetSubrangeAbsolute(firstDataRowIndex, 6, lastDataRowIndex - 1, 6).ToString());
            //    // Set chart title.
            //    chart.Title.Text = currentContext.EnvMenus[name].CulturedCaption;
            //    // Set axis titles.
            //    chart.Axes.Horizontal.Title.Text = currentContext.EnvDataColumns[Enumerations.DataColumnName.CommandeFournisseurNumeroCommande.ToString()].CulturedCaption;
            //    chart.Axes.Horizontal.ChangeAxisType(AxisType.Category);
            //    chart.CategoryLabelsReference = ws.Name + "!" + ws.Cells.GetSubrangeAbsolute(firstDataRowIndex, 1, lastDataRowIndex - 1, 1).ToString();
            //    chart.Axes.Vertical.Title.Text = currentContext.CulturedRessources.GetTextRessource(1429);
            //    chart.Axes.Vertical.NumberFormatLinkedToSource = false;
            //    chart.Axes.Vertical.NumberFormat = "0";
            //    //Set serie color
            //    for (int c = 0; c < 2; c++)
            //    {
            //        //Find color index
            //        int cI = c % Utils.hTMLColors.Length;
            //        chart.Series[c].Fill.SetSolid(DrawingColor.FromRgb(System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).R
            //            , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).G
            //            , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).B)
            //            );
            //    }
            //    //Labels
            //    chart.DataLabels.LabelContainsValue = true;
            //    chart.DataLabels.NumberFormat = "0";
            //    //Legend
            //    chart.Legend.IsVisible = true;
            //    chart.Legend.Position = ChartLegendPosition.Bottom;
            //}
            if (name == Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString()
                || name == Enumerations.StatType.EvolutionTonnage.ToString()
                || name == Enumerations.MenuName.AnnuaireMenuNbAffretementTransporteur.ToString())
            {
                // Create chart and select data for it.
                var chart = ws.Charts.Add<ColumnChart>(ChartGrouping.Clustered, ws.Cells[lastDataRowIndex + 3, 0].Name, ws.Cells[lastDataRowIndex + 30, 10].Name);
                chart.SelectData(ws.Cells.GetSubrangeAbsolute(firstDataRowIndex - 1, 0, lastDataRowIndex, ws.GetUsedCellRange(true).LastColumnIndex), true);
                // Set chart title.
                if (name == Enumerations.StatType.EvolutionTonnage.ToString())
                {
                    chart.Title.Text = currentContext.CulturedRessources.GetTextRessource(1347);
                }
                else
                {
                    chart.Title.Text = currentContext.EnvMenus[name].CulturedCaption;
                }
                // Set axis titles.
                chart.Axes.Horizontal.Title.Text = ws.Cells[firstDataRowIndex - 1, 0].StringValue;
                if (name == Enumerations.MenuName.AnnuaireMenuNbAffretementTransporteur.ToString())
                {
                    chart.Axes.Vertical.Title.Text = currentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsTonne.ToString()].CulturedCaption;
                }
                else if (name == Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString()
                        || name == Enumerations.StatType.EvolutionTonnage.ToString()
                    )
                {
                    chart.Axes.Vertical.Title.Text = currentContext.EnvDataColumns[Enumerations.DataColumnName.PoidsTonne.ToString()].CulturedCaption;
                    chart.Axes.Vertical.NumberFormatLinkedToSource = false;
                    chart.Axes.Vertical.NumberFormat = "0";
                }
                //Set serie color
                for (int c = 0; c < ws.GetUsedCellRange(true).LastColumnIndex; c++)
                {
                    //Find color index
                    int cI = c % Utils.hTMLColors.Length;
                    chart.Series[c].Fill.SetSolid(DrawingColor.FromRgb(System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).R
                        , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).G
                        , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).B)
                        );
                }
                //Labels
                chart.DataLabels.LabelContainsValue = true;
                if (name == Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString()
                        || name == Enumerations.StatType.EvolutionTonnage.ToString()
                    )
                {
                    chart.DataLabels.NumberFormat = "0.000 t";
                }
                //Legend
                if (chart.Series.Count > 0)
                {
                    if (ws.GetUsedCellRange(true).LastColumnIndex > 1 || chart.Series[0]?.DisplayName != chart.Axes.Vertical.Title.Text)
                    {
                        chart.Legend.IsVisible = true;
                        chart.Legend.Position = ChartLegendPosition.Bottom;
                    }
                }
                ////Hide data
                //chart.ShowDataInHiddenCells = true;
                //for (int r = firstDataRowIndex - 1; r <= lastDataRowIndex; r++)
                //{
                //    ws.Rows[r].OutlineLevel = 1;
                //    ws.Rows[r].Hidden = true;
                //}
                //ws.Rows[lastDataRowIndex + 1].Collapsed = false;
            }
            if (name == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString())
            {
                // Create chart and select data for it.
                var chart = ws.Charts.Add<LineChart>(ChartGrouping.Clustered, ws.Cells[lastDataRowIndex + 3, 0].Name, ws.Cells[lastDataRowIndex + 30, 10].Name);
                chart.SelectData(ws.Cells.GetSubrangeAbsolute(firstDataRowIndex - 1, 0, lastDataRowIndex, ws.GetUsedCellRange(true).LastColumnIndex), true);
                // Set chart title.
                chart.Title.Text = currentContext.EnvMenus[name].CulturedCaption;
                // Set axis titles.
                chart.Axes.Horizontal.Title.Text = ws.Cells[firstDataRowIndex - 1, 0].StringValue;
                if (name == Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString())
                {
                    chart.Axes.Vertical.Title.Text = currentContext.EnvDataColumns[Enumerations.DataColumnName.Nb.ToString()].CulturedCaption;
                }
                //Set serie color
                for (int c = 0; c < ws.GetUsedCellRange(true).LastColumnIndex; c++)
                {
                    //Find color index
                    int cI = c % Utils.hTMLColors.Length;
                    chart.Series[c].Outline.Fill.SetSolid(DrawingColor.FromRgb(System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).R
                        , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).G
                        , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).B)
                        );
                }
                //Labels
                chart.DataLabels.LabelContainsValue = true;
                //Legend
                if (chart.Series.Count > 0)
                {
                    if (ws.GetUsedCellRange(true).LastColumnIndex > 1 || chart.Series[0].DisplayName != chart.Axes.Vertical.Title.Text)
                    {
                        chart.Legend.IsVisible = true;
                        chart.Legend.Position = ChartLegendPosition.Bottom;
                    }
                }
            }
            if (name == Enumerations.StatType.DestinationTonnage.ToString())
            {
                // Create chart and select data for it.
                var chart = ws.Charts.Add<BarChart>(ChartGrouping.Clustered, ws.Cells[lastDataRowIndex + 3, 0].Name, ws.Cells[lastDataRowIndex + 30, 10].Name);
                chart.SelectData(ws.Cells.GetSubrangeAbsolute(firstDataRowIndex - 1, 0, lastDataRowIndex, ws.GetUsedCellRange(true).LastColumnIndex), true);
                // Set chart title.
                chart.Title.Text = currentContext.CulturedRessources.GetTextRessource(1345);
                // Set axis titles.
                chart.Axes.Horizontal.Title.Text = "%";
                chart.Axes.Horizontal.Title.Direction = ChartTitleDirection.Horizontal;
                //Set points color
                int count = chart.Series[0].Values.Cast<object>().Count();
                for (int c = 0; c < count; c++)
                {
                    //Find color index
                    int cI = c % Utils.hTMLColors.Length;
                    chart.Series[0].DataPoints[c].Fill.SetSolid(DrawingColor.FromRgb(System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).R
                        , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).G
                        , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).B)
                        );
                }
                //Labels
                chart.DataLabels.LabelContainsValue = true;
            }
            if (name == Enumerations.StatType.DeboucheBalle.ToString())
            {
                // Create chart and select data for it.
                var chart = ws.Charts.Add<PieChart>(ChartGrouping.Clustered, ws.Cells[lastDataRowIndex + 3, 0].Name, ws.Cells[lastDataRowIndex + 30, 10].Name);
                chart.SelectData(ws.Cells.GetSubrangeAbsolute(firstDataRowIndex - 1, 0, lastDataRowIndex, ws.GetUsedCellRange(true).LastColumnIndex), true);
                // Set chart title.
                chart.Title.Text = currentContext.CulturedRessources.GetTextRessource(1346);
                //Set points color
                for (int c = 0; c < chart.Series[0].DataPoints.Count(); c++)
                {
                    //Find color index
                    int cI = c % Utils.hTMLColors.Length;
                    chart.Series[0].DataPoints[c].Fill.SetSolid(DrawingColor.FromRgb(System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).R
                        , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).G
                        , System.Drawing.ColorTranslator.FromHtml(Utils.hTMLColors[cI]).B)
                        );
                }
                //Labels
                chart.DataLabels.LabelContainsValue = true;
                chart.Legend.IsVisible = true;
                chart.Legend.Position = ChartLegendPosition.Right;
            }
            //End
            excelFile.Save(ms, SaveOptions.XlsxDefault);
            return ms;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create FicheControle
        /// </summary>
        public static MemoryStream CreateFicheControle(int refFicheControle, bool courrier, int? bLUnique, string fileType, Context currentContext, ApplicationDbContext dbContext, string rootPath)
        {
            MemoryStream ms = new();
            //Instanciation de l'utilisateur connecté
            Utilisateur utilisateurConnecte = currentContext.ConnectedUtilisateur;
            int i = 0;      //Compteur de lignes
            int c = 0;      //Marqueur de ligne
            int j = 0;      //Marqueur de colonne
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            var excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\FicheDeControle.xlsx");            //Création de la feuille
            //var excelFile = ExcelFile.Load(@"c:\tmp\CommandeAffretement.xlsx");            //Création de la feuille
            GemBox.Spreadsheet.ExcelWorksheet ws = excelFile.Worksheets[0];
            //Initialisations
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            FicheControle fC = dbContext.FicheControles
                    .Include(r => r.FicheControleDescriptionReceptions)
                    .Include(r => r.Controles).ThenInclude(r => r.ControleDescriptionControles)
                    .Include(r => r.CVQs).ThenInclude(r => r.CVQDescriptionCVQs)
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .AsSplitQuery()
                    .Where(i => i.RefFicheControle == refFicheControle).FirstOrDefault();
            Entite client = dbContext.Entites.Where(el => el.RefEntite == fC.CommandeFournisseur.AdresseClient.RefEntite).FirstOrDefault();
            fC.currentCulture = currentContext.CurrentCulture;
            //**********************************************************************************
            //Fiche de contrôle
            //En-tête
            //Nom de la feuille
            ws.Name = fC.CommandeFournisseur.NumeroCommande.ToString();
            //Données
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(207);
            ws.Cells[i, 0].Style.Font.Size = 16 * 20;
            ws.Cells[i, 0].Style.Font.Weight = 700;
            ws.Cells[i, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
            ws.Rows[i].Height = 400;
            i++;
            ws.Cells[i, 0].Value = client.Libelle;
            ws.Cells[i, 0].Style.Font.Size = 14 * 20;
            ws.Cells[i, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
            ws.Rows[i].Height = 400;
            i++;
            string eAdr = "";
            if (!string.IsNullOrWhiteSpace(fC.CommandeFournisseur.AdresseClient?.Adr1)) { eAdr += fC.CommandeFournisseur.AdresseClient.Adr1 + " - ";  }
            if (!string.IsNullOrWhiteSpace(fC.CommandeFournisseur.AdresseClient?.CodePostal)) { eAdr += fC.CommandeFournisseur.AdresseClient.CodePostal + " "; }
            if (!string.IsNullOrWhiteSpace(fC.CommandeFournisseur.AdresseClient?.Ville)) { eAdr += fC.CommandeFournisseur.AdresseClient.Ville + " "; }
            if (!string.IsNullOrWhiteSpace(fC.CommandeFournisseur.AdresseClient?.Pays?.LibelleCourt)) { eAdr += "(" + fC.CommandeFournisseur.AdresseClient.Pays.LibelleCourt + ")"; }
            ws.Cells[i, 0].Value = eAdr;
            ws.Cells[i, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
            ws.Rows[i].Height = 400;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(17);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            eAdr = fC.CommandeFournisseur.Entite.Libelle + (String.IsNullOrWhiteSpace(fC.CommandeFournisseur.Entite.CodeEE) ? "" : " (" + fC.CommandeFournisseur.Entite.CodeEE + ")");
            if (!string.IsNullOrWhiteSpace(fC.CommandeFournisseur.Adr1)) { eAdr += Environment.NewLine + fC.CommandeFournisseur.Adr1; }
            if (!string.IsNullOrWhiteSpace(fC.CommandeFournisseur.CodePostal) || !string.IsNullOrWhiteSpace(fC.CommandeFournisseur.Ville))
            { eAdr += Environment.NewLine; }
            if (!string.IsNullOrWhiteSpace(fC.CommandeFournisseur.CodePostal)) { eAdr += fC.CommandeFournisseur.CodePostal + " "; }
            if (!string.IsNullOrWhiteSpace(fC.CommandeFournisseur.Ville)) { eAdr += fC.CommandeFournisseur.Ville + " "; }
            if (!string.IsNullOrWhiteSpace(fC.CommandeFournisseur.Pays?.LibelleCourt)) { eAdr += "(" + fC.CommandeFournisseur.Pays.LibelleCourt + ")"; }
            ws.Cells[i, 1].Value = eAdr;
            ws.Cells[i, 1].Style.Font.Weight = 700;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(14);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            ws.Cells[i, 1].Value = fC.CommandeFournisseur.NumeroCommande.ToString();
            ws.Cells[i, 1].Style.Font.Weight = 700;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(770);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            ws.Cells[i, 1].Value = fC.NumeroLotUsine;
            ws.Cells[i, 1].Style.Font.Weight = 700;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(48);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            ws.Cells[i, 1].Value = fC.CommandeFournisseur.DDechargement?.ToString("dd/MM/yyyy") ?? currentContext.CulturedRessources.GetTextRessource(771);
            ws.Cells[i, 1].Style.Font.Weight = 700;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(19);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            ws.Cells[i, 1].Value = fC.CommandeFournisseur.Produit.Libelle;
            ws.Cells[i, 1].Style.Font.Weight = 700;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(229);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            ws.Cells[i, 1].Value = (fC.CommandeFournisseur.PoidsChargement != 0 ? fC.CommandeFournisseur.PoidsChargement.ToString() : currentContext.CulturedRessources.GetTextRessource(771));
            ws.Cells[i, 1].Style.Font.Weight = 700;
            ws.Cells[i, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(231);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            ws.Cells[i, 1].Value = (fC.CommandeFournisseur.PoidsChargement != 0 ? fC.CommandeFournisseur.PoidsChargement.ToString() : currentContext.CulturedRessources.GetTextRessource(771));
            ws.Cells[i, 1].Style.Font.Weight = 700;
            ws.Cells[i, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(50);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            ws.Cells[i, 1].Value = fC.CommandeFournisseur.NbBalleDechargement;
            ws.Cells[i, 1].Style.Font.Weight = 700;
            ws.Cells[i, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(20);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            ws.Cells[i, 1].Value = fC.Controleur?.Contact.Prenom + " " + fC.Controleur?.Contact.Nom;
            ws.Cells[i, 1].Style.Font.Weight = 700;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(39);
            ws.Cells[i, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            ws.Cells[i, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
            ws.Cells[i, 1].Value = (fC.Reserve ? currentContext.CulturedRessources.GetTextRessource(205) : currentContext.CulturedRessources.GetTextRessource(206));
            ws.Cells[i, 1].Style.Font.Weight = 700;
            ws.Cells[i, 1].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
            //**********************************************************************************
            //Livraison
            //En-tête
            i = i - 9; //Positionnement sur le haut de la livraison
            ws.Cells[i, 2].Value = currentContext.CulturedRessources.GetTextRessource(42);
            ws.Cells[i, 2].Style.Font.Weight = 700;
            ws.Cells[i, 2].Style.Font.Size = 12 * 20;
            ws.Cells.GetSubrangeAbsolute(i, 2, i, 4).Merged = true;
            ws.Cells.GetSubrangeAbsolute(i, 2, i, 4).Style.Borders.SetBorders(MultipleBorders.Outside, System.Drawing.Color.Black, LineStyle.Thin);
            ws.Cells.GetSubrangeAbsolute(i, 2, i, 4).Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Cells.GetSubrangeAbsolute(i, 2, i, 4).Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            i++;
            c = i;
            foreach (var fCDR in fC.FicheControleDescriptionReceptions.OrderBy(o => o.Ordre))
            {
                fCDR.currentCulture = currentContext.CurrentCulture;
                ws.Cells[i, 2].Value = fCDR.LibelleCultured;
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Merged = true;
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 4].Value = (fCDR.Positif == true ? fCDR.OuiCultured : fCDR.NonCultured);
                ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                //Mise en évidence d'une réponse négative
                if (fCDR.Positif != true)
                {
                    ws.Cells[i, 4].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    ws.Cells[i, 4].Style.Font.Weight = 700;
                }
                //Bordure supérieure sauf premier élément
                if (i != c)
                {
                    ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                    ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                    ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                }
                //Next
                i++;
            }
            ws.Cells[i, 2].Value = fC.Cmt;
            ws.Cells.GetSubrangeAbsolute(i, 2, i, 4).Merged = true;
            ws.Cells.GetSubrangeAbsolute(i, 2, i, 4).Style.Borders.SetBorders(MultipleBorders.All, System.Drawing.Color.Black, LineStyle.Thin);
            ws.Cells.GetSubrangeAbsolute(i, 2, i, 4).Style.VerticalAlignment = VerticalAlignmentStyle.Top;
            ws.Rows[i].Height = 700;
            //Positionnement sur la première ligne du CVQ
            if (i < 15) { i = 15; } else { i++; }
            //**********************************************************************************
            //Contrôle Visuel Quantifié
            foreach (var cVQ in fC.CVQs)
            {
                cVQ.currentCulture = currentContext.CurrentCulture;
                //En-tête
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(51);
                ws.Cells[i, 0].Style.Font.Weight = 700;
                ws.Cells[i, 0].Style.Font.Size = 12 * 20;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Merged = true;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                ws.Cells[i, 2].Value = currentContext.CulturedRessources.GetTextRessource(20);
                ws.Cells[i, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 3].Value = cVQ.Controleur?.Contact.Prenom + " " + cVQ.Controleur?.Contact.Nom;
                ws.Cells[i, 3].Style.Font.Weight = 700;
                ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                i++;
                ws.Cells[i, 0].Value = cVQ.Cmt;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Merged = true;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Value = currentContext.CulturedRessources.GetTextRessource(22);
                ws.Cells[i, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                ws.Cells[i, 3].Value = cVQ.DCVQ?.ToString("dd/MM/yyyy");
                ws.Cells[i, 3].Style.Font.Weight = 700;
                ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 3].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                ws.Rows[i].Height = 700;
                //En-tête du tableau de description
                i++;    //On passe une ligne
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Merged = true;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Value = currentContext.CulturedRessources.GetTextRessource(27);
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Font.Weight = 700;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Outside, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Value = currentContext.CulturedRessources.GetTextRessource(132);
                ws.Cells[i, 2].Style.Font.Weight = 700;
                ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Outside, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells[i, 3].Value = currentContext.CulturedRessources.GetTextRessource(29);
                ws.Cells[i, 3].Style.Font.Weight = 700;
                ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Outside, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                i++;    //On passe une ligne
                        //Mémorisation du numéro de la première ligne
                c = i;
                foreach (var cVQDCVQ in cVQ.CVQDescriptionCVQs.OrderBy(o => o.Ordre))
                {
                    cVQDCVQ.currentCulture = currentContext.CurrentCulture;
                    ws.Cells[i, 0].Value = cVQDCVQ.LibelleCultured;
                    ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Merged = true;
                    ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells[i, 2].Value = cVQDCVQ.Nb;
                    ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells[i, 3].Value = cVQDCVQ.Qualite;
                    ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                    //Mise en évidence d'une qualité basse
                    if (cVQDCVQ.Qualite == 3)
                    {
                        ws.Cells[i, 3].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                        ws.Cells[i, 3].Style.Font.Weight = 700;
                    }
                    //Bordure supérieure sauf premier élément
                    if (i != c)
                    {
                        ws.Cells[i, 0].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                        ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                        ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                        ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                    }
                    //Next
                    i++;
                }
                i--;
                for (j = 0; j < 4; j++)
                {
                    ws.Cells[i, j].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                }
                i++;
                i++;
            }

            //Positionnement sur la première ligne du premier contrôle
            i++;
            //**********************************************************************************
            //Contrôles
            foreach (var ctl in fC.Controles)
            {
                ctl.currentCulture = currentContext.CurrentCulture;
                //En-tête
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(52);
                ws.Cells[i, 0].Style.Font.Size = 12 * 20;
                ws.Cells[i, 0].Style.Font.Weight = 700;
                ws.Cells[i, 0].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 0].Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Merged = true;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                ws.Cells[i, 2].Value = currentContext.CulturedRessources.GetTextRessource(20);
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Merged = true;
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 4].Value = ctl.Controleur?.Contact.Prenom + " " + ctl.Controleur?.Contact.Nom;
                ws.Cells[i, 4].Style.Font.Weight = 700;
                ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                i++;
                ws.Cells[i, 0].Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Merged = true;
                ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Value = currentContext.CulturedRessources.GetTextRessource(22);
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Merged = true;
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 4].Value = ctl.DControle?.ToString("dd/MM/yyyy");
                ws.Cells[i, 4].Style.Font.Weight = 700;
                ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                i++;
                ws.Cells[i, 0].Value = ctl.Cmt;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Merged = true;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Value = currentContext.CulturedRessources.GetTextRessource(124);
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Merged = true;
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(i, 2, i, 3).Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                ws.Cells[i, 4].Value = ctl.Poids;
                ws.Cells[i, 4].Style.Font.Weight = 700;
                ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                ws.Cells[i, 4].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                ws.Rows[i].Height = 700;
                //En-tête du tableau de description
                i++;    //On passe une ligne
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Merged = true;
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(1267);
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Font.Weight = 700;
                ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Outside, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Value = currentContext.CulturedRessources.GetTextRessource(56);
                ws.Cells[i, 2].Style.Font.Weight = 700;
                ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Outside, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells[i, 3].Value = currentContext.CulturedRessources.GetTextRessource(203);
                ws.Cells[i, 3].Style.Font.Weight = 700;
                ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Outside, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells[i, 4].Value = currentContext.CulturedRessources.GetTextRessource(136);
                ws.Cells[i, 4].Style.Font.Weight = 700;
                ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Outside, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                i++;    //On passe une ligne
                //Mémorisation du numéro de la première ligne
                c = i;
                double tPoids = 0; //Total d'indésirables
                double tPercent = 0; //Total d'indésirables
                foreach (var ctlDCtl in ctl.ControleDescriptionControles.OrderBy(o => o.Ordre))
                {
                    ctlDCtl.currentCulture = currentContext.CurrentCulture;
                    ws.Cells[i, 0].Value = ctlDCtl.LibelleCultured;
                    ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Merged = true;
                    ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells.GetSubrangeAbsolute(i, 0, i, 1).Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells[i, 2].Value = Math.Round(ctlDCtl.Poids, 3, MidpointRounding.AwayFromZero);
                    ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells[i, 3].Value = Math.Round(ctlDCtl.Poids / (double)(ctl.Poids ?? 0) * 100, 2, MidpointRounding.AwayFromZero);
                    ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells[i, 4].Value = ctlDCtl.CalculLimiteConformite ? currentContext.CulturedRessources.GetTextRessource(205) : currentContext.CulturedRessources.GetTextRessource(206);
                    ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                    ws.Cells[i, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    //Mise en évidence d'un nombre renseigné
                    if (ctlDCtl.Poids > 0 && ctlDCtl.CalculLimiteConformite)
                    {
                        ws.Cells[i, 2].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                        ws.Cells[i, 2].Style.Font.Weight = 700;
                        ws.Cells[i, 3].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                        ws.Cells[i, 3].Style.Font.Weight = 700;
                    }
                    //Total d'indésirable
                    if (ctlDCtl.CalculLimiteConformite)
                    {
                        tPoids += Math.Round(ctlDCtl.Poids, 3, MidpointRounding.AwayFromZero);
                        tPercent += Math.Round(ctlDCtl.Poids / (double)(ctl.Poids ?? 0) * 100, 2, MidpointRounding.AwayFromZero);
                    }
                    //Bordure supérieure sauf premier élément
                    if (i != c)
                    {
                        ws.Cells[i, 0].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                        ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                        ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                        ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                        ws.Cells[i, 4].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Hair);
                    }
                    //Next
                    i++;
                }
                for (j = 0; j < 5; j++)
                {
                    ws.Cells[i, j].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                }
                //Total
                ws.Cells[i, 1].Value = currentContext.CulturedRessources.GetTextRessource(790);
                ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 1].Style.Font.Weight = 700;
                ws.Cells[i, 1].Style.Font.Size = 12 * 20;
                ws.Cells[i, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells[i, 2].Value = Math.Round(tPoids, 2, MidpointRounding.AwayFromZero) + "kg";
                ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Style.Font.Weight = 700;
                ws.Cells[i, 2].Style.Font.Size = 12 * 20;
                ws.Cells[i, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                ws.Cells[i, 3].Value = Math.Round(tPercent, 2, MidpointRounding.AwayFromZero) + "%";
                ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 3].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 3].Style.Font.Weight = 700;
                ws.Cells[i, 3].Style.Font.Size = 12 * 20;
                ws.Cells[i, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                i++;
                i++;
            }
            //**********************************************************************************
            //Mise en page générale
            int r = 0;
            //Hauteur des lignes
            for (r = 0; r <= i; r++)
            {
                //ws.Rows[r].Height = 200;
            }
            //Wrap
            for (r = 0; r <= i; r++)
            {
                for (c = 0; c < 5; c++)
                {
                    if (r != 0 && c != 0) { ws.Cells[r, c].Style.WrapText = true; }
                }
            }
            //Options d'impression
            ws.Columns[0].Width = 9000;
            ws.Columns[1].Width = 12000;
            ws.Columns[2].Width = 4000;
            ws.Columns[3].Width = 4000;
            ws.Columns[4].Width = 4000;
            ws.PrintOptions.FitWorksheetWidthToPages = 1;
            ws.PrintOptions.FitWorksheetHeightToPages = 0;
            ws.PrintOptions.AutomaticPageBreakScalingFactor = 90;
            ws.PrintOptions.BottomMargin = 0.4;
            ws.PrintOptions.TopMargin = 0.4;
            ws.PrintOptions.LeftMargin = 0.4;
            ws.PrintOptions.RightMargin = 0.4;
            ws.PrintOptions.FitToPage = true;
            ws.PrintOptions.Portrait = true;
            //Sauvegarde du fichier
            if (fileType == "xlsx")
            {
                excelFile.Save(ms, SaveOptions.XlsxDefault);
            }
            else
            {
                excelFile.Save(ms, new PdfSaveOptions()
                {
                    SelectionType = SelectionType.EntireFile
                });
            }
            return ms;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Create FicheControle
        /// </summary>
        public static MemoryStream CreateNonConformite(int refNonConformite, string docType, Context currentContext, ApplicationDbContext dbContext, string rootPath)
        {
            MemoryStream ms = new();
            //Connected user
            Utilisateur utilisateurConnecte = currentContext.ConnectedUtilisateur;
            int i = 0;      //Compteur de lignes
            int j = 0;      //Marqueur de colonne
            SpreadsheetInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            var excelFile = ExcelFile.Load(rootPath + @"\Assets\Templates\NonConformite.xlsx");            //Création de la feuille
            GemBox.Spreadsheet.ExcelWorksheet ws = excelFile.Worksheets[0];
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            //Process
            var nC = dbContext.NonConformites
                .Include(l => l.NonConformiteNonConformiteFamilles)
                .Include(l => l.NonConformiteEtapes)
                .Include(l => l.NonConformiteFichiers)
                .AsSplitQuery()
                .Where(el => el.RefNonConformite == refNonConformite).FirstOrDefault();
            nC.currentCulture = currentContext.CurrentCulture;
            if (nC.NonConformiteDemandeClientType != null) { nC.NonConformiteDemandeClientType.currentCulture = currentContext.CurrentCulture; }
            if (nC.NonConformiteNature != null) { nC.NonConformiteNature.currentCulture = currentContext.CurrentCulture; }
            if (nC.NonConformiteReponseClientType != null) { nC.NonConformiteReponseClientType.currentCulture = currentContext.CurrentCulture; }
            if (nC.NonConformiteReponseFournisseurType != null) { nC.NonConformiteReponseFournisseurType.currentCulture = currentContext.CurrentCulture; }
            var client = dbContext.Entites.Where(el => el.RefEntite == nC.CommandeFournisseur.AdresseClient.RefEntite).FirstOrDefault();
            client.currentCulture = currentContext.CurrentCulture;
            var fC = dbContext.FicheControles.Where(el => el.RefCommandeFournisseur == nC.CommandeFournisseur.RefCommandeFournisseur).FirstOrDefault();
            if (fC != null) { fC.currentCulture = currentContext.CurrentCulture; }
            ws.Name = nC.CommandeFournisseur.NumeroCommande.ToString();
            //Adresses
            string eAdr = "";
            if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Adr1)) { eAdr += nC.CommandeFournisseur.AdresseClient.Adr1 + " - "; }
            if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.CodePostal)) { eAdr += nC.CommandeFournisseur.AdresseClient.CodePostal + " "; }
            if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Ville)) { eAdr += nC.CommandeFournisseur.AdresseClient.Ville + " "; }
            if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Pays?.LibelleCourt)) { eAdr += "(" + nC.CommandeFournisseur.AdresseClient.Pays.LibelleCourt + ")"; }
            //Data
            i = 1;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(862);
            i++;
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = client?.Libelle; 
                i++;
                eAdr = "";
                if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Adr1)) { eAdr += nC.CommandeFournisseur.AdresseClient.Adr1 + " - "; }
                if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.CodePostal)) { eAdr += nC.CommandeFournisseur.AdresseClient.CodePostal + " "; }
                if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Ville)) { eAdr += nC.CommandeFournisseur.AdresseClient.Ville + " "; }
                if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Pays?.LibelleCourt)) { eAdr += "(" + nC.CommandeFournisseur.AdresseClient.Pays.LibelleCourt + ")"; }
                ws.Cells[i, 0].Value = eAdr;
            }
            i = 4; 
            if (docType == "Fournisseur")
            {
                eAdr = "";
                if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Adr1)) { eAdr += nC.CommandeFournisseur.AdresseClient.Adr1; }
                if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.CodePostal)
                    || !string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Ville)
                    || !string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Pays?.LibelleCourt)
                    )
                {
                    eAdr += Environment.NewLine;
                }
                if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.CodePostal)) { eAdr += nC.CommandeFournisseur.AdresseClient.CodePostal + " "; }
                if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Ville)) { eAdr += nC.CommandeFournisseur.AdresseClient.Ville + " "; }
                if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.AdresseClient?.Pays?.LibelleCourt)) { eAdr += "(" + nC.CommandeFournisseur.AdresseClient.Pays.LibelleCourt + ")"; }
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(16);
                ws.Cells[i, 2].Value = client?.Libelle + Environment.NewLine + eAdr;
                i++;
            }
            else
            {
                ws.Rows.Remove(i);
                ws.Cells[i, 0].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 1].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i, 2].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.Black, LineStyle.Thin);
            }
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(14);
            ws.Cells[i, 2].Value = nC.CommandeFournisseur.NumeroCommande;
            i++;
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(770);
                ws.Cells[i, 2].Value = fC?.NumeroLotUsine;
                i++;
            }
            else { ws.Rows.Remove(i); }
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(277);
            ws.Cells[i, 2].Value = nC.CommandeFournisseur.DDechargement;
            i++;
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(231);
                ws.Cells[i, 2].Value = nC.CommandeFournisseur.PoidsDechargement;
                i++;
            }
            else
            {
                ws.Rows.Remove(i);
                ws.Cells[i - 1, 0].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i - 1, 1].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i - 1, 2].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
            }
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(232);
                ws.Cells[i, 2].Value = nC.CommandeFournisseur.NbBalleDechargement;
                i++;
            }
            else { ws.Rows.Remove(i); }
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(17);
            eAdr = nC.CommandeFournisseur.Entite.Libelle + (String.IsNullOrWhiteSpace(nC.CommandeFournisseur.Entite.CodeEE) ? "" : " (" + nC.CommandeFournisseur.Entite.CodeEE + ")");
            if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.Adr1)) { eAdr += Environment.NewLine + nC.CommandeFournisseur.Adr1; }
            if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.CodePostal)
                || !string.IsNullOrWhiteSpace(nC.CommandeFournisseur.Ville)
                || !string.IsNullOrWhiteSpace(nC.CommandeFournisseur.Pays?.LibelleCourt)
                )
            {
                eAdr += Environment.NewLine;
            }
            if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.CodePostal)) { eAdr += nC.CommandeFournisseur.CodePostal + " "; }
            if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.Ville)) { eAdr += nC.CommandeFournisseur.Ville + " "; }
            if (!string.IsNullOrWhiteSpace(nC.CommandeFournisseur.Pays?.LibelleCourt)) { eAdr += "(" + nC.CommandeFournisseur.Pays.LibelleCourt + ")"; }
            ws.Cells[i, 2].Value = eAdr;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(19);
            ws.Cells[i, 2].Value = nC.CommandeFournisseur.Produit.Libelle;
            i++;
            if (docType == "Fournisseur")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(229);
                ws.Cells[i, 2].Value = nC.CommandeFournisseur.PoidsChargement;
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Fournisseur")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(50);
                ws.Cells[i, 2].Value = nC.CommandeFournisseur.NbBalleChargement;
                i++;
            }
            else
            {
                ws.Rows.Remove(i);
                ws.Cells[i - 1, 0].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i - 1, 1].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                ws.Cells[i - 1, 2].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
            }
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(858);
            ws.Cells[i, 2].Value = nC.NonConformiteEtapes.Where(e => e.RefNonConformiteEtapeType == 1).FirstOrDefault().DCreation;
            i++;
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(57);
                ws.Cells[i, 2].Value = nC.DescrClient;
                i++;
            }
            else { ws.Rows.Remove(i); }
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(227);
            ws.Cells[i, 2].Value = nC.DescrValorplast;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(830);
            ws.Cells[i, 2].Value = nC.NonConformiteNature?.Libelle;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(816);
            ws.Cells[i, 2].Value = nC.NonConformiteFamilles;
            i++;
            ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(827);
            ws.Cells[i, 2].Value = (nC.IFFournisseurRetourLot ? currentContext.CulturedRessources.GetTextRessource(205) : currentContext.CulturedRessources.GetTextRessource(206));
            i++; i++;
            if (docType == "Fournisseur")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(185);
                ws.Cells[i, 2].Value = (!string.IsNullOrEmpty(nC.IFFournisseurDescr) || nC.IFFournisseurFactureMontant != null || nC.IFFournisseurDeductionTonnage != null ? currentContext.CulturedRessources.GetTextRessource(205) : currentContext.CulturedRessources.GetTextRessource(206));
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Fournisseur")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(825);
                ws.Cells[i, 2].Value = nC.IFFournisseurDescr;
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Fournisseur")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(826);
                ws.Cells[i, 2].Value = nC.IFFournisseurFactureMontant;
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Fournisseur")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(236);
                ws.Cells[i, 2].Value = nC.IFFournisseurDeductionTonnage;
                i++;
            }
            else { ws.Rows.Remove(i); }
            i++;
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(193);
                ws.Cells[i, 2].Value = (!string.IsNullOrEmpty(nC.IFClientDescr) || nC.IFClientFactureMontant != null ? currentContext.CulturedRessources.GetTextRessource(205) : currentContext.CulturedRessources.GetTextRessource(206));
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(825);
                ws.Cells[i, 2].Value = nC.IFClientDescr;
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(826);
                ws.Cells[i, 2].Value = nC.IFClientFactureMontant;
                i++; i++;
            }
            else { ws.Rows.Remove(i); ws.Rows.Remove(i); }
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(836);
                ws.Cells[i, 2].Value = nC.NonConformiteReponseClientType?.Libelle;
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(104);
                ws.Cells[i, 2].Value = nC.CmtReponseClient;
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(847);
                ws.Cells[i, 2].Value = nC.CmtOrigineAction;
                i++; i++;
            }
            else { ws.Rows.Remove(i); ws.Rows.Remove(i); }
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(841);
                ws.Cells[i, 2].Value = nC.IFClientFactureNro;
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(843);
                ws.Cells[i, 2].Value = nC.IFClientDFacture;
                i++;
            }
            else { ws.Rows.Remove(i); }
            if (docType == "Client")
            {
                ws.Cells[i, 0].Value = currentContext.CulturedRessources.GetTextRessource(104);
                ws.Cells[i, 2].Value = nC.IFClientCmtFacturation;
            }
            else { ws.Rows.Remove(i); }
            i++; i++;
            //Pictures if exist
            if (nC.NonConformiteFichiers != null)
            {
                //Set initial position
                j = 0;
                //For each file
                foreach (var f in nC.NonConformiteFichiers)
                {
                    //If Picture from Client
                    if (f.Miniature != null && f.RefNonConformiteFichierType == 1)
                    {
                        MemoryStream mSF = new(f.Corps);
                        //Calculate Excel image size
                        int offset = 17;
                        int imgMaxSize = (int)LengthUnitConverter.Convert(ws.DefaultRowHeight, LengthUnit.Twip, LengthUnit.Pixel) * (offset - 1);
                        int imgHeight = 0, imgWidth = 0;

                        using (var img = System.Drawing.Image.FromStream(mSF))
                        {
                            bool portrait = img.Height >= img.Width;
                            if (portrait)
                            {
                                imgHeight = imgMaxSize;
                                imgWidth = (int)(img.Width * (imgMaxSize / (decimal)img.Height));
                            }
                            else
                            {
                                imgWidth = imgMaxSize;
                                imgHeight = (int)(img.Height * (imgMaxSize / (decimal)img.Width));
                            }
                        }
                        //Add Picture
                        ws.Pictures.Add(mSF, ExcelPictureFormat.Jpeg
                        , new AnchorCell(ws.Columns[j], ws.Rows[i], 0, 2, LengthUnit.Pixel), imgWidth, imgHeight, LengthUnit.Pixel
                        ).Position.Mode = PositioningMode.MoveAndSize;
                        ////Save image to file system
                        //var fS = new FileStream("d:\\tmp\\"+f.Nom, FileMode.Create);
                        //mSF.WriteTo(fS);
                        //fS.Close();
                        //Next positionning
                        if (j == 0) { j = 2; }
                        else { j = 0; i += offset; }
                    }
                }
            }
            double maxHeight = LengthUnitConverter.Convert(
                ws.PrintOptions.PageHeight - ws.PrintOptions.TopMargin - ws.PrintOptions.BottomMargin,
                LengthUnit.Inch, LengthUnit.Pixel);

            double currentHeight = 0;
            var count = ws.Pictures.Last().Position.To.Row.Index;

            for (int index = 0; index < count; index++)
            {
                var currentRow = ws.Rows[index];
                currentRow.AutoFit(true);

                var currentRowHeight = currentRow.GetHeight(LengthUnit.Pixel);
                currentHeight += currentRowHeight;

                if (currentHeight >= maxHeight)
                {
                    currentHeight = currentRowHeight;

                    var picture = ws.Pictures.FirstOrDefault(pic => index > pic.Position.From.Row.Index && index < pic.Position.To.Row.Index);
                    if (picture != null)
                    {
                        int rowBreakIndex = picture.Position.From.Row.Index;
                        ws.HorizontalPageBreaks.Add(rowBreakIndex);
                        currentHeight = Enumerable.Range(rowBreakIndex, index - rowBreakIndex + 1)
                            .Select(i => ws.Rows[i].GetHeight(LengthUnit.Pixel))
                            .Sum();
                    }
                }
            }
            //Save file
            //excelFile.Save(ms, SaveOptions.XlsxDefault);
            excelFile.Save(ms, new PdfSaveOptions()
            {
                SelectionType = SelectionType.ActiveSheet,
                ImageDpi = 220
            });
            //End
            return ms;
        }
    }
}



