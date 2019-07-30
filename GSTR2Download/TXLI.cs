using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class TXLI
    {
        public string cpty { get; set; }
		public string reg_type { get; set; }
		public string chksum { get; set; }
		public string state_cd { get; set; }
		public string dnum { get; set; }
		public string dt { get; set; }
		public List<TXLI_Itm> itms { get; set; }
    }

    public class TXLI_Itm
    {
        public string ty { get; set; }
		public string hsn_sc { get; set; }
		public double txval { get; set; }
		public int irt { get; set; }
		public int iamt { get; set; }
		public double crt { get; set; }
		public double camt { get; set; }
		public double srt { get; set; }
		public double samt { get; set; }
		public double csrt { get; set; }
		public double csamt { get; set; }
    }
}
