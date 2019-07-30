using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models
{
    public class Panlist
    {
      public  int PanId { get; set; }
      public string Panno { get; set; }
      public int CustId { get; set; }
      public string CompanyName { get; set; }
    }

    public class Gstinlist
    {
        public int GstinId { get; set; }
        public int CustId { get; set; }
        public string Gstinno { get; set; }
        public string Panno { get; set; }
    }
}