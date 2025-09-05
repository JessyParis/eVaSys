/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/03/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ProduitViewModel
    {
        #region Constructor
        public ProduitViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefProduit { get; set; }
        public string _PCodeProd { get; set; }
        public string Libelle { get; set; }
        public string NomCommercial { get; set; }
        public string NomCommun { get; set; }
        public bool? Collecte { get; set; }
        public string NumeroStatistique { get; set; }
        public SAGECodeTransportViewModel SAGECodeTransport { get; set; }
        public bool? Actif { get; set; }
        public EntiteViewModel Repreneur { get; set; }
        public ApplicationProduitOrigineViewModel ApplicationProduitOrigine { get; set; }
        public bool? Composant { get; set; }
        public bool? PUHTSurtri { get; set; }
        public bool? PUHTTransport { get; set; }
        public ProduitGroupeReportingViewModel ProduitGroupeReporting { get; set; }
        public string CodeListeVerte { get; set; }
        public string CodeEE { get; set; }
        public bool? IncitationQualite { get; set; }
        public string Cmt { get; set; }
        public int? Co2KgParT { get; set; }
        public string CmtFournisseur { get; set; }
        public string CmtTransporteur { get; set; }
        public string CmtClient { get; set; }
        public string LaserType { get; set; }

        public ICollection<ProduitComposantViewModel> ProduitComposants { get; set; }
        public ICollection<ProduitStandardViewModel> ProduitStandards { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }

        #endregion
    }
}
