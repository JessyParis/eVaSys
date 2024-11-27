/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 02/05/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ActionTypeViewModel
    {
        #region Constructor
        public ActionTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefActionType { get; set; }
        public bool DocumentType { get; set; }
        public string Libelle { get; set; }
        public ICollection<ActionTypeEntiteTypeViewModel> ActionTypeEntiteTypes { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
