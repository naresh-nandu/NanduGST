using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeP_BAL;
using WeP_BAL.EwayBill;
using WeP_DAL;

namespace WeP_EWayBill
{
    public class EwbGeneration
    {
        protected EwbGeneration()
        {
            //
        }

        #region "EWAYBILL GENERATION"
        public static void EWAYBILL_GEN(string strDocNo, string strDocDate, string JsonData, string GSTINNo, string SessionUserId,
         string SessionCustId, string SessionUsername, out string OStatus, out string OResponse, out string OEwbNo, out string OEwbDate)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string EWB_GENResponse = "", CompanyName = "";
            EwbResponseAttributes EWB_Data = null;
            string strEWBNo = "", strEWBDate = "", strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            decimal WalletBalance = 0;
            string GSK = "";
            bool isChecked = false;
           
            try
            {
                Helper.GetWalletBalance(SessionCustId, "EWAYBILL", "", "", "", out WalletBalance, out GSK, out isChecked);
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EwbRequestAttributes objEWBGEN = new EwbRequestAttributes();
                objEWBGEN.action = "GENEWAYBILL";
                objEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

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
                        EWB_GENResponse = result.ToString();
                    }
                }

                EWB_Data = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    //if (GSK == "True")
                    //{
                    //    if (isChecked)
                    //    {
                    //        // Already Wallet Deducted
                    //    }
                    //    else
                    //    {
                    //        Helper.UpdateWalletBalance(SessionCustId, "EWAYBILL", 10, "", "", "");
                    //        Helper.UpdateWalletBalanceRequest(SessionCustId, 10, "EWAYBILL");
                    //    }
                    //}
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EwbResponseAttributes>(DecryptjsonData);
                    string ewbno = finaldata.ewayBillNo;
                    strEWBNo = ewbno;
                    string ewbdate = finaldata.ewayBillDate;
                    strEWBDate = ewbdate;
                    string validupto = finaldata.validUpto;
                    GetCompName(GSTINNo, out CompanyName);
                    Update_EwayBill(GSTINNo, strDocNo, strDocDate, ewbno, ewbdate, validupto, strStatus, "", "", SessionCustId, SessionUserId, CompanyName);
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Generated Successfully... EWayBill No - " + ewbno + " & EWayBill Date - " + ewbdate, "");
                    
