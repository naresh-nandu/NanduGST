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
using System.Security.Cryptography;
using SmartAdminMvc.Models.Common;

namespace SmartAdminMvc.Models.GSTAUTH
{
    public partial class GSP_GSTRRefreshToken
    {
        private static string GSTRefreshTokenResponse = "";
        
        private string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        private static string EncryptedAppKey = "";
        private static string EncryptedSek = "", Username = "", AppKey = "", NewAppKey = "", AuthToken = "";

        public static string SendRequest(string strGSTINNo, string SessionUserId, string SessionCustId)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            con.Open();
            SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_AUTH_KEYS where AuthorizationToken = '" + strGSTINNo + "'", con);
            DataTable dt = new DataTable();
            adt.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                Username = dt.Rows[0]["Username"].ToString().Trim().ToLower();
                AppKey = dt.Rows[0]["AppKey"].ToString();
                AuthToken = dt.Rows[0]["AuthToken"].ToString();
                EncryptedSek = dt.Rows[0]["EncryptedSEK"].ToString();
            }
            string strStateCode = strGSTINNo.Substring(0, 2); // State Code

            //string AuthorizeToken = GetAccessToken.getAuthToken();

            NewAppKey = AESEncryption.generateSecureKey();
            byte[] authEK = AESEncryption.decrypt(EncryptedSek, AESEncryption.decodeBase64StringTOByte(AppKey));
            EncryptedAppKey = Encrypt(AESEncryption.decodeBase64StringTOByte(NewAppKey), authEK);

            GSP_RefreshAuthTokenParameters objRefreshToken = new GSP_RefreshAuthTokenParameters();
            objRefreshToken.app_key = EncryptedAppKey;
            objRefreshToken.username = Username;
            objRefreshToken.auth_token = AuthToken;
            objRefreshToken.action = "REFRESHTOKEN";

            string jsondata = new JavaScriptSerializer().Serialize(objRefreshToken);

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
                    GSTRefreshTokenResponse = result.ToString();
                }
            }

            var data = JsonConvert.DeserializeObject<GSPResponse>(GSTRefreshTokenResponse);
            string status_response = data.status_cd.ToString();
            if (status_response == "1")
            {
                string authToken_response = data.auth_token.ToString();
                string sek_response = data.sek.ToString();
                string expiry_response = data.expiry.ToString();

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@AppKey", SqlDbType.NVarChar).Value = NewAppKey.Trim();
                cmd.Parameters.Add("@EncryptedAppKey", SqlDbType.NVarChar).Value = EncryptedAppKey.Trim();
                cmd.Parameters.Add("@EncryptedSEK", SqlDbType.NVarChar).Value = sek_response.Trim();
                cmd.Parameters.Add("@AuthToken", SqlDbType.NVarChar).Value = authToken_response.Trim();
                cmd.Parameters.Add("@Expiry", SqlDbType.Int).Value = expiry_response.Trim();
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                Common.Functions.UpdateTable("TBL_AUTH_KEYS", "AuthorizationToken", strGSTINNo, cmd, con);
                con.Close();
            }
            else
            {
                var root = JsonConvert.DeserializeObject<GSPResponse>(GSTRefreshTokenResponse);
                //ShowPopUpMsg(root.error.message + " - " + "Error Code : " + root.error.error_cd);
                status_response = root.error.message + " - " + "Error Code : " + root.error.error_cd;
                //status_response = "Invalid User";
            }
            return status_response;
        }

        public static string Encrypt(byte[] plainText, byte[] keyBytes)
        {
            AesManaged tdes = new AesManaged();

            tdes.KeySize = 256;
            tdes.BlockSize = 128;
            tdes.Key = keyBytes;// Encoding.ASCII.GetBytes(key);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] cipher = crypt.TransformFinalBlock(plainText, 0, plainText.Length);
            tdes.Clear();
            return Convert.ToBase64String(cipher, 0, cipher.Length);
        }
    }

    public class GSP_RefreshAuthTokenParameters
    {
        public string action { get; set; }
        public string username { get; set; }
        public string app_key { get; set; }
        public string auth_token { get; set; }
    }
}