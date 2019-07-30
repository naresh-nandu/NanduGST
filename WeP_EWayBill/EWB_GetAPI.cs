using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeP_DAL;
using WeP_BAL;
using System.Data;
using System.Collections;
using WeP_BAL.EwayBill;

namespace WeP_EWayBill
{
    public class EWB_GetAPI
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        private static string EWB_GENResponse = "";
        private static string URL = "";
        private static EWB_Response_Attributes EWB_Data = null;

        public static void GET_EWAYBILL(string strEwbNo, string GSTINNo, string SessionUserId,
            string SessionCustId, string SessionUsername, out string OStatus, out string OResponse)
        {
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            string ewayBillNo = "", ewayBillDate = "";
            int ewbid = 0, createdby = Convert.ToInt32(SessionUserId), Custid = Convert.ToInt32(SessionCustId);
            DateTime Createddate = DateTime.Now;
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));
                
                URL = ConfigurationManager.AppSettings["EWB_GETEWAYBILL"] + "?ewbno=" + strEwbNo;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    EWB_GENResponse = result.ToString();
                }
                
                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();
                    string rek_response = EWB_Data.rek.ToString();
                    string rek = EncryptionUtils.DecryptBySymmerticKey(rek_response, Convert.FromBase64String(sek));
                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(rek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    //var finaldata = JsonConvert.DeserializeObject<EWB_Response_Attributes>(DecryptjsonData);

                    #region "INSERT JSON DATA FROM GET API"
                    try
                    {
                        if (Con.State == ConnectionState.Closed)
                        {
                            Con.Open();
                        }

                        SqlCommand cmd = new SqlCommand("usp_Insert_JSON_EWB_GET_API_SA", Con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@userGSTIN", GSTINNo));
                        cmd.Parameters.Add(new SqlParameter("@RecordContents", DecryptjsonData.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@CreatedBy", SessionUserId));
                        cmd.Parameters.Add(new SqlParameter("@CustId", SessionCustId));
                        cmd.ExecuteNonQuery();
                        Con.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion

                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Downloaded Successfully... EWayBill No - " + ewayBillNo + " & EWayBill Date - " + ewayBillDate, "");

                    strResponse = "EWayBill Downloaded Successfully.";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Downloaded Error Code - " + strErrorCode + " & Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

                    strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
        }

        #region "Otherparty Eway Bill API"
        public static void GET_EWAYBILL_BY_Other(string strDate, string GSTINNo, string SessionUserId,
            string SessionCustId, string SessionUsername, out string OStatus, out string OResponse, out string EWBDate)
        {

            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            string ewayBillNo = "", ewayBillDate = "";
   
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                URL = ConfigurationManager.AppSettings["EWB_GETEWAYBILL_OTHERPARTY"] + "?date=" + strDate;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    EWB_GENResponse = result.ToString();
                }

                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();
                    string rek_response = EWB_Data.rek.ToString();
                    string rek = EncryptionUtils.DecryptBySymmerticKey(rek_response, Convert.FromBase64String(sek));
                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(rek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<List<EWB_Response_Attributes>>(DecryptjsonData);

                    #region "inserting to DB"
                    if (finaldata != null)
                    {
                       foreach(var data in finaldata)
                        {
                          EWB_BusinessLayer.Insert_Update_Otherpary(data.ewbNo,data.ewayBillDate, data.genMode,data.genGstin,data.docNo,data.docDate,data.fromGstin,
                              data.fromTradeName, data.toGstin,data.toTradeName, data.hsncode, data.hsndesc, data.status, data.rejectStatus, Convert.ToDecimal(data.totInvValue),
                              Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                        }
                    }
                    #endregion
                    
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill(s) Downloaded Successfully... EWayBill No - " + ewayBillNo + " & EWayBill Date - " + ewayBillDate, "");
                    strResponse = "EWayBill(s) Downloaded Successfully.";

                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Download failed Error Code - " + strErrorCode + " & Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);
                    strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }

            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
            EWBDate = ewayBillDate;
        }
        #endregion

        #region "Tranporter by Date API"
        public static void GET_EWAYBILL_Transporter(string strDate, string GSTINNo, string SessionUserId,
            string SessionCustId, string SessionUsername, out string OStatus, out string OResponse,out string EwayDate)
        {
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "", Date="";
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));
                
                URL = ConfigurationManager.AppSettings["EWB_GETEWAYBILL_TRANSPORTER"] + "?date=" + strDate;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    EWB_GENResponse = result.ToString();
                }

                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();
                    string rek_response = EWB_Data.rek.ToString();
                    string rek = EncryptionUtils.DecryptBySymmerticKey(rek_response, Convert.FromBase64String(sek));
                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(rek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<List<EWB_Response_Attributes>>(DecryptjsonData);
                   
                    #region "Inserting Values to DB"
                    if (finaldata != null)
                    {
                        foreach (var data in finaldata)
                        {
                            EWB_BusinessLayer.Insert_Update_TransPorter_GETAPI(data.ewbNo, data.ewbDate, data.genGstin, GSTINNo, data.docNo, data.docDate,
                               Convert.ToInt32(data.delPinCode), Convert.ToInt32(data.delStateCode), data.delPlace, data.validUpto, data.extendedTimes,
                               data.status, data.rejectStatus,"D", Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                        }
                    }
                    #endregion
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Downloaded Successfully.", "");

                    strResponse = "EWayBill Downloaded Successfully.";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Generation Error Code - " + strErrorCode + " & Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

                    strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
            EwayDate = Date;
        }
        #endregion

        #region "Transporter By Date And GSTIN API"
        public static void GET_EWAYBILL_TRANSPORTER_GSTIN(string strDate, string GSTIN, string GSTINNo, string SessionUserId,
            string SessionCustId, string SessionUsername, out string OStatus, out string OResponse,out string EwayDate)
        {
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "",Date="";
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));
                
                URL = ConfigurationManager.AppSettings["EWB_GETEWAYBILL_TRANSPORTER_GSTIN"] + "?Gen_gstin=" + GSTIN + "&date=" + strDate;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("client-id", "test_gsp69");
                httpWebRequest.Headers.Add("client-secret", "OUJDMTIzQEB1BU1NXT1JE139");
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    EWB_GENResponse = result.ToString();
                }


                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();
                    string rek_response = EWB_Data.rek.ToString();
                    string rek = EncryptionUtils.DecryptBySymmerticKey(rek_response, Convert.FromBase64String(sek));
                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(rek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<List<EWB_Response_Attributes>>(DecryptjsonData);
                   
                    #region "Inserting data to DB"
                    if (finaldata != null)
                    {
                        foreach (var data in finaldata)
                        {
                            EWB_BusinessLayer.Insert_Update_TransPorter_GETAPI(GSTINNo, data.ewbNo, data.ewbDate, data.genGstin, data.docNo, data.docDate,
                              Convert.ToInt32(data.delPinCode), Convert.ToInt32(data.delStateCode), data.delPlace, data.validUpto, data.extendedTimes,
                              data.status, data.rejectStatus, "G", Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                        }
                    }
                    #endregion

                     Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Downloaded Successfully.", "");

                    strResponse = "EWayBill Downloaded Successfully.";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Generation Error Code - " + strErrorCode + " & Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

                    strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            EwayDate = Date;
            OStatus = strStatus;
            OResponse = strResponse;
        }

        #endregion

        public static void GET_Consolidated_EWAYBILL(string strEwbNo, string GSTINNo, string SessionUserId,
            string SessionCustId, string SessionUsername, out string OStatus, out string OResponse)
        {
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "", cEwbNo = "", cEWBDate = "";
            int consewbid = 0, createdby = Convert.ToInt32(SessionUserId), CustId = Convert.ToInt32(SessionCustId);
            DateTime Createddate = DateTime.Now;
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));
                
                URL = ConfigurationManager.AppSettings["EWB_GETEWAYBILL_CONSOLIDATED"] + "?tripSheetNo=" + strEwbNo;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    EWB_GENResponse = result.ToString();
                }


                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();
                    string rek_response = EWB_Data.rek.ToString();
                    string rek = EncryptionUtils.DecryptBySymmerticKey(rek_response, Convert.FromBase64String(sek));
                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(rek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EWB_Response_Attributes>(DecryptjsonData);

                    #region "Inserting data to DB"
                    string flag = "G";
                    if (finaldata != null) { 
                        cEwbNo = finaldata.tripSheetNo;
                        cEWBDate = finaldata.enteredDate;
                        string fromPlace = finaldata.fromPlace;
                        string fromState = finaldata.fromState;
                        string vehicleNo = finaldata.vehicleNo;
                        string transMode = finaldata.transMode;
                        string transDocNo = finaldata.transDocNo;
                        string transDocDate = finaldata.transDocDate;
                        string userGstin = finaldata.userGstin;
                    int rowcount = EWAYBILLDataAccess.Check_ewyBillNo(cEwbNo, "CEWAY");
                    if (rowcount != 1)
                    {

                        string Query = "INSERT INTO TBL_EWB_GEN_CONSOLIDATED(cEwbNo,fromPlace,fromState,vehicleNo,transMode,transDocNo,transDocDate,userGstin,cEWBDate,flag,createdby,Createddate,CustId) VALUES(@cEwbNo,@fromPlace,@fromState,@vehicleNo,@transMode,@transDocNo,@transDocDate,@userGstin,@cEWBDate,@flag,@createdby,@createddate,@CustId)";
                        Query += "SELECT SCOPE_IDENTITY()";
                            using (SqlCommand cmd = new SqlCommand(Query))
                            {
                                cmd.Connection = Con;
                                if (Con.State == ConnectionState.Closed)
                                {
                                    Con.Open();
                                }

                                cmd.Parameters.AddWithValue("@cEwbNo", cEwbNo);
                                cmd.Parameters.AddWithValue("@fromPlace", fromPlace);
                                cmd.Parameters.AddWithValue("@fromState", fromState);
                                cmd.Parameters.AddWithValue("@vehicleNo", vehicleNo);
                                cmd.Parameters.AddWithValue("@transMode", transMode);
                                cmd.Parameters.AddWithValue("@transDocDate", transDocDate);
                                cmd.Parameters.AddWithValue("@transDocNo", transDocNo);
                                cmd.Parameters.AddWithValue("@userGstin", userGstin);
                                cmd.Parameters.AddWithValue("@cEWBDate", cEWBDate);
                                cmd.Parameters.AddWithValue("@flag", flag);
                                cmd.Parameters.AddWithValue("@createdby", createdby);
                                cmd.Parameters.AddWithValue("@createddate", Createddate);
                                cmd.Parameters.AddWithValue("@CustId", CustId);

                                consewbid = Convert.ToInt32(cmd.ExecuteScalar());
                                Con.Close();

                                if (finaldata.tripSheetEwbBills != null)
                                {
                                    foreach (var tripSheet in finaldata.tripSheetEwbBills)
                                    {
                                        string ewbNo = tripSheet.ewbNo;
                                        string ewbDate = tripSheet.ewbDate;
                                        string userGstins = tripSheet.userGstin;
                                        string docNo = tripSheet.docNo;
                                        string docDate = tripSheet.docDate;
                                        string assessValue = tripSheet.assessValue;
                                        string cgstValue = tripSheet.cgstValue;
                                        string sgstValue = tripSheet.sgstValue;
                                        string igstValue = tripSheet.igstValue;
                                        string cessValue = tripSheet.cessValue;

                                        if (Con.State == ConnectionState.Closed)
                                        {
                                            Con.Open();
                                        }
                                        SqlCommand cm = new SqlCommand();
                                        cm.Parameters.Clear();
                                        cm.Parameters.Add("@consewbid", SqlDbType.Int).Value = consewbid;
                                        cm.Parameters.Add("@ewbNo", SqlDbType.BigInt).Value = Convert.ToInt64(ewbNo);
                                        cm.Parameters.Add("@ewbDate", SqlDbType.VarChar).Value = ewbDate;
                                        cm.Parameters.Add("@userGstin", SqlDbType.VarChar).Value = userGstins;
                                        cm.Parameters.Add("@docNo", SqlDbType.VarChar).Value = docNo;
                                        cm.Parameters.Add("@docDate", SqlDbType.VarChar).Value = docDate;
                                        cm.Parameters.Add("@assessValue", SqlDbType.Decimal).Value = Convert.ToDecimal(assessValue);
                                        cm.Parameters.Add("@cgstValue", SqlDbType.Decimal).Value = Convert.ToDecimal(cgstValue);
                                        cm.Parameters.Add("@sgstValue", SqlDbType.Decimal).Value = Convert.ToDecimal(sgstValue);
                                        cm.Parameters.Add("@cessValue", SqlDbType.Decimal).Value = Convert.ToDecimal(cessValue);
                                        cm.Parameters.Add("@igstValue", SqlDbType.Decimal).Value = Convert.ToDecimal(igstValue);
                                        cm.Parameters.Add("@createdby", SqlDbType.Int).Value = createdby;
                                        cm.Parameters.Add("@createddate", SqlDbType.DateTime).Value = Createddate;
                                        cm.Parameters.Add("@CustId", SqlDbType.Int).Value = CustId;
                                        SQLHelper.InsertIntoTable("TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET", cm, Con);
                                        Con.Close();
                                    }
                                }

                            }
                        }
                        }
                    #endregion
                    
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "Cons.EWayBill Downloaded Successfully.", "");

                        strResponse = "Cons.EWayBill Downloaded Successfully.";
                    }
                else
                {
                        var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                        var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                        strErrorCode = root_error.errorcodes.ToString().Trim();
                        Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                        //Update_EwayBill(strDocNo, strDocDate, "", "", "", strStatus, strErrorCode, strErrorDesc);
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Generation Error Code - " + strErrorCode + " & Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

                        strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                    }
                }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
        }
       
        public static string GET_ErrorList(string GSTINNo)
        {
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "", cEwbNo = "", cEWBDate = "";
            //int consewbid = 0, createdby = Convert.ToInt32(SessionUserId), CustId = Convert.ToInt32(SessionCustId);
            DateTime Createddate = DateTime.Now;
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string receivedSEK = EncryptedSEK;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                URL = ConfigurationManager.AppSettings["EWB_GET_ERRORLIST"];
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 1000000;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    EWB_GENResponse = result.ToString();
                }


                EWB_Data = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();
                    string rek_response = EWB_Data.rek.ToString();
                    string rek = EncryptionUtils.DecryptBySymmerticKey(rek_response, Convert.FromBase64String(sek));
                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(rek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EWB_Response_Attributes>(DecryptjsonData);
                                        
                    //Helper.InsertAuditLog(SessionUserId, SessionUsername, "Cons.EWayBill Downloaded Successfully.", "");

                    return "ErrorList Downloaded Successfully.";
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EWB_Response_Attributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EWB_Response_Attributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    //Update_EwayBill(strDocNo, strDocDate, "", "", "", strStatus, strErrorCode, strErrorDesc);
                    //Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Generation Error Code - " + strErrorCode + " & Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

                    return "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
