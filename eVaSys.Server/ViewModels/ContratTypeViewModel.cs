/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 18/12/2024
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ContratTypeViewModel
    {
        #region Constructor
        public ContratTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefContratType { get; set; }
        public string Libelle { get; set; }
        #endregion Properties
    }
}
