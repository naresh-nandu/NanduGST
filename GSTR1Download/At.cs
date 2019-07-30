using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Download
{
    public class At_Itm
    {
        public string ty { get; set; }
        public string hsn_sc { get; set; }
        public int ad_amt { get; set; }
        public int irt { get; set; }
        public double iamt { get; set; }
        public int crt { get; set; }
        public int camt { get; set; }
        public int srt { get; set; }
        public int samt { get; set; }
        public int csrt { get; set; }
        public double csamt { get; set; }
    }


    public class At
    {
        public string chksum { get; set; }
        public string typ { get; set; }
        public string cpty { get; set; }
        public string state_cd { get; set; }
        public string doc_num { get; set; }
        public string doc_dt { get; set; }
        public List<At_Itm> itms { get; set; }
    }

}
