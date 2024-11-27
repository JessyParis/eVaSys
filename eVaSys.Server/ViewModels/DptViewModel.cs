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
    public class DptViewModel
    {
        #region Constructor
        public DptViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public string RefDpt { get; set; }
        public string Libelle { get; set; }
        #endregion
    }
}
