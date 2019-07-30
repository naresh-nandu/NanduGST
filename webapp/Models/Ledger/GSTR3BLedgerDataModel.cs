using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTAUTH;
using SmartAdminMvc.Models.Ledger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using WeP_BAL.Ledger;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR3
{
    public class Gstr3BLedgerDataModel
    {
        public string SendRequest_Ledger(string strGSTINNo,string strPeriod,string strAction,string fromDate, string toDate, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string GST_Path = "";
            string DecryptedJsonData = "";
            string GSTR_Ledger_Response = "";
            string gstin = "";
            string retperiod = "";
            string URL = "";

            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";

            GST_Path = ConfigurationManager.AppSettings["GSP_Ledger"];
            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                string strStateCode = strGSTINNo.Substring(0, 2);
                retperiod = strPeriod;
                gstin = strGSTINNo;
                if (strAction=="BAL" || strAction == "TAX")
                {
                    URL = GST_Path + "?action=" + strAction + "&gstin=" + strGSTINNo + "&ret_period=" + strPeriod;
                }
                else
                {
                    URL = GST_Path + "?action=" + strAction + "&gstin=" + strGSTINNo + "&ft_dt=" + fromDate + "&to_dt=" + toDate;
                }
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR_Ledger_Response = result.ToString();
                }
                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR_Ledger_Response);
                
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
                    var finaldata = JsonConvert.DeserializeObject<CashItcBalance>(DecryptedJsonData);

                    decimal Ledger_Sgst = finaldata.cash_bal.sgst_tot_bal;
                    decimal Ledger_Cgst = finaldata.cash_bal.cgst_tot_bal;
                    decimal Ledger_Igst = finaldata.cash_bal.igst_tot_bal;
                    decimal Ledger_Cess = finaldata.cash_bal.cess_tot_bal;
                    decimal Itc_Cgst = finaldata.itc_bal.cgst_bal;
                    decimal Itc_Sgst = finaldata.itc_bal.sgst_bal;
                    decimal Itc_Igst = finaldata.itc_bal.igst_bal;
                    decimal Itc_Cess = finaldata.itc_bal.cess_bal;
                    LedgerBal.Insert_ITC_Ledger(strGSTINNo, strPeriod, Ledger_Sgst, Ledger_Cgst, Ledger_Igst, Ledger_Cess,
                        Itc_Sgst, Itc_Cgst, Itc_Igst, Itc_Cess, Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                  
                    return DecryptedJsonData;
                }                
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR_Ledger_Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "LiabilityLedger " + strAction + " " + root.error.message + " for this " + strGSTINNo + " & Period Between " + fromDate+ "&"+ toDate, "Error Code - " + root.error.error_cd);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }            
        }

        public string SendRequest(string strGSTINNo, string strPeriod, string SessionUserId, string SessionCustId, string SessionUsername, int ledgerApi)
        {
            string DecryptedJsonData = "";
            string GSTR3B_LiabilityLedger_Response = "";
            string gstin = "";
            string retperiod = "";
            string URL = "";

            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";

            var GetApiPath = "";
            if (ledgerApi == 1)
            {
                GetApiPath = ConfigurationManager.AppSettings["GSP_LEDGER"];
            }
            else if (ledgerApi == 2)
            {
                GetApiPath = ConfigurationManager.AppSettings["GSP_LEDGER"];
            }
            else if (ledgerApi == 3)
            {
                GetApiPath = ConfigurationManager.AppSettings["GSP_LEDGER"];
            }

            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                
                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                var todayPeriod = DateTime.Now.ToString("MMyyyy");
                if (ledgerApi == 2)
                {
                    URL = GetApiPath + "&gstin=" + gstin + "&fr_dt=" + retperiod + "&to_dt=" + todayPeriod;
                }
                else
                {
                    URL = GetApiPath + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR3B_LiabilityLedger_Response = result.ToString();
                }
                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR3B_LiabilityLedger_Response);
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
                    return DecryptedJsonData;
                }                
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR3B_LiabilityLedger_Response);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ex.Message;
            }
        }
    }
}