/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 16/01/2019
/// ----------------------------------------------------------------------------------------------------- 

using eVaSys.Data;
using System.Globalization;

namespace eVaSys.Utils
{
    /// <summary>
    /// Class for data columns 
    /// </summary>
    public class EnvDataColumn
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EnvDataColumn()
        {
        }
        public string Name { get; set; }
        public int RefRessource { get; set; }
        public string CulturedCaption { get; set; }
        public string Field { get; set; }
        public string FullField { get; set; }
        public string DataType { get; set; }
        // ----------------------------------------------------------------------------------------------------- 
        //Format field
        //-replace NULL by ''
        //-format dates 
        //-format numbers
        public string Format(CultureInfo currentCulture)
        {
            string r = Name;
            if (DataType == Enumerations.EnvDataColumnDataType.date.ToString())
            {
                r = Name + " as [" + Name + "]";
                //r = "isnull(format( " + Name + ", 'd', '" + currentCulture.Name + "' ), '') as [" + Name + "]";
            }
            else if (DataType == Enumerations.EnvDataColumnDataType.dateTime.ToString())
            {
                r = Name + " as [" + Name + "]";
            }
            else if (DataType == Enumerations.EnvDataColumnDataType.year.ToString())
            {
                r = "isnull(format( " + Name + ", '#', '" + currentCulture.Name + "' ), '') as [" + Name + "]";
            }
            else if (DataType == Enumerations.EnvDataColumnDataType.intNumber.ToString())
            {
                r = "isnull(format( " + Name + ", '#,0', '" + currentCulture.Name + "' ), '') as [" + Name + "]";
            }
            else if (DataType == Enumerations.EnvDataColumnDataType.numeroCommande.ToString())
            {
                r = "isnull(format( " + Name + ", '0000 00 0000', '" + currentCulture.Name + "' ), '') as [" + Name + "]";
            }
            else if (DataType == Enumerations.EnvDataColumnDataType.decimalNumber2.ToString())
            {
                r = "isnull(format( " + Name + ", '#,0.00', '" + currentCulture.Name + "' ), '') as [" + Name + "]";
            }
            else if (DataType == Enumerations.EnvDataColumnDataType.decimalNumber3.ToString())
            {
                r = "isnull(format( " + Name + ", '#,0.000', '" + currentCulture.Name + "' ), '') as [" + Name + "]";
            }
            else if (DataType == Enumerations.EnvDataColumnDataType.id.ToString())
            {
                r = "isnull(cast(" + Name + " as nvarchar(20)), '') as [" + Name + "]";
            }
            else if (DataType == Enumerations.EnvDataColumnDataType.text.ToString())
            {
                r = "isnull(" + Name + ",'') as [" + Name + "]";
            }
            else
            {
                r = "isnull(cast(" + Name + " as nvarchar(max)),'') as [" + Name + "]";
            }
            return r;
        }
        public string Cmt { get; set; }
    }
    
    /// <summary>
    /// Class for modules
    /// </summary>
    public class EnvModule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EnvModule()
        {
        }
        public string Name { get; set; }
        public int RefRessource { get; set; }
        public string CulturedCaption { get; set; }
        public string Cmt { get; set; }
    }

    /// <summary>
    /// Class for components
    /// </summary>
    public class EnvComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EnvComponent()
        {
        }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Ref { get; set; }
        public int RessLibel { get; set; }
        public int RessBeforeDel { get; set; }
        public int RessAfterDel { get; set; }
        public string Moments { get; set; }
        public string FormClass { get; set; }
        public bool GetId0 { get; set; }
        public bool CreateModifTooltipVisible { get; set; }
        public bool HasHelp { get; set; } = false;
    }
    /// <summary>
    /// Class for menus
    /// </summary>
    public class EnvMenu
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EnvMenu()
        {
        }
        public string Name { get; set; }
        public EnvModule Module { get; set; }
        public int RefRessource { get; set; }
        public string CulturedCaption { get; set; }
        public string Cmt { get; set; }
        public string GridField { get; set; }
        public bool GridSort { get; set; }
        public string GridGoto { get; set; }
        public string GridRef { get; set; }
    }
    /// <summary>
    /// Class for actions
    /// </summary>
    public class EnvAction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EnvAction()
        {
        }
        public string Name { get; set; }
        public EnvModule Module { get; set; }
    }
    /// <summary>
    /// Class for CommandeFournisseur statuts
    /// </summary>
    public class EnvCommandeFournisseurStatut
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EnvCommandeFournisseurStatut()
        {
        }
        public string Name { get; set; }
        public int RefRessource { get; set; }
        public string CulturedCaption { get; set; }
    }
    /// <summary>
    /// Class for statistics filters
    /// </summary>
    public class EnvStatisticsFilters
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EnvStatisticsFilters(ApplicationDbContext dbContext, CultureInfo culture)
        {
            DbContext = dbContext;
            Culture = culture;
        }
        private ApplicationDbContext DbContext;
        private CultureInfo Culture;
        public string FilterText { get; set; }
        public string StatType { get; set; }
        public string FilterYears { get; set; }
        public string FilterYearsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterYears, "", DbContext, Culture);
                return s;
            }
        }
        public string FilterQuarters { get; set; }
        public string FilterQuartersCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterQuarters, "Quarter", DbContext, Culture);
                return s;
            }
        }
        public string FilterMonths { get; set; }
        public string FilterMonthsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterMonths, "Month", DbContext, Culture);
                return s;
            }
        }
        public string FilterDayWeekMonth { get; set; }
        public string FilterBegin { get; set; }
        public string FilterEnd { get; set; }
        public bool FilterActif { get; set; }
        public string FilterClients { get; set; }
        public string FilterClientsCaption {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterClients, Enumerations.ObjectName.Entite.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterCentreDeTris { get; set; }
        public string FilterCentreDeTrisCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterCentreDeTris, Enumerations.ObjectName.Entite.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterCollectivites { get; set; }
        public string FilterCollectivitesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterCollectivites, Enumerations.ObjectName.Entite.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterDptDeparts { get; set; }
        public string FilterDptDepartsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterDptDeparts, "", DbContext, Culture);
                return s;
            }
        }
        public string FilterDptArrivees { get; set; }
        public string FilterDptArriveesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterDptArrivees, "", DbContext, Culture);
                return s;
            }
        }
        public string FilterDRs { get; set; }
        public string FilterDRsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterDRs, Enumerations.ObjectName.Utilisateur.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterEntites { get; set; }
        public string FilterEntitesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterEntites, Enumerations.ObjectName.Entite.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterPayss { get; set; }
        public string FilterPayssCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterPayss, Enumerations.ObjectName.Pays.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterProcesss { get; set; }
        public string FilterProcesssCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterProcesss, Enumerations.ObjectName.Process.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterProduits { get; set; }
        public string FilterProduitsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterProduits, Enumerations.ObjectName.Produit.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterProduitNomCommunsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterProduits, "ProduitNomCommun", DbContext, Culture);
                return s;
            }
        }
        public string FilterProduitGroupeReportings { get; set; }
        public string FilterProduitGroupeReportingsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterProduitGroupeReportings, Enumerations.ObjectName.ProduitGroupeReporting.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterTransporteurs { get; set; }
        public string FilterTransporteursCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterTransporteurs, Enumerations.ObjectName.Entite.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterVilleDeparts { get; set; }
        public string FilterVilleDepartsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterVilleDeparts, "", DbContext, Culture);
                return s;
            }
        }
        public string FilterAdresseDestinations { get; set; }
        public string FilterAdresseDestinationsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterAdresseDestinations, Enumerations.ObjectName.Adresse.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterVilleArrivees { get; set; }
        public string FilterVilleArriveesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterVilleArrivees, "", DbContext, Culture);
                return s;
            }
        }
        public bool FilterMoins10Euros { get; set; }
        public bool FilterFirstLogin { get; set; }
        public string FilterCollecte { get; set; }
        public string FilterContrat { get; set; }
        public string FilterNonConformiteEtapeTypes { get; set; }
        public string FilterNonConformiteEtapeTypesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterNonConformiteEtapeTypes, Enumerations.ObjectName.NonConformiteEtapeType.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterNonConformiteNatures { get; set; }
        public string FilterNonConformiteNaturesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterNonConformiteNatures, Enumerations.ObjectName.NonConformiteNature.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterCamionTypes { get; set; }
        public string FilterCamionTypesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterCamionTypes, Enumerations.ObjectName.CamionType.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterActionTypes { get; set; }
        public string FilterActionTypesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterActionTypes, Enumerations.ObjectName.ActionType.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterAdresseTypes { get; set; }
        public string FilterAdresseTypesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterAdresseTypes, Enumerations.ObjectName.AdresseType.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterContactAdresseProcesss { get; set; }
        public string FilterContactAdresseProcesssCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterContactAdresseProcesss, Enumerations.ObjectName.ContactAdresseProcess.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterFonctions { get; set; }
        public string FilterFonctionsCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterFonctions, Enumerations.ObjectName.Fonction.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterServices { get; set; }
        public string FilterServicesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterServices, Enumerations.ObjectName.Service.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterEcoOrganismes { get; set; }
        public string FilterEcoOrganismesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterEcoOrganismes, Enumerations.ObjectName.EcoOrganisme.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterEntiteTypes { get; set; }
        public string FilterEntiteTypesCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterEntiteTypes, Enumerations.ObjectName.EntiteType.ToString(), DbContext, Culture);
                return s;
            }
        }
        public string FilterEmailType { get; set; }
        public string FilterUtilisateurs { get; set; }
        public string FilterUtilisateursCaption
        {
            get
            {
                string s = Utils.CreateFilterCaptionFromString(FilterUtilisateurs, Enumerations.ObjectName.Utilisateur.ToString(), DbContext, Culture);
                return s;
            }
        }
    }
}

