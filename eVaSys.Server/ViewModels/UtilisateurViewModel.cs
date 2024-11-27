using eVaSys.Data;
using Newtonsoft.Json;
/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 04/07/2018
/// ----------------------------------------------------------------------------------------------------- 
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class UtilisateurViewModel
    {
        #region Constructor
        public UtilisateurViewModel()
        {

        }
        #endregion Constructor
        #region Properties
        public int RefUtilisateur { get; set; }
        public int? RefUtilisateurMaitre { get; set; }
        public UtilisateurListViewModel UtilisateurMaitre { get; set; }
        public int? RefUtilisateurAffectationMaitre { get; set; }
        public DateTime? DAffectationMaitre { get; set; }
        public bool? Actif { get; set; }
        public string Nom { get; set; }
        public string Login { get; set; }
        public string Pwd { get; set; }
        public DateTime? PwdDModif { get; set; }
        public bool PwdExists { get; set; }
        public string HabilitationQualite { get; set; }
        public EntiteViewModel Client { get; set; }
        public string HabilitationAnnuaire { get; set; }
        public string HabilitationLogistique { get; set; }
        public EntiteViewModel Transporteur { get; set; }
        public string HabilitationStatistique { get; set; }
        public string HabilitationAdministration { get; set; }
        public string HabilitationModuleCollectivite { get; set; }
        public EntiteViewModel Collectivite { get; set; }
        public string HabilitationModuleCentreDeTri { get; set; }
        public EntiteViewModel CentreDeTri { get; set; }
        public string HabilitationMessagerie { get; set; }
        public string HabilitationModulePrestataire { get; set; }
        public EntiteViewModel Prestataire { get; set; }
        public string EMail { get; set; }
        public string PiedEmail { get; set; }
        public string Tel { get; set; }
        public string TelMobile { get; set; }
        public string Fax { get; set; }
        public ContactAdresseViewModel ContactAdresse { get; set; }
        public string UtilisateurType { get; set; }
        public string EntiteLibelle { get; set; }
        public bool HasProfils { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        public string PwdModificationText { get; set; }
        public string AffectationUtilisateurMaitreText { get; set; }
        #endregion Properties      
    }
}
