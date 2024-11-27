/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/07/2018
/// ----------------------------------------------------------------------------------------------------- 

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace eVaSys.Utils
{
    /// <summary>
    /// Session management for complex objects
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Serialize complex objects
        /// </summary>
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
        /// <summary>
        /// Deserialize complex objects
        /// </summary>
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}

