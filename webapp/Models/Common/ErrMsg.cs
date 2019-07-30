using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Common
{
    public class ErrorMsg
    {
        public string status_cd { get; set; }
        public Error error { get; set; }
    }

    public class Error
    {
        public string message { get; set; }
        public string error_cd { get; set; }
    }
}