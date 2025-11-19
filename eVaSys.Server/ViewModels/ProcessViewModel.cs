/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/09/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ProcessViewModel
    {
        public ProcessViewModel()
        {
        }
        public int RefProcess { get; set; }
        public string Libelle { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}