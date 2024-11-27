/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/12/2021
/// ----------------------------------------------------------------------------------------------------- 

namespace eVaSys.Data
{
    /// <summary>
    /// Class for ActionFichierNoFile
    /// </summary>
    public class ActionFichierNoFile
    {
        #region Constructor
        public ActionFichierNoFile()
        {
        }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefActionFichier { get; set; }
        public int RefAction { get; set; }
        public string Nom { get; set; }
        public string Extension { get; set; }
    }
}