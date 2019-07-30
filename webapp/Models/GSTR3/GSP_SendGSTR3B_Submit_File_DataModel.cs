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

namespace SmartAdminMvc.Models.GSTR3
{
    public class GSP_SendGSTR3B_Submit_File_DataModel
    {
        public WePGSPDBEntities db = new WePGSPDBEntities();

        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "", DecryptedJsonData1 = "";
        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        private static string GSTR1SubmitResponse = "", GSTR1FileResponse = "";
        GSPResponse GST_Data = null;

        public string GSTR3BSubmitRequest(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId, 
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

                GSP_GSTR3BSubmitParameters objGSTR3BSubmitParams = new GSP_GSTR3BSubmitParameters();
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
                        //File.WriteAllText(@"E:\GSTR1Filing_Response.json", GSTR1Response);
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
                    
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR3B Submitted Successfully for this " + GSTINNo + " & " + Period + ", : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR3B SUBMIT", SessionUserId, SessionCustId);
                    //int UserId = Convert.ToInt32(SessionUserId);
                    //string strMobileNo = (from lst in db.UserLists
                    //                   where lst.UserId == UserId
                    //                   select lst.MobileNo).SingleOrDefault();
                    //Common.Notification.SendSMS(strMobileNo, string.Format("Congratulations!! Your GSTR - 1 is Submitted Successfully!!!"));
                    //GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "RETSUBMIT", "", "1", "", SessionCustId, SessionUserId);

                    //GSP_GetGSTR1DataModel GSP_getGSTR1 = new GSP_GetGSTR1DataModel();
                    //string DownloadRes = GSP_getGSTR1.SendRequest(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", SessionUserId, SessionCustId, SessionUsername);

                    return "GSTR3B Submitted Successfully... Reference Id - " + refid_response;

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1SubmitResponse);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Submit " + root.error.message + " for this " + GSTINNo + " & " + Period, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SUBMIT", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GSTR3BFileRequest(string JsonData, string GSTINNo, string Period, string strDSC, string strPANNo, 
            string SessionUserId, string SessionCustId, string SessionUsername)
        {
            try
            {
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = GSTINNo.Substring(0, 2); // State Code

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                string Base64JsonPayload = Helper.Base64Encode(JsonData.Trim());
                string payload = Helper.Encrypt(Base64JsonPayload, authEK);

                string strSignedData = string.Empty;
                string sha256ForSignData = string.Empty;
                using (var sha256Obj = SHA256.Create())
                {
                    byte[] outputBytes = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(Base64JsonPayload));
                    sha256ForSignData = BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
                }
                strSignedData = DSCDataModel.SignData(sha256ForSignData, strDSC);

                GSP_GSTR3BFileParameters objGSTR3BFileParams = new GSP_GSTR3BFileParameters();
                objGSTR3BFileParams.action = "RETFILE";
                objGSTR3BFileParams.data = payload; // strdata;
                objGSTR3BFileParams.sign = strSignedData;
                objGSTR3BFileParams.st = "DSC";
                objGSTR3BFileParams.sid = strPANNo;

                string tempjsondata = new JavaScriptSerializer().Serialize(objGSTR3BFileParams);
                
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
                        //File.WriteAllText(@"E:\GSTR1_Filing_Response_" + GSTINNo + ".txt", GSTR1Response);
                    }
                }

                #region "Decrypting Response"
                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1FileResponse);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string hmac_response = GST_Data.hmac.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));
                    
                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);

                    string laststatus_response = finaldata["status"].Value<string>();
                    string Acknum_response = finaldata["Ack_num"].Value<string>();

                    Helper.InsertGSTRFileResponse(GSTINNo, "GSTR3B", laststatus_response, Acknum_response, Period, SessionCustId, SessionUserId);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR3B Filed Successfully for this " + GSTINNo + " & " + Period + ", : Ack No - " + Acknum_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR3B FILE", SessionUserId, SessionCustId);
                    //int UserId = Convert.ToInt32(SessionUserId);
                    //string strMobileNo = (from lst in db.UserLists
                    //                   where lst.UserId == UserId
                    //                   select lst.MobileNo).SingleOrDefault();
                    //Common.Notification.SendSMS(strMobileNo, string.Format("Congratulations!! Your GSTR - 1 is Filed Successfully!!!"));
                    return "GSTR3B Filed Successfully... Status - " + laststatus_response + " and Ack No. - " + Acknum_response;

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1FileResponse);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR3B File " + root.error.message + " for this " + GSTINNo + " & " + Period, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR3B FILE", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
                #endregion
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    public class GSP_GSTR3BSubmitParameters
    {
        public string action { get; set; }
        public string data { get; set; }
        public string hmac { get; set; }
    }

    public class GSP_GSTR3BFileParameters
    {
        public string action { get; set; }
        public string data { get; set; }
        public string sign { get; set; }
        public string st { get; set; }
        public string sid { get; set; }
    }
}