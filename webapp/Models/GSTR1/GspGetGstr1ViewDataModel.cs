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
using SmartAdminMvc.Models.GSTAUTH;
using SmartAdminMvc.Models.Common;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR1
{
    public partial class GspGetGstr1ViewDataModel
    {
        string GSTR1_Download_Response = "";
        string GST_Path = "", GST_TrackPath = "";
        string gstin = "";
        string retperiod = "";
        string action = "";
        string refid = "";
        string URL = "";
        string DecryptedJsonData = "";

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";

        public readonly string _sessioncustid;
        public readonly string _sessionuserid;
        public readonly string _sessionusername;

        public GspGetGstr1ViewDataModel(string SessionCustId, string SessionUserId, string SessionUsername)
        {
            this._sessioncustid = SessionCustId;
            this._sessionuserid = SessionUserId;
            this._sessionusername = SessionUsername;
        }

        public string SendRequest(string strGSTINAction, string strGSTINNo, string strPeriod, string strAction, string strRefId)
        {
            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR1"];
            GST_TrackPath = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];

            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                
                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                refid = strRefId;
                action = strAction;

                if (action == "RETSTATUS")
                {
                    URL = GST_TrackPath + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&ref_id=" + refid;
                }

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR1_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1_Download_Response);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    string decrypted_appkey = AppKey;
                    string receivedSEK = EncryptedSEK;
                    string gotREK = rek_response;
                    string gotdata = data_response;

                    byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    DecryptedJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));

                    string st_res = "";
                    if (action == "RETSTATUS")
                    {
                        var downloaddata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                        st_res = downloaddata["status_cd"].Value<string>();

                        HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                        if (st_res == "P")
                        {
                            ////InsertGSTR1_Json_Flag(strGSTINNo, strPeriod, "U");
                            Updating_GSTR1_RetStatus(st_res, downloaddata.ToString(), refid);
                            Update_Invoice_Flag_GSTR1(strGSTINAction, strGSTINNo, strPeriod, "", "D", "Z");
                        }
                        else if (st_res == "IP")
                        {
                            Updating_GSTR1_RetStatus(st_res, downloaddata.ToString(), refid);
                        }
                        else if (st_res == "PE")
                        {
                            ////string error_res = downloaddata["error_report"].Value<string>();
                            Updating_GSTR1_RetStatus(st_res, downloaddata.ToString(), refid);
                        }
                        else
                        {
                            ////string error_res = downloaddata["error_report"].Value<string>();
                            Updating_GSTR1_RetStatus(st_res, downloaddata.ToString(), refid);
                        }
                    }

                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + strPeriod + " and Status - '" + st_res + "' and Reference Id - '" + refid + "'", "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLOAD", this._sessionuserid, this._sessioncustid);
                    return "GSTR-1 " + action + " Downloaded Successfully";

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1_Download_Response);
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR-1 " + action + " " + root.error.message + " for this " + strGSTINNo + " & " + strPeriod, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLOAD", this._sessionuserid, this._sessioncustid);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
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
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
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