using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2ADownload
{

    public class Tdsa
    {
        public double samt { get; set; }
        public double amt_ded { get; set; }
        public double oamt_ded { get; set; }
        public double camt { get; set; }
        public double? iamt { get; set; }
        public string gstin_ded { get; set; }
        public string ogstin_ded { get; set; }
        public string omonth { get; set; }
    }

    public class TdsaJson
    {
        public List<Tdsa> tdsa { get; set; }
    }
}

