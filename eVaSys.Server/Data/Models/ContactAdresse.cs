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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for contact à l'adresse
    /// </summary>
    public class ContactAdresse : IMarkModification
    {
        #region Constructor
        public ContactAdresse()
        {
        }
        private ContactAdresse(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefContactAdresse { get; set; }
        public int RefContact { get; set; }
        private Contact _contact;
        public Contact Contact
        {
            get => LazyLoader.Load(this, ref _contact);
            set => _contact = value;
        }
        public int RefEntite { get; set; }
        public Entite Entite { get; set; }
        public int? RefAdresse { get; set; }
        //private Adresse _adresse;
        public Adresse Adresse { get; set; }
        //public Adresse Adresse
        //{
        //    get => LazyLoader.Load(this, ref _adresse);
        //    set => _adresse = value;
        //}
        public int? RefTitre { get; set; }
        private Titre _titre;
        public Titre Titre
        {
            get => LazyLoader.Load(this, ref _titre);
            set => _titre = value;
        }
        public string Tel { get; set; }
        public string TelMobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Cmt { get; set; }
        public string CmtServiceFonction { get; set; }
        public bool? Actif { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        public ICollection<ContactAdresseContactAdresseProcess> ContactAdresseContactAdresseProcesss { get; set; }
        public ICollection<ContactAdresseDocumentType> ContactAdresseDocumentTypes { get; set; }
        public ICollection<ContactAdresseServiceFonction> ContactAdresseServiceFonctions { get; set; }
        public ICollection<Action> Actions { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurs { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurTransporteurs { get; set; }
        public ICollection<FicheControle> FicheControleControleurs { get; set; }
        public ICollection<Controle> ControleControleurs { get; set; }
        public ICollection<CVQ> CVQControleurs { get; set; }
        public ICollection<Utilisateur> Utilisateurs { get; set; }
        public string ListText
        {
            get
            {
                string s = "";
                if (Contact != null)
                {
                    CulturedRessources cR = new(currentCulture, DbContext);
                    s = Contact.Nom + (string.IsNullOrEmpty(Contact.Prenom) ? "" : " " + Contact.Prenom);
                    if (!string.IsNullOrEmpty(TelMobile))
                    {
                        s += " - " + TelMobile;
                    }
                    else if (!string.IsNullOrEmpty(Tel))
                    {
                        s += " - " + Tel;
                    }
                    else if (!string.IsNullOrEmpty(Email))
                    {
                        s += " - " + Email;
                    }
                }
                return s;
            }
        }
        [NotMapped]
        public int RefUtilisateurCourant { get; set; }
        public int RefUtilisateurCreation { get; set; }
        public Utilisateur UtilisateurCreation { get; set; }
        public int? RefUtilisateurModif { get; set; }
        public Utilisateur UtilisateurModif { get; set; }
        public string GetTels
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (!string.IsNullOrWhiteSpace(Tel)) { s = cR.GetTextRessource(417) + " : " + Tel; }
                if (!string.IsNullOrWhiteSpace(TelMobile))
                {
                    if (!string.IsNullOrWhiteSpace(Tel)) { s += " / "; }
                    s += cR.GetTextRessource(437) + " : " + TelMobile;
                }
                return s;
            }
        }
        public string GetTelsOrEmail
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (!string.IsNullOrWhiteSpace(Tel)) { s = cR.GetTextRessource(417) + " : " + Tel; }
                if (!string.IsNullOrWhiteSpace(TelMobile))
                {
                    if (!string.IsNullOrWhiteSpace(Tel)) { s += " / "; }
                    s += cR.GetTextRessource(437) + " : " + TelMobile;
                }
                if ((string.IsNullOrWhiteSpace(Tel) || string.IsNullOrWhiteSpace(TelMobile)) && !string.IsNullOrWhiteSpace(Email))
                {
                    {
                        if (!string.IsNullOrWhiteSpace(Tel) || !string.IsNullOrWhiteSpace(TelMobile)) { s += " / "; }
                        s += cR.GetTextRessource(419) + " : " + Email;
                    }
                }
                return s;
            }
        }
        public string GetFirstTelOrEmail
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (!string.IsNullOrWhiteSpace(Tel)) { s = cR.GetTextRessource(417) + " : " + Tel; }
                else if (!string.IsNullOrWhiteSpace(TelMobile)) { s += cR.GetTextRessource(437) + " : " + TelMobile; }
                else if (!string.IsNullOrWhiteSpace(Email)) { s += cR.GetTextRessource(419) + " : " + Email; }
                return s;
            }
        }
        public string GetSecondTelOrEmail
        {
            get
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                string s = "";
                if (!string.IsNullOrWhiteSpace(Tel))
                {
                    if (!string.IsNullOrWhiteSpace(TelMobile))
                    {
                        s = cR.GetTextRessource(437) + " : " + TelMobile;
                    }
                    else if (!string.IsNullOrWhiteSpace(Email)) { s += cR.GetTextRessource(419) + " : " + Email; }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(TelMobile))
                    {
                        if(!string.IsNullOrWhiteSpace(Email)) { s += cR.GetTextRessource(419) + " : " + Email; }
                    }
                }
                return s;
            }
        }
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
            if (Email != null && Email?.Length > 200 || Tel != null && Tel?.Length > 20 || TelMobile != null && TelMobile?.Length > 20 || Fax != null && Fax?.Length > 20)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Email?.Length > 200) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(413); }
                if (Tel?.Length > 20) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(414); }
                if (TelMobile?.Length > 20) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(415); }
                if (Fax?.Length > 20) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(416); }
            }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if linked data exist.
        /// </summary>
        /// <param name="excludeProcess">Don't check if ContactAdresse has Processes</param>
        public string IsDeletable(bool excludeProcess)
        {
            string r = "";
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.CommandeFournisseurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CommandeFournisseurTransporteurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.FicheControleControleurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.ControleControleurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.CVQControleurs).Query().Count();
            nbLinkedData += DbContext.Entry(this).Collection(b => b.Actions).Query().Count();
            if (!excludeProcess)
            {
                nbLinkedData += DbContext.Entry(this).Collection(b => b.ContactAdresseContactAdresseProcesss).Query().Count();
            }
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