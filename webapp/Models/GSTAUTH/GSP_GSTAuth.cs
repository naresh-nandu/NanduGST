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
    public partial class GSP_GSTAuth
    {
        private static string GSTAuthResponse = "";
        private static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        private static string EncryptedAppKey = "", Username = "";        

        public static string SendRequest(string strGSTINNo, string SessionUserId, string SessionCustId)
        {
            SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_AUTH_KEYS where AuthorizationToken = '" + strGSTINNo + "'", con);
            DataTable dt = new DataTable();
            adt.Fill(dt);
            if(dt.Rows.Count > 0)
            {
                Username = dt.Rows[0]["Username"].ToString().Trim();
                EncryptedAppKey = dt.Rows[0]["EncryptedAppKey"].ToString();
            }
            string strStateCode = strGSTINNo.Substring(0, 2); // State Code

            //string AuthorizeToken = GetAccessToken.getAuthToken();

            GSP_GSTAuthParameters objGSTAuth = new GSP_GSTAuthParameters();
            objGSTAuth.app_key = EncryptedAppKey;
            objGSTAuth.username = Username;
            objGSTAuth.action = "OTPREQUEST";

            string jsondata = new JavaScriptSerializer().Serialize(objGSTAuth);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_AUTHENTICATE"]);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json; charset=utf-8";
            //httpWebRequest.Headers.Add("Authorization", AuthorizeToken);
            //httpWebRequest.Headers.Add("username", Username);            
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

            //var data = (JObject)JsonConvert.DeserializeObject(GSTAuthResponse);
            //string status_response = data["ResponseObject"].Value<string>();
            var data = JsonConvert.DeserializeObject<GSPResponse>(GSTAuthResponse);
            string status_response = data.status_cd.ToString();
            if (status_response == "1")
            {
                status_response = "One Time Password has been sent to GSTIN registered mobile number";
                Helper.InsertAPICountTransactions(strGSTINNo, "", "OTP REQUEST", SessionUserId, SessionCustId);
            }
            else
            {
                var root = JsonConvert.DeserializeObject<GSPResponse>(GSTAuthResponse);
                status_response = root.error.message + " - " + "Error Code : " + root.error.error_cd;
                Helper.InsertAPICountTransactions(strGSTINNo, "", "OTP REQUEST", SessionUserId, SessionCustId);
            }
            return status_response;
        }
    }

    public class GSP_GSTAuthParameters
    {
        public string action { get; set; }
        public string username { get; set; }
        public string app_key { get; set; }
    }
}