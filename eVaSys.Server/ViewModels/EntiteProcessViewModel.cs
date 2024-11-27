/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 15/11/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EntiteProcessViewModel
    {
        #region Constructor
        public EntiteProcessViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEntiteProcess { get; set; }
        public int RefEntite { get; set; }
        public ProcessViewModel Process { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
