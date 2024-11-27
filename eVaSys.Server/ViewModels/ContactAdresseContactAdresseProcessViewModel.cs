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
    public class ContactAdresseContactAdresseProcessViewModel
    {
        #region Constructor
        public ContactAdresseContactAdresseProcessViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefContactAdresseContactAdresseProcess { get; set; }
        public int RefContactAdresse { get; set; }
        public int RefContactAdresseProcess { get; set; }
        public ContactAdresseProcessViewModel ContactAdresseProcess { get; set; }
        #endregion Properties
    }
}
