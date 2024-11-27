/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 13/03/2019
/// ----------------------------------------------------------------------------------------------------- 

using System.Collections.Generic;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for SAGEModeReglement
    /// </summary>
    public class SAGEModeReglement
    {
        #region Constructor
        public SAGEModeReglement()
        {
        }
        #endregion
        public int RefSAGEModeReglement { get; set; }
        public string Libelle { get; set; }
        public ICollection<Entite> Entites { get; set; }
    }
}