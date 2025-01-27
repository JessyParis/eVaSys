/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/10/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class PrixRepriseViewModel
    {
        #region Constructor
        public PrixRepriseViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int? RefPrixReprise { get; set; }
        public DateTime D { get; set; }
        public ContratViewModel Contrat { get; set; }
        public ProcessViewModel Process { get; set; }
        public ProduitViewModel Produit { get; set; }
        public ProduitViewModel Composant { get; set; }
        public decimal? PUHT { get; set; }
        public decimal? PUHTSurtri { get; set; }
        public decimal? PUHTTransport { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }

        #endregion
    }
}
