/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/01/2026
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for DocumentType
    /// </summary>
    public class DocumentTypeMeta
    {
        #region Constructor

        public DocumentTypeMeta()
        {
        }

        #endregion

        public int RefDocumentTypeMeta { get; set; }
        public int RefDocumentType { get; set; }
        public string Libelle { get; set; }
        public string Contenu { get; set; }
    }
}