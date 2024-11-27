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
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class FicheControleViewModel
    {
        public FicheControleViewModel()
        {
        }
        public int RefFicheControle { get; set; }
        public CommandeFournisseurViewModel CommandeFournisseur { get; set; }
        public EntiteViewModel Fournisseur { get; set; }
        public ProduitViewModel Produit { get; set; }
        public ContactAdresseViewModel Controleur { get; set; }
        public string NumeroLotUsine { get; set; }
        public bool? Reserve { get; set; }
        public string Cmt { get; set; }
        public bool? Valide { get; set; }
        public ICollection<FicheControleDescriptionReceptionViewModel> FicheControleDescriptionReceptions { get; set; }
        public ICollection<ControleViewModel> Controles { get; set; }
        public ICollection<CVQViewModel> CVQs { get; set; }
        public string FicheControleText { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
    }
}