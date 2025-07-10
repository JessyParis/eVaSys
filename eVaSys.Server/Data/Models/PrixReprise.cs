/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/09/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for PrixReprise
    /// </summary>
    public class PrixReprise : IMarkModification
    {
        #region Constructor
        public PrixReprise()
        {
        }
        private PrixReprise(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        #endregion
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefPrixReprise { get; set; }
        public DateTime D { get; set; }
        public int? RefContrat { get; set; }
        private Contrat _contrat;
        public Contrat Contrat
        {
            get => LazyLoader.Load(this, ref _contrat);
            set => _contrat = value;
        }
        public int? RefProcess { get; set; }
        private Process _process;
        public Process Process
        {
            get => LazyLoader.Load(this, ref _process);
            set => _process = value;
        }
        public int RefProduit { get; set; }
        private Produit _produit;
        public Produit Produit
        {
            get => LazyLoader.Load(this, ref _produit);
            set => _produit = value;
        }
        public int? RefComposant { get; set; }
        private Produit _composant;
        public Produit Composant
        {
            get => LazyLoader.Load(this, ref _composant);
            set => _composant = value;
        }
        public decimal? PUHT { get; set; }
        public decimal? PUHTSurtri { get; set; }
        public decimal? PUHTTransport { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        [NotMapped]
        public int RefUtilisateurCourant { get; set; }
        [NotMapped]
        public bool ApplyMarkModification { get; set; } = true;
        public int RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        public int? RefUtilisateurModif { get; set; }
        public Utilisateur UtilisateurModif { get; set; }
        public int? RefUtilisateurCertif { get; set; }
        public Utilisateur UtilisateurCertif { get; set; }
        public DateTime? DCertif { get; set; }
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
        public string CertificationText
        {
            get
            {
                string s = "";
                if (UtilisateurCertif != null && DCertif != null)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    s = cR.GetTextRessource(1578) + " " + ((DateTime)DCertif).ToString("G", currentCulture) + " " + cR.GetTextRessource(390) + " " + UtilisateurCertif.Nom;
                }
                return s;
            }
        }
        public string MonolineSummaryText
        {
            get
            {
                string s = Produit.Libelle + " " + D.ToString("MM-YYYY");
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
                if (ApplyMarkModification)
                {
                    RefUtilisateurModif = RefUtilisateurCourant;
                    DModif = DateTime.Now;
                }
            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if asked modifications are valid
        /// </summary>
        public string IsPreValid(PrixRepriseViewModel viewModel, CultureInfo cultureContext, int refUtilisateurContext)
        {
            string r = "";
            CulturedRessources cR = new(cultureContext, DbContext);
            //Check certification
            //If certified, only previous certifier can modify or delete
            if (RefUtilisateurCertif > 0 || viewModel.Certif == false)
            {
                //Only previous certifier can modify/delete or uncetifiy
                if (refUtilisateurContext != RefUtilisateurCertif)
                {
                    if (r == "") { r += Environment.NewLine; }
                    r += cR.GetTextRessource(1579);
                }
            }
            else if (viewModel.Certif == true)
            {
                //Creator or previous modifier can't certify/uncertify
                if (refUtilisateurContext == RefUtilisateurCreation || refUtilisateurContext == RefUtilisateurModif)
                {
                    if (r == "") { r += Environment.NewLine; }
                    r += cR.GetTextRessource(1580);
                }
            }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            //Check duplicates
            int c = DbContext.PrixReprises.Where(q => q.D == D && q.RefProduit == RefProduit && q.RefContrat == RefContrat
                && q.RefPrixReprise != RefPrixReprise).Count();
            //Check certification

            //Create error message if applicable
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
        public string IsDeletable(CultureInfo cultureContext, int refUtilisateurContext)
        {
            string r = "";
            //Check linked data
            int nbLinkedData = 0;
            if (nbLinkedData != 0)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "") { r += Environment.NewLine; }
                r += cR.GetTextRessource(393);
            }
            //Check certification
            //If certified, only previous certifier can delete
            if (RefUtilisateurCertif > 0)
            {
                //Only previous certifier can delete
                if (refUtilisateurContext != RefUtilisateurCertif)
                {
                    if (r == "") { r += Environment.NewLine; }
                    r += new CulturedRessources(cultureContext, DbContext).GetTextRessource(1579);
                }
            }
            return r;
        }
    }
}