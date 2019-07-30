using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2ADownload
{
    public class B2BAItmDet
    {
        public double samt { get; set; }
        public double rt { get; set; }
        public double txval { get; set; }
        public double camt { get; set; }
        public double? iamt { get; set; }
        public double? csamt { get; set; }
    }

    public class B2BAItm
    {
        public int num { get; set; }
        public B2BAItmDet itm_det { get; set; }
    }

    public class B2BAInv
    {
        public List<B2BAItm> itms { get; set; }
        public string oinum { get; set; }
        public string oidt { get; set; }
        public double val { get; set; }
        public string inv_typ { get; set; }
        public string pos { get; set; }
        public string idt { get; set; }
        public string rchrg { get; set; }
        public string inum { get; set; }
        public string chksum { get; set; }
        public string updby { get; set; }
        public string cflag { get; set; }
        public double diff_percrnt { get; set; }
    }

    public class B2BA
    {
        public string ctin { get; set; }
        public string cfs { get; set; }
        public List<B2BAInv> inv { get; set; }
    }

    public class B2BAJson
    {
        public List<B2BA> b2ba { get; set; }
    }
}
