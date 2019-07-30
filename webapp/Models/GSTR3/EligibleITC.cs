using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR3
{
    public class EligibleItc
    {
        public EligibleItc()
        {
            TBL_GSTR3B_itc_elg_itc_avl = new List<TblGstr3BItcElgItcAvl>();
            TBL_GSTR3B_itc_elg_itc_inelg = new List<TblGstr3BItcElgItcInelg>();
            TBL_GSTR3B_itc_elg_itc_net = new TblGstr3BItcElgItcNet();
            TBL_GSTR3B_itc_elg_itc_rev = new List<TblGstr3BItcElgItcRev>();
        }
        public int Id { get; set; }
        public EligibleItcCommon EligibleITCCommon { get; set; }
        public List<TblGstr3BItcElgItcAvl> TBL_GSTR3B_itc_elg_itc_avl { get; set; }
        public List<TblGstr3BItcElgItcInelg> TBL_GSTR3B_itc_elg_itc_inelg { get; set; }
        public TblGstr3BItcElgItcNet TBL_GSTR3B_itc_elg_itc_net { get; set; }
        public List<TblGstr3BItcElgItcRev> TBL_GSTR3B_itc_elg_itc_rev { get; set; }
    }
    public class EligibleItcCommon
    {
        public string Gstinid { get; set; }
        public string Fp { get; set; }
    }

    public partial class TblGstr3BItcElgItcAvl
    {
        public int Id { get; set; }
        public string ty { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }
    public partial class TblGstr3BItcElgItcInelg
    {
        public int Id { get; set; }
        public string ty { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }
    public partial class TblGstr3BItcElgItcNet
    {
        public int Id { get; set; }
        public string ty { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }
    public partial class TblGstr3BItcElgItcRev
    {
        public int Id { get; set; }
        public string ty { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }
}