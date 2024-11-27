/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/10/2018
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EntiteListViewModel
    {
        #region Constructor
        public EntiteListViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEntite { get; set; }
        public string Libelle { get; set; }
        public bool? Actif { get; set; }
        #endregion
    }
}
