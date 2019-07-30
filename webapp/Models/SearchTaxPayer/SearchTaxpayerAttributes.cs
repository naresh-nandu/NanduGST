using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.SearchTaxPayer
{
    public class SearchTaxpayerAttributes
    {
        public string stjCd { get; set; }
        public string lgnm { get; set; }
        public string dty { get; set; }
        public string stj { get; set; }
        public string cxdt { get; set; }
        public string gstin { get; set; }
        public List<string> nba { get; set; }
        public string lstupdt { get; set; }
        public string rgdt { get; set; }
        public string ctb { get; set; }
        public string sts { get; set; }
        public string ctjCd { get; set; }
        public string tradeNam { get; set; }
        public string ctj { get; set; }
    }
}