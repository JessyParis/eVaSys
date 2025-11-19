/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 07/12/2018
/// ----------------------------------------------------------------------------------------------------- 

using System.Text;

namespace eVaSys.Data
{
    public class Dpt
    {
        #region Constructor
        public Dpt()
        {
        }
        #endregion
        public static string TextFilter(string filter, ApplicationDbContext dbContext)
        {
            StringBuilder r = new();
            if (filter != "")
            {
                foreach (string f in filter.Split(","))
                {
                    var res = dbContext.Dpts.Where(i => i.RefDpt == f).FirstOrDefault();
                    r.Append(res.Libelle + " (" + res.RefDpt + "), ");
                }
            }
            if (r.Length != 0) { r = r.Remove(r.Length - 2, 2); }
            return r.ToString();
        }
        public string RefDpt { get; set; }
        public string Libelle { get; set; }
        public ICollection<RegionEEDpt> RegionEEDpts { get; set; }
    }
}