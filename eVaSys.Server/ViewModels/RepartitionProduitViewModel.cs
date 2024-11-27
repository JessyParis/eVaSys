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
    public class RepartitionProduitViewModel
    {
        public RepartitionProduitViewModel()
        {
        }
        public int RefRepartitionProduit { get; set; }
        public int RefRepartition { get; set; }
        public ProcessViewModel Process { get; set; }
        public EntiteViewModel Fournisseur { get; set; }
        public ProduitViewModel Produit { get; set; }
        public int Poids { get; set; }
        public decimal PUHT { get; set; }
    }
}