using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTR4
{
    public class B2BAItmDet
    {
        public int csamt { get; set; }
        public int rt { get; set; }
        public int txval { get; set; }
        public int iamt { get; set; }
    }

    public class B2BAItm
    {
        public int num { get; set; }
        public B2BAItmDet itm_det { get; set; }
    }

    public class B2BAInv
    {
        public List<B2BAItm> itms { get; set; }
        public int val { get; set; }
        public string oinum { get; set; }
        public int txval { get; set; }
        public string inum { get; set; }
        public string inv_typ { get; set; }
        public string pos { get; set; }
        public string idt { get; set; }
        public string rchrg { get; set; }
        public string oidt { get; set; }
        public string chksum { get; set; }
    }

    public class B2BA
    {
        public string cfs { get; set; }
        public string ctin { get; set; }
        public List<B2BAInv> inv { get; set; }
    }

    public class B2BAJson
    {
        public List<B2BA> b2ba { get; set; }
    }
}
