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
using SmartAdminMvc.Models.Common;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTR2
{
    public partial class GetGSTR2ADataModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        //string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        private static string GSTR2A_Download_Response = "";
        private static string GST_Path = "", GST_TrackPath = "";
        static string gstin = "";
        static string fromdate = "";
        static string retperiod = "";
        static string action = "";
        static string token = "";
        static string transid = "";
        static string statusfilter = "";
        static string URL = "";
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "";

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strToken, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            GST_Path = ConfigurationManager.AppSettings["GST_GSTR2A"];
            GST_TrackPath = ConfigurationManager.AppSettings["GST_TRACKSTATUS"];
            try
            {
                SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_AUTH_KEYS where AuthorizationToken = '" + strGSTINNo + "'", con);
                System.Data.DataTable dt = new System.Data.DataTable();
                adt.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Username = dt.Rows[0]["Username"].ToString();
                    AppKey = dt.Rows[0]["AppKey"].ToString();
                    EncryptedSEK = dt.Rows[0]["EncryptedSEK"].ToString();
                    AuthToken = dt.Rows[0]["AuthToken"].ToString();
                }
                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                token = strToken;
                action = strAction;

                if (action == "B2B" || action == "CDN")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
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
                httpWebRequest.Headers.Add("state-cd", strStateCode); // ConfigurationManager.AppSettings["GSP_StateCode"]);
                httpWebRequest.Headers.Add("txn", ConfigurationManager.AppSettings["GSP_Txn"]);
                httpWebRequest.Headers.Add("client-secret", ConfigurationManager.AppSettings["GSP_ClientSecret"]);
                httpWebRequest.Headers.Add("ip-usr", ConfigurationManager.AppSettings["GSP_IpUsr"]);
                httpWebRequest.Headers.Add("ret_period", retperiod);
                httpWebRequest.Headers.Add("gstin", gstin);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR2A_Download_Response = result.ToString();
                    //File.WriteAllText(@"E:\GetGSTR2_B2B.json", GSTR2_B2B_Response);
                }

                var data = (JObject)JsonConvert.DeserializeObject(GSTR2A_Download_Response);
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
                    //string dataSaveResponse = GSTR2DownloadDataModel.SaveJsonDatatoDB(DecryptedJsonData, strGSTINNo, strPeriod);
                    //return DecryptedJsonData;
                    if (action == "FILEDET")
                    {
                        string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\TAR\\GSTR2A\\"));
                        folderPath = folderPath.Replace("\\", "/");

                        string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Unzip\\GSTR2A\\"));
                        folderPath1 = folderPath1.Replace("\\", "/");

                        HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                        string Json = DecryptedJsonData;
                        DirectoryInfo d1 = new DirectoryInfo(folderPath);
                        var GST_Data = JsonConvert.DeserializeObject<URLResponse>(Json);
                        int fc_res = GST_Data.fc;
                        string ek_res = GST_Data.ek;
                        for (int i = 0; i < fc_res; i++)
                        {
                            string urls_res = GST_Data.urls[i].ul.ToString();
                            string[] uls = urls_res.Split('/');
                            string[] uls1 = uls[5].Split('?');

                            WebClient webClient = new WebClient();
                            string url = "http://sbfiles.gstsystem.co.in" + urls_res;
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

                            string strJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(contents.Trim(), EK))));

                            InsertGSTR2A_Json(strGSTINNo, strPeriod, strJsonData);                                                        
                            //Response.Write(DecryptedJsonData);
                        }
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + strPeriod, "");
                        Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2A DOWNLOAD", SessionUserId, SessionCustId);
                    }
                    else
                    {
                        InsertGSTR2A_Json(strGSTINNo, strPeriod, DecryptedJsonData);

                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " & " + strPeriod, "");
                        Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2A DOWNLOAD", SessionUserId, SessionCustId);
                    }
                    //return status_response;
                    return "GSTR-2A " + action + " Downloaded Successfully";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<Common.ErrorMsg>(GSTR2A_Download_Response);
                    
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2A " + action + " " + root.error.message + " for this " + strGSTINNo + " & " + strPeriod, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2A DOWNLOAD", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void InsertGSTR2A_Json(string strGSTINNo, string strPeriod, string strJsonData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR2A_SA", conn);
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
    }
}