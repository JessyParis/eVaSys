/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 01/07/2022
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;

namespace eVaSys.APIUtils
{
    /// <summary>
    /// Standard API utils class
    /// </summary>
    public class ApiUtils
    {
        //Constructor
        private ApiUtils()
        {
        }
        /// <summary>
        //Log the API call in the database
        /// </summary>
        public static bool LogAPICall(string path, string jsonBody, int? statusCode, string response, string erreur
            , int refUtilisateur, ApplicationDbContext dbContext)
        {
            try
            {
                var aPILog = new APILog()
                {
                    RefUtilisateur = refUtilisateur,
                    URL = Utils.Utils.SetEmptyStringToNull(path),
                    Payload = Utils.Utils.SetEmptyStringToNull(jsonBody),
                    StatusCode=statusCode,
                    Response= Utils.Utils.SetEmptyStringToNull(response),
                    Erreur= Utils.Utils.SetEmptyStringToNull(erreur),
                };
                dbContext.APILogs.Add(aPILog);
                dbContext.SaveChanges();
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
