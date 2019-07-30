using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTRReport
{
   public class Gstr3B2AReportDal
    {
        public Gstr3B2AReportDal()
        {

        }

        public class Gstr3B2AAttributes
        {
            public string Description { get; set; }
            public decimal Total { get; set; }
            public decimal SGST { get; set; }
            public decimal CGST { get; set; }
            public decimal IGST { get; set; }
            public decimal CESS { get; set; }

        }

        public class Gstr2A3BReportViewModel
        {
            public List<Gstr3B2AAttributes> ReportMgmt { get; set; }
        }
    }
}
