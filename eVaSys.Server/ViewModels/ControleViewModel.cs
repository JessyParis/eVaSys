/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/04/2020
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ControleViewModel
    {
        public ControleViewModel()
        {
        }
        public int RefControle { get; set; }
        public int? RefFicheControle { get; set; }
        public ControleTypeViewModel ControleType { get; set; }
        public ContactAdresseViewModel Controleur { get; set; }
        public DateTime? DControle { get; set; }
        public decimal? Poids { get; set; }
        public string Etiquette { get; set; }
        public string Cmt { get; set; }
        public ICollection<ControleDescriptionControleViewModel> ControleDescriptionControles { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}