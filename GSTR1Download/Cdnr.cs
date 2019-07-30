using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Download
{
    public class Cdnr_Nt
    {
        public string flag { get; set; }
        public string updby { get; set; }
        public string chksum { get; set; }
        public string ntty { get; set; }
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public string rsn { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public string rchrg { get; set; }
        public int val { get; set; }
        public int irt { get; set; }
        public double iamt { get; set; }
        public int crt { get; set; }
        public int camt { get; set; }
        public int srt { get; set; }
        public int samt { get; set; }
        public int csrt { get; set; }
        public int csamt { get; set; }
        public string etin { get; set; }
    }



    public class Cdnr
    {
        public string ctin { get; set; }
        public string cfs { get; set; }
        public List<Cdnr_Nt> nt { get; set; }
    }

}
