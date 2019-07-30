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
using WeP_BAL.GstrDownload;
using WeP_DAL.GSTR6;

namespace SmartAdminMvc.Models.GSTR6
{
    public partial class GspGetGstr6DataModel
    {                
        public string SendRequest(string strGSTINNo, string strPeriod, string strAction, string strCTIN, string strRefId, string strToken, string from, string to, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string GSTR6_Download_Response = "";
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

            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR6"];
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
                    URL = GST_Path + "?gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "B2BA")
                {
                    URL = GST_Path + "?gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "CDN")
                {
                    URL = GST_Path + "?gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "CDNA")
                {
                    URL = GST_Path + "?gstin=" + gstin + "&ret_period=" + retperiod + "&ctin=" + ctin;
                }
                else if (action == "ISD")
                {
                    URL = GST_Path + "?gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "ISDA")
                {
                    URL = GST_Path + "?gstin=" + gstin + "&ret_period=" + retperiod;
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
                    GSTR6_Download_Response = result.ToString();
                }
                                
                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR6_Download_Response);

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
                    if (stringUrls[1] != "urls" && stringUrls[1] != "b2b" && stringUrls[1] != "cdn" && stringUrls[1] != "b2ba" && stringUrls[1] != "cdna" && stringUrls[1] != "isd" && stringUrls[1] != "isda" && stringUrls[1] != "urls" && stringUrls[1] != "form_typ")
                    {
                        string token_response = filedata["token"].Value<string>();
                        if (!string.IsNullOrEmpty(token_response))
                        {
                            new GstrDownload(Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId)).Insert_GSTRDownload_Filedet(strGSTINNo, strPeriod, strAction, token_response, "GSTR6Download", "", "", SessionUsername);
                        }
                        string Message = "GSTR-6 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response + "And Period for _" + strPeriod;
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, "");
                        return "GSTR-6 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + from + " to " + to + ", Token for Large File - " + token_response;
                    }
                    else
                    {
                        if (action == "RETSTATUS")
                        {
                            var downloaddata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                            string st_res = downloaddata["status_cd"].Value<string>();

                            HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                            if (st_res == "P")
                            {
                                Updating_GSTR6_RetStatus(st_res, downloaddata.ToString(), refid);
                                Updating_Invoice_Flag_GSTR6(refid, "U");
                            }
                            else if (st_res == "IP")
                            {
                                Updating_GSTR6_RetStatus(st_res, downloaddata.ToString(), refid);
                                Updating_Invoice_Flag_GSTR6(refid, "1");
                            }
                            else if (st_res == "PE" || st_res == "ER")
                            {
                                Updating_GSTR6_RetStatus(st_res, downloaddata.ToString(), refid);
                                Updating_Invoice_Flag_GSTR6(refid, "1");
                               // Process_RetStatus_Error_Invoices(strGSTINNo, strPeriod);
                            }
                            else
                            {
                                //..
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
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "ISD")
                            {
                                var splitObject = JsonConvert.DeserializeObject<ISDJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.isd.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.isd.GetRange(i, Math.Min(nSize, splitObject.isd.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        isd = splitJsonObj
                                    });
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "ISDA")
                            {
                                var splitObject = JsonConvert.DeserializeObject<ISDAJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.isda.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.isda.GetRange(i, Math.Min(nSize, splitObject.isda.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        isda = splitJsonObj
                                    });
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                        }
                        return "GSTR-6 " + action + " Downloaded Successfully for this " + strGSTINNo;
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

                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-6 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6 DOWNLOAD", SessionUserId, SessionCustId);

                    return "GSTR-6 " + action + " Downloaded Successfully with Token for Large File - " + token_response;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR6_Download_Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-6 " + action + " " + root.error.message + " for this " + strGSTINNo + " & " + strPeriod, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6 DOWNLOAD", SessionUserId, SessionCustId);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void SendRequestVoid(string strGSTINNo, string strPeriod, string strAction, string strCTIN, string strRefId, string strToken, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string GSTR6_Download_Response = "";
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

            GST_Path = ConfigurationManager.AppSettings["GSP_GSTR6"];
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
                else if (action == "CDN")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "CDNA")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "ISD")
                {
                    URL = GST_Path + "?action=" + action + "&gstin=" + gstin + "&ret_period=" + retperiod;
                }
                else if (action == "ISDA")
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
                    GSTR6_Download_Response = result.ToString();
                }

