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
    
    public partial class usp_Retrieve_GSTR1EXP_EXT_Result
    {
        public Nullable<long> S_No { get; set; }
        public string GSTIN { get; set; }
        public string Return_Period { get; set; }
        public string Export_Type { get; set; }
        public string Invoice_No { get; set; }
        public string Invoice_Date { get; set; }
        public Nullable<decimal> Invoice_Value { get; set; }
        public string Shipping_Bill_No { get; set; }
        public string Shipping_Bill_Date { get; set; }
        public Nullable<decimal> Total_Taxable_Value { get; set; }
        public Nullable<decimal> Rate_of_tax { get; set; }
        public Nullable<decimal> IGST_Amount { get; set; }
        public string Error_Message { get; set; }
    }
}
