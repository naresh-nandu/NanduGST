using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Common
{
    public class ErrorMessage
    {
        public string message { get; set; }
        public string error_cd { get; set; }
    }

    public class GSPResponse
    {
        public string data { get; set; }
        public string hmac { get; set; }
        public string auth_token { get; set; }
        public string expiry { get; set; }
        public string sek { get; set; }
        public int status_cd { get; set; }
        public string rek { get; set; }
        public ErrorMessage error { get; set; }        
    }

}