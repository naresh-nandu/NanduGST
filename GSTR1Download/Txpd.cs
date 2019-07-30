using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Download
{
    public class Txpd
    {
        public string chksum { get; set; }
        public string typ { get; set; }
        public string cpty { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public string doc_num { get; set; }
        public string doc_dt { get; set; }
        public int irt { get; set; }
        public double iamt { get; set; }
        public int crt { get; set; }
        public int camt { get; set; }
        public int srt { get; set; }
        public int samt { get; set; }
        public int csrt { get; set; }
        public int csamt { get; set; }
    }

}
