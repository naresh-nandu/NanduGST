using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class TXPD
    {
        public string chksum { get; set; }
		public string i_num { get; set; }
		public string i_dt { get; set; }
		public List<TXPD_Doc> doc { get; set; }
    }
	public class TXPD_Doc
	{
		public string doc_num { get; set; }
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
