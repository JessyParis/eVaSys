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
    /// Class for SAGEDocument
    /// </summary>
    public class SAGEDocumentViewModel
    {
        #region Constructor
        public SAGEDocumentViewModel()
        {
        }
        #endregion
        public int ID { get; }
        public string RefSAGEDocument { get; }
        public DateTime D { get; }
        public string CodeComptable { get; }
        public string ConditionLivraison { get; }
        public double TotalHTBdd { get; }
        public double TotalTTCBdd { get; }
        public double TotalNetBdd { get; }
        public double TotalTVABdd { get; }
        public string Adr1 { get; }
        public string Adr2 { get; }
        public string Adr3 { get; }
        public string Adr4 { get; }
        public SAGEDocumentCompteTVAViewModel SAGEDocumentCompteTVA { get; }
        public string Type { get; }
        public bool Regle { get; }
        public ICollection<SAGEDocumentLigneViewModel> SAGEDocumentLignes { get; }
    }
}