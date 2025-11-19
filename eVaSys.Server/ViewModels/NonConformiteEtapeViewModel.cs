/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 29/06/2020
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class NonConformiteEtapeViewModel
    {
        #region Constructor
        public NonConformiteEtapeViewModel()
        {
        }
        #endregion
        public int? RefNonConformiteEtape { get; set; }
        public int? RefNonConformite { get; set; }
        public NonConformiteEtapeTypeViewModel NonConformiteEtapeType { get; set; }
        public int Ordre { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public string Libelle { get; set; }
        public string Cmt { get; set; }
        public DateTime? DCreation { get; set; }
        public UtilisateurViewModel UtilisateurCreation { get; set; }
        public DateTime? DModif { get; set; }
        public UtilisateurViewModel UtilisateurModif { get; set; }
        public DateTime? DControle { get; set; }
        public UtilisateurViewModel UtilisateurControle { get; set; }
        public DateTime? DValide { get; set; }
        public UtilisateurViewModel UtilisateurValide { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        public string ValidationText { get; set; }
        public string ControleText { get; set; }
    }
}