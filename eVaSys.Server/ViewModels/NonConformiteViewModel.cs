/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/06/2020
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class NonConformiteViewModel
    {
        #region Constructor
        public NonConformiteViewModel()
        {
        }
        #endregion
        public int? RefNonConformite { get; set; }
        public CommandeFournisseurViewModel CommandeFournisseur { get; set; }
        public string DescrClient { get; set; }
        public NonConformiteDemandeClientTypeViewModel NonConformiteDemandeClientType { get; set; }
        public string DescrValorplast { get; set; }
        public NonConformiteNatureViewModel NonConformiteNature { get; set; }
        public DateTime? DTransmissionFournisseur { get; set; }
        public string ActionDR { get; set; }
        public NonConformiteReponseFournisseurTypeViewModel NonConformiteReponseFournisseurType { get; set; }
        public string CmtReponseFournisseur { get; set; }
        public NonConformiteReponseClientTypeViewModel NonConformiteReponseClientType { get; set; }
        public string CmtReponseClient { get; set; }
        public bool? PlanAction { get; set; }
        public string CmtOrigineAction { get; set; }
        public bool? PriseEnCharge { get; set; }
        public decimal? MontantPriseEnCharge { get; set; }
        public string CmtPriseEnCharge { get; set; }
        public string IFFournisseurDescr { get; set; }
        public decimal? IFFournisseurFactureMontant { get; set; }
        public int? IFFournisseurDeductionTonnage { get; set; }
        public bool? IFFournisseurRetourLot { get; set; }
        public NonConformiteAccordFournisseurTypeViewModel NonConformiteAccordFournisseurType { get; set; }
        public bool? IFFournisseurAttenteBonCommande { get; set; }
        public string IFFournisseurBonCommandeNro { get; set; }
        public bool? IFFournisseurFacture { get; set; }
        public bool? IFFournisseurTransmissionFacturation { get; set; }
        public string IFFournisseurFactureNro { get; set; }
        public string IFFournisseurCmtFacturation { get; set; }
        public string IFClientDescr { get; set; }
        public decimal? IFClientFactureMontant { get; set; }
        public string IFClientFactureNro { get; set; }
        public string IFClientCmtFacturation { get; set; }
        public DateTime? IFClientDFacture { get; set; }
        public bool IFClientCommandeAFaire { get; set; }
        public bool IFClientFactureEnAttente { get; set; }
        public ICollection<NonConformiteNonConformiteFamilleViewModel> NonConformiteNonConformiteFamilles { get; set; }
        public ICollection<NonConformiteFichierLightViewModel> NonConformiteFichiers { get; set; }
        public ICollection<NonConformiteEtapeViewModel> NonConformiteEtapes { get; set; }
        public string NonConformiteTextCmd { get; set; }
        public string NonConformiteTextClient { get; set; }
        public string NonConformiteTextFournisseur { get; set; }
    }
}