/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/05/2019
/// ----------------------------------------------------------------------------------------------------- 
namespace eVaSys.Data
{
    /// <summary>
    /// Class for CommandeFournisseurStatut
    /// </summary>
    public class CommandeFournisseurStatut
    {
        #region Constructor
        public CommandeFournisseurStatut()
        {
        }
        #endregion
        public int RefCommandeFournisseurStatut { get; private set; }
        public string Libelle { get; private set; }
        public string Couleur { get; private set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurs { get; set; }
    }
}