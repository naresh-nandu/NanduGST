using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Runtime.InteropServices;
using System.Reflection;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR1
{
    public partial class GetGSTR1DataModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        //string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        private static string GSTR1_Download_Response = "";
        private static string GST_Path = "", GST_TrackPath = "";
        static string gstin = "";
        static string ctin = "";
        static string retperiod = "";
        static string action = "";
        static string token = "";
        static string refid = "";
        static string statecd = "";
        static string URL = "";
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "";

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strCTIN, string strStatecd, string strRefId, string strToken, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            GST_Path = ConfigurationManager.AppSettings["GST_GSTR1"];
            GST_TrackPath = ConfigurationManager.AppSettings["GST_TRACKSTATUS"];
            try
            {
                SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_AUTH_KEYS where AuthorizationToken = '" + strGSTINNo + "'", con);
                System.Data.DataTable dt = new System.Data.DataTable();
                adt.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Username = dt.Rows[0]["Username"].ToString();
                    AppKey = dt.Rows[0]["AppKey"].ToString();
                    EncryptedSEK = dt.Rows[0]["EncryptedSEK"].ToString();
                    AuthToken = dt.Rows[0]["AuthToken"].ToString();
                }
                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                ctin = strCTIN;
                statecd = strStatecd;
                refid = strRefId;
                action = strAction;
                token = strToken;

                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                if (action == "B2CL")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&state_cd=" + statecd;
                }
                else if (action == "B2CS" || action == "CDNR" || action == "EXP" || action == "NIL" || action == "TXP" || action == "AT" || action == "HSNSUM" ||
                    action == "DOCISS" || action == "RETSUM")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "RETSTATUS")
                {
                    URL = GST_TrackPath + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&ref_id=" + refid;
                }
                else if (action == "FILEDET")
                {
                    URL = GST_TrackPath + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&token=" + token;
                }

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode); // ConfigurationManager.AppSettings["GSP_StateCode"]);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR1_Download_Response = result.ToString();
                }

                var data = (JObject)JsonConvert.DeserializeObject(GSTR1_Download_Response);
                string status_response = data["status_cd"].Value<string>();
                if (status_response == "1")
                {
                    string data_response = data["data"].Value<string>();
                    string hmac_response = data["hmac"].Value<string>();
                    string rek_response = data["rek"].Value<string>();

                    string decrypted_appkey = AppKey;
                    string receivedSEK = EncryptedSEK;
                    string gotREK = rek_response;
                    string gotdata = data_response;

                    byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                    //System.out.println("Encoded Auth EK (Received):" + AESEncryption.encodeBase64String(authEK));

                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    //System.out.println("Encoded Api EK (Received):" + AESEncryption.encodeBase64String(apiEK));
                    DecryptedJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));
                    //string dataSaveResponse = GSTR2DownloadDataModel.SaveJsonDatatoDB(DecryptedJsonData, strGSTINNo, strPeriod);
                    //return DecryptedJsonData;
                    
                    if (action == "RETSTATUS")
                    {
                        var downloaddata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);

                        string st_res = downloaddata["status_cd"].Value<string>();

                        HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                        if(st_res == "P")
                        {
                            //con.Open();
                            //SqlDataAdapter stadt = new SqlDataAdapter("Select * from TBL_GSTR_SAVE_Response where RefId = '"+ refid +"'", con);
                            //DataTable stdt = new DataTable();
                            //stadt.Fill(stdt);
                            //if(stdt.Rows.Count > 0)
                            //{
                            //    if(stdt.Rows[0]["GSTRName"].ToString() == "GSTR1")
                            //    {
                            //        InsertGSTR1_Json_Flag(strGSTINNo, strPeriod, "U");
                            //    }
                            //    else if (stdt.Rows[0]["GSTRName"].ToString() == "GSTR1D")
                            //    {
                            //        UpdateGSTR1_D_Flag(strGSTINNo, strPeriod, "D", "0");
                            //    }
                            //}
                            //con.Close();
                            Updating_GSTR1_RetStatus(st_res, "", refid);
                            Updating_Invoice_Flag_GSTR1(refid, "U");
                        }
                        else if(st_res =="IP")
                        {
                            Updating_GSTR1_RetStatus(st_res, "", refid);
                            Updating_Invoice_Flag_GSTR1(refid, "1");
                        }
                        else if (st_res == "PE")
                        {
                            string error_res = downloaddata["error_report"].Value<string>();
                            Updating_GSTR1_RetStatus(st_res, error_res, refid);
                            Updating_Invoice_Flag_GSTR1(refid, "2");
                            Process_RetStatus_Error_Invoices(strGSTINNo, strPeriod);
                        }
                        else
                        {
                            string error_res = downloaddata["error_report"].Value<string>();
                            Updating_GSTR1_RetStatus(st_res, error_res, refid);
                            Updating_Invoice_Flag_GSTR1(refid, "2");
                        }
                    }
                    else if(action == "FILEDET")
                    {                        
                        HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                    }
                    else if(action == "RETSUM")
                    {
                        InsertGSTR1_RETSUM_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                    }
                    else
                    {
                        InsertGSTR1_D_Json(strGSTINNo, strPeriod, DecryptedJsonData);                        
                    }
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@FK_Audit_User_ID", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@FK_Audit_Username", SqlDbType.VarChar).Value = SessionUsername;
                    cmd.Parameters.Add("@Audit_DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Audit_Message", SqlDbType.VarChar).Value = "GSTR-1 " + action + " Downloaded Successfully for " + strPeriod;
                    cmd.Parameters.Add("@Audit_Exception", SqlDbType.VarChar).Value = "";
                    Common.Functions.InsertIntoTable("TBL_Audit_Log", cmd, con);
                    con.Close();

                    return "GSTR-1 " + action + " Downloaded Successfully";
                }
                else if(status_response == "2")
                {
                    string data_response = data["data"].Value<string>();
                    string hmac_response = data["hmac"].Value<string>();
                    string rek_response = data["rek"].Value<string>();

                    string decrypted_appkey = AppKey;
                    string receivedSEK = EncryptedSEK;
                    string gotREK = rek_response;
                    string gotdata = data_response;

                    byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                    //System.out.println("Encoded Auth EK (Received):" + AESEncryption.encodeBase64String(authEK));

                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    //System.out.println("Encoded Api EK (Received):" + AESEncryption.encodeBase64String(apiEK));
                    string filetokenresponse = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));

                    var filedata = (JObject)JsonConvert.DeserializeObject(filetokenresponse);
                    string token_response = filedata["token"].Value<string>();

                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@FK_Audit_User_ID", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@FK_Audit_Username", SqlDbType.VarChar).Value = SessionUsername;
                    cmd.Parameters.Add("@Audit_DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Audit_Message", SqlDbType.VarChar).Value = "GSTR-1 " + action + " Downloaded Successfully for " + strPeriod + ", Token for Large File - " + token_response;
                    cmd.Parameters.Add("@Audit_Exception", SqlDbType.VarChar).Value = "";
                    Common.Functions.InsertIntoTable("TBL_Audit_Log", cmd, con);
                    con.Close();

                    return "GSTR-1 " + action + " Downloaded Successfully with Token for Large File - " + token_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<Common.ErrorMsg>(GSTR1_Download_Response);
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@FK_Audit_User_ID", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@FK_Audit_Username", SqlDbType.VarChar).Value = SessionUsername;
                    cmd.Parameters.Add("@Audit_DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Audit_Message", SqlDbType.VarChar).Value = "GSTR-1 " + action + " " + root.error.message + " for " + strPeriod;
                    cmd.Parameters.Add("@Audit_Exception", SqlDbType.VarChar).Value = "Error Code - " + root.error.error_cd;
                    Common.Functions.InsertIntoTable("TBL_Audit_Log", cmd, con);
                    con.Close();
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void InsertGSTR1_D_Json(string strGSTINNo, string strPeriod, string strJsonData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR1_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RecordContents", strJsonData));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }

        public static void InsertGSTR1_RETSUM_Json(string strGSTINNo, string strPeriod, string strJsonData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR1_RETSUM", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RecordContents", strJsonData));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }

        public static void InsertGSTR1_Json_Flag(string strGSTINNo, string strPeriod, string strFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR1", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }

        public static void UpdateGSTR1_D_Flag(string strGSTINNo, string strPeriod, string strOldFlag, string strNewFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR1_D", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@CurFlag", strOldFlag));
                    dCmd.Parameters.Add(new SqlParameter("@NewFlag", strNewFlag));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
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
                catch
                {

                }
            }            
        }
                
        public string Updating_Invoice_Flag_GSTR1(string strRefNo, string ErrorFlag)
        {
            string Status_Response = "";
            con.Open();
            SqlDataAdapter adt = new SqlDataAdapter("Select gstin, fp, actiontype, refids from TBL_GSTR1_SAVE_RETSTATUS where status != '1' and referenceno = '"+ strRefNo +"'", con);
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

                    Update_Invoice_Flag_GSTR1(Action, gstinno, period, RefIds, "1", ErrorFlag);                                        
                }
            }
            con.Close();
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
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));
                    dCmd.Parameters.Add(new SqlParameter("@CurFlag", CurFlag));
                    dCmd.Parameters.Add(new SqlParameter("@NewFlag", NewFlag));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }

        public static void Process_RetStatus_Error_Invoices(string strGSTINNo, string strPeriod)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Process_GSTR1_RETSTATUS", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }
    }
}