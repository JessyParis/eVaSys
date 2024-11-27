/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/11/2019
/// -----------------------------------------------------------------------------------------------------
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class MessageController : BaseApiController
    {
        private readonly IMapper _mapper;

        #region Constructor

        public MessageController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }

        #endregion Constructor

        #region RESTful Conventions

        /// <summary>
        /// GET: evapi/message/{id}
        /// Retrieves the Message with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Message</param>
        /// <param name="visualisations">boolean: true to load MessageVisualisations</param>
        /// <returns>the Message with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            bool visualisations = Request.Headers["visualisations"].ToString() == "true" ? true : false;
            Message message = null;
            //Get or create message
            if (id == 0)
            {
                message = new Message();
                DbContext.Messages.Add(message);
            }
            else
            {
                if (visualisations)
                {
                    message = DbContext.Messages
                        .Include(r => r.MessageDiffusions)
                        .Include(r => r.MessageVisualisations)
                        .ThenInclude(r => r.Utilisateur)
                        .Include(r => r.UtilisateurCreation)
                        .Include(r => r.UtilisateurModif)
                        .Where(i => i.RefMessage == id).FirstOrDefault();
                }
                else
                {
                    message = DbContext.Messages
                        .Include(r => r.MessageDiffusions)
                        .Include(r => r.UtilisateurCreation)
                        .Include(r => r.UtilisateurModif)
                        .Where(i => i.RefMessage == id).FirstOrDefault();
                }
            }
            // handle requests asking for non-existing object
            if (message == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            message.currentCulture = CurrentContext.CurrentCulture;
            return new JsonResult(
                _mapper.Map<Message, MessageViewModel>(message),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Message with the given {id}
        /// </summary>
        /// <param name="model">The MessageViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] MessageViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Message message;
            //Retrieve or create the entity to edit
            if (model.RefMessage > 0)
            {
                message = DbContext.Messages
                        .Where(q => q.RefMessage == model.RefMessage)
                        .Include(r => r.MessageDiffusions)
                        .Include(r => r.MessageVisualisations)
                        .FirstOrDefault();
            }
            else
            {
                message = new Message();
                DbContext.Messages.Add(message);
            }
            // handle requests asking for non-existing messagezes
            if (message == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            UpdateData(ref message, model);

            //Register session user
            message.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = message.IsValid();
            //End
            if (string.IsNullOrEmpty(valid))
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                //Return the updated Message to the client
                return new JsonResult(
                    _mapper.Map<Message, MessageViewModel>(message),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Message with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the message from the Database
            var message = DbContext.Messages.Where(i => i.RefMessage == id)
                .Include(rel => rel.MessageDiffusions)
                .Include(rel => rel.MessageVisualisations)
                .FirstOrDefault();

            // handle requests asking for non-existing messagezes
            if (message == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = message.IsDeletable();
            if (del == "")
            {
                DbContext.Messages.Remove(message);
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return an HTTP Status 200 (OK).
                return Ok();
            }
            else
            {
                return Conflict(new ConflictError(del));
            }
        }

        #endregion RESTful Conventions

        #region Attribute-based Routing

        /// <summary>
        /// GET: api/messages/resetmessageread
        /// ROUTING TYPE: attribute-based
        /// delete all read for current user today
        /// </summary>
        /// <returns>true</returns>
        [HttpGet("resetmessageread")]
        public IActionResult ResetMessageRead()
        {
            var mVs = DbContext.MessageVisualisations
                .Where(i => i.RefUtilisateur == CurrentContext.RefUtilisateur
                && i.D.Year == DateTime.Now.Year && i.D.Month == DateTime.Now.Month && i.D.Day == DateTime.Now.Day)
                .ToList();
            foreach (var mV in mVs)
            {
                var msg = DbContext.Messages.Find(mV.RefMessage);
                if (msg.DiffusionUnique == false || mV.Confirme == false)
                {
                    DbContext.Remove(mV);
                }
            }
            DbContext.SaveChanges();
            return new JsonResult(true, JsonSettings);
        }

        /// <summary>
        /// GET: api/messages/getmessagetodisplay
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>The next message to display to the connected user</returns>
        [HttpGet("getmessagetodisplay")]
        public IActionResult GetMessageToDisplay()
        {
            bool notSeenToday = Request.Headers["notSeenToday"].ToString() == "true" ? true : false;
            string sqlStr = "select distinct tbmMessageDiffusion.RefMessage"
                + " from tblMessage"
                + " 	inner join tbmMessageDiffusion on tbmMessageDiffusion.RefMessage=tblMessage.RefMessage"
                + " 	left join (select RefMessage, count(*) as nb from tbmMessageVisualisation where RefUtilisateur=" + CurrentContext.RefUtilisateur + " and Confirme=1 group by RefMessage) as visu on tblMessage.RefMessage=visu.RefMessage"
                + " where getdate() >= DDebut and getdate() < dateadd(day,1,DFin) and Actif=1"
                + " 	and (DiffusionUnique=0 or (DiffusionUnique=1 and nb is null))";
            if (notSeenToday)
            {
                sqlStr += "     and tblMessage.RefMessage not in (select distinct RefMessage from tbmMessageVisualisation where RefUtilisateur = " + CurrentContext.RefUtilisateur + " and DATEDIFF(day, D, getdate())=0)";
            }
            sqlStr += " 	and ("
             + " 		(RefModule is null and RefHabilitation is null and RefUtilisateur=" + CurrentContext.RefUtilisateur + ")";
            if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationAdministration))
            {
                sqlStr += " 		or(RefModule='Administration' and RefHabilitation is null and RefUtilisateur is null)"
                + " 		or(RefModule='Administration' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationAdministration + "' and RefUtilisateur is null)"
                + " 		or(RefModule='Administration' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationAdministration + "' and RefUtilisateur=" + CurrentContext.RefUtilisateur + ")";
            }
            if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationAnnuaire))
            {
                sqlStr += " 		or(RefModule='Annuaire' and RefHabilitation is null and RefUtilisateur is null)"
                + " 		or(RefModule='Annuaire' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationAnnuaire + "' and RefUtilisateur is null)"
                + " 		or(RefModule='Annuaire' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationAnnuaire + "' and RefUtilisateur=" + CurrentContext.RefUtilisateur + ")";
            }
            if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationLogistique))
            {
                sqlStr += " 		or(RefModule='Logistique' and RefHabilitation is null and RefUtilisateur is null)"
                + " 		or(RefModule='Logistique' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationLogistique + "' and RefUtilisateur is null)"
                + " 		or(RefModule='Logistique' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationLogistique + "' and RefUtilisateur=" + CurrentContext.RefUtilisateur + ")";
            }
            if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationMessagerie))
            {
                sqlStr += " 		or(RefModule='Messagerie' and RefHabilitation is null and RefUtilisateur is null)"
                + " 		or(RefModule='Messagerie' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationMessagerie + "' and RefUtilisateur is null)"
                + " 		or(RefModule='Messagerie' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationMessagerie + "' and RefUtilisateur=" + CurrentContext.RefUtilisateur + ")";
            }
            if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationModuleCollectivite))
            {
                sqlStr += " 		or(RefModule='ModuleCollectivite' and RefHabilitation is null and RefUtilisateur is null)"
                + " 		or(RefModule='ModuleCollectivite' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationModuleCollectivite + "' and RefUtilisateur is null)"
                + " 		or(RefModule='ModuleCollectivite' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationModuleCollectivite + "' and RefUtilisateur=" + CurrentContext.RefUtilisateur + ")";
            }
            if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationQualite))
            {
                sqlStr += " 		or(RefModule='Qualite' and RefHabilitation is null and RefUtilisateur is null)"
                + " 		or(RefModule='Qualite' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationQualite + "' and RefUtilisateur is null)"
                + " 		or(RefModule='Qualite' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationQualite + "' and RefUtilisateur=" + CurrentContext.RefUtilisateur + ")";
            }
            if (!string.IsNullOrEmpty(CurrentContext.ConnectedUtilisateur.HabilitationStatistique))
            {
                sqlStr += " 		or(RefModule='Statistique' and RefHabilitation is null and RefUtilisateur is null)"
                + " 		or(RefModule='Statistique' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationStatistique + "' and RefUtilisateur is null)"
                + " 		or(RefModule='Statistique' and RefHabilitation='" + CurrentContext.ConnectedUtilisateur.HabilitationStatistique + "' and RefUtilisateur=" + CurrentContext.RefUtilisateur + ")";
            }
            sqlStr += "     )";
            string refMessage = Utils.Utils.DbScalar(sqlStr, DbContext.Database.GetDbConnection());
            string nbMessage = Utils.Utils.DbScalar("select count(*) from (" + sqlStr + ") as u", DbContext.Database.GetDbConnection());
            //If only check if a message have to be shown
            bool checkExists = Request.Headers["checkExists"].ToString() == "true" ? true : false;
            if (checkExists)
            {
                return new JsonResult(nbMessage, JsonSettings);
                if (refMessage != "") { return new JsonResult(true, JsonSettings); }
                else { return new JsonResult(false, JsonSettings); }
            }
            else
            {
                //Mark a message as read if applicable
                string refMessageRead = Request.Headers["refMessageRead"].ToString();
                if (refMessageRead != "")
                {
                    int refM = 0;
                    int.TryParse(refMessageRead, out refM);
                    if (refM != 0)
                    {
                        MessageVisualisation mV = DbContext.MessageVisualisations
                            .Where(el => el.RefMessage == refM && el.D.Year == DateTime.Now.Year && el.D.Month == DateTime.Now.Month && el.D.Day == DateTime.Now.Day
                            && el.RefUtilisateur == CurrentContext.RefUtilisateur)
                            .FirstOrDefault();
                        if (mV != null)
                        {
                            mV.Confirme = true;
                            //Deacivate message if unique confirmed visualisation
                            var message = DbContext.Messages.Where(el => el.RefMessage == mV.RefMessage).FirstOrDefault();
                            if(message != null)
                            {
                                if (message.VisualisationConfirmeUnique == true)
                                {
                                    message.Actif = false;
                                }
                            }
                            DbContext.SaveChanges();
                        }
                    }
                }
                if (refMessage != "")
                {
                    int r = 0;
                    int.TryParse(refMessage, out r);
                    Message msg = DbContext.Messages.Where(el => el.RefMessage == r).FirstOrDefault();
                    if (msg != null)
                    {
                        //Mark as shown if applicable
                        MessageVisualisation mV = new()
                        {
                            D = DateTime.Now,
                            RefUtilisateur = CurrentContext.RefUtilisateur
                        };
                        DbContext.MessageVisualisations.Add(mV);
                        if (msg.MessageVisualisations == null) { msg.MessageVisualisations = new HashSet<MessageVisualisation>(); }
                        msg.MessageVisualisations.Add(mV);
                        DbContext.SaveChanges();
                        //Return Json
                        return new JsonResult(
                            _mapper.Map<Message, MessageViewModel>(msg),
                            JsonSettings);
                    }
                    else
                    {
                        return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
                    }
                }
                else
                {
                    return new EmptyResult();
                }
            }
        }

        #endregion Attribute-based Routing

        #region Services

        /// <summary>
        /// Write received data to dto
        /// </summary>
        private void UpdateData(ref Message dataModel, MessageViewModel viewModel)
        {
            dataModel.RefMessageType = viewModel.MessageType.RefMessageType;
            dataModel.Actif = viewModel.Actif ?? false;
            dataModel.Important = viewModel.Important ?? false;
            dataModel.VisualisationConfirmeUnique = viewModel.VisualisationConfirmeUnique ?? false;
            dataModel.DiffusionUnique = viewModel.DiffusionUnique ?? false;
            dataModel.RefMessageType = viewModel.MessageType.RefMessageType;
            dataModel.Libelle = Utils.Utils.SetEmptyStringToNull(viewModel.Libelle);
            dataModel.Titre = Utils.Utils.SetEmptyStringToNull(viewModel.Titre);
            dataModel.DDebut = viewModel.DDebut;
            dataModel.DFin = viewModel.DFin;
            dataModel.Corps = Utils.Utils.SetEmptyStringToNull(viewModel.Corps);
            dataModel.CorpsHTML = Utils.Utils.SetEmptyStringToNull(viewModel.CorpsHTML);
            dataModel.Cmt = Utils.Utils.SetEmptyStringToNull(viewModel.Cmt);
            //Dirty marker
            bool dirty = false;
            //Remove related data MessageDiffusion
            if (dataModel.MessageDiffusions != null)
            {
                foreach (MessageDiffusion mD in dataModel.MessageDiffusions)
                {
                    if (viewModel.MessageDiffusions == null)
                    {
                        DbContext.MessageDiffusions.Remove(mD);
                        dirty = true;
                    }
                    else if (viewModel.MessageDiffusions.Where(el => el.RefMessageDiffusion == mD.RefMessageDiffusion).FirstOrDefault() == null)
                    {
                        DbContext.MessageDiffusions.Remove(mD);
                        dirty = true;
                    }
                }
            }
            //Add or update related data MessageDiffusions
            foreach (MessageDiffusionViewModel mDVM in viewModel.MessageDiffusions)
            {
                MessageDiffusion mD = null;
                if (dataModel.MessageDiffusions != null && mDVM.RefMessageDiffusion != 0)
                {
                    mD = dataModel.MessageDiffusions.Where(el => el.RefMessageDiffusion == mDVM.RefMessageDiffusion).FirstOrDefault();
                }
                if (mD == null)
                {
                    mD = new MessageDiffusion();
                    DbContext.MessageDiffusions.Add(mD);
                    if (dataModel.MessageDiffusions == null) { dataModel.MessageDiffusions = new HashSet<MessageDiffusion>(); }
                    dataModel.MessageDiffusions.Add(mD);
                }
                //Mark as dirty if applicable
                if (mD.RefMessage != mDVM.RefMessage
                    || mD.RefModule != mDVM.RefModule
                    || mD.RefHabilitation != mDVM.RefHabilitation
                    || mD.RefUtilisateur != mDVM.Utilisateur?.RefUtilisateur) { dirty = true; }
                //Update data
                mD.RefMessage = mDVM.RefMessage;
                mD.RefModule = mDVM.RefModule;
                mD.RefHabilitation = mDVM.RefHabilitation;
                mD.RefUtilisateur = mDVM.Utilisateur?.RefUtilisateur;
            }
            //Remove duplicates
            if (dataModel.MessageDiffusions != null)
            {
                var resultMD = dataModel.MessageDiffusions
                    .AsEnumerable()
                    .GroupBy(s => new { s.RefModule, s.RefHabilitation, s.RefUtilisateur })
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultMD.Count > 0)
                {
                    foreach (var e in resultMD)
                    {
                        dataModel.MessageDiffusions.Remove(e);
                    }
                    dirty = true;
                }
            }
            //Mark for modification if applicable
            if (dirty) { dataModel.DModif = DateTime.Now; }
        }

        #endregion Services
    }
}