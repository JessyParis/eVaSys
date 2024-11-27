/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/09/2018
/// ----------------------------------------------------------------------------------------------------- 
using System.Collections.Generic;
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class AdresseViewModel
    {
        #region Constructor
        public AdresseViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefAdresse { get; set; }
        public int RefAdresseType { get; set; }
        public AdresseTypeViewModel AdresseType { get; set; }
        public int RefEntite { get; set; }
        public EntiteViewModel Entite { get; set; }
        public string Libelle { get; set; }
        public string Adr1 { get; set; }
        public string Adr2 { get; set; }
        public string CodePostal { get; set; }
        public RegionEEViewModel RegionEE { get; set; }
        public string Ville { get; set; }
        public int RefPays { get; set; }
        public PaysViewModel Pays { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string SiteWeb { get; set; }
        public string Email { get; set; }
        public string Horaires { get; set; }
        public string Cmt { get; set; }
        public bool? Actif { get; set; } = false;
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        public string TexteAdresseList { get; set; }
        public string TexteAdresse { get; set; }
        public string TexteAdresseCentreDeTri { get; set; }
        public ICollection<ContactAdresseViewModel> ContactAdresses { get; set; }
        #endregion Properties
    }
}
