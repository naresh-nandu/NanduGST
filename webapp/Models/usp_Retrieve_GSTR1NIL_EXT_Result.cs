//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmartAdminMvc.Models
{
    using System;
    
    public partial class usp_Retrieve_GSTR1NIL_EXT_Result
    {
        public Nullable<long> S_No { get; set; }
        public string GSTIN { get; set; }
        public string Return_Period { get; set; }
        public string Supply_Type { get; set; }
        public Nullable<decimal> Nil_Rated_Supplies { get; set; }
        public Nullable<decimal> Exempted_Supplies { get; set; }
        public Nullable<decimal> Non_GST_Supplies { get; set; }
    }
}
