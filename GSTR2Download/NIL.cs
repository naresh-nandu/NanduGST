using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class NIL
    {
        public string sply_ty { get; set; }
		public List<NIL_Nil> nil_data { get; set; }
    }

    public class NIL_Nil
    {
        public string chksum { get; set; }
		public string ty { get; set; }
		public string hsn_sc { get; set; }
		public int cpddr { get; set; }
		public int uredr { get; set; }
		public double exptdsply { get; set; }
		public double ngsply { get; set; }
		public int nilsply { get; set; }
    }
}
