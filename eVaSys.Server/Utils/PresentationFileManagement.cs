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
using GemBox.Presentation;
using GemBox.Pdf.Content;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace eVaSys.Utils
{
    /// <summary>
    /// Presentation (PowerPoint) file management 
    /// </summary>
    public static class PresentationFileManagement
    {
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Document collectivité grand public (Ref=2)
        /// </summary>
        public static MemoryStream CreateDocumentType2(int refEntite, ApplicationDbContext dbContext)
        {
            MemoryStream mS = new();
            ComponentInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            //Init
            Entite coll = dbContext.Entites.FirstOrDefault(e => e.RefEntite == refEntite);
            if (coll != null)
            {
                //Process
                var presentation = new PresentationDocument();
                var slide = presentation.Slides.AddNew(SlideLayoutType.Custom);
                var textBox = slide.Content.AddTextBox(ShapeGeometryType.Rectangle, 2, 2, 5, 4, LengthUnit.Centimeter);
                var paragraph = textBox.AddParagraph();

                paragraph.AddRun("Reporting collectivité grand public");
                // Save presentation
                presentation.Save(mS, SaveOptions.Pptx);
            }
            //End 
            return mS;
        }
    }
}