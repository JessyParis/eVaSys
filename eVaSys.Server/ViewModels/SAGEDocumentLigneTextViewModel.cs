

using eVaSys.Data;



using System.Collections.Generic;

/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 19/03/2024
/// ----------------------------------------------------------------------------------------------------- 


namespace eVaSys.ViewModels
{
    /// <summary>
    /// Class for SAGEDocumentLigne
    /// </summary>
    public class SAGEDocumentLigneTextViewModel
    {
        #region Constructor
        public SAGEDocumentLigneTextViewModel()
        {
        }
        #endregion
        public int ID { get; }
        public int RefLigne { get; }
        public string Cmt { get; }
        public ICollection<SAGEDocumentLigne> SAGEDocumentLignes { get; set; }
    }
}