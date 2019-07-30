using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Reconcilation
{
    public class Recon_Attributes_EWB
    {
        public class totalList
        {
            public string  total { get; set; }
 

        }


        public class matchList
        {
            public string match { get; set; }
        }
        public class diffList
        {
            public string diff { get; set; }
        }

        public class EwayList
        {
            public string gstin { get; set; }
            public string docdate { get; set; }
            public string docno { get; set; }


        }

    }
    public class ReportViewModel
    {
        public List<Recon_Attributes_EWB.totalList> ReportMgmt { get; set; }

        public List<Recon_Attributes_EWB.matchList> ReportMgmt1 { get; set; }
        public List<Recon_Attributes_EWB.diffList> ReportMgmt2 { get; set; }
        public List<Recon_Attributes_EWB.totalList> ReportMgmt3 { get; set; }
        public List<Recon_Attributes_EWB.totalList> ReportMgmt4 { get; set; }
        public List<Recon_Attributes_EWB.totalList> ReportMgmt5 { get; set; }
     

    }
}
 