/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/05/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CommandeFournisseurViewModel
    {
        public CommandeFournisseurViewModel()
        {
        }
        public int RefCommandeFournisseur { get; set; }
        public int? NumeroCommande { get; set; }
        public int? NumeroAffretement { get; set; }
        public int? OrdreAffretement { get; set; }
        public EntiteViewModel Entite { get; set; }
        public string Horaires { get; set; }
        public AdresseViewModel Adresse { get; set; }
        public string Libelle { get; set; }
        public string Adr1 { get; set; }
        public string Adr2 { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public PaysViewModel Pays { get; set; }
        public ContactAdresseViewModel ContactAdresse { get; set; }
        public CiviliteViewModel Civilite { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Tel { get; set; }
        public string TelMobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public ProduitViewModel Produit { get; set; }
        public DateTime? D { get; set; }
        public DateTime? DChargementPrevue { get; set; }
        public string HoraireChargementPrevu { get; set; }
        public bool? ChargementAnnule { get; set; }
        public DateTime? DChargementAnnule { get; set; }
        public UtilisateurViewModel UtilisateurChargementAnnule { get; set; }
        public string CmtChargementAnnule { get; set; }
        public DateTime? DChargement { get; set; }
        public DateTime? DDechargementPrevue { get; set; }
        public string HoraireDechargementPrevu { get; set; }
        public DateTime? DDechargement { get; set; }
        public int? PoidsChargement { get; set; }
        public int? NbBalleChargement { get; set; }
        public bool? TicketPeseeChargement { get; set; }
        public int? PoidsDechargement { get; set; }
        public int? NbBalleDechargement { get; set; }
        public bool? TicketPeseeDechargement { get; set; }
        public CamionTypeViewModel CamionType { get; set; }
        public EntiteViewModel Transporteur { get; set; }
        public ContactAdresseViewModel TransporteurContactAdresse;
        public decimal? PrixTransportHT { get; set; }
        public decimal? SurcoutCarburantHT { get; set; }
        public decimal? PrixTransportSupplementHT { get; set; }
        public int? Km { get; set; }
        public AdresseViewModel AdresseClient { get; set; }
        public string CmtFournisseur { get; set; }
        public string CmtClient { get; set; }
        public string CmtTransporteur { get; set; }
        public decimal PrixTonneHT { get; set; }
        public DateTime? DMoisDechargementPrevu { get; set; }
        public bool? Imprime { get; set; }
        public bool? ExportSAGE { get; set; }
        public bool? ValideDPrevues { get; set; }
        public bool? RefusCamion { get; set; }
        public int? PoidsReparti { get; set; }
        public DateTime? DBlocage { get; set; }
        public UtilisateurViewModel UtilisateurBlocage { get; set; }
        public string CmtBlocage { get; set; }
        public DateTime? DAffretement { get; set; }
        public CommandeFournisseurStatutViewModel CommandeFournisseurStatut { get; set; }
        public bool? CamionComplet { get; set; }
        public MotifCamionIncompletViewModel MotifCamionIncomplet { get; set; }
        public DateTime? DChargementModif { get; set; }
        public bool? ChargementEffectue { get; set; }
        public DateTime? DAnomalieChargement { get; set; }
        public MotifAnomalieChargementViewModel MotifAnomalieChargement { get; set; }
        public string CmtAnomalieChargement { get; set; }
        public DateTime? DTraitementAnomalieChargement { get; set; }
        public DateTime? DAnomalieClient { get; set; }
        public MotifAnomalieClientViewModel MotifAnomalieClient { get; set; }
        public string CmtAnomalieClient { get; set; }
        public DateTime? DTraitementAnomalieClient { get; set; }
        public DateTime? DAnomalieTransporteur { get; set; }
        public MotifAnomalieTransporteurViewModel MotifAnomalieTransporteur { get; set; }
        public string CmtAnomalieTransporteur { get; set; }
        public bool? LotControle { get; set; }
        public DateTime? DTraitementAnomalieTransporteur { get; set; }
        public DateTime? DAnomalieOk { get; set; }
        public UtilisateurViewModel UtilisateurAnomalieOk { get; set; }
        public bool NonRepartissable { get; set; }
        public string RefExt { get; set; }
        public bool Reparti { get; set; }
        public EntiteViewModel Prestataire { get; set; }
        public ICollection<CommandeFournisseurFichierLightViewModel> CommandeFournisseurFichiers { get; set; }
        public bool Mixte { get; set; }
        public bool MixteAndPrintable { get; set; }
        public UtilisateurViewModel LockedBy { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        public string AnomalieOkText { get; set; }
    }
}