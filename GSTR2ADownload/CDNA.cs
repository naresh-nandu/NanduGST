using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2ADownload
{
    public class NtaItmDet
    {
        public double rt { get; set; }
        public double txval { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }

    public class NtaItc
    {
        public string elg { get; set; }
        public double tx_i { get; set; }
        public double tx_s { get; set; }
        public double tx_c { get; set; }
        public double tx_cs { get; set; }
    }

    public class NtaItm
    {
        public int num { get; set; }
        public NtaItmDet itm_det { get; set; }
        public NtaItc itc { get; set; }
    }

    public class CdnaNt
    {
        public string flag { get; set; }
        public string chksum { get; set; }
        public string ntty { get; set; }
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public string ont_num { get; set; }
        public string ont_dt { get; set; }
        public string inum { get; set; }
        public string p_gst { get; set; }
        public string idt { get; set; }
        public string updby { get; set; }
        public string opd { get; set; }
        public string cflag { get; set; }
        public double val { get; set; }
        public double diff_percent { get; set; }
        public List<NtaItm> itms { get; set; }
    }

    public class Cdna
    {
        public string ctin { get; set; }
        public string cfs { get; set; }
        public List<CdnaNt> nt { get; set; }
    }

    public class CdnaJson
    {
        public List<Cdna> cdna { get; set; }
    }
}
