/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/10/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CommandeFournisseurFichierFullViewModel
    {
        #region Constructor
        public CommandeFournisseurFichierFullViewModel()
        {
        }
        #endregion
        public int RefCommandeFournisseurFichier { get; set; }
        public int RefCommandeFournisseur { get; set; }
        public string Nom { get; set; }
        public string Extension { get; set; }
        public string CorpsBase64 { get; set; }
        public string VignetteBase64 { get; set; }
        public string MiniatureBase64 { get; set; }
    }
}