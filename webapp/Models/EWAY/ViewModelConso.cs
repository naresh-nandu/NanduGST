using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.EWAY
{
    public class ViewModelConso
    {
        public class EwayConsoList
        {
            public int consewbid { get; set; }
            public string vehicleNo { get; set; }
            public string fromPlace { get; set; }
            public string transMode { get; set; }
            public string transDocNo { get; set; }
            public string transDocDate { get; set; }

            public string fromState { get; set; }
            public string ewbNo { get; set; }
            public Nullable<byte> rowstatus { get; set; }
            public string sourcetype { get; set; }
            public string cEwbNo { get; set; }
            public string referenceno { get; set; }
            public Nullable<int> fileid { get; set; }
            public string errormessage { get; set; }
            public Nullable<int> createdby { get; set; }
            public Nullable<System.DateTime> createddate { get; set; }
        }
        public class EwayConsoItems
        {
            public int tripsheetid { get; set; }
            public int consewbid { get; set; }
            public string ewbNo { get; set; }          
            public Nullable<int> createdby { get; set; }
            public Nullable<System.DateTime> createddate { get; set; }
       
        }
        public class ListViewmodel1
        {
            public List<ViewModelConso.EwayConsoList> master { get; set; }
            public List<ViewModelConso.EwayConsoItems> items { get; set; }
        }
    }
}
