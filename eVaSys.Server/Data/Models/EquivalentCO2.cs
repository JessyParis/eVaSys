/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 26/10/2023
/// -----------------------------------------------------------------------------------------------------
using eVaSys.Utils;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for EquivalentCO2
    /// </summary>
    public class EquivalentCO2 : IMarkModification
    {
        #region Constructor

        public EquivalentCO2()
        {
        }

        #endregion Constructor

        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefEquivalentCO2 { get; set; }
        public string Libelle { get; set; }
        public int Ordre { get; set; }
        public decimal? Ratio { get; set; }
        public byte[] Icone { get; set; }
        public string IconeBase64
        {
            get
            {
                string s = "";
                if (Icone != null) { s = Convert.ToBase64String(Icone); }
                return s;
            }
        }
        public bool Actif { get; set; } = false;
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
            int sameLibelle = DbContext.EquivalentCO2s.Where(q => q.Libelle == Libelle && q.RefEquivalentCO2 != RefEquivalentCO2).Count();
            int sameOrdre = DbContext.EquivalentCO2s.Where(q => q.Ordre == Ordre && q.RefEquivalentCO2 != RefEquivalentCO2).Count();
            if (sameLibelle > 0 || sameOrdre > 0 || Libelle?.Length > 50)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (sameLibelle > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
                if (sameOrdre > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(155); }
                if (Libelle?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
            }
            return r;
        }

    }
}