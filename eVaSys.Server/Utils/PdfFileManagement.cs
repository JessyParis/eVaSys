/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création :08/04/2021
/// ----------------------------------------------------------------------------------------------------- 

using eVaSys.Data;
using GemBox.Pdf;
using GemBox.Pdf.Content;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Telerik.Documents.Core.Fonts;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;
using FontFamily = Telerik.Documents.Core.Fonts.FontFamily;

namespace eVaSys.Utils
{
    /// <summary>
    /// PDF files management 
    /// </summary>
    public static class PdfFileManagement
    {
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Note de crédit SAGE
        /// </summary>
        public static MemoryStream CreateNoteCredit_test(string refDoc, ApplicationDbContext dbContext, string rootPath)
        {
            MemoryStream mS = new();
            //Initialisation du fichier PDF
            RadFixedDocument document = new();
            RadFixedDocumentEditor editor = new(document);
            //Informations
            document.DocumentInfo.Author = "e-Valorplast";
            document.DocumentInfo.Title = "Note de crédit";
            editor.SectionProperties.PageSize = new Telerik.Documents.Primitives.Size(793, 1123);
            editor.SectionProperties.PageMargins = new Thickness(47, 30, 45, 30);
            //Style
            Thickness noPadding = new(0, 0, 0, 0);
            //Borders
            Border stdBorder = new();
            Border noBorder = new(BorderStyle.None);
            Border thinBorder = new Border(2, new RgbColor(0, 0, 0));

            //Edition
            //----------------------------------------------------
            //Table principale
            Table mainTable = new();
            mainTable.DefaultCellProperties.Borders = new TableCellBorders(noBorder);
            mainTable.BorderCollapse = BorderCollapse.Collapse;
            mainTable.DefaultCellProperties.Padding = noPadding;
            //----------------------------------------------------
            //Ligne table titre
            TableRow mainRow = mainTable.Rows.AddTableRow();
            TableCell mainCell = mainRow.Cells.AddTableCell();
            mainCell.PreferredWidth = 700;
            //----------------------------------------------------
            //Table titre
            Table table = mainCell.Blocks.AddTable();
            table.DefaultCellProperties.Borders = new TableCellBorders(noBorder);
            table.BorderCollapse = BorderCollapse.Collapse;
            //Row logo | title
            TableRow row = table.Rows.AddTableRow();
            TableCell cell = row.Cells.AddTableCell();
            cell.PreferredWidth = 400;
            cell = row.Cells.AddTableCell();
            cell.PreferredWidth = 300;
            cell.Padding = noPadding;
            cell.VerticalAlignment = VerticalAlignment.Center;
            Block blk = new();
            blk.InsertText("Note de credit");
            cell.Blocks.Add(blk);
            //Row doc info | Adress 
            row = table.Rows.AddTableRow();
            //Cell 1
            cell = row.Cells.AddTableCell();
            cell.PreferredWidth = 400;
            cell.Padding = noPadding;
            blk = new();
            blk.InsertText("Numéro : ");
            blk.InsertLineBreak();
            blk.InsertText("Date : ");
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            //Cell 2
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.PreferredWidth = 300;
            cell.Padding = noPadding;
            blk = new();
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            blk = new();
            blk.InsertText("1");
            blk.InsertLineBreak();
            blk.InsertText("2");
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            //----------------------------------------------------
            //Table document lines
            mainRow = mainTable.Rows.AddTableRow();
            mainCell = mainRow.Cells.AddTableCell();
            mainCell.PreferredWidth = 700;
            table = mainCell.Blocks.AddTable();
            //Header row
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(noBorder, noBorder, noBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("Ref cell");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 350;
            blk = new();
            blk.InsertText("Désignation");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("Qté");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("P. U. moyen");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("Montant HT (€)");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("Code TVA");
            cell.Blocks.Add(blk);
            //Other row
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("Code article");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 350;
            blk = new();
            blk.InsertText("Désignation");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("Qté");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("P. U. moyen");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("Montant HT (€)");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, noBorder);
            cell.PreferredWidth = 70;
            blk = new();
            blk.InsertText("Code TVA");
            cell.Blocks.Add(blk);
            //----------------------------------------------------
            //Fin
            editor.InsertTable(mainTable);
            //Save to database
            PdfFormatProvider provider = new();
            provider.Export(document, mS);
            //End
            return mS;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Note de crédit SAGE
        /// </summary>
        public static MemoryStream CreateNoteCredit(string refDoc, ApplicationDbContext dbContext, string rootPath)
        {
            MemoryStream mS = new();
            //Get SAGE document
            var sAGEDocument = dbContext.SAGEDocuments
                .Include(i => i.SAGEDocumentLignes)
                .ThenInclude(i => i.SAGEDocumentArticle)
                .Include(i => i.SAGEDocumentLignes)
                .ThenInclude(i => i.SAGEDocumentLigneText)
                .Include(i => i.SAGEDocumentCompteTVA)
                .Where(e => e.RefSAGEDocument == refDoc)
                .FirstOrDefault();
            //Initialisation du fichier PDF
            RadFixedDocument document = new();
            RadFixedDocumentEditor editor = new(document);
            //Informations
            document.DocumentInfo.Author = "e-Valorplast";
            document.DocumentInfo.Title = "Pièce comptable";
            editor.SectionProperties.PageSize = new Telerik.Documents.Primitives.Size(793, 1123);
            editor.SectionProperties.PageMargins = new Thickness(47, 30, 45, 30);
            //Style
            Thickness stdPadding = new(10, 10, 10, 10);
            Thickness thinPadding = new(5, 5, 5, 5);
            Thickness noPadding = new(0, 0, 0, 0);
            TextProperties footer = new();
            footer.FontSize = 10;
            TextProperties normal = new();
            normal.FontSize = 12;
            TextProperties medium = new();
            medium.FontSize = 14;
            TextProperties large = new();
            large.FontSize = 24;
            //Borders
            Border stdBorder = new();
            Border noBorder = new(BorderStyle.None);
            Border mediumBorder = new Border(2, new RgbColor(0, 0, 0));
            Border thinBorder = new Border(1, new RgbColor(0, 0, 0));

            //Font
            FontBase arial;
            FontsRepository.RegisterFont(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, File.ReadAllBytes(rootPath + @"\Assets\Fonts\arial.ttf"));
            FontsRepository.TryCreateFont(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, out arial);
            FontBase arialBold;
            FontsRepository.RegisterFont(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, File.ReadAllBytes(rootPath + @"\Assets\Fonts\arialbd.ttf"));
            FontsRepository.TryCreateFont(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, out arialBold);
            //Edition
            //----------------------------------------------------
            //Table principale
            Table mainTable = new();
            mainTable.DefaultCellProperties.Borders = new TableCellBorders(noBorder);
            mainTable.BorderCollapse = BorderCollapse.Collapse;
            mainTable.DefaultCellProperties.Padding = noPadding;
            //----------------------------------------------------
            //Ligne table titre
            TableRow mainRow = mainTable.Rows.AddTableRow();
            TableCell mainCell = mainRow.Cells.AddTableCell();
            mainCell.PreferredWidth = 700;
            //----------------------------------------------------
            //Table titre
            Table table = mainCell.Blocks.AddTable();
            table.DefaultCellProperties.Borders = new TableCellBorders(noBorder);
            table.BorderCollapse = BorderCollapse.Collapse;
            //Row logo | title
            TableRow row = table.Rows.AddTableRow();
            TableCell cell = row.Cells.AddTableCell();
            cell.PreferredWidth = 400;
            cell.VerticalAlignment = VerticalAlignment.Top;
            using (Stream stream = File.OpenRead(rootPath + @"\assets\images\logo-expert-recyclage.jpg"))
            {
                Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource imageSource = new(stream, ImageQuality.High);
                cell.Blocks.AddBlock().InsertImage(imageSource, 200, (double)imageSource.Height / ((double)imageSource.Width / 200));
            }
            cell = row.Cells.AddTableCell();
            cell.PreferredWidth = 300;
            cell.Padding = noPadding;
            cell.VerticalAlignment = VerticalAlignment.Center;
            Block blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = large.FontSize;
            blk.InsertText(sAGEDocument.Type);
            cell.Blocks.Add(blk);
            //Row space
            row = table.Rows.AddTableRow();
            //Cell 1
            cell = row.Cells.AddTableCell();
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            //Cell 2
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Padding = noPadding;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            //Row doc info | Adress 
            row = table.Rows.AddTableRow();
            //Cell 1
            cell = row.Cells.AddTableCell();
            cell.Padding = noPadding;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertText("Numéro : " + sAGEDocument.RefSAGEDocument);
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertText("Date : " + sAGEDocument.D.ToString("dd/MM/yyyy"));
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertText("Vos références : " + sAGEDocument.CodeComptable);
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertText("Condition de livraison : " + sAGEDocument.ConditionLivraison);
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertText("N° TVA : " + sAGEDocument.SAGEDocumentCompteTVA?.CompteTVA);
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            //Cell 2
            cell = row.Cells.AddTableCell();
            cell.Padding = noPadding;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertText(sAGEDocument.Adr1);
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            blk = new();
            blk.TextProperties.Font = arial;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertText(sAGEDocument.Adr2);
            blk.InsertLineBreak();
            blk.InsertText(sAGEDocument.Adr3);
            blk.InsertLineBreak();
            blk.InsertText(sAGEDocument.Adr4);
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            //Row space
            row = table.Rows.AddTableRow();
            //Cell 1
            cell = row.Cells.AddTableCell();
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            //Cell 2
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Padding = noPadding;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = medium.FontSize;
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            //----------------------------------------------------
            //Table document lines
            mainRow = mainTable.Rows.AddTableRow();
            mainCell = mainRow.Cells.AddTableCell();
            mainCell.PreferredWidth = 700;
            table = mainCell.Blocks.AddTable();
            table.DefaultCellProperties.Padding = thinPadding;
            //Header row
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(mediumBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 80;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Code article");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 300;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Désignation");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 80;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Qté (t)");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 80;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("P. U. moyen");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 100;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Montant HT (€)");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, mediumBorder, thinBorder);
            cell.PreferredWidth = 60;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Code TVA");
            cell.Blocks.Add(blk);
            //Line rows
            foreach (var sDL in sAGEDocument.SAGEDocumentLignes)
            {
                row = table.Rows.AddTableRow();
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(mediumBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Center;
                blk.InsertText(sDL.RefSAGEDocumentArticle);
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.InsertText(sDL.SAGEDocumentArticle.Designation1);
                if (!string.IsNullOrWhiteSpace(sDL.SAGEDocumentArticle.Designation2))
                {
                    blk.InsertLineBreak();
                    blk.InsertText(sDL.SAGEDocumentArticle.Designation2);
                }
                if (!string.IsNullOrWhiteSpace(sDL.SAGEDocumentLigneText?.Cmt))
                {
                    blk.InsertLineBreak();
                    blk.InsertText(sDL.SAGEDocumentLigneText.Cmt);
                }
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Right;
                blk.InsertText(((decimal)(sDL.Quantite ?? 0)).ToString("### ##0.000"));
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Right;
                blk.InsertText(((decimal)(sDL.PUHT ?? 0)).ToString("### ##0.00"));
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Right;
                blk.InsertText(((decimal)(sDL.TotalHT ?? 0)).ToString("### ##0.00"));
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, mediumBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Center;
                blk.InsertText(sDL.TVACode.ToString());
                cell.Blocks.Add(blk);
            }
            //Last rows
            for (int i = 0; i <= (10 - sAGEDocument.SAGEDocumentLignes.Count); i++)
            {
                row = table.Rows.AddTableRow();
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(mediumBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Right;
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Right;
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Right;
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, noBorder, mediumBorder, noBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Center;
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                cell.Blocks.Add(blk);
            }
            //Last row
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(mediumBorder, noBorder, thinBorder, mediumBorder);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, mediumBorder);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, mediumBorder);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, mediumBorder);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, noBorder, thinBorder, mediumBorder);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, noBorder, mediumBorder, mediumBorder);
            //----------------------------------------------------
            //Tables totals
            //Calculate
            sAGEDocument.CalcTotaux();
            //Init main row
            mainRow = mainTable.Rows.AddTableRow();
            //Init table container for 2 totals tables
            mainCell = mainRow.Cells.AddTableCell();
            mainCell.PreferredWidth = 700;
            Table tableContainer = mainCell.Blocks.AddTable();
            tableContainer.DefaultCellProperties.Borders = new TableCellBorders(noBorder);
            tableContainer.BorderCollapse = BorderCollapse.Collapse;
            TableRow rowContainer = tableContainer.Rows.AddTableRow();
            //First table
            TableCell cellContainer = rowContainer.Cells.AddTableCell();
            cellContainer.PreferredWidth = 290;
            cellContainer.Padding = new Thickness(0, 5, 5, 0);
            table = cellContainer.Blocks.AddTable();
            table.DefaultCellProperties.Padding = thinPadding;
            //Header row
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(mediumBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 60;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Code");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 90;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Base HT €");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 60;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Taux");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, mediumBorder, thinBorder);
            cell.PreferredWidth = 80;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("TVA €");
            cell.Blocks.Add(blk);
            //TVA types rows
            foreach (var dR in sAGEDocument.Totaux.Select("TVACode in (8,1)"))
            {
                row = table.Rows.AddTableRow();
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(mediumBorder, thinBorder, thinBorder, thinBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Center;
                blk.InsertText(dR["TVACode"].ToString());
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, thinBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Right;
                blk.InsertText(((decimal)dR["BaseHT"]).ToString("### ##0.00"));
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, thinBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Center;
                blk.InsertText(((decimal)dR["TVATaux"]).ToString("0.00") + "%");
                cell.Blocks.Add(blk);
                cell = row.Cells.AddTableCell();
                cell.Borders = new TableCellBorders(thinBorder, thinBorder, mediumBorder, thinBorder);
                blk = new();
                blk.TextProperties.Font = arial;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Right;
                blk.InsertText(((decimal)dR["Montant"]).ToString("### ##0.00"));
                cell.Blocks.Add(blk);
            }
            //Total TVA
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(mediumBorder, thinBorder, thinBorder, thinBorder);
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Total");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, thinBorder);
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Right;
            blk.InsertText(((decimal)(sAGEDocument.TotalHT ?? 0)).ToString("### ##0.00"));
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.ColumnSpan = 2;
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, mediumBorder, thinBorder);
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Right;
            blk.InsertText(((decimal)(sAGEDocument.TotalTVA ?? 0)).ToString("### ##0.00"));
            cell.Blocks.Add(blk);
            //Second table
            cellContainer = rowContainer.Cells.AddTableCell();
            cellContainer.PreferredWidth = 410;
            cellContainer.Padding = new Thickness(0, 5, 0, 0);
            table = cellContainer.Blocks.AddTable();
            table.DefaultCellProperties.Padding = thinPadding;
            //Header row
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(mediumBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 130;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Total HT €");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 130;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText("Total TTC €");
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, mediumBorder, mediumBorder, thinBorder);
            cell.PreferredWidth = 150;
            blk = new();
            blk.TextProperties.Font = arialBold;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText(sAGEDocument.Type == "AVOIR" ? "Montant en notre faveur" : "Montant en votre faveur");
            cell.Blocks.Add(blk);
            //Data row
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(mediumBorder, thinBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 130;
            blk = new();
            blk.TextProperties.Font = arial;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText(((decimal)(sAGEDocument.TotalHT ?? 0)).ToString("### ##0.00"));
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, thinBorder, thinBorder);
            cell.PreferredWidth = 130;
            blk = new();
            blk.TextProperties.Font = arial;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText(((decimal)(sAGEDocument.TotalTTC ?? 0)).ToString("### ##0.00"));
            cell.Blocks.Add(blk);
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(thinBorder, thinBorder, mediumBorder, thinBorder);
            cell.PreferredWidth = 150;
            blk = new();
            blk.TextProperties.Font = arial;
            blk.TextProperties.FontSize = normal.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertText(((decimal)(sAGEDocument.TotalNet ?? 0)).ToString("### ##0.00"));
            cell.Blocks.Add(blk);
            //Text row
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.Borders = new TableCellBorders(mediumBorder, thinBorder, mediumBorder, mediumBorder);
            cell.ColumnSpan = 3;
            blk = new();
            blk.TextProperties.Font = arial;
            blk.TextProperties.FontSize = normal.FontSize;
            if (sAGEDocument.Type == "AVOIR")
            {
                blk.InsertText("Cet avoir sera déduit des prochains montants à vous devoir.");
            }
            else
            {
                blk.InsertText("Mode de règlement : VIREMENT");
                blk.InsertLineBreak();
                blk.InsertText("Conditions : à réception du bon à payer de votre trésorerie.");
            }
            cell.Blocks.Add(blk);
            //----------------------------------------------------
            //Ligne TVA acquitée
            mainRow = mainTable.Rows.AddTableRow();
            mainCell = mainRow.Cells.AddTableCell();
            mainCell.PreferredWidth = 700;
            //----------------------------------------------------
            //Table titre
            table = mainCell.Blocks.AddTable();
            table.BorderCollapse = BorderCollapse.Collapse;
            row = table.Rows.AddTableRow();
            cell = row.Cells.AddTableCell();
            cell.PreferredWidth = 400;
            blk = new Block();
            blk.TextProperties.FontSize = medium.FontSize;
            blk.TextProperties.Font = arialBold;
            blk.InsertLineBreak();
            blk.InsertText("TVA acquitée sur les débits.");
            blk.InsertLineBreak();
            cell.Blocks.Add(blk);
            //----------------------------------------------------
            //footer
            mainRow = mainTable.Rows.AddTableRow();
            mainCell = mainRow.Cells.AddTableCell();
            mainCell.PreferredWidth = 700;
            blk = new Block();
            blk.TextProperties.Font = arial;
            blk.TextProperties.FontSize = footer.FontSize;
            blk.HorizontalAlignment = HorizontalAlignment.Center;
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            blk.InsertLineBreak();
            blk.InsertText("VALORPLAST, SA au capital de 436 760 € - RCS Paris B 390 756 591 - N° TVA : FR 50 390 756 591");
            blk.InsertLineBreak();
            blk.InsertText("21, rue d’Artois - 75008 Paris - Tél. : +33 (0) 1 88 46 10 07 - E-mail : valorplast@valorplast.com");
            blk.InsertLineBreak();
            mainCell.Blocks.Add(blk);
            //----------------------------------------------------
            //Fin
            editor.InsertTable(mainTable);
            //Save to database
            PdfFormatProvider provider = new();
            PdfExportSettings settings = new PdfExportSettings();
            settings.ComplianceLevel = PdfComplianceLevel.PdfA2B;
            provider.ExportSettings = settings;
            provider.Export(document, mS);
            //End
            return mS;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Note de crédit SAGE
        /// </summary>
        public static MemoryStream CreateNoteCreditGemBox(string refDoc, ApplicationDbContext dbContext)
        {
            ComponentInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            MemoryStream mS = new();
            //Get SAGE document
            var sAGEDoc = dbContext.SAGEDocuments
                                .Where(e => e.RefSAGEDocument == refDoc)
                                .FirstOrDefault();
            using (var document = new PdfDocument())
            {
                // Add a page.
                var page = document.Pages.Add();

                // Write a text.
                using (var formattedText = new PdfFormattedText())
                {
                    formattedText.Append("Hello PDF World!");

                    page.Content.DrawText(formattedText, new PdfPoint(100, 700));
                }

                document.Save(mS);
            }
            //End
            return mS;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Incitation qualité - courriers de non versement
        /// </summary>
        public static int CreateIncitationQualiteCourrierNonVersement(int year, ApplicationDbContext dbContext, string rootPath
            , IConfiguration configuration, Context contexteCourant)
        {
            MemoryStream ms = new();
            DataSet ds = new();
            int r = 0;
            //Init
            //Check what documents to create
            SqlCommand cmd = new(); //commande Sql courante
            cmd.Parameters.Add("@year", SqlDbType.Int).Value = year;
            string sqlStr = "select tblEntite.RefEntite"
                + " 	, max(tbmContactAdresse.RefContactAdresse) as RefContactAdresse"
                + " 	, cast(round((cast(sum(case when tblProduit.IncitationQualite=1 and rec.RefCommandeFournisseur is null then PoidsChargement else 0 end) as decimal))/ 1000,3) as decimal(10, 3)) as [PoidsChargement]"
                + " 	, case when sum(case when tblProduit.IncitationQualite = 1 then 1 else 0 end) != 0 then"
                + "         cast(round(cast(count(rec.RefReclamation) as decimal)*100/sum(case when tblProduit.IncitationQualite=1 then 1 else 0 end),2) as decimal(10,2))"
                + " 		else null end as [Taux]"
                + " from tblCommandeFournisseur"
                + "     inner join tblEntite on tblCommandeFournisseur.RefEntite = tblEntite.RefEntite"
                + "     inner join tblAdresse on tblAdresse.RefEntite = tblEntite.refEntite"
                + "     inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                + "     inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresse.RefContactAdresse = tbmContactAdresseContactAdresseProcess.RefContactAdresse"
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
                + "     inner join(select distinct RefEntite from tblContratIncitationQualite where year(DDebut) = @year) as sousContrat on tblEntite.RefEntite = sousContrat.RefEntite"
                + " where case when tblCommandeFournisseur.RefusCamion=1 then year(tblCommandeFournisseur.DMoisDechargementPrevu) else year(tblCommandeFournisseur.DChargement) end = @year"
                + "     and tblProduit.IncitationQualite = 1"
                + "     and tbmContactAdresseContactAdresseProcess.RefContactAdresseProcess = 8"
                + " group by tblEntite.RefEntite"
                + " having count(rec.RefReclamation) * 50 > sum(case when tblProduit.IncitationQualite = 1 then 1 else 0 end)";
            //Load data
            using (SqlConnection sqlConn = new(configuration["Data:DefaultConnection:ConnectionString"]))
            {
                sqlConn.Open();
                cmd.Connection = sqlConn;
                cmd.CommandText = sqlStr;
                SqlDataAdapter adapter = new();
                adapter.SelectCommand = cmd;
                adapter.Fill(ds);
            }
            foreach (DataRow dRow in ds.Tables[0].Rows)
            {
                //Initialisation du fichier PDF
                RadFixedDocument document = new();
                RadFixedDocumentEditor editor = new(document);
                string s;
                //Récupération du contact de l'entite
                //Initialisations
                ContactAdresse cA = dbContext.ContactAdresses
                    .Include(r => r.Adresse)
                    .ThenInclude(r => r.Entite)
                    .Where(el => el.RefContactAdresse == (int)dRow["RefContactAdresse"]).FirstOrDefault();
                //Informations
                document.DocumentInfo.Author = "e-Valorplast";
                document.DocumentInfo.Title = "Courrier incitation autocontrôle";
                editor.SectionProperties.PageSize = new Telerik.Documents.Primitives.Size(793, 1123);
                editor.SectionProperties.PageMargins = new Thickness(47, 63, 48, 60);
                //Style
                Border stdBorder = new();
                Thickness stdPadding = new(10, 10, 10, 10);
                Thickness thinPadding = new(5, 5, 5, 5);
                Thickness noPadding = new(0, 0, 0, 0);
                TextProperties footer = new();
                footer.FontSize = 10;
                TextProperties normal = new();
                normal.FontSize = 14;
                //Font
                FontBase arialBold;
                FontsRepository.TryCreateFont(new Telerik.Documents.Core.Fonts.FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, out arialBold);
                FontBase arial;
                FontsRepository.TryCreateFont(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, out arial);
                FontsRepository.RegisterFont(new FontFamily("GilSansMt"), FontStyles.Normal, FontWeights.Normal, File.ReadAllBytes(rootPath + @"\Assets\Fonts\GIL.TTF"));
                FontBase gilSansMt;
                FontsRepository.TryCreateFont(new FontFamily("GilSansMt"), FontStyles.Normal, FontWeights.Normal, out gilSansMt);
                FontsRepository.RegisterFont(new FontFamily("GilSansMtB"), FontStyles.Normal, FontWeights.Bold, File.ReadAllBytes(rootPath + @"\Assets\Fonts\GILB.TTF"));
                FontBase gilSansMtBold;
                FontsRepository.TryCreateFont(new FontFamily("GilSansMtB"), FontStyles.Normal, FontWeights.Bold, out gilSansMtBold);
                //Edition
                //----------------------------------------------------
                //Table principale
                Table mainTable = new();
                mainTable.BorderCollapse = BorderCollapse.Collapse;
                mainTable.DefaultCellProperties.Padding = noPadding;
                //----------------------------------------------------
                //Ligne table titre
                TableRow mainRow = mainTable.Rows.AddTableRow();
                TableCell mainCell = mainRow.Cells.AddTableCell();
                mainCell.PreferredWidth = 700;
                //----------------------------------------------------
                //Table titre
                Table table = mainCell.Blocks.AddTable();
                table.BorderCollapse = BorderCollapse.Collapse;
                TableRow row = table.Rows.AddTableRow();
                TableCell cell = row.Cells.AddTableCell();
                cell.PreferredWidth = 400;
                using (Stream stream = File.OpenRead(rootPath + @"\Assets\images\logo-expert-recyclage.jpg"))
                {
                    Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource imageSource = new(stream, ImageQuality.High);
                    cell.Blocks.AddBlock().InsertImage(imageSource, 200, (double)imageSource.Height / ((double)imageSource.Width / 200));
                }
                cell = row.Cells.AddTableCell();
                cell.PreferredWidth = 300;
                cell.Padding = noPadding;
                Block blk = new();
                blk.TextProperties.Font = gilSansMt;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertText(cA.Adresse.Entite.Libelle);
                blk.InsertLineBreak();
                blk.InsertText(cA.Adresse.Adr1);
                if (!string.IsNullOrEmpty(cA.Adresse.Adr2))
                {
                    blk.InsertLineBreak();
                    blk.InsertText(cA.Adresse.Adr2);
                }
                blk.InsertLineBreak();
                blk.InsertText(cA.Adresse.CodePostal + " " + cA.Adresse.Ville);
                blk.InsertLineBreak();
                blk.InsertText(cA.Adresse.Pays.Libelle);
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertText("A l'attention de " + cA.Contact.Prenom + " " + cA.Contact.Nom);
                blk.InsertLineBreak();
                blk.InsertText("Paris le " + DateTime.Now.ToString("dd/MM/yyyy"));
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                cell.Blocks.Add(blk);
                //----------------------------------------------------
                //Corps de texte
                mainRow = mainTable.Rows.AddTableRow();
                mainCell = mainRow.Cells.AddTableCell();
                mainCell.PreferredWidth = 700;
                blk = new Block();
                blk.TextProperties.Font = gilSansMt;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertText("Code CITEO : " + cA.Adresse.Entite.CodeEE);
                blk.InsertLineBreak();
                blk.InsertText("Contact : Service Qualité - tél. : +33 (0) 1 88 46 10 08 - e-mail : qualite@valorplast.com");
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                mainCell.Blocks.Add(blk);
                blk = new Block();
                blk.TextProperties.Font = gilSansMtBold;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.InsertText("Objet : contrat d'incitation à la mise en œuvre de la procédure d'autocontrôle");
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                mainCell.Blocks.Add(blk);
                blk = new Block();
                blk.TextProperties.Font = gilSansMt;
                blk.TextProperties.FontSize = normal.FontSize;
                blk.InsertText("Madame, Monsieur,");
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertText("Au cours de l’année " + year.ToString() + " votre Centre de Tri " + cA.Adresse.Entite.Libelle + " nous a livré " + dRow["PoidsChargement"] + " tonnes d’emballages plastiques rigides.");
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertText("Sur l'ensemble de vos livraisons annuelles de flux d'emballages plastiques rigides, le nombre de réclamations représente " + dRow["Taux"] + "%.");
                blk.InsertLineBreak();
                blk.InsertText("Comme prévu dans le contrat d’incitation à la mise en œuvre de la procédure d’autocontrôle qui nous lie, le seuil de 2% de déclenchement de l'intéressement financier a été dépassé.");
                blk.InsertLineBreak();
                blk.InsertText("Aussi, cette année, nous sommes au regret de vous annoncer qu’aucune somme ne vous sera versée.");
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertText("Vous encourageant à poursuivre vos efforts d’autocontrôle qualité, nous vous prions de recevoir nos meilleures salutations.");
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                mainCell.Blocks.Add(blk);
                //----------------------------------------------------
                //Ligne signature
                mainRow = mainTable.Rows.AddTableRow();
                mainCell = mainRow.Cells.AddTableCell();
                mainCell.PreferredWidth = 700;
                //----------------------------------------------------
                //Table titre
                table = mainCell.Blocks.AddTable();
                table.BorderCollapse = BorderCollapse.Collapse;
                row = table.Rows.AddTableRow();
                cell = row.Cells.AddTableCell();
                cell.PreferredWidth = 500;
                cell = row.Cells.AddTableCell();
                cell.PreferredWidth = 200;
                blk = new Block();
                blk.TextProperties.FontSize = normal.FontSize;
                blk.HorizontalAlignment = HorizontalAlignment.Center;
                using (Stream stream = File.OpenRead(rootPath + @"\Assets\images\SignatureKlein.jpg"))
                {
                    Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource imageSource = new(stream, ImageQuality.High);
                    blk.InsertImage(imageSource, 100, imageSource.Height / (imageSource.Width / 100));
                }
                blk.TextProperties.Font = gilSansMt;
                blk.InsertLineBreak();
                blk.InsertText("Catherine KLEIN");
                blk.InsertLineBreak();
                blk.InsertText("Directrice Générale");
                blk.InsertLineBreak();
                cell.Blocks.Add(blk);
                //----------------------------------------------------
                //footer
                mainRow = mainTable.Rows.AddTableRow();
                mainCell = mainRow.Cells.AddTableCell();
                mainCell.PreferredWidth = 700;
                blk = new Block();
                blk.TextProperties.Font = gilSansMt;
                blk.TextProperties.FontSize = footer.FontSize;
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertLineBreak();
                blk.InsertText("VALORPLAST, SA au capital de 436 760 € - RCS Paris B 390 756 591 - N° TVA : FR 50 390 756 591. 21, rue d’Artois - 75008 Paris");
                blk.InsertLineBreak();
                blk.InsertText("Tél. : +33 (0) 1 88 46 10 07 - E-mail : valorplast@valorplast.com");
                blk.InsertLineBreak();
                mainCell.Blocks.Add(blk);
                //----------------------------------------------------
                //Fin
                editor.InsertTable(mainTable);
                //Save to database
                PdfFormatProvider provider = new();
                ms = new MemoryStream();
                provider.Export(document, ms);
                ms.Position = 0;
                ////HDD file
                //using (FileStream file = new FileStream(@"c:\tmp\Courrier" + r.ToString() + ".pdf", FileMode.Create, FileAccess.Write))
                //    ms.CopyTo(file);

                //Database file
                var doc = new Document()
                {
                    Libelle = "Incitation autocontrôle " + year + " (" + cA.Adresse.Entite.CodeEE.ToString() + ")",
                    RefUtilisateurCourant = contexteCourant.RefUtilisateur,
                    Corps = ms.ToArray(),
                    Nom = "CourrierDeNonVersement" + year + ".pdf",
                    VisibiliteTotale = true
                };
                dbContext.Documents.Add(doc);
                doc.DocumentEntites = new HashSet<DocumentEntite>();
                doc.DocumentEntites.Add(new DocumentEntite() { RefEntite = cA.Adresse.RefEntite });
                if (dbContext.Documents.Where(q => q.Libelle == doc.Libelle && q.RefDocument != doc.RefDocument).Count() == 0)
                {
                    r++;
                    dbContext.SaveChanges();
                }
                else { dbContext.Documents.Remove(doc); }
            }
            //Fin
            return r;
        }
    }
}