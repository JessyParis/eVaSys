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
    public class ControleDescriptionControleViewModel
    {
        public ControleDescriptionControleViewModel()
        {
        }
        public int RefControleDescriptionControle { get; set; }
        public int RefControle { get; set; }
        public DescriptionControleViewModel DescriptionControle;
        public int Ordre { get; set; }
        public double Poids { get; set; }
        public bool? CalculLimiteConformite { get; set; }
        public string Cmt { get; set; }
        public bool? CmtObligatoire { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public string LibelleCultured { get; set; }
    }
}