using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/11/2018
/// ----------------------------------------------------------------------------------------------------- 
using System.Globalization;

namespace eVaSys.Data
{
    public class MessageDiffusion
    {
        #region Constructor
        public MessageDiffusion()
        {
        }
        private MessageDiffusion(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        #endregion
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefMessageDiffusion { get; set; }
        public int RefMessage { get; set; }
        public string RefModule { get; set; }
        public string RefHabilitation { get; set; }
        public int? RefUtilisateur { get; set; }
        private Utilisateur _utilisateur;
        public Utilisateur Utilisateur
        {
            get => LazyLoader.Load(this, ref _utilisateur);
            set => _utilisateur = value;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            //bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tblMessageDiffusion where RefMessageDiffusion=" + RefMessageDiffusion, DbContext.Database.GetDbConnection()) != "0");
            //int c = DbContext.Payss.Where(q => (q.Libelle == Libelle)).Count();
            //if (!existsInDB && c > 0 || existsInDB && c > 1 || Libelle.Length > 250)
            //{
            //    CulturedRessources cR = new CulturedRessources(currentCulture, DbContext);
            //    if (Libelle.Length > 250) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
            //    if ((!existsInDB && c > 0) || (existsInDB && c > 1)) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
            //}
            return r;
        }
    }
}