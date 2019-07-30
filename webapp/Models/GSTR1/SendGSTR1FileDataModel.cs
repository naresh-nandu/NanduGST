using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartAdminMvc.Models.ESign;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR1
{
    public class SendGSTR1FileDataModel
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

        public string SendRequest(string JsonData, string strSignedData, string GSTINNo, string Period, string SessionUserId, string SessionCustId, string SessionUsername)
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
                //string encryptedpayload = AESEncryption.encryptEK(Convert.FromBase64String(Helper.Base64Encode(DecryptedJsonData)), authEK);
                //string encryptedhmac = Helper.HMAC_SHA256(encryptedpayload, AESEncryption.encodeBase64String(authEK));
                //byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                //string json = Convert.ToBase64String(encodejson);


                //string payload = Helper.Base64Encode(Encrypt(JsonData.Trim(), authEK));   // Encrypt(json, authEK);
                //string payload = Helper.Base64Encode(JsonData.Trim());   // Encrypt(json, authEK);

                // ---- First Process
                //string Base64Payload = Helper.Base64Encode(JsonData.Trim());
                //string sha256ForSignData = string.Empty;
                //using (var sha256Obj = SHA256.Create())
                //{
                //    byte[] outputBytes = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(Base64Payload));
                //    sha256ForSignData = BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
                //}
                //byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                //string json = Convert.ToBase64String(encodejson);
                //string payload = Helper.Encrypt(json, authEK);

                ////string payload = sha256ForSignData;
                //strSignedData = DSCDataModel.SignData(sha256ForSignData);


                // ---- Second Process
                //string payload = Helper.Encrypt(JsonData.Trim(), authEK);
                //string Base64Payload = Helper.Base64Encode(payload);
                //string sha256ForSignData = string.Empty;
                //using (var sha256Obj = SHA256.Create())
                //{
                //    byte[] outputBytes = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(Base64Payload));
                //    sha256ForSignData = BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
                //}
                ////byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                ////string json = Convert.ToBase64String(encodejson);
                ////string payload = Helper.Encrypt(json, authEK);

                ////string payload = sha256ForSignData;
                //strSignedData = DSCDataModel.SignData(sha256ForSignData);


                string json = RemoveWhitespace(JsonData);
                File.WriteAllText(@"E:\JsonSummaryData.txt", JsonData);
                File.WriteAllText(@"E:\TrimmedJsonData.txt", json);
                File.WriteAllText(@"E:\Base64Encode_TrimmedJsonData.txt", Helper.Base64Encode(json));
                string encdata = Helper.Encrypt(Helper.Base64Encode(json), authEK);
                strSignedData = SignData(json);

                GSTR1FileParameters objGSTR1FileParams = new GSTR1FileParameters();
                objGSTR1FileParams.action = "RETFILE";
                objGSTR1FileParams.data = encdata; // strdata;
                objGSTR1FileParams.sign = strSignedData;
                objGSTR1FileParams.st = "DSC";
                objGSTR1FileParams.sid = "AEGPG1699M";

                string tempjsondata = new JavaScriptSerializer().Serialize(objGSTR1FileParams);

                //string encdata1 = Helper.Encrypt(tempjsondata, authEK);
                //GSTR1FileParameters objGSTR1FileParams1 = new GSTR1FileParameters();
                //objGSTR1FileParams1.action = "RETFILE";
                //objGSTR1FileParams1.data = encdata1; // strdata;
                //string tempjsondata1 = new JavaScriptSerializer().Serialize(objGSTR1FileParams1);
                //File.WriteAllText(@"E:\GSTR1_Filing_SignedData_" + GSTINNo + ".json", strSignedData);

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
                File.WriteAllText(@"E:\GSTR1_Filing_Request_" + GSTINNo + ".txt", tempjsondata);
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
                        File.WriteAllText(@"E:\GSTR1_Filing_Response_" + GSTINNo + ".txt", GSTR1Response);
                    }
                }

                #region "Decrypting Response"
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
                    //string refid_response = finaldata["reference_id"].Value<string>();
                    //string transid_response = finaldata["trans_id"].Value<string>();
                    string laststatus_response = finaldata["status"].Value<string>();
                    string Acknum_response = finaldata["Ack_num"].Value<string>();

                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@GSTINNo", SqlDbType.VarChar).Value = GSTINNo;
                    cmd.Parameters.Add("@GSTRName", SqlDbType.VarChar).Value = "GSTR1";
                    cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@AckNo", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = SessionCustId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;

                    Common.Functions.InsertIntoTable("TBL_GSTR_FILE_Response", cmd, con);

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@FK_Audit_User_ID", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@FK_Audit_Username", SqlDbType.VarChar).Value = SessionUsername;
                    cmd.Parameters.Add("@Audit_DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Audit_Message", SqlDbType.VarChar).Value = "GSTR1 Filed";
                    cmd.Parameters.Add("@Audit_Exception", SqlDbType.VarChar).Value = "";
                    Common.Functions.InsertIntoTable("TBL_Audit_Log", cmd, con);

                    con.Close();

                    //int UserId = Convert.ToInt32(SessionUserId);
                    //string strMobileNo = (from lst in db.UserLists
                    //                   where lst.UserId == UserId
                    //                   select lst.MobileNo).SingleOrDefault();
                    //Common.Notification.SendSMS(strMobileNo, string.Format("Congratulations!! Your GSTR - 1 is Filed Successfully!!!"));
                    return "GSTR1 Filed Successfully... Status - " + laststatus_response + " and Ack No. - " + Acknum_response;

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<Common.ErrorMsg>(GSTR1Response);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
                #endregion
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string RemoveWhitespace(string str)
        {
            string[] strss = null;
            return string.Join("", str.Split(strss, StringSplitOptions.RemoveEmptyEntries));
        }

        private string SignData(string dataToSign)
        {
            string Base64Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(dataToSign));

            string sha256ForSignData = string.Empty;
            using (var sha256Obj = SHA256.Create())
            {
                byte[] outputBytes = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(Base64Payload));   //before this dataToSign was originag json
                sha256ForSignData = BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
            }
            File.WriteAllText(@"E:\Hashed_Data.txt", sha256ForSignData);

            byte[] data = Encoding.UTF8.GetBytes(sha256ForSignData);
            //load certificate 
            X509Certificate2 certObject = null;

            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            my.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in my.Certificates)
            {
                if (cert.Subject.Contains("CN=JAGRATI MOTWANI"))//change the subject name
                {
                    certObject = cert;
                    break;
                }
            }
            //
            string sign = "";
            if (certObject != null)
            {
                ContentInfo content = new ContentInfo(data);
                SignedCms signedCms = new SignedCms(content);
                CmsSigner signer = new CmsSigner(certObject);
                signer.DigestAlgorithm = new Oid("SHA256");
                signer.IncludeOption = X509IncludeOption.EndCertOnly;
                //compute signature and encode and convert to base64 string 
                signedCms.ComputeSignature(signer, false);
                //sign = Convert.ToBase64String(signedCms.Encode());
                sign = Convert.ToBase64String(signedCms.Encode());


            }

            else
            {
                sign = "no creater found";
            }
            //MessageBox.Show(sign);

            //mydata.AppendText(sign);

            return sign;



        }


    }

    public class GSTR1FileParameters
    {
        public string action { get; set; }
        public string data { get; set; }
        public string sign { get; set; }
        public string st { get; set; }
        public string sid { get; set; }
    }
}