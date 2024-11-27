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
    public class TransportViewModel
    {
        #region Constructor
        public TransportViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefTransport { get; set; }
        public int RefParcours { get; set; }
        public ParcoursViewModel Parcours { get; set; }
        public int RefTransporteur { get; set; }
        public EntiteViewModel Transporteur { get; set; }
        public int RefCamionType { get; set; }
        public CamionTypeViewModel CamionType { get; set; }
        public decimal? PUHT { get; set; }
        public decimal? PUHTDemande { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
