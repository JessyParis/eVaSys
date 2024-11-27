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
    public class ActionTypeEntiteTypeViewModel
    {
        #region Constructor
        public ActionTypeEntiteTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefActionTypeEntiteType { get; set; }
        public int RefActionType { get; set; }
        public EntiteTypeViewModel EntiteType { get; set; }
        #endregion Properties
    }
}
