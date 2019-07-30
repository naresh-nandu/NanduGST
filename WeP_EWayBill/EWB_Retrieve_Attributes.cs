using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_EWayBill
{
    public partial class EWB_Retrieve_Attributes
    {
        public EWB_Retrieve_Attributes()
        {

        }
        public string ewayBillNo { get; set; }
        public string ewayBillDate { get; set; }
        public string DocNo { get; set; }
        public string fromGstin { get; set; }
        public string totalvalue { get; set; }
        public string fromTrdname { get; set; }
    }
}
