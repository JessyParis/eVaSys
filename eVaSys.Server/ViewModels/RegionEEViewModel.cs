/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/10/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class RegionEEViewModel
    {
        #region Constructor
        public RegionEEViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefRegionEE { get; set; }
        public string Libelle { get; set; }
        public ICollection<RegionEEDptViewModel> RegionEEDpts { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
