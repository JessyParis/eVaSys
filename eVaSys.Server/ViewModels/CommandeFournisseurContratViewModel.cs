/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 26/01/2025
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Data;
using Newtonsoft.Json;
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CommandeFournisseurContratViewModel
    {
        #region Constructor
        public CommandeFournisseurContratViewModel()
        {

        }
        #endregion Constructor

        #region Properties
        public int RefCommandeFournisseurContrat { get; set; }
        public int RefCommandeFournisseur { get; set; }
        public int RefContrat { get; set; }
        public ContratViewModel Contrat { get; set; }
        #endregion Properties
    }
}
