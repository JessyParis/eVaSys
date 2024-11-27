/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/09/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class SAGEPeriodiciteViewModel
    {
        #region Constructor
        public SAGEPeriodiciteViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefSAGEPeriodicite { get; set; }
        public string Libelle { get; set; }
        #endregion Properties
    }
}
