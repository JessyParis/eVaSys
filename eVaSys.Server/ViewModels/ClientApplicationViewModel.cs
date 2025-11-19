/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ClientApplicationViewModel
    {
        #region Constructor
        public ClientApplicationViewModel()
        {
        }
        #endregion Constructor

        #region Properties
        public int RefClientApplication { get; set; }
        public EntiteViewModel Entite { get; set; }
        public ApplicationProduitOrigineViewModel ApplicationProduitOrigine { get; set; }
        public DateTime D { get; set; }
        public ICollection<ClientApplicationApplicationViewModel> ClientApplicationApplications { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
