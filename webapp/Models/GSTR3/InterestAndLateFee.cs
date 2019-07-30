using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR3
{
    //public class InterestAndLateFee
    //{
    //    public TBL_GSTR3B_intr_ltfee TBL_GSTR3B_intr_ltfee { get; set; }
    //    public List<TBL_GSTR3B_intr_ltfee_intr_det> TBL_GSTR3B_intr_ltfee_intr_det { get; set; }

    //}
    //public partial class TBL_GSTR3B_intr_ltfee
    //{
    //    public int intr_ltfeeid { get; set; }
    //    public int gstr3bid { get; set; }
    //    public int gstinid { get; set; }
    //}
    public partial class TBL_GSTR3B_InterestAndLateFee
    {
        //public int intr_detid { get; set; }
        //public int intr_ltfeeid { get; set; }
        public int Id { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
        public string Fp { get; set; }
        public string Gstinid { get; set; }
    }
}