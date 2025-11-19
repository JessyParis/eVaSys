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
    public class EmailViewModel
    {
        #region Constructor
        public EmailViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefEmail { get; set; }
        public ParamEmailListViewModel ParamEmail { get; set; }
        public string POPId { get; set; }
        public bool? Entrant { get; set; }
        public ActionViewModel Action { get; set; }
        public string Libelle { get; set; }
        public DateTime? DEnvoi { get; set; }
        public DateTime? DReception { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTextBody { get; set; }
        public string EmailHTMLBody { get; set; }
        public ICollection<EmailFichierViewModel> EmailFichiers { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
