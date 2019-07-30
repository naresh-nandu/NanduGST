using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartAdminMvc.Models.Common;
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
using WeP_DAL;
using WeP_GSTN;

namespace SmartAdminMvc.Models.GSTR4
{
    public class GspSendGstr4SaveDataModel
    {
            public string SendRequest(string JsonData, string strAction, string GSTINNo, string Period, string SessionUserId, string SessionCustId, string SessionUsername)
            {
                string Username = "";
                string AppKey = "";
                string EncryptedSEK = "";
                string AuthToken = "";
                string GSTR4Response = "";
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

                    GstRequestResponseAttributes objGSTR4SaveParams = new GstRequestResponseAttributes();
                    objGSTR4SaveParams.data = payload;
                    objGSTR4SaveParams.hmac = strhmac;
                    objGSTR4SaveParams.action = "RETSAVE";

                    string tempjsondata = JsonConvert.SerializeObject(objGSTR4SaveParams);
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR4"]);
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
                            GSTR4Response = result.ToString();
                        }
                    }

                    GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR4Response);

                    string status_response = GST_Data.status_cd.ToString();
                    if (status_response == "1")
                    {
                        string data_response = GST_Data.data.ToString();
                        string rek_response = GST_Data.rek.ToString();

                        byte[] DecryptapiEK = AESEncryption.decrypt(rek_response, authEK);
                        string DecryptjsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(data_response, DecryptapiEK))));

                        var finaldata = (JObject)JsonConvert.DeserializeObject(DecryptjsonData);
                        string refid_response = finaldata["reference_id"].Value<string>();

                        Helper.InsertGSTRSaveResponse(GSTINNo, "GSTR4", "", refid_response, Period, SessionCustId, SessionUserId);
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR4 Saved Successfully for this " + GSTINNo + " & " + Period + ". Action - " + strAction + " : Reference Id - " + refid_response, "");
                        Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR4 SAVE", SessionUserId, SessionCustId);
                        #region "UPDATE RETSTATUS & INVOICE FLAG"
                        if (HttpContext.Current.Session["G4B2BRefIds"].ToString() != "")
                        {
                            Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "B2B", HttpContext.Current.Session["G4B2BRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                            Update_Invoice_Flag_GSTR4("B2B", GSTINNo, Period, HttpContext.Current.Session["G4B2BRefIds"].ToString(), "", "1");
                        }
                        if (HttpContext.Current.Session["G4B2BARefIds"].ToString() != "")
                        {
                            Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "B2BA", HttpContext.Current.Session["G4B2BARefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                            Update_Invoice_Flag_GSTR4("B2BA", GSTINNo, Period, HttpContext.Current.Session["G4B2BARefIds"].ToString(), "", "1");
                        }
                        if (HttpContext.Current.Session["G4CDNRRefIds"].ToString() != "")
                        {
                            Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "CDNR", HttpContext.Current.Session["G4CDNRRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                            Update_Invoice_Flag_GSTR4("CDNR", GSTINNo, Period, HttpContext.Current.Session["G4CDNRRefIds"].ToString(), "", "1");
                        }
                        if (HttpContext.Current.Session["G4CDNRARefIds"].ToString() != "")
                        {
                            Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "CDNRA", HttpContext.Current.Session["G4CDNRARefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                            Update_Invoice_Flag_GSTR4("CDNRA", GSTINNo, Period, HttpContext.Current.Session["G4CDNRARefIds"].ToString(), "", "1");
                        }
                        if (HttpContext.Current.Session["G4CDNURRefIds"].ToString() != "")
                        {
                            Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "CDNUR", HttpContext.Current.Session["G4CDNURRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                            Update_Invoice_Flag_GSTR4("CDNUR", GSTINNo, Period, HttpContext.Current.Session["G4CDNURRefIds"].ToString(), "", "1");
                        }
                        if (HttpContext.Current.Session["G4CDNURARefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "CDNURA", HttpContext.Current.Session["G4CDNURARefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("CDNURA", GSTINNo, Period, HttpContext.Current.Session["G4RefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4IMPSRefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "IMPS", HttpContext.Current.Session["G4IMPSRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("IMPS", GSTINNo, Period, HttpContext.Current.Session["G4IMPSRefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4IMPSARefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "IMPSA", HttpContext.Current.Session["G4IMPSARefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("IMPSA", GSTINNo, Period, HttpContext.Current.Session["G4IMPSARefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4ATRefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "AT", HttpContext.Current.Session["G4ATRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("AT", GSTINNo, Period, HttpContext.Current.Session["G4ATRefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4ATARefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "ATA", HttpContext.Current.Session["G4ATARefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("ATA", GSTINNo, Period, HttpContext.Current.Session["G4ATARefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4TXPRefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "TXP", HttpContext.Current.Session["G4TXPRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("TXP", GSTINNo, Period, HttpContext.Current.Session["G4TXPRefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4TXPARefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "TXPA", HttpContext.Current.Session["G4TXPARefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("TXPA", GSTINNo, Period, HttpContext.Current.Session["G4TXPARefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4TXOSRefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "TXOS", HttpContext.Current.Session["G4TXOSRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("TXOS", GSTINNo, Period, HttpContext.Current.Session["G4TXOSRefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4TXOSARefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "TXOSA", HttpContext.Current.Session["G4TXOSARefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("TXOSA", GSTINNo, Period, HttpContext.Current.Session["G4TXOSARefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4B2BURRefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "B2BUR", HttpContext.Current.Session["G4B2BURRefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("B2BUR", GSTINNo, Period, HttpContext.Current.Session["G4B2BURRefIds"].ToString(), "", "1");
                    }
                        if (HttpContext.Current.Session["G4B2BURARefIds"].ToString() != "")
                    {
                        Gstr4Retstatus.Update_GSTR4_RetStatus(GSTINNo, Period, refid_response, "B2BURA", HttpContext.Current.Session["G4B2BURARefIds"].ToString(), "1", "", SessionCustId, SessionUserId);
                        Update_Invoice_Flag_GSTR4("B2BURA", GSTINNo, Period, HttpContext.Current.Session["G4B2BURARefIds"].ToString(), "", "1");
                    }
                         #endregion

                    GspGetGstr4DataModel GSP_getGSTR4 = new GspGetGstr4DataModel();
                        GSP_getGSTR4.SendRequest(GSTINNo, Period, "RETSTATUS", "", refid_response, "", SessionUserId, SessionCustId, SessionUsername);

                        return "GSTR4 Save is in Progress... Reference Id - " + refid_response;
                    }
                    else
                    {
                        var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR4Response);
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, " GSTR4 Save " + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + strAction, "Error Code - " + root.error.error_cd);
                        Helper.InsertAPICountTransactions(GSTINNo, Period, "GSTR4 SAVE", SessionUserId, SessionCustId);
                        return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                    }

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }


            public static void Update_Invoice_Flag_GSTR4(string ActionType, string strGSTINNo, string strPeriod, string RefIds, string CurFlag, string NewFlag)
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    try
                    {
                        #region commented

                        conn.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR4", conn);
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
