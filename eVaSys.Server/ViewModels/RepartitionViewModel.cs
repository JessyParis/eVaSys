/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/09/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class RepartitionViewModel
    {
        public RepartitionViewModel()
        {
        }
        public int RefRepartition { get; set; }
        public CommandeFournisseurViewModel CommandeFournisseur { get; set; }
        public bool? ExportSAGE { get; set; }
        public ICollection<RepartitionCollectiviteViewModel> RepartitionCollectivites { get; set; }
        public ICollection<RepartitionProduitViewModel> RepartitionProduits { get; set; }
        public int? PoidsReparti { get; set; }
        public int? PoidsChargement { get; set; }
        public UtilisateurViewModel UtilisateurCreation { get; set; }
        public string InfoText { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}