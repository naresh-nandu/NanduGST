using ClosedXML.Excel;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR1;
using SmartAdminMvc.Models.SearchTaxPayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL.ViewAndTrack;

namespace SmartAdminMvc.Controllers
{
    public class GstrViewTrackStatusController : Controller
    {
        // GET: GSTR Status Download
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
                int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
                string Period = TempData["Period"] as string;
                string strtoPeriod = TempData["toPeriod"] as string;
                ViewBag.Period = Period;
                ViewBag.toPeriod = strtoPeriod;

                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection frm, string Download, string Export, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            string strGSTIN = "", strType = "", strPeriod = "", strAction = "", strfromPeriod = "", strtoPeriod = "";

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];
                strType = frm["type"];
                strPeriod = frm["period"];
                strfromPeriod = frm["period"];
                strtoPeriod = frm["toperiod"];
                strAction = frm["action"];

                ViewBag.Period = strPeriod;
                ViewBag.toPeriod = strtoPeriod;
                TempData["gstn"] = strGSTIN;
                TempData["period"] = strPeriod;
                TempData["toPeriod"] = strtoPeriod;
                TempData["Action"] = strAction;

                String fromm = DateTime.ParseExact(strPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
                String too = DateTime.ParseExact(strtoPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
                DateTime from_dt = Convert.ToDateTime(fromm);
                DateTime to_dt = Convert.ToDateTime(too);

                String diff = (to_dt - from_dt).TotalDays.ToString();
                int diff_dt = Convert.ToInt32(diff);
                if (diff_dt < 0)
                {
                    TempData["Message"] = "To Date should always be greater than or equal to from Date";
                }

                if (!string.IsNullOrEmpty(Download))
                {
                    List<IDictionary> GridFill = null;
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.CustomerGSTIN(iCustId);

                    GspGetGstrStatusDataModel GSP_getGSTR = new GspGetGstrStatusDataModel();

                    string from_period = strPeriod;
                    string to_period = strtoPeriod;
                    string from_month = from_period.Substring(0, 2);
                    string from_year = from_period.Substring(from_period.Length - 4);
                    string to_month = to_period.Substring(0, 2);
                    string to_year = to_period.Substring(to_period.Length - 4);
                    int cons = 12;
                    int count1 = 1;
                    int frm_mon = Convert.ToInt32(from_month);
                    int to_mon = Convert.ToInt32(to_month);
                    int diff_month = cons - frm_mon;
                    int count_month = diff_month + to_mon;
                    int frm_yr = Convert.ToInt32(from_year);
                    int diff_year = Convert.ToInt32(to_year) - frm_yr;

                    if (diff_year > 1)
                    {
                        count_month = diff_month + (cons * (diff_year - 1)) + to_mon;
                    }

                    if (to_year == from_year)
                    {
                        int months = to_mon - frm_mon;
                        for (int i = 0; i <= months; i++)
                        {
                            string period;
                            if (i == 0)
                            {
                                period = from_period;
                            }
                            else
                            {
                                frm_mon++;
                                period = Convert.ToString(frm_mon) + from_year;
                                if (period.Length == 5)
                                {
                                    period = "0" + period;
                                }
                            }
                            strPeriod = period;
                            string DownloadRes = GSP_getGSTR.SendRequest(strGSTIN, strPeriod, strAction, strType, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                            ViewBag.DownloadResponse = DownloadRes;
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= count_month; i++)
                        {
                            string period;
                            if (i == 0)
                            {
                                period = from_period;
                            }
                            else
                            {
                                frm_mon++;
                                if (frm_mon > 12)
                                {
                                    frm_mon = count1;
                                    frm_yr++;
                                }
                                period = Convert.ToString(frm_mon) + Convert.ToString(frm_yr);
                                if (period.Length == 5)
                                {
                                    period = "0" + period;
                                }
                            }
                            strPeriod = period;
                            string DownloadRes = GSP_getGSTR.SendRequest(strGSTIN, strPeriod, strAction, strType, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                            ViewBag.DownloadResponse = DownloadRes;
                        }
                    }

                    ViewBag.TitleHeaders = "GSTR View & Track Status Download";
                    return View(GridFill);
                }
                else if (!string.IsNullOrEmpty(Export))
                {
                    GridView gv = new GridView();
                    gv.DataSource = TempData["ExportData"] as DataTable;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR1_Download.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.CustomerGSTIN(iCustId);
                    return View();
                }
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    string strOTP = frm["OTP"].ToString();
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
                else
                {
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.DownloadResponse = ex.Message.ToString();
                return View();
            }
        }


        #region "View and Track Status without authentication"

        [HttpGet]
        public ActionResult Home()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            ViewBag.SourceType = LoadDropDowns.GetViewAndTrackSourceType();
            ViewBag.FinancialYear = LoadDropDowns.GetFinancialYear();

            return View();
        }

        [HttpPost]
        public ActionResult Home(HttpPostedFileBase FileUpload, FormCollection frm, string btnCheck, string btnCheckALL, string btnDownload)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            string strUserName = Session["UserName"].ToString();
            try
            {
                string strData = "";
                int iStatus = 3;

                ViewBag.SourceType = LoadDropDowns.GetViewAndTrackSourceType();
                ViewBag.FinancialYear = LoadDropDowns.GetFinancialYear();

                string sourceType = "", financialYear = "";
                sourceType = frm["SourceType"];
                financialYear = frm["FinancialYear"];
                #region "BULK ViewandTrack Of GSTIN Checking"
                if (!string.IsNullOrEmpty(btnCheckALL))
                {

                    ViewBag.SourceType = LoadDropDowns.Exist_GetViewAndTrackSourceType(sourceType);
                    ViewBag.FinancialYear = LoadDropDowns.Exist_GetFinancialYear(financialYear);
                    switch (sourceType)
                    {
                        case "1":
                            DataSet CustomerCtin = ViewAndTrackStatus.getCtin(iCustId, "1");
                            if (CustomerCtin.Tables.Count > 0)
                            {
                                foreach (DataRow dr in CustomerCtin.Tables[0].Rows)
                                {
                                    string GSTINNo = dr.IsNull("GSTINno") ? "" : dr["GSTINno"].ToString();
                                    GspSearchTaxPayerGstin.SendRequest_ViewAndTrack(GSTINNo, financialYear,"Customer",Convert.ToString(iUserId), Convert.ToString(iCustId), strUserName,"", out iStatus, out strData);
                                }
                                TempData["Message"] = "View and Track Status successfully Completed... Please check in the Report";
                            }
                            break;
                        case "2":
                            DataSet SupplierCtin = ViewAndTrackStatus.getCtin(iCustId, "2");
                            if (SupplierCtin.Tables.Count > 0)
                            {
                                foreach (DataRow dr in SupplierCtin.Tables[0].Rows)
                                {
                                    string GSTINNo = dr.IsNull("GSTINno") ? "" : dr["GSTINno"].ToString();
                                    GspSearchTaxPayerGstin.SendRequest_ViewAndTrack(GSTINNo, financialYear,"Supplier", Convert.ToString(iUserId), Convert.ToString(iCustId), strUserName, "", out iStatus, out strData);
                                }
                                TempData["Message"] = "View and Track Status successfully Completed... Please check in the Report";
                            }
                            break;
                        case "3":
                            DataTable dt = new DataTable();
                            DataRow row;
                            List<string> lstGSTINNo = new List<string>();
                            if (Request.Files.Count > 0)
                            {
                                var file = Request.Files[0];
                                if (file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                                {
                                    string fileName = Path.GetFileName(FileUpload.FileName);
                                    fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", iUserId.ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(fileName));
                                    Session["FileName"] = fileName.Trim();
                                    string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                                    if (FileExtension.Trim() == "csv" && file.ContentLength > 0 && file.ContentLength < 5242880)
                                    {
                                        string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                        FileUpload.SaveAs(path);

                                        StreamReader sr = new StreamReader(path);
                                        string line = sr.ReadLine();
                                        string[] value = line.Split(',');

                                        foreach (string dc in value)
                                        {
                                            dt.Columns.Add(new DataColumn(dc));
                                        }
                                        int i = 0;
                                        while (!sr.EndOfStream)
                                        {
                                            line = sr.ReadLine();
                                            value = line.Split(',');
                                            if (value.Length == dt.Columns.Count)
                                            {
                                                row = dt.NewRow();
                                                row.ItemArray = value;
                                                dt.Rows.Add(row);
                                                if (dt.Rows.Count == 60000)
                                                {
                                                    break;
                                                }
                                                if (dt.Columns.Count == 56) // Template A
                                                {
                                                    lstGSTINNo.Add(dt.Rows[i][6].ToString().Trim());
                                                }
                                                else if (dt.Columns.Count == 52) // Template B
                                                {
                                                    lstGSTINNo.Add(dt.Rows[i][4].ToString().Trim());
                                                }
                                                else if (dt.Columns.Count == 46) // Template B
                                                {
                                                    lstGSTINNo.Add(dt.Rows[i][4].ToString().Trim());
                                                }
                                                else // Other Source
                                                {
                                                    lstGSTINNo.Add(dt.Rows[i][0].ToString().Trim());
                                                }
                                            }
                                            i++;
                                        }
                                        sr.Close();

                                        lstGSTINNo.RemoveAll(x => x == "");
                                        lstGSTINNo = lstGSTINNo.Distinct().ToList();
                                        if (lstGSTINNo.Count > 0)
                                        {
                                            for (int j = 0; j < lstGSTINNo.Count; j++)
                                            {
                                                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                                                {
                                                    using (SqlCommand sqlcmd = new SqlCommand())
                                                    {

                                                        if (lstGSTINNo[j].Trim().Length == 15)
                                                        {
                                                            GspSearchTaxPayerGstin.SendRequest_ViewAndTrack(lstGSTINNo[j].Trim(), financialYear,"CSV", Convert.ToString(iUserId), Convert.ToString(iCustId), strUserName, fileName, out iStatus, out strData);
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    TempData["Message"] = "View and Track Status successfully Completed... Please check in the Report";
                                }

                            }

                            break;
                    }

                }
                #endregion

                #region "Export"
                else if (!string.IsNullOrEmpty(btnDownload))
                {
                    if (sourceType == "3")

                    {

                        var das = new ViewAndTrackStatus(iCustId, sourceType, financialYear).GetFilingStatus_CSV(Session["FileName"].ToString());

                        using (DataSet ds = das)

                        {

                            //Set Name of DataTables.

                            if (ds.Tables[0].Rows.Count > 0)

                            {

                                ds.Tables[0].TableName = "GSTR1";

                            }

                            if (ds.Tables[1].Rows.Count > 0)

                            {

                                ds.Tables[1].TableName = "GSTR3B";

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

                                Response.AddHeader("content-disposition", "attachment; filename=GSTR Filing Status For_" + financialYear + ".xls");



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

                    }

                    else
                    {

                        var das = new ViewAndTrackStatus(iCustId, sourceType, financialYear).GetFilingStatus();

                        using (DataSet ds = das)

                        {

                            //Set Name of DataTables.

                            if (ds.Tables[0].Rows.Count > 0)

                            {

                                ds.Tables[0].TableName = "GSTR1";

                            }

                            if (ds.Tables[1].Rows.Count > 0)

                            {

                                ds.Tables[1].TableName = "GSTR3B";

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

                                Response.AddHeader("content-disposition", "attachment; filename=GSTR Filing Status For_" + financialYear + ".xls");



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



                    }

                    return View();
                }
                #endregion
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();

            }
            return View();
        }

        #endregion
    }
}