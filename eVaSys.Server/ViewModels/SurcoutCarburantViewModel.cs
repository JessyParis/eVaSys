/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/10/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class SurcoutCarburantViewModel
    {
        #region Constructor
        public SurcoutCarburantViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int? RefSurcoutCarburant { get; set; }
        public EntiteListViewModel Transporteur { get; set; }
        public PaysViewModel Pays { get; set; }
        public int? Annee { get; set; }
        public int? Mois { get; set; }
        public decimal? Ratio { get; set; }
        public bool AllPays { get; set;}
        public string CreationText { get; set;}
        public string ModificationText { get; set; }

        #endregion
    }
}
