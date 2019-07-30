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

namespace SmartAdminMvc.Models.GSTR1
{
    public class GspSendGstr1SaveDataModel
    {
        public readonly string _sessioncustid;
        public readonly string _sessionuserid;
        public readonly string _sessionusername;

        public GspSendGstr1SaveDataModel(string SessionCustId, string SessionUserId, string SessionUsername)
        {
            this._sessioncustid = SessionCustId;
            this._sessionuserid = SessionUserId;
            this._sessionusername = SessionUsername;
        }

        public void SendRequestVoid(string JsonData, string strAction, string GSTINNo, string Period, string strMinInvId, string strMaxInvId)
        {            
            string Username = "", AppKey = "", EncryptedSEK = "", AuthToken = "";
            string GSTR1Response = "";
            GSPResponse GST_Data = null;
            GspGetGstr1RetstatusDataModel GSP_getGSTR1RETSTATUS = new GspGetGstr1RetstatusDataModel();
            decimal WalletBalance = 0;
            string GSK = "";
            bool isChecked = false;
            try
            {
                Helper.GetWalletBalance(this._sessioncustid, "RETURN FILING", "GSTR1", GSTINNo, Period, out WalletBalance, out GSK, out isChecked);
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GstRequestResponseAttributes objGSTR1SaveParams = new GstRequestResponseAttributes();
                objGSTR1SaveParams.data = payload;
                objGSTR1SaveParams.hmac = strhmac;
                objGSTR1SaveParams.action = "RETSAVE";

                string tempjsondata = JsonConvert.SerializeObject(objGSTR1SaveParams);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR1"]);
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
                            Helper.UpdateWalletBalance(this._sessioncustid, "RETURN FILING", 50, "GSTR1", GSTINNo, Period);
                            Helper.UpdateWalletBalanceRequest(this._sessioncustid, 50, "RETURN FILING");
                        }
                    }
                    string data_response = GST_Data.data.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);
                    string refid_response = finaldata["reference_id"].Value<string>();

                    Helper.InsertGSTRSaveResponse(GSTINNo, "GSTR1", "", refid_response, Period, this._sessioncustid, this._sessionuserid);
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR1 Saved Successfully for this " + GSTINNo + " & " + Period + ". Action - " + strAction + " : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SAVE", this._sessionuserid, this._sessioncustid);

                    #region "UPDATE RETSTATUS & INVOICE FLAG"
                    if (!string.IsNullOrEmpty(strMinInvId) && !string.IsNullOrEmpty(strMaxInvId))
                    {
                        Gstr1Retstatus.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, strAction, "1", "", this._sessioncustid, this._sessionuserid, strMinInvId, strMaxInvId);
                        GSTR1Helper.Update_Invoice_Flag_GSTR1(strAction, GSTINNo, Period, "", "1", strMinInvId, strMaxInvId);
                    }
                    #endregion

                    Thread.Sleep(8000);
                    GSP_getGSTR1RETSTATUS.SendRequestVoid(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", this._sessionuserid, this._sessioncustid, this._sessionusername);
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1Response);
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR1 Save " + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR1 SAVE", this._sessionuserid, this._sessioncustid);
                }

            }
            catch (Exception ex)
            {
                Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR1 Save" + ex.Message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, ex.Message);
            }
        }

    }
}