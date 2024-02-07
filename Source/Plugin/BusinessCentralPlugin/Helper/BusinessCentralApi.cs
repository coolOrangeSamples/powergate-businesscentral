using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BusinessCentralPlugin.BusinessCentral;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using Attribute = BusinessCentralPlugin.BusinessCentral.Attribute;
using AttributeDefinition = BusinessCentralPlugin.BusinessCentral.AttributeDefinition;
using BusinessCentral_Document = BusinessCentralPlugin.BusinessCentral.Document;
using Company = BusinessCentralPlugin.BusinessCentral.Company;
using ItemCard = BusinessCentralPlugin.BusinessCentral.ItemCard;
using ItemCardMin = BusinessCentralPlugin.BusinessCentral.ItemCardMin;
using ItemMin = BusinessCentralPlugin.BusinessCentral.ItemMin;
using Link = BusinessCentralPlugin.BusinessCentral.Link;
using Lookup = BusinessCentralPlugin.BusinessCentral.Lookup;
using ProdBOMLine = BusinessCentralPlugin.BusinessCentral.ProdBOMLine;
using ProdBOMLineMin = BusinessCentralPlugin.BusinessCentral.ProdBOMLineMin;
using ProductionBOM = BusinessCentralPlugin.BusinessCentral.ProductionBOM;
using ProductionBOMMin = BusinessCentralPlugin.BusinessCentral.ProductionBOMMin;
using RoutingLink = BusinessCentralPlugin.BusinessCentral.RoutingLink;
using Vendor = BusinessCentralPlugin.BusinessCentral.Vendor;

namespace BusinessCentralPlugin.Helper
{
    public class BusinessCentralApi
    {
        private static BusinessCentralApi _instance;
        public static BusinessCentralApi Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BusinessCentralApi();

