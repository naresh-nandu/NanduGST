using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1ADownload
{
    public class B2B_ItmDet
    {
        public string ty { get; set; }
        public string hsn_sc { get; set; }
        public int txval { get; set; }
        public int irt { get; set; }
        public double iamt { get; set; }
        public int crt { get; set; }
        public double camt { get; set; }
        public int srt { get; set; }
        public int samt { get; set; }
        public int csrt { get; set; }
        public double csamt { get; set; }
    }

    public class B2B_Itm
    {
        public int num { get; set; }
        public B2B_ItmDet itm_det { get; set; }
    }

    public class B2B_Inv
    {
        public string flag { get; set; }
        public string updby { get; set; }
        public string chksum { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string pos { get; set; }
        public string rchrg { get; set; }
        public string prs { get; set; }
        public string od_num { get; set; }
        public string od_dt { get; set; }
        public string etin { get; set; }
        public List<B2B_Itm> itms { get; set; }
    }

    public class B2B
    {
        public string ctin { get; set; }
        public string cfs { get; set; }
        public List<B2B_Inv> inv { get; set; }
    }

}
