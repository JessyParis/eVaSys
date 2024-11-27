/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/10/2018
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CommandeFournisseurStatutViewModel
    {
        #region Constructor
        public CommandeFournisseurStatutViewModel() { }
        #endregion
        #region Properties
        public int RefCommandeFournisseurStatut { get; set; }
        public string Libelle { get; set; }
        public string Couleur { get; set; }
        #endregion
    }
}
