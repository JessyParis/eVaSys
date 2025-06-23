/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/06/2018
/// -----------------------------------------------------------------------------------------------------

using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace eVaSys.Data
{
    public class ApplicationDbContext : DbContext
    {
        //Override all SaveChanges
        public override int SaveChanges()
        {
            HandleModificationMark();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            HandleModificationMark();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            HandleModificationMark();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleModificationMark();
            return base.SaveChangesAsync(cancellationToken);
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Mark modifications (user and date)
        /// </summary>
        private void HandleModificationMark()
        {
            foreach (var entity in ChangeTracker.Entries()
                .Where(e =>
                e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                var tracked = entity.Entity as IMarkModification;
                tracked?.MarkModification(entity.State == EntityState.Added);
            }
        }

        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<ActionType> ActionTypes { get; set; }
        public virtual DbSet<ActionTypeEntiteType> ActionTypeEntiteTypes { get; set; }
        public virtual DbSet<ActionFichier> ActionFichiers { get; set; }
        public virtual DbSet<ActionFichierNoFile> ActionFichierNoFiles { get; set; }
        public virtual DbSet<Adresse> Adresses { get; set; }
        public virtual DbSet<AdresseType> AdresseTypes { get; set; }
        public virtual DbSet<Aide> Aides { get; set; }
        public virtual DbSet<APILog> APILogs { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<ApplicationProduitOrigine> ApplicationProduitOrigines { get; set; }
        public virtual DbSet<CamionType> CamionTypes { get; set; }
        public virtual DbSet<Civilite> Civilites { get; set; }
        public virtual DbSet<ClientApplication> ClientApplications { get; set; }
        public virtual DbSet<ClientApplicationApplication> ClientApplicationApplications { get; set; }
        public virtual DbSet<CommandeClient> CommandeClients { get; set; }
        public virtual DbSet<CommandeClientMensuelle> CommandeClientMensuelles { get; set; }
        public virtual DbSet<CommandeFournisseur> CommandeFournisseurs { get; set; }
        public virtual DbSet<CommandeFournisseurContrat> CommandeFournisseurContrats { get; set; }
        public virtual DbSet<CommandeFournisseurFichier> CommandeFournisseurFichiers { get; set; }
        public virtual DbSet<CommandeFournisseurStatut> CommandeFournisseurStatuts { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<ContactAdresse> ContactAdresses { get; set; }
        public virtual DbSet<ContactAdresseProcess> ContactAdresseProcesss { get; set; }
        public virtual DbSet<ContactAdresseServiceFonction> ContactAdresseServiceFonctions { get; set; }
        public virtual DbSet<Contrat> Contrats { get; set; }
        public virtual DbSet<ContratEntite> ContratEntites { get; set; }
        public virtual DbSet<ContratIncitationQualite> ContratIncitationQualites { get; set; }
        public virtual DbSet<ContratCollectivite> ContratCollectivites { get; set; }
        public virtual DbSet<ContratType> ContratTypes { get; set; }
        public virtual DbSet<Controle> Controles { get; set; }
        public virtual DbSet<ControleDescriptionControle> ControleDescriptionControles { get; set; }
        public virtual DbSet<ControleType> ControleTypes { get; set; }
        public virtual DbSet<CVQ> CVQs { get; set; }
        public virtual DbSet<CVQDescriptionCVQ> CVQDescriptionCVQs { get; set; }
        public virtual DbSet<DescriptionControle> DescriptionControles { get; set; }
        public virtual DbSet<DescriptionControleProduit> DescriptionControleProduits { get; set; }
        public virtual DbSet<DescriptionCVQ> DescriptionCVQs { get; set; }
        public virtual DbSet<DescriptionCVQProduit> DescriptionCVQProduits { get; set; }
        public virtual DbSet<DescriptionReception> DescriptionReceptions { get; set; }
        public virtual DbSet<Dpt> Dpts { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<DocumentNoFile> DocumentNoFiles { get; set; }
        public virtual DbSet<DocumentEntite> DocumentEntites { get; set; }
        public virtual DbSet<DocumentEntiteType> DocumentEntiteTypes { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes { get; set; }
        public virtual DbSet<FicheControle> FicheControles { get; set; }
        public virtual DbSet<FicheControleDescriptionReception> FicheControleDescriptionReceptions { get; set; }
        public virtual DbSet<EcoOrganisme> EcoOrganismes { get; set; }
        public virtual DbSet<EquivalentCO2> EquivalentCO2s { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<EmailIncitationQualite> EmailIncitationQualites { get; set; }
        public virtual DbSet<EmailNoteCredit> EmailNoteCredits { get; set; }
        public virtual DbSet<Entite> Entites { get; set; }
        public virtual DbSet<EntiteCamionType> EntiteCamionTypes { get; set; }
        public virtual DbSet<EntiteDR> EntiteDRs { get; set; }
        public virtual DbSet<EntiteEntite> EntiteEntites { get; set; }
        public virtual DbSet<EntiteProduit> EntiteProduits { get; set; }
        public virtual DbSet<EntiteProcess> EntiteProcesss { get; set; }
        public virtual DbSet<EntiteType> EntiteTypes { get; set; }
        public virtual DbSet<EntiteStandard> EntiteStandards { get; set; }
        public virtual DbSet<Equipementier> Equipementiers { get; set; }
        public virtual DbSet<Fonction> Fonctions { get; set; }
        public virtual DbSet<FormeContact> FormeContacts { get; set; }
        public virtual DbSet<FournisseurTO> FournisseurTOs { get; set; }
        public virtual DbSet<JourFerie> JourFeries { get; set; }
        public virtual DbSet<Lien> Liens { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MessageDiffusion> MessageDiffusions { get; set; }
        public virtual DbSet<MessageType> MessageTypes { get; set; }
        public virtual DbSet<MessageVisualisation> MessageVisualisations { get; set; }
        public virtual DbSet<ModeTransportEE> ModeTransportEEs { get; set; }
        public virtual DbSet<MontantIncitationQualite> MontantIncitationQualites { get; set; }
        public virtual DbSet<MotifCamionIncomplet> MotifCamionIncomplets { get; set; }
        public virtual DbSet<MotifAnomalieChargement> MotifAnomalieChargements { get; set; }
        public virtual DbSet<MotifAnomalieClient> MotifAnomalieClients { get; set; }
        public virtual DbSet<MotifAnomalieTransporteur> MotifAnomalieTransporteurs { get; set; }
        public virtual DbSet<NonConformite> NonConformites { get; set; }
        public virtual DbSet<NonConformiteAccordFournisseurType> NonConformiteAccordFournisseurTypes { get; set; }
        public virtual DbSet<NonConformiteDemandeClientType> NonConformiteDemandeClientTypes { get; set; }
        public virtual DbSet<NonConformiteEtape> NonConformiteEtapes { get; set; }
        public virtual DbSet<NonConformiteEtapeType> NonConformiteEtapeTypes { get; set; }
        public virtual DbSet<NonConformiteFamille> NonConformiteFamilles { get; set; }
        public virtual DbSet<NonConformiteNonConformiteFamille> NonConformiteNonConformiteFamilles { get; set; }
        public virtual DbSet<NonConformiteFichier> NonConformiteFichiers { get; set; }
        public virtual DbSet<NonConformiteFichierType> NonConformiteFichierTypes { get; set; }
        public virtual DbSet<NonConformiteNature> NonConformiteNatures { get; set; }
        public virtual DbSet<NonConformiteReponseFournisseurType> NonConformiteReponseFournisseurTypes { get; set; }
        public virtual DbSet<NonConformiteReponseClientType> NonConformiteReponseClientTypes { get; set; }
        public virtual DbSet<ParamEmail> ParamEmails { get; set; }
        public virtual DbSet<Parametre> Parametres { get; set; }
        public virtual DbSet<Parcours> Parcourss { get; set; }
        public virtual DbSet<Pays> Payss { get; set; }
        public virtual DbSet<Process> Processs { get; set; }
        public virtual DbSet<PrixReprise> PrixReprises { get; set; }
        public virtual DbSet<Produit> Produits { get; set; }
        public virtual DbSet<ProduitComposant> ProduitComposants { get; set; }
        public virtual DbSet<ProduitGroupeReporting> ProduitGroupeReportings { get; set; }
        public virtual DbSet<ProduitGroupeReportingType> ProduitGroupeReportingTypes { get; set; }
        public virtual DbSet<ProduitStandard> ProduitStandards { get; set; }
        public virtual DbSet<RegionEE> RegionEEs { get; set; }
        public virtual DbSet<RegionEEDpt> RegionEEDpts { get; set; }
        public virtual DbSet<RegionReporting> RegionReportings { get; set; }
        public virtual DbSet<Repartition> Repartitions { get; set; }
        public virtual DbSet<RepartitionCollectivite> RepartitionCollectivites { get; set; }
        public virtual DbSet<RepartitionDetail> RepartitionDetails { get; set; }
        public virtual DbSet<RepartitionProduit> RepartitionProduits { get; set; }
        public virtual DbSet<Repreneur> Repreneurs { get; set; }
        public virtual DbSet<RepriseType> RepriseTypes { get; set; }
        public virtual DbSet<Ressource> Ressources { get; set; }
        public virtual DbSet<SAGECategorieAchat> SAGECategorieAchats { get; set; }
        public virtual DbSet<SAGECategorieVente> SAGECategorieVentes { get; set; }
        public virtual DbSet<SAGECodeTransport> SAGECodeTransports { get; set; }
        public virtual DbSet<SAGEConditionLivraison> SAGEConditionLivraisons { get; set; }
        public virtual DbSet<SAGEDocument> SAGEDocuments { get; set; }
        public virtual DbSet<SAGEDocumentArticle> SAGEDocumentArticles { get; set; }
        public virtual DbSet<SAGEDocumentCompteTVA> SAGEDocumentCompteTVAs { get; set; }
        public virtual DbSet<SAGEDocumentLigne> SAGEDocumentLignes { get; set; }
        public virtual DbSet<SAGEDocumentLigneText> SAGEDocumentLigneTexts { get; set; }
        public virtual DbSet<SAGEDocumentReglement> SAGEDocumentReglements { get; set; }
        public virtual DbSet<SAGEModeReglement> SAGEModeReglements { get; set; }
        public virtual DbSet<SAGEPeriodicite> SAGEPeriodicites { get; set; }
        public virtual DbSet<Securite> Securites { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<SessionPop> SessionPops { get; set; }
        public virtual DbSet<Standard> Standards { get; set; }
        public virtual DbSet<SurcoutCarburant> SurcoutCarburants { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Titre> Titres { get; set; }
        public virtual DbSet<Transport> Transports { get; set; }
        public virtual DbSet<Utilisateur> Utilisateurs { get; set; }
        public virtual DbSet<UtilisateurAPI> UtilisateurAPIs { get; set; }
        public virtual DbSet<Verrouillage> Verrouillages { get; set; }

        //Scalar functions declarations as sendin exception, to be sure they are not used this way, but correctly in a query
        [DbFunction("GetRefContratType1", "dbo")]
        public static int GetRefContratType1(int refEntiteFournisseur, int refEntiteClient, DateOnly d)
        {
            throw new NotImplementedException();
        }

        #region Constructor

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constructor

        #region Models

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Action>(entity =>
            {
                entity.HasKey(e => e.RefAction);

                entity.ToTable("tblAction");

                entity.HasIndex(e => e.RefContactAdresse);

                entity.HasIndex(e => e.RefEntite);

                entity.HasIndex(e => e.RefEnvoiDocument);

                entity.HasIndex(e => e.RefFormeContact);

                entity.Property(e => e.DAction)
                    .HasColumnType("datetime");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle).HasMaxLength(100);

                entity.HasOne(d => d.FormeContact)
                    .WithMany(p => p.Actions)
                    .HasForeignKey(d => d.RefFormeContact)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ContactAdresse)
                    .WithMany(p => p.Actions)
                    .HasForeignKey(d => d.RefContactAdresse)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.ActionActionTypes)
                    .WithOne()
                    .HasForeignKey(d => d.RefAction)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.ActionDocumentTypes)
                    .WithOne()
                    .HasForeignKey(d => d.RefAction)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.ActionFichierNoFiles)
                    .WithOne()
                    .HasForeignKey(d => d.RefAction)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.ActionFichiers)
                    .WithOne()
                    .HasForeignKey(d => d.RefAction)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.Emails)
                    .WithOne()
                    .HasForeignKey(d => d.RefAction)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ActionCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ActionModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ActionActionType>(entity =>
            {
                entity.HasKey(e => e.RefActionActionType);
                entity.ToTable("tbmActionActionType");
                entity.HasIndex(e => e.RefAction);
                entity.HasIndex(e => e.RefActionType);
                entity.HasIndex(e => new { e.RefAction, e.RefActionType })
                    .HasDatabaseName("IX_tbmActionActionType_Unique")
                    .IsUnique();
                entity.HasOne(d => d.ActionType)
                    .WithMany(p => p.ActionActionTypes)
                    .HasForeignKey(d => d.RefActionType)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ActionDocumentType>(entity =>
            {
                entity.HasKey(e => e.RefActionDocumentType);
                entity.ToTable("tbmActionDocumentType");
                entity.HasIndex(e => e.RefAction);
                entity.HasIndex(e => e.RefDocumentType);
                entity.HasIndex(e => new { e.RefAction, e.RefDocumentType })
                    .HasDatabaseName("IX_tbmActionDocumentType_Unique")
                    .IsUnique();
                entity.Property(e => e.Nb).HasDefaultValueSql("((1))");
                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.ActionDocumentTypes)
                    .HasForeignKey(d => d.RefDocumentType)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ActionFichier>(entity =>
            {
                entity.HasKey(e => e.RefActionFichier);

                entity.ToTable("tblActionFichier");

                entity.HasIndex(e => e.RefAction);

                entity.Property(e => e.Extension).HasMaxLength(50);

                entity.Property(e => e.Nom).HasMaxLength(250);
            });
            modelBuilder.Entity<ActionFichierNoFile>(entity =>
            {
                entity.HasKey(e => e.RefActionFichier);

                entity.ToTable("VueActionFichierNoFile");

                entity.HasIndex(e => e.RefAction);

                entity.Property(e => e.Extension).HasMaxLength(50);

                entity.Property(e => e.Nom).HasMaxLength(250);
            });
            modelBuilder.Entity<ActionType>(entity =>
            {
                entity.HasKey(e => e.RefActionType);
                entity.ToTable("tbrActionType");
                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");
                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasMany(d => d.ActionTypeEntiteTypes)
                    .WithOne()
                    .HasForeignKey(d => d.RefActionType)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ActionTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ActionTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ActionTypeEntiteType>(entity =>
            {
                entity.HasKey(e => e.RefActionTypeEntiteType);
                entity.ToTable("tbmActionTypeEntiteType");
                entity.HasIndex(e => e.RefActionType);
                entity.HasIndex(e => e.RefEntiteType);
                entity.HasIndex(e => new { e.RefActionType, e.RefEntiteType })
                    .HasDatabaseName("IX_tbmActionTypeEntiteType_Unique");
                entity.HasOne(d => d.EntiteType)
                    .WithMany(p => p.ActionTypeEntiteTypes)
                    .HasForeignKey(d => d.RefEntiteType)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Adresse>(entity =>
            {
                entity.HasKey(e => e.RefAdresse);

                entity.ToTable("tblAdresse");

                entity.HasIndex(e => e.RefAdresseType);

                entity.HasIndex(e => e.RefEntite);

                entity.HasIndex(e => e.RefPays)
                    .HasDatabaseName("IX_tblAdresse_RefAdresse");

                entity.Property(e => e.Actif)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Adr1).HasMaxLength(69);

                entity.Property(e => e.Adr2).HasMaxLength(69);

                entity.Property(e => e.CodePostal)
                    .IsRequired()
                    .HasMaxLength(9);

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.Fax).HasMaxLength(20);

                entity.Property(e => e.Libelle).HasMaxLength(69);

                entity.Property(e => e.RefExt).HasMaxLength(50);

                entity.Property(e => e.SiteWeb).HasMaxLength(200);

                entity.Property(e => e.Tel).HasMaxLength(20);

                entity.Property(e => e.Ville)
                    .IsRequired()
                    .HasMaxLength(35);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.AdresseCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.AdresseModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Pays)
                    .WithMany(p => p.Adresses)
                    .HasForeignKey(d => d.RefPays)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.AdresseType)
                    .WithMany(p => p.Adresses)
                    .HasForeignKey(d => d.RefAdresseType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Entite)
                    .WithMany(p => p.Adresses)
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });
            modelBuilder.Entity<AdresseType>(entity =>
            {
                entity.HasKey(e => e.RefAdresseType);

                entity.ToTable("tbrAdresseType");

                entity.Property(e => e.RefAdresseType).ValueGeneratedOnAdd();

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.AdresseTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.AdresseTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Aide>(entity =>
            {
                entity.HasKey(e => e.RefAide);

                entity.ToTable("tblAide");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.Composant)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.AideCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.AideModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<APILog>(entity =>
            {
                entity.HasKey(e => e.RefAPILog);

                entity.ToTable("tbsAPILog");

                entity.Property(e => e.D)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Utilisateur)
                    .WithMany(p => p.APILogs)
                    .HasForeignKey(d => d.RefUtilisateur)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.RefApplication);

                entity.ToTable("tblApplication");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ApplicationCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ApplicationModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ApplicationProduitOrigine>(entity =>
            {
                entity.HasKey(e => e.RefApplicationProduitOrigine);

                entity.ToTable("tblApplicationProduitOrigine");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Libelle).HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ApplicationProduitOrigineCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ApplicationProduitOrigineModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Civilite>(entity =>
            {
                entity.HasKey(e => e.RefCivilite);

                entity.ToTable("tbrCivilite");

                entity.Property(e => e.RefCivilite).ValueGeneratedOnAdd();

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.CiviliteCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.CiviliteModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Pays)
                    .WithMany(p => p.Civilites)
                    .HasForeignKey(d => d.RefPays)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<CamionType>(entity =>
            {
                entity.HasKey(e => e.RefCamionType);

                entity.ToTable("tbrCamionType");

                entity.Property(e => e.RefCamionType).ValueGeneratedOnAdd();

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.CamionTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.CamionTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ModeTransportEE)
                    .WithMany(p => p.CamionTypes)
                    .HasForeignKey(d => d.RefModeTransportEE)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ClientApplication>(entity =>
            {
                entity.HasKey(e => e.RefClientApplication);

                entity.ToTable("tblClientApplication");

                entity.Property(e => e.D).HasColumnType("datetime");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Entite)
                    .WithMany(p => p.ClientApplications)
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ApplicationProduitOrigine)
                    .WithMany(p => p.ClientApplications)
                    .HasForeignKey(d => d.RefApplicationProduitOrigine)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.ClientApplicationApplications)
                    .WithOne()
                    .HasForeignKey(d => d.RefClientApplication)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ClientApplicationCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ClientApplicationModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ClientApplicationApplication>(entity =>
            {
                entity.HasKey(e => e.RefClientApplicationApplication);

                entity.ToTable("tblClientApplicationApplication");

                entity.Property(e => e.Ratio).HasColumnType("decimal(4, 1)");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ClientApplicationApplications)
                    .HasForeignKey(d => d.RefApplication)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<CommandeClient>(entity =>
            {
                entity.HasKey(e => e.RefCommandeClient);

                entity.ToTable("tblCommandeClient");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.HasMany(d => d.CommandeClientMensuelles)
                    .WithOne()
                    .HasForeignKey(d => d.RefCommandeClient)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.Entite)
                    .WithMany(p => p.CommandeClients)
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Contrat)
                    .WithMany(p => p.CommandeClients)
                    .HasForeignKey(d => d.RefContrat)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Adresse)
                    .WithMany(p => p.CommandeClients)
                    .HasForeignKey(d => d.RefAdresse)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.CommandeClients)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.CommandeClientCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.CommandeClientModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<CommandeClientMensuelle>(entity =>
            {
                entity.HasKey(e => e.RefCommandeClientMensuelle);

                entity.ToTable("tblCommandeClientMensuelle");

                entity.HasIndex(e => new { e.RefCommandeClient, e.D })
                    .HasDatabaseName("IX_tblCommandeClientMensuelle_Unique")
                    .IsUnique();

                entity.Property(e => e.D).HasColumnType("datetime");

                entity.Property(e => e.IdExt).HasMaxLength(50);

                entity.Property(e => e.PrixTonneHT)
                    .HasColumnType("decimal(10, 2)");
            });
            modelBuilder.Entity<CommandeFournisseur>(entity =>
            {
                entity.HasKey(e => e.RefCommandeFournisseur);

                entity.ToTable("tblCommandeFournisseur");
                entity.ToTable(tb => tb.HasTrigger("SetNumeroCommande"));

                entity.HasIndex(e => e.DBlocage);

                entity.HasIndex(e => e.NumeroAffretement);

                entity.HasIndex(e => e.NumeroCommande);

                entity.HasIndex(e => e.RefAdresse);

                entity.HasIndex(e => e.RefAdresseClient);

                entity.HasIndex(e => e.RefCamionType);

                entity.HasIndex(e => e.RefCivilite);

                entity.HasIndex(e => e.RefContactAdresse);

                entity.HasIndex(e => e.RefEntite);

                entity.HasIndex(e => e.RefMotifAnomalieChargement);

                entity.HasIndex(e => e.RefMotifAnomalieClient);

                entity.HasIndex(e => e.RefMotifAnomalieTransporteur);

                entity.HasIndex(e => e.RefMotifCamionIncomplet);

                entity.HasIndex(e => e.RefProduit);

                entity.HasIndex(e => e.RefTransporteur);

                entity.HasIndex(e => e.RefTransporteurContactAdresse);

                entity.HasIndex(e => e.RefUtilisateurBlocage);

                entity.Property(e => e.Adr1).HasMaxLength(69);

                entity.Property(e => e.Adr2).HasMaxLength(69);

                entity.Property(e => e.CmtClient).HasMaxLength(200);

                entity.Property(e => e.CmtFournisseur).HasMaxLength(200);

                entity.Property(e => e.CmtTransporteur).HasMaxLength(200);

                entity.Property(e => e.CodePostal).HasMaxLength(9);

                entity.Property(e => e.D).HasColumnType("datetime");

                entity.Property(e => e.DAffretement)
                    .HasColumnType("datetime");

                entity.Property(e => e.DAnomalieChargement)
                    .HasColumnType("datetime");

                entity.Property(e => e.DAnomalieClient)
                    .HasColumnType("datetime");

                entity.Property(e => e.DAnomalieTransporteur)
                    .HasColumnType("datetime");

                entity.Property(e => e.DBlocage)
                    .HasColumnType("datetime");

                entity.Property(e => e.DChargement)
                    .HasColumnType("datetime");

                entity.Property(e => e.DChargementModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.DChargementPrevue)
                    .HasColumnType("datetime");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DDechargement)
                    .HasColumnName("DDechargement")
                    .HasColumnType("datetime");

                entity.Property(e => e.DDechargementPrevue)
                    .HasColumnName("DDechargementPrevue")
                    .HasColumnType("datetime");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.DMoisDechargementPrevu)
                    .HasColumnType("datetime");

                entity.Property(e => e.DTraitementAnomalieChargement)
                    .HasColumnType("datetime");

                entity.Property(e => e.DTraitementAnomalieClient)
                    .HasColumnType("datetime");

                entity.Property(e => e.DTraitementAnomalieTransporteur)
                    .HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.ExportSAGE).HasColumnName("ExportSAGE");

                entity.Property(e => e.Fax).HasMaxLength(20);

                entity.Property(e => e.HoraireDechargementPrevu).HasMaxLength(5);

                entity.Property(e => e.Libelle).HasMaxLength(69);

                entity.Property(e => e.Nom).HasMaxLength(50);

                entity.Property(e => e.OrdreAffretement).HasDefaultValueSql("((1))");

                entity.Property(e => e.Prenom).HasMaxLength(50);

                entity.Property(e => e.PrixTonneHT)
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PrixTransportHT)
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PrixTransportSupplementHT)
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Tel).HasMaxLength(20);

                entity.Property(e => e.TelMobile).HasMaxLength(20);

                entity.Property(e => e.TicketPeseeDechargement).HasDefaultValueSql("((0))");

                entity.Property(e => e.Ville).HasMaxLength(35);

                entity.HasOne(d => d.Entite)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Adresse)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefAdresse)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ContactAdresse)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefContactAdresse)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CommandeFournisseurStatut)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefCommandeFournisseurStatut)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Transporteur)
                    .WithMany(p => p.CommandeFournisseurTransporteurs)
                    .HasForeignKey(d => d.RefTransporteur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.TransporteurContactAdresse)
                    .WithMany(p => p.CommandeFournisseurTransporteurs)
                    .HasForeignKey(d => d.RefTransporteurContactAdresse)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Pays)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefPays)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Civilite)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefCivilite)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CamionType)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefCamionType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CommandeFournisseurContrat)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefCamionType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.AdresseClient)
                    .WithMany(p => p.CommandeFournisseurAdresseClients)
                    .HasForeignKey(d => d.RefAdresseClient)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurChargementAnnule)
                    .WithMany(p => p.CommandeFournisseurUtilisateurChargementAnnules)
                    .HasForeignKey(d => d.RefUtilisateurChargementAnnule)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurBlocage)
                    .WithMany(p => p.CommandeFournisseurUtilisateurBlocages)
                    .HasForeignKey(d => d.RefUtilisateurBlocage)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.MotifCamionIncomplet)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefMotifCamionIncomplet)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.MotifAnomalieChargement)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefMotifAnomalieChargement)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.MotifAnomalieClient)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefMotifAnomalieClient)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.MotifAnomalieTransporteur)
                    .WithMany(p => p.CommandeFournisseurs)
                    .HasForeignKey(d => d.RefMotifAnomalieTransporteur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurAnomalieOk)
                    .WithMany(p => p.CommandeFournisseurUtilisateurAnomalieOks)
                    .HasForeignKey(d => d.RefUtilisateurAnomalieOk)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.CommandeFournisseurFichiers)
                    .WithOne()
                    .HasForeignKey(d => d.RefCommandeFournisseur)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.CommandeFournisseurCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.CommandeFournisseurModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Prestataire)
                    .WithMany(p => p.CommandeFournisseurPrestataires)
                    .HasForeignKey(d => d.RefPrestataire)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<CommandeFournisseurContrat>(entity =>
            {
                entity.HasKey(e => e.RefCommandeFournisseurContrat);

                entity.ToTable("VueCommandeFournisseurContrat");

                entity.HasOne(d => d.Contrat)
                    .WithMany(p => p.CommandeFournisseurContrats)
                    .HasForeignKey(d => d.RefContrat)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<CommandeFournisseurFichier>(entity =>
            {
                entity.HasKey(e => e.RefCommandeFournisseurFichier);

                entity.ToTable("tblCommandeFournisseurFichier");

                entity.Property(e => e.Corps).IsRequired();

                entity.Property(e => e.Extension).HasMaxLength(50);

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(250);
            });
            modelBuilder.Entity<CommandeFournisseurStatut>(entity =>
            {
                entity.HasKey(e => e.RefCommandeFournisseurStatut);

                entity.ToTable("tbsCommandeFournisseurStatut");

                entity.Property(e => e.Couleur).HasMaxLength(7);

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.RefContact);

                entity.ToTable("tblContact");

                entity.HasIndex(e => e.RefCivilite);

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Nom).HasMaxLength(50);

                entity.Property(e => e.Prenom).HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ContactCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ContactModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Civilite)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.RefCivilite)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ContactAdresse>(entity =>
            {
                entity.HasKey(e => e.RefContactAdresse);

                entity.ToTable("tbmContactAdresse");

                entity.HasIndex(e => e.RefAdresse);

                entity.HasIndex(e => e.RefContact);

                entity.HasIndex(e => e.RefTitre);

                entity.Property(e => e.Actif)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.Fax).HasMaxLength(20);

                entity.Property(e => e.Tel).HasMaxLength(20);

                entity.Property(e => e.TelMobile).HasMaxLength(20);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ContactAdresseCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ContactAdresseModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Titre)
                    .WithMany(p => p.ContactAdresses)
                    .HasForeignKey(d => d.RefTitre)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactAdresses)
                    .HasForeignKey(d => d.RefContact)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Adresse)
                    .WithMany(p => p.ContactAdresses)
                    .HasForeignKey(d => d.RefAdresse)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Entite)
                    .WithMany(p => p.ContactAdresses)
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.ContactAdresseContactAdresseProcesss)
                    .WithOne()
                    .HasForeignKey(d => d.RefContactAdresse)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.Utilisateurs)
                    .WithOne()
                    .HasForeignKey(d => d.RefContactAdresse)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.ContactAdresseDocumentTypes)
                    .WithOne()
                    .HasForeignKey(d => d.RefContactAdresse)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.ContactAdresseServiceFonctions)
                    .WithOne()
                    .HasForeignKey(d => d.RefContactAdresse)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });
            modelBuilder.Entity<ContactAdresseContactAdresseProcess>(entity =>
            {
                entity.HasKey(e => e.RefContactAdresseContactAdresseProcess);

                entity.ToTable("tbmContactAdresseContactAdresseProcess");

                entity.HasIndex(e => e.RefContactAdresse);

                entity.HasIndex(e => e.RefContactAdresseProcess);

                entity.HasIndex(e => new { e.RefContactAdresse, e.RefContactAdresseProcess })
                    .HasDatabaseName("IX_tbmContactAdresseContactAdresseProcess_Unique")
                    .IsUnique();

                entity.HasOne(d => d.ContactAdresseProcess)
                    .WithMany(p => p.ContactAdresseContactAdresseProcesss)
                    .HasForeignKey(d => d.RefContactAdresseProcess)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ContactAdresseDocumentType>(entity =>
            {
                entity.HasKey(e => e.RefContactAdresseDocumentType);
                entity.ToTable("tbmContactAdresseDocumentType");
                entity.HasIndex(e => e.RefContactAdresse);
                entity.HasIndex(e => e.RefDocumentType);
                entity.HasIndex(e => new { e.RefContactAdresse, e.RefDocumentType })
                    .HasDatabaseName("IX_tbmContactAdresseDocumentType_Unique")
                    .IsUnique();
                entity.Property(e => e.Nb).HasDefaultValueSql("((1))");
                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.ContactAdresseDocumentTypes)
                    .HasForeignKey(d => d.RefDocumentType)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ContactAdresseProcess>(entity =>
            {
                entity.HasKey(e => e.RefContactAdresseProcess);

                entity.ToTable("tbsContactAdresseProcess");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<ContactAdresseServiceFonction>(entity =>
            {
                entity.HasKey(e => e.RefContactAdresseServiceFonction);

                entity.ToTable("tbmContactAdresseServiceFonction");

                entity.HasIndex(e => e.RefContactAdresse);

                entity.HasIndex(e => e.RefFonction);

                entity.HasIndex(e => e.RefService);

                entity.HasIndex(e => new { e.RefContactAdresse, e.RefService, e.RefFonction })
                    .HasDatabaseName("IX_tbmContactAdresseServiceFonction_Unique")
                    .IsUnique();

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ContactAdresseServiceFonctions)
                    .HasForeignKey(d => d.RefService)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Fonction)
                    .WithMany(p => p.ContactAdresseServiceFonctions)
                    .HasForeignKey(d => d.RefFonction)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Contrat>(entity =>
            {
                entity.HasKey(e => e.RefContrat);

                entity.ToTable("tblContrat");

                entity.HasOne(d => d.ContratType)
                    .WithMany(p => p.Contrats)
                    .HasForeignKey(d => d.RefContratType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.ContratEntites)
                    .WithOne()
                    .HasForeignKey(d => d.RefContrat)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ContratCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ContratModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ContratEntite>(entity =>
            {
                entity.HasKey(e => e.RefContratEntite);

                entity.ToTable("tbmContratEntite");

                entity.HasIndex(e => new { e.RefContrat, e.RefEntite })
                    .HasDatabaseName("IX_tbmContratEntite_Unique")
                    .IsUnique();

                entity.HasOne(d => d.Contrat)
                    .WithMany(p => p.ContratEntites)
                    .HasForeignKey(d => d.RefContrat)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Entite)
                    .WithMany(p => p.ContratEntites)
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<ContratIncitationQualite>(entity =>
            {
                entity.HasKey(e => e.RefContratIncitationQualite);

                entity.ToTable("tblContratIncitationQualite");

                entity.Property(e => e.DDebut)
                    .HasColumnType("datetime");

                entity.Property(e => e.DFin)
                    .HasColumnType("datetime");
            });
            modelBuilder.Entity<ContratCollectivite>(entity =>
            {
                entity.HasKey(e => e.RefContratCollectivite);

                entity.ToTable("tblContratCollectivite");

                entity.Property(e => e.DDebut)
                    .HasColumnType("datetime");

                entity.Property(e => e.DFin)
                    .HasColumnType("datetime");
            });
            modelBuilder.Entity<ContratType>(entity =>
            {
                entity.HasKey(e => e.RefContratType);

                entity.ToTable("tbsContratType");
            });
            modelBuilder.Entity<Controle>(entity =>
            {
                entity.HasKey(e => e.RefControle);
                entity.ToTable("tblControle");

                entity.HasIndex(e => e.RefControleType);

                entity.HasIndex(e => e.RefControleur);

                entity.HasIndex(e => e.RefFicheControle);

                entity.Property(e => e.DControle)
                    .HasColumnType("datetime");

                entity.Property(e => e.Debit)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Etiquette).HasMaxLength(50);

                entity.Property(e => e.Poids)
                    .HasColumnType("decimal(10, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.HasOne(d => d.ControleType)
                    .WithMany(p => p.Controles)
                    .HasForeignKey(d => d.RefControleType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Controleur)
                    .WithMany(p => p.ControleControleurs)
                    .HasForeignKey(d => d.RefControleur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.ControleDescriptionControles)
                    .WithOne()
                    .HasForeignKey(d => d.RefControle)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ControleCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ControleModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ControleDescriptionControle>(entity =>
            {
                entity.HasKey(e => e.RefControleDescriptionControle);

                entity.ToTable("tblControleDescriptionControle");


                entity.HasIndex(e => e.Ordre);

                entity.HasIndex(e => e.RefControle);

                entity.Property(e => e.LibelleENGB)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.LibelleFRFR)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.DescriptionControle)
                    .WithMany(p => p.ControleDescriptionControles)
                    .HasForeignKey(d => d.RefDescriptionControle)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ControleType>(entity =>
            {
                entity.HasKey(e => e.RefControleType);
                entity.ToTable("tbrControleType");

                entity.Property(e => e.LibelleENGB)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LibelleFRFR)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<CVQ>(entity =>
            {
                entity.HasKey(e => e.RefCVQ);

                entity.ToTable("tblCVQ");

                entity.HasIndex(e => e.RefControleur);

                entity.HasIndex(e => e.RefFicheControle)
                    .IsUnique();

                entity.Property(e => e.DCVQ)
                    .HasColumnType("datetime");

                entity.Property(e => e.Etiquette).HasMaxLength(50);

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Controleur)
                    .WithMany(p => p.CVQControleurs)
                    .HasForeignKey(d => d.RefControleur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.CVQDescriptionCVQs)
                    .WithOne()
                    .HasForeignKey(d => d.RefCVQ)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.CVQCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.CVQModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<CVQDescriptionCVQ>(entity =>
            {
                entity.HasKey(e => e.RefCVQDescriptionCVQ);

                entity.ToTable("tblCVQDescriptionCVQ");

                entity.Property(e => e.LibelleENGB)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.LibelleFRFR)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.DescriptionCVQ)
                    .WithMany(p => p.CVQDescriptionCVQs)
                    .HasForeignKey(d => d.RefDescriptionCVQ)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Ressource>(entity =>
            {
                entity.HasKey(e => e.RefRessource);

                entity.ToTable("tblRessource");

                entity.Property(e => e.RefRessource).ValueGeneratedOnAdd();

                entity.Property(e => e.LibelleFRFR)
                    .IsRequired();

                entity.Property(e => e.LibelleENGB);
            });
            modelBuilder.Entity<DescriptionControle>(entity =>
            {
                entity.HasKey(e => e.RefDescriptionControle);
                entity.ToTable("tbrDescriptionControle");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.LibelleENGB)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.LibelleFRFR)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasMany(d => d.DescriptionControleProduits)
                    .WithOne()
                    .HasForeignKey(d => d.RefDescriptionControle)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.DescriptionControleCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.DescriptionControleModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DescriptionControleProduit>(entity =>
            {
                entity.HasKey(e => e.RefDescriptionControleProduit);

                entity.ToTable("tbmDescriptionControleProduit");

                entity.HasIndex(e => e.RefDescriptionControle);

                entity.HasIndex(e => e.RefProduit);

                entity.HasIndex(e => new { e.RefProduit, e.RefDescriptionControle })
                    .HasDatabaseName("IX_tbmDescriptionControleProduit_Unique")
                    .IsUnique();

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.DescriptionControleProduits)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DescriptionCVQ>(entity =>
            {
                entity.HasKey(e => e.RefDescriptionCVQ);
                entity.ToTable("tbrDescriptionCVQ");

                entity.Property(e => e.Actif)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.LibelleENGB)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.LibelleFRFR)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasMany(d => d.DescriptionCVQProduits)
                    .WithOne()
                    .HasForeignKey(d => d.RefDescriptionCVQ)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.DescriptionCVQCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.DescriptionCVQModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DescriptionCVQProduit>(entity =>
            {
                entity.HasKey(e => e.RefDescriptionCVQProduit);

                entity.ToTable("tbmDescriptionCVQProduit");

                entity.HasIndex(e => e.RefDescriptionCVQ);

                entity.HasIndex(e => e.RefProduit);

                entity.HasIndex(e => new { e.RefProduit, e.RefDescriptionCVQ })
                    .HasDatabaseName("IX_tbmDescriptionCVQProduit_Unique")
                    .IsUnique();

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.DescriptionCVQProduits)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DescriptionReception>(entity =>
            {
                entity.HasKey(e => e.RefDescriptionReception);
                entity.ToTable("tbrDescriptionReception");

                entity.HasIndex(e => e.RefDescriptionReception)
                    .HasDatabaseName("IX_tbrDescriptionReception");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.LibelleENGB)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LibelleFRFR)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NonENGB)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NonFRFR)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OuiENGB)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OuiFRFR)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.DescriptionReceptionCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.DescriptionReceptionModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Dpt>(entity =>
            {
                entity.HasKey(e => e.RefDpt);

                entity.ToTable("tbrDpt");

                entity.Property(e => e.RefDpt)
                    .HasMaxLength(2)
                    .ValueGeneratedNever();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.RefDocument);

                entity.ToTable("tblDocument");

                entity.Property(e => e.Actif)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Nom)
                    .HasMaxLength(250);

                entity.HasMany(d => d.DocumentEntites)
                    .WithOne()
                    .HasForeignKey(d => d.RefDocument)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.DocumentEntiteTypes)
                    .WithOne()
                    .HasForeignKey(d => d.RefDocument)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.DocumentCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.DocumentModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
                
            });
            modelBuilder.Entity<DocumentNoFile>(entity =>
            {
                entity.HasKey(e => e.RefDocument);

                entity.ToTable("VueDocumentNoFile");

                entity.HasMany(d => d.DocumentEntites)
                    .WithOne()
                    .HasForeignKey(d => d.RefDocument)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.DocumentEntiteTypes)
                    .WithOne()
                    .HasForeignKey(d => d.RefDocument)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.DocumentNoFileCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.DocumentNoFileModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DocumentEntite>(entity =>
            {
                entity.HasKey(e => e.RefDocumentEntite);

                entity.ToTable("tbmDocumentEntite");

                entity.HasIndex(e => new { e.RefDocument, e.RefEntite })
                    .HasDatabaseName("IX_tbmDocumentEntite_Unique")
                    .IsUnique();

                entity.HasOne(d => d.DocumentNoFile)
                    .WithMany(p => p.DocumentEntites)
                    .HasForeignKey(d => d.RefDocument)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Entite)
                    .WithMany(p => p.DocumentEntites)
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<DocumentEntiteType>(entity =>
            {
                entity.HasKey(e => e.RefDocumentEntiteType);

                entity.ToTable("tbmDocumentEntiteType");

                entity.HasIndex(e => new { e.RefDocument, e.RefEntiteType })
                    .HasDatabaseName("IX_tbmDocumentEntiteType_Unique")
                    .IsUnique();

                entity.Property(e => e.RefEntiteType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.EntiteType)
                    .WithMany(p => p.DocumentEntiteTypes)
                    .HasForeignKey(d => d.RefEntiteType)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasKey(e => e.RefDocumentType);

                entity.ToTable("tbrDocumentType");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.DocumentTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.DocumentTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<FicheControle>(entity =>
            {
                entity.HasKey(e => e.RefFicheControle);

                entity.ToTable("tblFicheControle");

                entity.HasIndex(e => e.NumeroBonLivraison);

                entity.HasIndex(e => e.RefControleur);

                entity.HasIndex(e => e.RefFournisseur);

                entity.HasIndex(e => e.RefProduit);

                entity.Property(e => e.DLivraison)
                    .HasColumnType("datetime");

                entity.Property(e => e.NumeroBonLivraison).HasMaxLength(50);

                entity.Property(e => e.NumeroLotUsine).HasMaxLength(50);

                entity.HasMany(d => d.FicheControleDescriptionReceptions)
                    .WithOne()
                    .HasForeignKey(d => d.RefFicheControle)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.Controles)
                    .WithOne()
                    .HasForeignKey(d => d.RefFicheControle)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.CVQs)
                    .WithOne()
                    .HasForeignKey(d => d.RefFicheControle)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.CommandeFournisseur)
                    .WithMany(p => p.FicheControles)
                    .HasForeignKey(d => d.RefCommandeFournisseur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Controleur)
                    .WithMany(p => p.FicheControleControleurs)
                    .HasForeignKey(d => d.RefControleur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.FicheControles)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Fournisseur)
                    .WithMany(p => p.FicheControles)
                    .HasForeignKey(d => d.RefFournisseur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.FicheControleCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.FicheControleModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<FicheControleDescriptionReception>(entity =>
            {
                entity.HasKey(e => e.RefFicheControleDescriptionReception);

                entity.ToTable("tblFicheControleDescriptionReception");

                entity.HasIndex(e => e.RefDescriptionReception)
                    .HasDatabaseName("IX_tbmFicheControleDescriptionReception_RefDescriptionReception");

                entity.HasIndex(e => e.RefFicheControle)
                    .HasDatabaseName("IX_tbmFicheControleDescriptionReception_RefFicheControle");

                entity.Property(e => e.LibelleENGB)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LibelleFRFR)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NonENGB)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NonFRFR)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OuiENGB)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OuiFRFR)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.DescriptionReception)
                    .WithMany(p => p.FicheControleDescriptionReceptions)
                    .HasForeignKey(d => d.RefDescriptionReception)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EcoOrganisme>(entity =>
            {
                entity.HasKey(e => e.RefEcoOrganisme);

                entity.ToTable("tbrEcoOrganisme");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.EcoOrganismeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.EcoOrganismeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EquivalentCO2>(entity =>
            {
                entity.HasKey(e => e.RefEquivalentCO2);

                entity.ToTable("tblEquivalentCO2");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Ratio).HasColumnType("decimal(10, 3)");

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.EquivalentCO2Creations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.EquivalentCO2Modifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Email>(entity =>
            {
                entity.HasKey(e => e.RefEmail);

                entity.ToTable("tblEmail");

                entity.HasIndex(e => e.EmailFrom);

                entity.HasIndex(e => e.EmailSubject);

                entity.HasIndex(e => e.EmailTo);

                entity.HasIndex(e => e.POPId);

                entity.HasIndex(e => e.RefAction);

                entity.HasIndex(e => e.RefParamEmail);

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmailFrom).HasMaxLength(200);

                entity.Property(e => e.EmailSubject).HasMaxLength(200);

                entity.Property(e => e.EmailTo).HasMaxLength(200);

                entity.Property(e => e.Libelle).HasMaxLength(200);

                entity.HasOne(d => d.ParamEmail)
                    .WithMany(p => p.Emails)
                    .HasForeignKey(d => d.RefParamEmail)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.Emails)
                    .HasForeignKey(d => d.RefAction)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.EmailCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.EmailModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EmailIncitationQualite>(entity =>
            {
                entity.HasKey(e => e.RefEmailIncitationQualite);

                entity.ToTable("tblEmailIncitationQualite");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<EmailNoteCredit>(entity =>
            {
                entity.HasKey(e => e.RefEmailNoteCredit);

                entity.ToTable("tblEmailNoteCredit");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<Entite>(entity =>
            {
                entity.HasKey(e => e.RefEntite);

                entity.ToTable("tblEntite");

                entity.HasIndex(e => e.RefEntiteType)
                    .HasDatabaseName("IX_tblEntite_EntiteType");

                entity.Property(e => e.Actif)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ActionnaireProprietaire).HasMaxLength(50);

                entity.Property(e => e.AncienCodeAdelphe).HasMaxLength(15);

                entity.Property(e => e.AssujettiTVA)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CodeEE)
                    .HasMaxLength(20);

                entity.Property(e => e.CodeTVA)
                    .HasMaxLength(20);

                entity.Property(e => e.CodeValorisation).HasMaxLength(3);

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Demarrage).HasMaxLength(50);

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Exploitant).HasMaxLength(50);

                entity.Property(e => e.Libelle).HasMaxLength(200);

                entity.Property(e => e.PresseSection).HasMaxLength(20);

                entity.Property(e => e.RefExt).HasMaxLength(50);

                entity.Property(e => e.SAGECodeComptable)
                    .HasMaxLength(17);

                entity.Property(e => e.SAGECompteTiers)
                    .HasMaxLength(13);

                entity.HasOne(d => d.EcoOrganisme)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefEcoOrganisme)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.EntiteType)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefEntiteType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.EntiteCamionTypes)
                    .WithOne()
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.EntiteDRs)
                    .WithOne()
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.EntiteProcesss)
                    .WithOne()
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.EntiteProduits)
                    .WithOne()
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.EntiteStandards)
                    .WithOne()
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.Actions)
                    .WithOne()
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.ContratIncitationQualites)
                    .WithOne()
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.ContratCollectivites)
                    .WithOne()
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.Repreneur)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefRepreneur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.RepriseType)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefRepriseType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.SAGECategorieAchat)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefSAGECategorieAchat)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.SAGECategorieVente)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefSAGECategorieVente)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.SAGEConditionLivraison)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefSAGEConditionLivraison)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.SAGEModeReglement)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefSAGEModeReglement)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.SAGEPeriodicite)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefSAGEPeriodicite)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Equipementier)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefEquipementier)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.FournisseurTO)
                    .WithMany(p => p.Entites)
                    .HasForeignKey(d => d.RefFournisseurTO)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.EntiteCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.EntiteModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EntiteCamionType>(entity =>
            {
                entity.HasKey(e => e.RefEntiteCamionType);
                entity.ToTable("tbmEntiteCamionType");
                entity.HasIndex(e => new { e.RefEntite, e.RefEntiteCamionType })
                    .HasDatabaseName("IX_tbmEntiteCamionType_Unique");

                entity.HasOne(d => d.CamionType)
                    .WithMany(p => p.EntiteCamionTypes)
                    .HasForeignKey(d => d.RefCamionType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.EntiteCamionTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EntiteDR>(entity =>
            {
                entity.HasKey(e => e.RefEntiteDR);
                entity.ToTable("tbmEntiteDR");
                entity.HasIndex(e => e.RefDR);
                entity.HasIndex(e => e.RefEntite);
                entity.HasOne(d => d.DR)
                    .WithMany(p => p.EntiteDRs)
                    .HasForeignKey(d => d.RefDR)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EntiteEntite>(entity =>
            {
                entity.HasKey(e => e.RefEntiteEntite);

                entity.ToTable("tbmEntiteEntite");

                entity.HasIndex(e => e.RefEntite);

                entity.HasIndex(e => e.RefEntiteRtt);

                entity.Property(e => e.Actif)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.EntiteRtt)
                    .WithMany(p => p.EntiteEntiteRttInits)
                    .HasForeignKey(d => d.RefEntiteRtt)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Entite)
                    .WithMany(p => p.EntiteEntiteInits)
                    .HasForeignKey(d => d.RefEntite)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.EntiteEntiteCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EntiteProcess>(entity =>
            {
                entity.HasKey(e => e.RefEntiteProcess);
                entity.ToTable("tbmEntiteProcess");
                entity.HasIndex(e => e.RefEntite);
                entity.HasIndex(e => e.RefProcess);
                entity.HasOne(d => d.Process)
                    .WithMany(p => p.EntiteProcesss)
                    .HasForeignKey(d => d.RefProcess)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EntiteProduit>(entity =>
            {
                entity.HasKey(e => e.RefEntiteProduit);
                entity.ToTable("tbmEntiteProduit");
                entity.HasIndex(e => e.RefEntite);
                entity.HasIndex(e => e.RefProduit);

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.EntiteProduits)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.EntiteProduitCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EntiteStandard>(entity =>
            {
                entity.HasKey(e => e.RefEntiteStandard);
                entity.ToTable("tbmEntiteStandard");
                entity.HasIndex(e => e.RefEntite);
                entity.HasIndex(e => e.RefStandard);
                entity.HasOne(d => d.Standard)
                    .WithMany(p => p.EntiteStandards)
                    .HasForeignKey(d => d.RefStandard)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EntiteType>(entity =>
            {
                entity.HasKey(e => e.RefEntiteType);

                entity.ToTable("tbrEntiteType");

                entity.Property(e => e.Libelle)
                    .HasMaxLength(50)
                    .HasComputedColumnSql("([LibelleFRFR])");

                entity.Property(e => e.LibelleENGB)
                    .HasMaxLength(50);

                entity.Property(e => e.LibelleFRFR)
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.EntiteTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.EntiteTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Equipementier>(entity =>
            {
                entity.HasKey(e => e.RefEquipementier);

                entity.ToTable("tbrEquipementier");

                entity.Property(e => e.RefEquipementier).ValueGeneratedOnAdd();

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.EquipementierCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.EquipementierModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Fonction>(entity =>
            {
                entity.HasKey(e => e.RefFonction);

                entity.ToTable("tbrFonction");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.FonctionCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.FonctionModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<FormeContact>(entity =>
            {
                entity.HasKey(e => e.RefFormeContact);

                entity.ToTable("tbrFormeContact");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.FormeContactCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.FormeContactModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<FournisseurTO>(entity =>
            {
                entity.HasKey(e => e.RefFournisseurTO);

                entity.ToTable("tbrFournisseurTO");

                entity.Property(e => e.RefFournisseurTO).ValueGeneratedOnAdd();

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.FournisseurTOCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.FournisseurTOModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<JourFerie>(entity =>
            {
                entity.HasKey(e => e.RefJourFerie);
                entity.ToTable("tblJourFerie");
                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");
                entity.Property(e => e.D)
                    .IsRequired();
                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.JourFerieCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.JourFerieModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Lien>(entity =>
            {
                entity.HasKey(e => e.RefLien);

                entity.ToTable("tblLien");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Utilisateur)
                    .WithMany(p => p.Liens)
                    .HasForeignKey(d => d.RefUtilisateur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.LienCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.RefLog);

                entity.ToTable("tblLog");

                entity.Property(e => e.DLogin)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Utilisateur)
                    .WithMany(p => p.Logs)
                    .HasForeignKey(d => d.RefUtilisateur)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.RefMessage);

                entity.ToTable("tblMessage");

                entity.Property(e => e.Actif)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Corps).IsRequired();

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DiffusionUnique)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.MessageType)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.RefMessageType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.MessageDiffusions)
                    .WithOne()
                    .HasForeignKey(d => d.RefMessage)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.MessageVisualisations)
                    .WithOne()
                    .HasForeignKey(d => d.RefMessage)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.MessageCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.MessageModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<MessageDiffusion>(entity =>
            {
                entity.HasKey(e => e.RefMessageDiffusion);

                entity.ToTable("tbmMessageDiffusion");

                entity.Property(e => e.RefHabilitation).HasMaxLength(50);

                entity.Property(e => e.RefModule).HasMaxLength(50);

                entity.HasOne(d => d.Utilisateur)
                    .WithMany(p => p.MessageDiffusions)
                    .HasForeignKey(d => d.RefUtilisateur)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<MessageType>(entity =>
            {
                entity.HasKey(e => e.RefMessageType);

                entity.ToTable("tbrMessageType");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.MessageTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.MessageTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<MessageVisualisation>(entity =>
            {
                entity.HasKey(e => e.RefMessageVisualisation);

                entity.ToTable("tbmMessageVisualisation");

                entity.Property(e => e.D)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Utilisateur)
                    .WithMany(p => p.MessageVisualisations)
                    .HasForeignKey(d => d.RefUtilisateur)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ModeTransportEE>(entity =>
            {
                entity.HasKey(e => e.RefModeTransportEE);

                entity.ToTable("tbrModeTransportEE");

                entity.Property(e => e.RefModeTransportEE).ValueGeneratedOnAdd();

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ModeTransportEECreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ModeTransportEEModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<MontantIncitationQualite>(entity =>
            {
                entity.HasKey(e => e.RefMontantIncitationQualite);

                entity.ToTable("tbrMontantIncitationQualite");

                entity.HasIndex(e => e.Annee)
                    .HasDatabaseName("IX_tbrMontantIncitationQualite_Unique")
                    .IsUnique();

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Montant).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.MontantIncitationQualiteCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.MontantIncitationQualiteModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<MotifAnomalieChargement>(entity =>
            {
                entity.HasKey(e => e.RefMotifAnomalieChargement);

                entity.ToTable("tbrMotifAnomalieChargement");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.MotifAnomalieChargementCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.MotifAnomalieChargementModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<MotifAnomalieClient>(entity =>
            {
                entity.HasKey(e => e.RefMotifAnomalieClient);

                entity.ToTable("tbrMotifAnomalieClient");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.MotifAnomalieClientCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.MotifAnomalieClientModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<MotifAnomalieTransporteur>(entity =>
            {
                entity.HasKey(e => e.RefMotifAnomalieTransporteur);

                entity.ToTable("tbrMotifAnomalieTransporteur");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.MotifAnomalieTransporteurCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.MotifAnomalieTransporteurModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<MotifCamionIncomplet>(entity =>
            {
                entity.HasKey(e => e.RefMotifCamionIncomplet);

                entity.ToTable("tbrMotifCamionIncomplet");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.MotifCamionIncompletCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.MotifCamionIncompletModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ParamEmail>(entity =>
            {
                entity.HasKey(e => e.RefParamEmail);

                entity.ToTable("tbsParamEmail");
                entity.Property(e => e.AREmailSujet).HasMaxLength(200);

                entity.Property(e => e.ARExpediteurEmail).HasMaxLength(200);

                entity.Property(e => e.ARExpediteurLibelle).HasMaxLength(200);

                entity.Property(e => e.ComptePOP).HasMaxLength(200);

                entity.Property(e => e.CompteSMTP).HasMaxLength(200);

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmailExpediteur).HasMaxLength(200);

                entity.Property(e => e.LibelleExpediteur).HasMaxLength(200);

                entity.Property(e => e.POP).HasMaxLength(200);

                entity.Property(e => e.PwdPOP).HasMaxLength(200);

                entity.Property(e => e.PwdSMTP).HasMaxLength(200);

                entity.Property(e => e.SMTP).HasMaxLength(200);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ParamEmailCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ParamEmailModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Parametre>(entity =>
            {
                entity.HasKey(e => e.RefParametre);

                entity.ToTable("tblParametre");

                entity.Property(e => e.Cmt).HasMaxLength(500);

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ParametreCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ParametreModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Parcours>(entity =>
            {
                entity.HasKey(e => e.RefParcours);
                entity.Property(e => e.RefParcours).ValueGeneratedOnAdd();

                entity.ToTable("tblParcours");

                entity.HasOne(d => d.AdresseOrigine)
                    .WithMany(p => p.ParcoursAdresseOrigines)
                    .HasForeignKey(d => d.RefAdresseOrigine)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.AdresseDestination)
                    .WithMany(p => p.ParcoursAdresseDestinations)
                    .HasForeignKey(d => d.RefAdresseDestination)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Pays>(entity =>
            {
                entity.HasKey(e => e.RefPays);

                entity.ToTable("tbrPays");

                entity.HasIndex(e => e.RefRegionReporting);

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LibelleCourt)
                    .IsRequired()
                    .HasMaxLength(3);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.PaysCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.PaysModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<PrixReprise>(entity =>
            {
                entity.HasKey(e => e.RefPrixReprise);

                entity.ToTable("tbrPrixReprise");

                entity.HasIndex(e => new { e.D, e.RefProcess, e.RefProduit, e.RefComposant })
                    .HasDatabaseName("IX_tbrPrixReprise_Unique")
                    .IsUnique();

                entity.Property(e => e.D).HasColumnType("datetime");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.PUHT)
                    .HasColumnName("PUHT")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PUHTSurtri)
                    .HasColumnName("PUHTSurtri")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PUHTTransport)
                    .HasColumnName("PUHTTransport")
                    .HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Contrat)
                    .WithMany(p => p.PrixReprises)
                    .HasForeignKey(d => d.RefContrat)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.PrixReprises)
                    .HasForeignKey(d => d.RefProcess)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.PrixReprises)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Composant)
                    .WithMany(p => p.PrixRepriseComposants)
                    .HasForeignKey(d => d.RefComposant)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.PrixRepriseCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.PrixRepriseModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Process>(entity =>
            {
                entity.HasKey(e => e.RefProcess);

                entity.ToTable("tbrProcess");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ProcessCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ProcessModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Produit>(entity =>
            {
                entity.HasKey(e => e.RefProduit);

                entity.ToTable("tblProduit");

                entity.Property(e => e.Actif)
                .HasDefaultValueSql("((0))");

                entity.Property(e => e.CodeEE)
                .HasMaxLength(50);

                entity.Property(e => e.CodeListeVerte).HasMaxLength(50);

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NomCommercial).HasMaxLength(50);

                entity.Property(e => e.NomCommun).HasMaxLength(50);

                entity.Property(e => e.NumeroStatistique).HasMaxLength(50);

                entity.Property(e => e._PCodeProd)
                    .HasMaxLength(4);

                entity.HasOne(d => d.SAGECodeTransport)
                    .WithMany(p => p.Produits)
                    .HasForeignKey(d => d.RefSAGECodeTransport)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ProduitGroupeReporting)
                    .WithMany(p => p.Produits)
                    .HasForeignKey(d => d.RefProduitGroupeReporting)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ApplicationProduitOrigine)
                    .WithMany(p => p.Produits)
                    .HasForeignKey(d => d.RefApplicationProduitOrigine)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.ProduitComposants)
                    .WithOne()
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.ProduitStandards)
                    .WithOne()
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ProduitCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ProduitModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ProduitComposant>(entity =>
            {
                entity.HasKey(e => e.RefProduitComposant);
                entity.ToTable("tbmProduitComposant");
                entity.HasIndex(e => e.RefProduit);
                entity.HasIndex(e => e.RefComposant);
                entity.HasOne(d => d.Composant)
                    .WithMany(p => p.Composants)
                    .HasForeignKey(d => d.RefComposant)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ProduitGroupeReporting>(entity =>
            {
                entity.HasKey(e => e.RefProduitGroupeReporting);

                entity.ToTable("tbrProduitGroupeReporting");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.ProduitGroupeReportingType)
                    .WithMany(p => p.ProduitGroupeReportingProduitGroupeReportingTypes)
                    .HasForeignKey(d => d.RefProduitGroupeReportingType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ProduitGroupeReportingCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ProduitGroupeReportingModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ProduitGroupeReportingType>(entity =>
            {
                entity.HasKey(e => e.RefProduitGroupeReportingType);

                entity.ToTable("tbrProduitGroupeReportingType");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ProduitGroupeReportingTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ProduitGroupeReportingTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ProduitStandard>(entity =>
            {
                entity.HasKey(e => e.RefProduitStandard);
                entity.ToTable("tbmProduitStandard");
                entity.HasIndex(e => e.RefProduit);
                entity.HasIndex(e => e.RefStandard);
                entity.HasOne(d => d.Standard)
                    .WithMany(p => p.ProduitStandards)
                    .HasForeignKey(d => d.RefStandard)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<NonConformite>(entity =>
            {
                entity.HasKey(e => e.RefNonConformite);

                entity.ToTable("tblNonConformite");

                entity.Property(e => e.PlanAction)
                    .HasDefaultValueSql("((1))"); 
                
                entity.HasIndex(e => e.RefCommandeFournisseur);

                entity.HasIndex(e => e.RefNonConformiteNature);

                entity.HasOne(d => d.CommandeFournisseur)
                    .WithMany(p => p.NonConformites)
                    .HasForeignKey(d => d.RefCommandeFournisseur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.NonConformiteDemandeClientType)
                    .WithMany(p => p.NonConformites)
                    .HasForeignKey(d => d.RefNonConformiteDemandeClientType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.NonConformiteNature)
                    .WithMany(p => p.NonConformites)
                    .HasForeignKey(d => d.RefNonConformiteNature)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.NonConformiteReponseClientType)
                    .WithMany(p => p.NonConformites)
                    .HasForeignKey(d => d.RefNonConformiteReponseClientType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.NonConformiteReponseFournisseurType)
                    .WithMany(p => p.NonConformites)
                    .HasForeignKey(d => d.RefNonConformiteReponseFournisseurType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.NonConformiteAccordFournisseurType)
                    .WithMany(p => p.NonConformites)
                    .HasForeignKey(d => d.RefNonConformiteAccordFournisseurType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.NonConformiteNonConformiteFamilles)
                    .WithOne()
                    .HasForeignKey(d => d.RefNonConformite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.NonConformiteFichiers)
                    .WithOne()
                    .HasForeignKey(d => d.RefNonConformite)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.NonConformiteEtapes)
                    .WithOne()
                    .HasForeignKey(d => d.RefNonConformite)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });
            modelBuilder.Entity<NonConformiteAccordFournisseurType>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteAccordFournisseurType);

                entity.ToTable("tbrNonConformiteAccordFournisseurType");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.NonConformiteAccordFournisseurTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.NonConformiteAccordFournisseurTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<NonConformiteDemandeClientType>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteDemandeClientType);

                entity.ToTable("tbrNonConformiteDemandeClientType");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.NonConformiteDemandeClientTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.NonConformiteDemandeClientTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<NonConformiteEtape>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteEtape);

                entity.ToTable("tblNonConformiteEtape");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.NonConformiteEtapeType)
                    .WithMany(p => p.NonConformiteEtapes)
                    .HasForeignKey(d => d.RefNonConformiteEtapeType)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.NonConformiteEtapeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.NonConformiteEtapeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurControle)
                    .WithMany(p => p.NonConformiteEtapeControles)
                    .HasForeignKey(d => d.RefUtilisateurControle)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurValide)
                    .WithMany(p => p.NonConformiteEtapeValides)
                    .HasForeignKey(d => d.RefUtilisateurValide)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<NonConformiteEtapeType>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteEtapeType);

                entity.ToTable("tbsNonConformiteEtapeType");
            });
            modelBuilder.Entity<NonConformiteFamille>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteFamille);

                entity.ToTable("tbrNonConformiteFamille");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.NonConformiteFamilleCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.NonConformiteFamilleModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<NonConformiteFichier>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteFichier);

                entity.ToTable("tblNonConformiteFichier");

                entity.HasIndex(e => e.RefNonConformite);

                entity.Property(e => e.Corps).IsRequired();

                entity.Property(e => e.Extension).HasMaxLength(50);

                entity.Property(e => e.Nom).HasMaxLength(250);

                entity.HasOne(d => d.NonConformiteFichierType)
                    .WithMany(p => p.NonConformiteFichiers)
                    .HasForeignKey(d => d.RefNonConformiteFichierType)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<NonConformiteFichierType>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteFichierType);

                entity.ToTable("tbsNonConformiteFichierType");

            });
            modelBuilder.Entity<NonConformiteNature>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteNature);

                entity.ToTable("tbrNonConformiteNature");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.NonConformiteNatureCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.NonConformiteNatureModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<NonConformiteNonConformiteFamille>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteNonConformiteFamille);

                entity.ToTable("tbmNonConformiteNonConformiteFamille");

                entity.HasIndex(e => e.RefNonConformite);

                entity.HasIndex(e => e.RefNonConformiteFamille);

                entity.HasIndex(e => new { e.RefNonConformite, e.RefNonConformiteFamille })
                    .HasDatabaseName("IX_tbmNonConformiteNonConformiteFamille_Unique")
                    .IsUnique();

                entity.HasOne(d => d.NonConformiteFamille)
                    .WithMany(p => p.NonConformiteNonConformiteFamilles)
                    .HasForeignKey(d => d.RefNonConformiteFamille)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<NonConformiteReponseClientType>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteReponseClientType);

                entity.ToTable("tbrNonConformiteReponseClientType");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.NonConformiteReponseClientTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.NonConformiteReponseClientTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<NonConformiteReponseFournisseurType>(entity =>
            {
                entity.HasKey(e => e.RefNonConformiteReponseFournisseurType);

                entity.ToTable("tbrNonConformiteReponseFournisseurType");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.NonConformiteReponseFournisseurTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.NonConformiteReponseFournisseurTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RegionEE>(entity =>
            {
                entity.HasKey(e => e.RefRegionEE);

                entity.ToTable("tbrRegionEE");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasMany(d => d.RegionEEDpts)
                .WithOne()
                .HasForeignKey(d => d.RefRegionEE)
                .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.RegionEECreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.RegionEEModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RegionEEDpt>(entity =>
            {
                entity.HasKey(e => e.RefRegionEEDpt);

                entity.ToTable("tbmRegionEEDpt");

                entity.HasIndex(e => e.RefDpt);

                entity.HasIndex(e => e.RefRegionEE);

                entity.Property(e => e.RefDpt)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.HasOne(d => d.Dpt)
                    .WithMany(p => p.RegionEEDpts)
                    .HasForeignKey(d => d.RefDpt)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RegionReporting>(entity =>
            {
                entity.HasKey(e => e.RefRegionReporting);
                entity.ToTable("tbrRegionReporting");
                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");
                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.RegionReportingCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.RegionReportingModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Repartition>(entity =>
            {
                entity.HasKey(e => e.RefRepartition);

                entity.ToTable("tblRepartition");

                entity.HasIndex(e => e.RefCommandeFournisseur);

                entity.HasIndex(e => e.RefFournisseur);

                entity.HasIndex(e => e.RefProduit);

                entity.Property(e => e.D).HasColumnType("datetime");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.HasMany(d => d.RepartitionCollectivites)
                    .WithOne()
                    .HasForeignKey(d => d.RefRepartition)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasMany(d => d.RepartitionProduits)
                    .WithOne()
                    .HasForeignKey(d => d.RefRepartition)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.Fournisseur)
                    .WithMany(p => p.Repartitions)
                    .HasForeignKey(d => d.RefFournisseur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CommandeFournisseur)
                    .WithMany(p => p.Repartitions)
                    .HasForeignKey(d => d.RefCommandeFournisseur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.Repartitions)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.RepartitionCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.RepartitionModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RepartitionCollectivite>(entity =>
            {
                entity.HasKey(e => e.RefRepartitionCollectivite);

                entity.ToTable("tblRepartitionCollectivite");

                entity.Property(e => e.PUHT)
                    .HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.RepartitionCollectivites)
                    .HasForeignKey(d => d.RefProcess)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.RepartitionCollectivites)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Collectivite)
                    .WithMany(p => p.RepartitionCollectivites)
                    .HasForeignKey(d => d.RefCollectivite)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RepartitionDetail>(entity =>
            {
                entity.ToView("VueRepartitionDetail");
                entity.HasNoKey();
                entity.Property(e => e.PUHT)
                    .HasColumnType("decimal(10, 2)");
            });
            modelBuilder.Entity<Repreneur>(entity =>
            {
                entity.HasKey(e => e.RefRepreneur);

                entity.ToTable("tbrRepreneur");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.RepreneurCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.RepreneurModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RepriseType>(entity =>
            {
                entity.HasKey(e => e.RefRepriseType);

                entity.ToTable("tbrRepriseType");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.RepriseTypeCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.RepriseTypeModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RepartitionProduit>(entity =>
            {
                entity.HasKey(e => e.RefRepartitionProduit);

                entity.ToTable("tblRepartitionProduit");

                entity.Property(e => e.PUHT)
                    .HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.RepartitionProduits)
                    .HasForeignKey(d => d.RefProcess)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Produit)
                    .WithMany(p => p.RepartitionProduits)
                    .HasForeignKey(d => d.RefProduit)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Fournisseur)
                    .WithMany(p => p.RepartitionProduits)
                    .HasForeignKey(d => d.RefFournisseur)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<SAGECategorieAchat>(entity =>
            {
                entity.HasKey(e => e.RefSAGECategorieAchat);

                entity.ToTable("tbrSAGECategorieAchat");

                entity.Property(e => e.RefSAGECategorieAchat)
                    .ValueGeneratedNever();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TVATaux)
                    .HasColumnType("decimal(10, 2)");
            });
            modelBuilder.Entity<SAGECategorieVente>(entity =>
            {
                entity.HasKey(e => e.RefSAGECategorieVente);

                entity.ToTable("tbrSAGECategorieVente");

                entity.Property(e => e.RefSAGECategorieVente)
                    .ValueGeneratedNever();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TVATaux)
                    .HasColumnType("decimal(10, 2)");
            });
            modelBuilder.Entity<SAGECodeTransport>(entity =>
            {
                entity.HasKey(e => e.RefSAGECodeTransport);

                entity.ToTable("tbrSAGECodeTransport");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.SAGECodeTransportCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.SAGECodeTransportModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<SAGEConditionLivraison>(entity =>
            {
                entity.HasKey(e => e.RefSAGEConditionLivraison);

                entity.ToTable("tbrSAGEConditionLivraison");

                entity.Property(e => e.RefSAGEConditionLivraison)
                    .ValueGeneratedNever();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<SAGEDocument>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.ToTable("F_DOCENTETE");

                entity.Property(e => e.ID)
                    .HasColumnName("cbMarq")
                    .ValueGeneratedNever();

                entity.Property(e => e.RefSAGEDocument).HasColumnName("DO_PIECE");
                entity.Property(e => e.D).HasColumnName("DO_DATE");
                entity.Property(e => e.CodeComptable).HasColumnName("DO_TIERS");
                entity.Property(e => e.ConditionLivraison).HasColumnName("INT_CONDITION");
                entity.Property(e => e.TotalHTBdd).HasColumnName("FNT_TOTALHT");
                entity.Property(e => e.TotalTTCBdd).HasColumnName("FNT_TOTALTTC");
                entity.Property(e => e.TotalNetBdd).HasColumnName("FNT_NETAPAYER");
                entity.Property(e => e.TotalTVABdd).HasColumnName("FNT_MONTANTTOTALTAXES");
                entity.Property(e => e.Adr1).HasColumnName("ADRESSE1_:");
                entity.Property(e => e.Adr2).HasColumnName("ADRESSE2_:");
                entity.Property(e => e.Adr3).HasColumnName("ADRESSE3_:");
                entity.Property(e => e.Adr4).HasColumnName("ADRESSE4_:");
                entity.Property(e => e.DocumentType).HasColumnName("DO_TYPE");

                entity.HasMany(d => d.SAGEDocumentLignes)
                    .WithOne()
                    .HasPrincipalKey(p => new { p.RefSAGEDocument, p.DocumentType })
                    .HasForeignKey(d => new { d.RefSAGEDocument, d.DocumentType });

                entity.HasMany(d => d.SAGEDocumentReglements)
                    .WithOne()
                    .HasPrincipalKey(p => p.RefSAGEDocument)
                    .HasForeignKey(d => d.RefSAGEDocument);

                entity.HasOne(d => d.SAGEDocumentCompteTVA)
                    .WithMany(p => p.SAGEDocuments)
                    .HasForeignKey(d => d.CodeComptable)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<SAGEDocumentArticle>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.ToTable("F_ARTICLE");

                entity.Property(e => e.ID)
                    .HasColumnName("cbMarq")
                    .ValueGeneratedNever();

                entity.Property(e => e.RefSAGEDocumentArticle).HasColumnName("AR_REF");
                entity.Property(e => e.LibelleAR).HasColumnName("AR_DESIGN");
                entity.Property(e => e.Designation1).HasColumnName("AR_LANGUE1");
                entity.Property(e => e.Designation2).HasColumnName("AR_LANGUE2");
            });
            modelBuilder.Entity<SAGEDocumentCompteTVA>(entity =>
            {
                entity.HasKey(e => e.RefSAGEDocumentCompteTVA);

                entity.ToTable("F_COMPTET");

                entity.Property(e => e.RefSAGEDocumentCompteTVA)
                    .HasColumnName("CT_Num")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompteTVA)
                    .HasColumnName("CT_Identifiant")
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<SAGEDocumentReglement>(entity =>
            {
                entity.HasKey(e => new { e.RefSAGEDocument, e.Lettrage });

                entity.ToTable("F_ECRITUREC");

                entity.Property(e => e.RefSAGEDocument).HasColumnName("EC_RefPiece");
                entity.Property(e => e.Lettrage).HasColumnName("EC_Lettrage");
                entity.Property(e => e.Nb).HasColumnName("Nb");
            });
            modelBuilder.Entity<SAGEDocumentLigne>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.ToTable("F_DOCLIGNE");

                entity.Property(e => e.ID)
                    .HasColumnName("cbMarq")
                    .ValueGeneratedNever();

                entity.Property(e => e.RefSAGEDocument).HasColumnName("DO_PIECE");
                entity.Property(e => e.RefSAGEDocumentArticle).HasColumnName("AR_REF");
                entity.Property(e => e.LibelleDL).HasColumnName("DL_DESIGN");
                entity.Property(e => e.QuantiteBdd).HasColumnName("DL_QTE");
                entity.Property(e => e.PUHTBdd).HasColumnName("DL_PRIXUNITAIRE");
                entity.Property(e => e.TotalHTBdd).HasColumnName("DL_MONTANTHT");
                entity.Property(e => e.TotalTTCBdd).HasColumnName("DL_MONTANTTTC");
                entity.Property(e => e.TVAMontantBdd).HasColumnName("FNT_MONTANTTAXES");
                entity.Property(e => e.TVATauxBdd).HasColumnName("DL_TAXE1");
                entity.Property(e => e.DocumentType).HasColumnName("DO_TYPE");
                entity.Property(e => e.RefLigne).HasColumnName("DT_NO");

                entity.HasOne(d => d.SAGEDocumentArticle)
                    .WithMany(p => p.SAGEDocumentLignes)
                    .HasPrincipalKey(p => p.RefSAGEDocumentArticle)
                    .HasForeignKey(d => d.RefSAGEDocumentArticle);

                entity.HasOne(d => d.SAGEDocumentLigneText)
                    .WithMany(p => p.SAGEDocumentLignes)
                    .HasPrincipalKey(p => p.RefLigne)
                    .HasForeignKey(d => d.RefLigne)
                    .IsRequired(false);
            });
            modelBuilder.Entity<SAGEDocumentLigneText>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.ToTable("F_DOCLIGNETEXT");

                entity.Property(e => e.ID)
                    .HasColumnName("cbMarq")
                    .ValueGeneratedNever();

                entity.Property(e => e.RefLigne).HasColumnName("DT_NO");
                entity.Property(e => e.Cmt).HasColumnName("DT_Text");
            });
            modelBuilder.Entity<SAGEModeReglement>(entity =>
            {
                entity.HasKey(e => e.RefSAGEModeReglement);

                entity.ToTable("tbrSAGEModeReglement");

                entity.Property(e => e.RefSAGEModeReglement)
                    .ValueGeneratedNever();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<SAGEPeriodicite>(entity =>
            {
                entity.HasKey(e => e.RefSAGEPeriodicite);

                entity.ToTable("tbrSAGEPeriodicite");

                entity.Property(e => e.RefSAGEPeriodicite)
                    .ValueGeneratedNever();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<Securite>(entity =>
            {
                entity.HasKey(e => e.RefSecurite);

                entity.ToTable("tblSecurite");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.DelaiAvantDesactivationUtilisateur)
                    .IsRequired()
                    .HasDefaultValue();

                entity.Property(e => e.DelaiAvantChangementMotDePasse)
                    .IsRequired()
                    .HasDefaultValue();

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.SecuriteCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.SecuriteModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.RefService);

                entity.ToTable("tbrService");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.ServiceCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.ServiceModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<SessionPop>(entity =>
            {
                entity.HasKey(e => e.RefSessionPop);

                entity.ToTable("tbsSessionPOP");

                entity.Property(e => e.ComptePOP).HasMaxLength(200);

                entity.Property(e => e.DDebut)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Etat).HasMaxLength(200);

                entity.Property(e => e.Libelle).HasMaxLength(200);

                entity.Property(e => e.NbEMail).HasDefaultValueSql("((0))");

                entity.Property(e => e.POP).HasMaxLength(200);

                entity.HasOne(d => d.ParamEmail)
                    .WithMany(p => p.SessionPops)
                    .HasForeignKey(d => d.RefParamEmail)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Standard>(entity =>
            {
                entity.HasKey(e => e.RefStandard);
                entity.ToTable("tbrStandard");
                entity.Property(e => e.Cmt).HasColumnType("ntext");
                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.StandardCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.StandardModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<SurcoutCarburant>(entity =>
            {
                entity.HasKey(e => e.RefSurcoutCarburant);

                entity.ToTable("tblSurcoutCarburant");

                entity.HasIndex(e => new { e.RefTransporteur, e.RefPays, e.Annee, e.Mois })
                    .HasDatabaseName("IX_tbrPrixReprise_Unique")
                    .IsUnique();


                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ratio)
                    .HasColumnName("Ratio")
                    .HasColumnType("decimal(4, 2)");

                entity.HasOne(d => d.Transporteur)
                    .WithMany(p => p.SurcoutCarburantTransporteurs)
                    .HasForeignKey(d => d.RefTransporteur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Pays)
                    .WithMany(p => p.SurcoutCarburants)
                    .HasForeignKey(d => d.RefPays)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.SurcoutCarburantCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.SurcoutCarburantModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.RefTicket);

                entity.ToTable("tblTicket");

                entity.HasIndex(e => e.RefPays);

                entity.Property(e => e.TicketTexte)
                .HasColumnName("Ticket");

                entity.Property(e => e.DCreation)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Libelle).HasMaxLength(200);

                entity.HasOne(d => d.Pays)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.RefPays)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.TicketCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.TicketModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Titre>(entity =>
            {
                entity.HasKey(e => e.RefTitre);

                entity.ToTable("tbrTitre");

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.TitreCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.TitreModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Transport>(entity =>
            {
                entity.HasKey(e => e.RefTransport);

                entity.ToTable("tblTransport");

                entity.Property(e => e.RefTransport).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.RefParcours);

                entity.HasIndex(e => e.RefTransporteur)
                    .HasDatabaseName("IX_tblTransport_RefEntite");

                entity.HasIndex(e => new { e.RefParcours, e.RefTransporteur, e.RefCamionType })
                    .HasDatabaseName("IX_tblTransport_Unique")
                    .IsUnique();

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.PUHT)
                    .HasColumnName("PUHT")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PUHTDemande)
                    .HasColumnName("PUHTDemande")
                    .HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.TransportCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.TransportModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Parcours)
                    .WithMany(p => p.Transports)
                    .HasForeignKey(d => d.RefParcours)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Transporteur)
                    .WithMany(p => p.Transports)
                    .HasForeignKey(d => d.RefTransporteur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CamionType)
                    .WithMany(p => p.Transports)
                    .HasForeignKey(d => d.RefCamionType)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Utilisateur>(entity =>
            {
                entity.HasKey(e => e.RefUtilisateur);

                entity.ToTable("tblUtilisateur");

                entity.Property(e => e.RefUtilisateur).ValueGeneratedOnAdd();

                entity.Property(e => e.DCreation)
                    .HasColumnName("DCreation")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DModif)
                    .HasColumnName("DModif")
                    .HasColumnType("datetime");

                entity.Property(e => e.EMail)
                    .HasColumnName("EMail")
                    .HasMaxLength(200);

                entity.Property(e => e.Fax).HasMaxLength(20);

                entity.Property(e => e.HabilitationAdministration).HasMaxLength(50);

                entity.Property(e => e.HabilitationAnnuaire).HasMaxLength(50);

                entity.Property(e => e.HabilitationBDC).HasMaxLength(50);

                entity.Property(e => e.HabilitationLogistique).HasMaxLength(50);

                entity.Property(e => e.HabilitationMessagerie).HasMaxLength(50);


                entity.Property(e => e.HabilitationQualite).HasMaxLength(50);

                entity.Property(e => e.HabilitationModuleCollectivite).HasMaxLength(50);

                entity.Property(e => e.HabilitationModuleTransporteur).HasMaxLength(50);

                entity.Property(e => e.HabilitationStatistique).HasMaxLength(50);

                entity.Property(e => e.Login)
                    //.IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Pwd)
                    //.IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Tel).HasMaxLength(20);

                entity.Property(e => e.TelMobile).HasMaxLength(20);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.UtilisateurClients)
                    .HasForeignKey(d => d.RefClient)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Transporteur)
                    .WithMany(p => p.UtilisateurTransporteurs)
                    .HasForeignKey(d => d.RefTransporteur)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CentreDeTri)
                    .WithMany(p => p.UtilisateurCentreDeTris)
                    .HasForeignKey(d => d.RefCentreDeTri)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Collectivite)
                    .WithMany(p => p.UtilisateurCollectivites)
                    .HasForeignKey(d => d.RefCollectivite)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Prestataire)
                    .WithMany(p => p.UtilisateuPrestataires)
                    .HasForeignKey(d => d.RefPrestataire)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.UtilisateurAPIs)
                    .WithOne()
                    .HasForeignKey(d => d.RefUtilisateur)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.Pays)
                    .WithMany(p => p.Utilisateurs)
                    .HasForeignKey(d => d.RefPays)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ContactAdresse)
                    .WithMany(p => p.Utilisateurs)
                    .HasForeignKey(d => d.RefContactAdresse)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurMaitre)
                    .WithMany(p => p.UtilisateurMaitres)
                    .HasForeignKey(d => d.RefUtilisateurMaitre)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurAffectationMaitre)
                    .WithMany(p => p.UtilisateurAffectationMaitres)
                    .HasForeignKey(d => d.RefUtilisateurAffectationMaitre)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurCreation)
                    .WithMany(p => p.UtilisateurCreations)
                    .HasForeignKey(d => d.RefUtilisateurCreation)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UtilisateurModif)
                    .WithMany(p => p.UtilisateurModifs)
                    .HasForeignKey(d => d.RefUtilisateurModif)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<UtilisateurAPI>(entity =>
            {
                entity.HasKey(e => e.RefUtilisateurAPI);

                entity.ToTable("tbmUtilisateurAPI");

            });
            modelBuilder.Entity<Verrouillage>(entity =>
            {
                entity.HasKey(e => e.RefVerrouillage);

                entity.ToTable("tbsVerrouillage");

                entity.HasIndex(e => e.Donnee);

                entity.HasIndex(e => e.DVerrouillage)
                    .HasDatabaseName("IX_tbsBlocage_DVerrouillage");

                entity.HasIndex(e => e.RefDonnee)
                    .HasDatabaseName("IX_tbsBlocage_RefDonnee");

                entity.HasIndex(e => e.RefUtilisateur)
                    .HasDatabaseName("IX_tbsBlocage_RefUtilisateur");

                entity.Property(e => e.Donnee)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DVerrouillage)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Utilisateur)
                    .WithMany(p => p.Verrouillages)
                    .HasForeignKey(d => d.RefUtilisateur)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        #endregion Models
    }
}