/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/09/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class SecuriteViewModel
    {
        #region Constructor
        public SecuriteViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefSecurite { get; set; }
        public int DelaiAvantDesactivationUtilisateur { get; set; }
        public int DelaiAvantChangementMotDePasse { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
