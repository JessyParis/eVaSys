/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/12/2021
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DocumentViewModel
    {
        #region Constructor
        public DocumentViewModel()
        {
        }
        #endregion
        public int RefDocument { get; set; }
        public string Libelle { get; set; }
        public string Description { get; set; }
        public DateTime? DDebut { get; set; }
        public DateTime? DFin { get; set; }
        public bool? Actif { get; set; }
        public bool? VisibiliteTotale { get; set; }
        public string Nom { get; set; }
        public DateTime DCreation { get; set; }
        public ICollection<DocumentEntiteViewModel> DocumentEntites { get; set; }
        public ICollection<DocumentEntiteTypeViewModel> DocumentEntiteTypes { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}