/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System.Net;

namespace eVaSys.APIUtils
{
    /// <summary>
    /// Standard API error class
    /// </summary>
    public class ApiError
    {
        //Constructors
        public ApiError(int statusCode, string statusDescription)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;
        }
        public ApiError(int statusCode, string statusDescription, string message)
            : this(statusCode, statusDescription)
        {
            this.Message = message;
        }
        public ApiError(int statusCode, string statusDescription, string message, string cultureName)
            : this(statusCode, statusDescription, message)
        {
            this.CultureName = cultureName;
        }
        //Properties
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; private set; }
        public string CultureName { get; private set; }
    }
    /// <summary>
    /// API error class for not found
    /// </summary>
    public class BadRequestError : ApiError
    {
        public BadRequestError()
            : base(400, HttpStatusCode.BadRequest.ToString())
        {
        }
        public BadRequestError(string message)
            : base(400, HttpStatusCode.BadRequest.ToString(), message)
        {
        }
    }
    /// <summary>
    /// API error class for not found
    /// </summary>
    public class ConflictError : ApiError
    {
        public ConflictError()
            : base(409, HttpStatusCode.Conflict.ToString())
        {
        }
        public ConflictError(string message)
            : base(409, HttpStatusCode.Conflict.ToString(), message)
        {
        }
        public ConflictError(string message, string cultureName)
            : base(409, HttpStatusCode.Conflict.ToString(), message, cultureName)
        {
        }
    }
    /// <summary>
    /// API error class for internal server error
    /// </summary>
    public class InternalServerError : ApiError
    {
        public InternalServerError()
            : base(500, HttpStatusCode.InternalServerError.ToString())
        {
        }
        public InternalServerError(string message)
            : base(500, HttpStatusCode.InternalServerError.ToString(), message)
        {
        }
    }
    /// <summary>
    /// API error class for not found
    /// </summary>
    public class NotFoundError : ApiError
    {
        public NotFoundError()
            : base(404, HttpStatusCode.NotFound.ToString())
        {
        }
        public NotFoundError(string message)
            : base(404, HttpStatusCode.NotFound.ToString(), message)
        {
        }
    }
    /// <summary>
    /// API error class for not unauthorized
    /// </summary>
    public class UnauthorizedError : ApiError
    {
        public UnauthorizedError()
            : base(401, HttpStatusCode.Unauthorized.ToString())
        {
        }
        public UnauthorizedError(string message)
            : base(401, HttpStatusCode.Unauthorized.ToString(), message)
        {
        }
    }
    /// <summary>
    /// API error class for not forbidden
    /// </summary>
    public class ForbiddenError : ApiError
    {
        public ForbiddenError()
            : base(403, HttpStatusCode.Forbidden.ToString())
        {
        }
        public ForbiddenError(string message)
            : base(403, HttpStatusCode.Forbidden.ToString(), message)
        {
        }
    }
}
