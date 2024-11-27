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
    public class RegionEEDptViewModel
    {
        #region Constructor
        public RegionEEDptViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefRegionEEDpt { get; set; }
        public int RefRegionEE { get; set; }
        public DptViewModel Dpt { get; set; }
        #endregion Properties
    }
}
