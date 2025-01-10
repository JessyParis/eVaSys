/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/01/2025
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ContratEntiteViewModel
    {
        #region Constructor
        public ContratEntiteViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefContratEntite { get; set; }
        public int RefContrat { get; set; }
        public int RefEntite { get; set; }
        public EntiteListViewModel Entite { get; set; }
        #endregion Properties
    }
}
