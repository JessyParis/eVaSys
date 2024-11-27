/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 04/04/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DashboardItemViewModel
    {
        #region Constructor
        public DashboardItemViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public string nb { get; set; }
        public string nbUrgent { get; set; }
        #endregion
    }
}
