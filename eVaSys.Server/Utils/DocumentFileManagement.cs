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
        public static MemoryStream CreateDocumentType1(int refEntite, ApplicationDbContext dbContext)
        {
            MemoryStream mS = new();
            ComponentInfo.SetLicense("BN-2025Aug04-ndN0CIiJY6CjOq50Ccj5vDpb8mtqTyUhhBvk5KsARIhO4hPXfuRpBMMkLyNmgaUwSqkS51ZY0o4Fe/w06lNyYJbhEqA==A");
            //Init
            Entite coll = dbContext.Entites.FirstOrDefault(e => e.RefEntite == refEntite);
            if (coll != null)
            {
                //Process
                DocumentModel document = new DocumentModel();

                Section section = new Section(document);
                document.Sections.Add(section);

                Paragraph paragraph = new Paragraph(document);
                section.Blocks.Add(paragraph);

                Run run = new Run(document, "Reporting collectivité élu");
                paragraph.Inlines.Add(run);
                document.Save(mS, SaveOptions.PdfDefault);
            }
            //End 
            return mS;
        }
    }
}