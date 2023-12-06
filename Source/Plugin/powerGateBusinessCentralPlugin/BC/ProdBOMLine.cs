using Newtonsoft.Json;

namespace powerGateBusinessCentralPlugin.BC
{
    public class ProdBOMLineMin
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string Production_BOM_No { get; set; }
        public int Line_No { get; set; }
        public string No { get; set; }
    }

    public class ProdBOMLine
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string Production_BOM_No { get; set; }
        public string Version_Code { get; set; }
        public int Line_No { get; set; }
        public string Type { get; set; }
        public string No { get; set; }
        public string Variant_Code { get; set; }
        public string Description { get; set; }
        public string Calculation_Formula { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Depth { get; set; }
        public decimal Weight { get; set; }
        public decimal Quantity_per { get; set; }
        public string Unit_of_Measure_Code { get; set; }
        public decimal Scrap_Percent { get; set; }
        public string Routing_Link_Code { get; set; }
        public string Position { get; set; }
        public string Position_2 { get; set; }
        public string Position_3 { get; set; }
        public string Lead_Time_Offset { get; set; }
        public string Starting_Date { get; set; }
        public string Ending_Date { get; set; }
    }
}
