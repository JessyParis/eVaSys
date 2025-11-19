/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/09/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for adresse
    /// </summary>
    public class Adresse : IMarkModification
    {
        #region Constructor
        public Adresse()
        {
        }
        private Adresse(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        #endregion
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefAdresse { get; set; }
        public int RefAdresseType { get; set; }
        private AdresseType _adresseType;
        public AdresseType AdresseType
        {
            get => LazyLoader.Load(this, ref _adresseType);
            set => _adresseType = value;
        }
        public int RefEntite { get; set; }
        public Entite Entite { get; set; }
        public string Libelle { get; set; }
        public string Adr1 { get; set; }
        public string Adr2 { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public int? RefPays { get; set; }
        private Pays _pays;
        public Pays Pays
        {
            get => LazyLoader.Load(this, ref _pays);
            set => _pays = value;
        }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string SiteWeb { get; set; }
        public string Email { get; set; }
        public string Horaires { get; set; }
        public string Cmt { get; set; }
        public bool Actif { get; set; } = true;
        public string RefExt { get; set; }
        [NotMapped]
        private RegionEE _regionEE;
        [NotMapped]
        public RegionEE RegionEE
        {
            get
            {
                if (_regionEE == null)
                {
                    if (Pays.LibelleCourt == "FR" && CodePostal.Length >= 2)
                    {
                        _regionEE = DbContext.RegionEEs
                            .Where(w => (w.RegionEEDpts.Any(rel => rel.RefDpt == CodePostal.Substring(0, 2))))
                            .FirstOrDefault();
                    }
                }
                return _regionEE;
            }
            set
            {
                _regionEE = value;
            }
        }
        public ICollection<Parcours> ParcoursAdresseOrigines { get; set; }
        public ICollection<Parcours> ParcoursAdresseDestinations { get; set; }
        public ICollection<ContactAdresse> ContactAdresses { get; set; }
        public ICollection<CommandeClient> CommandeClients { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurs { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurAdresseClients { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        [NotMapped]
        public string RefDpt
        {
            get
            {
                return CodePostal?.Length >= 2 ? CodePostal?.Substring(0, 2) : "";
            }
            set { }
        }
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
        public string TexteAdresseList
        {
            get
            {
                //Texte de synthèse des donnée sprincipales de l'adresse
                string s = Libelle + " - " + CodePostal + " " + Ville + " " + (string.IsNullOrEmpty(Pays.Libelle) ? "NR" : Pays.Libelle) + " - " + AdresseType.Libelle;
                return s;
            }
        }
        public string TexteAdresse
        {
            get
            {
                //Texte de synthèse des donnée sprincipales de l'adresse
                string s = Libelle + Environment.NewLine + Adr1 + (string.IsNullOrEmpty(Adr2) ? "" : Environment.NewLine + Adr2) + Environment.NewLine + CodePostal + " " + Ville + " (" + (string.IsNullOrEmpty(Pays.Libelle) ? "NR" : Pays.Libelle) + ")";
                return s;
            }
        }
        //Texte des données CDT
        public string TexteAdresseCentreDeTri
        {
            get
            {
                string s = "";
                if (RefEntite == 3)
                {
                    //Texte de synthèse des donnée sprincipales de l'adresse
                    Entite entite = DbContext.Entites.Single(p => p.RefEntite == RefEntite);
                    s = entite.Libelle + " (" + entite.CodeEE + ")"
                        + Environment.NewLine + Adr1 + (string.IsNullOrEmpty(Adr2) ? "" : Environment.NewLine + Adr2) + Environment.NewLine + CodePostal + " " + Ville + " (" + (string.IsNullOrEmpty(Pays.Libelle) ? "NR" : _pays.Libelle) + ")"
                        + Environment.NewLine + Horaires;
                }
                return s;
            }
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
        /// <param name="excludeProcess">Don't check if ContactAdresse has Processes</param>
        public string IsDeletable(bool excludeProcess)
        {
            string r = "";
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.ParcoursAdresseDestinations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ParcoursAdresseDestinations).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeClients).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurAdresseClients).Query().Count();
            if (!excludeProcess)
            {
                nbLinkedData += DbContext.Entry(this).Collection(b => b.ContactAdresses).Query()
                .Where(el => el.ContactAdresseContactAdresseProcesss.Count > 0).Count();
            }
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