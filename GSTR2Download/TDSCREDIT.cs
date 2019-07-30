using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class TDSCREDIT
    {
        public string ctin { get; set; }
		public List<TDSCREDIT_TdsInvoice> tds_invoices { get; set; }
    }

    public class TDSCREDIT_TdsInvoice
    {
        public string chksum { get; set; }
		public string i_num { get; set; }
		public string i_dt { get; set; }
		public double i_val { get; set; }
		public string pay_dt { get; set; }
		public double tds_val { get; set; }
		public int irt { get; set; }
		public int iamt { get; set; }
		public double crt { get; set; }
		public double camt { get; set; }
		public double srt { get; set; }
		public double samt { get; set; }
		public int csrt { get; set; }
		public int csamt { get; set; }
    }
}
