using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTRDelete
{
   public class GSTRCommonDelete_DAL
    {
        public GSTRCommonDelete_DAL()
        {

        }

        public class GSTRDeleteSummaryAttributes
        {
            public string ActionType { get; set; }
            public int RecordCount { get; set; }
            public int NoOfInvoices { get; set; }
            public decimal txval { get; set; }
            public decimal iamt { get; set; }
            public decimal camt { get; set; }
            public decimal samt { get; set; }
            public decimal csamt { get; set; }
            public string RecType { get; set; }
        }

        public class GSTRDeleteViewModel
        {
            public List<GSTRDeleteSummaryAttributes> GSTRDeleteSummary { get; set; }

            public List<GSTRDeleteSummaryAttributes> GSTR1Summary { get; set; }
        }
    }
}
