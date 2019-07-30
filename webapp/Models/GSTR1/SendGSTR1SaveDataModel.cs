﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.Security.Cryptography;
using SmartAdminMvc.Models.GSTAUTH;
using System.Threading.Tasks;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR1
{
    public class SendGSTR1SaveDataModel
    {
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

                //string AuthorizeToken = GetAccessToken.getAuthToken();

                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                
                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Encrypt(json, authEK);
                string strhmac = hmac(authEK, json);

                GSTR1ViewParameters objGSTR1ViewParams = new GSTR1ViewParameters();
                objGSTR1ViewParams.data = payload;
                objGSTR1ViewParams.hmac = strhmac;
                objGSTR1ViewParams.action = "RETSAVE";

                string tempjsondata = new JavaScriptSerializer().Serialize(objGSTR1ViewParams);
                //File.WriteAllText(@"E:\GSTR1Json.json", tempjsondata);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GST_GSTR1"]);
                //var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR1Save"]);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("Authorization", AuthorizeToken);
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode); // ConfigurationManager.AppSettings["GSP_StateCode"]);
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
                        //File.WriteAllText(@"E:\GSTR1.json", GSTR1Response);
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
                    //string refid_response = finaldata["reference_id"].Value<string>();
                    //string transid_response = finaldata["trans_id"].Value<string>();
                    string refid_response = finaldata["reference_id"].Value<string>();
                    //string transid_response = finaldata["trans_id"].Value<string>();

                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@GSTINNo", SqlDbType.VarChar).Value = GSTINNo;
                    cmd.Parameters.Add("@GSTRName", SqlDbType.VarChar).Value = "GSTR1";
                    cmd.Parameters.Add("@TransId", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@RefId", SqlDbType.VarChar).Value = refid_response;
                    cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = SessionCustId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    Common.Functions.InsertIntoTable("TBL_GSTR_SAVE_Response", cmd, con);

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@FK_Audit_User_ID", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@FK_Audit_Username", SqlDbType.VarChar).Value = SessionUsername;
                    cmd.Parameters.Add("@Audit_DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Audit_Message", SqlDbType.VarChar).Value = "GSTR1 Saved Successfully for the " + Period + " : Reference Id - " + refid_response;
                    cmd.Parameters.Add("@Audit_Exception", SqlDbType.VarChar).Value = "";
                    Common.Functions.InsertIntoTable("TBL_Audit_Log", cmd, con);
                                        
                    con.Close();

                    if(HttpContext.Current.Session["B2BRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2B", HttpContext.Current.Session["B2BRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("B2B", GSTINNo, Period, HttpContext.Current.Session["B2BRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["B2CLRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2CL", HttpContext.Current.Session["B2CLRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("B2CL", GSTINNo, Period, HttpContext.Current.Session["B2CLRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["CDNRRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "CDNR", HttpContext.Current.Session["CDNRRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("CDNR", GSTINNo, Period, HttpContext.Current.Session["CDNRRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["B2CSRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "B2CS", HttpContext.Current.Session["B2CSRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("B2CS", GSTINNo, Period, HttpContext.Current.Session["B2CSRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["EXPRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "EXP", HttpContext.Current.Session["EXPRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("EXP", GSTINNo, Period, HttpContext.Current.Session["EXPRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["HSNRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "HSN", HttpContext.Current.Session["HSNRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("HSN", GSTINNo, Period, HttpContext.Current.Session["HSNRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["NILRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "NIL", HttpContext.Current.Session["NILRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("NIL", GSTINNo, Period, HttpContext.Current.Session["NILRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["TXPRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "TXP", HttpContext.Current.Session["TXPRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("TXP", GSTINNo, Period, HttpContext.Current.Session["TXPRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["ATRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "AT", HttpContext.Current.Session["ATRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("AT", GSTINNo, Period, HttpContext.Current.Session["ATRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["DOCRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "DOC", HttpContext.Current.Session["DOCRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("DOC", GSTINNo, Period, HttpContext.Current.Session["DOCRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["CDNURRefIds"].ToString() != "")
                    {
                        GSTR1_RETSTATUS.Update_GSTR1_RetStatus(GSTINNo, Period, refid_response, "CDNUR", HttpContext.Current.Session["CDNURRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR1("CDNUR", GSTINNo, Period, HttpContext.Current.Session["CDNURRefIds"].ToString(), "", "1");
                    }

                    GetGSTR1DataModel GSP_getGSTR1 = new GetGSTR1DataModel();
                    //GSP_GetGSTR1DataModel GSP_getGSTR1 = new GSP_GetGSTR1DataModel();
                    string DownloadRes = GSP_getGSTR1.SendRequest(GSTINNo, Period, "RETSTATUS", "", "", refid_response, "", SessionUserId, SessionCustId, SessionUsername);

                    return "GSTR1 Save is in Progress... Reference Id - " + refid_response; // + " " + HttpContext.Current.Session["RETSTATUS"].ToString();
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<Common.ErrorMsg>(GSTR1Response);
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@FK_Audit_User_ID", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@FK_Audit_Username", SqlDbType.VarChar).Value = SessionUsername;
                    cmd.Parameters.Add("@Audit_DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Audit_Message", SqlDbType.VarChar).Value = root.error.message;
                    cmd.Parameters.Add("@Audit_Exception", SqlDbType.VarChar).Value = "Error Code - " + root.error.error_cd;
                    Common.Functions.InsertIntoTable("TBL_Audit_Log", cmd, con);
                    con.Close();
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Encrypt(string plainText, byte[] keyBytes)
        {
            byte[] dataToEncrypt = UTF8Encoding.UTF8.GetBytes(plainText);

            AesManaged tdes = new AesManaged();

            tdes.KeySize = 256;
            tdes.BlockSize = 128;
            tdes.Key = keyBytes;// Encoding.ASCII.GetBytes(key);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] cipher = crypt.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            tdes.Clear();
            return Convert.ToBase64String(cipher, 0, cipher.Length);
        }

        public string hmac(byte[] deCipher, string message)
        {
            string EK_result = Convert.ToBase64String(deCipher);
            Console.WriteLine(EK_result);
            var EK = Convert.FromBase64String(EK_result);

            string gen_hmac = "";
            //string message = data;

            using (var HMACSHA256 = new HMACSHA256(EK))
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] hashmessage = HMACSHA256.ComputeHash(data);
                gen_hmac = Convert.ToBase64String(hashmessage);
            }
            return gen_hmac;
        }

        public static void Update_Invoice_Flag_GSTR1(string ActionType, string strGSTINNo, string strPeriod, string RefIds, string CurFlag, string NewFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR1", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));                    
                    dCmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));
                    dCmd.Parameters.Add(new SqlParameter("@CurFlag", CurFlag));
                    dCmd.Parameters.Add(new SqlParameter("@NewFlag", NewFlag));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }
                
    }

    public class GSTR1SaveParameters
    {
        public string action { get; set; }
        public string data { get; set; }
        public string hmac { get; set; }
    }
}