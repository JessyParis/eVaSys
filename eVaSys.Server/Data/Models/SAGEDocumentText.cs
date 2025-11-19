/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 19/03/2024
/// ----------------------------------------------------------------------------------------------------- 

namespace eVaSys.Data
{
    /// <summary>
    /// Class for SAGEDocumentLigneText
    /// </summary>
    public class SAGEDocumentLigneText
    {
        #region Constructor
        public SAGEDocumentLigneText()
        {
        }
        #endregion
        public int ID { get; }
        public int RefLigne { get; }
        public string Cmt { get; }
        public ICollection<SAGEDocumentLigne> SAGEDocumentLignes { get; set; }
    }
}