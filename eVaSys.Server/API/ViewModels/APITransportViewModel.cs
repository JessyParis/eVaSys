/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/01/2025
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class APITransportViewModel
    {
        public APITransportViewModel()
        {
        }
        public int RefCommandeFournisseur { get; set; }
        public int? NumeroCommande { get; set; }
        public string CommandeFournisseurStatutLibelle { get; set; }
        public bool Mixte { get; set; }
        public int? NumeroAffretement { get; set; }
        public int? OrdreAffretement { get; set; }
        public bool? CamionComplet { get; set; }
        public int OrigineEntiteRefEntite { get; set; }
        public string OrigineEntiteSiret { get; set; }
        public string OrigineEntiteLibelle { get; set; }
        public string OrigineHoraires { get; set; }
        public int? OrigineAdresseRefAdresse { get; set; }
        public string OrigineAdresseLibelle { get; set; }
        public string OrigineAdresseAdr1 { get; set; }
        public string OrigineAdresseAdr2 { get; set; }
        public string OrigineAdresseCodePostal { get; set; }
        public string OrigineAdresseVille { get; set; }
        public string OrigineAdressePaysLibelle { get; set; }
        public string OrigineContactPrenom { get; set; }
        public string OrigineContactNom { get; set; }
        public string OrigineContactTel { get; set; }
        public string OrigineContactTelMobile { get; set; }
        public string OrigineContactEmail { get; set; }
        public int RefProduit { get; set; }
        public string ProduitLibelle { get; set; }
        public string ProduitCmtTransporteur { get; set; }
        public DateTime? D { get; set; }
        public int? DMoisDechargementPrevu { get; set; }
        public DateTime? DChargementPrevue { get; set; }
        public string HoraireChargementPrevu { get; set; }
        public DateTime? DChargement { get; set; }
        public DateTime? DDechargementPrevue { get; set; }
        public string HoraireDechargementPrevu { get; set; }
        public DateTime? DDechargement { get; set; }
        public int? PoidsChargement { get; set; }
        public int? NbBalleChargement { get; set; }
        public int? PoidsDechargement { get; set; }
        public int? NbBalleDechargement { get; set; }
        public string CamionTypeLibelle { get; set; }
        public int TransporteurEntiteRefEntite { get; set; }
        public string TransporteurEntiteSiret { get; set; }
        public string TransporteurEntiteLibelle { get; set; }
        public string TransporteurContactPrenom { get; set; }
        public string TransporteurContactNom { get; set; }
        public string TransporteurContactTel { get; set; }
        public string TransporteurContactTelMobile { get; set; }
        public string TransporteurContactEmail { get; set; }
        public int? Km { get; set; }
        public int DestinationEntiteRefEntite { get; set; }
        public string DestinationEntiteSiret { get; set; }
        public string DestinationEntiteLibelle { get; set; }
        public int DestinationAdresseRefAdresse { get; set; }
        public string DestinationAdresseLibelle { get; set; }
        public string DestinationAdresseAdr1 { get; set; }
        public string DestinationAdresseAdr2 { get; set; }
        public string DestinationAdresseCodePostal { get; set; }
        public string DestinationAdresseVille { get; set; }
        public string DestinationAdressePaysLibelle { get; set; }
        public string DestinationHoraires { get; set; }
        public string DestinationContactPrenom { get; set; }
        public string DestinationContactNom { get; set; }
        public string DestinationContactTel { get; set; }
        public string DestinationContactTelMobile { get; set; }
        public string DestinationContactEmail { get; set; }
        public string CommandeFournisseurCmtTransporteur { get; set; }
        public string RefExt { get; set; }
    }
}