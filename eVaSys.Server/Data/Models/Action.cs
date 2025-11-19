/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/04/2019
/// -----------------------------------------------------------------------------------------------------
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for Action
    /// </summary>
    public class Action : IMarkModification
    {
        #region Constructor

        public Action()
        {
        }

        private Action(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        private ILazyLoader LazyLoader { get; set; }

        #endregion Constructor

        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefAction { get; set; }
        public int RefEntite { get; set; }
        public int? RefContactAdresse { get; set; }
        private ContactAdresse _contactAdresse;

        public ContactAdresse ContactAdresse
        {
            get => LazyLoader.Load(this, ref _contactAdresse);
            set => _contactAdresse = value;
        }
        public int RefFormeContact { get; set; }
        private FormeContact _formeContact;

        public FormeContact FormeContact
        {
            get => LazyLoader.Load(this, ref _formeContact);
            set => _formeContact = value;
        }
        public int? RefEnvoiDocument { get; set; }
        public string Libelle { get; set; }
        public DateTime? DAction { get; set; }
        public string Email { get; set; }
        public string Cmt { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public ICollection<ActionActionType> ActionActionTypes { get; set; }
        public ICollection<ActionDocumentType> ActionDocumentTypes { get; set; }
        public ICollection<Email> Emails { get; set; }
        public ICollection<ActionFichier> ActionFichiers { get; set; }
        public ICollection<ActionFichierNoFile> ActionFichierNoFiles { get; set; }

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
            if (Libelle?.Length > 100)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Libelle.Length > 100) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.ActionActionTypes).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ActionDocumentTypes).Query().Count();
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