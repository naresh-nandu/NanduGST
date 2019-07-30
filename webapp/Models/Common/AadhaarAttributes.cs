using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Common
{
    public class AadhaarAttributes
    {
        public string AadhaarNo { get; set; }
        public string ReqOTPVal { get; set; }
        public string ResOTPVal { get; set; }        
        public string DeviceId { get; set; }
        public string Userid { get; set; }
        public string ErrMsg { get; set; }
        public string RRN { get; set; }
        public string TrxnRefCode { get; set; }
    }
}