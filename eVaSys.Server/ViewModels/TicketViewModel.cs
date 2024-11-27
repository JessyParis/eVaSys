/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 02/12/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class TicketViewModel
    {
        #region Constructor
        public TicketViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefTicket { get; set; }
        public PaysViewModel Pays { get; set; }
        public string Libelle { get; set; }
        public string TicketTexte { get; set; }
        public string TicketTexteHTMLFromText { get; set; }
        public string CreationText { get; set;}
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
