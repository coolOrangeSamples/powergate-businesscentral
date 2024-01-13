using System.Collections.Generic;
using Newtonsoft.Json;

namespace BusinessCentralPlugin.BusinessCentral
{
    internal class ODataResponse<T>
    {
        [JsonProperty("@odata.context")]
        public string oDataContext { get; set; }
            
        [JsonProperty("value")]
        public List<T> Value { get; set; }
    }
}