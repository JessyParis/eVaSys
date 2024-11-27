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
        public int RefProcess { get; set; }
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
        public int RefComposant { get; set; }
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