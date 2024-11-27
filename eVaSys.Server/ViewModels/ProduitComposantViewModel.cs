/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 25/07/2022
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ProduitComposantViewModel
    {
        #region Constructor
        public ProduitComposantViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefProduitComposant { get; set; }
        public int RefProduit { get; set; }
        public int RefComposant { get; set; }
        public ProduitListViewModel Composant { get; set; }
        #endregion Properties
    }
}
