using Newtonsoft.Json;
/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 25/11/2019
/// ----------------------------------------------------------------------------------------------------- 

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class UtilisateurListViewModel
    {
        #region Constructor
        public UtilisateurListViewModel()
        {

        }
        #endregion Constructor
        #region Properties
        public int RefUtilisateur { get; set; }
        public int? RefUtilisateurMaitre { get; set; }
        public string Login { get; set; }
        public string Nom { get; set; }
        public string Tel { get; set; }
        public string TelMobile { get; set; }
        public string EMail { get; set; }
        public bool? Actif { get; set; }
        public string UtilisateurType { get; set; }
        public string EntiteLibelle { get; set; }
        public bool HasProfils { get; set; }
        #endregion Properties
    }
}
