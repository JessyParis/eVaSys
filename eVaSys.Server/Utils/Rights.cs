/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/07/2018
/// ----------------------------------------------------------------------------------------------------- 

using System.Collections.Generic;
using System.Data;
using System.Linq;
using eVaSys.Data;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Telerik.Windows.Documents.Fixed.Model.Annotations;

namespace eVaSys.Utils
{
    /// <summary>
    /// Rights mamagement
    /// </summary>
    public class Rights
    {
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Datatable to store objects access rights definitions
        /// Module | Habilitation | Object | Right(s) (CRUD)
        /// </summary>
        private static DataTable rights = new();
        private static DataTable menuRights = new();
        private static DataTable actionRights = new();
        static Rights()
        {
            #region ObjectRights
            rights.Columns.Add("Module", typeof(string));
            rights.Columns.Add("Habilitation", typeof(string));
            rights.Columns.Add("Object", typeof(string));
            rights.Columns.Add("Rights", typeof(string));
            //Loading initial values
            object[] rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Action.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.ActionType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ActionType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Aide.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Aide.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Adresse.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Application.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Application.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.ApplicationProduitOrigine.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Civilite.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Civilite.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Couleur.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Couleur.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.ContratType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Document.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Ressource.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Ressource.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.FormeContact.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.FormeContact.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.EntiteType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.EntiteType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Email.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Equipementier.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.EquivalentCO2.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.EquivalentCO2.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Equipementier.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.FournisseurTO.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.FournisseurTO.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.JourFerie.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.JourFerie.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.ModeTransportEE.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ModeTransportEE.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Ticket.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.MessageType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.MessageType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.MotifAnomalieClient.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.MotifAnomalieClient.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.MotifAnomalieChargement.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.MotifAnomalieChargement.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.MotifAnomalieTransporteur.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.MotifAnomalieTransporteur.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.ParamEmail.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ParamEmail.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Repreneur.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Repreneur.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.EcoOrganisme.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.EcoOrganisme.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.RepriseType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.RepriseType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Service.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Service.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Fonction.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Fonction.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Parametre.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Parametre.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.RegionEE.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.RegionEE.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.RegionReporting.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.RegionReporting.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.SAGECodeTransport.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.SAGECodeTransport.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Repreneur.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.EcoOrganisme.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Ticket.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ClientApplication.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ApplicationProduitOrigine.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.DescriptionControle.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.DescriptionCVQ.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.DescriptionReception.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.MontantIncitationQualite.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Securite.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Adresse.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Adresse.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.AdresseType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ClientApplication.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.CommandeClient.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.CommandeClient.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.CommandeFournisseur.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.CommandeFournisseur.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            rowVals[2] = Enumerations.ObjectName.CommandeFournisseur.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.CommandeFournisseur.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.CentreDeTri.ToString();
            rowVals[2] = Enumerations.ObjectName.CommandeFournisseur.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Repartition.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Repartition.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.DocumentEdit.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.DocumentEdit.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ExportSAGE.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ExportSAGE.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.CommandeFournisseurStatut.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.ContactAdresse.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ContactAdresse.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ContactAdresse.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Search.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Dashboard.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Dpt.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Download.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Entite.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Annuaire.ToString(); rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Entite.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Annuaire.ToString(); rowVals[1] = Enumerations.HabilitationAnnuaire.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Entite.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Grid.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.GridSimple.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.MotifCamionIncomplet.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.MotifCamionIncomplet.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.CamionType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.CamionType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.ContactAdresseProcess.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ContactAdresseProcess.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            rowVals[2] = Enumerations.ObjectName.Statistique.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Statistique.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Statistique.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Transport.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            rowVals[2] = Enumerations.ObjectName.Transport.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Transport.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Pays.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Pays.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Message.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Message.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.PrixReprise.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.PrixReprise.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Produit.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Produit.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ProduitGroupeReporting.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.ProduitGroupeReporting.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ProduitGroupeReportingType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.ProduitGroupeReportingType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Process.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Process.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Standard.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Standard.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Upload.ToString(); rowVals[3] = "U";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Titre.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Titre.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Administration.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Utilisateur.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Utilisateur.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.SurcoutCarburant.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Logistique.ToString(); rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.SurcoutCarburant.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = ""; rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.Utils.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Messagerie.ToString(); rowVals[1] = Enumerations.HabilitationMessagerie.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Message.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Messagerie.ToString(); rowVals[1] = Enumerations.HabilitationMessagerie.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Message.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.FicheControle.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.FicheControle.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.FicheControle.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.FicheControleDescriptionReception.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.FicheControleDescriptionReception.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.FicheControleDescriptionReception.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Controle.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.Controle.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Controle.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ControleDescriptionControle.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.ControleDescriptionControle.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.ControleDescriptionControle.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.CVQ.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.CVQ.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.CVQ.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.CVQDescriptionCVQ.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.CVQDescriptionCVQ.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.CVQDescriptionCVQ.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformite.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformite.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformite.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteAccordFournisseurType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteAccordFournisseurType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteAccordFournisseurType.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteDemandeClientType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteDemandeClientType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteDemandeClientType.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteFamille.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = "";
            rowVals[2] = Enumerations.ObjectName.NonConformiteEtapeType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteFamille.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteFamille.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteNature.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteNature.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteNature.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteReponseClientType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteReponseClientType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteReponseClientType.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteReponseFournisseurType.ToString(); rowVals[3] = "CRUD";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteReponseFournisseurType.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.NonConformiteReponseFournisseurType.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            rowVals[2] = Enumerations.ObjectName.Reclamation.ToString(); rowVals[3] = "CRU";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.Qualite.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            rowVals[2] = Enumerations.ObjectName.Reclamation.ToString(); rowVals[3] = "R";
            rights.Rows.Add(rowVals);
            rowVals = new object[4];
            rowVals[0] = Enumerations.Module.ModulePrestataire.ToString(); rowVals[1] = Enumerations.HabilitationQualite.Fournisseur.ToString();
            rowVals[2] = Enumerations.ObjectName.APICommandeFournisseur.ToString(); rowVals[3] = "U";
            rights.Rows.Add(rowVals);
            #endregion
            #region MenuRights
            //---------------------------------------------------------------------------------------
            //Menus
            menuRights.Columns.Add("MenuName", typeof(string));
            menuRights.Columns.Add("Habilitation", typeof(string));
            // Create rights directly associated with the menu

            //Loading initial values
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuActionType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuAdresseType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuAide.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuApplication.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuApplicationProduitOrigine.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuCamionType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuCivilite.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals[0] = Enumerations.MenuName.AdministrationMenuEquipementier.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals[0] = Enumerations.MenuName.AdministrationMenuEquivalentCO2.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuFournisseurTO.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuDescriptionControle.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuDescriptionCVQ.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuDescriptionReception.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuClientApplication.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuDocument.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuExtractionLogin.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuEntiteType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuFonction.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuParametre.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuFormeContact.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuJourFerie.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuMessageType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuModeTransportEE.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuMontantIncitationQualite.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuMotifAnomalieChargement.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuMotifAnomalieClient.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuMotifAnomalieTransporteur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuMotifCamionIncomplet.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuNonConformiteDemandeClientType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuNonConformiteFamille.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuNonConformiteNature.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuNonConformiteReponseClientType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuNonConformiteReponseFournisseurType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuParamEmail.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuPays.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuProcess.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuProduit.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuProduitGroupeReportingType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuProduitGroupeReporting.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuRegionEE.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuRegionReporting.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuRepreneur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuEcoOrganisme.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuRepriseType.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuRessource.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuSAGECodeTransport.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuService.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuStandard.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuSuiviLogin.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuTicket.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuTitre.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuUtilisateur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuSecurite.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AdministrationMenuUtilisateurInactif.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuSuiviEnvois.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuEntite.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuEntite.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuExtractionAnnuaire.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuExtractionAction.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuExtractionAction.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuModifierTransport.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuModifierTransport.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportDemandeEnlevement.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportDemandeEnlevement.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportDemandeEnlevement.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportCommande.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportCommande.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportCommandeEnCours.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportCommandeEnCours.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportCommandeModifiee.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportCommandeModifiee.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuChargementAnnule.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuChargementAnnule.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuCommandeClient.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuCommandeClient.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuCommandeFournisseur.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.CentreDeTri.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuRepartition.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuRepartition.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuPrixReprise.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuPrixReprise.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuSurcoutCarburant.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportNonValide.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTransportNonValide.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuModifierTousPrixTransport.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuModifierTousPrixTransport.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuSupprimerTransportEnMasse.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuImporterTransport.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuImporterTransport.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuDelaiCommandeEnlevement.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.CentreDeTri.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Transporteur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatCoutTransport.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuLotDisponible.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuLotDisponible.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatTarifTransporteurClient.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuPartMarcheTransporteur.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatKmMoyen.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatKmMoyen.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReceptionFournisseurChargement.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReceptionFournisseurChargement.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReception.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReception.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuSuiviCommandeClient.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuSuiviCommandeClientProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuSuiviCommandeClientProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReceptionProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReceptionProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReceptionProduitDR.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReceptionProduitDR.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuExtractionReception.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuExtractionCommande.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDesPoids.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDesPoids.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatTonnageParProcess.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatTonnageParProcess.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatVenteAnnuelleProduitClient.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDestinationAnnuelleProduitClient.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuSuiviFacturationHC.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuSuiviFacturationHC.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTonnageCollectiviteProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTonnageCDTProduitComposant.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTonnageCLSousContrat.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuTonnageCLSousContrat.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuListeProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuListeProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReceptionEmballagePlastique.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatReceptionEmballagePlastique.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDesFluxDev.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDesFluxDev.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDesFluxDevLeko.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDesFluxDevLeko.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDesEnlevements.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDesEnlevements.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuEtatDesEnlevements.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.CentreDeTri.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuExtractionLeko.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuExtractionLeko.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.LogistiqueMenuPoidsMoyenChargementProduit.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.CentreDeTri.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.MessagerieMenuMessage.ToString();
            rowVals[1] = Enumerations.HabilitationMessagerie.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.MessagerieMenuVisualisation.ToString();
            rowVals[1] = Enumerations.HabilitationMessagerie.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModulePrestataireMenuCommandeFournisseur.ToString();
            rowVals[1] = Enumerations.HabilitationModulePrestataire.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModulePrestataireMenuCommandeFournisseur.ToString();
            rowVals[1] = Enumerations.HabilitationModulePrestataire.Fournisseur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuControle.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuControle.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuControle.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuNonConformite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuNonConformite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuNonConformite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuRepartitionControleClient.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionFicheControle.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionControle.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionControle.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionControle.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionNonConformite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuExtractionCVQ.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuEtatControleReception.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuEtatControleReception.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuEtatControleReception.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Client.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.QualiteMenuEtatIncitationQualite.ToString();
            rowVals[1] = Enumerations.HabilitationQualite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuEvolutionTonnage.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuNbAffretementTransporteur.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.AnnuaireMenuNbAffretementTransporteur.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuAccueil.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuAccueil.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuDocuments.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuDocuments.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuPrixReprise.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuPrixReprise.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuStatistiques.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuStatistiques.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuVisualisationAnnuaire.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.ModuleCollectiviteMenuVisualisationAnnuaire.ToString();
            rowVals[1] = Enumerations.HabilitationModuleCollectivite.Utilisateur.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.MenuDocuments.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Visualisation.ToString();
            menuRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.MenuName.MenuDocuments.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            menuRights.Rows.Add(rowVals);
            #endregion
            #region ActionRights
            //---------------------------------------------------------------------------------------
            //Actions
            actionRights.Columns.Add("ActionName", typeof(string));
            actionRights.Columns.Add("Habilitation", typeof(string));
            //Loading initial values
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.DeactivateUtilisateur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.IncitationQualite.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.EmailNoteCreditCollectivite.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.Mixte.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.ValidateTransportPrice.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.RejectTransportPrice.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.DeleteTransport.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.SetEnAttente.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.ChangeMoisDechargementPrevu.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.CopyPrixReprise.ToString();
            rowVals[1] = Enumerations.HabilitationLogistique.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.TransporteurForUtilisateur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.CentreDeTriForUtilisateur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.CollectiviteForUtilisateur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.ClientForUtilisateur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.PrestataireForUtilisateur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.EntiteForDocument.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.EntiteForDocument.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.EntiteForContrat.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.UtilisateurMaitreForUtilisateur.ToString();
            rowVals[1] = Enumerations.HabilitationAdministration.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            rowVals = new object[2];
            rowVals[0] = Enumerations.ActionName.ContactModificationRequest.ToString();
            rowVals[1] = Enumerations.HabilitationAnnuaire.Administrateur.ToString();
            actionRights.Rows.Add(rowVals);
            #endregion
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Chek if the access for a controller http method is granted or not
        /// <param name="controllerPath">The full path to controller</param>
        /// <param name="httpMethod">The http method invoked (GET, PUT, POST, DELETE)</param>
        /// <param name="configuration">The current IConfiguration</param>
        /// </summary>
        public static bool Authorized(string controllerPath, string httpMethod, Utilisateur u, IConfiguration configuration)
        {
            int c = 0;
            string filter = "", obj = "";
            //Initialize filters
            //User

            //Method
            switch (httpMethod)
            {
                case "PUT": filter = "Rights like '%C%'"; break;
                case "GET": filter = "Rights like '%R%'"; break;
                case "POST": filter = "Rights like '%U%'"; break;
                case "DELETE": filter = "Rights like '%D%'"; break;
            }
            //Object
            if (filter != "")
            {
                obj = controllerPath.Substring(7, controllerPath.Length - 7);
                int pos = obj.IndexOf('/');
                if (pos != -1) { obj = obj.Substring(0, pos); }
                filter += " and Object='" + obj + "'";
                //Filtering if filter is correct
                if (filter != "")
                {
                    c = rights.Select(filter).Count();
                }
                if (c == 0) { Utils.DebugPrint("Unauthorized controller - " + controllerPath + " - " + httpMethod, configuration["Data:DefaultConnection:ConnectionString"]); }
            }
            //End
            return (c != 0);
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Defines the habilitation required to access a menu
        /// <param name="menuName">The name of the menu</param>
        /// <param name="habilitation">A list of allowed habilitations for that menu</param>
        /// </summary>
        public static void AddMenuHabilitation(EnvMenu menu, List<string> habilitations)
        {
            lock (menuRights)
            {
                // Get the existing records for the given menuName
                string filter = "MenuName='" + menu.Name + "'";
                // List the items to add (unknown only)
                List<string> toAdd = habilitations.Except(menuRights.Select(filter).Select(x => x["Habilitation"].ToString())).ToList();
                // Add the records
                foreach (string hab in toAdd)
                {
                    DataRow r = menuRights.Rows.Add();
                    r["MenuName"] = menu.Name;
                    r["Habilitation"] = hab;
                }
            }
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Chek if the access to a menu is granted or not
        /// <param name="menuName">The name of the menu</param>
        /// <param name="habilitation">The connected user habilitation for that module</param>
        /// <param name="configuration">The current IConfiguration</param>
        /// </summary>
        public static bool AuthorizedMenu(string menuName, Context currentContext, IConfiguration configuration)
        {
            bool r = false;
            if (!string.IsNullOrEmpty(menuName))
            {
                string moduleName = currentContext.EnvMenus[menuName].Module.Name;
                string habilitation = "undefined";
                if (moduleName == Enumerations.Module.Administration.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationAdministration; }
                if (moduleName == Enumerations.Module.Logistique.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationLogistique; }
                if (moduleName == Enumerations.Module.Qualite.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationQualite; }
                if (moduleName == Enumerations.Module.Messagerie.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationMessagerie; }
                if (moduleName == Enumerations.Module.ModulePrestataire.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationModulePrestataire; }
                if (moduleName == Enumerations.Module.Annuaire.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationAnnuaire; }
                if (moduleName == Enumerations.Module.ModuleCollectivite.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationModuleCollectivite; }
                //Initialize filter
                string filter = "MenuName='" + menuName + "' and Habilitation='" + habilitation + "'";
                r = (menuRights.Select(filter).Count() != 0);
                if (!r) { Utils.DebugPrint("Unauthorized menu: " + menuName + "/" + moduleName + " - " + habilitation, configuration["Data:DefaultConnection:ConnectionString"]); }
            }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Chek if the access to an action is granted or not
        /// <param name="actionName">The name of the menu</param>
        /// <param name="habilitation">The connected user habilitation for that module</param>
        /// <param name="configuration">The current IConfiguration</param>
        /// </summary>
        public static bool AuthorizedAction(string actionName, Context currentContext, IConfiguration configuration)
        {
            bool r = false;
            if (!string.IsNullOrEmpty(actionName))
            {
                string moduleName = currentContext.EnvActions[actionName].Module.Name;
                string habilitation = "undefined";
                if (moduleName == Enumerations.Module.Administration.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationAdministration; }
                if (moduleName == Enumerations.Module.Logistique.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationLogistique; }
                if (moduleName == Enumerations.Module.Qualite.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationQualite; }
                if (moduleName == Enumerations.Module.Messagerie.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationMessagerie; }
                if (moduleName == Enumerations.Module.ModulePrestataire.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationModulePrestataire; }
                if (moduleName == Enumerations.Module.Annuaire.ToString()) { habilitation = currentContext.ConnectedUtilisateur.HabilitationAnnuaire; }
                //Initialize filter
                string filter = "ActionName='" + actionName + "' and Habilitation='" + habilitation + "'";
                r = (actionRights.Select(filter).Count() != 0);
                if (!r) { Utils.DebugPrint("Unauthorized action: " + actionName + "/" + moduleName + " - " + habilitation, configuration["Data:DefaultConnection:ConnectionString"]); }
            }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Chek if the access to a document is granted or not
        /// <param name="doc">The document</param>
        /// <param name="u">The connected user</param>
        /// </summary>
        public static bool AuthorizedDocument(int refDocument, Utilisateur u, ApplicationDbContext dbContext)
        {
            bool r = false;
            if (u != null)
            {
                //Init
                var doc = dbContext.Documents
                    .Include(i => i.DocumentEntites)
                    .Include(i => i.DocumentEntiteTypes)
                    .Where(e => e.RefDocument == refDocument).FirstOrDefault();
                if (doc != null)
                {
                    //Rights
                    if (u.HabilitationLogistique == Enumerations.HabilitationAnnuaire.Administrateur.ToString()
                        || u.HabilitationLogistique == Enumerations.HabilitationAnnuaire.Utilisateur.ToString()
                        || (doc.DocumentEntites?.Count > 0 && doc.VisibiliteTotale == true && doc.DocumentEntites.Count(e => e.RefEntite == u.Client?.RefEntite) > 0)
                        || (doc.DocumentEntites?.Count > 0 && doc.VisibiliteTotale == true && doc.DocumentEntites.Count(e => e.RefEntite == u.Collectivite?.RefEntite) > 0)
                        || (doc.DocumentEntites?.Count > 0 && doc.VisibiliteTotale == true && doc.DocumentEntites.Count(e => e.RefEntite == u.CentreDeTri?.RefEntite) > 0)
                        || (doc.DocumentEntites?.Count > 0 && doc.VisibiliteTotale == true && doc.DocumentEntites.Count(e => e.RefEntite == u.Transporteur?.RefEntite) > 0)
                        || (doc.DocumentEntites?.Count > 0 && doc.VisibiliteTotale == true && doc.DocumentEntites.Count(e => e.RefEntite == u.Prestataire?.RefEntite) > 0)
                        || (doc.DocumentEntiteTypes?.Count > 0 && doc.DocumentEntites?.Count == 0 && doc.VisibiliteTotale == true && doc.DocumentEntiteTypes.Count(e => e.RefEntiteType == u.Client?.RefEntiteType) > 0)
                        || (doc.DocumentEntiteTypes?.Count > 0 && doc.DocumentEntites?.Count == 0 && doc.VisibiliteTotale == true && doc.DocumentEntiteTypes.Count(e => e.RefEntiteType == u.Collectivite?.RefEntiteType) > 0)
                        || (doc.DocumentEntiteTypes?.Count > 0 && doc.DocumentEntites?.Count == 0 && doc.VisibiliteTotale == true && doc.DocumentEntiteTypes.Count(e => e.RefEntiteType == u.CentreDeTri?.RefEntiteType) > 0)
                        || (doc.DocumentEntiteTypes?.Count > 0 && doc.DocumentEntites?.Count == 0 && doc.VisibiliteTotale == true && doc.DocumentEntiteTypes.Count(e => e.RefEntiteType == u.Transporteur?.RefEntiteType) > 0)
                        || (doc.DocumentEntiteTypes?.Count > 0 && doc.DocumentEntites?.Count == 0 && doc.VisibiliteTotale == true && doc.DocumentEntiteTypes.Count(e => e.RefEntiteType == u.Prestataire?.RefEntiteType) > 0)
                        )
                    {
                        return true;
                    }
                }
            }
            return r;
        }
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Chek if the access to a document is granted or not
        /// <param name="refEntity">The document</param>
        /// <param name="u">The connected user</param>
        /// </summary>
        public static bool AuthorizedEntity(int refEntity, Enumerations.ObjectName entityType, Utilisateur u, ApplicationDbContext dbContext)
        {
            bool r = false;
            if (u != null)
            {
                //Init
                switch (entityType)
                {
                    case Enumerations.ObjectName.CommandeFournisseur:
                        var cmdF = dbContext.CommandeFournisseurs.Find(refEntity);
                        if (cmdF != null)
                        {
                            if (u.RefCentreDeTri != null && u.RefCentreDeTri == cmdF.RefEntite) { r = true; }
                            else if (u.RefTransporteur != null && u.RefTransporteur == cmdF.RefTransporteur) { r = true; }
                            else if (u.RefClient != null && u.RefClient == cmdF.AdresseClient?.RefEntite) { r = true; }
                            else if (u.RefPrestataire != null && u.RefPrestataire == cmdF.RefPrestataire) { r = true; }
                            else if (u.HabilitationLogistique==Enumerations.HabilitationLogistique.Administrateur.ToString()) { r = true; }
                            else if (u.HabilitationLogistique== Enumerations.HabilitationLogistique.Utilisateur.ToString()) { r = true; }
                        }
                        break;
                }
            }
            return r;
        }
    }
}
