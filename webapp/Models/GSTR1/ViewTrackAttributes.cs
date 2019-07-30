using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR1
{
    public class ViewTrackAttributes
    {
        public string valid { get; set; }
        public string mof { get; set; }
        public string dof { get; set; }
        public string ret_prd { get; set; }
        public string rtntype { get; set; }
        public string arn { get; set; }
        public string status { get; set; }

        public string EFiledlist { get; set; }
    }

    public class EFiledlist
    {
        public string valid { get; set; }
        public string mof { get; set; }
        public string dof { get; set; }
        public string ret_prd { get; set; }
        public string rtntype { get; set; }
        public string arn { get; set; }
        public string status { get; set; }
    }

    public class GstrViewTrackResponse
    {
        public List<EFiledlist> EFiledlist { get; set; }
    }
}