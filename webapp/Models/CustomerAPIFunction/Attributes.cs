using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;

namespace SmartAdmin.Models.CustomerAPIFunctions
{    
    public class Attributes
    {
        // ---------- Request Parameters For Registration & Login --------------

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Company { get; set; }
                
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GSTINNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MobileNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PANNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Statecode { get; set; }
                
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ValidFrom { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ValidTo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AadhaarNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ReferenceNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GSTINUserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        // ---------- Request Parameters For Registration & Login --------------


        // ----------- Input Prarameters ------------------ //
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string gstin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string period { get; set; }


        // ------------- Input parameters for Wallet Purpose ----------------------
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustEmail { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustMobile { get; set; }
                
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? TotalValue { get; set; }

        public int? WalletPack { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomerType { get; set; }



        // -------------- Output Parameters ---------------- //
        public int? Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
                
        public decimal? Balance { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InvoiceCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string APICount { get; set; }

    }
}