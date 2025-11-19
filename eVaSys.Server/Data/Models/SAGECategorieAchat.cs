/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 13/03/2019
/// ----------------------------------------------------------------------------------------------------- 

namespace eVaSys.Data
{
    /// <summary>
    /// Class for SAGEcategorieAchat
    /// </summary>
    public class SAGECategorieAchat
    {
        #region Constructor
        public SAGECategorieAchat()
        {
        }
        #endregion
        public int RefSAGECategorieAchat { get; set; }
        public string Libelle { get; set; }
        public decimal TVATaux { get; set; }
        public ICollection<Entite> Entites { get; set; }
    }
}