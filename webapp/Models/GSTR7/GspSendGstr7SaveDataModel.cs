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
using System.Web.Http;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.Security.Cryptography;
using SmartAdminMvc.Models.GSTAUTH;
using SmartAdminMvc.Models.Common;
using System.Threading.Tasks;
using WeP_DAL;
using WeP_GSTN;

namespace SmartAdminMvc.Models.GSTR7
{
    public class GspSendGstr7SaveDataModel
    {
        public string SendRequest(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string GSTR7Response = "";
            GSPResponse GST_Data = null;
            try
            {
                Helper.GSTR_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                
                string strStateCode = GSTINNo.Substring(0, 2); // State Code
                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = Helper.Encrypt(json, authEK);
                string strhmac = Helper.hmac(authEK, json);

                GstRequestResponseAttributes objGSTR7SaveParams = new GstRequestResponseAttributes();
                objGSTR7SaveParams.data = payload;
                objGSTR7SaveParams.hmac = strhmac;
                objGSTR7SaveParams.action = "RETSAVE";

                string tempjsondata = JsonConvert.SerializeObject(objGSTR7SaveParams);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR7"]);

                
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PUT";
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
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);
                
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(tempjsondata);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        GSTR7Response = result.ToString();
                    }
                }

                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR7Response);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                    string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                    var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);
                    string refid_response = finaldata["reference_id"].Value<string>();

                    Helper.InsertGSTRSaveResponse(GSTINNo, "GSTR7", "", refid_response, Period, SessionCustId, SessionUserId);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR7 Saved Successfully for this " + GSTINNo + " & " + Period + ". Action - " + strAction + " : Reference Id - " + refid_response, "");
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR7 SAVE", SessionUserId, SessionCustId);
                    #region "UPDATE RETSTATUS & INVOICE FLAG"
                    if (HttpContext.Current.Session["G7TDSRefIds"].ToString() != "")
                    {
                        Gstr7Retstatus.Update_GSTR7_RetStatus(GSTINNo, Period, refid_response, "TDS", HttpContext.Current.Session["G7TDSRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR7("TDS", GSTINNo, Period, HttpContext.Current.Session["G7TDSRefIds"].ToString(), "", "U");
                    }
                    if (HttpContext.Current.Session["G7TDSARefIds"].ToString() != "")
                    {
                        Gstr7Retstatus.Update_GSTR7_RetStatus(GSTINNo, Period, refid_response, "TDSA", HttpContext.Current.Session["G7TDSARefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR7("TDSA", GSTINNo, Period, HttpContext.Current.Session["G7TDSARefIds"].ToString(), "", "U");
                    }
                  
                    #endregion

                    GspGetGSTR7DataModel GSP_getGSTR7 = new GspGetGSTR7DataModel();
                    GSP_getGSTR7.SendRequest(GSTINNo, Period, "RETSTATUS", "", refid_response, "","","", SessionUserId, SessionCustId, SessionUsername);

                    return "GSTR7 Save is in Progress... Reference Id - " + refid_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR7Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, " GSTR7 Save " + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR7 SAVE", SessionUserId, SessionCustId);
                   
                    if (HttpContext.Current.Session["G7TDSRefIds"].ToString() != "")
                    {
                        Update_Invoice_Flag_GSTR7("TDS", GSTINNo, Period, HttpContext.Current.Session["G7TDSRefIds"].ToString(), "", "1");
                    }
                    if (HttpContext.Current.Session["G7TDSARefIds"].ToString() != "")
                    {
                         Update_Invoice_Flag_GSTR7("TDSA", GSTINNo, Period, HttpContext.Current.Session["G7TDSARefIds"].ToString(), "", "1");
                    }
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
     

        public static void Update_Invoice_Flag_GSTR7(string ActionType, string strGSTINNo, string strPeriod, string RefIds, string CurFlag, string NewFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR7", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));
                    dCmd.Parameters.Add(new SqlParameter("@CurFlag", CurFlag));
                    dCmd.Parameters.Add(new SqlParameter("@NewFlag", NewFlag));
                    dCmd.Connection = conn;
                    dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}