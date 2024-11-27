/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 29/06/2020
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class NonConformiteFichierTypeViewModel
    {
        #region Constructor
        public NonConformiteFichierTypeViewModel()
        {
        }
        #endregion
        public int? RefNonConformiteFichierType { get; set; }
        public string Libelle { get; set; }
    }
}