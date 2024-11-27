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
    public class DescriptionControleProduitViewModel
    {
        public DescriptionControleProduitViewModel()
        {
        }
        public int RefDescriptionControleProduit { get; set; }
        public int RefDescriptionControle { get; set; }
        public ProduitViewModel Produit { get; set; }
    }
}