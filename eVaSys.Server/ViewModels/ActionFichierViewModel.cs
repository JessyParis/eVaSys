/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/12/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ActionFichierViewModel
    {
        #region Constructor
        public ActionFichierViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefActionFichier { get; set; }
        public int RefAction { get; set; }
        public byte[] Corps { get; set; }
        public string CorpsBase64 { get; set; }
        public string Nom { get; set; }
        public string Extension { get; set; }
        #endregion Properties
    }
}
