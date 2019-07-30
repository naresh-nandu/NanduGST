using ClosedXML.Excel;
using SmartAdminMvc.Data_Access_Layer;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.Report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL.Common;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ReportController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly APIAnjularFunctions DbLayer = new APIAnjularFunctions();
        [HttpGet]
        public ActionResult AuditLog()
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                int cid = Convert.ToInt32(Session["Cust_ID"]);

                ViewBag.FromDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("MM/dd/yyyy");
                DateTime fromdate = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
                DateTime todate = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
                var ds = db.Retrieve_AuditLog1(cid, fromdate, todate).ToList();
                TempData["Refresh"] = ds;
                return View(ds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ActionName("AuditLog")]
        public ActionResult AuditLog1(FormCollection col)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                int cid = Convert.ToInt32(Session["Cust_ID"]);

                DateTime fromdate = DateTime.ParseExact(col["from"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime todate = DateTime.ParseExact(col["to"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                ViewBag.FromDate = col["from"];
                ViewBag.ToDate = col["to"];
                TempData["from"] = fromdate;
                TempData["to"] = todate;
                var sudit = db.Retrieve_AuditLog1(cid, fromdate, todate).ToList();
                TempData["Refresh"] = sudit;
                return View(sudit);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        [HttpPost]
        public ActionResult ExportData()
        {
            try
            {
                GridView gv = new GridView();
                gv.DataSource = TempData["Refresh"];
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=AuditLog Report.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("AuditLog");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet]
        public ActionResult Home()
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            try
            {
                ViewBag.FromDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.Panlist = LoadDropDowns.PanList(custid);
                ViewBag.GstinList = LoadDropDowns.GSTINList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Home(FormCollection Form, string command)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            try
            {
                string PANNo = Form["PANNo"];
                ViewBag.Panno = PANNo;
                string GSTINNo = Form["GSTINNo"];
                string FDate = Form["FromDate"];
                string TDate = Form["ToDate"];
                ViewBag.FromDate = FDate;
                ViewBag.ToDate = TDate;

                ViewBag.Panlist = LoadDropDowns.Exist_PanList(custid, PANNo);
                if (PANNo != null)
                {
                    ViewBag.GstinList = LoadDropDowns.GSTINList(PANNo);
                    if (GSTINNo != null)
                    {
                        ViewBag.GstinList = LoadDropDowns.Exist_GSTINList(PANNo, GSTINNo);
                    }
                }

                if (command == "submit")
                {
                    if (PANNo == "")
                    {
                        PANNo = "ALL";
                    }
                    if (GSTINNo == null || GSTINNo == "")
                    {
                        GSTINNo = "ALL";
                    }
                    var InvoiceCount = DbLayer.GetInvoiceCount(custid, userid, FDate, TDate, PANNo, GSTINNo);
                    ViewBag.InvoiceCount = InvoiceCount;

                    var APICount = DbLayer.GetAPICount(FDate, TDate, GSTINNo, PANNo, custid);
                    ViewBag.APICount = APICount;
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.Panlist = LoadDropDowns.PanList(custid);
                ViewBag.GstinList = LoadDropDowns.GSTINList();
            }
            return View();
        }


        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ReconcilationLog(int page = 0, string sort = null)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (sort != null || page != 0)
            {
                return View();
            }
            else
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;
                List<SelectListItem> Provinces1 = new List<SelectListItem>();
                Provinces1.Add(new SelectListItem() { Text = "GSTR2", Value = "GSTR2" });
                Provinces1.Add(new SelectListItem() { Text = "GSTR2A", Value = "GSTR2A" });
                ViewBag.GSTRList = new SelectList(Provinces1, "Value", "Text");

                return View();
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconcilationLog(FormCollection frm, string Export)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            string strGSTR = frm["ddlGSTR"];
            int strActionId = Convert.ToInt32(frm["ddlActionType"]);

            string strAction = (from lst1 in db.TBL_GSTR_ACTION_TYPE
                                where lst1.ActionId == strActionId
                                select lst1.ActionName).SingleOrDefault();

            if (!string.IsNullOrEmpty(Export))
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;
                List<SelectListItem> Provinces1 = new List<SelectListItem>();
                Provinces1.Add(new SelectListItem() { Text = "GSTR2", Value = "GSTR2" });
                Provinces1.Add(new SelectListItem() { Text = "GSTR2A", Value = "GSTR2A" });
                ViewBag.GSTRList = new SelectList(Provinces1, "Value", "Text");

                var das = GetDataSetReconcilationLog(strGSTR, strAction);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "Added Invocies";
                    ds.Tables[1].TableName = "Deleted Invocies";
                    ds.Tables[2].TableName = "Modified Invocies";

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            //Add DataTable as Worksheet.
                            wb.Worksheets.Add(dt);
                        }

                        //Export the Excel file.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=ReconcilationLog.xlsx");
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
                ViewData["Feedback"] = "Data Exported Successfully";
                return View();
            }
            else
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;
                List<SelectListItem> Provinces1 = new List<SelectListItem>();
                Provinces1.Add(new SelectListItem() { Text = "GSTR1", Value = "GSTR2" });
                Provinces1.Add(new SelectListItem() { Text = "GSTR2", Value = "GSTR2A" });
                ViewBag.GSTRList = new SelectList(Provinces1, "Value", "Text");

                return View();
            }
        }

        public static DataSet GetDataSetReconcilationLog(string GSTRType, string ActionType)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveGSTR2and2AHistoryReport", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@GSTRType", GSTRType));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
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
            return ds;
        }


        [System.Web.Mvc.HttpGet]
        public ActionResult LogFiles(string Name, string value)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!string.IsNullOrEmpty(TempData["Period"] as string))
            {
                string Period = TempData["Period"] as string;
                ViewBag.Period = Period;
                ViewBag.Period1 = Period;
                ViewBag.Period2 = DateTime.ParseExact(Period, "MMyyyy", CultureInfo.InvariantCulture).AddMonths(-1).ToString("MMyyyy");
                ViewBag.Period3 = DateTime.ParseExact(Period, "MMyyyy", CultureInfo.InvariantCulture).AddMonths(-2).ToString("MMyyyy");
            }
            else
            {
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.Period1 = DateTime.Now.ToString("MMyyyy");
                ViewBag.Period2 = DateTime.Now.AddMonths(-1).ToString("MMyyyy");
                ViewBag.Period3 = DateTime.Now.AddMonths(-2).ToString("MMyyyy");
            }

            string strPeriod = value;
            string strCustRefNo = Session["CustRefNo"].ToString();
            string strCustId = Session["Cust_ID"].ToString();
            string strUserId = Session["User_ID"].ToString();

            #region "OUTWARD"
            if (Name == "Outward")
            {
                var das = LogFilesDataModel.GetDataSetOutwardLog(strCustRefNo, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "AT";
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        ds.Tables[1].TableName = "B2B";
                    }
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        ds.Tables[2].TableName = "B2CL";
                    }
                    if (ds.Tables[3].Rows.Count > 0)
                    {
                        ds.Tables[3].TableName = "B2CS";
                    }
                    if (ds.Tables[4].Rows.Count > 0)
                    {
                        ds.Tables[4].TableName = "CDNR";
                    }
                    if (ds.Tables[5].Rows.Count > 0)
                    {
                        ds.Tables[5].TableName = "CDNUR";
                    }
                    if (ds.Tables[6].Rows.Count > 0)
                    {
                        ds.Tables[6].TableName = "DOCISSUE";
                    }
                    if (ds.Tables[7].Rows.Count > 0)
                    {
                        ds.Tables[7].TableName = "EXP";
                    }
                    if (ds.Tables[8].Rows.Count > 0)
                    {
                        ds.Tables[8].TableName = "HSN";
                    }
                    if (ds.Tables[9].Rows.Count > 0)
                    {
                        ds.Tables[9].TableName = "NIL";
                    }
                    if (ds.Tables[10].Rows.Count > 0)
                    {
                        ds.Tables[10].TableName = "TXP";
                    }
                    CommonFunctions.ExportExcel_XLSX(ds, "OutwardLog_" + strPeriod + ".xlsx");
                    
                }
                ViewData["Feedback"] = "Outward Data Exported Successfully";
            }
            #endregion

            #region "OUTWARD Amendments"
            #endregion

            #region "INWARD"
            if (Name == "Inward")
            {
                var das = LogFilesDataModel.GetDataSetInwardLog(strCustRefNo, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2B";
                    ds.Tables[1].TableName = "B2BUR";
                    ds.Tables[2].TableName = "CDNR";
                    ds.Tables[3].TableName = "CDNUR";
                    ds.Tables[4].TableName = "HSN";
                    ds.Tables[5].TableName = "IMPG";
                    ds.Tables[6].TableName = "IMPS";
                    ds.Tables[7].TableName = "ITCRVSL";
                    ds.Tables[8].TableName = "NIL";
                    ds.Tables[9].TableName = "TXI";
                    ds.Tables[10].TableName = "TXPD";

                    CommonFunctions.ExportExcel_XLSX(ds, "InwardLog_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "Inward Data Exported Successfully";
            }
            #endregion

            #region "GSTR1 UPLOADED"
            if (Name == "GSTR1Upload")
            {
                var das = LogFilesDataModel.GetDataSetGSTR1UploadedData(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2B";
                    ds.Tables[1].TableName = "B2CL";
                    ds.Tables[2].TableName = "B2CS";
                    ds.Tables[3].TableName = "EXP";
                    ds.Tables[4].TableName = "CDNR";
                    ds.Tables[5].TableName = "CDNUR";
                    ds.Tables[6].TableName = "HSN";
                    ds.Tables[7].TableName = "NIL";
                    ds.Tables[8].TableName = "TXP";
                    ds.Tables[9].TableName = "AT";
                    ds.Tables[10].TableName = "DOCISSUE";

                    CommonFunctions.ExportExcel_XLSX(ds, "GSTR1_UploadedData_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "GSTR1 UploadedData Exported Successfully";
            }
            #endregion

            #region "GSTR1 Amendments Uploaded"
            if (Name == "GSTR1AmendUpload")
            {
                var das = LogFilesDataModel.GetDataSetGSTR1AmendUploadedData(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2BA";
                    ds.Tables[1].TableName = "B2CLA";
                    ds.Tables[2].TableName = "B2CSA";
                    ds.Tables[3].TableName = "EXPA";
                    ds.Tables[4].TableName = "CDNRA";
                    ds.Tables[5].TableName = "CDNURA";
                    ds.Tables[6].TableName = "TXPA";
                    ds.Tables[7].TableName = "ATA";

                    CommonFunctions.ExportExcel_XLSX(ds, "GSTR1_Amendments_UploadedData_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "GSTR1 Amendments UploadedData Exported Successfully";
            }
            #endregion

            #region "GSTR1 SUMMARY"
            if (Name == "GSTR1Summary")
            {
                var das = LogFilesDataModel.GetDataSetGSTR1Summary(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "SUMMARY";

                    CommonFunctions.ExportExcel_XLSX(ds, "GSTR1_SummaryData_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "GSTR1 SummaryData Exported Successfully";
            }
            #endregion

            #region "GSTR2A DOWNLOADED"
            if (Name == "GSTR2A")
            {
                var das = LogFilesDataModel.GetDataSetGSTR2ADownloadedData(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "Base Data-B2B";
                    ds.Tables[1].TableName = "Modified B2B";
                    ds.Tables[2].TableName = "Base Data-CDNR";
                    ds.Tables[3].TableName = "Modified CDNR";

                    CommonFunctions.ExportExcel_XLSX(ds, "GSTR2A_DownloadedData_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "GSTR2A DownloadedData Exported Successfully";
            }
            #endregion

            #region "RECONCILIATION SUMMARY"
            if (Name == "ReconciliationSummary")
            {
                var das = LogFilesDataModel.GetDataSetReconciliationSummary(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2B-Matched Invocies";
                    ds.Tables[1].TableName = "CDN-Matched Invocies";
                    ds.Tables[2].TableName = "B2B-MissinginGSTR2A Invocies";
                    ds.Tables[3].TableName = "CDN-MissinginGSTR2A Invocies";
                    ds.Tables[4].TableName = "B2B-MissinginGSTR2 Invocies";
                    ds.Tables[5].TableName = "CDN-MissinginGSTR2 Invocies";
                    ds.Tables[6].TableName = "B2B-Mismatch Invocies";
                    ds.Tables[7].TableName = "CDN-Mismatch Invocies";

                    CommonFunctions.ExportExcel_XLSX(ds, "ReconciliationSummary_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "Inward Data Exported Successfully";
            }
            #endregion

            #region "AFTER RECONCILIATION"
            if (Name == "AfterReconciliation")
            {
                var das = LogFilesDataModel.GetDataSetReconciliationStatus(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2B Accepted from Missingin2A";
                    ds.Tables[1].TableName = "CDN Accepted from Missingin2A";
                    ds.Tables[2].TableName = "B2B-Rejected Invocies";
                    ds.Tables[3].TableName = "CDN-Rejected Invocies";
                    ds.Tables[4].TableName = "B2B-Modified Invocies";
                    ds.Tables[5].TableName = "CDN-Modified Invocies";
                    ds.Tables[6].TableName = "B2B-Hold Invocies";
                    ds.Tables[7].TableName = "CDN-Hold Invocies";

                    CommonFunctions.ExportExcel_XLSX(ds, "ReconciliationStatus_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "Inward Data Exported Successfully";
            }
            #endregion

            #region "GSTR2 UPLOADED"
            if (Name == "GSTR2Upload")
            {
                var das = LogFilesDataModel.GetDataSetGSTR2UploadedData(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2B";
                    ds.Tables[1].TableName = "B2BUR";
                    ds.Tables[2].TableName = "IMPG";
                    ds.Tables[3].TableName = "IMPS";
                    ds.Tables[4].TableName = "CDNR";
                    ds.Tables[5].TableName = "CDNUR";
                    ds.Tables[6].TableName = "HSN";
                    ds.Tables[7].TableName = "NIL";
                    ds.Tables[8].TableName = "TXPD";
                    ds.Tables[9].TableName = "TXI";
                    ds.Tables[10].TableName = "ITCRVSL";

                    CommonFunctions.ExportExcel_XLSX(ds, "GSTR2_UploadedData_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "GSTR2 UploadedData Exported Successfully";
            }
            #endregion

            #region "GSTR6 Imported"
            if (Name == "GSTR6Import")
            {
                var das = LogFilesDataModel.GetDataSetGSTR6Import(strCustRefNo, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2B";
                    ds.Tables[1].TableName = "CDNR";

                    CommonFunctions.ExportExcel_XLSX(ds, "GSTR6_ImportedData_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "GSTR6 Data Exported Successfully";
            }
            #endregion

            #region "GSTR6 UPLOADED"
            if (Name == "GSTR6Upload")
            {
                var das = LogFilesDataModel.GetDataSetGSTR6UploadData(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2B";
                    ds.Tables[1].TableName = "CDNR";

                    CommonFunctions.ExportExcel_XLSX(ds, "GSTR6_UploadedDate_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "GSTR6 UploadedData Exported Successfully";
            }
            #endregion

            #region "GSTR6 Reconciliation Summary"
            if (Name == "GSTR6ReconciliationSummary")
            {
                var das = LogFilesDataModel.GetDataSetGSTR6ReconciliationSummary(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2B-Matched Invocies";
                    ds.Tables[1].TableName = "CDN-Matched Invocies";
                    ds.Tables[2].TableName = "B2B-MissinginGSTR6A Invocies";
                    ds.Tables[3].TableName = "CDN-MissinginGSTR6A Invocies";
                    ds.Tables[4].TableName = "B2B-MissinginGSTR6 Invocies";
                    ds.Tables[5].TableName = "CDN-MissinginGSTR6 Invocies";
                    ds.Tables[6].TableName = "B2B-Mismatch Invocies";
                    ds.Tables[7].TableName = "CDN-Mismatch Invocies";

                    CommonFunctions.ExportExcel_XLSX(ds, "GSTR6_ReconciliationSummary_" + strPeriod + ".xlsx");
                }
                ViewData["Feedback"] = "GSTR6 Data Exported Successfully";
            }
            #endregion

            #region "AFTER RECONCILIATION GSTR6"
            if (Name == "AfterReconciliationGSTR6")
            {
                var das = LogFilesDataModel.GetDataSetReconciliationStatusGSTR6(strCustId, strUserId, strPeriod);

                using (DataSet ds = das)
                {
                    //Set Name of DataTables.
                    ds.Tables[0].TableName = "B2B-Accepted Invocies";
                    ds.Tables[1].TableName = "CDN-Accepted Invocies";
                    ds.Tables[2].TableName = "B2B-Rejected Invocies";
                    ds.Tables[3].TableName = "CDN-Rejected Invocies";
                    ds.Tables[4].TableName = "B2B-Modified Invocies";
                    ds.Tables[5].TableName = "CDN-Modified Invocies";
                    ds.Tables[6].TableName = "B2B-Pending Invocies";
                    ds.Tables[7].TableName = "CDN-Pending Invocies";

                    CommonFunctions.ExportExcel_XLSX(ds, "ReconciliationStatus_" + strPeriod + ".xlsx");                    
                }
                ViewData["Feedback"] = "Inward Data Exported Successfully";
            }
            #endregion

            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult LogFiles(FormCollection frm)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (frm["period"] == "")
            {
                TempData["ErrorMsg"] = "Please select valid period.";
                return View();
            }
            ViewBag.Period = frm["period"];
            ViewBag.Period1 = frm["period"];
            ViewBag.Period2 = DateTime.ParseExact(frm["period"], "MMyyyy", CultureInfo.InvariantCulture).AddMonths(-1).ToString("MMyyyy");
            ViewBag.Period3 = DateTime.ParseExact(frm["period"], "MMyyyy", CultureInfo.InvariantCulture).AddMonths(-2).ToString("MMyyyy");

            TempData["Period"] = ViewBag.Period;
            return View();
        }

        [HttpGet]
        public ActionResult GSTR3B()
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            }
            catch(Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }
            return View();
        }
    }
}