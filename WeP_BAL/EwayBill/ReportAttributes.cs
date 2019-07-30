using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.EwayBill
{
    public class ReportAttributes
    {
        public ReportAttributes()
        {

        }
        public class ewbList
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
            public string fromStateCode { get; set; }
            public string toGstin { get; set; }
            public string toTrdName { get; set; }
            public string toAddr1 { get; set; }
            public string toAddr2 { get; set; }
            public string toPlace { get; set; }
            public Nullable<int> toPincode { get; set; }
            public string toStateCode { get; set; }
            public Nullable<decimal> totalinvValue { get; set; }
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
            public string status { get; set; }
            public string cancelRsnCode { get; set; }
            public string cancelRmrk { get; set; }
            public string cancelewbDate { get; set; }
            public string genMode { get; set; }
            public string extendedTimes { get; set; }
            public string rejectStatus { get; set; }
            public string ewbRejectedDate { get; set; }
            public string Eway_Bill_Details { get; set; }
            public string Document_Details { get; set; }
            public string From_Details { get; set; }
            public string To_Details { get; set; }
            public string Transported_Details { get; set; }
            public string Vehicle_Details { get; set; }
            public string BranchName { get; set; }
            public decimal Taxable_Value { get; set; }
            public decimal Total_Invoice_Amount { get; set; }
            public decimal IGST_Amount { get; set; }
            public decimal CGST_Amount  { get; set; }
            public decimal SGST_Amount { get; set; }
            public decimal CESS_Amount { get; set; }
            public string Eway_Bill_Validity { get; set; }
            public string EwbNo { get; set; }
            public string ReasonCode { get; set; }
            public string ReasonRem { get; set; }
            public string vehUpdDate { get; set; }

        }

        public class Transporter
        {
            public int Transid { get; set; }
            public string ewbNo { get; set; }
            public string ewbDate { get; set; }
            public string status { get; set; }
            public string genGstin { get; set; }
            public string docDate { get; set; }
            public string docNo { get; set; }
            public string delPinCode { get; set; }
            public string delStateCode { get; set; }
            public string delPlace { get; set; }
            public string validUpto { get; set; }
            public string extendedTimes { get; set; }
            public string rejectStatus { get; set; }


        }
        public class consewbList
        {
            public string fromPlace { get; set; }
            public string fromState { get; set; }
            public string vehicleNo { get; set; }
            public string transMode { get; set; }
            public string transDocNo { get; set; }
            public string transDocDate { get; set; }
            public string cEwbNo { get; set; }
            public string cEWBDate { get; set; }
            public string Consolidate_Ewaybill_Details { get; set; }
            public string From_Details { get; set; }
            public string Transporter_Details { get; set; }
            public string Vehicle_No { get; set; }
            public string Branch_Name { get; set; }

        }
        public class extendValidity
        {
            public string userGstin { get; set; }
            public string ewbNo { get; set; }
            public string vehicleNo { get; set; }
            public string fromPlace { get; set; }
            public string fromStateCode { get; set; }
            public string reaminingDist { get; set; }
            public string transDocNo { get; set; }
            public string transDocDate { get; set; }
            public string transMode { get; set; }
            public string extnRsnCode { get; set; }
            public string extnRmrk { get; set; }
            public string errorCode { get; set; }
            public string errorDesc { get; set; }
        }
        public class updateTransporter
        {
            public string userGstin { get; set; }
            public string ewbNo { get; set; }
            public string transId { get; set; }
            public string updTransDate { get; set; }
            public string errorCode { get; set; }
            public string errorDesc { get; set; }
        }
    }

    public class ReportViewModel
    {
        public List<ReportAttributes.ewbList> ReportMgmt { get; set; }
        public List<ReportAttributes.consewbList> ConsReportMgmt { get; set; }
        public List<ReportAttributes.extendValidity> ValidityMgmt { get; set; }
        public List<ReportAttributes.Transporter> TransMgmt { get; set; }
        public List<ReportAttributes.updateTransporter> updTransMgmt { get; set; }
    }
}
