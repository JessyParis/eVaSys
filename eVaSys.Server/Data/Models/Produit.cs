/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 13/03/2019
/// -----------------------------------------------------------------------------------------------------
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for produit
    /// </summary>
    public class Produit : IMarkModification
    {
        #region Constructor

        public Produit()
        {
        }

        private Produit(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        #endregion Constructor

        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefProduit { get; set; }
        public string _PCodeProd { get; set; }
        public string Libelle { get; set; }
        public string NomCommercial { get; set; }
        public string NomCommun { get; set; }
        public bool Collecte { get; set; } = false;
        public string NumeroStatistique { get; set; }
        public int? RefProduitGroupeReporting { get; set; }
        private ProduitGroupeReporting _produitGroupeReporting;
        public ProduitGroupeReporting ProduitGroupeReporting
        {
            get => LazyLoader.Load(this, ref _produitGroupeReporting);
            set => _produitGroupeReporting = value;
        }
        public int? RefSAGECodeTransport { get; set; }
        private SAGECodeTransport _sAGECodeTransport;

        public SAGECodeTransport SAGECodeTransport
        {
            get => LazyLoader.Load(this, ref _sAGECodeTransport);
            set => _sAGECodeTransport = value;
        }
        public bool? Actif { get; set; } = false;
        public int? RefRepreneur { get; set; }
        public int? RefApplicationProduitOrigine { get; set; }
        private ApplicationProduitOrigine _applicationProduitOrigine;
        public ApplicationProduitOrigine ApplicationProduitOrigine
        {
            get => LazyLoader.Load(this, ref _applicationProduitOrigine);
            set => _applicationProduitOrigine = value;
        }
        public bool Composant { get; set; }
        public bool PUHTSurtri { get; set; } = false;
        public bool PUHTTransport { get; set; } = false;
        public string CodeListeVerte { get; set; }
        public string CodeEE { get; set; }
        public bool IncitationQualite { get; set; } = false;
        public string Cmt { get; set; }
        public int? Co2KgParT { get; set; }
        public string CmtFournisseur { get; set; }
        public string CmtTransporteur { get; set; }
        public string CmtClient { get; set; }
        public string LaserType { get; set; }
        public ICollection<ProduitComposant> Composants { get; set; }
        public ICollection<ProduitComposant> ProduitComposants { get; set; }
        public ICollection<ProduitStandard> ProduitStandards { get; set; }
        public ICollection<CommandeClient> CommandeClients { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurs { get; set; }
        public ICollection<DescriptionControleProduit> DescriptionControleProduits { get; set; }
        public ICollection<DescriptionCVQProduit> DescriptionCVQProduits { get; set; }
        public ICollection<EntiteProduit> EntiteProduits { get; set; }
        public ICollection<FicheControle> FicheControles { get; set; }
        public ICollection<PrixReprise> PrixRepriseComposants { get; set; }
        public ICollection<PrixReprise> PrixReprises { get; set; }
        public ICollection<RepartitionCollectivite> RepartitionCollectivites { get; set; }
        public ICollection<RepartitionProduit> RepartitionProduits { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }

        [NotMapped]
        public int RefUtilisateurCourant { get; set; }

        public int RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        public int? RefUtilisateurModif { get; set; }
        public Utilisateur UtilisateurModif { get; set; }

        public string CreationText
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (RefUtilisateurCreation <= 0) { s = cR.GetTextRessource(9); }
                if (UtilisateurCreation != null)
                {
                    s = cR.GetTextRessource(388) + " " + DCreation.ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurCreation.Nom;
                }
                return s;
            }
        }

        public string ModificationText
        {
            get
            {
                string s = "";
                if (UtilisateurModif != null && DModif != null)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    s = cR.GetTextRessource(389) + " " + ((DateTime)DModif).ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurModif.Nom;
                }
                return s;
            }
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Mark modifications
        /// </summary>
        public void MarkModification(bool add)
        {
            if (add)
            {
                RefUtilisateurCreation = RefUtilisateurCourant;
                DCreation = DateTime.Now;
            }
            else
            {
                RefUtilisateurModif = RefUtilisateurCourant;
                DModif = DateTime.Now;
            }
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            //bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tblTransport where RefTransport=" + RefTransport, DbContext.Database.GetDbConnection()) != "0");
            //int c = DbContext.Transports.Where(q => (q.RefParcours == RefParcours && q.RefTransporteur == RefTransporteur && q.RefCamionType == RefCamionType)).Count();
            //if ((!existsInDB && c > 0) || (existsInDB && c > 1)) { if (r == "") { r += Environment.NewLine; } r += "Un transport identique existe déjà."; }
            return r;
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if linked data exist.
        /// </summary>
        public string IsDeletable()
        {
            string r = "";
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.CommandeFournisseurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.EntiteProduits).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeClients).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DescriptionControleProduits).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.DescriptionCVQProduits).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FicheControles).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.PrixReprises).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.PrixRepriseComposants).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepartitionCollectivites).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepartitionProduits).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProduitComposants).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ProduitStandards).Query().Count();
            if (nbLinkedData != 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "") { r += Environment.NewLine; }
                r += cR.GetTextRessource(393);
            }
            return r;
        }
    }
}