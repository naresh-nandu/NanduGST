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
    
    public partial class usp_Retrieve_GSTR1DOCISSUE_EXT_Result
    {
        public Nullable<long> S_No { get; set; }
        public string GSTIN { get; set; }
        public string Return_Period { get; set; }
        public Nullable<int> Document_Number { get; set; }
        public string Nature_of_document { get; set; }
        public string From_serial_number { get; set; }
        public string To_serial_number { get; set; }
        public Nullable<int> Total_number { get; set; }
        public Nullable<int> Cancelled { get; set; }
        public Nullable<int> Net_issues { get; set; }
    }
}
