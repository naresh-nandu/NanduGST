﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTAUTH;
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
using System.Web.Script.Serialization;
using static SmartAdminMvc.Models.GSTR3.GspSendGstr3BSaveDataModel;
using WeP_DAL;
using WeP_GSTN;

namespace SmartAdminMvc.Models.Ledger
{
    public class OffsetLiabilityGstr3BSendData
    {
        public string SendRequest(string JsonData, string GSTINNo, string Period, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string GSTR3BResponse = "";
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

                GstRequestResponseAttributes objGSTR3BSaveParams = new GstRequestResponseAttributes();
                objGSTR3BSaveParams.data = payload;
                objGSTR3BSaveParams.hmac = strhmac;
                objGSTR3BSaveParams.action = "RETOFFSET";

                string tempjsondata = new JavaScriptSerializer().Serialize(objGSTR3BSaveParams);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GSP_GSTR3B"]);
                httpWebRequest.Timeout = 1000000;
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
                httpWebRequest.Headers.Add("GSTINNO", GSTINNo);

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
                        GSTR3BResponse = result.ToString();
                    }
                }
                GST_Data = JsonConvert.DeserializeObject<GSPResponse>(GSTR3BResponse);

                string status_response = GST_Data.status_cd.ToString();
                if (status_response == "1")
                {
                    return "Payment of tax successfully done";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<GSPResponse>(GSTR3BResponse);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "Offset Liability GSTR3B data" + root.error.message + " for this " + GSTINNo + " & " + Period + ". Action - " + "", "Error Code - " + root.error.error_cd);
                    return root.error.message + " - " + "Error Code : " + root.error.error_cd;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}