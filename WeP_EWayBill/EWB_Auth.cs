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
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Web.Script.Serialization;
using WeP_DAL;

namespace WeP_EWayBill
{
    public class EWB_Auth
    {
        private static string EWBAuthResponse = "";
        private static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        private static string EncryptedAppKey = "", Username = "", EncryptedPassword = "", ErrorDescription = "";

        public static string SendRequest(string strGSTINNo)
        {
            try
            {
                Helper.EWB_AUTH_DataAdapter(strGSTINNo, out Username, out EncryptedAppKey, out EncryptedPassword);

                EWB_Request_Attributes objEWBAuth = new EWB_Request_Attributes();
                objEWBAuth.action = "ACCESSTOKEN";
                objEWBAuth.username = Username;
                objEWBAuth.password = EncryptedPassword;
                objEWBAuth.app_key = EncryptedAppKey;

                string jsondata = JsonConvert.SerializeObject(objEWBAuth);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_AUTHENTICATE"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("ewb-user-id", ConfigurationManager.AppSettings["EWB_USER_ID"]);
                httpWebRequest.Headers.Add("Gstin", strGSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

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
                        EWBAuthResponse = result.ToString();
                    }
                }
                var data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWBAuthResponse);
                string status_response = data.status.ToString();
                if (status_response == "1")
                {
                    string authToken_response = data.authtoken.ToString();
                    string sek_response = data.sek.ToString();
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@EncryptedSEK", SqlDbType.NVarChar).Value = sek_response.Trim();
                    cmd.Parameters.Add("@AuthToken", SqlDbType.NVarChar).Value = authToken_response.Trim();
                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    SQLHelper.UpdateTable("TBL_AUTH_KEYS_EWB", "GSTIN", strGSTINNo, cmd, con);
                    con.Close();
                    status_response = "Authenticated Successfully";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWBAuthResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    Helper.EWB_Error_DataAdapter(root_error.errorcodes, out ErrorDescription);
                    status_response = "Error Code : " + root_error.errorcodes + " & Error Description : " + ErrorDescription;
                }
                return status_response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}