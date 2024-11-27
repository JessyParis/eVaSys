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
    public class CVQDescriptionCVQViewModel
    {
        public CVQDescriptionCVQViewModel()
        {
        }
        public int RefCVQDescriptionCVQ { get; set; }
        public int RefCVQ;
        public DescriptionCVQViewModel DescriptionCVQ { get; set; }
        public int Nb { get; set; }
        public int Ordre { get; set; }
        public int LimiteBasse { get; set; }
        public int LimiteHaute { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public string LibelleCultured { get; set; }
    }
}