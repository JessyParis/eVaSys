/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using System;
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ApplicationViewModel
    {
        #region Constructor
        public ApplicationViewModel()
        {
        }
        #endregion Constructor

        #region Properties
        public int RefApplication { get; set; }
        public string Libelle { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
