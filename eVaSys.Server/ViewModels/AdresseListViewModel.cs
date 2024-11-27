/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/12/2018
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class AdresseListViewModel
    {
        #region Constructor
        public AdresseListViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefAdresse { get; set; }
        public int RefEntite { get; set; }
        public string Libelle { get; set; }
        public string TexteAdresseList { get; set; }
        #endregion
    }
}
