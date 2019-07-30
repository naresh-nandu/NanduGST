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
using WeP_GSTN;

namespace SmartAdminMvc.Models.GSTR3
{
    public class GspSendGstr3BSubmitFileDataModel
    {
        public readonly string _jsondata;
        public readonly string _gstinno;
        public readonly string _period;
        public readonly string _sessioncustid;
        public readonly string _sessionuserid;
        public readonly string _sessionusername;

        public GspSendGstr3BSubmitFileDataModel(string JsonData, string GSTINNo, string Period, string SessionCustId, string SessionUserId, string SessionUsername)
        {
            this._jsondata = JsonData;
            this._gstinno = GSTINNo;
            this._period = Period;
            this._sessioncustid = SessionCustId;
            this._sessionuserid = SessionUserId;
            this._sessionusername = SessionUsername;
        }

        public string GSTR3BSubmitRequest(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId, 
            string SessionCustId, string SessionUsername)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string GSTR3BSubmitResponse = "";
            GSPResponse GST_Data = null;
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

                GstRequestResponseAttributes objGSTR3BSubmitParams = new GstRequestResponseAttributes();
                objGSTR3BSubmitParams.data = payload;
                objGSTR3BSubmitParams.hmac = strhmac;
                objGSTR3BSubmitParams.action = "RETSUBMIT";

                string tempjsondata = new JavaScriptSerializer().Serialize(objGSTR3BSubmitParams);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR3B"]);
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
                        GSTR3BSubmitResponse = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR3BSubmitResponse);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));
                    
                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);
                    string refid_response = finaldata["reference_id"].Value<string>();
                    
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR3B Submitted Successfully for this " + GSTINNo + " & " + Period + ", : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR3B SUBMIT", SessionUserId, SessionCustId);

                    GspGetGstr3BDataModel GSP_getGSTR3B = new GspGetGstr3BDataModel();
                    GSP_getGSTR3B.SendRequest(GSTINNo, Period, "RETSTATUS", "", refid_response, "", "", "", SessionUserId, SessionCustId, SessionUsername, "1");
                    GSP_getGSTR3B.SendRequest(GSTINNo, Period, "RETSUM", "", "", "", "", "", SessionUserId, SessionCustId, SessionUsername, "1");
                    return "GSTR3B Submitted Successfully... Reference Id - " + refid_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR3BSubmitResponse);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR3B Submit " + root.error.message + " for this " + GSTINNo + " & " + Period, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR3B SUBMIT", SessionUserId, SessionCustId);
                    GspGetGstr3BDataModel GSP_getGSTR3B = new GspGetGstr3BDataModel();
                    GSP_getGSTR3B.SendRequest(GSTINNo, Period, "RETSUM", "", "", "", "", "", SessionUserId, SessionCustId, SessionUsername, "1");
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void GSTR3BFileRequest(string strFileType, string strDSC, string strPANNo, string strEVCOTP, out string OStatus, out string OResponse)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string GSTR3BFileResponse = "";
            GSPResponse GST_Data = null;
            string tempjsondata = "", strStatus = "", strResponse = "";
            try
            {
                Helper.GSTR_DataAdapter(this._gstinno, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = this._gstinno.Substring(0, 2); // State Code

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                string strSignedData = string.Empty;
                string Base64JsonPayload = Helper.Base64Encode(this._jsondata.Trim());
                string payload = Helper.Encrypt(Base64JsonPayload, authEK);
                if (strFileType == "DSC")
                {
                    GstRequestResponseAttributes objGSTR3BFileParams = new GstRequestResponseAttributes();
                    objGSTR3BFileParams.action = "RETFILE";
                    objGSTR3BFileParams.data = payload;
                    objGSTR3BFileParams.sign = strDSC;
                    objGSTR3BFileParams.st = strFileType;
                    objGSTR3BFileParams.sid = strPANNo;

                    tempjsondata = new JavaScriptSerializer().Serialize(objGSTR3BFileParams);
                }
                else if (strFileType == "EVC")
                {
                    string strBase64SummaryPayload = "";
                    strBase64SummaryPayload = Helper.Base64Encode(this._jsondata.Trim());
                    strSignedData = Helper.HMAC_SHA256(strBase64SummaryPayload, strPANNo + "|" + strEVCOTP);

                    GstRequestResponseAttributes objGSTR3BFileParams = new GstRequestResponseAttributes();
                    objGSTR3BFileParams.action = "RETFILE";
                    objGSTR3BFileParams.data = payload;
                    objGSTR3BFileParams.sign = strSignedData;
                    objGSTR3BFileParams.st = strFileType;
                    objGSTR3BFileParams.sid = strPANNo + "|" + strEVCOTP;

                    tempjsondata = new JavaScriptSerializer().Serialize(objGSTR3BFileParams);
                }
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR3B"]);
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
                httpWebRequest.Headers.Add("ret_period", this._period);
                httpWebRequest.Headers.Add("gstin", this._gstinno);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);

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
                        GSTR3BFileResponse = result.ToString();
                    }
                }

                #region "Decrypting Response"
                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR3BFileResponse);

                string status_response = GST_Data.status_cd.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);

                    string Acknum_response = finaldata["ack_num"].Value<string>();

                    Helper.InsertGSTRFileResponse(this._gstinno, "GSTR3B", "", Acknum_response, this._period, this._sessioncustid, this._sessionuserid);
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR3B Filed Successfully for this " + this._gstinno + " and " + this._period + " : Ack No - " + Acknum_response, "");
                    Helper.InsertAPICountTransactions(this._gstinno, this._period, "GSTR3B FILE", this._sessionuserid, this._sessioncustid);

                    // SMS & EMAIL Notification for GSTR1 Filing - Starts                    
                    string strMobileNo = Helper.GetCustomerDetails("MobileNo", "UserList", " WHERE UserId = '" + this._sessionuserid + "' AND ROWSTATUS = 1");
                    string strEmail = Helper.GetCustomerDetails("Email", "UserList", " WHERE UserId = '" + this._sessionuserid + "' AND ROWSTATUS = 1");
                    string strAdminMobileNo = Helper.GetCustomerDetails("MobileNo", "TBL_Customer", " WHERE CustId = '" + this._sessioncustid + "' AND ROWSTATUS = 1");
                    string strAdminEmail = Helper.GetCustomerDetails("Email", "TBL_Customer", " WHERE CustId = '" + this._sessioncustid + "' AND ROWSTATUS = 1");

                    Notification.SendSMS(strMobileNo, "Congratulations!! Your GSTR - 3B is Filed Successfully!!!");
                    Notification.SendEmail(strEmail, "GSTR3B Filing Status", "Congratulations!! Your GSTR - 3B is Filed Successfully for this " + this._gstinno + " and " + this._period + ". Ack No - " + Acknum_response + ".");
                    if (strEmail != strAdminEmail)
                    {
                        Notification.SendSMS(strAdminMobileNo, "Congratulations!! Your GSTR - 3B is Filed Successfully!!!");
                        Notification.SendEmail(strAdminEmail, "GSTR3B Filing Status", "Congratulations!! Your GSTR - 3B is Filed Successfully for this " + this._gstinno + " and " + this._period + ". Ack No - " + Acknum_response + ".");
                    }
                    // SMS & EMAILNotification for GSTR1 Filing - Ends

                    strResponse = "GSTR3B Filed Successfully... Ack No. - " + Acknum_response + "";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR3BFileResponse);
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR3B File " + root.error.message + " for this " + this._gstinno + " and " + this._period, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(this._gstinno, this._period, "GSTR3B FILE", this._sessionuserid, this._sessioncustid);
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
}