/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 28/09/2023
/// ----------------------------------------------------------------------------------------------------- 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;

namespace eVaSys.Data
{
    /// <summary>
    /// Class for SAGEDocument
    /// </summary>
    public class SAGEDocument
    {
        #region Constructor
        public SAGEDocument()
        {
        }
        #endregion
        public int ID { get; }
        public string RefSAGEDocument { get; }
        public DateTime D { get; }
        public string? CodeComptable { get; }
        public string ConditionLivraison { get; }
        public double? TotalHTBdd { get; }
        [NotMapped]
        public decimal? TotalHT
        {
            get
            {
                return (decimal?)TotalHTBdd;
            }
        }
        public double? TotalTTCBdd { get; }
        [NotMapped]
        public decimal? TotalTTC
        {
            get
            {
                return (decimal?)TotalTTCBdd;
            }
        }
        public double? TotalNetBdd { get; }
        [NotMapped]
        public decimal? TotalNet
        {
            get
            {
                return (decimal?)TotalNetBdd;
            }
        }
        public double? TotalTVABdd { get; }
        [NotMapped]
        public decimal? TotalTVA
        {
            get
            {
                return (decimal?)TotalTVABdd;
            }
        }
        public string Adr1 { get; }
        public string Adr2 { get; }
        public string Adr3 { get; }
        public string Adr4 { get; }
        public Int16 DocumentType { get; }
        public SAGEDocumentCompteTVA SAGEDocumentCompteTVA { get; }
        public ICollection<SAGEDocumentLigne> SAGEDocumentLignes { get; }
        public ICollection<SAGEDocumentReglement> SAGEDocumentReglements { get; }
        [NotMapped]
        public string Type
        {
            get
            {
                string s = "NA";
                if (!string.IsNullOrWhiteSpace(RefSAGEDocument))
                {
                    if (TotalHTBdd < 0)
                    {
                        s = "AVOIR";
                    }
                    else
                    {
                        s = "NOTE DE CREDIT";
                    }
                }
                return s;
            }
        }
        [NotMapped]
        public bool Regle
        {
            get
            {
                //return SAGEDocumentReglements.Count > 0;
                return true;
            }
        }
        [NotMapped]
        //Totaux
        public DataTable Totaux { get; set; } = new DataTable();
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Calcul des totaux
        public string CalcTotaux()
        {
            decimal ht = 0, ttc = 0, taxe = 0;
            decimal taux;
            object[] rowVals = new object[5];
            //Initialisation de la table
            Totaux.Rows.Clear();
            Totaux.Columns.Clear();
            Totaux.Columns.Add("Libelle", typeof(string));
            Totaux.Columns.Add("Montant", typeof(decimal));
            Totaux.Columns.Add("BaseHT", typeof(decimal));
            Totaux.Columns.Add("TVATaux", typeof(decimal));
            Totaux.Columns.Add("TVACode", typeof(int));
            //Calcul des totaux HT et TTC
            foreach (var sDL in SAGEDocumentLignes)
            {
                ht += sDL.TotalHT??0;
                ttc += sDL.TotalHT ?? 0;
            }
            rowVals[0] = "Total HT";
            rowVals[1] = ht;
            Totaux.Rows.Add(rowVals);
            rowVals[0] = "Total TTC";
            rowVals[1] = ttc;
            Totaux.Rows.Add(rowVals);
            //Inscription des différents totaux HT pour chaque taux de TVA
            foreach (var sDL in SAGEDocumentLignes)
            {
                DataRow[] fr;
                ht = 0;
                taxe = 0;
                //si le taux de TVA de la ligne n'est pas déjà enregistré dans les totaux, alors on calcul le total pour ce taux et on ajoute la ligne au tableau des totaux
                fr = Totaux.Select("TVATaux='" + sDL.TVATaux.ToString() + "'");
                if (fr.Length == 0)
                {
                    taux = sDL.TVATaux ?? 0;
                    //Taux de TVA inexistant dans les totaux, on recherche dans les lignes les TVA correspondantes
                    var frl = SAGEDocumentLignes.Where(w => w.TVATaux == sDL.TVATaux).ToArray();
                    if (frl.Length > 0)
                    {
                        for (int i = 0; i < frl.Length; i++)
                        {
                            //Calcul du total de la taxe pour le taux de TVA en question
                            ht += frl[i].TotalHT??0;
                            taxe += frl[i].TVAMontant ?? 0;
                        }
                        //Ajout aux totaux avant le taux de TVA supérieur, recherhe de la bonne place
                        int place = 0;
                        for (int i = 1; i < Totaux.Rows.Count; i++)
                        {
                            if ((Totaux.Rows[i]["TVATaux"].ToString() == "" ? 0 : (decimal)Totaux.Rows[i]["TVATaux"]) > taux)
                            {
                                place = i;
                            }
                        }
                        if (place == 0) { place = Totaux.Rows.Count; }
                        //Ajout du total
                        DataRow dr = Totaux.NewRow();
                        dr["Libelle"] = "TVA " + taux.ToString();
                        dr["BaseHT"] = ht;
                        dr["Montant"] = taxe;
                        dr["TVATaux"] = taux;
                        dr["TVACode"] = sDL.TVACode;
                        Totaux.Rows.InsertAt(dr, place);
                    }
                }
            }
            //Valdation
            Totaux.AcceptChanges();
            return "ok";
        }
    }
}