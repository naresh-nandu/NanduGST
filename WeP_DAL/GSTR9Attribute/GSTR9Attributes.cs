using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeP_DAL.GSTR9Attribute
{
    public class GSTR9Attributes
    {
        public class Gstr9List
        {
            public decimal txval { get; set; }
            public decimal iamt { get; set; }
            public decimal camt { get; set; }
            public decimal samt { get; set; }
            public decimal csamt { get; set; }
            public string natureofsupplies { get; set; }
            public string gstin { get; set; }
            public string period { get; set; }
            public decimal taxpayable { get; set; }
            public decimal taxpaidthroughcash { get; set; }
            public decimal interest { get; set; }
            public decimal penalty { get; set; }
            public decimal latefee_oth { get; set; }
            public decimal totalquantity { get; set; }
            public decimal rate { get; set; }
            public string hsn { get; set; }
            public string uqc { get; set; }
            public string Action { get; set; }
            public string itc_typ { get; set; }
            public string desc { get; set; }
            public string isconcesstional { get; set; }

        }
        public class Gstr9List1
        {
            public decimal txval { get; set; }
            public decimal iamt { get; set; }
            public decimal camt { get; set; }
            public decimal samt { get; set; }
            public decimal csamt { get; set; }
           

        }

    }
    public class ReportViewModel
    {
        public List<GSTR9Attributes.Gstr9List> ReportMgmt { get; set; }

        public List<GSTR9Attributes.Gstr9List1> ReportMgmt1 { get; set; }
        public List<GSTR9Attributes.Gstr9List1> ReportMgmt2 { get; set; }
        public List<GSTR9Attributes.Gstr9List1> ReportMgmt3 { get; set; }
        public List<GSTR9Attributes.Gstr9List1> ReportMgmt4 { get; set; }
        public List<GSTR9Attributes.Gstr9List1> ReportMgmt5 { get; set; }
        public List<GSTR9Attributes.Gstr9List1> ReportMgmt6 { get; set; }
        public List<GSTR9Attributes.Gstr9List> ReportMgmt7 { get; set; }
        public List<GSTR9Attributes.Gstr9List> ReportMgmt8 { get; set; }
        public List<GSTR9Attributes.Gstr9List> ReportMgmt9 { get; set; }
        public List<GSTR9Attributes.Gstr9List> ReportMgmt10 { get; set; }
        public List<GSTR9Attributes.Gstr9List> ReportMgmt11 { get; set; }

    }
}