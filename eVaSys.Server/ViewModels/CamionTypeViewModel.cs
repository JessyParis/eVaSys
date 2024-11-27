/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/10/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CamionTypeViewModel
    {
        #region Constructor
        public CamionTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefCamionType { get; set; }
        public string Libelle { get; set; }
        public ModeTransportEEViewModel ModeTransportEE { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
