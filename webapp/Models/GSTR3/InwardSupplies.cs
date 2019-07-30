using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR3
{
    public class InwardSupplies
    {
        public InwardSupplies()
        {
            TBL_GSTR3B_inward_sup_isup_details = new List<TBL_GSTR3B_inward_sup_isup_details>();
        }
        public InwardSuppliesCommon Common { get; set; }
        public List<TBL_GSTR3B_inward_sup_isup_details> TBL_GSTR3B_inward_sup_isup_details { get; set; }
    }
    public  class InwardSuppliesCommon
    {
        public string  Gstinid { get; set; }
        public string  Fp { get; set; }
       
    }
    public partial class TBL_GSTR3B_inward_sup_isup_details
    {
        //public int isup_detid { get; set; }
        //public int inward_supid { get; set; }
        public int Id { get; set; }
        public string ty { get; set; }
        public decimal inter { get; set; }
        public decimal intra { get; set; }
       
    }
}