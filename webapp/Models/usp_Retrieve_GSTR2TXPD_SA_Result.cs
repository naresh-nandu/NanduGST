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
    
    public partial class usp_Retrieve_GSTR2TXPD_SA_Result
    {
        public Nullable<long> S_No { get; set; }
        public string POS { get; set; }
        public string Supply_Type { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Advance_Amount { get; set; }
        public Nullable<decimal> IGST_Amount { get; set; }
        public Nullable<decimal> CGST_Amount { get; set; }
        public Nullable<decimal> SGST_Amount { get; set; }
        public Nullable<decimal> Cess_Amount { get; set; }
    }
}
