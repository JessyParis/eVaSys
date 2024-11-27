/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/09/2023
/// ----------------------------------------------------------------------------------------------------- 


namespace eVaSys.ViewModels
{
    /// <summary>
    /// Class for SAGEDocumentLigne
    /// </summary>
    public class SAGEDocumentLigneViewModel
    {
        #region Constructor
        public SAGEDocumentLigneViewModel()
        {
        }
        #endregion
        public int ID { get; }
        public int RefSAGEDocument { get; }
        public int RefSAGEDocumentLigne { get; }
        public decimal Quantite { get; }
        public string LibelleDL { get; }
        public decimal PUHT { get; }
        public decimal TotalHT { get; }
        public decimal TotalTTC { get; }
        public decimal TVAMontant { get; }
        public decimal TVATaux { get; }
        public SAGEDocumentArticleViewModel SAGEDocumentArticle { get; }
    }
}