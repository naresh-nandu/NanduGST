using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTRReport
{
   public class GSTR3B2AReport_DL
    {
        public GSTR3B2AReport_DL()
        {

        }

        public class GSTR3B2A_Attributes
        {
            public string Description { get; set; }
            public decimal Total { get; set; }
            public decimal SGST { get; set; }
            public decimal CGST { get; set; }
            public decimal IGST { get; set; }
            public decimal CESS { get; set; }

        }

        public class GSTR2A3B_Report_ViewModel
        {
            public List<GSTR3B2A_Attributes> ReportMgmt { get; set; }
        }
    }
}
