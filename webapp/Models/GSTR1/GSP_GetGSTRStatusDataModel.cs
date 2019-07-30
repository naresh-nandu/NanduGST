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
    public partial class GSP_GetGSTRStatusDataModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        //string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        private static string GSTR_Download_Response = "";
        private static string GST_Path = "", GST_TrackPath = "";
        static string gstin = "";
        static string type = "";
        static string retperiod = "";
        static string action = "";
        static string URL = "";
        public static string flag = "";
        public static string token_flag = "";
        public static string month = "";
        public static string month1 = "";
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "";

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strType,string from, string to, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR1"];
            GST_TrackPath = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];

            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                //string AuthorizeToken = GetAccessToken.getAuthToken();

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                type = strType;
                action = strAction;

                if (action == "RETTRACK")
                {
                    URL = GST_TrackPath + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&type=" + type;
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
                    GSTR_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR_Download_Response);

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

                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR View & Track Status checked successfully for this " + strGSTINNo + " & " + from + " to " + to, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLAOD", SessionUserId, SessionCustId);
                    string[] stringUrls = DecryptedJsonData.Split('\"');

                    if (stringUrls.Count() < 10)
                    {
                        flag = "S";
                        if (month == "")
                        {
                            month = strPeriod;
                        }
                        else
                        {
                            month = month + "," + strPeriod;
                        }
                        var decrypted_Data = JsonConvert.DeserializeObject<ViewTrackAttributes>(DecryptedJsonData);
                        InsertViewTrackStatusError(strGSTINNo, strPeriod, Convert.ToInt32(status_response), DecryptedJsonData, SessionUserId, SessionCustId);
                        return "GSTR View & Track Status downloaded successfully... Message - " + decrypted_Data.EFiledlist + " for this " + strGSTINNo + " and " + month;
                    }
                    else
                    {
                        if (month1 == "")
                        {
                            month1 = strPeriod;
                        }
                        else
                        {
                            month1 = month1 + "," + strPeriod;
                        }
                        flag = "S";
                        token_flag = "true";
                        var decrypted_Data = JsonConvert.DeserializeObject<GSTRViewTrackResponse>(DecryptedJsonData);
                        InsertViewTrackStatus(strGSTINNo, strPeriod, Convert.ToInt32(status_response), DecryptedJsonData, SessionUserId, SessionCustId);
                        return "GSTR View & Track Status downloaded successfully... AckNo - " + decrypted_Data.EFiledlist[0].arn + " for this " + strGSTINNo + " and " + month1 + " and Status - " + decrypted_Data.EFiledlist[0].status;
                    }
                }
                else if (flag == "S" && token_flag == "")
                {
                    var decrypted_Data = JsonConvert.DeserializeObject<ViewTrackAttributes>(DecryptedJsonData);
                    InsertViewTrackStatusError(strGSTINNo, strPeriod, Convert.ToInt32(status_response), DecryptedJsonData, SessionUserId, SessionCustId);
                    return "GSTR View & Track Status downloaded successfully... Message - " + decrypted_Data.EFiledlist + " for this " + strGSTINNo + " and " + month;
                }
                else if (flag == "S" && token_flag == "true")
                {
                    var decrypted_Data = JsonConvert.DeserializeObject<ViewTrackAttributes>(DecryptedJsonData);
                    InsertViewTrackStatusError(strGSTINNo, strPeriod, Convert.ToInt32(status_response), DecryptedJsonData, SessionUserId, SessionCustId);
                    return "GSTR View & Track Status downloaded successfully... Message - " + decrypted_Data.EFiledlist + " for this " + strGSTINNo + " and " + month + ".For large files go to Audit log report.";
                }
                else
                {                   
                        var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR_Download_Response);
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR View & Track Status " + root.error.message + " for this " + strGSTINNo + " & " + from + " to " + to, "Error Code - " + root.error.error_cd);
                        Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLAOD", SessionUserId, SessionCustId);
                        return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                   
                }
                if(flag=="S")
                { 
}
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void InsertViewTrackStatus(string strGSTINNo, string strPeriod, int iStatus, string strData, string UserId, string CustId)
        {
            var decrypted_Data = JsonConvert.DeserializeObject<GSTRViewTrackResponse>(strData);
            if (decrypted_Data.EFiledlist.Count > 0)
            {
                for (int i = 0; i < decrypted_Data.EFiledlist.Count; i++)
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstin", SqlDbType.VarChar).Value = strGSTINNo;
                        cmd.Parameters.Add("@period", SqlDbType.VarChar).Value = strPeriod;
                        cmd.Parameters.Add("@status_res", SqlDbType.Int).Value = iStatus;
                        if (iStatus == 1)
                        {
                            if (!string.IsNullOrEmpty(decrypted_Data.EFiledlist[i].arn))
                            {
                                cmd.Parameters.Add("@arn", SqlDbType.VarChar).Value = decrypted_Data.EFiledlist[i].arn;
                            }
                            if (!string.IsNullOrEmpty(decrypted_Data.EFiledlist[i].ret_prd))
                            {
                                cmd.Parameters.Add("@ret_prd", SqlDbType.VarChar).Value = decrypted_Data.EFiledlist[i].ret_prd;
                            }
                            if (!string.IsNullOrEmpty(decrypted_Data.EFiledlist[i].mof))
                            {
                                cmd.Parameters.Add("@mof", SqlDbType.VarChar).Value = decrypted_Data.EFiledlist[i].mof;
                            }
                            if (!string.IsNullOrEmpty(decrypted_Data.EFiledlist[i].dof))
                            {
                                cmd.Parameters.Add("@dof", SqlDbType.VarChar).Value = decrypted_Data.EFiledlist[i].dof;
                            }
                            if (!string.IsNullOrEmpty(decrypted_Data.EFiledlist[i].rtntype))
                            {
                                cmd.Parameters.Add("@rtntype", SqlDbType.VarChar).Value = decrypted_Data.EFiledlist[i].rtntype;
                            }
                            if (!string.IsNullOrEmpty(decrypted_Data.EFiledlist[i].status))
                            {
                                cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = decrypted_Data.EFiledlist[i].status;
                                SqlDataAdapter adt = new SqlDataAdapter("Select TOP 1 * from TBL_GSTR_FILE_RESPONSE Where GSTINNo = '" + strGSTINNo +"' and fp = '"+ strPeriod +"'", con);
                                DataTable dt = new DataTable();
                                adt.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    
                                }
                                else
                                {
                                    Helper.InsertGSTRFileResponse(strGSTINNo, decrypted_Data.EFiledlist[i].rtntype, iStatus.ToString(), decrypted_Data.EFiledlist[i].arn, strPeriod, CustId, UserId);
                                }
                            }
                            if (!string.IsNullOrEmpty(decrypted_Data.EFiledlist[i].valid))
                            {
                                cmd.Parameters.Add("@valid", SqlDbType.VarChar).Value = decrypted_Data.EFiledlist[i].valid;
                            }
                        }
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = CustId;
                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                        Functions.InsertIntoTable("TBL_VIEW_TRACK_STATUS", cmd, con);
                        con.Close();
                    }
                }
            }
        }

        public void InsertViewTrackStatusError(string strGSTINNo, string strPeriod, int iStatus, string strData, string UserId, string CustId)
        {
            ViewTrackAttributes decrypted_Data = JsonConvert.DeserializeObject<ViewTrackAttributes>(strData);
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@gstin", SqlDbType.VarChar).Value = strGSTINNo;
                cmd.Parameters.Add("@period", SqlDbType.VarChar).Value = strPeriod;
                cmd.Parameters.Add("@status_res", SqlDbType.Int).Value = iStatus;
                if (iStatus == 1)
                {
                    if (!string.IsNullOrEmpty(decrypted_Data.EFiledlist))
                    {
                        cmd.Parameters.Add("@Message", SqlDbType.VarChar).Value = decrypted_Data.EFiledlist;
                    }
                }
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = CustId;
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                Functions.InsertIntoTable("TBL_VIEW_TRACK_STATUS", cmd, con);
                con.Close();
            }
        }

    }
}