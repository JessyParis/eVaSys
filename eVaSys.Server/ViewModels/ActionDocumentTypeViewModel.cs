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
    public class ActionDocumentTypeViewModel
    {
        #region Constructor
        public ActionDocumentTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefActionDocumentType { get; set; }
        public int RefAction { get; set; }
        public int RefDocumentType { get; set; }
        public DocumentTypeViewModel DocumentType { get; set; }
        #endregion Properties
    }
}
