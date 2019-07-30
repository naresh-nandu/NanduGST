using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTRDelete
{
   public class GstrCommonDeleteDal
    {
        public GstrCommonDeleteDal()
        {

        }

        public class GstrDeleteSummaryAttributes
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

        public class GstrDeleteViewModel
        {
            public List<GstrDeleteSummaryAttributes> GSTRDeleteSummary { get; set; }

            public List<GstrDeleteSummaryAttributes> GSTR1Summary { get; set; }

            public List<GstrDeleteSummaryAttributes> GSTR2ASummary { get; set; }

            public List<GstrDeleteSummaryAttributes> GSTR4Summary { get; set; }
        }

        public class Gstr2ADownloadSummaryAttributes
        {
            public string ActionType { get; set; }
            public string Gstin { get; set; }
            public string fp { get; set; }
            public int RecordCount { get; set; }
            public int NoOfInvoices { get; set; }
            public decimal txval { get; set; }
            public decimal iamt { get; set; }
            public decimal camt { get; set; }
            public decimal samt { get; set; }
            public decimal csamt { get; set; }
            public string RecType { get; set; }
        }

        public class Gstr1RetStatus
        {
            public string gstin { get; set; }
            public string fp { get; set; }
            public string referenceno { get; set; }
            public string actiontype { get; set; }
            public string status { get; set; }
            public string errorreport { get; set; }
            public string createddate { get; set; }
        }

        public class Gstr1RetSum
        {
            public string SNo { get; set; }
            public string Period { get; set; }
            public string actiontype { get; set; }
            public string totalrecords { get; set; }
            public string totalvalue { get; set; }
            public string totaltaxableValue { get; set; }
            public string totaligst { get; set; }
            public string totalcgst { get; set; }
            public string totalsgst { get; set; }
            public string totalcess { get; set; }
            public string nilsupply { get; set; }
            public string nilexempted { get; set; }
            public string nongst { get; set; }
            public string totaldocissued { get; set; }
            public string totaldoccancelled { get; set; }
            public string netdocissued { get; set; }


        }
        public class Gstr2ADownloadViewModel
        {
            public List<Gstr2ADownloadSummaryAttributes> GSTR2ASummary { get; set; }
            public List<Gstr1RetStatus> Gstr1RetStatus { get; set; }
            public List<Gstr1RetSum> Gstr1RetSum { get; set; }
        }

        public class Gstr6DownloadViewModel
        {
            public List<Gstr2ADownloadSummaryAttributes> GSTR6Summary { get; set; }
            public List<Gstr1RetStatus> Gstr6RetStatus { get; set; }
        }
    }
}
