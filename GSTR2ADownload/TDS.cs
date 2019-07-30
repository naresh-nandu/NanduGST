using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2ADownload
{

    public class Tds
    {
        public double samt { get; set; }
        public double amt_ded { get; set; }
        public double camt { get; set; }
        public double? iamt { get; set; }
        public string gstin_ded { get; set; }
    }

    public class TdsJson
    {
        public List<Tds> tds { get; set; }
    }
}

