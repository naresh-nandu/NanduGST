using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTR4
{
    public class CDNRItmDet
    {
        public int csamt { get; set; }
        public int rt { get; set; }
        public int txval { get; set; }
        public int iamt { get; set; }
    }

    public class CDNRItm
    {
        public int num { get; set; }
        public CDNRItmDet itm_det { get; set; }
    }

    public class CDNRNt
    {
        public List<CDNRItm> itms { get; set; }
        public int val { get; set; }
        public int txval { get; set; }
        public string nt_num { get; set; }
        public string inum { get; set; }
        public string nt_dt { get; set; }
        public string p_gst { get; set; }
        public string idt { get; set; }
        public string ntty { get; set; }
        public string chksum { get; set; }
    }

    public class CDNR
    {
        public string cfs { get; set; }
        public string ctin { get; set; }
        public List<CDNRNt> nt { get; set; }
    }

    public class CDNRJson
    {
        public List<CDNR> cdnr { get; set; }
    }
}
