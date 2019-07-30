using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Common
{
    public class DownloadStatusResponse
    {
        public string status_cd { get; set; }
        //public ErrorReport error_report { get; set; }
        public string error_report { get; set; }
    }

    public class ErrorReport
    {
        //public string error_msg { get; set; }
        //public string error_cd { get; set; }

        public string b2b { get; set; }
        public string b2cl { get; set; }
        public string b2cs { get; set; }
        public string cdnr { get; set; }
        public string exp { get; set; }
        public string txpd { get; set; }
        public string nil { get; set; }
        public string doc_issue { get; set; }
        public string hsnsum { get; set; }
        public string at { get; set; }
        public string cdnur { get; set; }
    }
    
}