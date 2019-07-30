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
using System.Web;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR9API
{
    public class Gstr9DownloadApiCall
    {
        string GSTR9_Download_Response = "";
        string GST_GSTR9_AUTO = "", GST_GSTR9_GET = "";
        string gstin = "";
        string retperiod = "";
        string action = "";
        string refid = "";
        string URL = "";
        
        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";

     

        public void SendRequestAuto(string strGSTINNo, string strPeriod, string SessionCustId, string SessionUserId, string SessionUsername,out string returnJson)
        {
            GST_GSTR9_AUTO = ConfigurationManager.AppSettings["GSP_GSTR9"];
            string st_res = "";
            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;              
               
                URL = GST_GSTR9_AUTO + "?action="+ "CALRCDS" + "&gstin=" + gstin + "&ret_period=" + retperiod +"";                                                    
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", "114e28efe3334395a5ee5b454e8221d2");
                httpWebRequest.Headers.Add("api_version", "1.1");
                httpWebRequest.Headers.Add("userrole", "GSTR9");
                httpWebRequest.Headers.Add("rtn_typ", "GSTR9");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR9_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR9_Download_Response);
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
                    string  DecryptedJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));
                    st_res = DecryptedJsonData;
                    Gstr9AutoInsert(strGSTINNo, strPeriod, st_res);
                    Helper.InsertAuditLog(SessionCustId, SessionUsername, "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + strPeriod + " and Status - '" + st_res + "' and Reference Id - '" + refid + "'", "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR9 DOWNLOAD",  SessionUserId, SessionCustId);
                    st_res= "GSTR-9 " + action + " Downloaded Successfully";

                }
                else
                {
                   
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR9_Download_Response);
                    Helper.InsertAuditLog( SessionUserId, SessionUsername, "GSTR-9 " + action + " " + root.error.message + " for this " + strGSTINNo + " & " + strPeriod, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR9 DOWNLOAD", SessionUserId,  SessionCustId);
                    st_res= root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            returnJson = st_res;
        }


        public void SendRequestGetDet(string strGSTINNo, string strPeriod, string SessionCustId, string SessionUserId, string SessionUsername, out string returnJson)
        {
             
            GST_GSTR9_GET = ConfigurationManager.AppSettings["GSP_GSTR9"];
            string st_res = "";
            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                
                URL = GST_GSTR9_GET + "?action=" + "RECORDS" + "&gstin=" + gstin + "&ret_period=" + retperiod + "";                
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", "114e28efe3334395a5ee5b454e8221d2");
                httpWebRequest.Headers.Add("api_version", "1.1");
                httpWebRequest.Headers.Add("userrole", "GSTR9");
                httpWebRequest.Headers.Add("rtn_typ", "GSTR9");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR9_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR9_Download_Response);

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
                    string DecryptedJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));
                    st_res = DecryptedJsonData;
                    Helper.InsertAuditLog(SessionCustId, SessionUsername, "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + strPeriod + " and Status - '" + st_res + "' and Reference Id - '" + refid + "'", "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR9 DOWNLOAD", SessionUserId, SessionCustId);
                    st_res = "GSTR-9 " + action + " Downloaded Successfully";

                }
                else
                {

                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR9_Download_Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-9 " + action + " " + root.error.message + " for this " + strGSTINNo + " & " + strPeriod, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR9 DOWNLOAD", SessionUserId, SessionCustId);
                    st_res = root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            returnJson = st_res;
        }

        #region "Insert Auto Calc Data"
        private void  Gstr9AutoInsert(string StrGSTIN, string ToDate,string json_Data)
        {

           SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Retrieve_GSTR9_Summary_SA]", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp ", ToDate));
                cmd.Parameters.Add(new SqlParameter("@RecordContents",json_Data));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cmd.ExecuteNonQuery();
                Con.Close();              
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        #endregion

    }
}