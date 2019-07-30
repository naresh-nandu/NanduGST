using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTR4
{
    public class CDNRAItmDet
    {
        public int rt { get; set; }
        public int txval { get; set; }
        public int iamt { get; set; }
    }

    public class CDNRAItm
    {
        public int num { get; set; }
        public CDNRAItmDet itm_det { get; set; }
    }

    public class CDNRANt
    {
        public List<CDNRAItm> itms { get; set; }
        public int val { get; set; }
        public string ont_num { get; set; }
        public int txval { get; set; }
        public string nt_num { get; set; }
        public string inum { get; set; }
        public string nt_dt { get; set; }
        public string p_gst { get; set; }
        public string ont_dt { get; set; }
        public string idt { get; set; }
        public string ntty { get; set; }
        public string chksum { get; set; }
    }

    public class CDNRA
    {
        public string cfs { get; set; }
        public string ctin { get; set; }
        public List<CDNRANt> nt { get; set; }
    }

    public class CDNRAJson
    {
        public List<CDNRA> cdnra { get; set; }
    }
}
