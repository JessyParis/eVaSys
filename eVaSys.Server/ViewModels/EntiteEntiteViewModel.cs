/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 15/10/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EntiteEntiteViewModel
    {
        #region Constructor
        public EntiteEntiteViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEntiteEntite { get; set; }
        public int RefEntite { get; set; }
        public int RefEntiteRtt { get; set; }
        public bool Actif { get; set; } = true;
        public string Cmt { get; set; }
        public string LibelleUtilisateurCreation { get; set; }
        public DateTime? DCreation { get; set; }
        public string CreationText { get; set; }
        public string LibelleEntiteRtt { get; set; }
        public bool? ActifEntiteRtt { get; set; }
        public int RefEntiteRttType { get; set; }
        #endregion Properties
    }
}
