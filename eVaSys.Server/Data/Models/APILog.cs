/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 23/06/2022
/// ----------------------------------------------------------------------------------------------------- 
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for APILog
    /// </summary>
    public class APILog
    {
        #region Constructor
        public APILog()
        {
        }
        private APILog(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefAPILog { get; set; }
        public DateTime D { get; set; }
        public int RefUtilisateur { get; set; }
        public Utilisateur Utilisateur { get; set; }
        public string URL { get; set; }
        public string Payload { get; set; }
        public int? StatusCode { get; set; }
        public string Response { get; set; }
        public string Erreur { get; set; }
    }
}