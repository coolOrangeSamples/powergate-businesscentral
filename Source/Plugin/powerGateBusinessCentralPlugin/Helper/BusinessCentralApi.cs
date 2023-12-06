using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using powerGateBusinessCentralPlugin.BC;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;

namespace powerGateBusinessCentralPlugin.Helper
{
    public class BusinessCentralApi
    {
        private static BusinessCentralApi _instance;
        public static BusinessCentralApi Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BusinessCentralApi();

                    var maxAttempts = 10;
                    var success = false;

                    for (int i = 0; i < maxAttempts; i++)
                    {
                        if (CheckConnection())
                        {
                            success = true;
                            break;
                        }
                        System.Threading.Thread.Sleep(2000);
                    }

                    if (!success)
                    {
                        throw new ApplicationException("Cannot connect to Business Central! Check the settings and restart powerGate Server");
                    }

                    WebService.InitializeBusinessCentral();

                }
                return _instance;
            }
        }

        public static string BaseUrl { get; }
        public static string Company { get; }
        public static string Username { get; }
        public static string Password { get; }

        static BusinessCentralApi()
        {
            BaseUrl = WebService.Config.BaseUrl.TrimEnd('/');
            Company = WebService.Config.Company;
            Username = WebService.Config.Username;
            Password = WebService.Config.Password;
        }

        private static RestClient GetRestClient()
        {
            return new RestClient(new RestClientOptions(BaseUrl)
                {
                    MaxTimeout = -1,
                    Authenticator = new HttpBasicAuthenticator(Username, Password)
                },
                configureSerialization: s => s.UseNewtonsoftJson());
        }

        public static bool CheckConnection()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<dynamic>>(request);
            return response.IsSuccessful && response.Data.Value?.Count > 0;
        }

        #region Lookups

        public List<Vendor> GetVendors()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/Vendors?$select=id,number,displayName");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<Vendor>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        public List<Lookup> GetItemCategories()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ItemCategories?$select=id,code");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<Lookup>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        public List<Lookup> GetUnitsOfMeasures()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/UnitsOfMeasures?$select=id,code");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<Lookup>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        public List<Lookup> GetInventoryPostingGroups()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/InventoryPostingGroups?$select=id,code");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<Lookup>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        public List<Lookup> GetGeneralProductPostingGroups()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/GeneralProductPostingGroups?$select=id,code");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<Lookup>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        public List<ItemAttribute> GetItemAttributesDefinitions()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ItemAttributes");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<ItemAttribute>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        #endregion

        #region Items

        public List<BC.Item> GetItems()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/Items?$expand=Itemspicture,ItemsdocumentAttachments");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<BC.Item>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        public BC.ItemMin GetItemMin(string number)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/Items?$filter=number eq '{number}'&$select=id,number");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<BC.ODataResponse<BC.ItemMin>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value.FirstOrDefault();
        }

        public BC.Item GetItem(string number)
        {
            var client = GetRestClient();

            var request =
                new RestRequest(
                    $"/Company('{Company}')/Items?$filter=number eq '{number}'&$expand=Itemspicture");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<BC.Item>>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data.Value?.FirstOrDefault();
        }

        public List<ItemCard> GetItemCards()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ItemCards");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<ItemCard>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data.Value;
        }

        public ItemCard GetItemCard(string number)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ItemCards('{number}')");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ItemCard>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }

        public ItemCardMin GetItemCardMin(string number)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ItemCards('{number}')?$select=No,Description");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ItemCardMin>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public string GetItemPicture(BC.Item item)
        {
            if (item.Itemspicture?.pictureContentodatamediaReadLink != null)
                try
                {
                    var task = GetImageAsBase64Async(item.Itemspicture.pictureContentodatamediaReadLink);
                    return task.Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }

            return null;
        }

        private async Task<string> GetImageAsBase64Async(string url)
        {
            using (var client = new HttpClient())
            {
                var authenticationString = $"{Username}:{Password}";
                var base64EncodedAuthenticationString =
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                var bytes = await client.GetByteArrayAsync(url);
                var base64String = Convert.ToBase64String(bytes);
                return base64String;
            }
        }

        public void SetItemPicture(string number, string thumbnail)
        {
            if (string.IsNullOrEmpty(thumbnail))
                return;

            var client = GetRestClient();

            var item = GetItem(number);
            var request = new RestRequest($"/Company('{Company}')/Items({item.id})/Itemspicture/pictureContent",
                Method.Patch);
            request.AddHeader("If-Match", item.Itemspicture.odataetag);
            request.AddParameter("application/octet-stream", Convert.FromBase64String(thumbnail),
                ParameterType.RequestBody);

            var response = client.Execute(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        public List<Link> GetItemLinks(string itemNumber)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/ItemRecordLinks_GetLinks?company={Company}", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    itemNumber
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute<dynamic>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            string result = response.Data.value;
            if (result == null)
                return new List<Link>();

            return JsonConvert.DeserializeObject<List<Link>>(result);
        }

        public List<Link> GetItemLinks()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/ItemRecordLinks_GetAllLinks?company={Company}", Method.Post);

            var response = client.Execute<dynamic>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            string result = response.Data.value;
            if (result == null)
                return new List<Link>();

            return JsonConvert.DeserializeObject<List<Link>>(result);
        }

        public void SetItemLink(string itemNumber, string url, string description)
        {
            if (string.IsNullOrEmpty(url))
                return;

            var client = GetRestClient();

            var request = new RestRequest($"/ItemRecordLinks_SetLink?company={Company}", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    itemNumber,
                    url,
                    description
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        //public string GetItemAttributeValue(string itemNumber, string attributeName)
        //{
        //    var client = GetRestClient();

        //    var request = new RestRequest($"/ItemAttributes_GetItemAttributeValue?company={Company}", Method.Post);
        //    var json = JsonConvert.SerializeObject(
        //        new
        //        {
        //            itemNumber,
        //            attributeName
        //        },
        //        Formatting.None,
        //        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    request.AddParameter("application/json", json, ParameterType.RequestBody);

        //    var response = client.Execute<dynamic>(request);
        //    if (!response.IsSuccessful)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data.value;
        //}

        public List<BC.Attribute> GetItemAttributes(string itemNumber)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/ItemAttributes_GetItemAttributes?company={Company}", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    itemNumber
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute<dynamic>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            string result = response.Data.value;
            if (result == null)
                return new List<BC.Attribute>();

            return JsonConvert.DeserializeObject<List<BC.Attribute>>(result);
        }

        public List<BC.Attribute> GetItemAttributes()
        {
            var client = GetRestClient();

            var request = new RestRequest($"/ItemAttributes_GetAllItemAttributes?company={Company}", Method.Post);

            var response = client.Execute<dynamic>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            string result = response.Data.value;
            if (result == null)
                return new List<BC.Attribute>();

            return JsonConvert.DeserializeObject<List<BC.Attribute>>(result);
        }

        public void SetItemAttribute(string itemNumber, string attributeName, string attributeValue)
        {
            if (string.IsNullOrEmpty(attributeValue))
                attributeValue = string.Empty;

            var client = GetRestClient();

            var request = new RestRequest($"/ItemAttributes_SetItemAttribute?company={Company}", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    itemNumber,
                    attributeName,
                    attributeValue
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        //public BC.Item CreateItem(BC.Item item)
        //{
        //    var client = GetRestClient();

        //    var request = new RestRequest($"/Company('{Company}')/Items", Method.Post);
        //    var json = JsonConvert.SerializeObject(
        //        new
        //        {
        //            type = "Inventory",
        //            item.displayName,
        //            item.number
        //        },
        //        Formatting.None,
        //        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    request.AddParameter("application/json", json, ParameterType.RequestBody);

        //    var response = client.Execute<BC.Item>(request);
        //    if (!response.IsSuccessful)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public BC.Item UpdateItem(BC.Item item)
        //{
        //    var itemMin = GetItemMin(item.number);
        //    var client = GetRestClient();

        //    var request = new RestRequest($"/Company('{Company}')/Items({itemMin.id})", Method.Patch);
        //    request.AddHeader("If-Match", itemMin.odataetag);
        //    var json = JsonConvert.SerializeObject(
        //        new
        //        {
        //            type = "Inventory",
        //            item.displayName,
        //            item.number
        //        },
        //        Formatting.None,
        //        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    request.AddParameter("application/json", json, ParameterType.RequestBody);

        //    var response = client.Execute<BC.Item>(request);
        //    if (!response.IsSuccessful)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public ItemCard CreateItemCard(ItemCard itemCard)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ItemCards", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    itemCard.No,
                    itemCard.Description,
                    Blocked = false,
                    Type = "Inventory",
                    itemCard.Base_Unit_of_Measure,
                    itemCard.Net_Weight,
                    Inventory_Posting_Group = WebService.Config.DefaultInventoryPostingGroup,
                    Item_Category_Code = WebService.Config.DefaultItemCategoryCode,
                    Gen_Prod_Posting_Group = WebService.Config.DefaultGeneralProductPostingGroup
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute<ItemCard>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public ItemCard UpdateItemCard(ItemCard itemCard)
        {
            var client = GetRestClient();

            var itemCardMin = GetItemCardMin(itemCard.No);
            var request = new RestRequest($"/Company('{Company}')/ItemCards('{itemCard.No}')", Method.Patch);
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

            var response = client.Execute<ItemCard>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public ItemCard UpdateItemCardProductionBom(string number)
        {
            var client = GetRestClient();

            var itemCardMin = GetItemCardMin(number);
            var request = new RestRequest($"/Company('{Company}')/ItemCards('{itemCardMin.No}')", Method.Patch);
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

            var response = client.Execute<ItemCard>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        #endregion

        #region BOMs

        public ProductionBOMMin GetBomHeaderMin(string number)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ProductionBOMs('{number}')?$select=No");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ProductionBOMMin>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public ProductionBOM GetBomHeaderAndRows(string number)
        {
            var client = GetRestClient();

            var request =
                new RestRequest($"/Company('{Company}')/ProductionBOMs('{number}')?$expand=ProductionBOMsProdBOMLine");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ProductionBOM>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }

        public ProductionBOM CreateBomHeader(ProductionBOM bomHeader)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ProductionBOMs", Method.Post);
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

            var response = client.Execute<ProductionBOM>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public ProductionBOM UpdateBomHeader(ProductionBOM bomHeader)
        {
            var client = GetRestClient();

            var bomHeaderMin = GetBomHeaderMin(bomHeader.No);
            var request = new RestRequest($"/Company('{Company}')/ProductionBOMs('{bomHeader.No}')", Method.Patch);
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

            var response = client.Execute<ProductionBOM>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public ProdBOMLineMin GetBomRowMin(string parentNumber, int position, string childNumber)
        {
            var client = GetRestClient();

            var request =
                new RestRequest(
                    $"/Company('{Company}')/ProductionBOMLines('{parentNumber}','',{position})?$filter=No eq '{childNumber}'&$select=Production_BOM_No,Line_No,No");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ProdBOMLineMin>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public ProdBOMLineMin GetBomRowMin(string parentNumber, int position)
        {
            var client = GetRestClient();

            var request =
                new RestRequest(
                    $"/Company('{Company}')/ProductionBOMLines('{parentNumber}','',{position})?$select=Production_BOM_No,Line_No,No");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ProdBOMLineMin>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public ProdBOMLine GetBomRow(string parentNumber, int position, string childNumber)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ProductionBOMLines('{parentNumber}','',{position})?$filter=No eq '{childNumber}'");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ProdBOMLine>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }

        public ProdBOMLine CreateBomRow(ProdBOMLine bomRow)
        {
            var client = GetRestClient();

            var request = new RestRequest($"/Company('{Company}')/ProductionBOMLines", Method.Post);
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

            var response = client.Execute<ProdBOMLine>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public ProdBOMLine UpdateBomRow(ProdBOMLine bomRow)
        {
            var client = GetRestClient();

            var bomRowMin = GetBomRowMin(bomRow.Production_BOM_No, bomRow.Line_No, bomRow.No);
            var request =
                new RestRequest(
                    $"/Company('{Company}')/ProductionBOMLines('{bomRow.Production_BOM_No}','',{bomRow.Line_No})",
                    Method.Patch);
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

            var response = client.Execute<ProdBOMLine>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void DeleteBomRow(ProdBOMLine bomRow)
        {
            var client = GetRestClient();

            var bomRowMin = GetBomRowMin(bomRow.Production_BOM_No, bomRow.Line_No);
            var request =
                new RestRequest(
                    $"/Company('{Company}')/ProductionBOMLines('{bomRow.Production_BOM_No}','',{bomRow.Line_No})",
                    Method.Delete);
            request.AddHeader("If-Match", bomRowMin.odataetag);

            var response = client.Execute(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        #endregion

        #region Documents

        public List<BC.Document> GetDocuments(string number)
        {
            var client = GetRestClient();

            var bcItemMin = GetItemMin(number);
            var request = new RestRequest($"/Company('{Company}')/Items({bcItemMin.id})/ItemsdocumentAttachments");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<BC.Document>>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data.Value;
        }

        public BC.Document CreateDocument(string number, string fileName)
        {
            var client = GetRestClient();

            var documents = GetDocuments(number);
            var exitingDocument = documents.SingleOrDefault(d => d.fileName.Equals(fileName));
            if (exitingDocument != null)
                return exitingDocument;

            var bcItemMin = GetItemMin(number);
            var request = new RestRequest($"/Company('{Company}')/DocumentAttachments", Method.Post);
            var json = JsonConvert.SerializeObject(
                new
                {
                    fileName = fileName,
                    parentType = "Item",
                    parentId = bcItemMin.id,
                    lineNumber = documents.Count
                },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute<BC.Document>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public byte[] DownloadDocument(string number, string fileName)
        {
            var client = GetRestClient();

            var bcItemMin = GetItemMin(number);
            var request = new RestRequest($"/Company('{Company}')/Items({bcItemMin.id})/ItemsdocumentAttachments");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<BC.Document>>(request);
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
                    var task = GetImageAsByteArray(documentAttachment.attachmentContentodatamediaReadLink);
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

        private async Task<byte[]> GetImageAsByteArray(string url)
        {
            using (var client = new HttpClient())
            {
                var authenticationString = $"{Username}:{Password}";
                var base64EncodedAuthenticationString =
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                var bytes = await client.GetByteArrayAsync(url);
                return bytes;
            }
        }

        public void UploadDocument(string number, string fileName, byte[] bytes)
        {
            var client = GetRestClient();

            var bcItemMin = GetItemMin(number);
            var request = new RestRequest($"/Company('{Company}')/Items({bcItemMin.id})/ItemsdocumentAttachments");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<ODataResponse<BC.Document>>(request);
            if (!response.IsSuccessful)
                return;

            var documentAttachments = response.Data.Value;
            var documentAttachment = documentAttachments.FirstOrDefault(d => d.fileName.Equals(fileName));
            if (documentAttachment == null)
                return;

            var uploadRequest = new RestRequest($"/Company('{Company}')/DocumentAttachments({documentAttachment.id})/attachmentContent",
                Method.Patch);
            uploadRequest.AddHeader("If-Match", documentAttachment.odataetag);
            uploadRequest.AddHeader("Content-Type", "application/pdf");
            uploadRequest.AddParameter("application/pdf", bytes,
                ParameterType.RequestBody);

            var uploadResponse = client.Execute(uploadRequest);
            if (!uploadResponse.IsSuccessful)
                throw new Exception(response.ErrorMessage);
        }

        #endregion
    }
}