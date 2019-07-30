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

namespace SmartAdminMvc.Models.GSTR1
{
    public partial class GspSendGstr1SubmitFileDataModel
    {
        public readonly string _jsondata;
        public readonly string _gstinno;
        public readonly string _period;
        public readonly string _sessioncustid;
        public readonly string _sessionuserid;
        public readonly string _sessionusername;

        public GspSendGstr1SubmitFileDataModel(string JsonData, string GSTINNo, string Period, string SessionCustId, string SessionUserId, string SessionUsername)
        {
            this._jsondata = JsonData;
            this._gstinno = GSTINNo;
            this._period = Period;
            this._sessioncustid = SessionCustId;
            this._sessionuserid = SessionUserId;
            this._sessionusername = SessionUsername;
        }

        public string GSTR1SubmitRequest()
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string GSTR1SubmitResponse = "";
            GSPResponse GST_Data = null;
            try
            {
                Helper.GSTR_DataAdapter(this._gstinno, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = this._gstinno.Substring(0, 2); // State Code

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                string json = Helper.Base64Encode(this._jsondata);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GstRequestResponseAttributes objGSTR1SubmitParams = new GstRequestResponseAttributes();
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
                        GSTR1SubmitResponse = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1SubmitResponse);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);
                    string refid_response = finaldata["reference_id"].Value<string>();

                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR1 Submitted Successfully for this " + this._gstinno + " & " + this._period + ", : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(this._gstinno, this._period, "GSTR1 SUBMIT", this._sessionuserid, this._sessioncustid);
                    
                    Gstr1Retstatus.Update_GSTR1_RetStatus(this._gstinno, this._period, refid_response, "RETSUBMIT", "", "1", "", this._sessioncustid, this._sessionuserid);

                    GspGetGstr1DataModel GSP_getGSTR1 = new GspGetGstr1DataModel();
                    GSP_getGSTR1.SendRequest(this._gstinno, this._period, "RETSTATUS", "", "", refid_response, "", "", "", this._sessionuserid, this._sessioncustid, this._sessionusername, "1");
                    GSP_getGSTR1.SendRequest(this._gstinno, this._period, "RETSUM", "", "", "", "", "", "", this._sessionuserid, this._sessioncustid, this._sessionusername, "1");

                    return "GSTR1 Submitted Successfully... Reference Id - " + refid_response;

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1SubmitResponse);
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR1 Submit " + root.error.message + " for this " + this._gstinno + " & " + this._period, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(this._gstinno, this._period, "GSTR1 SUBMIT", this._sessionuserid, this._sessioncustid);
                    GspGetGstr1DataModel GSP_getGSTR1 = new GspGetGstr1DataModel();
                    GSP_getGSTR1.SendRequest(this._gstinno, this._period, "RETSUM", "", "", "", "", "", "", this._sessionuserid, this._sessioncustid, this._sessionusername, "1");
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void GSTR1FileRequest(string strFileType, string strDSC, string strPANNo, string strEVCOTP, out string OStatus, out string OResponse)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string GSTR1FileResponse = "";
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
                    GstRequestResponseAttributes objGSTR1FileParams = new GstRequestResponseAttributes();
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
                    strBase64SummaryPayload = Helper.Base64Encode(this._jsondata.Trim());
                    strSignedData = Helper.HMAC_SHA256(strBase64SummaryPayload, strPANNo + "|" + strEVCOTP);

                    GstRequestResponseAttributes objGSTR1FileParams = new GstRequestResponseAttributes();
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
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);

                    string Acknum_response = finaldata["ack_num"].Value<string>();

                    Helper.InsertGSTRFileResponse(this._gstinno, "GSTR1", "", Acknum_response, this._period, this._sessioncustid, this._sessionuserid);
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR1 Filed Successfully for this " + this._gstinno + " and " + this._period + " : Ack No - " + Acknum_response, "");
                    Helper.InsertAPICountTransactions(this._gstinno, this._period, "GSTR1 FILE", this._sessionuserid, this._sessioncustid);

                    // SMS & EMAIL Notification for GSTR1 Filing - Starts                    
                    string strMobileNo = Helper.GetCustomerDetails("MobileNo", "UserList", " WHERE UserId = '" + this._sessionuserid + "' AND ROWSTATUS = 1");
                    string strEmail = Helper.GetCustomerDetails("Email", "UserList", " WHERE UserId = '" + this._sessionuserid + "' AND ROWSTATUS = 1");
                    string strAdminMobileNo = Helper.GetCustomerDetails("MobileNo", "TBL_Customer", " WHERE CustId = '" + this._sessioncustid + "' AND ROWSTATUS = 1");
                    string strAdminEmail = Helper.GetCustomerDetails("Email", "TBL_Customer", " WHERE CustId = '" + this._sessioncustid + "' AND ROWSTATUS = 1");
                    
                    Notification.SendSMS(strMobileNo, "Congratulations!! Your GSTR - 1 is Filed Successfully!!!");
                    Notification.SendEmail(strEmail, "GSTR1 Filing Status", "Congratulations!! Your GSTR - 1 is Filed Successfully for this " + this._gstinno + " and " + this._period + ". Ack No - " + Acknum_response + ".");
                    if(strEmail != strAdminEmail)
                    {
                        Notification.SendSMS(strAdminMobileNo, "Congratulations!! Your GSTR - 1 is Filed Successfully!!!");
                        Notification.SendEmail(strAdminEmail, "GSTR1 Filing Status", "Congratulations!! Your GSTR - 1 is Filed Successfully for this " + this._gstinno + " and " + this._period + ". Ack No - " + Acknum_response + ".");
                    }
                    // SMS & EMAILNotification for GSTR1 Filing - Ends

                    strResponse = "GSTR1 Filed Successfully... Ack No. - " + Acknum_response;

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1FileResponse);
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR1 File " + root.error.message + " for this " + this._gstinno + " and " + this._period, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(this._gstinno, this._period, "GSTR1 FILE", this._sessionuserid, this._sessioncustid);
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