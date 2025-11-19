/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/12/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DocumentNoFileViewModel
    {
        #region Constructor
        public DocumentNoFileViewModel()
        {
        }
        #endregion
        public int RefDocument { get; set; }
        public string Libelle { get; set; }
        public string Description { get; set; }
        public DateTime? DDebut { get; set; }
        public DateTime? DFin { get; set; }
        public bool? Actif { get; set; }
        public bool VisibiliteTotale { get; set; }
        public string Nom { get; set; }
        public DateTime DCreation { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}