/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/07/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for Process
    /// </summary>
    public class Process : IMarkModification
    {
        #region Constructor
        public Process()
        {
        }
        private Process(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public static string TextFilter(string filter, ApplicationDbContext dbContext)
        {
            StringBuilder r = new();
            if (filter != "")
            {
                foreach (string f in filter.Split(","))
                {
                    var res = dbContext.Processs.Where(i => i.RefProcess == System.Convert.ToInt32(f)).FirstOrDefault();
                    r.Append(res.Libelle + ", ");
                }
            }
            if (r.Length != 0) { r = r.Remove(r.Length - 2, 2); }
            return r.ToString();
        }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefProcess { get; set; }
        public string Libelle { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public ICollection<EntiteProcess> EntiteProcesss { get; set; }
        public ICollection<PrixReprise> PrixReprises { get; set; }
        public ICollection<RepartitionCollectivite> RepartitionCollectivites { get; set; }
        public ICollection<RepartitionProduit> RepartitionProduits { get; set; }
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
            bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tbrProcess where RefProcess=" + RefProcess, DbContext.Database.GetDbConnection()) != "0");
            int c = DbContext.Processs.Where(q => q.Libelle == Libelle).Count();
            if ((!existsInDB && c > 0) || (existsInDB && c > 1) || Libelle?.Length > 50)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Libelle?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
                if ((!existsInDB && c > 0) || (existsInDB && c > 1)) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.EntiteProcesss).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepartitionCollectivites).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.RepartitionProduits).Query().Count();
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