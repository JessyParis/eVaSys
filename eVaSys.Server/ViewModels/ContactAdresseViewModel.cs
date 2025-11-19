/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 01/05/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ContactAdresseViewModel
    {
        #region Constructor
        public ContactAdresseViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefContactAdresse { get; set; }
        public int RefContact { get; set; }
        public ContactViewModel Contact { get; set; }
        public int RefEntite { get; set; }
        public int? RefAdresse { get; set; }
        public TitreViewModel Titre { get; set; }
        public string Tel { get; set; }
        public string TelMobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Cmt { get; set; }
        public string CmtServiceFonction { get; set; }
        public bool? Actif { get; set; }
        public ICollection<ContactAdresseContactAdresseProcessViewModel> ContactAdresseContactAdresseProcesss { get; set; }
        public ICollection<ContactAdresseDocumentTypeViewModel> ContactAdresseDocumentTypes { get; set; }
        public ICollection<ContactAdresseServiceFonctionViewModel> ContactAdresseServiceFonctions { get; set; }
        public ICollection<UtilisateurListViewModel> Utilisateurs { get; set; }
        public string ListText { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
