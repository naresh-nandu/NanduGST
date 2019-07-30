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
using System.Globalization;

namespace SmartAdminMvc.Models.GSTR3
{

    public class GSP_GetGSTR3BDataModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        //string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.++) + @"\Downloads";
        private static string GSTR3B_Download_Response = "";
        private static string GST_Path = "", GST_TrackPath = "";
        static string gstin = "";
        static string ctin = "";
        static string retperiod = "";
        static string action = "";
        static string token = "";
        static string refid = "";
        static string statecd = "";
        static string URL = "";
        public static string flag = "";
        public static string month = "";
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "";

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strStatecd, string strRefId, string strToken,string from, string to, string SessionUserId, string SessionCustId, string SessionUsername, string flagg)
        {
            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR3B"];
            GST_TrackPath = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];
            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                //string AuthorizeToken = GetAccessToken.getAuthToken();

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;

                refid = strRefId;
                action = strAction;
                token = strToken;
                if (action == "RETSUM")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "RETSTATUS")
                {
                    URL = GST_TrackPath + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&ref_id=" + refid;
                }
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("Authorization", AuthorizeToken);
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode); // ConfigurationManager.AppSettings["GSP_StateCode"]);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);
                httpWebRequest.Headers.Add("GSTINNO", strGSTINNo);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR3B_Download_Response = result.ToString();
                }
                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR3B_Download_Response);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string hmac_response = GST_Data.hmac.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    string decrypted_appkey = AppKey;
                    string receivedSEK = EncryptedSEK;
                    string gotREK = rek_response;
                    string gotdata = data_response;
                    byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    DecryptedJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));
                    var filedata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                    string[] stringUrls = DecryptedJsonData.Split('\"');

                    if (action == "RETSTATUS")
                    {
                        var downloaddata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                        //var downloaddata = JsonConvert.DeserializeObject<DownloadStatusResponse>(DecryptedJsonData);

                        string st_res = downloaddata["status_cd"].Value<string>();

                        HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;

                        if (st_res == "P")
                        {
                            //InsertGSTR1_Json_Flag(strGSTINNo, strPeriod, "U");
                            Updating_GSTR3B_RetStatus(st_res, "", refid);

                        }
                        else if (st_res == "IP")
                        {
                            Updating_GSTR3B_RetStatus(st_res, "", refid);

                        }
                        else if (st_res == "PE")
                        {
                            //string error_res = downloaddata["error_report"].Value<string>();
                            Updating_GSTR3B_RetStatus(st_res, downloaddata.ToString(), refid);


                        }
                        else
                        {
                            //string error_res = downloaddata["error_report"].Value<string>();
                            Updating_GSTR3B_RetStatus(st_res, downloaddata.ToString(), refid);
                        }
                    }

                    if (action == "RETSUM")
                    {
                        InsertGSTR3B_RETSUM_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                    }
                    flag = "S";
                    if (flagg == "1")
                    {
                        month = "";
                        month = strPeriod;
                    }
                    else if (month == "" && flagg == "")
                    {
                        month = strPeriod;
                    }
                    else if (month != "" && flagg == "" && action == "RETSTATUS")
                    {
                        month = "";
                        month = strPeriod;
                    }
                    else
                    {
                        month = month + "," + strPeriod;
                    }
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-3B " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + month, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR3B DOWNLOAD", SessionUserId, SessionCustId);
                    return "GSTR-3B " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month;
                }
                else if (flag == "S")
                {
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-3B " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + month, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR3B DOWNLOAD", SessionUserId, SessionCustId);
                    return "GSTR-3B " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR3B_Download_Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-3B " + action + " " + root.error.message + " for this " + strGSTINNo + " & " + from + " to " + to, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR3B DOWNLOAD", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
                //return null;


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void Updating_GSTR3B_RetStatus(string strStatus, string strErrorMsg, string strRefNo)
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
                    Common.Functions.UpdateTable("TBL_GSTR3B_SAVE_RETSTATUS", "referenceno", strRefNo, cmd, conn);
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }

        private void InsertGSTR3B_D_Json(string strGSTINNo, string strPeriod, string decryptedJsonData)
        {
            throw new NotImplementedException();
        }

        public static void InsertGSTR3B_RETSUM_Json(string strGSTINNo, string strPeriod, string strRecordContents)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR3B_RETSUM", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RecordContents", strRecordContents));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}