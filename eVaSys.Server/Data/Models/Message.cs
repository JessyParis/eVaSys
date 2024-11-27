/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/11/2018
/// ----------------------------------------------------------------------------------------------------- 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    public class Message : IMarkModification
    {
        #region Constructor
        public Message()
        {
        }
        private Message(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        #endregion
        private ILazyLoader LazyLoader { get; set; }
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefMessage { get; set; }
        public int RefMessageType { get; set; }
        private MessageType _messageType;
        public MessageType MessageType
        {
            get => LazyLoader.Load(this, ref _messageType);
            set => _messageType = value;
        }
        public string Libelle { get; set; }
        public string Titre { get; set; }
        public string Corps { get; set; }
        public string CorpsHTML { get; set; }
        [NotMapped]
        public string CorpsHTMLFromText
        {
            get
            {
                string r = "";
                if (string.IsNullOrEmpty(CorpsHTML))
                {
                    r = System.Net.WebUtility.HtmlEncode(Corps ?? "").Replace(Environment.NewLine, "<br/>");
                }
                return r;
            }
        }
        public string Cmt { get; set; }
        public DateTime? DDebut { get; set; }
        public DateTime? DFin { get; set; }
        public bool? DiffusionUnique { get; set; } = false;
        public bool? Actif { get; set; } = true;
        public bool? Important { get; set; } = false;
        public bool? VisualisationConfirmeUnique { get; set; } = false;
        public ICollection<MessageDiffusion> MessageDiffusions { get; set; }
        public ICollection<MessageVisualisation> MessageVisualisations { get; set; }
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
            //int c = DbContext.Messages.Where(q => q.Libelle == Libelle & q.RefMessage != RefMessage).Count();
            if (/*c > 0 ||*/ Libelle?.Length > 250)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Libelle?.Length > 250) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(394); }
                //if (c > 0) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(410); }
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
            int nbLinkedData = DbContext.Entry(this).Collection(b => b.MessageVisualisations).Query().Count();
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