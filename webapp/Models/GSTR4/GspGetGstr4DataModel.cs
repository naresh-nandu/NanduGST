using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
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
using System.Web;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR4
{
    public class GspGetGstr4DataModel
    {


        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strCTIN, string strRefId, string strToken, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string GSTR4_Download_Response = "";
            string GST_Path = "", GST_TrackPath = "";
            string gstin = "";
            string ctin = "";
            string retperiod = "";
            string action = "";
            string token = "";
            string refid = "";
            string URL = "";

            string DecryptedJsonData = "";

            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";

            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR4"];
            GST_TrackPath = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];

            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                ctin = strCTIN;
                refid = strRefId;
                action = strAction;
                token = strToken;

                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "B2BA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "CDNR")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "CDNRA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "TDS")
                {
                    URL = GST_TrackPath + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&ref_id=" + refid;
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

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR4_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR4_Download_Response);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    string data_response = GST_Data.data.ToString();
                    string rek_response = GST_Data.rek.ToString();

                    string decrypted_appkey = AppKey;
                    string receivedSEK = EncryptedSEK;
                    string gotREK = rek_response;
                    string gotdata = data_response;

                    byte[] authEK = AESEncryption.decrypt(receivedSEK, AESEncryption.decodeBase64StringTOByte(decrypted_appkey));
                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    DecryptedJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));
                    if (action == "RETSTATUS")
                    {
                        var downloaddata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                        string st_res = downloaddata["status_cd"].Value<string>();

                        HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                        if (st_res == "P")
                        {
                            Updating_GSTR4_RetStatus(st_res, downloaddata.ToString(), refid);
                            Updating_Invoice_Flag_GSTR4(refid, "U");
                        }
                        else if (st_res == "IP")
                        {
                            Updating_GSTR4_RetStatus(st_res, downloaddata.ToString(), refid);
                            Updating_Invoice_Flag_GSTR4(refid, "1");
                        }
                        else if (st_res == "PE" || st_res == "ER")
                        {
                            Updating_GSTR4_RetStatus(st_res, downloaddata.ToString(), refid);
                            Updating_Invoice_Flag_GSTR4(refid, "1");
                            Process_RetStatus_Error_Invoices(strGSTINNo, strPeriod);
                        }
                        else
                        {
                            //..
                        }
                    }
                    else if (action == "FILEDET")
                    {
                        string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\TAR\\GSTR4\\"));
                        folderPath = folderPath.Replace("\\", "/");

                        string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Unzip\\GSTR4\\"));
                        folderPath1 = folderPath1.Replace("\\", "/");

                        HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                        string Json = DecryptedJsonData;
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

                        foreach (string jsonfile in Directory.EnumerateFiles(folderPath1, "*.json", SearchOption.AllDirectories))
                        {
                            string contents = File.ReadAllText(jsonfile);
                            byte[] EK = AESEncryption.decodeBase64StringTOByte(ek_res.Trim());
                            string strJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(contents.Trim(), EK))));
                        //    InsertGSTR2_D_Json(strGSTINNo, strPeriod, strJsonData);
                        }
                    }
                    else if (action == "RETSUM")
                    {
                        //InsertGSTR2_RETSUM_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                    }
                    else
                    {
                     //   InsertGSTR2_D_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                    }

                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-4 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR4 DOWNLOAD", SessionUserId, SessionCustId);
                    return "GSTR-4 " + action + " Downloaded Successfully";
                }
                else if (status_response == "2")
                {
                    string data_response = GST_Data.data.ToString();
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

                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-4 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR4 DOWNLOAD", SessionUserId, SessionCustId);

                    return "GSTR-4 " + action + " Downloaded Successfully with Token for Large File - " + token_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR4_Download_Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-4 " + action + " " + root.error.message + " for this " + strGSTINNo + " & " + strPeriod, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR4 DOWNLOAD", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void Updating_GSTR4_RetStatus(string strStatus, string strErrorMsg, string strRefNo)
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
                    Common.Functions.UpdateTable("TBL_GSTR4_SAVE_RETSTATUS", "referenceno", strRefNo, cmd, conn);
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public string Updating_Invoice_Flag_GSTR4(string strRefNo, string ErrorFlag)
        {
            string Status_Response = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                conn.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select gstin, fp, actiontype, refids from TBL_GSTR4_SAVE_RETSTATUS where status != '1' and referenceno = @RefNo";
                    sqlcmd.Parameters.AddWithValue("@RefNo", strRefNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
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

                                Update_Invoice_Flag_GSTR4(Action, gstinno, period, RefIds, "1", ErrorFlag);
                            }
                        }
                    }
                }
                conn.Close();
            }
            return Status_Response;
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