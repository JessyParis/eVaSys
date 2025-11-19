/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/12/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DocumentEntiteViewModel
    {
        #region Constructor
        public DocumentEntiteViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefDocumentEntite { get; set; }
        public int RefDocument { get; set; }
        public int RefEntite { get; set; }
        public EntiteListViewModel Entite { get; set; }
        public DocumentNoFileViewModel DocumentNoFile { get; set; }
        #endregion Properties
    }
}
