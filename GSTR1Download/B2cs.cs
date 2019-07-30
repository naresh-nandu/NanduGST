using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Download
{
    public class B2cs
    {
        public string chksum { get; set; }
        public string state_cd { get; set; }
        public string ty { get; set; }
        public string hsn_sc { get; set; }
        public int txval { get; set; }
        public int irt { get; set; }
        public int iamt { get; set; }
        public int crt { get; set; }
        public int camt { get; set; }
        public int srt { get; set; }
        public int samt { get; set; }
        public int csrt { get; set; }
        public int csamt { get; set; }
        public string prs { get; set; }
        public string od_num { get; set; }
        public string od_dt { get; set; }
        public string etin { get; set; }
        public string typ { get; set; }
    }



}
