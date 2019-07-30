using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_EWayBill
{
    public class EwbResponseAttributes
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string authtoken { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string sek { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rek { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string hmac { get; set; }

        // ---------------------- ERROR -------------------------------------
        #region "ERROR"
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string error { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string errorcodes { get; set; }
        #endregion

        // --------------------- SUCCESS -----------------------------------
        #region "SUCCESS"
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ewayBillNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ewayBillDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string validUpto { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string vehUpdDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cEwbNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cEWBDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cancelDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ewbRejectedDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ewbNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string genMode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string userGstin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string supplyType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string subSupplyType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string docType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string docNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string docDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromGstin { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromTrdName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromAddr1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromAddr2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromPlace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromPincode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromTradeName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toTradeName { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromStateCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toGstin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toTrdName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toAddr1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toAddr2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toPlace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toPincode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toStateCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string totalValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cgstValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string sgstValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string igstValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cessValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string transporterId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string transporterName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string transDocNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string transMode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string transDocDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string transUpdateDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string remainingDistance { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string extnRsnCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string extnRemarks { get; set; }



        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string actualDist { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string noValidDays { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string extendedTimes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rejectStatus { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string tripSheetNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string vehicleNo { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromState { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string enteredDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ewbDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string getGstin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string delPinCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string delStateCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string delPlace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string genGstin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string hsncode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string hsndesc { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string totInvValue { get; set; }


        public List<tripSheet> tripSheetEwbBills { get; set; }
        public List<Items> ItemList { get; set; }

       public  List<VehiclListDetail> VehiclListDetails { get; set; }

        public class tripSheet
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string ewbNo { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string ewbDate { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string userGstin { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string docNo { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string docDate { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string assessValue { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string cgstValue { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string sgstValue { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string igstValue { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string cessValue { get; set; }

        }
    }
        public class Items
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string itemNo { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string productId { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string productName { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string productDesc { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string hsnCode { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string quantity { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string qtyUnit { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string cgstRate { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string sgstRate { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string igstRate { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string cessRate { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string cessAdvol { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string taxableAmount { get; set; }          

        }

        public class VehiclListDetail
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string ewbNo { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string updMode { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string vehicleNo { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string fromPlace { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string fromState { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string tripshtNo { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string userGSTINTransin { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string enteredDate { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string transMode { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string transDocNo { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string transDocDate { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string vehicleType { get; set; }
        }
        #endregion

    }

