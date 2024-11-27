/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ClientApplicationApplicationViewModel
    {
        #region Constructor
        public ClientApplicationApplicationViewModel()
        {
        }
        #endregion Constructor

        #region Properties
        public int RefClientApplicationApplication { get; set; }
        public int RefClientApplication { get; set; }
        public ApplicationViewModel Application { get; set; }
        public decimal Ratio { get; set; }
        #endregion Properties
    }
}