                return _instance;
            }
        }

        private static readonly string Company;

        static BusinessCentralApi()
        {
            Company = Configuration.Company;
        }

        private static readonly HttpClient HttpClient = new HttpClient();
        private static RestClientWithLogging _restClient;
        //private static RestClient _restClient;

        private static RestClientWithLogging GetRestClient()
        {
            if (_restClient == null)
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                ServicePointManager.DefaultConnectionLimit = 20;

                var options = new RestClientOptions(Configuration.BaseUrl) { MaxTimeout = -1  };
                var client = new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson());

                _restClient = new RestClientWithLogging(client);
                //_restClient = client;
            }

            return _restClient;
        }

        private static RestRequest GetRestRequest(string url, Method method = Method.Get)
        {
            var token = GetToken();
            var request = new RestRequest(url, method);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"{token.TokenType} {token.AccessToken}");

            return request;
        }

        private async Task<string> GetResourceAsBase64Async(string url)
        {
            var token = GetToken();
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
            var bytes = await HttpClient.GetByteArrayAsync(url);
            var base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        private async Task<byte[]> GetResourceAsByteArray(string url)
        {
            var sp = ServicePointManager.FindServicePoint(new Uri(url));
            sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute

            var token = GetToken();
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
            var bytes = await HttpClient.GetByteArrayAsync(url);
            return bytes;
        }

        private static Token GetToken()
        {
            if (Configuration.AuthType == "OAuth")
            {
                return MicrosoftOAuth.GetToken(
                    Configuration.TenantId,
                    Configuration.ClientId,
                    Configuration.ClientSecret);
            }
            else if (Configuration.AuthType == "Basic")
            {
                var authenticationString = $"{Configuration.Username}:{Configuration.Password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
                return new Token { AccessToken = base64EncodedAuthenticationString, TokenType = "Basic" };
            }
            else { return null; }
        }

        //public async Task<string> GetMetadata()
        //{
        //    var request = GetRestRequest($"/$metadata#Company('{Company}')");
        //    request.AddHeader("Accept", "application/json");

        //    var response = await GetRestClient().ExecuteAsync(request);
        //    return Encoding.Default.GetString(response.RawBytes);
        //}

        #region Lookups
        // API Company
        public async Task<List<Company>> GetCompanies()
        {
            var request = GetRestRequest($"/Company?$select=Name,Id");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<Company>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        // Page 30010: APIV2 - Vendors
        public async Task<List<Vendor>> GetVendors()
        {
            var request = GetRestRequest($"/Company('{Company}')/Vendors?$select=id,number,displayName");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<Vendor>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        // Page 30025: APIV2 - Item Categories
        public async Task<List<Lookup>> GetItemCategories()
        {
            var request = GetRestRequest($"/Company('{Company}')/ItemCategories?$select=id,code");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<Lookup>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        // Page 30030: APIV2 - Units of Measure
        public async Task<List<Lookup>> GetUnitsOfMeasures()
        {
            var request = GetRestRequest($"/Company('{Company}')/UnitsOfMeasures?$select=id,code");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<Lookup>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        // Page 30096: APIV2 - Inventory Post. Group
        public async Task<List<Lookup>> GetInventoryPostingGroups()
        {
            var request = GetRestRequest($"/Company('{Company}')/InventoryPostingGroups?$select=id,code");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<Lookup>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        // Page 30079: APIV2 - Gen. Prod. Post. Group
        public async Task<List<Lookup>> GetGeneralProductPostingGroups()
        {
            var request = GetRestRequest($"/Company('{Company}')/GeneralProductPostingGroups?$select=id,code");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<Lookup>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        // Page 7500: Item Attributes
        public async Task<List<AttributeDefinition>> GetItemAttributeDefinitions()
        {
            var request = GetRestRequest($"/Company('{Company}')/ItemAttributes?$select=ID,Name,Type,Blocked");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<AttributeDefinition>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        // Page 99000798: Routing Links
        public async Task<List<RoutingLink>> GetRoutingLinks()
        {
            var request = GetRestRequest($"/Company('{Company}')/RoutingLinks");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<RoutingLink>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }
        #endregion

        #region Items
        // Page 30008: APIV2 - Items
        public async Task<ItemMin> GetItemMin(string number)
        {
            var request = GetRestRequest($"/Company('{Company}')/Items?$filter=number eq '{number}'&$select=id,number");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<ItemMin>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value.FirstOrDefault();
        }

        // Page 30008: APIV2 - Items
        private async Task<ItemMin> GetItemMinWithPicture(string number)
        {
            var request = GetRestRequest($"/Company('{Company}')/Items?$filter=number eq '{number}'&$expand=Itemspicture($select=id,pictureContent)&$select=id,number");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<ItemMin>>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data.Value?.FirstOrDefault();
        }

        // Page 30: Item Card
        public async Task<List<ItemCard>> GetItemCards()
        {
            var request = GetRestRequest($"/Company('{Company}')/ItemCards?$select=No,Description,Blocked,Base_Unit_of_Measure,Net_Weight,Unit_Price,Inventory,Gen_Prod_Posting_Group,Vendor_No");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<ItemCard>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        // Page 30: Item Card
        public async Task<ItemCard> GetItemCard(string number)
        {
            var request = GetRestRequest($"/Company('{Company}')/ItemCards('{number}')?$select=No,Description,Blocked,Type,Base_Unit_of_Measure,Net_Weight,Unit_Price,Inventory,Gen_Prod_Posting_Group,Vendor_No");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ItemCard>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }

        // Page 30: Item Card
        public async Task<ItemCardMin> GetItemCardMin(string number)
        {
            var request = GetRestRequest($"/Company('{Company}')/ItemCards('{number}')?$select=No,Description");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ItemCardMin>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public async Task<string> GetItemPicture(string number)
        {
            var itemMin = await GetItemMinWithPicture(number);
            if (itemMin.Itemspicture?.pictureContentodatamediaReadLink != null)
            {
                try
                {
                    return Task.Run(() => GetResourceAsBase64Async(itemMin.Itemspicture.pictureContentodatamediaReadLink)).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            return null;
        }

        // Page 30008: APIV2 - Items
        public async Task SetItemPicture(string number, string thumbnail)
        {
            if (string.IsNullOrEmpty(thumbnail))
                return;

            var itemMin = await GetItemMinWithPicture(number);

            var request = GetRestRequest($"/Company('{Company}')/Items({itemMin.id})/Itemspicture/pictureContent", Method.Patch);
            request.AddHeader("If-Match", itemMin.Itemspicture.odataetag);
            request.AddParameter("application/octet-stream", Convert.FromBase64String(thumbnail), ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        // Page 30: Item Card
        public async Task<ItemCard> CreateItemCard(ItemCard itemCard)
        {
            var request = GetRestRequest($"/Company('{Company}')/ItemCards", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    itemCard.No,
                    itemCard.Description,
                    Blocked = false,
                    Type = "Inventory",
                    itemCard.Base_Unit_of_Measure,
                    itemCard.Net_Weight,
                    Inventory_Posting_Group = Configuration.DefaultInventoryPostingGroup,
                    Item_Category_Code = Configuration.DefaultItemCategoryCode,
                    Gen_Prod_Posting_Group = Configuration.DefaultGeneralProductPostingGroup
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<ItemCard>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        // Page 30: Item Card
        public async Task<ItemCard> UpdateItemCard(ItemCard itemCard)
        {
            var itemCardMin = await GetItemCardMin(itemCard.No);

            var request = GetRestRequest($"/Company('{Company}')/ItemCards('{itemCard.No}')", Method.Patch);
            request.AddHeader("If-Match", itemCardMin.odataetag);
            var json = JsonConvert.SerializeObject(
                new
                {
                    itemCard.No,
                    itemCard.Description,
                    itemCard.Base_Unit_of_Measure,
                    itemCard.Net_Weight
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<ItemCard>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        // Page 30: Item Card
        public async Task<ItemCard> UpdateItemCardProductionBom(string number)
        {
            var itemCardMin = await GetItemCardMin(number);

            var request = GetRestRequest($"/Company('{Company}')/ItemCards('{itemCardMin.No}')", Method.Patch);
            request.AddHeader("If-Match", itemCardMin.odataetag);
            var json = JsonConvert.SerializeObject(
                new
                {
                    Production_BOM_No = number
                    //Replenishment_System = "Assembly"
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<ItemCard>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        #endregion

        #region Links and Attributes (CodeUnits from coolOrange App)
        // CodeUnit 50150: ItemRecordLinks
        public async Task<List<Link>> GetItemLinks(string itemNumber)
        {
            var request = GetRestRequest($"/ItemRecordLinks_GetLinks?company={Company}", Method.Post);
            var json = JsonConvert.SerializeObject(
                new { itemNumber },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<dynamic>(request);
            //if (!response.IsSuccessful)
            //    throw new Exception(response.ErrorMessage);

            string result = response.Data.value;
            if (result == null)
                return new List<Link>();

            return JsonConvert.DeserializeObject<List<Link>>(result);
        }

        // CodeUnit 50150: ItemRecordLinks
        public async Task<List<Link>> GetItemLinks()
        {
            var request = GetRestRequest($"/ItemRecordLinks_GetAllLinks?company={Company}", Method.Post);

            var response = await GetRestClient().ExecuteAsync<dynamic>(request);
            //if (!response.IsSuccessful)
            //    throw new Exception(response.ErrorMessage);

            string result = response.Data.value;
            if (result == null)
                return new List<Link>();

            return JsonConvert.DeserializeObject<List<Link>>(result);
        }

        // CodeUnit 50150: ItemRecordLinks
        public async Task SetItemLink(string itemNumber, string url, string description)
        {
            if (string.IsNullOrEmpty(url))
                return;

            var request = GetRestRequest($"/ItemRecordLinks_SetLink?company={Company}", Method.Post);
            var json = JsonConvert.SerializeObject(
                new { itemNumber, url, description },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        // CodeUnit 50149: ItemAttributes
        public async Task<List<Attribute>> GetItemAttributes(string itemNumber)
        {
            var request = GetRestRequest($"/ItemAttributes_GetItemAttributes?company={Company}", Method.Post);
            var json = JsonConvert.SerializeObject(
                new { itemNumber },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<dynamic>(request);
            //if (!response.IsSuccessful)
            //    throw new Exception(response.ErrorMessage);

            string result = response.Data.value;
            if (result == null)
                return new List<Attribute>();

            return JsonConvert.DeserializeObject<List<Attribute>>(result);
        }

        // CodeUnit 50149: ItemAttributes
        public async Task<List<Attribute>> GetItemAttributes()
        {
            var request = GetRestRequest($"/ItemAttributes_GetAllItemAttributes?company={Company}", Method.Post);

            var response = await GetRestClient().ExecuteAsync<dynamic>(request);
            //if (!response.IsSuccessful)
            //    throw new Exception(response.ErrorMessage);

            string result = response.Data.value;
            if (result == null)
                return new List<Attribute>();

            return JsonConvert.DeserializeObject<List<Attribute>>(result);
        }

        // CodeUnit 50149: ItemAttributes
        public async Task SetItemAttribute(string itemNumber, string attributeName, string attributeValue)
        {
            if (string.IsNullOrEmpty(attributeValue))
                attributeValue = string.Empty;

            var request = GetRestRequest($"/ItemAttributes_SetItemAttribute?company={Company}", Method.Post);
            var json = JsonConvert.SerializeObject(
                new { itemNumber, attributeName, attributeValue },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }
        #endregion

        #region BOMs
        // Page 99000786: Production BOM
        public async Task<ProductionBOMMin> GetBomHeaderMin(string number)
        {
            var request = GetRestRequest($"/Company('{Company}')/ProductionBOMs('{number}')?$select=No");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ProductionBOMMin>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        // Page 99000786: Production BOM
        public async Task<ProductionBOM> GetBomHeaderAndRows(string number)
        {
            var request = GetRestRequest($"/Company('{Company}')/ProductionBOMs('{number}')?$expand=ProductionBOMsProdBOMLine($select=Production_BOM_No,Line_No,No,Description,Quantity_per,Unit_of_Measure_Code,Routing_Link_Code)&$select=No,Description,Unit_of_Measure_Code");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ProductionBOM>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }

        // Page 99000786: Production BOM
        public async Task<ProductionBOM> CreateBomHeader(ProductionBOM bomHeader)
        {
            var request = GetRestRequest($"/Company('{Company}')/ProductionBOMs", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    bomHeader.No,
                    bomHeader.Description,
                    bomHeader.Unit_of_Measure_Code,
                    Status = "New"
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<ProductionBOM>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        // Page 99000786: Production BOM
        public async Task<ProductionBOM> UpdateBomHeader(ProductionBOM bomHeader)
        {
            var bomHeaderMin = await GetBomHeaderMin(bomHeader.No);

            var request = GetRestRequest($"/Company('{Company}')/ProductionBOMs('{bomHeader.No}')", Method.Patch);
            request.AddHeader("If-Match", bomHeaderMin.odataetag);
            var json = JsonConvert.SerializeObject(
                new
                {
                    bomHeader.No,
                    bomHeader.Description,
                    bomHeader.Unit_of_Measure_Code
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<ProductionBOM>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        // Page 99000788: Lines
        public async Task<ProdBOMLineMin> GetBomRowMin(string parentNumber, int position, string childNumber)
        {
            var request = GetRestRequest($"/Company('{Company}')/ProductionBOMLines('{parentNumber}','',{position})?$filter=No eq '{childNumber}'&$select=Production_BOM_No,Line_No,No");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ProdBOMLineMin>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        // Page 99000788: Lines
        public async Task<ProdBOMLine> GetBomRow(string parentNumber, int position, string childNumber)
        {
            var request = GetRestRequest($"/Company('{Company}')/ProductionBOMLines('{parentNumber}','',{position})?$filter=No eq '{childNumber}'$select=Production_BOM_No,Line_No,No,Description,Quantity_per,Unit_of_Measure_Code,Routing_Link_Code");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ProdBOMLine>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }

        // Page 99000788: Lines
        public async Task<ProdBOMLine> CreateBomRow(ProdBOMLine bomRow)
        {
            var request = GetRestRequest($"/Company('{Company}')/ProductionBOMLines", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    bomRow.Production_BOM_No,
                    bomRow.Line_No,
                    Type = "Item",
                    bomRow.No,
                    bomRow.Description,
                    bomRow.Quantity_per,
                    bomRow.Unit_of_Measure_Code,
                    bomRow.Routing_Link_Code
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<ProdBOMLine>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        // Page 99000788: Lines
        public async Task<ProdBOMLine> UpdateBomRow(ProdBOMLine bomRow)
        {
            var bomRowMin = await GetBomRowMin(bomRow.Production_BOM_No, bomRow.Line_No, bomRow.No);

            var request = GetRestRequest($"/Company('{Company}')/ProductionBOMLines('{bomRow.Production_BOM_No}','',{bomRow.Line_No})", Method.Patch);
            request.AddHeader("If-Match", bomRowMin.odataetag);
            var json = JsonConvert.SerializeObject(
                new
                {
                    bomRow.Production_BOM_No,
                    bomRow.Line_No,
                    Type = "Item",
                    bomRow.No,
                    bomRow.Description,
                    bomRow.Quantity_per,
                    bomRow.Unit_of_Measure_Code,
                    bomRow.Routing_Link_Code
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<ProdBOMLine>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        // Page 99000788: Lines
        public async Task DeleteBomRow(ProdBOMLine bomRow)
        {
            var bomRowMin = await GetBomRowMin(bomRow.Production_BOM_No, bomRow.Line_No, bomRow.No);

            var request = GetRestRequest($"/Company('{Company}')/ProductionBOMLines('{bomRow.Production_BOM_No}','',{bomRow.Line_No})", Method.Delete);
            request.AddHeader("If-Match", bomRowMin.odataetag);

            var response = await GetRestClient().ExecuteAsync(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }
        #endregion

        #region Documents
        // Page 30008: APIV2 - Items
        public async Task<List<BusinessCentral_Document>> GetDocuments(string number)
        {
            var bcItemMin = await GetItemMin(number);
            if (bcItemMin == null)
                return null;

            var request = GetRestRequest($"/Company('{Company}')/Items({bcItemMin.id})/ItemsdocumentAttachments");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<BusinessCentral_Document>>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data.Value;
        }

        // Page 30080: APIV2 - Document Attachments
        public async Task<BusinessCentral_Document> CreateDocument(string number, string fileName)
        {
            var documents = await GetDocuments(number);
            var exitingDocument = documents.SingleOrDefault(d => d.fileName.Equals(fileName));
            if (exitingDocument != null)
                return exitingDocument;

            var bcItemMin = await GetItemMin(number);

            var request = GetRestRequest($"/Company('{Company}')/DocumentAttachments", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    fileName,
                    parentType = "Item",
                    parentId = bcItemMin.id,
                    lineNumber = documents.Count
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = await GetRestClient().ExecuteAsync<BusinessCentral_Document>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        // Page 30008: APIV2 - Items
        public async Task<byte[]> DownloadDocument(string number, string fileName)
        {
            var bcItemMin = await GetItemMin(number);
            if (bcItemMin == null)
                return null;

            var request = GetRestRequest($"/Company('{Company}')/Items({bcItemMin.id})/ItemsdocumentAttachments");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<BusinessCentral_Document>>(request);
            if (!response.IsSuccessful)
                return null;

            var documentAttachments = response.Data.Value;
            var documentAttachment = documentAttachments.FirstOrDefault(d => d.fileName.Equals(fileName));
            if (documentAttachment == null)
                return null;

            if (documentAttachment.attachmentContentodatamediaReadLink != null)
            {
                try
                {
                    var task = GetResourceAsByteArray(documentAttachment.attachmentContentodatamediaReadLink);
                    return task.Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            return null;
        }

        // Page 30008: APIV2 - Items
        // Page 30080: APIV2 - Document Attachments
        public async Task UploadDocument(string number, string fileName, byte[] bytes)
        {
            var bcItemMin = await GetItemMin(number);
            if (bcItemMin == null)
                return;

            var request = GetRestRequest($"/Company('{Company}')/Items({bcItemMin.id})/ItemsdocumentAttachments");
            request.AddHeader("Accept", "application/json");

            var response = await GetRestClient().ExecuteAsync<ODataResponse<BusinessCentral_Document>>(request);
            if (!response.IsSuccessful)
                return;

            var documentAttachments = response.Data.Value;
            var documentAttachment = documentAttachments.FirstOrDefault(d => d.fileName.Equals(fileName));
            if (documentAttachment == null)
                return;

            var uploadRequest = GetRestRequest($"/Company('{Company}')/DocumentAttachments({documentAttachment.id})/attachmentContent", Method.Patch);
            uploadRequest.AddHeader("If-Match", documentAttachment.odataetag);
            uploadRequest.AddHeader("Content-Type", "application/pdf");
            uploadRequest.AddParameter("application/pdf", bytes,
                ParameterType.RequestBody);

            var uploadResponse = await GetRestClient().ExecuteAsync(uploadRequest);
            if (!uploadResponse.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }
        #endregion
    }
}