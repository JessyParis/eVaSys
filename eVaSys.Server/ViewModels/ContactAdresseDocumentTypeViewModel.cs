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
    public class ContactAdresseDocumentTypeViewModel
    {
        #region Constructor
        public ContactAdresseDocumentTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefContactAdresseDocumentType { get; set; }
        public int RefContactAdresse { get; set; }
        public int RefDocumentType { get; set; }
        public DocumentTypeViewModel DocumentTypeViewModel { get; set; }
        public int Nb { get; set; }
        #endregion Properties
    }
}
