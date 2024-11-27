/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/09/2018
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for transport
    /// </summary>
    public class Transport : IMarkModification
    {
        #region Constructor
        public Transport()
        {
        }
        private Transport(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefTransport { get; set; }
        public int RefParcours { get; set; }
        private Parcours _parcours;
        public Parcours Parcours
        {
            get => LazyLoader.Load(this, ref _parcours);
            set => _parcours = value;
        }
        public int RefTransporteur { get; set; }
        private Entite _transporteur;
        public Entite Transporteur
        {
            get => LazyLoader.Load(this, ref _transporteur);
            set => _transporteur = value;
        }
        public int RefCamionType { get; set; }
        private CamionType _camionType;
        public CamionType CamionType
        {
            get => LazyLoader.Load(this, ref _camionType);
            set => _camionType = value;
        }
        public decimal? PUHT { get; set; }
        public decimal? PUHTDemande { get; set; }
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
            if(add)
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
            bool existsInDB = (Utils.Utils.DbScalar("select count(*) from tblTransport where RefTransport=" + RefTransport, DbContext.Database.GetDbConnection()) != "0");
            int c = DbContext.Transports.Where(q => (q.RefParcours == RefParcours && q.RefTransporteur == RefTransporteur && q.RefCamionType == RefCamionType)).Count();
            if ((!existsInDB && c > 0) || (existsInDB && c > 1))
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (r == "")
                {
                    r += Environment.NewLine;
                }
                r += cR.GetTextRessource(389); //"Un transport identique existe déjà.";
            }
            return r;
        }
    }
}