/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 29/11/2019
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for Email
    /// </summary>
    public class Email : IMarkModification
    {
        #region Constructor
        public Email()
        {
        }
        private Email(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefEmail { get; set; }
        public int? RefParamEmail { get; set; }
        private ParamEmail _paramEmail;
        public ParamEmail ParamEmail
        {
            get => LazyLoader.Load(this, ref _paramEmail);
            set => _paramEmail = value;
        }
        public string POPId { get; set; }
        public bool? Entrant { get; set; }
        public int? RefAction { get; set; }
        private Action _action;
        public Action Action
        {
            get => LazyLoader.Load(this, ref _action);
            set => _action = value;
        }
        public string Libelle { get; set; }
        public byte[] EML { get; set; }
        public DateTime? DEnvoi { get; set; }
        public DateTime? DReception { get; set; }
        public string EmailFrom
        {
            get
            {
                return Message.From?.ToString() ?? "";
            }
            set { }
        }
        public string EmailTo
        {
            get
            {
                string a = "";
                foreach (MailBox mB in Message.To)
                {
                    a += mB.Address + ";";
                }
                if (a != "") { a = a.Substring(0, a.Length - 1); }
                return a;
            }
            set { }
        }
        public string EmailSubject
        {
            get
            {
                return Message.Subject?.ToString() ?? "";
            }
            set { }
        }
        public string EmailTextBody { get; set; }
        public DateTime DCreation { get; set; }
        public DateTime? DModif { get; set; }
        [NotMapped]
        private IMail _message;
        [NotMapped]
        public IMail Message
        {
            get
            {
                if (_message == null)
                {
                    if (EML == null)
                    {
                        _message = new MailBuilder().Create();
                    }
                    else { _message = new MailBuilder().CreateFromEml(new MemoryStream(EML).ToArray()); }
                }
                return _message;
            }
            set
            {
                if (value != null)
                {
                    string a = "";
                    foreach (MailBox mB in value.To)
                    {
                        a += mB.Address + ";";
                    }
                    if (a != "") { a = a.Substring(0, a.Length - 1); }
                    EmailTo = a;
                    EmailFrom = value.From?.ToString() ?? "";
                    EmailSubject = value.Subject?.Substring(0, value.Subject.Length) ?? "";
                }
                EML = value.Render();
                _message = value;
            }
        }
        [NotMapped]
        public string EmailHTMLBody
        {
            get
            {
                return Message?.Html ?? "";
            }
            set { }
        }
        [NotMapped]
        private ICollection<EmailFichier> _emailFichiers { get; set; }
        [NotMapped]
        public ICollection<EmailFichier> EmailFichiers
        {
            get
            {
                if (_emailFichiers == null)
                {
                    _emailFichiers = new HashSet<EmailFichier>();
                    for (int i = 0; i < _message.NonVisuals.Count; i++)
                    {
                        EmailFichier eF = new()
                        {
                            Index = i,
                            Nom = _message.NonVisuals[i]?.FileName ?? "nv_sans_titre_" + i.ToString(),
                            Corps = _message.NonVisuals[i].Data,
                            VisualType = "nonvisual"
                        };
                        _emailFichiers.Add(eF);
                    }
                    //for (int i = 0; i < _message.Visuals.Count; i++)
                    //{
                    //    EmailFichier eF = new EmailFichier()
                    //    {
                    //        Index = i,
                    //        Nom = _message.Visuals[i]?.FileName ?? "v_sans_titre_" + i.ToString(),
                    //        Corps = _message.Visuals[i].Data,
                    //        VisualType = "visual"
                    //    };
                    //    _emailFichiers.Add(eF);
                    //}
                }
                return _emailFichiers;
            }
        }
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
        /// Set ParamEmail
        /// </summary>
        public void setParamElail(ParamEmail pE)
        {
            if (pE == null)
            {

            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Checked if model is valid
        /// </summary>
        public string IsValid()
        {
            string r = "";
            if (Libelle?.Length > 200 || EmailSubject?.Length > 200)
            {
                CulturedRessources cR = new(currentCulture, DbContext);
                if (Libelle?.Length > 200) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(664); }
                if (EmailSubject?.Length > 200) { if (r == "") { r += Environment.NewLine; } r += cR.GetTextRessource(665); }
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
            //int nbLinkedData = DbContext.Entry(this).Collection(b => b.Adresses).Query().Count();
            //if (nbLinkedData != 0)
            //{
            //    CulturedRessources cR = new CulturedRessources(currentCulture, DbContext);
            //    if (r == "") { r += Environment.NewLine; }
            //    r += cR.GetTextRessource(393);
            //}
            return r;
        }
    }
}