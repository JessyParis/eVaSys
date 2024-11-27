/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 24/11/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EntiteDRViewModel
    {
        #region Constructor
        public EntiteDRViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEntiteDR { get; set; }
        public UtilisateurListViewModel DR { get; set; }
        #endregion Properties
    }
}
