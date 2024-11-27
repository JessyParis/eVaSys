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
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DescriptionCVQViewModel
    {
        public DescriptionCVQViewModel()
        {
        }
        public int RefDescriptionCVQ { get; set; }
        public int Ordre { get; set; }
        public int LimiteBasse { get; set; }
        public int LimiteHaute { get; set; }
        public bool? Actif { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public ICollection<DescriptionCVQProduitViewModel> DescriptionCVQProduits { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}