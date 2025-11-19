/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/10/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EntiteViewModel
    {
        #region Constructor
        public EntiteViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEntite { get; set; }
        public EntiteTypeViewModel EntiteType { get; set; }
        public string Libelle { get; set; }
        public string CodeEE { get; set; }
        public EcoOrganismeViewModel EcoOrganisme { get; set; }
        public bool? Adelphe { get; set; }
        public string CodeValorisation { get; set; }
        public string Cmt { get; set; }
        public bool? AssujettiTVA { get; set; }
        public string CodeTVA { get; set; }
        public string SAGECodeComptable { get; set; }
        public string SAGECompteTiers { get; set; }
        public SAGEConditionLivraisonViewModel SAGEConditionLivraison { get; set; }
        public SAGEPeriodiciteViewModel SAGEPeriodicite { get; set; }
        public SAGEModeReglementViewModel SAGEModeReglement { get; set; }
        public SAGECategorieAchatViewModel SAGECategorieAchat { get; set; }
        public SAGECategorieVenteViewModel SAGECategorieVente { get; set; }
        public string ActionnaireProprietaire { get; set; }
        public string Exploitant { get; set; }
        public string Demarrage { get; set; }
        public int? Capacite { get; set; }
        public int? TonnageGlobal { get; set; }
        public int? PopulationContratN { get; set; }
        public string Horaires { get; set; }
        public RepreneurViewModel Repreneur { get; set; }
        public RepriseTypeViewModel RepriseType { get; set; }
        public bool? ExportSAGE { get; set; }
        public bool? VisibiliteAffretementCommun { get; set; }
        public bool? AutoControle { get; set; }
        public bool? Actif { get; set; }
        public bool? SousContrat { get; set; }
        public bool? ReconductionIncitationQualite { get; set; }
        public bool? SurcoutCarburant { get; set; }
        public EquipementierViewModel Equipementier { get; set; }
        public FournisseurTOViewModel FournisseurTO { get; set; }
        public string DimensionBalle { get; set; }
        public string IdNational { get; set; }
        public ICollection<ActionViewModel> Actions { get; set; }
        public ICollection<EntiteCamionTypeViewModel> EntiteCamionTypes { get; set; }
        public ICollection<ContratViewModel> Contrats { get; set; }
        public ICollection<ContratIncitationQualiteViewModel> ContratIncitationQualites { get; set; }
        public ICollection<ContratCollectiviteViewModel> ContratCollectivites { get; set; }
        public ICollection<DocumentEntiteViewModel> DocumentEntites { get; set; }
        public ICollection<EntiteDRViewModel> EntiteDRs { get; set; }
        public ICollection<EntiteProcessViewModel> EntiteProcesss { get; set; }
        public ICollection<EntiteProduitViewModel> EntiteProduits { get; set; }
        public ICollection<EntiteStandardViewModel> EntiteStandards { get; set; }
        public ICollection<ContactAdresseViewModel> ContactAdresses { get; set; }
        public ICollection<AdresseViewModel> Adresses { get; set; }
        public ICollection<EntiteEntiteViewModel> EntiteEntites { get; set; }
        public ICollection<SAGEDocument> SAGEDocuments { get; set; }
        public string LibelleCode { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
