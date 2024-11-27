
using System.Collections.Generic;
/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/19/2023
/// ----------------------------------------------------------------------------------------------------- 
namespace eVaSys.ViewModels
{
    /// <summary>
    /// Class for SAGEDocumentArticle
    /// </summary>
    public class SAGEDocumentArticleViewModel
    {
        #region Constructor
        public SAGEDocumentArticleViewModel()
        {
        }
        #endregion
        public int ID { get; }
        public int RefSAGEDocumentArticle { get; }
        public string LibelleAR { get; }
        public string Designation1 { get; }
        public string Designation2 { get; }
        public ICollection<SAGEDocumentLigneViewModel> SAGEDocumentLignes { get; set; }
    }
}