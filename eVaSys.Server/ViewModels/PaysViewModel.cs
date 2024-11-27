/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 02/10/2018
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class PaysViewModel
    {
        #region Constructor
        public PaysViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefPays { get; set; }
        public bool? Actif { get; set; }
        public int? RefRegionReporting {get;set; }
        public string Libelle { get; set; }
        public string LibelleCourt { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
