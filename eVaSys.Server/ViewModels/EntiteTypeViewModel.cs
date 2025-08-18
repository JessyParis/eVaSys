/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/10/2018
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EntiteTypeViewModel
    {
        #region Constructor
        public EntiteTypeViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEntiteType { get; set; }
        public string Libelle { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public bool? CodeEE { get; set; }
        public bool Adelphe { get; set; }
        public bool? RefEcoOrganisme { get; set; }
        public bool? CodeValorisation { get; set; }
        public bool? AssujettiTVA { get; set; }
        public bool? CodeTVA { get; set; }
        public bool? SAGECodeComptable { get; set; }
        public bool? SAGECompteTiers { get; set; }
        public bool? RefSAGEConditionLivraison { get; set; }
        public bool? RefSAGEPeriodicite { get; set; }
        public bool? RefSAGEModeReglement { get; set; }
        public bool? RefSAGECategorieAchat { get; set; }
        public bool? RefSAGECategorieVente { get; set; }
        public bool ActionnaireProprietaire { get; set; }
        public bool Exploitant { get; set; }
        public bool Demarrage { get; set; }
        public bool Capacite { get; set; }
        public bool TonnageGlobal { get; set; }
        public bool PopulationContratN { get; set; }
        public bool LiaisonEntiteProduit { get; set; }
        public bool LiaisonEntiteProduitInterdit { get; set; }
        public bool Horaires { get; set; }
        public bool ContratOptimisationTransport { get; set; }
        public bool ContratCollectivite { get; set; }
        public bool RefRepreneur { get; set; }
        public bool RefRepriseType { get; set; }
        public bool UtilisateurCollectivite { get; set; }
        public bool ExportSAGE { get; set; }
        public bool Process { get; set; }
        public bool VisibiliteAffretementCommun { get; set; }
        public bool? BLUnique { get; set; }
        public bool AutoControle { get; set; }
        public bool LiaisonEntiteStandard { get; set; }
        public bool SousContrat { get; set; }
        public bool LiaisonEntiteCamionType { get; set; }
        public bool IncitationQualite { get; set; }
        public bool SurcoutCarburant { get; set; }
        public bool RefEquipementier { get; set; }
        public bool RefFournisseurTO { get; set; }
        public bool DimensionBalle { get; set; }
        public bool IdNational { get; set; }
        public bool LiaisonEntiteDR { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
