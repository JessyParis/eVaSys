/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 30/07/2019
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CommandeClientMensuelleViewModel
    {
        public CommandeClientMensuelleViewModel()
        {
        }
        public int RefCommandeClientMensuelle { get; set; }
        public int RefCommandeClient { get; set; }
        public DateTime D { get; set; }
        public int Poids { get; set; }
        public decimal PrixTonneHT { get; set; }
        public string IdExt { get; set; }
        public int? RefExt { get; set; }
        public int RefProduit { get; set; }
        public string LibelleProduit { get; set; }
    }
}