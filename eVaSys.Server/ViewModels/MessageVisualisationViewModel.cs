/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/11/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class MessageVisualisationViewModel
    {
        #region Constructor
        public MessageVisualisationViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefMessageVisualisation { get; set; }
        public int RefMessage { get; set; }
        public UtilisateurListViewModel Utilisateur { get; set; }
        public DateTime D { get; set; }
        public bool? Confirme { get; set; }
        #endregion Properties
    }
}
