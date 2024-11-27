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
    public class EntiteProduitViewModel
    {
        #region Constructor
        public EntiteProduitViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEntiteProduit { get; set; }
        public int RefEntite { get; set; }
        public ProduitViewModel Produit { get; set; }
        public bool Interdit { get; set; }
        public UtilisateurViewModel UtilisateurCreation { get; set; }
        public DateTime? DCreation { get; set; }
        public string Cmt { get; set; }
        public string CreationText { get; set; }
        #endregion Properties
    }
}
