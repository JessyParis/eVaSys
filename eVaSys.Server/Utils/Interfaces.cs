/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 01/01/2019
/// ----------------------------------------------------------------------------------------------------- 

using System;

namespace eVaSys.Data
{
    public interface IMarkModification
    {
        DateTime DCreation { get; set; }
        DateTime? DModif { get; set; }
        int RefUtilisateurCourant { get; set; }
        int RefUtilisateurCreation { get; set; }
        int? RefUtilisateurModif { get; set; }
        void MarkModification(bool add);
    }
}
