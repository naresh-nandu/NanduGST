using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.EWAY
{
    public class ViewModel
    {
        public class EwayList
        {
            public int ewbid { get; set; }
            public string supplyType { get; set; }
            public string subSupplyType { get; set; }
            public string docType { get; set; }
            public string genmode { get; set; }
            public string usertype { get; set; }
            public string usergstin { get; set; }
            public string userid { get; set; }
            public string docNo { get; set; }
            public string docDate { get; set; }
            public string fromGstin { get; set; }
            public string fromTrdName { get; set; }
            public string fromAddr1 { get; set; }
            public string fromAddr2 { get; set; }
            public string fromPlace { get; set; }
            public Nullable<int> fromPinCode { get; set; }
            public Nullable<int> fromStateCode { get; set; }
            public string toGstin { get; set; }
            public string toTrdName { get; set; }
            public string toAddr1 { get; set; }
            public string toAddr2 { get; set; }
            public string toPlace { get; set; }
            public Nullable<int> toPincode { get; set; }
            public Nullable<int> toStateCode { get; set; }
            public Nullable<decimal> totalValue { get; set; }
            public Nullable<decimal> cgstValue { get; set; }
            public Nullable<decimal> sgstValue { get; set; }
            public Nullable<decimal> igstValue { get; set; }
            public Nullable<decimal> cessValue { get; set; }
            public Nullable<int> transMode { get; set; }
            public string transDistance { get; set; }
            public string transporterId { get; set; }
            public string transporterName { get; set; }
            public string transDocNo { get; set; }
            public string transDocDate { get; set; }
            public string vehicleNo { get; set; }
            public string validUpto { get; set; }
            public string ewayBillNo { get; set; }
            public string ewayBillDate { get; set; }

        }
        public class EwayItems
        {
            public int itmsid { get; set; }
            public int ewbid { get; set; }
            public string productName { get; set; }
            public string productDesc { get; set; }
            public string hsnCode { get; set; }
            public Nullable<decimal> quantity { get; set; }
            public string qtyUnit { get; set; }
            public Nullable<decimal> taxableAmount { get; set; }
            public Nullable<decimal> cgstRate { get; set; }
            public Nullable<decimal> sgstRate { get; set; }
            public Nullable<decimal> igstRate { get; set; }
            public Nullable<decimal> cessRate { get; set; }
            public Nullable<decimal> rate { get; set; }
            public string ewayBillNo { get; set; }
            public string ewayBillDate { get; set; }
            public Nullable<byte> rowstatus { get; set; }
            public string sourcetype { get; set; }
            public string referenceno { get; set; }
            public Nullable<int> createdby { get; set; }
            public Nullable<System.DateTime> createddate { get; set; }
            public string errorCodes { get; set; }
            public string errormessage { get; set; }
            public Nullable<int> fileid { get; set; }
        }
        public class ListViewmodel
        {
            public List<ViewModel.EwayList> master { get; set; }
            public List<ViewModel.EwayItems> items { get; set; }
        }

      
    }
}
