using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.Security.Cryptography;
using SmartAdminMvc.Models.Common;

namespace SmartAdminMvc.Models.GSTR1
{
    public partial class Gstr1Retstatus
    {
        protected Gstr1Retstatus()
        {
            //
        }

        public static void Update_GSTR1_RetStatus(string GSTINNo, string Period, string RefNo, string Action, string RefIds, string Status, string error_msg, string CustId, string UserId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@gstin", SqlDbType.VarChar).Value = GSTINNo;
                cmd.Parameters.Add("@fp", SqlDbType.VarChar).Value = Period;
                cmd.Parameters.Add("@referenceno", SqlDbType.VarChar).Value = RefNo;
                cmd.Parameters.Add("@actiontype", SqlDbType.VarChar).Value = Action;
                cmd.Parameters.Add("@refids", SqlDbType.NVarChar).Value = RefIds;
                cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = Status;
                cmd.Parameters.Add("@errorreport", SqlDbType.NVarChar).Value = error_msg;
                cmd.Parameters.Add("@customerid", SqlDbType.Int).Value = CustId;
                cmd.Parameters.Add("@createdby", SqlDbType.Int).Value = UserId;
                cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now;

                if (Status == "1")
                {
                    Common.Functions.InsertIntoTable("TBL_GSTR1_SAVE_RETSTATUS", cmd, con);
                }
                else
                {
                    Common.Functions.UpdateTable("TBL_GSTR1_SAVE_RETSTATUS", "referenceno", RefNo, cmd, con);
                }
                con.Close();
            }
        }

        public static void Update_GSTR1_RetStatus(string GSTINNo, string Period, string RefNo, string Action, string Status, 
            string error_msg, string CustId, string UserId, string MinInvId, string MaxInvId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@gstin", SqlDbType.VarChar).Value = GSTINNo;
                cmd.Parameters.Add("@fp", SqlDbType.VarChar).Value = Period;
                cmd.Parameters.Add("@referenceno", SqlDbType.VarChar).Value = RefNo;
                cmd.Parameters.Add("@actiontype", SqlDbType.VarChar).Value = Action;
                cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = Status;
                cmd.Parameters.Add("@errorreport", SqlDbType.NVarChar).Value = error_msg;
                cmd.Parameters.Add("@customerid", SqlDbType.Int).Value = CustId;
                cmd.Parameters.Add("@createdby", SqlDbType.Int).Value = UserId;
                cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@MinInvId", SqlDbType.NVarChar).Value = MinInvId;
                cmd.Parameters.Add("@MaxInvId", SqlDbType.NVarChar).Value = MaxInvId;

                if (Status == "1")
                {
                    Common.Functions.InsertIntoTable("TBL_GSTR1_SAVE_RETSTATUS", cmd, con);
                }
                else
                {
                    Common.Functions.UpdateTable("TBL_GSTR1_SAVE_RETSTATUS", "referenceno", RefNo, cmd, con);
                }
                con.Close();
            }
        }

        public static void Updating_GSTR1_RetStatus(string strStatus, string strErrorMsg, string strRefNo)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = strStatus;
                    cmd.Parameters.Add("@errorreport", SqlDbType.NVarChar).Value = strErrorMsg;
                    Common.Functions.UpdateTable("TBL_GSTR1_SAVE_RETSTATUS", "referenceno", strRefNo, cmd, conn);
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static string Updating_Invoice_Flag_GSTR1(string strRefNo, string ErrorFlag)
        {
            string Status_Response = "";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                con.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select gstin, fp, actiontype, refids, MinInvId, MaxInvId from TBL_GSTR1_SAVE_RETSTATUS where status != '1' and referenceno = @RefNo";
                    sqlcmd.Parameters.AddWithValue("@RefNo", strRefNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string gstinno = dt.Rows[i]["gstin"].ToString();
                                string period = dt.Rows[i]["fp"].ToString();
                                string Action = dt.Rows[i]["actiontype"].ToString();
                                string RefIds = dt.Rows[i]["refids"].ToString();
                                string MinInvId = dt.Rows[i]["MinInvId"].ToString();
                                string MaxInvId = dt.Rows[i]["MaxInvId"].ToString();
                                if (string.IsNullOrEmpty(RefIds))
                                {
                                    GSTR1Helper.Update_Invoice_Flag_GSTR1(Action, gstinno, period, "1", ErrorFlag, MinInvId, MaxInvId);
                                }
                                else
                                {
                                    Update_Invoice_Flag_GSTR1(Action, gstinno, period, RefIds, "1", ErrorFlag);
                                }
                            }
                        }
                    }
                }
                con.Close();
            }
            return Status_Response;
        }

        public static void Update_Invoice_Flag_GSTR1(string ActionType, string strGSTINNo, string strPeriod, string RefIds, string CurFlag, string NewFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR1", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));
                    dCmd.Parameters.Add(new SqlParameter("@CurFlag", CurFlag));
                    dCmd.Parameters.Add(new SqlParameter("@NewFlag", NewFlag));
                    dCmd.Connection = conn;
                    dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void Process_RetStatus_Error_Invoices(string strGSTINNo, string strPeriod, string strRefNo)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Process_GSTR1_RETSTATUS_Bulk", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", strRefNo));
                    dCmd.Connection = conn;
                    dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}