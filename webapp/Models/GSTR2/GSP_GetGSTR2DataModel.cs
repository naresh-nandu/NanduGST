﻿using System;
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
using SmartAdminMvc.Models.GSTAUTH;
using SmartAdminMvc.Models.Common;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR2
{
    public partial class GSP_GetGSTR2DataModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        //string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        private static string GSTR2_Download_Response = "";
        private static string GST_Path = "", GST_TrackPath = "";
        static string gstin = "";
        static string ctin = "";
        static string retperiod = "";
        static string action = "";
        static string token = "";
        static string refid = "";
        static string statecd = "";
        static string URL = "";
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "";

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strCTIN, string strRefId, string strToken, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR2"];
            GST_TrackPath = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];

            try
            {
                SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_AUTH_KEYS where AuthorizationToken = '" + strGSTINNo + "'", con);
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

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                ctin = strCTIN;
                //statecd = strStatecd;
                refid = strRefId;
                action = strAction;
                token = strToken;

                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "B2BUR")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "CDN")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "CDNUR")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "IMPG")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "IMPS")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "NIL")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "TXP")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "TXLI")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "HSNSUM")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "ITCRVSL")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "RETSUM")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "RETSTATUS")
                {
                    URL = GST_TrackPath + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&ref_id=" + refid;
                }
                else if (action == "FILEDET")
                {
                    URL = GST_TrackPath + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&token=" + token;
                }

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("Authorization", AuthorizeToken);
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode); // ConfigurationManager.AppSettings["GSP_StateCode"]);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);
                httpWebRequest.Headers.Add("GSTINNO", strGSTINNo);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR2_Download_Response = result.ToString();
                }

                //var data = JsonConvert.DeserializeObject<GSPTokenResponse>(GSTR1_Download_Response);
                //string reqtoken = data.ToString();

                //System.Threading.Thread.Sleep(1000);

                //string gspResponse = GSP_GetTokenResponseModel.GetGSPTokenResponse(reqtoken, strGSTINNo);

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR2_Download_Response);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string hmac_response = GST_Data.hmac.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    string decrypted_appkey = AppKey;
                    string receivedSEK = EncryptedSEK;
                    string gotREK = rek_response;
                    string gotdata = data_response;

                    byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                    //System.out.println("Encoded Auth EK (Received):" + AESEncryption.encodeBase64String(authEK));

                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    //System.out.println("Encoded Api EK (Received):" + AESEncryption.encodeBase64String(apiEK));
                    DecryptedJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));
                    //string dataSaveResponse = GSTR2DownloadDataModel.SaveJsonDatatoDB(DecryptedJsonData, strGSTINNo, strPeriod);
                    //return DecryptedJsonData;
                    if (action == "RETSTATUS")
                    {
                        var downloaddata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                        //var downloaddata = JsonConvert.DeserializeObject<DownloadStatusResponse>(DecryptedJsonData);

                        string st_res = downloaddata["status_cd"].Value<string>();

                        HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                        if (st_res == "P")
                        {
                            //InsertGSTR1_Json_Flag(strGSTINNo, strPeriod, "U");
                            Updating_GSTR2_RetStatus(st_res, "", refid);
                            Updating_Invoice_Flag_GSTR2(refid, "U");
                        }
                        else if (st_res == "IP")
                        {
                            Updating_GSTR2_RetStatus(st_res, "", refid);
                            Updating_Invoice_Flag_GSTR2(refid, "1");
                        }
                        else if (st_res == "PE")
                        {
                            //string error_res = downloaddata["error_report"].Value<string>();
                            Updating_GSTR2_RetStatus(st_res, downloaddata.ToString(), refid);
                            Updating_Invoice_Flag_GSTR2(refid, "1");
                            Process_RetStatus_Error_Invoices(strGSTINNo, strPeriod);
                        }
                        else
                        {
                            //string error_res = downloaddata["error_report"].Value<string>();
                            Updating_GSTR2_RetStatus(st_res, downloaddata.ToString(), refid);
                            Updating_Invoice_Flag_GSTR2(refid, "1");
                            Process_RetStatus_Error_Invoices(strGSTINNo, strPeriod);
                        }
                    }
                    else if (action == "FILEDET")
                    {
                        string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\TAR\\GSTR2\\"));
                        folderPath = folderPath.Replace("\\", "/");

                        string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Unzip\\GSTR2\\"));
                        folderPath1 = folderPath1.Replace("\\", "/");

                        HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                        string Json = DecryptedJsonData;
                        DirectoryInfo d1 = new DirectoryInfo(folderPath);
                        var GST_URLData = JsonConvert.DeserializeObject<URLResponse>(Json);
                        int fc_res = GST_URLData.fc;
                        string ek_res = GST_URLData.ek;
                        for (int i = 0; i < fc_res; i++)
                        {
                            string urls_res = GST_URLData.urls[i].ul.ToString();
                            string[] uls = urls_res.Split('/');
                            string[] uls1 = uls[5].Split('?');

                            WebClient webClient = new WebClient();
                            string url = ConfigurationManager.AppSettings["GSP_LARGEFILEDOWNLOAD"] + urls_res;
                            webClient.DownloadFile(url, folderPath + uls1[0]);
                        }

                        DirectoryInfo d = new DirectoryInfo(folderPath);//Assuming Test is your Folder
                        FileInfo[] Files = d.GetFiles("*.tar.gz"); //Getting Text files

                        DirectoryInfo targetDirectory = new DirectoryInfo(folderPath1);// @"c:\tar\finish");
                        if (!targetDirectory.Exists)
                        {
                            targetDirectory.Create();
                        }
                        foreach (FileInfo tarFileInfo in Files)
                        {
                            using (Stream sourceStream = new GZipInputStream(tarFileInfo.OpenRead()))
                            {
                                using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(sourceStream, TarBuffer.DefaultBlockFactor))
                                {
                                    tarArchive.ExtractContents(targetDirectory.FullName);
                                }
                            }
                        }

                        foreach (string jsonfile in Directory.EnumerateFiles(folderPath1, "*.json", SearchOption.AllDirectories))
                        {
                            //string contents = File.ReadAllText(file);
                            string contents = File.ReadAllText(jsonfile);

                            //Response.Write(contents + "<br /><br />");
                            //byte[] Ek = AESEncryption.decodeBase64StringTOByte(AppKey);
                            byte[] EK = AESEncryption.decodeBase64StringTOByte(ek_res.Trim());

                            string strJsonData = Encoding.UTF8.GetString(AESEncryption.decodeBase64StringTOByte(Encoding.UTF8.GetString(AESEncryption.decrypt(contents.Trim(), EK))));

                            InsertGSTR2_D_Json(strGSTINNo, strPeriod, strJsonData);

                        }
                    }
                    else if (action == "RETSUM")
                    {
                        InsertGSTR2_RETSUM_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                    }
                    else
                    {
                        InsertGSTR2_D_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                    }
                    
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2 DOWNLOAD", SessionUserId, SessionCustId);
                    return "GSTR-2 " + action + " Downloaded Successfully";
                }
                else if (status_response == "2")
                {
                    string data_response = GST_Data.data.ToString();
                    string hmac_response = GST_Data.hmac.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    string decrypted_appkey = AppKey;
                    string receivedSEK = EncryptedSEK;
                    string gotREK = rek_response;
                    string gotdata = data_response;

                    byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                    //System.out.println("Encoded Auth EK (Received):" + AESEncryption.encodeBase64String(authEK));

                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    //System.out.println("Encoded Api EK (Received):" + AESEncryption.encodeBase64String(apiEK));
                    string filetokenresponse = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));

                    var filedata = (JObject)JsonConvert.DeserializeObject(filetokenresponse);
                    string token_response = filedata["token"].Value<string>();
                                        
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2 DOWNLOAD", SessionUserId, SessionCustId);

                    return "GSTR-2 " + action + " Downloaded Successfully with Token for Large File - " + token_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR2_Download_Response);
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@FK_Audit_User_ID", SqlDbType.Int).Value = SessionUserId;
                    cmd.Parameters.Add("@FK_Audit_Username", SqlDbType.VarChar).Value = SessionUsername;
                    cmd.Parameters.Add("@Audit_DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Audit_Message", SqlDbType.VarChar).Value = "GSTR-2 " + action + " " + root.error.message + " for " + strPeriod;
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

        public static void InsertGSTR2_D_Json(string strGSTINNo, string strPeriod, string strJsonData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR2_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RecordContents", strJsonData));
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

        public static void InsertGSTR2_RETSUM_Json(string strGSTINNo, string strPeriod, string strJsonData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR2_RETSUM", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RecordContents", strJsonData));
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

        public static void Updating_GSTR2_RetStatus(string strStatus, string strErrorMsg, string strRefNo)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = strStatus;
                    cmd.Parameters.Add("@errorreport", SqlDbType.NVarChar).Value = strErrorMsg;
                    Common.Functions.UpdateTable("TBL_GSTR2_SAVE_RETSTATUS", "referenceno", strRefNo, cmd, conn);
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }

        public string Updating_Invoice_Flag_GSTR2(string strRefNo, string ErrorFlag)
        {
            string Status_Response = "";
            con.Open();
            SqlDataAdapter adt = new SqlDataAdapter("Select gstin, fp, actiontype, refids from TBL_GSTR2_SAVE_RETSTATUS where status != '1' and referenceno = '" + strRefNo + "'", con);
            DataTable dt = new DataTable();
            adt.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string gstinno = dt.Rows[i]["gstin"].ToString();
                    string period = dt.Rows[i]["fp"].ToString();
                    string Action = dt.Rows[i]["actiontype"].ToString();
                    string RefIds = dt.Rows[i]["refids"].ToString();

                    Update_Invoice_Flag_GSTR2(Action, gstinno, period, RefIds, "1", ErrorFlag);
                }
            }
            con.Close();
            return Status_Response;
        }

        public static void Update_Invoice_Flag_GSTR2(string ActionType, string strGSTINNo, string strPeriod, string RefIds, string CurFlag, string NewFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR2", conn);
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

        public static void Process_RetStatus_Error_Invoices(string strGSTINNo, string strPeriod)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Process_GSTR2_RETSTATUS", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
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
}