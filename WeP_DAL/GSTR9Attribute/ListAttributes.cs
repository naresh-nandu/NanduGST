using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeP_DAL.GSTR9Attribute
{
    public class ListAttributes
    {
        public class Gstr1
        {
            public decimal rt { get; set; }
            public decimal txval { get; set; }        
            public decimal IGST { get; set; }
            public decimal CESS { get; set; }
            public decimal TotalTax { get; set; }
           
        }
        public class GstrIntra
        {      
            public decimal taxval { get; set; }
            public decimal CGST { get; set; }
            public decimal SGST { get; set; }
            public decimal rate { get; set; }
            public decimal TotTax { get; set; }
        }
        public class GstrExport
        {
            public decimal taxvalue { get; set; }
            public decimal IGSTRate { get; set; }
            public decimal rte { get; set; }
            public string extyp { get; set; }
        }
        public class GstrWopay
        {
            public decimal taxv { get; set; }           
        }
        public class GstrSewop
        {
            public decimal taxvall { get; set; }
        }
        public class GstrSales
        {
            public decimal taxval { get; set; }
            public decimal CGST { get; set; }
            public decimal SGST { get; set; }
            public decimal rate { get; set; }
            public decimal CESS { get; set; }
            public decimal IGST { get; set; }
            public decimal TotTax { get; set; }
        }
        public class Gstr2Inter
        {
            public decimal rt { get; set; }
            public decimal taxval { get; set; }
            public decimal CGST { get; set; }
            public decimal SGST { get; set; }
            public decimal rate { get; set; }
            public decimal CESS { get; set; }
            public decimal IGST { get; set; }
            public decimal TotTax { get; set; }
        }

        public class Gstr2Intra
        {
            public decimal rt { get; set; }
            public decimal taxval { get; set; }
            public decimal CGST { get; set; }
            public decimal SGST { get; set; }
            public decimal rate { get; set; }
            public decimal CESS { get; set; }
            public decimal IGST { get; set; }
            public decimal TotTax { get; set; }
        }
        public class Gstr2Export
        {
            public decimal rt { get; set; }
            public decimal taxval { get; set; }
            public decimal CGST { get; set; }
            public decimal SGST { get; set; }
            public decimal rate { get; set; }
            public decimal CESS { get; set; }
            public decimal IGST { get; set; }
            public decimal TotTax { get; set; }
        }

        public class Gstr2SalesReturns
        {
            public decimal rt { get; set; }
            public decimal taxval { get; set; }
            public decimal CGST { get; set; }
            public decimal SGST { get; set; }
            public decimal rate { get; set; }
            public decimal CESS { get; set; }
            public decimal IGST { get; set; }
            public decimal TotTax { get; set; }
        }
        public class Gstr2Nil
        {
            public decimal Purchased { get; set; }
        }
        public class ReportViewModel
        {
            public List<ListAttributes.Gstr1> ReportMgmt { get; set; }
            public List<ListAttributes.GstrIntra> ReportMgmt1 { get; set; }
            public List<ListAttributes.GstrExport> ReportMgmt2 { get; set; }
            public List<ListAttributes.GstrWopay> ReportMgmt3 { get; set; }
            public List<ListAttributes.GstrSewop> ReportMgmt4 { get; set; }
            public List<ListAttributes.GstrSales> ReportMgmt5 { get; set; }
            public List<ListAttributes.Gstr2Inter> ReportMgmt6 { get; set; }
            public List<ListAttributes.Gstr2Intra> ReportMgmt7 { get; set; }
            public List<ListAttributes.Gstr2Export> ReportMgmt8 { get; set; }
            public List<ListAttributes.Gstr2Nil> ReportMgmt9 { get; set; }
            public List<ListAttributes.Gstr2SalesReturns> ReportMgmt10 { get; set; }
        }
    }
}