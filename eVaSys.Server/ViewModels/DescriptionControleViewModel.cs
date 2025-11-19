/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/03/2020
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DescriptionControleViewModel
    {
        public DescriptionControleViewModel()
        {
        }
        public int RefDescriptionControle { get; set; }
        public int Ordre { get; set; }
        public bool? Actif { get; set; }
        public bool? CalculLimiteConformite { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public ICollection<DescriptionControleProduitViewModel> DescriptionControleProduits { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}