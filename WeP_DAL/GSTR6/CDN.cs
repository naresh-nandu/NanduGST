using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTR6
{
    public class NtItmDet
    {
        public double rt { get; set; }
        public double txval { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }

    public class NtItc
    {
        public string elg { get; set; }
        public double tx_i { get; set; }
        public double tx_s { get; set; }
        public double tx_c { get; set; }
        public double tx_cs { get; set; }
    }

    public class NtItm
    {
        public int num { get; set; }
        public NtItmDet itm_det { get; set; }
        public NtItc itc { get; set; }
    }

    public class CdnNt
    {
        public string flag { get; set; }
        public string chksum { get; set; }
        public string ntty { get; set; }
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public string pos { get; set; }
        public string inum { get; set; }
     
        public string idt { get; set; }
        public string updby { get; set; }
        public string opd { get; set; }
        public string cflag { get; set; }
     
        public List<NtItm> itms { get; set; }
    }

    public class Cdn
    {
        public string ctin { get; set; }
        public List<CdnNt> nt { get; set; }
    }

    public class CdnJson
    {
        public List<Cdn> cdn { get; set; }
    }
}
