using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTR6
{
    public class ISDInElglstDoclst
    {
        public string chksum { get; set; }
        public string isd_docty { get; set; }
        public string docnum { get; set; }
        public string docdt { get; set; }
        public string crdnum { get; set; }
        public string crddt { get; set; }
        public double iamti { get; set; }
        public double iamts { get; set; }
        public double iamtc { get; set; }
        public double samts { get; set; }
        public double samti { get; set; }
        public double camti { get; set; }
        public double camtc { get; set; }
        public double csamt { get; set; }
    }

    public class ISDElglstDoclst
    {
        public string chksum { get; set; }
        public string isd_docty { get; set; }
        public string docnum { get; set; }
        public string docdt { get; set; }
        public string crdnum { get; set; }
        public string crddt { get; set; }
        public double iamti { get; set; }
        public double iamts { get; set; }
        public double iamtc { get; set; }
        public double samts { get; set; }
        public double samti { get; set; }
        public double camti { get; set; }
        public double camtc { get; set; }
        public double csamt { get; set; }
    }

    public class ISDInElglst
    {
        public string typ { get; set; }
        public string cpty { get; set; }
        public string statecd { get; set; }
        public List<ISDInElglstDoclst> inelglstdoclst { get; set; }
    }

    public class ISDElglst
    {
        public string typ { get; set; }
        public string cpty { get; set; }
        public string statecd { get; set; }
        public List<ISDElglstDoclst> elglstdoclst { get; set; }
    }

    public class ISD
    {
        public List<ISDElglst> elglst { get; set; }
        public List<ISDInElglst> inelglst { get; set; }
    }

    public class ISDJson
    {
        public List<ISD> isd { get; set; }
    }
}
