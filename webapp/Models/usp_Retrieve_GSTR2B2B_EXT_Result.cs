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
    
    public partial class usp_Retrieve_GSTR2B2B_EXT_Result
    {
        public Nullable<long> S_No { get; set; }
        public string GSTIN___UIN { get; set; }
        public string Invoice_No { get; set; }
        public string Invoice_Date { get; set; }
        public Nullable<decimal> Invoice_Value { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Taxable_Value { get; set; }
        public Nullable<decimal> IGST_Amount { get; set; }
        public Nullable<decimal> CGST_Amount { get; set; }
        public Nullable<decimal> SGST_Amount { get; set; }
        public Nullable<decimal> Cess_Amount { get; set; }
        public string POS { get; set; }
        public string Eligibility_for_ITC { get; set; }
        public Nullable<decimal> ITC_IGST_Amount { get; set; }
        public Nullable<decimal> ITC_CGST_Amount { get; set; }
        public Nullable<decimal> ITC_SGST_Amount { get; set; }
        public Nullable<decimal> ITC_Cess_Amount { get; set; }
    }
}
