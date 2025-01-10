/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 13/12/2024
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for Contrat
    /// </summary>
    public class Contrat : IMarkModification
    {
        #region Constructor
        public Contrat()
        {
        }
        private Contrat(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefContrat { get; set; }
        public string IdContrat { get; set; }
        public int? RefContratType { get; set; }
        private ContratType _contratType;
        public ContratType ContratType
        {
            get => LazyLoader.Load(this, ref _contratType);
            set => _contratType = value;
        }
        public DateOnly? DDebut { get; set; }
        public DateOnly? DFin { get; set; }
        public bool ReconductionTacite { get; set; }
        public bool Avenant { get; set; }
        public string Cmt { get; set; }
        public byte[] Fichier { get; set; }
        public string FichierBase64
        {
            get
            {
                return Convert.ToBase64String(Fichier);
            }
        }
        public ICollection<ContratEntite> ContratEntites { get; set; }
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
            if (!(RefContratType > 0) || DDebut == null || DFin==null || string.IsNullOrWhiteSpace(IdContrat))
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (!(RefContratType > 0)) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(1543); }
                if (DDebut == null) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(1174); }
                if (DFin == null) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(1175); }
                if (string.IsNullOrWhiteSpace(IdContrat)) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(1542); }
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
            //If special prices for RepriseIndividuelle
            //if (RefContratType == (int)Enumerations.ContratType.RepriseIndividuelle)
            //{
            //    var e = DbContext.Entites.AsNoTracking().Where(q => q.RefEntite == RefEntite).FirstOrDefault();
            //    if (e?.RefEntiteType==)
            //    nbLinkedData = DbContext.PrixSpecifiques.Where(q => q.RefContrat == RefContrat).Count();
            //}
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