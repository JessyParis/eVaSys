/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 18/09/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CommandeClientMensuelleFormViewModel
    {
        public CommandeClientMensuelleFormViewModel()
        {
        }
        public int? RefCommandeClient { get; set; }
        public int? RefCommandeClientMensuelle { get; set; }
        public int? RefContrat { get; set; }
        public int RefEntite { get; set; }
        public int RefAdresse { get; set; }
        public int RefProduit { get; set; }
        public string LibelleProduit { get; set; }
        public string Cmt { get; set; }
        public DateTime D { get; set; }
        public int? Poids { get; set; }
        public decimal? PrixTonneHT { get; set; }
        public string IdExt { get; set; }
        public string CertificationText { get; set; }
        public bool? Certif { get; set; } = null;
    }
}