using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartAdminMvc.Controllers
{
    public class Gstr3BController : Controller
    {
        // GET: GSTR3B
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult File()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            ViewBag.DSCList = LoadDropDowns.GetDSC_Certificates();
            ViewBag.FileTypeList = LoadDropDowns.GetFilingType();
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult File(FormCollection frm, string ViewSummary, string ESign, string btnDSC, string btnEVC, string GSTR3BFile,
            string GetJson, string OTPSubmit, string GSTR3BSubmit, string ExportSummary)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            string OutFileStatus = "0", OutFileResponse = "";
            try
            {
                string strFp = frm["period"];
                string strGSTINNo = frm["ddlGSTINNo"];
                string strFileType = frm["ddlFileType"];
                string strDSC = "";
                string strPanNo = frm["panno"];
                string strEVCOTP = frm["evcotp"];
                string flag = "1";
                string strReturnType = "";
                strReturnType = frm["nilreturn"];
                if (string.IsNullOrEmpty(strReturnType))
                {
                    strReturnType = "RETURN";                    
                }

                ViewBag.Period = strFp;
                ViewBag.PANNo = strPanNo;
                ViewBag.TitleHeaders = "GSTR-3B Submit & File";
                ViewBag.DSCList = LoadDropDowns.GetDSC_Certificates();
                ViewBag.FileTypeList = LoadDropDowns.Exist_GetFilingType(strFileType);
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTINNo, Session["Role_Name"].ToString());

                #region "GSTR-1 VIEW SUMMARY"
                if (!string.IsNullOrEmpty(ViewSummary))
                {
                    if (string.IsNullOrEmpty(strGSTINNo))
                    {
                        TempData["ErrorMessage"] = "Please Select GSTIN No";
                    }
                    else if (string.IsNullOrEmpty(strFp))
                    {
                        TempData["ErrorMessage"] = "Please Select Period";
                    }
                    else
                    {
                        GspGetGstr3BDataModel GSP_getGSTR3B = new GspGetGstr3BDataModel();
                        string DownloadRetSum = GSP_getGSTR3B.SendRequest(strGSTINNo, strFp, "RETSUM", "", "", "", "", "", Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), flag);
                        TempData["GSTR3BResponse"] = DownloadRetSum;
                    }
                }
                #endregion

                #region "Export GSTR-1 Summary"
                else if (!string.IsNullOrEmpty(ExportSummary))
                {
                    GridView gv = new GridView();
                    gv.DataSource = GetDatatableGSTR3BDownload(strGSTINNo, strFp, strFp);
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR3B_Summary_" + strGSTINNo + "_" + strFp + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    return View();
                }
                #endregion

                #region "GSTR-3B FILE"
                else if (!string.IsNullOrEmpty(GSTR3BFile))
                {
                    if (string.IsNullOrEmpty(strPanNo))
                    {
                        TempData["ErrorMessage"] = "Please Enter PAN NO for GSTR3B Filing";
                    }
                    else if (string.IsNullOrEmpty(strFileType))
                    {
                        TempData["ErrorMessage"] = "Please Select FILING Type for GSTR3B Filing";
                    }
                    else
                    {
                        string filingpath = Path.Combine(Server.MapPath("~/App_Data/ReturnFiling"), "GSTR3B_RETSUM_" + strGSTINNo + "_" + strFp + ".json");
                        if (System.IO.File.Exists(filingpath))
                        {
                            if (strFileType == "DSC")
                            {
                                string strJsonData = "";
                                if (System.IO.File.Exists(filingpath))
                                {
                                    using (StreamReader r = new StreamReader(filingpath))
                                    {
                                        strJsonData = r.ReadToEnd();
                                    }
                                }

                                string strPartA = "", strPartB = "", strFinalPart = "";
                                string strHashValue1 = "", strHashValue2 = "", FinalstrHashValue = "", FinalstrHashValue1 = "";
                                if (strReturnType == "RETURN")
                                {
                                    JObject objPartA = JObject.Parse(strJsonData.Trim());
                                    objPartA.Remove("tx_pmt");

                                    JObject obj = JsonConvert.DeserializeObject<JObject>(strJsonData);
                                    JObject objPartB = obj["tx_pmt"] as JObject;
                                    
                                    strPartA = JsonConvert.SerializeObject(objPartA, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                    strPartB = JsonConvert.SerializeObject(objPartB, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                    
                                    using (var sha256Obj = SHA256.Create())
                                    {
                                        byte[] outputBytes = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(strPartA));
                                        strHashValue1 = BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
                                    }
                                    using (var sha256Obj = SHA256.Create())
                                    {
                                        byte[] outputBytes = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(strPartB));
                                        strHashValue2 = BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
                                    }
                                    strFinalPart = strHashValue1 + strHashValue2;
                                    using (var sha256Obj = SHA256.Create())
                                    {
                                        byte[] outputBytes = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(strFinalPart));
                                        FinalstrHashValue = BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
                                    }
                                    ViewBag.HashValue = FinalstrHashValue;
                                    ViewBag.DSCDialog = "OPEN_DSCPOPUP";
                                }
                                if (strReturnType == "NIL RETURN")
                                {
                                    JObject objPartA = JObject.Parse(strJsonData.Trim());
                                    objPartA.Remove("tx_pmt");
                                    strFinalPart = JsonConvert.SerializeObject(objPartA, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                    using (var sha256Obj = SHA256.Create())
                                    {
                                        byte[] outputBytes = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(strFinalPart));
                                        FinalstrHashValue = BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
                                    
                                        byte[] outputBytes1 = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(FinalstrHashValue));
                                        FinalstrHashValue1 = BitConverter.ToString(outputBytes1).Replace("-", "").ToLower().Trim();
                                    }

                                    ViewBag.HashValue = FinalstrHashValue1;
                                    ViewBag.DSCDialog = "OPEN_DSCPOPUP";
                                }


                            }
                            else if (strFileType == "EVC")
                            {
                                if (!string.IsNullOrEmpty(strPanNo) && strPanNo.Length == 10)
                                {
                                    ViewBag.EVCAuthResponse = Models.GSTAUTH.GspGstEvcOtp.SendRequest(strGSTINNo, strPanNo, "R3B", Session["User_ID"].ToString(), Session["Cust_ID"].ToString());
                                }

                                ViewBag.EVCDialog = "OPEN_EVCPOPUP";
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Please do GSTR3B RETSUM Download for GSTR3B Filing";
                        }
                    }
                }
                #endregion

                #region "GET JSON PAYLOAD"
                else if (!string.IsNullOrEmpty(GetJson))
                {
                    //
                }
                #endregion

                #region "DSC REQUEST"
                else if (!string.IsNullOrEmpty(btnDSC))
                {                    
                    string strJsonData = "";
                    string filingpath = Path.Combine(Server.MapPath("~/App_Data/ReturnFiling"), "GSTR3B_RETSUM_" + strGSTINNo + "_" + strFp + ".json");
                    if (System.IO.File.Exists(filingpath))
                    {
                        using (StreamReader r = new StreamReader(filingpath))
                        {
                            strJsonData = r.ReadToEnd();
                        }
                        strDSC = frm["outputsigneddata"];

                        new GspSendGstr3BSubmitFileDataModel(strJsonData, strGSTINNo, strFp, Session["Cust_ID"].ToString(), Session["User_ID"].ToString(),
                                            Session["UserName"].ToString()).GSTR3BFileRequest(strFileType, strDSC, strPanNo, strEVCOTP, out OutFileStatus, out OutFileResponse);

                        if (OutFileStatus == "0")
                        {
                            new GspSendGstr3BSubmitFileDataModel(strJsonData, strGSTINNo, strFp, Session["Cust_ID"].ToString(), Session["User_ID"].ToString(),
                                            Session["UserName"].ToString()).GSTR3BFileRequest(strFileType, strDSC, strPanNo, strEVCOTP, out OutFileStatus, out OutFileResponse);
                        }
                        if (OutFileStatus == "1")
                        {
                            TempData["GSTR3BResponse"] = OutFileResponse;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = OutFileResponse;
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please do GSTR3B RETSUM Download for GSTR3B Filing";
                    }
                }
                #endregion

                #region "EVC REQUEST"
                else if (!string.IsNullOrEmpty(btnEVC))
                {
                    string strJsonData = "";
                    string filingpath = Path.Combine(Server.MapPath("~/App_Data/ReturnFiling"), "GSTR3B_RETSUM_" + strGSTINNo + "_" + strFp + ".json");
                    if (System.IO.File.Exists(filingpath))
                    {
                        using (StreamReader r = new StreamReader(filingpath))
                        {
                            strJsonData = r.ReadToEnd();
                        }

                        new GspSendGstr3BSubmitFileDataModel(strJsonData, strGSTINNo, strFp, Session["Cust_ID"].ToString(), Session["User_ID"].ToString(),
                                            Session["UserName"].ToString()).GSTR3BFileRequest(strFileType, strDSC, strPanNo, strEVCOTP, out OutFileStatus, out OutFileResponse);

                        if (OutFileStatus == "0")
                        {
                            new GspSendGstr3BSubmitFileDataModel(strJsonData, strGSTINNo, strFp, Session["Cust_ID"].ToString(), Session["User_ID"].ToString(),
                                            Session["UserName"].ToString()).GSTR3BFileRequest(strFileType, strDSC, strPanNo, strEVCOTP, out OutFileStatus, out OutFileResponse);
                        }
                        if (OutFileStatus == "1")
                        {
                            TempData["GSTR3BResponse"] = OutFileResponse;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = OutFileResponse;
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please do GSTR3B RETSUM Download for GSTR3B Filing";
                    }
                }
                #endregion

                #region "OTP SUBMIT"
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    string strOTP = frm["OTP"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTINNo);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    if (status == "1")
                    {
                        TempData["AuthMsg"] = "Authenticated Successfully";
                    }
                    else
                    {
                        TempData["AuthMsg"] = status;
                    }
                }
                #endregion

                #region "ONCHANGE EVENT"
                else
                {
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTINNo;
                }
                #endregion

                ViewBag.GSTR3BFile = GetListGSTR3BDownload(strGSTINNo, strFp, strFp);
            }
            catch (Exception ex)
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        private object GetDatatableGSTR3BDownload(string strGSTIN, string strPeriod, string strtoperiod)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", strGSTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtoperiod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ds.Tables[0];
        }

        private List<IDictionary> GetListGSTR3BDownload(string strGSTIN, string strPeriod, string strtoperiod)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", strGSTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtoperiod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(ds.Tables[1]);
        }

        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }
    }
}