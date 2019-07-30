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

namespace SmartAdminMvc.Models.GSTAUTH
{
    public partial class GspGstEvcOtp
    {
        private static string GSTAuthResponse = "";
        private static string AuthToken = "", Username = "", URL = "";

        protected GspGstEvcOtp()
        {
            //
        }

        public static string SendRequest(string strGSTINNo, string strPANNo, string strFormType, string SessionUserId, string SessionCustId)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                sqlcon.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.CommandText = "Select * from TBL_AUTH_KEYS where AuthorizationToken = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Username = dt.Rows[0]["Username"].ToString().Trim();
                            AuthToken = dt.Rows[0]["AuthToken"].ToString();
                        }
                    }
                }
                sqlcon.Close();
            }
            string strStateCode = strGSTINNo.Substring(0, 2); // State Code
            URL = ConfigurationManager.AppSettings["GSP_AUTHENTICATE"] + "?action=EVCOTP&pan=" + strPANNo + "&form_type=" + strFormType + "&gstin=" + strGSTINNo;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json; charset=utf-8";
            httpWebRequest.Headers.Add("auth-token", AuthToken);
            httpWebRequest.Headers.Add("username", Username);
            httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
            httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
            httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
            httpWebRequest.Headers.Add("state-cd", strStateCode);
            httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
            httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);
            httpWebRequest.Headers.Add("gstin", strGSTINNo);
                        
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                GSTAuthResponse = result.ToString();
            }

            var data = JsonConvert.DeserializeObject<GSPResponse>(GSTAuthResponse);
            string status_response = data.status_cd.ToString();
            if (status_response == "1")
            {
                status_response = "One Time Password has been sent to GSTIN registered mobile number for EVC Filing";
                Helper.InsertAPICountTransactions(strGSTINNo, "", "OTP REQUEST", SessionUserId, SessionCustId);
            }
            else
            {
                var root = JsonConvert.DeserializeObject<GSPResponse>(GSTAuthResponse);
                status_response = root.error.message + " - " + "Error Code : " + root.error.error_cd;
                Helper.InsertAPICountTransactions(strGSTINNo, "", "EVC OTP REQUEST", SessionUserId, SessionCustId);
            }
            return status_response;
        }
    }
}