using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class Main
    {

        public string gstin { get; set; }
        public string fp { get; set; }
        public List<B2B> b2b { get; set; }
        public List<B2BA> b2ba { get; set; }
        public List<CDN> cdn { get; set; }
		public List<CDNA> cdna { get; set; }
        public List<IMPG> imp_g { get; set; }
        public List<IMPGA> imp_ga { get; set; }
        public List<IMPS> imp_s { get; set; }
        public List<IMPSA> imp_sa { get; set; }
        public List<NIL> nil_supplies { get; set; }
        public List<ISDRCD> isd_rcd { get; set; }
        public List<TDSCREDIT> tds_credit { get; set; }
        public List<TCSDATA> tcs_data { get; set; }
        public List<ITC> itc_rcd { get; set; }
        public List<TXLI> txi { get; set; }
        public List<ATXLI> atxi { get; set; }
        public List<TXPD> txpd { get; set; }       

    }
}
