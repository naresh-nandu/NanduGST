using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL
{
    public class EwayAttributes
    {
        public EwayAttributes()
        {

        }


        public class Master
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
            public string CompanyName { get; set; }
            
           public string transactionType { get; set; }
        }

        public class MasterList
        {
            public int ewbVehUpdid { get; set; }
            public string userGSTIN { get; set; }
            public string EwbNo { get; set; }
            public string validUpto { get; set; }
            public string vehicleNo { get; set; }
            public string FromPlace { get; set; }
            public string FromState { get; set; }
            public string TransMode { get; set; }
            public string TransDocNo { get; set; }
            public string TransDocDate { get; set; }
            public string UPD_status { get; set; }
            public string ReasonCode { get; set; }
            public string ReasonRem { get; set; }
            public string UPD_errorCodes { get; set; }
            public string UPD_errorDescription { get; set; }

        }

        public class HsnList
        {
            public int hsnCode { get; set; }
            public int hsnCount { get; set; }
        }
        public class ConsolidatedMaster
        {
            public int consewbid { get; set; }
            public string cewbNo { get; set; }
            public string cewbDate { get; set; }
            public string vechileNo { get; set; }
            public string fromPlace { get; set; }
        }

        public class ConsolidatedMasterList
        {
            public int tripsheetid { get; set; }
            public int consewbid { get; set; }
            public string ewbNo { get; set; }
            public string ewbDate { get; set; }
            public string docNo { get; set; }
            public string docDate { get; set; }
            public string generatedBy { get; set; }
            public string toAddress { get; set; }

        }

        public class DashBoardSummary
        {

            public int gstinId { get; set; }
            public string gstinNo { get; set; }
            public string panNo { get; set; }
            public string companyName { get; set; }
            public int Generated { get; set; }
            public int Received { get; set; }
            public int Cancel { get; set; }
            public int RejectedByMe { get; set; }
            public int RejectedByCounterParty { get; set; }
        }
        public class DashBoardCount
        {
            public int Total_Generate { get; set; }
            public int Total_Received { get; set; }
            public int Total_Cancelled { get; set; }
            public int Total_RejectedByMe { get; set; }
            public int Total_RejectedByCounterParty { get; set; }

        }

        public class CustomerMgmt
        {
            public int buyerId { get; set; }
            public string buyerName { get; set; }
            public string gstinNo { get; set; }
            public int custId { get; set; }
            public string Address { get; set; }
        }

        public class SupplierMgmt
        {
            public int supplierId { get; set; }
            public string supplierName { get; set; }
            public string gstinNo { get; set; }
            public int custId { get; set; }
            public string Address { get; set; }
        }

        public class EwaybillMgmt
        {
            public string ewbNo { get; set; }
        }

        public class VehicleHistory
        {
            public string userGSTIN { get; set; }
            public string ewbNo { get; set; }
            public string vehicleNo { get; set; }
            public string fromPlace { get; set; }
            public string fromState { get; set; }
            public string reasonCode { get; set; }
            public string reasonRemark { get; set; }
            public string transDocNo { get; set; }
            public string transDocDate { get; set; }
            public string transMode { get; set; }
            public string vehupddt { get; set; }
            public string errorCode { get; set; }
            public string errorDesc { get; set; }
        }
        public class EwayDetailsAutoPopulate
        {
            public string ewbNo { get; set; }
            public string ewbDate { get; set; }
            public string generatedBy { get; set; }
            public string docNo { get; set; }
            public string docDate { get; set; }
            public string taxValue { get; set; }
            public string fromPlace { get; set; }
            public string toPlace { get; set; }
        }
    }

    public class EwayViewModel
    {
        public List<EwayAttributes.EwaybillMgmt> ewbNoMgmt { get; set; }
        public List<EwayAttributes.ConsolidatedMaster> ConsolidatedMaster { get; set; }
        public List<EwayAttributes.ConsolidatedMasterList> ConsolidatedMasterList { get; set; }
        public List<EwayAttributes.Master> Master { get; set; }
        public List<EwayAttributes.MasterList> MasterList { get; set; }
        public List<EwayAttributes.EwayDetailsAutoPopulate> ewbDetails { get; set; }
        public List<EwayAttributes.DashBoardSummary> DashBoardSummary { get; set; }
        public List<EwayAttributes.DashBoardCount> DashBoardCount { get; set; }
        public List<EwayAttributes.CustomerMgmt> CustMgmt { get; set; }
        public List<EwayAttributes.VehicleHistory> VehicleHistory { get; set; }
        public List<EwayAttributes.SupplierMgmt> SupMgmt { get; set; }

        public List<EwayAttributes.HsnList> hsnList { get; set; }

    }
}
