using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using SmartAdminMvc.Models.ESign;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR1
{
    public class GSP_SendGSTR1_Submit_File_DataModel
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private static string DecryptedJsonData = "", DecryptedJsonData1 = "";
        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        private static string GSTR1SubmitResponse = "", GSTR1FileResponse = "", WalletResponse = "";
        GSPResponse GST_Data = null;

        public string GSTR1SubmitRequest(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId,
            string SessionCustId, string SessionUsername)
        {
            try
            {
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = GSTINNo.Substring(0, 2); // State Code

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                string json = Helper.Base64Encode(JsonData);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GSP_GSTR1SubmitParameters objGSTR1SubmitParams = new GSP_GSTR1SubmitParameters();
                objGSTR1SubmitParams.data = payload;
                objGSTR1SubmitParams.hmac = strhmac;
                objGSTR1SubmitParams.action = "RETSUBMIT";

                string tempjsondata = new JavaScriptSerializer().Serialize(objGSTR1SubmitParams);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR1"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode);
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
                        GSTR1SubmitResponse = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1SubmitResponse);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string hmac_response = GST_Data.hmac.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);
                    string refid_response = finaldata["reference_id"].Value<string>();

                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Submitted Successfully for this " + GSTINNo + " & " + Period + ", : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SUBMIT", SessionUserId, SessionCustId);
                    //int UserId = Convert.ToInt32(SessionUserId);
                    //string strMobileNo = (from lst in db.UserLists
                    //                   where lst.UserId == UserId
                    //                   select lst.MobileNo).SingleOrDefault();
                    //Common.Notification.SendSMS(strMobileNo, string.Format("Congratulations!! Your GSTR - 1 is Submitted Successfully!!!"));
                    GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "RETSUBMIT", "", "1", "", SessionCustId, SessionUserId);

                    GSP_GetGSTR1DataModel GSP_getGSTR1 = new GSP_GetGSTR1DataModel();
                    string DownloadRes = GSP_getGSTR1.SendRequest(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", "", "", SessionUserId, SessionCustId, SessionUsername, "1");
                    //string DownloadRetSum = GSP_getGSTR1.SendRequest(GSTINNo, Period, "RETSUM", "", "", "", "", SessionUserId, SessionCustId, SessionUsername);

                    return "GSTR1 Submitted Successfully... Reference Id - " + refid_response;

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1SubmitResponse);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Submit " + root.error.message + " for this " + GSTINNo + " & " + Period, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SUBMIT", SessionUserId, SessionCustId);
                    GSP_GetGSTR1DataModel GSP_getGSTR1 = new GSP_GetGSTR1DataModel();
                    string DownloadRetSum = GSP_getGSTR1.SendRequest(GSTINNo, Period, "RETSUM", "", "", "", "", "", "", SessionUserId, SessionCustId, SessionUsername, "1");
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void GSTR1FileRequest(string JsonData, string GSTINNo, string Period, string strFileType, string strDSC, string strPANNo, string strEVCOTP,
            string SessionUserId, string SessionCustId, string SessionUsername, out string OStatus, out string OResponse)
        {
            string tempjsondata = "", strStatus = "0", strResponse = "";
            try
            {
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = GSTINNo.Substring(0, 2); // State Code

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                string strSignedData = string.Empty;
                string Base64JsonPayload = Helper.Base64Encode(JsonData.Trim());
                string payload = Helper.Encrypt(Base64JsonPayload, authEK);
                if (strFileType == "DSC")
                {
                    GSTR1FileParameters objGSTR1FileParams = new GSTR1FileParameters();
                    objGSTR1FileParams.action = "RETFILE";
                    objGSTR1FileParams.data = payload;
                    objGSTR1FileParams.sign = strDSC;
                    objGSTR1FileParams.st = strFileType;
                    objGSTR1FileParams.sid = strPANNo;

                    tempjsondata = new JavaScriptSerializer().Serialize(objGSTR1FileParams);
                }
                else if(strFileType == "EVC")
                {
                    string strBase64SummaryPayload = "";
                    strBase64SummaryPayload = Helper.Base64Encode(JsonData.Trim());

                    string strEVCSecretKey = string.Empty;
                    strEVCSecretKey = Helper.Base64Encode(strPANNo + "|" + strEVCOTP);
                    strSignedData = Helper.HMAC_SHA256(strBase64SummaryPayload, strPANNo + "|" + strEVCOTP);

                    GSTR1FileParameters objGSTR1FileParams = new GSTR1FileParameters();
                    objGSTR1FileParams.action = "RETFILE";
                    objGSTR1FileParams.data = payload;
                    objGSTR1FileParams.sign = strSignedData;
                    objGSTR1FileParams.st = strFileType;
                    objGSTR1FileParams.sid = strPANNo + "|" + strEVCOTP;

                    tempjsondata = new JavaScriptSerializer().Serialize(objGSTR1FileParams);
                }
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR1"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode);
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
                        GSTR1FileResponse = result.ToString();
                    }
                }

                #region "Decrypting GSTR1 Response"
                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1FileResponse);

                string status_response = GST_Data.status_cd.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string hmac_response = GST_Data.hmac.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);

                    string Acknum_response = finaldata["ack_num"].Value<string>();

                    Helper.InsertGSTRFileResponse(GSTINNo, "GSTR1", "", Acknum_response, Period, SessionCustId, SessionUserId);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Filed Successfully for this " + GSTINNo + " & " + Period + ", : Ack No - " + Acknum_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 FILE", SessionUserId, SessionCustId);

                    // SMS & EMAIL Notification for GSTR1 Filing - Starts                    
                    string strMobileNo = Helper.GetCustomerDetails("MobileNo", "UserList", "UserId = '" + SessionUserId + "' AND ROWSTATUS = 1");
                    string strEmail = Helper.GetCustomerDetails("Email", "UserList", "UserId = '" + SessionUserId + "' AND ROWSTATUS = 1");
                    string strAdminMobileNo = Helper.GetCustomerDetails("MobileNo", "TBL_Customer", "CustId = '" + SessionCustId + "' AND ROWSTATUS = 1");
                    string strAdminEmail = Helper.GetCustomerDetails("Email", "TBL_Customer", "CustId = '" + SessionCustId + "' AND ROWSTATUS = 1");
                    //string strMobileNo = (from lst in db.UserLists
                    //                      where lst.UserId == UserId
                    //                      select lst.MobileNo).SingleOrDefault();

                    //string strEmail = (from lst in db.UserLists
                    //                      where lst.UserId == UserId
                    //                      select lst.Email).SingleOrDefault();
                    Notification.SendSMS(strMobileNo, string.Format("Congratulations!! Your GSTR - 1 is Filed Successfully!!!"));
                    Notification.SendEmail(strEmail, string.Format("GSTR1 Filing Status"), string.Format("Congratulations!! Your GSTR - 1 is Filed Successfully for this " + GSTINNo + " & " + Period + ". Ack No - " + Acknum_response + "."));
                    if(strEmail != strAdminEmail)
                    {
                        Notification.SendSMS(strAdminMobileNo, string.Format("Congratulations!! Your GSTR - 1 is Filed Successfully!!!"));
                        Notification.SendEmail(strAdminEmail, string.Format("GSTR1 Filing Status"), string.Format("Congratulations!! Your GSTR - 1 is Filed Successfully for this " + GSTINNo + " & " + Period + ". Ack No - " + Acknum_response + "."));
                    }
                    // SMS & EMAILNotification for GSTR1 Filing - Ends

                    strResponse = "GSTR1 Filed Successfully... Ack No. - " + Acknum_response;

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1FileResponse);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 File " + root.error.message + " for this " + GSTINNo + " & " + Period, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 FILE", SessionUserId, SessionCustId);
                    strResponse = root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
                #endregion

            }
            catch (Exception ex)
            {
                strStatus = "2";
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
        }
    }
        
    public class GSP_GSTR1SubmitParameters
    {
        public string action { get; set; }
        public string data { get; set; }
        public string hmac { get; set; }
    }

}