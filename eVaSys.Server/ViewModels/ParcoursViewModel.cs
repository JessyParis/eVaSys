/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/06/2018
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ParcoursViewModel
    {
        #region Constructor
        public ParcoursViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefParcours { get; set; }
        public int RefAdresseOrigine { get; set; }
        public AdresseViewModel AdresseOrigine { get; set; }
        public int RefAdresseDestination { get; set; }
        public AdresseViewModel AdresseDestination { get; set; }
        public int Km { get; set; }
        #endregion Properties
    }
}
