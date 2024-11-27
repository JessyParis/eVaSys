/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 02/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ParamEmailListViewModel
    {
        #region Constructor
        public ParamEmailListViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefParamEmail { get; set; }
        public string EmailExpediteur { get; set; }
        public string LibelleExpediteur { get; set; }
        #endregion Properties
    }
}
