/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 23/07/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using eVaSys.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for CommandeClientMensuelle
    /// </summary>
    public class CommandeClientMensuelle
    {
        #region Constructor
        public CommandeClientMensuelle()
        {
        }
        private CommandeClientMensuelle(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefCommandeClientMensuelle { get; set; }
        public int RefCommandeClient { get; set; }
        public DateTime D { get; set; }
        public int Poids { get; set; }
        public decimal PrixTonneHT { get; set; }
        public string IdExt { get; set; }
        public int? RefExt { get; set; }
        public int? RefUtilisateurCertif { get; set; }
        public Utilisateur UtilisateurCertif { get; set; }
        public DateTime? DCertif { get; set; }
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
                string s = D.ToString("MM-YYYY");
                var cmdC = DbContext.CommandeClients
                    .Where(q => q.RefCommandeClient == RefCommandeClient)
                    .FirstOrDefault();
                if (cmdC == null)
                {
                    s = cmdC.Adresse.Libelle + " - " + cmdC.Produit.Libelle + " " + D.ToString("MM-YYYY");
                }
                return s;
            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if asked modifications are valid
        /// </summary>
        public string IsPreValid(CommandeClientMensuelleViewModel viewModel, CultureInfo cultureContext, int refUtilisateurContext)
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
                var cmdC = DbContext.CommandeClients
                    .Where(q => q.RefCommandeClient == RefCommandeClient)
                    .FirstOrDefault();
                if (cmdC == null)
                {
                    //Creator or previous modifier can't certify/uncertify
                    if (refUtilisateurContext == cmdC.RefUtilisateurCreation || refUtilisateurContext == cmdC.RefUtilisateurModif)
                    {
                        if (r == "") { r += Environment.NewLine; }
                        r += cR.GetTextRessource(1580);
                    }
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
            bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tblCommandeClientMensuelle where RefCommandeClientMensuelle=" + RefCommandeClientMensuelle, DbContext.Database.GetDbConnection()) != "0");
            //XXXXXXXXXXXXXXXXXXXXXXXXXX
            int c = DbContext.CommandeClientMensuelles.Where(q => (q.RefCommandeClient == RefCommandeClient && q.D == D)).Count();
            if ((!existsInDB && c > 0) || (existsInDB && c > 1))
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "") { r += Environment.NewLine; }
                r += cR.GetTextRessource(410);
            }
            return r;
        }
    }
}