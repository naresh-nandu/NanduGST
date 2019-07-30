using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Download
{
    public class Hsn
    {
        public string chksum { get; set; }
        public List<Hsn_Datum> data { get; set; }
    }

    public class Hsn_Datum
    {
        public int num { get; set; }
        public string ty { get; set; }
        public string hsn_sc { get; set; }
        public string desc { get; set; }
        public string uqc { get; set; }
        public int qty { get; set; }
        public string sply_ty { get; set; }
        public int txval { get; set; }
        public int irt { get; set; }
        public double iamt { get; set; }
        public int crt { get; set; }
        public int camt { get; set; }
        public int srt { get; set; }
        public int samt { get; set; }
        public double csrt { get; set; }
        public double csamt { get; set; }
    }

}
