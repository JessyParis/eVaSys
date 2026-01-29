/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création :29/08/2022
/// ----------------------------------------------------------------------------------------------------- 

using eVaSys.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using static eVaSys.Utils.Enumerations;

namespace eVaSys.Utils
{
    /// <summary>
    /// Object for Laser credentials
    /// </summary>
    public class LaserCredentials
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    /// <summary>
    /// Object for getting acces token
    /// </summary>
    public class LaserAccessToken
    {
        public string access { get; set; }
    }
    /// <summary>
    /// Global object for Laser transition
    /// </summary>
    public class LaserTransition
    {
        public string workflow { get; set; }
        public string transitionName { get; set; }
        public string transaction { get; set; }     //"/api/transactions/{transaction_id}"
        public LaserAdditionalInfo additionalInfo { get; set; } = null;
        public string comment { get; set; }
    }
    /// <summary>
    /// Global object for Laser transition additional info
    /// </summary>
    public class LaserAdditionalInfo
    {
        public string forecastedCollectionDate { get; set; }    //yyyy-MM-ddThh:mm:ss.FFFZ
        public string forecastedDeliveryDate { get; set; }    //yyyy-MM-ddThh:mm:ss.FFFZ
        public string effectiveCollectionDate { get; set; }    //yyyy-MM-ddThh:mm:ss.FFFZ
        public string effectiveDeliveryDate { get; set; }    //yyyy-MM-ddThh:mm:ss.FFFZ
        public int? effectiveQuantity { get; set; }
        public int? effectiveBalesAmount { get; set; }
        public string receiptDate { get; set; }    //yyyy-MM-ddThh:mm:ss.FFFZ
        public LaserTransactionComment transactionComment { get; set; } = null;
        public string logisticsChain { get; set; }
    }
    /// <summary>
    /// Global object for Laser transition additional info transaction comment
    /// </summary>
    public class LaserTransactionComment
    {
        public string comment { get; set; }
        public string transaction { get; set; }     //"/api/transactions/{transaction_id}"
        public string type { get; set; }
    }
    /// <summary>
    /// Global object for Laser transaction
    /// </summary>
    public class LaserTransaction
    {
        public int id { get; set; }
        public string businessId { get; set; }
        public LaserActor sourceActor { get; set; }
        public LaserLogisticsChain logisticsChain { get; set; } = null;
        public LaserQuality quality { get; set; } = null;
        public LaserFlow flow { get; set; } = null;
        public int forecastedQuantity { get; set; }
        public int forecastedBalesAmount { get; set; }
        public DateTime sourceAvailabilityDate { get; set; }    //yyyy-MM-ddThh:mm:ss.FFFZ
        public string carrierRejectedLastComment { get; set; }
        public string receiptRejectedLastComment { get; set; }
        public string forecastedCollectionDate { get; set; }    //yyyy-MM-ddThh:mm:ss.FFFZ
        public string forecastedDeliveryDate { get; set; }    //yyyy-MM-ddThh:mm:ss.FFFZ
    }
    public class LaserLogisticsChain
    {
        public int id { get; set; }
        public LaserActor sourceActor { get; set; }
        public LaserActor destinationActor { get; set; }
        public LaserQuality quality { get; set; } = null;
        public LaserFlow flow { get; set; } = null;
    }
    public class LaserActor
    {
        public string name { get; set; }
        public string code { get; set; }
        public string addressLine { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public LaserOperationalContact operationalContact { get; set; } = null;
    }
    public class LaserOperationalContact
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
    public class LaserQuality
    {
        public string name { get; set; }
        public string code { get; set; }
    }
    public class LaserFlow
    {
        public string name { get; set; }
        public string type { get; set; }
    }
    public class LaserLogisticsChains
    {
        [JsonPropertyName("hydra:member")]
        public LaserLogisticsChain[] logistiqChains { get; set; }
    }
    /// <summary>
    /// Laser access token
    /// </summary>
    public class LaserAPIUtils
    {
        //protected IConfiguration Configuration { get; private set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public LaserAPIUtils(/*IConfiguration configuration*/)
        {
            // Instantiate the ApplicationDbContext through DI
            //Configuration = configuration;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Logs the user in as a carrier
        /// <param client="HttpClient">The HttpClient to use to get auth cookie</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <returns>true if API call success, false if not</returns>
        /// </summary>
        public static string AuthenticateAsCarrier(ref HttpClient client, int refUtilisateur
            , CommandeFournisseur cmdF, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            string r = "";
            string responseBody = "";
            string jsonBody = "";
            string url = configuration["AppResources:Laser:Url"] + "/api/auth/jwt/create";
            string refCmd = (cmdF?.NumeroCommande.ToString() ?? "NA") + " (" + (cmdF?.RefExt ?? "NA") + "-" + (cmdF?.LibExt ?? "NA") + ") - ";
            //Process
            try
            {
                var bodyCredentials = new LaserCredentials()
                {
                    email = configuration["AppResources:Laser:TransporteurEmail"],
                    password = configuration["AppResources:Laser:TransporteurPassword"]
                };
                var encodedContent = System.Net.Http.Json.JsonContent.Create(bodyCredentials);
                jsonBody = JsonSerializer.Serialize(bodyCredentials);
                //Post http call for authentification
                HttpResponseMessage response = client.PostAsync(url, encodedContent).Result;
                response.EnsureSuccessStatusCode();
                responseBody = response.Content.ReadAsStringAsync().Result;
                //Get JSON response
                LaserAccessToken cr = JsonSerializer.Deserialize<LaserAccessToken>(responseBody);
                r = cr.access;
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
            }
            catch (HttpRequestException e)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int?)(e.StatusCode), responseBody, refCmd + e.Message, refUtilisateur, dbContext);
                //Création du message à l'usage des utilisateurs
                CreateAPIErrorMessage(cmdF, e, "Erreur d'authentification transporteur", refUtilisateur, dbContext, configuration);
            }
            //End
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Logs the user in as a recycler
        /// <param client="HttpClient">The HttpClient to use to get auth cookie</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <returns>true if API call success, false if not</returns>
        /// </summary>
        public static string AuthenticateAsDestination(ref HttpClient client, int refUtilisateur
            , CommandeFournisseur cmdF, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            string r = "";
            string responseBody = "";
            string jsonBody = "";
            string url = configuration["AppResources:Laser:Url"] + "/api/auth/jwt/create";
            string refCmd = cmdF?.NumeroCommande.ToString() ?? "NA" + " (" + (cmdF?.RefExt ?? "NA") + "-" + (cmdF?.LibExt ?? "NA") + ") - ";
            //Process
            try
            {
                var bodyCredentials = new LaserCredentials()
                {
                    email = configuration["AppResources:Laser:RecycleurEmail"],
                    password = configuration["AppResources:Laser:RecycleurPassword"]
                };
                var encodedContent = System.Net.Http.Json.JsonContent.Create(bodyCredentials);
                jsonBody = JsonSerializer.Serialize(bodyCredentials);
                //Post http call for authentification
                HttpResponseMessage response = client.PostAsync(url, encodedContent).Result;
                response.EnsureSuccessStatusCode();
                responseBody = response.Content.ReadAsStringAsync().Result;
                //Get JSON response
                LaserAccessToken cr = JsonSerializer.Deserialize<LaserAccessToken>(responseBody);
                r = cr.access;
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
            }
            catch (HttpRequestException e)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int?)(e.StatusCode), responseBody, refCmd + e.Message, refUtilisateur, dbContext);
                //Création du message à l'usage des utilisateurs
                CreateAPIErrorMessage(cmdF, e, "Erreur d'authentification recycleur", refUtilisateur, dbContext, configuration);
            }
            //End
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Refuse carriage
        /// <param cmdF>The related CommandeFournisseur object</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <returns>true if API call success, false if not</returns>
        /// </summary>
        public static bool CommandeFournisseurRefuseCarriage(CommandeFournisseur cmdF, int refUtilisateur, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            bool r = false;
            string responseBody = "";
            string jsonBody = "";
            string url = configuration["AppResources:Laser:Url"] + "/api/transitions";
            //Process
            HttpClient client = new HttpClient();
            try
            {
                string loginAccess = AuthenticateAsCarrier(ref client, refUtilisateur, cmdF, dbContext, configuration);
                if (!string.IsNullOrWhiteSpace(loginAccess))
                {
                    var bodyLaserForecastedCarrierDates = new LaserTransition()
                    {
                        workflow = "transaction_shipping_status",
                        transitionName = "reject_carrier",
                        transaction = "/api/transactions/" + cmdF.RefExt
                    };
                    var encodedContent = JsonContent.Create(bodyLaserForecastedCarrierDates, typeof(LaserTransition), null, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                    jsonBody = JsonSerializer.Serialize(bodyLaserForecastedCarrierDates);
                    //Set header
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginAccess);
                    //Post http call for sending forcasted carrier dates
                    var response = client.PostAsync(url, encodedContent).Result;
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    r = true;
                    //Log
                    APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
                }
            }
            catch (HttpRequestException e)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int?)(e.StatusCode), responseBody, e.Message, refUtilisateur, dbContext);
                //Création du message à l'usage des utilisateurs
                CreateAPIErrorMessage(cmdF, e, "Erreur d'envoi de suppression d'une demande d'enlèvement", refUtilisateur, dbContext, configuration);
            }
            //End
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Modify carrier
        /// <param cmdF>The related CommandeFournisseur object</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <returns>true if API call success, false if not</returns>
        /// </summary>
        public static bool CommandeFournisseurModifyCarrier(CommandeFournisseur cmdF, int refUtilisateur, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            bool r = false;
            string responseBody = "";
            string jsonBody = "";
            string url = configuration["AppResources:Laser:Url"] + "/api/transitions";
            //Process
            HttpClient client = new HttpClient();
            try
            {
                string loginAccess = AuthenticateAsCarrier(ref client, refUtilisateur, cmdF, dbContext, configuration);
                if (!string.IsNullOrWhiteSpace(loginAccess))
                {
                    var bodyLaserForecastedCarrierDates = new LaserTransition()
                    {
                        workflow = "transaction_shipping_status",
                        transitionName = "update_transaction",
                        transaction = "/api/transactions/" + cmdF.RefExt,
                        additionalInfo = { },
                        comment = "Modification transporteur : " + cmdF.Transporteur?.Libelle
                        + ". Nro commande Valorplast : " + Utils.FormatNumeroCommande(cmdF.NumeroCommande.ToString())
                    };
                    var encodedContent = JsonContent.Create(bodyLaserForecastedCarrierDates, typeof(LaserTransition), null, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                    jsonBody = JsonSerializer.Serialize(bodyLaserForecastedCarrierDates);
                    //Set header
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginAccess);
                    //Post http call for sending forcasted carrier dates
                    var response = client.PostAsync(url, encodedContent).Result;
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    r = true;
                    //Log
                    APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
                }
            }
            catch (HttpRequestException e)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int?)(e.StatusCode), responseBody, e.Message, refUtilisateur, dbContext);
                //Création du message à l'usage des utilisateurs
                CreateAPIErrorMessage(cmdF, e, "Erreur d'envoi du transporteur", refUtilisateur, dbContext, configuration);
            }
            //End
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Send forcasted carrier dates
        /// <param cmdF>The related CommandeFournisseur object</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <returns>true if API call success, false if not</returns>
        /// </summary>
        public static bool CommandeFournisseurSendDatesPrevues(CommandeFournisseur cmdF, int refUtilisateur, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            bool r = false;
            string responseBody = "";
            string jsonBody = "";
            string url = configuration["AppResources:Laser:Url"] + "/api/transitions";
            string urlTransaction = configuration["AppResources:Laser:Url"] + "/api/transactions/" + cmdF.RefExt;
            //Process
            HttpClient client = new HttpClient();
            try
            {
                string loginAccess = AuthenticateAsCarrier(ref client, refUtilisateur, cmdF, dbContext, configuration);
                if (!string.IsNullOrWhiteSpace(loginAccess))
                {
                    //Set header
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginAccess);
                    //Check if Dates are already set
                    //Get http call for getting data
                    var response = client.GetAsync(urlTransaction).Result;
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    //Log
                    APIUtils.ApiUtils.LogAPICall(url, null, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
                    var transaction = (LaserTransaction)JsonSerializer.Deserialize(responseBody, typeof(LaserTransaction));
                    //Set or update dates
                    var bodyLaserForecastedCarrierDates = new LaserTransition();
                    if (string.IsNullOrEmpty(transaction.forecastedDeliveryDate) && string.IsNullOrEmpty(transaction.forecastedCollectionDate))
                    {
                        //Set dates
                        bodyLaserForecastedCarrierDates = new LaserTransition()
                        {
                            workflow = "transaction_shipping_status",
                            transitionName = "accept_carrier",
                            transaction = "/api/transactions/" + cmdF.RefExt,
                            additionalInfo = new LaserAdditionalInfo()
                            {
                                forecastedCollectionDate = ((DateTime)cmdF.DChargementPrevue).ToString("yyyy-MM-ddT00:00:00.000Z"),
                                forecastedDeliveryDate = ((DateTime)cmdF.DDechargementPrevue).ToString("yyyy-MM-ddT00:00:00.000Z")
                            }
                        };
                    }
                    else
                    {
                        //Update dates
                        bodyLaserForecastedCarrierDates = new LaserTransition()
                        {
                            workflow = "transaction_shipping_status",
                            transitionName = "update_transaction",
                            transaction = "/api/transactions/" + cmdF.RefExt,
                            additionalInfo = new LaserAdditionalInfo()
                            {
                                forecastedCollectionDate = ((DateTime)cmdF.DChargementPrevue).ToString("yyyy-MM-ddT00:00:00.000Z"),
                                forecastedDeliveryDate = ((DateTime)cmdF.DDechargementPrevue).ToString("yyyy-MM-ddT00:00:00.000Z")
                            },
                            comment = "Modification de la date de collecte planifiée"
                        };
                    }
                    var encodedContent = JsonContent.Create(bodyLaserForecastedCarrierDates, typeof(LaserTransition), null, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                    jsonBody = JsonSerializer.Serialize(bodyLaserForecastedCarrierDates);
                    //Post http call for sending forcasted carrier dates
                    response = client.PostAsync(url, encodedContent).Result;
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    r = true;
                    //Log
                    APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
                }
            }
            catch (HttpRequestException e)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int?)(e.StatusCode), responseBody, e.Message, refUtilisateur, dbContext);
                //Création du message à l'usage des utilisateurs
                CreateAPIErrorMessage(cmdF, e, "Erreur d'envoi des dates prévisionnelles", refUtilisateur, dbContext, configuration);
            }
            //End
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Send destination
        /// <param cmdF>The related CommandeFournisseur object</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <returns>true if API call success, false if not</returns>
        /// </summary>
        public static bool CommandeFournisseurSendDestination(CommandeFournisseur cmdF, int refUtilisateur, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            bool r = false;
            string responseBody = "";
            string jsonBody = "";
            string urlGetLogisticsChain = configuration["AppResources:Laser:Url"] + "/api/logistics_chains?sourceActor.code="
                + cmdF.Entite.CodeEE
                + "&activeDatetimeTo[after]=" + DateTime.Now.ToString("yyyy-MM-dd")
                ;
            if (cmdF.Produit?.LaserType == LaserType.Quality.ToString()) { urlGetLogisticsChain += "&quality.code="; }
            if (cmdF.Produit?.LaserType == LaserType.Flow.ToString()) { urlGetLogisticsChain += "&flow.type="; }
            string url = configuration["AppResources:Laser:Url"] + "/api/transitions";
            //Process
            HttpClient client = new HttpClient();
            try
            {
                string loginAccess = AuthenticateAsCarrier(ref client, refUtilisateur, cmdF, dbContext, configuration);
                if (!string.IsNullOrWhiteSpace(loginAccess))
                {
                    //Search for the logistics chain
                    //Get product code
                    string productCode = cmdF.Produit.Libelle.Split(" - ")[0];
                    urlGetLogisticsChain += productCode;
                    //Set header
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginAccess);
                    //Get http call for getting data
                    var response = client.GetAsync(urlGetLogisticsChain).Result;
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    //Log
                    APIUtils.ApiUtils.LogAPICall(urlGetLogisticsChain, null, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
                    var laserLogisticsChains = (LaserLogisticsChains)JsonSerializer.Deserialize(responseBody, typeof(LaserLogisticsChains));
                    //Exit if no client found
                    LaserLogisticsChain found = null;
                    try
                    {
                        if (laserLogisticsChains?.logistiqChains == null) { throw new HttpRequestException("Recycleur non trouvé"); }
                        dbContext.Entry(cmdF.AdresseClient).Reference(b => b.Entite).Load();
                        found = laserLogisticsChains?.logistiqChains?.Where(w => w.destinationActor.code.Trim() == cmdF.AdresseClient.Entite.CodeEE.Trim()).FirstOrDefault();
                    }
                    catch { }
                    //Exit if no client found
                    if (found == null) { throw new HttpRequestException("Recycleur non trouvé"); }
                    //Update logistics chain
                    var bodyLaserTransition = new LaserTransition()
                    {
                        workflow = "transaction_shipping_status",
                        transitionName = "update_transaction_logistics_chain",
                        transaction = "/api/transactions/" + cmdF.RefExt,
                        additionalInfo = new LaserAdditionalInfo()
                        {
                            logisticsChain = found.id.ToString()
                        }
                    };
                    var encodedContent = JsonContent.Create(bodyLaserTransition, typeof(LaserTransition), null, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                    jsonBody = JsonSerializer.Serialize(bodyLaserTransition);
                    //Post http call for sending forcasted carrier dates
                    response = client.PostAsync(url, encodedContent).Result;
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    r = true;
                    //Log
                    APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
                }
            }
            catch (HttpRequestException e)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int?)(e.StatusCode), responseBody, e.Message, refUtilisateur, dbContext);
                //Création du message à l'usage des utilisateurs
                CreateAPIErrorMessage(cmdF, e, "Erreur d'envoi du recycleur", refUtilisateur, dbContext, configuration);
            }
            //End
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Send carrier dates
        /// <param cmdF>The related CommandeFournisseur object</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <returns>true if API call success, false if not</returns>
        /// </summary>
        public static bool CommandeFournisseurSendDatesReelles(CommandeFournisseur cmdF, int refUtilisateur, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            bool r = false;
            string responseBody = "";
            string jsonBody = "";
            string url = configuration["AppResources:Laser:Url"] + "/api/transitions";
            //Process
            HttpClient client = new HttpClient();
            try
            {
                string loginAccess = AuthenticateAsCarrier(ref client, refUtilisateur, cmdF, dbContext, configuration);
                if (!string.IsNullOrWhiteSpace(loginAccess))
                {
                    var bodyLaserForecastedCarrierDates = new LaserTransition()
                    {
                        workflow = "transaction_shipping_status",
                        transitionName = "document_carrier",
                        transaction = "/api/transactions/" + cmdF.RefExt,
                        additionalInfo = new LaserAdditionalInfo()
                        {
                            effectiveCollectionDate = ((DateTime)cmdF.DChargement).ToString("yyyy-MM-ddT00:00:00.000Z"),
                            effectiveDeliveryDate = ((DateTime)cmdF.DDechargement).ToString("yyyy-MM-ddT00:00:00.000Z"),
                            transactionComment = new LaserTransactionComment()
                            {
                                comment = cmdF.CmtTransporteur,
                                transaction = "/api/transactions/" + cmdF.RefExt,
                                type = "carrierActor"
                            }
                        }
                    };
                    var encodedContent = JsonContent.Create(bodyLaserForecastedCarrierDates, typeof(LaserTransition), null, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                    jsonBody = JsonSerializer.Serialize(bodyLaserForecastedCarrierDates);
                    //Set header
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginAccess);
                    //Post http call for sending forcasted carrier dates
                    var response = client.PostAsync(url, encodedContent).Result;
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    r = true;
                    //Log
                    APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
                }
            }
            catch (HttpRequestException e)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int?)(e.StatusCode), responseBody, e.Message, refUtilisateur, dbContext);
                //Création du message à l'usage des utilisateurs
                CreateAPIErrorMessage(cmdF, e, "Erreur d'envoi des dates réelles", refUtilisateur, dbContext, configuration);
            }
            //End
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Send reception data
        /// <param cmdF>The related CommandeFournisseur object</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <returns>true if API call success, false if not</returns>
        /// </summary>
        public static bool CommandeFournisseurSendReception(CommandeFournisseur cmdF, int refUtilisateur, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            bool r = false;
            string responseBody = "";
            string jsonBody = "";
            string url = configuration["AppResources:Laser:Url"] + "/api/transitions";
            //Process
            HttpClient client = new HttpClient();
            try
            {
                string loginAccess = AuthenticateAsCarrier(ref client, refUtilisateur, cmdF, dbContext, configuration);
                if (!string.IsNullOrWhiteSpace(loginAccess))
                {
                    var bodyLaserForecastedCarrierDates = new LaserTransition()
                    {
                        workflow = "transaction_receipt_status",
                        transitionName = "receipt_accept_destination",
                        transaction = "/api/transactions/" + cmdF.RefExt,
                        additionalInfo = new LaserAdditionalInfo()
                        {
                            effectiveQuantity = cmdF.PoidsDechargement,
                            effectiveBalesAmount = cmdF.NbBalleDechargement,
                            receiptDate = ((DateTime)cmdF.DDechargement).ToString("yyyy-MM-ddT00:00:00.000Z"),
                            transactionComment = new LaserTransactionComment()
                            {
                                comment = cmdF.CmtClient,
                                transaction = "/api/transactions/" + cmdF.RefExt,
                                type = "destinationActor"
                            }
                        }
                    };
                    var encodedContent = JsonContent.Create(bodyLaserForecastedCarrierDates, typeof(LaserTransition), null, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                    jsonBody = JsonSerializer.Serialize(bodyLaserForecastedCarrierDates);
                    //Set header
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginAccess);
                    //Post http call for sending forcasted carrier dates
                    var response = client.PostAsync(url, encodedContent).Result;
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    r = true;
                    //Log
                    APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
                }
            }
            catch (HttpRequestException e)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(url, jsonBody, (int?)(e.StatusCode), responseBody, e.Message, refUtilisateur, dbContext);
                //Création du message à l'usage des utilisateurs
                CreateAPIErrorMessage(cmdF, e, "Erreur d'envoi des poids et nombre de balle réels", refUtilisateur, dbContext, configuration);
            }
            //End
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Send reception data
        /// <param laserTransaction>The laserTransaction object to get data from</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <returns>true if API call success, false if not</returns>
        /// </summary>
        public static bool CommandeFournisseurGetData(string refExt, ref LaserTransaction laserTransaction, int refUtilisateur, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            bool r = false;
            string responseBody = "";
            string url = configuration["AppResources:Laser:Url"] + "/api/transactions/" + refExt;
            CommandeFournisseur cmdF = new CommandeFournisseur() { RefExt = refExt };
            string refCmd = (cmdF.NumeroCommande <= 0 ? "NA" : cmdF.NumeroCommande.ToString()) + " (" + (cmdF?.RefExt ?? "NA") + "-" + (cmdF?.LibExt ?? "NA") + ")";
            //Process
            HttpClient client = new HttpClient();
            try
            {
                string loginAccess = AuthenticateAsCarrier(ref client, refUtilisateur, cmdF, dbContext, configuration);
                if (!string.IsNullOrWhiteSpace(loginAccess))
                {
                    //Set header
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginAccess);
                    //Get http call for getting data
                    var response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    laserTransaction = (LaserTransaction)JsonSerializer.Deserialize(responseBody, typeof(LaserTransaction));
                    r = true;
                    //Log
                    APIUtils.ApiUtils.LogAPICall(url, null, (int?)response.StatusCode, responseBody, null, refUtilisateur, dbContext);
                }
            }
            catch (HttpRequestException e)
            {
                //Log
                APIUtils.ApiUtils.LogAPICall(url, null, (int?)(e.StatusCode), responseBody, refCmd + " - " + e.Message, refUtilisateur, dbContext);
                //Création du message à l'usage des utilisateurs
                CreateAPIErrorMessage(cmdF, e, "Erreur de réception des données de demande d\'enlèvement " + refCmd, refUtilisateur, dbContext, configuration);
            }
            //End
            return r;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates message for users in case of error
        /// <param cmdF>The related CommandeFournisseur object</param>
        /// <param refUtilisateur>The id of the current user </param>
        /// <param dbContext>The current ApplicationDbContext</param>
        /// <param name="configuration">The current IConfiguration</param>
        /// <returns>true if success, false if not</returns>
        /// </summary>
        public static bool CreateAPIErrorMessage(CommandeFournisseur cmdF, HttpRequestException e, string message, int refUtilisateur
            , ApplicationDbContext dbContext, IConfiguration configuration)
        {
            bool r = false;
            try
            {
                string corps, corpsHTML;
                if (cmdF != null)
                {
                    corps = "Une erreur d'API s'est produite le " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                        + " concernant la demande d'enlèvement "
                        + (Utils.FormatNumeroCommande(cmdF?.NumeroCommande.ToString()) ?? "NA") + " (Réf. ext. " + (cmdF?.LibExt ?? "NA") + ", Id ext. " + (cmdF?.RefExt ?? "NA") + ")"
                        + Environment.NewLine + message
                        + Environment.NewLine + e.Message
                        + Environment.NewLine + e.StackTrace;
                    corpsHTML = HttpUtility.HtmlEncode("Une erreur d'API s'est produite le " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " concernant la demande d'enlèvement ")
                        + "<a href=\"/*appLink=commande-fournisseur/" + cmdF.RefCommandeFournisseur.ToString() + "\">" + (Utils.FormatNumeroCommande(cmdF?.NumeroCommande.ToString()) ?? "NA") + " (Réf. ext. " + (cmdF?.LibExt ?? "NA") + ", Id ext. " + (cmdF?.RefExt ?? "NA") + ")" + "</a>"
                        + "<BR/>" + HttpUtility.HtmlEncode(message)
                        + "<BR/>" + HttpUtility.HtmlEncode(e.Message)
                        + "<BR/>" + HttpUtility.HtmlEncode(e.StackTrace);
                }
                else
                {
                    corps = "Une erreur d'API s'est produite à " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                        + Environment.NewLine + message
                        + Environment.NewLine + e.Message
                        + Environment.NewLine + e.StackTrace;
                    corpsHTML = HttpUtility.HtmlEncode(corps).Replace(Environment.NewLine, "<BR/>");
                }
                corpsHTML = Utils.FormatHTMLNewLines(System.Net.WebUtility.HtmlEncode(corps));
                corpsHTML = "<span style=\"font-family:Roboto; font-size:11pt;\">" + corpsHTML + "</span>";
                //Process
                var msg = new Message()
                {
                    RefUtilisateurCourant = refUtilisateur,
                    RefMessageType = 4,
                    Libelle = "Erreur API externe",
                    Titre = "Erreur API externe",
                    Corps = corps,
                    CorpsHTML = corpsHTML,
                    DiffusionUnique = true,
                    DDebut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    DFin = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).AddDays(7),
                    Actif = true,
                    Important = true
                };
                dbContext.Messages.Add(msg);
                //Manage Diffusion
                msg.MessageDiffusions = new HashSet<MessageDiffusion>();
                msg.MessageDiffusions.Add(new MessageDiffusion()
                {
                    RefModule = Enumerations.Module.Logistique.ToString(),
                    RefHabilitation = Enumerations.HabilitationLogistique.Administrateur.ToString()
                });
                r = true;
            }
            catch (HttpRequestException ex)
            {

                string txt = "Erreur création de message erreur API: "
                    + Environment.NewLine + ex.Message
                    + Environment.NewLine + ex.StackTrace;
                txt = txt.Substring(0, 800);
                Utils.DebugPrint("Erreur création de message erreur API: " + Environment.NewLine + ex.StackTrace,
                    configuration["Data:DefaultConnection:ConnectionString"]);
            }

            //End
            return r;
        }
    }
}