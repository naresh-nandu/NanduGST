using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class IMPGA
    {
        public string boe_num { get; set; }
		public string boe_dt { get; set; }
		public double boe_val { get; set; }
		public string port_code { get; set; }
		public string chksum { get; set; }
		public string oboe_num { get; set; }
		public string oboe_dt { get; set; }
		public List<IMPGA_Itm> itms { get; set; }
    }

    public class IMPGA_Itm
    {
		public int num { get; set; }
		public string hsn_sc { get; set; }
		public double txval { get; set; }
		public double irt { get; set; }
		public double iamt { get; set; }
		public double csrt { get; set; }
		public double csamt { get; set; }
		public string elg { get; set; }
		public double tx_i { get; set; }
		public double tc_i { get; set; }
		public int tx_cs { get; set; }
		public int tc_cs { get; set; }
    }
}
