/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EmailFichierViewModel
    {
        #region Constructor
        public EmailFichierViewModel()
        {
        }
        #endregion
        public int Index { get; set; }
        public string Nom { get; set; }
        public string VisualType { get; set; }
    }
}