﻿using ICSharpCode.SharpZipLib.GZip;
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
    public partial class GspGetGstr6ADataModel
    {
        private static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public readonly string _sessioncustid;
        public readonly string _sessionuserid;
        public readonly string _sessionusername;

        public GspGetGstr6ADataModel(string SessionCustId, string SessionUserId, string SessionUsername)
        {
            this._sessioncustid = SessionCustId;
            this._sessionuserid = SessionUserId;
            this._sessionusername = SessionUsername;
        }

        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strCTIN, string strToken)
        {
            string GSTR6A_Download_Response = "";
            string GST_Path = "", GST_FileDet = "";
            string gstin = "";
            string ctin = "";
            string retperiod = "";
            string action = "";
            string token = "";
            string URL = "";
            string DecryptedJsonData = "";
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";

            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR6A"];
            GST_FileDet = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];
            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                token = strToken;
                action = strAction;
                ctin = strCTIN;

                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "CDN")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
               else if (action == "B2BA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "CDNA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "FILEDET")
                {
                    URL = GST_FileDet + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&token=" + token;
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
                    GSTR6A_Download_Response = result.ToString();
                }
                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR6A_Download_Response);

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
                    if (stringUrls[1] != "urls" && stringUrls[1] != "b2b" && stringUrls[1] != "cdn" && stringUrls[1] != "b2ba" && stringUrls[1] != "cdna")
                    {
                        string token_response = filedata["token"].Value<string>();
                        Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response, "");
                        return "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response;
                    }
                    else
                    {
                        if (action == "FILEDET")
                        {
                            string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["D_AppData_TAR_GSTR6A"]));
                            folderPath = folderPath.Replace("\\", "/");

                            string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["D_AppData_UNZIP_GSTR6A"]));
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
                                HttpContext.Current.Session["R6AstringAction"] = stringAction[1];
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
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                for (int i = 0; i < splitObject.b2b.Count; i += nSize)
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
                                var splitObject = JsonConvert.DeserializeObject<CdnJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.cdn.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        cdn = splitJsonObj
                                    });
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                        }
                        Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod, "");
                        Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6A DOWNLOAD", this._sessionuserid, this._sessioncustid);
                        
                        return "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod;
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

                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6A DOWNLOAD", this._sessionuserid, this._sessioncustid);
                    return "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR6A_Download_Response);
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, "GSTR-6A " + action + " " + root.error.message + " for this " + strGSTINNo + " and " + strPeriod, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6A DOWNLOAD", this._sessionuserid, this._sessioncustid);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void SendRequestVoid(string strGSTINNo, string strPeriod, string strAction, string strCTIN, string strToken)
        {
            string GSTR6A_Download_Response = "";
            string GST_Path = "", GST_FileDet = "";
            string gstin = "";
            string ctin = "";
            string retperiod = "";
            string action = "";
            string token = "";
            string URL = "";
            string DecryptedJsonData = "";
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";

            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR6"];
            GST_FileDet = ConfigurationManager.AppSettings["GSP_GSTR_COMMON"];
            try
            {
                Helper.GSTR_DataAdapter(strGSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string strStateCode = strGSTINNo.Substring(0, 2);
                gstin = strGSTINNo;
                retperiod = strPeriod;
                token = strToken;
                action = strAction;
                ctin = strCTIN;

                if (action == "B2B")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "CDN")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "B2BA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "CDNA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "FILEDET")
                {
                    URL = GST_FileDet + "?gstin=" + gstin + "&ret_period=" + retperiod + "&action=" + action + "&token=" + token;
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
                    GSTR6A_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR6A_Download_Response);

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
                    if (stringUrls[1] != "urls" && stringUrls[1] != "b2b" && stringUrls[1] != "cdn" && stringUrls[1] != "b2ba" && stringUrls[1] != "cdna")
                    {
                        string token_response = filedata["token"].Value<string>();

                        string Message = "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response;
                        Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, Message, "");
                    }
                    else
                    {
                        if (action == "FILEDET")
                        {
                            string folderPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["D_AppData_TAR_GSTR6A"]));
                            folderPath = folderPath.Replace("\\", "/");

                            string folderPath1 = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["D_AppData_UNZIP_GSTR6A"]));
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
                                HttpContext.Current.Session["R6AstringAction"] = stringAction[1];
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
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                        InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                for (int i = 0; i < splitObject.b2b.Count; i += nSize)
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
                                var splitObject = JsonConvert.DeserializeObject<CdnJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.cdn.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.cdn.GetRange(i, Math.Min(nSize, splitObject.cdn.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        cdn = splitJsonObj
                                    });
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6A_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                        }

                        string Message = "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod;
                        Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, Message, "");

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

                    string Message = "GSTR-6A " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response;
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, Message, "");

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR6A_Download_Response);

                    string Message = "GSTR-6A " + action + " " + root.error.message + " for this " + strGSTINNo + " and " + strPeriod;
                    string Exception = "Error Code - " + root.error.error_cd;
                    Helper.InsertAuditLog(this._sessionuserid, this._sessionusername, Message, Exception);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
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