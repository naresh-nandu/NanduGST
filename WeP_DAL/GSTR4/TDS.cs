using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTR4
{
    public class TDS
    {
        public string gstin_ded { get; set; }
        public string chksum { get; set; }
        public int amt_ded { get; set; }
        public int camt { get; set; }
        public int samt { get; set; }
    }

    public class TDSJson
    {
        public List<TDS> tds { get; set; }
    }
}
