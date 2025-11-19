using eVaSys.Utils;
/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/09/2018
/// ----------------------------------------------------------------------------------------------------- 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for parcours
    /// </summary>
    public class Parcours
    {
        public Parcours()
        {
        }
        private Parcours(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefParcours { get; set; }
        public int RefAdresseOrigine { get; set; }
        private Adresse _adresseOrigine;
        public Adresse AdresseOrigine
        {
            get => LazyLoader.Load(this, ref _adresseOrigine);
            set => _adresseOrigine = value;
        }
        public int RefAdresseDestination { get; set; }
        private Adresse _adresseDestination;
        public Adresse AdresseDestination
        {
            get => LazyLoader.Load(this, ref _adresseDestination);
            set => _adresseDestination = value;
        }
        public int Km { get; set; }
        public ICollection<Transport> Transports { get; set; }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tblParcours where RefParcours=" + RefParcours, DbContext.Database.GetDbConnection()) != "0");
            int c = DbContext.Parcourss.Where(q => (q.RefAdresseOrigine == RefAdresseOrigine && q.RefAdresseDestination == RefAdresseDestination)).Count();
            if ((!existsInDB && c > 0) || (existsInDB && c > 1))
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "") { r += Environment.NewLine; }
                r += cR.GetTextRessource(410);
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.Transports).Query().Count();
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