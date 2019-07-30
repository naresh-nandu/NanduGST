using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTAUTH;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using WeP_BAL.ViewAndTrack;
using WeP_DAL;

namespace SmartAdminMvc.Models.SearchTaxPayer
{
    public partial class GspSearchTaxPayerGstin
    {
        static GSPResponse GST_Data = null;
        private static string SearchTaxPayer_Response = "";
        private static string GST_Path = "";
        static string gstin = "";
        static string URL = "";

        protected GspSearchTaxPayerGstin()
        {
            
        }

        public static string SendRequest(string strGSTINNo, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string response = "";
            GST_Path = ConfigurationManager.AppSettings["GSP_PUBLICAPI"];
            try
            {
                gstin = strGSTINNo;
                URL = GST_Path + "?action=TP&gstin=" + gstin;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    SearchTaxPayer_Response = result.ToString();
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(SearchTaxPayer_Response);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "TaxPayer Search Done Successfully for this " + strGSTINNo, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, "", "TAXPAYER COMMON API", SessionUserId, SessionCustId);
                    return status_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(SearchTaxPayer_Response);
                    
                    Helper.InsertAPICountTransactions(strGSTINNo, "", "TAXPAYER COMMON API", SessionUserId, SessionCustId);
                    if (root.error != null)
                    {
                        response = root.error.message;
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "Taxpayer Search " + root.error.message + " for this " + strGSTINNo + ". Action - TP", "Error Code - " + root.error.error_cd);
                        return response;
                    }
                    else
                    {
                        response = "No Response from GSTIN or Invalid GSTIN";
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "Taxpayer Search " + response + " for this " + strGSTINNo + ". Action - TP", "Error Code - " + response);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void SendRequest(string strGSTINNo, string SessionUserId, string SessionCustId, string SessionUsername, out int iStatus, out string strResponse)
        {
            int status = 0;
            string response = "";
            GST_Path = ConfigurationManager.AppSettings["GSP_PUBLICAPI"];
            try
            {
                gstin = strGSTINNo;
                URL = GST_Path + "?action=TP&gstin=" + gstin;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    SearchTaxPayer_Response = result.ToString();
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(SearchTaxPayer_Response);

                string status_response = GST_Data.status_cd.ToString();
                status = Convert.ToInt32(status_response);
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    response = data_response;
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "TaxPayer Search Done Successfully for this " + strGSTINNo, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, "", "TAXPAYER COMMON API", SessionUserId, SessionCustId);
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(SearchTaxPayer_Response);
                    Helper.InsertAPICountTransactions(strGSTINNo, "", "TAXPAYER COMMON API", SessionUserId, SessionCustId);
                    if (root.error != null)
                    {
                        response = root.error.message;
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "Taxpayer Search " + root.error.message + " for this " + strGSTINNo + ". Action - TP", "Error Code - " + root.error.error_cd);
                    }
                    else
                    {
                        response = "No Response from GSTIN or Invalid GSTIN";
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "Taxpayer Search " + response + " for this " + strGSTINNo + ". Action - TP", "Error Code - " + response);
                    }
                }

            }
            catch (Exception ex)
            {
                status = 0;
                response = ex.Message;
            }
            iStatus = status;
            strResponse = response;
        }

        public static void SendRequest_ViewAndTrack(string strGSTINNo,string financialYear,string sourceType,string SessionUserId, string SessionCustId, string SessionUsername,string FileName, out int iStatus, out string strResponse)
        {
            int status = 0;
            string response = "";
            GST_Path = ConfigurationManager.AppSettings["GSP_VIEWANDTRACK"];
            try
            {
                gstin = strGSTINNo;
                URL = GST_Path + "?action=RETTRACK&fy=" + financialYear+"&gstin=" + gstin;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    SearchTaxPayer_Response = result.ToString();
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(SearchTaxPayer_Response);

                string status_response = GST_Data.status_cd.ToString();
                status = Convert.ToInt32(status_response);
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    response = data_response;
                    byte[] authEK = AESEncryption.decodeBase64StringTOByte(response);
                    string DecryptedJsonData = Helper.openFileToString(authEK);
                    new ViewAndTrackStatus(Convert.ToInt32(SessionUserId), Convert.ToInt32(SessionCustId), FileName, strGSTINNo,sourceType).insert_View_and_Track(DecryptedJsonData);
                     Helper.InsertAuditLog(SessionUserId, SessionUsername, "View and Track Filing Status Done Successfully for this " + strGSTINNo, "");
                     Helper.InsertAPICountTransactions(strGSTINNo, "", "View and Track Filing Status", SessionUserId, SessionCustId);
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(SearchTaxPayer_Response);
                    Helper.InsertAPICountTransactions(strGSTINNo, "", "View and Track Filing Status", SessionUserId, SessionCustId);
                    if (root.error != null)
                    {
                        response = root.error.message;
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "View and Track Filing Status " + root.error.message + " for this " + strGSTINNo + ". Action - TP", "Error Code - " + root.error.error_cd);
                    }
                    else
                    {
                        response = "No Response from GSTIN or Invalid GSTIN";
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "View and Track Filing Status " + response + " for this " + strGSTINNo + ". Action - TP", "Error Code - " + response);
                    }
                }

            }
            catch (Exception ex)
            {
                status = 0;
                response = ex.Message;
            }
            iStatus = status;
            strResponse = response;
        }
    }
}