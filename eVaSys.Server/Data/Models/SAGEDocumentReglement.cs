/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/12/2023
/// ----------------------------------------------------------------------------------------------------- 


namespace eVaSys.Data
{
    /// <summary>
    /// Class for SAGEDocumentReglement
    /// </summary>
    public class SAGEDocumentReglement
    {
        #region Constructor
        public SAGEDocumentReglement()
        {
        }
        #endregion
        public string RefSAGEDocument { get; }
        public string Lettrage { get; }
        public int Nb { get; }
    }
}