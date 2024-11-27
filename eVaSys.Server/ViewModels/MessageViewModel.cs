/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/11/2019
/// ----------------------------------------------------------------------------------------------------- 
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class MessageViewModel
    {
        #region Constructor
        public MessageViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefMessage { get; set; }
        public MessageTypeViewModel MessageType { get; set; }
        public string Libelle { get; set; }
        public string Titre { get; set; }
        public string Corps { get; set; }
        public string CorpsHTML { get; set; }
        public string CorpsHTMLFromText { get; set; }
        public string Cmt { get; set; }
        public DateTime? DDebut { get; set; }
        public DateTime? DFin { get; set; }
        public bool? DiffusionUnique { get; set; }
        public bool? Actif { get; set; }
        public bool? Important { get; set; }
        public bool? VisualisationConfirmeUnique { get; set; }
        public ICollection<MessageDiffusionViewModel> MessageDiffusions { get; set; }
        public ICollection<MessageVisualisationViewModel> MessageVisualisations { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
