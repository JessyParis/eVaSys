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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

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