/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/09/2018
/// ----------------------------------------------------------------------------------------------------- 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    public class Pays : IMarkModification
    {
        #region Constructor
        public Pays()
        {
            //AdressePays = new HashSet<Adresse>();
            //UtilisateurPays = new HashSet<Utilisateur>();
        }
        private Pays(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        #endregion
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }
        public static string TextFilter(string filter, ApplicationDbContext dbContext)
        {
            StringBuilder r = new();
            if (filter != "")
            {
                foreach (string f in filter.Split(","))
                {
                    var res = dbContext.Payss.Where(i => i.RefPays == System.Convert.ToInt32(f)).FirstOrDefault();
                    r.Append(res.Libelle + ", ");
                }
            }
            if (r.Length != 0) { r = r.Remove(r.Length - 2, 2); }
            return r.ToString();
        }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefPays { get; set; }
        public bool Actif { get; set; } = true;
        public string Libelle { get; set; }
        public string LibelleCourt { get; set; }
        public int? RefRegionReporting { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public ICollection<Adresse> Adresses { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurs { get; set; }
        public ICollection<Civilite> Civilites { get; set; }
        public ICollection<SurcoutCarburant> SurcoutCarburants { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Utilisateur> Utilisateurs { get; set; }
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
            bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tbrPays where RefPays=" + RefPays, DbContext.Database.GetDbConnection()) != "0");
            int c = DbContext.Payss.Where(q => (q.Libelle == Libelle || q.LibelleCourt == LibelleCourt)).Count();
            if ((!existsInDB && c > 0) || (existsInDB && c > 1) || Libelle?.Length > 50 || LibelleCourt?.Length > 3)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Libelle?.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
                if (LibelleCourt?.Length > 3) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(411); }
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.Adresses).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.Civilites).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.Utilisateurs).Query().Count();
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