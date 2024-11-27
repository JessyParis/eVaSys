/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/05/2024
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
using System.Linq;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class DocumentController : BaseApiController
    {
        private readonly IMapper _mapper;

        #region Constructor

        public DocumentController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }

        #endregion Constructor

        #region RESTful Conventions

        /// <summary>
        /// GET: evapi/document/{id}
        /// Retrieves the Document with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Document</param>
        /// <returns>the Document with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var document = DbContext.Documents
                .Include(i => i.DocumentEntites)
                .ThenInclude(i=>i.Entite)
                .Include(i => i.DocumentEntiteTypes)
                .Include(r => r.UtilisateurCreation)
                .Include(r => r.UtilisateurModif)
                .Where(i => i.RefDocument == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (document == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<Document, DocumentViewModel>(document),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Document with the given {id}
        /// </summary>
        /// <param name="model">The DocumentViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] DocumentViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            Document document;
            //Retrieve or create the entity to edit
            if (model.RefDocument > 0)
            {
                document = DbContext.Documents
                    .Include(i => i.DocumentEntites)
                    .Include(i => i.DocumentEntiteTypes)
                    .Where(q => q.RefDocument == model.RefDocument)
                    .FirstOrDefault();
            }
            else
            {
                document = new Document();
                DbContext.Documents.Add(document);
            }
            // handle requests asking for non-existing documentzes
            if (document == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            Utils.DataUtils.UpdateDataDocument(ref document, model, CurrentContext.RefUtilisateur);

            //Register session user
            document.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = document.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Document to the client.
                return new JsonResult(
                    _mapper.Map<Document, DocumentViewModel>(document),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the Document with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the document from the Database
            var document = DbContext.Documents
                .Include(r => r.DocumentEntites)
                .Include(r => r.DocumentEntiteTypes)
                .Where(i => i.RefDocument == id)
                .FirstOrDefault();

            // handle requests asking for non-existing documentzes
            if (document == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            // remove the entity from the DbContext if applicable
            string del = document.IsDeletable();
            if (del == "")
            {
                DbContext.Documents.Remove(document);
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

        #endregion Attribute-based Routing
    }
}