using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Runtime.InteropServices;
using System.Reflection;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR3
{
    public class GetGstr3DataModel
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        //string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        private static string GSTR3_Download_Response = "";
        private static string GST_Path = "";
        static string gstin = "";
        static string fromdate = "";
        static string retperiod = "";
        static string action = "";
        static string finalparam = "";
        static string transid = "";
        static string URL = "";
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "";

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        public static string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strTransId)
        {
            GST_Path = ConfigurationManager.AppSettings["GST_GSTR3"];
            try
            {
                SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_AUTH_KEYS", con);
                System.Data.DataTable dt = new System.Data.DataTable();
                adt.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Username = dt.Rows[0]["Username"].ToString();
                    AppKey = dt.Rows[0]["AppKey"].ToString();
                    EncryptedSEK = dt.Rows[0]["EncryptedSEK"].ToString();
                    AuthToken = dt.Rows[0]["AuthToken"].ToString();
                }

                gstin = strGSTINNo;
                retperiod = strPeriod;
                transid = strTransId;
                action = strAction;
                //if (drpActionRequired.SelectedItem.ToString() == "Yes" || drpActionRequired.SelectedItem.ToString() == "Select")
                //{
                //    actionrequired = "Y";
                //}
                //else if (drpActionRequired.SelectedItem.ToString() == "No")
                //{
                //    actionrequired = "N";
                //}
                finalparam = "Y";

                //if (action == "RETDET")
                //{
                //    URL = GST_Path + "?ret_period=" + retperiod + "&gstin=" + gstin + "&finalparam=" + finalparam + "&action=" + action;
                //}
                //if (action == "RETSTATUS")
                //{
                //    URL = GST_Path + "?trans_id=" + transid + "&gstin=" + gstin + "&action=" + action;
                //}
                if (action == "RETDET")
                {
                    URL = GST_Path + "?ret_period=" + retperiod + "&gstin=" + gstin + "&action=" + action;
                }
                if (action == "RETSTATUS")
                {
                    URL = GST_Path + "?trans_id=" + transid + "&gstin=" + gstin + "&action=" + action;
                }

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", ConfigurationManager.AppSettings["GSP_StateCode"]);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR3_Download_Response = result.ToString();
                    //File.WriteAllText(@"E:\GetGSTR2_B2B.json", GSTR2_B2B_Response);
                }

                var data = (JObject)JsonConvert.DeserializeObject(GSTR3_Download_Response);
                string status_response = data["status_cd"].Value<string>();
                if (status_response == "1")
                {
                    string data_response = data["data"].Value<string>();
                    string hmac_response = data["hmac"].Value<string>();
                    string rek_response = data["rek"].Value<string>();

                    string decrypted_appkey = AppKey;
                    string receivedSEK = EncryptedSEK;
                    string gotREK = rek_response;
                    string gotdata = data_response;

                    byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                    //System.out.println("Encoded Auth EK (Received):" + AESEncryption.encodeBase64String(authEK));

                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    //System.out.println("Encoded Api EK (Received):" + AESEncryption.encodeBase64String(apiEK));
                    DecryptedJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));

                    return DecryptedJsonData;// "GSTR3 Downloaded and Saved Successfully...";

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<Common.ErrorMsg>(GSTR3_Download_Response);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}