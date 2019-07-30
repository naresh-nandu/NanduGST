using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.IO;
using System.Reflection;
using System.Net;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using SmartAdminMvc.Models.Common;
using WeP_DAL;
using WeP_GSTN;

namespace SmartAdminMvc.Models.GSTAUTH
{
    public partial class GspGstAuth
    {
        private static string GSTAuthResponse = "";
        private static string EncryptedAppKey = "", Username = "";        

        protected GspGstAuth()
        {
            //
        }

        public static string SendRequest(string strGSTINNo, string SessionUserId, string SessionCustId, string SessionUserName)
        {
            Helper.GSTR_Auth_DataAdapter(strGSTINNo, out Username, out EncryptedAppKey);
            string strStateCode = strGSTINNo.Substring(0, 2); // State Code

            GstRequestResponseAttributes objGSTAuth = new GstRequestResponseAttributes();
            objGSTAuth.app_key = EncryptedAppKey;
            objGSTAuth.username = Username;
            objGSTAuth.action = "OTPREQUEST";

            string jsondata = new JavaScriptSerializer().Serialize(objGSTAuth);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_AUTHENTICATE"]);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json; charset=utf-8";
            httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
            httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
            httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
            httpWebRequest.Headers.Add("state-cd", strStateCode);
            httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
            httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);
            httpWebRequest.Headers.Add("gstin", strGSTINNo);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsondata);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTAuthResponse = result.ToString();
                }
            }

            var data = JsonConvert.DeserializeObject<GSPResponse>(GSTAuthResponse);
            string status_response = data.status_cd.ToString();
            if (status_response == "1")
            {
                status_response = "One Time Password has been sent to GSTIN registered mobile number";
                Helper.InsertAPICountTransactions(strGSTINNo, "", "OTP REQUEST", SessionUserId, SessionCustId);
                Helper.InsertAuditLog(SessionUserId, SessionUserName, "OTP REQUESTED for this " + strGSTINNo, "");
            }
            else
            {
                var root = JsonConvert.DeserializeObject<GSPResponse>(GSTAuthResponse);
                status_response = root.error.message + " - " + "Error Code : " + root.error.error_cd;
                Helper.InsertAPICountTransactions(strGSTINNo, "", "OTP REQUEST", SessionUserId, SessionCustId);
                Helper.InsertAuditLog(SessionUserId, SessionUserName, "OTP REQUEST - " + status_response + " for this " + strGSTINNo, "");
            }
            return status_response;
        }

        public static void GSTAuthentication(string strGSTINNo, string SessionUserId, string SessionCustId, string SessionUserName, out string POPUPValue, out string AUTHResponse)
        {
            string strPOPUPValue = "", strAUTHResponse = "";
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                if(sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.CommandText = "Select TOP 1 * from TBL_AUTH_KEYS where AuthorizationToken = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            var varFinish = DateTime.Now;
                            var varValue = dt.Rows[0]["CreatedDate"];
                            TimeSpan varTime = varFinish - (DateTime)varValue;
                            int intMinutes = (int)varTime.TotalMinutes;

                            if (Convert.ToInt32(dt.Rows[0]["Expiry"]) > intMinutes)
                            {
                                if (300 >= intMinutes) // 5 Hours
                                {
                                    strPOPUPValue = "CLOSE_POPUP";
                                }
                                else
                                {
                                    GenerateKeys.GeneratingEncryptedKeys(strGSTINNo);
                                    strAUTHResponse = GspGstAuth.SendRequest(strGSTINNo, SessionUserId, SessionCustId, SessionUserName);
                                    strPOPUPValue = "OPEN_POPUP";
                                }
                            }
                            else
                            {
                                GenerateKeys.GeneratingEncryptedKeys(strGSTINNo);
                                strAUTHResponse = GspGstAuth.SendRequest(strGSTINNo, SessionUserId, SessionCustId, SessionUserName);
                                strPOPUPValue = "OPEN_POPUP";
                            }
                        }
                    }
                }
                sqlcon.Close();
            }
            POPUPValue = strPOPUPValue;
            AUTHResponse = strAUTHResponse;
        }
    }
}