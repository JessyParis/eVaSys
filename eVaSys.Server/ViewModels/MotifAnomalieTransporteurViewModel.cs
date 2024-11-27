/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/05/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class MotifAnomalieTransporteurViewModel
    {
        #region Constructor
        public MotifAnomalieTransporteurViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefMotifAnomalieTransporteur { get; set; }
        public string Libelle { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
