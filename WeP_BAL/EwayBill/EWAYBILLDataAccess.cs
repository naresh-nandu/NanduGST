using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeP_DAL;

namespace WeP_BAL
{
    public class EwaybillDataAccess
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public EwaybillDataAccess()
        {
            //
        }

        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }


        #region "EWB & CONSEWB & UPDVEHNO LIST"
        public static List<IDictionary> GetEWBList(string strGSTINNo, string strFromDate, string strToDate, int CustId, int BranchId, string strMode)
        {
            DataTable dtable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_EWB_GENERATION", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@userGSTIN", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@FromDate", strFromDate));
                    dCmd.Parameters.Add(new SqlParameter("@ToDate", strToDate));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@BranchId", BranchId));
                    dCmd.Parameters.Add(new SqlParameter("@Mode", strMode));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    da.Fill(dtable);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(dtable);
        }

        public static List<IDictionary> GetCONSEWBList(string strGSTINNo, string strFromDate, string strToDate, int CustId, int BranchId, string strMode)
        {
            DataTable dtable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_EWB_GENERATION_CONSOLIDATED", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@userGSTIN", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@FromDate", strFromDate));
                    dCmd.Parameters.Add(new SqlParameter("@ToDate", strToDate));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@BranchId", BranchId));
                    dCmd.Parameters.Add(new SqlParameter("@Mode", strMode));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    da.Fill(dtable);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(dtable);
        }

        public static List<IDictionary> GetUPDVEHNOList(string strGSTINNo, int CustId, string strMode)
        {
            DataTable dtable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_EWB_UPD_VEHICLENO", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@userGSTIN", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@Mode", strMode));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    da.Fill(dtable);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(dtable);
        }
        #endregion

        #region "EWB Report Userwise"
        public static DataSet GetEWBListUserwise(string userGSTIN, string Period, string toPeriod, string ewbNo, string cewbNo, string genGSTIN, string Type, int CustId, int UserId, string BranchId)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_EWB_Report_Userwise", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", userGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Period", Period));
                cmd.Parameters.Add(new SqlParameter("@toPeriod", toPeriod));
                cmd.Parameters.Add(new SqlParameter("@EwbNo", ewbNo));
                cmd.Parameters.Add(new SqlParameter("@CEwbNo", cewbNo));
                cmd.Parameters.Add(new SqlParameter("@genGSTIN", genGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Type", Type));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                cmd.Parameters.Add(new SqlParameter("@BranchId", BranchId));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return ds;
        }
        #endregion

        #region "Eway Bill Data Access"

        #region "get gstin"
        public  DataSet getGSTN(string EwbNo,out string usergstin)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                 
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "select * from tbl_ewb_generation where ewayBillNo = @EwbNo ";
                        sqlcmd.Parameters.AddWithValue("@EwbNo", EwbNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                           DataTable ds = new DataTable();
                            da.Fill(ds);
                            var gstin = ds.Rows[0]["userGSTIN"].ToString();
                            usergstin = gstin;
                            return ds.DataSet;
                    }
                }

            } 
        }
                #endregion

                public static DataSet getEWAY(string EwbNo, string gstin, string ActionData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                if (ActionData == "ExtendValidity")
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select top(1) t1.ewbid,t1.docNo,t1.supplyType,t1.subSupplyType,t1.docType,t1.docDate,t1.fromGstin,t1.fromTrdName,fromAddr1,t1.fromAddr2,t1.fromPlace,t1.fromPinCode,t1.fromStateCode,t1.toGstin, t1.toTrdName, t1.toAddr1, t1.toAddr2, t1.toPlace, t1.toPincode, t1.toStateCode, t1.totalValue, t1.cgstValue, t1.sgstValue, t1.igstValue, t1.cessValue, t1.transMode, t1.transDistance,t1.transporterName,t1.transporterId, t1.transDocNo, t1.transDocDate, t1.vehicleNo, t1.ewayBillNo, t1.ewayBillDate,t2.ExtnId,t2.validUpto from TBL_EWB_GENERATION t1 Inner Join TBL_EWB_UPDATE_EXTEND_VALIDITY t2  On t1.ewayBillNo = t2.EwbNo where t1.ewayBillNo = @EwbNo AND t2.EwbNo = @EwbNo ORDER BY t2.ExtnId desc";
                        sqlcmd.Parameters.AddWithValue("@EwbNo", EwbNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                    }
                }
                else if (ActionData == "UpdateTransporter")
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select top(1) t1.ewbid,t1.docNo,t1.supplyType,t1.subSupplyType,t1.docType,t1.docDate,t1.fromGstin,t1.fromTrdName,fromAddr1,t1.fromAddr2,t1.fromPlace,t1.fromPinCode,t1.fromStateCode,t1.toGstin, t1.toTrdName, t1.toAddr1, t1.toAddr2, t1.toPlace, t1.toPincode, t1.toStateCode, t1.totalValue, t1.cgstValue, t1.sgstValue, t1.igstValue, t1.cessValue, t1.transMode, t1.transDistance, t1.transDocNo, t1.transDocDate, t1.vehicleNo, t1.validUpto, t1.ewayBillNo, t1.ewayBillDate, t2.TransId as transporterId,t2.Tid,t2.transporterName as transporterName from TBL_EWB_GENERATION t1 Inner Join TBL_EWB_UPDATE_TRANSPORTER t2  On t1.ewayBillNo = t2.EwbNo where t1.ewayBillNo = @EwbNo AND t2.EwbNo = @EwbNo ORDER BY t2.TId desc";
                        sqlcmd.Parameters.AddWithValue("@EwbNo", EwbNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                    }
                }
                else
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select TOP(1) * from TBL_EWB_GENERATION where ewayBillNo = @EwbNo";
                        sqlcmd.Parameters.AddWithValue("@EwbNo", EwbNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {                             
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
        }

        public static DataSet getEWAYList(string EwbNo)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select  * from TBL_EWB_UPDATE_VEHICLENO where EwbNo = @EwbNo and UPD_status <> '0'";
                    sqlcmd.Parameters.AddWithValue("@EwbNo", EwbNo);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }




        public static int getEwbId(string ewbNo, int CustId)
        {
            int ewbId = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where ewayBillNo = @EwbNo and CustId = @CustId";
                    sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            ewbId = Convert.ToInt32(dt.Rows[0]["ewbid"]);
                        }
                    }
                }
            }
            return ewbId;
        }

        public static int getHSNDetails(int ewbId, int CustId)
        {
            int hsnCode = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select TOP(1) * from TBL_EWB_GENERATION_ITMS where ewbid= @EwbId and CustId = @CustId";
                    sqlcmd.Parameters.AddWithValue("@EwbId", ewbId);
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            hsnCode = Convert.ToInt32(dt.Rows[0]["hsnCode"]);
                        }
                    }
                }
            }
            return hsnCode;
        }
        public static int getHSNCount(int ewbId, int CustId)
        {
            int hsnCount = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select Count(hsnCode) As HSNCODE from TBL_EWB_Generation_ITMS where ewbid= @EwbId and CustId = @CustId";
                    sqlcmd.Parameters.AddWithValue("@EwbId", ewbId);
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        hsnCount = Convert.ToInt32(dt.Rows[0]["HSNCODE"]);
                    }
                }
            }
            return hsnCount;
        }
        #endregion

        #region "Consolidation Eway Bill Data Access"
        public static DataSet getCEWAY(string cewbNo)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select TOP(1) * from TBL_EWB_GEN_CONSOLIDATED where cEwbNo = @cEwbNo";
                    sqlcmd.Parameters.AddWithValue("@cEwbNo", cewbNo);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public static void getConsewbid(string cewbNo, out int consewbid)
        {
            int cewbid = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_EWB_GEN_CONSOLIDATED where cEwbNo = @cEwbNo";
                    sqlcmd.Parameters.AddWithValue("@cEwbNo", cewbNo);
                    using (SqlDataAdapter adt1 = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt1 = new DataTable();
                        adt1.Fill(dt1);
                        if (dt1.Rows.Count > 0)
                        {
                            cewbid = Convert.ToInt32(dt1.Rows[0]["consewbid"]);
                        }
                    }
                }
            }
            consewbid = cewbid;
        }

        public static DataSet getCEWAYList(int consewbid)
        {
            StringBuilder myBuilder = new StringBuilder();
            DataSet das = new DataSet();
            DataSet ds = new DataSet();
            string ewbNo = "";

            if (consewbid != 0)
            {
                das = getConsolidatedTripsheet(consewbid);
            }

            if (das.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < das.Tables[0].Rows.Count; i++)
                {
                    myBuilder.Append("'" + das.Tables[0].Rows[i]["ewbNo"].ToString() + "',");
                }
                ewbNo = myBuilder.ToString();
                ewbNo = ewbNo.TrimEnd(',');
            }

            if (ewbNo != null)
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where ewayBillNo in ( @EwbNo )";
                        sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
            }
            return ds;
        }
        public static DataSet getConsolidatedTripsheet(int consewbid)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET where consewbid = @consewbid";
                    sqlcmd.Parameters.AddWithValue("@consewbid", consewbid);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }
        #endregion

        #region "DashBoard Data Access"
        public static DataSet getDaashboardSummary(string FP, string GSTIN, int Custid, int Userid)
        {
            DataSet ds = new DataSet();
            if (Con.State == ConnectionState.Closed)
            {
                Con.Open();
            }
            SqlCommand sqlComm = new SqlCommand("usp_Retrieve_EWB_Dashboard_SUMMARY", Con);
            sqlComm.Parameters.AddWithValue("@userGstin", GSTIN);
            sqlComm.Parameters.AddWithValue("@FP", FP);
            sqlComm.Parameters.AddWithValue("@CustId", Custid);
            sqlComm.Parameters.AddWithValue("@UserId", Userid);
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlComm;
            da.Fill(ds);
            Con.Close();
            return ds;
        }
        #endregion

        #region "Cancel Data Insert"
        public static void Insert_Cancel_SuccessData(string userGSTIN, string status, int cancelRsnCode, string cancelRmrk, string ewbNo, string cancelDate, int custid, int userid)
        {
            try
            {


                int rowstatus1 = Check_ewyBillNo(ewbNo, "GETEWAY");
                int rowstatus2 = Check_ewyBillNo(ewbNo, "GENERATION");
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                if (rowstatus1 == 1 || rowstatus2 == 1)
                {
                    SqlCommand cm = new SqlCommand();
                    cm.Parameters.Clear();
                    cm.Parameters.Add("@cancelRsnCode", SqlDbType.Int).Value = cancelRsnCode;
                    cm.Parameters.Add("@cancelRmrk", SqlDbType.NVarChar).Value = cancelRmrk;
                    cm.Parameters.Add("@cancelDate", SqlDbType.NVarChar).Value = cancelDate;
                    cm.Parameters.Add("@status", SqlDbType.NVarChar).Value = "CNL";
                    cm.Parameters.Add("@CAN_errorCodes", SqlDbType.NVarChar).Value = "";
                    cm.Parameters.Add("@CAN_errorDescription", SqlDbType.NVarChar).Value = "";
                    cm.Parameters.Add("@custid", SqlDbType.Int).Value = custid;
                    cm.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = userid;
                    cm.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    SQLHelper.UpdateTable("TBL_EWB_GENERATION", "ewayBillNo", ewbNo, cm, Con);
                    Con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static void Insert_Cancel_ErrorData(string strewbNo, string status, string errorCode, string errorDesc, int custid, int userid)
        {
            try
            {
                int rowstatus1 = Check_ewyBillNo(strewbNo, "GETEWAY");
                int rowstatus2 = Check_ewyBillNo(strewbNo, "GENERATION");
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                if (rowstatus1 == 1 || rowstatus2 == 1)
                {
                    SqlCommand cm = new SqlCommand();
                    cm.Parameters.Clear();
                    cm.Parameters.Add("@CAN_errorCodes", SqlDbType.NVarChar).Value = errorCode;
                    cm.Parameters.Add("@CAN_errorDescription", SqlDbType.NVarChar).Value = errorDesc;
                    cm.Parameters.Add("@custid", SqlDbType.Int).Value = custid;
                    cm.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = userid;
                    cm.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    SQLHelper.UpdateTable("TBL_EWB_GENERATION", "ewayBillNo", strewbNo, cm, Con);
                }
                Con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        #endregion

        #region "Reject Data Insert"
        public static void Insert_Reject_SuccessData(string userGSTIN, string status, string ewbNo, string rejectDate, int custid, int userid)
        {
            try
            {
                int rowstatus = Check_ewyBillNo(ewbNo, "OTHERPARTY");
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                if (rowstatus == 1)
                {
                    SqlCommand cm = new SqlCommand();
                    cm.Parameters.Clear();
                    cm.Parameters.Add("@ewbRejectedDate", SqlDbType.NVarChar).Value = rejectDate;
                    cm.Parameters.Add("@rejectStatus", SqlDbType.NVarChar).Value = "Y";
                    cm.Parameters.Add("@EWB_status", SqlDbType.NVarChar).Value = status;
                    cm.Parameters.Add("@EWB_errorCodes", SqlDbType.NVarChar).Value = "";
                    cm.Parameters.Add("@EWB_errorDescription", SqlDbType.NVarChar).Value = "";
                    cm.Parameters.Add("@custid", SqlDbType.Int).Value = custid;
                    cm.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = userid;
                    cm.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    SQLHelper.UpdateTable("TBL_EWB_GENERATION", "ewayBillNo", ewbNo, cm, Con);
                }
                Con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static void Insert_Reject_ErrorData(string strEwbNo, string status, string errorCode, string errorDesc, int custid, int userid)
        {
            try
            {
                int rowstatus = Check_ewyBillNo(strEwbNo, "OTHERPARTY");
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                if (rowstatus == 1)
                {

                    SqlCommand cm = new SqlCommand();
                    cm.Parameters.Clear();
                    cm.Parameters.Add("@EWB_status", SqlDbType.NVarChar).Value = status;
                    cm.Parameters.Add("@EWB_errorCodes", SqlDbType.NVarChar).Value = errorCode;
                    cm.Parameters.Add("@EWB_errorDescription", SqlDbType.NVarChar).Value = errorDesc;
                    cm.Parameters.Add("@custid", SqlDbType.Int).Value = custid;
                    cm.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = userid;
                    cm.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    SQLHelper.UpdateTable("TBL_EWB_GENERATION", "ewayBillNo", strEwbNo, cm, Con);
                }
                Con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        #endregion

        #region "Checking Either ewayBillNo exist or not "
        public static int Check_ewyBillNo(string ewbNo, string Mode)
        {
            int status = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        switch (Mode)
                        {
                            case "GETEWAY":
                                sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where ewayBillNo = @EwbNo and flag = 'G'";
                                sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                                break;
                            case "GENERATION":
                                sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where ewayBillNo = @EwbNo and flag is null";
                                sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                                break;
                            case "TRANS":
                                sqlcmd.CommandText = "Select * from TBL_EWB_Transporter where ewbNo = @EwbNo and flag is 'D'";
                                sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                                break;
                            case "TRANSGSTIN":
                                sqlcmd.CommandText = "Select * from TBL_EWB_Transporter where ewbNo = @EwbNo and flag is 'G'";
                                sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                                break;
                            case "OTHERPARTY":
                                sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where ewayBillNo = @EwbNo and flag = 'O'";
                                sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                                break;
                            case "CEWAY":
                                sqlcmd.CommandText = "Select * from TBL_EWB_GEN_CONSOLIDATED where cEwbNo = @EwbNo and flag = 'G'";
                                sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                                break;
                        }
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            adt.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                status = 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return status;
        }
        #endregion

        #region "Customer Management Linkage"
        public static DataSet getCustomerList(string strGSTIN, int CustId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_Buyer where GSTINno like '%@GSTINNo%' and CustomerId = @CustId and RowStatus = 1";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTIN);
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }
        #endregion

        #region "Supplier Management Linkage"
        public static DataSet getSupplierList(string strGSTIN, int CustId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_Supplier where GSTINno like '%@GSTINNo%' and CustomerId = @CustId and RowStatus = 1";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTIN);
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }
        #endregion

        #region "ewbNo Auto Populate Linkage"
        public static DataSet getewbNoList(string strGSTIN, string Prefix, int CustId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where ewayBillNo like '%@Prefix%' and userGSTIN = @GSTINNo and CustId = @CustId and (flag is null or flag='G') and status !='CAN'";
                    sqlcmd.Parameters.AddWithValue("@Prefix", Prefix);
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTIN);
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }
        #endregion

        #region "Retrieving Cancel Data Of Eway Bill"

        public static List<IDictionary> Retrieve_EWAYBILL_CancelData(string strGSTINNo, int CustId)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * From TBL_EWB_Generation Where userGSTIN = @GSTINNo and isnull(ewayBillNo,'') <> '' and isnull(ewayBillDate,'') <> '' and (flag='G' OR isnull(flag,'')='') and CustId = @CustId order by 1 desc";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        #endregion

        #region "eway bill Generation"

        public static void ewb_insert(string strGSTINNo, string supplyType, string subsupplyType, string documentType,
             string dispatchName, string dispatchGstin, string dispatchAdd1, string dispatchAdd2, string dispatchCity,
             string dispatchStateCode, string actFromStateCode, string dispatchPinCode, string ShipToName, string ShipToGstin, string ShipToAdd1,
             string ShipToAdd2, string ShipToCity, string ShipToStateCode, string actToStateCode, string ShipToPinCode, string transMode,
             string transName, string transId, string transdocNo, string transdocDate, string approDistance, string vehicleType, string ActDispatchFromGSTIN, string ActDispatchFromTradeName, string ActShipToGSTIN, string ActShipToTradeName, string TransactionType,
        string vehicleNo, string docNo, string docDate, string hsn, string itemDesc, string productName, string uqc,
             decimal taxValue, decimal Igst, decimal Cgst, decimal Sgst, decimal Cess, decimal Qty, decimal Unitprice,
             decimal totaligstvalue, decimal totalcgstvalue, decimal totalsgstvalue, decimal totalcessvalue, decimal totaltexablevalue,
             decimal totainvvalue,decimal cessnonadvolval,decimal otherval, string refrencenumber, string createdby, int branchId, string invType, string subsupplyDesc)
        {

            try
            {

                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Insert_EWAYBILL_GENERATION_EXT]", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.Add(new SqlParameter("@usergstin", strGSTINNo));
                cmd.Parameters.Add(new SqlParameter("@supplyType", supplyType));
                cmd.Parameters.Add(new SqlParameter("@subSupplyType", subsupplyType));
                cmd.Parameters.Add(new SqlParameter("@docType", documentType));
                cmd.Parameters.Add(new SqlParameter("@docDate", docDate));
                cmd.Parameters.Add(new SqlParameter("@docNo", docNo));
                cmd.Parameters.Add(new SqlParameter("@invType", invType));
                cmd.Parameters.Add(new SqlParameter("@transactionType", TransactionType));
                cmd.Parameters.Add(new SqlParameter("@subsupplyDesc", subsupplyDesc));
                cmd.Parameters.Add(new SqlParameter("@fromGstin", dispatchGstin));
                cmd.Parameters.Add(new SqlParameter("@fromTrdName", dispatchName));
                cmd.Parameters.Add(new SqlParameter("@fromAddr1", dispatchAdd1));
                cmd.Parameters.Add(new SqlParameter("@fromAddr2", dispatchAdd2));
                cmd.Parameters.Add(new SqlParameter("@fromPincode", dispatchPinCode));
                cmd.Parameters.Add(new SqlParameter("@fromPlace", dispatchCity));
                cmd.Parameters.Add(new SqlParameter("@fromStateCode", dispatchStateCode));
                cmd.Parameters.Add(new SqlParameter("@actfromStateCode", actFromStateCode));

                cmd.Parameters.Add(new SqlParameter("@toGstin", ShipToGstin));
                cmd.Parameters.Add(new SqlParameter("@toTrdName", ShipToName));
                cmd.Parameters.Add(new SqlParameter("@toAddr1", ShipToAdd1));
                cmd.Parameters.Add(new SqlParameter("@toAddr2", ShipToAdd2));
                cmd.Parameters.Add(new SqlParameter("@toStateCode", ShipToStateCode));
                cmd.Parameters.Add(new SqlParameter("@acttoStateCode", actToStateCode));
                cmd.Parameters.Add(new SqlParameter("@toPlace", ShipToCity));
                cmd.Parameters.Add(new SqlParameter("@toPincode", ShipToPinCode));

                cmd.Parameters.Add(new SqlParameter("@transMode", transMode));
                cmd.Parameters.Add(new SqlParameter("@transDistance", approDistance));
                cmd.Parameters.Add(new SqlParameter("@transporterId", transId));
                cmd.Parameters.Add(new SqlParameter("@transporterName", transName));
                cmd.Parameters.Add(new SqlParameter("@transDocNo", transdocNo));
                cmd.Parameters.Add(new SqlParameter("@transDocDate", transdocDate));
                cmd.Parameters.Add(new SqlParameter("@vehicleNo", vehicleNo.ToUpper()));
                if (string.IsNullOrEmpty(vehicleType))
                {
                    vehicleType = "";
                }
                cmd.Parameters.Add(new SqlParameter("@vehicleType", vehicleType));
                cmd.Parameters.Add(new SqlParameter("@dispatchFromGSTIN", ActDispatchFromGSTIN));
                cmd.Parameters.Add(new SqlParameter("@dispatchFromTradeName", ActDispatchFromTradeName));
                cmd.Parameters.Add(new SqlParameter("@shipToGSTIN", ActShipToGSTIN));
                cmd.Parameters.Add(new SqlParameter("@shipToTradeName", ActShipToTradeName));
                cmd.Parameters.Add(new SqlParameter("@totalValue", totaltexablevalue));
                cmd.Parameters.Add(new SqlParameter("@totalInvValue", totainvvalue));
                cmd.Parameters.Add(new SqlParameter("@cgstValue", totalcgstvalue));
                cmd.Parameters.Add(new SqlParameter("@sgstValue", totalsgstvalue));
                cmd.Parameters.Add(new SqlParameter("@igstValue", totaligstvalue));
                cmd.Parameters.Add(new SqlParameter("@cessValue", totalcessvalue));
                cmd.Parameters.Add(new SqlParameter("@otherValue", cessnonadvolval));
                cmd.Parameters.Add(new SqlParameter("@cessNonAdvolValue", otherval));

                cmd.Parameters.Add(new SqlParameter("@productName", productName));
                cmd.Parameters.Add(new SqlParameter("@productDesc", itemDesc));
                cmd.Parameters.Add(new SqlParameter("@hsnCode", hsn));
                cmd.Parameters.Add(new SqlParameter("@quantity", Qty));
                cmd.Parameters.Add(new SqlParameter("@qtyUnit", uqc));
                cmd.Parameters.Add(new SqlParameter("@taxableAmount", taxValue));
                cmd.Parameters.Add(new SqlParameter("@cgstRate", Cgst));
                cmd.Parameters.Add(new SqlParameter("@sgstRate", Sgst));
                cmd.Parameters.Add(new SqlParameter("@igstRate", Igst));
                cmd.Parameters.Add(new SqlParameter("@cessRate", Cess));

                cmd.Parameters.Add(new SqlParameter("@Referenceno", refrencenumber));
                cmd.Parameters.Add(new SqlParameter("@createdby", createdby));
                cmd.Parameters.Add(new SqlParameter("@branchId", branchId));
                cmd.ExecuteNonQuery();
                Con.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        #endregion

        #region "Consolidate Eway Bill Insreting"
        public static void cewb_insert(string strGSTIN, string transMode, string transDocNo, string transDocDate, string fromPlace, string fromState, string vechicleNo, string ewbNo, string refNo, string createdBy, int branchId)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("[dbo].[usp_Insert_EWAYBILL_GENERATION_CONSOLIDATED_EXT]", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", strGSTIN));
                cmd.Parameters.Add(new SqlParameter("@vehicleNo", vechicleNo));
                cmd.Parameters.Add(new SqlParameter("@fromPlace", fromPlace));
                cmd.Parameters.Add(new SqlParameter("@transMode", transMode));
                cmd.Parameters.Add(new SqlParameter("@transDocNo", transDocNo));
                cmd.Parameters.Add(new SqlParameter("@transDocDate", transDocDate));
                cmd.Parameters.Add(new SqlParameter("@fromState", fromState));
                cmd.Parameters.Add(new SqlParameter("@ewbNo", ewbNo));
                cmd.Parameters.Add(new SqlParameter("@referenceno", refNo));
                cmd.Parameters.Add(new SqlParameter("@createdby", createdBy));
                cmd.Parameters.Add(new SqlParameter("@BranchId", branchId));

                cmd.ExecuteNonQuery();
                Con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion

        #region "Retrieving Vechicle Details For Updating"

        public static DataSet getVechicleDetails(string strGSTIN, string ewbNo, int CustId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_EWB_UPDATE_VEHICLENO where EwbNo = @EwbNo and userGSTIN = @GSTINNo and CustId = @CustId";
                    sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTIN);
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }
        #endregion

        #region "Inserting Vechicle Details"
        public static void Insert_Vechicle_Details(string userGSTIN, string ewbNo, string vehicleNo, string fromPlace, string fromState, string reasonCode, string reasonRmrk, string transMode, string transDocNo, string transDocDate, string refNo, int createdBy)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Insert_EWAYBILL_UPDATE_VEHICLENO_EXT", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", userGSTIN));
                cmd.Parameters.Add(new SqlParameter("@EwbNo", ewbNo));
                cmd.Parameters.Add(new SqlParameter("@vehicleNo", vehicleNo));
                cmd.Parameters.Add(new SqlParameter("@fromPlace", fromPlace));
                cmd.Parameters.Add(new SqlParameter("@fromState", fromState));
                cmd.Parameters.Add(new SqlParameter("@reasonCode", reasonCode));
                cmd.Parameters.Add(new SqlParameter("@reasonRemarks", reasonRmrk));
                cmd.Parameters.Add(new SqlParameter("@transMode", transMode));
                cmd.Parameters.Add(new SqlParameter("@transDocNo", transDocNo));
                cmd.Parameters.Add(new SqlParameter("@transDocDate", transDocDate));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                cmd.Parameters.Add(new SqlParameter("@createdby", createdBy));
                cmd.Parameters.Add(new SqlParameter("@UPD_status", "1"));
                cmd.ExecuteNonQuery();

                Con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static void Insert_Vechicle_Details_SA(string userGSTIN, string ewbNo, string ewbDate, string vehicleNo, string fromPlace,
            string fromState, string reasonCode, string reasonRmrk, string transMode, string transDocNo, string transDocDate, int custId, int createdBy)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@userGSTIN", SqlDbType.NVarChar).Value = userGSTIN.Trim();
                cmd.Parameters.Add("@EwbNo", SqlDbType.NVarChar).Value = ewbNo.Trim();
                cmd.Parameters.Add("@vehUpdDate", SqlDbType.NVarChar).Value = ewbDate.Trim();
                cmd.Parameters.Add("@vehicleNo", SqlDbType.NVarChar).Value = vehicleNo.Trim();
                cmd.Parameters.Add("@FromPlace", SqlDbType.NVarChar).Value = fromPlace.Trim();
                cmd.Parameters.Add("@FromState", SqlDbType.NVarChar).Value = fromState.Trim();
                cmd.Parameters.Add("@ReasonCode", SqlDbType.NVarChar).Value = reasonCode.Trim();
                cmd.Parameters.Add("@ReasonRem", SqlDbType.NVarChar).Value = reasonRmrk.Trim();
                cmd.Parameters.Add("@TransMode", SqlDbType.NVarChar).Value = transMode.Trim();
                cmd.Parameters.Add("@TransDocNo", SqlDbType.NVarChar).Value = transDocNo.Trim();
                cmd.Parameters.Add("@TransDocDate", SqlDbType.NVarChar).Value = transDocDate.Trim();
                cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = custId;
                cmd.Parameters.Add("@createdby", SqlDbType.Int).Value = createdBy;
                cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@UPD_status", SqlDbType.NVarChar).Value = "1";
                SQLHelper.InsertIntoTable("TBL_EWB_UPDATE_VEHICLENO", cmd, Con);
                Con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        #endregion

        #region "eway bill Details Autopopulate for Generating ConsEway Bill"
        public static DataSet getEwbDetails(string prefix, string strGSTIN, int CustId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where ewayBillNo like '%@Prefix%' and userGSTIN = @GSTINNo and CustId = @CustId and flag is Null";
                    sqlcmd.Parameters.AddWithValue("@Prefix", prefix);
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTIN);
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }
        #endregion

        #region "Retrieve eway Billl List for Extend Validity
        public static DataSet getEwayBillList(string strGSTIN, string ewbNo, int CustId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where CustId = @CustId and flag is Null and ewaybillNo is not null";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }
        #endregion

        #region "EWB Report"

        public static DataSet Retrieve_EWB_Report(string userGSTIN, string Period, string ewbNo, string cewbNo, string genGSTIN, string Type, int CustId, int UserId, string BranchId)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_EWB_Report", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", userGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Period", Period));
                cmd.Parameters.Add(new SqlParameter("@EwbNo", ewbNo));
                cmd.Parameters.Add(new SqlParameter("@CEwbNo", cewbNo));
                cmd.Parameters.Add(new SqlParameter("@genGSTIN", genGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Type", Type));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                cmd.Parameters.Add(new SqlParameter("@BranchId", BranchId));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;
        }
        #endregion
        #region "EWB Report Status Active or INActive"
        public static DataSet GetStatusList(string userGSTIN, string Type, int CustId, int UserId, string BranchId)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_EWB_Report_Active_Expired_NotGenerated", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", userGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Type", Type));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                cmd.Parameters.Add(new SqlParameter("@BranchId", BranchId));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return ds;
        }
        #endregion



        public static int getGSTIN(string ewbNo,out string gstinnum,out string ewaybilldate)
        {
            string FromGSTIN = "", EwayBillDate = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where ewayBillNo = @EwbNo";
                    sqlcmd.Parameters.AddWithValue("@EwbNo", ewbNo);
                   
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            EwayBillDate = Convert.ToString(dt.Rows[0]["CreatedDate"]);
                            FromGSTIN = Convert.ToString(dt.Rows[0]["fromGstin"]);
                        }
                    }
                }
            }
            gstinnum = FromGSTIN;
            ewaybilldate = EwayBillDate;
            return 0;
        }


        public static List<IDictionary> Retrieve_EWAYBILL_BY_Other(string userGSTIN, string Period, string ewbNo, string cewbNo, string genGSTIN, string Type, int CustId)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_EWB_Report", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userGSTIN", userGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Period", Period));
                cmd.Parameters.Add(new SqlParameter("@EwbNo", ewbNo));
                cmd.Parameters.Add(new SqlParameter("@CEwbNo", cewbNo));
                cmd.Parameters.Add(new SqlParameter("@genGSTIN", genGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Type", Type));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds.Clear();
                da.Fill(ds);
                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

    }
}

