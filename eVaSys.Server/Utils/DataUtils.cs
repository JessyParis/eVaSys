/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création :12/10/2021
/// ----------------------------------------------------------------------------------------------------- 

using eVaSys.Data;
using eVaSys.ViewModels;

namespace eVaSys.Utils
{
    /// <summary>
    /// Utilities for dto
    /// </summary>
    public class DataUtils
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private DataUtils()
        {
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for Adresse
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataAdresse(ref Adresse dataModel, AdresseViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.RefAdresseType != viewModel.AdresseType.RefAdresseType
                || dataModel.Actif != (viewModel.Actif ?? false)
                || dataModel.Libelle != Utils.SetEmptyStringToNull(viewModel.Libelle)
                || dataModel.Adr1 != Utils.SetEmptyStringToNull(viewModel.Adr1)
                || dataModel.Adr2 != Utils.SetEmptyStringToNull(viewModel.Adr2)
                || dataModel.CodePostal != Utils.SetEmptyStringToNull(viewModel.CodePostal)
                || dataModel.Ville != Utils.SetEmptyStringToNull(viewModel.Ville)
                || dataModel.RefPays != viewModel.Pays?.RefPays
                || dataModel.Horaires != Utils.SetEmptyStringToNull(viewModel.Horaires)
                || dataModel.Cmt != Utils.SetEmptyStringToNull(viewModel.Cmt)
                || dataModel.Email != Utils.SetEmptyStringToNull(viewModel.Email)
                || dataModel.Tel != Utils.SetEmptyStringToNull(viewModel.Tel)
                || dataModel.SiteWeb != Utils.SetEmptyStringToNull(viewModel.SiteWeb)
                )
            {
                dirty = true;
                //Update data
                dataModel.RefAdresse = viewModel.RefAdresse;
                dataModel.Actif = (viewModel.Actif ?? false);
                dataModel.RefAdresseType = viewModel.AdresseType.RefAdresseType;
                dataModel.Libelle = Utils.SetEmptyStringToNull(viewModel.Libelle);
                dataModel.Adr1 = Utils.SetEmptyStringToNull(viewModel.Adr1);
                dataModel.Adr2 = Utils.SetEmptyStringToNull(viewModel.Adr2);
                dataModel.CodePostal = Utils.SetEmptyStringToNull(viewModel.CodePostal);
                dataModel.Ville = Utils.SetEmptyStringToNull(viewModel.Ville);
                dataModel.RefPays = viewModel.Pays?.RefPays;
                dataModel.Horaires = Utils.SetEmptyStringToNull(viewModel.Horaires);
                dataModel.Cmt = Utils.SetEmptyStringToNull(viewModel.Cmt);
                dataModel.Tel = Utils.SetEmptyStringToNull(viewModel.Tel);
                dataModel.Email = Utils.SetEmptyStringToNull(viewModel.Email);
                dataModel.SiteWeb = Utils.SetEmptyStringToNull(viewModel.SiteWeb);
            }
            //Register session user
            dataModel.RefUtilisateurCourant = refUtilisateurContext;
            //End
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for Action
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataAction(ref Data.Action dataModel, ActionViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.RefEntite != viewModel.RefEntite
            || dataModel.RefFormeContact != viewModel.FormeContact?.RefFormeContact
            || dataModel.RefContactAdresse != viewModel.ContactAdresse?.RefContactAdresse
            || dataModel.DAction != viewModel.DAction
            || dataModel.Libelle != Utils.SetEmptyStringToNull(viewModel.Libelle)
            || dataModel.Cmt != Utils.SetEmptyStringToNull(viewModel.Cmt)

                )
            {
                dirty = true;
                //Update data
                dataModel.RefEntite = viewModel.RefEntite;
                dataModel.RefFormeContact = viewModel.FormeContact.RefFormeContact;
                dataModel.RefContactAdresse = viewModel.ContactAdresse?.RefContactAdresse;
                dataModel.DAction = viewModel.DAction;
                dataModel.Libelle = Utils.SetEmptyStringToNull(viewModel.Libelle);
                dataModel.Cmt = Utils.SetEmptyStringToNull(viewModel.Cmt);
            }
            //-----------------------------------------------------------------------------------
            //Remove related data ActionActionType is deletable
            if (dataModel.ActionActionTypes != null)
            {
                foreach (ActionActionType aAT in dataModel.ActionActionTypes)
                {
                    if (viewModel.ActionActionTypes == null)
                    {
                        dataModel.ActionActionTypes.Remove(aAT);
                        dirty = true;
                    }
                    else if (viewModel.ActionActionTypes.Where(el => el.ActionType.RefActionType == aAT.RefActionType).FirstOrDefault() == null)
                    {
                        dataModel.ActionActionTypes.Remove(aAT);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ActionActionTypes
            if (viewModel.ActionActionTypes != null)
            {
                foreach (ActionActionTypeViewModel aATVM in viewModel.ActionActionTypes)
                {
                    ActionActionType aAT = null;
                    if (dataModel.ActionActionTypes != null)
                    {
                        aAT = dataModel.ActionActionTypes.Where(el => el.RefActionType == aATVM.ActionType.RefActionType).FirstOrDefault();
                    }
                    if (aAT == null)
                    {
                        //Create entity
                        aAT = new ActionActionType() { RefActionType = aATVM.ActionType.RefActionType, };
                        if (dataModel.ActionActionTypes == null) { dataModel.ActionActionTypes = new HashSet<ActionActionType>(); }
                        dataModel.ActionActionTypes.Add(aAT);
                        dirty = true;
                    }
                }
            }
            //Remove duplicates
            if (dataModel.ActionActionTypes != null)
            {
                var resultP = dataModel.ActionActionTypes
                    .AsEnumerable()
                    .GroupBy(s => s.RefActionType)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultP.Count > 0)
                {
                    foreach (var e in resultP)
                    {
                        dataModel.ActionActionTypes.Remove(e);
                    }
                    dirty = true;
                }
            }
            //Files
            //delete files
            if (dataModel.ActionFichierNoFiles != null)
            {
                foreach (ActionFichierNoFile aF in dataModel.ActionFichierNoFiles)
                {
                    if (viewModel.ActionFichierNoFiles.Where(el => el.RefActionFichier == aF.RefActionFichier).FirstOrDefault() == null)
                    {
                        dataModel.ActionFichierNoFiles.Remove(aF);
                    }
                }
            }
            //Register session user
            dataModel.RefUtilisateurCourant = refUtilisateurContext;
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for Produit
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataProduit(ref Produit dataModel, ProduitViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.RefProduit != viewModel.RefProduit
                || dataModel.Actif != (viewModel.Actif ?? false)
                || dataModel.Libelle != Utils.SetEmptyStringToNull(viewModel.Libelle)
                || dataModel.NomCommercial != Utils.SetEmptyStringToNull(viewModel.NomCommercial)
                || dataModel.NomCommun != Utils.SetEmptyStringToNull(viewModel.NomCommun)
                || dataModel.Collecte != (viewModel.Collecte ?? false)
                || dataModel.NumeroStatistique != Utils.SetEmptyStringToNull(viewModel.NumeroStatistique)
                || dataModel.RefSAGECodeTransport != viewModel.SAGECodeTransport?.RefSAGECodeTransport
                || dataModel.RefApplicationProduitOrigine != viewModel.ApplicationProduitOrigine?.RefApplicationProduitOrigine
                || dataModel.Composant != (viewModel.Composant ?? false)
                || dataModel.PUHTSurtri != (viewModel.PUHTSurtri ?? false)
                || dataModel.PUHTTransport != (viewModel.PUHTTransport ?? false)
                || dataModel.RefProduitGroupeReporting != viewModel.ProduitGroupeReporting?.RefProduitGroupeReporting
                || dataModel.CodeListeVerte != Utils.SetEmptyStringToNull(viewModel.CodeListeVerte)
                || dataModel.CodeEE != Utils.SetEmptyStringToNull(viewModel.CodeEE)
                || dataModel.IncitationQualite != (viewModel.IncitationQualite ?? false)
                || dataModel.Cmt != Utils.SetEmptyStringToNull(viewModel.Cmt)
                || dataModel.CmtFournisseur != Utils.SetEmptyStringToNull(viewModel.CmtFournisseur)
                || dataModel.CmtTransporteur != Utils.SetEmptyStringToNull(viewModel.CmtTransporteur)
                || dataModel.CmtClient != Utils.SetEmptyStringToNull(viewModel.CmtClient)
                || dataModel.Co2KgParT != viewModel.Co2KgParT
                )
            {
                dirty = true;
                //Update data
                dataModel.Actif = (viewModel.Actif ?? false);
                dataModel.Libelle = Utils.SetEmptyStringToNull(viewModel.Libelle);
                dataModel.NomCommercial = Utils.SetEmptyStringToNull(viewModel.NomCommercial);
                dataModel.NomCommun = Utils.SetEmptyStringToNull(viewModel.NomCommun);
                dataModel.Collecte = (viewModel.Collecte ?? false);
                dataModel.NumeroStatistique = Utils.SetEmptyStringToNull(viewModel.NumeroStatistique);
                dataModel.RefSAGECodeTransport = viewModel.SAGECodeTransport?.RefSAGECodeTransport;
                dataModel.RefApplicationProduitOrigine = viewModel.ApplicationProduitOrigine?.RefApplicationProduitOrigine;
                dataModel.Composant = (viewModel.Composant ?? false);
                dataModel.PUHTSurtri = (viewModel.PUHTSurtri ?? false);
                dataModel.PUHTTransport = (viewModel.PUHTTransport ?? false);
                dataModel.RefProduitGroupeReporting = viewModel.ProduitGroupeReporting?.RefProduitGroupeReporting;
                dataModel.CodeListeVerte = Utils.SetEmptyStringToNull(viewModel.CodeListeVerte);
                dataModel.CodeEE = Utils.SetEmptyStringToNull(viewModel.CodeEE);
                dataModel.IncitationQualite = (viewModel.IncitationQualite ?? false);
                dataModel.Cmt = Utils.SetEmptyStringToNull(viewModel.Cmt);
                dataModel.CmtFournisseur = Utils.SetEmptyStringToNull(viewModel.CmtFournisseur);
                dataModel.CmtTransporteur = Utils.SetEmptyStringToNull(viewModel.CmtTransporteur);
                dataModel.CmtClient = Utils.SetEmptyStringToNull(viewModel.CmtClient);
                dataModel.Co2KgParT = viewModel.Co2KgParT;
            }
            //-----------------------------------------------------------------------------------
            //Remove related data ProduitComposant
            if (dataModel.ProduitComposants != null)
            {
                foreach (ProduitComposant cACAP in dataModel.ProduitComposants)
                {
                    if (viewModel.ProduitComposants == null)
                    {
                        dataModel.ProduitComposants.Remove(cACAP);
                        dirty = true;
                    }
                    else if (viewModel.ProduitComposants.Where(el => el.Composant.RefProduit == cACAP.RefComposant).FirstOrDefault() == null)
                    {
                        dataModel.ProduitComposants.Remove(cACAP);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ProduitComposants
            if (viewModel.ProduitComposants != null)
            {
                foreach (ProduitComposantViewModel cACAPVM in viewModel.ProduitComposants)
                {
                    ProduitComposant cACAP = null;
                    if (dataModel.ProduitComposants != null)
                    {
                        cACAP = dataModel.ProduitComposants.Where(el => el.RefComposant == cACAPVM.Composant.RefProduit).FirstOrDefault();
                    }
                    if (cACAP == null)
                    {
                        //Create entity
                        cACAP = new ProduitComposant() { RefComposant = cACAPVM.Composant.RefProduit, };
                        if (dataModel.ProduitComposants == null) { dataModel.ProduitComposants = new HashSet<ProduitComposant>(); }
                        dataModel.ProduitComposants.Add(cACAP);
                        dirty = true;
                    }
                }
            }
            //Remove duplicates
            if (dataModel.ProduitComposants != null)
            {
                var resultP = dataModel.ProduitComposants
                    .AsEnumerable()
                    .GroupBy(s => s.RefComposant)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultP.Count > 0)
                {
                    foreach (var e in resultP)
                    {
                        dataModel.ProduitComposants.Remove(e);
                    }
                    dirty = true;
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data ProduitStandard is deletable
            if (dataModel.ProduitStandards != null)
            {
                foreach (ProduitStandard cASF in dataModel.ProduitStandards)
                {
                    if (viewModel.ProduitStandards == null)
                    {
                        dataModel.ProduitStandards.Remove(cASF);
                        dirty = true;
                    }
                    else if (viewModel.ProduitStandards.Where(el => el.Standard?.RefStandard == cASF.RefStandard)
                        .FirstOrDefault() == null)
                    {
                        dataModel.ProduitStandards.Remove(cASF);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ProduitStandards
            if (viewModel.ProduitStandards != null)
            {
                foreach (ProduitStandardViewModel cASFVM in viewModel.ProduitStandards)
                {
                    ProduitStandard cASF = null;
                    if (dataModel.ProduitStandards != null)
                    {
                        cASF = dataModel.ProduitStandards.Where(el =>
                        el.RefStandard == cASFVM.Standard?.RefStandard).FirstOrDefault();
                    }
                    if (cASF == null)
                    {
                        //Create entity
                        cASF = new ProduitStandard() { RefStandard = cASFVM.Standard.RefStandard };
                        if (dataModel.ProduitStandards == null) { dataModel.ProduitStandards = new HashSet<ProduitStandard>(); }
                        dataModel.ProduitStandards.Add(cASF);
                        dirty = true;
                    }
                }
            }
            //Remove duplicates
            if (dataModel.ProduitStandards != null)
            {
                var resultSF = dataModel.ProduitStandards
                    .AsEnumerable()
                    .GroupBy(s => new { s.RefStandard })
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultSF.Count > 0)
                {
                    foreach (var e in resultSF)
                    {
                        dataModel.ProduitStandards.Remove(e);
                    }
                    dirty = true;
                }
            }
            //Register session user
            dataModel.RefUtilisateurCourant = refUtilisateurContext;
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for Document
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataDocument(ref Document dataModel, DocumentViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.RefDocument != viewModel.RefDocument
                || dataModel.Actif != (viewModel.Actif ?? false)
                || dataModel.VisibiliteTotale != (viewModel.VisibiliteTotale ?? false)
                || dataModel.Libelle != Utils.SetEmptyStringToNull(viewModel.Libelle)
                || dataModel.Description != Utils.SetEmptyStringToNull(viewModel.Description)
                || dataModel.DDebut != viewModel.DDebut
                || dataModel.DFin != viewModel.DFin
                )
            {
                dirty = true;
                //Update data
                dataModel.Actif = (viewModel.Actif ?? false);
                dataModel.VisibiliteTotale = (viewModel.VisibiliteTotale ?? false);
                dataModel.Libelle = Utils.SetEmptyStringToNull(viewModel.Libelle);
                dataModel.Description = Utils.SetEmptyStringToNull(viewModel.Description);
                dataModel.DDebut = viewModel.DDebut;
                dataModel.DFin = viewModel.DFin;
            }
            //-----------------------------------------------------------------------------------
            //Remove related data DocumentEntite
            if (dataModel.DocumentEntites != null)
            {
                foreach (DocumentEntite dDE in dataModel.DocumentEntites)
                {
                    if (viewModel.DocumentEntites == null)
                    {
                        dataModel.DocumentEntites.Remove(dDE);
                        dirty = true;
                    }
                    else if (viewModel.DocumentEntites.Where(el => el.RefEntite == dDE.RefEntite).FirstOrDefault() == null)
                    {
                        dataModel.DocumentEntites.Remove(dDE);
                        dirty = true;
                    }
                }
            }
            //Add or update related data DocumentEntites
            if (viewModel.DocumentEntites != null)
            {
                foreach (DocumentEntiteViewModel dDEVM in viewModel.DocumentEntites)
                {
                    DocumentEntite dDE = null;
                    if (dataModel.DocumentEntites != null)
                    {
                        dDE = dataModel.DocumentEntites.Where(el => el.RefEntite == dDEVM.RefEntite).FirstOrDefault();
                    }
                    if (dDE == null)
                    {
                        //Create entity
                        dDE = new DocumentEntite() { RefEntite = dDEVM.RefEntite, };
                        if (dataModel.DocumentEntites == null) { dataModel.DocumentEntites = new HashSet<DocumentEntite>(); }
                        dataModel.DocumentEntites.Add(dDE);
                        dirty = true;
                    }
                }
            }
            //Remove duplicates
            if (dataModel.DocumentEntites != null)
            {
                var resultP = dataModel.DocumentEntites
                    .AsEnumerable()
                    .GroupBy(s => s.RefEntite)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultP.Count > 0)
                {
                    foreach (var e in resultP)
                    {
                        dataModel.DocumentEntites.Remove(e);
                    }
                    dirty = true;
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data DocumentEntiteType is deletable
            if (dataModel.DocumentEntiteTypes != null)
            {
                foreach (DocumentEntiteType dDET in dataModel.DocumentEntiteTypes)
                {
                    if (viewModel.DocumentEntiteTypes == null)
                    {
                        dataModel.DocumentEntiteTypes.Remove(dDET);
                        dirty = true;
                    }
                    else if (viewModel.DocumentEntiteTypes.Where(el => el.EntiteType?.RefEntiteType == dDET.RefEntiteType)
                        .FirstOrDefault() == null)
                    {
                        dataModel.DocumentEntiteTypes.Remove(dDET);
                        dirty = true;
                    }
                }
            }
            //Add or update related data DocumentEntiteTypes
            if (viewModel.DocumentEntiteTypes != null)
            {
                foreach (DocumentEntiteTypeViewModel dDETVM in viewModel.DocumentEntiteTypes)
                {
                    DocumentEntiteType dDET = null;
                    if (dataModel.DocumentEntiteTypes != null)
                    {
                        dDET = dataModel.DocumentEntiteTypes.Where(el =>
                        el.RefEntiteType == dDETVM.EntiteType?.RefEntiteType).FirstOrDefault();
                    }
                    if (dDET == null)
                    {
                        //Create entity
                        dDET = new DocumentEntiteType() { RefEntiteType = dDETVM.EntiteType.RefEntiteType };
                        if (dataModel.DocumentEntiteTypes == null) { dataModel.DocumentEntiteTypes = new HashSet<DocumentEntiteType>(); }
                        dataModel.DocumentEntiteTypes.Add(dDET);
                        dirty = true;
                    }
                }
            }
            //Remove duplicates
            if (dataModel.DocumentEntiteTypes != null)
            {
                var resultSF = dataModel.DocumentEntiteTypes
                    .AsEnumerable()
                    .GroupBy(s => new { s.RefEntiteType })
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultSF.Count > 0)
                {
                    foreach (var e in resultSF)
                    {
                        dataModel.DocumentEntiteTypes.Remove(e);
                    }
                    dirty = true;
                }
            }
            //Register session user
            dataModel.RefUtilisateurCourant = refUtilisateurContext;
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for ContactAdresse
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataContactAdresse(ref ContactAdresse dataModel, ContactAdresseViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.RefAdresse != viewModel.RefAdresse
                || dataModel.Actif != (viewModel.Actif ?? false)
                || dataModel.Contact.RefCivilite != viewModel.Contact.Civilite?.RefCivilite
                || dataModel.RefTitre != viewModel.Titre?.RefTitre
                || dataModel.Contact.Nom != Utils.SetEmptyStringToNull(viewModel.Contact.Nom)
                || dataModel.Contact.Prenom != Utils.SetEmptyStringToNull(viewModel.Contact.Prenom)
                || dataModel.Email != Utils.SetEmptyStringToNull(viewModel.Email)
                || dataModel.Tel != Utils.SetEmptyStringToNull(viewModel.Tel)
                || dataModel.TelMobile != Utils.SetEmptyStringToNull(viewModel.TelMobile)
                || dataModel.Cmt != Utils.SetEmptyStringToNull(viewModel.Cmt)
                || dataModel.CmtServiceFonction != Utils.SetEmptyStringToNull(viewModel.CmtServiceFonction)
                )
            {
                dirty = true;
                //Update data
                dataModel.RefContactAdresse = viewModel.RefContactAdresse;
                dataModel.RefAdresse = viewModel.RefAdresse;
                dataModel.Actif = (viewModel.Actif ?? false);
                dataModel.Contact.RefCivilite = viewModel.Contact.Civilite?.RefCivilite;
                dataModel.RefTitre = viewModel.Titre?.RefTitre;
                dataModel.Contact.Nom = Utils.SetEmptyStringToNull(viewModel.Contact.Nom);
                dataModel.Contact.Prenom = Utils.SetEmptyStringToNull(viewModel.Contact.Prenom);
                dataModel.Email = Utils.SetEmptyStringToNull(viewModel.Email);
                dataModel.Tel = Utils.SetEmptyStringToNull(viewModel.Tel);
                dataModel.TelMobile = Utils.SetEmptyStringToNull(viewModel.TelMobile);
                dataModel.Cmt = Utils.SetEmptyStringToNull(viewModel.Cmt);
                dataModel.CmtServiceFonction = Utils.SetEmptyStringToNull(viewModel.CmtServiceFonction);
            }
            //-----------------------------------------------------------------------------------
            //Remove related data ContactAdresseContactAdresseProcess is deletable
            if (dataModel.ContactAdresseContactAdresseProcesss != null)
            {
                foreach (ContactAdresseContactAdresseProcess cACAP in dataModel.ContactAdresseContactAdresseProcesss)
                {
                    if (viewModel.ContactAdresseContactAdresseProcesss == null)
                    {
                        dataModel.ContactAdresseContactAdresseProcesss.Remove(cACAP);
                        dirty = true;
                    }
                    else if (viewModel.ContactAdresseContactAdresseProcesss.Where(el => el.ContactAdresseProcess.RefContactAdresseProcess == cACAP.RefContactAdresseProcess).FirstOrDefault() == null)
                    {
                        dataModel.ContactAdresseContactAdresseProcesss.Remove(cACAP);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ContactAdresseContactAdresseProcesss
            if (viewModel.ContactAdresseContactAdresseProcesss != null)
            {
                foreach (ContactAdresseContactAdresseProcessViewModel cACAPVM in viewModel.ContactAdresseContactAdresseProcesss)
                {
                    ContactAdresseContactAdresseProcess cACAP = null;
                    if (dataModel.ContactAdresseContactAdresseProcesss != null)
                    {
                        cACAP = dataModel.ContactAdresseContactAdresseProcesss.Where(el => el.RefContactAdresseProcess == cACAPVM.ContactAdresseProcess.RefContactAdresseProcess).FirstOrDefault();
                    }
                    if (cACAP == null)
                    {
                        //Create entity
                        cACAP = new ContactAdresseContactAdresseProcess() { RefContactAdresseProcess = cACAPVM.ContactAdresseProcess.RefContactAdresseProcess, };
                        if (dataModel.ContactAdresseContactAdresseProcesss == null) { dataModel.ContactAdresseContactAdresseProcesss = new HashSet<ContactAdresseContactAdresseProcess>(); }
                        dataModel.ContactAdresseContactAdresseProcesss.Add(cACAP);
                        dirty = true;
                    }
                }
            }
            //Remove duplicates
            if (dataModel.ContactAdresseContactAdresseProcesss != null)
            {
                var resultP = dataModel.ContactAdresseContactAdresseProcesss
                    .AsEnumerable()
                    .GroupBy(s => s.RefContactAdresseProcess)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultP.Count > 0)
                {
                    foreach (var e in resultP)
                    {
                        dataModel.ContactAdresseContactAdresseProcesss.Remove(e);
                    }
                    dirty = true;
                }
            }
            //-----------------------------------------------------------------------------------
            //Remove related data ContactAdresseServiceFonction is deletable
            if (dataModel.ContactAdresseServiceFonctions != null)
            {
                foreach (ContactAdresseServiceFonction cASF in dataModel.ContactAdresseServiceFonctions)
                {
                    if (viewModel.ContactAdresseServiceFonctions == null)
                    {
                        dataModel.ContactAdresseServiceFonctions.Remove(cASF);
                        dirty = true;
                    }
                    else if (viewModel.ContactAdresseServiceFonctions.Where(el =>
                        el.Service?.RefService == cASF.RefService && el.Fonction?.RefFonction == cASF.RefFonction)
                        .FirstOrDefault() == null)
                    {
                        dataModel.ContactAdresseServiceFonctions.Remove(cASF);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ContactAdresseServiceFonctions
            if (viewModel.ContactAdresseServiceFonctions != null)
            {
                foreach (ContactAdresseServiceFonctionViewModel cASFVM in viewModel.ContactAdresseServiceFonctions)
                {
                    ContactAdresseServiceFonction cASF = null;
                    if (dataModel.ContactAdresseServiceFonctions != null)
                    {
                        cASF = dataModel.ContactAdresseServiceFonctions.Where(el =>
                        el.RefService == cASFVM.Service?.RefService && el.RefFonction == cASFVM.Fonction?.RefFonction).FirstOrDefault();
                    }
                    if (cASF == null)
                    {
                        //Create entity
                        cASF = new ContactAdresseServiceFonction() { RefService = cASFVM.Service?.RefService, RefFonction = cASFVM.Fonction?.RefFonction };
                        if (dataModel.ContactAdresseServiceFonctions == null) { dataModel.ContactAdresseServiceFonctions = new HashSet<ContactAdresseServiceFonction>(); }
                        dataModel.ContactAdresseServiceFonctions.Add(cASF);
                        dirty = true;
                    }
                }
            }
            //Remove duplicates
            if (dataModel.ContactAdresseServiceFonctions != null)
            {
                var resultSF = dataModel.ContactAdresseServiceFonctions
                    .AsEnumerable()
                    .GroupBy(s => new { s.RefService, s.RefFonction })
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultSF.Count > 0)
                {
                    foreach (var e in resultSF)
                    {
                        dataModel.ContactAdresseServiceFonctions.Remove(e);
                    }
                    dirty = true;
                }
            }
            //Register session user
            dataModel.RefUtilisateurCourant = refUtilisateurContext;
            dataModel.Contact.RefUtilisateurCourant = refUtilisateurContext;
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for ContratIncitationQualite
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataContratIncitationQualite(ref ContratIncitationQualite dataModel, ContratIncitationQualiteViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.DDebut != viewModel.DDebut
                || dataModel.DFin != viewModel.DFin
                )
            {
                dirty = true;
                //Update data
                dataModel.DDebut = viewModel.DDebut;
                dataModel.DFin = viewModel.DFin;
            }
            //End
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for Contrat
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataContrat(ref Contrat dataModel, ContratViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.IdContrat != viewModel.IdContrat
                || dataModel.RefContratType != viewModel.ContratType?.RefContratType
                || dataModel.DDebut != viewModel.DDebut
                || dataModel.DFin != viewModel.DFin
                || dataModel.ReconductionTacite != viewModel.ReconductionTacite
                || dataModel.Avenant != viewModel.Avenant
                || dataModel.Cmt != viewModel.Cmt
                )
            {
                dirty = true;
                //Update data
                dataModel.IdContrat = viewModel.IdContrat;
                dataModel.RefContratType = viewModel.ContratType?.RefContratType;
                dataModel.DDebut = viewModel.DDebut;
                dataModel.DDebut = viewModel.DDebut;
                dataModel.DFin = viewModel.DFin;
                dataModel.ReconductionTacite = viewModel.ReconductionTacite;
                dataModel.Avenant = viewModel.Avenant;
                dataModel.Cmt = viewModel.Cmt;
            }
            //-----------------------------------------------------------------------------------
            //Remove related data ContratEntite
            if (dataModel.ContratEntites != null)
            {
                foreach (ContratEntite cE in dataModel.ContratEntites)
                {
                    if (viewModel.ContratEntites == null)
                    {
                        dataModel.ContratEntites.Remove(cE);
                        dirty = true;
                    }
                    else if (viewModel.ContratEntites.Where(el => el.RefEntite == cE.RefEntite).FirstOrDefault() == null)
                    {
                        dataModel.ContratEntites.Remove(cE);
                        dirty = true;
                    }
                }
            }
            //Add or update related data ContratEntites
            if (viewModel.ContratEntites != null)
            {
                foreach (ContratEntiteViewModel cEVM in viewModel.ContratEntites)
                {
                    ContratEntite cE = null;
                    if (dataModel.ContratEntites != null)
                    {
                        cE = dataModel.ContratEntites.Where(el => el.RefEntite == cEVM.RefEntite).FirstOrDefault();
                    }
                    if (cE == null)
                    {
                        //Create entity
                        cE = new ContratEntite() { RefEntite = cEVM.RefEntite, };
                        if (dataModel.ContratEntites == null) { dataModel.ContratEntites = new HashSet<ContratEntite>(); }
                        dataModel.ContratEntites.Add(cE);
                        dirty = true;
                    }
                }
            }
            //Remove duplicates
            if (dataModel.ContratEntites != null)
            {
                var resultP = dataModel.ContratEntites
                    .AsEnumerable()
                    .GroupBy(s => s.RefEntite)
                    .SelectMany(g => g.Skip(1))
                    .ToList();
                if (resultP.Count > 0)
                {
                    foreach (var e in resultP)
                    {
                        dataModel.ContratEntites.Remove(e);
                    }
                    dirty = true;
                }
            }
            //End
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for ContratCollectivite
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataContratCollectivite(ref ContratCollectivite dataModel, ContratCollectiviteViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.DDebut != viewModel.DDebut
                || dataModel.DFin != viewModel.DFin
                || dataModel.Avenant != viewModel.Avenant
                || dataModel.Cmt != viewModel.Cmt
                )
            {
                dirty = true;
                //Update data
                dataModel.DDebut = viewModel.DDebut;
                dataModel.DFin = viewModel.DFin;
                dataModel.Avenant = viewModel.Avenant;
                dataModel.Cmt = viewModel.Cmt;
            }
            //End
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for EntiteEntite
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataEntiteEntite(ref EntiteEntite dataModel, EntiteEntiteViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.Actif != viewModel.Actif
                || dataModel.RefEntite != viewModel.RefEntite
                || dataModel.RefEntiteRtt != viewModel.RefEntiteRtt
                || dataModel.Cmt != Utils.SetEmptyStringToNull(viewModel.Cmt)
                )
            {
                dirty = true;
                //Update data
                dataModel.Actif = viewModel.Actif;
                dataModel.RefEntite = viewModel.RefEntite;
                dataModel.RefEntiteRtt = viewModel.RefEntiteRtt;
                dataModel.EntiteRtt = null;
                dataModel.Cmt = Utils.SetEmptyStringToNull(viewModel.Cmt);
                //Register session user
                if (dataModel.RefUtilisateurCreation == null && dataModel.RefEntiteEntite <= 0)
                {
                    dataModel.RefUtilisateurCreation = refUtilisateurContext;
                    dataModel.DCreation = System.DateTime.Now;
                }
            }
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for EntiteCamionType
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataEntiteCamionType(ref EntiteCamionType dataModel, EntiteCamionTypeViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.RefEntite != viewModel.RefEntite
                || dataModel.RefCamionType != viewModel.CamionType?.RefCamionType
                || dataModel.Cmt != Utils.SetEmptyStringToNull(viewModel.Cmt)
                )
            {
                dirty = true;
                //Update data
                dataModel.RefEntite = viewModel.RefEntite;
                dataModel.RefCamionType = viewModel.CamionType.RefCamionType;
                dataModel.CamionType = null;
                dataModel.Cmt = Utils.SetEmptyStringToNull(viewModel.Cmt);
                //Register session user
                if (dataModel.UtilisateurCreation == null && dataModel.RefEntiteCamionType <= 0)
                {
                    dataModel.RefUtilisateurCreation = refUtilisateurContext;
                    dataModel.DCreation = System.DateTime.Now;
                }
            }
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for EntiteProduit
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataEntiteProduit(ref EntiteProduit dataModel, EntiteProduitViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.RefEntite != viewModel.RefEntite
                || dataModel.RefProduit != viewModel.Produit?.RefProduit
                || dataModel.Interdit != viewModel.Interdit
                || dataModel.Cmt != Utils.SetEmptyStringToNull(viewModel.Cmt)
                )
            {
                dirty = true;
                //Update data
                dataModel.RefEntite = viewModel.RefEntite;
                dataModel.RefProduit = viewModel.Produit.RefProduit;
                dataModel.Produit = null;
                dataModel.Interdit = viewModel.Interdit;
                dataModel.Cmt = Utils.SetEmptyStringToNull(viewModel.Cmt);
                //Register session user
                if (dataModel.UtilisateurCreation == null && dataModel.RefEntiteProduit <= 0)
                {
                    dataModel.RefUtilisateurCreation = refUtilisateurContext;
                    dataModel.DCreation = System.DateTime.Now;
                }
            }
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for PrixReprise
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataPrixReprise(ref PrixReprise dataModel, PrixRepriseViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.RefProcess != viewModel.Process?.RefProcess
                || dataModel.RefProduit != viewModel.Produit?.RefProduit
                || dataModel.RefComposant != viewModel.Composant?.RefProduit
                || dataModel.RefContrat != viewModel.Contrat?.RefContrat
                || dataModel.D != viewModel.D
                || dataModel.PUHT != (viewModel.PUHT ?? 0)
                || dataModel.PUHTSurtri != (viewModel.PUHTSurtri ?? 0)
                || dataModel.PUHTTransport != (viewModel.PUHTTransport ?? 0)
                )
            {
                dirty = true;
            }
            //Save data
            if (dirty)
            {
                //Update data
                dataModel.RefProcess = viewModel.Process?.RefProcess;
                dataModel.RefProduit = viewModel.Produit.RefProduit;
                dataModel.RefComposant = viewModel.Composant?.RefProduit;
                dataModel.RefContrat = viewModel.Contrat?.RefContrat;
                dataModel.D = viewModel.D;
                dataModel.PUHT = viewModel.PUHT ?? 0;
                dataModel.PUHTSurtri = viewModel.PUHTSurtri ?? 0;
                dataModel.PUHTTransport = viewModel.PUHTTransport ?? 0;
            }
            else
            {
                dataModel.ApplyMarkModification = false; // Avoid mark modification if no data modified
            }
            //Register certification/uncertification
            if (viewModel.Certif == true)
            {
                dataModel.RefUtilisateurCertif = refUtilisateurContext;
                dataModel.DCertif = DateTime.Now;
            }
            if (viewModel.Certif == false)
            {
                dataModel.RefUtilisateurCertif = null;
                dataModel.DCertif = null;
            }
            //Register session user
            dataModel.RefUtilisateurCourant = refUtilisateurContext;
            return dirty;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save values for PrixReprise
        /// <param name="dataModel">The dto object to write data to</param>
        /// <param name="viewModel">The view model to read data from </param>
        /// <returns>true if data are different (dirty)</returns>
        /// </summary>
        public static bool UpdateDataCommandeClientMensuelle(ref CommandeClientMensuelle dataModel, CommandeClientMensuelleFormViewModel viewModel, int refUtilisateurContext)
        {
            bool dirty = false;
            //Mark as dirty if applicable
            if (dataModel.Poids != viewModel.Poids
                || dataModel.PrixTonneHT != viewModel.PrixTonneHT
                || dataModel.IdExt != viewModel.IdExt
                || dataModel.D != viewModel.D
                )
            {
                dirty = true;
            }
            //Save data
            if (dirty)
            {
                //Update data
                dataModel.Poids = (int)viewModel.Poids;
                dataModel.PrixTonneHT = (decimal)(viewModel.PrixTonneHT == null ? 0 : viewModel.PrixTonneHT);
                dataModel.IdExt = viewModel.IdExt;
                dataModel.D = viewModel.D;
            }
            //Register certification/uncertification
            if (viewModel.Certif == true)
            {
                dataModel.RefUtilisateurCertif = refUtilisateurContext;
                dataModel.DCertif = DateTime.Now;
            }
            if (viewModel.Certif == false)
            {
                dataModel.RefUtilisateurCertif = null;
                dataModel.DCertif = null;
            }
            return dirty;
        }
        /// <summary>
        /// Write received data to dto
        /// </summary>
        public static bool UpdateDataRepartition(ref Repartition dataModel, RepartitionViewModel viewModel, int refUtilisateurContext)
        {
            //Update main data
            dataModel.RefFournisseur = (viewModel.CommandeFournisseur == null ? (int?)viewModel.Fournisseur.RefEntite : null);
            dataModel.RefCommandeFournisseur = viewModel.CommandeFournisseur?.RefCommandeFournisseur;
            dataModel.RefProduit = (viewModel.CommandeFournisseur == null ? (int?)viewModel.Produit.RefProduit : null); ;
            dataModel.D = (viewModel.CommandeFournisseur == null ? viewModel.D : null);
            //Dirty marker
            bool dirty = false;
            //Remove related data RepartitionCollectivite
            if (dataModel.RepartitionCollectivites != null)
            {
                foreach (RepartitionCollectivite rC in dataModel.RepartitionCollectivites)
                {
                    if (viewModel.RepartitionCollectivites == null)
                    {
                        dataModel.RepartitionCollectivites.Remove(rC);
                        dirty = true;
                    }
                    else if (viewModel.RepartitionCollectivites.Where(el => el.RefRepartitionCollectivite == rC.RefRepartitionCollectivite).FirstOrDefault() == null)
                    {
                        dataModel.RepartitionCollectivites.Remove(rC);
                        dirty = true;
                    }
                }
            }
            //Add or update related data RepartitionCollectivites
            foreach (RepartitionCollectiviteViewModel rCVM in viewModel.RepartitionCollectivites)
            {
                RepartitionCollectivite rC = null;
                if (dataModel.RepartitionCollectivites != null && rCVM.RefRepartitionCollectivite != 0)
                {
                    rC = dataModel.RepartitionCollectivites.Where(el => el.RefRepartitionCollectivite == rCVM.RefRepartitionCollectivite).FirstOrDefault();
                }
                if (rC == null)
                {
                    rC = new RepartitionCollectivite();
                    if (dataModel.RepartitionCollectivites == null) { dataModel.RepartitionCollectivites = new HashSet<RepartitionCollectivite>(); }
                    dataModel.RepartitionCollectivites.Add(rC);
                }
                //Mark as dirty if applicable
                if (rC.RefCollectivite != rCVM.Collectivite.RefEntite
                    || rC.RefProcess != rCVM.Process?.RefProcess
                    || rC.RefProduit != rCVM.Produit?.RefProduit
                    || rC.Poids != rCVM.Poids
                    || rC.PUHT != rCVM.PUHT) { dirty = true; }
                //Update data
                rC.RefCollectivite = rCVM.Collectivite.RefEntite;
                rC.RefProcess = rCVM.Process?.RefProcess;
                rC.RefProduit = rCVM.Produit?.RefProduit;
                rC.Poids = rCVM.Poids;
                rC.PUHT = rCVM.PUHT;
            }
            //Remove related data RepartitionProduit
            if (dataModel.RepartitionProduits != null)
            {
                foreach (RepartitionProduit rC in dataModel.RepartitionProduits)
                {
                    if (viewModel.RepartitionProduits == null)
                    {
                        dataModel.RepartitionProduits.Remove(rC);
                        dirty = true;
                    }
                    else if (viewModel.RepartitionProduits.Where(el => el.RefRepartitionProduit == rC.RefRepartitionProduit).FirstOrDefault() == null)
                    {
                        dataModel.RepartitionProduits.Remove(rC);
                        dirty = true;
                    }
                }
            }
            //Add or update related data RepartitionProduits
            foreach (RepartitionProduitViewModel rCVM in viewModel.RepartitionProduits)
            {
                RepartitionProduit rC = null;
                if (dataModel.RepartitionProduits != null && rCVM.RefRepartitionProduit != 0)
                {
                    rC = dataModel.RepartitionProduits.Where(el => el.RefRepartitionProduit == rCVM.RefRepartitionProduit).FirstOrDefault();
                }
                if (rC == null)
                {
                    rC = new RepartitionProduit();
                    if (dataModel.RepartitionProduits == null) { dataModel.RepartitionProduits = new HashSet<RepartitionProduit>(); }
                    dataModel.RepartitionProduits.Add(rC);
                }
                //Mark as dirty if applicable
                if (rC.RefFournisseur != rCVM.Fournisseur?.RefEntite
                    || rC.RefProcess != rCVM.Process?.RefProcess
                    || rC.RefProduit != rCVM.Produit?.RefProduit
                    || rC.Poids != rCVM.Poids
                    || rC.PUHT != rCVM.PUHT) { dirty = true; }
                //Update data
                rC.RefFournisseur = rCVM.Fournisseur?.RefEntite;
                rC.RefProcess = rCVM.Process?.RefProcess;
                rC.RefProduit = rCVM.Produit?.RefProduit;
                rC.Poids = rCVM.Poids;
                rC.PUHT = rCVM.PUHT;
            }
            //Mark for modification if applicable
            if (dirty) { dataModel.DModif = DateTime.Now; }
            //End
            return dirty;
        }
    }
}

