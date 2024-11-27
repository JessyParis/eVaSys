/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/07/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CommandeClientViewModel
    {
        public CommandeClientViewModel()
        {
        }
        public int RefCommandeClient { get; set; }
        public EntiteViewModel Entite { get; set; }
        public AdresseViewModel Adresse { get; set; }
        public DateTime D { get; set; }
        public ProduitViewModel Produit { get; set; }
        public int Poids { get; set; }
        public string Cmt { get; set; }
        public ICollection<CommandeClientMensuelleViewModel> CommandeClientMensuelles { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
    }
}