/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/19/2023
/// ----------------------------------------------------------------------------------------------------- 

namespace eVaSys.Data
{
    /// <summary>
    /// Class for SAGEDocumentArticle
    /// </summary>
    public class SAGEDocumentArticle
    {
        #region Constructor
        public SAGEDocumentArticle()
        {
        }
        #endregion
        public int ID { get; }
        public string RefSAGEDocumentArticle { get; }
        public string LibelleAR { get; }
        public string Designation1 { get; }
        public string Designation2 { get; }
        public ICollection<SAGEDocumentLigne> SAGEDocumentLignes { get; set; }
    }
}