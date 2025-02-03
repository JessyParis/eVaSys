/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 03/01/2025
/// ----------------------------------------------------------------------------------------------------- 
using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace eVaSys.Data
{
    public class UtilisateurAPI
    {
        #region Constructor
        public UtilisateurAPI()
        {
        }
        #endregion
        public int RefUtilisateurAPI { get; set; }
        public int RefUtilisateur { get; set; }
        public string API { get; set; }
        public bool R { get; set; } = true;
        public bool W { get; set; }
    }
}