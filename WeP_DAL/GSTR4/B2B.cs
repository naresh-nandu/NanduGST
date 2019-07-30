using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTR4
{
    public class B2BItmDet
    {
        public int samt { get; set; }
        public int csamt { get; set; }
        public int rt { get; set; }
        public int txval { get; set; }
        public int camt { get; set; }
    }

    public class B2BItm
    {
        public int num { get; set; }
        public B2BItmDet itm_det { get; set; }
    }

    public class B2BInv
    {
        public List<B2BItm> itms { get; set; }
        public int val { get; set; }
        public int txval { get; set; }
        public string inum { get; set; }
        public string inv_typ { get; set; }
        public string pos { get; set; }
        public string idt { get; set; }
        public string rchrg { get; set; }
        public string chksum { get; set; }
    }

    public class B2B
    {
        public string ctin { get; set; }
        public string cfs { get; set; }
        public List<B2BInv> inv { get; set; }
    }

    public class B2BJson
    {
        public List<B2B> b2b { get; set; }
    }
}
