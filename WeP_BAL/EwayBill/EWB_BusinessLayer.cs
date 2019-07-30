using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_BAL.EwayBill
{
    public class EWB_BusinessLayer
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public EWB_BusinessLayer()
        {

        }

        #region "Extend Validity of EwbNo"

        public static DataSet Retrieve_Extend_Validty_History(int custId)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Insert_Retrieve_EWB_Extend_Validity", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", ""));
                cmd.Parameters.Add(new SqlParameter("@EwbNo", ""));
                cmd.Parameters.Add(new SqlParameter("@VehicleNo", ""));
                cmd.Parameters.Add(new SqlParameter("@FromPlace", ""));
                cmd.Parameters.Add(new SqlParameter("@FromStateCode", ""));
                cmd.Parameters.Add(new SqlParameter("@RemainingDistance", ""));
                cmd.Parameters.Add(new SqlParameter("@TransDocNo", ""));
                cmd.Parameters.Add(new SqlParameter("@TransDocDate", ""));
                cmd.Parameters.Add(new SqlParameter("@TransMode", ""));
                cmd.Parameters.Add(new SqlParameter("@ExtnRsnCode", ""));
                cmd.Parameters.Add(new SqlParameter("@ExtnRmrk", ""));
                cmd.Parameters.Add(new SqlParameter("@Status", ""));
                cmd.Parameters.Add(new SqlParameter("@ErrorCode", ""));
                cmd.Parameters.Add(new SqlParameter("@ErrorDesc", ""));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", ""));
                cmd.Parameters.Add(new SqlParameter("@CustId", custId));
                cmd.Parameters.Add(new SqlParameter("@Mode", "R"));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;

        }

        public static void Insert_Extend_Validity(string userGSTIN, string ewbNo, string vehicleNo, string fromPlace, int fromStateCode, string RemainingDistance,
               string transDocNo, string transDocDate, string transMode, string extnRsnCode, string extnRmrk, int status, string errorCode, string errorDesc, int createdBy, int custId)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Insert_Retrieve_EWB_Extend_Validity", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", userGSTIN));
                cmd.Parameters.Add(new SqlParameter("@EwbNo", ewbNo));
                cmd.Parameters.Add(new SqlParameter("@VehicleNo", vehicleNo));
                cmd.Parameters.Add(new SqlParameter("@FromPlace", fromPlace));
                cmd.Parameters.Add(new SqlParameter("@FromStateCode", fromStateCode));
                cmd.Parameters.Add(new SqlParameter("@RemainingDistance", RemainingDistance));
                cmd.Parameters.Add(new SqlParameter("@TransDocNo", transDocNo));
                cmd.Parameters.Add(new SqlParameter("@TransDocDate", transDocDate));
                cmd.Parameters.Add(new SqlParameter("@TransMode", transMode));
                cmd.Parameters.Add(new SqlParameter("@ExtnRsnCode", extnRsnCode));
                cmd.Parameters.Add(new SqlParameter("@ExtnRmrk", extnRmrk));
                cmd.Parameters.Add(new SqlParameter("@Status", status));
                cmd.Parameters.Add(new SqlParameter("@ErrorCode", errorCode));
                cmd.Parameters.Add(new SqlParameter("@ErrorDesc", errorDesc));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", createdBy));
                cmd.Parameters.Add(new SqlParameter("@CustId", custId));
                cmd.Parameters.Add(new SqlParameter("@Mode", "I"));

                cmd.ExecuteNonQuery();

                Con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return;
        }

        #endregion

        #region "Update Transporter of EWB"

        public static DataSet Retrieve_Update_TransPorter_History(int custId)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Insert_Retrieve_EWB_Update_Transporter", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", ""));
                cmd.Parameters.Add(new SqlParameter("@EwbNo", ""));
                cmd.Parameters.Add(new SqlParameter("@TransId", ""));
                cmd.Parameters.Add(new SqlParameter("@UpdTransDate", ""));
                cmd.Parameters.Add(new SqlParameter("@Status", ""));
                cmd.Parameters.Add(new SqlParameter("@ErrorCode", ""));
                cmd.Parameters.Add(new SqlParameter("@ErrorDesc", ""));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", ""));
                cmd.Parameters.Add(new SqlParameter("@CustId", custId));
                cmd.Parameters.Add(new SqlParameter("@Mode", "R"));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;

        }

        public static void Insert_Update_TransPorter(string userGSTIN, string ewbNo, string transId, string updTransDate, int status, string errorCode, string errorDesc, int createdBy, int custId)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Insert_Retrieve_EWB_Update_Transporter", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", userGSTIN));
                cmd.Parameters.Add(new SqlParameter("@EwbNo", ewbNo));
                cmd.Parameters.Add(new SqlParameter("@TransId", transId));
                cmd.Parameters.Add(new SqlParameter("@UpdTransDate", updTransDate));
                cmd.Parameters.Add(new SqlParameter("@Status", status));
                cmd.Parameters.Add(new SqlParameter("@ErrorCode", errorCode));
                cmd.Parameters.Add(new SqlParameter("@ErrorDesc", errorDesc));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", createdBy));
                cmd.Parameters.Add(new SqlParameter("@CustId", custId));
                cmd.Parameters.Add(new SqlParameter("@Mode", "I"));

                cmd.ExecuteNonQuery();

                Con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return;
        }

        #endregion

        #region "Insert and Updating Otherparty EWB-GET API"
        public static void Insert_Update_Otherpary(string ewbNo, string ewbDate,string genMode, string genGstin, string docNo, string docDate, string fromGstin, string fromTrdName, string togstin, string toTrdName, string hsncode, string hsndesc, string status, string rejectStatus, decimal totalinvvalue, int CustId, int UserId)
        {

            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Insert_Update_EWB_OtherParty", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ewbNo",ewbNo));
                cmd.Parameters.Add(new SqlParameter("@ewbDate",ewbDate));
                cmd.Parameters.Add(new SqlParameter("@genMode",genMode));
                cmd.Parameters.Add(new SqlParameter("@genGstin",genGstin));
                cmd.Parameters.Add(new SqlParameter("@docNo", docNo));
                cmd.Parameters.Add(new SqlParameter("@docDate",docDate));
                cmd.Parameters.Add(new SqlParameter("@fromGstin",fromGstin));
                cmd.Parameters.Add(new SqlParameter("@fromTrdName",fromTrdName));
                cmd.Parameters.Add(new SqlParameter("@toGstin", togstin));
                cmd.Parameters.Add(new SqlParameter("@toTrdName",toTrdName));
                cmd.Parameters.Add(new SqlParameter("@totInvValue", totalinvvalue));
                cmd.Parameters.Add(new SqlParameter("@hsnCode",hsncode));
                cmd.Parameters.Add(new SqlParameter("@hsnDesc",hsndesc));
                cmd.Parameters.Add(new SqlParameter("@status",status));
                cmd.Parameters.Add(new SqlParameter("@rejectStatus",rejectStatus));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy",UserId));
                cmd.Parameters.Add(new SqlParameter("@CustId",CustId));

                cmd.ExecuteNonQuery();

                Con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return;
        }
        #endregion

        #region "Insert and Updating Regenerate Consolidate EWB"
        public static void Insert_EWB_CONS_REGENERATE(string cEwbNo, string REGENcEwbNo, string REGENStatus, string REGNDetails, string Status, string ErrorCode, string ErrorDesc,int CustId, int UserId)
        {

            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Insert_EWB__CONS_REGENARATE", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@cEwbNo", cEwbNo));
                cmd.Parameters.Add(new SqlParameter("@REGENcEwbNo", REGENcEwbNo));
                cmd.Parameters.Add(new SqlParameter("@REGENStatus", REGENStatus));
                cmd.Parameters.Add(new SqlParameter("@REGNDetails", REGNDetails));
                cmd.Parameters.Add(new SqlParameter("@Status", Status));
                cmd.Parameters.Add(new SqlParameter("@ErrorCode", ErrorCode));
                cmd.Parameters.Add(new SqlParameter("@ErrorDesc", ErrorDesc));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));

                cmd.ExecuteNonQuery();

                Con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return;
        }
        #endregion

        #region "Insert and Updating Transporter EWB-GET API"
        public static void Insert_Update_TransPorter_GETAPI(string ewbNo, string ewbDate,string genGstin,string userGstin, string docNo, string docDate,
            int pinCode, int stateCode, string place, string validUpto, string extendedTimes,string status, string rejectStatus,string flag,int CustId, int UserId)
        {

            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Insert_Update_EWB_Transporter", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ewbNo", ewbNo));
                cmd.Parameters.Add(new SqlParameter("@ewbDate", ewbDate));
                cmd.Parameters.Add(new SqlParameter("@genGstin", genGstin));
                cmd.Parameters.Add(new SqlParameter("@userGstin", userGstin));
                cmd.Parameters.Add(new SqlParameter("@docNo", docNo));
                cmd.Parameters.Add(new SqlParameter("@docDate", docDate));
                cmd.Parameters.Add(new SqlParameter("@delPinCode", pinCode));
                cmd.Parameters.Add(new SqlParameter("@delStateCode", stateCode));
                cmd.Parameters.Add(new SqlParameter("@delPlace", place));
                cmd.Parameters.Add(new SqlParameter("@validUpto", validUpto));
                cmd.Parameters.Add(new SqlParameter("@extendedTimes", extendedTimes));
                cmd.Parameters.Add(new SqlParameter("@status", status));
                cmd.Parameters.Add(new SqlParameter("@rejectStatus", rejectStatus));
                cmd.Parameters.Add(new SqlParameter("@flag", flag));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", UserId));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));

                int i = cmd.ExecuteNonQuery();

                Con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return;
        }
        #endregion

        #region "Checking Duplicate docNo and docDate"
        public static int duplicatedocNoChecking(string docNo, string docDate, int CustId)
        {
            int status = 0;
            SqlDataAdapter da = new SqlDataAdapter("select * from TBL_EWB_GENERATION where docNo= '" + docNo + "' and docDate= '" + docDate + "' and CustId = '" + CustId + "' and flag is Null", Con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                status = 1;
            }
            return status;
        }
        #endregion

        #region "EWayBill Email Sending"
        public static void EWayBillEmail(int custid, int branchid, out string EMAIL)
        {
            int status = 0;
            string email = "";
            SqlDataAdapter da = new SqlDataAdapter("select EwbEmailReqd from TBL_Cust_Settings where CustId= '" + custid + "' ", Con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int id = Convert.ToInt32(dt.Rows[0]["EwbEmailReqd"]);
                if(id == 1)
                {
                    status = 1;
                }
            }
            if (status == 1)
            {
                SqlDataAdapter da1 = new SqlDataAdapter("select emailId from TBL_Cust_LOCATION where branchId= '" + branchid + "' ", Con);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    email = dt1.Rows[0]["emailId"].ToString();
                }
            }
            EMAIL = email;
        }
        #endregion
    }
}
