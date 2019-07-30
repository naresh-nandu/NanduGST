using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR3
{
    public class OutwardSuppliesInwardSupplies
    {
        public OutwardSuppliesInwardSupplies()
        {
            TBL_GSTR3B_sup_det_isup_rev = new TBL_GSTR3B_sup_det_isup_rev();
            TBL_GSTR3B_sup_det_osup_det = new TBL_GSTR3B_sup_det_osup_det();
            TBL_GSTR3B_sup_det_osup_nil_exmp = new TBL_GSTR3B_sup_det_osup_nil_exmp();
            TBL_GSTR3B_sup_det_osup_nongst = new TBL_GSTR3B_sup_det_osup_nongst();
            TBL_GSTR3B_sup_det_osup_zero = new TBL_GSTR3B_sup_det_osup_zero();


        }
        public int Id { get; set; }
        public string gstinid { get; set; }
        public string fp { get; set; }
        public TBL_GSTR3B_sup_det_isup_rev TBL_GSTR3B_sup_det_isup_rev { get; set; }
        public TBL_GSTR3B_sup_det_osup_det TBL_GSTR3B_sup_det_osup_det { get; set; }
        public TBL_GSTR3B_sup_det_osup_nil_exmp TBL_GSTR3B_sup_det_osup_nil_exmp { get; set; }
        public TBL_GSTR3B_sup_det_osup_nongst TBL_GSTR3B_sup_det_osup_nongst { get; set; }
        public TBL_GSTR3B_sup_det_osup_zero TBL_GSTR3B_sup_det_osup_zero { get; set; }
    }
    public class TBL_GSTR3B_sup_det_isup_rev
    {
        //public int isup_rev_id { get; set; }
        //public int sup_detid { get; set; }
        public int Id { get; set; }
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
        //public Nullable<int> gstr3bid { get; set; }
        //public Nullable<int> gstinid { get; set; }
    }
    public partial class TBL_GSTR3B_sup_det_osup_det
    {
        //public int osup_detid { get; set; }
        //public int sup_detid { get; set; }
        public int Id { get; set; }
        public decimal txval { get; set; }
        public  decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
        //public Nullable<int> gstr3bid { get; set; }
        //public Nullable<int> gstinid { get; set; }
    }
    public partial class TBL_GSTR3B_sup_det_osup_nil_exmp
    {
        //public int osup_nil_exmpid { get; set; }
        //public int sup_detid { get; set; }
        public int Id { get; set; }
        public decimal txval { get; set; }
        //public Nullable<int> gstr3bid { get; set; }
        //public Nullable<int> gstinid { get; set; }
    }
    public partial class TBL_GSTR3B_sup_det_osup_nongst
    {
        //public int osup_nongstid { get; set; }
        //public int sup_detid { get; set; }
        public int Id { get; set; }
        public decimal txval { get; set; }
        //public Nullable<int> gstr3bid { get; set; }
        //public Nullable<int> gstinid { get; set; }
    }
    public partial class TBL_GSTR3B_sup_det_osup_zero
    {
        //public int osup_zeroid { get; set; }
        //public int sup_detid { get; set; }
        public int Id { get; set; }
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        public decimal csamt { get; set; }
        //public Nullable<int> gstr3bid { get; set; }
        //public Nullable<int> gstinid { get; set; }
    }
}