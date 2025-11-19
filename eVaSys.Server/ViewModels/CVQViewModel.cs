/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/04/2020
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CVQViewModel
    {
        public CVQViewModel()
        {
        }
        public int RefCVQ { get; set; }
        public int? RefFicheControle { get; set; }
        public ContactAdresseViewModel Controleur { get; set; }
        public DateTime? DCVQ { get; set; }
        public string Etiquette { get; set; }
        public string Cmt { get; set; }
        public ICollection<CVQDescriptionCVQViewModel> CVQDescriptionCVQs { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}