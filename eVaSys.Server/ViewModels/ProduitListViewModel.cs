/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/03/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ProduitListViewModel
    {
        #region Constructor
        public ProduitListViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefProduit { get; set; }
        public string Libelle { get; set; }
        #endregion
    }
}
