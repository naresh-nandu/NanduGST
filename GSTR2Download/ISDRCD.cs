using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class ISDRCD
    {
        public string gstin_isd { get; set; }
		public string i_num { get; set; }
		public string i_dt { get; set; }
		public int ig_cr { get; set; }
		public double cg_cr { get; set; }
		public double sg_cr { get; set; }
		public string chksum { get; set; }
    }
}
