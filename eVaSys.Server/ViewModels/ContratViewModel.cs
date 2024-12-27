/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 18/12/2024
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Newtonsoft.Json;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ContratViewModel
    {
        #region Constructor
        public ContratViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefContrat { get; set; }
        public string IdContrat { get; set; }
        public ContratTypeViewModel ContratType { get; set; }
        public int? RefEntite { get; set; }
        public Entite Entite { get; set; }
        public DateOnly? DDebut { get; set; }
        public DateOnly? DFin { get; set; }
        public bool ReconductionTacite { get; set; }
        public bool Avenant { get; set; }
        public string Cmt { get; set; }
        public string CorpsBase64 { get; set; }
        #endregion Properties
    }
}
