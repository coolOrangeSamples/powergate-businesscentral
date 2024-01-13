using System.Collections.Generic;
using Newtonsoft.Json;

namespace BusinessCentralPlugin.BusinessCentral
{
    public class ProductionBOMMin
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string No { get; set; }
    }

    public class ProductionBOM
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string No { get; set; }
        public string Description { get; set; }
        public string Unit_of_Measure_Code { get; set; }
        public List<ProdBOMLine> ProductionBOMsProdBOMLine { get; set; }
    }
}
