using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace powerGateBusinessCentralPlugin.BC
{
    public class ItemMin
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string id { get; set; }
        public string number { get; set; }
    }
    
    public class Item
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string id { get; set; }
        public string number { get; set; }
        public string displayName { get; set; }
        public string type { get; set; }
        public string itemCategoryId { get; set; }
        public string itemCategoryCode { get; set; }
        public bool blocked { get; set; }
        public string gtin { get; set; }
        public int inventory { get; set; }
        public double unitPrice { get; set; }
        public bool priceIncludesTax { get; set; }
        public double unitCost { get; set; }
        public string taxGroupId { get; set; }
        public string taxGroupCode { get; set; }
        public string baseUnitOfMeasureId { get; set; }
        public string baseUnitOfMeasureCode { get; set; }
        public string generalProductPostingGroupId { get; set; }
        public string generalProductPostingGroupCode { get; set; }
        public string inventoryPostingGroupId { get; set; }
        public string inventoryPostingGroupCode { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public Itemspicture Itemspicture { get; set; }
    }

    public class Itemspicture
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string id { get; set; }
        public string parentType { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string contentType { get; set; }

        [JsonProperty("pictureContent@odata.mediaEditLink")]
        public string pictureContentodatamediaEditLink { get; set; }

        [JsonProperty("pictureContent@odata.mediaReadLink")]
        public string pictureContentodatamediaReadLink { get; set; }
    }
}