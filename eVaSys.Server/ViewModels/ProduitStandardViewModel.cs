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
    public class ProduitStandardViewModel
    {
        #region Constructor
        public ProduitStandardViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefProduitStandard { get; set; }
        public int RefProduit { get; set; }
        public StandardViewModel Standard { get; set; }
        #endregion Properties
    }
}
