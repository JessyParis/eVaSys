/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 19/10/2023
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class RessourceViewModel
    {
        #region Constructor
        public RessourceViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefRessource { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        #endregion Properties
    }
}
