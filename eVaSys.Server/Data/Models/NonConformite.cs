/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 22/07/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for NonConformite
    /// </summary>
    public class NonConformite
    {
        #region Constructor
        public NonConformite()
        {
        }
        private NonConformite(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefNonConformite { get; set; }
        public int? RefCommandeFournisseur { get; set; }
        private CommandeFournisseur _commandeFournisseur;
        public CommandeFournisseur CommandeFournisseur
        {
            get => LazyLoader.Load(this, ref _commandeFournisseur);
            set => _commandeFournisseur = value;
        }
        public string DescrClient { get; set; }
        public int? RefNonConformiteDemandeClientType { get; set; }
        private NonConformiteDemandeClientType _nonConformiteDemandeClientType;
        public NonConformiteDemandeClientType NonConformiteDemandeClientType
        {
            get => LazyLoader.Load(this, ref _nonConformiteDemandeClientType);
            set => _nonConformiteDemandeClientType = value;
        }
        public string DescrValorplast { get; set; }
        public int? RefNonConformiteNature { get; set; }
        private NonConformiteNature _nonConformiteNature;
        public NonConformiteNature NonConformiteNature
        {
            get => LazyLoader.Load(this, ref _nonConformiteNature);
            set => _nonConformiteNature = value;
        }
        public DateTime? DTransmissionFournisseur { get; set; }
        public string ActionDR { get; set; }
        public int? RefNonConformiteReponseFournisseurType { get; set; }
        private NonConformiteReponseFournisseurType _nonConformiteReponseFournisseurType;
        public NonConformiteReponseFournisseurType NonConformiteReponseFournisseurType
        {
            get => LazyLoader.Load(this, ref _nonConformiteReponseFournisseurType);
            set => _nonConformiteReponseFournisseurType = value;
        }
        public string CmtReponseFournisseur { get; set; }
        public int? RefNonConformiteReponseClientType { get; set; }
        private NonConformiteReponseClientType _nonConformiteReponseClientType;
        public NonConformiteReponseClientType NonConformiteReponseClientType
        {
            get => LazyLoader.Load(this, ref _nonConformiteReponseClientType);
            set => _nonConformiteReponseClientType = value;
        }
        public string CmtReponseClient { get; set; }
        public bool PlanAction { get; set; } = true;
        public string CmtOrigineAction { get; set; }
        public bool PriseEnCharge { get; set; }
        public decimal? MontantPriseEnCharge { get; set; }
        public string CmtPriseEnCharge { get; set; }
        public string IFFournisseurDescr { get; set; }
        public decimal? IFFournisseurFactureMontant { get; set; }
        public int? IFFournisseurDeductionTonnage { get; set; }
        public bool IFFournisseurRetourLot { get; set; }
        public int? RefNonConformiteAccordFournisseurType { get; set; }
        private NonConformiteAccordFournisseurType _nonConformiteAccordFournisseurType;
        public NonConformiteAccordFournisseurType NonConformiteAccordFournisseurType
        {
            get => LazyLoader.Load(this, ref _nonConformiteAccordFournisseurType);
            set => _nonConformiteAccordFournisseurType = value;
        }
        public bool IFFournisseurAttenteBonCommande { get; set; }
        public string IFFournisseurBonCommandeNro { get; set; }
        public bool IFFournisseurFacture { get; set; }
        public bool IFFournisseurTransmissionFacturation { get; set; }
        public string IFFournisseurFactureNro { get; set; }
        public string IFFournisseurCmtFacturation { get; set; }
        public string IFClientDescr { get; set; }
        public decimal? IFClientFactureMontant { get; set; }
        public string IFClientFactureNro { get; set; }
        public string IFClientCmtFacturation { get; set; }
        public DateTime? IFClientDFacture { get; set; }
        public bool IFClientCommandeAFaire { get; set; }
        public bool IFClientFactureEnAttente { get; set; }
        public ICollection<NonConformiteNonConformiteFamille> NonConformiteNonConformiteFamilles { get; set; }
        public ICollection<NonConformiteFichier> NonConformiteFichiers { get; set; }
        public ICollection<NonConformiteEtape> NonConformiteEtapes { get; set; }
        [NotMapped]
        public string NonConformiteTextCmd
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = cR.GetTextRessource(319);
                try
                {
                    s = cR.GetTextRessource(14) + " : " + Utils.Utils.FormatNumeroCommande(CommandeFournisseur.NumeroCommande.ToString());
                    s += Environment.NewLine;
                    s += cR.GetTextRessource(19) + " : " + CommandeFournisseur.Produit.Libelle;
                    s += Environment.NewLine;
                    s += cR.GetTextRessource(764) + " : ";
                    string dr = "";
                    DbContext.Entry(CommandeFournisseur.Entite).Collection(r => r.EntiteDRs).Load();
                    if (CommandeFournisseur.Entite.EntiteDRs != null)
                    {
                        foreach (var eDR in CommandeFournisseur.Entite.EntiteDRs)
                        {
                            dr += eDR.DR.Nom + ", ";
                        }
                        if (!string.IsNullOrEmpty(dr))
                        {
                            dr = dr.Remove(dr.Length - 2);
                            s += dr;
                        }
                    }
                }
                catch { }
                return s;
            }
        }
        [NotMapped]
        public string NonConformiteTextClient
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = cR.GetTextRessource(319);
                Entite client = null;
                try
                {
                    client = DbContext.Entites.Find(CommandeFournisseur.AdresseClient.RefEntite);
                    s = cR.GetTextRessource(16) + " : " + client.Libelle;
                    if (!string.IsNullOrWhiteSpace(CommandeFournisseur.AdresseClient?.Adr1))
                    {
                        s += Environment.NewLine;
                        s += "  " + CommandeFournisseur.AdresseClient.Adr1;
                    }
                    if (!string.IsNullOrWhiteSpace(CommandeFournisseur.CodePostal)
                        || !string.IsNullOrWhiteSpace(CommandeFournisseur.Ville)
                        || !string.IsNullOrWhiteSpace(CommandeFournisseur.Pays?.LibelleCourt)
                        )
                    {
                        s += Environment.NewLine + "  ";
                        if (!string.IsNullOrWhiteSpace(CommandeFournisseur.AdresseClient?.CodePostal))
                        {
                            s += CommandeFournisseur.AdresseClient.CodePostal + " ";
                        }
                        if (!string.IsNullOrWhiteSpace(CommandeFournisseur.AdresseClient?.Ville))
                        {
                            s += CommandeFournisseur.AdresseClient.Ville + " ";
                        }
                        if (!string.IsNullOrWhiteSpace(CommandeFournisseur.AdresseClient?.Pays?.LibelleCourt))
                        {
                            s += "(" + CommandeFournisseur.AdresseClient.Pays.LibelleCourt + ")";
                        }
                    }
                    s += Environment.NewLine;
                    s += cR.GetTextRessource(277) + " : " + CommandeFournisseur.DDechargement?.ToString("dd/MM/yyyy") ?? cR.GetTextRessource(771);
                    s += Environment.NewLine;
                    s += cR.GetTextRessource(231) + " : " + (CommandeFournisseur.PoidsDechargement != 0 ? CommandeFournisseur.PoidsDechargement.ToString("# ##0") : cR.GetTextRessource(771));
                    s += Environment.NewLine;
                    s += cR.GetTextRessource(232) + " : " + (CommandeFournisseur.NbBalleDechargement != 0 ? CommandeFournisseur.NbBalleDechargement.ToString() : cR.GetTextRessource(771));
                }
                catch { }
                return s;
            }
        }
        [NotMapped]
        public string NonConformiteTextFournisseur
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = cR.GetTextRessource(319);
                try
                {
                    if (!string.IsNullOrWhiteSpace(CommandeFournisseur.Entite.CodeEE))
                    {
                        s = cR.GetTextRessource(64) + " : " + CommandeFournisseur.Entite.CodeEE;
                    }
                    if (s != cR.GetTextRessource(319)) { s += Environment.NewLine; } else { s = ""; }
                    s += cR.GetTextRessource(17) + " : " + CommandeFournisseur.Entite.Libelle;
                    if (!string.IsNullOrWhiteSpace(CommandeFournisseur.Adr1))
                    {
                        if (s != cR.GetTextRessource(319)) { s += Environment.NewLine; }
                        s += "  " + CommandeFournisseur.Adr1;
                    }
                    if (!string.IsNullOrWhiteSpace(CommandeFournisseur.CodePostal)
                        || !string.IsNullOrWhiteSpace(CommandeFournisseur.Ville)
                        || !string.IsNullOrWhiteSpace(CommandeFournisseur.Pays?.LibelleCourt)
                        )
                    {
                        if (s != cR.GetTextRessource(319)) { s += Environment.NewLine + "  "; }
                        if (!string.IsNullOrWhiteSpace(CommandeFournisseur.CodePostal))
                        {
                            s += CommandeFournisseur.CodePostal + " ";
                        }
                        if (!string.IsNullOrWhiteSpace(CommandeFournisseur.Ville))
                        {
                            s += CommandeFournisseur.Ville + " ";
                        }
                        if (!string.IsNullOrWhiteSpace(CommandeFournisseur.Pays?.LibelleCourt))
                        {
                            s += "(" + CommandeFournisseur.Pays.LibelleCourt + ")";
                        }
                    }
                    if (s != cR.GetTextRessource(319)) { s += Environment.NewLine; }
                    s += cR.GetTextRessource(229) + " : " + (CommandeFournisseur.PoidsChargement != 0 ? CommandeFournisseur.PoidsChargement.ToString("# ##0") : cR.GetTextRessource(771));
                }
                catch { }
                return s;
            }
        }
        [NotMapped]
        public string NonConformiteFamilles
        {
            get
            {
                string s = "";
                if (NonConformiteNonConformiteFamilles != null)
                {
                    if (currentCulture.Name == "fr-FR")
                    {
                        var f = NonConformiteNonConformiteFamilles.Select(e => e.NonConformiteFamille.LibelleFRFR).ToArray();
                        s = string.Join(", ", f);
                    }
                    else
                    {
                        var f = NonConformiteNonConformiteFamilles.Select(e => e.NonConformiteFamille.LibelleENGB).ToArray();
                        s = string.Join(", ", f);
                    }
                }
                return s;
            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if autovalidated stages are correct
        /// </summary>
        public string AutoValidateEtapes(int refUtilisateur)
        {
            string r = "";
            //Process
            //Autovalidate Etape type 7 if applicable (IFClient)
            if (IFClientFactureMontant == null && string.IsNullOrWhiteSpace(IFClientDescr))
            {
                var etape = NonConformiteEtapes.Where(e => e.RefNonConformiteEtapeType == 7).FirstOrDefault();
                if (etape != null)
                {
                    IFClientFactureNro = null;
                    IFClientDFacture = null;
                    IFClientCmtFacturation = null;
                    etape.DValide = DateTime.Now;
                    etape.RefUtilisateurValide = refUtilisateur;
                }
            }
            //Autovalidate Etape type 6 if applicable (IFFournisseur)
            if (IFFournisseurFactureMontant == null && string.IsNullOrWhiteSpace(IFFournisseurDescr) && IFFournisseurDeductionTonnage == null)
            {
                var etape = NonConformiteEtapes.Where(e => e.RefNonConformiteEtapeType == 6).FirstOrDefault();
                if (etape != null)
                {
                    IFFournisseurBonCommandeNro = null;
                    IFFournisseurAttenteBonCommande = false;
                    IFFournisseurTransmissionFacturation = false;
                    IFFournisseurFactureNro = null;
                    IFFournisseurCmtFacturation = null;
                    etape.DValide = DateTime.Now;
                    etape.RefUtilisateurValide = refUtilisateur;
                }
            }
            //End
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if linked data exist.
        /// </summary>
        public string IsDeletable()
        {
            string r = "";
            int nbLinkedData = 0;
            //int nbLinkedData = DbContext.Entry(this).Collection(b => b.NonConformiteCollectivites).Query().Count();
            //nbLinkedData += DbContext.Entry(this).Collection(b => b.NonConformiteProduits).Query().Count();
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