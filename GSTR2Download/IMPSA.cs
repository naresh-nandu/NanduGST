using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class IMPSA
    {
        public string inum { get; set; }
		public string idt { get; set; }
		public string oinum { get; set; }
		public string oidt { get; set; }
		public double ival { get; set; }
		public string chksum { get; set; }
		public List<IMPSA_Itm> itms { get; set; }
    }

    public class IMPSA_Itm
    {
        public int num { get; set; }
		public string sac { get; set; }
		public double txval { get; set; }
		public string elg { get; set; }
		public double irt { get; set; }
		public double iamt { get; set; }
		public int csrt { get; set; }
		public int csamt { get; set; }
		public int tx_i { get; set; }
		public double tc_i { get; set; }
		public int tx_cs { get; set; }
		public int tc_cs { get; set; }
    }
}
