using Newtonsoft.Json;
using System.Collections.Generic;

namespace powerGateBusinessCentralPlugin.BC
{
    internal class ODataResponse<T>
    {
        [JsonProperty("@odata.context")]
        public string oDataContext { get; set; }
            
        [JsonProperty("value")]
        public List<T> Value { get; set; }
    }
}