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
    /// Class for SAGEPeriodicite
    /// </summary>
    public class SAGEPeriodicite
    {
        #region Constructor
        public SAGEPeriodicite()
        {
        }
        #endregion
        public int RefSAGEPeriodicite { get; set; }
        public string Libelle { get; set; }
        public ICollection<Entite> Entites { get; set; }
    }
}