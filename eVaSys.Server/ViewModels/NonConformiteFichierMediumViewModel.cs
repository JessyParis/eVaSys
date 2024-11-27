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
    public class NonConformiteFichierMediumViewModel
    {
        #region Constructor
        public NonConformiteFichierMediumViewModel()
        {
        }
        #endregion
        public int RefNonConformiteFichier { get; set; }
        public int RefNonConformite { get; set; }
        public NonConformiteFichierTypeViewModel NonConformiteFichierType { get; set; }
        public string Nom { get; set; }
        public string Extension { get; set; }
        public string VignetteBase64 { get; set; }
        public string MiniatureBase64 { get; set; }
    }
}