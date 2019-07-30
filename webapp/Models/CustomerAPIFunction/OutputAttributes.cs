using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;

namespace SmartAdmin.Models.CustomerAPIFunctions
{
    public class OuptputAttributes
    {
        // ---------- Response Parameters --------------
        
        public int? Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }        
        
    }
}