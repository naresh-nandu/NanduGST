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
using System.Web;
using System.Web.Script.Serialization;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR1
{
    public class SendGSTR1SubmitDataModel
    {
        public WePGSPDBEntities db = new WePGSPDBEntities();

        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "", DecryptedJsonData1 = "";
        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        private static string GSTR1Response = "";

        public string SendRequest(string JsonData, string GSTINNo, string Period, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            try
            {
                SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_AUTH_KEYS where AuthorizationToken = '" + GSTINNo + "'", con);
                DataTable dt = new DataTable();
                adt.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Username = dt.Rows[0]["Username"].ToString();
                    AppKey = dt.Rows[0]["AppKey"].ToString();
                    EncryptedSEK = dt.Rows[0]["EncryptedSEK"].ToString();
                    AuthToken = dt.Rows[0]["AuthToken"].ToString();
                }

                string strStateCode = GSTINNo.Substring(0, 2); // State Code

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);
                                
                GSTR1SubmitParameters objGSTR1SubmitParams = new GSTR1SubmitParameters();
                objGSTR1SubmitParams.data = payload;
                objGSTR1SubmitParams.hmac = strhmac;
                objGSTR1SubmitParams.action = "RETSUBMIT";

                string tempjsondata = new JavaScriptSerializer().Serialize(objGSTR1SubmitParams);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GST_GSTR1"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", Period);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
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
                        GSTR1Response = result.ToString();
                        File.WriteAllText(@"E:\GSTR1Filing_Response.json", GSTR1Response);
                    }
                }

                var data = (JObject)JsonConvert.DeserializeObject(GSTR1Response);
                string status_response = data["status_cd"].Value<string>();
                if (status_response == "1")
                {
                    string data_response = data["data"].Value<string>();
                    string hmac_response = data["hmac"].Value<string>();
                    string rek_response = data["rek"].Value<string>();


                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    //System.out.println("Encoded Api EK (Received):" + AESEncryption.encodeBase64String(apiEK));
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                    //File.WriteAllText(@"E:\GSTR1Response.json", DecryptjsonData);
                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);
                    string refid_response = finaldata["reference_id"].Value<string>();
                    //string transid_response = finaldata["trans_id"].Value<string>();
                    //string laststatus_response = finaldata["status"].Value<string>();
                    //string Acknum_response = finaldata["Ack_num"].Value<string>();

                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    //cmd.Parameters.Clear();
                    //cmd.Parameters.Add("@GSTINNo", SqlDbType.VarChar).Value = GSTINNo;
                    //cmd.Parameters.Add("@GSTRName", SqlDbType.VarChar).Value = "GSTR1";
                    //cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = "";
                    //cmd.Parameters.Add("@AckNo", SqlDbType.VarChar).Value = "";
                    //cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = SessionCustId;
                    //cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = SessionUserId;
                    //cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;

                    //Common.Functions.InsertIntoTable("TBL_GSTR_FILE_Response", cmd, con);

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@FK_Audit_User_ID", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@FK_Audit_Username", SqlDbType.VarChar).Value = SessionUsername;
                    cmd.Parameters.Add("@Audit_DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Audit_Message", SqlDbType.VarChar).Value = "GSTR1 Submitted Successfully for the " + Period +" : Reference Id - " + refid_response;
                    cmd.Parameters.Add("@Audit_Exception", SqlDbType.VarChar).Value = "";
                    Common.Functions.InsertIntoTable("TBL_Audit_Log", cmd, con);

                    con.Close();
                    //int UserId = Convert.ToInt32(SessionUserId);
                    //string strMobileNo = (from lst in db.UserLists
                    //                   where lst.UserId == UserId
                    //                   select lst.MobileNo).SingleOrDefault();
                    //Common.Notification.SendSMS(strMobileNo, string.Format("Congratulations!! Your GSTR - 1 is Submitted Successfully!!!"));
                    GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "GSTR1 SUBMIT", "", "1", "", SessionCustId, SessionUserId);
                    return "GSTR1 Submitted Successfully... Reference Id - " + refid_response;

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<Common.ErrorMsg>(GSTR1Response);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    public class GSTR1SubmitParameters
    {
        public string action { get; set; }
        public string data { get; set; }
        public string hmac { get; set; }
    }
}