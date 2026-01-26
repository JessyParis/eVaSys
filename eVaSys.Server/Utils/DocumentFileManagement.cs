/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création :03/12/2025
/// ----------------------------------------------------------------------------------------------------- 

using eVaSys.Data;
using GemBox.Document;
using GemBox.Document.Drawing;
using GemBox.Pdf.Content;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace eVaSys.Utils
{
    /// <summary>
    /// Documents in different format file management 
    /// </summary>
    public static class DocumentFileManagement
    {
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Document collectivité élu (Ref=1)
        /// </summary>
        public static MemoryStream CreateDocumentType1(int refEntite, int year, ApplicationDbContext dbContext, string rootPath
        )
        {
            MemoryStream mS = new();
            ComponentInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            //Get data
            Entite coll = dbContext.Entites.FirstOrDefault(e => e.RefEntite == refEntite);
            if (coll != null)
            {
                //Process
                DocumentModel document = new DocumentModel();
                //Init document information
                document.DocumentProperties.BuiltIn[BuiltInDocumentProperty.Author] = "Valorplast";
                document.DocumentProperties.BuiltIn[BuiltInDocumentProperty.Comments] = "Edité par Enviromatic";
                //Initialize content
                //Logo
                Section section = new Section(document);
                document.Sections.Add(section);
                Paragraph paragraph = new Paragraph(document);
                section.Blocks.Add(paragraph);
                var picture = new Picture(document, rootPath + @"\Assets\images\logo-horiz.png");
                paragraph.Inlines.Add(picture);
                // Create a rectangle shape with a size of 100x100 pixels and a red background.
                var rectangle = new Shape(document, ShapeType.Rectangle, Layout.Floating(
                        new HorizontalPosition( 50, LengthUnit.Pixel, HorizontalPositionAnchor.Margin), // 50 pixels from the left of the page
                        new VerticalPosition(50, LengthUnit.Pixel, VerticalPositionAnchor.Margin),    // 50 pixels from the top of the page
                        new Size(100, 100)));
                rectangle.Fill.SetSolid(Color.Red);
                // Add the rectangle to the paragraph.
                paragraph.Inlines.Add(rectangle);
                Run run = new Run(document, "Reporting collectivité élu");
                paragraph.Inlines.Add(run);
                document.Save(mS, SaveOptions.PdfDefault);
            }
            //End 
            return mS;
        }
    }
}