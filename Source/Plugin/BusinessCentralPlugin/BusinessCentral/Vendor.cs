using Newtonsoft.Json;

namespace BusinessCentralPlugin.BusinessCentral
{
    public class Vendor
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string id { get; set; }
        public string number { get; set; }
        public string displayName { get; set; }
    }
}