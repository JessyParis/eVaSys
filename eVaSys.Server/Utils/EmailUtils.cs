/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création :24/10/2019
/// ----------------------------------------------------------------------------------------------------- 

using eVaSys.Data;
using Limilabs.Client.SMTP;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.MIME;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.Data;
using System.Net;
using System.Security.Authentication;
using System.Windows.Interop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace eVaSys.Utils
{
    /// <summary>
    /// Utilities for Email
    /// </summary>
    public static class EmailUtils
    {
        /// <summary>
        /// Send an e-mail
        /// </summary>
        /// <param name="email">The e-mail to send</param>
        /// <param name="refUtilisateur">The id of the user that sends the e-mail, O to skip database persisting</param>
        /// <param name="CurrentContext">The current session context</param>
        /// <param name="configuration">The current IConfiguration</param>
        /// <returns>"ok", or the error message</returns>
        public static string Send(Email email, int refUtilisateur, ApplicationDbContext dbContext, Context CurrentContext, IConfiguration configuration)
        {
            //Init
            string err = CurrentContext.CulturedRessources.GetTextRessource(677);
            //Process
            //On vérifie qu'un parametrage email existe
            if (email.ParamEmail != null)
            {
                if (!string.IsNullOrEmpty(email.ParamEmail.SMTP))
                {
                    var msg = email.Message;

                    if (msg.From.Count == 0) { msg.From.Add(new Limilabs.Mail.Headers.MailBox(email.ParamEmail.EmailExpediteur, email.ParamEmail.LibelleExpediteur)); }
                    msg.From.Clear();
                    msg.From.Add(new Limilabs.Mail.Headers.MailBox(email.ParamEmail.EmailExpediteur, "Valorplast"));
                    //Modification de l'adresse en cas de formation
                    if (configuration["AppParameters:DebugMode"] == "true")
                    {
                        //Inscription des destinataires dans le sujet d'email
                        string s = " (To: ";
                        foreach (MailBox mB in msg.To)
                        {
                            s += mB.Address + ", ";
                        }
                        if (msg.To.Count > 0) { s = s.Substring(0, s.Length - 2); }
                        if (msg.Cc.Count > 0)
                        {
                            s += ", Cc: ";
                            foreach (MailBox mB in msg.Cc)
                            {
                                s += mB.Address + ", ";
                            }
                            s = s.Substring(0, s.Length - 2);
                        }
                        if (msg.Bcc.Count > 0)
                        {
                            s += ", Bcc: ";
                            foreach (MailBox mB in msg.Bcc)
                            {
                                s += mB.Address + ", ";
                            }
                            s = s.Substring(0, s.Length - 2);
                        }
                        s += ")";
                        msg.Subject += s;
                        //Modification de l'expediteur
                        if (configuration["AppParameters:EmailDev"] == "true")
                        {
                            msg.From.Clear();
                            msg.From.Add(new Limilabs.Mail.Headers.MailBox("debug.enviromatic@orange.fr", "debug.enviromatic@orange.fr"));
                        }
                        //Modification des destinataires
                        msg.To.Clear();
                        msg.Bcc.Clear();
                        msg.Cc.Clear();
                        //Tentative d'ajout de l'utilisateur courant en tant que destinataire
                        msg.To.Add(new MailBox("debug.enviromatic@orange.fr"));
                        //Utilisateur u = dbContext.Utilisateurs.Where(el => el.RefUtilisateur == refUtilisateur).FirstOrDefault();
                        //if (!string.IsNullOrEmpty(u?.EMail))
                        //{
                        //    msg.To.Add(new MailBox(u.EMail));
                        //}
                        //else
                        //{
                        //    //msg.To.Add(new MailBox("debug.enviromatic@orange.fr"));
                        //    //msg.To.Add(new MailBox("jean-christian.nugues@enviromatic.fr"));
                        //    //msg.To.Add(new MailBox("jean-christian.nugues@wanadoo.fr"));
                        //    msg.To.Add(new MailBox("nugues.jean@wanadoo.fr"));
                        //    //msg.To.Add(new MailBox("J.DECAMAS@valorplast.com"));
                        //    //msg.To.Add(new MailBox("G.TRAORE@valorplast.com"));
                        //}
                    }
                    email.Message = msg;
                    using (Smtp smtp = new())
                    {
                        //Modification du compte email pour l'envoi
                        if (configuration["AppParameters:EmailDev"] == "true")
                        {
                            smtp.ConnectSSL("smtp.orange.fr", 465);       // or ConnectSSL for SSL
                            smtp.UseBestLogin("debug.enviromatic@orange.fr", "jsbiue453l");
                        }
                        else
                        {
                            if (email.ParamEmail.TLSSMTP)
                            {
                                smtp.ConnectSSL(email.ParamEmail.SMTP, (email.ParamEmail.PortSMTP == null ? 465 : (int)email.ParamEmail.PortSMTP));
                            }
                            else if (email.ParamEmail.StartTLSSMTP)
                            {
                                smtp.SSLConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
                                smtp.Connect(email.ParamEmail.SMTP, (email.ParamEmail.PortSMTP == null ? 587 : (int)email.ParamEmail.PortSMTP));
                                smtp.StartTLS();
                            }
                            else
                            {
                                smtp.Connect(email.ParamEmail.SMTP, (email.ParamEmail.PortSMTP == null ? 25 : (int)email.ParamEmail.PortSMTP));
                            }
                            if (email.ParamEmail.AllowAuthSMTP)
                            {
                                smtp.UseBestLogin(email.ParamEmail.CompteSMTP, email.ParamEmail.PwdSMTP);
                            }
                        }
                        //Tentative d'envoi
                        try
                        {
                            smtp.SendMessage(msg);
                            //Mise à jour de l'email
                            email.DEnvoi = DateTime.Now;
                            email.RefUtilisateurCourant = refUtilisateur;
                            //Mise à jour le cas échéant
                            if (refUtilisateur != 0)
                            {
                                dbContext.SaveChanges();
                            }
                            //Fin
                            err = "ok";
                        }
                        catch (Exception ex)
                        {
                            //Renvoi de l'erreur
                            string s = ex.Message.Replace("'", "\'");
                            err = @s;
                        }
                        smtp.Close();
                    }
                }
                else
                {
                    //Renvoi de l'erreur
                    err = CurrentContext.CulturedRessources.GetTextRessource(678);
                }
            }
            else
            {
                //Renvoi de l'erreur
                err = CurrentContext.CulturedRessources.GetTextRessource(679);
            }
            //end
            return err;
        }

        /// <summary>
        /// Send an e-mailing
        /// </summary>
        /// <param name="email">e-mail template</param>
        /// <param name="emailing">The e-mailing to send</param>
        /// <param name="year">The year concerned</param>
        /// <param name="dbContext">The current EFCore context</param>
        /// <param name="currentContext">The current session context</param>
        /// <param name="configuration">The current IConfiguration</param>
        /// <returns>"ok", or the error message</returns>
        public static string SendEmailing(Email email, string emailing, int year, DateTime d, ApplicationDbContext dbContext, Context currentContext, IConfiguration configuration)
        {
            //Init
            string err = currentContext.CulturedRessources.GetTextRessource(677);
            int nb = 0;
            //Reload reference
            dbContext.Entry(email).Reference(s => s.ParamEmail).Load();
            //Process
            //Check parameters
            if (!string.IsNullOrEmpty(emailing))
            {
                SqlCommand cmd = new();
                DataSet dS = new();
                string sqlStr = "";
                switch (emailing)
                {
                    case "IncitationQualite":
                        if (year > 0)
                        {
                            sqlStr = "select distinct tblEntite.RefEntite"
                            + "     , tbmContactAdresse.Email"
                            + " from tblEntite"
                            + " 	inner join tblAdresse on tblAdresse.RefEntite=tblEntite.refEntite"
                            + " 	inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                            + " 	inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresse.RefContactAdresse = tbmContactAdresseContactAdresseProcess.RefContactAdresse"
                            + "     left join (select distinct RefEntite from  tblEmailIncitationQualite where Annee=@year) as tblEmailIncitationQualite on tblEntite.RefEntite=tblEmailIncitationQualite.RefEntite"
                            + " where tbmContactAdresseContactAdresseProcess.RefContactAdresseProcess = 8"
                            + "     and tblEmailIncitationQualite.RefEntite is null"
                            + " 	and tblEntite.RefEntite in (select RefEntite from tblContratIncitationQualite where year(DDebut) = @year)";
                            //Filters
                            cmd.Parameters.Add("@year", SqlDbType.NVarChar).Value = year;
                            using (SqlConnection sqlConn = new(configuration["Data:DefaultConnection:ConnectionString"]))
                            {
                                sqlConn.Open();
                                cmd.Connection = sqlConn;
                                cmd.CommandText = sqlStr;
                                SqlDataAdapter dA = new(cmd);
                                dA.Fill(dS);
                            }
                            foreach (DataRow dRow in dS.Tables[0].Rows)
                            {
                                //Send every e-mail
                                email.Message.Subject = "Valorplast – Incitation autocontrôle.";
                                email.Message.PriorityHigh();
                                email.Message.To.Clear();
                                email.Message.To.Add(new MailBox(dRow[1].ToString()));
                                err = Send(email, 0, dbContext, currentContext, configuration);
                                if (err == "ok")
                                {
                                    //Save email sended to dadatabe
                                    dbContext.EmailIncitationQualites.Add(new EmailIncitationQualite()
                                    {
                                        Annee = year,
                                        RefEntite = (int)dRow[0],
                                        EmailTo = dRow[1].ToString(),
                                        RefUtilisateurCreation = currentContext.RefUtilisateur
                                    });
                                    dbContext.SaveChanges();
                                    nb++;
                                }
                                else
                                {
                                    Utils.DebugPrint("Erreur envoi e-mail incitation qualité : " + err, configuration["Data:DefaultConnection:ConnectionString"]);
                                    break;
                                }
                            }
                        }
                        else { err = currentContext.CulturedRessources.GetTextRessource(617); }
                        break;
                    case "EmailNoteCreditCollectivite":
                        if (d != DateTime.MinValue)
                        {
                            //Filters
                            cmd.Parameters.Add("@year", SqlDbType.NVarChar).Value = year;
                            sqlStr = "select distinct tblEntite.RefEntite"
                                + "     , tbmContactAdresse.Email"
                                + "     , DO_PIECE"
                                + " from F_DOCENTETE"
                                + " 	inner join tblEntite on F_DOCENTETE.DO_TIERS=tblEntite.SAGECodeComptable"
                                + " 	inner join tblAdresse on tblAdresse.RefEntite=tblEntite.refEntite"
                                + " 	inner join tbmContactAdresse on tblAdresse.RefAdresse = tbmContactAdresse.RefAdresse"
                                + " 	inner join tbmContactAdresseContactAdresseProcess on tbmContactAdresse.RefContactAdresse = tbmContactAdresseContactAdresseProcess.RefContactAdresse"
                                + "     left join tblEmailNoteCredit on F_DOCENTETE.DO_PIECE=tblEmailNoteCredit.RefSAGEDocument"
                                + " where tbmContactAdresseContactAdresseProcess.RefContactAdresseProcess = 3 and tbmContactAdresse.Email is not null"
                                + "     and tbmContactAdresse.Actif=1 and tblAdresse.Actif=1 and tblEntite.Actif=1"
                                + " 	and DO_TYPE = 17 and DO_DATE >= @d"
                                + "     and tblEmailNoteCredit.RefSAGEDocument is null";
                            //Filters
                            cmd.Parameters.Add("@d", SqlDbType.SmallDateTime).Value = d;
                            using (SqlConnection sqlConn = new(configuration["Data:DefaultConnection:ConnectionString"]))
                            {
                                sqlConn.Open();
                                cmd.Connection = sqlConn;
                                cmd.CommandText = sqlStr;
                                SqlDataAdapter dA = new(cmd);
                                dA.Fill(dS);
                            }
                            foreach (DataRow dRow in dS.Tables[0].Rows)
                            {
                                //Send every e-mail
                                email.Message.Subject = "Valorplast : reprise des balles d'emballages plastiques.";
                                email.Message.PriorityHigh();
                                email.Message.To.Clear();
                                email.Message.To.Add(new MailBox(dRow[1].ToString()));
                                err = Send(email, 0, dbContext, currentContext, configuration);
                                if (err == "ok")
                                {
                                    //Save email sended to dadabase
                                    dbContext.EmailNoteCredits.Add(new EmailNoteCredit()
                                    {
                                        RefSAGEDocument = dRow["DO_PIECE"].ToString(),
                                        EmailTo = dRow[1].ToString(),
                                        RefUtilisateurCreation = currentContext.RefUtilisateur
                                    });
                                    dbContext.SaveChanges();
                                    nb++;
                                }
                                else
                                {
                                    Utils.DebugPrint("Erreur envoi note de crédit : " + err, configuration["Data:DefaultConnection:ConnectionString"]);
                                    break;
                                }
                            }
                        }
                        else { err = currentContext.CulturedRessources.GetTextRessource(617); }
                        break;
                    case "ReportingCollectiviteElu":
                        if (year > 0)
                        {
                            var cAs = dbContext.ContactAdresses.Where(e => e.ContactAdresseDocumentTypes.Any(ca => ca.RefDocumentType == (int)Enumerations.RefDocumentType.ReportingCollectiviteElu)
                                && !string.IsNullOrWhiteSpace(e.Email)
                                && e.Actif == true && e.Entite.Actif == true && (e.Adresse == null || e.Adresse.Actif)).ToList();
                            foreach (var cA in cAs)
                            {
                                //Send every e-mail
                                email.Message.Subject = "Valorplast : synthèse de la reprise des balles d'emballages plastiques pour " + year.ToString() + ".";
                                email.Message.To.Clear();
                                email.Message.To.Add(new MailBox(cA.Email));
                                //Create attachment
                                MemoryStream ms = DocumentFileManagement.CreateDocumentType1(cA.RefEntite, dbContext);
                                if (ms!=null)
                                {
                                    var mBuilder = email.Message.ToBuilder();
                                    //Ajout du fichier
                                    MimeData att1 = mBuilder.AddAttachment(ms.ToArray());
                                    att1.ContentType = ContentType.ApplicationPdf;
                                    att1.FileName = "Reporting.pdf";
                                    email.Message = mBuilder.Create();
                                }
                                //Send e-mail
                                err = Send(email, 0, dbContext, currentContext, configuration);
                                if (err == "ok")
                                {
                                    //Save email sended to dadabase
                                    dbContext.EmailDocumentTypes.Add(new EmailDocumentType()
                                    {
                                        RefDocumentType = (int)Enum.Parse(typeof(Enumerations.RefDocumentType), emailing),
                                        RefEntite = cA.RefEntite,
                                        RefContactAdresse = cA.RefContactAdresse,
                                        Annee = year,
                                        D = DateOnly.FromDateTime(d),
                                        EmailTo = cA.Email,
                                        RefUtilisateurCreation = currentContext.RefUtilisateur
                                    });
                                    dbContext.SaveChanges();
                                    nb++;
                                }
                                else
                                {
                                    Utils.DebugPrint("Erreur envoi " + Enum.Parse(typeof(Enumerations.RefDocumentType), emailing).ToString() + " : " + err, configuration["Data:DefaultConnection:ConnectionString"]);
                                    break;
                                }
                                //Clear attachments
                                email.Message.RemoveAttachments();
                            }
                        }
                        else { err = currentContext.CulturedRessources.GetTextRessource(617); }
                        break;
                    case "ReportingCollectiviteGrandPublic":
                        if (year > 0)
                        {
                            var cAs = dbContext.ContactAdresses.Where(e => e.ContactAdresseDocumentTypes.Any(ca => ca.RefDocumentType == (int)Enumerations.RefDocumentType.ReportingCollectiviteGrandPublic)
                                && !string.IsNullOrWhiteSpace(e.Email)
                                && e.Actif == true && e.Entite.Actif == true && (e.Adresse == null || e.Adresse.Actif)).ToList();
                            foreach (var cA in cAs)
                            {
                                //Send every e-mail
                                email.Message.Subject = "Valorplast : synthèse de la reprise des balles d'emballages plastiques pour " + year.ToString() + ".";
                                email.Message.To.Clear();
                                email.Message.To.Add(new MailBox(cA.Email));
                                //Create attachment
                                MemoryStream ms = PresentationFileManagement.CreateDocumentType2(cA.RefEntite, dbContext);
                                if (ms != null)
                                {
                                    var mBuilder = email.Message.ToBuilder();
                                    //Ajout du fichier
                                    MimeData att1 = mBuilder.AddAttachment(ms.ToArray());
                                    att1.ContentType = ContentType.ApplicationVndMsPowerPoint;
                                    att1.FileName = "Reporting.pptx";
                                    email.Message = mBuilder.Create();
                                }
                                //Send e-mail
                                err = Send(email, 0, dbContext, currentContext, configuration);
                                if (err == "ok")
                                {
                                    //Save email sended to dadabase
                                    dbContext.EmailDocumentTypes.Add(new EmailDocumentType()
                                    {
                                        RefDocumentType = (int)Enum.Parse(typeof(Enumerations.RefDocumentType), emailing),
                                        RefEntite = cA.RefEntite,
                                        RefContactAdresse = cA.RefContactAdresse,
                                        Annee = year,
                                        D = DateOnly.FromDateTime(d),
                                        EmailTo = cA.Email,
                                        RefUtilisateurCreation = currentContext.RefUtilisateur
                                    });
                                    dbContext.SaveChanges();
                                    nb++;
                                }
                                else
                                {
                                    Utils.DebugPrint("Erreur envoi " + Enum.Parse(typeof(Enumerations.RefDocumentType), emailing).ToString() + " : " + err, configuration["Data:DefaultConnection:ConnectionString"]);
                                    break;
                                }
                                //Clear attachments
                                email.Message.RemoveAttachments();
                            }
                        }
                        else { err = currentContext.CulturedRessources.GetTextRessource(617); }
                        break;
                    default:
                        err = currentContext.CulturedRessources.GetTextRessource(617);
                        break;
                }
            }
            else { err = currentContext.CulturedRessources.GetTextRessource(617); }
            //End
            if (err == "ok")
            { return nb.ToString(); }
            else { return err; }
        }
        /// <summary>
        /// Send an e-mail regarding password management
        /// </summary>
        /// <param name="emailType">e-mail type</param>
        /// <param name="utilisateur">the recipient user</param>
        /// <param name="refUtilisateur">the connected user id</param>
        /// <param name="dbContext">The current EFCore context</param>
        /// <param name="ctx">The session context for resources</param>
        /// <param name="configuration">The current IConfiguration</param>
        /// <returns>"ok", or the error message</returns>
        public static string SendEmailPasswordManagement(string emailType, Utilisateur utilisateur, int refUtilisateur, ApplicationDbContext dbContext, Context ctx, IConfiguration configuration)
        {
            //Init
            string err = ctx.CulturedRessources.GetTextRessource(677);
            //Process
            //Check parameters
            if (!string.IsNullOrEmpty(emailType))
            {
                //Search for ParamEmail
                var paramEmail = dbContext.ParamEmails.Where(el => el.Actif && el.Defaut).FirstOrDefault();
                if (paramEmail != null)
                {
                    string body = "";
                    Email email = new();
                    IMail msg;
                    MailBuilder mBuilder;
                    //Init
                    email.Entrant = false;
                    email.ParamEmail = paramEmail;
                    email.RefUtilisateurCourant = 1;
                    msg = email.Message;
                    msg.From.Add(new MailBox("e-valorplast@valorplast.com", "Valorplast"));
                    //Create e-mail
                    dbContext.Emails.Add(email);
                    int refL = 0;
                    Lien l = new();
                    switch (emailType)
                    {
                        case "HasMaitre":
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                msg.Subject = "e-Valorplast password reset denied";
                            }
                            else
                            {
                                msg.Subject = "Réinitialisation impossible du mot de passe e-Valorplast";
                            }
                            //Création du corps du message
                            body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("This email has been sent automatically, do not reply.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Hello " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("You have multi-profile access and therefore cannot reset the password for this account. Please use the e-mail address of your main access.") + " <br/>"
                                    + "</p><p><strong>"
                                    + WebUtility.HtmlEncode("Team Valorplast");
                            }
                            else
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("Cet email a été envoyé automatiquement, ne pas y répondre.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Bonjour " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("Vous disposez d'un accès multi-profil et ne pouvez donc pas réinitialiser le mot de passe de ce compte. Merci d’utiliser l'e-mail de votre accès principal.") + " <br/>"
                                    + "</p><p><strong>"
                                    + WebUtility.HtmlEncode("L’équipe Valorplast");
                            }
                            body += "</strong></p></span>";
                            break;
                        case "Creation":
                            //Create link, exit if error
                            refL = Utils.GenerateLien(utilisateur.RefUtilisateur, 1, ctx.CurrentCulture.Name, dbContext, configuration);
                            try { l = dbContext.Liens.Find(refL); }
                            catch
                            {
                                err = ctx.CulturedRessources.GetTextRessource(1098);
                                Utils.DebugPrint("Erreur envoi e-mail mot de passe - " + emailType + " : création de lien", configuration["Data: DefaultConnection:ConnectionString"]);
                                return err;
                            }
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                msg.Subject = "e-Valorplast password creation";
                            }
                            else
                            {
                                msg.Subject = "Création de mot de passe e-Valorplast";
                            }
                            //Création du corps du message
                            body += "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("This email has been sent automatically, do not reply.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Hello " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("We’re sending you this email because you requested a password creation. Click on the link below to create a password:") + " <br/>"
                                    + "<a href='" + configuration["AppParameters:url"] + "?pwdlost=" + l.Valeur + "'>" + WebUtility.HtmlEncode("PASSWORD CREATION") + "</a><br/><br/>"
                                    + WebUtility.HtmlEncode("This link is valid for "
                                    + ((double)Utils.GetParametre(6, dbContext).ValeurNumerique).ToString() + " hour(s). Its validity will expire at " + l.DCreation.AddHours(2).ToShortTimeString() + " French time.") + " <br/>"
                                    + "</p><p><strong>"
                                    + WebUtility.HtmlEncode("Team Valorplast");
                            }
                            else
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("Cet email a été envoyé automatiquement, ne pas y répondre.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Bonjour " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("Nous vous envoyons ce courriel parce que vous avez demandé une création de mot de passe. Cliquez sur le lien ci-dessous pour créer un mot de passe :") + " <br/>"
                                    + "<a href='" + configuration["AppParameters:url"] + "?pwdlost=" + l.Valeur + "'>" + WebUtility.HtmlEncode("CREER MON MOT DE PASSE") + "</a><br/><br/>"
                                    + WebUtility.HtmlEncode("Ce lien est valide pendant "
                                    + ((double)Utils.GetParametre(6, dbContext).ValeurNumerique).ToString() + " heure(s). Sa validité prendra fin à " + l.DCreation.AddHours(2).ToShortTimeString() + ".") + " <br/>"
                                    + "</p><p><strong>"
                                    + WebUtility.HtmlEncode("L’équipe Valorplast");
                            }
                            body += "</strong></p></span>";
                            break;
                        case "Reset":
                            //Create link, exit if error
                            refL = Utils.GenerateLien(utilisateur.RefUtilisateur, 1, ctx.CurrentCulture.Name, dbContext, configuration);
                            try { l = dbContext.Liens.Find(refL); }
                            catch
                            {
                                err = ctx.CulturedRessources.GetTextRessource(1098);
                                Utils.DebugPrint("Erreur envoi e-mail mot de passe - " + emailType + " : création de lien", configuration["Data: DefaultConnection:ConnectionString"]);
                                return err;
                            }
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                msg.Subject = "e-Valorplast password reset";
                            }
                            else
                            {
                                msg.Subject = "Réinitialisation de mot de passe e-Valorplast";
                            }
                            //Création du corps du message
                            body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("This email has been sent automatically, do not reply.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Hello " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("We’re sending you this email because you requested a password reset. Click on the link below to create a new password:") + " <br/>"
                                    + "<a href='" + configuration["AppParameters:url"] + "?pwdlost=" + l.Valeur + "'>" + WebUtility.HtmlEncode("PASSWORD RESET") + "</a><br/><br/>"
                                    + WebUtility.HtmlEncode("This link is valid for "
                                    + ((double)Utils.GetParametre(6, dbContext).ValeurNumerique).ToString() + " hour(s). Its validity will expire at " + l.DCreation.AddHours(2).ToShortTimeString() + " French time.") + " <br/>"
                                    + "</p><p><strong>"
                                + WebUtility.HtmlEncode("Team Valorplast");
                            }
                            else
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("Cet email a été envoyé automatiquement, ne pas y répondre.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Bonjour " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("Nous vous envoyons ce courriel parce que vous avez demandé une réinitialisation de mot de passe. Cliquez sur le lien ci-dessous pour créer un nouveau mot de passe :") + " <br/>"
                                    + "<a href='" + configuration["AppParameters:url"] + "?pwdlost=" + l.Valeur + "'>" + WebUtility.HtmlEncode("REINITIALISER MON MOT DE PASSE") + "</a><br/><br/>"
                                    + WebUtility.HtmlEncode("Ce lien est valide pendant "
                                    + ((double)Utils.GetParametre(6, dbContext).ValeurNumerique).ToString() + " heure(s). Sa validité prendra fin à " + l.DCreation.AddHours(2).ToShortTimeString() + ".") + " <br/>"
                                    + "</p><p><strong>"
                                    + WebUtility.HtmlEncode("L’équipe Valorplast");
                            }
                            body += "</strong></p></span>";
                            break;
                        case "CreationConfirmation":
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                msg.Subject = "e-Valorplast password creation confirmation";
                            }
                            else
                            {
                                msg.Subject = "Confirmation création de mot de passe e-Valorplast";
                            }
                            //Création du corps du message
                            body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("This email has been sent automatically, do not reply.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Hello " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("We confirm that you have created your password. If you did not initiate this request, please contact Valorplast (" + Utils.GetParametre(7, dbContext).ValeurTexte + ").") + " <br/>"
                                    + "</p><p><strong>"
                                    + WebUtility.HtmlEncode("Team Valorplast");
                            }
                            else
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("Cet email a été envoyé automatiquement, ne pas y répondre.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Bonjour " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("Nous vous confirmons la création de votre mot de passe. Si vous n’êtes pas à l’origine de cette demande, merci de contacter Valorplast (" + Utils.GetParametre(7, dbContext).ValeurTexte + ").") + " <br/>"
                                    + "</p><p><strong>"
                                    + WebUtility.HtmlEncode("L’équipe Valorplast");
                            }
                            body += "</strong></p></span>";
                            break;
                        case "ResetConfirmation":
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                msg.Subject = "e-Valorplast password reset confirmation";
                            }
                            else
                            {
                                msg.Subject = "Confirmation de réinitialisation de mot de passe e-Valorplast";
                            }
                            //Création du corps du message
                            body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                            if (ctx.CurrentCulture.Name == "en-GB")
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("This email has been sent automatically, do not reply.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Hello " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("We confirm that you have reset your password. If you did not initiate this request, please contact Valorplast (" + Utils.GetParametre(7, dbContext).ValeurTexte + ").") + " <br/>"
                                    + "</p><p><strong>"
                                    + WebUtility.HtmlEncode("Team Valorplast");
                            }
                            else
                            {
                                body += "<hr/>"
                                    + "<div align=\"center\">"
                                    + "<strong>" + WebUtility.HtmlEncode("Cet email a été envoyé automatiquement, ne pas y répondre.") + "</strong></div>"
                                    + "</div>"
                                    + "<hr/>";
                                body += "<p>" + WebUtility.HtmlEncode("Bonjour " + utilisateur.Nom + ",")
                                    + "</p>"
                                    + "<p>"
                                    + WebUtility.HtmlEncode("Nous vous confirmons la réinitialisation de votre mot de passe. Si vous n’êtes pas à l’origine de cette demande, merci de contacter Valorplast (" + Utils.GetParametre(7, dbContext).ValeurTexte + ").") + " <br/>"
                                    + "</p><p><strong>"
                                    + WebUtility.HtmlEncode("L’équipe Valorplast");
                            }
                            body += "</strong></p></span>";
                            break;
                    }
                    //Send e-mail
                    mBuilder = msg.ToBuilder();
                    mBuilder.Html = body;
                    msg = mBuilder.Create();
                    msg.HtmlData.ContentTransferEncoding = MimeEncoding.Base64;
                    //Destinataire
                    msg.To.Add(new MailBox(utilisateur.EMail));
                    //Objet
                    email.Message = msg;
                    email.currentCulture = ctx.CurrentCulture;
                    dbContext.SaveChanges();
                    dbContext.Entry(email).Reload();
                    string s = EmailUtils.Send(email, refUtilisateur, dbContext, ctx, configuration);
                    //End
                    if (s != "ok")
                    {
                        err = ctx.CulturedRessources.GetTextRessource(1098);
                        Utils.DebugPrint("Erreur envoi e-mail mot de passe - " + emailType + " : " + s, configuration["Data: DefaultConnection:ConnectionString"]);
                    }
                    else
                    {
                        err = "ok";
                    }
                }
                else
                {
                    err = ctx.CulturedRessources.GetTextRessource(1098);
                    Utils.DebugPrint("Erreur envoi e-mail mot de passe - " + emailType + " : pas de paramétrage e-mail", configuration["Data:DefaultConnection:ConnectionString"]);
                }
            }
            else
            {
                err = ctx.CulturedRessources.GetTextRessource(1098);
                Utils.DebugPrint("Erreur envoi e-mail mot de passe - " + emailType + " : pas de type d'e-mail", configuration["Data:DefaultConnection:ConnectionString"]);
            }
            return err;
        }
        /// <summary>
        /// Send an e-mail regarding maître assignment
        /// </summary>
        /// <param name="emailType">e-mail type</param>
        /// <param name="utilisateur">the recipient user</param>
        /// <param name="refUtilisateur">the connected user id</param>
        /// <param name="dbContext">The current EFCore context</param>
        /// <param name="ctx">The session context for resources</param>
        /// <param name="configuration">The current IConfiguration</param>
        /// <returns>"ok", or the error message</returns>
        public static string SendEmailAssignmentMaitre(string emailType, Utilisateur utilisateur, int refUtilisateur, ApplicationDbContext dbContext, Context ctx, IConfiguration configuration)
        {
            //Init
            string err = ctx.CulturedRessources.GetTextRessource(677);
            //Process
            //Check parameters
            if (!string.IsNullOrEmpty(emailType))
            {
                //Search for ParamEmail
                var paramEmail = dbContext.ParamEmails.Where(el => el.Actif && el.Defaut).FirstOrDefault();
                if (paramEmail != null)
                {
                    string body = "";
                    Email email = new();
                    IMail msg;
                    MailBuilder mBuilder;
                    //Init
                    email.Entrant = false;
                    email.ParamEmail = paramEmail;
                    email.RefUtilisateurCourant = 1;
                    msg = email.Message;
                    msg.From.Add(new MailBox("e-valorplast@valorplast.com", "Valorplast"));
                    //Create e-mail
                    dbContext.Emails.Add(email);
                    int refL = 0;
                    Lien l = new();
                    switch (emailType)
                    {
                        case "AssignmentMaitre":
                            //init
                            dbContext.Entry(utilisateur).Reference(e => e.UtilisateurMaitre).Load();
                            //Subject
                            msg.Subject = "Affectation d'un maître";
                            //Création du corps du message
                            body = "<span style=\"font-family:Gill Sans MT; font-size:11pt;\">";
                            body += "<hr/>"
                                + "<div align=\"center\">"
                                + "<strong>" + WebUtility.HtmlEncode("Cet email a été envoyé automatiquement, ne pas y répondre.") + "</strong></div>"
                                + "</div>"
                                + "<hr/>";
                            body += "<p>"
                                + WebUtility.HtmlEncode(DateTime.Now.ToString("dd/MM/yyyy hh:mm")) + " <br/>"
                                + WebUtility.HtmlEncode("Une affectation d'un profil à un maître vient d'être effectuée :") + " <br/>"
                                + WebUtility.HtmlEncode(" - profil : ") + utilisateur.Nom + " - " + utilisateur.UtilisateurType + " - " + utilisateur.EntiteLibelle + " <br/>"
                                + WebUtility.HtmlEncode(" - maître : ") + utilisateur.UtilisateurMaitre.Nom + " - " + utilisateur.UtilisateurMaitre.UtilisateurType + " - " + utilisateur.UtilisateurMaitre.EntiteLibelle + " <br/>"
                                + WebUtility.HtmlEncode(" - effectué par : " + ctx.ConnectedUtilisateur.Nom) + " <br/>"
                                + "</p><p><strong>"
                                + WebUtility.HtmlEncode("e-Valorplast");
                            body += "</strong></p></span>";
                            break;
                    }
                    //Send e-mail
                    mBuilder = msg.ToBuilder();
                    mBuilder.Html = body;
                    msg = mBuilder.Create();
                    msg.HtmlData.ContentTransferEncoding = MimeEncoding.Base64;
                    //Destinataire
                    msg.To.Add(new MailBox(Utils.GetParametre(7, dbContext).ValeurTexte));
                    //Objet
                    email.Message = msg;
                    email.currentCulture = ctx.CurrentCulture;
                    dbContext.SaveChanges();
                    dbContext.Entry(email).Reload();
                    string s = EmailUtils.Send(email, refUtilisateur, dbContext, ctx, configuration);
                    //End
                    if (s != "ok")
                    {
                        err = ctx.CulturedRessources.GetTextRessource(1098);
                        Utils.DebugPrint("Erreur envoi e-mail mot de passe - " + emailType + " : " + s, configuration["Data: DefaultConnection:ConnectionString"]);
                    }
                    else
                    {
                        err = "ok";
                    }
                }
                else
                {
                    err = ctx.CulturedRessources.GetTextRessource(1098);
                    Utils.DebugPrint("Erreur envoi e-mail mot de passe - " + emailType + " : pas de paramétrage e-mail", configuration["Data:DefaultConnection:ConnectionString"]);
                }
            }
            else
            {
                err = ctx.CulturedRessources.GetTextRessource(1098);
                Utils.DebugPrint("Erreur envoi e-mail mot de passe - " + emailType + " : pas de type d'e-mail", configuration["Data:DefaultConnection:ConnectionString"]);
            }
            return err;
        }
    }
}

