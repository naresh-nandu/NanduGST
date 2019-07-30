using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Download
{
    public class Nil_G
    {
        public string sply_ty { get; set; }
        public double expt_amt { get; set; }
        public double nil_amt { get; set; }
        public double ngsup_amt { get; set; }
    }

    public class Nil_S
    {
        public string sply_ty { get; set; }
        public double expt_amt { get; set; }
        public double nil_amt { get; set; }
        public double ngsup_amt { get; set; }
    }

    public class Nil
    {
        public List<Nil_G> g { get; set; }
        public List<Nil_S> s { get; set; }
    }

}
