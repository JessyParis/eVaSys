
using System.Collections.Generic;
/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 02/05/2019
/// ----------------------------------------------------------------------------------------------------- 
namespace eVaSys.Data
{
    /// <summary>
    /// Class for ContactAdresseProcess
    /// </summary>
    public class ContactAdresseProcess
    {
        #region Constructor
        public ContactAdresseProcess()
        {
        }
        #endregion
        public int RefContactAdresseProcess { get; set; }
        public string Libelle { get; set; }
        public ICollection<ContactAdresseContactAdresseProcess> ContactAdresseContactAdresseProcesss { get; set; }
    }
}