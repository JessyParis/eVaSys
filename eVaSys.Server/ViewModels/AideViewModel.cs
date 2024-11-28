/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/10/2024
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class AideViewModel
    {
        #region Constructor
        public AideViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int? RefAide { get; set; }
        public string Composant { get; set; }
        public string ValeurHTML { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        public int RefRessource { get; set; }
        #endregion Properties
    }
}
