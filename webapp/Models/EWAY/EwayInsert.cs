using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmartAdminMvc.Models.EWAY
{
    public class EwayInsert
    {
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private readonly int outputparam;

        internal void InsertEwayDetails(string strGSTINNo, string vehichletype, string productame, string transportdate, int tranportmode, string transactiontype,
            decimal iGSTa, decimal cGSTa, string hSN, string itemdesc, decimal sGSTa, decimal cESSa,
            string subtype, decimal taxablevalue, decimal rate, string documenttype, string documentdate,
            string documnumber, string fname, string gstin, string address1, string address2, string state,
            string pincode, string city, string name1, string gstin1, string address11, string address22,
            string state1, string city1, string pincode1, string transportername, string transporterid,
            string transporterDocNo, string vehiclenumber, string approximatedistance, int userid,
            int iCustId, string uqc, decimal unitprice, decimal qty, Decimal totaligstvalue, Decimal totalcgstvalue,
            Decimal totalsgstvalue, Decimal totaltexablevalue, Decimal totalcessvalue, string refrencenumber, string createdby)
        {
            try
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Insert_EWAYBILL_GENERATION_EXT]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@usergstin", strGSTINNo));
                cmd.Parameters.Add(new SqlParameter("@supplyType", transactiontype));
                cmd.Parameters.Add(new SqlParameter("@subSupplyType", subtype));
                cmd.Parameters.Add(new SqlParameter("@docType", documenttype));
                cmd.Parameters.Add(new SqlParameter("@docDate", documentdate));
                cmd.Parameters.Add(new SqlParameter("@docNo", documnumber));
                cmd.Parameters.Add(new SqlParameter("@fromGstin", gstin));
                cmd.Parameters.Add(new SqlParameter("@fromTrdName", fname));
                cmd.Parameters.Add(new SqlParameter("@fromAddr1", address1));
                cmd.Parameters.Add(new SqlParameter("@fromAddr2", address2));
                cmd.Parameters.Add(new SqlParameter("@fromPincode", pincode));
                cmd.Parameters.Add(new SqlParameter("@fromPlace", city));
                cmd.Parameters.Add(new SqlParameter("@fromStateCode", state));
                cmd.Parameters.Add(new SqlParameter("@toGstin", gstin1));
                cmd.Parameters.Add(new SqlParameter("@toTrdName", name1));
                cmd.Parameters.Add(new SqlParameter("@toAddr1", address11));
                cmd.Parameters.Add(new SqlParameter("@toAddr2", address22));
                cmd.Parameters.Add(new SqlParameter("@toStateCode", state1));
                cmd.Parameters.Add(new SqlParameter("@toPlace", city1));
                cmd.Parameters.Add(new SqlParameter("@toPincode", pincode1));
                cmd.Parameters.Add(new SqlParameter("@totalValue", totaltexablevalue));
                cmd.Parameters.Add(new SqlParameter("@cgstValue", totalcgstvalue));
                cmd.Parameters.Add(new SqlParameter("@sgstValue", totalsgstvalue));

                cmd.Parameters.Add(new SqlParameter("@igstValue", totaligstvalue));
                cmd.Parameters.Add(new SqlParameter("@cessValue", totalcessvalue));
                cmd.Parameters.Add(new SqlParameter("@transMode", tranportmode));
                cmd.Parameters.Add(new SqlParameter("@transDistance", approximatedistance));
                cmd.Parameters.Add(new SqlParameter("@transporterId", transporterid));
                cmd.Parameters.Add(new SqlParameter("@transporterName", transportername));
                cmd.Parameters.Add(new SqlParameter("@transDocNo", transporterDocNo));
                cmd.Parameters.Add(new SqlParameter("@transDocDate", transportdate));
                cmd.Parameters.Add(new SqlParameter("@vehicleNo", vehiclenumber));
                cmd.Parameters.Add(new SqlParameter("@vehicleType", vehichletype));
                cmd.Parameters.Add(new SqlParameter("@productName", productame));
                cmd.Parameters.Add(new SqlParameter("@productDesc", itemdesc));
                cmd.Parameters.Add(new SqlParameter("@hsnCode", hSN));
                cmd.Parameters.Add(new SqlParameter("@quantity", qty));
                cmd.Parameters.Add(new SqlParameter("@qtyUnit", uqc));
                cmd.Parameters.Add(new SqlParameter("@taxableAmount", taxablevalue));
                cmd.Parameters.Add(new SqlParameter("@cgstRate", cGSTa));
                cmd.Parameters.Add(new SqlParameter("@sgstRate", sGSTa));
                cmd.Parameters.Add(new SqlParameter("@igstRate", iGSTa));
                cmd.Parameters.Add(new SqlParameter("@cessRate", cESSa));
                cmd.Parameters.Add(new SqlParameter("@Referenceno", refrencenumber));
                cmd.Parameters.Add(new SqlParameter("@createdby", createdby));
                cmd.ExecuteNonQuery();
                con.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        internal void EwayConsoEwayAdd(decimal consewbid, string ewbNo)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Insert_CONSO_EWAYBILL_Add]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@consewbid ", consewbid));
                cmd.Parameters.Add(new SqlParameter("@ewbNo ", ewbNo));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        internal int EwayBillDeleteConso(int id)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[Delete_Eway_consoBill]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@tripsheetid ", id));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }

        internal void InsertEwayConsoDetails(string vehicleNumber, string fromPlace, string transMode, string transDocDate, string fromState, string transDocNo, decimal ewbNo, string v1, string v2)
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Insert_EWAYBILL_GENERATION_CONSOLIDATED_EXT]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@vehicleNo ", vehicleNumber));
                cmd.Parameters.Add(new SqlParameter("@fromPlace ", fromPlace));
                cmd.Parameters.Add(new SqlParameter("@transMode ", transMode));
                cmd.Parameters.Add(new SqlParameter("@transDocNo ", transDocNo));
                cmd.Parameters.Add(new SqlParameter("@transDocDate ", transDocDate));
                cmd.Parameters.Add(new SqlParameter("@fromState ", fromState));
                cmd.Parameters.Add(new SqlParameter("@ewbNo ", ewbNo));
                cmd.Parameters.Add(new SqlParameter("@referenceno ", v1));
                cmd.Parameters.Add(new SqlParameter("@createdby ", v2));


                cmd.ExecuteNonQuery();
                con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        internal int EwayBillUpdate(int id, string ewayBill)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Update_EWAYBILL_CONSO_Items]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@tripsheetid ", id));
                cmd.Parameters.Add(new SqlParameter("@ewbNo ", ewayBill));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;

        }

        internal void EwayConsoDetailsUpdate(int consewbid, string vehicleNumber, string fromPlace, string transMode, string transDocDate, string fromState, string transDocNo)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Update_Conso_EWAYBILL_GENERATION_MASTER]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@consewbid ", consewbid));
                cmd.Parameters.Add(new SqlParameter("@vehicleNo ", vehicleNumber));
                cmd.Parameters.Add(new SqlParameter("@fromPlace ", fromPlace));
                cmd.Parameters.Add(new SqlParameter("@transMode ", transMode));
                cmd.Parameters.Add(new SqlParameter("@transDocNo ", transDocNo));
                cmd.Parameters.Add(new SqlParameter("@transDocDate ", transDocDate));
                cmd.Parameters.Add(new SqlParameter("@fromState ", fromState));
                cmd.ExecuteNonQuery();
                con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        internal int EwayItemDelete(int id, int mode)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_InsUpdDel_EwayBill_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@itmsid", id));
                cmd.Parameters.Add(new SqlParameter("@mode", mode));
                cmd.ExecuteNonQuery();
                con.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }
        internal int EwayRecordDelete(int id)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[Delete_Eway_Record]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@ewbid", id));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }


        internal void InsertEwayDetailsUpdated(string transportdate, string tranportmode,
         string transactiontype, string subtype, string documenttype, string documentdate,
         string documnumber, string fname, string gstin, string address1, string address2,
         string state, string pincode, string city, string name1, string gstin1, string address11,
         string address22, string state1, string city1, string pincode1, string transportername,
         string transporterid, string transporterDocNo, string vehiclenumber, string approximatedistance)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Update_EWAYBILL_GENERATION_Master", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@supplyType", transactiontype));
                cmd.Parameters.Add(new SqlParameter("@subSupplyType", subtype));
                cmd.Parameters.Add(new SqlParameter("@docType", documenttype));
                cmd.Parameters.Add(new SqlParameter("@docDate", documentdate));
                cmd.Parameters.Add(new SqlParameter("@docNo", documnumber));
                cmd.Parameters.Add(new SqlParameter("@fromGstin", gstin));
                cmd.Parameters.Add(new SqlParameter("@fromTrdName", fname));
                cmd.Parameters.Add(new SqlParameter("@fromAddr1", address1));
                cmd.Parameters.Add(new SqlParameter("@fromAddr2", address2));
                cmd.Parameters.Add(new SqlParameter("@fromPincode", pincode));
                cmd.Parameters.Add(new SqlParameter("@fromPlace", city));
                cmd.Parameters.Add(new SqlParameter("@fromStateCode", state));
                cmd.Parameters.Add(new SqlParameter("@toGstin", gstin1));
                cmd.Parameters.Add(new SqlParameter("@toTrdName", name1));
                cmd.Parameters.Add(new SqlParameter("@toAddr1", address11));
                cmd.Parameters.Add(new SqlParameter("@toAddr2", address22));
                cmd.Parameters.Add(new SqlParameter("@toStateCode", state1));
                cmd.Parameters.Add(new SqlParameter("@toPlace", city1));
                cmd.Parameters.Add(new SqlParameter("@toPincode", pincode1));
                cmd.Parameters.Add(new SqlParameter("@transMode", tranportmode));
                cmd.Parameters.Add(new SqlParameter("@transDistance", approximatedistance));
                cmd.Parameters.Add(new SqlParameter("@transporterId", transporterid));
                cmd.Parameters.Add(new SqlParameter("@transporterName", transportername));
                cmd.Parameters.Add(new SqlParameter("@transDocNo", transporterDocNo));
                cmd.Parameters.Add(new SqlParameter("@transDocDate", transportdate));
                cmd.Parameters.Add(new SqlParameter("@vehicleNo", vehiclenumber));

                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        internal void InsertEwayItems1(string productname, int ewbid, string hSN, string itemdesc, decimal qty, string uqc, decimal unitprice, 
            decimal rate, decimal taxablevalue, decimal iGSTa, decimal cESSa, decimal cGSTa, decimal sGSTa, string v1, string v2)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Insert_EWAYBILL_Items_Add]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@ewbid", ewbid));
                cmd.Parameters.Add(new SqlParameter("@productDesc", itemdesc));
                cmd.Parameters.Add(new SqlParameter("@productName", productname));
                cmd.Parameters.Add(new SqlParameter("@hsnCode", hSN));
                cmd.Parameters.Add(new SqlParameter("@quantity", qty));
                cmd.Parameters.Add(new SqlParameter("@qtyUnit", uqc));
                cmd.Parameters.Add(new SqlParameter("@cgstRate", cGSTa));
                cmd.Parameters.Add(new SqlParameter("@sgstRate", sGSTa));
                cmd.Parameters.Add(new SqlParameter("@igstRate", iGSTa));
                cmd.Parameters.Add(new SqlParameter("@cessRate", cESSa));
                cmd.Parameters.Add(new SqlParameter("@taxableAmount", taxablevalue));
                cmd.Parameters.Add(new SqlParameter("@Referenceno", v1));
                cmd.Parameters.Add(new SqlParameter("@createdby", v2));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        internal int EwayItemUpdate(string productname, int id, string hsncode, string itemdesc, decimal qty, string uqc, decimal unitprice, decimal txamt, decimal igsta, decimal cgsta, decimal sgsta, decimal cessa)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Updated_EwayBill_Items]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@itmsid", id));
                cmd.Parameters.Add(new SqlParameter("@productDesc", itemdesc));
                cmd.Parameters.Add(new SqlParameter("@productName", productname));
                cmd.Parameters.Add(new SqlParameter("@hsnCode", hsncode));
                cmd.Parameters.Add(new SqlParameter("@quantity", qty));
                cmd.Parameters.Add(new SqlParameter("@taxableAmount", txamt));

                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }





    }
}







