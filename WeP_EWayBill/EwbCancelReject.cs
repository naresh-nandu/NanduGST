using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WeP_BAL;
using WeP_BAL.EwayBill;
using WeP_DAL;

namespace WeP_EWayBill
{
    public class EwbCancelReject
    {
        public readonly string _sessioncustid;
        public readonly string _sessionuserid;
        public readonly string _sessionusername;
        public EwbCancelReject(string SessionCustId, string SessionUserId, string SessionUsername)
        {
            this._sessioncustid = SessionCustId;
            this._sessionuserid = SessionUserId;
            this._sessionusername = SessionUsername;
        }

        public void EWB_CANCEL(string strEwbNo, string strReason, string strRmrk, string JsonData, string GSTINNo, out string OStatus, out string OResponse)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string EWB_GENResponse = "";
            EwbResponseAttributes EWB_Data = null;
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(receivedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EwbRequestAttributes objEWBGEN = new EwbRequestAttributes();
                objEWBGEN.action = "CANEWB";
                objEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

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
                        EWB_GENResponse = result.ToString();
                    }
                }

                EWB_Data = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EwbResponseAttributes>(DecryptjsonData);
                    string ewbno = finaldata.ewayBillNo;
                    string canceldate = finaldata.cancelDate;
                    EwaybillDataAccess.Insert_Cancel_SuccessData(GSTINNo, strStatus, Convert.ToInt32(strReason), strRmrk, ewbno, canceldate, Convert.ToInt32(this._sessioncustid), Convert.ToInt32(this._sessionuserid));
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "EWayBill Cancelled Successfully for this " + GSTINNo + " : EWayBill No - " + ewbno + " and Cancel Date : " + canceldate, "");

                    strResponse = "EWayBill Cancelled Successfully... EWayBill No - " + ewbno + " and Cancel Date : " + canceldate;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EwbResponseAttributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    EwaybillDataAccess.Insert_Cancel_ErrorData(strEwbNo, strStatus, strErrorCode, strErrorDesc, Convert.ToInt32(this._sessioncustid), Convert.ToInt32(this._sessionuserid));
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "EWayBill Cancellation Error Code - " + strErrorCode + " and Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

                    strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
        }

        public void EWB_UPDATE_TRANSPORTER(string strGSTIN, string strEwbNo, string strTransId, string transName, string JsonData, out string OStatus, out string OResponse)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string EWB_GENResponse = "";
            EwbResponseAttributes EWB_Data = null;
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(strGSTIN, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EwbRequestAttributes objEWBGEN = new EwbRequestAttributes();
                objEWBGEN.action = "UPDATETRANSPORTER";
                objEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", strGSTIN);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

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
                        EWB_GENResponse = result.ToString();
                    }
                }

                EWB_Data = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EwbResponseAttributes>(DecryptjsonData);
                    if (finaldata != null)
                    {
                        string ewbNo = finaldata.ewayBillNo;
                        string transId = finaldata.transporterId;
                        string updTransDate = finaldata.transUpdateDate;

                        EwbBusinessLayer.Insert_Update_TransPorter(strGSTIN, ewbNo, transId, transName, updTransDate, 1, "", "", Convert.ToInt32(this._sessionuserid), Convert.ToInt32(this._sessioncustid));

                        Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "EWayBill Transporter Deatils Updated Successfully for this " + strGSTIN + " : EWayBill No - " + ewbNo + " and Upadted Transporter Date : " + updTransDate, "");
                        strResponse = "EWayBill Transporter Details Updated Successfully";
                    }
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EwbResponseAttributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    EwbBusinessLayer.Insert_Update_TransPorter(strGSTIN, strEwbNo, strTransId, transName, "", 0, strErrorCode, strErrorDesc, Convert.ToInt32(this._sessionuserid), Convert.ToInt32(this._sessioncustid));
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "EWayBill Transporter Deatils Updation Failed. Error Code - " + strErrorCode + " and Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);
                    strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
        }

        public void EWB_EXTEND_VALIDITY(string strGSTIN, string strEwbNo, string vechicleNo, string fromPlace, int fromStateCode, string remainingDist,
            string transDocNo, string transDocDate, int transMode, string extnRsnCode, string extnRmrk, string JsonData, out string OStatus, out string OResponse)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string EWB_GENResponse = "";
            EwbResponseAttributes EWB_Data = null;
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(strGSTIN, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EwbRequestAttributes objEWBGEN = new EwbRequestAttributes();
                objEWBGEN.action = "EXTENDVALIDITY";
                objEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", strGSTIN);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

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
                        EWB_GENResponse = result.ToString();
                    }
                }

                EWB_Data = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EwbResponseAttributes>(DecryptjsonData);
                    if (finaldata != null)
                    {
                        EwbBusinessLayer.Insert_Extend_Validity(strGSTIN, finaldata.ewbNo, finaldata.vehicleNo, finaldata.fromPlace, Convert.ToInt32(finaldata.fromStateCode), finaldata.remainingDistance, finaldata.transDocNo, finaldata.transDocDate, finaldata.transMode, finaldata.extnRsnCode, finaldata.extnRemarks, 1, "", "", Convert.ToInt32(this._sessionuserid), Convert.ToInt32(this._sessioncustid));
                        Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "EWayBill Validity Extended Successfully for this " + strGSTIN + " : EWayBill No - " + finaldata.ewbNo, "");
                        strResponse = "EWayBill Validity Extended Successfully";
                    }
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EwbResponseAttributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    EwbBusinessLayer.Insert_Extend_Validity(strGSTIN, strEwbNo, "", "", 0, "", "", "", "", "", "", 0, strErrorCode, strErrorDesc, Convert.ToInt32(this._sessionuserid), Convert.ToInt32(this._sessioncustid));
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "EWayBill Validity Extended Failed. Error Code - " + strErrorCode + " and Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);
                    strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
        }

        public void EWB_REJECT(string strEwbNo, string JsonData, string GSTINNo, out string OStatus, out string OResponse)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string EWB_GENResponse = "";
            EwbResponseAttributes EWB_Data = null;
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EwbRequestAttributes objCONSEWBGEN = new EwbRequestAttributes();
                objCONSEWBGEN.action = "REJEWB";
                objCONSEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objCONSEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

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
                        EWB_GENResponse = result.ToString();
                    }
                }

                EWB_Data = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EwbResponseAttributes>(DecryptjsonData);
                    string ewbno = finaldata.ewayBillNo;
                    string rejectdate = finaldata.ewbRejectedDate;
                    EwaybillDataAccess.Insert_Reject_SuccessData(GSTINNo, strStatus, ewbno, rejectdate, Convert.ToInt32(this._sessioncustid), Convert.ToInt32(this._sessionuserid));
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "EWayBill Rejected Successfully for this " + GSTINNo + " : EWayBill No - " + ewbno + " & EWayBill Rejected Date : " + rejectdate, "");

                    strResponse = "EWayBill Rejected Successfully for this " + GSTINNo + " : EWayBill No - " + ewbno + " and EWayBill Rejected Date : " + rejectdate;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EwbResponseAttributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    EwaybillDataAccess.Insert_Reject_ErrorData(strEwbNo, strStatus, strErrorCode, strErrorDesc, Convert.ToInt32(this._sessioncustid), Convert.ToInt32(this._sessionuserid));
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "EWayBill Rejection Error Code - " + strErrorCode + " and Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

                    strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
        }


        public static void EWAYBILL_Reject_THREAD(string strRefIds, string strGSTINNo, string SessionUserId,
            string SessionCustId, string SessionUsername)
        {
            string strStatus = "", strResponse = "";
            try
            {
                string[] strEWBIDS = strRefIds.Split(',');
                for (int i = 0; i < strEWBIDS.Count(); i++)
                {
                    string strJsonData = "{\"ewbNo\":" + strEWBIDS[i] + "}";
                    new EwbCancelReject(SessionCustId, SessionUserId, SessionUsername).EWB_REJECT(strEWBIDS[i], strJsonData, strGSTINNo, out strStatus, out strResponse);
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
                Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Generation Error - " + strResponse, "");
            }
        }
    }
}
