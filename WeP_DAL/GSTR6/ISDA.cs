using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTR6
{
    public class ISDAInElglstDoclst
    {
        public string chksum { get; set; }
        public string isd_docty { get; set; }
        public string rdocnum { get; set; }
        public string rdocdt { get; set; }
        public string odocnum { get; set; }
        public string odocdt { get; set; }
        public string rcrdnum { get; set; }
        public string rcrddt { get; set; }
        public string ocrdnum { get; set; }
        public string ocrddt { get; set; }
        public double iamti { get; set; }
        public double iamts { get; set; }
        public double iamtc { get; set; }
        public double samts { get; set; }
        public double samti { get; set; }
        public double camti { get; set; }
        public double camtc { get; set; }
        public double csamt { get; set; }
    }

    public class ISDAElglstDoclst
    {
        public string chksum { get; set; }
        public string isd_docty { get; set; }
        public string rdocnum { get; set; }
        public string rdocdt { get; set; }
        public string odocnum { get; set; }
        public string odocdt { get; set; }
        public string rcrdnum { get; set; }
        public string rcrddt { get; set; }
        public string ocrdnum { get; set; }
        public string ocrddt { get; set; }
        public double iamti { get; set; }
        public double iamts { get; set; }
        public double iamtc { get; set; }
        public double samts { get; set; }
        public double samti { get; set; }
        public double camti { get; set; }
        public double camtc { get; set; }
        public double csamt { get; set; }
    }

    public class ISDAInElglst
    {
        public string typ { get; set; }
        public string cpty { get; set; }
        public string rcpty { get; set; }
        public string statecd { get; set; }
        public string rstatecd { get; set; }
        public List<ISDAInElglstDoclst> inelglstdoclst { get; set; }
    }

    public class ISDAElglst
    {
        public string typ { get; set; }
        public string cpty { get; set; }
        public string rcpty { get; set; }
        public string statecd { get; set; }
        public string rstatecd { get; set; }
        public List<ISDAElglstDoclst> elglstdoclst { get; set; }
    }

    public class ISDA
    {
        public List<ISDAElglst> elglst { get; set; }
        public List<ISDAInElglst> inelglst { get; set; }
    }

    public class ISDAJson
    {
        public List<ISDA> isda { get; set; }
    }
}
