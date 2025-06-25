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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for Repartition
    /// </summary>
    public class Repartition : IMarkModification
    {
        #region Constructor
        public Repartition()
        {
        }
        private Repartition(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefRepartition { get; set; }
        public int? RefFournisseur { get; set; }
        private Entite _fournisseur;
        public Entite Fournisseur
        {
            get => LazyLoader.Load(this, ref _fournisseur);
            set => _fournisseur = value;
        }
        public int? RefCommandeFournisseur { get; set; }
        private CommandeFournisseur _commandeFournisseur;
        public CommandeFournisseur CommandeFournisseur
        {
            get => LazyLoader.Load(this, ref _commandeFournisseur);
            set => _commandeFournisseur = value;
        }
        public int? RefProduit { get; set; }
        private Produit _produit;
        public Produit Produit
        {
            get => LazyLoader.Load(this, ref _produit);
            set => _produit = value;
        }
        public DateTime? D { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public ICollection<RepartitionCollectivite> RepartitionCollectivites { get; set; }
        public ICollection<RepartitionProduit> RepartitionProduits { get; set; }
        [NotMapped]
        public int RefUtilisateurCourant { get; set; }
        public int RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        public int? RefUtilisateurModif { get; set; }
        public Utilisateur UtilisateurModif { get; set; }
        public int PoidsReparti
        {
            get
            {
                int p = 0;
                if (CommandeFournisseur != null)
                {
                    p = CommandeFournisseur.PoidsReparti;
                }
                else
                {
                    p = DbContext.CommandeFournisseurs
                        .Where(el => el.RefEntite == RefFournisseur && el.RefProduit == RefProduit && ((DateTime)D).Month == ((DateTime)el.DDechargement).Month && ((DateTime)D).Year == ((DateTime)el.DDechargement).Year)
                        .Sum(el => el.PoidsReparti);
                }
                return p;
            }
        }
        public int PoidsChargement
        {
            get
            {
                int p = 0;
                if (CommandeFournisseur != null)
                {
                    p = CommandeFournisseur.PoidsChargement;
                }
                else
                {
                    p = DbContext.CommandeFournisseurs
                        .Where(el => el.RefEntite == RefFournisseur && el.RefProduit == RefProduit && ((DateTime)D).Month == ((DateTime)el.DDechargement).Month && ((DateTime)D).Year == ((DateTime)el.DDechargement).Year)
                        .Sum(el => el.PoidsChargement);
                }
                return p;
            }
        }
        public string InfoText
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (CommandeFournisseur != null)
                {
                    //Unit
                    s = cR.GetTextRessource(14) + " - " + Utils.Utils.FormatNumeroCommande(CommandeFournisseur.NumeroCommande.ToString())
                        + Environment.NewLine + CommandeFournisseur.Entite.Libelle + " (" + CommandeFournisseur.Entite.CodeEE + ")"
                        + Environment.NewLine + CommandeFournisseur.Produit.Libelle;
                    if (CommandeFournisseur.DChargement != null)
                    {
                        s += Environment.NewLine + cR.GetTextRessource(1572) + " - " + ((DateTime)CommandeFournisseur.DChargement).ToString("d", currentCulture);
                    }
                    if (CommandeFournisseur.DDechargement != null)
                    {
                        s += Environment.NewLine + cR.GetTextRessource(587) + " - " + ((DateTime)CommandeFournisseur.DDechargement).ToString("d", currentCulture);
                    }
                    s += Environment.NewLine + cR.GetTextRessource(229) + " - " + PoidsChargement.ToString()
                    + Environment.NewLine + cR.GetTextRessource(480) + " - " + PoidsReparti.ToString();
                }
                else
                {
                    //Monthly
                    s = Fournisseur.Libelle + " (" + Fournisseur.CodeEE + ")"
                        + Environment.NewLine + Produit.Libelle
                        + Environment.NewLine + ((DateTime)D).ToString("MMMM yyyy", currentCulture)
                        + Environment.NewLine + cR.GetTextRessource(229) + " - " + PoidsChargement.ToString()
                        + Environment.NewLine + cR.GetTextRessource(480) + " - " + PoidsReparti.ToString();
                    ;
                }
                return s;
            }
        }
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
            //Check if no Repartition already exists for CommandeFournisseur
            if (RefCommandeFournisseur != null)
            {
                if (DbContext.Repartitions.Where(e => e.RefCommandeFournisseur == RefCommandeFournisseur && e.RefRepartition != RefRepartition).Count() > 0)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    r = cR.GetTextRessource(1477);
                }
            }
            else
            {
                if (DbContext.Repartitions.Where(e => e.RefFournisseur == RefFournisseur && e.D == D
                    && e.RefRepartition != RefRepartition).Count() > 0)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    r = cR.GetTextRessource(1477);
                }
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
            //int nbLinkedData = DbContext.Entry(this).Collection(b => b.RepartitionCollectivites).Query().Count();
            //nbLinkedData += DbContext.Entry(this).Collection(b => b.RepartitionProduits).Query().Count();
            if (nbLinkedData != 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "") { r += Environment.NewLine; }
                r += cR.GetTextRessource(393);
            }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Set all existing PrixReprise
        /// </summary>
        public void SetPrixReprises()
        {
            if (CommandeFournisseur != null)
            {
                //Get PrixReprise for this Repartition
                var pR = DbContext.PrixReprises
                    .Where(el => el.D == (CommandeFournisseur.DChargement ?? CommandeFournisseur.D)
                        && el.RefProduit == CommandeFournisseur.Produit.RefProduit
                        && el.Process == null && el.Composant == null)
                    .FirstOrDefault();
                if (pR != null)
                {
                    foreach (var rP in RepartitionProduits)
                    {
                        if (rP.PUHT == null) rP.PUHT = pR.PUHT;
                    }
                    foreach (var rC in RepartitionCollectivites)
                    {
                        if (rC.PUHT == null) rC.PUHT = pR.PUHT;
                    }
                }
            }
        }
    }
}