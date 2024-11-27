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
    public class ContactAdresseServiceFonctionViewModel
    {
        #region Constructor
        public ContactAdresseServiceFonctionViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefContactAdresseServiceFonction { get; set; }
        public int RefContactAdresse { get; set; }
        public FonctionViewModel Fonction { get; set; }
        public ServiceViewModel Service { get; set; }
        #endregion Properties
    }
}
