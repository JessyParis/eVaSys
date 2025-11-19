/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 02/05/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ActionViewModel
    {
        #region Constructor
        public ActionViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefAction { get; set; }
        public int RefEntite { get; set; }
        public FormeContactViewModel FormeContact { get; set; }
        public int? RefContactAdresse { get; set; }
        public ContactAdresseViewModel ContactAdresse { get; set; }
        public int? RefEnvoiDocument { get; set; }
        public string Libelle { get; set; }
        public DateTime? DAction { get; set; }
        public string Email { get; set; }
        public string Cmt { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public ICollection<ActionActionTypeViewModel> ActionActionTypes { get; set; }
        public ICollection<ActionFichierNoFileViewModel> ActionFichierNoFiles { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
