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
    /// Class for SAGEConditionLivraison
    /// </summary>
    public class SAGEConditionLivraison
    {
        #region Constructor
        public SAGEConditionLivraison()
        {
        }
        #endregion
        public int RefSAGEConditionLivraison { get; set; }
        public string Libelle { get; set; }
        public ICollection<Entite> Entites { get; set; }
    }
}