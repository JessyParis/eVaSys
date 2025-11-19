/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/12/2019
/// ----------------------------------------------------------------------------------------------------- 
namespace eVaSys.Data
{
    /// <summary>
    /// Class for EmailFichier
    /// </summary>
    public class EmailFichier
    {
        #region Constructor
        public EmailFichier()
        {
        }
        #endregion
        public int Index { get; set; }
        public string Nom { get; set; }
        public byte[] Corps { get; set; }
        public string CorpsBase64
        {
            get
            {
                return Convert.ToBase64String(Corps);
            }
        }
        public string VisualType { get; set; }
    }
}