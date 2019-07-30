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
using System.Threading;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR1
{
    public class GSP_SendGSTR1SaveDataModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "", DecryptedJsonData1 = "", WalletResponse = "";
        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        private static string GSTR1Response = "";
        private static string message = "";
        GSPResponse GST_Data = null;
        GSP_GetGSTR1RETSTATUSDataModel GSP_getGSTR1RETSTATUS = new GSP_GetGSTR1RETSTATUSDataModel();
        public string SendRequest(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId, string SessionCustId, string SessionUsername,
            string strB2BRefIds, string strB2CLRefIds, string strB2CSRefIds, string strCDNRRefIds, string strEXPRefIds, string strHSNRefIds,
            string strNILRefIds, string strTXPRefIds, string strATRefIds, string strDOCRefIds, string strCDNURRefIds,
            string strB2BARefIds, string strB2CLARefIds, string strB2CSARefIds, string strCDNRARefIds, string strEXPARefIds, 
            string strTXPARefIds, string strATARefIds, string strCDNURARefIds)
        {
            decimal WalletBalance = 0;
            string GSK = "";
            bool isChecked = false;
            try
            {
                Helper.GetWalletBalance(SessionCustId, "RETURN FILING", "GSTR1", GSTINNo, Period, out WalletBalance, out GSK, out isChecked);
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                                
                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GSP_GSTR1SaveParameters objGSTR1SaveParams = new GSP_GSTR1SaveParameters();
                objGSTR1SaveParams.data = payload;
                objGSTR1SaveParams.hmac = strhmac;
                objGSTR1SaveParams.action = "RETSAVE";

                string tempjsondata = JsonConvert.SerializeObject(objGSTR1SaveParams);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR1"]);
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
                        GSTR1Response = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1Response);

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
                            Helper.UpdateWalletBalance(SessionCustId, "RETURN FILING", 50, "GSTR1", GSTINNo, Period);
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

                    Helper.InsertGSTRSaveResponse(GSTINNo, "GSTR1", "", refid_response, Period, SessionCustId, SessionUserId);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Saved Successfully for this " + GSTINNo + " & " + Period + ". Action - " + strAction + " : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SAVE", SessionUserId, SessionCustId);
                    #region "UPDATE RETSTATUS & INVOICE FLAG"

                    #region "NON AMMENDMENTS"
                    if (!string.IsNullOrEmpty(strB2BRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2B", strB2BRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("B2B", GSTINNo, Period, strB2BRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strB2CLRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2CL", strB2CLRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("B2CL", GSTINNo, Period, strB2CLRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strCDNRRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "CDNR", strCDNRRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("CDNR", GSTINNo, Period, strCDNRRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strB2CSRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2CS", strB2CSRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("B2CS", GSTINNo, Period, strB2CSRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strEXPRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "EXP", strEXPRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("EXP", GSTINNo, Period, strEXPRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strHSNRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "HSN", strHSNRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("HSN", GSTINNo, Period, strHSNRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strNILRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "NIL", strNILRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("NIL", GSTINNo, Period, strNILRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strTXPRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "TXP", strTXPRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("TXP", GSTINNo, Period, strTXPRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strATRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "AT", strATRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("AT", GSTINNo, Period, strATRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strDOCRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "DOC", strDOCRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("DOC", GSTINNo, Period, strDOCRefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strCDNURRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "CDNUR", strCDNURRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("CDNUR", GSTINNo, Period, strCDNURRefIds, "", "1");
                    }
                    #endregion

                    #region "AMMENDMENTS"
                    if (!string.IsNullOrEmpty(strB2BARefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2BA", strB2BARefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("B2BA", GSTINNo, Period, strB2BARefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strB2CLARefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2CLA", strB2CLARefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("B2CLA", GSTINNo, Period, strB2CLARefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strCDNRARefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "CDNRA", strCDNRARefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("CDNRA", GSTINNo, Period, strCDNRARefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strB2CSARefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2CSA", strB2CSARefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("B2CSA", GSTINNo, Period, strB2CSARefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strEXPARefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "EXPA", strEXPARefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("EXPA", GSTINNo, Period, strEXPARefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strTXPARefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "TXPA", strTXPARefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("TXPA", GSTINNo, Period, strTXPARefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strATARefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "ATA", strATARefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("ATA", GSTINNo, Period, strATARefIds, "", "1");
                    }
                    if (!string.IsNullOrEmpty(strCDNURARefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "CDNURA", strCDNURARefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("CDNURA", GSTINNo, Period, strCDNURARefIds, "", "1");
                    }
                    #endregion

                    #endregion

                    GSP_GetGSTR1DataModel GSP_getGSTR1 = new GSP_GetGSTR1DataModel();
                    string DownloadRes = GSP_getGSTR1.SendRequest(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", "", "", SessionUserId, SessionCustId, SessionUsername,"");

                    return "GSTR1 Save is in Progress... Reference Id - " + refid_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Save " + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, "Error Code - " + root.error.error_cd);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void SendRequestVoid(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId, 
            string SessionCustId, string SessionUsername, string strB2BRefIds)
        {
            decimal WalletBalance = 0;
            string GSK = "";
            bool isChecked = false;
            try
            {
                Helper.GetWalletBalance(SessionCustId, "RETURN FILING", "GSTR1", GSTINNo, Period, out WalletBalance, out GSK, out isChecked);
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                
                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GSP_GSTR1SaveParameters objGSTR1SaveParams = new GSP_GSTR1SaveParameters();
                objGSTR1SaveParams.data = payload;
                objGSTR1SaveParams.hmac = strhmac;
                objGSTR1SaveParams.action = "RETSAVE";
                                
                string tempjsondata = JsonConvert.SerializeObject(objGSTR1SaveParams);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR1"]);
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
                        GSTR1Response = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1Response);

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
                            Helper.UpdateWalletBalance(SessionCustId, "RETURN FILING", 50, "GSTR1", GSTINNo, Period);
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

                    Helper.InsertGSTRSaveResponse(GSTINNo, "GSTR1", "", refid_response, Period, SessionCustId, SessionUserId);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Saved Successfully for this " + GSTINNo + " & " + Period + ". Action - " + strAction + " : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SAVE", SessionUserId, SessionCustId);
                    
                    #region "UPDATE RETSTATUS & INVOICE FLAG"
                    if (!string.IsNullOrEmpty(strB2BRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2B", strB2BRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("B2B", GSTINNo, Period, strB2BRefIds, "", "1");
                    }                    
                    #endregion

                    Thread.Sleep(1000);
                    GSP_getGSTR1RETSTATUS.SendRequestVoid(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", SessionUserId, SessionCustId, SessionUsername);
                    //Thread.Sleep(1000);
                    //Task.Factory.StartNew(() => GSP_getGSTR1RETSTATUS.SendRequestVoid(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", SessionUserId, SessionCustId, SessionUsername)
                    //);
                    //Thread.Sleep(1000);
                    //return "GSTR1 Save is in Progress... Reference Id - " + refid_response; // + " " + HttpContext.Current.Session["RETSTATUS"].ToString();
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Save " + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SAVE", SessionUserId, SessionCustId);
                }

            }
            catch (Exception ex)
            {
                Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Save" + ex.Message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, ex.Message);
            }
        }

        public void SendRequestVoid(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId,
            string SessionCustId, string SessionUsername, string strMinInvId, string strMaxInvId)
        {
            decimal WalletBalance = 0;
            string GSK = "";
            bool isChecked = false;
            try
            {
                Helper.GetWalletBalance(SessionCustId, "RETURN FILING", "GSTR1", GSTINNo, Period, out WalletBalance, out GSK, out isChecked);
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GSP_GSTR1SaveParameters objGSTR1SaveParams = new GSP_GSTR1SaveParameters();
                objGSTR1SaveParams.data = payload;
                objGSTR1SaveParams.hmac = strhmac;
                objGSTR1SaveParams.action = "RETSAVE";

                string tempjsondata = JsonConvert.SerializeObject(objGSTR1SaveParams);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR1"]);
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
                        GSTR1Response = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1Response);

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
                            Helper.UpdateWalletBalance(SessionCustId, "RETURN FILING", 50, "GSTR1", GSTINNo, Period);
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

                    Helper.InsertGSTRSaveResponse(GSTINNo, "GSTR1", "", refid_response, Period, SessionCustId, SessionUserId);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Saved Successfully for this " + GSTINNo + " & " + Period + ". Action - " + strAction + " : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SAVE", SessionUserId, SessionCustId);

                    #region "UPDATE RETSTATUS & INVOICE FLAG"
                    if (!string.IsNullOrEmpty(strMinInvId) && !string.IsNullOrEmpty(strMaxInvId))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, strAction, "1", "", SessionCustId, SessionUserId, strMinInvId, strMaxInvId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1(strAction, GSTINNo, Period, "", "1", strMinInvId, strMaxInvId);
                    }
                    #endregion

                    Thread.Sleep(8000);
                    GSP_getGSTR1RETSTATUS.SendRequestVoid(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", SessionUserId, SessionCustId, SessionUsername);
                    //Thread.Sleep(1000);
                    //Task.Factory.StartNew(() => GSP_getGSTR1RETSTATUS.SendRequestVoid(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", SessionUserId, SessionCustId, SessionUsername)
                    //);
                    //Thread.Sleep(1000);
                    //return "GSTR1 Save is in Progress... Reference Id - " + refid_response; // + " " + HttpContext.Current.Session["RETSTATUS"].ToString();
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Save " + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SAVE", SessionUserId, SessionCustId);
                }

            }
            catch (Exception ex)
            {
                Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Save" + ex.Message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, ex.Message);
            }
        }

        public void SendRequestVoidCDNR(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId
            , string SessionCustId, string SessionUsername, string strCDNRRefIds)
        {
            decimal WalletBalance = 0;
            string GSK = "";
            bool isChecked = false;
            try
            {
                Helper.GetWalletBalance(SessionCustId, "RETURN FILING", "GSTR1", GSTINNo, Period, out WalletBalance, out GSK, out isChecked);
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                
                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GSP_GSTR1SaveParameters objGSTR1SaveParams = new GSP_GSTR1SaveParameters();
                objGSTR1SaveParams.data = payload;
                objGSTR1SaveParams.hmac = strhmac;
                objGSTR1SaveParams.action = "RETSAVE";

                string tempjsondata = JsonConvert.SerializeObject(objGSTR1SaveParams);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR1"]);
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
                        GSTR1Response = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1Response);

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
                            Helper.UpdateWalletBalance(SessionCustId, "RETURN FILING", 50, "GSTR1", GSTINNo, Period);
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

                    Helper.InsertGSTRSaveResponse(GSTINNo, "GSTR1", "", refid_response, Period, SessionCustId, SessionUserId);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Saved Successfully for this " + GSTINNo + " & " + Period + ". Action - " + strAction + " : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SAVE", SessionUserId, SessionCustId);
                    
                    #region "UPDATE RETSTATUS & INVOICE FLAG"                    
                    if (!string.IsNullOrEmpty(strCDNRRefIds))
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "CDNR", strCDNRRefIds, "1", "", SessionCustId, SessionUserId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1("CDNR", GSTINNo, Period, strCDNRRefIds, "", "1");
                    }
                    #endregion

                    Thread.Sleep(1000);
                    Task.Factory.StartNew(() => GSP_getGSTR1RETSTATUS.SendRequestVoid(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", SessionUserId, SessionCustId, SessionUsername)
                    );
                    Thread.Sleep(1000);
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Save" + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SAVE", SessionUserId, SessionCustId);
                }

            }
            catch (Exception ex)
            {
                Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR1 Save" + ex.Message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, ex.Message);
            }
        }                        
    }

    public class GSP_GSTR1SaveParameters
    {
        public string action { get; set; }
        public string data { get; set; }
        public string hmac { get; set; }
    }

    
}