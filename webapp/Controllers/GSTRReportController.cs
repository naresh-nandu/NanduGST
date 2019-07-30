using ClosedXML.Excel;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL.GSTRReport;
using static WeP_DAL.GSTRReport.Gstr3B2AReportDal;
using static WeP_DAL.GSTRReport.Gstr3B2ASupplierReportDal;

namespace SmartAdminMvc.Controllers
{
    public class GstrReportController : Controller
    {
        #region "Reconciliation Of GSTR3B and GSTR2A"

        [HttpGet]
        public ActionResult GSTR2A3BReport()
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
            try
            {
                ViewBag.fromPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult GSTR2A3BReport(FormCollection Form, string Command)
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
            try
            {
                string strGSTIN = "", fromPeriod = "", toPeriod = "";
                strGSTIN = Form["ddlGSTINNo"];
                fromPeriod = Form["fromPeriod"];
                toPeriod = Form["toPeriod"];
                ViewBag.fromPeriod = fromPeriod;
                ViewBag.toPeriod = toPeriod;
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                switch (Command)
                {
                    case "viewReport":
                        #region "Data Retrieve for Report"
                        DataSet Report = Gstr3B2AReportBal.Retrieve_GSTR3BAndGSTR2A_Report(strGSTIN, fromPeriod, toPeriod);
                        TempData["GSTR3B2A"] = Report;
                        List<Gstr3B2AAttributes> ReportMgmt = new List<Gstr3B2AAttributes>();

                        #region "Data Assign to Attributes"
                        if (Report.Tables.Count > 0)
                        {
                            foreach (DataRow dr in Report.Tables[0].Rows)
                            {
                                ReportMgmt.Add(new Gstr3B2AAttributes
                                {
                                    Description = dr.IsNull("Description") ? "" : dr["Description"].ToString(),
                                    SGST = dr.IsNull("SGST") ? 0 : Convert.ToDecimal(dr["SGST"]),
                                    CGST = dr.IsNull("CGST") ? 0 : Convert.ToDecimal(dr["CGST"]),
                                    IGST = dr.IsNull("IGST") ? 0 : Convert.ToDecimal(dr["IGST"]),
                                    CESS = dr.IsNull("CESS") ? 0 : Convert.ToDecimal(dr["CESS"]),
                                    Total = dr.IsNull("Total") ? 0 : Convert.ToDecimal(dr["Total"]),

                                });
                            }
                        }
                        #endregion

                        Gstr2A3BReportViewModel Model = new Gstr2A3BReportViewModel();
                        Model.ReportMgmt = ReportMgmt;
                        ViewBag.GSTR3B2AReport = ReportMgmt.Count;
                        return View(Model);
                    #endregion

                    case "exportReport":
                        #region "export Report and Retrieve data once again"
                        GridView gv = new GridView();
                        gv.DataSource = TempData["GSTR3B2A"];
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        // Response.AddHeader("content-disposition", "attachment; filename=GSTR3B_GSTR2A_Comparision-Report.xls");
                        Response.AddHeader("content-disposition", "attachment; filename=GSTR3B_GSTR2A_Reconciliation-Report_ " + strGSTIN + "_" + fromPeriod + "-" + toPeriod + ".xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();

                        #region "Data Retrieve for Report"
                        DataSet Report1 = Gstr3B2AReportBal.Retrieve_GSTR3BAndGSTR2A_Report(strGSTIN, fromPeriod, toPeriod);
                        TempData["GSTR3B2A"] = Report1;
                        List<Gstr3B2AAttributes> ReportMgmt1 = new List<Gstr3B2AAttributes>();

                        #region "Data Assign to Attributes"
                        if (Report1.Tables.Count > 0)
                        {
                            foreach (DataRow dr in Report1.Tables[0].Rows)
                            {
                                ReportMgmt1.Add(new Gstr3B2AAttributes
                                {
                                    Description = dr.IsNull("Description") ? "" : dr["Description"].ToString(),
                                    SGST = dr.IsNull("SGST") ? 0 : Convert.ToDecimal(dr["SGST"]),
                                    CGST = dr.IsNull("CGST") ? 0 : Convert.ToDecimal(dr["CGST"]),
                                    IGST = dr.IsNull("IGST") ? 0 : Convert.ToDecimal(dr["IGST"]),
                                    CESS = dr.IsNull("CESS") ? 0 : Convert.ToDecimal(dr["CESS"]),
                                    Total = dr.IsNull("Total") ? 0 : Convert.ToDecimal(dr["Total"]),

                                });
                            }
                        }
                        #endregion

                        Gstr2A3BReportViewModel Model1 = new Gstr2A3BReportViewModel();
                        Model1.ReportMgmt = ReportMgmt1;
                        ViewBag.GSTR3B2AReport = ReportMgmt1.Count;
                        return View(Model1);
                        #endregion
                        #endregion
                }
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        #endregion

        #region "Supplier Wise Reconciliation Of GSTR3B and GSTR2A"
        [HttpGet]
        public ActionResult GSTR3B2ASupReport()
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
            try
            {
                ViewBag.fromDate = DateTime.Now.ToString("MMyyyy");
                ViewBag.toDate = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                string reportType = "GSTR3B Supplier Reconciliation";
                ViewBag.ReportType = Gstr3B2ASupplierReportBal.Existing_GetReportTypeList(reportType);
                ViewBag.fromPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }

            return View();
        }

        [HttpPost]
        public ActionResult GSTR3B2ASupReport(HttpPostedFileBase FileUpload, FormCollection Form, string Command)
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
            try
            {
                string strGSTIN = "", fromPeriod = "", toPeriod = "";
                strGSTIN = Form["ddlGSTINNo"];
                fromPeriod = Form["fromPeriod"];
                toPeriod = Form["toPeriod"];
                ViewBag.fromPeriod = fromPeriod;
                ViewBag.toPeriod = toPeriod;
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                string reportType = "GSTR3B Supplier Reconciliation";
                ViewBag.ReportType = Gstr3B2ASupplierReportBal.Existing_GetReportTypeList(reportType);

                switch (Command)
                {
                    case "Import":
                        #region "CSV Upload"
                        if (Request.Files.Count > 0)
                        {
                            var file = Request.Files[0];
                            if (file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = Path.GetFileName(FileUpload.FileName);
                                fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", userid.ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(fileName));
                                string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                                if (FileExtension.Trim() == "csv")
                                {
                                    if (file.ContentLength > 0 && file.ContentLength < 5242880)
                                    {
                                        string UserEmil = "";
                                        Gstr3B2ASupplierReportBal.getUserEmail(userid, out UserEmil);
                                        string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                        FileUpload.SaveAs(path);
                                        DataSet dsErrorRecords = new DataSet();
                                        int totalRecords = 0, processRecords = 0, errorRecords = 0;

                                        new Gstr3B2ASupplierReportBal(custid).ImportCSV(path, UserEmil, Session["CustRefNo"].ToString(), out totalRecords, out processRecords, out errorRecords, out dsErrorRecords);

                                        TempData["TotalRecordsCount"] = totalRecords.ToString();
                                        TempData["ProcessedRecordsCount"] = processRecords.ToString();
                                        TempData["ErrorRecordsCount"] = errorRecords.ToString();

                                        GridView grid = new GridView();
                                        grid.DataSource = dsErrorRecords.Tables[0];
                                        grid.DataBind();
                                        Session["ErrorRecordDownload"] = grid;

                                    }
                                    else
                                    {
                                        TempData["UploadMessage"] = "File size should be 1 kb to 5 MB only.";
                                    }
                                }
                                else
                                {
                                    TempData["UploadMessage"] = "File format should be .csv";
                                }
                            }
                            else
                            {
                                TempData["UploadMessage"] = "Please select a file.";
                            }

                        }
                        #endregion
                        break;

                    case "viewReport":
                        #region "Data Retrieve for Report"
                        DataSet Report = Gstr3B2ASupplierReportBal.Retrieve_GSTR3BAndGSTR2A_Supplier_Report(strGSTIN, fromPeriod, toPeriod);
                        TempData["Supplier_GSTR3B2A"] = Report;
                        List<Gstr3AGstr2AAttributes> ReportMgmt = new List<Gstr3AGstr2AAttributes>();

                        #region "Data Assign to Attributes"
                        if (Report.Tables.Count > 0)
                        {
                            foreach (DataRow dr in Report.Tables[0].Rows)
                            {
                                ReportMgmt.Add(new Gstr3AGstr2AAttributes
                                {
                                    Ctin = dr.IsNull("Supplier_GSTIN") ? "" : dr["Supplier_GSTIN"].ToString(),
                                    CtinName = dr.IsNull("Supplier_Name") ? "" : dr["Supplier_Name"].ToString(),
                                    Sgst = dr.IsNull("GSTR3B_SGST") ? 0 : Convert.ToDecimal(dr["GSTR3B_SGST"]),
                                    Cgst = dr.IsNull("GSTR3B_CGST") ? 0 : Convert.ToDecimal(dr["GSTR3B_CGST"]),
                                    Igst = dr.IsNull("GSTR3B_IGST") ? 0 : Convert.ToDecimal(dr["GSTR3B_IGST"]),
                                    Cess = dr.IsNull("GSTR3B_CESS") ? 0 : Convert.ToDecimal(dr["GSTR3B_CESS"]),
                                    Total = dr.IsNull("GSTR3B_TOTAL") ? 0 : Convert.ToDecimal(dr["GSTR3B_TOTAL"]),
                                    TaxVal = dr.IsNull("GSTR3B_TaxableValue") ? 0 : Convert.ToDecimal(dr["GSTR3B_TaxableValue"]),
                                    samt = dr.IsNull("GSTR2A_SGST") ? 0 : Convert.ToDecimal(dr["GSTR2A_SGST"]),
                                    camt = dr.IsNull("GSTR2A_CGST") ? 0 : Convert.ToDecimal(dr["GSTR2A_CGST"]),
                                    iamt = dr.IsNull("GSTR2A_IGST") ? 0 : Convert.ToDecimal(dr["GSTR2A_IGST"]),
                                    csamt = dr.IsNull("GSTR2A_CESS") ? 0 : Convert.ToDecimal(dr["GSTR2A_CESS"]),
                                    TotalValue = dr.IsNull("GSTR2A_TOTAL") ? 0 : Convert.ToDecimal(dr["GSTR2A_TOTAL"]),
                                    GrandTotal = dr.IsNull("Excess_GrandTotal") ? 0 : Convert.ToDecimal(dr["Excess_GrandTotal"]),
                                    taxval = dr.IsNull("GSTR2A_TaxableValue") ? 0 : Convert.ToDecimal(dr["GSTR2A_TaxableValue"]),

                                });
                            }
                        }
                        #endregion

                        Gstr3BGstr2AReportViewModel Model = new Gstr3BGstr2AReportViewModel();
                        Model.GSTRReportMgmt = ReportMgmt;
                        ViewBag.SupGSTR3B2AReport = ReportMgmt.Count;
                        return View(Model);
                    #endregion

                    case "exportReport":
                        #region "export Report and Retrieve data once again"
                        GridView gv = new GridView();
                        gv.DataSource = TempData["Supplier_GSTR3B2A"];
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        // Response.AddHeader("content-disposition", "attachment; filename=GSTR3B_GSTR2A_Comparision-Report.xls");
                        Response.AddHeader("content-disposition", "attachment; filename=GSTR3B_GSTR2A_Supplier_Reconciliation-Report_ " + strGSTIN + "_" + fromPeriod + "-" + toPeriod + ".xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();

                        DataSet Report1 = Gstr3B2ASupplierReportBal.Retrieve_GSTR3BAndGSTR2A_Supplier_Report(strGSTIN, fromPeriod, toPeriod);
                        TempData["Supplier_GSTR3B2A"] = Report1;
                        List<Gstr3AGstr2AAttributes> ReportMgmt1 = new List<Gstr3AGstr2AAttributes>();

                        #region "Data Assign to Attributes"
                        if (Report1.Tables.Count > 0)
                        {
                            foreach (DataRow dr in Report1.Tables[0].Rows)
                            {
                                ReportMgmt1.Add(new Gstr3AGstr2AAttributes
                                {
                                    Ctin = dr.IsNull("Supplier_GSTIN") ? "" : dr["Supplier_GSTIN"].ToString(),
                                    CtinName = dr.IsNull("Supplier_Name") ? "" : dr["Supplier_Name"].ToString(),
                                    Sgst = dr.IsNull("GSTR3B_SGST") ? 0 : Convert.ToDecimal(dr["GSTR3B_SGST"]),
                                    Cgst = dr.IsNull("GSTR3B_CGST") ? 0 : Convert.ToDecimal(dr["GSTR3B_CGST"]),
                                    Igst = dr.IsNull("GSTR3B_IGST") ? 0 : Convert.ToDecimal(dr["GSTR3B_IGST"]),
                                    Cess = dr.IsNull("GSTR3B_CESS") ? 0 : Convert.ToDecimal(dr["GSTR3B_CESS"]),
                                    Total = dr.IsNull("GSTR3B_TOTAL") ? 0 : Convert.ToDecimal(dr["GSTR3B_TOTAL"]),
                                    TaxVal = dr.IsNull("GSTR3B_TaxableValue") ? 0 : Convert.ToDecimal(dr["GSTR3B_TaxableValue"]),
                                    samt = dr.IsNull("GSTR2A_SGST") ? 0 : Convert.ToDecimal(dr["GSTR2A_SGST"]),
                                    camt = dr.IsNull("GSTR2A_CGST") ? 0 : Convert.ToDecimal(dr["GSTR2A_CGST"]),
                                    iamt = dr.IsNull("GSTR2A_IGST") ? 0 : Convert.ToDecimal(dr["GSTR2A_IGST"]),
                                    csamt = dr.IsNull("GSTR2A_CESS") ? 0 : Convert.ToDecimal(dr["GSTR2A_CESS"]),
                                    TotalValue = dr.IsNull("GSTR2A_TOTAL") ? 0 : Convert.ToDecimal(dr["GSTR2A_TOTAL"]),
                                    GrandTotal = dr.IsNull("Excess_GrandTotal") ? 0 : Convert.ToDecimal(dr["Excess_GrandTotal"]),
                                    taxval = dr.IsNull("GSTR2A_TaxableValue") ? 0 : Convert.ToDecimal(dr["GSTR2A_TaxableValue"]),

                                });
                            }
                        }
                        #endregion

                        Gstr3BGstr2AReportViewModel Model1 = new Gstr3BGstr2AReportViewModel();
                        Model1.GSTRReportMgmt = ReportMgmt1;
                        return View(Model1);

                        #endregion

                }
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }

            return View();
        }

        public ActionResult Download()
        {
            return new DownloadFileActionResult((GridView)Session["ErrorRecordDownload"], "ImportErrors.xls");

        }
        #endregion


        [HttpGet]
        #region
        public ActionResult Gstr3BReport()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.FromPeriod = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            return View();
        }


        [HttpPost]
        public ActionResult Gstr3BReport(FormCollection form)
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
            try
            {
                string todate, fromdate;
                todate = form["todate"];
                ViewBag.ToPeriod = todate;
                fromdate = form["fromdate"];
                ViewBag.FromPeriod = fromdate;

                var Report = Gstr3B2AReportBal.Report(fromdate, todate, custid);             
                using (DataSet ds = Report)
                {
                    //Set Name of DataTables.
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "GSTIN Wise";
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        ds.Tables[1].TableName = "Comapany&Pan Wise";
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
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment; filename=GSTR3B_ITC_LEDGER_OUTPUTTAX_REPORT_For_" + fromdate + "to"+ todate + ".xls");
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

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return View();
        }
        #endregion
    }
}