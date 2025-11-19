/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/09/2023
/// ----------------------------------------------------------------------------------------------------- 

namespace eVaSys.Data
{
    /// <summary>
    /// Class for SAGEDocumentCompteTVA
    /// </summary>
    public class SAGEDocumentCompteTVA
    {
        #region Constructor
        public SAGEDocumentCompteTVA()
        {
        }
        #endregion
        public string RefSAGEDocumentCompteTVA { get; }
        public string CompteTVA { get; }
        public ICollection<SAGEDocument> SAGEDocuments { get; }
    }
}