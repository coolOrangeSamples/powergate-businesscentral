using Newtonsoft.Json;

namespace BusinessCentralPlugin.BusinessCentral
{
    public class Lookup
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string id { get; set; }
        public string code { get; set; }
    }
}
