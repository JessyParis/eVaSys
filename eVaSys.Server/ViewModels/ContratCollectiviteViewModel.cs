/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/09/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ContratCollectiviteViewModel
    {
        public ContratCollectiviteViewModel()
        {

        }
        public int RefContratCollectivite { get; set; }
        public DateTime? DDebut { get; set; }
        public DateTime? DFin { get; set; }
        public int RefEntite { get; set; }
        public bool Avenant { get; set; }
        public string Cmt { get; set; }
    }
}
