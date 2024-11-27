/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 02/05/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ActionActionTypeViewModel
    {
        #region Constructor
        public ActionActionTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefActionActionType { get; set; }
        public int RefAction { get; set; }
        public int RefActionType { get; set; }
        public ActionTypeViewModel ActionType { get; set; }
        #endregion Properties
    }
}
