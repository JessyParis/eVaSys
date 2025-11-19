/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/09/2023
/// ----------------------------------------------------------------------------------------------------- 

using System.ComponentModel.DataAnnotations.Schema;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for SAGEDocumentLigne
    /// </summary>
    public class SAGEDocumentLigne
    {
        #region Constructor
        public SAGEDocumentLigne()
        {
        }
        #endregion
        public int ID { get; }
        public string RefSAGEDocument { get; }
        public double? QuantiteBdd { get; }
        public string LibelleDL { get; }
        [NotMapped]
        public decimal? Quantite
        {
            get
            {
                //if (QuantiteBdd != null) { return (decimal)Math.Abs((double)QuantiteBdd); }
                if (QuantiteBdd != null) { return (decimal)QuantiteBdd; }
                else { return null; }
            }
        }
        public double? PUHTBdd { get; }
        [NotMapped]
        public decimal? PUHT
        {
            get
            {
                //if (PUHTBdd != null) { return (decimal)Math.Abs((double)PUHTBdd); }
                if (PUHTBdd != null) { return (decimal)PUHTBdd; }
                else { return null; }
            }
        }
        public double? TotalHTBdd { get; }
        [NotMapped]
        public decimal? TotalHT
        {
            get
            {
                //if (TotalHTBdd != null) { return (decimal)Math.Abs((double)TotalHTBdd); }
                if (TotalHTBdd != null) { return (decimal)TotalHTBdd; }
                else { return null; }
            }
        }
        public double? TotalTTCBdd { get; }
        [NotMapped]
        public decimal? TotalTTC
        {
            get
            {
                //if (TotalTTCBdd != null) { return (decimal)Math.Abs((double)TotalTTCBdd); }
                if (TotalTTCBdd != null) { return (decimal)TotalTTCBdd; }
                else { return null; }
            }
        }
        public double? TVAMontantBdd { get; }
        [NotMapped]
        public decimal? TVAMontant
        {
            get
            {
                return (decimal?)TVAMontantBdd;
            }
        }
        public double? TVATauxBdd { get; }
        [NotMapped]
        public decimal? TVATaux
        {
            get
            {
                return (decimal?)TVATauxBdd;
            }
        }
        [NotMapped]
        public int? TVACode
        {
            get
            {
                return (TVATaux == 0 ? 8 : 1);
            }
        }
        public Int16 DocumentType { get; }
        public string RefSAGEDocumentArticle { get; }
        public SAGEDocumentArticle SAGEDocumentArticle { get; }
        public int RefLigne { get; }
        public SAGEDocumentLigneText SAGEDocumentLigneText { get; }
    }
}