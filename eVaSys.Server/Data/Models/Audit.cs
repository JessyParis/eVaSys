/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 02/07/2019
/// ----------------------------------------------------------------------------------------------------- 
namespace eVaSys.Data
{
    public class Audit
    {
        #region Constructor
        public Audit()
        {
        }
        #endregion
        protected ApplicationDbContext DbContext { get; private set; }
        public int RefAudit { get; set; }
        public DateTime D { get; set; }
        public int RefUtilisateur { get; set; }
        public string NomTable { get; set; }
        public string Refs { get; set; }
        public string AncienneValeur { get; set; }
        public string NouvelleValeur { get; set; }
    }
}
