using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class ITC
    {
        public string chksum { get; set; }
		public string typ { get; set; }
		public string stin { get; set; }
		public string inv_doc_num { get; set; }
		public string inv_doc_dt { get; set; }
		public int o_ig { get; set; }
		public int n_ig { get; set; }
		public int o_cg { get; set; }
		public int n_cg { get; set; }
		public int o_sg { get; set; }
		public int n_sg { get; set; }
		public int o_cs { get; set; }
		public int n_cs { get; set; }
    }
}
