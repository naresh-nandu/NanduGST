using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_EWayBill
{
    public class EwbRequestAttributes
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string action { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string username { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string password { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string app_key { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string data { get; set; }        
    }
}
