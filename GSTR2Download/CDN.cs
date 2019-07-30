using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class CDN
    {
		public string ctin { get; set; }
		public string cfs { get; set; }
		public List<CDN_Itc> nt { get; set; }
    }

    public class CDN_Itc
    {
         public double tx_i { get; set; }
		public double tx_s { get; set; }
		public double tx_c { get; set; }
		public int tx_cs { get; set; }
		public int tc_c { get; set; }
		public double tc_i { get; set; }
		public double tc_s { get; set; }
		public int tc_cs { get; set; }
    }
	public class CDN_Nt
	{
		public string flag { get; set; }
		public string chksum { get; set; }
		public string ntty { get; set; }
		public string nt_num { get; set; }
		public string nt_dt { get; set; }
		public string rsn { get; set; }
		public string inum { get; set; }
		public string idt { get; set; }
		public double val { get; set; }
		public double irt { get; set; }
		public double iamt { get; set; }
		public double crt { get; set; }
		public double camt { get; set; }
		public double srt { get; set; }
		public double samt { get; set; }
		public int csrt { get; set; }
		public double csamt { get; set; }
		public string updby { get; set; }
		public string elg { get; set; }
		public string rchrg { get; set; }
		public CDN_Itc itc { get; set; }
	}
}