                var GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR6_Download_Response);

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
                    if (stringUrls[1] != "urls" && stringUrls[1] != "b2b" && stringUrls[1] != "cdn" && stringUrls[1] != "b2ba" && stringUrls[1] != "cdna" && stringUrls[1] != "isd" && stringUrls[1] != "isda")
                    {
                        string token_response = filedata["token"].Value<string>();
                        if (!string.IsNullOrEmpty(token_response))
                        {
                            new GstrDownload(Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId)).Insert_GSTRDownload_Filedet(strGSTINNo, strPeriod, strAction, token_response, "GSTR6Download", "", "", SessionUsername);
                        }
                        string Message = "GSTR-6 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response + "And Period for _" + strPeriod;
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, Message, "");
                    }
                    else
                    {
                        if (action == "RETSTATUS")
                        {
                            var downloaddata = (JObject)JsonConvert.DeserializeObject(DecryptedJsonData);
                            string st_res = downloaddata["status_cd"].Value<string>();

                            HttpContext.Current.Session["RETSTATUS"] = DecryptedJsonData;
                            if (st_res == "P")
                            {
                                Updating_GSTR6_RetStatus(st_res, downloaddata.ToString(), refid);
                                Updating_Invoice_Flag_GSTR6(refid, "U");
                            }
                            else if (st_res == "IP")
                            {
                                Updating_GSTR6_RetStatus(st_res, downloaddata.ToString(), refid);
                                Updating_Invoice_Flag_GSTR6(refid, "1");
                            }
                            else if (st_res == "PE" || st_res == "ER")
                            {
                                Updating_GSTR6_RetStatus(st_res, downloaddata.ToString(), refid);
                                Updating_Invoice_Flag_GSTR6(refid, "1");
                                Process_RetStatus_Error_Invoices(strGSTINNo, strPeriod);
                            }
                            else
                            {
                                //..
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
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
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
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "ISD")
                            {
                                var splitObject = JsonConvert.DeserializeObject<ISDJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.isd.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.isd.GetRange(i, Math.Min(nSize, splitObject.isd.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        isd = splitJsonObj
                                    });
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                            if (action == "ISDA")
                            {
                                var splitObject = JsonConvert.DeserializeObject<ISDAJson>(DecryptedJsonData);
                                for (int i = 0; i < splitObject.isda.Count; i += nSize)
                                {
                                    var splitJsonObj = splitObject.isda.GetRange(i, Math.Min(nSize, splitObject.isda.Count - i));

                                    var splitJson = JsonConvert.SerializeObject(new
                                    {
                                        isda = splitJsonObj
                                    });
                                    InsertGSTR6_D_Json(strGSTINNo, strPeriod, splitJson);
                                }
                            }
                        }
                    }
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-6 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6 DOWNLOAD", SessionUserId, SessionCustId);
                  
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

                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-6 " + action + " Downloaded Successfully for this " + strGSTINNo + " and " + strPeriod + ", Token for Large File - " + token_response, "");
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6 DOWNLOAD", SessionUserId, SessionCustId);
                   
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR6_Download_Response);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "GSTR-6 " + action + " " + root.error.message + " for this " + strGSTINNo + " & " + strPeriod, "Error Code - " + root.error.error_cd);
                    Helper.InsertAPICountTransactions(strGSTINNo, strPeriod, "GSTR6 DOWNLAOD", SessionUserId, SessionCustId);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public static void InsertGSTR6_D_Json(string strGSTINNo, string strPeriod, string strJsonData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR6_D_SA", conn);
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


        public static void Updating_GSTR6_RetStatus(string strStatus, string strErrorMsg, string strRefNo)
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
                    Common.Functions.UpdateTable("TBL_GSTR6_SAVE_RETSTATUS", "referenceno", strRefNo, cmd, conn);
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public string Updating_Invoice_Flag_GSTR6(string strRefNo, string ErrorFlag)
        {
            string Status_Response = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                conn.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.CommandTimeout = 0;
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select gstin, fp, actiontype, refids from TBL_GSTR6_SAVE_RETSTATUS where status != '1' and referenceno = @RefNo";
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

                                Update_Invoice_Flag_GSTR6(Action, gstinno, period, RefIds, "1", ErrorFlag);
                            }
                        }
                    }
                }
                conn.Close();
            }
            return Status_Response;
        }


        public static void Update_Invoice_Flag_GSTR6(string ActionType, string strGSTINNo, string strPeriod, string RefIds, string CurFlag, string NewFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR6", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
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
                    dCmd.CommandTimeout = 0;
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