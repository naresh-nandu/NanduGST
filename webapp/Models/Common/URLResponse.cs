using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Common
{
    public class URLResponse
    {
        public List<Url> urls { get; set; }
        public string ek { get; set; }
        public int fc { get; set; }
    }

    public class Url
    {
        public string ul { get; set; }
        public int ic { get; set; }
        public string hash { get; set; }
    }
}