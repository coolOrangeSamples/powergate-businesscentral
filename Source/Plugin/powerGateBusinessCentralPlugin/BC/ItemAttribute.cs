using Newtonsoft.Json;

namespace powerGateBusinessCentralPlugin.BC
{
    public class ItemAttribute
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Values { get; set; }
        public bool Blocked { get; set; }
    }
}
