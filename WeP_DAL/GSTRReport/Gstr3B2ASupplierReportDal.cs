using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTRReport
{
    public class Gstr3B2ASupplierReportDal
    {
        public Gstr3B2ASupplierReportDal()
        {

        }

        public class Gstr3AGstr2AAttributes
        {
            public string Ctin { get; set; }
            public string CtinName { get; set; }
            public decimal Sgst { get; set; }
            public decimal Cgst { get; set; }
            public decimal Igst { get; set; }
            public decimal Cess { get; set; }
            public decimal TaxVal { get; set; }
            public decimal Total { get; set; }
            public decimal samt { get; set; }
            public decimal camt { get; set; }
            public decimal iamt { get; set; }
            public decimal csamt { get; set; }
            public decimal TotalValue { get; set; }
            public decimal taxval { get; set; }
            public decimal GrandTotal { get; set; }
        }


        public class Gstr3BGstr2AReportViewModel
        {
            public List<Gstr3AGstr2AAttributes> GSTRReportMgmt { get; set; }
        }

    }


}
