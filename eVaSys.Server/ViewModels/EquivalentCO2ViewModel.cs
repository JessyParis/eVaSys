/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 26/10/2023
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EquivalentCO2ViewModel
    {
        #region Constructor
        public EquivalentCO2ViewModel()
        {
        }
        #endregion
        public int RefEquivalentCO2 { get; set; }
        public string Libelle { get; set; }
        public int Ordre { get; set; }
        public decimal? Ratio { get; set; }
        public string IconeBase64 { get; set; }
        public bool Actif { get; set; } = false;
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}