/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/06/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for ParamEmail
    /// </summary>
    public class ParamEmail : IMarkModification
    {
        #region Constructor
        public ParamEmail()
        {
        }
        private ParamEmail(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefParamEmail { get; set; }
        public bool Actif { get; set; }
        public bool Defaut { get; set; }
        public bool RttClient { get; set; }
        public string POP { get; set; }
        public int? PortPOP { get; set; }
        public string ComptePOP { get; set; }
        public string PwdPOP { get; set; }
        public bool TLSPOP { get; set; }
        public string EnTeteEmail { get; set; }
        public string PiedEmail { get; set; }
        public string EmailExpediteur { get; set; }
        public string LibelleExpediteur { get; set; }
        public string SMTP { get; set; }
        public int? PortSMTP { get; set; }
        public string CompteSMTP { get; set; }
        public string PwdSMTP { get; set; }
        public bool AllowAuthSMTP { get; set; }
        public bool TLSSMTP { get; set; }
        public bool StartTLSSMTP { get; set; }
        public bool AR { get; set; }
        public string AREmail { get; set; }
        public string AREmailSujet { get; set; }
        public string ARExpediteurEmail { get; set; }
        public string ARExpediteurLibelle { get; set; }
        [JsonIgnore]
        public ICollection<SessionPop> SessionPops { get; set; }
        [JsonIgnore]
        public ICollection<Email> Emails { get; set; }
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
            //bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tbrParamEmail where RefParamEmail=" + RefParamEmail, DbContext.Database.GetDbConnection()) != "0");
            //int c = DbContext.ParamEmails.Where(q => q.Libelle == Libelle).Count();
            //if ((!existsInDB && c > 0) || (existsInDB && c > 1) || Libelle.Length > 50)
            //{
            //    CulturedRessources cR = new CulturedRessources(currentCulture, DbContext);
            //    if ((!existsInDB && c > 0) || (existsInDB && c > 1)) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
            //    if (Libelle.Length > 50) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
            //}
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if linked data exist.
        /// </summary>
        public string IsDeletable()
        {
            string r = "";
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.SessionPops).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.Emails).Query().Count();
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