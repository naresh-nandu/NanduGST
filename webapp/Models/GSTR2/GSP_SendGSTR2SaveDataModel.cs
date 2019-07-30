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
using SmartAdminMvc.Models.GSTAUTH;
using SmartAdminMvc.Models.Common;
using System.Threading.Tasks;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR2
{
    public class GSP_SendGSTR2SaveDataModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "", DecryptedJsonData1 = "";
        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        private static string GSTR2Response = "";
        private static string message = "";
        GSPResponse GST_Data = null;

        //  GSP_GetTokenResponseModel gsptoken = new GSP_GetTokenResponseModel();

        public string SendRequest(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            decimal WalletBalance = 0, WalletMinBalance = 0;
            string GSK = "";
            bool isChecked = false;
            try
            {
                Helper.GetWalletBalance(SessionCustId, "RETURN FILING", "GSTR2", GSTINNo, Period, out WalletBalance, out GSK, out isChecked);
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                
                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GSP_GSTR2SaveParameters objGSTR2SaveParams = new GSP_GSTR2SaveParameters();
                objGSTR2SaveParams.data = payload;
                objGSTR2SaveParams.hmac = strhmac;
                objGSTR2SaveParams.action = "RETSAVE";

                string tempjsondata = JsonConvert.SerializeObject(objGSTR2SaveParams);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR2"]);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode); // ConfigurationManager.AppSettings["GSP_StateCode"]);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", Period);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);
                httpWebRequest.Headers.Add("GSTINNO", GSTINNo);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(tempjsondata);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        GSTR2Response = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR2Response);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    if (GSK == "True")
                    {
                        if (isChecked)
                        {
                            // Already Wallet Deducted
                        }
                        else
                        {
                            Helper.UpdateWalletBalance(SessionCustId, "RETURN FILING", 50, "GSTR3B", GSTINNo, Period);
                            Helper.UpdateWalletBalanceRequest(SessionCustId, 50, "RETURN FILING");
                        }
                    }

                    string data_response = GST_Data.data.ToString();
                    string hmac_response = GST_Data.hmac.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);
                    string refid_response = finaldata["reference_id"].Value<string>();

                    Helper.InsertGSTRSaveResponse(GSTINNo, "GSTR2", "", refid_response, Period, SessionCustId, SessionUserId);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR2 Saved Successfully for this " + GSTINNo + " & " + Period + ". Action - " + strAction + " : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR2 SAVE", SessionUserId, SessionCustId);

                    #region "UPDATE RETSTATUS & INVOICE FLAG"
                    if (HttpContext.Current.Session["G2B2BRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "B2B", HttpContext.Current.Session["G2B2BRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("B2B", GSTINNo, Period, HttpContext.Current.Session["G2B2BRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2B2BURRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "B2BUR", HttpContext.Current.Session["G2B2BURRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("B2BUR", GSTINNo, Period, HttpContext.Current.Session["G2B2BURRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2CDNRRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "CDNR", HttpContext.Current.Session["G2CDNRRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("CDNR", GSTINNo, Period, HttpContext.Current.Session["G2CDNRRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2IMPGRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "IMPG", HttpContext.Current.Session["G2IMPGRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("IMPG", GSTINNo, Period, HttpContext.Current.Session["G2IMPGRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2IMPSRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "IMPS", HttpContext.Current.Session["G2IMPSRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("IMPS", GSTINNo, Period, HttpContext.Current.Session["G2IMPSRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2HSNRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "HSN", HttpContext.Current.Session["G2HSNRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("HSN", GSTINNo, Period, HttpContext.Current.Session["G2HSNRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2NILRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "NIL", HttpContext.Current.Session["G2NILRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("NIL", GSTINNo, Period, HttpContext.Current.Session["G2NILRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2ITCRVSLRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "ITCRVSL", HttpContext.Current.Session["G2ITCRVSLRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("ITCRVSL", GSTINNo, Period, HttpContext.Current.Session["G2ITCRVSLRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2TXIRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "TXI", HttpContext.Current.Session["G2TXIRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("TXI", GSTINNo, Period, HttpContext.Current.Session["G2TXIRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2TXPDRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "TXPD", HttpContext.Current.Session["G2TXPDRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("TXPD", GSTINNo, Period, HttpContext.Current.Session["G2TXPDRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G2CDNURRefIds"].ToString() != "")
                    {
                        GSTR2_RETSTATUS.Update_GSTR2_RetStatus(GSTINNo, Period, refid_response, "CDNUR", HttpContext.Current.Session["G2CDNURRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR2("CDNUR", GSTINNo, Period, HttpContext.Current.Session["G2CDNURRefIds"].ToString(), "", "1");
                    }
                    #endregion

                    GSP_GetGSTR2DataModel GSP_getGSTR2 = new GSP_GetGSTR2DataModel();
                    string DownloadRes = GSP_getGSTR2.SendRequest(GSTINNo, Period, "RETSTATUS", "", refid_response, "", SessionUserId, SessionCustId, SessionUsername);

                    return "GSTR2 Save is in Progress... Reference Id - " + refid_response; // + " " + HttpContext.Current.Session["RETSTATUS"].ToString();
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR2Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, " GSTR2 Save " + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR2 SAVE", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void Update_Invoice_Flag_GSTR2(string ActionType, string strGSTINNo, string strPeriod, string RefIds, string CurFlag, string NewFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR2", conn);
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
    }

    public class GSP_GSTR2SaveParameters
    {
        public string action { get; set; }
        public string data { get; set; }
        public string hmac { get; set; }
    }
}