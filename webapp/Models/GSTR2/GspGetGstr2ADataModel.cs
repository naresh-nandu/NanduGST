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
using GSTR2ADownload;
using WeP_DAL;
using WeP_BAL.GstrDownload;

namespace SmartAdminMvc.Models.GSTR2
{
    public partial class GspGetGstr2ADataModel
    {
        public static readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        string flag = "";
        string token_flag = "";
        string month = "";
        string month1 = "";

        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strToken, string from, string to, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string GSTR2A_Download_Response = "";
            string GST_Path = "", GST_FileDet = "";
            string gstin = "";
            string retperiod = "";
            string action = "";
            string token = "";
            string URL = "";
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string DecryptedJsonData = "";

            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR2A"];
            GST_FileDet = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];
            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                token = strToken;
                action = strAction;

                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "CDN")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "FILEDET")
                {
                    URL = GST_FileDet + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&token=" + token;
                }
                if (action == "B2BA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                if (action == "CDNA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }

                if (action == "ISD")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                if (action == "ISDA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                if (action == "TDS")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                if (action == "TDSA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
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
                httpWebRequest.Headers.Add("GSTINNO", strGSTINNo);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR2A_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR2A_Download_Response);

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

                    var filedata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                    string[] stringUrls = DecryptedJsonData.Split('\"');
                    if (stringUrls[1] != "urls" && stringUrls[1] != "b2b" && stringUrls[1] != "cdn" && stringUrls[1] != "b2ba" && stringUrls[1] != "cdna" && stringUrls[1] != "isd" && stringUrls[1] != "isda" && stringUrls[1] != "tds" && stringUrls[1] != "tdsa")
                    {
                        string token_response = filedata["token"].Value<string>();

                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + from + " to " + to + ", Token for Large File - " + token_response, "");

                        return "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + from + " to " + to + ", Token for Large File - " + token_response;
                    }
                    else
                    {
                        if (action == "FILEDET")
                        {
                            string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["D_AppData_TAR_GSTR2A"]));
                            folderPath = folderPath.Replace("\\", "/");

                            string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["D_AppData_UNZIP_GSTR2A"]));
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

                                using (WebClient webClient = new WebClient())
                                {
                                    string url = ConfigurationManager.AppSettings["GSP_LARGEFILEDOWNLOAD"] + urls_res;
                                    webClient.DownloadFile(url, folderPath + uls1[0]);
                                }
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
                                HttpContext.Current.Session["R2AstringAction"] = stringAction[1];
                                int nSize = 15;
                                if (stringAction[1] == "b2b")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<B2BJson>(strJsonData);
                                    for (int i = 0; i < splitObject.b2b.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.b2b.GetRange(i, Math.Min(nSize, splitObject.b2b.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            b2b = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "cdn")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<CdnJson>(strJsonData);
                                    for (int i = 0; i < splitObject.cdn.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            cdn = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "b2ba")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<B2BAJson>(strJsonData);
                                    for (int i = 0; i < splitObject.b2ba.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.b2ba.GetRange(i, Math.Min(nSize, splitObject.b2ba.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            b2ba = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "cdna")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<CdnaJson>(strJsonData);
                                    for (int i = 0; i < splitObject.cdna.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.cdna.GetRange(i, Math.Min(nSize, splitObject.cdna.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            cdna = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "isd")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<IsdJson>(strJsonData);
                                    for (int i = 0; i < splitObject.isd.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.isd.GetRange(i, Math.Min(nSize, splitObject.isd.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            isd = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "isda")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<IsdaJson>(strJsonData);
                                    for (int i = 0; i < splitObject.isda.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.isda.GetRange(i, Math.Min(nSize, splitObject.isda.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            isda = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "tds")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<TdsJson>(strJsonData);
                                    for (int i = 0; i < splitObject.tds.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.tds.GetRange(i, Math.Min(nSize, splitObject.tds.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            tds = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "tdsa")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<TdsaJson>(strJsonData);
                                    for (int i = 0; i < splitObject.tdsa.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.tdsa.GetRange(i, Math.Min(nSize, splitObject.tdsa.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            tdsa = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
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
                                for (int i = 0; i < splitObject.b2b.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.b2b.GetRange(i, Math.Min(nSize, splitObject.b2b.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        b2b = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "CDN")
                            {
                                var splitObject = JsonConvert.DeserializeObject<CdnJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.cdn.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        cdn = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "B2BA")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<B2BAJson>(DecryptedJsonData);
                                    for (int i = 0; i < splitObject.b2ba.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.b2ba.GetRange(i, Math.Min(nSize, splitObject.b2ba.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            b2ba = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                            if (action == "CDNA")
                            {
                                var splitObject = JsonConvert.DeserializeObject<CdnaJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.cdna.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.cdna.GetRange(i, Math.Min(nSize, splitObject.cdna.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        cdna = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "ISD")
                            {
                                var splitObject = JsonConvert.DeserializeObject<IsdJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.isd.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.isd.GetRange(i, Math.Min(nSize, splitObject.isd.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        isd = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "ISDA")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<IsdaJson>(DecryptedJsonData);
                                    for (int i = 0; i < splitObject.isda.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.isda.GetRange(i, Math.Min(nSize, splitObject.isda.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            isda = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                            if (action == "TDS")
                            {
                                var splitObject = JsonConvert.DeserializeObject<TdsJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.tds.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.tds.GetRange(i, Math.Min(nSize, splitObject.tds.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        tds = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "TDSA")
                            {
                                var splitObject = JsonConvert.DeserializeObject<TdsaJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.tdsa.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.tdsa.GetRange(i, Math.Min(nSize, splitObject.tdsa.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        tdsa = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                        }
                        flag = "S";
                        if (month == "")
                        {
                            month = strPeriod;
                        }
                        else
                        {
                            month = month + "," + strPeriod;
                        }
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month, "");
                        Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2A DOWNLOAD", SessionUserId, SessionCustId);

                        return "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month;
                    }
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

                    if (month1 == "")
                    {
                        month1 = strPeriod;
                    }
                    else
                    {
                        month1 = month1 + "," + strPeriod;
                    }
                    flag = "S";
                    token_flag = "true";
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month1 + ", Token for Large File - " + token_response, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2A DOWNLOAD", SessionUserId, SessionCustId);
                    return "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month1 + ", Token for Large File - " + token_response;
                }
                else if (flag == "S" && token_flag == "")
                {
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2A DOWNLOAD", SessionUserId, SessionCustId);

                    return "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month;
                }
                else if (flag == "S" && token_flag == "true")
                {
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2A DOWNLOAD", SessionUserId, SessionCustId);
                    return "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month + " .For large files go to Audit log report.";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR2A_Download_Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-2A " + action + " " + root.error.message + " for this " + strGSTINNo + " and " + strPeriod, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR2A DOWNLOAD", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void SendRequestVoid(string strGSTINNo, string strPeriod, string strAction, string strToken, string from, string to, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string GSTR2A_Download_Response = "";
            string GST_Path = "";
            string gstin = "";
            string retperiod = "";
            string action = "";
            string token = "";
            string URL = "";
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string DecryptedJsonData = "";

            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR2A"];
            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                token = strToken;
                action = strAction;

                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "CDN")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                if (action == "B2BA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                if (action == "CDNA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                if (action == "ISD")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                if (action == "ISDA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "FILEDET")
                {
                    URL = GST_Path + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&token=" + token;
                }
                if (action == "TDS")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                if (action == "TDSA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
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
                httpWebRequest.Headers.Add("GSTINNO", strGSTINNo);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSTR2A_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR2A_Download_Response);

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

                    var filedata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                    string[] stringUrls = DecryptedJsonData.Split('\"');
                    if (stringUrls[1] != "urls" && stringUrls[1] != "b2b" && stringUrls[1] != "cdn" && stringUrls[1] != "b2ba" && stringUrls[1] != "cdna" && stringUrls[1] != "isd" && stringUrls[1] != "isda" && stringUrls[1] != "tds" && stringUrls[1] != "tdsa")
                    {
                        string token_response = filedata["token"].Value<string>();
                        if (!string.IsNullOrEmpty(token_response))
                        {
                            new GstrDownload(Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId)).Insert_GSTRDownload_Filedet(strGSTINNo, strPeriod, strAction, token_response, "GSTR2ADownload","", "", SessionUsername);
                        }
                        string Message = "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + " to " + to + ", Token for Large File - " + token_response + "And Period for _" + strPeriod;
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, "");
                    }
                    else
                    {
                        if (action == "FILEDET")
                        {
                            string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["D_AppData_TAR_GSTR2A"]));
                            folderPath = folderPath.Replace("\\", "/");

                            string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["D_AppData_UNZIP_GSTR2A"]));
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

                            foreach (string jsonfile in Directory.EnumerateFiles(folderPath1 + strToken + "/", "*.json", SearchOption.AllDirectories))
                            {
                                string contents = File.ReadAllText(jsonfile);
                                byte[] EK = AESEncryption.decodeBase64StringTOByte(ek_res.Trim());

                                string strJsonData = Helper.openFileToString(AESEncryption.decodeBase64StringTOByte(Helper.openFileToString(AESEncryption.decrypt(contents.Trim(), EK))));
                                string[] stringAction = strJsonData.Split('\"');
                                HttpContext.Current.Session["R2AstringAction"] = stringAction[1];
                                int nSize = 15;
                                if (stringAction[1] == "b2b")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<B2BJson>(strJsonData);
                                    for (int i = 0; i < splitObject.b2b.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.b2b.GetRange(i, Math.Min(nSize, splitObject.b2b.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            b2b = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "cdn")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<CdnJson>(strJsonData);
                                    for (int i = 0; i < splitObject.cdn.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            cdn = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "b2ba")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<B2BAJson>(strJsonData);
                                    for (int i = 0; i < splitObject.b2ba.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.b2ba.GetRange(i, Math.Min(nSize, splitObject.b2ba.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            b2ba = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "cdna")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<CdnaJson>(strJsonData);
                                    for (int i = 0; i < splitObject.cdna.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.cdna.GetRange(i, Math.Min(nSize, splitObject.cdna.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            cdna = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "isd")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<IsdJson>(strJsonData);
                                    for (int i = 0; i < splitObject.isd.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.isd.GetRange(i, Math.Min(nSize, splitObject.isd.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            isd = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "isda")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<IsdaJson>(strJsonData);
                                    for (int i = 0; i < splitObject.isda.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.isda.GetRange(i, Math.Min(nSize, splitObject.isda.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            isda = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "tds")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<TdsJson>(strJsonData);
                                    for (int i = 0; i < splitObject.tds.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.tds.GetRange(i, Math.Min(nSize, splitObject.tds.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            tds = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                    }
                                }
                                if (stringAction[1] == "tdsa")
                                {
                                    var splitObject = JsonConvert.DeserializeObject<TdsaJson>(strJsonData);
                                    for (int i = 0; i < splitObject.tdsa.Count; i += nSize)
                                    {
                                        var splitJsonObj = splitObject.tdsa.GetRange(i, Math.Min(nSize, splitObject.tdsa.Count - i));

                                        var splitJson = JsonConvert.SerializeObject(new
                                        {
                                            tdsa = splitJsonObj
                                        });
                                        InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
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
                                for (int i = 0; i < splitObject.b2b.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.b2b.GetRange(i, Math.Min(nSize, splitObject.b2b.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        b2b = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "CDN")
                            {
                                var splitObject = JsonConvert.DeserializeObject<CdnJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.cdn.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        cdn = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "B2BA")
                            {
                                var splitObject = JsonConvert.DeserializeObject<B2BAJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.b2ba.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.b2ba.GetRange(i, Math.Min(nSize, splitObject.b2ba.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        b2ba = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "CDNA")
                            {
                                var splitObject = JsonConvert.DeserializeObject<CdnaJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.cdna.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.cdna.GetRange(i, Math.Min(nSize, splitObject.cdna.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        cdna = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "ISD")
                            {
                                var splitObject = JsonConvert.DeserializeObject<IsdJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.isd.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.isd.GetRange(i, Math.Min(nSize, splitObject.isd.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        isd = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "ISDA")
                            {
                                var splitObject = JsonConvert.DeserializeObject<IsdaJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.isda.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.isda.GetRange(i, Math.Min(nSize, splitObject.isda.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        isda = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "TDS")
                            {
                                var splitObject = JsonConvert.DeserializeObject<TdsJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.tds.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.tds.GetRange(i, Math.Min(nSize, splitObject.tds.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        tds = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "TDSA")
                            {
                                var splitObject = JsonConvert.DeserializeObject<TdsaJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.tdsa.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.tdsa.GetRange(i, Math.Min(nSize, splitObject.tdsa.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        tdsa = splitJsonObj
                                    });
                                    InsertGSTR2A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                        }
                        flag = "S";
                        if (month == "")
                        {
                            month = strPeriod;
                        }
                        else
                        {
                            month = month + "," + strPeriod;
                        }
                        string Message = "GSTR-2A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month;
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, "");

                    }
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


                    if (month1 == "")
                    {
                        month1 = strPeriod;
                    }
                    else
                    {
                        month1 = month1 + "," + strPeriod;
                    }
                    flag = "S";
                    token_flag = "true";

                    if (!string.IsNullOrEmpty(token_response))
                    {
                        new GstrDownload(Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId)).Insert_GSTRDownload_Filedet(strGSTINNo, strPeriod, strAction, token_response, "GSTR6Download", "", "", SessionUsername);
                    }
                    string Message = "GSTR-6 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + month1 + ", Token for Large File - " + token_response + "And Period For _ " +strPeriod;
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, "");

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR2A_Download_Response);

                    string Message = "GSTR-2A " + action + " " + root.error.message + " for this " + strGSTINNo + " and " + strPeriod;
                    string Exception = "Error Code - " + root.error.error_cd;
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, Exception);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
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
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RecordContents", strJsonData));
                    dCmd.Connection = conn;
                    dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
    }
}