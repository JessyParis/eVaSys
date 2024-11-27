/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/03/2020
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
    /// Class for DescriptionCVQs
    /// </summary>
    public class DescriptionCVQ : IMarkModification
    {
        #region Constructor
        public DescriptionCVQ()
        {
        }
        private DescriptionCVQ(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefDescriptionCVQ { get; set; }
        public int Ordre { get; set; }
        public int LimiteBasse { get; set; }
        public int LimiteHaute { get; set; }
        public bool Actif { get; set; } = true;
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        public ICollection<DescriptionCVQProduit> DescriptionCVQProduits { get; set; }
        public ICollection<CVQDescriptionCVQ> CVQDescriptionCVQs { get; set; }
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
            int c = DbContext.DescriptionControles.Include(r => r.DescriptionControleProduits).AsEnumerable()
                .Where(q => (q.LibelleFRFR == LibelleFRFR || q.LibelleENGB == LibelleENGB)
                    && q.RefDescriptionControle != RefDescriptionCVQ
                    && q.DescriptionControleProduits.Any(rel => DescriptionCVQProduits.Where(i => i.RefProduit == rel.RefProduit).Count() > 0))
                .Count();
            if (c > 0 || LibelleFRFR?.Length > 200 || LibelleENGB?.Length > 200
                || DescriptionCVQProduits.Count != 1)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
                if (LibelleFRFR?.Length > 200) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(732); }
                if (LibelleENGB?.Length > 200) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(733); }
                if (DescriptionCVQProduits.Count != 1) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(739); }
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