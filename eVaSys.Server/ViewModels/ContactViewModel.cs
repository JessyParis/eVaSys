/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 01/05/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ContactViewModel
    {
        #region Constructor
        public ContactViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefContact { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public int? RefCivilite { get; set; }
        public CiviliteViewModel Civilite { get; set; }
        #endregion Properties
    }
}
