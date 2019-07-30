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
using WeP_DAL.GSTR6;

namespace SmartAdminMvc.Models.GSTR6
{
    public class GSP_GetGSTR6ADataModel
    {
        public static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public static string conString = ConfigurationManager.AppSettings["ConnectionString"];
        //string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        private static string GSTR6A_Download_Response = "";
        private static string GST_Path = "", GST_FileDet = "";
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

        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strToken,string strfromdt, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR6"];
            GST_FileDet = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];
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
                fromdate = strfromdt;
                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&from_time=" + strfromdt;
                }
                else if (action == "CDN")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&from_time=" + strfromdt;
                }
                else if (action == "FILEDET")
                {
                    URL = GST_FileDet + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&token=" + token + "&from_time=" + strfromdt;
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
                    GSTR6A_Download_Response = result.ToString();
                }
                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR6A_Download_Response);

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

                    var filedata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                    string[] stringUrls = DecryptedJsonData.Split('\"');
                    if (stringUrls[1] != "urls" && stringUrls[1] != "b2b" && stringUrls[1] != "cdn")
                    {
                        string token_response = filedata["token"].Value<string>();
                        //InsertGSTR6A_Json(strGSTINNo, strPeriod, DecryptedJsonData);

                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response, "");

                        return "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response;
                    }
                    else
                    {
                        if (action == "FILEDET")
                        {
                            string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\TAR\\GSTR6A\\"));
                            folderPath = folderPath.Replace("\\", "/");

                            string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Unzip\\GSTR6A\\"));
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

                            foreach (string jsonfile in Directory.EnumerateFiles(folderPath1 + strToken + "/", "*.json", SearchOption.AllDirectories))
                            {
                                //string contents = File.ReadAllText(file);
                                string contents = File.ReadAllText(jsonfile);

                                //Response.Write(contents + "<br /><br />");
                                //byte[] Ek = AESEncryption.decodeBase64StringTOByte(AppKey);
                                byte[] EK = AESEncryption.decodeBase64StringTOByte(ek_res.Trim());

                                string strJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(contents.Trim(), EK))));
                                string[] stringAction = strJsonData.Split('\"');
                                HttpContext.Current.Session["R6AstringAction"] = stringAction[1];
                                //var aa = JsonConvert.DeserializeObject<List<B2B>>(strJsonData);
                                //InsertGSTR6A_Json(strGSTINNo, strPeriod, strJsonData);
                                int nSize = 15;
                                if (stringAction[1] == "b2b")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<B2BJson>(strJsonData);
                                    var list = new List<B2B>();
                                    B2BJson b2b = new B2BJson();
                                    for (int i = 0; i < splitObject.b2b.Count(); i += nSize)
                                    {
                                        var splitJsonObj = splitObject.b2b.GetRange(i, Math.Min(nSize, splitObject.b2b.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            b2b = splitJsonObj
                                        });
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "cdn")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<CDNJson>(strJsonData);
                                    var list = new List<CDN>();
                                    CDNJson cdn = new CDNJson();
                                    for (int i = 0; i < splitObject.cdn.Count(); i += nSize)
                                    {
                                        var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            cdn = splitJsonObj
                                        });
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                            }
                        }
                        else
                        {
                            int nSize = 15;
                            if (action == "B2B")
                            {
                                var splitObject = JsonConvert.DeserializeObject<B2BJson>(DecryptedJsonData);
                                var list = new List<B2B>();
                                B2BJson b2b = new B2BJson();
                                for (int i = 0; i < splitObject.b2b.Count(); i += nSize)
                                {
                                    var splitJsonObj = splitObject.b2b.GetRange(i, Math.Min(nSize, splitObject.b2b.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        b2b = splitJsonObj
                                    });
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "CDN")
                            {
                                var splitObject = JsonConvert.DeserializeObject<CDNJson>(DecryptedJsonData);
                                var list = new List<CDN>();
                                CDNJson cdn = new CDNJson();
                                for (int i = 0; i < splitObject.cdn.Count(); i += nSize)
                                {
                                    var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        cdn = splitJsonObj
                                    });
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            //InsertGSTR6A_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                        }
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod, "");
                        Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6A DOWNLOAD", SessionUserId, SessionCustId);
                        //return status_response;
                        return "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod;
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
                    //System.out.println("Encoded Auth EK (Received):" + AESEncryption.encodeBase64String(authEK));

                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    //System.out.println("Encoded Api EK (Received):" + AESEncryption.encodeBase64String(apiEK));
                    string filetokenresponse = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));

                    var filedata = (JObject)JsonConvert.DeserializeObject(filetokenresponse);
                    string token_response = filedata["token"].Value<string>();

                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6A DOWNLOAD", SessionUserId, SessionCustId);
                    return "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR6A_Download_Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-6A " + action + " " + root.error.message + " for this " + strGSTINNo + " and " + strPeriod, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6A DOWNLOAD", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void SendRequestVoid(string strGSTINNo, string strPeriod, string strAction, string strToken, string strfromdt, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR6"];
            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);
                //string AuthorizeToken = GetAccessToken.getAuthToken();

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                token = strToken;
                action = strAction;
                fromdate = strfromdt;

                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&from_time=" + strfromdt;
                }
                else if (action == "CDN")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&from_time=" + strfromdt;
                }
                else if (action == "FILEDET")
                {
                    URL = GST_Path + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&token=" + token + "&from_time=" + strfromdt;
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
                    GSTR6A_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR6A_Download_Response);

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

                    var filedata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                    string[] stringUrls = DecryptedJsonData.Split('\"');
                    if (stringUrls[1] != "urls" && stringUrls[1] != "b2b" && stringUrls[1] != "cdn")
                    {
                        string token_response = filedata["token"].Value<string>();

                        string Message = "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response;
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, "");
                        //InsertGSTR6A_Json(strGSTINNo, strPeriod, DecryptedJsonData);                                                        
                    }
                    else
                    {
                        if (action == "FILEDET")
                        {
                            string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\TAR\\GSTR6A\\"));
                            folderPath = folderPath.Replace("\\", "/");

                            string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Unzip\\GSTR6A\\"));
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

                            foreach (string jsonfile in Directory.EnumerateFiles(folderPath1 + strToken + "/", "*.json", SearchOption.AllDirectories))
                            {
                                //string contents = File.ReadAllText(file);
                                string contents = File.ReadAllText(jsonfile);

                                //Response.Write(contents + "<br /><br />");
                                //byte[] Ek = AESEncryption.decodeBase64StringTOByte(AppKey);
                                byte[] EK = AESEncryption.decodeBase64StringTOByte(ek_res.Trim());

                                string strJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(contents.Trim(), EK))));
                                string[] stringAction = strJsonData.Split('\"');
                                HttpContext.Current.Session["R6AstringAction"] = stringAction[1];
                                //var aa = JsonConvert.DeserializeObject<List<B2B>>(strJsonData);
                                //InsertGSTR6A_Json(strGSTINNo, strPeriod, strJsonData);
                                int nSize = 15;
                                if (stringAction[1] == "b2b")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<B2BJson>(strJsonData);
                                    var list = new List<B2B>();
                                    B2BJson b2b = new B2BJson();
                                    for (int i = 0; i < splitObject.b2b.Count(); i += nSize)
                                    {
                                        var splitJsonObj = splitObject.b2b.GetRange(i, Math.Min(nSize, splitObject.b2b.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            b2b = splitJsonObj
                                        });
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "cdn")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<CDNJson>(strJsonData);
                                    var list = new List<CDN>();
                                    CDNJson cdn = new CDNJson();
                                    for (int i = 0; i < splitObject.cdn.Count(); i += nSize)
                                    {
                                        var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            cdn = splitJsonObj
                                        });
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                            }
                        }
                        else
                        {
                            int nSize = 15;
                            if (action == "B2B")
                            {
                                var splitObject = JsonConvert.DeserializeObject<B2BJson>(DecryptedJsonData);
                                var list = new List<B2B>();
                                B2BJson b2b = new B2BJson();
                                for (int i = 0; i < splitObject.b2b.Count(); i += nSize)
                                {
                                    var splitJsonObj = splitObject.b2b.GetRange(i, Math.Min(nSize, splitObject.b2b.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        b2b = splitJsonObj
                                    });
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "CDN")
                            {
                                var splitObject = JsonConvert.DeserializeObject<CDNJson>(DecryptedJsonData);
                                var list = new List<CDN>();
                                CDNJson cdn = new CDNJson();
                                for (int i = 0; i < splitObject.cdn.Count(); i += nSize)
                                {
                                    var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        cdn = splitJsonObj
                                    });
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            //InsertGSTR6A_Json(strGSTINNo, strPeriod, DecryptedJsonData);
                        }

                        string Message = "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod;
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, "");

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
                    //System.out.println("Encoded Auth EK (Received):" + AESEncryption.encodeBase64String(authEK));

                    byte[] apiEK = AESEncryption.decrypt(gotREK, authEK);
                    //System.out.println("Encoded Api EK (Received):" + AESEncryption.encodeBase64String(apiEK));
                    string filetokenresponse = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(gotdata, apiEK))));

                    var filedata = (JObject)JsonConvert.DeserializeObject(filetokenresponse);
                    string token_response = filedata["token"].Value<string>();

                    string Message = "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response;
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, "");

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR6A_Download_Response);

                    string Message = "GSTR-6A " + action + " " + root.error.message + " for this " + strGSTINNo + " and " + strPeriod;
                    string Exception = "Error Code - " + root.error.error_cd;
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, Exception);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InsertGSTR6A_Json(string strGSTINNo, string strPeriod, string strJsonData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR6A_SA", conn);
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
    }
}