/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/03/2020
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DescriptionCVQProduitViewModel
    {
        public DescriptionCVQProduitViewModel()
        {
        }
        public int RefDescriptionCVQProduit { get; set; }
        public int RefDescriptionCVQ { get; set; }
        public ProduitViewModel Produit { get; set; }
    }
}