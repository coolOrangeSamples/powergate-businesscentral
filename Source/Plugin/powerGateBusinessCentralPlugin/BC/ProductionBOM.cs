using Newtonsoft.Json;
using System.Collections.Generic;

namespace powerGateBusinessCentralPlugin.BC
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
        public string Status { get; set; }
        public string Search_Name { get; set; }
        public string Version_Nos { get; set; }
        public string ActiveVersionCode { get; set; }
        public string Last_Date_Modified { get; set; }
        public List<ProdBOMLine> ProductionBOMsProdBOMLine { get; set; }
    }
}
