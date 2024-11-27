/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/06/2020
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class NonConformiteAccordFournisseurTypeViewModel
    {
        #region Constructor
        public NonConformiteAccordFournisseurTypeViewModel()
        {
        }
        #endregion
        public int RefNonConformiteAccordFournisseurType { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public string Libelle { get; set; }
        public bool Actif { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}