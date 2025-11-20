/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/11/2025
/// ----------------------------------------------------------------------------------------------------- 
using AutoMapper;
using eVaSys.APIUtils;
using eVaSys.Data;
using eVaSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVaSys.Controllers
{
    [Route("evapi/[controller]")]
    public class DocumentTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        #region Constructor
        public DocumentTypeController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
            : base(context, configuration)
        {
            _mapper = mapper;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: evapi/documenttype/{id}
        /// Retrieves the DocumentType with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing DocumentType</param>
        /// <returns>the DocumentType with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var documentType = DbContext.DocumentTypes
                    .Include(r => r.UtilisateurCreation)
                    .Include(r => r.UtilisateurModif)
                    .Where(i => i.RefDocumentType == id).FirstOrDefault();
            // handle requests asking for non-existing object
            if (documentType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            //End
            return new JsonResult(
                _mapper.Map<DocumentType, DocumentTypeViewModel>(documentType),
                JsonSettings);
        }

        /// <summary>
        /// Edit the DocumentType with the given {id}
        /// </summary>
        /// <param name="model">The DocumentTypeViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] DocumentTypeViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return BadRequest(new BadRequestError(CurrentContext.CulturedRessources.GetTextRessource(711)));

            DocumentType documentType;
            //Retrieve or create the entiey to edit
            if (model.RefDocumentType > 0)
            {
                documentType = DbContext.DocumentTypes
                        .Where(q => q.RefDocumentType == model.RefDocumentType)
                        .FirstOrDefault();
            }
            else
            {
                documentType = new DocumentType();
                DbContext.DocumentTypes.Add(documentType);
            }
            // handle requests asking for non-existing documentTypezes
            if (documentType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }

            //Set values
            documentType.Libelle = Utils.Utils.SetEmptyStringToNull(model.Libelle);

            //Register session user
            documentType.RefUtilisateurCourant = CurrentContext.RefUtilisateur;

            //Check validation
            string valid = documentType.IsValid();
            //End
            if (valid == "")
            {
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated DocumentType to the client.
                return new JsonResult(
                    _mapper.Map<DocumentType, DocumentTypeViewModel>(documentType),
                    JsonSettings);
            }
            else
            {
                return Conflict(new ConflictError(valid));
            }
        }

        /// <summary>
        /// Deletes the DocumentType with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the documentType from the Database
            var documentType = DbContext.DocumentTypes.Where(i => i.RefDocumentType == id)
                .FirstOrDefault();

            // handle requests asking for non-existing documentTypezes
            if (documentType == null)
            {
                return NotFound(new NotFoundError(CurrentContext.CulturedRessources.GetTextRessource(460)));
            }
            // remove the entity from the DbContext if applicable
            string del = documentType.IsDeletable();
            if (del == "")
            {
                // remove the documentType from the DbContext.
                DbContext.DocumentTypes.Remove(documentType);
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
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/documenttype/GetList
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of items.</returns>
        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            //Query
            System.Linq.IQueryable<eVaSys.Data.DocumentType> req = DbContext.DocumentTypes;
            //Get data
            var all = req.OrderBy(el => el.Libelle).ToArray();
            //Return Json
            return new JsonResult(
                _mapper.Map<DocumentType[], DocumentTypeViewModel[]>(all),
                JsonSettings);
        }
        #endregion
    }
}
