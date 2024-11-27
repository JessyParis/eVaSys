/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 19/08/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ResetPasswordViewModel
    {
        #region Constructor
        public ResetPasswordViewModel()
        {

        }
        #endregion

        #region Properties
        public string cultureName { get; set; }
        public bool error { get; set; }
        #endregion
    }
}
