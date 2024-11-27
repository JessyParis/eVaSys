/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/01/2024
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ParametreViewModel
    {
        #region Constructor
        public ParametreViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int? RefParametre { get; set; }
        public string Libelle { get; set; }
        public bool Serveur { get; set; }
        public decimal? ValeurNumerique { get; set; }
        public string ValeurTexte { get; set; }
        public string ValeurHTML { get; set; }
        public string ValeurBinaireBase64 { get; set; }
        public string Cmt { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
