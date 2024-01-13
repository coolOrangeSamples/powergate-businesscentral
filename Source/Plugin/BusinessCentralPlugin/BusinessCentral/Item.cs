using Newtonsoft.Json;

namespace BusinessCentralPlugin.BusinessCentral
{
    public class ItemMin
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string id { get; set; }
        public string number { get; set; }
        public Itemspicture Itemspicture { get; set; }
    }

    public class Itemspicture
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string id { get; set; }
        [JsonProperty("pictureContent@odata.mediaEditLink")]
        public string pictureContentodatamediaEditLink { get; set; }
        [JsonProperty("pictureContent@odata.mediaReadLink")]
        public string pictureContentodatamediaReadLink { get; set; }
    }
}