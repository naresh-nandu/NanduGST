using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2ADownload
{

    public class IsdaInv
    {
        public string flag { get; set; }
        public double it_elg { get; set; }
        public string isd_docty { get; set; }
        public string docdt { get; set; }
        public string docnum { get; set; }
        public string odocdt { get; set; }
        public string odocnum { get; set; }
        public string chksum { get; set; }
        public string updby { get; set; }
        public string cflag { get; set; }
        public double samt { get; set; }
        public double rt { get; set; }
        public double txval { get; set; }
        public double camt { get; set; }
        public double? iamt { get; set; }
        public double? csamt { get; set; }
    }

    public class Isda
    {
        public string ctin { get; set; }
        public string cfs { get; set; }
        public List<IsdaInv> inv { get; set; }
    }

    public class IsdaJson
    {
        public List<Isda> isda { get; set; }
    }
}
