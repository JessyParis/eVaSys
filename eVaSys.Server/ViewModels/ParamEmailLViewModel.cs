/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/11/2022
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ParamEmailViewModel
    {
        #region Constructor
        public ParamEmailViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefParamEmail { get; set; }
        public bool? Actif { get; set; }
        public bool? Defaut { get; set; }
        public bool? RttClient { get; set; }
        public string POP { get; set; }
        public int? PortPOP { get; set; }
        public string ComptePOP { get; set; }
        public string PwdPOP { get; set; }
        public bool? TLSPOP { get; set; }
        public string EnTeteEmail { get; set; }
        public string PiedEmail { get; set; }
        public string EmailExpediteur { get; set; }
        public string LibelleExpediteur { get; set; }
        public string SMTP { get; set; }
        public int? PortSMTP { get; set; }
        public string CompteSMTP { get; set; }
        public string PwdSMTP { get; set; }
        public bool? AllowAuthSMTP { get; set; }
        public bool? TLSSMTP { get; set; }
        public bool? StartTLSSMTP { get; set; }
        public bool? AR { get; set; }
        public string AREmail { get; set; }
        public string AREmailSujet { get; set; }
        public string ARExpediteurEmail { get; set; }
        public string ARExpediteurLibelle { get; set; }
        public string CreationText { get; set; }
        public string ModificationText { get; set; }
        #endregion Properties
    }
}
