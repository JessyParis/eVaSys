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
    public class MessageDiffusionViewModel
    {
        #region Constructor
        public MessageDiffusionViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefMessageDiffusion { get; set; }
        public int RefMessage { get; set; }
        public string RefModule { get; set; }
        public string RefHabilitation { get; set; }
        public UtilisateurViewModel Utilisateur { get; set; }
        #endregion Properties
    }
}
