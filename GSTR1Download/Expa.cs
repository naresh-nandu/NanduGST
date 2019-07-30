using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Download
{

    public class Expa_Itm
    {
        public string ty { get; set; }
        public string hsn_sc { get; set; }
        public int txval { get; set; }
        public int irt { get; set; }
        public double iamt { get; set; }
        public int crt { get; set; }
        public int camt { get; set; }
        public int srt { get; set; }
        public int samt { get; set; }
        public int csrt { get; set; }
        public double csamt { get; set; }
    }

    public class Expa_Inv
    {
        public string chksum { get; set; }
        public string oinum { get; set; }
        public string oidt { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string sbpcode { get; set; }
        public string sbnum { get; set; }
        public string sbdt { get; set; }
        public string prs { get; set; }
        public string od_num { get; set; }
        public string od_dt { get; set; }
        public List<Expa_Itm> itms { get; set; }
    }

    public class Expa
    {
        public string ex_tp { get; set; }
        public List<Expa_Inv> inv { get; set; }
    }

}
