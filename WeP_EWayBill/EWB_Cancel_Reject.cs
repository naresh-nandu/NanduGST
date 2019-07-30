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
using System.Text;
using System.Threading.Tasks;
using WeP_DAL;
using WeP_BAL;
using System.Threading;
using WeP_BAL.EwayBill;

namespace WeP_EWayBill
{
    public class EWB_Cancel_Reject
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        private static string EWB_GENResponse = "";
        static EWB_Response_Attributes EWB_Data = null;

        public void EWB_CANCEL(string strEwbNo,string strReason,string strRmrk, string JsonData, string GSTINNo, string SessionUserId, string SessionCustId, 
            string SessionUsername, out string OStatus, out string OResponse)
        {
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EWB_Request_Attributes objEWBGEN = new EWB_Request_Attributes();
                objEWBGEN.action = "CANEWB";
                objEWBGEN.data = payload;
                                
                string tempjsondata = JsonConvert.SerializeObject(objEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.Timeout = 1000000;
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

                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EWB_Response_Attributes>(DecryptjsonData);
                    string ewbno = finaldata.ewayBillNo;
                    string canceldate = finaldata.cancelDate;
                    EWAYBILLDataAccess.Insert_Cancel_SuccessData(GSTINNo, strStatus, Convert.ToInt32(strReason), strRmrk, ewbno, canceldate,Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Cancelled Successfully for this " + GSTINNo + " : EWayBill No - " + ewbno + " and Cancel Date : " + canceldate, "");

                    strResponse = "EWayBill Cancelled Successfully... EWayBill No - " + ewbno + " and Cancel Date : " + canceldate;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    EWAYBILLDataAccess.Insert_Cancel_ErrorData(strEwbNo,strStatus, strErrorCode, strErrorDesc, Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Cancellation Error Code - " + strErrorCode + " and Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

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

        public void EWB_UPDATE_TRANSPORTER(string strGSTIN,string strEwbNo, string strTransId,string JsonData,string SessionUserId, string SessionCustId,
            string SessionUsername, out string OStatus, out string OResponse)
        {
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(strGSTIN, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EWB_Request_Attributes objEWBGEN = new EWB_Request_Attributes();
                objEWBGEN.action = "UPDATETRANSPORTER";
                objEWBGEN.data = payload;
                
                string tempjsondata = JsonConvert.SerializeObject(objEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin",strGSTIN);
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

                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EWB_Response_Attributes>(DecryptjsonData);
                    if (finaldata != null)
                    {
                        string ewbNo = finaldata.ewayBillNo;
                        string transId = finaldata.transporterId;
                        string updTransDate = finaldata.updTransporterDate;

                        EWB_BusinessLayer.Insert_Update_TransPorter(strGSTIN, ewbNo, transId, updTransDate, 1, "", "", Convert.ToInt32(SessionUserId), Convert.ToInt32(SessionCustId));

                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Transporter Deatils Updated Successfully for this " + strGSTIN + " : EWayBill No - " + ewbNo + " and Upadted Transporter Date : " + updTransDate, "");
                        strResponse = "EWayBill Transporter Deatils Updated Successfully";
                    }


                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    EWB_BusinessLayer.Insert_Update_TransPorter(strGSTIN, strEwbNo,strTransId,"",0,strErrorCode, strErrorDesc, Convert.ToInt32(SessionUserId), Convert.ToInt32(SessionCustId));
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Transporter Deatils Updation Failed. Error Code - " + strErrorCode + " and Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);
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

        public void EWB_EXTEND_VALIDITY(string strGSTIN, string strEwbNo,string vechicleNo,string fromPlace,int fromStateCode,string remainingDist,
            string transDocNo,string transDocDate,int transMode,string extnRsnCode,string extnRmrk,string JsonData, string SessionUserId, string SessionCustId,
          string SessionUsername, out string OStatus, out string OResponse)
        {
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(strGSTIN, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EWB_Request_Attributes objEWBGEN = new EWB_Request_Attributes();
                objEWBGEN.action = "EXTENDVALIDITY";
                objEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.Timeout = 1000000;
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

                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EWB_Response_Attributes>(DecryptjsonData);
                    if (finaldata != null)
                    {
                        EWB_BusinessLayer.Insert_Extend_Validity(strGSTIN,finaldata.ewbNo,finaldata.vehicleNo, finaldata.fromPlace,Convert.ToInt32(finaldata.fromStateCode),finaldata.remainingDistance, finaldata.transDocNo, finaldata.transDocDate, finaldata.transMode, finaldata.extnRsnCode,finaldata.extnRemarks,1,"","",Convert.ToInt32(SessionUserId), Convert.ToInt32(SessionCustId));
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Validity Extended Successfully for this " + strGSTIN + " : EWayBill No - " + finaldata.ewbNo, "");
                        strResponse = "EWayBill Validity Extended Successfully";
                    }
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    EWB_BusinessLayer.Insert_Extend_Validity(strGSTIN,strEwbNo,"","",0,"","","","","","", 0, strErrorCode,strErrorDesc, Convert.ToInt32(SessionUserId), Convert.ToInt32(SessionCustId));
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Validity Extended Failed. Error Code - " + strErrorCode + " and Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);
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

        public static void EWB_REJECT(string strEwbNo, string JsonData, string GSTINNo, string SessionUserId, string SessionCustId, 
            string SessionUsername, out string OStatus, out string OResponse)
        {
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EWB_Request_Attributes objCONSEWBGEN = new EWB_Request_Attributes();
                objCONSEWBGEN.action = "REJEWB";
                objCONSEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objCONSEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.Timeout = 1000000;
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

                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EWB_Response_Attributes>(DecryptjsonData);
                    string ewbno = finaldata.ewayBillNo;
                    string rejectdate = finaldata.ewbRejectedDate;
                    EWAYBILLDataAccess.Insert_Reject_SuccessData(GSTINNo, strStatus, ewbno,rejectdate,Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Rejected Successfully for this " + GSTINNo + " : EWayBill No - " + ewbno + " & EWayBill Rejected Date : " + rejectdate, "");

                    strResponse = "EWayBill Rejected Successfully for this " + GSTINNo + " : EWayBill No - " + ewbno + " and EWayBill Rejected Date : " + rejectdate;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    EWAYBILLDataAccess.Insert_Reject_ErrorData(strEwbNo,strStatus, strErrorCode, strErrorDesc, Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Rejection Error Code - " + strErrorCode + " and Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

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
            string strStatus = "0", strResponse = "", strEWBNo = "";
            try
            {
                string[] strEWBIDS = strRefIds.Split(',');
                for (int i = 0; i < strEWBIDS.Count(); i++)
                {
                    string strJsonData = "{\"ewbNo\":" + strEWBIDS[i] + "}";
                    EWB_REJECT(strEWBIDS[i], strJsonData, strGSTINNo,
                    SessionUserId, SessionCustId, SessionUsername, out strStatus, out strResponse);

                    //Task.Factory.StartNew(() => EWB_REJECT(strEWBIDS[i], strJsonData, strGSTINNo,
                    //            SessionUserId, SessionCustId, SessionUsername, out strStatus, out strResponse)
                    //        );
                    //        Thread.Sleep(1000);

                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
               // EWAYBILLDataAccess.Insert_Reject_ErrorData(strStatus, strResponse, strResponse, Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Generation Error - " + strResponse, "");
            }
        }
    }
}
