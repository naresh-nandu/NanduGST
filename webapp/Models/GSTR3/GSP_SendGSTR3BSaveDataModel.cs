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

namespace SmartAdminMvc.Models.GSTR3
{
    public class GSP_SendGSTR3BSaveDataModel
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "", DecryptedJsonData1 = "";
        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        private static string GSTR3BResponse = "";
        private static string message = "";
        GSPResponse GST_Data = null;

        public string SendRequest(string JsonData, string GSTINNo, string Period, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            decimal WalletBalance = 0, WalletMinBalance = 0;
            string GSK = "";
            bool isChecked = false;
            try
            {
                Helper.GetWalletBalance(SessionCustId, "RETURN FILING", "GSTR3B", GSTINNo, Period, out WalletBalance, out GSK, out isChecked);
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                
                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);
                GSP_GSTR3BSaveParameters objGSTR3BSaveParams = new GSP_GSTR3BSaveParameters();
                objGSTR3BSaveParams.data = payload;
                objGSTR3BSaveParams.hmac = strhmac;
                objGSTR3BSaveParams.action = "RETSAVE";

                string tempjsondata = new JavaScriptSerializer().Serialize(objGSTR3BSaveParams);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR3B"]);
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
                        GSTR3BResponse = result.ToString();
                    }
                }
                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR3BResponse);

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
                    Helper.InsertGSTRSaveResponse(GSTINNo, "GSTR3B", "", refid_response, Period, SessionCustId, SessionUserId);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR3B Saved Successfully for this " + GSTINNo + " & " + Period + " : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR3B SAVE", SessionUserId, SessionCustId);

                    return "GSTR3B Save is in Progress... Reference Id - " + refid_response; // + " " + HttpContext.Current.Session["RETSTATUS"].ToString();
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR3BResponse);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR3B Save " + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + "", "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR3B SAVE", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public class GSP_GSTR3BSaveParameters
        {
            public string action { get; set; }
            public string data { get; set; }
            public string hmac { get; set; }
        }
    }
}



