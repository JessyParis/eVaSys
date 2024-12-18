/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 13/12/2024
/// ----------------------------------------------------------------------------------------------------- 
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for ContratType
    /// </summary>
    public class ContratType
    {
        #region Constructor
        public ContratType()
        {
        }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public CultureInfo currentCulture = new("fr-FR");
        public int RefContratType { get; set; }
        public string LibelleFRFR { get; set; }
        public string LibelleENGB { get; set; }
        [NotMapped]
        public string Libelle
        {
            get { return (currentCulture.Name == "fr-FR" ? LibelleFRFR : LibelleENGB); }
            set { }
        }
        public ICollection<Contrat> Contrats { get; set; }
    }
}