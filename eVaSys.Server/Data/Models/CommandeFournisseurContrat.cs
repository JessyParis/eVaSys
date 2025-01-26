/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 26/01/2025
/// ----------------------------------------------------------------------------------------------------- 
using eVaSys.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for CommandeFournisseurContrat
    /// </summary>
    public class CommandeFournisseurContrat
    {
        #region Constructor
        public CommandeFournisseurContrat()
        {
        }
        #endregion
        public int RefCommandeFournisseurContrat { get; set; }
        public int RefCommandeFournisseur { get; set; }
        public int RefContrat { get; set; }
        public Contrat Contrat { get; set; }
        public ICollection<CommandeFournisseur> CommandeFournisseurs { get; set; }
    }
}