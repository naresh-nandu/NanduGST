using ClosedXML.Excel;
using SmartAdminMvc.Models.GSTR9API;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;
using WeP_DAL.GSTR9Attribute;

namespace SmartAdminMvc.Controllers
{
    public class Gstr9DownloadController : Controller
    {
        // GET: Gstr9Download
        #region"Download Auto Calc"
        public ActionResult Download()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.FinancialList();
            ViewBag.GetGSTR9DataList = WeP_BAL.EwayBill.LoadDropDowns.GetGSTR9DataList();
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            return View();
        }


        [HttpPost]
        public ActionResult Download(FormCollection form, string command, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string FromDate, ToDate, strGSTIN, returnJson = "";
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string FinancialYear = form["FinancialList"];
            Session["FinancialYear"] = FinancialYear;
            strGSTIN = form["strGSTIN"];
            Session["strGSTIN"] = strGSTIN;
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
            ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.Exist_FinancialList(FinancialYear);
            try
            {
                Gstr9DownloadApiCall Gstr9Api = new Gstr9DownloadApiCall();
                if (!String.IsNullOrEmpty(command))
                {
                    if (command == "getReport")
                    {
                        string str = FinancialYear.Substring(0, 4);
                        FromDate = str.Insert(0, "04");
                        Session["Fromdate"] = FromDate;
                        var result = FinancialYear.Substring(FinancialYear.Length - 4);
                        ToDate = result.Insert(0, "03");
                        Session["Todate"] = ToDate;

                        Gstr9Api.SendRequestAuto(strGSTIN, ToDate, Convert.ToString(custid), Convert.ToString(userid), Session["UserName"].ToString(), out returnJson);
                        TempData["SuccessMessage"] = returnJson;

                        DataSet gstr9 = Gstr9.Gstr9AutoCal(strGSTIN, ToDate);
                        TempData["gstr9"] = gstr9;
                        List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[0].Rows)
                            {
                                OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                                {
                                    Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                                    natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                                    txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                                    taxpayable = dr.IsNull("txpyble") ? 0 : Convert.ToDecimal(dr["txpyble"]),
                                    taxpaidthroughcash = dr.IsNull("txpaid_cash") ? 0 : Convert.ToDecimal(dr["txpaid_cash"]),
                                });
                            }
                        }
                        ReportViewModel Model = new ReportViewModel();

                        Model.ReportMgmt = OReportMgmt;
                        ViewBag.GetAuto = OReportMgmt.Count;
                        return View(Model);

                    }
                    else if (command == "ExportTemplate")
                    {
                        var das = Gstr9.Retrieve_GSTR9_Template(strGSTIN, Convert.ToString(Session["Fromdate"]), Convert.ToString(Session["todate"]));

                        using (DataSet ds = das)
                        {
                            //Set Name of DataTables.
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                ds.Tables[0].TableName = "GSTR9 Report";
                            }
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                foreach (DataTable dt in ds.Tables)
                                {
                                    //Add DataTable as Worksheet.
                                    if (dt.Rows.Count > 0)
                                    {
                                        wb.Worksheets.Add(dt);
                                    }
                                }
                                //Export the Excel file.
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment; filename=GSTR9Report_For_" + strGSTIN + "_" + FinancialYear + ".xls");
                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    if (wb.Worksheets.Count > 0)
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                    }
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                        }
                        return RedirectToAction("Download");
                    }
                    else if (command == "Export")
                    {
                        var das = Gstr9.Gstr9AutoCal(strGSTIN, Convert.ToString(Session["Todate"]));

                        using (DataSet ds = das)
                        {
                            //Set Name of DataTables.
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                ds.Tables[0].TableName = "GSTR9 Report";
                            }
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                foreach (DataTable dt in ds.Tables)
                                {
                                    //Add DataTable as Worksheet.
                                    if (dt.Rows.Count > 0)
                                    {
                                        wb.Worksheets.Add(dt);
                                    }
                                }
                                //Export the Excel file.
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment; filename=GSTR9Report_For_" + strGSTIN + "_" + FinancialYear + ".xls");
                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    if (wb.Worksheets.Count > 0)
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                    }
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                        }
                        return RedirectToAction("Download");
                    }
                }
                #region "OTP SUBMIT"
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    string strOTP = form["OTP"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTIN);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
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
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);
                    Session["Save"] = null;
                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
                #endregion
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return View();
        }
        #endregion



        #region"Download Get Details"
        [HttpGet]
        public ActionResult GetDetails()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.FinancialList();
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            return View();
        }
        [HttpPost]

        public ActionResult GetDetails(FormCollection form, string command, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string FromDate, ToDate, strGSTIN, returnJson = "";
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string FinancialYear = form["FinancialList"];
            Session["FinancialYear"] = FinancialYear;
            strGSTIN = form["strGSTIN"];
            Session["strGSTIN"] = strGSTIN;

            try
            {
                Gstr9DownloadApiCall Gstr9Api = new Gstr9DownloadApiCall();
                if (!String.IsNullOrEmpty(command))
                {
                    string str = FinancialYear.Substring(0, 4);
                    FromDate = str.Insert(0, "04");
                    Session["Fromdate"] = FromDate;
                    var result = FinancialYear.Substring(FinancialYear.Length - 4);
                    ToDate = result.Insert(0, "03");
                    Session["Todate"] = ToDate;
                    Gstr9Api.SendRequestGetDet(strGSTIN, ToDate, Convert.ToString(custid), Convert.ToString(userid), Session["UserName"].ToString(), out returnJson);
                    ViewBag.Gstr9Data = returnJson;
                }
                #region "OTP SUBMIT"
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    string strOTP = form["OTP"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTIN);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
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
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);
                    Session["Save"] = null;
                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
            }
            #endregion

            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
            ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.Exist_FinancialList(FinancialYear);

            return View();
        }
        #endregion
    }
}