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
    using System.Collections.Generic;
    
    public partial class TBL_EXT_GSTR2_TXI
    {
        public int txiid { get; set; }
        public string gstin { get; set; }
        public string fp { get; set; }
        public string flag { get; set; }
        public string chksum { get; set; }
        public string pos { get; set; }
        public string sply_ty { get; set; }
        public Nullable<decimal> rt { get; set; }
        public Nullable<decimal> adamt { get; set; }
        public Nullable<decimal> camt { get; set; }
        public Nullable<decimal> samt { get; set; }
        public Nullable<decimal> csamt { get; set; }
        public Nullable<decimal> iamt { get; set; }
        public Nullable<int> num { get; set; }
        public Nullable<byte> rowstatus { get; set; }
        public string sourcetype { get; set; }
        public string referenceno { get; set; }
        public Nullable<System.DateTime> createddate { get; set; }
        public string errormessage { get; set; }
        public Nullable<int> fileid { get; set; }
        public string CompCode { get; set; }
        public string UnitCode { get; set; }
        public string ReceivedBy { get; set; }
        public string ReceivedDate { get; set; }
        public Nullable<int> createdby { get; set; }
    }
}
