/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 06/12/2021
/// ----------------------------------------------------------------------------------------------------- 
using System;
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ContratIncitationQualiteViewModel
    {
        public ContratIncitationQualiteViewModel()
        {

        }
        public int RefContratIncitationQualite { get; set; }
        public DateTime? DDebut { get; set; }
        public DateTime? DFin { get; set; }
        public int RefEntite { get; set; }
    }
}
