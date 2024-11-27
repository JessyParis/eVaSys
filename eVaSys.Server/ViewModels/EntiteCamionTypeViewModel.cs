/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/11/2021
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EntiteCamionTypeViewModel
    {
        #region Constructor
        public EntiteCamionTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEntiteCamionType { get; set; }
        public int RefEntite { get; set; }
        public CamionTypeViewModel CamionType { get; set; }
        public string Cmt { get; set; }
        public UtilisateurViewModel UtilisateurCreation { get; set; }
        public DateTime? DCreation { get; set; }
        public string CreationText { get; set; }
        #endregion Properties
    }
}
