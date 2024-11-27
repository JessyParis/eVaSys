/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/05/2024
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DocumentEntiteTypeViewModel
    {
        #region Constructor
        public DocumentEntiteTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefDocumentEntiteType { get; set; }
        public int RefDocument { get; set; }
        public int RefEntiteType { get; set; }
        public EntiteTypeViewModel EntiteType { get; set; }
        #endregion Properties
    }
}
