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
using SmartAdminMvc.Models.GSTAUTH;
using SmartAdminMvc.Models.Common;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR1
{
    public partial class GSP_GetGSTR1DataModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        //string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        private static string GSTR1_Download_Response = "";
        private static string GST_Path = "", GST_TrackPath = "";
        static string gstin = "";
        static string ctin = "";
        static string retperiod = "";
        static string action = "";
        static string token = "";
        static string refid = "";
        static string statecd = "";
        static string URL = "";
        private static string DecryptedJsonData = "";
        public static string flag = "";
        public static string token_flag = "";
        static string flag_error = "";
        public static string month = "";
        public  static string month1 = "";

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strCTIN, string strStatecd, string strRefId, string strToken, string from,string to, string SessionUserId, string SessionCustId, string SessionUsername, string flagg)
        {
            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR1"];
            GST_TrackPath = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];

            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                
                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                ctin = strCTIN;
                statecd = strStatecd;
                refid = strRefId;
                action = strAction;
                token = strToken;

                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "B2CL")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&state_cd=" + statecd;
                }
                else if (action == "B2CS")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&state_cd=" + statecd;
                }
                else if (action == "CDNR")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "CDNUR")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "EXP")
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
                else if (action == "AT")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "HSNSUM")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "DOCISS")
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
                httpWebRequest.Headers.Add("username", Username);
                httpWebRequest.Headers.Add("auth-token", AuthToken);
                httpWebRequest.Headers.Add("clientid", ConfigurationManager.AppSettings["GSP_ClientId"]);
                httpWebRequest.Headers.Add("state-cd", strStateCode);
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
                    GSTR1_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR1_Download_Response);

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
                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    DecryptedJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));

                    var filedata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                    string[] stringUrls = DecryptedJsonData.Split('\"');
                    if (stringUrls[1] != "urls" && stringUrls[1] != "form_typ" && stringUrls[1] != "b2b" && stringUrls[1] != "cdnr"
                        && stringUrls[1] != "b2cl" && stringUrls[1] != "b2cs" && stringUrls[1] != "nil" && stringUrls[1] != "exp"
                        && stringUrls[1] != "at" && stringUrls[1] != "txpd" && stringUrls[1] != "hsn" && stringUrls[1] != "cdnur"
                        && stringUrls[1] != "doc_issue" && stringUrls[1] != "ret_period" && stringUrls[1] != "gstin")
                    {
                        string token_response = filedata["token"].Value<string>();
                        flag = "S";
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + from + " to " + to + ", Token for Large File - " + token_response, "");
                        Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLOAD", SessionUserId, SessionCustId);
                        return "GSTR-1 " + action + " Downloaded Successfully with Token for Large File - " + token_response;
                    }
                    else
                    {
                        string st_res = "";
                        if (action == "RETSTATUS")
                        {
                            var downloaddata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                            st_res = downloaddata["status_cd"].Value<string>();
                            if (st_res == "P")
                            {
                                Updating_GSTR1_RetStatus(st_res, downloaddata.ToString(), refid);
                                Updating_Invoice_Flag_GSTR1(refid, "U");
                            }
                            else if (st_res == "IP")
                            {
                                Updating_GSTR1_RetStatus(st_res, downloaddata.ToString(), refid);
                                Updating_Invoice_Flag_GSTR1(refid, "1");
                            }
                            else if (st_res == "PE" || st_res == "ER")
                            {
                                Updating_GSTR1_RetStatus(st_res, downloaddata.ToString(), refid);
                                GSTR1_RETSTATUS.Process_RetStatus_Error_Invoices(strGSTINNo, strPeriod, refid);
                            }
                            else
                            {
                                // ...
                            }
                        }
                        else if (action == "FILEDET")
                        {
                            string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\TAR\\GSTR1\\"));
                            folderPath = folderPath.Replace("\\", "/");

                            string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Unzip\\GSTR1\\"));
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
                                webClient.Dispose();
                            }

                            DirectoryInfo d = new DirectoryInfo(folderPath);//Assuming Test is your Folder
                            FileInfo[] Files = d.GetFiles("*.tar.gz"); //Getting Text files

                            DirectoryInfo targetDirectory = new DirectoryInfo(folderPath1);
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

                            foreach (string jsonfile in Directory.EnumerateFiles(folderPath1 + strToken + "/", "*.json", SearchOption.AllDirectories))
                            {
                                string contents = File.ReadAllText(jsonfile);
                                byte[] EK = AESEncryption.decodeBase64StringTOByte(ek_res.Trim());

                                string strJsonData = Encoding.UTF8.GetString(AESEncryption.decodeBase64StringTOByte(Encoding.UTF8.GetString(AESEncryption.decrypt(contents.Trim(), EK))));
                                string[] stringAction = strJsonData.Split('\"');
                                HttpContext.Current.Session["R1stringAction"] = stringAction[1];
                                InsertGSTR1_D_Json(strGSTINNo, strPeriod, strJsonData);
                            }
                        }
                        else if (action == "RETSUM")
                        {
                            string filingpath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/ReturnFiling"), "GSTR1_RETSUM_" + strGSTINNo + "_" + strPeriod + ".json");
                            if (File.Exists(filingpath))
                            {
                                File.Delete(filingpath);
                            }
                            File.WriteAllText(filingpath, DecryptedJsonData);
                            InsertGSTR1_RETSUM_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                        }
                        else
                        {
                            InsertGSTR1_D_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                        }

                        // Insert AuditLog 
                        if (action == "RETSTATUS")
                        {
                            Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + from + " and Status - '" + st_res + "' and Reference Id - '" + refid + "'", "");
                        }
                        else
                        {
                            Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + from + "to" + to, "");
                        }
                        flag = "S";

                        if (flagg == "1")
                        {
                            month = "";
                            month = strPeriod;
                        }
                        else if (month == "" && flagg == "")
                        {
                            month = strPeriod;
                        }
                        else if (month != "" && flagg == "" && action == "RETSTATUS")
                        {
                            month = "";
                            month = strPeriod;
                        }
                        else
                        {
                            month = month + "," + strPeriod;
                        }
                        Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLAOD", SessionUserId, SessionCustId);
                        return "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month;

                    }
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
                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    string filetokenresponse = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));

                    var filedata = (JObject)JsonConvert.DeserializeObject(filetokenresponse);
                    string token_response = filedata["token"].Value<string>();

                    if (flagg == "1")
                    {
                        month = "";
                        month = strPeriod;
                    }
                    else if (month == "" && flagg == "")
                    {
                        month = strPeriod;
                    }
                    else if (month != "" && flagg == "" && action == "RETSTATUS")
                    {
                        month = "";
                        month = strPeriod;
                    }
                    else
                    {
                        month = month + "," + strPeriod;
                    }
                    flag = "S";
                    token_flag = "true";
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + month1 + ", Token for Large File - " + token_response, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLAOD", SessionUserId, SessionCustId);
                    return "GSTR-1 " + action + " Downloaded Successfully with Token for Large File - " + token_response;
                }
                else if (flag == "S" && token_flag == "")
                {
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLAOD", SessionUserId, SessionCustId);
                    return "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month;
                }
                else if (flag == "S" && token_flag == "true")
                {
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLAOD", SessionUserId, SessionCustId);
                    return "GSTR-1 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month + ".For large files go to Audit log report.";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR1_Download_Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-1 " + action + " " + root.error.message + " for this " + strGSTINNo + " & " + from + " to " + to, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR1 DOWNLAOD", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void InsertGSTR1_D_Json(string strGSTINNo, string strPeriod, string strJsonData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR1_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RecordContents", strJsonData));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void InsertGSTR1_RETSUM_Json(string strGSTINNo, string strPeriod, string strJsonData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR1_RETSUM", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RecordContents", strJsonData));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void Updating_GSTR1_RetStatus(string strStatus, string strErrorMsg, string strRefNo)
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
                    Common.Functions.UpdateTable("TBL_GSTR1_SAVE_RETSTATUS", "referenceno", strRefNo, cmd, conn);
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }

        public string Updating_Invoice_Flag_GSTR1(string strRefNo, string ErrorFlag)
        {
            string Status_Response = "";
            con.Open();
            SqlDataAdapter adt = new SqlDataAdapter("Select gstin, fp, actiontype, refids, MinInvId, MaxInvId from TBL_GSTR1_SAVE_RETSTATUS where status != '1' and referenceno = '" + strRefNo + "'", con);
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
                    string MinInvId = dt.Rows[i]["MinInvId"].ToString();
                    string MaxInvId = dt.Rows[i]["MaxInvId"].ToString();
                    if (string.IsNullOrEmpty(RefIds))
                    {
                        GSTR1Helper.Update_Invoice_Flag_GSTR1(Action, gstinno, period, "1", ErrorFlag, MinInvId, MaxInvId);
                    }
                    else
                    {
                        Update_Invoice_Flag_GSTR1(Action, gstinno, period, RefIds, "1", ErrorFlag);
                    }
                }
            }
            con.Close();
            return Status_Response;
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

        public static void Process_RetStatus_Error_Invoices(string strGSTINNo, string strPeriod)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Process_GSTR1_RETSTATUS", conn);
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