                    strResponse = "EWayBill Generated Successfully. EWayBill No - " + ewbno + " : EWayBill Date - " + ewbdate;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EwbResponseAttributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    Update_EwayBill(GSTINNo, strDocNo, strDocDate, "", "", "", strStatus, strErrorCode, strErrorDesc, SessionCustId, SessionUserId,"");
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Generation Error Code - " + strErrorCode + " & Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);

                    strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OEwbNo = strEWBNo;
            OEwbDate = strEWBDate;
            OStatus = strStatus;
            OResponse = strResponse;
        }

        public static void EWAYBILL_GEN_THREAD(string strRefIds, string strGSTINNo, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string strStatus = "0", strResponse = "", strEWBNo = "", strEWBDate = "";
            try
            {
                string[] strEWBIDS = strRefIds.Split(',');
                for (int i = 0; i < strEWBIDS.Count(); i++)
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        conn.Open();
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = conn;
                            sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_GENERATION where ewbid = @EwbId";
                            sqlcmd.Parameters.AddWithValue("@EwbId", strEWBIDS[i]);
                            using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable dt = new DataTable();
                                adt.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    string strDocNo = dt.Rows[0]["docNo"].ToString();
                                    string strDocDate = dt.Rows[0]["docDate"].ToString();
                                    string strJsonData = EwbJsonDataModel.GetJsonEWBGeneration(strGSTINNo, strDocNo, strDocDate);

                                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                                    Task.Factory.StartNew(() => EWAYBILL_GEN(strDocNo, strDocDate, strJsonData, strGSTINNo,
                                        SessionUserId, SessionCustId, SessionUsername, out strStatus, out strResponse, out strEWBNo, out strEWBDate)
                                    );
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
                Helper.InsertAuditLog(SessionUserId, SessionUsername, "EWayBill Generation Error - " + strResponse, "");
            }
        }

        public static void Update_EwayBill(string strGSTINNo, string strDocNo, string strDocDate, string strEwbNo, string strEwbDate, string strValidUpto,
            string strStatus, string strErrorCode, string strErrorDesc, string strCustId, string strUserId,string CompanyName)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                conn.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_GENERATION where userGSTIN = @GSTINNo and docNo = @DocNo and docDate = @DocDate and ISNULL(ewayBillNo, '') = '' and CustId = @CustId Order By 1 DESC";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    sqlcmd.Parameters.AddWithValue("@DocNo", strDocNo);
                    sqlcmd.Parameters.AddWithValue("@DocDate", strDocDate);
                    sqlcmd.Parameters.AddWithValue("@CustId", strCustId);
                    
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string ewbId = dt.Rows[0]["ewbid"].ToString();
                            string vehicleno = dt.Rows[0]["vehicleNo"].ToString();
                            string fromplace = dt.Rows[0]["fromPlace"].ToString();
                            string fromstatecode = dt.Rows[0]["fromStateCode"].ToString();
                            string transmode = dt.Rows[0]["transMode"].ToString();
                            string transdocno = dt.Rows[0]["transDocNo"].ToString();
                            string transdocdate = dt.Rows[0]["transDocDate"].ToString();

                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@ewayBillNo", SqlDbType.NVarChar).Value = strEwbNo.Trim();
                            cmd.Parameters.Add("@ewayBillDate", SqlDbType.NVarChar).Value = strEwbDate.Trim();
                            if (!string.IsNullOrEmpty(strValidUpto))
                            {
                                cmd.Parameters.Add("@validUpto", SqlDbType.NVarChar).Value = strValidUpto.Trim();
                            }
                            cmd.Parameters.Add("@CompanyName", SqlDbType.NVarChar).Value = CompanyName.Trim();
            
                            cmd.Parameters.Add("@EWB_status", SqlDbType.NVarChar).Value = strStatus.Trim();
                            cmd.Parameters.Add("@EWB_errorCodes", SqlDbType.NVarChar).Value = strErrorCode.Trim();
                            cmd.Parameters.Add("@EWB_errorDescription", SqlDbType.NVarChar).Value = strErrorDesc.Trim();
                            SQLHelper.UpdateTable("TBL_EWB_GENERATION", "ewbId", ewbId, cmd, conn);
                            

                            // INSERT TRANSPORTER DETAILS IN UPDATE VEHICLENO TABLE FOR PRINT PURPOSE
                            if (transmode != "0")
                            {
                                EwaybillDataAccess.Insert_Vechicle_Details_SA(strGSTINNo, strEwbNo, strEwbDate, vehicleno, fromplace, fromstatecode, "", "",
                                            transmode, transdocno, transdocdate, Convert.ToInt32(strCustId), Convert.ToInt32(strUserId));
                            }
                        }
                    }
                }
                conn.Close();
            }
        }

        public static void GetCompName(string strGSTINNo, out string CompanyName)
        {

            string CompName = "";
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from TBL_Customer where GSTINNo= '" + strGSTINNo + "' and rowstatus = 1", Con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                     CompName = dt.Rows[0]["CompanyName"].ToString();
                    if (String.IsNullOrEmpty(CompName))
                    {
                        CompName = "";
                    }
                }
            }
            CompanyName = CompName;
        }
            #endregion

            #region "CONSOLIDATED EWAYBILL GENERATION"
            public static void CONS_EWB_GEN(string strtransDocNo, string strtransDocDate, string JsonData, string GSTINNo,
            string SessionUserId, string SessionCustId, string SessionUsername, out string OStatus, out string OResponse, out string OCEWBNo)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string EWB_GENResponse = "";
            EwbResponseAttributes EWB_Data = null;
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "", strcEWBNo = "";
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EwbRequestAttributes objCONSEWBGEN = new EwbRequestAttributes();
                objCONSEWBGEN.action = "GENCEWB";
                objCONSEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objCONSEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

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
                        EWB_GENResponse = result.ToString();
                    }
                }

                EWB_Data = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EwbResponseAttributes>(DecryptjsonData);
                    string cewbno = finaldata.cEwbNo;
                    strcEWBNo = cewbno;
                    string cewbdate = finaldata.cEWBDate;
                    Update_ConsEwayBill(strtransDocNo, strtransDocDate, cewbno, cewbdate, strStatus, "", "");
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "Consolidated EWayBill Generated Successfully...  Consolidated EWayBill No - " + cewbno + " & Consolidated EWayBill Date : " + cewbdate, "");

                    strResponse = "Consolidated EWayBill Generated Successfully... Consolidated EWayBill No - " + cewbno + " : Consolidated EWayBill Date : " + cewbdate;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EwbResponseAttributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);

                    if (!string.IsNullOrEmpty(strErrorDesc))
                    {
                        Update_ConsEwayBill(strtransDocNo, strtransDocDate, "", "", strStatus, strErrorCode, strErrorDesc);
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "Consolidated EWayBill Generation Error Code : " + strErrorCode + " & Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);
                        strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                    }
                    else
                    {
                        Update_ConsEwayBill(strtransDocNo, strtransDocDate, "", "", strStatus, "", strErrorCode);
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "Consolidated EWayBill Generation Error Message - " + strErrorCode, "Error Code - " + strErrorCode);
                        strResponse = root_error.errorcodes;
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OCEWBNo = strcEWBNo;
            OStatus = strStatus;
            OResponse = strResponse;
        }

        public static void CONS_EWAYBILL_GEN_THREAD(string strRefIds, string strGSTINNo, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string strStatus = "0", strResponse = "", strCEWBNo = "";
            try
            {
                string[] strCONSEWBIDS = strRefIds.Split(',');
                for (int i = 0; i < strCONSEWBIDS.Count(); i++)
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        conn.Open();
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = conn;
                            sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_GEN_CONSOLIDATED where consewbid = @ConsEwbId";
                            sqlcmd.Parameters.AddWithValue("@ConsEwbId", strCONSEWBIDS[i]);
                            using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable dt = new DataTable();
                                adt.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    string strTransDocNo = dt.Rows[0]["transDocNo"].ToString();
                                    string strTransDocDate = dt.Rows[0]["transDocDate"].ToString();
                                    string strJsonData = EwbJsonDataModel.GetJsonCONSEWBGeneration(strGSTINNo, strTransDocNo, strTransDocDate);

                                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                                    Task.Factory.StartNew(() => CONS_EWB_GEN(strTransDocNo, strTransDocDate, strJsonData, strGSTINNo,
                                        SessionUserId, SessionCustId, SessionUsername, out strStatus, out strResponse, out strCEWBNo)
                                    );
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
                Helper.InsertAuditLog(SessionUserId, SessionUsername, "Consolidated EWayBill Generation Error - " + strResponse, "");
            }
        }

        public static void Update_ConsEwayBill(string strtransDocNo, string strtransDocDate, string strConsEwbNo, string strConsEwbDate,
            string strStatus, string strErrorCode, string strErrorDesc)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                conn.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_GEN_CONSOLIDATED where transDocNo = @TransDocNo and transDocDate = @TransDocDate and ISNULL(cEwbNo, '') = '' Order By 1 DESC";
                    sqlcmd.Parameters.AddWithValue("@TransDocNo", strtransDocNo);
                    sqlcmd.Parameters.AddWithValue("@TransDocDate", strtransDocDate);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string consewbId = dt.Rows[0]["consewbid"].ToString();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@cEwbNo", SqlDbType.NVarChar).Value = strConsEwbNo.Trim();
                            cmd.Parameters.Add("@cEwbDate", SqlDbType.NVarChar).Value = strConsEwbDate.Trim();
                            cmd.Parameters.Add("@CEWB_status", SqlDbType.NVarChar).Value = strStatus.Trim();
                            cmd.Parameters.Add("@CEWB_errorCodes", SqlDbType.NVarChar).Value = strErrorCode.Trim();
                            cmd.Parameters.Add("@CEWB_errorDescription", SqlDbType.NVarChar).Value = strErrorDesc.Trim();
                            SQLHelper.UpdateTable("TBL_EWB_GEN_CONSOLIDATED", "consewbId", consewbId, cmd, conn);
                            
                        }
                    }
                }
                conn.Close();
            }
        }
        #endregion

        #region "EWAYBILL UPDATE VEHICLENO"
        public static void EWB_UPDATE_VEHICLENO(string strEwbNo, string JsonData, string GSTINNo,
            string SessionUserId, string SessionCustId, string SessionUsername, out string OStatus, out string OResponse)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string EWB_GENResponse = "";
            EwbResponseAttributes EWB_Data = null;
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(GSTINNo, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EwbRequestAttributes objCONSEWBGEN = new EwbRequestAttributes();
                objCONSEWBGEN.action = "VEHEWB";
                objCONSEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objCONSEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", GSTINNo);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey_EWB"]);

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
                        EWB_GENResponse = result.ToString();
                    }
                }

                EWB_Data = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EwbResponseAttributes>(DecryptjsonData);
                    string vehUpdDate = finaldata.vehUpdDate;
                    string validUpto = finaldata.validUpto;
                    Update_EWayBillVehicleNo(strEwbNo, vehUpdDate, validUpto, strStatus, "", "");
                    Update_EWayBillVehicleNoINSA(strEwbNo, vehUpdDate, validUpto, strStatus, "", "");

                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "Vehicle No Updated Successfully for this EWayBill No - " + strEwbNo + " and Vehicle Updated Date : " + vehUpdDate, "");

                    strResponse = "Vehicle No Updated Successfully for this EWayBill No - " + strEwbNo + " and Vehicle Updated Date : " + vehUpdDate;
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EwbResponseAttributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString().Trim();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);

                    if (!string.IsNullOrEmpty(strErrorDesc))
                    {
                        Update_EWayBillVehicleNo(strEwbNo, "", "", strStatus, strErrorCode, strErrorDesc);
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "Update Vehicle No Error Code : " + strErrorCode + " & Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);
                        strResponse = "Error Code - " + root_error.errorcodes + " and Error Message - " + strErrorDesc;
                    }
                    else
                    {
                        Update_EWayBillVehicleNo(strEwbNo, "", "", strStatus, "", strErrorCode);
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "Update Vehicle No Error Message - " + strErrorCode, "Error Code - " + strErrorCode);
                        strResponse = root_error.errorcodes;
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            OStatus = strStatus;
            OResponse = strResponse;
        }

        public static void EWB_UPDATE_VEHICLENO_THREAD(string strRefIds, string strGSTINNo, string SessionUserId, string SessionCustId, string SessionUsername)
        {
            string strStatus = "0", strResponse = "";
            try
            {
                string[] strUPDVEHIDS = strRefIds.Split(',');
                for (int i = 0; i < strUPDVEHIDS.Count(); i++)
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        conn.Open();
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = conn;
                            sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_UPDATE_VEHICLENO where ewbVehUpdid = @VehUpdId";
                            sqlcmd.Parameters.AddWithValue("@VehUpdId", strUPDVEHIDS[i]);
                            using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable dt = new DataTable();
                                adt.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    string strEWBNo = dt.Rows[0]["EwbNo"].ToString();
                                    string strVehNo = dt.Rows[0]["vehicleNo"].ToString();
                                    string strJsonData = EwbJsonDataModel.GetJsonEWBUpdateVehicleNo(strGSTINNo, strEWBNo, strVehNo);
                                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');

                                    Task.Factory.StartNew(() => EWB_UPDATE_VEHICLENO(strEWBNo, strJsonData, strGSTINNo,
                                            SessionUserId, SessionCustId, SessionUsername, out strStatus, out strResponse)
                                    );
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
                Helper.InsertAuditLog(SessionUserId, SessionUsername, "Consolidated EWayBill Generation Error - " + strResponse, "");
            }
        }

        public static void Update_EWayBillVehicleNo(string strEwbNo, string strVehicleUpdateDate, string strValidUpto,
            string strStatus, string strErrorCode, string strErrorDesc)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                conn.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_UPDATE_VEHICLENO where EwbNo = @EwbNo Order by ewbVehUpdid desc";
                    sqlcmd.Parameters.AddWithValue("@EwbNo", strEwbNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string ewbVehUpdid = dt.Rows[0]["ewbVehUpdid"].ToString();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@vehUpdDate", SqlDbType.NVarChar).Value = strVehicleUpdateDate.Trim();
                            cmd.Parameters.Add("@validUpto", SqlDbType.NVarChar).Value = strValidUpto.Trim();
                            cmd.Parameters.Add("@UPD_status", SqlDbType.NVarChar).Value = strStatus.Trim();
                            cmd.Parameters.Add("@UPD_errorCodes", SqlDbType.NVarChar).Value = strErrorCode.Trim();
                            cmd.Parameters.Add("@UPD_errorDescription", SqlDbType.NVarChar).Value = strErrorDesc.Trim();
                            SQLHelper.UpdateTable("TBL_EWB_UPDATE_VEHICLENO", "ewbVehUpdid", ewbVehUpdid, cmd, conn);                            
                        }
                    }
                }
                conn.Close();
            }
        }
        #endregion



        public static void Update_EWayBillVehicleNoINSA(string strEwbNo, string strVehicleUpdateDate, string strValidUpto,
            string strStatus, string strErrorCode, string strErrorDesc)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                conn.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_GENERATION where ewayBillNo = @ewayBillNo";
                    sqlcmd.Parameters.AddWithValue("@ewayBillNo", strEwbNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string ewbid = dt.Rows[0]["ewbid"].ToString();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@validUpto", SqlDbType.NVarChar).Value = strValidUpto.Trim();
                            SQLHelper.UpdateTable("TBL_EWB_GENERATION", "ewbid", ewbid, cmd, conn);
                        }
                    }
                }
                conn.Close();
            }
        }
 

        #region "REGENERATE CONSOLIDATE EWAYBILL"
        public static void EWB_REGENERATE_CONS(string strGSTIN,string JsonData, string cewbNo,string fromPlace,string fromState,string transMode, string transNo, string transDate, string vehicleNo, string SessionUserId, string SessionCustId,
            string SessionUsername, out string OStatus, out string OResponse)
        {
            string Username = "";
            string AppKey = "";
            string EncryptedSEK = "";
            string AuthToken = "";
            string EWB_GENResponse = "";
            EwbResponseAttributes EWB_Data = null;
            string strStatus = "0", strResponse = "", strErrorCode = "", strErrorDesc = "";
            try
            {
                Helper.EWB_DataAdapter(strGSTIN, out Username, out AppKey, out EncryptedSEK, out AuthToken);

                string decrypted_appkey = AppKey;
                string sek = EncryptionUtils.DecryptBySymmerticKey(EncryptedSEK, Convert.FromBase64String(decrypted_appkey));

                byte[] encodejson = UTF8Encoding.UTF8.GetBytes(JsonData);
                string json = Convert.ToBase64String(encodejson);
                string payload = EncryptionUtils.EncryptBySymmerticKey(json, sek);

                EwbRequestAttributes objEWBGEN = new EwbRequestAttributes();
                objEWBGEN.action = "REGENTRIPSHEET";
                objEWBGEN.data = payload;

                string tempjsondata = JsonConvert.SerializeObject(objEWBGEN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["EWB_GENERATION"]);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("authtoken", AuthToken);
                httpWebRequest.Headers.Add("gstin", strGSTIN);

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
                        EWB_GENResponse = result.ToString();
                    }
                }

                EWB_Data = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);

                string status_response = EWB_Data.status.ToString();
                strStatus = status_response;
                if (status_response == "1")
                {
                    string data_response = EWB_Data.data.ToString();

                    string DecryptjsonData = EncryptionUtils.DecryptBySymmerticKey(data_response, Convert.FromBase64String(sek));
                    DecryptjsonData = Helper.Base64Decode(DecryptjsonData);
                    var finaldata = JsonConvert.DeserializeObject<EwbResponseAttributes>(DecryptjsonData);
                    if (finaldata != null)
                    {
                        string REGENDetails = fromPlace + ',' + fromState + ',' + vehicleNo + ',' + transMode + ',' + transNo + ',' + transDate;
                            EwbBusinessLayer.Insert_EWB_CONS_REGENERATE(cewbNo,finaldata.tripSheetNo,"YES", REGENDetails, status_response,"","",
                                Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                        Helper.InsertAuditLog(SessionUserId, SessionUsername, "Consolidate EwayBill Regenerated Successfully for this CewbNo " + cewbNo + " :New CewbNo - " + finaldata.tripSheetNo + "","");
                        strResponse = "Consolidate EwayBill Regenerated Successfully for this CewbNo " + cewbNo + " :New CewbNo - " + finaldata.tripSheetNo + "";
                    }
                }
                else
                {
                    var root = JsonConvert.DeserializeObject<EwbResponseAttributes>(EWB_GENResponse);
                    var root_error = JsonConvert.DeserializeObject<EwbResponseAttributes>(Helper.Base64Decode(root.error));
                    strErrorCode = root_error.errorcodes.ToString();
                    Helper.EWB_Error_DataAdapter(strErrorCode, out strErrorDesc);
                    EwbBusinessLayer.Insert_EWB_CONS_REGENERATE(cewbNo,"", "NO", "", status_response, strErrorCode, strErrorDesc,
                               Convert.ToInt32(SessionCustId), Convert.ToInt32(SessionUserId));
                    Helper.InsertAuditLog(SessionUserId, SessionUsername, "Consolidate EwayBill Regenerated Failed. Error Code - " + strErrorCode + " and Error Message - " + strErrorDesc, "Error Code - " + strErrorCode);
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
        #endregion
    }
}
