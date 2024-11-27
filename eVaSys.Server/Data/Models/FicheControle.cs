/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 10/04/2020
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for FicheControles
    /// </summary>
    public class FicheControle : IMarkModification
    {
        #region Constructor
        public FicheControle()
        {
        }
        private FicheControle(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefFicheControle { get; set; }
        public int? RefCommandeFournisseur { get; set; }
        private CommandeFournisseur _commandeFournisseur;
        public CommandeFournisseur CommandeFournisseur
        {
            get => LazyLoader.Load(this, ref _commandeFournisseur);
            set => _commandeFournisseur = value;
        }
        public string NumeroBonLivraison { get; set; }
        public int? RefFournisseur { get; set; }
        private Entite _fournisseur;
        public Entite Fournisseur
        {
            get => LazyLoader.Load(this, ref _fournisseur);
            set => _fournisseur = value;
        }
        public int? RefProduit { get; set; }
        private Produit _produit;
        public Produit Produit
        {
            get => LazyLoader.Load(this, ref _produit);
            set => _produit = value;
        }
        public DateTime? DLivraison { get; set; }
        public int? PoidsTotal { get; set; }
        public int? NbBalle { get; set; }
        public string NumeroLotUsine { get; set; }
        public bool Reserve { get; set; }
        public int? RefControleur { get; set; }
        private ContactAdresse _controleur;
        public ContactAdresse Controleur
        {
            get => LazyLoader.Load(this, ref _controleur);
            set => _controleur = value;
        }
        public string Cmt { get; set; }
        public bool Valide { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public ICollection<FicheControleDescriptionReception> FicheControleDescriptionReceptions { get; set; }
        public ICollection<Controle> Controles { get; set; }
        public ICollection<CVQ> CVQs { get; set; }
        [NotMapped]
        public string FicheControleText
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = cR.GetTextRessource(319);
                Entite client = null;
                try { 
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
                    s += cR.GetTextRessource(19) + " : " + CommandeFournisseur.Produit.Libelle;
                    s += Environment.NewLine;
                    s += cR.GetTextRessource(64) + " : " + CommandeFournisseur.Entite.CodeEE;
                    s += Environment.NewLine;
                    s += cR.GetTextRessource(17) + " : " + CommandeFournisseur.Entite.Libelle;
                    if (!string.IsNullOrWhiteSpace(CommandeFournisseur.Adr1))
                    {
                        s += Environment.NewLine; 
                        s += "  " + CommandeFournisseur.Adr1;
                    }
                    if (!string.IsNullOrWhiteSpace(CommandeFournisseur.CodePostal) || !string.IsNullOrWhiteSpace(CommandeFournisseur.Ville))
                    {
                        s += Environment.NewLine + "  "; 
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
                    s += Environment.NewLine;
                    s += cR.GetTextRessource(277) + " : " + CommandeFournisseur.DDechargement?.ToString("dd/MM/yyyy")??cR.GetTextRessource(771);
                    s += Environment.NewLine;
                    s += cR.GetTextRessource(229) + " : " + (CommandeFournisseur.PoidsChargement != 0 ? CommandeFournisseur.PoidsChargement.ToString("# ##0") : cR.GetTextRessource(771));
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
            int c = DbContext.FicheControles.Where(q => q.RefCommandeFournisseur == RefCommandeFournisseur
                    && q.RefFicheControle != RefFicheControle)
                .Count();
            if (c > 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
            }
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