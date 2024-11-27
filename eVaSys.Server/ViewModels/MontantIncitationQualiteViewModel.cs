/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class MontantIncitationQualiteViewModel
    {
        #region Constructor
        public MontantIncitationQualiteViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefMontantIncitationQualite { get; set; }
        public decimal Montant { get; set; }
        public int Annee { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
