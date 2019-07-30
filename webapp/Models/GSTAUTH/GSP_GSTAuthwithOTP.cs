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
    public partial class GSP_GSTAuthwithOTP
    {
        private static string GSTAuthResponse = "";
        
        private string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        private static string EncryptedAppKey = "";
        private static string EncryptedOTP = "", Username = "";

        public static string SendRequest(string strGSTINNo, string SessionUserId, string SessionCustId)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            con.Open();
            SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_AUTH_KEYS where AuthorizationToken = '" + strGSTINNo + "'", con);
            DataTable dt = new DataTable();
            adt.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                Username = dt.Rows[0]["Username"].ToString().Trim();
                EncryptedAppKey = dt.Rows[0]["EncryptedAppKey"].ToString();
                EncryptedOTP = dt.Rows[0]["EncryptedOTP"].ToString();
            }
            string strStateCode = strGSTINNo.Substring(0, 2); // State Code

            //string AuthorizeToken = GetAccessToken.getAuthToken();

            GSP_GSTAuthOTPParameters objGSTAuthOTP = new GSP_GSTAuthOTPParameters();
            objGSTAuthOTP.app_key = EncryptedAppKey;
            objGSTAuthOTP.username = Username;
            objGSTAuthOTP.otp = EncryptedOTP;
            objGSTAuthOTP.action = "AUTHTOKEN";

            string jsondata = new JavaScriptSerializer().Serialize(objGSTAuthOTP);

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
                    //File.WriteAllText(@"E:\AuthToken.json", GSTAuthResponse);
                }
            }

            var data = JsonConvert.DeserializeObject<GSPResponse>(GSTAuthResponse);
            string status_response = data.status_cd.ToString();
            if (status_response == "1")
            {
                string authToken_response = data.auth_token.ToString();
                string sek_response = data.sek.ToString();
                string expiry_response = data.expiry.ToString();

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@EncryptedSEK", SqlDbType.NVarChar).Value = sek_response.Trim();
                cmd.Parameters.Add("@AuthToken", SqlDbType.NVarChar).Value = authToken_response.Trim();
                cmd.Parameters.Add("@Expiry", SqlDbType.Int).Value = expiry_response.Trim();
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                Common.Functions.UpdateTable("TBL_AUTH_KEYS", "AuthorizationToken", strGSTINNo, cmd, con);
                con.Close();

                Helper.InsertAPICountTransactions(strGSTINNo, "", "AUTH TOKEN", SessionUserId, SessionCustId);
            }
            else
            {
                var root = JsonConvert.DeserializeObject<GSPResponse>(GSTAuthResponse);
                status_response = root.error.message + " - " + "Error Code : " + root.error.error_cd;
                Helper.InsertAPICountTransactions(strGSTINNo, "", "AUTH TOKEN", SessionUserId, SessionCustId);                
            }
            return status_response;
        }
    }

    public class GSP_GSTAuthOTPParameters
    {
        public string action { get; set; }
        public string username { get; set; }
        public string app_key { get; set; }
        public string otp { get; set; }
    }
}