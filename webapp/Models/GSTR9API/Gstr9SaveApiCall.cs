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
using WeP_GSTN;
using SmartAdminMvc.Models.GSTR1;

namespace SmartAdminMvc.Models.GSTR9API
{
    public class Gstr9SaveApiCall
    {
        public  void SendRequestGSTR9(string JsonData, string GSTINNo, string Period,int userid,int custid, string strAction,out string ReferenceId, out string Error)
        {
            string Username = "", AppKey = "", EncryptedSEK = "", AuthToken = "";
            string GSTR9Response = "";
            string Reference_Id = "";
            string ErrorRecord = "";
            GSPResponse GST_Data = null;


             try
            {
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GstRequestResponseAttributes objGSTR9SaveParams = new GstRequestResponseAttributes();
                objGSTR9SaveParams.data = payload;
                objGSTR9SaveParams.hmac = strhmac;
                objGSTR9SaveParams.action = "RETSAVE";

                string tempjsondata = JsonConvert.SerializeObject(objGSTR9SaveParams);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR9_Save"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PUT";
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
                httpWebRequest.Headers.Add("api_version", "1.1");
                httpWebRequest.Headers.Add("userrole", "GSTR9");
                httpWebRequest.Headers.Add("rtn_typ", "GSTR9");
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
                        GSTR9Response = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR9Response);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {

                    string data_response = GST_Data.data.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));
                    Helper.InsertAuditLog(Convert.ToString(custid), Username, "GSTR-9 Downloaded Successfully for this " + GSTINNo + " & " + Period + " and Status - '" + DecryptjsonData + "' and Reference Id - '", "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR9 DOWNLOAD", Convert.ToString(userid), Convert.ToString(custid));
                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);
                    Reference_Id = finaldata["reference_id"].Value<string>();
                   
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR9Response);
                    Helper.InsertAuditLog(userid.ToString(), Username, "GSTR9 Save " + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR9 SAVE", userid.ToString(),custid.ToString());
                    Reference_Id = root.error.message;

                }
            }
            catch (Exception ex)
            {
                Helper.InsertAuditLog( userid.ToString(),Username, "GSTR9 Save" + ex.Message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, ex.Message);
                ErrorRecord = ex.Message;

            }
            ReferenceId = Reference_Id;
            Error = ErrorRecord;
        }
    }
}