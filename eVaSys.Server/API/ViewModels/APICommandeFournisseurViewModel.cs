/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 23/06/2022
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class APICommandeFournisseurViewModel
    {
        public APICommandeFournisseurViewModel()
        {
        }
        public string RefExt { get; set; }
        public string LibExt { get; set; }
        public string FournisseurCodeCITEO { get; set; }
        public string FournisseurLibelle { get; set; }
        public string ProduitLibelle { get; set; }
        public string Adr1 { get; set; }
        public string Adr2 { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public string Civilite { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public DateTime? DDisponibilite { get; set; }
        public int PoidsChargement { get; set; }
        public int NbBalleChargement { get; set; }
        public string Cmt { get; set; }
        public string ActionCode { get; set; }
        public string carrierRejectedLastComment { get; set; }
        public string receiptRejectedLastComment { get; set; }
        public int? RefPrestataire { get; set; }
    }
}