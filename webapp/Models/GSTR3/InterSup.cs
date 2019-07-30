using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR3
{
    public class InterSup
    {
        public InterSup()
        {

            TBL_GSTR3B_inter_sup_uin_det = new List<TBL_GSTR3B_inter_sup_uin_det>();
            TBL_GSTR3B_inter_sup_comp_det = new List<TBL_GSTR3B_inter_sup_comp_det>();
            TBL_GSTR3B_inter_sup_unreg_det = new List<TBL_GSTR3B_inter_sup_unreg_det>();
        }
      
        public int Id { get; set; }
        public InterSupCommon Common { get; set; }

        public List<TBL_GSTR3B_inter_sup_uin_det> TBL_GSTR3B_inter_sup_uin_det { get; set; }

        public List<TBL_GSTR3B_inter_sup_comp_det> TBL_GSTR3B_inter_sup_comp_det { get; set; }

        public List<TBL_GSTR3B_inter_sup_unreg_det> TBL_GSTR3B_inter_sup_unreg_det { get; set; }
    }
    public  class InterSupCommon
    {
        public string Gstinid { get; set; }
        public string Fp { get; set; }
    }
    public  class TBL_GSTR3B_inter_sup_uin_det
    {
        public int Id { get; set; }
        //public int uin_detid { get; set; }
        public int inter_supid { get; set; }
        public int pos { get; set; }
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        //public int gstr3bid { get; set; }
        //public int gstinid { get; set; }
    }
    public  class TBL_GSTR3B_inter_sup_comp_det
    {
        public int Id { get; set; }
        //public int comp_detid { get; set; }
        //public int inter_supid { get; set; }
        public int pos { get; set; }
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        //public int gstr3bid { get; set; }
        //public int gstinid { get; set; }
    }
    public  class TBL_GSTR3B_inter_sup_unreg_det
    {
        public int Id { get; set; }
        //public int unreg_detid { get; set; }
        //public int inter_supid { get; set; }
        public int pos { get; set; }
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        //public int gstr3bid { get; set; }
        //public int gstinid { get; set; }
    }
}