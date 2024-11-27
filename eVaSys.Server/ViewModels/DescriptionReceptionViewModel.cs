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
    public class DescriptionReceptionViewModel
    {
        public DescriptionReceptionViewModel()
        {
        }
        public int RefDescriptionReception { get; set; }
        public bool? Actif { get; set; }
        public int Ordre { get; set; }
        public string LibelleFRFR { get; set; }
        public string OuiFRFR { get; set; }
        public string NonFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public string OuiENGB { get; set; }
        public string NonENGB { get; set; }
        public string LibelleCultured { get; set; }
        public string OuiCultured { get; set; }
        public string NonCultured { get; set; }
        public bool? ReponseParDefaut { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}