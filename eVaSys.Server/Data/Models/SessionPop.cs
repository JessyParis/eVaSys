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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for SessionPop
    /// </summary>
    public class SessionPop
    {
        #region Constructor
        public SessionPop()
        {
        }
        private SessionPop(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefSessionPop { get; set; }
        public int? RefParamEmail { get; set; }
        private ParamEmail _paramEmail;
        public ParamEmail ParamEmail
        {
            get => LazyLoader.Load(this, ref _paramEmail);
            set => _paramEmail = value;
        }
        public string POP { get; set; }
        public string ComptePOP { get; set; }
        public string Etat { get; set; }
        public string Libelle { get; set; }
        public int? NbEMail { get; set; }
        public DateTime? DDebut { get; set; }
        public DateTime? DFin { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            CulturedRessources cR = new(currentCulture, DbContext);
            if (Libelle?.Length > 200) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(664); }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if linked data exist.
        /// </summary>
        public string IsDeletable()
        {
            CulturedRessources cR = new(currentCulture, DbContext);
            return cR.GetTextRessource(666);
        }
    }
}