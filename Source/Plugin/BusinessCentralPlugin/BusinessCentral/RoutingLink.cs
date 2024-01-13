using Newtonsoft.Json;

namespace BusinessCentralPlugin.BusinessCentral
{
    public class RoutingLink
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}