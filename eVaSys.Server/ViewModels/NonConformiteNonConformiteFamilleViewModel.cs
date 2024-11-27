/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 19/07/2020
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class NonConformiteNonConformiteFamilleViewModel
    {
        #region Constructor
        public NonConformiteNonConformiteFamilleViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefNonConformiteNonConformiteFamille { get; set; }
        public int RefNonConformite { get; set; }
        public NonConformiteFamilleViewModel NonConformiteFamille { get; set; }
        #endregion Properties
    }
}
