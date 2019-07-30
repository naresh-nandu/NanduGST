using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.EWAY
{
    public class PinDistance
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public int distance { get; set; }
        public string status { get; set; }
    }
}