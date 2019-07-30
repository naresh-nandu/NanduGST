using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models
{
    public class ErrorList
    {
        public int totalrecords { get; set; }
        public int processedrecords { get; set; }
        public int errorrecords { get; set; }
        public string actiontype { get; set; }

        public string payload { get; set; }


    }
}