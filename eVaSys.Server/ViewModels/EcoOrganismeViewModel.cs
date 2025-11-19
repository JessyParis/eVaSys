/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 15/01/2024
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EcoOrganismeViewModel
    {
        #region Constructor
        public EcoOrganismeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEcoOrganisme { get; set; }
        public string Libelle { get; set; }
        public bool? EcoOrganismeContrat { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
